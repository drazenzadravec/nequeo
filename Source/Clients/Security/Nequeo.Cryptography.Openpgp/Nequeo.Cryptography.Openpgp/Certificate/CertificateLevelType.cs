/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Security.Cryptography;

using Nequeo.Cryptography.Key;

namespace Nequeo.Cryptography.Openpgp
{
    /// <summary>
    /// Certificate level type.
    /// </summary>
    public enum CertificateLevelType
    {
        /// <summary>
        /// Binary Document.
        /// </summary>
        BinaryDocument = 0,
        /// <summary>
        /// Canonical Text Document.
        /// </summary>
        CanonicalTextDocument = 1,
        /// <summary>
        /// Casual Certification.
        /// </summary>
        CasualCertification = 18,
        /// <summary>
        /// Certification Revocation.
        /// </summary>
        CertificationRevocation = 48,
        /// <summary>
        /// Default Certification.
        /// </summary>
        DefaultCertification = 16,
        /// <summary>
        /// Direct Key.
        /// </summary>
        DirectKey = 31,
        /// <summary>
        /// Key Revocation.
        /// </summary>
        KeyRevocation = 32,
        /// <summary>
        /// No Certification.
        /// </summary>
        NoCertification = 17,
        /// <summary>
        /// Positive Certification.
        /// </summary>
        PositiveCertification = 19,
        /// <summary>
        /// Primary Key Binding.
        /// </summary>
        PrimaryKeyBinding = 25,
        /// <summary>
        /// Stand Alone.
        /// </summary>
        StandAlone = 2,
        /// <summary>
        /// Subkey Binding.
        /// </summary>
        SubkeyBinding = 24,
        /// <summary>
        /// Subkey Revocation.
        /// </summary>
        SubkeyRevocation = 40,
        /// <summary>
        /// Timestamp.
        /// </summary>
        Timestamp = 64,
    }
}
