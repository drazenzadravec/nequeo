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
using System.Net;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Security;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel.Composition;

namespace Nequeo.Net.Ftp
{
    #region Public Event Handler Delegate
    /// <summary>
    /// This delegate handles the transport of data from sender
    /// to receiver when ftp socket process error occurs.
    /// </summary>
    /// <param name="sender">The current object sending the message</param>
    /// <param name="e">The object containing the event information.</param>
    public delegate void FTPSocketErrorHandler(object sender, FTPSocketErrorArgs e);

    /// <summary>
    /// This delegate handles the transport of data from sender
    /// to receiver when ftp socket process occurs.
    /// </summary>
    /// <param name="sender">The current object sending the message</param>
    /// <param name="e">The object containing the event information.</param>
    public delegate void FTPSocketHandler(object sender, FTPSocketArgs e);
    #endregion

    #region Public Callback Delegates
    /// <summary>
    /// FTP web socket callback delegate handler.
    /// </summary>
    /// <param name="processIndex">The unique processing index.</param>
    /// <param name="processingType">The current processing type.</param>
    /// <param name="processInformation">The processing message.</param>
    public delegate void FTPWebSocketCallbackHandler(int processIndex,
        string processingType, string processInformation);
    #endregion

    /// <summary>
    /// A complete FTP integration socket to an FTP server,
    /// contains methods to upload/download data to and
    /// from the specified FTP sever.
    /// </summary>
    [Export(typeof(IFTPSocket))]
    public partial class FTPSocket : Nequeo.Runtime.DisposableBase, IDisposable, IFTPSocket
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public FTPSocket()
        {
            OnCreated();
        }

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="callbackHandler">Processing callback delegate.</param>
        public FTPSocket(FTPWebSocketCallbackHandler callbackHandler)
        {
            OnCreated();
            this.callbackHandler = callbackHandler;
        }
        #endregion

        #region Private Fields
        private FtpStatusCode ftpStatusCode = FtpStatusCode.Undefined;
        private string ftpStatusDescription = string.Empty;
        private FTPWebSocketCallbackHandler callbackHandler = null;
        private Nequeo.Security.X509Certificate2Info _sslCertificate = null;
        #endregion

        #region Public Events
        /// <summary>
        /// This event occurs when an error occurs 
        /// attempting to communicate with the ftp server.
        /// </summary>
        public event FTPSocketErrorHandler OnFTPError;

        /// <summary>
        /// This event occurs when the download 
        /// has completed through the ftp server.
        /// </summary>
        public event FTPSocketHandler OnDownloadComplete;

        /// <summary>
        /// This event occurs when upload
        /// has completed through the ftp server.
        /// </summary>
        public event FTPSocketHandler OnUploadComplete;

        /// <summary>
        /// This event occurs when directory removal
        /// has completed through the ftp server.
        /// </summary>
        public event FTPSocketHandler OnDirectoryRemovedComplete;

        /// <summary>
        /// This event occurs when delete a file
        /// has completed through the ftp server.
        /// </summary>
        public event FTPSocketHandler OnDeleteFileComplete;

        /// <summary>
        /// This event occurs when creation of a directory
        /// has completed through the ftp server.
        /// </summary>
        public event FTPSocketHandler OnMakeDirectoryComplete;

        /// <summary>
        /// This event occurs when renaming a file
        /// has completed through the ftp server.
        /// </summary>
        public event FTPSocketHandler OnRenameFileComplete;

        /// <summary>
        /// This event occurs when a directory list
        /// has completed through the ftp server.
        /// </summary>
        public event FTPSocketHandler OnDirectoryListComplete;

        /// <summary>
        /// This event occurs when a file size
        /// has completed through the ftp server.
        /// </summary>
        public event FTPSocketHandler OnFileSizeComplete;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get, the most recent ftp status code from the server.
        /// </summary>
        public FtpStatusCode FtpStatusCode
        {
            get { return ftpStatusCode; }
        }

        /// <summary>
        /// Get, the most recent ftp status description from the server.
        /// </summary>
        public string FtpStatusDescription
        {
            get { return ftpStatusDescription; }
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Get, the secure certificate.
        /// </summary>
        public Nequeo.Security.X509Certificate2Info Certificate
        {
            get { return _sslCertificate; }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// This method will invoke the callback handler delegate.
        /// </summary>
        /// <param name="processIndex">The unique processing index.</param>
        /// <param name="processingType">The current processing type.</param>
        /// <param name="processInformation">The processing message.</param>
        private void DelegateCallbackHandler(int processIndex,
            string processingType, string processInformation)
        {
            // Invoke the delegate and send the message
            // to the calling thread.
            if (callbackHandler != null)
                callbackHandler(processIndex, processingType, processInformation);
        }

        /// <summary>
        /// Certificate validator.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="certificate">The certificate</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The policy error.</param>
        /// <returns>True if the certificate is valid else false.</returns>
        private bool OnCertificateValidation(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Create a new instance of the x509 certificate 
            // information class.
            _sslCertificate = new Nequeo.Security.X509Certificate2Info(
                certificate as X509Certificate2, chain, sslPolicyErrors);

            // Get the current error level.
            if (sslPolicyErrors == SslPolicyErrors.None) 
                return true;
            else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable)
                return false;
            else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
                return false;
            else 
            {
                //Remote Certificate Name Mismatch
                System.Security.Policy.Zone z = 
                    System.Security.Policy.Zone.CreateFromUrl(((HttpWebRequest)sender).RequestUri.ToString());

                // Get the security zone for
                // the current request URI.
                if (z.SecurityZone == System.Security.SecurityZone.Intranet ||
                    z.SecurityZone == System.Security.SecurityZone.MyComputer ||
                    z.SecurityZone == System.Security.SecurityZone.NoZone ||
                    z.SecurityZone == System.Security.SecurityZone.Trusted)
                    return true;
                
                // Return false otherwise.
                return false;
            }
        }

        /// <summary>
        /// Certificate override validator.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="certificate">The certificate</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The policy error.</param>
        /// <returns>True if the certificate is valid else false.</returns>
        private bool OnCertificateValidationOverride(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Create a new instance of the x509 certificate 
            // information class.
            _sslCertificate = new Nequeo.Security.X509Certificate2Info(
                certificate as X509Certificate2, chain, sslPolicyErrors);

            return true;
        }
        #endregion

        #region Private Transfer Methods
        /// <summary>
        /// Download a file from the specified server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="destinationFile">The full path and file name where data should be written to.</param>
        /// <param name="downloadUri">The Uri object containing a valid file to download.</param>
        /// <returns>True if the download was successful else false.</returns>
        private bool DownloadEx(FTPConnectionAdapter ftpAdapter,
            string destinationFile, Uri downloadUri)
        {
            // The ftp response object,
            // and ftp request object.
            FtpWebResponse ftpResponse = null;
            FtpWebRequest ftpRequest = null;

            // The read and write stream objects
            // for the transfer of data.
            Stream stream = null;
            StreamReader reader = null;
            StreamWriter writer = null;

            try
            {
                // Check if the URI is valid FTP scheme.
                if (downloadUri.Scheme != Uri.UriSchemeFtp)
                    throw new ArgumentException("Uri is not a valid FTP scheme");

                // Extract the directory path.
                string sFolderPath = System.IO.Path.GetDirectoryName(destinationFile);

                // If the directory does not exists create it.
                if (!Directory.Exists(sFolderPath))
                    Directory.CreateDirectory(sFolderPath);

                // Set the ftp request object.
                ftpRequest = (FtpWebRequest)WebRequest.Create(downloadUri);

                // If a proxy server has been specified.
                if (ftpAdapter.Proxy != null)
                    ftpRequest.Proxy = ftpAdapter.Proxy;

                // If a client certificate is to be used
                // add the client certificate.
                if (ftpAdapter.ClientCertificate != null)
                    ftpRequest.ClientCertificates.Add(ftpAdapter.ClientCertificate);

                // If the ftp connection is not
                // anonymous then create a new
                // network credential.
                if (!ftpAdapter.IsAnonymousUser)
                    ftpRequest.Credentials = new NetworkCredential(
                        ftpAdapter.UserName, ftpAdapter.Password, ftpAdapter.Domain);
                else
                    ftpRequest.Credentials = new NetworkCredential("anonymous", "anonymous@anonymoususer.com");

                // Assign the download file method to the
                // FTP request object. Time out 5 minutes.
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpRequest.Timeout = ftpAdapter.TimeOut;
                ftpRequest.UsePassive = ftpAdapter.UsePassive;
                ftpRequest.UseBinary = ftpAdapter.UseBinary;
                ftpRequest.EnableSsl = ftpAdapter.UseSSLConnection;
                ftpRequest.KeepAlive = false;

                // Override the validation of
                // the server certificate.
                if (ftpAdapter.UseSSLConnection && ftpAdapter.SslCertificateOverride)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidationOverride); 
                else if(ftpAdapter.UseSSLConnection)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidation); 

                // Assign the FTP response object from
                // the FTP request server.
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                // Get the response stream and create the read and
                // write stream for data exchange.
                using (stream = ftpResponse.GetResponseStream())
                using (reader = new StreamReader(stream, Encoding.UTF8))
                using (writer = new StreamWriter(destinationFile, false))
                {
                    // Get the number of bytes in total
                    // that will be sent by the server.
                    long downloadFileSize = ftpResponse.ContentLength;

                    // Write the data to the destination file
                    // read from the FTP server stream.
                    writer.Write(reader.ReadToEnd());

                    // Close all streams
                    stream.Close();
                    reader.Close();
                    writer.Close();
                }

                // Create a new event handler for
                // the current event, send to the client.
                this.CompletionEventHandler(ftpAdapter, ftpResponse, "Download", destinationFile,
                    Nequeo.Net.Ftp.SocketTransferDirection.Download);

                // Call back processing invoke.
                DelegateCallbackHandler(0, "Download Complete", "Downloading : " + downloadUri.ToString() +
                    " to destination : " + destinationFile);

                // Return true if download was completed.
                return true;
            }
            catch (System.Exception ex)
            {
                // Create a custom error message
                // and pass the ftp objects.
                this.ExceptionHandler(ex, ftpAdapter, ftpResponse, downloadUri, "Download", destinationFile,
                    "Downloading a file from the ftp server : " + ftpAdapter.FTPServer + ".\n" + ex.Message,
                    Nequeo.Net.Ftp.SocketTransferDirection.Download);

                // Call back processing invoke.
                DelegateCallbackHandler(-1, "Download Error", "Downloading : " + downloadUri.ToString() +
                    " to destination : " + destinationFile);

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Close and clean up
                // all streams.
                if (stream != null)
                    stream.Close();

                if (reader != null)
                    reader.Close();

                if (writer != null)
                    writer.Close();

                // Get the most recent ftp status code
                // and description from the ftp server.
                if (ftpResponse != null)
                {
                    ftpStatusCode = ftpResponse.StatusCode;
                    ftpStatusDescription = ftpResponse.StatusDescription;
                    ftpResponse.Close();
                }
            }
        }

        /// <summary>
        /// File size from the specified server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="remoteFileUri">The Uri object containing a valid file on the remote host.</param>
        /// <param name="remoteFileSize">The size of the file on the remote host.</param>
        /// <returns>True if the file size return was successful else false.</returns>
        private bool FileSizeEx(FTPConnectionAdapter ftpAdapter,
            Uri remoteFileUri, out long remoteFileSize)
        {
            // The ftp response object,
            // and ftp request object.
            FtpWebResponse ftpResponse = null;
            FtpWebRequest ftpRequest = null;

            // The read and write stream objects
            // for the transfer of data.
            Stream stream = null;
            StreamReader reader = null;

            // Initially assign the list;
            remoteFileSize = 0;

            // The string that will contain
            // the file size.
            string fileSize = string.Empty;

            try
            {
                // Check if the URI is valid FTP scheme.
                if (remoteFileUri.Scheme != Uri.UriSchemeFtp)
                    throw new ArgumentException("Uri is not a valid FTP scheme");

                // Set the ftp request object.
                ftpRequest = (FtpWebRequest)WebRequest.Create(remoteFileUri);

                // If a proxy server has been specified.
                if (ftpAdapter.Proxy != null)
                    ftpRequest.Proxy = ftpAdapter.Proxy;

                // If a client certificate is to be used
                // add the client certificate.
                if (ftpAdapter.ClientCertificate != null)
                    ftpRequest.ClientCertificates.Add(ftpAdapter.ClientCertificate);

                // If the ftp connection is not
                // anonymous then create a new
                // network credential.
                if (!ftpAdapter.IsAnonymousUser)
                    ftpRequest.Credentials = new NetworkCredential(
                        ftpAdapter.UserName, ftpAdapter.Password, ftpAdapter.Domain);
                else
                    ftpRequest.Credentials = new NetworkCredential("anonymous", "anonymous@anonymoususer.com");

                // Assign the download file method to the
                // FTP request object. Time out 5 minutes.
                ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                ftpRequest.Timeout = ftpAdapter.TimeOut;
                ftpRequest.UsePassive = ftpAdapter.UsePassive;
                ftpRequest.UseBinary = ftpAdapter.UseBinary;
                ftpRequest.EnableSsl = ftpAdapter.UseSSLConnection;
                ftpRequest.KeepAlive = false;

                // Override the validation of
                // the server certificate.
                if (ftpAdapter.UseSSLConnection && ftpAdapter.SslCertificateOverride)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidationOverride);
                else if (ftpAdapter.UseSSLConnection)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidation); 

                // Assign the FTP response object from
                // the FTP request server.
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                // Get the response stream and create the read and
                // write stream for data exchange.
                using (stream = ftpResponse.GetResponseStream())
                using (reader = new StreamReader(stream, Encoding.UTF8))
                {
                    // Read the data from the host.
                    fileSize = ftpResponse.ContentLength.ToString();

                    // Search for null characters '\r\n', '\n'.
                    // Split with current delimeter the data returned
                    // from the server. Create the new file list.
                    char[] delimeter = new char[] { };
                    string[] splitList = fileSize.Split(delimeter, StringSplitOptions.RemoveEmptyEntries);

                    // Assign the file size.
                    remoteFileSize = (long)Convert.ToInt64(fileSize.Trim());

                    // Close all streams.
                    stream.Close();
                    reader.Close();
                }

                // Create a new event handler for
                // the current event, send to the client.
                this.CompletionEventHandler(ftpAdapter, ftpResponse, "FileSize", null,
                    Nequeo.Net.Ftp.SocketTransferDirection.FileSize);

                // Call back processing invoke.
                DelegateCallbackHandler(0, "File Size Complete", "Remote file : " + remoteFileUri.ToString());

                // Return true if download was completed.
                return true;
            }
            catch (System.Exception ex)
            {
                // Create a custom error message
                // and pass the ftp objects.
                this.ExceptionHandler(ex, ftpAdapter, ftpResponse, remoteFileUri, "FileSize", null,
                    "Getting the file size from the ftp server : " + ftpAdapter.FTPServer + ".\n" + ex.Message,
                    Nequeo.Net.Ftp.SocketTransferDirection.FileSize);

                // Call back processing invoke.
                DelegateCallbackHandler(0, "File Size Error", "Remote file : " + remoteFileUri.ToString());

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Close and clean up
                // all streams.
                if (stream != null)
                    stream.Close();

                if (reader != null)
                    reader.Close();

                // Get the most recent ftp status code
                // and description from the ftp server.
                if (ftpResponse != null)
                {
                    ftpStatusCode = ftpResponse.StatusCode;
                    ftpStatusDescription = ftpResponse.StatusDescription;
                    ftpResponse.Close();
                }
            }
        }

        /// <summary>
        /// Upload a file to the specified server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="targetFile">The full path and file name of the file to upload.</param>
        /// <param name="uploadUri">The Uri object containing a valid file to upload to.</param>
        /// <returns>True if the upload was successful else false.</returns>
        private bool UploadEx(FTPConnectionAdapter ftpAdapter,
            string targetFile, Uri uploadUri)
        {
            // The ftp response object,
            // and ftp request object.
            FtpWebResponse ftpResponse = null;
            FtpWebRequest ftpRequest = null;

            // The read and write stream objects
            // for the transfer of data.
            Stream stream = null;
            StreamReader reader = null;

            try
            {
                // Check if the URI is valid FTP scheme.
                if (uploadUri.Scheme != Uri.UriSchemeFtp)
                    throw new ArgumentException("Uri is not a valid FTP scheme");

                // Set the ftp request object.
                ftpRequest = (FtpWebRequest)WebRequest.Create(uploadUri);

                // If a proxy server has been specified.
                if (ftpAdapter.Proxy != null)
                    ftpRequest.Proxy = ftpAdapter.Proxy;

                // If a client certificate is to be used
                // add the client certificate.
                if (ftpAdapter.ClientCertificate != null)
                    ftpRequest.ClientCertificates.Add(ftpAdapter.ClientCertificate);

                // If the ftp connection is not
                // anonymous then create a new
                // network credential.
                if (!ftpAdapter.IsAnonymousUser)
                    ftpRequest.Credentials = new NetworkCredential(
                        ftpAdapter.UserName, ftpAdapter.Password, ftpAdapter.Domain);
                else
                    ftpRequest.Credentials = new NetworkCredential("anonymous", "anonymous@anonymoususer.com");

                // Assign the upload file method to the
                // FTP request object. Time out 5 minutes.
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.Timeout = ftpAdapter.TimeOut;
                ftpRequest.UsePassive = ftpAdapter.UsePassive;
                ftpRequest.UseBinary = ftpAdapter.UseBinary;
                ftpRequest.EnableSsl = ftpAdapter.UseSSLConnection;
                ftpRequest.KeepAlive = false;

                // Override the validation of
                // the server certificate.
                if (ftpAdapter.UseSSLConnection && ftpAdapter.SslCertificateOverride)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidationOverride);
                else if (ftpAdapter.UseSSLConnection)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidation); 

                // Get the response stream and create the read and
                // write stream for data exchange.
                using (stream = ftpRequest.GetRequestStream())
                using (reader = new StreamReader(targetFile))
                {
                    // Read into the byte array the
                    // contents of the file to upload.
                    byte[] fileContents = Encoding.UTF8.GetBytes(reader.ReadToEnd());

                    // Write the data to the ftp server stream
                    // read from the target file stream.
                    stream.Write(fileContents, 0, fileContents.Length);

                    // Close all streams
                    stream.Close();
                    reader.Close();
                }

                // Assign the FTP response object from
                // the FTP request server.
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                // Create a new event handler for
                // the current event, send to the client.
                this.CompletionEventHandler(ftpAdapter, ftpResponse, "Uploading", targetFile,
                    Nequeo.Net.Ftp.SocketTransferDirection.Upload);

                // Call back processing invoke.
                DelegateCallbackHandler(1, "Upload Complete", "Uploading target : " + targetFile +
                    " to " + uploadUri.ToString());

                // Return true if upload was completed.
                return true;
            }
            catch (System.Exception ex)
            {
                // Create a custom error message
                // and pass the ftp objects.
                this.ExceptionHandler(ex, ftpAdapter, ftpResponse, uploadUri, "Upload", targetFile,
                    "Uploading a file to the ftp server : " + ftpAdapter.FTPServer + ".\n" + ex.Message,
                    Nequeo.Net.Ftp.SocketTransferDirection.Upload);

                // Call back processing invoke.
                DelegateCallbackHandler(-1, "Upload Error", "Uploading target : " + targetFile +
                    " to " + uploadUri.ToString());

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Close and clean up
                // all streams.
                if (stream != null)
                    stream.Close();

                if (reader != null)
                    reader.Close();

                // Get the most recent ftp status code
                // and description from the ftp server.
                if (ftpResponse != null)
                {
                    ftpStatusCode = ftpResponse.StatusCode;
                    ftpStatusDescription = ftpResponse.StatusDescription;
                    ftpResponse.Close();
                }
            }
        }

        /// <summary>
        /// Remove the directory from the server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="removeDirectoryUri">The Uri object containing a valid directory to remove to.</param>
        /// <returns>True if the remove directory was successful else false.</returns>
        private bool RemoveDirectoryEx(FTPConnectionAdapter ftpAdapter, Uri removeDirectoryUri)
        {
            // The ftp response object,
            // and ftp request object.
            FtpWebResponse ftpResponse = null;
            FtpWebRequest ftpRequest = null;

            try
            {
                // Check if the URI is valid FTP scheme.
                if (removeDirectoryUri.Scheme != Uri.UriSchemeFtp)
                    throw new ArgumentException("Uri is not a valid FTP scheme");

                // Set the ftp request object.
                ftpRequest = (FtpWebRequest)WebRequest.Create(removeDirectoryUri);

                // If a proxy server has been specified.
                if (ftpAdapter.Proxy != null)
                    ftpRequest.Proxy = ftpAdapter.Proxy;

                // If a client certificate is to be used
                // add the client certificate.
                if (ftpAdapter.ClientCertificate != null)
                    ftpRequest.ClientCertificates.Add(ftpAdapter.ClientCertificate);

                // If the ftp connection is not
                // anonymous then create a new
                // network credential.
                if (!ftpAdapter.IsAnonymousUser)
                    ftpRequest.Credentials = new NetworkCredential(
                        ftpAdapter.UserName, ftpAdapter.Password, ftpAdapter.Domain);
                else
                    ftpRequest.Credentials = new NetworkCredential("anonymous", "anonymous@anonymoususer.com");

                // Assign the remove directory file method to the
                // FTP request object. Time out 5 minutes.
                ftpRequest.Method = WebRequestMethods.Ftp.RemoveDirectory;
                ftpRequest.Timeout = ftpAdapter.TimeOut;
                ftpRequest.UsePassive = ftpAdapter.UsePassive;
                ftpRequest.UseBinary = ftpAdapter.UseBinary;
                ftpRequest.EnableSsl = ftpAdapter.UseSSLConnection;
                ftpRequest.KeepAlive = false;

                // Override the validation of
                // the server certificate.
                if (ftpAdapter.UseSSLConnection && ftpAdapter.SslCertificateOverride)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidationOverride);
                else if (ftpAdapter.UseSSLConnection)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidation); 

                // Assign the FTP response object from
                // the FTP request server.
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                // Create a new event handler for
                // the current event, send to the client.
                this.CompletionEventHandler(ftpAdapter, ftpResponse, "RemoveDirectory", string.Empty,
                    Nequeo.Net.Ftp.SocketTransferDirection.RemoveDirectory);

                // Return true if directory removal
                // was successful.
                return true;
            }
            catch (System.Exception ex)
            {
                // Create a custom error message
                // and pass the ftp objects.
                this.ExceptionHandler(ex, ftpAdapter, ftpResponse, removeDirectoryUri, "RemoveDirectory", string.Empty,
                    "Removing the directory : " + removeDirectoryUri.OriginalString + " on the ftp server : "
                    + ftpAdapter.FTPServer + ".\n" + ex.Message, Nequeo.Net.Ftp.SocketTransferDirection.RemoveDirectory);

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Get the most recent ftp status code
                // and description from the ftp server.
                if (ftpResponse != null)
                {
                    ftpStatusCode = ftpResponse.StatusCode;
                    ftpStatusDescription = ftpResponse.StatusDescription;
                    ftpResponse.Close();
                }
            }
        }

        /// <summary>
        /// Delete a file from the server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="deleteFileUri">The Uri object containing a valid file to delete to.</param>
        /// <returns>True if the delete file was successful else false.</returns>
        private bool DeleteFileEx(FTPConnectionAdapter ftpAdapter, Uri deleteFileUri)
        {
            // The ftp response object,
            // and ftp request object.
            FtpWebResponse ftpResponse = null;
            FtpWebRequest ftpRequest = null;

            try
            {
                // Check if the URI is valid FTP scheme.
                if (deleteFileUri.Scheme != Uri.UriSchemeFtp)
                    throw new ArgumentException("Uri is not a valid FTP scheme");

                // Set the ftp request object.
                ftpRequest = (FtpWebRequest)WebRequest.Create(deleteFileUri);

                // If a proxy server has been specified.
                if (ftpAdapter.Proxy != null)
                    ftpRequest.Proxy = ftpAdapter.Proxy;

                // If a client certificate is to be used
                // add the client certificate.
                if (ftpAdapter.ClientCertificate != null)
                    ftpRequest.ClientCertificates.Add(ftpAdapter.ClientCertificate);

                // If the ftp connection is not
                // anonymous then create a new
                // network credential.
                if (!ftpAdapter.IsAnonymousUser)
                    ftpRequest.Credentials = new NetworkCredential(
                        ftpAdapter.UserName, ftpAdapter.Password, ftpAdapter.Domain);
                else
                    ftpRequest.Credentials = new NetworkCredential("anonymous", "anonymous@anonymoususer.com");

                // Assign the delete file method to the
                // FTP request object. Time out 5 minutes.
                ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                ftpRequest.Timeout = ftpAdapter.TimeOut;
                ftpRequest.UsePassive = ftpAdapter.UsePassive;
                ftpRequest.UseBinary = ftpAdapter.UseBinary;
                ftpRequest.EnableSsl = ftpAdapter.UseSSLConnection;
                ftpRequest.KeepAlive = false;

                // Override the validation of
                // the server certificate.
                if (ftpAdapter.UseSSLConnection && ftpAdapter.SslCertificateOverride)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidationOverride);
                else if (ftpAdapter.UseSSLConnection)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidation); 

                // Assign the FTP response object from
                // the FTP request server.
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                // Create a new event handler for
                // the current event, send to the client.
                this.CompletionEventHandler(ftpAdapter, ftpResponse, "DeleteFile", string.Empty,
                    Nequeo.Net.Ftp.SocketTransferDirection.DeleteFile);

                // Return true if file 
                // was successful deleted.
                return true;
            }
            catch (System.Exception ex)
            {
                // Create a custom error message
                // and pass the ftp objects.
                this.ExceptionHandler(ex, ftpAdapter, ftpResponse, deleteFileUri, "DeleteFile", string.Empty,
                    "Deleting to file : " + deleteFileUri.OriginalString + " on the ftp server : "
                    + ftpAdapter.FTPServer + ".\n" + ex.Message, Nequeo.Net.Ftp.SocketTransferDirection.DeleteFile);

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Get the most recent ftp status code
                // and description from the ftp server.
                if (ftpResponse != null)
                {
                    ftpStatusCode = ftpResponse.StatusCode;
                    ftpStatusDescription = ftpResponse.StatusDescription;
                    ftpResponse.Close();
                }
            }
        }

        /// <summary>
        /// Create a new directory on the server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="makeDirectoryUri">The Uri object containing a valid directory to create to.</param>
        /// <returns>True if the make directory was successful else false.</returns>
        private bool MakeDirectoryEx(FTPConnectionAdapter ftpAdapter, Uri makeDirectoryUri)
        {
            // The ftp response object,
            // and ftp request object.
            FtpWebResponse ftpResponse = null;
            FtpWebRequest ftpRequest = null;

            try
            {
                // Check if the URI is valid FTP scheme.
                if (makeDirectoryUri.Scheme != Uri.UriSchemeFtp)
                    throw new ArgumentException("Uri is not a valid FTP scheme");

                // Set the ftp request object.
                ftpRequest = (FtpWebRequest)WebRequest.Create(makeDirectoryUri);

                // If a proxy server has been specified.
                if (ftpAdapter.Proxy != null)
                    ftpRequest.Proxy = ftpAdapter.Proxy;

                // If a client certificate is to be used
                // add the client certificate.
                if (ftpAdapter.ClientCertificate != null)
                    ftpRequest.ClientCertificates.Add(ftpAdapter.ClientCertificate);

                // If the ftp connection is not
                // anonymous then create a new
                // network credential.
                if (!ftpAdapter.IsAnonymousUser)
                    ftpRequest.Credentials = new NetworkCredential(
                        ftpAdapter.UserName, ftpAdapter.Password, ftpAdapter.Domain);
                else
                    ftpRequest.Credentials = new NetworkCredential("anonymous", "anonymous@anonymoususer.com");

                // Assign the make directory method to the
                // FTP request object. Time out 5 minutes.
                ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                ftpRequest.Timeout = ftpAdapter.TimeOut;
                ftpRequest.UsePassive = ftpAdapter.UsePassive;
                ftpRequest.UseBinary = ftpAdapter.UseBinary;
                ftpRequest.EnableSsl = ftpAdapter.UseSSLConnection;
                ftpRequest.KeepAlive = false;

                // Override the validation of
                // the server certificate.
                if (ftpAdapter.UseSSLConnection && ftpAdapter.SslCertificateOverride)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidationOverride);
                else if (ftpAdapter.UseSSLConnection)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidation);  

                // Assign the FTP response object from
                // the FTP request server.
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                // Create a new event handler for
                // the current event, send to the client.
                this.CompletionEventHandler(ftpAdapter, ftpResponse, "MakeDirectory", string.Empty,
                    Nequeo.Net.Ftp.SocketTransferDirection.MakeDirectory);

                // Return true if directory 
                // was successful created.
                return true;
            }
            catch (System.Exception ex)
            {
                // Create a custom error message
                // and pass the ftp objects.
                this.ExceptionHandler(ex, ftpAdapter, ftpResponse, makeDirectoryUri, "MakeDirectory", string.Empty,
                    "Created directory : " + makeDirectoryUri.OriginalString + " on the ftp server : "
                    + ftpAdapter.FTPServer + ".\n" + ex.Message, Nequeo.Net.Ftp.SocketTransferDirection.MakeDirectory);

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Get the most recent ftp status code
                // and description from the ftp server.
                if (ftpResponse != null)
                {
                    ftpStatusCode = ftpResponse.StatusCode;
                    ftpStatusDescription = ftpResponse.StatusDescription;
                    ftpResponse.Close();
                }
            }
        }

        /// <summary>
        /// Rename the file on the server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="newFileName">The name to change to server file to.</param>
        /// <param name="renameFileUri">The Uri object containing a valid file to rename to.</param>
        /// <returns>True if the rename file was successful else false.</returns>
        private bool RenameFileEx(FTPConnectionAdapter ftpAdapter,
            string newFileName, Uri renameFileUri)
        {
            // The ftp response object,
            // and ftp request object.
            FtpWebResponse ftpResponse = null;
            FtpWebRequest ftpRequest = null;

            try
            {
                // Check if the URI is valid FTP scheme.
                if (renameFileUri.Scheme != Uri.UriSchemeFtp)
                    throw new ArgumentException("Uri is not a valid FTP scheme");

                // Set the ftp request object.
                ftpRequest = (FtpWebRequest)WebRequest.Create(renameFileUri);

                // If a proxy server has been specified.
                if (ftpAdapter.Proxy != null)
                    ftpRequest.Proxy = ftpAdapter.Proxy;

                // If a client certificate is to be used
                // add the client certificate.
                if (ftpAdapter.ClientCertificate != null)
                    ftpRequest.ClientCertificates.Add(ftpAdapter.ClientCertificate);

                // If the ftp connection is not
                // anonymous then create a new
                // network credential.
                if (!ftpAdapter.IsAnonymousUser)
                    ftpRequest.Credentials = new NetworkCredential(
                        ftpAdapter.UserName, ftpAdapter.Password, ftpAdapter.Domain);
                else
                    ftpRequest.Credentials = new NetworkCredential("anonymous", "anonymous@anonymoususer.com");

                // Assign the rename file method to the
                // FTP request object. Time out 5 minutes.
                ftpRequest.Method = WebRequestMethods.Ftp.Rename;
                ftpRequest.Timeout = ftpAdapter.TimeOut;
                ftpRequest.UsePassive = ftpAdapter.UsePassive;
                ftpRequest.UseBinary = ftpAdapter.UseBinary;
                ftpRequest.RenameTo = newFileName;
                ftpRequest.EnableSsl = ftpAdapter.UseSSLConnection;
                ftpRequest.KeepAlive = false;

                // Override the validation of
                // the server certificate.
                if (ftpAdapter.UseSSLConnection && ftpAdapter.SslCertificateOverride)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidationOverride);
                else if (ftpAdapter.UseSSLConnection)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidation); 

                // Assign the FTP response object from
                // the FTP request server.
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                // Create a new event handler for
                // the current event, send to the client.
                this.CompletionEventHandler(ftpAdapter, ftpResponse, "RenameFile", newFileName,
                    Nequeo.Net.Ftp.SocketTransferDirection.RenameFile);

                // Return true if file 
                // was successful renamed.
                return true;
            }
            catch (System.Exception ex)
            {
                // Create a custom error message
                // and pass the ftp objects.
                this.ExceptionHandler(ex, ftpAdapter, ftpResponse, renameFileUri, "RenameFile", newFileName,
                    "Renamed file : " + renameFileUri.OriginalString
                    + " to " + newFileName + " on the ftp server : "
                    + ftpAdapter.FTPServer + ".\n" + ex.Message,
                    Nequeo.Net.Ftp.SocketTransferDirection.RenameFile);

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Get the most recent ftp status code
                // and description from the ftp server.
                if (ftpResponse != null)
                {
                    ftpStatusCode = ftpResponse.StatusCode;
                    ftpStatusDescription = ftpResponse.StatusDescription;
                    ftpResponse.Close();
                }
            }
        }

        /// <summary>
        /// A directory list of all files on the specified server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="directoryListUri">The Uri object containing a valid directory to list.</param>
        /// <param name="fileList">The list of files within the directory.</param>
        /// <returns>True if the download was successful else false.</returns>
        private bool DirectoryListEx(FTPConnectionAdapter ftpAdapter,
            Uri directoryListUri, out List<string> fileList)
        {
            // The ftp response object,
            // and ftp request object.
            FtpWebResponse ftpResponse = null;
            FtpWebRequest ftpRequest = null;

            // The read and write stream objects
            // for the transfer of data.
            Stream stream = null;
            StreamReader reader = null;

            // Initially assign the list;
            fileList = null;

            // The string that will contain
            // the list of files.
            string listDirectory = string.Empty;

            try
            {
                // Check if the URI is valid FTP scheme.
                if (directoryListUri.Scheme != Uri.UriSchemeFtp)
                    throw new ArgumentException("Uri is not a valid FTP scheme");

                // Set the ftp request object.
                ftpRequest = (FtpWebRequest)WebRequest.Create(directoryListUri);

                // If a proxy server has been specified.
                if (ftpAdapter.Proxy != null)
                    ftpRequest.Proxy = ftpAdapter.Proxy;

                // If a client certificate is to be used
                // add the client certificate.
                if (ftpAdapter.ClientCertificate != null)
                    ftpRequest.ClientCertificates.Add(ftpAdapter.ClientCertificate);

                // If the ftp connection is not
                // anonymous then create a new
                // network credential.
                if (!ftpAdapter.IsAnonymousUser)
                    ftpRequest.Credentials = new NetworkCredential(
                        ftpAdapter.UserName, ftpAdapter.Password, ftpAdapter.Domain);
                else
                    ftpRequest.Credentials = new NetworkCredential("anonymous", "anonymous@anonymoususer.com");

                // Assign the download file method to the
                // FTP request object. Time out 5 minutes.
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                ftpRequest.Timeout = ftpAdapter.TimeOut;
                ftpRequest.UsePassive = ftpAdapter.UsePassive;
                ftpRequest.UseBinary = ftpAdapter.UseBinary;
                ftpRequest.EnableSsl = ftpAdapter.UseSSLConnection;
                ftpRequest.KeepAlive = false;

                // Override the validation of
                // the server certificate.
                if (ftpAdapter.UseSSLConnection && ftpAdapter.SslCertificateOverride)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidationOverride);
                else if (ftpAdapter.UseSSLConnection)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidation); 

                // Assign the FTP response object from
                // the FTP request server.
                ftpResponse = (FtpWebResponse)ftpRequest.GetResponse();

                // Get the response stream and create the read and
                // write stream for data exchange.
                using (stream = ftpResponse.GetResponseStream())
                using (reader = new StreamReader(stream, Encoding.UTF8))
                {
                    // Write the data to the destination file
                    // read from the FTP server stream.
                    listDirectory = reader.ReadToEnd();

                    // Search for null characters '\r\n', '\n'.
                    // Split with current delimeter the data returned
                    // from the server. Create the new file list.
                    char[] delimeter = new char[] { '\r' };
                    string[] splitList = listDirectory.Split(delimeter, 
                        StringSplitOptions.RemoveEmptyEntries);

                    fileList = new List<string>();

                    // For each file in the split list
                    // add the file into the list array.
                    foreach (string file in splitList)
                    {
                        if (!String.IsNullOrEmpty(file.Trim()))
                            fileList.Add(file.Trim());
                    }

                    // Close all streams.
                    stream.Close();
                    reader.Close();
                }

                // Create a new event handler for
                // the current event, send to the client.
                this.CompletionEventHandler(ftpAdapter, ftpResponse, "DirectoryList", listDirectory,
                    Nequeo.Net.Ftp.SocketTransferDirection.DirectoryList);

                // Call back processing invoke.
                DelegateCallbackHandler(6, "Completed Directory List From : " + directoryListUri.ToString(),
                    listDirectory);

                // Return true if download was completed.
                return true;
            }
            catch (System.Exception ex)
            {
                // Create a custom error message
                // and pass the ftp objects.
                this.ExceptionHandler(ex, ftpAdapter, ftpResponse, directoryListUri, "DirectoryList", listDirectory,
                    "Getting directory list from the ftp server : " + ftpAdapter.FTPServer + ".\n" + ex.Message,
                    Nequeo.Net.Ftp.SocketTransferDirection.DirectoryList);

                // Call back processing invoke.
                DelegateCallbackHandler(-1, "Error Directory List From : " + directoryListUri.ToString(),
                    listDirectory);

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Close and clean up
                // all streams.
                if (stream != null)
                    stream.Close();

                if (reader != null)
                    reader.Close();

                // Get the most recent ftp status code
                // and description from the ftp server.
                if (ftpResponse != null)
                {
                    ftpStatusCode = ftpResponse.StatusCode;
                    ftpStatusDescription = ftpResponse.StatusDescription;
                    ftpResponse.Close();
                }
            }
        }
        #endregion

        #region Private Error Handler Methods
        /// <summary>
        /// General exception handler for the class methods.
        /// </summary>
        /// <param name="e">The system exception object containing the current exception stack.</param>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="ftpResponse">The current ftp response object containing the server
        /// response elements.</param>
        /// <param name="uri">The Uri object containing a valid ftp uri scheme.</param>
        /// <param name="ftpMethod">The current ftp process method.</param>
        /// <param name="fileInfo">The original upload target or download destination file.</param>
        /// <param name="ftpCustomError">The custom error message for the specified method.</param>
        /// <param name="eventIndex">The event index to create.</param>
        private void ExceptionHandler(System.Exception e, FTPConnectionAdapter ftpAdapter,
            FtpWebResponse ftpResponse, Uri uri, string ftpMethod, string fileInfo,
            string ftpCustomError, Nequeo.Net.Ftp.SocketTransferDirection eventIndex)
        {
            // Detect a thread abort exception.
            if (e is ThreadAbortException)
                Thread.ResetAbort();

            // Set the response values to null if
            // no object created else assign the
            // response values.
            if (ftpResponse != null)
            {
                // Assign the cuurent request information.
                ftpStatusCode = ftpResponse.StatusCode;
                ftpStatusDescription = ftpResponse.StatusDescription;
            }
            else
            {
                // Assign the default request information.
                ftpStatusCode = FtpStatusCode.Undefined;
                ftpStatusDescription = string.Empty;
            }

            // Make sure than a receiver instance of the
            // event delegate handler was created.
            if (OnFTPError != null)
                OnFTPError(this, new FTPSocketErrorArgs(ftpCustomError, ftpAdapter.FTPServer,
                    ftpMethod, fileInfo, ftpStatusCode, ftpStatusDescription, eventIndex));

            //// Write the error to the specified location.
            //StackTrace st = new StackTrace(e, true);
            //this.Write("FTPWebSocket", ftpMethod, ftpCustomError, st.GetFrame(0).GetFileLineNumber(),
            //    Enumeration.EnumHandler.WriteTo.FileAndEventLog,
            //    Enumeration.EnumHandler.LogType.Error);
        }
        #endregion

        #region Private Event Handler Methods
        /// <summary>
        /// The general event completion handler.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="ftpResponse">The current ftp response object containing the server
        /// response elements.</param>
        /// <param name="ftpMethod">The current ftp process method.</param>
        /// <param name="fileInfo">The current upload target download destination.</param>
        /// <param name="eventIndex">The event index to create.</param>
        private void CompletionEventHandler(FTPConnectionAdapter ftpAdapter,
            FtpWebResponse ftpResponse, string ftpMethod, string fileInfo,
            Nequeo.Net.Ftp.SocketTransferDirection eventIndex)
        {
            switch (eventIndex)
            {
                case Nequeo.Net.Ftp.SocketTransferDirection.Download:
                    // Create a new download complete event
                    // for the current client.
                    if (OnDownloadComplete != null)
                        OnDownloadComplete(this, new FTPSocketArgs(ftpAdapter.FTPServer, ftpMethod,
                            fileInfo, ftpResponse.StatusCode, ftpResponse.StatusDescription, eventIndex));
                    break;
                case Nequeo.Net.Ftp.SocketTransferDirection.Upload:
                    // Create a new upload complete event
                    // for the current client.
                    if (OnUploadComplete != null)
                        OnUploadComplete(this, new FTPSocketArgs(ftpAdapter.FTPServer, ftpMethod,
                            fileInfo, ftpResponse.StatusCode, ftpResponse.StatusDescription, eventIndex));
                    break;
                case Nequeo.Net.Ftp.SocketTransferDirection.RemoveDirectory:
                    // Create a new removing directory complete event
                    // for the current client.
                    if (OnDirectoryRemovedComplete != null)
                        OnDirectoryRemovedComplete(this, new FTPSocketArgs(ftpAdapter.FTPServer, ftpMethod,
                            fileInfo, ftpResponse.StatusCode, ftpResponse.StatusDescription, eventIndex));
                    break;
                case Nequeo.Net.Ftp.SocketTransferDirection.DeleteFile:
                    // Create a new delete file complete event
                    // for the current client.
                    if (OnDeleteFileComplete != null)
                        OnDeleteFileComplete(this, new FTPSocketArgs(ftpAdapter.FTPServer, ftpMethod,
                            fileInfo, ftpResponse.StatusCode, ftpResponse.StatusDescription, eventIndex));
                    break;
                case Nequeo.Net.Ftp.SocketTransferDirection.MakeDirectory:
                    // Create a new make directory complete event
                    // for the current client.
                    if (OnMakeDirectoryComplete != null)
                        OnMakeDirectoryComplete(this, new FTPSocketArgs(ftpAdapter.FTPServer, ftpMethod,
                            fileInfo, ftpResponse.StatusCode, ftpResponse.StatusDescription, eventIndex));
                    break;
                case Nequeo.Net.Ftp.SocketTransferDirection.RenameFile:
                    // Create a new rename file complete event
                    // for the current client.
                    if (OnRenameFileComplete != null)
                        OnRenameFileComplete(this, new FTPSocketArgs(ftpAdapter.FTPServer, ftpMethod,
                            fileInfo, ftpResponse.StatusCode, ftpResponse.StatusDescription, eventIndex));
                    break;
                case Nequeo.Net.Ftp.SocketTransferDirection.DirectoryList:
                    // Create a new directory list complete event
                    // for the current client.
                    if (OnDirectoryListComplete != null)
                        OnDirectoryListComplete(this, new FTPSocketArgs(ftpAdapter.FTPServer, ftpMethod,
                            fileInfo, ftpResponse.StatusCode, ftpResponse.StatusDescription, eventIndex));
                    break;
                case Nequeo.Net.Ftp.SocketTransferDirection.FileSize:
                    // Create a new directory list complete event
                    // for the current client.
                    if (OnFileSizeComplete != null)
                        OnFileSizeComplete(this, new FTPSocketArgs(ftpAdapter.FTPServer, ftpMethod,
                            fileInfo, ftpResponse.StatusCode, ftpResponse.StatusDescription, eventIndex));
                    break;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Download a file from the specified server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="destinationFile">The full path and file name where data should be written to.</param>
        /// <param name="downloadUri">The Uri object containing a valid file to download.</param>
        /// <returns>True if the download was successful else false.</returns>
        public virtual bool Download(FTPConnectionAdapter ftpAdapter,
            string destinationFile, Uri downloadUri)
        {
            return this.DownloadEx(ftpAdapter, destinationFile, downloadUri);
        }

        /// <summary>
        /// Upload a file to the specified server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="targetFile">The full path and file name of the file to upload.</param>
        /// <param name="uploadUri">The Uri object containing a valid file to upload to.</param>
        /// <returns>True if the upload was successful else false.</returns>
        public virtual bool Upload(FTPConnectionAdapter ftpAdapter,
            string targetFile, Uri uploadUri)
        {
            return this.UploadEx(ftpAdapter, targetFile, uploadUri);
        }

        /// <summary>
        /// Remove the directory from the server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="removeDirectoryUri">The Uri object containing a valid directory to remove to.</param>
        /// <returns>True if the remove directory was successful else false.</returns>
        public virtual bool RemoveDirectory(FTPConnectionAdapter ftpAdapter, Uri removeDirectoryUri)
        {
            return this.RemoveDirectoryEx(ftpAdapter, removeDirectoryUri);
        }

        /// <summary>
        /// Delete a file from the server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="deleteFileUri">The Uri object containing a valid file to delete to.</param>
        /// <returns>True if the delete file was successful else false.</returns>
        public virtual bool DeleteFile(FTPConnectionAdapter ftpAdapter, Uri deleteFileUri)
        {
            return this.DeleteFileEx(ftpAdapter, deleteFileUri);
        }

        /// <summary>
        /// Create a new directory on the server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="makeDirectoryUri">The Uri object containing a valid directory to create to.</param>
        /// <returns>True if the make directory was successful else false.</returns>
        public virtual bool MakeDirectory(FTPConnectionAdapter ftpAdapter, Uri makeDirectoryUri)
        {
            return this.MakeDirectoryEx(ftpAdapter, makeDirectoryUri);
        }

        /// <summary>
        /// Rename the file on the server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="newFileName">The name to change to server file to.</param>
        /// <param name="renameFileUri">The Uri object containing a valid file to rename to.</param>
        /// <returns>True if the rename file was successful else false.</returns>
        public virtual bool RenameFile(FTPConnectionAdapter ftpAdapter,
            string newFileName, Uri renameFileUri)
        {
            return this.RenameFileEx(ftpAdapter, newFileName, renameFileUri);
        }

        /// <summary>
        /// A directory list of all files on the specified server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="directoryListUri">The Uri object containing a valid directory to list.</param>
        /// <param name="fileList">The list of files within the directory.</param>
        /// <returns>True if the download was successful else false.</returns>
        public virtual bool DirectoryList(FTPConnectionAdapter ftpAdapter,
            Uri directoryListUri, out List<string> fileList)
        {
            return this.DirectoryListEx(ftpAdapter, directoryListUri, out fileList);
        }

        /// <summary>
        /// File size from the specified server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="remoteFileUri">The Uri object containing a valid file on the remote host.</param>
        /// <param name="remoteFileSize">The size of the file on the remote host.</param>
        /// <returns>True if the file size return was successful else false.</returns>
        public virtual bool FileSize(FTPConnectionAdapter ftpAdapter,
            Uri remoteFileUri, out long remoteFileSize)
        {
            return this.FileSizeEx(ftpAdapter, remoteFileUri, out remoteFileSize);
        }
        #endregion
    }

    /// <summary>
    /// A complete FTP integration socket to an FTP server,
    /// contains methods to upload data to and
    /// from the specified FTP sever asynchronously.
    /// </summary>
    public class AsynchronousFTPSocket : Nequeo.Runtime.DisposableBase, IDisposable
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public AsynchronousFTPSocket()
        {
        }

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        /// <param name="callbackHandler">Processing callback delegate.</param>
        public AsynchronousFTPSocket(FTPWebSocketCallbackHandler callbackHandler)
        {
            this.callbackHandler = callbackHandler;
        }
        #endregion

        #region Private Fields
        private FtpStatusCode ftpStatusCode = FtpStatusCode.Undefined;
        private string ftpStatusDescription = string.Empty;
        private FTPWebSocketCallbackHandler callbackHandler = null;
        private Nequeo.Security.X509Certificate2Info _sslCertificate = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get, the most recent ftp status code from the server.
        /// </summary>
        public FtpStatusCode FtpStatusCode
        {
            get { return ftpStatusCode; }
        }

        /// <summary>
        /// Get, the most recent ftp status description from the server.
        /// </summary>
        public string FtpStatusDescription
        {
            get { return ftpStatusDescription; }
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Get, the secure certificate.
        /// </summary>
        public Nequeo.Security.X509Certificate2Info Certificate
        {
            get { return _sslCertificate; }
        }
        #endregion

        #region Public Events
        /// <summary>
        /// This event occurs when an error occurs 
        /// attempting to communicate with the ftp server.
        /// </summary>
        public event FTPSocketErrorHandler OnFTPError;

        /// <summary>
        /// This event occurs when upload
        /// has completed through the ftp server.
        /// </summary>
        public event FTPSocketHandler OnUploadComplete;

        /// <summary>
        /// This event occurs when upload
        /// has completed through the ftp server.
        /// </summary>
        public event FTPSocketHandler OnDownloadComplete;
        #endregion

        #region Private Methods
        /// <summary>
        /// This method will invoke the callback handler delegate.
        /// </summary>
        /// <param name="processIndex">The unique processing index.</param>
        /// <param name="processingType">The current processing type.</param>
        /// <param name="processInformation">The processing message.</param>
        private void DelegateCallbackHandler(int processIndex,
            string processingType, string processInformation)
        {
            // Invoke the delegate and send the message
            // to the calling thread.
            if (callbackHandler != null)
                callbackHandler(processIndex, processingType, processInformation);
        }

        /// <summary>
        /// Certificate validator.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="certificate">The certificate</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The policy error.</param>
        /// <returns>True if the certificate is valid else false.</returns>
        private bool OnCertificateValidation(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Create a new instance of the x509 certificate 
            // information class.
            _sslCertificate = new Nequeo.Security.X509Certificate2Info(
                certificate as X509Certificate2, chain, sslPolicyErrors);

            // Get the current error level.
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable)
                return false;
            else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
                return false;
            else
            {
                //Remote Certificate Name Mismatch
                System.Security.Policy.Zone z =
                    System.Security.Policy.Zone.CreateFromUrl(((HttpWebRequest)sender).RequestUri.ToString());

                // Get the security zone for
                // the current request URI.
                if (z.SecurityZone == System.Security.SecurityZone.Intranet ||
                    z.SecurityZone == System.Security.SecurityZone.MyComputer ||
                    z.SecurityZone == System.Security.SecurityZone.NoZone ||
                    z.SecurityZone == System.Security.SecurityZone.Trusted)
                    return true;

                // Return false otherwise.
                return false;
            }
        }

        /// <summary>
        /// Certificate override validator.
        /// </summary>
        /// <param name="sender">The current sender.</param>
        /// <param name="certificate">The certificate</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The policy error.</param>
        /// <returns>True if the certificate is valid else false.</returns>
        private bool OnCertificateValidationOverride(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Create a new instance of the x509 certificate 
            // information class.
            _sslCertificate = new Nequeo.Security.X509Certificate2Info(
                certificate as X509Certificate2, chain, sslPolicyErrors);

            return true;
        }
        #endregion

        #region Private Asynchronous Upload Methods
        /// <summary>
        /// Upload a file to the specified server asynchronous.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="targetFile">The full path and file name of the file to upload.</param>
        /// <param name="uploadUri">The Uri object containing a valid file to upload to.</param>
        /// <returns>True if the upload was successful else false.</returns>
        private bool AsynchronousUploadEx(FTPConnectionAdapter ftpAdapter,
            string targetFile, Uri uploadUri)
        {
            // Notifies one or more waiting threads that an
            // event has occured. Set the request object
            ManualResetEvent waitObject = null;
            FtpWebRequest ftpRequest = null;
            FTPStateAdapter state = null;

            try
            {
                // Check if the URI is valid FTP scheme.
                if (uploadUri.Scheme != Uri.UriSchemeFtp)
                    throw new ArgumentException("Uri is not a valid FTP scheme");

                // Set the ftp request object, and
                // the ftp state adapter.
                state = new FTPStateAdapter();
                ftpRequest = (FtpWebRequest)WebRequest.Create(uploadUri);

                // If a proxy server has been specified.
                if (ftpAdapter.Proxy != null)
                    ftpRequest.Proxy = ftpAdapter.Proxy;

                // If a client certificate is to be used
                // add the client certificate.
                if (ftpAdapter.ClientCertificate != null)
                    ftpRequest.ClientCertificates.Add(ftpAdapter.ClientCertificate);

                // If the ftp connection is not
                // anonymous then create a new
                // network credential.
                if (!ftpAdapter.IsAnonymousUser)
                    ftpRequest.Credentials = new NetworkCredential(
                        ftpAdapter.UserName, ftpAdapter.Password, ftpAdapter.Domain);
                else
                    ftpRequest.Credentials = new NetworkCredential("anonymous", "anonymous@anonymoususer.com");

                // Assign the rename file method to the
                // FTP request object. Time out 5 minutes.
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.Timeout = ftpAdapter.TimeOut;
                ftpRequest.UsePassive = ftpAdapter.UsePassive;
                ftpRequest.UseBinary = ftpAdapter.UseBinary;
                ftpRequest.EnableSsl = ftpAdapter.UseSSLConnection;
                ftpRequest.KeepAlive = false;

                // Override the validation of
                // the server certificate.
                if (ftpAdapter.UseSSLConnection && ftpAdapter.SslCertificateOverride)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidationOverride);
                else if (ftpAdapter.UseSSLConnection)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidation);  

                // Store the request in the object that we 
                // pass into the asynchronous operations.
                state.FtpRequest = ftpRequest;
                state.TargetFile = targetFile;

                // Get the event to wait on.
                waitObject = state.UploadComplete;

                // Asynchronously get the stream for the file contents.
                ftpRequest.BeginGetRequestStream(
                    new AsyncCallback(UploadEndGetStreamCallback), state);

                // Block the current thread until all 
                // operations are complete.
                waitObject.WaitOne();

                // The operations either completed or threw an exception.
                if (state.OperationException != null)
                    throw state.OperationException;

                // Create a new event handler for
                // the current event, send to the client.
                this.CompletionEventHandler(ftpAdapter, state, "AsynchronousUpload", targetFile,
                    Nequeo.Net.Ftp.SocketTransferDirection.Upload);

                // Call back processing invoke.
                DelegateCallbackHandler(1, "Asynchronous Upload Complete", "Uploading target : " + targetFile +
                    " to " + uploadUri.ToString());

                // Return true if upload was completed.
                return true;
            }
            catch (System.Exception ex)
            {
                // Create a custom error message
                // and pass the ftp objects.
                this.ExceptionHandler(ex, ftpAdapter, state, uploadUri, "AsynchronousUpload", targetFile,
                    "Uploading a file to the ftp server : " + ftpAdapter.FTPServer + ".\n" + ex.Message,
                    Nequeo.Net.Ftp.SocketTransferDirection.Upload);

                // Call back processing invoke.
                DelegateCallbackHandler(-1, "Asynchronous Upload Error", "Uploading target : " + targetFile +
                    " to " + uploadUri.ToString());

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Get the most recent ftp status code
                // and description from the state object.
                if (state != null)
                {
                    ftpStatusCode = state.FtpStatusCode;
                    ftpStatusDescription = state.FtpStatusDescription;
                }
            }
        }

        /// <summary>
        /// The end of the asynchronous request stream callback.
        /// </summary>
        /// <param name="ar">The status of the asynchronous operation.</param>
        private void UploadEndGetStreamCallback(IAsyncResult ar)
        {
            // Get the state adapter from the current
            // async result callback object.
            FTPStateAdapter state = (FTPStateAdapter)ar.AsyncState;
            Stream requestStream = null;
            FileStream fileStream = null;

            try
            {
                // End the asynchronous call to get the request stream.
                requestStream = state.FtpRequest.EndGetRequestStream(ar);

                // Assign the reading buffer size
                // and the buffer array.
                int readBytes = 0;
                const int bufferLength = 2048;
                byte[] buffer = new byte[bufferLength];

                // Create a new instance of the file stream.
                using (fileStream = new FileStream(state.TargetFile, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite))
                {
                    // Read each byte from the target file
                    // and write the bytes to the ftp request
                    // stream to the ftp server.
                    do
                    {
                        readBytes = fileStream.Read(buffer, 0, bufferLength);
                        requestStream.Write(buffer, 0, readBytes);
                    }
                    while (readBytes != 0);

                    // Close the file stream.
                    fileStream.Close();
                }

                // Close the request stream before sending the request.
                requestStream.Close();

                // Asynchronously get the response to the upload request.
                state.FtpRequest.BeginGetResponse(
                    new AsyncCallback(UploadEndGetResponseCallback), state);
            }
            catch (System.Exception e)
            {
                // Set the current exception that has occured
                // and notify the lop level thread that the
                // upload has completed. (Error in this case).
                state.OperationException = e;
                state.UploadComplete.Set();
            }
            finally
            {
                // Close and clean up
                // all streams.
                if (fileStream != null)
                    fileStream.Close();

                if (requestStream != null)
                    requestStream.Close();
            }
        }

        /// <summary>
        /// The end of the asynchronous response callback.
        /// </summary>
        /// <param name="ar">The status of the asynchronous operation.</param>
        private void UploadEndGetResponseCallback(IAsyncResult ar)
        {
            // Get the state adapter from the current
            // async result callback object.
            FTPStateAdapter state = (FTPStateAdapter)ar.AsyncState;
            FtpWebResponse ftpResponse = null;

            try
            {
                // Get the async end response callback
                // object from the request object.
                ftpResponse = (FtpWebResponse)state.FtpRequest.EndGetResponse(ar);

                // Signal the main application thread that 
                // the upload has completed.
                state.UploadComplete.Set();
            }
            catch (System.Exception e)
            {
                // Set the current exception that has occured
                // and notify the lop level thread that the
                // upload has completed. (Error in this case).
                state.OperationException = e;
                state.UploadComplete.Set();
            }
            finally
            {
                // Get the most recent ftp status code
                // and description from the ftp server.
                if (ftpResponse != null)
                {
                    state.FtpStatusCode = ftpResponse.StatusCode;
                    state.FtpStatusDescription = ftpResponse.StatusDescription;
                    ftpResponse.Close();
                }
            }
        }
        #endregion

        #region Private Asynchronous Download Methods
        /// <summary>
        /// Download a file from the specified server asynchronous.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="destinationFile">The full path and file name where data should be written to.</param>
        /// <param name="downloadUri">The Uri object containing a valid file to download.</param>
        /// <returns>True if the download was successful else false.</returns>
        private bool AsynchronousDownloadEx(FTPConnectionAdapter ftpAdapter,
            string destinationFile, Uri downloadUri)
        {
            // Notifies one or more waiting threads that an
            // event has occured. Set the request object
            ManualResetEvent waitObject = null;
            FtpWebRequest ftpRequest = null;
            FTPStateAdapter state = null;

            try
            {
                // Check if the URI is valid FTP scheme.
                if (downloadUri.Scheme != Uri.UriSchemeFtp)
                    throw new ArgumentException("Uri is not a valid FTP scheme");

                // Extract the directory path.
                string sFolderPath = System.IO.Path.GetDirectoryName(destinationFile);

                // If the directory does not exists create it.
                if (!Directory.Exists(sFolderPath))
                    Directory.CreateDirectory(sFolderPath);

                // Set the ftp request object, and
                // the ftp state adapter.
                state = new FTPStateAdapter();
                ftpRequest = (FtpWebRequest)WebRequest.Create(downloadUri);

                // If a proxy server has been specified.
                if (ftpAdapter.Proxy != null)
                    ftpRequest.Proxy = ftpAdapter.Proxy;

                // If a client certificate is to be used
                // add the client certificate.
                if (ftpAdapter.ClientCertificate != null)
                    ftpRequest.ClientCertificates.Add(ftpAdapter.ClientCertificate);

                // If the ftp connection is not
                // anonymous then create a new
                // network credential.
                if (!ftpAdapter.IsAnonymousUser)
                    ftpRequest.Credentials = new NetworkCredential(
                        ftpAdapter.UserName, ftpAdapter.Password, ftpAdapter.Domain);
                else
                    ftpRequest.Credentials = new NetworkCredential("anonymous", "anonymous@anonymoususer.com");

                // Assign the rename file method to the
                // FTP request object. Time out 5 minutes.
                ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpRequest.Timeout = ftpAdapter.TimeOut;
                ftpRequest.UsePassive = ftpAdapter.UsePassive;
                ftpRequest.UseBinary = ftpAdapter.UseBinary;
                ftpRequest.EnableSsl = ftpAdapter.UseSSLConnection;
                ftpRequest.KeepAlive = false;

                // Override the validation of
                // the server certificate.
                if (ftpAdapter.UseSSLConnection && ftpAdapter.SslCertificateOverride)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidationOverride);
                else if (ftpAdapter.UseSSLConnection)
                    ServicePointManager.ServerCertificateValidationCallback = new
                        RemoteCertificateValidationCallback(OnCertificateValidation);  

                // Store the request in the object that we 
                // pass into the asynchronous operations.
                state.FtpRequest = ftpRequest;
                state.DestinationFile = destinationFile;

                // Get the event to wait on.
                waitObject = state.DownloadComplete;

                // Asynchronously get the stream for the file contents.
                ftpRequest.BeginGetResponse(
                    new AsyncCallback(DownloadEndGetResponseCallback), state);

                // Block the current thread until all 
                // operations are complete.
                waitObject.WaitOne();

                // The operations either completed or threw an exception.
                if (state.OperationException != null)
                    throw state.OperationException;

                // Create a new event handler for
                // the current event, send to the client.
                this.CompletionEventHandler(ftpAdapter, state, "AsynchronousDownload", destinationFile,
                    Nequeo.Net.Ftp.SocketTransferDirection.Download);

                // Call back processing invoke.
                DelegateCallbackHandler(0, "Asynchronous Download Complete", "Downloading : " + downloadUri.ToString() +
                    " to destination : " + destinationFile);

                // Return true if upload was completed.
                return true;
            }
            catch (System.Exception ex)
            {
                // Create a custom error message
                // and pass the ftp objects.
                this.ExceptionHandler(ex, ftpAdapter, state, downloadUri, "AsynchronousDownload", destinationFile,
                    "Downloading a file from the ftp server : " + ftpAdapter.FTPServer + ".\n" + ex.Message,
                    Nequeo.Net.Ftp.SocketTransferDirection.Download);

                // Call back processing invoke.
                DelegateCallbackHandler(-1, "Asynchronous Download Error", "Downloading : " + downloadUri.ToString() +
                    " to destination : " + destinationFile);

                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
            finally
            {
                // Get the most recent ftp status code
                // and description from the state object.
                if (state != null)
                {
                    ftpStatusCode = state.FtpStatusCode;
                    ftpStatusDescription = state.FtpStatusDescription;
                }
            }
        }

        /// <summary>
        /// The end of the asynchronous response callback.
        /// </summary>
        /// <param name="ar">The status of the asynchronous operation.</param>
        private void DownloadEndGetResponseCallback(IAsyncResult ar)
        {
            // Get the state adapter from the current
            // async result callback object.
            FTPStateAdapter state = (FTPStateAdapter)ar.AsyncState;
            FtpWebResponse ftpResponse = null;

            try
            {
                // Get the async end response callback
                // object from the request object.
                ftpResponse = (FtpWebResponse)state.FtpRequest.EndGetResponse(ar);

                // Assign the response object.
                state.FtpResponse = ftpResponse;

                // Asynchronously get the response to the download request.
                DownloadGetResponseStream(state);
            }
            catch (System.Exception e)
            {
                // Set the current exception that has occured
                // and notify the lop level thread that the
                // download has completed. (Error in this case).
                state.OperationException = e;
                state.DownloadComplete.Set();
            }
            finally
            {
                // Get the most recent ftp status code
                // and description from the ftp server.
                if (ftpResponse != null)
                {
                    state.FtpStatusCode = ftpResponse.StatusCode;
                    state.FtpStatusDescription = ftpResponse.StatusDescription;
                    ftpResponse.Close();
                }
            }
        }

        /// <summary>
        /// The asynchronous response stream.
        /// </summary>
        /// <param name="state">The status of the asynchronous operation.</param>
        private void DownloadGetResponseStream(FTPStateAdapter state)
        {
            // Get the state adapter from the current
            // async result callback object.
            Stream responseStream = null;
            FileStream fileStreamWriter = null;

            try
            {
                // End the asynchronous call to get the response stream.
                responseStream = state.FtpResponse.GetResponseStream();

                // Assign the reading buffer size
                // and the buffer array.
                int readBytes = 0;
                const int bufferLength = 2048;
                byte[] buffer = new byte[bufferLength];

                // Create a new instance of the file stream.
                using (fileStreamWriter = new FileStream(state.DestinationFile, FileMode.Create,
                    FileAccess.Write, FileShare.ReadWrite))
                {
                    // Read each byte from the target file
                    // and write the bytes to the ftp request
                    // stream to the ftp server.
                    do
                    {
                        readBytes = responseStream.Read(buffer, 0, bufferLength);
                        fileStreamWriter.Write(buffer, 0, readBytes);
                    }
                    while (readBytes != 0);

                    // Close the file stream.
                    fileStreamWriter.Close();
                }

                // Close the request stream before sending the request.
                responseStream.Close();

                // Signal the main application thread that 
                // the download has completed.
                state.DownloadComplete.Set();
            }
            catch (System.Exception e)
            {
                // Set the current exception that has occured
                // and notify the lop level thread that the
                // download has completed. (Error in this case).
                state.OperationException = e;
                state.DownloadComplete.Set();
            }
            finally
            {
                // Close and clean up
                // all streams.
                if (fileStreamWriter != null)
                    fileStreamWriter.Close();

                if (responseStream != null)
                    responseStream.Close();
            }
        }
        #endregion

        #region Private Error Handler Methods
        /// <summary>
        /// General exception handler for the class methods.
        /// </summary>
        /// <param name="e">The system exception object containing the current exception stack.</param>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="state">The current ftp state object containing ftp state information.</param>
        /// <param name="uri">The Uri object containing a valid ftp uri scheme.</param>
        /// <param name="ftpMethod">The current ftp process method.</param>
        /// <param name="fileInfo">The current upload target download destination.</param>
        /// <param name="ftpCustomError">The custom error message for the specified method.</param>
        /// <param name="eventIndex">The event index to create.</param>
        private void ExceptionHandler(System.Exception e, FTPConnectionAdapter ftpAdapter,
            FTPStateAdapter state, Uri uri, string ftpMethod, string fileInfo,
            string ftpCustomError, Nequeo.Net.Ftp.SocketTransferDirection eventIndex)
        {
            // Detect a thread abort exception.
            if (e is ThreadAbortException)
                Thread.ResetAbort();

            // Set the response values to null if
            // no object created else assign the
            // response values.
            if (state != null)
            {
                // Assign the cuurent request information.
                ftpStatusCode = state.FtpStatusCode;
                ftpStatusDescription = state.FtpStatusDescription;
            }
            else
            {
                // Assign the default request information.
                ftpStatusCode = FtpStatusCode.Undefined;
                ftpStatusDescription = string.Empty;
            }

            // Make sure than a receiver instance of the
            // event delegate handler was created.
            if (OnFTPError != null)
                OnFTPError(this, new FTPSocketErrorArgs(ftpCustomError, ftpAdapter.FTPServer,
                    ftpMethod, fileInfo, ftpStatusCode, ftpStatusDescription, eventIndex));

            //// Write the error to the specified location.
            //StackTrace st = new StackTrace(e, true);
            //this.Write("AsynchronousFTPWebSocket", ftpMethod,
            //    ftpCustomError, st.GetFrame(0).GetFileLineNumber(),
            //    Enumeration.EnumHandler.WriteTo.FileAndEventLog,
            //    Enumeration.EnumHandler.LogType.Error);
        }
        #endregion

        #region Private Event Handler Methods
        /// <summary>
        /// The general event completion handler.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="state">The current ftp state object containing ftp state information.</param>
        /// <param name="ftpMethod">The current ftp process method.</param>
        /// <param name="fileInfo">The current upload target download destination.</param>
        /// <param name="eventIndex">The event index to create.</param>
        private void CompletionEventHandler(FTPConnectionAdapter ftpAdapter,
            FTPStateAdapter state, string ftpMethod, string fileInfo,
            Nequeo.Net.Ftp.SocketTransferDirection eventIndex)
        {
            switch (eventIndex)
            {
                case Nequeo.Net.Ftp.SocketTransferDirection.Upload:
                    // Create a new upload complete event
                    // for the current client.
                    if (OnUploadComplete != null)
                        OnUploadComplete(this, new FTPSocketArgs(ftpAdapter.FTPServer, ftpMethod,
                            fileInfo, state.FtpStatusCode, state.FtpStatusDescription, eventIndex));
                    break;

                case Nequeo.Net.Ftp.SocketTransferDirection.Download:
                    // Create a new download complete event
                    // for the current client.
                    if (OnDownloadComplete != null)
                        OnDownloadComplete(this, new FTPSocketArgs(ftpAdapter.FTPServer, ftpMethod,
                            fileInfo, state.FtpStatusCode, state.FtpStatusDescription, eventIndex));
                    break;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Upload a file to the specified server asynchronous.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="targetFile">The full path and file name of the file to upload.</param>
        /// <param name="uploadUri">The Uri object containing a valid file to upload to.</param>
        /// <returns>True if the upload was successful else false.</returns>
        public virtual bool Upload(FTPConnectionAdapter ftpAdapter,
            string targetFile, Uri uploadUri)
        {
            return this.AsynchronousUploadEx(ftpAdapter, targetFile, uploadUri);
        }

        /// <summary>
        /// Begin upload a file to the specified server asynchronous.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="targetFile">The full path and file name of the file to upload.</param>
        /// <param name="uploadUri">The Uri object containing a valid file to upload to.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginUpload(FTPConnectionAdapter ftpAdapter, string targetFile, 
            Uri uploadUri, AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncUpload(ftpAdapter, targetFile, uploadUri, this, callback, state);
        }

        /// <summary>
        /// End upload a file to the specified server asynchronous.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the data was returned else false.</returns>
        public virtual bool EndUpload(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncUpload.End(ar);
        }

        /// <summary>
        /// Download a file from the specified server asynchronous.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="destinationFile">The full path and file name where data should be written to.</param>
        /// <param name="downloadUri">The Uri object containing a valid file to download.</param>
        /// <returns>True if the download was successful else false.</returns>
        public virtual bool Download(FTPConnectionAdapter ftpAdapter,
            string destinationFile, Uri downloadUri)
        {
            return this.AsynchronousDownloadEx(ftpAdapter, destinationFile, downloadUri);
        }

        /// <summary>
        /// Begin download a file from the specified server asynchronous.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="destinationFile">The full path and file name where data should be written to.</param>
        /// <param name="downloadUri">The Uri object containing a valid file to download.</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        /// <returns>The asynchronous result.</returns>
        public virtual IAsyncResult BeginDownload(FTPConnectionAdapter ftpAdapter, string destinationFile,
            Uri downloadUri, AsyncCallback callback, object state)
        {
            // Return an AsyncResult.
            return new AsyncDownload(ftpAdapter, destinationFile, downloadUri, this, callback, state);
        }

        /// <summary>
        /// End download a file from the specified server asynchronous.
        /// </summary>
        /// <param name="ar">The asynchronous result.</param>
        /// <returns>True if the data was returned else false.</returns>
        public virtual bool EndDownload(IAsyncResult ar)
        {
            // Use the AsyncResult to complete that async operation.
            return AsyncDownload.End(ar);
        }
        #endregion
    }

    /// <summary>
    /// Class for asynchronous upload operations.
    /// </summary>
    internal class AsyncUpload : Nequeo.Threading.AsynchronousResult<bool>
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous upload operation.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="targetFile">The full path and file name of the file to upload.</param>
        /// <param name="uploadUri">The Uri object containing a valid file to upload to.</param>
        /// <param name="ftpSocket">The current ftp socket instance</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncUpload(FTPConnectionAdapter ftpAdapter, string targetFile, Uri uploadUri, 
            AsynchronousFTPSocket ftpSocket, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _uploadUri = uploadUri;
            _targetFile = targetFile;
            _ftpAdapter = ftpAdapter;
            _ftpSocket = ftpSocket;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncUploadThread1));
            Thread.Sleep(20);
        }

        private Uri _uploadUri = null;
        private string _targetFile = null;
        private FTPConnectionAdapter _ftpAdapter = null;
        private AsynchronousFTPSocket _ftpSocket = null;

        /// <summary>
        /// The async upload method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncUploadThread1(Object stateInfo)
        {
            // Upload the file.
            bool data = _ftpSocket.Upload(_ftpAdapter, _targetFile, _uploadUri);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data)
                base.Complete(data, true);
            else
                base.Complete(false);
        }
        #endregion
    }

    /// <summary>
    /// Class for asynchronous download operations.
    /// </summary>
    internal class AsyncDownload : Nequeo.Threading.AsynchronousResult<bool>
    {
        #region Asynchronous Operations
        /// <summary>
        /// Start the asynchronous download operation.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="destinationFile">The full path and file name where data should be written to.</param>
        /// <param name="downloadUri">The Uri object containing a valid file to download.</param>
        /// <param name="ftpSocket">The current ftp socket instance</param>
        /// <param name="callback">The asynchronous call back method.</param>
        /// <param name="state">The state object value.</param>
        public AsyncDownload(FTPConnectionAdapter ftpAdapter, string destinationFile, Uri downloadUri,
            AsynchronousFTPSocket ftpSocket, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _downloadUri = downloadUri;
            _destinationFile = destinationFile;
            _ftpAdapter = ftpAdapter;
            _ftpSocket = ftpSocket;

            ThreadPool.QueueUserWorkItem(new WaitCallback(AsyncDownloadThread1));
            Thread.Sleep(20);
        }

        private Uri _downloadUri = null;
        private string _destinationFile = null;
        private FTPConnectionAdapter _ftpAdapter = null;
        private AsynchronousFTPSocket _ftpSocket = null;

        /// <summary>
        /// The async download method.
        /// </summary>
        /// <param name="stateInfo">The thread object state.</param>
        private void AsyncDownloadThread1(Object stateInfo)
        {
            // Download the file.
            bool data = _ftpSocket.Download(_ftpAdapter, _destinationFile, _downloadUri);

            // If data exits then indicate that the asynchronous
            // operation has completed and send the result to the
            // client, else indicate that the asynchronous operation
            // has failed and did not complete.
            if (data)
                base.Complete(data, true);
            else
                base.Complete(false);
        }
        #endregion
    }

    /// <summary>
    /// A complete FTP integration socket to an FTP server,
    /// contains methods to upload/download data to and
    /// from the specified FTP sever.
    /// </summary>
    public interface IFTPSocket
    {
        #region Public Methods
        /// <summary>
        /// Download a file from the specified server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="destinationFile">The full path and file name where data should be written to.</param>
        /// <param name="downloadUri">The Uri object containing a valid file to download.</param>
        /// <returns>True if the download was successful else false.</returns>
        bool Download(FTPConnectionAdapter ftpAdapter,
            string destinationFile, Uri downloadUri);

        /// <summary>
        /// Upload a file to the specified server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="targetFile">The full path and file name of the file to upload.</param>
        /// <param name="uploadUri">The Uri object containing a valid file to upload to.</param>
        /// <returns>True if the upload was successful else false.</returns>
        bool Upload(FTPConnectionAdapter ftpAdapter,
            string targetFile, Uri uploadUri);

        /// <summary>
        /// Remove the directory from the server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="removeDirectoryUri">The Uri object containing a valid directory to remove to.</param>
        /// <returns>True if the remove directory was successful else false.</returns>
        bool RemoveDirectory(FTPConnectionAdapter ftpAdapter, Uri removeDirectoryUri);

        /// <summary>
        /// Delete a file from the server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="deleteFileUri">The Uri object containing a valid file to delete to.</param>
        /// <returns>True if the delete file was successful else false.</returns>
        bool DeleteFile(FTPConnectionAdapter ftpAdapter, Uri deleteFileUri);

        /// <summary>
        /// Create a new directory on the server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="makeDirectoryUri">The Uri object containing a valid directory to create to.</param>
        /// <returns>True if the make directory was successful else false.</returns>
        bool MakeDirectory(FTPConnectionAdapter ftpAdapter, Uri makeDirectoryUri);

        /// <summary>
        /// Rename the file on the server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="newFileName">The name to change to server file to.</param>
        /// <param name="renameFileUri">The Uri object containing a valid file to rename to.</param>
        /// <returns>True if the rename file was successful else false.</returns>
        bool RenameFile(FTPConnectionAdapter ftpAdapter,
            string newFileName, Uri renameFileUri);

        /// <summary>
        /// A directory list of all files on the specified server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="directoryListUri">The Uri object containing a valid directory to list.</param>
        /// <param name="fileList">The list of files within the directory.</param>
        /// <returns>True if the download was successful else false.</returns>
        bool DirectoryList(FTPConnectionAdapter ftpAdapter,
            Uri directoryListUri, out List<string> fileList);

        /// <summary>
        /// File size from the specified server.
        /// </summary>
        /// <param name="ftpAdapter">FTP adapter contains the complete connection information.</param>
        /// <param name="remoteFileUri">The Uri object containing a valid file on the remote host.</param>
        /// <param name="remoteFileSize">The size of the file on the remote host.</param>
        /// <returns>True if the file size return was successful else false.</returns>
        bool FileSize(FTPConnectionAdapter ftpAdapter,
            Uri remoteFileUri, out long remoteFileSize);

        #endregion
    }
}
