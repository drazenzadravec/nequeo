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
#include "spline3d.h"


/*$ Declarations $*/
static void spline3d_spline3ddiff(spline3dinterpolant* c,
     double x,
     double y,
     double z,
     double* f,
     double* fx,
     double* fy,
     double* fxy,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
This subroutine calculates the value of the trilinear or tricubic spline at
the given point (X,Y,Z).

INPUT PARAMETERS:
    C   -   coefficients table.
            Built by BuildBilinearSpline or BuildBicubicSpline.
    X, Y,
    Z   -   point

Result:
    S(x,y,z)

  -- ALGLIB PROJECT --
     Copyright 26.04.2012 by Bochkanov Sergey
*************************************************************************/
double spline3dcalc(spline3dinterpolant* c,
     double x,
     double y,
     double z,
     ae_state *_state)
{
    double v;
    double vx;
    double vy;
    double vxy;
    double result;


    ae_assert(c->stype==-1||c->stype==-3, "Spline3DCalc: incorrect C (incorrect parameter C.SType)", _state);
    ae_assert((ae_isfinite(x, _state)&&ae_isfinite(y, _state))&&ae_isfinite(z, _state), "Spline3DCalc: X=NaN/Infinite, Y=NaN/Infinite or Z=NaN/Infinite", _state);
    if( c->d!=1 )
    {
        result = (double)(0);
        return result;
    }
    spline3d_spline3ddiff(c, x, y, z, &v, &vx, &vy, &vxy, _state);
    result = v;
    return result;
}


/*************************************************************************
This subroutine performs linear transformation of the spline argument.

INPUT PARAMETERS:
    C       -   spline interpolant
    AX, BX  -   transformation coefficients: x = A*u + B
    AY, BY  -   transformation coefficients: y = A*v + B
    AZ, BZ  -   transformation coefficients: z = A*w + B
    
OUTPUT PARAMETERS:
    C   -   transformed spline

  -- ALGLIB PROJECT --
     Copyright 26.04.2012 by Bochkanov Sergey
*************************************************************************/
void spline3dlintransxyz(spline3dinterpolant* c,
     double ax,
     double bx,
     double ay,
     double by,
     double az,
     double bz,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    ae_vector y;
    ae_vector z;
    ae_vector f;
    ae_vector v;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t di;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&z, 0, DT_REAL, _state);
    ae_vector_init(&f, 0, DT_REAL, _state);
    ae_vector_init(&v, 0, DT_REAL, _state);

    ae_assert(c->stype==-3||c->stype==-1, "Spline3DLinTransXYZ: incorrect C (incorrect parameter C.SType)", _state);
    ae_vector_set_length(&x, c->n, _state);
    ae_vector_set_length(&y, c->m, _state);
    ae_vector_set_length(&z, c->l, _state);
    ae_vector_set_length(&f, c->m*c->n*c->l*c->d, _state);
    for(j=0; j<=c->n-1; j++)
    {
        x.ptr.p_double[j] = c->x.ptr.p_double[j];
    }
    for(i=0; i<=c->m-1; i++)
    {
        y.ptr.p_double[i] = c->y.ptr.p_double[i];
    }
    for(i=0; i<=c->l-1; i++)
    {
        z.ptr.p_double[i] = c->z.ptr.p_double[i];
    }
    
    /*
     * Handle different combinations of zero/nonzero AX/AY/AZ
     */
    if( (ae_fp_neq(ax,(double)(0))&&ae_fp_neq(ay,(double)(0)))&&ae_fp_neq(az,(double)(0)) )
    {
        ae_v_move(&f.ptr.p_double[0], 1, &c->f.ptr.p_double[0], 1, ae_v_len(0,c->m*c->n*c->l*c->d-1));
    }
    if( (ae_fp_eq(ax,(double)(0))&&ae_fp_neq(ay,(double)(0)))&&ae_fp_neq(az,(double)(0)) )
    {
        for(i=0; i<=c->m-1; i++)
        {
            for(j=0; j<=c->l-1; j++)
            {
                spline3dcalcv(c, bx, y.ptr.p_double[i], z.ptr.p_double[j], &v, _state);
                for(k=0; k<=c->n-1; k++)
                {
                    for(di=0; di<=c->d-1; di++)
                    {
                        f.ptr.p_double[c->d*(c->n*(c->m*j+i)+k)+di] = v.ptr.p_double[di];
                    }
                }
            }
        }
        ax = (double)(1);
        bx = (double)(0);
    }
    if( (ae_fp_neq(ax,(double)(0))&&ae_fp_eq(ay,(double)(0)))&&ae_fp_neq(az,(double)(0)) )
    {
        for(i=0; i<=c->n-1; i++)
        {
            for(j=0; j<=c->l-1; j++)
            {
                spline3dcalcv(c, x.ptr.p_double[i], by, z.ptr.p_double[j], &v, _state);
                for(k=0; k<=c->m-1; k++)
                {
                    for(di=0; di<=c->d-1; di++)
                    {
                        f.ptr.p_double[c->d*(c->n*(c->m*j+k)+i)+di] = v.ptr.p_double[di];
                    }
                }
            }
        }
        ay = (double)(1);
        by = (double)(0);
    }
    if( (ae_fp_neq(ax,(double)(0))&&ae_fp_neq(ay,(double)(0)))&&ae_fp_eq(az,(double)(0)) )
    {
        for(i=0; i<=c->n-1; i++)
        {
            for(j=0; j<=c->m-1; j++)
            {
                spline3dcalcv(c, x.ptr.p_double[i], y.ptr.p_double[j], bz, &v, _state);
                for(k=0; k<=c->l-1; k++)
                {
                    for(di=0; di<=c->d-1; di++)
                    {
                        f.ptr.p_double[c->d*(c->n*(c->m*k+j)+i)+di] = v.ptr.p_double[di];
                    }
                }
            }
        }
        az = (double)(1);
        bz = (double)(0);
    }
    if( (ae_fp_eq(ax,(double)(0))&&ae_fp_eq(ay,(double)(0)))&&ae_fp_neq(az,(double)(0)) )
    {
        for(i=0; i<=c->l-1; i++)
        {
            spline3dcalcv(c, bx, by, z.ptr.p_double[i], &v, _state);
            for(k=0; k<=c->m-1; k++)
            {
                for(j=0; j<=c->n-1; j++)
                {
                    for(di=0; di<=c->d-1; di++)
                    {
                        f.ptr.p_double[c->d*(c->n*(c->m*i+k)+j)+di] = v.ptr.p_double[di];
                    }
                }
            }
        }
        ax = (double)(1);
        bx = (double)(0);
        ay = (double)(1);
        by = (double)(0);
    }
    if( (ae_fp_eq(ax,(double)(0))&&ae_fp_neq(ay,(double)(0)))&&ae_fp_eq(az,(double)(0)) )
    {
        for(i=0; i<=c->m-1; i++)
        {
            spline3dcalcv(c, bx, y.ptr.p_double[i], bz, &v, _state);
            for(k=0; k<=c->l-1; k++)
            {
                for(j=0; j<=c->n-1; j++)
                {
                    for(di=0; di<=c->d-1; di++)
                    {
                        f.ptr.p_double[c->d*(c->n*(c->m*k+i)+j)+di] = v.ptr.p_double[di];
                    }
                }
            }
        }
        ax = (double)(1);
        bx = (double)(0);
        az = (double)(1);
        bz = (double)(0);
    }
    if( (ae_fp_neq(ax,(double)(0))&&ae_fp_eq(ay,(double)(0)))&&ae_fp_eq(az,(double)(0)) )
    {
        for(i=0; i<=c->n-1; i++)
        {
            spline3dcalcv(c, x.ptr.p_double[i], by, bz, &v, _state);
            for(k=0; k<=c->l-1; k++)
            {
                for(j=0; j<=c->m-1; j++)
                {
                    for(di=0; di<=c->d-1; di++)
                    {
                        f.ptr.p_double[c->d*(c->n*(c->m*k+j)+i)+di] = v.ptr.p_double[di];
                    }
                }
            }
        }
        ay = (double)(1);
        by = (double)(0);
        az = (double)(1);
        bz = (double)(0);
    }
    if( (ae_fp_eq(ax,(double)(0))&&ae_fp_eq(ay,(double)(0)))&&ae_fp_eq(az,(double)(0)) )
    {
        spline3dcalcv(c, bx, by, bz, &v, _state);
        for(k=0; k<=c->l-1; k++)
        {
            for(j=0; j<=c->m-1; j++)
            {
                for(i=0; i<=c->n-1; i++)
                {
                    for(di=0; di<=c->d-1; di++)
                    {
                        f.ptr.p_double[c->d*(c->n*(c->m*k+j)+i)+di] = v.ptr.p_double[di];
                    }
                }
            }
        }
        ax = (double)(1);
        bx = (double)(0);
        ay = (double)(1);
        by = (double)(0);
        az = (double)(1);
        bz = (double)(0);
    }
    
    /*
     * General case: AX<>0, AY<>0, AZ<>0
     * Unpack, scale and pack again.
     */
    for(i=0; i<=c->n-1; i++)
    {
        x.ptr.p_double[i] = (x.ptr.p_double[i]-bx)/ax;
    }
    for(i=0; i<=c->m-1; i++)
    {
        y.ptr.p_double[i] = (y.ptr.p_double[i]-by)/ay;
    }
    for(i=0; i<=c->l-1; i++)
    {
        z.ptr.p_double[i] = (z.ptr.p_double[i]-bz)/az;
    }
    if( c->stype==-1 )
    {
        spline3dbuildtrilinearv(&x, c->n, &y, c->m, &z, c->l, &f, c->d, c, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This subroutine performs linear transformation of the spline.

INPUT PARAMETERS:
    C   -   spline interpolant.
    A, B-   transformation coefficients: S2(x,y) = A*S(x,y,z) + B
    
OUTPUT PARAMETERS:
    C   -   transformed spline

  -- ALGLIB PROJECT --
     Copyright 26.04.2012 by Bochkanov Sergey
*************************************************************************/
void spline3dlintransf(spline3dinterpolant* c,
     double a,
     double b,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    ae_vector y;
    ae_vector z;
    ae_vector f;
    ae_int_t i;
    ae_int_t j;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&z, 0, DT_REAL, _state);
    ae_vector_init(&f, 0, DT_REAL, _state);

    ae_assert(c->stype==-3||c->stype==-1, "Spline3DLinTransF: incorrect C (incorrect parameter C.SType)", _state);
    ae_vector_set_length(&x, c->n, _state);
    ae_vector_set_length(&y, c->m, _state);
    ae_vector_set_length(&z, c->l, _state);
    ae_vector_set_length(&f, c->m*c->n*c->l*c->d, _state);
    for(j=0; j<=c->n-1; j++)
    {
        x.ptr.p_double[j] = c->x.ptr.p_double[j];
    }
    for(i=0; i<=c->m-1; i++)
    {
        y.ptr.p_double[i] = c->y.ptr.p_double[i];
    }
    for(i=0; i<=c->l-1; i++)
    {
        z.ptr.p_double[i] = c->z.ptr.p_double[i];
    }
    for(i=0; i<=c->m*c->n*c->l*c->d-1; i++)
    {
        f.ptr.p_double[i] = a*c->f.ptr.p_double[i]+b;
    }
    if( c->stype==-1 )
    {
        spline3dbuildtrilinearv(&x, c->n, &y, c->m, &z, c->l, &f, c->d, c, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This subroutine makes the copy of the spline model.

INPUT PARAMETERS:
    C   -   spline interpolant

OUTPUT PARAMETERS:
    CC  -   spline copy

  -- ALGLIB PROJECT --
     Copyright 26.04.2012 by Bochkanov Sergey
*************************************************************************/
void spline3dcopy(spline3dinterpolant* c,
     spline3dinterpolant* cc,
     ae_state *_state)
{
    ae_int_t tblsize;

    _spline3dinterpolant_clear(cc);

    ae_assert(c->k==1||c->k==3, "Spline3DCopy: incorrect C (incorrect parameter C.K)", _state);
    cc->k = c->k;
    cc->n = c->n;
    cc->m = c->m;
    cc->l = c->l;
    cc->d = c->d;
    tblsize = c->n*c->m*c->l*c->d;
    cc->stype = c->stype;
    ae_vector_set_length(&cc->x, cc->n, _state);
    ae_vector_set_length(&cc->y, cc->m, _state);
    ae_vector_set_length(&cc->z, cc->l, _state);
    ae_vector_set_length(&cc->f, tblsize, _state);
    ae_v_move(&cc->x.ptr.p_double[0], 1, &c->x.ptr.p_double[0], 1, ae_v_len(0,cc->n-1));
    ae_v_move(&cc->y.ptr.p_double[0], 1, &c->y.ptr.p_double[0], 1, ae_v_len(0,cc->m-1));
    ae_v_move(&cc->z.ptr.p_double[0], 1, &c->z.ptr.p_double[0], 1, ae_v_len(0,cc->l-1));
    ae_v_move(&cc->f.ptr.p_double[0], 1, &c->f.ptr.p_double[0], 1, ae_v_len(0,tblsize-1));
}


/*************************************************************************
Trilinear spline resampling

INPUT PARAMETERS:
    A           -   array[0..OldXCount*OldYCount*OldZCount-1], function
                    values at the old grid, :
                        A[0]        x=0,y=0,z=0
                        A[1]        x=1,y=0,z=0
                        A[..]       ...
                        A[..]       x=oldxcount-1,y=0,z=0
                        A[..]       x=0,y=1,z=0
                        A[..]       ...
                        ...
    OldZCount   -   old Z-count, OldZCount>1
    OldYCount   -   old Y-count, OldYCount>1
    OldXCount   -   old X-count, OldXCount>1
    NewZCount   -   new Z-count, NewZCount>1
    NewYCount   -   new Y-count, NewYCount>1
    NewXCount   -   new X-count, NewXCount>1

OUTPUT PARAMETERS:
    B           -   array[0..NewXCount*NewYCount*NewZCount-1], function
                    values at the new grid:
                        B[0]        x=0,y=0,z=0
                        B[1]        x=1,y=0,z=0
                        B[..]       ...
                        B[..]       x=newxcount-1,y=0,z=0
                        B[..]       x=0,y=1,z=0
                        B[..]       ...
                        ...

  -- ALGLIB routine --
     26.04.2012
     Copyright by Bochkanov Sergey
*************************************************************************/
void spline3dresampletrilinear(/* Real    */ ae_vector* a,
     ae_int_t oldzcount,
     ae_int_t oldycount,
     ae_int_t oldxcount,
     ae_int_t newzcount,
     ae_int_t newycount,
     ae_int_t newxcount,
     /* Real    */ ae_vector* b,
     ae_state *_state)
{
    double xd;
    double yd;
    double zd;
    double c0;
    double c1;
    double c2;
    double c3;
    ae_int_t ix;
    ae_int_t iy;
    ae_int_t iz;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;

    ae_vector_clear(b);

    ae_assert((oldycount>1&&oldzcount>1)&&oldxcount>1, "Spline3DResampleTrilinear: length/width/height less than 1", _state);
    ae_assert((newycount>1&&newzcount>1)&&newxcount>1, "Spline3DResampleTrilinear: length/width/height less than 1", _state);
    ae_assert(a->cnt>=oldycount*oldzcount*oldxcount, "Spline3DResampleTrilinear: length/width/height less than 1", _state);
    ae_vector_set_length(b, newxcount*newycount*newzcount, _state);
    for(i=0; i<=newxcount-1; i++)
    {
        for(j=0; j<=newycount-1; j++)
        {
            for(k=0; k<=newzcount-1; k++)
            {
                ix = i*(oldxcount-1)/(newxcount-1);
                if( ix==oldxcount-1 )
                {
                    ix = oldxcount-2;
                }
                xd = (double)(i*(oldxcount-1))/(double)(newxcount-1)-ix;
                iy = j*(oldycount-1)/(newycount-1);
                if( iy==oldycount-1 )
                {
                    iy = oldycount-2;
                }
                yd = (double)(j*(oldycount-1))/(double)(newycount-1)-iy;
                iz = k*(oldzcount-1)/(newzcount-1);
                if( iz==oldzcount-1 )
                {
                    iz = oldzcount-2;
                }
                zd = (double)(k*(oldzcount-1))/(double)(newzcount-1)-iz;
                c0 = a->ptr.p_double[oldxcount*(oldycount*iz+iy)+ix]*(1-xd)+a->ptr.p_double[oldxcount*(oldycount*iz+iy)+(ix+1)]*xd;
                c1 = a->ptr.p_double[oldxcount*(oldycount*iz+(iy+1))+ix]*(1-xd)+a->ptr.p_double[oldxcount*(oldycount*iz+(iy+1))+(ix+1)]*xd;
                c2 = a->ptr.p_double[oldxcount*(oldycount*(iz+1)+iy)+ix]*(1-xd)+a->ptr.p_double[oldxcount*(oldycount*(iz+1)+iy)+(ix+1)]*xd;
                c3 = a->ptr.p_double[oldxcount*(oldycount*(iz+1)+(iy+1))+ix]*(1-xd)+a->ptr.p_double[oldxcount*(oldycount*(iz+1)+(iy+1))+(ix+1)]*xd;
                c0 = c0*(1-yd)+c1*yd;
                c1 = c2*(1-yd)+c3*yd;
                b->ptr.p_double[newxcount*(newycount*k+j)+i] = c0*(1-zd)+c1*zd;
            }
        }
    }
}


/*************************************************************************
This subroutine builds trilinear vector-valued spline.

INPUT PARAMETERS:
    X   -   spline abscissas,  array[0..N-1]
    Y   -   spline ordinates,  array[0..M-1]
    Z   -   spline applicates, array[0..L-1] 
    F   -   function values, array[0..M*N*L*D-1]:
            * first D elements store D values at (X[0],Y[0],Z[0])
            * next D elements store D values at (X[1],Y[0],Z[0])
            * next D elements store D values at (X[2],Y[0],Z[0])
            * ...
            * next D elements store D values at (X[0],Y[1],Z[0])
            * next D elements store D values at (X[1],Y[1],Z[0])
            * next D elements store D values at (X[2],Y[1],Z[0])
            * ...
            * next D elements store D values at (X[0],Y[0],Z[1])
            * next D elements store D values at (X[1],Y[0],Z[1])
            * next D elements store D values at (X[2],Y[0],Z[1])
            * ...
            * general form - D function values at (X[i],Y[j]) are stored
              at F[D*(N*(M*K+J)+I)...D*(N*(M*K+J)+I)+D-1].
    M,N,
    L   -   grid size, M>=2, N>=2, L>=2
    D   -   vector dimension, D>=1

OUTPUT PARAMETERS:
    C   -   spline interpolant

  -- ALGLIB PROJECT --
     Copyright 26.04.2012 by Bochkanov Sergey
*************************************************************************/
void spline3dbuildtrilinearv(/* Real    */ ae_vector* x,
     ae_int_t n,
     /* Real    */ ae_vector* y,
     ae_int_t m,
     /* Real    */ ae_vector* z,
     ae_int_t l,
     /* Real    */ ae_vector* f,
     ae_int_t d,
     spline3dinterpolant* c,
     ae_state *_state)
{
    double t;
    ae_int_t tblsize;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t i0;
    ae_int_t j0;

    _spline3dinterpolant_clear(c);

    ae_assert(m>=2, "Spline3DBuildTrilinearV: M<2", _state);
    ae_assert(n>=2, "Spline3DBuildTrilinearV: N<2", _state);
    ae_assert(l>=2, "Spline3DBuildTrilinearV: L<2", _state);
    ae_assert(d>=1, "Spline3DBuildTrilinearV: D<1", _state);
    ae_assert((x->cnt>=n&&y->cnt>=m)&&z->cnt>=l, "Spline3DBuildTrilinearV: length of X, Y or Z is too short (Length(X/Y/Z)<N/M/L)", _state);
    ae_assert((isfinitevector(x, n, _state)&&isfinitevector(y, m, _state))&&isfinitevector(z, l, _state), "Spline3DBuildTrilinearV: X, Y or Z contains NaN or Infinite value", _state);
    tblsize = n*m*l*d;
    ae_assert(f->cnt>=tblsize, "Spline3DBuildTrilinearV: length of F is too short (Length(F)<N*M*L*D)", _state);
    ae_assert(isfinitevector(f, tblsize, _state), "Spline3DBuildTrilinearV: F contains NaN or Infinite value", _state);
    
    /*
     * Fill interpolant
     */
    c->k = 1;
    c->n = n;
    c->m = m;
    c->l = l;
    c->d = d;
    c->stype = -1;
    ae_vector_set_length(&c->x, c->n, _state);
    ae_vector_set_length(&c->y, c->m, _state);
    ae_vector_set_length(&c->z, c->l, _state);
    ae_vector_set_length(&c->f, tblsize, _state);
    for(i=0; i<=c->n-1; i++)
    {
        c->x.ptr.p_double[i] = x->ptr.p_double[i];
    }
    for(i=0; i<=c->m-1; i++)
    {
        c->y.ptr.p_double[i] = y->ptr.p_double[i];
    }
    for(i=0; i<=c->l-1; i++)
    {
        c->z.ptr.p_double[i] = z->ptr.p_double[i];
    }
    for(i=0; i<=tblsize-1; i++)
    {
        c->f.ptr.p_double[i] = f->ptr.p_double[i];
    }
    
    /*
     * Sort points:
     *  * sort x;
     *  * sort y;
     *  * sort z.
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
                for(j0=0; j0<=c->l-1; j0++)
                {
                    for(i0=0; i0<=c->d-1; i0++)
                    {
                        t = c->f.ptr.p_double[c->d*(c->n*(c->m*j0+i)+j)+i0];
                        c->f.ptr.p_double[c->d*(c->n*(c->m*j0+i)+j)+i0] = c->f.ptr.p_double[c->d*(c->n*(c->m*j0+i)+k)+i0];
                        c->f.ptr.p_double[c->d*(c->n*(c->m*j0+i)+k)+i0] = t;
                    }
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
                for(j0=0; j0<=c->l-1; j0++)
                {
                    for(i0=0; i0<=c->d-1; i0++)
                    {
                        t = c->f.ptr.p_double[c->d*(c->n*(c->m*j0+i)+j)+i0];
                        c->f.ptr.p_double[c->d*(c->n*(c->m*j0+i)+j)+i0] = c->f.ptr.p_double[c->d*(c->n*(c->m*j0+k)+j)+i0];
                        c->f.ptr.p_double[c->d*(c->n*(c->m*j0+k)+j)+i0] = t;
                    }
                }
            }
            t = c->y.ptr.p_double[i];
            c->y.ptr.p_double[i] = c->y.ptr.p_double[k];
            c->y.ptr.p_double[k] = t;
        }
    }
    for(k=0; k<=c->l-1; k++)
    {
        i = k;
        for(j=i+1; j<=c->l-1; j++)
        {
            if( ae_fp_less(c->z.ptr.p_double[j],c->z.ptr.p_double[i]) )
            {
                i = j;
            }
        }
        if( i!=k )
        {
            for(j=0; j<=c->m-1; j++)
            {
                for(j0=0; j0<=c->n-1; j0++)
                {
                    for(i0=0; i0<=c->d-1; i0++)
                    {
                        t = c->f.ptr.p_double[c->d*(c->n*(c->m*k+j)+j0)+i0];
                        c->f.ptr.p_double[c->d*(c->n*(c->m*k+j)+j0)+i0] = c->f.ptr.p_double[c->d*(c->n*(c->m*i+j)+j0)+i0];
                        c->f.ptr.p_double[c->d*(c->n*(c->m*i+j)+j0)+i0] = t;
                    }
                }
            }
            t = c->z.ptr.p_double[k];
            c->z.ptr.p_double[k] = c->z.ptr.p_double[i];
            c->z.ptr.p_double[i] = t;
        }
    }
}


/*************************************************************************
This subroutine calculates bilinear or bicubic vector-valued spline at the
given point (X,Y,Z).

INPUT PARAMETERS:
    C   -   spline interpolant.
    X, Y,
    Z   -   point
    F   -   output buffer, possibly preallocated array. In case array size
            is large enough to store result, it is not reallocated.  Array
            which is too short will be reallocated

OUTPUT PARAMETERS:
    F   -   array[D] (or larger) which stores function values

  -- ALGLIB PROJECT --
     Copyright 26.04.2012 by Bochkanov Sergey
*************************************************************************/
void spline3dcalcvbuf(spline3dinterpolant* c,
     double x,
     double y,
     double z,
     /* Real    */ ae_vector* f,
     ae_state *_state)
{
    double xd;
    double yd;
    double zd;
    double c0;
    double c1;
    double c2;
    double c3;
    ae_int_t ix;
    ae_int_t iy;
    ae_int_t iz;
    ae_int_t l;
    ae_int_t r;
    ae_int_t h;
    ae_int_t i;


    ae_assert(c->stype==-1||c->stype==-3, "Spline3DCalcVBuf: incorrect C (incorrect parameter C.SType)", _state);
    ae_assert((ae_isfinite(x, _state)&&ae_isfinite(y, _state))&&ae_isfinite(z, _state), "Spline3DCalcVBuf: X, Y or Z contains NaN/Infinite", _state);
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
    ix = l;
    
    /*
     * Binary search in the [ y[0], ..., y[n-2] ] (y[n-1] is not included)
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
    iy = l;
    
    /*
     * Binary search in the [ z[0], ..., z[n-2] ] (z[n-1] is not included)
     */
    l = 0;
    r = c->l-1;
    while(l!=r-1)
    {
        h = (l+r)/2;
        if( ae_fp_greater_eq(c->z.ptr.p_double[h],z) )
        {
            r = h;
        }
        else
        {
            l = h;
        }
    }
    iz = l;
    xd = (x-c->x.ptr.p_double[ix])/(c->x.ptr.p_double[ix+1]-c->x.ptr.p_double[ix]);
    yd = (y-c->y.ptr.p_double[iy])/(c->y.ptr.p_double[iy+1]-c->y.ptr.p_double[iy]);
    zd = (z-c->z.ptr.p_double[iz])/(c->z.ptr.p_double[iz+1]-c->z.ptr.p_double[iz]);
    for(i=0; i<=c->d-1; i++)
    {
        
        /*
         * Trilinear interpolation
         */
        if( c->stype==-1 )
        {
            c0 = c->f.ptr.p_double[c->d*(c->n*(c->m*iz+iy)+ix)+i]*(1-xd)+c->f.ptr.p_double[c->d*(c->n*(c->m*iz+iy)+(ix+1))+i]*xd;
            c1 = c->f.ptr.p_double[c->d*(c->n*(c->m*iz+(iy+1))+ix)+i]*(1-xd)+c->f.ptr.p_double[c->d*(c->n*(c->m*iz+(iy+1))+(ix+1))+i]*xd;
            c2 = c->f.ptr.p_double[c->d*(c->n*(c->m*(iz+1)+iy)+ix)+i]*(1-xd)+c->f.ptr.p_double[c->d*(c->n*(c->m*(iz+1)+iy)+(ix+1))+i]*xd;
            c3 = c->f.ptr.p_double[c->d*(c->n*(c->m*(iz+1)+(iy+1))+ix)+i]*(1-xd)+c->f.ptr.p_double[c->d*(c->n*(c->m*(iz+1)+(iy+1))+(ix+1))+i]*xd;
            c0 = c0*(1-yd)+c1*yd;
            c1 = c2*(1-yd)+c3*yd;
            f->ptr.p_double[i] = c0*(1-zd)+c1*zd;
        }
    }
}


/*************************************************************************
This subroutine calculates trilinear or tricubic vector-valued spline at the
given point (X,Y,Z).

INPUT PARAMETERS:
    C   -   spline interpolant.
    X, Y,
    Z   -   point

OUTPUT PARAMETERS:
    F   -   array[D] which stores function values.  F is out-parameter and
            it  is  reallocated  after  call to this function. In case you
            want  to    reuse  previously  allocated  F,   you   may   use
            Spline2DCalcVBuf(),  which  reallocates  F only when it is too
            small.

  -- ALGLIB PROJECT --
     Copyright 26.04.2012 by Bochkanov Sergey
*************************************************************************/
void spline3dcalcv(spline3dinterpolant* c,
     double x,
     double y,
     double z,
     /* Real    */ ae_vector* f,
     ae_state *_state)
{

    ae_vector_clear(f);

    ae_assert(c->stype==-1||c->stype==-3, "Spline3DCalcV: incorrect C (incorrect parameter C.SType)", _state);
    ae_assert((ae_isfinite(x, _state)&&ae_isfinite(y, _state))&&ae_isfinite(z, _state), "Spline3DCalcV: X=NaN/Infinite, Y=NaN/Infinite or Z=NaN/Infinite", _state);
    ae_vector_set_length(f, c->d, _state);
    spline3dcalcvbuf(c, x, y, z, f, _state);
}


/*************************************************************************
This subroutine unpacks tri-dimensional spline into the coefficients table

INPUT PARAMETERS:
    C   -   spline interpolant.

Result:
    N   -   grid size (X)
    M   -   grid size (Y)
    L   -   grid size (Z)
    D   -   number of components
    SType-  spline type. Currently, only one spline type is supported:
            trilinear spline, as indicated by SType=1.
    Tbl -   spline coefficients: [0..(N-1)*(M-1)*(L-1)*D-1, 0..13].
            For T=0..D-1 (component index), I = 0...N-2 (x index),
            J=0..M-2 (y index), K=0..L-2 (z index):
                Q := T + I*D + J*D*(N-1) + K*D*(N-1)*(M-1),
                
                Q-th row stores decomposition for T-th component of the
                vector-valued function
                
                Tbl[Q,0] = X[i]
                Tbl[Q,1] = X[i+1]
                Tbl[Q,2] = Y[j]
                Tbl[Q,3] = Y[j+1]
                Tbl[Q,4] = Z[k]
                Tbl[Q,5] = Z[k+1]
                
                Tbl[Q,6] = C000
                Tbl[Q,7] = C100
                Tbl[Q,8] = C010
                Tbl[Q,9] = C110
                Tbl[Q,10]= C001
                Tbl[Q,11]= C101
                Tbl[Q,12]= C011
                Tbl[Q,13]= C111
            On each grid square spline is equals to:
                S(x) = SUM(c[i,j,k]*(x^i)*(y^j)*(z^k), i=0..1, j=0..1, k=0..1)
                t = x-x[j]
                u = y-y[i]
                v = z-z[k]
            
            NOTE: format of Tbl is given for SType=1. Future versions of
                  ALGLIB can use different formats for different values of
                  SType.

  -- ALGLIB PROJECT --
     Copyright 26.04.2012 by Bochkanov Sergey
*************************************************************************/
void spline3dunpackv(spline3dinterpolant* c,
     ae_int_t* n,
     ae_int_t* m,
     ae_int_t* l,
     ae_int_t* d,
     ae_int_t* stype,
     /* Real    */ ae_matrix* tbl,
     ae_state *_state)
{
    ae_int_t p;
    ae_int_t ci;
    ae_int_t cj;
    ae_int_t ck;
    double du;
    double dv;
    double dw;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t di;
    ae_int_t i0;

    *n = 0;
    *m = 0;
    *l = 0;
    *d = 0;
    *stype = 0;
    ae_matrix_clear(tbl);

    ae_assert(c->stype==-1, "Spline3DUnpackV: incorrect C (incorrect parameter C.SType)", _state);
    *n = c->n;
    *m = c->m;
    *l = c->l;
    *d = c->d;
    *stype = ae_iabs(c->stype, _state);
    ae_matrix_set_length(tbl, (*n-1)*(*m-1)*(*l-1)*(*d), 14, _state);
    
    /*
     * Fill
     */
    for(i=0; i<=*n-2; i++)
    {
        for(j=0; j<=*m-2; j++)
        {
            for(k=0; k<=*l-2; k++)
            {
                for(di=0; di<=*d-1; di++)
                {
                    p = *d*((*n-1)*((*m-1)*k+j)+i)+di;
                    tbl->ptr.pp_double[p][0] = c->x.ptr.p_double[i];
                    tbl->ptr.pp_double[p][1] = c->x.ptr.p_double[i+1];
                    tbl->ptr.pp_double[p][2] = c->y.ptr.p_double[j];
                    tbl->ptr.pp_double[p][3] = c->y.ptr.p_double[j+1];
                    tbl->ptr.pp_double[p][4] = c->z.ptr.p_double[k];
                    tbl->ptr.pp_double[p][5] = c->z.ptr.p_double[k+1];
                    du = 1/(tbl->ptr.pp_double[p][1]-tbl->ptr.pp_double[p][0]);
                    dv = 1/(tbl->ptr.pp_double[p][3]-tbl->ptr.pp_double[p][2]);
                    dw = 1/(tbl->ptr.pp_double[p][5]-tbl->ptr.pp_double[p][4]);
                    
                    /*
                     * Trilinear interpolation
                     */
                    if( c->stype==-1 )
                    {
                        for(i0=6; i0<=13; i0++)
                        {
                            tbl->ptr.pp_double[p][i0] = (double)(0);
                        }
                        tbl->ptr.pp_double[p][6+2*(2*0+0)+0] = c->f.ptr.p_double[*d*(*n*(*m*k+j)+i)+di];
                        tbl->ptr.pp_double[p][6+2*(2*0+0)+1] = c->f.ptr.p_double[*d*(*n*(*m*k+j)+(i+1))+di]-c->f.ptr.p_double[*d*(*n*(*m*k+j)+i)+di];
                        tbl->ptr.pp_double[p][6+2*(2*0+1)+0] = c->f.ptr.p_double[*d*(*n*(*m*k+(j+1))+i)+di]-c->f.ptr.p_double[*d*(*n*(*m*k+j)+i)+di];
                        tbl->ptr.pp_double[p][6+2*(2*0+1)+1] = c->f.ptr.p_double[*d*(*n*(*m*k+(j+1))+(i+1))+di]-c->f.ptr.p_double[*d*(*n*(*m*k+(j+1))+i)+di]-c->f.ptr.p_double[*d*(*n*(*m*k+j)+(i+1))+di]+c->f.ptr.p_double[*d*(*n*(*m*k+j)+i)+di];
                        tbl->ptr.pp_double[p][6+2*(2*1+0)+0] = c->f.ptr.p_double[*d*(*n*(*m*(k+1)+j)+i)+di]-c->f.ptr.p_double[*d*(*n*(*m*k+j)+i)+di];
                        tbl->ptr.pp_double[p][6+2*(2*1+0)+1] = c->f.ptr.p_double[*d*(*n*(*m*(k+1)+j)+(i+1))+di]-c->f.ptr.p_double[*d*(*n*(*m*(k+1)+j)+i)+di]-c->f.ptr.p_double[*d*(*n*(*m*k+j)+(i+1))+di]+c->f.ptr.p_double[*d*(*n*(*m*k+j)+i)+di];
                        tbl->ptr.pp_double[p][6+2*(2*1+1)+0] = c->f.ptr.p_double[*d*(*n*(*m*(k+1)+(j+1))+i)+di]-c->f.ptr.p_double[*d*(*n*(*m*(k+1)+j)+i)+di]-c->f.ptr.p_double[*d*(*n*(*m*k+(j+1))+i)+di]+c->f.ptr.p_double[*d*(*n*(*m*k+j)+i)+di];
                        tbl->ptr.pp_double[p][6+2*(2*1+1)+1] = c->f.ptr.p_double[*d*(*n*(*m*(k+1)+(j+1))+(i+1))+di]-c->f.ptr.p_double[*d*(*n*(*m*(k+1)+(j+1))+i)+di]-c->f.ptr.p_double[*d*(*n*(*m*(k+1)+j)+(i+1))+di]+c->f.ptr.p_double[*d*(*n*(*m*(k+1)+j)+i)+di]-c->f.ptr.p_double[*d*(*n*(*m*k+(j+1))+(i+1))+di]+c->f.ptr.p_double[*d*(*n*(*m*k+(j+1))+i)+di]+c->f.ptr.p_double[*d*(*n*(*m*k+j)+(i+1))+di]-c->f.ptr.p_double[*d*(*n*(*m*k+j)+i)+di];
                    }
                    
                    /*
                     * Rescale Cij
                     */
                    for(ci=0; ci<=1; ci++)
                    {
                        for(cj=0; cj<=1; cj++)
                        {
                            for(ck=0; ck<=1; ck++)
                            {
                                tbl->ptr.pp_double[p][6+2*(2*ck+cj)+ci] = tbl->ptr.pp_double[p][6+2*(2*ck+cj)+ci]*ae_pow(du, (double)(ci), _state)*ae_pow(dv, (double)(cj), _state)*ae_pow(dw, (double)(ck), _state);
                            }
                        }
                    }
                }
            }
        }
    }
}


/*************************************************************************
This subroutine calculates the value of the trilinear(or tricubic;possible
will be later) spline  at the given point X(and its derivatives; possible
will be later).

INPUT PARAMETERS:
    C       -   spline interpolant.
    X, Y, Z -   point

OUTPUT PARAMETERS:
    F   -   S(x,y,z)
    FX  -   dS(x,y,z)/dX
    FY  -   dS(x,y,z)/dY
    FXY -   d2S(x,y,z)/dXdY

  -- ALGLIB PROJECT --
     Copyright 26.04.2012 by Bochkanov Sergey
*************************************************************************/
static void spline3d_spline3ddiff(spline3dinterpolant* c,
     double x,
     double y,
     double z,
     double* f,
     double* fx,
     double* fy,
     double* fxy,
     ae_state *_state)
{
    double xd;
    double yd;
    double zd;
    double c0;
    double c1;
    double c2;
    double c3;
    ae_int_t ix;
    ae_int_t iy;
    ae_int_t iz;
    ae_int_t l;
    ae_int_t r;
    ae_int_t h;

    *f = 0;
    *fx = 0;
    *fy = 0;
    *fxy = 0;

    ae_assert(c->stype==-1||c->stype==-3, "Spline3DDiff: incorrect C (incorrect parameter C.SType)", _state);
    ae_assert(ae_isfinite(x, _state)&&ae_isfinite(y, _state), "Spline3DDiff: X or Y contains NaN or Infinite value", _state);
    
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
    ix = l;
    
    /*
     * Binary search in the [ y[0], ..., y[n-2] ] (y[n-1] is not included)
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
    iy = l;
    
    /*
     * Binary search in the [ z[0], ..., z[n-2] ] (z[n-1] is not included)
     */
    l = 0;
    r = c->l-1;
    while(l!=r-1)
    {
        h = (l+r)/2;
        if( ae_fp_greater_eq(c->z.ptr.p_double[h],z) )
        {
            r = h;
        }
        else
        {
            l = h;
        }
    }
    iz = l;
    xd = (x-c->x.ptr.p_double[ix])/(c->x.ptr.p_double[ix+1]-c->x.ptr.p_double[ix]);
    yd = (y-c->y.ptr.p_double[iy])/(c->y.ptr.p_double[iy+1]-c->y.ptr.p_double[iy]);
    zd = (z-c->z.ptr.p_double[iz])/(c->z.ptr.p_double[iz+1]-c->z.ptr.p_double[iz]);
    
    /*
     * Trilinear interpolation
     */
    if( c->stype==-1 )
    {
        c0 = c->f.ptr.p_double[c->n*(c->m*iz+iy)+ix]*(1-xd)+c->f.ptr.p_double[c->n*(c->m*iz+iy)+(ix+1)]*xd;
        c1 = c->f.ptr.p_double[c->n*(c->m*iz+(iy+1))+ix]*(1-xd)+c->f.ptr.p_double[c->n*(c->m*iz+(iy+1))+(ix+1)]*xd;
        c2 = c->f.ptr.p_double[c->n*(c->m*(iz+1)+iy)+ix]*(1-xd)+c->f.ptr.p_double[c->n*(c->m*(iz+1)+iy)+(ix+1)]*xd;
        c3 = c->f.ptr.p_double[c->n*(c->m*(iz+1)+(iy+1))+ix]*(1-xd)+c->f.ptr.p_double[c->n*(c->m*(iz+1)+(iy+1))+(ix+1)]*xd;
        c0 = c0*(1-yd)+c1*yd;
        c1 = c2*(1-yd)+c3*yd;
        *f = c0*(1-zd)+c1*zd;
    }
}


void _spline3dinterpolant_init(void* _p, ae_state *_state)
{
    spline3dinterpolant *p = (spline3dinterpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->x, 0, DT_REAL, _state);
    ae_vector_init(&p->y, 0, DT_REAL, _state);
    ae_vector_init(&p->z, 0, DT_REAL, _state);
    ae_vector_init(&p->f, 0, DT_REAL, _state);
}


void _spline3dinterpolant_init_copy(void* _dst, void* _src, ae_state *_state)
{
    spline3dinterpolant *dst = (spline3dinterpolant*)_dst;
    spline3dinterpolant *src = (spline3dinterpolant*)_src;
    dst->k = src->k;
    dst->stype = src->stype;
    dst->n = src->n;
    dst->m = src->m;
    dst->l = src->l;
    dst->d = src->d;
    ae_vector_init_copy(&dst->x, &src->x, _state);
    ae_vector_init_copy(&dst->y, &src->y, _state);
    ae_vector_init_copy(&dst->z, &src->z, _state);
    ae_vector_init_copy(&dst->f, &src->f, _state);
}


void _spline3dinterpolant_clear(void* _p)
{
    spline3dinterpolant *p = (spline3dinterpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->x);
    ae_vector_clear(&p->y);
    ae_vector_clear(&p->z);
    ae_vector_clear(&p->f);
}


void _spline3dinterpolant_destroy(void* _p)
{
    spline3dinterpolant *p = (spline3dinterpolant*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->x);
    ae_vector_destroy(&p->y);
    ae_vector_destroy(&p->z);
    ae_vector_destroy(&p->f);
}


/*$ End $*/
