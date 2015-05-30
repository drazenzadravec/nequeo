/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Nequeo.IO.Audio.Wave;

namespace Nequeo.IO.Audio.Formats
{
    /// <summary>
    /// Custom marshaller for WaveFormat structures
    /// </summary>
    internal sealed class WaveFormatCustomMarshaler : ICustomMarshaler
    {
        private static WaveFormatCustomMarshaler marshaler = null;

        /// <summary>
        /// Gets the instance of this marshaller
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static ICustomMarshaler GetInstance(string cookie)
        {
            if (marshaler == null)
            {
                marshaler = new WaveFormatCustomMarshaler();
            }
            return marshaler;
        }

        /// <summary>
        /// Clean up managed data
        /// </summary>
        public void CleanUpManagedData(object ManagedObj)
        {

        }

        /// <summary>
        /// Clean up native data
        /// </summary>
        /// <param name="pNativeData"></param>
        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeHGlobal(pNativeData);
        }

        /// <summary>
        /// Get native data size
        /// </summary>        
        public int GetNativeDataSize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Marshal managed to native
        /// </summary>
        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            return WaveFormatProvider.MarshalToPtr((WaveFormatProvider)ManagedObj);
        }

        /// <summary>
        /// Marshal Native to Managed
        /// </summary>
        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            return WaveFormatProvider.MarshalFromPtr(pNativeData);
        }
    }
}
