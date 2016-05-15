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


#include <stdafx.h>
#include <stdio.h>
#include "testmatgenunit.h"


/*$ Declarations $*/
static ae_int_t testmatgenunit_maxsvditerations = 60;
static void testmatgenunit_unset2d(/* Real    */ ae_matrix* a,
     ae_state *_state);
static void testmatgenunit_unset2dc(/* Complex */ ae_matrix* a,
     ae_state *_state);
static ae_bool testmatgenunit_isspd(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_state *_state);
static ae_bool testmatgenunit_ishpd(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state);
static ae_bool testmatgenunit_testeult(ae_state *_state);
static double testmatgenunit_svdcond(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state);
static ae_bool testmatgenunit_obsoletesvddecomposition(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* v,
     ae_state *_state);
static double testmatgenunit_extsign(double a, double b, ae_state *_state);
static double testmatgenunit_mymax(double a, double b, ae_state *_state);
static double testmatgenunit_pythag(double a, double b, ae_state *_state);


/*$ Body $*/


ae_bool testmatgen(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix b;
    ae_matrix u;
    ae_matrix v;
    ae_matrix ca;
    ae_matrix cb;
    ae_matrix r1;
    ae_matrix r2;
    ae_matrix c1;
    ae_matrix c2;
    ae_vector w;
    ae_int_t n;
    ae_int_t maxn;
    ae_int_t i;
    ae_int_t j;
    ae_int_t pass;
    ae_int_t passcount;
    ae_bool waserrors;
    double cond;
    double threshold;
    double vt;
    ae_complex ct;
    double minw;
    double maxw;
    ae_bool serr;
    ae_bool herr;
    ae_bool spderr;
    ae_bool hpderr;
    ae_bool rerr;
    ae_bool cerr;
    ae_bool eulerr;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&b, 0, 0, DT_REAL, _state);
    ae_matrix_init(&u, 0, 0, DT_REAL, _state);
    ae_matrix_init(&v, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ca, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&cb, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&r1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&r2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c1, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&c2, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);

    rerr = ae_false;
    cerr = ae_false;
    serr = ae_false;
    herr = ae_false;
    spderr = ae_false;
    hpderr = ae_false;
    eulerr = ae_false;
    waserrors = ae_false;
    maxn = 20;
    passcount = 15;
    threshold = 1000*ae_machineepsilon;
    
    /*
     * Testing orthogonal
     */
    for(n=1; n<=maxn; n++)
    {
        for(pass=1; pass<=passcount; pass++)
        {
            ae_matrix_set_length(&r1, n-1+1, 2*n-1+1, _state);
            ae_matrix_set_length(&r2, 2*n-1+1, n-1+1, _state);
            ae_matrix_set_length(&c1, n-1+1, 2*n-1+1, _state);
            ae_matrix_set_length(&c2, 2*n-1+1, n-1+1, _state);
            
            /*
             * Random orthogonal, real
             */
            testmatgenunit_unset2d(&a, _state);
            testmatgenunit_unset2d(&b, _state);
            rmatrixrndorthogonal(n, &a, _state);
            rmatrixrndorthogonal(n, &b, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    
                    /*
                     * orthogonality test
                     */
                    vt = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &a.ptr.pp_double[j][0], 1, ae_v_len(0,n-1));
                    if( i==j )
                    {
                        rerr = rerr||ae_fp_greater(ae_fabs(vt-1, _state),threshold);
                    }
                    else
                    {
                        rerr = rerr||ae_fp_greater(ae_fabs(vt, _state),threshold);
                    }
                    vt = ae_v_dotproduct(&b.ptr.pp_double[i][0], 1, &b.ptr.pp_double[j][0], 1, ae_v_len(0,n-1));
                    if( i==j )
                    {
                        rerr = rerr||ae_fp_greater(ae_fabs(vt-1, _state),threshold);
                    }
                    else
                    {
                        rerr = rerr||ae_fp_greater(ae_fabs(vt, _state),threshold);
                    }
                    
                    /*
                     * test for difference in A and B
                     */
                    if( n>=2 )
                    {
                        rerr = rerr||ae_fp_eq(a.ptr.pp_double[i][j],b.ptr.pp_double[i][j]);
                    }
                }
            }
            
            /*
             * Random orthogonal, complex
             */
            testmatgenunit_unset2dc(&ca, _state);
            testmatgenunit_unset2dc(&cb, _state);
            cmatrixrndorthogonal(n, &ca, _state);
            cmatrixrndorthogonal(n, &cb, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    
                    /*
                     * orthogonality test
                     */
                    ct = ae_v_cdotproduct(&ca.ptr.pp_complex[i][0], 1, "N", &ca.ptr.pp_complex[j][0], 1, "Conj", ae_v_len(0,n-1));
                    if( i==j )
                    {
                        cerr = cerr||ae_fp_greater(ae_c_abs(ae_c_sub_d(ct,1), _state),threshold);
                    }
                    else
                    {
                        cerr = cerr||ae_fp_greater(ae_c_abs(ct, _state),threshold);
                    }
                    ct = ae_v_cdotproduct(&cb.ptr.pp_complex[i][0], 1, "N", &cb.ptr.pp_complex[j][0], 1, "Conj", ae_v_len(0,n-1));
                    if( i==j )
                    {
                        cerr = cerr||ae_fp_greater(ae_c_abs(ae_c_sub_d(ct,1), _state),threshold);
                    }
                    else
                    {
                        cerr = cerr||ae_fp_greater(ae_c_abs(ct, _state),threshold);
                    }
                    
                    /*
                     * test for difference in A and B
                     */
                    if( n>=2 )
                    {
                        cerr = cerr||ae_c_eq(ca.ptr.pp_complex[i][j],cb.ptr.pp_complex[i][j]);
                    }
                }
            }
            
            /*
             * From the right real tests:
             * 1. E*Q is orthogonal
             * 2. Q1<>Q2 (routine result is changing)
             * 3. (E E)'*Q = (Q' Q')' (correct handling of non-square matrices)
             */
            testmatgenunit_unset2d(&a, _state);
            testmatgenunit_unset2d(&b, _state);
            ae_matrix_set_length(&a, n-1+1, n-1+1, _state);
            ae_matrix_set_length(&b, n-1+1, n-1+1, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = (double)(0);
                    b.ptr.pp_double[i][j] = (double)(0);
                }
                a.ptr.pp_double[i][i] = (double)(1);
                b.ptr.pp_double[i][i] = (double)(1);
            }
            rmatrixrndorthogonalfromtheright(&a, n, n, _state);
            rmatrixrndorthogonalfromtheright(&b, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    
                    /*
                     * orthogonality test
                     */
                    vt = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &a.ptr.pp_double[j][0], 1, ae_v_len(0,n-1));
                    if( i==j )
                    {
                        rerr = rerr||ae_fp_greater(ae_fabs(vt-1, _state),threshold);
                    }
                    else
                    {
                        rerr = rerr||ae_fp_greater(ae_fabs(vt, _state),threshold);
                    }
                    vt = ae_v_dotproduct(&b.ptr.pp_double[i][0], 1, &b.ptr.pp_double[j][0], 1, ae_v_len(0,n-1));
                    if( i==j )
                    {
                        rerr = rerr||ae_fp_greater(ae_fabs(vt-1, _state),threshold);
                    }
                    else
                    {
                        rerr = rerr||ae_fp_greater(ae_fabs(vt, _state),threshold);
                    }
                    
                    /*
                     * test for difference in A and B
                     */
                    if( n>=2 )
                    {
                        rerr = rerr||ae_fp_eq(a.ptr.pp_double[i][j],b.ptr.pp_double[i][j]);
                    }
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    r2.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                    r2.ptr.pp_double[i+n][j] = r2.ptr.pp_double[i][j];
                }
            }
            rmatrixrndorthogonalfromtheright(&r2, 2*n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    rerr = rerr||ae_fp_greater(ae_fabs(r2.ptr.pp_double[i+n][j]-r2.ptr.pp_double[i][j], _state),threshold);
                }
            }
            
            /*
             * From the left real tests:
             * 1. Q*E is orthogonal
             * 2. Q1<>Q2 (routine result is changing)
             * 3. Q*(E E) = (Q Q) (correct handling of non-square matrices)
             */
            testmatgenunit_unset2d(&a, _state);
            testmatgenunit_unset2d(&b, _state);
            ae_matrix_set_length(&a, n-1+1, n-1+1, _state);
            ae_matrix_set_length(&b, n-1+1, n-1+1, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = (double)(0);
                    b.ptr.pp_double[i][j] = (double)(0);
                }
                a.ptr.pp_double[i][i] = (double)(1);
                b.ptr.pp_double[i][i] = (double)(1);
            }
            rmatrixrndorthogonalfromtheleft(&a, n, n, _state);
            rmatrixrndorthogonalfromtheleft(&b, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    
                    /*
                     * orthogonality test
                     */
                    vt = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &a.ptr.pp_double[j][0], 1, ae_v_len(0,n-1));
                    if( i==j )
                    {
                        rerr = rerr||ae_fp_greater(ae_fabs(vt-1, _state),threshold);
                    }
                    else
                    {
                        rerr = rerr||ae_fp_greater(ae_fabs(vt, _state),threshold);
                    }
                    vt = ae_v_dotproduct(&b.ptr.pp_double[i][0], 1, &b.ptr.pp_double[j][0], 1, ae_v_len(0,n-1));
                    if( i==j )
                    {
                        rerr = rerr||ae_fp_greater(ae_fabs(vt-1, _state),threshold);
                    }
                    else
                    {
                        rerr = rerr||ae_fp_greater(ae_fabs(vt, _state),threshold);
                    }
                    
                    /*
                     * test for difference in A and B
                     */
                    if( n>=2 )
                    {
                        rerr = rerr||ae_fp_eq(a.ptr.pp_double[i][j],b.ptr.pp_double[i][j]);
                    }
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    r1.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                    r1.ptr.pp_double[i][j+n] = r1.ptr.pp_double[i][j];
                }
            }
            rmatrixrndorthogonalfromtheleft(&r1, n, 2*n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    rerr = rerr||ae_fp_greater(ae_fabs(r1.ptr.pp_double[i][j]-r1.ptr.pp_double[i][j+n], _state),threshold);
                }
            }
            
            /*
             * From the right complex tests:
             * 1. E*Q is orthogonal
             * 2. Q1<>Q2 (routine result is changing)
             * 3. (E E)'*Q = (Q' Q')' (correct handling of non-square matrices)
             */
            testmatgenunit_unset2dc(&ca, _state);
            testmatgenunit_unset2dc(&cb, _state);
            ae_matrix_set_length(&ca, n-1+1, n-1+1, _state);
            ae_matrix_set_length(&cb, n-1+1, n-1+1, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    ca.ptr.pp_complex[i][j] = ae_complex_from_i(0);
                    cb.ptr.pp_complex[i][j] = ae_complex_from_i(0);
                }
                ca.ptr.pp_complex[i][i] = ae_complex_from_i(1);
                cb.ptr.pp_complex[i][i] = ae_complex_from_i(1);
            }
            cmatrixrndorthogonalfromtheright(&ca, n, n, _state);
            cmatrixrndorthogonalfromtheright(&cb, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    
                    /*
                     * orthogonality test
                     */
                    ct = ae_v_cdotproduct(&ca.ptr.pp_complex[i][0], 1, "N", &ca.ptr.pp_complex[j][0], 1, "Conj", ae_v_len(0,n-1));
                    if( i==j )
                    {
                        cerr = cerr||ae_fp_greater(ae_c_abs(ae_c_sub_d(ct,1), _state),threshold);
                    }
                    else
                    {
                        cerr = cerr||ae_fp_greater(ae_c_abs(ct, _state),threshold);
                    }
                    ct = ae_v_cdotproduct(&cb.ptr.pp_complex[i][0], 1, "N", &cb.ptr.pp_complex[j][0], 1, "Conj", ae_v_len(0,n-1));
                    if( i==j )
                    {
                        cerr = cerr||ae_fp_greater(ae_c_abs(ae_c_sub_d(ct,1), _state),threshold);
                    }
                    else
                    {
                        cerr = cerr||ae_fp_greater(ae_c_abs(ct, _state),threshold);
                    }
                    
                    /*
                     * test for difference in A and B
                     */
                    cerr = cerr||ae_c_eq(ca.ptr.pp_complex[i][j],cb.ptr.pp_complex[i][j]);
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    c2.ptr.pp_complex[i][j] = ae_complex_from_d(2*ae_randomreal(_state)-1);
                    c2.ptr.pp_complex[i+n][j] = c2.ptr.pp_complex[i][j];
                }
            }
            cmatrixrndorthogonalfromtheright(&c2, 2*n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    cerr = cerr||ae_fp_greater(ae_c_abs(ae_c_sub(c2.ptr.pp_complex[i+n][j],c2.ptr.pp_complex[i][j]), _state),threshold);
                }
            }
            
            /*
             * From the left complex tests:
             * 1. Q*E is orthogonal
             * 2. Q1<>Q2 (routine result is changing)
             * 3. Q*(E E) = (Q Q) (correct handling of non-square matrices)
             */
            testmatgenunit_unset2dc(&ca, _state);
            testmatgenunit_unset2dc(&cb, _state);
            ae_matrix_set_length(&ca, n-1+1, n-1+1, _state);
            ae_matrix_set_length(&cb, n-1+1, n-1+1, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    ca.ptr.pp_complex[i][j] = ae_complex_from_i(0);
                    cb.ptr.pp_complex[i][j] = ae_complex_from_i(0);
                }
                ca.ptr.pp_complex[i][i] = ae_complex_from_i(1);
                cb.ptr.pp_complex[i][i] = ae_complex_from_i(1);
            }
            cmatrixrndorthogonalfromtheleft(&ca, n, n, _state);
            cmatrixrndorthogonalfromtheleft(&cb, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    
                    /*
                     * orthogonality test
                     */
                    ct = ae_v_cdotproduct(&ca.ptr.pp_complex[i][0], 1, "N", &ca.ptr.pp_complex[j][0], 1, "Conj", ae_v_len(0,n-1));
                    if( i==j )
                    {
                        cerr = cerr||ae_fp_greater(ae_c_abs(ae_c_sub_d(ct,1), _state),threshold);
                    }
                    else
                    {
                        cerr = cerr||ae_fp_greater(ae_c_abs(ct, _state),threshold);
                    }
                    ct = ae_v_cdotproduct(&cb.ptr.pp_complex[i][0], 1, "N", &cb.ptr.pp_complex[j][0], 1, "Conj", ae_v_len(0,n-1));
                    if( i==j )
                    {
                        cerr = cerr||ae_fp_greater(ae_c_abs(ae_c_sub_d(ct,1), _state),threshold);
                    }
                    else
                    {
                        cerr = cerr||ae_fp_greater(ae_c_abs(ct, _state),threshold);
                    }
                    
                    /*
                     * test for difference in A and B
                     */
                    cerr = cerr||ae_c_eq(ca.ptr.pp_complex[i][j],cb.ptr.pp_complex[i][j]);
                }
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    c1.ptr.pp_complex[i][j] = ae_complex_from_d(2*ae_randomreal(_state)-1);
                    c1.ptr.pp_complex[i][j+n] = c1.ptr.pp_complex[i][j];
                }
            }
            cmatrixrndorthogonalfromtheleft(&c1, n, 2*n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    cerr = cerr||ae_fp_greater(ae_c_abs(ae_c_sub(c1.ptr.pp_complex[i][j],c1.ptr.pp_complex[i][j+n]), _state),threshold);
                }
            }
        }
    }
    
    /*
     * Testing GCond
     */
    for(n=2; n<=maxn; n++)
    {
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             * real test
             */
            testmatgenunit_unset2d(&a, _state);
            cond = ae_exp(ae_log((double)(1000), _state)*ae_randomreal(_state), _state);
            rmatrixrndcond(n, cond, &a, _state);
            ae_matrix_set_length(&b, n+1, n+1, _state);
            for(i=1; i<=n; i++)
            {
                for(j=1; j<=n; j++)
                {
                    b.ptr.pp_double[i][j] = a.ptr.pp_double[i-1][j-1];
                }
            }
            if( testmatgenunit_obsoletesvddecomposition(&b, n, n, &w, &v, _state) )
            {
                maxw = w.ptr.p_double[1];
                minw = w.ptr.p_double[1];
                for(i=2; i<=n; i++)
                {
                    if( ae_fp_greater(w.ptr.p_double[i],maxw) )
                    {
                        maxw = w.ptr.p_double[i];
                    }
                    if( ae_fp_less(w.ptr.p_double[i],minw) )
                    {
                        minw = w.ptr.p_double[i];
                    }
                }
                vt = maxw/minw/cond;
                if( ae_fp_greater(ae_fabs(ae_log(vt, _state), _state),ae_log(1+threshold, _state)) )
                {
                    rerr = ae_true;
                }
            }
        }
    }
    
    /*
     * Symmetric/SPD
     * N = 2 .. 30
     */
    for(n=2; n<=maxn; n++)
    {
        
        /*
         * SPD matrices
         */
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             * Generate A
             */
            testmatgenunit_unset2d(&a, _state);
            cond = ae_exp(ae_log((double)(1000), _state)*ae_randomreal(_state), _state);
            spdmatrixrndcond(n, cond, &a, _state);
            
            /*
             * test condition number
             */
            spderr = spderr||ae_fp_greater(testmatgenunit_svdcond(&a, n, _state)/cond-1,threshold);
            
            /*
             * test SPD
             */
            spderr = spderr||!testmatgenunit_isspd(&a, n, ae_true, _state);
            
            /*
             * test that A is symmetic
             */
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    spderr = spderr||ae_fp_greater(ae_fabs(a.ptr.pp_double[i][j]-a.ptr.pp_double[j][i], _state),threshold);
                }
            }
            
            /*
             * test for difference between A and B (subsequent matrix)
             */
            testmatgenunit_unset2d(&b, _state);
            spdmatrixrndcond(n, cond, &b, _state);
            if( n>=2 )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        spderr = spderr||ae_fp_eq(a.ptr.pp_double[i][j],b.ptr.pp_double[i][j]);
                    }
                }
            }
        }
        
        /*
         * HPD matrices
         */
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             * Generate A
             */
            testmatgenunit_unset2dc(&ca, _state);
            cond = ae_exp(ae_log((double)(1000), _state)*ae_randomreal(_state), _state);
            hpdmatrixrndcond(n, cond, &ca, _state);
            
            /*
             * test HPD
             */
            hpderr = hpderr||!testmatgenunit_ishpd(&ca, n, _state);
            
            /*
             * test that A is Hermitian
             */
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    hpderr = hpderr||ae_fp_greater(ae_c_abs(ae_c_sub(ca.ptr.pp_complex[i][j],ae_c_conj(ca.ptr.pp_complex[j][i], _state)), _state),threshold);
                }
            }
            
            /*
             * test for difference between A and B (subsequent matrix)
             */
            testmatgenunit_unset2dc(&cb, _state);
            hpdmatrixrndcond(n, cond, &cb, _state);
            if( n>=2 )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        hpderr = hpderr||ae_c_eq(ca.ptr.pp_complex[i][j],cb.ptr.pp_complex[i][j]);
                    }
                }
            }
        }
        
        /*
         * Symmetric matrices
         */
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             * test condition number
             */
            testmatgenunit_unset2d(&a, _state);
            cond = ae_exp(ae_log((double)(1000), _state)*ae_randomreal(_state), _state);
            smatrixrndcond(n, cond, &a, _state);
            serr = serr||ae_fp_greater(testmatgenunit_svdcond(&a, n, _state)/cond-1,threshold);
            
            /*
             * test for difference between A and B
             */
            testmatgenunit_unset2d(&b, _state);
            smatrixrndcond(n, cond, &b, _state);
            if( n>=2 )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        serr = serr||ae_fp_eq(a.ptr.pp_double[i][j],b.ptr.pp_double[i][j]);
                    }
                }
            }
        }
        
        /*
         * Hermitian matrices
         */
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             * Generate A
             */
            testmatgenunit_unset2dc(&ca, _state);
            cond = ae_exp(ae_log((double)(1000), _state)*ae_randomreal(_state), _state);
            hmatrixrndcond(n, cond, &ca, _state);
            
            /*
             * test that A is Hermitian
             */
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    herr = herr||ae_fp_greater(ae_c_abs(ae_c_sub(ca.ptr.pp_complex[i][j],ae_c_conj(ca.ptr.pp_complex[j][i], _state)), _state),threshold);
                }
            }
            
            /*
             * test for difference between A and B (subsequent matrix)
             */
            testmatgenunit_unset2dc(&cb, _state);
            hmatrixrndcond(n, cond, &cb, _state);
            if( n>=2 )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        herr = herr||ae_c_eq(ca.ptr.pp_complex[i][j],cb.ptr.pp_complex[i][j]);
                    }
                }
            }
        }
    }
    
    /*
     * Test for symmetric matrices
     */
    eulerr = testmatgenunit_testeult(_state);
    
    /*
     * report
     */
    waserrors = (((((rerr||cerr)||serr)||spderr)||herr)||hpderr)||eulerr;
    if( !silent )
    {
        printf("TESTING MATRIX GENERATOR\n");
        printf("REAL TEST:                               ");
        if( !rerr )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("COMPLEX TEST:                            ");
        if( !cerr )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("SYMMETRIC TEST:                          ");
        if( !serr )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("HERMITIAN TEST:                          ");
        if( !herr )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("SPD TEST:                                ");
        if( !spderr )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("HPD TEST:                                ");
        if( !hpderr )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("TEST FOR SYMMETRIC MATRICES:             ");
        if( !eulerr )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        if( waserrors )
        {
            printf("TEST FAILED\n");
        }
        else
        {
            printf("TEST PASSED\n");
        }
        printf("\n\n");
    }
    result = !waserrors;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testmatgen(ae_bool silent, ae_state *_state)
{
    return testmatgen(silent, _state);
}


/*************************************************************************
Unsets 2D array.
*************************************************************************/
static void testmatgenunit_unset2d(/* Real    */ ae_matrix* a,
     ae_state *_state)
{


    ae_matrix_set_length(a, 0+1, 0+1, _state);
    a->ptr.pp_double[0][0] = 2*ae_randomreal(_state)-1;
}


/*************************************************************************
Unsets 2D array.
*************************************************************************/
static void testmatgenunit_unset2dc(/* Complex */ ae_matrix* a,
     ae_state *_state)
{


    ae_matrix_set_length(a, 0+1, 0+1, _state);
    a->ptr.pp_complex[0][0] = ae_complex_from_d(2*ae_randomreal(_state)-1);
}


/*************************************************************************
Test whether matrix is SPD
*************************************************************************/
static ae_bool testmatgenunit_isspd(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_int_t i;
    ae_int_t j;
    double ajj;
    double v;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;

    
    /*
     *     Test the input parameters.
     */
    ae_assert(n>=0, "Error in SMatrixCholesky: incorrect function arguments", _state);
    
    /*
     *     Quick return if possible
     */
    result = ae_true;
    if( n<=0 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( isupper )
    {
        
        /*
         * Compute the Cholesky factorization A = U'*U.
         */
        for(j=0; j<=n-1; j++)
        {
            
            /*
             * Compute U(J,J) and test for non-positive-definiteness.
             */
            v = ae_v_dotproduct(&a->ptr.pp_double[0][j], a->stride, &a->ptr.pp_double[0][j], a->stride, ae_v_len(0,j-1));
            ajj = a->ptr.pp_double[j][j]-v;
            if( ae_fp_less_eq(ajj,(double)(0)) )
            {
                result = ae_false;
                ae_frame_leave(_state);
                return result;
            }
            ajj = ae_sqrt(ajj, _state);
            a->ptr.pp_double[j][j] = ajj;
            
            /*
             * Compute elements J+1:N of row J.
             */
            if( j<n-1 )
            {
                for(i=j+1; i<=n-1; i++)
                {
                    v = ae_v_dotproduct(&a->ptr.pp_double[0][i], a->stride, &a->ptr.pp_double[0][j], a->stride, ae_v_len(0,j-1));
                    a->ptr.pp_double[j][i] = a->ptr.pp_double[j][i]-v;
                }
                v = 1/ajj;
                ae_v_muld(&a->ptr.pp_double[j][j+1], 1, ae_v_len(j+1,n-1), v);
            }
        }
    }
    else
    {
        
        /*
         * Compute the Cholesky factorization A = L*L'.
         */
        for(j=0; j<=n-1; j++)
        {
            
            /*
             * Compute L(J,J) and test for non-positive-definiteness.
             */
            v = ae_v_dotproduct(&a->ptr.pp_double[j][0], 1, &a->ptr.pp_double[j][0], 1, ae_v_len(0,j-1));
            ajj = a->ptr.pp_double[j][j]-v;
            if( ae_fp_less_eq(ajj,(double)(0)) )
            {
                result = ae_false;
                ae_frame_leave(_state);
                return result;
            }
            ajj = ae_sqrt(ajj, _state);
            a->ptr.pp_double[j][j] = ajj;
            
            /*
             * Compute elements J+1:N of column J.
             */
            if( j<n-1 )
            {
                for(i=j+1; i<=n-1; i++)
                {
                    v = ae_v_dotproduct(&a->ptr.pp_double[i][0], 1, &a->ptr.pp_double[j][0], 1, ae_v_len(0,j-1));
                    a->ptr.pp_double[i][j] = a->ptr.pp_double[i][j]-v;
                }
                v = 1/ajj;
                ae_v_muld(&a->ptr.pp_double[j+1][j], a->stride, ae_v_len(j+1,n-1), v);
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Tests whether A is HPD
*************************************************************************/
static ae_bool testmatgenunit_ishpd(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix _a;
    ae_int_t j;
    double ajj;
    ae_complex v;
    double r;
    ae_vector t;
    ae_vector t2;
    ae_vector t3;
    ae_int_t i;
    ae_matrix a1;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_init(&t, 0, DT_COMPLEX, _state);
    ae_vector_init(&t2, 0, DT_COMPLEX, _state);
    ae_vector_init(&t3, 0, DT_COMPLEX, _state);
    ae_matrix_init(&a1, 0, 0, DT_COMPLEX, _state);

    ae_vector_set_length(&t, n-1+1, _state);
    ae_vector_set_length(&t2, n-1+1, _state);
    ae_vector_set_length(&t3, n-1+1, _state);
    result = ae_true;
    
    /*
     * Compute the Cholesky factorization A = U'*U.
     */
    for(j=0; j<=n-1; j++)
    {
        
        /*
         * Compute U(J,J) and test for non-positive-definiteness.
         */
        v = ae_v_cdotproduct(&a->ptr.pp_complex[0][j], a->stride, "Conj", &a->ptr.pp_complex[0][j], a->stride, "N", ae_v_len(0,j-1));
        ajj = ae_c_sub(a->ptr.pp_complex[j][j],v).x;
        if( ae_fp_less_eq(ajj,(double)(0)) )
        {
            a->ptr.pp_complex[j][j] = ae_complex_from_d(ajj);
            result = ae_false;
            ae_frame_leave(_state);
            return result;
        }
        ajj = ae_sqrt(ajj, _state);
        a->ptr.pp_complex[j][j] = ae_complex_from_d(ajj);
        
        /*
         * Compute elements J+1:N-1 of row J.
         */
        if( j<n-1 )
        {
            ae_v_cmove(&t2.ptr.p_complex[0], 1, &a->ptr.pp_complex[0][j], a->stride, "Conj", ae_v_len(0,j-1));
            ae_v_cmove(&t3.ptr.p_complex[j+1], 1, &a->ptr.pp_complex[j][j+1], 1, "N", ae_v_len(j+1,n-1));
            for(i=j+1; i<=n-1; i++)
            {
                v = ae_v_cdotproduct(&a->ptr.pp_complex[0][i], a->stride, "N", &t2.ptr.p_complex[0], 1, "N", ae_v_len(0,j-1));
                t3.ptr.p_complex[i] = ae_c_sub(t3.ptr.p_complex[i],v);
            }
            ae_v_cmove(&a->ptr.pp_complex[j][j+1], 1, &t3.ptr.p_complex[j+1], 1, "N", ae_v_len(j+1,n-1));
            r = 1/ajj;
            ae_v_cmuld(&a->ptr.pp_complex[j][j+1], 1, ae_v_len(j+1,n-1), r);
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
The function check, that upper triangle from symmetric matrix is equal to
lower triangle.
*************************************************************************/
static ae_bool testmatgenunit_testeult(ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix b;
    double c;
    double range;
    double eps;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&b, 0, 0, DT_COMPLEX, _state);

    eps = 2*ae_machineepsilon;
    range = 100*(2*ae_randomreal(_state)-1);
    for(n=1; n<=15; n++)
    {
        c = 900*ae_randomreal(_state)+100;
        
        /*
         * Generate symmetric matrix and check it
         */
        smatrixrndcond(n, c, &a, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( ae_fp_greater(ae_fabs(a.ptr.pp_double[i][j]-a.ptr.pp_double[j][i], _state),eps) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
        spdmatrixrndcond(n, c, &a, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( ae_fp_greater(ae_fabs(a.ptr.pp_double[i][j]-a.ptr.pp_double[j][i], _state),eps) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
        hmatrixrndcond(n, c, &b, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( ae_fp_greater(ae_fabs(b.ptr.pp_complex[i][j].x-b.ptr.pp_complex[j][i].x, _state),eps)||ae_fp_greater(ae_fabs(b.ptr.pp_complex[i][j].y+b.ptr.pp_complex[j][i].y, _state),eps) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
        hpdmatrixrndcond(n, c, &b, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( ae_fp_greater(ae_fabs(b.ptr.pp_complex[i][j].x-b.ptr.pp_complex[j][i].x, _state),eps)||ae_fp_greater(ae_fabs(b.ptr.pp_complex[i][j].y+b.ptr.pp_complex[j][i].y, _state),eps) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
        
        /*
         * Prepare symmetric matrix with real values
         */
        for(i=0; i<=n-1; i++)
        {
            for(j=i; j<=n-1; j++)
            {
                a.ptr.pp_double[i][j] = range*(2*ae_randomreal(_state)-1);
            }
        }
        for(i=0; i<=n-2; i++)
        {
            for(j=i+1; j<=n-1; j++)
            {
                a.ptr.pp_double[j][i] = a.ptr.pp_double[i][j];
            }
        }
        smatrixrndmultiply(&a, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( ae_fp_greater(ae_fabs(a.ptr.pp_double[i][j]-a.ptr.pp_double[j][i], _state),eps) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
        
        /*
         * Prepare symmetric matrix with complex values
         */
        for(i=0; i<=n-1; i++)
        {
            for(j=i; j<=n-1; j++)
            {
                b.ptr.pp_complex[i][j].x = range*(2*ae_randomreal(_state)-1);
                if( i!=j )
                {
                    b.ptr.pp_complex[i][j].y = range*(2*ae_randomreal(_state)-1);
                }
                else
                {
                    b.ptr.pp_complex[i][j].y = (double)(0);
                }
            }
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=i+1; j<=n-1; j++)
            {
                b.ptr.pp_complex[i][j].x = b.ptr.pp_complex[j][i].x;
                b.ptr.pp_complex[i][j].y = -b.ptr.pp_complex[j][i].y;
            }
        }
        hmatrixrndmultiply(&b, n, _state);
        for(i=0; i<=n-1; i++)
        {
            b.ptr.pp_complex[i][i].y = (double)(0);
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( ae_fp_greater(ae_fabs(b.ptr.pp_complex[i][j].x-b.ptr.pp_complex[j][i].x, _state),eps)||ae_fp_greater(ae_fabs(b.ptr.pp_complex[i][j].y+b.ptr.pp_complex[j][i].y, _state),eps) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
SVD condition number
*************************************************************************/
static double testmatgenunit_svdcond(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a1;
    ae_matrix v;
    ae_vector w;
    ae_int_t i;
    ae_int_t j;
    double minw;
    double maxw;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&v, 0, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);

    ae_matrix_set_length(&a1, n+1, n+1, _state);
    for(i=1; i<=n; i++)
    {
        for(j=1; j<=n; j++)
        {
            a1.ptr.pp_double[i][j] = a->ptr.pp_double[i-1][j-1];
        }
    }
    if( !testmatgenunit_obsoletesvddecomposition(&a1, n, n, &w, &v, _state) )
    {
        result = (double)(0);
        ae_frame_leave(_state);
        return result;
    }
    minw = w.ptr.p_double[1];
    maxw = w.ptr.p_double[1];
    for(i=2; i<=n; i++)
    {
        if( ae_fp_less(w.ptr.p_double[i],minw) )
        {
            minw = w.ptr.p_double[i];
        }
        if( ae_fp_greater(w.ptr.p_double[i],maxw) )
        {
            maxw = w.ptr.p_double[i];
        }
    }
    result = maxw/minw;
    ae_frame_leave(_state);
    return result;
}


static ae_bool testmatgenunit_obsoletesvddecomposition(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* v,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t nm;
    ae_int_t minmn;
    ae_int_t l;
    ae_int_t k;
    ae_int_t j;
    ae_int_t jj;
    ae_int_t its;
    ae_int_t i;
    double z;
    double y;
    double x;
    double vscale;
    double s;
    double h;
    double g;
    double f;
    double c;
    double anorm;
    ae_vector rv1;
    ae_bool flag;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(w);
    ae_matrix_clear(v);
    ae_vector_init(&rv1, 0, DT_REAL, _state);

    ae_vector_set_length(&rv1, n+1, _state);
    ae_vector_set_length(w, n+1, _state);
    ae_matrix_set_length(v, n+1, n+1, _state);
    result = ae_true;
    if( m<n )
    {
        minmn = m;
    }
    else
    {
        minmn = n;
    }
    g = 0.0;
    vscale = 0.0;
    anorm = 0.0;
    l = n;
    for(i=1; i<=n; i++)
    {
        l = i+1;
        rv1.ptr.p_double[i] = vscale*g;
        g = (double)(0);
        s = (double)(0);
        vscale = (double)(0);
        if( i<=m )
        {
            for(k=i; k<=m; k++)
            {
                vscale = vscale+ae_fabs(a->ptr.pp_double[k][i], _state);
            }
            if( ae_fp_neq(vscale,0.0) )
            {
                for(k=i; k<=m; k++)
                {
                    a->ptr.pp_double[k][i] = a->ptr.pp_double[k][i]/vscale;
                    s = s+a->ptr.pp_double[k][i]*a->ptr.pp_double[k][i];
                }
                f = a->ptr.pp_double[i][i];
                g = -testmatgenunit_extsign(ae_sqrt(s, _state), f, _state);
                h = f*g-s;
                a->ptr.pp_double[i][i] = f-g;
                if( i!=n )
                {
                    for(j=l; j<=n; j++)
                    {
                        s = 0.0;
                        for(k=i; k<=m; k++)
                        {
                            s = s+a->ptr.pp_double[k][i]*a->ptr.pp_double[k][j];
                        }
                        f = s/h;
                        for(k=i; k<=m; k++)
                        {
                            a->ptr.pp_double[k][j] = a->ptr.pp_double[k][j]+f*a->ptr.pp_double[k][i];
                        }
                    }
                }
                for(k=i; k<=m; k++)
                {
                    a->ptr.pp_double[k][i] = vscale*a->ptr.pp_double[k][i];
                }
            }
        }
        w->ptr.p_double[i] = vscale*g;
        g = 0.0;
        s = 0.0;
        vscale = 0.0;
        if( i<=m&&i!=n )
        {
            for(k=l; k<=n; k++)
            {
                vscale = vscale+ae_fabs(a->ptr.pp_double[i][k], _state);
            }
            if( ae_fp_neq(vscale,0.0) )
            {
                for(k=l; k<=n; k++)
                {
                    a->ptr.pp_double[i][k] = a->ptr.pp_double[i][k]/vscale;
                    s = s+a->ptr.pp_double[i][k]*a->ptr.pp_double[i][k];
                }
                f = a->ptr.pp_double[i][l];
                g = -testmatgenunit_extsign(ae_sqrt(s, _state), f, _state);
                h = f*g-s;
                a->ptr.pp_double[i][l] = f-g;
                for(k=l; k<=n; k++)
                {
                    rv1.ptr.p_double[k] = a->ptr.pp_double[i][k]/h;
                }
                if( i!=m )
                {
                    for(j=l; j<=m; j++)
                    {
                        s = 0.0;
                        for(k=l; k<=n; k++)
                        {
                            s = s+a->ptr.pp_double[j][k]*a->ptr.pp_double[i][k];
                        }
                        for(k=l; k<=n; k++)
                        {
                            a->ptr.pp_double[j][k] = a->ptr.pp_double[j][k]+s*rv1.ptr.p_double[k];
                        }
                    }
                }
                for(k=l; k<=n; k++)
                {
                    a->ptr.pp_double[i][k] = vscale*a->ptr.pp_double[i][k];
                }
            }
        }
        anorm = testmatgenunit_mymax(anorm, ae_fabs(w->ptr.p_double[i], _state)+ae_fabs(rv1.ptr.p_double[i], _state), _state);
    }
    for(i=n; i>=1; i--)
    {
        if( i<n )
        {
            if( ae_fp_neq(g,0.0) )
            {
                for(j=l; j<=n; j++)
                {
                    v->ptr.pp_double[j][i] = a->ptr.pp_double[i][j]/a->ptr.pp_double[i][l]/g;
                }
                for(j=l; j<=n; j++)
                {
                    s = 0.0;
                    for(k=l; k<=n; k++)
                    {
                        s = s+a->ptr.pp_double[i][k]*v->ptr.pp_double[k][j];
                    }
                    for(k=l; k<=n; k++)
                    {
                        v->ptr.pp_double[k][j] = v->ptr.pp_double[k][j]+s*v->ptr.pp_double[k][i];
                    }
                }
            }
            for(j=l; j<=n; j++)
            {
                v->ptr.pp_double[i][j] = 0.0;
                v->ptr.pp_double[j][i] = 0.0;
            }
        }
        v->ptr.pp_double[i][i] = 1.0;
        g = rv1.ptr.p_double[i];
        l = i;
    }
    for(i=minmn; i>=1; i--)
    {
        l = i+1;
        g = w->ptr.p_double[i];
        if( i<n )
        {
            for(j=l; j<=n; j++)
            {
                a->ptr.pp_double[i][j] = 0.0;
            }
        }
        if( ae_fp_neq(g,0.0) )
        {
            g = 1.0/g;
            if( i!=n )
            {
                for(j=l; j<=n; j++)
                {
                    s = 0.0;
                    for(k=l; k<=m; k++)
                    {
                        s = s+a->ptr.pp_double[k][i]*a->ptr.pp_double[k][j];
                    }
                    f = s/a->ptr.pp_double[i][i]*g;
                    for(k=i; k<=m; k++)
                    {
                        a->ptr.pp_double[k][j] = a->ptr.pp_double[k][j]+f*a->ptr.pp_double[k][i];
                    }
                }
            }
            for(j=i; j<=m; j++)
            {
                a->ptr.pp_double[j][i] = a->ptr.pp_double[j][i]*g;
            }
        }
        else
        {
            for(j=i; j<=m; j++)
            {
                a->ptr.pp_double[j][i] = 0.0;
            }
        }
        a->ptr.pp_double[i][i] = a->ptr.pp_double[i][i]+1.0;
    }
    nm = 0;
    for(k=n; k>=1; k--)
    {
        for(its=1; its<=testmatgenunit_maxsvditerations; its++)
        {
            flag = ae_true;
            for(l=k; l>=1; l--)
            {
                nm = l-1;
                if( ae_fp_eq(ae_fabs(rv1.ptr.p_double[l], _state)+anorm,anorm) )
                {
                    flag = ae_false;
                    break;
                }
                if( ae_fp_eq(ae_fabs(w->ptr.p_double[nm], _state)+anorm,anorm) )
                {
                    break;
                }
            }
            if( flag )
            {
                c = 0.0;
                s = 1.0;
                for(i=l; i<=k; i++)
                {
                    f = s*rv1.ptr.p_double[i];
                    if( ae_fp_neq(ae_fabs(f, _state)+anorm,anorm) )
                    {
                        g = w->ptr.p_double[i];
                        h = testmatgenunit_pythag(f, g, _state);
                        w->ptr.p_double[i] = h;
                        h = 1.0/h;
                        c = g*h;
                        s = -f*h;
                        for(j=1; j<=m; j++)
                        {
                            y = a->ptr.pp_double[j][nm];
                            z = a->ptr.pp_double[j][i];
                            a->ptr.pp_double[j][nm] = y*c+z*s;
                            a->ptr.pp_double[j][i] = -y*s+z*c;
                        }
                    }
                }
            }
            z = w->ptr.p_double[k];
            if( l==k )
            {
                if( ae_fp_less(z,0.0) )
                {
                    w->ptr.p_double[k] = -z;
                    for(j=1; j<=n; j++)
                    {
                        v->ptr.pp_double[j][k] = -v->ptr.pp_double[j][k];
                    }
                }
                break;
            }
            if( its==testmatgenunit_maxsvditerations )
            {
                result = ae_false;
                ae_frame_leave(_state);
                return result;
            }
            x = w->ptr.p_double[l];
            nm = k-1;
            y = w->ptr.p_double[nm];
            g = rv1.ptr.p_double[nm];
            h = rv1.ptr.p_double[k];
            f = ((y-z)*(y+z)+(g-h)*(g+h))/(2.0*h*y);
            g = testmatgenunit_pythag(f, (double)(1), _state);
            f = ((x-z)*(x+z)+h*(y/(f+testmatgenunit_extsign(g, f, _state))-h))/x;
            c = 1.0;
            s = 1.0;
            for(j=l; j<=nm; j++)
            {
                i = j+1;
                g = rv1.ptr.p_double[i];
                y = w->ptr.p_double[i];
                h = s*g;
                g = c*g;
                z = testmatgenunit_pythag(f, h, _state);
                rv1.ptr.p_double[j] = z;
                c = f/z;
                s = h/z;
                f = x*c+g*s;
                g = -x*s+g*c;
                h = y*s;
                y = y*c;
                for(jj=1; jj<=n; jj++)
                {
                    x = v->ptr.pp_double[jj][j];
                    z = v->ptr.pp_double[jj][i];
                    v->ptr.pp_double[jj][j] = x*c+z*s;
                    v->ptr.pp_double[jj][i] = -x*s+z*c;
                }
                z = testmatgenunit_pythag(f, h, _state);
                w->ptr.p_double[j] = z;
                if( ae_fp_neq(z,0.0) )
                {
                    z = 1.0/z;
                    c = f*z;
                    s = h*z;
                }
                f = c*g+s*y;
                x = -s*g+c*y;
                for(jj=1; jj<=m; jj++)
                {
                    y = a->ptr.pp_double[jj][j];
                    z = a->ptr.pp_double[jj][i];
                    a->ptr.pp_double[jj][j] = y*c+z*s;
                    a->ptr.pp_double[jj][i] = -y*s+z*c;
                }
            }
            rv1.ptr.p_double[l] = 0.0;
            rv1.ptr.p_double[k] = f;
            w->ptr.p_double[k] = x;
        }
    }
    ae_frame_leave(_state);
    return result;
}


static double testmatgenunit_extsign(double a, double b, ae_state *_state)
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


static double testmatgenunit_mymax(double a, double b, ae_state *_state)
{
    double result;


    if( ae_fp_greater(a,b) )
    {
        result = a;
    }
    else
    {
        result = b;
    }
    return result;
}


static double testmatgenunit_pythag(double a, double b, ae_state *_state)
{
    double result;


    if( ae_fp_less(ae_fabs(a, _state),ae_fabs(b, _state)) )
    {
        result = ae_fabs(b, _state)*ae_sqrt(1+ae_sqr(a/b, _state), _state);
    }
    else
    {
        result = ae_fabs(a, _state)*ae_sqrt(1+ae_sqr(b/a, _state), _state);
    }
    return result;
}


/*$ End $*/
