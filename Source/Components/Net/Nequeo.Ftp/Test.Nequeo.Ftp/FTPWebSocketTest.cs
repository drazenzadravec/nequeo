using Nequeo.Net.Ftp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Test.Nequeo.Ftp
{
    
    
    /// <summary>
    ///This is a test class for FTPWebSocketTest and is intended
    ///to contain all FTPWebSocketTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FTPWebSocketTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for DirectoryList
        ///</summary>
        [TestMethod()]
        public void DirectoryListTest()
        {
            FTPWebSocket target = new FTPWebSocket();
            FTPConnectionAdapter ftpAdapter = new FTPConnectionAdapter("ftp.nequeo.net.au", "code", "code8", false, true, true, -1, false);
            Uri directoryListUri = new Uri("ftp://ftp.nequeo.net.au/");
            List<string> fileList = null;
            bool actual;
            actual = target.DirectoryList(ftpAdapter, directoryListUri, out fileList);
        }
    }
}
