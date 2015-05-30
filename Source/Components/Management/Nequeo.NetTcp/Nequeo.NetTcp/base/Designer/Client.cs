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

namespace Nequeo.Management.NetTcp
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
    
    #region Client Extension Type
    /// <summary>
    /// The Client object class.
    /// </summary>
    public partial class Client
    {
        private Exception _exceptionClient = null;
		private ClientThread _threadClientContext = null;

		/// <summary>
        /// Gets the current async exception; else null;
        /// </summary>
        public Exception ExceptionClient
        {
            get { return _exceptionClient; }
        }

		/// <summary>
        /// Gets the Client threading context.
        /// </summary>
        public ClientThread ClientThreadContext
        {
            get { return _threadClientContext; }
        }

		/// <summary>
        /// On create.
        /// </summary>
        partial void OnCreated();

		/// <summary>
        /// On create instance of Client
        /// </summary>
		partial void OnCreated()
		{
			// Start the async control.
			_threadClientContext = new ClientThread(this);
			_threadClientContext.AsyncError += new Threading.EventHandler<Exception>(_asyncAccount_AsyncError);
		}

		/// <summary>
        /// Async error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e1"></param>
        private void _asyncAccount_AsyncError(object sender, Exception e1)
        {
            _exceptionClient = e1;
        }

		/// <summary>
        /// Client threading handler.
        /// </summary>
        public class ClientThread : Nequeo.Threading.AsyncExecutionHandler<Client>
        {
            /// <summary>
            /// Client threading handler.
            /// </summary>
            /// <param name="service">The Client type.</param>
            public ClientThread(Client service)
                : base(service) { }
        }
    }
    #endregion
}

#pragma warning restore 169
