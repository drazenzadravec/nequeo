/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System.Runtime.InteropServices;
using System;
namespace Nequeo.IO.Audio.Api.Interfaces
{
    [Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionEnumerator
    {
        int GetCount( out int SessionCount);
        int GetSession( int SessionCount,out IAudioSessionControl Session );
    }
}
