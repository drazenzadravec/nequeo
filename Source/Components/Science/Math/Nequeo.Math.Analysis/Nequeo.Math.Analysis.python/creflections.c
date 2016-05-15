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
#include "creflections.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Generation of an elementary complex reflection transformation

The subroutine generates elementary complex reflection H of  order  N,  so
that, for a given X, the following equality holds true:

     ( X(1) )   ( Beta )
H' * (  ..  ) = (  0   ),   H'*H = I,   Beta is a real number
     ( X(n) )   (  0   )

where

              ( V(1) )
H = 1 - Tau * (  ..  ) * ( conj(V(1)), ..., conj(V(n)) )
              ( V(n) )

where the first component of vector V equals 1.

Input parameters:
    X   -   vector. Array with elements [1..N].
    N   -   reflection order.

Output parameters:
    X   -   components from 2 to N are replaced by vector V.
            The first component is replaced with parameter Beta.
    Tau -   scalar value Tau.

This subroutine is the modification of CLARFG subroutines  from the LAPACK
library. It has similar functionality except for the fact that it  doesn’t
handle errors when intermediate results cause an overflow.

  -- LAPACK auxiliary routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     September 30, 1994
*************************************************************************/
void complexgeneratereflection(/* Complex */ ae_vector* x,
     ae_int_t n,
     ae_complex* tau,
     ae_state *_state)
{
    ae_int_t j;
    ae_complex alpha;
    double alphi;
    double alphr;
    double beta;
    double xnorm;
    double mx;
    ae_complex t;
    double s;
    ae_complex v;

    tau->x = 0;
    tau->y = 0;

    if( n<=0 )
    {
        *tau = ae_complex_from_i(0);
        return;
    }
    
    /*
     * Scale if needed (to avoid overflow/underflow during intermediate
     * calculations).
     */
    mx = (double)(0);
    for(j=1; j<=n; j++)
    {
        mx = ae_maxreal(ae_c_abs(x->ptr.p_complex[j], _state), mx, _state);
    }
    s = (double)(1);
    if( ae_fp_neq(mx,(double)(0)) )
    {
        if( ae_fp_less(mx,(double)(1)) )
        {
            s = ae_sqrt(ae_minrealnumber, _state);
            v = ae_complex_from_d(1/s);
            ae_v_cmulc(&x->ptr.p_complex[1], 1, ae_v_len(1,n), v);
        }
        else
        {
            s = ae_sqrt(ae_maxrealnumber, _state);
            v = ae_complex_from_d(1/s);
            ae_v_cmulc(&x->ptr.p_complex[1], 1, ae_v_len(1,n), v);
        }
    }
    
    /*
     * calculate
     */
    alpha = x->ptr.p_complex[1];
    mx = (double)(0);
    for(j=2; j<=n; j++)
    {
        mx = ae_maxreal(ae_c_abs(x->ptr.p_complex[j], _state), mx, _state);
    }
    xnorm = (double)(0);
    if( ae_fp_neq(mx,(double)(0)) )
    {
        for(j=2; j<=n; j++)
        {
            t = ae_c_div_d(x->ptr.p_complex[j],mx);
            xnorm = xnorm+ae_c_mul(t,ae_c_conj(t, _state)).x;
        }
        xnorm = ae_sqrt(xnorm, _state)*mx;
    }
    alphr = alpha.x;
    alphi = alpha.y;
    if( ae_fp_eq(xnorm,(double)(0))&&ae_fp_eq(alphi,(double)(0)) )
    {
        *tau = ae_complex_from_i(0);
        x->ptr.p_complex[1] = ae_c_mul_d(x->ptr.p_complex[1],s);
        return;
    }
    mx = ae_maxreal(ae_fabs(alphr, _state), ae_fabs(alphi, _state), _state);
    mx = ae_maxreal(mx, ae_fabs(xnorm, _state), _state);
    beta = -mx*ae_sqrt(ae_sqr(alphr/mx, _state)+ae_sqr(alphi/mx, _state)+ae_sqr(xnorm/mx, _state), _state);
    if( ae_fp_less(alphr,(double)(0)) )
    {
        beta = -beta;
    }
    tau->x = (beta-alphr)/beta;
    tau->y = -alphi/beta;
    alpha = ae_c_d_div(1,ae_c_sub_d(alpha,beta));
    if( n>1 )
    {
        ae_v_cmulc(&x->ptr.p_complex[2], 1, ae_v_len(2,n), alpha);
    }
    alpha = ae_complex_from_d(beta);
    x->ptr.p_complex[1] = alpha;
    
    /*
     * Scale back
     */
    x->ptr.p_complex[1] = ae_c_mul_d(x->ptr.p_complex[1],s);
}


/*************************************************************************
Application of an elementary reflection to a rectangular matrix of size MxN

The  algorithm  pre-multiplies  the  matrix  by  an  elementary reflection
transformation  which  is  given  by  column  V  and  scalar  Tau (see the
description of the GenerateReflection). Not the whole matrix  but  only  a
part of it is transformed (rows from M1 to M2, columns from N1 to N2). Only
the elements of this submatrix are changed.

Note: the matrix is multiplied by H, not by H'.   If  it  is  required  to
multiply the matrix by H', it is necessary to pass Conj(Tau) instead of Tau.

Input parameters:
    C       -   matrix to be transformed.
    Tau     -   scalar defining transformation.
    V       -   column defining transformation.
                Array whose index ranges within [1..M2-M1+1]
    M1, M2  -   range of rows to be transformed.
    N1, N2  -   range of columns to be transformed.
    WORK    -   working array whose index goes from N1 to N2.

Output parameters:
    C       -   the result of multiplying the input matrix C by the
                transformation matrix which is given by Tau and V.
                If N1>N2 or M1>M2, C is not modified.

  -- LAPACK auxiliary routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     September 30, 1994
*************************************************************************/
void complexapplyreflectionfromtheleft(/* Complex */ ae_matrix* c,
     ae_complex tau,
     /* Complex */ ae_vector* v,
     ae_int_t m1,
     ae_int_t m2,
     ae_int_t n1,
     ae_int_t n2,
     /* Complex */ ae_vector* work,
     ae_state *_state)
{
    ae_complex t;
    ae_int_t i;


    if( (ae_c_eq_d(tau,(double)(0))||n1>n2)||m1>m2 )
    {
        return;
    }
    
    /*
     * w := C^T * conj(v)
     */
    for(i=n1; i<=n2; i++)
    {
        work->ptr.p_complex[i] = ae_complex_from_i(0);
    }
    for(i=m1; i<=m2; i++)
    {
        t = ae_c_conj(v->ptr.p_complex[i+1-m1], _state);
        ae_v_caddc(&work->ptr.p_complex[n1], 1, &c->ptr.pp_complex[i][n1], 1, "N", ae_v_len(n1,n2), t);
    }
    
    /*
     * C := C - tau * v * w^T
     */
    for(i=m1; i<=m2; i++)
    {
        t = ae_c_mul(v->ptr.p_complex[i-m1+1],tau);
        ae_v_csubc(&c->ptr.pp_complex[i][n1], 1, &work->ptr.p_complex[n1], 1, "N", ae_v_len(n1,n2), t);
    }
}


/*************************************************************************
Application of an elementary reflection to a rectangular matrix of size MxN

The  algorithm  post-multiplies  the  matrix  by  an elementary reflection
transformation  which  is  given  by  column  V  and  scalar  Tau (see the
description  of  the  GenerateReflection). Not the whole matrix but only a
part  of  it  is  transformed (rows from M1 to M2, columns from N1 to N2).
Only the elements of this submatrix are changed.

Input parameters:
    C       -   matrix to be transformed.
    Tau     -   scalar defining transformation.
    V       -   column defining transformation.
                Array whose index ranges within [1..N2-N1+1]
    M1, M2  -   range of rows to be transformed.
    N1, N2  -   range of columns to be transformed.
    WORK    -   working array whose index goes from M1 to M2.

Output parameters:
    C       -   the result of multiplying the input matrix C by the
                transformation matrix which is given by Tau and V.
                If N1>N2 or M1>M2, C is not modified.

  -- LAPACK auxiliary routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     September 30, 1994
*************************************************************************/
void complexapplyreflectionfromtheright(/* Complex */ ae_matrix* c,
     ae_complex tau,
     /* Complex */ ae_vector* v,
     ae_int_t m1,
     ae_int_t m2,
     ae_int_t n1,
     ae_int_t n2,
     /* Complex */ ae_vector* work,
     ae_state *_state)
{
    ae_complex t;
    ae_int_t i;
    ae_int_t vm;


    if( (ae_c_eq_d(tau,(double)(0))||n1>n2)||m1>m2 )
    {
        return;
    }
    
    /*
     * w := C * v
     */
    vm = n2-n1+1;
    for(i=m1; i<=m2; i++)
    {
        t = ae_v_cdotproduct(&c->ptr.pp_complex[i][n1], 1, "N", &v->ptr.p_complex[1], 1, "N", ae_v_len(n1,n2));
        work->ptr.p_complex[i] = t;
    }
    
    /*
     * C := C - w * conj(v^T)
     */
    ae_v_cmove(&v->ptr.p_complex[1], 1, &v->ptr.p_complex[1], 1, "Conj", ae_v_len(1,vm));
    for(i=m1; i<=m2; i++)
    {
        t = ae_c_mul(work->ptr.p_complex[i],tau);
        ae_v_csubc(&c->ptr.pp_complex[i][n1], 1, &v->ptr.p_complex[1], 1, "N", ae_v_len(n1,n2), t);
    }
    ae_v_cmove(&v->ptr.p_complex[1], 1, &v->ptr.p_complex[1], 1, "Conj", ae_v_len(1,vm));
}


/*$ End $*/
