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
#include "spline2d.h"


/*$ Declarations $*/
static void spline2d_bicubiccalcderivatives(/* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* dx,
     /* Real    */ ae_matrix* dy,
     /* Real    */ ae_matrix* dxy,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
This subroutine calculates the value of the bilinear or bicubic spline  at
the given point X.

Input parameters:
    C   -   coefficients table.
            Built by BuildBilinearSpline or BuildBicubicSpline.
    X, Y-   point

Result:
    S(x,y)

  -- ALGLIB PROJECT --
     Copyright 05.07.2007 by Bochkanov Sergey
*************************************************************************/
double spline2dcalc(spline2dinterpolant* c,
     double x,
     double y,
     ae_state *_state)
{
    double v;
    double vx;
    double vy;
    double vxy;
    double result;


    ae_assert(c->stype==-1||c->stype==-3, "Spline2DCalc: incorrect C (incorrect parameter C.SType)", _state);
    ae_assert(ae_isfinite(x, _state)&&ae_isfinite(y, _state), "Spline2DCalc: X or Y contains NaN or Infinite value", _state);
    if( c->d!=1 )
    {
        result = (double)(0);
        return result;
    }
    spline2ddiff(c, x, y, &v, &vx, &vy, &vxy, _state);
    result = v;
    return result;
}


/*************************************************************************
This subroutine calculates the value of the bilinear or bicubic spline  at
the given point X and its derivatives.

Input parameters:
    C   -   spline interpolant.
    X, Y-   point

Output parameters:
    F   -   S(x,y)
    FX  -   dS(x,y)/dX
    FY  -   dS(x,y)/dY
    FXY -   d2S(x,y)/dXdY

  -- ALGLIB PROJECT --
     Copyright 05.07.2007 by Bochkanov Sergey
*************************************************************************/
void spline2ddiff(spline2dinterpolant* c,
     double x,
     double y,
     double* f,
     double* fx,
     double* fy,
     double* fxy,
     ae_state *_state)
{
    double t;
    double dt;
    double u;
    double du;
    ae_int_t ix;
    ae_int_t iy;
    ae_int_t l;
    ae_int_t r;
    ae_int_t h;
    ae_int_t s1;
    ae_int_t s2;
    ae_int_t s3;
    ae_int_t s4;
    ae_int_t sfx;
    ae_int_t sfy;
    ae_int_t sfxy;
    double y1;
    double y2;
    double y3;
    double y4;
    double v;
    double t0;
    double t1;
    double t2;
    double t3;
    double u0;
    double u1;
    double u2;
    double u3;

    *f = 0;
    *fx = 0;
    *fy = 0;
    *fxy = 0;

    ae_assert(c->stype==-1||c->stype==-3, "Spline2DDiff: incorrect C (incorrect parameter C.SType)", _state);
    ae_assert(ae_isfinite(x, _state)&&ae_isfinite(y, _state), "Spline2DDiff: X or Y contains NaN or Infinite value", _state);
    
    /*
     * Prepare F, dF/dX, dF/dY, d2F/dXdY
     */
    *f = (double)(0);
    *fx = (double)(0);
    *fy = (double)(0);
    *fxy = (double)(0);
    if( c->d!=1 )
    {
        return;
    }
    
    /*
     * Binary search in the [ x[0], ..., x[n-2] ] (x[n-1] is not included)
     */
    l = 0;
    r = c->n-1;
    while(l!=r-1)
    {
        h = (l+r)/2;
        if( ae_fp_greater_eq(c->x.ptr.p_double[h],x) )
        {
            r = h;
        }
        else
        {
            l = h;
        }
    }
    t = (x-c->x.ptr.p_double[l])/(c->x.ptr.p_double[l+1]-c->x.ptr.p_double[l]);
    dt = 1.0/(c->x.ptr.p_double[l+1]-c->x.ptr.p_double[l]);
    ix = l;
    
    /*
     * Binary search in the [ y[0], ..., y[m-2] ] (y[m-1] is not included)
     */
    l = 0;
    r = c->m-1;
    while(l!=r-1)
    {
        h = (l+r)/2;
        if( ae_fp_greater_eq(c->y.ptr.p_double[h],y) )
        {
            r = h;
        }
        else
        {
            l = h;
        }
    }
    u = (y-c->y.ptr.p_double[l])/(c->y.ptr.p_double[l+1]-c->y.ptr.p_double[l]);
    du = 1.0/(c->y.ptr.p_double[l+1]-c->y.ptr.p_double[l]);
    iy = l;
    
    /*
     * Bilinear interpolation
     */
    if( c->stype==-1 )
    {
        y1 = c->f.ptr.p_double[c->n*iy+ix];
        y2 = c->f.ptr.p_double[c->n*iy+(ix+1)];
        y3 = c->f.ptr.p_double[c->n*(iy+1)+(ix+1)];
        y4 = c->f.ptr.p_double[c->n*(iy+1)+ix];
        *f = (1-t)*(1-u)*y1+t*(1-u)*y2+t*u*y3+(1-t)*u*y4;
        *fx = (-(1-u)*y1+(1-u)*y2+u*y3-u*y4)*dt;
        *fy = (-(1-t)*y1-t*y2+t*y3+(1-t)*y4)*du;
        *fxy = (y1-y2+y3-y4)*du*dt;
        return;
    }
    
    /*
     * Bicubic interpolation
     */
    if( c->stype==-3 )
    {
        
        /*
         * Prepare info
         */
        t0 = (double)(1);
        t1 = t;
        t2 = ae_sqr(t, _state);
        t3 = t*t2;
        u0 = (double)(1);
        u1 = u;
        u2 = ae_sqr(u, _state);
        u3 = u*u2;
        sfx = c->n*c->m;
        sfy = 2*c->n*c->m;
        sfxy = 3*c->n*c->m;
        s1 = c->n*iy+ix;
        s2 = c->n*iy+(ix+1);
        s3 = c->n*(iy+1)+(ix+1);
        s4 = c->n*(iy+1)+ix;
        
        /*
         * Calculate
         */
        v = c->f.ptr.p_double[s1];
        *f = *f+v*t0*u0;
        v = c->f.ptr.p_double[sfy+s1]/du;
        *f = *f+v*t0*u1;
        *fy = *fy+v*t0*u0*du;
        v = -3*c->f.ptr.p_double[s1]+3*c->f.ptr.p_double[s4]-2*c->f.ptr.p_double[sfy+s1]/du-c->f.ptr.p_double[sfy+s4]/du;
        *f = *f+v*t0*u2;
        *fy = *fy+2*v*t0*u1*du;
        v = 2*c->f.ptr.p_double[s1]-2*c->f.ptr.p_double[s4]+c->f.ptr.p_double[sfy+s1]/du+c->f.ptr.p_double[sfy+s4]/du;
        *f = *f+v*t0*u3;
        *fy = *fy+3*v*t0*u2*du;
        v = c->f.ptr.p_double[sfx+s1]/dt;
        *f = *f+v*t1*u0;
        *fx = *fx+v*t0*u0*dt;
        v = c->f.ptr.p_double[sfxy+s1]/(dt*du);
        *f = *f+v*t1*u1;
        *fx = *fx+v*t0*u1*dt;
        *fy = *fy+v*t1*u0*du;
        *fxy = *fxy+v*t0*u0*dt*du;
        v = -3*c->f.ptr.p_double[sfx+s1]/dt+3*c->f.ptr.p_double[sfx+s4]/dt-2*c->f.ptr.p_double[sfxy+s1]/(dt*du)-c->f.ptr.p_double[sfxy+s4]/(dt*du);
        *f = *f+v*t1*u2;
        *fx = *fx+v*t0*u2*dt;
        *fy = *fy+2*v*t1*u1*du;
        *fxy = *fxy+2*v*t0*u1*dt*du;
        v = 2*c->f.ptr.p_double[sfx+s1]/dt-2*c->f.ptr.p_double[sfx+s4]/dt+c->f.ptr.p_double[sfxy+s1]/(dt*du)+c->f.ptr.p_double[sfxy+s4]/(dt*du);
        *f = *f+v*t1*u3;
        *fx = *fx+v*t0*u3*dt;
        *fy = *fy+3*v*t1*u2*du;
        *fxy = *fxy+3*v*t0*u2*dt*du;
        v = -3*c->f.ptr.p_double[s1]+3*c->f.ptr.p_double[s2]-2*c->f.ptr.p_double[sfx+s1]/dt-c->f.ptr.p_double[sfx+s2]/dt;
        *f = *f+v*t2*u0;
        *fx = *fx+2*v*t1*u0*dt;
        v = -3*c->f.ptr.p_double[sfy+s1]/du+3*c->f.ptr.p_double[sfy+s2]/du-2*c->f.ptr.p_double[sfxy+s1]/(dt*du)-c->f.ptr.p_double[sfxy+s2]/(dt*du);
        *f = *f+v*t2*u1;
        *fx = *fx+2*v*t1*u1*dt;
        *fy = *fy+v*t2*u0*du;
        *fxy = *fxy+2*v*t1*u0*dt*du;
        v = 9*c->f.ptr.p_double[s1]-9*c->f.ptr.p_double[s2]+9*c->f.ptr.p_double[s3]-9*c->f.ptr.p_double[s4]+6*c->f.ptr.p_double[sfx+s1]/dt+3*c->f.ptr.p_double[sfx+s2]/dt-3*c->f.ptr.p_double[sfx+s3]/dt-6*c->f.ptr.p_double[sfx+s4]/dt+6*c->f.ptr.p_double[sfy+s1]/du-6*c->f.ptr.p_double[sfy+s2]/du-3*c->f.ptr.p_double[sfy+s3]/du+3*c->f.ptr.p_double[sfy+s4]/du+4*c->f.ptr.p_double[sfxy+s1]/(dt*du)+2*c->f.ptr.p_double[sfxy+s2]/(dt*du)+c->f.ptr.p_double[sfxy+s3]/(dt*du)+2*c->f.ptr.p_double[sfxy+s4]/(dt*du);
        *f = *f+v*t2*u2;
        *fx = *fx+2*v*t1*u2*dt;
        *fy = *fy+2*v*t2*u1*du;
        *fxy = *fxy+4*v*t1*u1*dt*du;
        v = -6*c->f.ptr.p_double[s1]+6*c->f.ptr.p_double[s2]-6*c->f.ptr.p_double[s3]+6*c->f.ptr.p_double[s4]-4*c->f.ptr.p_double[sfx+s1]/dt-2*c->f.ptr.p_double[sfx+s2]/dt+2*c->f.ptr.p_double[sfx+s3]/dt+4*c->f.ptr.p_double[sfx+s4]/dt-3*c->f.ptr.p_double[sfy+s1]/du+3*c->f.ptr.p_double[sfy+s2]/du+3*c->f.ptr.p_double[sfy+s3]/du-3*c->f.ptr.p_double[sfy+s4]/du-2*c->f.ptr.p_double[sfxy+s1]/(dt*du)-c->f.ptr.p_double[sfxy+s2]/(dt*du)-c->f.ptr.p_double[sfxy+s3]/(dt*du)-2*c->f.ptr.p_double[sfxy+s4]/(dt*du);
        *f = *f+v*t2*u3;
        *fx = *fx+2*v*t1*u3*dt;
        *fy = *fy+3*v*t2*u2*du;
        *fxy = *fxy+6*v*t1*u2*dt*du;
        v = 2*c->f.ptr.p_double[s1]-2*c->f.ptr.p_double[s2]+c->f.ptr.p_double[sfx+s1]/dt+c->f.ptr.p_double[sfx+s2]/dt;
        *f = *f+v*t3*u0;
        *fx = *fx+3*v*t2*u0*dt;
        v = 2*c->f.ptr.p_double[sfy+s1]/du-2*c->f.ptr.p_double[sfy+s2]/du+c->f.ptr.p_double[sfxy+s1]/(dt*du)+c->f.ptr.p_double[sfxy+s2]/(dt*du);
        *f = *f+v*t3*u1;
        *fx = *fx+3*v*t2*u1*dt;
        *fy = *fy+v*t3*u0*du;
        *fxy = *fxy+3*v*t2*u0*dt*du;
        v = -6*c->f.ptr.p_double[s1]+6*c->f.ptr.p_double[s2]-6*c->f.ptr.p_double[s3]+6*c->f.ptr.p_double[s4]-3*c->f.ptr.p_double[sfx+s1]/dt-3*c->f.ptr.p_double[sfx+s2]/dt+3*c->f.ptr.p_double[sfx+s3]/dt+3*c->f.ptr.p_double[sfx+s4]/dt-4*c->f.ptr.p_double[sfy+s1]/du+4*c->f.ptr.p_double[sfy+s2]/du+2*c->f.ptr.p_double[sfy+s3]/du-2*c->f.ptr.p_double[sfy+s4]/du-2*c->f.ptr.p_double[sfxy+s1]/(dt*du)-2*c->f.ptr.p_double[sfxy+s2]/(dt*du)-c->f.ptr.p_double[sfxy+s3]/(dt*du)-c->f.ptr.p_double[sfxy+s4]/(dt*du);
        *f = *f+v*t3*u2;
        *fx = *fx+3*v*t2*u2*dt;
        *fy = *fy+2*v*t3*u1*du;
        *fxy = *fxy+6*v*t2*u1*dt*du;
        v = 4*c->f.ptr.p_double[s1]-4*c->f.ptr.p_double[s2]+4*c->f.ptr.p_double[s3]-4*c->f.ptr.p_double[s4]+2*c->f.ptr.p_double[sfx+s1]/dt+2*c->f.ptr.p_double[sfx+s2]/dt-2*c->f.ptr.p_double[sfx+s3]/dt-2*c->f.ptr.p_double[sfx+s4]/dt+2*c->f.ptr.p_double[sfy+s1]/du-2*c->f.ptr.p_double[sfy+s2]/du-2*c->f.ptr.p_double[sfy+s3]/du+2*c->f.ptr.p_double[sfy+s4]/du+c->f.ptr.p_double[sfxy+s1]/(dt*du)+c->f.ptr.p_double[sfxy+s2]/(dt*du)+c->f.ptr.p_double[sfxy+s3]/(dt*du)+c->f.ptr.p_double[sfxy+s4]/(dt*du);
        *f = *f+v*t3*u3;
        *fx = *fx+3*v*t2*u3*dt;
        *fy = *fy+3*v*t3*u2*du;
        *fxy = *fxy+9*v*t2*u2*dt*du;
        return;
    }
}


/*************************************************************************
This subroutine performs linear transformation of the spline argument.

Input parameters:
    C       -   spline interpolant
    AX, BX  -   transformation coefficients: x = A*t + B
    AY, BY  -   transformation coefficients: y = A*u + B
Result:
    C   -   transformed spline

  -- ALGLIB PROJECT --
     Copyright 30.06.2007 by Bochkanov Sergey
*************************************************************************/
void spline2dlintransxy(spline2dinterpolant* c,
     double ax,
     double bx,
     double ay,
     double by,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    ae_vector y;
    ae_vector f;
    ae_vector v;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&f, 0, DT_REAL, _state);
    ae_vector_init(&v, 0, DT_REAL, _state);

    ae_assert(c->stype==-3||c->stype==-1, "Spline2DLinTransXY: incorrect C (incorrect parameter C.SType)", _state);
    ae_assert(ae_isfinite(ax, _state), "Spline2DLinTransXY: AX is infinite or NaN", _state);
    ae_assert(ae_isfinite(bx, _state), "Spline2DLinTransXY: BX is infinite or NaN", _state);
    ae_assert(ae_isfinite(ay, _state), "Spline2DLinTransXY: AY is infinite or NaN", _state);
    ae_assert(ae_isfinite(by, _state), "Spline2DLinTransXY: BY is infinite or NaN", _state);
    ae_vector_set_length(&x, c->n, _state);
    ae_vector_set_length(&y, c->m, _state);
    ae_vector_set_length(&f, c->m*c->n*c->d, _state);
    for(j=0; j<=c->n-1; j++)
    {
        x.ptr.p_double[j] = c->x.ptr.p_double[j];
    }
    for(i=0; i<=c->m-1; i++)
    {
        y.ptr.p_double[i] = c->y.ptr.p_double[i];
    }
    for(i=0; i<=c->m-1; i++)
    {
        for(j=0; j<=c->n-1; j++)
        {
            for(k=0; k<=c->d-1; k++)
            {
                f.ptr.p_double[c->d*(i*c->n+j)+k] = c->f.ptr.p_double[c->d*(i*c->n+j)+k];
            }
        }
    }
    
    /*
     * Handle different combinations of AX/AY
     */
    if( ae_fp_eq(ax,(double)(0))&&ae_fp_neq(ay,(double)(0)) )
    {
        for(i=0; i<=c->m-1; i++)
        {
            spline2dcalcvbuf(c, bx, y.ptr.p_double[i], &v, _state);
            y.ptr.p_double[i] = (y.ptr.p_double[i]-by)/ay;
            for(j=0; j<=c->n-1; j++)
            {
                for(k=0; k<=c->d-1; k++)
                {
                    f.ptr.p_double[c->d*(i*c->n+j)+k] = v.ptr.p_double[k];
                }
            }
        }
    }
    if( ae_fp_neq(ax,(double)(0))&&ae_fp_eq(ay,(double)(0)) )
    {
        for(j=0; j<=c->n-1; j++)
        {
            spline2dcalcvbuf(c, x.ptr.p_double[j], by, &v, _state);
            x.ptr.p_double[j] = (x.ptr.p_double[j]-bx)/ax;
            for(i=0; i<=c->m-1; i++)
            {
                for(k=0; k<=c->d-1; k++)
                {
                    f.ptr.p_double[c->d*(i*c->n+j)+k] = v.ptr.p_double[k];
                }
            }
        }
    }
    if( ae_fp_neq(ax,(double)(0))&&ae_fp_neq(ay,(double)(0)) )
    {
        for(j=0; j<=c->n-1; j++)
        {
            x.ptr.p_double[j] = (x.ptr.p_double[j]-bx)/ax;
        }
        for(i=0; i<=c->m-1; i++)
        {
            y.ptr.p_double[i] = (y.ptr.p_double[i]-by)/ay;
        }
    }
    if( ae_fp_eq(ax,(double)(0))&&ae_fp_eq(ay,(double)(0)) )
    {
        spline2dcalcvbuf(c, bx, by, &v, _state);
        for(i=0; i<=c->m-1; i++)
        {
            for(j=0; j<=c->n-1; j++)
            {
                for(k=0; k<=c->d-1; k++)
                {
                    f.ptr.p_double[c->d*(i*c->n+j)+k] = v.ptr.p_double[k];
                }
            }
        }
    }
    
    /*
     * Rebuild spline
     */
    if( c->stype==-3 )
    {
        spline2dbuildbicubicv(&x, c->n, &y, c->m, &f, c->d, c, _state);
    }
    if( c->stype==-1 )
    {
        spline2dbuildbilinearv(&x, c->n, &y, c->m, &f, c->d, c, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This subroutine performs linear transformation of the spline.

Input parameters:
    C   -   spline interpolant.
    A, B-   transformation coefficients: S2(x,y) = A*S(x,y) + B
    
Output parameters:
    C   -   transformed spline

  -- ALGLIB PROJECT --
     Copyright 30.06.2007 by Bochkanov Sergey
*************************************************************************/
void spline2dlintransf(spline2dinterpolant* c,
     double a,
     double b,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    ae_vector y;
    ae_vector f;
    ae_int_t i;
    ae_int_t j;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&f, 0, DT_REAL, _state);

    ae_assert(c->stype==-3||c->stype==-1, "Spline2DLinTransF: incorrect C (incorrect parameter C.SType)", _state);
    ae_vector_set_length(&x, c->n, _state);
    ae_vector_set_length(&y, c->m, _state);
    ae_vector_set_length(&f, c->m*c->n*c->d, _state);
    for(j=0; j<=c->n-1; j++)
    {
        x.ptr.p_double[j] = c->x.ptr.p_double[j];
    }
    for(i=0; i<=c->m-1; i++)
    {
        y.ptr.p_double[i] = c->y.ptr.p_double[i];
    }
    for(i=0; i<=c->m*c->n*c->d-1; i++)
    {
        f.ptr.p_double[i] = a*c->f.ptr.p_double[i]+b;
    }
    if( c->stype==-3 )
    {
        spline2dbuildbicubicv(&x, c->n, &y, c->m, &f, c->d, c, _state);
    }
    if( c->stype==-1 )
    {
        spline2dbuildbilinearv(&x, c->n, &y, c->m, &f, c->d, c, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This subroutine makes the copy of the spline model.

Input parameters:
    C   -   spline interpolant

Output parameters:
    CC  -   spline copy

  -- ALGLIB PROJECT --
     Copyright 29.06.2007 by Bochkanov Sergey
*************************************************************************/
void spline2dcopy(spline2dinterpolant* c,
     spline2dinterpolant* cc,
     ae_state *_state)
{
    ae_int_t tblsize;

    _spline2dinterpolant_clear(cc);

    ae_assert(c->k==1||c->k==3, "Spline2DCopy: incorrect C (incorrect parameter C.K)", _state);
    cc->k = c->k;
    cc->n = c->n;
    cc->m = c->m;
    cc->d = c->d;
    cc->stype = c->stype;
    tblsize = -1;
    if( c->stype==-3 )
    {
        tblsize = 4*c->n*c->m*c->d;
    }
    if( c->stype==-1 )
    {
        tblsize = c->n*c->m*c->d;
    }
    ae_assert(tblsize>0, "Spline2DCopy: internal error", _state);
    ae_vector_set_length(&cc->x, cc->n, _state);
    ae_vector_set_length(&cc->y, cc->m, _state);
    ae_vector_set_length(&cc->f, tblsize, _state);
    ae_v_move(&cc->x.ptr.p_double[0], 1, &c->x.ptr.p_double[0], 1, ae_v_len(0,cc->n-1));
    ae_v_move(&cc->y.ptr.p_double[0], 1, &c->y.ptr.p_double[0], 1, ae_v_len(0,cc->m-1));
    ae_v_move(&cc->f.ptr.p_double[0], 1, &c->f.ptr.p_double[0], 1, ae_v_len(0,tblsize-1));
}


/*************************************************************************
Bicubic spline resampling

Input parameters:
    A           -   function values at the old grid,
                    array[0..OldHeight-1, 0..OldWidth-1]
    OldHeight   -   old grid height, OldHeight>1
    OldWidth    -   old grid width, OldWidth>1
    NewHeight   -   new grid height, NewHeight>1
    NewWidth    -   new grid width, NewWidth>1
    
Output parameters:
    B           -   function values at the new grid,
                    array[0..NewHeight-1, 0..NewWidth-1]

  -- ALGLIB routine --
     15 May, 2007
     Copyright by Bochkanov Sergey
*************************************************************************/
void spline2dresamplebicubic(/* Real    */ ae_matrix* a,
     ae_int_t oldheight,
     ae_int_t oldwidth,
     /* Real    */ ae_matrix* b,
     ae_int_t newheight,
     ae_int_t newwidth,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix buf;
    ae_vector x;
    ae_vector y;
    spline1dinterpolant c;
    ae_int_t mw;
    ae_int_t mh;
    ae_int_t i;
    ae_int_t j;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(b);
    ae_matrix_init(&buf, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    _spline1dinterpolant_init(&c, _state);

    ae_assert(oldwidth>1&&oldheight>1, "Spline2DResampleBicubic: width/height less than 1", _state);
    ae_assert(newwidth>1&&newheight>1, "Spline2DResampleBicubic: width/height less than 1", _state);
    
    /*
     * Prepare
     */
    mw = ae_maxint(oldwidth, newwidth, _state);
    mh = ae_maxint(oldheight, newheight, _state);
    ae_matrix_set_length(b, newheight, newwidth, _state);
    ae_matrix_set_length(&buf, oldheight, newwidth, _state);
    ae_vector_set_length(&x, ae_maxint(mw, mh, _state), _state);
    ae_vector_set_length(&y, ae_maxint(mw, mh, _state), _state);
    
    /*
     * Horizontal interpolation
     */
    for(i=0; i<=oldheight-1; i++)
    {
        
        /*
         * Fill X, Y
         */
        for(j=0; j<=oldwidth-1; j++)
        {
            x.ptr.p_double[j] = (double)j/(double)(oldwidth-1);
            y.ptr.p_double[j] = a->ptr.pp_double[i][j];
        }
        
        /*
         * Interpolate and place result into temporary matrix
         */
        spline1dbuildcubic(&x, &y, oldwidth, 0, 0.0, 0, 0.0, &c, _state);
        for(j=0; j<=newwidth-1; j++)
        {
            buf.ptr.pp_double[i][j] = spline1dcalc(&c, (double)j/(double)(newwidth-1), _state);
        }
    }
    
    /*
     * Vertical interpolation
     */
    for(j=0; j<=newwidth-1; j++)
    {
        
        /*
         * Fill X, Y
         */
        for(i=0; i<=oldheight-1; i++)
        {
            x.ptr.p_double[i] = (double)i/(double)(oldheight-1);
            y.ptr.p_double[i] = buf.ptr.pp_double[i][j];
        }
        
        /*
         * Interpolate and place result into B
         */
        spline1dbuildcubic(&x, &y, oldheight, 0, 0.0, 0, 0.0, &c, _state);
        for(i=0; i<=newheight-1; i++)
        {
            b->ptr.pp_double[i][j] = spline1dcalc(&c, (double)i/(double)(newheight-1), _state);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Bilinear spline resampling

Input parameters:
    A           -   function values at the old grid,
                    array[0..OldHeight-1, 0..OldWidth-1]
    OldHeight   -   old grid height, OldHeight>1
    OldWidth    -   old grid width, OldWidth>1
    NewHeight   -   new grid height, NewHeight>1
    NewWidth    -   new grid width, NewWidth>1

Output parameters:
    B           -   function values at the new grid,
                    array[0..NewHeight-1, 0..NewWidth-1]

  -- ALGLIB routine --
     09.07.2007
     Copyright by Bochkanov Sergey
*************************************************************************/
void spline2dresamplebilinear(/* Real    */ ae_matrix* a,
     ae_int_t oldheight,
     ae_int_t oldwidth,
     /* Real    */ ae_matrix* b,
     ae_int_t newheight,
     ae_int_t newwidth,
     ae_state *_state)
{
    ae_int_t l;
    ae_int_t c;
    double t;
    double u;
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(b);

    ae_assert(oldwidth>1&&oldheight>1, "Spline2DResampleBilinear: width/height less than 1", _state);
    ae_assert(newwidth>1&&newheight>1, "Spline2DResampleBilinear: width/height less than 1", _state);
    ae_matrix_set_length(b, newheight, newwidth, _state);
    for(i=0; i<=newheight-1; i++)
    {
        for(j=0; j<=newwidth-1; j++)
        {
            l = i*(oldheight-1)/(newheight-1);
            if( l==oldheight-1 )
            {
                l = oldheight-2;
            }
            u = (double)i/(double)(newheight-1)*(oldheight-1)-l;
            c = j*(oldwidth-1)/(newwidth-1);
            if( c==oldwidth-1 )
            {
                c = oldwidth-2;
            }
            t = (double)(j*(oldwidth-1))/(double)(newwidth-1)-c;
            b->ptr.pp_double[i][j] = (1-t)*(1-u)*a->ptr.pp_double[l][c]+t*(1-u)*a->ptr.pp_double[l][c+1]+t*u*a->ptr.pp_double[l+1][c+1]+(1-t)*u*a->ptr.pp_double[l+1][c];
        }
    }
}


/*************************************************************************
This subroutine builds bilinear vector-valued spline.

Input parameters:
    X   -   spline abscissas, array[0..N-1]
    Y   -   spline ordinates, array[0..M-1]
    F   -   function values, array[0..M*N*D-1]:
            * first D elements store D values at (X[0],Y[0])
            * next D elements store D values at (X[1],Y[0])
            * general form - D function values at (X[i],Y[j]) are stored
              at F[D*(J*N+I)...D*(J*N+I)+D-1].
    M,N -   grid size, M>=2, N>=2
    D   -   vector dimension, D>=1

Output parameters:
    C   -   spline interpolant

  -- ALGLIB PROJECT --
     Copyright 16.04.2012 by Bochkanov Sergey
*************************************************************************/
void spline2dbuildbilinearv(/* Real    */ ae_vector* x,
     ae_int_t n,
     /* Real    */ ae_vector* y,
     ae_int_t m,
     /* Real    */ ae_vector* f,
     ae_int_t d,
     spline2dinterpolant* c,
     ae_state *_state)
{
    double t;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t i0;

    _spline2dinterpolant_clear(c);

    ae_assert(n>=2, "Spline2DBuildBilinearV: N is less then 2", _state);
    ae_assert(m>=2, "Spline2DBuildBilinearV: M is less then 2", _state);
    ae_assert(d>=1, "Spline2DBuildBilinearV: invalid argument D (D<1)", _state);
    ae_assert(x->cnt>=n&&y->cnt>=m, "Spline2DBuildBilinearV: length of X or Y is too short (Length(X/Y)<N/M)", _state);
    ae_assert(isfinitevector(x, n, _state)&&isfinitevector(y, m, _state), "Spline2DBuildBilinearV: X or Y contains NaN or Infinite value", _state);
    k = n*m*d;
    ae_assert(f->cnt>=k, "Spline2DBuildBilinearV: length of F is too short (Length(F)<N*M*D)", _state);
    ae_assert(isfinitevector(f, k, _state), "Spline2DBuildBilinearV: F contains NaN or Infinite value", _state);
    
    /*
     * Fill interpolant
     */
    c->k = 1;
    c->n = n;
    c->m = m;
    c->d = d;
    c->stype = -1;
    ae_vector_set_length(&c->x, c->n, _state);
    ae_vector_set_length(&c->y, c->m, _state);
    ae_vector_set_length(&c->f, k, _state);
    for(i=0; i<=c->n-1; i++)
    {
        c->x.ptr.p_double[i] = x->ptr.p_double[i];
    }
    for(i=0; i<=c->m-1; i++)
    {
        c->y.ptr.p_double[i] = y->ptr.p_double[i];
    }
    for(i=0; i<=k-1; i++)
    {
        c->f.ptr.p_double[i] = f->ptr.p_double[i];
    }
    
    /*
     * Sort points
     */
    for(j=0; j<=c->n-1; j++)
    {
        k = j;
        for(i=j+1; i<=c->n-1; i++)
        {
            if( ae_fp_less(c->x.ptr.p_double[i],c->x.ptr.p_double[k]) )
            {
                k = i;
            }
        }
        if( k!=j )
        {
            for(i=0; i<=c->m-1; i++)
            {
                for(i0=0; i0<=c->d-1; i0++)
                {
                    t = c->f.ptr.p_double[c->d*(i*c->n+j)+i0];
                    c->f.ptr.p_double[c->d*(i*c->n+j)+i0] = c->f.ptr.p_double[c->d*(i*c->n+k)+i0];
                    c->f.ptr.p_double[c->d*(i*c->n+k)+i0] = t;
                }
            }
            t = c->x.ptr.p_double[j];
            c->x.ptr.p_double[j] = c->x.ptr.p_double[k];
            c->x.ptr.p_double[k] = t;
        }
    }
    for(i=0; i<=c->m-1; i++)
    {
        k = i;
        for(j=i+1; j<=c->m-1; j++)
        {
            if( ae_fp_less(c->y.ptr.p_double[j],c->y.ptr.p_double[k]) )
            {
                k = j;
            }
        }
        if( k!=i )
        {
            for(j=0; j<=c->n-1; j++)
            {
                for(i0=0; i0<=c->d-1; i0++)
                {
                    t = c->f.ptr.p_double[c->d*(i*c->n+j)+i0];
                    c->f.ptr.p_double[c->d*(i*c->n+j)+i0] = c->f.ptr.p_double[c->d*(k*c->n+j)+i0];
                    c->f.ptr.p_double[c->d*(k*c->n+j)+i0] = t;
                }
            }
            t = c->y.ptr.p_double[i];
            c->y.ptr.p_double[i] = c->y.ptr.p_double[k];
            c->y.ptr.p_double[k] = t;
        }
    }
}


/*************************************************************************
This subroutine builds bicubic vector-valued spline.

Input parameters:
    X   -   spline abscissas, array[0..N-1]
    Y   -   spline ordinates, array[0..M-1]
    F   -   function values, array[0..M*N*D-1]:
            * first D elements store D values at (X[0],Y[0])
            * next D elements store D values at (X[1],Y[0])
            * general form - D function values at (X[i],Y[j]) are stored
              at F[D*(J*N+I)...D*(J*N+I)+D-1].
    M,N -   grid size, M>=2, N>=2
    D   -   vector dimension, D>=1

Output parameters:
    C   -   spline interpolant

  -- ALGLIB PROJECT --
     Copyright 16.04.2012 by Bochkanov Sergey
*************************************************************************/
void spline2dbuildbicubicv(/* Real    */ ae_vector* x,
     ae_int_t n,
     /* Real    */ ae_vector* y,
     ae_int_t m,
     /* Real    */ ae_vector* f,
     ae_int_t d,
     spline2dinterpolant* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _f;
    ae_matrix tf;
    ae_matrix dx;
    ae_matrix dy;
    ae_matrix dxy;
    double t;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t di;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_f, f, _state);
    f = &_f;
    _spline2dinterpolant_clear(c);
    ae_matrix_init(&tf, 0, 0, DT_REAL, _state);
    ae_matrix_init(&dx, 0, 0, DT_REAL, _state);
    ae_matrix_init(&dy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&dxy, 0, 0, DT_REAL, _state);

    ae_assert(n>=2, "Spline2DBuildBicubicV: N is less than 2", _state);
    ae_assert(m>=2, "Spline2DBuildBicubicV: M is less than 2", _state);
    ae_assert(d>=1, "Spline2DBuildBicubicV: invalid argument D (D<1)", _state);
    ae_assert(x->cnt>=n&&y->cnt>=m, "Spline2DBuildBicubicV: length of X or Y is too short (Length(X/Y)<N/M)", _state);
    ae_assert(isfinitevector(x, n, _state)&&isfinitevector(y, m, _state), "Spline2DBuildBicubicV: X or Y contains NaN or Infinite value", _state);
    k = n*m*d;
    ae_assert(f->cnt>=k, "Spline2DBuildBicubicV: length of F is too short (Length(F)<N*M*D)", _state);
    ae_assert(isfinitevector(f, k, _state), "Spline2DBuildBicubicV: F contains NaN or Infinite value", _state);
    
    /*
     * Fill interpolant:
     *  F[0]...F[N*M*D-1]:
     *      f(i,j) table. f(0,0), f(0, 1), f(0,2) and so on...
     *  F[N*M*D]...F[2*N*M*D-1]:
     *      df(i,j)/dx table.
     *  F[2*N*M*D]...F[3*N*M*D-1]:
     *      df(i,j)/dy table.
     *  F[3*N*M*D]...F[4*N*M*D-1]:
     *      d2f(i,j)/dxdy table.
     */
    c->k = 3;
    c->d = d;
    c->n = n;
    c->m = m;
    c->stype = -3;
    k = 4*k;
    ae_vector_set_length(&c->x, c->n, _state);
    ae_vector_set_length(&c->y, c->m, _state);
    ae_vector_set_length(&c->f, k, _state);
    ae_matrix_set_length(&tf, c->m, c->n, _state);
    for(i=0; i<=c->n-1; i++)
    {
        c->x.ptr.p_double[i] = x->ptr.p_double[i];
    }
    for(i=0; i<=c->m-1; i++)
    {
        c->y.ptr.p_double[i] = y->ptr.p_double[i];
    }
    
    /*
     * Sort points
     */
    for(j=0; j<=c->n-1; j++)
    {
        k = j;
        for(i=j+1; i<=c->n-1; i++)
        {
            if( ae_fp_less(c->x.ptr.p_double[i],c->x.ptr.p_double[k]) )
            {
                k = i;
            }
        }
        if( k!=j )
        {
            for(i=0; i<=c->m-1; i++)
            {
                for(di=0; di<=c->d-1; di++)
                {
                    t = f->ptr.p_double[c->d*(i*c->n+j)+di];
                    f->ptr.p_double[c->d*(i*c->n+j)+di] = f->ptr.p_double[c->d*(i*c->n+k)+di];
                    f->ptr.p_double[c->d*(i*c->n+k)+di] = t;
                }
            }
            t = c->x.ptr.p_double[j];
            c->x.ptr.p_double[j] = c->x.ptr.p_double[k];
            c->x.ptr.p_double[k] = t;
        }
    }
    for(i=0; i<=c->m-1; i++)
    {
        k = i;
        for(j=i+1; j<=c->m-1; j++)
        {
            if( ae_fp_less(c->y.ptr.p_double[j],c->y.ptr.p_double[k]) )
            {
                k = j;
            }
        }
        if( k!=i )
        {
            for(j=0; j<=c->n-1; j++)
            {
                for(di=0; di<=c->d-1; di++)
                {
                    t = f->ptr.p_double[c->d*(i*c->n+j)+di];
                    f->ptr.p_double[c->d*(i*c->n+j)+di] = f->ptr.p_double[c->d*(k*c->n+j)+di];
                    f->ptr.p_double[c->d*(k*c->n+j)+di] = t;
                }
            }
            t = c->y.ptr.p_double[i];
            c->y.ptr.p_double[i] = c->y.ptr.p_double[k];
            c->y.ptr.p_double[k] = t;
        }
    }
    for(di=0; di<=c->d-1; di++)
    {
        for(i=0; i<=c->m-1; i++)
        {
            for(j=0; j<=c->n-1; j++)
            {
                tf.ptr.pp_double[i][j] = f->ptr.p_double[c->d*(i*c->n+j)+di];
            }
        }
        spline2d_bicubiccalcderivatives(&tf, &c->x, &c->y, c->m, c->n, &dx, &dy, &dxy, _state);
        for(i=0; i<=c->m-1; i++)
        {
            for(j=0; j<=c->n-1; j++)
            {
                k = c->d*(i*c->n+j)+di;
                c->f.ptr.p_double[k] = tf.ptr.pp_double[i][j];
                c->f.ptr.p_double[c->n*c->m*c->d+k] = dx.ptr.pp_double[i][j];
                c->f.ptr.p_double[2*c->n*c->m*c->d+k] = dy.ptr.pp_double[i][j];
                c->f.ptr.p_double[3*c->n*c->m*c->d+k] = dxy.ptr.pp_double[i][j];
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This subroutine calculates bilinear or bicubic vector-valued spline at the
given point (X,Y).

INPUT PARAMETERS:
    C   -   spline interpolant.
    X, Y-   point
    F   -   output buffer, possibly preallocated array. In case array size
            is large enough to store result, it is not reallocated.  Array
            which is too short will be reallocated

OUTPUT PARAMETERS:
    F   -   array[D] (or larger) which stores function values

  -- ALGLIB PROJECT --
     Copyright 16.04.2012 by Bochkanov Sergey
*************************************************************************/
void spline2dcalcvbuf(spline2dinterpolant* c,
     double x,
     double y,
     /* Real    */ ae_vector* f,
     ae_state *_state)
{
    double t;
    double dt;
    double u;
    double du;
    ae_int_t ix;
    ae_int_t iy;
    ae_int_t l;
    ae_int_t r;
    ae_int_t h;
    ae_int_t s1;
    ae_int_t s2;
    ae_int_t s3;
    ae_int_t s4;
    ae_int_t sfx;
    ae_int_t sfy;
    ae_int_t sfxy;
    double y1;
    double y2;
    double y3;
    double y4;
    double v;
    double t0;
    double t1;
    double t2;
    double t3;
    double u0;
    double u1;
    double u2;
    double u3;
    ae_int_t i;


    ae_assert(c->stype==-1||c->stype==-3, "Spline2DCalcVBuf: incorrect C (incorrect parameter C.SType)", _state);
    ae_assert(ae_isfinite(x, _state)&&ae_isfinite(y, _state), "Spline2DCalcVBuf: either X=NaN/Infinite or Y=NaN/Infinite", _state);
    rvectorsetlengthatleast(f, c->d, _state);
    
    /*
     * Binary search in the [ x[0], ..., x[n-2] ] (x[n-1] is not included)
     */
    l = 0;
    r = c->n-1;
    while(l!=r-1)
    {
        h = (l+r)/2;
        if( ae_fp_greater_eq(c->x.ptr.p_double[h],x) )
        {
            r = h;
        }
        else
        {
            l = h;
        }
    }
    t = (x-c->x.ptr.p_double[l])/(c->x.ptr.p_double[l+1]-c->x.ptr.p_double[l]);
    dt = 1.0/(c->x.ptr.p_double[l+1]-c->x.ptr.p_double[l]);
    ix = l;
    
    /*
     * Binary search in the [ y[0], ..., y[m-2] ] (y[m-1] is not included)
     */
    l = 0;
    r = c->m-1;
    while(l!=r-1)
    {
        h = (l+r)/2;
        if( ae_fp_greater_eq(c->y.ptr.p_double[h],y) )
        {
            r = h;
        }
        else
        {
            l = h;
        }
    }
    u = (y-c->y.ptr.p_double[l])/(c->y.ptr.p_double[l+1]-c->y.ptr.p_double[l]);
    du = 1.0/(c->y.ptr.p_double[l+1]-c->y.ptr.p_double[l]);
    iy = l;
    
    /*
     * Bilinear interpolation
     */
    if( c->stype==-1 )
    {
        for(i=0; i<=c->d-1; i++)
        {
            y1 = c->f.ptr.p_double[c->d*(c->n*iy+ix)+i];
            y2 = c->f.ptr.p_double[c->d*(c->n*iy+(ix+1))+i];
            y3 = c->f.ptr.p_double[c->d*(c->n*(iy+1)+(ix+1))+i];
            y4 = c->f.ptr.p_double[c->d*(c->n*(iy+1)+ix)+i];
            f->ptr.p_double[i] = (1-t)*(1-u)*y1+t*(1-u)*y2+t*u*y3+(1-t)*u*y4;
        }
        return;
    }
    
    /*
     * Bicubic interpolation
     */
    if( c->stype==-3 )
    {
        
        /*
         * Prepare info
         */
        t0 = (double)(1);
        t1 = t;
        t2 = ae_sqr(t, _state);
        t3 = t*t2;
        u0 = (double)(1);
        u1 = u;
        u2 = ae_sqr(u, _state);
        u3 = u*u2;
        sfx = c->n*c->m*c->d;
        sfy = 2*c->n*c->m*c->d;
        sfxy = 3*c->n*c->m*c->d;
        for(i=0; i<=c->d-1; i++)
        {
            
            /*
             * Prepare F, dF/dX, dF/dY, d2F/dXdY
             */
            f->ptr.p_double[i] = (double)(0);
            s1 = c->d*(c->n*iy+ix)+i;
            s2 = c->d*(c->n*iy+(ix+1))+i;
            s3 = c->d*(c->n*(iy+1)+(ix+1))+i;
            s4 = c->d*(c->n*(iy+1)+ix)+i;
            
            /*
             * Calculate
             */
            v = c->f.ptr.p_double[s1];
            f->ptr.p_double[i] = f->ptr.p_double[i]+v*t0*u0;
            v = c->f.ptr.p_double[sfy+s1]/du;
            f->ptr.p_double[i] = f->ptr.p_double[i]+v*t0*u1;
            v = -3*c->f.ptr.p_double[s1]+3*c->f.ptr.p_double[s4]-2*c->f.ptr.p_double[sfy+s1]/du-c->f.ptr.p_double[sfy+s4]/du;
            f->ptr.p_double[i] = f->ptr.p_double[i]+v*t0*u2;
            v = 2*c->f.ptr.p_double[s1]-2*c->f.ptr.p_double[s4]+c->f.ptr.p_double[sfy+s1]/du+c->f.ptr.p_double[sfy+s4]/du;
            f->ptr.p_double[i] = f->ptr.p_double[i]+v*t0*u3;
            v = c->f.ptr.p_double[sfx+s1]/dt;
            f->ptr.p_double[i] = f->ptr.p_double[i]+v*t1*u0;
            v = c->f.ptr.p_double[sfxy+s1]/(dt*du);
            f->ptr.p_double[i] = f->ptr.p_double[i]+v*t1*u1;
            v = -3*c->f.ptr.p_double[sfx+s1]/dt+3*c->f.ptr.p_double[sfx+s4]/dt-2*c->f.ptr.p_double[sfxy+s1]/(dt*du)-c->f.ptr.p_double[sfxy+s4]/(dt*du);
            f->ptr.p_double[i] = f->ptr.p_double[i]+v*t1*u2;
            v = 2*c->f.ptr.p_double[sfx+s1]/dt-2*c->f.ptr.p_double[sfx+s4]/dt+c->f.ptr.p_double[sfxy+s1]/(dt*du)+c->f.ptr.p_double[sfxy+s4]/(dt*du);
            f->ptr.p_double[i] = f->ptr.p_double[i]+v*t1*u3;
            v = -3*c->f.ptr.p_double[s1]+3*c->f.ptr.p_double[s2]-2*c->f.ptr.p_double[sfx+s1]/dt-c->f.ptr.p_double[sfx+s2]/dt;
            f->ptr.p_double[i] = f->ptr.p_double[i]+v*t2*u0;
            v = -3*c->f.ptr.p_double[sfy+s1]/du+3*c->f.ptr.p_double[sfy+s2]/du-2*c->f.ptr.p_double[sfxy+s1]/(dt*du)-c->f.ptr.p_double[sfxy+s2]/(dt*du);
            f->ptr.p_double[i] = f->ptr.p_double[i]+v*t2*u1;
            v = 9*c->f.ptr.p_double[s1]-9*c->f.ptr.p_double[s2]+9*c->f.ptr.p_double[s3]-9*c->f.ptr.p_double[s4]+6*c->f.ptr.p_double[sfx+s1]/dt+3*c->f.ptr.p_double[sfx+s2]/dt-3*c->f.ptr.p_double[sfx+s3]/dt-6*c->f.ptr.p_double[sfx+s4]/dt+6*c->f.ptr.p_double[sfy+s1]/du-6*c->f.ptr.p_double[sfy+s2]/du-3*c->f.ptr.p_double[sfy+s3]/du+3*c->f.ptr.p_double[sfy+s4]/du+4*c->f.ptr.p_double[sfxy+s1]/(dt*du)+2*c->f.ptr.p_double[sfxy+s2]/(dt*du)+c->f.ptr.p_double[sfxy+s3]/(dt*du)+2*c->f.ptr.p_double[sfxy+s4]/(dt*du);
            f->ptr.p_double[i] = f->ptr.p_double[i]+v*t2*u2;
            v = -6*c->f.ptr.p_double[s1]+6*c->f.ptr.p_double[s2]-6*c->f.ptr.p_double[s3]+6*c->f.ptr.p_double[s4]-4*c->f.ptr.p_double[sfx+s1]/dt-2*c->f.ptr.p_double[sfx+s2]/dt+2*c->f.ptr.p_double[sfx+s3]/dt+4*c->f.ptr.p_double[sfx+s4]/dt-3*c->f.ptr.p_double[sfy+s1]/du+3*c->f.ptr.p_double[sfy+s2]/du+3*c->f.ptr.p_double[sfy+s3]/du-3*c->f.ptr.p_double[sfy+s4]/du-2*c->f.ptr.p_double[sfxy+s1]/(dt*du)-c->f.ptr.p_double[sfxy+s2]/(dt*du)-c->f.ptr.p_double[sfxy+s3]/(dt*du)-2*c->f.ptr.p_double[sfxy+s4]/(dt*du);
            f->ptr.p_double[i] = f->ptr.p_double[i]+v*t2*u3;
            v = 2*c->f.ptr.p_double[s1]-2*c->f.ptr.p_double[s2]+c->f.ptr.p_double[sfx+s1]/dt+c->f.ptr.p_double[sfx+s2]/dt;
            f->ptr.p_double[i] = f->ptr.p_double[i]+v*t3*u0;
            v = 2*c->f.ptr.p_double[sfy+s1]/du-2*c->f.ptr.p_double[sfy+s2]/du+c->f.ptr.p_double[sfxy+s1]/(dt*du)+c->f.ptr.p_double[sfxy+s2]/(dt*du);
            f->ptr.p_double[i] = f->ptr.p_double[i]+v*t3*u1;
            v = -6*c->f.ptr.p_double[s1]+6*c->f.ptr.p_double[s2]-6*c->f.ptr.p_double[s3]+6*c->f.ptr.p_double[s4]-3*c->f.ptr.p_double[sfx+s1]/dt-3*c->f.ptr.p_double[sfx+s2]/dt+3*c->f.ptr.p_double[sfx+s3]/dt+3*c->f.ptr.p_double[sfx+s4]/dt-4*c->f.ptr.p_double[sfy+s1]/du+4*c->f.ptr.p_double[sfy+s2]/du+2*c->f.ptr.p_double[sfy+s3]/du-2*c->f.ptr.p_double[sfy+s4]/du-2*c->f.ptr.p_double[sfxy+s1]/(dt*du)-2*c->f.ptr.p_double[sfxy+s2]/(dt*du)-c->f.ptr.p_double[sfxy+s3]/(dt*du)-c->f.ptr.p_double[sfxy+s4]/(dt*du);
            f->ptr.p_double[i] = f->ptr.p_double[i]+v*t3*u2;
            v = 4*c->f.ptr.p_double[s1]-4*c->f.ptr.p_double[s2]+4*c->f.ptr.p_double[s3]-4*c->f.ptr.p_double[s4]+2*c->f.ptr.p_double[sfx+s1]/dt+2*c->f.ptr.p_double[sfx+s2]/dt-2*c->f.ptr.p_double[sfx+s3]/dt-2*c->f.ptr.p_double[sfx+s4]/dt+2*c->f.ptr.p_double[sfy+s1]/du-2*c->f.ptr.p_double[sfy+s2]/du-2*c->f.ptr.p_double[sfy+s3]/du+2*c->f.ptr.p_double[sfy+s4]/du+c->f.ptr.p_double[sfxy+s1]/(dt*du)+c->f.ptr.p_double[sfxy+s2]/(dt*du)+c->f.ptr.p_double[sfxy+s3]/(dt*du)+c->f.ptr.p_double[sfxy+s4]/(dt*du);
            f->ptr.p_double[i] = f->ptr.p_double[i]+v*t3*u3;
        }
        return;
    }
}


/*************************************************************************
This subroutine calculates bilinear or bicubic vector-valued spline at the
given point (X,Y).

INPUT PARAMETERS:
    C   -   spline interpolant.
    X, Y-   point

OUTPUT PARAMETERS:
    F   -   array[D] which stores function values.  F is out-parameter and
            it  is  reallocated  after  call to this function. In case you
            want  to    reuse  previously  allocated  F,   you   may   use
            Spline2DCalcVBuf(),  which  reallocates  F only when it is too
            small.

  -- ALGLIB PROJECT --
     Copyright 16.04.2012 by Bochkanov Sergey
*************************************************************************/
void spline2dcalcv(spline2dinterpolant* c,
     double x,
     double y,
     /* Real    */ ae_vector* f,
     ae_state *_state)
{

    ae_vector_clear(f);

    ae_assert(c->stype==-1||c->stype==-3, "Spline2DCalcV: incorrect C (incorrect parameter C.SType)", _state);
    ae_assert(ae_isfinite(x, _state)&&ae_isfinite(y, _state), "Spline2DCalcV: either X=NaN/Infinite or Y=NaN/Infinite", _state);
    ae_vector_set_length(f, c->d, _state);
    spline2dcalcvbuf(c, x, y, f, _state);
}


/*************************************************************************
This subroutine unpacks two-dimensional spline into the coefficients table

Input parameters:
    C   -   spline interpolant.

Result:
    M, N-   grid size (x-axis and y-axis)
    D   -   number of components
    Tbl -   coefficients table, unpacked format,
            D - components: [0..(N-1)*(M-1)*D-1, 0..19].
            For T=0..D-1 (component index), I = 0...N-2 (x index),
            J=0..M-2 (y index):
                K :=  T + I*D + J*D*(N-1)
                
                K-th row stores decomposition for T-th component of the
                vector-valued function
                
                Tbl[K,0] = X[i]
                Tbl[K,1] = X[i+1]
                Tbl[K,2] = Y[j]
                Tbl[K,3] = Y[j+1]
                Tbl[K,4] = C00
                Tbl[K,5] = C01
                Tbl[K,6] = C02
                Tbl[K,7] = C03
                Tbl[K,8] = C10
                Tbl[K,9] = C11
                ...
                Tbl[K,19] = C33
            On each grid square spline is equals to:
                S(x) = SUM(c[i,j]*(t^i)*(u^j), i=0..3, j=0..3)
                t = x-x[j]
                u = y-y[i]

  -- ALGLIB PROJECT --
     Copyright 16.04.2012 by Bochkanov Sergey
*************************************************************************/
void spline2dunpackv(spline2dinterpolant* c,
     ae_int_t* m,
     ae_int_t* n,
     ae_int_t* d,
     /* Real    */ ae_matrix* tbl,
     ae_state *_state)
{
    ae_int_t k;
    ae_int_t p;
    ae_int_t ci;
    ae_int_t cj;
    ae_int_t s1;
    ae_int_t s2;
    ae_int_t s3;
    ae_int_t s4;
    ae_int_t sfx;
    ae_int_t sfy;
    ae_int_t sfxy;
    double y1;
    double y2;
    double y3;
    double y4;
    double dt;
    double du;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k0;

    *m = 0;
    *n = 0;
    *d = 0;
    ae_matrix_clear(tbl);

    ae_assert(c->stype==-3||c->stype==-1, "Spline2DUnpackV: incorrect C (incorrect parameter C.SType)", _state);
    *n = c->n;
    *m = c->m;
    *d = c->d;
    ae_matrix_set_length(tbl, (*n-1)*(*m-1)*(*d), 20, _state);
    sfx = *n*(*m)*(*d);
    sfy = 2*(*n)*(*m)*(*d);
    sfxy = 3*(*n)*(*m)*(*d);
    for(i=0; i<=*m-2; i++)
    {
        for(j=0; j<=*n-2; j++)
        {
            for(k=0; k<=*d-1; k++)
            {
                p = *d*(i*(*n-1)+j)+k;
                tbl->ptr.pp_double[p][0] = c->x.ptr.p_double[j];
                tbl->ptr.pp_double[p][1] = c->x.ptr.p_double[j+1];
                tbl->ptr.pp_double[p][2] = c->y.ptr.p_double[i];
                tbl->ptr.pp_double[p][3] = c->y.ptr.p_double[i+1];
                dt = 1/(tbl->ptr.pp_double[p][1]-tbl->ptr.pp_double[p][0]);
                du = 1/(tbl->ptr.pp_double[p][3]-tbl->ptr.pp_double[p][2]);
                
                /*
                 * Bilinear interpolation
                 */
                if( c->stype==-1 )
                {
                    for(k0=4; k0<=19; k0++)
                    {
                        tbl->ptr.pp_double[p][k0] = (double)(0);
                    }
                    y1 = c->f.ptr.p_double[*d*(*n*i+j)+k];
                    y2 = c->f.ptr.p_double[*d*(*n*i+(j+1))+k];
                    y3 = c->f.ptr.p_double[*d*(*n*(i+1)+(j+1))+k];
                    y4 = c->f.ptr.p_double[*d*(*n*(i+1)+j)+k];
                    tbl->ptr.pp_double[p][4] = y1;
                    tbl->ptr.pp_double[p][4+1*4+0] = y2-y1;
                    tbl->ptr.pp_double[p][4+0*4+1] = y4-y1;
                    tbl->ptr.pp_double[p][4+1*4+1] = y3-y2-y4+y1;
                }
                
                /*
                 * Bicubic interpolation
                 */
                if( c->stype==-3 )
                {
                    s1 = *d*(*n*i+j)+k;
                    s2 = *d*(*n*i+(j+1))+k;
                    s3 = *d*(*n*(i+1)+(j+1))+k;
                    s4 = *d*(*n*(i+1)+j)+k;
                    tbl->ptr.pp_double[p][4+0*4+0] = c->f.ptr.p_double[s1];
                    tbl->ptr.pp_double[p][4+0*4+1] = c->f.ptr.p_double[sfy+s1]/du;
                    tbl->ptr.pp_double[p][4+0*4+2] = -3*c->f.ptr.p_double[s1]+3*c->f.ptr.p_double[s4]-2*c->f.ptr.p_double[sfy+s1]/du-c->f.ptr.p_double[sfy+s4]/du;
                    tbl->ptr.pp_double[p][4+0*4+3] = 2*c->f.ptr.p_double[s1]-2*c->f.ptr.p_double[s4]+c->f.ptr.p_double[sfy+s1]/du+c->f.ptr.p_double[sfy+s4]/du;
                    tbl->ptr.pp_double[p][4+1*4+0] = c->f.ptr.p_double[sfx+s1]/dt;
                    tbl->ptr.pp_double[p][4+1*4+1] = c->f.ptr.p_double[sfxy+s1]/(dt*du);
                    tbl->ptr.pp_double[p][4+1*4+2] = -3*c->f.ptr.p_double[sfx+s1]/dt+3*c->f.ptr.p_double[sfx+s4]/dt-2*c->f.ptr.p_double[sfxy+s1]/(dt*du)-c->f.ptr.p_double[sfxy+s4]/(dt*du);
                    tbl->ptr.pp_double[p][4+1*4+3] = 2*c->f.ptr.p_double[sfx+s1]/dt-2*c->f.ptr.p_double[sfx+s4]/dt+c->f.ptr.p_double[sfxy+s1]/(dt*du)+c->f.ptr.p_double[sfxy+s4]/(dt*du);
                    tbl->ptr.pp_double[p][4+2*4+0] = -3*c->f.ptr.p_double[s1]+3*c->f.ptr.p_double[s2]-2*c->f.ptr.p_double[sfx+s1]/dt-c->f.ptr.p_double[sfx+s2]/dt;
                    tbl->ptr.pp_double[p][4+2*4+1] = -3*c->f.ptr.p_double[sfy+s1]/du+3*c->f.ptr.p_double[sfy+s2]/du-2*c->f.ptr.p_double[sfxy+s1]/(dt*du)-c->f.ptr.p_double[sfxy+s2]/(dt*du);
                    tbl->ptr.pp_double[p][4+2*4+2] = 9*c->f.ptr.p_double[s1]-9*c->f.ptr.p_double[s2]+9*c->f.ptr.p_double[s3]-9*c->f.ptr.p_double[s4]+6*c->f.ptr.p_double[sfx+s1]/dt+3*c->f.ptr.p_double[sfx+s2]/dt-3*c->f.ptr.p_double[sfx+s3]/dt-6*c->f.ptr.p_double[sfx+s4]/dt+6*c->f.ptr.p_double[sfy+s1]/du-6*c->f.ptr.p_double[sfy+s2]/du-3*c->f.ptr.p_double[sfy+s3]/du+3*c->f.ptr.p_double[sfy+s4]/du+4*c->f.ptr.p_double[sfxy+s1]/(dt*du)+2*c->f.ptr.p_double[sfxy+s2]/(dt*du)+c->f.ptr.p_double[sfxy+s3]/(dt*du)+2*c->f.ptr.p_double[sfxy+s4]/(dt*du);
                    tbl->ptr.pp_double[p][4+2*4+3] = -6*c->f.ptr.p_double[s1]+6*c->f.ptr.p_double[s2]-6*c->f.ptr.p_double[s3]+6*c->f.ptr.p_double[s4]-4*c->f.ptr.p_double[sfx+s1]/dt-2*c->f.ptr.p_double[sfx+s2]/dt+2*c->f.ptr.p_double[sfx+s3]/dt+4*c->f.ptr.p_double[sfx+s4]/dt-3*c->f.ptr.p_double[sfy+s1]/du+3*c->f.ptr.p_double[sfy+s2]/du+3*c->f.ptr.p_double[sfy+s3]/du-3*c->f.ptr.p_double[sfy+s4]/du-2*c->f.ptr.p_double[sfxy+s1]/(dt*du)-c->f.ptr.p_double[sfxy+s2]/(dt*du)-c->f.ptr.p_double[sfxy+s3]/(dt*du)-2*c->f.ptr.p_double[sfxy+s4]/(dt*du);
                    tbl->ptr.pp_double[p][4+3*4+0] = 2*c->f.ptr.p_double[s1]-2*c->f.ptr.p_double[s2]+c->f.ptr.p_double[sfx+s1]/dt+c->f.ptr.p_double[sfx+s2]/dt;
                    tbl->ptr.pp_double[p][4+3*4+1] = 2*c->f.ptr.p_double[sfy+s1]/du-2*c->f.ptr.p_double[sfy+s2]/du+c->f.ptr.p_double[sfxy+s1]/(dt*du)+c->f.ptr.p_double[sfxy+s2]/(dt*du);
                    tbl->ptr.pp_double[p][4+3*4+2] = -6*c->f.ptr.p_double[s1]+6*c->f.ptr.p_double[s2]-6*c->f.ptr.p_double[s3]+6*c->f.ptr.p_double[s4]-3*c->f.ptr.p_double[sfx+s1]/dt-3*c->f.ptr.p_double[sfx+s2]/dt+3*c->f.ptr.p_double[sfx+s3]/dt+3*c->f.ptr.p_double[sfx+s4]/dt-4*c->f.ptr.p_double[sfy+s1]/du+4*c->f.ptr.p_double[sfy+s2]/du+2*c->f.ptr.p_double[sfy+s3]/du-2*c->f.ptr.p_double[sfy+s4]/du-2*c->f.ptr.p_double[sfxy+s1]/(dt*du)-2*c->f.ptr.p_double[sfxy+s2]/(dt*du)-c->f.ptr.p_double[sfxy+s3]/(dt*du)-c->f.ptr.p_double[sfxy+s4]/(dt*du);
                    tbl->ptr.pp_double[p][4+3*4+3] = 4*c->f.ptr.p_double[s1]-4*c->f.ptr.p_double[s2]+4*c->f.ptr.p_double[s3]-4*c->f.ptr.p_double[s4]+2*c->f.ptr.p_double[sfx+s1]/dt+2*c->f.ptr.p_double[sfx+s2]/dt-2*c->f.ptr.p_double[sfx+s3]/dt-2*c->f.ptr.p_double[sfx+s4]/dt+2*c->f.ptr.p_double[sfy+s1]/du-2*c->f.ptr.p_double[sfy+s2]/du-2*c->f.ptr.p_double[sfy+s3]/du+2*c->f.ptr.p_double[sfy+s4]/du+c->f.ptr.p_double[sfxy+s1]/(dt*du)+c->f.ptr.p_double[sfxy+s2]/(dt*du)+c->f.ptr.p_double[sfxy+s3]/(dt*du)+c->f.ptr.p_double[sfxy+s4]/(dt*du);
                }
                
                /*
                 * Rescale Cij
                 */
                for(ci=0; ci<=3; ci++)
                {
                    for(cj=0; cj<=3; cj++)
                    {
                        tbl->ptr.pp_double[p][4+ci*4+cj] = tbl->ptr.pp_double[p][4+ci*4+cj]*ae_pow(dt, (double)(ci), _state)*ae_pow(du, (double)(cj), _state);
                    }
                }
            }
        }
    }
}


/*************************************************************************
This subroutine was deprecated in ALGLIB 3.6.0

We recommend you to switch  to  Spline2DBuildBilinearV(),  which  is  more
flexible and accepts its arguments in more convenient order.

  -- ALGLIB PROJECT --
     Copyright 05.07.2007 by Bochkanov Sergey
*************************************************************************/
void spline2dbuildbilinear(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_matrix* f,
     ae_int_t m,
     ae_int_t n,
     spline2dinterpolant* c,
     ae_state *_state)
{
    double t;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;

    _spline2dinterpolant_clear(c);

    ae_assert(n>=2, "Spline2DBuildBilinear: N<2", _state);
    ae_assert(m>=2, "Spline2DBuildBilinear: M<2", _state);
    ae_assert(x->cnt>=n&&y->cnt>=m, "Spline2DBuildBilinear: length of X or Y is too short (Length(X/Y)<N/M)", _state);
    ae_assert(isfinitevector(x, n, _state)&&isfinitevector(y, m, _state), "Spline2DBuildBilinear: X or Y contains NaN or Infinite value", _state);
    ae_assert(f->rows>=m&&f->cols>=n, "Spline2DBuildBilinear: size of F is too small (rows(F)<M or cols(F)<N)", _state);
    ae_assert(apservisfinitematrix(f, m, n, _state), "Spline2DBuildBilinear: F contains NaN or Infinite value", _state);
    
    /*
     * Fill interpolant
     */
    c->k = 1;
    c->n = n;
    c->m = m;
    c->d = 1;
    c->stype = -1;
    ae_vector_set_length(&c->x, c->n, _state);
    ae_vector_set_length(&c->y, c->m, _state);
    ae_vector_set_length(&c->f, c->n*c->m, _state);
    for(i=0; i<=c->n-1; i++)
    {
        c->x.ptr.p_double[i] = x->ptr.p_double[i];
    }
    for(i=0; i<=c->m-1; i++)
    {
        c->y.ptr.p_double[i] = y->ptr.p_double[i];
    }
    for(i=0; i<=c->m-1; i++)
    {
        for(j=0; j<=c->n-1; j++)
        {
            c->f.ptr.p_double[i*c->n+j] = f->ptr.pp_double[i][j];
        }
    }
    
    /*
     * Sort points
     */
    for(j=0; j<=c->n-1; j++)
    {
        k = j;
        for(i=j+1; i<=c->n-1; i++)
        {
            if( ae_fp_less(c->x.ptr.p_double[i],c->x.ptr.p_double[k]) )
            {
                k = i;
            }
        }
        if( k!=j )
        {
            for(i=0; i<=c->m-1; i++)
            {
                t = c->f.ptr.p_double[i*c->n+j];
                c->f.ptr.p_double[i*c->n+j] = c->f.ptr.p_double[i*c->n+k];
                c->f.ptr.p_double[i*c->n+k] = t;
            }
            t = c->x.ptr.p_double[j];
            c->x.ptr.p_double[j] = c->x.ptr.p_double[k];
            c->x.ptr.p_double[k] = t;
        }
    }
    for(i=0; i<=c->m-1; i++)
    {
        k = i;
        for(j=i+1; j<=c->m-1; j++)
        {
            if( ae_fp_less(c->y.ptr.p_double[j],c->y.ptr.p_double[k]) )
            {
                k = j;
            }
        }
        if( k!=i )
        {
            for(j=0; j<=c->n-1; j++)
            {
                t = c->f.ptr.p_double[i*c->n+j];
                c->f.ptr.p_double[i*c->n+j] = c->f.ptr.p_double[k*c->n+j];
                c->f.ptr.p_double[k*c->n+j] = t;
            }
            t = c->y.ptr.p_double[i];
            c->y.ptr.p_double[i] = c->y.ptr.p_double[k];
            c->y.ptr.p_double[k] = t;
        }
    }
}


/*************************************************************************
This subroutine was deprecated in ALGLIB 3.6.0

We recommend you to switch  to  Spline2DBuildBicubicV(),  which  is  more
flexible and accepts its arguments in more convenient order.

  -- ALGLIB PROJECT --
     Copyright 05.07.2007 by Bochkanov Sergey
*************************************************************************/
void spline2dbuildbicubic(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_matrix* f,
     ae_int_t m,
     ae_int_t n,
     spline2dinterpolant* c,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _f;
    ae_int_t sfx;
    ae_int_t sfy;
    ae_int_t sfxy;
    ae_matrix dx;
    ae_matrix dy;
    ae_matrix dxy;
    double t;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_f, f, _state);
    f = &_f;
    _spline2dinterpolant_clear(c);
    ae_matrix_init(&dx, 0, 0, DT_REAL, _state);
    ae_matrix_init(&dy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&dxy, 0, 0, DT_REAL, _state);

    ae_assert(n>=2, "Spline2DBuildBicubicSpline: N<2", _state);
    ae_assert(m>=2, "Spline2DBuildBicubicSpline: M<2", _state);
    ae_assert(x->cnt>=n&&y->cnt>=m, "Spline2DBuildBicubic: length of X or Y is too short (Length(X/Y)<N/M)", _state);
    ae_assert(isfinitevector(x, n, _state)&&isfinitevector(y, m, _state), "Spline2DBuildBicubic: X or Y contains NaN or Infinite value", _state);
    ae_assert(f->rows>=m&&f->cols>=n, "Spline2DBuildBicubic: size of F is too small (rows(F)<M or cols(F)<N)", _state);
    ae_assert(apservisfinitematrix(f, m, n, _state), "Spline2DBuildBicubic: F contains NaN or Infinite value", _state);
    
    /*
     * Fill interpolant:
     *  F[0]...F[N*M-1]:
     *      f(i,j) table. f(0,0), f(0, 1), f(0,2) and so on...
     *  F[N*M]...F[2*N*M-1]:
     *      df(i,j)/dx table.
     *  F[2*N*M]...F[3*N*M-1]:
     *      df(i,j)/dy table.
     *  F[3*N*M]...F[4*N*M-1]:
     *      d2f(i,j)/dxdy table.
     */
    c->k = 3;
    c->d = 1;
    c->n = n;
    c->m = m;
    c->stype = -3;
    sfx = c->n*c->m;
    sfy = 2*c->n*c->m;
    sfxy = 3*c->n*c->m;
    ae_vector_set_length(&c->x, c->n, _state);
    ae_vector_set_length(&c->y, c->m, _state);
    ae_vector_set_length(&c->f, 4*c->n*c->m, _state);
    for(i=0; i<=c->n-1; i++)
    {
        c->x.ptr.p_double[i] = x->ptr.p_double[i];
    }
    for(i=0; i<=c->m-1; i++)
    {
        c->y.ptr.p_double[i] = y->ptr.p_double[i];
    }
    
    /*
     * Sort points
     */
    for(j=0; j<=c->n-1; j++)
    {
        k = j;
        for(i=j+1; i<=c->n-1; i++)
        {
            if( ae_fp_less(c->x.ptr.p_double[i],c->x.ptr.p_double[k]) )
            {
                k = i;
            }
        }
        if( k!=j )
        {
            for(i=0; i<=c->m-1; i++)
            {
                t = f->ptr.pp_double[i][j];
                f->ptr.pp_double[i][j] = f->ptr.pp_double[i][k];
                f->ptr.pp_double[i][k] = t;
            }
            t = c->x.ptr.p_double[j];
            c->x.ptr.p_double[j] = c->x.ptr.p_double[k];
            c->x.ptr.p_double[k] = t;
        }
    }
    for(i=0; i<=c->m-1; i++)
    {
        k = i;
        for(j=i+1; j<=c->m-1; j++)
        {
            if( ae_fp_less(c->y.ptr.p_double[j],c->y.ptr.p_double[k]) )
            {
                k = j;
            }
        }
        if( k!=i )
        {
            for(j=0; j<=c->n-1; j++)
            {
                t = f->ptr.pp_double[i][j];
                f->ptr.pp_double[i][j] = f->ptr.pp_double[k][j];
                f->ptr.pp_double[k][j] = t;
            }
            t = c->y.ptr.p_double[i];
            c->y.ptr.p_double[i] = c->y.ptr.p_double[k];
            c->y.ptr.p_double[k] = t;
        }
    }
    spline2d_bicubiccalcderivatives(f, &c->x, &c->y, c->m, c->n, &dx, &dy, &dxy, _state);
    for(i=0; i<=c->m-1; i++)
    {
        for(j=0; j<=c->n-1; j++)
        {
            k = i*c->n+j;
            c->f.ptr.p_double[k] = f->ptr.pp_double[i][j];
            c->f.ptr.p_double[sfx+k] = dx.ptr.pp_double[i][j];
            c->f.ptr.p_double[sfy+k] = dy.ptr.pp_double[i][j];
            c->f.ptr.p_double[sfxy+k] = dxy.ptr.pp_double[i][j];
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This subroutine was deprecated in ALGLIB 3.6.0

We recommend you to switch  to  Spline2DUnpackV(),  which is more flexible
and accepts its arguments in more convenient order.

  -- ALGLIB PROJECT --
     Copyright 29.06.2007 by Bochkanov Sergey
*************************************************************************/
void spline2dunpack(spline2dinterpolant* c,
     ae_int_t* m,
     ae_int_t* n,
     /* Real    */ ae_matrix* tbl,
     ae_state *_state)
{
    ae_int_t k;
    ae_int_t p;
    ae_int_t ci;
    ae_int_t cj;
    ae_int_t s1;
    ae_int_t s2;
    ae_int_t s3;
    ae_int_t s4;
    ae_int_t sfx;
    ae_int_t sfy;
    ae_int_t sfxy;
    double y1;
    double y2;
    double y3;
    double y4;
    double dt;
    double du;
    ae_int_t i;
    ae_int_t j;

    *m = 0;
    *n = 0;
    ae_matrix_clear(tbl);

    ae_assert(c->stype==-3||c->stype==-1, "Spline2DUnpack: incorrect C (incorrect parameter C.SType)", _state);
    if( c->d!=1 )
    {
        *n = 0;
        *m = 0;
        return;
    }
    *n = c->n;
    *m = c->m;
    ae_matrix_set_length(tbl, (*n-1)*(*m-1), 20, _state);
    sfx = *n*(*m);
    sfy = 2*(*n)*(*m);
    sfxy = 3*(*n)*(*m);
    
    /*
     * Fill
     */
    for(i=0; i<=*m-2; i++)
    {
        for(j=0; j<=*n-2; j++)
        {
            p = i*(*n-1)+j;
            tbl->ptr.pp_double[p][0] = c->x.ptr.p_double[j];
            tbl->ptr.pp_double[p][1] = c->x.ptr.p_double[j+1];
            tbl->ptr.pp_double[p][2] = c->y.ptr.p_double[i];
            tbl->ptr.pp_double[p][3] = c->y.ptr.p_double[i+1];
            dt = 1/(tbl->ptr.pp_double[p][1]-tbl->ptr.pp_double[p][0]);
            du = 1/(tbl->ptr.pp_double[p][3]-tbl->ptr.pp_double[p][2]);
            
            /*
             * Bilinear interpolation
             */
            if( c->stype==-1 )
            {
                for(k=4; k<=19; k++)
                {
                    tbl->ptr.pp_double[p][k] = (double)(0);
                }
                y1 = c->f.ptr.p_double[*n*i+j];
                y2 = c->f.ptr.p_double[*n*i+(j+1)];
                y3 = c->f.ptr.p_double[*n*(i+1)+(j+1)];
                y4 = c->f.ptr.p_double[*n*(i+1)+j];
                tbl->ptr.pp_double[p][4] = y1;
                tbl->ptr.pp_double[p][4+1*4+0] = y2-y1;
                tbl->ptr.pp_double[p][4+0*4+1] = y4-y1;
                tbl->ptr.pp_double[p][4+1*4+1] = y3-y2-y4+y1;
            }
            
            /*
             * Bicubic interpolation
             */
            if( c->stype==-3 )
            {
                s1 = *n*i+j;
                s2 = *n*i+(j+1);
                s3 = *n*(i+1)+(j+1);
                s4 = *n*(i+1)+j;
                tbl->ptr.pp_double[p][4+0*4+0] = c->f.ptr.p_double[s1];
                tbl->ptr.pp_double[p][4+0*4+1] = c->f.ptr.p_double[sfy+s1]/du;
                tbl->ptr.pp_double[p][4+0*4+2] = -3*c->f.ptr.p_double[s1]+3*c->f.ptr.p_double[s4]-2*c->f.ptr.p_double[sfy+s1]/du-c->f.ptr.p_double[sfy+s4]/du;
                tbl->ptr.pp_double[p][4+0*4+3] = 2*c->f.ptr.p_double[s1]-2*c->f.ptr.p_double[s4]+c->f.ptr.p_double[sfy+s1]/du+c->f.ptr.p_double[sfy+s4]/du;
                tbl->ptr.pp_double[p][4+1*4+0] = c->f.ptr.p_double[sfx+s1]/dt;
                tbl->ptr.pp_double[p][4+1*4+1] = c->f.ptr.p_double[sfxy+s1]/(dt*du);
                tbl->ptr.pp_double[p][4+1*4+2] = -3*c->f.ptr.p_double[sfx+s1]/dt+3*c->f.ptr.p_double[sfx+s4]/dt-2*c->f.ptr.p_double[sfxy+s1]/(dt*du)-c->f.ptr.p_double[sfxy+s4]/(dt*du);
                tbl->ptr.pp_double[p][4+1*4+3] = 2*c->f.ptr.p_double[sfx+s1]/dt-2*c->f.ptr.p_double[sfx+s4]/dt+c->f.ptr.p_double[sfxy+s1]/(dt*du)+c->f.ptr.p_double[sfxy+s4]/(dt*du);
                tbl->ptr.pp_double[p][4+2*4+0] = -3*c->f.ptr.p_double[s1]+3*c->f.ptr.p_double[s2]-2*c->f.ptr.p_double[sfx+s1]/dt-c->f.ptr.p_double[sfx+s2]/dt;
                tbl->ptr.pp_double[p][4+2*4+1] = -3*c->f.ptr.p_double[sfy+s1]/du+3*c->f.ptr.p_double[sfy+s2]/du-2*c->f.ptr.p_double[sfxy+s1]/(dt*du)-c->f.ptr.p_double[sfxy+s2]/(dt*du);
                tbl->ptr.pp_double[p][4+2*4+2] = 9*c->f.ptr.p_double[s1]-9*c->f.ptr.p_double[s2]+9*c->f.ptr.p_double[s3]-9*c->f.ptr.p_double[s4]+6*c->f.ptr.p_double[sfx+s1]/dt+3*c->f.ptr.p_double[sfx+s2]/dt-3*c->f.ptr.p_double[sfx+s3]/dt-6*c->f.ptr.p_double[sfx+s4]/dt+6*c->f.ptr.p_double[sfy+s1]/du-6*c->f.ptr.p_double[sfy+s2]/du-3*c->f.ptr.p_double[sfy+s3]/du+3*c->f.ptr.p_double[sfy+s4]/du+4*c->f.ptr.p_double[sfxy+s1]/(dt*du)+2*c->f.ptr.p_double[sfxy+s2]/(dt*du)+c->f.ptr.p_double[sfxy+s3]/(dt*du)+2*c->f.ptr.p_double[sfxy+s4]/(dt*du);
                tbl->ptr.pp_double[p][4+2*4+3] = -6*c->f.ptr.p_double[s1]+6*c->f.ptr.p_double[s2]-6*c->f.ptr.p_double[s3]+6*c->f.ptr.p_double[s4]-4*c->f.ptr.p_double[sfx+s1]/dt-2*c->f.ptr.p_double[sfx+s2]/dt+2*c->f.ptr.p_double[sfx+s3]/dt+4*c->f.ptr.p_double[sfx+s4]/dt-3*c->f.ptr.p_double[sfy+s1]/du+3*c->f.ptr.p_double[sfy+s2]/du+3*c->f.ptr.p_double[sfy+s3]/du-3*c->f.ptr.p_double[sfy+s4]/du-2*c->f.ptr.p_double[sfxy+s1]/(dt*du)-c->f.ptr.p_double[sfxy+s2]/(dt*du)-c->f.ptr.p_double[sfxy+s3]/(dt*du)-2*c->f.ptr.p_double[sfxy+s4]/(dt*du);
                tbl->ptr.pp_double[p][4+3*4+0] = 2*c->f.ptr.p_double[s1]-2*c->f.ptr.p_double[s2]+c->f.ptr.p_double[sfx+s1]/dt+c->f.ptr.p_double[sfx+s2]/dt;
                tbl->ptr.pp_double[p][4+3*4+1] = 2*c->f.ptr.p_double[sfy+s1]/du-2*c->f.ptr.p_double[sfy+s2]/du+c->f.ptr.p_double[sfxy+s1]/(dt*du)+c->f.ptr.p_double[sfxy+s2]/(dt*du);
                tbl->ptr.pp_double[p][4+3*4+2] = -6*c->f.ptr.p_double[s1]+6*c->f.ptr.p_double[s2]-6*c->f.ptr.p_double[s3]+6*c->f.ptr.p_double[s4]-3*c->f.ptr.p_double[sfx+s1]/dt-3*c->f.ptr.p_double[sfx+s2]/dt+3*c->f.ptr.p_double[sfx+s3]/dt+3*c->f.ptr.p_double[sfx+s4]/dt-4*c->f.ptr.p_double[sfy+s1]/du+4*c->f.ptr.p_double[sfy+s2]/du+2*c->f.ptr.p_double[sfy+s3]/du-2*c->f.ptr.p_double[sfy+s4]/du-2*c->f.ptr.p_double[sfxy+s1]/(dt*du)-2*c->f.ptr.p_double[sfxy+s2]/(dt*du)-c->f.ptr.p_double[sfxy+s3]/(dt*du)-c->f.ptr.p_double[sfxy+s4]/(dt*du);
                tbl->ptr.pp_double[p][4+3*4+3] = 4*c->f.ptr.p_double[s1]-4*c->f.ptr.p_double[s2]+4*c->f.ptr.p_double[s3]-4*c->f.ptr.p_double[s4]+2*c->f.ptr.p_double[sfx+s1]/dt+2*c->f.ptr.p_double[sfx+s2]/dt-2*c->f.ptr.p_double[sfx+s3]/dt-2*c->f.ptr.p_double[sfx+s4]/dt+2*c->f.ptr.p_double[sfy+s1]/du-2*c->f.ptr.p_double[sfy+s2]/du-2*c->f.ptr.p_double[sfy+s3]/du+2*c->f.ptr.p_double[sfy+s4]/du+c->f.ptr.p_double[sfxy+s1]/(dt*du)+c->f.ptr.p_double[sfxy+s2]/(dt*du)+c->f.ptr.p_double[sfxy+s3]/(dt*du)+c->f.ptr.p_double[sfxy+s4]/(dt*du);
            }
            
            /*
             * Rescale Cij
             */
            for(ci=0; ci<=3; ci++)
            {
                for(cj=0; cj<=3; cj++)
                {
                    tbl->ptr.pp_double[p][4+ci*4+cj] = tbl->ptr.pp_double[p][4+ci*4+cj]*ae_pow(dt, (double)(ci), _state)*ae_pow(du, (double)(cj), _state);
                }
            }
        }
    }
}


/*************************************************************************
Internal subroutine.
Calculation of the first derivatives and the cross-derivative.
*************************************************************************/
static void spline2d_bicubiccalcderivatives(/* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* dx,
     /* Real    */ ae_matrix* dy,
     /* Real    */ ae_matrix* dxy,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_vector xt;
    ae_vector ft;
    double s;
    double ds;
    double d2s;
    spline1dinterpolant c;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(dx);
    ae_matrix_clear(dy);
    ae_matrix_clear(dxy);
    ae_vector_init(&xt, 0, DT_REAL, _state);
    ae_vector_init(&ft, 0, DT_REAL, _state);
    _spline1dinterpolant_init(&c, _state);

    ae_matrix_set_length(dx, m, n, _state);
    ae_matrix_set_length(dy, m, n, _state);
    ae_matrix_set_length(dxy, m, n, _state);
    
    /*
     * dF/dX
     */
    ae_vector_set_length(&xt, n, _state);
    ae_vector_set_length(&ft, n, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            xt.ptr.p_double[j] = x->ptr.p_double[j];
            ft.ptr.p_double[j] = a->ptr.pp_double[i][j];
        }
        spline1dbuildcubic(&xt, &ft, n, 0, 0.0, 0, 0.0, &c, _state);
        for(j=0; j<=n-1; j++)
        {
            spline1ddiff(&c, x->ptr.p_double[j], &s, &ds, &d2s, _state);
            dx->ptr.pp_double[i][j] = ds;
        }
    }
    
    /*
     * dF/dY
     */
    ae_vector_set_length(&xt, m, _state);
    ae_vector_set_length(&ft, m, _state);
    for(j=0; j<=n-1; j++)
    {
        for(i=0; i<=m-1; i++)
        {
            xt.ptr.p_double[i] = y->ptr.p_double[i];
            ft.ptr.p_double[i] = a->ptr.pp_double[i][j];
        }
        spline1dbuildcubic(&xt, &ft, m, 0, 0.0, 0, 0.0, &c, _state);
        for(i=0; i<=m-1; i++)
        {
            spline1ddiff(&c, y->ptr.p_double[i], &s, &ds, &d2s, _state);
            dy->ptr.pp_double[i][j] = ds;
        }
    }
    
    /*
     * d2F/dXdY
     */
    ae_vector_set_length(&xt, n, _state);
    ae_vector_set_length(&ft, n, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            xt.ptr.p_double[j] = x->ptr.p_double[j];
            ft.ptr.p_double[j] = dy->ptr.pp_double[i][j];
        }
        spline1dbuildcubic(&xt, &ft, n, 0, 0.0, 0, 0.0, &c, _state);
        for(j=0; j<=n-1; j++)
        {
            spline1ddiff(&c, x->ptr.p_double[j], &s, &ds, &d2s, _state);
            dxy->ptr.pp_double[i][j] = ds;
        }
    }
    ae_frame_leave(_state);
}


void _spline2dinterpolant_init(void* _p, ae_state *_state)
{
    spline2dinterpolant *p = (spline2dinterpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->x, 0, DT_REAL, _state);
    ae_vector_init(&p->y, 0, DT_REAL, _state);
    ae_vector_init(&p->f, 0, DT_REAL, _state);
}


void _spline2dinterpolant_init_copy(void* _dst, void* _src, ae_state *_state)
{
    spline2dinterpolant *dst = (spline2dinterpolant*)_dst;
    spline2dinterpolant *src = (spline2dinterpolant*)_src;
    dst->k = src->k;
    dst->stype = src->stype;
    dst->n = src->n;
    dst->m = src->m;
    dst->d = src->d;
    ae_vector_init_copy(&dst->x, &src->x, _state);
    ae_vector_init_copy(&dst->y, &src->y, _state);
    ae_vector_init_copy(&dst->f, &src->f, _state);
}


void _spline2dinterpolant_clear(void* _p)
{
    spline2dinterpolant *p = (spline2dinterpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->x);
    ae_vector_clear(&p->y);
    ae_vector_clear(&p->f);
}


void _spline2dinterpolant_destroy(void* _p)
{
    spline2dinterpolant *p = (spline2dinterpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->x);
    ae_vector_destroy(&p->y);
    ae_vector_destroy(&p->f);
}


/*$ End $*/
