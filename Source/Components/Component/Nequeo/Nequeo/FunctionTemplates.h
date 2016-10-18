/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          FunctionTemplates.h
*  Purpose :       Function templates definition header.
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

#ifndef _FUNCTIONTEMPLATES_H
#define _FUNCTIONTEMPLATES_H

#include "Global.h"
#include "Allocator.h"

#include <functional>

namespace Nequeo
{
	///	<summary>
	///	Function execution handler.
	///	</summary>
	/// <param name='function'>The function to execute.</param>
	/// <returns>The return value of the function execution.</returns>
	template <typename _ReturnType>
	_ReturnType FunctionHandler(_ReturnType(*Handler)())
	{
		return (*Handler)();
	}

	///	<summary>
	///	Function execution handler.
	///	</summary>
	/// <param name='parOne'>A parameter to pass to the function handler.</param>
	/// <param name='function'>The function to execute.</param>
	/// <returns>The return value of the function execution.</returns>
	template <typename _ReturnType, typename _ParOneType>
	_ReturnType FunctionHandler(_ParOneType&& parOne, _ReturnType(*Handler)(_ParOneType))
	{
		return (*Handler)(std::forward<_ParOneType>(parOne));
	}

	///	<summary>
	///	Function execution handler.
	///	</summary>
	/// <param name='parOne'>A parameter to pass to the function handler.</param>
	/// <param name='parTwo'>A parameter to pass to the function handler.</param>
	/// <param name='function'>The function to execute.</param>
	/// <returns>The return value of the function execution.</returns>
	template <typename _ReturnType, typename _ParOneType, typename _ParTwoType>
	_ReturnType FunctionHandler(_ParOneType&& parOne, _ParTwoType&& parTwo, _ReturnType(*Handler)(_ParOneType, _ParTwoType))
	{
		return (*Handler)(std::forward<_ParOneType>(parOne), std::forward<_ParTwoType>(parTwo));
	}

	///	<summary>
	///	Function execution handler.
	///	</summary>
	/// <param name='parOne'>A parameter to pass to the function handler.</param>
	/// <param name='parTwo'>A parameter to pass to the function handler.</param>
	/// <param name='parThree'>A parameter to pass to the function handler.</param>
	/// <param name='function'>The function to execute.</param>
	/// <returns>The return value of the function execution.</returns>
	template <typename _ReturnType, typename _ParOneType, typename _ParTwoType, typename _ParThreeType>
	_ReturnType FunctionHandler(_ParOneType&& parOne, _ParTwoType&& parTwo, _ParThreeType&& parThree, _ReturnType(*Handler)(_ParOneType, _ParTwoType, _ParThreeType))
	{
		return (*Handler)(std::forward<_ParOneType>(parOne), std::forward<_ParTwoType>(parTwo), std::forward<_ParThreeType>(parThree));
	}

	///	<summary>
	///	Function execution handler.
	///	</summary>
	/// <param name='parOne'>A parameter to pass to the function handler.</param>
	/// <param name='parTwo'>A parameter to pass to the function handler.</param>
	/// <param name='parThree'>A parameter to pass to the function handler.</param>
	/// <param name='parFour'>A parameter to pass to the function handler.</param>
	/// <param name='function'>The function to execute.</param>
	/// <returns>The return value of the function execution.</returns>
	template <typename _ReturnType, typename _ParOneType, typename _ParTwoType, typename _ParThreeType, typename _ParFourType>
	_ReturnType FunctionHandler(_ParOneType&& parOne, _ParTwoType&& parTwo, _ParThreeType&& parThree, _ParFourType&& parFour, _ReturnType(*Handler)(_ParOneType, _ParTwoType, _ParThreeType, _ParFourType))
	{
		return (*Handler)(std::forward<_ParOneType>(parOne), std::forward<_ParTwoType>(parTwo), std::forward<_ParThreeType>(parThree), std::forward<_ParFourType>(parFour));
	}

	///	<summary>
	///	Function execution call.
	///	</summary>
	/// <param name='Func'>The function to execute.</param>
	/// <returns>The return value of the function execution.</returns>
	template <typename _ReturnType, typename _Function>
	_ReturnType FunctionCall(_Function&& Func)
	{
		return Func();
	}

	///	<summary>
	///	Function execution call.
	///	</summary>
	/// <param name='parOne'>A parameter to pass to the function handler.</param>
	/// <param name='Func'>The function to execute.</param>
	/// <returns>The return value of the function execution.</returns>
	template <typename _ReturnType, typename _ParOneType, typename _Function>
	_ReturnType FunctionCall(_ParOneType&& parOne, _Function&& Func)
	{
		return Func(std::forward<_ParOneType>(parOne));
	}

	///	<summary>
	///	Function execution call.
	///	</summary>
	/// <param name='parOne'>A parameter to pass to the function handler.</param>
	/// <param name='parTwo'>A parameter to pass to the function handler.</param>
	/// <param name='Func'>The function to execute.</param>
	/// <returns>The return value of the function execution.</returns>
	template <typename _ReturnType, typename _ParOneType, typename _ParTwoType, typename _Function>
	_ReturnType FunctionCall(_ParOneType&& parOne, _ParTwoType&& parTwo, _Function&& Func)
	{
		return Func(std::forward<_ParOneType>(parOne), std::forward<_ParTwoType>(parTwo));
	}

	///	<summary>
	///	Function execution call.
	///	</summary>
	/// <param name='parOne'>A parameter to pass to the function handler.</param>
	/// <param name='parTwo'>A parameter to pass to the function handler.</param>
	/// <param name='parThree'>A parameter to pass to the function handler.</param>
	/// <param name='Func'>The function to execute.</param>
	/// <returns>The return value of the function execution.</returns>
	template <typename _ReturnType, typename _ParOneType, typename _ParTwoType, typename _ParThreeType, typename _Function>
	_ReturnType FunctionCall(_ParOneType&& parOne, _ParTwoType&& parTwo, _ParThreeType&& parThree, _Function&& Func)
	{
		return Func(std::forward<_ParOneType>(parOne), std::forward<_ParTwoType>(parTwo), std::forward<_ParThreeType>(parThree));
	}

	///	<summary>
	///	Function execution call.
	///	</summary>
	/// <param name='parOne'>A parameter to pass to the function handler.</param>
	/// <param name='parTwo'>A parameter to pass to the function handler.</param>
	/// <param name='parThree'>A parameter to pass to the function handler.</param>
	/// <param name='parFour'>A parameter to pass to the function handler.</param>
	/// <param name='Func'>The function to execute.</param>
	/// <returns>The return value of the function execution.</returns>
	template <typename _ReturnType, typename _ParOneType, typename _ParTwoType, typename _ParThreeType, typename _ParFourType, typename _Function>
	_ReturnType FunctionCall(_ParOneType&& parOne, _ParTwoType&& parTwo, _ParThreeType&& parThree, _ParFourType&& parFour, _Function&& Func)
	{
		return Func(std::forward<_ParOneType>(parOne), std::forward<_ParTwoType>(parTwo), std::forward<_ParThreeType>(parThree), std::forward<_ParFourType>(parFour));
	}

	///	<summary>
	///	Function definition.
	///	</summary>
	/// <param name='Func'>The function to execute.</param>
	/// <returns>The return value of the function execution.</returns>
	template <typename _ReturnType, typename _Function>
	auto FunctionDefinition(_Function&& Func)
	{
		// The auto return type implicited with '-> _ReturnType' explicid return type, lambda function handler.
		auto function = [&Func]() -> _ReturnType
		{
			return Func();
		};

		// Return the function definition.
		return function;
	}

	///	<summary>
	///	Function definition.
	///	</summary>
	/// <param name='Func'>The function to execute.</param>
	/// <returns>The return value of the function execution.</returns>
	template <typename _ReturnType, typename _ParOneType, typename _Function>
	auto FunctionDefinition(_Function&& Func)
	{
		// The auto return type implicited with '-> _ReturnType' explicid return type, lambda function handler.
		auto function = [&Func](_ParOneType&& one) -> _ReturnType
		{
			return Func(std::forward<_ParOneType>(one));
		};

		// Return the function definition.
		return function;
	}

	///	<summary>
	///	Function definition.
	///	</summary>
	/// <param name='Func'>The function to execute.</param>
	/// <returns>The return value of the function execution.</returns>
	template <typename _ReturnType, typename _ParOneType, typename _ParTwoType, typename _Function>
	auto FunctionDefinition(_Function&& Func)
	{
		// The auto return type implicited with '-> _ReturnType' explicid return type, lambda function handler.
		auto function = [&Func](_ParOneType&& one, _ParTwoType&& two) -> _ReturnType
		{
			return Func(std::forward<_ParOneType>(one), std::forward<_ParTwoType>(two));
		};

		// Return the function definition.
		return function;
	}

	///	<summary>
	///	Function definition.
	///	</summary>
	/// <param name='Func'>The function to execute.</param>
	/// <returns>The return value of the function execution.</returns>
	template <typename _ReturnType, typename _ParOneType, typename _ParTwoType, typename _ParThreeType, typename _Function>
	auto FunctionDefinition(_Function&& Func)
	{
		// The auto return type implicited with '-> _ReturnType' explicid return type, lambda function handler.
		auto function = [&Func](_ParOneType&& one, _ParTwoType&& two, _ParThreeType&& three) -> _ReturnType
		{
			return Func(std::forward<_ParOneType>(one), std::forward<_ParTwoType>(two), std::forward<_ParThreeType>(three));
		};

		// Return the function definition.
		return function;
	}

	///	<summary>
	///	Function definition.
	///	</summary>
	/// <param name='Func'>The function to execute.</param>
	/// <returns>The return value of the function execution.</returns>
	template <typename _ReturnType, typename _ParOneType, typename _ParTwoType, typename _ParThreeType, typename _ParFourType, typename _Function>
	auto FunctionDefinition(_Function&& Func)
	{
		// The auto return type implicited with '-> _ReturnType' explicid return type, lambda function handler.
		auto function = [&Func](_ParOneType&& one, _ParTwoType&& two, _ParThreeType&& three, _ParFourType&& four) -> _ReturnType
		{
			return Func(std::forward<_ParOneType>(one), std::forward<_ParTwoType>(two), std::forward<_ParThreeType>(three), std::forward<_ParFourType>(four));
		};

		// Return the function definition.
		return function;
	}

	///	<summary>
	///	Function build (_Function can be defined as : int (int, bool) return int pass int and bool parameters).
	///	</summary>
	/// <returns>The created function.</returns>
	template<typename _Function>
	std::function< _Function > FunctionBuild(const _Function& f)
	{
		return std::function< _Function >(std::allocator_arg_t(), Nequeo::Allocator<void>(), f);
	}

	///	<summary>
	///	Function build (_Function can be defined as : int (int, bool) return int pass int and bool parameters).
	///	</summary>
	/// <returns>The created function.</returns>
	template<typename _Function>
	std::function< _Function > FunctionBuild(const std::function< _Function >& f)
	{
		return std::function< _Function >(std::allocator_arg_t(), Nequeo::Allocator<void>(), f);
	}

	///	<summary>
	///	Function build (_Function can be defined as : int (int, bool) return int pass int and bool parameters).
	///	</summary>
	/// <returns>The created function.</returns>
	template<typename _Function>
	std::function< _Function > FunctionBuild(std::function< _Function >&& f)
	{
		return std::function< _Function >(std::allocator_arg_t(), Nequeo::Allocator<void>(), f);
	}
}
#endif