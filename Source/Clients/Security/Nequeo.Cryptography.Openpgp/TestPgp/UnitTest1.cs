using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestPgp
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Nequeo.Cryptography.Openpgp.Certificate cert = new Nequeo.Cryptography.Openpgp.Certificate();
            Nequeo.Cryptography.Openpgp.PublicKey pubKey = cert.LoadPublicKey(System.IO.File.Open(@"C:\Users\PC\AppData\Roaming\gnupg\pubring.gpg", System.IO.FileMode.Open));
            Nequeo.Cryptography.Openpgp.SecretKey secKey =
                cert.LoadSecretKey(System.IO.File.Open(@"C:\Users\PC\AppData\Roaming\gnupg\secring.gpg", System.IO.FileMode.Open), pubKey.KeyId, "HollyTaylor_TH8");

            var key = pubKey.GetKey();
            var keys = secKey.GetPrivateKey();
        }
    }
}
