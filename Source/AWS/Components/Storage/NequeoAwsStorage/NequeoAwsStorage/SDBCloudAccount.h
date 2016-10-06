/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          SDBCloudAccount.h
*  Purpose :       Simple database Cloud account provider class.
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
#undef GetObject

#include <aws\sdb\SimpleDBClient.h>
#include <aws\sdb\SimpleDBEndpoint.h>
#include <aws\sdb\model\CreateDomainRequest.h>
#include <aws\sdb\model\DeleteDomainRequest.h>
#include <aws\sdb\model\ListDomainsRequest.h>
#include <aws\sdb\model\ListDomainsResult.h>
#include <aws\sdb\model\PutAttributesRequest.h>
#include <aws\sdb\model\GetAttributesRequest.h>
#include <aws\sdb\model\DeleteAttributesRequest.h>
#include <aws\sdb\model\SelectRequest.h>
#include <aws\sdb\model\SelectResult.h>
#include <aws\sdb\model\Item.h>

namespace Nequeo {
	namespace AWS {
		namespace Storage
		{
			///	<summary>
			///	Cloud account provider.
			///	</summary>
			class EXPORT_NEQUEO_AWS_STORAGE_API SDBCloudAccount
			{
			public:
				///	<summary>
				///	Cloud account provider.
				///	</summary>
				/// <param name="account">The AWS services account.</param>
				SDBCloudAccount(const AwsAccount& account);

				///	<summary>
				///	Cloud account provider destructor.
				///	</summary>
				~SDBCloudAccount();

				/// <summary>
				/// Gets the Simple database client.
				/// </summary>
				/// <return>The Simple database client.</return>
				const Aws::SimpleDB::SimpleDBClient& GetClient() const;

				///	<summary>
				///	Get the service URI.
				///	</summary>
				///	<return>The service URI.</return>
				std::string GetServiceUri();

				///	<summary>
				///	Create the domain asynchronously.
				///	</summary>
				/// <param name="request">The domain request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void CreateDomainAsync(
					const Aws::SimpleDB::Model::CreateDomainRequest& request,
					const Aws::SimpleDB::CreateDomainResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Create the domain asynchronously.
				///	</summary>
				/// <param name="domainName">The name of the domain to create. The name can range between 3 and 255 characters
				/// and can contain the following characters: a-z, A-Z, 0-9, '_', '-', and '.'.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void CreateDomainAsync(
					const std::string& domainName,
					const Aws::SimpleDB::CreateDomainResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Delete the domain asynchronously.
				///	</summary>
				/// <param name="request">The delete request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void DeleteDomainAsync(
					const Aws::SimpleDB::Model::DeleteDomainRequest& request,
					const Aws::SimpleDB::DeleteDomainResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Delete the domain asynchronously.
				///	</summary>
				/// <param name="domainName">The name of the domain to create. The name can range between 3 and 255 characters
				/// and can contain the following characters: a-z, A-Z, 0-9, '_', '-', and '.'.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void DeleteDomainAsync(
					const std::string& domainName,
					const Aws::SimpleDB::DeleteDomainResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	List the domains asynchronously.
				///	</summary>
				/// <param name="request">The list request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void ListDomainsAsync(
					const Aws::SimpleDB::Model::ListDomainsRequest& request,
					const Aws::SimpleDB::ListDomainsResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Get all domain items after request.
				///	</summary>
				/// <param name="client">The SimpleDB client.</param>
				/// <param name="results">The domain result.</param>
				/// <param name="take">The maximum number of results to return (set take = -1 to return all results found).</param>
				///	<return>The array of domain results.</return>
				const Aws::Vector<Aws::String> GetDomainItems(
					const Aws::SimpleDB::SimpleDBClient* client, 
					const Aws::SimpleDB::Model::ListDomainsResult& results,
					long long take = 100) const;

				///	<summary>
				///	Add data to domain (table) and row (item).
				///	</summary>
				/// <param name="request">The put request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void PutAttributesAsync(
					const Aws::SimpleDB::Model::PutAttributesRequest& request,
					const Aws::SimpleDB::PutAttributesResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Get data from domain (table) and row (item).
				///	</summary>
				/// <param name="request">The get request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void GetAttributesAsync(
					const Aws::SimpleDB::Model::GetAttributesRequest& request,
					const Aws::SimpleDB::GetAttributesResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Delete data from domain (table) and row (item).
				///	</summary>
				/// <param name="request">The delete request.</param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void DeleteAttributesAsync(
					const Aws::SimpleDB::Model::DeleteAttributesRequest& request,
					const Aws::SimpleDB::DeleteAttributesResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Select data from domain (table) and row (item).
				///	</summary>
				/// <param name="request">
				/// The select request; samples
				/// SELECT a, b, c FROM DomainName WHERE a > 6 ORDER BY b LIMIT 100
				/// </param>
				/// <param name="handler">The function callback handler.</param>
				/// <param name="context">The user defined context.</param>
				void SelectAsync(
					const Aws::SimpleDB::Model::SelectRequest& request,
					const Aws::SimpleDB::SelectResponseReceivedHandler& handler,
					const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context = nullptr) const;

				///	<summary>
				///	Get all select items after request.
				///	</summary>
				/// <param name="client">The SimpleDB client.</param>
				/// <param name="results">The select result.</param>
				/// <param name="take">The maximum number of results to return (set take = -1 to return all results found).</param>
				///	<return>The array of select results.</return>
				const Aws::Vector<Aws::SimpleDB::Model::Item> GetItems(
					const Aws::SimpleDB::SimpleDBClient* client,
					const Aws::SimpleDB::Model::SelectResult& results,
					long long take = 100) const;

			private:
				bool _disposed;
				AwsAccount _account;
				Aws::UniquePtr<Aws::SimpleDB::SimpleDBClient> _client;
			};
		}
	}
}