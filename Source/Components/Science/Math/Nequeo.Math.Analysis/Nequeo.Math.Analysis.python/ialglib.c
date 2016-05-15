/**************************************************************************
ALGLIB 3.10.0 (source code generated 2015-08-19)
Copyright (c) Sergey Bochkanov (ALGLIB project).

>>> SOURCE LICENSE >>>
This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation (www.fsf.org); either version 2 of the 
License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

A copy of the GNU General Public License is available at
http://www.fsf.org/licensing/licenses
>>> END OF LICENSE >>>
**************************************************************************/
#include "stdafx.h"
#include "ialglib.h"

/*$ Declarations $*/
#define alglib_simd_alignment 16

#define alglib_r_block        32
#define alglib_half_r_block   16
#define alglib_twice_r_block  64

#define alglib_c_block        24
#define alglib_half_c_block   12
#define alglib_twice_c_block  48

/*$ Body $*/
/********************************************************************
This subroutine calculates fast 32x32 real matrix-vector product:

    y := beta*y + alpha*A*x

using either generic C code or native optimizations (if available)

IMPORTANT:
* A must be stored in row-major order,
  stride is alglib_r_block,
  aligned on alglib_simd_alignment boundary
* X must be aligned on alglib_simd_alignment boundary
* Y may be non-aligned
********************************************************************/
void _ialglib_mv_32(const double *a, const double *x, double *y, ae_int_t stride, double alpha, double beta)
{
    ae_int_t i, k;
    const double *pa0, *pa1, *pb;

    pa0 = a;
    pa1 = a+alglib_r_block;
    pb = x;
    for(i=0; i<16; i++)
    {
        double v0 = 0, v1 = 0;
        for(k=0; k<4; k++)
        {
            v0 += pa0[0]*pb[0];
            v1 += pa1[0]*pb[0];
            v0 += pa0[1]*pb[1];
            v1 += pa1[1]*pb[1];
            v0 += pa0[2]*pb[2];
            v1 += pa1[2]*pb[2];
            v0 += pa0[3]*pb[3];
            v1 += pa1[3]*pb[3];
            v0 += pa0[4]*pb[4];
            v1 += pa1[4]*pb[4];
            v0 += pa0[5]*pb[5];
            v1 += pa1[5]*pb[5];
            v0 += pa0[6]*pb[6];
            v1 += pa1[6]*pb[6];
            v0 += pa0[7]*pb[7];
            v1 += pa1[7]*pb[7];
            pa0 += 8;
            pa1 += 8;
            pb  += 8;
        }
        y[0] = beta*y[0]+alpha*v0;
        y[stride] = beta*y[stride]+alpha*v1;

        /*
         * now we've processed rows I and I+1,
         * pa0 and pa1 are pointing to rows I+1 and I+2.
         * move to I+2 and I+3.
         */
        pa0 += alglib_r_block;
        pa1 += alglib_r_block;
        pb = x;
        y+=2*stride;
    }
}


/*************************************************************************
This function calculates MxN real matrix-vector product:

    y := beta*y + alpha*A*x

using generic C code. It calls _ialglib_mv_32 if both M=32 and N=32.

If beta is zero, we do not use previous values of y (they are  overwritten
by alpha*A*x without ever being read).  If alpha is zero, no matrix-vector
product is calculated (only beta is updated); however, this update  is not
efficient  and  this  function  should  NOT  be used for multiplication of 
vector and scalar.

IMPORTANT:
* 0<=M<=alglib_r_block, 0<=N<=alglib_r_block
* A must be stored in row-major order with stride equal to alglib_r_block
*************************************************************************/
void _ialglib_rmv(ae_int_t m, ae_int_t n, const double *a, const double *x, double *y, ae_int_t stride, double alpha, double beta)
{
    /*
     * Handle special cases:
     * - alpha is zero or n is zero
     * - m is zero
     */
    if( m==0 )
        return;
    if( alpha==0.0 || n==0 )
    {
        ae_int_t i;
        if( beta==0.0 )
        {
            for(i=0; i<m; i++)
            {
                *y = 0.0;
                y += stride;
            }
        }
        else
        {
            for(i=0; i<m; i++)
            {
                *y *= beta;
                y += stride;
            }
        }
        return;
    }
    
    /*
     * Handle general case: nonzero alpha, n and m
     *
     */
    if( m==32 && n==32 )
    {
        /*
         * 32x32, may be we have something better than general implementation
         */
        _ialglib_mv_32(a, x, y, stride, alpha, beta);
    }
    else
    {
        ae_int_t i, k, m2, n8, n2, ntrail2;
        const double *pa0, *pa1, *pb;

        /*
         * First M/2 rows of A are processed in pairs.
         * optimized code is used.
         */
        m2 = m/2;
        n8 = n/8;
        ntrail2 = (n-8*n8)/2;
        for(i=0; i<m2; i++)
        {
            double v0 = 0, v1 = 0;

            /*
             * 'a' points to the part of the matrix which
             * is not processed yet
             */
            pb = x;
            pa0 = a;
            pa1 = a+alglib_r_block;
            a += alglib_twice_r_block;

            /*
             * 8 elements per iteration
             */
            for(k=0; k<n8; k++)
            {
                v0 += pa0[0]*pb[0];
                v1 += pa1[0]*pb[0];
                v0 += pa0[1]*pb[1];
                v1 += pa1[1]*pb[1];
                v0 += pa0[2]*pb[2];
                v1 += pa1[2]*pb[2];
                v0 += pa0[3]*pb[3];
                v1 += pa1[3]*pb[3];
                v0 += pa0[4]*pb[4];
                v1 += pa1[4]*pb[4];
                v0 += pa0[5]*pb[5];
                v1 += pa1[5]*pb[5];
                v0 += pa0[6]*pb[6];
                v1 += pa1[6]*pb[6];
                v0 += pa0[7]*pb[7];
                v1 += pa1[7]*pb[7];
                pa0 += 8;
                pa1 += 8;
                pb  += 8;
            }

            /*
             * 2 elements per iteration
             */
            for(k=0; k<ntrail2; k++)
            {
                v0 += pa0[0]*pb[0];
                v1 += pa1[0]*pb[0];
                v0 += pa0[1]*pb[1];
                v1 += pa1[1]*pb[1];
                pa0 += 2;
                pa1 += 2;
                pb  += 2;
            }

            /*
             * last element, if needed
             */
            if( n%2!=0 )
            {
                v0 += pa0[0]*pb[0];
                v1 += pa1[0]*pb[0];
            }

            /*
             * final update
             */
            if( beta!=0 )
            {
                y[0] = beta*y[0]+alpha*v0;
                y[stride] = beta*y[stride]+alpha*v1;
            }
            else
            {
                y[0] = alpha*v0;
                y[stride] = alpha*v1;
            }
            
            /*
             * move to the next pair of elements
             */
            y+=2*stride;
        }


        /*
         * Last (odd) row is processed with less optimized code.
         */
        if( m%2!=0 )
        {
            double v0 = 0;

            /*
             * 'a' points to the part of the matrix which
             * is not processed yet
             */
            pb = x;
            pa0 = a;

            /*
             * 2 elements per iteration
             */
            n2 = n/2;
            for(k=0; k<n2; k++)
            {
                v0 += pa0[0]*pb[0]+pa0[1]*pb[1];
                pa0 += 2;
                pb  += 2;
            }

            /*
             * last element, if needed
             */
            if( n%2!=0 )
                v0 += pa0[0]*pb[0];

            /*
             * final update
             */
            if( beta!=0 )
                y[0] = beta*y[0]+alpha*v0;
            else
                y[0] = alpha*v0;
        }
    }
}


/*************************************************************************
This function calculates MxN real matrix-vector product:

    y := beta*y + alpha*A*x

using generic C code. It calls _ialglib_mv_32 if both M=32 and N=32.

If beta is zero, we do not use previous values of y (they are  overwritten
by alpha*A*x without ever being read).  If alpha is zero, no matrix-vector
product is calculated (only beta is updated); however, this update  is not
efficient  and  this  function  should  NOT  be used for multiplication of 
vector and scalar.

IMPORTANT:
* 0<=M<=alglib_r_block, 0<=N<=alglib_r_block
* A must be stored in row-major order with stride equal to alglib_r_block
* y may be non-aligned
* both A and x must have same offset with respect to 16-byte boundary:
  either both are aligned, or both are aligned with offset 8. Function
  will crash your system if you try to call it with misaligned or 
  incorrectly aligned data.

This function supports SSE2; it can be used when:
1. AE_HAS_SSE2_INTRINSICS was defined (checked at compile-time)
2. ae_cpuid() result contains CPU_SSE2 (checked at run-time)

If (1) is failed, this function will be undefined. If (2) is failed,  call 
to this function will probably crash your system. 

If  you  want  to  know  whether  it  is safe to call it, you should check 
results  of  ae_cpuid(). If CPU_SSE2 bit is set, this function is callable 
and will do its work.
*************************************************************************/
#if defined(AE_HAS_SSE2_INTRINSICS)
void _ialglib_rmv_sse2(ae_int_t m, ae_int_t n, const double *a, const double *x, double *y, ae_int_t stride, double alpha, double beta)
{
    ae_int_t i, k, n2;
    ae_int_t mb3, mtail, nhead, nb8, nb2, ntail;
    const double *pa0, *pa1, *pa2, *pb;
    __m128d v0, v1, v2, va0, va1, va2, vx, vtmp;
    
    /*
     * Handle special cases:
     * - alpha is zero or n is zero
     * - m is zero
     */
    if( m==0 )
        return;
    if( alpha==0.0 || n==0 )
    {
        if( beta==0.0 )
        {
            for(i=0; i<m; i++)
            {
                *y = 0.0;
                y += stride;
            }
        }
        else
        {
            for(i=0; i<m; i++)
            {
                *y *= beta;
                y += stride;
            }
        }
        return;
    }
    
    /*
     * Handle general case: nonzero alpha, n and m
     *
     * We divide problem as follows...
     *
     * Rows M are divided into:
     * - mb3 blocks, each 3xN
     * - mtail blocks, each 1xN
     *
     * Within a row, elements are divided  into:
     * - nhead 1x1 blocks (used to align the rest, either 0 or 1)
     * - nb8 1x8 blocks, aligned to 16-byte boundary
     * - nb2 1x2 blocks, aligned to 16-byte boundary
     * - ntail 1x1 blocks, aligned too (altough we don't rely on it)
     *
     */
    n2 = n/2;    
    mb3 = m/3;
    mtail = m%3;
    nhead = ae_misalignment(a,alglib_simd_alignment)==0 ? 0 : 1;
    nb8 = (n-nhead)/8;
    nb2 = (n-nhead-8*nb8)/2;
    ntail = n-nhead-8*nb8-2*nb2;
    for(i=0; i<mb3; i++)
    {
        double row0, row1, row2;
        row0 = 0;
        row1 = 0;
        row2 = 0;
        pb = x;
        pa0 = a;
        pa1 = a+alglib_r_block;
        pa2 = a+alglib_twice_r_block;
        a += 3*alglib_r_block;
        if( nhead==1 )
        {
            vx  = _mm_load_sd(pb);
            v0 = _mm_load_sd(pa0);
            v1 = _mm_load_sd(pa1);
            v2 = _mm_load_sd(pa2);
            
            v0 = _mm_mul_sd(v0,vx);
            v1 = _mm_mul_sd(v1,vx);
            v2 = _mm_mul_sd(v2,vx);
            
            pa0++;
            pa1++;
            pa2++;
            pb++;
        }
        else
        {
            v0 = _mm_setzero_pd();
            v1 = _mm_setzero_pd();
            v2 = _mm_setzero_pd();
        }
        for(k=0; k<nb8; k++)
        {
            /*
             * this code is a shuffle of simultaneous dot product.
             * see below for commented unshuffled original version.
             */
            vx  = _mm_load_pd(pb);
            va0 = _mm_load_pd(pa0);
            va1 = _mm_load_pd(pa1);
            va0 = _mm_mul_pd(va0,vx);
            va2 = _mm_load_pd(pa2);
            v0 = _mm_add_pd(va0,v0);
            va1 = _mm_mul_pd(va1,vx);
            va0 = _mm_load_pd(pa0+2);
            v1 = _mm_add_pd(va1,v1);
            va2 = _mm_mul_pd(va2,vx);
            va1 = _mm_load_pd(pa1+2);
            v2 = _mm_add_pd(va2,v2);
            vx  = _mm_load_pd(pb+2);
            va0 = _mm_mul_pd(va0,vx);
            va2 = _mm_load_pd(pa2+2);
            v0 = _mm_add_pd(va0,v0);
            va1 = _mm_mul_pd(va1,vx);
            va0 = _mm_load_pd(pa0+4);
            v1 = _mm_add_pd(va1,v1);
            va2 = _mm_mul_pd(va2,vx);
            va1 = _mm_load_pd(pa1+4);
            v2 = _mm_add_pd(va2,v2);
            vx  = _mm_load_pd(pb+4);
            va0 = _mm_mul_pd(va0,vx);
            va2 = _mm_load_pd(pa2+4);
            v0 = _mm_add_pd(va0,v0);
            va1 = _mm_mul_pd(va1,vx);
            va0 = _mm_load_pd(pa0+6);
            v1 = _mm_add_pd(va1,v1);
            va2 = _mm_mul_pd(va2,vx);
            va1 = _mm_load_pd(pa1+6);
            v2 = _mm_add_pd(va2,v2);
            vx  = _mm_load_pd(pb+6);
            va0 = _mm_mul_pd(va0,vx);
            v0 = _mm_add_pd(va0,v0);
            va2 = _mm_load_pd(pa2+6);
            va1 = _mm_mul_pd(va1,vx);
            v1 = _mm_add_pd(va1,v1);
            va2 = _mm_mul_pd(va2,vx);
            v2 = _mm_add_pd(va2,v2);
            
            pa0 += 8;
            pa1 += 8;
            pa2 += 8;
            pb += 8;

            /*
            this is unshuffled version of code above
            
            vx  = _mm_load_pd(pb);
            va0 = _mm_load_pd(pa0);            
            va1 = _mm_load_pd(pa1);
            va2 = _mm_load_pd(pa2);
            
            va0 = _mm_mul_pd(va0,vx);
            va1 = _mm_mul_pd(va1,vx);
            va2 = _mm_mul_pd(va2,vx);
            
            v0 = _mm_add_pd(va0,v0);
            v1 = _mm_add_pd(va1,v1);
            v2 = _mm_add_pd(va2,v2);
            
            vx  = _mm_load_pd(pb+2);
            va0 = _mm_load_pd(pa0+2);            
            va1 = _mm_load_pd(pa1+2);
            va2 = _mm_load_pd(pa2+2);
            
            va0 = _mm_mul_pd(va0,vx);
            va1 = _mm_mul_pd(va1,vx);
            va2 = _mm_mul_pd(va2,vx);
            
            v0 = _mm_add_pd(va0,v0);
            v1 = _mm_add_pd(va1,v1);
            v2 = _mm_add_pd(va2,v2);

            vx  = _mm_load_pd(pb+4);
            va0 = _mm_load_pd(pa0+4);            
            va1 = _mm_load_pd(pa1+4);
            va2 = _mm_load_pd(pa2+4);
            
            va0 = _mm_mul_pd(va0,vx);
            va1 = _mm_mul_pd(va1,vx);
            va2 = _mm_mul_pd(va2,vx);
            
            v0 = _mm_add_pd(va0,v0);
            v1 = _mm_add_pd(va1,v1);
            v2 = _mm_add_pd(va2,v2);

            vx  = _mm_load_pd(pb+6);
            va0 = _mm_load_pd(pa0+6);
            va1 = _mm_load_pd(pa1+6);
            va2 = _mm_load_pd(pa2+6);
            
            va0 = _mm_mul_pd(va0,vx);
            va1 = _mm_mul_pd(va1,vx);
            va2 = _mm_mul_pd(va2,vx);
            
            v0 = _mm_add_pd(va0,v0);
            v1 = _mm_add_pd(va1,v1);
            v2 = _mm_add_pd(va2,v2);
            */
        }
        for(k=0; k<nb2; k++)
        {
            vx  = _mm_load_pd(pb);
            va0 = _mm_load_pd(pa0);
            va1 = _mm_load_pd(pa1);
            va2 = _mm_load_pd(pa2);
            
            va0 = _mm_mul_pd(va0,vx);
            v0 = _mm_add_pd(va0,v0);
            va1 = _mm_mul_pd(va1,vx);
            v1 = _mm_add_pd(va1,v1);
            va2 = _mm_mul_pd(va2,vx);
            v2 = _mm_add_pd(va2,v2);
            
            pa0 += 2;
            pa1 += 2;
            pa2 += 2;
            pb += 2;
        }
        for(k=0; k<ntail; k++)
        {
            vx  = _mm_load1_pd(pb);
            va0 = _mm_load1_pd(pa0);
            va1 = _mm_load1_pd(pa1);
            va2 = _mm_load1_pd(pa2);
            
            va0 = _mm_mul_sd(va0,vx);
            v0 = _mm_add_sd(v0,va0);
            va1 = _mm_mul_sd(va1,vx);
            v1 = _mm_add_sd(v1,va1);
            va2 = _mm_mul_sd(va2,vx);
            v2 = _mm_add_sd(v2,va2);
        }        
        vtmp = _mm_add_pd(_mm_unpacklo_pd(v0,v1),_mm_unpackhi_pd(v0,v1));
        _mm_storel_pd(&row0, vtmp);
        _mm_storeh_pd(&row1, vtmp);
        v2 = _mm_add_sd(_mm_shuffle_pd(v2,v2,1),v2);
        _mm_storel_pd(&row2, v2);
        if( beta!=0 )
        {
            y[0] = beta*y[0]+alpha*row0;
            y[stride] = beta*y[stride]+alpha*row1;
            y[2*stride] = beta*y[2*stride]+alpha*row2;
        }
        else
        {
            y[0] = alpha*row0;
            y[stride] = alpha*row1;
            y[2*stride] = alpha*row2;
        }
        y+=3*stride;
    }
    for(i=0; i<mtail; i++)
    {
        double row0;
        row0 = 0;
        pb = x;
        pa0 = a;
        a += alglib_r_block;
        for(k=0; k<n2; k++)
        {
            row0 += pb[0]*pa0[0]+pb[1]*pa0[1];            
            pa0 += 2;
            pb += 2;
        }
        if( n%2 )
            row0 += pb[0]*pa0[0];
        if( beta!=0 )
            y[0] = beta*y[0]+alpha*row0;
        else
            y[0] = alpha*row0;
        y+=stride;
    }
}
#endif


/*************************************************************************
This subroutine calculates fast MxN complex matrix-vector product:

    y := beta*y + alpha*A*x

using generic C code, where A, x, y, alpha and beta are complex.

If beta is zero, we do not use previous values of y (they are  overwritten
by alpha*A*x without ever being read). However, when  alpha  is  zero,  we 
still calculate A*x and  multiply  it  by  alpha  (this distinction can be
important when A or x contain infinities/NANs).

IMPORTANT:
* 0<=M<=alglib_c_block, 0<=N<=alglib_c_block
* A must be stored in row-major order, as sequence of double precision
  pairs. Stride is alglib_c_block (it is measured in pairs of doubles, not
  in doubles).
* Y may be referenced by cy (pointer to ae_complex) or
  dy (pointer to array of double precision pair) depending on what type of 
  output you wish. Pass pointer to Y as one of these parameters,
  AND SET OTHER PARAMETER TO NULL.
* both A and x must be aligned; y may be non-aligned.
*************************************************************************/
void _ialglib_cmv(ae_int_t m, ae_int_t n, const double *a, const double *x, ae_complex *cy, double *dy, ae_int_t stride, ae_complex alpha, ae_complex beta)
{
    ae_int_t i, j;
    const double *pa, *parow, *pb;

    parow = a;
    for(i=0; i<m; i++)
    {
        double v0 = 0, v1 = 0;
        pa = parow;
        pb = x;
        for(j=0; j<n; j++)
        {
            v0 += pa[0]*pb[0];
            v1 += pa[0]*pb[1];
            v0 -= pa[1]*pb[1];
            v1 += pa[1]*pb[0];

            pa  += 2;
            pb  += 2;
        }
        if( cy!=NULL )
        {
            double tx = (beta.x*cy->x-beta.y*cy->y)+(alpha.x*v0-alpha.y*v1);
            double ty = (beta.x*cy->y+beta.y*cy->x)+(alpha.x*v1+alpha.y*v0);
            cy->x = tx;
            cy->y = ty;
            cy+=stride;
        }
        else
        {
            double tx = (beta.x*dy[0]-beta.y*dy[1])+(alpha.x*v0-alpha.y*v1);
            double ty = (beta.x*dy[1]+beta.y*dy[0])+(alpha.x*v1+alpha.y*v0);
            dy[0] = tx;
            dy[1] = ty;
            dy += 2*stride;
        }
        parow += 2*alglib_c_block;
    }
}


/*************************************************************************
This subroutine calculates fast MxN complex matrix-vector product:

    y := beta*y + alpha*A*x

using generic C code, where A, x, y, alpha and beta are complex.

If beta is zero, we do not use previous values of y (they are  overwritten
by alpha*A*x without ever being read). However, when  alpha  is  zero,  we 
still calculate A*x and  multiply  it  by  alpha  (this distinction can be
important when A or x contain infinities/NANs).

IMPORTANT:
* 0<=M<=alglib_c_block, 0<=N<=alglib_c_block
* A must be stored in row-major order, as sequence of double precision
  pairs. Stride is alglib_c_block (it is measured in pairs of doubles, not
  in doubles).
* Y may be referenced by cy (pointer to ae_complex) or
  dy (pointer to array of double precision pair) depending on what type of 
  output you wish. Pass pointer to Y as one of these parameters,
  AND SET OTHER PARAMETER TO NULL.
* both A and x must be aligned; y may be non-aligned.

This function supports SSE2; it can be used when:
1. AE_HAS_SSE2_INTRINSICS was defined (checked at compile-time)
2. ae_cpuid() result contains CPU_SSE2 (checked at run-time)

If (1) is failed, this function will be undefined. If (2) is failed,  call 
to this function will probably crash your system. 

If  you  want  to  know  whether  it  is safe to call it, you should check 
results  of  ae_cpuid(). If CPU_SSE2 bit is set, this function is callable 
and will do its work.
*************************************************************************/
#if defined(AE_HAS_SSE2_INTRINSICS)
void _ialglib_cmv_sse2(ae_int_t m, ae_int_t n, const double *a, const double *x, ae_complex *cy, double *dy, ae_int_t stride, ae_complex alpha, ae_complex beta)
{
    ae_int_t i, j, m2;
    const double *pa0, *pa1, *parow, *pb;
    __m128d vbeta, vbetax, vbetay;
    __m128d valpha, valphax, valphay;
    
    m2 = m/2;
    parow = a;
    if( cy!=NULL )
    {
        dy = (double*)cy;
        cy = NULL;
    }
    vbeta = _mm_loadh_pd(_mm_load_sd(&beta.x),&beta.y);
    vbetax = _mm_unpacklo_pd(vbeta,vbeta);
    vbetay = _mm_unpackhi_pd(vbeta,vbeta);
    valpha = _mm_loadh_pd(_mm_load_sd(&alpha.x),&alpha.y);
    valphax = _mm_unpacklo_pd(valpha,valpha);
    valphay = _mm_unpackhi_pd(valpha,valpha);
    for(i=0; i<m2; i++)
    {
        __m128d vx, vy, vt0, vt1, vt2, vt3, vt4, vt5, vrx, vry, vtx, vty;
        pa0 = parow;
        pa1 = parow+2*alglib_c_block;
        pb = x;
        vx = _mm_setzero_pd();
        vy = _mm_setzero_pd();
        for(j=0; j<n; j++)
        {
            vt0 = _mm_load1_pd(pb);
            vt1 = _mm_load1_pd(pb+1);
            vt2 = _mm_load_pd(pa0);
            vt3 = _mm_load_pd(pa1);
            vt5 = _mm_unpacklo_pd(vt2,vt3);
            vt4 = _mm_unpackhi_pd(vt2,vt3);
            vt2 = vt5;
            vt3 = vt4;
            
            vt2 = _mm_mul_pd(vt2,vt0);
            vx = _mm_add_pd(vx,vt2);
            vt3 = _mm_mul_pd(vt3,vt1);
            vx = _mm_sub_pd(vx,vt3);
            vt4 = _mm_mul_pd(vt4,vt0);
            vy = _mm_add_pd(vy,vt4);
            vt5 = _mm_mul_pd(vt5,vt1);
            vy = _mm_add_pd(vy,vt5);
            
            pa0 += 2;
            pa1 += 2;
            pb  += 2;
        }
        if( beta.x==0.0 && beta.y==0.0 )
        {
            vrx = _mm_setzero_pd();
            vry = _mm_setzero_pd();
        }
        else
        {
            vtx = _mm_loadh_pd(_mm_load_sd(dy+0),dy+2*stride+0);
            vty = _mm_loadh_pd(_mm_load_sd(dy+1),dy+2*stride+1);
            vrx = _mm_sub_pd(_mm_mul_pd(vbetax,vtx),_mm_mul_pd(vbetay,vty));
            vry = _mm_add_pd(_mm_mul_pd(vbetax,vty),_mm_mul_pd(vbetay,vtx));
        }
        vtx = _mm_sub_pd(_mm_mul_pd(valphax,vx),_mm_mul_pd(valphay,vy));
        vty = _mm_add_pd(_mm_mul_pd(valphax,vy),_mm_mul_pd(valphay,vx));
        vrx = _mm_add_pd(vrx,vtx);
        vry = _mm_add_pd(vry,vty);
        _mm_storel_pd(dy+0,          vrx);
        _mm_storeh_pd(dy+2*stride+0, vrx);
        _mm_storel_pd(dy+1,          vry);
        _mm_storeh_pd(dy+2*stride+1, vry);
        dy += 4*stride;        
        parow += 4*alglib_c_block;
    }
    if( m%2 )
    {
        double v0 = 0, v1 = 0;
        double tx, ty;
        pa0 = parow;
        pb = x;
        for(j=0; j<n; j++)
        {
            v0 += pa0[0]*pb[0];
            v1 += pa0[0]*pb[1];
            v0 -= pa0[1]*pb[1];
            v1 += pa0[1]*pb[0];

            pa0 += 2;
            pb  += 2;
        }
        if( beta.x==0.0 && beta.y==0.0 )
        {
            tx = 0.0;
            ty = 0.0;
        }
        else
        {
            tx = beta.x*dy[0]-beta.y*dy[1];
            ty = beta.x*dy[1]+beta.y*dy[0];
        }
        tx += alpha.x*v0-alpha.y*v1;
        ty += alpha.x*v1+alpha.y*v0;
        dy[0] = tx;
        dy[1] = ty;
        dy += 2*stride;
        parow += 2*alglib_c_block;
    }
}
#endif

/********************************************************************
This subroutine sets vector to zero
********************************************************************/
void _ialglib_vzero(ae_int_t n, double *p, ae_int_t stride)
{
    ae_int_t i;
    if( stride==1 )
    {
        for(i=0; i<n; i++,p++)
            *p = 0.0;
    }
    else
    {
        for(i=0; i<n; i++,p+=stride)
            *p = 0.0;
    }
}

/********************************************************************
This subroutine sets vector to zero
********************************************************************/
void _ialglib_vzero_complex(ae_int_t n, ae_complex *p, ae_int_t stride)
{
    ae_int_t i;
    if( stride==1 )
    {
        for(i=0; i<n; i++,p++)
        {
            p->x = 0.0;
            p->y = 0.0;
        }
    }
    else
    {
        for(i=0; i<n; i++,p+=stride)
        {
            p->x = 0.0;
            p->y = 0.0;
        }
    }
}


/********************************************************************
This subroutine copies unaligned real vector
********************************************************************/
void _ialglib_vcopy(ae_int_t n, const double *a, ae_int_t stridea, double *b, ae_int_t strideb)
{
    ae_int_t i, n2;
    if( stridea==1 && strideb==1 )
    {
        n2 = n/2;
        for(i=n2; i!=0; i--, a+=2, b+=2)
        {
            b[0] = a[0];
            b[1] = a[1];
        }
        if( n%2!=0 )
            b[0] = a[0];
    }
    else
    {
        for(i=0; i<n; i++,a+=stridea,b+=strideb)
            *b = *a;
    }
}


/********************************************************************
This subroutine copies unaligned complex vector
(passed as ae_complex*)

1. strideb is stride measured in complex numbers, not doubles
2. conj may be "N" (no conj.) or "C" (conj.)
********************************************************************/
void _ialglib_vcopy_complex(ae_int_t n, const ae_complex *a, ae_int_t stridea, double *b, ae_int_t strideb, const char *conj)
{
    ae_int_t i;

    /*
     * more general case
     */
    if( conj[0]=='N' || conj[0]=='n' )
    {
        for(i=0; i<n; i++,a+=stridea,b+=2*strideb)
        {
            b[0] = a->x;
            b[1] = a->y;
        }
    }
    else
    {
        for(i=0; i<n; i++,a+=stridea,b+=2*strideb)
        {
            b[0] = a->x;
            b[1] = -a->y;
        }
    }
}


/********************************************************************
This subroutine copies unaligned complex vector (passed as double*)

1. strideb is stride measured in complex numbers, not doubles
2. conj may be "N" (no conj.) or "C" (conj.)
********************************************************************/
void _ialglib_vcopy_dcomplex(ae_int_t n, const double *a, ae_int_t stridea, double *b, ae_int_t strideb, const char *conj)
{
    ae_int_t i;

    /*
     * more general case
     */
    if( conj[0]=='N' || conj[0]=='n' )
    {
        for(i=0; i<n; i++,a+=2*stridea,b+=2*strideb)
        {
            b[0] = a[0];
            b[1] = a[1];
        }
    }
    else
    {
        for(i=0; i<n; i++,a+=2*stridea,b+=2*strideb)
        {
            b[0] = a[0];
            b[1] = -a[1];
        }
    }
}


/********************************************************************
This subroutine copies matrix from  non-aligned non-contigous storage
to aligned contigous storage

A:
* MxN
* non-aligned
* non-contigous
* may be transformed during copying (as prescribed by op)

B:
* alglib_r_block*alglib_r_block (only MxN/NxM submatrix is used)
* aligned
* stride is alglib_r_block

Transformation types:
* 0 - no transform
* 1 - transposition
********************************************************************/
void _ialglib_mcopyblock(ae_int_t m, ae_int_t n, const double *a, ae_int_t op, ae_int_t stride, double *b)
{
    ae_int_t i, j, n2;
    const double *psrc;
    double *pdst;
    if( op==0 )
    {
        n2 = n/2;
        for(i=0,psrc=a; i<m; i++,a+=stride,b+=alglib_r_block,psrc=a)
        {
            for(j=0,pdst=b; j<n2; j++,pdst+=2,psrc+=2)
            {
                pdst[0] = psrc[0];
                pdst[1] = psrc[1];
            }
            if( n%2!=0 )
                pdst[0] = psrc[0];
        }
    }
    else
    {
        n2 = n/2;
        for(i=0,psrc=a; i<m; i++,a+=stride,b+=1,psrc=a)
        {
            for(j=0,pdst=b; j<n2; j++,pdst+=alglib_twice_r_block,psrc+=2)
            {
                pdst[0] = psrc[0];
                pdst[alglib_r_block] = psrc[1];
            }
            if( n%2!=0 )
                pdst[0] = psrc[0];
        }
    }
}


/********************************************************************
This subroutine copies matrix from  non-aligned non-contigous storage
to aligned contigous storage

A:
* MxN
* non-aligned
* non-contigous
* may be transformed during copying (as prescribed by op)

B:
* alglib_r_block*alglib_r_block (only MxN/NxM submatrix is used)
* aligned
* stride is alglib_r_block

Transformation types:
* 0 - no transform
* 1 - transposition

This function supports SSE2; it can be used when:
1. AE_HAS_SSE2_INTRINSICS was defined (checked at compile-time)
2. ae_cpuid() result contains CPU_SSE2 (checked at run-time)

If (1) is failed, this function will be undefined. If (2) is failed,  call 
to this function will probably crash your system. 

If  you  want  to  know  whether  it  is safe to call it, you should check 
results  of  ae_cpuid(). If CPU_SSE2 bit is set, this function is callable 
and will do its work.
********************************************************************/
#if defined(AE_HAS_SSE2_INTRINSICS)
void _ialglib_mcopyblock_sse2(ae_int_t m, ae_int_t n, const double *a, ae_int_t op, ae_int_t stride, double *b)
{
    ae_int_t i, j, nb8, mb2, ntail;
    const double *psrc0, *psrc1;
    double *pdst;
    nb8 = n/8;
    ntail = n-8*nb8;
    if( op==0 )
    {
        for(i=0,psrc0=a; i<m; i++,a+=stride,b+=alglib_r_block,psrc0=a)
        {
            pdst=b;
            for(j=0; j<nb8; j++)
            {
                __m128d v0, v1;
                v0 = _mm_loadu_pd(psrc0);
                _mm_store_pd(pdst, v0);
                v1 = _mm_loadu_pd(psrc0+2);
                _mm_store_pd(pdst+2, v1);
                v1 = _mm_loadu_pd(psrc0+4);
                _mm_store_pd(pdst+4, v1);
                v1 = _mm_loadu_pd(psrc0+6);
                _mm_store_pd(pdst+6, v1);
                pdst+=8;
                psrc0+=8;
            }
            for(j=0; j<ntail; j++)
                pdst[j] = psrc0[j];
        }
    }
    else
    {
        const double *arow0, *arow1;
        double *bcol0, *bcol1, *pdst0, *pdst1;
        ae_int_t nb4, ntail, n2;
                
        n2 = n/2;
        mb2 = m/2;
        nb4 = n/4;
        ntail = n-4*nb4;
        
        arow0 = a;
        arow1 = a+stride;
        bcol0 = b;
        bcol1 = b+1;
        for(i=0; i<mb2; i++)
        {
            psrc0 = arow0;
            psrc1 = arow1;
            pdst0 = bcol0;
            pdst1 = bcol1;
            for(j=0; j<nb4; j++)
            {
                __m128d v0, v1, v2, v3;
                v0 = _mm_loadu_pd(psrc0);
                v1 = _mm_loadu_pd(psrc1);
                v2 = _mm_loadu_pd(psrc0+2);
                v3 = _mm_loadu_pd(psrc1+2);
                _mm_store_pd(pdst0, _mm_unpacklo_pd(v0,v1));
                _mm_store_pd(pdst0+alglib_r_block, _mm_unpackhi_pd(v0,v1));                
                _mm_store_pd(pdst0+2*alglib_r_block, _mm_unpacklo_pd(v2,v3));
                _mm_store_pd(pdst0+3*alglib_r_block, _mm_unpackhi_pd(v2,v3));

                pdst0 += 4*alglib_r_block;
                pdst1 += 4*alglib_r_block;
                psrc0 += 4;
                psrc1 += 4;
            }
            for(j=0; j<ntail; j++)
            {
                pdst0[0] = psrc0[0];
                pdst1[0] = psrc1[0];
                pdst0 += alglib_r_block;
                pdst1 += alglib_r_block;
                psrc0 += 1;
                psrc1 += 1;
            }
            arow0 += 2*stride;
            arow1 += 2*stride;
            bcol0 += 2;
            bcol1 += 2;
        }
        if( m%2 )
        {
            psrc0 = arow0;
            pdst0 = bcol0;
            for(j=0; j<n2; j++)
            {
                pdst0[0] = psrc0[0];
                pdst0[alglib_r_block] = psrc0[1];
                pdst0 += alglib_twice_r_block;
                psrc0 += 2;
            }
            if( n%2!=0 )
                pdst0[0] = psrc0[0];
        }
    }
}
#endif


/********************************************************************
This subroutine copies matrix from  aligned contigous storage to non-
aligned non-contigous storage

A:
* MxN
* aligned
* contigous
* stride is alglib_r_block
* may be transformed during copying (as prescribed by op)

B:
* alglib_r_block*alglib_r_block (only MxN/NxM submatrix is used)
* non-aligned, non-contigous

Transformation types:
* 0 - no transform
* 1 - transposition
********************************************************************/
void _ialglib_mcopyunblock(ae_int_t m, ae_int_t n, const double *a, ae_int_t op, double *b, ae_int_t stride)
{
    ae_int_t i, j, n2;
    const double *psrc;
    double *pdst;
    if( op==0 )
    {
        n2 = n/2;
        for(i=0,psrc=a; i<m; i++,a+=alglib_r_block,b+=stride,psrc=a)
        {
            for(j=0,pdst=b; j<n2; j++,pdst+=2,psrc+=2)
            {
                pdst[0] = psrc[0];
                pdst[1] = psrc[1];
            }
            if( n%2!=0 )
                pdst[0] = psrc[0];
        }
    }
    else
    {
        n2 = n/2;
        for(i=0,psrc=a; i<m; i++,a++,b+=stride,psrc=a)
        {
            for(j=0,pdst=b; j<n2; j++,pdst+=2,psrc+=alglib_twice_r_block)
            {
                pdst[0] = psrc[0];
                pdst[1] = psrc[alglib_r_block];
            }
            if( n%2!=0 )
                pdst[0] = psrc[0];
        }
    }
}


/********************************************************************
This subroutine copies matrix from  non-aligned non-contigous storage
to aligned contigous storage

A:
* MxN
* non-aligned
* non-contigous
* may be transformed during copying (as prescribed by op)
* pointer to ae_complex is passed

B:
* 2*alglib_c_block*alglib_c_block doubles (only MxN/NxM submatrix is used)
* aligned
* stride is alglib_c_block
* pointer to double is passed

Transformation types:
* 0 - no transform
* 1 - transposition
* 2 - conjugate transposition
* 3 - conjugate, but no  transposition
********************************************************************/
void _ialglib_mcopyblock_complex(ae_int_t m, ae_int_t n, const ae_complex *a, ae_int_t op, ae_int_t stride, double *b)
{
    ae_int_t i, j;
    const ae_complex *psrc;
    double *pdst;
    if( op==0 )
    {
        for(i=0,psrc=a; i<m; i++,a+=stride,b+=alglib_twice_c_block,psrc=a)
            for(j=0,pdst=b; j<n; j++,pdst+=2,psrc++)
            {
                pdst[0] = psrc->x;
                pdst[1] = psrc->y;
            }
    }
    if( op==1 )
    {
        for(i=0,psrc=a; i<m; i++,a+=stride,b+=2,psrc=a)
            for(j=0,pdst=b; j<n; j++,pdst+=alglib_twice_c_block,psrc++)
            {
                pdst[0] = psrc->x;
                pdst[1] = psrc->y;
            }
    }
    if( op==2 )
    {
        for(i=0,psrc=a; i<m; i++,a+=stride,b+=2,psrc=a)
            for(j=0,pdst=b; j<n; j++,pdst+=alglib_twice_c_block,psrc++)
            {
                pdst[0] = psrc->x;
                pdst[1] = -psrc->y;
            }
    }
    if( op==3 )
    {
        for(i=0,psrc=a; i<m; i++,a+=stride,b+=alglib_twice_c_block,psrc=a)
            for(j=0,pdst=b; j<n; j++,pdst+=2,psrc++)
            {
                pdst[0] = psrc->x;
                pdst[1] = -psrc->y;
            }
    }
}


/********************************************************************
This subroutine copies matrix from aligned contigous storage to
non-aligned non-contigous storage

A:
* 2*alglib_c_block*alglib_c_block doubles (only MxN submatrix is used)
* aligned
* stride is alglib_c_block
* pointer to double is passed
* may be transformed during copying (as prescribed by op)

B:
* MxN
* non-aligned
* non-contigous
* pointer to ae_complex is passed

Transformation types:
* 0 - no transform
* 1 - transposition
* 2 - conjugate transposition
* 3 - conjugate, but no  transposition
********************************************************************/
void _ialglib_mcopyunblock_complex(ae_int_t m, ae_int_t n, const double *a, ae_int_t op, ae_complex* b, ae_int_t stride)
{
    ae_int_t i, j;
    const double *psrc;
    ae_complex *pdst;
    if( op==0 )
    {
        for(i=0,psrc=a; i<m; i++,a+=alglib_twice_c_block,b+=stride,psrc=a)
            for(j=0,pdst=b; j<n; j++,pdst++,psrc+=2)
            {
                pdst->x = psrc[0];
                pdst->y = psrc[1];
            }
    }
    if( op==1 )
    {
        for(i=0,psrc=a; i<m; i++,a+=2,b+=stride,psrc=a)
            for(j=0,pdst=b; j<n; j++,pdst++,psrc+=alglib_twice_c_block)
            {
                pdst->x = psrc[0];
                pdst->y = psrc[1];
            }
    }
    if( op==2 )
    {
        for(i=0,psrc=a; i<m; i++,a+=2,b+=stride,psrc=a)
            for(j=0,pdst=b; j<n; j++,pdst++,psrc+=alglib_twice_c_block)
            {
                pdst->x = psrc[0];
                pdst->y = -psrc[1];
            }
    }
    if( op==3 )
    {
        for(i=0,psrc=a; i<m; i++,a+=alglib_twice_c_block,b+=stride,psrc=a)
            for(j=0,pdst=b; j<n; j++,pdst++,psrc+=2)
            {
                pdst->x = psrc[0];
                pdst->y = -psrc[1];
            }
    }
}


/********************************************************************
Real GEMM kernel
********************************************************************/
ae_bool _ialglib_rmatrixgemm(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     double alpha,
     double *_a,
     ae_int_t _a_stride,
     ae_int_t optypea,
     double *_b,
     ae_int_t _b_stride,
     ae_int_t optypeb,
     double beta,
     double *_c,
     ae_int_t _c_stride)
{
    int i;
    double *crow;
    double _abuf[alglib_r_block+alglib_simd_alignment];
    double _bbuf[alglib_r_block*alglib_r_block+alglib_simd_alignment];
    double * const abuf = (double * const) ae_align(_abuf,alglib_simd_alignment);
    double * const b    = (double * const) ae_align(_bbuf,alglib_simd_alignment);
    void (*rmv)(ae_int_t, ae_int_t, const double *, const double *, double *, ae_int_t, double, double) = &_ialglib_rmv;
    void (*mcopyblock)(ae_int_t, ae_int_t, const double *, ae_int_t, ae_int_t, double *) = &_ialglib_mcopyblock;
    
    if( m>alglib_r_block || n>alglib_r_block || k>alglib_r_block || m<=0 || n<=0 || k<=0 || alpha==0.0 )
        return ae_false;

    /*
     * Check for SSE2 support
     */
#ifdef AE_HAS_SSE2_INTRINSICS
    if( ae_cpuid() & CPU_SSE2 )
    {
        rmv = &_ialglib_rmv_sse2;
        mcopyblock = &_ialglib_mcopyblock_sse2;
    }
#endif
    
    /*
     * copy b
     */
    if( optypeb==0 )
        mcopyblock(k, n, _b, 1, _b_stride, b);
    else
        mcopyblock(n, k, _b, 0, _b_stride, b);

    /*
     * multiply B by A (from the right, by rows)
     * and store result in C
     */
    crow  = _c;
    if( optypea==0 )
    {
        const double *arow = _a;
        for(i=0; i<m; i++)
        {
            _ialglib_vcopy(k, arow, 1, abuf, 1);
            if( beta==0 )
                _ialglib_vzero(n, crow, 1);
            rmv(n, k, b, abuf, crow, 1, alpha, beta);
            crow += _c_stride;
            arow += _a_stride;
        }
    }
    else
    {
        const double *acol = _a;
        for(i=0; i<m; i++)
        {
            _ialglib_vcopy(k, acol, _a_stride, abuf, 1);
            if( beta==0 )
                _ialglib_vzero(n, crow, 1);
            rmv(n, k, b, abuf, crow, 1, alpha, beta);
            crow += _c_stride;
            acol++;
        }
    }
    return ae_true;
}


/********************************************************************
Complex GEMM kernel
********************************************************************/
ae_bool _ialglib_cmatrixgemm(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     ae_complex alpha,
     ae_complex *_a,
     ae_int_t _a_stride,
     ae_int_t optypea,
     ae_complex *_b,
     ae_int_t _b_stride,
     ae_int_t optypeb,
     ae_complex beta,
     ae_complex *_c,
     ae_int_t _c_stride)
 {
    const ae_complex *arow;
    ae_complex *crow;
    ae_int_t i;
    double _loc_abuf[2*alglib_c_block+alglib_simd_alignment];
    double _loc_b[2*alglib_c_block*alglib_c_block+alglib_simd_alignment];
    double * const abuf = (double * const) ae_align(_loc_abuf,alglib_simd_alignment);
    double * const b    = (double * const) ae_align(_loc_b,   alglib_simd_alignment);
    ae_int_t brows;
    ae_int_t bcols;
    void (*cmv)(ae_int_t, ae_int_t, const double *, const double *, ae_complex *, double *, ae_int_t, ae_complex, ae_complex) = &_ialglib_cmv;
    
    if( m>alglib_c_block || n>alglib_c_block || k>alglib_c_block )
        return ae_false;

    /*
     * Check for SSE2 support
     */
#ifdef AE_HAS_SSE2_INTRINSICS
    if( ae_cpuid() & CPU_SSE2 )
    {
        cmv = &_ialglib_cmv_sse2;
    }    
#endif
    
    /*
     * copy b
     */
    brows = optypeb==0 ? k : n;
    bcols = optypeb==0 ? n : k;
    if( optypeb==0 )
        _ialglib_mcopyblock_complex(brows, bcols, _b, 1, _b_stride, b);
    if( optypeb==1 )
        _ialglib_mcopyblock_complex(brows, bcols, _b, 0, _b_stride, b);
    if( optypeb==2 )
        _ialglib_mcopyblock_complex(brows, bcols, _b, 3, _b_stride, b);

    /*
     * multiply B by A (from the right, by rows)
     * and store result in C
     */
    arow  = _a;
    crow  = _c;
    for(i=0; i<m; i++)
    {
        if( optypea==0 )
        {
            _ialglib_vcopy_complex(k, arow, 1, abuf, 1, "No conj");
            arow += _a_stride;
        }
        else if( optypea==1 )
        {
            _ialglib_vcopy_complex(k, arow, _a_stride, abuf, 1, "No conj");
            arow++;
        }
        else
        {
            _ialglib_vcopy_complex(k, arow, _a_stride, abuf, 1, "Conj");
            arow++;
        }
        if( beta.x==0 && beta.y==0 )
            _ialglib_vzero_complex(n, crow, 1);
        cmv(n, k, b, abuf, crow, NULL, 1, alpha, beta);
        crow += _c_stride;
    }
    return ae_true;
}


/********************************************************************
complex TRSM kernel
********************************************************************/
ae_bool _ialglib_cmatrixrighttrsm(ae_int_t m,
     ae_int_t n,
     ae_complex *_a,
     ae_int_t _a_stride,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     ae_complex *_x,
     ae_int_t _x_stride)
{
    /*
     * local buffers
     */
    double *pdiag;
    ae_int_t i;
    double _loc_abuf[2*alglib_c_block*alglib_c_block+alglib_simd_alignment];
    double _loc_xbuf[2*alglib_c_block*alglib_c_block+alglib_simd_alignment];
    double _loc_tmpbuf[2*alglib_c_block+alglib_simd_alignment];
    double * const abuf   = (double * const) ae_align(_loc_abuf,  alglib_simd_alignment);
    double * const xbuf   = (double * const) ae_align(_loc_xbuf,  alglib_simd_alignment);
    double * const tmpbuf = (double * const) ae_align(_loc_tmpbuf,alglib_simd_alignment);
    ae_bool uppera;
    void (*cmv)(ae_int_t, ae_int_t, const double *, const double *, ae_complex *, double *, ae_int_t, ae_complex, ae_complex) = &_ialglib_cmv;
    
    if( m>alglib_c_block || n>alglib_c_block )
        return ae_false;

    /*
     * Check for SSE2 support
     */
#ifdef AE_HAS_SSE2_INTRINSICS
    if( ae_cpuid() & CPU_SSE2 )
    {
        cmv = &_ialglib_cmv_sse2;
    }    
#endif
    
    /*
     * Prepare
     */
    _ialglib_mcopyblock_complex(n, n, _a, optype, _a_stride, abuf);
    _ialglib_mcopyblock_complex(m, n, _x, 0, _x_stride, xbuf);
    if( isunit )
        for(i=0,pdiag=abuf; i<n; i++,pdiag+=2*(alglib_c_block+1))
        {
            pdiag[0] = 1.0;
            pdiag[1] = 0.0;
        }
    if( optype==0 )
        uppera = isupper;
    else
        uppera = !isupper;

    /*
     * Solve Y*A^-1=X where A is upper or lower triangular
     */
    if( uppera )
    {
        for(i=0,pdiag=abuf; i<n; i++,pdiag+=2*(alglib_c_block+1))
        {
            ae_complex tmp_c;
            ae_complex beta;
            ae_complex alpha;
            tmp_c.x = pdiag[0];
            tmp_c.y = pdiag[1];
            beta = ae_c_d_div(1.0, tmp_c);
            alpha.x = -beta.x;
            alpha.y = -beta.y;
            _ialglib_vcopy_dcomplex(i, abuf+2*i, alglib_c_block, tmpbuf, 1, "No conj");
            cmv(m, i, xbuf, tmpbuf, NULL, xbuf+2*i, alglib_c_block, alpha, beta);
        }
        _ialglib_mcopyunblock_complex(m, n, xbuf, 0, _x, _x_stride);
    }
    else
    {
        for(i=n-1,pdiag=abuf+2*((n-1)*alglib_c_block+(n-1)); i>=0; i--,pdiag-=2*(alglib_c_block+1))
        {
            ae_complex tmp_c;
            ae_complex beta;
            ae_complex alpha;
            tmp_c.x = pdiag[0];
            tmp_c.y = pdiag[1];
            beta = ae_c_d_div(1.0, tmp_c);
            alpha.x = -beta.x;
            alpha.y = -beta.y;
            _ialglib_vcopy_dcomplex(n-1-i, pdiag+2*alglib_c_block, alglib_c_block, tmpbuf, 1, "No conj");
            cmv(m, n-1-i, xbuf+2*(i+1), tmpbuf, NULL, xbuf+2*i, alglib_c_block, alpha, beta);
        }
        _ialglib_mcopyunblock_complex(m, n, xbuf, 0, _x, _x_stride);
    }
    return ae_true;
}


/********************************************************************
real TRSM kernel
********************************************************************/
ae_bool _ialglib_rmatrixrighttrsm(ae_int_t m,
     ae_int_t n,
     double *_a,
     ae_int_t _a_stride,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     double *_x,
     ae_int_t _x_stride)
{
    /*
     * local buffers
     */
    double *pdiag;
    ae_int_t i;
    double _loc_abuf[alglib_r_block*alglib_r_block+alglib_simd_alignment];
    double _loc_xbuf[alglib_r_block*alglib_r_block+alglib_simd_alignment];
    double _loc_tmpbuf[alglib_r_block+alglib_simd_alignment];
    double * const abuf   = (double * const) ae_align(_loc_abuf,  alglib_simd_alignment);
    double * const xbuf   = (double * const) ae_align(_loc_xbuf,  alglib_simd_alignment);
    double * const tmpbuf = (double * const) ae_align(_loc_tmpbuf,alglib_simd_alignment);
    ae_bool uppera;
    void (*rmv)(ae_int_t, ae_int_t, const double *, const double *, double *, ae_int_t, double, double) = &_ialglib_rmv;
    void (*mcopyblock)(ae_int_t, ae_int_t, const double *, ae_int_t, ae_int_t, double *) = &_ialglib_mcopyblock;
    
    if( m>alglib_r_block || n>alglib_r_block )
        return ae_false;

    /*
     * Check for SSE2 support
     */
#ifdef AE_HAS_SSE2_INTRINSICS
    if( ae_cpuid() & CPU_SSE2 )
    {
        rmv = &_ialglib_rmv_sse2;
        mcopyblock = &_ialglib_mcopyblock_sse2;
    }    
#endif
    
    /*
     * Prepare
     */
    mcopyblock(n, n, _a, optype, _a_stride, abuf);
    mcopyblock(m, n, _x, 0, _x_stride, xbuf);
    if( isunit )
        for(i=0,pdiag=abuf; i<n; i++,pdiag+=alglib_r_block+1)
            *pdiag = 1.0;
    if( optype==0 )
        uppera = isupper;
    else
        uppera = !isupper;

    /*
     * Solve Y*A^-1=X where A is upper or lower triangular
     */
    if( uppera )
    {
        for(i=0,pdiag=abuf; i<n; i++,pdiag+=alglib_r_block+1)
        {
            double beta  = 1.0/(*pdiag);
            double alpha = -beta;
            _ialglib_vcopy(i, abuf+i, alglib_r_block, tmpbuf, 1);
            rmv(m, i, xbuf, tmpbuf, xbuf+i, alglib_r_block, alpha, beta);
        }
        _ialglib_mcopyunblock(m, n, xbuf, 0, _x, _x_stride);
    }
    else
    {
        for(i=n-1,pdiag=abuf+(n-1)*alglib_r_block+(n-1); i>=0; i--,pdiag-=alglib_r_block+1)
        {
            double beta = 1.0/(*pdiag);
            double alpha = -beta;
            _ialglib_vcopy(n-1-i, pdiag+alglib_r_block, alglib_r_block, tmpbuf+i+1, 1);
            rmv(m, n-1-i, xbuf+i+1, tmpbuf+i+1, xbuf+i, alglib_r_block, alpha, beta);
        }
        _ialglib_mcopyunblock(m, n, xbuf, 0, _x, _x_stride);
    }
    return ae_true;
}


/********************************************************************
complex TRSM kernel
********************************************************************/
ae_bool _ialglib_cmatrixlefttrsm(ae_int_t m,
     ae_int_t n,
     ae_complex *_a,
     ae_int_t _a_stride,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     ae_complex *_x,
     ae_int_t _x_stride)
{
    /*
     * local buffers
     */
    double *pdiag, *arow;
    ae_int_t i;
    double _loc_abuf[2*alglib_c_block*alglib_c_block+alglib_simd_alignment];
    double _loc_xbuf[2*alglib_c_block*alglib_c_block+alglib_simd_alignment];
    double _loc_tmpbuf[2*alglib_c_block+alglib_simd_alignment];
    double * const abuf   = (double * const) ae_align(_loc_abuf,  alglib_simd_alignment);
    double * const xbuf   = (double * const) ae_align(_loc_xbuf,  alglib_simd_alignment);
    double * const tmpbuf = (double * const) ae_align(_loc_tmpbuf,alglib_simd_alignment);
    ae_bool uppera;
    void (*cmv)(ae_int_t, ae_int_t, const double *, const double *, ae_complex *, double *, ae_int_t, ae_complex, ae_complex) = &_ialglib_cmv;
    
    if( m>alglib_c_block || n>alglib_c_block )
        return ae_false;

    /*
     * Check for SSE2 support
     */
#ifdef AE_HAS_SSE2_INTRINSICS
    if( ae_cpuid() & CPU_SSE2 )
    {
        cmv = &_ialglib_cmv_sse2;
    }    
#endif
    
    /*
     * Prepare
     * Transpose X (so we may use mv, which calculates A*x, but not x*A)
     */
    _ialglib_mcopyblock_complex(m, m, _a, optype, _a_stride, abuf);
    _ialglib_mcopyblock_complex(m, n, _x, 1, _x_stride, xbuf);
    if( isunit )
        for(i=0,pdiag=abuf; i<m; i++,pdiag+=2*(alglib_c_block+1))
        {
            pdiag[0] = 1.0;
            pdiag[1] = 0.0;
        }
    if( optype==0 )
        uppera = isupper;
    else
        uppera = !isupper;

    /*
     * Solve A^-1*Y^T=X^T where A is upper or lower triangular
     */
    if( uppera )
    {
        for(i=m-1,pdiag=abuf+2*((m-1)*alglib_c_block+(m-1)); i>=0; i--,pdiag-=2*(alglib_c_block+1))
        {
            ae_complex tmp_c;
            ae_complex beta;
            ae_complex alpha;
            tmp_c.x = pdiag[0];
            tmp_c.y = pdiag[1];
            beta = ae_c_d_div(1.0, tmp_c);
            alpha.x = -beta.x;
            alpha.y = -beta.y;
            _ialglib_vcopy_dcomplex(m-1-i, pdiag+2, 1, tmpbuf, 1, "No conj");
            cmv(n, m-1-i, xbuf+2*(i+1), tmpbuf, NULL, xbuf+2*i, alglib_c_block, alpha, beta);
        }
        _ialglib_mcopyunblock_complex(m, n, xbuf, 1, _x, _x_stride);
    }
    else
    {   for(i=0,pdiag=abuf,arow=abuf; i<m; i++,pdiag+=2*(alglib_c_block+1),arow+=2*alglib_c_block)
        {
            ae_complex tmp_c;
            ae_complex beta;
            ae_complex alpha;
            tmp_c.x = pdiag[0];
            tmp_c.y = pdiag[1];
            beta = ae_c_d_div(1.0, tmp_c);
            alpha.x = -beta.x;
            alpha.y = -beta.y;
            _ialglib_vcopy_dcomplex(i, arow, 1, tmpbuf, 1, "No conj");
            cmv(n, i, xbuf, tmpbuf, NULL, xbuf+2*i, alglib_c_block, alpha, beta);
        }
        _ialglib_mcopyunblock_complex(m, n, xbuf, 1, _x, _x_stride);
    }
    return ae_true;
}


/********************************************************************
real TRSM kernel
********************************************************************/
ae_bool _ialglib_rmatrixlefttrsm(ae_int_t m,
     ae_int_t n,
     double *_a,
     ae_int_t _a_stride,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     double *_x,
     ae_int_t _x_stride)
{
    /*
     * local buffers
     */
    double *pdiag, *arow;
    ae_int_t i;
    double _loc_abuf[alglib_r_block*alglib_r_block+alglib_simd_alignment];
    double _loc_xbuf[alglib_r_block*alglib_r_block+alglib_simd_alignment];
    double _loc_tmpbuf[alglib_r_block+alglib_simd_alignment];
    double * const abuf   = (double * const) ae_align(_loc_abuf,  alglib_simd_alignment);
    double * const xbuf   = (double * const) ae_align(_loc_xbuf,  alglib_simd_alignment);
    double * const tmpbuf = (double * const) ae_align(_loc_tmpbuf,alglib_simd_alignment);
    ae_bool uppera;
    void (*rmv)(ae_int_t, ae_int_t, const double *, const double *, double *, ae_int_t, double, double) = &_ialglib_rmv;
    void (*mcopyblock)(ae_int_t, ae_int_t, const double *, ae_int_t, ae_int_t, double *) = &_ialglib_mcopyblock;
    
    if( m>alglib_r_block || n>alglib_r_block )
        return ae_false;

    /*
     * Check for SSE2 support
     */
#ifdef AE_HAS_SSE2_INTRINSICS
    if( ae_cpuid() & CPU_SSE2 )
    {
        rmv = &_ialglib_rmv_sse2;
        mcopyblock = &_ialglib_mcopyblock_sse2;
    }    
#endif
    
    /*
     * Prepare
     * Transpose X (so we may use mv, which calculates A*x, but not x*A)
     */
    mcopyblock(m, m, _a, optype, _a_stride, abuf);
    mcopyblock(m, n, _x, 1, _x_stride, xbuf);
    if( isunit )
        for(i=0,pdiag=abuf; i<m; i++,pdiag+=alglib_r_block+1)
            *pdiag = 1.0;
    if( optype==0 )
        uppera = isupper;
    else
        uppera = !isupper;

    /*
     * Solve A^-1*Y^T=X^T where A is upper or lower triangular
     */
    if( uppera )
    {
        for(i=m-1,pdiag=abuf+(m-1)*alglib_r_block+(m-1); i>=0; i--,pdiag-=alglib_r_block+1)
        {
            double beta = 1.0/(*pdiag);
            double alpha = -beta;
            _ialglib_vcopy(m-1-i, pdiag+1, 1, tmpbuf+i+1, 1);
            rmv(n, m-1-i, xbuf+i+1, tmpbuf+i+1, xbuf+i, alglib_r_block, alpha, beta);
        }
        _ialglib_mcopyunblock(m, n, xbuf, 1, _x, _x_stride);
    }
    else
    {   for(i=0,pdiag=abuf,arow=abuf; i<m; i++,pdiag+=alglib_r_block+1,arow+=alglib_r_block)
        {
            double beta = 1.0/(*pdiag);
            double alpha = -beta;
            _ialglib_vcopy(i, arow, 1, tmpbuf, 1);
            rmv(n, i, xbuf, tmpbuf, xbuf+i, alglib_r_block, alpha, beta);
        }
        _ialglib_mcopyunblock(m, n, xbuf, 1, _x, _x_stride);
    }
    return ae_true;
}


/********************************************************************
complex SYRK kernel
********************************************************************/
ae_bool _ialglib_cmatrixherk(ae_int_t n,
     ae_int_t k,
     double alpha,
     ae_complex *_a,
     ae_int_t _a_stride,
     ae_int_t optypea,
     double beta,
     ae_complex *_c,
     ae_int_t _c_stride,
     ae_bool isupper)
{
    /*
     * local buffers
     */
    double *arow, *crow;
    ae_complex c_alpha, c_beta;
    ae_int_t i;
    double _loc_abuf[2*alglib_c_block*alglib_c_block+alglib_simd_alignment];
    double _loc_cbuf[2*alglib_c_block*alglib_c_block+alglib_simd_alignment];
    double _loc_tmpbuf[2*alglib_c_block+alglib_simd_alignment];
    double * const abuf   = (double * const) ae_align(_loc_abuf,  alglib_simd_alignment);
    double * const cbuf   = (double * const) ae_align(_loc_cbuf,  alglib_simd_alignment);
    double * const tmpbuf = (double * const) ae_align(_loc_tmpbuf,alglib_simd_alignment);

    if( n>alglib_c_block || k>alglib_c_block )
        return ae_false;
    if( n==0 )
        return ae_true;

    /*
     * copy A and C, task is transformed to "A*A^H"-form.
     * if beta==0, then C is filled by zeros (and not referenced)
     *
     * alpha==0 or k==0 are correctly processed (A is not referenced)
     */
    c_alpha.x = alpha;
    c_alpha.y = 0;
    c_beta.x = beta;
    c_beta.y = 0;
    if( alpha==0 )
        k = 0;
    if( k>0 )
    {
        if( optypea==0 )
            _ialglib_mcopyblock_complex(n, k, _a, 3, _a_stride, abuf);
        else
            _ialglib_mcopyblock_complex(k, n, _a, 1, _a_stride, abuf);
    }
    _ialglib_mcopyblock_complex(n, n, _c, 0, _c_stride, cbuf);
    if( beta==0 )
    {
        for(i=0,crow=cbuf; i<n; i++,crow+=2*alglib_c_block)
            if( isupper )
                _ialglib_vzero(2*(n-i), crow+2*i, 1);
            else
                _ialglib_vzero(2*(i+1), crow, 1);
    }


    /*
     * update C
     */
    if( isupper )
    {
        for(i=0,arow=abuf,crow=cbuf; i<n; i++,arow+=2*alglib_c_block,crow+=2*alglib_c_block)
        {
            _ialglib_vcopy_dcomplex(k, arow, 1, tmpbuf, 1, "Conj");
            _ialglib_cmv(n-i, k, arow, tmpbuf, NULL, crow+2*i, 1, c_alpha, c_beta);
        }
    }
    else
    {
        for(i=0,arow=abuf,crow=cbuf; i<n; i++,arow+=2*alglib_c_block,crow+=2*alglib_c_block)
        {
            _ialglib_vcopy_dcomplex(k, arow, 1, tmpbuf, 1, "Conj");
            _ialglib_cmv(i+1, k, abuf, tmpbuf, NULL, crow, 1, c_alpha, c_beta);
        }
    }

    /*
     * copy back
     */
    _ialglib_mcopyunblock_complex(n, n, cbuf, 0, _c, _c_stride);

    return ae_true;
}


/********************************************************************
real SYRK kernel
********************************************************************/
ae_bool _ialglib_rmatrixsyrk(ae_int_t n,
     ae_int_t k,
     double alpha,
     double *_a,
     ae_int_t _a_stride,
     ae_int_t optypea,
     double beta,
     double *_c,
     ae_int_t _c_stride,
     ae_bool isupper)
{
    /*
     * local buffers
     */
    double *arow, *crow;
    ae_int_t i;
    double _loc_abuf[alglib_r_block*alglib_r_block+alglib_simd_alignment];
    double _loc_cbuf[alglib_r_block*alglib_r_block+alglib_simd_alignment];
    double * const abuf   = (double * const) ae_align(_loc_abuf,  alglib_simd_alignment);
    double * const cbuf   = (double * const) ae_align(_loc_cbuf,  alglib_simd_alignment);

    if( n>alglib_r_block || k>alglib_r_block )
        return ae_false;
    if( n==0 )
        return ae_true;

    /*
     * copy A and C, task is transformed to "A*A^T"-form.
     * if beta==0, then C is filled by zeros (and not referenced)
     *
     * alpha==0 or k==0 are correctly processed (A is not referenced)
     */
    if( alpha==0 )
        k = 0;
    if( k>0 )
    {
        if( optypea==0 )
            _ialglib_mcopyblock(n, k, _a, 0, _a_stride, abuf);
        else
            _ialglib_mcopyblock(k, n, _a, 1, _a_stride, abuf);
    }
    _ialglib_mcopyblock(n, n, _c, 0, _c_stride, cbuf);
    if( beta==0 )
    {
        for(i=0,crow=cbuf; i<n; i++,crow+=alglib_r_block)
            if( isupper )
                _ialglib_vzero(n-i, crow+i, 1);
            else
                _ialglib_vzero(i+1, crow, 1);
    }


    /*
     * update C
     */
    if( isupper )
    {
        for(i=0,arow=abuf,crow=cbuf; i<n; i++,arow+=alglib_r_block,crow+=alglib_r_block)
        {
            _ialglib_rmv(n-i, k, arow, arow, crow+i, 1, alpha, beta);
        }
    }
    else
    {
        for(i=0,arow=abuf,crow=cbuf; i<n; i++,arow+=alglib_r_block,crow+=alglib_r_block)
        {
            _ialglib_rmv(i+1, k, abuf, arow, crow, 1, alpha, beta);
        }
    }

    /*
     * copy back
     */
    _ialglib_mcopyunblock(n, n, cbuf, 0, _c, _c_stride);

    return ae_true;
}


/********************************************************************
complex rank-1 kernel
********************************************************************/
ae_bool _ialglib_cmatrixrank1(ae_int_t m,
     ae_int_t n,
     ae_complex *_a,
     ae_int_t _a_stride,
     ae_complex *_u,
     ae_complex *_v)
{
    ae_complex *arow, *pu, *pv, *vtmp, *dst;
    ae_int_t n2 = n/2;
    ae_int_t i, j;

    /*
     * update pairs of rows
     */
    arow  = _a;
    pu    = _u;
    vtmp  = _v;
    for(i=0; i<m; i++, arow+=_a_stride, pu++)
    {
        /*
         * update by two
         */
        for(j=0,pv=vtmp, dst=arow; j<n2; j++, dst+=2, pv+=2)
        {
            double ux  = pu[0].x;
            double uy  = pu[0].y;
            double v0x = pv[0].x;
            double v0y = pv[0].y;
            double v1x = pv[1].x;
            double v1y = pv[1].y;
            dst[0].x += ux*v0x-uy*v0y;
            dst[0].y += ux*v0y+uy*v0x;
            dst[1].x += ux*v1x-uy*v1y;
            dst[1].y += ux*v1y+uy*v1x;
        }

        /*
         * final update
         */
        if( n%2!=0 )
        {
            double ux = pu[0].x;
            double uy = pu[0].y;
            double vx = pv[0].x;
            double vy = pv[0].y;
            dst[0].x += ux*vx-uy*vy;
            dst[0].y += ux*vy+uy*vx;
        }
    }
    return ae_true;
}


/********************************************************************
real rank-1 kernel
********************************************************************/
ae_bool _ialglib_rmatrixrank1(ae_int_t m,
     ae_int_t n,
     double *_a,
     ae_int_t _a_stride,
     double *_u,
     double *_v)
{
    double *arow0, *arow1, *pu, *pv, *vtmp, *dst0, *dst1;
    ae_int_t m2 = m/2;
    ae_int_t n2 = n/2;
    ae_int_t stride  = _a_stride;
    ae_int_t stride2 = 2*_a_stride;
    ae_int_t i, j;

    /*
     * update pairs of rows
     */
    arow0 = _a;
    arow1 = arow0+stride;
    pu    = _u;
    vtmp  = _v;
    for(i=0; i<m2; i++,arow0+=stride2,arow1+=stride2,pu+=2)
    {
        /*
         * update by two
         */
        for(j=0,pv=vtmp, dst0=arow0, dst1=arow1; j<n2; j++, dst0+=2, dst1+=2, pv+=2)
        {
            dst0[0] += pu[0]*pv[0];
            dst0[1] += pu[0]*pv[1];
            dst1[0] += pu[1]*pv[0];
            dst1[1] += pu[1]*pv[1];
        }

        /*
         * final update
         */
        if( n%2!=0 )
        {
            dst0[0] += pu[0]*pv[0];
            dst1[0] += pu[1]*pv[0];
        }
    }

    /*
     * update last row
     */
    if( m%2!=0 )
    {
        /*
         * update by two
         */
        for(j=0,pv=vtmp, dst0=arow0; j<n2; j++, dst0+=2, pv+=2)
        {
            dst0[0] += pu[0]*pv[0];
            dst0[1] += pu[0]*pv[1];
        }

        /*
         * final update
         */
        if( n%2!=0 )
            dst0[0] += pu[0]*pv[0];
    }
    return ae_true;
}


/********************************************************************
Interface functions for efficient kernels
********************************************************************/
ae_bool _ialglib_i_rmatrixgemmf(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     double alpha,
     ae_matrix *_a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     ae_matrix *_b,
     ae_int_t ib,
     ae_int_t jb,
     ae_int_t optypeb,
     double beta,
     ae_matrix *_c,
     ae_int_t ic,
     ae_int_t jc)
{
    return _ialglib_rmatrixgemm(m, n, k, alpha, _a->ptr.pp_double[ia]+ja, _a->stride, optypea, _b->ptr.pp_double[ib]+jb, _b->stride, optypeb, beta, _c->ptr.pp_double[ic]+jc, _c->stride);
}

ae_bool _ialglib_i_cmatrixgemmf(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     ae_complex alpha,
     ae_matrix *_a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     ae_matrix *_b,
     ae_int_t ib,
     ae_int_t jb,
     ae_int_t optypeb,
     ae_complex beta,
     ae_matrix *_c,
     ae_int_t ic,
     ae_int_t jc)
{
    return _ialglib_cmatrixgemm(m, n, k, alpha, _a->ptr.pp_complex[ia]+ja, _a->stride, optypea, _b->ptr.pp_complex[ib]+jb, _b->stride, optypeb, beta, _c->ptr.pp_complex[ic]+jc, _c->stride);
}

ae_bool _ialglib_i_cmatrixrighttrsmf(ae_int_t m,
     ae_int_t n,
     ae_matrix *a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     ae_matrix *x,
     ae_int_t i2,
     ae_int_t j2)
{
    return _ialglib_cmatrixrighttrsm(m, n, &a->ptr.pp_complex[i1][j1], a->stride, isupper, isunit, optype, &x->ptr.pp_complex[i2][j2], x->stride);
}

ae_bool _ialglib_i_rmatrixrighttrsmf(ae_int_t m,
     ae_int_t n,
     ae_matrix *a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     ae_matrix *x,
     ae_int_t i2,
     ae_int_t j2)
{
    return _ialglib_rmatrixrighttrsm(m, n, &a->ptr.pp_double[i1][j1], a->stride, isupper, isunit, optype, &x->ptr.pp_double[i2][j2], x->stride);
}

ae_bool _ialglib_i_cmatrixlefttrsmf(ae_int_t m,
     ae_int_t n,
     ae_matrix *a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     ae_matrix *x,
     ae_int_t i2,
     ae_int_t j2)
{
    return _ialglib_cmatrixlefttrsm(m, n, &a->ptr.pp_complex[i1][j1], a->stride, isupper, isunit, optype, &x->ptr.pp_complex[i2][j2], x->stride);
}

ae_bool _ialglib_i_rmatrixlefttrsmf(ae_int_t m,
     ae_int_t n,
     ae_matrix *a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     ae_matrix *x,
     ae_int_t i2,
     ae_int_t j2)
{
    return _ialglib_rmatrixlefttrsm(m, n, &a->ptr.pp_double[i1][j1], a->stride, isupper, isunit, optype, &x->ptr.pp_double[i2][j2], x->stride);
}

ae_bool _ialglib_i_cmatrixherkf(ae_int_t n,
     ae_int_t k,
     double alpha,
     ae_matrix *a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     double beta,
     ae_matrix *c,
     ae_int_t ic,
     ae_int_t jc,
     ae_bool isupper)
{
    return _ialglib_cmatrixherk(n, k, alpha, &a->ptr.pp_complex[ia][ja], a->stride, optypea, beta, &c->ptr.pp_complex[ic][jc], c->stride, isupper);
}

ae_bool _ialglib_i_rmatrixsyrkf(ae_int_t n,
     ae_int_t k,
     double alpha,
     ae_matrix *a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     double beta,
     ae_matrix *c,
     ae_int_t ic,
     ae_int_t jc,
     ae_bool isupper)
{
    return _ialglib_rmatrixsyrk(n, k, alpha, &a->ptr.pp_double[ia][ja], a->stride, optypea, beta, &c->ptr.pp_double[ic][jc], c->stride, isupper);
}

ae_bool _ialglib_i_cmatrixrank1f(ae_int_t m,
     ae_int_t n,
     ae_matrix *a,
     ae_int_t ia,
     ae_int_t ja,
     ae_vector *u,
     ae_int_t uoffs,
     ae_vector *v,
     ae_int_t voffs)
{
    return _ialglib_cmatrixrank1(m, n, &a->ptr.pp_complex[ia][ja], a->stride, &u->ptr.p_complex[uoffs], &v->ptr.p_complex[voffs]);
}

ae_bool _ialglib_i_rmatrixrank1f(ae_int_t m,
     ae_int_t n,
     ae_matrix *a,
     ae_int_t ia,
     ae_int_t ja,
     ae_vector *u,
     ae_int_t uoffs,
     ae_vector *v,
     ae_int_t voffs)
{
    return _ialglib_rmatrixrank1(m, n, &a->ptr.pp_double[ia][ja], a->stride, &u->ptr.p_double[uoffs], &v->ptr.p_double[voffs]);
}




/********************************************************************
This function reads rectangular matrix A given by two column pointers
col0 and col1 and stride src_stride and moves it into contiguous row-
by-row storage given by dst.

It can handle following special cases:
* col1==NULL    in this case second column of A is filled by zeros
********************************************************************/
void _ialglib_pack_n2(
    double *col0,
    double *col1,
    ae_int_t n,
    ae_int_t src_stride,
    double *dst)
{
    ae_int_t n2, j, stride2;
    
    /*
     * handle special case
     */
    if( col1==NULL )
    {
        for(j=0; j<n; j++)
        {
            dst[0] = *col0;
            dst[1] = 0.0;
            col0 += src_stride;
            dst  += 2;
        }
        return;
    }

    /*
     * handle general case
     */
    n2 = n/2;
    stride2 = src_stride*2;
    for(j=0; j<n2; j++)
    {
        dst[0] = *col0;
        dst[1] = *col1;
        dst[2] = col0[src_stride];
        dst[3] = col1[src_stride];
        col0 += stride2;
        col1 += stride2;
        dst  += 4;
    }
    if( n%2 )
    {
        dst[0] = *col0;
        dst[1] = *col1;
    }
}

/*************************************************************************
This function reads rectangular matrix A given by two column pointers col0 
and  col1  and  stride src_stride and moves it into  contiguous row-by-row 
storage given by dst.

dst must be aligned, col0 and col1 may be non-aligned.

It can handle following special cases:
* col1==NULL        in this case second column of A is filled by zeros
* src_stride==1     efficient SSE-based code is used
* col1-col0==1      efficient SSE-based code is used

This function supports SSE2; it can be used when:
1. AE_HAS_SSE2_INTRINSICS was defined (checked at compile-time)
2. ae_cpuid() result contains CPU_SSE2 (checked at run-time)

If  you  want  to  know  whether  it  is safe to call it, you should check 
results  of  ae_cpuid(). If CPU_SSE2 bit is set, this function is callable 
and will do its work.
*************************************************************************/
#if defined(AE_HAS_SSE2_INTRINSICS)
void _ialglib_pack_n2_sse2(
    double *col0,
    double *col1,
    ae_int_t n,
    ae_int_t src_stride,
    double *dst)
{
    ae_int_t n2, j, stride2;
    
    /*
     * handle special case: col1==NULL
     */
    if( col1==NULL )
    {
        for(j=0; j<n; j++)
        {
            dst[0] = *col0;
            dst[1] = 0.0;
            col0 += src_stride;
            dst  += 2;
        }
        return;
    }

    /*
     * handle unit stride
     */
    if( src_stride==1 )
    {
        __m128d v0, v1;
        n2 = n/2;
        for(j=0; j<n2; j++)
        {
            v0 = _mm_loadu_pd(col0);
            col0 += 2;
            v1 = _mm_loadu_pd(col1);
            col1 += 2;
            _mm_store_pd(dst,  _mm_unpacklo_pd(v0,v1));
            _mm_store_pd(dst+2,_mm_unpackhi_pd(v0,v1));
            dst  += 4;
        }
        if( n%2 )
        {
            dst[0] = *col0;
            dst[1] = *col1;
        }
        return;
    }

    /*
     * handle col1-col0==1
     */
    if( col1-col0==1 )
    {
        __m128d v0, v1;
        n2 = n/2;
        stride2 = 2*src_stride;
        for(j=0; j<n2; j++)
        {
            v0 = _mm_loadu_pd(col0);
            v1 = _mm_loadu_pd(col0+src_stride);
            _mm_store_pd(dst,  v0);
            _mm_store_pd(dst+2,v1);
            col0 += stride2;
            dst  += 4;
        }
        if( n%2 )
        {
            dst[0] = col0[0];
            dst[1] = col0[1];
        }
        return;
    }
    
    /*
     * handle general case
     */
    n2 = n/2;
    stride2 = src_stride*2;
    for(j=0; j<n2; j++)
    {
        dst[0] = *col0;
        dst[1] = *col1;
        dst[2] = col0[src_stride];
        dst[3] = col1[src_stride];
        col0 += stride2;
        col1 += stride2;
        dst  += 4;
    }
    if( n%2 )
    {
        dst[0] = *col0;
        dst[1] = *col1;
    }
}
#endif


/********************************************************************
This function calculates R := alpha*A'*B+beta*R where A and B are Kx2 
matrices stored in contiguous row-by-row storage,  R  is  2x2  matrix
stored in non-contiguous row-by-row storage.

A and B must be aligned; R may be non-aligned.

If beta is zero, contents of R is ignored (not  multiplied  by zero -
just ignored).

However, when alpha is zero, we still calculate A'*B, which is 
multiplied by zero afterwards.

Function accepts additional parameter store_mode:
* if 0, full R is stored
* if 1, only first row of R is stored
* if 2, only first column of R is stored
* if 3, only top left element of R is stored
********************************************************************/
void _ialglib_mm22(double alpha, const double *a, const double *b, ae_int_t k, double beta, double *r, ae_int_t stride, ae_int_t store_mode)
{
    double v00, v01, v10, v11;
    ae_int_t t;
    v00 = 0.0;
    v01 = 0.0;
    v10 = 0.0;
    v11 = 0.0;
    for(t=0; t<k; t++)
    {
        v00 += a[0]*b[0];
        v01 += a[0]*b[1];
        v10 += a[1]*b[0];
        v11 += a[1]*b[1];
        a+=2;
        b+=2;
    }
    if( store_mode==0 )
    {
        if( beta==0 )
        {
            r[0] = alpha*v00;
            r[1] = alpha*v01;
            r[stride+0] = alpha*v10;
            r[stride+1] = alpha*v11;
        }
        else
        {
            r[0] = beta*r[0] + alpha*v00;
            r[1] = beta*r[1] + alpha*v01;
            r[stride+0] = beta*r[stride+0] + alpha*v10;
            r[stride+1] = beta*r[stride+1] + alpha*v11;
        }
        return;
    }
    if( store_mode==1 )
    {
        if( beta==0 )
        {
            r[0] = alpha*v00;
            r[1] = alpha*v01;
        }
        else
        {
            r[0] = beta*r[0] + alpha*v00;
            r[1] = beta*r[1] + alpha*v01;
        }
        return;
    }
    if( store_mode==2 )
    {
        if( beta==0 )
        {
            r[0] =alpha*v00;
            r[stride+0] = alpha*v10;
        }
        else
        {
            r[0] = beta*r[0] + alpha*v00; 
            r[stride+0] = beta*r[stride+0] + alpha*v10;
        }
        return;
    }
    if( store_mode==3 )
    {
        if( beta==0 )
        {
            r[0] = alpha*v00;
        }
        else
        {
            r[0] = beta*r[0] + alpha*v00;
        }
        return;
    }
}


/********************************************************************
This function calculates R := alpha*A'*B+beta*R where A and B are Kx2 
matrices stored in contiguous row-by-row storage,  R  is  2x2  matrix
stored in non-contiguous row-by-row storage.

A and B must be aligned; R may be non-aligned.

If beta is zero, contents of R is ignored (not  multiplied  by zero -
just ignored).

However, when alpha is zero, we still calculate A'*B, which is 
multiplied by zero afterwards.

Function accepts additional parameter store_mode:
* if 0, full R is stored
* if 1, only first row of R is stored
* if 2, only first column of R is stored
* if 3, only top left element of R is stored

This function supports SSE2; it can be used when:
1. AE_HAS_SSE2_INTRINSICS was defined (checked at compile-time)
2. ae_cpuid() result contains CPU_SSE2 (checked at run-time)

If (1) is failed, this function will still be defined and callable, but it 
will do nothing.  If (2)  is  failed , call to this function will probably 
crash your system. 

If  you  want  to  know  whether  it  is safe to call it, you should check 
results  of  ae_cpuid(). If CPU_SSE2 bit is set, this function is callable 
and will do its work.
********************************************************************/
#if defined(AE_HAS_SSE2_INTRINSICS)
void _ialglib_mm22_sse2(double alpha, const double *a, const double *b, ae_int_t k, double beta, double *r, ae_int_t stride, ae_int_t store_mode)
{
    /*
     * We calculate product of two Kx2 matrices (result is 2x2). 
     * VA and VB store result as follows:
     *
     *        [ VD[0]  VE[0] ]
     * A'*B = [              ]
     *        [ VE[1]  VD[1] ]
     *
     */
    __m128d va, vb, vd, ve, vt, r0, r1, valpha, vbeta; 
    ae_int_t t, k2;
    
    /*
     * calculate product
     */
    k2 = k/2;
    vd = _mm_setzero_pd();
    ve = _mm_setzero_pd();
    for(t=0; t<k2; t++)
    {
        vb = _mm_load_pd(b);
        va = _mm_load_pd(a);
        vt = vb;
        vb = _mm_mul_pd(va,vb);
        vt = _mm_shuffle_pd(vt, vt, 1);
        vd = _mm_add_pd(vb,vd);        
        vt = _mm_mul_pd(va,vt);
        vb = _mm_load_pd(b+2);
        ve = _mm_add_pd(vt,ve);
        va = _mm_load_pd(a+2);
        vt = vb;
        vb = _mm_mul_pd(va,vb);
        vt = _mm_shuffle_pd(vt, vt, 1);
        vd = _mm_add_pd(vb,vd);
        vt = _mm_mul_pd(va,vt);
        ve = _mm_add_pd(vt,ve);
        a+=4;
        b+=4;
    }
    if( k%2 )
    {
        va = _mm_load_pd(a);
        vb = _mm_load_pd(b);
        vt = _mm_shuffle_pd(vb, vb, 1);
        vd = _mm_add_pd(_mm_mul_pd(va,vb),vd);
        ve = _mm_add_pd(_mm_mul_pd(va,vt),ve);
    }    
    
    /*
     * r0 is first row of alpha*A'*B, r1 is second row
     */
    valpha = _mm_load1_pd(&alpha);
    r0 = _mm_mul_pd(_mm_unpacklo_pd(vd,ve),valpha);
    r1 = _mm_mul_pd(_mm_unpackhi_pd(ve,vd),valpha);
    
    /*
     * store
     */
    if( store_mode==0 )
    {
        if( beta==0 )
        {
            _mm_storeu_pd(r,r0);
            _mm_storeu_pd(r+stride,r1);
        }
        else
        {
            vbeta = _mm_load1_pd(&beta);
            _mm_storeu_pd(r,_mm_add_pd(_mm_mul_pd(_mm_loadu_pd(r),vbeta),r0));
            _mm_storeu_pd(r+stride,_mm_add_pd(_mm_mul_pd(_mm_loadu_pd(r+stride),vbeta),r1));
        }
        return;
    }
    if( store_mode==1 )
    {
        if( beta==0 )
            _mm_storeu_pd(r,r0);
        else
            _mm_storeu_pd(r,_mm_add_pd(_mm_mul_pd(_mm_loadu_pd(r),_mm_load1_pd(&beta)),r0));
        return;
    }
    if( store_mode==2 )
    {
        double buf[4];
        _mm_storeu_pd(buf,r0);
        _mm_storeu_pd(buf+2,r1);
        if( beta==0 )
        {
            r[0] =buf[0];
            r[stride+0] = buf[2];
        }
        else
        {
            r[0] = beta*r[0] + buf[0]; 
            r[stride+0] = beta*r[stride+0] + buf[2];
        }
        return;
    }
    if( store_mode==3 )
    {
        double buf[2];
        _mm_storeu_pd(buf,r0);
        if( beta==0 )
            r[0] = buf[0];
        else
            r[0] = beta*r[0] + buf[0];
        return;
    }
}
#endif


/*************************************************************************
This function calculates R := alpha*A'*(B0|B1)+beta*R where A, B0  and  B1 
are Kx2 matrices stored in contiguous row-by-row storage, R is 2x4  matrix 
stored in non-contiguous row-by-row storage.

A, B0 and B1 must be aligned; R may be non-aligned.

Note  that  B0  and  B1  are  two  separate  matrices  stored in different 
locations.

If beta is zero, contents of R is ignored (not  multiplied  by zero - just 
ignored).

However,  when  alpha  is  zero , we still calculate MM product,  which is 
multiplied by zero afterwards.

Unlike mm22 functions, this function does NOT support partial  output of R 
- we always store full 2x4 matrix.
*************************************************************************/
void _ialglib_mm22x2(double alpha, const double *a, const double *b0, const double *b1, ae_int_t k, double beta, double *r, ae_int_t stride)
{
    _ialglib_mm22(alpha, a, b0, k, beta, r, stride, 0);
    _ialglib_mm22(alpha, a, b1, k, beta, r+2, stride, 0);
}

/*************************************************************************
This function calculates R := alpha*A'*(B0|B1)+beta*R where A, B0  and  B1 
are Kx2 matrices stored in contiguous row-by-row storage, R is 2x4  matrix 
stored in non-contiguous row-by-row storage.

A, B0 and B1 must be aligned; R may be non-aligned.

Note  that  B0  and  B1  are  two  separate  matrices  stored in different 
locations.

If beta is zero, contents of R is ignored (not  multiplied  by zero - just 
ignored).

However,  when  alpha  is  zero , we still calculate MM product,  which is 
multiplied by zero afterwards.

Unlike mm22 functions, this function does NOT support partial  output of R 
- we always store full 2x4 matrix.

This function supports SSE2; it can be used when:
1. AE_HAS_SSE2_INTRINSICS was defined (checked at compile-time)
2. ae_cpuid() result contains CPU_SSE2 (checked at run-time)

If (1) is failed, this function will still be defined and callable, but it 
will do nothing.  If (2)  is  failed , call to this function will probably 
crash your system. 

If  you  want  to  know  whether  it  is safe to call it, you should check 
results  of  ae_cpuid(). If CPU_SSE2 bit is set, this function is callable 
and will do its work.
*************************************************************************/
#if defined(AE_HAS_SSE2_INTRINSICS)
void _ialglib_mm22x2_sse2(double alpha, const double *a, const double *b0, const double *b1, ae_int_t k, double beta, double *r, ae_int_t stride)
{
    /*
     * We calculate product of two Kx2 matrices (result is 2x2). 
     * V0, V1, V2, V3 store result as follows:
     *
     *     [ V0[0]  V1[1] V2[0]  V3[1] ]
     * R = [                           ]
     *     [ V1[0]  V0[1] V3[0]  V2[1] ]
     *
     * VA0 stores current 1x2 block of A, VA1 stores shuffle of VA0,
     * VB0 and VB1 are used to store two copies of 1x2 block of B0 or B1
     * (both vars store same data - either B0 or B1). Results from multiplication
     * by VA0/VA1 are stored in VB0/VB1 too.
     * 
     */
    __m128d v0, v1, v2, v3, va0, va1, vb0, vb1; 
    __m128d r00, r01, r10, r11, valpha, vbeta; 
    ae_int_t t;
    
    v0 = _mm_setzero_pd();
    v1 = _mm_setzero_pd();
    v2 = _mm_setzero_pd();
    v3 = _mm_setzero_pd();
    for(t=0; t<k; t++)
    {
        va0 = _mm_load_pd(a);
        vb0 = _mm_load_pd(b0);
        va1 = _mm_load_pd(a);
        
        vb0 = _mm_mul_pd(va0,vb0);
        vb1 = _mm_load_pd(b0);
        v0 = _mm_add_pd(v0,vb0);        
        vb1 = _mm_mul_pd(va1,vb1);
        vb0 = _mm_load_pd(b1);
        v1 = _mm_add_pd(v1,vb1);        
        
        vb0 = _mm_mul_pd(va0,vb0);
        vb1 = _mm_load_pd(b1);
        v2 = _mm_add_pd(v2,vb0);        
        vb1 = _mm_mul_pd(va1,vb1);
        v3 = _mm_add_pd(v3,vb1);        

        a+=2;
        b0+=2;
        b1+=2;
    }

    /*
     * shuffle V1 and V3 (conversion to more convenient storage format):
     *
     *     [ V0[0]  V1[0] V2[0]  V3[0] ]
     * R = [                           ]
     *     [ V1[1]  V0[1] V3[1]  V2[1] ]
     *
     * unpack results to
     *
     * [ r00 r01 ]
     * [ r10 r11 ]
     *
     */
    valpha = _mm_load1_pd(&alpha);
    v1 = _mm_shuffle_pd(v1, v1, 1);
    v3 = _mm_shuffle_pd(v3, v3, 1);
    r00 = _mm_mul_pd(_mm_unpacklo_pd(v0,v1),valpha);
    r10 = _mm_mul_pd(_mm_unpackhi_pd(v1,v0),valpha);
    r01 = _mm_mul_pd(_mm_unpacklo_pd(v2,v3),valpha);
    r11 = _mm_mul_pd(_mm_unpackhi_pd(v3,v2),valpha);
    
    /*
     * store
     */
    if( beta==0 )
    {
        _mm_storeu_pd(r,r00);
        _mm_storeu_pd(r+2,r01);
        _mm_storeu_pd(r+stride,r10);
        _mm_storeu_pd(r+stride+2,r11);
    }
    else
    {
        vbeta = _mm_load1_pd(&beta);
        _mm_storeu_pd(r,          _mm_add_pd(_mm_mul_pd(_mm_loadu_pd(r),vbeta),r00));
        _mm_storeu_pd(r+2,        _mm_add_pd(_mm_mul_pd(_mm_loadu_pd(r+2),vbeta),r01));
        _mm_storeu_pd(r+stride,   _mm_add_pd(_mm_mul_pd(_mm_loadu_pd(r+stride),vbeta),r10));
        _mm_storeu_pd(r+stride+2, _mm_add_pd(_mm_mul_pd(_mm_loadu_pd(r+stride+2),vbeta),r11));
    }    
}
#endif
/*$ End $*/
