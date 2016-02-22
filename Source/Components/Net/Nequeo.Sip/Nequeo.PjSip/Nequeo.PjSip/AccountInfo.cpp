/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AccountInfo.cpp
*  Purpose :       SIP AccountInfo class.
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

#include "AccountInfo.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Account information.
/// </summary>
AccountInfo::AccountInfo()
{
}

/// <summary>
/// Gets the the data, which can be a plain text password or a hashed digest.
/// </summary>
int AccountInfo::Id::get()
{
	return _id;
}

/// <summary>
/// Sets the the data, which can be a plain text password or a hashed digest.
/// </summary>
void AccountInfo::Id::set(int value)
{
	_id = value;
}

/// <summary>
/// Gets or sets a flag to indicate whether this is the default account.
/// </summary>
bool AccountInfo::IsDefault::get()
{
	return _isDefault;
}

/// <summary>
/// Gets or sets a flag to indicate whether this is the default account.
/// </summary>
void AccountInfo::IsDefault::set(bool value)
{
	_isDefault = value;
}

/// <summary>
/// Gets or sets the presence online status for this account.
/// </summary>
bool AccountInfo::OnlineStatus::get()
{
	return _onlineStatus;
}

/// <summary>
/// Gets or sets the presence online status for this account.
/// </summary>
void AccountInfo::OnlineStatus::set(bool value)
{
	_onlineStatus = value;
}

/// <summary>
/// Gets or sets the presence online status text.
/// </summary>
String^ AccountInfo::OnlineStatusText::get()
{
	return _onlineStatusText;
}

/// <summary>
/// Gets or sets the presence online status text.
/// </summary>
void AccountInfo::OnlineStatusText::set(String^ value)
{
	_onlineStatusText = value;
}

/// <summary>
/// Gets or sets an up to date expiration interval for account registration session.
/// </summary>
int AccountInfo::RegExpiresSec::get()
{
	return _regExpiresSec;
}

/// <summary>
/// Gets or sets an up to date expiration interval for account registration session.
/// </summary>
void AccountInfo::RegExpiresSec::set(int value)
{
	_regExpiresSec = value;
}

/// <summary>
/// Gets or sets a flag to tell whether this account is currently registered (has active registration session).
/// </summary>
bool AccountInfo::RegIsActive::get()
{
	return _regIsActive;
}

/// <summary>
/// Gets or sets a flag to tell whether this account is currently registered (has active registration session).
/// </summary>
void AccountInfo::RegIsActive::set(bool value)
{
	_regIsActive = value;
}

/// <summary>
/// Gets or sets a flag to tell whether this account has registration setting (reg_uri is not empty).
/// </summary>
bool AccountInfo::RegIsConfigured::get()
{
	return _regIsConfigured;
}

/// <summary>
/// Gets or sets a flag to tell whether this account has registration setting (reg_uri is not empty).
/// </summary>
void AccountInfo::RegIsConfigured::set(bool value)
{
	_regIsConfigured = value;
}

/// <summary>
/// Gets or sets the Last registration error code. When the status field contains a SIP
/// status code that indicates a registration failure, last registration
/// error code contains the error code that causes the failure.In any
/// other case, its value is zero.
/// </summary>
int AccountInfo::RegLastErr::get()
{
	return _regLastErr;
}

/// <summary>
/// Gets or sets the Last registration error code. When the status field contains a SIP
/// status code that indicates a registration failure, last registration
/// error code contains the error code that causes the failure.In any
/// other case, its value is zero.
/// </summary>
void AccountInfo::RegLastErr::set(int value)
{
	_regLastErr = value;
}

/// <summary>
/// Gets or sets the status code.
/// </summary>
StatusCode AccountInfo::RegStatus::get()
{
	return _regStatus;
}

/// <summary>
/// Gets or sets the status code.
/// </summary>
void AccountInfo::RegStatus::set(StatusCode value)
{
	_regStatus = value;
}

/// <summary>
/// Gets or sets a describing the registration status.
/// </summary>
String^ AccountInfo::RegStatusText::get()
{
	return _regStatusText;
}

/// <summary>
/// Gets or sets a describing the registration status.
/// </summary>
void AccountInfo::RegStatusText::set(String^ value)
{
	_regStatusText = value;
}

/// <summary>
/// Gets or sets the account URI.
/// </summary>
String^ AccountInfo::Uri::get()
{
	return _uri;
}

/// <summary>
/// Gets or sets the account URI.
/// </summary>
void AccountInfo::Uri::set(String^ value)
{
	_uri = value;
}