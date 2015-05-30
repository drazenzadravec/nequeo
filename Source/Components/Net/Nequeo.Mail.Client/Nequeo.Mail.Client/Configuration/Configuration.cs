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
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Net.Security;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Xml;
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

namespace Nequeo.Net.Mail.Configuration
{
    /// <summary>
    /// Class that contains the default host data
    /// within the configuration file.
    /// </summary>
    public class WebMail : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public WebMail()
        {
        }

        /// <summary>
        /// Gets sets, the smtp section attributes.
        /// </summary>
        [ConfigurationProperty("Smtp")]
        public SmtpElement SmtpSection
        {
            get { return (SmtpElement)this["Smtp"]; }
            set { this["Smtp"] = value; }
        }

        /// <summary>
        /// Reads XML from the configuration file.
        /// </summary>
        /// <param name="reader">The System.Xml.XmlReader object, 
        /// which reads from the configuration file.</param>
        protected override void DeserializeSection(XmlReader reader)
        {
            base.DeserializeSection(reader);
        }

        /// <summary>
        /// Creates an XML string containing an unmerged view of the 
        /// System.Configuration.ConfigurationSection
        ///  object as a single section to write to a file.
        /// </summary>
        /// <param name="parentElement">The System.Configuration.ConfigurationElement 
        /// instance to use as the parent when performing the un-merge.</param>
        /// <param name="name">The name of the section to create.</param>
        /// <param name="saveMode">The System.Configuration.ConfigurationSaveMode 
        /// instance to use when writing to a string.</param>
        /// <returns>An XML string containing an unmerged view of the 
        /// System.Configuration.ConfigurationSection object.</returns>
        protected override string SerializeSection(
            ConfigurationElement parentElement,
            string name, ConfigurationSaveMode saveMode)
        {
            return base.SerializeSection(parentElement, name, saveMode);
        }
    }

    /// <summary>
    /// Class that contains all the smtp attributes.
    /// </summary>
    public class SmtpElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SmtpElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="from">The from attribute.</param>
        /// <param name="host">The host attribute.</param>
        /// <param name="port">The port attribute.</param>
        /// <param name="username">The username attribute.</param>
        /// <param name="password">The password attribute.</param>
        public SmtpElement(String from, String host, int port, String username, String password)
        {
            From = from;
            Host = host;
            Port = port;
            Username = username;
            Password = password;
        }

        /// <summary>
        /// Gets sets, the from attribute.
        /// </summary>
        [ConfigurationProperty("from", DefaultValue = "", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String From
        {
            get { return (String)this["from"]; }
            set { this["from"] = value; }
        }

        /// <summary>
        /// Gets sets, the host attribute.
        /// </summary>
        [ConfigurationProperty("host", DefaultValue = "", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String Host
        {
            get { return (String)this["host"]; }
            set { this["host"] = value; }
        }

        /// <summary>
        /// Gets sets, the port attribute.
        /// </summary>
        [ConfigurationProperty("port", DefaultValue = 25, IsRequired = false)]
        public int Port
        {
            get { return (int)this["port"]; }
            set { this["port"] = value; }
        }

        /// <summary>
        /// Gets sets, the username attribute.
        /// </summary>
        [ConfigurationProperty("username", DefaultValue = "", IsRequired = false)]
        [StringValidator(MinLength = 0)]
        public String Username
        {
            get { return (String)this["username"]; }
            set { this["username"] = value; }
        }

        /// <summary>
        /// Gets sets, the password attribute.
        /// </summary>
        [ConfigurationProperty("password", DefaultValue = "", IsRequired = false)]
        [StringValidator(MinLength = 0)]
        public String Password
        {
            get { return (String)this["password"]; }
            set { this["password"] = value; }
        }
    }

    /// <summary>
    /// Configuration reader
    /// </summary>
    public class Reader
    {
        /// <summary>
        /// Get the smtp host.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The smtp model.</returns>
        /// <exception cref="System.Exception">Configuration load exception is thrown.</exception>
        public SmtpModel GetSmtpHost(string section = "NequeoMailGroup/NequeoNetMail")
        {
            SmtpModel encoder = null;

            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                WebMail defaultEncoder =
                    (WebMail)System.Configuration.ConfigurationManager.GetSection(section);

                // Make sure the section is defined.
                if (defaultEncoder == null)
                    throw new Exception("Configuration section has not been defined.");

                // Get the encoder element.
                SmtpElement encoderElement = defaultEncoder.SmtpSection;
                if (encoderElement == null)
                    throw new Exception("Configuration element Smtp has not been defined.");

                // Create an instance of the encoder type.
                EmailerConnectionAdapter adapter = new EmailerConnectionAdapter(encoderElement.Host, encoderElement.Port);
                adapter.UserName = encoderElement.Username;
                adapter.Password = encoderElement.Password;

                // Create the smtp model.
                encoder = new SmtpModel();
                encoder.Adapter = adapter;
                encoder.From = encoderElement.From;
            }
            catch (Exception)
            {
                throw;
            }

            // Return the encoder.
            return encoder;
        }
    }
}
