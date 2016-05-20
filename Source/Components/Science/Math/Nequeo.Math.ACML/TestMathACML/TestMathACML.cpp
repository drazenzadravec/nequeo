// TestMathACML.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <vector>
#include <iostream>

#include "RandomNumberGenerator.h"

int main()
{
	Nequeo::Math::ACML::RandomNumberGenerator numb;
	std::vector<double> rand = numb.Basic(20, 10.0, 5.0, 5645);
	std::vector<double> rand1 = numb.Basic(2000, 11.0, 5.0, 5645);

	std::cout << rand[19] << std::endl;
	std::cout << rand1[199] << std::endl;

    return 0;
}

