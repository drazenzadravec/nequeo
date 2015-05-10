/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          ExceptionCodes.h
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

#pragma once

#ifndef _EXCEPTIONCODES_H
#define _EXCEPTIONCODES_H

#include "GlobalException.h"

#include "Exception.h"

namespace Nequeo{
	namespace Exceptions
	{
		// Macros for quickly declaring and implementing exception classes.
		// Unfortunately, we cannot use a template here because character
		// pointers (which we need for specifying the exception name)
		// are not allowed as template arguments.
#define NEQUEO_DECLARE_EXCEPTION_CODE(CLS, BASE, CODE) \
	class CLS: public BASE														\
		{																				\
	public:																			\
		CLS(int code = CODE);														\
		CLS(const std::string& msg, int code = CODE);								\
		CLS(const std::string& msg, const std::string& arg, int code = CODE);		\
		CLS(const std::string& msg, const Nequeo::Exception& exc, int code = CODE);	\
		CLS(const CLS& exc);														\
		~CLS() throw();																\
		CLS& operator = (const CLS& exc);											\
		const char* name() const throw();											\
		const char* className() const throw();										\
		Nequeo::Exception* clone() const;												\
		void rethrow() const;														\
		};

#define NEQUEO_DECLARE_EXCEPTION(CLS, BASE) \
	NEQUEO_DECLARE_EXCEPTION_CODE(CLS, BASE, 0)

#define NEQUEO_IMPLEMENT_EXCEPTION(CLS, BASE, NAME)													\
	CLS::CLS(int code): BASE(code)																	\
		{																								\
		}																								\
	CLS::CLS(const std::string& msg, int code): BASE(msg, code)										\
		{																								\
		}																								\
	CLS::CLS(const std::string& msg, const std::string& arg, int code): BASE(msg, arg, code)		\
		{																								\
		}																								\
	CLS::CLS(const std::string& msg, const Nequeo::Exception& exc, int code): BASE(msg, exc, code)	\
		{																								\
		}																								\
	CLS::CLS(const CLS& exc): BASE(exc)																\
		{																								\
		}																								\
	CLS::~CLS() throw()																				\
		{																								\
		}																								\
	CLS& CLS::operator = (const CLS& exc)															\
		{																								\
		BASE::operator = (exc);																		\
		return *this;																				\
		}																								\
	const char* CLS::name() const throw()															\
		{																								\
		return NAME;																				\
		}																								\
	const char* CLS::className() const throw()														\
		{																								\
		return typeid(*this).name();																\
		}																								\
	Nequeo::Exception* CLS::clone() const																\
		{																								\
		return new CLS(*this);																		\
		}																								\
	void CLS::rethrow() const																		\
		{																								\
		throw *this;																				\
		}

		// Standard exception classes
		NEQUEO_DECLARE_EXCEPTION(LogicException, Exception)
		NEQUEO_DECLARE_EXCEPTION(AssertionViolationException, LogicException)
		NEQUEO_DECLARE_EXCEPTION(NullPointerException, LogicException)
		NEQUEO_DECLARE_EXCEPTION(NullValueException, LogicException)
		NEQUEO_DECLARE_EXCEPTION(BugcheckException, LogicException)
		NEQUEO_DECLARE_EXCEPTION(InvalidArgumentException, LogicException)
		NEQUEO_DECLARE_EXCEPTION(NotImplementedException, LogicException)
		NEQUEO_DECLARE_EXCEPTION(RangeException, LogicException)
		NEQUEO_DECLARE_EXCEPTION(IllegalStateException, LogicException)
		NEQUEO_DECLARE_EXCEPTION(InvalidAccessException, LogicException)
		NEQUEO_DECLARE_EXCEPTION(SignalException, LogicException)
		NEQUEO_DECLARE_EXCEPTION(UnhandledException, LogicException)

		NEQUEO_DECLARE_EXCEPTION(RuntimeException, Exception)
		NEQUEO_DECLARE_EXCEPTION(NotFoundException, RuntimeException)
		NEQUEO_DECLARE_EXCEPTION(ExistsException, RuntimeException)
		NEQUEO_DECLARE_EXCEPTION(TimeoutException, RuntimeException)
		NEQUEO_DECLARE_EXCEPTION(SystemException, RuntimeException)
		NEQUEO_DECLARE_EXCEPTION(RegularExpressionException, RuntimeException)
		NEQUEO_DECLARE_EXCEPTION(LibraryLoadException, RuntimeException)
		NEQUEO_DECLARE_EXCEPTION(LibraryAlreadyLoadedException, RuntimeException)
		NEQUEO_DECLARE_EXCEPTION(NoThreadAvailableException, RuntimeException)
		NEQUEO_DECLARE_EXCEPTION(PropertyNotSupportedException, RuntimeException)
		NEQUEO_DECLARE_EXCEPTION(PoolOverflowException, RuntimeException)
		NEQUEO_DECLARE_EXCEPTION(NoPermissionException, RuntimeException)
		NEQUEO_DECLARE_EXCEPTION(OutOfMemoryException, RuntimeException)
		NEQUEO_DECLARE_EXCEPTION(DataException, RuntimeException)

		NEQUEO_DECLARE_EXCEPTION(DataFormatException, DataException)
		NEQUEO_DECLARE_EXCEPTION(SyntaxException, DataException)
		NEQUEO_DECLARE_EXCEPTION(CircularReferenceException, DataException)
		NEQUEO_DECLARE_EXCEPTION(PathSyntaxException, SyntaxException)
		NEQUEO_DECLARE_EXCEPTION(IOException, RuntimeException)
		NEQUEO_DECLARE_EXCEPTION(ProtocolException, IOException)
		NEQUEO_DECLARE_EXCEPTION(FileException, IOException)
		NEQUEO_DECLARE_EXCEPTION(FileExistsException, FileException)
		NEQUEO_DECLARE_EXCEPTION(FileNotFoundException, FileException)
		NEQUEO_DECLARE_EXCEPTION(PathNotFoundException, FileException)
		NEQUEO_DECLARE_EXCEPTION(FileReadOnlyException, FileException)
		NEQUEO_DECLARE_EXCEPTION(FileAccessDeniedException, FileException)
		NEQUEO_DECLARE_EXCEPTION(CreateFileException, FileException)
		NEQUEO_DECLARE_EXCEPTION(OpenFileException, FileException)
		NEQUEO_DECLARE_EXCEPTION(WriteFileException, FileException)
		NEQUEO_DECLARE_EXCEPTION(ReadFileException, FileException)
		NEQUEO_DECLARE_EXCEPTION(UnknownURISchemeException, RuntimeException)

		NEQUEO_DECLARE_EXCEPTION(ApplicationException, Exception)
		NEQUEO_DECLARE_EXCEPTION(BadCastException, RuntimeException)
	}
}

namespace Nequeo{
	namespace Exceptions{
		namespace Net
		{
			NEQUEO_DECLARE_EXCEPTION(NetException, IOException)
			NEQUEO_DECLARE_EXCEPTION(InvalidAddressException, NetException)
			NEQUEO_DECLARE_EXCEPTION(InvalidSocketException, NetException)
			NEQUEO_DECLARE_EXCEPTION(ServiceNotFoundException, NetException)
			NEQUEO_DECLARE_EXCEPTION(ConnectionAbortedException, NetException)
			NEQUEO_DECLARE_EXCEPTION(ConnectionResetException, NetException)
			NEQUEO_DECLARE_EXCEPTION(ConnectionRefusedException, NetException)
			NEQUEO_DECLARE_EXCEPTION(DNSException, NetException)
			NEQUEO_DECLARE_EXCEPTION(HostNotFoundException, DNSException)
			NEQUEO_DECLARE_EXCEPTION(NoAddressFoundException, DNSException)
			NEQUEO_DECLARE_EXCEPTION(InterfaceNotFoundException, NetException)
			NEQUEO_DECLARE_EXCEPTION(NoMessageException, NetException)
			NEQUEO_DECLARE_EXCEPTION(MessageException, NetException)
			NEQUEO_DECLARE_EXCEPTION(MultipartException, MessageException)
			NEQUEO_DECLARE_EXCEPTION(HTTPException, NetException)
			NEQUEO_DECLARE_EXCEPTION(NotAuthenticatedException, HTTPException)
			NEQUEO_DECLARE_EXCEPTION(UnsupportedRedirectException, HTTPException)
			NEQUEO_DECLARE_EXCEPTION(FTPException, NetException)
			NEQUEO_DECLARE_EXCEPTION(SMTPException, NetException)
			NEQUEO_DECLARE_EXCEPTION(POP3Exception, NetException)
			NEQUEO_DECLARE_EXCEPTION(ICMPException, NetException)
			NEQUEO_DECLARE_EXCEPTION(HTMLFormException, NetException)
			NEQUEO_DECLARE_EXCEPTION(WebSocketException, NetException)
		}
	}
}
#endif