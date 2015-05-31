/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Model;
using Nequeo.Extension;
using Nequeo.Cryptography.Key;

namespace Nequeo.Security.Ocsp
{
    /// <summary>
    /// Online certificate status protocol responder.
    /// </summary>
    public sealed partial class Responder
    {
        /// <summary>
        /// Online certificate status protocol responder.
        /// </summary>
        /// <param name="signerPublicKey">The signing public key parameters (The certificate to sign OCSP responses with : can be the Certificate Authority Public Key).</param>
        /// <param name="certificateAuthorityPublicKey">The certificate authority public key parameters (Corresponding to the revocation information in index file).</param>
        /// <param name="certificateAuthorityPrivateKey">The certificate authority private key parameters (The private key to sign OCSP responses with).</param>
        public Responder(RSAParameters signerPublicKey, RSAParameters certificateAuthorityPublicKey, RSAParameters certificateAuthorityPrivateKey)
        {
            _publicKeyCA =
                new Cryptography.Key.Crypto.Parameters.RsaKeyParameters(false,
                    new Cryptography.Key.Math.BigInteger(1, certificateAuthorityPublicKey.Modulus),
                    new Cryptography.Key.Math.BigInteger(1, certificateAuthorityPublicKey.Exponent));

            _publicKeySig =
                new Cryptography.Key.Crypto.Parameters.RsaKeyParameters(false,
                    new Cryptography.Key.Math.BigInteger(1, signerPublicKey.Modulus),
                    new Cryptography.Key.Math.BigInteger(1, signerPublicKey.Exponent));

            _privateKeyCA =
                new Cryptography.Key.Crypto.Parameters.RsaPrivateCrtKeyParameters(
                    new Cryptography.Key.Math.BigInteger(1, certificateAuthorityPrivateKey.Modulus),
                    new Cryptography.Key.Math.BigInteger(1, certificateAuthorityPrivateKey.Exponent),
                    new Cryptography.Key.Math.BigInteger(1, certificateAuthorityPrivateKey.D),
                    new Cryptography.Key.Math.BigInteger(1, certificateAuthorityPrivateKey.P),
                    new Cryptography.Key.Math.BigInteger(1, certificateAuthorityPrivateKey.Q),
                    new Cryptography.Key.Math.BigInteger(1, certificateAuthorityPrivateKey.DP),
                    new Cryptography.Key.Math.BigInteger(1, certificateAuthorityPrivateKey.DQ),
                    new Cryptography.Key.Math.BigInteger(1, certificateAuthorityPrivateKey.InverseQ));
        }

        private Cryptography.Key.Crypto.AsymmetricKeyParameter _publicKeyCA = null;
        private Cryptography.Key.Crypto.AsymmetricKeyParameter _publicKeySig = null;
        private Cryptography.Key.Crypto.AsymmetricKeyParameter _privateKeyCA = null;

        private Cryptography.Key.X509.X509Certificate[] _chain = null;

        /// <summary>
        /// Set the certificate chain hierarchy (certificate authority chain). for the response certificate.
        /// </summary>
        /// <param name="certificates">The list of certificates within the chain.</param>
        public void SetChain(X509Certificate2[] certificates)
        {
            // Create the list.
            List<Cryptography.Key.X509.X509Certificate> certChain = new List<Cryptography.Key.X509.X509Certificate>();

            // For each certificate.
            foreach (X509Certificate2 certificate in certificates)
            {
                // Convert X509Certificate2 to X509.X509Certificate 
                Cryptography.Key.X509.X509CertificateParser certParser = new Cryptography.Key.X509.X509CertificateParser();
                Cryptography.Key.X509.X509Certificate certBouncy = certParser.ReadCertificate(certificate.RawData);

                // Add the certificate.
                certChain.Add(certBouncy);
            }

            // Set the certificate chain list.
            _chain = certChain.ToArray();
        }

        /// <summary>
        /// Get the response data for the certificate.
        /// </summary>
        /// <param name="certificates">The certificates to create the response for.</param>
        /// <param name="responseStatus">The response status.</param>
        /// <returns>The response data.</returns>
        public byte[] GetResponse(CertificateResponse[] certificates, ResponseStatusType responseStatus)
        {
            byte[] response = null;
            Cryptography.Key.Ocsp.OcspResp ocspResponse = null;

            // If the response is successful
            // then create the complete response.
            if (responseStatus == ResponseStatusType.Successful)
            {
                // Only get the first signature.
                bool isFirstCertificate = true;
                string signatureAlogorithm = null;

                // Add the certificate ID and status to the response.
                Cryptography.Key.Ocsp.BasicOcspRespGenerator basicOcspResponseGen = new Cryptography.Key.Ocsp.BasicOcspRespGenerator(_publicKeySig);

                // For each certificate add to the response collection.
                foreach (CertificateResponse certificate in certificates)
                {
                    // Create the correct  certificate status response.
                    Cryptography.Key.Ocsp.CertificateStatus certStatus = Cryptography.Key.Ocsp.CertificateStatus.Good;
                    switch (certificate.CertificateStatus.Status)
                    {
                        case CertificateStatusType.Revoked:
                            // Revoked.
                            certStatus = new Cryptography.Key.Ocsp.RevokedStatus(certificate.CertificateStatus.RevocationDate, (int)certificate.CertificateStatus.RevocationReason);
                            break;
                        case CertificateStatusType.Unknown:
                            // Unknown
                            certStatus = new Cryptography.Key.Ocsp.UnknownStatus();
                            break;
                        default:
                            // Good.
                            certStatus = Cryptography.Key.Ocsp.CertificateStatus.Good;
                            break;
                    }

                    // Convert X509Certificate2 to X509.X509Certificate 
                    Cryptography.Key.X509.X509CertificateParser certParser = new Cryptography.Key.X509.X509CertificateParser();
                    Cryptography.Key.X509.X509Certificate certBouncy = certParser.ReadCertificate(certificate.X509Certificate.RawData);

                    // Create the certificate ID.
                    Cryptography.Key.Ocsp.CertificateID certID =
                        new Cryptography.Key.Ocsp.CertificateID(Cryptography.Key.Ocsp.CertificateID.HashSha1, certBouncy, certBouncy.SerialNumber);
                    basicOcspResponseGen.AddResponse(certID, certStatus);

                    // If the first certificate.
                    if (isFirstCertificate)
                    {
                        // Get the signature algorithm.
                        isFirstCertificate = false;
                        signatureAlogorithm = certBouncy.SigAlgName;
                    }
                }

                // Generate the basic response.
                Cryptography.Key.Ocsp.BasicOcspResp basicOcspResponse = basicOcspResponseGen.Generate(signatureAlogorithm, _privateKeyCA, _chain, DateTime.Now);

                // Create the complete response.
                Cryptography.Key.Ocsp.OCSPRespGenerator ocspResponseGen = new Cryptography.Key.Ocsp.OCSPRespGenerator();
                ocspResponse = ocspResponseGen.Generate((int)responseStatus, basicOcspResponse);
                response = ocspResponse.GetEncoded();
            }
            else
            {
                // Only create a limited response.
                Cryptography.Key.Ocsp.OCSPRespGenerator ocspResponseGen = new Cryptography.Key.Ocsp.OCSPRespGenerator();
                ocspResponse = ocspResponseGen.Generate((int)responseStatus, null);
                response = ocspResponse.GetEncoded();
            }

            // Return the response data.
            return response;
        }

        /// <summary>
        /// Get the request data for the certificates.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>The collection of certificate request data.</returns>
        public CertificateRequest[] GetRequest(byte[] request)
        {
            List<CertificateRequest> certRequests = new List<CertificateRequest>();

            // Load the request into the ocsp handler.
            Cryptography.Key.Ocsp.OcspReq resp = new Cryptography.Key.Ocsp.OcspReq(request);

            // get the list of certificates within the request.
            Cryptography.Key.X509.X509Certificate[] certificates = resp.GetCerts();

            // Get the der identifiers.
            Nequeo.Cryptography.Key.Asn1.DerObjectIdentifier identifier_E = Cryptography.Key.Asn1.X509.X509Name.E;
            Nequeo.Cryptography.Key.Asn1.DerObjectIdentifier identifier_CN = Cryptography.Key.Asn1.X509.X509Name.CN;
            Nequeo.Cryptography.Key.Asn1.DerObjectIdentifier identifier_OU = Cryptography.Key.Asn1.X509.X509Name.OU;
            Nequeo.Cryptography.Key.Asn1.DerObjectIdentifier identifier_O = Cryptography.Key.Asn1.X509.X509Name.O;
            Nequeo.Cryptography.Key.Asn1.DerObjectIdentifier identifier_L = Cryptography.Key.Asn1.X509.X509Name.L;
            Nequeo.Cryptography.Key.Asn1.DerObjectIdentifier identifier_C = Cryptography.Key.Asn1.X509.X509Name.C;

            // Assign the der identifiers.
            IDictionary derIdentifiers = new Dictionary<Nequeo.Cryptography.Key.Asn1.DerObjectIdentifier, string>();
            derIdentifiers.Add(identifier_E, "E");
            derIdentifiers.Add(identifier_CN, "CN");
            derIdentifiers.Add(identifier_OU, "OU");
            derIdentifiers.Add(identifier_O, "O");
            derIdentifiers.Add(identifier_L, "L");
            derIdentifiers.Add(identifier_C, "C");

            // For each certificate in the response.
            foreach (Cryptography.Key.X509.X509Certificate cert in certificates)
            {
                // Assign the certificate request.
                CertificateRequest certRequest = new CertificateRequest();
                certRequest.SerialNumber = cert.SerialNumber.ToByteArrayUnsigned();
                certRequest.NotAfter = cert.NotAfter;
                certRequest.NotBefore = cert.NotBefore;
                certRequest.Subject = cert.SubjectDN.ToString(false, derIdentifiers);
                certRequest.Issuer = cert.IssuerDN.ToString(false, derIdentifiers);
                certRequest.SubjectUniqueID = cert.SubjectUniqueID.GetBytes();
                certRequest.IssuerUniqueID = cert.IssuerUniqueID.GetBytes();
                
                // Add the certificate to the collection.
                certRequests.Add(certRequest);
            }

            // Return the list if certificate requests.
            return certRequests.ToArray();
        }
    }
}
