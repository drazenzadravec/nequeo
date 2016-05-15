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
#include "inverseupdate.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Inverse matrix update by the Sherman-Morrison formula

The algorithm updates matrix A^-1 when adding a number to an element
of matrix A.

Input parameters:
    InvA    -   inverse of matrix A.
                Array whose indexes range within [0..N-1, 0..N-1].
    N       -   size of matrix A.
    UpdRow  -   row where the element to be updated is stored.
    UpdColumn - column where the element to be updated is stored.
    UpdVal  -   a number to be added to the element.


Output parameters:
    InvA    -   inverse of modified matrix A.

  -- ALGLIB --
     Copyright 2005 by Bochkanov Sergey
*************************************************************************/
void rmatrixinvupdatesimple(/* Real    */ ae_matrix* inva,
     ae_int_t n,
     ae_int_t updrow,
     ae_int_t updcolumn,
     double updval,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector t1;
    ae_vector t2;
    ae_int_t i;
    double lambdav;
    double vt;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&t1, 0, DT_REAL, _state);
    ae_vector_init(&t2, 0, DT_REAL, _state);

    ae_assert(updrow>=0&&updrow<n, "RMatrixInvUpdateSimple: incorrect UpdRow!", _state);
    ae_assert(updcolumn>=0&&updcolumn<n, "RMatrixInvUpdateSimple: incorrect UpdColumn!", _state);
    ae_vector_set_length(&t1, n-1+1, _state);
    ae_vector_set_length(&t2, n-1+1, _state);
    
    /*
     * T1 = InvA * U
     */
    ae_v_move(&t1.ptr.p_double[0], 1, &inva->ptr.pp_double[0][updrow], inva->stride, ae_v_len(0,n-1));
    
    /*
     * T2 = v*InvA
     */
    ae_v_move(&t2.ptr.p_double[0], 1, &inva->ptr.pp_double[updcolumn][0], 1, ae_v_len(0,n-1));
    
    /*
     * Lambda = v * InvA * U
     */
    lambdav = updval*inva->ptr.pp_double[updcolumn][updrow];
    
    /*
     * InvA = InvA - correction
     */
    for(i=0; i<=n-1; i++)
    {
        vt = updval*t1.ptr.p_double[i];
        vt = vt/(1+lambdav);
        ae_v_subd(&inva->ptr.pp_double[i][0], 1, &t2.ptr.p_double[0], 1, ae_v_len(0,n-1), vt);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Inverse matrix update by the Sherman-Morrison formula

The algorithm updates matrix A^-1 when adding a vector to a row
of matrix A.

Input parameters:
    InvA    -   inverse of matrix A.
                Array whose indexes range within [0..N-1, 0..N-1].
    N       -   size of matrix A.
    UpdRow  -   the row of A whose vector V was added.
                0 <= Row <= N-1
    V       -   the vector to be added to a row.
                Array whose index ranges within [0..N-1].

Output parameters:
    InvA    -   inverse of modified matrix A.

  -- ALGLIB --
     Copyright 2005 by Bochkanov Sergey
*************************************************************************/
void rmatrixinvupdaterow(/* Real    */ ae_matrix* inva,
     ae_int_t n,
     ae_int_t updrow,
     /* Real    */ ae_vector* v,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector t1;
    ae_vector t2;
    ae_int_t i;
    ae_int_t j;
    double lambdav;
    double vt;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&t1, 0, DT_REAL, _state);
    ae_vector_init(&t2, 0, DT_REAL, _state);

    ae_vector_set_length(&t1, n-1+1, _state);
    ae_vector_set_length(&t2, n-1+1, _state);
    
    /*
     * T1 = InvA * U
     */
    ae_v_move(&t1.ptr.p_double[0], 1, &inva->ptr.pp_double[0][updrow], inva->stride, ae_v_len(0,n-1));
    
    /*
     * T2 = v*InvA
     * Lambda = v * InvA * U
     */
    for(j=0; j<=n-1; j++)
    {
        vt = ae_v_dotproduct(&v->ptr.p_double[0], 1, &inva->ptr.pp_double[0][j], inva->stride, ae_v_len(0,n-1));
        t2.ptr.p_double[j] = vt;
    }
    lambdav = t2.ptr.p_double[updrow];
    
    /*
     * InvA = InvA - correction
     */
    for(i=0; i<=n-1; i++)
    {
        vt = t1.ptr.p_double[i]/(1+lambdav);
        ae_v_subd(&inva->ptr.pp_double[i][0], 1, &t2.ptr.p_double[0], 1, ae_v_len(0,n-1), vt);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Inverse matrix update by the Sherman-Morrison formula

The algorithm updates matrix A^-1 when adding a vector to a column
of matrix A.

Input parameters:
    InvA        -   inverse of matrix A.
                    Array whose indexes range within [0..N-1, 0..N-1].
    N           -   size of matrix A.
    UpdColumn   -   the column of A whose vector U was added.
                    0 <= UpdColumn <= N-1
    U           -   the vector to be added to a column.
                    Array whose index ranges within [0..N-1].

Output parameters:
    InvA        -   inverse of modified matrix A.

  -- ALGLIB --
     Copyright 2005 by Bochkanov Sergey
*************************************************************************/
void rmatrixinvupdatecolumn(/* Real    */ ae_matrix* inva,
     ae_int_t n,
     ae_int_t updcolumn,
     /* Real    */ ae_vector* u,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector t1;
    ae_vector t2;
    ae_int_t i;
    double lambdav;
    double vt;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&t1, 0, DT_REAL, _state);
    ae_vector_init(&t2, 0, DT_REAL, _state);

    ae_vector_set_length(&t1, n-1+1, _state);
    ae_vector_set_length(&t2, n-1+1, _state);
    
    /*
     * T1 = InvA * U
     * Lambda = v * InvA * U
     */
    for(i=0; i<=n-1; i++)
    {
        vt = ae_v_dotproduct(&inva->ptr.pp_double[i][0], 1, &u->ptr.p_double[0], 1, ae_v_len(0,n-1));
        t1.ptr.p_double[i] = vt;
    }
    lambdav = t1.ptr.p_double[updcolumn];
    
    /*
     * T2 = v*InvA
     */
    ae_v_move(&t2.ptr.p_double[0], 1, &inva->ptr.pp_double[updcolumn][0], 1, ae_v_len(0,n-1));
    
    /*
     * InvA = InvA - correction
     */
    for(i=0; i<=n-1; i++)
    {
        vt = t1.ptr.p_double[i]/(1+lambdav);
        ae_v_subd(&inva->ptr.pp_double[i][0], 1, &t2.ptr.p_double[0], 1, ae_v_len(0,n-1), vt);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Inverse matrix update by the Sherman-Morrison formula

The algorithm computes the inverse of matrix A+u*v’ by using the given matrix
A^-1 and the vectors u and v.

Input parameters:
    InvA    -   inverse of matrix A.
                Array whose indexes range within [0..N-1, 0..N-1].
    N       -   size of matrix A.
    U       -   the vector modifying the matrix.
                Array whose index ranges within [0..N-1].
    V       -   the vector modifying the matrix.
                Array whose index ranges within [0..N-1].

Output parameters:
    InvA - inverse of matrix A + u*v'.

  -- ALGLIB --
     Copyright 2005 by Bochkanov Sergey
*************************************************************************/
void rmatrixinvupdateuv(/* Real    */ ae_matrix* inva,
     ae_int_t n,
     /* Real    */ ae_vector* u,
     /* Real    */ ae_vector* v,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector t1;
    ae_vector t2;
    ae_int_t i;
    ae_int_t j;
    double lambdav;
    double vt;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&t1, 0, DT_REAL, _state);
    ae_vector_init(&t2, 0, DT_REAL, _state);

    ae_vector_set_length(&t1, n-1+1, _state);
    ae_vector_set_length(&t2, n-1+1, _state);
    
    /*
     * T1 = InvA * U
     * Lambda = v * T1
     */
    for(i=0; i<=n-1; i++)
    {
        vt = ae_v_dotproduct(&inva->ptr.pp_double[i][0], 1, &u->ptr.p_double[0], 1, ae_v_len(0,n-1));
        t1.ptr.p_double[i] = vt;
    }
    lambdav = ae_v_dotproduct(&v->ptr.p_double[0], 1, &t1.ptr.p_double[0], 1, ae_v_len(0,n-1));
    
    /*
     * T2 = v*InvA
     */
    for(j=0; j<=n-1; j++)
    {
        vt = ae_v_dotproduct(&v->ptr.p_double[0], 1, &inva->ptr.pp_double[0][j], inva->stride, ae_v_len(0,n-1));
        t2.ptr.p_double[j] = vt;
    }
    
    /*
     * InvA = InvA - correction
     */
    for(i=0; i<=n-1; i++)
    {
        vt = t1.ptr.p_double[i]/(1+lambdav);
        ae_v_subd(&inva->ptr.pp_double[i][0], 1, &t2.ptr.p_double[0], 1, ae_v_len(0,n-1), vt);
    }
    ae_frame_leave(_state);
}


/*$ End $*/
