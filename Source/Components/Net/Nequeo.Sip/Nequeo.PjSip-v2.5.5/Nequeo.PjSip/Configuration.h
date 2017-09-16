/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          Configuration.h
*  Purpose :       SIP Endpoint Configuration class.
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

#ifndef _ENDCONFIGURATION_H
#define _ENDCONFIGURATION_H

#include "stdafx.h"

#include "pjsua2.hpp"
#include "pjsua2\endpoint.hpp"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::Runtime::InteropServices;

namespace Nequeo
{
	namespace Net
	{
		namespace PjSip
		{
			///	<summary>
			///	Log file header type.
			///	</summary>
			public enum class LogHeaderFlag : unsigned
			{
				///	<summary>
				/// Include day name [default: no]
				///	</summary>
				LOG_HAS_DAY_NAME = 1,
				///	<summary>
				/// Include year digit [no]
				///	</summary>
				LOG_HAS_YEAR = 2,
				///	<summary>
				/// Include month [no]
				///	</summary>
				LOG_HAS_MONTH = 4,
				///	<summary>
				/// Include day of month [no]
				///	</summary>
				LOG_HAS_DAY_OF_MON = 8,
				///	<summary>
				/// Include time [yes]
				///	</summary>
				LOG_HAS_TIME = 16,
				///	<summary>
				/// Include microseconds [yes]
				///	</summary>
				LOG_HAS_MICRO_SEC = 32,
				///	<summary>
				/// Include sender in the log [yes]
				///	</summary>
				LOG_HAS_SENDER = 64,
				///	<summary>
				/// Terminate each call with newline [yes]
				///	</summary>
				LOG_HAS_NEWLINE = 128,
				///	<summary>
				/// Include carriage return [no]
				///	</summary>
				LOG_HAS_CR = 256,
				///	<summary>
				/// Include two spaces before log [yes]
				///	</summary>
				LOG_HAS_SPACE = 512,
				///	<summary>
				/// Colorize logs [yes on win32]
				///	</summary>
				LOG_HAS_COLOR = 1024,
				///	<summary>
				/// Include level text string [no]
				///	</summary>
				LOG_HAS_LEVEL_TEXT = 2048,
				///	<summary>
				/// Include thread identification [no]
				///	</summary>
				LOG_HAS_THREAD_ID = 4096,
				///	<summary>
				/// Add mark when thread has switched [yes]
				///	</summary>
				LOG_HAS_THREAD_SWC = 8192,
				///	<summary>
				/// Indentation [yes]
				///	</summary>
				LOG_HAS_INDENT = 16384
			};

			///	<summary>
			///	Log file write type.
			///	</summary>
			public enum class LogWriteFlag : unsigned
			{
				///	<summary>
				/// Open file for writing (overwriting the file).
				///	</summary>
				FILE_DEFAULT = 0x0,
				///	<summary>
				/// Open file for writing (overwriting the file).
				///	</summary>
				FILE_WRITING = 0x1102,
				///	<summary>
				/// Append to existing file.
				///	</summary>
				FILE_APPEND = 0x1108
			};

			///	<summary>
			///	Endpoint configuration.
			///	</summary>
			class EndPointConfiguration
			{
			public:
				///	<summary>
				///	Endpoint configuration.
				///	</summary>
				EndPointConfiguration() : 
					_disposed(false), 
					_hasConfig(false), 
					_fileName("voip_sip.log"),
					_log(false),
					_logLevel(0),
					_logLevelConsole(0),
					_logFileFlag(0),
					_logFileDecor(25328),
					_port_UDP_v4(0),
					_port_UDP_v6(0),
					_port_TCP_v4(0),
					_port_TCP_v6(0),
					_port_TLS_v4(0),
					_port_TLS_v6(0),
					_portRange_UDP_v4(0),
					_portRange_UDP_v6(0),
					_portRange_TCP_v4(0),
					_portRange_TCP_v6(0),
					_portRange_TLS_v4(0),
					_portRange_TLS_v6(0)
				{}

				///	<summary>
				///	Endpoint configuration.
				///	</summary>
				virtual ~EndPointConfiguration() 
				{
					if (!_disposed)
					{
						_disposed = true;
					}
				}

				///	<summary>
				///	Get has configuration.
				///	</summary>
				inline bool GetHasConfiguration() const
				{
					return _hasConfig;
				}

				///	<summary>
				///	Set has configuration.
				///	</summary>
				/// <param name="hasConfig">The file name and path.</param>
				inline void SetHasConfiguration(bool hasConfig)
				{
					_hasConfig = hasConfig;
				}

				///	<summary>
				///	Get the log file name and path.
				///	</summary>
				inline std::string GetFileName() const
				{
					return _fileName;
				}

				///	<summary>
				///	Set the log file name and path.
				///	</summary>
				/// <param name="fileName">The file name and path.</param>
				inline void SetFileName(const std::string& fileName)
				{
					_fileName = fileName;
				}

				///	<summary>
				///	Get log incomming and outgoing messages.
				///	</summary>
				inline bool GetLogMessages() const
				{
					return _log;
				}

				///	<summary>
				///	Set log incomming and outgoing messages.
				///	</summary>
				/// <param name="logMessages">True to log messages; else false.</param>
				inline void SetLogMessages(bool logMessages)
				{
					_log = logMessages;
				}

				///	<summary>
				///	Get log level.
				///	</summary>
				inline unsigned int GetLogLevel() const
				{
					return _logLevel;
				}

				///	<summary>
				///	Set log level.
				///	</summary>
				/// <param name="level">The log level (0 .. 5).</param>
				inline void SetLogLevel(unsigned int level)
				{
					_logLevel = level;
				}

				///	<summary>
				///	Get console log level.
				///	</summary>
				inline unsigned int GetConsoleLogLevel() const
				{
					return _logLevelConsole;
				}

				///	<summary>
				///	Set console log level.
				///	</summary>
				/// <param name="level">The log level (0 .. 4).</param>
				inline void SetConsoleLogLevel(unsigned int level)
				{
					_logLevelConsole = level;
				}

				///	<summary>
				///	Get the log file write flag.
				///	</summary>
				inline unsigned int GetFileWriteFlag() const
				{
					return _logFileFlag;
				}

				///	<summary>
				///	Set the log file write flag.
				///	</summary>
				/// <param name="flag">The file flag.</param>
				inline void SetFileWriteFlag(unsigned int flag)
				{
					_logFileFlag = flag;
				}

				///	<summary>
				///	Get the log file header flag.
				///	</summary>
				inline unsigned int GetFileHeaderFlag() const
				{
					return _logFileDecor;
				}

				///	<summary>
				///	Set the log file header flag.
				///	</summary>
				/// <param name="flag">The log file header flag.</param>
				inline void SetFileHeaderFlag(unsigned int flag)
				{
					_logFileDecor = flag;
				}


				///	<summary>
				///	Get the UDP v4 transport port (default 0, random port).
				///	</summary>
				inline unsigned int GetPortUDPv4() const
				{
					return _port_UDP_v4;
				}

				///	<summary>
				///	Set the UDP v4 transport port (default 0, random port).
				///	</summary>
				/// <param name="port">The port.</param>
				inline void SetPortUDPv4(unsigned int port)
				{
					_port_UDP_v4 = port;
				}

				///	<summary>
				///	Get the UDP v6 transport port (default 0, random port).
				///	</summary>
				inline unsigned int GetPortUDPv6() const
				{
					return _port_UDP_v6;
				}

				///	<summary>
				///	Set the UDP v6 transport port (default 0, random port).
				///	</summary>
				/// <param name="port">The port.</param>
				inline void SetPortUDPv6(unsigned int port)
				{
					_port_UDP_v6 = port;
				}

				///	<summary>
				///	Get the TCP v4 transport port (default 0, random port).
				///	</summary>
				inline unsigned int GetPortTCPv4() const
				{
					return _port_TCP_v4;
				}

				///	<summary>
				///	Set the TCP v4 transport port (default 0, random port).
				///	</summary>
				/// <param name="port">The port.</param>
				inline void SetPortTCPv4(unsigned int port)
				{
					_port_TCP_v4 = port;
				}

				///	<summary>
				///	Get the TCP v6 transport port (default 0, random port).
				///	</summary>
				inline unsigned int GetPortTCPv6() const
				{
					return _port_TCP_v6;
				}

				///	<summary>
				///	Set the TCP v6 transport port (default 0, random port).
				///	</summary>
				/// <param name="port">The port.</param>
				inline void SetPortTCPv6(unsigned int port)
				{
					_port_TCP_v6 = port;
				}

				///	<summary>
				///	Get the TLS v4 transport port (default 0, random port).
				///	</summary>
				inline unsigned int GetPortTLSv4() const
				{
					return _port_TLS_v4;
				}

				///	<summary>
				///	Set the TLS v4 transport port (default 0, random port).
				///	</summary>
				/// <param name="port">The port.</param>
				inline void SetPortTLSv4(unsigned int port)
				{
					_port_TLS_v4 = port;
				}

				///	<summary>
				///	Get the TLS v6 transport port (default 0, random port).
				///	</summary>
				inline unsigned int GetPortTLSv6() const
				{
					return _port_TLS_v6;
				}

				///	<summary>
				///	Set the TLS v6 transport port (default 0, random port).
				///	</summary>
				/// <param name="port">The port.</param>
				inline void SetPortTLSv6(unsigned int port)
				{
					_port_TLS_v6 = port;
				}

				///	<summary>
				///	Get the UDP v4 transport port range (default 0, random port).
				///	</summary>
				inline unsigned int GetPortRangeUDPv4() const
				{
					return _portRange_UDP_v4;
				}

				///	<summary>
				///	Set the UDP v4 transport port range (default 0, random port).
				///	</summary>
				/// <param name="portRange">The port range.</param>
				inline void SetPortRangeUDPv4(unsigned int portRange)
				{
					_portRange_UDP_v4 = portRange;
				}

				///	<summary>
				///	Get the UDP v6 transport port range (default 0, random port).
				///	</summary>
				inline unsigned int GetPortRangeUDPv6() const
				{
					return _portRange_UDP_v6;
				}

				///	<summary>
				///	Set the UDP v6 transport port range (default 0, random port).
				///	</summary>
				/// <param name="portRange">The port range.</param>
				inline void SetPortRangeUDPv6(unsigned int portRange)
				{
					_portRange_UDP_v6 = portRange;
				}

				///	<summary>
				///	Get the TCP v4 transport port range (default 0, random port).
				///	</summary>
				inline unsigned int GetPortRangeTCPv4() const
				{
					return _portRange_TCP_v4;
				}

				///	<summary>
				///	Set the TCP v4 transport port range (default 0, random port).
				///	</summary>
				/// <param name="portRange">The port range.</param>
				inline void SetPortRangeTCPv4(unsigned int portRange)
				{
					_portRange_TCP_v4 = portRange;
				}

				///	<summary>
				///	Get the TCP v6 transport port range (default 0, random port).
				///	</summary>
				inline unsigned int GetPortRangeTCPv6() const
				{
					return _portRange_TCP_v6;
				}

				///	<summary>
				///	Set the TCP v6 transport port range (default 0, random port).
				///	</summary>
				/// <param name="portRange">The port range.</param>
				inline void SetPortRangeTCPv6(unsigned int portRange)
				{
					_portRange_TCP_v6 = portRange;
				}

				///	<summary>
				///	Get the TLS v4 transport port range (default 0, random port).
				///	</summary>
				inline unsigned int GetPortRangeTLSv4() const
				{
					return _portRange_TLS_v4;
				}

				///	<summary>
				///	Set the TLS v4 transport port range (default 0, random port).
				///	</summary>
				/// <param name="portRange">The port range.</param>
				inline void SetPortRangeTLSv4(unsigned int portRange)
				{
					_portRange_TLS_v4 = portRange;
				}

				///	<summary>
				///	Get the TLS v6 transport port range (default 0, random port).
				///	</summary>
				inline unsigned int GetPortRangeTLSv6() const
				{
					return _portRange_TLS_v6;
				}

				///	<summary>
				///	Set the TLS v6 transport port range (default 0, random port).
				///	</summary>
				/// <param name="portRange">The port range.</param>
				inline void SetPortRangeTLSv6(unsigned int portRange)
				{
					_portRange_TLS_v6 = portRange;
				}

			private:
				bool _disposed;
				bool _hasConfig;
				std::string _fileName;
				bool _log;
				unsigned int _logLevel;
				unsigned int _logLevelConsole;
				unsigned int _logFileFlag;
				unsigned int _logFileDecor;

				unsigned int _port_UDP_v4;
				unsigned int _portRange_UDP_v4;
				unsigned int _port_UDP_v6;
				unsigned int _portRange_UDP_v6;

				unsigned int _port_TCP_v4;
				unsigned int _portRange_TCP_v4;
				unsigned int _port_TCP_v6;
				unsigned int _portRange_TCP_v6;

				unsigned int _port_TLS_v4;
				unsigned int _portRange_TLS_v4;
				unsigned int _port_TLS_v6;
				unsigned int _portRange_TLS_v6;
			};

			///	<summary>
			/// Endpoint configuration.
			///	</summary>
			public ref class EndPointConfig sealed
			{
			public:
				///	<summary>
				/// Endpoint configuration.
				///	</summary>
				EndPointConfig();

				/// <summary>
				/// Gets or sets a value that specifies if an end point configuration should be used.
				/// </summary>
				property bool UseConfig
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets the log filename and path.
				/// </summary>
				property String^ LogFileName
				{
					String^ get();
					void set(String^ value);
				}

				/// <summary>
				/// Gets or sets a value that specifies log incomming and outgoing messages.
				/// </summary>
				property bool LogMessages
				{
					bool get();
					void set(bool value);
				}

				/// <summary>
				/// Gets or sets the log level (0 .. 5).
				/// </summary>
				property UInt32 LogLevel
				{
					UInt32 get();
					void set(UInt32 value);
				}

				/// <summary>
				/// Gets or sets the console log level (0 .. 4).
				/// </summary>
				property UInt32 LogLevelConsole
				{
					UInt32 get();
					void set(UInt32 value);
				}

				/// <summary>
				/// Gets or sets log file header flag.
				/// </summary>
				property LogHeaderFlag LogHeader
				{
					LogHeaderFlag get();
					void set(LogHeaderFlag value);
				}

				/// <summary>
				/// Gets or sets log file write flag.
				/// </summary>
				property LogWriteFlag LogFileWrite
				{
					LogWriteFlag get();
					void set(LogWriteFlag value);
				}

				/// <summary>
				/// Gets or sets the UDP v4 transport port (default 0, random port).
				/// </summary>
				property UInt32 PortUDPv4
				{
					UInt32 get();
					void set(UInt32 value);
				}

				/// <summary>
				/// Gets or sets the UDP v4 transport port range (default 0, random port).
				/// </summary>
				property UInt32 PortRangeUDPv4
				{
					UInt32 get();
					void set(UInt32 value);
				}

				/// <summary>
				/// Gets or sets the UDP v6 transport port (default 0, random port).
				/// </summary>
				property UInt32 PortUDPv6
				{
					UInt32 get();
					void set(UInt32 value);
				}

				/// <summary>
				/// Gets or sets the UDP v6 transport port range (default 0, random port).
				/// </summary>
				property UInt32 PortRangeUDPv6
				{
					UInt32 get();
					void set(UInt32 value);
				}

				/// <summary>
				/// Gets or sets the TCP v4 transport port (default 0, random port).
				/// </summary>
				property UInt32 PortTCPv4
				{
					UInt32 get();
					void set(UInt32 value);
				}

				/// <summary>
				/// Gets or sets the TCP v4 transport port range (default 0, random port).
				/// </summary>
				property UInt32 PortRangeTCPv4
				{
					UInt32 get();
					void set(UInt32 value);
				}

				/// <summary>
				/// Gets or sets the TCP v6 transport port (default 0, random port).
				/// </summary>
				property UInt32 PortTCPv6
				{
					UInt32 get();
					void set(UInt32 value);
				}

				/// <summary>
				/// Gets or sets the TCP v6 transport port range (default 0, random port).
				/// </summary>
				property UInt32 PortRangeTCPv6
				{
					UInt32 get();
					void set(UInt32 value);
				}

				/// <summary>
				/// Gets or sets the TLS v4 transport port (default 0, random port).
				/// </summary>
				property UInt32 PortTLSv4
				{
					UInt32 get();
					void set(UInt32 value);
				}

				/// <summary>
				/// Gets or sets the TLS v4 transport port range (default 0, random port).
				/// </summary>
				property UInt32 PortRangeTLSv4
				{
					UInt32 get();
					void set(UInt32 value);
				}

				/// <summary>
				/// Gets or sets the TLS v6 transport port (default 0, random port).
				/// </summary>
				property UInt32 PortTLSv6
				{
					UInt32 get();
					void set(UInt32 value);
				}

				/// <summary>
				/// Gets or sets the TLS v6 transport port range (default 0, random port).
				/// </summary>
				property UInt32 PortRangeTLSv6
				{
					UInt32 get();
					void set(UInt32 value);
				}

			private:
				bool _hasConfig;
				String^ _fileName;
				bool _logMessages;
				UInt32 _logLevel;
				UInt32 _logLevelConsole;
				LogHeaderFlag _logHeaderFlag;
				LogWriteFlag _logFileWriteFlag;

				UInt32 _portUDPv4;
				UInt32 _portRangeUDPv4;
				UInt32 _portUDPv6;
				UInt32 _portRangeUDPv6;

				UInt32 _portTCPv4;
				UInt32 _portRangeTCPv4;
				UInt32 _portTCPv6;
				UInt32 _portRangeTCPv6;

				UInt32 _portTLSv4;
				UInt32 _portRangeTLSv4;
				UInt32 _portTLSv6;
				UInt32 _portRangeTLSv6;
			};
		}
	}
}
#endif

