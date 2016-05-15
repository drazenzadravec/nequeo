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
#include "fft.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
1-dimensional complex FFT.

Array size N may be arbitrary number (composite or prime).  Composite  N's
are handled with cache-oblivious variation of  a  Cooley-Tukey  algorithm.
Small prime-factors are transformed using hard coded  codelets (similar to
FFTW codelets, but without low-level  optimization),  large  prime-factors
are handled with Bluestein's algorithm.

Fastests transforms are for smooth N's (prime factors are 2, 3,  5  only),
most fast for powers of 2. When N have prime factors  larger  than  these,
but orders of magnitude smaller than N, computations will be about 4 times
slower than for nearby highly composite N's. When N itself is prime, speed
will be 6 times lower.

Algorithm has O(N*logN) complexity for any N (composite or prime).

INPUT PARAMETERS
    A   -   array[0..N-1] - complex function to be transformed
    N   -   problem size
    
OUTPUT PARAMETERS
    A   -   DFT of a input array, array[0..N-1]
            A_out[j] = SUM(A_in[k]*exp(-2*pi*sqrt(-1)*j*k/N), k = 0..N-1)


  -- ALGLIB --
     Copyright 29.05.2009 by Bochkanov Sergey
*************************************************************************/
void fftc1d(/* Complex */ ae_vector* a, ae_int_t n, ae_state *_state)
{
    ae_frame _frame_block;
    fasttransformplan plan;
    ae_int_t i;
    ae_vector buf;

    ae_frame_make(_state, &_frame_block);
    _fasttransformplan_init(&plan, _state);
    ae_vector_init(&buf, 0, DT_REAL, _state);

    ae_assert(n>0, "FFTC1D: incorrect N!", _state);
    ae_assert(a->cnt>=n, "FFTC1D: Length(A)<N!", _state);
    ae_assert(isfinitecvector(a, n, _state), "FFTC1D: A contains infinite or NAN values!", _state);
    
    /*
     * Special case: N=1, FFT is just identity transform.
     * After this block we assume that N is strictly greater than 1.
     */
    if( n==1 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * convert input array to the more convinient format
     */
    ae_vector_set_length(&buf, 2*n, _state);
    for(i=0; i<=n-1; i++)
    {
        buf.ptr.p_double[2*i+0] = a->ptr.p_complex[i].x;
        buf.ptr.p_double[2*i+1] = a->ptr.p_complex[i].y;
    }
    
    /*
     * Generate plan and execute it.
     *
     * Plan is a combination of a successive factorizations of N and
     * precomputed data. It is much like a FFTW plan, but is not stored
     * between subroutine calls and is much simpler.
     */
    ftcomplexfftplan(n, 1, &plan, _state);
    ftapplyplan(&plan, &buf, 0, 1, _state);
    
    /*
     * result
     */
    for(i=0; i<=n-1; i++)
    {
        a->ptr.p_complex[i].x = buf.ptr.p_double[2*i+0];
        a->ptr.p_complex[i].y = buf.ptr.p_double[2*i+1];
    }
    ae_frame_leave(_state);
}


/*************************************************************************
1-dimensional complex inverse FFT.

Array size N may be arbitrary number (composite or prime).  Algorithm  has
O(N*logN) complexity for any N (composite or prime).

See FFTC1D() description for more information about algorithm performance.

INPUT PARAMETERS
    A   -   array[0..N-1] - complex array to be transformed
    N   -   problem size

OUTPUT PARAMETERS
    A   -   inverse DFT of a input array, array[0..N-1]
            A_out[j] = SUM(A_in[k]/N*exp(+2*pi*sqrt(-1)*j*k/N), k = 0..N-1)


  -- ALGLIB --
     Copyright 29.05.2009 by Bochkanov Sergey
*************************************************************************/
void fftc1dinv(/* Complex */ ae_vector* a, ae_int_t n, ae_state *_state)
{
    ae_int_t i;


    ae_assert(n>0, "FFTC1DInv: incorrect N!", _state);
    ae_assert(a->cnt>=n, "FFTC1DInv: Length(A)<N!", _state);
    ae_assert(isfinitecvector(a, n, _state), "FFTC1DInv: A contains infinite or NAN values!", _state);
    
    /*
     * Inverse DFT can be expressed in terms of the DFT as
     *
     *     invfft(x) = fft(x')'/N
     *
     * here x' means conj(x).
     */
    for(i=0; i<=n-1; i++)
    {
        a->ptr.p_complex[i].y = -a->ptr.p_complex[i].y;
    }
    fftc1d(a, n, _state);
    for(i=0; i<=n-1; i++)
    {
        a->ptr.p_complex[i].x = a->ptr.p_complex[i].x/n;
        a->ptr.p_complex[i].y = -a->ptr.p_complex[i].y/n;
    }
}


/*************************************************************************
1-dimensional real FFT.

Algorithm has O(N*logN) complexity for any N (composite or prime).

INPUT PARAMETERS
    A   -   array[0..N-1] - real function to be transformed
    N   -   problem size

OUTPUT PARAMETERS
    F   -   DFT of a input array, array[0..N-1]
            F[j] = SUM(A[k]*exp(-2*pi*sqrt(-1)*j*k/N), k = 0..N-1)

NOTE:
    F[] satisfies symmetry property F[k] = conj(F[N-k]),  so just one half
of  array  is  usually needed. But for convinience subroutine returns full
complex array (with frequencies above N/2), so its result may be  used  by
other FFT-related subroutines.


  -- ALGLIB --
     Copyright 01.06.2009 by Bochkanov Sergey
*************************************************************************/
void fftr1d(/* Real    */ ae_vector* a,
     ae_int_t n,
     /* Complex */ ae_vector* f,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t n2;
    ae_int_t idx;
    ae_complex hn;
    ae_complex hmnc;
    ae_complex v;
    ae_vector buf;
    fasttransformplan plan;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(f);
    ae_vector_init(&buf, 0, DT_REAL, _state);
    _fasttransformplan_init(&plan, _state);

    ae_assert(n>0, "FFTR1D: incorrect N!", _state);
    ae_assert(a->cnt>=n, "FFTR1D: Length(A)<N!", _state);
    ae_assert(isfinitevector(a, n, _state), "FFTR1D: A contains infinite or NAN values!", _state);
    
    /*
     * Special cases:
     * * N=1, FFT is just identity transform.
     * * N=2, FFT is simple too
     *
     * After this block we assume that N is strictly greater than 2
     */
    if( n==1 )
    {
        ae_vector_set_length(f, 1, _state);
        f->ptr.p_complex[0] = ae_complex_from_d(a->ptr.p_double[0]);
        ae_frame_leave(_state);
        return;
    }
    if( n==2 )
    {
        ae_vector_set_length(f, 2, _state);
        f->ptr.p_complex[0].x = a->ptr.p_double[0]+a->ptr.p_double[1];
        f->ptr.p_complex[0].y = (double)(0);
        f->ptr.p_complex[1].x = a->ptr.p_double[0]-a->ptr.p_double[1];
        f->ptr.p_complex[1].y = (double)(0);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Choose between odd-size and even-size FFTs
     */
    if( n%2==0 )
    {
        
        /*
         * even-size real FFT, use reduction to the complex task
         */
        n2 = n/2;
        ae_vector_set_length(&buf, n, _state);
        ae_v_move(&buf.ptr.p_double[0], 1, &a->ptr.p_double[0], 1, ae_v_len(0,n-1));
        ftcomplexfftplan(n2, 1, &plan, _state);
        ftapplyplan(&plan, &buf, 0, 1, _state);
        ae_vector_set_length(f, n, _state);
        for(i=0; i<=n2; i++)
        {
            idx = 2*(i%n2);
            hn.x = buf.ptr.p_double[idx+0];
            hn.y = buf.ptr.p_double[idx+1];
            idx = 2*((n2-i)%n2);
            hmnc.x = buf.ptr.p_double[idx+0];
            hmnc.y = -buf.ptr.p_double[idx+1];
            v.x = -ae_sin(-2*ae_pi*i/n, _state);
            v.y = ae_cos(-2*ae_pi*i/n, _state);
            f->ptr.p_complex[i] = ae_c_sub(ae_c_add(hn,hmnc),ae_c_mul(v,ae_c_sub(hn,hmnc)));
            f->ptr.p_complex[i].x = 0.5*f->ptr.p_complex[i].x;
            f->ptr.p_complex[i].y = 0.5*f->ptr.p_complex[i].y;
        }
        for(i=n2+1; i<=n-1; i++)
        {
            f->ptr.p_complex[i] = ae_c_conj(f->ptr.p_complex[n-i], _state);
        }
    }
    else
    {
        
        /*
         * use complex FFT
         */
        ae_vector_set_length(f, n, _state);
        for(i=0; i<=n-1; i++)
        {
            f->ptr.p_complex[i] = ae_complex_from_d(a->ptr.p_double[i]);
        }
        fftc1d(f, n, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
1-dimensional real inverse FFT.

Algorithm has O(N*logN) complexity for any N (composite or prime).

INPUT PARAMETERS
    F   -   array[0..floor(N/2)] - frequencies from forward real FFT
    N   -   problem size

OUTPUT PARAMETERS
    A   -   inverse DFT of a input array, array[0..N-1]

NOTE:
    F[] should satisfy symmetry property F[k] = conj(F[N-k]), so just  one
half of frequencies array is needed - elements from 0 to floor(N/2).  F[0]
is ALWAYS real. If N is even F[floor(N/2)] is real too. If N is odd,  then
F[floor(N/2)] has no special properties.

Relying on properties noted above, FFTR1DInv subroutine uses only elements
from 0th to floor(N/2)-th. It ignores imaginary part of F[0],  and in case
N is even it ignores imaginary part of F[floor(N/2)] too.

When you call this function using full arguments list - "FFTR1DInv(F,N,A)"
- you can pass either either frequencies array with N elements or  reduced
array with roughly N/2 elements - subroutine will  successfully  transform
both.

If you call this function using reduced arguments list -  "FFTR1DInv(F,A)"
- you must pass FULL array with N elements (although higher  N/2 are still
not used) because array size is used to automatically determine FFT length


  -- ALGLIB --
     Copyright 01.06.2009 by Bochkanov Sergey
*************************************************************************/
void fftr1dinv(/* Complex */ ae_vector* f,
     ae_int_t n,
     /* Real    */ ae_vector* a,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_vector h;
    ae_vector fh;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(a);
    ae_vector_init(&h, 0, DT_REAL, _state);
    ae_vector_init(&fh, 0, DT_COMPLEX, _state);

    ae_assert(n>0, "FFTR1DInv: incorrect N!", _state);
    ae_assert(f->cnt>=ae_ifloor((double)n/(double)2, _state)+1, "FFTR1DInv: Length(F)<Floor(N/2)+1!", _state);
    ae_assert(ae_isfinite(f->ptr.p_complex[0].x, _state), "FFTR1DInv: F contains infinite or NAN values!", _state);
    for(i=1; i<=ae_ifloor((double)n/(double)2, _state)-1; i++)
    {
        ae_assert(ae_isfinite(f->ptr.p_complex[i].x, _state)&&ae_isfinite(f->ptr.p_complex[i].y, _state), "FFTR1DInv: F contains infinite or NAN values!", _state);
    }
    ae_assert(ae_isfinite(f->ptr.p_complex[ae_ifloor((double)n/(double)2, _state)].x, _state), "FFTR1DInv: F contains infinite or NAN values!", _state);
    if( n%2!=0 )
    {
        ae_assert(ae_isfinite(f->ptr.p_complex[ae_ifloor((double)n/(double)2, _state)].y, _state), "FFTR1DInv: F contains infinite or NAN values!", _state);
    }
    
    /*
     * Special case: N=1, FFT is just identity transform.
     * After this block we assume that N is strictly greater than 1.
     */
    if( n==1 )
    {
        ae_vector_set_length(a, 1, _state);
        a->ptr.p_double[0] = f->ptr.p_complex[0].x;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * inverse real FFT is reduced to the inverse real FHT,
     * which is reduced to the forward real FHT,
     * which is reduced to the forward real FFT.
     *
     * Don't worry, it is really compact and efficient reduction :)
     */
    ae_vector_set_length(&h, n, _state);
    ae_vector_set_length(a, n, _state);
    h.ptr.p_double[0] = f->ptr.p_complex[0].x;
    for(i=1; i<=ae_ifloor((double)n/(double)2, _state)-1; i++)
    {
        h.ptr.p_double[i] = f->ptr.p_complex[i].x-f->ptr.p_complex[i].y;
        h.ptr.p_double[n-i] = f->ptr.p_complex[i].x+f->ptr.p_complex[i].y;
    }
    if( n%2==0 )
    {
        h.ptr.p_double[ae_ifloor((double)n/(double)2, _state)] = f->ptr.p_complex[ae_ifloor((double)n/(double)2, _state)].x;
    }
    else
    {
        h.ptr.p_double[ae_ifloor((double)n/(double)2, _state)] = f->ptr.p_complex[ae_ifloor((double)n/(double)2, _state)].x-f->ptr.p_complex[ae_ifloor((double)n/(double)2, _state)].y;
        h.ptr.p_double[ae_ifloor((double)n/(double)2, _state)+1] = f->ptr.p_complex[ae_ifloor((double)n/(double)2, _state)].x+f->ptr.p_complex[ae_ifloor((double)n/(double)2, _state)].y;
    }
    fftr1d(&h, n, &fh, _state);
    for(i=0; i<=n-1; i++)
    {
        a->ptr.p_double[i] = (fh.ptr.p_complex[i].x-fh.ptr.p_complex[i].y)/n;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine. Never call it directly!


  -- ALGLIB --
     Copyright 01.06.2009 by Bochkanov Sergey
*************************************************************************/
void fftr1dinternaleven(/* Real    */ ae_vector* a,
     ae_int_t n,
     /* Real    */ ae_vector* buf,
     fasttransformplan* plan,
     ae_state *_state)
{
    double x;
    double y;
    ae_int_t i;
    ae_int_t n2;
    ae_int_t idx;
    ae_complex hn;
    ae_complex hmnc;
    ae_complex v;


    ae_assert(n>0&&n%2==0, "FFTR1DEvenInplace: incorrect N!", _state);
    
    /*
     * Special cases:
     * * N=2
     *
     * After this block we assume that N is strictly greater than 2
     */
    if( n==2 )
    {
        x = a->ptr.p_double[0]+a->ptr.p_double[1];
        y = a->ptr.p_double[0]-a->ptr.p_double[1];
        a->ptr.p_double[0] = x;
        a->ptr.p_double[1] = y;
        return;
    }
    
    /*
     * even-size real FFT, use reduction to the complex task
     */
    n2 = n/2;
    ae_v_move(&buf->ptr.p_double[0], 1, &a->ptr.p_double[0], 1, ae_v_len(0,n-1));
    ftapplyplan(plan, buf, 0, 1, _state);
    a->ptr.p_double[0] = buf->ptr.p_double[0]+buf->ptr.p_double[1];
    for(i=1; i<=n2-1; i++)
    {
        idx = 2*(i%n2);
        hn.x = buf->ptr.p_double[idx+0];
        hn.y = buf->ptr.p_double[idx+1];
        idx = 2*(n2-i);
        hmnc.x = buf->ptr.p_double[idx+0];
        hmnc.y = -buf->ptr.p_double[idx+1];
        v.x = -ae_sin(-2*ae_pi*i/n, _state);
        v.y = ae_cos(-2*ae_pi*i/n, _state);
        v = ae_c_sub(ae_c_add(hn,hmnc),ae_c_mul(v,ae_c_sub(hn,hmnc)));
        a->ptr.p_double[2*i+0] = 0.5*v.x;
        a->ptr.p_double[2*i+1] = 0.5*v.y;
    }
    a->ptr.p_double[1] = buf->ptr.p_double[0]-buf->ptr.p_double[1];
}


/*************************************************************************
Internal subroutine. Never call it directly!


  -- ALGLIB --
     Copyright 01.06.2009 by Bochkanov Sergey
*************************************************************************/
void fftr1dinvinternaleven(/* Real    */ ae_vector* a,
     ae_int_t n,
     /* Real    */ ae_vector* buf,
     fasttransformplan* plan,
     ae_state *_state)
{
    double x;
    double y;
    double t;
    ae_int_t i;
    ae_int_t n2;


    ae_assert(n>0&&n%2==0, "FFTR1DInvInternalEven: incorrect N!", _state);
    
    /*
     * Special cases:
     * * N=2
     *
     * After this block we assume that N is strictly greater than 2
     */
    if( n==2 )
    {
        x = 0.5*(a->ptr.p_double[0]+a->ptr.p_double[1]);
        y = 0.5*(a->ptr.p_double[0]-a->ptr.p_double[1]);
        a->ptr.p_double[0] = x;
        a->ptr.p_double[1] = y;
        return;
    }
    
    /*
     * inverse real FFT is reduced to the inverse real FHT,
     * which is reduced to the forward real FHT,
     * which is reduced to the forward real FFT.
     *
     * Don't worry, it is really compact and efficient reduction :)
     */
    n2 = n/2;
    buf->ptr.p_double[0] = a->ptr.p_double[0];
    for(i=1; i<=n2-1; i++)
    {
        x = a->ptr.p_double[2*i+0];
        y = a->ptr.p_double[2*i+1];
        buf->ptr.p_double[i] = x-y;
        buf->ptr.p_double[n-i] = x+y;
    }
    buf->ptr.p_double[n2] = a->ptr.p_double[1];
    fftr1dinternaleven(buf, n, a, plan, _state);
    a->ptr.p_double[0] = buf->ptr.p_double[0]/n;
    t = (double)1/(double)n;
    for(i=1; i<=n2-1; i++)
    {
        x = buf->ptr.p_double[2*i+0];
        y = buf->ptr.p_double[2*i+1];
        a->ptr.p_double[i] = t*(x-y);
        a->ptr.p_double[n-i] = t*(x+y);
    }
    a->ptr.p_double[n2] = buf->ptr.p_double[1]/n;
}


/*$ End $*/
