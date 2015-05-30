﻿// Warning 169 (Disables the 'Never used' warning)
#pragma warning disable 169
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Nequeo.Net.Http
{
    using System;
    using System.Text;
    using System.Data;
    using System.Threading;
    using System.Diagnostics;
    using System.Data.SqlClient;
    using System.Data.OleDb;
    using System.Data.Odbc;
    using System.Collections;
    using System.Reflection;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using System.Runtime.Serialization;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    
    #region HttpClient Extension Type
    /// <summary>
    /// The HttpClient object class.
    /// </summary>
    public partial class HttpClient
    {
        private Exception _exception = null;
		private HttpClientThread _threadHttpClientContext = null;

		/// <summary>
        /// Gets the current async exception; else null;
        /// </summary>
        public Exception ExceptionHttpClient
        {
            get { return _exception; }
        }

		/// <summary>
        /// Gets the HttpClient threading context.
        /// </summary>
        public HttpClientThread HttpClientThreadContext
        {
            get { return _threadHttpClientContext; }
        }

		/// <summary>
        /// On create.
        /// </summary>
        partial void OnCreated();

		/// <summary>
        /// On create instance of HttpClient
        /// </summary>
		partial void OnCreated()
		{
			// Start the async control.
			_threadHttpClientContext = new HttpClientThread(this);
			_threadHttpClientContext.AsyncError += new Threading.EventHandler<Exception>(_asyncAccount_AsyncError);
		}

		/// <summary>
        /// Async error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e1"></param>
        private void _asyncAccount_AsyncError(object sender, Exception e1)
        {
            _exception = e1;
        }

		/// <summary>
        /// HttpClient threading handler.
        /// </summary>
        public class HttpClientThread : Nequeo.Threading.AsyncExecutionHandler<HttpClient>
        {
            /// <summary>
            /// HttpClient threading handler.
            /// </summary>
            /// <param name="service">The HttpClient type.</param>
            public HttpClientThread(HttpClient service)
                : base(service) { }
        }
    }
    #endregion
}

#pragma warning restore 169
