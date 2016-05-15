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
#include "ablas.h"


/*$ Declarations $*/
static ae_int_t ablas_rgemmparallelsize = 64;
static ae_int_t ablas_cgemmparallelsize = 64;
static void ablas_ablasinternalsplitlength(ae_int_t n,
     ae_int_t nb,
     ae_int_t* n1,
     ae_int_t* n2,
     ae_state *_state);
static void ablas_cmatrixrighttrsm2(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Complex */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state);
static void ablas_cmatrixlefttrsm2(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Complex */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state);
static void ablas_rmatrixrighttrsm2(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Real    */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state);
static void ablas_rmatrixlefttrsm2(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Real    */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state);
static void ablas_cmatrixherk2(ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     double beta,
     /* Complex */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_bool isupper,
     ae_state *_state);
static void ablas_rmatrixsyrk2(ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     double beta,
     /* Real    */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_bool isupper,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Splits matrix length in two parts, left part should match ABLAS block size

INPUT PARAMETERS
    A   -   real matrix, is passed to ensure that we didn't split
            complex matrix using real splitting subroutine.
            matrix itself is not changed.
    N   -   length, N>0

OUTPUT PARAMETERS
    N1  -   length
    N2  -   length

N1+N2=N, N1>=N2, N2 may be zero

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
void ablassplitlength(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_int_t* n1,
     ae_int_t* n2,
     ae_state *_state)
{

    *n1 = 0;
    *n2 = 0;

    if( n>ablasblocksize(a, _state) )
    {
        ablas_ablasinternalsplitlength(n, ablasblocksize(a, _state), n1, n2, _state);
    }
    else
    {
        ablas_ablasinternalsplitlength(n, ablasmicroblocksize(_state), n1, n2, _state);
    }
}


/*************************************************************************
Complex ABLASSplitLength

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
void ablascomplexsplitlength(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_int_t* n1,
     ae_int_t* n2,
     ae_state *_state)
{

    *n1 = 0;
    *n2 = 0;

    if( n>ablascomplexblocksize(a, _state) )
    {
        ablas_ablasinternalsplitlength(n, ablascomplexblocksize(a, _state), n1, n2, _state);
    }
    else
    {
        ablas_ablasinternalsplitlength(n, ablasmicroblocksize(_state), n1, n2, _state);
    }
}


/*************************************************************************
Returns block size - subdivision size where  cache-oblivious  soubroutines
switch to the optimized kernel.

INPUT PARAMETERS
    A   -   real matrix, is passed to ensure that we didn't split
            complex matrix using real splitting subroutine.
            matrix itself is not changed.

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
ae_int_t ablasblocksize(/* Real    */ ae_matrix* a, ae_state *_state)
{
    ae_int_t result;


    result = 32;
    return result;
}


/*************************************************************************
Block size for complex subroutines.

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
ae_int_t ablascomplexblocksize(/* Complex */ ae_matrix* a,
     ae_state *_state)
{
    ae_int_t result;


    result = 24;
    return result;
}


/*************************************************************************
Microblock size

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
ae_int_t ablasmicroblocksize(ae_state *_state)
{
    ae_int_t result;


    result = 8;
    return result;
}


/*************************************************************************
Cache-oblivous complex "copy-and-transpose"

Input parameters:
    M   -   number of rows
    N   -   number of columns
    A   -   source matrix, MxN submatrix is copied and transposed
    IA  -   submatrix offset (row index)
    JA  -   submatrix offset (column index)
    B   -   destination matrix, must be large enough to store result
    IB  -   submatrix offset (row index)
    JB  -   submatrix offset (column index)
*************************************************************************/
void cmatrixtranspose(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     /* Complex */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t s1;
    ae_int_t s2;


    if( m<=2*ablascomplexblocksize(a, _state)&&n<=2*ablascomplexblocksize(a, _state) )
    {
        
        /*
         * base case
         */
        for(i=0; i<=m-1; i++)
        {
            ae_v_cmove(&b->ptr.pp_complex[ib][jb+i], b->stride, &a->ptr.pp_complex[ia+i][ja], 1, "N", ae_v_len(ib,ib+n-1));
        }
    }
    else
    {
        
        /*
         * Cache-oblivious recursion
         */
        if( m>n )
        {
            ablascomplexsplitlength(a, m, &s1, &s2, _state);
            cmatrixtranspose(s1, n, a, ia, ja, b, ib, jb, _state);
            cmatrixtranspose(s2, n, a, ia+s1, ja, b, ib, jb+s1, _state);
        }
        else
        {
            ablascomplexsplitlength(a, n, &s1, &s2, _state);
            cmatrixtranspose(m, s1, a, ia, ja, b, ib, jb, _state);
            cmatrixtranspose(m, s2, a, ia, ja+s1, b, ib+s1, jb, _state);
        }
    }
}


/*************************************************************************
Cache-oblivous real "copy-and-transpose"

Input parameters:
    M   -   number of rows
    N   -   number of columns
    A   -   source matrix, MxN submatrix is copied and transposed
    IA  -   submatrix offset (row index)
    JA  -   submatrix offset (column index)
    B   -   destination matrix, must be large enough to store result
    IB  -   submatrix offset (row index)
    JB  -   submatrix offset (column index)
*************************************************************************/
void rmatrixtranspose(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     /* Real    */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t s1;
    ae_int_t s2;


    if( m<=2*ablasblocksize(a, _state)&&n<=2*ablasblocksize(a, _state) )
    {
        
        /*
         * base case
         */
        for(i=0; i<=m-1; i++)
        {
            ae_v_move(&b->ptr.pp_double[ib][jb+i], b->stride, &a->ptr.pp_double[ia+i][ja], 1, ae_v_len(ib,ib+n-1));
        }
    }
    else
    {
        
        /*
         * Cache-oblivious recursion
         */
        if( m>n )
        {
            ablassplitlength(a, m, &s1, &s2, _state);
            rmatrixtranspose(s1, n, a, ia, ja, b, ib, jb, _state);
            rmatrixtranspose(s2, n, a, ia+s1, ja, b, ib, jb+s1, _state);
        }
        else
        {
            ablassplitlength(a, n, &s1, &s2, _state);
            rmatrixtranspose(m, s1, a, ia, ja, b, ib, jb, _state);
            rmatrixtranspose(m, s2, a, ia, ja+s1, b, ib+s1, jb, _state);
        }
    }
}


/*************************************************************************
This code enforces symmetricy of the matrix by copying Upper part to lower
one (or vice versa).

INPUT PARAMETERS:
    A   -   matrix
    N   -   number of rows/columns
    IsUpper - whether we want to copy upper triangle to lower one (True)
            or vice versa (False).
*************************************************************************/
void rmatrixenforcesymmetricity(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    if( isupper )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=i+1; j<=n-1; j++)
            {
                a->ptr.pp_double[j][i] = a->ptr.pp_double[i][j];
            }
        }
    }
    else
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=i+1; j<=n-1; j++)
            {
                a->ptr.pp_double[i][j] = a->ptr.pp_double[j][i];
            }
        }
    }
}


/*************************************************************************
Copy

Input parameters:
    M   -   number of rows
    N   -   number of columns
    A   -   source matrix, MxN submatrix is copied and transposed
    IA  -   submatrix offset (row index)
    JA  -   submatrix offset (column index)
    B   -   destination matrix, must be large enough to store result
    IB  -   submatrix offset (row index)
    JB  -   submatrix offset (column index)
*************************************************************************/
void cmatrixcopy(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     /* Complex */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     ae_state *_state)
{
    ae_int_t i;


    if( m==0||n==0 )
    {
        return;
    }
    for(i=0; i<=m-1; i++)
    {
        ae_v_cmove(&b->ptr.pp_complex[ib+i][jb], 1, &a->ptr.pp_complex[ia+i][ja], 1, "N", ae_v_len(jb,jb+n-1));
    }
}


/*************************************************************************
Copy

Input parameters:
    M   -   number of rows
    N   -   number of columns
    A   -   source matrix, MxN submatrix is copied and transposed
    IA  -   submatrix offset (row index)
    JA  -   submatrix offset (column index)
    B   -   destination matrix, must be large enough to store result
    IB  -   submatrix offset (row index)
    JB  -   submatrix offset (column index)
*************************************************************************/
void rmatrixcopy(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     /* Real    */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     ae_state *_state)
{
    ae_int_t i;


    if( m==0||n==0 )
    {
        return;
    }
    for(i=0; i<=m-1; i++)
    {
        ae_v_move(&b->ptr.pp_double[ib+i][jb], 1, &a->ptr.pp_double[ia+i][ja], 1, ae_v_len(jb,jb+n-1));
    }
}


/*************************************************************************
Rank-1 correction: A := A + u*v'

INPUT PARAMETERS:
    M   -   number of rows
    N   -   number of columns
    A   -   target matrix, MxN submatrix is updated
    IA  -   submatrix offset (row index)
    JA  -   submatrix offset (column index)
    U   -   vector #1
    IU  -   subvector offset
    V   -   vector #2
    IV  -   subvector offset
*************************************************************************/
void cmatrixrank1(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     /* Complex */ ae_vector* u,
     ae_int_t iu,
     /* Complex */ ae_vector* v,
     ae_int_t iv,
     ae_state *_state)
{
    ae_int_t i;
    ae_complex s;


    if( m==0||n==0 )
    {
        return;
    }
    if( cmatrixrank1f(m, n, a, ia, ja, u, iu, v, iv, _state) )
    {
        return;
    }
    for(i=0; i<=m-1; i++)
    {
        s = u->ptr.p_complex[iu+i];
        ae_v_caddc(&a->ptr.pp_complex[ia+i][ja], 1, &v->ptr.p_complex[iv], 1, "N", ae_v_len(ja,ja+n-1), s);
    }
}


/*************************************************************************
Rank-1 correction: A := A + u*v'

INPUT PARAMETERS:
    M   -   number of rows
    N   -   number of columns
    A   -   target matrix, MxN submatrix is updated
    IA  -   submatrix offset (row index)
    JA  -   submatrix offset (column index)
    U   -   vector #1
    IU  -   subvector offset
    V   -   vector #2
    IV  -   subvector offset
*************************************************************************/
void rmatrixrank1(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     /* Real    */ ae_vector* u,
     ae_int_t iu,
     /* Real    */ ae_vector* v,
     ae_int_t iv,
     ae_state *_state)
{
    ae_int_t i;
    double s;


    if( m==0||n==0 )
    {
        return;
    }
    if( rmatrixrank1f(m, n, a, ia, ja, u, iu, v, iv, _state) )
    {
        return;
    }
    for(i=0; i<=m-1; i++)
    {
        s = u->ptr.p_double[iu+i];
        ae_v_addd(&a->ptr.pp_double[ia+i][ja], 1, &v->ptr.p_double[iv], 1, ae_v_len(ja,ja+n-1), s);
    }
}


/*************************************************************************
Matrix-vector product: y := op(A)*x

INPUT PARAMETERS:
    M   -   number of rows of op(A)
            M>=0
    N   -   number of columns of op(A)
            N>=0
    A   -   target matrix
    IA  -   submatrix offset (row index)
    JA  -   submatrix offset (column index)
    OpA -   operation type:
            * OpA=0     =>  op(A) = A
            * OpA=1     =>  op(A) = A^T
            * OpA=2     =>  op(A) = A^H
    X   -   input vector
    IX  -   subvector offset
    IY  -   subvector offset
    Y   -   preallocated matrix, must be large enough to store result

OUTPUT PARAMETERS:
    Y   -   vector which stores result

if M=0, then subroutine does nothing.
if N=0, Y is filled by zeros.


  -- ALGLIB routine --

     28.01.2010
     Bochkanov Sergey
*************************************************************************/
void cmatrixmv(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t opa,
     /* Complex */ ae_vector* x,
     ae_int_t ix,
     /* Complex */ ae_vector* y,
     ae_int_t iy,
     ae_state *_state)
{
    ae_int_t i;
    ae_complex v;


    if( m==0 )
    {
        return;
    }
    if( n==0 )
    {
        for(i=0; i<=m-1; i++)
        {
            y->ptr.p_complex[iy+i] = ae_complex_from_i(0);
        }
        return;
    }
    if( cmatrixmvf(m, n, a, ia, ja, opa, x, ix, y, iy, _state) )
    {
        return;
    }
    if( opa==0 )
    {
        
        /*
         * y = A*x
         */
        for(i=0; i<=m-1; i++)
        {
            v = ae_v_cdotproduct(&a->ptr.pp_complex[ia+i][ja], 1, "N", &x->ptr.p_complex[ix], 1, "N", ae_v_len(ja,ja+n-1));
            y->ptr.p_complex[iy+i] = v;
        }
        return;
    }
    if( opa==1 )
    {
        
        /*
         * y = A^T*x
         */
        for(i=0; i<=m-1; i++)
        {
            y->ptr.p_complex[iy+i] = ae_complex_from_i(0);
        }
        for(i=0; i<=n-1; i++)
        {
            v = x->ptr.p_complex[ix+i];
            ae_v_caddc(&y->ptr.p_complex[iy], 1, &a->ptr.pp_complex[ia+i][ja], 1, "N", ae_v_len(iy,iy+m-1), v);
        }
        return;
    }
    if( opa==2 )
    {
        
        /*
         * y = A^H*x
         */
        for(i=0; i<=m-1; i++)
        {
            y->ptr.p_complex[iy+i] = ae_complex_from_i(0);
        }
        for(i=0; i<=n-1; i++)
        {
            v = x->ptr.p_complex[ix+i];
            ae_v_caddc(&y->ptr.p_complex[iy], 1, &a->ptr.pp_complex[ia+i][ja], 1, "Conj", ae_v_len(iy,iy+m-1), v);
        }
        return;
    }
}


/*************************************************************************
Matrix-vector product: y := op(A)*x

INPUT PARAMETERS:
    M   -   number of rows of op(A)
    N   -   number of columns of op(A)
    A   -   target matrix
    IA  -   submatrix offset (row index)
    JA  -   submatrix offset (column index)
    OpA -   operation type:
            * OpA=0     =>  op(A) = A
            * OpA=1     =>  op(A) = A^T
    X   -   input vector
    IX  -   subvector offset
    IY  -   subvector offset
    Y   -   preallocated matrix, must be large enough to store result

OUTPUT PARAMETERS:
    Y   -   vector which stores result

if M=0, then subroutine does nothing.
if N=0, Y is filled by zeros.


  -- ALGLIB routine --

     28.01.2010
     Bochkanov Sergey
*************************************************************************/
void rmatrixmv(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t opa,
     /* Real    */ ae_vector* x,
     ae_int_t ix,
     /* Real    */ ae_vector* y,
     ae_int_t iy,
     ae_state *_state)
{
    ae_int_t i;
    double v;


    if( m==0 )
    {
        return;
    }
    if( n==0 )
    {
        for(i=0; i<=m-1; i++)
        {
            y->ptr.p_double[iy+i] = (double)(0);
        }
        return;
    }
    if( rmatrixmvf(m, n, a, ia, ja, opa, x, ix, y, iy, _state) )
    {
        return;
    }
    if( opa==0 )
    {
        
        /*
         * y = A*x
         */
        for(i=0; i<=m-1; i++)
        {
            v = ae_v_dotproduct(&a->ptr.pp_double[ia+i][ja], 1, &x->ptr.p_double[ix], 1, ae_v_len(ja,ja+n-1));
            y->ptr.p_double[iy+i] = v;
        }
        return;
    }
    if( opa==1 )
    {
        
        /*
         * y = A^T*x
         */
        for(i=0; i<=m-1; i++)
        {
            y->ptr.p_double[iy+i] = (double)(0);
        }
        for(i=0; i<=n-1; i++)
        {
            v = x->ptr.p_double[ix+i];
            ae_v_addd(&y->ptr.p_double[iy], 1, &a->ptr.pp_double[ia+i][ja], 1, ae_v_len(iy,iy+m-1), v);
        }
        return;
    }
}


/*************************************************************************
This subroutine calculates X*op(A^-1) where:
* X is MxN general matrix
* A is NxN upper/lower triangular/unitriangular matrix
* "op" may be identity transformation, transposition, conjugate transposition

Multiplication result replaces X.
Cache-oblivious algorithm is used.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of  this  function.  Because  starting/stopping  worker  thread always
  ! involves some overhead, parallelism starts to be  profitable  for  N's
  ! larger than 128.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    N   -   matrix size, N>=0
    M   -   matrix size, N>=0
    A       -   matrix, actial matrix is stored in A[I1:I1+N-1,J1:J1+N-1]
    I1      -   submatrix offset
    J1      -   submatrix offset
    IsUpper -   whether matrix is upper triangular
    IsUnit  -   whether matrix is unitriangular
    OpType  -   transformation type:
                * 0 - no transformation
                * 1 - transposition
                * 2 - conjugate transposition
    X   -   matrix, actial matrix is stored in X[I2:I2+M-1,J2:J2+N-1]
    I2  -   submatrix offset
    J2  -   submatrix offset

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
void cmatrixrighttrsm(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Complex */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state)
{
    ae_int_t s1;
    ae_int_t s2;
    ae_int_t bs;


    bs = ablascomplexblocksize(a, _state);
    
    /*
     * Basecase: either MKL-supported code or ALGLIB basecase code
     */
    if( cmatrixrighttrsmmkl(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state) )
    {
        return;
    }
    if( m<=bs&&n<=bs )
    {
        ablas_cmatrixrighttrsm2(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
        return;
    }
    
    /*
     * Recursive subdivision
     */
    if( m>=n )
    {
        
        /*
         * Split X: X*A = (X1 X2)^T*A
         */
        ablascomplexsplitlength(a, m, &s1, &s2, _state);
        cmatrixrighttrsm(s1, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
        cmatrixrighttrsm(s2, n, a, i1, j1, isupper, isunit, optype, x, i2+s1, j2, _state);
        return;
    }
    else
    {
        
        /*
         * Split A:
         *               (A1  A12)
         * X*op(A) = X*op(       )
         *               (     A2)
         *
         * Different variants depending on
         * IsUpper/OpType combinations
         */
        ablascomplexsplitlength(a, n, &s1, &s2, _state);
        if( isupper&&optype==0 )
        {
            
            /*
             *                  (A1  A12)-1
             * X*A^-1 = (X1 X2)*(       )
             *                  (     A2)
             */
            cmatrixrighttrsm(m, s1, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
            cmatrixgemm(m, s2, s1, ae_complex_from_d(-1.0), x, i2, j2, 0, a, i1, j1+s1, 0, ae_complex_from_d(1.0), x, i2, j2+s1, _state);
            cmatrixrighttrsm(m, s2, a, i1+s1, j1+s1, isupper, isunit, optype, x, i2, j2+s1, _state);
            return;
        }
        if( isupper&&optype!=0 )
        {
            
            /*
             *                  (A1'     )-1
             * X*A^-1 = (X1 X2)*(        )
             *                  (A12' A2')
             */
            cmatrixrighttrsm(m, s2, a, i1+s1, j1+s1, isupper, isunit, optype, x, i2, j2+s1, _state);
            cmatrixgemm(m, s1, s2, ae_complex_from_d(-1.0), x, i2, j2+s1, 0, a, i1, j1+s1, optype, ae_complex_from_d(1.0), x, i2, j2, _state);
            cmatrixrighttrsm(m, s1, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
            return;
        }
        if( !isupper&&optype==0 )
        {
            
            /*
             *                  (A1     )-1
             * X*A^-1 = (X1 X2)*(       )
             *                  (A21  A2)
             */
            cmatrixrighttrsm(m, s2, a, i1+s1, j1+s1, isupper, isunit, optype, x, i2, j2+s1, _state);
            cmatrixgemm(m, s1, s2, ae_complex_from_d(-1.0), x, i2, j2+s1, 0, a, i1+s1, j1, 0, ae_complex_from_d(1.0), x, i2, j2, _state);
            cmatrixrighttrsm(m, s1, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
            return;
        }
        if( !isupper&&optype!=0 )
        {
            
            /*
             *                  (A1' A21')-1
             * X*A^-1 = (X1 X2)*(        )
             *                  (     A2')
             */
            cmatrixrighttrsm(m, s1, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
            cmatrixgemm(m, s2, s1, ae_complex_from_d(-1.0), x, i2, j2, 0, a, i1+s1, j1, optype, ae_complex_from_d(1.0), x, i2, j2+s1, _state);
            cmatrixrighttrsm(m, s2, a, i1+s1, j1+s1, isupper, isunit, optype, x, i2, j2+s1, _state);
            return;
        }
    }
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixrighttrsm(ae_int_t m,
    ae_int_t n,
    /* Complex */ ae_matrix* a,
    ae_int_t i1,
    ae_int_t j1,
    ae_bool isupper,
    ae_bool isunit,
    ae_int_t optype,
    /* Complex */ ae_matrix* x,
    ae_int_t i2,
    ae_int_t j2, ae_state *_state)
{
    cmatrixrighttrsm(m,n,a,i1,j1,isupper,isunit,optype,x,i2,j2, _state);
}


/*************************************************************************
This subroutine calculates op(A^-1)*X where:
* X is MxN general matrix
* A is MxM upper/lower triangular/unitriangular matrix
* "op" may be identity transformation, transposition, conjugate transposition

Multiplication result replaces X.
Cache-oblivious algorithm is used.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of  this  function.  Because  starting/stopping  worker  thread always
  ! involves some overhead, parallelism starts to be  profitable  for  N's
  ! larger than 128.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    N   -   matrix size, N>=0
    M   -   matrix size, N>=0
    A       -   matrix, actial matrix is stored in A[I1:I1+M-1,J1:J1+M-1]
    I1      -   submatrix offset
    J1      -   submatrix offset
    IsUpper -   whether matrix is upper triangular
    IsUnit  -   whether matrix is unitriangular
    OpType  -   transformation type:
                * 0 - no transformation
                * 1 - transposition
                * 2 - conjugate transposition
    X   -   matrix, actial matrix is stored in X[I2:I2+M-1,J2:J2+N-1]
    I2  -   submatrix offset
    J2  -   submatrix offset

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
void cmatrixlefttrsm(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Complex */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state)
{
    ae_int_t s1;
    ae_int_t s2;
    ae_int_t bs;


    bs = ablascomplexblocksize(a, _state);
    
    /*
     * Basecase: either MKL-supported code or ALGLIB basecase code
     */
    if( cmatrixlefttrsmmkl(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state) )
    {
        return;
    }
    if( m<=bs&&n<=bs )
    {
        ablas_cmatrixlefttrsm2(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
        return;
    }
    
    /*
     * Recursive subdivision
     */
    if( n>=m )
    {
        
        /*
         * Split X: op(A)^-1*X = op(A)^-1*(X1 X2)
         */
        ablascomplexsplitlength(x, n, &s1, &s2, _state);
        cmatrixlefttrsm(m, s1, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
        cmatrixlefttrsm(m, s2, a, i1, j1, isupper, isunit, optype, x, i2, j2+s1, _state);
        return;
    }
    else
    {
        
        /*
         * Split A
         */
        ablascomplexsplitlength(a, m, &s1, &s2, _state);
        if( isupper&&optype==0 )
        {
            
            /*
             *           (A1  A12)-1  ( X1 )
             * A^-1*X* = (       )   *(    )
             *           (     A2)    ( X2 )
             */
            cmatrixlefttrsm(s2, n, a, i1+s1, j1+s1, isupper, isunit, optype, x, i2+s1, j2, _state);
            cmatrixgemm(s1, n, s2, ae_complex_from_d(-1.0), a, i1, j1+s1, 0, x, i2+s1, j2, 0, ae_complex_from_d(1.0), x, i2, j2, _state);
            cmatrixlefttrsm(s1, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
            return;
        }
        if( isupper&&optype!=0 )
        {
            
            /*
             *          (A1'     )-1 ( X1 )
             * A^-1*X = (        )  *(    )
             *          (A12' A2')   ( X2 )
             */
            cmatrixlefttrsm(s1, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
            cmatrixgemm(s2, n, s1, ae_complex_from_d(-1.0), a, i1, j1+s1, optype, x, i2, j2, 0, ae_complex_from_d(1.0), x, i2+s1, j2, _state);
            cmatrixlefttrsm(s2, n, a, i1+s1, j1+s1, isupper, isunit, optype, x, i2+s1, j2, _state);
            return;
        }
        if( !isupper&&optype==0 )
        {
            
            /*
             *          (A1     )-1 ( X1 )
             * A^-1*X = (       )  *(    )
             *          (A21  A2)   ( X2 )
             */
            cmatrixlefttrsm(s1, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
            cmatrixgemm(s2, n, s1, ae_complex_from_d(-1.0), a, i1+s1, j1, 0, x, i2, j2, 0, ae_complex_from_d(1.0), x, i2+s1, j2, _state);
            cmatrixlefttrsm(s2, n, a, i1+s1, j1+s1, isupper, isunit, optype, x, i2+s1, j2, _state);
            return;
        }
        if( !isupper&&optype!=0 )
        {
            
            /*
             *          (A1' A21')-1 ( X1 )
             * A^-1*X = (        )  *(    )
             *          (     A2')   ( X2 )
             */
            cmatrixlefttrsm(s2, n, a, i1+s1, j1+s1, isupper, isunit, optype, x, i2+s1, j2, _state);
            cmatrixgemm(s1, n, s2, ae_complex_from_d(-1.0), a, i1+s1, j1, optype, x, i2+s1, j2, 0, ae_complex_from_d(1.0), x, i2, j2, _state);
            cmatrixlefttrsm(s1, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
            return;
        }
    }
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixlefttrsm(ae_int_t m,
    ae_int_t n,
    /* Complex */ ae_matrix* a,
    ae_int_t i1,
    ae_int_t j1,
    ae_bool isupper,
    ae_bool isunit,
    ae_int_t optype,
    /* Complex */ ae_matrix* x,
    ae_int_t i2,
    ae_int_t j2, ae_state *_state)
{
    cmatrixlefttrsm(m,n,a,i1,j1,isupper,isunit,optype,x,i2,j2, _state);
}


/*************************************************************************
This subroutine calculates X*op(A^-1) where:
* X is MxN general matrix
* A is NxN upper/lower triangular/unitriangular matrix
* "op" may be identity transformation, transposition

Multiplication result replaces X.
Cache-oblivious algorithm is used.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of  this  function.  Because  starting/stopping  worker  thread always
  ! involves some overhead, parallelism starts to be  profitable  for  N's
  ! larger than 128.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    N   -   matrix size, N>=0
    M   -   matrix size, N>=0
    A       -   matrix, actial matrix is stored in A[I1:I1+N-1,J1:J1+N-1]
    I1      -   submatrix offset
    J1      -   submatrix offset
    IsUpper -   whether matrix is upper triangular
    IsUnit  -   whether matrix is unitriangular
    OpType  -   transformation type:
                * 0 - no transformation
                * 1 - transposition
    X   -   matrix, actial matrix is stored in X[I2:I2+M-1,J2:J2+N-1]
    I2  -   submatrix offset
    J2  -   submatrix offset

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
void rmatrixrighttrsm(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Real    */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state)
{
    ae_int_t s1;
    ae_int_t s2;
    ae_int_t bs;


    bs = ablasblocksize(a, _state);
    
    /*
     * Basecase: MKL or ALGLIB code
     */
    if( rmatrixrighttrsmmkl(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state) )
    {
        return;
    }
    if( m<=bs&&n<=bs )
    {
        ablas_rmatrixrighttrsm2(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
        return;
    }
    
    /*
     * Recursive subdivision
     */
    if( m>=n )
    {
        
        /*
         * Split X: X*A = (X1 X2)^T*A
         */
        ablassplitlength(a, m, &s1, &s2, _state);
        rmatrixrighttrsm(s1, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
        rmatrixrighttrsm(s2, n, a, i1, j1, isupper, isunit, optype, x, i2+s1, j2, _state);
        return;
    }
    else
    {
        
        /*
         * Split A:
         *               (A1  A12)
         * X*op(A) = X*op(       )
         *               (     A2)
         *
         * Different variants depending on
         * IsUpper/OpType combinations
         */
        ablassplitlength(a, n, &s1, &s2, _state);
        if( isupper&&optype==0 )
        {
            
            /*
             *                  (A1  A12)-1
             * X*A^-1 = (X1 X2)*(       )
             *                  (     A2)
             */
            rmatrixrighttrsm(m, s1, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
            rmatrixgemm(m, s2, s1, -1.0, x, i2, j2, 0, a, i1, j1+s1, 0, 1.0, x, i2, j2+s1, _state);
            rmatrixrighttrsm(m, s2, a, i1+s1, j1+s1, isupper, isunit, optype, x, i2, j2+s1, _state);
            return;
        }
        if( isupper&&optype!=0 )
        {
            
            /*
             *                  (A1'     )-1
             * X*A^-1 = (X1 X2)*(        )
             *                  (A12' A2')
             */
            rmatrixrighttrsm(m, s2, a, i1+s1, j1+s1, isupper, isunit, optype, x, i2, j2+s1, _state);
            rmatrixgemm(m, s1, s2, -1.0, x, i2, j2+s1, 0, a, i1, j1+s1, optype, 1.0, x, i2, j2, _state);
            rmatrixrighttrsm(m, s1, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
            return;
        }
        if( !isupper&&optype==0 )
        {
            
            /*
             *                  (A1     )-1
             * X*A^-1 = (X1 X2)*(       )
             *                  (A21  A2)
             */
            rmatrixrighttrsm(m, s2, a, i1+s1, j1+s1, isupper, isunit, optype, x, i2, j2+s1, _state);
            rmatrixgemm(m, s1, s2, -1.0, x, i2, j2+s1, 0, a, i1+s1, j1, 0, 1.0, x, i2, j2, _state);
            rmatrixrighttrsm(m, s1, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
            return;
        }
        if( !isupper&&optype!=0 )
        {
            
            /*
             *                  (A1' A21')-1
             * X*A^-1 = (X1 X2)*(        )
             *                  (     A2')
             */
            rmatrixrighttrsm(m, s1, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
            rmatrixgemm(m, s2, s1, -1.0, x, i2, j2, 0, a, i1+s1, j1, optype, 1.0, x, i2, j2+s1, _state);
            rmatrixrighttrsm(m, s2, a, i1+s1, j1+s1, isupper, isunit, optype, x, i2, j2+s1, _state);
            return;
        }
    }
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixrighttrsm(ae_int_t m,
    ae_int_t n,
    /* Real    */ ae_matrix* a,
    ae_int_t i1,
    ae_int_t j1,
    ae_bool isupper,
    ae_bool isunit,
    ae_int_t optype,
    /* Real    */ ae_matrix* x,
    ae_int_t i2,
    ae_int_t j2, ae_state *_state)
{
    rmatrixrighttrsm(m,n,a,i1,j1,isupper,isunit,optype,x,i2,j2, _state);
}


/*************************************************************************
This subroutine calculates op(A^-1)*X where:
* X is MxN general matrix
* A is MxM upper/lower triangular/unitriangular matrix
* "op" may be identity transformation, transposition

Multiplication result replaces X.
Cache-oblivious algorithm is used.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of  this  function.  Because  starting/stopping  worker  thread always
  ! involves some overhead, parallelism starts to be  profitable  for  N's
  ! larger than 128.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    N   -   matrix size, N>=0
    M   -   matrix size, N>=0
    A       -   matrix, actial matrix is stored in A[I1:I1+M-1,J1:J1+M-1]
    I1      -   submatrix offset
    J1      -   submatrix offset
    IsUpper -   whether matrix is upper triangular
    IsUnit  -   whether matrix is unitriangular
    OpType  -   transformation type:
                * 0 - no transformation
                * 1 - transposition
    X   -   matrix, actial matrix is stored in X[I2:I2+M-1,J2:J2+N-1]
    I2  -   submatrix offset
    J2  -   submatrix offset

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
void rmatrixlefttrsm(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Real    */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state)
{
    ae_int_t s1;
    ae_int_t s2;
    ae_int_t bs;


    bs = ablasblocksize(a, _state);
    
    /*
     * Basecase: MKL or ALGLIB code
     */
    if( rmatrixlefttrsmmkl(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state) )
    {
        return;
    }
    if( m<=bs&&n<=bs )
    {
        ablas_rmatrixlefttrsm2(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
        return;
    }
    
    /*
     * Recursive subdivision
     */
    if( n>=m )
    {
        
        /*
         * Split X: op(A)^-1*X = op(A)^-1*(X1 X2)
         */
        ablassplitlength(x, n, &s1, &s2, _state);
        rmatrixlefttrsm(m, s1, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
        rmatrixlefttrsm(m, s2, a, i1, j1, isupper, isunit, optype, x, i2, j2+s1, _state);
    }
    else
    {
        
        /*
         * Split A
         */
        ablassplitlength(a, m, &s1, &s2, _state);
        if( isupper&&optype==0 )
        {
            
            /*
             *           (A1  A12)-1  ( X1 )
             * A^-1*X* = (       )   *(    )
             *           (     A2)    ( X2 )
             */
            rmatrixlefttrsm(s2, n, a, i1+s1, j1+s1, isupper, isunit, optype, x, i2+s1, j2, _state);
            rmatrixgemm(s1, n, s2, -1.0, a, i1, j1+s1, 0, x, i2+s1, j2, 0, 1.0, x, i2, j2, _state);
            rmatrixlefttrsm(s1, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
            return;
        }
        if( isupper&&optype!=0 )
        {
            
            /*
             *          (A1'     )-1 ( X1 )
             * A^-1*X = (        )  *(    )
             *          (A12' A2')   ( X2 )
             */
            rmatrixlefttrsm(s1, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
            rmatrixgemm(s2, n, s1, -1.0, a, i1, j1+s1, optype, x, i2, j2, 0, 1.0, x, i2+s1, j2, _state);
            rmatrixlefttrsm(s2, n, a, i1+s1, j1+s1, isupper, isunit, optype, x, i2+s1, j2, _state);
            return;
        }
        if( !isupper&&optype==0 )
        {
            
            /*
             *          (A1     )-1 ( X1 )
             * A^-1*X = (       )  *(    )
             *          (A21  A2)   ( X2 )
             */
            rmatrixlefttrsm(s1, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
            rmatrixgemm(s2, n, s1, -1.0, a, i1+s1, j1, 0, x, i2, j2, 0, 1.0, x, i2+s1, j2, _state);
            rmatrixlefttrsm(s2, n, a, i1+s1, j1+s1, isupper, isunit, optype, x, i2+s1, j2, _state);
            return;
        }
        if( !isupper&&optype!=0 )
        {
            
            /*
             *          (A1' A21')-1 ( X1 )
             * A^-1*X = (        )  *(    )
             *          (     A2')   ( X2 )
             */
            rmatrixlefttrsm(s2, n, a, i1+s1, j1+s1, isupper, isunit, optype, x, i2+s1, j2, _state);
            rmatrixgemm(s1, n, s2, -1.0, a, i1+s1, j1, optype, x, i2+s1, j2, 0, 1.0, x, i2, j2, _state);
            rmatrixlefttrsm(s1, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state);
            return;
        }
    }
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixlefttrsm(ae_int_t m,
    ae_int_t n,
    /* Real    */ ae_matrix* a,
    ae_int_t i1,
    ae_int_t j1,
    ae_bool isupper,
    ae_bool isunit,
    ae_int_t optype,
    /* Real    */ ae_matrix* x,
    ae_int_t i2,
    ae_int_t j2, ae_state *_state)
{
    rmatrixlefttrsm(m,n,a,i1,j1,isupper,isunit,optype,x,i2,j2, _state);
}


/*************************************************************************
This subroutine calculates  C=alpha*A*A^H+beta*C  or  C=alpha*A^H*A+beta*C
where:
* C is NxN Hermitian matrix given by its upper/lower triangle
* A is NxK matrix when A*A^H is calculated, KxN matrix otherwise

Additional info:
* cache-oblivious algorithm is used.
* multiplication result replaces C. If Beta=0, C elements are not used in
  calculations (not multiplied by zero - just not referenced)
* if Alpha=0, A is not used (not multiplied by zero - just not referenced)
* if both Beta and Alpha are zero, C is filled by zeros.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of  this  function.  Because  starting/stopping  worker  thread always
  ! involves some overhead, parallelism starts to be  profitable  for  N's
  ! larger than 128.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    N       -   matrix size, N>=0
    K       -   matrix size, K>=0
    Alpha   -   coefficient
    A       -   matrix
    IA      -   submatrix offset (row index)
    JA      -   submatrix offset (column index)
    OpTypeA -   multiplication type:
                * 0 - A*A^H is calculated
                * 2 - A^H*A is calculated
    Beta    -   coefficient
    C       -   preallocated input/output matrix
    IC      -   submatrix offset (row index)
    JC      -   submatrix offset (column index)
    IsUpper -   whether upper or lower triangle of C is updated;
                this function updates only one half of C, leaving
                other half unchanged (not referenced at all).

  -- ALGLIB routine --
     16.12.2009
     Bochkanov Sergey
*************************************************************************/
void cmatrixherk(ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     double beta,
     /* Complex */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_bool isupper,
     ae_state *_state)
{
    ae_int_t s1;
    ae_int_t s2;
    ae_int_t bs;


    bs = ablascomplexblocksize(a, _state);
    
    /*
     * Use MKL or ALGLIB basecase code
     */
    if( cmatrixherkmkl(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state) )
    {
        return;
    }
    if( n<=bs&&k<=bs )
    {
        ablas_cmatrixherk2(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state);
        return;
    }
    
    /*
     * Recursive division of the problem
     */
    if( k>=n )
    {
        
        /*
         * Split K
         */
        ablascomplexsplitlength(a, k, &s1, &s2, _state);
        if( optypea==0 )
        {
            cmatrixherk(n, s1, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state);
            cmatrixherk(n, s2, alpha, a, ia, ja+s1, optypea, 1.0, c, ic, jc, isupper, _state);
        }
        else
        {
            cmatrixherk(n, s1, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state);
            cmatrixherk(n, s2, alpha, a, ia+s1, ja, optypea, 1.0, c, ic, jc, isupper, _state);
        }
    }
    else
    {
        
        /*
         * Split N
         */
        ablascomplexsplitlength(a, n, &s1, &s2, _state);
        if( optypea==0&&isupper )
        {
            cmatrixherk(s1, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state);
            cmatrixgemm(s1, s2, k, ae_complex_from_d(alpha), a, ia, ja, 0, a, ia+s1, ja, 2, ae_complex_from_d(beta), c, ic, jc+s1, _state);
            cmatrixherk(s2, k, alpha, a, ia+s1, ja, optypea, beta, c, ic+s1, jc+s1, isupper, _state);
            return;
        }
        if( optypea==0&&!isupper )
        {
            cmatrixherk(s1, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state);
            cmatrixgemm(s2, s1, k, ae_complex_from_d(alpha), a, ia+s1, ja, 0, a, ia, ja, 2, ae_complex_from_d(beta), c, ic+s1, jc, _state);
            cmatrixherk(s2, k, alpha, a, ia+s1, ja, optypea, beta, c, ic+s1, jc+s1, isupper, _state);
            return;
        }
        if( optypea!=0&&isupper )
        {
            cmatrixherk(s1, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state);
            cmatrixgemm(s1, s2, k, ae_complex_from_d(alpha), a, ia, ja, 2, a, ia, ja+s1, 0, ae_complex_from_d(beta), c, ic, jc+s1, _state);
            cmatrixherk(s2, k, alpha, a, ia, ja+s1, optypea, beta, c, ic+s1, jc+s1, isupper, _state);
            return;
        }
        if( optypea!=0&&!isupper )
        {
            cmatrixherk(s1, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state);
            cmatrixgemm(s2, s1, k, ae_complex_from_d(alpha), a, ia, ja+s1, 2, a, ia, ja, 0, ae_complex_from_d(beta), c, ic+s1, jc, _state);
            cmatrixherk(s2, k, alpha, a, ia, ja+s1, optypea, beta, c, ic+s1, jc+s1, isupper, _state);
            return;
        }
    }
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixherk(ae_int_t n,
    ae_int_t k,
    double alpha,
    /* Complex */ ae_matrix* a,
    ae_int_t ia,
    ae_int_t ja,
    ae_int_t optypea,
    double beta,
    /* Complex */ ae_matrix* c,
    ae_int_t ic,
    ae_int_t jc,
    ae_bool isupper, ae_state *_state)
{
    cmatrixherk(n,k,alpha,a,ia,ja,optypea,beta,c,ic,jc,isupper, _state);
}


/*************************************************************************
This subroutine calculates  C=alpha*A*A^T+beta*C  or  C=alpha*A^T*A+beta*C
where:
* C is NxN symmetric matrix given by its upper/lower triangle
* A is NxK matrix when A*A^T is calculated, KxN matrix otherwise

Additional info:
* cache-oblivious algorithm is used.
* multiplication result replaces C. If Beta=0, C elements are not used in
  calculations (not multiplied by zero - just not referenced)
* if Alpha=0, A is not used (not multiplied by zero - just not referenced)
* if both Beta and Alpha are zero, C is filled by zeros.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of  this  function.  Because  starting/stopping  worker  thread always
  ! involves some overhead, parallelism starts to be  profitable  for  N's
  ! larger than 128.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS
    N       -   matrix size, N>=0
    K       -   matrix size, K>=0
    Alpha   -   coefficient
    A       -   matrix
    IA      -   submatrix offset (row index)
    JA      -   submatrix offset (column index)
    OpTypeA -   multiplication type:
                * 0 - A*A^T is calculated
                * 2 - A^T*A is calculated
    Beta    -   coefficient
    C       -   preallocated input/output matrix
    IC      -   submatrix offset (row index)
    JC      -   submatrix offset (column index)
    IsUpper -   whether C is upper triangular or lower triangular

  -- ALGLIB routine --
     16.12.2009
     Bochkanov Sergey
*************************************************************************/
void rmatrixsyrk(ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     double beta,
     /* Real    */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_bool isupper,
     ae_state *_state)
{
    ae_int_t s1;
    ae_int_t s2;
    ae_int_t bs;


    bs = ablasblocksize(a, _state);
    
    /*
     * Use MKL or generic basecase code
     */
    if( rmatrixsyrkmkl(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state) )
    {
        return;
    }
    if( n<=bs&&k<=bs )
    {
        ablas_rmatrixsyrk2(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state);
        return;
    }
    
    /*
     * Recursive subdivision of the problem
     */
    if( k>=n )
    {
        
        /*
         * Split K
         */
        ablassplitlength(a, k, &s1, &s2, _state);
        if( optypea==0 )
        {
            rmatrixsyrk(n, s1, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state);
            rmatrixsyrk(n, s2, alpha, a, ia, ja+s1, optypea, 1.0, c, ic, jc, isupper, _state);
        }
        else
        {
            rmatrixsyrk(n, s1, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state);
            rmatrixsyrk(n, s2, alpha, a, ia+s1, ja, optypea, 1.0, c, ic, jc, isupper, _state);
        }
    }
    else
    {
        
        /*
         * Split N
         */
        ablassplitlength(a, n, &s1, &s2, _state);
        if( optypea==0&&isupper )
        {
            rmatrixsyrk(s1, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state);
            rmatrixgemm(s1, s2, k, alpha, a, ia, ja, 0, a, ia+s1, ja, 1, beta, c, ic, jc+s1, _state);
            rmatrixsyrk(s2, k, alpha, a, ia+s1, ja, optypea, beta, c, ic+s1, jc+s1, isupper, _state);
            return;
        }
        if( optypea==0&&!isupper )
        {
            rmatrixsyrk(s1, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state);
            rmatrixgemm(s2, s1, k, alpha, a, ia+s1, ja, 0, a, ia, ja, 1, beta, c, ic+s1, jc, _state);
            rmatrixsyrk(s2, k, alpha, a, ia+s1, ja, optypea, beta, c, ic+s1, jc+s1, isupper, _state);
            return;
        }
        if( optypea!=0&&isupper )
        {
            rmatrixsyrk(s1, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state);
            rmatrixgemm(s1, s2, k, alpha, a, ia, ja, 1, a, ia, ja+s1, 0, beta, c, ic, jc+s1, _state);
            rmatrixsyrk(s2, k, alpha, a, ia, ja+s1, optypea, beta, c, ic+s1, jc+s1, isupper, _state);
            return;
        }
        if( optypea!=0&&!isupper )
        {
            rmatrixsyrk(s1, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state);
            rmatrixgemm(s2, s1, k, alpha, a, ia, ja+s1, 1, a, ia, ja, 0, beta, c, ic+s1, jc, _state);
            rmatrixsyrk(s2, k, alpha, a, ia, ja+s1, optypea, beta, c, ic+s1, jc+s1, isupper, _state);
            return;
        }
    }
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixsyrk(ae_int_t n,
    ae_int_t k,
    double alpha,
    /* Real    */ ae_matrix* a,
    ae_int_t ia,
    ae_int_t ja,
    ae_int_t optypea,
    double beta,
    /* Real    */ ae_matrix* c,
    ae_int_t ic,
    ae_int_t jc,
    ae_bool isupper, ae_state *_state)
{
    rmatrixsyrk(n,k,alpha,a,ia,ja,optypea,beta,c,ic,jc,isupper, _state);
}


/*************************************************************************
This subroutine calculates C = alpha*op1(A)*op2(B) +beta*C where:
* C is MxN general matrix
* op1(A) is MxK matrix
* op2(B) is KxN matrix
* "op" may be identity transformation, transposition, conjugate transposition

Additional info:
* cache-oblivious algorithm is used.
* multiplication result replaces C. If Beta=0, C elements are not used in
  calculations (not multiplied by zero - just not referenced)
* if Alpha=0, A is not used (not multiplied by zero - just not referenced)
* if both Beta and Alpha are zero, C is filled by zeros.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of  this  function.  Because  starting/stopping  worker  thread always
  ! involves some overhead, parallelism starts to be  profitable  for  N's
  ! larger than 128.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

IMPORTANT:

This function does NOT preallocate output matrix C, it MUST be preallocated
by caller prior to calling this function. In case C does not have  enough
space to store result, exception will be generated.

INPUT PARAMETERS
    M       -   matrix size, M>0
    N       -   matrix size, N>0
    K       -   matrix size, K>0
    Alpha   -   coefficient
    A       -   matrix
    IA      -   submatrix offset
    JA      -   submatrix offset
    OpTypeA -   transformation type:
                * 0 - no transformation
                * 1 - transposition
                * 2 - conjugate transposition
    B       -   matrix
    IB      -   submatrix offset
    JB      -   submatrix offset
    OpTypeB -   transformation type:
                * 0 - no transformation
                * 1 - transposition
                * 2 - conjugate transposition
    Beta    -   coefficient
    C       -   matrix (PREALLOCATED, large enough to store result)
    IC      -   submatrix offset
    JC      -   submatrix offset

  -- ALGLIB routine --
     16.12.2009
     Bochkanov Sergey
*************************************************************************/
void cmatrixgemm(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     ae_complex alpha,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     /* Complex */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     ae_int_t optypeb,
     ae_complex beta,
     /* Complex */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_state *_state)
{
    ae_int_t s1;
    ae_int_t s2;
    ae_int_t bs;


    bs = ablascomplexblocksize(a, _state);
    
    /*
     * Use MKL or ALGLIB basecase code
     */
    if( cmatrixgemmmkl(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state) )
    {
        return;
    }
    if( (m<=bs&&n<=bs)&&k<=bs )
    {
        cmatrixgemmk(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
        return;
    }
    
    /*
     * SMP support is turned on when M or N are larger than some boundary value.
     * Magnitude of K is not taken into account because splitting on K does not
     * allow us to spawn child tasks.
     */
    
    /*
     * Recursive algorithm: parallel splitting on M/N
     */
    if( m>=n&&m>=k )
    {
        
        /*
         * A*B = (A1 A2)^T*B
         */
        ablascomplexsplitlength(a, m, &s1, &s2, _state);
        cmatrixgemm(s1, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
        if( optypea==0 )
        {
            cmatrixgemm(s2, n, k, alpha, a, ia+s1, ja, optypea, b, ib, jb, optypeb, beta, c, ic+s1, jc, _state);
        }
        else
        {
            cmatrixgemm(s2, n, k, alpha, a, ia, ja+s1, optypea, b, ib, jb, optypeb, beta, c, ic+s1, jc, _state);
        }
        return;
    }
    if( n>=m&&n>=k )
    {
        
        /*
         * A*B = A*(B1 B2)
         */
        ablascomplexsplitlength(a, n, &s1, &s2, _state);
        if( optypeb==0 )
        {
            cmatrixgemm(m, s1, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
            cmatrixgemm(m, s2, k, alpha, a, ia, ja, optypea, b, ib, jb+s1, optypeb, beta, c, ic, jc+s1, _state);
        }
        else
        {
            cmatrixgemm(m, s1, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
            cmatrixgemm(m, s2, k, alpha, a, ia, ja, optypea, b, ib+s1, jb, optypeb, beta, c, ic, jc+s1, _state);
        }
        return;
    }
    
    /*
     * Recursive algorithm: serial splitting on K
     */
    
    /*
     * A*B = (A1 A2)*(B1 B2)^T
     */
    ablascomplexsplitlength(a, k, &s1, &s2, _state);
    if( optypea==0&&optypeb==0 )
    {
        cmatrixgemm(m, n, s1, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
        cmatrixgemm(m, n, s2, alpha, a, ia, ja+s1, optypea, b, ib+s1, jb, optypeb, ae_complex_from_d(1.0), c, ic, jc, _state);
    }
    if( optypea==0&&optypeb!=0 )
    {
        cmatrixgemm(m, n, s1, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
        cmatrixgemm(m, n, s2, alpha, a, ia, ja+s1, optypea, b, ib, jb+s1, optypeb, ae_complex_from_d(1.0), c, ic, jc, _state);
    }
    if( optypea!=0&&optypeb==0 )
    {
        cmatrixgemm(m, n, s1, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
        cmatrixgemm(m, n, s2, alpha, a, ia+s1, ja, optypea, b, ib+s1, jb, optypeb, ae_complex_from_d(1.0), c, ic, jc, _state);
    }
    if( optypea!=0&&optypeb!=0 )
    {
        cmatrixgemm(m, n, s1, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
        cmatrixgemm(m, n, s2, alpha, a, ia+s1, ja, optypea, b, ib, jb+s1, optypeb, ae_complex_from_d(1.0), c, ic, jc, _state);
    }
    return;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixgemm(ae_int_t m,
    ae_int_t n,
    ae_int_t k,
    ae_complex alpha,
    /* Complex */ ae_matrix* a,
    ae_int_t ia,
    ae_int_t ja,
    ae_int_t optypea,
    /* Complex */ ae_matrix* b,
    ae_int_t ib,
    ae_int_t jb,
    ae_int_t optypeb,
    ae_complex beta,
    /* Complex */ ae_matrix* c,
    ae_int_t ic,
    ae_int_t jc, ae_state *_state)
{
    cmatrixgemm(m,n,k,alpha,a,ia,ja,optypea,b,ib,jb,optypeb,beta,c,ic,jc, _state);
}


/*************************************************************************
This subroutine calculates C = alpha*op1(A)*op2(B) +beta*C where:
* C is MxN general matrix
* op1(A) is MxK matrix
* op2(B) is KxN matrix
* "op" may be identity transformation, transposition

Additional info:
* cache-oblivious algorithm is used.
* multiplication result replaces C. If Beta=0, C elements are not used in
  calculations (not multiplied by zero - just not referenced)
* if Alpha=0, A is not used (not multiplied by zero - just not referenced)
* if both Beta and Alpha are zero, C is filled by zeros.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Say, on SSE2-capable CPU with N=1024, HPC ALGLIB will be:
  ! * about 2-3x faster than ALGLIB for C++ without MKL
  ! * about 7-10x faster than "pure C#" edition of ALGLIB
  ! Difference in performance will be more striking  on  newer  CPU's with
  ! support for newer SIMD instructions.
  !
  ! Commercial edition of ALGLIB also supports multithreaded  acceleration
  ! of  this  function.  Because  starting/stopping  worker  thread always
  ! involves some overhead, parallelism starts to be  profitable  for  N's
  ! larger than 128.
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

IMPORTANT:

This function does NOT preallocate output matrix C, it MUST be preallocated
by caller prior to calling this function. In case C does not have  enough
space to store result, exception will be generated.

INPUT PARAMETERS
    M       -   matrix size, M>0
    N       -   matrix size, N>0
    K       -   matrix size, K>0
    Alpha   -   coefficient
    A       -   matrix
    IA      -   submatrix offset
    JA      -   submatrix offset
    OpTypeA -   transformation type:
                * 0 - no transformation
                * 1 - transposition
    B       -   matrix
    IB      -   submatrix offset
    JB      -   submatrix offset
    OpTypeB -   transformation type:
                * 0 - no transformation
                * 1 - transposition
    Beta    -   coefficient
    C       -   PREALLOCATED output matrix, large enough to store result
    IC      -   submatrix offset
    JC      -   submatrix offset

  -- ALGLIB routine --
     2009-2013
     Bochkanov Sergey
*************************************************************************/
void rmatrixgemm(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     /* Real    */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     ae_int_t optypeb,
     double beta,
     /* Real    */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_state *_state)
{
    ae_int_t s1;
    ae_int_t s2;
    ae_int_t bs;


    bs = ablasblocksize(a, _state);
    
    /*
     * Check input sizes for correctness
     */
    ae_assert(optypea==0||optypea==1, "RMatrixGEMM: incorrect OpTypeA (must be 0 or 1)", _state);
    ae_assert(optypeb==0||optypeb==1, "RMatrixGEMM: incorrect OpTypeB (must be 0 or 1)", _state);
    ae_assert(ic+m<=c->rows, "RMatrixGEMM: incorect size of output matrix C", _state);
    ae_assert(jc+n<=c->cols, "RMatrixGEMM: incorect size of output matrix C", _state);
    
    /*
     * Use MKL or ALGLIB basecase code
     */
    if( rmatrixgemmmkl(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state) )
    {
        return;
    }
    if( (m<=bs&&n<=bs)&&k<=bs )
    {
        rmatrixgemmk(m, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
        return;
    }
    
    /*
     * SMP support is turned on when M or N are larger than some boundary value.
     * Magnitude of K is not taken into account because splitting on K does not
     * allow us to spawn child tasks.
     */
    
    /*
     * Recursive algorithm: split on M or N
     */
    if( m>=n&&m>=k )
    {
        
        /*
         * A*B = (A1 A2)^T*B
         */
        ablassplitlength(a, m, &s1, &s2, _state);
        if( optypea==0 )
        {
            rmatrixgemm(s1, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
            rmatrixgemm(s2, n, k, alpha, a, ia+s1, ja, optypea, b, ib, jb, optypeb, beta, c, ic+s1, jc, _state);
        }
        else
        {
            rmatrixgemm(s1, n, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
            rmatrixgemm(s2, n, k, alpha, a, ia, ja+s1, optypea, b, ib, jb, optypeb, beta, c, ic+s1, jc, _state);
        }
        return;
    }
    if( n>=m&&n>=k )
    {
        
        /*
         * A*B = A*(B1 B2)
         */
        ablassplitlength(a, n, &s1, &s2, _state);
        if( optypeb==0 )
        {
            rmatrixgemm(m, s1, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
            rmatrixgemm(m, s2, k, alpha, a, ia, ja, optypea, b, ib, jb+s1, optypeb, beta, c, ic, jc+s1, _state);
        }
        else
        {
            rmatrixgemm(m, s1, k, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
            rmatrixgemm(m, s2, k, alpha, a, ia, ja, optypea, b, ib+s1, jb, optypeb, beta, c, ic, jc+s1, _state);
        }
        return;
    }
    
    /*
     * Recursive algorithm: split on K
     */
    
    /*
     * A*B = (A1 A2)*(B1 B2)^T
     */
    ablassplitlength(a, k, &s1, &s2, _state);
    if( optypea==0&&optypeb==0 )
    {
        rmatrixgemm(m, n, s1, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
        rmatrixgemm(m, n, s2, alpha, a, ia, ja+s1, optypea, b, ib+s1, jb, optypeb, 1.0, c, ic, jc, _state);
    }
    if( optypea==0&&optypeb!=0 )
    {
        rmatrixgemm(m, n, s1, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
        rmatrixgemm(m, n, s2, alpha, a, ia, ja+s1, optypea, b, ib, jb+s1, optypeb, 1.0, c, ic, jc, _state);
    }
    if( optypea!=0&&optypeb==0 )
    {
        rmatrixgemm(m, n, s1, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
        rmatrixgemm(m, n, s2, alpha, a, ia+s1, ja, optypea, b, ib+s1, jb, optypeb, 1.0, c, ic, jc, _state);
    }
    if( optypea!=0&&optypeb!=0 )
    {
        rmatrixgemm(m, n, s1, alpha, a, ia, ja, optypea, b, ib, jb, optypeb, beta, c, ic, jc, _state);
        rmatrixgemm(m, n, s2, alpha, a, ia+s1, ja, optypea, b, ib, jb+s1, optypeb, 1.0, c, ic, jc, _state);
    }
    return;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_rmatrixgemm(ae_int_t m,
    ae_int_t n,
    ae_int_t k,
    double alpha,
    /* Real    */ ae_matrix* a,
    ae_int_t ia,
    ae_int_t ja,
    ae_int_t optypea,
    /* Real    */ ae_matrix* b,
    ae_int_t ib,
    ae_int_t jb,
    ae_int_t optypeb,
    double beta,
    /* Real    */ ae_matrix* c,
    ae_int_t ic,
    ae_int_t jc, ae_state *_state)
{
    rmatrixgemm(m,n,k,alpha,a,ia,ja,optypea,b,ib,jb,optypeb,beta,c,ic,jc, _state);
}


/*************************************************************************
This subroutine is an older version of CMatrixHERK(), one with wrong  name
(it is HErmitian update, not SYmmetric). It  is  left  here  for  backward
compatibility.

  -- ALGLIB routine --
     16.12.2009
     Bochkanov Sergey
*************************************************************************/
void cmatrixsyrk(ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     double beta,
     /* Complex */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_bool isupper,
     ae_state *_state)
{


    cmatrixherk(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_cmatrixsyrk(ae_int_t n,
    ae_int_t k,
    double alpha,
    /* Complex */ ae_matrix* a,
    ae_int_t ia,
    ae_int_t ja,
    ae_int_t optypea,
    double beta,
    /* Complex */ ae_matrix* c,
    ae_int_t ic,
    ae_int_t jc,
    ae_bool isupper, ae_state *_state)
{
    cmatrixsyrk(n,k,alpha,a,ia,ja,optypea,beta,c,ic,jc,isupper, _state);
}


/*************************************************************************
Complex ABLASSplitLength

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
static void ablas_ablasinternalsplitlength(ae_int_t n,
     ae_int_t nb,
     ae_int_t* n1,
     ae_int_t* n2,
     ae_state *_state)
{
    ae_int_t r;

    *n1 = 0;
    *n2 = 0;

    if( n<=nb )
    {
        
        /*
         * Block size, no further splitting
         */
        *n1 = n;
        *n2 = 0;
    }
    else
    {
        
        /*
         * Greater than block size
         */
        if( n%nb!=0 )
        {
            
            /*
             * Split remainder
             */
            *n2 = n%nb;
            *n1 = n-(*n2);
        }
        else
        {
            
            /*
             * Split on block boundaries
             */
            *n2 = n/2;
            *n1 = n-(*n2);
            if( *n1%nb==0 )
            {
                return;
            }
            r = nb-*n1%nb;
            *n1 = *n1+r;
            *n2 = *n2-r;
        }
    }
}


/*************************************************************************
Level 2 variant of CMatrixRightTRSM
*************************************************************************/
static void ablas_cmatrixrighttrsm2(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Complex */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_complex vc;
    ae_complex vd;


    
    /*
     * Special case
     */
    if( n*m==0 )
    {
        return;
    }
    
    /*
     * Try to call fast TRSM
     */
    if( cmatrixrighttrsmf(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state) )
    {
        return;
    }
    
    /*
     * General case
     */
    if( isupper )
    {
        
        /*
         * Upper triangular matrix
         */
        if( optype==0 )
        {
            
            /*
             * X*A^(-1)
             */
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( isunit )
                    {
                        vd = ae_complex_from_i(1);
                    }
                    else
                    {
                        vd = a->ptr.pp_complex[i1+j][j1+j];
                    }
                    x->ptr.pp_complex[i2+i][j2+j] = ae_c_div(x->ptr.pp_complex[i2+i][j2+j],vd);
                    if( j<n-1 )
                    {
                        vc = x->ptr.pp_complex[i2+i][j2+j];
                        ae_v_csubc(&x->ptr.pp_complex[i2+i][j2+j+1], 1, &a->ptr.pp_complex[i1+j][j1+j+1], 1, "N", ae_v_len(j2+j+1,j2+n-1), vc);
                    }
                }
            }
            return;
        }
        if( optype==1 )
        {
            
            /*
             * X*A^(-T)
             */
            for(i=0; i<=m-1; i++)
            {
                for(j=n-1; j>=0; j--)
                {
                    vc = ae_complex_from_i(0);
                    vd = ae_complex_from_i(1);
                    if( j<n-1 )
                    {
                        vc = ae_v_cdotproduct(&x->ptr.pp_complex[i2+i][j2+j+1], 1, "N", &a->ptr.pp_complex[i1+j][j1+j+1], 1, "N", ae_v_len(j2+j+1,j2+n-1));
                    }
                    if( !isunit )
                    {
                        vd = a->ptr.pp_complex[i1+j][j1+j];
                    }
                    x->ptr.pp_complex[i2+i][j2+j] = ae_c_div(ae_c_sub(x->ptr.pp_complex[i2+i][j2+j],vc),vd);
                }
            }
            return;
        }
        if( optype==2 )
        {
            
            /*
             * X*A^(-H)
             */
            for(i=0; i<=m-1; i++)
            {
                for(j=n-1; j>=0; j--)
                {
                    vc = ae_complex_from_i(0);
                    vd = ae_complex_from_i(1);
                    if( j<n-1 )
                    {
                        vc = ae_v_cdotproduct(&x->ptr.pp_complex[i2+i][j2+j+1], 1, "N", &a->ptr.pp_complex[i1+j][j1+j+1], 1, "Conj", ae_v_len(j2+j+1,j2+n-1));
                    }
                    if( !isunit )
                    {
                        vd = ae_c_conj(a->ptr.pp_complex[i1+j][j1+j], _state);
                    }
                    x->ptr.pp_complex[i2+i][j2+j] = ae_c_div(ae_c_sub(x->ptr.pp_complex[i2+i][j2+j],vc),vd);
                }
            }
            return;
        }
    }
    else
    {
        
        /*
         * Lower triangular matrix
         */
        if( optype==0 )
        {
            
            /*
             * X*A^(-1)
             */
            for(i=0; i<=m-1; i++)
            {
                for(j=n-1; j>=0; j--)
                {
                    if( isunit )
                    {
                        vd = ae_complex_from_i(1);
                    }
                    else
                    {
                        vd = a->ptr.pp_complex[i1+j][j1+j];
                    }
                    x->ptr.pp_complex[i2+i][j2+j] = ae_c_div(x->ptr.pp_complex[i2+i][j2+j],vd);
                    if( j>0 )
                    {
                        vc = x->ptr.pp_complex[i2+i][j2+j];
                        ae_v_csubc(&x->ptr.pp_complex[i2+i][j2], 1, &a->ptr.pp_complex[i1+j][j1], 1, "N", ae_v_len(j2,j2+j-1), vc);
                    }
                }
            }
            return;
        }
        if( optype==1 )
        {
            
            /*
             * X*A^(-T)
             */
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    vc = ae_complex_from_i(0);
                    vd = ae_complex_from_i(1);
                    if( j>0 )
                    {
                        vc = ae_v_cdotproduct(&x->ptr.pp_complex[i2+i][j2], 1, "N", &a->ptr.pp_complex[i1+j][j1], 1, "N", ae_v_len(j2,j2+j-1));
                    }
                    if( !isunit )
                    {
                        vd = a->ptr.pp_complex[i1+j][j1+j];
                    }
                    x->ptr.pp_complex[i2+i][j2+j] = ae_c_div(ae_c_sub(x->ptr.pp_complex[i2+i][j2+j],vc),vd);
                }
            }
            return;
        }
        if( optype==2 )
        {
            
            /*
             * X*A^(-H)
             */
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    vc = ae_complex_from_i(0);
                    vd = ae_complex_from_i(1);
                    if( j>0 )
                    {
                        vc = ae_v_cdotproduct(&x->ptr.pp_complex[i2+i][j2], 1, "N", &a->ptr.pp_complex[i1+j][j1], 1, "Conj", ae_v_len(j2,j2+j-1));
                    }
                    if( !isunit )
                    {
                        vd = ae_c_conj(a->ptr.pp_complex[i1+j][j1+j], _state);
                    }
                    x->ptr.pp_complex[i2+i][j2+j] = ae_c_div(ae_c_sub(x->ptr.pp_complex[i2+i][j2+j],vc),vd);
                }
            }
            return;
        }
    }
}


/*************************************************************************
Level-2 subroutine
*************************************************************************/
static void ablas_cmatrixlefttrsm2(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Complex */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_complex vc;
    ae_complex vd;


    
    /*
     * Special case
     */
    if( n*m==0 )
    {
        return;
    }
    
    /*
     * Try to call fast TRSM
     */
    if( cmatrixlefttrsmf(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state) )
    {
        return;
    }
    
    /*
     * General case
     */
    if( isupper )
    {
        
        /*
         * Upper triangular matrix
         */
        if( optype==0 )
        {
            
            /*
             * A^(-1)*X
             */
            for(i=m-1; i>=0; i--)
            {
                for(j=i+1; j<=m-1; j++)
                {
                    vc = a->ptr.pp_complex[i1+i][j1+j];
                    ae_v_csubc(&x->ptr.pp_complex[i2+i][j2], 1, &x->ptr.pp_complex[i2+j][j2], 1, "N", ae_v_len(j2,j2+n-1), vc);
                }
                if( !isunit )
                {
                    vd = ae_c_d_div(1,a->ptr.pp_complex[i1+i][j1+i]);
                    ae_v_cmulc(&x->ptr.pp_complex[i2+i][j2], 1, ae_v_len(j2,j2+n-1), vd);
                }
            }
            return;
        }
        if( optype==1 )
        {
            
            /*
             * A^(-T)*X
             */
            for(i=0; i<=m-1; i++)
            {
                if( isunit )
                {
                    vd = ae_complex_from_i(1);
                }
                else
                {
                    vd = ae_c_d_div(1,a->ptr.pp_complex[i1+i][j1+i]);
                }
                ae_v_cmulc(&x->ptr.pp_complex[i2+i][j2], 1, ae_v_len(j2,j2+n-1), vd);
                for(j=i+1; j<=m-1; j++)
                {
                    vc = a->ptr.pp_complex[i1+i][j1+j];
                    ae_v_csubc(&x->ptr.pp_complex[i2+j][j2], 1, &x->ptr.pp_complex[i2+i][j2], 1, "N", ae_v_len(j2,j2+n-1), vc);
                }
            }
            return;
        }
        if( optype==2 )
        {
            
            /*
             * A^(-H)*X
             */
            for(i=0; i<=m-1; i++)
            {
                if( isunit )
                {
                    vd = ae_complex_from_i(1);
                }
                else
                {
                    vd = ae_c_d_div(1,ae_c_conj(a->ptr.pp_complex[i1+i][j1+i], _state));
                }
                ae_v_cmulc(&x->ptr.pp_complex[i2+i][j2], 1, ae_v_len(j2,j2+n-1), vd);
                for(j=i+1; j<=m-1; j++)
                {
                    vc = ae_c_conj(a->ptr.pp_complex[i1+i][j1+j], _state);
                    ae_v_csubc(&x->ptr.pp_complex[i2+j][j2], 1, &x->ptr.pp_complex[i2+i][j2], 1, "N", ae_v_len(j2,j2+n-1), vc);
                }
            }
            return;
        }
    }
    else
    {
        
        /*
         * Lower triangular matrix
         */
        if( optype==0 )
        {
            
            /*
             * A^(-1)*X
             */
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=i-1; j++)
                {
                    vc = a->ptr.pp_complex[i1+i][j1+j];
                    ae_v_csubc(&x->ptr.pp_complex[i2+i][j2], 1, &x->ptr.pp_complex[i2+j][j2], 1, "N", ae_v_len(j2,j2+n-1), vc);
                }
                if( isunit )
                {
                    vd = ae_complex_from_i(1);
                }
                else
                {
                    vd = ae_c_d_div(1,a->ptr.pp_complex[i1+j][j1+j]);
                }
                ae_v_cmulc(&x->ptr.pp_complex[i2+i][j2], 1, ae_v_len(j2,j2+n-1), vd);
            }
            return;
        }
        if( optype==1 )
        {
            
            /*
             * A^(-T)*X
             */
            for(i=m-1; i>=0; i--)
            {
                if( isunit )
                {
                    vd = ae_complex_from_i(1);
                }
                else
                {
                    vd = ae_c_d_div(1,a->ptr.pp_complex[i1+i][j1+i]);
                }
                ae_v_cmulc(&x->ptr.pp_complex[i2+i][j2], 1, ae_v_len(j2,j2+n-1), vd);
                for(j=i-1; j>=0; j--)
                {
                    vc = a->ptr.pp_complex[i1+i][j1+j];
                    ae_v_csubc(&x->ptr.pp_complex[i2+j][j2], 1, &x->ptr.pp_complex[i2+i][j2], 1, "N", ae_v_len(j2,j2+n-1), vc);
                }
            }
            return;
        }
        if( optype==2 )
        {
            
            /*
             * A^(-H)*X
             */
            for(i=m-1; i>=0; i--)
            {
                if( isunit )
                {
                    vd = ae_complex_from_i(1);
                }
                else
                {
                    vd = ae_c_d_div(1,ae_c_conj(a->ptr.pp_complex[i1+i][j1+i], _state));
                }
                ae_v_cmulc(&x->ptr.pp_complex[i2+i][j2], 1, ae_v_len(j2,j2+n-1), vd);
                for(j=i-1; j>=0; j--)
                {
                    vc = ae_c_conj(a->ptr.pp_complex[i1+i][j1+j], _state);
                    ae_v_csubc(&x->ptr.pp_complex[i2+j][j2], 1, &x->ptr.pp_complex[i2+i][j2], 1, "N", ae_v_len(j2,j2+n-1), vc);
                }
            }
            return;
        }
    }
}


/*************************************************************************
Level 2 subroutine

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
static void ablas_rmatrixrighttrsm2(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Real    */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double vr;
    double vd;


    
    /*
     * Special case
     */
    if( n*m==0 )
    {
        return;
    }
    
    /*
     * Try to use "fast" code
     */
    if( rmatrixrighttrsmf(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state) )
    {
        return;
    }
    
    /*
     * General case
     */
    if( isupper )
    {
        
        /*
         * Upper triangular matrix
         */
        if( optype==0 )
        {
            
            /*
             * X*A^(-1)
             */
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( isunit )
                    {
                        vd = (double)(1);
                    }
                    else
                    {
                        vd = a->ptr.pp_double[i1+j][j1+j];
                    }
                    x->ptr.pp_double[i2+i][j2+j] = x->ptr.pp_double[i2+i][j2+j]/vd;
                    if( j<n-1 )
                    {
                        vr = x->ptr.pp_double[i2+i][j2+j];
                        ae_v_subd(&x->ptr.pp_double[i2+i][j2+j+1], 1, &a->ptr.pp_double[i1+j][j1+j+1], 1, ae_v_len(j2+j+1,j2+n-1), vr);
                    }
                }
            }
            return;
        }
        if( optype==1 )
        {
            
            /*
             * X*A^(-T)
             */
            for(i=0; i<=m-1; i++)
            {
                for(j=n-1; j>=0; j--)
                {
                    vr = (double)(0);
                    vd = (double)(1);
                    if( j<n-1 )
                    {
                        vr = ae_v_dotproduct(&x->ptr.pp_double[i2+i][j2+j+1], 1, &a->ptr.pp_double[i1+j][j1+j+1], 1, ae_v_len(j2+j+1,j2+n-1));
                    }
                    if( !isunit )
                    {
                        vd = a->ptr.pp_double[i1+j][j1+j];
                    }
                    x->ptr.pp_double[i2+i][j2+j] = (x->ptr.pp_double[i2+i][j2+j]-vr)/vd;
                }
            }
            return;
        }
    }
    else
    {
        
        /*
         * Lower triangular matrix
         */
        if( optype==0 )
        {
            
            /*
             * X*A^(-1)
             */
            for(i=0; i<=m-1; i++)
            {
                for(j=n-1; j>=0; j--)
                {
                    if( isunit )
                    {
                        vd = (double)(1);
                    }
                    else
                    {
                        vd = a->ptr.pp_double[i1+j][j1+j];
                    }
                    x->ptr.pp_double[i2+i][j2+j] = x->ptr.pp_double[i2+i][j2+j]/vd;
                    if( j>0 )
                    {
                        vr = x->ptr.pp_double[i2+i][j2+j];
                        ae_v_subd(&x->ptr.pp_double[i2+i][j2], 1, &a->ptr.pp_double[i1+j][j1], 1, ae_v_len(j2,j2+j-1), vr);
                    }
                }
            }
            return;
        }
        if( optype==1 )
        {
            
            /*
             * X*A^(-T)
             */
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    vr = (double)(0);
                    vd = (double)(1);
                    if( j>0 )
                    {
                        vr = ae_v_dotproduct(&x->ptr.pp_double[i2+i][j2], 1, &a->ptr.pp_double[i1+j][j1], 1, ae_v_len(j2,j2+j-1));
                    }
                    if( !isunit )
                    {
                        vd = a->ptr.pp_double[i1+j][j1+j];
                    }
                    x->ptr.pp_double[i2+i][j2+j] = (x->ptr.pp_double[i2+i][j2+j]-vr)/vd;
                }
            }
            return;
        }
    }
}


/*************************************************************************
Level 2 subroutine
*************************************************************************/
static void ablas_rmatrixlefttrsm2(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Real    */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double vr;
    double vd;


    
    /*
     * Special case
     */
    if( n==0||m==0 )
    {
        return;
    }
    
    /*
     * Try fast code
     */
    if( rmatrixlefttrsmf(m, n, a, i1, j1, isupper, isunit, optype, x, i2, j2, _state) )
    {
        return;
    }
    
    /*
     * General case
     */
    if( isupper )
    {
        
        /*
         * Upper triangular matrix
         */
        if( optype==0 )
        {
            
            /*
             * A^(-1)*X
             */
            for(i=m-1; i>=0; i--)
            {
                for(j=i+1; j<=m-1; j++)
                {
                    vr = a->ptr.pp_double[i1+i][j1+j];
                    ae_v_subd(&x->ptr.pp_double[i2+i][j2], 1, &x->ptr.pp_double[i2+j][j2], 1, ae_v_len(j2,j2+n-1), vr);
                }
                if( !isunit )
                {
                    vd = 1/a->ptr.pp_double[i1+i][j1+i];
                    ae_v_muld(&x->ptr.pp_double[i2+i][j2], 1, ae_v_len(j2,j2+n-1), vd);
                }
            }
            return;
        }
        if( optype==1 )
        {
            
            /*
             * A^(-T)*X
             */
            for(i=0; i<=m-1; i++)
            {
                if( isunit )
                {
                    vd = (double)(1);
                }
                else
                {
                    vd = 1/a->ptr.pp_double[i1+i][j1+i];
                }
                ae_v_muld(&x->ptr.pp_double[i2+i][j2], 1, ae_v_len(j2,j2+n-1), vd);
                for(j=i+1; j<=m-1; j++)
                {
                    vr = a->ptr.pp_double[i1+i][j1+j];
                    ae_v_subd(&x->ptr.pp_double[i2+j][j2], 1, &x->ptr.pp_double[i2+i][j2], 1, ae_v_len(j2,j2+n-1), vr);
                }
            }
            return;
        }
    }
    else
    {
        
        /*
         * Lower triangular matrix
         */
        if( optype==0 )
        {
            
            /*
             * A^(-1)*X
             */
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=i-1; j++)
                {
                    vr = a->ptr.pp_double[i1+i][j1+j];
                    ae_v_subd(&x->ptr.pp_double[i2+i][j2], 1, &x->ptr.pp_double[i2+j][j2], 1, ae_v_len(j2,j2+n-1), vr);
                }
                if( isunit )
                {
                    vd = (double)(1);
                }
                else
                {
                    vd = 1/a->ptr.pp_double[i1+j][j1+j];
                }
                ae_v_muld(&x->ptr.pp_double[i2+i][j2], 1, ae_v_len(j2,j2+n-1), vd);
            }
            return;
        }
        if( optype==1 )
        {
            
            /*
             * A^(-T)*X
             */
            for(i=m-1; i>=0; i--)
            {
                if( isunit )
                {
                    vd = (double)(1);
                }
                else
                {
                    vd = 1/a->ptr.pp_double[i1+i][j1+i];
                }
                ae_v_muld(&x->ptr.pp_double[i2+i][j2], 1, ae_v_len(j2,j2+n-1), vd);
                for(j=i-1; j>=0; j--)
                {
                    vr = a->ptr.pp_double[i1+i][j1+j];
                    ae_v_subd(&x->ptr.pp_double[i2+j][j2], 1, &x->ptr.pp_double[i2+i][j2], 1, ae_v_len(j2,j2+n-1), vr);
                }
            }
            return;
        }
    }
}


/*************************************************************************
Level 2 subroutine
*************************************************************************/
static void ablas_cmatrixherk2(ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     double beta,
     /* Complex */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_bool isupper,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t j1;
    ae_int_t j2;
    ae_complex v;


    
    /*
     * Fast exit (nothing to be done)
     */
    if( (ae_fp_eq(alpha,(double)(0))||k==0)&&ae_fp_eq(beta,(double)(1)) )
    {
        return;
    }
    
    /*
     * Try to call fast SYRK
     */
    if( cmatrixherkf(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state) )
    {
        return;
    }
    
    /*
     * SYRK
     */
    if( optypea==0 )
    {
        
        /*
         * C=alpha*A*A^H+beta*C
         */
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
                if( ae_fp_neq(alpha,(double)(0))&&k>0 )
                {
                    v = ae_v_cdotproduct(&a->ptr.pp_complex[ia+i][ja], 1, "N", &a->ptr.pp_complex[ia+j][ja], 1, "Conj", ae_v_len(ja,ja+k-1));
                }
                else
                {
                    v = ae_complex_from_i(0);
                }
                if( ae_fp_eq(beta,(double)(0)) )
                {
                    c->ptr.pp_complex[ic+i][jc+j] = ae_c_mul_d(v,alpha);
                }
                else
                {
                    c->ptr.pp_complex[ic+i][jc+j] = ae_c_add(ae_c_mul_d(c->ptr.pp_complex[ic+i][jc+j],beta),ae_c_mul_d(v,alpha));
                }
            }
        }
        return;
    }
    else
    {
        
        /*
         * C=alpha*A^H*A+beta*C
         */
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
            if( ae_fp_eq(beta,(double)(0)) )
            {
                for(j=j1; j<=j2; j++)
                {
                    c->ptr.pp_complex[ic+i][jc+j] = ae_complex_from_i(0);
                }
            }
            else
            {
                ae_v_cmuld(&c->ptr.pp_complex[ic+i][jc+j1], 1, ae_v_len(jc+j1,jc+j2), beta);
            }
        }
        for(i=0; i<=k-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( isupper )
                {
                    j1 = j;
                    j2 = n-1;
                }
                else
                {
                    j1 = 0;
                    j2 = j;
                }
                v = ae_c_mul_d(ae_c_conj(a->ptr.pp_complex[ia+i][ja+j], _state),alpha);
                ae_v_caddc(&c->ptr.pp_complex[ic+j][jc+j1], 1, &a->ptr.pp_complex[ia+i][ja+j1], 1, "N", ae_v_len(jc+j1,jc+j2), v);
            }
        }
        return;
    }
}


/*************************************************************************
Level 2 subrotuine
*************************************************************************/
static void ablas_rmatrixsyrk2(ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     double beta,
     /* Real    */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_bool isupper,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t j1;
    ae_int_t j2;
    double v;


    
    /*
     * Fast exit (nothing to be done)
     */
    if( (ae_fp_eq(alpha,(double)(0))||k==0)&&ae_fp_eq(beta,(double)(1)) )
    {
        return;
    }
    
    /*
     * Try to call fast SYRK
     */
    if( rmatrixsyrkf(n, k, alpha, a, ia, ja, optypea, beta, c, ic, jc, isupper, _state) )
    {
        return;
    }
    
    /*
     * SYRK
     */
    if( optypea==0 )
    {
        
        /*
         * C=alpha*A*A^H+beta*C
         */
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
                if( ae_fp_neq(alpha,(double)(0))&&k>0 )
                {
                    v = ae_v_dotproduct(&a->ptr.pp_double[ia+i][ja], 1, &a->ptr.pp_double[ia+j][ja], 1, ae_v_len(ja,ja+k-1));
                }
                else
                {
                    v = (double)(0);
                }
                if( ae_fp_eq(beta,(double)(0)) )
                {
                    c->ptr.pp_double[ic+i][jc+j] = alpha*v;
                }
                else
                {
                    c->ptr.pp_double[ic+i][jc+j] = beta*c->ptr.pp_double[ic+i][jc+j]+alpha*v;
                }
            }
        }
        return;
    }
    else
    {
        
        /*
         * C=alpha*A^H*A+beta*C
         */
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
            if( ae_fp_eq(beta,(double)(0)) )
            {
                for(j=j1; j<=j2; j++)
                {
                    c->ptr.pp_double[ic+i][jc+j] = (double)(0);
                }
            }
            else
            {
                ae_v_muld(&c->ptr.pp_double[ic+i][jc+j1], 1, ae_v_len(jc+j1,jc+j2), beta);
            }
        }
        for(i=0; i<=k-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( isupper )
                {
                    j1 = j;
                    j2 = n-1;
                }
                else
                {
                    j1 = 0;
                    j2 = j;
                }
                v = alpha*a->ptr.pp_double[ia+i][ja+j];
                ae_v_addd(&c->ptr.pp_double[ic+j][jc+j1], 1, &a->ptr.pp_double[ia+i][ja+j1], 1, ae_v_len(jc+j1,jc+j2), v);
            }
        }
        return;
    }
}


/*$ End $*/
