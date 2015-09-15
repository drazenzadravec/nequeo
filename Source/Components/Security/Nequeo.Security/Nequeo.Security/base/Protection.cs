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
using System.Threading;
using System.IO;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;

using Nequeo.Cryptography;

namespace Nequeo.Security
{
    /// <summary>
    /// Security data protection implementation.
    /// </summary>
    public sealed class Protection
    {
        /// <summary>
        /// Security data protection implementation.
        /// </summary>
        public Protection() { }

        /// <summary>
        /// Encrypts the data in a specified byte array and returns a byte array that contains the encrypted data.
        /// This method uses the current user account.
        /// </summary>
        /// <param name="data">The data to protect.</param>
        /// <returns>The stream containing the protected data.</returns>
        public MemoryStream UserProtect(byte[] data)
        {
            DataProtection protect = new DataProtection();
            byte[] protectedData = protect.Protect(data, DataProtectionScope.CurrentUser);
            return new MemoryStream(protectedData);
        }

        /// <summary>
        /// Decrypts the data in a specified byte array and returns a byte array that contains the decrypted data.
        /// This method uses the current user account.
        /// </summary>
        /// <param name="data">The data to unprotect.</param>
        /// <returns>The stream containing the unprotected data.</returns>
        public MemoryStream UserUnprotect(byte[] data)
        {
            DataProtection unprotect = new DataProtection();
            byte[] unprotectedData = unprotect.Unprotect(data, DataProtectionScope.CurrentUser);
            return new MemoryStream(unprotectedData);
        }

        /// <summary>
        /// Encrypts the data in a specified byte array and returns a byte array that contains the encrypted data.
        /// This method uses the local machine account.
        /// </summary>
        /// <param name="data">The data to protect.</param>
        /// <returns>The stream containing the protected data.</returns>
        public MemoryStream MachineProtect(byte[] data)
        {
            DataProtection protect = new DataProtection();
            byte[] protectedData = protect.Protect(data, DataProtectionScope.LocalMachine);
            return new MemoryStream(protectedData);
        }

        /// <summary>
        /// Decrypts the data in a specified byte array and returns a byte array that contains the decrypted data.
        /// This method uses the local machine account.
        /// </summary>
        /// <param name="data">The data to unprotect.</param>
        /// <returns>The stream containing the unprotected data.</returns>
        public MemoryStream MachineUnprotect(byte[] data)
        {
            DataProtection unprotect = new DataProtection();
            byte[] unprotectedData = unprotect.Unprotect(data, DataProtectionScope.LocalMachine);
            return new MemoryStream(unprotectedData);
        }

        /// <summary>
        /// Protects the specified data. This method uses the same process memory scope.
        /// </summary>
        /// <param name="data">The byte array containing data in memory to protect. The array must be a multiple of 16 bytes.</param>
        /// <returns>A byte array representing the encrypted data.</returns>
        public byte[] SameProcessProtect(byte[] data)
        {
            return new MemoryProtection().Protect(data, MemoryProtectionScope.SameProcess);
        }

        /// <summary>
        /// Unprotects data in memory that was protected. This method uses the same process memory scope.
        /// </summary>
        /// <param name="data">The byte array in memory to unencrypt.</param>
        /// <returns>A byte array representing the decrypted data.</returns>
        public byte[] SameProcessUnprotect(byte[] data)
        {
            return new MemoryProtection().Unprotect(data, MemoryProtectionScope.SameProcess);
        }

        /// <summary>
        /// Protects the specified data. This method uses the same logon memory scope.
        /// </summary>
        /// <param name="data">The byte array containing data in memory to protect. The array must be a multiple of 16 bytes.</param>
        /// <returns>A byte array representing the encrypted data.</returns>
        public byte[] SameLogonProtect(byte[] data)
        {
            return new MemoryProtection().Protect(data, MemoryProtectionScope.SameLogon);
        }

        /// <summary>
        /// Unprotects data in memory that was protected. This method uses the same logon memory scope.
        /// </summary>
        /// <param name="data">The byte array in memory to unencrypt.</param>
        /// <returns>A byte array representing the decrypted data.</returns>
        public byte[] SameLogonUnprotect(byte[] data)
        {
            return new MemoryProtection().Unprotect(data, MemoryProtectionScope.SameLogon);
        }

        /// <summary>
        /// Protects the specified data. This method uses the cross process memory scope.
        /// </summary>
        /// <param name="data">The byte array containing data in memory to protect. The array must be a multiple of 16 bytes.</param>
        /// <returns>A byte array representing the encrypted data.</returns>
        public byte[] CrossProcessProtect(byte[] data)
        {
            return new MemoryProtection().Protect(data, MemoryProtectionScope.CrossProcess);
        }

        /// <summary>
        /// Unprotects data in memory that was protected. This method uses the cross process memory scope.
        /// </summary>
        /// <param name="data">The byte array in memory to unencrypt.</param>
        /// <returns>A byte array representing the decrypted data.</returns>
        public byte[] CrossProcessUnprotect(byte[] data)
        {
            return new MemoryProtection().Unprotect(data, MemoryProtectionScope.CrossProcess);
        }
    }
}
