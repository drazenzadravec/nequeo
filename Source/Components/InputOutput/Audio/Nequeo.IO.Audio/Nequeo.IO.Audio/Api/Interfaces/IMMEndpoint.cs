/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Nequeo.IO.Audio.Api.Interfaces
{
    [Guid("1BE09788-6894-4089-8586-9A2A6C265AC5"),
      InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMEndpoint 
    {
        [PreserveSig]
        int GetDataFlow(out EDataFlow pDataFlow);
    }; 
}
