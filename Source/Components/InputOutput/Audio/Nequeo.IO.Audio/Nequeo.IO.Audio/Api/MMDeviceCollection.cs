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
using Nequeo.IO.Audio.Api.Interfaces;

namespace Nequeo.IO.Audio.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class MMDeviceCollection
    {
        private IMMDeviceCollection _MMDeviceCollection;

        public int Count
        {
            get
            {
                uint result;
                Marshal.ThrowExceptionForHR(_MMDeviceCollection.GetCount(out result));
                return (int)result;
            }
        }

        public MMDevice this[int index]
        {
            get
            {
                IMMDevice result;
                _MMDeviceCollection.Item((uint)index, out result);
                return new MMDevice(result);
            }
        }

        internal MMDeviceCollection(IMMDeviceCollection parent)
        {
            _MMDeviceCollection = parent;
        }
    }
}
