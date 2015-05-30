using Nequeo.Net.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Nequeo.Net.ActiveDirectory.Model;
using System.Collections.Generic;
using System.DirectoryServices;

namespace Test.ActiveDirectory
{
    
    
    /// <summary>
    ///This is a test class for ApplicationDirectoryClientTest and is intended
    ///to contain all ApplicationDirectoryClientTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ApplicationDirectoryClientTest
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
        ///A test for AuthenticateUser
        ///</summary>
        [TestMethod()]
        public void AuthenticateUserTest()
        {
            ApplicationDirectoryClient target = new ApplicationDirectoryClient();
            string server = "nequeompc";
            string username = "drazen";
            string password = "taylorhollyth8";
            string containerDN = "O=Nequeo,O=com,O=au";
            bool secureConnection = false;
            bool actual;
            actual = target.AuthenticateUser(server, username, password, containerDN, secureConnection);
        }

        /// <summary>
        ///A test for GetAllUserAccountDetails
        ///</summary>
        [TestMethod()]
        public void GetAllUserAccountDetailsTest()
        {
            ApplicationDirectoryClient target = new ApplicationDirectoryClient("Drazen Zadravec", "drazen8");
            string ldapPath = "nequeompc/O=Nequeo,O=com,O=au";
            List<DirectoryEntryModel> actual;
            actual = target.GetAllUserAccountDetails(ldapPath);
        }

        /// <summary>
        ///A test for GetUserMembership
        ///</summary>
        [TestMethod()]
        public void GetUserMembershipTest()
        {
            ApplicationDirectoryClient target = new ApplicationDirectoryClient("Drazen Zadravec", "drazen8");
            string ldapPath = "nequeompc/O=Nequeo,O=com,O=au";
            string username = "drazen";
            List<string> actual;
            actual = target.GetUserMembership(ldapPath, username);
        }

        /// <summary>
        ///A test for GetUserMembershipGroupAccount
        ///</summary>
        [TestMethod()]
        public void GetUserMembershipGroupAccountTest()
        {
            ApplicationDirectoryClient target = new ApplicationDirectoryClient("Drazen Zadravec", "drazen8");
            string ldapPath = "nequeompc/O=Nequeo,O=com,O=au";
            string group = "CN=Users,CN=Roles,O=Nequeo,O=com,O=au";
            List<DirectoryEntry> actual;
            actual = target.GetUserMembershipGroupAccount(ldapPath, group);
        }
    }
}
