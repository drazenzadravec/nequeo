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
#include "testminnsunit.h"


/*$ Declarations $*/
static double testminnsunit_scalingtesttol = 1.0E-6;
static ae_int_t testminnsunit_scalingtestcnt = 5;
static void testminnsunit_basictest0uc(ae_bool* errors, ae_state *_state);
static void testminnsunit_basictest1uc(ae_bool* errors, ae_state *_state);
static void testminnsunit_basictest0bc(ae_bool* errors, ae_state *_state);
static void testminnsunit_basictest1bc(ae_bool* errors, ae_state *_state);
static void testminnsunit_basictest0lc(ae_bool* errors, ae_state *_state);
static void testminnsunit_basictest1lc(ae_bool* errors, ae_state *_state);
static void testminnsunit_basictest0nlc(ae_bool* errors, ae_state *_state);
static void testminnsunit_testuc(ae_bool* primaryerrors,
     ae_bool* othererrors,
     ae_state *_state);
static void testminnsunit_testbc(ae_bool* primaryerrors,
     ae_bool* othererrors,
     ae_state *_state);
static void testminnsunit_testlc(ae_bool* primaryerrors,
     ae_bool* othererrors,
     ae_state *_state);
static void testminnsunit_testnlc(ae_bool* primaryerrors,
     ae_bool* othererrors,
     ae_state *_state);
static void testminnsunit_testother(ae_bool* othererrors,
     ae_state *_state);


/*$ Body $*/


ae_bool testminns(ae_bool silent, ae_state *_state)
{
    ae_bool wereerrors;
    ae_bool ucerrors;
    ae_bool bcerrors;
    ae_bool lcerrors;
    ae_bool nlcerrors;
    ae_bool othererrors;
    ae_bool result;


    wereerrors = ae_false;
    ucerrors = ae_false;
    bcerrors = ae_false;
    lcerrors = ae_false;
    nlcerrors = ae_false;
    othererrors = ae_false;
    
    /*
     * Basic tests
     */
    testminnsunit_basictest0nlc(&nlcerrors, _state);
    testminnsunit_basictest0uc(&ucerrors, _state);
    testminnsunit_basictest1uc(&ucerrors, _state);
    testminnsunit_basictest0bc(&bcerrors, _state);
    testminnsunit_basictest1bc(&bcerrors, _state);
    testminnsunit_basictest0lc(&lcerrors, _state);
    testminnsunit_basictest1lc(&lcerrors, _state);
    
    /*
     * Special tests
     */
    testminnsunit_testother(&othererrors, _state);
    
    /*
     * Full scale tests
     */
    testminnsunit_testuc(&ucerrors, &othererrors, _state);
    testminnsunit_testbc(&bcerrors, &othererrors, _state);
    testminnsunit_testlc(&lcerrors, &othererrors, _state);
    testminnsunit_testnlc(&nlcerrors, &othererrors, _state);
    
    /*
     * end
     */
    wereerrors = (((ucerrors||bcerrors)||lcerrors)||nlcerrors)||othererrors;
    if( !silent )
    {
        printf("TESTING MINNS OPTIMIZATION\n");
        printf("TESTS:\n");
        printf("* UNCONSTRAINED                           ");
        if( ucerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* BOUND CONSTRAINED                       ");
        if( bcerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* LINEARLY CONSTRAINED                    ");
        if( lcerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* NONLINEARLY CONSTRAINED                 ");
        if( nlcerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* OTHER PROPERTIES                        ");
        if( othererrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        if( wereerrors )
        {
            printf("TEST FAILED\n");
        }
        else
        {
            printf("TEST PASSED\n");
        }
        printf("\n\n");
    }
    result = !wereerrors;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testminns(ae_bool silent, ae_state *_state)
{
    return testminns(silent, _state);
}


/*************************************************************************
Basic unconstrained test
*************************************************************************/
static void testminnsunit_basictest0uc(ae_bool* errors, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t i;
    ae_vector x0;
    ae_vector x1;
    ae_vector d;
    minnsstate s;
    minnsreport rep;
    double sumits;
    double sumnfev;
    ae_int_t pass;
    ae_int_t passcount;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    _minnsstate_init(&s, _state);
    _minnsreport_init(&rep, _state);

    n = 5;
    passcount = 10;
    sumits = (double)(0);
    sumnfev = (double)(0);
    ae_vector_set_length(&x0, n, _state);
    ae_vector_set_length(&d, n, _state);
    for(pass=1; pass<=10; pass++)
    {
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            d.ptr.p_double[i] = ae_pow((double)(10), 2*ae_randomreal(_state)-1, _state);
        }
        minnscreate(n, &x0, &s, _state);
        minnssetalgoags(&s, 0.1, 0.0, _state);
        while(minnsiteration(&s, _state))
        {
            if( s.needfij )
            {
                s.fi.ptr.p_double[0] = 0.0;
                for(i=0; i<=n-1; i++)
                {
                    s.fi.ptr.p_double[0] = s.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_fabs(s.x.ptr.p_double[i], _state);
                    s.j.ptr.pp_double[0][i] = d.ptr.p_double[i]*ae_sign(s.x.ptr.p_double[i], _state);
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnsresults(&s, &x1, &rep, _state);
        seterrorflag(errors, rep.terminationtype<=0, _state);
        if( *errors )
        {
            ae_frame_leave(_state);
            return;
        }
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(errors, !ae_isfinite(x1.ptr.p_double[i], _state)||ae_fp_greater(ae_fabs(x1.ptr.p_double[i], _state),0.001), _state);
        }
        sumits = sumits+(double)rep.iterationscount/(double)passcount;
        sumnfev = sumnfev+(double)rep.nfev/(double)passcount;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Basic unconstrained test: nonsmooth Rosenbrock posed as unconstrained problem.

             [                                                                       ]
    minimize [ 10*|x0^2-x1| + (1-x0)^2 + 100*max(sqrt(2)*x0-1,0) + 100*max(2*x1-1,0) ]
             [                                                                       ]

It's exact solution is x0=1/sqrt(2), x1=1/2
*************************************************************************/
static void testminnsunit_basictest1uc(ae_bool* errors, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    double v0;
    double v1;
    ae_vector x0;
    ae_vector x1;
    minnsstate s;
    minnsreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    _minnsstate_init(&s, _state);
    _minnsreport_init(&rep, _state);

    n = 2;
    ae_vector_set_length(&x0, n, _state);
    x0.ptr.p_double[0] = (double)(0);
    x0.ptr.p_double[1] = (double)(0);
    minnscreate(n, &x0, &s, _state);
    minnssetalgoags(&s, 0.1, 0.0, _state);
    while(minnsiteration(&s, _state))
    {
        if( s.needfij )
        {
            v0 = s.x.ptr.p_double[0];
            v1 = s.x.ptr.p_double[1];
            s.fi.ptr.p_double[0] = 10*ae_fabs(ae_sqr(v0, _state)-v1, _state)+ae_sqr(v0-1, _state);
            s.j.ptr.pp_double[0][0] = 10*ae_sign(ae_sqr(v0, _state)-v1, _state)*2*v0+2*(v0-1);
            s.j.ptr.pp_double[0][1] = (double)(10*ae_sign(ae_sqr(v0, _state)-v1, _state)*(-1));
            if( ae_fp_greater(ae_sqrt((double)(2), _state)*v0-1,0.0) )
            {
                s.fi.ptr.p_double[0] = s.fi.ptr.p_double[0]+100*(ae_sqrt((double)(2), _state)*v0-1);
                s.j.ptr.pp_double[0][0] = s.j.ptr.pp_double[0][0]+100*ae_sqrt((double)(2), _state);
            }
            if( ae_fp_greater(2*v1-1,0.0) )
            {
                s.fi.ptr.p_double[0] = s.fi.ptr.p_double[0]+100*(2*v1-1);
                s.j.ptr.pp_double[0][1] = s.j.ptr.pp_double[0][1]+100*2;
            }
            continue;
        }
        ae_assert(ae_false, "Assertion failed", _state);
    }
    minnsresults(&s, &x1, &rep, _state);
    seterrorflag(errors, rep.terminationtype<=0, _state);
    if( *errors )
    {
        ae_frame_leave(_state);
        return;
    }
    seterrorflag(errors, !ae_isfinite(x1.ptr.p_double[0], _state)||ae_fp_greater(ae_fabs(x1.ptr.p_double[0]-1/ae_sqrt((double)(2), _state), _state),0.001), _state);
    seterrorflag(errors, !ae_isfinite(x1.ptr.p_double[1], _state)||ae_fp_greater(ae_fabs(x1.ptr.p_double[1]-(double)1/(double)2, _state),0.001), _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Basic box constrained test
*************************************************************************/
static void testminnsunit_basictest0bc(ae_bool* errors, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t i;
    ae_vector x0;
    ae_vector x1;
    ae_vector d;
    ae_vector bl;
    ae_vector bu;
    minnsstate s;
    minnsreport rep;
    double sumits;
    double sumnfev;
    ae_int_t pass;
    ae_int_t passcount;
    double v0;
    double v1;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&bl, 0, DT_REAL, _state);
    ae_vector_init(&bu, 0, DT_REAL, _state);
    _minnsstate_init(&s, _state);
    _minnsreport_init(&rep, _state);

    n = 5;
    passcount = 10;
    sumits = (double)(0);
    sumnfev = (double)(0);
    ae_vector_set_length(&x0, n, _state);
    ae_vector_set_length(&bl, n, _state);
    ae_vector_set_length(&bu, n, _state);
    ae_vector_set_length(&d, n, _state);
    for(pass=1; pass<=10; pass++)
    {
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            d.ptr.p_double[i] = ae_pow((double)(10), 2*ae_randomreal(_state)-1, _state);
            v0 = 2*ae_randomreal(_state)-1;
            v1 = 2*ae_randomreal(_state)-1;
            bl.ptr.p_double[i] = ae_minreal(v0, v1, _state);
            bu.ptr.p_double[i] = ae_maxreal(v0, v1, _state);
        }
        minnscreate(n, &x0, &s, _state);
        minnssetalgoags(&s, 0.1, 0.0, _state);
        minnssetbc(&s, &bl, &bu, _state);
        while(minnsiteration(&s, _state))
        {
            if( s.needfij )
            {
                s.fi.ptr.p_double[0] = 0.0;
                for(i=0; i<=n-1; i++)
                {
                    s.fi.ptr.p_double[0] = s.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_fabs(s.x.ptr.p_double[i], _state);
                    s.j.ptr.pp_double[0][i] = d.ptr.p_double[i]*ae_sign(s.x.ptr.p_double[i], _state);
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnsresults(&s, &x1, &rep, _state);
        seterrorflag(errors, rep.terminationtype<=0, _state);
        if( *errors )
        {
            ae_frame_leave(_state);
            return;
        }
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(errors, !ae_isfinite(x1.ptr.p_double[i], _state)||ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-boundval(0.0, bl.ptr.p_double[i], bu.ptr.p_double[i], _state), _state),0.001), _state);
        }
        sumits = sumits+(double)rep.iterationscount/(double)passcount;
        sumnfev = sumnfev+(double)rep.nfev/(double)passcount;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Basic constrained test: nonsmooth Rosenbrock posed as box constrained problem.

             [                         ]
    minimize [ 10*|x0^2-x1| + (1-x0)^2 ]
             [                         ]
             
    s.t. x0<=1/sqrt(2), x1<=0.5

It's exact solution is x0=1/sqrt(2), x1=1/2
*************************************************************************/
static void testminnsunit_basictest1bc(ae_bool* errors, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    double v0;
    double v1;
    ae_vector x0;
    ae_vector x1;
    ae_vector bndl;
    ae_vector bndu;
    minnsstate s;
    minnsreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    _minnsstate_init(&s, _state);
    _minnsreport_init(&rep, _state);

    n = 2;
    ae_vector_set_length(&x0, n, _state);
    ae_vector_set_length(&bndl, n, _state);
    ae_vector_set_length(&bndu, n, _state);
    x0.ptr.p_double[0] = (double)(0);
    x0.ptr.p_double[1] = (double)(0);
    bndl.ptr.p_double[0] = _state->v_neginf;
    bndl.ptr.p_double[1] = _state->v_neginf;
    bndu.ptr.p_double[0] = 1/ae_sqrt((double)(2), _state);
    bndu.ptr.p_double[1] = (double)1/(double)2;
    minnscreate(n, &x0, &s, _state);
    minnssetbc(&s, &bndl, &bndu, _state);
    minnssetalgoags(&s, 0.1, 0.0, _state);
    while(minnsiteration(&s, _state))
    {
        if( s.needfij )
        {
            v0 = s.x.ptr.p_double[0];
            v1 = s.x.ptr.p_double[1];
            s.fi.ptr.p_double[0] = 10*ae_fabs(ae_sqr(v0, _state)-v1, _state)+ae_sqr(v0-1, _state);
            s.j.ptr.pp_double[0][0] = 10*ae_sign(ae_sqr(v0, _state)-v1, _state)*2*v0+2*(v0-1);
            s.j.ptr.pp_double[0][1] = (double)(10*ae_sign(ae_sqr(v0, _state)-v1, _state)*(-1));
            continue;
        }
        ae_assert(ae_false, "Assertion failed", _state);
    }
    minnsresults(&s, &x1, &rep, _state);
    seterrorflag(errors, rep.terminationtype<=0, _state);
    if( *errors )
    {
        ae_frame_leave(_state);
        return;
    }
    seterrorflag(errors, !ae_isfinite(x1.ptr.p_double[0], _state)||ae_fp_greater(ae_fabs(x1.ptr.p_double[0]-1/ae_sqrt((double)(2), _state), _state),0.001), _state);
    seterrorflag(errors, !ae_isfinite(x1.ptr.p_double[1], _state)||ae_fp_greater(ae_fabs(x1.ptr.p_double[1]-(double)1/(double)2, _state),0.001), _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Basic linearly constrained test
*************************************************************************/
static void testminnsunit_basictest0lc(ae_bool* errors, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_vector x0;
    ae_vector x1;
    ae_matrix c;
    ae_vector ct;
    double d;
    minnsstate s;
    minnsreport rep;
    double sumits;
    double sumnfev;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t nc;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    _minnsstate_init(&s, _state);
    _minnsreport_init(&rep, _state);

    d = -10.0;
    n = 5;
    passcount = 10;
    sumits = (double)(0);
    sumnfev = (double)(0);
    ae_vector_set_length(&x0, n, _state);
    ae_matrix_set_length(&c, 2*n, n+1, _state);
    ae_vector_set_length(&ct, 2*n, _state);
    for(pass=1; pass<=10; pass++)
    {
        nc = 0;
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            if( ae_fp_less(ae_randomreal(_state),0.5) )
            {
                for(j=0; j<=n; j++)
                {
                    c.ptr.pp_double[nc][j] = 0.0;
                }
                c.ptr.pp_double[nc][i] = 1.0+ae_randomreal(_state);
                ct.ptr.p_int[nc] = 0;
                inc(&nc, _state);
            }
            else
            {
                for(j=0; j<=n; j++)
                {
                    c.ptr.pp_double[nc+0][j] = 0.0;
                    c.ptr.pp_double[nc+1][j] = 0.0;
                }
                c.ptr.pp_double[nc+0][i] = 1.0+ae_randomreal(_state);
                c.ptr.pp_double[nc+1][i] = 1.0+ae_randomreal(_state);
                ct.ptr.p_int[nc+0] = 1;
                ct.ptr.p_int[nc+1] = -1;
                nc = nc+2;
            }
        }
        minnscreate(n, &x0, &s, _state);
        minnssetalgoags(&s, 0.1, 0.0, _state);
        minnssetlc(&s, &c, &ct, nc, _state);
        while(minnsiteration(&s, _state))
        {
            if( s.needfij )
            {
                s.fi.ptr.p_double[0] = 0.0;
                for(i=0; i<=n-1; i++)
                {
                    s.fi.ptr.p_double[0] = d*ae_sqr(s.x.ptr.p_double[i], _state);
                    s.j.ptr.pp_double[0][i] = d*2*s.x.ptr.p_double[i];
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnsresults(&s, &x1, &rep, _state);
        seterrorflag(errors, rep.terminationtype<=0, _state);
        if( *errors )
        {
            ae_frame_leave(_state);
            return;
        }
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(errors, !ae_isfinite(x1.ptr.p_double[i], _state)||ae_fp_greater(ae_fabs(x1.ptr.p_double[i], _state),0.001), _state);
        }
        sumits = sumits+(double)rep.iterationscount/(double)passcount;
        sumnfev = sumnfev+(double)rep.nfev/(double)passcount;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Basic constrained test: nonsmooth Rosenbrock posed as linearly constrained problem.

             [                         ]
    minimize [ 10*|x0^2-x1| + (1-x0)^2 ]
             [                         ]
             
    s.t. x0<=1/sqrt(2), x1<=0.5

It's exact solution is x0=1/sqrt(2), x1=1/2
*************************************************************************/
static void testminnsunit_basictest1lc(ae_bool* errors, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    double v0;
    double v1;
    ae_vector x0;
    ae_vector x1;
    ae_matrix c;
    ae_vector ct;
    minnsstate s;
    minnsreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    _minnsstate_init(&s, _state);
    _minnsreport_init(&rep, _state);

    n = 2;
    ae_vector_set_length(&x0, n, _state);
    ae_matrix_set_length(&c, 2, n+1, _state);
    ae_vector_set_length(&ct, 2, _state);
    x0.ptr.p_double[0] = (double)(0);
    x0.ptr.p_double[1] = (double)(0);
    c.ptr.pp_double[0][0] = 1.0;
    c.ptr.pp_double[0][1] = 0.0;
    c.ptr.pp_double[0][2] = 1/ae_sqrt((double)(2), _state);
    c.ptr.pp_double[1][0] = 0.0;
    c.ptr.pp_double[1][1] = 1.0;
    c.ptr.pp_double[1][2] = (double)1/(double)2;
    ct.ptr.p_int[0] = -1;
    ct.ptr.p_int[1] = -1;
    minnscreate(n, &x0, &s, _state);
    minnssetlc(&s, &c, &ct, 2, _state);
    minnssetalgoags(&s, 0.1, 0.0, _state);
    while(minnsiteration(&s, _state))
    {
        if( s.needfij )
        {
            v0 = s.x.ptr.p_double[0];
            v1 = s.x.ptr.p_double[1];
            s.fi.ptr.p_double[0] = 10*ae_fabs(ae_sqr(v0, _state)-v1, _state)+ae_sqr(v0-1, _state);
            s.j.ptr.pp_double[0][0] = 10*ae_sign(ae_sqr(v0, _state)-v1, _state)*2*v0+2*(v0-1);
            s.j.ptr.pp_double[0][1] = (double)(10*ae_sign(ae_sqr(v0, _state)-v1, _state)*(-1));
            continue;
        }
        ae_assert(ae_false, "Assertion failed", _state);
    }
    minnsresults(&s, &x1, &rep, _state);
    seterrorflag(errors, rep.terminationtype<=0, _state);
    if( *errors )
    {
        ae_frame_leave(_state);
        return;
    }
    seterrorflag(errors, !ae_isfinite(x1.ptr.p_double[0], _state)||ae_fp_greater(ae_fabs(x1.ptr.p_double[0]-1/ae_sqrt((double)(2), _state), _state),0.001), _state);
    seterrorflag(errors, !ae_isfinite(x1.ptr.p_double[1], _state)||ae_fp_greater(ae_fabs(x1.ptr.p_double[1]-(double)1/(double)2, _state),0.001), _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Basic nonlinearly constrained test
*************************************************************************/
static void testminnsunit_basictest0nlc(ae_bool* errors, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_vector x0;
    ae_vector x1;
    ae_matrix ec;
    ae_matrix ic;
    ae_int_t nec;
    ae_int_t nic;
    double d;
    minnsstate s;
    minnsreport rep;
    double sumits;
    double sumnfev;
    ae_int_t pass;
    ae_int_t passcount;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_matrix_init(&ec, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ic, 0, 0, DT_REAL, _state);
    _minnsstate_init(&s, _state);
    _minnsreport_init(&rep, _state);

    d = -10.0;
    n = 5;
    passcount = 10;
    sumits = (double)(0);
    sumnfev = (double)(0);
    ae_vector_set_length(&x0, n, _state);
    ae_matrix_set_length(&ec, 2*n, n+1, _state);
    ae_matrix_set_length(&ic, 2*n, n+1, _state);
    for(pass=1; pass<=10; pass++)
    {
        nec = 0;
        nic = 0;
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            if( ae_fp_less(ae_randomreal(_state),0.5) )
            {
                for(j=0; j<=n; j++)
                {
                    ec.ptr.pp_double[nec][j] = 0.0;
                }
                ec.ptr.pp_double[nec][i] = 1.0+ae_randomreal(_state);
                inc(&nec, _state);
            }
            else
            {
                for(j=0; j<=n; j++)
                {
                    ic.ptr.pp_double[nic+0][j] = 0.0;
                    ic.ptr.pp_double[nic+1][j] = 0.0;
                }
                ic.ptr.pp_double[nic+0][i] = 1.0+ae_randomreal(_state);
                ic.ptr.pp_double[nic+1][i] = -1.0-ae_randomreal(_state);
                nic = nic+2;
            }
        }
        minnscreate(n, &x0, &s, _state);
        minnssetalgoags(&s, 0.1, 100.0, _state);
        minnssetnlc(&s, nec, nic, _state);
        while(minnsiteration(&s, _state))
        {
            if( s.needfij )
            {
                s.fi.ptr.p_double[0] = 0.0;
                for(j=0; j<=n-1; j++)
                {
                    s.fi.ptr.p_double[0] = d*ae_sqr(s.x.ptr.p_double[j], _state);
                    s.j.ptr.pp_double[0][j] = d*2*s.x.ptr.p_double[j];
                }
                for(i=0; i<=nec-1; i++)
                {
                    s.fi.ptr.p_double[1+i] = -ec.ptr.pp_double[i][n];
                    for(j=0; j<=n-1; j++)
                    {
                        s.fi.ptr.p_double[1+i] = s.fi.ptr.p_double[1+i]+s.x.ptr.p_double[j]*ec.ptr.pp_double[i][j];
                        s.j.ptr.pp_double[1+i][j] = ec.ptr.pp_double[i][j];
                    }
                }
                for(i=0; i<=nic-1; i++)
                {
                    s.fi.ptr.p_double[1+nec+i] = -ic.ptr.pp_double[i][n];
                    for(j=0; j<=n-1; j++)
                    {
                        s.fi.ptr.p_double[1+nec+i] = s.fi.ptr.p_double[1+nec+i]+s.x.ptr.p_double[j]*ic.ptr.pp_double[i][j];
                        s.j.ptr.pp_double[1+nec+i][j] = ic.ptr.pp_double[i][j];
                    }
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnsresults(&s, &x1, &rep, _state);
        seterrorflag(errors, rep.terminationtype<=0, _state);
        if( *errors )
        {
            ae_frame_leave(_state);
            return;
        }
        for(i=0; i<=n-1; i++)
        {
            seterrorflag(errors, !ae_isfinite(x1.ptr.p_double[i], _state)||ae_fp_greater(ae_fabs(x1.ptr.p_double[i], _state),0.001), _state);
        }
        sumits = sumits+(double)rep.iterationscount/(double)passcount;
        sumnfev = sumnfev+(double)rep.nfev/(double)passcount;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Unconstrained test
*************************************************************************/
static void testminnsunit_testuc(ae_bool* primaryerrors,
     ae_bool* othererrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t i;
    ae_vector x0;
    ae_vector x0s;
    ae_vector x1;
    ae_vector x1s;
    ae_vector d;
    ae_vector xc;
    ae_vector s;
    ae_vector xrfirst;
    ae_vector xrlast;
    minnsstate state;
    minnsreport rep;
    double v;
    ae_int_t pass;
    ae_int_t passcount;
    ae_bool requirexrep;
    double epsrad;
    ae_bool werexreports;
    double repferr;
    double xtol;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x0s, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&x1s, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&xc, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&xrfirst, 0, DT_REAL, _state);
    ae_vector_init(&xrlast, 0, DT_REAL, _state);
    _minnsstate_init(&state, _state);
    _minnsreport_init(&rep, _state);

    passcount = 10;
    for(pass=1; pass<=10; pass++)
    {
        for(n=1; n<=5; n++)
        {
            
            /*
             * First test:
             * * test that problem is successfully solved
             * * test that X-reports are performed correctly - present
             *   when requested, return first and last points correctly,
             *   not present by default, function value is reported
             *   correctly.
             * * we use non-unit scale, randomly chosen one, which results
             *   in badly conditioned problems (to check robustness)
             */
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&xc, n, _state);
            ae_vector_set_length(&d, n, _state);
            ae_vector_set_length(&s, n, _state);
            ae_vector_set_length(&xrfirst, n, _state);
            ae_vector_set_length(&xrlast, n, _state);
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = 10*(2*ae_randomreal(_state)-1);
                xc.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                d.ptr.p_double[i] = ae_pow((double)(10), 2*(2*ae_randomreal(_state)-1), _state);
                s.ptr.p_double[i] = ae_pow((double)(10), 2*(2*ae_randomreal(_state)-1), _state);
            }
            requirexrep = ae_fp_greater(ae_randomreal(_state),0.5);
            epsrad = 0.01*ae_pow((double)(10), -2*ae_randomreal(_state), _state);
            xtol = 15.0*epsrad;
            minnscreate(n, &x0, &state, _state);
            minnssetalgoags(&state, 0.1, 0.0, _state);
            minnssetcond(&state, epsrad, 0, _state);
            minnssetscale(&state, &s, _state);
            if( requirexrep )
            {
                minnssetxrep(&state, ae_true, _state);
            }
            werexreports = ae_false;
            repferr = 0.0;
            while(minnsiteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                        state.j.ptr.pp_double[0][i] = d.ptr.p_double[i]*ae_sign(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                    }
                    continue;
                }
                if( state.xupdated )
                {
                    if( !werexreports )
                    {
                        ae_v_move(&xrfirst.ptr.p_double[0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    }
                    ae_v_move(&xrlast.ptr.p_double[0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    werexreports = ae_true;
                    v = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        v = v+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                    }
                    repferr = ae_maxreal(repferr, ae_fabs(v-state.f, _state), _state);
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnsresults(&state, &x1, &rep, _state);
            seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
            seterrorflag(othererrors, werexreports&&!requirexrep, _state);
            seterrorflag(othererrors, requirexrep&&!werexreports, _state);
            seterrorflag(othererrors, ae_fp_greater(repferr,10000*ae_machineepsilon), _state);
            if( *primaryerrors||(*othererrors) )
            {
                ae_frame_leave(_state);
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(primaryerrors, !ae_isfinite(x1.ptr.p_double[i], _state)||ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-xc.ptr.p_double[i], _state)/s.ptr.p_double[i],xtol), _state);
                if( requirexrep )
                {
                    seterrorflag(othererrors, !ae_isfinite(xrfirst.ptr.p_double[i], _state)||ae_fp_greater(ae_fabs(x0.ptr.p_double[i]-xrfirst.ptr.p_double[i], _state),100*ae_machineepsilon), _state);
                    seterrorflag(othererrors, !ae_isfinite(xrlast.ptr.p_double[i], _state)||ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-xrlast.ptr.p_double[i], _state),100*ae_machineepsilon), _state);
                }
            }
            
            /*
             * Test numerical differentiation:
             * * test that problem is successfully solved
             * * test that correct function value is reported
             */
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&xc, n, _state);
            ae_vector_set_length(&d, n, _state);
            ae_vector_set_length(&s, n, _state);
            ae_vector_set_length(&xrlast, n, _state);
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = 10*(2*ae_randomreal(_state)-1);
                xc.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                d.ptr.p_double[i] = ae_pow((double)(10), 2*(2*ae_randomreal(_state)-1), _state);
                s.ptr.p_double[i] = ae_pow((double)(10), 2*(2*ae_randomreal(_state)-1), _state);
            }
            epsrad = 0.01*ae_pow((double)(10), -2*ae_randomreal(_state), _state);
            xtol = 15.0*epsrad;
            minnscreatef(n, &x0, epsrad/100, &state, _state);
            minnssetalgoags(&state, 0.1, 0.0, _state);
            minnssetcond(&state, epsrad, 0, _state);
            minnssetscale(&state, &s, _state);
            minnssetxrep(&state, ae_true, _state);
            repferr = 0.0;
            while(minnsiteration(&state, _state))
            {
                if( state.needfi )
                {
                    state.fi.ptr.p_double[0] = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                    }
                    continue;
                }
                if( state.xupdated )
                {
                    v = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        v = v+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                    }
                    repferr = ae_maxreal(repferr, ae_fabs(v-state.f, _state), _state);
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnsresults(&state, &x1, &rep, _state);
            seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
            seterrorflag(othererrors, ae_fp_greater(repferr,10000*ae_machineepsilon), _state);
            if( *primaryerrors||(*othererrors) )
            {
                ae_frame_leave(_state);
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(primaryerrors, !ae_isfinite(x1.ptr.p_double[i], _state)||ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-xc.ptr.p_double[i], _state)/s.ptr.p_double[i],xtol), _state);
            }
            
            /*
             * Test scaling: we perform several steps on unit-scale problem,
             * then we perform same amount of steps on re-scaled problem,
             * starting from same point (but scaled according to chosen scale).
             *
             * Correctly written optimizer should perform essentially same steps
             * (up to scale) on both problems. At least, it holds within first
             * several steps, before rounding errors start to accumulate.
             *
             * NOTE: we also check that correctly scaled points are reported.
             *       And, as side effect, we check MinNSRestartFrom().
             *
             * NOTE: we use moderate scale and diagonal coefficients in order
             *       to have well-conditioned system. We test correctness of 
             *       formulae here, not robustness of algorithm.
             */
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&xc, n, _state);
            ae_vector_set_length(&x0s, n, _state);
            ae_vector_set_length(&d, n, _state);
            ae_vector_set_length(&s, n, _state);
            ae_vector_set_length(&xrlast, n, _state);
            for(i=0; i<=n-1; i++)
            {
                s.ptr.p_double[i] = ae_pow((double)(10), 2*ae_randomreal(_state)-1, _state);
                d.ptr.p_double[i] = ae_pow((double)(10), 2*ae_randomreal(_state)-1, _state);
                x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                xc.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                x0s.ptr.p_double[i] = x0.ptr.p_double[i]*s.ptr.p_double[i];
            }
            minnscreate(n, &x0, &state, _state);
            minnssetalgoags(&state, 0.1, 0.0, _state);
            minnssetcond(&state, 0.0, testminnsunit_scalingtestcnt, _state);
            minnssetxrep(&state, ae_false, _state);
            while(minnsiteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                        state.j.ptr.pp_double[0][i] = d.ptr.p_double[i]*ae_sign(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnsresults(&state, &x1, &rep, _state);
            seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
            if( *primaryerrors||(*othererrors) )
            {
                ae_frame_leave(_state);
                return;
            }
            minnssetscale(&state, &s, _state);
            minnssetxrep(&state, ae_true, _state);
            minnsrestartfrom(&state, &x0s, _state);
            werexreports = ae_false;
            while(minnsiteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]/s.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                        state.j.ptr.pp_double[0][i] = d.ptr.p_double[i]*ae_sign(state.x.ptr.p_double[i]/s.ptr.p_double[i]-xc.ptr.p_double[i], _state)/s.ptr.p_double[i];
                    }
                    continue;
                }
                if( state.xupdated )
                {
                    ae_v_move(&xrlast.ptr.p_double[0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    werexreports = ae_true;
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnsresults(&state, &x1s, &rep, _state);
            seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
            if( *primaryerrors||(*othererrors) )
            {
                ae_frame_leave(_state);
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(primaryerrors, (!ae_isfinite(x1.ptr.p_double[i], _state)||!ae_isfinite(x1s.ptr.p_double[i], _state))||ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-x1s.ptr.p_double[i]/s.ptr.p_double[i], _state),1.0E-4), _state);
                seterrorflag(othererrors, !ae_isfinite(xrlast.ptr.p_double[i], _state)||ae_fp_greater(ae_fabs(x1s.ptr.p_double[i]-xrlast.ptr.p_double[i], _state),testminnsunit_scalingtesttol), _state);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Box constrained test
*************************************************************************/
static void testminnsunit_testbc(ae_bool* primaryerrors,
     ae_bool* othererrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_vector x0;
    ae_vector x0s;
    ae_vector x1;
    ae_vector x1s;
    ae_vector b;
    ae_vector d;
    ae_vector xc;
    ae_vector s;
    ae_vector bndl;
    ae_vector bndu;
    ae_vector scaledbndl;
    ae_vector scaledbndu;
    ae_vector xrfirst;
    ae_vector xrlast;
    ae_matrix a;
    minnsstate state;
    minnsreport rep;
    double v;
    double v0;
    double v1;
    ae_int_t pass;
    ae_int_t passcount;
    ae_bool requirexrep;
    double epsrad;
    ae_bool werexreports;
    double repferr;
    double xtol;
    ae_int_t maxn;
    double conda;
    double gnorm;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x0s, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&x1s, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&xc, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    ae_vector_init(&scaledbndl, 0, DT_REAL, _state);
    ae_vector_init(&scaledbndu, 0, DT_REAL, _state);
    ae_vector_init(&xrfirst, 0, DT_REAL, _state);
    ae_vector_init(&xrlast, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    _minnsstate_init(&state, _state);
    _minnsreport_init(&rep, _state);

    passcount = 10;
    maxn = 5;
    
    /*
     * First test:
     * * sparse function
     * * test that problem is successfully solved
     * * non-unit scale is used, which results in badly conditioned problem
     * * check that all iterates are feasible (box-constrained)
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&xc, n, _state);
            ae_vector_set_length(&d, n, _state);
            ae_vector_set_length(&s, n, _state);
            ae_vector_set_length(&xrfirst, n, _state);
            ae_vector_set_length(&xrlast, n, _state);
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = 10*(2*ae_randomreal(_state)-1);
                xc.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                d.ptr.p_double[i] = ae_pow((double)(10), 2*(2*ae_randomreal(_state)-1), _state);
                s.ptr.p_double[i] = ae_pow((double)(10), 2*(2*ae_randomreal(_state)-1), _state);
                bndl.ptr.p_double[i] = _state->v_neginf;
                bndu.ptr.p_double[i] = _state->v_posinf;
                k = ae_randominteger(5, _state);
                if( k==1 )
                {
                    bndl.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                if( k==2 )
                {
                    bndu.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                if( k==3 )
                {
                    v0 = 2*ae_randomreal(_state)-1;
                    v1 = 2*ae_randomreal(_state)-1;
                    bndl.ptr.p_double[i] = ae_minreal(v0, v1, _state);
                    bndu.ptr.p_double[i] = ae_maxreal(v0, v1, _state);
                }
                if( k==4 )
                {
                    bndl.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    bndu.ptr.p_double[i] = bndl.ptr.p_double[i];
                }
            }
            requirexrep = ae_fp_greater(ae_randomreal(_state),0.5);
            epsrad = 0.01*ae_pow((double)(10), -2*ae_randomreal(_state), _state);
            xtol = 15.0*epsrad;
            minnscreate(n, &x0, &state, _state);
            minnssetalgoags(&state, 0.1, 0.0, _state);
            minnssetcond(&state, epsrad, 0, _state);
            minnssetbc(&state, &bndl, &bndu, _state);
            minnssetscale(&state, &s, _state);
            if( requirexrep )
            {
                minnssetxrep(&state, ae_true, _state);
            }
            werexreports = ae_false;
            repferr = 0.0;
            while(minnsiteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                        state.j.ptr.pp_double[0][i] = d.ptr.p_double[i]*ae_sign(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                    }
                    continue;
                }
                if( state.xupdated )
                {
                    if( !werexreports )
                    {
                        ae_v_move(&xrfirst.ptr.p_double[0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    }
                    ae_v_move(&xrlast.ptr.p_double[0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    werexreports = ae_true;
                    v = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        v = v+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                    }
                    repferr = ae_maxreal(repferr, ae_fabs(v-state.f, _state), _state);
                    for(i=0; i<=n-1; i++)
                    {
                        seterrorflag(primaryerrors, ae_fp_less(state.x.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
                        seterrorflag(primaryerrors, ae_fp_greater(state.x.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnsresults(&state, &x1, &rep, _state);
            seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
            seterrorflag(othererrors, werexreports&&!requirexrep, _state);
            seterrorflag(othererrors, requirexrep&&!werexreports, _state);
            seterrorflag(othererrors, ae_fp_greater(repferr,10000*ae_machineepsilon), _state);
            if( *primaryerrors||(*othererrors) )
            {
                ae_frame_leave(_state);
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(primaryerrors, !ae_isfinite(x1.ptr.p_double[i], _state)||ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-boundval(xc.ptr.p_double[i], bndl.ptr.p_double[i], bndu.ptr.p_double[i], _state), _state)/s.ptr.p_double[i],xtol), _state);
                seterrorflag(primaryerrors, ae_fp_less(x1.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
                seterrorflag(primaryerrors, ae_fp_greater(x1.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
                if( requirexrep )
                {
                    seterrorflag(othererrors, !ae_isfinite(xrfirst.ptr.p_double[i], _state)||ae_fp_greater(ae_fabs(boundval(x0.ptr.p_double[i], bndl.ptr.p_double[i], bndu.ptr.p_double[i], _state)-xrfirst.ptr.p_double[i], _state),100*ae_machineepsilon), _state);
                    seterrorflag(othererrors, !ae_isfinite(xrlast.ptr.p_double[i], _state)||ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-xrlast.ptr.p_double[i], _state),100*ae_machineepsilon), _state);
                }
            }
        }
    }
    
    /*
     * A bit harder test:
     * * dense quadratic function (smooth), may be prone to different
     *   rounding-related issues
     * * non-negativity box constraints
     * * unit scale is used
     * * extreme stopping criteria (EpsX=1.0E-12)
     * * single pass for each problem size
     * * check that constrained gradient at solution is small
     */
    conda = 1.0E3;
    epsrad = 1.0E-12;
    for(n=1; n<=10; n++)
    {
        ae_vector_set_length(&x0, n, _state);
        ae_vector_set_length(&bndl, n, _state);
        ae_vector_set_length(&bndu, n, _state);
        ae_vector_set_length(&b, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = 1.0;
            b.ptr.p_double[i] = ae_randomreal(_state)-0.5;
            bndl.ptr.p_double[i] = 0.0;
            bndu.ptr.p_double[i] = _state->v_posinf;
        }
        spdmatrixrndcond(n, conda, &a, _state);
        minnscreate(n, &x0, &state, _state);
        minnssetalgoags(&state, 0.1, 0.0, _state);
        minnssetcond(&state, epsrad, 0, _state);
        minnssetbc(&state, &bndl, &bndu, _state);
        while(minnsiteration(&state, _state))
        {
            if( state.needfij )
            {
                state.fi.ptr.p_double[0] = 0.0;
                for(i=0; i<=n-1; i++)
                {
                    state.j.ptr.pp_double[0][i] = 0.0;
                }
                for(i=0; i<=n-1; i++)
                {
                    state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+b.ptr.p_double[i]*state.x.ptr.p_double[i];
                    for(j=0; j<=n-1; j++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+0.5*state.x.ptr.p_double[i]*a.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                    }
                }
                for(i=0; i<=n-1; i++)
                {
                    state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+b.ptr.p_double[i];
                    for(j=0; j<=n-1; j++)
                    {
                        state.j.ptr.pp_double[0][i] = state.j.ptr.pp_double[0][i]+a.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                    }
                }
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnsresults(&state, &x1, &rep, _state);
        seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
        if( *primaryerrors||(*othererrors) )
        {
            ae_frame_leave(_state);
            return;
        }
        gnorm = 0.0;
        for(i=0; i<=n-1; i++)
        {
            v = b.ptr.p_double[i];
            for(j=0; j<=n-1; j++)
            {
                v = v+a.ptr.pp_double[i][j]*x1.ptr.p_double[j];
            }
            if( ae_fp_eq(x1.ptr.p_double[i],bndl.ptr.p_double[i])&&ae_fp_greater(v,(double)(0)) )
            {
                v = (double)(0);
            }
            if( ae_fp_eq(x1.ptr.p_double[i],bndu.ptr.p_double[i])&&ae_fp_less(v,(double)(0)) )
            {
                v = (double)(0);
            }
            gnorm = gnorm+ae_sqr(v, _state);
            seterrorflag(primaryerrors, ae_fp_less(x1.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
            seterrorflag(primaryerrors, ae_fp_greater(x1.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
        }
        gnorm = ae_sqrt(gnorm, _state);
        seterrorflag(primaryerrors, ae_fp_greater(gnorm,1.0E-5), _state);
    }
    
    /*
     * Test on HIGHLY nonconvex bound constrained problem.
     * Algorithm should be able to stop.
     *
     * NOTE: because algorithm can be attracted to saddle points,
     *       x[i] may be -1, +1 or approximately zero.
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = ae_randomreal(_state)-0.5;
                bndl.ptr.p_double[i] = -1.0;
                bndu.ptr.p_double[i] = 1.0;
            }
            epsrad = 0.0001;
            xtol = 15.0*epsrad;
            minnscreate(n, &x0, &state, _state);
            minnssetalgoags(&state, 0.1, 0.0, _state);
            minnssetcond(&state, epsrad, 0, _state);
            minnssetbc(&state, &bndl, &bndu, _state);
            v = -1000.0;
            while(minnsiteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        v0 = ae_fabs(state.x.ptr.p_double[i], _state);
                        v1 = (double)(ae_sign(state.x.ptr.p_double[i], _state));
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+v*(v0+v0*v0);
                        state.j.ptr.pp_double[0][i] = v*(v1+2*v0*v1);
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnsresults(&state, &x1, &rep, _state);
            seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
            for(i=0; i<=n-1; i++)
            {
                v = ae_fabs(x1.ptr.p_double[i], _state);
                seterrorflag(primaryerrors, !ae_isfinite(x1.ptr.p_double[i], _state), _state);
                seterrorflag(primaryerrors, ae_fp_neq(v,1.0)&&ae_fp_greater(v,xtol), _state);
            }
        }
    }
    
    /*
     * Test numerical differentiation:
     * * test that problem is successfully solved
     * * test that correct function value is reported
     * * test that all iterates are within bound-constrained area
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&xc, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&d, n, _state);
            ae_vector_set_length(&s, n, _state);
            ae_vector_set_length(&xrlast, n, _state);
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = 10*(2*ae_randomreal(_state)-1);
                xc.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                d.ptr.p_double[i] = ae_pow((double)(10), 2*(2*ae_randomreal(_state)-1), _state);
                s.ptr.p_double[i] = ae_pow((double)(10), 2*(2*ae_randomreal(_state)-1), _state);
                bndl.ptr.p_double[i] = _state->v_neginf;
                bndu.ptr.p_double[i] = _state->v_posinf;
                k = ae_randominteger(5, _state);
                if( k==1 )
                {
                    bndl.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                if( k==2 )
                {
                    bndu.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                if( k==3 )
                {
                    v0 = 2*ae_randomreal(_state)-1;
                    v1 = 2*ae_randomreal(_state)-1;
                    bndl.ptr.p_double[i] = ae_minreal(v0, v1, _state);
                    bndu.ptr.p_double[i] = ae_maxreal(v0, v1, _state);
                }
                if( k==4 )
                {
                    bndl.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    bndu.ptr.p_double[i] = bndl.ptr.p_double[i];
                }
            }
            epsrad = 0.01*ae_pow((double)(10), -2*ae_randomreal(_state), _state);
            xtol = 15.0*epsrad;
            minnscreatef(n, &x0, epsrad/100, &state, _state);
            minnssetalgoags(&state, 0.1, 0.0, _state);
            minnssetcond(&state, epsrad, 0, _state);
            minnssetscale(&state, &s, _state);
            minnssetbc(&state, &bndl, &bndu, _state);
            minnssetxrep(&state, ae_true, _state);
            repferr = 0.0;
            while(minnsiteration(&state, _state))
            {
                if( state.needfi )
                {
                    state.fi.ptr.p_double[0] = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                        seterrorflag(primaryerrors, ae_fp_less(state.x.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
                        seterrorflag(primaryerrors, ae_fp_greater(state.x.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
                    }
                    continue;
                }
                if( state.xupdated )
                {
                    v = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        v = v+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                        seterrorflag(primaryerrors, ae_fp_less(state.x.ptr.p_double[i],bndl.ptr.p_double[i]), _state);
                        seterrorflag(primaryerrors, ae_fp_greater(state.x.ptr.p_double[i],bndu.ptr.p_double[i]), _state);
                    }
                    repferr = ae_maxreal(repferr, ae_fabs(v-state.f, _state), _state);
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnsresults(&state, &x1, &rep, _state);
            seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
            seterrorflag(othererrors, ae_fp_greater(repferr,10000*ae_machineepsilon), _state);
            if( *primaryerrors||(*othererrors) )
            {
                ae_frame_leave(_state);
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(primaryerrors, !ae_isfinite(x1.ptr.p_double[i], _state)||ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-boundval(xc.ptr.p_double[i], bndl.ptr.p_double[i], bndu.ptr.p_double[i], _state), _state)/s.ptr.p_double[i],xtol), _state);
            }
        }
    }
    
    /*
     * Test scaling: we perform several steps on unit-scale problem,
     * then we perform same amount of steps on re-scaled problem,
     * starting from same point (but scaled according to chosen scale).
     *
     * Correctly written optimizer should perform essentially same steps
     * (up to scale) on both problems. At least, it holds within first
     * several steps, before rounding errors start to accumulate.
     *
     * NOTE: we also check that correctly scaled points are reported.
     *       And, as side effect, we check MinNSRestartFrom().
     *
     * NOTE: we use very low scale and diagonal coefficients in order
     *       to have well-conditioned system. We test correctness of 
     *       formulae here, not robustness of algorithm.
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&xc, n, _state);
            ae_vector_set_length(&x0s, n, _state);
            ae_vector_set_length(&d, n, _state);
            ae_vector_set_length(&s, n, _state);
            ae_vector_set_length(&xrlast, n, _state);
            ae_vector_set_length(&bndl, n, _state);
            ae_vector_set_length(&bndu, n, _state);
            ae_vector_set_length(&scaledbndl, n, _state);
            ae_vector_set_length(&scaledbndu, n, _state);
            for(i=0; i<=n-1; i++)
            {
                s.ptr.p_double[i] = ae_pow((double)(10), ae_randomreal(_state)-0.5, _state);
                d.ptr.p_double[i] = ae_pow((double)(10), ae_randomreal(_state)-0.5, _state);
                x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                xc.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                x0s.ptr.p_double[i] = x0.ptr.p_double[i]*s.ptr.p_double[i];
                bndl.ptr.p_double[i] = _state->v_neginf;
                bndu.ptr.p_double[i] = _state->v_posinf;
                k = ae_randominteger(5, _state);
                if( k==1 )
                {
                    bndl.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                if( k==2 )
                {
                    bndu.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
                if( k==3 )
                {
                    v0 = 2*ae_randomreal(_state)-1;
                    v1 = 2*ae_randomreal(_state)-1;
                    bndl.ptr.p_double[i] = ae_minreal(v0, v1, _state);
                    bndu.ptr.p_double[i] = ae_maxreal(v0, v1, _state);
                }
                if( k==4 )
                {
                    bndl.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    bndu.ptr.p_double[i] = bndl.ptr.p_double[i];
                }
                scaledbndl.ptr.p_double[i] = bndl.ptr.p_double[i]*s.ptr.p_double[i];
                scaledbndu.ptr.p_double[i] = bndu.ptr.p_double[i]*s.ptr.p_double[i];
            }
            minnscreate(n, &x0, &state, _state);
            minnssetalgoags(&state, 0.01, 0.0, _state);
            minnssetcond(&state, 0.0, testminnsunit_scalingtestcnt, _state);
            minnssetbc(&state, &bndl, &bndu, _state);
            minnssetxrep(&state, ae_false, _state);
            while(minnsiteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                        state.j.ptr.pp_double[0][i] = d.ptr.p_double[i]*ae_sign(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnsresults(&state, &x1, &rep, _state);
            seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
            if( *primaryerrors||(*othererrors) )
            {
                ae_frame_leave(_state);
                return;
            }
            minnssetscale(&state, &s, _state);
            minnssetbc(&state, &scaledbndl, &scaledbndu, _state);
            minnsrestartfrom(&state, &x0s, _state);
            while(minnsiteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]/s.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                        state.j.ptr.pp_double[0][i] = d.ptr.p_double[i]*ae_sign(state.x.ptr.p_double[i]/s.ptr.p_double[i]-xc.ptr.p_double[i], _state)/s.ptr.p_double[i];
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnsresults(&state, &x1s, &rep, _state);
            seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
            if( *primaryerrors||(*othererrors) )
            {
                ae_frame_leave(_state);
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(primaryerrors, (!ae_isfinite(x1.ptr.p_double[i], _state)||!ae_isfinite(x1s.ptr.p_double[i], _state))||ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-x1s.ptr.p_double[i]/s.ptr.p_double[i], _state),testminnsunit_scalingtesttol), _state);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Linearly constrained test
*************************************************************************/
static void testminnsunit_testlc(ae_bool* primaryerrors,
     ae_bool* othererrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t nc;
    ae_vector x0;
    ae_vector x0s;
    ae_vector x1;
    ae_vector x2;
    ae_vector x1s;
    ae_vector d;
    ae_vector xc;
    ae_vector s;
    ae_vector bndl;
    ae_vector bndu;
    ae_matrix c;
    ae_matrix scaledc;
    ae_vector ct;
    ae_vector scaledbndl;
    ae_vector scaledbndu;
    ae_vector xrfirst;
    ae_vector xrlast;
    minnsstate state;
    minnsreport rep;
    double v;
    double v0;
    double v1;
    double vv;
    double flast0;
    double flast1;
    ae_int_t pass;
    ae_int_t passcount;
    double epsrad;
    double repferr;
    double xtol;
    double ftol;
    double rho;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x0s, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);
    ae_vector_init(&x1s, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&xc, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_matrix_init(&scaledc, 0, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    ae_vector_init(&scaledbndl, 0, DT_REAL, _state);
    ae_vector_init(&scaledbndu, 0, DT_REAL, _state);
    ae_vector_init(&xrfirst, 0, DT_REAL, _state);
    ae_vector_init(&xrlast, 0, DT_REAL, _state);
    _minnsstate_init(&state, _state);
    _minnsreport_init(&rep, _state);

    passcount = 10;
    for(pass=1; pass<=10; pass++)
    {
        for(n=1; n<=5; n++)
        {
            
            /*
             * First test:
             * * smooth problem
             * * subject to random linear constraints
             * * with non-unit scale
             *
             * We:
             * * compare function value at constrained solution with function
             *   value for penalized unconstrained problem. We do not compare
             *   actual X-values returned, because they are highly unstable -
             *   function values at minimum show better stability.
             * * check that correct function values are reported
             */
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&xc, n, _state);
            ae_vector_set_length(&d, n, _state);
            ae_vector_set_length(&s, n, _state);
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = 10*(2*ae_randomreal(_state)-1);
                xc.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                d.ptr.p_double[i] = 1+ae_randomreal(_state);
                s.ptr.p_double[i] = 1+ae_randomreal(_state);
            }
            nc = ae_randominteger((n+1)/2, _state);
            if( nc>0 )
            {
                ae_matrix_set_length(&c, nc, n+1, _state);
                ae_vector_set_length(&ct, nc, _state);
                for(i=0; i<=nc-1; i++)
                {
                    ct.ptr.p_int[i] = ae_randominteger(3, _state)-1;
                    for(j=0; j<=n; j++)
                    {
                        c.ptr.pp_double[i][j] = ae_randomreal(_state)-0.5;
                    }
                }
            }
            epsrad = 0.00001;
            ftol = 0.01;
            minnscreate(n, &x0, &state, _state);
            minnssetalgoags(&state, 0.1, 0.0, _state);
            minnssetcond(&state, epsrad, 0, _state);
            minnssetscale(&state, &s, _state);
            minnssetxrep(&state, ae_true, _state);
            minnssetlc(&state, &c, &ct, nc, _state);
            repferr = 0.0;
            flast0 = _state->v_nan;
            while(minnsiteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_sqr(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                        state.j.ptr.pp_double[0][i] = d.ptr.p_double[i]*(2*(state.x.ptr.p_double[i]-xc.ptr.p_double[i]));
                    }
                    continue;
                }
                if( state.xupdated )
                {
                    flast0 = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        flast0 = flast0+d.ptr.p_double[i]*ae_sqr(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                    }
                    repferr = ae_maxreal(repferr, ae_fabs(flast0-state.f, _state), _state);
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnsresults(&state, &x1, &rep, _state);
            seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
            seterrorflag(primaryerrors, !ae_isfinite(flast0, _state), _state);
            seterrorflag(othererrors, ae_fp_greater(repferr,10000*ae_machineepsilon), _state);
            if( *primaryerrors||(*othererrors) )
            {
                ae_frame_leave(_state);
                return;
            }
            minnssetlc(&state, &c, &ct, 0, _state);
            minnsrestartfrom(&state, &x0, _state);
            rho = 1000.0;
            repferr = 0.0;
            flast1 = _state->v_nan;
            while(minnsiteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_sqr(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                        state.j.ptr.pp_double[0][i] = d.ptr.p_double[i]*(2*(state.x.ptr.p_double[i]-xc.ptr.p_double[i]));
                    }
                    for(i=0; i<=nc-1; i++)
                    {
                        v = ae_v_dotproduct(&state.x.ptr.p_double[0], 1, &c.ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
                        v = v-c.ptr.pp_double[i][n];
                        vv = 0.0;
                        if( ct.ptr.p_int[i]<0 )
                        {
                            vv = (double)(ae_sign(ae_maxreal(v, 0.0, _state), _state));
                            v = ae_maxreal(v, 0.0, _state);
                        }
                        if( ct.ptr.p_int[i]==0 )
                        {
                            vv = (double)(ae_sign(v, _state));
                            v = ae_fabs(v, _state);
                        }
                        if( ct.ptr.p_int[i]>0 )
                        {
                            vv = (double)(-ae_sign(ae_maxreal(-v, 0.0, _state), _state));
                            v = ae_maxreal(-v, 0.0, _state);
                        }
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+rho*v;
                        for(j=0; j<=n-1; j++)
                        {
                            state.j.ptr.pp_double[0][j] = state.j.ptr.pp_double[0][j]+rho*vv*c.ptr.pp_double[i][j];
                        }
                    }
                    continue;
                }
                if( state.xupdated )
                {
                    flast1 = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        flast1 = flast1+d.ptr.p_double[i]*ae_sqr(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnsresults(&state, &x2, &rep, _state);
            seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
            seterrorflag(primaryerrors, !ae_isfinite(flast1, _state), _state);
            if( *primaryerrors||(*othererrors) )
            {
                ae_frame_leave(_state);
                return;
            }
            seterrorflag(primaryerrors, ae_fp_greater(ae_fabs(flast0-flast1, _state),ftol), _state);
            
            /*
             * Test on HIGHLY nonconvex linearly constrained problem.
             * Algorithm should be able to stop at the bounds.
             */
            ae_vector_set_length(&x0, n, _state);
            ae_matrix_set_length(&c, 2*n, n+1, _state);
            ae_vector_set_length(&ct, 2*n, _state);
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = ae_randomreal(_state)-0.5;
                for(j=0; j<=n-1; j++)
                {
                    c.ptr.pp_double[2*i+0][j] = 0.0;
                    c.ptr.pp_double[2*i+1][j] = 0.0;
                }
                c.ptr.pp_double[2*i+0][i] = 1.0;
                c.ptr.pp_double[2*i+0][n] = -1.0;
                ct.ptr.p_int[2*i+0] = 1;
                c.ptr.pp_double[2*i+1][i] = 1.0;
                c.ptr.pp_double[2*i+1][n] = 1.0;
                ct.ptr.p_int[2*i+1] = -1;
            }
            epsrad = 0.0001;
            xtol = 15.0*epsrad;
            minnscreate(n, &x0, &state, _state);
            minnssetalgoags(&state, 0.1, 0.0, _state);
            minnssetcond(&state, epsrad, 0, _state);
            minnssetlc(&state, &c, &ct, 2*n, _state);
            v = -1000.0;
            while(minnsiteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        v0 = ae_fabs(state.x.ptr.p_double[i], _state);
                        v1 = (double)(ae_sign(state.x.ptr.p_double[i], _state));
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+v*(v0+v0*v0);
                        state.j.ptr.pp_double[0][i] = v*(v1+2*v0*v1);
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnsresults(&state, &x1, &rep, _state);
            seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(primaryerrors, !ae_isfinite(x1.ptr.p_double[i], _state), _state);
                seterrorflag(primaryerrors, (ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-1, _state),xtol)&&ae_fp_greater(ae_fabs(x1.ptr.p_double[i], _state),xtol))&&ae_fp_greater(ae_fabs(x1.ptr.p_double[i]+1, _state),xtol), _state);
            }
            
            /*
             * Test scaling: we perform several steps on unit-scale problem,
             * then we perform same amount of steps on re-scaled problem,
             * starting from same point (but scaled according to chosen scale).
             *
             * Correctly written optimizer should perform essentially same steps
             * (up to scale) on both problems. At least, it holds within first
             * several steps, before rounding errors start to accumulate.
             *
             * NOTE: we also check that correctly scaled points are reported.
             *       And, as side effect, we check MinNSRestartFrom().
             *
             * NOTE: we use moderate scale and diagonal coefficients in order
             *       to have well-conditioned system. We test correctness of 
             *       formulae here, not robustness of algorithm.
             */
            ae_vector_set_length(&x0, n, _state);
            ae_vector_set_length(&xc, n, _state);
            ae_vector_set_length(&x0s, n, _state);
            ae_vector_set_length(&d, n, _state);
            ae_vector_set_length(&s, n, _state);
            ae_vector_set_length(&xrlast, n, _state);
            ae_matrix_set_length(&c, 2*n, n+1, _state);
            ae_matrix_set_length(&scaledc, 2*n, n+1, _state);
            ae_vector_set_length(&ct, 2*n, _state);
            for(i=0; i<=2*n-1; i++)
            {
                ct.ptr.p_int[i] = 0;
                for(j=0; j<=n; j++)
                {
                    c.ptr.pp_double[i][j] = (double)(0);
                }
            }
            for(i=0; i<=n-1; i++)
            {
                s.ptr.p_double[i] = ae_pow((double)(10), 2*ae_randomreal(_state)-1, _state);
                d.ptr.p_double[i] = ae_pow((double)(10), 2*ae_randomreal(_state)-1, _state);
                x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                xc.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                x0s.ptr.p_double[i] = x0.ptr.p_double[i]*s.ptr.p_double[i];
                k = ae_randominteger(5, _state);
                if( k==1 )
                {
                    c.ptr.pp_double[2*i+0][i] = 1.0;
                    c.ptr.pp_double[2*i+0][n] = 2*ae_randomreal(_state)-1;
                    ct.ptr.p_int[2*i+0] = 1;
                }
                if( k==2 )
                {
                    c.ptr.pp_double[2*i+0][i] = 1.0;
                    c.ptr.pp_double[2*i+0][n] = 2*ae_randomreal(_state)-1;
                    ct.ptr.p_int[2*i+0] = -1;
                }
                if( k==3 )
                {
                    v0 = 2*ae_randomreal(_state)-1;
                    v1 = 2*ae_randomreal(_state)-1;
                    c.ptr.pp_double[2*i+0][i] = 1.0;
                    c.ptr.pp_double[2*i+0][n] = ae_minreal(v0, v1, _state);
                    c.ptr.pp_double[2*i+1][i] = 1.0;
                    c.ptr.pp_double[2*i+1][n] = ae_maxreal(v0, v1, _state);
                    ct.ptr.p_int[2*i+0] = 1;
                    ct.ptr.p_int[2*i+1] = -1;
                }
                if( k==4 )
                {
                    c.ptr.pp_double[2*i+0][i] = 1.0;
                    c.ptr.pp_double[2*i+0][n] = 2*ae_randomreal(_state)-1;
                    ct.ptr.p_int[2*i+0] = 0;
                }
            }
            for(i=0; i<=2*n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    scaledc.ptr.pp_double[i][j] = c.ptr.pp_double[i][j]/s.ptr.p_double[j];
                }
                scaledc.ptr.pp_double[i][n] = c.ptr.pp_double[i][n];
            }
            minnscreate(n, &x0, &state, _state);
            minnssetalgoags(&state, 0.1, 0.0, _state);
            minnssetcond(&state, 0.0, testminnsunit_scalingtestcnt, _state);
            minnssetlc(&state, &c, &ct, 2*n, _state);
            minnssetxrep(&state, ae_false, _state);
            while(minnsiteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                        state.j.ptr.pp_double[0][i] = d.ptr.p_double[i]*ae_sign(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnsresults(&state, &x1, &rep, _state);
            seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
            if( *primaryerrors||(*othererrors) )
            {
                ae_frame_leave(_state);
                return;
            }
            minnssetscale(&state, &s, _state);
            minnssetlc(&state, &scaledc, &ct, 2*n, _state);
            minnsrestartfrom(&state, &x0s, _state);
            while(minnsiteration(&state, _state))
            {
                if( state.needfij )
                {
                    state.fi.ptr.p_double[0] = 0.0;
                    for(i=0; i<=n-1; i++)
                    {
                        state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]/s.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                        state.j.ptr.pp_double[0][i] = d.ptr.p_double[i]*ae_sign(state.x.ptr.p_double[i]/s.ptr.p_double[i]-xc.ptr.p_double[i], _state)/s.ptr.p_double[i];
                    }
                    continue;
                }
                ae_assert(ae_false, "Assertion failed", _state);
            }
            minnsresults(&state, &x1s, &rep, _state);
            seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
            if( *primaryerrors||(*othererrors) )
            {
                ae_frame_leave(_state);
                return;
            }
            for(i=0; i<=n-1; i++)
            {
                seterrorflag(primaryerrors, (!ae_isfinite(x1.ptr.p_double[i], _state)||!ae_isfinite(x1s.ptr.p_double[i], _state))||ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-x1s.ptr.p_double[i]/s.ptr.p_double[i], _state),testminnsunit_scalingtesttol), _state);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Nonlinearly constrained test
*************************************************************************/
static void testminnsunit_testnlc(ae_bool* primaryerrors,
     ae_bool* othererrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_int_t nc;
    ae_int_t nec;
    ae_vector x0;
    ae_vector x0s;
    ae_vector x1;
    ae_vector x2;
    ae_vector x1s;
    ae_vector d;
    ae_vector xc;
    ae_vector s;
    ae_vector bndl;
    ae_vector bndu;
    ae_vector b;
    ae_vector r;
    ae_matrix c;
    ae_matrix scaledc;
    ae_vector ct;
    ae_vector scaledbndl;
    ae_vector scaledbndu;
    ae_vector xrfirst;
    ae_vector xrlast;
    minnsstate state;
    minnsreport rep;
    double v;
    ae_int_t pass;
    ae_int_t passcount;
    double epsrad;
    double xtol;
    double rho;
    ae_int_t maxn;
    double diffstep;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x0s, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);
    ae_vector_init(&x1s, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&xc, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&r, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_matrix_init(&scaledc, 0, 0, DT_REAL, _state);
    ae_vector_init(&ct, 0, DT_INT, _state);
    ae_vector_init(&scaledbndl, 0, DT_REAL, _state);
    ae_vector_init(&scaledbndu, 0, DT_REAL, _state);
    ae_vector_init(&xrfirst, 0, DT_REAL, _state);
    ae_vector_init(&xrlast, 0, DT_REAL, _state);
    _minnsstate_init(&state, _state);
    _minnsreport_init(&rep, _state);

    passcount = 10;
    maxn = 5;
    rho = 100.0;
    
    /*
     * First test:
     * * simple problem
     * * subject to random nonlinear constraints of form r[i]*x[i] OPERATION 0.0,
     *   where OPERATION is <= or =
     * * with non-unit scale
     *
     * We:
     * * compare numerical solution with analytic one, which can be
     *   easily calculated
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            for(nc=1; nc<=n; nc++)
            {
                for(nec=0; nec<=nc; nec++)
                {
                    ae_vector_set_length(&x0, n, _state);
                    ae_vector_set_length(&xc, n, _state);
                    ae_vector_set_length(&d, n, _state);
                    ae_vector_set_length(&r, n, _state);
                    ae_vector_set_length(&s, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                        xc.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                        d.ptr.p_double[i] = ae_pow((double)(10), ae_randomreal(_state)-0.5, _state);
                        s.ptr.p_double[i] = ae_pow((double)(10), ae_randomreal(_state)-0.5, _state);
                        r.ptr.p_double[i] = (2*ae_randominteger(2, _state)-1)*(0.1+ae_randomreal(_state));
                    }
                    epsrad = 0.001;
                    xtol = 0.01;
                    minnscreate(n, &x0, &state, _state);
                    minnssetalgoags(&state, 0.1, rho, _state);
                    minnssetcond(&state, epsrad, 0, _state);
                    minnssetscale(&state, &s, _state);
                    minnssetnlc(&state, nec, nc-nec, _state);
                    while(minnsiteration(&state, _state))
                    {
                        if( state.needfij )
                        {
                            state.fi.ptr.p_double[0] = 0.0;
                            for(i=0; i<=n-1; i++)
                            {
                                state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                                state.j.ptr.pp_double[0][i] = d.ptr.p_double[i]*ae_sign(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                            }
                            for(i=1; i<=nc; i++)
                            {
                                state.fi.ptr.p_double[i] = state.x.ptr.p_double[i-1]*r.ptr.p_double[i-1];
                                for(j=0; j<=n-1; j++)
                                {
                                    state.j.ptr.pp_double[i][j] = 0.0;
                                }
                                state.j.ptr.pp_double[i][i-1] = r.ptr.p_double[i-1];
                            }
                            continue;
                        }
                        ae_assert(ae_false, "Assertion failed", _state);
                    }
                    minnsresults(&state, &x1, &rep, _state);
                    seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
                    if( *primaryerrors||(*othererrors) )
                    {
                        ae_frame_leave(_state);
                        return;
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        v = xc.ptr.p_double[i];
                        if( i<nec )
                        {
                            v = 0.0;
                        }
                        if( i>=nec&&i<nc )
                        {
                            if( ae_fp_greater(r.ptr.p_double[i],0.0) )
                            {
                                v = ae_minreal(v, 0.0, _state);
                            }
                            if( ae_fp_less(r.ptr.p_double[i],0.0) )
                            {
                                v = ae_maxreal(v, 0.0, _state);
                            }
                        }
                        seterrorflag(primaryerrors, !ae_isfinite(x1.ptr.p_double[i], _state), _state);
                        seterrorflag(primaryerrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-v, _state),xtol*s.ptr.p_double[i]), _state);
                    }
                }
            }
        }
    }
    
    /*
     * Numerical differentiation test.
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            for(nc=1; nc<=n; nc++)
            {
                for(nec=0; nec<=nc; nec++)
                {
                    ae_vector_set_length(&x0, n, _state);
                    ae_vector_set_length(&xc, n, _state);
                    ae_vector_set_length(&d, n, _state);
                    ae_vector_set_length(&r, n, _state);
                    ae_vector_set_length(&s, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                        xc.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                        d.ptr.p_double[i] = ae_pow((double)(10), ae_randomreal(_state)-0.5, _state);
                        s.ptr.p_double[i] = ae_pow((double)(10), ae_randomreal(_state)-0.5, _state);
                        r.ptr.p_double[i] = (2*ae_randominteger(2, _state)-1)*(0.1+ae_randomreal(_state));
                    }
                    epsrad = 0.001;
                    xtol = 0.01;
                    diffstep = 0.001;
                    minnscreatef(n, &x0, diffstep, &state, _state);
                    minnssetalgoags(&state, 0.1, rho, _state);
                    minnssetcond(&state, epsrad, 0, _state);
                    minnssetscale(&state, &s, _state);
                    minnssetnlc(&state, nec, nc-nec, _state);
                    while(minnsiteration(&state, _state))
                    {
                        if( state.needfi )
                        {
                            state.fi.ptr.p_double[0] = 0.0;
                            for(i=0; i<=n-1; i++)
                            {
                                state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                            }
                            for(i=1; i<=nc; i++)
                            {
                                state.fi.ptr.p_double[i] = state.x.ptr.p_double[i-1]*r.ptr.p_double[i-1];
                            }
                            continue;
                        }
                        ae_assert(ae_false, "Assertion failed", _state);
                    }
                    minnsresults(&state, &x1, &rep, _state);
                    seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
                    if( *primaryerrors||(*othererrors) )
                    {
                        ae_frame_leave(_state);
                        return;
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        v = xc.ptr.p_double[i];
                        if( i<nec )
                        {
                            v = 0.0;
                        }
                        if( i>=nec&&i<nc )
                        {
                            if( ae_fp_greater(r.ptr.p_double[i],0.0) )
                            {
                                v = ae_minreal(v, 0.0, _state);
                            }
                            if( ae_fp_less(r.ptr.p_double[i],0.0) )
                            {
                                v = ae_maxreal(v, 0.0, _state);
                            }
                        }
                        seterrorflag(primaryerrors, !ae_isfinite(x1.ptr.p_double[i], _state), _state);
                        seterrorflag(primaryerrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-v, _state),xtol*s.ptr.p_double[i]), _state);
                    }
                }
            }
        }
    }
    
    /*
     * Test scaling: we perform several steps on unit-scale problem,
     * then we perform same amount of steps on re-scaled problem,
     * starting from same point (but scaled according to chosen scale).
     *
     * Correctly written optimizer should perform essentially same steps
     * (up to scale) on both problems. At least, it holds within first
     * several steps, before rounding errors start to accumulate.
     *
     * NOTE: we use moderate scale and diagonal coefficients in order
     *       to have well-conditioned system. We test correctness of 
     *       formulae here, not robustness of algorithm.
     *
     * NOTE: we have to use very relaxed thresholds for this test because
     *       each AGS iteration involves solution of several nested QP
     *       subproblems, so rounding errors accumulate too quickly.
     *       It does not mean that algorithm is inexact, just that two
     *       almost identical optimization sessions diverge too fast to
     *       compare them.
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=maxn; n++)
        {
            for(nc=1; nc<=n; nc++)
            {
                for(nec=0; nec<=nc; nec++)
                {
                    ae_vector_set_length(&x0, n, _state);
                    ae_vector_set_length(&x0s, n, _state);
                    ae_vector_set_length(&xc, n, _state);
                    ae_vector_set_length(&d, n, _state);
                    ae_vector_set_length(&r, n, _state);
                    ae_vector_set_length(&s, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                        xc.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                        d.ptr.p_double[i] = ae_pow((double)(10), ae_randomreal(_state)-0.5, _state);
                        s.ptr.p_double[i] = ae_pow((double)(10), ae_randomreal(_state)-0.5, _state);
                        r.ptr.p_double[i] = (2*ae_randominteger(2, _state)-1)*(0.1+ae_randomreal(_state));
                        x0s.ptr.p_double[i] = x0.ptr.p_double[i]*s.ptr.p_double[i];
                    }
                    minnscreate(n, &x0, &state, _state);
                    minnssetalgoags(&state, 0.1, 1.0, _state);
                    minnssetcond(&state, 0.0, testminnsunit_scalingtestcnt, _state);
                    minnssetnlc(&state, nec, nc-nec, _state);
                    while(minnsiteration(&state, _state))
                    {
                        if( state.needfij )
                        {
                            state.fi.ptr.p_double[0] = 0.0;
                            for(i=0; i<=n-1; i++)
                            {
                                state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                                state.j.ptr.pp_double[0][i] = d.ptr.p_double[i]*ae_sign(state.x.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                            }
                            for(i=1; i<=nc; i++)
                            {
                                state.fi.ptr.p_double[i] = state.x.ptr.p_double[i-1]*r.ptr.p_double[i-1];
                                for(j=0; j<=n-1; j++)
                                {
                                    state.j.ptr.pp_double[i][j] = 0.0;
                                }
                                state.j.ptr.pp_double[i][i-1] = r.ptr.p_double[i-1];
                            }
                            continue;
                        }
                        ae_assert(ae_false, "Assertion failed", _state);
                    }
                    minnsresults(&state, &x1, &rep, _state);
                    seterrorflag(primaryerrors, rep.terminationtype<=0, _state);
                    if( *primaryerrors||(*othererrors) )
                    {
                        ae_frame_leave(_state);
                        return;
                    }
                    minnssetscale(&state, &s, _state);
                    minnsrestartfrom(&state, &x0s, _state);
                    while(minnsiteration(&state, _state))
                    {
                        if( state.needfij )
                        {
                            state.fi.ptr.p_double[0] = 0.0;
                            for(i=0; i<=n-1; i++)
                            {
                                state.fi.ptr.p_double[0] = state.fi.ptr.p_double[0]+d.ptr.p_double[i]*ae_fabs(state.x.ptr.p_double[i]/s.ptr.p_double[i]-xc.ptr.p_double[i], _state);
                                state.j.ptr.pp_double[0][i] = d.ptr.p_double[i]*ae_sign(state.x.ptr.p_double[i]/s.ptr.p_double[i]-xc.ptr.p_double[i], _state)/s.ptr.p_double[i];
                            }
                            for(i=1; i<=nc; i++)
                            {
                                state.fi.ptr.p_double[i] = state.x.ptr.p_double[i-1]/s.ptr.p_double[i-1]*r.ptr.p_double[i-1];
                                for(j=0; j<=n-1; j++)
                                {
                                    state.j.ptr.pp_double[i][j] = 0.0;
                                }
                                state.j.ptr.pp_double[i][i-1] = r.ptr.p_double[i-1]/s.ptr.p_double[i-1];
                            }
                            continue;
                        }
                        ae_assert(ae_false, "Assertion failed", _state);
                    }
                    minnsresults(&state, &x1s, &rep, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        seterrorflag(primaryerrors, (!ae_isfinite(x1.ptr.p_double[i], _state)||!ae_isfinite(x1s.ptr.p_double[i], _state))||ae_fp_greater(ae_fabs(x1.ptr.p_double[i]-x1s.ptr.p_double[i]/s.ptr.p_double[i], _state),testminnsunit_scalingtesttol), _state);
                    }
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Special tests
*************************************************************************/
static void testminnsunit_testother(ae_bool* othererrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t k;
    ae_vector x0;
    ae_vector x1;
    minnsstate state;
    minnsreport rep;
    double v0;
    double v1;
    double v;
    double xtol;
    double epsrad;
    double rho;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    _minnsstate_init(&state, _state);
    _minnsreport_init(&rep, _state);

    
    /*
     * First test:
     * * 2D problem, minimization of F(x0,x1)=x1
     * * two constraints, with wildly different magnitudes
     *   * G0(x0,x1)=Rho*Abs(x0+x1)=0
     *   * H0(x0,x1)=Rho*(x0-1000)<=0
     *   where Rho is some large value
     *
     * Optimizer should be able to deal with situation when
     * magnitude of Jacobian components is so wildly different.
     */
    n = 2;
    ae_vector_set_length(&x0, n, _state);
    x0.ptr.p_double[0] = 0.1;
    x0.ptr.p_double[1] = 1.0;
    epsrad = 0.00001;
    xtol = 0.01;
    for(k=0; k<=6; k++)
    {
        rho = ae_pow((double)(10), (double)(k), _state);
        minnscreate(n, &x0, &state, _state);
        minnssetalgoags(&state, 0.1, 10.0, _state);
        minnssetcond(&state, epsrad, 0, _state);
        minnssetnlc(&state, 1, 1, _state);
        v = 1000.0;
        while(minnsiteration(&state, _state))
        {
            if( state.needfij )
            {
                v0 = state.x.ptr.p_double[0];
                v1 = state.x.ptr.p_double[1];
                state.fi.ptr.p_double[0] = v1;
                state.j.ptr.pp_double[0][0] = 0.0;
                state.j.ptr.pp_double[0][1] = 1.0;
                state.fi.ptr.p_double[1] = rho*ae_fabs(v0+v1, _state);
                state.j.ptr.pp_double[1][0] = rho*ae_sign(v0+v1, _state);
                state.j.ptr.pp_double[1][1] = rho*ae_sign(v0+v1, _state);
                state.fi.ptr.p_double[2] = rho*(v0-v);
                state.j.ptr.pp_double[2][0] = rho;
                state.j.ptr.pp_double[2][1] = 0.0;
                continue;
            }
            ae_assert(ae_false, "Assertion failed", _state);
        }
        minnsresults(&state, &x1, &rep, _state);
        seterrorflag(othererrors, rep.terminationtype<=0, _state);
        seterrorflag(othererrors, !ae_isfinite(x1.ptr.p_double[0], _state), _state);
        seterrorflag(othererrors, !ae_isfinite(x1.ptr.p_double[1], _state), _state);
        seterrorflag(othererrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[0]-v, _state),xtol), _state);
        seterrorflag(othererrors, ae_fp_greater(ae_fabs(x1.ptr.p_double[1]+v, _state),xtol), _state);
    }
    ae_frame_leave(_state);
}


/*$ End $*/
