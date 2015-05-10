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
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Net.Mail;
using System.Net;

namespace Nequeo.Invention
{
    /// <summary>
    /// Class for external/internal application integration.
    /// </summary>
    public class Application
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly Application Instance = new Application();

        /// <summary>
        /// Static constructor
        /// </summary>
        static Application() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public Application()
        {
        }
        #endregion

        #region Private Application Methods
        /// <summary>
        /// Runs the application with no standard output/error incapsolation.
        /// </summary>
        /// <param name="applicationExecutable">The application to start.</param>
        /// <param name="applicationArguments">The application arguments if any.</param>
        /// <param name="verb">The application verb if any.</param>
        /// <param name="redirectOutput">Should the output/error streams be captured.</param>
        /// <param name="waitForExit">The amount of time to wait before exit; -1 indicates do not wait; 0 indicates wait indefinitely</param>
        /// <returns>The results from the application if any.</returns>
        /// <remarks>Option for application output/error capture.</remarks>
        private Nullable<ApplicationResult> RunApplicationEx(string applicationExecutable,
            string applicationArguments, string verb, bool redirectOutput, int waitForExit = -1)
        {
            // The process application object.
            Process process = null;

            // The standard application results.
            string errorMesssge = null;
            string outputMessage = null;

            // The standard application return streams.
            StreamReader errorStream = null;
            StreamReader outputStream = null;

            try
            {
                // Create a new instance of the process object.
                process = new Process();

                // Assign the startinfo property
                // with each of the process parameters.
                // Start the application.
                process.StartInfo.FileName = applicationExecutable;
                process.StartInfo.Arguments = applicationArguments;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(applicationExecutable);
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = redirectOutput;
                process.StartInfo.RedirectStandardError = redirectOutput;

                // If a verb exists.
                if (!String.IsNullOrEmpty(verb))
                    process.StartInfo.Verb = verb;

                // Start the process.
                process.Start();

                // If more than -1 then wait.
                if (waitForExit > -1)
                {
                    // If more than zero then wait.
                    if (waitForExit == 0)
                    {
                        // Wait indefinitely.
                        process.WaitForExit();

                        if (!process.HasExited)
                            process.WaitForInputIdle();
                    }
                    else
                    {
                        // Wait the specified amount of time before exit
                        process.WaitForExit(waitForExit);

                        if (!process.HasExited)
                            process.WaitForInputIdle(waitForExit);
                    }
                }

                // If output/error streams should be catured.
                if (redirectOutput)
                {
                    // Assign the output and error
                    // standard streams for the application.
                    errorStream = process.StandardError;
                    outputStream = process.StandardOutput;

                    // Assign the results from the output/error
                    // streams into the string objects.
                    errorMesssge = errorStream.ReadToEnd();
                    outputMessage = outputStream.ReadToEnd();
                }

                // Create a new application result structer
                // and store the output/error results.
                ApplicationResult applicationResult =
                    new ApplicationResult(outputMessage, errorMesssge);

                // Return the application results.
                return applicationResult;
            }
            catch (System.Exception)
            {
                // Return a null value.
                throw;
            }
            finally
            {
                // Release all resources.
                if (errorStream != null)
                    errorStream.Close();

                // Release all resources.
                if (outputStream != null)
                    outputStream.Close();
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Runs the application with no standard output/error incapsolation.
        /// </summary>
        /// <param name="applicationExecutable">The application to start.</param>
        /// <param name="applicationArguments">The application arguments if any.</param>
        /// <param name="verb">The application verb if any.</param>
        /// <param name="redirectOutput">Should the output/error streams be captured.</param>
        /// <param name="waitForExit">The amount of time to wait before exit; -1 indicates do not wait; 0 indicates wait indefinitely</param>
        /// <returns>The results from the application if any.</returns>
        /// <remarks>Option for application output/error capture.</remarks>
        public virtual Nullable<ApplicationResult> RunApplication(string applicationExecutable,
            string applicationArguments, string verb, bool redirectOutput, int waitForExit)
        {
            return RunApplicationEx(applicationExecutable, applicationArguments, verb, redirectOutput, waitForExit);
        }

        /// <summary>
        /// Runs the application with no standard output/error incapsolation.
        /// </summary>
        /// <param name="applicationExecutable">The application to start.</param>
        /// <param name="applicationArguments">The application arguments if any.</param>
        /// <param name="verb">The application verb if any.</param>
        /// <param name="redirectOutput">Should the output/error streams be captured.</param>
        /// <returns>The results from the application if any.</returns>
        /// <remarks>Option for application output/error capture.</remarks>
        public virtual Nullable<ApplicationResult> RunApplication(string applicationExecutable,
            string applicationArguments, string verb, bool redirectOutput)
        {
            return RunApplicationEx(applicationExecutable, applicationArguments, verb, redirectOutput);
        }

        /// <summary>
        /// Runs the application with no standard output/error incapsolation.
        /// </summary>
        /// <param name="applicationExecutable">The application to start.</param>
        /// <param name="applicationArguments">The application arguments if any.</param>
        /// <param name="verb">The application verb if any.</param>
        /// <returns>The results from the application if any.</returns>
        /// <remarks>No application output/error is captured.</remarks>
        public virtual Nullable<ApplicationResult> RunApplication(string applicationExecutable,
            string applicationArguments, string verb)
        {
            return RunApplicationEx(applicationExecutable, applicationArguments, verb, false);
        }

        /// <summary>
        /// Runs the application with no standard output/error incapsolation.
        /// </summary>
        /// <param name="applicationExecutable">The application to start.</param>
        /// <param name="applicationArguments">The application arguments if any.</param>
        /// <returns>The results from the application if any.</returns>
        /// <remarks>No application output/error is captured.</remarks>
        public virtual Nullable<ApplicationResult> RunApplication(string applicationExecutable,
            string applicationArguments)
        {
            return RunApplicationEx(applicationExecutable, applicationArguments, String.Empty, false);
        }
        #endregion
    }

    /// <summary>
    /// The struct will contain the application
    /// return results when application integration
    /// is applied.
    /// </summary>
    public struct ApplicationResult
    {
        #region Public Fields
        /// <summary>
        /// The output string from an application.
        /// </summary>
        public string Output;
        /// <summary>
        /// The error string from an application.
        /// </summary>
        public string Error;
        #endregion

        #region Constructors
        /// <summary>
        /// Default application result constructor.
        /// </summary>
        /// <param name="output">The ouput string from the application.</param>
        /// <param name="error">The error string from the application.</param>
        public ApplicationResult(string output, string error)
        {
            this.Output = output;
            this.Error = error;
        }
        #endregion
    }

    /// <summary>
    /// Class for external/internal application integration.
    /// </summary>
    public class ApplicationInteraction
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly ApplicationInteraction Instance = new ApplicationInteraction();

        /// <summary>
        /// Static constructor
        /// </summary>
        static ApplicationInteraction() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public ApplicationInteraction()
        {
        }
        #endregion

        #region Private Application Methods
        /// <summary>
        /// Runs the application with no standard output/error incapsolation.
        /// </summary>
        /// <param name="applicationExecutable">The application to start.</param>
        /// <param name="applicationArguments">The application arguments if any.</param>
        /// <param name="verb">The application verb if any.</param>
        /// <param name="waitForExit">The amount of time to wait before exit; -1 indicates do not wait; 0 indicates wait indefinitely</param>
        /// <returns>The results from the application if any.</returns>
        /// <remarks>Option for application output/error capture.</remarks>
        private Nullable<ApplicationInteractionResult> RunApplicationEx(string applicationExecutable,
            string applicationArguments, string verb, int waitForExit = -1)
        {
            // The process application object.
            Process process = null;

            // The standard application return streams.
            StreamWriter inputStream = null;
            StreamReader outputStream = null;

            try
            {
                // Create a new instance of the process object.
                process = new Process();

                // Assign the startinfo property
                // with each of the process parameters.
                // Start the application.
                process.StartInfo.FileName = applicationExecutable;
                process.StartInfo.Arguments = applicationArguments;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(applicationExecutable);
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardInput = true;

                // If a verb exists.
                if (!String.IsNullOrEmpty(verb))
                    process.StartInfo.Verb = verb;

                // Start the process.
                process.Start();

                // If more than -1 then wait.
                if (waitForExit > -1)
                {
                    // If more than zero then wait.
                    if (waitForExit == 0)
                    {
                        // Wait indefinitely.
                        process.WaitForExit();

                        if (!process.HasExited)
                            process.WaitForInputIdle();
                    }
                    else
                    {
                        // Wait the specified amount of time before exit
                        process.WaitForExit(waitForExit);

                        if (!process.HasExited)
                            process.WaitForInputIdle(waitForExit);
                    }
                }

                // Assign the output and error
                // standard streams for the application.
                inputStream = process.StandardInput;
                outputStream = process.StandardOutput;

                // Create a new application result structer
                // and store the output/error results.
                ApplicationInteractionResult applicationResult =
                    new ApplicationInteractionResult(outputStream, inputStream);

                // Return the application results.
                return applicationResult;
            }
            catch (System.Exception)
            {
                // Release all resources.
                if (inputStream != null)
                    inputStream.Close();

                // Release all resources.
                if (outputStream != null)
                    outputStream.Close();

                // Return a null value.
                throw;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Runs the application with no standard output/error incapsolation.
        /// </summary>
        /// <param name="applicationExecutable">The application to start.</param>
        /// <param name="applicationArguments">The application arguments if any.</param>
        /// <param name="verb">The application verb if any.</param>
        /// <param name="waitForExit">The amount of time to wait before exit; -1 indicates do not wait; 0 indicates wait indefinitely</param>
        /// <returns>The results from the application if any.</returns>
        /// <remarks>Option for application output/error capture.</remarks>
        public virtual Nullable<ApplicationInteractionResult> RunApplication(string applicationExecutable,
            string applicationArguments, string verb, int waitForExit)
        {
            return RunApplicationEx(applicationExecutable, applicationArguments, verb, waitForExit);
        }

        /// <summary>
        /// Runs the application with no standard output/error incapsolation.
        /// </summary>
        /// <param name="applicationExecutable">The application to start.</param>
        /// <param name="applicationArguments">The application arguments if any.</param>
        /// <param name="verb">The application verb if any.</param>
        /// <returns>The results from the application if any.</returns>
        /// <remarks>No application output/error is captured.</remarks>
        public virtual Nullable<ApplicationInteractionResult> RunApplication(string applicationExecutable,
            string applicationArguments, string verb)
        {
            return RunApplicationEx(applicationExecutable, applicationArguments, verb);
        }

        /// <summary>
        /// Runs the application with no standard output/error incapsolation.
        /// </summary>
        /// <param name="applicationExecutable">The application to start.</param>
        /// <param name="applicationArguments">The application arguments if any.</param>
        /// <returns>The results from the application if any.</returns>
        /// <remarks>No application output/error is captured.</remarks>
        public virtual Nullable<ApplicationInteractionResult> RunApplication(string applicationExecutable,
            string applicationArguments)
        {
            return RunApplicationEx(applicationExecutable, applicationArguments, String.Empty);
        }
        #endregion
    }

    /// <summary>
    /// The struct will contain the application
    /// return results when application integration
    /// is applied.
    /// </summary>
    public struct ApplicationInteractionResult
    {
        #region Public Fields
        /// <summary>
        /// The output stream from an application.
        /// </summary>
        public StreamReader Output;
        /// <summary>
        /// The input stream from an application.
        /// </summary>
        public StreamWriter Input;
        #endregion

        #region Constructors
        /// <summary>
        /// Default application result constructor.
        /// </summary>
        /// <param name="output">The ouput stream from the application.</param>
        /// <param name="input">The input stream from the application.</param>
        public ApplicationInteractionResult(StreamReader output, StreamWriter input)
        {
            this.Output = output;
            this.Input = input;
        }
        #endregion
    }
}
