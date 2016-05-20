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

namespace Nequeo.Cryptography
{
    /// <summary>
    /// Simple encryption argument class containing event handler
    /// information for the simple encryption delegate.
    /// </summary>
    public class SimpleEncryptionArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the simple encryption event argument.
        /// </summary>
        /// <param name="exceptionMessage">The error message within the simple encryption.</param>
        /// <param name="dataToEncrypt">The original data that was to be encrypted.</param>
        public SimpleEncryptionArgs(string exceptionMessage, string dataToEncrypt)
        {
            this.exceptionMessage = exceptionMessage;
            this.dataToEncrypt = dataToEncrypt;
        }
        #endregion

        #region Private Fields
        // The error message within the simple encryption.
        private string exceptionMessage = string.Empty;
        // The original data that was to be encrypted.
        private string dataToEncrypt = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the error message within the simple encryption.
        /// </summary>
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
        }

        /// <summary>
        /// Contains the original data that was to be encrypted.
        /// </summary>
        public string DataToEncrypt
        {
            get { return dataToEncrypt; }
        }
        #endregion
    }

    /// <summary>
    /// Simple decryption argument class containing event handler
    /// information for the simple decryption delegate.
    /// </summary>
    public class SimpleDecryptionArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the simple decryption event argument.
        /// </summary>
        /// <param name="exceptionMessage">The error message within the simple decryption.</param>
        /// <param name="dataToDecrypt">The original data that was to be decrypted.</param>
        public SimpleDecryptionArgs(string exceptionMessage, string dataToDecrypt)
        {
            this.exceptionMessage = exceptionMessage;
            this.dataToDecrypt = dataToDecrypt;
        }
        #endregion

        #region Private Fields
        // The error message within the simple decryption.
        private string exceptionMessage = string.Empty;
        // The original data that was to be decrypted.
        private string dataToDecrypt = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the error message within the simple decryption.
        /// </summary>
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
        }

        /// <summary>
        /// Contains the original data that was to be decrypted.
        /// </summary>
        public string DataToDecrypt
        {
            get { return dataToDecrypt; }
        }
        #endregion
    }

    /// <summary>
    /// Advanced TDES encryption argument class containing event handler
    /// information for the advanced TDES encryption delegate.
    /// </summary>
    public class AdvancedTDESEncryptionArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the advanced TDES encryption event argument.
        /// </summary>
        /// <param name="exceptionMessage">The error message within the advanced TDES encryption.</param>
        /// <param name="dataToEncrypt">The original data that was to be encrypted.</param>
        public AdvancedTDESEncryptionArgs(string exceptionMessage, byte[] dataToEncrypt)
        {
            this.exceptionMessage = exceptionMessage;
            this.dataToEncrypt = dataToEncrypt;
        }

        /// <summary>
        /// Constructor for the advanced TDES encryption event argument.
        /// </summary>
        /// <param name="exceptionMessage">The error message within the advanced TDES encryption.</param>
        /// <param name="fileToEncrypt">The file to encrypt to.</param>
        /// <param name="fileToDecrypt">The file to decrypt from.</param>
        public AdvancedTDESEncryptionArgs(string exceptionMessage, string fileToEncrypt, string fileToDecrypt)
        {
            this.exceptionMessage = exceptionMessage;
            this.fileToEncrypt = fileToEncrypt;
            this.fileToDecrypt = fileToDecrypt;
        }
        #endregion

        #region Private Fields
        // The error message within the advanced TDES encryption.
        private string exceptionMessage = string.Empty;
        // The original data that was to be encrypted.
        private byte[] dataToEncrypt = null;
        // The original file to encrypt.
        private string fileToEncrypt = string.Empty;
        // The original file to decrypt.
        private string fileToDecrypt = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the error message within the advanced TDES encryption.
        /// </summary>
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
        }

        /// <summary>
        /// Contains the original data that was to be encrypted.
        /// </summary>
        public byte[] DataToEncrypt
        {
            get { return dataToEncrypt; }
        }

        /// <summary>
        /// Contains the original file to encrypt.
        /// </summary>
        public string FileToEncrypt
        {
            get { return fileToEncrypt; }
        }

        /// <summary>
        /// Contains the original file to decrypt.
        /// </summary>
        public string FileToDecrypt
        {
            get { return fileToDecrypt; }
        }
        #endregion
    }

    /// <summary>
    /// Advanced TDES decryption argument class containing event handler
    /// information for the advanced TDES decryption delegate.
    /// </summary>
    public class AdvancedTDESDecryptionArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the advanced TDES decryption event argument.
        /// </summary>
        /// <param name="exceptionMessage">The error message within the advanced TDES decryption.</param>
        /// <param name="dataToDecrypt">The original data that was to be decrypted.</param>
        public AdvancedTDESDecryptionArgs(string exceptionMessage, string dataToDecrypt)
        {
            this.exceptionMessage = exceptionMessage;
            this.dataToDecrypt = dataToDecrypt;
        }

        /// <summary>
        /// Constructor for the advanced TDES decryption event argument.
        /// </summary>
        /// <param name="exceptionMessage">The error message within the advanced TDES decryption.</param>
        /// <param name="fileToEncrypt">The file to encrypt to.</param>
        /// <param name="fileToDecrypt">The file to decrypt from.</param>
        public AdvancedTDESDecryptionArgs(string exceptionMessage, string fileToEncrypt, string fileToDecrypt)
        {
            this.exceptionMessage = exceptionMessage;
            this.fileToEncrypt = fileToEncrypt;
            this.fileToDecrypt = fileToDecrypt;
        }
        #endregion

        #region Private Fields
        // The error message within the advanced TDES decryption.
        private string exceptionMessage = string.Empty;
        // The original data that was to be decrypted.
        private string dataToDecrypt = string.Empty;
        // The original file to encrypt.
        private string fileToEncrypt = string.Empty;
        // The original file to decrypt.
        private string fileToDecrypt = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the error message within the advanced TDES decryption.
        /// </summary>
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
        }

        /// <summary>
        /// Contains the original data that was to be decrypted.
        /// </summary>
        public string DataToDecrypt
        {
            get { return dataToDecrypt; }
        }

        /// <summary>
        /// Contains the original file to encrypt.
        /// </summary>
        public string FileToEncrypt
        {
            get { return fileToEncrypt; }
        }

        /// <summary>
        /// Contains the original file to decrypt.
        /// </summary>
        public string FileToDecrypt
        {
            get { return fileToDecrypt; }
        }
        #endregion
    }

    /// <summary>
    /// Advanced AES encryption argument class containing event handler
    /// information for the advanced AES encryption delegate.
    /// </summary>
    public class AdvancedAESEncryptionArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the advanced TDES encryption event argument.
        /// </summary>
        /// <param name="exceptionMessage">The error message within the advanced TDES encryption.</param>
        /// <param name="dataToEncrypt">The original data that was to be encrypted.</param>
        public AdvancedAESEncryptionArgs(string exceptionMessage, byte[] dataToEncrypt)
        {
            this.exceptionMessage = exceptionMessage;
            this.dataToEncrypt = dataToEncrypt;
        }

        /// <summary>
        /// Constructor for the advanced TDES encryption event argument.
        /// </summary>
        /// <param name="exceptionMessage">The error message within the advanced TDES encryption.</param>
        /// <param name="fileToEncrypt">The file to encrypt to.</param>
        /// <param name="fileToDecrypt">The file to decrypt from.</param>
        public AdvancedAESEncryptionArgs(string exceptionMessage, string fileToEncrypt, string fileToDecrypt)
        {
            this.exceptionMessage = exceptionMessage;
            this.fileToEncrypt = fileToEncrypt;
            this.fileToDecrypt = fileToDecrypt;
        }
        #endregion

        #region Private Fields
        // The error message within the advanced TDES encryption.
        private string exceptionMessage = string.Empty;
        // The original data that was to be encrypted.
        private byte[] dataToEncrypt = null;
        // The original file to encrypt.
        private string fileToEncrypt = string.Empty;
        // The original file to decrypt.
        private string fileToDecrypt = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the error message within the advanced TDES encryption.
        /// </summary>
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
        }

        /// <summary>
        /// Contains the original data that was to be encrypted.
        /// </summary>
        public byte[] DataToEncrypt
        {
            get { return dataToEncrypt; }
        }

        /// <summary>
        /// Contains the original file to encrypt.
        /// </summary>
        public string FileToEncrypt
        {
            get { return fileToEncrypt; }
        }

        /// <summary>
        /// Contains the original file to decrypt.
        /// </summary>
        public string FileToDecrypt
        {
            get { return fileToDecrypt; }
        }
        #endregion
    }

    /// <summary>
    /// Advanced AES decryption argument class containing event handler
    /// information for the advanced AES decryption delegate.
    /// </summary>
    public class AdvancedAESDecryptionArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Constructor for the advanced AES decryption event argument.
        /// </summary>
        /// <param name="exceptionMessage">The error message within the advanced TDES decryption.</param>
        /// <param name="dataToDecrypt">The original data that was to be decrypted.</param>
        public AdvancedAESDecryptionArgs(string exceptionMessage, string dataToDecrypt)
        {
            this.exceptionMessage = exceptionMessage;
            this.dataToDecrypt = dataToDecrypt;
        }

        /// <summary>
        /// Constructor for the advanced AES decryption event argument.
        /// </summary>
        /// <param name="exceptionMessage">The error message within the advanced TDES decryption.</param>
        /// <param name="fileToEncrypt">The file to encrypt to.</param>
        /// <param name="fileToDecrypt">The file to decrypt from.</param>
        public AdvancedAESDecryptionArgs(string exceptionMessage, string fileToEncrypt, string fileToDecrypt)
        {
            this.exceptionMessage = exceptionMessage;
            this.fileToEncrypt = fileToEncrypt;
            this.fileToDecrypt = fileToDecrypt;
        }
        #endregion

        #region Private Fields
        // The error message within the advanced TDES decryption.
        private string exceptionMessage = string.Empty;
        // The original data that was to be decrypted.
        private string dataToDecrypt = string.Empty;
        // The original file to encrypt.
        private string fileToEncrypt = string.Empty;
        // The original file to decrypt.
        private string fileToDecrypt = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Contains the error message within the advanced TDES decryption.
        /// </summary>
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
        }

        /// <summary>
        /// Contains the original data that was to be decrypted.
        /// </summary>
        public string DataToDecrypt
        {
            get { return dataToDecrypt; }
        }

        /// <summary>
        /// Contains the original file to encrypt.
        /// </summary>
        public string FileToEncrypt
        {
            get { return fileToEncrypt; }
        }

        /// <summary>
        /// Contains the original file to decrypt.
        /// </summary>
        public string FileToDecrypt
        {
            get { return fileToDecrypt; }
        }
        #endregion
    }
}
