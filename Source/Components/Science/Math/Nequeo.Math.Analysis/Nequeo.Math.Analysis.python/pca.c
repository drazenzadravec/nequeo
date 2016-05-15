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
#include "pca.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Principal components analysis

Subroutine  builds  orthogonal  basis  where  first  axis  corresponds  to
direction with maximum variance, second axis maximizes variance in subspace
orthogonal to first axis and so on.

It should be noted that, unlike LDA, PCA does not use class labels.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes one  important  improvement   of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison. Best results are achieved  for  high-dimensional  problems
  ! (NVars is at least 256).
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS:
    X           -   dataset, array[0..NPoints-1,0..NVars-1].
                    matrix contains ONLY INDEPENDENT VARIABLES.
    NPoints     -   dataset size, NPoints>=0
    NVars       -   number of independent variables, NVars>=1

OUTPUT PARAMETERS:
    Info        -   return code:
                    * -4, if SVD subroutine haven't converged
                    * -1, if wrong parameters has been passed (NPoints<0,
                          NVars<1)
                    *  1, if task is solved
    S2          -   array[0..NVars-1]. variance values corresponding
                    to basis vectors.
    V           -   array[0..NVars-1,0..NVars-1]
                    matrix, whose columns store basis vectors.

  -- ALGLIB --
     Copyright 25.08.2008 by Bochkanov Sergey
*************************************************************************/
void pcabuildbasis(/* Real    */ ae_matrix* x,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t* info,
     /* Real    */ ae_vector* s2,
     /* Real    */ ae_matrix* v,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix u;
    ae_matrix vt;
    ae_vector m;
    ae_vector t;
    ae_int_t i;
    ae_int_t j;
    double mean;
    double variance;
    double skewness;
    double kurtosis;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    ae_vector_clear(s2);
    ae_matrix_clear(v);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&u, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vt, 0, 0, DT_REAL, _state);
    ae_vector_init(&m, 0, DT_REAL, _state);
    ae_vector_init(&t, 0, DT_REAL, _state);

    
    /*
     * Check input data
     */
    if( npoints<0||nvars<1 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    *info = 1;
    
    /*
     * Special case: NPoints=0
     */
    if( npoints==0 )
    {
        ae_vector_set_length(s2, nvars, _state);
        ae_matrix_set_length(v, nvars, nvars, _state);
        for(i=0; i<=nvars-1; i++)
        {
            s2->ptr.p_double[i] = (double)(0);
        }
        for(i=0; i<=nvars-1; i++)
        {
            for(j=0; j<=nvars-1; j++)
            {
                if( i==j )
                {
                    v->ptr.pp_double[i][j] = (double)(1);
                }
                else
                {
                    v->ptr.pp_double[i][j] = (double)(0);
                }
            }
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Calculate means
     */
    ae_vector_set_length(&m, nvars, _state);
    ae_vector_set_length(&t, npoints, _state);
    for(j=0; j<=nvars-1; j++)
    {
        ae_v_move(&t.ptr.p_double[0], 1, &x->ptr.pp_double[0][j], x->stride, ae_v_len(0,npoints-1));
        samplemoments(&t, npoints, &mean, &variance, &skewness, &kurtosis, _state);
        m.ptr.p_double[j] = mean;
    }
    
    /*
     * Center, apply SVD, prepare output
     */
    ae_matrix_set_length(&a, ae_maxint(npoints, nvars, _state), nvars, _state);
    for(i=0; i<=npoints-1; i++)
    {
        ae_v_move(&a.ptr.pp_double[i][0], 1, &x->ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
        ae_v_sub(&a.ptr.pp_double[i][0], 1, &m.ptr.p_double[0], 1, ae_v_len(0,nvars-1));
    }
    for(i=npoints; i<=nvars-1; i++)
    {
        for(j=0; j<=nvars-1; j++)
        {
            a.ptr.pp_double[i][j] = (double)(0);
        }
    }
    if( !rmatrixsvd(&a, ae_maxint(npoints, nvars, _state), nvars, 0, 1, 2, s2, &u, &vt, _state) )
    {
        *info = -4;
        ae_frame_leave(_state);
        return;
    }
    if( npoints!=1 )
    {
        for(i=0; i<=nvars-1; i++)
        {
            s2->ptr.p_double[i] = ae_sqr(s2->ptr.p_double[i], _state)/(npoints-1);
        }
    }
    ae_matrix_set_length(v, nvars, nvars, _state);
    copyandtranspose(&vt, 0, nvars-1, 0, nvars-1, v, 0, nvars-1, 0, nvars-1, _state);
    ae_frame_leave(_state);
}


/*$ End $*/
