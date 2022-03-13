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

using log4net;
using SandboxberryLib.InstructionsModel;
using SandboxberryLib.SalesforcePartnerApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandboxberryLib
{
    public class SalesforceTasks
    {

        private static readonly ILog logger = LogManager.GetLogger(typeof(SalesforceTasks));

        private SalesforceSession _salesforceSession;
        private SforceService _binding = null;

        private Dictionary<string, DescribeSObjectResult> _metaDictionary;

        public SalesforceTasks(SbbCredentials cred)
            : this(new SalesforceSession(cred))
        { }

        public SalesforceTasks(SalesforceSession sesh)
        {
            _salesforceSession = sesh;
            _metaDictionary = new Dictionary<string, DescribeSObjectResult>();
        }
        public void FetchObjectMetadata(string[] apiNameArray)
        {
            LoginIfRequired();
            logger.DebugFormat("Fetching metadata for {0} object types", apiNameArray.Length);
            var metaList = _binding.describeSObjects(apiNameArray);

            foreach (var metaLoop in metaList)
            {
                if (!_metaDictionary.ContainsKey(metaLoop.name))
                    _metaDictionary.Add(metaLoop.name, metaLoop);
            }
        }
        public DescribeSObjectResult GetObjectMeta(string apiName)
        {
            if (!_metaDictionary.ContainsKey(apiName))
            {
                logger.DebugFormat("metadata for {0} needed, sending request to salesforce api...", apiName);
                FetchObjectMetadata(new string[] { apiName });
            }
            if (!_metaDictionary.ContainsKey(apiName))
                throw new ApplicationException(string.Format("Could not return metadata for type {0}", apiName));
            return _metaDictionary[apiName];
        }
        public Dictionary<string, string> GetObjectRelationships(string apiName)
        {
            var res = new Dictionary<string, string>();
            var meta = GetObjectMeta(apiName);
            var fieldsWeUse = RemoveSystemColumns(GetObjectColumns(apiName));

            var relationshipFields = meta.fields.Where(f => f.relationshipName != null);
            foreach (var fieldLoop in relationshipFields)
            {
                if (fieldsWeUse.Contains(fieldLoop.name))
                {
                    if (fieldLoop.referenceTo == null || fieldLoop.referenceTo.Length == 0)
                    {
                        logger.DebugFormat("GetObjectRelationships: {0} {1} does not specify referenceTo type so ignoring",
                            apiName, fieldLoop.name);
                    }
                    else if (fieldLoop.referenceTo.Length > 1)
                    {
                        logger.DebugFormat("GetObjectRelationships: {0} {1} refers to multiple types {2} so ignoring",
                            apiName, fieldLoop.name, string.Join(", ", fieldLoop.referenceTo));
                    }
                    else
                    {
                        var fieldName = fieldLoop.name;
                        var destinationObject = fieldLoop.referenceTo[0];
                        if (destinationObject == "RecordType")
                            logger.DebugFormat("GetObjectRelationships: {0} {1} refers to {2} so ignoring",
                         apiName, fieldLoop.name, destinationObject);
                        else
                            res.Add(fieldName, destinationObject);
                    }
                }
                else
                    logger.DebugFormat("GetObjectRelationships: {0} ignore field {1} because out of scope",
                        apiName, fieldLoop.name);

            }
            logger.DebugFormat("GetObjectRelationships: returning {0} relationships for {1}",
                res.Count, apiName);
            if (logger.IsDebugEnabled)
            {
                foreach (string keyLoop in res.Keys)
                {
                    logger.DebugFormat("GetObjectRelationships: returning {0} {1} relationship to {2}",
                        apiName, keyLoop, res[keyLoop]);
                }
            }
            return res;
        }
        public List<string> GetObjectColumns(string objName)
        {

            var meta = GetObjectMeta(objName);
            List<string> fieldNames = meta.fields.Where(f => f.autoNumber == false && f.calculated == false && f.type != fieldType.address && (f.createable == true || f.name == "Id")).Select(f => f.name).ToList<string>();

            return fieldNames;
        }
        public List<string> RemoveSystemColumns(List<string> colNameList)
        {
            List<string> unwantedCols = new string[] {
                            "IsDeleted",
                            "CreatedDate",
                            "CreatedById",
                            "LastModifiedDate",
                            "LastModifiedById",
                            "SystemModstamp",
                            "LastViewedDate",
                            "LastReferencedDate"}.ToList();
            return colNameList.Except(unwantedCols).ToList();
        }
        public void LoginIfRequired()
        {
            if (_binding == null)
                _binding = _salesforceSession.Login();
        }
        public string BuildQuery(string sobjectName, List<string> colList, string filter, string limit)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select {0} from {1}",
                String.Join(", ", colList), sobjectName);
            if (!string.IsNullOrEmpty(filter))
            {  
              sb.AppendFormat(" where {0}", filter);   
            }
            if (!string.IsNullOrEmpty(limit))
            {
                sb.AppendFormat(" limit {0}", limit);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets data for all columns of an sobject
        /// </summary>
        public List<sObject> GetDataFromSObject(string sobjectName, string filter, string limit)
        {
            List<string> colNames = RemoveSystemColumns(GetObjectColumns(sobjectName));
            string soql = BuildQuery(sobjectName, colNames, filter, limit);
            List<sObject> allResults = GetDataFromSObject(sobjectName, colNames, filter, limit);
            return allResults;
        }

        /// <summary>
        /// Gets data for specified columns of an sobject
        /// </summary>
        public List<sObject> GetDataFromSObject(string sobjectName, List<string> colList, string filter, string limit)
        {
            LoginIfRequired();
            colList = RemoveSystemColumns(colList);
            string soql = BuildQuery(sobjectName, colList, filter, limit);

            bool allResultsReturned = false;
            List<sObject> allResults = new List<sObject>();
            QueryResult qr = _binding.query(soql);
            if (qr.size > 0)
            {
                while (!allResultsReturned)
                {
                    allResults.AddRange(qr.records);
                    if (qr.done)
                        allResultsReturned = true;
                    else
                        qr = _binding.queryMore(qr.queryLocator);
                }
            }

            return allResults;
        }
        public List<string> GetIdsFromSObject(string sobjectName, string filter, string limit)
        {
            LoginIfRequired();

            string soql = BuildQuery(sobjectName, new string[] { "Id" }.ToList(), filter, limit);

            List<string> allResults = FetchQueryDataIdOnly(soql);

            return allResults;
        }

        private List<string> FetchQueryDataIdOnly(string soql)
        {
            bool allResultsReturned = false;
            List<string> allResults = new List<string>();
            QueryResult qr = _binding.query(soql);
            if (qr.size > 0)
            {
                while (!allResultsReturned)
                {
                    allResults.AddRange(qr.records.Select(s => s.Id));
                    if (qr.done)
                        allResultsReturned = true;
                    else
                        qr = _binding.queryMore(qr.queryLocator);
                }
            }
            return allResults;
        }
        public string InsertSObject(string sobjectName, sObject obj)
        {

            LoginIfRequired();
            sObject[] objArray = new sObject[] { obj };
            var saveResults = _binding.create(objArray);

            CheckSaveResults(saveResults, string.Format("Creation of {0}", sobjectName), true);

            if (saveResults.Length != 1)
                throw new ApplicationException(string.Format("Expected one saveresult back for creation of {0} but got {1}",
                    sobjectName, saveResults.Length));

            var rowResult = saveResults[0];


            return rowResult.id;
        }

        public InsertSObjectsResult[] InsertSObjects(string sobjectName, sObject[] objArray)
        {

            LoginIfRequired();
            var res = new InsertSObjectsResult[objArray.Length];
            var saveResults = _binding.create(objArray);

            if (saveResults.Length != objArray.Length)
                throw new ApplicationException(string.Format("Sent {0} objects to create api call but got {1} results back. Numbers should match.",
                    objArray.Length, saveResults.Length));
            for (int i = 0; i < saveResults.Length; i++)
            {
                res[i] = new InsertSObjectsResult();
                if (saveResults[i].success)
                    res[i].NewId = saveResults[i].id;
                else
                    res[i].ErrorMessage = GetSaveResultErrorText(saveResults[i]);

            }

            return res;
        }
        public UpdateSObjectsResult[] UpdateSObjects(string sobjectName, sObject[] objArray)
        {

            LoginIfRequired();
            var res = new UpdateSObjectsResult[objArray.Length];
            var saveResults = _binding.update(objArray);

            if (saveResults.Length != objArray.Length)
                throw new ApplicationException(string.Format("Sent {0} objects to update api call but got {1} results back. Numbers should match.",
                    objArray.Length, saveResults.Length));
            for (int i = 0; i < saveResults.Length; i++)
            {
                res[i] = new UpdateSObjectsResult();
                if (saveResults[i].success)
                    res[i].Success = true;
                else
                    res[i].ErrorMessage = GetSaveResultErrorText(saveResults[i]);

            }

            return res;
        }
        public class InsertSObjectsResult
        {
            public string NewId { get; set; }
            public string ErrorMessage { get; set; }
        }
        public class UpdateSObjectsResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }
        public void DeleteSObjects(string sobjectName, string[] idarray)
        {
            LoginIfRequired();

            if (!_salesforceSession.AllowDeletion())
            {
                throw new ApplicationException("This app will not allow deletion to be run on target database");
            }

            var deleteResults = _binding.delete(idarray);

            int successCount = 0;
            int errorCount = 0;
            List<string> errorMessages = new List<string>();
            foreach (DeleteResult resLoop in deleteResults)
            {
                if (resLoop.success)
                    successCount += 1;
                else
                {
                    errorCount += 1;
                    string errMess = string.Format("delete of {0} {1} failed message {2}",
                        sobjectName, resLoop.id, string.Join(", ", resLoop.errors.Select(e => e.message)));
                    logger.DebugFormat(errMess);
                    errorMessages.Add(errMess);
                }
            }
            logger.DebugFormat("deletion of {0} - {1} success {2} failed",
                sobjectName, successCount, errorCount);
            if (errorCount > 0)
                throw new ApplicationException(string.Format("deletion of {0} - {1} failed {2}",
                    sobjectName, errorCount, string.Join(", ", errorMessages)));

        }
        public List<string> GetInactiveUsers()
        {
            LoginIfRequired();
            string soql = "Select id from user where isActive=False";
            return FetchQueryDataIdOnly(soql);
        }
        public List<string> GetAllUsers()
        {
            LoginIfRequired();
            string soql = "Select id from user";
            return FetchQueryDataIdOnly(soql);
        }
        public string GetCurrentUserId()
        {
            LoginIfRequired();
            return _binding.getUserInfo().userId;
        }
        public bool CheckSaveResults(SaveResult[] saveResults, string contextForLog, bool throwError)
        {
            bool allOK = true;
            List<string> errorSummaries = new List<string>();
            foreach (SaveResult srLoop in saveResults)
            {
                if (!srLoop.success)
                {
                    allOK = false;
                    string saveResultErr = GetSaveResultErrorText(srLoop);

                    string errMess = string.Format("Salesforce Save failed, Context {0}, Errors {1}",
                        contextForLog, saveResultErr);
                    logger.WarnFormat(errMess);
                    errorSummaries.Add(errMess);
                }
            }
            if (!allOK && throwError)
                throw new ApplicationException(string.Join(", ", errorSummaries));
            return allOK;
        }
        private string GetSaveResultErrorText(SaveResult saveResult)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var eloop in saveResult.errors)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.AppendFormat("Error {0} Status Code {1} ", eloop.message, eloop.statusCode.ToString());
                if (eloop.fields != null)
                    sb.AppendFormat("Fields {0}", string.Join(",", eloop.fields));
            }
            return sb.ToString();
        }
    }
}