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
#include "testlinregunit.h"


/*$ Declarations $*/
static void testlinregunit_generaterandomtask(double xl,
     double xr,
     ae_bool randomx,
     double ymin,
     double ymax,
     double smin,
     double smax,
     ae_int_t n,
     /* Real    */ ae_matrix* xy,
     /* Real    */ ae_vector* s,
     ae_state *_state);
static void testlinregunit_generatetask(double a,
     double b,
     double xl,
     double xr,
     ae_bool randomx,
     double smin,
     double smax,
     ae_int_t n,
     /* Real    */ ae_matrix* xy,
     /* Real    */ ae_vector* s,
     ae_state *_state);
static void testlinregunit_filltaskwithy(double a,
     double b,
     ae_int_t n,
     /* Real    */ ae_matrix* xy,
     /* Real    */ ae_vector* s,
     ae_state *_state);
static double testlinregunit_generatenormal(double mean,
     double sigma,
     ae_state *_state);
static void testlinregunit_calculatemv(/* Real    */ ae_vector* x,
     ae_int_t n,
     double* mean,
     double* means,
     double* stddev,
     double* stddevs,
     ae_state *_state);
static void testlinregunit_unsetlr(linearmodel* lr, ae_state *_state);


/*$ Body $*/


ae_bool testlinreg(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    double sigmathreshold;
    ae_int_t maxn;
    ae_int_t maxm;
    ae_int_t passcount;
    ae_int_t estpasscount;
    double threshold;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t tmpi;
    ae_int_t pass;
    ae_int_t epass;
    ae_int_t m;
    ae_int_t tasktype;
    ae_int_t modeltype;
    ae_int_t m1;
    ae_int_t m2;
    ae_int_t n1;
    ae_int_t n2;
    ae_int_t info;
    ae_int_t info2;
    ae_matrix xy;
    ae_matrix xy2;
    ae_vector s;
    ae_vector s2;
    ae_vector w2;
    ae_vector x;
    ae_vector ta;
    ae_vector tb;
    ae_vector tc;
    ae_vector xy0;
    ae_vector tmpweights;
    linearmodel w;
    linearmodel wt;
    linearmodel wt2;
    ae_vector x1;
    ae_vector x2;
    double y1;
    double y2;
    ae_bool allsame;
    double ea;
    double eb;
    double varatested;
    double varbtested;
    double a;
    double b;
    double vara;
    double varb;
    double a2;
    double b2;
    double covab;
    double corrab;
    double p;
    ae_int_t qcnt;
    ae_vector qtbl;
    ae_vector qvals;
    ae_vector qsigma;
    lrreport ar;
    lrreport ar2;
    double f;
    double fp;
    double fm;
    double v;
    double vv;
    double cvrmserror;
    double cvavgerror;
    double cvavgrelerror;
    double rmserror;
    double avgerror;
    double avgrelerror;
    ae_bool nondefect;
    double sinshift;
    double tasklevel;
    double noiselevel;
    double hstep;
    double sigma;
    double mean;
    double means;
    double stddev;
    double stddevs;
    ae_bool slcerrors;
    ae_bool slerrors;
    ae_bool grcoverrors;
    ae_bool gropterrors;
    ae_bool gresterrors;
    ae_bool grothererrors;
    ae_bool grconverrors;
    ae_bool waserrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xy2, 0, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&s2, 0, DT_REAL, _state);
    ae_vector_init(&w2, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&ta, 0, DT_REAL, _state);
    ae_vector_init(&tb, 0, DT_REAL, _state);
    ae_vector_init(&tc, 0, DT_REAL, _state);
    ae_vector_init(&xy0, 0, DT_REAL, _state);
    ae_vector_init(&tmpweights, 0, DT_REAL, _state);
    _linearmodel_init(&w, _state);
    _linearmodel_init(&wt, _state);
    _linearmodel_init(&wt2, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);
    ae_vector_init(&qtbl, 0, DT_REAL, _state);
    ae_vector_init(&qvals, 0, DT_REAL, _state);
    ae_vector_init(&qsigma, 0, DT_REAL, _state);
    _lrreport_init(&ar, _state);
    _lrreport_init(&ar2, _state);

    
    /*
     * Primary settings
     */
    maxn = 40;
    maxm = 5;
    passcount = 3;
    estpasscount = 1000;
    sigmathreshold = (double)(7);
    threshold = 1000000*ae_machineepsilon;
    slerrors = ae_false;
    slcerrors = ae_false;
    grcoverrors = ae_false;
    gropterrors = ae_false;
    gresterrors = ae_false;
    grothererrors = ae_false;
    grconverrors = ae_false;
    waserrors = ae_false;
    
    /*
     * Quantiles table setup
     */
    qcnt = 5;
    ae_vector_set_length(&qtbl, qcnt-1+1, _state);
    ae_vector_set_length(&qvals, qcnt-1+1, _state);
    ae_vector_set_length(&qsigma, qcnt-1+1, _state);
    qtbl.ptr.p_double[0] = 0.5;
    qtbl.ptr.p_double[1] = 0.25;
    qtbl.ptr.p_double[2] = 0.10;
    qtbl.ptr.p_double[3] = 0.05;
    qtbl.ptr.p_double[4] = 0.025;
    for(i=0; i<=qcnt-1; i++)
    {
        qsigma.ptr.p_double[i] = ae_sqrt(qtbl.ptr.p_double[i]*(1-qtbl.ptr.p_double[i])/estpasscount, _state);
    }
    
    /*
     * Other setup
     */
    ae_vector_set_length(&ta, estpasscount-1+1, _state);
    ae_vector_set_length(&tb, estpasscount-1+1, _state);
    
    /*
     * Test straight line regression
     */
    for(n=2; n<=maxn; n++)
    {
        
        /*
         * Fail/pass test
         */
        testlinregunit_generaterandomtask((double)(-1), (double)(1), ae_false, (double)(-1), (double)(1), (double)(1), (double)(2), n, &xy, &s, _state);
        lrlines(&xy, &s, n, &info, &a, &b, &vara, &varb, &covab, &corrab, &p, _state);
        slcerrors = slcerrors||info!=1;
        testlinregunit_generaterandomtask((double)(1), (double)(1), ae_false, (double)(-1), (double)(1), (double)(1), (double)(2), n, &xy, &s, _state);
        lrlines(&xy, &s, n, &info, &a, &b, &vara, &varb, &covab, &corrab, &p, _state);
        slcerrors = slcerrors||info!=-3;
        testlinregunit_generaterandomtask((double)(-1), (double)(1), ae_false, (double)(-1), (double)(1), (double)(-1), (double)(-1), n, &xy, &s, _state);
        lrlines(&xy, &s, n, &info, &a, &b, &vara, &varb, &covab, &corrab, &p, _state);
        slcerrors = slcerrors||info!=-2;
        testlinregunit_generaterandomtask((double)(-1), (double)(1), ae_false, (double)(-1), (double)(1), (double)(2), (double)(1), 2, &xy, &s, _state);
        lrlines(&xy, &s, 1, &info, &a, &b, &vara, &varb, &covab, &corrab, &p, _state);
        slcerrors = slcerrors||info!=-1;
        
        /*
         * Multipass tests
         */
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             * Test S variant against non-S variant
             */
            ea = 2*ae_randomreal(_state)-1;
            eb = 2*ae_randomreal(_state)-1;
            testlinregunit_generatetask(ea, eb, -5*ae_randomreal(_state), 5*ae_randomreal(_state), ae_fp_greater(ae_randomreal(_state),0.5), (double)(1), (double)(1), n, &xy, &s, _state);
            lrlines(&xy, &s, n, &info, &a, &b, &vara, &varb, &covab, &corrab, &p, _state);
            lrline(&xy, n, &info2, &a2, &b2, _state);
            if( info!=1||info2!=1 )
            {
                slcerrors = ae_true;
            }
            else
            {
                slerrors = (slerrors||ae_fp_greater(ae_fabs(a-a2, _state),threshold))||ae_fp_greater(ae_fabs(b-b2, _state),threshold);
            }
            
            /*
             * Test for A/B
             *
             * Generate task with exact, non-perturbed y[i],
             * then make non-zero s[i]
             */
            ea = 2*ae_randomreal(_state)-1;
            eb = 2*ae_randomreal(_state)-1;
            testlinregunit_generatetask(ea, eb, -5*ae_randomreal(_state), 5*ae_randomreal(_state), n>4, 0.0, 0.0, n, &xy, &s, _state);
            for(i=0; i<=n-1; i++)
            {
                s.ptr.p_double[i] = 1+ae_randomreal(_state);
            }
            lrlines(&xy, &s, n, &info, &a, &b, &vara, &varb, &covab, &corrab, &p, _state);
            if( info!=1 )
            {
                slcerrors = ae_true;
            }
            else
            {
                slerrors = (slerrors||ae_fp_greater(ae_fabs(a-ea, _state),0.001))||ae_fp_greater(ae_fabs(b-eb, _state),0.001);
            }
            
            /*
             * Test for VarA, VarB, P (P is being tested only for N>2)
             */
            for(i=0; i<=qcnt-1; i++)
            {
                qvals.ptr.p_double[i] = (double)(0);
            }
            ea = 2*ae_randomreal(_state)-1;
            eb = 2*ae_randomreal(_state)-1;
            testlinregunit_generatetask(ea, eb, -5*ae_randomreal(_state), 5*ae_randomreal(_state), n>4, 1.0, 2.0, n, &xy, &s, _state);
            lrlines(&xy, &s, n, &info, &a, &b, &vara, &varb, &covab, &corrab, &p, _state);
            if( info!=1 )
            {
                slcerrors = ae_true;
                continue;
            }
            varatested = vara;
            varbtested = varb;
            for(epass=0; epass<=estpasscount-1; epass++)
            {
                
                /*
                 * Generate
                 */
                testlinregunit_filltaskwithy(ea, eb, n, &xy, &s, _state);
                lrlines(&xy, &s, n, &info, &a, &b, &vara, &varb, &covab, &corrab, &p, _state);
                if( info!=1 )
                {
                    slcerrors = ae_true;
                    continue;
                }
                
                /*
                 * A, B, P
                 * (P is being tested for uniformity, additional p-tests are below)
                 */
                ta.ptr.p_double[epass] = a;
                tb.ptr.p_double[epass] = b;
                for(i=0; i<=qcnt-1; i++)
                {
                    if( ae_fp_less_eq(p,qtbl.ptr.p_double[i]) )
                    {
                        qvals.ptr.p_double[i] = qvals.ptr.p_double[i]+(double)1/(double)estpasscount;
                    }
                }
            }
            testlinregunit_calculatemv(&ta, estpasscount, &mean, &means, &stddev, &stddevs, _state);
            slerrors = slerrors||ae_fp_greater_eq(ae_fabs(mean-ea, _state)/means,sigmathreshold);
            slerrors = slerrors||ae_fp_greater_eq(ae_fabs(stddev-ae_sqrt(varatested, _state), _state)/stddevs,sigmathreshold);
            testlinregunit_calculatemv(&tb, estpasscount, &mean, &means, &stddev, &stddevs, _state);
            slerrors = slerrors||ae_fp_greater_eq(ae_fabs(mean-eb, _state)/means,sigmathreshold);
            slerrors = slerrors||ae_fp_greater_eq(ae_fabs(stddev-ae_sqrt(varbtested, _state), _state)/stddevs,sigmathreshold);
            if( n>2 )
            {
                for(i=0; i<=qcnt-1; i++)
                {
                    if( ae_fp_greater(ae_fabs(qtbl.ptr.p_double[i]-qvals.ptr.p_double[i], _state)/qsigma.ptr.p_double[i],sigmathreshold) )
                    {
                        slerrors = ae_true;
                    }
                }
            }
            
            /*
             * Additional tests for P: correlation with fit quality
             */
            if( n>2 )
            {
                testlinregunit_generatetask(ea, eb, -5*ae_randomreal(_state), 5*ae_randomreal(_state), ae_false, 0.0, 0.0, n, &xy, &s, _state);
                for(i=0; i<=n-1; i++)
                {
                    s.ptr.p_double[i] = 1+ae_randomreal(_state);
                }
                lrlines(&xy, &s, n, &info, &a, &b, &vara, &varb, &covab, &corrab, &p, _state);
                if( info!=1 )
                {
                    slcerrors = ae_true;
                    continue;
                }
                slerrors = slerrors||ae_fp_less(p,0.999);
                testlinregunit_generatetask((double)(0), (double)(0), -5*ae_randomreal(_state), 5*ae_randomreal(_state), ae_false, 1.0, 1.0, n, &xy, &s, _state);
                for(i=0; i<=n-1; i++)
                {
                    if( i%2==0 )
                    {
                        xy.ptr.pp_double[i][1] = 5.0;
                    }
                    else
                    {
                        xy.ptr.pp_double[i][1] = -5.0;
                    }
                }
                if( n%2!=0 )
                {
                    xy.ptr.pp_double[n-1][1] = (double)(0);
                }
                lrlines(&xy, &s, n, &info, &a, &b, &vara, &varb, &covab, &corrab, &p, _state);
                if( info!=1 )
                {
                    slcerrors = ae_true;
                    continue;
                }
                slerrors = slerrors||ae_fp_greater(p,0.001);
            }
        }
    }
    
    /*
     * General regression tests:
     */
    
    /*
     * Simple linear tests (small sample, optimum point, covariance)
     */
    for(n=3; n<=maxn; n++)
    {
        ae_vector_set_length(&s, n-1+1, _state);
        
        /*
         * Linear tests:
         * a. random points, sigmas
         * b. no sigmas
         */
        ae_matrix_set_length(&xy, n-1+1, 1+1, _state);
        for(i=0; i<=n-1; i++)
        {
            xy.ptr.pp_double[i][0] = 2*ae_randomreal(_state)-1;
            xy.ptr.pp_double[i][1] = 2*ae_randomreal(_state)-1;
            s.ptr.p_double[i] = 1+ae_randomreal(_state);
        }
        lrbuilds(&xy, &s, n, 1, &info, &wt, &ar, _state);
        if( info!=1 )
        {
            grconverrors = ae_true;
            continue;
        }
        lrunpack(&wt, &tmpweights, &tmpi, _state);
        lrlines(&xy, &s, n, &info2, &a, &b, &vara, &varb, &covab, &corrab, &p, _state);
        gropterrors = gropterrors||ae_fp_greater(ae_fabs(a-tmpweights.ptr.p_double[1], _state),threshold);
        gropterrors = gropterrors||ae_fp_greater(ae_fabs(b-tmpweights.ptr.p_double[0], _state),threshold);
        grcoverrors = grcoverrors||ae_fp_greater(ae_fabs(vara-ar.c.ptr.pp_double[1][1], _state),threshold);
        grcoverrors = grcoverrors||ae_fp_greater(ae_fabs(varb-ar.c.ptr.pp_double[0][0], _state),threshold);
        grcoverrors = grcoverrors||ae_fp_greater(ae_fabs(covab-ar.c.ptr.pp_double[1][0], _state),threshold);
        grcoverrors = grcoverrors||ae_fp_greater(ae_fabs(covab-ar.c.ptr.pp_double[0][1], _state),threshold);
        lrbuild(&xy, n, 1, &info, &wt, &ar, _state);
        if( info!=1 )
        {
            grconverrors = ae_true;
            continue;
        }
        lrunpack(&wt, &tmpweights, &tmpi, _state);
        lrline(&xy, n, &info2, &a, &b, _state);
        gropterrors = gropterrors||ae_fp_greater(ae_fabs(a-tmpweights.ptr.p_double[1], _state),threshold);
        gropterrors = gropterrors||ae_fp_greater(ae_fabs(b-tmpweights.ptr.p_double[0], _state),threshold);
    }
    
    /*
     * S covariance versus S-less covariance.
     * Slightly skewed task, large sample size.
     * Will S-less subroutine estimate covariance matrix good enough?
     */
    n = 1000+ae_randominteger(3000, _state);
    sigma = 0.1+ae_randomreal(_state)*1.9;
    ae_matrix_set_length(&xy, n-1+1, 1+1, _state);
    ae_vector_set_length(&s, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        xy.ptr.pp_double[i][0] = 1.5*ae_randomreal(_state)-0.5;
        xy.ptr.pp_double[i][1] = 1.2*xy.ptr.pp_double[i][0]-0.3+testlinregunit_generatenormal((double)(0), sigma, _state);
        s.ptr.p_double[i] = sigma;
    }
    lrbuild(&xy, n, 1, &info, &wt, &ar, _state);
    lrlines(&xy, &s, n, &info2, &a, &b, &vara, &varb, &covab, &corrab, &p, _state);
    if( info!=1||info2!=1 )
    {
        grconverrors = ae_true;
    }
    else
    {
        grcoverrors = grcoverrors||ae_fp_greater(ae_fabs(ae_log(ar.c.ptr.pp_double[0][0]/varb, _state), _state),ae_log(1.2, _state));
        grcoverrors = grcoverrors||ae_fp_greater(ae_fabs(ae_log(ar.c.ptr.pp_double[1][1]/vara, _state), _state),ae_log(1.2, _state));
        grcoverrors = grcoverrors||ae_fp_greater(ae_fabs(ae_log(ar.c.ptr.pp_double[0][1]/covab, _state), _state),ae_log(1.2, _state));
        grcoverrors = grcoverrors||ae_fp_greater(ae_fabs(ae_log(ar.c.ptr.pp_double[1][0]/covab, _state), _state),ae_log(1.2, _state));
    }
    
    /*
     * General tests:
     * * basis functions - up to cubic
     * * task types:
     * * data set is noisy sine half-period with random shift
     * * tests:
     *   unpacking/packing
     *   optimality
     *   error estimates
     * * tasks:
     *   0 = noised sine
     *   1 = degenerate task with 1-of-n encoded categorical variables
     *   2 = random task with large variation (for 1-type models)
     *   3 = random task with small variation (for 1-type models)
     *
     *   Additional tasks TODO
     *   specially designed task with defective vectors which leads to
     *   the failure of the fast CV formula.
     *
     */
    m1 = 0;
    m2 = -1;
    n1 = 0;
    n2 = -1;
    for(modeltype=0; modeltype<=1; modeltype++)
    {
        for(tasktype=0; tasktype<=3; tasktype++)
        {
            if( tasktype==0 )
            {
                m1 = 1;
                m2 = 3;
            }
            if( tasktype==1 )
            {
                m1 = 9;
                m2 = 9;
            }
            if( tasktype==2||tasktype==3 )
            {
                m1 = 9;
                m2 = 9;
            }
            for(m=m1; m<=m2; m++)
            {
                if( tasktype==0 )
                {
                    n1 = m+3;
                    n2 = m+20;
                }
                if( tasktype==1 )
                {
                    n1 = 70+ae_randominteger(70, _state);
                    n2 = n1;
                }
                if( tasktype==2||tasktype==3 )
                {
                    n1 = 100;
                    n2 = n1;
                }
                for(n=n1; n<=n2; n++)
                {
                    ae_matrix_set_length(&xy, n-1+1, m+1, _state);
                    ae_vector_set_length(&xy0, n-1+1, _state);
                    ae_vector_set_length(&s, n-1+1, _state);
                    hstep = 0.001;
                    noiselevel = 0.2;
                    
                    /*
                     * Prepare task
                     */
                    if( tasktype==0 )
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            xy.ptr.pp_double[i][0] = 2*ae_randomreal(_state)-1;
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=1; j<=m-1; j++)
                            {
                                xy.ptr.pp_double[i][j] = xy.ptr.pp_double[i][0]*xy.ptr.pp_double[i][j-1];
                            }
                        }
                        sinshift = ae_randomreal(_state)*ae_pi;
                        for(i=0; i<=n-1; i++)
                        {
                            xy0.ptr.p_double[i] = ae_sin(sinshift+ae_pi*0.5*(xy.ptr.pp_double[i][0]+1), _state);
                            xy.ptr.pp_double[i][m] = xy0.ptr.p_double[i]+noiselevel*testlinregunit_generatenormal((double)(0), (double)(1), _state);
                        }
                    }
                    if( tasktype==1 )
                    {
                        ae_assert(m==9, "Assertion failed", _state);
                        ae_vector_set_length(&ta, 8+1, _state);
                        ta.ptr.p_double[0] = (double)(1);
                        ta.ptr.p_double[1] = (double)(2);
                        ta.ptr.p_double[2] = (double)(3);
                        ta.ptr.p_double[3] = 0.25;
                        ta.ptr.p_double[4] = 0.5;
                        ta.ptr.p_double[5] = 0.75;
                        ta.ptr.p_double[6] = 0.06;
                        ta.ptr.p_double[7] = 0.12;
                        ta.ptr.p_double[8] = 0.18;
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                xy.ptr.pp_double[i][j] = (double)(0);
                            }
                            xy.ptr.pp_double[i][0+i%3] = (double)(1);
                            xy.ptr.pp_double[i][3+i/3%3] = (double)(1);
                            xy.ptr.pp_double[i][6+i/9%3] = (double)(1);
                            v = ae_v_dotproduct(&xy.ptr.pp_double[i][0], 1, &ta.ptr.p_double[0], 1, ae_v_len(0,8));
                            xy0.ptr.p_double[i] = v;
                            xy.ptr.pp_double[i][m] = v+noiselevel*testlinregunit_generatenormal((double)(0), (double)(1), _state);
                        }
                    }
                    if( tasktype==2||tasktype==3 )
                    {
                        ae_assert(m==9, "Assertion failed", _state);
                        ae_vector_set_length(&ta, 8+1, _state);
                        ta.ptr.p_double[0] = (double)(1);
                        ta.ptr.p_double[1] = (double)(-2);
                        ta.ptr.p_double[2] = (double)(3);
                        ta.ptr.p_double[3] = 0.25;
                        ta.ptr.p_double[4] = -0.5;
                        ta.ptr.p_double[5] = 0.75;
                        ta.ptr.p_double[6] = -0.06;
                        ta.ptr.p_double[7] = 0.12;
                        ta.ptr.p_double[8] = -0.18;
                        for(i=0; i<=n-1; i++)
                        {
                            for(j=0; j<=m-1; j++)
                            {
                                if( tasktype==2 )
                                {
                                    xy.ptr.pp_double[i][j] = 1+testlinregunit_generatenormal((double)(0), (double)(3), _state);
                                }
                                else
                                {
                                    xy.ptr.pp_double[i][j] = 1+testlinregunit_generatenormal((double)(0), 0.05, _state);
                                }
                            }
                            v = ae_v_dotproduct(&xy.ptr.pp_double[i][0], 1, &ta.ptr.p_double[0], 1, ae_v_len(0,8));
                            xy0.ptr.p_double[i] = v;
                            xy.ptr.pp_double[i][m] = v+noiselevel*testlinregunit_generatenormal((double)(0), (double)(1), _state);
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        s.ptr.p_double[i] = 1+ae_randomreal(_state);
                    }
                    
                    /*
                     * Solve (using S-variant, non-S-variant is not tested)
                     */
                    if( modeltype==0 )
                    {
                        lrbuilds(&xy, &s, n, m, &info, &wt, &ar, _state);
                    }
                    else
                    {
                        lrbuildzs(&xy, &s, n, m, &info, &wt, &ar, _state);
                    }
                    if( info!=1 )
                    {
                        grconverrors = ae_true;
                        continue;
                    }
                    lrunpack(&wt, &tmpweights, &tmpi, _state);
                    
                    /*
                     * LRProcess test
                     */
                    ae_vector_set_length(&x, m-1+1, _state);
                    v = tmpweights.ptr.p_double[m];
                    for(i=0; i<=m-1; i++)
                    {
                        x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                        v = v+tmpweights.ptr.p_double[i]*x.ptr.p_double[i];
                    }
                    grothererrors = grothererrors||ae_fp_greater(ae_fabs(v-lrprocess(&wt, &x, _state), _state)/ae_maxreal(ae_fabs(v, _state), (double)(1), _state),threshold);
                    
                    /*
                     * LRPack test
                     */
                    lrpack(&tmpweights, m, &wt2, _state);
                    ae_vector_set_length(&x, m-1+1, _state);
                    for(i=0; i<=m-1; i++)
                    {
                        x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    }
                    v = lrprocess(&wt, &x, _state);
                    grothererrors = grothererrors||ae_fp_greater(ae_fabs(v-lrprocess(&wt2, &x, _state), _state)/ae_fabs(v, _state),threshold);
                    
                    /*
                     * Optimality test
                     */
                    for(k=0; k<=m; k++)
                    {
                        if( modeltype==1&&k==m )
                        {
                            
                            /*
                             * 0-type models (with non-zero constant term)
                             * are tested for optimality of all coefficients.
                             *
                             * 1-type models (with zero constant term)
                             * are tested for optimality of non-constant terms only.
                             */
                            continue;
                        }
                        f = (double)(0);
                        fp = (double)(0);
                        fm = (double)(0);
                        for(i=0; i<=n-1; i++)
                        {
                            v = tmpweights.ptr.p_double[m];
                            for(j=0; j<=m-1; j++)
                            {
                                v = v+xy.ptr.pp_double[i][j]*tmpweights.ptr.p_double[j];
                            }
                            f = f+ae_sqr((v-xy.ptr.pp_double[i][m])/s.ptr.p_double[i], _state);
                            if( k<m )
                            {
                                vv = xy.ptr.pp_double[i][k];
                            }
                            else
                            {
                                vv = (double)(1);
                            }
                            fp = fp+ae_sqr((v+vv*hstep-xy.ptr.pp_double[i][m])/s.ptr.p_double[i], _state);
                            fm = fm+ae_sqr((v-vv*hstep-xy.ptr.pp_double[i][m])/s.ptr.p_double[i], _state);
                        }
                        gropterrors = (gropterrors||ae_fp_greater(f,fp))||ae_fp_greater(f,fm);
                    }
                    
                    /*
                     * Covariance matrix test:
                     * generate random vector, project coefficients on it,
                     * compare variance of projection with estimate provided
                     * by cov.matrix
                     */
                    ae_vector_set_length(&ta, estpasscount-1+1, _state);
                    ae_vector_set_length(&tb, m+1, _state);
                    ae_vector_set_length(&tc, m+1, _state);
                    ae_matrix_set_length(&xy2, n-1+1, m+1, _state);
                    for(i=0; i<=m; i++)
                    {
                        tb.ptr.p_double[i] = testlinregunit_generatenormal((double)(0), (double)(1), _state);
                    }
                    for(epass=0; epass<=estpasscount-1; epass++)
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            ae_v_move(&xy2.ptr.pp_double[i][0], 1, &xy.ptr.pp_double[i][0], 1, ae_v_len(0,m-1));
                            xy2.ptr.pp_double[i][m] = xy0.ptr.p_double[i]+s.ptr.p_double[i]*testlinregunit_generatenormal((double)(0), (double)(1), _state);
                        }
                        if( modeltype==0 )
                        {
                            lrbuilds(&xy2, &s, n, m, &info, &wt, &ar2, _state);
                        }
                        else
                        {
                            lrbuildzs(&xy2, &s, n, m, &info, &wt, &ar2, _state);
                        }
                        if( info!=1 )
                        {
                            ta.ptr.p_double[epass] = (double)(0);
                            grconverrors = ae_true;
                            continue;
                        }
                        lrunpack(&wt, &w2, &tmpi, _state);
                        v = ae_v_dotproduct(&tb.ptr.p_double[0], 1, &w2.ptr.p_double[0], 1, ae_v_len(0,m));
                        ta.ptr.p_double[epass] = v;
                    }
                    testlinregunit_calculatemv(&ta, estpasscount, &mean, &means, &stddev, &stddevs, _state);
                    for(i=0; i<=m; i++)
                    {
                        v = ae_v_dotproduct(&tb.ptr.p_double[0], 1, &ar.c.ptr.pp_double[0][i], ar.c.stride, ae_v_len(0,m));
                        tc.ptr.p_double[i] = v;
                    }
                    v = ae_v_dotproduct(&tc.ptr.p_double[0], 1, &tb.ptr.p_double[0], 1, ae_v_len(0,m));
                    grcoverrors = grcoverrors||ae_fp_greater_eq(ae_fabs((ae_sqrt(v, _state)-stddev)/stddevs, _state),sigmathreshold);
                    
                    /*
                     * Test for the fast CV error:
                     * calculate CV error by definition (leaving out N
                     * points and recalculating solution).
                     *
                     * Test for the training set error
                     */
                    cvrmserror = (double)(0);
                    cvavgerror = (double)(0);
                    cvavgrelerror = (double)(0);
                    rmserror = (double)(0);
                    avgerror = (double)(0);
                    avgrelerror = (double)(0);
                    ae_matrix_set_length(&xy2, n-2+1, m+1, _state);
                    ae_vector_set_length(&s2, n-2+1, _state);
                    for(i=0; i<=n-2; i++)
                    {
                        ae_v_move(&xy2.ptr.pp_double[i][0], 1, &xy.ptr.pp_double[i+1][0], 1, ae_v_len(0,m));
                        s2.ptr.p_double[i] = s.ptr.p_double[i+1];
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        
                        /*
                         * Trn
                         */
                        v = ae_v_dotproduct(&xy.ptr.pp_double[i][0], 1, &tmpweights.ptr.p_double[0], 1, ae_v_len(0,m-1));
                        v = v+tmpweights.ptr.p_double[m];
                        rmserror = rmserror+ae_sqr(v-xy.ptr.pp_double[i][m], _state);
                        avgerror = avgerror+ae_fabs(v-xy.ptr.pp_double[i][m], _state);
                        avgrelerror = avgrelerror+ae_fabs((v-xy.ptr.pp_double[i][m])/xy.ptr.pp_double[i][m], _state);
                        
                        /*
                         * CV: non-defect vectors only
                         */
                        nondefect = ae_true;
                        for(k=0; k<=ar.ncvdefects-1; k++)
                        {
                            if( ar.cvdefects.ptr.p_int[k]==i )
                            {
                                nondefect = ae_false;
                            }
                        }
                        if( nondefect )
                        {
                            if( modeltype==0 )
                            {
                                lrbuilds(&xy2, &s2, n-1, m, &info2, &wt, &ar2, _state);
                            }
                            else
                            {
                                lrbuildzs(&xy2, &s2, n-1, m, &info2, &wt, &ar2, _state);
                            }
                            if( info2!=1 )
                            {
                                grconverrors = ae_true;
                                continue;
                            }
                            lrunpack(&wt, &w2, &tmpi, _state);
                            v = ae_v_dotproduct(&xy.ptr.pp_double[i][0], 1, &w2.ptr.p_double[0], 1, ae_v_len(0,m-1));
                            v = v+w2.ptr.p_double[m];
                            cvrmserror = cvrmserror+ae_sqr(v-xy.ptr.pp_double[i][m], _state);
                            cvavgerror = cvavgerror+ae_fabs(v-xy.ptr.pp_double[i][m], _state);
                            cvavgrelerror = cvavgrelerror+ae_fabs((v-xy.ptr.pp_double[i][m])/xy.ptr.pp_double[i][m], _state);
                        }
                        
                        /*
                         * Next set
                         */
                        if( i!=n-1 )
                        {
                            ae_v_move(&xy2.ptr.pp_double[i][0], 1, &xy.ptr.pp_double[i][0], 1, ae_v_len(0,m));
                            s2.ptr.p_double[i] = s.ptr.p_double[i];
                        }
                    }
                    cvrmserror = ae_sqrt(cvrmserror/(n-ar.ncvdefects), _state);
                    cvavgerror = cvavgerror/(n-ar.ncvdefects);
                    cvavgrelerror = cvavgrelerror/(n-ar.ncvdefects);
                    rmserror = ae_sqrt(rmserror/n, _state);
                    avgerror = avgerror/n;
                    avgrelerror = avgrelerror/n;
                    gresterrors = gresterrors||ae_fp_greater(ae_fabs(ae_log(ar.cvrmserror/cvrmserror, _state), _state),ae_log(1+1.0E-5, _state));
                    gresterrors = gresterrors||ae_fp_greater(ae_fabs(ae_log(ar.cvavgerror/cvavgerror, _state), _state),ae_log(1+1.0E-5, _state));
                    gresterrors = gresterrors||ae_fp_greater(ae_fabs(ae_log(ar.cvavgrelerror/cvavgrelerror, _state), _state),ae_log(1+1.0E-5, _state));
                    gresterrors = gresterrors||ae_fp_greater(ae_fabs(ae_log(ar.rmserror/rmserror, _state), _state),ae_log(1+1.0E-5, _state));
                    gresterrors = gresterrors||ae_fp_greater(ae_fabs(ae_log(ar.avgerror/avgerror, _state), _state),ae_log(1+1.0E-5, _state));
                    gresterrors = gresterrors||ae_fp_greater(ae_fabs(ae_log(ar.avgrelerror/avgrelerror, _state), _state),ae_log(1+1.0E-5, _state));
                }
            }
        }
    }
    
    /*
     * Additional subroutines
     */
    for(pass=1; pass<=50; pass++)
    {
        n = 2;
        do
        {
            noiselevel = ae_randomreal(_state)+0.1;
            tasklevel = 2*ae_randomreal(_state)-1;
        }
        while(ae_fp_less_eq(ae_fabs(noiselevel-tasklevel, _state),0.05));
        ae_matrix_set_length(&xy, 3*n-1+1, 1+1, _state);
        for(i=0; i<=n-1; i++)
        {
            xy.ptr.pp_double[3*i+0][0] = (double)(i);
            xy.ptr.pp_double[3*i+1][0] = (double)(i);
            xy.ptr.pp_double[3*i+2][0] = (double)(i);
            xy.ptr.pp_double[3*i+0][1] = tasklevel-noiselevel;
            xy.ptr.pp_double[3*i+1][1] = tasklevel;
            xy.ptr.pp_double[3*i+2][1] = tasklevel+noiselevel;
        }
        lrbuild(&xy, 3*n, 1, &info, &wt, &ar, _state);
        if( info==1 )
        {
            lrunpack(&wt, &tmpweights, &tmpi, _state);
            v = lrrmserror(&wt, &xy, 3*n, _state);
            grothererrors = grothererrors||ae_fp_greater(ae_fabs(v-noiselevel*ae_sqrt((double)2/(double)3, _state), _state),threshold);
            v = lravgerror(&wt, &xy, 3*n, _state);
            grothererrors = grothererrors||ae_fp_greater(ae_fabs(v-noiselevel*((double)2/(double)3), _state),threshold);
            v = lravgrelerror(&wt, &xy, 3*n, _state);
            vv = (ae_fabs(noiselevel/(tasklevel-noiselevel), _state)+ae_fabs(noiselevel/(tasklevel+noiselevel), _state))/3;
            grothererrors = grothererrors||ae_fp_greater(ae_fabs(v-vv, _state),threshold*vv);
        }
        else
        {
            grothererrors = ae_true;
        }
        for(i=0; i<=n-1; i++)
        {
            xy.ptr.pp_double[3*i+0][0] = (double)(i);
            xy.ptr.pp_double[3*i+1][0] = (double)(i);
            xy.ptr.pp_double[3*i+2][0] = (double)(i);
            xy.ptr.pp_double[3*i+0][1] = -noiselevel;
            xy.ptr.pp_double[3*i+1][1] = (double)(0);
            xy.ptr.pp_double[3*i+2][1] = noiselevel;
        }
        lrbuild(&xy, 3*n, 1, &info, &wt, &ar, _state);
        if( info==1 )
        {
            lrunpack(&wt, &tmpweights, &tmpi, _state);
            v = lravgrelerror(&wt, &xy, 3*n, _state);
            grothererrors = grothererrors||ae_fp_greater(ae_fabs(v-1, _state),threshold);
        }
        else
        {
            grothererrors = ae_true;
        }
    }
    for(pass=1; pass<=10; pass++)
    {
        m = 1+ae_randominteger(5, _state);
        n = 10+ae_randominteger(10, _state);
        ae_matrix_set_length(&xy, n-1+1, m+1, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m; j++)
            {
                xy.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
        }
        lrbuild(&xy, n, m, &info, &w, &ar, _state);
        if( info<0 )
        {
            grothererrors = ae_true;
            break;
        }
        ae_vector_set_length(&x1, m-1+1, _state);
        ae_vector_set_length(&x2, m-1+1, _state);
        
        /*
         * Same inputs on original leads to same outputs
         * on copy created using LRCopy
         */
        testlinregunit_unsetlr(&wt, _state);
        lrcopy(&w, &wt, _state);
        for(i=0; i<=m-1; i++)
        {
            x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            x2.ptr.p_double[i] = x1.ptr.p_double[i];
        }
        y1 = lrprocess(&w, &x1, _state);
        y2 = lrprocess(&wt, &x2, _state);
        allsame = ae_fp_eq(y1,y2);
        grothererrors = grothererrors||!allsame;
    }
    
    /*
     * TODO: Degenerate tests (when design matrix and right part are zero)
     */
    
    /*
     * Final report
     */
    waserrors = (((((slerrors||slcerrors)||gropterrors)||grcoverrors)||gresterrors)||grothererrors)||grconverrors;
    if( !silent )
    {
        printf("REGRESSION TEST\n");
        printf("STRAIGHT LINE REGRESSION:                ");
        if( !slerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("STRAIGHT LINE REGRESSION CONVERGENCE:    ");
        if( !slcerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("GENERAL LINEAR REGRESSION:               ");
        if( !((((gropterrors||grcoverrors)||gresterrors)||grothererrors)||grconverrors) )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* OPTIMALITY:                            ");
        if( !gropterrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* COV. MATRIX:                           ");
        if( !grcoverrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* ERROR ESTIMATES:                       ");
        if( !gresterrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* CONVERGENCE:                           ");
        if( !grconverrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* OTHER SUBROUTINES:                     ");
        if( !grothererrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        if( waserrors )
        {
            printf("TEST SUMMARY: FAILED\n");
        }
        else
        {
            printf("TEST SUMMARY: PASSED\n");
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
ae_bool _pexec_testlinreg(ae_bool silent, ae_state *_state)
{
    return testlinreg(silent, _state);
}


/*************************************************************************
Task generation. Meaningless task, just random numbers.
*************************************************************************/
static void testlinregunit_generaterandomtask(double xl,
     double xr,
     ae_bool randomx,
     double ymin,
     double ymax,
     double smin,
     double smax,
     ae_int_t n,
     /* Real    */ ae_matrix* xy,
     /* Real    */ ae_vector* s,
     ae_state *_state)
{
    ae_int_t i;


    ae_matrix_set_length(xy, n-1+1, 1+1, _state);
    ae_vector_set_length(s, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        if( randomx )
        {
            xy->ptr.pp_double[i][0] = xl+(xr-xl)*ae_randomreal(_state);
        }
        else
        {
            xy->ptr.pp_double[i][0] = xl+(xr-xl)*i/(n-1);
        }
        xy->ptr.pp_double[i][1] = ymin+(ymax-ymin)*ae_randomreal(_state);
        s->ptr.p_double[i] = smin+(smax-smin)*ae_randomreal(_state);
    }
}


/*************************************************************************
Task generation.
*************************************************************************/
static void testlinregunit_generatetask(double a,
     double b,
     double xl,
     double xr,
     ae_bool randomx,
     double smin,
     double smax,
     ae_int_t n,
     /* Real    */ ae_matrix* xy,
     /* Real    */ ae_vector* s,
     ae_state *_state)
{
    ae_int_t i;


    ae_matrix_set_length(xy, n-1+1, 1+1, _state);
    ae_vector_set_length(s, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        if( randomx )
        {
            xy->ptr.pp_double[i][0] = xl+(xr-xl)*ae_randomreal(_state);
        }
        else
        {
            xy->ptr.pp_double[i][0] = xl+(xr-xl)*i/(n-1);
        }
        s->ptr.p_double[i] = smin+(smax-smin)*ae_randomreal(_state);
        xy->ptr.pp_double[i][1] = a+b*xy->ptr.pp_double[i][0]+testlinregunit_generatenormal((double)(0), s->ptr.p_double[i], _state);
    }
}


/*************************************************************************
Task generation.
y[i] are filled based on A, B, X[I], S[I]
*************************************************************************/
static void testlinregunit_filltaskwithy(double a,
     double b,
     ae_int_t n,
     /* Real    */ ae_matrix* xy,
     /* Real    */ ae_vector* s,
     ae_state *_state)
{
    ae_int_t i;


    for(i=0; i<=n-1; i++)
    {
        xy->ptr.pp_double[i][1] = a+b*xy->ptr.pp_double[i][0]+testlinregunit_generatenormal((double)(0), s->ptr.p_double[i], _state);
    }
}


/*************************************************************************
Normal random numbers
*************************************************************************/
static double testlinregunit_generatenormal(double mean,
     double sigma,
     ae_state *_state)
{
    double u;
    double v;
    double sum;
    double result;


    result = mean;
    for(;;)
    {
        u = (2*ae_randominteger(2, _state)-1)*ae_randomreal(_state);
        v = (2*ae_randominteger(2, _state)-1)*ae_randomreal(_state);
        sum = u*u+v*v;
        if( ae_fp_less(sum,(double)(1))&&ae_fp_greater(sum,(double)(0)) )
        {
            sum = ae_sqrt(-2*ae_log(sum, _state)/sum, _state);
            result = sigma*u*sum+mean;
            return result;
        }
    }
    return result;
}


/*************************************************************************
Moments estimates and their errors
*************************************************************************/
static void testlinregunit_calculatemv(/* Real    */ ae_vector* x,
     ae_int_t n,
     double* mean,
     double* means,
     double* stddev,
     double* stddevs,
     ae_state *_state)
{
    ae_int_t i;
    double v1;
    double v2;
    double variance;

    *mean = 0;
    *means = 0;
    *stddev = 0;
    *stddevs = 0;

    *mean = (double)(0);
    *means = (double)(1);
    *stddev = (double)(0);
    *stddevs = (double)(1);
    variance = (double)(0);
    if( n<=1 )
    {
        return;
    }
    
    /*
     * Mean
     */
    for(i=0; i<=n-1; i++)
    {
        *mean = *mean+x->ptr.p_double[i];
    }
    *mean = *mean/n;
    
    /*
     * Variance (using corrected two-pass algorithm)
     */
    if( n!=1 )
    {
        v1 = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            v1 = v1+ae_sqr(x->ptr.p_double[i]-(*mean), _state);
        }
        v2 = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            v2 = v2+(x->ptr.p_double[i]-(*mean));
        }
        v2 = ae_sqr(v2, _state)/n;
        variance = (v1-v2)/(n-1);
        if( ae_fp_less(variance,(double)(0)) )
        {
            variance = (double)(0);
        }
        *stddev = ae_sqrt(variance, _state);
    }
    
    /*
     * Errors
     */
    *means = *stddev/ae_sqrt((double)(n), _state);
    *stddevs = *stddev*ae_sqrt((double)(2), _state)/ae_sqrt((double)(n-1), _state);
}


/*************************************************************************
Unsets LR
*************************************************************************/
static void testlinregunit_unsetlr(linearmodel* lr, ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix xy;
    ae_int_t info;
    lrreport rep;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _lrreport_init(&rep, _state);

    ae_matrix_set_length(&xy, 5+1, 1+1, _state);
    for(i=0; i<=5; i++)
    {
        xy.ptr.pp_double[i][0] = (double)(0);
        xy.ptr.pp_double[i][1] = (double)(0);
    }
    lrbuild(&xy, 6, 1, &info, lr, &rep, _state);
    ae_assert(info>0, "Assertion failed", _state);
    ae_frame_leave(_state);
}


/*$ End $*/
