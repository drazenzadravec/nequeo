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
#include "hsschur.h"


/*$ Declarations $*/
static void hsschur_internalauxschur(ae_bool wantt,
     ae_bool wantz,
     ae_int_t n,
     ae_int_t ilo,
     ae_int_t ihi,
     /* Real    */ ae_matrix* h,
     /* Real    */ ae_vector* wr,
     /* Real    */ ae_vector* wi,
     ae_int_t iloz,
     ae_int_t ihiz,
     /* Real    */ ae_matrix* z,
     /* Real    */ ae_vector* work,
     /* Real    */ ae_vector* workv3,
     /* Real    */ ae_vector* workc1,
     /* Real    */ ae_vector* works1,
     ae_int_t* info,
     ae_state *_state);
static void hsschur_aux2x2schur(double* a,
     double* b,
     double* c,
     double* d,
     double* rt1r,
     double* rt1i,
     double* rt2r,
     double* rt2i,
     double* cs,
     double* sn,
     ae_state *_state);
static double hsschur_extschursign(double a, double b, ae_state *_state);
static ae_int_t hsschur_extschursigntoone(double b, ae_state *_state);


/*$ Body $*/


void rmatrixinternalschurdecomposition(/* Real    */ ae_matrix* h,
     ae_int_t n,
     ae_int_t tneeded,
     ae_int_t zneeded,
     /* Real    */ ae_vector* wr,
     /* Real    */ ae_vector* wi,
     /* Real    */ ae_matrix* z,
     ae_int_t* info,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_matrix h1;
    ae_matrix z1;
    ae_vector wr1;
    ae_vector wi1;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(wr);
    ae_vector_clear(wi);
    *info = 0;
    ae_matrix_init(&h1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&z1, 0, 0, DT_REAL, _state);
    ae_vector_init(&wr1, 0, DT_REAL, _state);
    ae_vector_init(&wi1, 0, DT_REAL, _state);

    
    /*
     * Allocate space
     */
    ae_vector_set_length(wr, n, _state);
    ae_vector_set_length(wi, n, _state);
    if( zneeded==2 )
    {
        rmatrixsetlengthatleast(z, n, n, _state);
    }
    
    /*
     * MKL version
     */
    if( rmatrixinternalschurdecompositionmkl(h, n, tneeded, zneeded, wr, wi, z, info, _state) )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * ALGLIB version
     */
    ae_matrix_set_length(&h1, n+1, n+1, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            h1.ptr.pp_double[1+i][1+j] = h->ptr.pp_double[i][j];
        }
    }
    if( zneeded==1 )
    {
        ae_matrix_set_length(&z1, n+1, n+1, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                z1.ptr.pp_double[1+i][1+j] = z->ptr.pp_double[i][j];
            }
        }
    }
    internalschurdecomposition(&h1, n, tneeded, zneeded, &wr1, &wi1, &z1, info, _state);
    for(i=0; i<=n-1; i++)
    {
        wr->ptr.p_double[i] = wr1.ptr.p_double[i+1];
        wi->ptr.p_double[i] = wi1.ptr.p_double[i+1];
    }
    if( tneeded!=0 )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                h->ptr.pp_double[i][j] = h1.ptr.pp_double[1+i][1+j];
            }
        }
    }
    if( zneeded!=0 )
    {
        rmatrixsetlengthatleast(z, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                z->ptr.pp_double[i][j] = z1.ptr.pp_double[1+i][1+j];
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Subroutine performing  the  Schur  decomposition  of  a  matrix  in  upper
Hessenberg form using the QR algorithm with multiple shifts.

The  source matrix  H  is  represented as  S'*H*S = T, where H - matrix in
upper Hessenberg form,  S - orthogonal matrix (Schur vectors),   T - upper
quasi-triangular matrix (with blocks of sizes  1x1  and  2x2  on  the main
diagonal).

Input parameters:
    H   -   matrix to be decomposed.
            Array whose indexes range within [1..N, 1..N].
    N   -   size of H, N>=0.


Output parameters:
    H   –   contains the matrix T.
            Array whose indexes range within [1..N, 1..N].
            All elements below the blocks on the main diagonal are equal
            to 0.
    S   -   contains Schur vectors.
            Array whose indexes range within [1..N, 1..N].

Note 1:
    The block structure of matrix T could be easily recognized: since  all
    the elements  below  the blocks are zeros, the elements a[i+1,i] which
    are equal to 0 show the block border.

Note 2:
    the algorithm  performance  depends  on  the  value  of  the  internal
    parameter NS of InternalSchurDecomposition  subroutine  which  defines
    the number of shifts in the QR algorithm (analog of  the  block  width
    in block matrix algorithms in linear algebra). If you require  maximum
    performance  on  your  machine,  it  is  recommended  to  adjust  this
    parameter manually.

Result:
    True, if the algorithm has converged and the parameters H and S contain
        the result.
    False, if the algorithm has not converged.

Algorithm implemented on the basis of subroutine DHSEQR (LAPACK 3.0 library).
*************************************************************************/
ae_bool upperhessenbergschurdecomposition(/* Real    */ ae_matrix* h,
     ae_int_t n,
     /* Real    */ ae_matrix* s,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector wi;
    ae_vector wr;
    ae_int_t info;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(s);
    ae_vector_init(&wi, 0, DT_REAL, _state);
    ae_vector_init(&wr, 0, DT_REAL, _state);

    internalschurdecomposition(h, n, 1, 2, &wr, &wi, s, &info, _state);
    result = info==0;
    ae_frame_leave(_state);
    return result;
}


void internalschurdecomposition(/* Real    */ ae_matrix* h,
     ae_int_t n,
     ae_int_t tneeded,
     ae_int_t zneeded,
     /* Real    */ ae_vector* wr,
     /* Real    */ ae_vector* wi,
     /* Real    */ ae_matrix* z,
     ae_int_t* info,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector work;
    ae_int_t i;
    ae_int_t i1;
    ae_int_t i2;
    ae_int_t ierr;
    ae_int_t ii;
    ae_int_t itemp;
    ae_int_t itn;
    ae_int_t its;
    ae_int_t j;
    ae_int_t k;
    ae_int_t l;
    ae_int_t maxb;
    ae_int_t nr;
    ae_int_t ns;
    ae_int_t nv;
    double absw;
    double smlnum;
    double tau;
    double temp;
    double tst1;
    double ulp;
    double unfl;
    ae_matrix s;
    ae_vector v;
    ae_vector vv;
    ae_vector workc1;
    ae_vector works1;
    ae_vector workv3;
    ae_vector tmpwr;
    ae_vector tmpwi;
    ae_bool initz;
    ae_bool wantt;
    ae_bool wantz;
    double cnst;
    ae_bool failflag;
    ae_int_t p1;
    ae_int_t p2;
    double vt;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(wr);
    ae_vector_clear(wi);
    *info = 0;
    ae_vector_init(&work, 0, DT_REAL, _state);
    ae_matrix_init(&s, 0, 0, DT_REAL, _state);
    ae_vector_init(&v, 0, DT_REAL, _state);
    ae_vector_init(&vv, 0, DT_REAL, _state);
    ae_vector_init(&workc1, 0, DT_REAL, _state);
    ae_vector_init(&works1, 0, DT_REAL, _state);
    ae_vector_init(&workv3, 0, DT_REAL, _state);
    ae_vector_init(&tmpwr, 0, DT_REAL, _state);
    ae_vector_init(&tmpwi, 0, DT_REAL, _state);

    
    /*
     * Set the order of the multi-shift QR algorithm to be used.
     * If you want to tune algorithm, change this values
     */
    ns = 12;
    maxb = 50;
    
    /*
     * Now 2 < NS <= MAXB < NH.
     */
    maxb = ae_maxint(3, maxb, _state);
    ns = ae_minint(maxb, ns, _state);
    
    /*
     * Initialize
     */
    cnst = 1.5;
    ae_vector_set_length(&work, ae_maxint(n, 1, _state)+1, _state);
    ae_matrix_set_length(&s, ns+1, ns+1, _state);
    ae_vector_set_length(&v, ns+1+1, _state);
    ae_vector_set_length(&vv, ns+1+1, _state);
    ae_vector_set_length(wr, ae_maxint(n, 1, _state)+1, _state);
    ae_vector_set_length(wi, ae_maxint(n, 1, _state)+1, _state);
    ae_vector_set_length(&workc1, 1+1, _state);
    ae_vector_set_length(&works1, 1+1, _state);
    ae_vector_set_length(&workv3, 3+1, _state);
    ae_vector_set_length(&tmpwr, ae_maxint(n, 1, _state)+1, _state);
    ae_vector_set_length(&tmpwi, ae_maxint(n, 1, _state)+1, _state);
    ae_assert(n>=0, "InternalSchurDecomposition: incorrect N!", _state);
    ae_assert(tneeded==0||tneeded==1, "InternalSchurDecomposition: incorrect TNeeded!", _state);
    ae_assert((zneeded==0||zneeded==1)||zneeded==2, "InternalSchurDecomposition: incorrect ZNeeded!", _state);
    wantt = tneeded==1;
    initz = zneeded==2;
    wantz = zneeded!=0;
    *info = 0;
    
    /*
     * Initialize Z, if necessary
     */
    if( initz )
    {
        rmatrixsetlengthatleast(z, n+1, n+1, _state);
        for(i=1; i<=n; i++)
        {
            for(j=1; j<=n; j++)
            {
                if( i==j )
                {
                    z->ptr.pp_double[i][j] = (double)(1);
                }
                else
                {
                    z->ptr.pp_double[i][j] = (double)(0);
                }
            }
        }
    }
    
    /*
     * Quick return if possible
     */
    if( n==0 )
    {
        ae_frame_leave(_state);
        return;
    }
    if( n==1 )
    {
        wr->ptr.p_double[1] = h->ptr.pp_double[1][1];
        wi->ptr.p_double[1] = (double)(0);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Set rows and columns 1 to N to zero below the first
     * subdiagonal.
     */
    for(j=1; j<=n-2; j++)
    {
        for(i=j+2; i<=n; i++)
        {
            h->ptr.pp_double[i][j] = (double)(0);
        }
    }
    
    /*
     * Test if N is sufficiently small
     */
    if( (ns<=2||ns>n)||maxb>=n )
    {
        
        /*
         * Use the standard double-shift algorithm
         */
        hsschur_internalauxschur(wantt, wantz, n, 1, n, h, wr, wi, 1, n, z, &work, &workv3, &workc1, &works1, info, _state);
        
        /*
         * fill entries under diagonal blocks of T with zeros
         */
        if( wantt )
        {
            j = 1;
            while(j<=n)
            {
                if( ae_fp_eq(wi->ptr.p_double[j],(double)(0)) )
                {
                    for(i=j+1; i<=n; i++)
                    {
                        h->ptr.pp_double[i][j] = (double)(0);
                    }
                    j = j+1;
                }
                else
                {
                    for(i=j+2; i<=n; i++)
                    {
                        h->ptr.pp_double[i][j] = (double)(0);
                        h->ptr.pp_double[i][j+1] = (double)(0);
                    }
                    j = j+2;
                }
            }
        }
        ae_frame_leave(_state);
        return;
    }
    unfl = ae_minrealnumber;
    ulp = 2*ae_machineepsilon;
    smlnum = unfl*(n/ulp);
    
    /*
     * I1 and I2 are the indices of the first row and last column of H
     * to which transformations must be applied. If eigenvalues only are
     * being computed, I1 and I2 are set inside the main loop.
     */
    i1 = 1;
    i2 = n;
    
    /*
     * ITN is the total number of multiple-shift QR iterations allowed.
     */
    itn = 30*n;
    
    /*
     * The main loop begins here. I is the loop index and decreases from
     * IHI to ILO in steps of at most MAXB. Each iteration of the loop
     * works with the active submatrix in rows and columns L to I.
     * Eigenvalues I+1 to IHI have already converged. Either L = ILO or
     * H(L,L-1) is negligible so that the matrix splits.
     */
    i = n;
    for(;;)
    {
        l = 1;
        if( i<1 )
        {
            
            /*
             * fill entries under diagonal blocks of T with zeros
             */
            if( wantt )
            {
                j = 1;
                while(j<=n)
                {
                    if( ae_fp_eq(wi->ptr.p_double[j],(double)(0)) )
                    {
                        for(i=j+1; i<=n; i++)
                        {
                            h->ptr.pp_double[i][j] = (double)(0);
                        }
                        j = j+1;
                    }
                    else
                    {
                        for(i=j+2; i<=n; i++)
                        {
                            h->ptr.pp_double[i][j] = (double)(0);
                            h->ptr.pp_double[i][j+1] = (double)(0);
                        }
                        j = j+2;
                    }
                }
            }
            
            /*
             * Exit
             */
            ae_frame_leave(_state);
            return;
        }
        
        /*
         * Perform multiple-shift QR iterations on rows and columns ILO to I
         * until a submatrix of order at most MAXB splits off at the bottom
         * because a subdiagonal element has become negligible.
         */
        failflag = ae_true;
        for(its=0; its<=itn; its++)
        {
            
            /*
             * Look for a single small subdiagonal element.
             */
            for(k=i; k>=l+1; k--)
            {
                tst1 = ae_fabs(h->ptr.pp_double[k-1][k-1], _state)+ae_fabs(h->ptr.pp_double[k][k], _state);
                if( ae_fp_eq(tst1,(double)(0)) )
                {
                    tst1 = upperhessenberg1norm(h, l, i, l, i, &work, _state);
                }
                if( ae_fp_less_eq(ae_fabs(h->ptr.pp_double[k][k-1], _state),ae_maxreal(ulp*tst1, smlnum, _state)) )
                {
                    break;
                }
            }
            l = k;
            if( l>1 )
            {
                
                /*
                 * H(L,L-1) is negligible.
                 */
                h->ptr.pp_double[l][l-1] = (double)(0);
            }
            
            /*
             * Exit from loop if a submatrix of order <= MAXB has split off.
             */
            if( l>=i-maxb+1 )
            {
                failflag = ae_false;
                break;
            }
            
            /*
             * Now the active submatrix is in rows and columns L to I. If
             * eigenvalues only are being computed, only the active submatrix
             * need be transformed.
             */
            if( its==20||its==30 )
            {
                
                /*
                 * Exceptional shifts.
                 */
                for(ii=i-ns+1; ii<=i; ii++)
                {
                    wr->ptr.p_double[ii] = cnst*(ae_fabs(h->ptr.pp_double[ii][ii-1], _state)+ae_fabs(h->ptr.pp_double[ii][ii], _state));
                    wi->ptr.p_double[ii] = (double)(0);
                }
            }
            else
            {
                
                /*
                 * Use eigenvalues of trailing submatrix of order NS as shifts.
                 */
                copymatrix(h, i-ns+1, i, i-ns+1, i, &s, 1, ns, 1, ns, _state);
                hsschur_internalauxschur(ae_false, ae_false, ns, 1, ns, &s, &tmpwr, &tmpwi, 1, ns, z, &work, &workv3, &workc1, &works1, &ierr, _state);
                for(p1=1; p1<=ns; p1++)
                {
                    wr->ptr.p_double[i-ns+p1] = tmpwr.ptr.p_double[p1];
                    wi->ptr.p_double[i-ns+p1] = tmpwi.ptr.p_double[p1];
                }
                if( ierr>0 )
                {
                    
                    /*
                     * If DLAHQR failed to compute all NS eigenvalues, use the
                     * unconverged diagonal elements as the remaining shifts.
                     */
                    for(ii=1; ii<=ierr; ii++)
                    {
                        wr->ptr.p_double[i-ns+ii] = s.ptr.pp_double[ii][ii];
                        wi->ptr.p_double[i-ns+ii] = (double)(0);
                    }
                }
            }
            
            /*
             * Form the first column of (G-w(1)) (G-w(2)) . . . (G-w(ns))
             * where G is the Hessenberg submatrix H(L:I,L:I) and w is
             * the vector of shifts (stored in WR and WI). The result is
             * stored in the local array V.
             */
            v.ptr.p_double[1] = (double)(1);
            for(ii=2; ii<=ns+1; ii++)
            {
                v.ptr.p_double[ii] = (double)(0);
            }
            nv = 1;
            for(j=i-ns+1; j<=i; j++)
            {
                if( ae_fp_greater_eq(wi->ptr.p_double[j],(double)(0)) )
                {
                    if( ae_fp_eq(wi->ptr.p_double[j],(double)(0)) )
                    {
                        
                        /*
                         * real shift
                         */
                        p1 = nv+1;
                        ae_v_move(&vv.ptr.p_double[1], 1, &v.ptr.p_double[1], 1, ae_v_len(1,p1));
                        matrixvectormultiply(h, l, l+nv, l, l+nv-1, ae_false, &vv, 1, nv, 1.0, &v, 1, nv+1, -wr->ptr.p_double[j], _state);
                        nv = nv+1;
                    }
                    else
                    {
                        if( ae_fp_greater(wi->ptr.p_double[j],(double)(0)) )
                        {
                            
                            /*
                             * complex conjugate pair of shifts
                             */
                            p1 = nv+1;
                            ae_v_move(&vv.ptr.p_double[1], 1, &v.ptr.p_double[1], 1, ae_v_len(1,p1));
                            matrixvectormultiply(h, l, l+nv, l, l+nv-1, ae_false, &v, 1, nv, 1.0, &vv, 1, nv+1, -2*wr->ptr.p_double[j], _state);
                            itemp = vectoridxabsmax(&vv, 1, nv+1, _state);
                            temp = 1/ae_maxreal(ae_fabs(vv.ptr.p_double[itemp], _state), smlnum, _state);
                            p1 = nv+1;
                            ae_v_muld(&vv.ptr.p_double[1], 1, ae_v_len(1,p1), temp);
                            absw = pythag2(wr->ptr.p_double[j], wi->ptr.p_double[j], _state);
                            temp = temp*absw*absw;
                            matrixvectormultiply(h, l, l+nv+1, l, l+nv, ae_false, &vv, 1, nv+1, 1.0, &v, 1, nv+2, temp, _state);
                            nv = nv+2;
                        }
                    }
                    
                    /*
                     * Scale V(1:NV) so that max(abs(V(i))) = 1. If V is zero,
                     * reset it to the unit vector.
                     */
                    itemp = vectoridxabsmax(&v, 1, nv, _state);
                    temp = ae_fabs(v.ptr.p_double[itemp], _state);
                    if( ae_fp_eq(temp,(double)(0)) )
                    {
                        v.ptr.p_double[1] = (double)(1);
                        for(ii=2; ii<=nv; ii++)
                        {
                            v.ptr.p_double[ii] = (double)(0);
                        }
                    }
                    else
                    {
                        temp = ae_maxreal(temp, smlnum, _state);
                        vt = 1/temp;
                        ae_v_muld(&v.ptr.p_double[1], 1, ae_v_len(1,nv), vt);
                    }
                }
            }
            
            /*
             * Multiple-shift QR step
             */
            for(k=l; k<=i-1; k++)
            {
                
                /*
                 * The first iteration of this loop determines a reflection G
                 * from the vector V and applies it from left and right to H,
                 * thus creating a nonzero bulge below the subdiagonal.
                 *
                 * Each subsequent iteration determines a reflection G to
                 * restore the Hessenberg form in the (K-1)th column, and thus
                 * chases the bulge one step toward the bottom of the active
                 * submatrix. NR is the order of G.
                 */
                nr = ae_minint(ns+1, i-k+1, _state);
                if( k>l )
                {
                    p1 = k-1;
                    p2 = k+nr-1;
                    ae_v_move(&v.ptr.p_double[1], 1, &h->ptr.pp_double[k][p1], h->stride, ae_v_len(1,nr));
                    touchint(&p2, _state);
                }
                generatereflection(&v, nr, &tau, _state);
                if( k>l )
                {
                    h->ptr.pp_double[k][k-1] = v.ptr.p_double[1];
                    for(ii=k+1; ii<=i; ii++)
                    {
                        h->ptr.pp_double[ii][k-1] = (double)(0);
                    }
                }
                v.ptr.p_double[1] = (double)(1);
                
                /*
                 * Apply G from the left to transform the rows of the matrix in
                 * columns K to I2.
                 */
                applyreflectionfromtheleft(h, tau, &v, k, k+nr-1, k, i2, &work, _state);
                
                /*
                 * Apply G from the right to transform the columns of the
                 * matrix in rows I1 to min(K+NR,I).
                 */
                applyreflectionfromtheright(h, tau, &v, i1, ae_minint(k+nr, i, _state), k, k+nr-1, &work, _state);
                if( wantz )
                {
                    
                    /*
                     * Accumulate transformations in the matrix Z
                     */
                    applyreflectionfromtheright(z, tau, &v, 1, n, k, k+nr-1, &work, _state);
                }
            }
        }
        
        /*
         * Failure to converge in remaining number of iterations
         */
        if( failflag )
        {
            *info = i;
            ae_frame_leave(_state);
            return;
        }
        
        /*
         * A submatrix of order <= MAXB in rows and columns L to I has split
         * off. Use the double-shift QR algorithm to handle it.
         */
        hsschur_internalauxschur(wantt, wantz, n, l, i, h, wr, wi, 1, n, z, &work, &workv3, &workc1, &works1, info, _state);
        if( *info>0 )
        {
            ae_frame_leave(_state);
            return;
        }
        
        /*
         * Decrement number of remaining iterations, and return to start of
         * the main loop with a new value of I.
         */
        itn = itn-its;
        i = l-1;
    }
    ae_frame_leave(_state);
}


static void hsschur_internalauxschur(ae_bool wantt,
     ae_bool wantz,
     ae_int_t n,
     ae_int_t ilo,
     ae_int_t ihi,
     /* Real    */ ae_matrix* h,
     /* Real    */ ae_vector* wr,
     /* Real    */ ae_vector* wi,
     ae_int_t iloz,
     ae_int_t ihiz,
     /* Real    */ ae_matrix* z,
     /* Real    */ ae_vector* work,
     /* Real    */ ae_vector* workv3,
     /* Real    */ ae_vector* workc1,
     /* Real    */ ae_vector* works1,
     ae_int_t* info,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t i1;
    ae_int_t i2;
    ae_int_t itn;
    ae_int_t its;
    ae_int_t j;
    ae_int_t k;
    ae_int_t l;
    ae_int_t m;
    ae_int_t nh;
    ae_int_t nr;
    ae_int_t nz;
    double ave;
    double cs;
    double disc;
    double h00;
    double h10;
    double h11;
    double h12;
    double h21;
    double h22;
    double h33;
    double h33s;
    double h43h34;
    double h44;
    double h44s;
    double s;
    double smlnum;
    double sn;
    double sum;
    double t1;
    double t2;
    double t3;
    double tst1;
    double unfl;
    double v1;
    double v2;
    double v3;
    ae_bool failflag;
    double dat1;
    double dat2;
    ae_int_t p1;
    double him1im1;
    double him1i;
    double hiim1;
    double hii;
    double wrim1;
    double wri;
    double wiim1;
    double wii;
    double ulp;

    *info = 0;

    *info = 0;
    dat1 = 0.75;
    dat2 = -0.4375;
    ulp = ae_machineepsilon;
    
    /*
     * Quick return if possible
     */
    if( n==0 )
    {
        return;
    }
    if( ilo==ihi )
    {
        wr->ptr.p_double[ilo] = h->ptr.pp_double[ilo][ilo];
        wi->ptr.p_double[ilo] = (double)(0);
        return;
    }
    nh = ihi-ilo+1;
    nz = ihiz-iloz+1;
    
    /*
     * Set machine-dependent constants for the stopping criterion.
     * If norm(H) <= sqrt(MaxRealNumber), overflow should not occur.
     */
    unfl = ae_minrealnumber;
    smlnum = unfl*(nh/ulp);
    
    /*
     * I1 and I2 are the indices of the first row and last column of H
     * to which transformations must be applied. If eigenvalues only are
     * being computed, I1 and I2 are set inside the main loop.
     */
    i1 = 1;
    i2 = n;
    
    /*
     * ITN is the total number of QR iterations allowed.
     */
    itn = 30*nh;
    
    /*
     * The main loop begins here. I is the loop index and decreases from
     * IHI to ILO in steps of 1 or 2. Each iteration of the loop works
     * with the active submatrix in rows and columns L to I.
     * Eigenvalues I+1 to IHI have already converged. Either L = ILO or
     * H(L,L-1) is negligible so that the matrix splits.
     */
    i = ihi;
    for(;;)
    {
        l = ilo;
        if( i<ilo )
        {
            return;
        }
        
        /*
         * Perform QR iterations on rows and columns ILO to I until a
         * submatrix of order 1 or 2 splits off at the bottom because a
         * subdiagonal element has become negligible.
         */
        failflag = ae_true;
        for(its=0; its<=itn; its++)
        {
            
            /*
             * Look for a single small subdiagonal element.
             */
            for(k=i; k>=l+1; k--)
            {
                tst1 = ae_fabs(h->ptr.pp_double[k-1][k-1], _state)+ae_fabs(h->ptr.pp_double[k][k], _state);
                if( ae_fp_eq(tst1,(double)(0)) )
                {
                    tst1 = upperhessenberg1norm(h, l, i, l, i, work, _state);
                }
                if( ae_fp_less_eq(ae_fabs(h->ptr.pp_double[k][k-1], _state),ae_maxreal(ulp*tst1, smlnum, _state)) )
                {
                    break;
                }
            }
            l = k;
            if( l>ilo )
            {
                
                /*
                 * H(L,L-1) is negligible
                 */
                h->ptr.pp_double[l][l-1] = (double)(0);
            }
            
            /*
             * Exit from loop if a submatrix of order 1 or 2 has split off.
             */
            if( l>=i-1 )
            {
                failflag = ae_false;
                break;
            }
            
            /*
             * Now the active submatrix is in rows and columns L to I. If
             * eigenvalues only are being computed, only the active submatrix
             * need be transformed.
             */
            if( its==10||its==20 )
            {
                
                /*
                 * Exceptional shift.
                 */
                s = ae_fabs(h->ptr.pp_double[i][i-1], _state)+ae_fabs(h->ptr.pp_double[i-1][i-2], _state);
                h44 = dat1*s+h->ptr.pp_double[i][i];
                h33 = h44;
                h43h34 = dat2*s*s;
            }
            else
            {
                
                /*
                 * Prepare to use Francis' double shift
                 * (i.e. 2nd degree generalized Rayleigh quotient)
                 */
                h44 = h->ptr.pp_double[i][i];
                h33 = h->ptr.pp_double[i-1][i-1];
                h43h34 = h->ptr.pp_double[i][i-1]*h->ptr.pp_double[i-1][i];
                s = h->ptr.pp_double[i-1][i-2]*h->ptr.pp_double[i-1][i-2];
                disc = (h33-h44)*0.5;
                disc = disc*disc+h43h34;
                if( ae_fp_greater(disc,(double)(0)) )
                {
                    
                    /*
                     * Real roots: use Wilkinson's shift twice
                     */
                    disc = ae_sqrt(disc, _state);
                    ave = 0.5*(h33+h44);
                    if( ae_fp_greater(ae_fabs(h33, _state)-ae_fabs(h44, _state),(double)(0)) )
                    {
                        h33 = h33*h44-h43h34;
                        h44 = h33/(hsschur_extschursign(disc, ave, _state)+ave);
                    }
                    else
                    {
                        h44 = hsschur_extschursign(disc, ave, _state)+ave;
                    }
                    h33 = h44;
                    h43h34 = (double)(0);
                }
            }
            
            /*
             * Look for two consecutive small subdiagonal elements.
             */
            for(m=i-2; m>=l; m--)
            {
                
                /*
                 * Determine the effect of starting the double-shift QR
                 * iteration at row M, and see if this would make H(M,M-1)
                 * negligible.
                 */
                h11 = h->ptr.pp_double[m][m];
                h22 = h->ptr.pp_double[m+1][m+1];
                h21 = h->ptr.pp_double[m+1][m];
                h12 = h->ptr.pp_double[m][m+1];
                h44s = h44-h11;
                h33s = h33-h11;
                v1 = (h33s*h44s-h43h34)/h21+h12;
                v2 = h22-h11-h33s-h44s;
                v3 = h->ptr.pp_double[m+2][m+1];
                s = ae_fabs(v1, _state)+ae_fabs(v2, _state)+ae_fabs(v3, _state);
                v1 = v1/s;
                v2 = v2/s;
                v3 = v3/s;
                workv3->ptr.p_double[1] = v1;
                workv3->ptr.p_double[2] = v2;
                workv3->ptr.p_double[3] = v3;
                if( m==l )
                {
                    break;
                }
                h00 = h->ptr.pp_double[m-1][m-1];
                h10 = h->ptr.pp_double[m][m-1];
                tst1 = ae_fabs(v1, _state)*(ae_fabs(h00, _state)+ae_fabs(h11, _state)+ae_fabs(h22, _state));
                if( ae_fp_less_eq(ae_fabs(h10, _state)*(ae_fabs(v2, _state)+ae_fabs(v3, _state)),ulp*tst1) )
                {
                    break;
                }
            }
            
            /*
             * Double-shift QR step
             */
            for(k=m; k<=i-1; k++)
            {
                
                /*
                 * The first iteration of this loop determines a reflection G
                 * from the vector V and applies it from left and right to H,
                 * thus creating a nonzero bulge below the subdiagonal.
                 *
                 * Each subsequent iteration determines a reflection G to
                 * restore the Hessenberg form in the (K-1)th column, and thus
                 * chases the bulge one step toward the bottom of the active
                 * submatrix. NR is the order of G.
                 */
                nr = ae_minint(3, i-k+1, _state);
                if( k>m )
                {
                    for(p1=1; p1<=nr; p1++)
                    {
                        workv3->ptr.p_double[p1] = h->ptr.pp_double[k+p1-1][k-1];
                    }
                }
                generatereflection(workv3, nr, &t1, _state);
                if( k>m )
                {
                    h->ptr.pp_double[k][k-1] = workv3->ptr.p_double[1];
                    h->ptr.pp_double[k+1][k-1] = (double)(0);
                    if( k<i-1 )
                    {
                        h->ptr.pp_double[k+2][k-1] = (double)(0);
                    }
                }
                else
                {
                    if( m>l )
                    {
                        h->ptr.pp_double[k][k-1] = -h->ptr.pp_double[k][k-1];
                    }
                }
                v2 = workv3->ptr.p_double[2];
                t2 = t1*v2;
                if( nr==3 )
                {
                    v3 = workv3->ptr.p_double[3];
                    t3 = t1*v3;
                    
                    /*
                     * Apply G from the left to transform the rows of the matrix
                     * in columns K to I2.
                     */
                    for(j=k; j<=i2; j++)
                    {
                        sum = h->ptr.pp_double[k][j]+v2*h->ptr.pp_double[k+1][j]+v3*h->ptr.pp_double[k+2][j];
                        h->ptr.pp_double[k][j] = h->ptr.pp_double[k][j]-sum*t1;
                        h->ptr.pp_double[k+1][j] = h->ptr.pp_double[k+1][j]-sum*t2;
                        h->ptr.pp_double[k+2][j] = h->ptr.pp_double[k+2][j]-sum*t3;
                    }
                    
                    /*
                     * Apply G from the right to transform the columns of the
                     * matrix in rows I1 to min(K+3,I).
                     */
                    for(j=i1; j<=ae_minint(k+3, i, _state); j++)
                    {
                        sum = h->ptr.pp_double[j][k]+v2*h->ptr.pp_double[j][k+1]+v3*h->ptr.pp_double[j][k+2];
                        h->ptr.pp_double[j][k] = h->ptr.pp_double[j][k]-sum*t1;
                        h->ptr.pp_double[j][k+1] = h->ptr.pp_double[j][k+1]-sum*t2;
                        h->ptr.pp_double[j][k+2] = h->ptr.pp_double[j][k+2]-sum*t3;
                    }
                    if( wantz )
                    {
                        
                        /*
                         * Accumulate transformations in the matrix Z
                         */
                        for(j=iloz; j<=ihiz; j++)
                        {
                            sum = z->ptr.pp_double[j][k]+v2*z->ptr.pp_double[j][k+1]+v3*z->ptr.pp_double[j][k+2];
                            z->ptr.pp_double[j][k] = z->ptr.pp_double[j][k]-sum*t1;
                            z->ptr.pp_double[j][k+1] = z->ptr.pp_double[j][k+1]-sum*t2;
                            z->ptr.pp_double[j][k+2] = z->ptr.pp_double[j][k+2]-sum*t3;
                        }
                    }
                }
                else
                {
                    if( nr==2 )
                    {
                        
                        /*
                         * Apply G from the left to transform the rows of the matrix
                         * in columns K to I2.
                         */
                        for(j=k; j<=i2; j++)
                        {
                            sum = h->ptr.pp_double[k][j]+v2*h->ptr.pp_double[k+1][j];
                            h->ptr.pp_double[k][j] = h->ptr.pp_double[k][j]-sum*t1;
                            h->ptr.pp_double[k+1][j] = h->ptr.pp_double[k+1][j]-sum*t2;
                        }
                        
                        /*
                         * Apply G from the right to transform the columns of the
                         * matrix in rows I1 to min(K+3,I).
                         */
                        for(j=i1; j<=i; j++)
                        {
                            sum = h->ptr.pp_double[j][k]+v2*h->ptr.pp_double[j][k+1];
                            h->ptr.pp_double[j][k] = h->ptr.pp_double[j][k]-sum*t1;
                            h->ptr.pp_double[j][k+1] = h->ptr.pp_double[j][k+1]-sum*t2;
                        }
                        if( wantz )
                        {
                            
                            /*
                             * Accumulate transformations in the matrix Z
                             */
                            for(j=iloz; j<=ihiz; j++)
                            {
                                sum = z->ptr.pp_double[j][k]+v2*z->ptr.pp_double[j][k+1];
                                z->ptr.pp_double[j][k] = z->ptr.pp_double[j][k]-sum*t1;
                                z->ptr.pp_double[j][k+1] = z->ptr.pp_double[j][k+1]-sum*t2;
                            }
                        }
                    }
                }
            }
        }
        if( failflag )
        {
            
            /*
             * Failure to converge in remaining number of iterations
             */
            *info = i;
            return;
        }
        if( l==i )
        {
            
            /*
             * H(I,I-1) is negligible: one eigenvalue has converged.
             */
            wr->ptr.p_double[i] = h->ptr.pp_double[i][i];
            wi->ptr.p_double[i] = (double)(0);
        }
        else
        {
            if( l==i-1 )
            {
                
                /*
                 * H(I-1,I-2) is negligible: a pair of eigenvalues have converged.
                 *
                 *        Transform the 2-by-2 submatrix to standard Schur form,
                 *        and compute and store the eigenvalues.
                 */
                him1im1 = h->ptr.pp_double[i-1][i-1];
                him1i = h->ptr.pp_double[i-1][i];
                hiim1 = h->ptr.pp_double[i][i-1];
                hii = h->ptr.pp_double[i][i];
                hsschur_aux2x2schur(&him1im1, &him1i, &hiim1, &hii, &wrim1, &wiim1, &wri, &wii, &cs, &sn, _state);
                wr->ptr.p_double[i-1] = wrim1;
                wi->ptr.p_double[i-1] = wiim1;
                wr->ptr.p_double[i] = wri;
                wi->ptr.p_double[i] = wii;
                h->ptr.pp_double[i-1][i-1] = him1im1;
                h->ptr.pp_double[i-1][i] = him1i;
                h->ptr.pp_double[i][i-1] = hiim1;
                h->ptr.pp_double[i][i] = hii;
                if( wantt )
                {
                    
                    /*
                     * Apply the transformation to the rest of H.
                     */
                    if( i2>i )
                    {
                        workc1->ptr.p_double[1] = cs;
                        works1->ptr.p_double[1] = sn;
                        applyrotationsfromtheleft(ae_true, i-1, i, i+1, i2, workc1, works1, h, work, _state);
                    }
                    workc1->ptr.p_double[1] = cs;
                    works1->ptr.p_double[1] = sn;
                    applyrotationsfromtheright(ae_true, i1, i-2, i-1, i, workc1, works1, h, work, _state);
                }
                if( wantz )
                {
                    
                    /*
                     * Apply the transformation to Z.
                     */
                    workc1->ptr.p_double[1] = cs;
                    works1->ptr.p_double[1] = sn;
                    applyrotationsfromtheright(ae_true, iloz, iloz+nz-1, i-1, i, workc1, works1, z, work, _state);
                }
            }
        }
        
        /*
         * Decrement number of remaining iterations, and return to start of
         * the main loop with new value of I.
         */
        itn = itn-its;
        i = l-1;
    }
}


static void hsschur_aux2x2schur(double* a,
     double* b,
     double* c,
     double* d,
     double* rt1r,
     double* rt1i,
     double* rt2r,
     double* rt2i,
     double* cs,
     double* sn,
     ae_state *_state)
{
    double multpl;
    double aa;
    double bb;
    double bcmax;
    double bcmis;
    double cc;
    double cs1;
    double dd;
    double eps;
    double p;
    double sab;
    double sac;
    double scl;
    double sigma;
    double sn1;
    double tau;
    double temp;
    double z;

    *rt1r = 0;
    *rt1i = 0;
    *rt2r = 0;
    *rt2i = 0;
    *cs = 0;
    *sn = 0;

    multpl = 4.0;
    eps = ae_machineepsilon;
    if( ae_fp_eq(*c,(double)(0)) )
    {
        *cs = (double)(1);
        *sn = (double)(0);
    }
    else
    {
        if( ae_fp_eq(*b,(double)(0)) )
        {
            
            /*
             * Swap rows and columns
             */
            *cs = (double)(0);
            *sn = (double)(1);
            temp = *d;
            *d = *a;
            *a = temp;
            *b = -*c;
            *c = (double)(0);
        }
        else
        {
            if( ae_fp_eq(*a-(*d),(double)(0))&&hsschur_extschursigntoone(*b, _state)!=hsschur_extschursigntoone(*c, _state) )
            {
                *cs = (double)(1);
                *sn = (double)(0);
            }
            else
            {
                temp = *a-(*d);
                p = 0.5*temp;
                bcmax = ae_maxreal(ae_fabs(*b, _state), ae_fabs(*c, _state), _state);
                bcmis = ae_minreal(ae_fabs(*b, _state), ae_fabs(*c, _state), _state)*hsschur_extschursigntoone(*b, _state)*hsschur_extschursigntoone(*c, _state);
                scl = ae_maxreal(ae_fabs(p, _state), bcmax, _state);
                z = p/scl*p+bcmax/scl*bcmis;
                
                /*
                 * If Z is of the order of the machine accuracy, postpone the
                 * decision on the nature of eigenvalues
                 */
                if( ae_fp_greater_eq(z,multpl*eps) )
                {
                    
                    /*
                     * Real eigenvalues. Compute A and D.
                     */
                    z = p+hsschur_extschursign(ae_sqrt(scl, _state)*ae_sqrt(z, _state), p, _state);
                    *a = *d+z;
                    *d = *d-bcmax/z*bcmis;
                    
                    /*
                     * Compute B and the rotation matrix
                     */
                    tau = pythag2(*c, z, _state);
                    *cs = z/tau;
                    *sn = *c/tau;
                    *b = *b-(*c);
                    *c = (double)(0);
                }
                else
                {
                    
                    /*
                     * Complex eigenvalues, or real (almost) equal eigenvalues.
                     * Make diagonal elements equal.
                     */
                    sigma = *b+(*c);
                    tau = pythag2(sigma, temp, _state);
                    *cs = ae_sqrt(0.5*(1+ae_fabs(sigma, _state)/tau), _state);
                    *sn = -p/(tau*(*cs))*hsschur_extschursign((double)(1), sigma, _state);
                    
                    /*
                     * Compute [ AA  BB ] = [ A  B ] [ CS -SN ]
                     *         [ CC  DD ]   [ C  D ] [ SN  CS ]
                     */
                    aa = *a*(*cs)+*b*(*sn);
                    bb = -*a*(*sn)+*b*(*cs);
                    cc = *c*(*cs)+*d*(*sn);
                    dd = -*c*(*sn)+*d*(*cs);
                    
                    /*
                     * Compute [ A  B ] = [ CS  SN ] [ AA  BB ]
                     *         [ C  D ]   [-SN  CS ] [ CC  DD ]
                     */
                    *a = aa*(*cs)+cc*(*sn);
                    *b = bb*(*cs)+dd*(*sn);
                    *c = -aa*(*sn)+cc*(*cs);
                    *d = -bb*(*sn)+dd*(*cs);
                    temp = 0.5*(*a+(*d));
                    *a = temp;
                    *d = temp;
                    if( ae_fp_neq(*c,(double)(0)) )
                    {
                        if( ae_fp_neq(*b,(double)(0)) )
                        {
                            if( hsschur_extschursigntoone(*b, _state)==hsschur_extschursigntoone(*c, _state) )
                            {
                                
                                /*
                                 * Real eigenvalues: reduce to upper triangular form
                                 */
                                sab = ae_sqrt(ae_fabs(*b, _state), _state);
                                sac = ae_sqrt(ae_fabs(*c, _state), _state);
                                p = hsschur_extschursign(sab*sac, *c, _state);
                                tau = 1/ae_sqrt(ae_fabs(*b+(*c), _state), _state);
                                *a = temp+p;
                                *d = temp-p;
                                *b = *b-(*c);
                                *c = (double)(0);
                                cs1 = sab*tau;
                                sn1 = sac*tau;
                                temp = *cs*cs1-*sn*sn1;
                                *sn = *cs*sn1+*sn*cs1;
                                *cs = temp;
                            }
                        }
                        else
                        {
                            *b = -*c;
                            *c = (double)(0);
                            temp = *cs;
                            *cs = -*sn;
                            *sn = temp;
                        }
                    }
                }
            }
        }
    }
    
    /*
     * Store eigenvalues in (RT1R,RT1I) and (RT2R,RT2I).
     */
    *rt1r = *a;
    *rt2r = *d;
    if( ae_fp_eq(*c,(double)(0)) )
    {
        *rt1i = (double)(0);
        *rt2i = (double)(0);
    }
    else
    {
        *rt1i = ae_sqrt(ae_fabs(*b, _state), _state)*ae_sqrt(ae_fabs(*c, _state), _state);
        *rt2i = -*rt1i;
    }
}


static double hsschur_extschursign(double a, double b, ae_state *_state)
{
    double result;


    if( ae_fp_greater_eq(b,(double)(0)) )
    {
        result = ae_fabs(a, _state);
    }
    else
    {
        result = -ae_fabs(a, _state);
    }
    return result;
}


static ae_int_t hsschur_extschursigntoone(double b, ae_state *_state)
{
    ae_int_t result;


    if( ae_fp_greater_eq(b,(double)(0)) )
    {
        result = 1;
    }
    else
    {
        result = -1;
    }
    return result;
}


/*$ End $*/
