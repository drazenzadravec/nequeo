/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          CableCompany.h
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

#pragma once

#include "stdafx.h"

using namespace System;

namespace Nequeo 
{
	namespace Collections 
	{
		namespace Pool 
		{
			const int kNumChannels = 10;

			class CableCompany
			{
				public:
					CableCompany() {}

					// Adds the package with the specified channels to the databse
					void addPackage(const string& packageName,

					const bitset<kNumChannels>& channels);

					// Removes the specified package from the database
					void removePackage(const string& packageName);

					// Adds the customer to the database with initial channels found in package
					// Throws out_of_range if the package name is invalid.
					void newCustomer(const string& name, const string& package) throw (out_of_range);

					// Adds the customer to the database with initial channels specified
					// in channels
					void newCustomer(const string& name, const bitset<kNumChannels>& channels);

					// Adds the channel to the customers profile
					void addChannel(const string& name, int channel);

					// Removes the channel from the customers profile
					void removeChannel(const string& name, int channel);

					// Adds the specified package to the customers profile
					void addPackageToCustomer(const string& name, const string& package);

					// Removes the specified customer from the database
					void deleteCustomer(const string& name);

					// Retrieves the channels to which this customer subscribes
					// Throws out_of_range if name is not a valid customer
					bitset<kNumChannels>& getCustomerChannels(const string& name) throw (out_of_range);

				protected:
					typedef map<string, bitset<kNumChannels> > MapType;
					MapType mPackages, mCustomers;

			};
		}
	}
}