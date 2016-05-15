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
#include "testminqpunit.h"


/*$ Declarations $*/
static void testminqpunit_bcqptest(ae_bool* wereerrors, ae_state *_state);
static ae_bool testminqpunit_ecqptest(ae_state *_state);
static void testminqpunit_icqptest(ae_bool* err, ae_state *_state);
static ae_bool testminqpunit_specialicqptests(ae_state *_state);
static double testminqpunit_projectedantigradnorm(ae_int_t n,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* g,
     /* Real    */ ae_vector* bndl,
     /* Real    */ ae_vector* bndu,
     ae_state *_state);
static void testminqpunit_testbcgradandfeasibility(/* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* bndl,
     /* Real    */ ae_vector* bndu,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     double eps,
     ae_bool* errorflag,
     ae_state *_state);
static void testminqpunit_setrandomalgoallmodern(minqpstate* s,
     ae_state *_state);
static void testminqpunit_setrandomalgononconvex(minqpstate* s,
     ae_state *_state);
static void testminqpunit_setrandomalgosemidefinite(minqpstate* s,
     ae_state *_state);
static void testminqpunit_setrandomalgobc(minqpstate* s, ae_state *_state);
static void testminqpunit_setrandomalgoconvexlc(minqpstate* s,
     ae_state *_state);
static void testminqpunit_setrandomalgononconvexlc(minqpstate* s,
     ae_state *_state);
static void testminqpunit_densetosparse(/* Real    */ ae_matrix* a,
     ae_int_t n,
     sparsematrix* s,
     ae_state *_state);


/*$ Body $*/


ae_bool testminqp(ae_bool silent, ae_state *_state)
{
    ae_bool simpleerrors;
    ae_bool func1errors;
    ae_bool func2errors;
    ae_bool bcqperrors;
    ae_bool ecqperrors;
    ae_bool icqperrors;
    ae_bool cholerrors;
    ae_bool quickqperrors;
    ae_bool bleicerrors;
    ae_bool waserrors;
    ae_bool result;


    bcqperrors = ae_false;
    
    /*
     * The VERY basic tests for Cholesky and BLEIC
     */
    simpleerrors = simpletest(_state);
    func1errors = functest1(_state);
    func2errors = functest2(_state);
    
    /*
     * Cholesky-specific tests
     */
    cholerrors = choleskytests(_state);
    
    /*
     * QuickQP-specific tests
     */
    quickqperrors = quickqptests(_state);
    
    /*
     * BLEIC-specific tests
     */
    bleicerrors = bleictests(_state);
    
    /*
     * Test all solvers on bound-constrained problems
     */
    testminqpunit_bcqptest(&bcqperrors, _state);
    
    /*
     * Test Cholesky and BLEIC solvers on equality-constrained problems
     */
    ecqperrors = testminqpunit_ecqptest(_state);
    
    /*
     * Test Cholesky and BLEIC solvers on inequality-constrained problems
     */
    icqperrors = ae_false;
    testminqpunit_icqptest(&icqperrors, _state);
    icqperrors = icqperrors||testminqpunit_specialicqptests(_state);
    
    /*
     * report
     */
    waserrors = (((((((simpleerrors||func1errors)||func2errors)||bcqperrors)||ecqperrors)||icqperrors)||quickqperrors)||cholerrors)||bleicerrors;
    if( !silent )
    {
        printf("TESTING MinQP\n");
        printf("SimpleTest:                               ");
        if( simpleerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("Func1Test:                                ");
        if( func1errors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("Func2Test:                                ");
        if( func2errors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("Bound constrained:                        ");
        if( bcqperrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("Equality constrained:                     ");
        if( ecqperrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("Inequality constrained:                   ");
        if( icqperrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("Cholesky solver tests:                    ");
        if( cholerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("QuickQP solver tests:                     ");
        if( quickqperrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("BLEIC solver tests:                       ");
        if( bleicerrors )
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
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testminqp(ae_bool silent, ae_state *_state)
{
    return testminqp(silent, _state);
}


/*************************************************************************
Function to test: 'MinQPCreate', 'MinQPSetQuadraticTerm', 'MinQPSetBC', 
'MinQPSetOrigin', 'MinQPSetStartingPoint', 'MinQPOptimize', 'MinQPResults'.

Test problem:
    A = diag(aii), aii>0 (random)
    b = 0
    random bounds (either no bounds, one bound, two bounds a<b, two bounds a=b)
    random start point
    dimension - from 1 to 5.
    
Returns True on success, False on failure.
*************************************************************************/
ae_bool simpletest(ae_state *_state)
{
    ae_frame _frame_block;
    minqpstate state;
    ae_int_t nexp;
    ae_int_t msn;
    ae_int_t sn;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_matrix a;
    ae_vector ub;
    ae_vector db;
    ae_vector x;
    ae_vector tx;
    double maxstb;
    ae_vector stx;
    ae_vector xori;
    ae_int_t infd;
    minqpreport rep;
    double maxn;
    double minn;
    double maxnb;
    double minnb;
    double eps;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _minqpstate_init(&state, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&ub, 0, DT_REAL, _state);
    ae_vector_init(&db, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&tx, 0, DT_REAL, _state);
    ae_vector_init(&stx, 0, DT_REAL, _state);
    ae_vector_init(&xori, 0, DT_REAL, _state);
    _minqpreport_init(&rep, _state);

    eps = 0.001;
    msn = 5;
    maxstb = (double)(10);
    nexp = 1000;
    maxn = (double)(10);
    minn = (double)(-10);
    maxnb = (double)(1000);
    minnb = (double)(-1000);
    for(sn=1; sn<=msn; sn++)
    {
        ae_vector_set_length(&tx, sn, _state);
        ae_vector_set_length(&xori, sn, _state);
        ae_vector_set_length(&stx, sn, _state);
        ae_vector_set_length(&db, sn, _state);
        ae_vector_set_length(&ub, sn, _state);
        ae_matrix_set_length(&a, sn, sn, _state);
        for(i=0; i<=nexp; i++)
        {
            
            /*
             *create diagonal matrix
             */
            for(k=0; k<=sn-1; k++)
            {
                for(j=0; j<=k; j++)
                {
                    if( j!=k )
                    {
                        a.ptr.pp_double[k][j] = (double)(0);
                    }
                    else
                    {
                        a.ptr.pp_double[k][j] = maxn*ae_randomreal(_state)+1;
                    }
                }
            }
            minqpcreate(sn, &state, _state);
            testminqpunit_setrandomalgobc(&state, _state);
            minqpsetquadraticterm(&state, &a, ae_false, _state);
            for(j=0; j<=sn-1; j++)
            {
                infd = ae_randominteger(5, _state);
                if( infd==0 )
                {
                    db.ptr.p_double[j] = _state->v_neginf;
                    ub.ptr.p_double[j] = _state->v_posinf;
                }
                else
                {
                    if( infd==1 )
                    {
                        db.ptr.p_double[j] = _state->v_neginf;
                        ub.ptr.p_double[j] = (maxnb-minnb)*ae_randomreal(_state)+minnb;
                    }
                    else
                    {
                        if( infd==2 )
                        {
                            db.ptr.p_double[j] = (maxnb-minnb)*ae_randomreal(_state)+minnb;
                            ub.ptr.p_double[j] = _state->v_posinf;
                        }
                        else
                        {
                            if( infd==3 )
                            {
                                db.ptr.p_double[j] = (maxnb-minnb)*ae_randomreal(_state)+minnb;
                                ub.ptr.p_double[j] = db.ptr.p_double[j]+maxstb*ae_randomreal(_state)+0.01;
                            }
                            else
                            {
                                db.ptr.p_double[j] = (maxnb-minnb)*ae_randomreal(_state)+minnb;
                                ub.ptr.p_double[j] = db.ptr.p_double[j];
                            }
                        }
                    }
                }
            }
            minqpsetbc(&state, &db, &ub, _state);
            
            /*
             *initialization for shifting
             *initial value for 'XORi'
             *and searching true results
             */
            for(j=0; j<=sn-1; j++)
            {
                xori.ptr.p_double[j] = (maxnb-minnb)*ae_randomreal(_state)+minnb;
                tx.ptr.p_double[j] = boundval(xori.ptr.p_double[j], db.ptr.p_double[j], ub.ptr.p_double[j], _state);
            }
            minqpsetorigin(&state, &xori, _state);
            
            /*
             *initialization for starting point
             */
            for(j=0; j<=sn-1; j++)
            {
                stx.ptr.p_double[j] = (maxnb-minnb)*ae_randomreal(_state)+minnb;
            }
            minqpsetstartingpoint(&state, &stx, _state);
            
            /*
             *optimize and get result
             */
            minqpoptimize(&state, _state);
            minqpresults(&state, &x, &rep, _state);
            for(j=0; j<=sn-1; j++)
            {
                if( ae_fp_greater(ae_fabs(tx.ptr.p_double[j]-x.ptr.p_double[j], _state),eps)||(ae_fp_less(x.ptr.p_double[j],db.ptr.p_double[j])||ae_fp_greater(x.ptr.p_double[j],ub.ptr.p_double[j])) )
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
Function to test: 'MinQPCreate', 'MinQPSetLinearTerm', 'MinQPSetQuadraticTerm',
'MinQPSetOrigin', 'MinQPSetStartingPoint', 'MinQPOptimize', 'MinQPResults'.

Test problem:
    A = positive-definite matrix, obtained by 'SPDMatrixRndCond' function
    b <> 0
    without bounds
    random start point
    dimension - from 1 to 5.
*************************************************************************/
ae_bool functest1(ae_state *_state)
{
    ae_frame _frame_block;
    minqpstate state;
    ae_int_t nexp;
    ae_int_t msn;
    ae_int_t sn;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_matrix a;
    ae_vector ub;
    ae_vector db;
    ae_vector x;
    ae_vector tx;
    double maxstb;
    ae_vector stx;
    ae_vector xori;
    ae_vector xoric;
    minqpreport rep;
    double maxn;
    double minn;
    double maxnb;
    double minnb;
    double eps;
    ae_vector b;
    ae_int_t c2;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _minqpstate_init(&state, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&ub, 0, DT_REAL, _state);
    ae_vector_init(&db, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&tx, 0, DT_REAL, _state);
    ae_vector_init(&stx, 0, DT_REAL, _state);
    ae_vector_init(&xori, 0, DT_REAL, _state);
    ae_vector_init(&xoric, 0, DT_REAL, _state);
    _minqpreport_init(&rep, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);

    eps = 0.001;
    msn = 5;
    c2 = 1000;
    maxstb = (double)(10);
    nexp = 1000;
    maxn = (double)(10);
    minn = (double)(-10);
    maxnb = (double)(1000);
    minnb = (double)(-1000);
    for(sn=1; sn<=msn; sn++)
    {
        ae_vector_set_length(&b, sn, _state);
        ae_vector_set_length(&tx, sn, _state);
        ae_vector_set_length(&xori, sn, _state);
        ae_vector_set_length(&xoric, sn, _state);
        ae_vector_set_length(&stx, sn, _state);
        for(i=0; i<=nexp; i++)
        {
            
            /*
             *create simmetric matrix 'A'
             */
            spdmatrixrndcond(sn, ae_exp(ae_randomreal(_state)*ae_log((double)(c2), _state), _state), &a, _state);
            minqpcreate(sn, &state, _state);
            testminqpunit_setrandomalgobc(&state, _state);
            minqpsetquadraticterm(&state, &a, ae_false, _state);
            for(j=0; j<=sn-1; j++)
            {
                xoric.ptr.p_double[j] = 2*ae_randomreal(_state)-1;
            }
            
            /*
             *create linear part
             */
            for(j=0; j<=sn-1; j++)
            {
                b.ptr.p_double[j] = (double)(0);
                for(k=0; k<=sn-1; k++)
                {
                    b.ptr.p_double[j] = b.ptr.p_double[j]-xoric.ptr.p_double[k]*a.ptr.pp_double[k][j];
                }
            }
            minqpsetlinearterm(&state, &b, _state);
            
            /*
             *initialization for shifting
             *initial value for 'XORi'
             *and searching true results
             */
            for(j=0; j<=sn-1; j++)
            {
                xori.ptr.p_double[j] = 2*ae_randomreal(_state)-1;
                tx.ptr.p_double[j] = xori.ptr.p_double[j]+xoric.ptr.p_double[j];
            }
            minqpsetorigin(&state, &xori, _state);
            
            /*
             *initialization for starting point
             */
            for(j=0; j<=sn-1; j++)
            {
                stx.ptr.p_double[j] = 2*ae_randomreal(_state)-1;
            }
            minqpsetstartingpoint(&state, &stx, _state);
            
            /*
             *optimize and get result
             */
            minqpoptimize(&state, _state);
            minqpresults(&state, &x, &rep, _state);
            for(j=0; j<=sn-1; j++)
            {
                if( ae_fp_greater(ae_fabs(tx.ptr.p_double[j]-x.ptr.p_double[j], _state),eps) )
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
Function to test: 'MinQPCreate', 'MinQPSetLinearTerm', 'MinQPSetQuadraticTerm',
'MinQPSetBC', 'MinQPSetOrigin', 'MinQPSetStartingPoint', 'MinQPOptimize', 
'MinQPResults'.

Test problem:
    A = positive-definite matrix, obtained by 'SPDMatrixRndCond' function
    b <> 0
    boundary constraints
    random start point
    dimension - from 1 to 5.
*************************************************************************/
ae_bool functest2(ae_state *_state)
{
    ae_frame _frame_block;
    minqpstate state;
    ae_int_t nexp;
    ae_int_t msn;
    ae_int_t sn;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_matrix a;
    ae_vector ub;
    ae_vector db;
    ae_vector x;
    ae_vector tmpx;
    double maxstb;
    ae_vector stx;
    ae_vector xori;
    ae_vector xoric;
    ae_int_t infd;
    minqpreport rep;
    double maxn;
    double minn;
    double maxnb;
    double minnb;
    double eps;
    ae_vector b;
    ae_vector g;
    ae_vector c;
    ae_vector y0;
    ae_vector y1;
    ae_int_t c2;
    double anti;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _minqpstate_init(&state, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&ub, 0, DT_REAL, _state);
    ae_vector_init(&db, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&tmpx, 0, DT_REAL, _state);
    ae_vector_init(&stx, 0, DT_REAL, _state);
    ae_vector_init(&xori, 0, DT_REAL, _state);
    ae_vector_init(&xoric, 0, DT_REAL, _state);
    _minqpreport_init(&rep, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&g, 0, DT_REAL, _state);
    ae_vector_init(&c, 0, DT_REAL, _state);
    ae_vector_init(&y0, 0, DT_REAL, _state);
    ae_vector_init(&y1, 0, DT_REAL, _state);

    eps = 0.001;
    msn = 5;
    c2 = 1000;
    maxstb = (double)(10);
    nexp = 1000;
    maxn = (double)(10);
    minn = (double)(-10);
    maxnb = (double)(1000);
    minnb = (double)(-1000);
    for(sn=1; sn<=msn; sn++)
    {
        ae_vector_set_length(&tmpx, sn, _state);
        ae_vector_set_length(&b, sn, _state);
        ae_vector_set_length(&c, sn, _state);
        ae_vector_set_length(&g, sn, _state);
        ae_vector_set_length(&xori, sn, _state);
        ae_vector_set_length(&xoric, sn, _state);
        ae_vector_set_length(&stx, sn, _state);
        ae_vector_set_length(&db, sn, _state);
        ae_vector_set_length(&ub, sn, _state);
        ae_vector_set_length(&y0, sn, _state);
        ae_vector_set_length(&y1, sn, _state);
        for(i=0; i<=nexp; i++)
        {
            
            /*
             *create simmetric matrix 'A'
             */
            spdmatrixrndcond(sn, ae_exp(ae_randomreal(_state)*ae_log((double)(c2), _state), _state), &a, _state);
            minqpcreate(sn, &state, _state);
            testminqpunit_setrandomalgobc(&state, _state);
            minqpsetquadraticterm(&state, &a, ae_false, _state);
            for(j=0; j<=sn-1; j++)
            {
                xoric.ptr.p_double[j] = (maxnb-minnb)*ae_randomreal(_state)+minnb;
            }
            
            /*
             *create linear part
             */
            for(j=0; j<=sn-1; j++)
            {
                b.ptr.p_double[j] = (double)(0);
                for(k=0; k<=sn-1; k++)
                {
                    b.ptr.p_double[j] = b.ptr.p_double[j]-xoric.ptr.p_double[k]*a.ptr.pp_double[k][j];
                }
            }
            minqpsetlinearterm(&state, &b, _state);
            for(j=0; j<=sn-1; j++)
            {
                infd = ae_randominteger(4, _state);
                if( infd==0 )
                {
                    db.ptr.p_double[j] = _state->v_neginf;
                    ub.ptr.p_double[j] = _state->v_posinf;
                }
                else
                {
                    if( infd==1 )
                    {
                        db.ptr.p_double[j] = _state->v_neginf;
                        ub.ptr.p_double[j] = (maxnb-minnb)*ae_randomreal(_state)+minnb;
                    }
                    else
                    {
                        if( infd==2 )
                        {
                            db.ptr.p_double[j] = (maxnb-minnb)*ae_randomreal(_state)+minnb;
                            ub.ptr.p_double[j] = _state->v_posinf;
                        }
                        else
                        {
                            db.ptr.p_double[j] = (maxnb-minnb)*ae_randomreal(_state)+minnb;
                            ub.ptr.p_double[j] = db.ptr.p_double[j]+maxstb*ae_randomreal(_state)+0.01;
                        }
                    }
                }
            }
            minqpsetbc(&state, &db, &ub, _state);
            
            /*
             *initialization for shifting
             *initial value for 'XORi'
             *and searching true results
             */
            for(j=0; j<=sn-1; j++)
            {
                xori.ptr.p_double[j] = (maxnb-minnb)*ae_randomreal(_state)+minnb;
            }
            minqpsetorigin(&state, &xori, _state);
            for(j=0; j<=sn-1; j++)
            {
                c.ptr.p_double[j] = (double)(0);
                for(k=0; k<=sn-1; k++)
                {
                    c.ptr.p_double[j] = c.ptr.p_double[j]-xori.ptr.p_double[k]*a.ptr.pp_double[k][j];
                }
            }
            
            /*
             *initialization for starting point
             */
            for(j=0; j<=sn-1; j++)
            {
                stx.ptr.p_double[j] = (maxnb-minnb)*ae_randomreal(_state)+minnb;
            }
            minqpsetstartingpoint(&state, &stx, _state);
            
            /*
             *optimize and get result
             */
            minqpoptimize(&state, _state);
            minqpresults(&state, &x, &rep, _state);
            rmatrixmv(sn, sn, &a, 0, 0, 0, &x, 0, &y0, 0, _state);
            for(j=0; j<=sn-1; j++)
            {
                g.ptr.p_double[j] = y0.ptr.p_double[j]+c.ptr.p_double[j]+b.ptr.p_double[j];
            }
            anti = testminqpunit_projectedantigradnorm(sn, &x, &g, &db, &ub, _state);
            for(j=0; j<=sn-1; j++)
            {
                if( ae_fp_greater(ae_fabs(anti, _state),eps) )
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
ConsoleTest.
*************************************************************************/
ae_bool consoletest(ae_state *_state)
{
    ae_frame _frame_block;
    minqpstate state;
    ae_int_t nexp;
    ae_int_t msn;
    ae_int_t sn;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_matrix a;
    ae_vector ub;
    ae_vector db;
    ae_vector x;
    double maxstb;
    ae_vector stx;
    ae_vector xori;
    ae_vector xoric;
    minqpreport rep;
    double maxn;
    double minn;
    double maxnb;
    double minnb;
    double eps;
    ae_vector b;
    ae_vector g;
    ae_vector y0;
    ae_vector y1;
    ae_int_t c2;
    double c;
    double anti;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _minqpstate_init(&state, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&ub, 0, DT_REAL, _state);
    ae_vector_init(&db, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&stx, 0, DT_REAL, _state);
    ae_vector_init(&xori, 0, DT_REAL, _state);
    ae_vector_init(&xoric, 0, DT_REAL, _state);
    _minqpreport_init(&rep, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&g, 0, DT_REAL, _state);
    ae_vector_init(&y0, 0, DT_REAL, _state);
    ae_vector_init(&y1, 0, DT_REAL, _state);

    eps = 0.001;
    msn = 2;
    c2 = 1000;
    maxstb = (double)(10);
    nexp = 0;
    maxn = (double)(10);
    minn = (double)(-10);
    maxnb = (double)(1000);
    minnb = (double)(-1000);
    for(sn=2; sn<=msn; sn++)
    {
        ae_vector_set_length(&b, sn, _state);
        ae_vector_set_length(&g, sn, _state);
        ae_vector_set_length(&xori, sn, _state);
        ae_vector_set_length(&xoric, sn, _state);
        ae_vector_set_length(&stx, sn, _state);
        ae_vector_set_length(&db, sn, _state);
        ae_vector_set_length(&ub, sn, _state);
        ae_vector_set_length(&y0, sn, _state);
        ae_vector_set_length(&y1, sn, _state);
        for(i=0; i<=nexp; i++)
        {
            
            /*
             *create simmetric matrix 'A'
             */
            ae_matrix_set_length(&a, sn, sn, _state);
            for(j=0; j<=sn-1; j++)
            {
                for(k=0; k<=sn-1; k++)
                {
                    if( j==k )
                    {
                        a.ptr.pp_double[j][k] = (double)(1);
                    }
                    else
                    {
                        a.ptr.pp_double[j][k] = (double)(0);
                    }
                    printf("%0.5f ",
                        (double)(a.ptr.pp_double[j][k]));
                }
                printf("\n");
            }
            minqpcreate(sn, &state, _state);
            testminqpunit_setrandomalgobc(&state, _state);
            minqpsetquadraticterm(&state, &a, ae_false, _state);
            for(j=0; j<=sn-1; j++)
            {
                xoric.ptr.p_double[j] = (double)(1);
                printf("XoriC=%0.5f \n",
                    (double)(xoric.ptr.p_double[j]));
            }
            
            /*
             *create linear part
             */
            for(j=0; j<=sn-1; j++)
            {
                b.ptr.p_double[j] = (double)(0);
                for(k=0; k<=sn-1; k++)
                {
                    b.ptr.p_double[j] = b.ptr.p_double[j]-xoric.ptr.p_double[k]*a.ptr.pp_double[k][j];
                }
                printf("B[%0d]=%0.5f\n",
                    (int)(j),
                    (double)(b.ptr.p_double[j]));
            }
            minqpsetlinearterm(&state, &b, _state);
            for(j=0; j<=sn-1; j++)
            {
                db.ptr.p_double[j] = (double)(10);
                ub.ptr.p_double[j] = (double)(20);
            }
            minqpsetbc(&state, &db, &ub, _state);
            
            /*
             *initialization for shifting
             *initial value for 'XORi'
             *and searching true results
             */
            for(j=0; j<=sn-1; j++)
            {
                xori.ptr.p_double[j] = (double)(1);
            }
            minqpsetorigin(&state, &xori, _state);
            
            /*
             *optimize and get result
             */
            minqpoptimize(&state, _state);
            minqpresults(&state, &x, &rep, _state);
            rmatrixmv(sn, sn, &a, 0, 0, 0, &x, 0, &y0, 0, _state);
            rmatrixmv(sn, sn, &a, 0, 0, 0, &x, 0, &y1, 0, _state);
            for(j=0; j<=sn-1; j++)
            {
                c = (double)(0);
                for(k=0; k<=sn-1; k++)
                {
                    c = c-xori.ptr.p_double[k]*a.ptr.pp_double[k][j];
                }
                g.ptr.p_double[j] = b.ptr.p_double[j]+c+y0.ptr.p_double[j]+y1.ptr.p_double[j];
            }
            anti = testminqpunit_projectedantigradnorm(sn, &x, &b, &db, &ub, _state);
            printf("SN=%0d\n",
                (int)(sn));
            printf("NEXP=%0d\n",
                (int)(i));
            printf("TermType=%0d\n",
                (int)(rep.terminationtype));
            for(j=0; j<=sn-1; j++)
            {
                printf("X[%0d]=%0.5f;\n",
                    (int)(j),
                    (double)(x.ptr.p_double[j]));
                printf("DB[%0d]=%0.5f; UB[%0d]=%0.5f\n",
                    (int)(j),
                    (double)(db.ptr.p_double[j]),
                    (int)(j),
                    (double)(ub.ptr.p_double[j]));
                printf("XORi[%0d]=%0.5f; XORiC[%0d]=%0.5f;\n",
                    (int)(j),
                    (double)(xori.ptr.p_double[j]),
                    (int)(j),
                    (double)(xoric.ptr.p_double[j]));
                printf("Anti[%0d]=%0.5f;\n",
                    (int)(j),
                    (double)(anti));
                if( ae_fp_greater(ae_fabs(anti, _state),eps) )
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
This function performs tests specific for Cholesky solver
    
Returns True on success, False on failure.
*************************************************************************/
ae_bool choleskytests(ae_state *_state)
{
    ae_frame _frame_block;
    minqpstate state;
    minqpreport rep;
    sparsematrix sa;
    ae_matrix a;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_vector bndl;
    ae_vector bndu;
    ae_vector x;
    ae_vector xend;
    ae_vector xend0;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _minqpstate_init(&state, _state);
    _minqpreport_init(&rep, _state);
    _sparsematrix_init(&sa, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&xend, 0, DT_REAL, _state);
    ae_vector_init(&xend0, 0, DT_REAL, _state);

    result = ae_false;
    
    /*
     * TEST: Cholesky solver should return -5 on sparse matrices.
     */
    n = 5;
    sparsecreate(n, n, 0, &sa, _state);
    for(i=0; i<=n-1; i++)
    {
        sparseset(&sa, i, i, 1.0, _state);
    }
    minqpcreate(n, &state, _state);
    minqpsetalgocholesky(&state, _state);
    minqpsetquadratictermsparse(&state, &sa, ae_true, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend, &rep, _state);
    seterrorflag(&result, rep.terminationtype!=-5, _state);
    
    /*
     * TEST: default solver is Cholesky one.
     *
     * It is tested by checking that default solver returns -5 on sparse matrices.
     */
    n = 5;
    sparsecreate(n, n, 0, &sa, _state);
    for(i=0; i<=n-1; i++)
    {
        sparseset(&sa, i, i, 1.0, _state);
    }
    minqpcreate(n, &state, _state);
    minqpsetquadratictermsparse(&state, &sa, ae_true, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend, &rep, _state);
    seterrorflag(&result, rep.terminationtype!=-5, _state);
    
    /*
     * Test CQP solver on non-convex problems,
     * which are bounded from below on the feasible set:
     *
     *     min -||x||^2 s.t. x[i] in [-1,+1]
     *
     * We test ability of the solver to detect such problems
     * and report failure.
     */
    n = 20;
    ae_matrix_set_length(&a, n, n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a.ptr.pp_double[i][j] = (double)(0);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        a.ptr.pp_double[i][i] = -1.0;
    }
    ae_vector_set_length(&bndl, n, _state);
    ae_vector_set_length(&bndu, n, _state);
    ae_vector_set_length(&x, n, _state);
    for(i=0; i<=n-1; i++)
    {
        bndl.ptr.p_double[i] = (double)(-1);
        bndu.ptr.p_double[i] = (double)(1);
        x.ptr.p_double[i] = ae_randomreal(_state)-0.5;
    }
    minqpcreate(n, &state, _state);
    minqpsetalgocholesky(&state, _state);
    minqpsetquadraticterm(&state, &a, ae_true, _state);
    minqpsetbc(&state, &bndl, &bndu, _state);
    minqpsetstartingpoint(&state, &x, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend0, &rep, _state);
    seterrorflag(&result, rep.terminationtype!=-5, _state);
    
    /*
     * Test CQP solver on non-convex problems,
     * which are unbounded from below:
     *
     *     min -||x||^2
     *
     * We test ability of the solver to detect such problems
     * and report failure.
     */
    n = 20;
    ae_matrix_set_length(&a, n, n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a.ptr.pp_double[i][j] = (double)(0);
        }
    }
    for(i=0; i<=n-1; i++)
    {
        a.ptr.pp_double[i][i] = -1.0;
    }
    ae_vector_set_length(&x, n, _state);
    for(i=0; i<=n-1; i++)
    {
        x.ptr.p_double[i] = ae_randomreal(_state)-0.5;
    }
    minqpcreate(n, &state, _state);
    minqpsetalgocholesky(&state, _state);
    minqpsetquadraticterm(&state, &a, ae_true, _state);
    minqpsetstartingpoint(&state, &x, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend0, &rep, _state);
    seterrorflag(&result, rep.terminationtype!=-5, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function performs tests specific for QuickQP solver
    
Returns True on failure.
*************************************************************************/
ae_bool quickqptests(ae_state *_state)
{
    ae_frame _frame_block;
    minqpstate state;
    minqpreport rep;
    ae_int_t n;
    ae_int_t pass;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    double g;
    double gnorm;
    ae_bool flag;
    ae_int_t origintype;
    ae_int_t scaletype;
    ae_bool isupper;
    ae_bool issparse;
    ae_int_t itscnt;
    ae_vector nlist;
    ae_int_t nidx;
    ae_matrix a;
    ae_matrix za;
    ae_matrix fulla;
    ae_matrix halfa;
    ae_matrix c;
    sparsematrix sa;
    ae_vector ct;
    ae_vector b;
    ae_vector zb;
    ae_vector bndl;
    ae_vector bndu;
    ae_vector x0;
    ae_vector x1;
    ae_vector xend0;
    ae_vector xend1;
    ae_vector xori;
    ae_vector xz;
    ae_vector s;
    double eps;
    hqrndstate rs;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _minqpstate_init(&state, _state);
    _minqpreport_init(&rep, _state);
    ae_vector_init(&nlist, 0, DT_INT, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&za, 0, 0, DT_REAL, _state);
    ae_matrix_init(&fulla, 0, 0, DT_REAL, _state);
    ae_matrix_init(&halfa, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&sa, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&zb, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&xend0, 0, DT_REAL, _state);
    ae_vector_init(&xend1, 0, DT_REAL, _state);
    ae_vector_init(&xori, 0, DT_REAL, _state);
    ae_vector_init(&xz, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    _hqrndstate_init(&rs, _state);

    result = ae_false;
    hqrndrandomize(&rs, _state);
    
    /*
     * Convex test:
     * * N dimensions
     * * random number (0..N) of random boundary constraints
     * * positive-definite A
     * * algorithm randomly choose dense or sparse A, and for
     *   sparse matrix it randomly choose format.
     * * random B with normal entries
     * * initial point is random, feasible
     * * random origin (zero or non-zero) and scale (unit or
     *   non-unit) are generated
     */
    eps = 1.0E-5;
    for(n=1; n<=10; n++)
    {
        for(pass=1; pass<=10; pass++)
        {
            
            /*
             * Generate problem
             */
            origintype = hqrnduniformi(&rs, 2, _state);
            scaletype = hqrnduniformi(&rs, 2, _state);
            isupper = ae_fp_less(hqrnduniformr(&rs, _state),0.5);
            issparse = ae_fp_less(hqrnduniformr(&rs, _state),0.5);
            spdmatrixrndcond(n, 1.0E3, &fulla, _state);
            ae_matrix_set_length(&halfa, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( (j>=i&&isupper)||(j<=i&&!isupper) )
                    {
                        halfa.ptr.pp_double[i][j] = fulla.ptr.pp_double[i][j];
                    }
                    else
                    {
                        halfa.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                    }
                }
            }
            testminqpunit_densetosparse(&halfa, n, &sa, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&xori, n, _state);
            ae_vector_set_length(&s, n, _state);
            for(i=0; i<=n-1; i++)
            {
                b.ptr.p_double[i] = hqrndnormal(&rs, _state);
                bndl.ptr.p_double[i] = _state->v_neginf;
                bndu.ptr.p_double[i] = _state->v_posinf;
                x0.ptr.p_double[i] = hqrndnormal(&rs, _state);
                if( origintype==0 )
                {
                    xori.ptr.p_double[i] = (double)(0);
                }
                else
                {
                    xori.ptr.p_double[i] = hqrndnormal(&rs, _state);
                }
                if( scaletype==0 )
                {
                    s.ptr.p_double[i] = (double)(1);
                }
                else
                {
                    s.ptr.p_double[i] = ae_exp(hqrndnormal(&rs, _state), _state);
                }
                j = hqrnduniformi(&rs, 5, _state);
                if( j==0 )
                {
                    bndl.ptr.p_double[i] = (double)(0);
                    x0.ptr.p_double[i] = ae_fabs(x0.ptr.p_double[i], _state);
                }
                if( j==1 )
                {
                    bndu.ptr.p_double[i] = (double)(0);
                    x0.ptr.p_double[i] = -ae_fabs(x0.ptr.p_double[i], _state);
                }
                if( j==2 )
                {
                    bndl.ptr.p_double[i] = hqrndnormal(&rs, _state);
                    bndu.ptr.p_double[i] = bndl.ptr.p_double[i];
                    x0.ptr.p_double[i] = bndl.ptr.p_double[i];
                }
                if( j==3 )
                {
                    bndl.ptr.p_double[i] = -0.1;
                    bndu.ptr.p_double[i] = 0.1;
                    x0.ptr.p_double[i] = 0.2*hqrnduniformr(&rs, _state)-0.1;
                }
            }
            
            /*
             * Solve problem
             */
            minqpcreate(n, &state, _state);
            minqpsetalgoquickqp(&state, 0.0, 0.0, 0.0, 0, ae_fp_greater(hqrnduniformr(&rs, _state),0.5), _state);
            minqpsetlinearterm(&state, &b, _state);
            if( issparse )
            {
                minqpsetquadratictermsparse(&state, &sa, isupper, _state);
            }
            else
            {
                minqpsetquadraticterm(&state, &halfa, isupper, _state);
            }
            if( origintype!=0 )
            {
                minqpsetorigin(&state, &xori, _state);
            }
            if( scaletype!=0 )
            {
                minqpsetscale(&state, &s, _state);
            }
            minqpsetbc(&state, &bndl, &bndu, _state);
            minqpoptimize(&state, _state);
            minqpresults(&state, &x1, &rep, _state);
            seterrorflag(&result, rep.terminationtype<=0, _state);
            if( rep.terminationtype<=0 )
            {
                ae_frame_leave(_state);
                return result;
            }
            
            /*
             * Test - calculate constrained gradient at solution,
             * check its norm.
             */
            gnorm = 0.0;
            for(i=0; i<=n-1; i++)
            {
                g = b.ptr.p_double[i];
                for(j=0; j<=n-1; j++)
                {
                    g = g+fulla.ptr.pp_double[i][j]*(x1.ptr.p_double[j]-xori.ptr.p_double[j]);
                }
                if( ae_fp_eq(x1.ptr.p_double[i],bndl.ptr.p_double[i])&&ae_fp_greater(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                if( ae_fp_eq(x1.ptr.p_double[i],bndu.ptr.p_double[i])&&ae_fp_less(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                gnorm = gnorm+ae_sqr(g, _state);
                seterrorflag(&result, ae_fp_less(x1.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
                seterrorflag(&result, ae_fp_greater(x1.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
            }
            gnorm = ae_sqrt(gnorm, _state);
            seterrorflag(&result, ae_fp_greater(gnorm,eps), _state);
        }
    }
    
    /*
     * Strongly non-convex test:
     * * N dimensions, N>=2
     * * box constraints, x[i] in [-1,+1]
     * * A = A0-0.5*I, where A0 is SPD with unit norm and smallest
     *   singular value equal to 1.0E-3, I is identity matrix
     * * random B with normal entries
     * * initial point is random, feasible
     *
     * We perform two tests:
     * * unconstrained problem must be recognized as unbounded
     * * constrained problem can be successfully solved
     *
     * NOTE: it is important to have N>=2, because formula for A
     *       can be applied only to matrix with at least two
     *       singular values
     */
    eps = 1.0E-5;
    for(n=2; n<=10; n++)
    {
        for(pass=1; pass<=10; pass++)
        {
            
            /*
             * Generate problem
             */
            spdmatrixrndcond(n, 1.0E3, &fulla, _state);
            for(i=0; i<=n-1; i++)
            {
                fulla.ptr.pp_double[i][i] = fulla.ptr.pp_double[i][i]-0.5;
            }
            isupper = ae_fp_less(hqrnduniformr(&rs, _state),0.5);
            ae_matrix_set_length(&halfa, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( (j>=i&&isupper)||(j<=i&&!isupper) )
                    {
                        halfa.ptr.pp_double[i][j] = fulla.ptr.pp_double[i][j];
                    }
                    else
                    {
                        halfa.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                    }
                }
            }
            testminqpunit_densetosparse(&halfa, n, &sa, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&x0, n, _state);
            for(i=0; i<=n-1; i++)
            {
                b.ptr.p_double[i] = hqrndnormal(&rs, _state);
                bndl.ptr.p_double[i] = (double)(-1);
                bndu.ptr.p_double[i] = (double)(1);
                x0.ptr.p_double[i] = 2*hqrnduniformr(&rs, _state)-1;
            }
            
            /*
             * Solve problem:
             * * without constraints we expect failure
             * * with constraints algorithm must succeed
             */
            minqpcreate(n, &state, _state);
            minqpsetalgoquickqp(&state, 0.0, 0.0, 0.0, 0, ae_fp_greater(hqrnduniformr(&rs, _state),0.5), _state);
            minqpsetlinearterm(&state, &b, _state);
            if( ae_fp_greater(hqrndnormal(&rs, _state),(double)(0)) )
            {
                minqpsetquadraticterm(&state, &halfa, isupper, _state);
            }
            else
            {
                minqpsetquadratictermsparse(&state, &sa, isupper, _state);
            }
            minqpoptimize(&state, _state);
            minqpresults(&state, &x1, &rep, _state);
            seterrorflag(&result, rep.terminationtype!=-4, _state);
            minqpsetbc(&state, &bndl, &bndu, _state);
            minqpoptimize(&state, _state);
            minqpresults(&state, &x1, &rep, _state);
            seterrorflag(&result, rep.terminationtype<=0, _state);
            if( rep.terminationtype<=0 )
            {
                ae_frame_leave(_state);
                return result;
            }
            
            /*
             * Test - calculate constrained gradient at solution,
             * check its norm.
             */
            gnorm = 0.0;
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&fulla.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                g = v+b.ptr.p_double[i];
                if( ae_fp_eq(x1.ptr.p_double[i],bndl.ptr.p_double[i])&&ae_fp_greater(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                if( ae_fp_eq(x1.ptr.p_double[i],bndu.ptr.p_double[i])&&ae_fp_less(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                gnorm = gnorm+ae_sqr(g, _state);
                seterrorflag(&result, ae_fp_less(x1.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
                seterrorflag(&result, ae_fp_greater(x1.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
            }
            gnorm = ae_sqrt(gnorm, _state);
            seterrorflag(&result, ae_fp_greater(gnorm,eps), _state);
        }
    }
    
    /*
     * Basic semi-definite test:
     * * N dimensions, N>=2
     * * box constraints, x[i] in [-1,+1]
     *       [ 1 1 ... 1 1 ]
     * * A = [ ... ... ... ]
     *       [ 1 1 ... 1 1 ]
     * * random B with normal entries
     * * initial point is random, feasible
     *
     * We perform two tests:
     * * unconstrained problem must be recognized as unbounded
     * * constrained problem must be recognized as bounded and
     *   successfully solved
     *
     * Both problems require subtle programming when we work
     * with semidefinite QP.
     *
     * NOTE: unlike BLEIC-QP algorthm, QQP may detect unboundedness
     *       of the problem when started from any x0, with any b.
     *       BLEIC-based solver requires carefully chosen x0 and b
     *       to find direction of zero curvature, but this solver
     *       can find it from any point.
     */
    ae_vector_set_length(&nlist, 12, _state);
    nlist.ptr.p_int[0] = 2;
    nlist.ptr.p_int[1] = 3;
    nlist.ptr.p_int[2] = 4;
    nlist.ptr.p_int[3] = 5;
    nlist.ptr.p_int[4] = 6;
    nlist.ptr.p_int[5] = 7;
    nlist.ptr.p_int[6] = 8;
    nlist.ptr.p_int[7] = 9;
    nlist.ptr.p_int[8] = 10;
    nlist.ptr.p_int[9] = 20;
    nlist.ptr.p_int[10] = 40;
    nlist.ptr.p_int[11] = 80;
    eps = 1.0E-5;
    for(nidx=0; nidx<=nlist.cnt-1; nidx++)
    {
        for(pass=1; pass<=10; pass++)
        {
            
            /*
             * Generate problem
             */
            n = nlist.ptr.p_int[nidx];
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&x0, n, _state);
            for(i=0; i<=n-1; i++)
            {
                do
                {
                    b.ptr.p_double[i] = hqrndnormal(&rs, _state);
                }
                while(ae_fp_eq(b.ptr.p_double[i],(double)(0)));
                bndl.ptr.p_double[i] = (double)(-1);
                bndu.ptr.p_double[i] = (double)(1);
                x0.ptr.p_double[i] = 2*hqrnduniformr(&rs, _state)-1;
            }
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = 1.0;
                }
            }
            testminqpunit_densetosparse(&a, n, &sa, _state);
            
            /*
             * Solve problem:
             * * without constraints we expect failure
             * * with constraints algorithm must succeed
             */
            minqpcreate(n, &state, _state);
            minqpsetalgoquickqp(&state, 0.0, 0.0, 0.0, 0, ae_fp_greater(hqrnduniformr(&rs, _state),0.5), _state);
            minqpsetlinearterm(&state, &b, _state);
            if( ae_fp_greater(hqrndnormal(&rs, _state),(double)(0)) )
            {
                minqpsetquadraticterm(&state, &a, ae_true, _state);
            }
            else
            {
                minqpsetquadratictermsparse(&state, &sa, ae_true, _state);
            }
            minqpoptimize(&state, _state);
            minqpresults(&state, &x1, &rep, _state);
            seterrorflag(&result, rep.terminationtype!=-4, _state);
            minqpsetbc(&state, &bndl, &bndu, _state);
            minqpoptimize(&state, _state);
            minqpresults(&state, &x1, &rep, _state);
            seterrorflag(&result, rep.terminationtype<=0, _state);
            if( rep.terminationtype<=0 )
            {
                ae_frame_leave(_state);
                return result;
            }
            
            /*
             * Test - calculate constrained gradient at solution,
             * check its norm.
             */
            gnorm = 0.0;
            for(i=0; i<=n-1; i++)
            {
                g = b.ptr.p_double[i];
                for(j=0; j<=n-1; j++)
                {
                    g = g+a.ptr.pp_double[i][j]*x1.ptr.p_double[j];
                }
                if( ae_fp_eq(x1.ptr.p_double[i],bndl.ptr.p_double[i])&&ae_fp_greater(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                if( ae_fp_eq(x1.ptr.p_double[i],bndu.ptr.p_double[i])&&ae_fp_less(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                gnorm = gnorm+ae_sqr(g, _state);
                seterrorflag(&result, ae_fp_less(x1.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
                seterrorflag(&result, ae_fp_greater(x1.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
            }
            gnorm = ae_sqrt(gnorm, _state);
            seterrorflag(&result, ae_fp_greater(gnorm,eps), _state);
        }
    }
    
    /*
     * Linear (zero-quadratic) test:
     * * N dimensions, N>=1
     * * box constraints, x[i] in [-1,+1]
     * * A = 0
     * * random B with normal entries
     * * initial point is random, feasible
     *
     * We perform two tests:
     * * unconstrained problem must be recognized as unbounded
     * * constrained problem can be successfully solved
     *
     * NOTE: we may explicitly set zero A, or assume that by
     *       default it is zero. During test we will try both
     *       ways.
     */
    eps = 1.0E-5;
    for(n=1; n<=10; n++)
    {
        for(pass=1; pass<=10; pass++)
        {
            
            /*
             * Generate problem
             */
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&x0, n, _state);
            for(i=0; i<=n-1; i++)
            {
                do
                {
                    b.ptr.p_double[i] = hqrndnormal(&rs, _state);
                }
                while(ae_fp_eq(b.ptr.p_double[i],(double)(0)));
                bndl.ptr.p_double[i] = (double)(-1);
                bndu.ptr.p_double[i] = (double)(1);
                x0.ptr.p_double[i] = 2*hqrnduniformr(&rs, _state)-1;
            }
            
            /*
             * Solve problem:
             * * without constraints we expect failure
             * * with constraints algorithm must succeed
             */
            minqpcreate(n, &state, _state);
            minqpsetalgoquickqp(&state, 0.0, 0.0, 0.0, 0, ae_fp_greater(hqrnduniformr(&rs, _state),0.5), _state);
            minqpsetlinearterm(&state, &b, _state);
            if( ae_fp_greater(hqrndnormal(&rs, _state),(double)(0)) )
            {
                ae_matrix_set_length(&a, n, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a.ptr.pp_double[i][j] = (double)(0);
                    }
                }
                minqpsetquadraticterm(&state, &a, ae_true, _state);
            }
            minqpoptimize(&state, _state);
            minqpresults(&state, &x1, &rep, _state);
            seterrorflag(&result, rep.terminationtype!=-4, _state);
            minqpsetbc(&state, &bndl, &bndu, _state);
            minqpoptimize(&state, _state);
            minqpresults(&state, &x1, &rep, _state);
            seterrorflag(&result, rep.terminationtype<=0, _state);
            if( rep.terminationtype<=0 )
            {
                ae_frame_leave(_state);
                return result;
            }
            
            /*
             * Test - calculate constrained gradient at solution,
             * check its norm.
             */
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(&result, ae_fp_greater(b.ptr.p_double[i],(double)(0))&&ae_fp_greater(x1.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
                seterrorflag(&result, ae_fp_less(b.ptr.p_double[i],(double)(0))&&ae_fp_less(x1.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
            }
        }
    }
    
    /*
     * Test for Newton phase of QQP algorithm - we test that Newton
     * phase can find good solution within one step. In order to do
     * so we:
     * * solve convex QP problem (dense or sparse)
     * * with K<=N equality-only constraints ai=x=bi
     * * with number of outer iterations limited to just 1
     * * and with CG phase turned off (we modify internal structures
     *   of the QQP solver in order to make it)
     */
    eps = 1.0E-5;
    for(pass=1; pass<=10; pass++)
    {
        
        /*
         * Generate problem
         */
        n = 50+hqrnduniformi(&rs, 51, _state);
        spdmatrixrndcond(n, 1.0E3, &a, _state);
        testminqpunit_densetosparse(&a, n, &sa, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&bndl, n, _state);
        ae_vector_set_length(&bndu, n, _state);
        ae_vector_set_length(&x0, n, _state);
        for(i=0; i<=n-1; i++)
        {
            b.ptr.p_double[i] = hqrndnormal(&rs, _state);
            x0.ptr.p_double[i] = hqrndnormal(&rs, _state);
            if( ae_fp_greater(hqrndnormal(&rs, _state),(double)(0)) )
            {
                bndl.ptr.p_double[i] = _state->v_neginf;
                bndu.ptr.p_double[i] = _state->v_posinf;
            }
            else
            {
                bndl.ptr.p_double[i] = hqrndnormal(&rs, _state);
                bndu.ptr.p_double[i] = bndl.ptr.p_double[i];
            }
        }
        
        /*
         * Solve problem
         *
         * NOTE: we modify internal structures of QQP solver in order
         *       to deactivate CG phase
         */
        minqpcreate(n, &state, _state);
        minqpsetalgoquickqp(&state, 0.0, 0.0, 0.0, 1, ae_true, _state);
        state.qqpsettingsuser.cgphase = ae_false;
        minqpsetlinearterm(&state, &b, _state);
        if( ae_fp_greater(hqrndnormal(&rs, _state),(double)(0)) )
        {
            minqpsetquadraticterm(&state, &a, ae_fp_greater(hqrndnormal(&rs, _state),(double)(0)), _state);
        }
        else
        {
            minqpsetquadratictermsparse(&state, &sa, ae_fp_greater(hqrndnormal(&rs, _state),(double)(0)), _state);
        }
        minqpsetbc(&state, &bndl, &bndu, _state);
        minqpoptimize(&state, _state);
        minqpresults(&state, &x1, &rep, _state);
        seterrorflag(&result, rep.terminationtype<=0, _state);
        if( rep.terminationtype<=0 )
        {
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Test - calculate constrained gradient at solution,
         * check its norm.
         */
        gnorm = 0.0;
        for(i=0; i<=n-1; i++)
        {
            g = b.ptr.p_double[i];
            for(j=0; j<=n-1; j++)
            {
                g = g+a.ptr.pp_double[i][j]*x1.ptr.p_double[j];
            }
            if( ae_fp_eq(x1.ptr.p_double[i],bndl.ptr.p_double[i])&&ae_fp_greater(g,(double)(0)) )
            {
                g = (double)(0);
            }
            if( ae_fp_eq(x1.ptr.p_double[i],bndu.ptr.p_double[i])&&ae_fp_less(g,(double)(0)) )
            {
                g = (double)(0);
            }
            gnorm = gnorm+ae_sqr(g, _state);
            seterrorflag(&result, ae_fp_less(x1.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
            seterrorflag(&result, ae_fp_greater(x1.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
        }
        gnorm = ae_sqrt(gnorm, _state);
        seterrorflag(&result, ae_fp_greater(gnorm,eps), _state);
    }
    
    /*
     * Test for Newton phase of QQP algorithm - we test that Newton
     * updates work correctly, i.e. that CNewtonUpdate() internal
     * function correctly updates inverse Hessian matrix.
     *
     * To test it we:
     * * solve ill conditioned convex QP problem
     * * with unconstrained solution XZ whose components are within [-0.5,+0.5]
     * * with one inequality constraint X[k]>=5
     * * with initial point such that:
     *   * X0[i] = 100       for i<>k
     *   * X0[k] = 5+1.0E-5
     * * with number of outer iterations limited to just 1
     * * and with CG phase turned off (we modify internal structures
     *   of the QQP solver in order to make it)
     *
     * The idea is that single Newton step is not enough to find solution,
     * but with just one update we can move exactly to the solution.
     *
     * We perform two tests:
     * * first one with State.QQP.NewtMaxIts set to 1, in order to
     *   make sure that algorithm fails with just one iteration
     * * second one with State.QQP.NewtMaxIts set to 2, in order to
     *   make sure that algorithm converges when it can perform update
     */
    eps = 1.0E-5;
    for(pass=1; pass<=10; pass++)
    {
        
        /*
         * Generate problem
         */
        n = 20+hqrnduniformi(&rs, 20, _state);
        spdmatrixrndcond(n, 1.0E5, &a, _state);
        testminqpunit_densetosparse(&a, n, &sa, _state);
        sparseconverttocrs(&sa, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&bndl, n, _state);
        ae_vector_set_length(&bndu, n, _state);
        ae_vector_set_length(&x0, n, _state);
        ae_vector_set_length(&xz, n, _state);
        for(i=0; i<=n-1; i++)
        {
            xz.ptr.p_double[i] = hqrnduniformr(&rs, _state)-0.5;
            x0.ptr.p_double[i] = (double)(100);
            bndl.ptr.p_double[i] = _state->v_neginf;
            bndu.ptr.p_double[i] = _state->v_posinf;
        }
        k = hqrnduniformi(&rs, n, _state);
        x0.ptr.p_double[k] = 5.00001;
        bndl.ptr.p_double[k] = 5.0;
        sparsemv(&sa, &xz, &b, _state);
        for(i=0; i<=n-1; i++)
        {
            b.ptr.p_double[i] = -b.ptr.p_double[i];
        }
        
        /*
         * Create solver
         */
        minqpcreate(n, &state, _state);
        minqpsetalgoquickqp(&state, 0.0, 0.0, 0.0, 1, ae_true, _state);
        minqpsetlinearterm(&state, &b, _state);
        minqpsetquadraticterm(&state, &a, ae_fp_greater(hqrndnormal(&rs, _state),(double)(0)), _state);
        minqpsetbc(&state, &bndl, &bndu, _state);
        minqpsetstartingpoint(&state, &x0, _state);
        
        /*
         * Solve problem. First time, with no Newton updates.
         * It must fail.
         *
         * NOTE: we modify internal structures of QQP solver in order
         *       to deactivate CG phase and turn off Newton updates.
         */
        state.qqpsettingsuser.cgphase = ae_false;
        state.qqpsettingsuser.cnphase = ae_true;
        state.qqpsettingsuser.cnmaxupdates = 0;
        minqpoptimize(&state, _state);
        minqpresults(&state, &x1, &rep, _state);
        seterrorflag(&result, rep.terminationtype<=0, _state);
        if( result )
        {
            ae_frame_leave(_state);
            return result;
        }
        flag = ae_false;
        testminqpunit_testbcgradandfeasibility(&a, &b, &bndl, &bndu, n, &x1, eps, &flag, _state);
        seterrorflag(&result, !flag, _state);
        
        /*
         * Now with Newton updates - it must succeeed.
         */
        state.qqpsettingsuser.cgphase = ae_false;
        state.qqpsettingsuser.cnmaxupdates = n;
        minqpoptimize(&state, _state);
        minqpresults(&state, &x1, &rep, _state);
        seterrorflag(&result, rep.terminationtype<=0, _state);
        if( result )
        {
            ae_frame_leave(_state);
            return result;
        }
        flag = ae_false;
        testminqpunit_testbcgradandfeasibility(&a, &b, &bndl, &bndu, n, &x1, eps, &flag, _state);
        seterrorflag(&result, flag, _state);
    }
    
    /*
     * Check that problem with general constraints results in
     * correct error code (-5 should be returned).
     */
    ae_matrix_set_length(&c, 1, 3, _state);
    ae_vector_set_length(&ct, 1, _state);
    c.ptr.pp_double[0][0] = 1.0;
    c.ptr.pp_double[0][1] = 1.0;
    c.ptr.pp_double[0][2] = 2.0;
    ct.ptr.p_int[0] = 0;
    minqpcreate(2, &state, _state);
    minqpsetalgoquickqp(&state, 0.0, 0.0, 0.0, 0, ae_fp_greater(hqrnduniformr(&rs, _state),0.5), _state);
    minqpsetlc(&state, &c, &ct, 1, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &x1, &rep, _state);
    seterrorflag(&result, rep.terminationtype!=-5, _state);
    
    /*
     * Test sparse functionality. QQP solver must perform
     * same steps independently of matrix type (dense or sparse).
     *
     * We generate random unconstrained test problem and solve it
     * twice - first time we solve dense version, second time -
     * sparse version is solved.
     *
     * During this test we:
     * * use stringent stopping criteria (one outer iteration)
     * * turn off Newton phase of the algorithm to slow down
     *   convergence
     */
    eps = 1.0E-3;
    itscnt = 1;
    n = 20;
    isupper = ae_fp_greater(ae_randomreal(_state),0.5);
    spdmatrixrndcond(n, 1.0E3, &za, _state);
    sparsecreate(n, n, 0, &sa, _state);
    ae_matrix_set_length(&a, n, n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( j>=i&&isupper )
            {
                sparseset(&sa, i, j, za.ptr.pp_double[i][j], _state);
                a.ptr.pp_double[i][j] = za.ptr.pp_double[i][j];
            }
            if( j<=i&&!isupper )
            {
                sparseset(&sa, i, j, za.ptr.pp_double[i][j], _state);
                a.ptr.pp_double[i][j] = za.ptr.pp_double[i][j];
            }
        }
    }
    ae_vector_set_length(&b, n, _state);
    ae_vector_set_length(&s, n, _state);
    for(i=0; i<=n-1; i++)
    {
        b.ptr.p_double[i] = randomnormal(_state);
        s.ptr.p_double[i] = ae_pow(10.0, randomnormal(_state)/10, _state);
    }
    minqpcreate(n, &state, _state);
    minqpsetalgoquickqp(&state, 0.0, 0.0, 0.0, itscnt, ae_false, _state);
    minqpsetscale(&state, &s, _state);
    minqpsetlinearterm(&state, &b, _state);
    minqpsetquadraticterm(&state, &a, isupper, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend0, &rep, _state);
    minqpcreate(n, &state, _state);
    minqpsetalgoquickqp(&state, 0.0, 0.0, 0.0, itscnt, ae_false, _state);
    minqpsetscale(&state, &s, _state);
    minqpsetlinearterm(&state, &b, _state);
    minqpsetquadratictermsparse(&state, &sa, isupper, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend1, &rep, _state);
    for(i=0; i<=n-1; i++)
    {
        seterrorflag(&result, ae_fp_greater(ae_fabs(xend0.ptr.p_double[i]-xend1.ptr.p_double[i], _state),eps), _state);
    }
    
    /*
     * Test scale-invariance. QQP performs same steps on scaled and
     * unscaled problems (assuming that scale of the variables is known).
     *
     * We generate random scale matrix S and random well-conditioned and
     * well scaled matrix A. Then we solve two problems:
     *
     *     (1) f = 0.5*x'*A*x+b'*x
     *         (identity scale matrix is used)
     *
     * and
     *
     *     (2) f = 0.5*y'*(inv(S)*A*inv(S))*y + (inv(S)*b)'*y
     *         (scale matrix S is used)
     *
     * Solution process is started from X=0, we perform ItsCnt=1 outer
     * iterations with Newton phase turned off (to slow down convergence;
     * we want to prevent algorithm from converging to exact solution which
     * is exactly same for both problems; the idea is to test that same
     * intermediate tests are taken).
     *
     * As result, we must get S*x=y
     */
    eps = 1.0E-3;
    itscnt = 1;
    n = 100;
    ae_vector_set_length(&s, n, _state);
    for(i=0; i<=n-1; i++)
    {
        s.ptr.p_double[i] = ae_pow(10.0, randomnormal(_state)/10, _state);
    }
    spdmatrixrndcond(n, 1.0E3, &a, _state);
    ae_matrix_set_length(&za, n, n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            za.ptr.pp_double[i][j] = a.ptr.pp_double[i][j]/(s.ptr.p_double[i]*s.ptr.p_double[j]);
        }
    }
    ae_vector_set_length(&b, n, _state);
    ae_vector_set_length(&zb, n, _state);
    for(i=0; i<=n-1; i++)
    {
        b.ptr.p_double[i] = randomnormal(_state);
        zb.ptr.p_double[i] = b.ptr.p_double[i]/s.ptr.p_double[i];
    }
    minqpcreate(n, &state, _state);
    minqpsetalgoquickqp(&state, 0.0, 0.0, 0.0, itscnt, ae_false, _state);
    minqpsetlinearterm(&state, &b, _state);
    minqpsetquadraticterm(&state, &a, ae_true, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend0, &rep, _state);
    minqpcreate(n, &state, _state);
    minqpsetalgoquickqp(&state, 0.0, 0.0, 0.0, itscnt, ae_false, _state);
    minqpsetlinearterm(&state, &zb, _state);
    minqpsetquadraticterm(&state, &za, ae_true, _state);
    minqpsetscale(&state, &s, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend1, &rep, _state);
    for(i=0; i<=n-1; i++)
    {
        seterrorflag(&result, ae_fp_greater(ae_fabs(s.ptr.p_double[i]*xend0.ptr.p_double[i]-xend1.ptr.p_double[i], _state),eps), _state);
    }
    
    /*
     * Test that QQP can efficiently use sparse matrices (i.e. it is
     * not disguised version of some dense QP solver). In order to test
     * it we create very large and very sparse problem (diagonal matrix
     * with N=40.000) and perform 10 iterations of QQP solver.
     *
     * In case QP solver uses some form of dense linear algebra to solve
     * this problem, it will take TOO much time to solve it. And we will
     * notice it by EXTREME slowdown during testing.
     */
    n = 40000;
    sparsecreate(n, n, 0, &sa, _state);
    for(i=0; i<=n-1; i++)
    {
        sparseset(&sa, i, i, ae_pow(10.0, -3*ae_randomreal(_state), _state), _state);
    }
    ae_vector_set_length(&b, n, _state);
    for(i=0; i<=n-1; i++)
    {
        b.ptr.p_double[i] = randomnormal(_state);
    }
    minqpcreate(n, &state, _state);
    minqpsetalgoquickqp(&state, 0.0, 0.0, 0.0, 10, ae_fp_greater(hqrnduniformr(&rs, _state),0.5), _state);
    minqpsetlinearterm(&state, &b, _state);
    minqpsetquadratictermsparse(&state, &sa, ae_true, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend0, &rep, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function performs tests specific for BLEIC solver
    
Returns True on error, False on success.
*************************************************************************/
ae_bool bleictests(ae_state *_state)
{
    ae_frame _frame_block;
    minqpstate state;
    minqpreport rep;
    ae_vector nlist;
    ae_int_t nidx;
    ae_matrix a;
    ae_matrix za;
    ae_matrix c;
    ae_vector b;
    ae_vector zb;
    ae_vector bndl;
    ae_vector bndu;
    ae_vector s;
    ae_vector x;
    ae_vector ct;
    sparsematrix sa;
    ae_int_t n;
    ae_vector x0;
    ae_vector x1;
    hqrndstate rs;
    ae_int_t i;
    ae_int_t j;
    ae_int_t pass;
    ae_vector xend0;
    ae_vector xend1;
    double eps;
    double v;
    double g;
    double gnorm;
    ae_int_t itscnt;
    ae_bool isupper;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _minqpstate_init(&state, _state);
    _minqpreport_init(&rep, _state);
    ae_vector_init(&nlist, 0, DT_INT, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&za, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&zb, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    _sparsematrix_init(&sa, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    _hqrndstate_init(&rs, _state);
    ae_vector_init(&xend0, 0, DT_REAL, _state);
    ae_vector_init(&xend1, 0, DT_REAL, _state);

    result = ae_false;
    hqrndrandomize(&rs, _state);
    
    /*
     * Test sparse functionality. BLEIC-based solver must perform
     * same steps independently of matrix type (dense or sparse).
     *
     * We generate random unconstrained test problem and solve it
     * twice - first time we solve dense version, second time -
     * sparse version is solved.
     */
    eps = 1.0E-3;
    itscnt = 5;
    n = 20;
    isupper = ae_fp_greater(ae_randomreal(_state),0.5);
    spdmatrixrndcond(n, 1.0E3, &za, _state);
    sparsecreate(n, n, 0, &sa, _state);
    ae_matrix_set_length(&a, n, n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( j>=i&&isupper )
            {
                sparseset(&sa, i, j, za.ptr.pp_double[i][j], _state);
                a.ptr.pp_double[i][j] = za.ptr.pp_double[i][j];
            }
            if( j<=i&&!isupper )
            {
                sparseset(&sa, i, j, za.ptr.pp_double[i][j], _state);
                a.ptr.pp_double[i][j] = za.ptr.pp_double[i][j];
            }
        }
    }
    ae_vector_set_length(&b, n, _state);
    ae_vector_set_length(&s, n, _state);
    for(i=0; i<=n-1; i++)
    {
        b.ptr.p_double[i] = randomnormal(_state);
        s.ptr.p_double[i] = ae_pow(10.0, randomnormal(_state)/10, _state);
    }
    minqpcreate(n, &state, _state);
    minqpsetalgobleic(&state, 0.0, 0.0, 0.0, itscnt, _state);
    minqpsetlinearterm(&state, &b, _state);
    minqpsetquadraticterm(&state, &a, isupper, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend0, &rep, _state);
    minqpcreate(n, &state, _state);
    minqpsetalgobleic(&state, 0.0, 0.0, 0.0, itscnt, _state);
    minqpsetlinearterm(&state, &b, _state);
    minqpsetquadratictermsparse(&state, &sa, isupper, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend1, &rep, _state);
    for(i=0; i<=n-1; i++)
    {
        seterrorflag(&result, ae_fp_greater(ae_fabs(xend0.ptr.p_double[i]-xend1.ptr.p_double[i], _state),eps), _state);
    }
    
    /*
     * Test scale-invariance. BLEIC performs same steps on scaled and
     * unscaled problems (assuming that scale of the variables is known).
     *
     * We generate random scale matrix S and random well-conditioned and
     * well scaled matrix A. Then we solve two problems:
     *
     *     (1) f = 0.5*x'*A*x+b'*x
     *         (identity scale matrix is used)
     *
     * and
     *
     *     (2) f = 0.5*y'*(inv(S)*A*inv(S))*y + (inv(S)*b)'*y
     *         (scale matrix S is used)
     *
     * Solution process is started from X=0, we perform ItsCnt=5 steps.
     * As result, we must get S*x=y
     */
    eps = 1.0E-3;
    itscnt = 5;
    n = 20;
    ae_vector_set_length(&s, n, _state);
    for(i=0; i<=n-1; i++)
    {
        s.ptr.p_double[i] = ae_pow(10.0, randomnormal(_state)/10, _state);
    }
    spdmatrixrndcond(n, 1.0E3, &a, _state);
    ae_matrix_set_length(&za, n, n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            za.ptr.pp_double[i][j] = a.ptr.pp_double[i][j]/(s.ptr.p_double[i]*s.ptr.p_double[j]);
        }
    }
    ae_vector_set_length(&b, n, _state);
    ae_vector_set_length(&zb, n, _state);
    for(i=0; i<=n-1; i++)
    {
        b.ptr.p_double[i] = randomnormal(_state);
        zb.ptr.p_double[i] = b.ptr.p_double[i]/s.ptr.p_double[i];
    }
    minqpcreate(n, &state, _state);
    minqpsetalgobleic(&state, 0.0, 0.0, 0.0, itscnt, _state);
    minqpsetlinearterm(&state, &b, _state);
    minqpsetquadraticterm(&state, &a, ae_true, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend0, &rep, _state);
    minqpcreate(n, &state, _state);
    minqpsetalgobleic(&state, 0.0, 0.0, 0.0, itscnt, _state);
    minqpsetlinearterm(&state, &zb, _state);
    minqpsetquadraticterm(&state, &za, ae_true, _state);
    minqpsetscale(&state, &s, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend1, &rep, _state);
    for(i=0; i<=n-1; i++)
    {
        seterrorflag(&result, ae_fp_greater(ae_fabs(s.ptr.p_double[i]*xend0.ptr.p_double[i]-xend1.ptr.p_double[i], _state),eps), _state);
    }
    
    /*
     * Test that BLEIC can efficiently use sparse matrices (i.e. it is
     * not disguised version of some dense QP solver). In order to test
     * it we create very large and very sparse problem (diagonal matrix
     * with N=20.000) and perform 10 iterations of BLEIC-based QP solver.
     *
     * In case QP solver uses some form of dense linear algebra to solve
     * this problem, it will take TOO much time to solve it. And we will
     * notice it by EXTREME slowdown during testing.
     */
    n = 20000;
    sparsecreate(n, n, 0, &sa, _state);
    for(i=0; i<=n-1; i++)
    {
        sparseset(&sa, i, i, ae_pow(10.0, -3*ae_randomreal(_state), _state), _state);
    }
    ae_vector_set_length(&b, n, _state);
    for(i=0; i<=n-1; i++)
    {
        b.ptr.p_double[i] = randomnormal(_state);
    }
    minqpcreate(n, &state, _state);
    minqpsetalgobleic(&state, 0.0, 0.0, 0.0, 10, _state);
    minqpsetlinearterm(&state, &b, _state);
    minqpsetquadratictermsparse(&state, &sa, ae_true, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend0, &rep, _state);
    
    /*
     * Special semi-definite test:
     * * N dimensions, N>=2 (important!)
     * * box constraints, x[i] in [-1,+1]
     *       [ 1 1 ... 1 1 ]
     * * A = [ ... ... ... ]
     *       [ 1 1 ... 1 1 ]
     * * random B such that SUM(b[i])=0.0 (important!)
     * * initial point x0 is chosen in such way that SUM(x[i])=0.0
     *   (important!)
     *
     * We perform two tests:
     * * unconstrained problem must be recognized as unbounded
     *   (when starting from x0!)
     * * constrained problem must be recognized as bounded
     *   and successfully solved
     *
     * Both problems require subtle programming when we work
     * with semidefinite QP.
     *
     * NOTE: it is very important to have N>=2 (otherwise problem
     *       will be bounded from below even without boundary
     *       constraints) and to have x0/b0 such that sum of 
     *       components is zero (such x0 is exact minimum of x'*A*x,
     *       which allows algorithm to find direction of zero curvature
     *       at the very first step). If x0/b are chosen in other way,
     *       algorithm may be unable to find direction of zero
     *       curvature and will cycle forever, slowly decreasing
     *       function value at each iteration.
     *       This is major difference from similar test for QQP solver -
     *       QQP can find direction of zero curvature from almost any
     *       point due to internal CG solver which favors such directions.
     *       BLEIC uses LBFGS, which is less able to find direction of
     *       zero curvature.
     */
    ae_vector_set_length(&nlist, 12, _state);
    nlist.ptr.p_int[0] = 2;
    nlist.ptr.p_int[1] = 3;
    nlist.ptr.p_int[2] = 4;
    nlist.ptr.p_int[3] = 5;
    nlist.ptr.p_int[4] = 6;
    nlist.ptr.p_int[5] = 7;
    nlist.ptr.p_int[6] = 8;
    nlist.ptr.p_int[7] = 9;
    nlist.ptr.p_int[8] = 10;
    nlist.ptr.p_int[9] = 20;
    nlist.ptr.p_int[10] = 40;
    nlist.ptr.p_int[11] = 80;
    eps = 1.0E-5;
    for(nidx=0; nidx<=nlist.cnt-1; nidx++)
    {
        for(pass=1; pass<=10; pass++)
        {
            
            /*
             * Generate problem
             */
            n = nlist.ptr.p_int[nidx];
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&x0, n, _state);
            for(i=0; i<=n-1; i++)
            {
                do
                {
                    b.ptr.p_double[i] = hqrndnormal(&rs, _state);
                }
                while(ae_fp_eq(b.ptr.p_double[i],(double)(0)));
                bndl.ptr.p_double[i] = (double)(-1);
                bndu.ptr.p_double[i] = (double)(1);
                x0.ptr.p_double[i] = 2*hqrnduniformr(&rs, _state)-1;
            }
            v = 0.0;
            for(i=0; i<=n-1; i++)
            {
                v = v+x0.ptr.p_double[i];
            }
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = x0.ptr.p_double[i]-v/n;
            }
            v = 0.0;
            for(i=0; i<=n-1; i++)
            {
                v = v+b.ptr.p_double[i];
            }
            for(i=0; i<=n-1; i++)
            {
                b.ptr.p_double[i] = b.ptr.p_double[i]-v/n;
            }
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = 1.0;
                }
            }
            testminqpunit_densetosparse(&a, n, &sa, _state);
            
            /*
             * Solve problem:
             * * without constraints we expect failure
             * * with constraints algorithm must succeed
             */
            minqpcreate(n, &state, _state);
            minqpsetalgobleic(&state, 0.0, 0.0, 0.0, 0, _state);
            minqpsetlinearterm(&state, &b, _state);
            minqpsetstartingpoint(&state, &x0, _state);
            if( ae_fp_greater(hqrndnormal(&rs, _state),(double)(0)) )
            {
                minqpsetquadraticterm(&state, &a, ae_true, _state);
            }
            else
            {
                minqpsetquadratictermsparse(&state, &sa, ae_true, _state);
            }
            minqpoptimize(&state, _state);
            minqpresults(&state, &x1, &rep, _state);
            seterrorflag(&result, rep.terminationtype!=-4, _state);
            minqpsetbc(&state, &bndl, &bndu, _state);
            minqpoptimize(&state, _state);
            minqpresults(&state, &x1, &rep, _state);
            seterrorflag(&result, rep.terminationtype<=0, _state);
            if( rep.terminationtype<=0 )
            {
                ae_frame_leave(_state);
                return result;
            }
            
            /*
             * Test - calculate constrained gradient at solution,
             * check its norm.
             */
            gnorm = 0.0;
            for(i=0; i<=n-1; i++)
            {
                g = b.ptr.p_double[i];
                for(j=0; j<=n-1; j++)
                {
                    g = g+a.ptr.pp_double[i][j]*x1.ptr.p_double[j];
                }
                if( ae_fp_eq(x1.ptr.p_double[i],bndl.ptr.p_double[i])&&ae_fp_greater(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                if( ae_fp_eq(x1.ptr.p_double[i],bndu.ptr.p_double[i])&&ae_fp_less(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                gnorm = gnorm+ae_sqr(g, _state);
                seterrorflag(&result, ae_fp_less(x1.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
                seterrorflag(&result, ae_fp_greater(x1.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
            }
            gnorm = ae_sqrt(gnorm, _state);
            seterrorflag(&result, ae_fp_greater(gnorm,eps), _state);
        }
    }
    
    /*
     * Test that BLEIC-based QP solver can solve non-convex problems
     * which are bounded from below on the feasible set:
     *
     *     min -||x||^2 s.t. x[i] in [-1,+1]
     *
     * We also test ability of the solver to detect unbounded problems
     * (we remove one of the constraints and repeat solution process).
     */
    n = 20;
    eps = 1.0E-14;
    sparsecreate(n, n, 0, &sa, _state);
    for(i=0; i<=n-1; i++)
    {
        sparseset(&sa, i, i, (double)(-1), _state);
    }
    ae_vector_set_length(&bndl, n, _state);
    ae_vector_set_length(&bndu, n, _state);
    ae_vector_set_length(&x, n, _state);
    for(i=0; i<=n-1; i++)
    {
        bndl.ptr.p_double[i] = (double)(-1);
        bndu.ptr.p_double[i] = (double)(1);
        x.ptr.p_double[i] = ae_randomreal(_state)-0.5;
    }
    minqpcreate(n, &state, _state);
    minqpsetalgobleic(&state, eps, 0.0, 0.0, 0, _state);
    minqpsetquadratictermsparse(&state, &sa, ae_true, _state);
    minqpsetbc(&state, &bndl, &bndu, _state);
    minqpsetstartingpoint(&state, &x, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend0, &rep, _state);
    seterrorflag(&result, rep.terminationtype<=0, _state);
    if( rep.terminationtype>0 )
    {
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(&result, ae_fp_neq(xend0.ptr.p_double[i],(double)(-1))&&ae_fp_neq(xend0.ptr.p_double[i],(double)(1)), _state);
        }
    }
    i = ae_randominteger(n, _state);
    bndl.ptr.p_double[i] = _state->v_neginf;
    bndu.ptr.p_double[i] = _state->v_posinf;
    minqpsetbc(&state, &bndl, &bndu, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend0, &rep, _state);
    seterrorflag(&result, rep.terminationtype!=-4, _state);
    
    /*
     * Test that BLEIC-based QP solver can solve non-convex problems
     * which are bounded from below on the feasible set:
     *
     *     min -||x||^2 s.t. x[i] in [-1,+1],
     *     with inequality constraints handled as general linear ones
     *
     * We also test ability of the solver to detect unbounded problems
     * (we remove last pair of constraints and try to solve modified
     * problem).
     */
    n = 20;
    eps = 1.0E-14;
    sparsecreate(n, n, 0, &sa, _state);
    for(i=0; i<=n-1; i++)
    {
        sparseset(&sa, i, i, (double)(-1), _state);
    }
    ae_matrix_set_length(&c, 2*n, n+1, _state);
    ae_vector_set_length(&ct, 2*n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n; j++)
        {
            c.ptr.pp_double[2*i+0][j] = (double)(0);
            c.ptr.pp_double[2*i+1][j] = (double)(0);
        }
        c.ptr.pp_double[2*i+0][i] = 1.0;
        c.ptr.pp_double[2*i+0][n] = 1.0;
        ct.ptr.p_int[2*i+0] = -1;
        c.ptr.pp_double[2*i+1][i] = 1.0;
        c.ptr.pp_double[2*i+1][n] = -1.0;
        ct.ptr.p_int[2*i+1] = 1;
    }
    ae_vector_set_length(&x, n, _state);
    for(i=0; i<=n-1; i++)
    {
        x.ptr.p_double[i] = ae_randomreal(_state)-0.5;
    }
    minqpcreate(n, &state, _state);
    minqpsetalgobleic(&state, eps, 0.0, 0.0, 0, _state);
    minqpsetquadratictermsparse(&state, &sa, ae_true, _state);
    minqpsetlc(&state, &c, &ct, 2*n, _state);
    minqpsetstartingpoint(&state, &x, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend0, &rep, _state);
    seterrorflag(&result, rep.terminationtype<=0, _state);
    if( rep.terminationtype>0 )
    {
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(&result, ae_fp_greater(ae_fabs(xend0.ptr.p_double[i]+1, _state),100*ae_machineepsilon)&&ae_fp_greater(ae_fabs(xend0.ptr.p_double[i]-1, _state),100*ae_machineepsilon), _state);
        }
    }
    minqpsetlc(&state, &c, &ct, 2*(n-1), _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend0, &rep, _state);
    seterrorflag(&result, rep.terminationtype!=-4, _state);
    
    /*
     * Test that BLEIC-based QP solver can solve QP problems with
     * zero quadratic term:
     *
     *     min b'*x  s.t. x[i] in [-1,+1]
     *
     * It means that QP solver can be used as linear programming solver
     * (altough performance of such solver is worse than that of specialized
     * LP solver).
     *
     * NOTE: we perform this test twice - first time without explicitly setting
     *       quadratic term (we test that default quadratic term is zero), and
     *       second time - with explicitly set quadratic term.
     */
    n = 20;
    sparsecreate(n, n, 0, &sa, _state);
    ae_vector_set_length(&bndl, n, _state);
    ae_vector_set_length(&bndu, n, _state);
    ae_vector_set_length(&b, n, _state);
    for(i=0; i<=n-1; i++)
    {
        bndl.ptr.p_double[i] = (double)(-1);
        bndu.ptr.p_double[i] = (double)(1);
        b.ptr.p_double[i] = randomnormal(_state);
    }
    minqpcreate(n, &state, _state);
    minqpsetalgobleic(&state, eps, 0.0, 0.0, 0, _state);
    minqpsetlinearterm(&state, &b, _state);
    minqpsetbc(&state, &bndl, &bndu, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend0, &rep, _state);
    seterrorflag(&result, rep.terminationtype<=0, _state);
    if( rep.terminationtype>0 )
    {
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(&result, ae_fp_greater(b.ptr.p_double[i],(double)(0))&&ae_fp_neq(xend0.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
            seterrorflag(&result, ae_fp_less(b.ptr.p_double[i],(double)(0))&&ae_fp_neq(xend0.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
        }
    }
    minqpcreate(n, &state, _state);
    minqpsetalgobleic(&state, eps, 0.0, 0.0, 0, _state);
    minqpsetlinearterm(&state, &b, _state);
    minqpsetbc(&state, &bndl, &bndu, _state);
    minqpsetquadratictermsparse(&state, &sa, ae_true, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend0, &rep, _state);
    seterrorflag(&result, rep.terminationtype<=0, _state);
    if( rep.terminationtype>0 )
    {
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(&result, ae_fp_greater(b.ptr.p_double[i],(double)(0))&&ae_fp_neq(xend0.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
            seterrorflag(&result, ae_fp_less(b.ptr.p_double[i],(double)(0))&&ae_fp_neq(xend0.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
        }
    }
    
    /*
     * Test specific problem sent by V.Semenenko, which resulted in
     * the initinite loop in FindFeasiblePoint (before fix). We do
     * not test results returned by solver - simply being able to
     * stop is enough for this test.
     *
     * NOTE: it is important that modifications to problem are applied
     *       sequentially. Test fails after 100-5000 such modifications.
     *       One modification is not enough to cause failure.
     */
    n = 3;
    ae_matrix_set_length(&a, n, n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a.ptr.pp_double[i][j] = 0.0;
        }
    }
    a.ptr.pp_double[0][0] = 1.222990;
    a.ptr.pp_double[1][1] = 1.934900;
    a.ptr.pp_double[2][2] = 0.603924;
    ae_vector_set_length(&b, n, _state);
    b.ptr.p_double[0] = -4.97245;
    b.ptr.p_double[1] = -9.09039;
    b.ptr.p_double[2] = -4.63856;
    ae_matrix_set_length(&c, 8, n+1, _state);
    for(i=0; i<=c.rows-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            c.ptr.pp_double[i][j] = 0.0;
        }
    }
    c.ptr.pp_double[0][0] = (double)(1);
    c.ptr.pp_double[0][n] = 4.94298;
    c.ptr.pp_double[1][0] = (double)(1);
    c.ptr.pp_double[1][n] = 4.79981;
    c.ptr.pp_double[2][1] = (double)(1);
    c.ptr.pp_double[2][n] = -0.4848;
    c.ptr.pp_double[3][1] = (double)(1);
    c.ptr.pp_double[3][n] = -0.73804;
    c.ptr.pp_double[4][2] = (double)(1);
    c.ptr.pp_double[4][n] = 0.575729;
    c.ptr.pp_double[5][2] = (double)(1);
    c.ptr.pp_double[5][n] = 0.458645;
    c.ptr.pp_double[6][0] = (double)(1);
    c.ptr.pp_double[6][2] = (double)(-1);
    c.ptr.pp_double[6][n] = -0.0546574;
    c.ptr.pp_double[7][0] = (double)(1);
    c.ptr.pp_double[7][2] = (double)(-1);
    c.ptr.pp_double[7][n] = -0.5900440;
    ae_vector_set_length(&ct, 8, _state);
    ct.ptr.p_int[0] = -1;
    ct.ptr.p_int[1] = 1;
    ct.ptr.p_int[2] = -1;
    ct.ptr.p_int[3] = 1;
    ct.ptr.p_int[4] = -1;
    ct.ptr.p_int[5] = 1;
    ct.ptr.p_int[6] = -1;
    ct.ptr.p_int[7] = 1;
    ae_vector_set_length(&s, n, _state);
    s.ptr.p_double[0] = 0.143171;
    s.ptr.p_double[1] = 0.253240;
    s.ptr.p_double[2] = 0.117084;
    ae_vector_set_length(&x0, n, _state);
    x0.ptr.p_double[0] = 3.51126;
    x0.ptr.p_double[1] = 4.05731;
    x0.ptr.p_double[2] = 6.63307;
    for(pass=1; pass<=10000; pass++)
    {
        
        /*
         * Apply random distortion
         */
        for(j=0; j<=n-1; j++)
        {
            b.ptr.p_double[j] = b.ptr.p_double[j]+(2*hqrnduniformi(&rs, 2, _state)-1)*0.1;
        }
        for(j=0; j<=6-1; j++)
        {
            c.ptr.pp_double[j][n] = c.ptr.pp_double[j][n]+(2*hqrnduniformi(&rs, 2, _state)-1)*0.1;
        }
        
        /*
         * Solve
         */
        minqpcreate(3, &state, _state);
        minqpsetquadraticterm(&state, &a, ae_true, _state);
        minqpsetlinearterm(&state, &b, _state);
        minqpsetlc(&state, &c, &ct, 8, _state);
        minqpsetalgobleic(&state, 0.0, 0.0, 0.0, 0, _state);
        minqpsetstartingpoint(&state, &x0, _state);
        minqpoptimize(&state, _state);
        minqpresults(&state, &x1, &rep, _state);
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function tests bound constrained quadratic programming algorithm.

On failure sets error flag.
*************************************************************************/
static void testminqpunit_bcqptest(ae_bool* wereerrors, ae_state *_state)
{
    ae_frame _frame_block;
    minqpstate state;
    minqpreport rep;
    ae_int_t n;
    ae_int_t pass;
    ae_int_t i;
    ae_int_t j;
    double v;
    double g;
    double gnorm;
    ae_int_t origintype;
    ae_int_t scaletype;
    ae_bool isupper;
    ae_bool issparse;
    ae_matrix a;
    ae_matrix fulla;
    ae_matrix halfa;
    ae_matrix c;
    sparsematrix sa;
    ae_vector ct;
    ae_vector b;
    ae_vector bndl;
    ae_vector bndu;
    ae_vector x0;
    ae_vector x1;
    ae_vector xori;
    ae_vector xz;
    ae_vector s;
    double eps;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    _minqpstate_init(&state, _state);
    _minqpreport_init(&rep, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&fulla, 0, 0, DT_REAL, _state);
    ae_matrix_init(&halfa, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&sa, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&xori, 0, DT_REAL, _state);
    ae_vector_init(&xz, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    _hqrndstate_init(&rs, _state);

    hqrndrandomize(&rs, _state);
    
    /*
     * Convex test:
     * * N dimensions
     * * random number (0..N) of random boundary constraints
     * * positive-definite A
     * * algorithm randomly choose dense or sparse A, and for
     *   sparse matrix it randomly choose format.
     * * random B with normal entries
     * * initial point is random, feasible
     * * random origin (zero or non-zero) and scale (unit or
     *   non-unit) are generated
     */
    eps = 1.0E-4;
    for(n=1; n<=10; n++)
    {
        for(pass=1; pass<=10; pass++)
        {
            
            /*
             * Generate problem
             */
            origintype = hqrnduniformi(&rs, 2, _state);
            scaletype = hqrnduniformi(&rs, 2, _state);
            isupper = ae_fp_less(hqrnduniformr(&rs, _state),0.5);
            issparse = ae_fp_less(hqrnduniformr(&rs, _state),0.5);
            spdmatrixrndcond(n, 1.0E3, &fulla, _state);
            ae_matrix_set_length(&halfa, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( (j>=i&&isupper)||(j<=i&&!isupper) )
                    {
                        halfa.ptr.pp_double[i][j] = fulla.ptr.pp_double[i][j];
                    }
                    else
                    {
                        halfa.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                    }
                }
            }
            testminqpunit_densetosparse(&halfa, n, &sa, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&xori, n, _state);
            ae_vector_set_length(&s, n, _state);
            for(i=0; i<=n-1; i++)
            {
                b.ptr.p_double[i] = hqrndnormal(&rs, _state);
                bndl.ptr.p_double[i] = _state->v_neginf;
                bndu.ptr.p_double[i] = _state->v_posinf;
                x0.ptr.p_double[i] = hqrndnormal(&rs, _state);
                if( origintype==0 )
                {
                    xori.ptr.p_double[i] = (double)(0);
                }
                else
                {
                    xori.ptr.p_double[i] = hqrndnormal(&rs, _state);
                }
                if( scaletype==0 )
                {
                    s.ptr.p_double[i] = (double)(1);
                }
                else
                {
                    s.ptr.p_double[i] = ae_exp(hqrndnormal(&rs, _state), _state);
                }
                j = hqrnduniformi(&rs, 5, _state);
                if( j==0 )
                {
                    bndl.ptr.p_double[i] = (double)(0);
                    x0.ptr.p_double[i] = ae_fabs(x0.ptr.p_double[i], _state);
                }
                if( j==1 )
                {
                    bndu.ptr.p_double[i] = (double)(0);
                    x0.ptr.p_double[i] = -ae_fabs(x0.ptr.p_double[i], _state);
                }
                if( j==2 )
                {
                    bndl.ptr.p_double[i] = hqrndnormal(&rs, _state);
                    bndu.ptr.p_double[i] = bndl.ptr.p_double[i];
                    x0.ptr.p_double[i] = bndl.ptr.p_double[i];
                }
                if( j==3 )
                {
                    bndl.ptr.p_double[i] = -0.1;
                    bndu.ptr.p_double[i] = 0.1;
                    x0.ptr.p_double[i] = 0.2*hqrnduniformr(&rs, _state)-0.1;
                }
            }
            
            /*
             * Solve problem
             */
            minqpcreate(n, &state, _state);
            testminqpunit_setrandomalgoallmodern(&state, _state);
            minqpsetlinearterm(&state, &b, _state);
            minqpsetstartingpoint(&state, &x0, _state);
            if( issparse )
            {
                minqpsetquadratictermsparse(&state, &sa, isupper, _state);
            }
            else
            {
                minqpsetquadraticterm(&state, &halfa, isupper, _state);
            }
            if( origintype!=0 )
            {
                minqpsetorigin(&state, &xori, _state);
            }
            if( scaletype!=0 )
            {
                minqpsetscale(&state, &s, _state);
            }
            minqpsetbc(&state, &bndl, &bndu, _state);
            minqpoptimize(&state, _state);
            minqpresults(&state, &x1, &rep, _state);
            seterrorflag(wereerrors, rep.terminationtype<=0, _state);
            if( rep.terminationtype<=0 )
            {
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Test - calculate constrained gradient at solution,
             * check its norm.
             */
            gnorm = 0.0;
            for(i=0; i<=n-1; i++)
            {
                g = b.ptr.p_double[i];
                for(j=0; j<=n-1; j++)
                {
                    g = g+fulla.ptr.pp_double[i][j]*(x1.ptr.p_double[j]-xori.ptr.p_double[j]);
                }
                if( ae_fp_eq(x1.ptr.p_double[i],bndl.ptr.p_double[i])&&ae_fp_greater(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                if( ae_fp_eq(x1.ptr.p_double[i],bndu.ptr.p_double[i])&&ae_fp_less(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                gnorm = gnorm+ae_sqr(g, _state);
                seterrorflag(wereerrors, ae_fp_less(x1.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
                seterrorflag(wereerrors, ae_fp_greater(x1.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
            }
            gnorm = ae_sqrt(gnorm, _state);
            seterrorflag(wereerrors, ae_fp_greater(gnorm,eps), _state);
        }
    }
    
    /*
     * Semidefinite test:
     * * N dimensions
     * * nonnegativity constraints
     * * A = [ 1 1 ... 1 1 ; 1 1 ... 1 1 ; .... ; 1 1 ... 1 1 ]
     * * algorithm randomly choose dense or sparse A, and for
     *   sparse matrix it randomly choose format.
     * * random B with normal entries
     * * initial point is random, feasible
     */
    eps = 1.0E-4;
    for(n=1; n<=10; n++)
    {
        for(pass=1; pass<=10; pass++)
        {
            
            /*
             * Generate problem
             */
            isupper = ae_fp_less(hqrnduniformr(&rs, _state),0.5);
            issparse = ae_fp_less(hqrnduniformr(&rs, _state),0.5);
            ae_matrix_set_length(&fulla, n, n, _state);
            ae_matrix_set_length(&halfa, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    fulla.ptr.pp_double[i][j] = 1.0;
                    if( (j>=i&&isupper)||(j<=i&&!isupper) )
                    {
                        halfa.ptr.pp_double[i][j] = fulla.ptr.pp_double[i][j];
                    }
                    else
                    {
                        halfa.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                    }
                }
            }
            testminqpunit_densetosparse(&halfa, n, &sa, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&x0, n, _state);
            for(i=0; i<=n-1; i++)
            {
                b.ptr.p_double[i] = hqrndnormal(&rs, _state);
                bndl.ptr.p_double[i] = 0.0;
                bndu.ptr.p_double[i] = _state->v_posinf;
                x0.ptr.p_double[i] = (double)(hqrnduniformi(&rs, 2, _state));
            }
            
            /*
             * Solve problem
             */
            minqpcreate(n, &state, _state);
            testminqpunit_setrandomalgosemidefinite(&state, _state);
            minqpsetstartingpoint(&state, &x0, _state);
            minqpsetlinearterm(&state, &b, _state);
            if( issparse )
            {
                minqpsetquadratictermsparse(&state, &sa, isupper, _state);
            }
            else
            {
                minqpsetquadraticterm(&state, &halfa, isupper, _state);
            }
            minqpsetbc(&state, &bndl, &bndu, _state);
            minqpoptimize(&state, _state);
            minqpresults(&state, &x1, &rep, _state);
            seterrorflag(wereerrors, rep.terminationtype<=0, _state);
            if( rep.terminationtype<=0 )
            {
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Test - calculate constrained gradient at solution,
             * check its norm.
             */
            gnorm = 0.0;
            for(i=0; i<=n-1; i++)
            {
                g = b.ptr.p_double[i];
                for(j=0; j<=n-1; j++)
                {
                    g = g+fulla.ptr.pp_double[i][j]*x1.ptr.p_double[j];
                }
                if( ae_fp_eq(x1.ptr.p_double[i],bndl.ptr.p_double[i])&&ae_fp_greater(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                if( ae_fp_eq(x1.ptr.p_double[i],bndu.ptr.p_double[i])&&ae_fp_less(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                gnorm = gnorm+ae_sqr(g, _state);
                seterrorflag(wereerrors, ae_fp_less(x1.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
                seterrorflag(wereerrors, ae_fp_greater(x1.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
            }
            gnorm = ae_sqrt(gnorm, _state);
            seterrorflag(wereerrors, ae_fp_greater(gnorm,eps), _state);
        }
    }
    
    /*
     * Non-convex test:
     * * N dimensions, N>=2
     * * box constraints, x[i] in [-1,+1]
     * * A = A0-0.5*I, where A0 is SPD with unit norm and smallest
     *   singular value equal to 1.0E-3, I is identity matrix
     * * random B with normal entries
     * * initial point is random, feasible
     *
     * We perform two tests:
     * * unconstrained problem must be recognized as unbounded
     * * constrained problem can be successfully solved
     *
     * NOTE: it is important to have N>=2, because formula for A
     *       can be applied only to matrix with at least two
     *       singular values
     */
    eps = 1.0E-4;
    for(n=2; n<=10; n++)
    {
        for(pass=1; pass<=10; pass++)
        {
            
            /*
             * Generate problem
             */
            spdmatrixrndcond(n, 1.0E3, &fulla, _state);
            for(i=0; i<=n-1; i++)
            {
                fulla.ptr.pp_double[i][i] = fulla.ptr.pp_double[i][i]-0.5;
            }
            isupper = ae_fp_less(hqrnduniformr(&rs, _state),0.5);
            ae_matrix_set_length(&halfa, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( (j>=i&&isupper)||(j<=i&&!isupper) )
                    {
                        halfa.ptr.pp_double[i][j] = fulla.ptr.pp_double[i][j];
                    }
                    else
                    {
                        halfa.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                    }
                }
            }
            testminqpunit_densetosparse(&halfa, n, &sa, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&x0, n, _state);
            for(i=0; i<=n-1; i++)
            {
                b.ptr.p_double[i] = hqrndnormal(&rs, _state);
                bndl.ptr.p_double[i] = (double)(-1);
                bndu.ptr.p_double[i] = (double)(1);
                x0.ptr.p_double[i] = 2*hqrnduniformr(&rs, _state)-1;
            }
            
            /*
             * Solve problem:
             * * without constraints we expect failure
             * * with constraints algorithm must succeed
             */
            minqpcreate(n, &state, _state);
            testminqpunit_setrandomalgononconvex(&state, _state);
            minqpsetstartingpoint(&state, &x0, _state);
            minqpsetlinearterm(&state, &b, _state);
            if( ae_fp_greater(hqrndnormal(&rs, _state),(double)(0)) )
            {
                minqpsetquadraticterm(&state, &halfa, isupper, _state);
            }
            else
            {
                minqpsetquadratictermsparse(&state, &sa, isupper, _state);
            }
            minqpoptimize(&state, _state);
            minqpresults(&state, &x1, &rep, _state);
            seterrorflag(wereerrors, rep.terminationtype!=-4, _state);
            minqpsetbc(&state, &bndl, &bndu, _state);
            minqpoptimize(&state, _state);
            minqpresults(&state, &x1, &rep, _state);
            seterrorflag(wereerrors, rep.terminationtype<=0, _state);
            if( rep.terminationtype<=0 )
            {
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Test - calculate constrained gradient at solution,
             * check its norm.
             */
            gnorm = 0.0;
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&fulla.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                g = v+b.ptr.p_double[i];
                if( ae_fp_eq(x1.ptr.p_double[i],bndl.ptr.p_double[i])&&ae_fp_greater(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                if( ae_fp_eq(x1.ptr.p_double[i],bndu.ptr.p_double[i])&&ae_fp_less(g,(double)(0)) )
                {
                    g = (double)(0);
                }
                gnorm = gnorm+ae_sqr(g, _state);
                seterrorflag(wereerrors, ae_fp_less(x1.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
                seterrorflag(wereerrors, ae_fp_greater(x1.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
            }
            gnorm = ae_sqrt(gnorm, _state);
            seterrorflag(wereerrors, ae_fp_greater(gnorm,eps), _state);
        }
    }
    
    /*
     * Linear (zero-quadratic) test:
     * * N dimensions, N>=1
     * * box constraints, x[i] in [-1,+1]
     * * A = 0
     * * random B with normal entries
     * * initial point is random, feasible
     *
     * We perform two tests:
     * * unconstrained problem must be recognized as unbounded
     * * constrained problem can be successfully solved
     *
     * NOTE: we may explicitly set zero A, or assume that by
     *       default it is zero. During test we will try both
     *       ways.
     */
    eps = 1.0E-4;
    for(n=1; n<=10; n++)
    {
        for(pass=1; pass<=10; pass++)
        {
            
            /*
             * Generate problem
             */
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&x0, n, _state);
            for(i=0; i<=n-1; i++)
            {
                do
                {
                    b.ptr.p_double[i] = hqrndnormal(&rs, _state);
                }
                while(ae_fp_eq(b.ptr.p_double[i],(double)(0)));
                bndl.ptr.p_double[i] = (double)(-1);
                bndu.ptr.p_double[i] = (double)(1);
                x0.ptr.p_double[i] = 2*hqrnduniformr(&rs, _state)-1;
            }
            
            /*
             * Solve problem:
             * * without constraints we expect failure
             * * with constraints algorithm must succeed
             */
            minqpcreate(n, &state, _state);
            testminqpunit_setrandomalgononconvex(&state, _state);
            minqpsetlinearterm(&state, &b, _state);
            minqpsetstartingpoint(&state, &x0, _state);
            if( ae_fp_greater(hqrndnormal(&rs, _state),(double)(0)) )
            {
                ae_matrix_set_length(&a, n, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a.ptr.pp_double[i][j] = (double)(0);
                    }
                }
                minqpsetquadraticterm(&state, &a, ae_true, _state);
            }
            minqpoptimize(&state, _state);
            minqpresults(&state, &x1, &rep, _state);
            seterrorflag(wereerrors, rep.terminationtype!=-4, _state);
            minqpsetbc(&state, &bndl, &bndu, _state);
            minqpoptimize(&state, _state);
            minqpresults(&state, &x1, &rep, _state);
            seterrorflag(wereerrors, rep.terminationtype<=0, _state);
            if( rep.terminationtype<=0 )
            {
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * Test - calculate constrained gradient at solution,
             * check its norm.
             */
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(wereerrors, ae_fp_greater(b.ptr.p_double[i],(double)(0))&&ae_fp_greater(x1.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
                seterrorflag(wereerrors, ae_fp_less(b.ptr.p_double[i],(double)(0))&&ae_fp_less(x1.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function tests equality constrained quadratic programming algorithm.

Returns True on errors.
*************************************************************************/
static ae_bool testminqpunit_ecqptest(ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t k;
    ae_matrix a;
    ae_matrix q;
    ae_matrix c;
    ae_matrix a2;
    ae_vector b;
    ae_vector b2;
    ae_vector xstart;
    ae_vector xstart2;
    ae_vector xend;
    ae_vector xend2;
    ae_vector x0;
    ae_vector x1;
    ae_vector xd;
    ae_vector xs;
    ae_vector tmp;
    ae_vector g;
    ae_vector bndl;
    ae_vector bndu;
    ae_vector xorigin;
    ae_vector ct;
    double eps;
    double theta;
    double f0;
    double f1;
    minqpstate state;
    minqpstate state2;
    minqpreport rep;
    ae_int_t i;
    ae_int_t j;
    ae_int_t pass;
    ae_int_t rk;
    double v;
    ae_int_t aulits;
    ae_bool waserrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&q, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a2, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&b2, 0, DT_REAL, _state);
    ae_vector_init(&xstart, 0, DT_REAL, _state);
    ae_vector_init(&xstart2, 0, DT_REAL, _state);
    ae_vector_init(&xend, 0, DT_REAL, _state);
    ae_vector_init(&xend2, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&xd, 0, DT_REAL, _state);
    ae_vector_init(&xs, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    ae_vector_init(&g, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    ae_vector_init(&xorigin, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    _minqpstate_init(&state, _state);
    _minqpstate_init(&state2, _state);
    _minqpreport_init(&rep, _state);

    waserrors = ae_false;
    
    /*
     * First test:
     * * N*N identity A
     * * K<N equality constraints Q*x = Q*x0, where Q is random
     *   orthogonal K*N matrix, x0 is some random vector
     * * x1 is some random vector such that Q*x1=0. It is always possible
     *   to find such x1, because K<N
     * * optimization problem has form 0.5*x'*A*x-(x1*A)*x
     * * exact solution must be equal to x0
     */
    eps = 1.0E-4;
    for(n=2; n<=6; n++)
    {
        for(k=1; k<=n-1; k++)
        {
            
            /*
             * Generate problem: A, b, CMatrix, x0, XStart
             */
            rmatrixrndorthogonal(n, &q, _state);
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = (double)(0);
                }
                a.ptr.pp_double[i][i] = (double)(1);
            }
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&x1, n, _state);
            ae_vector_set_length(&xstart, n, _state);
            ae_matrix_set_length(&c, k, n+1, _state);
            ae_vector_set_length(&ct, k, _state);
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                x1.ptr.p_double[i] = x0.ptr.p_double[i];
                xstart.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            for(i=0; i<=k-1; i++)
            {
                ae_v_move(&c.ptr.pp_double[i][0], 1, &q.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                v = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                c.ptr.pp_double[i][n] = v;
                ct.ptr.p_int[i] = 0;
                v = 2*ae_randomreal(_state)-1;
                ae_v_addd(&x1.ptr.p_double[0], 1, &q.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
            }
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                b.ptr.p_double[i] = -v;
            }
            
            /*
             * Create optimizer, solve
             */
            minqpcreate(n, &state, _state);
            testminqpunit_setrandomalgoconvexlc(&state, _state);
            minqpsetlinearterm(&state, &b, _state);
            minqpsetquadraticterm(&state, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
            minqpsetstartingpoint(&state, &xstart, _state);
            minqpsetlc(&state, &c, &ct, k, _state);
            minqpoptimize(&state, _state);
            minqpresults(&state, &xend, &rep, _state);
            
            /*
             * Compare with analytic solution
             */
            if( rep.terminationtype<=0 )
            {
                waserrors = ae_true;
                continue;
            }
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors||ae_fp_greater(ae_fabs(xend.ptr.p_double[i]-x0.ptr.p_double[i], _state),eps);
            }
        }
    }
    
    /*
     * Second test:
     * * N*N SPD A
     * * K<N equality constraints Q*x = Q*x0, where Q is random
     *   orthogonal K*N matrix, x0 is some random vector
     * * optimization problem has form 0.5*x'*A*x-(x1*A)*x,
     *   where x1 is some random vector
     * * we check feasibility properties of the solution
     * * we do not know analytic form of the exact solution,
     *   but we know that projection of gradient into equality constrained
     *   subspace must be zero at the solution
     */
    eps = 1.0E-4;
    for(n=2; n<=6; n++)
    {
        for(k=1; k<=n-1; k++)
        {
            
            /*
             * Generate problem: A, b, CMatrix, x0, XStart
             */
            rmatrixrndorthogonal(n, &q, _state);
            spdmatrixrndcond(n, ae_pow(10.0, 3*ae_randomreal(_state), _state), &a, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&x1, n, _state);
            ae_vector_set_length(&xstart, n, _state);
            ae_matrix_set_length(&c, k, n+1, _state);
            ae_vector_set_length(&ct, k, _state);
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                xstart.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            for(i=0; i<=k-1; i++)
            {
                ae_v_move(&c.ptr.pp_double[i][0], 1, &q.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                v = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                c.ptr.pp_double[i][n] = v;
                ct.ptr.p_int[i] = 0;
            }
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                b.ptr.p_double[i] = -v;
            }
            
            /*
             * Create optimizer, solve
             */
            minqpcreate(n, &state, _state);
            testminqpunit_setrandomalgoconvexlc(&state, _state);
            minqpsetlinearterm(&state, &b, _state);
            minqpsetquadraticterm(&state, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
            minqpsetstartingpoint(&state, &xstart, _state);
            minqpsetlc(&state, &c, &ct, k, _state);
            minqpoptimize(&state, _state);
            minqpresults(&state, &xend, &rep, _state);
            
            /*
             * Calculate gradient, check projection
             */
            if( rep.terminationtype<=0 )
            {
                waserrors = ae_true;
                continue;
            }
            for(i=0; i<=k-1; i++)
            {
                v = ae_v_dotproduct(&xend.ptr.p_double[0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                waserrors = waserrors||ae_fp_greater(ae_fabs(v-c.ptr.pp_double[i][n], _state),eps);
            }
            ae_vector_set_length(&g, n, _state);
            ae_v_move(&g.ptr.p_double[0], 1, &b.ptr.p_double[0], 1, ae_v_len(0,n-1));
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xend.ptr.p_double[0], 1, ae_v_len(0,n-1));
                g.ptr.p_double[i] = g.ptr.p_double[i]+v;
            }
            for(i=0; i<=k-1; i++)
            {
                v = ae_v_dotproduct(&g.ptr.p_double[0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                ae_v_subd(&g.ptr.p_double[0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
            }
            v = ae_v_dotproduct(&g.ptr.p_double[0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,n-1));
            waserrors = waserrors||ae_fp_greater(ae_sqrt(v, _state),eps);
        }
    }
    
    /*
     * Boundary and linear equality constrained QP problem:
     * * N*N SPD A with moderate condtion number (up to 100)
     * * boundary constraints 0<=x[i]<=1
     * * K<N equality constraints C*x = C*x0, where Q is random
     *   K*N matrix, x0 is some random vector from the
     *   feasible hypercube (0<=x0[i]<=1)
     * * optimization problem has form 0.5*x'*A*x-(x1*A)*x,
     *   where x1 is some random vector with -1<=x1[i]<=+1.
     *   (sometimes solution is in the inner area, sometimes at the boundary)
     * * every component of the initial point XStart is either 0 or 1
     *   (point is located at the vertices of the feasible hypercube)
     * 
     * Solution of such problem is calculated using two methods:
     * a) boundary and linearly constrained QP
     * b) augmented Lagrangian boundary constrained QP: we add explicit quadratic
     *    penalty to the problem; we also add Lagrangian terms and perform many
     *    subsequent iterations to find good estimates of the Lagrange multipliers.
     *
     * Sometimes augmented Largangian converges to slightly different point
     * (boundary constraints lead to extremely slow, non-smooth convergence of
     * the Lagrange multipliers). In order to correctly handle such situations
     * we compare function values instead of final points - and use relaxed criteria
     * to test for convergence.
     *
     * NOTE: sometimes we need as much as 300 Augmented Lagrangian iterations for
     *       method to converge.
     */
    eps = 5.0E-2;
    theta = 1.0E+4;
    aulits = 300;
    for(n=2; n<=6; n++)
    {
        for(k=1; k<=n-1; k++)
        {
            
            /*
             * Generate problem: A, b, BndL, BndU, CMatrix, x0, x1, XStart
             */
            spdmatrixrndcond(n, ae_pow(10.0, 2*ae_randomreal(_state), _state), &a, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&x1, n, _state);
            ae_vector_set_length(&xstart, n, _state);
            ae_matrix_set_length(&c, k, n+1, _state);
            ae_vector_set_length(&ct, k, _state);
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = ae_randomreal(_state);
                x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                bndl.ptr.p_double[i] = 0.0;
                bndu.ptr.p_double[i] = 1.0;
                xstart.ptr.p_double[i] = (double)(ae_randominteger(2, _state));
            }
            for(i=0; i<=k-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    c.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                }
                v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                c.ptr.pp_double[i][n] = v;
                ct.ptr.p_int[i] = 0;
            }
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                b.ptr.p_double[i] = -v;
            }
            
            /*
             * Create exact optimizer, solve
             */
            minqpcreate(n, &state, _state);
            testminqpunit_setrandomalgoconvexlc(&state, _state);
            minqpsetlinearterm(&state, &b, _state);
            minqpsetquadraticterm(&state, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
            minqpsetstartingpoint(&state, &xstart, _state);
            minqpsetbc(&state, &bndl, &bndu, _state);
            minqpsetlc(&state, &c, &ct, k, _state);
            minqpoptimize(&state, _state);
            minqpresults(&state, &xend, &rep, _state);
            if( rep.terminationtype<=0 )
            {
                waserrors = ae_true;
                continue;
            }
            
            /*
             * Solve problem using barrier functions (quadrative objective, boundary constraints,
             * explicit penalty term added to the main quadratic matrix. Lagrangian terms improve
             * solution quality):
             * * A2 := A+C'*C
             * * b2 := b-r'*C
             * * b2 is iteratively updated using augmented Lagrangian update
             * 
             * NOTE: we may need many outer iterations to converge to the optimal values
             *       of Lagrange multipliers. Convergence is slowed down by the presense
             *       of boundary constraints, whose activation/deactivation slows down
             *       process.
             */
            ae_matrix_set_length(&a2, n, n, _state);
            rmatrixcopy(n, n, &a, 0, 0, &a2, 0, 0, _state);
            rmatrixsyrk(n, k, theta, &c, 0, 0, 2, 1.0, &a2, 0, 0, ae_true, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=i+1; j<=n-1; j++)
                {
                    a2.ptr.pp_double[j][i] = a2.ptr.pp_double[i][j];
                }
            }
            ae_vector_set_length(&b2, n, _state);
            ae_v_move(&b2.ptr.p_double[0], 1, &b.ptr.p_double[0], 1, ae_v_len(0,n-1));
            for(i=0; i<=k-1; i++)
            {
                v = c.ptr.pp_double[i][n]*theta;
                ae_v_subd(&b2.ptr.p_double[0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
            }
            minqpcreate(n, &state2, _state);
            testminqpunit_setrandomalgoconvexlc(&state, _state);
            minqpsetquadraticterm(&state2, &a2, ae_fp_greater(ae_randomreal(_state),0.5), _state);
            minqpsetstartingpoint(&state2, &xstart, _state);
            minqpsetbc(&state2, &bndl, &bndu, _state);
            for(i=1; i<=aulits; i++)
            {
                
                /*
                 * Solve, update B2 according to augmented Lagrangian algorithm
                 */
                minqpsetlinearterm(&state2, &b2, _state);
                minqpoptimize(&state2, _state);
                minqpresults(&state2, &xend2, &rep, _state);
                if( rep.terminationtype<=0 )
                {
                    waserrors = ae_true;
                    continue;
                }
                for(j=0; j<=k-1; j++)
                {
                    v = ae_v_dotproduct(&c.ptr.pp_double[j][0], 1, &xend2.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    v = theta*(v-c.ptr.pp_double[j][n]);
                    ae_v_addd(&b2.ptr.p_double[0], 1, &c.ptr.pp_double[j][0], 1, ae_v_len(0,n-1), v);
                }
            }
            
            /*
             * Calculate function value and XEnd and XEnd2
             */
            f0 = 0.0;
            f1 = 0.0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    f0 = f0+0.5*xend.ptr.p_double[i]*a.ptr.pp_double[i][j]*xend.ptr.p_double[j];
                    f1 = f1+0.5*xend2.ptr.p_double[i]*a.ptr.pp_double[i][j]*xend2.ptr.p_double[j];
                }
                f0 = f0+xend.ptr.p_double[i]*b.ptr.p_double[i];
                f1 = f1+xend2.ptr.p_double[i]*b.ptr.p_double[i];
            }
            
            /*
             * Check feasibility properties and compare
             */
            waserrors = waserrors||ae_fp_greater(ae_fabs(f0-f1, _state),eps);
            for(i=0; i<=k-1; i++)
            {
                v = ae_v_dotproduct(&xend.ptr.p_double[0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                waserrors = waserrors||ae_fp_greater(ae_fabs(v-c.ptr.pp_double[i][n], _state),1.0E6*ae_machineepsilon);
            }
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors||ae_fp_less(xend.ptr.p_double[i],(double)(0));
                waserrors = waserrors||ae_fp_greater(xend.ptr.p_double[i],(double)(1));
            }
            if( waserrors )
            {
                waserrors = waserrors;
            }
        }
    }
    
    /*
     * Boundary and linear equality constrained QP problem,
     * test for correct handling of non-zero XOrigin:
     * * N*N SPD A with moderate condtion number (up to 100)
     * * boundary constraints 0<=x[i]<=1
     * * K<N equality constraints Q*x = Q*x0, where Q is random
     *   orthogonal K*N matrix, x0 is some random vector from the
     *   inner area of the feasible hypercube (0.1<=x0[i]<=0.9)
     * * optimization problem has form 0.5*(x-xorigin)'*A*(x-xorigin)+b*(x-xorigin),
     *   where b is some random vector with -1<=b[i]<=+1.
     *   (sometimes solution is in the inner area, sometimes at the boundary)
     * * every component of the initial point XStart is random from [-1,1]
     * 
     * Solution of such problem is calculated using two methods:
     * a) QP with SetOrigin() call
     * b) QP with XOrigin explicitly added to the quadratic function,
     *
     * Both methods should give same results; any significant difference is
     * evidence of some error in the QP implementation.
     */
    eps = 1.0E-4;
    for(n=2; n<=6; n++)
    {
        for(k=1; k<=n-1; k++)
        {
            
            /*
             * Generate problem: A, b, BndL, BndU, CMatrix, x0, x1, XStart.
             * Additionally, we compute modified b: b2 = b-xorigin'*A
             */
            rmatrixrndorthogonal(n, &q, _state);
            spdmatrixrndcond(n, ae_pow(10.0, 2*ae_randomreal(_state), _state), &a, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&b2, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&x1, n, _state);
            ae_vector_set_length(&xorigin, n, _state);
            ae_vector_set_length(&xstart, n, _state);
            ae_matrix_set_length(&c, k, n+1, _state);
            ae_vector_set_length(&ct, k, _state);
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = 0.1+0.8*ae_randomreal(_state);
                b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                bndl.ptr.p_double[i] = 0.0;
                bndu.ptr.p_double[i] = 1.0;
                xstart.ptr.p_double[i] = (double)(ae_randominteger(2, _state));
                xorigin.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            for(i=0; i<=k-1; i++)
            {
                ae_v_move(&c.ptr.pp_double[i][0], 1, &q.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                v = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                c.ptr.pp_double[i][n] = v;
                ct.ptr.p_int[i] = 0;
            }
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xorigin.ptr.p_double[0], 1, ae_v_len(0,n-1));
                b2.ptr.p_double[i] = b.ptr.p_double[i]-v;
            }
            
            /*
             * Solve with SetOrigin() call
             */
            minqpcreate(n, &state, _state);
            testminqpunit_setrandomalgoconvexlc(&state, _state);
            minqpsetlinearterm(&state, &b, _state);
            minqpsetquadraticterm(&state, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
            minqpsetstartingpoint(&state, &xstart, _state);
            minqpsetorigin(&state, &xorigin, _state);
            minqpsetbc(&state, &bndl, &bndu, _state);
            minqpsetlc(&state, &c, &ct, k, _state);
            minqpoptimize(&state, _state);
            minqpresults(&state, &xend, &rep, _state);
            if( rep.terminationtype<=0 )
            {
                waserrors = ae_true;
                continue;
            }
            
            /*
             * Solve problem using explicit origin
             */
            minqpcreate(n, &state2, _state);
            testminqpunit_setrandomalgoconvexlc(&state, _state);
            minqpsetlinearterm(&state2, &b2, _state);
            minqpsetquadraticterm(&state2, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
            minqpsetstartingpoint(&state2, &xstart, _state);
            minqpsetbc(&state2, &bndl, &bndu, _state);
            minqpsetlc(&state2, &c, &ct, k, _state);
            minqpoptimize(&state2, _state);
            minqpresults(&state2, &xend2, &rep, _state);
            if( rep.terminationtype<=0 )
            {
                waserrors = ae_true;
                continue;
            }
            
            /*
             * Check feasibility properties and compare solutions
             */
            for(i=0; i<=k-1; i++)
            {
                v = ae_v_dotproduct(&xend.ptr.p_double[0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                waserrors = waserrors||ae_fp_greater(ae_fabs(v-c.ptr.pp_double[i][n], _state),eps);
            }
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors||ae_fp_greater(ae_fabs(xend.ptr.p_double[i]-xend2.ptr.p_double[i], _state),eps);
                waserrors = waserrors||ae_fp_less(xend.ptr.p_double[i],(double)(0));
                waserrors = waserrors||ae_fp_greater(xend.ptr.p_double[i],(double)(1));
            }
            if( waserrors )
            {
                waserrors = waserrors;
            }
        }
    }
    
    /*
     * Boundary and linear equality constrained QP problem with excessive
     * equality constraints:
     * * N*N SPD A with moderate condtion number (up to 100)
     * * boundary constraints 0<=x[i]<=1
     * * K=2*N equality constraints Q*x = Q*x0, where Q is random matrix,
     *   x0 is some random vector from the feasible hypercube (0.1<=x0[i]<=0.9)
     * * optimization problem has form 0.5*x'*A*x-b*x,
     *   where b is some random vector
     * * because constraints are excessive, the main problem is to find
     *   feasible point; the only existing feasible point is solution,
     *   so we have to check only feasibility
     */
    eps = 1.0E-4;
    for(n=1; n<=6; n++)
    {
        
        /*
         * Generate problem: A, b, BndL, BndU, CMatrix, x0, x1, XStart
         */
        k = 2*n;
        spdmatrixrndcond(n, ae_pow(10.0, 3*ae_randomreal(_state), _state), &a, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&bndl, n, _state);
        ae_vector_set_length(&bndu, n, _state);
        ae_vector_set_length(&x0, n, _state);
        ae_vector_set_length(&x1, n, _state);
        ae_vector_set_length(&xstart, n, _state);
        ae_matrix_set_length(&c, k, n+1, _state);
        ae_vector_set_length(&ct, k, _state);
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = 0.1+0.8*ae_randomreal(_state);
            x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            bndl.ptr.p_double[i] = 0.0;
            bndu.ptr.p_double[i] = 1.0;
            xstart.ptr.p_double[i] = (double)(ae_randominteger(2, _state));
        }
        for(i=0; i<=k-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                c.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
            v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
            c.ptr.pp_double[i][n] = v;
            ct.ptr.p_int[i] = 0;
        }
        for(i=0; i<=n-1; i++)
        {
            b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        
        /*
         * Create optimizer, solve
         */
        minqpcreate(n, &state, _state);
        testminqpunit_setrandomalgoconvexlc(&state, _state);
        minqpsetlinearterm(&state, &b, _state);
        minqpsetquadraticterm(&state, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
        minqpsetstartingpoint(&state, &xstart, _state);
        minqpsetbc(&state, &bndl, &bndu, _state);
        minqpsetlc(&state, &c, &ct, k, _state);
        minqpoptimize(&state, _state);
        minqpresults(&state, &xend, &rep, _state);
        
        /*
         * Check feasibility properties of the solution
         */
        if( rep.terminationtype<=0 )
        {
            waserrors = ae_true;
            continue;
        }
        for(i=0; i<=k-1; i++)
        {
            v = ae_v_dotproduct(&xend.ptr.p_double[0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
            waserrors = waserrors||ae_fp_greater(ae_fabs(v-c.ptr.pp_double[i][n], _state),eps);
        }
    }
    
    /*
     * Boundary and linear equality constrained QP problem,
     * test checks that different starting points yield same final point:
     * * random N from [1..6], random K from [1..2*N]
     * * N*N SPD A with moderate condtion number (up to 100)
     * * boundary constraints 0<=x[i]<=1
     * * K<2*N random linear equality constraints C*x = C*x0,
     *   where x0 is some random vector from the inner area of the
     *   feasible hypercube (0.1<=x0[i]<=0.9)
     * * optimization problem has form 0.5*x'*A*x+b*x,
     *   where b is some random vector with -5<=b[i]<=+5
     * * every component of the initial point XStart is random from [-2,+2]
     * * we perform two starts from random different XStart and compare values
     *   of the target function (although final points may be slightly different,
     *   function values should match each other)
     * 
     * Both points should give same results; any significant difference is
     * evidence of some error in the QP implementation.
     */
    eps = 1.0E-4;
    for(pass=1; pass<=50; pass++)
    {
        
        /*
         * Generate problem: N, K, A, b, BndL, BndU, CMatrix, x0, x1, XStart.
         */
        n = ae_randominteger(5, _state)+2;
        k = ae_randominteger(2*n, _state)+1;
        spdmatrixrndcond(n, ae_pow(10.0, 2*ae_randomreal(_state), _state), &a, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&b2, n, _state);
        ae_vector_set_length(&bndl, n, _state);
        ae_vector_set_length(&bndu, n, _state);
        ae_vector_set_length(&x0, n, _state);
        ae_vector_set_length(&xstart, n, _state);
        ae_vector_set_length(&xstart2, n, _state);
        ae_matrix_set_length(&c, k, n+1, _state);
        ae_vector_set_length(&ct, k, _state);
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = 0.1+0.8*ae_randomreal(_state);
            b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            bndl.ptr.p_double[i] = 0.0;
            bndu.ptr.p_double[i] = 1.0;
            xstart.ptr.p_double[i] = 4*ae_randomreal(_state)-2;
            xstart2.ptr.p_double[i] = 4*ae_randomreal(_state)-2;
        }
        for(i=0; i<=k-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                c.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
            v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
            c.ptr.pp_double[i][n] = v;
            ct.ptr.p_int[i] = 0;
        }
        
        /*
         * Solve with XStart
         */
        minqpcreate(n, &state, _state);
        testminqpunit_setrandomalgoconvexlc(&state, _state);
        minqpsetlinearterm(&state, &b, _state);
        minqpsetquadraticterm(&state, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
        minqpsetstartingpoint(&state, &xstart, _state);
        minqpsetbc(&state, &bndl, &bndu, _state);
        minqpsetlc(&state, &c, &ct, k, _state);
        minqpoptimize(&state, _state);
        minqpresults(&state, &xend, &rep, _state);
        if( rep.terminationtype<=0 )
        {
            waserrors = ae_true;
            continue;
        }
        
        /*
         * Solve with XStart2
         */
        minqpsetstartingpoint(&state, &xstart2, _state);
        minqpoptimize(&state, _state);
        minqpresults(&state, &xend2, &rep, _state);
        if( rep.terminationtype<=0 )
        {
            waserrors = ae_true;
            continue;
        }
        
        /*
         * Calculate function value and XEnd and XEnd2, compare solutions
         */
        f0 = 0.0;
        f1 = 0.0;
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                f0 = f0+0.5*xend.ptr.p_double[i]*a.ptr.pp_double[i][j]*xend.ptr.p_double[j];
                f1 = f1+0.5*xend2.ptr.p_double[i]*a.ptr.pp_double[i][j]*xend2.ptr.p_double[j];
            }
            f0 = f0+xend.ptr.p_double[i]*b.ptr.p_double[i];
            f1 = f1+xend2.ptr.p_double[i]*b.ptr.p_double[i];
        }
        seterrorflag(&waserrors, ae_fp_greater(ae_fabs(f0-f1, _state),eps), _state);
    }
    
    /*
     * Test ability to correctly handle situation where algorithm
     * either:
     * (1) starts from point with gradient whose projection to
     *     active set is almost zero (but not exactly zero)
     * (2) performs step to such point
     *
     * In order to do this we solve problem
     * * min 0.5*x'*x - (x0+c)'*x
     * * subject to c'*x = c'*x0, with c and x0 random unit vectors
     * * with initial point xs = x0+r*xd, where r is scalar,
     *   xd is vector which is orthogonal to c.
     * * we try different r=power(2,-rk) for rk=0...70. The idea
     *   is that as we approach closer and closer to x0, which is
     *   a solution of the constrained problem, constrained gradient
     *   of the function rapidly vanishes.
     */
    eps = 1.0E-6;
    for(rk=0; rk<=70; rk++)
    {
        n = 10;
        
        /*
         * Generate x0, c, xd, xs, generate unit A
         */
        randomunit(n, &x0, _state);
        randomunit(n, &xd, _state);
        randomunit(n, &tmp, _state);
        ae_matrix_set_length(&c, 1, n+1, _state);
        ae_vector_set_length(&ct, 1, _state);
        ae_vector_set_length(&xs, n, _state);
        ae_vector_set_length(&b, n, _state);
        c.ptr.pp_double[0][n] = (double)(0);
        ct.ptr.p_int[0] = 0;
        v = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            c.ptr.pp_double[0][i] = tmp.ptr.p_double[i];
            c.ptr.pp_double[0][n] = c.ptr.pp_double[0][n]+tmp.ptr.p_double[i]*x0.ptr.p_double[i];
            b.ptr.p_double[i] = -(x0.ptr.p_double[i]+tmp.ptr.p_double[i]);
            v = v+tmp.ptr.p_double[i]*xd.ptr.p_double[i];
        }
        for(i=0; i<=n-1; i++)
        {
            xd.ptr.p_double[i] = xd.ptr.p_double[i]-v*tmp.ptr.p_double[i];
            xs.ptr.p_double[i] = x0.ptr.p_double[i]+xd.ptr.p_double[i]*ae_pow((double)(2), (double)(-rk), _state);
        }
        ae_matrix_set_length(&a, n, n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a.ptr.pp_double[i][j] = (double)(0);
            }
            a.ptr.pp_double[i][i] = (double)(1);
        }
        
        /*
         * Create and solve optimization problem
         */
        minqpcreate(n, &state, _state);
        testminqpunit_setrandomalgoconvexlc(&state, _state);
        minqpsetlinearterm(&state, &b, _state);
        minqpsetquadraticterm(&state, &a, ae_true, _state);
        minqpsetstartingpoint(&state, &xs, _state);
        minqpsetlc(&state, &c, &ct, 1, _state);
        minqpoptimize(&state, _state);
        minqpresults(&state, &xend, &rep, _state);
        if( seterrorflag(&waserrors, rep.terminationtype<=0, _state) )
        {
            continue;
        }
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(&waserrors, ae_fp_greater(ae_fabs(xend.ptr.p_double[i]-x0.ptr.p_double[i], _state),eps), _state);
        }
        v = -c.ptr.pp_double[0][n];
        for(i=0; i<=n-1; i++)
        {
            v = v+xend.ptr.p_double[i]*c.ptr.pp_double[0][i];
        }
        seterrorflag(&waserrors, ae_fp_greater(ae_fabs(v, _state),1.0E5*ae_machineepsilon), _state);
    }
    result = waserrors;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function tests inequality constrained quadratic programming algorithm.

On failure sets Err to True; on success leaves it unchanged.
*************************************************************************/
static void testminqpunit_icqptest(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t k;
    ae_matrix a;
    ae_matrix q;
    ae_matrix c;
    ae_matrix a2;
    ae_matrix t2;
    ae_matrix t3;
    ae_matrix ce;
    ae_vector xs0;
    ae_vector bl;
    ae_vector bu;
    ae_vector tmp;
    ae_vector x;
    ae_vector xstart;
    ae_vector xstart2;
    ae_vector xend;
    ae_vector xend2;
    ae_vector x0;
    ae_vector x1;
    ae_vector b;
    ae_vector b2;
    ae_vector g;
    ae_vector bndl;
    ae_vector bndu;
    ae_vector xorigin;
    ae_vector tmp0;
    ae_vector tmp1;
    ae_vector da;
    ae_vector ct;
    ae_vector nonnegative;
    double eps;
    minqpstate state;
    minqpstate state2;
    minqpreport rep;
    minqpreport rep2;
    ae_int_t i;
    ae_int_t j;
    ae_int_t pass;
    double v;
    double vv;
    double f0;
    double f1;
    double tolconstr;
    ae_int_t bscale;
    ae_int_t akind;
    ae_int_t shiftkind;
    ae_int_t ccnt;
    hqrndstate rs;
    snnlssolver nnls;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&q, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&t2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&t3, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ce, 0, 0, DT_REAL, _state);
    ae_vector_init(&xs0, 0, DT_REAL, _state);
    ae_vector_init(&bl, 0, DT_REAL, _state);
    ae_vector_init(&bu, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&xstart, 0, DT_REAL, _state);
    ae_vector_init(&xstart2, 0, DT_REAL, _state);
    ae_vector_init(&xend, 0, DT_REAL, _state);
    ae_vector_init(&xend2, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&b2, 0, DT_REAL, _state);
    ae_vector_init(&g, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    ae_vector_init(&xorigin, 0, DT_REAL, _state);
    ae_vector_init(&tmp0, 0, DT_REAL, _state);
    ae_vector_init(&tmp1, 0, DT_REAL, _state);
    ae_vector_init(&da, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    ae_vector_init(&nonnegative, 0, DT_BOOL, _state);
    _minqpstate_init(&state, _state);
    _minqpstate_init(&state2, _state);
    _minqpreport_init(&rep, _state);
    _minqpreport_init(&rep2, _state);
    _hqrndstate_init(&rs, _state);
    _snnlssolver_init(&nnls, _state);

    hqrndrandomize(&rs, _state);
    
    /*
     * Inequality constrained problem:
     * * N*N diagonal A
     * * one inequality constraint q'*x>=0, where q is random unit vector
     * * optimization problem has form 0.5*x'*A*x-(x1*A)*x,
     *   where x1 is some random vector
     * * either:
     *   a) x1 is feasible => we must stop at x1
     *   b) x1 is infeasible => we must stop at the boundary q'*x=0 and
     *      projection of gradient onto q*x=0 must be zero
     *
     * NOTE: we make several passes because some specific kind of errors is rarely
     *       caught by this test, so we need several repetitions.
     */
    eps = 1.0E-4;
    for(n=2; n<=6; n++)
    {
        for(pass=0; pass<=4; pass++)
        {
            
            /*
             * Generate problem: A, b, CMatrix, x0, XStart
             */
            spdmatrixrndcond(n, ae_pow(10.0, 3*ae_randomreal(_state), _state), &a, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&x1, n, _state);
            ae_vector_set_length(&xstart, n, _state);
            ae_matrix_set_length(&c, 1, n+1, _state);
            ae_vector_set_length(&ct, 1, _state);
            for(i=0; i<=n-1; i++)
            {
                x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                xstart.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            do
            {
                v = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    c.ptr.pp_double[0][i] = 2*ae_randomreal(_state)-1;
                    v = v+ae_sqr(c.ptr.pp_double[0][i], _state);
                }
                v = ae_sqrt(v, _state);
            }
            while(ae_fp_eq(v,(double)(0)));
            for(i=0; i<=n-1; i++)
            {
                c.ptr.pp_double[0][i] = c.ptr.pp_double[0][i]/v;
            }
            c.ptr.pp_double[0][n] = (double)(0);
            ct.ptr.p_int[0] = 1;
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                b.ptr.p_double[i] = -v;
            }
            
            /*
             * Create optimizer, solve
             */
            minqpcreate(n, &state, _state);
            testminqpunit_setrandomalgoconvexlc(&state, _state);
            minqpsetlinearterm(&state, &b, _state);
            minqpsetquadraticterm(&state, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
            minqpsetstartingpoint(&state, &xstart, _state);
            minqpsetlc(&state, &c, &ct, 1, _state);
            minqpoptimize(&state, _state);
            minqpresults(&state, &xend, &rep, _state);
            
            /*
             * Test
             */
            if( rep.terminationtype<=0 )
            {
                seterrorflag(err, ae_true, _state);
                continue;
            }
            v = ae_v_dotproduct(&x1.ptr.p_double[0], 1, &c.ptr.pp_double[0][0], 1, ae_v_len(0,n-1));
            if( ae_fp_greater_eq(v,(double)(0)) )
            {
                
                /*
                 * X1 is feasible
                 */
                for(i=0; i<=n-1; i++)
                {
                    seterrorflag(err, ae_fp_greater(ae_fabs(xend.ptr.p_double[i]-x1.ptr.p_double[i], _state),eps), _state);
                }
            }
            else
            {
                
                /*
                 * X1 is infeasible:
                 * * XEnd must be approximately feasible
                 * * gradient projection onto c'*x=0 must be zero
                 */
                v = ae_v_dotproduct(&xend.ptr.p_double[0], 1, &c.ptr.pp_double[0][0], 1, ae_v_len(0,n-1));
                seterrorflag(err, ae_fp_less(v,-eps), _state);
                ae_vector_set_length(&g, n, _state);
                ae_v_move(&g.ptr.p_double[0], 1, &b.ptr.p_double[0], 1, ae_v_len(0,n-1));
                for(i=0; i<=n-1; i++)
                {
                    v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xend.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    g.ptr.p_double[i] = g.ptr.p_double[i]+v;
                }
                v = ae_v_dotproduct(&g.ptr.p_double[0], 1, &c.ptr.pp_double[0][0], 1, ae_v_len(0,n-1));
                ae_v_subd(&g.ptr.p_double[0], 1, &c.ptr.pp_double[0][0], 1, ae_v_len(0,n-1), v);
                v = ae_v_dotproduct(&g.ptr.p_double[0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,n-1));
                seterrorflag(err, ae_fp_greater(ae_sqrt(v, _state),eps), _state);
            }
        }
    }
    
    /*
     * Boundary and linear equality/inequality constrained QP problem,
     * test for correct handling of non-zero XOrigin:
     * * N*N SPD A with moderate condtion number (up to 100)
     * * boundary constraints 0<=x[i]<=1
     * * K<N linear equality/inequality constraints Q*x = Q*x0, where
     *   Q is random orthogonal K*N matrix, x0 is some random vector from the
     *   inner area of the feasible hypercube (0.1<=x0[i]<=0.9)
     * * optimization problem has form 0.5*(x-xorigin)'*A*(x-xorigin)+b*(x-xorigin),
     *   where b is some random vector with -1<=b[i]<=+1.
     *   (sometimes solution is in the inner area, sometimes at the boundary)
     * * every component of the initial point XStart is random from [-1,1]
     * 
     * Solution of such problem is calculated using two methods:
     * a) QP with SetOrigin() call
     * b) QP with XOrigin explicitly added to the quadratic function,
     *
     * Both methods should give same results; any significant difference is
     * evidence of some error in the QP implementation.
     */
    eps = 1.0E-4;
    for(n=2; n<=6; n++)
    {
        for(k=1; k<=n-1; k++)
        {
            
            /*
             * Generate problem: A, b, BndL, BndU, CMatrix, x0, x1, XStart.
             * Additionally, we compute modified b: b2 = b-xorigin'*A
             */
            rmatrixrndorthogonal(n, &q, _state);
            spdmatrixrndcond(n, ae_pow(10.0, 2*ae_randomreal(_state), _state), &a, _state);
            ae_vector_set_length(&b, n, _state);
            ae_vector_set_length(&b2, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&x1, n, _state);
            ae_vector_set_length(&xorigin, n, _state);
            ae_vector_set_length(&xstart, n, _state);
            ae_matrix_set_length(&c, k, n+1, _state);
            ae_vector_set_length(&ct, k, _state);
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = 0.1+0.8*ae_randomreal(_state);
                b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                bndl.ptr.p_double[i] = 0.0;
                bndu.ptr.p_double[i] = 1.0;
                xstart.ptr.p_double[i] = (double)(ae_randominteger(2, _state));
                xorigin.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            for(i=0; i<=k-1; i++)
            {
                ae_v_move(&c.ptr.pp_double[i][0], 1, &q.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                v = ae_v_dotproduct(&q.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                c.ptr.pp_double[i][n] = v;
                ct.ptr.p_int[i] = ae_randominteger(3, _state)-1;
            }
            for(i=0; i<=n-1; i++)
            {
                v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xorigin.ptr.p_double[0], 1, ae_v_len(0,n-1));
                b2.ptr.p_double[i] = b.ptr.p_double[i]-v;
            }
            
            /*
             * Solve with SetOrigin() call
             */
            minqpcreate(n, &state, _state);
            testminqpunit_setrandomalgoconvexlc(&state, _state);
            minqpsetlinearterm(&state, &b, _state);
            minqpsetquadraticterm(&state, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
            minqpsetstartingpoint(&state, &xstart, _state);
            minqpsetorigin(&state, &xorigin, _state);
            minqpsetbc(&state, &bndl, &bndu, _state);
            minqpsetlc(&state, &c, &ct, k, _state);
            minqpoptimize(&state, _state);
            minqpresults(&state, &xend, &rep, _state);
            if( rep.terminationtype<=0 )
            {
                seterrorflag(err, ae_true, _state);
                continue;
            }
            
            /*
             * Solve problem using explicit origin
             */
            minqpcreate(n, &state2, _state);
            testminqpunit_setrandomalgoconvexlc(&state, _state);
            minqpsetlinearterm(&state2, &b2, _state);
            minqpsetquadraticterm(&state2, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
            minqpsetstartingpoint(&state2, &xstart, _state);
            minqpsetbc(&state2, &bndl, &bndu, _state);
            minqpsetlc(&state2, &c, &ct, k, _state);
            minqpoptimize(&state2, _state);
            minqpresults(&state2, &xend2, &rep, _state);
            if( rep.terminationtype<=0 )
            {
                seterrorflag(err, ae_true, _state);
                continue;
            }
            
            /*
             * Calculate function value and XEnd and XEnd2
             */
            f0 = 0.0;
            f1 = 0.0;
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    f0 = f0+0.5*(xend.ptr.p_double[i]-xorigin.ptr.p_double[i])*a.ptr.pp_double[i][j]*(xend.ptr.p_double[j]-xorigin.ptr.p_double[j]);
                    f1 = f1+0.5*(xend2.ptr.p_double[i]-xorigin.ptr.p_double[i])*a.ptr.pp_double[i][j]*(xend2.ptr.p_double[j]-xorigin.ptr.p_double[j]);
                }
                f0 = f0+(xend.ptr.p_double[i]-xorigin.ptr.p_double[i])*b.ptr.p_double[i];
                f1 = f1+(xend2.ptr.p_double[i]-xorigin.ptr.p_double[i])*b.ptr.p_double[i];
            }
            
            /*
             * Check feasibility properties and compare solutions
             */
            for(i=0; i<=k-1; i++)
            {
                v = ae_v_dotproduct(&xend.ptr.p_double[0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                if( ct.ptr.p_int[i]==0 )
                {
                    seterrorflag(err, ae_fp_greater(ae_fabs(v-c.ptr.pp_double[i][n], _state),eps), _state);
                }
                if( ct.ptr.p_int[i]>0 )
                {
                    seterrorflag(err, ae_fp_less(v,c.ptr.pp_double[i][n]-eps), _state);
                }
                if( ct.ptr.p_int[i]<0 )
                {
                    seterrorflag(err, ae_fp_greater(v,c.ptr.pp_double[i][n]+eps), _state);
                }
            }
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(err, ae_fp_greater(ae_fabs(xend.ptr.p_double[i]-xend2.ptr.p_double[i], _state),eps), _state);
                seterrorflag(err, ae_fp_less(xend.ptr.p_double[i],(double)(0)), _state);
                seterrorflag(err, ae_fp_greater(xend.ptr.p_double[i],(double)(1)), _state);
            }
        }
    }
    
    /*
     * Boundary constraints vs linear ones:
     * * N*N SPD A
     * * optimization problem has form 0.5*x'*A*x-(x1*A)*x,
     *   where x1 is some random vector from [-1,+1]
     * * K=2*N constraints of the form ai<=x[i] or x[i]<=b[i],
     *   with ai in [-1.0,-0.1], bi in [+0.1,+1.0]
     * * initial point xstart is from [-1,+2]
     * * we solve two related QP problems:
     *   a) one with constraints posed as boundary ones
     *   b) another one with same constraints posed as general linear ones
     * both problems must have same solution.
     * Here we test that boundary constrained and linear inequality constrained
     * solvers give same results.
     */
    eps = 1.0E-4;
    for(n=1; n<=6; n++)
    {
        
        /*
         * Generate problem: A, b, x0, XStart, C, CT
         */
        spdmatrixrndcond(n, ae_pow(10.0, 3*ae_randomreal(_state), _state), &a, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&x1, n, _state);
        ae_vector_set_length(&xstart, n, _state);
        ae_matrix_set_length(&c, 2*n, n+1, _state);
        ae_vector_set_length(&ct, 2*n, _state);
        ae_vector_set_length(&bndl, n, _state);
        ae_vector_set_length(&bndu, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            xstart.ptr.p_double[i] = 3*ae_randomreal(_state)-1;
            bndl.ptr.p_double[i] = -(0.1+0.9*ae_randomreal(_state));
            bndu.ptr.p_double[i] = 0.1+0.9*ae_randomreal(_state);
            for(j=0; j<=n-1; j++)
            {
                c.ptr.pp_double[2*i+0][j] = (double)(0);
                c.ptr.pp_double[2*i+1][j] = (double)(0);
            }
            c.ptr.pp_double[2*i+0][i] = (double)(1);
            c.ptr.pp_double[2*i+0][n] = bndl.ptr.p_double[i];
            ct.ptr.p_int[2*i+0] = 1;
            c.ptr.pp_double[2*i+1][i] = (double)(1);
            c.ptr.pp_double[2*i+1][n] = bndu.ptr.p_double[i];
            ct.ptr.p_int[2*i+1] = -1;
        }
        for(i=0; i<=n-1; i++)
        {
            v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,n-1));
            b.ptr.p_double[i] = -v;
        }
        
        /*
         * Solve linear inequality constrained problem
         */
        minqpcreate(n, &state, _state);
        testminqpunit_setrandomalgoconvexlc(&state, _state);
        minqpsetlinearterm(&state, &b, _state);
        minqpsetquadraticterm(&state, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
        minqpsetstartingpoint(&state, &xstart, _state);
        minqpsetlc(&state, &c, &ct, 2*n, _state);
        minqpoptimize(&state, _state);
        minqpresults(&state, &xend, &rep, _state);
        
        /*
         * Solve boundary constrained problem
         */
        minqpcreate(n, &state2, _state);
        testminqpunit_setrandomalgoconvexlc(&state, _state);
        minqpsetlinearterm(&state2, &b, _state);
        minqpsetquadraticterm(&state2, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
        minqpsetstartingpoint(&state2, &xstart, _state);
        minqpsetbc(&state2, &bndl, &bndu, _state);
        minqpoptimize(&state2, _state);
        minqpresults(&state2, &xend2, &rep2, _state);
        
        /*
         * Calculate gradient, check projection
         */
        if( rep.terminationtype<=0||rep2.terminationtype<=0 )
        {
            seterrorflag(err, ae_true, _state);
            continue;
        }
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(err, ae_fp_less(xend.ptr.p_double[i],bndl.ptr.p_double[i]-eps), _state);
            seterrorflag(err, ae_fp_greater(xend.ptr.p_double[i],bndu.ptr.p_double[i]+eps), _state);
            seterrorflag(err, ae_fp_greater(ae_fabs(xend.ptr.p_double[i]-xend2.ptr.p_double[i], _state),eps), _state);
        }
    }
    
    /*
     * Boundary constraints posed as general linear ones:
     * * no bound constraints
     * * 2*N linear constraints 0 <= x[i] <= 1
     * * preconditioner is chosen at random (we just want to be
     *   sure that preconditioning won't prevent us from converging
     *   to the feasible point):
     *   * unit preconditioner
     *   * random diagonal-based preconditioner
     *   * random scale-based preconditioner
     * * F(x) = |x-x0|^P, where P={2,4} and x0 is randomly selected from [-1,+2]^N
     * * with such simple constraints and function it is easy to find
     *   analytic form of solution: S[i] = bound(x0[i], 0, 1).
     * * however, we can't guarantee that solution is strictly feasible
     *   with respect to nonlinearity constraint, so we check
     *   for approximate feasibility.
     */
    for(n=1; n<=5; n++)
    {
        
        /*
         * Generate X, BL, BU.
         */
        ae_matrix_set_length(&a, n, n, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&xstart, n, _state);
        ae_vector_set_length(&x0, n, _state);
        ae_matrix_set_length(&c, 2*n, n+1, _state);
        ae_vector_set_length(&ct, 2*n, _state);
        for(i=0; i<=n-1; i++)
        {
            xstart.ptr.p_double[i] = ae_randomreal(_state);
            x0.ptr.p_double[i] = 3*ae_randomreal(_state)-1;
            b.ptr.p_double[i] = -x0.ptr.p_double[i];
            for(j=0; j<=n; j++)
            {
                c.ptr.pp_double[2*i+0][j] = (double)(0);
                c.ptr.pp_double[2*i+1][j] = (double)(0);
            }
            c.ptr.pp_double[2*i+0][i] = (double)(1);
            c.ptr.pp_double[2*i+0][n] = (double)(0);
            ct.ptr.p_int[2*i+0] = 1;
            c.ptr.pp_double[2*i+1][i] = (double)(1);
            c.ptr.pp_double[2*i+1][n] = (double)(1);
            ct.ptr.p_int[2*i+1] = -1;
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( i==j )
                {
                    a.ptr.pp_double[i][j] = (double)(1);
                }
                else
                {
                    a.ptr.pp_double[i][j] = (double)(0);
                }
            }
        }
        
        /*
         * Create and optimize
         */
        minqpcreate(n, &state, _state);
        testminqpunit_setrandomalgoconvexlc(&state, _state);
        minqpsetlc(&state, &c, &ct, 2*n, _state);
        minqpsetlinearterm(&state, &b, _state);
        minqpsetquadraticterm(&state, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
        minqpsetstartingpoint(&state, &xstart, _state);
        minqpoptimize(&state, _state);
        minqpresults(&state, &xend, &rep, _state);
        if( rep.terminationtype<=0 )
        {
            seterrorflag(err, ae_true, _state);
            continue;
        }
        
        /*
         * * compare solution with analytic one
         * * check feasibility
         */
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(err, ae_fp_greater(ae_fabs(xend.ptr.p_double[i]-boundval(x0.ptr.p_double[i], 0.0, 1.0, _state), _state),0.05), _state);
            seterrorflag(err, ae_fp_less(xend.ptr.p_double[i],0.0-1.0E-6), _state);
            seterrorflag(err, ae_fp_greater(xend.ptr.p_double[i],1.0+1.0E-6), _state);
        }
    }
    
    /*
     * Boundary and linear equality/inequality constrained QP problem with
     * excessive constraints:
     * * N*N SPD A with moderate condtion number (up to 100)
     * * boundary constraints 0<=x[i]<=1
     * * K=2*N equality/inequality constraints Q*x = Q*x0, where Q is random matrix,
     *   x0 is some random vector from the feasible hypercube (0.1<=x0[i]<=0.9)
     * * optimization problem has form 0.5*x'*A*x-b*x,
     *   where b is some random vector
     * * because constraints are excessive, the main problem is to find
     *   feasible point; usually, the only existing feasible point is solution,
     *   so we have to check only feasibility
     */
    eps = 1.0E-4;
    for(n=1; n<=6; n++)
    {
        
        /*
         * Generate problem: A, b, BndL, BndU, CMatrix, x0, x1, XStart
         */
        k = 2*n;
        spdmatrixrndcond(n, ae_pow(10.0, 3*ae_randomreal(_state), _state), &a, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&bndl, n, _state);
        ae_vector_set_length(&bndu, n, _state);
        ae_vector_set_length(&x0, n, _state);
        ae_vector_set_length(&x1, n, _state);
        ae_vector_set_length(&xstart, n, _state);
        ae_matrix_set_length(&c, k, n+1, _state);
        ae_vector_set_length(&ct, k, _state);
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = 0.1+0.8*ae_randomreal(_state);
            x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            bndl.ptr.p_double[i] = 0.0;
            bndu.ptr.p_double[i] = 1.0;
            xstart.ptr.p_double[i] = (double)(ae_randominteger(2, _state));
        }
        for(i=0; i<=k-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                c.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
            v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
            ct.ptr.p_int[i] = ae_randominteger(3, _state)-1;
            if( ct.ptr.p_int[i]==0 )
            {
                c.ptr.pp_double[i][n] = v;
            }
            if( ct.ptr.p_int[i]>0 )
            {
                c.ptr.pp_double[i][n] = v-1.0E-3;
            }
            if( ct.ptr.p_int[i]<0 )
            {
                c.ptr.pp_double[i][n] = v+1.0E-3;
            }
        }
        for(i=0; i<=n-1; i++)
        {
            b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        
        /*
         * Create optimizer, solve
         */
        minqpcreate(n, &state, _state);
        testminqpunit_setrandomalgoconvexlc(&state, _state);
        minqpsetlinearterm(&state, &b, _state);
        minqpsetquadraticterm(&state, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
        minqpsetstartingpoint(&state, &xstart, _state);
        minqpsetbc(&state, &bndl, &bndu, _state);
        minqpsetlc(&state, &c, &ct, k, _state);
        minqpoptimize(&state, _state);
        minqpresults(&state, &xend, &rep, _state);
        
        /*
         * Check feasibility properties of the solution
         */
        if( rep.terminationtype<=0 )
        {
            seterrorflag(err, ae_true, _state);
            continue;
        }
        for(i=0; i<=k-1; i++)
        {
            v = ae_v_dotproduct(&xend.ptr.p_double[0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
            if( ct.ptr.p_int[i]==0 )
            {
                seterrorflag(err, ae_fp_greater(ae_fabs(v-c.ptr.pp_double[i][n], _state),eps), _state);
            }
            if( ct.ptr.p_int[i]>0 )
            {
                seterrorflag(err, ae_fp_less(v,c.ptr.pp_double[i][n]-eps), _state);
            }
            if( ct.ptr.p_int[i]<0 )
            {
                seterrorflag(err, ae_fp_greater(v,c.ptr.pp_double[i][n]+eps), _state);
            }
        }
    }
    
    /*
     * General inequality constrained problem:
     * * N*N SPD diagonal A with moderate condtion number
     * * no boundary constraints
     * * K=N inequality constraints C*x >= C*x0, where C is N*N well conditioned
     *   matrix, x0 is some random vector [-1,+1]
     * * optimization problem has form 0.5*x'*A*x-b'*x,
     *   where b is random vector from [-1,+1]
     * * using duality, we can obtain solution of QP problem as follows:
     *   a) intermediate problem min(0.5*y'*B*y + d'*y) s.t. y>=0
     *      is solved, where B = C*inv(A)*C', d = -(C*inv(A)*b + C*x0)
     *   b) after we got dual solution ys, we calculate primal solution
     *      xs = inv(A)*(C'*ys-b)
     */
    eps = 1.0E-3;
    for(n=1; n<=6; n++)
    {
        
        /*
         * Generate problem
         */
        ae_vector_set_length(&da, n, _state);
        ae_matrix_set_length(&a, n, n, _state);
        rmatrixrndcond(n, ae_pow(10.0, 2*ae_randomreal(_state), _state), &t2, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&x0, n, _state);
        ae_vector_set_length(&xstart, n, _state);
        ae_matrix_set_length(&c, n, n+1, _state);
        ae_vector_set_length(&ct, n, _state);
        for(i=0; i<=n-1; i++)
        {
            da.ptr.p_double[i] = ae_exp(8*ae_randomreal(_state)-4, _state);
            for(j=0; j<=n-1; j++)
            {
                a.ptr.pp_double[i][j] = (double)(0);
            }
            a.ptr.pp_double[i][i] = da.ptr.p_double[i];
        }
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            xstart.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        for(i=0; i<=n-1; i++)
        {
            ae_v_move(&c.ptr.pp_double[i][0], 1, &t2.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
            v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
            c.ptr.pp_double[i][n] = v;
            ct.ptr.p_int[i] = 1;
        }
        
        /*
         * Solve primal problem, check feasibility
         */
        minqpcreate(n, &state, _state);
        testminqpunit_setrandomalgoconvexlc(&state, _state);
        minqpsetlinearterm(&state, &b, _state);
        minqpsetquadraticterm(&state, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
        minqpsetstartingpoint(&state, &xstart, _state);
        minqpsetlc(&state, &c, &ct, n, _state);
        minqpoptimize(&state, _state);
        minqpresults(&state, &xend, &rep, _state);
        if( rep.terminationtype<=0 )
        {
            seterrorflag(err, ae_true, _state);
            continue;
        }
        for(i=0; i<=n-1; i++)
        {
            v = ae_v_dotproduct(&xend.ptr.p_double[0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
            seterrorflag(err, ae_fp_less(v,c.ptr.pp_double[i][n]-eps), _state);
        }
        
        /*
         * Generate dual problem:
         * * A2 stores new quadratic term
         * * B2 stores new linear term
         * * BndL/BndU store boundary constraints
         */
        ae_matrix_set_length(&t3, n, n, _state);
        ae_matrix_set_length(&a2, n, n, _state);
        rmatrixtranspose(n, n, &c, 0, 0, &t3, 0, 0, _state);
        for(i=0; i<=n-1; i++)
        {
            v = 1/ae_sqrt(da.ptr.p_double[i], _state);
            ae_v_muld(&t3.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
        }
        rmatrixsyrk(n, n, 1.0, &t3, 0, 0, 2, 0.0, &a2, 0, 0, ae_true, _state);
        ae_vector_set_length(&tmp0, n, _state);
        ae_v_move(&tmp0.ptr.p_double[0], 1, &b.ptr.p_double[0], 1, ae_v_len(0,n-1));
        for(i=0; i<=n-1; i++)
        {
            tmp0.ptr.p_double[i] = tmp0.ptr.p_double[i]/da.ptr.p_double[i];
        }
        ae_vector_set_length(&b2, n, _state);
        for(i=0; i<=n-1; i++)
        {
            v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &tmp0.ptr.p_double[0], 1, ae_v_len(0,n-1));
            b2.ptr.p_double[i] = -(v+c.ptr.pp_double[i][n]);
        }
        ae_vector_set_length(&bndl, n, _state);
        ae_vector_set_length(&bndu, n, _state);
        for(i=0; i<=n-1; i++)
        {
            bndl.ptr.p_double[i] = 0.0;
            bndu.ptr.p_double[i] = _state->v_posinf;
        }
        minqpcreate(n, &state2, _state);
        testminqpunit_setrandomalgoconvexlc(&state, _state);
        minqpsetlinearterm(&state2, &b2, _state);
        minqpsetquadraticterm(&state2, &a2, ae_true, _state);
        minqpsetbc(&state2, &bndl, &bndu, _state);
        minqpoptimize(&state2, _state);
        minqpresults(&state2, &xend2, &rep2, _state);
        if( rep2.terminationtype<=0 )
        {
            seterrorflag(err, ae_true, _state);
            continue;
        }
        for(i=0; i<=n-1; i++)
        {
            v = ae_v_dotproduct(&c.ptr.pp_double[0][i], c.stride, &xend2.ptr.p_double[0], 1, ae_v_len(0,n-1));
            tmp0.ptr.p_double[i] = v-b.ptr.p_double[i];
        }
        for(i=0; i<=n-1; i++)
        {
            tmp0.ptr.p_double[i] = tmp0.ptr.p_double[i]/da.ptr.p_double[i];
        }
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(err, ae_fp_greater(ae_fabs(tmp0.ptr.p_double[i]-xend.ptr.p_double[i], _state),eps*ae_maxreal(ae_fabs(tmp0.ptr.p_double[i], _state), 1.0, _state)), _state);
        }
    }
    
    /*
     * Boundary and linear equality/inequality constrained QP problem,
     * test checks that different starting points yield same final point:
     * * random N from [1..6], random K from [1..2*N]
     * * N*N SPD A with moderate condtion number (up to 100)
     * * boundary constraints 0<=x[i]<=1
     * * K<2*N linear inequality constraints Q*x <= Q*x0, where
     *   Q is random K*N matrix, x0 is some random vector from the
     *   inner area of the feasible hypercube (0.1<=x0[i]<=0.9)
     * * optimization problem has form 0.5*x'*A*x+b*x,
     *   where b is some random vector with -5<=b[i]<=+5
     * * every component of the initial point XStart is random from [-2,+2]
     * * we perform two starts from random different XStart and compare values
     *   of the target function (although final points may be slightly different,
     *   function values should match each other)
     */
    eps = 1.0E-4;
    for(pass=1; pass<=50; pass++)
    {
        
        /*
         * Generate problem: N, K, A, b, BndL, BndU, CMatrix, x0, x1, XStart.
         */
        n = ae_randominteger(5, _state)+2;
        k = ae_randominteger(2*n, _state)+1;
        spdmatrixrndcond(n, ae_pow(10.0, 2*ae_randomreal(_state), _state), &a, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&b2, n, _state);
        ae_vector_set_length(&bndl, n, _state);
        ae_vector_set_length(&bndu, n, _state);
        ae_vector_set_length(&x0, n, _state);
        ae_vector_set_length(&xstart, n, _state);
        ae_vector_set_length(&xstart2, n, _state);
        ae_matrix_set_length(&c, k, n+1, _state);
        ae_vector_set_length(&ct, k, _state);
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = 0.1+0.8*ae_randomreal(_state);
            b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            bndl.ptr.p_double[i] = 0.0;
            bndu.ptr.p_double[i] = 1.0;
            xstart.ptr.p_double[i] = 4*ae_randomreal(_state)-2;
            xstart2.ptr.p_double[i] = 4*ae_randomreal(_state)-2;
        }
        for(i=0; i<=k-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                c.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
            v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &x0.ptr.p_double[0], 1, ae_v_len(0,n-1));
            c.ptr.pp_double[i][n] = v;
            ct.ptr.p_int[i] = ae_randominteger(3, _state)-1;
        }
        
        /*
         * Solve with XStart
         */
        minqpcreate(n, &state, _state);
        testminqpunit_setrandomalgoconvexlc(&state, _state);
        minqpsetlinearterm(&state, &b, _state);
        minqpsetquadraticterm(&state, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
        minqpsetstartingpoint(&state, &xstart, _state);
        minqpsetbc(&state, &bndl, &bndu, _state);
        minqpsetlc(&state, &c, &ct, k, _state);
        minqpoptimize(&state, _state);
        minqpresults(&state, &xend, &rep, _state);
        if( rep.terminationtype<=0 )
        {
            seterrorflag(err, ae_true, _state);
            continue;
        }
        
        /*
         * Solve with XStart2
         */
        minqpsetstartingpoint(&state, &xstart2, _state);
        minqpoptimize(&state, _state);
        minqpresults(&state, &xend2, &rep, _state);
        if( rep.terminationtype<=0 )
        {
            seterrorflag(err, ae_true, _state);
            continue;
        }
        
        /*
         * Calculate function value and XEnd and XEnd2, compare solutions
         */
        f0 = 0.0;
        f1 = 0.0;
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                f0 = f0+0.5*xend.ptr.p_double[i]*a.ptr.pp_double[i][j]*xend.ptr.p_double[j];
                f1 = f1+0.5*xend2.ptr.p_double[i]*a.ptr.pp_double[i][j]*xend2.ptr.p_double[j];
            }
            f0 = f0+xend.ptr.p_double[i]*b.ptr.p_double[i];
            f1 = f1+xend2.ptr.p_double[i]*b.ptr.p_double[i];
        }
        seterrorflag(err, ae_fp_greater(ae_fabs(f0-f1, _state),eps), _state);
    }
    
    /*
     * Convex/nonconvex optimization problem with excessive
     * (degenerate constraints):
     *
     * * N=2..5
     * * f = 0.5*x'*A*x+b'*x
     * * b has normally distributed entries with scale 10^BScale
     * * several kinds of A are tried: zero, well conditioned SPD, well conditioned indefinite, low rank
     * * box constraints: x[i] in [-1,+1]
     * * 2^N "excessive" general linear constraints (v_k,x)<=(v_k,v_k)+v_shift,
     *   where v_k is one of 2^N vertices of feasible hypercube, v_shift is
     *   a shift parameter:
     *   * with zero v_shift such constraints are degenerate (each vertex has
     *     N box constraints and one "redundant" linear constraint)
     *   * with positive v_shift linear constraint is always inactive
     *   * with small (about machine epsilon) but negative v_shift,
     *     constraint is close to degenerate - but not exactly
     *
     * We check that constrained gradient is close to zero at solution.
     * Box constraint is considered active if distance to boundary is less
     * than TolConstr.
     *
     * NOTE: TolConstr must be large enough so it won't conflict with
     *       perturbation introduced by v_shift
     */
    tolconstr = 1.0E-8;
    for(n=2; n<=8; n++)
    {
        for(akind=0; akind<=3; akind++)
        {
            for(shiftkind=-5; shiftkind<=1; shiftkind++)
            {
                for(bscale=0; bscale>=-2; bscale--)
                {
                    
                    /*
                     * Generate A, B and initial point
                     */
                    ae_matrix_set_length(&a, n, n, _state);
                    ae_vector_set_length(&b, n, _state);
                    ae_vector_set_length(&x, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        b.ptr.p_double[i] = ae_pow((double)(10), (double)(bscale), _state)*hqrndnormal(&rs, _state);
                        x.ptr.p_double[i] = hqrnduniformr(&rs, _state)-0.5;
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_double[i][j] = 0.0;
                        }
                    }
                    if( akind==1 )
                    {
                        
                        /*
                         * Dense well conditioned SPD
                         */
                        spdmatrixrndcond(n, 50.0, &a, _state);
                    }
                    if( akind==2 )
                    {
                        
                        /*
                         * Dense well conditioned indefinite
                         */
                        smatrixrndcond(n, 50.0, &a, _state);
                    }
                    if( akind==3 )
                    {
                        
                        /*
                         * Low rank
                         */
                        ae_vector_set_length(&tmp, n, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                a.ptr.pp_double[i][j] = 0.0;
                            }
                        }
                        for(k=1; k<=ae_minint(3, n-1, _state); k++)
                        {
                            for(i=0; i<=n-1; i++)
                            {
                                tmp.ptr.p_double[i] = hqrndnormal(&rs, _state);
                            }
                            v = hqrndnormal(&rs, _state);
                            for(i=0; i<=n-1; i++)
                            {
                                for(j=0; j<=n-1; j++)
                                {
                                    a.ptr.pp_double[i][j] = a.ptr.pp_double[i][j]+v*tmp.ptr.p_double[i]*tmp.ptr.p_double[j];
                                }
                            }
                        }
                    }
                    
                    /*
                     * Generate constraints
                     */
                    ae_vector_set_length(&bl, n, _state);
                    ae_vector_set_length(&bu, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        bl.ptr.p_double[i] = -1.0;
                        bu.ptr.p_double[i] = 1.0;
                    }
                    ccnt = ae_round(ae_pow((double)(2), (double)(n), _state), _state);
                    ae_matrix_set_length(&c, ccnt, n+1, _state);
                    ae_vector_set_length(&ct, ccnt, _state);
                    for(i=0; i<=ccnt-1; i++)
                    {
                        ct.ptr.p_int[i] = -1;
                        k = i;
                        c.ptr.pp_double[i][n] = ae_sign((double)(shiftkind), _state)*ae_pow((double)(10), ae_fabs((double)(shiftkind), _state), _state)*ae_machineepsilon;
                        for(j=0; j<=n-1; j++)
                        {
                            c.ptr.pp_double[i][j] = (double)(2*(k%2)-1);
                            c.ptr.pp_double[i][n] = c.ptr.pp_double[i][n]+c.ptr.pp_double[i][j]*c.ptr.pp_double[i][j];
                            k = k/2;
                        }
                    }
                    
                    /*
                     * Create and optimize
                     */
                    minqpcreate(n, &state, _state);
                    minqpsetstartingpoint(&state, &x, _state);
                    testminqpunit_setrandomalgononconvexlc(&state, _state);
                    minqpsetbc(&state, &bl, &bu, _state);
                    minqpsetlc(&state, &c, &ct, ccnt, _state);
                    minqpsetlinearterm(&state, &b, _state);
                    minqpsetquadraticterm(&state, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
                    minqpoptimize(&state, _state);
                    minqpresults(&state, &xs0, &rep, _state);
                    seterrorflag(err, rep.terminationtype<=0, _state);
                    if( *err )
                    {
                        ae_frame_leave(_state);
                        return;
                    }
                    
                    /*
                     * Evaluate gradient at solution and test
                     */
                    vv = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xs0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                        v = v+b.ptr.p_double[i];
                        if( ae_fp_less_eq(xs0.ptr.p_double[i],bl.ptr.p_double[i]+tolconstr)&&ae_fp_greater(v,(double)(0)) )
                        {
                            v = 0.0;
                        }
                        if( ae_fp_greater_eq(xs0.ptr.p_double[i],bu.ptr.p_double[i]-tolconstr)&&ae_fp_less(v,(double)(0)) )
                        {
                            v = 0.0;
                        }
                        vv = vv+ae_sqr(v, _state);
                    }
                    vv = ae_sqrt(vv, _state);
                    seterrorflag(err, ae_fp_greater(vv,1.0E-5), _state);
                }
            }
        }
    }
    
    /*
     * Convex/nonconvex optimization problem with combination of
     * box and linear constraints:
     *
     * * N=2..8
     * * f = 0.5*x'*A*x+b'*x
     * * b has normally distributed entries with scale 10^BScale
     * * several kinds of A are tried: zero, well conditioned SPD,
     *   well conditioned indefinite, low rank
     * * box constraints: x[i] in [-1,+1]
     * * initial point x0 = [0 0 ... 0 0]
     * * CCnt=min(3,N-1) general linear constraints of form (c,x)=0.
     *   random mix of equality/inequality constraints is tried.
     *   x0 is guaranteed to be feasible.
     *
     * We check that constrained gradient is close to zero at solution.
     * Inequality constraint is considered active if distance to boundary
     * is less than TolConstr. We use nonnegative least squares solver
     * in order to compute constrained gradient.
     */
    tolconstr = 1.0E-8;
    for(n=2; n<=8; n++)
    {
        for(akind=0; akind<=3; akind++)
        {
            for(bscale=0; bscale>=-2; bscale--)
            {
                
                /*
                 * Generate A, B and initial point
                 */
                ae_matrix_set_length(&a, n, n, _state);
                ae_vector_set_length(&b, n, _state);
                ae_vector_set_length(&x, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    b.ptr.p_double[i] = ae_pow((double)(10), (double)(bscale), _state)*hqrndnormal(&rs, _state);
                    x.ptr.p_double[i] = 0.0;
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        a.ptr.pp_double[i][j] = 0.0;
                    }
                }
                if( akind==1 )
                {
                    
                    /*
                     * Dense well conditioned SPD
                     */
                    spdmatrixrndcond(n, 50.0, &a, _state);
                }
                if( akind==2 )
                {
                    
                    /*
                     * Dense well conditioned indefinite
                     */
                    smatrixrndcond(n, 50.0, &a, _state);
                }
                if( akind==3 )
                {
                    
                    /*
                     * Low rank
                     */
                    ae_vector_set_length(&tmp, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_double[i][j] = 0.0;
                        }
                    }
                    for(k=1; k<=ae_minint(3, n-1, _state); k++)
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            tmp.ptr.p_double[i] = hqrndnormal(&rs, _state);
                        }
                        v = hqrndnormal(&rs, _state);
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=n-1; j++)
                            {
                                a.ptr.pp_double[i][j] = a.ptr.pp_double[i][j]+v*tmp.ptr.p_double[i]*tmp.ptr.p_double[j];
                            }
                        }
                    }
                }
                
                /*
                 * Generate constraints
                 */
                ae_vector_set_length(&bl, n, _state);
                ae_vector_set_length(&bu, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    bl.ptr.p_double[i] = -1.0;
                    bu.ptr.p_double[i] = 1.0;
                }
                ccnt = ae_minint(3, n-1, _state);
                ae_matrix_set_length(&c, ccnt, n+1, _state);
                ae_vector_set_length(&ct, ccnt, _state);
                for(i=0; i<=ccnt-1; i++)
                {
                    ct.ptr.p_int[i] = hqrnduniformi(&rs, 3, _state)-1;
                    c.ptr.pp_double[i][n] = 0.0;
                    for(j=0; j<=n-1; j++)
                    {
                        c.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                    }
                }
                
                /*
                 * Create and optimize
                 */
                minqpcreate(n, &state, _state);
                minqpsetstartingpoint(&state, &x, _state);
                testminqpunit_setrandomalgononconvexlc(&state, _state);
                minqpsetbc(&state, &bl, &bu, _state);
                minqpsetlc(&state, &c, &ct, ccnt, _state);
                minqpsetlinearterm(&state, &b, _state);
                minqpsetquadraticterm(&state, &a, ae_fp_greater(ae_randomreal(_state),0.5), _state);
                minqpoptimize(&state, _state);
                minqpresults(&state, &xs0, &rep, _state);
                seterrorflag(err, rep.terminationtype<=0, _state);
                if( *err )
                {
                    ae_frame_leave(_state);
                    return;
                }
                
                /*
                 * 1. evaluate unconstrained gradient at solution
                 *
                 * 2. calculate constrained gradient (NNLS solver is used
                 *    to evaluate gradient subject to active constraints).
                 *    In order to do this we form CE matrix, matrix of active
                 *    constraints (columns store constraint vectors). Then
                 *    we try to approximate gradient vector by columns of CE,
                 *    subject to non-negativity restriction placed on variables
                 *    corresponding to inequality constraints.
                 *
                 *    Residual from such regression is a constrained gradient vector.
                 */
                ae_vector_set_length(&g, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &xs0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    g.ptr.p_double[i] = v+b.ptr.p_double[i];
                }
                ae_matrix_set_length(&ce, n, n+ccnt, _state);
                ae_vector_set_length(&nonnegative, n+ccnt, _state);
                k = 0;
                for(i=0; i<=n-1; i++)
                {
                    seterrorflag(err, ae_fp_less(xs0.ptr.p_double[i],bl.ptr.p_double[i]), _state);
                    seterrorflag(err, ae_fp_greater(xs0.ptr.p_double[i],bu.ptr.p_double[i]), _state);
                    if( ae_fp_less_eq(xs0.ptr.p_double[i],bl.ptr.p_double[i]+tolconstr) )
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            ce.ptr.pp_double[j][k] = 0.0;
                        }
                        ce.ptr.pp_double[i][k] = 1.0;
                        nonnegative.ptr.p_bool[k] = ae_true;
                        inc(&k, _state);
                        continue;
                    }
                    if( ae_fp_greater_eq(xs0.ptr.p_double[i],bu.ptr.p_double[i]-tolconstr) )
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            ce.ptr.pp_double[j][k] = 0.0;
                        }
                        ce.ptr.pp_double[i][k] = -1.0;
                        nonnegative.ptr.p_bool[k] = ae_true;
                        inc(&k, _state);
                        continue;
                    }
                }
                for(i=0; i<=ccnt-1; i++)
                {
                    v = ae_v_dotproduct(&c.ptr.pp_double[i][0], 1, &xs0.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    v = v-c.ptr.pp_double[i][n];
                    seterrorflag(err, ct.ptr.p_int[i]==0&&ae_fp_greater(ae_fabs(v, _state),tolconstr), _state);
                    seterrorflag(err, ct.ptr.p_int[i]>0&&ae_fp_less(v,-tolconstr), _state);
                    seterrorflag(err, ct.ptr.p_int[i]<0&&ae_fp_greater(v,tolconstr), _state);
                    if( ct.ptr.p_int[i]==0 )
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            ce.ptr.pp_double[j][k] = c.ptr.pp_double[i][j];
                        }
                        nonnegative.ptr.p_bool[k] = ae_false;
                        inc(&k, _state);
                        continue;
                    }
                    if( (ct.ptr.p_int[i]>0&&ae_fp_less_eq(v,tolconstr))||(ct.ptr.p_int[i]<0&&ae_fp_greater_eq(v,-tolconstr)) )
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            ce.ptr.pp_double[j][k] = ae_sign((double)(ct.ptr.p_int[i]), _state)*c.ptr.pp_double[i][j];
                        }
                        nonnegative.ptr.p_bool[k] = ae_true;
                        inc(&k, _state);
                        continue;
                    }
                }
                snnlsinit(0, 0, 0, &nnls, _state);
                snnlssetproblem(&nnls, &ce, &g, 0, k, n, _state);
                for(i=0; i<=k-1; i++)
                {
                    if( !nonnegative.ptr.p_bool[i] )
                    {
                        snnlsdropnnc(&nnls, i, _state);
                    }
                }
                snnlssolve(&nnls, &tmp, _state);
                for(i=0; i<=k-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        g.ptr.p_double[j] = g.ptr.p_double[j]-tmp.ptr.p_double[i]*ce.ptr.pp_double[j][i];
                    }
                }
                vv = ae_v_dotproduct(&g.ptr.p_double[0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,n-1));
                vv = ae_sqrt(vv, _state);
                seterrorflag(err, ae_fp_greater(vv,1.0E-5), _state);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function tests special inequality constrained QP problems.

Returns True on errors.
*************************************************************************/
static ae_bool testminqpunit_specialicqptests(ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix c;
    ae_vector xstart;
    ae_vector xend;
    ae_vector xexact;
    ae_vector b;
    ae_vector bndl;
    ae_vector bndu;
    ae_vector ct;
    minqpstate state;
    minqpreport rep;
    ae_bool waserrors;
    ae_int_t i;
    ae_int_t j;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_vector_init(&xstart, 0, DT_REAL, _state);
    ae_vector_init(&xend, 0, DT_REAL, _state);
    ae_vector_init(&xexact, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    _minqpstate_init(&state, _state);
    _minqpreport_init(&rep, _state);

    waserrors = ae_false;
    
    /*
     * Test 1: reported by Vanderlande Industries.
     *         Tests algorithm ability to handle degenerate constraints.
     */
    ae_matrix_set_length(&a, 3, 3, _state);
    for(i=0; i<=2; i++)
    {
        for(j=0; j<=2; j++)
        {
            a.ptr.pp_double[i][j] = (double)(0);
        }
    }
    for(i=0; i<=2; i++)
    {
        a.ptr.pp_double[i][i] = (double)(1);
    }
    ae_vector_set_length(&b, 3, _state);
    b.ptr.p_double[0] = (double)(-50);
    b.ptr.p_double[1] = (double)(-50);
    b.ptr.p_double[2] = (double)(-75);
    ae_vector_set_length(&bndl, 3, _state);
    bndl.ptr.p_double[0] = (double)(0);
    bndl.ptr.p_double[1] = (double)(0);
    bndl.ptr.p_double[2] = (double)(0);
    ae_vector_set_length(&bndu, 3, _state);
    bndu.ptr.p_double[0] = (double)(100);
    bndu.ptr.p_double[1] = (double)(100);
    bndu.ptr.p_double[2] = (double)(150);
    ae_vector_set_length(&xstart, 3, _state);
    xstart.ptr.p_double[0] = (double)(0);
    xstart.ptr.p_double[1] = (double)(100);
    xstart.ptr.p_double[2] = (double)(0);
    ae_vector_set_length(&xexact, 3, _state);
    xexact.ptr.p_double[0] = (double)(0);
    xexact.ptr.p_double[1] = (double)(100);
    xexact.ptr.p_double[2] = (double)(50);
    ae_matrix_set_length(&c, 3, 4, _state);
    c.ptr.pp_double[0][0] = (double)(1);
    c.ptr.pp_double[0][1] = (double)(-1);
    c.ptr.pp_double[0][2] = (double)(0);
    c.ptr.pp_double[0][3] = (double)(-100);
    c.ptr.pp_double[1][0] = (double)(1);
    c.ptr.pp_double[1][1] = (double)(0);
    c.ptr.pp_double[1][2] = (double)(-1);
    c.ptr.pp_double[1][3] = (double)(0);
    c.ptr.pp_double[2][0] = (double)(-1);
    c.ptr.pp_double[2][1] = (double)(0);
    c.ptr.pp_double[2][2] = (double)(1);
    c.ptr.pp_double[2][3] = (double)(50);
    ae_vector_set_length(&ct, 3, _state);
    ct.ptr.p_int[0] = -1;
    ct.ptr.p_int[1] = -1;
    ct.ptr.p_int[2] = -1;
    minqpcreate(3, &state, _state);
    testminqpunit_setrandomalgoconvexlc(&state, _state);
    minqpsetlinearterm(&state, &b, _state);
    minqpsetquadraticterm(&state, &a, ae_true, _state);
    minqpsetstartingpoint(&state, &xstart, _state);
    minqpsetbc(&state, &bndl, &bndu, _state);
    minqpsetlc(&state, &c, &ct, 3, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend, &rep, _state);
    if( rep.terminationtype>0 )
    {
        for(i=0; i<=2; i++)
        {
            waserrors = waserrors||ae_fp_greater(ae_fabs(xend.ptr.p_double[i]-xexact.ptr.p_double[i], _state),1.0E6*ae_machineepsilon);
        }
    }
    else
    {
        waserrors = ae_true;
    }
    
    /*
     * Test 2: reported by Vanderlande Industries.
     *         Tests algorithm ability to handle degenerate constraints.
     */
    ae_matrix_set_length(&a, 3, 3, _state);
    for(i=0; i<=2; i++)
    {
        for(j=0; j<=2; j++)
        {
            a.ptr.pp_double[i][j] = (double)(0);
        }
    }
    for(i=0; i<=2; i++)
    {
        a.ptr.pp_double[i][i] = (double)(1);
    }
    ae_vector_set_length(&b, 3, _state);
    b.ptr.p_double[0] = (double)(-50);
    b.ptr.p_double[1] = (double)(-50);
    b.ptr.p_double[2] = (double)(-75);
    ae_vector_set_length(&bndl, 3, _state);
    bndl.ptr.p_double[0] = (double)(0);
    bndl.ptr.p_double[1] = (double)(0);
    bndl.ptr.p_double[2] = (double)(0);
    ae_vector_set_length(&bndu, 3, _state);
    bndu.ptr.p_double[0] = (double)(100);
    bndu.ptr.p_double[1] = (double)(100);
    bndu.ptr.p_double[2] = (double)(150);
    ae_vector_set_length(&xstart, 3, _state);
    xstart.ptr.p_double[0] = (double)(0);
    xstart.ptr.p_double[1] = (double)(100);
    xstart.ptr.p_double[2] = (double)(150);
    ae_vector_set_length(&xexact, 3, _state);
    xexact.ptr.p_double[0] = (double)(0);
    xexact.ptr.p_double[1] = (double)(100);
    xexact.ptr.p_double[2] = (double)(100);
    ae_matrix_set_length(&c, 3, 4, _state);
    c.ptr.pp_double[0][0] = (double)(1);
    c.ptr.pp_double[0][1] = (double)(-1);
    c.ptr.pp_double[0][2] = (double)(0);
    c.ptr.pp_double[0][3] = (double)(-100);
    c.ptr.pp_double[1][0] = (double)(0);
    c.ptr.pp_double[1][1] = (double)(1);
    c.ptr.pp_double[1][2] = (double)(-1);
    c.ptr.pp_double[1][3] = (double)(0);
    c.ptr.pp_double[2][0] = (double)(0);
    c.ptr.pp_double[2][1] = (double)(-1);
    c.ptr.pp_double[2][2] = (double)(1);
    c.ptr.pp_double[2][3] = (double)(50);
    ae_vector_set_length(&ct, 3, _state);
    ct.ptr.p_int[0] = -1;
    ct.ptr.p_int[1] = -1;
    ct.ptr.p_int[2] = -1;
    minqpcreate(3, &state, _state);
    testminqpunit_setrandomalgoconvexlc(&state, _state);
    minqpsetlinearterm(&state, &b, _state);
    minqpsetquadraticterm(&state, &a, ae_true, _state);
    minqpsetstartingpoint(&state, &xstart, _state);
    minqpsetbc(&state, &bndl, &bndu, _state);
    minqpsetlc(&state, &c, &ct, 3, _state);
    minqpoptimize(&state, _state);
    minqpresults(&state, &xend, &rep, _state);
    if( rep.terminationtype>0 )
    {
        for(i=0; i<=2; i++)
        {
            waserrors = waserrors||ae_fp_greater(ae_fabs(xend.ptr.p_double[i]-xexact.ptr.p_double[i], _state),1.0E6*ae_machineepsilon);
        }
    }
    else
    {
        waserrors = ae_true;
    }
    result = waserrors;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function normal
*************************************************************************/
static double testminqpunit_projectedantigradnorm(ae_int_t n,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* g,
     /* Real    */ ae_vector* bndl,
     /* Real    */ ae_vector* bndu,
     ae_state *_state)
{
    ae_int_t i;
    double r;
    double result;


    r = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        ae_assert(ae_fp_greater_eq(x->ptr.p_double[i],bndl->ptr.p_double[i])&&ae_fp_less_eq(x->ptr.p_double[i],bndu->ptr.p_double[i]), "ProjectedAntiGradNormal: boundary constraints violation", _state);
        if( ((ae_fp_greater(x->ptr.p_double[i],bndl->ptr.p_double[i])&&ae_fp_less(x->ptr.p_double[i],bndu->ptr.p_double[i]))||(ae_fp_eq(x->ptr.p_double[i],bndl->ptr.p_double[i])&&ae_fp_greater(-g->ptr.p_double[i],(double)(0))))||(ae_fp_eq(x->ptr.p_double[i],bndu->ptr.p_double[i])&&ae_fp_less(-g->ptr.p_double[i],(double)(0))) )
        {
            r = r+g->ptr.p_double[i]*g->ptr.p_double[i];
        }
    }
    result = ae_sqrt(r, _state);
    return result;
}


/*************************************************************************
This function tests that norm of bound-constrained gradient at point X is
less than Eps:
* unconstrained gradient is A*x+b
* if I-th component is at the boundary, and antigradient points outside of
  the feasible area, I-th component of constrained gradient is zero

This function accepts QP terms A and B, bound constraints, current point,
and performs test. Additionally, it checks that point is feasible w.r.t.
boundary constraints.

In case of failure, error flag is set. Otherwise, it is not modified.

IMPORTANT: this function does NOT use SetErrorFlag() to modify flag.
           If you want to use SetErrorFlag() for easier tracking of errors,
           you should store flag returned by this function into separate
           variable TmpFlag and call SetErrorFlag(ErrorFlag, TmpFlag) yourself.
*************************************************************************/
static void testminqpunit_testbcgradandfeasibility(/* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* bndl,
     /* Real    */ ae_vector* bndu,
     ae_int_t n,
     /* Real    */ ae_vector* x,
     double eps,
     ae_bool* errorflag,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double g;
    double gnorm;


    gnorm = 0.0;
    for(i=0; i<=n-1; i++)
    {
        g = b->ptr.p_double[i];
        for(j=0; j<=n-1; j++)
        {
            g = g+a->ptr.pp_double[i][j]*x->ptr.p_double[j];
        }
        if( ae_fp_eq(x->ptr.p_double[i],bndl->ptr.p_double[i])&&ae_fp_greater(g,(double)(0)) )
        {
            g = (double)(0);
        }
        if( ae_fp_eq(x->ptr.p_double[i],bndu->ptr.p_double[i])&&ae_fp_less(g,(double)(0)) )
        {
            g = (double)(0);
        }
        gnorm = gnorm+ae_sqr(g, _state);
        if( ae_fp_less(x->ptr.p_double[i],bndl->ptr.p_double[i]) )
        {
            *errorflag = ae_true;
        }
        if( ae_fp_greater(x->ptr.p_double[i],bndu->ptr.p_double[i]) )
        {
            *errorflag = ae_true;
        }
    }
    gnorm = ae_sqrt(gnorm, _state);
    if( ae_fp_greater(gnorm,eps) )
    {
        *errorflag = ae_true;
    }
}


/*************************************************************************
set random type of the QP solver.
All "modern" solvers can be chosen.
*************************************************************************/
static void testminqpunit_setrandomalgoallmodern(minqpstate* s,
     ae_state *_state)
{
    ae_int_t i;


    i = 1+ae_randominteger(2, _state);
    if( i==1 )
    {
        minqpsetalgobleic(s, 1.0E-12, 0.0, 0.0, 0, _state);
    }
    if( i==2 )
    {
        minqpsetalgoquickqp(s, 1.0E-12, 0.0, 0.0, 0, ae_fp_greater(ae_randomreal(_state),0.5), _state);
    }
}


/*************************************************************************
set random type of theQP solver
*************************************************************************/
static void testminqpunit_setrandomalgononconvex(minqpstate* s,
     ae_state *_state)
{
    ae_int_t i;


    i = 1+ae_randominteger(2, _state);
    if( i==1 )
    {
        minqpsetalgobleic(s, 1.0E-12, 0.0, 0.0, 0, _state);
    }
    if( i==2 )
    {
        minqpsetalgoquickqp(s, 1.0E-12, 0.0, 0.0, 0, ae_fp_greater(ae_randomreal(_state),0.5), _state);
    }
}


/*************************************************************************
set random type of theQP solver
*************************************************************************/
static void testminqpunit_setrandomalgosemidefinite(minqpstate* s,
     ae_state *_state)
{
    ae_int_t i;


    i = 1+ae_randominteger(2, _state);
    if( i==1 )
    {
        minqpsetalgobleic(s, 1.0E-12, 0.0, 0.0, 0, _state);
    }
    if( i==2 )
    {
        minqpsetalgoquickqp(s, 1.0E-12, 0.0, 0.0, 0, ae_fp_greater(ae_randomreal(_state),0.5), _state);
    }
}


/*************************************************************************
set random type of the QP solver, must support boundary constraints
*************************************************************************/
static void testminqpunit_setrandomalgobc(minqpstate* s, ae_state *_state)
{
    ae_int_t i;


    i = ae_randominteger(2, _state);
    if( i==0 )
    {
        minqpsetalgocholesky(s, _state);
    }
    if( i==1 )
    {
        minqpsetalgobleic(s, 1.0E-12, 0.0, 0.0, 0, _state);
    }
}


/*************************************************************************
set random type of the QP solver,
must support convex problems with boundary/linear constraints
*************************************************************************/
static void testminqpunit_setrandomalgoconvexlc(minqpstate* s,
     ae_state *_state)
{
    ae_int_t i;


    i = ae_randominteger(2, _state);
    if( i==0 )
    {
        minqpsetalgocholesky(s, _state);
    }
    if( i==1 )
    {
        minqpsetalgobleic(s, 1.0E-12, 0.0, 0.0, 0, _state);
    }
}


/*************************************************************************
set random type of the QP solver,
must support nonconvex problems with boundary/linear constraints
*************************************************************************/
static void testminqpunit_setrandomalgononconvexlc(minqpstate* s,
     ae_state *_state)
{
    ae_int_t i;


    i = ae_randominteger(1, _state);
    if( i==0 )
    {
        minqpsetalgobleic(s, 1.0E-12, 0.0, 0.0, 0, _state);
    }
}


/*************************************************************************
Convert dense matrix to sparse matrix using random format
*************************************************************************/
static void testminqpunit_densetosparse(/* Real    */ ae_matrix* a,
     ae_int_t n,
     sparsematrix* s,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    sparsematrix s0;

    ae_frame_make(_state, &_frame_block);
    _sparsematrix_clear(s);
    _sparsematrix_init(&s0, _state);

    sparsecreate(n, n, n*n, &s0, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            sparseset(&s0, i, j, a->ptr.pp_double[i][j], _state);
        }
    }
    sparsecopytobuf(&s0, ae_randominteger(3, _state), s, _state);
    ae_frame_leave(_state);
}


/*$ End $*/
