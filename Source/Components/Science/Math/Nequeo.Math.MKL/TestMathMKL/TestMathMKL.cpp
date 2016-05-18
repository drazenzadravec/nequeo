// TestMathMKL.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <vector>
#include <iostream>

#include "RandomNumberGenerator.h"
#include "Solvers.h"
#include "Statistics.h"

int main()
{
	Nequeo::Math::MKL::RandomNumberGenerator numb;
	std::vector<double> rand = numb.Basic(20, 10.0, 5.0);
	std::vector<double> rand1 = numb.Basic(2000, 11.0, 5.0, 5645);

	std::cout << rand[19] << std::endl;
	std::cout << rand1[199] << std::endl;

	float af[2];
	double ad[2];

	af[0] = 45.78;
	af[1] = 475.78;

	ad[0] = 545.78;
	ad[1] = 75.78;

	Nequeo::Math::MKL::Solvers solv;
	std::vector<double> solvd = solv.Sqrt(2, ad);
	std::vector<float> solvf = solv.Sqrt(2, af);

	std::cout << solvf[0] << std::endl;
	std::cout << solvf[1] << std::endl;

	std::cout << solvd[0] << std::endl;
	std::cout << solvd[1] << std::endl;

	ComplexDouble cd[2];
	ComplexFloat cf[2];

	cd[0].imag = 34.78;
	cd[0].real = 134.78;
	cd[1].imag = 4.78;
	cd[1].real = 14.78;

	cf[0].imag = 364.78;
	cf[0].real = 1634.78;
	cf[1].imag = 46.78;
	cf[1].real = 164.78;

	std::vector<ComplexDouble> solcvd = solv.Sqrt(2, cd);
	std::vector<ComplexFloat> solcvf = solv.Sqrt(2, cf);

	std::cout << "Complex" << std::endl;
	std::cout << solcvf[0].imag << std::endl;
	std::cout << solcvf[0].real << std::endl;
	std::cout << solcvf[1].imag << std::endl;
	std::cout << solcvf[1].real << std::endl;

	std::cout << solcvd[0].imag << std::endl;
	std::cout << solcvd[0].real << std::endl;
	std::cout << solcvd[1].imag << std::endl;
	std::cout << solcvd[1].real << std::endl;

	std::cout << "Statistics" << std::endl;
	Nequeo::Math::MKL::Statistics stats;

	double samples[4] = { 1.0, 2.0, 3.0, 4.0 };
	double mean[1] = { 0.0 };
	double variation[1] = { 0.0 };
	double skewness[1] = { 0.0 };
	double kurtosis[1] = { 0.0 };
	double cov[1];
	double cor[1];

	stats.Sample(4, samples, mean, variation, skewness, kurtosis, cov, cor);
	std::cout << mean[0] << std::endl;
	std::cout << variation[0] << std::endl;
	std::cout << skewness[0] << std::endl;
	std::cout << kurtosis[0] << std::endl;
	std::cout << cov[0] << std::endl;
	std::cout << cor[0] << std::endl;

	std::cout << "Statistics Random" << std::endl;
	stats.Sample(rand1, mean, variation, skewness, kurtosis, cov, cor);
	std::cout << mean[0] << std::endl;
	std::cout << variation[0] << std::endl;
	std::cout << skewness[0] << std::endl;
	std::cout << kurtosis[0] << std::endl;
	std::cout << cov[0] << std::endl;
	std::cout << cor[0] << std::endl;

    return 0;
}

