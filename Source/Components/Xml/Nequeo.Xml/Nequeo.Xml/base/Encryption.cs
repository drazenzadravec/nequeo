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
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;

namespace Nequeo.Xml
{
    /// <summary>
    /// Xml encryption provider.
    /// </summary>
    public sealed class Encryption
    {
        /// <summary>
        /// Xml encryption provider.
        /// </summary>
        public Encryption() { }

        // The cryptography key.
        private byte[] _internalKey = new byte[] { 
            23, 84, 92, 34, 200, 215, 169, 101, 
            152, 114, 67, 34, 98, 235, 242, 51, 
            30, 79, 67, 114, 147, 159, 161, 251,
            25, 39, 69, 135, 149, 224, 202, 63};

        // The initializations vector.
        private byte[] _internalIV = new byte[] { 
            19, 43, 65, 126, 179, 214, 212, 57,
            18, 41, 71, 127, 159, 224, 212, 59};

        /// <summary>
        /// Gets or sets the encryption key.
        /// </summary>
        public byte[] Key
        {
            get { return _internalKey; }
            set { _internalKey = value; }
        }

        /// <summary>
        /// Gets or sets the encryption vector.
        /// </summary>
        public byte[] Vector
        {
            get { return _internalIV; }
            set { _internalIV = value; }
        }

        /// <summary>
        /// Encrypt the element within the xml document.
        /// </summary>
        /// <param name="document">The xml document containing the element to encrypt.</param>
        /// <param name="elementToEncrypt">The element to encrypt in the xml document.</param>
        /// <param name="keyName">The name to map to keyObject.</param>
        /// <remarks>Encrypts the element using the AES symmetric alogorithm with cipher mode : CBC and padding mode : Zeros.
        /// Includes an internal key and vector.</remarks>
        public void Encrypt(XmlDocument document, string elementToEncrypt, string keyName)
        {
            // Create a new AES provider.
            AesCryptoServiceProvider provider = new AesCryptoServiceProvider();
            provider.Mode = CipherMode.CBC;
            provider.Padding = PaddingMode.Zeros;
            provider.BlockSize = 128;
            provider.KeySize = 256;
            provider.Key = _internalKey;
            provider.IV = _internalIV;

            // Encrypt the document.
            Encrypt(document, elementToEncrypt, provider, keyName);
        }

        /// <summary>
        /// Encrypt the element within the xml document.
        /// </summary>
        /// <param name="document">The xml document containing the element to encrypt.</param>
        /// <param name="elementToEncrypt">The element to encrypt in the xml document.</param>
        /// <param name="algorithm">The symmetric alogorithm used to encrypt the element.</param>
        /// <param name="keyName">The name to map to keyObject.</param>
        public void Encrypt(XmlDocument document, string elementToEncrypt, SymmetricAlgorithm algorithm, string keyName)
        {
            // Check the arguments.   
            if (document == null)
                throw new ArgumentNullException("document");

            if (string.IsNullOrEmpty(elementToEncrypt))
                throw new ArgumentNullException("elementToEncrypt");

            if (string.IsNullOrEmpty(keyName))
                throw new ArgumentNullException("keyName");

            if (document == null)
                throw new ArgumentNullException("algorithm");

            // Find the specified element in the XmlDocument 
            // object and create a new XmlElemnt object.
            XmlElement element = document.GetElementsByTagName(elementToEncrypt)[0] as XmlElement;

            // Throw an XmlException if the element was not found. 
            if (element == null)
            {
                throw new XmlException("The specified element was not found");
            }

            // Create a new instance of the EncryptedXml class  
            // and use it to encrypt the XmlElement with the  
            // symmetric key. 
            EncryptedXml eXml = new EncryptedXml();
            byte[] encryptedElement = eXml.EncryptData(element, algorithm, false);

            // Construct an EncryptedData object and populate 
            // it with the desired encryption information. 
            EncryptedData edElement = new EncryptedData();
            edElement.Type = EncryptedXml.XmlEncElementUrl;

            // Create an EncryptionMethod element so that the  
            // receiver knows which algorithm to use for decryption. 
            // Determine what kind of algorithm is being used and 
            // supply the appropriate URL to the EncryptionMethod element. 
            string encryptionMethod = null;

            if (algorithm is TripleDES)
            {
                encryptionMethod = EncryptedXml.XmlEncTripleDESUrl;
            }
            else if (algorithm is DES)
            {
                encryptionMethod = EncryptedXml.XmlEncDESUrl;
            }
            else if (algorithm is Rijndael)
            {
                switch (algorithm.KeySize)
                {
                    case 128:
                        encryptionMethod = EncryptedXml.XmlEncAES128Url;
                        break;
                    case 192:
                        encryptionMethod = EncryptedXml.XmlEncAES192Url;
                        break;
                    case 256:
                        encryptionMethod = EncryptedXml.XmlEncAES256Url;
                        break;
                }
            }
            else
            {
                // Throw an exception if the transform is not in the previous categories 
                throw new CryptographicException("The specified algorithm is not supported for XML Encryption.");
            }

            // Set the encryption method.
            edElement.EncryptionMethod = new EncryptionMethod(encryptionMethod);

            // Set the KeyInfo element to specify the 
            // name of a key. 
            // Create a new KeyInfo element.
            edElement.KeyInfo = new KeyInfo();

            // Create a new KeyInfoName element.
            KeyInfoName kin = new KeyInfoName();

            // Specify a name for the key.
            kin.Value = keyName;

            // Add the KeyInfoName element.
            edElement.KeyInfo.AddClause(kin);

            // Add the encrypted element data to the  
            // EncryptedData object.
            edElement.CipherData.CipherValue = encryptedElement;

            // Replace the element from the original XmlDocument 
            // object with the EncryptedData element. 
            EncryptedXml.ReplaceElement(element, edElement, false);
        }

        /// <summary>
        /// Decrypts all EncryptedData elements of the XML document that were specified
        /// during initialization of the System.Security.Cryptography.Xml.EncryptedXml class.
        /// </summary>
        /// <param name="document">The xml document containing the element to decrypt.</param>
        /// <param name="keyName">The name to map to keyObject.</param>
        /// <remarks>Decrypts the element using the AES symmetric alogorithm with cipher mode : CBC and padding mode : Zeros.
        /// Includes an internal key and vector.</remarks>
        public void Decrypt(XmlDocument document, string keyName)
        {
            // Create a new AES provider.
            AesCryptoServiceProvider provider = new AesCryptoServiceProvider();
            provider.Mode = CipherMode.CBC;
            provider.Padding = PaddingMode.Zeros;
            provider.BlockSize = 128;
            provider.KeySize = 256;
            provider.Key = _internalKey;
            provider.IV = _internalIV;

            // Decrypt the document.
            Decrypt(document, provider, keyName);
        }

        /// <summary>
        /// Decrypts all EncryptedData elements of the XML document that were specified
        /// during initialization of the System.Security.Cryptography.Xml.EncryptedXml class.
        /// </summary>
        /// <param name="document">The xml document containing the element to decrypt.</param>
        /// <param name="algorithm">The symmetric alogorithm used to decrypt the element.</param>
        /// <param name="keyName">The name to map to keyObject.</param>
        public static void Decrypt(XmlDocument document, SymmetricAlgorithm algorithm, string keyName)
        {
            // Check the arguments.   
            if (document == null)
                throw new ArgumentNullException("document");

            if (string.IsNullOrEmpty(keyName))
                throw new ArgumentNullException("keyName");

            if (document == null)
                throw new ArgumentNullException("algorithm");

            // Create a new EncryptedXml object.
            EncryptedXml exml = new EncryptedXml(document);

            // Add a key-name mapping. 
            // This method can only decrypt documents 
            // that present the specified key name.
            exml.AddKeyNameMapping(keyName, algorithm);

            // Decrypt the element.
            exml.DecryptDocument();
        }

        /// <summary>
        /// Encrypt the element within the xml document.
        /// </summary>
        /// <param name="document">The xml document containing the element to encrypt.</param>
        /// <param name="elementToEncrypt">The element to encrypt.</param>
        /// <param name="certificate">The certificate used to encrypt the element.</param>
        public void Encrypt(XmlDocument document, string elementToEncrypt, X509Certificate2 certificate)
        {
            // Check the arguments.   
            if (document == null)
                throw new ArgumentNullException("document");

            if (string.IsNullOrEmpty(elementToEncrypt))
                throw new ArgumentNullException("elementToEncrypt");

            if (document == null)
                throw new ArgumentNullException("algorithm");

            
            // Find the specified element in the XmlDocument 
            // object and create a new XmlElemnt object. 
            XmlElement element = document.GetElementsByTagName(elementToEncrypt)[0] as XmlElement;

            // Throw an XmlException if the element was not found. 
            if (elementToEncrypt == null)
            {
                throw new XmlException("The specified element was not found");
            }

            // Create a new instance of the EncryptedXml class  
            // and use it to encrypt the XmlElement with the  
            // X.509 Certificate. 
            EncryptedXml eXml = new EncryptedXml();

            // Encrypt the element.
            EncryptedData edElement = eXml.Encrypt(element, certificate);

            // Replace the element from the original XmlDocument 
            // object with the EncryptedData element. 
            EncryptedXml.ReplaceElement(element, edElement, false);
        }

        /// <summary>
        /// Decrypts all EncryptedData elements of the XML document that were specified
        /// during initialization of the System.Security.Cryptography.Xml.EncryptedXml class.
        /// </summary>
        /// <param name="document">The xml document containing the element to decrypt.</param>
        /// <remarks>Use this method along with the certificate store only.</remarks>
        public void Decrypt(XmlDocument document)
        {
            // Check the arguments.   
            if (document == null)
                throw new ArgumentNullException("document");

            // Create a new EncryptedXml object.
            EncryptedXml exml = new EncryptedXml(document);

            // Decrypt the XML document.
            exml.DecryptDocument();
        }
    }
}
