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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.IO;
using System.Reflection;
using System.Web;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;

namespace Nequeo.Service.Ajax
{
    /// <summary>
    /// The AJAX WCF service handler interface.
    /// </summary>
    public interface IAjaxWcfHandler : ICloneable
    {
        /// <summary>
        /// Execute the composition action
        /// </summary>
        /// <param name="parameters">The collection of parameters</param>
        /// <param name="serviceName">The unique service method name.</param>
        /// <param name="checkIfValid">Check if the operation result is valid (result is not null).</param>
        /// <returns>The collection result</returns>
        string[] CompositionActionCol(string[] parameters, string serviceName, bool checkIfValid);

        /// <summary>
        /// Execute the composition action
        /// </summary>
        /// <param name="parameters">The collection of parameters</param>
        /// <param name="serviceName">The unique service method name.</param>
        /// <param name="checkIfValid">Check if the operation result is valid (result is not null).</param>
        /// <returns>The single result</returns>
        string CompositionActionSig(string[] parameters, string serviceName, bool checkIfValid);
    }
}
