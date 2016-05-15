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
#include "reflections.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Generation of an elementary reflection transformation

The subroutine generates elementary reflection H of order N, so that, for
a given X, the following equality holds true:

    ( X(1) )   ( Beta )
H * (  ..  ) = (  0   )
    ( X(n) )   (  0   )

where
              ( V(1) )
H = 1 - Tau * (  ..  ) * ( V(1), ..., V(n) )
              ( V(n) )

where the first component of vector V equals 1.

Input parameters:
    X   -   vector. Array whose index ranges within [1..N].
    N   -   reflection order.

Output parameters:
    X   -   components from 2 to N are replaced with vector V.
            The first component is replaced with parameter Beta.
    Tau -   scalar value Tau. If X is a null vector, Tau equals 0,
            otherwise 1 <= Tau <= 2.

This subroutine is the modification of the DLARFG subroutines from
the LAPACK library.

MODIFICATIONS:
    24.12.2005 sign(Alpha) was replaced with an analogous to the Fortran SIGN code.

  -- LAPACK auxiliary routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     September 30, 1994
*************************************************************************/
void generatereflection(/* Real    */ ae_vector* x,
     ae_int_t n,
     double* tau,
     ae_state *_state)
{
    ae_int_t j;
    double alpha;
    double xnorm;
    double v;
    double beta;
    double mx;
    double s;

    *tau = 0;

    if( n<=1 )
    {
        *tau = (double)(0);
        return;
    }
    
    /*
     * Scale if needed (to avoid overflow/underflow during intermediate
     * calculations).
     */
    mx = (double)(0);
    for(j=1; j<=n; j++)
    {
        mx = ae_maxreal(ae_fabs(x->ptr.p_double[j], _state), mx, _state);
    }
    s = (double)(1);
    if( ae_fp_neq(mx,(double)(0)) )
    {
        if( ae_fp_less_eq(mx,ae_minrealnumber/ae_machineepsilon) )
        {
            s = ae_minrealnumber/ae_machineepsilon;
            v = 1/s;
            ae_v_muld(&x->ptr.p_double[1], 1, ae_v_len(1,n), v);
            mx = mx*v;
        }
        else
        {
            if( ae_fp_greater_eq(mx,ae_maxrealnumber*ae_machineepsilon) )
            {
                s = ae_maxrealnumber*ae_machineepsilon;
                v = 1/s;
                ae_v_muld(&x->ptr.p_double[1], 1, ae_v_len(1,n), v);
                mx = mx*v;
            }
        }
    }
    
    /*
     * XNORM = DNRM2( N-1, X, INCX )
     */
    alpha = x->ptr.p_double[1];
    xnorm = (double)(0);
    if( ae_fp_neq(mx,(double)(0)) )
    {
        for(j=2; j<=n; j++)
        {
            xnorm = xnorm+ae_sqr(x->ptr.p_double[j]/mx, _state);
        }
        xnorm = ae_sqrt(xnorm, _state)*mx;
    }
    if( ae_fp_eq(xnorm,(double)(0)) )
    {
        
        /*
         * H  =  I
         */
        *tau = (double)(0);
        x->ptr.p_double[1] = x->ptr.p_double[1]*s;
        return;
    }
    
    /*
     * general case
     */
    mx = ae_maxreal(ae_fabs(alpha, _state), ae_fabs(xnorm, _state), _state);
    beta = -mx*ae_sqrt(ae_sqr(alpha/mx, _state)+ae_sqr(xnorm/mx, _state), _state);
    if( ae_fp_less(alpha,(double)(0)) )
    {
        beta = -beta;
    }
    *tau = (beta-alpha)/beta;
    v = 1/(alpha-beta);
    ae_v_muld(&x->ptr.p_double[2], 1, ae_v_len(2,n), v);
    x->ptr.p_double[1] = beta;
    
    /*
     * Scale back outputs
     */
    x->ptr.p_double[1] = x->ptr.p_double[1]*s;
}


/*************************************************************************
Application of an elementary reflection to a rectangular matrix of size MxN

The algorithm pre-multiplies the matrix by an elementary reflection transformation
which is given by column V and scalar Tau (see the description of the
GenerateReflection procedure). Not the whole matrix but only a part of it
is transformed (rows from M1 to M2, columns from N1 to N2). Only the elements
of this submatrix are changed.

Input parameters:
    C       -   matrix to be transformed.
    Tau     -   scalar defining the transformation.
    V       -   column defining the transformation.
                Array whose index ranges within [1..M2-M1+1].
    M1, M2  -   range of rows to be transformed.
    N1, N2  -   range of columns to be transformed.
    WORK    -   working array whose indexes goes from N1 to N2.

Output parameters:
    C       -   the result of multiplying the input matrix C by the
                transformation matrix which is given by Tau and V.
                If N1>N2 or M1>M2, C is not modified.

  -- LAPACK auxiliary routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     September 30, 1994
*************************************************************************/
void applyreflectionfromtheleft(/* Real    */ ae_matrix* c,
     double tau,
     /* Real    */ ae_vector* v,
     ae_int_t m1,
     ae_int_t m2,
     ae_int_t n1,
     ae_int_t n2,
     /* Real    */ ae_vector* work,
     ae_state *_state)
{
    double t;
    ae_int_t i;


    if( (ae_fp_eq(tau,(double)(0))||n1>n2)||m1>m2 )
    {
        return;
    }
    
    /*
     * w := C' * v
     */
    for(i=n1; i<=n2; i++)
    {
        work->ptr.p_double[i] = (double)(0);
    }
    for(i=m1; i<=m2; i++)
    {
        t = v->ptr.p_double[i+1-m1];
        ae_v_addd(&work->ptr.p_double[n1], 1, &c->ptr.pp_double[i][n1], 1, ae_v_len(n1,n2), t);
    }
    
    /*
     * C := C - tau * v * w'
     */
    for(i=m1; i<=m2; i++)
    {
        t = v->ptr.p_double[i-m1+1]*tau;
        ae_v_subd(&c->ptr.pp_double[i][n1], 1, &work->ptr.p_double[n1], 1, ae_v_len(n1,n2), t);
    }
}


/*************************************************************************
Application of an elementary reflection to a rectangular matrix of size MxN

The algorithm post-multiplies the matrix by an elementary reflection transformation
which is given by column V and scalar Tau (see the description of the
GenerateReflection procedure). Not the whole matrix but only a part of it
is transformed (rows from M1 to M2, columns from N1 to N2). Only the
elements of this submatrix are changed.

Input parameters:
    C       -   matrix to be transformed.
    Tau     -   scalar defining the transformation.
    V       -   column defining the transformation.
                Array whose index ranges within [1..N2-N1+1].
    M1, M2  -   range of rows to be transformed.
    N1, N2  -   range of columns to be transformed.
    WORK    -   working array whose indexes goes from M1 to M2.

Output parameters:
    C       -   the result of multiplying the input matrix C by the
                transformation matrix which is given by Tau and V.
                If N1>N2 or M1>M2, C is not modified.

  -- LAPACK auxiliary routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     September 30, 1994
*************************************************************************/
void applyreflectionfromtheright(/* Real    */ ae_matrix* c,
     double tau,
     /* Real    */ ae_vector* v,
     ae_int_t m1,
     ae_int_t m2,
     ae_int_t n1,
     ae_int_t n2,
     /* Real    */ ae_vector* work,
     ae_state *_state)
{
    double t;
    ae_int_t i;
    ae_int_t vm;


    if( (ae_fp_eq(tau,(double)(0))||n1>n2)||m1>m2 )
    {
        return;
    }
    vm = n2-n1+1;
    for(i=m1; i<=m2; i++)
    {
        t = ae_v_dotproduct(&c->ptr.pp_double[i][n1], 1, &v->ptr.p_double[1], 1, ae_v_len(n1,n2));
        t = t*tau;
        ae_v_subd(&c->ptr.pp_double[i][n1], 1, &v->ptr.p_double[1], 1, ae_v_len(n1,n2), t);
    }
    
    /*
     * This line is necessary to avoid spurious compiler warnings
     */
    touchint(&vm, _state);
}


/*$ End $*/
