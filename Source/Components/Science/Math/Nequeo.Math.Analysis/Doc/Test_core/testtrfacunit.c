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
#include "testtrfacunit.h"


/*$ Declarations $*/
static void testtrfacunit_testcluproblem(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double threshold,
     ae_bool* err,
     ae_bool* properr,
     ae_state *_state);
static void testtrfacunit_testrluproblem(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double threshold,
     ae_bool* err,
     ae_bool* properr,
     ae_state *_state);
static void testtrfacunit_testdensecholeskyupdates(ae_bool* spdupderrorflag,
     ae_state *_state);


/*$ Body $*/


ae_bool testtrfac(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix ra;
    ae_matrix ral;
    ae_matrix rau;
    ae_matrix ca;
    ae_matrix cal;
    ae_matrix cau;
    ae_int_t m;
    ae_int_t n;
    ae_int_t mx;
    ae_int_t maxmn;
    ae_int_t largemn;
    ae_int_t i;
    ae_int_t j;
    ae_complex vc;
    double vr;
    ae_bool waserrors;
    ae_bool dspderr;
    ae_bool sspderr;
    ae_bool hpderr;
    ae_bool rerr;
    ae_bool cerr;
    ae_bool properr;
    ae_bool dspdupderr;
    double threshold;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&ra, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ral, 0, 0, DT_REAL, _state);
    ae_matrix_init(&rau, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ca, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&cal, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&cau, 0, 0, DT_COMPLEX, _state);

    rerr = ae_false;
    dspderr = ae_false;
    sspderr = ae_false;
    cerr = ae_false;
    hpderr = ae_false;
    properr = ae_false;
    dspdupderr = ae_false;
    waserrors = ae_false;
    maxmn = 4*ablasblocksize(&ra, _state)+1;
    largemn = 256;
    threshold = 1000*ae_machineepsilon*maxmn;
    
    /*
     * Sparse Cholesky
     */
    sspderr = sparserealcholeskytest(_state);
    
    /*
     * Cholesky updates
     */
    testtrfacunit_testdensecholeskyupdates(&dspdupderr, _state);
    
    /*
     * test LU:
     * * first, test on small-scale matrices
     * * then, perform several large-scale tests
     */
    for(mx=1; mx<=maxmn; mx++)
    {
        
        /*
         * Initialize N/M, both are <=MX,
         * at least one of them is exactly equal to MX
         */
        n = 1+ae_randominteger(mx, _state);
        m = 1+ae_randominteger(mx, _state);
        if( ae_fp_greater(ae_randomreal(_state),0.5) )
        {
            n = mx;
        }
        else
        {
            m = mx;
        }
        
        /*
         * First, test on zero matrix
         */
        ae_matrix_set_length(&ra, m, n, _state);
        ae_matrix_set_length(&ca, m, n, _state);
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                ra.ptr.pp_double[i][j] = (double)(0);
                ca.ptr.pp_complex[i][j] = ae_complex_from_i(0);
            }
        }
        testtrfacunit_testcluproblem(&ca, m, n, threshold, &cerr, &properr, _state);
        testtrfacunit_testrluproblem(&ra, m, n, threshold, &rerr, &properr, _state);
        
        /*
         * Second, random matrix with moderate condition number
         */
        ae_matrix_set_length(&ra, m, n, _state);
        ae_matrix_set_length(&ca, m, n, _state);
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                ra.ptr.pp_double[i][j] = (double)(0);
                ca.ptr.pp_complex[i][j] = ae_complex_from_i(0);
            }
        }
        for(i=0; i<=ae_minint(m, n, _state)-1; i++)
        {
            ra.ptr.pp_double[i][i] = 1+10*ae_randomreal(_state);
            ca.ptr.pp_complex[i][i] = ae_complex_from_d(1+10*ae_randomreal(_state));
        }
        cmatrixrndorthogonalfromtheleft(&ca, m, n, _state);
        cmatrixrndorthogonalfromtheright(&ca, m, n, _state);
        rmatrixrndorthogonalfromtheleft(&ra, m, n, _state);
        rmatrixrndorthogonalfromtheright(&ra, m, n, _state);
        testtrfacunit_testcluproblem(&ca, m, n, threshold, &cerr, &properr, _state);
        testtrfacunit_testrluproblem(&ra, m, n, threshold, &rerr, &properr, _state);
    }
    for(m=largemn-1; m<=largemn+1; m++)
    {
        for(n=largemn-1; n<=largemn+1; n++)
        {
            
            /*
             * Random matrix with moderate condition number
             */
            ae_matrix_set_length(&ra, m, n, _state);
            ae_matrix_set_length(&ca, m, n, _state);
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    ra.ptr.pp_double[i][j] = (double)(0);
                    ca.ptr.pp_complex[i][j] = ae_complex_from_i(0);
                }
            }
            for(i=0; i<=ae_minint(m, n, _state)-1; i++)
            {
                ra.ptr.pp_double[i][i] = 1+10*ae_randomreal(_state);
                ca.ptr.pp_complex[i][i] = ae_complex_from_d(1+10*ae_randomreal(_state));
            }
            cmatrixrndorthogonalfromtheleft(&ca, m, n, _state);
            cmatrixrndorthogonalfromtheright(&ca, m, n, _state);
            rmatrixrndorthogonalfromtheleft(&ra, m, n, _state);
            rmatrixrndorthogonalfromtheright(&ra, m, n, _state);
            testtrfacunit_testcluproblem(&ca, m, n, threshold, &cerr, &properr, _state);
            testtrfacunit_testrluproblem(&ra, m, n, threshold, &rerr, &properr, _state);
        }
    }
    
    /*
     * Test Cholesky
     */
    for(n=1; n<=maxmn; n++)
    {
        
        /*
         * Load CA (HPD matrix with low condition number),
         *      CAL and CAU - its lower and upper triangles
         */
        hpdmatrixrndcond(n, 1+50*ae_randomreal(_state), &ca, _state);
        ae_matrix_set_length(&cal, n, n, _state);
        ae_matrix_set_length(&cau, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                cal.ptr.pp_complex[i][j] = ae_complex_from_i(i);
                cau.ptr.pp_complex[i][j] = ae_complex_from_i(j);
            }
        }
        for(i=0; i<=n-1; i++)
        {
            ae_v_cmove(&cal.ptr.pp_complex[i][0], 1, &ca.ptr.pp_complex[i][0], 1, "N", ae_v_len(0,i));
            ae_v_cmove(&cau.ptr.pp_complex[i][i], 1, &ca.ptr.pp_complex[i][i], 1, "N", ae_v_len(i,n-1));
        }
        
        /*
         * Test HPDMatrixCholesky:
         * 1. it must leave upper (lower) part unchanged
         * 2. max(A-L*L^H) must be small
         */
        if( hpdmatrixcholesky(&cal, n, ae_false, _state) )
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( j>i )
                    {
                        hpderr = hpderr||ae_c_neq_d(cal.ptr.pp_complex[i][j],(double)(i));
                    }
                    else
                    {
                        vc = ae_v_cdotproduct(&cal.ptr.pp_complex[i][0], 1, "N", &cal.ptr.pp_complex[j][0], 1, "Conj", ae_v_len(0,j));
                        hpderr = hpderr||ae_fp_greater(ae_c_abs(ae_c_sub(ca.ptr.pp_complex[i][j],vc), _state),threshold);
                    }
                }
            }
        }
        else
        {
            hpderr = ae_true;
        }
        if( hpdmatrixcholesky(&cau, n, ae_true, _state) )
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( j<i )
                    {
                        hpderr = hpderr||ae_c_neq_d(cau.ptr.pp_complex[i][j],(double)(j));
                    }
                    else
                    {
                        vc = ae_v_cdotproduct(&cau.ptr.pp_complex[0][i], cau.stride, "Conj", &cau.ptr.pp_complex[0][j], cau.stride, "N", ae_v_len(0,i));
                        hpderr = hpderr||ae_fp_greater(ae_c_abs(ae_c_sub(ca.ptr.pp_complex[i][j],vc), _state),threshold);
                    }
                }
            }
        }
        else
        {
            hpderr = ae_true;
        }
        
        /*
         * Load RA (SPD matrix with low condition number),
         *      RAL and RAU - its lower and upper triangles
         *
         * Test SPDMatrixCholesky:
         * 1. it must leave upper (lower) part unchanged
         * 2. max(A-L*L^H) must be small
         *
         * After testing SPDMatrixCholesky() we compare results
         * returned by SparseCholeskyX() against ones returned
         * by SPDMatrixCholesky().
         */
        spdmatrixrndcond(n, 1+50*ae_randomreal(_state), &ra, _state);
        ae_matrix_set_length(&ral, n, n, _state);
        ae_matrix_set_length(&rau, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                ral.ptr.pp_double[i][j] = (double)(i);
                rau.ptr.pp_double[i][j] = (double)(j);
            }
        }
        for(i=0; i<=n-1; i++)
        {
            ae_v_move(&ral.ptr.pp_double[i][0], 1, &ra.ptr.pp_double[i][0], 1, ae_v_len(0,i));
            ae_v_move(&rau.ptr.pp_double[i][i], 1, &ra.ptr.pp_double[i][i], 1, ae_v_len(i,n-1));
        }
        if( spdmatrixcholesky(&ral, n, ae_false, _state) )
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( j>i )
                    {
                        dspderr = dspderr||ae_fp_neq(ral.ptr.pp_double[i][j],(double)(i));
                    }
                    else
                    {
                        vr = ae_v_dotproduct(&ral.ptr.pp_double[i][0], 1, &ral.ptr.pp_double[j][0], 1, ae_v_len(0,j));
                        dspderr = dspderr||ae_fp_greater(ae_fabs(ra.ptr.pp_double[i][j]-vr, _state),threshold);
                    }
                }
            }
        }
        else
        {
            dspderr = ae_true;
        }
        if( spdmatrixcholesky(&rau, n, ae_true, _state) )
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( j<i )
                    {
                        dspderr = dspderr||ae_fp_neq(rau.ptr.pp_double[i][j],(double)(j));
                    }
                    else
                    {
                        vr = ae_v_dotproduct(&rau.ptr.pp_double[0][i], rau.stride, &rau.ptr.pp_double[0][j], rau.stride, ae_v_len(0,i));
                        dspderr = dspderr||ae_fp_greater(ae_fabs(ra.ptr.pp_double[i][j]-vr, _state),threshold);
                    }
                }
            }
        }
        else
        {
            dspderr = ae_true;
        }
        
        /*
         * Check algorithms on negative definite matrices -
         * correct error code must be returned.
         */
        ae_matrix_set_length(&ra, n, n, _state);
        ae_matrix_set_length(&ca, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                ra.ptr.pp_double[i][j] = 0.0;
                ca.ptr.pp_complex[i][j] = ae_complex_from_d(0.0);
            }
            ra.ptr.pp_double[i][i] = 1.0;
            ca.ptr.pp_complex[i][i] = ae_complex_from_d(1.0);
        }
        ra.ptr.pp_double[n/2][n/2] = -1.0;
        ca.ptr.pp_complex[n/2][n/2] = ae_complex_from_d(-1.0);
        seterrorflag(&dspderr, spdmatrixcholesky(&ra, n, ae_fp_greater(ae_randomreal(_state),0.5), _state), _state);
        seterrorflag(&hpderr, hpdmatrixcholesky(&ca, n, ae_fp_greater(ae_randomreal(_state),0.5), _state), _state);
    }
    
    /*
     * report
     */
    waserrors = (((((rerr||dspderr)||sspderr)||cerr)||hpderr)||properr)||dspdupderr;
    if( !silent )
    {
        printf("TESTING TRIANGULAR FACTORIZATIONS\n");
        printf("* REAL:                                  ");
        if( rerr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* SPD (dense)                            ");
        if( dspderr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* SPD (sparse)                           ");
        if( sspderr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* COMPLEX:                               ");
        if( cerr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* HPD:                                   ");
        if( hpderr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* OTHER PROPERTIES:                      ");
        if( properr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("TESTING UPDATED FACTORIZATIONS\n");
        printf("* SPD (dense)                            ");
        if( dspdupderr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
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
ae_bool _pexec_testtrfac(ae_bool silent, ae_state *_state)
{
    return testtrfac(silent, _state);
}


/*************************************************************************
Function for testing sparse real Cholesky.
Returns True on errors, False on success.

  -- ALGLIB PROJECT --
     Copyright 16.01.1014 by Bochkanov Sergey
*************************************************************************/
ae_bool sparserealcholeskytest(ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t nz;
    double pnz;
    ae_matrix a;
    ae_matrix a1;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    ae_int_t t0;
    ae_int_t t1;
    ae_bool isupper;
    double offscale;
    double tol;
    sparsematrix sa;
    sparsematrix sa1;
    sparsematrix sc;
    sparsebuffers sbuf;
    ae_vector p0;
    ae_vector p1;
    ae_vector b1;
    ae_int_t cfmt;
    ae_int_t cord;
    hqrndstate rs;
    ae_int_t maxfmt;
    ae_int_t maxord;
    ae_int_t minord;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a1, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&sa, _state);
    _sparsematrix_init(&sa1, _state);
    _sparsematrix_init(&sc, _state);
    _sparsebuffers_init(&sbuf, _state);
    ae_vector_init(&p0, 0, DT_INT, _state);
    ae_vector_init(&p1, 0, DT_INT, _state);
    ae_vector_init(&b1, 0, DT_BOOL, _state);
    _hqrndstate_init(&rs, _state);

    result = ae_false;
    hqrndrandomize(&rs, _state);
    
    /*
     * Settings
     */
    maxfmt = 2;
    maxord = 0;
    minord = -2;
    offscale = 1.0E-3;
    tol = 1.0E-8;
    
    /*
     * SparseCholeskyX test: performed for matrices
     * of all sizes in 1..20 and all sparcity percentages.
     */
    for(n=1; n<=20; n++)
    {
        nz = n*n-n;
        for(;;)
        {
            
            /*
             * Generate symmetric N*N matrix where probability of non-diagonal element
             * being non-zero is PNZ. Off-diagonal elements are set to very
             * small values, so positive definiteness is guaranteed.
             */
            if( n>1 )
            {
                pnz = (double)nz/(double)(n*n-n);
            }
            else
            {
                pnz = 1.0;
            }
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=i; j++)
                {
                    if( i==j )
                    {
                        a.ptr.pp_double[i][i] = 1+hqrnduniformr(&rs, _state);
                        continue;
                    }
                    if( ae_fp_less_eq(hqrnduniformr(&rs, _state),pnz) )
                    {
                        a.ptr.pp_double[i][j] = offscale*(hqrnduniformr(&rs, _state)-0.5);
                        a.ptr.pp_double[j][i] = a.ptr.pp_double[i][j];
                    }
                    else
                    {
                        a.ptr.pp_double[i][j] = 0.0;
                        a.ptr.pp_double[j][i] = 0.0;
                    }
                }
            }
            
            /*
             * Problem statement
             */
            isupper = ae_fp_greater(ae_randomreal(_state),0.5);
            cfmt = ae_randominteger(maxfmt+1, _state);
            cord = ae_randominteger(maxord+1-minord, _state)+minord;
            
            /*
             * Create matrix is hash-based storage format, convert it to random storage format.
             */
            sparsecreate(n, n, 0, &sa, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( (j<=i&&!isupper)||(j>=i&&isupper) )
                    {
                        sparseset(&sa, i, j, a.ptr.pp_double[i][j], _state);
                    }
                }
            }
            sparseconvertto(&sa, hqrnduniformi(&rs, maxfmt+1, _state), _state);
            
            /*
             * Perform sparse Cholesky and make several tests:
             * * correctness of P0 and P1 (they are correct permutations and one is inverse of another)
             * * format of SC matches CFmt
             * * SC has correct size (exactly N*N)
             * * check that correct triangle is returned
             */
            if( !sparsecholeskyx(&sa, n, isupper, &p0, &p1, cord, ae_randominteger(3, _state), cfmt, &sbuf, &sc, _state) )
            {
                seterrorflag(&result, ae_true, _state);
                ae_frame_leave(_state);
                return result;
            }
            seterrorflag(&result, p0.cnt<n, _state);
            seterrorflag(&result, p1.cnt<n, _state);
            if( result )
            {
                ae_frame_leave(_state);
                return result;
            }
            ae_vector_set_length(&b1, n, _state);
            for(i=0; i<=n-1; i++)
            {
                b1.ptr.p_bool[i] = ae_false;
            }
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(&result, p0.ptr.p_int[i]<0, _state);
                seterrorflag(&result, p1.ptr.p_int[i]<0, _state);
                seterrorflag(&result, p0.ptr.p_int[i]>=n, _state);
                seterrorflag(&result, p1.ptr.p_int[i]>=n, _state);
                if( result )
                {
                    ae_frame_leave(_state);
                    return result;
                }
                seterrorflag(&result, b1.ptr.p_bool[p0.ptr.p_int[i]], _state);
                b1.ptr.p_bool[p0.ptr.p_int[i]] = ae_true;
                seterrorflag(&result, p1.ptr.p_int[p0.ptr.p_int[i]]!=i, _state);
            }
            seterrorflag(&result, sparsegetmatrixtype(&sc, _state)!=cfmt, _state);
            seterrorflag(&result, sparsegetncols(&sc, _state)!=n, _state);
            seterrorflag(&result, sparsegetnrows(&sc, _state)!=n, _state);
            t0 = 0;
            t1 = 0;
            while(sparseenumerate(&sc, &t0, &t1, &i, &j, &v, _state))
            {
                seterrorflag(&result, j<i&&isupper, _state);
                seterrorflag(&result, j>i&&!isupper, _state);
            }
            
            /*
             * Now, test correctness of Cholesky decomposition itself.
             * We calculate U'*U (or L*L') and check at against permutation
             * of A given by P0.
             *
             * NOTE: we expect that only one triangle of SC is filled,
             *       and another one is exactly zero.
             */
            if( isupper )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        v = 0.0;
                        for(k=0; k<=n-1; k++)
                        {
                            v = v+sparseget(&sc, k, j, _state)*sparseget(&sc, k, i, _state);
                        }
                        seterrorflag(&result, ae_fp_greater(ae_fabs(a.ptr.pp_double[p0.ptr.p_int[i]][p0.ptr.p_int[j]]-v, _state),tol), _state);
                    }
                }
            }
            else
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        v = 0.0;
                        for(k=0; k<=n-1; k++)
                        {
                            v = v+sparseget(&sc, j, k, _state)*sparseget(&sc, i, k, _state);
                        }
                        seterrorflag(&result, ae_fp_greater(ae_fabs(a.ptr.pp_double[p0.ptr.p_int[i]][p0.ptr.p_int[j]]-v, _state),tol), _state);
                    }
                }
            }
            
            /*
             * Increase problem sparcity and try one more time. 
             * Stop after testing NZ=0.
             */
            if( nz==0 )
            {
                break;
            }
            nz = nz/2;
        }
    }
    
    /*
     * SparseCholeskySkyline test: performed for matrices
     * of all sizes in 1..20 and all sparcity percentages.
     */
    for(n=1; n<=20; n++)
    {
        nz = n*n-n;
        for(;;)
        {
            
            /*
             * Choose IsUpper - main triangle to work with.
             *
             * Generate A - symmetric N*N matrix where probability of non-diagonal
             * element being non-zero is PNZ. Off-diagonal elements are set to
             * very small values, so positive definiteness is guaranteed. Full matrix
             * is generated.
             *
             * Additionally, we create A1 - same as A, but one of the triangles is
             * asymmetrically spoiled. If IsUpper is True, we spoil lower one, or vice versa.
             */
            isupper = ae_fp_greater(ae_randomreal(_state),0.5);
            if( n>1 )
            {
                pnz = (double)nz/(double)(n*n-n);
            }
            else
            {
                pnz = 1.0;
            }
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=i; j++)
                {
                    if( i==j )
                    {
                        a.ptr.pp_double[i][i] = 1+hqrnduniformr(&rs, _state);
                        continue;
                    }
                    if( ae_fp_less_eq(hqrnduniformr(&rs, _state),pnz) )
                    {
                        a.ptr.pp_double[i][j] = offscale*(hqrnduniformr(&rs, _state)-0.5);
                        a.ptr.pp_double[j][i] = a.ptr.pp_double[i][j];
                    }
                    else
                    {
                        a.ptr.pp_double[i][j] = 0.0;
                        a.ptr.pp_double[j][i] = 0.0;
                    }
                }
            }
            ae_matrix_set_length(&a1, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( (j<=i&&!isupper)||(j>=i&&isupper) )
                    {
                        
                        /*
                         * Copy one triangle
                         */
                        a1.ptr.pp_double[i][j] = a.ptr.pp_double[i][j];
                    }
                    else
                    {
                        
                        /*
                         * Form another sparse pattern in different triangle.
                         */
                        if( ae_fp_less_eq(hqrnduniformr(&rs, _state),pnz) )
                        {
                            a1.ptr.pp_double[i][j] = offscale*(hqrnduniformr(&rs, _state)-0.5);
                        }
                        else
                        {
                            a1.ptr.pp_double[i][j] = 0.0;
                        }
                    }
                }
            }
            
            /*
             * Create copies of A and A1 in hash-based storage format.
             * Only one triangle of A is copied, but A1 is copied fully.
             * Convert them to SKS
             */
            sparsecreate(n, n, 0, &sa, _state);
            sparsecreate(n, n, 0, &sa1, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( (j<=i&&!isupper)||(j>=i&&isupper) )
                    {
                        sparseset(&sa, i, j, a.ptr.pp_double[i][j], _state);
                    }
                    sparseset(&sa1, i, j, a1.ptr.pp_double[i][j], _state);
                }
            }
            sparseconverttosks(&sa, _state);
            sparseconverttosks(&sa1, _state);
            
            /*
             * Call SparseCholeskySkyline() for SA and make several tests:
             * * check that it is still SKS
             * * check that it has correct size (exactly N*N)
             * * check that correct triangle is returned (and another one is unchanged - zero)
             * * check that it is correct Cholesky decomposition.
             *   We calculate U'*U (or L*L') and check at against A. We expect
             *   that only one triangle of SA is filled, and another one is
             *   exactly zero.
             */
            if( !sparsecholeskyskyline(&sa, n, isupper, _state) )
            {
                seterrorflag(&result, ae_true, _state);
                ae_frame_leave(_state);
                return result;
            }
            seterrorflag(&result, !sparseissks(&sa, _state), _state);
            seterrorflag(&result, sparsegetncols(&sa, _state)!=n, _state);
            seterrorflag(&result, sparsegetnrows(&sa, _state)!=n, _state);
            t0 = 0;
            t1 = 0;
            while(sparseenumerate(&sa, &t0, &t1, &i, &j, &v, _state))
            {
                seterrorflag(&result, j<i&&isupper, _state);
                seterrorflag(&result, j>i&&!isupper, _state);
            }
            if( isupper )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        v = 0.0;
                        for(k=0; k<=n-1; k++)
                        {
                            v = v+sparseget(&sa, k, j, _state)*sparseget(&sa, k, i, _state);
                        }
                        seterrorflag(&result, ae_fp_greater(ae_fabs(a.ptr.pp_double[i][j]-v, _state),tol), _state);
                    }
                }
            }
            else
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        v = 0.0;
                        for(k=0; k<=n-1; k++)
                        {
                            v = v+sparseget(&sa, j, k, _state)*sparseget(&sa, i, k, _state);
                        }
                        seterrorflag(&result, ae_fp_greater(ae_fabs(a.ptr.pp_double[i][j]-v, _state),tol), _state);
                    }
                }
            }
            
            /*
             * Call SparseCholeskySkyline() for SA1 and make several tests:
             * * check that it is still SKS
             * * check that it has correct size (exactly N*N)
             * * check that factorized triangle matches contents of SA,
             *   and another triangle was unchanged (matches contents of A1).
             */
            if( !sparsecholeskyskyline(&sa1, n, isupper, _state) )
            {
                seterrorflag(&result, ae_true, _state);
                ae_frame_leave(_state);
                return result;
            }
            seterrorflag(&result, !sparseissks(&sa1, _state), _state);
            seterrorflag(&result, sparsegetncols(&sa1, _state)!=n, _state);
            seterrorflag(&result, sparsegetnrows(&sa1, _state)!=n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( (j<=i&&!isupper)||(j>=i&&isupper) )
                    {
                        seterrorflag(&result, ae_fp_greater(ae_fabs(sparseget(&sa1, i, j, _state)-sparseget(&sa, i, j, _state), _state),10*ae_machineepsilon), _state);
                    }
                    else
                    {
                        seterrorflag(&result, ae_fp_greater(ae_fabs(sparseget(&sa1, i, j, _state)-a1.ptr.pp_double[i][j], _state),10*ae_machineepsilon), _state);
                    }
                }
            }
            
            /*
             * Increase problem sparcity and try one more time. 
             * Stop after testing NZ=0.
             */
            if( nz==0 )
            {
                break;
            }
            nz = nz/2;
        }
    }
    ae_frame_leave(_state);
    return result;
}


static void testtrfacunit_testcluproblem(/* Complex */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double threshold,
     ae_bool* err,
     ae_bool* properr,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix ca;
    ae_matrix cl;
    ae_matrix cu;
    ae_matrix ca2;
    ae_vector ct;
    ae_int_t i;
    ae_int_t j;
    ae_int_t minmn;
    ae_complex v;
    ae_vector p;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&ca, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&cl, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&cu, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&ca2, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&ct, 0, DT_COMPLEX, _state);
    ae_vector_init(&p, 0, DT_INT, _state);

    minmn = ae_minint(m, n, _state);
    
    /*
     * PLU test
     */
    ae_matrix_set_length(&ca, m, n, _state);
    for(i=0; i<=m-1; i++)
    {
        ae_v_cmove(&ca.ptr.pp_complex[i][0], 1, &a->ptr.pp_complex[i][0], 1, "N", ae_v_len(0,n-1));
    }
    cmatrixplu(&ca, m, n, &p, _state);
    for(i=0; i<=minmn-1; i++)
    {
        if( p.ptr.p_int[i]<i||p.ptr.p_int[i]>=m )
        {
            *properr = ae_true;
            ae_frame_leave(_state);
            return;
        }
    }
    ae_matrix_set_length(&cl, m, minmn, _state);
    for(j=0; j<=minmn-1; j++)
    {
        for(i=0; i<=j-1; i++)
        {
            cl.ptr.pp_complex[i][j] = ae_complex_from_d(0.0);
        }
        cl.ptr.pp_complex[j][j] = ae_complex_from_d(1.0);
        for(i=j+1; i<=m-1; i++)
        {
            cl.ptr.pp_complex[i][j] = ca.ptr.pp_complex[i][j];
        }
    }
    ae_matrix_set_length(&cu, minmn, n, _state);
    for(i=0; i<=minmn-1; i++)
    {
        for(j=0; j<=i-1; j++)
        {
            cu.ptr.pp_complex[i][j] = ae_complex_from_d(0.0);
        }
        for(j=i; j<=n-1; j++)
        {
            cu.ptr.pp_complex[i][j] = ca.ptr.pp_complex[i][j];
        }
    }
    ae_matrix_set_length(&ca2, m, n, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_cdotproduct(&cl.ptr.pp_complex[i][0], 1, "N", &cu.ptr.pp_complex[0][j], cu.stride, "N", ae_v_len(0,minmn-1));
            ca2.ptr.pp_complex[i][j] = v;
        }
    }
    ae_vector_set_length(&ct, n, _state);
    for(i=minmn-1; i>=0; i--)
    {
        if( i!=p.ptr.p_int[i] )
        {
            ae_v_cmove(&ct.ptr.p_complex[0], 1, &ca2.ptr.pp_complex[i][0], 1, "N", ae_v_len(0,n-1));
            ae_v_cmove(&ca2.ptr.pp_complex[i][0], 1, &ca2.ptr.pp_complex[p.ptr.p_int[i]][0], 1, "N", ae_v_len(0,n-1));
            ae_v_cmove(&ca2.ptr.pp_complex[p.ptr.p_int[i]][0], 1, &ct.ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
        }
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            *err = *err||ae_fp_greater(ae_c_abs(ae_c_sub(a->ptr.pp_complex[i][j],ca2.ptr.pp_complex[i][j]), _state),threshold);
        }
    }
    
    /*
     * LUP test
     */
    ae_matrix_set_length(&ca, m, n, _state);
    for(i=0; i<=m-1; i++)
    {
        ae_v_cmove(&ca.ptr.pp_complex[i][0], 1, &a->ptr.pp_complex[i][0], 1, "N", ae_v_len(0,n-1));
    }
    cmatrixlup(&ca, m, n, &p, _state);
    for(i=0; i<=minmn-1; i++)
    {
        if( p.ptr.p_int[i]<i||p.ptr.p_int[i]>=n )
        {
            *properr = ae_true;
            ae_frame_leave(_state);
            return;
        }
    }
    ae_matrix_set_length(&cl, m, minmn, _state);
    for(j=0; j<=minmn-1; j++)
    {
        for(i=0; i<=j-1; i++)
        {
            cl.ptr.pp_complex[i][j] = ae_complex_from_d(0.0);
        }
        for(i=j; i<=m-1; i++)
        {
            cl.ptr.pp_complex[i][j] = ca.ptr.pp_complex[i][j];
        }
    }
    ae_matrix_set_length(&cu, minmn, n, _state);
    for(i=0; i<=minmn-1; i++)
    {
        for(j=0; j<=i-1; j++)
        {
            cu.ptr.pp_complex[i][j] = ae_complex_from_d(0.0);
        }
        cu.ptr.pp_complex[i][i] = ae_complex_from_d(1.0);
        for(j=i+1; j<=n-1; j++)
        {
            cu.ptr.pp_complex[i][j] = ca.ptr.pp_complex[i][j];
        }
    }
    ae_matrix_set_length(&ca2, m, n, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_cdotproduct(&cl.ptr.pp_complex[i][0], 1, "N", &cu.ptr.pp_complex[0][j], cu.stride, "N", ae_v_len(0,minmn-1));
            ca2.ptr.pp_complex[i][j] = v;
        }
    }
    ae_vector_set_length(&ct, m, _state);
    for(i=minmn-1; i>=0; i--)
    {
        if( i!=p.ptr.p_int[i] )
        {
            ae_v_cmove(&ct.ptr.p_complex[0], 1, &ca2.ptr.pp_complex[0][i], ca2.stride, "N", ae_v_len(0,m-1));
            ae_v_cmove(&ca2.ptr.pp_complex[0][i], ca2.stride, &ca2.ptr.pp_complex[0][p.ptr.p_int[i]], ca2.stride, "N", ae_v_len(0,m-1));
            ae_v_cmove(&ca2.ptr.pp_complex[0][p.ptr.p_int[i]], ca2.stride, &ct.ptr.p_complex[0], 1, "N", ae_v_len(0,m-1));
        }
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            *err = *err||ae_fp_greater(ae_c_abs(ae_c_sub(a->ptr.pp_complex[i][j],ca2.ptr.pp_complex[i][j]), _state),threshold);
        }
    }
    ae_frame_leave(_state);
}


static void testtrfacunit_testrluproblem(/* Real    */ ae_matrix* a,
     ae_int_t m,
     ae_int_t n,
     double threshold,
     ae_bool* err,
     ae_bool* properr,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix ca;
    ae_matrix cl;
    ae_matrix cu;
    ae_matrix ca2;
    ae_vector ct;
    ae_int_t i;
    ae_int_t j;
    ae_int_t minmn;
    double v;
    ae_vector p;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&ca, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cl, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cu, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ca2, 0, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_REAL, _state);
    ae_vector_init(&p, 0, DT_INT, _state);

    minmn = ae_minint(m, n, _state);
    
    /*
     * PLU test
     */
    ae_matrix_set_length(&ca, m, n, _state);
    for(i=0; i<=m-1; i++)
    {
        ae_v_move(&ca.ptr.pp_double[i][0], 1, &a->ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
    }
    rmatrixplu(&ca, m, n, &p, _state);
    for(i=0; i<=minmn-1; i++)
    {
        if( p.ptr.p_int[i]<i||p.ptr.p_int[i]>=m )
        {
            *properr = ae_true;
            ae_frame_leave(_state);
            return;
        }
    }
    ae_matrix_set_length(&cl, m, minmn, _state);
    for(j=0; j<=minmn-1; j++)
    {
        for(i=0; i<=j-1; i++)
        {
            cl.ptr.pp_double[i][j] = 0.0;
        }
        cl.ptr.pp_double[j][j] = 1.0;
        for(i=j+1; i<=m-1; i++)
        {
            cl.ptr.pp_double[i][j] = ca.ptr.pp_double[i][j];
        }
    }
    ae_matrix_set_length(&cu, minmn, n, _state);
    for(i=0; i<=minmn-1; i++)
    {
        for(j=0; j<=i-1; j++)
        {
            cu.ptr.pp_double[i][j] = 0.0;
        }
        for(j=i; j<=n-1; j++)
        {
            cu.ptr.pp_double[i][j] = ca.ptr.pp_double[i][j];
        }
    }
    ae_matrix_set_length(&ca2, m, n, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&cl.ptr.pp_double[i][0], 1, &cu.ptr.pp_double[0][j], cu.stride, ae_v_len(0,minmn-1));
            ca2.ptr.pp_double[i][j] = v;
        }
    }
    ae_vector_set_length(&ct, n, _state);
    for(i=minmn-1; i>=0; i--)
    {
        if( i!=p.ptr.p_int[i] )
        {
            ae_v_move(&ct.ptr.p_double[0], 1, &ca2.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
            ae_v_move(&ca2.ptr.pp_double[i][0], 1, &ca2.ptr.pp_double[p.ptr.p_int[i]][0], 1, ae_v_len(0,n-1));
            ae_v_move(&ca2.ptr.pp_double[p.ptr.p_int[i]][0], 1, &ct.ptr.p_double[0], 1, ae_v_len(0,n-1));
        }
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            *err = *err||ae_fp_greater(ae_fabs(a->ptr.pp_double[i][j]-ca2.ptr.pp_double[i][j], _state),threshold);
        }
    }
    
    /*
     * LUP test
     */
    ae_matrix_set_length(&ca, m, n, _state);
    for(i=0; i<=m-1; i++)
    {
        ae_v_move(&ca.ptr.pp_double[i][0], 1, &a->ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
    }
    rmatrixlup(&ca, m, n, &p, _state);
    for(i=0; i<=minmn-1; i++)
    {
        if( p.ptr.p_int[i]<i||p.ptr.p_int[i]>=n )
        {
            *properr = ae_true;
            ae_frame_leave(_state);
            return;
        }
    }
    ae_matrix_set_length(&cl, m, minmn, _state);
    for(j=0; j<=minmn-1; j++)
    {
        for(i=0; i<=j-1; i++)
        {
            cl.ptr.pp_double[i][j] = 0.0;
        }
        for(i=j; i<=m-1; i++)
        {
            cl.ptr.pp_double[i][j] = ca.ptr.pp_double[i][j];
        }
    }
    ae_matrix_set_length(&cu, minmn, n, _state);
    for(i=0; i<=minmn-1; i++)
    {
        for(j=0; j<=i-1; j++)
        {
            cu.ptr.pp_double[i][j] = 0.0;
        }
        cu.ptr.pp_double[i][i] = 1.0;
        for(j=i+1; j<=n-1; j++)
        {
            cu.ptr.pp_double[i][j] = ca.ptr.pp_double[i][j];
        }
    }
    ae_matrix_set_length(&ca2, m, n, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            v = ae_v_dotproduct(&cl.ptr.pp_double[i][0], 1, &cu.ptr.pp_double[0][j], cu.stride, ae_v_len(0,minmn-1));
            ca2.ptr.pp_double[i][j] = v;
        }
    }
    ae_vector_set_length(&ct, m, _state);
    for(i=minmn-1; i>=0; i--)
    {
        if( i!=p.ptr.p_int[i] )
        {
            ae_v_move(&ct.ptr.p_double[0], 1, &ca2.ptr.pp_double[0][i], ca2.stride, ae_v_len(0,m-1));
            ae_v_move(&ca2.ptr.pp_double[0][i], ca2.stride, &ca2.ptr.pp_double[0][p.ptr.p_int[i]], ca2.stride, ae_v_len(0,m-1));
            ae_v_move(&ca2.ptr.pp_double[0][p.ptr.p_int[i]], ca2.stride, &ct.ptr.p_double[0], 1, ae_v_len(0,m-1));
        }
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            *err = *err||ae_fp_greater(ae_fabs(a->ptr.pp_double[i][j]-ca2.ptr.pp_double[i][j], _state),threshold);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Function for testing dense Cholesky updates
Sets error flag to True on errors, does not change it on success.

  -- ALGLIB PROJECT --
     Copyright 16.01.1014 by Bochkanov Sergey
*************************************************************************/
static void testtrfacunit_testdensecholeskyupdates(ae_bool* spdupderrorflag,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    double pfix;
    ae_matrix a0;
    ae_matrix a1;
    ae_vector u;
    ae_vector fix;
    ae_int_t i;
    ae_int_t j;
    ae_bool isupper;
    double tol;
    ae_vector bufr;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a0, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a1, 0, 0, DT_REAL, _state);
    ae_vector_init(&u, 0, DT_REAL, _state);
    ae_vector_init(&fix, 0, DT_BOOL, _state);
    ae_vector_init(&bufr, 0, DT_REAL, _state);
    _hqrndstate_init(&rs, _state);

    hqrndrandomize(&rs, _state);
    
    /*
     * Settings
     */
    tol = 1.0E-8;
    
    /*
     * Test rank-1 updates
     *
     * For each matrix size in 1..30 select sparse update vector with probability of element
     * being non-zero equal to 1/2.
     */
    for(n=1; n<=30; n++)
    {
        
        /*
         * Generate two matrices A0=A1, fill one triangle with SPD matrix,
         * another one with trash. Prepare vector U.
         */
        isupper = ae_fp_less(hqrnduniformr(&rs, _state),0.5);
        spdmatrixrndcond(n, 1.0E4, &a0, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( (j<i&&isupper)||(j>i&&!isupper) )
                {
                    a0.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                }
            }
        }
        ae_matrix_set_length(&a1, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a1.ptr.pp_double[i][j] = a0.ptr.pp_double[i][j];
            }
        }
        ae_vector_set_length(&u, n, _state);
        for(i=0; i<=n-1; i++)
        {
            if( ae_fp_less_eq(hqrnduniformr(&rs, _state),0.5) )
            {
                u.ptr.p_double[i] = hqrnduniformr(&rs, _state)-0.5;
            }
            else
            {
                u.ptr.p_double[i] = (double)(0);
            }
        }
        
        /*
         * Factorize and compare:
         * * A0 is factorized as follows: first with full Cholesky, then
         *   we call SPDMatrixCholeskyUpdateAdd1
         * * A1 is transformed explicitly before factorization with full Cholesky
         *
         * We randomly test either SPDMatrixCholeskyUpdateFix() or its
         * buffered version, SPDMatrixCholeskyUpdateFixBuf()
         */
        seterrorflag(spdupderrorflag, !spdmatrixcholesky(&a0, n, isupper, _state), _state);
        if( *spdupderrorflag )
        {
            ae_frame_leave(_state);
            return;
        }
        if( ae_fp_less(hqrnduniformr(&rs, _state),0.5) )
        {
            spdmatrixcholeskyupdateadd1(&a0, n, isupper, &u, _state);
        }
        else
        {
            spdmatrixcholeskyupdateadd1buf(&a0, n, isupper, &u, &bufr, _state);
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( (j>=i&&isupper)||(j<=i&&!isupper) )
                {
                    a1.ptr.pp_double[i][j] = a1.ptr.pp_double[i][j]+u.ptr.p_double[i]*u.ptr.p_double[j];
                }
            }
        }
        seterrorflag(spdupderrorflag, !spdmatrixcholesky(&a1, n, isupper, _state), _state);
        if( *spdupderrorflag )
        {
            ae_frame_leave(_state);
            return;
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                seterrorflag(spdupderrorflag, ae_fp_greater(ae_fabs(a0.ptr.pp_double[i][j]-a1.ptr.pp_double[i][j], _state),tol), _state);
            }
        }
    }
    
    /*
     * Test variable fixing functions.
     *
     * For each matrix size in 1..30 select PFix - probability of each variable being fixed,
     * and perform test.
     */
    for(n=1; n<=30; n++)
    {
        
        /*
         * Generate two matrices A0=A1, fill one triangle with SPD matrix,
         * another one with trash. Prepare vector Fix.
         */
        pfix = (double)hqrnduniformi(&rs, n+1, _state)/(double)n;
        isupper = ae_fp_less(hqrnduniformr(&rs, _state),0.5);
        spdmatrixrndcond(n, 1.0E4, &a0, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( (j<i&&isupper)||(j>i&&!isupper) )
                {
                    a0.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                }
            }
        }
        ae_matrix_set_length(&a1, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a1.ptr.pp_double[i][j] = a0.ptr.pp_double[i][j];
            }
        }
        ae_vector_set_length(&fix, n, _state);
        for(i=0; i<=n-1; i++)
        {
            fix.ptr.p_bool[i] = ae_fp_less_eq(hqrnduniformr(&rs, _state),pfix);
        }
        
        /*
         * Factorize and compare:
         * * A0 is factorized as follows: first with full Cholesky, then
         *   variables are fixed with SPDMatrixCholeskyUpdateFix
         * * A1 is fixed explicitly before factorization with full Cholesky
         *
         * We randomly test either SPDMatrixCholeskyUpdateFix() or its
         * buffered version, SPDMatrixCholeskyUpdateFixBuf()
         */
        seterrorflag(spdupderrorflag, !spdmatrixcholesky(&a0, n, isupper, _state), _state);
        if( *spdupderrorflag )
        {
            ae_frame_leave(_state);
            return;
        }
        if( ae_fp_less(hqrnduniformr(&rs, _state),0.5) )
        {
            spdmatrixcholeskyupdatefixbuf(&a0, n, isupper, &fix, &bufr, _state);
        }
        else
        {
            spdmatrixcholeskyupdatefix(&a0, n, isupper, &fix, _state);
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( (j>=i&&isupper)||(j<=i&&!isupper) )
                {
                    if( fix.ptr.p_bool[i]||fix.ptr.p_bool[j] )
                    {
                        if( i==j )
                        {
                            a1.ptr.pp_double[i][j] = (double)(1);
                        }
                        else
                        {
                            a1.ptr.pp_double[i][j] = (double)(0);
                        }
                    }
                }
            }
        }
        seterrorflag(spdupderrorflag, !spdmatrixcholesky(&a1, n, isupper, _state), _state);
        if( *spdupderrorflag )
        {
            ae_frame_leave(_state);
            return;
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                seterrorflag(spdupderrorflag, ae_fp_greater(ae_fabs(a0.ptr.pp_double[i][j]-a1.ptr.pp_double[i][j], _state),tol), _state);
            }
        }
    }
    ae_frame_leave(_state);
}


/*$ End $*/
