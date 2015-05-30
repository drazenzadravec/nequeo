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

namespace Nequeo.Cryptography.Parser
{
    /// <summary>
    /// Represents Abstract Syntax Notation One (ASN.1)-encoded data.
    /// </summary>
    public class AsnEncoded
    {
        /// <summary>
        /// Get the ANS.1 encoded data from the certificate collection.
        /// </summary>
        /// <param name="certificateCollection">The certificate collection.</param>
        /// <returns>The collection of ANS.1 encoded data.</returns>
        public AsnEncodedDataCollection GetData(X509Certificate2Collection certificateCollection)
        {
            // Create a new AsnEncodedDataCollection object.
            AsnEncodedDataCollection asncoll = new AsnEncodedDataCollection();

            // For each certificate.
            for (int i = 0; i < certificateCollection.Count; i++)
            {
                // Display extensions information. 
                foreach (X509Extension extension in certificateCollection[i].Extensions)
                {
                    // Create an AsnEncodedData object using the extensions information.
                    AsnEncodedData asndata = new AsnEncodedData(extension.Oid, extension.RawData);
                    
                    // Add the AsnEncodedData object to the AsnEncodedDataCollection object.
                    asncoll.Add(asndata);
                }
            }

            // Return the ASN.1 encoded data.
            return asncoll;
        }

        /// <summary>
        /// Get the ANS.1 encoded data from the certificate.
        /// </summary>
        /// <param name="certificate">The certificate.</param>
        /// <returns>The collection of ANS.1 encoded data.</returns>
        public AsnEncodedDataCollection GetData(X509Certificate2 certificate)
        {
            // Create a new AsnEncodedDataCollection object.
            AsnEncodedDataCollection asncoll = new AsnEncodedDataCollection();

            // Display extensions information. 
            foreach (X509Extension extension in certificate.Extensions)
            {
                // Create an AsnEncodedData object using the extensions information.
                AsnEncodedData asndata = new AsnEncodedData(extension.Oid, extension.RawData);

                // Add the AsnEncodedData object to the AsnEncodedDataCollection object.
                asncoll.Add(asndata);
            }

            // Return the ASN.1 encoded data.
            return asncoll;
        }
    }
}
