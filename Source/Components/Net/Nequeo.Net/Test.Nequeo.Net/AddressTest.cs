using Nequeo.Net.IP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;

namespace Test.Nequeo.Net
{
    
    
    /// <summary>
    ///This is a test class for AddressTest and is intended
    ///to contain all AddressTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AddressTest
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
        ///A test for GetHostEntryIPAddress
        ///</summary>
        [TestMethod()]
        public void GetHostEntryIPAddressTest()
        {
            string server = "nequeo.net.au"; 
            IPAddress[] actual;
            actual = Address.GetHostEntryIPAddress(server);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for GetIPHostEntry
        ///</summary>
        [TestMethod()]
        public void GetIPHostEntryTest()
        {
            string server = string.Empty;
            IPHostEntry expected = null;
            IPHostEntry actual;
            actual = Address.GetIPHostEntry(server);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
