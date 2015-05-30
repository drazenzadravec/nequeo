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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.IO;

using Nequeo.Net.OAuth.Framework;
using Nequeo.Net.OAuth.Framework.Signing;
using Nequeo.Net.OAuth.Framework.Utility;

namespace Nequeo.Net.OAuth.Storage.Basic
{
    /// <summary>
    /// A simplistic repository for access and request of token models - the example implementation of
    /// <see cref="ITokenStore" /> relies on this repository - normally you would make use of repositories
    /// wired up to your domain model i.e. NHibernate, Entity Framework etc.
    /// </summary>
    /// <typeparam name="T">The type to examine</typeparam>
    public interface ITokenRepository<T> where T : TokenBase
    {
        /// <summary>
        /// Gets an existing token from the underlying store
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        T GetToken(string token);

        /// <summary>
        /// Saves the token in the underlying store
        /// </summary>
        /// <param name="token"></param>
        void SaveToken(T token);
    }
}
