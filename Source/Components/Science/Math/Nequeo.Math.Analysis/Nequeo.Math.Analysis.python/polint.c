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
#include "polint.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Conversion from barycentric representation to Chebyshev basis.
This function has O(N^2) complexity.

INPUT PARAMETERS:
    P   -   polynomial in barycentric form
    A,B -   base interval for Chebyshev polynomials (see below)
            A<>B

OUTPUT PARAMETERS
    T   -   coefficients of Chebyshev representation;
            P(x) = sum { T[i]*Ti(2*(x-A)/(B-A)-1), i=0..N-1 },
            where Ti - I-th Chebyshev polynomial.

NOTES:
    barycentric interpolant passed as P may be either polynomial  obtained
    from  polynomial  interpolation/ fitting or rational function which is
    NOT polynomial. We can't distinguish between these two cases, and this
    algorithm just tries to work assuming that P IS a polynomial.  If not,
    algorithm will return results, but they won't have any meaning.

  -- ALGLIB --
     Copyright 30.09.2010 by Bochkanov Sergey
*************************************************************************/
void polynomialbar2cheb(barycentricinterpolant* p,
     double a,
     double b,
     /* Real    */ ae_vector* t,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t k;
    ae_vector vp;
    ae_vector vx;
    ae_vector tk;
    ae_vector tk1;
    double v;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(t);
    ae_vector_init(&vp, 0, DT_REAL, _state);
    ae_vector_init(&vx, 0, DT_REAL, _state);
    ae_vector_init(&tk, 0, DT_REAL, _state);
    ae_vector_init(&tk1, 0, DT_REAL, _state);

    ae_assert(ae_isfinite(a, _state), "PolynomialBar2Cheb: A is not finite!", _state);
    ae_assert(ae_isfinite(b, _state), "PolynomialBar2Cheb: B is not finite!", _state);
    ae_assert(ae_fp_neq(a,b), "PolynomialBar2Cheb: A=B!", _state);
    ae_assert(p->n>0, "PolynomialBar2Cheb: P is not correctly initialized barycentric interpolant!", _state);
    
    /*
     * Calculate function values on a Chebyshev grid
     */
    ae_vector_set_length(&vp, p->n, _state);
    ae_vector_set_length(&vx, p->n, _state);
    for(i=0; i<=p->n-1; i++)
    {
        vx.ptr.p_double[i] = ae_cos(ae_pi*(i+0.5)/p->n, _state);
        vp.ptr.p_double[i] = barycentriccalc(p, 0.5*(vx.ptr.p_double[i]+1)*(b-a)+a, _state);
    }
    
    /*
     * T[0]
     */
    ae_vector_set_length(t, p->n, _state);
    v = (double)(0);
    for(i=0; i<=p->n-1; i++)
    {
        v = v+vp.ptr.p_double[i];
    }
    t->ptr.p_double[0] = v/p->n;
    
    /*
     * other T's.
     *
     * NOTES:
     * 1. TK stores T{k} on VX, TK1 stores T{k-1} on VX
     * 2. we can do same calculations with fast DCT, but it
     *    * adds dependencies
     *    * still leaves us with O(N^2) algorithm because
     *      preparation of function values is O(N^2) process
     */
    if( p->n>1 )
    {
        ae_vector_set_length(&tk, p->n, _state);
        ae_vector_set_length(&tk1, p->n, _state);
        for(i=0; i<=p->n-1; i++)
        {
            tk.ptr.p_double[i] = vx.ptr.p_double[i];
            tk1.ptr.p_double[i] = (double)(1);
        }
        for(k=1; k<=p->n-1; k++)
        {
            
            /*
             * calculate discrete product of function vector and TK
             */
            v = ae_v_dotproduct(&tk.ptr.p_double[0], 1, &vp.ptr.p_double[0], 1, ae_v_len(0,p->n-1));
            t->ptr.p_double[k] = v/(0.5*p->n);
            
            /*
             * Update TK and TK1
             */
            for(i=0; i<=p->n-1; i++)
            {
                v = 2*vx.ptr.p_double[i]*tk.ptr.p_double[i]-tk1.ptr.p_double[i];
                tk1.ptr.p_double[i] = tk.ptr.p_double[i];
                tk.ptr.p_double[i] = v;
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Conversion from Chebyshev basis to barycentric representation.
This function has O(N^2) complexity.

INPUT PARAMETERS:
    T   -   coefficients of Chebyshev representation;
            P(x) = sum { T[i]*Ti(2*(x-A)/(B-A)-1), i=0..N },
            where Ti - I-th Chebyshev polynomial.
    N   -   number of coefficients:
            * if given, only leading N elements of T are used
            * if not given, automatically determined from size of T
    A,B -   base interval for Chebyshev polynomials (see above)
            A<B

OUTPUT PARAMETERS
    P   -   polynomial in barycentric form

  -- ALGLIB --
     Copyright 30.09.2010 by Bochkanov Sergey
*************************************************************************/
void polynomialcheb2bar(/* Real    */ ae_vector* t,
     ae_int_t n,
     double a,
     double b,
     barycentricinterpolant* p,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t k;
    ae_vector y;
    double tk;
    double tk1;
    double vx;
    double vy;
    double v;

    ae_frame_make(_state, &_frame_block);
    _barycentricinterpolant_clear(p);
    ae_vector_init(&y, 0, DT_REAL, _state);

    ae_assert(ae_isfinite(a, _state), "PolynomialBar2Cheb: A is not finite!", _state);
    ae_assert(ae_isfinite(b, _state), "PolynomialBar2Cheb: B is not finite!", _state);
    ae_assert(ae_fp_neq(a,b), "PolynomialBar2Cheb: A=B!", _state);
    ae_assert(n>=1, "PolynomialBar2Cheb: N<1", _state);
    ae_assert(t->cnt>=n, "PolynomialBar2Cheb: Length(T)<N", _state);
    ae_assert(isfinitevector(t, n, _state), "PolynomialBar2Cheb: T[] contains INF or NAN", _state);
    
    /*
     * Calculate function values on a Chebyshev grid spanning [-1,+1]
     */
    ae_vector_set_length(&y, n, _state);
    for(i=0; i<=n-1; i++)
    {
        
        /*
         * Calculate value on a grid spanning [-1,+1]
         */
        vx = ae_cos(ae_pi*(i+0.5)/n, _state);
        vy = t->ptr.p_double[0];
        tk1 = (double)(1);
        tk = vx;
        for(k=1; k<=n-1; k++)
        {
            vy = vy+t->ptr.p_double[k]*tk;
            v = 2*vx*tk-tk1;
            tk1 = tk;
            tk = v;
        }
        y.ptr.p_double[i] = vy;
    }
    
    /*
     * Build barycentric interpolant, map grid from [-1,+1] to [A,B]
     */
    polynomialbuildcheb1(a, b, &y, n, p, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Conversion from barycentric representation to power basis.
This function has O(N^2) complexity.

INPUT PARAMETERS:
    P   -   polynomial in barycentric form
    C   -   offset (see below); 0.0 is used as default value.
    S   -   scale (see below);  1.0 is used as default value. S<>0.

OUTPUT PARAMETERS
    A   -   coefficients, P(x) = sum { A[i]*((X-C)/S)^i, i=0..N-1 }
    N   -   number of coefficients (polynomial degree plus 1)

NOTES:
1.  this function accepts offset and scale, which can be  set  to  improve
    numerical properties of polynomial. For example, if P was obtained  as
    result of interpolation on [-1,+1],  you  can  set  C=0  and  S=1  and
    represent  P  as sum of 1, x, x^2, x^3 and so on. In most cases you it
    is exactly what you need.

    However, if your interpolation model was built on [999,1001], you will
    see significant growth of numerical errors when using {1, x, x^2, x^3}
    as basis. Representing P as sum of 1, (x-1000), (x-1000)^2, (x-1000)^3
    will be better option. Such representation can be  obtained  by  using
    1000.0 as offset C and 1.0 as scale S.

2.  power basis is ill-conditioned and tricks described above can't  solve
    this problem completely. This function  will  return  coefficients  in
    any  case,  but  for  N>8  they  will  become unreliable. However, N's
    less than 5 are pretty safe.
    
3.  barycentric interpolant passed as P may be either polynomial  obtained
    from  polynomial  interpolation/ fitting or rational function which is
    NOT polynomial. We can't distinguish between these two cases, and this
    algorithm just tries to work assuming that P IS a polynomial.  If not,
    algorithm will return results, but they won't have any meaning.

  -- ALGLIB --
     Copyright 30.09.2010 by Bochkanov Sergey
*************************************************************************/
void polynomialbar2pow(barycentricinterpolant* p,
     double c,
     double s,
     /* Real    */ ae_vector* a,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t k;
    double e;
    double d;
    ae_vector vp;
    ae_vector vx;
    ae_vector tk;
    ae_vector tk1;
    ae_vector t;
    double v;
    double c0;
    double s0;
    double va;
    double vb;
    ae_vector vai;
    ae_vector vbi;
    double minx;
    double maxx;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(a);
    ae_vector_init(&vp, 0, DT_REAL, _state);
    ae_vector_init(&vx, 0, DT_REAL, _state);
    ae_vector_init(&tk, 0, DT_REAL, _state);
    ae_vector_init(&tk1, 0, DT_REAL, _state);
    ae_vector_init(&t, 0, DT_REAL, _state);
    ae_vector_init(&vai, 0, DT_REAL, _state);
    ae_vector_init(&vbi, 0, DT_REAL, _state);

    
    /*
     * We have barycentric model built using set of points X[], and we
     * want to convert it to power basis centered about point  C  with
     * scale S: I-th basis function is ((X-C)/S)^i.
     *
     * We use following three-stage algorithm:
     *
     * 1. we build Chebyshev representation of polynomial using
     *    intermediate center C0 and scale S0, which are derived from X[]:
     *    C0 = 0.5*(min(X)+max(X)), S0 = 0.5*(max(X)-min(X)). Chebyshev
     *    representation is built by sampling points around center C0,
     *    with typical distance between them proportional to S0.
     * 2. then we transform form Chebyshev basis to intermediate power
     *    basis, using same center/scale C0/S0.
     * 3. after that, we apply linear transformation to intermediate
     *    power basis which moves it to final center/scale C/S.
     *
     * The idea of such multi-stage algorithm is that it is much easier to
     * transform barycentric model to Chebyshev basis, and only later to
     * power basis, than transforming it directly to power basis. It is
     * also more numerically stable to sample points using intermediate C0/S0,
     * which are derived from user-supplied model, than using "final" C/S,
     * which may be unsuitable for sampling (say, if S=1, we may have stability
     * problems when working with models built from dataset with non-unit
     * scale of abscissas).
     */
    ae_assert(ae_isfinite(c, _state), "PolynomialBar2Pow: C is not finite!", _state);
    ae_assert(ae_isfinite(s, _state), "PolynomialBar2Pow: S is not finite!", _state);
    ae_assert(ae_fp_neq(s,(double)(0)), "PolynomialBar2Pow: S=0!", _state);
    ae_assert(p->n>0, "PolynomialBar2Pow: P is not correctly initialized barycentric interpolant!", _state);
    
    /*
     * Select intermediate center/scale
     */
    minx = p->x.ptr.p_double[0];
    maxx = p->x.ptr.p_double[0];
    for(i=1; i<=p->n-1; i++)
    {
        minx = ae_minreal(minx, p->x.ptr.p_double[i], _state);
        maxx = ae_maxreal(maxx, p->x.ptr.p_double[i], _state);
    }
    if( ae_fp_eq(minx,maxx) )
    {
        c0 = minx;
        s0 = 1.0;
    }
    else
    {
        c0 = 0.5*(maxx+minx);
        s0 = 0.5*(maxx-minx);
    }
    
    /*
     * Calculate function values on a Chebyshev grid using intermediate C0/S0
     */
    ae_vector_set_length(&vp, p->n+1, _state);
    ae_vector_set_length(&vx, p->n, _state);
    for(i=0; i<=p->n-1; i++)
    {
        vx.ptr.p_double[i] = ae_cos(ae_pi*(i+0.5)/p->n, _state);
        vp.ptr.p_double[i] = barycentriccalc(p, s0*vx.ptr.p_double[i]+c0, _state);
    }
    
    /*
     * T[0]
     */
    ae_vector_set_length(&t, p->n, _state);
    v = (double)(0);
    for(i=0; i<=p->n-1; i++)
    {
        v = v+vp.ptr.p_double[i];
    }
    t.ptr.p_double[0] = v/p->n;
    
    /*
     * other T's.
     *
     * NOTES:
     * 1. TK stores T{k} on VX, TK1 stores T{k-1} on VX
     * 2. we can do same calculations with fast DCT, but it
     *    * adds dependencies
     *    * still leaves us with O(N^2) algorithm because
     *      preparation of function values is O(N^2) process
     */
    if( p->n>1 )
    {
        ae_vector_set_length(&tk, p->n, _state);
        ae_vector_set_length(&tk1, p->n, _state);
        for(i=0; i<=p->n-1; i++)
        {
            tk.ptr.p_double[i] = vx.ptr.p_double[i];
            tk1.ptr.p_double[i] = (double)(1);
        }
        for(k=1; k<=p->n-1; k++)
        {
            
            /*
             * calculate discrete product of function vector and TK
             */
            v = ae_v_dotproduct(&tk.ptr.p_double[0], 1, &vp.ptr.p_double[0], 1, ae_v_len(0,p->n-1));
            t.ptr.p_double[k] = v/(0.5*p->n);
            
            /*
             * Update TK and TK1
             */
            for(i=0; i<=p->n-1; i++)
            {
                v = 2*vx.ptr.p_double[i]*tk.ptr.p_double[i]-tk1.ptr.p_double[i];
                tk1.ptr.p_double[i] = tk.ptr.p_double[i];
                tk.ptr.p_double[i] = v;
            }
        }
    }
    
    /*
     * Convert from Chebyshev basis to power basis
     */
    ae_vector_set_length(a, p->n, _state);
    for(i=0; i<=p->n-1; i++)
    {
        a->ptr.p_double[i] = (double)(0);
    }
    d = (double)(0);
    for(i=0; i<=p->n-1; i++)
    {
        for(k=i; k<=p->n-1; k++)
        {
            e = a->ptr.p_double[k];
            a->ptr.p_double[k] = (double)(0);
            if( i<=1&&k==i )
            {
                a->ptr.p_double[k] = (double)(1);
            }
            else
            {
                if( i!=0 )
                {
                    a->ptr.p_double[k] = 2*d;
                }
                if( k>i+1 )
                {
                    a->ptr.p_double[k] = a->ptr.p_double[k]-a->ptr.p_double[k-2];
                }
            }
            d = e;
        }
        d = a->ptr.p_double[i];
        e = (double)(0);
        k = i;
        while(k<=p->n-1)
        {
            e = e+a->ptr.p_double[k]*t.ptr.p_double[k];
            k = k+2;
        }
        a->ptr.p_double[i] = e;
    }
    
    /*
     * Apply linear transformation which converts basis from intermediate
     * one Fi=((x-C0)/S0)^i to final one Fi=((x-C)/S)^i.
     *
     * We have y=(x-C0)/S0, z=(x-C)/S, and coefficients A[] for basis Fi(y).
     * Because we have y=A*z+B, for A=s/s0 and B=c/s0-c0/s0, we can perform
     * substitution and get coefficients A_new[] in basis Fi(z).
     */
    ae_assert(vp.cnt>=p->n+1, "PolynomialBar2Pow: internal error", _state);
    ae_assert(t.cnt>=p->n, "PolynomialBar2Pow: internal error", _state);
    for(i=0; i<=p->n-1; i++)
    {
        t.ptr.p_double[i] = 0.0;
    }
    va = s/s0;
    vb = c/s0-c0/s0;
    ae_vector_set_length(&vai, p->n, _state);
    ae_vector_set_length(&vbi, p->n, _state);
    vai.ptr.p_double[0] = (double)(1);
    vbi.ptr.p_double[0] = (double)(1);
    for(k=1; k<=p->n-1; k++)
    {
        vai.ptr.p_double[k] = vai.ptr.p_double[k-1]*va;
        vbi.ptr.p_double[k] = vbi.ptr.p_double[k-1]*vb;
    }
    for(k=0; k<=p->n-1; k++)
    {
        
        /*
         * Generate set of binomial coefficients in VP[]
         */
        if( k>0 )
        {
            vp.ptr.p_double[k] = (double)(1);
            for(i=k-1; i>=1; i--)
            {
                vp.ptr.p_double[i] = vp.ptr.p_double[i]+vp.ptr.p_double[i-1];
            }
            vp.ptr.p_double[0] = (double)(1);
        }
        else
        {
            vp.ptr.p_double[0] = (double)(1);
        }
        
        /*
         * Update T[] with expansion of K-th basis function
         */
        for(i=0; i<=k; i++)
        {
            t.ptr.p_double[i] = t.ptr.p_double[i]+a->ptr.p_double[k]*vai.ptr.p_double[i]*vbi.ptr.p_double[k-i]*vp.ptr.p_double[i];
        }
    }
    for(k=0; k<=p->n-1; k++)
    {
        a->ptr.p_double[k] = t.ptr.p_double[k];
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Conversion from power basis to barycentric representation.
This function has O(N^2) complexity.

INPUT PARAMETERS:
    A   -   coefficients, P(x) = sum { A[i]*((X-C)/S)^i, i=0..N-1 }
    N   -   number of coefficients (polynomial degree plus 1)
            * if given, only leading N elements of A are used
            * if not given, automatically determined from size of A
    C   -   offset (see below); 0.0 is used as default value.
    S   -   scale (see below);  1.0 is used as default value. S<>0.

OUTPUT PARAMETERS
    P   -   polynomial in barycentric form


NOTES:
1.  this function accepts offset and scale, which can be  set  to  improve
    numerical properties of polynomial. For example, if you interpolate on
    [-1,+1],  you  can  set C=0 and S=1 and convert from sum of 1, x, x^2,
    x^3 and so on. In most cases you it is exactly what you need.

    However, if your interpolation model was built on [999,1001], you will
    see significant growth of numerical errors when using {1, x, x^2, x^3}
    as  input  basis.  Converting  from  sum  of  1, (x-1000), (x-1000)^2,
    (x-1000)^3 will be better option (you have to specify 1000.0 as offset
    C and 1.0 as scale S).

2.  power basis is ill-conditioned and tricks described above can't  solve
    this problem completely. This function  will  return barycentric model
    in any case, but for N>8 accuracy well degrade. However, N's less than
    5 are pretty safe.

  -- ALGLIB --
     Copyright 30.09.2010 by Bochkanov Sergey
*************************************************************************/
void polynomialpow2bar(/* Real    */ ae_vector* a,
     ae_int_t n,
     double c,
     double s,
     barycentricinterpolant* p,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t k;
    ae_vector y;
    double vx;
    double vy;
    double px;

    ae_frame_make(_state, &_frame_block);
    _barycentricinterpolant_clear(p);
    ae_vector_init(&y, 0, DT_REAL, _state);

    ae_assert(ae_isfinite(c, _state), "PolynomialPow2Bar: C is not finite!", _state);
    ae_assert(ae_isfinite(s, _state), "PolynomialPow2Bar: S is not finite!", _state);
    ae_assert(ae_fp_neq(s,(double)(0)), "PolynomialPow2Bar: S is zero!", _state);
    ae_assert(n>=1, "PolynomialPow2Bar: N<1", _state);
    ae_assert(a->cnt>=n, "PolynomialPow2Bar: Length(A)<N", _state);
    ae_assert(isfinitevector(a, n, _state), "PolynomialPow2Bar: A[] contains INF or NAN", _state);
    
    /*
     * Calculate function values on a Chebyshev grid spanning [-1,+1]
     */
    ae_vector_set_length(&y, n, _state);
    for(i=0; i<=n-1; i++)
    {
        
        /*
         * Calculate value on a grid spanning [-1,+1]
         */
        vx = ae_cos(ae_pi*(i+0.5)/n, _state);
        vy = a->ptr.p_double[0];
        px = vx;
        for(k=1; k<=n-1; k++)
        {
            vy = vy+px*a->ptr.p_double[k];
            px = px*vx;
        }
        y.ptr.p_double[i] = vy;
    }
    
    /*
     * Build barycentric interpolant, map grid from [-1,+1] to [A,B]
     */
    polynomialbuildcheb1(c-s, c+s, &y, n, p, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Lagrange intepolant: generation of the model on the general grid.
This function has O(N^2) complexity.

INPUT PARAMETERS:
    X   -   abscissas, array[0..N-1]
    Y   -   function values, array[0..N-1]
    N   -   number of points, N>=1

OUTPUT PARAMETERS
    P   -   barycentric model which represents Lagrange interpolant
            (see ratint unit info and BarycentricCalc() description for
            more information).

  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
void polynomialbuild(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     barycentricinterpolant* p,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_int_t j;
    ae_int_t k;
    ae_vector w;
    double b;
    double a;
    double v;
    double mx;
    ae_vector sortrbuf;
    ae_vector sortrbuf2;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    _barycentricinterpolant_clear(p);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&sortrbuf, 0, DT_REAL, _state);
    ae_vector_init(&sortrbuf2, 0, DT_REAL, _state);

    ae_assert(n>0, "PolynomialBuild: N<=0!", _state);
    ae_assert(x->cnt>=n, "PolynomialBuild: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "PolynomialBuild: Length(Y)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "PolynomialBuild: X contains infinite or NaN values!", _state);
    ae_assert(isfinitevector(y, n, _state), "PolynomialBuild: Y contains infinite or NaN values!", _state);
    tagsortfastr(x, y, &sortrbuf, &sortrbuf2, n, _state);
    ae_assert(aredistinct(x, n, _state), "PolynomialBuild: at least two consequent points are too close!", _state);
    
    /*
     * calculate W[j]
     * multi-pass algorithm is used to avoid overflow
     */
    ae_vector_set_length(&w, n, _state);
    a = x->ptr.p_double[0];
    b = x->ptr.p_double[0];
    for(j=0; j<=n-1; j++)
    {
        w.ptr.p_double[j] = (double)(1);
        a = ae_minreal(a, x->ptr.p_double[j], _state);
        b = ae_maxreal(b, x->ptr.p_double[j], _state);
    }
    for(k=0; k<=n-1; k++)
    {
        
        /*
         * W[K] is used instead of 0.0 because
         * cycle on J does not touch K-th element
         * and we MUST get maximum from ALL elements
         */
        mx = ae_fabs(w.ptr.p_double[k], _state);
        for(j=0; j<=n-1; j++)
        {
            if( j!=k )
            {
                v = (b-a)/(x->ptr.p_double[j]-x->ptr.p_double[k]);
                w.ptr.p_double[j] = w.ptr.p_double[j]*v;
                mx = ae_maxreal(mx, ae_fabs(w.ptr.p_double[j], _state), _state);
            }
        }
        if( k%5==0 )
        {
            
            /*
             * every 5-th run we renormalize W[]
             */
            v = 1/mx;
            ae_v_muld(&w.ptr.p_double[0], 1, ae_v_len(0,n-1), v);
        }
    }
    barycentricbuildxyw(x, y, &w, n, p, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Lagrange intepolant: generation of the model on equidistant grid.
This function has O(N) complexity.

INPUT PARAMETERS:
    A   -   left boundary of [A,B]
    B   -   right boundary of [A,B]
    Y   -   function values at the nodes, array[0..N-1]
    N   -   number of points, N>=1
            for N=1 a constant model is constructed.

OUTPUT PARAMETERS
    P   -   barycentric model which represents Lagrange interpolant
            (see ratint unit info and BarycentricCalc() description for
            more information).

  -- ALGLIB --
     Copyright 03.12.2009 by Bochkanov Sergey
*************************************************************************/
void polynomialbuildeqdist(double a,
     double b,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     barycentricinterpolant* p,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_vector w;
    ae_vector x;
    double v;

    ae_frame_make(_state, &_frame_block);
    _barycentricinterpolant_clear(p);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);

    ae_assert(n>0, "PolynomialBuildEqDist: N<=0!", _state);
    ae_assert(y->cnt>=n, "PolynomialBuildEqDist: Length(Y)<N!", _state);
    ae_assert(ae_isfinite(a, _state), "PolynomialBuildEqDist: A is infinite or NaN!", _state);
    ae_assert(ae_isfinite(b, _state), "PolynomialBuildEqDist: B is infinite or NaN!", _state);
    ae_assert(isfinitevector(y, n, _state), "PolynomialBuildEqDist: Y contains infinite or NaN values!", _state);
    ae_assert(ae_fp_neq(b,a), "PolynomialBuildEqDist: B=A!", _state);
    ae_assert(ae_fp_neq(a+(b-a)/n,a), "PolynomialBuildEqDist: B is too close to A!", _state);
    
    /*
     * Special case: N=1
     */
    if( n==1 )
    {
        ae_vector_set_length(&x, 1, _state);
        ae_vector_set_length(&w, 1, _state);
        x.ptr.p_double[0] = 0.5*(b+a);
        w.ptr.p_double[0] = (double)(1);
        barycentricbuildxyw(&x, y, &w, 1, p, _state);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * general case
     */
    ae_vector_set_length(&x, n, _state);
    ae_vector_set_length(&w, n, _state);
    v = (double)(1);
    for(i=0; i<=n-1; i++)
    {
        w.ptr.p_double[i] = v;
        x.ptr.p_double[i] = a+(b-a)*i/(n-1);
        v = -v*(n-1-i);
        v = v/(i+1);
    }
    barycentricbuildxyw(&x, y, &w, n, p, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Lagrange intepolant on Chebyshev grid (first kind).
This function has O(N) complexity.

INPUT PARAMETERS:
    A   -   left boundary of [A,B]
    B   -   right boundary of [A,B]
    Y   -   function values at the nodes, array[0..N-1],
            Y[I] = Y(0.5*(B+A) + 0.5*(B-A)*Cos(PI*(2*i+1)/(2*n)))
    N   -   number of points, N>=1
            for N=1 a constant model is constructed.

OUTPUT PARAMETERS
    P   -   barycentric model which represents Lagrange interpolant
            (see ratint unit info and BarycentricCalc() description for
            more information).

  -- ALGLIB --
     Copyright 03.12.2009 by Bochkanov Sergey
*************************************************************************/
void polynomialbuildcheb1(double a,
     double b,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     barycentricinterpolant* p,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_vector w;
    ae_vector x;
    double v;
    double t;

    ae_frame_make(_state, &_frame_block);
    _barycentricinterpolant_clear(p);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);

    ae_assert(n>0, "PolynomialBuildCheb1: N<=0!", _state);
    ae_assert(y->cnt>=n, "PolynomialBuildCheb1: Length(Y)<N!", _state);
    ae_assert(ae_isfinite(a, _state), "PolynomialBuildCheb1: A is infinite or NaN!", _state);
    ae_assert(ae_isfinite(b, _state), "PolynomialBuildCheb1: B is infinite or NaN!", _state);
    ae_assert(isfinitevector(y, n, _state), "PolynomialBuildCheb1: Y contains infinite or NaN values!", _state);
    ae_assert(ae_fp_neq(b,a), "PolynomialBuildCheb1: B=A!", _state);
    
    /*
     * Special case: N=1
     */
    if( n==1 )
    {
        ae_vector_set_length(&x, 1, _state);
        ae_vector_set_length(&w, 1, _state);
        x.ptr.p_double[0] = 0.5*(b+a);
        w.ptr.p_double[0] = (double)(1);
        barycentricbuildxyw(&x, y, &w, 1, p, _state);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * general case
     */
    ae_vector_set_length(&x, n, _state);
    ae_vector_set_length(&w, n, _state);
    v = (double)(1);
    for(i=0; i<=n-1; i++)
    {
        t = ae_tan(0.5*ae_pi*(2*i+1)/(2*n), _state);
        w.ptr.p_double[i] = 2*v*t/(1+ae_sqr(t, _state));
        x.ptr.p_double[i] = 0.5*(b+a)+0.5*(b-a)*(1-ae_sqr(t, _state))/(1+ae_sqr(t, _state));
        v = -v;
    }
    barycentricbuildxyw(&x, y, &w, n, p, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Lagrange intepolant on Chebyshev grid (second kind).
This function has O(N) complexity.

INPUT PARAMETERS:
    A   -   left boundary of [A,B]
    B   -   right boundary of [A,B]
    Y   -   function values at the nodes, array[0..N-1],
            Y[I] = Y(0.5*(B+A) + 0.5*(B-A)*Cos(PI*i/(n-1)))
    N   -   number of points, N>=1
            for N=1 a constant model is constructed.

OUTPUT PARAMETERS
    P   -   barycentric model which represents Lagrange interpolant
            (see ratint unit info and BarycentricCalc() description for
            more information).

  -- ALGLIB --
     Copyright 03.12.2009 by Bochkanov Sergey
*************************************************************************/
void polynomialbuildcheb2(double a,
     double b,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     barycentricinterpolant* p,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_vector w;
    ae_vector x;
    double v;

    ae_frame_make(_state, &_frame_block);
    _barycentricinterpolant_clear(p);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);

    ae_assert(n>0, "PolynomialBuildCheb2: N<=0!", _state);
    ae_assert(y->cnt>=n, "PolynomialBuildCheb2: Length(Y)<N!", _state);
    ae_assert(ae_isfinite(a, _state), "PolynomialBuildCheb2: A is infinite or NaN!", _state);
    ae_assert(ae_isfinite(b, _state), "PolynomialBuildCheb2: B is infinite or NaN!", _state);
    ae_assert(ae_fp_neq(b,a), "PolynomialBuildCheb2: B=A!", _state);
    ae_assert(isfinitevector(y, n, _state), "PolynomialBuildCheb2: Y contains infinite or NaN values!", _state);
    
    /*
     * Special case: N=1
     */
    if( n==1 )
    {
        ae_vector_set_length(&x, 1, _state);
        ae_vector_set_length(&w, 1, _state);
        x.ptr.p_double[0] = 0.5*(b+a);
        w.ptr.p_double[0] = (double)(1);
        barycentricbuildxyw(&x, y, &w, 1, p, _state);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * general case
     */
    ae_vector_set_length(&x, n, _state);
    ae_vector_set_length(&w, n, _state);
    v = (double)(1);
    for(i=0; i<=n-1; i++)
    {
        if( i==0||i==n-1 )
        {
            w.ptr.p_double[i] = v*0.5;
        }
        else
        {
            w.ptr.p_double[i] = v;
        }
        x.ptr.p_double[i] = 0.5*(b+a)+0.5*(b-a)*ae_cos(ae_pi*i/(n-1), _state);
        v = -v;
    }
    barycentricbuildxyw(&x, y, &w, n, p, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Fast equidistant polynomial interpolation function with O(N) complexity

INPUT PARAMETERS:
    A   -   left boundary of [A,B]
    B   -   right boundary of [A,B]
    F   -   function values, array[0..N-1]
    N   -   number of points on equidistant grid, N>=1
            for N=1 a constant model is constructed.
    T   -   position where P(x) is calculated

RESULT
    value of the Lagrange interpolant at T
    
IMPORTANT
    this function provides fast interface which is not overflow-safe
    nor it is very precise.
    the best option is to use  PolynomialBuildEqDist()/BarycentricCalc()
    subroutines unless you are pretty sure that your data will not result
    in overflow.

  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
double polynomialcalceqdist(double a,
     double b,
     /* Real    */ ae_vector* f,
     ae_int_t n,
     double t,
     ae_state *_state)
{
    double s1;
    double s2;
    double v;
    double threshold;
    double s;
    double h;
    ae_int_t i;
    ae_int_t j;
    double w;
    double x;
    double result;


    ae_assert(n>0, "PolynomialCalcEqDist: N<=0!", _state);
    ae_assert(f->cnt>=n, "PolynomialCalcEqDist: Length(F)<N!", _state);
    ae_assert(ae_isfinite(a, _state), "PolynomialCalcEqDist: A is infinite or NaN!", _state);
    ae_assert(ae_isfinite(b, _state), "PolynomialCalcEqDist: B is infinite or NaN!", _state);
    ae_assert(isfinitevector(f, n, _state), "PolynomialCalcEqDist: F contains infinite or NaN values!", _state);
    ae_assert(ae_fp_neq(b,a), "PolynomialCalcEqDist: B=A!", _state);
    ae_assert(!ae_isinf(t, _state), "PolynomialCalcEqDist: T is infinite!", _state);
    
    /*
     * Special case: T is NAN
     */
    if( ae_isnan(t, _state) )
    {
        result = _state->v_nan;
        return result;
    }
    
    /*
     * Special case: N=1
     */
    if( n==1 )
    {
        result = f->ptr.p_double[0];
        return result;
    }
    
    /*
     * First, decide: should we use "safe" formula (guarded
     * against overflow) or fast one?
     */
    threshold = ae_sqrt(ae_minrealnumber, _state);
    j = 0;
    s = t-a;
    for(i=1; i<=n-1; i++)
    {
        x = a+(double)i/(double)(n-1)*(b-a);
        if( ae_fp_less(ae_fabs(t-x, _state),ae_fabs(s, _state)) )
        {
            s = t-x;
            j = i;
        }
    }
    if( ae_fp_eq(s,(double)(0)) )
    {
        result = f->ptr.p_double[j];
        return result;
    }
    if( ae_fp_greater(ae_fabs(s, _state),threshold) )
    {
        
        /*
         * use fast formula
         */
        j = -1;
        s = 1.0;
    }
    
    /*
     * Calculate using safe or fast barycentric formula
     */
    s1 = (double)(0);
    s2 = (double)(0);
    w = 1.0;
    h = (b-a)/(n-1);
    for(i=0; i<=n-1; i++)
    {
        if( i!=j )
        {
            v = s*w/(t-(a+i*h));
            s1 = s1+v*f->ptr.p_double[i];
            s2 = s2+v;
        }
        else
        {
            v = w;
            s1 = s1+v*f->ptr.p_double[i];
            s2 = s2+v;
        }
        w = -w*(n-1-i);
        w = w/(i+1);
    }
    result = s1/s2;
    return result;
}


/*************************************************************************
Fast polynomial interpolation function on Chebyshev points (first kind)
with O(N) complexity.

INPUT PARAMETERS:
    A   -   left boundary of [A,B]
    B   -   right boundary of [A,B]
    F   -   function values, array[0..N-1]
    N   -   number of points on Chebyshev grid (first kind),
            X[i] = 0.5*(B+A) + 0.5*(B-A)*Cos(PI*(2*i+1)/(2*n))
            for N=1 a constant model is constructed.
    T   -   position where P(x) is calculated

RESULT
    value of the Lagrange interpolant at T

IMPORTANT
    this function provides fast interface which is not overflow-safe
    nor it is very precise.
    the best option is to use  PolIntBuildCheb1()/BarycentricCalc()
    subroutines unless you are pretty sure that your data will not result
    in overflow.

  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
double polynomialcalccheb1(double a,
     double b,
     /* Real    */ ae_vector* f,
     ae_int_t n,
     double t,
     ae_state *_state)
{
    double s1;
    double s2;
    double v;
    double threshold;
    double s;
    ae_int_t i;
    ae_int_t j;
    double a0;
    double delta;
    double alpha;
    double beta;
    double ca;
    double sa;
    double tempc;
    double temps;
    double x;
    double w;
    double p1;
    double result;


    ae_assert(n>0, "PolynomialCalcCheb1: N<=0!", _state);
    ae_assert(f->cnt>=n, "PolynomialCalcCheb1: Length(F)<N!", _state);
    ae_assert(ae_isfinite(a, _state), "PolynomialCalcCheb1: A is infinite or NaN!", _state);
    ae_assert(ae_isfinite(b, _state), "PolynomialCalcCheb1: B is infinite or NaN!", _state);
    ae_assert(isfinitevector(f, n, _state), "PolynomialCalcCheb1: F contains infinite or NaN values!", _state);
    ae_assert(ae_fp_neq(b,a), "PolynomialCalcCheb1: B=A!", _state);
    ae_assert(!ae_isinf(t, _state), "PolynomialCalcCheb1: T is infinite!", _state);
    
    /*
     * Special case: T is NAN
     */
    if( ae_isnan(t, _state) )
    {
        result = _state->v_nan;
        return result;
    }
    
    /*
     * Special case: N=1
     */
    if( n==1 )
    {
        result = f->ptr.p_double[0];
        return result;
    }
    
    /*
     * Prepare information for the recurrence formula
     * used to calculate sin(pi*(2j+1)/(2n+2)) and
     * cos(pi*(2j+1)/(2n+2)):
     *
     * A0    = pi/(2n+2)
     * Delta = pi/(n+1)
     * Alpha = 2 sin^2 (Delta/2)
     * Beta  = sin(Delta)
     *
     * so that sin(..) = sin(A0+j*delta) and cos(..) = cos(A0+j*delta).
     * Then we use
     *
     * sin(x+delta) = sin(x) - (alpha*sin(x) - beta*cos(x))
     * cos(x+delta) = cos(x) - (alpha*cos(x) - beta*sin(x))
     *
     * to repeatedly calculate sin(..) and cos(..).
     */
    threshold = ae_sqrt(ae_minrealnumber, _state);
    t = (t-0.5*(a+b))/(0.5*(b-a));
    a0 = ae_pi/(2*(n-1)+2);
    delta = 2*ae_pi/(2*(n-1)+2);
    alpha = 2*ae_sqr(ae_sin(delta/2, _state), _state);
    beta = ae_sin(delta, _state);
    
    /*
     * First, decide: should we use "safe" formula (guarded
     * against overflow) or fast one?
     */
    ca = ae_cos(a0, _state);
    sa = ae_sin(a0, _state);
    j = 0;
    x = ca;
    s = t-x;
    for(i=1; i<=n-1; i++)
    {
        
        /*
         * Next X[i]
         */
        temps = sa-(alpha*sa-beta*ca);
        tempc = ca-(alpha*ca+beta*sa);
        sa = temps;
        ca = tempc;
        x = ca;
        
        /*
         * Use X[i]
         */
        if( ae_fp_less(ae_fabs(t-x, _state),ae_fabs(s, _state)) )
        {
            s = t-x;
            j = i;
        }
    }
    if( ae_fp_eq(s,(double)(0)) )
    {
        result = f->ptr.p_double[j];
        return result;
    }
    if( ae_fp_greater(ae_fabs(s, _state),threshold) )
    {
        
        /*
         * use fast formula
         */
        j = -1;
        s = 1.0;
    }
    
    /*
     * Calculate using safe or fast barycentric formula
     */
    s1 = (double)(0);
    s2 = (double)(0);
    ca = ae_cos(a0, _state);
    sa = ae_sin(a0, _state);
    p1 = 1.0;
    for(i=0; i<=n-1; i++)
    {
        
        /*
         * Calculate X[i], W[i]
         */
        x = ca;
        w = p1*sa;
        
        /*
         * Proceed
         */
        if( i!=j )
        {
            v = s*w/(t-x);
            s1 = s1+v*f->ptr.p_double[i];
            s2 = s2+v;
        }
        else
        {
            v = w;
            s1 = s1+v*f->ptr.p_double[i];
            s2 = s2+v;
        }
        
        /*
         * Next CA, SA, P1
         */
        temps = sa-(alpha*sa-beta*ca);
        tempc = ca-(alpha*ca+beta*sa);
        sa = temps;
        ca = tempc;
        p1 = -p1;
    }
    result = s1/s2;
    return result;
}


/*************************************************************************
Fast polynomial interpolation function on Chebyshev points (second kind)
with O(N) complexity.

INPUT PARAMETERS:
    A   -   left boundary of [A,B]
    B   -   right boundary of [A,B]
    F   -   function values, array[0..N-1]
    N   -   number of points on Chebyshev grid (second kind),
            X[i] = 0.5*(B+A) + 0.5*(B-A)*Cos(PI*i/(n-1))
            for N=1 a constant model is constructed.
    T   -   position where P(x) is calculated

RESULT
    value of the Lagrange interpolant at T

IMPORTANT
    this function provides fast interface which is not overflow-safe
    nor it is very precise.
    the best option is to use PolIntBuildCheb2()/BarycentricCalc()
    subroutines unless you are pretty sure that your data will not result
    in overflow.

  -- ALGLIB --
     Copyright 02.12.2009 by Bochkanov Sergey
*************************************************************************/
double polynomialcalccheb2(double a,
     double b,
     /* Real    */ ae_vector* f,
     ae_int_t n,
     double t,
     ae_state *_state)
{
    double s1;
    double s2;
    double v;
    double threshold;
    double s;
    ae_int_t i;
    ae_int_t j;
    double a0;
    double delta;
    double alpha;
    double beta;
    double ca;
    double sa;
    double tempc;
    double temps;
    double x;
    double w;
    double p1;
    double result;


    ae_assert(n>0, "PolynomialCalcCheb2: N<=0!", _state);
    ae_assert(f->cnt>=n, "PolynomialCalcCheb2: Length(F)<N!", _state);
    ae_assert(ae_isfinite(a, _state), "PolynomialCalcCheb2: A is infinite or NaN!", _state);
    ae_assert(ae_isfinite(b, _state), "PolynomialCalcCheb2: B is infinite or NaN!", _state);
    ae_assert(ae_fp_neq(b,a), "PolynomialCalcCheb2: B=A!", _state);
    ae_assert(isfinitevector(f, n, _state), "PolynomialCalcCheb2: F contains infinite or NaN values!", _state);
    ae_assert(!ae_isinf(t, _state), "PolynomialCalcEqDist: T is infinite!", _state);
    
    /*
     * Special case: T is NAN
     */
    if( ae_isnan(t, _state) )
    {
        result = _state->v_nan;
        return result;
    }
    
    /*
     * Special case: N=1
     */
    if( n==1 )
    {
        result = f->ptr.p_double[0];
        return result;
    }
    
    /*
     * Prepare information for the recurrence formula
     * used to calculate sin(pi*i/n) and
     * cos(pi*i/n):
     *
     * A0    = 0
     * Delta = pi/n
     * Alpha = 2 sin^2 (Delta/2)
     * Beta  = sin(Delta)
     *
     * so that sin(..) = sin(A0+j*delta) and cos(..) = cos(A0+j*delta).
     * Then we use
     *
     * sin(x+delta) = sin(x) - (alpha*sin(x) - beta*cos(x))
     * cos(x+delta) = cos(x) - (alpha*cos(x) - beta*sin(x))
     *
     * to repeatedly calculate sin(..) and cos(..).
     */
    threshold = ae_sqrt(ae_minrealnumber, _state);
    t = (t-0.5*(a+b))/(0.5*(b-a));
    a0 = 0.0;
    delta = ae_pi/(n-1);
    alpha = 2*ae_sqr(ae_sin(delta/2, _state), _state);
    beta = ae_sin(delta, _state);
    
    /*
     * First, decide: should we use "safe" formula (guarded
     * against overflow) or fast one?
     */
    ca = ae_cos(a0, _state);
    sa = ae_sin(a0, _state);
    j = 0;
    x = ca;
    s = t-x;
    for(i=1; i<=n-1; i++)
    {
        
        /*
         * Next X[i]
         */
        temps = sa-(alpha*sa-beta*ca);
        tempc = ca-(alpha*ca+beta*sa);
        sa = temps;
        ca = tempc;
        x = ca;
        
        /*
         * Use X[i]
         */
        if( ae_fp_less(ae_fabs(t-x, _state),ae_fabs(s, _state)) )
        {
            s = t-x;
            j = i;
        }
    }
    if( ae_fp_eq(s,(double)(0)) )
    {
        result = f->ptr.p_double[j];
        return result;
    }
    if( ae_fp_greater(ae_fabs(s, _state),threshold) )
    {
        
        /*
         * use fast formula
         */
        j = -1;
        s = 1.0;
    }
    
    /*
     * Calculate using safe or fast barycentric formula
     */
    s1 = (double)(0);
    s2 = (double)(0);
    ca = ae_cos(a0, _state);
    sa = ae_sin(a0, _state);
    p1 = 1.0;
    for(i=0; i<=n-1; i++)
    {
        
        /*
         * Calculate X[i], W[i]
         */
        x = ca;
        if( i==0||i==n-1 )
        {
            w = 0.5*p1;
        }
        else
        {
            w = 1.0*p1;
        }
        
        /*
         * Proceed
         */
        if( i!=j )
        {
            v = s*w/(t-x);
            s1 = s1+v*f->ptr.p_double[i];
            s2 = s2+v;
        }
        else
        {
            v = w;
            s1 = s1+v*f->ptr.p_double[i];
            s2 = s2+v;
        }
        
        /*
         * Next CA, SA, P1
         */
        temps = sa-(alpha*sa-beta*ca);
        tempc = ca-(alpha*ca+beta*sa);
        sa = temps;
        ca = tempc;
        p1 = -p1;
    }
    result = s1/s2;
    return result;
}


/*$ End $*/
