/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          AwsAccount.h
*  Purpose :       AWS account class.
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

namespace Nequeo {
	namespace Aws {
		namespace Storage 
		{
			///	<summary>
			///	AWS account provider.
			///	</summary>
			class EXPORT_NEQUEO_AWS_STORAGE_API AwsAccount
			{
			public:
				///	<summary>
				///	AWS account provider.
				///	</summary>
				/// <param name="credentials">The credentials object for AWS services.</param>
				/// <param name="clientConfiguration">Configuration for accessing Amazon web services.</param>
				AwsAccount(const AWS::Auth::AWSCredentials& credentials, const AWS::Client::ClientConfiguration& clientConfiguration = AWS::Client::ClientConfiguration());

				///	<summary>
				///	AWS account provider destructor.
				///	</summary>
				~AwsAccount();

				///	<summary>
				///	Initialize the AWS SDK, this must be call after setting options.
				///	</summary>
				void Initialize();

				///	<summary>
				///	Get the region.
				///	</summary>
				///	<return>The region.</return>
				std::string GetRegion();
				void SetRegion(const std::string& region = std::string(AWS::Region::AP_SOUTHEAST_2));

				///	<summary>
				///	Set the defualt client configuration executor.
				///	</summary>
				void SetDefaultExecutor();

				///	<summary>
				///	Set the pooled thread client configuration executor.
				///	</summary>
				/// <param name="poolSize">The thread pool size.</param>
				void SetPooledExecutor(size_t poolSize);

				///	<summary>
				///	Get the AWS SDK options (change what is needed).
				///	</summary>
				///	<return>The AWS SDK options.</return>
				inline AWS::SDKOptions& GetSDKOptions()
				{
					return _options;
				}

				///	<summary>
				///	Is the AWS SDK initialized.
				///	</summary>
				///	<return>True if initialized; else false.</return>
				inline bool IsInitialized() const
				{
					return _initialized;
				}

			private:
				bool _disposed;
				bool _initialized;

				AWS::SDKOptions _options;
				AWS::Auth::AWSCredentials _credentials;
				AWS::Client::ClientConfiguration _clientConfiguration;

				std::shared_ptr<AWS::Utils::Threading::Executor> _executor;

				// Allow internal access to private members.
				friend class DynamoDBCloudAccount;
				
			};
		}
	}
}