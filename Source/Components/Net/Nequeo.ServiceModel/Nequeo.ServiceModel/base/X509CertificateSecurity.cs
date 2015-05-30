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
using System.Reflection;
using System.ServiceModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Compilation;

using Nequeo.Security;

namespace Nequeo.Net.ServiceModel
{
    /// <summary>
    /// Custom x509 certificate validator.
    /// </summary>
    public class X509CertificateSecurity : ServiceX509CertificateValidator
    {
        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="validateValue">The validation value to match.</param>
        /// <param name="validationLevel">The validation enum to match with.</param>
        public X509CertificateSecurity(object validateValue,
            Nequeo.Security.X509CertificateLevel validationLevel)
            : base(validateValue, validationLevel)
        {
            // Get the validate value.
            if (validateValue == null)
                throw new ArgumentNullException("Validate value has no reference.",
                    new Exception("A validate value was not supplied."));
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="selectors">The collection of validation selectors.</param>
        public X509CertificateSecurity(ServiceX509CertificateValidationSelector[] selectors) : base(selectors)
        {
            // Get the validate value.
            if (selectors == null)
                throw new ArgumentNullException("Validate selector has no reference.",
                    new Exception("Validate values have not been supplied."));
        }
        #endregion
    }
}
