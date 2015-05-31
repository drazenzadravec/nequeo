/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          MapBankAccount.cpp
 *  Purpose :       
 *					Class MapBankAccount
 *
 *					Provides an map association container example.
 *
 *					You can implement a simple bank account database using a map. A common pattern is for the key to be
					one field of a class or struct that is stored in the map. In this case, the key is the account number. Here
					are simple BankAccount and BankDB classes:
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

#include "MapBankAccount.h"

bool Nequeo::Collections::Pool::BankDB::addAccount(const MapBankAccount& acct)
{
	// Declare a variable to store the return from insert().
	pair<map<int, MapBankAccount>::iterator, bool> res;

	// Do the actual insert, using the account number as the key.
	res = mAccounts.insert(make_pair(acct.getAcctNum(), acct));

	// Return the bool field of the pair specifying success or failure.
	return (res.second);
}

void Nequeo::Collections::Pool::BankDB::deleteAccount(int acctNum)
{
	mAccounts.erase(acctNum);
}

Nequeo::Collections::Pool::MapBankAccount& Nequeo::Collections::Pool::BankDB::findAccount(int acctNum) throw(out_of_range)
{
	// Finding an element via its key can be done with find().
	map<int, MapBankAccount>::iterator it = mAccounts.find(acctNum);

	if (it == mAccounts.end()) 
	{
		throw (out_of_range("No account with that number."));
	}

	// Remember that iterators into maps refer to pairs of key/value.
	return (it->second);
}

Nequeo::Collections::Pool::MapBankAccount& Nequeo::Collections::Pool::BankDB::findAccount(const string& name) throw(out_of_range)
{
	// Finding an element by a non-key attribute requires a linear
	// search through the elements.
	for (map<int, MapBankAccount>::iterator it = mAccounts.begin(); it != mAccounts.end(); ++it) 
	{
		if (it->second.getClientName() == name) 
		{
			// Found it!
			return (it->second);
		}
	}

	throw (out_of_range("No account with that name."));
}

void Nequeo::Collections::Pool::BankDB::mergeDatabase(BankDB& db)
{
	// Just insert copies of all the accounts in the old db
	// into the new one.

	mAccounts.insert(db.mAccounts.begin(), db.mAccounts.end());
	// Now delete all the accounts in the old one.
	db.mAccounts.clear();
}