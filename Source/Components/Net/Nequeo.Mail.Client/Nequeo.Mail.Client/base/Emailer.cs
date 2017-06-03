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
using System.Threading;
using System.Diagnostics;
using System.Net.Security;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;

using Nequeo.Extension;
using Nequeo.Security;
using Nequeo.ComponentModel.Composition;

namespace Nequeo.Net.Mail
{
    /// <summary>
    /// General emailer class.
    /// </summary>
    [Export(typeof(IEmailer))]
    [ContentMetadata(Name = "Emailer")]
    public partial class Emailer : IDisposable, IEmailer
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public Emailer()
        {
            // Start the async control.
            _asyncAccount = new Nequeo.Threading.AsyncExecutionHandler<Emailer>();
            _asyncAccount.AsyncError += new Threading.EventHandler<Exception>(_asyncAccount_AsyncError);
            _asyncAccount.AsyncComplete += new Threading.EventHandler<object, string>(_asyncAccount_AsyncComplete);
            _asyncAccount.InitiliseAsyncInstance(this);

            OnCreated();
        }
        #endregion

        #region Private Fields
        private X509Certificate2Info _sslCertificate = null;
        private EmailerConnectionAdapter _emailAdapter = null;

        private Nequeo.Threading.AsyncExecutionHandler<Emailer> _asyncAccount = null;

        private Dictionary<object, object> _callback = new Dictionary<object, object>();
        private Dictionary<object, object> _state = new Dictionary<object, object>();
        
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the secure certificate.
        /// </summary>
        public X509Certificate2Info Certificate
        {
            get { return _sslCertificate; }
        }
        #endregion

        #region Private Methods
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
            // Certificate should be validated.
            if (_emailAdapter.ValidateCertificate)
            {
                // If the certificate is valid
                // return true.
                if (sslPolicyErrors == SslPolicyErrors.None)
                    return true;
                else
                {
                    // Create a new certificate collection
                    // instance and return false.
                    _sslCertificate = new X509Certificate2Info(
                        certificate as X509Certificate2, chain, sslPolicyErrors);
                    return false;
                }
            }
            else
                // Return true.
                return true;
        }

        /// <summary>
        /// Async complete action handler
        /// </summary>
        /// <param name="sender">The current object handler</param>
        /// <param name="e1">The action execution result</param>
        /// <param name="e2">The unique action name.</param>
        private void _asyncAccount_AsyncComplete(object sender, object e1, string e2)
        {
            switch (e2)
            {
                case "SendEmail_1":
                    Action<Nequeo.Threading.AsyncOperationResult<bool>> callbackSendEmail_1 = (Action<Nequeo.Threading.AsyncOperationResult<bool>>)_callback[e2];
                    callbackSendEmail_1(new Nequeo.Threading.AsyncOperationResult<bool>(((bool)e1), _state[e2], e2));
                    break;

                case "SendEmail_2":
                    Action<Nequeo.Threading.AsyncOperationResult<bool>> callbackSendEmail_2 = (Action<Nequeo.Threading.AsyncOperationResult<bool>>)_callback[e2];
                    callbackSendEmail_2(new Nequeo.Threading.AsyncOperationResult<bool>(((bool)e1), _state[e2], e2));
                    break;

                case "SendEmail_3":
                    Action<Nequeo.Threading.AsyncOperationResult<bool>> callbackSendEmail_3 = (Action<Nequeo.Threading.AsyncOperationResult<bool>>)_callback[e2];
                    callbackSendEmail_3(new Nequeo.Threading.AsyncOperationResult<bool>(((bool)e1), _state[e2], e2));
                    break;

                case "SendEmail_4":
                    Action<Nequeo.Threading.AsyncOperationResult<bool>> callbackSendEmail_4 = (Action<Nequeo.Threading.AsyncOperationResult<bool>>)_callback[e2];
                    callbackSendEmail_4(new Nequeo.Threading.AsyncOperationResult<bool>(((bool)e1), _state[e2], e2));
                    break;

                case "SendEmail_5":
                    Action<Nequeo.Threading.AsyncOperationResult<bool>> callbackSendEmail_5 = (Action<Nequeo.Threading.AsyncOperationResult<bool>>)_callback[e2];
                    callbackSendEmail_5(new Nequeo.Threading.AsyncOperationResult<bool>(((bool)e1), _state[e2], e2));
                    break;

                case "SendEmail_6":
                    Action<Nequeo.Threading.AsyncOperationResult<bool>> callbackSendEmail_6 = (Action<Nequeo.Threading.AsyncOperationResult<bool>>)_callback[e2];
                    callbackSendEmail_6(new Nequeo.Threading.AsyncOperationResult<bool>(((bool)e1), _state[e2], e2));
                    break;

                default:
                    _exception = new Exception("The async operation is not supported.");
                    break;
            }
        }
        #endregion

        #region Private Email
        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="emailCCs">The collection of carbon copies recipients.</param>
        /// <param name="emailBCCs">The collection of blind carbon copies recipients.</param>
        /// <param name="replyTo">The reply to email address for the message.</param>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <param name="richTextMessage">The rich text version of the email.</param>
        /// <param name="xmlMessage">The xml version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="priority">The priority of the email message.</param>
        /// <param name="emailType">The email format to sent as.</param>
        /// <param name="linkedResources">The collection of linked resource to sent, used for embeded images in html.</param>
        /// <returns>True if the email was sent to the recipients else false.</returns>
        /// <exception cref="System.Exception"></exception>
        private bool SendEmailEx(EmailerConnectionAdapter emailAdapter, MailAddress from, 
            List<MailAddress> emailTo, string subject, List<MailAddress> emailCCs,
            List<MailAddress> emailBCCs, List<MailAddress> replyTo, string textMessage, string htmlMessage, 
            string richTextMessage, string xmlMessage, List<Attachment> attachments,
            MailPriority priority, Nequeo.Net.Mail.MessageType emailType, List<LinkedResource> linkedResources)
        {
            // Assign the email adapter.
            _emailAdapter = emailAdapter;

            try
            {
                // Get the recipients from the application configuration data.
                // Create a new instance of the mail message class. Set the
                // from and to text.
                using (MailMessage MyMail = new MailMessage())
                {
                    AlternateView textView = null;
                    AlternateView htmlView = null;
                    AlternateView richView = null;
                    AlternateView xmlView = null;

                    // Make sure text exists.
                    if (!String.IsNullOrEmpty(textMessage) && _emailAdapter.EncryptionCertificate == null)
                    {
                        // Add the html message type to the email.
                        textView = AlternateView.CreateAlternateViewFromString(textMessage, null, "text/plain");
                        textView.TransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;
                    }

                    // Make sure text exists.
                    if (!String.IsNullOrEmpty(htmlMessage) && _emailAdapter.EncryptionCertificate == null)
                    {
                        // Add the html message type to the email.
                        htmlView = AlternateView.CreateAlternateViewFromString(htmlMessage, null, "text/html");
                        htmlView.TransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;

                        // Contains embebed images in html.
                        if (linkedResources != null)
                            foreach (LinkedResource linkedResource in linkedResources)
                                htmlView.LinkedResources.Add(linkedResource);
                    }

                    // Make sure text exists.
                    if (!String.IsNullOrEmpty(richTextMessage) && _emailAdapter.EncryptionCertificate == null)
                    {
                        // Add the rich text message type to the email.
                        richView = AlternateView.CreateAlternateViewFromString(richTextMessage, null, "text/html");
                        richView.TransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;
                    }

                    // Make sure text exists.
                    if (!String.IsNullOrEmpty(xmlMessage) && _emailAdapter.EncryptionCertificate == null)
                    {
                        // Add the xml message type to the email.
                        xmlView = AlternateView.CreateAlternateViewFromString(xmlMessage, null, "text/xml");
                        xmlView.TransferEncoding = System.Net.Mime.TransferEncoding.QuotedPrintable;
                    }

                    // Select the email type to
                    // send to the user.
                    switch (emailType)
                    {
                        case Nequeo.Net.Mail.MessageType.Xml:
                            // Text email type.
                            if (xmlView != null)
                                MyMail.AlternateViews.Add(xmlView);
                            break;

                        case Nequeo.Net.Mail.MessageType.Text:
                            // Text email type.
                            if (textView != null)
                                MyMail.AlternateViews.Add(textView);
                            break;

                        case Nequeo.Net.Mail.MessageType.Html:
                            // Html email type.
                            if (htmlView != null)
                                MyMail.AlternateViews.Add(htmlView);
                            break;

                        case Nequeo.Net.Mail.MessageType.RichText:
                            // Rich text email type.
                            if (richView != null)
                                MyMail.AlternateViews.Add(richView);
                            break;

                        case Nequeo.Net.Mail.MessageType.TextAndHtml:
                            // Text and html email type.
                            if (textView != null)
                                MyMail.AlternateViews.Add(textView);

                            if (htmlView != null)
                                MyMail.AlternateViews.Add(htmlView);
                            break;

                        case Nequeo.Net.Mail.MessageType.TextAndRichText:
                            // Text and rich text email type.
                            if (textView != null)
                                MyMail.AlternateViews.Add(textView);

                            if (richView != null)
                                MyMail.AlternateViews.Add(richView);
                            break;

                        case Nequeo.Net.Mail.MessageType.HtmlAndRichText:
                            // Html and rich text email type.
                            if (htmlView != null)
                                MyMail.AlternateViews.Add(htmlView);

                            if (richView != null)
                                MyMail.AlternateViews.Add(richView);
                            break;

                        case Nequeo.Net.Mail.MessageType.All:
                            // All email types.
                            if (textView != null)
                                MyMail.AlternateViews.Add(textView);

                            if (htmlView!= null)
                                MyMail.AlternateViews.Add(htmlView);

                            if (richView != null)
                                MyMail.AlternateViews.Add(richView);

                            if (xmlView != null)
                                MyMail.AlternateViews.Add(xmlView);
                            break;
                    }

                    // Add each email attachments.
                    if (attachments != null && _emailAdapter.EncryptionCertificate == null)
                        foreach (Attachment attachment in attachments)
                            MyMail.Attachments.Add(attachment);

                    // If signed or encypted.
                    if (_emailAdapter.SigningCertificate != null || _emailAdapter.EncryptionCertificate != null)
                    {
                        // Get the alternative view.
                        Attachment viewSignEnc = CreateAlternateView(textMessage, htmlMessage, richTextMessage, xmlMessage, attachments, emailType);
                        if (viewSignEnc != null)
                            MyMail.Attachments.Add(viewSignEnc);
                    }

                    // Add each cc email address.
                    if (emailCCs != null)
                        foreach (MailAddress emailCC in emailCCs)
                            MyMail.CC.Add(emailCC);

                    // Add each bcc email address.
                    if (emailBCCs != null)
                        foreach (MailAddress emailBCC in emailBCCs)
                            MyMail.Bcc.Add(emailBCC);

                    // Add each recipient.
                    if (emailTo != null)
                        foreach (MailAddress to in emailTo)
                            MyMail.To.Add(to);

                    // Add the subject and body to the email.
                    MyMail.Subject = subject;
                    MyMail.Priority = priority;
                    MyMail.From = from;

                    // If a reply to email  address has been set.
                    if (replyTo != null)
                        foreach (MailAddress to in replyTo)
                            MyMail.ReplyToList.Add(to);

                    // Create a new instance of the smtp client
                    using (SmtpClient smtpClient = new SmtpClient())
                    {
                        smtpClient.Timeout = emailAdapter.TimeOut;

                        // If a ssl connection is required.
                        if (emailAdapter.UseSSLConnection)
                            smtpClient.EnableSsl = true;

                        // If a client certificate is to be used
                        // add the client certificate.
                        if (emailAdapter.ClientCertificate != null)
                            smtpClient.ClientCertificates.Add(emailAdapter.ClientCertificate);

                        // Create a new service point manger to
                        // validate the certificate.
                        ServicePointManager.ServerCertificateValidationCallback = new
                            RemoteCertificateValidationCallback(OnCertificateValidation);

                        // Get the mail server domain name
                        // or IP address to use as an email
                        // relay.
                        smtpClient.Host = emailAdapter.Server;

                        // If a port number is supplied then
                        // use the assigned port number to
                        // connect to the mail server.
                        smtpClient.Port = emailAdapter.Port;

                        // If both the password and username for the
                        // email server are supplied then create
                        // a new network credential class else
                        // use the default credential cache.
                        if (!String.IsNullOrEmpty(emailAdapter.UserName) &&
                            !String.IsNullOrEmpty(emailAdapter.Password))
                        {
                            // Create a new network credential account and
                            // pass the information to the mail server.
                            smtpClient.Credentials = new NetworkCredential(emailAdapter.UserName,
                                emailAdapter.Password, emailAdapter.Domain);
                        }
                        else
                            // Use default cache credentials.
                            smtpClient.UseDefaultCredentials = true;

                        // Send the mail message to the
                        // recipients.
                        smtpClient.Send(MyMail);
                    }
                }

                return true;
            }
            catch (System.Exception ex)
            {
                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Create an alternative view with signing and encryption certificates.
        /// </summary>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <param name="richTextMessage">The rich text version of the email.</param>
        /// <param name="xmlMessage">The xml version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="emailType">The email format to sent as.</param>
        /// <returns>The new alternate view.</returns>
        private Attachment CreateAlternateView(string textMessage, string htmlMessage,
            string richTextMessage, string xmlMessage, List<Attachment> attachments,
            Nequeo.Net.Mail.MessageType emailType)
        {
            Attachment view = null;
            MemoryStream signStream = null;
            MemoryStream encStream = null;
            byte[] message = null;

            try
            {
                Nequeo.Cryptography.Cms cms = new Cryptography.Cms();
                string contentType = null;
                
                // Get a messages.
                byte[] textMessageData = (!String.IsNullOrEmpty(textMessage) ? Encoding.Default.GetBytes(textMessage) : new byte[0]);
                byte[] htmlMessageData = (!String.IsNullOrEmpty(htmlMessage) ? Encoding.Default.GetBytes(htmlMessage) : new byte[0]);
                byte[] richTextMessageData = (!String.IsNullOrEmpty(richTextMessage) ? Encoding.Default.GetBytes(richTextMessage) : new byte[0]);
                byte[] xmlMessageData = (!String.IsNullOrEmpty(xmlMessage) ? Encoding.Default.GetBytes(xmlMessage) : new byte[0]);

                // Select the email type to
                // send to the user.
                switch (emailType)
                {
                    case Nequeo.Net.Mail.MessageType.Xml:
                        // Combine all messages.
                        message = xmlMessageData;
                        break;

                    case Nequeo.Net.Mail.MessageType.Text:
                        // Combine all messages.
                        message = textMessageData;
                        break;

                    case Nequeo.Net.Mail.MessageType.Html:
                        // Combine all messages.
                        message = htmlMessageData;
                        break;

                    case Nequeo.Net.Mail.MessageType.RichText:
                        // Combine all messages.
                        message = richTextMessageData;
                        break;

                    case Nequeo.Net.Mail.MessageType.TextAndHtml:
                        // Combine all messages.
                        message = textMessageData.Combine(htmlMessageData);
                        break;

                    case Nequeo.Net.Mail.MessageType.TextAndRichText:
                        // Combine all messages.
                        message = textMessageData.Combine(richTextMessageData);
                        break;

                    case Nequeo.Net.Mail.MessageType.HtmlAndRichText:
                        // Combine all messages.
                        message = htmlMessageData.Combine(richTextMessageData);
                        break;

                    case Nequeo.Net.Mail.MessageType.All:
                        // Combine all messages.
                        message = textMessageData.Combine(htmlMessageData, richTextMessageData, xmlMessageData);
                        break;
                }

                // Sign the data.
                if (_emailAdapter.SigningCertificate != null)
                {
                    // Make sure text exists.
                    if (message != null)
                    {
                        // Signed content.
                        contentType = "application/pkcs7-signature; name=\"smime.p7s\"";

                        // Get the message.
                        ContentInfo contentText = null;

                        // Sign message and attachments.
                        if (attachments != null && attachments.Count > 0)
                        {
                            byte[] completeContentInfo = new byte[0];
                            byte[] temp = completeContentInfo.Combine(message);
                            completeContentInfo = temp;
                            
                            // For each attachment.
                            foreach (Attachment attachment in attachments)
                            {
                                Stream stream = null;
                                long position = 0;

                                try
                                {
                                    // Get the attachment stream.
                                    stream = attachment.ContentStream;
                                    position = stream.Position;
                                    byte[] buffer = new byte[(int)stream.Length];
                                    stream.Position = 0;

                                    // Read all the data.
                                    int bytesRead = stream.Read(buffer, 0, (int)stream.Length);
                                    if (bytesRead > 0)
                                    {
                                        // Combine all the attachment data.
                                        byte[] temp1 = completeContentInfo.Combine(buffer);
                                        completeContentInfo = temp1;
                                    }
                                }
                                catch { }
                                finally
                                {
                                    // Make sure the stream exists.
                                    if (stream != null)
                                    {
                                        // Reset the position.
                                        stream.Position = position;
                                    }
                                }
                            }

                            // Sign message and attachments.
                            contentText = contentText = new ContentInfo(completeContentInfo);
                            byte[] signText = cms.ComputeSignature(contentText, _emailAdapter.SigningCertificate);
                            signStream = new MemoryStream(Nequeo.Custom.Base64Encoder.Encode(signText));
                        }
                        else
                        {
                            // Sign plain message only.
                            contentText = new ContentInfo(message);
                            byte[] signText = cms.ComputeSignature(contentText, _emailAdapter.SigningCertificate);
                            signStream = new MemoryStream(Nequeo.Custom.Base64Encoder.Encode(signText));
                        }
                    }
                }

                // Encryption the data.
                if (_emailAdapter.EncryptionCertificate != null)
                {
                    // Make sure text exists.
                    if (message != null)
                    {
                        // Encrypted content.
                        contentType = "application/pkcs7-mime; name=\"smime.p7m\"; smime-type=enveloped-data";

                        // If singer.
                        if (signStream != null)
                        {
                            // Get the email message and signed content.
                            byte[] signEncText = CreateMultipartSignedEncrypt(signStream, textMessage, htmlMessage, richTextMessage, xmlMessage, attachments, emailType);

                            // Encrypt the message along with the signature.
                            ContentInfo contentText = new ContentInfo(signEncText);
                            byte[] encText = cms.Encrypt(contentText, _emailAdapter.EncryptionCertificate);
                            encStream = new MemoryStream(Nequeo.Custom.Base64Encoder.Encode(encText));
                        }
                        else
                        {
                            // Get the email message.
                            byte[] messageText = CreateMultipartEncrypt(textMessage, htmlMessage, richTextMessage, xmlMessage, attachments, emailType);

                            // No signer, just encrypter.
                            ContentInfo contentText = new ContentInfo(messageText);
                            byte[] encText = cms.Encrypt(contentText, _emailAdapter.EncryptionCertificate);
                            encStream = new MemoryStream(Nequeo.Custom.Base64Encoder.Encode(encText));
                        }
                    }
                }

                // Which to use.
                if (signStream != null || encStream != null)
                {
                    System.Net.Mime.ContentType contType = new System.Net.Mime.ContentType(contentType);
                    view = new Attachment((signStream != null ? signStream : encStream), contType);
                    view.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;
                }

                // Return the view.
                return view;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (signStream != null)
                    signStream.Dispose();

                if (encStream != null)
                    encStream.Dispose();
            }
        }

        /// <summary>
        /// Create the multipart message and signed data.
        /// </summary>
        /// <param name="signStream">The stream containing the signed data.</param>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <param name="richTextMessage">The rich text version of the email.</param>
        /// <param name="xmlMessage">The xml version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="emailType">The email format to sent as.</param>
        /// <returns>The formatted message and signed data.</returns>
        private byte[] CreateMultipartSignedEncrypt(MemoryStream signStream, string textMessage, string htmlMessage,
            string richTextMessage, string xmlMessage, List<Attachment> attachments, Nequeo.Net.Mail.MessageType emailType)
        {
            // Get the random number.
            string randomBounbary = new Nequeo.Invention.NumberGenerator().Random(20, 20);

            // Create messsage content.
            byte[] messageContent = Encoding.UTF8.GetBytes("--------------" + randomBounbary);
            byte[] multipartMessage = CreateMultipartEncrypt(textMessage, htmlMessage, richTextMessage, xmlMessage, attachments, emailType);

            // Base64 encode the signed data.
            byte[] signHeader = Encoding.UTF8.GetBytes
                (
                    "--------------" + randomBounbary + "\r\n" +
                    "Content-Type: application/pkcs7-signature; name=\"smime.p7s\"" + "\r\n" +
                    "Content-Transfer-Encoding: base64" + "\r\n" +
                    "Content-Disposition: attachment; filename=\"smime.p7s\"" + "\r\n" +
                    "Content-Description: S/MIME Cryptographic Signature" + "\r\n" +
                    "\r\n"
                );
            byte[] sign = signStream.ToArray();
            byte[] end = Encoding.UTF8.GetBytes("--------------" + randomBounbary + "--" + "\r\n" + "\r\n");

            // Combine the message and signature
            byte[] signEncText = messageContent.Combine(multipartMessage, signHeader, sign, end);
            return signEncText;
        }

        /// <summary>
        /// Create the plain message data.
        /// </summary>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <param name="richTextMessage">The rich text version of the email.</param>
        /// <param name="xmlMessage">The xml version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="emailType">The email format to sent as.</param>
        /// <returns>The formatted message data.</returns>
        private byte[] CreateMultipartEncrypt(string textMessage, string htmlMessage,
            string richTextMessage, string xmlMessage, List<Attachment> attachments,
            Nequeo.Net.Mail.MessageType emailType)
        {
            // Get the random number.
            string randomBounbary = new Nequeo.Invention.NumberGenerator().Random(20, 20);

            // Create messsage content.
            byte[] messageContent = Encoding.UTF8.GetBytes
                (
                    "Content-Type: multipart/mixed; boundary=\"------------" + randomBounbary + "\"" + "\r\n" +
                    "\r\n" +
                    "This is a multi-part message in MIME format." + "\r\n"
                );

            byte[] messageContentEnd = Encoding.UTF8.GetBytes("--------------" + randomBounbary + "--" + "\r\n" + "\r\n");
            byte[] completeContentInfo = new byte[0];
            byte[] message = new byte[0];

            // Select the email type to
            // send to the user.
            switch (emailType)
            {
                case Nequeo.Net.Mail.MessageType.Xml:
                    // Combine all messages.
                    message = Encoding.UTF8.GetBytes
                        (
                            (
                                !String.IsNullOrEmpty(xmlMessage) ?
                                "--------------" + randomBounbary + "\r\n" +
                                "Content-Type: text/xml; charset=utf-8; format=flowed" + "\r\n" +
                                "Content-Transfer-Encoding: quoted-printable" + "\r\n" +
                                "\r\n" +
                                Nequeo.Custom.QuotedPrintable.Encode(xmlMessage.ToCharArray()) + "\r\n" +
                                "\r\n" +
                                "\r\n" : ""
                            )
                        );
                    break;

                case Nequeo.Net.Mail.MessageType.Text:
                    // Combine all messages.
                    message = Encoding.UTF8.GetBytes
                        (
                            (
                                !String.IsNullOrEmpty(textMessage) ?
                                "--------------" + randomBounbary + "\r\n" +
                                "Content-Type: text/plain; charset=utf-8; format=flowed" + "\r\n" +
                                "Content-Transfer-Encoding: quoted-printable" + "\r\n" +
                                "\r\n" +
                                Nequeo.Custom.QuotedPrintable.Encode(textMessage.ToCharArray()) + "\r\n" +
                                "\r\n" +
                                "\r\n" : ""
                            )
                        );
                    break;

                case Nequeo.Net.Mail.MessageType.Html:
                    // Combine all messages.
                    message = Encoding.UTF8.GetBytes
                        (
                            (
                                !String.IsNullOrEmpty(htmlMessage) ?
                                "--------------" + randomBounbary + "\r\n" +
                                "Content-Type: text/html; charset=utf-8; format=flowed" + "\r\n" +
                                "Content-Transfer-Encoding: quoted-printable" + "\r\n" +
                                "\r\n" +
                                Nequeo.Custom.QuotedPrintable.Encode(htmlMessage.ToCharArray()) + "\r\n" +
                                "\r\n" +
                                "\r\n" : ""
                            )
                        );
                    break;

                case Nequeo.Net.Mail.MessageType.RichText:
                    // Combine all messages.
                    message = Encoding.UTF8.GetBytes
                        (
                            (
                                !String.IsNullOrEmpty(richTextMessage) ?
                                "--------------" + randomBounbary + "\r\n" +
                                "Content-Type: text/html; charset=utf-8; format=flowed" + "\r\n" +
                                "Content-Transfer-Encoding: quoted-printable" + "\r\n" +
                                "\r\n" +
                                Nequeo.Custom.QuotedPrintable.Encode(richTextMessage.ToCharArray()) + "\r\n" +
                                "\r\n" +
                                "\r\n" : ""
                            )
                        );
                    break;

                case Nequeo.Net.Mail.MessageType.TextAndHtml:
                    // Combine all messages.
                    message = Encoding.UTF8.GetBytes
                        (
                            (
                                !String.IsNullOrEmpty(textMessage) ?
                                "--------------" + randomBounbary + "\r\n" +
                                "Content-Type: text/plain; charset=utf-8; format=flowed" + "\r\n" +
                                "Content-Transfer-Encoding: quoted-printable" + "\r\n" +
                                "\r\n" +
                                Nequeo.Custom.QuotedPrintable.Encode(textMessage.ToCharArray()) + "\r\n" +
                                "\r\n" +
                                "\r\n" : ""
                            ) +
                            (
                                !String.IsNullOrEmpty(htmlMessage) ?
                                "--------------" + randomBounbary + "\r\n" +
                                "Content-Type: text/html; charset=utf-8; format=flowed" + "\r\n" +
                                "Content-Transfer-Encoding: quoted-printable" + "\r\n" +
                                "\r\n" +
                                Nequeo.Custom.QuotedPrintable.Encode(htmlMessage.ToCharArray()) + "\r\n" +
                                "\r\n" +
                                "\r\n" : ""
                            )
                        );
                    break;

                case Nequeo.Net.Mail.MessageType.TextAndRichText:
                    // Combine all messages.
                    message = Encoding.UTF8.GetBytes
                        (
                            (
                                !String.IsNullOrEmpty(textMessage) ?
                                "--------------" + randomBounbary + "\r\n" +
                                "Content-Type: text/plain; charset=utf-8; format=flowed" + "\r\n" +
                                "Content-Transfer-Encoding: quoted-printable" + "\r\n" +
                                "\r\n" +
                                Nequeo.Custom.QuotedPrintable.Encode(textMessage.ToCharArray()) + "\r\n" +
                                "\r\n" +
                                "\r\n" : ""
                            ) +
                            (
                                !String.IsNullOrEmpty(richTextMessage) ?
                                "--------------" + randomBounbary + "\r\n" +
                                "Content-Type: text/html; charset=utf-8; format=flowed" + "\r\n" +
                                "Content-Transfer-Encoding: quoted-printable" + "\r\n" +
                                "\r\n" +
                                Nequeo.Custom.QuotedPrintable.Encode(richTextMessage.ToCharArray()) + "\r\n" +
                                "\r\n" +
                                "\r\n" : ""
                            )
                        );
                    break;

                case Nequeo.Net.Mail.MessageType.HtmlAndRichText:
                    // Combine all messages.
                    message = Encoding.UTF8.GetBytes
                        (
                            (
                                !String.IsNullOrEmpty(htmlMessage) ?
                                "--------------" + randomBounbary + "\r\n" +
                                "Content-Type: text/html; charset=utf-8; format=flowed" + "\r\n" +
                                "Content-Transfer-Encoding: quoted-printable" + "\r\n" +
                                "\r\n" +
                                Nequeo.Custom.QuotedPrintable.Encode(htmlMessage.ToCharArray()) + "\r\n" +
                                "\r\n" +
                                "\r\n" : ""
                            ) +
                            (
                                !String.IsNullOrEmpty(richTextMessage) ?
                                "--------------" + randomBounbary + "\r\n" +
                                "Content-Type: text/html; charset=utf-8; format=flowed" + "\r\n" +
                                "Content-Transfer-Encoding: quoted-printable" + "\r\n" +
                                "\r\n" +
                                Nequeo.Custom.QuotedPrintable.Encode(richTextMessage.ToCharArray()) + "\r\n" +
                                "\r\n" +
                                "\r\n" : ""
                            )
                        );
                    break;

                case Nequeo.Net.Mail.MessageType.All:
                    // Combine all messages.
                    message = Encoding.UTF8.GetBytes
                        (
                            (
                                !String.IsNullOrEmpty(textMessage) ?
                                "--------------" + randomBounbary + "\r\n" +
                                "Content-Type: text/plain; charset=utf-8; format=flowed" + "\r\n" +
                                "Content-Transfer-Encoding: quoted-printable" + "\r\n" +
                                "\r\n" +
                                Nequeo.Custom.QuotedPrintable.Encode(textMessage.ToCharArray()) + "\r\n" +
                                "\r\n" +
                                "\r\n" : ""
                            ) +
                            (
                                !String.IsNullOrEmpty(htmlMessage) ?
                                "--------------" + randomBounbary + "\r\n" +
                                "Content-Type: text/html; charset=utf-8; format=flowed" + "\r\n" +
                                "Content-Transfer-Encoding: quoted-printable" + "\r\n" +
                                "\r\n" +
                                Nequeo.Custom.QuotedPrintable.Encode(htmlMessage.ToCharArray()) + "\r\n" +
                                "\r\n" +
                                "\r\n" : ""
                            ) +
                            (
                                !String.IsNullOrEmpty(richTextMessage) ?
                                "--------------" + randomBounbary + "\r\n" +
                                "Content-Type: text/html; charset=utf-8; format=flowed" + "\r\n" +
                                "Content-Transfer-Encoding: quoted-printable" + "\r\n" +
                                "\r\n" +
                                Nequeo.Custom.QuotedPrintable.Encode(richTextMessage.ToCharArray()) + "\r\n" +
                                "\r\n" +
                                "\r\n" : ""
                            ) +
                            (
                                !String.IsNullOrEmpty(xmlMessage) ?
                                "--------------" + randomBounbary + "\r\n" +
                                "Content-Type: text/xml; charset=utf-8; format=flowed" + "\r\n" +
                                "Content-Transfer-Encoding: quoted-printable" + "\r\n" +
                                "\r\n" +
                                Nequeo.Custom.QuotedPrintable.Encode(xmlMessage.ToCharArray()) + "\r\n" +
                                "\r\n" +
                                "\r\n" : ""
                            )
                        );
                    break;
            }

            // Message and attachments.
            if (attachments != null && attachments.Count > 0)
            {
                byte[] temp = completeContentInfo.Combine(message);
                completeContentInfo = temp;

                // For each attachment.
                foreach (Attachment attachment in attachments)
                {
                    Stream stream = null;
                    long position = 0;

                    try
                    {
                        // Get the attachment stream.
                        stream = attachment.ContentStream;
                        position = stream.Position;
                        byte[] buffer = new byte[(int)stream.Length];
                        stream.Position = 0;

                        // Read all the data.
                        int bytesRead = stream.Read(buffer, 0, (int)stream.Length);
                        if (bytesRead > 0)
                        {
                            // Combine all messages.
                            byte[] contentTypeEnd = Encoding.UTF8.GetBytes("\r\n");
                            byte[] contentType = Encoding.UTF8.GetBytes
                                (
                                    "--------------" + randomBounbary + "\r\n" +
                                    "Content-Type: " + 
                                        ((attachment.ContentType == null) ? "text/plain" : 
                                            (!String.IsNullOrEmpty(attachment.ContentType.MediaType) ? attachment.ContentType.MediaType : "text/plain")) +
                                        "; charset=" + 
                                            ((attachment.ContentType == null) ? "windows-1252" : 
                                                (!String.IsNullOrEmpty(attachment.ContentType.CharSet) ? attachment.ContentType.CharSet : "windows-1252")) + 
                                        "; name=\"" + 
                                            (!String.IsNullOrEmpty(attachment.Name) ? attachment.Name : "unknown.txt") + "\"" + "\r\n" +
                                    "Content-Transfer-Encoding: " + Nequeo.Net.Mime.MimeEncoder.GetTransferEncoding(attachment.TransferEncoding) + "\r\n" +
                                    "Content-Disposition: attachment; filename=\"" + 
                                        ((attachment.ContentDisposition == null) ? "unknown.txt" : 
                                            (!String.IsNullOrEmpty(attachment.ContentDisposition.FileName) ? attachment.ContentDisposition.FileName : "unknown.txt")) + "\"" + "\r\n" +
                                    "\r\n"
                                );

                            // Combine all the attachment data.
                            byte[] temp1 = contentType.Combine(completeContentInfo, buffer, contentTypeEnd);
                            completeContentInfo = temp1;
                        }
                    }
                    catch { }
                    finally
                    {
                        // Make sure the stream exists.
                        if (stream != null)
                        {
                            // Reset the position.
                            stream.Position = position;
                        }
                    }
                }
            }
            else
            {
                // Only message content.
                completeContentInfo = message;
            }

            // Return the message.
            return messageContent.Combine(completeContentInfo, messageContentEnd);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="emailCCs">The collection of carbon copies recipients.</param>
        /// <param name="emailBCCs">The collection of blind carbon copies recipients.</param>
        /// <param name="replyTo">The reply to email address for the message.</param>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <param name="richTextMessage">The rich text version of the email.</param>
        /// <param name="xmlMessage">The xml version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="linkedResources">The collection of linked resource to sent, used for embeded images in html.</param>
        /// <param name="priority">The priority of the email message.</param>
        /// <param name="emailType">The email format to sent as.</param>
        /// <returns>True if the email was sent to the recipients else false.</returns>
        /// <exception cref="System.Exception"></exception>
        [Export("SendConstructedEmail")]
        public virtual bool SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, List<MailAddress> emailCCs,
            List<MailAddress> emailBCCs, List<MailAddress> replyTo, string textMessage, string htmlMessage,
            string richTextMessage, string xmlMessage, List<Attachment> attachments, List<LinkedResource> linkedResources, 
            MailPriority priority, Nequeo.Net.Mail.MessageType emailType)
        {
            return SendEmailEx(emailAdapter, from, emailTo, subject, emailCCs, emailBCCs, replyTo,
                textMessage, htmlMessage, richTextMessage, xmlMessage, attachments, priority, emailType, linkedResources);
        }

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="emailCCs">The collection of carbon copies recipients.</param>
        /// <param name="emailBCCs">The collection of blind carbon copies recipients.</param>
        /// <param name="replyTo">The reply to email address for the message.</param>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <param name="richTextMessage">The rich text version of the email.</param>
        /// <param name="xmlMessage">The xml version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="linkedResources">The collection of linked resource to sent, used for embeded images in html.</param>
        /// <param name="priority">The priority of the email message.</param>
        /// <param name="emailType">The email format to sent as.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        /// <exception cref="System.Exception"></exception>
        [Export("SendConstructedEmailAsync")]
        public virtual void SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, List<MailAddress> emailCCs,
            List<MailAddress> emailBCCs, List<MailAddress> replyTo, string textMessage, string htmlMessage,
            string richTextMessage, string xmlMessage, List<Attachment> attachments, List<LinkedResource> linkedResources,
            MailPriority priority, Nequeo.Net.Mail.MessageType emailType, 
            Action<Nequeo.Threading.AsyncOperationResult<bool>> callback, object state = null)
        {
            string keyName = "SendEmail_1";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<bool>(
                u => u.SendEmail
                    (
                        emailAdapter, from,
                        emailTo, subject, emailCCs,
                        emailBCCs, replyTo, textMessage, htmlMessage,
                        richTextMessage, xmlMessage, attachments,
                        linkedResources, priority,
                        emailType
                    ), keyName);
        }

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="message">The message body.</param>
        /// <param name="isText">True if the message body is text else html.</param>
        /// <returns>True if the email was sent to the recipients else false.</returns>
        /// <exception cref="System.Exception"></exception>
        public virtual bool SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string message, bool isText)
        {
            if(isText)
                return SendEmailEx(emailAdapter, from, emailTo, subject, null, null, null,
                    message, null, null, null, null, MailPriority.Normal, Nequeo.Net.Mail.MessageType.Text, null);
            else
                return SendEmailEx(emailAdapter, from, emailTo, subject, null, null, null,
                    null, message, null, null, null, MailPriority.Normal, Nequeo.Net.Mail.MessageType.Html, null);
        }

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="message">The message body.</param>
        /// <param name="isText">True if the message body is text else html.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        /// <exception cref="System.Exception"></exception>
        public virtual void SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string message, bool isText,
            Action<Nequeo.Threading.AsyncOperationResult<bool>> callback, object state = null)
        {
            string keyName = "SendEmail_2";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<bool>(
                u => u.SendEmail
                    (
                        emailAdapter, from,
                        emailTo, subject, message, isText
                    ), keyName);
        }

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="message">The message body.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="linkedResources">The collection of linked resource to sent, used for embeded images in html.</param>
        /// <param name="isText">True if the message body is text else html.</param>
        /// <returns>True if the email was sent to the recipients else false.</returns>
        /// <exception cref="System.Exception"></exception>
        public virtual bool SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string message,
            List<Attachment> attachments, List<LinkedResource> linkedResources, bool isText)
        {
            if (isText)
                return SendEmailEx(emailAdapter, from, emailTo, subject, null, null, null,
                    message, null, null, null, attachments, MailPriority.Normal,
                    Nequeo.Net.Mail.MessageType.Text, null);
            else
                return SendEmailEx(emailAdapter, from, emailTo, subject, null, null, null,
                    null, message, null, null, attachments, MailPriority.Normal,
                    Nequeo.Net.Mail.MessageType.Html, linkedResources);
        }

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="message">The message body.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="linkedResources">The collection of linked resource to sent, used for embeded images in html.</param>
        /// <param name="isText">True if the message body is text else html.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        /// <exception cref="System.Exception"></exception>
        public virtual void SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string message,
            List<Attachment> attachments, List<LinkedResource> linkedResources, bool isText,
            Action<Nequeo.Threading.AsyncOperationResult<bool>> callback, object state = null)
        {
            string keyName = "SendEmail_3";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<bool>(
                u => u.SendEmail
                    (
                        emailAdapter, from,
                        emailTo, subject, message,
                        attachments, linkedResources, isText
                    ), keyName);
        }

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <returns>True if the email was sent to the recipients else false.</returns>
        /// <exception cref="System.Exception"></exception>
        public virtual bool SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string textMessage, string htmlMessage)
        {
            return SendEmailEx(emailAdapter, from, emailTo, subject, null, null, null,
                textMessage, htmlMessage, null, null, null, MailPriority.Normal,
                Nequeo.Net.Mail.MessageType.TextAndHtml, null);
        }

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        /// <exception cref="System.Exception"></exception>
        public virtual void SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string textMessage, string htmlMessage,
            Action<Nequeo.Threading.AsyncOperationResult<bool>> callback, object state = null)
        {
            string keyName = "SendEmail_4";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<bool>(
                u => u.SendEmail
                    (
                        emailAdapter, from,
                        emailTo, subject, textMessage, htmlMessage
                    ), keyName);
        }

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="linkedResources">The collection of linked resource to sent, used for embeded images in html.</param>
        /// <returns>True if the email was sent to the recipients else false.</returns>
        /// <exception cref="System.Exception"></exception>
        public virtual bool SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string textMessage, string htmlMessage, 
            List<Attachment> attachments, List<LinkedResource> linkedResources)
        {
            return SendEmailEx(emailAdapter, from, emailTo, subject, null, null, null,
                textMessage, htmlMessage, null, null, attachments, MailPriority.Normal,
                Nequeo.Net.Mail.MessageType.TextAndHtml, linkedResources);
        }

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="linkedResources">The collection of linked resource to sent, used for embeded images in html.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        /// <exception cref="System.Exception"></exception>
        public virtual void SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string textMessage, string htmlMessage,
            List<Attachment> attachments, List<LinkedResource> linkedResources,
            Action<Nequeo.Threading.AsyncOperationResult<bool>> callback, object state = null)
        {
            string keyName = "SendEmail_5";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<bool>(
                u => u.SendEmail
                    (
                        emailAdapter, from,
                        emailTo, subject, textMessage, htmlMessage,
                        attachments, linkedResources
                    ), keyName);
        }

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="xmlMessage">The text version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <returns>True if the email was sent to the recipients else false.</returns>
        /// <exception cref="System.Exception"></exception>
        public virtual bool SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string xmlMessage,
            List<Attachment> attachments)
        {
            return SendEmailEx(emailAdapter, from, emailTo, subject, null, null, null,
                null, null, null, xmlMessage, attachments, MailPriority.Normal,
                Nequeo.Net.Mail.MessageType.Xml, null);
        }

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="xmlMessage">The text version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        /// <exception cref="System.Exception"></exception>
        public virtual void SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string xmlMessage,
            List<Attachment> attachments,
            Action<Nequeo.Threading.AsyncOperationResult<bool>> callback, object state = null)
        {
            string keyName = "SendEmail_6";
            _callback[keyName] = callback;
            _state[keyName] = state;
            _asyncAccount.Execute<bool>(
                u => u.SendEmail
                    (
                        emailAdapter, from,
                        emailTo, subject, xmlMessage,
                        attachments
                    ), keyName);
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
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
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_emailAdapter != null)
                        _emailAdapter.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _emailAdapter = null;

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~Emailer()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// General emailer class.
    /// </summary>
    public interface IEmailer
    {
        #region Public Properties
        /// <summary>
        /// Get, the secure certificate..
        /// </summary>
        X509Certificate2Info Certificate { get; }

        /// <summary>
        /// Gets the current async exception; else null;
        /// </summary>
        Exception ExceptionEmailer { get; }

        #endregion

        #region Public Methods
        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="emailCCs">The collection of carbon copies recipients.</param>
        /// <param name="emailBCCs">The collection of blind carbon copies recipients.</param>
        /// <param name="replyTo">The reply to email address for the message.</param>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <param name="richTextMessage">The rich text version of the email.</param>
        /// <param name="xmlMessage">The xml version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="linkedResources">The collection of linked resource to sent, used for embeded images in html.</param>
        /// <param name="priority">The priority of the email message.</param>
        /// <param name="emailType">The email format to sent as.</param>
        /// <returns>True if the email was sent to the recipients else false.</returns>
        /// <exception cref="System.Exception"></exception>
        bool SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, List<MailAddress> emailCCs,
            List<MailAddress> emailBCCs, List<MailAddress> replyTo, string textMessage, string htmlMessage,
            string richTextMessage, string xmlMessage, List<Attachment> attachments, List<LinkedResource> linkedResources,
            MailPriority priority, Nequeo.Net.Mail.MessageType emailType);

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="emailCCs">The collection of carbon copies recipients.</param>
        /// <param name="emailBCCs">The collection of blind carbon copies recipients.</param>
        /// <param name="replyTo">The reply to email address for the message.</param>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <param name="richTextMessage">The rich text version of the email.</param>
        /// <param name="xmlMessage">The xml version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="linkedResources">The collection of linked resource to sent, used for embeded images in html.</param>
        /// <param name="priority">The priority of the email message.</param>
        /// <param name="emailType">The email format to sent as.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        /// <exception cref="System.Exception"></exception>
        void SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, List<MailAddress> emailCCs,
            List<MailAddress> emailBCCs, List<MailAddress> replyTo, string textMessage, string htmlMessage,
            string richTextMessage, string xmlMessage, List<Attachment> attachments, List<LinkedResource> linkedResources,
            MailPriority priority, Nequeo.Net.Mail.MessageType emailType,
            Action<Nequeo.Threading.AsyncOperationResult<bool>> callback, object state = null);

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="message">The message body.</param>
        /// <param name="isText">True if the message body is text else html.</param>
        /// <returns>True if the email was sent to the recipients else false.</returns>
        /// <exception cref="System.Exception"></exception>
        bool SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string message, bool isText);

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="message">The message body.</param>
        /// <param name="isText">True if the message body is text else html.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        /// <exception cref="System.Exception"></exception>
        void SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string message, bool isText,
            Action<Nequeo.Threading.AsyncOperationResult<bool>> callback, object state = null);

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="message">The message body.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="linkedResources">The collection of linked resource to sent, used for embeded images in html.</param>
        /// <param name="isText">True if the message body is text else html.</param>
        /// <returns>True if the email was sent to the recipients else false.</returns>
        /// <exception cref="System.Exception"></exception>
        bool SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string message,
            List<Attachment> attachments, List<LinkedResource> linkedResources, bool isText);

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="message">The message body.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="linkedResources">The collection of linked resource to sent, used for embeded images in html.</param>
        /// <param name="isText">True if the message body is text else html.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        /// <exception cref="System.Exception"></exception>
        void SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string message,
            List<Attachment> attachments, List<LinkedResource> linkedResources, bool isText,
            Action<Nequeo.Threading.AsyncOperationResult<bool>> callback, object state = null);

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <returns>True if the email was sent to the recipients else false.</returns>
        /// <exception cref="System.Exception"></exception>
        bool SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string textMessage, string htmlMessage);

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        /// <exception cref="System.Exception"></exception>
        void SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string textMessage, string htmlMessage,
            Action<Nequeo.Threading.AsyncOperationResult<bool>> callback, object state = null);

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="linkedResources">The collection of linked resource to sent, used for embeded images in html.</param>
        /// <returns>True if the email was sent to the recipients else false.</returns>
        /// <exception cref="System.Exception"></exception>
        bool SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string textMessage, string htmlMessage,
            List<Attachment> attachments, List<LinkedResource> linkedResources);

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="linkedResources">The collection of linked resource to sent, used for embeded images in html.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        /// <exception cref="System.Exception"></exception>
        void SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string textMessage, string htmlMessage,
            List<Attachment> attachments, List<LinkedResource> linkedResources,
            Action<Nequeo.Threading.AsyncOperationResult<bool>> callback, object state = null);

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="xmlMessage">The text version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <returns>True if the email was sent to the recipients else false.</returns>
        /// <exception cref="System.Exception"></exception>
        bool SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string xmlMessage,
            List<Attachment> attachments);

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="xmlMessage">The text version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="callback">The callback action handler.</param>
        /// <param name="state">The action state.</param>
        /// <exception cref="System.Exception"></exception>
        void SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, string xmlMessage,
            List<Attachment> attachments,
            Action<Nequeo.Threading.AsyncOperationResult<bool>> callback, object state = null);

        #endregion
    }

    /// <summary>
    /// Class contains properties that hold all the
    /// connection collection for the specified server.
    /// </summary>
    [Serializable]
    public class EmailerConnectionAdapter : IEmailerConnectionAdapter, IDisposable
    {
        #region Constructors
        /// <summary>
        /// The emailer connection adapter for emailing information.
        /// </summary>
        /// <param name="server">The mail server.</param>
        /// <param name="port">The mail server port (25 (MTA) Mail Transport Agent: 587 (MSA) Mail Submission Agent).</param>
        /// <param name="userName">The smtp account username.</param>
        /// <param name="password">The smtp account password.</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        public EmailerConnectionAdapter(string server, int port,
            string userName, string password, int timeOut = -1)
        {
            this.server = server;
            this.port = port;
            this.userName = userName;
            this.password = password;
            this.timeOut = timeOut;
        }

        /// <summary>
        /// The emailer connection adapter for emailing information.
        /// </summary>
        /// <param name="server">The mail server.</param>
        /// <param name="userName">The smtp account username.</param>
        /// <param name="password">The smtp account password.</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        public EmailerConnectionAdapter(string server,
            string userName, string password, int timeOut = -1)
        {
            this.server = server;
            this.userName = userName;
            this.password = password;
            this.timeOut = timeOut;
        }

        /// <summary>
        /// The emailer connection adapter for emailing information.
        /// </summary>
        /// <param name="server">The mail server.</param>
        /// <param name="port">The mail server port (25 (MTA) Mail Transport Agent: 587 (MSA) Mail Submission Agent).</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        public EmailerConnectionAdapter(string server, int port = 25, int timeOut = -1)
        {
            this.server = server;
            this.port = port;
            this.timeOut = timeOut;
        }

        /// <summary>
        /// The emailer connection adapter for emailing information.
        /// </summary>
        /// <param name="server">The mail server.</param>
        /// <param name="timeOut">The time out for connection (-1 for infinity).</param>
        public EmailerConnectionAdapter(string server, int timeOut = -1)
        {
            this.server = server;
            this.timeOut = timeOut;
        }
        #endregion

        #region Private Fields
        private string userName = string.Empty;
        private string password = string.Empty;
        private SecureString securePassword;
        private Nequeo.Cryptography.IPasswordEncryption passwordEncryption;
        private string server = string.Empty;
        private string domain = string.Empty;

        /// <summary>
        /// (25 (MTA) Mail Transport Agent: 587 (MSA) Mail Submission Agent)
        /// </summary>
        private int port = 25;

        private int timeOut = -1;
        private bool useSSLConnection = false;
        private bool disposed = false;
        private bool validateCertificate = false;
        private X509Certificate2 _clientCertificate = null;
        private X509Certificate2 _signing = null;
        private X509Certificate2 _encryption = null;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get set, the client certificate.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public X509Certificate2 ClientCertificate
        {
            get { return _clientCertificate; }
            set { _clientCertificate = value; }
        }

        /// <summary>
        /// Gets or sets the signing certificate; if not null then certificate is always used.
        /// </summary>
        public X509Certificate2 SigningCertificate
        {
            get { return _signing; }
            set { _signing = value; }
        }

        /// <summary>
        /// Gets or sets the encryption certificate; if not null then certificate is always used.
        /// </summary>
        public X509Certificate2 EncryptionCertificate
        {
            get { return _encryption; }
            set { _encryption = value; }
        }

        /// <summary>
        /// Gets or sets the password encryption provider.
        /// </summary>
        public Nequeo.Cryptography.IPasswordEncryption PasswordEncryption 
        {
            get { return passwordEncryption; }
            set { passwordEncryption = value; }
        }

        /// <summary>
        /// Gets or sets the secure password.
        /// </summary>
        public SecureString SecurePassword 
        {
            get { return securePassword; }
            set { securePassword = value; }
        }

        /// <summary>
        /// Get set, should the ssl/tsl certificate be veryfied
        /// when making a secure connection.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public bool ValidateCertificate
        {
            get { return validateCertificate; }
            set { validateCertificate = value; }
        }

        /// <summary>
        /// Get Set, the user name.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// Get Set, the password.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        /// <summary>
        /// Get Set, the domain.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }

        /// <summary>
        /// Get Set, the server.
        /// </summary>
        [XmlElement(IsNullable = true)]
        public string Server
        {
            get { return server; }
            set { server = value; }
        }

        /// <summary>
        /// Get Set, the server port (25 (MTA) Mail Transport Agent: 587 (MSA) Mail Submission Agent).
        /// </summary>
        [XmlElement(IsNullable = false)]
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        /// <summary>
        /// Get Set, the time out request.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public int TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }

        /// <summary>
        /// Get Set, use ssl encryption transfer.
        /// </summary>
        [XmlElement(IsNullable = false)]
        public bool UseSSLConnection
        {
            get { return useSSLConnection; }
            set { useSSLConnection = value; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Load the client certificate.
        /// </summary>
        /// <param name="fileName">The path to the certificate.</param>
        public void LoadClientCertificate(string fileName)
        {
            _clientCertificate = (X509Certificate2)X509Certificate2.CreateFromCertFile(fileName);
        }

        /// <summary>
        /// Load the client certificate.
        /// </summary>
        /// <param name="storeName">The store name to search in.</param>
        /// <param name="storeLocation">The store location to search in.</param>
        /// <param name="x509FindType">The type of data to find on.</param>
        /// <param name="findValue">The value to find in the certificate.</param>
        /// <param name="validOnly">Search for only valid certificates.</param>
        public void LoadClientCertificate(
            StoreName storeName,
            StoreLocation storeLocation,
            X509FindType x509FindType,
            object findValue,
            bool validOnly)
        {
            _clientCertificate = X509Certificate2Store.GetCertificate(
                storeName, storeLocation, x509FindType, findValue, validOnly);
        }

        /// <summary>
        /// Set the default SecurePassword from the PasswordEncryption Decrypt method.
        /// </summary>
        public void SetDefaultSecurePassword()
        {
            if (passwordEncryption != null)
            {
                // Get the encoded and encrypted password.
                string password = passwordEncryption.Decrypt(Password);

                // Get the certificate path details and create
                // the x509 certificate reference.
                securePassword = new Nequeo.Security.SecureText().GetSecureText(password);
            }
        }

        /// <summary>
        /// Set the default SecurePassword from the PasswordEncryption Decrypt method.
        /// </summary>
        /// <param name="key">The key used to decrypt the password.</param>
        public void SetDefaultSecurePassword(string key)
        {
            if (passwordEncryption != null)
            {
                // Get the encoded and encrypted password.
                string password = passwordEncryption.Decrypt(Password, key);

                // Get the certificate path details and create
                // the x509 certificate reference.
                securePassword = new Nequeo.Security.SecureText().GetSecureText(password);
            }
        }
        #endregion

        #region Dispose Object Methods
        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
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
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                userName = null;
                password = null;
                server = null;

                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~EmailerConnectionAdapter()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// Smtp model.
    /// </summary>
    public class SmtpModel
    {
        /// <summary>
        /// Gets or sets the email connection adapter.
        /// </summary>
        public EmailerConnectionAdapter Adapter { get; set; }

        /// <summary>
        /// Gets or sets the email address of the sender.
        /// </summary>
        public string From { get; set; }
    }

    /// <summary>
    /// SMTP connection interface.
    /// </summary>
    public interface IEmailerConnectionAdapter
    {
        #region Public Properies
        /// <summary>
        /// Get Set, the user name.
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// Get Set, the password.
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Get Set, the server.
        /// </summary>
        string Server { get; set; }

        /// <summary>
        /// Get Set, the server port.
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// Get Set, the time out request.
        /// </summary>
        int TimeOut { get; set; }

        #endregion
    }

    /// <summary>
    /// MEF enabled emailer
    /// </summary>
    public class EmailerComposer
    {
        private bool _composed = false;
        private bool _emailer = true;

        /// <summary>
        /// Emailer interface
        /// </summary>
        [Import]
        internal IEmailer Emailer { get; set; }

        /// <summary>
        /// Send email action.
        /// </summary>
        [Import("SendConstructedEmail")]
        internal Func<
            EmailerConnectionAdapter, MailAddress,
            List<MailAddress>, string, List<MailAddress>,
            List<MailAddress>, List<MailAddress>, string, string,
            string, string, List<Attachment>, MailPriority,
            Nequeo.Net.Mail.MessageType, Boolean
        > SendEmailAction { get; set; }

        /// <summary>
        /// Compose the MEF instance.
        /// </summary>
        public void Compose(bool useEmailInterface = true)
        {
            AggregateCatalog catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetAssembly(typeof(Emailer))));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            _composed = true;
            _emailer = useEmailInterface;
        }

        /// <summary>
        /// Sends an email in the specified mail format.
        /// </summary>
        /// <param name="emailAdapter">The emailer connection adapter.</param>
        /// <param name="from">The email address of the sender.</param>
        /// <param name="emailTo">The collection of recipients.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="emailCCs">The collection of carbon copies recipients.</param>
        /// <param name="emailBCCs">The collection of blind carbon copies recipients.</param>
        /// <param name="replyTo">The reply to email address for the message.</param>
        /// <param name="textMessage">The text version of the email.</param>
        /// <param name="htmlMessage">The html version of the email.</param>
        /// <param name="richTextMessage">The rich text version of the email.</param>
        /// <param name="xmlMessage">The xml version of the email.</param>
        /// <param name="attachments">The collection of attachments to sent.</param>
        /// <param name="linkedResources">The collection of linked resource to sent, used for embeded images in html.</param>
        /// <param name="priority">The priority of the email message.</param>
        /// <param name="emailType">The email format to sent as.</param>
        /// <returns>True if the email was sent to the recipients else false.</returns>
        /// <exception cref="System.Exception"></exception>
        public void SendEmail(EmailerConnectionAdapter emailAdapter, MailAddress from,
            List<MailAddress> emailTo, string subject, List<MailAddress> emailCCs,
            List<MailAddress> emailBCCs, List<MailAddress> replyTo, string textMessage, string htmlMessage,
            string richTextMessage, string xmlMessage, List<Attachment> attachments,
            List<LinkedResource> linkedResources, MailPriority priority,
            Nequeo.Net.Mail.MessageType emailType)
        {
            // Call compose first.
            if (!_composed)
                throw new Exception("Call the 'Compose' method first.");

            if (_emailer)
                // Send the email through the interface composer.
                Emailer.SendEmail(emailAdapter, from,
                    emailTo, subject, emailCCs,
                    emailBCCs, replyTo, textMessage, htmlMessage,
                    richTextMessage, xmlMessage, attachments, linkedResources, priority,
                    emailType);
            else
                // Send the email through the action composer.
                SendEmailAction(emailAdapter,  from,
                    emailTo,  subject,  emailCCs,
                    emailBCCs,  replyTo,  textMessage,  htmlMessage,
                    richTextMessage,  xmlMessage,  attachments,  priority,
                    emailType);
        }
    }
}
