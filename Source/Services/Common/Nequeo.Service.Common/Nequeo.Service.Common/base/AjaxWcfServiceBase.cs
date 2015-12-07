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
using System.ComponentModel.Composition;

using Nequeo.Handler;

namespace Nequeo.Service.Ajax
{
    /// <summary>
    /// Base AJAX WCF service handler
    /// </summary>
    public abstract class AjaxWcfServiceBase
    {
        /// <summary>
        /// Compose the MEF instance.
        /// </summary>
        public abstract void Compose();

        /// <summary>
        /// Gets sets the current error.
        /// </summary>
        public abstract Exception Exception { get; set; }

        /// <summary>
        /// AJAX WCF servic handler interface
        /// </summary>
        [ImportMany]
        public IEnumerable<IAjaxWcfHandler> AjaxWcfHandler { get; set; }

        /// <summary>
        /// Composition operation
        /// </summary>
        /// <param name="parameters">The collection of parameters</param>
        /// <param name="serviceName">The unique service method name.</param>
        /// <returns>The collection result</returns>
        public virtual string[] CompositeOperationCol(string[] parameters, string serviceName)
        {
            try
            {
                // Initialise the composition assembly collection.
                Compose();

                String[] response = null;

                // Is there a collection of imported assemblies.
                if (AjaxWcfHandler.Count() < 1)
                    throw new Exception("No composition assemblies have been loaded.");

                // Get response.
                response = AjaxWcfHandler.First(
                    u => u.CompositionActionCol(parameters, serviceName, true) != null).CompositionActionCol(parameters, serviceName, false);

                // If no response.
                if (response == null)
                    throw new Exception("Request name is not supported.");

                // Return the response.
                return response;
            }
            catch (System.Threading.ThreadAbortException)
            {
                return null;
            }
            catch (Exception ex)
            {
                // Get the current exception.
                Exception = ex;
                return null;
            }
            finally
            {
            }
        }

        /// <summary>
        /// Composition operation
        /// </summary>
        /// <param name="parameters">The collection of parameters</param>
        /// <param name="serviceName">The unique service method name.</param>
        /// <returns>The single result</returns>
        public virtual string CompositeOperationSig(string[] parameters, string serviceName)
        {
            try
            {
                // Initialise the composition assembly collection.
                Compose();

                String response = null;

                // Is there a collection of imported assemblies.
                if (AjaxWcfHandler.Count() < 1)
                    throw new Exception("No composition assemblies have been loaded.");

                // Get response.
                response = AjaxWcfHandler.First(
                    u => u.CompositionActionSig(parameters, serviceName, true) != null).CompositionActionSig(parameters, serviceName, false);

                // If no response.
                if (response == null)
                    throw new Exception("Request name is not supported.");

                // Return the response.
                return response;
            }
            catch (System.Threading.ThreadAbortException)
            {
                return null;
            }
            catch (Exception ex)
            {
                // Get the current exception.
                Exception = ex;
                return null;
            }
            finally
            {
            }
        }
    }
}
