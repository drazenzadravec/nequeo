/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Nequeo
{
    /// <summary>
    /// Environment helper.
    /// </summary>
    public sealed class EnvironmentEx
    {
        /// <summary>
        /// Gets the collection of environment variables
        /// </summary>
        /// <returns>A dictionary containing the collection of environment variables</returns>
        public static IDictionary GetEnvironmentVariables()
        {
            return Environment.GetEnvironmentVariables();
        }

        /// <summary>
        /// Get the specified environment variable
        /// </summary>
        /// <param name="key">The key of the environment variable.</param>
        /// <returns>The value of the environment variable.</returns>
        public static object GetEnvironmentVariable(object key)
        {
            // Make sure the page reference exists.
            if (key == null) throw new ArgumentNullException("key");

            IDictionary env = Environment.GetEnvironmentVariables();
            return env[key];
        }

        /// <summary>
        /// Set the environment variable value fro the key.
        /// </summary>
        /// <param name="key">The environment variable key.</param>
        /// <param name="value">The environment variable value.</param>
        public static void SetEnvironmentVariable(object key, object value)
        {
            // Make sure the page reference exists.
            if (key == null) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");

            IDictionary env = Environment.GetEnvironmentVariables();
            env[key] = value;
        }

        /// <summary>
        /// Add the environment variable to the collection.
        /// </summary>
        /// <param name="key">The environment variable key.</param>
        /// <param name="value">The environment variable value.</param>
        public static void AddEnvironmentVariable(object key, object value)
        {
            // Make sure the page reference exists.
            if (key == null) throw new ArgumentNullException("key");
            if (value == null) throw new ArgumentNullException("value");

            IDictionary env = Environment.GetEnvironmentVariables();
            env.Add(key, value);
        }

        /// <summary>
        /// Method to locate application configuration file.
        /// </summary>
        /// <returns>The full path of the logging file, else empty string.</returns>
        public static System.Configuration.Configuration GetApplicationConfigurationFile()
        {
            return GetApplicationConfigurationFile(string.Empty);
        }

        /// <summary>
        /// Method to locate application configuration file.
        /// </summary>
        /// <param name="specificPath">The specific path of the config file, used for web applications</param>
        /// <returns>The application configuration istance else null</returns>
        public static System.Configuration.Configuration GetApplicationConfigurationFile(string specificPath)
        {
            // Make sure the page reference exists.
            if (specificPath == null) throw new ArgumentNullException("specificPath");

            System.Configuration.Configuration config = null;

            try
            {
                // Get the absolute configuration information
                // of the current application.
                if (string.IsNullOrEmpty(specificPath))
                    config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                else
                    config = ConfigurationManager.OpenExeConfiguration(specificPath);
            }
            catch { }

            // Return the configuration instance.
            return config;
        }
    }
}
