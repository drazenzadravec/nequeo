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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Net.Security;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace Nequeo.Security
{
    /// <summary>
    /// Service X509 certificate validation selector
    /// </summary>
    public sealed class ServiceX509CertificateValidationSelector
    {
        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="validateValue">The validation value.</param>
        /// <param name="x509CertificateLevel">The validation level</param>
        public ServiceX509CertificateValidationSelector(Object validateValue,
            Nequeo.Security.X509CertificateLevel x509CertificateLevel)
        {
            // Get the validate value.
            if (validateValue == null)
                throw new ArgumentNullException("validateValue");

            _validateValue = validateValue;
            _x509CertificateLevel = x509CertificateLevel;
        }
        #endregion

        #region Private Fields
        private Object _validateValue = null;
        private Nequeo.Security.X509CertificateLevel _x509CertificateLevel = Nequeo.Security.X509CertificateLevel.None;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the validation level for the validation value.
        /// </summary>
        public Nequeo.Security.X509CertificateLevel X509CertificateLevel
        {
            get { return _x509CertificateLevel; }
        }

        /// <summary>
        /// Gets the validation value for the corresponding level.
        /// </summary>
        public Object ValidateValue
        {
            get { return _validateValue; }
        }
        #endregion
    }

    /// <summary>
    /// Class contains methods that validate a service/server certificate.
    /// </summary>
    public class ServiceX509CertificateValidator : X509CertificateValidator
    {
        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="validateValue">The validation value to match.</param>
        /// <param name="validationLevel">The validation enum to match with.</param>
        public ServiceX509CertificateValidator(object validateValue,
            Nequeo.Security.X509CertificateLevel validationLevel)
        {
            // Get the validate value.
            if (validateValue == null)
                throw new ArgumentNullException("Validate value has no reference.",
                    new Exception("A validate value was not supplied."));

            // Assign the validation level
            // and the validate value.
            _validationLevel = validationLevel;
            _validateValue = validateValue;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="selectors">The collection of validation selectors.</param>
        public ServiceX509CertificateValidator(ServiceX509CertificateValidationSelector[] selectors)
        {
            // Get the validate value.
            if (selectors == null)
                throw new ArgumentNullException("Validate selector has no reference.",
                    new Exception("Validate values have not been supplied."));

            // Assign the validation level
            // and the validate value.
            _selectors = selectors;
        }
        #endregion

        #region Private Fields
        private X509Certificate2 _certificate = null;
        private object _validateValue = null;
        private Nequeo.Security.X509CertificateLevel _validationLevel =
            Nequeo.Security.X509CertificateLevel.None;
        private ServiceX509CertificateValidationSelector[] _selectors = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get, the current service certificate.
        /// </summary>
        public X509Certificate2 Certificate
        {
            get { return _certificate; }
        }
        #endregion

        #region Public Override Methods
        /// <summary>
        /// Validates the service/server certificate.
        /// </summary>
        /// <param name="certificate">The current service certificate.</param>
        public override void Validate(X509Certificate2 certificate)
        {
            // Make sure a certificate has been passed.
            if (certificate == null)
                throw new ArgumentNullException("Certificate has no reference.",
                    new Exception("Cerficate was not supplied."));

            // Assign the service certificate.
            _certificate = certificate;

            if (_selectors == null)
            {
                // Validate to the selected level.
                ValidateCertificate(certificate, _validateValue, _validationLevel);
            }
            else
            {
                // For each level validate the value.
                foreach (ServiceX509CertificateValidationSelector item in _selectors)
                    ValidateCertificate(certificate, item.ValidateValue, item.X509CertificateLevel);
            }
        }

        /// <summary>
        /// Validate the certificate.
        /// </summary>
        /// <param name="certificate">The current service certificate.</param>
        /// <param name="validateValue">The validation value to match.</param>
        /// <param name="validationLevel">The validation enum to match with.</param>
        private void ValidateCertificate(X509Certificate2 certificate, object validateValue,
            Nequeo.Security.X509CertificateLevel validationLevel)
        {
            switch (validationLevel)
            {
                case Nequeo.Security.X509CertificateLevel.None:
                    // No validation is done all certificates are passed.
                    break;

                case Nequeo.Security.X509CertificateLevel.IssuerName:
                    // Only a valid certificate is passed.
                    // Check that the certificate issuer matches the configured issuer.
                    if ((string)validateValue != certificate.IssuerName.Name)
                        throw new SecurityTokenValidationException
                          ("Certificate was not issued by a trusted issuer.");
                    break;

                case Nequeo.Security.X509CertificateLevel.Issuer:
                    // Only a valid certificate is passed.
                    // Check that the certificate issuer matches the configured issuer.
                    if ((string)validateValue != certificate.Issuer)
                        throw new SecurityTokenValidationException
                          ("Certificate was not issued by a trusted issuer.");
                    break;

                case Nequeo.Security.X509CertificateLevel.SubjectName:
                    // Only a valid certificate is passed.
                    // Check that the certificate issuer matches the configured issuer.
                    if ((string)validateValue != certificate.SubjectName.Name)
                        throw new SecurityTokenValidationException
                          ("Certificate was not issued by a trusted issuer.");
                    break;

                case Nequeo.Security.X509CertificateLevel.Subject:
                    // Only a valid certificate is passed.
                    // Check that the certificate issuer matches the configured issuer.
                    if ((string)validateValue != certificate.Subject)
                        throw new SecurityTokenValidationException
                          ("Certificate was not issued by a trusted issuer.");
                    break;

                case Nequeo.Security.X509CertificateLevel.Thumbprint:
                    // Only a valid certificate is passed.
                    // Check that the certificate issuer matches the configured issuer.
                    if ((string)validateValue != certificate.Thumbprint)
                        throw new SecurityTokenValidationException
                          ("Certificate was not issued by a trusted issuer.");
                    break;

                case Nequeo.Security.X509CertificateLevel.FriendlyName:
                    // Only a valid certificate is passed.
                    // Check that the certificate issuer matches the configured issuer.
                    if ((string)validateValue != certificate.FriendlyName)
                        throw new SecurityTokenValidationException
                          ("Certificate was not issued by a trusted issuer.");
                    break;

                case Nequeo.Security.X509CertificateLevel.NotAfter:
                    // Only a valid certificate is passed.
                    // Check that the certificate issuer matches the configured issuer.
                    if ((DateTime)validateValue != certificate.NotAfter)
                        throw new SecurityTokenValidationException
                          ("Certificate was not issued by a trusted issuer.");
                    break;

                case Nequeo.Security.X509CertificateLevel.NotBefore:
                    // Only a valid certificate is passed.
                    // Check that the certificate issuer matches the configured issuer.
                    if ((DateTime)validateValue != certificate.NotBefore)
                        throw new SecurityTokenValidationException
                          ("Certificate was not issued by a trusted issuer.");
                    break;

                case Nequeo.Security.X509CertificateLevel.SerialNumber:
                    // Only a valid certificate is passed.
                    // Check that the certificate issuer matches the configured issuer.
                    if ((string)validateValue != certificate.SerialNumber)
                        throw new SecurityTokenValidationException
                          ("Certificate was not issued by a trusted issuer.");
                    break;
            }
        }
        #endregion
    }

    /// <summary>
    /// Class contains the current certificate.
    /// </summary>
    public sealed class X509CertificateInfo
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The policy error.</param>
        public X509CertificateInfo(X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            _certificate = certificate;
            _chain = chain;
            _sslPolicyErrors = sslPolicyErrors;
        }
        #endregion

        #region Private Fields
        private X509Certificate _certificate = null;
        private X509Chain _chain = null;
        private SslPolicyErrors _sslPolicyErrors = SslPolicyErrors.None;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get, the certificate.
        /// </summary>
        public X509Certificate Certificate
        {
            get { return _certificate; }
        }

        /// <summary>
        /// Get, certificate chain.
        /// </summary>
        public X509Chain Chain
        {
            get { return _chain; }
        }

        /// <summary>
        /// Get, policy error.
        /// </summary>
        public SslPolicyErrors SslPolicyErrors
        {
            get { return _sslPolicyErrors; }
        }
        #endregion
    }

    /// <summary>
    /// Class contains the current certificate.
    /// </summary>
    public sealed class X509Certificate2Info
    {
        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The policy error.</param>
        public X509Certificate2Info(X509Certificate2 certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            _certificate = certificate;
            _chain = chain;
            _sslPolicyErrors = sslPolicyErrors;
        }
        #endregion

        #region Private Fields
        private X509Certificate2 _certificate = null;
        private X509Chain _chain = null;
        private SslPolicyErrors _sslPolicyErrors = SslPolicyErrors.None;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get, the certificate.
        /// </summary>
        public X509Certificate2 Certificate
        {
            get { return _certificate; }
        }

        /// <summary>
        /// Get, certificate chain.
        /// </summary>
        public X509Chain Chain
        {
            get { return _chain; }
        }

        /// <summary>
        /// Get, policy error.
        /// </summary>
        public SslPolicyErrors SslPolicyErrors
        {
            get { return _sslPolicyErrors; }
        }
        #endregion
    }

    /// <summary>
    /// Class contains methods to read write x509 certificates
    /// </summary>
    public sealed class X509Certificate2Store
    {
        #region Public Static x509 Certificate Collection
        /// <summary>
        /// Displays a dialog box that contains the properties of an X.509 certificate
        /// and its associated certificate chain using a handle to a parent window.
        /// </summary>
        /// <param name="certificate">The X.509 certificate to display.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        public static void DisplayCertificate(X509Certificate2 certificate)
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
        public static void DisplayCertificate(X509Certificate2 certificate, IntPtr hwndParent)
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
        public static void DisplayCertificate(X509Certificate2Collection certificates)
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
        public static void DisplayCertificate(X509Certificate2Collection certificates, IntPtr hwndParent)
        {
            X509Certificate2UI.SelectFromCollection(certificates, "Certificate Select",
                "Select certificates from the following list to get extension information on that certificate",
                X509SelectionFlag.MultiSelection, hwndParent);
        }

        /// <summary>
        /// Loads a certificate given both it's private and public keys - generally used to 
        /// load keys provided on the OAuth wiki's for verification of implementation correctness.
        /// </summary>
        /// <param name="privateKey">The private key value.</param>
        /// <param name="certificate">The certificate value.</param>
        /// <returns>The certificate if found.</returns>
        public static X509Certificate2 LoadCertificateFromStrings(string privateKey, string certificate)
        {
            var parser = new Nequeo.Cryptography.Parser.AsnKeyParser(Convert.FromBase64String(privateKey));
            RSAParameters parameters = parser.ParseRSAPrivateKey();

            var x509 = new X509Certificate2(Encoding.ASCII.GetBytes(certificate));
            var provider = new RSACryptoServiceProvider();
            provider.ImportParameters(parameters);
            x509.PrivateKey = provider;

            return x509;
        }

        /// <summary>
        /// Get the certificate found for the criteria.
        /// </summary>
        /// <param name="base64Encoded">The certificate raw data.</param>
        /// <returns>The certificate if found.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public static X509Certificate2 LoadCertificateFromBase64String(string base64Encoded)
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
        public static X509Certificate2 LoadCertificateFromByteArray(Byte[] rawData)
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
        public static X509Certificate2 GetCertificate(string certificatePath)
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
        public static X509Certificate2 GetCertificate(string certificatePath, string password)
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
        /// Get the certificate private key for the criteria.
        /// </summary>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <returns>The certificate if found.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public static RSA GetCertificatePrivateKey(string certificatePath)
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
                return (RSA)x509Certificate.PrivateKey;
            }
            catch
            {
                throw new CryptographicException("Unable to open the certificate file.");
            }
        }

        /// <summary>
        /// Get the certificate private key for the criteria.
        /// </summary>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <param name="password">The password used to unlock the certificate.</param>
        /// <returns>The certificate if found.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public static RSA GetCertificatePrivateKey(string certificatePath, string password)
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
                return (RSA)x509Certificate.PrivateKey;
            }
            catch
            {
                throw new CryptographicException("Unable to open the certificate file.");
            }
        }

        /// <summary>
        /// Get the certificate public key for the criteria.
        /// </summary>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <returns>The certificate if found.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public static RSA GetCertificatePublicKey(string certificatePath)
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
                return (RSA)x509Certificate.PublicKey.Key;
            }
            catch
            {
                throw new CryptographicException("Unable to open the certificate file.");
            }
        }

        /// <summary>
        /// Get the certificate public key for the criteria.
        /// </summary>
        /// <param name="certificatePath">The path to a certificate with a private key.</param>
        /// <param name="password">The password used to unlock the certificate.</param>
        /// <returns>The certificate if found.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.Exception"></exception>
        public static RSA GetCertificatePublicKey(string certificatePath, string password)
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
                return (RSA)x509Certificate.PublicKey.Key;
            }
            catch
            {
                throw new CryptographicException("Unable to open the certificate file.");
            }
        }

        /// <summary>
        /// Extract the public key only certificate from the x509 certificate.
        /// </summary>
        /// <param name="x509Certificate">The certificate.</param>
        /// <returns>The public key only certificate.</returns>
        public static X509Certificate ExtractCertificate(X509Certificate2 x509Certificate)
        {
            byte[] raw = x509Certificate.GetRawCertData();
            X509Certificate publicKey = new X509Certificate2(raw);
            return publicKey;
        }

        /// <summary>
        /// Get the first certificate found for the criteria.
        /// </summary>
        /// <param name="storeName">The store name to search in.</param>
        /// <param name="storeLocation">The store location to search in.</param>
        /// <param name="x509FindType">The type of data to find on.</param>
        /// <param name="findValue">The value to find in the certificate.</param>
        /// <param name="validOnly">Search for only valid certificates.</param>
        /// <returns>The first certificate found.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        public static X509Certificate2 GetCertificate(StoreName storeName,
            StoreLocation storeLocation, X509FindType x509FindType,
            object findValue, bool validOnly)
        {
            X509Certificate2 certificate = null;
            X509Store store = null;

            try
            {
                // Create a new instance of the certificate store
                // and open the store in read only mode.
                store = new X509Store(storeName, storeLocation);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                // Get the collection of certificates in the
                // store and filter the collection on the
                // specified criteria.
                X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
                X509Certificate2Collection findCollection = (X509Certificate2Collection)collection.
                    Find(x509FindType, findValue, validOnly);

                // For each certificate found.
                foreach (X509Certificate2 x509 in findCollection)
                {
                    // Assign the first certificate
                    // and break out of the loop.
                    certificate = x509;
                    break;
                }

                // Close the store.
                store.Close();

                // Return the first certificate.
                return certificate;
            }
            catch (CryptographicException ex)
            {
                // Throw an exception if failed.
                throw new Exception(ex.Message);
            }
            finally
            {
                // Make sure that the store is closed.
                if (store != null)
                    store.Close();
            }
        }

        /// <summary>
        /// Get all certificates found for the criteria.
        /// </summary>
        /// <param name="storeName">The store name to search in.</param>
        /// <param name="storeLocation">The store location to search in.</param>
        /// <param name="x509FindType">The type of data to find on.</param>
        /// <param name="findValue">The value to find in the certificate.</param>
        /// <param name="validOnly">Search for only valid certificates.</param>
        /// <returns>All certificates found.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        public static X509Certificate2Collection GetCertificates(StoreName storeName,
            StoreLocation storeLocation, X509FindType x509FindType, object findValue,
            bool validOnly)
        {
            X509Certificate2Collection certificates = null;
            X509Store store = null;

            try
            {
                // Create a new instance of the certificate store
                // and open the store in read only mode.
                store = new X509Store(storeName, storeLocation);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                // Get the collection of certificates in the
                // store and filter the collection on the
                // specified criteria.
                X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
                X509Certificate2Collection findCollection = (X509Certificate2Collection)collection.
                    Find(x509FindType, findValue, validOnly);

                // Create a new certificate collection.
                certificates = new X509Certificate2Collection();

                // For each certificate found.
                foreach (X509Certificate2 x509 in findCollection)
                {
                    // Add each certificate to the collection.
                    certificates.Add(x509);
                }

                // Close the store.
                store.Close();

                // Return the all certificates.
                return certificates;
            }
            catch (CryptographicException ex)
            {
                // Throw an exception if failed.
                throw new Exception(ex.Message);
            }
            finally
            {
                // Make sure that the store is closed.
                if (store != null)
                    store.Close();
            }
        }

        /// <summary>
        /// Get all certificates found for the criteria.
        /// </summary>
        /// <param name="storeName">The store name to search in.</param>
        /// <param name="storeLocation">The store location to search in.</param>
        /// <returns>All certificates found.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        public static X509Certificate2Collection GetCertificates(
            StoreName storeName, StoreLocation storeLocation)
        {
            X509Certificate2Collection certificates = null;
            X509Store store = null;

            try
            {
                // Create a new instance of the certificate store
                // and open the store in read only mode.
                store = new X509Store(storeName, storeLocation);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                // Get the collection of certificates in the store
                // and close the store.
                certificates = (X509Certificate2Collection)store.Certificates;
                store.Close();

                // Return the all certificates.
                return certificates;
            }
            catch (CryptographicException ex)
            {
                // Throw an exception if failed.
                throw new Exception(ex.Message);
            }
            finally
            {
                // Make sure that the store is closed.
                if (store != null)
                    store.Close();
            }
        }

        /// <summary>
        /// Get all certificates found for the criteria.
        /// </summary>
        /// <param name="storeLocation">The store location to search in.</param>
        /// <returns>All certificates found.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        public static X509Certificate2Collection GetCertificates(StoreLocation storeLocation)
        {
            X509Certificate2Collection certificates = null;
            X509Store store = null;

            try
            {
                // Create a new instance of the certificate store
                // and open the store in read only mode.
                store = new X509Store(storeLocation);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                // Get the collection of certificates in the store
                // and close the store.
                certificates = (X509Certificate2Collection)store.Certificates;
                store.Close();

                // Return the all certificates.
                return certificates;
            }
            catch (CryptographicException ex)
            {
                // Throw an exception if failed.
                throw new Exception(ex.Message);
            }
            finally
            {
                // Make sure that the store is closed.
                if (store != null)
                    store.Close();
            }
        }

        /// <summary>
        /// Get all certificates found for the criteria.
        /// </summary>
        /// <param name="storeName">The store name to search in.</param>
        /// <returns>All certificates found.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException"></exception>
        public static X509Certificate2Collection GetCertificates(StoreName storeName)
        {
            X509Certificate2Collection certificates = null;
            X509Store store = null;

            try
            {
                // Create a new instance of the certificate store
                // and open the store in read only mode.
                store = new X509Store(storeName);
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                // Get the collection of certificates in the store
                // and close the store.
                certificates = (X509Certificate2Collection)store.Certificates;
                store.Close();

                // Return the all certificates.
                return certificates;
            }
            catch (CryptographicException ex)
            {
                // Throw an exception if failed.
                throw new Exception(ex.Message);
            }
            finally
            {
                // Make sure that the store is closed.
                if (store != null)
                    store.Close();
            }
        }
        #endregion
    }

    /// <summary>
    /// X509 certificate validation level.
    /// </summary>
    public enum X509CertificateLevel
    {
        /// <summary>
        /// No validation of the x509 certificate.
        /// </summary>
        None = 0,
        /// <summary>
        /// Issuer name of the x509 certificate.
        /// </summary>
        IssuerName = 1,
        /// <summary>
        /// Issuer of the x509 certificate.
        /// </summary>
        Issuer = 2,
        /// <summary>
        /// Subject name of the x509 certificate.
        /// </summary>
        SubjectName = 3,
        /// <summary>
        /// Subject of the x509 certificate.
        /// </summary>
        Subject = 4,
        /// <summary>
        /// Thumbprint of the x509 certificate.
        /// </summary>
        Thumbprint = 5,
        /// <summary>
        /// FriendlyName of the x509 certificate.
        /// </summary>
        FriendlyName = 6,
        /// <summary>
        /// NotAfter of the x509 certificate.
        /// </summary>
        NotAfter = 7,
        /// <summary>
        /// NotBefore of the x509 certificate.
        /// </summary>
        NotBefore = 8,
        /// <summary>
        /// SerialNumber of the x509 certificate.
        /// </summary>
        SerialNumber = 9
    }
}
