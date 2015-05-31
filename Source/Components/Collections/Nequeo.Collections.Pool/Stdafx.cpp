// stdafx.cpp : source file that includes just the standard includes
// Nequeo.Collections.Pool.pch will be the pre-compiled header
// stdafx.obj will contain the pre-compiled type information

#include "stdafx.h"
#include "DefaultHash.h"

using namespace Nequeo::Collections::Pool;

/*
// Here is a small test program demonstrating the basic "hashmap" class template.
void test_hashmap()
{
	hashmap<int, int> myHash;
	myHash.insert(make_pair(4, 40));
	myHash.insert(make_pair(6, 60));

	hashmap<int, int>::value_type* x = myHash.find(4);
	if (x != NULL) 
	{
		cout << "4 maps to " << x->second << endl;
	} 
	else 
	{
		cout << "cannot find 4 in map\n";
	}

	myHash.erase(4);
	hashmap<int, int>::value_type* x2 = myHash.find(4);
	if (x2 != NULL) 
	{
		cout << "4 maps to " << x2->second << endl;
	} 
	else 
	{
		cout << "cannot find 4 in map\n";
	}

	myHash[4] = 35;
}
*/

// Suppose that you wanted to find all the elements matching a predicate in a given range. find() and
// find_if() are the most likely candidate algorithms, but each return an iterator referring to only one
// element. In fact, there is no standard algorithm to find all the elements matching a predicate. Luckily,
// you can write your own version of this functionality called find_all().
template <typename InputIterator, typename Predicate> 
vector<InputIterator> find_all(InputIterator first, InputIterator last, Predicate pred)
{
	vector<InputIterator> res;
	while (true) 
	{
		// Find the next match in the current range.
		first = find_if(first, last, pred);

		// check if find_if failed to find a match
		if (first == last) 
		{
			break;
		}

		// Store this match.
		res.push_back(first);

		// Shorten the range to start at one past the current match
		++first;
	}
	return (res);
}

// Here is some code that tests the function (find_all)
void test_find_all()
{
	int arr[] = {3, 4, 5, 4, 5, 6, 5, 8};
	vector<int*> all = find_all(arr, arr + 8, bind2nd(equal_to<int>(), 5));
	cout << "Found " << all.size() << " matching elements: ";

	for (vector<int*>::iterator it = all.begin(); it != all.end(); ++it) 
	{
		cout << **it << " ";
	}
	cout << endl;
}

// C++ provides a class template called iterator_traits that allows you to find this info. You instantiate
// the iterator_traits class template with the iterator type of interest, and access one of five typedefs:
// value_type, difference_type, iterator_category, pointer, and reference. For example, the
// following template function declares a temporary variable of the type to which an iterator of type
// IteratorType refers:
template <typename IteratorType>
void iteratorTraitsTest(IteratorType it)
{
	typename std::iterator_traits<IteratorType>::value_type temp;
	temp = *it;
	cout << temp << endl;
}

// classLists is a vector of lists, one for each course. The lists
// contain the students enrolled in those courses. They are not sorted.
//
// droppedStudents is a list of students who failed to pay their
// tuition and so were dropped from their courses.
//
// The function returns a list of every enrolled (nondropped) student in
// all the courses.
list<string> getTotalEnrollment(const vector<list<string> >& classLists, const list<string>& droppedStudents)
{
	list<string> allStudents;

	// Concatenate all the course lists onto the master list.
	for (size_t i = 0; i < classLists.size(); ++i) 
	{
		allStudents.insert(allStudents.end(), classLists[i].begin(), classLists[i].end());
	}

	// Sort the master list.
	allStudents.sort();

	// Remove duplicate student names (those who are in multiple courses).
	allStudents.unique();

	// Remove students who are on the dropped list.
	// Iterate through the dropped list, calling remove on the
	// master list for each student in the dropped list.
	for (list<string>::const_iterator it = droppedStudents.begin(); it != droppedStudents.end(); ++it) 
	{
		allStudents.remove(*it);
	}

	// Done!
	return (allStudents);
}

// Function template to populate a container (T) of ints.
// The container (T) must support push_back().
template<typename T>
void populateContainer(T& cont)
{
	int num;

	while (true) 
	{
		cout << "Enter a number (0 to quit): ";
		cin >> num;

		if (num == 0) 
		{
			break;
		}

		cont.push_back(num);
	}
}