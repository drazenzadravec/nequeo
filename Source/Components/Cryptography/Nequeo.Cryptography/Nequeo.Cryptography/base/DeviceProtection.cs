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
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel.Composition;

namespace Nequeo.Cryptography
{
    /// <summary>
    /// Creates a cryptographic service provider (CSP) that accesses a properly installed hardware encryption device.
    /// </summary>
    public sealed class DeviceProtection
    {
        /// <summary>
        /// Creates a cryptographic service provider (CSP) that accesses a properly installed hardware encryption device.
        /// </summary>
        public DeviceProtection() { }

        /// <summary>
        /// Sign the data with the RSA CSP.
        /// </summary>
        /// <param name="data">The data to sign.</param>
        /// <param name="cspParameters">The CSP parameters.</param>
        /// <param name="hashcode">The hash code to use.</param>
        /// <returns>The signature.</returns>
        public byte[] Sign(byte[] data, CspParameters cspParameters, HashcodeType hashcode = HashcodeType.SHA512)
        {
            // Initialize an RSACryptoServiceProvider object using
            // the CspParameters object.
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParameters);

            // Sign the data using the Smart Card CryptoGraphic Provider.
            byte[] sig = rsa.SignData(data, hashcode.ToString());
            return sig;
        }

        /// <summary>
        /// Verify the data with the RSA CSP.
        /// </summary>
        /// <param name="data">The data that was signed.</param>
        /// <param name="signature">The signature of the original data.</param>
        /// <param name="cspParameters">The CSP parameters.</param>
        /// <param name="hashcode">The hash code to use.</param>
        /// <returns>True if the data is valid; else false.</returns>
        public bool Verify(byte[] data, byte[] signature, CspParameters cspParameters, HashcodeType hashcode = HashcodeType.SHA512)
        {
            // Initialize an RSACryptoServiceProvider object using
            // the CspParameters object.
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParameters);

            // Verify the data using the Smart Card CryptoGraphic Provider.
            bool verified = rsa.VerifyData(data, hashcode.ToString(), signature);
            return verified;
        }

        /// <summary>
        /// Convert the data array to hex values.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <returns>The hex array.</returns>
        public byte[] ConvertToHex(byte[] data)
        {
            return Nequeo.Custom.HexEncoder.Encode(data);
        }

        /// <summary>
        /// Convert the hex data array to the original values.
        /// </summary>
        /// <param name="data">The hex byte array.</param>
        /// <returns>The original values.</returns>
        public byte[] ConvertFromHex(byte[] data)
        {
            return Nequeo.Custom.HexEncoder.Decode(data);
        }
    }
}
