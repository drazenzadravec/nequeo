using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.Http.Service
{
    /// <summary>
    /// The application starting point.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Common server name.
        /// </summary>
        internal static string CommonServerName = "Nequeo Application Server 19.7.3";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new Service.HttpStaticServer(),
                new Service.HttpStaticSingleServer(),
                new Service.HttpDynamicSingleServer()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
