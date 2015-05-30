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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Web;

using Nequeo.Model;
using Nequeo.Extension;

namespace Nequeo.Net.Http
{
    /// <summary>
    /// Http utility provider
    /// </summary>
	public class Utility
	{
        /// <summary>
        /// Get the status code for the code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="subcode">The subcode.</param>
        /// <returns>Status code details; else null if the code has not been found.</returns>
        public static Nequeo.Net.Http.Common.HttpStatusCode GetStatusCode(int code, int subcode = 0)
        {
            try
            {
                // Invoke the member.
                Nequeo.Net.Http.Common.HttpCodes httpCodes = new Nequeo.Net.Http.Common.HttpCodes();
                object member = httpCodes.GetType().InvokeMember(
                    "Code_" + code + "_" + subcode, BindingFlags.InvokeMethod, null, httpCodes, null);

                // Assign the code, then return.
                Nequeo.Net.Http.Common.HttpStatusCode statusCode = (Nequeo.Net.Http.Common.HttpStatusCode)member;
                return statusCode;
            }
            catch { return null; }
        }

        /// <summary>
        /// Transfer the stream data from the source to the destination.
        /// </summary>
        /// <param name="source">The source stream to read from.</param>
        /// <param name="destination">The destination stream to write to.</param>
        public static void TransferData(System.IO.Stream source, System.IO.Stream destination)
        {
            Nequeo.IO.Stream.Operation.CopyStream(source, destination);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="numberToRead">The total number of bytes to read.</param>
        public static void TransferData(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, long offsetStart, long numberToRead)
        {
            Nequeo.IO.Stream.Operation.CopyStreamEx(reader, writer, offsetStart, numberToRead);
        }

        /// <summary>
        /// Copy one stream to another.
        /// </summary>
        /// <param name="reader">The source stream to copy from (read).</param>
        /// <param name="writer">The destination stream to copy to (write).</param>
        /// <param name="byteLength">The total number of bytes that need to be read 
        /// (must be the same as the number of reader bytes). Waits until all bytes are read.</param>
        /// <param name="offsetStart">The number of bytes to skip from the top of the stream (number of bytes not to read from the top).</param>
        /// <param name="offsetEnd">The number of bytes to skip from the end of the stream (number of bytes not to read from the bottom).</param>
        public static void TransferData(System.IO.BinaryReader reader, System.IO.BinaryWriter writer, long byteLength, long offsetStart, long offsetEnd)
        {
            Nequeo.IO.Stream.Operation.CopyStream(reader, writer, byteLength, offsetStart, offsetEnd);
        }

        /// <summary>
        /// Create the directory on the server.
        /// </summary>
        /// <param name="directory">The directory to create.</param>
        public static void CreateDirectory(string directory)
        {
            // If the directory does not exist then create it.
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(directory)))
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(directory));
        }

        /// <summary>
        /// Parse the post back form data into a name value collection.
        /// </summary>
        /// <param name="content">The collection of lines containing the form data.</param>
        /// <returns>The name value collection containing the form data.</returns>
        public static NameValueCollection FormParser(IEnumerable<string> content)
        {
            NameValueCollection formData = new NameValueCollection();

            try
            {
                int countLines = 0;
                int countNameValues = 0;
                bool foundNameValue = false;
                bool startValueFound = false;

                List<string> contentLines = new List<string>();
                List<string> Names = new List<string>();
                List<string> Values = new List<string>();
                StringBuilder valueBuilder = new StringBuilder();

                // The first three lines are content upload data;
                // the last line is the end of the content.
                // Read all the lines in the file.
                foreach (string line in content)
                {
                    // Increment the count.
                    countLines++;

                    // Only read the first four lines; this this the upload content information.
                    switch (countLines)
                    {
                        case 1:
                            // The start indicator line.
                            contentLines.Add(line);
                            break;

                        default:
                            // Is file upload Content-Disposition:
                            if ((line.Length >= 25) && (line.Substring(0, 24).ToLower().Contains("content-disposition")))
                            {
                                // Get the name of the file in the content disposition line; and the save path.
                                string[] splitContent = line.Split(new char[] { ';' });
                                string[] splitName = splitContent[splitContent.Length - 1].Split(new char[] { '=' });

                                // If this content-disposition contains the name 'name'
                                // then attempt to extract the name of the form data.
                                if (splitName[0].ToLower().Trim() == "name".ToLower())
                                {
                                    valueBuilder.Clear();
                                    foundNameValue = true;
                                    countNameValues++;

                                    // Add the name value.
                                    Names.Add(splitName[1].Replace("\"", ""));
                                }
                            }

                            // If a Name has been found.
                            if (foundNameValue)
                            {
                                // If an empty line has been found after a
                                // content-disposition line with a ' filename'
                                // then this is the starting point of the file data.
                                if (line.Length <= 0)
                                    startValueFound = true;

                                // If the starting point of the data has been found.
                                if (startValueFound)
                                {
                                    // If the end of the value section has been found.
                                    if (line.ToLower().Contains(contentLines[0].ToLower()))
                                    {
                                        foundNameValue = false;
                                        startValueFound = false;

                                        // Add the value for the name.
                                        Values.Add(valueBuilder.ToString().Trim());
                                    }
                                    else
                                    {
                                        // Get the current line data value.
                                        valueBuilder.Append(line + "\r\n");
                                    }
                                }
                            }
                            break;
                    }
                }

                // For each name found in the content file
                // write each name value to the collction.
                for (int i = 0; i < Names.Count; i++)
                {
                    // Add the form name and value data.
                    formData.Add(Names[i], Values[i]);
                }
            }
            catch (Exception)
            {
                throw;
            }

            // Return the form data.
            return formData;
        }

        /// <summary>
        /// Parse the upload file content data and search for all the file names to upload.
        /// </summary>
        /// <param name="uploadedFilePath">The original content upload file.</param>
        /// <returns>The collection of file names to upload.</returns>
        public static string[] ParseUploadedFileNames(string uploadedFilePath)
        {
            IEnumerable<string> content = System.IO.File.ReadLines(uploadedFilePath);
            return ParseUploadedFileNames(content);
        }

        /// <summary>
        /// Parse the upload file content data and search for all the file names to upload.
        /// </summary>
        /// <param name="content">The collection of lines containing the form data.</param>
        /// <returns>The collection of file names to upload.</returns>
        public static string[] ParseUploadedFileNames(IEnumerable<string> content)
        {
            try
            {
                int countLines = 0;

                List<string> contentLines = new List<string>();
                List<string> fileNames = new List<string>();

                // The first three lines are content upload data;
                // the last line is the end of the content.
                // Read all the lines in the file.
                foreach (string line in content)
                {
                    // Increment the count.
                    countLines++;

                    // Only read the first four lines; this this the upload content information.
                    switch (countLines)
                    {
                        case 1:
                            // The start indicator line.
                            contentLines.Add(line);
                            break;

                        default:
                            // Is file upload Content-Disposition:
                            if ((line.Length >= 25) && (line.Substring(0, 24).ToLower().Contains("content-disposition")))
                            {
                                // Get the name of the file in the content disposition line; and the save path.
                                string[] splitContent = line.Split(new char[] { ';' });
                                string[] splitFileName = splitContent[splitContent.Length - 1].Split(new char[] { '=' });

                                // If this content-disposition contains the name 'filename'
                                // then attempt to extract the name of the file
                                if (splitFileName[0].ToLower().Contains("filename"))
                                {
                                    // Extract the file name.
                                    string fileName = splitFileName[1].Replace("\"", "");
                                    if (!string.IsNullOrEmpty(fileName))
                                    {
                                        string fileNamePath = fileName;
                                        fileNames.Add(fileNamePath);
                                    }
                                }
                            }
                            break;
                    }
                }

                // Return the collection of file names in the opload content file.
                return fileNames.ToArray();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Write the current file data to the uploaded file name.
        /// </summary>
        /// <param name="originalDataFile">The original upload content file to read from.</param>
        /// <param name="fileName">The upload file name to write the data to.</param>
        /// <param name="start">The starting offset to start reading from.</param>
        /// <param name="numberToRead">The number of bytes to read.</param>
        public static void UploadFileData(string originalDataFile, string fileName, int start, int numberToRead)
        {
            // Make sure a file name has been passed.
            if (!String.IsNullOrEmpty(fileName))
            {
                // Create the file and binary stream readers and writers.
                using (System.IO.FileStream streamRead = new System.IO.FileStream(originalDataFile, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                using (System.IO.FileStream streamWrite = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite))
                using (System.IO.BinaryReader reader = new System.IO.BinaryReader(streamRead))
                using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(streamWrite))
                {
                    // Copy the upload content data to the file uploaded.
                    TransferData(reader, writer, start, numberToRead);

                    // Close the streams.
                    writer.Flush();
                    reader.Close();
                    writer.Close();
                }
            }
        }

        /// <summary>
        /// Parse a uploaded content file to the original uploaded data.
        /// </summary>
        /// <param name="uploadedFilePath">The current temp file with upload content.</param>
        /// <returns>The list of file names within the data.</returns>
        public static string[] ParseUploadedFileEx(string uploadedFilePath)
        {
            bool isError = false;
            System.IO.FileStream reader = null;
            List<string> fileNames = new List<string>();

            try
            {
                int readSize = 0;
                int countLines = 0;
                int position = 0;
                int fileNameCounter = -1;
                bool foundUploadFile = false;
                bool foundEndWindow = false;

                List<string> contentLines = new List<string>();
                List<int> offsetStart = new List<int>();
                List<int> numberOfBytesToTake = new List<int>();

                // Open a new stream file to the uploaded content data file.
                using (reader = new System.IO.FileStream(uploadedFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                {
                    // Read all the bytes of the file.
                    byte[] buffer = new byte[reader.Length];
                    readSize = reader.Read(buffer, 0, buffer.Length);

                    // Get ASCII string equivalent. 
                    string linecurrent = Encoding.ASCII.GetString(buffer);

                    // Split all the content into individual lines
                    // split by CR-LF.
                    string[] lines = linecurrent.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                    // For each line in the file.
                    foreach (string line in lines)
                    {
                        // Get the current line ending position (new line starting position).
                        // Add ther value 2 at the end which is the CR-LF bytes.
                        position += line.Length + 2;

                        // Increment the count.
                        countLines++;

                        // Only read the first four lines; this the upload content information.
                        switch (countLines)
                        {
                            case 1:
                                // The start indicator line.
                                contentLines.Add(line);
                                break;

                            default:
                                // Is file upload Content-Disposition:
                                if ((line.Length >= 25) && (line.Substring(0, 24).ToLower().Contains("content-disposition")))
                                {
                                    // Get the name of the file in the content disposition line; and the save path.
                                    string[] splitContent = line.Split(new char[] { ';' });
                                    string[] splitFileName = splitContent[splitContent.Length - 1].Split(new char[] { '=' });

                                    // If this content-disposition contains the name 'filename'
                                    // then attempt to extract the name of the file
                                    if (splitFileName[0].ToLower().Contains("filename"))
                                    {
                                        // Extract the file name.
                                        string fileName = splitFileName[1].Replace("\"", "");
                                        if (!string.IsNullOrEmpty(fileName))
                                        {
                                            // Upload file has been found.
                                            foundUploadFile = true;
                                            fileNameCounter++;

                                            string fileNamePath = System.IO.Path.GetDirectoryName(uploadedFilePath).TrimEnd('\\') + "\\" + fileName;
                                            fileNames.Add(fileNamePath);
                                        }
                                    }
                                }

                                // If a file has been found.
                                if (foundUploadFile)
                                {
                                    // Find the position where the file data starts.
                                    if (!foundEndWindow && line.Length <= 0)
                                    {
                                        foundEndWindow = true;
                                        offsetStart.Add(position);
                                    }

                                    // If the end of the file section has been found.
                                    if (line.ToLower().Contains(contentLines[0].ToLower()))
                                    {
                                        foundUploadFile = false;
                                        foundEndWindow = false;

                                        // Get the number of bytes to take. The current position
                                        // mimus the starting offset the current end of data
                                        // content line and mimus the two last CR-LF values (4 bytes);
                                        numberOfBytesToTake.Add(position - (offsetStart[fileNameCounter] + line.Length + 2 + 2));
                                    }
                                }
                                break;
                        }
                    }

                    // Close the streams.
                    reader.Close();
                }

                // For each file found in the uploaded content file
                // write each upload file into individual files.
                for (int i = 0; i < fileNames.Count; i++)
                {
                    try
                    {
                        // Write the current file data to the uploaded file name.
                        UploadFileData(uploadedFilePath, fileNames[i], offsetStart[i], numberOfBytesToTake[i]);
                    }
                    catch (Exception)
                    {
                        // Error has occrred.
                        isError = true;
                    }
                }
            }
            catch (Exception)
            {
                // Error has occrred.
                isError = true;
                throw;
            }
            finally
            {
                if (reader != null)
                    reader.Close();

                // If no error the delete the file.
                if (!isError)
                    // If the local file exits the delete it.
                    if (!String.IsNullOrEmpty(uploadedFilePath))
                        if (System.IO.File.Exists(uploadedFilePath))
                            System.IO.File.Delete(uploadedFilePath);
            }

            // Send back the array of uploaded files.
            return fileNames.ToArray();
        }

        /// <summary>
        /// Parse a single uploaded content file to the original uploaded data.
        /// </summary>
        /// <param name="uploadedFilePath">The current temp file with upload content.</param>
        public static void ParseSingleUploadedFile(string uploadedFilePath)
        {
            bool isError = false;

            try
            {
                int countLines = 0;
                List<string> contentLines = new List<string>();

                // The first three lines are content upload data;
                // the last line is the end of the content.
                // Read all the lines in the file.
                foreach (string line in System.IO.File.ReadLines(uploadedFilePath))
                {
                    // Only read the first four lines; this this the upload content information.
                    switch (countLines)
                    {
                        case 0:
                            // The start indicator line.
                            contentLines.Add(line);
                            break;
                        case 1:
                            // The content disposition line.
                            contentLines.Add(line);
                            break;
                        default:
                            // Read all other lines until the empty line before the upload file data.
                            contentLines.Add(line);
                            break;
                    }

                    // If the empty line has been found.
                    if (contentLines[countLines].Length <= 0)
                        break;

                    // Increment the count.
                    countLines++;
                }

                // The end is two bytes bigger than the start
                // 2 + 2 are the last two carrage retuen line feeds
                // of the uploaded conrent file. The first CR-LF is
                // after the data and the second CR-LF is after
                // the end of file indicator.
                string end = contentLines[0] + "--";
                int endLength = end.Length + 2 + 2;

                // Get the current upload content file size.
                long fileSize = new System.IO.FileInfo(uploadedFilePath).Length;

                // The start reading offset is all the content lengths
                // plus two ( \r\n - carrage return line feed).
                int offsetLength = 0;
                foreach (string lineItem in contentLines)
                    offsetLength += (lineItem.Length + 2);

                // Get the name of the file in the content disposition line; and the save path.
                string[] splitContent = contentLines[1].Split(new char[] { ';' });
                string[] splitFileName = splitContent[splitContent.Length - 1].Split(new char[] { '=' });
                string fileName = splitFileName[1].Replace("\"", "");
                string fileNamePath = System.IO.Path.GetDirectoryName(uploadedFilePath).TrimEnd('\\') + "\\" + fileName;

                // Make sure a file name has been passed.
                if (!String.IsNullOrEmpty(fileName))
                {
                    // Create the file and binary stream readers and writers.
                    using (System.IO.FileStream streamRead = new System.IO.FileStream(uploadedFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                    using (System.IO.FileStream streamWrite = new System.IO.FileStream(fileNamePath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite))
                    using (System.IO.BinaryReader reader = new System.IO.BinaryReader(streamRead))
                    using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(streamWrite))
                    {
                        // Copy the upload content data to the file uploaded.
                        TransferData(reader, writer, (int)fileSize, offsetLength, endLength);

                        // Close the streams.
                        writer.Flush();
                        reader.Close();
                        writer.Close();
                    }
                }
            }
            catch (Exception)
            {
                // Error has occrred.
                isError = true;
                throw;
            }
            finally
            {
                // If no error the delete the file.
                if (!isError)
                    // If the local file exits the delete it.
                    if (!String.IsNullOrEmpty(uploadedFilePath))
                        if (System.IO.File.Exists(uploadedFilePath))
                            System.IO.File.Delete(uploadedFilePath);
            }
        }

        /// <summary>
        /// Set the request headers from the input stream within the current http context.
        /// </summary>
        /// <param name="httpContext">The current http context.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely.</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <param name="requestBufferStore">The request buffer store stream.</param>
        /// <exception cref="System.Exception"></exception>
        /// <returns>True if the headers have been found; else false.</returns>
        public static bool SetRequestHeaders(Nequeo.Net.Http.HttpContext httpContext, long timeout = -1, int maxReadLength = 0, System.IO.Stream requestBufferStore = null)
        {
            // Header has not been found at this point.
            string requestMethod = "";
            string protocolVersion = "";
            byte[] rawData = null;
            List<NameValue> headers = null;

            // Http context is null.
            if (httpContext == null)
                return false;

            // Http request context is null.
            if (httpContext.HttpRequest == null)
                return false;

            // Http request context stream is null.
            if (httpContext.HttpRequest.Input == null)
                return false;

            // If not using the buffer store.
            if (requestBufferStore == null)
            {
                // We need to wait until we get all the header
                // data then send the context to the server.
                headers = Nequeo.Net.Utility.
                    ParseHeaders(httpContext.HttpRequest.Input, out requestMethod, ref rawData, timeout, maxReadLength);
            }
            else
            {
                // We need to wait until we get all the header
                // data then send the context to the server.
                headers = Nequeo.Net.Utility.
                    ParseHeaders(httpContext.RequestBufferStore, out requestMethod, ref rawData, timeout, maxReadLength);
            }

            // If headers exist then all has been found.
            if (headers != null)
            {
                // Set all the request headers.
                httpContext.HttpRequest.ReadHttpHeaders(headers, requestMethod);
                protocolVersion = httpContext.HttpRequest.ProtocolVersion;
                httpContext.HttpRequest.HeadersFound = true;

                // If the client is using protocol version "HTTP/1.1"
                if (protocolVersion.ToUpper().Trim().Replace(" ", "").Contains("HTTP/1.1"))
                {
                    // Do nothing.
                }

                // If the client is using protocol version "HTTP/2.0".
                if (protocolVersion.ToUpper().Trim().Replace(" ", "").Contains("HTTP/2"))
                {
                    // Do nothing.
                }

                // Set the user principle if credentials
                // have been passed.
                if (httpContext.HttpRequest.Credentials != null)
                {
                    // Add the credentials.
                    Nequeo.Security.IdentityMember identity =
                        new Nequeo.Security.IdentityMember(
                            httpContext.HttpRequest.Credentials.UserName,
                            httpContext.HttpRequest.Credentials.Password,
                            httpContext.HttpRequest.Credentials.Domain);

                    Nequeo.Security.AuthenticationType authType = Nequeo.Security.AuthenticationType.None;
                    try
                    {
                        // Attempt to get the authentication type.
                        authType = (Nequeo.Security.AuthenticationType)
                            Enum.Parse(typeof(Nequeo.Security.AuthenticationType), httpContext.HttpRequest.AuthorizationType);
                    }
                    catch { }

                    // Set the cuurent authentication schema.
                    identity.AuthenticationSchemes = authType;

                    // Create the principal.
                    Nequeo.Security.PrincipalMember principal = new Nequeo.Security.PrincipalMember(identity, null);

                    // Assign the principal
                    httpContext.User = principal;
                }

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Parse the form data within the stream, including any uploaded file data.
        /// </summary>
        /// <param name="form">The stream containing the data to parse.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait Indefinitely</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The http form model parsed from the stream; else null.</returns>
        public static Nequeo.Net.Http.Common.HttpFormModel ParseForm(byte[] form, long timeout = -1, int maxReadLength = 0)
        {
            System.IO.BinaryReader reader = null;
            System.IO.MemoryStream buffer = null;
            Nequeo.Net.Http.Common.HttpFormModel model = null;

            try
            {
                // Create a new buffer
                using (buffer = new System.IO.MemoryStream(form))
                using(reader = new System.IO.BinaryReader(buffer, Encoding.Default, true))
                {
                    // Return the result.
                    model = Nequeo.Net.Http.Utility.ParseForm(reader, timeout, maxReadLength);
                }

                // Return the model.
                return model;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (buffer != null)
                    buffer.Dispose();

                if (reader != null)
                    reader.Dispose();
            }
        }

        /// <summary>
        /// Parse the form data within the stream, including any uploaded file data.
        /// </summary>
        /// <param name="form">The stream containing the data to parse.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The http form model parsed from the stream; else null.</returns>
        public static Nequeo.Net.Http.Common.HttpFormModel ParseForm(System.IO.BinaryReader form, long timeout = -1, int maxReadLength = 0)
        {
            bool endOfData = false;
            Nequeo.Net.Http.Common.HttpFormModel formModel = null;

            try
            {
                int bytesRead = 0;
                bool foundSectionHeaders = false;
                byte[] buffer = new byte[1];
                byte[] sectionHeader = new byte[0];
                int position = 0;

                // Start a new timeout clock.
                Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);

                // While the end of the section header data
                // has not been found.
                while (!foundSectionHeaders)
                {
                    // Read a single byte from the stream
                    // add the data to the store and re assign.
                    bytesRead = form.Read(buffer, 0, 1);

                    // If data exists.
                    if (bytesRead > 0)
                    {
                        // Each time data is read reset the timeout.
                        timeoutClock.Reset();
                        byte[] temp = sectionHeader.CombineParallel(buffer);
                        sectionHeader = temp;

                        // If the store contains the right
                        // amount of data.
                        if (sectionHeader.Length > 1)
                        {
                            // If the end of the header data has been found
                            // \r\n (13 10).
                            if (sectionHeader[position] == 10 && sectionHeader[position - 1] == 13)
                            {
                                // The end of the section header data has been found.
                                foundSectionHeaders = true;
                                break;
                            }
                        }

                        // Increment the position.
                        position++;
                    }
                    else
                        SpinWaitHandler(form, timeoutClock);

                    // If the timeout has bee reached then
                    // break from the loop.
                    if (timeoutClock.IsComplete())
                        break;

                    // Only if > than zero.
                    if (maxReadLength > 0)
                    {
                        // If max read length then exit the loop.
                        if (sectionHeader.Length > maxReadLength)
                            break;
                    }
                }

                // If the end of headers has been found.
                if (foundSectionHeaders)
                {
                    // Get the section header.
                    string section = System.Text.Encoding.Default.GetString(sectionHeader, 0, sectionHeader.Length - 2);

                    // Create the new http form model.
                    formModel = new Common.HttpFormModel();

                    // Keep on reading until the end of the data.
                    while (!endOfData)
                    {
                        // Get the current content headers.
                        NameValueCollection contentHeader = GetContentHeader(form, timeout, maxReadLength);

                        // If headers exist.
                        if (contentHeader != null)
                        {
                            // Get the content disposition type.
                            string formName = null;
                            int dispositionType = ContentDispositionType(contentHeader, formModel, out formName);

                            // Select the correct content disposition type.
                            switch (dispositionType)
                            {
                                case 1:
                                    // Form content disposition.
                                    endOfData = GetFormValue(form, formModel.Form, formName, section, timeout, maxReadLength);
                                    break;

                                case 2:
                                    // Form upload file content disposition.
                                    endOfData = GetUploadFileValue(form, formModel.UploadedFiles[formModel.UploadedFiles.Count - 1], section, timeout, maxReadLength);
                                    break;

                                default:
                                    endOfData = true;
                                    break;
                            }
                        }
                        else
                            break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            // Return the form data.
            return formModel;
        }

        /// <summary>
        /// Get the content headers.
        /// </summary>
        /// <param name="form">The stream containing the data to parse.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>The collection of content headers.</returns>
        private static NameValueCollection GetContentHeader(System.IO.BinaryReader form, long timeout = -1, int maxReadLength = 0)
        {
            int bytesRead = 0;
            bool foundEndOfHeaders = false;
            byte[] buffer = new byte[1];
            byte[] store = new byte[0];
            int position = 0;

            // Start a new timeout clock.
            Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);

            // Create a header name value collection.
            NameValueCollection headers = null;

            // While the end of the header data
            // has not been found.
            while (!foundEndOfHeaders)
            {
                // Read a single byte from the stream
                // add the data to the store and re assign.
                bytesRead = form.Read(buffer, 0, 1);

                // If data exists.
                if (bytesRead > 0)
                {
                    // Each time data is read reset the timeout.
                    timeoutClock.Reset();
                    byte[] temp = store.CombineParallel(buffer);
                    store = temp;

                    // If the store contains the right
                    // amount of data.
                    if (store.Length > 3)
                    {
                        // If the end of the header data has been found
                        // \r\n\r\n (13 10 13 10).
                        if (store[position] == 10 &&
                            store[position - 1] == 13 &&
                            store[position - 2] == 10 &&
                            store[position - 3] == 13)
                        {
                            // The end of the header data has been found.
                            foundEndOfHeaders = true;
                            break;
                        }
                    }

                    // Increment the position.
                    position++;
                }
                else
                    SpinWaitHandler(form, timeoutClock);

                // If the timeout has bee reached then
                // break from the loop.
                if (timeoutClock.IsComplete())
                    break;

                // Only if > than zero.
                if (maxReadLength > 0)
                {
                    // If max read length then exit the loop.
                    if (store.Length >= maxReadLength)
                        break;
                }
            }

            // If the end of headers has been found.
            if (foundEndOfHeaders)
            {
                // Get the header store minus the ending bytes,
                // split the headers into a collection.
                string headersStore = System.Text.Encoding.Default.GetString(store, 0, store.Length - 4);
                string[] headerCol = headersStore.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                // Create a header name value collection.
                headers = new NameValueCollection();

                // For each header
                for (int i = 0; i < headerCol.Length; i++)
                {
                    // Make sure a valid header exists.
                    if (!String.IsNullOrEmpty(headerCol[i]))
                    {
                        // Split the name and value header pair.
                        string[] nameValue = headerCol[i].Split(new string[] { ":" }, StringSplitOptions.None);

                        // Get the values
                        string values = "";
                        for (int j = 1; j < nameValue.Length; j++)
                            values += nameValue[j] + ":";

                        // Add the header.
                        headers.Add(nameValue[0].Trim(), values.TrimEnd(new char[] { ':' }).Trim());
                    }
                }
            }

            // Return the content headers.
            return headers;
        }

        /// <summary>
        /// Get form data content disposition indicators.
        /// </summary>
        /// <param name="contentHeader">The current content headers.</param>
        /// <param name="formModel">The http form model.</param>
        /// <param name="formName">The form name associated with the value.</param>
        /// <returns>A value indicating the type of form data.</returns>
        private static int ContentDispositionType(NameValueCollection contentHeader, Nequeo.Net.Http.Common.HttpFormModel formModel, out string formName)
        {
            int ret = 0;
            formName = null;

            // Does form content exist.
            if (!String.IsNullOrEmpty(contentHeader["Content-Disposition"]))
            {
                // Get all content vaues.
                string[] contentValues = contentHeader["Content-Disposition"].Split(new string[] { ";" }, StringSplitOptions.None).Trim();
                string nameValue = null;

                // For each value for the header.
                foreach (string item in contentValues)
                {
                    // Get the name value pair.
                    string[] contentNameValue = item.Split(new string[] { "=" }, StringSplitOptions.None).Trim();

                    // If the name value exists.
                    if (contentNameValue[0].ToLower() == "name".ToLower())
                    {
                        // If the form collection has not been created.
                        if (formModel.Form == null)
                            formModel.Form = new NameValueCollection();

                        // Form data exists.
                        ret = 1;

                        // Get the name value.
                        nameValue = contentNameValue[1].Replace("\"", "");
                        formName = nameValue;

                        // Add the name 
                        formModel.Form.Add(nameValue.Trim(), "");
                    }

                    // If the filename value exists.
                    if (contentNameValue[0].ToLower() == "filename".ToLower())
                    {
                        // Get the filename value.
                        string filenameValue = contentNameValue[1].Replace("\"", "");

                        // If a file name has been specified.
                        if (!String.IsNullOrEmpty(filenameValue))
                        {
                            // If the form collection has not been created.
                            if (formModel.UploadedFiles == null)
                                formModel.UploadedFiles = new List<Common.HttpUploadFileModel>();

                            // Uploaded file data exists.
                            ret = 2;
                            string contentType = null;

                            // Does form content exist.
                            if (!String.IsNullOrEmpty(contentHeader["Content-Type"]))
                            {
                                // Get the file content type.
                                contentType = contentHeader["Content-Type"];
                            }

                            // Create the http upload file model.
                            Common.HttpUploadFileModel uploadModel = new Common.HttpUploadFileModel()
                            {
                                ContentType = contentType,
                                FileName = filenameValue,
                                Name = nameValue,
                            };

                            // Add the file to the collection.
                            formModel.UploadedFiles.Add(uploadModel);
                        }
                    }
                }
            }

            // Return result.
            return ret;
        }

        /// <summary>
        /// Get the current form value.
        /// </summary>
        /// <param name="form">The stream containing the data to parse.</param>
        /// <param name="nameValues">The form name value collection.</param>
        /// <param name="name">The form name for the current value.</param>
        /// <param name="section">The divider section name.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>True if end of data; else false.</returns>
        private static bool GetFormValue(System.IO.BinaryReader form, NameValueCollection nameValues, string name, string section, long timeout = -1, int maxReadLength = 0)
        {
            bool endOfData = false;
            int bytesRead = 0;
            bool foundEndOfSection = false;
            byte[] buffer = new byte[1];
            byte[] store = new byte[0];

            // Start a new timeout clock.
            Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);
            
            // While the end of the section data
            // has not been found.
            while (!foundEndOfSection)
            {
                // Read a single byte from the stream
                // add the data to the store and re assign.
                bytesRead = form.Read(buffer, 0, 1);

                // If data exists.
                if (bytesRead > 0)
                {
                    // Each time data is read reset the timeout.
                    timeoutClock.Reset();
                    byte[] temp = store.CombineParallel(buffer);
                    store = temp;

                    // If the store contains the right
                    // amount of data.
                    if (store.Length > section.Length)
                    {
                        // Get a subset of the current data.
                        byte[] currentBytes = store.Skip(store.Length - section.Length).Take(section.Length).ToArray();
                        string currentData = Encoding.Default.GetString(currentBytes);

                        // If the end was found.
                        if (currentData.ToLower() == section.ToLower())
                        {
                            foundEndOfSection = true;
                            break;
                        }
                    }
                }
                else
                    SpinWaitHandler(form, timeoutClock);

                // If the timeout has bee reached then
                // break from the loop.
                if (timeoutClock.IsComplete())
                    break;

                // Only if > than zero.
                if (maxReadLength > 0)
                {
                    // If max read length then exit the loop.
                    if (store.Length >= maxReadLength)
                        break;
                }
            }

            // If the end of headers has been found.
            if (foundEndOfSection)
            {
                // Get the form value.
                byte[] formValueBytes = store.Take(store.Length - section.Length - 2).ToArray();
                nameValues[name] = Encoding.Default.GetString(formValueBytes);
            }

            // Return the current end of data state.
            return endOfData;
        }

        /// <summary>
        /// Get the current upload file value.
        /// </summary>
        /// <param name="form">The stream containing the data to parse.</param>
        /// <param name="uploadModel">The current upload file model.</param>
        /// <param name="section">The divider section name.</param>
        /// <param name="timeout">The maximum time in milliseconds to wait for the end of the header data; -1 wait indefinitely</param>
        /// <param name="maxReadLength">The maximun number of bytes to read before cancelling (must be greater then zero).</param>
        /// <returns>True if end of data; else false.</returns>
        private static bool GetUploadFileValue(System.IO.BinaryReader form, Common.HttpUploadFileModel uploadModel, string section, long timeout = -1, int maxReadLength = 0)
        {
            bool endOfData = false;

            // If a file name has been specified.
            if (!String.IsNullOrEmpty(uploadModel.FileName))
            {
                int bytesRead = 0;
                bool foundEndOfSection = false;
                byte[] buffer = new byte[1];
                byte[] store = new byte[0];

                // Start a new timeout clock.
                Custom.TimeoutClock timeoutClock = new Custom.TimeoutClock((int)timeout);

                // While the end of the section data
                // has not been found.
                while (!foundEndOfSection)
                {
                    // Read a single byte from the stream
                    // add the data to the store and re assign.
                    bytesRead = form.Read(buffer, 0, 1);

                    // If data exists.
                    if (bytesRead > 0)
                    {
                        // Each time data is read reset the timeout.
                        timeoutClock.Reset();
                        byte[] temp = store.CombineParallel(buffer);
                        store = temp;

                        // If the store contains the right
                        // amount of data.
                        if (store.Length > section.Length)
                        {
                            // Get a subset of the current data.
                            byte[] currentBytes = store.Skip(store.Length - section.Length).Take(section.Length).ToArray();
                            string currentData = Encoding.Default.GetString(currentBytes);

                            // If the end was found.
                            if (currentData.ToLower() == section.ToLower())
                            {
                                foundEndOfSection = true;
                                break;
                            }
                        }
                    }
                    else
                        SpinWaitHandler(form, timeoutClock);

                    // If the timeout has bee reached then
                    // break from the loop.
                    if (timeoutClock.IsComplete())
                        break;

                    // Only if > than zero.
                    if (maxReadLength > 0)
                    {
                        // If max read length then exit the loop.
                        if (store.Length >= maxReadLength)
                            break;
                    }
                }

                // If the end of headers has been found.
                if (foundEndOfSection)
                {
                    // Get the form upload file data.
                    byte[] formValueBytes = store.Take(store.Length - section.Length - 2).ToArray();
                    uploadModel.UploadFile = new System.IO.MemoryStream(formValueBytes);
                }
            }

            // Return the current end of data state.
            return endOfData;
        }

        /// <summary>
        /// Spin wait until data is avaiable or timed out.
        /// </summary>
        /// <param name="source">The source stream to check.</param>
        /// <param name="timeoutClock">The time to check.</param>
        private static void SpinWaitHandler(System.IO.Stream source, Custom.TimeoutClock timeoutClock)
        {
            bool exitIndicator = false;

            // Create the tasks.
            Task[] tasks = new Task[1];

            // Poller task.
            Task poller = Task.Factory.StartNew(() =>
            {
                // Create a new spin wait.
                SpinWait sw = new SpinWait();

                // Action to perform.
                while (!exitIndicator)
                {
                    // The NextSpinWillYield property returns true if 
                    // calling sw.SpinOnce() will result in yielding the 
                    // processor instead of simply spinning. 
                    if (sw.NextSpinWillYield)
                    {
                        if (timeoutClock.IsComplete() || source.Length > 0) exitIndicator = true;
                    }
                    sw.SpinOnce();
                }
            });

            // Assign the listener task.
            tasks[0] = poller;

            // Wait for all tasks to complete.
            Task.WaitAll(tasks);

            // For each task.
            foreach (Task item in tasks)
            {
                try
                {
                    // Release the resources.
                    item.Dispose();
                }
                catch { }
            }
            tasks = null;
        }

        /// <summary>
        /// Spin wait until data is avaiable or timed out.
        /// </summary>
        /// <param name="source">The source stream to check.</param>
        /// <param name="timeoutClock">The time to check.</param>
        private static void SpinWaitHandler(System.IO.BinaryReader source, Custom.TimeoutClock timeoutClock)
        {
            bool exitIndicator = false;

            // Create the tasks.
            Task[] tasks = new Task[1];

            // Poller task.
            Task poller = Task.Factory.StartNew(() =>
            {
                // Create a new spin wait.
                SpinWait sw = new SpinWait();

                // Action to perform.
                while (!exitIndicator)
                {
                    // The NextSpinWillYield property returns true if 
                    // calling sw.SpinOnce() will result in yielding the 
                    // processor instead of simply spinning. 
                    if (sw.NextSpinWillYield)
                    {
                        if (timeoutClock.IsComplete() || source.BaseStream.Length > 0) exitIndicator = true;
                    }
                    sw.SpinOnce();
                }
            });

            // Assign the listener task.
            tasks[0] = poller;

            // Wait for all tasks to complete.
            Task.WaitAll(tasks);

            // For each task.
            foreach (Task item in tasks)
            {
                try
                {
                    // Release the resources.
                    item.Dispose();
                }
                catch { }
            }
            tasks = null;
        }
	}
}
