/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          MapBankAccount.h
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

#pragma once

#include "stdafx.h"

using namespace System;

namespace Nequeo 
{
	namespace Collections 
	{
		namespace Pool 
		{
			///	<summary>
			///	Provides an map association container example..
			///	</summary>
			class MapBankAccount
			{
				public:
					MapBankAccount(int acctNum, const string& name) : mAcctNum(acctNum), mClientName(name) {}

					void setAcctNum(int acctNum) { mAcctNum = acctNum; }
					int getAcctNum() const {return (mAcctNum); }
					void setClientName(const string& name) { mClientName = name; }
					string getClientName() const { return mClientName; }
					

				protected:
					int mAcctNum;
					string mClientName;

			};

			///	<summary>
			///	Provides an map association container example..
			///	</summary>
			class BankDB
			{
				public:
					BankDB() {}

					// Adds acct to the bank database. If an account
					// exists already with that number, the new account is
					// not added. Returns true if the account is added, false
					// if it’s not.
					bool addAccount(const MapBankAccount& acct);

					// Removes the account acctNum from the database
					void deleteAccount(int acctNum);

					// Returns a reference to the account represented
					// by its number or the client name.
					// Throws out_of_range if the account is not found
					MapBankAccount& findAccount(int acctNum) throw(out_of_range);
					MapBankAccount& findAccount(const string& name) throw(out_of_range);

					// Adds all the accounts from db to this database.
					// Deletes all the accounts in db.
					void mergeDatabase(BankDB& db);

				protected:
					map<int, MapBankAccount> mAccounts;

			};
		}
	}
}