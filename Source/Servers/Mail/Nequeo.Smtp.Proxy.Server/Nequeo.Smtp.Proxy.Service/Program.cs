using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Nequeo.Smtp.Proxy.Service
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
				new Nequeo.Service.SmtpSslProxy(),
                new Nequeo.Service.SmtpTlsProxy()
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
