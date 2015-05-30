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
using System.Xml;
using System.Text;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Generic;

namespace Nequeo.Handler.Global
{
    /// <summary>
    /// Class contains methods to retreive connection configuration
    /// information from the config file.
    /// </summary>
    public class DatabaseConnections : Nequeo.Handler.LogHandler, IDisposable, IDatabaseConnection
    {
        #region Constructors
        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public DatabaseConnections()
            : base(applicationName, eventNamespace)
        {
        }
        #endregion

        #region Private Constants
        private const string applicationName = "Nequeo";
        private const string eventNamespace = "Nequeo.Handler.Global";
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to retrieve the specified database
        /// connection key from the connection string section.
        /// </summary>
        /// <param name="configurationKey">The configuration key.</param>
        /// <param name="providerName">The provider type name.</param>
        /// <returns>The database connection string, else empty string.</returns>
        public virtual string DatabaseConnection(string configurationKey, out string providerName)
        {
            // Make sure the page reference exists.
            if (configurationKey == null) throw new ArgumentNullException("configurationKey");

            providerName = null;

            try
            {
                // Get the current database connection
                // string from the configuration file.
                string dbConnectionString = ConnectionStringsReader[configurationKey].ConnectionString;
                providerName = ConnectionStringsReader[configurationKey].ProviderName;

                // Return the connection string.
                return dbConnectionString;
            }
            catch
            {
                // Throw a general exception.
                return string.Empty;
            }
        }
        #endregion
    }

    /// <summary>
    /// Configuration connection interface.
    /// </summary>
    public interface IDatabaseConnection
    {
        /// <summary>
        /// Method to retrieve the specified database
        /// connection key from the connection string section.
        /// </summary>
        /// <param name="configurationKey">The configuration key.</param>
        /// <param name="providerName">The provider type name.</param>
        /// <returns>The database connection string.</returns>
        string DatabaseConnection(string configurationKey, out string providerName);
    }
}
