
#include "Arithmetic.h"

using namespace std;
using namespace concurrency;

///	<summary>
///	Addition.
///	</summary>
/// <param name='a'>The a parameter.</param>
/// <param name='b'>The b parameter.</param>
/// <returns>The addition of the parameters.</returns>
double Nequeo::Math::Arithmetic::Add(double a, double b)
{
	// The auto return type implicited, lambda function handler.
	auto addFunction = [] (double aValue, double bValue)
	{
		return aValue + bValue;
	};
	
	return addFunction(a, b);
}

///	<summary>
///	Subtraction.
///	</summary>
/// <param name='a'>The a parameter.</param>
/// <param name='b'>The b parameter.</param>
/// <returns>The subtraction of the parameters.</returns>
double Nequeo::Math::Arithmetic::Subtract(double a, double b)
{
    // The return type explicid, lambda function handler.
	function<double (double, double)> subtractFunction = [] (double aValue, double bValue)
	{
		return aValue - bValue;
	};
	
	return subtractFunction(a, b);
}

///	<summary>
///	Multiplication.
///	</summary>
/// <param name='a'>The a parameter.</param>
/// <param name='b'>The b parameter.</param>
/// <returns>The multiplication of the parameters.</returns>
double Nequeo::Math::Arithmetic::Multiply(double a, double b)
{
    // The auto return type implicited with '-> double' explicid return type, lambda function handler.
	auto multiplyFunction = [] (double aValue, double bValue) -> double
	{
		return aValue * bValue;
	};

	return multiplyFunction(a, b);
}

///	<summary>
///	Division.
///	</summary>
/// <param name='a'>The a parameter.</param>
/// <param name='b'>The b parameter.</param>
/// <returns>The division of the parameters.</returns>
/// <exception cref="std::invalid_argument">Thrown when b is 0.</exception>
double Nequeo::Math::Arithmetic::Divide(double a, double b)
{
    if (b == 0)
    {
        throw invalid_argument("b cannot be zero!");
    }

	// The auto return type implicited with '-> double' explicid return type, lambda function handler.
	auto divideFunction = [] (double aValue, double bValue) -> double
	{
		return aValue / bValue;
	};

	return FunctionCall<double>(a, b, divideFunction);
	// Below return is equivalent to the above return.
	// return FunctionCall<double, double&, double&>(a, b, divideFunction);
}

///	<summary>
///	Is the number prime.
///	</summary>
/// <param name='n'>The n parameter.</param>
/// <returns>True if the number is prime; else false.</returns>
bool Nequeo::Math::Arithmetic::IsPrime(int n)
{
	if (n < 2)
		return false;

	for (int i = 2; i < n; ++i)
	{
		if ((n % i) == 0)
			return false;
	}

	return true;
	
	//auto f = async([=] () { return 0;});

	/*double (*function)(double x) = &sin;
	double jj = FunctionHandler(5.6, function);

	ArithmeticHandler hh = &Add;
	double rrr = FunctionHandler(5.6, 3.5, hh);*/
}