/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Cryptography.Key;

namespace Nequeo.Cryptography.Openssl
{
    /// <summary>
    /// Certificate utility.
    /// </summary>
    public class Certificate : IDisposable
    {
        /// <summary>
        /// Certificate utility.
        /// </summary>
        public Certificate() { }

        private Key.X509.X509Certificate _certificate = null;
        private Openssl.Subject _subjectDN = null;
        private Key.Crypto.Parameters.RsaPrivateCrtKeyParameters _rsPrivateParam = null;

        /// <summary>
        /// Generate an X509 certificate.
        /// </summary>
        /// <param name="privateKey">The private key of the issuer that is signing this certificate.</param>
        /// <param name="publicKey">The public key of the issuer that is signing this certificate.</param>
        /// <param name="serialNumber">The certificate serial number.</param>
        /// <param name="issuerDN">The issuer.</param>
        /// <param name="subjectDN">The subject.</param>
        /// <param name="notBefore">Not before date.</param>
        /// <param name="notAfter">Not after data.</param>
        /// <param name="publicExponent">The public exponent (e; the public key is now represented as {e, n}).</param>
        /// <param name="strength">The strength of the cipher.</param>
        /// <param name="signatureAlgorithm">The signature algorithm to use.</param>
        /// <returns>The X509 certificate.</returns>
        public X509Certificate2 Generate(RSACryptoServiceProvider privateKey, RSACryptoServiceProvider publicKey, long serialNumber, 
            Openssl.Subject issuerDN, Openssl.Subject subjectDN, DateTime notBefore, DateTime notAfter, long publicExponent = 3, int strength = 4096, 
            Nequeo.Cryptography.Signing.SignatureAlgorithm signatureAlgorithm = Nequeo.Cryptography.Signing.SignatureAlgorithm.SHA512withRSA)
        {
            // Clear the certificate.
            _certificate = null;
            _subjectDN = subjectDN;
            _rsPrivateParam = null;

            // Create the rsa key paramaters from the strength and public exponent.
            Key.Crypto.Generators.RsaKeyPairGenerator keyPair = new Key.Crypto.Generators.RsaKeyPairGenerator();
            Key.Crypto.Parameters.RsaKeyGenerationParameters keyPairParam =
                new Key.Crypto.Parameters.RsaKeyGenerationParameters(
                    Key.Math.BigInteger.ValueOf(publicExponent), new Key.Security.SecureRandom(), strength, 25);

            // Initialise the parameters and generate the public private key pair.
            keyPair.Init(keyPairParam);
            Key.Crypto.AsymmetricCipherKeyPair rsaKeyPair = keyPair.GenerateKeyPair();

            // Assign the rsa parameters.
            RSAParameters rsaParam = new RSAParameters();
            Key.Crypto.Parameters.RsaKeyParameters rsaPublicParam = (Key.Crypto.Parameters.RsaKeyParameters)rsaKeyPair.Public;
            _rsPrivateParam = (Key.Crypto.Parameters.RsaPrivateCrtKeyParameters)rsaKeyPair.Private;

            rsaParam.D = _rsPrivateParam.Exponent.ToByteArrayUnsigned();
            rsaParam.DP = _rsPrivateParam.DP.ToByteArrayUnsigned();
            rsaParam.DQ = _rsPrivateParam.DQ.ToByteArrayUnsigned();
            rsaParam.InverseQ = _rsPrivateParam.QInv.ToByteArrayUnsigned();
            rsaParam.P = _rsPrivateParam.P.ToByteArrayUnsigned();
            rsaParam.Q = _rsPrivateParam.Q.ToByteArrayUnsigned();
            rsaParam.Modulus = _rsPrivateParam.Modulus.ToByteArrayUnsigned();
            rsaParam.Exponent = _rsPrivateParam.PublicExponent.ToByteArrayUnsigned();

            // Create the encyption provider.
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
            rsaProvider.ImportParameters(rsaParam);

            // Create a new certificate generator.
            Key.X509.X509V3CertificateGenerator certGenerator = new Key.X509.X509V3CertificateGenerator();
            certGenerator.Reset();

            // Set the certificate values.
            certGenerator.SetSerialNumber(Key.Math.BigInteger.ValueOf(serialNumber));
            certGenerator.SetIssuerDN(GetNames(issuerDN));
            certGenerator.SetSubjectDN(GetNames(subjectDN));
            certGenerator.SetNotBefore(notBefore);
            certGenerator.SetNotAfter(notAfter);
            certGenerator.SetPublicKey(rsaPublicParam);
            certGenerator.SetSignatureAlgorithm(GetSignatureAlgorithm(signatureAlgorithm));

            // Export the signer private key parameters.
            RSAParameters rsaPrivateKeySignerParam = privateKey.ExportParameters(true);
            Key.Crypto.Parameters.RsaPrivateCrtKeyParameters rsaPrivateKeySigner = 
                new Key.Crypto.Parameters.RsaPrivateCrtKeyParameters(
                    new Key.Math.BigInteger(1, rsaPrivateKeySignerParam.Modulus),
                    new Key.Math.BigInteger(1, rsaPrivateKeySignerParam.Exponent),
                    new Key.Math.BigInteger(1, rsaPrivateKeySignerParam.D),
                    new Key.Math.BigInteger(1, rsaPrivateKeySignerParam.P),
                    new Key.Math.BigInteger(1, rsaPrivateKeySignerParam.Q),
                    new Key.Math.BigInteger(1, rsaPrivateKeySignerParam.DP),
                    new Key.Math.BigInteger(1, rsaPrivateKeySignerParam.DQ),
                    new Key.Math.BigInteger(1, rsaPrivateKeySignerParam.InverseQ)
                );

            // Export the signer public key parameters.
            RSAParameters rsaPublicKeySignerParam = publicKey.ExportParameters(false);
            Key.Crypto.Parameters.RsaKeyParameters rsaPublicKeySigner = 
                new Key.Crypto.Parameters.RsaKeyParameters(
                    false,
                    new Key.Math.BigInteger(1, rsaPublicKeySignerParam.Modulus),
                    new Key.Math.BigInteger(1, rsaPublicKeySignerParam.Exponent)
                );

            // Add the extensions
            certGenerator.AddExtension(
                Key.Asn1.X509.X509Extensions.SubjectKeyIdentifier,
                false,
                new Key.X509.Extension.SubjectKeyIdentifierStructure(rsaPublicParam));

            certGenerator.AddExtension(
                Key.Asn1.X509.X509Extensions.AuthorityKeyIdentifier,
                false,
                new Key.X509.Extension.AuthorityKeyIdentifierStructure(rsaPublicKeySigner));

            // Create the certificate.
            _certificate = certGenerator.Generate(rsaPrivateKeySigner);
            _certificate.CheckValidity(DateTime.UtcNow);
            _certificate.Verify(rsaPublicKeySigner);
            byte[] raw = _certificate.GetEncoded();

            // Return the certificate.
            X509Certificate2 x509Certificate2 = new X509Certificate2(raw);
            x509Certificate2.PrivateKey = rsaProvider;
            x509Certificate2.FriendlyName = subjectDN.CommonName;
            return x509Certificate2;
        }

        /// <summary>
        /// Export the certificate to the stream.
        /// </summary>
        /// <param name="output">The stream to write data to.</param>
        /// <param name="certificate">The certificate to export.</param>
        public void ExportCertificate(Stream output, X509Certificate2 certificate)
        {
            // Write the certificate.
            byte[] data = certificate.Export(X509ContentType.Cert);
            output.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Export the certificate in Pkcs format to the stream (includes the private key);
        /// </summary>
        /// <param name="output">The stream to write data to.</param>
        /// <param name="password">The password used to protect the private key.</param>
        public void ExportCertificatePkcs(Stream output, string password)
        {
            // Create the certificate store.
            var pkscStore = new Key.Pkcs.Pkcs12Store();
            var certEntry = new Key.Pkcs.X509CertificateEntry(_certificate);

            // Save the data to the stream.
            pkscStore.SetCertificateEntry(_subjectDN.CommonName, certEntry);
            pkscStore.SetKeyEntry(_subjectDN.CommonName, new Key.Pkcs.AsymmetricKeyEntry(_rsPrivateParam), new[] { certEntry });
            pkscStore.Save(output, password.ToCharArray(), new Key.Security.SecureRandom());
        }

        /// <summary>
        /// Export the certificate public and private key details.
        /// </summary>
        /// <param name="output">The stream to write the data to.</param>
        /// <param name="certificate">The certificate to export.</param>
        public void ExportCertificatePem(TextWriter output, X509Certificate2 certificate)
        {
            // Convert X509Certificate2 to X509.X509Certificate 
            Key.X509.X509CertificateParser certParser = new Key.X509.X509CertificateParser();
            Key.X509.X509Certificate certBouncy = certParser.ReadCertificate(certificate.RawData);

            // Write the public and private key pem
            Key.OpenSsl.PemWriter pemWriter = new Key.OpenSsl.PemWriter(output);
            pemWriter.WriteObject(certBouncy.GetPublicKey());

            // If the certificate has a private key.
            if (certificate.HasPrivateKey)
            {
                string pemPrivateKeyData = certificate.PrivateKey.ToXmlString(true);
                byte[] pemPrivateKeyList = Encoding.Default.GetBytes(pemPrivateKeyData);
                Key.Utilities.IO.Pem.PemObject pemPrivateKey = new Key.Utilities.IO.Pem.PemObject("RSA PRIVATE KEY", pemPrivateKeyList);
                pemWriter.WriteObject(pemPrivateKey);
            }
        }

        /// <summary>
        /// Displays a dialog box that contains the properties of an X.509 certificate
        /// and its associated certificate chain using a handle to a parent window.
        /// </summary>
        /// <param name="certificate">The X.509 certificate to display.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        public void DisplayCertificate(X509Certificate2 certificate)
        {
            X509Certificate2UI.DisplayCertificate(certificate);
        }

        /// <summary>
        /// Displays a dialog box that contains the properties of an X.509 certificate
        /// and its associated certificate chain using a handle to a parent window.
        /// </summary>
        /// <param name="certificate">The X.509 certificate to display.</param>
        /// <param name="hwndParent">A handle to the parent window to use for the display dialog.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        public void DisplayCertificate(X509Certificate2 certificate, IntPtr hwndParent)
        {
            X509Certificate2UI.DisplayCertificate(certificate, hwndParent);
        }

        /// <summary>
        /// Displays a dialog box that contains the properties of an X.509 certificate
        /// and its associated certificate chain using a handle to a parent window.
        /// </summary>
        /// <param name="certificates">The X.509 certificates to display.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        public void DisplayCertificate(X509Certificate2Collection certificates)
        {
            X509Certificate2UI.SelectFromCollection(certificates, "Certificate Select",
                "Select certificates from the following list to get extension information on that certificate",
                X509SelectionFlag.MultiSelection);
        }

        /// <summary>
        /// Displays a dialog box that contains the properties of an X.509 certificate
        /// and its associated certificate chain using a handle to a parent window.
        /// </summary>
        /// <param name="certificates">The X.509 certificates to display.</param>
        /// <param name="hwndParent">A handle to the parent window to use for the display dialog.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        public void DisplayCertificate(X509Certificate2Collection certificates, IntPtr hwndParent)
        {
            X509Certificate2UI.SelectFromCollection(certificates, "Certificate Select",
                "Select certificates from the following list to get extension information on that certificate",
                X509SelectionFlag.MultiSelection, hwndParent);
        }

        /// <summary>
        /// Get the certificate found for the criteria.
        /// </summary>
        /// <param name="base64Encoded">The certificate raw data.</param>
        /// <returns>The certificate if found.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public X509Certificate2 LoadCertificateFromBase64String(string base64Encoded)
        {
            // Data exists.
            if (String.IsNullOrEmpty(base64Encoded))
                throw new System.ArgumentNullException("Raw certificate data has not been set.",
                    new System.Exception("Valid raw certificate data must be set."));

            // The x509 certificate reference.
            X509Certificate2 x509Certificate = null;

            try
            {
                // Create a new instance of the
                // x509 certificate.
                x509Certificate = new X509Certificate2(Convert.FromBase64String(base64Encoded));
                return x509Certificate;
            }
            catch
            {
                throw new CryptographicException("Unable to open the certificate data.");
            }
        }

        /// <summary>
        /// Get the certificate found for the criteria.
        /// </summary>
        /// <param name="rawData">The certificate raw data.</param>
        /// <returns>The certificate if found.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public X509Certificate2 LoadCertificateFromByteArray(Byte[] rawData)
        {
            // Data exists.
            if (rawData == null)
                throw new System.ArgumentNullException("Raw certificate data has not been set.",
                    new System.Exception("Valid raw certificate data must be set."));

            // The x509 certificate reference.
            X509Certificate2 x509Certificate = null;

            try
            {
                // Create a new instance of the
                // x509 certificate.
                x509Certificate = new X509Certificate2(rawData);
                return x509Certificate;
            }
            catch
            {
                throw new CryptographicException("Unable to open the certificate data.");
            }
        }

        /// <summary>
        /// Get the certificate found for the criteria.
        /// </summary>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <returns>The certificate if found.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public X509Certificate2 LoadCertificate(string certificatePath)
        {
            // A path is specified.
            if (String.IsNullOrEmpty(certificatePath))
                throw new System.ArgumentNullException("A certificate path has not been set.",
                    new System.Exception("A valid certificate path must be set."));

            // The x509 certificate reference.
            X509Certificate2 x509Certificate = null;

            try
            {
                // Create a new instance of the
                // x509 certificate.
                x509Certificate = new X509Certificate2(certificatePath);
                return x509Certificate;
            }
            catch
            {
                throw new CryptographicException("Unable to open the certificate file.");
            }
        }

        /// <summary>
        /// Get the certificate found for the criteria.
        /// </summary>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <param name="password">The password used to unlock the certificate.</param>
        /// <returns>The certificate if found.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public X509Certificate2 LoadCertificate(string certificatePath, string password)
        {
            // A path is specified.
            if (String.IsNullOrEmpty(certificatePath))
                throw new System.ArgumentNullException("A certificate path has not been set.",
                    new System.Exception("A valid certificate path must be set."));

            // The x509 certificate reference.
            X509Certificate2 x509Certificate = null;

            try
            {
                // Create a new instance of the
                // x509 certificate.
                x509Certificate = new X509Certificate2(certificatePath, password);
                return x509Certificate;
            }
            catch
            {
                throw new CryptographicException("Unable to open the certificate file.");
            }
        }

        /// <summary>
        /// Get the DN name.
        /// </summary>
        /// <param name="dn">The current DN.</param>
        /// <returns>The translated DN.</returns>
        private Key.Asn1.X509.X509Name GetNames(Openssl.Subject dn)
        {
            IList ord = new ArrayList();
            ord.Add(Key.Asn1.X509.X509Name.C);
            ord.Add(Key.Asn1.X509.X509Name.ST);
            ord.Add(Key.Asn1.X509.X509Name.L);
            ord.Add(Key.Asn1.X509.X509Name.O);
            ord.Add(Key.Asn1.X509.X509Name.OU);
            ord.Add(Key.Asn1.X509.X509Name.CN);
            ord.Add(Key.Asn1.X509.X509Name.EmailAddress);

            IDictionary attrs = new Hashtable();
            attrs[Key.Asn1.X509.X509Name.C] = dn.CountryName;
            attrs[Key.Asn1.X509.X509Name.ST] = dn.State;
            attrs[Key.Asn1.X509.X509Name.L] = dn.LocationName;
            attrs[Key.Asn1.X509.X509Name.O] = dn.OrganisationName;
            attrs[Key.Asn1.X509.X509Name.OU] = dn.OrganisationUnitName;
            attrs[Key.Asn1.X509.X509Name.CN] = dn.CommonName;
            attrs[Key.Asn1.X509.X509Name.EmailAddress] = dn.EmailAddress;

            return new Key.Asn1.X509.X509Name(ord, attrs);
        }

        /// <summary>
        /// Get the signature alogorithm.
        /// </summary>
        /// <param name="signatureAlgorithm">The RSA signature algorithm.</param>
        /// <returns>The signature.</returns>
        private string GetSignatureAlgorithm(Nequeo.Cryptography.Signing.SignatureAlgorithm signatureAlgorithm = Nequeo.Cryptography.Signing.SignatureAlgorithm.SHA512withRSA)
        {
            string signature = "SHA-1withRSA";

            // Select the signature.
            switch(signatureAlgorithm)
            {
                case Signing.SignatureAlgorithm.MD5withRSA:
                    signature = "MD5WITHRSA";
                    break;

                case Signing.SignatureAlgorithm.SHA1withRSA:
                    signature = "SHA1WITHRSA";
                    break;

                case Signing.SignatureAlgorithm.SHA224withRSA:
                    signature = "SHA224WITHRSA";
                    break;

                case Signing.SignatureAlgorithm.SHA256withRSA:
                    signature = "SHA256WITHRSA";
                    break;

                case Signing.SignatureAlgorithm.SHA384withRSA:
                    signature = "SHA384WITHRSA";
                    break;

                case Signing.SignatureAlgorithm.SHA512withRSA:
                    signature = "SHA512WITHRSA";
                    break;
            }

            // Return the signature
            return signature;
        }

        #region Dispose Object Methods

        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _rsPrivateParam = null;
                _certificate = null;
                _subjectDN = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Certificate()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
