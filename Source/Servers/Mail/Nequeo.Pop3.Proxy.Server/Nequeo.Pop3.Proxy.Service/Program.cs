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
				new Nequeo.Service.Pop3SslProxy(),
                new Nequeo.Service.Pop3TlsProxy() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
