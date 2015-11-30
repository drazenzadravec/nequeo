/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          CableCompany.cpp
 *  Purpose :       
 *					Class CableCompany
 *
 *					One possible use of bitsets is tracking channels of cable subscribers. Each subscriber could have a
					bitset of channels associated with his or her subscription, with set bits representing the channels to
					which he or she actually subscribes. This system could also support “packages” of channels, also represented
					as bitsets, which represent commonly subscribed combinations of channels.

					The following CableCompany class is a simple example of this model. It uses two maps, each of string/
					bitset, storing the cable packages as well as the subscriber information.
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

#include "CableCompany.h"

void Nequeo::Collections::Pool::CableCompany::addPackage(const string& packageName,const bitset<kNumChannels>& channels)
{
	// Just make a key/value pair and insert it into the packages map.
	mPackages.insert(make_pair(packageName, channels));
}

void Nequeo::Collections::Pool::CableCompany::removePackage(const string& packageName)
{
	// Just erase the package from the package map.
	mPackages.erase(packageName);
}

void Nequeo::Collections::Pool::CableCompany::newCustomer(const string& name, const string& package) throw (out_of_range)
{
	// Get a reference to the specified package.
	MapType::const_iterator it = mPackages.find(package);

	if (it == mPackages.end()) 
	{
		// That package doesn’t exist. Throw an exception.
		throw (out_of_range("Invalid package"));
	} 
	else 
	{
		// Create the account with the bitset representing that package.
		// Note that it refers to a name/bitset pair. The bitset is the
		// second field.
		mCustomers.insert(make_pair(name, it->second));
	}
}

void Nequeo::Collections::Pool::CableCompany::newCustomer(const string& name, const bitset<kNumChannels>& channels)
{
	// Just add the customer/channels pair to the customers map.
	mCustomers.insert(make_pair(name, channels));
}

void Nequeo::Collections::Pool::CableCompany::addChannel(const string& name, int channel)
{
	// Find a reference to the customers.
	MapType::iterator it = mCustomers.find(name);

	if (it != mCustomers.end()) 
	{
		// We found this customer; set the channel.
		// Note that it is a reference to a name/bitset pair.
		// The bitset is the second field.
		it->second.set(channel);
	}
}

void Nequeo::Collections::Pool::CableCompany::removeChannel(const string& name, int channel)
{
	// Find a reference to the customers.
	MapType::iterator it = mCustomers.find(name);

	if (it != mCustomers.end()) 
	{
		// We found this customer; remove the channel.
		// Note that it is a refernce to a name/bitset pair.
		// The bitset is the second field.
		it->second.reset(channel);
	}
}

void Nequeo::Collections::Pool::CableCompany::addPackageToCustomer(const string& name, const string& package)
{
	// Find the package.
	MapType::iterator itPack = mPackages.find(package);

	// Find the customer.
	MapType::iterator itCust = mCustomers.find(name);

	if (itCust != mCustomers.end() && itPack != mPackages.end()) 
	{
		// Only if both package and customer are found, can we do the update.
		// Or-in the package to the customers existing channels.
		// Note that it is a reference to a name/bitset pair.
		// The bitset is the second field.
		itCust->second |= itPack->second;
	}
}

void Nequeo::Collections::Pool::CableCompany::deleteCustomer(const string& name)
{
	// Remove the customer with this name.
	mCustomers.erase(name);
}

bitset<Nequeo::Collections::Pool::kNumChannels>& Nequeo::Collections::Pool::CableCompany::getCustomerChannels(const string& name) throw (out_of_range)
{
	// Find the customer.
	MapType::iterator it = mCustomers.find(name);

	if (it != mCustomers.end()) 
	{
		// Found it!
		// Note that it is a reference to a name/bitset pair.
		// The bitset is the second field.
		return (it->second);
	}

	// Didn’t find it. Throw an exception.
	throw (out_of_range("No customer of that name"));
}