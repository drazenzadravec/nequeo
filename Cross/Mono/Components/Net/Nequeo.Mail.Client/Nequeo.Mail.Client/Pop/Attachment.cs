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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Nequeo.Net.Mail.Pop
{
	/// <summary>
	/// Controls the collection of all attachments, contains
    /// methods that get attachment data and decoding of
    /// attachment data.
	/// </summary>
	internal class MessageAttachment
    {
        #region Constructors
        /// <summary>
        /// General message constructor.
        /// </summary>
        /// <param name="contentType">The content type.</param>
        /// <param name="data">The body data.</param>
        /// <param name="writeAttachmentToFile">Write to file.</param>
        /// <param name="connection">The connection object.</param>
        public MessageAttachment(string contentType, string data, 
            bool writeAttachmentToFile, Pop3ConnectionAdapter connection)
        {
            _data = data;
            _connection = connection;
            _contentType = contentType;
            _writeAttachmentToFile = writeAttachmentToFile;
        }

        /// <summary>
        /// Attachment message constructor.
        /// </summary>
        /// <param name="contentType">Content type.</param>
        /// <param name="name">The attachment name.</param>
        /// <param name="filename">The attachment file name.</param>
        /// <param name="contentTransferEncoding">The encoding type.</param>
        /// <param name="contentDescription">The description.</param>
        /// <param name="contentDisposition">The content disposition.</param>
        /// <param name="data">The attachment data.</param>
        /// <param name="writeAttachmentToFile">Write to file.</param>
        /// <param name="connection">The connection object.</param>
        public MessageAttachment(string contentType, string name, string filename,
            string contentTransferEncoding, string contentDescription,
            string contentDisposition, string data, bool writeAttachmentToFile, 
            Pop3ConnectionAdapter connection)
        {
            _data = data;
            _name = name;
            _filename = filename;
            _connection = connection;
            _contentType = contentType;
            _contentDescription = contentDescription;
            _contentDisposition = contentDisposition;
            _writeAttachmentToFile = writeAttachmentToFile;
            _contentTransferEncoding = contentTransferEncoding;
            
            // Decode the attachment data.
            DecodeData(writeAttachmentToFile);
        }
        #endregion

        #region Private Fields
        private string _name = null;
        private string _data = null;
        private Byte[] _file = null;
        private bool _decoded = false;
        private string _filename = null;
        private string _filePath = null;
        private byte[] _binaryData = null;
        private string _contentType = null;
        private string _contentDescription = null;
        private string _contentDisposition = null;
        private bool _writeAttachmentToFile = false;
        private string _contentTransferEncoding = null;
        private Pop3ConnectionAdapter _connection = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get, indicator if the current multipart is the body.
        /// </summary>
        public bool IsBody
        {
            get { return (_contentDisposition == null) ? true : false; }
        }

        /// <summary>
        /// Get, indicator if the current multipart is an attachment.
        /// </summary>
        public bool IsAttachment
        {
            get
            {
                // Initialise the result.
                bool ret = false;

                // If the content disposition string
                // has been set then multipart is an
                // attachment.
                if (_contentDisposition != null)
                    ret = Regex.Match(_contentDisposition,
                        "^attachment.*$").Success;

                // Return the result.
                return ret;
            }
        }

        /// <summary>
        /// Get, the file extension.
        /// </summary>
		public string FileExtension
		{
			get 
			{
                // Initalise the extension string.
				string extension = null;

				// If the filename has an extension
                // then get the extension.
				if((_filename != null) && Regex.Match(_filename,@"^.*\..*$").Success)
					extension = Regex.Replace(_name,@"^[^\.]*\.([^\.]+)$","$1");

				// Return the extension.
				return extension;
			}
		}

        /// <summary>
        /// Get, the file without the extension.
        /// </summary>
		public string FileNoExtension
		{
			get 
			{
                // Initalise the no extension file.
				string extension = null;

                // If the filename has an extension
                // then get the no extension file.
				if((_filename != null) && Regex.Match(_filename,@"^.*\..*$").Success)
					extension = Regex.Replace(_name,@"^([^\.]*)\.[^\.]+$","$1");

                // Return the no extension file.
				return extension;
			}
		}

        /// <summary>
        /// Get, the file path.
        /// </summary>
		public string FilePath
		{
			get { return _filePath; }
		}

        /// <summary>
        /// Get, the file name.
        /// </summary>
		public string Filename
		{
			get { return _filename; }
		}

        /// <summary>
        /// Get, the content type.
        /// </summary>
		public string ContentType
		{
			get { return _contentType; }
		}

        /// <summary>
        /// Get, the file name.
        /// </summary>
		public string Name
		{
			get { return _name; }
		}

        /// <summary>
        /// Get, the mime content transfer encoding type.
        /// </summary>
		public string ContentTransferEncoding
		{
			get { return _contentTransferEncoding; }
		}

        /// <summary>
        /// Get, the decoded mime content transfer indicator.
        /// </summary>
        public bool Decoded
        {
            get { return _decoded; }
        }

        /// <summary>
        /// Get, the content description.
        /// </summary>
		public string ContentDescription
		{
			get { return _contentDescription; }
		}

        /// <summary>
        /// Get, the content disposition.
        /// </summary>
		public string ContentDisposition
		{
			get { return _contentDisposition; }
		}

        /// <summary>
        /// Get, the attachment data.
        /// </summary>
		public string Data
		{
			get { return _data; }
		}

        /// <summary>
        /// Get, the attachment decoded data.
        /// </summary>
        public Byte[] FileData
        {
            get { return _file; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Decodes the attachment data from a mime encoded type.
        /// </summary>
        /// <param name="writeAttachmentToFile">Write the attachment indicator.</param>
        private void DecodeData(bool writeAttachmentToFile)
		{
			// If the data is an attachemnt.
			if( _contentDisposition != null )
			{
				// If the attachment is encoded as BASE-64
                // then decode the data from base 64.
				if((_contentDisposition.Equals("attachment;")) &&
				    (_contentTransferEncoding.ToUpper().Equals("BASE64")))
				{
					// Convert the attachment data to a byte array
                    // from the base 64 encoding. Assign the decoded
                    // data to the byte array of the raw file.
					_binaryData = Convert.FromBase64String(_data.Replace("\n",""));
                    _file = _binaryData;
                    _decoded = true;

                    // If write the attachment set then
                    // directly write the attachment
                    // to the specified file.
                    if (writeAttachmentToFile)
                        EmailMessageParse.WriteAttachment(_binaryData,
                            _connection.AttachmentDirectory, _filename);
				}
                // If the attachment is encoded as QUOTED-PRINTABLE
                // then decode the data from quoted printable.
				else if((_contentDisposition.Equals("attachment;")) &&
					(_contentTransferEncoding.ToUpper().Equals("QUOTED-PRINTABLE")))
				{
                    // Convert the attachment data to a byte array
                    // from the quoted printable encoding. Assign the decoded
                    // data to the byte array of the raw file.
                    string output = EmailMessageParse.FromQuotedPrintable(_data);
                    _file = Encoding.ASCII.GetBytes(output);
                    _decoded = true;

                    // If write the attachment set then
                    // directly write the attachment
                    // to the specified file.
                    if(writeAttachmentToFile)
                        EmailMessageParse.WriteQuotedPrintable(output,
                            _connection.AttachmentDirectory, _filename);
				}
                // If the attachment has been encoded with
                // some other transfer encoding then assign
                // the raw encoded data.
                else if (_contentDisposition.Equals("attachment;"))
                {
                    _file = Encoding.ASCII.GetBytes(_data);
                    _decoded = false;
                }
			}
        }
        #endregion

        #region Override Public Methods
        /// <summary>
        /// Override the ToString method.
        /// </summary>
        /// <returns>The header of the email message attachment only.</returns>
        public override string ToString()
        {
            // The override string includes
            // the header of the message attachment only.
            return
                "Content-Type: " + _contentType + "\r\n" +
                "Name: " + _name + "\r\n" +
                "Filename: " + _filename + "\r\n" +
                "Content-Transfer-Encoding: " + _contentTransferEncoding + "\r\n" +
                "Content-Description: " + _contentDescription + "\r\n" +
                "Content-Disposition: " + _contentDisposition;
        }
        #endregion
    }
}
