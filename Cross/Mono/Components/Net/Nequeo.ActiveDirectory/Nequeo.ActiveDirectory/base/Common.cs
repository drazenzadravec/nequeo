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

namespace Nequeo.Net.ActiveDirectory
{
    /// <summary>
    /// Directory services create account handler.
    /// </summary>
    /// <param name="sender">The object sender.</param>
    /// <param name="result">The result of the operation.</param>
    public delegate void CreateAccountHandler(object sender, Boolean result);

    /// <summary>
    /// Directory services remove account handler.
    /// </summary>
    /// <param name="sender">The object sender.</param>
    /// <param name="result">The result of the operation.</param>
    public delegate void RemoveAccountHandler(object sender, Boolean result);

    /// <summary>
    /// Active directory user account flags.
    /// </summary>
    [Flags]
    internal enum ADS_USER_FLAG_ENUM
    {
        ADS_UF_SCRIPT = 0x0001,
        ADS_UF_ACCOUNTDISABLE = 0x0002,
        ADS_UF_HOMEDIR_REQUIRED = 0x0008,
        ADS_UF_LOCKOUT = 0x0010,
        ADS_UF_PASSWD_NOTREQD = 0x0020,
        ADS_UF_PASSWD_CANT_CHANGE = 0x0040,
        ADS_UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0x0080,
        ADS_UF_TEMP_DUPLICATE_ACCOUNT = 0x0100,
        ADS_UF_NORMAL_ACCOUNT = 0x0200,
        ADS_UF_INTERDOMAIN_TRUST_ACCOUNT = 0x0800,
        ADS_UF_WORKSTATION_TRUST_ACCOUNT = 0x1000,
        ADS_UF_SERVER_TRUST_ACCOUNT = 0x2000,
        ADS_UF_DONT_EXPIRE_PASSWD = 0x10000,
        ADS_UF_MNS_LOGON_ACCOUNT = 0x20000,
        ADS_UF_SMARTCARD_REQUIRED = 0x40000,
        ADS_UF_TRUSTED_FOR_DELEGATION = 0x80000,
        ADS_UF_NOT_DELEGATED = 0x100000,
        ADS_UF_USE_DES_KEY_ONLY = 0x200000,
        ADS_UF_DONT_REQUIRE_PREAUTH = 0x400000,
        ADS_UF_PASSWORD_EXPIRED = 0x800000,
        ADS_UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0x1000000
    }

    /// <summary>
    /// The security type used in IIS directory services.
    /// </summary>
    public enum SecurityType
    {
        /// <summary>
        /// IP grant security.
        /// </summary>
        IPGrant = 0,
        /// <summary>
        /// IP deny security.
        /// </summary>
        IPDeny = 1,
        /// <summary>
        /// Domain grant security.
        /// </summary>
        DomainGrant = 2,
        /// <summary>
        /// Domain deny security.
        /// </summary>
        DomainDeny = 3
    }
}
