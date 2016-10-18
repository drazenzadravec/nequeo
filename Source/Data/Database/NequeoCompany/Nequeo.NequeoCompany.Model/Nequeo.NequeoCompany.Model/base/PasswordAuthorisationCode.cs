/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.DataAccess.NequeoCompany
{
    /// <summary>
    /// Password authorisation code helper.
    /// </summary>
    internal class PasswordAuthorisationCode
    {
        /// <summary>
        /// Get encoder.
        /// </summary>
        /// <param name="useAuthorisationCode">Should the authorisation code be used.</param>
        /// <returns>The password encoder.</returns>
        public static Nequeo.Cryptography.IPasswordEncryption GetEncoder(bool useAuthorisationCode = true)
        {
            Nequeo.Security.Configuration.Reader reader = new Security.Configuration.Reader();
            Nequeo.Cryptography.IPasswordEncryption encoder = reader.GetEncoder();

            // Return the encoder.
            return encoder;
        }
    }
}
