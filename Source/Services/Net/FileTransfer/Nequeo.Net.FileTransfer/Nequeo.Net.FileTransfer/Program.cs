using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.Net.FileTransfer
{
    /// <summary>
    /// The application starting point.
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
                new Service.FileTransfer(),
                new Service.FileTransferSsl() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
