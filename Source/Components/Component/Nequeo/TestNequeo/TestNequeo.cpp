// TestNequeo.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>

#include "FunctionTemplates.h"

int Funct(int a, int b);

int main()
{
	auto ret = Nequeo::FunctionDefinition<int, int, int>(Funct);
	int value = Nequeo::FunctionCall<int>(5, 5, ret);

	std::cout << value << std::endl;

    return 0;
}

int Funct(int a, int b)
{
	return a * b;
}