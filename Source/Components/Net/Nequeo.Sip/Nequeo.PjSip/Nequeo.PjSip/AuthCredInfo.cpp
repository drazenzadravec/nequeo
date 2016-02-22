/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          AuthCredInfo.cpp
*  Purpose :       SIP AuthCredInfo class.
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

#include "AuthCredInfo.h"

using namespace Nequeo::Net::PjSip;

/// <summary>
/// Credential information. Credential contains information to authenticate against a service.
/// </summary>
AuthCredInfo::AuthCredInfo()
{
}

/// <summary>
/// Credential information. Credential contains information to authenticate against a service.
/// </summary>
/// <param name="username">The sip username.</param>
/// <param name="password">The sip password.</param>
AuthCredInfo::AuthCredInfo(String^ username, String^ password) :
	_scheme("plain"), _realm("*"), _dataType(0)
{
	_username = username;
	_data = password;
}

/// <summary>
/// Credential information. Credential contains information to authenticate against a service.
/// </summary>
/// <param name="username">The sip username.</param>
/// <param name="password">The sip password.</param>
/// <param name="scheme">The authentication scheme (e.g. "digest").</param>
/// <param name="realm">Realm on which this credential is to be used. Use "*" to make a credential that can be used to authenticate against any challenges.</param>
/// <param name="dataType">Type of data that is contained in the "data" field. Use 0 if the data contains plain text password.</param>
AuthCredInfo::AuthCredInfo(String^ username, String^ password, String^ scheme, String^ realm, int dataType)
{
	_username = username;
	_data = password;
	_scheme = scheme;
	_realm = realm;
	_dataType = dataType;
}

/// <summary>
/// Gets the the data, which can be a plain text password or a hashed digest.
/// </summary>
String^ AuthCredInfo::Data::get()
{
	return _data;
}

/// <summary>
/// Sets the the data, which can be a plain text password or a hashed digest.
/// </summary>
void AuthCredInfo::Data::set(String^ value)
{
	_data = value;
}

/// <summary>
/// Gets the Realm on which this credential is to be used. Use "*" to make
/// a credential that can be used to authenticate against any challenges.
/// </summary>
String^ AuthCredInfo::Realm::get()
{
	return _realm;
}

/// <summary>
/// Sets the Realm on which this credential is to be used. Use "*" to make
/// a credential that can be used to authenticate against any challenges.
/// </summary>
void AuthCredInfo::Realm::set(String^ value)
{
	_realm = value;
}

/// <summary>
/// Gets the authentication scheme (e.g. "digest").
/// </summary>
String^ AuthCredInfo::Scheme::get()
{
	return _scheme;
}

/// <summary>
/// Sets the authentication scheme (e.g. "digest").
/// </summary>
void AuthCredInfo::Scheme::set(String^ value)
{
	_scheme = value;
}

/// <summary>
/// Gets the authentication user name.
/// </summary>
String^ AuthCredInfo::Username::get()
{
	return _username;
}

/// <summary>
/// Sets the authentication user name.
/// </summary>
void AuthCredInfo::Username::set(String^ value)
{
	_username = value;
}

/// <summary>
/// Gets the type of data that is contained in the "data" field. Use 0 if the data
/// contains plain text password.
/// </summary>
int AuthCredInfo::DataType::get()
{
	return _dataType;
}

/// <summary>
/// Sets the type of data that is contained in the "data" field. Use 0 if the data
/// contains plain text password.
/// </summary>
void AuthCredInfo::DataType::set(int value)
{
	_dataType = value;
}