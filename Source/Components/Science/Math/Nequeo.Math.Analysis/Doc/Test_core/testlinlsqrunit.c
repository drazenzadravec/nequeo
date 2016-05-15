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
#include "testlinlsqrunit.h"


/*$ Declarations $*/
static double testlinlsqrunit_e0 = 1.0E-6;
static double testlinlsqrunit_tolort = 1.0E-4;
static double testlinlsqrunit_e1 = 1.0E+6;
static double testlinlsqrunit_emergencye0 = 1.0E-12;
static ae_bool testlinlsqrunit_svdtest(ae_bool silent, ae_state *_state);
static ae_bool testlinlsqrunit_mwcranksvdtest(ae_bool silent,
     ae_state *_state);
static ae_bool testlinlsqrunit_mwicranksvdtest(ae_bool silent,
     ae_state *_state);
static ae_bool testlinlsqrunit_bidiagonaltest(ae_bool silent,
     ae_state *_state);
static ae_bool testlinlsqrunit_zeromatrixtest(ae_bool silent,
     ae_state *_state);
static ae_bool testlinlsqrunit_reportcorrectnesstest(ae_bool silent,
     ae_state *_state);
static ae_bool testlinlsqrunit_stoppingcriteriatest(ae_bool silent,
     ae_state *_state);
static ae_bool testlinlsqrunit_analytictest(ae_bool silent,
     ae_state *_state);
static ae_bool testlinlsqrunit_isitgoodsolution(/* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* b,
     ae_int_t m,
     ae_int_t n,
     double lambdav,
     /* Real    */ ae_vector* x,
     double epserr,
     double epsort,
     ae_state *_state);
static ae_bool testlinlsqrunit_preconditionertest(ae_state *_state);


/*$ Body $*/


ae_bool testlinlsqr(ae_bool silent, ae_state *_state)
{
    ae_bool svdtesterrors;
    ae_bool mwcranksvderr;
    ae_bool mwicranksvderr;
    ae_bool bidiagonalerr;
    ae_bool zeromatrixerr;
    ae_bool reportcorrectnesserr;
    ae_bool stoppingcriteriaerr;
    ae_bool analytictesterrors;
    ae_bool prectesterrors;
    ae_bool waserrors;
    ae_bool result;


    svdtesterrors = testlinlsqrunit_svdtest(ae_true, _state);
    mwcranksvderr = testlinlsqrunit_mwcranksvdtest(ae_true, _state);
    mwicranksvderr = testlinlsqrunit_mwicranksvdtest(ae_true, _state);
    bidiagonalerr = testlinlsqrunit_bidiagonaltest(ae_true, _state);
    zeromatrixerr = testlinlsqrunit_zeromatrixtest(ae_true, _state);
    reportcorrectnesserr = testlinlsqrunit_reportcorrectnesstest(ae_true, _state);
    stoppingcriteriaerr = testlinlsqrunit_stoppingcriteriatest(ae_true, _state);
    analytictesterrors = testlinlsqrunit_analytictest(ae_true, _state);
    prectesterrors = testlinlsqrunit_preconditionertest(_state);
    
    /*
     * report
     */
    waserrors = (((((((svdtesterrors||mwcranksvderr)||mwicranksvderr)||bidiagonalerr)||zeromatrixerr)||reportcorrectnesserr)||stoppingcriteriaerr)||analytictesterrors)||prectesterrors;
    if( !silent )
    {
        printf("TESTING LinLSQR\n");
        printf("SVDTest:                                      ");
        if( svdtesterrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("MWCRankSVDTest:                               ");
        if( mwcranksvderr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("MWICRankSVDTest:                              ");
        if( mwicranksvderr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("BidiagonalTest:                               ");
        if( bidiagonalerr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("ZeroMatrixTest:                               ");
        if( zeromatrixerr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("ReportCorrectnessTest:                        ");
        if( reportcorrectnesserr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("StoppingCriteriaTest:                         ");
        if( stoppingcriteriaerr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("Analytic properties:                          ");
        if( analytictesterrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("Preconditioner test:                          ");
        if( prectesterrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        
        /*
         *was errors?
         */
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
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testlinlsqr(ae_bool silent, ae_state *_state)
{
    return testlinlsqr(silent, _state);
}


/*************************************************************************
This  function  generates  random  MxN  problem,  solves  it with LSQR and
compares with results obtained from SVD solver. Matrix A is  generated  as
MxN  matrix  with  uniformly  distributed  random  entries, i.e. it has no
special properties (like conditioning or separation of singular values).

We apply random amount regularization to our problem (from zero to  large)
in  order  to  test  regularizer.  Default  stopping  criteria  are  used.
Preconditioning is turned off because it skews results for  rank-deficient
problems.

INPUT: 
    Silent   -   if true then function output report

  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlinlsqrunit_svdtest(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    linlsqrstate s;
    linlsqrreport rep;
    sparsematrix spa;
    ae_matrix a;
    ae_vector b;
    ae_vector x0;
    ae_int_t szn;
    ae_int_t szm;
    ae_int_t n;
    ae_int_t m;
    double lambdai;
    ae_int_t i;
    ae_int_t j;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _linlsqrstate_init(&s, _state);
    _linlsqrreport_init(&rep, _state);
    _sparsematrix_init(&spa, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);

    szm = 5;
    szn = 5;
    for(m=1; m<=szm; m++)
    {
        for(n=1; n<=szn; n++)
        {
            
            /*
             * Prepare MxN matrix A, right part B, lambda
             */
            lambdai = ae_randomreal(_state);
            ae_matrix_set_length(&a, m, n, _state);
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                }
            }
            sparsecreate(m, n, 1, &spa, _state);
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    sparseset(&spa, i, j, a.ptr.pp_double[i][j], _state);
                }
            }
            sparseconverttocrs(&spa, _state);
            ae_vector_set_length(&b, m, _state);
            for(i=0; i<=m-1; i++)
            {
                b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            
            /*
             * Solve by calling LinLSQRIteration
             */
            linlsqrcreate(m, n, &s, _state);
            linlsqrsetb(&s, &b, _state);
            linlsqrsetlambdai(&s, lambdai, _state);
            linlsqrsetprecunit(&s, _state);
            while(linlsqriteration(&s, _state))
            {
                if( s.needmv )
                {
                    for(i=0; i<=m-1; i++)
                    {
                        s.mv.ptr.p_double[i] = (double)(0);
                        for(j=0; j<=n-1; j++)
                        {
                            s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                        }
                    }
                }
                if( s.needmtv )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        s.mtv.ptr.p_double[i] = (double)(0);
                        for(j=0; j<=m-1; j++)
                        {
                            s.mtv.ptr.p_double[i] = s.mtv.ptr.p_double[i]+a.ptr.pp_double[j][i]*s.x.ptr.p_double[j];
                        }
                    }
                }
            }
            linlsqrresults(&s, &x0, &rep, _state);
            if( !testlinlsqrunit_isitgoodsolution(&a, &b, m, n, lambdai, &x0, testlinlsqrunit_e0, testlinlsqrunit_tolort, _state) )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            
            /*
             *test LinLSQRRestart and LinLSQRSolveSparse
             */
            linlsqrrestart(&s, _state);
            linlsqrsolvesparse(&s, &spa, &b, _state);
            linlsqrresults(&s, &x0, &rep, _state);
            if( !testlinlsqrunit_isitgoodsolution(&a, &b, m, n, lambdai, &x0, testlinlsqrunit_e0, testlinlsqrunit_tolort, _state) )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
    }
    if( !silent )
    {
        printf("SVDTest::Ok\n");
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
The test checks that algorithm can solve MxN (with N<=M)  well-conditioned
problems - and can do so within exactly  N  iterations.  We  use  moderate
condition numbers, from 1.0 to 10.0, because larger condition  number  may
require several additional iterations to converge.

We try different scalings of the A and B.

INPUT: 
    Silent   -   if true then function does not outputs results to console

  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlinlsqrunit_mwcranksvdtest(ae_bool silent,
     ae_state *_state)
{
    ae_frame _frame_block;
    linlsqrstate s;
    linlsqrreport rep;
    ae_matrix a;
    ae_vector b;
    double bnorm;
    ae_vector x0;
    ae_int_t szm;
    ae_int_t n;
    ae_int_t m;
    ae_int_t ns0;
    ae_int_t ns1;
    ae_int_t nlambdai;
    double s0;
    double s1;
    double lambdai;
    double c;
    ae_int_t i;
    ae_int_t j;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _linlsqrstate_init(&s, _state);
    _linlsqrreport_init(&rep, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);

    szm = 5;
    for(m=1; m<=szm; m++)
    {
        for(n=1; n<=m; n++)
        {
            for(nlambdai=0; nlambdai<=3; nlambdai++)
            {
                for(ns0=-1; ns0<=1; ns0++)
                {
                    for(ns1=-1; ns1<=1; ns1++)
                    {
                        
                        /*
                         * Generate problem:
                         * * scale factors s0, s1
                         * * MxN well conditioned A (with condition number C in [1,10] and norm s0)
                         * * regularization coefficient LambdaI
                         * * right part b, with |b|=s1
                         */
                        s0 = ae_pow((double)(10), (double)(10*ns0), _state);
                        s1 = ae_pow((double)(10), (double)(10*ns1), _state);
                        lambdai = (double)(0);
                        if( nlambdai==0 )
                        {
                            lambdai = (double)(0);
                        }
                        if( nlambdai==1 )
                        {
                            lambdai = s0/1000;
                        }
                        if( nlambdai==2 )
                        {
                            lambdai = s0;
                        }
                        if( nlambdai==3 )
                        {
                            lambdai = s0*1000;
                        }
                        c = (10-1)*ae_randomreal(_state)+1;
                        rmatrixrndcond(m, c, &a, _state);
                        for(i=0; i<=m-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                a.ptr.pp_double[i][j] = s0*a.ptr.pp_double[i][j];
                            }
                        }
                        ae_vector_set_length(&b, m, _state);
                        do
                        {
                            bnorm = (double)(0);
                            for(i=0; i<=m-1; i++)
                            {
                                b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                                bnorm = bnorm+b.ptr.p_double[i]*b.ptr.p_double[i];
                            }
                            bnorm = ae_sqrt(bnorm, _state);
                        }
                        while(ae_fp_less_eq(bnorm,testlinlsqrunit_e0));
                        for(i=0; i<=m-1; i++)
                        {
                            b.ptr.p_double[i] = b.ptr.p_double[i]*s1/bnorm;
                        }
                        
                        /*
                         * Solve by LSQR method
                         */
                        linlsqrcreate(m, n, &s, _state);
                        linlsqrsetb(&s, &b, _state);
                        linlsqrsetcond(&s, (double)(0), (double)(0), n, _state);
                        linlsqrsetlambdai(&s, lambdai, _state);
                        while(linlsqriteration(&s, _state))
                        {
                            if( s.needmv )
                            {
                                for(i=0; i<=m-1; i++)
                                {
                                    s.mv.ptr.p_double[i] = (double)(0);
                                    for(j=0; j<=n-1; j++)
                                    {
                                        s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                                    }
                                }
                            }
                            if( s.needmtv )
                            {
                                for(i=0; i<=n-1; i++)
                                {
                                    s.mtv.ptr.p_double[i] = (double)(0);
                                    for(j=0; j<=m-1; j++)
                                    {
                                        s.mtv.ptr.p_double[i] = s.mtv.ptr.p_double[i]+a.ptr.pp_double[j][i]*s.x.ptr.p_double[j];
                                    }
                                }
                            }
                        }
                        linlsqrresults(&s, &x0, &rep, _state);
                        if( !testlinlsqrunit_isitgoodsolution(&a, &b, m, n, lambdai, &x0, testlinlsqrunit_e0, testlinlsqrunit_tolort, _state) )
                        {
                            result = ae_true;
                            ae_frame_leave(_state);
                            return result;
                        }
                    }
                }
            }
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
The test checks that algorithm can find a solution with minimum norm for a
singular rectangular problem. System matrix has special property - singular 
values are either zero or well separated from zero.

INPUT: 
    Silent   -   if true then function output report

  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlinlsqrunit_mwicranksvdtest(ae_bool silent,
     ae_state *_state)
{
    ae_frame _frame_block;
    linlsqrstate s;
    linlsqrreport rep;
    sparsematrix spa;
    ae_vector b;
    double bnorm;
    ae_vector x0;
    ae_int_t szm;
    ae_int_t n;
    ae_int_t m;
    ae_int_t nz;
    ae_int_t ns0;
    ae_int_t ns1;
    ae_int_t nlambdai;
    double s0;
    double s1;
    double lambdai;
    ae_int_t i;
    ae_int_t j;
    ae_matrix a;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _linlsqrstate_init(&s, _state);
    _linlsqrreport_init(&rep, _state);
    _sparsematrix_init(&spa, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);

    result = ae_false;
    szm = 5;
    for(m=1; m<=szm; m++)
    {
        for(n=1; n<=m; n++)
        {
            for(nlambdai=0; nlambdai<=2; nlambdai++)
            {
                for(ns0=-1; ns0<=1; ns0++)
                {
                    for(ns1=-1; ns1<=1; ns1++)
                    {
                        for(nz=0; nz<=n-1; nz++)
                        {
                            
                            /*
                             * Generate problem:
                             * * scale coefficients s0, s1
                             * * regularization coefficient LambdaI
                             * * MxN matrix A, norm(A)=s0, with NZ zero singular values and N-NZ nonzero ones
                             * * right part b with norm(b)=s1
                             */
                            s0 = ae_pow((double)(10), (double)(10*ns0), _state);
                            s1 = ae_pow((double)(10), (double)(10*ns1), _state);
                            lambdai = (double)(0);
                            if( nlambdai==0 )
                            {
                                lambdai = (double)(0);
                            }
                            if( nlambdai==1 )
                            {
                                lambdai = s0;
                            }
                            if( nlambdai==2 )
                            {
                                lambdai = s0*1000;
                            }
                            ae_matrix_set_length(&a, m, n, _state);
                            for(i=0; i<=m-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    a.ptr.pp_double[i][j] = (double)(0);
                                }
                            }
                            for(i=0; i<=n-nz-1; i++)
                            {
                                a.ptr.pp_double[i][i] = s0*(0.1+0.9*ae_randomreal(_state));
                            }
                            rmatrixrndorthogonalfromtheleft(&a, m, n, _state);
                            rmatrixrndorthogonalfromtheright(&a, m, n, _state);
                            ae_vector_set_length(&b, m, _state);
                            do
                            {
                                bnorm = (double)(0);
                                for(i=0; i<=m-1; i++)
                                {
                                    b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                                    bnorm = bnorm+b.ptr.p_double[i]*b.ptr.p_double[i];
                                }
                                bnorm = ae_sqrt(bnorm, _state);
                            }
                            while(ae_fp_less_eq(bnorm,testlinlsqrunit_e0));
                            for(i=0; i<=m-1; i++)
                            {
                                b.ptr.p_double[i] = b.ptr.p_double[i]*s1/bnorm;
                            }
                            
                            /*
                             * Solve by LSQR method
                             */
                            linlsqrcreate(m, n, &s, _state);
                            linlsqrsetb(&s, &b, _state);
                            linlsqrsetcond(&s, testlinlsqrunit_emergencye0, testlinlsqrunit_emergencye0, n, _state);
                            linlsqrsetlambdai(&s, lambdai, _state);
                            while(linlsqriteration(&s, _state))
                            {
                                if( s.needmv )
                                {
                                    for(i=0; i<=m-1; i++)
                                    {
                                        s.mv.ptr.p_double[i] = (double)(0);
                                        for(j=0; j<=n-1; j++)
                                        {
                                            s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                                        }
                                    }
                                }
                                if( s.needmtv )
                                {
                                    for(i=0; i<=n-1; i++)
                                    {
                                        s.mtv.ptr.p_double[i] = (double)(0);
                                        for(j=0; j<=m-1; j++)
                                        {
                                            s.mtv.ptr.p_double[i] = s.mtv.ptr.p_double[i]+a.ptr.pp_double[j][i]*s.x.ptr.p_double[j];
                                        }
                                    }
                                }
                            }
                            linlsqrresults(&s, &x0, &rep, _state);
                            
                            /*
                             * Check
                             */
                            if( !testlinlsqrunit_isitgoodsolution(&a, &b, m, n, lambdai, &x0, testlinlsqrunit_e0, testlinlsqrunit_tolort, _state) )
                            {
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                        }
                    }
                }
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
The test does check, that algorithm can find a solution with minimum norm,
if a problem has bidiagonal matrix on diagonals of a lot of zeros. This
problem has to lead to case when State.Alpha and State.Beta are zero, and we
we can be sure that the algorithm correctly handles it.

We do not use iteration count as stopping condition, because problem can
be degenerate and we may need more than N iterations to converge.

INPUT: 
    Silent   -   if true then function output report

  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlinlsqrunit_bidiagonaltest(ae_bool silent,
     ae_state *_state)
{
    ae_frame _frame_block;
    linlsqrstate s;
    linlsqrreport rep;
    ae_matrix a;
    ae_vector b;
    double bnorm;
    ae_vector x0;
    ae_int_t sz;
    ae_int_t n;
    ae_int_t m;
    ae_int_t minmn;
    ae_int_t ns0;
    ae_int_t ns1;
    double s0;
    double s1;
    ae_int_t i;
    ae_int_t j;
    ae_int_t p;
    ae_int_t diag;
    double pz;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _linlsqrstate_init(&s, _state);
    _linlsqrreport_init(&rep, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);

    sz = 5;
    for(m=1; m<=sz; m++)
    {
        for(n=1; n<=sz; n++)
        {
            minmn = ae_minint(m, n, _state);
            for(p=0; p<=2; p++)
            {
                for(ns0=-1; ns0<=1; ns0++)
                {
                    for(ns1=-1; ns1<=1; ns1++)
                    {
                        for(diag=0; diag<=1; diag++)
                        {
                            
                            /*
                             * Generate problem:
                             * * scaling coefficients s0, s1
                             * * bidiagonal A, with probability of having zero element at diagonal equal to PZ
                             */
                            s0 = ae_pow((double)(10), (double)(10*ns0), _state);
                            s1 = ae_pow((double)(10), (double)(10*ns1), _state);
                            pz = 0.0;
                            if( p==0 )
                            {
                                pz = 0.25;
                            }
                            if( p==1 )
                            {
                                pz = 0.5;
                            }
                            if( p==2 )
                            {
                                pz = 0.75;
                            }
                            ae_matrix_set_length(&a, m, n, _state);
                            for(i=0; i<=m-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    a.ptr.pp_double[i][j] = (double)(0);
                                }
                            }
                            for(i=0; i<=minmn-1; i++)
                            {
                                if( ae_fp_greater_eq(ae_randomreal(_state),pz) )
                                {
                                    a.ptr.pp_double[i][i] = 2*ae_randomreal(_state)-1;
                                }
                            }
                            for(i=1; i<=minmn-1; i++)
                            {
                                if( ae_fp_greater_eq(ae_randomreal(_state),pz) )
                                {
                                    if( diag==0 )
                                    {
                                        a.ptr.pp_double[i-1][i] = 2*ae_randomreal(_state)-1;
                                    }
                                    if( diag==1 )
                                    {
                                        a.ptr.pp_double[i][i-1] = 2*ae_randomreal(_state)-1;
                                    }
                                }
                            }
                            for(i=0; i<=m-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    a.ptr.pp_double[i][j] = s0*a.ptr.pp_double[i][j];
                                }
                            }
                            ae_vector_set_length(&b, m, _state);
                            do
                            {
                                bnorm = (double)(0);
                                for(i=0; i<=m-1; i++)
                                {
                                    b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                                    bnorm = bnorm+b.ptr.p_double[i]*b.ptr.p_double[i];
                                }
                                bnorm = ae_sqrt(bnorm, _state);
                            }
                            while(ae_fp_less_eq(bnorm,testlinlsqrunit_e0));
                            for(i=0; i<=m-1; i++)
                            {
                                b.ptr.p_double[i] = b.ptr.p_double[i]*s1/bnorm;
                            }
                            
                            /*
                             * LSQR solution
                             */
                            linlsqrcreate(m, n, &s, _state);
                            linlsqrsetb(&s, &b, _state);
                            linlsqrsetcond(&s, testlinlsqrunit_e0, testlinlsqrunit_e0, 0, _state);
                            while(linlsqriteration(&s, _state))
                            {
                                if( s.needmv )
                                {
                                    for(i=0; i<=m-1; i++)
                                    {
                                        s.mv.ptr.p_double[i] = (double)(0);
                                        for(j=0; j<=n-1; j++)
                                        {
                                            s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                                        }
                                    }
                                }
                                if( s.needmtv )
                                {
                                    for(i=0; i<=n-1; i++)
                                    {
                                        s.mtv.ptr.p_double[i] = (double)(0);
                                        for(j=0; j<=m-1; j++)
                                        {
                                            s.mtv.ptr.p_double[i] = s.mtv.ptr.p_double[i]+a.ptr.pp_double[j][i]*s.x.ptr.p_double[j];
                                        }
                                    }
                                }
                            }
                            linlsqrresults(&s, &x0, &rep, _state);
                            
                            /*
                             * Check
                             */
                            if( !testlinlsqrunit_isitgoodsolution(&a, &b, m, n, 0.0, &x0, testlinlsqrunit_e0, testlinlsqrunit_tolort, _state) )
                            {
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                        }
                    }
                }
            }
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
The test does check, that algorithm correctly solves a problem in cases:
    1. A=0, B<>0;
    2. A<>0, B=0;
    3. A=0, B=0.
If some part is not zero then it filled with ones.

INPUT: 
    Silent   -   if true then function output report

  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlinlsqrunit_zeromatrixtest(ae_bool silent,
     ae_state *_state)
{
    ae_frame _frame_block;
    linlsqrstate s;
    linlsqrreport rep;
    ae_matrix a;
    ae_vector b;
    ae_vector x0;
    ae_int_t sz;
    ae_int_t n;
    ae_int_t m;
    ae_int_t i;
    ae_int_t j;
    ae_int_t nzeropart;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _linlsqrstate_init(&s, _state);
    _linlsqrreport_init(&rep, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);

    sz = 5;
    result = ae_false;
    for(m=1; m<=sz; m++)
    {
        for(n=1; n<=sz; n++)
        {
            for(nzeropart=0; nzeropart<=2; nzeropart++)
            {
                
                /*
                 * Initialize A, b
                 */
                ae_matrix_set_length(&a, m, n, _state);
                if( nzeropart==0||nzeropart==2 )
                {
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_double[i][j] = (double)(0);
                        }
                    }
                }
                else
                {
                    for(i=0; i<=m-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_double[i][j] = (double)(1);
                        }
                    }
                }
                ae_vector_set_length(&b, m, _state);
                if( nzeropart==1||nzeropart==2 )
                {
                    for(i=0; i<=m-1; i++)
                    {
                        b.ptr.p_double[i] = (double)(0);
                    }
                }
                else
                {
                    for(i=0; i<=m-1; i++)
                    {
                        b.ptr.p_double[i] = (double)(1);
                    }
                }
                
                /*
                 * LSQR
                 */
                linlsqrcreate(m, n, &s, _state);
                linlsqrsetb(&s, &b, _state);
                linlsqrsetcond(&s, (double)(0), (double)(0), n, _state);
                while(linlsqriteration(&s, _state))
                {
                    if( s.needmv )
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            s.mv.ptr.p_double[i] = (double)(0);
                            for(j=0; j<=n-1; j++)
                            {
                                s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                            }
                        }
                    }
                    if( s.needmtv )
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            s.mtv.ptr.p_double[i] = (double)(0);
                            for(j=0; j<=m-1; j++)
                            {
                                s.mtv.ptr.p_double[i] = s.mtv.ptr.p_double[i]+a.ptr.pp_double[j][i]*s.x.ptr.p_double[j];
                            }
                        }
                    }
                }
                linlsqrresults(&s, &x0, &rep, _state);
                
                /*
                 * Check
                 */
                if( !testlinlsqrunit_isitgoodsolution(&a, &b, m, n, 0.0, &x0, testlinlsqrunit_e0, testlinlsqrunit_tolort, _state) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
The test does check, that algorithm correctly displays a progress report.

INPUT: 
    Silent   -   if true then function output report

  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlinlsqrunit_reportcorrectnesstest(ae_bool silent,
     ae_state *_state)
{
    ae_frame _frame_block;
    linlsqrstate s;
    linlsqrreport rep;
    ae_matrix a;
    ae_matrix u;
    ae_matrix v;
    ae_vector w;
    ae_vector b;
    ae_vector x0;
    ae_vector firstx;
    ae_vector lastx;
    double rnorm;
    double tnorm;
    ae_int_t sz;
    ae_int_t n;
    ae_int_t m;
    ae_int_t lambdai;
    double mn;
    double mx;
    double c;
    ae_int_t i;
    ae_int_t j;
    ae_int_t its;
    double tmp;
    double eps;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _linlsqrstate_init(&s, _state);
    _linlsqrreport_init(&rep, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&u, 0, 0, DT_REAL, _state);
    ae_matrix_init(&v, 0, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&firstx, 0, DT_REAL, _state);
    ae_vector_init(&lastx, 0, DT_REAL, _state);

    eps = 0.001;
    sz = 5;
    mn = (double)(-100);
    mx = (double)(100);
    c = (double)(100);
    for(m=1; m<=sz; m++)
    {
        for(n=1; n<=m; n++)
        {
            for(lambdai=0; lambdai<=1; lambdai++)
            {
                its = -1;
                
                /*
                 *initialize matrix A
                 */
                spdmatrixrndcond(m+n, c, &a, _state);
                for(i=m; i<=m+n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( i-m==j )
                        {
                            a.ptr.pp_double[i][j] = (double)(lambdai);
                        }
                        else
                        {
                            a.ptr.pp_double[i][j] = (double)(0);
                        }
                    }
                }
                
                /*
                 *initialize b
                 */
                ae_vector_set_length(&b, m+n, _state);
                rnorm = (double)(0);
                for(i=0; i<=m+n-1; i++)
                {
                    if( i<m )
                    {
                        b.ptr.p_double[i] = (mx-mn)*ae_randomreal(_state)+mn;
                        rnorm = rnorm+b.ptr.p_double[i]*b.ptr.p_double[i];
                    }
                    else
                    {
                        b.ptr.p_double[i] = (double)(0);
                    }
                }
                
                /*
                 *initialize FirstX and LastX
                 */
                ae_vector_set_length(&firstx, n, _state);
                ae_vector_set_length(&lastx, n, _state);
                
                /*
                 *calculating by LSQR method
                 */
                linlsqrcreate(m, n, &s, _state);
                linlsqrsetb(&s, &b, _state);
                linlsqrsetcond(&s, (double)(0), (double)(0), n, _state);
                linlsqrsetlambdai(&s, (double)(lambdai), _state);
                linlsqrsetxrep(&s, ae_true, _state);
                while(linlsqriteration(&s, _state))
                {
                    if( s.needmv )
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            s.mv.ptr.p_double[i] = (double)(0);
                            for(j=0; j<=n-1; j++)
                            {
                                s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                            }
                        }
                    }
                    if( s.needmtv )
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            s.mtv.ptr.p_double[i] = (double)(0);
                            for(j=0; j<=m-1; j++)
                            {
                                s.mtv.ptr.p_double[i] = s.mtv.ptr.p_double[i]+a.ptr.pp_double[j][i]*s.x.ptr.p_double[j];
                            }
                        }
                    }
                    if( s.xupdated )
                    {
                        tnorm = (double)(0);
                        for(i=0; i<=m+n-1; i++)
                        {
                            tmp = (double)(0);
                            for(j=0; j<=n-1; j++)
                            {
                                tmp = tmp+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                            }
                            tnorm = tnorm+(b.ptr.p_double[i]-tmp)*(b.ptr.p_double[i]-tmp);
                        }
                        
                        /*
                         *check, that RNorm is't more than S.R2
                         *and difference between S.R2 and TNorm
                         *is't more than 'eps'(here S.R2=||rk||,
                         *calculated by the algorithm for LSQR, and
                         *TNorm=||A*S.x-b||, calculated by test function).
                         */
                        if( ae_fp_greater(s.r2,rnorm)||ae_fp_greater(ae_fabs(s.r2-tnorm, _state),eps) )
                        {
                            if( !silent )
                            {
                                printf("ReportCorrectnessTest::Fail\n");
                                printf("TNorm=%0.2e;RNorm=%0.2e;S.R2=%0.2e;\n",
                                    (double)(tnorm),
                                    (double)(rnorm),
                                    (double)(s.r2));
                            }
                            result = ae_true;
                            ae_frame_leave(_state);
                            return result;
                        }
                        rnorm = s.r2;
                        its = its+1;
                        
                        /*
                         *get X value from first iteration 
                         *and from last iteration.
                         */
                        if( its==0 )
                        {
                            for(i=0; i<=n-1; i++)
                            {
                                firstx.ptr.p_double[i] = s.x.ptr.p_double[i];
                            }
                        }
                        if( its==n )
                        {
                            for(i=0; i<=n-1; i++)
                            {
                                lastx.ptr.p_double[i] = s.x.ptr.p_double[i];
                            }
                        }
                    }
                }
                linlsqrresults(&s, &x0, &rep, _state);
                
                /*
                 *check, that FirstX is equal to zero and LastX
                 *is equal to x0.
                 */
                for(i=0; i<=n-1; i++)
                {
                    if( ae_fp_neq(firstx.ptr.p_double[i],(double)(0))||ae_fp_neq(lastx.ptr.p_double[i],x0.ptr.p_double[i]) )
                    {
                        if( !silent )
                        {
                            printf("ReportCorrectnessTest::Fail\n");
                            printf("IterationsCount=%0d\n",
                                (int)(rep.iterationscount));
                            printf("NMV=%0d\n",
                                (int)(rep.nmv));
                            printf("TerminationType=%0d\n",
                                (int)(rep.terminationtype));
                            printf("X and LastX...\n");
                            for(j=0; j<=n-1; j++)
                            {
                                printf("x[%0d]=%0.10e; LastX[%0d]=%0.10e\n",
                                    (int)(j),
                                    (double)(x0.ptr.p_double[j]),
                                    (int)(j),
                                    (double)(lastx.ptr.p_double[j]));
                            }
                        }
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
        }
    }
    if( !silent )
    {
        printf("ReportCorrectnessTest::Ok\n");
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
The test does check, that correctly executed stop criteria by algorithm.

INPUT: 
    Silent   -   if true then function output report

  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlinlsqrunit_stoppingcriteriatest(ae_bool silent,
     ae_state *_state)
{
    ae_frame _frame_block;
    linlsqrstate s;
    linlsqrreport rep;
    ae_matrix a;
    ae_matrix u;
    ae_matrix v;
    ae_vector w;
    ae_vector b;
    double bnorm;
    ae_vector x0;
    ae_int_t sz;
    ae_int_t n;
    ae_int_t k0;
    ae_int_t k1;
    ae_vector ark;
    double anorm;
    double arknorm;
    double rknorm;
    ae_vector rk;
    double mn;
    double mx;
    double c;
    ae_int_t maxits;
    ae_int_t i;
    ae_int_t j;
    double tmp;
    double eps;
    double epsmod;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _linlsqrstate_init(&s, _state);
    _linlsqrreport_init(&rep, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&u, 0, 0, DT_REAL, _state);
    ae_matrix_init(&v, 0, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&ark, 0, DT_REAL, _state);
    ae_vector_init(&rk, 0, DT_REAL, _state);

    sz = 5;
    mn = (double)(-100);
    mx = (double)(100);
    c = (double)(100);
    for(n=1; n<=sz; n++)
    {
        
        /*
         * Initialize A, unit b
         */
        spdmatrixrndcond(n, c, &a, _state);
        ae_vector_set_length(&b, n, _state);
        bnorm = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            b.ptr.p_double[i] = (mx-mn)*ae_randomreal(_state)+mn;
            bnorm = bnorm+b.ptr.p_double[i]*b.ptr.p_double[i];
        }
        bnorm = ae_sqrt(bnorm, _state);
        
        /*
         * Test MaxIts
         *
         * NOTE: we do not check TerminationType because algorithm may terminate for
         * several reasons. The only thing which is guaranteed is that stopping condition
         * MaxIts holds.
         */
        maxits = 1+ae_randominteger(n, _state);
        linlsqrcreate(n, n, &s, _state);
        linlsqrsetb(&s, &b, _state);
        linlsqrsetcond(&s, (double)(0), (double)(0), maxits, _state);
        while(linlsqriteration(&s, _state))
        {
            if( s.needmv )
            {
                for(i=0; i<=n-1; i++)
                {
                    s.mv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                    }
                }
            }
            if( s.needmtv )
            {
                for(i=0; i<=n-1; i++)
                {
                    s.mtv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        s.mtv.ptr.p_double[i] = s.mtv.ptr.p_double[i]+a.ptr.pp_double[j][i]*s.x.ptr.p_double[j];
                    }
                }
            }
        }
        linlsqrresults(&s, &x0, &rep, _state);
        if( rep.iterationscount>maxits||rep.terminationtype<=0 )
        {
            if( !silent )
            {
                printf("StoppingCriteriaTest::Fail\n");
                printf("N=%0d\n",
                    (int)(n));
                printf("IterationsCount=%0d\n",
                    (int)(rep.iterationscount));
                printf("NMV=%0d\n",
                    (int)(rep.nmv));
                printf("TerminationType=%0d\n",
                    (int)(rep.terminationtype));
            }
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Test EpsB.
         * Set EpsB=eps, check that |r|<epsMod*|b|, where epsMod=1.1*eps.
         * This modified epsilon is used to avoid influence of the numerical errors.
         *
         * NOTE: we do not check TerminationType because algorithm may terminate for
         * several reasons. The only thing which is guaranteed is that stopping condition
         * EpsB holds.
         */
        eps = ae_pow((double)(10), (double)(-(2+ae_randominteger(3, _state))), _state);
        epsmod = 1.1*eps;
        ae_vector_set_length(&rk, n, _state);
        linlsqrcreate(n, n, &s, _state);
        linlsqrsetb(&s, &b, _state);
        linlsqrsetcond(&s, (double)(0), eps, 0, _state);
        while(linlsqriteration(&s, _state))
        {
            if( s.needmv )
            {
                for(i=0; i<=n-1; i++)
                {
                    s.mv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                    }
                }
            }
            if( s.needmtv )
            {
                for(i=0; i<=n-1; i++)
                {
                    s.mtv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        s.mtv.ptr.p_double[i] = s.mtv.ptr.p_double[i]+a.ptr.pp_double[j][i]*s.x.ptr.p_double[j];
                    }
                }
            }
        }
        linlsqrresults(&s, &x0, &rep, _state);
        rknorm = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            tmp = (double)(0);
            for(j=0; j<=n-1; j++)
            {
                tmp = tmp+a.ptr.pp_double[i][j]*x0.ptr.p_double[j];
            }
            rknorm = rknorm+(tmp-b.ptr.p_double[i])*(tmp-b.ptr.p_double[i]);
        }
        rknorm = ae_sqrt(rknorm, _state);
        if( ae_fp_greater(rknorm,epsmod*bnorm)||rep.terminationtype<=0 )
        {
            if( !silent )
            {
                printf("StoppingCriteriaTest::Fail\n");
                printf("rkNorm=%0.2e\n",
                    (double)(rknorm));
                printf("IterationsCount=%0d\n",
                    (int)(rep.iterationscount));
                printf("NMV=%0d\n",
                    (int)(rep.nmv));
                printf("TerminationType=%0d\n",
                    (int)(rep.terminationtype));
            }
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    
    /*
     * Test EpsA.
     *
     * Generate well conditioned underdetermined system with nonzero residual
     * at the solution. Such system can be generated by generating random
     * orthogonal matrix (N>=2) and using first N-1 columns as rectangular
     * system matrix, and sum of all columns with random non-zero coefficients 
     * as right part.
     */
    for(n=2; n<=sz; n++)
    {
        for(k0=-1; k0<=1; k0++)
        {
            for(k1=-1; k1<=1; k1++)
            {
                
                /*
                 * Initialize A with non-unit norm 10^(10*K0), b with non-unit norm 10^(10*K1)
                 */
                anorm = ae_pow((double)(10), (double)(10*k0), _state);
                rmatrixrndorthogonal(n, &a, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a.ptr.pp_double[i][j] = anorm*a.ptr.pp_double[i][j];
                    }
                }
                ae_vector_set_length(&b, n, _state);
                for(j=0; j<=n-1; j++)
                {
                    b.ptr.p_double[j] = (double)(0);
                }
                for(i=0; i<=n-1; i++)
                {
                    tmp = 1+ae_randomreal(_state);
                    ae_v_addd(&b.ptr.p_double[0], 1, &a.ptr.pp_double[0][i], a.stride, ae_v_len(0,n-1), tmp);
                }
                tmp = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    tmp = tmp+ae_sqr(b.ptr.p_double[i], _state);
                }
                tmp = ae_pow((double)(10), (double)(10*k1), _state)/ae_sqrt(tmp, _state);
                ae_v_muld(&b.ptr.p_double[0], 1, ae_v_len(0,n-1), tmp);
                
                /*
                 * Test EpsA
                 *
                 * NOTE: it is guaranteed that algorithm will terminate with correct
                 * TerminationType because other stopping criteria (EpsB) won't be satisfied
                 * on such system.
                 */
                eps = ae_pow((double)(10), (double)(-(2+ae_randominteger(3, _state))), _state);
                epsmod = 1.1*eps;
                linlsqrcreate(n, n-1, &s, _state);
                linlsqrsetb(&s, &b, _state);
                linlsqrsetcond(&s, eps, (double)(0), 0, _state);
                while(linlsqriteration(&s, _state))
                {
                    if( s.needmv )
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            s.mv.ptr.p_double[i] = (double)(0);
                            for(j=0; j<=n-2; j++)
                            {
                                s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                            }
                        }
                    }
                    if( s.needmtv )
                    {
                        for(i=0; i<=n-2; i++)
                        {
                            s.mtv.ptr.p_double[i] = (double)(0);
                            for(j=0; j<=n-1; j++)
                            {
                                s.mtv.ptr.p_double[i] = s.mtv.ptr.p_double[i]+a.ptr.pp_double[j][i]*s.x.ptr.p_double[j];
                            }
                        }
                    }
                }
                linlsqrresults(&s, &x0, &rep, _state);
                
                /*
                 * Check condition
                 */
                ae_vector_set_length(&rk, n, _state);
                ae_vector_set_length(&ark, n-1, _state);
                rknorm = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    rk.ptr.p_double[i] = b.ptr.p_double[i];
                    for(j=0; j<=n-2; j++)
                    {
                        rk.ptr.p_double[i] = rk.ptr.p_double[i]-a.ptr.pp_double[i][j]*x0.ptr.p_double[j];
                    }
                    rknorm = rknorm+ae_sqr(rk.ptr.p_double[i], _state);
                }
                rknorm = ae_sqrt(rknorm, _state);
                arknorm = (double)(0);
                for(i=0; i<=n-2; i++)
                {
                    ark.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        ark.ptr.p_double[i] = ark.ptr.p_double[i]+a.ptr.pp_double[j][i]*rk.ptr.p_double[j];
                    }
                    arknorm = arknorm+ae_sqr(ark.ptr.p_double[i], _state);
                }
                arknorm = ae_sqrt(arknorm, _state);
                if( ae_fp_greater(arknorm/(anorm*rknorm),epsmod)||rep.terminationtype!=4 )
                {
                    if( !silent )
                    {
                        printf("StoppingCriteriaTest::Fail\n");
                        printf("N=%0d\n",
                            (int)(n));
                        printf("IterationsCount=%0d\n",
                            (int)(rep.iterationscount));
                        printf("NMV=%0d\n",
                            (int)(rep.nmv));
                        printf("TerminationType=%0d\n",
                            (int)(rep.terminationtype));
                    }
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
    }
    if( !silent )
    {
        printf("StoppingCriteriaTest::Ok\n");
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This test compares LSQR  for  original  system  A*x=b  against  CG  for  a
modified system (A'*A)x = A*b. Both algorithms should give same  sequences
of trial points (under exact arithmetics, or  for  very  good  conditioned
systems).

INPUT: 
    Silent   -   if true then function does not output report

  -- ALGLIB --
     Copyright 30.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlinlsqrunit_analytictest(ae_bool silent,
     ae_state *_state)
{
    ae_frame _frame_block;
    linlsqrstate s;
    linlsqrreport rep;
    ae_matrix a;
    ae_matrix xk;
    ae_matrix ap;
    ae_matrix r;
    ae_vector b;
    ae_vector tmp;
    ae_int_t n;
    ae_int_t m;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t smallk;
    ae_int_t pointsstored;
    double v;
    double tol;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _linlsqrstate_init(&s, _state);
    _linlsqrreport_init(&rep, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xk, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ap, 0, 0, DT_REAL, _state);
    ae_matrix_init(&r, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);

    
    /*
     * Set:
     * * SmallK - number of steps to check, must be small number in order
     *   to reduce influence of the rounding errors
     * * Tol - error tolerance for orthogonality/conjugacy criteria
     */
    result = ae_false;
    smallk = 4;
    tol = 1.0E-7;
    for(m=smallk; m<=smallk+5; m++)
    {
        for(n=smallk; n<=m; n++)
        {
            
            /*
             * Prepare problem:
             * * MxN matrix A, Mx1 vector B
             * * A is filled with random values from [-1,+1]
             * * diagonal elements are filled with large positive values
             *   (should make system better conditioned)
             */
            ae_matrix_set_length(&a, m, n, _state);
            ae_vector_set_length(&b, m, _state);
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                }
                b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            for(i=0; i<=n-1; i++)
            {
                a.ptr.pp_double[i][i] = 10*(1+ae_randomreal(_state));
            }
            
            /*
             * Solve with LSQR, save trial points into XK[] array
             */
            ae_matrix_set_length(&xk, smallk+1, n, _state);
            linlsqrcreate(m, n, &s, _state);
            linlsqrsetb(&s, &b, _state);
            linlsqrsetcond(&s, (double)(0), (double)(0), smallk, _state);
            linlsqrsetxrep(&s, ae_true, _state);
            pointsstored = 0;
            while(linlsqriteration(&s, _state))
            {
                if( s.needmv )
                {
                    for(i=0; i<=m-1; i++)
                    {
                        s.mv.ptr.p_double[i] = (double)(0);
                        for(j=0; j<=n-1; j++)
                        {
                            s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                        }
                    }
                }
                if( s.needmtv )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        s.mtv.ptr.p_double[i] = (double)(0);
                        for(j=0; j<=m-1; j++)
                        {
                            s.mtv.ptr.p_double[i] = s.mtv.ptr.p_double[i]+a.ptr.pp_double[j][i]*s.x.ptr.p_double[j];
                        }
                    }
                }
                if( s.xupdated )
                {
                    ae_assert(pointsstored<xk.rows, "LinLSQR test: internal error", _state);
                    ae_v_move(&xk.ptr.pp_double[pointsstored][0], 1, &s.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    pointsstored = pointsstored+1;
                }
            }
            if( pointsstored<3 )
            {
                
                /*
                 * At least two iterations should be performed
                 * (our task is not that easy to solve)
                 */
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            
            /*
             * We have recorded sequence of points generated by LSQR,
             * and now we want to make a comparion against linear CG.
             *
             * Of course, we could just perform CG solution of (A'*A)*x = A'*b,
             * but it will need a CG solver, and we do not want to reference one
             * just to perform testing.
             *
             * However, we can do better - we can check that sequence of steps
             * satisfies orthogonality/conjugacy conditions, which are stated
             * as follows:
             * * (r[i]^T)*r[j]=0 for i<>j
             * * (p[i]^T)*A'*A*p[j]=0 for i<>j
             * where r[i]=(A'*A)*x[i]-A'*b is I-th residual , p[i] is I-th step.
             *
             * In order to test these criteria we generate two matrices:
             * * (PointsStored-1)*M matrix AP (matrix of A*p products)
             * * (PointsStored-1)*N matrix R  (matrix of residuals)
             * Then, we check that each matrix has orthogonal rows.
             */
            ae_matrix_set_length(&ap, pointsstored-1, m, _state);
            ae_matrix_set_length(&r, pointsstored-1, n, _state);
            ae_vector_set_length(&tmp, m, _state);
            for(k=0; k<=pointsstored-2; k++)
            {
                
                /*
                 * Calculate K-th row of AP
                 */
                for(i=0; i<=m-1; i++)
                {
                    ap.ptr.pp_double[k][i] = 0.0;
                    for(j=0; j<=n-1; j++)
                    {
                        ap.ptr.pp_double[k][i] = ap.ptr.pp_double[k][i]+a.ptr.pp_double[i][j]*(xk.ptr.pp_double[k+1][j]-xk.ptr.pp_double[k][j]);
                    }
                }
                
                /*
                 * Calculate K-th row of R
                 */
                for(i=0; i<=m-1; i++)
                {
                    v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xk.ptr.pp_double[k][0], 1, ae_v_len(0,n-1));
                    tmp.ptr.p_double[i] = v-b.ptr.p_double[i];
                }
                for(j=0; j<=n-1; j++)
                {
                    v = ae_v_dotproduct(&a.ptr.pp_double[0][j], a.stride, &tmp.ptr.p_double[0], 1, ae_v_len(0,m-1));
                    r.ptr.pp_double[k][j] = v;
                }
            }
            for(i=0; i<=pointsstored-2; i++)
            {
                for(j=0; j<=pointsstored-2; j++)
                {
                    if( i!=j )
                    {
                        v = ae_v_dotproduct(&ap.ptr.pp_double[i][0], 1, &ap.ptr.pp_double[j][0], 1, ae_v_len(0,m-1));
                        result = result||ae_fp_greater(ae_fabs(v, _state),tol);
                        v = ae_v_dotproduct(&r.ptr.pp_double[i][0], 1, &r.ptr.pp_double[j][0], 1, ae_v_len(0,n-1));
                        result = result||ae_fp_greater(ae_fabs(v, _state),tol);
                    }
                }
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function compares solution calculated by LSQR with one calculated  by
SVD solver. Following comparisons are performed:
1. either:
   1.a) residual norm |Rk| for LSQR solution is at most epsErr*|B|
   1.b) |A^T*Rk|/(|A|*|Rk|)<=EpsOrt
2. norm(LSQR_solution) is at most 1.2*norm(SVD_solution)

Test (1) verifies that LSQR found good solution, test  (2)  verifies  that
LSQR finds solution with close-to-minimum norm. We use factor as large  as
1.2 to test deviation from SVD solution because LSQR is not very  good  at
solving degenerate problems.

INPUT PARAMETERS:
    A       -   array[M,N], system matrix
    B       -   right part
    M, N    -   problem size
    LambdaV -   regularization value for the problem, >=0
    X       -   array[N], solution found by LSQR
    EpsErr  -   tolerance for |A*x-b|
    EpsOrt  -   tolerance for |A^T*Rk|/(|A|*|Rk|)

RESULT
    True, for solution which passess all the tests
*************************************************************************/
static ae_bool testlinlsqrunit_isitgoodsolution(/* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* b,
     ae_int_t m,
     ae_int_t n,
     double lambdav,
     /* Real    */ ae_vector* x,
     double epserr,
     double epsort,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix svda;
    ae_matrix u;
    ae_matrix vt;
    ae_vector w;
    ae_vector svdx;
    ae_vector tmparr;
    ae_vector r;
    ae_int_t i;
    ae_int_t j;
    ae_int_t minmn;
    ae_bool svdresult;
    double v;
    double rnorm;
    double bnorm;
    double anorm;
    double atrnorm;
    double xnorm;
    double svdxnorm;
    ae_bool clause1holds;
    ae_bool clause2holds;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&svda, 0, 0, DT_REAL, _state);
    ae_matrix_init(&u, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vt, 0, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&svdx, 0, DT_REAL, _state);
    ae_vector_init(&tmparr, 0, DT_REAL, _state);
    ae_vector_init(&r, 0, DT_REAL, _state);

    
    /*
     * Solve regularized problem with SVD solver
     */
    ae_matrix_set_length(&svda, m+n, n, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            svda.ptr.pp_double[i][j] = a->ptr.pp_double[i][j];
        }
    }
    for(i=m; i<=m+n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( i-m==j )
            {
                svda.ptr.pp_double[i][j] = lambdav;
            }
            else
            {
                svda.ptr.pp_double[i][j] = (double)(0);
            }
        }
    }
    svdresult = rmatrixsvd(&svda, m+n, n, 1, 1, 0, &w, &u, &vt, _state);
    ae_assert(svdresult, "LINLSQR: internal error in unit test (SVD failed)", _state);
    minmn = ae_minint(m, n, _state);
    ae_vector_set_length(&svdx, n, _state);
    ae_vector_set_length(&tmparr, minmn, _state);
    for(i=0; i<=minmn-1; i++)
    {
        tmparr.ptr.p_double[i] = (double)(0);
        for(j=0; j<=m-1; j++)
        {
            tmparr.ptr.p_double[i] = tmparr.ptr.p_double[i]+u.ptr.pp_double[j][i]*b->ptr.p_double[j];
        }
        if( ae_fp_less_eq(w.ptr.p_double[i],ae_sqrt(ae_machineepsilon, _state)*w.ptr.p_double[0]) )
        {
            tmparr.ptr.p_double[i] = (double)(0);
        }
        else
        {
            tmparr.ptr.p_double[i] = tmparr.ptr.p_double[i]/w.ptr.p_double[i];
        }
    }
    for(i=0; i<=n-1; i++)
    {
        svdx.ptr.p_double[i] = (double)(0);
        for(j=0; j<=minmn-1; j++)
        {
            svdx.ptr.p_double[i] = svdx.ptr.p_double[i]+vt.ptr.pp_double[j][i]*tmparr.ptr.p_double[j];
        }
    }
    
    /*
     * Calculate residual, perform checks 1.a and 1.b:
     * * first, we check 1.a
     * * in case 1.a fails we check 1.b
     */
    ae_vector_set_length(&r, m+n, _state);
    for(i=0; i<=m+n-1; i++)
    {
        v = ae_v_dotproduct(&svda.ptr.pp_double[i][0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
        r.ptr.p_double[i] = v;
        if( i<m )
        {
            r.ptr.p_double[i] = r.ptr.p_double[i]-b->ptr.p_double[i];
        }
    }
    v = ae_v_dotproduct(&r.ptr.p_double[0], 1, &r.ptr.p_double[0], 1, ae_v_len(0,m+n-1));
    rnorm = ae_sqrt(v, _state);
    v = ae_v_dotproduct(&b->ptr.p_double[0], 1, &b->ptr.p_double[0], 1, ae_v_len(0,m-1));
    bnorm = ae_sqrt(v, _state);
    if( ae_fp_less_eq(rnorm,epserr*bnorm) )
    {
        
        /*
         * 1.a is true, no further checks is needed
         */
        clause1holds = ae_true;
    }
    else
    {
        
        /*
         * 1.a is false, we have to check 1.b
         *
         * In order to do so, we calculate ||A|| and ||A^T*Rk||. We do
         * not store product of A and Rk to some array, all we need is
         * just one component of product at time, stored in V.
         * 
         */
        anorm = (double)(0);
        atrnorm = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            v = (double)(0);
            for(j=0; j<=m+n-1; j++)
            {
                v = v+svda.ptr.pp_double[j][i]*r.ptr.p_double[j];
                anorm = anorm+ae_sqr(svda.ptr.pp_double[j][i], _state);
            }
            atrnorm = atrnorm+ae_sqr(v, _state);
        }
        anorm = ae_sqrt(anorm, _state);
        atrnorm = ae_sqrt(atrnorm, _state);
        clause1holds = ae_fp_eq(anorm*rnorm,(double)(0))||ae_fp_less_eq(atrnorm/(anorm*rnorm),epsort);
    }
    
    /*
     * Check (2).
     * Here we assume that Result=True when we enter this block.
     */
    v = ae_v_dotproduct(&x->ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
    xnorm = ae_sqrt(v, _state);
    v = ae_v_dotproduct(&svdx.ptr.p_double[0], 1, &svdx.ptr.p_double[0], 1, ae_v_len(0,n-1));
    svdxnorm = ae_sqrt(v, _state);
    clause2holds = ae_fp_less_eq(xnorm,1.2*svdxnorm);
    
    /*
     * End
     */
    result = clause1holds&&clause2holds;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing preconditioned LSQR method.

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlinlsqrunit_preconditionertest(ae_state *_state)
{
    ae_frame _frame_block;
    linlsqrstate s;
    linlsqrreport rep;
    ae_matrix a;
    ae_matrix ta;
    sparsematrix sa;
    ae_vector b;
    ae_vector d;
    ae_vector xe;
    ae_vector x0;
    ae_bool bflag;
    ae_int_t i;
    ae_int_t j;
    ae_int_t n;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _linlsqrstate_init(&s, _state);
    _linlsqrreport_init(&rep, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ta, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&sa, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&xe, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);

    
    /*
     * Test 1.
     *
     * We test automatic diagonal preconditioning used by SolveSparse.
     * In order to do so we:
     * 1. generate 20*20 matrix A0 with condition number equal to 1.0E1
     * 2. generate random "exact" solution xe and right part b=A0*xe
     * 3. generate random ill-conditioned diagonal scaling matrix D with
     *    condition number equal to 1.0E50:
     * 4. transform A*x=b into badly scaled problem:
     *    A0*x0=b0
     *    (A0*D)*(inv(D)*x0)=b0
     *    finally we got new problem A*x=b with A=A0*D, b=b0, x=inv(D)*x0
     *
     * Then we solve A*x=b:
     * 1. with default preconditioner
     * 2. with explicitly activayed diagonal preconditioning
     * 3. with unit preconditioner.
     * 1st and 2nd solutions must be close to xe, 3rd solution must be very
     * far from the true one.
     */
    n = 20;
    rmatrixrndcond(n, 1.0E1, &ta, _state);
    ae_vector_set_length(&xe, n, _state);
    for(i=0; i<=n-1; i++)
    {
        xe.ptr.p_double[i] = randomnormal(_state);
    }
    ae_vector_set_length(&b, n, _state);
    for(i=0; i<=n-1; i++)
    {
        b.ptr.p_double[i] = (double)(0);
        for(j=0; j<=n-1; j++)
        {
            b.ptr.p_double[i] = b.ptr.p_double[i]+ta.ptr.pp_double[i][j]*xe.ptr.p_double[j];
        }
    }
    ae_vector_set_length(&d, n, _state);
    for(i=0; i<=n-1; i++)
    {
        d.ptr.p_double[i] = ae_pow((double)(10), 100*ae_randomreal(_state)-50, _state);
    }
    ae_matrix_set_length(&a, n, n, _state);
    sparsecreate(n, n, n*n, &sa, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a.ptr.pp_double[i][j] = ta.ptr.pp_double[i][j]*d.ptr.p_double[j];
            sparseset(&sa, i, j, a.ptr.pp_double[i][j], _state);
        }
        xe.ptr.p_double[i] = xe.ptr.p_double[i]/d.ptr.p_double[i];
    }
    sparseconverttocrs(&sa, _state);
    linlsqrcreate(n, n, &s, _state);
    linlsqrsetcond(&s, (double)(0), (double)(0), 2*n, _state);
    linlsqrsolvesparse(&s, &sa, &b, _state);
    linlsqrresults(&s, &x0, &rep, _state);
    if( rep.terminationtype<=0 )
    {
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_greater(ae_fabs(xe.ptr.p_double[i]-x0.ptr.p_double[i], _state),5.0E-2/d.ptr.p_double[i]) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    linlsqrsetprecunit(&s, _state);
    linlsqrsolvesparse(&s, &sa, &b, _state);
    linlsqrresults(&s, &x0, &rep, _state);
    if( rep.terminationtype>0 )
    {
        bflag = ae_false;
        for(i=0; i<=n-1; i++)
        {
            bflag = bflag||ae_fp_greater(ae_fabs(xe.ptr.p_double[i]-x0.ptr.p_double[i], _state),5.0E-2/d.ptr.p_double[i]);
        }
        if( !bflag )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    linlsqrsetprecdiag(&s, _state);
    linlsqrsolvesparse(&s, &sa, &b, _state);
    linlsqrresults(&s, &x0, &rep, _state);
    if( rep.terminationtype<=0 )
    {
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_greater(ae_fabs(xe.ptr.p_double[i]-x0.ptr.p_double[i], _state),5.0E-2/d.ptr.p_double[i]) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    
    /*
     *test has been passed
     */
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*$ End $*/
