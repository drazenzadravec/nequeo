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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Nequeo.Model;
using Nequeo.Extension;
using Nequeo.Cryptography.Key;

namespace Nequeo.Security.Ocsp
{
    /// <summary>
    /// The certificate status type.
    /// </summary>
    public enum CertificateStatusType
    {
        /// <summary>
        /// Good
        /// </summary>
        Good = 0,
        /// <summary>
        /// Revoked
        /// </summary>
        Revoked = 1,
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 2,
    }

    /// <summary>
    /// The certificate revocation type.
    /// </summary>
    public enum RevocationReasonType
    {
        /// <summary>
        /// Unspecified
        /// </summary>
        Unspecified = 0,
        /// <summary>
        /// KeyCompromise
        /// </summary>
        KeyCompromise = 1,
        /// <summary>
        /// CACompromise
        /// </summary>
        CACompromise = 2,
        /// <summary>
        /// AffiliationChanged
        /// </summary>
        AffiliationChanged = 3,
        /// <summary>
        /// Superseded
        /// </summary>
        Superseded = 4,
        /// <summary>
        /// CessationOfOperation
        /// </summary>
        CessationOfOperation  = 5,
        /// <summary>
        /// CertificateHold
        /// </summary>
        CertificateHold = 6,
        /// <summary>
        /// Unknown
        /// </summary>
		Unknown = 7,
        /// <summary>
        /// RemoveFromCrl
        /// </summary>
        RemoveFromCrl = 8,
        /// <summary>
        /// PrivilegeWithdrawn
        /// </summary>
        PrivilegeWithdrawn = 9,
        /// <summary>
        /// AACompromise
        /// </summary>
        AACompromise = 10,
    }

    /// <summary>
    /// The response status type.
    /// </summary>
    public enum ResponseStatusType
    {
        /// <summary>
        /// Successful
        /// </summary>
        Successful = 0,
        /// <summary>
        /// MalformedRequest
        /// </summary>
        MalformedRequest = 1,
        /// <summary>
        /// InternalError
        /// </summary>
        InternalError = 2,
        /// <summary>
        /// TryLater
        /// </summary>
        TryLater = 3,
        /// <summary>
        /// SigRequired
        /// </summary>
        SigRequired = 5,
        /// <summary>
        /// Unauthorized
        /// </summary>
        Unauthorized = 6,
    }

    /// <summary>
    /// Certificate status.
    /// </summary>
    public class CertificateStatus
    {
        private CertificateStatusType _status = CertificateStatusType.Good;
        private DateTime _revocationDate = DateTime.Now;
        private RevocationReasonType _reason = RevocationReasonType.Unspecified;

        /// <summary>
        /// Gets or sets the certificate status type.
        /// </summary>
        public CertificateStatusType Status 
        {
            get { return _status; }
            set { _status = value; }
        }

        /// <summary>
        /// Gets or sets the revocation data.
        /// </summary>
        public DateTime RevocationDate
        {
            get { return _revocationDate; }
            set { _revocationDate = value; }
        }

        /// <summary>
        /// Gets or sets the revocation reason.
        /// </summary>
        public RevocationReasonType RevocationReason
        {
            get { return _reason; }
            set { _reason = value; }
        }
    }

    /// <summary>
    /// Certificate response.
    /// </summary>
    public class CertificateResponse
    {
        /// <summary>
        /// Gets or sets the X509 Certificate.
        /// </summary>
        public X509Certificate2 X509Certificate { get; set; }

        /// <summary>
        /// Gets or sets the status of the X509 certificate.
        /// </summary>
        public CertificateStatus CertificateStatus { get; set; }
    }

    /// <summary>
    /// Certificate request.
    /// </summary>
    public class CertificateRequest
    {
        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
        public byte[] SerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the valid to date.
        /// </summary>
        public DateTime NotAfter { get; set; }

        /// <summary>
        /// Gets or sets the valid from date.
        /// </summary>
        public DateTime NotBefore { get; set; }

        /// <summary>
        /// Gets or sets the certificate subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the subject unique id.
        /// </summary>
        public byte[] SubjectUniqueID { get; set; }

        /// <summary>
        /// Gets or sets the certificate issuer.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the issuer unique id.
        /// </summary>
        public byte[] IssuerUniqueID { get; set; }
    }
}
