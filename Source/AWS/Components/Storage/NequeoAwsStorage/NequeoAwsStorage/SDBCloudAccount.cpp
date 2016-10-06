/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          SDBCloudAccount.cpp
*  Purpose :       Simple database account provider class.
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

#include "SDBCloudAccount.h"

using namespace Nequeo::AWS::Storage;

static const char* SIMPLEDB_CLIENT_TAG = "NequeoSimpleDBClient";

///	<summary>
///	Cloud account provider.
///	</summary>
/// <param name="account">The AWS services account.</param>
SDBCloudAccount::SDBCloudAccount(const AwsAccount& account) : _disposed(false), _account(account)
{
	// Create the client.
	_client = Aws::MakeUnique<Aws::SimpleDB::SimpleDBClient>(SIMPLEDB_CLIENT_TAG, _account._credentials, _account._clientConfiguration);
}

///	<summary>
///	Cloud account provider.
///	</summary>
SDBCloudAccount::~SDBCloudAccount()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Gets the Simple database client.
/// </summary>
/// <return>The Simple database client.</return>
const Aws::SimpleDB::SimpleDBClient& SDBCloudAccount::GetClient() const
{
	return *(_client.get());
}

///	<summary>
///	Get the service URI.
///	</summary>
///	<return>The service URI.</return>
std::string SDBCloudAccount::GetServiceUri()
{
	return std::string(Aws::SimpleDB::SimpleDBEndpoint::ForRegion(_account._clientConfiguration.region).c_str());
}

///	<summary>
///	Create the domain asynchronously.
///	</summary>
/// <param name="request">The domain request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SDBCloudAccount::CreateDomainAsync(
	const Aws::SimpleDB::Model::CreateDomainRequest& request,
	const Aws::SimpleDB::CreateDomainResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->CreateDomainAsync(request, handler, context);
}

///	<summary>
///	Create the domain asynchronously.
///	</summary>
/// <param name="domainName">The name of the domain to create. The name can range between 3 and 255 characters
/// and can contain the following characters: a-z, A-Z, 0-9, '_', '-', and '.'.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SDBCloudAccount::CreateDomainAsync(
	const std::string& domainName,
	const Aws::SimpleDB::CreateDomainResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::SimpleDB::Model::CreateDomainRequest request;
	request.SetDomainName(Aws::String(domainName.c_str()));
	_client->CreateDomainAsync(request, handler, context);
}

///	<summary>
///	Delete the domain asynchronously.
///	</summary>
/// <param name="request">The domain request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SDBCloudAccount::DeleteDomainAsync(
	const Aws::SimpleDB::Model::DeleteDomainRequest& request,
	const Aws::SimpleDB::DeleteDomainResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->DeleteDomainAsync(request, handler, context);
}

///	<summary>
///	Delete the domain asynchronously.
///	</summary>
/// <param name="domainName">The name of the domain to create. The name can range between 3 and 255 characters
/// and can contain the following characters: a-z, A-Z, 0-9, '_', '-', and '.'.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SDBCloudAccount::DeleteDomainAsync(
	const std::string& domainName,
	const Aws::SimpleDB::DeleteDomainResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	Aws::SimpleDB::Model::DeleteDomainRequest request;
	request.SetDomainName(Aws::String(domainName.c_str()));
	_client->DeleteDomainAsync(request, handler, context);
}

///	<summary>
///	List the domains asynchronously.
///	</summary>
/// <param name="request">The domain request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SDBCloudAccount::ListDomainsAsync(
	const Aws::SimpleDB::Model::ListDomainsRequest& request,
	const Aws::SimpleDB::ListDomainsResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->ListDomainsAsync(request, handler, context);
}

///	<summary>
///	Get all domain items after request.
///	</summary>
/// <param name="client">The SimpleDB client.</param>
/// <param name="results">The domain result.</param>
/// <param name="take">The maximum number of results to return (set take = -1 to return all results found).</param>
///	<return>The array of domain results.</return>
const Aws::Vector<Aws::String> SDBCloudAccount::GetDomainItems(
	const Aws::SimpleDB::SimpleDBClient* client,
	const Aws::SimpleDB::Model::ListDomainsResult& results,
	long long take) const
{
	long long count = 0;
	bool foundEnought = false;
	bool getAll = (take < 0 ? true : false);
	Aws::Vector<Aws::String> domains;

	// Get the first set of items.
	Aws::Vector<Aws::String> items = results.GetDomainNames();

	// Get the vector size.
	size_t vectorSize = items.size();

	// If items exist.
	if (vectorSize > 0)
	{
		// For each item found.
		for (int i = 0; i < vectorSize; i++)
		{
			// Add the item.
			auto item = items[i];
			domains.push_back(item);

			// If not getting all.
			if (!getAll)
			{
				++count;
				if (count >= take)
				{
					foundEnought = true;
					break;
				}
			}
		}
	}
	else
	{
		// No items.
		foundEnought = true;
	}

	// Find more items.
	if (!foundEnought)
	{
		Aws::SimpleDB::Model::ListDomainsResult domainResult = results;

		// Keep getting more items until limit reached.
		do 
		{
			// Create a new request from to current key data.
			Aws::SimpleDB::Model::ListDomainsRequest request;
			request.SetNextToken(domainResult.GetNextToken());

			// Make the request.
			Aws::SimpleDB::Model::ListDomainsOutcome outcome = client->ListDomains(request);

			// If success.
			if (outcome.IsSuccess())
			{
				// Get the result.
				domainResult = outcome.GetResult();
				auto domainItems = domainResult.GetDomainNames();

				// Get the vector size.
				vectorSize = domainItems.size();

				// If items exist.
				if (vectorSize > 0)
				{
					// For each item found.
					for (int i = 0; i < vectorSize; i++)
					{
						// Add the item.
						auto item = domainItems[i];
						domains.push_back(item);

						// If not getting all.
						if (!getAll)
						{
							++count;
							if (count >= take)
							{
								foundEnought = true;
								break;
							}
						}
					}
				}
			}
			else
			{
				// Do not continue.
				break;
			}

		} while (!foundEnought);
	}

	// Return all domains.
	return domains;
}

///	<summary>
///	Add data to domain (table) and row (item).
///	</summary>
/// <param name="request">The domain request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SDBCloudAccount::PutAttributesAsync(
	const Aws::SimpleDB::Model::PutAttributesRequest& request,
	const Aws::SimpleDB::PutAttributesResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->PutAttributesAsync(request, handler, context);
}

///	<summary>
///	Get data from domain (table) and row (item).
///	</summary>
/// <param name="request">The get request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SDBCloudAccount::GetAttributesAsync(
	const Aws::SimpleDB::Model::GetAttributesRequest& request,
	const Aws::SimpleDB::GetAttributesResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->GetAttributesAsync(request, handler, context);
}

///	<summary>
///	Delete data from domain (table) and row (item).
///	</summary>
/// <param name="request">The delete request.</param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SDBCloudAccount::DeleteAttributesAsync(
	const Aws::SimpleDB::Model::DeleteAttributesRequest& request,
	const Aws::SimpleDB::DeleteAttributesResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->DeleteAttributesAsync(request, handler, context);
}

///	<summary>
///	Select data from domain (table) and row (item).
///	</summary>
/// <param name="request">
/// The select request; samples
/// SELECT a, b, c FROM DomainName WHERE a > 6 ORDER BY b LIMIT 100
/// </param>
/// <param name="handler">The function callback handler.</param>
/// <param name="context">The user defined context.</param>
void SDBCloudAccount::SelectAsync(
	const Aws::SimpleDB::Model::SelectRequest& request,
	const Aws::SimpleDB::SelectResponseReceivedHandler& handler,
	const std::shared_ptr<const Aws::Client::AsyncCallerContext>& context) const
{
	_client->SelectAsync(request, handler, context);
}

///	<summary>
///	Get all select items after request.
///	</summary>
/// <param name="client">The SimpleDB client.</param>
/// <param name="results">The select result.</param>
/// <param name="take">The maximum number of results to return (set take = -1 to return all results found).</param>
///	<return>The array of select results.</return>
const Aws::Vector<Aws::SimpleDB::Model::Item> SDBCloudAccount::GetItems(
	const Aws::SimpleDB::SimpleDBClient* client,
	const Aws::SimpleDB::Model::SelectResult& results,
	long long take) const
{
	long long count = 0;
	bool foundEnought = false;
	bool getAll = (take < 0 ? true : false);
	Aws::Vector<Aws::SimpleDB::Model::Item> selects;

	// Get the first set of items.
	Aws::Vector<Aws::SimpleDB::Model::Item> items = results.GetItems();

	// Get the vector size.
	size_t vectorSize = items.size();

	// If items exist.
	if (vectorSize > 0)
	{
		// For each item found.
		for (int i = 0; i < vectorSize; i++)
		{
			// Add the item.
			auto item = items[i];
			selects.push_back(item);

			// If not getting all.
			if (!getAll)
			{
				++count;
				if (count >= take)
				{
					foundEnought = true;
					break;
				}
			}
		}
	}
	else
	{
		// No items.
		foundEnought = true;
	}

	// Find more items.
	if (!foundEnought)
	{
		Aws::SimpleDB::Model::SelectResult selectResult = results;

		// Keep getting more items until limit reached.
		do
		{
			// Create a new request from to current key data.
			Aws::SimpleDB::Model::SelectRequest request;
			request.SetNextToken(selectResult.GetNextToken());

			// Make the request.
			Aws::SimpleDB::Model::SelectOutcome outcome = client->Select(request);

			// If success.
			if (outcome.IsSuccess())
			{
				// Get the result.
				selectResult = outcome.GetResult();
				auto selectItems = selectResult.GetItems();

				// Get the vector size.
				vectorSize = selectItems.size();

				// If items exist.
				if (vectorSize > 0)
				{
					// For each item found.
					for (int i = 0; i < vectorSize; i++)
					{
						// Add the item.
						auto item = selectItems[i];
						selects.push_back(item);

						// If not getting all.
						if (!getAll)
						{
							++count;
							if (count >= take)
							{
								foundEnought = true;
								break;
							}
						}
					}
				}
			}
			else
			{
				// Do not continue.
				break;
			}

		} while (!foundEnought);
	}

	// Return all selects.
	return selects;
}