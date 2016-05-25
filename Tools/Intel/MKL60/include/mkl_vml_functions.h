/*
//               INTEL CORPORATION PROPRIETARY INFORMATION
//  This software is supplied under the terms of a license agreement or
//  nondisclosure agreement with Intel Corporation and may not be copied
//  or disclosed except in accordance with the terms of that agreement.
//    Copyright (C) 1996-2003 Intel Corporation. All Rights Reserved.
//
// File   : mkl_vml_functions.h
// Purpose: VML header (Windows version)
*/

#ifndef __VML_FUNCTIONS_H__
#define __VML_FUNCTIONS_H__

#include "mkl_vml_types.h"

#ifdef __cplusplus
extern "C" {
#endif /* __cplusplus */

#if defined(MKL_VML_STDCALL)
#  define __VML_API(_VML_RET_TYPE,_VML_FUNC_NAME,_VML_FUNC_ARG) extern _VML_RET_TYPE __stdcall _VML_FUNC_NAME _VML_FUNC_ARG;
#  define __vml_api(_vml_ret_type,_vml_func_name,_vml_func_arg) extern _vml_ret_type __stdcall _vml_func_name _vml_func_arg;
#  define __Vml_Api(_Vml_Ret_Type,_Vml_Func_Name,_Vml_Func_Arg) extern _Vml_Ret_Type __stdcall _Vml_Func_Name _Vml_Func_Arg;
#else /* MKL_VML_CDECL */
#  define __VML_API(_VML_RET_TYPE,_VML_FUNC_NAME,_VML_FUNC_ARG) extern _VML_RET_TYPE __cdecl _VML_FUNC_NAME _VML_FUNC_ARG;
#  define __vml_api(_vml_ret_type,_vml_func_name,_vml_func_arg) extern _vml_ret_type __cdecl _vml_func_name _vml_func_arg;
#  define __Vml_Api(_Vml_Ret_Type,_Vml_Func_Name,_Vml_Func_Arg) extern _Vml_Ret_Type __cdecl _Vml_Func_Name _Vml_Func_Arg;
#endif

/*******************************************************************************
  Math functions
*******************************************************************************/

/* Inversion: r = 1.0 / a                                                 */
__VML_API(void, VSINV,(int *n, const float  a[], float  r[]))
__VML_API(void, VDINV,(int *n, const double a[], double r[]))
__vml_api(void, vsinv, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdinv, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsInv, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdInv, ( int n, const double a[], double r[] ))

/* Square root: r = a**0.5                                                */
__VML_API(void, VSSQRT, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDSQRT, ( int *n, const double a[], double r[] ))
__vml_api(void, vssqrt, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdsqrt, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsSqrt, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdSqrt, ( int n, const double a[], double r[] ))

/* Inversion Square root: r = 1/a**0.5                                                */
__VML_API(void, VSINVSQRT, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDINVSQRT, ( int *n, const double a[], double r[] ))
__vml_api(void, vsinvsqrt, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdinvsqrt, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsInvSqrt, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdInvSqrt, ( int n, const double a[], double r[] ))

/* Cube root: r = a**0.3(3)                                                */
__VML_API(void, VSCBRT, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDCBRT, ( int *n, const double a[], double r[] ))
__vml_api(void, vscbrt, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdcbrt, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsCbrt, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdCbrt, ( int n, const double a[], double r[] ))

/* Inversion Cube root: r = 1/a**0.3(3)                                                */
__VML_API(void, VSINVCBRT, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDINVCBRT, ( int *n, const double a[], double r[] ))
__vml_api(void, vsinvcbrt, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdinvcbrt, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsInvCbrt, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdInvCbrt, ( int n, const double a[], double r[] ))

/* Exponent: r = e**a                                                     */
__VML_API(void, VSEXP, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDEXP, ( int *n, const double a[], double r[] ))
__vml_api(void, vsexp, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdexp, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsExp, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdExp, ( int n, const double a[], double r[] ))

/* Logarithm: r = ln a                                                    */
__VML_API(void, VSLN, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDLN, ( int *n, const double a[], double r[] ))
__vml_api(void, vsln, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdln, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsLn, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdLn, ( int n, const double a[], double r[] ))

/* Decimal logarithm: r = lg a                                            */
__VML_API(void, VSLOG10, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDLOG10, ( int *n, const double a[], double r[] ))
__vml_api(void, vslog10, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdlog10, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsLog10, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdLog10, ( int n, const double a[], double r[] ))

/* Sine: r = SIN a                                                        */
__VML_API(void, VSSIN, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDSIN, ( int *n, const double a[], double r[] ))
__vml_api(void, vssin, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdsin, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsSin, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdSin, ( int n, const double a[], double r[] ))

/* Cosine: r = COS a                                                      */
__VML_API(void, VSCOS, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDCOS, ( int *n, const double a[], double r[] ))
__vml_api(void, vscos, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdcos, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsCos, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdCos, ( int n, const double a[], double r[] ))

/* Tangent: r = tan a                                                     */
__VML_API(void, VSTAN, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDTAN, ( int *n, const double a[], double r[] ))
__vml_api(void, vstan, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdtan, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsTan, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdTan, ( int n, const double a[], double r[] ))

/* Hyperbolic Sine: r = sh a                                              */
__VML_API(void, VSSINH, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDSINH, ( int *n, const double a[], double r[] ))
__vml_api(void, vssinh, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdsinh, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsSinh, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdSinh, ( int n, const double a[], double r[] ))

/* Hyperbolic Cosine: r = ch a                                            */
__VML_API(void, VSCOSH, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDCOSH, ( int *n, const double a[], double r[] ))
__vml_api(void, vscosh, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdcosh, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsCosh, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdCosh, ( int n, const double a[], double r[] ))

/* Hyperbolic Tangent: r = th a                                           */
__VML_API(void, VSTANH, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDTANH, ( int *n, const double a[], double r[] ))
__vml_api(void, vstanh, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdtanh, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsTanh, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdTanh, ( int n, const double a[], double r[] ))

/* Arc Cosine: r = arcCOS a                                               */
__VML_API(void, VSACOS, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDACOS, ( int *n, const double a[], double r[] ))
__vml_api(void, vsacos, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdacos, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsAcos, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdAcos, ( int n, const double a[], double r[] ))

/* Arc Sine: r = arcSIN a                                                 */
__VML_API(void, VSASIN, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDASIN, ( int *n, const double a[], double r[] ))
__vml_api(void, vsasin, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdasin, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsAsin, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdAsin, ( int n, const double a[], double r[] ))

/* Arc Tangent: r = arctan a                                              */
__VML_API(void, VSATAN, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDATAN, ( int *n, const double a[], double r[] ))
__vml_api(void, vsatan, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdatan, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsAtan, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdAtan, ( int n, const double a[], double r[] ))

/* Hyperbolic Arc Cosine: r = arcCH a                                     */
__VML_API(void, VSACOSH, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDACOSH, ( int *n, const double a[], double r[] ))
__vml_api(void, vsacosh, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdacosh, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsAcosh, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdAcosh, ( int n, const double a[], double r[] ))

/* Hyperbolic Arc Sine: r = arcSH a                                       */
__VML_API(void, VSASINH, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDASINH, ( int *n, const double a[], double r[] ))
__vml_api(void, vsasinh, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdasinh, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsAsinh, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdAsinh, ( int n, const double a[], double r[] ))

/* Hyperbolic Arc Tangent: r = arcTH a                                    */
__VML_API(void, VSATANH, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDATANH, ( int *n, const double a[], double r[] ))
__vml_api(void, vsatanh, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vdatanh, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsAtanh, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdAtanh, ( int n, const double a[], double r[] ))

/*  Error function: r = erf a                                             */
__VML_API(void, VSERF, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDERF, ( int *n, const double a[], double r[] ))
__vml_api(void, vserf, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vderf, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsErf, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdErf, ( int n, const double a[], double r[] ))

/*  Error function: r = 1 - erf a                                         */
__VML_API(void, VSERFC, ( int *n, const float  a[], float  r[] ))
__VML_API(void, VDERFC, ( int *n, const double a[], double r[] ))
__vml_api(void, vserfc, ( int *n, const float  a[], float  r[] ))
__vml_api(void, vderfc, ( int *n, const double a[], double r[] ))
__Vml_Api(void, vsErfc, ( int n, const float  a[], float  r[] ))
__Vml_Api(void, vdErfc, ( int n, const double a[], double r[] ))

/* Arc Tangent of a/b: r = arctan a / b                                   */
__VML_API(void, VSATAN2, ( int *n, const float  a[], const float  b[], float  r[] ))
__VML_API(void, VDATAN2, ( int *n, const double a[], const double b[], double r[] ))
__vml_api(void, vsatan2, ( int *n, const float  a[], const float  b[], float  r[] ))
__vml_api(void, vdatan2, ( int *n, const double a[], const double b[], double r[] ))
__Vml_Api(void, vsAtan2, ( int n, const float  a[], const float  b[], float  r[] ))
__Vml_Api(void, vdAtan2, ( int n, const double a[], const double b[], double r[] ))

/* Division: r = a / b                                                    */
__VML_API(void, VSDIV, ( int *n, const float  a[], const float  b[], float  r[] ))
__VML_API(void, VDDIV, ( int *n, const double a[], const double b[], double r[] ))
__vml_api(void, vsdiv, ( int *n, const float  a[], const float  b[], float  r[] ))
__vml_api(void, vddiv, ( int *n, const double a[], const double b[], double r[] ))
__Vml_Api(void, vsDiv, ( int n, const float  a[], const float  b[], float  r[] ))
__Vml_Api(void, vdDiv, ( int n, const double a[], const double b[], double r[] ))

/* Power: r = a**b                                                        */
__VML_API(void, VSPOW, ( int *n, const float  a[], const float  b[], float  r[] ))
__VML_API(void, VDPOW, ( int *n, const double a[], const double b[], double r[] ))
__vml_api(void, vspow, ( int *n, const float  a[], const float  b[], float  r[] ))
__vml_api(void, vdpow, ( int *n, const double a[], const double b[], double r[] ))
__Vml_Api(void, vsPow, ( int n, const float  a[], const float  b[], float  r[] ))
__Vml_Api(void, vdPow, ( int n, const double a[], const double b[], double r[] ))

/* "Scalar" Power: r = a**b, b - scalar                                   */
__VML_API(void, VSPOWX, ( int *n, const float  a[], const float  *b, float  r[] ))
__VML_API(void, VDPOWX, ( int *n, const double a[], const double *b, double r[] ))
__vml_api(void, vspowx, ( int *n, const float  a[], const float  *b, float  r[] ))
__vml_api(void, vdpowx, ( int *n, const double a[], const double *b, double r[] ))
__Vml_Api(void, vsPowx, ( int n, const float  a[], const float  b, float  r[] ))
__Vml_Api(void, vdPowx, ( int n, const double a[], const double b, double r[] ))

/* Sine & Cosine: r = SIN a r1=COS a                                      */
__VML_API(void, VSSINCOS, ( int *n, const float  a[], float  r1[], float  r2[] ))
__VML_API(void, VDSINCOS, ( int *n, const double a[], double r1[], double r2[] ))
__vml_api(void, vssincos, ( int *n, const float  a[], float  r1[], float  r2[] ))
__vml_api(void, vdsincos, ( int *n, const double a[], double r1[], double r2[] ))
__Vml_Api(void, vsSinCos, ( int n, const float  a[], float  r1[], float  r2[] ))
__Vml_Api(void, vdSinCos, ( int n, const double a[], double r1[], double r2[] ))

/*******************************************************************************
  Pack/Unpack functions
*******************************************************************************/

__VML_API(void, VSPACKI, ( int *n, const float  a[], int * incra, float  y[]    ))
__VML_API(void, VDPACKI, ( int *n, const double a[], int * incra, double y[]    ))
__VML_API(void, VSPACKV, ( int *n, const float  a[], const int ia[], float  y[] ))
__VML_API(void, VDPACKV, ( int *n, const double a[], const int ia[], double y[] ))
__VML_API(void, VSPACKM, ( int *n, const float  a[], const int ma[], float  y[] ))
__VML_API(void, VDPACKM, ( int *n, const double a[], const int ma[], double y[] ))
__vml_api(void, vspacki, ( int *n, const float  a[], int * incra, float  y[]    ))
__vml_api(void, vdpacki, ( int *n, const double a[], int * incra, double y[]    ))
__vml_api(void, vspackv, ( int *n, const float  a[], const int ia[], float  y[] ))
__vml_api(void, vdpackv, ( int *n, const double a[], const int ia[], double y[] ))
__vml_api(void, vspackm, ( int *n, const float  a[], const int ma[], float  y[] ))
__vml_api(void, vdpackm, ( int *n, const double a[], const int ma[], double y[] ))
__Vml_Api(void, vsPackI, ( int n, const float  a[], int incra, float  y[]    ))
__Vml_Api(void, vdPackI, ( int n, const double a[], int incra, double y[]    ))
__Vml_Api(void, vsPackV, ( int n, const float  a[], const int ia[], float  y[] ))
__Vml_Api(void, vdPackV, ( int n, const double a[], const int ia[], double y[] ))
__Vml_Api(void, vsPackM, ( int n, const float  a[], const int ma[], float  y[] ))
__Vml_Api(void, vdPackM, ( int n, const double a[], const int ma[], double y[] ))

__VML_API(void, VSUNPACKI, ( int *n, const float  a[], float  y[], int * incry    ))
__VML_API(void, VDUNPACKI, ( int *n, const double a[], double y[], int * incry    ))
__VML_API(void, VSUNPACKV, ( int *n, const float  a[], float  y[], const int iy[] ))
__VML_API(void, VDUNPACKV, ( int *n, const double a[], double y[], const int iy[] ))
__VML_API(void, VSUNPACKM, ( int *n, const float  a[], float  y[], const int my[] ))
__VML_API(void, VDUNPACKM, ( int *n, const double a[], double y[], const int my[] ))
__vml_api(void, vsunpacki, ( int *n, const float  a[], float  y[], int * incry    ))
__vml_api(void, vdunpacki, ( int *n, const double a[], double y[], int * incry    ))
__vml_api(void, vsunpackv, ( int *n, const float  a[], float  y[], const int iy[] ))
__vml_api(void, vdunpackv, ( int *n, const double a[], double y[], const int iy[] ))
__vml_api(void, vsunpackm, ( int *n, const float  a[], float  y[], const int my[] ))
__vml_api(void, vdunpackm, ( int *n, const double a[], double y[], const int my[] ))
__Vml_Api(void, vsUnpackI, ( int n, const float  a[], float  y[], int incry    ))
__Vml_Api(void, vdUnpackI, ( int n, const double a[], double y[], int incry    ))
__Vml_Api(void, vsUnpackV, ( int n, const float  a[], float  y[], const int iy[] ))
__Vml_Api(void, vdUnpackV, ( int n, const double a[], double y[], const int iy[] ))
__Vml_Api(void, vsUnpackM, ( int n, const float  a[], float  y[], const int my[] ))
__Vml_Api(void, vdUnpackM, ( int n, const double a[], double y[], const int my[] ))


/*******************************************************************************
  Errors functions
*******************************************************************************/

__VML_API(unsigned int,VMLSETERRSTATUS,(unsigned int * status))
__vml_api(unsigned int,vmlseterrstatus,(unsigned int * status))
__Vml_Api(unsigned int,vmlSetErrStatus,(unsigned int status))

__VML_API(unsigned int, VMLGETERRSTATUS, (void))
__vml_api(unsigned int, vmlgeterrstatus, (void))
__Vml_Api(unsigned int, vmlGetErrStatus, (void))

__VML_API(unsigned int, VMLCLEARERRSTATUS, (void))
__vml_api(unsigned int, vmlclearerrstatus, (void))
__Vml_Api(unsigned int, vmlClearErrStatus, (void))

__VML_API(VMLErrorCallBack, VMLSETERRORCALLBACK, (VMLErrorCallBack func))
__vml_api(VMLErrorCallBack, vmlseterrorcallback, (VMLErrorCallBack func))
__Vml_Api(VMLErrorCallBack, vmlSetErrorCallBack, (VMLErrorCallBack func))

__VML_API(VMLErrorCallBack, VMLGETERRORCALLBACK, (void))
__vml_api(VMLErrorCallBack, vmlgeterrorcallback, (void))
__Vml_Api(VMLErrorCallBack, vmlGetErrorCallBack, (void))

__VML_API(VMLErrorCallBack, VMLCLEARERRORCALLBACK, (void))
__vml_api(VMLErrorCallBack, vmlclearerrorcallback, (void))
__Vml_Api(VMLErrorCallBack, vmlClearErrorCallBack, (void))


/*******************************************************************************
  Mode functions
*******************************************************************************/

__VML_API(unsigned int, VMLSETMODE, (unsigned int *newmode))
__vml_api(unsigned int, vmlsetmode, (unsigned int *newmode))
__Vml_Api(unsigned int, vmlSetMode, (unsigned int newmode))

__VML_API(unsigned int, VMLGETMODE, (void))
__vml_api(unsigned int, vmlgetmode, (void))
__Vml_Api(unsigned int, vmlGetMode, (void))


#ifdef __cplusplus
}
#endif /* __cplusplus */

#endif /* __VML_FUNCTIONS_H__ */
