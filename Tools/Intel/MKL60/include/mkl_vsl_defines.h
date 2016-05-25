/*
//               INTEL CORPORATION PROPRIETARY INFORMATION
//  This software is supplied under the terms of a license agreement or
//  nondisclosure agreement with Intel Corporation and may not be copied
//  or disclosed except in accordance with the terms of that agreement.
//    Copyright (C) 2001-2003 Intel Corporation. All Rights Reserved.
//
// File   : mkl_vsl_defines.h
// Purpose: VSL definitions (Windows version)
*/

#ifndef __VSL_DEFINES_H__
#define __VSL_DEFINES_H__

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

/*******************************************************************************
*************************  Max allowed BRNGs to be used  ***********************
*******************************************************************************/
#define VSL_MAX_REG_BRNGS           512

/*******************************************************************************
*********************************  BRNG names  *********************************
*******************************************************************************/
#define VSL_BRNG_MCG31      0
#define VSL_BRNG_R250       1
#define VSL_BRNG_MRG32K3A   2
#define VSL_BRNG_MCG59      3
#define VSL_BRNG_WH         4

/*******************************************************************************
********************************  Method names  ********************************
*******************************************************************************/

/******************************************************************************/
/* Uniform */
#define VSL_METHOD_SUNIFORM_STD 0
#define VSL_METHOD_DUNIFORM_STD 0
#define VSL_METHOD_IUNIFORM_STD 0

/******************************************************************************/
/* UniformBits */
#define VSL_METHOD_IUNIFORMBITS_STD 0

/******************************************************************************/
/* Gaussian (normal) */
/* Comments:
// BOXMULLER - generates a normal distributed random number x via a pair of
//             uniforms u1, u2 according to the formula
//             x=sqrt(-ln(u1))*sin(2*Pi*u2)
// BOXMULLER2- generates a pair of normal distributed random numbers x1, x2 
//             via a pair of uniforms u1, u2 according to the formula
//             x1=sqrt(-ln(u1))*sin(2*Pi*u2)
//             x2=sqrt(-ln(u1))*cos(2*Pi*u2)
//             Method implemented so that it correctly processes odd vector
//             length, i.e. if a call ends with generating x1, the next 
//             call starts from generating x2
*/
#define VSL_METHOD_SGAUSSIAN_BOXMULLER   0
#define VSL_METHOD_SGAUSSIAN_BOXMULLER2  1
#define VSL_METHOD_DGAUSSIAN_BOXMULLER   0
#define VSL_METHOD_DGAUSSIAN_BOXMULLER2  1

/******************************************************************************/
/* Exponential */
/* Comments:
// ICDF - inverse cumulative distribution function method according to formula
//        x=-ln(u), where x - exponentially distributed random number,
//        u - uniformly distributed random number
*/
#define VSL_METHOD_SEXPONENTIAL_ICDF 0
#define VSL_METHOD_DEXPONENTIAL_ICDF 0

/******************************************************************************/
/* Laplace */
/* Comments:
// ICDF - inverse cumulative distribution function method according to formula
//        x=+/-ln(u) with probability 1/2, where x - Laplace 
//        distributed random number, u - uniformly distributed random number
*/
#define VSL_METHOD_SLAPLACE_ICDF 0
#define VSL_METHOD_DLAPLACE_ICDF 0

/******************************************************************************/
/* Weibull */
/* Comments:
// ICDF - inverse cumulative distribution function method according to formula
//        x=(-ln(u))^(1/alpha), where x - Weibull distributed random number, 
//        u - uniformly distributed random number
*/
#define VSL_METHOD_SWEIBULL_ICDF 0
#define VSL_METHOD_DWEIBULL_ICDF 0

/******************************************************************************/
/* Cauchy */
/* Comments:
// ICDF - inverse cumulative distribution function method according to formula
//        x=tan(u), where x - Cauchy distributed random number, u - uniformly 
//        distributed random number in interval (-Pi/2,Pi/2)
*/
#define VSL_METHOD_SCAUCHY_ICDF 0
#define VSL_METHOD_DCAUCHY_ICDF 0

/******************************************************************************/
/* Rayleigh */
/* Comments:
// ICDF - inverse cumulative distribution function method according to formula
//        x=(-ln(u))^(1/2), where x - Rayleigh distributed random number, 
//        u - uniformly distributed random number. Rayleigh distribution is the
//	  special case of Weibull distribution, where alpha=2
*/
#define VSL_METHOD_SRAYLEIGH_ICDF 0
#define VSL_METHOD_DRAYLEIGH_ICDF 0

/******************************************************************************/
/* Lognormal */
/* Comments:
// ICDF - inverse cumulative distribution function method according to formula
//        x=exp(y), where x - lognormally distributed random number, 
//        y - normally distributed random number.
*/
#define VSL_METHOD_SLOGNORMAL_ICDF 0
#define VSL_METHOD_DLOGNORMAL_ICDF 0

/******************************************************************************/
/* Gumbel */
/* Comments:
// ICDF - inverse cumulative distribution function method according to formula
//        x=(ln(y)), where x - Gumbel distributed random number, 
//        y - exponentially distributed random number
*/
#define VSL_METHOD_SGUMBEL_ICDF 0
#define VSL_METHOD_DGUMBEL_ICDF 0

/******************************************************************************/
/* Bernoulli */
/* Comments:
// ICDF - inverse cumulative distribution function method according to formula
//        x=0 if u<=p and x=1 if u>p, where x - Bernoulli distributed random 
//        number, u - uniformly distributed random number
*/
#define VSL_METHOD_IBERNOULLI_ICDF 0

/******************************************************************************/
/* Geometric */
/* Comments:
// ICDF - inverse cumulative distribution function method according to formula
//        x=floor(ln(u)/ln(1-p)), where x - geometrically distributed random 
//        number, u - uniformly distributed random number
*/
#define VSL_METHOD_IGEOMETRIC_ICDF 0

/******************************************************************************/
/* Binomial */
/* Comments:
// BTPE - for ntrial*min(p,1-p)>30 acceptance/rejection method with 
//        decomposition onto 4 regions:
//        combined 2 parallelograms, triangle, left exponential tail,
//        right exponential tail;
//        othewise table lookup method
*/
#define VSL_METHOD_IBINOMIAL_BTPE 0

/******************************************************************************/
/* Hypergeometric */
/* Comments:
// H2PE - if mode of distribution is large, acceptance/rejection method is used
//        with decomposition onto 3 regions:
//        rectangular, left exponential tail and right exponential tail;
//        othewise table lookup method is used
*/
#define VSL_METHOD_IHYPERGEOMETRIC_H2PE 0

/******************************************************************************/
/* Poisson */
/* Comments:
// PTPE - if lambda>=27, acceptance/rejection method is used
//        with decomposition onto 4 regions:
//        2 combined parallelograms, triangle, left exponential tail and 
//        right exponential tail;
//        othewise table lookup method is used
// POISNORM - for lambda>=1 method is based on Poisson inverse CDF
//            approximation via Gaussian (normal) inverse CDF; for lambda<1
//            table lookup method is used.
*/
#define VSL_METHOD_IPOISSON_PTPE     0
#define VSL_METHOD_IPOISSON_POISNORM 1

/******************************************************************************/
/* NegBinomial */
/* Comments:
// NBAR - if (a-1)*(1-p)/p>=100, acceptance/rejection method is used with
//        decomposition onto 5 regions: rectangular, 2 trapezoid, left
//        exponential tail, right exponential tail; othewise table lookup
//        method is used.
*/
#define VSL_METHOD_INEGBINOMIAL_NBAR 0

/*******************************************************************************
*********************************           ************************************
*******************************************************************************/
/* Following defines are for user-designed BRNG stream initialization routine */
/* Initialization method */
#define	VSL_INIT_METHOD_STANDARD	0
#define	VSL_INIT_METHOD_LEAPFROG	1
#define	VSL_INIT_METHOD_SKIPAHEAD	2

/* Possible errors in initialization routine */
#define VSL_ERROR_OK                     0
#define VSL_ERROR_LEAPFROG_UNSUPPORTED  -4
#define VSL_ERROR_SKIPAHEAD_UNSUPPORTED -5


#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /* __VSL_DEFINES_H__ */
