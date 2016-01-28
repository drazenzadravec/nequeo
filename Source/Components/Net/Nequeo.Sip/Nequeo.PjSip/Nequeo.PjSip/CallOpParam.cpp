/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallOpParam.cpp
*  Purpose :       SIP CallOpParam class.
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

#include "CallOpParam.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Call options.
///	</summary>
CallOpParam::CallOpParam() :
	_disposed(false), _useDefaultCallSetting(false)
{
}

///	<summary>
///	Call options.
///	</summary>
/// <param name="useDefaultCallSetting">Use default call settings. Setting useDefaultCallSetting to true will initialize opt with default
/// call setting values.</param>
CallOpParam::CallOpParam(bool useDefaultCallSetting) :
	_disposed(false)
{
	_useDefaultCallSetting = useDefaultCallSetting;
}

///	<summary>
///	Call options.
///	</summary>
CallOpParam::~CallOpParam()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

///	<summary>
///	Gets or sets the call settings.
///	</summary>
CallSetting^ CallOpParam::Setting::get()
{
	return _setting;
}

///	<summary>
///	Gets or sets the call settings.
///	</summary>
void CallOpParam::Setting::set(CallSetting^ value)
{
	_setting = value;
}

///	<summary>
///	Gets or sets the options.
///	</summary>
unsigned CallOpParam::Options::get()
{
	return _options;
}

///	<summary>
///	Gets or sets the options.
///	</summary>
void CallOpParam::Options::set(unsigned value)
{
	_options = value;
}

///	<summary>
///	Gets or sets the reason phrase.
///	</summary>
String^ CallOpParam::Reason::get()
{
	return _reason;
}

///	<summary>
///	Gets or sets the reason phrase.
///	</summary>
void CallOpParam::Reason::set(String^ value)
{
	_reason = value;
}

///	<summary>
///	Gets or sets the status code.
///	</summary>
StatusCode CallOpParam::Code::get()
{
	return _code;
}

///	<summary>
///	Gets or sets the status code.
///	</summary>
void CallOpParam::Code::set(StatusCode value)
{
	_code = value;
}

///	<summary>
///	Gets or sets the list of headers etc to be added to outgoing response message.
/// Note that this message data will be persistent in all next
/// answers / responses for this INVITE request.
///	</summary>
SipTxOption^ CallOpParam::TxOption::get()
{
	return _txOption;
}

///	<summary>
///	Gets or sets the list of headers etc to be added to outgoing response message.
/// Note that this message data will be persistent in all next
/// answers / responses for this INVITE request.
///	</summary>
void CallOpParam::TxOption::set(SipTxOption^ value)
{
	_txOption = value;
}

///	<summary>
///	Gets the use default values.
///	</summary>
bool CallOpParam::UseDefaultCallSetting::get()
{
	return _useDefaultCallSetting;
}