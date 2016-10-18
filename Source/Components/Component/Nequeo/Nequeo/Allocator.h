/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Allocator.h
*  Purpose :       Allocator class.
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

#include "Global.h"
#include "Memory.h"

#include <memory>
#include <cstdlib>
#include <deque>
#include <queue>
#include <list>
#include <map>
#include <set>
#include <stack>
#include <iostream>
#include <string>
#include <sstream>
#include <vector>
#include <functional>

namespace Nequeo
{
	/**
	* Std allocator interface that is used for all STL types in the event that Custom Memory Management is being used.
	*/
	template <typename T>
	class Allocator : public std::allocator<T>
	{
	public:

		typedef std::allocator<T> Base;

		Allocator() throw() :
			Base()
		{}

		Allocator(const Allocator<T>& a) throw() :
			Base(a)
		{}

		template <class U>
		Allocator(const Allocator<U>& a) throw() :
			Base(a)
		{}

		~Allocator() throw() {}

		typedef std::size_t size_type;

		template<typename U>
		struct rebind
		{
			typedef Allocator<U> other;
		};

		typename Base::pointer allocate(size_type n, const void *hint = nullptr)
		{
			NEQUEO_UNREFERENCED_PARAM(hint);

			return reinterpret_cast<typename Base::pointer>(Malloc("NEQUEOSTL", n * sizeof(T)));
		}

		void deallocate(typename Base::pointer p, size_type n)
		{
			NEQUEO_UNREFERENCED_PARAM(n);

			Nequeo::Free(p);
		}

	};

	/**
	* Creates a shared_ptr using AWS Allocator hooks.
	* allocationTag is for memory tracking purposes.
	*/
	template<typename T, typename ...ArgTypes>
	std::shared_ptr<T> MakeShared(const char* allocationTag, ArgTypes&&... args)
	{
		NEQUEO_UNREFERENCED_PARAM(allocationTag);

		return std::allocate_shared<T, Nequeo::Allocator<T>>(Nequeo::Allocator<T>(), std::forward<ArgTypes>(args)...);
	}

	template< typename T > using Vector = std::vector< T, Nequeo::Allocator< T > >;
	template< typename T > using Deque = std::deque< T, Nequeo::Allocator< T > >;
	template< typename T > using List = std::list< T, Nequeo::Allocator< T > >;
	template< typename K, typename V > using Map = std::map< K, V, std::less< K >, Nequeo::Allocator< std::pair< const K, V > > >;
	template< typename V> using CStringMap = std::map<const char*, V, CompareStrings, Nequeo::Allocator<std::pair<const char*, V> > >;
	template< typename K, typename V > using MultiMap = std::multimap< K, V, std::less< K >, Nequeo::Allocator< std::pair< const K, V > > >;
	template< typename T > using Queue = std::queue< T, Deque< T > >;
	template< typename T > using Set = std::set< T, std::less< T >, Nequeo::Allocator< T > >;
	template< typename T > using Stack = std::stack< T, Deque< T > >;


	// Serves no purpose other than to help my conversion process
	typedef std::basic_ifstream< char, std::char_traits< char > > IFStream;
	typedef std::basic_ofstream< char, std::char_traits< char > > OFStream;
	typedef std::basic_fstream< char, std::char_traits< char > > FStream;
	typedef std::basic_istream< char, std::char_traits< char > > IStream;
	typedef std::basic_ostream< char, std::char_traits< char > > OStream;
	typedef std::basic_iostream< char, std::char_traits< char > > IOStream;
	typedef std::istreambuf_iterator< char, std::char_traits< char > > IStreamBufIterator;

	using IOStreamFactory = std::function< Nequeo::IOStream*(void) >;

	using String = std::basic_string< char, std::char_traits< char >, Nequeo::Allocator< char > >;
	using WString = std::basic_string< wchar_t, std::char_traits< wchar_t >, Nequeo::Allocator< wchar_t > >;

	typedef std::basic_stringstream< char, std::char_traits< char >, Nequeo::Allocator< char > > StringStream;
	typedef std::basic_istringstream< char, std::char_traits< char >, Nequeo::Allocator< char > > IStringStream;
	typedef std::basic_ostringstream< char, std::char_traits< char >, Nequeo::Allocator< char > > OStringStream;
	typedef std::basic_stringbuf< char, std::char_traits< char >, Nequeo::Allocator< char > > StringBuf;
}