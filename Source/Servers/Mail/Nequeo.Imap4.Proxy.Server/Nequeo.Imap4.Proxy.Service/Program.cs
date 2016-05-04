using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Nequeo
{
    /// <summary>
    /// 
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new Nequeo.Service.Imap4SslProxy(),
                new Nequeo.Service.Imap4TlsProxy() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
