using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Nequeo.Diagnostics
{
    /// <summary>
    /// 
    /// </summary>
	public class XmlTraceListener : TraceListener
	{
        /// <summary>
        /// 
        /// </summary>
        public XmlTraceListener()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public XmlTraceListener(string filename) 
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void Write(string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void WriteLine(string message)
        {
            throw new NotImplementedException();
        }
    }
}
