
#pragma once

#ifndef _ARITHMETIC_H
#define _ARITHMETIC_H

#include "Stdafx.h"

namespace Nequeo
{
namespace Math
{
	typedef double (*ArithmeticHandler)(double, double);



	template <typename _ReturnType>
	_ReturnType FunctionHandler(_ReturnType (*Handler)())
	{
		return (*Handler)();
	}

	template <typename _ReturnType, typename _ParOneType>
	_ReturnType FunctionHandler(_ParOneType&& parOne, _ReturnType (*Handler)(_ParOneType))
	{
		return (*Handler)(std::forward<_ParOneType>(parOne));
	}

	template <typename _ReturnType, typename _ParOneType, typename _ParTwoType>
	_ReturnType FunctionHandler(_ParOneType&& parOne, _ParTwoType&& parTwo, _ReturnType (*Handler)(_ParOneType, _ParTwoType))
	{
		return (*Handler)(std::forward<_ParOneType>(parOne), std::forward<_ParTwoType>(parTwo));
	}

	template <typename _ReturnType, typename _ParOneType, typename _ParTwoType, typename _ParThreeType>
	_ReturnType FunctionHandler(_ParOneType&& parOne, _ParTwoType&& parTwo, _ParThreeType&& parThree, _ReturnType (*Handler)(_ParOneType, _ParTwoType, _ParThreeType))
	{
		return (*Handler)(std::forward<_ParOneType>(parOne), std::forward<_ParTwoType>(parTwo), std::forward<_ParThreeType>(parThree));
	}

	template <typename _ReturnType, typename _ParOneType, typename _ParTwoType, typename _ParThreeType, typename _ParFourType>
	_ReturnType FunctionHandler(_ParOneType&& parOne, _ParTwoType&& parTwo, _ParThreeType&& parThree, _ParFourType&& parFour, _ReturnType (*Handler)(_ParOneType, _ParTwoType, _ParThreeType, _ParFourType))
	{
		return (*Handler)(std::forward<_ParOneType>(parOne), std::forward<_ParTwoType>(parTwo), std::forward<_ParThreeType>(parThree), std::forward<_ParFourType>(parFour));
	}




	template <typename _ReturnType, typename _Function>
	_ReturnType FunctionCall(_Function&& Func)
	{
		return Func();
	}

	template <typename _ReturnType, typename _ParOneType, typename _Function>
	_ReturnType FunctionCall(_ParOneType&& parOne, _Function&& Func)
	{
		return Func(std::forward<_ParOneType>(parOne));
	}

	template <typename _ReturnType, typename _ParOneType, typename _ParTwoType, typename _Function>
	_ReturnType FunctionCall(_ParOneType&& parOne, _ParTwoType&& parTwo, _Function&& Func)
	{
		return Func(std::forward<_ParOneType>(parOne), std::forward<_ParTwoType>(parTwo));
	}

	template <typename _ReturnType, typename _ParOneType, typename _ParTwoType, typename _ParThreeType, typename _Function>
	_ReturnType FunctionCall(_ParOneType&& parOne, _ParTwoType&& parTwo, _ParThreeType&& parThree, _Function&& Func)
	{
		return Func(std::forward<_ParOneType>(parOne), std::forward<_ParTwoType>(parTwo), std::forward<_ParThreeType>(parThree));
	}

	template <typename _ReturnType, typename _ParOneType, typename _ParTwoType, typename _ParThreeType, typename _ParFourType, typename _Function>
	_ReturnType FunctionCall(_ParOneType&& parOne, _ParTwoType&& parTwo, _ParThreeType&& parThree, _ParFourType&& parFour, _Function&& Func)
	{
		return Func(std::forward<_ParOneType>(parOne), std::forward<_ParTwoType>(parTwo), std::forward<_ParThreeType>(parThree), std::forward<_ParFourType>(parFour));
	}

	///	<summary>
	///	Contains basic arithmetic calculations.
	///	</summary>
    class Arithmetic
    {
		public: 
			///	<summary>
			///	Addition.
			///	</summary>
			/// <param name='a'>The a parameter.</param>
			/// <param name='b'>The b parameter.</param>
			/// <returns>The addition of the parameters.</returns>
			static MATHCALCULUS_API double Add(double a, double b); 

			///	<summary>
			///	Subtraction.
			///	</summary>
			/// <param name='a'>The a parameter.</param>
			/// <param name='b'>The b parameter.</param>
			/// <returns>The subtraction of the parameters.</returns>
			static MATHCALCULUS_API double Subtract(double a, double b); 

			///	<summary>
			///	Multiplication.
			///	</summary>
			/// <param name='a'>The a parameter.</param>
			/// <param name='b'>The b parameter.</param>
			/// <returns>The multiplication of the parameters.</returns>
			static MATHCALCULUS_API double Multiply(double a, double b); 

			///	<summary>
			///	Division.
			///	</summary>
			/// <param name='a'>The a parameter.</param>
			/// <param name='b'>The b parameter.</param>
			/// <returns>The division of the parameters.</returns>
			/// <exception cref="std::invalid_argument">Thrown when b is 0.</exception>
			static MATHCALCULUS_API double Divide(double a, double b); 

			///	<summary>
			///	Is the number prime.
			///	</summary>
			/// <param name='n'>The n parameter.</param>
			/// <returns>True if the number is prime; else false.</returns>
			static MATHCALCULUS_API bool IsPrime(int n); 

    };
}
}

#endif //_ARITHMETIC_H