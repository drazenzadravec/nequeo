﻿/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
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

namespace Nequeo.Net.OAuth2.Storage
{
    using System;
    using System.Collections.Generic;

    using Nequeo.Net.Core.Messaging.Bindings;
    using Nequeo.Net.Core.Messaging;

    /// <summary>
    /// A persistent store for rotating symmetric cryptographic keys.
    /// </summary>
    /// <remarks>
    /// Implementations should persist it in such a way that the keys are shared across all servers
    /// on a web farm, where applicable.  
    /// The store should consider protecting the persistent store against theft resulting in the loss
    /// of the confidentiality of the keys.  One possible mitigation is to asymmetrically encrypt
    /// each key using a certificate installed in the server's certificate store.
    /// </remarks>
    public interface ICryptographyKeyStore : Nequeo.Net.Core.Messaging.Bindings.ICryptoKeyStore
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        string ClientIndetifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the nonce.
        /// </summary>
        string Nonce
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the code key.
        /// </summary>
        string CodeKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the get code key indicator.
        /// </summary>
        bool GetCodeKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the expiry date time.
        /// </summary>
        DateTime ExpiryDateTime
        {
            get;
            set;
        }
    }
}
