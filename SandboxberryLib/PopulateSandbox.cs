﻿// SandboxBerry - tool for copying test data into Salesforce sandboxes
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
            var objectsToReprocess = new List<ObjectTransformer>();

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

                if (transformer.RecursiveRelationshipField != null)
                    logger.DebugFormat("Object {0} has a recurive relation to iteself in field {1}",
                        objLoop.ApiName, transformer.RecursiveRelationshipField);

                // find lookups on this object that can't be populated, to reprocess later
                transformer.LookupsToReprocess = transformer.ObjectRelationships
                    .Where(d => d.Value != objLoop.ApiName)          // where it's not a recursive relationship
                    .Where(d => !processedObjects.Contains(d.Value)) // and the referenced record doesn't exist yet
                    .Where(d => !objLoop.SbbFieldOptions.Any(        // and it's not one of the skipped fields
                                e => e.ApiName.Equals(d.Key)
                                     && e.Skip))
                    // TODO: but is still one of the included object types (e.g. not Contact -> "rh2__PS_Describe__c")
                    .Select(d => new LookupInfo
                    {
                        FieldName = d.Key,
                        ObjectName = objLoop.ApiName,
                        RelatedObjectName = d.Value
                    })
                    .ToList();

                if (transformer.LookupsToReprocess.Count > 0)
                {
                    objectsToReprocess.Add(transformer);
                    var fields = transformer.LookupsToReprocess.Select(lookup => lookup.FieldName);
                    logger.DebugFormat("Object {0} has lookups that will need to be reprocessed: {1}",
                        objLoop.ApiName,
                        String.Join(", ", fields));
                }

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

            // reprocess lookup relationships that can be populated now that the inserts are done
            var reprocessingResults = ReprocessObjects(objectsToReprocess);

            // log summary
            ProgressUpdate("************************************************");
            foreach (var resLoop in res.ObjectResults)
            {
                ProgressUpdate(string.Format("Summary for {0}: Success {1} Fail {2}",
                 resLoop.ApiName, resLoop.SuccessCount, resLoop.FailCount));

            }

            // log reprocssing summary
            foreach (var resLoop in reprocessingResults.ObjectResults)
            {
                ProgressUpdate(string.Format("Reprocessing summary for {0}: Success {1} Fail {2}",
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
                    var updateObject = CreateSobjectWithLookup(apiName, apiName, recursiveRelationshipField,
                        new KeyValuePair<string, string>(wrapLoop.NewId, wrapLoop.RecursiveRelationshipOriginalId));

                    if (updateObject != null)
                    {
                        updateList.Add(updateObject);
                    }
                }
            }

            logger.DebugFormat("{0} rows in Object {1} have recursive relation {2} to update ....",
                updateList.Count(), apiName, recursiveRelationshipField);

            var result = UpdateRecords(apiName, updateList);
        }

        /// <summary>
        /// Reprocesses lookup relationship fields that were missed during the initial import and
        /// populates them, once all the records are loaded into the sandbox. Similar to 
        /// UpdateRecursiveField.
        /// </summary>
        private PopulateSandboxResult ReprocessObjects(List<ObjectTransformer> objectsToReprocess)
        {
            var results = new PopulateSandboxResult();

            foreach (var obj in objectsToReprocess)
            {
                foreach (var field in obj.LookupsToReprocess)
                {
                    ProgressUpdate(string.Format("Reprocessing referenced objects for {0} field {1}",
                        field.ObjectName, field.FieldName));

                    List<sObject> updateList = new List<sObject>();
                    foreach (var idPair in field.IdPairs)
                    {
                        var updateObj = CreateSobjectWithLookup(field.ObjectName,
                            field.RelatedObjectName, field.FieldName, idPair);

                        if (updateObj != null)
                        {
                            updateList.Add(updateObj);
                        }
                    }

                    // switch the IDs with the new ones in the sandbox
                    foreach (sObject rowLoop in updateList)
                    {
                        rowLoop.Id = _relationMapper.RecallNewId(field.ObjectName, rowLoop.Id);
                    }

                    ProgressUpdate(string.Format("Updating {0} {1} records",
                        updateList.Count, field.ObjectName));

                    var result = UpdateRecords(field.ObjectName, updateList);
                    results.ObjectResults.Add(result);
                }
            }

            return results;
        }

        /// <summary>
        /// Creates a new sObject record that only contains the specified lookup relationship field
        /// and replaces IDs for the referenced objects
        /// </summary>
        /// <returns>The newly-constructed sObject with the correct referenced ID, otherwise null</returns>
        private sObject CreateSobjectWithLookup(string objectName, string relatedObjectName,
            string fieldName, KeyValuePair<string, string> idPair)
        {
            var newObject = new sObject
            {
                type = objectName,
                Id = idPair.Key
            };

            XmlDocument dummydoc = new XmlDocument();
            XmlElement recursiveEl = dummydoc.CreateElement(fieldName);
            string replaceValue = _relationMapper.RecallNewId(relatedObjectName, idPair.Value);

            if (replaceValue == null)
            {
                logger.DebugFormat("Object {0} {1} relationship field {2} have value {3} could not translate - will ignore",
                   objectName, idPair.Key, fieldName, idPair.Value);
                return null;
            }
            else
            {
                recursiveEl.InnerText = replaceValue;
                newObject.Any = new XmlElement[] { recursiveEl };
                return newObject;
            }
        }

        /// <summary>
        /// Updates a list of sobject records
        /// </summary>
        private PopulateObjectResult UpdateRecords(string objectName, List<sObject> updateList)
        {
            var result = new PopulateObjectResult();
            result.ApiName = objectName;

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

                var updateRes = _targetTasks.UpdateSObjects(objectName,
                       batch.ToArray());

                for (int i = 0; i < updateRes.Length; i++)
                {
                    if (updateRes[i].Success)
                    {
                        result.SuccessCount += 1;
                    }
                    else
                    {

                        logger.WarnFormat("Error when updating {0} {1} in target: {2}",
                            objectName, batch[i].Id, updateRes[i].ErrorMessage);
                        result.FailCount += 1;
                    }

                }

            }

            logger.DebugFormat("Object {0} updated. Attempted {1} Success {2} Failed {3}",
               objectName, updateList.Count, result.SuccessCount, result.FailCount);

            return result;
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
