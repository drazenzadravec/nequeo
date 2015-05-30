// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently,
// but are changed infrequently

#pragma once

#ifdef NEQUEOMATHCALCULUS_EXPORTS
	#define MATHCALCULUS_API __declspec(dllexport) 
#else
	#define MATHCALCULUS_API __declspec(dllimport) 
#endif

#define PI 3.1415926535897932384626433832795
#define MAXDEGREE 100
#define MDP1 MAXDEGREE + 1

#include <math.h>
#include <cctype>
#include <cmath>
#include <cfloat>

#include <stdio.h>
#include <time.h>
#include <ctime>
#include <stdlib.h>
#include <cstdlib>

#include <string>

#include <complex>
#include <ccomplex>
#include <memory>
#include <exception>
#include <stdexcept>

#include <functional>
#include <ppl.h>
#include <algorithm>

using namespace std;