using Nequeo.Management.Sys;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Xml.Serialization;

namespace Test.Management
{
    
    
    /// <summary>
    ///This is a test class for InformationTest and is intended
    ///to contain all InformationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class InformationTest
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
        ///A test for Get
        ///</summary>
        [TestMethod()]
        public void GetTest()
        {
            Information target = new Information();
            string host = "localhost";
            Status actual;
            actual = target.Get(host);
            string serl = target.SerialiseResult();

            InformationData infoData = new InformationData();
            // Attempt to serialise the result of the current object.
            Nequeo.Serialisation.GeneralSerialisation serialise = new Nequeo.Serialisation.GeneralSerialisation();
            byte[] data = serialise.Serialise(infoData, typeof(InformationData));
            string ret = Encoding.ASCII.GetString(data);
        }
    }

    /// <summary>
    /// System information
    /// </summary>
    [Serializable]
    public class InformationData
    {
        private String _bios = "Dog";

        /// <summary>
        /// Gets the system BIOS
        /// </summary>
        [XmlElement]
        public String Bios
        {
            get { return _bios; }
            set { _bios = value; }
        }
    }
}
