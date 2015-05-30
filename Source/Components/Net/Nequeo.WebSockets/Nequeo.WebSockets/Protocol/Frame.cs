/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Net.WebSockets;
using System.Web;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Specialized;

using Nequeo.Extension;
using Nequeo.Net.WebSockets;
using Nequeo.IO.Stream.Extension;
using Nequeo.Net.WebSockets.Common;
using Nequeo.IO.Compression;

namespace Nequeo.Net.WebSockets.Protocol
{
    /// <summary>
    /// Web socket frame structure.
    /// </summary>
    internal class Frame : IEnumerable<byte>
    {
        /// <summary>
        /// Web socket frame structure.
        /// </summary>
        static Frame()
        {
            EmptyUnmaskPingBytes = CreatePingFrame(false).ToByteArray();
        }

        /// <summary>
        /// Web socket frame structure.
        /// </summary>
        private Frame()
        {
        }

        /// <summary>
        /// Web socket frame structure.
        /// </summary>
        /// <param name="opcode">Represents the frame type of the WebSocket frame as defined in section 11.8 of the WebSocket protocol spec.</param>
        /// <param name="payloadData">The payload.</param>
        /// <param name="mask">True if masked.</param>
        public Frame(OpCodeFrame opcode, Payload payloadData, bool mask)
            : this(Fin.Final, opcode, payloadData, false, mask)
        {
        }

        /// <summary>
        /// Web socket frame structure.
        /// </summary>
        /// <param name="fin">The state of the data.</param>
        /// <param name="opcode">Represents the frame type of the WebSocket frame as defined in section 11.8 of the WebSocket protocol spec.</param>
        /// <param name="data">The payload.</param>
        /// <param name="compressed">True to compress the data.</param>
        /// <param name="mask">True if masked.</param>
        public Frame(Fin fin, OpCodeFrame opcode, byte[] data, bool compressed, bool mask)
            : this(fin, opcode, new Payload(data), compressed, mask)
        {
        }

        /// <summary>
        /// Web socket frame structure.
        /// </summary>
        /// <param name="fin">The state of the data.</param>
        /// <param name="opcode">Represents the frame type of the WebSocket frame as defined in section 11.8 of the WebSocket protocol spec.</param>
        /// <param name="payloadData">The payload.</param>
        /// <param name="compressed">True to compress the data.</param>
        /// <param name="mask">True if masked.</param>
        public Frame(Fin fin, OpCodeFrame opcode, Payload payloadData, bool compressed, bool mask)
        {
            _fin = fin;
            _rsv1 = IsDataEx(opcode) && compressed ? Rsv.On : Rsv.Off;
            _rsv2 = Rsv.Off;
            _rsv3 = Rsv.Off;
            _opcode = opcode;

            var len = payloadData.Length;
            if (len < 126)
            {
                _payloadLength = (byte)len;
                _extPayloadLength = new byte[0];
            }
            else if (len < 0x010000)
            {
                _payloadLength = (byte)126;
                _extPayloadLength = ((ushort)len).InternalToByteArray(Nequeo.Custom.ByteOrder.Big);
            }
            else
            {
                _payloadLength = (byte)127;
                _extPayloadLength = len.InternalToByteArray(Nequeo.Custom.ByteOrder.Big);
            }

            if (mask)
            {
                _mask = Mask.Mask;
                _maskingKey = CreateMaskingKey();
                payloadData.Mask(_maskingKey);
            }
            else
            {
                _mask = Mask.Unmask;
                _maskingKey = new byte[0];
            }

            _payloadData = payloadData;
        }

        private byte[] _extPayloadLength;
        private Fin _fin;
        private Mask _mask;
        private byte[] _maskingKey;
        private OpCodeFrame _opcode;
        private Payload _payloadData;
        private byte _payloadLength;
        private Rsv _rsv1;
        private Rsv _rsv2;
        private Rsv _rsv3;

        /// <summary>
        /// Empty unmasked ping frame.
        /// </summary>
        internal static readonly byte[] EmptyUnmaskPingBytes;

        /// <summary>
        /// Gets the extented payload length.
        /// </summary>
        public byte[] ExtendedPayloadLength
        {
            get { return _extPayloadLength; }
        }

        /// <summary>
        /// Gets the final state.
        /// </summary>
        public Fin Fin
        {
            get { return _fin; }
        }

        /// <summary>
        /// Gets an indicator specifying if the payload is binary.
        /// </summary>
        public bool IsBinary
        {
            get { return _opcode == OpCodeFrame.Binary; }
        }

        /// <summary>
        /// Gets an indicator specifying if the payload is closed.
        /// </summary>
        public bool IsClose
        {
            get { return _opcode == OpCodeFrame.Close; }
        }

        /// <summary>
        /// Gets an indicator specifying if the payload is compressed.
        /// </summary>
        public bool IsCompressed
        {
            get { return _rsv1 == Rsv.On; }
        }

        /// <summary>
        /// Gets an indicator specifying if the payload is continuation.
        /// </summary>
        public bool IsContinuation
        {
            get { return _opcode == OpCodeFrame.Continuation; }
        }

        /// <summary>
        /// Gets an indicator specifying if the payload is control.
        /// </summary>
        public bool IsControl
        {
            get { return _opcode == OpCodeFrame.Close || _opcode == OpCodeFrame.Ping || _opcode == OpCodeFrame.Pong; }
        }

        /// <summary>
        /// Gets an indicator specifying if the payload is data.
        /// </summary>
        public bool IsData
        {
            get { return _opcode == OpCodeFrame.Binary || _opcode == OpCodeFrame.Text; }
        }

        /// <summary>
        /// Gets an indicator specifying if the payload is final.
        /// </summary>
        public bool IsFinal
        {
            get { return _fin == Fin.Final; }
        }

        /// <summary>
        /// Gets an indicator specifying if the payload is fragmented.
        /// </summary>
        public bool IsFragmented
        {
            get { return _fin == Fin.More || _opcode == OpCodeFrame.Continuation; }
        }

        /// <summary>
        /// Gets an indicator specifying if the payload is masked.
        /// </summary>
        public bool IsMasked
        {
            get { return _mask == Mask.Mask; }
        }

        /// <summary>
        /// Gets an indicator specifying if the payload is per message compressed.
        /// </summary>
        public bool IsPerMessageCompressed
        {
            get { return (_opcode == OpCodeFrame.Binary || _opcode == OpCodeFrame.Text) && _rsv1 == Rsv.On; }
        }

        /// <summary>
        /// Gets an indicator specifying if the payload is ping.
        /// </summary>
        public bool IsPing
        {
            get { return _opcode == OpCodeFrame.Ping; }
        }

        /// <summary>
        /// Gets an indicator specifying if the payload is pong.
        /// </summary>
        public bool IsPong
        {
            get { return _opcode == OpCodeFrame.Pong; }
        }

        /// <summary>
        /// Gets an indicator specifying if the payload is text.
        /// </summary>
        public bool IsText
        {
            get { return _opcode == OpCodeFrame.Text; }
        }

        /// <summary>
        /// Gets the total data length.
        /// </summary>
        public ulong Length
        {
            get { return 2 + (ulong)(_extPayloadLength.Length + _maskingKey.Length) + _payloadData.Length; }
        }

        /// <summary>
        /// Gets the mask indicator.
        /// </summary>
        public Mask Mask
        {
            get { return _mask; }
        }

        /// <summary>
        /// Gets the mask key.
        /// </summary>
        public byte[] MaskingKey
        {
            get { return _maskingKey; }
        }

        /// <summary>
        /// Gets the op code.
        /// </summary>
        public OpCodeFrame Opcode
        {
            get { return _opcode; }
        }

        /// <summary>
        /// Gets the payload.
        /// </summary>
        public Payload PayloadData
        {
            get { return _payloadData; }
        }

        /// <summary>
        /// Gets the payload length.
        /// </summary>
        public byte PayloadLength
        {
            get { return _payloadLength; }
        }

        /// <summary>
        /// Gets the reserved item.
        /// </summary>
        public Rsv Rsv1
        {
            get { return _rsv1; }
        }

        /// <summary>
        /// Gets the reserved item.
        /// </summary>
        public Rsv Rsv2
        {
            get { return _rsv2; }
        }

        /// <summary>
        /// Gets the reserved item.
        /// </summary>
        public Rsv Rsv3
        {
            get { return _rsv3; }
        }

        /// <summary>
        /// Create the close frame.
        /// </summary>
        /// <param name="payloadData">The payload.</param>
        /// <param name="mask">True if masked.</param>
        /// <returns>The web socket frame.</returns>
        public static Frame CreateCloseFrame(Payload payloadData, bool mask)
        {
            return new Frame(Fin.Final, OpCodeFrame.Close, payloadData, false, mask);
        }

        /// <summary>
        /// Create the ping frame.
        /// </summary>
        /// <param name="mask">True if masked.</param>
        /// <returns>The web socket frame.</returns>
        public static Frame CreatePingFrame(bool mask)
        {
            return new Frame(Fin.Final, OpCodeFrame.Ping, new Payload(), false, mask);
        }

        /// <summary>
        /// Create the ping frame.
        /// </summary>
        /// <param name="data">The payload.</param>
        /// <param name="mask">True if masked.</param>
        /// <returns>The web socket frame.</returns>
        public static Frame CreatePingFrame(byte[] data, bool mask)
        {
            return new Frame(Fin.Final, OpCodeFrame.Ping, new Payload(data), false, mask);
        }

        /// <summary>
        /// Read from the stream and create the frame.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The web socket frame.</returns>
        public static Frame Read(Stream stream)
        {
            return Read(stream, true);
        }

        /// <summary>
        /// Read from the stream and create the frame.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="unmask">True if unmasked.</param>
        /// <returns>The web socket frame.</returns>
        public static Frame Read(Stream stream, bool unmask)
        {
            var header = stream.ReadBytes(2);
            if (header.Length != 2)
                throw new WebSocketException(
                  "The header part of a frame cannot be read from the data source.");

            return Read(header, stream, unmask);
        }

        /// <summary>
        /// Read the data async.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="completed">The complete frame.</param>
        /// <param name="error">An error.</param>
        public static void ReadAsync(Stream stream, Action<Frame> completed, Action<Exception> error)
        {
            ReadAsync(stream, true, completed, error);
        }

        /// <summary>
        /// Read the data async.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="unmask">True to unmask.</param>
        /// <param name="completed">The complete frame.</param>
        /// <param name="error">An error.</param>
        public static void ReadAsync(Stream stream, bool unmask, Action<Frame> completed, Action<Exception> error)
        {
            stream.ReadAsync(
              2,
              header =>
              {
                  if (header.Length != 2)
                      throw new WebSocketException(
                        "The header part of a frame cannot be read from the data source.");

                  var frame = Read(header, stream, unmask);
                  if (completed != null)
                      completed(frame);
              },
              error);
        }

        /// <summary>
        /// Read the data.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="unmask">True to unmask.</param>
        /// <param name="completed">The complete frame.</param>
        /// <param name="error">An error.</param>
        public static void Read(Stream stream, bool unmask, Action<Frame> completed, Action<Exception> error)
        {
            stream.Read(
              2,
              header =>
              {
                  if (header.Length != 2)
                      throw new WebSocketException(
                        "The header part of a frame cannot be read from the data source.");

                  var frame = Read(header, stream, unmask);
                  if (completed != null)
                      completed(frame);
              },
              error);
        }

        /// <summary>
        /// Write the frame data.
        /// </summary>
        /// <returns>The frame string data.</returns>
        public string Write()
        {
            return Write(this);
        }

        /// <summary>
        /// Un mask the payload.
        /// </summary>
        public void Unmask()
        {
            if (_mask == Mask.Unmask)
                return;

            _mask = Mask.Unmask;
            _payloadData.Mask(_maskingKey);
            _maskingKey = new byte[0];
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A System.Collections.Generic.IEnumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<byte> GetEnumerator()
        {
            foreach (var item in ToByteArray())
                yield return item;
        }

        /// <summary>
        /// Get the byte array of the message frame.
        /// </summary>
        /// <returns>The byte array.</returns>
        public byte[] ToByteArray()
        {
            using (MemoryStream buff = new MemoryStream())
            {
                var header = (int)_fin;
                header = (header << 1) + (int)_rsv1;
                header = (header << 1) + (int)_rsv2;
                header = (header << 1) + (int)_rsv3;
                header = (header << 4) + (int)_opcode;
                header = (header << 1) + (int)_mask;
                header = (header << 7) + (int)_payloadLength;
                buff.Write(((ushort)header).InternalToByteArray(Nequeo.Custom.ByteOrder.Big), 0, 2);

                if (_payloadLength > 125)
                    buff.Write(_extPayloadLength, 0, _extPayloadLength.Length);

                if (_mask == Mask.Mask)
                    buff.Write(_maskingKey, 0, _maskingKey.Length);

                if (_payloadLength > 0)
                {
                    var payload = _payloadData.ToByteArray();
                    if (_payloadLength < 127)
                        buff.Write(payload, 0, payload.Length);
                    else
                        buff.WriteBytes(payload);
                }

                buff.Close();
                return buff.ToArray();
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return BitConverter.ToString(ToByteArray());
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator object that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Create the masking key.
        /// </summary>
        /// <returns>The byte array.</returns>
        private static byte[] CreateMaskingKey()
        {
            var key = new byte[4];
            var rand = new Random();
            rand.NextBytes(key);

            return key;
        }

        /// <summary>
        /// Dump the frame.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <returns>The string data.</returns>
        private static string Dump(Frame frame)
        {
            var len = frame.Length;
            var cnt = (long)(len / 4);
            var rem = (int)(len % 4);

            int cntDigit;
            string cntFmt;
            if (cnt < 10000)
            {
                cntDigit = 4;
                cntFmt = "{0,4}";
            }
            else if (cnt < 0x010000)
            {
                cntDigit = 4;
                cntFmt = "{0,4:X}";
            }
            else if (cnt < 0x0100000000)
            {
                cntDigit = 8;
                cntFmt = "{0,8:X}";
            }
            else
            {
                cntDigit = 16;
                cntFmt = "{0,16:X}";
            }

            var spFmt = String.Format("{{0,{0}}}", cntDigit);
            var headerFmt = String.Format(@"{0} 01234567 89ABCDEF 01234567 89ABCDEF {0}+--------+--------+--------+--------+\n", spFmt);
            var lineFmt = String.Format("{0}|{{1,8}} {{2,8}} {{3,8}} {{4,8}}|\n", cntFmt);
            var footerFmt = String.Format("{0}+--------+--------+--------+--------+", spFmt);

            var output = new StringBuilder(64);
            Func<Action<string, string, string, string>> linePrinter = () =>
            {
                long lineCnt = 0;
                return (arg1, arg2, arg3, arg4) =>
                  output.AppendFormat(lineFmt, ++lineCnt, arg1, arg2, arg3, arg4);
            };

            output.AppendFormat(headerFmt, String.Empty);

            var printLine = linePrinter();
            var bytes = frame.ToByteArray();
            for (long i = 0; i <= cnt; i++)
            {
                var j = i * 4;
                if (i < cnt)
                    printLine(
                      Convert.ToString(bytes[j], 2).PadLeft(8, '0'),
                      Convert.ToString(bytes[j + 1], 2).PadLeft(8, '0'),
                      Convert.ToString(bytes[j + 2], 2).PadLeft(8, '0'),
                      Convert.ToString(bytes[j + 3], 2).PadLeft(8, '0'));
                else if (rem > 0)
                    printLine(
                      Convert.ToString(bytes[j], 2).PadLeft(8, '0'),
                      rem >= 2 ? Convert.ToString(bytes[j + 1], 2).PadLeft(8, '0') : String.Empty,
                      rem == 3 ? Convert.ToString(bytes[j + 2], 2).PadLeft(8, '0') : String.Empty,
                      String.Empty);
            }

            output.AppendFormat(footerFmt, String.Empty);
            return output.ToString();
        }

        /// <summary>
        /// Is control.
        /// </summary>
        /// <param name="opcode">The opcode.</param>
        /// <returns>True if is control.</returns>
        private static bool IsControlEx(OpCodeFrame opcode)
        {
            return opcode == OpCodeFrame.Close || opcode == OpCodeFrame.Ping || opcode == OpCodeFrame.Pong;
        }

        /// <summary>
        /// Is data.
        /// </summary>
        /// <param name="opcode">The opcode.</param>
        /// <returns>True if is data.</returns>
        private static bool IsDataEx(OpCodeFrame opcode)
        {
            return opcode == OpCodeFrame.Text || opcode == OpCodeFrame.Binary;
        }

        /// <summary>
        /// Write the frame data.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <returns>The frame string data.</returns>
        private static string Write(Frame frame)
        {
            /* Opcode */
            var opcode = frame._opcode.ToString();

            /* Payload Length */
            var payloadLen = frame._payloadLength;

            /* Extended Payload Length */
            var extPayloadLen = payloadLen < 126
                                ? String.Empty
                                : payloadLen == 126
                                  ? frame._extPayloadLength.ToUInt16(Nequeo.Custom.ByteOrder.Big).ToString()
                                  : frame._extPayloadLength.ToUInt64(Nequeo.Custom.ByteOrder.Big).ToString();

            /* Masking Key */
            var masked = frame.IsMasked;
            var maskingKey = masked ? BitConverter.ToString(frame._maskingKey) : String.Empty;

            /* Payload Data */
            var payload = payloadLen == 0
                          ? String.Empty
                          : payloadLen > 125
                            ? String.Format("A {0} frame.", opcode.ToLower())
                            : !masked && !frame.IsFragmented && !frame.IsCompressed && frame.IsText
                              ? Encoding.UTF8.GetString(frame._payloadData.ApplicationData)
                              : frame._payloadData.ToString();

            var fmt = @"
                    FIN: {0}
                    RSV1: {1}
                    RSV2: {2}
                    RSV3: {3}
                    Opcode: {4}
                    MASK: {5}
                    Payload Length: {6}
                    Extended Payload Length: {7}
                    Masking Key: {8}
                    Payload Data: {9}";

            return String.Format(
              fmt,
              frame._fin,
              frame._rsv1,
              frame._rsv2,
              frame._rsv3,
              opcode,
              frame._mask,
              payloadLen,
              extPayloadLen,
              maskingKey,
              payload);
        }

        /// <summary>
        /// Read the frame data.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="stream">The stream containing the payload.</param>
        /// <param name="unmask">True if unmasked.</param>
        /// <returns>The created web socket frame.</returns>
        private static Frame Read(byte[] header, Stream stream, bool unmask)
        {
            bool completed = true;

            /* Header */

            // FIN
            var fin = (header[0] & 0x80) == 0x80 ? Fin.Final : Fin.More;
            // RSV1
            var rsv1 = (header[0] & 0x40) == 0x40 ? Rsv.On : Rsv.Off;
            // RSV2
            var rsv2 = (header[0] & 0x20) == 0x20 ? Rsv.On : Rsv.Off;
            // RSV3
            var rsv3 = (header[0] & 0x10) == 0x10 ? Rsv.On : Rsv.Off;
            // Opcode
            var opcode = (OpCodeFrame)(header[0] & 0x0f);
            // MASK
            var mask = (header[1] & 0x80) == 0x80 ? Mask.Mask : Mask.Unmask;
            // Payload Length
            var payloadLen = (byte)(header[1] & 0x7f);

            // Check if valid header
            var err = IsControlEx(opcode) && payloadLen > 125
                      ? "A control frame has a payload data which is greater than the allowable max size."
                      : IsControlEx(opcode) && fin == Fin.More
                        ? "A control frame is fragmented."
                        : !IsDataEx(opcode) && rsv1 == Rsv.On
                          ? "A non data frame is compressed."
                          : null;

            if (err != null)
                throw new Nequeo.Net.WebSockets.WebSocketException(CloseCodeStatus.ProtocolError, err);

            var frame = new Frame();
            frame._fin = fin;
            frame._rsv1 = rsv1;
            frame._rsv2 = rsv2;
            frame._rsv3 = rsv3;
            frame._opcode = opcode;
            frame._mask = mask;
            frame._payloadLength = payloadLen;

            /* Extended Payload Length */

            var size = payloadLen < 126
                       ? 0
                       : payloadLen == 126
                         ? 2
                         : 8;

            var extPayloadLen = size > 0 ? stream.ReadBytesTimer(size, out completed, 20000) : new byte[0];
            if (size > 0 && extPayloadLen.Length != size)
                throw new WebSocketException(
                  "The 'Extended Payload Length' of a frame cannot be read from the data source.");

            frame._extPayloadLength = extPayloadLen;

            /* Masking Key */

            var masked = mask == Mask.Mask;
            var maskingKey = masked ? stream.ReadBytesTimer(4, out completed, 20000) : new byte[0];
            if (masked && maskingKey.Length != 4)
                throw new WebSocketException(
                  "The 'Masking Key' of a frame cannot be read from the data source.");

            frame._maskingKey = maskingKey;

            /* Payload Data */

            ulong len = payloadLen < 126
                        ? payloadLen
                        : payloadLen == 126
                          ? extPayloadLen.ToUInt16(Nequeo.Custom.ByteOrder.Big)
                          : extPayloadLen.ToUInt64(Nequeo.Custom.ByteOrder.Big);

            byte[] data = null;
            if (len > 0)
            {
                // Check if allowable max length.
                if (payloadLen > 126 && len > Payload.MaxLength)
                    throw new Nequeo.Net.WebSockets.WebSocketException(
                      CloseCodeStatus.MessageTooBig,
                      "The length of 'Payload Data' of a frame is greater than the allowable max length.");

                data = payloadLen > 126
                       ? stream.ReadBytesTimer((int)len, out completed, 20000)
                       : stream.ReadBytesTimer((int)len, out completed, 20000);

                if (data.LongLength != (long)len)
                    throw new WebSocketException(
                      "The 'Payload Data' of a frame cannot be read from the data source.");
            }
            else
            {
                data = new byte[0];
            }

            frame._payloadData = new Payload(data, masked);
            if (unmask && masked)
                frame.Unmask();

            return frame;
        }
    }
}
