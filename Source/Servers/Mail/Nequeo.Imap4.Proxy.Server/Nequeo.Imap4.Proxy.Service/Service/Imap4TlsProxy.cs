using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Nequeo.Service
{
    /// <summary>
    /// 
    /// </summary>
    partial class Imap4TlsProxy : ServiceBase
    {
        /// <summary>
        /// 
        /// </summary>
        public Imap4TlsProxy()
        {
            InitializeComponent();
            Initialise();
        }

        private Nequeo.Net.Controller.Imap4TlsProxyControl imap4TlsControl = null;

        /// <summary>
        /// 
        /// </summary>
        private void Initialise()
        {
            // Start a new instance of the application controller.
            imap4TlsControl = new Nequeo.Net.Controller.Imap4TlsProxyControl();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            // If the object exists then start all
            // client threads.
            if (imap4TlsControl != null)
                imap4TlsControl.StartServerThreads();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnStop()
        {
            // If the object exists then stop all
            // client threads.
            if (imap4TlsControl != null)
                imap4TlsControl.StopServerThreads();
        }
    }
}
