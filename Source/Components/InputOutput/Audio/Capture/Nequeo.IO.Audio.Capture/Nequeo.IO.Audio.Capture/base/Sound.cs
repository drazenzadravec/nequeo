/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

using Nequeo.IO.Audio.Capture;

namespace Nequeo.IO.Audio.Directx
{
    /// <summary>
    /// 
    /// </summary>
	public class Sound
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="format"></param>
        public Sound(IDevice device, IFormat format)
        {
            _device = device;
            _format = format;
        }

        private const int _numberRecordNotifications = 16;

        private IDevice _device = null;
        private IFormat _format = null;
        private Stream _outputStream = null;

        private bool _capturing = false;

        private BufferPositionNotify[] _positionNotify = new BufferPositionNotify[_numberRecordNotifications + 1];
        private CaptureBuffer _applicationBuffer = null;
        private Microsoft.DirectX.DirectSound.Capture _applicationDevice = null;
        private Notify _applicationNotify = null;
        private WaveFormat _cachedInputFormat;

        /// <summary>
        /// Releases notification listener. The event is signaled either by DirectX engine
        /// or by Stop() operation (the latter to dispose DirectX within notification thread).
        /// </summary>
        private AutoResetEvent _notificationArrivalEvent = null;

        /// <summary>
        /// This event is signaled by notification thread just after DirectX objects have been
        /// disposed, that is when the capturing session has been definitely finished.
        /// </summary>
        private AutoResetEvent _directSoundDisposedEvent = null;
        private Thread _notificationListenerThread = null;

        private int _captureBufferSize = 0;
        private int _nextCaptureOffset = 0;
        private int _sampleByteCount = 0;
        private int _notifySize = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputStream"></param>
        public void Start(Stream outputStream)
        {
            _outputStream = outputStream;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            
        }
	}
}
