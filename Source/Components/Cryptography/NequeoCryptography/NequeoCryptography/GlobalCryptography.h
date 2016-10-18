/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          GlobalCryptography.h
*  Purpose :       GlobalCryptography class.
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

#ifndef _GLOBALCRYPTOGRAPHY_H
#define _GLOBALCRYPTOGRAPHY_H

#include "stdafx.h"
#include <string>
#include <ctime>
#include <random>

using namespace std;

namespace Nequeo {
	namespace Cryptography
	{
		/// <summary>
		/// Open ssl certificate subject container
		/// </summary>
		typedef struct 
		{
			/// <summary>
			/// The country name.
			/// </summary>
			std::string CountryName;
			/// <summary>
			/// The state.
			/// </summary>
			std::string State;
			/// <summary>
			/// The location name.
			/// </summary>
			std::string LocationName;
			/// <summary>
			/// The organisation name.
			/// </summary>
			std::string OrganisationName;
			/// <summary>
			/// The organisation unit name.
			/// </summary>
			std::string OrganisationUnitName;
			/// <summary>
			/// The comman name.
			/// </summary>
			std::string CommonName;
			/// <summary>
			/// The email address.
			/// </summary>
			std::string EmailAddress;

			/// <summary>
			/// Creates the subject list from the subject arguments.
			/// </summary>
			/// <returns>The well formed subject</returns>
			std::string SubjectArguments()
			{
				return
					"/C=" + CountryName +
					"/ST=" + State +
					"/L=" + LocationName +
					"/O=" + OrganisationName +
					"/OU=" + OrganisationUnitName +
					"/CN=" + CommonName +
					(EmailAddress.size() > 0 ? "/emailAddress=" + EmailAddress : "");
			}

		} Subject;

		/// <summary>
		/// Open ssl certificate issuer container.
		/// </summary>
		typedef struct
		{
			/// <summary>
			/// Private key data.
			/// </summary>
			const unsigned char* PrivateKey;
			/// <summary>
			/// Private key data length.
			/// </summary>
			int PrivateKeyLength;
			/// <summary>
			/// Public key data.
			/// </summary>
			const unsigned char* PublicKey;
			/// <summary>
			/// Public key data length.
			/// </summary>
			int PublicKeyLength;

		} RSACertificateIssuer;
	}
}

#endif