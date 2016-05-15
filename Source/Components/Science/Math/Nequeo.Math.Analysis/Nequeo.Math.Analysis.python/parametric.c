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
#include "parametric.h"


/*$ Declarations $*/
static void parametric_pspline2par(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t pt,
     /* Real    */ ae_vector* p,
     ae_state *_state);
static void parametric_pspline3par(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t pt,
     /* Real    */ ae_vector* p,
     ae_state *_state);
static void parametric_rdpanalyzesectionpar(/* Real    */ ae_matrix* xy,
     ae_int_t i0,
     ae_int_t i1,
     ae_int_t d,
     ae_int_t* worstidx,
     double* worsterror,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
This function  builds  non-periodic 2-dimensional parametric spline  which
starts at (X[0],Y[0]) and ends at (X[N-1],Y[N-1]).

INPUT PARAMETERS:
    XY  -   points, array[0..N-1,0..1].
            XY[I,0:1] corresponds to the Ith point.
            Order of points is important!
    N   -   points count, N>=5 for Akima splines, N>=2 for other types  of
            splines.
    ST  -   spline type:
            * 0     Akima spline
            * 1     parabolically terminated Catmull-Rom spline (Tension=0)
            * 2     parabolically terminated cubic spline
    PT  -   parameterization type:
            * 0     uniform
            * 1     chord length
            * 2     centripetal

OUTPUT PARAMETERS:
    P   -   parametric spline interpolant


NOTES:
* this function  assumes  that  there all consequent points  are distinct.
  I.e. (x0,y0)<>(x1,y1),  (x1,y1)<>(x2,y2),  (x2,y2)<>(x3,y3)  and  so on.
  However, non-consequent points may coincide, i.e. we can  have  (x0,y0)=
  =(x2,y2).

  -- ALGLIB PROJECT --
     Copyright 28.05.2010 by Bochkanov Sergey
*************************************************************************/
void pspline2build(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t st,
     ae_int_t pt,
     pspline2interpolant* p,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _xy;
    ae_vector tmp;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_xy, xy, _state);
    xy = &_xy;
    _pspline2interpolant_clear(p);
    ae_vector_init(&tmp, 0, DT_REAL, _state);

    ae_assert(st>=0&&st<=2, "PSpline2Build: incorrect spline type!", _state);
    ae_assert(pt>=0&&pt<=2, "PSpline2Build: incorrect parameterization type!", _state);
    if( st==0 )
    {
        ae_assert(n>=5, "PSpline2Build: N<5 (minimum value for Akima splines)!", _state);
    }
    else
    {
        ae_assert(n>=2, "PSpline2Build: N<2!", _state);
    }
    
    /*
     * Prepare
     */
    p->n = n;
    p->periodic = ae_false;
    ae_vector_set_length(&tmp, n, _state);
    
    /*
     * Build parameterization, check that all parameters are distinct
     */
    parametric_pspline2par(xy, n, pt, &p->p, _state);
    ae_assert(aredistinct(&p->p, n, _state), "PSpline2Build: consequent points are too close!", _state);
    
    /*
     * Build splines
     */
    if( st==0 )
    {
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][0], xy->stride, ae_v_len(0,n-1));
        spline1dbuildakima(&p->p, &tmp, n, &p->x, _state);
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][1], xy->stride, ae_v_len(0,n-1));
        spline1dbuildakima(&p->p, &tmp, n, &p->y, _state);
    }
    if( st==1 )
    {
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][0], xy->stride, ae_v_len(0,n-1));
        spline1dbuildcatmullrom(&p->p, &tmp, n, 0, 0.0, &p->x, _state);
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][1], xy->stride, ae_v_len(0,n-1));
        spline1dbuildcatmullrom(&p->p, &tmp, n, 0, 0.0, &p->y, _state);
    }
    if( st==2 )
    {
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][0], xy->stride, ae_v_len(0,n-1));
        spline1dbuildcubic(&p->p, &tmp, n, 0, 0.0, 0, 0.0, &p->x, _state);
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][1], xy->stride, ae_v_len(0,n-1));
        spline1dbuildcubic(&p->p, &tmp, n, 0, 0.0, 0, 0.0, &p->y, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function  builds  non-periodic 3-dimensional parametric spline  which
starts at (X[0],Y[0],Z[0]) and ends at (X[N-1],Y[N-1],Z[N-1]).

Same as PSpline2Build() function, but for 3D, so we  won't  duplicate  its
description here.

  -- ALGLIB PROJECT --
     Copyright 28.05.2010 by Bochkanov Sergey
*************************************************************************/
void pspline3build(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t st,
     ae_int_t pt,
     pspline3interpolant* p,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _xy;
    ae_vector tmp;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_xy, xy, _state);
    xy = &_xy;
    _pspline3interpolant_clear(p);
    ae_vector_init(&tmp, 0, DT_REAL, _state);

    ae_assert(st>=0&&st<=2, "PSpline3Build: incorrect spline type!", _state);
    ae_assert(pt>=0&&pt<=2, "PSpline3Build: incorrect parameterization type!", _state);
    if( st==0 )
    {
        ae_assert(n>=5, "PSpline3Build: N<5 (minimum value for Akima splines)!", _state);
    }
    else
    {
        ae_assert(n>=2, "PSpline3Build: N<2!", _state);
    }
    
    /*
     * Prepare
     */
    p->n = n;
    p->periodic = ae_false;
    ae_vector_set_length(&tmp, n, _state);
    
    /*
     * Build parameterization, check that all parameters are distinct
     */
    parametric_pspline3par(xy, n, pt, &p->p, _state);
    ae_assert(aredistinct(&p->p, n, _state), "PSpline3Build: consequent points are too close!", _state);
    
    /*
     * Build splines
     */
    if( st==0 )
    {
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][0], xy->stride, ae_v_len(0,n-1));
        spline1dbuildakima(&p->p, &tmp, n, &p->x, _state);
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][1], xy->stride, ae_v_len(0,n-1));
        spline1dbuildakima(&p->p, &tmp, n, &p->y, _state);
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][2], xy->stride, ae_v_len(0,n-1));
        spline1dbuildakima(&p->p, &tmp, n, &p->z, _state);
    }
    if( st==1 )
    {
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][0], xy->stride, ae_v_len(0,n-1));
        spline1dbuildcatmullrom(&p->p, &tmp, n, 0, 0.0, &p->x, _state);
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][1], xy->stride, ae_v_len(0,n-1));
        spline1dbuildcatmullrom(&p->p, &tmp, n, 0, 0.0, &p->y, _state);
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][2], xy->stride, ae_v_len(0,n-1));
        spline1dbuildcatmullrom(&p->p, &tmp, n, 0, 0.0, &p->z, _state);
    }
    if( st==2 )
    {
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][0], xy->stride, ae_v_len(0,n-1));
        spline1dbuildcubic(&p->p, &tmp, n, 0, 0.0, 0, 0.0, &p->x, _state);
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][1], xy->stride, ae_v_len(0,n-1));
        spline1dbuildcubic(&p->p, &tmp, n, 0, 0.0, 0, 0.0, &p->y, _state);
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][2], xy->stride, ae_v_len(0,n-1));
        spline1dbuildcubic(&p->p, &tmp, n, 0, 0.0, 0, 0.0, &p->z, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This  function  builds  periodic  2-dimensional  parametric  spline  which
starts at (X[0],Y[0]), goes through all points to (X[N-1],Y[N-1]) and then
back to (X[0],Y[0]).

INPUT PARAMETERS:
    XY  -   points, array[0..N-1,0..1].
            XY[I,0:1] corresponds to the Ith point.
            XY[N-1,0:1] must be different from XY[0,0:1].
            Order of points is important!
    N   -   points count, N>=3 for other types of splines.
    ST  -   spline type:
            * 1     Catmull-Rom spline (Tension=0) with cyclic boundary conditions
            * 2     cubic spline with cyclic boundary conditions
    PT  -   parameterization type:
            * 0     uniform
            * 1     chord length
            * 2     centripetal

OUTPUT PARAMETERS:
    P   -   parametric spline interpolant


NOTES:
* this function  assumes  that there all consequent points  are  distinct.
  I.e. (x0,y0)<>(x1,y1), (x1,y1)<>(x2,y2),  (x2,y2)<>(x3,y3)  and  so  on.
  However, non-consequent points may coincide, i.e. we can  have  (x0,y0)=
  =(x2,y2).
* last point of sequence is NOT equal to the first  point.  You  shouldn't
  make curve "explicitly periodic" by making them equal.

  -- ALGLIB PROJECT --
     Copyright 28.05.2010 by Bochkanov Sergey
*************************************************************************/
void pspline2buildperiodic(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t st,
     ae_int_t pt,
     pspline2interpolant* p,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _xy;
    ae_matrix xyp;
    ae_vector tmp;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_xy, xy, _state);
    xy = &_xy;
    _pspline2interpolant_clear(p);
    ae_matrix_init(&xyp, 0, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);

    ae_assert(st>=1&&st<=2, "PSpline2BuildPeriodic: incorrect spline type!", _state);
    ae_assert(pt>=0&&pt<=2, "PSpline2BuildPeriodic: incorrect parameterization type!", _state);
    ae_assert(n>=3, "PSpline2BuildPeriodic: N<3!", _state);
    
    /*
     * Prepare
     */
    p->n = n;
    p->periodic = ae_true;
    ae_vector_set_length(&tmp, n+1, _state);
    ae_matrix_set_length(&xyp, n+1, 2, _state);
    ae_v_move(&xyp.ptr.pp_double[0][0], xyp.stride, &xy->ptr.pp_double[0][0], xy->stride, ae_v_len(0,n-1));
    ae_v_move(&xyp.ptr.pp_double[0][1], xyp.stride, &xy->ptr.pp_double[0][1], xy->stride, ae_v_len(0,n-1));
    ae_v_move(&xyp.ptr.pp_double[n][0], 1, &xy->ptr.pp_double[0][0], 1, ae_v_len(0,1));
    
    /*
     * Build parameterization, check that all parameters are distinct
     */
    parametric_pspline2par(&xyp, n+1, pt, &p->p, _state);
    ae_assert(aredistinct(&p->p, n+1, _state), "PSpline2BuildPeriodic: consequent (or first and last) points are too close!", _state);
    
    /*
     * Build splines
     */
    if( st==1 )
    {
        ae_v_move(&tmp.ptr.p_double[0], 1, &xyp.ptr.pp_double[0][0], xyp.stride, ae_v_len(0,n));
        spline1dbuildcatmullrom(&p->p, &tmp, n+1, -1, 0.0, &p->x, _state);
        ae_v_move(&tmp.ptr.p_double[0], 1, &xyp.ptr.pp_double[0][1], xyp.stride, ae_v_len(0,n));
        spline1dbuildcatmullrom(&p->p, &tmp, n+1, -1, 0.0, &p->y, _state);
    }
    if( st==2 )
    {
        ae_v_move(&tmp.ptr.p_double[0], 1, &xyp.ptr.pp_double[0][0], xyp.stride, ae_v_len(0,n));
        spline1dbuildcubic(&p->p, &tmp, n+1, -1, 0.0, -1, 0.0, &p->x, _state);
        ae_v_move(&tmp.ptr.p_double[0], 1, &xyp.ptr.pp_double[0][1], xyp.stride, ae_v_len(0,n));
        spline1dbuildcubic(&p->p, &tmp, n+1, -1, 0.0, -1, 0.0, &p->y, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This  function  builds  periodic  3-dimensional  parametric  spline  which
starts at (X[0],Y[0],Z[0]), goes through all points to (X[N-1],Y[N-1],Z[N-1])
and then back to (X[0],Y[0],Z[0]).

Same as PSpline2Build() function, but for 3D, so we  won't  duplicate  its
description here.

  -- ALGLIB PROJECT --
     Copyright 28.05.2010 by Bochkanov Sergey
*************************************************************************/
void pspline3buildperiodic(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t st,
     ae_int_t pt,
     pspline3interpolant* p,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _xy;
    ae_matrix xyp;
    ae_vector tmp;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_xy, xy, _state);
    xy = &_xy;
    _pspline3interpolant_clear(p);
    ae_matrix_init(&xyp, 0, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);

    ae_assert(st>=1&&st<=2, "PSpline3BuildPeriodic: incorrect spline type!", _state);
    ae_assert(pt>=0&&pt<=2, "PSpline3BuildPeriodic: incorrect parameterization type!", _state);
    ae_assert(n>=3, "PSpline3BuildPeriodic: N<3!", _state);
    
    /*
     * Prepare
     */
    p->n = n;
    p->periodic = ae_true;
    ae_vector_set_length(&tmp, n+1, _state);
    ae_matrix_set_length(&xyp, n+1, 3, _state);
    ae_v_move(&xyp.ptr.pp_double[0][0], xyp.stride, &xy->ptr.pp_double[0][0], xy->stride, ae_v_len(0,n-1));
    ae_v_move(&xyp.ptr.pp_double[0][1], xyp.stride, &xy->ptr.pp_double[0][1], xy->stride, ae_v_len(0,n-1));
    ae_v_move(&xyp.ptr.pp_double[0][2], xyp.stride, &xy->ptr.pp_double[0][2], xy->stride, ae_v_len(0,n-1));
    ae_v_move(&xyp.ptr.pp_double[n][0], 1, &xy->ptr.pp_double[0][0], 1, ae_v_len(0,2));
    
    /*
     * Build parameterization, check that all parameters are distinct
     */
    parametric_pspline3par(&xyp, n+1, pt, &p->p, _state);
    ae_assert(aredistinct(&p->p, n+1, _state), "PSplineBuild2Periodic: consequent (or first and last) points are too close!", _state);
    
    /*
     * Build splines
     */
    if( st==1 )
    {
        ae_v_move(&tmp.ptr.p_double[0], 1, &xyp.ptr.pp_double[0][0], xyp.stride, ae_v_len(0,n));
        spline1dbuildcatmullrom(&p->p, &tmp, n+1, -1, 0.0, &p->x, _state);
        ae_v_move(&tmp.ptr.p_double[0], 1, &xyp.ptr.pp_double[0][1], xyp.stride, ae_v_len(0,n));
        spline1dbuildcatmullrom(&p->p, &tmp, n+1, -1, 0.0, &p->y, _state);
        ae_v_move(&tmp.ptr.p_double[0], 1, &xyp.ptr.pp_double[0][2], xyp.stride, ae_v_len(0,n));
        spline1dbuildcatmullrom(&p->p, &tmp, n+1, -1, 0.0, &p->z, _state);
    }
    if( st==2 )
    {
        ae_v_move(&tmp.ptr.p_double[0], 1, &xyp.ptr.pp_double[0][0], xyp.stride, ae_v_len(0,n));
        spline1dbuildcubic(&p->p, &tmp, n+1, -1, 0.0, -1, 0.0, &p->x, _state);
        ae_v_move(&tmp.ptr.p_double[0], 1, &xyp.ptr.pp_double[0][1], xyp.stride, ae_v_len(0,n));
        spline1dbuildcubic(&p->p, &tmp, n+1, -1, 0.0, -1, 0.0, &p->y, _state);
        ae_v_move(&tmp.ptr.p_double[0], 1, &xyp.ptr.pp_double[0][2], xyp.stride, ae_v_len(0,n));
        spline1dbuildcubic(&p->p, &tmp, n+1, -1, 0.0, -1, 0.0, &p->z, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function returns vector of parameter values correspoding to points.

I.e. for P created from (X[0],Y[0])...(X[N-1],Y[N-1]) and U=TValues(P)  we
have
    (X[0],Y[0]) = PSpline2Calc(P,U[0]),
    (X[1],Y[1]) = PSpline2Calc(P,U[1]),
    (X[2],Y[2]) = PSpline2Calc(P,U[2]),
    ...

INPUT PARAMETERS:
    P   -   parametric spline interpolant

OUTPUT PARAMETERS:
    N   -   array size
    T   -   array[0..N-1]


NOTES:
* for non-periodic splines U[0]=0, U[0]<U[1]<...<U[N-1], U[N-1]=1
* for periodic splines     U[0]=0, U[0]<U[1]<...<U[N-1], U[N-1]<1

  -- ALGLIB PROJECT --
     Copyright 28.05.2010 by Bochkanov Sergey
*************************************************************************/
void pspline2parametervalues(pspline2interpolant* p,
     ae_int_t* n,
     /* Real    */ ae_vector* t,
     ae_state *_state)
{

    *n = 0;
    ae_vector_clear(t);

    ae_assert(p->n>=2, "PSpline2ParameterValues: internal error!", _state);
    *n = p->n;
    ae_vector_set_length(t, *n, _state);
    ae_v_move(&t->ptr.p_double[0], 1, &p->p.ptr.p_double[0], 1, ae_v_len(0,*n-1));
    t->ptr.p_double[0] = (double)(0);
    if( !p->periodic )
    {
        t->ptr.p_double[*n-1] = (double)(1);
    }
}


/*************************************************************************
This function returns vector of parameter values correspoding to points.

Same as PSpline2ParameterValues(), but for 3D.

  -- ALGLIB PROJECT --
     Copyright 28.05.2010 by Bochkanov Sergey
*************************************************************************/
void pspline3parametervalues(pspline3interpolant* p,
     ae_int_t* n,
     /* Real    */ ae_vector* t,
     ae_state *_state)
{

    *n = 0;
    ae_vector_clear(t);

    ae_assert(p->n>=2, "PSpline3ParameterValues: internal error!", _state);
    *n = p->n;
    ae_vector_set_length(t, *n, _state);
    ae_v_move(&t->ptr.p_double[0], 1, &p->p.ptr.p_double[0], 1, ae_v_len(0,*n-1));
    t->ptr.p_double[0] = (double)(0);
    if( !p->periodic )
    {
        t->ptr.p_double[*n-1] = (double)(1);
    }
}


/*************************************************************************
This function  calculates  the value of the parametric spline for a  given
value of parameter T

INPUT PARAMETERS:
    P   -   parametric spline interpolant
    T   -   point:
            * T in [0,1] corresponds to interval spanned by points
            * for non-periodic splines T<0 (or T>1) correspond to parts of
              the curve before the first (after the last) point
            * for periodic splines T<0 (or T>1) are projected  into  [0,1]
              by making T=T-floor(T).

OUTPUT PARAMETERS:
    X   -   X-position
    Y   -   Y-position


  -- ALGLIB PROJECT --
     Copyright 28.05.2010 by Bochkanov Sergey
*************************************************************************/
void pspline2calc(pspline2interpolant* p,
     double t,
     double* x,
     double* y,
     ae_state *_state)
{

    *x = 0;
    *y = 0;

    if( p->periodic )
    {
        t = t-ae_ifloor(t, _state);
    }
    *x = spline1dcalc(&p->x, t, _state);
    *y = spline1dcalc(&p->y, t, _state);
}


/*************************************************************************
This function  calculates  the value of the parametric spline for a  given
value of parameter T.

INPUT PARAMETERS:
    P   -   parametric spline interpolant
    T   -   point:
            * T in [0,1] corresponds to interval spanned by points
            * for non-periodic splines T<0 (or T>1) correspond to parts of
              the curve before the first (after the last) point
            * for periodic splines T<0 (or T>1) are projected  into  [0,1]
              by making T=T-floor(T).

OUTPUT PARAMETERS:
    X   -   X-position
    Y   -   Y-position
    Z   -   Z-position


  -- ALGLIB PROJECT --
     Copyright 28.05.2010 by Bochkanov Sergey
*************************************************************************/
void pspline3calc(pspline3interpolant* p,
     double t,
     double* x,
     double* y,
     double* z,
     ae_state *_state)
{

    *x = 0;
    *y = 0;
    *z = 0;

    if( p->periodic )
    {
        t = t-ae_ifloor(t, _state);
    }
    *x = spline1dcalc(&p->x, t, _state);
    *y = spline1dcalc(&p->y, t, _state);
    *z = spline1dcalc(&p->z, t, _state);
}


/*************************************************************************
This function  calculates  tangent vector for a given value of parameter T

INPUT PARAMETERS:
    P   -   parametric spline interpolant
    T   -   point:
            * T in [0,1] corresponds to interval spanned by points
            * for non-periodic splines T<0 (or T>1) correspond to parts of
              the curve before the first (after the last) point
            * for periodic splines T<0 (or T>1) are projected  into  [0,1]
              by making T=T-floor(T).

OUTPUT PARAMETERS:
    X    -   X-component of tangent vector (normalized)
    Y    -   Y-component of tangent vector (normalized)
    
NOTE:
    X^2+Y^2 is either 1 (for non-zero tangent vector) or 0.


  -- ALGLIB PROJECT --
     Copyright 28.05.2010 by Bochkanov Sergey
*************************************************************************/
void pspline2tangent(pspline2interpolant* p,
     double t,
     double* x,
     double* y,
     ae_state *_state)
{
    double v;
    double v0;
    double v1;

    *x = 0;
    *y = 0;

    if( p->periodic )
    {
        t = t-ae_ifloor(t, _state);
    }
    pspline2diff(p, t, &v0, x, &v1, y, _state);
    if( ae_fp_neq(*x,(double)(0))||ae_fp_neq(*y,(double)(0)) )
    {
        
        /*
         * this code is a bit more complex than X^2+Y^2 to avoid
         * overflow for large values of X and Y.
         */
        v = safepythag2(*x, *y, _state);
        *x = *x/v;
        *y = *y/v;
    }
}


/*************************************************************************
This function  calculates  tangent vector for a given value of parameter T

INPUT PARAMETERS:
    P   -   parametric spline interpolant
    T   -   point:
            * T in [0,1] corresponds to interval spanned by points
            * for non-periodic splines T<0 (or T>1) correspond to parts of
              the curve before the first (after the last) point
            * for periodic splines T<0 (or T>1) are projected  into  [0,1]
              by making T=T-floor(T).

OUTPUT PARAMETERS:
    X    -   X-component of tangent vector (normalized)
    Y    -   Y-component of tangent vector (normalized)
    Z    -   Z-component of tangent vector (normalized)

NOTE:
    X^2+Y^2+Z^2 is either 1 (for non-zero tangent vector) or 0.


  -- ALGLIB PROJECT --
     Copyright 28.05.2010 by Bochkanov Sergey
*************************************************************************/
void pspline3tangent(pspline3interpolant* p,
     double t,
     double* x,
     double* y,
     double* z,
     ae_state *_state)
{
    double v;
    double v0;
    double v1;
    double v2;

    *x = 0;
    *y = 0;
    *z = 0;

    if( p->periodic )
    {
        t = t-ae_ifloor(t, _state);
    }
    pspline3diff(p, t, &v0, x, &v1, y, &v2, z, _state);
    if( (ae_fp_neq(*x,(double)(0))||ae_fp_neq(*y,(double)(0)))||ae_fp_neq(*z,(double)(0)) )
    {
        v = safepythag3(*x, *y, *z, _state);
        *x = *x/v;
        *y = *y/v;
        *z = *z/v;
    }
}


/*************************************************************************
This function calculates derivative, i.e. it returns (dX/dT,dY/dT).

INPUT PARAMETERS:
    P   -   parametric spline interpolant
    T   -   point:
            * T in [0,1] corresponds to interval spanned by points
            * for non-periodic splines T<0 (or T>1) correspond to parts of
              the curve before the first (after the last) point
            * for periodic splines T<0 (or T>1) are projected  into  [0,1]
              by making T=T-floor(T).

OUTPUT PARAMETERS:
    X   -   X-value
    DX  -   X-derivative
    Y   -   Y-value
    DY  -   Y-derivative


  -- ALGLIB PROJECT --
     Copyright 28.05.2010 by Bochkanov Sergey
*************************************************************************/
void pspline2diff(pspline2interpolant* p,
     double t,
     double* x,
     double* dx,
     double* y,
     double* dy,
     ae_state *_state)
{
    double d2s;

    *x = 0;
    *dx = 0;
    *y = 0;
    *dy = 0;

    if( p->periodic )
    {
        t = t-ae_ifloor(t, _state);
    }
    spline1ddiff(&p->x, t, x, dx, &d2s, _state);
    spline1ddiff(&p->y, t, y, dy, &d2s, _state);
}


/*************************************************************************
This function calculates derivative, i.e. it returns (dX/dT,dY/dT,dZ/dT).

INPUT PARAMETERS:
    P   -   parametric spline interpolant
    T   -   point:
            * T in [0,1] corresponds to interval spanned by points
            * for non-periodic splines T<0 (or T>1) correspond to parts of
              the curve before the first (after the last) point
            * for periodic splines T<0 (or T>1) are projected  into  [0,1]
              by making T=T-floor(T).

OUTPUT PARAMETERS:
    X   -   X-value
    DX  -   X-derivative
    Y   -   Y-value
    DY  -   Y-derivative
    Z   -   Z-value
    DZ  -   Z-derivative


  -- ALGLIB PROJECT --
     Copyright 28.05.2010 by Bochkanov Sergey
*************************************************************************/
void pspline3diff(pspline3interpolant* p,
     double t,
     double* x,
     double* dx,
     double* y,
     double* dy,
     double* z,
     double* dz,
     ae_state *_state)
{
    double d2s;

    *x = 0;
    *dx = 0;
    *y = 0;
    *dy = 0;
    *z = 0;
    *dz = 0;

    if( p->periodic )
    {
        t = t-ae_ifloor(t, _state);
    }
    spline1ddiff(&p->x, t, x, dx, &d2s, _state);
    spline1ddiff(&p->y, t, y, dy, &d2s, _state);
    spline1ddiff(&p->z, t, z, dz, &d2s, _state);
}


/*************************************************************************
This function calculates first and second derivative with respect to T.

INPUT PARAMETERS:
    P   -   parametric spline interpolant
    T   -   point:
            * T in [0,1] corresponds to interval spanned by points
            * for non-periodic splines T<0 (or T>1) correspond to parts of
              the curve before the first (after the last) point
            * for periodic splines T<0 (or T>1) are projected  into  [0,1]
              by making T=T-floor(T).

OUTPUT PARAMETERS:
    X   -   X-value
    DX  -   derivative
    D2X -   second derivative
    Y   -   Y-value
    DY  -   derivative
    D2Y -   second derivative


  -- ALGLIB PROJECT --
     Copyright 28.05.2010 by Bochkanov Sergey
*************************************************************************/
void pspline2diff2(pspline2interpolant* p,
     double t,
     double* x,
     double* dx,
     double* d2x,
     double* y,
     double* dy,
     double* d2y,
     ae_state *_state)
{

    *x = 0;
    *dx = 0;
    *d2x = 0;
    *y = 0;
    *dy = 0;
    *d2y = 0;

    if( p->periodic )
    {
        t = t-ae_ifloor(t, _state);
    }
    spline1ddiff(&p->x, t, x, dx, d2x, _state);
    spline1ddiff(&p->y, t, y, dy, d2y, _state);
}


/*************************************************************************
This function calculates first and second derivative with respect to T.

INPUT PARAMETERS:
    P   -   parametric spline interpolant
    T   -   point:
            * T in [0,1] corresponds to interval spanned by points
            * for non-periodic splines T<0 (or T>1) correspond to parts of
              the curve before the first (after the last) point
            * for periodic splines T<0 (or T>1) are projected  into  [0,1]
              by making T=T-floor(T).

OUTPUT PARAMETERS:
    X   -   X-value
    DX  -   derivative
    D2X -   second derivative
    Y   -   Y-value
    DY  -   derivative
    D2Y -   second derivative
    Z   -   Z-value
    DZ  -   derivative
    D2Z -   second derivative


  -- ALGLIB PROJECT --
     Copyright 28.05.2010 by Bochkanov Sergey
*************************************************************************/
void pspline3diff2(pspline3interpolant* p,
     double t,
     double* x,
     double* dx,
     double* d2x,
     double* y,
     double* dy,
     double* d2y,
     double* z,
     double* dz,
     double* d2z,
     ae_state *_state)
{

    *x = 0;
    *dx = 0;
    *d2x = 0;
    *y = 0;
    *dy = 0;
    *d2y = 0;
    *z = 0;
    *dz = 0;
    *d2z = 0;

    if( p->periodic )
    {
        t = t-ae_ifloor(t, _state);
    }
    spline1ddiff(&p->x, t, x, dx, d2x, _state);
    spline1ddiff(&p->y, t, y, dy, d2y, _state);
    spline1ddiff(&p->z, t, z, dz, d2z, _state);
}


/*************************************************************************
This function  calculates  arc length, i.e. length of  curve  between  t=a
and t=b.

INPUT PARAMETERS:
    P   -   parametric spline interpolant
    A,B -   parameter values corresponding to arc ends:
            * B>A will result in positive length returned
            * B<A will result in negative length returned

RESULT:
    length of arc starting at T=A and ending at T=B.


  -- ALGLIB PROJECT --
     Copyright 30.05.2010 by Bochkanov Sergey
*************************************************************************/
double pspline2arclength(pspline2interpolant* p,
     double a,
     double b,
     ae_state *_state)
{
    ae_frame _frame_block;
    autogkstate state;
    autogkreport rep;
    double sx;
    double dsx;
    double d2sx;
    double sy;
    double dsy;
    double d2sy;
    double result;

    ae_frame_make(_state, &_frame_block);
    _autogkstate_init(&state, _state);
    _autogkreport_init(&rep, _state);

    autogksmooth(a, b, &state, _state);
    while(autogkiteration(&state, _state))
    {
        spline1ddiff(&p->x, state.x, &sx, &dsx, &d2sx, _state);
        spline1ddiff(&p->y, state.x, &sy, &dsy, &d2sy, _state);
        state.f = safepythag2(dsx, dsy, _state);
    }
    autogkresults(&state, &result, &rep, _state);
    ae_assert(rep.terminationtype>0, "PSpline2ArcLength: internal error!", _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function  calculates  arc length, i.e. length of  curve  between  t=a
and t=b.

INPUT PARAMETERS:
    P   -   parametric spline interpolant
    A,B -   parameter values corresponding to arc ends:
            * B>A will result in positive length returned
            * B<A will result in negative length returned

RESULT:
    length of arc starting at T=A and ending at T=B.


  -- ALGLIB PROJECT --
     Copyright 30.05.2010 by Bochkanov Sergey
*************************************************************************/
double pspline3arclength(pspline3interpolant* p,
     double a,
     double b,
     ae_state *_state)
{
    ae_frame _frame_block;
    autogkstate state;
    autogkreport rep;
    double sx;
    double dsx;
    double d2sx;
    double sy;
    double dsy;
    double d2sy;
    double sz;
    double dsz;
    double d2sz;
    double result;

    ae_frame_make(_state, &_frame_block);
    _autogkstate_init(&state, _state);
    _autogkreport_init(&rep, _state);

    autogksmooth(a, b, &state, _state);
    while(autogkiteration(&state, _state))
    {
        spline1ddiff(&p->x, state.x, &sx, &dsx, &d2sx, _state);
        spline1ddiff(&p->y, state.x, &sy, &dsy, &d2sy, _state);
        spline1ddiff(&p->z, state.x, &sz, &dsz, &d2sz, _state);
        state.f = safepythag3(dsx, dsy, dsz, _state);
    }
    autogkresults(&state, &result, &rep, _state);
    ae_assert(rep.terminationtype>0, "PSpline3ArcLength: internal error!", _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This  subroutine fits piecewise linear curve to points with Ramer-Douglas-
Peucker algorithm. This  function  performs PARAMETRIC fit, i.e. it can be
used to fit curves like circles.

On  input  it  accepts dataset which describes parametric multidimensional
curve X(t), with X being vector, and t taking values in [0,N), where N  is
a number of points in dataset. As result, it returns reduced  dataset  X2,
which can be used to build  parametric  curve  X2(t),  which  approximates
X(t) with desired precision (or has specified number of sections).


INPUT PARAMETERS:
    X       -   array of multidimensional points:
                * at least N elements, leading N elements are used if more
                  than N elements were specified
                * order of points is IMPORTANT because  it  is  parametric
                  fit
                * each row of array is one point which has D coordinates
    N       -   number of elements in X
    D       -   number of dimensions (elements per row of X)
    StopM   -   stopping condition - desired number of sections:
                * at most M sections are generated by this function
                * less than M sections can be generated if we have N<M
                  (or some X are non-distinct).
                * zero StopM means that algorithm does not stop after
                  achieving some pre-specified section count
    StopEps -   stopping condition - desired precision:
                * algorithm stops after error in each section is at most Eps
                * zero Eps means that algorithm does not stop after
                  achieving some pre-specified precision

OUTPUT PARAMETERS:
    X2      -   array of corner points for piecewise approximation,
                has length NSections+1 or zero (for NSections=0).
    Idx2    -   array of indexes (parameter values):
                * has length NSections+1 or zero (for NSections=0).
                * each element of Idx2 corresponds to same-numbered
                  element of X2
                * each element of Idx2 is index of  corresponding  element
                  of X2 at original array X, i.e. I-th  row  of  X2  is
                  Idx2[I]-th row of X.
                * elements of Idx2 can be treated as parameter values
                  which should be used when building new parametric curve
                * Idx2[0]=0, Idx2[NSections]=N-1
    NSections-  number of sections found by algorithm, NSections<=M,
                NSections can be zero for degenerate datasets
                (N<=1 or all X[] are non-distinct).

NOTE: algorithm stops after:
      a) dividing curve into StopM sections 
      b) achieving required precision StopEps
      c) dividing curve into N-1 sections
      If both StopM and StopEps are non-zero, algorithm is stopped by  the
      FIRST criterion which is satisfied. In case both StopM  and  StopEps
      are zero, algorithm stops because of (c).
                
  -- ALGLIB --
     Copyright 02.10.2014 by Bochkanov Sergey
*************************************************************************/
void parametricrdpfixed(/* Real    */ ae_matrix* x,
     ae_int_t n,
     ae_int_t d,
     ae_int_t stopm,
     double stopeps,
     /* Real    */ ae_matrix* x2,
     /* Integer */ ae_vector* idx2,
     ae_int_t* nsections,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_bool allsame;
    ae_int_t k0;
    ae_int_t k1;
    ae_int_t k2;
    double e0;
    double e1;
    ae_int_t idx0;
    ae_int_t idx1;
    ae_int_t worstidx;
    double worsterror;
    ae_matrix sections;
    ae_vector heaperrors;
    ae_vector heaptags;
    ae_vector buf0;
    ae_vector buf1;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(x2);
    ae_vector_clear(idx2);
    *nsections = 0;
    ae_matrix_init(&sections, 0, 0, DT_REAL, _state);
    ae_vector_init(&heaperrors, 0, DT_REAL, _state);
    ae_vector_init(&heaptags, 0, DT_INT, _state);
    ae_vector_init(&buf0, 0, DT_REAL, _state);
    ae_vector_init(&buf1, 0, DT_REAL, _state);

    ae_assert(n>=0, "LSTFitPiecewiseLinearParametricRDP: N<0", _state);
    ae_assert(d>=1, "LSTFitPiecewiseLinearParametricRDP: D<=0", _state);
    ae_assert(stopm>=0, "LSTFitPiecewiseLinearParametricRDP: StopM<1", _state);
    ae_assert(ae_isfinite(stopeps, _state)&&ae_fp_greater_eq(stopeps,(double)(0)), "LSTFitPiecewiseLinearParametricRDP: StopEps<0 or is infinite", _state);
    ae_assert(x->rows>=n, "LSTFitPiecewiseLinearParametricRDP: Rows(X)<N", _state);
    ae_assert(x->cols>=d, "LSTFitPiecewiseLinearParametricRDP: Cols(X)<D", _state);
    ae_assert(apservisfinitematrix(x, n, d, _state), "LSTFitPiecewiseLinearParametricRDP: X contains infinite/NAN values", _state);
    
    /*
     * Handle degenerate cases
     */
    if( n<=1 )
    {
        *nsections = 0;
        ae_frame_leave(_state);
        return;
    }
    allsame = ae_true;
    for(i=1; i<=n-1; i++)
    {
        for(j=0; j<=d-1; j++)
        {
            allsame = allsame&&ae_fp_eq(x->ptr.pp_double[i][j],x->ptr.pp_double[0][j]);
        }
    }
    if( allsame )
    {
        *nsections = 0;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Prepare first section
     */
    parametric_rdpanalyzesectionpar(x, 0, n-1, d, &worstidx, &worsterror, _state);
    ae_matrix_set_length(&sections, n, 4, _state);
    ae_vector_set_length(&heaperrors, n, _state);
    ae_vector_set_length(&heaptags, n, _state);
    *nsections = 1;
    sections.ptr.pp_double[0][0] = (double)(0);
    sections.ptr.pp_double[0][1] = (double)(n-1);
    sections.ptr.pp_double[0][2] = (double)(worstidx);
    sections.ptr.pp_double[0][3] = worsterror;
    heaperrors.ptr.p_double[0] = worsterror;
    heaptags.ptr.p_int[0] = 0;
    ae_assert(ae_fp_eq(sections.ptr.pp_double[0][1],(double)(n-1)), "RDP algorithm: integrity check failed", _state);
    
    /*
     * Main loop.
     * Repeatedly find section with worst error and divide it.
     * Terminate after M-th section, or because of other reasons (see loop internals).
     */
    for(;;)
    {
        
        /*
         * Break loop if one of the stopping conditions was met.
         * Store index of worst section to K.
         */
        if( ae_fp_eq(heaperrors.ptr.p_double[0],(double)(0)) )
        {
            break;
        }
        if( ae_fp_greater(stopeps,(double)(0))&&ae_fp_less_eq(heaperrors.ptr.p_double[0],stopeps) )
        {
            break;
        }
        if( stopm>0&&*nsections>=stopm )
        {
            break;
        }
        k = heaptags.ptr.p_int[0];
        
        /*
         * K-th section is divided in two:
         * * first  one spans interval from X[Sections[K,0]] to X[Sections[K,2]]
         * * second one spans interval from X[Sections[K,2]] to X[Sections[K,1]]
         *
         * First section is stored at K-th position, second one is appended to the table.
         * Then we update heap which stores pairs of (error,section_index)
         */
        k0 = ae_round(sections.ptr.pp_double[k][0], _state);
        k1 = ae_round(sections.ptr.pp_double[k][1], _state);
        k2 = ae_round(sections.ptr.pp_double[k][2], _state);
        parametric_rdpanalyzesectionpar(x, k0, k2, d, &idx0, &e0, _state);
        parametric_rdpanalyzesectionpar(x, k2, k1, d, &idx1, &e1, _state);
        sections.ptr.pp_double[k][0] = (double)(k0);
        sections.ptr.pp_double[k][1] = (double)(k2);
        sections.ptr.pp_double[k][2] = (double)(idx0);
        sections.ptr.pp_double[k][3] = e0;
        tagheapreplacetopi(&heaperrors, &heaptags, *nsections, e0, k, _state);
        sections.ptr.pp_double[*nsections][0] = (double)(k2);
        sections.ptr.pp_double[*nsections][1] = (double)(k1);
        sections.ptr.pp_double[*nsections][2] = (double)(idx1);
        sections.ptr.pp_double[*nsections][3] = e1;
        tagheappushi(&heaperrors, &heaptags, nsections, e1, *nsections, _state);
    }
    
    /*
     * Convert from sections to indexes
     */
    ae_vector_set_length(&buf0, *nsections+1, _state);
    for(i=0; i<=*nsections-1; i++)
    {
        buf0.ptr.p_double[i] = (double)(ae_round(sections.ptr.pp_double[i][0], _state));
    }
    buf0.ptr.p_double[*nsections] = (double)(n-1);
    tagsortfast(&buf0, &buf1, *nsections+1, _state);
    ae_vector_set_length(idx2, *nsections+1, _state);
    for(i=0; i<=*nsections; i++)
    {
        idx2->ptr.p_int[i] = ae_round(buf0.ptr.p_double[i], _state);
    }
    ae_assert(idx2->ptr.p_int[0]==0, "RDP algorithm: integrity check failed", _state);
    ae_assert(idx2->ptr.p_int[*nsections]==n-1, "RDP algorithm: integrity check failed", _state);
    
    /*
     * Output sections:
     * * first NSection elements of X2/Y2 are filled by x/y at left boundaries of sections
     * * last element of X2/Y2 is filled by right boundary of rightmost section
     * * X2/Y2 is sorted by ascending of X2
     */
    ae_matrix_set_length(x2, *nsections+1, d, _state);
    for(i=0; i<=*nsections; i++)
    {
        for(j=0; j<=d-1; j++)
        {
            x2->ptr.pp_double[i][j] = x->ptr.pp_double[idx2->ptr.p_int[i]][j];
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Builds non-periodic parameterization for 2-dimensional spline
*************************************************************************/
static void parametric_pspline2par(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t pt,
     /* Real    */ ae_vector* p,
     ae_state *_state)
{
    double v;
    ae_int_t i;

    ae_vector_clear(p);

    ae_assert(pt>=0&&pt<=2, "PSpline2Par: internal error!", _state);
    
    /*
     * Build parameterization:
     * * fill by non-normalized values
     * * normalize them so we have P[0]=0, P[N-1]=1.
     */
    ae_vector_set_length(p, n, _state);
    if( pt==0 )
    {
        for(i=0; i<=n-1; i++)
        {
            p->ptr.p_double[i] = (double)(i);
        }
    }
    if( pt==1 )
    {
        p->ptr.p_double[0] = (double)(0);
        for(i=1; i<=n-1; i++)
        {
            p->ptr.p_double[i] = p->ptr.p_double[i-1]+safepythag2(xy->ptr.pp_double[i][0]-xy->ptr.pp_double[i-1][0], xy->ptr.pp_double[i][1]-xy->ptr.pp_double[i-1][1], _state);
        }
    }
    if( pt==2 )
    {
        p->ptr.p_double[0] = (double)(0);
        for(i=1; i<=n-1; i++)
        {
            p->ptr.p_double[i] = p->ptr.p_double[i-1]+ae_sqrt(safepythag2(xy->ptr.pp_double[i][0]-xy->ptr.pp_double[i-1][0], xy->ptr.pp_double[i][1]-xy->ptr.pp_double[i-1][1], _state), _state);
        }
    }
    v = 1/p->ptr.p_double[n-1];
    ae_v_muld(&p->ptr.p_double[0], 1, ae_v_len(0,n-1), v);
}


/*************************************************************************
Builds non-periodic parameterization for 3-dimensional spline
*************************************************************************/
static void parametric_pspline3par(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t pt,
     /* Real    */ ae_vector* p,
     ae_state *_state)
{
    double v;
    ae_int_t i;

    ae_vector_clear(p);

    ae_assert(pt>=0&&pt<=2, "PSpline3Par: internal error!", _state);
    
    /*
     * Build parameterization:
     * * fill by non-normalized values
     * * normalize them so we have P[0]=0, P[N-1]=1.
     */
    ae_vector_set_length(p, n, _state);
    if( pt==0 )
    {
        for(i=0; i<=n-1; i++)
        {
            p->ptr.p_double[i] = (double)(i);
        }
    }
    if( pt==1 )
    {
        p->ptr.p_double[0] = (double)(0);
        for(i=1; i<=n-1; i++)
        {
            p->ptr.p_double[i] = p->ptr.p_double[i-1]+safepythag3(xy->ptr.pp_double[i][0]-xy->ptr.pp_double[i-1][0], xy->ptr.pp_double[i][1]-xy->ptr.pp_double[i-1][1], xy->ptr.pp_double[i][2]-xy->ptr.pp_double[i-1][2], _state);
        }
    }
    if( pt==2 )
    {
        p->ptr.p_double[0] = (double)(0);
        for(i=1; i<=n-1; i++)
        {
            p->ptr.p_double[i] = p->ptr.p_double[i-1]+ae_sqrt(safepythag3(xy->ptr.pp_double[i][0]-xy->ptr.pp_double[i-1][0], xy->ptr.pp_double[i][1]-xy->ptr.pp_double[i-1][1], xy->ptr.pp_double[i][2]-xy->ptr.pp_double[i-1][2], _state), _state);
        }
    }
    v = 1/p->ptr.p_double[n-1];
    ae_v_muld(&p->ptr.p_double[0], 1, ae_v_len(0,n-1), v);
}


/*************************************************************************
This function analyzes section of curve for processing by RDP algorithm:
given set of points X,Y with indexes [I0,I1] it returns point with
worst deviation from linear model (PARAMETRIC version which sees curve
as X(t) with vector X).

Input parameters:
    XY          -   array
    I0,I1       -   interval (boundaries included) to process
    D           -   number of dimensions
    
OUTPUT PARAMETERS:
    WorstIdx    -   index of worst point
    WorstError  -   error at worst point
    
NOTE: this function guarantees that it returns exactly zero for a section
      with less than 3 points.

  -- ALGLIB PROJECT --
     Copyright 02.10.2014 by Bochkanov Sergey
*************************************************************************/
static void parametric_rdpanalyzesectionpar(/* Real    */ ae_matrix* xy,
     ae_int_t i0,
     ae_int_t i1,
     ae_int_t d,
     ae_int_t* worstidx,
     double* worsterror,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;
    double d2;
    double ts;
    double vv;

    *worstidx = 0;
    *worsterror = 0;

    
    /*
     * Quick exit for 0, 1, 2 points
     */
    if( i1-i0+1<3 )
    {
        *worstidx = i0;
        *worsterror = 0.0;
        return;
    }
    
    /*
     * Estimate D2 - squared distance between XY[I1] and XY[I0].
     * In case D2=0 handle it as special case.
     */
    d2 = 0.0;
    for(j=0; j<=d-1; j++)
    {
        d2 = d2+ae_sqr(xy->ptr.pp_double[i1][j]-xy->ptr.pp_double[i0][j], _state);
    }
    if( ae_fp_eq(d2,(double)(0)) )
    {
        
        /*
         * First and last points are equal, interval evaluation is
         * trivial - we just calculate distance from all points to
         * the first/last one.
         */
        *worstidx = i0;
        *worsterror = 0.0;
        for(i=i0+1; i<=i1-1; i++)
        {
            vv = 0.0;
            for(j=0; j<=d-1; j++)
            {
                v = xy->ptr.pp_double[i][j]-xy->ptr.pp_double[i0][j];
                vv = vv+v*v;
            }
            vv = ae_sqrt(vv, _state);
            if( ae_fp_greater(vv,*worsterror) )
            {
                *worsterror = vv;
                *worstidx = i;
            }
        }
        return;
    }
    
    /*
     * General case
     *
     * Current section of curve is modeled as x(t) = d*t+c, where
     *     d = XY[I1]-XY[I0]
     *     c = XY[I0]
     *     t is in [0,1]
     */
    *worstidx = i0;
    *worsterror = 0.0;
    for(i=i0+1; i<=i1-1; i++)
    {
        
        /*
         * Determine t_s - parameter value for projected point.
         */
        ts = (double)(i-i0)/(double)(i1-i0);
        
        /*
         * Estimate error norm
         */
        vv = 0.0;
        for(j=0; j<=d-1; j++)
        {
            v = (xy->ptr.pp_double[i1][j]-xy->ptr.pp_double[i0][j])*ts-(xy->ptr.pp_double[i][j]-xy->ptr.pp_double[i0][j]);
            vv = vv+ae_sqr(v, _state);
        }
        vv = ae_sqrt(vv, _state);
        if( ae_fp_greater(vv,*worsterror) )
        {
            *worsterror = vv;
            *worstidx = i;
        }
    }
}


void _pspline2interpolant_init(void* _p, ae_state *_state)
{
    pspline2interpolant *p = (pspline2interpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->p, 0, DT_REAL, _state);
    _spline1dinterpolant_init(&p->x, _state);
    _spline1dinterpolant_init(&p->y, _state);
}


void _pspline2interpolant_init_copy(void* _dst, void* _src, ae_state *_state)
{
    pspline2interpolant *dst = (pspline2interpolant*)_dst;
    pspline2interpolant *src = (pspline2interpolant*)_src;
    dst->n = src->n;
    dst->periodic = src->periodic;
    ae_vector_init_copy(&dst->p, &src->p, _state);
    _spline1dinterpolant_init_copy(&dst->x, &src->x, _state);
    _spline1dinterpolant_init_copy(&dst->y, &src->y, _state);
}


void _pspline2interpolant_clear(void* _p)
{
    pspline2interpolant *p = (pspline2interpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->p);
    _spline1dinterpolant_clear(&p->x);
    _spline1dinterpolant_clear(&p->y);
}


void _pspline2interpolant_destroy(void* _p)
{
    pspline2interpolant *p = (pspline2interpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->p);
    _spline1dinterpolant_destroy(&p->x);
    _spline1dinterpolant_destroy(&p->y);
}


void _pspline3interpolant_init(void* _p, ae_state *_state)
{
    pspline3interpolant *p = (pspline3interpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->p, 0, DT_REAL, _state);
    _spline1dinterpolant_init(&p->x, _state);
    _spline1dinterpolant_init(&p->y, _state);
    _spline1dinterpolant_init(&p->z, _state);
}


void _pspline3interpolant_init_copy(void* _dst, void* _src, ae_state *_state)
{
    pspline3interpolant *dst = (pspline3interpolant*)_dst;
    pspline3interpolant *src = (pspline3interpolant*)_src;
    dst->n = src->n;
    dst->periodic = src->periodic;
    ae_vector_init_copy(&dst->p, &src->p, _state);
    _spline1dinterpolant_init_copy(&dst->x, &src->x, _state);
    _spline1dinterpolant_init_copy(&dst->y, &src->y, _state);
    _spline1dinterpolant_init_copy(&dst->z, &src->z, _state);
}


void _pspline3interpolant_clear(void* _p)
{
    pspline3interpolant *p = (pspline3interpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->p);
    _spline1dinterpolant_clear(&p->x);
    _spline1dinterpolant_clear(&p->y);
    _spline1dinterpolant_clear(&p->z);
}


void _pspline3interpolant_destroy(void* _p)
{
    pspline3interpolant *p = (pspline3interpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->p);
    _spline1dinterpolant_destroy(&p->x);
    _spline1dinterpolant_destroy(&p->y);
    _spline1dinterpolant_destroy(&p->z);
}


/*$ End $*/
