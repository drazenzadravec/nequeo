/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
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
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nequeo.VoIP.PjSip.Data
{
    /// <summary>
    /// Common.
    /// </summary>
    internal class Common
    {
        /// <summary>
        /// The collection of audio codecs.
        /// </summary>
        public Nequeo.Net.PjSip.CodecInfo[] AudioCodecs = null;

        /// <summary>
        /// The collection of video codecs.
        /// </summary>
        public Nequeo.Net.PjSip.CodecInfo[] VideoCodecs = null;

        /// <summary>
        /// Account name.
        /// </summary>
        public string AccountName = "";

        /// <summary>
        /// Sip host.
        /// </summary>
        public string SipHost = "";

        /// <summary>
        /// Sip username.
        /// </summary>
        public string SipUsername = "";

        /// <summary>
        /// Sip password.
        /// </summary>
        public string SipPassword = "";

        /// <summary>
        /// The incoming call ring file path.
        /// </summary>
        public string IncomingCallRingFilePath = null;

        /// <summary>
        /// Instant message file path.
        /// </summary>
        public string InstantMessageFilePath = null;

        /// <summary>
        /// Auto answer file path.
        /// </summary>
        public string AutoAnswerFilePath = null;

        /// <summary>
        /// Auto answer.
        /// </summary>
        public bool AutoAnswer = false;

        /// <summary>
        /// Auto answer wait time (seconds).
        /// </summary>
        public int AutoAnswerWait = 0;

        /// <summary>
        /// Message bank wait time (seconds).
        /// </summary>
        public int MessageBankWait = 0;

        /// <summary>
        /// The audio device index.
        /// </summary>
        public int AudioDeviceIndex = -1;

        /// <summary>
        /// Capture audio device index.
        /// </summary>
        public int CaptureAudioDeviceIndex = -1;

        /// <summary>
        /// Playback audio device index.
        /// </summary>
        public int PlaybackAudioDeviceIndex = -1;

        /// <summary>
        /// Enable outgoing call audio recording.
        /// </summary>
        public bool OutgoingCallAudioRecordingEnabled = false;

        /// <summary>
        /// Enable incoming call audio recording.
        /// </summary>
        public bool IncomingCallAudioRecordingEnabled = false;

        /// <summary>
        /// Enable video.
        /// </summary>
        public bool EnableVideo = false;

        /// <summary>
        /// Video capture index.
        /// </summary>
        public int VideoCaptureIndex = -1;

        /// <summary>
        /// Video render index.
        /// </summary>
        public int VideoRenderIndex = -1;

        /// <summary>
        /// Enable call redirect.
        /// </summary>
        public bool EnableRedirect = false;

        /// <summary>
        /// Redirect call number.
        /// </summary>
        public string RedirectCallNumber = "";

        /// <summary>
        /// Redirect call after.
        /// </summary>
        public int RedirectCallAfter = 30;
    }

    /// <summary>
    /// Incomming and outgoing calls.
    /// </summary>
    internal class IncomingOutgoingCalls : System.Collections.IEnumerable
    {
        /// <summary>
        /// Incomming and outgoing calls.
        /// </summary>
        public IncomingOutgoingCalls()
        {
            _callInfoList = new List<Param.CallInfoParam>();
        }

        private List<Nequeo.VoIP.PjSip.Param.CallInfoParam> _callInfoList = null;

        /// <summary>
        /// Get the count.
        /// </summary>
        public int Count
        {
            get { return _callInfoList.Count; }
        }

        /// <summary>
        /// Add the call information.
        /// </summary>
        /// <param name="callInfo">The call information.</param>
        public void Add(Nequeo.VoIP.PjSip.Param.CallInfoParam callInfo)
        {
            _callInfoList.Add(callInfo);
        }

        /// <summary>
        /// Remove the call information.
        /// </summary>
        /// <param name="callInfo">The call information.</param>
        public void Remove(Nequeo.VoIP.PjSip.Param.CallInfoParam callInfo)
        {
            _callInfoList.Remove(callInfo);
        }

        /// <summary>
        /// Get all the call information.
        /// </summary>
        /// <returns>Get the call info</returns>
        public Nequeo.VoIP.PjSip.Param.CallInfoParam Get(int index)
        {
            return _callInfoList[index];
        }

        /// <summary>
        /// Get all the call information.
        /// </summary>
        /// <returns>Get the call info</returns>
        public Nequeo.VoIP.PjSip.Param.CallInfoParam GetCallInfo(int callID)
        {
            Nequeo.VoIP.PjSip.Param.CallInfoParam callInfo = null;
            try
            {
                callInfo = _callInfoList.First(u => u.CallID == callID);
            }
            catch { callInfo = null; }
            return callInfo;
        }

        /// <summary>
        /// Get all the call information.
        /// </summary>
        /// <returns>All the call info param.</returns>
        public Nequeo.VoIP.PjSip.Param.CallInfoParam[] GetAll()
        {
            return _callInfoList.ToArray();
        }

        /// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An System.Collections.IEnumerator object that can be used to iterate through the collection.</returns>
		public IEnumerator GetEnumerator()
        {
            return _callInfoList.GetEnumerator();
        }
    }
}
