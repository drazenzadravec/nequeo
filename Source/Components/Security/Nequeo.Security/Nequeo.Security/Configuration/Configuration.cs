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
using System.Data;
using System.Configuration;
using System.Threading;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;

namespace Nequeo.Security.Configuration
{
    #region Host Security Configuration
    /// <summary>
    /// Class that contains the default host data
    /// within the configuration file.
    /// </summary>
    public class SecurityHost : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SecurityHost()
        {
        }

        /// <summary>
        /// Gets sets, the host section attributes.
        /// </summary>
        [ConfigurationProperty("ServerCredentials")]
        public ServerCredentialsElement ServerCredentialsSection
        {
            get { return (ServerCredentialsElement)this["ServerCredentials"]; }
            set { this["ServerCredentials"] = value; }
        }

        /// <summary>
        /// Gets sets, the host section attributes.
        /// </summary>
        [ConfigurationProperty("ServerCredentialsEncoded")]
        public ServerCredentialsEncodedElement ServerCredentialsEncodedSection
        {
            get { return (ServerCredentialsEncodedElement)this["ServerCredentialsEncoded"]; }
            set { this["ServerCredentialsEncoded"] = value; }
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
    /// Class that contains all the host attributes.
    /// </summary>
    public class ServerCredentialsEncodedElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServerCredentialsEncodedElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="useServerCertificate">The use server certificate attribute.</param>
        public ServerCredentialsEncodedElement(Boolean useServerCertificate)
        {
            UseServerCertificate = useServerCertificate;
        }

        /// <summary>
        /// Gets sets, the use server certificate attribute.
        /// </summary>
        [ConfigurationProperty("useServerCertificate", DefaultValue = "false", IsRequired = true)]
        public Boolean UseServerCertificate
        {
            get { return (Boolean)this["useServerCertificate"]; }
            set { this["useServerCertificate"] = value; }
        }

        /// <summary>
        /// Gets sets, the certificate path attribute.
        /// </summary>
        [ConfigurationProperty("CertificatePath")]
        public ServerCredentialsCertificatePathElement CertificatePath
        {
            get { return (ServerCredentialsCertificatePathElement)this["CertificatePath"]; }
            set { this["CertificatePath"] = value; }
        }
    }

    /// <summary>
    /// Class that contains all the host attributes.
    /// </summary>
    public class ServerCredentialsElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServerCredentialsElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="useCertificateStore">The use certificate store attribute.</param>
        /// <param name="useServerCertificate">The use server certificate attribute.</param>
        public ServerCredentialsElement(Boolean useCertificateStore, Boolean useServerCertificate)
        {
            UseCertificateStore = useCertificateStore;
            UseServerCertificate = useServerCertificate;
        }

        /// <summary>
        /// Gets sets, the use server certificate attribute.
        /// </summary>
        [ConfigurationProperty("useServerCertificate", DefaultValue = "false", IsRequired = true)]
        public Boolean UseServerCertificate
        {
            get { return (Boolean)this["useServerCertificate"]; }
            set { this["useServerCertificate"] = value; }
        }

        /// <summary>
        /// Gets sets, the use certificate store attribute.
        /// </summary>
        [ConfigurationProperty("useCertificateStore", DefaultValue = "true", IsRequired = true)]
        public Boolean UseCertificateStore
        {
            get { return (Boolean)this["useCertificateStore"]; }
            set { this["useCertificateStore"] = value; }
        }

        /// <summary>
        /// Gets sets, the certificate path attribute.
        /// </summary>
        [ConfigurationProperty("CertificatePath")]
        public ServerCredentialsCertificatePathElement CertificatePath
        {
            get { return (ServerCredentialsCertificatePathElement)this["CertificatePath"]; }
            set { this["CertificatePath"] = value; }
        }

        /// <summary>
        /// Gets sets, the certificate store attribute.
        /// </summary>
        [ConfigurationProperty("CertificateStore")]
        public ServerCredentialsCertificateStoreElement CertificateStore
        {
            get { return (ServerCredentialsCertificateStoreElement)this["CertificateStore"]; }
            set { this["CertificateStore"] = value; }
        }
    }

    /// <summary>
    /// Class that contains all the host attributes.
    /// </summary>
    public class ServerCredentialsCertificateStoreElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServerCredentialsCertificateStoreElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="findValue">The find value attribute.</param>
        /// <param name="storeLocation">The store location attribute.</param>
        /// <param name="storeName">The store name attribute.</param>
        /// <param name="x509FindType">The x509 find type attribute.</param>
        public ServerCredentialsCertificateStoreElement(String findValue, StoreLocation storeLocation,
            StoreName storeName, X509FindType x509FindType)
        {
            FindValue = findValue;
            StoreLocation = storeLocation;
            StoreName = storeName;
            X509FindType = x509FindType;
        }

        /// <summary>
        /// Gets sets, the find value attribute.
        /// </summary>
        [ConfigurationProperty("findValue", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String FindValue
        {
            get { return (String)this["findValue"]; }
            set { this["findValue"] = value; }
        }

        /// <summary>
        /// Gets sets, the store location attribute.
        /// </summary>
        [ConfigurationProperty("storeLocation", DefaultValue = "LocalMachine", IsRequired = true)]
        public StoreLocation StoreLocation
        {
            get { return (StoreLocation)this["storeLocation"]; }
            set { this["storeLocation"] = value; }
        }

        /// <summary>
        /// Gets sets, the store name attribute.
        /// </summary>
        [ConfigurationProperty("storeName", DefaultValue = "My", IsRequired = true)]
        public StoreName StoreName
        {
            get { return (StoreName)this["storeName"]; }
            set { this["storeName"] = value; }
        }

        /// <summary>
        /// Gets sets, the X509 find type attribute.
        /// </summary>
        [ConfigurationProperty("x509FindType", DefaultValue = "FindBySubjectName", IsRequired = true)]
        public X509FindType X509FindType
        {
            get { return (X509FindType)this["x509FindType"]; }
            set { this["x509FindType"] = value; }
        }
    }

    /// <summary>
    /// Class that contains all the host attributes.
    /// </summary>
    public class ServerCredentialsCertificatePathElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServerCredentialsCertificatePathElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="path">The path attribute.</param>
        /// <param name="password">The password attribute.</param>
        public ServerCredentialsCertificatePathElement(String path, String password)
        {
            Path = path;
            Password = password;
        }

        /// <summary>
        /// Gets sets, the path attribute.
        /// </summary>
        [ConfigurationProperty("path", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 1)]
        public String Path
        {
            get { return (String)this["path"]; }
            set { this["path"] = value; }
        }

        /// <summary>
        /// Gets sets, the password attribute.
        /// </summary>
        [ConfigurationProperty("password", DefaultValue = "", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String Password
        {
            get { return (String)this["password"]; }
            set { this["password"] = value; }
        }
    }
    #endregion

    #region User Security Configuration
    /// <summary>
    /// Class that contains the default host data
    /// within the configuration file.
    /// </summary>
    public class SecurityCredentials : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SecurityCredentials()
        {
        }

        /// <summary>
        /// Gets sets, the host section attributes.
        /// </summary>
        [ConfigurationProperty("UserCredentials")]
        public UserCredentialsElement UserCredentialsSection
        {
            get { return (UserCredentialsElement)this["UserCredentials"]; }
            set { this["UserCredentials"] = value; }
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
    /// Class that contains all the host attributes.
    /// </summary>
    public class UserCredentialsElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public UserCredentialsElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="username">The username attribute.</param>
        /// <param name="password">The password attribute.</param>
        /// <param name="domain">The domain attribute.</param>
        /// <param name="authorisationType">The authorisationType attribute.</param>
        /// <param name="authenticationType">The authenticationType attribute.</param>
        /// <param name="applicationName">The application name attribute.</param>
        public UserCredentialsElement(String username, String password, String domain,
            Nequeo.Security.AuthorisationType authorisationType, 
            Nequeo.Security.AuthenticationType authenticationType,
            String applicationName)
        {
            Username = username;
            Password = password;
            Domain = domain;
            AuthorisationType = authorisationType;
            AuthenticationType = authenticationType;
            ApplicationName = applicationName;
        }

        /// <summary>
        /// Gets sets, the username attribute.
        /// </summary>
        [ConfigurationProperty("username", DefaultValue = "username", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String Username
        {
            get { return (String)this["username"]; }
            set { this["username"] = value; }
        }

        /// <summary>
        /// Gets sets, the password attribute.
        /// </summary>
        [ConfigurationProperty("password", DefaultValue = "", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String Password
        {
            get { return (String)this["password"]; }
            set { this["password"] = value; }
        }

        /// <summary>
        /// Gets sets, the domain attribute.
        /// </summary>
        [ConfigurationProperty("domain", DefaultValue = "", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String Domain
        {
            get { return (String)this["domain"]; }
            set { this["domain"] = value; }
        }

        /// <summary>
        /// Gets sets, the authorisation type attribute.
        /// </summary>
        [ConfigurationProperty("authorisationType", DefaultValue = "None", IsRequired = true)]
        public Nequeo.Security.AuthorisationType AuthorisationType
        {
            get { return (Nequeo.Security.AuthorisationType)this["authorisationType"]; }
            set { this["authorisationType"] = value; }
        }

        /// <summary>
        /// Gets sets, the authentication type attribute.
        /// </summary>
        [ConfigurationProperty("authenticationType", DefaultValue = "None", IsRequired = true)]
        public Nequeo.Security.AuthenticationType AuthenticationType
        {
            get { return (Nequeo.Security.AuthenticationType)this["authenticationType"]; }
            set { this["authenticationType"] = value; }
        }

        /// <summary>
        /// Gets sets, the certificate path attribute.
        /// </summary>
        [ConfigurationProperty("AuthorisationCredentials")]
        public AuthorisationCredentialsElement AuthorisationCredentials
        {
            get { return (AuthorisationCredentialsElement)this["AuthorisationCredentials"]; }
            set { this["AuthorisationCredentials"] = value; }
        }

        /// <summary>
        /// Gets sets, the application name attribute.
        /// </summary>
        [ConfigurationProperty("applicationName", DefaultValue = "All", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String ApplicationName
        {
            get { return (String)this["applicationName"]; }
            set { this["applicationName"] = value; }
        }
    }

    /// <summary>
    /// Class that contains all the host attributes.
    /// </summary>
    public class AuthorisationCredentialsElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AuthorisationCredentialsElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="username">The username attribute.</param>
        /// <param name="password">The password attribute.</param>
        /// <param name="server">The server attribute.</param>
        /// <param name="secureConnection">The secureConnection attribute.</param>
        /// <param name="containerDN">The containerDN attribute.</param>
        public AuthorisationCredentialsElement(String username, String password, String server,
            Boolean secureConnection, String containerDN)
        {
            Username = username;
            Password = password;
            Server = server;
            SecureConnection = secureConnection;
            ContainerDN = containerDN;
        }

        /// <summary>
        /// Gets sets, the username attribute.
        /// </summary>
        [ConfigurationProperty("username", DefaultValue = "username", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String Username
        {
            get { return (String)this["username"]; }
            set { this["username"] = value; }
        }

        /// <summary>
        /// Gets sets, the password attribute.
        /// </summary>
        [ConfigurationProperty("password", DefaultValue = "", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String Password
        {
            get { return (String)this["password"]; }
            set { this["password"] = value; }
        }

        /// <summary>
        /// Gets sets, the server attribute.
        /// </summary>
        [ConfigurationProperty("server", DefaultValue = "", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String Server
        {
            get { return (String)this["server"]; }
            set { this["server"] = value; }
        }

        /// <summary>
        /// Gets sets, the secure connection attribute.
        /// </summary>
        [ConfigurationProperty("secureConnection", DefaultValue = "false", IsRequired = true)]
        public Boolean SecureConnection
        {
            get { return (Boolean)this["secureConnection"]; }
            set { this["secureConnection"] = value; }
        }

        /// <summary>
        /// Gets sets, the containerDN attribute.
        /// </summary>
        [ConfigurationProperty("containerDN", DefaultValue = "", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String ContainerDN
        {
            get { return (String)this["containerDN"]; }
            set { this["containerDN"] = value; }
        }
    }
    #endregion

    #region Password Encoder Configuration
    /// <summary>
    /// Class that contains the default host data
    /// within the configuration file.
    /// </summary>
    public class SecurityPassword : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SecurityPassword()
        {
        }

        /// <summary>
        /// Gets sets, the host section attributes.
        /// </summary>
        [ConfigurationProperty("Encoder")]
        public EncoderElement EncoderSection
        {
            get { return (EncoderElement)this["Encoder"]; }
            set { this["Encoder"] = value; }
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
    /// Class that contains all the host attributes.
    /// </summary>
    public class EncoderElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public EncoderElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="type">The type attribute.</param>
        /// <param name="passwordFormat">The password format attribute.</param>
        /// <param name="authorisationCode">The authorisation code attribute.</param>
        /// <param name="hashcodeType">The hashcode type attribute.</param>
        public EncoderElement(String type, Nequeo.Cryptography.PasswordFormat passwordFormat, String authorisationCode, Nequeo.Cryptography.HashcodeType hashcodeType)
        {
            TypeValue = type;
            PasswordFormat = passwordFormat;
            AuthorisationCode = authorisationCode;
            HashcodeType = hashcodeType;
        }

        /// <summary>
        /// Gets sets, the type attribute.
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = "", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String TypeValue
        {
            get { return (String)this["type"]; }
            set { this["type"] = value; }
        }

        /// <summary>
        /// Gets sets, the password format type attribute.
        /// </summary>
        [ConfigurationProperty("passwordFormat", DefaultValue = "Clear", IsRequired = true)]
        public Nequeo.Cryptography.PasswordFormat PasswordFormat
        {
            get { return (Nequeo.Cryptography.PasswordFormat)this["passwordFormat"]; }
            set { this["passwordFormat"] = value; }
        }

        /// <summary>
        /// Gets sets, the authorisation code attribute.
        /// </summary>
        [ConfigurationProperty("authorisationCode", DefaultValue = null, IsRequired = false)]
        public String AuthorisationCode
        {
            get { return (String)this["authorisationCode"]; }
            set { this["authorisationCode"] = value; }
        }

        /// <summary>
        /// Gets sets, the hashcode type attribute.
        /// </summary>
        [ConfigurationProperty("hashcodeType", DefaultValue = "SHA512", IsRequired = false)]
        public Nequeo.Cryptography.HashcodeType HashcodeType
        {
            get { return (Nequeo.Cryptography.HashcodeType)this["hashcodeType"]; }
            set { this["hashcodeType"] = value; }
        }
    }
    #endregion

    #region Permission Encoder Configuration
    /// <summary>
    /// Class that contains the default host data
    /// within the configuration file.
    /// </summary>
    public class SecurityPermission : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SecurityPermission()
        {
        }

        /// <summary>
        /// Gets sets, the host section attributes.
        /// </summary>
        [ConfigurationProperty("Source")]
        public SourceElement SourceSection
        {
            get { return (SourceElement)this["Source"]; }
            set { this["Source"] = value; }
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
    /// Class that contains all the host attributes.
    /// </summary>
    public class SourceElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SourceElement()
        {
        }

        /// <summary>
        /// Constructor with host attributes
        /// </summary>
        /// <param name="type">The type attribute.</param>
        /// <param name="permissionType">The permission type attribute.</param>
        public SourceElement(String type, Nequeo.Security.PermissionType permissionType)
        {
            TypeValue = type;
            PermissionType = permissionType;
        }

        /// <summary>
        /// Gets sets, the type attribute.
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = "", IsRequired = true)]
        [StringValidator(MinLength = 0)]
        public String TypeValue
        {
            get { return (String)this["type"]; }
            set { this["type"] = value; }
        }

        /// <summary>
        /// Gets sets, the permission type attribute.
        /// </summary>
        [ConfigurationProperty("permissionType", DefaultValue = "None", IsRequired = true)]
        public Nequeo.Security.PermissionType PermissionType
        {
            get { return (Nequeo.Security.PermissionType)this["permissionType"]; }
            set { this["permissionType"] = value; }
        }
    }
    #endregion

    /// <summary>
    /// Configuration reader
    /// </summary>
    public class Reader
    {
        /// <summary>
        /// Get the user credentials
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The user credentials.</returns>
        /// <exception cref="System.Exception">Configuration load exception is thrown.</exception>
        public Nequeo.Security.UserCredentials GetUserCredentials(string section = "NequeoSecurityGroup/NequeoSecurityCredentials")
        {
            Nequeo.Security.UserCredentials credentials = null;

            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                SecurityCredentials defaultHost =
                    (SecurityCredentials)System.Configuration.ConfigurationManager.GetSection(section);

                // Make sure the section is defined.
                if (defaultHost == null)
                    throw new Exception("Configuration section has not been defined.");

                // Get the user credetials element.
                UserCredentialsElement userCredentials = defaultHost.UserCredentialsSection;
                if (userCredentials == null)
                    throw new Exception("Configuration element UserCredentials has not been defined.");

                // Add the credentials.
                credentials = new UserCredentials()
                {
                    Username = userCredentials.Username,
                    Password = userCredentials.Password,
                    Domain = userCredentials.Domain,
                    AuthenticationType = userCredentials.AuthenticationType,
                    AuthorisationType = userCredentials.AuthorisationType,
                    ApplicationName = userCredentials.ApplicationName,
                };

                // Get the authorisation credetials element.
                AuthorisationCredentialsElement authorisationCredentials = userCredentials.AuthorisationCredentials;
                if (authorisationCredentials != null)
                {
                    // Add the credentials.
                    Nequeo.Security.AuthorisationCredentials authCredentials = new AuthorisationCredentials()
                    {
                        Username = authorisationCredentials.Username,
                        Password = authorisationCredentials.Password,
                        Server = authorisationCredentials.Server,
                        SecureConnection = authorisationCredentials.SecureConnection,
                        ContainerDN = authorisationCredentials.ContainerDN
                    };

                    // Add the credentials
                    credentials.AuthorisationCredentials = authCredentials;
                }
            }
            catch (Exception)
            {
                throw;
            }

            // Return the credentials.
            return credentials;
        }

        /// <summary>
        /// Get the server credentials from the certificate store or file.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The x509 certificate else null.</returns>
        /// <exception cref="System.Exception">Configuration load exception is thrown.</exception>
        public X509Certificate2 GetServerCredentials(string section = "NequeoSecurityGroup/NequeoSecurityHost")
        {
            X509Certificate2 certificate = null;

            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                SecurityHost defaultHost =
                    (SecurityHost)System.Configuration.ConfigurationManager.GetSection(section);

                // Make sure the section is defined.
                if (defaultHost == null)
                    throw new Exception("Configuration section has not been defined.");

                // Get the server credetials element.
                ServerCredentialsElement serverCredentials = defaultHost.ServerCredentialsSection;
                if (serverCredentials == null)
                    throw new Exception("Configuration element ServerCredentials has not been defined.");

                // Should a certificate be loaded.
                if (serverCredentials.UseServerCertificate)
                {
                    // If using the certificate store.
                    if (serverCredentials.UseCertificateStore)
                    {
                        // Get the certificate from the store.
                        ServerCredentialsCertificateStoreElement certificateStore = serverCredentials.CertificateStore;
                        if (certificateStore == null)
                            throw new Exception("Configuration element CertificateStore has not been defined.");

                        // Get the certificate refrence details from the certificate store.
                        certificate = X509Certificate2Store.GetCertificate(
                            certificateStore.StoreName,
                            certificateStore.StoreLocation,
                            certificateStore.X509FindType,
                            certificateStore.FindValue,
                            false);
                    }
                    else
                    {
                        // Get the certificate path
                        ServerCredentialsCertificatePathElement certificatePath = serverCredentials.CertificatePath;
                        if (certificatePath == null)
                            throw new Exception("Configuration element CertificatePath has not been defined.");

                        // Get the certificate path details and create
                        // the x509 certificate reference.
                        certificate = X509Certificate2Store.GetCertificate(certificatePath.Path, certificatePath.Password);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            // Return the certificate.
            return certificate;
        }

        /// <summary>
        /// Get the server credentials encoded from the certificate store or file.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The x509 certificate model else null.</returns>
        /// <exception cref="System.Exception">Configuration load exception is thrown.</exception>
        public X509Certificate2Model GetServerCredentialsEncoded(string section = "NequeoSecurityGroup/NequeoSecurityHost")
        {
            X509Certificate2Model certificate = null;

            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                SecurityHost defaultHost =
                    (SecurityHost)System.Configuration.ConfigurationManager.GetSection(section);

                // Make sure the section is defined.
                if (defaultHost == null)
                    throw new Exception("Configuration section has not been defined.");

                // Get the server credetials element.
                ServerCredentialsEncodedElement serverCredentials = defaultHost.ServerCredentialsEncodedSection;
                if (serverCredentials == null)
                    throw new Exception("Configuration element ServerCredentialsEncoded has not been defined.");

                // Get the certificate path
                ServerCredentialsCertificatePathElement certificatePath = serverCredentials.CertificatePath;
                if (certificatePath == null)
                    throw new Exception("Configuration element CertificatePath has not been defined.");

                // Create the X509 certificate model.
                certificate = new X509Certificate2Model()
                {
                    UseServerCertificate = serverCredentials.UseServerCertificate,
                    Path = certificatePath.Path,
                    Password = certificatePath.Password
                };
            }
            catch (Exception)
            {
                throw;
            }

            // Return the certificate.
            return certificate;
        }

        /// <summary>
        /// Get the password encoder.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The password encryption encoder.</returns>
        /// <exception cref="System.Exception">Configuration load exception is thrown.</exception>
        public Nequeo.Cryptography.IPasswordEncryption GetEncoder(string section = "NequeoSecurityGroup/NequeoSecurityPassword")
        {
            Nequeo.Cryptography.IPasswordEncryption encoder = null;

            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                SecurityPassword defaultEncoder =
                    (SecurityPassword)System.Configuration.ConfigurationManager.GetSection(section);

                // Make sure the section is defined.
                if (defaultEncoder == null)
                    throw new Exception("Configuration section has not been defined.");

                // Get the encoder element.
                EncoderElement encoderElement = defaultEncoder.EncoderSection;
                if (encoderElement == null)
                    throw new Exception("Configuration element Encoder has not been defined.");

                // Create an instance of the encoder type.
                Type ecoderType = Nequeo.Reflection.TypeAccessor.GetType(encoderElement.TypeValue);
                encoder = (Nequeo.Cryptography.IPasswordEncryption)Nequeo.Reflection.TypeAccessor.CreateInstance(ecoderType);
                encoder.PasswordFormat = encoderElement.PasswordFormat;
                encoder.AuthorisationCode = encoderElement.AuthorisationCode;
                encoder.HashcodeType = encoderElement.HashcodeType;
            }
            catch (Exception)
            {
                throw;
            }

            // Return the encoder.
            return encoder;
        }

        /// <summary>
        /// Get the permission source.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The permission source.</returns>
        /// <exception cref="System.Exception">Configuration load exception is thrown.</exception>
        public Nequeo.Security.IPermission GetPermission(string section = "NequeoSecurityGroup/NequeoSecurityPermission")
        {
            Nequeo.Security.IPermission encoder = null;

            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Create a new default host type
                // an load the values from the configuration
                // file into the default host type.
                SecurityPermission defaultEncoder =
                    (SecurityPermission)System.Configuration.ConfigurationManager.GetSection(section);

                // Make sure the section is defined.
                if (defaultEncoder == null)
                    throw new Exception("Configuration section has not been defined.");

                // Get the encoder element.
                SourceElement sourceElement = defaultEncoder.SourceSection;
                if (sourceElement == null)
                    throw new Exception("Configuration element Source has not been defined.");

                // Create an instance of the encoder type.
                Type ecoderType = Nequeo.Reflection.TypeAccessor.GetType(sourceElement.TypeValue);
                encoder = (Nequeo.Security.IPermission)Nequeo.Reflection.TypeAccessor.CreateInstance(ecoderType);
                encoder.Permission = sourceElement.PermissionType;
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
