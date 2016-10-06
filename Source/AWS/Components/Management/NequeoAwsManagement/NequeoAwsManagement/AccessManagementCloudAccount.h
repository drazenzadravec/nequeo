/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          AccessManagementCloudAccount.h
*  Purpose :       Manage User Access and Encryption Keys Cloud account provider class.
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
#undef UpdateResource

#include <aws\access-management\AccessManagementClient.h>

namespace Nequeo {
	namespace AWS {
		namespace Management
		{
			///	<summary>
			///	Cloud account provider.
			///	</summary>
			class EXPORT_NEQUEO_AWS_MANAGEMENT_API AccessManagementCloudAccount
			{
			public:
				///	<summary>
				///	Cloud account provider.
				///	</summary>
				/// <param name="account">The AWS services account.</param>
				/// <param name="iamClient">The AWS IAM client.</param>
				/// <param name="cognitoClient">The AWS Cognito Identity Client.</param>
				AccessManagementCloudAccount(const AwsAccount& account, 
					std::shared_ptr<Aws::IAM::IAMClient>& iamClient, 
					std::shared_ptr<Aws::CognitoIdentity::CognitoIdentityClient>& cognitoClient);

				///	<summary>
				///	Cloud account provider destructor.
				///	</summary>
				~AccessManagementCloudAccount();

				/// <summary>
				/// Gets the Manage User Access and Encryption Keys client.
				/// </summary>
				/// <return>The Manage User Access and Encryption Keys client.</return>
				const Aws::AccessManagement::AccessManagementClient& GetClient() const;

			private:
				bool _disposed;
				AwsAccount _account;
				Aws::UniquePtr<Aws::AccessManagement::AccessManagementClient> _client;
			};
		}
	}
}