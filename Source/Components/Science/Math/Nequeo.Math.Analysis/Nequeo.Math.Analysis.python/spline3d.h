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

#ifndef _spline3d_h
#define _spline3d_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "tsort.h"
#include "spline1d.h"


/*$ Declarations $*/


/*************************************************************************
3-dimensional spline inteprolant
*************************************************************************/
typedef struct
{
    ae_int_t k;
    ae_int_t stype;
    ae_int_t n;
    ae_int_t m;
    ae_int_t l;
    ae_int_t d;
    ae_vector x;
    ae_vector y;
    ae_vector z;
    ae_vector f;
} spline3dinterpolant;


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);
void _spline3dinterpolant_init(void* _p, ae_state *_state);
void _spline3dinterpolant_init_copy(void* _dst, void* _src, ae_state *_state);
void _spline3dinterpolant_clear(void* _p);
void _spline3dinterpolant_destroy(void* _p);


/*$ End $*/
#endif

