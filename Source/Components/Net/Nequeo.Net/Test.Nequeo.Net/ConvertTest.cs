using Nequeo.Net.IP;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using NequeoNetIP = Nequeo.Net.IP;

namespace Test.Nequeo.Net
{
    
    
    /// <summary>
    ///This is a test class for ConvertTest and is intended
    ///to contain all ConvertTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConvertTest
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
        ///A test for IPv4ToIPv6
        ///</summary>
        [TestMethod()]
        public void IPv4ToIPv6Test()
        {
            string ipv4Address = "115.70.58.27";
            string ipv6Version = string.Empty; 
            string expected = string.Empty; 
            string actual;
            actual = NequeoNetIP.Convert.IPv4ToIPv6(ipv4Address, ipv6Version);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
