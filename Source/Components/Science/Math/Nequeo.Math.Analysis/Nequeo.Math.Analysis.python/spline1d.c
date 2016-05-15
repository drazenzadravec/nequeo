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
#include "spline1d.h"


/*$ Declarations $*/
static void spline1d_spline1dgriddiffcubicinternal(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t boundltype,
     double boundl,
     ae_int_t boundrtype,
     double boundr,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_vector* a1,
     /* Real    */ ae_vector* a2,
     /* Real    */ ae_vector* a3,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* dt,
     ae_state *_state);
static void spline1d_heapsortpoints(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_state *_state);
static void spline1d_heapsortppoints(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     ae_state *_state);
static void spline1d_solvetridiagonal(/* Real    */ ae_vector* a,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* c,
     /* Real    */ ae_vector* d,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     ae_state *_state);
static void spline1d_solvecyclictridiagonal(/* Real    */ ae_vector* a,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* c,
     /* Real    */ ae_vector* d,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     ae_state *_state);
static double spline1d_diffthreepoint(double t,
     double x0,
     double f0,
     double x1,
     double f1,
     double x2,
     double f2,
     ae_state *_state);
static void spline1d_hermitecalc(double p0,
     double m0,
     double p1,
     double m1,
     double t,
     double* s,
     double* ds,
     ae_state *_state);
static double spline1d_rescaleval(double a0,
     double b0,
     double a1,
     double b1,
     double t,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
This subroutine builds linear spline interpolant

INPUT PARAMETERS:
    X   -   spline nodes, array[0..N-1]
    Y   -   function values, array[0..N-1]
    N   -   points count (optional):
            * N>=2
            * if given, only first N points are used to build spline
            * if not given, automatically detected from X/Y sizes
              (len(X) must be equal to len(Y))
    
OUTPUT PARAMETERS:
    C   -   spline interpolant


ORDER OF POINTS

Subroutine automatically sorts points, so caller may pass unsorted array.

  -- ALGLIB PROJECT --
     Copyright 24.06.2007 by Bochkanov Sergey
*************************************************************************/
void spline1dbuildlinear(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     spline1dinterpolant* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    _spline1dinterpolant_clear(c);

    ae_assert(n>1, "Spline1DBuildLinear: N<2!", _state);
    ae_assert(x->cnt>=n, "Spline1DBuildLinear: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Spline1DBuildLinear: Length(Y)<N!", _state);
    
    /*
     * check and sort points
     */
    ae_assert(isfinitevector(x, n, _state), "Spline1DBuildLinear: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(y, n, _state), "Spline1DBuildLinear: Y contains infinite or NAN values!", _state);
    spline1d_heapsortpoints(x, y, n, _state);
    ae_assert(aredistinct(x, n, _state), "Spline1DBuildLinear: at least two consequent points are too close!", _state);
    
    /*
     * Build
     */
    c->periodic = ae_false;
    c->n = n;
    c->k = 3;
    c->continuity = 0;
    ae_vector_set_length(&c->x, n, _state);
    ae_vector_set_length(&c->c, 4*(n-1)+2, _state);
    for(i=0; i<=n-1; i++)
    {
        c->x.ptr.p_double[i] = x->ptr.p_double[i];
    }
    for(i=0; i<=n-2; i++)
    {
        c->c.ptr.p_double[4*i+0] = y->ptr.p_double[i];
        c->c.ptr.p_double[4*i+1] = (y->ptr.p_double[i+1]-y->ptr.p_double[i])/(x->ptr.p_double[i+1]-x->ptr.p_double[i]);
        c->c.ptr.p_double[4*i+2] = (double)(0);
        c->c.ptr.p_double[4*i+3] = (double)(0);
    }
    c->c.ptr.p_double[4*(n-1)+0] = y->ptr.p_double[n-1];
    c->c.ptr.p_double[4*(n-1)+1] = c->c.ptr.p_double[4*(n-2)+1];
    ae_frame_leave(_state);
}


/*************************************************************************
This subroutine builds cubic spline interpolant.

INPUT PARAMETERS:
    X           -   spline nodes, array[0..N-1].
    Y           -   function values, array[0..N-1].
    
OPTIONAL PARAMETERS:
    N           -   points count:
                    * N>=2
                    * if given, only first N points are used to build spline
                    * if not given, automatically detected from X/Y sizes
                      (len(X) must be equal to len(Y))
    BoundLType  -   boundary condition type for the left boundary
    BoundL      -   left boundary condition (first or second derivative,
                    depending on the BoundLType)
    BoundRType  -   boundary condition type for the right boundary
    BoundR      -   right boundary condition (first or second derivative,
                    depending on the BoundRType)

OUTPUT PARAMETERS:
    C           -   spline interpolant

ORDER OF POINTS

Subroutine automatically sorts points, so caller may pass unsorted array.

SETTING BOUNDARY VALUES:

The BoundLType/BoundRType parameters can have the following values:
    * -1, which corresonds to the periodic (cyclic) boundary conditions.
          In this case:
          * both BoundLType and BoundRType must be equal to -1.
          * BoundL/BoundR are ignored
          * Y[last] is ignored (it is assumed to be equal to Y[first]).
    *  0, which  corresponds  to  the  parabolically   terminated  spline
          (BoundL and/or BoundR are ignored).
    *  1, which corresponds to the first derivative boundary condition
    *  2, which corresponds to the second derivative boundary condition
    *  by default, BoundType=0 is used

PROBLEMS WITH PERIODIC BOUNDARY CONDITIONS:

Problems with periodic boundary conditions have Y[first_point]=Y[last_point].
However, this subroutine doesn't require you to specify equal  values  for
the first and last points - it automatically forces them  to  be  equal by
copying  Y[first_point]  (corresponds  to the leftmost,  minimal  X[])  to
Y[last_point]. However it is recommended to pass consistent values of Y[],
i.e. to make Y[first_point]=Y[last_point].

  -- ALGLIB PROJECT --
     Copyright 23.06.2007 by Bochkanov Sergey
*************************************************************************/
void spline1dbuildcubic(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t boundltype,
     double boundl,
     ae_int_t boundrtype,
     double boundr,
     spline1dinterpolant* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_vector a1;
    ae_vector a2;
    ae_vector a3;
    ae_vector b;
    ae_vector dt;
    ae_vector d;
    ae_vector p;
    ae_int_t ylen;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    _spline1dinterpolant_clear(c);
    ae_vector_init(&a1, 0, DT_REAL, _state);
    ae_vector_init(&a2, 0, DT_REAL, _state);
    ae_vector_init(&a3, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&dt, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&p, 0, DT_INT, _state);

    
    /*
     * check correctness of boundary conditions
     */
    ae_assert(((boundltype==-1||boundltype==0)||boundltype==1)||boundltype==2, "Spline1DBuildCubic: incorrect BoundLType!", _state);
    ae_assert(((boundrtype==-1||boundrtype==0)||boundrtype==1)||boundrtype==2, "Spline1DBuildCubic: incorrect BoundRType!", _state);
    ae_assert((boundrtype==-1&&boundltype==-1)||(boundrtype!=-1&&boundltype!=-1), "Spline1DBuildCubic: incorrect BoundLType/BoundRType!", _state);
    if( boundltype==1||boundltype==2 )
    {
        ae_assert(ae_isfinite(boundl, _state), "Spline1DBuildCubic: BoundL is infinite or NAN!", _state);
    }
    if( boundrtype==1||boundrtype==2 )
    {
        ae_assert(ae_isfinite(boundr, _state), "Spline1DBuildCubic: BoundR is infinite or NAN!", _state);
    }
    
    /*
     * check lengths of arguments
     */
    ae_assert(n>=2, "Spline1DBuildCubic: N<2!", _state);
    ae_assert(x->cnt>=n, "Spline1DBuildCubic: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Spline1DBuildCubic: Length(Y)<N!", _state);
    
    /*
     * check and sort points
     */
    ylen = n;
    if( boundltype==-1 )
    {
        ylen = n-1;
    }
    ae_assert(isfinitevector(x, n, _state), "Spline1DBuildCubic: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(y, ylen, _state), "Spline1DBuildCubic: Y contains infinite or NAN values!", _state);
    spline1d_heapsortppoints(x, y, &p, n, _state);
    ae_assert(aredistinct(x, n, _state), "Spline1DBuildCubic: at least two consequent points are too close!", _state);
    
    /*
     * Now we've checked and preordered everything,
     * so we can call internal function to calculate derivatives,
     * and then build Hermite spline using these derivatives
     */
    if( boundltype==-1||boundrtype==-1 )
    {
        y->ptr.p_double[n-1] = y->ptr.p_double[0];
    }
    spline1d_spline1dgriddiffcubicinternal(x, y, n, boundltype, boundl, boundrtype, boundr, &d, &a1, &a2, &a3, &b, &dt, _state);
    spline1dbuildhermite(x, y, &d, n, c, _state);
    c->periodic = boundltype==-1||boundrtype==-1;
    c->continuity = 2;
    ae_frame_leave(_state);
}


/*************************************************************************
This function solves following problem: given table y[] of function values
at nodes x[], it calculates and returns table of function derivatives  d[]
(calculated at the same nodes x[]).

This function yields same result as Spline1DBuildCubic() call followed  by
sequence of Spline1DDiff() calls, but it can be several times faster  when
called for ordered X[] and X2[].

INPUT PARAMETERS:
    X           -   spline nodes
    Y           -   function values

OPTIONAL PARAMETERS:
    N           -   points count:
                    * N>=2
                    * if given, only first N points are used
                    * if not given, automatically detected from X/Y sizes
                      (len(X) must be equal to len(Y))
    BoundLType  -   boundary condition type for the left boundary
    BoundL      -   left boundary condition (first or second derivative,
                    depending on the BoundLType)
    BoundRType  -   boundary condition type for the right boundary
    BoundR      -   right boundary condition (first or second derivative,
                    depending on the BoundRType)

OUTPUT PARAMETERS:
    D           -   derivative values at X[]

ORDER OF POINTS

Subroutine automatically sorts points, so caller may pass unsorted array.
Derivative values are correctly reordered on return, so  D[I]  is  always
equal to S'(X[I]) independently of points order.

SETTING BOUNDARY VALUES:

The BoundLType/BoundRType parameters can have the following values:
    * -1, which corresonds to the periodic (cyclic) boundary conditions.
          In this case:
          * both BoundLType and BoundRType must be equal to -1.
          * BoundL/BoundR are ignored
          * Y[last] is ignored (it is assumed to be equal to Y[first]).
    *  0, which  corresponds  to  the  parabolically   terminated  spline
          (BoundL and/or BoundR are ignored).
    *  1, which corresponds to the first derivative boundary condition
    *  2, which corresponds to the second derivative boundary condition
    *  by default, BoundType=0 is used

PROBLEMS WITH PERIODIC BOUNDARY CONDITIONS:

Problems with periodic boundary conditions have Y[first_point]=Y[last_point].
However, this subroutine doesn't require you to specify equal  values  for
the first and last points - it automatically forces them  to  be  equal by
copying  Y[first_point]  (corresponds  to the leftmost,  minimal  X[])  to
Y[last_point]. However it is recommended to pass consistent values of Y[],
i.e. to make Y[first_point]=Y[last_point].

  -- ALGLIB PROJECT --
     Copyright 03.09.2010 by Bochkanov Sergey
*************************************************************************/
void spline1dgriddiffcubic(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t boundltype,
     double boundl,
     ae_int_t boundrtype,
     double boundr,
     /* Real    */ ae_vector* d,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_vector a1;
    ae_vector a2;
    ae_vector a3;
    ae_vector b;
    ae_vector dt;
    ae_vector p;
    ae_int_t i;
    ae_int_t ylen;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    ae_vector_clear(d);
    ae_vector_init(&a1, 0, DT_REAL, _state);
    ae_vector_init(&a2, 0, DT_REAL, _state);
    ae_vector_init(&a3, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&dt, 0, DT_REAL, _state);
    ae_vector_init(&p, 0, DT_INT, _state);

    
    /*
     * check correctness of boundary conditions
     */
    ae_assert(((boundltype==-1||boundltype==0)||boundltype==1)||boundltype==2, "Spline1DGridDiffCubic: incorrect BoundLType!", _state);
    ae_assert(((boundrtype==-1||boundrtype==0)||boundrtype==1)||boundrtype==2, "Spline1DGridDiffCubic: incorrect BoundRType!", _state);
    ae_assert((boundrtype==-1&&boundltype==-1)||(boundrtype!=-1&&boundltype!=-1), "Spline1DGridDiffCubic: incorrect BoundLType/BoundRType!", _state);
    if( boundltype==1||boundltype==2 )
    {
        ae_assert(ae_isfinite(boundl, _state), "Spline1DGridDiffCubic: BoundL is infinite or NAN!", _state);
    }
    if( boundrtype==1||boundrtype==2 )
    {
        ae_assert(ae_isfinite(boundr, _state), "Spline1DGridDiffCubic: BoundR is infinite or NAN!", _state);
    }
    
    /*
     * check lengths of arguments
     */
    ae_assert(n>=2, "Spline1DGridDiffCubic: N<2!", _state);
    ae_assert(x->cnt>=n, "Spline1DGridDiffCubic: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Spline1DGridDiffCubic: Length(Y)<N!", _state);
    
    /*
     * check and sort points
     */
    ylen = n;
    if( boundltype==-1 )
    {
        ylen = n-1;
    }
    ae_assert(isfinitevector(x, n, _state), "Spline1DGridDiffCubic: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(y, ylen, _state), "Spline1DGridDiffCubic: Y contains infinite or NAN values!", _state);
    spline1d_heapsortppoints(x, y, &p, n, _state);
    ae_assert(aredistinct(x, n, _state), "Spline1DGridDiffCubic: at least two consequent points are too close!", _state);
    
    /*
     * Now we've checked and preordered everything,
     * so we can call internal function.
     */
    spline1d_spline1dgriddiffcubicinternal(x, y, n, boundltype, boundl, boundrtype, boundr, d, &a1, &a2, &a3, &b, &dt, _state);
    
    /*
     * Remember that HeapSortPPoints() call?
     * Now we have to reorder them back.
     */
    if( dt.cnt<n )
    {
        ae_vector_set_length(&dt, n, _state);
    }
    for(i=0; i<=n-1; i++)
    {
        dt.ptr.p_double[p.ptr.p_int[i]] = d->ptr.p_double[i];
    }
    ae_v_move(&d->ptr.p_double[0], 1, &dt.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_frame_leave(_state);
}


/*************************************************************************
This function solves following problem: given table y[] of function values
at  nodes  x[],  it  calculates  and  returns  tables  of first and second
function derivatives d1[] and d2[] (calculated at the same nodes x[]).

This function yields same result as Spline1DBuildCubic() call followed  by
sequence of Spline1DDiff() calls, but it can be several times faster  when
called for ordered X[] and X2[].

INPUT PARAMETERS:
    X           -   spline nodes
    Y           -   function values

OPTIONAL PARAMETERS:
    N           -   points count:
                    * N>=2
                    * if given, only first N points are used
                    * if not given, automatically detected from X/Y sizes
                      (len(X) must be equal to len(Y))
    BoundLType  -   boundary condition type for the left boundary
    BoundL      -   left boundary condition (first or second derivative,
                    depending on the BoundLType)
    BoundRType  -   boundary condition type for the right boundary
    BoundR      -   right boundary condition (first or second derivative,
                    depending on the BoundRType)

OUTPUT PARAMETERS:
    D1          -   S' values at X[]
    D2          -   S'' values at X[]

ORDER OF POINTS

Subroutine automatically sorts points, so caller may pass unsorted array.
Derivative values are correctly reordered on return, so  D[I]  is  always
equal to S'(X[I]) independently of points order.

SETTING BOUNDARY VALUES:

The BoundLType/BoundRType parameters can have the following values:
    * -1, which corresonds to the periodic (cyclic) boundary conditions.
          In this case:
          * both BoundLType and BoundRType must be equal to -1.
          * BoundL/BoundR are ignored
          * Y[last] is ignored (it is assumed to be equal to Y[first]).
    *  0, which  corresponds  to  the  parabolically   terminated  spline
          (BoundL and/or BoundR are ignored).
    *  1, which corresponds to the first derivative boundary condition
    *  2, which corresponds to the second derivative boundary condition
    *  by default, BoundType=0 is used

PROBLEMS WITH PERIODIC BOUNDARY CONDITIONS:

Problems with periodic boundary conditions have Y[first_point]=Y[last_point].
However, this subroutine doesn't require you to specify equal  values  for
the first and last points - it automatically forces them  to  be  equal by
copying  Y[first_point]  (corresponds  to the leftmost,  minimal  X[])  to
Y[last_point]. However it is recommended to pass consistent values of Y[],
i.e. to make Y[first_point]=Y[last_point].

  -- ALGLIB PROJECT --
     Copyright 03.09.2010 by Bochkanov Sergey
*************************************************************************/
void spline1dgriddiff2cubic(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t boundltype,
     double boundl,
     ae_int_t boundrtype,
     double boundr,
     /* Real    */ ae_vector* d1,
     /* Real    */ ae_vector* d2,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_vector a1;
    ae_vector a2;
    ae_vector a3;
    ae_vector b;
    ae_vector dt;
    ae_vector p;
    ae_int_t i;
    ae_int_t ylen;
    double delta;
    double delta2;
    double delta3;
    double s2;
    double s3;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    ae_vector_clear(d1);
    ae_vector_clear(d2);
    ae_vector_init(&a1, 0, DT_REAL, _state);
    ae_vector_init(&a2, 0, DT_REAL, _state);
    ae_vector_init(&a3, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&dt, 0, DT_REAL, _state);
    ae_vector_init(&p, 0, DT_INT, _state);

    
    /*
     * check correctness of boundary conditions
     */
    ae_assert(((boundltype==-1||boundltype==0)||boundltype==1)||boundltype==2, "Spline1DGridDiff2Cubic: incorrect BoundLType!", _state);
    ae_assert(((boundrtype==-1||boundrtype==0)||boundrtype==1)||boundrtype==2, "Spline1DGridDiff2Cubic: incorrect BoundRType!", _state);
    ae_assert((boundrtype==-1&&boundltype==-1)||(boundrtype!=-1&&boundltype!=-1), "Spline1DGridDiff2Cubic: incorrect BoundLType/BoundRType!", _state);
    if( boundltype==1||boundltype==2 )
    {
        ae_assert(ae_isfinite(boundl, _state), "Spline1DGridDiff2Cubic: BoundL is infinite or NAN!", _state);
    }
    if( boundrtype==1||boundrtype==2 )
    {
        ae_assert(ae_isfinite(boundr, _state), "Spline1DGridDiff2Cubic: BoundR is infinite or NAN!", _state);
    }
    
    /*
     * check lengths of arguments
     */
    ae_assert(n>=2, "Spline1DGridDiff2Cubic: N<2!", _state);
    ae_assert(x->cnt>=n, "Spline1DGridDiff2Cubic: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Spline1DGridDiff2Cubic: Length(Y)<N!", _state);
    
    /*
     * check and sort points
     */
    ylen = n;
    if( boundltype==-1 )
    {
        ylen = n-1;
    }
    ae_assert(isfinitevector(x, n, _state), "Spline1DGridDiff2Cubic: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(y, ylen, _state), "Spline1DGridDiff2Cubic: Y contains infinite or NAN values!", _state);
    spline1d_heapsortppoints(x, y, &p, n, _state);
    ae_assert(aredistinct(x, n, _state), "Spline1DGridDiff2Cubic: at least two consequent points are too close!", _state);
    
    /*
     * Now we've checked and preordered everything,
     * so we can call internal function.
     *
     * After this call we will calculate second derivatives
     * (manually, by converting to the power basis)
     */
    spline1d_spline1dgriddiffcubicinternal(x, y, n, boundltype, boundl, boundrtype, boundr, d1, &a1, &a2, &a3, &b, &dt, _state);
    ae_vector_set_length(d2, n, _state);
    delta = (double)(0);
    s2 = (double)(0);
    s3 = (double)(0);
    for(i=0; i<=n-2; i++)
    {
        
        /*
         * We convert from Hermite basis to the power basis.
         * Si is coefficient before x^i.
         *
         * Inside this cycle we need just S2,
         * because we calculate S'' exactly at spline node,
         * (only x^2 matters at x=0), but after iterations
         * will be over, we will need other coefficients
         * to calculate spline value at the last node.
         */
        delta = x->ptr.p_double[i+1]-x->ptr.p_double[i];
        delta2 = ae_sqr(delta, _state);
        delta3 = delta*delta2;
        s2 = (3*(y->ptr.p_double[i+1]-y->ptr.p_double[i])-2*d1->ptr.p_double[i]*delta-d1->ptr.p_double[i+1]*delta)/delta2;
        s3 = (2*(y->ptr.p_double[i]-y->ptr.p_double[i+1])+d1->ptr.p_double[i]*delta+d1->ptr.p_double[i+1]*delta)/delta3;
        d2->ptr.p_double[i] = 2*s2;
    }
    d2->ptr.p_double[n-1] = 2*s2+6*s3*delta;
    
    /*
     * Remember that HeapSortPPoints() call?
     * Now we have to reorder them back.
     */
    if( dt.cnt<n )
    {
        ae_vector_set_length(&dt, n, _state);
    }
    for(i=0; i<=n-1; i++)
    {
        dt.ptr.p_double[p.ptr.p_int[i]] = d1->ptr.p_double[i];
    }
    ae_v_move(&d1->ptr.p_double[0], 1, &dt.ptr.p_double[0], 1, ae_v_len(0,n-1));
    for(i=0; i<=n-1; i++)
    {
        dt.ptr.p_double[p.ptr.p_int[i]] = d2->ptr.p_double[i];
    }
    ae_v_move(&d2->ptr.p_double[0], 1, &dt.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_frame_leave(_state);
}


/*************************************************************************
This function solves following problem: given table y[] of function values
at old nodes x[]  and new nodes  x2[],  it calculates and returns table of
function values y2[] (calculated at x2[]).

This function yields same result as Spline1DBuildCubic() call followed  by
sequence of Spline1DDiff() calls, but it can be several times faster  when
called for ordered X[] and X2[].

INPUT PARAMETERS:
    X           -   old spline nodes
    Y           -   function values
    X2           -  new spline nodes

OPTIONAL PARAMETERS:
    N           -   points count:
                    * N>=2
                    * if given, only first N points from X/Y are used
                    * if not given, automatically detected from X/Y sizes
                      (len(X) must be equal to len(Y))
    BoundLType  -   boundary condition type for the left boundary
    BoundL      -   left boundary condition (first or second derivative,
                    depending on the BoundLType)
    BoundRType  -   boundary condition type for the right boundary
    BoundR      -   right boundary condition (first or second derivative,
                    depending on the BoundRType)
    N2          -   new points count:
                    * N2>=2
                    * if given, only first N2 points from X2 are used
                    * if not given, automatically detected from X2 size

OUTPUT PARAMETERS:
    F2          -   function values at X2[]

ORDER OF POINTS

Subroutine automatically sorts points, so caller  may pass unsorted array.
Function  values  are correctly reordered on  return, so F2[I]  is  always
equal to S(X2[I]) independently of points order.

SETTING BOUNDARY VALUES:

The BoundLType/BoundRType parameters can have the following values:
    * -1, which corresonds to the periodic (cyclic) boundary conditions.
          In this case:
          * both BoundLType and BoundRType must be equal to -1.
          * BoundL/BoundR are ignored
          * Y[last] is ignored (it is assumed to be equal to Y[first]).
    *  0, which  corresponds  to  the  parabolically   terminated  spline
          (BoundL and/or BoundR are ignored).
    *  1, which corresponds to the first derivative boundary condition
    *  2, which corresponds to the second derivative boundary condition
    *  by default, BoundType=0 is used

PROBLEMS WITH PERIODIC BOUNDARY CONDITIONS:

Problems with periodic boundary conditions have Y[first_point]=Y[last_point].
However, this subroutine doesn't require you to specify equal  values  for
the first and last points - it automatically forces them  to  be  equal by
copying  Y[first_point]  (corresponds  to the leftmost,  minimal  X[])  to
Y[last_point]. However it is recommended to pass consistent values of Y[],
i.e. to make Y[first_point]=Y[last_point].

  -- ALGLIB PROJECT --
     Copyright 03.09.2010 by Bochkanov Sergey
*************************************************************************/
void spline1dconvcubic(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t boundltype,
     double boundl,
     ae_int_t boundrtype,
     double boundr,
     /* Real    */ ae_vector* x2,
     ae_int_t n2,
     /* Real    */ ae_vector* y2,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_vector _x2;
    ae_vector a1;
    ae_vector a2;
    ae_vector a3;
    ae_vector b;
    ae_vector d;
    ae_vector dt;
    ae_vector d1;
    ae_vector d2;
    ae_vector p;
    ae_vector p2;
    ae_int_t i;
    ae_int_t ylen;
    double t;
    double t2;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    ae_vector_init_copy(&_x2, x2, _state);
    x2 = &_x2;
    ae_vector_clear(y2);
    ae_vector_init(&a1, 0, DT_REAL, _state);
    ae_vector_init(&a2, 0, DT_REAL, _state);
    ae_vector_init(&a3, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&dt, 0, DT_REAL, _state);
    ae_vector_init(&d1, 0, DT_REAL, _state);
    ae_vector_init(&d2, 0, DT_REAL, _state);
    ae_vector_init(&p, 0, DT_INT, _state);
    ae_vector_init(&p2, 0, DT_INT, _state);

    
    /*
     * check correctness of boundary conditions
     */
    ae_assert(((boundltype==-1||boundltype==0)||boundltype==1)||boundltype==2, "Spline1DConvCubic: incorrect BoundLType!", _state);
    ae_assert(((boundrtype==-1||boundrtype==0)||boundrtype==1)||boundrtype==2, "Spline1DConvCubic: incorrect BoundRType!", _state);
    ae_assert((boundrtype==-1&&boundltype==-1)||(boundrtype!=-1&&boundltype!=-1), "Spline1DConvCubic: incorrect BoundLType/BoundRType!", _state);
    if( boundltype==1||boundltype==2 )
    {
        ae_assert(ae_isfinite(boundl, _state), "Spline1DConvCubic: BoundL is infinite or NAN!", _state);
    }
    if( boundrtype==1||boundrtype==2 )
    {
        ae_assert(ae_isfinite(boundr, _state), "Spline1DConvCubic: BoundR is infinite or NAN!", _state);
    }
    
    /*
     * check lengths of arguments
     */
    ae_assert(n>=2, "Spline1DConvCubic: N<2!", _state);
    ae_assert(x->cnt>=n, "Spline1DConvCubic: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Spline1DConvCubic: Length(Y)<N!", _state);
    ae_assert(n2>=2, "Spline1DConvCubic: N2<2!", _state);
    ae_assert(x2->cnt>=n2, "Spline1DConvCubic: Length(X2)<N2!", _state);
    
    /*
     * check and sort X/Y
     */
    ylen = n;
    if( boundltype==-1 )
    {
        ylen = n-1;
    }
    ae_assert(isfinitevector(x, n, _state), "Spline1DConvCubic: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(y, ylen, _state), "Spline1DConvCubic: Y contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(x2, n2, _state), "Spline1DConvCubic: X2 contains infinite or NAN values!", _state);
    spline1d_heapsortppoints(x, y, &p, n, _state);
    ae_assert(aredistinct(x, n, _state), "Spline1DConvCubic: at least two consequent points are too close!", _state);
    
    /*
     * set up DT (we will need it below)
     */
    ae_vector_set_length(&dt, ae_maxint(n, n2, _state), _state);
    
    /*
     * sort X2:
     * * use fake array DT because HeapSortPPoints() needs both integer AND real arrays
     * * if we have periodic problem, wrap points
     * * sort them, store permutation at P2
     */
    if( boundrtype==-1&&boundltype==-1 )
    {
        for(i=0; i<=n2-1; i++)
        {
            t = x2->ptr.p_double[i];
            apperiodicmap(&t, x->ptr.p_double[0], x->ptr.p_double[n-1], &t2, _state);
            x2->ptr.p_double[i] = t;
        }
    }
    spline1d_heapsortppoints(x2, &dt, &p2, n2, _state);
    
    /*
     * Now we've checked and preordered everything, so we:
     * * call internal GridDiff() function to get Hermite form of spline
     * * convert using internal Conv() function
     * * convert Y2 back to original order
     */
    spline1d_spline1dgriddiffcubicinternal(x, y, n, boundltype, boundl, boundrtype, boundr, &d, &a1, &a2, &a3, &b, &dt, _state);
    spline1dconvdiffinternal(x, y, &d, n, x2, n2, y2, ae_true, &d1, ae_false, &d2, ae_false, _state);
    ae_assert(dt.cnt>=n2, "Spline1DConvCubic: internal error!", _state);
    for(i=0; i<=n2-1; i++)
    {
        dt.ptr.p_double[p2.ptr.p_int[i]] = y2->ptr.p_double[i];
    }
    ae_v_move(&y2->ptr.p_double[0], 1, &dt.ptr.p_double[0], 1, ae_v_len(0,n2-1));
    ae_frame_leave(_state);
}


/*************************************************************************
This function solves following problem: given table y[] of function values
at old nodes x[]  and new nodes  x2[],  it calculates and returns table of
function values y2[] and derivatives d2[] (calculated at x2[]).

This function yields same result as Spline1DBuildCubic() call followed  by
sequence of Spline1DDiff() calls, but it can be several times faster  when
called for ordered X[] and X2[].

INPUT PARAMETERS:
    X           -   old spline nodes
    Y           -   function values
    X2           -  new spline nodes

OPTIONAL PARAMETERS:
    N           -   points count:
                    * N>=2
                    * if given, only first N points from X/Y are used
                    * if not given, automatically detected from X/Y sizes
                      (len(X) must be equal to len(Y))
    BoundLType  -   boundary condition type for the left boundary
    BoundL      -   left boundary condition (first or second derivative,
                    depending on the BoundLType)
    BoundRType  -   boundary condition type for the right boundary
    BoundR      -   right boundary condition (first or second derivative,
                    depending on the BoundRType)
    N2          -   new points count:
                    * N2>=2
                    * if given, only first N2 points from X2 are used
                    * if not given, automatically detected from X2 size

OUTPUT PARAMETERS:
    F2          -   function values at X2[]
    D2          -   first derivatives at X2[]

ORDER OF POINTS

Subroutine automatically sorts points, so caller  may pass unsorted array.
Function  values  are correctly reordered on  return, so F2[I]  is  always
equal to S(X2[I]) independently of points order.

SETTING BOUNDARY VALUES:

The BoundLType/BoundRType parameters can have the following values:
    * -1, which corresonds to the periodic (cyclic) boundary conditions.
          In this case:
          * both BoundLType and BoundRType must be equal to -1.
          * BoundL/BoundR are ignored
          * Y[last] is ignored (it is assumed to be equal to Y[first]).
    *  0, which  corresponds  to  the  parabolically   terminated  spline
          (BoundL and/or BoundR are ignored).
    *  1, which corresponds to the first derivative boundary condition
    *  2, which corresponds to the second derivative boundary condition
    *  by default, BoundType=0 is used

PROBLEMS WITH PERIODIC BOUNDARY CONDITIONS:

Problems with periodic boundary conditions have Y[first_point]=Y[last_point].
However, this subroutine doesn't require you to specify equal  values  for
the first and last points - it automatically forces them  to  be  equal by
copying  Y[first_point]  (corresponds  to the leftmost,  minimal  X[])  to
Y[last_point]. However it is recommended to pass consistent values of Y[],
i.e. to make Y[first_point]=Y[last_point].

  -- ALGLIB PROJECT --
     Copyright 03.09.2010 by Bochkanov Sergey
*************************************************************************/
void spline1dconvdiffcubic(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t boundltype,
     double boundl,
     ae_int_t boundrtype,
     double boundr,
     /* Real    */ ae_vector* x2,
     ae_int_t n2,
     /* Real    */ ae_vector* y2,
     /* Real    */ ae_vector* d2,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_vector _x2;
    ae_vector a1;
    ae_vector a2;
    ae_vector a3;
    ae_vector b;
    ae_vector d;
    ae_vector dt;
    ae_vector rt1;
    ae_vector p;
    ae_vector p2;
    ae_int_t i;
    ae_int_t ylen;
    double t;
    double t2;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    ae_vector_init_copy(&_x2, x2, _state);
    x2 = &_x2;
    ae_vector_clear(y2);
    ae_vector_clear(d2);
    ae_vector_init(&a1, 0, DT_REAL, _state);
    ae_vector_init(&a2, 0, DT_REAL, _state);
    ae_vector_init(&a3, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&dt, 0, DT_REAL, _state);
    ae_vector_init(&rt1, 0, DT_REAL, _state);
    ae_vector_init(&p, 0, DT_INT, _state);
    ae_vector_init(&p2, 0, DT_INT, _state);

    
    /*
     * check correctness of boundary conditions
     */
    ae_assert(((boundltype==-1||boundltype==0)||boundltype==1)||boundltype==2, "Spline1DConvDiffCubic: incorrect BoundLType!", _state);
    ae_assert(((boundrtype==-1||boundrtype==0)||boundrtype==1)||boundrtype==2, "Spline1DConvDiffCubic: incorrect BoundRType!", _state);
    ae_assert((boundrtype==-1&&boundltype==-1)||(boundrtype!=-1&&boundltype!=-1), "Spline1DConvDiffCubic: incorrect BoundLType/BoundRType!", _state);
    if( boundltype==1||boundltype==2 )
    {
        ae_assert(ae_isfinite(boundl, _state), "Spline1DConvDiffCubic: BoundL is infinite or NAN!", _state);
    }
    if( boundrtype==1||boundrtype==2 )
    {
        ae_assert(ae_isfinite(boundr, _state), "Spline1DConvDiffCubic: BoundR is infinite or NAN!", _state);
    }
    
    /*
     * check lengths of arguments
     */
    ae_assert(n>=2, "Spline1DConvDiffCubic: N<2!", _state);
    ae_assert(x->cnt>=n, "Spline1DConvDiffCubic: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Spline1DConvDiffCubic: Length(Y)<N!", _state);
    ae_assert(n2>=2, "Spline1DConvDiffCubic: N2<2!", _state);
    ae_assert(x2->cnt>=n2, "Spline1DConvDiffCubic: Length(X2)<N2!", _state);
    
    /*
     * check and sort X/Y
     */
    ylen = n;
    if( boundltype==-1 )
    {
        ylen = n-1;
    }
    ae_assert(isfinitevector(x, n, _state), "Spline1DConvDiffCubic: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(y, ylen, _state), "Spline1DConvDiffCubic: Y contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(x2, n2, _state), "Spline1DConvDiffCubic: X2 contains infinite or NAN values!", _state);
    spline1d_heapsortppoints(x, y, &p, n, _state);
    ae_assert(aredistinct(x, n, _state), "Spline1DConvDiffCubic: at least two consequent points are too close!", _state);
    
    /*
     * set up DT (we will need it below)
     */
    ae_vector_set_length(&dt, ae_maxint(n, n2, _state), _state);
    
    /*
     * sort X2:
     * * use fake array DT because HeapSortPPoints() needs both integer AND real arrays
     * * if we have periodic problem, wrap points
     * * sort them, store permutation at P2
     */
    if( boundrtype==-1&&boundltype==-1 )
    {
        for(i=0; i<=n2-1; i++)
        {
            t = x2->ptr.p_double[i];
            apperiodicmap(&t, x->ptr.p_double[0], x->ptr.p_double[n-1], &t2, _state);
            x2->ptr.p_double[i] = t;
        }
    }
    spline1d_heapsortppoints(x2, &dt, &p2, n2, _state);
    
    /*
     * Now we've checked and preordered everything, so we:
     * * call internal GridDiff() function to get Hermite form of spline
     * * convert using internal Conv() function
     * * convert Y2 back to original order
     */
    spline1d_spline1dgriddiffcubicinternal(x, y, n, boundltype, boundl, boundrtype, boundr, &d, &a1, &a2, &a3, &b, &dt, _state);
    spline1dconvdiffinternal(x, y, &d, n, x2, n2, y2, ae_true, d2, ae_true, &rt1, ae_false, _state);
    ae_assert(dt.cnt>=n2, "Spline1DConvDiffCubic: internal error!", _state);
    for(i=0; i<=n2-1; i++)
    {
        dt.ptr.p_double[p2.ptr.p_int[i]] = y2->ptr.p_double[i];
    }
    ae_v_move(&y2->ptr.p_double[0], 1, &dt.ptr.p_double[0], 1, ae_v_len(0,n2-1));
    for(i=0; i<=n2-1; i++)
    {
        dt.ptr.p_double[p2.ptr.p_int[i]] = d2->ptr.p_double[i];
    }
    ae_v_move(&d2->ptr.p_double[0], 1, &dt.ptr.p_double[0], 1, ae_v_len(0,n2-1));
    ae_frame_leave(_state);
}


/*************************************************************************
This function solves following problem: given table y[] of function values
at old nodes x[]  and new nodes  x2[],  it calculates and returns table of
function  values  y2[],  first  and  second  derivatives  d2[]  and  dd2[]
(calculated at x2[]).

This function yields same result as Spline1DBuildCubic() call followed  by
sequence of Spline1DDiff() calls, but it can be several times faster  when
called for ordered X[] and X2[].

INPUT PARAMETERS:
    X           -   old spline nodes
    Y           -   function values
    X2           -  new spline nodes

OPTIONAL PARAMETERS:
    N           -   points count:
                    * N>=2
                    * if given, only first N points from X/Y are used
                    * if not given, automatically detected from X/Y sizes
                      (len(X) must be equal to len(Y))
    BoundLType  -   boundary condition type for the left boundary
    BoundL      -   left boundary condition (first or second derivative,
                    depending on the BoundLType)
    BoundRType  -   boundary condition type for the right boundary
    BoundR      -   right boundary condition (first or second derivative,
                    depending on the BoundRType)
    N2          -   new points count:
                    * N2>=2
                    * if given, only first N2 points from X2 are used
                    * if not given, automatically detected from X2 size

OUTPUT PARAMETERS:
    F2          -   function values at X2[]
    D2          -   first derivatives at X2[]
    DD2         -   second derivatives at X2[]

ORDER OF POINTS

Subroutine automatically sorts points, so caller  may pass unsorted array.
Function  values  are correctly reordered on  return, so F2[I]  is  always
equal to S(X2[I]) independently of points order.

SETTING BOUNDARY VALUES:

The BoundLType/BoundRType parameters can have the following values:
    * -1, which corresonds to the periodic (cyclic) boundary conditions.
          In this case:
          * both BoundLType and BoundRType must be equal to -1.
          * BoundL/BoundR are ignored
          * Y[last] is ignored (it is assumed to be equal to Y[first]).
    *  0, which  corresponds  to  the  parabolically   terminated  spline
          (BoundL and/or BoundR are ignored).
    *  1, which corresponds to the first derivative boundary condition
    *  2, which corresponds to the second derivative boundary condition
    *  by default, BoundType=0 is used

PROBLEMS WITH PERIODIC BOUNDARY CONDITIONS:

Problems with periodic boundary conditions have Y[first_point]=Y[last_point].
However, this subroutine doesn't require you to specify equal  values  for
the first and last points - it automatically forces them  to  be  equal by
copying  Y[first_point]  (corresponds  to the leftmost,  minimal  X[])  to
Y[last_point]. However it is recommended to pass consistent values of Y[],
i.e. to make Y[first_point]=Y[last_point].

  -- ALGLIB PROJECT --
     Copyright 03.09.2010 by Bochkanov Sergey
*************************************************************************/
void spline1dconvdiff2cubic(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t boundltype,
     double boundl,
     ae_int_t boundrtype,
     double boundr,
     /* Real    */ ae_vector* x2,
     ae_int_t n2,
     /* Real    */ ae_vector* y2,
     /* Real    */ ae_vector* d2,
     /* Real    */ ae_vector* dd2,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_vector _x2;
    ae_vector a1;
    ae_vector a2;
    ae_vector a3;
    ae_vector b;
    ae_vector d;
    ae_vector dt;
    ae_vector p;
    ae_vector p2;
    ae_int_t i;
    ae_int_t ylen;
    double t;
    double t2;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    ae_vector_init_copy(&_x2, x2, _state);
    x2 = &_x2;
    ae_vector_clear(y2);
    ae_vector_clear(d2);
    ae_vector_clear(dd2);
    ae_vector_init(&a1, 0, DT_REAL, _state);
    ae_vector_init(&a2, 0, DT_REAL, _state);
    ae_vector_init(&a3, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&dt, 0, DT_REAL, _state);
    ae_vector_init(&p, 0, DT_INT, _state);
    ae_vector_init(&p2, 0, DT_INT, _state);

    
    /*
     * check correctness of boundary conditions
     */
    ae_assert(((boundltype==-1||boundltype==0)||boundltype==1)||boundltype==2, "Spline1DConvDiff2Cubic: incorrect BoundLType!", _state);
    ae_assert(((boundrtype==-1||boundrtype==0)||boundrtype==1)||boundrtype==2, "Spline1DConvDiff2Cubic: incorrect BoundRType!", _state);
    ae_assert((boundrtype==-1&&boundltype==-1)||(boundrtype!=-1&&boundltype!=-1), "Spline1DConvDiff2Cubic: incorrect BoundLType/BoundRType!", _state);
    if( boundltype==1||boundltype==2 )
    {
        ae_assert(ae_isfinite(boundl, _state), "Spline1DConvDiff2Cubic: BoundL is infinite or NAN!", _state);
    }
    if( boundrtype==1||boundrtype==2 )
    {
        ae_assert(ae_isfinite(boundr, _state), "Spline1DConvDiff2Cubic: BoundR is infinite or NAN!", _state);
    }
    
    /*
     * check lengths of arguments
     */
    ae_assert(n>=2, "Spline1DConvDiff2Cubic: N<2!", _state);
    ae_assert(x->cnt>=n, "Spline1DConvDiff2Cubic: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Spline1DConvDiff2Cubic: Length(Y)<N!", _state);
    ae_assert(n2>=2, "Spline1DConvDiff2Cubic: N2<2!", _state);
    ae_assert(x2->cnt>=n2, "Spline1DConvDiff2Cubic: Length(X2)<N2!", _state);
    
    /*
     * check and sort X/Y
     */
    ylen = n;
    if( boundltype==-1 )
    {
        ylen = n-1;
    }
    ae_assert(isfinitevector(x, n, _state), "Spline1DConvDiff2Cubic: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(y, ylen, _state), "Spline1DConvDiff2Cubic: Y contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(x2, n2, _state), "Spline1DConvDiff2Cubic: X2 contains infinite or NAN values!", _state);
    spline1d_heapsortppoints(x, y, &p, n, _state);
    ae_assert(aredistinct(x, n, _state), "Spline1DConvDiff2Cubic: at least two consequent points are too close!", _state);
    
    /*
     * set up DT (we will need it below)
     */
    ae_vector_set_length(&dt, ae_maxint(n, n2, _state), _state);
    
    /*
     * sort X2:
     * * use fake array DT because HeapSortPPoints() needs both integer AND real arrays
     * * if we have periodic problem, wrap points
     * * sort them, store permutation at P2
     */
    if( boundrtype==-1&&boundltype==-1 )
    {
        for(i=0; i<=n2-1; i++)
        {
            t = x2->ptr.p_double[i];
            apperiodicmap(&t, x->ptr.p_double[0], x->ptr.p_double[n-1], &t2, _state);
            x2->ptr.p_double[i] = t;
        }
    }
    spline1d_heapsortppoints(x2, &dt, &p2, n2, _state);
    
    /*
     * Now we've checked and preordered everything, so we:
     * * call internal GridDiff() function to get Hermite form of spline
     * * convert using internal Conv() function
     * * convert Y2 back to original order
     */
    spline1d_spline1dgriddiffcubicinternal(x, y, n, boundltype, boundl, boundrtype, boundr, &d, &a1, &a2, &a3, &b, &dt, _state);
    spline1dconvdiffinternal(x, y, &d, n, x2, n2, y2, ae_true, d2, ae_true, dd2, ae_true, _state);
    ae_assert(dt.cnt>=n2, "Spline1DConvDiff2Cubic: internal error!", _state);
    for(i=0; i<=n2-1; i++)
    {
        dt.ptr.p_double[p2.ptr.p_int[i]] = y2->ptr.p_double[i];
    }
    ae_v_move(&y2->ptr.p_double[0], 1, &dt.ptr.p_double[0], 1, ae_v_len(0,n2-1));
    for(i=0; i<=n2-1; i++)
    {
        dt.ptr.p_double[p2.ptr.p_int[i]] = d2->ptr.p_double[i];
    }
    ae_v_move(&d2->ptr.p_double[0], 1, &dt.ptr.p_double[0], 1, ae_v_len(0,n2-1));
    for(i=0; i<=n2-1; i++)
    {
        dt.ptr.p_double[p2.ptr.p_int[i]] = dd2->ptr.p_double[i];
    }
    ae_v_move(&dd2->ptr.p_double[0], 1, &dt.ptr.p_double[0], 1, ae_v_len(0,n2-1));
    ae_frame_leave(_state);
}


/*************************************************************************
This subroutine builds Catmull-Rom spline interpolant.

INPUT PARAMETERS:
    X           -   spline nodes, array[0..N-1].
    Y           -   function values, array[0..N-1].
    
OPTIONAL PARAMETERS:
    N           -   points count:
                    * N>=2
                    * if given, only first N points are used to build spline
                    * if not given, automatically detected from X/Y sizes
                      (len(X) must be equal to len(Y))
    BoundType   -   boundary condition type:
                    * -1 for periodic boundary condition
                    *  0 for parabolically terminated spline (default)
    Tension     -   tension parameter:
                    * tension=0   corresponds to classic Catmull-Rom spline (default)
                    * 0<tension<1 corresponds to more general form - cardinal spline

OUTPUT PARAMETERS:
    C           -   spline interpolant


ORDER OF POINTS

Subroutine automatically sorts points, so caller may pass unsorted array.

PROBLEMS WITH PERIODIC BOUNDARY CONDITIONS:

Problems with periodic boundary conditions have Y[first_point]=Y[last_point].
However, this subroutine doesn't require you to specify equal  values  for
the first and last points - it automatically forces them  to  be  equal by
copying  Y[first_point]  (corresponds  to the leftmost,  minimal  X[])  to
Y[last_point]. However it is recommended to pass consistent values of Y[],
i.e. to make Y[first_point]=Y[last_point].

  -- ALGLIB PROJECT --
     Copyright 23.06.2007 by Bochkanov Sergey
*************************************************************************/
void spline1dbuildcatmullrom(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t boundtype,
     double tension,
     spline1dinterpolant* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_vector d;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    _spline1dinterpolant_clear(c);
    ae_vector_init(&d, 0, DT_REAL, _state);

    ae_assert(n>=2, "Spline1DBuildCatmullRom: N<2!", _state);
    ae_assert(boundtype==-1||boundtype==0, "Spline1DBuildCatmullRom: incorrect BoundType!", _state);
    ae_assert(ae_fp_greater_eq(tension,(double)(0)), "Spline1DBuildCatmullRom: Tension<0!", _state);
    ae_assert(ae_fp_less_eq(tension,(double)(1)), "Spline1DBuildCatmullRom: Tension>1!", _state);
    ae_assert(x->cnt>=n, "Spline1DBuildCatmullRom: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Spline1DBuildCatmullRom: Length(Y)<N!", _state);
    
    /*
     * check and sort points
     */
    ae_assert(isfinitevector(x, n, _state), "Spline1DBuildCatmullRom: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(y, n, _state), "Spline1DBuildCatmullRom: Y contains infinite or NAN values!", _state);
    spline1d_heapsortpoints(x, y, n, _state);
    ae_assert(aredistinct(x, n, _state), "Spline1DBuildCatmullRom: at least two consequent points are too close!", _state);
    
    /*
     * Special cases:
     * * N=2, parabolic terminated boundary condition on both ends
     * * N=2, periodic boundary condition
     */
    if( n==2&&boundtype==0 )
    {
        
        /*
         * Just linear spline
         */
        spline1dbuildlinear(x, y, n, c, _state);
        ae_frame_leave(_state);
        return;
    }
    if( n==2&&boundtype==-1 )
    {
        
        /*
         * Same as cubic spline with periodic conditions
         */
        spline1dbuildcubic(x, y, n, -1, 0.0, -1, 0.0, c, _state);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Periodic or non-periodic boundary conditions
     */
    if( boundtype==-1 )
    {
        
        /*
         * Periodic boundary conditions
         */
        y->ptr.p_double[n-1] = y->ptr.p_double[0];
        ae_vector_set_length(&d, n, _state);
        d.ptr.p_double[0] = (y->ptr.p_double[1]-y->ptr.p_double[n-2])/(2*(x->ptr.p_double[1]-x->ptr.p_double[0]+x->ptr.p_double[n-1]-x->ptr.p_double[n-2]));
        for(i=1; i<=n-2; i++)
        {
            d.ptr.p_double[i] = (1-tension)*(y->ptr.p_double[i+1]-y->ptr.p_double[i-1])/(x->ptr.p_double[i+1]-x->ptr.p_double[i-1]);
        }
        d.ptr.p_double[n-1] = d.ptr.p_double[0];
        
        /*
         * Now problem is reduced to the cubic Hermite spline
         */
        spline1dbuildhermite(x, y, &d, n, c, _state);
        c->periodic = ae_true;
    }
    else
    {
        
        /*
         * Non-periodic boundary conditions
         */
        ae_vector_set_length(&d, n, _state);
        for(i=1; i<=n-2; i++)
        {
            d.ptr.p_double[i] = (1-tension)*(y->ptr.p_double[i+1]-y->ptr.p_double[i-1])/(x->ptr.p_double[i+1]-x->ptr.p_double[i-1]);
        }
        d.ptr.p_double[0] = 2*(y->ptr.p_double[1]-y->ptr.p_double[0])/(x->ptr.p_double[1]-x->ptr.p_double[0])-d.ptr.p_double[1];
        d.ptr.p_double[n-1] = 2*(y->ptr.p_double[n-1]-y->ptr.p_double[n-2])/(x->ptr.p_double[n-1]-x->ptr.p_double[n-2])-d.ptr.p_double[n-2];
        
        /*
         * Now problem is reduced to the cubic Hermite spline
         */
        spline1dbuildhermite(x, y, &d, n, c, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This subroutine builds Hermite spline interpolant.

INPUT PARAMETERS:
    X           -   spline nodes, array[0..N-1]
    Y           -   function values, array[0..N-1]
    D           -   derivatives, array[0..N-1]
    N           -   points count (optional):
                    * N>=2
                    * if given, only first N points are used to build spline
                    * if not given, automatically detected from X/Y sizes
                      (len(X) must be equal to len(Y))

OUTPUT PARAMETERS:
    C           -   spline interpolant.


ORDER OF POINTS

Subroutine automatically sorts points, so caller may pass unsorted array.

  -- ALGLIB PROJECT --
     Copyright 23.06.2007 by Bochkanov Sergey
*************************************************************************/
void spline1dbuildhermite(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* d,
     ae_int_t n,
     spline1dinterpolant* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_vector _d;
    ae_int_t i;
    double delta;
    double delta2;
    double delta3;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    ae_vector_init_copy(&_d, d, _state);
    d = &_d;
    _spline1dinterpolant_clear(c);

    ae_assert(n>=2, "Spline1DBuildHermite: N<2!", _state);
    ae_assert(x->cnt>=n, "Spline1DBuildHermite: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Spline1DBuildHermite: Length(Y)<N!", _state);
    ae_assert(d->cnt>=n, "Spline1DBuildHermite: Length(D)<N!", _state);
    
    /*
     * check and sort points
     */
    ae_assert(isfinitevector(x, n, _state), "Spline1DBuildHermite: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(y, n, _state), "Spline1DBuildHermite: Y contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(d, n, _state), "Spline1DBuildHermite: D contains infinite or NAN values!", _state);
    heapsortdpoints(x, y, d, n, _state);
    ae_assert(aredistinct(x, n, _state), "Spline1DBuildHermite: at least two consequent points are too close!", _state);
    
    /*
     * Build
     */
    ae_vector_set_length(&c->x, n, _state);
    ae_vector_set_length(&c->c, 4*(n-1)+2, _state);
    c->periodic = ae_false;
    c->k = 3;
    c->n = n;
    c->continuity = 1;
    for(i=0; i<=n-1; i++)
    {
        c->x.ptr.p_double[i] = x->ptr.p_double[i];
    }
    for(i=0; i<=n-2; i++)
    {
        delta = x->ptr.p_double[i+1]-x->ptr.p_double[i];
        delta2 = ae_sqr(delta, _state);
        delta3 = delta*delta2;
        c->c.ptr.p_double[4*i+0] = y->ptr.p_double[i];
        c->c.ptr.p_double[4*i+1] = d->ptr.p_double[i];
        c->c.ptr.p_double[4*i+2] = (3*(y->ptr.p_double[i+1]-y->ptr.p_double[i])-2*d->ptr.p_double[i]*delta-d->ptr.p_double[i+1]*delta)/delta2;
        c->c.ptr.p_double[4*i+3] = (2*(y->ptr.p_double[i]-y->ptr.p_double[i+1])+d->ptr.p_double[i]*delta+d->ptr.p_double[i+1]*delta)/delta3;
    }
    c->c.ptr.p_double[4*(n-1)+0] = y->ptr.p_double[n-1];
    c->c.ptr.p_double[4*(n-1)+1] = d->ptr.p_double[n-1];
    ae_frame_leave(_state);
}


/*************************************************************************
This subroutine builds Akima spline interpolant

INPUT PARAMETERS:
    X           -   spline nodes, array[0..N-1]
    Y           -   function values, array[0..N-1]
    N           -   points count (optional):
                    * N>=2
                    * if given, only first N points are used to build spline
                    * if not given, automatically detected from X/Y sizes
                      (len(X) must be equal to len(Y))

OUTPUT PARAMETERS:
    C           -   spline interpolant


ORDER OF POINTS

Subroutine automatically sorts points, so caller may pass unsorted array.

  -- ALGLIB PROJECT --
     Copyright 24.06.2007 by Bochkanov Sergey
*************************************************************************/
void spline1dbuildakima(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     spline1dinterpolant* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_int_t i;
    ae_vector d;
    ae_vector w;
    ae_vector diff;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    _spline1dinterpolant_clear(c);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&diff, 0, DT_REAL, _state);

    ae_assert(n>=2, "Spline1DBuildAkima: N<2!", _state);
    ae_assert(x->cnt>=n, "Spline1DBuildAkima: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Spline1DBuildAkima: Length(Y)<N!", _state);
    
    /*
     * check and sort points
     */
    ae_assert(isfinitevector(x, n, _state), "Spline1DBuildAkima: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(y, n, _state), "Spline1DBuildAkima: Y contains infinite or NAN values!", _state);
    spline1d_heapsortpoints(x, y, n, _state);
    ae_assert(aredistinct(x, n, _state), "Spline1DBuildAkima: at least two consequent points are too close!", _state);
    
    /*
     * Handle special cases: N=2, N=3, N=4
     */
    if( n<=4 )
    {
        spline1dbuildcubic(x, y, n, 0, 0.0, 0, 0.0, c, _state);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Prepare W (weights), Diff (divided differences)
     */
    ae_vector_set_length(&w, n-1, _state);
    ae_vector_set_length(&diff, n-1, _state);
    for(i=0; i<=n-2; i++)
    {
        diff.ptr.p_double[i] = (y->ptr.p_double[i+1]-y->ptr.p_double[i])/(x->ptr.p_double[i+1]-x->ptr.p_double[i]);
    }
    for(i=1; i<=n-2; i++)
    {
        w.ptr.p_double[i] = ae_fabs(diff.ptr.p_double[i]-diff.ptr.p_double[i-1], _state);
    }
    
    /*
     * Prepare Hermite interpolation scheme
     */
    ae_vector_set_length(&d, n, _state);
    for(i=2; i<=n-3; i++)
    {
        if( ae_fp_neq(ae_fabs(w.ptr.p_double[i-1], _state)+ae_fabs(w.ptr.p_double[i+1], _state),(double)(0)) )
        {
            d.ptr.p_double[i] = (w.ptr.p_double[i+1]*diff.ptr.p_double[i-1]+w.ptr.p_double[i-1]*diff.ptr.p_double[i])/(w.ptr.p_double[i+1]+w.ptr.p_double[i-1]);
        }
        else
        {
            d.ptr.p_double[i] = ((x->ptr.p_double[i+1]-x->ptr.p_double[i])*diff.ptr.p_double[i-1]+(x->ptr.p_double[i]-x->ptr.p_double[i-1])*diff.ptr.p_double[i])/(x->ptr.p_double[i+1]-x->ptr.p_double[i-1]);
        }
    }
    d.ptr.p_double[0] = spline1d_diffthreepoint(x->ptr.p_double[0], x->ptr.p_double[0], y->ptr.p_double[0], x->ptr.p_double[1], y->ptr.p_double[1], x->ptr.p_double[2], y->ptr.p_double[2], _state);
    d.ptr.p_double[1] = spline1d_diffthreepoint(x->ptr.p_double[1], x->ptr.p_double[0], y->ptr.p_double[0], x->ptr.p_double[1], y->ptr.p_double[1], x->ptr.p_double[2], y->ptr.p_double[2], _state);
    d.ptr.p_double[n-2] = spline1d_diffthreepoint(x->ptr.p_double[n-2], x->ptr.p_double[n-3], y->ptr.p_double[n-3], x->ptr.p_double[n-2], y->ptr.p_double[n-2], x->ptr.p_double[n-1], y->ptr.p_double[n-1], _state);
    d.ptr.p_double[n-1] = spline1d_diffthreepoint(x->ptr.p_double[n-1], x->ptr.p_double[n-3], y->ptr.p_double[n-3], x->ptr.p_double[n-2], y->ptr.p_double[n-2], x->ptr.p_double[n-1], y->ptr.p_double[n-1], _state);
    
    /*
     * Build Akima spline using Hermite interpolation scheme
     */
    spline1dbuildhermite(x, y, &d, n, c, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
This subroutine calculates the value of the spline at the given point X.

INPUT PARAMETERS:
    C   -   spline interpolant
    X   -   point

Result:
    S(x)

  -- ALGLIB PROJECT --
     Copyright 23.06.2007 by Bochkanov Sergey
*************************************************************************/
double spline1dcalc(spline1dinterpolant* c, double x, ae_state *_state)
{
    ae_int_t l;
    ae_int_t r;
    ae_int_t m;
    double t;
    double result;


    ae_assert(c->k==3, "Spline1DCalc: internal error", _state);
    ae_assert(!ae_isinf(x, _state), "Spline1DCalc: infinite X!", _state);
    
    /*
     * special case: NaN
     */
    if( ae_isnan(x, _state) )
    {
        result = _state->v_nan;
        return result;
    }
    
    /*
     * correct if periodic
     */
    if( c->periodic )
    {
        apperiodicmap(&x, c->x.ptr.p_double[0], c->x.ptr.p_double[c->n-1], &t, _state);
    }
    
    /*
     * Binary search in the [ x[0], ..., x[n-2] ] (x[n-1] is not included)
     */
    l = 0;
    r = c->n-2+1;
    while(l!=r-1)
    {
        m = (l+r)/2;
        if( c->x.ptr.p_double[m]>=x )
        {
            r = m;
        }
        else
        {
            l = m;
        }
    }
    
    /*
     * Interpolation
     */
    x = x-c->x.ptr.p_double[l];
    m = 4*l;
    result = c->c.ptr.p_double[m]+x*(c->c.ptr.p_double[m+1]+x*(c->c.ptr.p_double[m+2]+x*c->c.ptr.p_double[m+3]));
    return result;
}


/*************************************************************************
This subroutine differentiates the spline.

INPUT PARAMETERS:
    C   -   spline interpolant.
    X   -   point

Result:
    S   -   S(x)
    DS  -   S'(x)
    D2S -   S''(x)

  -- ALGLIB PROJECT --
     Copyright 24.06.2007 by Bochkanov Sergey
*************************************************************************/
void spline1ddiff(spline1dinterpolant* c,
     double x,
     double* s,
     double* ds,
     double* d2s,
     ae_state *_state)
{
    ae_int_t l;
    ae_int_t r;
    ae_int_t m;
    double t;

    *s = 0;
    *ds = 0;
    *d2s = 0;

    ae_assert(c->k==3, "Spline1DDiff: internal error", _state);
    ae_assert(!ae_isinf(x, _state), "Spline1DDiff: infinite X!", _state);
    
    /*
     * special case: NaN
     */
    if( ae_isnan(x, _state) )
    {
        *s = _state->v_nan;
        *ds = _state->v_nan;
        *d2s = _state->v_nan;
        return;
    }
    
    /*
     * correct if periodic
     */
    if( c->periodic )
    {
        apperiodicmap(&x, c->x.ptr.p_double[0], c->x.ptr.p_double[c->n-1], &t, _state);
    }
    
    /*
     * Binary search
     */
    l = 0;
    r = c->n-2+1;
    while(l!=r-1)
    {
        m = (l+r)/2;
        if( c->x.ptr.p_double[m]>=x )
        {
            r = m;
        }
        else
        {
            l = m;
        }
    }
    
    /*
     * Differentiation
     */
    x = x-c->x.ptr.p_double[l];
    m = 4*l;
    *s = c->c.ptr.p_double[m]+x*(c->c.ptr.p_double[m+1]+x*(c->c.ptr.p_double[m+2]+x*c->c.ptr.p_double[m+3]));
    *ds = c->c.ptr.p_double[m+1]+2*x*c->c.ptr.p_double[m+2]+3*ae_sqr(x, _state)*c->c.ptr.p_double[m+3];
    *d2s = 2*c->c.ptr.p_double[m+2]+6*x*c->c.ptr.p_double[m+3];
}


/*************************************************************************
This subroutine makes the copy of the spline.

INPUT PARAMETERS:
    C   -   spline interpolant.

Result:
    CC  -   spline copy

  -- ALGLIB PROJECT --
     Copyright 29.06.2007 by Bochkanov Sergey
*************************************************************************/
void spline1dcopy(spline1dinterpolant* c,
     spline1dinterpolant* cc,
     ae_state *_state)
{
    ae_int_t s;

    _spline1dinterpolant_clear(cc);

    cc->periodic = c->periodic;
    cc->n = c->n;
    cc->k = c->k;
    cc->continuity = c->continuity;
    ae_vector_set_length(&cc->x, cc->n, _state);
    ae_v_move(&cc->x.ptr.p_double[0], 1, &c->x.ptr.p_double[0], 1, ae_v_len(0,cc->n-1));
    s = c->c.cnt;
    ae_vector_set_length(&cc->c, s, _state);
    ae_v_move(&cc->c.ptr.p_double[0], 1, &c->c.ptr.p_double[0], 1, ae_v_len(0,s-1));
}


/*************************************************************************
This subroutine unpacks the spline into the coefficients table.

INPUT PARAMETERS:
    C   -   spline interpolant.
    X   -   point

OUTPUT PARAMETERS:
    Tbl -   coefficients table, unpacked format, array[0..N-2, 0..5].
            For I = 0...N-2:
                Tbl[I,0] = X[i]
                Tbl[I,1] = X[i+1]
                Tbl[I,2] = C0
                Tbl[I,3] = C1
                Tbl[I,4] = C2
                Tbl[I,5] = C3
            On [x[i], x[i+1]] spline is equals to:
                S(x) = C0 + C1*t + C2*t^2 + C3*t^3
                t = x-x[i]
                
NOTE:
    You  can rebuild spline with  Spline1DBuildHermite()  function,  which
    accepts as inputs function values and derivatives at nodes, which  are
    easy to calculate when you have coefficients.

  -- ALGLIB PROJECT --
     Copyright 29.06.2007 by Bochkanov Sergey
*************************************************************************/
void spline1dunpack(spline1dinterpolant* c,
     ae_int_t* n,
     /* Real    */ ae_matrix* tbl,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    *n = 0;
    ae_matrix_clear(tbl);

    ae_matrix_set_length(tbl, c->n-2+1, 2+c->k+1, _state);
    *n = c->n;
    
    /*
     * Fill
     */
    for(i=0; i<=*n-2; i++)
    {
        tbl->ptr.pp_double[i][0] = c->x.ptr.p_double[i];
        tbl->ptr.pp_double[i][1] = c->x.ptr.p_double[i+1];
        for(j=0; j<=c->k; j++)
        {
            tbl->ptr.pp_double[i][2+j] = c->c.ptr.p_double[(c->k+1)*i+j];
        }
    }
}


/*************************************************************************
This subroutine performs linear transformation of the spline argument.

INPUT PARAMETERS:
    C   -   spline interpolant.
    A, B-   transformation coefficients: x = A*t + B
Result:
    C   -   transformed spline

  -- ALGLIB PROJECT --
     Copyright 30.06.2007 by Bochkanov Sergey
*************************************************************************/
void spline1dlintransx(spline1dinterpolant* c,
     double a,
     double b,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t n;
    double v;
    double dv;
    double d2v;
    ae_vector x;
    ae_vector y;
    ae_vector d;
    ae_bool isperiodic;
    ae_int_t contval;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);

    ae_assert(c->k==3, "Spline1DLinTransX: internal error", _state);
    n = c->n;
    ae_vector_set_length(&x, n, _state);
    ae_vector_set_length(&y, n, _state);
    ae_vector_set_length(&d, n, _state);
    
    /*
     * Unpack, X, Y, dY/dX.
     * Scale and pack with Spline1DBuildHermite again.
     */
    if( ae_fp_eq(a,(double)(0)) )
    {
        
        /*
         * Special case: A=0
         */
        v = spline1dcalc(c, b, _state);
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = c->x.ptr.p_double[i];
            y.ptr.p_double[i] = v;
            d.ptr.p_double[i] = 0.0;
        }
    }
    else
    {
        
        /*
         * General case, A<>0
         */
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[i] = c->x.ptr.p_double[i];
            spline1ddiff(c, x.ptr.p_double[i], &v, &dv, &d2v, _state);
            x.ptr.p_double[i] = (x.ptr.p_double[i]-b)/a;
            y.ptr.p_double[i] = v;
            d.ptr.p_double[i] = a*dv;
        }
    }
    isperiodic = c->periodic;
    contval = c->continuity;
    if( contval>0 )
    {
        spline1dbuildhermite(&x, &y, &d, n, c, _state);
    }
    else
    {
        spline1dbuildlinear(&x, &y, n, c, _state);
    }
    c->periodic = isperiodic;
    c->continuity = contval;
    ae_frame_leave(_state);
}


/*************************************************************************
This subroutine performs linear transformation of the spline.

INPUT PARAMETERS:
    C   -   spline interpolant.
    A, B-   transformation coefficients: S2(x) = A*S(x) + B
Result:
    C   -   transformed spline

  -- ALGLIB PROJECT --
     Copyright 30.06.2007 by Bochkanov Sergey
*************************************************************************/
void spline1dlintransy(spline1dinterpolant* c,
     double a,
     double b,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t n;


    ae_assert(c->k==3, "Spline1DLinTransX: internal error", _state);
    n = c->n;
    for(i=0; i<=n-2; i++)
    {
        c->c.ptr.p_double[4*i] = a*c->c.ptr.p_double[4*i]+b;
        for(j=1; j<=3; j++)
        {
            c->c.ptr.p_double[4*i+j] = a*c->c.ptr.p_double[4*i+j];
        }
    }
    c->c.ptr.p_double[4*(n-1)+0] = a*c->c.ptr.p_double[4*(n-1)+0]+b;
    c->c.ptr.p_double[4*(n-1)+1] = a*c->c.ptr.p_double[4*(n-1)+1];
}


/*************************************************************************
This subroutine integrates the spline.

INPUT PARAMETERS:
    C   -   spline interpolant.
    X   -   right bound of the integration interval [a, x],
            here 'a' denotes min(x[])
Result:
    integral(S(t)dt,a,x)

  -- ALGLIB PROJECT --
     Copyright 23.06.2007 by Bochkanov Sergey
*************************************************************************/
double spline1dintegrate(spline1dinterpolant* c,
     double x,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_int_t l;
    ae_int_t r;
    ae_int_t m;
    double w;
    double v;
    double t;
    double intab;
    double additionalterm;
    double result;


    n = c->n;
    
    /*
     * Periodic splines require special treatment. We make
     * following transformation:
     *
     *     integral(S(t)dt,A,X) = integral(S(t)dt,A,Z)+AdditionalTerm
     *
     * here X may lie outside of [A,B], Z lies strictly in [A,B],
     * AdditionalTerm is equals to integral(S(t)dt,A,B) times some
     * integer number (may be zero).
     */
    if( c->periodic&&(ae_fp_less(x,c->x.ptr.p_double[0])||ae_fp_greater(x,c->x.ptr.p_double[c->n-1])) )
    {
        
        /*
         * compute integral(S(x)dx,A,B)
         */
        intab = (double)(0);
        for(i=0; i<=c->n-2; i++)
        {
            w = c->x.ptr.p_double[i+1]-c->x.ptr.p_double[i];
            m = (c->k+1)*i;
            intab = intab+c->c.ptr.p_double[m]*w;
            v = w;
            for(j=1; j<=c->k; j++)
            {
                v = v*w;
                intab = intab+c->c.ptr.p_double[m+j]*v/(j+1);
            }
        }
        
        /*
         * map X into [A,B]
         */
        apperiodicmap(&x, c->x.ptr.p_double[0], c->x.ptr.p_double[c->n-1], &t, _state);
        additionalterm = t*intab;
    }
    else
    {
        additionalterm = (double)(0);
    }
    
    /*
     * Binary search in the [ x[0], ..., x[n-2] ] (x[n-1] is not included)
     */
    l = 0;
    r = n-2+1;
    while(l!=r-1)
    {
        m = (l+r)/2;
        if( ae_fp_greater_eq(c->x.ptr.p_double[m],x) )
        {
            r = m;
        }
        else
        {
            l = m;
        }
    }
    
    /*
     * Integration
     */
    result = (double)(0);
    for(i=0; i<=l-1; i++)
    {
        w = c->x.ptr.p_double[i+1]-c->x.ptr.p_double[i];
        m = (c->k+1)*i;
        result = result+c->c.ptr.p_double[m]*w;
        v = w;
        for(j=1; j<=c->k; j++)
        {
            v = v*w;
            result = result+c->c.ptr.p_double[m+j]*v/(j+1);
        }
    }
    w = x-c->x.ptr.p_double[l];
    m = (c->k+1)*l;
    v = w;
    result = result+c->c.ptr.p_double[m]*w;
    for(j=1; j<=c->k; j++)
    {
        v = v*w;
        result = result+c->c.ptr.p_double[m+j]*v/(j+1);
    }
    result = result+additionalterm;
    return result;
}


/*************************************************************************
Internal version of Spline1DConvDiff

Converts from Hermite spline given by grid XOld to new grid X2

INPUT PARAMETERS:
    XOld    -   old grid
    YOld    -   values at old grid
    DOld    -   first derivative at old grid
    N       -   grid size
    X2      -   new grid
    N2      -   new grid size
    Y       -   possibly preallocated output array
                (reallocate if too small)
    NeedY   -   do we need Y?
    D1      -   possibly preallocated output array
                (reallocate if too small)
    NeedD1  -   do we need D1?
    D2      -   possibly preallocated output array
                (reallocate if too small)
    NeedD2  -   do we need D1?

OUTPUT ARRAYS:
    Y       -   values, if needed
    D1      -   first derivative, if needed
    D2      -   second derivative, if needed

  -- ALGLIB PROJECT --
     Copyright 03.09.2010 by Bochkanov Sergey
*************************************************************************/
void spline1dconvdiffinternal(/* Real    */ ae_vector* xold,
     /* Real    */ ae_vector* yold,
     /* Real    */ ae_vector* dold,
     ae_int_t n,
     /* Real    */ ae_vector* x2,
     ae_int_t n2,
     /* Real    */ ae_vector* y,
     ae_bool needy,
     /* Real    */ ae_vector* d1,
     ae_bool needd1,
     /* Real    */ ae_vector* d2,
     ae_bool needd2,
     ae_state *_state)
{
    ae_int_t intervalindex;
    ae_int_t pointindex;
    ae_bool havetoadvance;
    double c0;
    double c1;
    double c2;
    double c3;
    double a;
    double b;
    double w;
    double w2;
    double w3;
    double fa;
    double fb;
    double da;
    double db;
    double t;


    
    /*
     * Prepare space
     */
    if( needy&&y->cnt<n2 )
    {
        ae_vector_set_length(y, n2, _state);
    }
    if( needd1&&d1->cnt<n2 )
    {
        ae_vector_set_length(d1, n2, _state);
    }
    if( needd2&&d2->cnt<n2 )
    {
        ae_vector_set_length(d2, n2, _state);
    }
    
    /*
     * These assignments aren't actually needed
     * (variables are initialized in the loop below),
     * but without them compiler will complain about uninitialized locals
     */
    c0 = (double)(0);
    c1 = (double)(0);
    c2 = (double)(0);
    c3 = (double)(0);
    a = (double)(0);
    b = (double)(0);
    
    /*
     * Cycle
     */
    intervalindex = -1;
    pointindex = 0;
    for(;;)
    {
        
        /*
         * are we ready to exit?
         */
        if( pointindex>=n2 )
        {
            break;
        }
        t = x2->ptr.p_double[pointindex];
        
        /*
         * do we need to advance interval?
         */
        havetoadvance = ae_false;
        if( intervalindex==-1 )
        {
            havetoadvance = ae_true;
        }
        else
        {
            if( intervalindex<n-2 )
            {
                havetoadvance = ae_fp_greater_eq(t,b);
            }
        }
        if( havetoadvance )
        {
            intervalindex = intervalindex+1;
            a = xold->ptr.p_double[intervalindex];
            b = xold->ptr.p_double[intervalindex+1];
            w = b-a;
            w2 = w*w;
            w3 = w*w2;
            fa = yold->ptr.p_double[intervalindex];
            fb = yold->ptr.p_double[intervalindex+1];
            da = dold->ptr.p_double[intervalindex];
            db = dold->ptr.p_double[intervalindex+1];
            c0 = fa;
            c1 = da;
            c2 = (3*(fb-fa)-2*da*w-db*w)/w2;
            c3 = (2*(fa-fb)+da*w+db*w)/w3;
            continue;
        }
        
        /*
         * Calculate spline and its derivatives using power basis
         */
        t = t-a;
        if( needy )
        {
            y->ptr.p_double[pointindex] = c0+t*(c1+t*(c2+t*c3));
        }
        if( needd1 )
        {
            d1->ptr.p_double[pointindex] = c1+2*t*c2+3*t*t*c3;
        }
        if( needd2 )
        {
            d2->ptr.p_double[pointindex] = 2*c2+6*t*c3;
        }
        pointindex = pointindex+1;
    }
}


/*************************************************************************
This function finds all roots and extrema of the spline S(x)  defined  at
[A,B] (interval which contains spline nodes).

It  does not extrapolates function, so roots and extrema located  outside 
of [A,B] will not be found. It returns all isolated (including  multiple)
roots and extrema.

INPUT PARAMETERS
    C           -   spline interpolant
    
OUTPUT PARAMETERS
    R           -   array[NR], contains roots of the spline. 
                    In case there is no roots, this array has zero length.
    NR          -   number of roots, >=0
    DR          -   is set to True in case there is at least one interval
                    where spline is just a zero constant. Such degenerate
                    cases are not reported in the R/NR
    E           -   array[NE], contains  extrema  (maximums/minimums)  of 
                    the spline. In case there is no extrema,  this  array 
                    has zero length.
    ET          -   array[NE], extrema types:
                    * ET[i]>0 in case I-th extrema is a minimum
                    * ET[i]<0 in case I-th extrema is a maximum
    NE          -   number of extrema, >=0
    DE          -   is set to True in case there is at least one interval
                    where spline is a constant. Such degenerate cases are  
                    not reported in the E/NE.
                    
NOTES:

1. This function does NOT report following kinds of roots:
   * intervals where function is constantly zero
   * roots which are outside of [A,B] (note: it CAN return A or B)

2. This function does NOT report following kinds of extrema:
   * intervals where function is a constant
   * extrema which are outside of (A,B) (note: it WON'T return A or B)
   
 -- ALGLIB PROJECT --
     Copyright 26.09.2011 by Bochkanov Sergey   
*************************************************************************/
void spline1drootsandextrema(spline1dinterpolant* c,
     /* Real    */ ae_vector* r,
     ae_int_t* nr,
     ae_bool* dr,
     /* Real    */ ae_vector* e,
     /* Integer */ ae_vector* et,
     ae_int_t* ne,
     ae_bool* de,
     ae_state *_state)
{
    ae_frame _frame_block;
    double pl;
    double ml;
    double pll;
    double pr;
    double mr;
    ae_vector tr;
    ae_vector tmpr;
    ae_vector tmpe;
    ae_vector tmpet;
    ae_vector tmpc;
    double x0;
    double x1;
    double x2;
    double ex0;
    double ex1;
    ae_int_t tne;
    ae_int_t tnr;
    ae_int_t i;
    ae_int_t j;
    ae_bool nstep;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(r);
    *nr = 0;
    *dr = ae_false;
    ae_vector_clear(e);
    ae_vector_clear(et);
    *ne = 0;
    *de = ae_false;
    ae_vector_init(&tr, 0, DT_REAL, _state);
    ae_vector_init(&tmpr, 0, DT_REAL, _state);
    ae_vector_init(&tmpe, 0, DT_REAL, _state);
    ae_vector_init(&tmpet, 0, DT_INT, _state);
    ae_vector_init(&tmpc, 0, DT_REAL, _state);

    
    /*
     *exception handling
     */
    ae_assert(c->k==3, "Spline1DRootsAndExtrema : incorrect parameter C.K!", _state);
    ae_assert(c->continuity>=0, "Spline1DRootsAndExtrema : parameter C.Continuity must not be less than 0!", _state);
    
    /*
     *initialization of variable
     */
    *nr = 0;
    *ne = 0;
    *dr = ae_false;
    *de = ae_false;
    nstep = ae_true;
    
    /*
     *consider case, when C.Continuty=0
     */
    if( c->continuity==0 )
    {
        
        /*
         *allocation for auxiliary arrays 
         *'TmpR ' - it stores a time value for roots
         *'TmpE ' - it stores a time value for extremums
         *'TmpET '- it stores a time value for extremums type
         */
        rvectorsetlengthatleast(&tmpr, 3*(c->n-1), _state);
        rvectorsetlengthatleast(&tmpe, 2*(c->n-1), _state);
        ivectorsetlengthatleast(&tmpet, 2*(c->n-1), _state);
        
        /*
         *start calculating
         */
        for(i=0; i<=c->n-2; i++)
        {
            
            /*
             *initialization pL, mL, pR, mR
             */
            pl = c->c.ptr.p_double[4*i];
            ml = c->c.ptr.p_double[4*i+1];
            pr = c->c.ptr.p_double[4*(i+1)];
            mr = c->c.ptr.p_double[4*i+1]+2*c->c.ptr.p_double[4*i+2]*(c->x.ptr.p_double[i+1]-c->x.ptr.p_double[i])+3*c->c.ptr.p_double[4*i+3]*(c->x.ptr.p_double[i+1]-c->x.ptr.p_double[i])*(c->x.ptr.p_double[i+1]-c->x.ptr.p_double[i]);
            
            /*
             *pre-searching roots and extremums
             */
            solvecubicpolinom(pl, ml, pr, mr, c->x.ptr.p_double[i], c->x.ptr.p_double[i+1], &x0, &x1, &x2, &ex0, &ex1, &tnr, &tne, &tr, _state);
            *dr = *dr||tnr==-1;
            *de = *de||tne==-1;
            
            /*
             *searching of roots
             */
            if( tnr==1&&nstep )
            {
                
                /*
                 *is there roots?
                 */
                if( *nr>0 )
                {
                    
                    /*
                     *is a next root equal a previous root?
                     *if is't, then write new root
                     */
                    if( ae_fp_neq(x0,tmpr.ptr.p_double[*nr-1]) )
                    {
                        tmpr.ptr.p_double[*nr] = x0;
                        *nr = *nr+1;
                    }
                }
                else
                {
                    
                    /*
                     *write a first root
                     */
                    tmpr.ptr.p_double[*nr] = x0;
                    *nr = *nr+1;
                }
            }
            else
            {
                
                /*
                 *case when function at a segment identically to zero
                 *then we have to clear a root, if the one located on a 
                 *constant segment
                 */
                if( tnr==-1 )
                {
                    
                    /*
                     *safe state variable as constant
                     */
                    if( nstep )
                    {
                        nstep = ae_false;
                    }
                    
                    /*
                     *clear the root, if there is
                     */
                    if( *nr>0 )
                    {
                        if( ae_fp_eq(c->x.ptr.p_double[i],tmpr.ptr.p_double[*nr-1]) )
                        {
                            *nr = *nr-1;
                        }
                    }
                    
                    /*
                     *change state for 'DR'
                     */
                    if( !*dr )
                    {
                        *dr = ae_true;
                    }
                }
                else
                {
                    nstep = ae_true;
                }
            }
            
            /*
             *searching of extremums
             */
            if( i>0 )
            {
                pll = c->c.ptr.p_double[4*(i-1)];
                
                /*
                 *if pL=pLL or pL=pR then
                 */
                if( tne==-1 )
                {
                    if( !*de )
                    {
                        *de = ae_true;
                    }
                }
                else
                {
                    if( ae_fp_greater(pl,pll)&&ae_fp_greater(pl,pr) )
                    {
                        
                        /*
                         *maximum
                         */
                        tmpet.ptr.p_int[*ne] = -1;
                        tmpe.ptr.p_double[*ne] = c->x.ptr.p_double[i];
                        *ne = *ne+1;
                    }
                    else
                    {
                        if( ae_fp_less(pl,pll)&&ae_fp_less(pl,pr) )
                        {
                            
                            /*
                             *minimum
                             */
                            tmpet.ptr.p_int[*ne] = 1;
                            tmpe.ptr.p_double[*ne] = c->x.ptr.p_double[i];
                            *ne = *ne+1;
                        }
                    }
                }
            }
        }
        
        /*
         *write final result
         */
        rvectorsetlengthatleast(r, *nr, _state);
        rvectorsetlengthatleast(e, *ne, _state);
        ivectorsetlengthatleast(et, *ne, _state);
        
        /*
         *write roots
         */
        for(i=0; i<=*nr-1; i++)
        {
            r->ptr.p_double[i] = tmpr.ptr.p_double[i];
        }
        
        /*
         *write extremums and their types
         */
        for(i=0; i<=*ne-1; i++)
        {
            e->ptr.p_double[i] = tmpe.ptr.p_double[i];
            et->ptr.p_int[i] = tmpet.ptr.p_int[i];
        }
    }
    else
    {
        
        /*
         *case, when C.Continuity>=1 
         *'TmpR ' - it stores a time value for roots
         *'TmpC' - it stores a time value for extremums and 
         *their function value (TmpC={EX0,F(EX0), EX1,F(EX1), ..., EXn,F(EXn)};)
         *'TmpE' - it stores a time value for extremums only
         *'TmpET'- it stores a time value for extremums type
         */
        rvectorsetlengthatleast(&tmpr, 2*c->n-1, _state);
        rvectorsetlengthatleast(&tmpc, 4*c->n, _state);
        rvectorsetlengthatleast(&tmpe, 2*c->n, _state);
        ivectorsetlengthatleast(&tmpet, 2*c->n, _state);
        
        /*
         *start calculating
         */
        for(i=0; i<=c->n-2; i++)
        {
            
            /*
             *we calculate pL,mL, pR,mR as Fi+1(F'i+1) at left border
             */
            pl = c->c.ptr.p_double[4*i];
            ml = c->c.ptr.p_double[4*i+1];
            pr = c->c.ptr.p_double[4*(i+1)];
            mr = c->c.ptr.p_double[4*(i+1)+1];
            
            /*
             *calculating roots and extremums at [X[i],X[i+1]]
             */
            solvecubicpolinom(pl, ml, pr, mr, c->x.ptr.p_double[i], c->x.ptr.p_double[i+1], &x0, &x1, &x2, &ex0, &ex1, &tnr, &tne, &tr, _state);
            
            /*
             *searching roots
             */
            if( tnr>0 )
            {
                
                /*
                 *re-init tR
                 */
                if( tnr>=1 )
                {
                    tr.ptr.p_double[0] = x0;
                }
                if( tnr>=2 )
                {
                    tr.ptr.p_double[1] = x1;
                }
                if( tnr==3 )
                {
                    tr.ptr.p_double[2] = x2;
                }
                
                /*
                 *start root selection
                 */
                if( *nr>0 )
                {
                    if( ae_fp_neq(tmpr.ptr.p_double[*nr-1],x0) )
                    {
                        
                        /*
                         *previous segment was't constant identical zero
                         */
                        if( nstep )
                        {
                            for(j=0; j<=tnr-1; j++)
                            {
                                tmpr.ptr.p_double[*nr+j] = tr.ptr.p_double[j];
                            }
                            *nr = *nr+tnr;
                        }
                        else
                        {
                            
                            /*
                             *previous segment was constant identical zero
                             *and we must ignore [NR+j-1] root
                             */
                            for(j=1; j<=tnr-1; j++)
                            {
                                tmpr.ptr.p_double[*nr+j-1] = tr.ptr.p_double[j];
                            }
                            *nr = *nr+tnr-1;
                            nstep = ae_true;
                        }
                    }
                    else
                    {
                        for(j=1; j<=tnr-1; j++)
                        {
                            tmpr.ptr.p_double[*nr+j-1] = tr.ptr.p_double[j];
                        }
                        *nr = *nr+tnr-1;
                    }
                }
                else
                {
                    
                    /*
                     *write first root
                     */
                    for(j=0; j<=tnr-1; j++)
                    {
                        tmpr.ptr.p_double[*nr+j] = tr.ptr.p_double[j];
                    }
                    *nr = *nr+tnr;
                }
            }
            else
            {
                if( tnr==-1 )
                {
                    
                    /*
                     *decrement 'NR' if at previous step was writen a root
                     *(previous segment identical zero)
                     */
                    if( *nr>0&&nstep )
                    {
                        *nr = *nr-1;
                    }
                    
                    /*
                     *previous segment is't constant
                     */
                    if( nstep )
                    {
                        nstep = ae_false;
                    }
                    
                    /*
                     *rewrite 'DR'
                     */
                    if( !*dr )
                    {
                        *dr = ae_true;
                    }
                }
            }
            
            /*
             *searching extremums
             *write all term like extremums
             */
            if( tne==1 )
            {
                if( *ne>0 )
                {
                    
                    /*
                     *just ignore identical extremums
                     *because he must be one
                     */
                    if( ae_fp_neq(tmpc.ptr.p_double[*ne-2],ex0) )
                    {
                        tmpc.ptr.p_double[*ne] = ex0;
                        tmpc.ptr.p_double[*ne+1] = c->c.ptr.p_double[4*i]+c->c.ptr.p_double[4*i+1]*(ex0-c->x.ptr.p_double[i])+c->c.ptr.p_double[4*i+2]*(ex0-c->x.ptr.p_double[i])*(ex0-c->x.ptr.p_double[i])+c->c.ptr.p_double[4*i+3]*(ex0-c->x.ptr.p_double[i])*(ex0-c->x.ptr.p_double[i])*(ex0-c->x.ptr.p_double[i]);
                        *ne = *ne+2;
                    }
                }
                else
                {
                    
                    /*
                     *write first extremum and it function value
                     */
                    tmpc.ptr.p_double[*ne] = ex0;
                    tmpc.ptr.p_double[*ne+1] = c->c.ptr.p_double[4*i]+c->c.ptr.p_double[4*i+1]*(ex0-c->x.ptr.p_double[i])+c->c.ptr.p_double[4*i+2]*(ex0-c->x.ptr.p_double[i])*(ex0-c->x.ptr.p_double[i])+c->c.ptr.p_double[4*i+3]*(ex0-c->x.ptr.p_double[i])*(ex0-c->x.ptr.p_double[i])*(ex0-c->x.ptr.p_double[i]);
                    *ne = *ne+2;
                }
            }
            else
            {
                if( tne==2 )
                {
                    if( *ne>0 )
                    {
                        
                        /*
                         *ignore identical extremum
                         */
                        if( ae_fp_neq(tmpc.ptr.p_double[*ne-2],ex0) )
                        {
                            tmpc.ptr.p_double[*ne] = ex0;
                            tmpc.ptr.p_double[*ne+1] = c->c.ptr.p_double[4*i]+c->c.ptr.p_double[4*i+1]*(ex0-c->x.ptr.p_double[i])+c->c.ptr.p_double[4*i+2]*(ex0-c->x.ptr.p_double[i])*(ex0-c->x.ptr.p_double[i])+c->c.ptr.p_double[4*i+3]*(ex0-c->x.ptr.p_double[i])*(ex0-c->x.ptr.p_double[i])*(ex0-c->x.ptr.p_double[i]);
                            *ne = *ne+2;
                        }
                    }
                    else
                    {
                        
                        /*
                         *write first extremum
                         */
                        tmpc.ptr.p_double[*ne] = ex0;
                        tmpc.ptr.p_double[*ne+1] = c->c.ptr.p_double[4*i]+c->c.ptr.p_double[4*i+1]*(ex0-c->x.ptr.p_double[i])+c->c.ptr.p_double[4*i+2]*(ex0-c->x.ptr.p_double[i])*(ex0-c->x.ptr.p_double[i])+c->c.ptr.p_double[4*i+3]*(ex0-c->x.ptr.p_double[i])*(ex0-c->x.ptr.p_double[i])*(ex0-c->x.ptr.p_double[i]);
                        *ne = *ne+2;
                    }
                    
                    /*
                     *write second extremum
                     */
                    tmpc.ptr.p_double[*ne] = ex1;
                    tmpc.ptr.p_double[*ne+1] = c->c.ptr.p_double[4*i]+c->c.ptr.p_double[4*i+1]*(ex1-c->x.ptr.p_double[i])+c->c.ptr.p_double[4*i+2]*(ex1-c->x.ptr.p_double[i])*(ex1-c->x.ptr.p_double[i])+c->c.ptr.p_double[4*i+3]*(ex1-c->x.ptr.p_double[i])*(ex1-c->x.ptr.p_double[i])*(ex1-c->x.ptr.p_double[i]);
                    *ne = *ne+2;
                }
                else
                {
                    if( tne==-1 )
                    {
                        if( !*de )
                        {
                            *de = ae_true;
                        }
                    }
                }
            }
        }
        
        /*
         *checking of arrays
         *get number of extremums (tNe=NE/2)
         *initialize pL as value F0(X[0]) and
         *initialize pR as value Fn-1(X[N])
         */
        tne = *ne/2;
        *ne = 0;
        pl = c->c.ptr.p_double[0];
        pr = c->c.ptr.p_double[4*(c->n-1)];
        for(i=0; i<=tne-1; i++)
        {
            if( i>0&&i<tne-1 )
            {
                if( ae_fp_greater(tmpc.ptr.p_double[2*i+1],tmpc.ptr.p_double[2*(i-1)+1])&&ae_fp_greater(tmpc.ptr.p_double[2*i+1],tmpc.ptr.p_double[2*(i+1)+1]) )
                {
                    
                    /*
                     *maximum
                     */
                    tmpe.ptr.p_double[*ne] = tmpc.ptr.p_double[2*i];
                    tmpet.ptr.p_int[*ne] = -1;
                    *ne = *ne+1;
                }
                else
                {
                    if( ae_fp_less(tmpc.ptr.p_double[2*i+1],tmpc.ptr.p_double[2*(i-1)+1])&&ae_fp_less(tmpc.ptr.p_double[2*i+1],tmpc.ptr.p_double[2*(i+1)+1]) )
                    {
                        
                        /*
                         *minimum
                         */
                        tmpe.ptr.p_double[*ne] = tmpc.ptr.p_double[2*i];
                        tmpet.ptr.p_int[*ne] = 1;
                        *ne = *ne+1;
                    }
                }
            }
            else
            {
                if( i==0 )
                {
                    if( ae_fp_neq(tmpc.ptr.p_double[2*i],c->x.ptr.p_double[0]) )
                    {
                        if( ae_fp_greater(tmpc.ptr.p_double[2*i+1],pl)&&ae_fp_greater(tmpc.ptr.p_double[2*i+1],tmpc.ptr.p_double[2*(i+1)+1]) )
                        {
                            
                            /*
                             *maximum
                             */
                            tmpe.ptr.p_double[*ne] = tmpc.ptr.p_double[2*i];
                            tmpet.ptr.p_int[*ne] = -1;
                            *ne = *ne+1;
                        }
                        else
                        {
                            if( ae_fp_less(tmpc.ptr.p_double[2*i+1],pl)&&ae_fp_less(tmpc.ptr.p_double[2*i+1],tmpc.ptr.p_double[2*(i+1)+1]) )
                            {
                                
                                /*
                                 *minimum
                                 */
                                tmpe.ptr.p_double[*ne] = tmpc.ptr.p_double[2*i];
                                tmpet.ptr.p_int[*ne] = 1;
                                *ne = *ne+1;
                            }
                        }
                    }
                }
                else
                {
                    if( i==tne-1 )
                    {
                        if( ae_fp_neq(tmpc.ptr.p_double[2*i],c->x.ptr.p_double[c->n-1]) )
                        {
                            if( ae_fp_greater(tmpc.ptr.p_double[2*i+1],tmpc.ptr.p_double[2*(i-1)+1])&&ae_fp_greater(tmpc.ptr.p_double[2*i+1],pr) )
                            {
                                
                                /*
                                 *maximum
                                 */
                                tmpe.ptr.p_double[*ne] = tmpc.ptr.p_double[2*i];
                                tmpet.ptr.p_int[*ne] = -1;
                                *ne = *ne+1;
                            }
                            else
                            {
                                if( ae_fp_less(tmpc.ptr.p_double[2*i+1],tmpc.ptr.p_double[2*(i-1)+1])&&ae_fp_less(tmpc.ptr.p_double[2*i+1],pr) )
                                {
                                    
                                    /*
                                     *minimum
                                     */
                                    tmpe.ptr.p_double[*ne] = tmpc.ptr.p_double[2*i];
                                    tmpet.ptr.p_int[*ne] = 1;
                                    *ne = *ne+1;
                                }
                            }
                        }
                    }
                }
            }
        }
        
        /*
         *final results
         *allocate R, E, ET
         */
        rvectorsetlengthatleast(r, *nr, _state);
        rvectorsetlengthatleast(e, *ne, _state);
        ivectorsetlengthatleast(et, *ne, _state);
        
        /*
         *write result for extremus and their types
         */
        for(i=0; i<=*ne-1; i++)
        {
            e->ptr.p_double[i] = tmpe.ptr.p_double[i];
            et->ptr.p_int[i] = tmpet.ptr.p_int[i];
        }
        
        /*
         *write result for roots
         */
        for(i=0; i<=*nr-1; i++)
        {
            r->ptr.p_double[i] = tmpr.ptr.p_double[i];
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine. Heap sort.
*************************************************************************/
void heapsortdpoints(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* d,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector rbuf;
    ae_vector ibuf;
    ae_vector rbuf2;
    ae_vector ibuf2;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&rbuf, 0, DT_REAL, _state);
    ae_vector_init(&ibuf, 0, DT_INT, _state);
    ae_vector_init(&rbuf2, 0, DT_REAL, _state);
    ae_vector_init(&ibuf2, 0, DT_INT, _state);

    ae_vector_set_length(&ibuf, n, _state);
    ae_vector_set_length(&rbuf, n, _state);
    for(i=0; i<=n-1; i++)
    {
        ibuf.ptr.p_int[i] = i;
    }
    tagsortfasti(x, &ibuf, &rbuf2, &ibuf2, n, _state);
    for(i=0; i<=n-1; i++)
    {
        rbuf.ptr.p_double[i] = y->ptr.p_double[ibuf.ptr.p_int[i]];
    }
    ae_v_move(&y->ptr.p_double[0], 1, &rbuf.ptr.p_double[0], 1, ae_v_len(0,n-1));
    for(i=0; i<=n-1; i++)
    {
        rbuf.ptr.p_double[i] = d->ptr.p_double[ibuf.ptr.p_int[i]];
    }
    ae_v_move(&d->ptr.p_double[0], 1, &rbuf.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_frame_leave(_state);
}


/*************************************************************************
This procedure search roots of an quadratic equation inside [0;1] and it number of roots.

INPUT PARAMETERS:
    P0   -   value of a function at 0
    M0   -   value of a derivative at 0
    P1   -   value of a function at 1
    M1   -   value of a derivative at 1

OUTPUT PARAMETERS:
    X0   -  first root of an equation
    X1   -  second root of an equation
    NR   -  number of roots
    
RESTRICTIONS OF PARAMETERS:

Parameters for this procedure has't to be zero simultaneously. Is expected, 
that input polinom is't degenerate or constant identicaly ZERO.


REMARK:

The procedure always fill value for X1 and X2, even if it is't belongs to [0;1].
But first true root(even if existing one) is in X1.
Number of roots is NR.

 -- ALGLIB PROJECT --
     Copyright 26.09.2011 by Bochkanov Sergey
*************************************************************************/
void solvepolinom2(double p0,
     double m0,
     double p1,
     double m1,
     double* x0,
     double* x1,
     ae_int_t* nr,
     ae_state *_state)
{
    double a;
    double b;
    double c;
    double dd;
    double tmp;
    double exf;
    double extr;

    *x0 = 0;
    *x1 = 0;
    *nr = 0;

    
    /*
     *calculate parameters for equation: A, B  and C
     */
    a = 6*p0+3*m0-6*p1+3*m1;
    b = -6*p0-4*m0+6*p1-2*m1;
    c = m0;
    
    /*
     *check case, when A=0
     *we are considering the linear equation
     */
    if( ae_fp_eq(a,(double)(0)) )
    {
        
        /*
         *B<>0 and root inside [0;1]
         *one root
         */
        if( (ae_fp_neq(b,(double)(0))&&ae_sign(c, _state)*ae_sign(b, _state)<=0)&&ae_fp_greater_eq(ae_fabs(b, _state),ae_fabs(c, _state)) )
        {
            *x0 = -c/b;
            *nr = 1;
            return;
        }
        else
        {
            *nr = 0;
            return;
        }
    }
    
    /*
     *consider case, when extremumu outside (0;1)
     *exist one root only
     */
    if( ae_fp_less_eq(ae_fabs(2*a, _state),ae_fabs(b, _state))||ae_sign(b, _state)*ae_sign(a, _state)>=0 )
    {
        if( ae_sign(m0, _state)*ae_sign(m1, _state)>0 )
        {
            *nr = 0;
            return;
        }
        
        /*
         *consider case, when the one exist
         *same sign of derivative
         */
        if( ae_sign(m0, _state)*ae_sign(m1, _state)<0 )
        {
            *nr = 1;
            extr = -b/(2*a);
            dd = b*b-4*a*c;
            if( ae_fp_less(dd,(double)(0)) )
            {
                return;
            }
            *x0 = (-b-ae_sqrt(dd, _state))/(2*a);
            *x1 = (-b+ae_sqrt(dd, _state))/(2*a);
            if( (ae_fp_greater_eq(extr,(double)(1))&&ae_fp_less_eq(*x1,extr))||(ae_fp_less_eq(extr,(double)(0))&&ae_fp_greater_eq(*x1,extr)) )
            {
                *x0 = *x1;
            }
            return;
        }
        
        /*
         *consider case, when the one is 0
         */
        if( ae_fp_eq(m0,(double)(0)) )
        {
            *x0 = (double)(0);
            *nr = 1;
            return;
        }
        if( ae_fp_eq(m1,(double)(0)) )
        {
            *x0 = (double)(1);
            *nr = 1;
            return;
        }
    }
    else
    {
        
        /*
         *consider case, when both of derivatives is 0
         */
        if( ae_fp_eq(m0,(double)(0))&&ae_fp_eq(m1,(double)(0)) )
        {
            *x0 = (double)(0);
            *x1 = (double)(1);
            *nr = 2;
            return;
        }
        
        /*
         *consider case, when derivative at 0 is 0, and derivative at 1 is't 0
         */
        if( ae_fp_eq(m0,(double)(0))&&ae_fp_neq(m1,(double)(0)) )
        {
            dd = b*b-4*a*c;
            if( ae_fp_less(dd,(double)(0)) )
            {
                *x0 = (double)(0);
                *nr = 1;
                return;
            }
            *x0 = (-b-ae_sqrt(dd, _state))/(2*a);
            *x1 = (-b+ae_sqrt(dd, _state))/(2*a);
            extr = -b/(2*a);
            exf = a*extr*extr+b*extr+c;
            if( ae_sign(exf, _state)*ae_sign(m1, _state)>0 )
            {
                *x0 = (double)(0);
                *nr = 1;
                return;
            }
            else
            {
                if( ae_fp_greater(extr,*x0) )
                {
                    *x0 = (double)(0);
                }
                else
                {
                    *x1 = (double)(0);
                }
                *nr = 2;
                
                /*
                 *roots must placed ascending
                 */
                if( ae_fp_greater(*x0,*x1) )
                {
                    tmp = *x0;
                    *x0 = *x1;
                    *x1 = tmp;
                }
                return;
            }
        }
        if( ae_fp_eq(m1,(double)(0))&&ae_fp_neq(m0,(double)(0)) )
        {
            dd = b*b-4*a*c;
            if( ae_fp_less(dd,(double)(0)) )
            {
                *x0 = (double)(1);
                *nr = 1;
                return;
            }
            *x0 = (-b-ae_sqrt(dd, _state))/(2*a);
            *x1 = (-b+ae_sqrt(dd, _state))/(2*a);
            extr = -b/(2*a);
            exf = a*extr*extr+b*extr+c;
            if( ae_sign(exf, _state)*ae_sign(m0, _state)>0 )
            {
                *x0 = (double)(1);
                *nr = 1;
                return;
            }
            else
            {
                if( ae_fp_less(extr,*x0) )
                {
                    *x0 = (double)(1);
                }
                else
                {
                    *x1 = (double)(1);
                }
                *nr = 2;
                
                /*
                 *roots must placed ascending
                 */
                if( ae_fp_greater(*x0,*x1) )
                {
                    tmp = *x0;
                    *x0 = *x1;
                    *x1 = tmp;
                }
                return;
            }
        }
        else
        {
            extr = -b/(2*a);
            exf = a*extr*extr+b*extr+c;
            if( ae_sign(exf, _state)*ae_sign(m0, _state)>0&&ae_sign(exf, _state)*ae_sign(m1, _state)>0 )
            {
                *nr = 0;
                return;
            }
            dd = b*b-4*a*c;
            if( ae_fp_less(dd,(double)(0)) )
            {
                *nr = 0;
                return;
            }
            *x0 = (-b-ae_sqrt(dd, _state))/(2*a);
            *x1 = (-b+ae_sqrt(dd, _state))/(2*a);
            
            /*
             *if EXF and m0, EXF and m1 has different signs, then equation has two roots              
             */
            if( ae_sign(exf, _state)*ae_sign(m0, _state)<0&&ae_sign(exf, _state)*ae_sign(m1, _state)<0 )
            {
                *nr = 2;
                
                /*
                 *roots must placed ascending
                 */
                if( ae_fp_greater(*x0,*x1) )
                {
                    tmp = *x0;
                    *x0 = *x1;
                    *x1 = tmp;
                }
                return;
            }
            else
            {
                *nr = 1;
                if( ae_sign(exf, _state)*ae_sign(m0, _state)<0 )
                {
                    if( ae_fp_less(*x1,extr) )
                    {
                        *x0 = *x1;
                    }
                    return;
                }
                if( ae_sign(exf, _state)*ae_sign(m1, _state)<0 )
                {
                    if( ae_fp_greater(*x1,extr) )
                    {
                        *x0 = *x1;
                    }
                    return;
                }
            }
        }
    }
}


/*************************************************************************
This procedure search roots of an cubic equation inside [A;B], it number of roots 
and number of extremums.

INPUT PARAMETERS:
    pA   -   value of a function at A
    mA   -   value of a derivative at A
    pB   -   value of a function at B
    mB   -   value of a derivative at B
    A0   -   left border [A0;B0]
    B0   -   right border [A0;B0]

OUTPUT PARAMETERS:
    X0   -  first root of an equation
    X1   -  second root of an equation
    X2   -  third root of an equation
    EX0  -  first extremum of a function
    EX0  -  second extremum of a function
    NR   -  number of roots
    NR   -  number of extrmums
    
RESTRICTIONS OF PARAMETERS:

Length of [A;B] must be positive and is't zero, i.e. A<>B and A<B.


REMARK:

If 'NR' is -1 it's mean, than polinom has infiniti roots.
If 'NE' is -1 it's mean, than polinom has infiniti extremums.

 -- ALGLIB PROJECT --
     Copyright 26.09.2011 by Bochkanov Sergey
*************************************************************************/
void solvecubicpolinom(double pa,
     double ma,
     double pb,
     double mb,
     double a,
     double b,
     double* x0,
     double* x1,
     double* x2,
     double* ex0,
     double* ex1,
     ae_int_t* nr,
     ae_int_t* ne,
     /* Real    */ ae_vector* tempdata,
     ae_state *_state)
{
    ae_int_t i;
    double tmpma;
    double tmpmb;
    double tex0;
    double tex1;

    *x0 = 0;
    *x1 = 0;
    *x2 = 0;
    *ex0 = 0;
    *ex1 = 0;
    *nr = 0;
    *ne = 0;

    rvectorsetlengthatleast(tempdata, 3, _state);
    
    /*
     *case, when A>B
     */
    ae_assert(ae_fp_less(a,b), "\nSolveCubicPolinom: incorrect borders for [A;B]!\n", _state);
    
    /*
     *case 1    
     *function can be identicaly to ZERO
     */
    if( ((ae_fp_eq(ma,(double)(0))&&ae_fp_eq(mb,(double)(0)))&&ae_fp_eq(pa,pb))&&ae_fp_eq(pa,(double)(0)) )
    {
        *nr = -1;
        *ne = -1;
        return;
    }
    if( (ae_fp_eq(ma,(double)(0))&&ae_fp_eq(mb,(double)(0)))&&ae_fp_eq(pa,pb) )
    {
        *nr = 0;
        *ne = -1;
        return;
    }
    tmpma = ma*(b-a);
    tmpmb = mb*(b-a);
    solvepolinom2(pa, tmpma, pb, tmpmb, ex0, ex1, ne, _state);
    *ex0 = spline1d_rescaleval((double)(0), (double)(1), a, b, *ex0, _state);
    *ex1 = spline1d_rescaleval((double)(0), (double)(1), a, b, *ex1, _state);
    
    /*
     *case 3.1
     *no extremums at [A;B]
     */
    if( *ne==0 )
    {
        *nr = bisectmethod(pa, tmpma, pb, tmpmb, (double)(0), (double)(1), x0, _state);
        if( *nr==1 )
        {
            *x0 = spline1d_rescaleval((double)(0), (double)(1), a, b, *x0, _state);
        }
        return;
    }
    
    /*
     *case 3.2
     *one extremum
     */
    if( *ne==1 )
    {
        if( ae_fp_eq(*ex0,a)||ae_fp_eq(*ex0,b) )
        {
            *nr = bisectmethod(pa, tmpma, pb, tmpmb, (double)(0), (double)(1), x0, _state);
            if( *nr==1 )
            {
                *x0 = spline1d_rescaleval((double)(0), (double)(1), a, b, *x0, _state);
            }
            return;
        }
        else
        {
            *nr = 0;
            i = 0;
            tex0 = spline1d_rescaleval(a, b, (double)(0), (double)(1), *ex0, _state);
            *nr = bisectmethod(pa, tmpma, pb, tmpmb, (double)(0), tex0, x0, _state)+(*nr);
            if( *nr>i )
            {
                tempdata->ptr.p_double[i] = spline1d_rescaleval((double)(0), tex0, a, *ex0, *x0, _state);
                i = i+1;
            }
            *nr = bisectmethod(pa, tmpma, pb, tmpmb, tex0, (double)(1), x0, _state)+(*nr);
            if( *nr>i )
            {
                *x0 = spline1d_rescaleval(tex0, (double)(1), *ex0, b, *x0, _state);
                if( i>0 )
                {
                    if( ae_fp_neq(*x0,tempdata->ptr.p_double[i-1]) )
                    {
                        tempdata->ptr.p_double[i] = *x0;
                        i = i+1;
                    }
                    else
                    {
                        *nr = *nr-1;
                    }
                }
                else
                {
                    tempdata->ptr.p_double[i] = *x0;
                    i = i+1;
                }
            }
            if( *nr>0 )
            {
                *x0 = tempdata->ptr.p_double[0];
                if( *nr>1 )
                {
                    *x1 = tempdata->ptr.p_double[1];
                }
                return;
            }
        }
        return;
    }
    else
    {
        
        /*
         *case 3.3
         *two extremums(or more, but it's impossible)
         *
         *
         *case 3.3.0
         *both extremums at the border
         */
        if( ae_fp_eq(*ex0,a)&&ae_fp_eq(*ex1,b) )
        {
            *nr = bisectmethod(pa, tmpma, pb, tmpmb, (double)(0), (double)(1), x0, _state);
            if( *nr==1 )
            {
                *x0 = spline1d_rescaleval((double)(0), (double)(1), a, b, *x0, _state);
            }
            return;
        }
        if( ae_fp_eq(*ex0,a)&&ae_fp_neq(*ex1,b) )
        {
            *nr = 0;
            i = 0;
            tex1 = spline1d_rescaleval(a, b, (double)(0), (double)(1), *ex1, _state);
            *nr = bisectmethod(pa, tmpma, pb, tmpmb, (double)(0), tex1, x0, _state)+(*nr);
            if( *nr>i )
            {
                tempdata->ptr.p_double[i] = spline1d_rescaleval((double)(0), tex1, a, *ex1, *x0, _state);
                i = i+1;
            }
            *nr = bisectmethod(pa, tmpma, pb, tmpmb, tex1, (double)(1), x0, _state)+(*nr);
            if( *nr>i )
            {
                *x0 = spline1d_rescaleval(tex1, (double)(1), *ex1, b, *x0, _state);
                if( ae_fp_neq(*x0,tempdata->ptr.p_double[i-1]) )
                {
                    tempdata->ptr.p_double[i] = *x0;
                    i = i+1;
                }
                else
                {
                    *nr = *nr-1;
                }
            }
            if( *nr>0 )
            {
                *x0 = tempdata->ptr.p_double[0];
                if( *nr>1 )
                {
                    *x1 = tempdata->ptr.p_double[1];
                }
                return;
            }
        }
        if( ae_fp_eq(*ex1,b)&&ae_fp_neq(*ex0,a) )
        {
            *nr = 0;
            i = 0;
            tex0 = spline1d_rescaleval(a, b, (double)(0), (double)(1), *ex0, _state);
            *nr = bisectmethod(pa, tmpma, pb, tmpmb, (double)(0), tex0, x0, _state)+(*nr);
            if( *nr>i )
            {
                tempdata->ptr.p_double[i] = spline1d_rescaleval((double)(0), tex0, a, *ex0, *x0, _state);
                i = i+1;
            }
            *nr = bisectmethod(pa, tmpma, pb, tmpmb, tex0, (double)(1), x0, _state)+(*nr);
            if( *nr>i )
            {
                *x0 = spline1d_rescaleval(tex0, (double)(1), *ex0, b, *x0, _state);
                if( i>0 )
                {
                    if( ae_fp_neq(*x0,tempdata->ptr.p_double[i-1]) )
                    {
                        tempdata->ptr.p_double[i] = *x0;
                        i = i+1;
                    }
                    else
                    {
                        *nr = *nr-1;
                    }
                }
                else
                {
                    tempdata->ptr.p_double[i] = *x0;
                    i = i+1;
                }
            }
            if( *nr>0 )
            {
                *x0 = tempdata->ptr.p_double[0];
                if( *nr>1 )
                {
                    *x1 = tempdata->ptr.p_double[1];
                }
                return;
            }
        }
        else
        {
            
            /*
             *case 3.3.2
             *both extremums inside (0;1)
             */
            *nr = 0;
            i = 0;
            tex0 = spline1d_rescaleval(a, b, (double)(0), (double)(1), *ex0, _state);
            tex1 = spline1d_rescaleval(a, b, (double)(0), (double)(1), *ex1, _state);
            *nr = bisectmethod(pa, tmpma, pb, tmpmb, (double)(0), tex0, x0, _state)+(*nr);
            if( *nr>i )
            {
                tempdata->ptr.p_double[i] = spline1d_rescaleval((double)(0), tex0, a, *ex0, *x0, _state);
                i = i+1;
            }
            *nr = bisectmethod(pa, tmpma, pb, tmpmb, tex0, tex1, x0, _state)+(*nr);
            if( *nr>i )
            {
                *x0 = spline1d_rescaleval(tex0, tex1, *ex0, *ex1, *x0, _state);
                if( i>0 )
                {
                    if( ae_fp_neq(*x0,tempdata->ptr.p_double[i-1]) )
                    {
                        tempdata->ptr.p_double[i] = *x0;
                        i = i+1;
                    }
                    else
                    {
                        *nr = *nr-1;
                    }
                }
                else
                {
                    tempdata->ptr.p_double[i] = *x0;
                    i = i+1;
                }
            }
            *nr = bisectmethod(pa, tmpma, pb, tmpmb, tex1, (double)(1), x0, _state)+(*nr);
            if( *nr>i )
            {
                *x0 = spline1d_rescaleval(tex1, (double)(1), *ex1, b, *x0, _state);
                if( i>0 )
                {
                    if( ae_fp_neq(*x0,tempdata->ptr.p_double[i-1]) )
                    {
                        tempdata->ptr.p_double[i] = *x0;
                        i = i+1;
                    }
                    else
                    {
                        *nr = *nr-1;
                    }
                }
                else
                {
                    tempdata->ptr.p_double[i] = *x0;
                    i = i+1;
                }
            }
            
            /*
             *write are found roots
             */
            if( *nr>0 )
            {
                *x0 = tempdata->ptr.p_double[0];
                if( *nr>1 )
                {
                    *x1 = tempdata->ptr.p_double[1];
                }
                if( *nr>2 )
                {
                    *x2 = tempdata->ptr.p_double[2];
                }
                return;
            }
        }
    }
}


/*************************************************************************
Function for searching a root at [A;B] by bisection method and return number of roots
(0 or 1)

INPUT PARAMETERS:
    pA   -   value of a function at A
    mA   -   value of a derivative at A
    pB   -   value of a function at B
    mB   -   value of a derivative at B
    A0   -   left border [A0;B0] 
    B0   -   right border [A0;B0]
    
RESTRICTIONS OF PARAMETERS:

We assume, that B0>A0.


REMARK:

Assume, that exist one root only at [A;B], else 
function may be work incorrectly.
The function dont check value A0,B0!

 -- ALGLIB PROJECT --
     Copyright 26.09.2011 by Bochkanov Sergey
*************************************************************************/
ae_int_t bisectmethod(double pa,
     double ma,
     double pb,
     double mb,
     double a,
     double b,
     double* x,
     ae_state *_state)
{
    double vacuum;
    double eps;
    double a0;
    double b0;
    double m;
    double lf;
    double rf;
    double mf;
    ae_int_t result;

    *x = 0;

    
    /*
     *accuracy
     */
    eps = 1000*(b-a)*ae_machineepsilon;
    
    /*
     *initialization left and right borders
     */
    a0 = a;
    b0 = b;
    
    /*
     *initialize function value at 'A' and 'B'
     */
    spline1d_hermitecalc(pa, ma, pb, mb, a, &lf, &vacuum, _state);
    spline1d_hermitecalc(pa, ma, pb, mb, b, &rf, &vacuum, _state);
    
    /*
     *check, that 'A' and 'B' are't roots,
     *and that root exist
     */
    if( ae_sign(lf, _state)*ae_sign(rf, _state)>0 )
    {
        result = 0;
        return result;
    }
    else
    {
        if( ae_fp_eq(lf,(double)(0)) )
        {
            *x = a;
            result = 1;
            return result;
        }
        else
        {
            if( ae_fp_eq(rf,(double)(0)) )
            {
                *x = b;
                result = 1;
                return result;
            }
        }
    }
    
    /*
     *searching a root
     */
    do
    {
        m = (b0+a0)/2;
        spline1d_hermitecalc(pa, ma, pb, mb, a0, &lf, &vacuum, _state);
        spline1d_hermitecalc(pa, ma, pb, mb, b0, &rf, &vacuum, _state);
        spline1d_hermitecalc(pa, ma, pb, mb, m, &mf, &vacuum, _state);
        if( ae_sign(mf, _state)*ae_sign(lf, _state)<0 )
        {
            b0 = m;
        }
        else
        {
            if( ae_sign(mf, _state)*ae_sign(rf, _state)<0 )
            {
                a0 = m;
            }
            else
            {
                if( ae_fp_eq(lf,(double)(0)) )
                {
                    *x = a0;
                    result = 1;
                    return result;
                }
                if( ae_fp_eq(rf,(double)(0)) )
                {
                    *x = b0;
                    result = 1;
                    return result;
                }
                if( ae_fp_eq(mf,(double)(0)) )
                {
                    *x = m;
                    result = 1;
                    return result;
                }
            }
        }
    }
    while(ae_fp_greater_eq(ae_fabs(b0-a0, _state),eps));
    *x = m;
    result = 1;
    return result;
}


/*************************************************************************
This function builds monotone cubic Hermite interpolant. This interpolant
is monotonic in [x(0),x(n-1)] and is constant outside of this interval.

In  case  y[]  form  non-monotonic  sequence,  interpolant  is  piecewise
monotonic.  Say, for x=(0,1,2,3,4)  and  y=(0,1,2,1,0)  interpolant  will
monotonically grow at [0..2] and monotonically decrease at [2..4].

INPUT PARAMETERS:
    X           -   spline nodes, array[0..N-1]. Subroutine automatically
                    sorts points, so caller may pass unsorted array.
    Y           -   function values, array[0..N-1]
    N           -   the number of points(N>=2).

OUTPUT PARAMETERS:
    C           -   spline interpolant.

 -- ALGLIB PROJECT --
     Copyright 21.06.2012 by Bochkanov Sergey
*************************************************************************/
void spline1dbuildmonotone(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     spline1dinterpolant* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_vector d;
    ae_vector ex;
    ae_vector ey;
    ae_vector p;
    double delta;
    double alpha;
    double beta;
    ae_int_t tmpn;
    ae_int_t sn;
    double ca;
    double cb;
    double epsilon;
    ae_int_t i;
    ae_int_t j;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    _spline1dinterpolant_clear(c);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&ex, 0, DT_REAL, _state);
    ae_vector_init(&ey, 0, DT_REAL, _state);
    ae_vector_init(&p, 0, DT_INT, _state);

    
    /*
     * Check lengths of arguments
     */
    ae_assert(n>=2, "Spline1DBuildMonotone: N<2", _state);
    ae_assert(x->cnt>=n, "Spline1DBuildMonotone: Length(X)<N", _state);
    ae_assert(y->cnt>=n, "Spline1DBuildMonotone: Length(Y)<N", _state);
    
    /*
     * Check and sort points
     */
    ae_assert(isfinitevector(x, n, _state), "Spline1DBuildMonotone: X contains infinite or NAN values", _state);
    ae_assert(isfinitevector(y, n, _state), "Spline1DBuildMonotone: Y contains infinite or NAN values", _state);
    spline1d_heapsortppoints(x, y, &p, n, _state);
    ae_assert(aredistinct(x, n, _state), "Spline1DBuildMonotone: at least two consequent points are too close", _state);
    epsilon = ae_machineepsilon;
    n = n+2;
    ae_vector_set_length(&d, n, _state);
    ae_vector_set_length(&ex, n, _state);
    ae_vector_set_length(&ey, n, _state);
    ex.ptr.p_double[0] = x->ptr.p_double[0]-ae_fabs(x->ptr.p_double[1]-x->ptr.p_double[0], _state);
    ex.ptr.p_double[n-1] = x->ptr.p_double[n-3]+ae_fabs(x->ptr.p_double[n-3]-x->ptr.p_double[n-4], _state);
    ey.ptr.p_double[0] = y->ptr.p_double[0];
    ey.ptr.p_double[n-1] = y->ptr.p_double[n-3];
    for(i=1; i<=n-2; i++)
    {
        ex.ptr.p_double[i] = x->ptr.p_double[i-1];
        ey.ptr.p_double[i] = y->ptr.p_double[i-1];
    }
    
    /*
     * Init sign of the function for first segment
     */
    i = 0;
    ca = (double)(0);
    do
    {
        ca = ey.ptr.p_double[i+1]-ey.ptr.p_double[i];
        i = i+1;
    }
    while(!(ae_fp_neq(ca,(double)(0))||i>n-2));
    if( ae_fp_neq(ca,(double)(0)) )
    {
        ca = ca/ae_fabs(ca, _state);
    }
    i = 0;
    while(i<n-1)
    {
        
        /*
         * Partition of the segment [X0;Xn]
         */
        tmpn = 1;
        for(j=i; j<=n-2; j++)
        {
            cb = ey.ptr.p_double[j+1]-ey.ptr.p_double[j];
            if( ae_fp_greater_eq(ca*cb,(double)(0)) )
            {
                tmpn = tmpn+1;
            }
            else
            {
                ca = cb/ae_fabs(cb, _state);
                break;
            }
        }
        sn = i+tmpn;
        ae_assert(tmpn>=2, "Spline1DBuildMonotone: internal error", _state);
        
        /*
         * Calculate derivatives for current segment
         */
        d.ptr.p_double[i] = (double)(0);
        d.ptr.p_double[sn-1] = (double)(0);
        for(j=i+1; j<=sn-2; j++)
        {
            d.ptr.p_double[j] = ((ey.ptr.p_double[j]-ey.ptr.p_double[j-1])/(ex.ptr.p_double[j]-ex.ptr.p_double[j-1])+(ey.ptr.p_double[j+1]-ey.ptr.p_double[j])/(ex.ptr.p_double[j+1]-ex.ptr.p_double[j]))/2;
        }
        for(j=i; j<=sn-2; j++)
        {
            delta = (ey.ptr.p_double[j+1]-ey.ptr.p_double[j])/(ex.ptr.p_double[j+1]-ex.ptr.p_double[j]);
            if( ae_fp_less_eq(ae_fabs(delta, _state),epsilon) )
            {
                d.ptr.p_double[j] = (double)(0);
                d.ptr.p_double[j+1] = (double)(0);
            }
            else
            {
                alpha = d.ptr.p_double[j]/delta;
                beta = d.ptr.p_double[j+1]/delta;
                if( ae_fp_neq(alpha,(double)(0)) )
                {
                    cb = alpha*ae_sqrt(1+ae_sqr(beta/alpha, _state), _state);
                }
                else
                {
                    if( ae_fp_neq(beta,(double)(0)) )
                    {
                        cb = beta;
                    }
                    else
                    {
                        continue;
                    }
                }
                if( ae_fp_greater(cb,(double)(3)) )
                {
                    d.ptr.p_double[j] = 3*alpha*delta/cb;
                    d.ptr.p_double[j+1] = 3*beta*delta/cb;
                }
            }
        }
        
        /*
         * Transition to next segment
         */
        i = sn-1;
    }
    spline1dbuildhermite(&ex, &ey, &d, n, c, _state);
    c->continuity = 2;
    ae_frame_leave(_state);
}


/*************************************************************************
Internal version of Spline1DGridDiffCubic.

Accepts pre-ordered X/Y, temporary arrays (which may be  preallocated,  if
you want to save time, or not) and output array (which may be preallocated
too).

Y is passed as var-parameter because we may need to force last element  to
be equal to the first one (if periodic boundary conditions are specified).

  -- ALGLIB PROJECT --
     Copyright 03.09.2010 by Bochkanov Sergey
*************************************************************************/
static void spline1d_spline1dgriddiffcubicinternal(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t boundltype,
     double boundl,
     ae_int_t boundrtype,
     double boundr,
     /* Real    */ ae_vector* d,
     /* Real    */ ae_vector* a1,
     /* Real    */ ae_vector* a2,
     /* Real    */ ae_vector* a3,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* dt,
     ae_state *_state)
{
    ae_int_t i;


    
    /*
     * allocate arrays
     */
    if( d->cnt<n )
    {
        ae_vector_set_length(d, n, _state);
    }
    if( a1->cnt<n )
    {
        ae_vector_set_length(a1, n, _state);
    }
    if( a2->cnt<n )
    {
        ae_vector_set_length(a2, n, _state);
    }
    if( a3->cnt<n )
    {
        ae_vector_set_length(a3, n, _state);
    }
    if( b->cnt<n )
    {
        ae_vector_set_length(b, n, _state);
    }
    if( dt->cnt<n )
    {
        ae_vector_set_length(dt, n, _state);
    }
    
    /*
     * Special cases:
     * * N=2, parabolic terminated boundary condition on both ends
     * * N=2, periodic boundary condition
     */
    if( (n==2&&boundltype==0)&&boundrtype==0 )
    {
        d->ptr.p_double[0] = (y->ptr.p_double[1]-y->ptr.p_double[0])/(x->ptr.p_double[1]-x->ptr.p_double[0]);
        d->ptr.p_double[1] = d->ptr.p_double[0];
        return;
    }
    if( (n==2&&boundltype==-1)&&boundrtype==-1 )
    {
        d->ptr.p_double[0] = (double)(0);
        d->ptr.p_double[1] = (double)(0);
        return;
    }
    
    /*
     * Periodic and non-periodic boundary conditions are
     * two separate classes
     */
    if( boundrtype==-1&&boundltype==-1 )
    {
        
        /*
         * Periodic boundary conditions
         */
        y->ptr.p_double[n-1] = y->ptr.p_double[0];
        
        /*
         * Boundary conditions at N-1 points
         * (one point less because last point is the same as first point).
         */
        a1->ptr.p_double[0] = x->ptr.p_double[1]-x->ptr.p_double[0];
        a2->ptr.p_double[0] = 2*(x->ptr.p_double[1]-x->ptr.p_double[0]+x->ptr.p_double[n-1]-x->ptr.p_double[n-2]);
        a3->ptr.p_double[0] = x->ptr.p_double[n-1]-x->ptr.p_double[n-2];
        b->ptr.p_double[0] = 3*(y->ptr.p_double[n-1]-y->ptr.p_double[n-2])/(x->ptr.p_double[n-1]-x->ptr.p_double[n-2])*(x->ptr.p_double[1]-x->ptr.p_double[0])+3*(y->ptr.p_double[1]-y->ptr.p_double[0])/(x->ptr.p_double[1]-x->ptr.p_double[0])*(x->ptr.p_double[n-1]-x->ptr.p_double[n-2]);
        for(i=1; i<=n-2; i++)
        {
            
            /*
             * Altough last point is [N-2], we use X[N-1] and Y[N-1]
             * (because of periodicity)
             */
            a1->ptr.p_double[i] = x->ptr.p_double[i+1]-x->ptr.p_double[i];
            a2->ptr.p_double[i] = 2*(x->ptr.p_double[i+1]-x->ptr.p_double[i-1]);
            a3->ptr.p_double[i] = x->ptr.p_double[i]-x->ptr.p_double[i-1];
            b->ptr.p_double[i] = 3*(y->ptr.p_double[i]-y->ptr.p_double[i-1])/(x->ptr.p_double[i]-x->ptr.p_double[i-1])*(x->ptr.p_double[i+1]-x->ptr.p_double[i])+3*(y->ptr.p_double[i+1]-y->ptr.p_double[i])/(x->ptr.p_double[i+1]-x->ptr.p_double[i])*(x->ptr.p_double[i]-x->ptr.p_double[i-1]);
        }
        
        /*
         * Solve, add last point (with index N-1)
         */
        spline1d_solvecyclictridiagonal(a1, a2, a3, b, n-1, dt, _state);
        ae_v_move(&d->ptr.p_double[0], 1, &dt->ptr.p_double[0], 1, ae_v_len(0,n-2));
        d->ptr.p_double[n-1] = d->ptr.p_double[0];
    }
    else
    {
        
        /*
         * Non-periodic boundary condition.
         * Left boundary conditions.
         */
        if( boundltype==0 )
        {
            a1->ptr.p_double[0] = (double)(0);
            a2->ptr.p_double[0] = (double)(1);
            a3->ptr.p_double[0] = (double)(1);
            b->ptr.p_double[0] = 2*(y->ptr.p_double[1]-y->ptr.p_double[0])/(x->ptr.p_double[1]-x->ptr.p_double[0]);
        }
        if( boundltype==1 )
        {
            a1->ptr.p_double[0] = (double)(0);
            a2->ptr.p_double[0] = (double)(1);
            a3->ptr.p_double[0] = (double)(0);
            b->ptr.p_double[0] = boundl;
        }
        if( boundltype==2 )
        {
            a1->ptr.p_double[0] = (double)(0);
            a2->ptr.p_double[0] = (double)(2);
            a3->ptr.p_double[0] = (double)(1);
            b->ptr.p_double[0] = 3*(y->ptr.p_double[1]-y->ptr.p_double[0])/(x->ptr.p_double[1]-x->ptr.p_double[0])-0.5*boundl*(x->ptr.p_double[1]-x->ptr.p_double[0]);
        }
        
        /*
         * Central conditions
         */
        for(i=1; i<=n-2; i++)
        {
            a1->ptr.p_double[i] = x->ptr.p_double[i+1]-x->ptr.p_double[i];
            a2->ptr.p_double[i] = 2*(x->ptr.p_double[i+1]-x->ptr.p_double[i-1]);
            a3->ptr.p_double[i] = x->ptr.p_double[i]-x->ptr.p_double[i-1];
            b->ptr.p_double[i] = 3*(y->ptr.p_double[i]-y->ptr.p_double[i-1])/(x->ptr.p_double[i]-x->ptr.p_double[i-1])*(x->ptr.p_double[i+1]-x->ptr.p_double[i])+3*(y->ptr.p_double[i+1]-y->ptr.p_double[i])/(x->ptr.p_double[i+1]-x->ptr.p_double[i])*(x->ptr.p_double[i]-x->ptr.p_double[i-1]);
        }
        
        /*
         * Right boundary conditions
         */
        if( boundrtype==0 )
        {
            a1->ptr.p_double[n-1] = (double)(1);
            a2->ptr.p_double[n-1] = (double)(1);
            a3->ptr.p_double[n-1] = (double)(0);
            b->ptr.p_double[n-1] = 2*(y->ptr.p_double[n-1]-y->ptr.p_double[n-2])/(x->ptr.p_double[n-1]-x->ptr.p_double[n-2]);
        }
        if( boundrtype==1 )
        {
            a1->ptr.p_double[n-1] = (double)(0);
            a2->ptr.p_double[n-1] = (double)(1);
            a3->ptr.p_double[n-1] = (double)(0);
            b->ptr.p_double[n-1] = boundr;
        }
        if( boundrtype==2 )
        {
            a1->ptr.p_double[n-1] = (double)(1);
            a2->ptr.p_double[n-1] = (double)(2);
            a3->ptr.p_double[n-1] = (double)(0);
            b->ptr.p_double[n-1] = 3*(y->ptr.p_double[n-1]-y->ptr.p_double[n-2])/(x->ptr.p_double[n-1]-x->ptr.p_double[n-2])+0.5*boundr*(x->ptr.p_double[n-1]-x->ptr.p_double[n-2]);
        }
        
        /*
         * Solve
         */
        spline1d_solvetridiagonal(a1, a2, a3, b, n, d, _state);
    }
}


/*************************************************************************
Internal subroutine. Heap sort.
*************************************************************************/
static void spline1d_heapsortpoints(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector bufx;
    ae_vector bufy;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&bufx, 0, DT_REAL, _state);
    ae_vector_init(&bufy, 0, DT_REAL, _state);

    tagsortfastr(x, y, &bufx, &bufy, n, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine. Heap sort.

Accepts:
    X, Y    -   points
    P       -   empty or preallocated array
    
Returns:
    X, Y    -   sorted by X
    P       -   array of permutations; I-th position of output
                arrays X/Y contains (X[P[I]],Y[P[I]])
*************************************************************************/
static void spline1d_heapsortppoints(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Integer */ ae_vector* p,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector rbuf;
    ae_vector ibuf;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&rbuf, 0, DT_REAL, _state);
    ae_vector_init(&ibuf, 0, DT_INT, _state);

    if( p->cnt<n )
    {
        ae_vector_set_length(p, n, _state);
    }
    ae_vector_set_length(&rbuf, n, _state);
    for(i=0; i<=n-1; i++)
    {
        p->ptr.p_int[i] = i;
    }
    tagsortfasti(x, p, &rbuf, &ibuf, n, _state);
    for(i=0; i<=n-1; i++)
    {
        rbuf.ptr.p_double[i] = y->ptr.p_double[p->ptr.p_int[i]];
    }
    ae_v_move(&y->ptr.p_double[0], 1, &rbuf.ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine. Tridiagonal solver. Solves

( B[0] C[0]                      
( A[1] B[1] C[1]                 )
(      A[2] B[2] C[2]            )
(            ..........          ) * X = D
(            ..........          )
(           A[N-2] B[N-2] C[N-2] )
(                  A[N-1] B[N-1] )

*************************************************************************/
static void spline1d_solvetridiagonal(/* Real    */ ae_vector* a,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* c,
     /* Real    */ ae_vector* d,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _b;
    ae_vector _d;
    ae_int_t k;
    double t;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_b, b, _state);
    b = &_b;
    ae_vector_init_copy(&_d, d, _state);
    d = &_d;

    if( x->cnt<n )
    {
        ae_vector_set_length(x, n, _state);
    }
    for(k=1; k<=n-1; k++)
    {
        t = a->ptr.p_double[k]/b->ptr.p_double[k-1];
        b->ptr.p_double[k] = b->ptr.p_double[k]-t*c->ptr.p_double[k-1];
        d->ptr.p_double[k] = d->ptr.p_double[k]-t*d->ptr.p_double[k-1];
    }
    x->ptr.p_double[n-1] = d->ptr.p_double[n-1]/b->ptr.p_double[n-1];
    for(k=n-2; k>=0; k--)
    {
        x->ptr.p_double[k] = (d->ptr.p_double[k]-c->ptr.p_double[k]*x->ptr.p_double[k+1])/b->ptr.p_double[k];
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine. Cyclic tridiagonal solver. Solves

( B[0] C[0]                 A[0] )
( A[1] B[1] C[1]                 )
(      A[2] B[2] C[2]            )
(            ..........          ) * X = D
(            ..........          )
(           A[N-2] B[N-2] C[N-2] )
( C[N-1]           A[N-1] B[N-1] )
*************************************************************************/
static void spline1d_solvecyclictridiagonal(/* Real    */ ae_vector* a,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* c,
     /* Real    */ ae_vector* d,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _b;
    ae_int_t k;
    double alpha;
    double beta;
    double gamma;
    ae_vector y;
    ae_vector z;
    ae_vector u;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_b, b, _state);
    b = &_b;
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&z, 0, DT_REAL, _state);
    ae_vector_init(&u, 0, DT_REAL, _state);

    if( x->cnt<n )
    {
        ae_vector_set_length(x, n, _state);
    }
    beta = a->ptr.p_double[0];
    alpha = c->ptr.p_double[n-1];
    gamma = -b->ptr.p_double[0];
    b->ptr.p_double[0] = 2*b->ptr.p_double[0];
    b->ptr.p_double[n-1] = b->ptr.p_double[n-1]-alpha*beta/gamma;
    ae_vector_set_length(&u, n, _state);
    for(k=0; k<=n-1; k++)
    {
        u.ptr.p_double[k] = (double)(0);
    }
    u.ptr.p_double[0] = gamma;
    u.ptr.p_double[n-1] = alpha;
    spline1d_solvetridiagonal(a, b, c, d, n, &y, _state);
    spline1d_solvetridiagonal(a, b, c, &u, n, &z, _state);
    for(k=0; k<=n-1; k++)
    {
        x->ptr.p_double[k] = y.ptr.p_double[k]-(y.ptr.p_double[0]+beta/gamma*y.ptr.p_double[n-1])/(1+z.ptr.p_double[0]+beta/gamma*z.ptr.p_double[n-1])*z.ptr.p_double[k];
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine. Three-point differentiation
*************************************************************************/
static double spline1d_diffthreepoint(double t,
     double x0,
     double f0,
     double x1,
     double f1,
     double x2,
     double f2,
     ae_state *_state)
{
    double a;
    double b;
    double result;


    t = t-x0;
    x1 = x1-x0;
    x2 = x2-x0;
    a = (f2-f0-x2/x1*(f1-f0))/(ae_sqr(x2, _state)-x1*x2);
    b = (f1-f0-a*ae_sqr(x1, _state))/x1;
    result = 2*a*t+b;
    return result;
}


/*************************************************************************
Procedure for calculating value of a function is providet in the form of
Hermite polinom  

INPUT PARAMETERS:
    P0   -   value of a function at 0
    M0   -   value of a derivative at 0
    P1   -   value of a function at 1
    M1   -   value of a derivative at 1
    T    -   point inside [0;1]
    
OUTPUT PARAMETERS:
    S    -   value of a function at T
    B0   -   value of a derivative function at T
    
 -- ALGLIB PROJECT --
     Copyright 26.09.2011 by Bochkanov Sergey
*************************************************************************/
static void spline1d_hermitecalc(double p0,
     double m0,
     double p1,
     double m1,
     double t,
     double* s,
     double* ds,
     ae_state *_state)
{

    *s = 0;
    *ds = 0;

    *s = p0*(1+2*t)*(1-t)*(1-t)+m0*t*(1-t)*(1-t)+p1*(3-2*t)*t*t+m1*t*t*(t-1);
    *ds = -p0*6*t*(1-t)+m0*(1-t)*(1-3*t)+p1*6*t*(1-t)+m1*t*(3*t-2);
}


/*************************************************************************
Function for mapping from [A0;B0] to [A1;B1]

INPUT PARAMETERS:
    A0   -   left border [A0;B0]
    B0   -   right border [A0;B0]
    A1   -   left border [A1;B1]
    B1   -   right border [A1;B1]
    T    -   value inside [A0;B0]  
    
RESTRICTIONS OF PARAMETERS:

We assume, that B0>A0 and B1>A1. But we chech, that T is inside [A0;B0], 
and if T<A0 then T become A1, if T>B0 then T - B1. 

INPUT PARAMETERS:
        A0   -   left border for segment [A0;B0] from 'T' is converted to [A1;B1] 
        B0   -   right border for segment [A0;B0] from 'T' is converted to [A1;B1] 
        A1   -   left border for segment [A1;B1] to 'T' is converted from [A0;B0] 
        B1   -   right border for segment [A1;B1] to 'T' is converted from [A0;B0] 
        T    -   the parameter is mapped from [A0;B0] to [A1;B1] 

Result:
    is converted value for 'T' from [A0;B0] to [A1;B1]
         
REMARK:

The function dont check value A0,B0 and A1,B1!

 -- ALGLIB PROJECT --
     Copyright 26.09.2011 by Bochkanov Sergey
*************************************************************************/
static double spline1d_rescaleval(double a0,
     double b0,
     double a1,
     double b1,
     double t,
     ae_state *_state)
{
    double result;


    
    /*
     *return left border
     */
    if( ae_fp_less_eq(t,a0) )
    {
        result = a1;
        return result;
    }
    
    /*
     *return right border
     */
    if( ae_fp_greater_eq(t,b0) )
    {
        result = b1;
        return result;
    }
    
    /*
     *return value between left and right borders
     */
    result = (b1-a1)*(t-a0)/(b0-a0)+a1;
    return result;
}


void _spline1dinterpolant_init(void* _p, ae_state *_state)
{
    spline1dinterpolant *p = (spline1dinterpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->x, 0, DT_REAL, _state);
    ae_vector_init(&p->c, 0, DT_REAL, _state);
}


void _spline1dinterpolant_init_copy(void* _dst, void* _src, ae_state *_state)
{
    spline1dinterpolant *dst = (spline1dinterpolant*)_dst;
    spline1dinterpolant *src = (spline1dinterpolant*)_src;
    dst->periodic = src->periodic;
    dst->n = src->n;
    dst->k = src->k;
    dst->continuity = src->continuity;
    ae_vector_init_copy(&dst->x, &src->x, _state);
    ae_vector_init_copy(&dst->c, &src->c, _state);
}


void _spline1dinterpolant_clear(void* _p)
{
    spline1dinterpolant *p = (spline1dinterpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->x);
    ae_vector_clear(&p->c);
}


void _spline1dinterpolant_destroy(void* _p)
{
    spline1dinterpolant *p = (spline1dinterpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->x);
    ae_vector_destroy(&p->c);
}


/*$ End $*/
