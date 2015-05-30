/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          WebSocketType.h
*  Purpose :       WebSocketType class.
*
*/

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

#pragma once

#ifndef _WEBSOCKETTYPE_H
#define _WEBSOCKETTYPE_H

#include "GlobalSocket.h"

namespace Nequeo {
	namespace Net {
		namespace Provider
		{
			enum Mode
			{
				WS_SERVER, /// Server-side WebSocket.
				WS_CLIENT  /// Client-side WebSocket.
			};

			enum FrameFlags
				/// Frame header flags.
			{
				FRAME_FLAG_FIN = 0x80, /// FIN bit: final fragment of a multi-fragment message.
				FRAME_FLAG_RSV1 = 0x40, /// Reserved for future use. Must be zero.
				FRAME_FLAG_RSV2 = 0x20, /// Reserved for future use. Must be zero.
				FRAME_FLAG_RSV3 = 0x10, /// Reserved for future use. Must be zero.
			};

			enum FrameOpcodes
				/// Frame header opcodes.
			{
				FRAME_OP_CONT = 0x00, /// Continuation frame.
				FRAME_OP_TEXT = 0x01, /// Text frame.
				FRAME_OP_BINARY = 0x02, /// Binary frame.
				FRAME_OP_CLOSE = 0x08, /// Close connection.
				FRAME_OP_PING = 0x09, /// Ping frame.
				FRAME_OP_PONG = 0x0a, /// Pong frame.
				FRAME_OP_BITMASK = 0x0f  /// Bit mask for opcodes. 
			};

			enum SendFlags
				/// Combined header flags and opcodes for use with sendFrame().
			{
				FRAME_TEXT = FRAME_FLAG_FIN | FRAME_OP_TEXT,
				/// Use this for sending a single text (UTF-8) payload frame.
				FRAME_BINARY = FRAME_FLAG_FIN | FRAME_OP_BINARY
				/// Use this for sending a single binary payload frame.
			};

			enum StatusCodes
				/// StatusCodes for CLOSE frames sent with shutdown().
			{
				WS_NORMAL_CLOSE = 1000,
				WS_ENDPOINT_GOING_AWAY = 1001,
				WS_PROTOCOL_ERROR = 1002,
				WS_PAYLOAD_NOT_ACCEPTABLE = 1003,
				WS_RESERVED = 1004,
				WS_RESERVED_NO_STATUS_CODE = 1005,
				WS_RESERVED_ABNORMAL_CLOSE = 1006,
				WS_MALFORMED_PAYLOAD = 1007,
				WS_POLICY_VIOLATION = 1008,
				WS_PAYLOAD_TOO_BIG = 1009,
				WS_EXTENSION_REQUIRED = 1010,
				WS_UNEXPECTED_CONDITION = 1011,
				WS_RESERVED_TLS_FAILURE = 1015
			};

			enum ErrorCodes
				/// These error codes can be obtained from a WebSocketException
				/// to determine the exact cause of the error.
			{
				WS_ERR_NO_HANDSHAKE = 1,
				/// No Connection: Upgrade or Upgrade: websocket header in handshake request.
				WS_ERR_HANDSHAKE_NO_VERSION = 2,
				/// No Sec-WebSocket-Version header in handshake request.
				WS_ERR_HANDSHAKE_UNSUPPORTED_VERSION = 3,
				/// Unsupported WebSocket version requested by client.
				WS_ERR_HANDSHAKE_NO_KEY = 4,
				/// No Sec-WebSocket-Key header in handshake request.
				WS_ERR_HANDSHAKE_ACCEPT = 5,
				/// No Sec-WebSocket-Accept header or wrong value.
				WS_ERR_UNAUTHORIZED = 6,
				/// The server rejected the username or password for authentication.
				WS_ERR_PAYLOAD_TOO_BIG = 10,
				/// Payload too big for supplied buffer.
				WS_ERR_INCOMPLETE_FRAME = 11
				/// Incomplete frame received.
			};
		}
	}
}
#endif