/*
//               INTEL CORPORATION PROPRIETARY INFORMATION
//  This software is supplied under the terms of a license agreement or
//  nondisclosure agreement with Intel Corporation and may not be copied
//  or disclosed except in accordance with the terms of that agreement.
//    Copyright (C) 2001-2003 Intel Corporation. All Rights Reserved.
//
// File   : mkl_vsl_functions.h
// Purpose: VSL header (Windows version)
*/

#ifndef __VSL_FUNCTIONS_H__
#define __VSL_FUNCTIONS_H__

#include "mkl_vsl_types.h"

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

#if defined(MKL_VSL_STDCALL)
  #define _VSL_DECL(name,arg)   extern void __stdcall name##arg;
  #define _VSL_DECL_I(name,arg) extern int  __stdcall name##arg;
#else
  #define _VSL_DECL(name,arg)   extern void __cdecl name##arg;
  #define _VSL_DECL_I(name,arg) extern int  __cdecl name##arg;
#endif

/*******************************************************************************
********************************* functions ************************************
*******************************************************************************/

_VSL_DECL(vdRngCauchy,(int  , VSLStreamStatePtr , int  , double [], double  , double  ))
_VSL_DECL(VDRNGCAUCHY,(int *, VSLStreamStatePtr , int *, double [], double *, double *))
_VSL_DECL(vdrngcauchy,(int *, VSLStreamStatePtr , int *, double [], double *, double *))
_VSL_DECL(vsRngCauchy,(int  , VSLStreamStatePtr , int  , float [], float  , float  ))
_VSL_DECL(VSRNGCAUCHY,(int *, VSLStreamStatePtr , int *, float [], float *, float *))
_VSL_DECL(vsrngcauchy,(int *, VSLStreamStatePtr , int *, float [], float *, float *))

_VSL_DECL(vdRngUniform,(int  , VSLStreamStatePtr , int  , double [], double  , double  ))
_VSL_DECL(VDRNGUNIFORM,(int *, VSLStreamStatePtr , int *, double [], double *, double *))
_VSL_DECL(vdrnguniform,(int *, VSLStreamStatePtr , int *, double [], double *, double *))
_VSL_DECL(vsRngUniform,(int  , VSLStreamStatePtr , int  , float [], float  , float  ))
_VSL_DECL(VSRNGUNIFORM,(int *, VSLStreamStatePtr , int *, float [], float *, float *))
_VSL_DECL(vsrnguniform,(int *, VSLStreamStatePtr , int *, float [], float *, float *))

_VSL_DECL(vdRngGaussian,(int  , VSLStreamStatePtr , int  , double [], double  , double  ))
_VSL_DECL(VDRNGGAUSSIAN,(int *, VSLStreamStatePtr , int *, double [], double *, double *))
_VSL_DECL(vdrnggaussian,(int *, VSLStreamStatePtr , int *, double [], double *, double *))
_VSL_DECL(vsRngGaussian,(int  , VSLStreamStatePtr , int  , float [], float  , float  ))
_VSL_DECL(VSRNGGAUSSIAN,(int *, VSLStreamStatePtr , int *, float [], float *, float *))
_VSL_DECL(vsrnggaussian,(int *, VSLStreamStatePtr , int *, float [], float *, float *))

_VSL_DECL(vdRngExponential,(int  , VSLStreamStatePtr , int  , double [], double  , double  ))
_VSL_DECL(VDRNGEXPONENTIAL,(int *, VSLStreamStatePtr , int *, double [], double *, double *))
_VSL_DECL(vdrngexponential,(int *, VSLStreamStatePtr , int *, double [], double *, double *))
_VSL_DECL(vsRngExponential,(int  , VSLStreamStatePtr , int  , float [], float  , float  ))
_VSL_DECL(VSRNGEXPONENTIAL,(int *, VSLStreamStatePtr , int *, float [], float *, float *))
_VSL_DECL(vsrngexponential,(int *, VSLStreamStatePtr , int *, float [], float *, float *))

_VSL_DECL(vdRngLaplace,(int  , VSLStreamStatePtr , int  , double [], double  , double  ))
_VSL_DECL(VDRNGLAPLACE,(int *, VSLStreamStatePtr , int *, double [], double *, double *))
_VSL_DECL(vdrnglaplace,(int *, VSLStreamStatePtr , int *, double [], double *, double *))
_VSL_DECL(vsRngLaplace,(int  , VSLStreamStatePtr , int  , float [], float  , float  ))
_VSL_DECL(VSRNGLAPLACE,(int *, VSLStreamStatePtr , int *, float [], float *, float *))
_VSL_DECL(vsrnglaplace,(int *, VSLStreamStatePtr , int *, float [], float *, float *))

_VSL_DECL(vdRngWeibull,(int  , VSLStreamStatePtr , int  , double [], double  , double  , double  ))
_VSL_DECL(VDRNGWEIBULL,(int *, VSLStreamStatePtr , int *, double [], double *, double *, double *))
_VSL_DECL(vdrngweibull,(int *, VSLStreamStatePtr , int *, double [], double *, double *, double *))
_VSL_DECL(vsRngWeibull,(int  , VSLStreamStatePtr , int  , float [], float  , float  , float  ))
_VSL_DECL(VSRNGWEIBULL,(int *, VSLStreamStatePtr , int *, float [], float *, float *, float *))
_VSL_DECL(vsrngweibull,(int *, VSLStreamStatePtr , int *, float [], float *, float *, float *))

_VSL_DECL(vdRngRayleigh,(int  , VSLStreamStatePtr , int  , double [], double  , double  ))
_VSL_DECL(VDRNGRAYLEIGH,(int *, VSLStreamStatePtr , int *, double [], double *, double *))
_VSL_DECL(vdrngrayleigh,(int *, VSLStreamStatePtr , int *, double [], double *, double *))
_VSL_DECL(vsRngRayleigh,(int  , VSLStreamStatePtr , int  , float [], float  , float  ))
_VSL_DECL(VSRNGRAYLEIGH,(int *, VSLStreamStatePtr , int *, float [], float *, float *))
_VSL_DECL(vsrngrayleigh,(int *, VSLStreamStatePtr , int *, float [], float *, float *))

_VSL_DECL(vdRngLognormal,(int  , VSLStreamStatePtr , int  , double [], double  , double  , double  , double  ))
_VSL_DECL(VDRNGLOGNORMAL,(int *, VSLStreamStatePtr , int *, double [], double *, double *, double *, double *))
_VSL_DECL(vdrnglognormal,(int *, VSLStreamStatePtr , int *, double [], double *, double *, double *, double *))
_VSL_DECL(vsRngLognormal,(int  , VSLStreamStatePtr , int  , float [], float  , float  , float  , float  ))
_VSL_DECL(VSRNGLOGNORMAL,(int *, VSLStreamStatePtr , int *, float [], float *, float *, float *, float *))
_VSL_DECL(vsrnglognormal,(int *, VSLStreamStatePtr , int *, float [], float *, float *, float *, float *))

_VSL_DECL(vdRngGumbel,(int  , VSLStreamStatePtr , int  , double [], double  , double  ))
_VSL_DECL(VDRNGGUMBEL,(int *, VSLStreamStatePtr , int *, double [], double *, double *))
_VSL_DECL(vdrnggumbel,(int *, VSLStreamStatePtr , int *, double [], double *, double *))
_VSL_DECL(vsRngGumbel,(int  , VSLStreamStatePtr , int  , float [], float  , float  ))
_VSL_DECL(VSRNGGUMBEL,(int *, VSLStreamStatePtr , int *, float [], float *, float *))
_VSL_DECL(vsrnggumbel,(int *, VSLStreamStatePtr , int *, float [], float *, float *))

_VSL_DECL(viRngBernoulli,(int  , VSLStreamStatePtr , int  , int [], double  ))
_VSL_DECL(VIRNGBERNOULLI,(int *, VSLStreamStatePtr , int *, int [], double *))
_VSL_DECL(virngbernoulli,(int *, VSLStreamStatePtr , int *, int [], double *))

_VSL_DECL(viRngUniform,(int  , VSLStreamStatePtr , int  , int [], int  , int  ))
_VSL_DECL(VIRNGUNIFORM,(int *, VSLStreamStatePtr , int *, int [], int *, int *))
_VSL_DECL(virnguniform,(int *, VSLStreamStatePtr , int *, int [], int *, int *))

_VSL_DECL(viRngUniformBits,(int  , VSLStreamStatePtr , int  , unsigned int []))
_VSL_DECL(VIRNGUNIFORMBITS,(int *, VSLStreamStatePtr , int *, unsigned int []))
_VSL_DECL(virnguniformbits,(int *, VSLStreamStatePtr , int *, unsigned int []))

_VSL_DECL(viRngGeometric,(int  , VSLStreamStatePtr , int  , int [], double  ))
_VSL_DECL(VIRNGGEOMETRIC,(int *, VSLStreamStatePtr , int *, int [], double *))
_VSL_DECL(virnggeometric,(int *, VSLStreamStatePtr , int *, int [], double *))

_VSL_DECL(viRngBinomial,(int  , VSLStreamStatePtr , int  , int [], int  , double  ))
_VSL_DECL(VIRNGBINOMIAL,(int *, VSLStreamStatePtr , int *, int [], int *, double *))
_VSL_DECL(virngbinomial,(int *, VSLStreamStatePtr , int *, int [], int *, double *))

_VSL_DECL(viRngHypergeometric,(int  , VSLStreamStatePtr , int  , int [], int  , int  , int  ))
_VSL_DECL(VIRNGHYPERGEOMETRIC,(int *, VSLStreamStatePtr , int *, int [], int *, int *, int *))
_VSL_DECL(virnghypergeometric,(int *, VSLStreamStatePtr , int *, int [], int *, int *, int *))

_VSL_DECL(viRngNegbinomial,(int  , VSLStreamStatePtr , int  , int [], double, double ))
_VSL_DECL(VIRNGNEGBINOMIAL,(int *, VSLStreamStatePtr , int *, int [], double *, double *))
_VSL_DECL(virngnegbinomial,(int *, VSLStreamStatePtr , int *, int [], double *, double *))

_VSL_DECL(viRngPoisson,(int  , VSLStreamStatePtr , int  , int [], double  ))
_VSL_DECL(VIRNGPOISSON,(int *, VSLStreamStatePtr , int *, int [], double *))
_VSL_DECL(virngpoisson,(int *, VSLStreamStatePtr , int *, int [], double *))

_VSL_DECL(viRngPoissonV,(int  , VSLStreamStatePtr , int  , int [], double []))
_VSL_DECL(VIRNGPOISSONV,(int *, VSLStreamStatePtr , int *, int [], double []))
_VSL_DECL(virngpoissonv,(int *, VSLStreamStatePtr , int *, int [], double []))

_VSL_DECL(vslNewStream,( VSLStreamStatePtr* , int , unsigned int ))
_VSL_DECL(vslNewStreamEx,( VSLStreamStatePtr* , int , int, const unsigned int[] ))
_VSL_DECL(vslDeleteStream,( VSLStreamStatePtr* ))
_VSL_DECL(vslCopyStream,( VSLStreamStatePtr*, VSLStreamStatePtr ))
_VSL_DECL(vslCopyStreamState,( VSLStreamStatePtr, VSLStreamStatePtr ))
_VSL_DECL(vslLeapfrogStream,( VSLStreamStatePtr, int, int ))
_VSL_DECL(vslSkipAheadStream,( VSLStreamStatePtr, int ))
_VSL_DECL(vslGetBrngProperties,( int, VSLBRngProperties* properties ))
_VSL_DECL_I(vslGetNumRegBrngs, ( void ))
_VSL_DECL_I(vslGetStreamStateBrng, ( VSLStreamStatePtr ))
_VSL_DECL_I(vslRegisterBrng,( const VSLBRngProperties* properties ))

#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /* __VSL_FUNCTIONS_H__ */
