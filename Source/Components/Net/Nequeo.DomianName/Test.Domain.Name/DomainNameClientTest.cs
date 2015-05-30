using Nequeo.Net.Dns;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Nequeo.Security;

namespace Test.Domain.Name
{
    
    
    /// <summary>
    ///This is a test class for DomainNameClientTest and is intended
    ///to contain all DomainNameClientTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DomainNameClientTest
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
        ///A test for DomainNameClient Constructor
        ///</summary>
        [TestMethod()]
        public void DomainNameClientConstructorTest()
        {
            DnsConnectionAdapter connection = null; // TODO: Initialize to an appropriate value
            DomainNameClient target = new DomainNameClient(connection);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for DomainNameClient Constructor
        ///</summary>
        [TestMethod()]
        public void DomainNameClientConstructorTest1()
        {
            string dnsServer = string.Empty; // TODO: Initialize to an appropriate value
            DomainNameClient target = new DomainNameClient(dnsServer);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for DomainNameClient Constructor
        ///</summary>
        [TestMethod()]
        public void DomainNameClientConstructorTest2()
        {
            DomainNameClient target = new DomainNameClient();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for BeginGetDomainNameServers
        ///</summary>
        [TestMethod()]
        public void BeginGetDomainNameServersTest()
        {
            DomainNameClient target = new DomainNameClient(); // TODO: Initialize to an appropriate value
            string domain = string.Empty; // TODO: Initialize to an appropriate value
            AsyncCallback callback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            IAsyncResult expected = null; // TODO: Initialize to an appropriate value
            IAsyncResult actual;
            actual = target.BeginGetDomainNameServers(domain, callback, state);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for BeginQuery
        ///</summary>
        [TestMethod()]
        public void BeginQueryTest()
        {
            DomainNameClient target = new DomainNameClient(); // TODO: Initialize to an appropriate value
            string domain = string.Empty; // TODO: Initialize to an appropriate value
            DnsType dnsType = new DnsType(); // TODO: Initialize to an appropriate value
            DnsClass dnsClass = new DnsClass(); // TODO: Initialize to an appropriate value
            AsyncCallback callback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            IAsyncResult expected = null; // TODO: Initialize to an appropriate value
            IAsyncResult actual;
            actual = target.BeginQuery(domain, dnsType, dnsClass, callback, state);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ClientValidation
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Nequeo.Net.DomianName.dll")]
        public void ClientValidationTest()
        {
            DomainNameClient_Accessor target = new DomainNameClient_Accessor(); // TODO: Initialize to an appropriate value
            target.ClientValidation();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Nequeo.Net.DomianName.dll")]
        public void DisposeTest()
        {
            DomainNameClient_Accessor target = new DomainNameClient_Accessor(); // TODO: Initialize to an appropriate value
            bool disposing = false; // TODO: Initialize to an appropriate value
            target.Dispose(disposing);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        public void DisposeTest1()
        {
            DomainNameClient target = new DomainNameClient(); // TODO: Initialize to an appropriate value
            target.Dispose();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for EndGetDomainNameServers
        ///</summary>
        [TestMethod()]
        public void EndGetDomainNameServersTest()
        {
            DomainNameClient target = new DomainNameClient(); // TODO: Initialize to an appropriate value
            IAsyncResult ar = null; // TODO: Initialize to an appropriate value
            DomainNameServer[] expected = null; // TODO: Initialize to an appropriate value
            DomainNameServer[] actual;
            actual = target.EndGetDomainNameServers(ar);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EndQuery
        ///</summary>
        [TestMethod()]
        public void EndQueryTest()
        {
            DomainNameClient target = new DomainNameClient(); // TODO: Initialize to an appropriate value
            IAsyncResult ar = null; // TODO: Initialize to an appropriate value
            Response expected = null; // TODO: Initialize to an appropriate value
            Response actual;
            actual = target.EndQuery(ar);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Finalize
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Nequeo.Net.DomianName.dll")]
        public void FinalizeTest()
        {
            DomainNameClient_Accessor target = new DomainNameClient_Accessor(); // TODO: Initialize to an appropriate value
            target.Finalize();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetDomainNameServers
        ///</summary>
        [TestMethod()]
        public void GetDomainNameServersTest()
        {
            DomainNameClient target = new DomainNameClient(); // TODO: Initialize to an appropriate value
            string domain = string.Empty; // TODO: Initialize to an appropriate value
            DomainNameServer[] expected = null; // TODO: Initialize to an appropriate value
            DomainNameServer[] actual;
            actual = target.GetDomainNameServers(domain);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetHostIPAddresses
        ///</summary>
        [TestMethod()]
        public void GetHostIPAddressesTest()
        {
            string domain = string.Empty; // TODO: Initialize to an appropriate value
            IPAddress[] expected = null; // TODO: Initialize to an appropriate value
            IPAddress[] actual;
            actual = DomainNameClient.GetHostIPAddresses(domain);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetNameServersEx
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Nequeo.Net.DomianName.dll")]
        public void GetNameServersExTest()
        {
            DomainNameClient_Accessor target = new DomainNameClient_Accessor(); // TODO: Initialize to an appropriate value
            string domain = string.Empty; // TODO: Initialize to an appropriate value
            DomainNameServer[] expected = null; // TODO: Initialize to an appropriate value
            DomainNameServer[] actual;
            actual = target.GetNameServersEx(domain);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for OnCertificateValidation
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Nequeo.Net.DomianName.dll")]
        public void OnCertificateValidationTest()
        {
            DomainNameClient_Accessor target = new DomainNameClient_Accessor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            X509Certificate certificate = null; // TODO: Initialize to an appropriate value
            X509Chain chain = null; // TODO: Initialize to an appropriate value
            SslPolicyErrors sslPolicyErrors = new SslPolicyErrors(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.OnCertificateValidation(sender, certificate, chain, sslPolicyErrors);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Query
        ///</summary>
        [TestMethod()]
        public void QueryTest()
        {
            DomainNameClient target = new DomainNameClient("nequeo.net.au");
            string domain = "www.microsoft.com";
            DnsType dnsType = DnsType.A;
            DnsClass dnsClass = DnsClass.IN;
            Response expected = null;
            Response actual;
            actual = target.Query(domain, dnsType, dnsClass);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ReverseDnsLookup
        ///</summary>
        [TestMethod()]
        public void ReverseDnsLookupTest()
        {
            string ipAddress = string.Empty; // TODO: Initialize to an appropriate value
            int timeout = 0; // TODO: Initialize to an appropriate value
            IPHostEntry expected = null; // TODO: Initialize to an appropriate value
            IPHostEntry actual;
            actual = DomainNameClient.ReverseDnsLookup(ipAddress, timeout);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Certificate
        ///</summary>
        [TestMethod()]
        public void CertificateTest()
        {
            DomainNameClient target = new DomainNameClient(); // TODO: Initialize to an appropriate value
            X509Certificate2Info actual;
            actual = target.Certificate;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Connection
        ///</summary>
        [TestMethod()]
        public void ConnectionTest()
        {
            DomainNameClient target = new DomainNameClient(); // TODO: Initialize to an appropriate value
            DnsConnectionAdapter expected = null; // TODO: Initialize to an appropriate value
            DnsConnectionAdapter actual;
            target.Connection = expected;
            actual = target.Connection;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DnsPort
        ///</summary>
        [TestMethod()]
        public void DnsPortTest()
        {
            DomainNameClient target = new DomainNameClient(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.DnsPort = expected;
            actual = target.DnsPort;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DnsServer
        ///</summary>
        [TestMethod()]
        public void DnsServerTest()
        {
            DomainNameClient target = new DomainNameClient(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.DnsServer = expected;
            actual = target.DnsServer;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Domain
        ///</summary>
        [TestMethod()]
        public void DomainTest()
        {
            DomainNameClient target = new DomainNameClient(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.Domain;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ProtocolType
        ///</summary>
        [TestMethod()]
        public void ProtocolTypeTest()
        {
            DomainNameClient target = new DomainNameClient(); // TODO: Initialize to an appropriate value
            ProtocolType expected = new ProtocolType(); // TODO: Initialize to an appropriate value
            ProtocolType actual;
            target.ProtocolType = expected;
            actual = target.ProtocolType;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
