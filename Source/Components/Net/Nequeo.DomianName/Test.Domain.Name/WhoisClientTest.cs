using Nequeo.Net.Dns;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Nequeo.Security;

namespace Test.Domain.Name
{
    
    
    /// <summary>
    ///This is a test class for WhoisClientTest and is intended
    ///to contain all WhoisClientTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WhoisClientTest
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
        ///A test for WhoisClient Constructor
        ///</summary>
        [TestMethod()]
        public void WhoisClientConstructorTest()
        {
            WhoisConnectionAdapter connection = null; // TODO: Initialize to an appropriate value
            WhoisClient target = new WhoisClient(connection);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for WhoisClient Constructor
        ///</summary>
        [TestMethod()]
        public void WhoisClientConstructorTest1()
        {
            string whoisServer = string.Empty; // TODO: Initialize to an appropriate value
            WhoisClient target = new WhoisClient(whoisServer);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for WhoisClient Constructor
        ///</summary>
        [TestMethod()]
        public void WhoisClientConstructorTest2()
        {
            WhoisClient target = new WhoisClient();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for BeginGetWhoisServer
        ///</summary>
        [TestMethod()]
        public void BeginGetWhoisServerTest()
        {
            WhoisClient target = new WhoisClient(); // TODO: Initialize to an appropriate value
            string domain = string.Empty; // TODO: Initialize to an appropriate value
            AsyncCallback callback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            IAsyncResult expected = null; // TODO: Initialize to an appropriate value
            IAsyncResult actual;
            actual = target.BeginGetWhoisServer(domain, callback, state);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for BeginQuery
        ///</summary>
        [TestMethod()]
        public void BeginQueryTest()
        {
            WhoisClient target = new WhoisClient(); // TODO: Initialize to an appropriate value
            string domain = string.Empty; // TODO: Initialize to an appropriate value
            AsyncCallback callback = null; // TODO: Initialize to an appropriate value
            object state = null; // TODO: Initialize to an appropriate value
            IAsyncResult expected = null; // TODO: Initialize to an appropriate value
            IAsyncResult actual;
            actual = target.BeginQuery(domain, callback, state);
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
            WhoisClient_Accessor target = new WhoisClient_Accessor(); // TODO: Initialize to an appropriate value
            target.ClientValidation();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Close
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Nequeo.Net.DomianName.dll")]
        public void CloseTest()
        {
            WhoisClient_Accessor target = new WhoisClient_Accessor(); // TODO: Initialize to an appropriate value
            target.Close();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Nequeo.Net.DomianName.dll")]
        public void DisposeTest()
        {
            WhoisClient_Accessor target = new WhoisClient_Accessor(); // TODO: Initialize to an appropriate value
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
            WhoisClient target = new WhoisClient(); // TODO: Initialize to an appropriate value
            target.Dispose();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for EndGetWhoisServer
        ///</summary>
        [TestMethod()]
        public void EndGetWhoisServerTest()
        {
            WhoisClient target = new WhoisClient(); // TODO: Initialize to an appropriate value
            IAsyncResult ar = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.EndGetWhoisServer(ar);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EndQuery
        ///</summary>
        [TestMethod()]
        public void EndQueryTest()
        {
            WhoisClient target = new WhoisClient(); // TODO: Initialize to an appropriate value
            IAsyncResult ar = null; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
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
            WhoisClient_Accessor target = new WhoisClient_Accessor(); // TODO: Initialize to an appropriate value
            target.Finalize();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetServerResponse
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Nequeo.Net.DomianName.dll")]
        public void GetServerResponseTest()
        {
            WhoisClient_Accessor target = new WhoisClient_Accessor(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetServerResponse();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetSocket
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Nequeo.Net.DomianName.dll")]
        public void GetSocketTest()
        {
            WhoisClient_Accessor target = new WhoisClient_Accessor(); // TODO: Initialize to an appropriate value
            string ipAddress = string.Empty; // TODO: Initialize to an appropriate value
            int port = 0; // TODO: Initialize to an appropriate value
            Nequeo.Net.Dns.ProtocolType protocolType = new Nequeo.Net.Dns.ProtocolType(); // TODO: Initialize to an appropriate value
            Socket expected = null; // TODO: Initialize to an appropriate value
            Socket actual;
            actual = target.GetSocket(ipAddress, port, protocolType, true);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetWhoisServer
        ///</summary>
        [TestMethod()]
        public void GetWhoisServerTest()
        {
            WhoisClient target = new WhoisClient(); // TODO: Initialize to an appropriate value
            string domain = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetWhoisServer(domain);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetWhoisServerEx
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Nequeo.Net.DomianName.dll")]
        public void GetWhoisServerExTest()
        {
            WhoisClient_Accessor target = new WhoisClient_Accessor(); // TODO: Initialize to an appropriate value
            string domain = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetWhoisServerEx(domain);
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
            WhoisClient_Accessor target = new WhoisClient_Accessor(); // TODO: Initialize to an appropriate value
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
        public void QueryIPTest()
        {
            WhoisClient target = new WhoisClient("whois.arin.net");
            string domain = "180.76.5.164";
            string expected = string.Empty;
            string actual;
            actual = target.Query(domain);

            target = new WhoisClient("whois.apnic.net");
            //target = new WhoisClient("whois.ripe.net");
            domain = "180.76.5.164";
            expected = string.Empty;
            actual = target.Query(domain);

            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Query
        ///</summary>
        [TestMethod()]
        public void QueryDomainTest()
        {
            WhoisClient target = new WhoisClient("whois.iana.org");
            string domain = "microsoft.com";
            string expected = string.Empty;
            string actual;
            actual = target.Query(domain);

            target = new WhoisClient("whois.apnic.net");
            //target = new WhoisClient("whois.ripe.net");
            domain = "nequeo.com.au";
            expected = string.Empty;
            actual = target.Query(domain);

            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SendCommand
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Nequeo.Net.DomianName.dll")]
        public void SendCommandTest()
        {
            WhoisClient_Accessor target = new WhoisClient_Accessor(); // TODO: Initialize to an appropriate value
            string data = string.Empty; // TODO: Initialize to an appropriate value
            target.SendCommand(data);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Certificate
        ///</summary>
        [TestMethod()]
        public void CertificateTest()
        {
            WhoisClient target = new WhoisClient(); // TODO: Initialize to an appropriate value
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
            WhoisClient target = new WhoisClient(); // TODO: Initialize to an appropriate value
            WhoisConnectionAdapter expected = null; // TODO: Initialize to an appropriate value
            WhoisConnectionAdapter actual;
            target.Connection = expected;
            actual = target.Connection;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Domain
        ///</summary>
        [TestMethod()]
        public void DomainTest()
        {
            WhoisClient target = new WhoisClient(); // TODO: Initialize to an appropriate value
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
            WhoisClient target = new WhoisClient(); // TODO: Initialize to an appropriate value
            Nequeo.Net.Dns.ProtocolType expected = new Nequeo.Net.Dns.ProtocolType(); // TODO: Initialize to an appropriate value
            Nequeo.Net.Dns.ProtocolType actual;
            target.ProtocolType = expected;
            actual = target.ProtocolType;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for WhoisPort
        ///</summary>
        [TestMethod()]
        public void WhoisPortTest()
        {
            WhoisClient target = new WhoisClient(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.WhoisPort = expected;
            actual = target.WhoisPort;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for WhoisServer
        ///</summary>
        [TestMethod()]
        public void WhoisServerTest()
        {
            WhoisClient target = new WhoisClient(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.WhoisServer = expected;
            actual = target.WhoisServer;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
