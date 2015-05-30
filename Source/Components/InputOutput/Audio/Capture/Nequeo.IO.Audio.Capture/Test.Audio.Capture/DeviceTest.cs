using Nequeo.IO.Audio.Directx;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Test.Audio.Capture
{
    
    
    /// <summary>
    ///This is a test class for DeviceTest and is intended
    ///to contain all DeviceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DeviceTest
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
        ///A test for Device Constructor
        ///</summary>
        [TestMethod()]
        public void DeviceConstructorTest()
        {
            Device target = new Device();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            Device target = new Device(); // TODO: Initialize to an appropriate value
            Device other = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Equals(other);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest1()
        {
            Device target = new Device(); // TODO: Initialize to an appropriate value
            object obj = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Equals(obj);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDevice
        ///</summary>
        [TestMethod()]
        public void GetDeviceTest()
        {
            Device target = new Device(); // TODO: Initialize to an appropriate value
            int index = 0; // TODO: Initialize to an appropriate value
            Microsoft.DirectX.DirectSound.DeviceInformation expected = new Microsoft.DirectX.DirectSound.DeviceInformation(); // TODO: Initialize to an appropriate value
            Microsoft.DirectX.DirectSound.DeviceInformation actual;
            actual = target.GetDevice(index);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetHashCode
        ///</summary>
        [TestMethod()]
        public void GetHashCodeTest()
        {
            Device target = new Device(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.GetHashCode();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetDevice
        ///</summary>
        [TestMethod()]
        public void SetDeviceTest()
        {
            Device target = new Device(); // TODO: Initialize to an appropriate value
            int index = 0; // TODO: Initialize to an appropriate value
            target.SetDevice(index);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            Device target = new Device(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for op_Equality
        ///</summary>
        [TestMethod()]
        public void op_EqualityTest()
        {
            Device device1 = null; // TODO: Initialize to an appropriate value
            Device device2 = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = (device1 == device2);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for op_Inequality
        ///</summary>
        [TestMethod()]
        public void op_InequalityTest()
        {
            Device device1 = null; // TODO: Initialize to an appropriate value
            Device device2 = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = (device1 != device2);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Available
        ///</summary>
        [TestMethod()]
        public void AvailableTest()
        {
            Device target = new Device(); // TODO: Initialize to an appropriate value
            IEnumerable<Device> actual;
            actual = target.Available;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Description
        ///</summary>
        [TestMethod()]
        public void DescriptionTest()
        {
            Device target = new Device(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Description;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Devices
        ///</summary>
        [TestMethod()]
        public void DevicesTest()
        {
            Device target = new Device();
            Microsoft.DirectX.DirectSound.CaptureDevicesCollection actual;
            actual = target.Devices;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DriverGuid
        ///</summary>
        [TestMethod()]
        public void DriverGuidTest()
        {
            Device target = new Device(); // TODO: Initialize to an appropriate value
            Guid actual;
            actual = target.DriverGuid;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ModuleName
        ///</summary>
        [TestMethod()]
        public void ModuleNameTest()
        {
            Device target = new Device(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.ModuleName;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
