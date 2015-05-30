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
using System.Net.Sockets;
using System.Net;
using System.Net.Security;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Nequeo.Web.Mvc
{
    /// <summary>
    /// Abstract class used to extended a data type extension type.
    /// </summary>
    /// <typeparam name="TExtension">The type to extended.</typeparam>
    public abstract class ExtensionMvc<TExtension> : Nequeo.Data.Control.IExtension<TExtension>
        where TExtension : class, new()
    {
        /// <summary>
        /// Gets, the extension generic type.
        /// </summary>
        public TExtension Extension
        {
            get { return new TExtension(); }
        }
    }

    /// <summary>
    /// Abstract class used to extended a data type extension type.
    /// </summary>
    /// <typeparam name="TExtension">The type to extended.</typeparam>
    /// <typeparam name="TExtension1">The type to extended.</typeparam>
    public abstract class ExtensionMvc<TExtension, TExtension1> : Nequeo.Data.Control.IExtension<TExtension, TExtension1>
        where TExtension : class, new()
        where TExtension1 : class, new()
    {
        /// <summary>
        /// Gets, the extension generic type.
        /// </summary>
        public TExtension Extension
        {
            get { return new TExtension(); }
        }

        /// <summary>
        /// Gets, the extension generic type.
        /// </summary>
        public TExtension1 Extension1
        {
            get { return new TExtension1(); }
        }
    }

    /// <summary>
    /// The generic Mvc Model type extension with a Metadata Type extension attribute.
    /// </summary>
    /// <typeparam name="TMvcModel">The Mvc model type.</typeparam>
    /// <typeparam name="TDataEntity">The data entity type.</typeparam>
    public interface IMvcModelMetadataType<TMvcModel, TDataEntity>
        where TDataEntity : class, new()
        where TMvcModel : class, new()
    {
        #region IDataBase Interface

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mvcModel"></param>
        /// <returns></returns>
        TDataEntity MvcMetadataTypeTranslator(TMvcModel mvcModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mvcModel"></param>
        /// <returns></returns>
        TDataEntity[] MvcMetadataTypeTranslator(TMvcModel[] mvcModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        TMvcModel MvcMetadataTypeTranslator(TDataEntity dataEntity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataEntity"></param>
        /// <returns></returns>
        TMvcModel[] MvcMetadataTypeTranslator(TDataEntity[] dataEntity);

        #endregion

        #endregion
    }
}
