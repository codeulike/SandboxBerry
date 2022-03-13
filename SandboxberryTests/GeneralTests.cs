using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SandboxberryLib;
using SandboxberryLib.InstructionsModel;
using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace SandboxberryTests
{
    [TestClass]
    public class GeneralTests
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(GeneralTests));

        [TestMethod]
        public void CanBuildQuery()
        {
            List<string> pretendCols = new string[] { 
                            "Id",
                            "Name",
                            "Something__c"}.ToList();

            SalesforceTasks tasks = new SalesforceTasks(TestUtils.MakeDummyCredentials());

            var res = tasks.BuildQuery("Madeup__c", pretendCols, null, null);
            Assert.AreEqual(res, "select Id, Name, Something__c from Madeup__c");
        }

        [TestMethod]
        public void CanBuildQueryWithFilter()
        {
            List<string> pretendCols = new string[] { 
                            "Id",
                            "Name",
                            "Something__c"}.ToList();

            SalesforceTasks tasks = new SalesforceTasks(TestUtils.MakeDummyCredentials());

            var res = tasks.BuildQuery("Madeup__c", pretendCols, "Something__c = 'a'", null);
            Assert.AreEqual(res, "select Id, Name, Something__c from Madeup__c where Something__c = 'a'");
        }

        [TestMethod]
        public void CanRemoveSystemCols()
        {
            List<string> pretendCols = new string[] { 
                            "Id",
                            "Name",
                            "Something__c",
                            "IsDeleted",
                            "CreatedDate",
                            "CreatedById",
                            "LastModifiedDate",
                            "LastModifiedById",
                            "SystemModstamp",
                            "LastViewedDate",
                            "LastReferencedDate",
                            "SomethingElse__c"}.ToList();

            SalesforceTasks tasks = new SalesforceTasks(TestUtils.MakeDummyCredentials());

            var res = tasks.RemoveSystemColumns(pretendCols);
            Assert.AreEqual(res.Count, 4);
            Assert.IsTrue(res.Contains("Id"));
            Assert.IsTrue(res.Contains("Name"));
            Assert.IsTrue(res.Contains("Something__c"));
            Assert.IsTrue(res.Contains("SomethingElse__c"));

        }

        [TestMethod]
        public void CanSerializeInstructions()
        {
            string[] tables = {"Country__c","Nationality__c",
                       "Account", "Client__c" };

            SbbInstructionSet inst = TestUtils.MakeInstructionSet(tables.ToList());
            inst.SbbObjects.First(o => o.ApiName == "Account").Filter = "Help_Sandbox_Data_Set__c = true";
            inst.SbbObjects.First(o => o.ApiName == "Client__c").Filter = "account__r.Help_Sandbox_Data_Set__c = true";

            var baseDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var testFile = System.IO.Path.Combine(baseDir, "testinstructions.xml");

            SbbInstructionSet.SaveToFile(testFile, inst);
        }

        [TestMethod]
        public void GetManfiestResourceNames()
        {
            Assembly dll = Assembly.GetAssembly(typeof(SbbInstructionSet));
            var names = dll.GetManifestResourceNames();
            foreach (var n in names)
                logger.DebugFormat("Resource: {0}", n);

        }
    }
}
