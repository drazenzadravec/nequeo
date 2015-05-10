/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ExceptionCodes.cpp
*  Purpose :       ExceptionCodes class.
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

#include "ExceptionCodes.h"

namespace Nequeo{
	namespace Exceptions
	{
		NEQUEO_IMPLEMENT_EXCEPTION(LogicException, Exception, "Logic exception")
		NEQUEO_IMPLEMENT_EXCEPTION(AssertionViolationException, LogicException, "Assertion violation")
		NEQUEO_IMPLEMENT_EXCEPTION(NullPointerException, LogicException, "Null pointer")
		NEQUEO_IMPLEMENT_EXCEPTION(NullValueException, LogicException, "Null value")
		NEQUEO_IMPLEMENT_EXCEPTION(BugcheckException, LogicException, "Bugcheck")
		NEQUEO_IMPLEMENT_EXCEPTION(InvalidArgumentException, LogicException, "Invalid argument")
		NEQUEO_IMPLEMENT_EXCEPTION(NotImplementedException, LogicException, "Not implemented")
		NEQUEO_IMPLEMENT_EXCEPTION(RangeException, LogicException, "Out of range")
		NEQUEO_IMPLEMENT_EXCEPTION(IllegalStateException, LogicException, "Illegal state")
		NEQUEO_IMPLEMENT_EXCEPTION(InvalidAccessException, LogicException, "Invalid access")
		NEQUEO_IMPLEMENT_EXCEPTION(SignalException, LogicException, "Signal received")
		NEQUEO_IMPLEMENT_EXCEPTION(UnhandledException, LogicException, "Unhandled exception")

		NEQUEO_IMPLEMENT_EXCEPTION(RuntimeException, Exception, "Runtime exception")
		NEQUEO_IMPLEMENT_EXCEPTION(NotFoundException, RuntimeException, "Not found")
		NEQUEO_IMPLEMENT_EXCEPTION(ExistsException, RuntimeException, "Exists")
		NEQUEO_IMPLEMENT_EXCEPTION(TimeoutException, RuntimeException, "Timeout")
		NEQUEO_IMPLEMENT_EXCEPTION(SystemException, RuntimeException, "System exception")
		NEQUEO_IMPLEMENT_EXCEPTION(RegularExpressionException, RuntimeException, "Error in regular expression")
		NEQUEO_IMPLEMENT_EXCEPTION(LibraryLoadException, RuntimeException, "Cannot load library")
		NEQUEO_IMPLEMENT_EXCEPTION(LibraryAlreadyLoadedException, RuntimeException, "Library already loaded")
		NEQUEO_IMPLEMENT_EXCEPTION(NoThreadAvailableException, RuntimeException, "No thread available")
		NEQUEO_IMPLEMENT_EXCEPTION(PropertyNotSupportedException, RuntimeException, "Property not supported")
		NEQUEO_IMPLEMENT_EXCEPTION(PoolOverflowException, RuntimeException, "Pool overflow")
		NEQUEO_IMPLEMENT_EXCEPTION(NoPermissionException, RuntimeException, "No permission")
		NEQUEO_IMPLEMENT_EXCEPTION(OutOfMemoryException, RuntimeException, "Out of memory")
		NEQUEO_IMPLEMENT_EXCEPTION(DataException, RuntimeException, "Data error")

		NEQUEO_IMPLEMENT_EXCEPTION(DataFormatException, DataException, "Bad data format")
		NEQUEO_IMPLEMENT_EXCEPTION(SyntaxException, DataException, "Syntax error")
		NEQUEO_IMPLEMENT_EXCEPTION(CircularReferenceException, DataException, "Circular reference")
		NEQUEO_IMPLEMENT_EXCEPTION(PathSyntaxException, SyntaxException, "Bad path syntax")
		NEQUEO_IMPLEMENT_EXCEPTION(IOException, RuntimeException, "I/O error")
		NEQUEO_IMPLEMENT_EXCEPTION(ProtocolException, IOException, "Protocol error")
		NEQUEO_IMPLEMENT_EXCEPTION(FileException, IOException, "File access error")
		NEQUEO_IMPLEMENT_EXCEPTION(FileExistsException, FileException, "File exists")
		NEQUEO_IMPLEMENT_EXCEPTION(FileNotFoundException, FileException, "File not found")
		NEQUEO_IMPLEMENT_EXCEPTION(PathNotFoundException, FileException, "Path not found")
		NEQUEO_IMPLEMENT_EXCEPTION(FileReadOnlyException, FileException, "File is read-only")
		NEQUEO_IMPLEMENT_EXCEPTION(FileAccessDeniedException, FileException, "Access to file denied")
		NEQUEO_IMPLEMENT_EXCEPTION(CreateFileException, FileException, "Cannot create file")
		NEQUEO_IMPLEMENT_EXCEPTION(OpenFileException, FileException, "Cannot open file")
		NEQUEO_IMPLEMENT_EXCEPTION(WriteFileException, FileException, "Cannot write file")
		NEQUEO_IMPLEMENT_EXCEPTION(ReadFileException, FileException, "Cannot read file")
		NEQUEO_IMPLEMENT_EXCEPTION(UnknownURISchemeException, RuntimeException, "Unknown URI scheme")

		NEQUEO_IMPLEMENT_EXCEPTION(ApplicationException, Exception, "Application exception")
		NEQUEO_IMPLEMENT_EXCEPTION(BadCastException, RuntimeException, "Bad cast exception")
	}
}

namespace Nequeo{
	namespace Exceptions{
		namespace Net
		{
			NEQUEO_IMPLEMENT_EXCEPTION(NetException, IOException, "Net Exception")
			NEQUEO_IMPLEMENT_EXCEPTION(InvalidAddressException, NetException, "Invalid address")
			NEQUEO_IMPLEMENT_EXCEPTION(InvalidSocketException, NetException, "Invalid socket")
			NEQUEO_IMPLEMENT_EXCEPTION(ServiceNotFoundException, NetException, "Service not found")
			NEQUEO_IMPLEMENT_EXCEPTION(ConnectionAbortedException, NetException, "Software caused connection abort")
			NEQUEO_IMPLEMENT_EXCEPTION(ConnectionResetException, NetException, "Connection reset by peer")
			NEQUEO_IMPLEMENT_EXCEPTION(ConnectionRefusedException, NetException, "Connection refused")
			NEQUEO_IMPLEMENT_EXCEPTION(DNSException, NetException, "DNS error")
			NEQUEO_IMPLEMENT_EXCEPTION(HostNotFoundException, DNSException, "Host not found")
			NEQUEO_IMPLEMENT_EXCEPTION(NoAddressFoundException, DNSException, "No address found")
			NEQUEO_IMPLEMENT_EXCEPTION(InterfaceNotFoundException, NetException, "Interface not found")
			NEQUEO_IMPLEMENT_EXCEPTION(NoMessageException, NetException, "No message received")
			NEQUEO_IMPLEMENT_EXCEPTION(MessageException, NetException, "Malformed message")
			NEQUEO_IMPLEMENT_EXCEPTION(MultipartException, MessageException, "Malformed multipart message")
			NEQUEO_IMPLEMENT_EXCEPTION(HTTPException, NetException, "HTTP Exception")
			NEQUEO_IMPLEMENT_EXCEPTION(NotAuthenticatedException, HTTPException, "No authentication information found")
			NEQUEO_IMPLEMENT_EXCEPTION(UnsupportedRedirectException, HTTPException, "Unsupported HTTP redirect (protocol change)")
			NEQUEO_IMPLEMENT_EXCEPTION(FTPException, NetException, "FTP Exception")
			NEQUEO_IMPLEMENT_EXCEPTION(SMTPException, NetException, "SMTP Exception")
			NEQUEO_IMPLEMENT_EXCEPTION(POP3Exception, NetException, "POP3 Exception")
			NEQUEO_IMPLEMENT_EXCEPTION(ICMPException, NetException, "ICMP Exception")
			NEQUEO_IMPLEMENT_EXCEPTION(HTMLFormException, NetException, "HTML Form Exception")
			NEQUEO_IMPLEMENT_EXCEPTION(WebSocketException, NetException, "WebSocket Exception")
		}
	}
}