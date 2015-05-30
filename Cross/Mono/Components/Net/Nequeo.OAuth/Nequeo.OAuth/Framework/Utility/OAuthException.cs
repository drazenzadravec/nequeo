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

using Nequeo.Cryptography.Signing;

namespace Nequeo.Net.OAuth.Framework.Utility
{
    /// <summary>
    /// OAuth execption handler.
    /// </summary>
    public class OAuthException : Exception
    {
        /// <summary>
        /// OAuth execption.
        /// </summary>
        public OAuthException()
        {
        }

        /// <summary>
        /// OAuth execption.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public OAuthException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// OAuth execption.
        /// </summary>
        /// <param name="context">The OAuth context</param>
        /// <param name="problem">The problem</param>
        /// <param name="advice">The advice.</param>
        public OAuthException(IOAuthContext context, string problem, string advice)
            : base(advice)
        {
            Context = context;
            Report = new OAuthProblemReport { Problem = problem, ProblemAdvice = advice };
        }

        /// <summary>
        /// OAuth execption.
        /// </summary>
        /// <param name="context">The OAuth context</param>
        /// <param name="problem">The problem</param>
        /// <param name="advice">The advice.</param>
        /// <param name="innerException">The inner exception.</param>
        public OAuthException(IOAuthContext context, string problem, string advice, Exception innerException)
            : base(advice, innerException)
        {
            Context = context;
            Report = new OAuthProblemReport { Problem = problem, ProblemAdvice = advice };
        }

        /// <summary>
        /// OAuth execption.
        /// </summary>
        /// <param name="problem">The problem</param>
        /// <param name="advice">The advice.</param>
        /// <param name="innerException">The inner exception.</param>
        public OAuthException(string problem, string advice, Exception innerException)
            : base(advice, innerException)
        {
            Report = new OAuthProblemReport { Problem = problem, ProblemAdvice = advice };
        }

        /// <summary>
        /// OAuth execption.
        /// </summary>
        /// <param name="info">Stores all the data needed to serialize or deserialize an object. This class cannot be inherited.</param>
        /// <param name="context">Describes the source and destination of a given serialized stream, and provides an additional caller-defined context.</param>
        public OAuthException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets sets the OAuth problem report.
        /// </summary>
        public OAuthProblemReport Report { get; set; }

        /// <summary>
        /// Gets sets the OAuth context.
        /// </summary>
        public IOAuthContext Context { get; set; }
    }
}
