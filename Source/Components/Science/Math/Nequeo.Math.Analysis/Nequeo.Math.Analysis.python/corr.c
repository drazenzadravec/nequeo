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
#include "corr.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
1-dimensional complex cross-correlation.

For given Pattern/Signal returns corr(Pattern,Signal) (non-circular).

Correlation is calculated using reduction to  convolution.  Algorithm with
max(N,N)*log(max(N,N)) complexity is used (see  ConvC1D()  for  more  info
about performance).

IMPORTANT:
    for  historical reasons subroutine accepts its parameters in  reversed
    order: CorrC1D(Signal, Pattern) = Pattern x Signal (using  traditional
    definition of cross-correlation, denoting cross-correlation as "x").

INPUT PARAMETERS
    Signal  -   array[0..N-1] - complex function to be transformed,
                signal containing pattern
    N       -   problem size
    Pattern -   array[0..M-1] - complex function to be transformed,
                pattern to search withing signal
    M       -   problem size

OUTPUT PARAMETERS
    R       -   cross-correlation, array[0..N+M-2]:
                * positive lags are stored in R[0..N-1],
                  R[i] = sum(conj(pattern[j])*signal[i+j]
                * negative lags are stored in R[N..N+M-2],
                  R[N+M-1-i] = sum(conj(pattern[j])*signal[-i+j]

NOTE:
    It is assumed that pattern domain is [0..M-1].  If Pattern is non-zero
on [-K..M-1],  you can still use this subroutine, just shift result by K.

  -- ALGLIB --
     Copyright 21.07.2009 by Bochkanov Sergey
*************************************************************************/
void corrc1d(/* Complex */ ae_vector* signal,
     ae_int_t n,
     /* Complex */ ae_vector* pattern,
     ae_int_t m,
     /* Complex */ ae_vector* r,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector p;
    ae_vector b;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(r);
    ae_vector_init(&p, 0, DT_COMPLEX, _state);
    ae_vector_init(&b, 0, DT_COMPLEX, _state);

    ae_assert(n>0&&m>0, "CorrC1D: incorrect N or M!", _state);
    ae_vector_set_length(&p, m, _state);
    for(i=0; i<=m-1; i++)
    {
        p.ptr.p_complex[m-1-i] = ae_c_conj(pattern->ptr.p_complex[i], _state);
    }
    convc1d(&p, m, signal, n, &b, _state);
    ae_vector_set_length(r, m+n-1, _state);
    ae_v_cmove(&r->ptr.p_complex[0], 1, &b.ptr.p_complex[m-1], 1, "N", ae_v_len(0,n-1));
    if( m+n-2>=n )
    {
        ae_v_cmove(&r->ptr.p_complex[n], 1, &b.ptr.p_complex[0], 1, "N", ae_v_len(n,m+n-2));
    }
    ae_frame_leave(_state);
}


/*************************************************************************
1-dimensional circular complex cross-correlation.

For given Pattern/Signal returns corr(Pattern,Signal) (circular).
Algorithm has linearithmic complexity for any M/N.

IMPORTANT:
    for  historical reasons subroutine accepts its parameters in  reversed
    order:   CorrC1DCircular(Signal, Pattern) = Pattern x Signal    (using
    traditional definition of cross-correlation, denoting cross-correlation
    as "x").

INPUT PARAMETERS
    Signal  -   array[0..N-1] - complex function to be transformed,
                periodic signal containing pattern
    N       -   problem size
    Pattern -   array[0..M-1] - complex function to be transformed,
                non-periodic pattern to search withing signal
    M       -   problem size

OUTPUT PARAMETERS
    R   -   convolution: A*B. array[0..M-1].


  -- ALGLIB --
     Copyright 21.07.2009 by Bochkanov Sergey
*************************************************************************/
void corrc1dcircular(/* Complex */ ae_vector* signal,
     ae_int_t m,
     /* Complex */ ae_vector* pattern,
     ae_int_t n,
     /* Complex */ ae_vector* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector p;
    ae_vector b;
    ae_int_t i1;
    ae_int_t i2;
    ae_int_t i;
    ae_int_t j2;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(c);
    ae_vector_init(&p, 0, DT_COMPLEX, _state);
    ae_vector_init(&b, 0, DT_COMPLEX, _state);

    ae_assert(n>0&&m>0, "ConvC1DCircular: incorrect N or M!", _state);
    
    /*
     * normalize task: make M>=N,
     * so A will be longer (at least - not shorter) that B.
     */
    if( m<n )
    {
        ae_vector_set_length(&b, m, _state);
        for(i1=0; i1<=m-1; i1++)
        {
            b.ptr.p_complex[i1] = ae_complex_from_i(0);
        }
        i1 = 0;
        while(i1<n)
        {
            i2 = ae_minint(i1+m-1, n-1, _state);
            j2 = i2-i1;
            ae_v_cadd(&b.ptr.p_complex[0], 1, &pattern->ptr.p_complex[i1], 1, "N", ae_v_len(0,j2));
            i1 = i1+m;
        }
        corrc1dcircular(signal, m, &b, m, c, _state);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Task is normalized
     */
    ae_vector_set_length(&p, n, _state);
    for(i=0; i<=n-1; i++)
    {
        p.ptr.p_complex[n-1-i] = ae_c_conj(pattern->ptr.p_complex[i], _state);
    }
    convc1dcircular(signal, m, &p, n, &b, _state);
    ae_vector_set_length(c, m, _state);
    ae_v_cmove(&c->ptr.p_complex[0], 1, &b.ptr.p_complex[n-1], 1, "N", ae_v_len(0,m-n));
    if( m-n+1<=m-1 )
    {
        ae_v_cmove(&c->ptr.p_complex[m-n+1], 1, &b.ptr.p_complex[0], 1, "N", ae_v_len(m-n+1,m-1));
    }
    ae_frame_leave(_state);
}


/*************************************************************************
1-dimensional real cross-correlation.

For given Pattern/Signal returns corr(Pattern,Signal) (non-circular).

Correlation is calculated using reduction to  convolution.  Algorithm with
max(N,N)*log(max(N,N)) complexity is used (see  ConvC1D()  for  more  info
about performance).

IMPORTANT:
    for  historical reasons subroutine accepts its parameters in  reversed
    order: CorrR1D(Signal, Pattern) = Pattern x Signal (using  traditional
    definition of cross-correlation, denoting cross-correlation as "x").

INPUT PARAMETERS
    Signal  -   array[0..N-1] - real function to be transformed,
                signal containing pattern
    N       -   problem size
    Pattern -   array[0..M-1] - real function to be transformed,
                pattern to search withing signal
    M       -   problem size

OUTPUT PARAMETERS
    R       -   cross-correlation, array[0..N+M-2]:
                * positive lags are stored in R[0..N-1],
                  R[i] = sum(pattern[j]*signal[i+j]
                * negative lags are stored in R[N..N+M-2],
                  R[N+M-1-i] = sum(pattern[j]*signal[-i+j]

NOTE:
    It is assumed that pattern domain is [0..M-1].  If Pattern is non-zero
on [-K..M-1],  you can still use this subroutine, just shift result by K.

  -- ALGLIB --
     Copyright 21.07.2009 by Bochkanov Sergey
*************************************************************************/
void corrr1d(/* Real    */ ae_vector* signal,
     ae_int_t n,
     /* Real    */ ae_vector* pattern,
     ae_int_t m,
     /* Real    */ ae_vector* r,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector p;
    ae_vector b;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(r);
    ae_vector_init(&p, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);

    ae_assert(n>0&&m>0, "CorrR1D: incorrect N or M!", _state);
    ae_vector_set_length(&p, m, _state);
    for(i=0; i<=m-1; i++)
    {
        p.ptr.p_double[m-1-i] = pattern->ptr.p_double[i];
    }
    convr1d(&p, m, signal, n, &b, _state);
    ae_vector_set_length(r, m+n-1, _state);
    ae_v_move(&r->ptr.p_double[0], 1, &b.ptr.p_double[m-1], 1, ae_v_len(0,n-1));
    if( m+n-2>=n )
    {
        ae_v_move(&r->ptr.p_double[n], 1, &b.ptr.p_double[0], 1, ae_v_len(n,m+n-2));
    }
    ae_frame_leave(_state);
}


/*************************************************************************
1-dimensional circular real cross-correlation.

For given Pattern/Signal returns corr(Pattern,Signal) (circular).
Algorithm has linearithmic complexity for any M/N.

IMPORTANT:
    for  historical reasons subroutine accepts its parameters in  reversed
    order:   CorrR1DCircular(Signal, Pattern) = Pattern x Signal    (using
    traditional definition of cross-correlation, denoting cross-correlation
    as "x").

INPUT PARAMETERS
    Signal  -   array[0..N-1] - real function to be transformed,
                periodic signal containing pattern
    N       -   problem size
    Pattern -   array[0..M-1] - real function to be transformed,
                non-periodic pattern to search withing signal
    M       -   problem size

OUTPUT PARAMETERS
    R   -   convolution: A*B. array[0..M-1].


  -- ALGLIB --
     Copyright 21.07.2009 by Bochkanov Sergey
*************************************************************************/
void corrr1dcircular(/* Real    */ ae_vector* signal,
     ae_int_t m,
     /* Real    */ ae_vector* pattern,
     ae_int_t n,
     /* Real    */ ae_vector* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector p;
    ae_vector b;
    ae_int_t i1;
    ae_int_t i2;
    ae_int_t i;
    ae_int_t j2;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(c);
    ae_vector_init(&p, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);

    ae_assert(n>0&&m>0, "ConvC1DCircular: incorrect N or M!", _state);
    
    /*
     * normalize task: make M>=N,
     * so A will be longer (at least - not shorter) that B.
     */
    if( m<n )
    {
        ae_vector_set_length(&b, m, _state);
        for(i1=0; i1<=m-1; i1++)
        {
            b.ptr.p_double[i1] = (double)(0);
        }
        i1 = 0;
        while(i1<n)
        {
            i2 = ae_minint(i1+m-1, n-1, _state);
            j2 = i2-i1;
            ae_v_add(&b.ptr.p_double[0], 1, &pattern->ptr.p_double[i1], 1, ae_v_len(0,j2));
            i1 = i1+m;
        }
        corrr1dcircular(signal, m, &b, m, c, _state);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Task is normalized
     */
    ae_vector_set_length(&p, n, _state);
    for(i=0; i<=n-1; i++)
    {
        p.ptr.p_double[n-1-i] = pattern->ptr.p_double[i];
    }
    convr1dcircular(signal, m, &p, n, &b, _state);
    ae_vector_set_length(c, m, _state);
    ae_v_move(&c->ptr.p_double[0], 1, &b.ptr.p_double[n-1], 1, ae_v_len(0,m-n));
    if( m-n+1<=m-1 )
    {
        ae_v_move(&c->ptr.p_double[m-n+1], 1, &b.ptr.p_double[0], 1, ae_v_len(m-n+1,m-1));
    }
    ae_frame_leave(_state);
}


/*$ End $*/
