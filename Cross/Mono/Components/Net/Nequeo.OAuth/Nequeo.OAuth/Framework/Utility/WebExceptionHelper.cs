/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.IO;
using System.Net;

using Nequeo.Cryptography.Signing;

namespace Nequeo.Net.OAuth.Framework.Utility
{
    /// <summary>
    /// WebException Helper
    /// </summary>
    public static class WebExceptionHelper
    {
        /// <summary>
        /// Will attempt to wrap the exception, returning true if the exception was wrapped, or returning false if it was not (in which case
        /// the original exception should be thrown).
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="webEx"></param>
        /// <param name="authException"></param>
        /// <param name="responseBodyAction"></param>
        /// <returns></returns>
        public static bool TryWrapException(IOAuthContext requestContext, WebException webEx, out OAuthException authException, Action<string> responseBodyAction)
        {
            try
            {
                string content = webEx.Response.ReadToEnd();

                if (responseBodyAction != null)
                {
                    responseBodyAction(content);
                }

                if (content.Contains(Parameters.OAuth_Problem))
                {
                    var report = new OAuthProblemReport(content);
                    authException = new OAuthException(report.ProblemAdvice ?? report.Problem, webEx) { Context = requestContext, Report = report };
                    return true;
                }
            }
            catch
            {
            }
            authException = new OAuthException();
            return false;
        }
    }
}
