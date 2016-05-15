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
#include "apserv.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
This function is used to set error flags  during  unit  tests.  When  COND
parameter is True, FLAG variable is  set  to  True.  When  COND is  False,
FLAG is unchanged.

The purpose of this function is to have single  point  where  failures  of
unit tests can be detected.

This function returns value of COND.
*************************************************************************/
ae_bool seterrorflag(ae_bool* flag, ae_bool cond, ae_state *_state)
{
    ae_bool result;


    if( cond )
    {
        *flag = ae_true;
    }
    result = cond;
    return result;
}


/*************************************************************************
Internally calls SetErrorFlag() with condition:

    Abs(Val-RefVal)>Tol*Max(Abs(RefVal),S)
    
This function is used to test relative error in Val against  RefVal,  with
relative error being replaced by absolute when scale  of  RefVal  is  less
than S.

This function returns value of COND.
*************************************************************************/
ae_bool seterrorflagdiff(ae_bool* flag,
     double val,
     double refval,
     double tol,
     double s,
     ae_state *_state)
{
    ae_bool result;


    result = seterrorflag(flag, ae_fp_greater(ae_fabs(val-refval, _state),tol*ae_maxreal(ae_fabs(refval, _state), s, _state)), _state);
    return result;
}


/*************************************************************************
The function "touches" integer - it is used  to  avoid  compiler  messages
about unused variables (in rare cases when we do NOT want to remove  these
variables).

  -- ALGLIB --
     Copyright 17.09.2012 by Bochkanov Sergey
*************************************************************************/
void touchint(ae_int_t* a, ae_state *_state)
{


}


/*************************************************************************
The function "touches" real   -  it is used  to  avoid  compiler  messages
about unused variables (in rare cases when we do NOT want to remove  these
variables).

  -- ALGLIB --
     Copyright 17.09.2012 by Bochkanov Sergey
*************************************************************************/
void touchreal(double* a, ae_state *_state)
{


}


/*************************************************************************
The function performs zero-coalescing on real value.

NOTE: no check is performed for B<>0

  -- ALGLIB --
     Copyright 18.05.2015 by Bochkanov Sergey
*************************************************************************/
double coalesce(double a, double b, ae_state *_state)
{
    double result;


    result = a;
    if( ae_fp_eq(a,0.0) )
    {
        result = b;
    }
    return result;
}


/*************************************************************************
The function convert integer value to real value.

  -- ALGLIB --
     Copyright 17.09.2012 by Bochkanov Sergey
*************************************************************************/
double inttoreal(ae_int_t a, ae_state *_state)
{
    double result;


    result = (double)(a);
    return result;
}


/*************************************************************************
The function calculates binary logarithm.

NOTE: it costs twice as much as Ln(x)

  -- ALGLIB --
     Copyright 17.09.2012 by Bochkanov Sergey
*************************************************************************/
double logbase2(double x, ae_state *_state)
{
    double result;


    result = ae_log(x, _state)/ae_log((double)(2), _state);
    return result;
}


/*************************************************************************
This function compares two numbers for approximate equality, with tolerance
to errors as large as max(|a|,|b|)*tol.


  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
ae_bool approxequalrel(double a, double b, double tol, ae_state *_state)
{
    ae_bool result;


    result = ae_fp_less_eq(ae_fabs(a-b, _state),ae_maxreal(ae_fabs(a, _state), ae_fabs(b, _state), _state)*tol);
    return result;
}


/*************************************************************************
This  function  generates  1-dimensional  general  interpolation task with
moderate Lipshitz constant (close to 1.0)

If N=1 then suborutine generates only one point at the middle of [A,B]

  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
void taskgenint1d(double a,
     double b,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{
    ae_int_t i;
    double h;

    ae_vector_clear(x);
    ae_vector_clear(y);

    ae_assert(n>=1, "TaskGenInterpolationEqdist1D: N<1!", _state);
    ae_vector_set_length(x, n, _state);
    ae_vector_set_length(y, n, _state);
    if( n>1 )
    {
        x->ptr.p_double[0] = a;
        y->ptr.p_double[0] = 2*ae_randomreal(_state)-1;
        h = (b-a)/(n-1);
        for(i=1; i<=n-1; i++)
        {
            if( i!=n-1 )
            {
                x->ptr.p_double[i] = a+(i+0.2*(2*ae_randomreal(_state)-1))*h;
            }
            else
            {
                x->ptr.p_double[i] = b;
            }
            y->ptr.p_double[i] = y->ptr.p_double[i-1]+(2*ae_randomreal(_state)-1)*(x->ptr.p_double[i]-x->ptr.p_double[i-1]);
        }
    }
    else
    {
        x->ptr.p_double[0] = 0.5*(a+b);
        y->ptr.p_double[0] = 2*ae_randomreal(_state)-1;
    }
}


/*************************************************************************
This function generates  1-dimensional equidistant interpolation task with
moderate Lipshitz constant (close to 1.0)

If N=1 then suborutine generates only one point at the middle of [A,B]

  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
void taskgenint1dequidist(double a,
     double b,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{
    ae_int_t i;
    double h;

    ae_vector_clear(x);
    ae_vector_clear(y);

    ae_assert(n>=1, "TaskGenInterpolationEqdist1D: N<1!", _state);
    ae_vector_set_length(x, n, _state);
    ae_vector_set_length(y, n, _state);
    if( n>1 )
    {
        x->ptr.p_double[0] = a;
        y->ptr.p_double[0] = 2*ae_randomreal(_state)-1;
        h = (b-a)/(n-1);
        for(i=1; i<=n-1; i++)
        {
            x->ptr.p_double[i] = a+i*h;
            y->ptr.p_double[i] = y->ptr.p_double[i-1]+(2*ae_randomreal(_state)-1)*h;
        }
    }
    else
    {
        x->ptr.p_double[0] = 0.5*(a+b);
        y->ptr.p_double[0] = 2*ae_randomreal(_state)-1;
    }
}


/*************************************************************************
This function generates  1-dimensional Chebyshev-1 interpolation task with
moderate Lipshitz constant (close to 1.0)

If N=1 then suborutine generates only one point at the middle of [A,B]

  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
void taskgenint1dcheb1(double a,
     double b,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{
    ae_int_t i;

    ae_vector_clear(x);
    ae_vector_clear(y);

    ae_assert(n>=1, "TaskGenInterpolation1DCheb1: N<1!", _state);
    ae_vector_set_length(x, n, _state);
    ae_vector_set_length(y, n, _state);
    if( n>1 )
    {
        for(i=0; i<=n-1; i++)
        {
            x->ptr.p_double[i] = 0.5*(b+a)+0.5*(b-a)*ae_cos(ae_pi*(2*i+1)/(2*n), _state);
            if( i==0 )
            {
                y->ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            else
            {
                y->ptr.p_double[i] = y->ptr.p_double[i-1]+(2*ae_randomreal(_state)-1)*(x->ptr.p_double[i]-x->ptr.p_double[i-1]);
            }
        }
    }
    else
    {
        x->ptr.p_double[0] = 0.5*(a+b);
        y->ptr.p_double[0] = 2*ae_randomreal(_state)-1;
    }
}


/*************************************************************************
This function generates  1-dimensional Chebyshev-2 interpolation task with
moderate Lipshitz constant (close to 1.0)

If N=1 then suborutine generates only one point at the middle of [A,B]

  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
void taskgenint1dcheb2(double a,
     double b,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{
    ae_int_t i;

    ae_vector_clear(x);
    ae_vector_clear(y);

    ae_assert(n>=1, "TaskGenInterpolation1DCheb2: N<1!", _state);
    ae_vector_set_length(x, n, _state);
    ae_vector_set_length(y, n, _state);
    if( n>1 )
    {
        for(i=0; i<=n-1; i++)
        {
            x->ptr.p_double[i] = 0.5*(b+a)+0.5*(b-a)*ae_cos(ae_pi*i/(n-1), _state);
            if( i==0 )
            {
                y->ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            else
            {
                y->ptr.p_double[i] = y->ptr.p_double[i-1]+(2*ae_randomreal(_state)-1)*(x->ptr.p_double[i]-x->ptr.p_double[i-1]);
            }
        }
    }
    else
    {
        x->ptr.p_double[0] = 0.5*(a+b);
        y->ptr.p_double[0] = 2*ae_randomreal(_state)-1;
    }
}


/*************************************************************************
This function checks that all values from X[] are distinct. It does more
than just usual floating point comparison:
* first, it calculates max(X) and min(X)
* second, it maps X[] from [min,max] to [1,2]
* only at this stage actual comparison is done

The meaning of such check is to ensure that all values are "distinct enough"
and will not cause interpolation subroutine to fail.

NOTE:
    X[] must be sorted by ascending (subroutine ASSERT's it)

  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
ae_bool aredistinct(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_state *_state)
{
    double a;
    double b;
    ae_int_t i;
    ae_bool nonsorted;
    ae_bool result;


    ae_assert(n>=1, "APSERVAreDistinct: internal error (N<1)", _state);
    if( n==1 )
    {
        
        /*
         * everything is alright, it is up to caller to decide whether it
         * can interpolate something with just one point
         */
        result = ae_true;
        return result;
    }
    a = x->ptr.p_double[0];
    b = x->ptr.p_double[0];
    nonsorted = ae_false;
    for(i=1; i<=n-1; i++)
    {
        a = ae_minreal(a, x->ptr.p_double[i], _state);
        b = ae_maxreal(b, x->ptr.p_double[i], _state);
        nonsorted = nonsorted||ae_fp_greater_eq(x->ptr.p_double[i-1],x->ptr.p_double[i]);
    }
    ae_assert(!nonsorted, "APSERVAreDistinct: internal error (not sorted)", _state);
    for(i=1; i<=n-1; i++)
    {
        if( ae_fp_eq((x->ptr.p_double[i]-a)/(b-a)+1,(x->ptr.p_double[i-1]-a)/(b-a)+1) )
        {
            result = ae_false;
            return result;
        }
    }
    result = ae_true;
    return result;
}


/*************************************************************************
This function checks that two boolean values are the same (both  are  True 
or both are False).

  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
ae_bool aresameboolean(ae_bool v1, ae_bool v2, ae_state *_state)
{
    ae_bool result;


    result = (v1&&v2)||(!v1&&!v2);
    return result;
}


/*************************************************************************
If Length(X)<N, resizes X

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
void bvectorsetlengthatleast(/* Boolean */ ae_vector* x,
     ae_int_t n,
     ae_state *_state)
{


    if( x->cnt<n )
    {
        ae_vector_set_length(x, n, _state);
    }
}


/*************************************************************************
If Length(X)<N, resizes X

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
void ivectorsetlengthatleast(/* Integer */ ae_vector* x,
     ae_int_t n,
     ae_state *_state)
{


    if( x->cnt<n )
    {
        ae_vector_set_length(x, n, _state);
    }
}


/*************************************************************************
If Length(X)<N, resizes X

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
void rvectorsetlengthatleast(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_state *_state)
{


    if( x->cnt<n )
    {
        ae_vector_set_length(x, n, _state);
    }
}


/*************************************************************************
If Cols(X)<N or Rows(X)<M, resizes X

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
void rmatrixsetlengthatleast(/* Real    */ ae_matrix* x,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state)
{


    if( m>0&&n>0 )
    {
        if( x->rows<m||x->cols<n )
        {
            ae_matrix_set_length(x, m, n, _state);
        }
    }
}


/*************************************************************************
Resizes X and:
* preserves old contents of X
* fills new elements by zeros

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
void rmatrixresize(/* Real    */ ae_matrix* x,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix oldx;
    ae_int_t i;
    ae_int_t j;
    ae_int_t m2;
    ae_int_t n2;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&oldx, 0, 0, DT_REAL, _state);

    m2 = x->rows;
    n2 = x->cols;
    ae_swap_matrices(x, &oldx);
    ae_matrix_set_length(x, m, n, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( i<m2&&j<n2 )
            {
                x->ptr.pp_double[i][j] = oldx.ptr.pp_double[i][j];
            }
            else
            {
                x->ptr.pp_double[i][j] = 0.0;
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Resizes X and:
* preserves old contents of X
* fills new elements by zeros

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
void imatrixresize(/* Integer */ ae_matrix* x,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix oldx;
    ae_int_t i;
    ae_int_t j;
    ae_int_t m2;
    ae_int_t n2;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&oldx, 0, 0, DT_INT, _state);

    m2 = x->rows;
    n2 = x->cols;
    ae_swap_matrices(x, &oldx);
    ae_matrix_set_length(x, m, n, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( i<m2&&j<n2 )
            {
                x->ptr.pp_int[i][j] = oldx.ptr.pp_int[i][j];
            }
            else
            {
                x->ptr.pp_int[i][j] = 0;
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function checks that length(X) is at least N and first N values  from
X[] are finite

  -- ALGLIB --
     Copyright 18.06.2010 by Bochkanov Sergey
*************************************************************************/
ae_bool isfinitevector(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_bool result;


    ae_assert(n>=0, "APSERVIsFiniteVector: internal error (N<0)", _state);
    if( n==0 )
    {
        result = ae_true;
        return result;
    }
    if( x->cnt<n )
    {
        result = ae_false;
        return result;
    }
    for(i=0; i<=n-1; i++)
    {
        if( !ae_isfinite(x->ptr.p_double[i], _state) )
        {
            result = ae_false;
            return result;
        }
    }
    result = ae_true;
    return result;
}


/*************************************************************************
This function checks that first N values from X[] are finite

  -- ALGLIB --
     Copyright 18.06.2010 by Bochkanov Sergey
*************************************************************************/
ae_bool isfinitecvector(/* Complex */ ae_vector* z,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_bool result;


    ae_assert(n>=0, "APSERVIsFiniteCVector: internal error (N<0)", _state);
    for(i=0; i<=n-1; i++)
    {
        if( !ae_isfinite(z->ptr.p_complex[i].x, _state)||!ae_isfinite(z->ptr.p_complex[i].y, _state) )
        {
            result = ae_false;
            return result;
        }
    }
    result = ae_true;
    return result;
}


/*************************************************************************
This function checks that size of X is at least MxN and values from
X[0..M-1,0..N-1] are finite.

  -- ALGLIB --
     Copyright 18.06.2010 by Bochkanov Sergey
*************************************************************************/
ae_bool apservisfinitematrix(/* Real    */ ae_matrix* x,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_bool result;


    ae_assert(n>=0, "APSERVIsFiniteMatrix: internal error (N<0)", _state);
    ae_assert(m>=0, "APSERVIsFiniteMatrix: internal error (M<0)", _state);
    if( m==0||n==0 )
    {
        result = ae_true;
        return result;
    }
    if( x->rows<m||x->cols<n )
    {
        result = ae_false;
        return result;
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( !ae_isfinite(x->ptr.pp_double[i][j], _state) )
            {
                result = ae_false;
                return result;
            }
        }
    }
    result = ae_true;
    return result;
}


/*************************************************************************
This function checks that all values from X[0..M-1,0..N-1] are finite

  -- ALGLIB --
     Copyright 18.06.2010 by Bochkanov Sergey
*************************************************************************/
ae_bool apservisfinitecmatrix(/* Complex */ ae_matrix* x,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_bool result;


    ae_assert(n>=0, "APSERVIsFiniteCMatrix: internal error (N<0)", _state);
    ae_assert(m>=0, "APSERVIsFiniteCMatrix: internal error (M<0)", _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( !ae_isfinite(x->ptr.pp_complex[i][j].x, _state)||!ae_isfinite(x->ptr.pp_complex[i][j].y, _state) )
            {
                result = ae_false;
                return result;
            }
        }
    }
    result = ae_true;
    return result;
}


/*************************************************************************
This function checks that size of X is at least NxN and all values from
upper/lower triangle of X[0..N-1,0..N-1] are finite

  -- ALGLIB --
     Copyright 18.06.2010 by Bochkanov Sergey
*************************************************************************/
ae_bool isfinitertrmatrix(/* Real    */ ae_matrix* x,
     ae_int_t n,
     ae_bool isupper,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j1;
    ae_int_t j2;
    ae_int_t j;
    ae_bool result;


    ae_assert(n>=0, "APSERVIsFiniteRTRMatrix: internal error (N<0)", _state);
    if( n==0 )
    {
        result = ae_true;
        return result;
    }
    if( x->rows<n||x->cols<n )
    {
        result = ae_false;
        return result;
    }
    for(i=0; i<=n-1; i++)
    {
        if( isupper )
        {
            j1 = i;
            j2 = n-1;
        }
        else
        {
            j1 = 0;
            j2 = i;
        }
        for(j=j1; j<=j2; j++)
        {
            if( !ae_isfinite(x->ptr.pp_double[i][j], _state) )
            {
                result = ae_false;
                return result;
            }
        }
    }
    result = ae_true;
    return result;
}


/*************************************************************************
This function checks that all values from upper/lower triangle of
X[0..N-1,0..N-1] are finite

  -- ALGLIB --
     Copyright 18.06.2010 by Bochkanov Sergey
*************************************************************************/
ae_bool apservisfinitectrmatrix(/* Complex */ ae_matrix* x,
     ae_int_t n,
     ae_bool isupper,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j1;
    ae_int_t j2;
    ae_int_t j;
    ae_bool result;


    ae_assert(n>=0, "APSERVIsFiniteCTRMatrix: internal error (N<0)", _state);
    for(i=0; i<=n-1; i++)
    {
        if( isupper )
        {
            j1 = i;
            j2 = n-1;
        }
        else
        {
            j1 = 0;
            j2 = i;
        }
        for(j=j1; j<=j2; j++)
        {
            if( !ae_isfinite(x->ptr.pp_complex[i][j].x, _state)||!ae_isfinite(x->ptr.pp_complex[i][j].y, _state) )
            {
                result = ae_false;
                return result;
            }
        }
    }
    result = ae_true;
    return result;
}


/*************************************************************************
This function checks that all values from X[0..M-1,0..N-1] are  finite  or
NaN's.

  -- ALGLIB --
     Copyright 18.06.2010 by Bochkanov Sergey
*************************************************************************/
ae_bool apservisfiniteornanmatrix(/* Real    */ ae_matrix* x,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_bool result;


    ae_assert(n>=0, "APSERVIsFiniteOrNaNMatrix: internal error (N<0)", _state);
    ae_assert(m>=0, "APSERVIsFiniteOrNaNMatrix: internal error (M<0)", _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( !(ae_isfinite(x->ptr.pp_double[i][j], _state)||ae_isnan(x->ptr.pp_double[i][j], _state)) )
            {
                result = ae_false;
                return result;
            }
        }
    }
    result = ae_true;
    return result;
}


/*************************************************************************
Safe sqrt(x^2+y^2)

  -- ALGLIB --
     Copyright by Bochkanov Sergey
*************************************************************************/
double safepythag2(double x, double y, ae_state *_state)
{
    double w;
    double xabs;
    double yabs;
    double z;
    double result;


    xabs = ae_fabs(x, _state);
    yabs = ae_fabs(y, _state);
    w = ae_maxreal(xabs, yabs, _state);
    z = ae_minreal(xabs, yabs, _state);
    if( ae_fp_eq(z,(double)(0)) )
    {
        result = w;
    }
    else
    {
        result = w*ae_sqrt(1+ae_sqr(z/w, _state), _state);
    }
    return result;
}


/*************************************************************************
Safe sqrt(x^2+y^2)

  -- ALGLIB --
     Copyright by Bochkanov Sergey
*************************************************************************/
double safepythag3(double x, double y, double z, ae_state *_state)
{
    double w;
    double result;


    w = ae_maxreal(ae_fabs(x, _state), ae_maxreal(ae_fabs(y, _state), ae_fabs(z, _state), _state), _state);
    if( ae_fp_eq(w,(double)(0)) )
    {
        result = (double)(0);
        return result;
    }
    x = x/w;
    y = y/w;
    z = z/w;
    result = w*ae_sqrt(ae_sqr(x, _state)+ae_sqr(y, _state)+ae_sqr(z, _state), _state);
    return result;
}


/*************************************************************************
Safe division.

This function attempts to calculate R=X/Y without overflow.

It returns:
* +1, if abs(X/Y)>=MaxRealNumber or undefined - overflow-like situation
      (no overlfow is generated, R is either NAN, PosINF, NegINF)
*  0, if MinRealNumber<abs(X/Y)<MaxRealNumber or X=0, Y<>0
      (R contains result, may be zero)
* -1, if 0<abs(X/Y)<MinRealNumber - underflow-like situation
      (R contains zero; it corresponds to underflow)

No overflow is generated in any case.

  -- ALGLIB --
     Copyright by Bochkanov Sergey
*************************************************************************/
ae_int_t saferdiv(double x, double y, double* r, ae_state *_state)
{
    ae_int_t result;

    *r = 0;

    
    /*
     * Two special cases:
     * * Y=0
     * * X=0 and Y<>0
     */
    if( ae_fp_eq(y,(double)(0)) )
    {
        result = 1;
        if( ae_fp_eq(x,(double)(0)) )
        {
            *r = _state->v_nan;
        }
        if( ae_fp_greater(x,(double)(0)) )
        {
            *r = _state->v_posinf;
        }
        if( ae_fp_less(x,(double)(0)) )
        {
            *r = _state->v_neginf;
        }
        return result;
    }
    if( ae_fp_eq(x,(double)(0)) )
    {
        *r = (double)(0);
        result = 0;
        return result;
    }
    
    /*
     * make Y>0
     */
    if( ae_fp_less(y,(double)(0)) )
    {
        x = -x;
        y = -y;
    }
    
    /*
     *
     */
    if( ae_fp_greater_eq(y,(double)(1)) )
    {
        *r = x/y;
        if( ae_fp_less_eq(ae_fabs(*r, _state),ae_minrealnumber) )
        {
            result = -1;
            *r = (double)(0);
        }
        else
        {
            result = 0;
        }
    }
    else
    {
        if( ae_fp_greater_eq(ae_fabs(x, _state),ae_maxrealnumber*y) )
        {
            if( ae_fp_greater(x,(double)(0)) )
            {
                *r = _state->v_posinf;
            }
            else
            {
                *r = _state->v_neginf;
            }
            result = 1;
        }
        else
        {
            *r = x/y;
            result = 0;
        }
    }
    return result;
}


/*************************************************************************
This function calculates "safe" min(X/Y,V) for positive finite X, Y, V.
No overflow is generated in any case.

  -- ALGLIB --
     Copyright by Bochkanov Sergey
*************************************************************************/
double safeminposrv(double x, double y, double v, ae_state *_state)
{
    double r;
    double result;


    if( ae_fp_greater_eq(y,(double)(1)) )
    {
        
        /*
         * Y>=1, we can safely divide by Y
         */
        r = x/y;
        result = v;
        if( ae_fp_greater(v,r) )
        {
            result = r;
        }
        else
        {
            result = v;
        }
    }
    else
    {
        
        /*
         * Y<1, we can safely multiply by Y
         */
        if( ae_fp_less(x,v*y) )
        {
            result = x/y;
        }
        else
        {
            result = v;
        }
    }
    return result;
}


/*************************************************************************
This function makes periodic mapping of X to [A,B].

It accepts X, A, B (A>B). It returns T which lies in  [A,B] and integer K,
such that X = T + K*(B-A).

NOTES:
* K is represented as real value, although actually it is integer
* T is guaranteed to be in [A,B]
* T replaces X

  -- ALGLIB --
     Copyright by Bochkanov Sergey
*************************************************************************/
void apperiodicmap(double* x,
     double a,
     double b,
     double* k,
     ae_state *_state)
{

    *k = 0;

    ae_assert(ae_fp_less(a,b), "APPeriodicMap: internal error!", _state);
    *k = (double)(ae_ifloor((*x-a)/(b-a), _state));
    *x = *x-*k*(b-a);
    while(ae_fp_less(*x,a))
    {
        *x = *x+(b-a);
        *k = *k-1;
    }
    while(ae_fp_greater(*x,b))
    {
        *x = *x-(b-a);
        *k = *k+1;
    }
    *x = ae_maxreal(*x, a, _state);
    *x = ae_minreal(*x, b, _state);
}


/*************************************************************************
Returns random normal number using low-quality system-provided generator

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
double randomnormal(ae_state *_state)
{
    double u;
    double v;
    double s;
    double result;


    for(;;)
    {
        u = 2*ae_randomreal(_state)-1;
        v = 2*ae_randomreal(_state)-1;
        s = ae_sqr(u, _state)+ae_sqr(v, _state);
        if( ae_fp_greater(s,(double)(0))&&ae_fp_less(s,(double)(1)) )
        {
            
            /*
             * two Sqrt's instead of one to
             * avoid overflow when S is too small
             */
            s = ae_sqrt(-2*ae_log(s, _state), _state)/ae_sqrt(s, _state);
            result = u*s;
            break;
        }
    }
    return result;
}


/*************************************************************************
Generates random unit vector using low-quality system-provided generator.
Reallocates array if its size is too short.

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
void randomunit(ae_int_t n, /* Real    */ ae_vector* x, ae_state *_state)
{
    ae_int_t i;
    double v;
    double vv;


    ae_assert(n>0, "RandomUnit: N<=0", _state);
    if( x->cnt<n )
    {
        ae_vector_set_length(x, n, _state);
    }
    do
    {
        v = 0.0;
        for(i=0; i<=n-1; i++)
        {
            vv = randomnormal(_state);
            x->ptr.p_double[i] = vv;
            v = v+vv*vv;
        }
    }
    while(ae_fp_less_eq(v,(double)(0)));
    v = 1/ae_sqrt(v, _state);
    for(i=0; i<=n-1; i++)
    {
        x->ptr.p_double[i] = x->ptr.p_double[i]*v;
    }
}


/*************************************************************************
This function is used to swap two integer values
*************************************************************************/
void swapi(ae_int_t* v0, ae_int_t* v1, ae_state *_state)
{
    ae_int_t v;


    v = *v0;
    *v0 = *v1;
    *v1 = v;
}


/*************************************************************************
This function is used to swap two real values
*************************************************************************/
void swapr(double* v0, double* v1, ae_state *_state)
{
    double v;


    v = *v0;
    *v0 = *v1;
    *v1 = v;
}


/*************************************************************************
This function is used to return maximum of three real values
*************************************************************************/
double maxreal3(double v0, double v1, double v2, ae_state *_state)
{
    double result;


    result = v0;
    if( ae_fp_less(result,v1) )
    {
        result = v1;
    }
    if( ae_fp_less(result,v2) )
    {
        result = v2;
    }
    return result;
}


/*************************************************************************
This function is used to increment value of integer variable
*************************************************************************/
void inc(ae_int_t* v, ae_state *_state)
{


    *v = *v+1;
}


/*************************************************************************
This function is used to decrement value of integer variable
*************************************************************************/
void dec(ae_int_t* v, ae_state *_state)
{


    *v = *v-1;
}


/*************************************************************************
This function performs two operations:
1. decrements value of integer variable, if it is positive
2. explicitly sets variable to zero if it is non-positive
It is used by some algorithms to decrease value of internal counters.
*************************************************************************/
void countdown(ae_int_t* v, ae_state *_state)
{


    if( *v>0 )
    {
        *v = *v-1;
    }
    else
    {
        *v = 0;
    }
}


/*************************************************************************
'bounds' value: maps X to [B1,B2]

  -- ALGLIB --
     Copyright 20.03.2009 by Bochkanov Sergey
*************************************************************************/
double boundval(double x, double b1, double b2, ae_state *_state)
{
    double result;


    if( ae_fp_less_eq(x,b1) )
    {
        result = b1;
        return result;
    }
    if( ae_fp_greater_eq(x,b2) )
    {
        result = b2;
        return result;
    }
    result = x;
    return result;
}


/*************************************************************************
Allocation of serializer: complex value
*************************************************************************/
void alloccomplex(ae_serializer* s, ae_complex v, ae_state *_state)
{


    ae_serializer_alloc_entry(s);
    ae_serializer_alloc_entry(s);
}


/*************************************************************************
Serialization: complex value
*************************************************************************/
void serializecomplex(ae_serializer* s, ae_complex v, ae_state *_state)
{


    ae_serializer_serialize_double(s, v.x, _state);
    ae_serializer_serialize_double(s, v.y, _state);
}


/*************************************************************************
Unserialization: complex value
*************************************************************************/
ae_complex unserializecomplex(ae_serializer* s, ae_state *_state)
{
    ae_complex result;


    ae_serializer_unserialize_double(s, &result.x, _state);
    ae_serializer_unserialize_double(s, &result.y, _state);
    return result;
}


/*************************************************************************
Allocation of serializer: real array
*************************************************************************/
void allocrealarray(ae_serializer* s,
     /* Real    */ ae_vector* v,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;


    if( n<0 )
    {
        n = v->cnt;
    }
    ae_serializer_alloc_entry(s);
    for(i=0; i<=n-1; i++)
    {
        ae_serializer_alloc_entry(s);
    }
}


/*************************************************************************
Serialization: complex value
*************************************************************************/
void serializerealarray(ae_serializer* s,
     /* Real    */ ae_vector* v,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;


    if( n<0 )
    {
        n = v->cnt;
    }
    ae_serializer_serialize_int(s, n, _state);
    for(i=0; i<=n-1; i++)
    {
        ae_serializer_serialize_double(s, v->ptr.p_double[i], _state);
    }
}


/*************************************************************************
Unserialization: complex value
*************************************************************************/
void unserializerealarray(ae_serializer* s,
     /* Real    */ ae_vector* v,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    double t;

    ae_vector_clear(v);

    ae_serializer_unserialize_int(s, &n, _state);
    if( n==0 )
    {
        return;
    }
    ae_vector_set_length(v, n, _state);
    for(i=0; i<=n-1; i++)
    {
        ae_serializer_unserialize_double(s, &t, _state);
        v->ptr.p_double[i] = t;
    }
}


/*************************************************************************
Allocation of serializer: Integer array
*************************************************************************/
void allocintegerarray(ae_serializer* s,
     /* Integer */ ae_vector* v,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;


    if( n<0 )
    {
        n = v->cnt;
    }
    ae_serializer_alloc_entry(s);
    for(i=0; i<=n-1; i++)
    {
        ae_serializer_alloc_entry(s);
    }
}


/*************************************************************************
Serialization: Integer array
*************************************************************************/
void serializeintegerarray(ae_serializer* s,
     /* Integer */ ae_vector* v,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;


    if( n<0 )
    {
        n = v->cnt;
    }
    ae_serializer_serialize_int(s, n, _state);
    for(i=0; i<=n-1; i++)
    {
        ae_serializer_serialize_int(s, v->ptr.p_int[i], _state);
    }
}


/*************************************************************************
Unserialization: complex value
*************************************************************************/
void unserializeintegerarray(ae_serializer* s,
     /* Integer */ ae_vector* v,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    ae_int_t t;

    ae_vector_clear(v);

    ae_serializer_unserialize_int(s, &n, _state);
    if( n==0 )
    {
        return;
    }
    ae_vector_set_length(v, n, _state);
    for(i=0; i<=n-1; i++)
    {
        ae_serializer_unserialize_int(s, &t, _state);
        v->ptr.p_int[i] = t;
    }
}


/*************************************************************************
Allocation of serializer: real matrix
*************************************************************************/
void allocrealmatrix(ae_serializer* s,
     /* Real    */ ae_matrix* v,
     ae_int_t n0,
     ae_int_t n1,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    if( n0<0 )
    {
        n0 = v->rows;
    }
    if( n1<0 )
    {
        n1 = v->cols;
    }
    ae_serializer_alloc_entry(s);
    ae_serializer_alloc_entry(s);
    for(i=0; i<=n0-1; i++)
    {
        for(j=0; j<=n1-1; j++)
        {
            ae_serializer_alloc_entry(s);
        }
    }
}


/*************************************************************************
Serialization: complex value
*************************************************************************/
void serializerealmatrix(ae_serializer* s,
     /* Real    */ ae_matrix* v,
     ae_int_t n0,
     ae_int_t n1,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    if( n0<0 )
    {
        n0 = v->rows;
    }
    if( n1<0 )
    {
        n1 = v->cols;
    }
    ae_serializer_serialize_int(s, n0, _state);
    ae_serializer_serialize_int(s, n1, _state);
    for(i=0; i<=n0-1; i++)
    {
        for(j=0; j<=n1-1; j++)
        {
            ae_serializer_serialize_double(s, v->ptr.pp_double[i][j], _state);
        }
    }
}


/*************************************************************************
Unserialization: complex value
*************************************************************************/
void unserializerealmatrix(ae_serializer* s,
     /* Real    */ ae_matrix* v,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t n0;
    ae_int_t n1;
    double t;

    ae_matrix_clear(v);

    ae_serializer_unserialize_int(s, &n0, _state);
    ae_serializer_unserialize_int(s, &n1, _state);
    if( n0==0||n1==0 )
    {
        return;
    }
    ae_matrix_set_length(v, n0, n1, _state);
    for(i=0; i<=n0-1; i++)
    {
        for(j=0; j<=n1-1; j++)
        {
            ae_serializer_unserialize_double(s, &t, _state);
            v->ptr.pp_double[i][j] = t;
        }
    }
}


/*************************************************************************
Copy integer array
*************************************************************************/
void copyintegerarray(/* Integer */ ae_vector* src,
     /* Integer */ ae_vector* dst,
     ae_state *_state)
{
    ae_int_t i;

    ae_vector_clear(dst);

    if( src->cnt>0 )
    {
        ae_vector_set_length(dst, src->cnt, _state);
        for(i=0; i<=src->cnt-1; i++)
        {
            dst->ptr.p_int[i] = src->ptr.p_int[i];
        }
    }
}


/*************************************************************************
Copy real array
*************************************************************************/
void copyrealarray(/* Real    */ ae_vector* src,
     /* Real    */ ae_vector* dst,
     ae_state *_state)
{
    ae_int_t i;

    ae_vector_clear(dst);

    if( src->cnt>0 )
    {
        ae_vector_set_length(dst, src->cnt, _state);
        for(i=0; i<=src->cnt-1; i++)
        {
            dst->ptr.p_double[i] = src->ptr.p_double[i];
        }
    }
}


/*************************************************************************
Copy real matrix
*************************************************************************/
void copyrealmatrix(/* Real    */ ae_matrix* src,
     /* Real    */ ae_matrix* dst,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(dst);

    if( src->rows>0&&src->cols>0 )
    {
        ae_matrix_set_length(dst, src->rows, src->cols, _state);
        for(i=0; i<=src->rows-1; i++)
        {
            for(j=0; j<=src->cols-1; j++)
            {
                dst->ptr.pp_double[i][j] = src->ptr.pp_double[i][j];
            }
        }
    }
}


/*************************************************************************
This function searches integer array. Elements in this array are actually
records, each NRec elements wide. Each record has unique header - NHeader
integer values, which identify it. Records are lexicographically sorted by
header.

Records are identified by their index, not offset (offset = NRec*index).

This function searches A (records with indices [I0,I1)) for a record with
header B. It returns index of this record (not offset!), or -1 on failure.

  -- ALGLIB --
     Copyright 28.03.2011 by Bochkanov Sergey
*************************************************************************/
ae_int_t recsearch(/* Integer */ ae_vector* a,
     ae_int_t nrec,
     ae_int_t nheader,
     ae_int_t i0,
     ae_int_t i1,
     /* Integer */ ae_vector* b,
     ae_state *_state)
{
    ae_int_t mididx;
    ae_int_t cflag;
    ae_int_t k;
    ae_int_t offs;
    ae_int_t result;


    result = -1;
    for(;;)
    {
        if( i0>=i1 )
        {
            break;
        }
        mididx = (i0+i1)/2;
        offs = nrec*mididx;
        cflag = 0;
        for(k=0; k<=nheader-1; k++)
        {
            if( a->ptr.p_int[offs+k]<b->ptr.p_int[k] )
            {
                cflag = -1;
                break;
            }
            if( a->ptr.p_int[offs+k]>b->ptr.p_int[k] )
            {
                cflag = 1;
                break;
            }
        }
        if( cflag==0 )
        {
            result = mididx;
            return result;
        }
        if( cflag<0 )
        {
            i0 = mididx+1;
        }
        else
        {
            i1 = mididx;
        }
    }
    return result;
}


/*************************************************************************
This function is used in parallel functions for recurrent division of large
task into two smaller tasks.

It has following properties:
* it works only for TaskSize>=2 (assertion is thrown otherwise)
* for TaskSize=2, it returns Task0=1, Task1=1
* in case TaskSize is odd,  Task0=TaskSize-1, Task1=1
* in case TaskSize is even, Task0 and Task1 are approximately TaskSize/2
  and both Task0 and Task1 are even, Task0>=Task1

  -- ALGLIB --
     Copyright 07.04.2013 by Bochkanov Sergey
*************************************************************************/
void splitlengtheven(ae_int_t tasksize,
     ae_int_t* task0,
     ae_int_t* task1,
     ae_state *_state)
{

    *task0 = 0;
    *task1 = 0;

    ae_assert(tasksize>=2, "SplitLengthEven: TaskSize<2", _state);
    if( tasksize==2 )
    {
        *task0 = 1;
        *task1 = 1;
        return;
    }
    if( tasksize%2==0 )
    {
        
        /*
         * Even division
         */
        *task0 = tasksize/2;
        *task1 = tasksize/2;
        if( *task0%2!=0 )
        {
            *task0 = *task0+1;
            *task1 = *task1-1;
        }
    }
    else
    {
        
        /*
         * Odd task size, split trailing odd part from it.
         */
        *task0 = tasksize-1;
        *task1 = 1;
    }
    ae_assert(*task0>=1, "SplitLengthEven: internal error", _state);
    ae_assert(*task1>=1, "SplitLengthEven: internal error", _state);
}


/*************************************************************************
This function is used in parallel functions for recurrent division of large
task into two smaller tasks.

It has following properties:
* it works only for TaskSize>=2 and ChunkSize>=2
  (assertion is thrown otherwise)
* Task0+Task1=TaskSize, Task0>0, Task1>0
* Task0 and Task1 are close to each other
* in case TaskSize>ChunkSize, Task0 is always divisible by ChunkSize

  -- ALGLIB --
     Copyright 07.04.2013 by Bochkanov Sergey
*************************************************************************/
void splitlength(ae_int_t tasksize,
     ae_int_t chunksize,
     ae_int_t* task0,
     ae_int_t* task1,
     ae_state *_state)
{

    *task0 = 0;
    *task1 = 0;

    ae_assert(chunksize>=2, "SplitLength: ChunkSize<2", _state);
    ae_assert(tasksize>=2, "SplitLength: TaskSize<2", _state);
    *task0 = tasksize/2;
    if( *task0>chunksize&&*task0%chunksize!=0 )
    {
        *task0 = *task0-*task0%chunksize;
    }
    *task1 = tasksize-(*task0);
    ae_assert(*task0>=1, "SplitLength: internal error", _state);
    ae_assert(*task1>=1, "SplitLength: internal error", _state);
}


/*************************************************************************
This function is used to calculate number of chunks (including partial,
non-complete chunks) in some set. It expects that ChunkSize>=1, TaskSize>=0.
Assertion is thrown otherwise.

Function result is equivalent to Ceil(TaskSize/ChunkSize), but with guarantees
that rounding errors won't ruin results.

  -- ALGLIB --
     Copyright 21.01.2015 by Bochkanov Sergey
*************************************************************************/
ae_int_t chunkscount(ae_int_t tasksize,
     ae_int_t chunksize,
     ae_state *_state)
{
    ae_int_t result;


    ae_assert(tasksize>=0, "ChunksCount: TaskSize<0", _state);
    ae_assert(chunksize>=1, "ChunksCount: ChunkSize<1", _state);
    result = tasksize/chunksize;
    if( tasksize%chunksize!=0 )
    {
        result = result+1;
    }
    return result;
}


void _apbuffers_init(void* _p, ae_state *_state)
{
    apbuffers *p = (apbuffers*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->ba0, 0, DT_BOOL, _state);
    ae_vector_init(&p->ia0, 0, DT_INT, _state);
    ae_vector_init(&p->ia1, 0, DT_INT, _state);
    ae_vector_init(&p->ia2, 0, DT_INT, _state);
    ae_vector_init(&p->ia3, 0, DT_INT, _state);
    ae_vector_init(&p->ra0, 0, DT_REAL, _state);
    ae_vector_init(&p->ra1, 0, DT_REAL, _state);
    ae_vector_init(&p->ra2, 0, DT_REAL, _state);
    ae_vector_init(&p->ra3, 0, DT_REAL, _state);
    ae_matrix_init(&p->rm0, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->rm1, 0, 0, DT_REAL, _state);
}


void _apbuffers_init_copy(void* _dst, void* _src, ae_state *_state)
{
    apbuffers *dst = (apbuffers*)_dst;
    apbuffers *src = (apbuffers*)_src;
    ae_vector_init_copy(&dst->ba0, &src->ba0, _state);
    ae_vector_init_copy(&dst->ia0, &src->ia0, _state);
    ae_vector_init_copy(&dst->ia1, &src->ia1, _state);
    ae_vector_init_copy(&dst->ia2, &src->ia2, _state);
    ae_vector_init_copy(&dst->ia3, &src->ia3, _state);
    ae_vector_init_copy(&dst->ra0, &src->ra0, _state);
    ae_vector_init_copy(&dst->ra1, &src->ra1, _state);
    ae_vector_init_copy(&dst->ra2, &src->ra2, _state);
    ae_vector_init_copy(&dst->ra3, &src->ra3, _state);
    ae_matrix_init_copy(&dst->rm0, &src->rm0, _state);
    ae_matrix_init_copy(&dst->rm1, &src->rm1, _state);
}


void _apbuffers_clear(void* _p)
{
    apbuffers *p = (apbuffers*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->ba0);
    ae_vector_clear(&p->ia0);
    ae_vector_clear(&p->ia1);
    ae_vector_clear(&p->ia2);
    ae_vector_clear(&p->ia3);
    ae_vector_clear(&p->ra0);
    ae_vector_clear(&p->ra1);
    ae_vector_clear(&p->ra2);
    ae_vector_clear(&p->ra3);
    ae_matrix_clear(&p->rm0);
    ae_matrix_clear(&p->rm1);
}


void _apbuffers_destroy(void* _p)
{
    apbuffers *p = (apbuffers*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->ba0);
    ae_vector_destroy(&p->ia0);
    ae_vector_destroy(&p->ia1);
    ae_vector_destroy(&p->ia2);
    ae_vector_destroy(&p->ia3);
    ae_vector_destroy(&p->ra0);
    ae_vector_destroy(&p->ra1);
    ae_vector_destroy(&p->ra2);
    ae_vector_destroy(&p->ra3);
    ae_matrix_destroy(&p->rm0);
    ae_matrix_destroy(&p->rm1);
}


void _sboolean_init(void* _p, ae_state *_state)
{
    sboolean *p = (sboolean*)_p;
    ae_touch_ptr((void*)p);
}


void _sboolean_init_copy(void* _dst, void* _src, ae_state *_state)
{
    sboolean *dst = (sboolean*)_dst;
    sboolean *src = (sboolean*)_src;
    dst->val = src->val;
}


void _sboolean_clear(void* _p)
{
    sboolean *p = (sboolean*)_p;
    ae_touch_ptr((void*)p);
}


void _sboolean_destroy(void* _p)
{
    sboolean *p = (sboolean*)_p;
    ae_touch_ptr((void*)p);
}


void _sbooleanarray_init(void* _p, ae_state *_state)
{
    sbooleanarray *p = (sbooleanarray*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->val, 0, DT_BOOL, _state);
}


void _sbooleanarray_init_copy(void* _dst, void* _src, ae_state *_state)
{
    sbooleanarray *dst = (sbooleanarray*)_dst;
    sbooleanarray *src = (sbooleanarray*)_src;
    ae_vector_init_copy(&dst->val, &src->val, _state);
}


void _sbooleanarray_clear(void* _p)
{
    sbooleanarray *p = (sbooleanarray*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->val);
}


void _sbooleanarray_destroy(void* _p)
{
    sbooleanarray *p = (sbooleanarray*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->val);
}


void _sinteger_init(void* _p, ae_state *_state)
{
    sinteger *p = (sinteger*)_p;
    ae_touch_ptr((void*)p);
}


void _sinteger_init_copy(void* _dst, void* _src, ae_state *_state)
{
    sinteger *dst = (sinteger*)_dst;
    sinteger *src = (sinteger*)_src;
    dst->val = src->val;
}


void _sinteger_clear(void* _p)
{
    sinteger *p = (sinteger*)_p;
    ae_touch_ptr((void*)p);
}


void _sinteger_destroy(void* _p)
{
    sinteger *p = (sinteger*)_p;
    ae_touch_ptr((void*)p);
}


void _sintegerarray_init(void* _p, ae_state *_state)
{
    sintegerarray *p = (sintegerarray*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->val, 0, DT_INT, _state);
}


void _sintegerarray_init_copy(void* _dst, void* _src, ae_state *_state)
{
    sintegerarray *dst = (sintegerarray*)_dst;
    sintegerarray *src = (sintegerarray*)_src;
    ae_vector_init_copy(&dst->val, &src->val, _state);
}


void _sintegerarray_clear(void* _p)
{
    sintegerarray *p = (sintegerarray*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->val);
}


void _sintegerarray_destroy(void* _p)
{
    sintegerarray *p = (sintegerarray*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->val);
}


void _sreal_init(void* _p, ae_state *_state)
{
    sreal *p = (sreal*)_p;
    ae_touch_ptr((void*)p);
}


void _sreal_init_copy(void* _dst, void* _src, ae_state *_state)
{
    sreal *dst = (sreal*)_dst;
    sreal *src = (sreal*)_src;
    dst->val = src->val;
}


void _sreal_clear(void* _p)
{
    sreal *p = (sreal*)_p;
    ae_touch_ptr((void*)p);
}


void _sreal_destroy(void* _p)
{
    sreal *p = (sreal*)_p;
    ae_touch_ptr((void*)p);
}


void _srealarray_init(void* _p, ae_state *_state)
{
    srealarray *p = (srealarray*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->val, 0, DT_REAL, _state);
}


void _srealarray_init_copy(void* _dst, void* _src, ae_state *_state)
{
    srealarray *dst = (srealarray*)_dst;
    srealarray *src = (srealarray*)_src;
    ae_vector_init_copy(&dst->val, &src->val, _state);
}


void _srealarray_clear(void* _p)
{
    srealarray *p = (srealarray*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->val);
}


void _srealarray_destroy(void* _p)
{
    srealarray *p = (srealarray*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->val);
}


void _scomplex_init(void* _p, ae_state *_state)
{
    scomplex *p = (scomplex*)_p;
    ae_touch_ptr((void*)p);
}


void _scomplex_init_copy(void* _dst, void* _src, ae_state *_state)
{
    scomplex *dst = (scomplex*)_dst;
    scomplex *src = (scomplex*)_src;
    dst->val = src->val;
}


void _scomplex_clear(void* _p)
{
    scomplex *p = (scomplex*)_p;
    ae_touch_ptr((void*)p);
}


void _scomplex_destroy(void* _p)
{
    scomplex *p = (scomplex*)_p;
    ae_touch_ptr((void*)p);
}


void _scomplexarray_init(void* _p, ae_state *_state)
{
    scomplexarray *p = (scomplexarray*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->val, 0, DT_COMPLEX, _state);
}


void _scomplexarray_init_copy(void* _dst, void* _src, ae_state *_state)
{
    scomplexarray *dst = (scomplexarray*)_dst;
    scomplexarray *src = (scomplexarray*)_src;
    ae_vector_init_copy(&dst->val, &src->val, _state);
}


void _scomplexarray_clear(void* _p)
{
    scomplexarray *p = (scomplexarray*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->val);
}


void _scomplexarray_destroy(void* _p)
{
    scomplexarray *p = (scomplexarray*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->val);
}


/*$ End $*/
