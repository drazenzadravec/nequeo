/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          Configuration.cpp
*  Purpose :       SIP Configuration class.
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

#include "stdafx.h"

#include "Configuration.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
/// Endpoint configuration.
///	</summary>
EndPointConfig::EndPointConfig() : _hasConfig(false), _fileName("voip_sip.log"),
	_logMessages(false), _logLevel(0), _logLevelConsole(0), _logFileWriteFlag(LogWriteFlag::FILE_DEFAULT),
	_logHeaderFlag(
		LogHeaderFlag::LOG_HAS_CR | 
		LogHeaderFlag::LOG_HAS_DAY_OF_MON |
		LogHeaderFlag::LOG_HAS_MONTH |
		LogHeaderFlag::LOG_HAS_YEAR |
		LogHeaderFlag::LOG_HAS_TIME |
		LogHeaderFlag::LOG_HAS_SPACE |
		LogHeaderFlag::LOG_HAS_INDENT |
		LogHeaderFlag::LOG_HAS_LEVEL_TEXT |
		LogHeaderFlag::LOG_HAS_NEWLINE |
		LogHeaderFlag::LOG_HAS_SENDER),
	_portUDPv4(0), _portRangeUDPv4(0), _portUDPv6(0), _portRangeUDPv6(0),
	_portTCPv4(0), _portRangeTCPv4(0), _portTCPv6(0), _portRangeTCPv6(0),
	_portTLSv4(0), _portRangeTLSv4(0), _portTLSv6(0), _portRangeTLSv6(0)

{
}

/// <summary>
/// Gets or sets a value that specifies if an end point configuration should be used.
/// </summary>
bool EndPointConfig::UseConfig::get()
{
	return _hasConfig;
}

/// <summary>
/// Gets or sets a value that specifies if an end point configuration should be used.
/// </summary>
void EndPointConfig::UseConfig::set(bool value)
{
	_hasConfig = value;
}

/// <summary>
/// Gets or sets the log filename and path.
/// </summary>
String^ EndPointConfig::LogFileName::get()
{
	return _fileName;
}

/// <summary>
/// Gets or sets the log filename and path.
/// </summary>
void EndPointConfig::LogFileName::set(String^ value)
{
	_fileName = value;
}

/// <summary>
/// Gets or sets a value that specifies log incomming and outgoing messages.
/// </summary>
bool EndPointConfig::LogMessages::get()
{
	return _logMessages;
}

/// <summary>
/// Gets or sets a value that specifies log incomming and outgoing messages.
/// </summary>
void EndPointConfig::LogMessages::set(bool value)
{
	_logMessages = value;
}

/// <summary>
/// Gets or sets the log level (0 .. 5).
/// </summary>
UInt32 EndPointConfig::LogLevel::get()
{
	return _logLevel;
}

/// <summary>
/// Gets or sets the log level (0 .. 5).
/// </summary>
void EndPointConfig::LogLevel::set(UInt32 value)
{
	_logLevel = value;
}

/// <summary>
/// Gets or sets the console log level (0 .. 4).
/// </summary>
UInt32 EndPointConfig::LogLevelConsole::get()
{
	return _logLevelConsole;
}

/// <summary>
/// Gets or sets the console log level (0 .. 4).
/// </summary>
void EndPointConfig::LogLevelConsole::set(UInt32 value)
{
	_logLevelConsole = value;
}

/// <summary>
/// Gets or sets log file header flag.
/// </summary>
LogHeaderFlag EndPointConfig::LogHeader::get()
{
	return _logHeaderFlag;
}

/// <summary>
/// Gets or sets log file header flag.
/// </summary>
void EndPointConfig::LogHeader::set(LogHeaderFlag value)
{
	_logHeaderFlag = value;
}

/// <summary>
/// Gets or sets log file write flag.
/// </summary>
LogWriteFlag EndPointConfig::LogFileWrite::get()
{
	return _logFileWriteFlag;
}

/// <summary>
/// Gets or sets log file write flag.
/// </summary>
void EndPointConfig::LogFileWrite::set(LogWriteFlag value)
{
	_logFileWriteFlag = value;
}

/// <summary>
/// Gets or sets the UDP v4 transport port (default 0, random port).
/// </summary>
UInt32 EndPointConfig::PortUDPv4::get()
{
	return _portUDPv4;
}

/// <summary>
/// Gets or sets the UDP v4 transport port (default 0, random port).
/// </summary>
void EndPointConfig::PortUDPv4::set(UInt32 value)
{
	_portUDPv4 = value;
}

/// <summary>
/// Gets or sets the UDP v6 transport port (default 0, random port).
/// </summary>
UInt32 EndPointConfig::PortUDPv6::get()
{
	return _portUDPv6;
}

/// <summary>
/// Gets or sets the UDP v6 transport port (default 0, random port).
/// </summary>
void EndPointConfig::PortUDPv6::set(UInt32 value)
{
	_portUDPv6 = value;
}

/// <summary>
/// Gets or sets the TCP v4 transport port (default 0, random port).
/// </summary>
UInt32 EndPointConfig::PortTCPv4::get()
{
	return _portTCPv4;
}

/// <summary>
/// Gets or sets the TCP v4 transport port (default 0, random port).
/// </summary>
void EndPointConfig::PortTCPv4::set(UInt32 value)
{
	_portTCPv4 = value;
}

/// <summary>
/// Gets or sets the TCP v6 transport port (default 0, random port).
/// </summary>
UInt32 EndPointConfig::PortTCPv6::get()
{
	return _portTCPv6;
}

/// <summary>
/// Gets or sets the TCP v6 transport port (default 0, random port).
/// </summary>
void EndPointConfig::PortTCPv6::set(UInt32 value)
{
	_portTCPv6 = value;
}

/// <summary>
/// Gets or sets the TLS v4 transport port (default 0, random port).
/// </summary>
UInt32 EndPointConfig::PortTLSv4::get()
{
	return _portTLSv4;
}

/// <summary>
/// Gets or sets the TLS v4 transport port (default 0, random port).
/// </summary>
void EndPointConfig::PortTLSv4::set(UInt32 value)
{
	_portTLSv4 = value;
}

/// <summary>
/// Gets or sets the TLS v6 transport port (default 0, random port).
/// </summary>
UInt32 EndPointConfig::PortTLSv6::get()
{
	return _portTLSv6;
}

/// <summary>
/// Gets or sets the TLS v6 transport port (default 0, random port).
/// </summary>
void EndPointConfig::PortTLSv6::set(UInt32 value)
{
	_portTLSv6 = value;
}














/// <summary>
/// Gets or sets the UDP v4 transport port range (default 0, random port).
/// </summary>
UInt32 EndPointConfig::PortRangeUDPv4::get()
{
	return _portRangeUDPv4;
}

/// <summary>
/// Gets or sets the UDP v4 transport port range (default 0, random port).
/// </summary>
void EndPointConfig::PortRangeUDPv4::set(UInt32 value)
{
	_portRangeUDPv4 = value;
}

/// <summary>
/// Gets or sets the UDP v6 transport port range (default 0, random port).
/// </summary>
UInt32 EndPointConfig::PortRangeUDPv6::get()
{
	return _portRangeUDPv6;
}

/// <summary>
/// Gets or sets the UDP v6 transport port range (default 0, random port).
/// </summary>
void EndPointConfig::PortRangeUDPv6::set(UInt32 value)
{
	_portRangeUDPv6 = value;
}

/// <summary>
/// Gets or sets the TCP v4 transport port range (default 0, random port).
/// </summary>
UInt32 EndPointConfig::PortRangeTCPv4::get()
{
	return _portRangeTCPv4;
}

/// <summary>
/// Gets or sets the TCP v4 transport port range (default 0, random port).
/// </summary>
void EndPointConfig::PortRangeTCPv4::set(UInt32 value)
{
	_portRangeTCPv4 = value;
}

/// <summary>
/// Gets or sets the TCP v6 transport port range (default 0, random port).
/// </summary>
UInt32 EndPointConfig::PortRangeTCPv6::get()
{
	return _portRangeTCPv6;
}

/// <summary>
/// Gets or sets the TCP v6 transport port range (default 0, random port).
/// </summary>
void EndPointConfig::PortRangeTCPv6::set(UInt32 value)
{
	_portRangeTCPv6 = value;
}

/// <summary>
/// Gets or sets the TLS v4 transport port range (default 0, random port).
/// </summary>
UInt32 EndPointConfig::PortRangeTLSv4::get()
{
	return _portRangeTLSv4;
}

/// <summary>
/// Gets or sets the TLS v4 transport port range (default 0, random port).
/// </summary>
void EndPointConfig::PortRangeTLSv4::set(UInt32 value)
{
	_portRangeTLSv4 = value;
}

/// <summary>
/// Gets or sets the TLS v6 transport port range (default 0, random port).
/// </summary>
UInt32 EndPointConfig::PortRangeTLSv6::get()
{
	return _portRangeTLSv6;
}

/// <summary>
/// Gets or sets the TLS v6 transport port range (default 0, random port).
/// </summary>
void EndPointConfig::PortRangeTLSv6::set(UInt32 value)
{
	_portRangeTLSv6 = value;
}