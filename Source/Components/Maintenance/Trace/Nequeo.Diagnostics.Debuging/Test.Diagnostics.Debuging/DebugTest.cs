using Nequeo.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Test.Diagnostics.Debuging
{
    
    
    /// <summary>
    ///This is a test class for DebugTest and is intended
    ///to contain all DebugTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DebugTest
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
        ///A test for Debug Constructor
        ///</summary>
        [TestMethod()]
        public void DebugConstructorTest()
        {
            Debug target = new Debug();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for ConsoleMessage
        ///</summary>
        [TestMethod()]
        public void ConsoleMessageTest()
        {
            string message = "Stuff";
            Debug.ConsoleMessage(message);
            
        }

        /// <summary>
        ///A test for ExceptionHandler
        ///</summary>
        [TestMethod()]
        public void ExceptionHandlerTest()
        {
            Debug target = new Debug(); // TODO: Initialize to an appropriate value
            Exception message = null; // TODO: Initialize to an appropriate value
            target.ExceptionHandler(message);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for MessageHandler
        ///</summary>
        [TestMethod()]
        public void MessageHandlerTest()
        {
            Debug target = new Debug(); // TODO: Initialize to an appropriate value
            string message = string.Empty; // TODO: Initialize to an appropriate value
            target.MessageHandler(message);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for MessageExceptionHandler
        ///</summary>
        [TestMethod()]
        public void MessageExceptionHandlerTest()
        {
            Debug target = new Debug(); // TODO: Initialize to an appropriate value
            Action<Exception> expected = null; // TODO: Initialize to an appropriate value
            Action<Exception> actual;
            target.MessageExceptionHandler = expected;
            actual = target.MessageExceptionHandler;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MessageStringHandler
        ///</summary>
        [TestMethod()]
        public void MessageStringHandlerTest()
        {
            Debug target = new Debug(); // TODO: Initialize to an appropriate value
            Action<string> expected = null; // TODO: Initialize to an appropriate value
            Action<string> actual;
            target.MessageStringHandler = expected;
            actual = target.MessageStringHandler;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
