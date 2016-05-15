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
#include "testdforestunit.h"


/*$ Declarations $*/
static void testdforestunit_testprocessing(ae_bool* err, ae_state *_state);
static void testdforestunit_basictest1(ae_int_t nvars,
     ae_int_t nclasses,
     ae_int_t passcount,
     ae_bool* err,
     ae_state *_state);
static void testdforestunit_basictest2(ae_bool* err, ae_state *_state);
static void testdforestunit_basictest3(ae_bool* err, ae_state *_state);
static void testdforestunit_basictest4(ae_bool* err, ae_state *_state);
static void testdforestunit_basictest5(ae_bool* err, ae_state *_state);
static void testdforestunit_unsetdf(decisionforest* df, ae_state *_state);


/*$ Body $*/


ae_bool testdforest(ae_bool silent, ae_state *_state)
{
    ae_int_t ncmax;
    ae_int_t nvmax;
    ae_int_t passcount;
    ae_int_t nvars;
    ae_int_t nclasses;
    ae_bool waserrors;
    ae_bool basicerrors;
    ae_bool procerrors;
    ae_bool result;


    
    /*
     * Primary settings
     */
    nvmax = 4;
    ncmax = 3;
    passcount = 10;
    basicerrors = ae_false;
    procerrors = ae_false;
    waserrors = ae_false;
    
    /*
     * Tests
     */
    testdforestunit_testprocessing(&procerrors, _state);
    for(nvars=1; nvars<=nvmax; nvars++)
    {
        for(nclasses=1; nclasses<=ncmax; nclasses++)
        {
            testdforestunit_basictest1(nvars, nclasses, passcount, &basicerrors, _state);
        }
    }
    testdforestunit_basictest2(&basicerrors, _state);
    testdforestunit_basictest3(&basicerrors, _state);
    testdforestunit_basictest4(&basicerrors, _state);
    testdforestunit_basictest5(&basicerrors, _state);
    
    /*
     * Final report
     */
    waserrors = basicerrors||procerrors;
    if( !silent )
    {
        printf("RANDOM FOREST TEST\n");
        printf("TOTAL RESULTS:                           ");
        if( !waserrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* PROCESSING FUNCTIONS:                  ");
        if( !procerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* BASIC TESTS:                           ");
        if( !basicerrors )
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
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testdforest(ae_bool silent, ae_state *_state)
{
    return testdforest(silent, _state);
}


/*************************************************************************
Processing functions test
*************************************************************************/
static void testdforestunit_testprocessing(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t nvars;
    ae_int_t nclasses;
    ae_int_t nsample;
    ae_int_t ntrees;
    ae_int_t nfeatures;
    ae_int_t flags;
    decisionforest df1;
    decisionforest df2;
    ae_int_t npoints;
    ae_matrix xy;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t i;
    ae_int_t j;
    ae_bool allsame;
    ae_int_t info;
    dfreport rep;
    ae_vector x1;
    ae_vector x2;
    ae_vector y1;
    ae_vector y2;
    double v;

    ae_frame_make(_state, &_frame_block);
    _decisionforest_init(&df1, _state);
    _decisionforest_init(&df2, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _dfreport_init(&rep, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);
    ae_vector_init(&y1, 0, DT_REAL, _state);
    ae_vector_init(&y2, 0, DT_REAL, _state);

    passcount = 100;
    
    /*
     * Main cycle
     */
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * initialize parameters
         */
        nvars = 1+ae_randominteger(5, _state);
        nclasses = 1+ae_randominteger(3, _state);
        ntrees = 1+ae_randominteger(4, _state);
        nfeatures = 1+ae_randominteger(nvars, _state);
        flags = 0;
        if( ae_fp_greater(ae_randomreal(_state),0.5) )
        {
            flags = flags+2;
        }
        
        /*
         * Initialize arrays and data
         */
        npoints = 10+ae_randominteger(50, _state);
        nsample = ae_maxint(10, ae_randominteger(npoints, _state), _state);
        ae_vector_set_length(&x1, nvars-1+1, _state);
        ae_vector_set_length(&x2, nvars-1+1, _state);
        ae_vector_set_length(&y1, nclasses-1+1, _state);
        ae_vector_set_length(&y2, nclasses-1+1, _state);
        ae_matrix_set_length(&xy, npoints-1+1, nvars+1, _state);
        for(i=0; i<=npoints-1; i++)
        {
            for(j=0; j<=nvars-1; j++)
            {
                if( j%2==0 )
                {
                    xy.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                }
                else
                {
                    xy.ptr.pp_double[i][j] = (double)(ae_randominteger(2, _state));
                }
            }
            if( nclasses==1 )
            {
                xy.ptr.pp_double[i][nvars] = 2*ae_randomreal(_state)-1;
            }
            else
            {
                xy.ptr.pp_double[i][nvars] = (double)(ae_randominteger(nclasses, _state));
            }
        }
        
        /*
         * create forest
         */
        dfbuildinternal(&xy, npoints, nvars, nclasses, ntrees, nsample, nfeatures, flags, &info, &df1, &rep, _state);
        if( info<=0 )
        {
            *err = ae_true;
            ae_frame_leave(_state);
            return;
        }
        
        /*
         * Same inputs leads to same outputs
         */
        for(i=0; i<=nvars-1; i++)
        {
            x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            x2.ptr.p_double[i] = x1.ptr.p_double[i];
        }
        for(i=0; i<=nclasses-1; i++)
        {
            y1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            y2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        dfprocess(&df1, &x1, &y1, _state);
        dfprocess(&df1, &x2, &y2, _state);
        allsame = ae_true;
        for(i=0; i<=nclasses-1; i++)
        {
            allsame = allsame&&ae_fp_eq(y1.ptr.p_double[i],y2.ptr.p_double[i]);
        }
        *err = *err||!allsame;
        
        /*
         * Same inputs on original forest leads to same outputs
         * on copy created using DFCopy
         */
        testdforestunit_unsetdf(&df2, _state);
        dfcopy(&df1, &df2, _state);
        for(i=0; i<=nvars-1; i++)
        {
            x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            x2.ptr.p_double[i] = x1.ptr.p_double[i];
        }
        for(i=0; i<=nclasses-1; i++)
        {
            y1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            y2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        dfprocess(&df1, &x1, &y1, _state);
        dfprocess(&df2, &x2, &y2, _state);
        allsame = ae_true;
        for(i=0; i<=nclasses-1; i++)
        {
            allsame = allsame&&ae_fp_eq(y1.ptr.p_double[i],y2.ptr.p_double[i]);
        }
        *err = *err||!allsame;
        
        /*
         * Same inputs on original forest leads to same outputs
         * on copy created using DFSerialize
         */
        testdforestunit_unsetdf(&df2, _state);
        {
            /*
             * This code passes data structure through serializers
             * (serializes it to string and loads back)
             */
            ae_serializer _local_serializer;
            ae_int_t _local_ssize;
            ae_frame _local_frame_block;
            ae_dyn_block _local_dynamic_block;
            
            ae_frame_make(_state, &_local_frame_block);
            
            ae_serializer_init(&_local_serializer);
            ae_serializer_alloc_start(&_local_serializer);
            dfalloc(&_local_serializer, &df1, _state);
            _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
            ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
            ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
            dfserialize(&_local_serializer, &df1, _state);
            ae_serializer_stop(&_local_serializer);
            ae_serializer_clear(&_local_serializer);
            
            ae_serializer_init(&_local_serializer);
            ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
            dfunserialize(&_local_serializer, &df2, _state);
            ae_serializer_stop(&_local_serializer);
            ae_serializer_clear(&_local_serializer);
            
            ae_frame_leave(_state);
        }
        for(i=0; i<=nvars-1; i++)
        {
            x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            x2.ptr.p_double[i] = x1.ptr.p_double[i];
        }
        for(i=0; i<=nclasses-1; i++)
        {
            y1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            y2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        dfprocess(&df1, &x1, &y1, _state);
        dfprocess(&df2, &x2, &y2, _state);
        allsame = ae_true;
        for(i=0; i<=nclasses-1; i++)
        {
            allsame = allsame&&ae_fp_eq(y1.ptr.p_double[i],y2.ptr.p_double[i]);
        }
        *err = *err||!allsame;
        
        /*
         * Normalization properties
         */
        if( nclasses>1 )
        {
            for(i=0; i<=nvars-1; i++)
            {
                x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            dfprocess(&df1, &x1, &y1, _state);
            v = (double)(0);
            for(i=0; i<=nclasses-1; i++)
            {
                v = v+y1.ptr.p_double[i];
                *err = *err||ae_fp_less(y1.ptr.p_double[i],(double)(0));
            }
            *err = *err||ae_fp_greater(ae_fabs(v-1, _state),1000*ae_machineepsilon);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Basic test:  one-tree forest built using full sample must remember all the
training cases
*************************************************************************/
static void testdforestunit_basictest1(ae_int_t nvars,
     ae_int_t nclasses,
     ae_int_t passcount,
     ae_bool* err,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t pass;
    ae_matrix xy;
    ae_int_t npoints;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double s;
    ae_int_t info;
    decisionforest df;
    ae_vector x;
    ae_vector y;
    dfreport rep;
    ae_bool hassame;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _decisionforest_init(&df, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    _dfreport_init(&rep, _state);

    if( nclasses==1 )
    {
        
        /*
         * only classification tasks
         */
        ae_frame_leave(_state);
        return;
    }
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * select number of points
         */
        if( pass<=3&&passcount>3 )
        {
            npoints = pass;
        }
        else
        {
            npoints = 100+ae_randominteger(100, _state);
        }
        
        /*
         * Prepare task
         */
        ae_matrix_set_length(&xy, npoints-1+1, nvars+1, _state);
        ae_vector_set_length(&x, nvars-1+1, _state);
        ae_vector_set_length(&y, nclasses-1+1, _state);
        for(i=0; i<=npoints-1; i++)
        {
            for(j=0; j<=nvars-1; j++)
            {
                xy.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
            xy.ptr.pp_double[i][nvars] = (double)(ae_randominteger(nclasses, _state));
        }
        
        /*
         * Test
         */
        dfbuildinternal(&xy, npoints, nvars, nclasses, 1, npoints, 1, 1, &info, &df, &rep, _state);
        if( info<=0 )
        {
            *err = ae_true;
            ae_frame_leave(_state);
            return;
        }
        for(i=0; i<=npoints-1; i++)
        {
            ae_v_move(&x.ptr.p_double[0], 1, &xy.ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
            dfprocess(&df, &x, &y, _state);
            s = (double)(0);
            for(j=0; j<=nclasses-1; j++)
            {
                if( ae_fp_less(y.ptr.p_double[j],(double)(0)) )
                {
                    *err = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
                s = s+y.ptr.p_double[j];
            }
            if( ae_fp_greater(ae_fabs(s-1, _state),1000*ae_machineepsilon) )
            {
                *err = ae_true;
                ae_frame_leave(_state);
                return;
            }
            if( ae_fp_greater(ae_fabs(y.ptr.p_double[ae_round(xy.ptr.pp_double[i][nvars], _state)]-1, _state),1000*ae_machineepsilon) )
            {
                
                /*
                 * not an error if there exists such K,J that XY[K,J]=XY[I,J]
                 * (may be we just can't distinguish two tied values).
                 *
                 * definitely error otherwise.
                 */
                hassame = ae_false;
                for(k=0; k<=npoints-1; k++)
                {
                    if( k!=i )
                    {
                        for(j=0; j<=nvars-1; j++)
                        {
                            if( ae_fp_eq(xy.ptr.pp_double[k][j],xy.ptr.pp_double[i][j]) )
                            {
                                hassame = ae_true;
                            }
                        }
                    }
                }
                if( !hassame )
                {
                    *err = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Basic test:  tests generalization ability on a simple noisy classification
task:
* 0<x<1 - P(class=0)=1
* 1<x<2 - P(class=0)=2-x
* 2<x<3 - P(class=0)=0
*************************************************************************/
static void testdforestunit_basictest2(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t pass;
    ae_int_t passcount;
    ae_matrix xy;
    ae_int_t npoints;
    ae_int_t ntrees;
    ae_int_t i;
    ae_int_t j;
    double s;
    ae_int_t info;
    decisionforest df;
    ae_vector x;
    ae_vector y;
    dfreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _decisionforest_init(&df, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    _dfreport_init(&rep, _state);

    passcount = 1;
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * select npoints and ntrees
         */
        npoints = 3000;
        ntrees = 50;
        
        /*
         * Prepare task
         */
        ae_matrix_set_length(&xy, npoints-1+1, 1+1, _state);
        ae_vector_set_length(&x, 0+1, _state);
        ae_vector_set_length(&y, 1+1, _state);
        for(i=0; i<=npoints-1; i++)
        {
            xy.ptr.pp_double[i][0] = 3*ae_randomreal(_state);
            if( ae_fp_less_eq(xy.ptr.pp_double[i][0],(double)(1)) )
            {
                xy.ptr.pp_double[i][1] = (double)(0);
            }
            else
            {
                if( ae_fp_less_eq(xy.ptr.pp_double[i][0],(double)(2)) )
                {
                    if( ae_fp_less(ae_randomreal(_state),xy.ptr.pp_double[i][0]-1) )
                    {
                        xy.ptr.pp_double[i][1] = (double)(1);
                    }
                    else
                    {
                        xy.ptr.pp_double[i][1] = (double)(0);
                    }
                }
                else
                {
                    xy.ptr.pp_double[i][1] = (double)(1);
                }
            }
        }
        
        /*
         * Test
         */
        dfbuildinternal(&xy, npoints, 1, 2, ntrees, ae_round(0.05*npoints, _state), 1, 0, &info, &df, &rep, _state);
        if( info<=0 )
        {
            *err = ae_true;
            ae_frame_leave(_state);
            return;
        }
        x.ptr.p_double[0] = 0.0;
        while(ae_fp_less_eq(x.ptr.p_double[0],3.0))
        {
            dfprocess(&df, &x, &y, _state);
            
            /*
             * Test for basic properties
             */
            s = (double)(0);
            for(j=0; j<=1; j++)
            {
                if( ae_fp_less(y.ptr.p_double[j],(double)(0)) )
                {
                    *err = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
                s = s+y.ptr.p_double[j];
            }
            if( ae_fp_greater(ae_fabs(s-1, _state),1000*ae_machineepsilon) )
            {
                *err = ae_true;
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * test for good correlation with results
             */
            if( ae_fp_less(x.ptr.p_double[0],(double)(1)) )
            {
                *err = *err||ae_fp_less(y.ptr.p_double[0],0.8);
            }
            if( ae_fp_greater_eq(x.ptr.p_double[0],(double)(1))&&ae_fp_less_eq(x.ptr.p_double[0],(double)(2)) )
            {
                *err = *err||ae_fp_greater(ae_fabs(y.ptr.p_double[1]-(x.ptr.p_double[0]-1), _state),0.5);
            }
            if( ae_fp_greater(x.ptr.p_double[0],(double)(2)) )
            {
                *err = *err||ae_fp_less(y.ptr.p_double[1],0.8);
            }
            x.ptr.p_double[0] = x.ptr.p_double[0]+0.01;
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Basic test:  tests  generalization ability on a simple classification task
(no noise):
* |x|<1, |y|<1
* x^2+y^2<=0.25 - P(class=0)=1
* x^2+y^2>0.25  - P(class=0)=0
*************************************************************************/
static void testdforestunit_basictest3(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t pass;
    ae_int_t passcount;
    ae_matrix xy;
    ae_int_t npoints;
    ae_int_t ntrees;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double s;
    ae_int_t info;
    decisionforest df;
    ae_vector x;
    ae_vector y;
    dfreport rep;
    ae_int_t testgridsize;
    double r;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _decisionforest_init(&df, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    _dfreport_init(&rep, _state);

    passcount = 1;
    testgridsize = 50;
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * select npoints and ntrees
         */
        npoints = 2000;
        ntrees = 100;
        
        /*
         * Prepare task
         */
        ae_matrix_set_length(&xy, npoints-1+1, 2+1, _state);
        ae_vector_set_length(&x, 1+1, _state);
        ae_vector_set_length(&y, 1+1, _state);
        for(i=0; i<=npoints-1; i++)
        {
            xy.ptr.pp_double[i][0] = 2*ae_randomreal(_state)-1;
            xy.ptr.pp_double[i][1] = 2*ae_randomreal(_state)-1;
            if( ae_fp_less_eq(ae_sqr(xy.ptr.pp_double[i][0], _state)+ae_sqr(xy.ptr.pp_double[i][1], _state),0.25) )
            {
                xy.ptr.pp_double[i][2] = (double)(0);
            }
            else
            {
                xy.ptr.pp_double[i][2] = (double)(1);
            }
        }
        
        /*
         * Test
         */
        dfbuildinternal(&xy, npoints, 2, 2, ntrees, ae_round(0.1*npoints, _state), 1, 0, &info, &df, &rep, _state);
        if( info<=0 )
        {
            *err = ae_true;
            ae_frame_leave(_state);
            return;
        }
        for(i=-testgridsize/2; i<=testgridsize/2; i++)
        {
            for(j=-testgridsize/2; j<=testgridsize/2; j++)
            {
                x.ptr.p_double[0] = (double)i/(double)(testgridsize/2);
                x.ptr.p_double[1] = (double)j/(double)(testgridsize/2);
                dfprocess(&df, &x, &y, _state);
                
                /*
                 * Test for basic properties
                 */
                s = (double)(0);
                for(k=0; k<=1; k++)
                {
                    if( ae_fp_less(y.ptr.p_double[k],(double)(0)) )
                    {
                        *err = ae_true;
                        ae_frame_leave(_state);
                        return;
                    }
                    s = s+y.ptr.p_double[k];
                }
                if( ae_fp_greater(ae_fabs(s-1, _state),1000*ae_machineepsilon) )
                {
                    *err = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
                
                /*
                 * test for good correlation with results
                 */
                r = ae_sqrt(ae_sqr(x.ptr.p_double[0], _state)+ae_sqr(x.ptr.p_double[1], _state), _state);
                if( ae_fp_less(r,0.5*0.5) )
                {
                    *err = *err||ae_fp_less(y.ptr.p_double[0],0.6);
                }
                if( ae_fp_greater(r,0.5*1.5) )
                {
                    *err = *err||ae_fp_less(y.ptr.p_double[1],0.6);
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Basic test: simple regression task without noise:
* |x|<1, |y|<1
* F(x,y) = x^2+y
*************************************************************************/
static void testdforestunit_basictest4(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t pass;
    ae_int_t passcount;
    ae_matrix xy;
    ae_int_t npoints;
    ae_int_t ntrees;
    ae_int_t ns;
    ae_int_t strongc;
    ae_int_t i;
    ae_int_t j;
    ae_int_t info;
    decisionforest df;
    decisionforest df2;
    ae_vector x;
    ae_vector y;
    dfreport rep;
    dfreport rep2;
    ae_int_t testgridsize;
    double maxerr;
    double maxerr2;
    double avgerr;
    double avgerr2;
    ae_int_t cnt;
    double ey;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _decisionforest_init(&df, _state);
    _decisionforest_init(&df2, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    _dfreport_init(&rep, _state);
    _dfreport_init(&rep2, _state);

    passcount = 1;
    testgridsize = 50;
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * select npoints and ntrees
         */
        npoints = 5000;
        ntrees = 100;
        ns = ae_round(0.1*npoints, _state);
        strongc = 1;
        
        /*
         * Prepare task
         */
        ae_matrix_set_length(&xy, npoints-1+1, 2+1, _state);
        ae_vector_set_length(&x, 1+1, _state);
        ae_vector_set_length(&y, 0+1, _state);
        for(i=0; i<=npoints-1; i++)
        {
            xy.ptr.pp_double[i][0] = 2*ae_randomreal(_state)-1;
            xy.ptr.pp_double[i][1] = 2*ae_randomreal(_state)-1;
            xy.ptr.pp_double[i][2] = ae_sqr(xy.ptr.pp_double[i][0], _state)+xy.ptr.pp_double[i][1];
        }
        
        /*
         * Test
         */
        dfbuildinternal(&xy, npoints, 2, 1, ntrees, ns, 1, 0, &info, &df, &rep, _state);
        if( info<=0 )
        {
            *err = ae_true;
            ae_frame_leave(_state);
            return;
        }
        dfbuildinternal(&xy, npoints, 2, 1, ntrees, ns, 1, strongc, &info, &df2, &rep2, _state);
        if( info<=0 )
        {
            *err = ae_true;
            ae_frame_leave(_state);
            return;
        }
        maxerr = (double)(0);
        maxerr2 = (double)(0);
        avgerr = (double)(0);
        avgerr2 = (double)(0);
        cnt = 0;
        for(i=ae_round(-0.7*testgridsize/2, _state); i<=ae_round(0.7*testgridsize/2, _state); i++)
        {
            for(j=ae_round(-0.7*testgridsize/2, _state); j<=ae_round(0.7*testgridsize/2, _state); j++)
            {
                x.ptr.p_double[0] = (double)i/(double)(testgridsize/2);
                x.ptr.p_double[1] = (double)j/(double)(testgridsize/2);
                ey = ae_sqr(x.ptr.p_double[0], _state)+x.ptr.p_double[1];
                dfprocess(&df, &x, &y, _state);
                maxerr = ae_maxreal(maxerr, ae_fabs(y.ptr.p_double[0]-ey, _state), _state);
                avgerr = avgerr+ae_fabs(y.ptr.p_double[0]-ey, _state);
                dfprocess(&df2, &x, &y, _state);
                maxerr2 = ae_maxreal(maxerr2, ae_fabs(y.ptr.p_double[0]-ey, _state), _state);
                avgerr2 = avgerr2+ae_fabs(y.ptr.p_double[0]-ey, _state);
                cnt = cnt+1;
            }
        }
        avgerr = avgerr/cnt;
        avgerr2 = avgerr2/cnt;
        *err = *err||ae_fp_greater(maxerr,0.2);
        *err = *err||ae_fp_greater(maxerr2,0.2);
        *err = *err||ae_fp_greater(avgerr,0.1);
        *err = *err||ae_fp_greater(avgerr2,0.1);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Basic test: extended variable selection leads to better results.

Next task CAN be solved without EVS but it is very unlikely. With EVS
it can be easily and exactly solved.

Task matrix:
    1 0 0 0 ... 0   0
    0 1 0 0 ... 0   1
    0 0 1 0 ... 0   2
    0 0 0 1 ... 0   3
    0 0 0 0 ... 1   N-1
*************************************************************************/
static void testdforestunit_basictest5(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix xy;
    ae_int_t nvars;
    ae_int_t npoints;
    ae_int_t nfeatures;
    ae_int_t nsample;
    ae_int_t ntrees;
    ae_int_t evs;
    ae_int_t i;
    ae_int_t j;
    ae_bool eflag;
    ae_int_t info;
    decisionforest df;
    ae_vector x;
    ae_vector y;
    dfreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _decisionforest_init(&df, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    _dfreport_init(&rep, _state);

    
    /*
     * select npoints and ntrees
     */
    npoints = 50;
    nvars = npoints;
    ntrees = 1;
    nsample = npoints;
    evs = 2;
    nfeatures = 1;
    
    /*
     * Prepare task
     */
    ae_matrix_set_length(&xy, npoints-1+1, nvars+1, _state);
    ae_vector_set_length(&x, nvars-1+1, _state);
    ae_vector_set_length(&y, 0+1, _state);
    for(i=0; i<=npoints-1; i++)
    {
        for(j=0; j<=nvars-1; j++)
        {
            xy.ptr.pp_double[i][j] = (double)(0);
        }
        xy.ptr.pp_double[i][i] = (double)(1);
        xy.ptr.pp_double[i][nvars] = (double)(i);
    }
    
    /*
     * Without EVS
     */
    dfbuildinternal(&xy, npoints, nvars, 1, ntrees, nsample, nfeatures, 0, &info, &df, &rep, _state);
    if( info<=0 )
    {
        *err = ae_true;
        ae_frame_leave(_state);
        return;
    }
    eflag = ae_false;
    for(i=0; i<=npoints-1; i++)
    {
        ae_v_move(&x.ptr.p_double[0], 1, &xy.ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
        dfprocess(&df, &x, &y, _state);
        if( ae_fp_greater(ae_fabs(y.ptr.p_double[0]-xy.ptr.pp_double[i][nvars], _state),1000*ae_machineepsilon) )
        {
            eflag = ae_true;
        }
    }
    if( !eflag )
    {
        *err = ae_true;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * With EVS
     */
    dfbuildinternal(&xy, npoints, nvars, 1, ntrees, nsample, nfeatures, evs, &info, &df, &rep, _state);
    if( info<=0 )
    {
        *err = ae_true;
        ae_frame_leave(_state);
        return;
    }
    eflag = ae_false;
    for(i=0; i<=npoints-1; i++)
    {
        ae_v_move(&x.ptr.p_double[0], 1, &xy.ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
        dfprocess(&df, &x, &y, _state);
        if( ae_fp_greater(ae_fabs(y.ptr.p_double[0]-xy.ptr.pp_double[i][nvars], _state),1000*ae_machineepsilon) )
        {
            eflag = ae_true;
        }
    }
    if( eflag )
    {
        *err = ae_true;
        ae_frame_leave(_state);
        return;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Unsets DF
*************************************************************************/
static void testdforestunit_unsetdf(decisionforest* df, ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix xy;
    ae_int_t info;
    dfreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _dfreport_init(&rep, _state);

    ae_matrix_set_length(&xy, 0+1, 1+1, _state);
    xy.ptr.pp_double[0][0] = (double)(0);
    xy.ptr.pp_double[0][1] = (double)(0);
    dfbuildinternal(&xy, 1, 1, 1, 1, 1, 1, 0, &info, df, &rep, _state);
    ae_frame_leave(_state);
}


/*$ End $*/
