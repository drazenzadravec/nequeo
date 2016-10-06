/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          DirectoryServiceCloudAccount.h
*  Purpose :       Host and Manage Active Directory Cloud account provider class.
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

#include "stdafx.h"

#include "Global.h"
#include "AwsAccount.h"

#undef IN
#undef CreateDirectory

#include <aws\ds\DirectoryServiceClient.h>
#include <aws\ds\DirectoryServiceEndpoint.h>

namespace Nequeo {
	namespace AWS {
		namespace Management
		{
			///	<summary>
			///	Cloud account provider.
			///	</summary>
			class EXPORT_NEQUEO_AWS_MANAGEMENT_API DirectoryServiceCloudAccount
			{
			public:
				///	<summary>
				///	Cloud account provider.
				///	</summary>
				/// <param name="account">The AWS services account.</param>
				DirectoryServiceCloudAccount(const AwsAccount& account);

				///	<summary>
				///	Cloud account provider destructor.
				///	</summary>
				~DirectoryServiceCloudAccount();

				/// <summary>
				/// Gets the Host and Manage Active Directory client.
				/// </summary>
				/// <return>The Host and Manage Active Directory client.</return>
				const Aws::DirectoryService::DirectoryServiceClient& GetClient() const;

				///	<summary>
				///	Get the service URI.
				///	</summary>
				///	<return>The service URI.</return>
				std::string GetServiceUri();

			private:
				bool _disposed;
				AwsAccount _account;
				Aws::UniquePtr<Aws::DirectoryService::DirectoryServiceClient> _client;
			};
		}
	}
}