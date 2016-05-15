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
#include "trlinsolve.h"


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
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool normin;
    ae_vector cnorm;
    ae_matrix a1;
    ae_vector x1;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    *s = 0;
    ae_vector_init(&cnorm, 0, DT_REAL, _state);
    ae_matrix_init(&a1, 0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);

    
    /*
     * From 0-based to 1-based
     */
    normin = ae_false;
    ae_matrix_set_length(&a1, n+1, n+1, _state);
    ae_vector_set_length(&x1, n+1, _state);
    for(i=1; i<=n; i++)
    {
        ae_v_move(&a1.ptr.pp_double[i][1], 1, &a->ptr.pp_double[i-1][0], 1, ae_v_len(1,n));
    }
    ae_v_move(&x1.ptr.p_double[1], 1, &x->ptr.p_double[0], 1, ae_v_len(1,n));
    
    /*
     * Solve 1-based
     */
    safesolvetriangular(&a1, n, &x1, s, isupper, istrans, isunit, normin, &cnorm, _state);
    
    /*
     * From 1-based to 0-based
     */
    ae_v_move(&x->ptr.p_double[0], 1, &x1.ptr.p_double[1], 1, ae_v_len(0,n-1));
    ae_frame_leave(_state);
}


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
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t imax;
    ae_int_t j;
    ae_int_t jfirst;
    ae_int_t jinc;
    ae_int_t jlast;
    ae_int_t jm1;
    ae_int_t jp1;
    ae_int_t ip1;
    ae_int_t im1;
    ae_int_t k;
    ae_int_t flg;
    double v;
    double vd;
    double bignum;
    double grow;
    double rec;
    double smlnum;
    double sumj;
    double tjj;
    double tjjs;
    double tmax;
    double tscal;
    double uscal;
    double xbnd;
    double xj;
    double xmax;
    ae_bool notran;
    ae_bool upper;
    ae_bool nounit;

    *s = 0;

    upper = isupper;
    notran = !istrans;
    nounit = !isunit;
    
    /*
     * these initializers are not really necessary,
     * but without them compiler complains about uninitialized locals
     */
    tjjs = (double)(0);
    
    /*
     * Quick return if possible
     */
    if( n==0 )
    {
        return;
    }
    
    /*
     * Determine machine dependent parameters to control overflow.
     */
    smlnum = ae_minrealnumber/(ae_machineepsilon*2);
    bignum = 1/smlnum;
    *s = (double)(1);
    if( !normin )
    {
        ae_vector_set_length(cnorm, n+1, _state);
        
        /*
         * Compute the 1-norm of each column, not including the diagonal.
         */
        if( upper )
        {
            
            /*
             * A is upper triangular.
             */
            for(j=1; j<=n; j++)
            {
                v = (double)(0);
                for(k=1; k<=j-1; k++)
                {
                    v = v+ae_fabs(a->ptr.pp_double[k][j], _state);
                }
                cnorm->ptr.p_double[j] = v;
            }
        }
        else
        {
            
            /*
             * A is lower triangular.
             */
            for(j=1; j<=n-1; j++)
            {
                v = (double)(0);
                for(k=j+1; k<=n; k++)
                {
                    v = v+ae_fabs(a->ptr.pp_double[k][j], _state);
                }
                cnorm->ptr.p_double[j] = v;
            }
            cnorm->ptr.p_double[n] = (double)(0);
        }
    }
    
    /*
     * Scale the column norms by TSCAL if the maximum element in CNORM is
     * greater than BIGNUM.
     */
    imax = 1;
    for(k=2; k<=n; k++)
    {
        if( ae_fp_greater(cnorm->ptr.p_double[k],cnorm->ptr.p_double[imax]) )
        {
            imax = k;
        }
    }
    tmax = cnorm->ptr.p_double[imax];
    if( ae_fp_less_eq(tmax,bignum) )
    {
        tscal = (double)(1);
    }
    else
    {
        tscal = 1/(smlnum*tmax);
        ae_v_muld(&cnorm->ptr.p_double[1], 1, ae_v_len(1,n), tscal);
    }
    
    /*
     * Compute a bound on the computed solution vector to see if the
     * Level 2 BLAS routine DTRSV can be used.
     */
    j = 1;
    for(k=2; k<=n; k++)
    {
        if( ae_fp_greater(ae_fabs(x->ptr.p_double[k], _state),ae_fabs(x->ptr.p_double[j], _state)) )
        {
            j = k;
        }
    }
    xmax = ae_fabs(x->ptr.p_double[j], _state);
    xbnd = xmax;
    if( notran )
    {
        
        /*
         * Compute the growth in A * x = b.
         */
        if( upper )
        {
            jfirst = n;
            jlast = 1;
            jinc = -1;
        }
        else
        {
            jfirst = 1;
            jlast = n;
            jinc = 1;
        }
        if( ae_fp_neq(tscal,(double)(1)) )
        {
            grow = (double)(0);
        }
        else
        {
            if( nounit )
            {
                
                /*
                 * A is non-unit triangular.
                 *
                 * Compute GROW = 1/G(j) and XBND = 1/M(j).
                 * Initially, G(0) = max{x(i), i=1,...,n}.
                 */
                grow = 1/ae_maxreal(xbnd, smlnum, _state);
                xbnd = grow;
                j = jfirst;
                while((jinc>0&&j<=jlast)||(jinc<0&&j>=jlast))
                {
                    
                    /*
                     * Exit the loop if the growth factor is too small.
                     */
                    if( ae_fp_less_eq(grow,smlnum) )
                    {
                        break;
                    }
                    
                    /*
                     * M(j) = G(j-1) / abs(A(j,j))
                     */
                    tjj = ae_fabs(a->ptr.pp_double[j][j], _state);
                    xbnd = ae_minreal(xbnd, ae_minreal((double)(1), tjj, _state)*grow, _state);
                    if( ae_fp_greater_eq(tjj+cnorm->ptr.p_double[j],smlnum) )
                    {
                        
                        /*
                         * G(j) = G(j-1)*( 1 + CNORM(j) / abs(A(j,j)) )
                         */
                        grow = grow*(tjj/(tjj+cnorm->ptr.p_double[j]));
                    }
                    else
                    {
                        
                        /*
                         * G(j) could overflow, set GROW to 0.
                         */
                        grow = (double)(0);
                    }
                    if( j==jlast )
                    {
                        grow = xbnd;
                    }
                    j = j+jinc;
                }
            }
            else
            {
                
                /*
                 * A is unit triangular.
                 *
                 * Compute GROW = 1/G(j), where G(0) = max{x(i), i=1,...,n}.
                 */
                grow = ae_minreal((double)(1), 1/ae_maxreal(xbnd, smlnum, _state), _state);
                j = jfirst;
                while((jinc>0&&j<=jlast)||(jinc<0&&j>=jlast))
                {
                    
                    /*
                     * Exit the loop if the growth factor is too small.
                     */
                    if( ae_fp_less_eq(grow,smlnum) )
                    {
                        break;
                    }
                    
                    /*
                     * G(j) = G(j-1)*( 1 + CNORM(j) )
                     */
                    grow = grow*(1/(1+cnorm->ptr.p_double[j]));
                    j = j+jinc;
                }
            }
        }
    }
    else
    {
        
        /*
         * Compute the growth in A' * x = b.
         */
        if( upper )
        {
            jfirst = 1;
            jlast = n;
            jinc = 1;
        }
        else
        {
            jfirst = n;
            jlast = 1;
            jinc = -1;
        }
        if( ae_fp_neq(tscal,(double)(1)) )
        {
            grow = (double)(0);
        }
        else
        {
            if( nounit )
            {
                
                /*
                 * A is non-unit triangular.
                 *
                 * Compute GROW = 1/G(j) and XBND = 1/M(j).
                 * Initially, M(0) = max{x(i), i=1,...,n}.
                 */
                grow = 1/ae_maxreal(xbnd, smlnum, _state);
                xbnd = grow;
                j = jfirst;
                while((jinc>0&&j<=jlast)||(jinc<0&&j>=jlast))
                {
                    
                    /*
                     * Exit the loop if the growth factor is too small.
                     */
                    if( ae_fp_less_eq(grow,smlnum) )
                    {
                        break;
                    }
                    
                    /*
                     * G(j) = max( G(j-1), M(j-1)*( 1 + CNORM(j) ) )
                     */
                    xj = 1+cnorm->ptr.p_double[j];
                    grow = ae_minreal(grow, xbnd/xj, _state);
                    
                    /*
                     * M(j) = M(j-1)*( 1 + CNORM(j) ) / abs(A(j,j))
                     */
                    tjj = ae_fabs(a->ptr.pp_double[j][j], _state);
                    if( ae_fp_greater(xj,tjj) )
                    {
                        xbnd = xbnd*(tjj/xj);
                    }
                    if( j==jlast )
                    {
                        grow = ae_minreal(grow, xbnd, _state);
                    }
                    j = j+jinc;
                }
            }
            else
            {
                
                /*
                 * A is unit triangular.
                 *
                 * Compute GROW = 1/G(j), where G(0) = max{x(i), i=1,...,n}.
                 */
                grow = ae_minreal((double)(1), 1/ae_maxreal(xbnd, smlnum, _state), _state);
                j = jfirst;
                while((jinc>0&&j<=jlast)||(jinc<0&&j>=jlast))
                {
                    
                    /*
                     * Exit the loop if the growth factor is too small.
                     */
                    if( ae_fp_less_eq(grow,smlnum) )
                    {
                        break;
                    }
                    
                    /*
                     * G(j) = ( 1 + CNORM(j) )*G(j-1)
                     */
                    xj = 1+cnorm->ptr.p_double[j];
                    grow = grow/xj;
                    j = j+jinc;
                }
            }
        }
    }
    if( ae_fp_greater(grow*tscal,smlnum) )
    {
        
        /*
         * Use the Level 2 BLAS solve if the reciprocal of the bound on
         * elements of X is not too small.
         */
        if( (upper&&notran)||(!upper&&!notran) )
        {
            if( nounit )
            {
                vd = a->ptr.pp_double[n][n];
            }
            else
            {
                vd = (double)(1);
            }
            x->ptr.p_double[n] = x->ptr.p_double[n]/vd;
            for(i=n-1; i>=1; i--)
            {
                ip1 = i+1;
                if( upper )
                {
                    v = ae_v_dotproduct(&a->ptr.pp_double[i][ip1], 1, &x->ptr.p_double[ip1], 1, ae_v_len(ip1,n));
                }
                else
                {
                    v = ae_v_dotproduct(&a->ptr.pp_double[ip1][i], a->stride, &x->ptr.p_double[ip1], 1, ae_v_len(ip1,n));
                }
                if( nounit )
                {
                    vd = a->ptr.pp_double[i][i];
                }
                else
                {
                    vd = (double)(1);
                }
                x->ptr.p_double[i] = (x->ptr.p_double[i]-v)/vd;
            }
        }
        else
        {
            if( nounit )
            {
                vd = a->ptr.pp_double[1][1];
            }
            else
            {
                vd = (double)(1);
            }
            x->ptr.p_double[1] = x->ptr.p_double[1]/vd;
            for(i=2; i<=n; i++)
            {
                im1 = i-1;
                if( upper )
                {
                    v = ae_v_dotproduct(&a->ptr.pp_double[1][i], a->stride, &x->ptr.p_double[1], 1, ae_v_len(1,im1));
                }
                else
                {
                    v = ae_v_dotproduct(&a->ptr.pp_double[i][1], 1, &x->ptr.p_double[1], 1, ae_v_len(1,im1));
                }
                if( nounit )
                {
                    vd = a->ptr.pp_double[i][i];
                }
                else
                {
                    vd = (double)(1);
                }
                x->ptr.p_double[i] = (x->ptr.p_double[i]-v)/vd;
            }
        }
    }
    else
    {
        
        /*
         * Use a Level 1 BLAS solve, scaling intermediate results.
         */
        if( ae_fp_greater(xmax,bignum) )
        {
            
            /*
             * Scale X so that its components are less than or equal to
             * BIGNUM in absolute value.
             */
            *s = bignum/xmax;
            ae_v_muld(&x->ptr.p_double[1], 1, ae_v_len(1,n), *s);
            xmax = bignum;
        }
        if( notran )
        {
            
            /*
             * Solve A * x = b
             */
            j = jfirst;
            while((jinc>0&&j<=jlast)||(jinc<0&&j>=jlast))
            {
                
                /*
                 * Compute x(j) = b(j) / A(j,j), scaling x if necessary.
                 */
                xj = ae_fabs(x->ptr.p_double[j], _state);
                flg = 0;
                if( nounit )
                {
                    tjjs = a->ptr.pp_double[j][j]*tscal;
                }
                else
                {
                    tjjs = tscal;
                    if( ae_fp_eq(tscal,(double)(1)) )
                    {
                        flg = 100;
                    }
                }
                if( flg!=100 )
                {
                    tjj = ae_fabs(tjjs, _state);
                    if( ae_fp_greater(tjj,smlnum) )
                    {
                        
                        /*
                         * abs(A(j,j)) > SMLNUM:
                         */
                        if( ae_fp_less(tjj,(double)(1)) )
                        {
                            if( ae_fp_greater(xj,tjj*bignum) )
                            {
                                
                                /*
                                 * Scale x by 1/b(j).
                                 */
                                rec = 1/xj;
                                ae_v_muld(&x->ptr.p_double[1], 1, ae_v_len(1,n), rec);
                                *s = *s*rec;
                                xmax = xmax*rec;
                            }
                        }
                        x->ptr.p_double[j] = x->ptr.p_double[j]/tjjs;
                        xj = ae_fabs(x->ptr.p_double[j], _state);
                    }
                    else
                    {
                        if( ae_fp_greater(tjj,(double)(0)) )
                        {
                            
                            /*
                             * 0 < abs(A(j,j)) <= SMLNUM:
                             */
                            if( ae_fp_greater(xj,tjj*bignum) )
                            {
                                
                                /*
                                 * Scale x by (1/abs(x(j)))*abs(A(j,j))*BIGNUM
                                 * to avoid overflow when dividing by A(j,j).
                                 */
                                rec = tjj*bignum/xj;
                                if( ae_fp_greater(cnorm->ptr.p_double[j],(double)(1)) )
                                {
                                    
                                    /*
                                     * Scale by 1/CNORM(j) to avoid overflow when
                                     * multiplying x(j) times column j.
                                     */
                                    rec = rec/cnorm->ptr.p_double[j];
                                }
                                ae_v_muld(&x->ptr.p_double[1], 1, ae_v_len(1,n), rec);
                                *s = *s*rec;
                                xmax = xmax*rec;
                            }
                            x->ptr.p_double[j] = x->ptr.p_double[j]/tjjs;
                            xj = ae_fabs(x->ptr.p_double[j], _state);
                        }
                        else
                        {
                            
                            /*
                             * A(j,j) = 0:  Set x(1:n) = 0, x(j) = 1, and
                             * scale = 0, and compute a solution to A*x = 0.
                             */
                            for(i=1; i<=n; i++)
                            {
                                x->ptr.p_double[i] = (double)(0);
                            }
                            x->ptr.p_double[j] = (double)(1);
                            xj = (double)(1);
                            *s = (double)(0);
                            xmax = (double)(0);
                        }
                    }
                }
                
                /*
                 * Scale x if necessary to avoid overflow when adding a
                 * multiple of column j of A.
                 */
                if( ae_fp_greater(xj,(double)(1)) )
                {
                    rec = 1/xj;
                    if( ae_fp_greater(cnorm->ptr.p_double[j],(bignum-xmax)*rec) )
                    {
                        
                        /*
                         * Scale x by 1/(2*abs(x(j))).
                         */
                        rec = rec*0.5;
                        ae_v_muld(&x->ptr.p_double[1], 1, ae_v_len(1,n), rec);
                        *s = *s*rec;
                    }
                }
                else
                {
                    if( ae_fp_greater(xj*cnorm->ptr.p_double[j],bignum-xmax) )
                    {
                        
                        /*
                         * Scale x by 1/2.
                         */
                        ae_v_muld(&x->ptr.p_double[1], 1, ae_v_len(1,n), 0.5);
                        *s = *s*0.5;
                    }
                }
                if( upper )
                {
                    if( j>1 )
                    {
                        
                        /*
                         * Compute the update
                         * x(1:j-1) := x(1:j-1) - x(j) * A(1:j-1,j)
                         */
                        v = x->ptr.p_double[j]*tscal;
                        jm1 = j-1;
                        ae_v_subd(&x->ptr.p_double[1], 1, &a->ptr.pp_double[1][j], a->stride, ae_v_len(1,jm1), v);
                        i = 1;
                        for(k=2; k<=j-1; k++)
                        {
                            if( ae_fp_greater(ae_fabs(x->ptr.p_double[k], _state),ae_fabs(x->ptr.p_double[i], _state)) )
                            {
                                i = k;
                            }
                        }
                        xmax = ae_fabs(x->ptr.p_double[i], _state);
                    }
                }
                else
                {
                    if( j<n )
                    {
                        
                        /*
                         * Compute the update
                         * x(j+1:n) := x(j+1:n) - x(j) * A(j+1:n,j)
                         */
                        jp1 = j+1;
                        v = x->ptr.p_double[j]*tscal;
                        ae_v_subd(&x->ptr.p_double[jp1], 1, &a->ptr.pp_double[jp1][j], a->stride, ae_v_len(jp1,n), v);
                        i = j+1;
                        for(k=j+2; k<=n; k++)
                        {
                            if( ae_fp_greater(ae_fabs(x->ptr.p_double[k], _state),ae_fabs(x->ptr.p_double[i], _state)) )
                            {
                                i = k;
                            }
                        }
                        xmax = ae_fabs(x->ptr.p_double[i], _state);
                    }
                }
                j = j+jinc;
            }
        }
        else
        {
            
            /*
             * Solve A' * x = b
             */
            j = jfirst;
            while((jinc>0&&j<=jlast)||(jinc<0&&j>=jlast))
            {
                
                /*
                 * Compute x(j) = b(j) - sum A(k,j)*x(k).
                 *   k<>j
                 */
                xj = ae_fabs(x->ptr.p_double[j], _state);
                uscal = tscal;
                rec = 1/ae_maxreal(xmax, (double)(1), _state);
                if( ae_fp_greater(cnorm->ptr.p_double[j],(bignum-xj)*rec) )
                {
                    
                    /*
                     * If x(j) could overflow, scale x by 1/(2*XMAX).
                     */
                    rec = rec*0.5;
                    if( nounit )
                    {
                        tjjs = a->ptr.pp_double[j][j]*tscal;
                    }
                    else
                    {
                        tjjs = tscal;
                    }
                    tjj = ae_fabs(tjjs, _state);
                    if( ae_fp_greater(tjj,(double)(1)) )
                    {
                        
                        /*
                         * Divide by A(j,j) when scaling x if A(j,j) > 1.
                         */
                        rec = ae_minreal((double)(1), rec*tjj, _state);
                        uscal = uscal/tjjs;
                    }
                    if( ae_fp_less(rec,(double)(1)) )
                    {
                        ae_v_muld(&x->ptr.p_double[1], 1, ae_v_len(1,n), rec);
                        *s = *s*rec;
                        xmax = xmax*rec;
                    }
                }
                sumj = (double)(0);
                if( ae_fp_eq(uscal,(double)(1)) )
                {
                    
                    /*
                     * If the scaling needed for A in the dot product is 1,
                     * call DDOT to perform the dot product.
                     */
                    if( upper )
                    {
                        if( j>1 )
                        {
                            jm1 = j-1;
                            sumj = ae_v_dotproduct(&a->ptr.pp_double[1][j], a->stride, &x->ptr.p_double[1], 1, ae_v_len(1,jm1));
                        }
                        else
                        {
                            sumj = (double)(0);
                        }
                    }
                    else
                    {
                        if( j<n )
                        {
                            jp1 = j+1;
                            sumj = ae_v_dotproduct(&a->ptr.pp_double[jp1][j], a->stride, &x->ptr.p_double[jp1], 1, ae_v_len(jp1,n));
                        }
                    }
                }
                else
                {
                    
                    /*
                     * Otherwise, use in-line code for the dot product.
                     */
                    if( upper )
                    {
                        for(i=1; i<=j-1; i++)
                        {
                            v = a->ptr.pp_double[i][j]*uscal;
                            sumj = sumj+v*x->ptr.p_double[i];
                        }
                    }
                    else
                    {
                        if( j<n )
                        {
                            for(i=j+1; i<=n; i++)
                            {
                                v = a->ptr.pp_double[i][j]*uscal;
                                sumj = sumj+v*x->ptr.p_double[i];
                            }
                        }
                    }
                }
                if( ae_fp_eq(uscal,tscal) )
                {
                    
                    /*
                     * Compute x(j) := ( x(j) - sumj ) / A(j,j) if 1/A(j,j)
                     * was not used to scale the dotproduct.
                     */
                    x->ptr.p_double[j] = x->ptr.p_double[j]-sumj;
                    xj = ae_fabs(x->ptr.p_double[j], _state);
                    flg = 0;
                    if( nounit )
                    {
                        tjjs = a->ptr.pp_double[j][j]*tscal;
                    }
                    else
                    {
                        tjjs = tscal;
                        if( ae_fp_eq(tscal,(double)(1)) )
                        {
                            flg = 150;
                        }
                    }
                    
                    /*
                     * Compute x(j) = x(j) / A(j,j), scaling if necessary.
                     */
                    if( flg!=150 )
                    {
                        tjj = ae_fabs(tjjs, _state);
                        if( ae_fp_greater(tjj,smlnum) )
                        {
                            
                            /*
                             * abs(A(j,j)) > SMLNUM:
                             */
                            if( ae_fp_less(tjj,(double)(1)) )
                            {
                                if( ae_fp_greater(xj,tjj*bignum) )
                                {
                                    
                                    /*
                                     * Scale X by 1/abs(x(j)).
                                     */
                                    rec = 1/xj;
                                    ae_v_muld(&x->ptr.p_double[1], 1, ae_v_len(1,n), rec);
                                    *s = *s*rec;
                                    xmax = xmax*rec;
                                }
                            }
                            x->ptr.p_double[j] = x->ptr.p_double[j]/tjjs;
                        }
                        else
                        {
                            if( ae_fp_greater(tjj,(double)(0)) )
                            {
                                
                                /*
                                 * 0 < abs(A(j,j)) <= SMLNUM:
                                 */
                                if( ae_fp_greater(xj,tjj*bignum) )
                                {
                                    
                                    /*
                                     * Scale x by (1/abs(x(j)))*abs(A(j,j))*BIGNUM.
                                     */
                                    rec = tjj*bignum/xj;
                                    ae_v_muld(&x->ptr.p_double[1], 1, ae_v_len(1,n), rec);
                                    *s = *s*rec;
                                    xmax = xmax*rec;
                                }
                                x->ptr.p_double[j] = x->ptr.p_double[j]/tjjs;
                            }
                            else
                            {
                                
                                /*
                                 * A(j,j) = 0:  Set x(1:n) = 0, x(j) = 1, and
                                 * scale = 0, and compute a solution to A'*x = 0.
                                 */
                                for(i=1; i<=n; i++)
                                {
                                    x->ptr.p_double[i] = (double)(0);
                                }
                                x->ptr.p_double[j] = (double)(1);
                                *s = (double)(0);
                                xmax = (double)(0);
                            }
                        }
                    }
                }
                else
                {
                    
                    /*
                     * Compute x(j) := x(j) / A(j,j)  - sumj if the dot
                     * product has already been divided by 1/A(j,j).
                     */
                    x->ptr.p_double[j] = x->ptr.p_double[j]/tjjs-sumj;
                }
                xmax = ae_maxreal(xmax, ae_fabs(x->ptr.p_double[j], _state), _state);
                j = j+jinc;
            }
        }
        *s = *s/tscal;
    }
    
    /*
     * Scale the column norms by 1/TSCAL for return.
     */
    if( ae_fp_neq(tscal,(double)(1)) )
    {
        v = 1/tscal;
        ae_v_muld(&cnorm->ptr.p_double[1], 1, ae_v_len(1,n), v);
    }
}


/*$ End $*/
