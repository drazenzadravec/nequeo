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

#ifndef _trlinsolve_h
#define _trlinsolve_h

#include "aenv.h"
#include "ialglib.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Utility subroutine performing the "safe" solution of system of linear
equations with triangular coefficient matrices.

The subroutine uses scaling and solves the scaled system A*x=s*b (where  s
is  a  scalar  value)  instead  of  A*x=b,  choosing  s  so  that x can be
represented by a floating-point number. The closer the system  gets  to  a
singular, the less s is. If the system is singular, s=0 and x contains the
non-trivial solution of equation A*x=0.

The feature of an algorithm is that it could not cause an  overflow  or  a
division by zero regardless of the matrix used as the input.

The algorithm can solve systems of equations with  upper/lower  triangular
matrices,  with/without unit diagonal, and systems of type A*x=b or A'*x=b
(where A' is a transposed matrix A).

Input parameters:
    A       -   system matrix. Array whose indexes range within [0..N-1, 0..N-1].
    N       -   size of matrix A.
    X       -   right-hand member of a system.
                Array whose index ranges within [0..N-1].
    IsUpper -   matrix type. If it is True, the system matrix is the upper
                triangular and is located in  the  corresponding  part  of
                matrix A.
    Trans   -   problem type. If it is True, the problem to be  solved  is
                A'*x=b, otherwise it is A*x=b.
    Isunit  -   matrix type. If it is True, the system matrix has  a  unit
                diagonal (the elements on the main diagonal are  not  used
                in the calculation process), otherwise the matrix is considered
                to be a general triangular matrix.

Output parameters:
    X       -   solution. Array whose index ranges within [0..N-1].
    S       -   scaling factor.

  -- LAPACK auxiliary routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     June 30, 1992
*************************************************************************/
void rmatrixtrsafesolve(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     double* s,
     ae_bool isupper,
     ae_bool istrans,
     ae_bool isunit,
     ae_state *_state);


/*************************************************************************
Obsolete 1-based subroutine.
See RMatrixTRSafeSolve for 0-based replacement.
*************************************************************************/
void safesolvetriangular(/* Real    */ ae_matrix* a,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     double* s,
     ae_bool isupper,
     ae_bool istrans,
     ae_bool isunit,
     ae_bool normin,
     /* Real    */ ae_vector* cnorm,
     ae_state *_state);


/*$ End $*/
#endif

