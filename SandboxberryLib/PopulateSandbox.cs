// SandboxBerry - tool for copying test data into Salesforce sandboxes
// Copyright (C) 2017 Ian Finch
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandboxberryLib.InstructionsModel;
using SandboxberryLib.ResultsModel;
using SandboxberryLib.SalesforcePartnerApi;
using log4net;
using System.Xml;

namespace SandboxberryLib
{
    public class PopulateSandbox
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PopulateSandbox));


        private SalesforceTasks _sourceTasks;
        private SalesforceTasks _targetTasks;

        private SbbInstructionSet _instructions;
        private RelationMapper _relationMapper;

        private IProgress<string> _progress;

        public PopulateSandbox(SbbInstructionSet instructions):this(instructions, new RelationMapper())
        {
        }

        public PopulateSandbox(SbbInstructionSet instructions, RelationMapper relMapper)
        {
            _sourceTasks = new SalesforceTasks(instructions.SourceCredentials);
            _targetTasks = new SalesforceTasks(instructions.TargetCredentuals);
            _instructions = instructions;
            _relationMapper = relMapper;

        }

        public PopulateSandboxResult Start()
        {
            return this.Start(null);
        }

        public void ProgressUpdate(string message)
        {
            if (_progress == null)
            {
                logger.DebugFormat("Progress message: {0}", message);
            }
            else
                _progress.Report(message);
        }

        public PopulateSandboxResult Start(IProgress<string> progress)
        {
            _progress = progress;
            var res = new PopulateSandboxResult();
            ProgressUpdate("Logging in to Salesforce API ...");
            LoginToBoth();
            ProgressUpdate("Fetching metadata...");
            var apiNameArray = _instructions.SbbObjects.Select(o => o.ApiName).ToArray();
            _sourceTasks.FetchObjectMetadata(apiNameArray);
            ProgressUpdate("Getting inactive users...");
            var inactiveUserIds = _targetTasks.GetInactiveUsers();
            string currentUserId = _targetTasks.GetCurrentUserId();
            ProgressUpdate("Checking for missing users...");
            var sourceUserIds = _sourceTasks.GetAllUsers();
            var targetUserIds = _targetTasks.GetAllUsers();
            var missingUserIds = sourceUserIds.Except(targetUserIds).ToList();
            logger.DebugFormat("Found {0} users in Source that are not in Target", missingUserIds.Count());
            var processedObjects = new List<String> { "User" }; // user is copied already
            var lookupsToRevisit = new List<LookupInfo>();

            foreach (SbbObject objLoop in _instructions.SbbObjects)
            {
                ProgressUpdate(string.Format("Starting to process {0}", objLoop.ApiName));
                var objres = new PopulateObjectResult();
                var transformer = new ObjectTransformer();
                transformer.RelationMapper = _relationMapper;
                transformer.InactiveUserIds = inactiveUserIds;
                transformer.MissingUserIds = missingUserIds;
                transformer.CurrentUserId = currentUserId;
                transformer.SbbObjectInstructions = objLoop;

                objres.ApiName = objLoop.ApiName;
                res.ObjectResults.Add(objres);

                transformer.ObjectRelationships = _sourceTasks.GetObjectRelationships(objLoop.ApiName);
                transformer.RecursiveRelationshipField = transformer.ObjectRelationships.FirstOrDefault(d => d.Value == objLoop.ApiName).Key;

                // find lookups on this object that can't be populated, to revisit later
                var missingLookups = transformer.ObjectRelationships
                    .Where(d => d.Value != objLoop.ApiName)          // where it's not a recursive relationship
                    .Where(d => !processedObjects.Contains(d.Value)) // and this lookup can't be populated because we haven't already processed this object (i.e. the referenced record doesn't exist yet)
                    .Where(d => !objLoop.SbbFieldOptions.Any(        // and it's not one of the skipped fields
                                e => e.ApiName.Equals(d.Key)
                                     && e.Skip))
                    // TODO: but is still one of the included object types (e.g. not Contact -> "rh2__PS_Describe__c")
                    .Select(d => new LookupInfo
                    {
                        FieldName = d.Key,
                        ObjectName = objLoop.ApiName,
                        RelatedObjectName = d.Value
                    });
                
                lookupsToRevisit = lookupsToRevisit.Concat(missingLookups).ToList();

                if (transformer.RecursiveRelationshipField != null)
                    logger.DebugFormat("Object {0} has a recurive relation to iteself in field {1}",
                        objLoop.ApiName, transformer.RecursiveRelationshipField);


                List<sObject> sourceData = null;
                try
                {
                    sourceData = _sourceTasks.GetDataFromSObject(objLoop.ApiName, objLoop.Filter);
                }
                catch (Exception e)
                {
                    string errMess = string.Format("Error while fetching data for {0}: {1}", objLoop.ApiName, e.Message);
                    throw new ApplicationException(errMess, e);
                }  
                objres.SourceRows = sourceData.Count();
                ProgressUpdate(string.Format("Received {0} {1} records from source", sourceData.Count, objLoop.ApiName));

                // get working info and transform objects
                var workingList = new List<ObjectTransformer.sObjectWrapper>();
                foreach (sObject rowLoop in sourceData)
                {
                    var wrap = new ObjectTransformer.sObjectWrapper();
                    wrap.OriginalId = rowLoop.Id;
                    wrap.sObj = rowLoop;

                    transformer.ApplyTransformations(wrap);

                    
                    workingList.Add(wrap);
                }

                // insert objects in batches
                int batchSize = 100;
                int done = 0;
                bool allDone = false;
                if (workingList.Count == 0)
                    allDone = true;
                while (!allDone)
                {
                    var workBatch = workingList.Skip(done).Take(batchSize).ToList();
                    done += workBatch.Count;
                    if (done >= workingList.Count)
                        allDone = true;

                    var insertRes = _targetTasks.InsertSObjects(objLoop.ApiName,
                        workBatch.Select(w => w.sObj).ToArray());

                    for (int i = 0; i < insertRes.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(insertRes[i].NewId))
                        {
                            workBatch[i].NewId = insertRes[i].NewId;
                            objres.SuccessCount += 1;
                            _relationMapper.Remember(objLoop.ApiName, workBatch[i].OriginalId, workBatch[i].NewId);
                        }
                        else
                        {
                            workBatch[i].ErrorMessage = insertRes[i].ErrorMessage;
                            logger.WarnFormat("Error when inserting {0} {1} into target: {2}",
                                objLoop.ApiName, workBatch[i].OriginalId, workBatch[i].ErrorMessage);
                            objres.FailCount += 1;
                        }

                    }
                   
                }

                // inserts done.
                // if there's a recurive field, do update
                if (transformer.RecursiveRelationshipField != null)
                    UpdateRecursiveField(objLoop.ApiName, workingList, transformer.RecursiveRelationshipField);


                ProgressUpdate(string.Format("Summary for {0}: Success {1} Fail {2}",
                    objLoop.ApiName, objres.SuccessCount, objres.FailCount));
                processedObjects.Add(objLoop.ApiName);
            }

            // revisit lookups that will be missing and populate them now that all the records are
            // loaded into the sandbox
            foreach(var missingLookup in lookupsToRevisit)
            {
                var transformer = new ObjectTransformer();
                transformer.RelationMapper = _relationMapper;

                // retrieve data from the source org just for this field
                var objres = new PopulateObjectResult();
                List<sObject> sourceData = null;

                try
                {
                    // get all records where this lookup has a value
                    sourceData = _sourceTasks.GetDataFromSObject(
                        sobjectName: missingLookup.ObjectName, //needs to be account, not contact
                        colList:     new List<string> { "Id", missingLookup.FieldName },
                        filter:      missingLookup.FieldName + " <> ''");
                    // TODO: also include filters on this sobject from the instructions file
                }
                catch (Exception e)
                {
                    string errMess = string.Format("Error while fetching data for {0}: {1}", missingLookup.ObjectName, e.Message);
                    throw new ApplicationException(errMess, e);
                }

                objres.SourceRows = sourceData.Count();
                ProgressUpdate(string.Format("Received {0} {1} records from source", sourceData.Count, missingLookup.ObjectName));

                // switch the IDs with the new ones in the sandbox
                // get working info and transform objects
                var workingList = new List<ObjectTransformer.sObjectWrapper>();
                foreach (sObject rowLoop in sourceData)
                {
                    rowLoop.Any = rowLoop.Any.Where(e => e.LocalName != "Id").ToArray();
                    var wrap = new ObjectTransformer.sObjectWrapper();
                    wrap.OriginalId = rowLoop.Id;
                    wrap.sObj = rowLoop;

                    transformer.FixRelatedIds(rowLoop,
                        new Dictionary<string, string>
                        {
                            [missingLookup.FieldName] = missingLookup.RelatedObjectName
                        });

                    // also update the ID of the object itself
                    wrap.sObj.Id = _relationMapper.RecallNewId(missingLookup.ObjectName, wrap.sObj.Id);

                    if (wrap.sObj.Id != null)
                    {
                        workingList.Add(wrap);
                    }
                }

                // update records in batches
                int batchSize = 100;
                int done = 0;
                bool allDone = false;
                if (workingList.Count == 0)
                    allDone = true;
                while (!allDone)
                {
                    var workBatch = workingList.Skip(done).Take(batchSize).ToList();
                    done += workBatch.Count;
                    if (done >= workingList.Count)
                        allDone = true;

                    var insertRes = _targetTasks.UpdateSObjects(missingLookup.ObjectName,
                        workBatch.Select(w => w.sObj).ToArray());

                    for (int i = 0; i < insertRes.Length; i++)
                    {
                        if (insertRes[i].Success)
                        {
                            objres.SuccessCount += 1;
                        }
                        else
                        {
                            workBatch[i].ErrorMessage = insertRes[i].ErrorMessage;
                            logger.WarnFormat("Error when updating {0} {1} into target: {2}",
                                missingLookup.ObjectName, workBatch[i].OriginalId, workBatch[i].ErrorMessage);
                            objres.FailCount += 1;
                        }
                    }
                }
            }

            // log summary
            ProgressUpdate("************************************************");
            foreach (var resLoop in res.ObjectResults)
            {
                ProgressUpdate(string.Format("Summary for {0}: Success {1} Fail {2}",
                 resLoop.ApiName, resLoop.SuccessCount, resLoop.FailCount));

            }
            ProgressUpdate("************************************************");
            
            return res;
        }

      

        


        private void UpdateRecursiveField(string apiName, List<ObjectTransformer.sObjectWrapper> workingList, string recursiveRelationshipField)
        {
            logger.DebugFormat("Object {0} has a recursive relation field {1}, now doing second pass to set it....",
                apiName, recursiveRelationshipField);

            // make a new list of sObjects to do the update
            List<sObject> updateList = new List<sObject>();
            foreach (var wrapLoop in workingList)
            {
                if (!string.IsNullOrEmpty(wrapLoop.RecursiveRelationshipOriginalId))
                {
                    var upd = new sObject();

                    upd.type = wrapLoop.sObj.type;
                    upd.Id = wrapLoop.NewId;
                    XmlDocument dummydoc = new XmlDocument();
                    XmlElement recursiveEl = dummydoc.CreateElement(recursiveRelationshipField);

                    string replaceValue = _relationMapper.RecallNewId(apiName, wrapLoop.RecursiveRelationshipOriginalId);

                    if (replaceValue == null)
                    {
                        logger.DebugFormat("Object {0} {1} recursive field {2} have value {3} could not translate - will ignore",
                           apiName, wrapLoop.OriginalId, recursiveRelationshipField, wrapLoop.RecursiveRelationshipOriginalId);
                    }
                    else
                    {

                        recursiveEl.InnerText = replaceValue;

                        upd.Any = new XmlElement[] { recursiveEl };

                        updateList.Add(upd);
                    }
                }

            }

            logger.DebugFormat("{0} rows in Object {1} have recursive relation {2} to update ....",
                updateList.Count(), apiName, recursiveRelationshipField);

            // update objects in batches
            int successCount = 0;
            int failCount = 0;
            int done = 0;
            bool allDone = false;
            if (updateList.Count == 0)
                allDone = true;
            while (!allDone)
            {
                var batch = updateList.Skip(done).Take(100).ToList();
                done += batch.Count;
                if (done >= updateList.Count)
                    allDone = true;

                var updateRes = _targetTasks.UpdateSObjects(apiName,
                       batch.ToArray());

                for (int i = 0; i < updateRes.Length; i++)
                {
                    if (updateRes[i].Success)
                    {
                        successCount+=1;
                    }
                    else
                    {
                        
                        logger.WarnFormat("Error when updating {0} {1} in target: {2}",
                            apiName, batch[i].Id, updateRes[i].ErrorMessage);
                        failCount += 1;
                    }

                }
                   
            }
            logger.DebugFormat("Object {0} recursive relation {1} updated. Attempted {2} Success {3} Failed {4}",
               apiName, recursiveRelationshipField, updateList.Count, successCount, failCount);

        }

        private void LoginToBoth()
        {
            
            try
            {
                _sourceTasks.LoginIfRequired();
               
            }
            catch (Exception e)
            {
                string newMess = string.Format("Error while logging in to Source: {0}",
                    e.Message);
                throw new ApplicationException(newMess, e);
            }
            
            try
            {
                _targetTasks.LoginIfRequired();
               
            }
            catch (Exception e)
            {
                string newMess = string.Format("Error while logging in to Target: {0}",
                 e.Message);
                throw new ApplicationException(newMess, e);
            }
        }

        public void DeleteTargetData()
        {
            this.DeleteTargetData(null, false);
        }

        public void DeleteTargetData(IProgress<string> progress, bool ignoreFilters)
        {
            _progress = progress;
            ProgressUpdate("Logging into Target ...");
            _targetTasks.LoginIfRequired();
            var reverseObjects = _instructions.SbbObjects.AsEnumerable().Reverse();
            
            foreach (SbbObject objLoop in reverseObjects)
            {
                String filter = objLoop.Filter;
                String filterInfo = null;
                if (filter == null)
                    filterInfo = "No filter specified in instruction file";
                else
                    filterInfo = "Filter in instruction file = " + filter;

                if (ignoreFilters && filter != null)
                {
                    filterInfo = "Filter from instruction file bypassed due to ignoreFilters flag";
                    filter = null;
                }
                ProgressUpdate(string.Format("Starting deletion for {0}, filter: {1}", objLoop.ApiName, filterInfo));
                

                var targetIds = _targetTasks.GetIdsFromSObject(objLoop.ApiName, filter);
                ProgressUpdate(string.Format("Received {0} {1} ids from target", targetIds.Count, objLoop.ApiName));
                int done = 0;
                bool allDone = false;
                if (targetIds.Count == 0)
                    allDone = true;
                while (!allDone)
                {
                    string[] idarray = targetIds.Skip(done).Take(100).ToArray();
                    done += idarray.Length;
                    if (done >= targetIds.Count)
                        allDone = true;
                    _targetTasks.DeleteSObjects(objLoop.ApiName, idarray);
                    
                }
                ProgressUpdate(string.Format("Deleted {0} {1} records from target", targetIds.Count, objLoop.ApiName));

            }
        }

    }
}
