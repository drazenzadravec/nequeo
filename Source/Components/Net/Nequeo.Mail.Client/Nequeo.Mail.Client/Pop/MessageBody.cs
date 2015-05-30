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
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace Nequeo.Net.Mail.Pop
{
	/// <summary>
	/// Class that gets the message body data and 
    /// the messasge attachment data.
	/// </summary>
    internal class MessageBody
    {
        #region Constructors
        /// <summary>
        /// Message body and attachment constructor.
        /// </summary>
        /// <param name="lines">The array of lines in the message.</param>
        /// <param name="startOfBody">The start of the body data.</param>
        /// <param name="multipartBoundary">The multipary boundary.</param>
        /// <param name="mainContentType">The main content type.</param>
        /// <param name="writeAttachmentToFile">Write to file.</param>
        /// <param name="connection">The connection object.</param>
        public MessageBody(string[] lines, long startOfBody, string multipartBoundary, 
            string mainContentType, bool writeAttachmentToFile, Pop3ConnectionAdapter connection)
		{
            // Get the number of message lines.
			long stopOfBody = lines.Length;

            // If the multipart boudary is null then
            // only a body part exists.
			if(multipartBoundary == null)
			{
                // Create a new string builder.
				StringBuilder sbText = new StringBuilder();

                // For each line in the message
                // append the body lines.
				for(long i = startOfBody; i < stopOfBody; i++)
					sbText.Append(lines[i].Replace("\n","").Replace("\r",""));

				// Add a new body to the component
                // list store.
				_component.Add(new MessageAttachment(mainContentType, sbText.ToString(),
                    writeAttachmentToFile, connection));
			}
            // Multipart boundary exists then a body and attachment
            // exists in the message.
			else
			{				
                // Get the multipart boundary
                // string in the message.
				string boundary = multipartBoundary;

                // Set as first component or body
                // of the message to get.
				bool firstComponent = true;

				// Loop through whole of email message
                // get the bodt all attachments.
				for(long i = startOfBody; i < stopOfBody;)
				{
					bool boundaryFound = true;
					string contentType = null;
					string name = null;
					string filename = null;
					string contentTransferEncoding= null;
					string contentDescription = null;
					string contentDisposition = null;
					string data = null;

					// If the first component is to be added
                    // this is the body of a multipart message.
					if(firstComponent)
					{
						boundaryFound = false;
						firstComponent = false;

                        // For all the lines in the message.
						while( i < stopOfBody )
						{
                            // Get the current line in the message.
							string line = lines[i].Replace("\n","").Replace("\r","");

                            // If the end of the body boundary
                            // has been found then exit the loop
                            // all body data has been found.
                            if (EmailMessageParse.GetContentMappingHeaderType(line, boundary)
                                == Nequeo.Net.Mail.MesssageContentMappingType.MultipartBoundaryFound)
							{
								boundaryFound = true;
								++i;
								break;
							}
							else ++i;
						}
					}

					// Check to see whether multipart boundary
					// was not found. This will indicate that
                    // the message data is corrupt.
					if(!boundaryFound)
						throw new Exception("Missing multipart boundary: " + boundary);

                    // Initalise the end of header indicator.
					bool endOfHeader = false;

					// Read all header information.
                    // For all other lines in the
                    // messsage.
					while( (i < stopOfBody) )
					{
                        // Get the current line in the message.
						string line = lines[i].Replace("\n","").Replace("\r","");

                        // Get the current content mapping type.
                        Nequeo.Net.Mail.MesssageContentMappingType lineType
                            = EmailMessageParse.GetContentMappingHeaderType(line, boundary);
						
                        // Get the mapping type.
						switch(lineType)
						{
                            case Nequeo.Net.Mail.MesssageContentMappingType.ContentType:
                                // Mapping type is content type header.
                                // Get the content type data.
								contentType = EmailMessageParse.ContentType(line);
								break;

                            case Nequeo.Net.Mail.MesssageContentMappingType.ContentTransferEncoding:
                                // Mapping type is content transfer encoding header.
                                // Get the content transfer encoding data.
								contentTransferEncoding = EmailMessageParse.ContentTransferEncoding(line);
								break;

                            case Nequeo.Net.Mail.MesssageContentMappingType.ContentDisposition:
                                // Mapping type is content disposition header.
                                // Get the content disposition data.
								contentDisposition = EmailMessageParse.ContentDisposition(line);
								break;

                            case Nequeo.Net.Mail.MesssageContentMappingType.ContentDescription:
                                // Mapping type is content description header.
                                // Get the content description data.
								contentDescription = EmailMessageParse.ContentDescription(line);
								break;

                            case Nequeo.Net.Mail.MesssageContentMappingType.EndOfHeader:
                                // End of header has been found.
								endOfHeader = true;
								break;
						}
						
                        // Increment to the next line after
                        // find the content mapping type in
                        // the prevoius line.
						++i;

                        // If end of header found then search
                        // for the next component starting point
                        // search for the next attachment. break
                        // out of the loop and end the process.
						if(endOfHeader)
							break;
						else
						{
                            // Read all header information.
                            // For all other lines in the
                            // messsage.
							while(i < stopOfBody)
							{
								// If more lines to read for this line.
								if(line.Substring(line.Length - 1, 1).Equals(";"))
								{
                                    // Get the current line in the message.
									string nextLine = lines[i].Replace("\r","").Replace("\n","");

                                    // Get the current attachment header type.
                                    // interate through each attachment data.
                                    switch (EmailMessageParse.GetAttachmenHeaderType(nextLine))
									{
                                        case Nequeo.Net.Mail.MesssageAttachmentType.Name:
                                            // Attachment name header type.
                                            // Get the attachment name in the message.
                                            name = EmailMessageParse.Name(nextLine);
											break;

                                        case Nequeo.Net.Mail.MesssageAttachmentType.Filename:
                                            // Attachment file nale header type.
                                            // Get the attachment file name in the message.
                                            filename = EmailMessageParse.Filename(nextLine);
											break;

                                        case Nequeo.Net.Mail.MesssageAttachmentType.EndOfHeader:
                                            // End of header has been found.
											endOfHeader = true;
											break;
									}

                                    // If the end of the attachment 
                                    // data has not been found
									if( !endOfHeader )
									{
										// Assign the current line to
                                        // the next line
										line = nextLine;
										++i;
									}
									else
									{
                                        // End the loop.
										break;
									}
								}
								else
								{
                                    // End the loop.
									break;
								}
							}
						}
					}

                    // Boundary found to false.
					boundaryFound = false;

                    // Create a new string builder.
					StringBuilder sbText = new StringBuilder();

                    // Initailise the emial composed indicator.
					bool emailComposed = false;

					// For each remaining line in
                    // the message store the body
                    // and the attachment.
					while(i < stopOfBody)
					{
                        // Get the current line in the message.
						string line = lines[i].Replace("\n","");

                        // If the end of the body boundary
                        // has been found then exit the loop
                        // all component data has been found.
                        if (EmailMessageParse.GetContentMappingHeaderType(line, boundary)
                            == Nequeo.Net.Mail.MesssageContentMappingType.MultipartBoundaryFound)
						{
							boundaryFound = true;
							++i;
							break;
						}
                        // If the component is done then
                        // the email is composed, this
                        // component has been completed
                        // composed.
						else if (EmailMessageParse.GetContentMappingHeaderType(line, boundary)
                                == Nequeo.Net.Mail.MesssageContentMappingType.ComponentDone)
						{
							emailComposed = true;
							break;
						}
						
						// Add the current line of component
                        // data to the string builder.
                        // Move to the next line in the message.
						sbText.Append(lines[i]);
						++i;
					}

                    // If data has been built
                    // then this is the component data
                    // that is body or attachment data.
					if(sbText.Length > 0)
						data = sbText.ToString();

					// Add a new message body or attachment to the
                    // component list store.
					_component.Add(new MessageAttachment(contentType, name, filename,
						contentTransferEncoding, contentDescription, contentDisposition,
						data, writeAttachmentToFile, connection));

					// If all multiparts have been
					// composed then exit break
                    // out of the loop.
					if(emailComposed)
						break;
				}
			}
        }
        #endregion

        #region Private Fields
        private List<MessageAttachment> _component = new List<MessageAttachment>();
        #endregion

        #region Private Properties
        /// <summary>
        /// Get, the component enumerator.
        /// </summary>
		public IEnumerator ComponentEnumerator
		{
			get { return _component.GetEnumerator(); }
		}

        /// <summary>
        /// Get, the number of components.
        /// </summary>
		public int NumberOfComponents
		{
			get { return _component.Count; }
		}
        #endregion
	}
}
