/*  Company :       Nequeo Pty Ltd, http://www.Nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2008 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 */

using System;
using System.Collections.Generic;
using System.Text;
using Nequeo.IO.Audio.Api.Interfaces;
using System.Runtime.InteropServices;

namespace Nequeo.IO.Audio.Api
{
    internal class SessionCollection
    {
        IAudioSessionEnumerator _AudioSessionEnumerator;
        internal SessionCollection(IAudioSessionEnumerator realEnumerator)
        {
            _AudioSessionEnumerator = realEnumerator;
        }

        public AudioSessionControl this[int index]
        {
            get
            {
                IAudioSessionControl _Result;
                Marshal.ThrowExceptionForHR(_AudioSessionEnumerator.GetSession(index, out _Result));
                return new AudioSessionControl(_Result);
            }
        }

        public int Count
        {
            get
            {
                int result;
                Marshal.ThrowExceptionForHR(_AudioSessionEnumerator.GetCount(out result));
                return (int)result;
            }
        }
    }
}
