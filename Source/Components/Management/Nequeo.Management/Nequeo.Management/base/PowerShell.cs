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
using System.IO;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Nequeo.Management
{
    /// <summary>
    /// PowerShell automation provider.
    /// </summary>
	public sealed class PowerShellEx
	{
        /// <summary>
        /// Runs a script.
        /// </summary>
        /// <param name="scriptText">The script text to run.</param>
        /// <returns>The result of the action.</returns>
        public static string RunScript(string scriptText)
        {
            Runspace runspace = null;

            try
            {
                // create Powershell runspace
                runspace = RunspaceFactory.CreateRunspace();

                // open it
                runspace.Open();

                // create a pipeline and feed it the script text
                Pipeline pipeline = runspace.CreatePipeline();
                pipeline.Commands.AddScript(scriptText);

                // add an extra command to transform the script
                // output objects into nicely formatted strings
                // remove this line to get the actual objects
                // that the script returns. For example, the script
                // "Get-Process" returns a collection
                // of System.Diagnostics.Process instances.
                pipeline.Commands.Add("Out-String");

                // execute the script
                Collection<PSObject> results = pipeline.Invoke();

                // convert the script result into a single string
                StringBuilder stringBuilder = new StringBuilder();
                foreach (PSObject obj in results)
                {
                    stringBuilder.AppendLine(obj.ToString());
                }

                // Return the result.
                return stringBuilder.ToString();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // close the runspace
                if(runspace != null)
                    runspace.Close();
            }
        }

        /// <summary>
        /// Run the script with a file.
        /// </summary>
        /// <param name="filename">The path and filename of the file that contains the script.</param>
        /// <returns>The result of the action; else null.</returns>
        public static string RunScriptFile(string filename)
        {
            FileStream fileStream = null;
            StreamReader streamReader = null;
            string result = null;
            string script = null;

            try
            {
                // Open the file stream.
                using (fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (streamReader = new StreamReader(fileStream))
                {
                    // Read all the script data in the file.
                    script = streamReader.ReadToEnd();

                    // Close the streams.
                    streamReader.Close();
                    fileStream.Close();
                }

                // If script data has been read.
                if (!String.IsNullOrEmpty(script))
                {
                    // Run the script in the file.
                    result = RunScript(script);
                }

                // Return the result.
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (streamReader != null)
                    streamReader.Dispose();

                if (fileStream != null)
                    fileStream.Dispose();
            }
        }
	}
}
