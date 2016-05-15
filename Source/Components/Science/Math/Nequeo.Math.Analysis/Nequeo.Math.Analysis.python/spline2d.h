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

#ifndef _spline2d_h
#define _spline2d_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "tsort.h"
#include "spline1d.h"


/*$ Declarations $*/


/*************************************************************************
2-dimensional spline inteprolant
*************************************************************************/
typedef struct
{
    ae_int_t k;
    ae_int_t stype;
    ae_int_t n;
    ae_int_t m;
    ae_int_t d;
    ae_vector x;
    ae_vector y;
    ae_vector f;
} spline2dinterpolant;


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);
void _spline2dinterpolant_init(void* _p, ae_state *_state);
void _spline2dinterpolant_init_copy(void* _dst, void* _src, ae_state *_state);
void _spline2dinterpolant_clear(void* _p);
void _spline2dinterpolant_destroy(void* _p);


/*$ End $*/
#endif

