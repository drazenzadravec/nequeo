/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
 * 
 *  File :          AuditVoterRoll.h
 *  Purpose :       
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

using namespace System;

namespace Nequeo 
{
	namespace Collections 
	{
		namespace Pool 
		{
			// auditVoterRolls
			//
			// Expects a map of string/list<string> pairs keyed on county names
			// and containing lists of all the registered voters in those counties
			//
			// Removes from each list any name on the convictedFelons list and
			// any name that is found on any other list
			void auditVoterRolls(map<string, list<string> >& votersByCounty, const list<string>& convictedFelons)
			{
				// Get all the duplicate names.
				list<string> duplicates = getDuplicates(votersByCounty);

				// Combine the duplicates and convicted felons--we want
				// to remove names on both lists from all voter rolls.
				duplicates.insert(duplicates.end(), convictedFelons.begin(), convictedFelons.end());

				// If there were any duplicates, remove them.
				// Use the list versions of sort and unique instead of the generic
				// algorithms, because the list versions are more efficient.
				duplicates.sort();
				duplicates.unique();

				// Now remove all the names we need to remove.
				for_each(votersByCounty.begin(), votersByCounty.end(), RemoveNames(duplicates));
			}

			// getDuplicates()
			//
			// Returns a list of all names that appear in more than one list in
			// the map
			//
			// The implementation generates one large list of all the names from
			// all the lists in the map, sorts it, then finds all duplicates
			// in the sorted list with adjacent_find().
			list<string> getDuplicates(const map<string, list<string> >& voters)
			{
				list<string> allNames, duplicates;

				// Collect all the names from all the lists into one big list.
				map<string, list<string> >::const_iterator it;

				for(it = voters.begin(); it != voters.end(); ++it) 
				{
					allNames.insert(allNames.end(), it->second.begin(), it->second.end());
				}

				// Sort the list--use the list version, not the general algorithm,
				// because the list version is faster.
				allNames.sort();

				// Now that it’s sorted, all duplicate names will be next to each other.
				// Use adjacent_find() to find instances of two or more identical names
				// next to each other.
				//
				// Loop until adjacent_find returns the end iterator.
				list<string>::iterator lit;
				for (lit = allNames.begin(); lit != allNames.end(); ++lit) 
				{
					lit = adjacent_find(lit, allNames.end());
					if (lit == allNames.end()) 
					{
						break;
					}

					duplicates.push_back(*lit);
				}
	
				// If someone was on more than two voter lists, he or she will
				// show up more than once in the duplicates list. Sort the list
				// and remove duplicates with unique.
				//
				// Use the list versions because they are faster than the generic versions.
				duplicates.sort();
				duplicates.unique();
				return (duplicates);
			}

			// RemoveNames
			//
			// Functor class that takes a string/list<string> pair and removes
			// any strings from the list that are found in a list of names
			// (supplied in the constructor)
			class RemoveNames : public unary_function<pair<const string, list<string> >, void>
			{
				public:
					RemoveNames(const list<string>& names) : mNames(names) {}
					void operator() (pair<const string, list<string> >& val);

				protected:
					const list<string>& mNames;
			};

			// Function-call operator for RemoveNames functor.
			//
			// Uses remove_if() followed by erase to actually delete the names
			// from the list
			//
			// Names are removed if they are in our list of mNames. Use the NameInList
			// functor to check if the name is in the list.
			void RemoveNames::operator() (pair<const string, list<string> >& val)
			{
				list<string>::iterator it = remove_if(val.second.begin(), val.second.end(), NameInList(mNames));
				val.second.erase(it, val.second.end());
			}

			// NameInList
			//
			// Functor to check if a string is in a list of strings (supplied
			// at construction time).
			class NameInList : public unary_function<string, bool>
			{
				public:
					NameInList(const list<string>& names) : mNames(names) {}
					bool operator() (const string& val);

				protected:
				const list<string>& mNames;
			};

			// function-call operator for NameInList functor
			//
			// Returns true if it can find name in mNames, false otherwise.
			// Uses find() algorithm.
			bool NameInList::operator() (const string& name)
			{
				return (find(mNames.begin(), mNames.end(), name) != mNames.end());
			}
		}
	}
}