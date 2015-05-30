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
using System.Collections;
using System.Configuration;
using System.Xml;

namespace Nequeo.Handler.Configuration
{
    #region Base Handler Configuration
    /// <summary>
    /// Class that contains the default base handler data
    /// within the configuration file.
    /// </summary>
    internal class BaseHandlerDefaultSection : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseHandlerDefaultSection()
        {
        }

        /// <summary>
        /// Constructor with base handler attribute.
        /// </summary>
        /// <param name="baseHandlerName">The base handler name attribute.</param>
        public BaseHandlerDefaultSection(String baseHandlerName)
        {
            BaseHandlerName = baseHandlerName;
        }

        /// <summary>
        /// Gets sets, the base handler name attribute value.
        /// </summary>
        [ConfigurationProperty("baseHandlerName", DefaultValue = "BaseHandler", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 0, MaxLength = 100)]
        public String BaseHandlerName
        {
            get { return (String)this["baseHandlerName"]; }
            set { this["baseHandlerName"] = value; }
        }

        /// <summary>
        /// Gets sets, the base handler section attributes.
        /// </summary>
        [ConfigurationProperty("BaseHandler")]
        public BaseHandlerElement BaseHandlerSection
        {
            get { return (BaseHandlerElement)this["BaseHandler"]; }
            set { this["BaseHandler"] = value; }
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
    /// Class that contains all the base handler attributes.
    /// </summary>
    public class BaseHandlerElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseHandlerElement()
        {
        }

        /// <summary>
        /// Constructor with base handler attributes
        /// </summary>
        /// <param name="processLogFilePathIsRelative">The Process Log File Path Is Relative attribute</param>
        /// <param name="processLogFilePath">The Process LogFile Path attribute</param>
        /// <param name="errorLogFilePathIsRelative">The Error Log File Path Is Relative attribute</param>
        /// <param name="errorLogFilePath">The Error Log File Path attribute.</param>
        /// <param name="sessionStatePathIsRelative">The Session State Path Is Relative attribute</param>
        /// <param name="sessionStatePath">The Session State Path attribute.</param>
        /// <param name="membershipCacheTimeOut">The Membership Cache Time Out attribute.</param>
        /// <param name="processLogFileMaxSize">The Process Log File Max Size attribute.</param>
        /// <param name="errorLogFileMaxSize">The Error Log File Max Size attribute.</param>
        public BaseHandlerElement(Boolean processLogFilePathIsRelative, String processLogFilePath,
            Boolean errorLogFilePathIsRelative, String errorLogFilePath, Boolean sessionStatePathIsRelative,
            String sessionStatePath, Int32 membershipCacheTimeOut, Int32 processLogFileMaxSize,
            Int32 errorLogFileMaxSize)
        {
            ProcessLogFilePathIsRelative = processLogFilePathIsRelative;
            ProcessLogFilePath = processLogFilePath;
            ErrorLogFilePathIsRelative = errorLogFilePathIsRelative;
            ErrorLogFilePath = errorLogFilePath;
            SessionStatePathIsRelative = sessionStatePathIsRelative;
            SessionStatePath = sessionStatePath;
            MembershipCacheTimeOut = membershipCacheTimeOut;
            ProcessLogFileMaxSize = processLogFileMaxSize;
            ErrorLogFileMaxSize = errorLogFileMaxSize;
        }

        /// <summary>
        /// Gets sets, the Process Log File Path Is Relative attribute.
        /// </summary>
        [ConfigurationProperty("processLogFilePathIsRelative", DefaultValue = "true", IsRequired = true)]
        public Boolean ProcessLogFilePathIsRelative
        {
            get { return (Boolean)this["processLogFilePathIsRelative"]; }
            set { this["processLogFilePathIsRelative"] = value; }
        }

        /// <summary>
        /// Gets sets, the Process LogFile Path attribute.
        /// </summary>
        [ConfigurationProperty("processLogFilePath", DefaultValue = "Log\\Process", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|", MinLength = 0, MaxLength = 500)]
        public String ProcessLogFilePath
        {
            get { return (String)this["processLogFilePath"]; }
            set { this["processLogFilePath"] = value; }
        }

        /// <summary>
        /// Gets sets, the Process Log File Max Size attribute.
        /// </summary>
        [ConfigurationProperty("processLogFileMaxSize", DefaultValue = "1", IsRequired = true)]
        [IntegerValidator(MinValue = 1)]
        public Int32 ProcessLogFileMaxSize
        {
            get { return (Int32)this["processLogFileMaxSize"]; }
            set { this["processLogFileMaxSize"] = value; }
        }

        /// <summary>
        /// Gets sets, the Error Log File Path Is Relative attribute.
        /// </summary>
        [ConfigurationProperty("errorLogFilePathIsRelative", DefaultValue = "true", IsRequired = true)]
        public Boolean ErrorLogFilePathIsRelative
        {
            get { return (Boolean)this["errorLogFilePathIsRelative"]; }
            set { this["errorLogFilePathIsRelative"] = value; }
        }

        /// <summary>
        /// Gets sets, the Error Log File Path attribute.
        /// </summary>
        [ConfigurationProperty("errorLogFilePath", DefaultValue = "Log\\Error", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|", MinLength = 0, MaxLength = 500)]
        public String ErrorLogFilePath
        {
            get { return (String)this["errorLogFilePath"]; }
            set { this["errorLogFilePath"] = value; }
        }

        /// <summary>
        /// Gets sets, the Error Log File Max Size attribute.
        /// </summary>
        [ConfigurationProperty("errorLogFileMaxSize", DefaultValue = "1", IsRequired = true)]
        [IntegerValidator(MinValue = 1)]
        public Int32 ErrorLogFileMaxSize
        {
            get { return (Int32)this["errorLogFileMaxSize"]; }
            set { this["errorLogFileMaxSize"] = value; }
        }

        /// <summary>
        /// Gets sets, the Session State Path Is Relative attribute.
        /// </summary>
        [ConfigurationProperty("sessionStatePathIsRelative", DefaultValue = "true", IsRequired = true)]
        public Boolean SessionStatePathIsRelative
        {
            get { return (Boolean)this["sessionStatePathIsRelative"]; }
            set { this["sessionStatePathIsRelative"] = value; }
        }

        /// <summary>
        /// Gets sets, the Session State Path attribute.
        /// </summary>
        [ConfigurationProperty("sessionStatePath", DefaultValue = "App_Data\\Session_Data", IsRequired = true)]
        [StringValidator(InvalidCharacters = "!@#$%^&*()[]{};'\"|", MinLength = 0, MaxLength = 500)]
        public String SessionStatePath
        {
            get { return (String)this["sessionStatePath"]; }
            set { this["sessionStatePath"] = value; }
        }

        /// <summary>
        /// Gets sets, the Membership Cache Time Out attribute.
        /// </summary>
        [ConfigurationProperty("membershipCacheTimeOut", DefaultValue = "86400", IsRequired = true)]
        [IntegerValidator(MinValue = -1)]
        public Int32 MembershipCacheTimeOut
        {
            get { return (Int32)this["membershipCacheTimeOut"]; }
            set { this["membershipCacheTimeOut"] = value; }
        }
    }

    #endregion

    #region Base Type Handler Configuration
    /// <summary>
    /// Class that contains the collection of all extension
    /// data within the configuration file.
    /// </summary>
    public class BaseTypeHandlerDefaultSection : ConfigurationSection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseTypeHandlerDefaultSection()
        {
        }

        /// <summary>
        /// Gets sets, the extension collection type.
        /// </summary>
        [ConfigurationProperty("Extension")]
        public BaseTypeHandlerCollection HostSection
        {
            get { return (BaseTypeHandlerCollection)this["Extension"]; }
            set { this["Extension"] = value; }
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
    /// Class that contains all the extension attributes.
    /// </summary>
    public class BaseTypeHandlerElement : ConfigurationElement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseTypeHandlerElement()
        {
        }

        /// <summary>
        /// Constructor with extension attributes
        /// </summary>
        /// <param name="name">The name attribute.</param>
        /// <param name="assemblyName">The assembly name attribute.</param>
        /// <param name="typeName">The type name attribute.</param>
        /// <param name="typeMemberName">The type member name attribute.</param>
        /// <param name="writeTo">The write to attribute.</param>
        /// <param name="logType">The log type to attribute.</param>
        public BaseTypeHandlerElement(String name, String assemblyName, String typeName,
            String typeMemberName, Nequeo.Handler.WriteTo writeTo, Nequeo.Handler.LogType logType)
        {
            Name = name;
            AssemblyName = assemblyName;
            TypeName = typeName;
            TypeMemberName = typeMemberName;
            WriteTo = writeTo;
            LogType = logType;
        }

        /// <summary>
        /// Gets sets, the name attribute.
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "default", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1)]
        public String Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets sets, the assembly name attribute.
        /// </summary>
        [ConfigurationProperty("assemblyName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 1)]
        public String AssemblyName
        {
            get { return (String)this["assemblyName"]; }
            set { this["assemblyName"] = value; }
        }

        /// <summary>
        /// Gets sets, the type name attribute.
        /// </summary>
        [ConfigurationProperty("typeName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 1)]
        public String TypeName
        {
            get { return (String)this["typeName"]; }
            set { this["typeName"] = value; }
        }

        /// <summary>
        /// Gets sets, the type member name attribute.
        /// </summary>
        [ConfigurationProperty("typeMemberName", DefaultValue = "default", IsRequired = true)]
        [StringValidator(MinLength = 1)]
        public String TypeMemberName
        {
            get { return (String)this["typeMemberName"]; }
            set { this["typeMemberName"] = value; }
        }

        /// <summary>
        /// Gets sets, the write to attribute.
        /// </summary>
        [ConfigurationProperty("writeTo", DefaultValue = "Default", IsRequired = true)]
        public Nequeo.Handler.WriteTo WriteTo
        {
            get { return (Nequeo.Handler.WriteTo)this["writeTo"]; }
            set { this["writeTo"] = value; }
        }

        /// <summary>
        /// Gets sets, the log type to attribute.
        /// </summary>
        [ConfigurationProperty("logType", DefaultValue = "Error", IsRequired = true)]
        public Nequeo.Handler.LogType LogType
        {
            get { return (Nequeo.Handler.LogType)this["logType"]; }
            set { this["logType"] = value; }
        }
    }

    /// <summary>
    /// Class that contains all the exenstion elements
    /// found in the configuration file.
    /// </summary>
    public class BaseTypeHandlerCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public BaseTypeHandlerCollection()
        {
            // Get the current host element.
            BaseTypeHandlerElement host =
                (BaseTypeHandlerElement)CreateNewElement();

            // Add the element to the collection.
            Add(host);
        }

        /// <summary>
        /// Create a new configuration element.
        /// </summary>
        /// <returns>A new host element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new BaseTypeHandlerElement();
        }

        /// <summary>
        /// Get the current element key.
        /// </summary>
        /// <param name="element">The current element.</param>
        /// <returns>The current host element key.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            // The current element key is the name attribute
            // of the host element.
            return ((BaseTypeHandlerElement)element).Name;
        }

        /// <summary>
        /// Add a new host element type to the collection.
        /// </summary>
        /// <param name="element">The current host element.</param>
        public void Add(BaseTypeHandlerElement element)
        {
            // Add the element to the base
            // ConfigurationElementCollection type.
            BaseAdd(element);
        }

        /// <summary>
        /// Indexer that gets the specified host element
        /// for the specified index.
        /// </summary>
        /// <param name="index">The index of the host element.</param>
        /// <returns>The current host element type.</returns>
        public BaseTypeHandlerElement this[int index]
        {
            get { return (BaseTypeHandlerElement)BaseGet(index); }
            set
            {
                // Remove the current host element
                // from the collection at this index.
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                // Add a new host element.
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Indexer that gets the specified host element
        /// for the specified key name.
        /// </summary>
        /// <param name="Name">The key name of the element.</param>
        /// <returns>The current host element type.</returns>
        new public BaseTypeHandlerElement this[string Name]
        {
            get { return (BaseTypeHandlerElement)BaseGet(Name); }
        }
    }
    #endregion

    /// <summary>
    /// Base handler section group configuration manager.
    /// </summary>
    public class BaseHandlerConfigurationManager
    {
        /// <summary>
        /// Gets, the base handler data default section elements.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The base handler; else null.</returns>
        public static BaseHandlerElement BaseHandlerElement(string section = "DataBaseHandlerGroup/BaseHandlerDefaultSection")
        {
            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Get the default element
                // configuration information
                // from the configuration manager.
                BaseHandlerDefaultSection baseHandler =
                    (BaseHandlerDefaultSection)System.Configuration.ConfigurationManager.GetSection(section);

                // Return the element.
                return baseHandler.BaseHandlerSection;
            }
            catch { }
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Base Type Handler Configuration Manager
    /// </summary>
    public class BaseTypeHandlerConfigurationManager
    {
        /// <summary>
        /// Gets, the Base type handler collection elements.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The base type handler; else null.</returns>
        public static BaseTypeHandlerCollection BaseTypeHandlerCollection(string section = "NequeoHandlerGroup/BaseTypeHandlerDefaultSection")
        {
            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Get the default element
                // configuration information
                // from the configuration manager.
                BaseTypeHandlerDefaultSection baseHandler =
                    (BaseTypeHandlerDefaultSection)System.Configuration.ConfigurationManager.GetSection(section);

                // Return the element.
                return baseHandler.HostSection;
            }
            catch { }
            {
                return null;
            }
        }

        /// <summary>
        /// Gets sets, the Base type handler elements.
        /// </summary>
        /// <param name="section">The config section group and section name.</param>
        /// <returns>The base type handler; else null.</returns>
        public static BaseTypeHandlerElement[] BaseTypeHandlerElements(string section = "NequeoHandlerGroup/BaseTypeHandlerDefaultSection")
        {
            try
            {
                // Refreshes the named section so the next time that it is retrieved it will be re-read from disk.
                System.Configuration.ConfigurationManager.RefreshSection(section);

                // Get the default element
                // configuration information
                // from the configuration manager.
                BaseTypeHandlerDefaultSection baseHandler =
                    (BaseTypeHandlerDefaultSection)System.Configuration.ConfigurationManager.GetSection(section);

                // Return the collection.
                BaseTypeHandlerElement[] items = new BaseTypeHandlerElement[baseHandler.HostSection.Count];
                baseHandler.HostSection.CopyTo(items, 0);
                return items.Where(q => (q.Name != "default")).ToArray();
            }
            catch { }
            {
                return null;
            }
        }
    }
}
