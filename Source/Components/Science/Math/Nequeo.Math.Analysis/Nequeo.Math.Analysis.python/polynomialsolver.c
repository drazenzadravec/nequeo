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
#include "polynomialsolver.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Polynomial root finding.

This function returns all roots of the polynomial
    P(x) = a0 + a1*x + a2*x^2 + ... + an*x^n
Both real and complex roots are returned (see below).

INPUT PARAMETERS:
    A       -   array[N+1], polynomial coefficients:
                * A[0] is constant term
                * A[N] is a coefficient of X^N
    N       -   polynomial degree

OUTPUT PARAMETERS:
    X       -   array of complex roots:
                * for isolated real root, X[I] is strictly real: IMAGE(X[I])=0
                * complex roots are always returned in pairs - roots occupy
                  positions I and I+1, with:
                  * X[I+1]=Conj(X[I])
                  * IMAGE(X[I]) > 0
                  * IMAGE(X[I+1]) = -IMAGE(X[I]) < 0
                * multiple real roots may have non-zero imaginary part due
                  to roundoff errors. There is no reliable way to distinguish
                  real root of multiplicity 2 from two  complex  roots  in
                  the presence of roundoff errors.
    Rep     -   report, additional information, following fields are set:
                * Rep.MaxErr - max( |P(xi)| )  for  i=0..N-1.  This  field
                  allows to quickly estimate "quality" of the roots  being
                  returned.

NOTE:   this function uses companion matrix method to find roots. In  case
        internal EVD  solver  fails  do  find  eigenvalues,  exception  is
        generated.

NOTE:   roots are not "polished" and  no  matrix  balancing  is  performed
        for them.

  -- ALGLIB --
     Copyright 24.02.2014 by Bochkanov Sergey
*************************************************************************/
void polynomialsolve(/* Real    */ ae_vector* a,
     ae_int_t n,
     /* Complex */ ae_vector* x,
     polynomialsolverreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _a;
    ae_matrix c;
    ae_matrix vl;
    ae_matrix vr;
    ae_vector wr;
    ae_vector wi;
    ae_int_t i;
    ae_int_t j;
    ae_bool status;
    ae_int_t nz;
    ae_int_t ne;
    ae_complex v;
    ae_complex vv;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_clear(x);
    _polynomialsolverreport_clear(rep);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vl, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vr, 0, 0, DT_REAL, _state);
    ae_vector_init(&wr, 0, DT_REAL, _state);
    ae_vector_init(&wi, 0, DT_REAL, _state);

    ae_assert(n>0, "PolynomialSolve: N<=0", _state);
    ae_assert(a->cnt>=n+1, "PolynomialSolve: Length(A)<N+1", _state);
    ae_assert(isfinitevector(a, n+1, _state), "PolynomialSolve: A contains infitite numbers", _state);
    ae_assert(ae_fp_neq(a->ptr.p_double[n],(double)(0)), "PolynomialSolve: A[N]=0", _state);
    
    /*
     * Prepare
     */
    ae_vector_set_length(x, n, _state);
    
    /*
     * Normalize A:
     * * analytically determine NZ zero roots
     * * quick exit for NZ=N
     * * make residual NE-th degree polynomial monic
     *   (here NE=N-NZ)
     */
    nz = 0;
    while(nz<n&&ae_fp_eq(a->ptr.p_double[nz],(double)(0)))
    {
        nz = nz+1;
    }
    ne = n-nz;
    for(i=nz; i<=n; i++)
    {
        a->ptr.p_double[i-nz] = a->ptr.p_double[i]/a->ptr.p_double[n];
    }
    
    /*
     * For NZ<N, build companion matrix and find NE non-zero roots
     */
    if( ne>0 )
    {
        ae_matrix_set_length(&c, ne, ne, _state);
        for(i=0; i<=ne-1; i++)
        {
            for(j=0; j<=ne-1; j++)
            {
                c.ptr.pp_double[i][j] = (double)(0);
            }
        }
        c.ptr.pp_double[0][ne-1] = -a->ptr.p_double[0];
        for(i=1; i<=ne-1; i++)
        {
            c.ptr.pp_double[i][i-1] = (double)(1);
            c.ptr.pp_double[i][ne-1] = -a->ptr.p_double[i];
        }
        status = rmatrixevd(&c, ne, 0, &wr, &wi, &vl, &vr, _state);
        ae_assert(status, "PolynomialSolve: inernal error - EVD solver failed", _state);
        for(i=0; i<=ne-1; i++)
        {
            x->ptr.p_complex[i].x = wr.ptr.p_double[i];
            x->ptr.p_complex[i].y = wi.ptr.p_double[i];
        }
    }
    
    /*
     * Remaining NZ zero roots
     */
    for(i=ne; i<=n-1; i++)
    {
        x->ptr.p_complex[i] = ae_complex_from_i(0);
    }
    
    /*
     * Rep
     */
    rep->maxerr = (double)(0);
    for(i=0; i<=ne-1; i++)
    {
        v = ae_complex_from_i(0);
        vv = ae_complex_from_i(1);
        for(j=0; j<=ne; j++)
        {
            v = ae_c_add(v,ae_c_mul_d(vv,a->ptr.p_double[j]));
            vv = ae_c_mul(vv,x->ptr.p_complex[i]);
        }
        rep->maxerr = ae_maxreal(rep->maxerr, ae_c_abs(v, _state), _state);
    }
    ae_frame_leave(_state);
}


void _polynomialsolverreport_init(void* _p, ae_state *_state)
{
    polynomialsolverreport *p = (polynomialsolverreport*)_p;
    ae_touch_ptr((void*)p);
}


void _polynomialsolverreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    polynomialsolverreport *dst = (polynomialsolverreport*)_dst;
    polynomialsolverreport *src = (polynomialsolverreport*)_src;
    dst->maxerr = src->maxerr;
}


void _polynomialsolverreport_clear(void* _p)
{
    polynomialsolverreport *p = (polynomialsolverreport*)_p;
    ae_touch_ptr((void*)p);
}


void _polynomialsolverreport_destroy(void* _p)
{
    polynomialsolverreport *p = (polynomialsolverreport*)_p;
    ae_touch_ptr((void*)p);
}


/*$ End $*/
