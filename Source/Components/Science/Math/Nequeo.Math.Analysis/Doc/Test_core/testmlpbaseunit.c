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
#include "testmlpbaseunit.h"


/*$ Declarations $*/
static double testmlpbaseunit_vectordiff(/* Real    */ ae_vector* g0,
     /* Real    */ ae_vector* g1,
     ae_int_t n,
     double s,
     ae_state *_state);
static void testmlpbaseunit_createnetwork(multilayerperceptron* network,
     ae_int_t nkind,
     double a1,
     double a2,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_state *_state);
static void testmlpbaseunit_unsetnetwork(multilayerperceptron* network,
     ae_state *_state);
static void testmlpbaseunit_testinformational(ae_int_t nkind,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t passcount,
     ae_bool* err,
     ae_state *_state);
static void testmlpbaseunit_testprocessing(ae_int_t nkind,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t passcount,
     ae_bool* err,
     ae_state *_state);
static void testmlpbaseunit_testgradient(ae_int_t nkind,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t passcount,
     ae_int_t sizemin,
     ae_int_t sizemax,
     ae_bool* err,
     ae_state *_state);
static void testmlpbaseunit_testhessian(ae_int_t nkind,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t passcount,
     ae_bool* err,
     ae_state *_state);
static void testmlpbaseunit_testerr(ae_int_t nkind,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t passcount,
     ae_int_t sizemin,
     ae_int_t sizemax,
     ae_bool* err,
     ae_state *_state);
static void testmlpbaseunit_spectests(ae_bool* inferrors,
     ae_bool* procerrors,
     ae_bool* graderrors,
     ae_bool* hesserrors,
     ae_bool* errerrors,
     ae_state *_state);
static ae_bool testmlpbaseunit_testmlpgbsubset(ae_state *_state);


/*$ Body $*/


ae_bool testmlpbase(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool waserrors;
    ae_int_t passcount;
    ae_int_t maxn;
    ae_int_t maxhid;
    ae_int_t sizemin;
    ae_int_t sizemax;
    ae_int_t nf;
    ae_int_t nl;
    ae_int_t nhid1;
    ae_int_t nhid2;
    ae_int_t nkind;
    multilayerperceptron network;
    multilayerperceptron network2;
    ae_matrix xy;
    ae_matrix valxy;
    ae_bool inferrors;
    ae_bool procerrors;
    ae_bool graderrors;
    ae_bool hesserrors;
    ae_bool errerrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&network, _state);
    _multilayerperceptron_init(&network2, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&valxy, 0, 0, DT_REAL, _state);

    waserrors = ae_false;
    inferrors = ae_false;
    procerrors = ae_false;
    graderrors = ae_false;
    hesserrors = ae_false;
    errerrors = ae_false;
    passcount = 5;
    maxn = 3;
    maxhid = 3;
    
    /*
     * Special tests
     */
    testmlpbaseunit_spectests(&inferrors, &procerrors, &graderrors, &hesserrors, &errerrors, _state);
    
    /*
     * General multilayer network tests.
     * These tests are performed with small dataset, whose size is in [0,10].
     * We test correctness of functions on small sets, but do not test code
     * which splits large dataset into smaller chunks.
     */
    sizemin = 0;
    sizemax = 10;
    for(nf=1; nf<=maxn; nf++)
    {
        for(nl=1; nl<=maxn; nl++)
        {
            for(nhid1=0; nhid1<=maxhid; nhid1++)
            {
                for(nhid2=0; nhid2<=maxhid; nhid2++)
                {
                    for(nkind=0; nkind<=3; nkind++)
                    {
                        
                        /*
                         *  Skip meaningless parameters combinations
                         */
                        if( nkind==1&&nl<2 )
                        {
                            continue;
                        }
                        if( nhid1==0&&nhid2!=0 )
                        {
                            continue;
                        }
                        
                        /*
                         * Tests
                         */
                        testmlpbaseunit_testinformational(nkind, nf, nhid1, nhid2, nl, passcount, &inferrors, _state);
                        testmlpbaseunit_testprocessing(nkind, nf, nhid1, nhid2, nl, passcount, &procerrors, _state);
                        testmlpbaseunit_testgradient(nkind, nf, nhid1, nhid2, nl, passcount, sizemin, sizemax, &graderrors, _state);
                        testmlpbaseunit_testhessian(nkind, nf, nhid1, nhid2, nl, passcount, &hesserrors, _state);
                        testmlpbaseunit_testerr(nkind, nf, nhid1, nhid2, nl, passcount, sizemin, sizemax, &errerrors, _state);
                    }
                }
            }
        }
    }
    
    /*
     * Special tests on large datasets: test ability to correctly split
     * work into smaller chunks.
     */
    nf = 2;
    nhid1 = 30;
    nhid2 = 30;
    nl = 2;
    sizemin = 1000;
    sizemax = 1000;
    testmlpbaseunit_testerr(0, nf, nhid1, nhid2, nl, 1, sizemin, sizemax, &errerrors, _state);
    testmlpbaseunit_testgradient(0, nf, nhid1, nhid2, nl, 1, sizemin, sizemax, &graderrors, _state);
    
    /*
     * Test for MLPGradBatch____Subset()
     */
    graderrors = graderrors||testmlpbaseunit_testmlpgbsubset(_state);
    
    /*
     * Final report
     */
    waserrors = (((inferrors||procerrors)||graderrors)||hesserrors)||errerrors;
    if( !silent )
    {
        printf("MLP TEST\n");
        printf("INFORMATIONAL FUNCTIONS:                 ");
        if( !inferrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("BASIC PROCESSING:                        ");
        if( !procerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("GRADIENT CALCULATION:                    ");
        if( !graderrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("HESSIAN CALCULATION:                     ");
        if( !hesserrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("ERROR FUNCTIONS:                         ");
        if( !errerrors )
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
ae_bool _pexec_testmlpbase(ae_bool silent, ae_state *_state)
{
    return testmlpbase(silent, _state);
}


/*************************************************************************
This function compares vectors G0 and G1 and returns
    ||G0-G1||/max(||G0||,||G1||,S)

For zero G0, G1 and S (all three quantities are zero) it returns zero.
*************************************************************************/
static double testmlpbaseunit_vectordiff(/* Real    */ ae_vector* g0,
     /* Real    */ ae_vector* g1,
     ae_int_t n,
     double s,
     ae_state *_state)
{
    ae_int_t i;
    double norm0;
    double norm1;
    double diff;
    double result;


    norm0 = (double)(0);
    norm1 = (double)(0);
    diff = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        norm0 = norm0+ae_sqr(g0->ptr.p_double[i], _state);
        norm1 = norm1+ae_sqr(g1->ptr.p_double[i], _state);
        diff = diff+ae_sqr(g0->ptr.p_double[i]-g1->ptr.p_double[i], _state);
    }
    norm0 = ae_sqrt(norm0, _state);
    norm1 = ae_sqrt(norm1, _state);
    diff = ae_sqrt(diff, _state);
    if( (ae_fp_neq(norm0,(double)(0))||ae_fp_neq(norm1,(double)(0)))||ae_fp_neq(s,(double)(0)) )
    {
        diff = diff/ae_maxreal(ae_maxreal(norm0, norm1, _state), s, _state);
    }
    else
    {
        diff = (double)(0);
    }
    result = diff;
    return result;
}


/*************************************************************************
Network creation

This function creates network with desired structure.

Network is created using one of the three methods:
a) straightforward creation using MLPCreate???()
b) MLPCreate???() for proxy object, which is copied with PassThroughSerializer()
c) MLPCreate???() for proxy object, which is copied with MLPCopy()

One of these methods is chosen at random.
*************************************************************************/
static void testmlpbaseunit_createnetwork(multilayerperceptron* network,
     ae_int_t nkind,
     double a1,
     double a2,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t mkind;
    multilayerperceptron tmp;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&tmp, _state);

    ae_assert(((nin>0&&nhid1>=0)&&nhid2>=0)&&nout>0, "CreateNetwork error", _state);
    ae_assert(nhid1!=0||nhid2==0, "CreateNetwork error", _state);
    ae_assert(nkind!=1||nout>=2, "CreateNetwork error", _state);
    mkind = ae_randominteger(3, _state);
    if( nhid1==0 )
    {
        
        /*
         * No hidden layers
         */
        if( nkind==0 )
        {
            if( mkind==0 )
            {
                mlpcreate0(nin, nout, network, _state);
            }
            if( mkind==1 )
            {
                mlpcreate0(nin, nout, &tmp, _state);
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
                    mlpalloc(&_local_serializer, &tmp, _state);
                    _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                    ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                    ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                    mlpserialize(&_local_serializer, &tmp, _state);
                    ae_serializer_stop(&_local_serializer);
                    ae_serializer_clear(&_local_serializer);
                    
                    ae_serializer_init(&_local_serializer);
                    ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                    mlpunserialize(&_local_serializer, network, _state);
                    ae_serializer_stop(&_local_serializer);
                    ae_serializer_clear(&_local_serializer);
                    
                    ae_frame_leave(_state);
                }
            }
            if( mkind==2 )
            {
                mlpcreate0(nin, nout, &tmp, _state);
                mlpcopy(&tmp, network, _state);
            }
        }
        else
        {
            if( nkind==1 )
            {
                if( mkind==0 )
                {
                    mlpcreatec0(nin, nout, network, _state);
                }
                if( mkind==1 )
                {
                    mlpcreatec0(nin, nout, &tmp, _state);
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
                        mlpalloc(&_local_serializer, &tmp, _state);
                        _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                        ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                        ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                        mlpserialize(&_local_serializer, &tmp, _state);
                        ae_serializer_stop(&_local_serializer);
                        ae_serializer_clear(&_local_serializer);
                        
                        ae_serializer_init(&_local_serializer);
                        ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                        mlpunserialize(&_local_serializer, network, _state);
                        ae_serializer_stop(&_local_serializer);
                        ae_serializer_clear(&_local_serializer);
                        
                        ae_frame_leave(_state);
                    }
                }
                if( mkind==2 )
                {
                    mlpcreatec0(nin, nout, &tmp, _state);
                    mlpcopy(&tmp, network, _state);
                }
            }
            else
            {
                if( nkind==2 )
                {
                    if( mkind==0 )
                    {
                        mlpcreateb0(nin, nout, a1, a2, network, _state);
                    }
                    if( mkind==1 )
                    {
                        mlpcreateb0(nin, nout, a1, a2, &tmp, _state);
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
                            mlpalloc(&_local_serializer, &tmp, _state);
                            _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                            ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                            ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                            mlpserialize(&_local_serializer, &tmp, _state);
                            ae_serializer_stop(&_local_serializer);
                            ae_serializer_clear(&_local_serializer);
                            
                            ae_serializer_init(&_local_serializer);
                            ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                            mlpunserialize(&_local_serializer, network, _state);
                            ae_serializer_stop(&_local_serializer);
                            ae_serializer_clear(&_local_serializer);
                            
                            ae_frame_leave(_state);
                        }
                    }
                    if( mkind==2 )
                    {
                        mlpcreateb0(nin, nout, a1, a2, &tmp, _state);
                        mlpcopy(&tmp, network, _state);
                    }
                }
                else
                {
                    if( nkind==3 )
                    {
                        if( mkind==0 )
                        {
                            mlpcreater0(nin, nout, a1, a2, network, _state);
                        }
                        if( mkind==1 )
                        {
                            mlpcreater0(nin, nout, a1, a2, &tmp, _state);
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
                                mlpalloc(&_local_serializer, &tmp, _state);
                                _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                                ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                                ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                                mlpserialize(&_local_serializer, &tmp, _state);
                                ae_serializer_stop(&_local_serializer);
                                ae_serializer_clear(&_local_serializer);
                                
                                ae_serializer_init(&_local_serializer);
                                ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                                mlpunserialize(&_local_serializer, network, _state);
                                ae_serializer_stop(&_local_serializer);
                                ae_serializer_clear(&_local_serializer);
                                
                                ae_frame_leave(_state);
                            }
                        }
                        if( mkind==2 )
                        {
                            mlpcreater0(nin, nout, a1, a2, &tmp, _state);
                            mlpcopy(&tmp, network, _state);
                        }
                    }
                }
            }
        }
        mlprandomizefull(network, _state);
        ae_frame_leave(_state);
        return;
    }
    if( nhid2==0 )
    {
        
        /*
         * One hidden layer
         */
        if( nkind==0 )
        {
            if( mkind==0 )
            {
                mlpcreate1(nin, nhid1, nout, network, _state);
            }
            if( mkind==1 )
            {
                mlpcreate1(nin, nhid1, nout, &tmp, _state);
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
                    mlpalloc(&_local_serializer, &tmp, _state);
                    _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                    ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                    ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                    mlpserialize(&_local_serializer, &tmp, _state);
                    ae_serializer_stop(&_local_serializer);
                    ae_serializer_clear(&_local_serializer);
                    
                    ae_serializer_init(&_local_serializer);
                    ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                    mlpunserialize(&_local_serializer, network, _state);
                    ae_serializer_stop(&_local_serializer);
                    ae_serializer_clear(&_local_serializer);
                    
                    ae_frame_leave(_state);
                }
            }
            if( mkind==2 )
            {
                mlpcreate1(nin, nhid1, nout, &tmp, _state);
                mlpcopy(&tmp, network, _state);
            }
        }
        else
        {
            if( nkind==1 )
            {
                if( mkind==0 )
                {
                    mlpcreatec1(nin, nhid1, nout, network, _state);
                }
                if( mkind==1 )
                {
                    mlpcreatec1(nin, nhid1, nout, &tmp, _state);
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
                        mlpalloc(&_local_serializer, &tmp, _state);
                        _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                        ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                        ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                        mlpserialize(&_local_serializer, &tmp, _state);
                        ae_serializer_stop(&_local_serializer);
                        ae_serializer_clear(&_local_serializer);
                        
                        ae_serializer_init(&_local_serializer);
                        ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                        mlpunserialize(&_local_serializer, network, _state);
                        ae_serializer_stop(&_local_serializer);
                        ae_serializer_clear(&_local_serializer);
                        
                        ae_frame_leave(_state);
                    }
                }
                if( mkind==2 )
                {
                    mlpcreatec1(nin, nhid1, nout, &tmp, _state);
                    mlpcopy(&tmp, network, _state);
                }
            }
            else
            {
                if( nkind==2 )
                {
                    if( mkind==0 )
                    {
                        mlpcreateb1(nin, nhid1, nout, a1, a2, network, _state);
                    }
                    if( mkind==1 )
                    {
                        mlpcreateb1(nin, nhid1, nout, a1, a2, &tmp, _state);
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
                            mlpalloc(&_local_serializer, &tmp, _state);
                            _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                            ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                            ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                            mlpserialize(&_local_serializer, &tmp, _state);
                            ae_serializer_stop(&_local_serializer);
                            ae_serializer_clear(&_local_serializer);
                            
                            ae_serializer_init(&_local_serializer);
                            ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                            mlpunserialize(&_local_serializer, network, _state);
                            ae_serializer_stop(&_local_serializer);
                            ae_serializer_clear(&_local_serializer);
                            
                            ae_frame_leave(_state);
                        }
                    }
                    if( mkind==2 )
                    {
                        mlpcreateb1(nin, nhid1, nout, a1, a2, &tmp, _state);
                        mlpcopy(&tmp, network, _state);
                    }
                }
                else
                {
                    if( nkind==3 )
                    {
                        if( mkind==0 )
                        {
                            mlpcreater1(nin, nhid1, nout, a1, a2, network, _state);
                        }
                        if( mkind==1 )
                        {
                            mlpcreater1(nin, nhid1, nout, a1, a2, &tmp, _state);
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
                                mlpalloc(&_local_serializer, &tmp, _state);
                                _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                                ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                                ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                                mlpserialize(&_local_serializer, &tmp, _state);
                                ae_serializer_stop(&_local_serializer);
                                ae_serializer_clear(&_local_serializer);
                                
                                ae_serializer_init(&_local_serializer);
                                ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                                mlpunserialize(&_local_serializer, network, _state);
                                ae_serializer_stop(&_local_serializer);
                                ae_serializer_clear(&_local_serializer);
                                
                                ae_frame_leave(_state);
                            }
                        }
                        if( mkind==2 )
                        {
                            mlpcreater1(nin, nhid1, nout, a1, a2, &tmp, _state);
                            mlpcopy(&tmp, network, _state);
                        }
                    }
                }
            }
        }
        mlprandomizefull(network, _state);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Two hidden layers
     */
    if( nkind==0 )
    {
        if( mkind==0 )
        {
            mlpcreate2(nin, nhid1, nhid2, nout, network, _state);
        }
        if( mkind==1 )
        {
            mlpcreate2(nin, nhid1, nhid2, nout, &tmp, _state);
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
                mlpalloc(&_local_serializer, &tmp, _state);
                _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                mlpserialize(&_local_serializer, &tmp, _state);
                ae_serializer_stop(&_local_serializer);
                ae_serializer_clear(&_local_serializer);
                
                ae_serializer_init(&_local_serializer);
                ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                mlpunserialize(&_local_serializer, network, _state);
                ae_serializer_stop(&_local_serializer);
                ae_serializer_clear(&_local_serializer);
                
                ae_frame_leave(_state);
            }
        }
        if( mkind==2 )
        {
            mlpcreate2(nin, nhid1, nhid2, nout, &tmp, _state);
            mlpcopy(&tmp, network, _state);
        }
    }
    else
    {
        if( nkind==1 )
        {
            if( mkind==0 )
            {
                mlpcreatec2(nin, nhid1, nhid2, nout, network, _state);
            }
            if( mkind==1 )
            {
                mlpcreatec2(nin, nhid1, nhid2, nout, &tmp, _state);
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
                    mlpalloc(&_local_serializer, &tmp, _state);
                    _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                    ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                    ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                    mlpserialize(&_local_serializer, &tmp, _state);
                    ae_serializer_stop(&_local_serializer);
                    ae_serializer_clear(&_local_serializer);
                    
                    ae_serializer_init(&_local_serializer);
                    ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                    mlpunserialize(&_local_serializer, network, _state);
                    ae_serializer_stop(&_local_serializer);
                    ae_serializer_clear(&_local_serializer);
                    
                    ae_frame_leave(_state);
                }
            }
            if( mkind==2 )
            {
                mlpcreatec2(nin, nhid1, nhid2, nout, &tmp, _state);
                mlpcopy(&tmp, network, _state);
            }
        }
        else
        {
            if( nkind==2 )
            {
                if( mkind==0 )
                {
                    mlpcreateb2(nin, nhid1, nhid2, nout, a1, a2, network, _state);
                }
                if( mkind==1 )
                {
                    mlpcreateb2(nin, nhid1, nhid2, nout, a1, a2, &tmp, _state);
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
                        mlpalloc(&_local_serializer, &tmp, _state);
                        _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                        ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                        ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                        mlpserialize(&_local_serializer, &tmp, _state);
                        ae_serializer_stop(&_local_serializer);
                        ae_serializer_clear(&_local_serializer);
                        
                        ae_serializer_init(&_local_serializer);
                        ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                        mlpunserialize(&_local_serializer, network, _state);
                        ae_serializer_stop(&_local_serializer);
                        ae_serializer_clear(&_local_serializer);
                        
                        ae_frame_leave(_state);
                    }
                }
                if( mkind==2 )
                {
                    mlpcreateb2(nin, nhid1, nhid2, nout, a1, a2, &tmp, _state);
                    mlpcopy(&tmp, network, _state);
                }
            }
            else
            {
                if( nkind==3 )
                {
                    if( mkind==0 )
                    {
                        mlpcreater2(nin, nhid1, nhid2, nout, a1, a2, network, _state);
                    }
                    if( mkind==1 )
                    {
                        mlpcreater2(nin, nhid1, nhid2, nout, a1, a2, &tmp, _state);
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
                            mlpalloc(&_local_serializer, &tmp, _state);
                            _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                            ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                            ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                            mlpserialize(&_local_serializer, &tmp, _state);
                            ae_serializer_stop(&_local_serializer);
                            ae_serializer_clear(&_local_serializer);
                            
                            ae_serializer_init(&_local_serializer);
                            ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                            mlpunserialize(&_local_serializer, network, _state);
                            ae_serializer_stop(&_local_serializer);
                            ae_serializer_clear(&_local_serializer);
                            
                            ae_frame_leave(_state);
                        }
                    }
                    if( mkind==2 )
                    {
                        mlpcreater2(nin, nhid1, nhid2, nout, a1, a2, &tmp, _state);
                        mlpcopy(&tmp, network, _state);
                    }
                }
            }
        }
    }
    mlprandomizefull(network, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Unsets network (initialize it to smallest network possible
*************************************************************************/
static void testmlpbaseunit_unsetnetwork(multilayerperceptron* network,
     ae_state *_state)
{


    mlpcreate0(1, 1, network, _state);
}


/*************************************************************************
Informational functions test
*************************************************************************/
static void testmlpbaseunit_testinformational(ae_int_t nkind,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t passcount,
     ae_bool* err,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron network;
    ae_int_t n1;
    ae_int_t n2;
    ae_int_t wcount;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double threshold;
    ae_int_t nlayers;
    ae_int_t nmax;
    ae_bool issoftmax;
    ae_matrix neurons;
    ae_vector x;
    ae_vector y;
    double mean;
    double sigma;
    ae_int_t fkind;
    double c;
    double f;
    double df;
    double d2f;
    double s;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&network, _state);
    ae_matrix_init(&neurons, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);

    threshold = 100000*ae_machineepsilon;
    testmlpbaseunit_createnetwork(&network, nkind, 0.0, 0.0, nin, nhid1, nhid2, nout, _state);
    
    /*
     * test MLPProperties()
     */
    mlpproperties(&network, &n1, &n2, &wcount, _state);
    *err = ((*err||n1!=nin)||n2!=nout)||wcount<=0;
    *err = ((*err||mlpgetinputscount(&network, _state)!=nin)||mlpgetoutputscount(&network, _state)!=nout)||mlpgetweightscount(&network, _state)!=wcount;
    
    /*
     * Test network geometry functions
     *
     * In order to do this we calculate neural network output using
     * informational functions only, and compare results with ones
     * obtained with MLPProcess():
     * 1. we allocate 2-dimensional array of neurons and fill it by zeros
     * 2. we full first layer of neurons by input values
     * 3. we move through array, calculating values of subsequent layers
     * 4. if we have classification network, we SOFTMAX-normalize output layer
     * 5. we apply scaling to the outputs
     * 6. we compare results with ones obtained by MLPProcess()
     *
     * NOTE: it is important to do (4) before (5), because on SOFTMAX network
     *       MLPGetOutputScaling() must return Mean=0 and Sigma=1. In order
     *       to test it implicitly, we apply it to the classifier results
     *       (already normalized). If one of the coefficients deviates from
     *       expected values, we will get error during (6).
     */
    nlayers = 2;
    nmax = ae_maxint(nin, nout, _state);
    issoftmax = nkind==1;
    if( nhid1!=0 )
    {
        nlayers = 3;
        nmax = ae_maxint(nmax, nhid1, _state);
    }
    if( nhid2!=0 )
    {
        nlayers = 4;
        nmax = ae_maxint(nmax, nhid2, _state);
    }
    ae_matrix_set_length(&neurons, nlayers, nmax, _state);
    for(i=0; i<=nlayers-1; i++)
    {
        for(j=0; j<=nmax-1; j++)
        {
            neurons.ptr.pp_double[i][j] = (double)(0);
        }
    }
    ae_vector_set_length(&x, nin, _state);
    for(i=0; i<=nin-1; i++)
    {
        x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
    }
    ae_vector_set_length(&y, nout, _state);
    for(i=0; i<=nout-1; i++)
    {
        y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
    }
    for(j=0; j<=nin-1; j++)
    {
        mlpgetinputscaling(&network, j, &mean, &sigma, _state);
        neurons.ptr.pp_double[0][j] = (x.ptr.p_double[j]-mean)/sigma;
    }
    for(i=1; i<=nlayers-1; i++)
    {
        for(j=0; j<=mlpgetlayersize(&network, i, _state)-1; j++)
        {
            for(k=0; k<=mlpgetlayersize(&network, i-1, _state)-1; k++)
            {
                neurons.ptr.pp_double[i][j] = neurons.ptr.pp_double[i][j]+mlpgetweight(&network, i-1, k, i, j, _state)*neurons.ptr.pp_double[i-1][k];
            }
            mlpgetneuroninfo(&network, i, j, &fkind, &c, _state);
            mlpactivationfunction(neurons.ptr.pp_double[i][j]-c, fkind, &f, &df, &d2f, _state);
            neurons.ptr.pp_double[i][j] = f;
        }
    }
    if( nkind==1 )
    {
        s = (double)(0);
        for(j=0; j<=nout-1; j++)
        {
            s = s+ae_exp(neurons.ptr.pp_double[nlayers-1][j], _state);
        }
        for(j=0; j<=nout-1; j++)
        {
            neurons.ptr.pp_double[nlayers-1][j] = ae_exp(neurons.ptr.pp_double[nlayers-1][j], _state)/s;
        }
    }
    for(j=0; j<=nout-1; j++)
    {
        mlpgetoutputscaling(&network, j, &mean, &sigma, _state);
        neurons.ptr.pp_double[nlayers-1][j] = neurons.ptr.pp_double[nlayers-1][j]*sigma+mean;
    }
    mlpprocess(&network, &x, &y, _state);
    for(j=0; j<=nout-1; j++)
    {
        *err = *err||ae_fp_greater(ae_fabs(neurons.ptr.pp_double[nlayers-1][j]-y.ptr.p_double[j], _state),threshold);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Processing functions test
*************************************************************************/
static void testmlpbaseunit_testprocessing(ae_int_t nkind,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t passcount,
     ae_bool* err,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron network;
    multilayerperceptron network2;
    sparsematrix sparsexy;
    ae_matrix densexy;
    ae_int_t npoints;
    ae_int_t subnp;
    ae_bool iscls;
    ae_int_t n1;
    ae_int_t n2;
    ae_int_t wcount;
    ae_bool zeronet;
    double a1;
    double a2;
    ae_int_t pass;
    ae_int_t i;
    ae_int_t j;
    ae_bool allsame;
    ae_vector x1;
    ae_vector x2;
    ae_vector y1;
    ae_vector y2;
    ae_vector p0;
    ae_vector p1;
    ae_int_t pcount;
    double v;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&network, _state);
    _multilayerperceptron_init(&network2, _state);
    _sparsematrix_init(&sparsexy, _state);
    ae_matrix_init(&densexy, 0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);
    ae_vector_init(&y1, 0, DT_REAL, _state);
    ae_vector_init(&y2, 0, DT_REAL, _state);
    ae_vector_init(&p0, 0, DT_REAL, _state);
    ae_vector_init(&p1, 0, DT_REAL, _state);

    ae_assert(passcount>=2, "PassCount<2!", _state);
    
    /*
     * Prepare network
     */
    a1 = (double)(0);
    a2 = (double)(0);
    if( nkind==2 )
    {
        a1 = 1000*ae_randomreal(_state)-500;
        a2 = 2*ae_randomreal(_state)-1;
    }
    if( nkind==3 )
    {
        a1 = 1000*ae_randomreal(_state)-500;
        a2 = a1+(2*ae_randominteger(2, _state)-1)*(0.1+0.9*ae_randomreal(_state));
    }
    testmlpbaseunit_createnetwork(&network, nkind, a1, a2, nin, nhid1, nhid2, nout, _state);
    mlpproperties(&network, &n1, &n2, &wcount, _state);
    iscls = mlpissoftmax(&network, _state);
    
    /*
     * Initialize arrays
     */
    ae_vector_set_length(&x1, nin, _state);
    ae_vector_set_length(&x2, nin, _state);
    ae_vector_set_length(&y1, nout, _state);
    ae_vector_set_length(&y2, nout, _state);
    
    /*
     * Initialize sets
     */
    npoints = ae_randominteger(11, _state)+10;
    if( iscls )
    {
        ae_matrix_set_length(&densexy, npoints, nin+1, _state);
        sparsecreate(npoints, nin+1, npoints, &sparsexy, _state);
    }
    else
    {
        ae_matrix_set_length(&densexy, npoints, nin+nout, _state);
        sparsecreate(npoints, nin+nout, npoints, &sparsexy, _state);
    }
    sparseconverttocrs(&sparsexy, _state);
    
    /*
     * Main cycle
     */
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * Last run is made on zero network
         */
        mlprandomizefull(&network, _state);
        zeronet = ae_false;
        if( pass==passcount )
        {
            ae_v_muld(&network.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1), 0);
            zeronet = ae_true;
        }
        
        /*
         * Same inputs leads to same outputs
         */
        for(i=0; i<=nin-1; i++)
        {
            x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            x2.ptr.p_double[i] = x1.ptr.p_double[i];
        }
        for(i=0; i<=nout-1; i++)
        {
            y1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            y2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        mlpprocess(&network, &x1, &y1, _state);
        mlpprocess(&network, &x2, &y2, _state);
        seterrorflag(err, ae_fp_neq(testmlpbaseunit_vectordiff(&y1, &y2, nout, 1.0, _state),0.0), _state);
        
        /*
         * Same inputs on original network leads to same outputs
         * on copy created:
         * * using MLPCopy
         * * using MLPCopyShared
         */
        for(i=0; i<=nin-1; i++)
        {
            x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            x2.ptr.p_double[i] = x1.ptr.p_double[i];
        }
        for(i=0; i<=nout-1; i++)
        {
            y1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        for(i=0; i<=nout-1; i++)
        {
            y2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        testmlpbaseunit_unsetnetwork(&network2, _state);
        mlpcopy(&network, &network2, _state);
        mlpprocess(&network, &x1, &y1, _state);
        mlpprocess(&network2, &x2, &y2, _state);
        seterrorflag(err, ae_fp_neq(testmlpbaseunit_vectordiff(&y1, &y2, nout, 1.0, _state),0.0), _state);
        for(i=0; i<=nout-1; i++)
        {
            y2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        testmlpbaseunit_unsetnetwork(&network2, _state);
        mlpcopyshared(&network, &network2, _state);
        mlpprocess(&network, &x1, &y1, _state);
        mlpprocess(&network2, &x2, &y2, _state);
        seterrorflag(err, ae_fp_neq(testmlpbaseunit_vectordiff(&y1, &y2, nout, 1.0, _state),0.0), _state);
        
        /*
         * Additionally we tests functions for copying of tunable
         * parameters by:
         * * copying network using MLPCopy
         * * randomizing tunable parameters with MLPRandomizeFull()
         * * copying tunable parameters with:
         *   a) MLPCopyTunableParameters
         *   b) combination of MLPExportTunableParameters and
         *      MLPImportTunableParameters - we export parameters
         *      to P1, copy PCount elements to P2, then test import.
         */
        for(i=0; i<=nin-1; i++)
        {
            x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            x2.ptr.p_double[i] = x1.ptr.p_double[i];
        }
        for(i=0; i<=nout-1; i++)
        {
            y1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        for(i=0; i<=nout-1; i++)
        {
            y2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        testmlpbaseunit_unsetnetwork(&network2, _state);
        mlpcopy(&network, &network2, _state);
        mlprandomizefull(&network2, _state);
        mlpcopytunableparameters(&network, &network2, _state);
        mlpprocess(&network, &x1, &y1, _state);
        mlpprocess(&network2, &x2, &y2, _state);
        seterrorflag(err, ae_fp_neq(testmlpbaseunit_vectordiff(&y1, &y2, nout, 1.0, _state),0.0), _state);
        for(i=0; i<=nout-1; i++)
        {
            y2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        testmlpbaseunit_unsetnetwork(&network2, _state);
        mlpcopy(&network, &network2, _state);
        mlprandomizefull(&network2, _state);
        mlpexporttunableparameters(&network, &p0, &pcount, _state);
        ae_vector_set_length(&p1, pcount, _state);
        for(i=0; i<=pcount-1; i++)
        {
            p1.ptr.p_double[i] = p0.ptr.p_double[i];
        }
        mlpimporttunableparameters(&network2, &p1, _state);
        mlpprocess(&network, &x1, &y1, _state);
        mlpprocess(&network2, &x2, &y2, _state);
        seterrorflag(err, ae_fp_neq(testmlpbaseunit_vectordiff(&y1, &y2, nout, 1.0, _state),0.0), _state);
        
        /*
         * Same inputs on original network leads to same outputs
         * on copy created using MLPSerialize
         */
        testmlpbaseunit_unsetnetwork(&network2, _state);
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
            mlpalloc(&_local_serializer, &network, _state);
            _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
            ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
            ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
            mlpserialize(&_local_serializer, &network, _state);
            ae_serializer_stop(&_local_serializer);
            ae_serializer_clear(&_local_serializer);
            
            ae_serializer_init(&_local_serializer);
            ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
            mlpunserialize(&_local_serializer, &network2, _state);
            ae_serializer_stop(&_local_serializer);
            ae_serializer_clear(&_local_serializer);
            
            ae_frame_leave(_state);
        }
        for(i=0; i<=nin-1; i++)
        {
            x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            x2.ptr.p_double[i] = x1.ptr.p_double[i];
        }
        for(i=0; i<=nout-1; i++)
        {
            y1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            y2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        mlpprocess(&network, &x1, &y1, _state);
        mlpprocess(&network2, &x2, &y2, _state);
        allsame = ae_true;
        for(i=0; i<=nout-1; i++)
        {
            allsame = allsame&&ae_fp_eq(y1.ptr.p_double[i],y2.ptr.p_double[i]);
        }
        *err = *err||!allsame;
        
        /*
         * Different inputs leads to different outputs (non-zero network)
         */
        if( !zeronet )
        {
            for(i=0; i<=nin-1; i++)
            {
                x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                x2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            for(i=0; i<=nout-1; i++)
            {
                y1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                y2.ptr.p_double[i] = y1.ptr.p_double[i];
            }
            mlpprocess(&network, &x1, &y1, _state);
            mlpprocess(&network, &x2, &y2, _state);
            allsame = ae_true;
            for(i=0; i<=nout-1; i++)
            {
                allsame = allsame&&ae_fp_eq(y1.ptr.p_double[i],y2.ptr.p_double[i]);
            }
            *err = *err||allsame;
        }
        
        /*
         * Randomization changes outputs (when inputs are unchanged, non-zero network)
         */
        if( !zeronet )
        {
            for(i=0; i<=nin-1; i++)
            {
                x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                x2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            for(i=0; i<=nout-1; i++)
            {
                y1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                y2.ptr.p_double[i] = y1.ptr.p_double[i];
            }
            mlpcopy(&network, &network2, _state);
            mlprandomize(&network2, _state);
            mlpprocess(&network, &x1, &y1, _state);
            mlpprocess(&network2, &x1, &y2, _state);
            allsame = ae_true;
            for(i=0; i<=nout-1; i++)
            {
                allsame = allsame&&ae_fp_eq(y1.ptr.p_double[i],y2.ptr.p_double[i]);
            }
            *err = *err||allsame;
        }
        
        /*
         * Full randomization changes outputs (when inputs are unchanged, non-zero network)
         */
        if( !zeronet )
        {
            for(i=0; i<=nin-1; i++)
            {
                x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                x2.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            for(i=0; i<=nout-1; i++)
            {
                y1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                y2.ptr.p_double[i] = y1.ptr.p_double[i];
            }
            mlpcopy(&network, &network2, _state);
            mlprandomizefull(&network2, _state);
            mlpprocess(&network, &x1, &y1, _state);
            mlpprocess(&network2, &x1, &y2, _state);
            allsame = ae_true;
            for(i=0; i<=nout-1; i++)
            {
                allsame = allsame&&ae_fp_eq(y1.ptr.p_double[i],y2.ptr.p_double[i]);
            }
            *err = *err||allsame;
        }
        
        /*
         * Normalization properties
         */
        if( nkind==1 )
        {
            
            /*
             * Classifier network outputs are normalized
             */
            for(i=0; i<=nin-1; i++)
            {
                x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            mlpprocess(&network, &x1, &y1, _state);
            v = (double)(0);
            for(i=0; i<=nout-1; i++)
            {
                v = v+y1.ptr.p_double[i];
                *err = *err||ae_fp_less(y1.ptr.p_double[i],(double)(0));
            }
            *err = *err||ae_fp_greater(ae_fabs(v-1, _state),1000*ae_machineepsilon);
        }
        if( nkind==2 )
        {
            
            /*
             * B-type network outputs are bounded from above/below
             */
            for(i=0; i<=nin-1; i++)
            {
                x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            mlpprocess(&network, &x1, &y1, _state);
            for(i=0; i<=nout-1; i++)
            {
                if( ae_fp_greater_eq(a2,(double)(0)) )
                {
                    *err = *err||ae_fp_less(y1.ptr.p_double[i],a1);
                }
                else
                {
                    *err = *err||ae_fp_greater(y1.ptr.p_double[i],a1);
                }
            }
        }
        if( nkind==3 )
        {
            
            /*
             * R-type network outputs are within [A1,A2] (or [A2,A1])
             */
            for(i=0; i<=nin-1; i++)
            {
                x1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            mlpprocess(&network, &x1, &y1, _state);
            for(i=0; i<=nout-1; i++)
            {
                *err = (*err||ae_fp_less(y1.ptr.p_double[i],ae_minreal(a1, a2, _state)))||ae_fp_greater(y1.ptr.p_double[i],ae_maxreal(a1, a2, _state));
            }
        }
        
        /*
         * Comperison MLPInitPreprocessor results with
         * MLPInitPreprocessorSparse results
         */
        sparseconverttohash(&sparsexy, _state);
        if( iscls )
        {
            for(i=0; i<=npoints-1; i++)
            {
                for(j=0; j<=nin-1; j++)
                {
                    densexy.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                    sparseset(&sparsexy, i, j, densexy.ptr.pp_double[i][j], _state);
                }
                densexy.ptr.pp_double[i][nin] = (double)(ae_randominteger(nout, _state));
                sparseset(&sparsexy, i, j, densexy.ptr.pp_double[i][nin], _state);
            }
        }
        else
        {
            for(i=0; i<=npoints-1; i++)
            {
                for(j=0; j<=nin+nout-1; j++)
                {
                    densexy.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                    sparseset(&sparsexy, i, j, densexy.ptr.pp_double[i][j], _state);
                }
            }
        }
        sparseconverttocrs(&sparsexy, _state);
        mlpcopy(&network, &network2, _state);
        mlpinitpreprocessor(&network, &densexy, npoints, _state);
        mlpinitpreprocessorsparse(&network2, &sparsexy, npoints, _state);
        subnp = ae_randominteger(npoints, _state);
        for(i=0; i<=subnp-1; i++)
        {
            for(j=0; j<=nin-1; j++)
            {
                x1.ptr.p_double[j] = 2*ae_randomreal(_state)-1;
            }
            mlpprocess(&network, &x1, &y1, _state);
            mlpprocess(&network2, &x1, &y2, _state);
            for(j=0; j<=nout-1; j++)
            {
                *err = *err||ae_fp_greater(ae_fabs(y1.ptr.p_double[j]-y2.ptr.p_double[j], _state),1.0E-6);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Gradient functions test
*************************************************************************/
static void testmlpbaseunit_testgradient(ae_int_t nkind,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t passcount,
     ae_int_t sizemin,
     ae_int_t sizemax,
     ae_bool* err,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron network;
    sparsematrix sparsexy;
    sparsematrix sparsexy2;
    ae_int_t n1;
    ae_int_t n2;
    ae_int_t wcount;
    double h;
    double etol;
    double escale;
    double gscale;
    double nonstricttolerance;
    double a1;
    double a2;
    ae_int_t pass;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t ssize;
    ae_int_t subsetsize;
    ae_int_t rowsize;
    ae_matrix xy;
    ae_matrix xy2;
    ae_vector grad1;
    ae_vector grad2;
    ae_vector gradsp;
    ae_vector x;
    ae_vector y;
    ae_vector x1;
    ae_vector x2;
    ae_vector y1;
    ae_vector y2;
    ae_vector idx;
    double v;
    double e;
    double e1;
    double e2;
    double esp;
    double v1;
    double v2;
    double v3;
    double v4;
    double wprev;
    double referencee;
    ae_vector referenceg;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&network, _state);
    _sparsematrix_init(&sparsexy, _state);
    _sparsematrix_init(&sparsexy2, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xy2, 0, 0, DT_REAL, _state);
    ae_vector_init(&grad1, 0, DT_REAL, _state);
    ae_vector_init(&grad2, 0, DT_REAL, _state);
    ae_vector_init(&gradsp, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);
    ae_vector_init(&y1, 0, DT_REAL, _state);
    ae_vector_init(&y2, 0, DT_REAL, _state);
    ae_vector_init(&idx, 0, DT_INT, _state);
    ae_vector_init(&referenceg, 0, DT_REAL, _state);

    a1 = (double)(0);
    a2 = (double)(0);
    if( nkind==2 )
    {
        a1 = 1000*ae_randomreal(_state)-500;
        a2 = 2*ae_randomreal(_state)-1;
    }
    if( nkind==3 )
    {
        a1 = 1000*ae_randomreal(_state)-500;
        a2 = a1+(2*ae_randominteger(2, _state)-1)*(0.1+0.9*ae_randomreal(_state));
    }
    testmlpbaseunit_createnetwork(&network, nkind, a1, a2, nin, nhid1, nhid2, nout, _state);
    mlpproperties(&network, &n1, &n2, &wcount, _state);
    h = 0.0001;
    etol = 1.0E-2;
    escale = 1.0E-2;
    gscale = 1.0E-2;
    nonstricttolerance = 0.01;
    
    /*
     * Initialize
     */
    ae_vector_set_length(&x, nin, _state);
    ae_vector_set_length(&x1, nin, _state);
    ae_vector_set_length(&x2, nin, _state);
    ae_vector_set_length(&y, nout, _state);
    ae_vector_set_length(&y1, nout, _state);
    ae_vector_set_length(&y2, nout, _state);
    ae_vector_set_length(&referenceg, wcount, _state);
    ae_vector_set_length(&grad1, wcount, _state);
    ae_vector_set_length(&grad2, wcount, _state);
    
    /*
     * Process
     */
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * Randomize network, then re-randomaze weights manually.
         *
         * NOTE: weights magnitude is chosen to be small, about 0.1,
         *       which allows us to avoid oversaturated network.
         *       In 10% of cases we use zero weights.
         */
        mlprandomizefull(&network, _state);
        if( ae_fp_less_eq(ae_randomreal(_state),0.1) )
        {
            for(i=0; i<=wcount-1; i++)
            {
                network.weights.ptr.p_double[i] = 0.0;
            }
        }
        else
        {
            for(i=0; i<=wcount-1; i++)
            {
                network.weights.ptr.p_double[i] = 0.2*ae_randomreal(_state)-0.1;
            }
        }
        
        /*
         * Test MLPError(), MLPErrorSparse(), MLPGrad() for single-element dataset:
         * * generate input X, output Y, combine them in dataset XY
         * * calculate "reference" error on dataset manually (call MLPProcess and evaluate sum-of-squared errors)
         * * calculate "reference" gradient by performing numerical differentiation of "reference" error
         *   using 4-point differentiation formula
         * * test error/gradient returned by MLPGrad(), MLPError(), MLPErrorSparse()
         */
        ae_matrix_set_length(&xy, 1, nin+nout, _state);
        sparsecreate(1, nin+nout, nin+nout, &sparsexy, _state);
        for(i=0; i<=nin-1; i++)
        {
            x.ptr.p_double[i] = 4*ae_randomreal(_state)-2;
        }
        ae_v_move(&xy.ptr.pp_double[0][0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,nin-1));
        for(i=0; i<=nin-1; i++)
        {
            sparseset(&sparsexy, 0, i, x.ptr.p_double[i], _state);
        }
        if( mlpissoftmax(&network, _state) )
        {
            for(i=0; i<=nout-1; i++)
            {
                y.ptr.p_double[i] = (double)(0);
            }
            xy.ptr.pp_double[0][nin] = (double)(ae_randominteger(nout, _state));
            sparseset(&sparsexy, 0, nin, xy.ptr.pp_double[0][nin], _state);
            y.ptr.p_double[ae_round(xy.ptr.pp_double[0][nin], _state)] = (double)(1);
        }
        else
        {
            for(i=0; i<=nout-1; i++)
            {
                y.ptr.p_double[i] = 4*ae_randomreal(_state)-2;
                sparseset(&sparsexy, 0, nin+i, y.ptr.p_double[i], _state);
            }
            ae_v_move(&xy.ptr.pp_double[0][nin], 1, &y.ptr.p_double[0], 1, ae_v_len(nin,nin+nout-1));
        }
        sparseconverttocrs(&sparsexy, _state);
        mlpprocess(&network, &x, &y2, _state);
        ae_v_sub(&y2.ptr.p_double[0], 1, &y.ptr.p_double[0], 1, ae_v_len(0,nout-1));
        referencee = ae_v_dotproduct(&y2.ptr.p_double[0], 1, &y2.ptr.p_double[0], 1, ae_v_len(0,nout-1));
        referencee = referencee/2;
        for(i=0; i<=wcount-1; i++)
        {
            wprev = network.weights.ptr.p_double[i];
            network.weights.ptr.p_double[i] = wprev-2*h;
            mlpprocess(&network, &x, &y1, _state);
            ae_v_sub(&y1.ptr.p_double[0], 1, &y.ptr.p_double[0], 1, ae_v_len(0,nout-1));
            v1 = ae_v_dotproduct(&y1.ptr.p_double[0], 1, &y1.ptr.p_double[0], 1, ae_v_len(0,nout-1));
            v1 = v1/2;
            network.weights.ptr.p_double[i] = wprev-h;
            mlpprocess(&network, &x, &y1, _state);
            ae_v_sub(&y1.ptr.p_double[0], 1, &y.ptr.p_double[0], 1, ae_v_len(0,nout-1));
            v2 = ae_v_dotproduct(&y1.ptr.p_double[0], 1, &y1.ptr.p_double[0], 1, ae_v_len(0,nout-1));
            v2 = v2/2;
            network.weights.ptr.p_double[i] = wprev+h;
            mlpprocess(&network, &x, &y1, _state);
            ae_v_sub(&y1.ptr.p_double[0], 1, &y.ptr.p_double[0], 1, ae_v_len(0,nout-1));
            v3 = ae_v_dotproduct(&y1.ptr.p_double[0], 1, &y1.ptr.p_double[0], 1, ae_v_len(0,nout-1));
            v3 = v3/2;
            network.weights.ptr.p_double[i] = wprev+2*h;
            mlpprocess(&network, &x, &y1, _state);
            ae_v_sub(&y1.ptr.p_double[0], 1, &y.ptr.p_double[0], 1, ae_v_len(0,nout-1));
            v4 = ae_v_dotproduct(&y1.ptr.p_double[0], 1, &y1.ptr.p_double[0], 1, ae_v_len(0,nout-1));
            v4 = v4/2;
            network.weights.ptr.p_double[i] = wprev;
            referenceg.ptr.p_double[i] = (v1-8*v2+8*v3-v4)/(12*h);
        }
        mlpgrad(&network, &x, &y, &e, &grad2, _state);
        seterrorflagdiff(err, e, referencee, etol, escale, _state);
        seterrorflagdiff(err, mlperror(&network, &xy, 1, _state), referencee, etol, escale, _state);
        seterrorflagdiff(err, mlperrorsparse(&network, &sparsexy, 1, _state), referencee, etol, escale, _state);
        seterrorflag(err, ae_fp_greater(testmlpbaseunit_vectordiff(&referenceg, &grad2, wcount, gscale, _state),etol), _state);
        
        /*
         * Test MLPErrorN(), MLPGradN() for single-element dataset:
         * * generate input X, output Y, combine them in dataset XY
         * * calculate "reference" error on dataset manually (call MLPProcess and evaluate sum-of-squared errors)
         * * calculate "reference" gradient by performing numerical differentiation of "reference" error
         * * test error/gradient returned by MLPGradN(), MLPErrorN()
         *
         * NOTE: because we use inexact 2-point formula, we perform gradient test with NonStrictTolerance
         */
        ae_matrix_set_length(&xy, 1, nin+nout, _state);
        for(i=0; i<=nin-1; i++)
        {
            x.ptr.p_double[i] = 4*ae_randomreal(_state)-2;
        }
        ae_v_move(&xy.ptr.pp_double[0][0], 1, &x.ptr.p_double[0], 1, ae_v_len(0,nin-1));
        if( mlpissoftmax(&network, _state) )
        {
            for(i=0; i<=nout-1; i++)
            {
                y.ptr.p_double[i] = (double)(0);
            }
            xy.ptr.pp_double[0][nin] = (double)(ae_randominteger(nout, _state));
            y.ptr.p_double[ae_round(xy.ptr.pp_double[0][nin], _state)] = (double)(1);
        }
        else
        {
            for(i=0; i<=nout-1; i++)
            {
                y.ptr.p_double[i] = 4*ae_randomreal(_state)-2;
            }
            ae_v_move(&xy.ptr.pp_double[0][nin], 1, &y.ptr.p_double[0], 1, ae_v_len(nin,nin+nout-1));
        }
        mlpprocess(&network, &x, &y2, _state);
        referencee = (double)(0);
        if( nkind!=1 )
        {
            for(i=0; i<=nout-1; i++)
            {
                referencee = referencee+0.5*ae_sqr(y2.ptr.p_double[i]-y.ptr.p_double[i], _state);
            }
        }
        else
        {
            for(i=0; i<=nout-1; i++)
            {
                if( ae_fp_neq(y.ptr.p_double[i],(double)(0)) )
                {
                    if( ae_fp_eq(y2.ptr.p_double[i],(double)(0)) )
                    {
                        referencee = referencee+y.ptr.p_double[i]*ae_log(ae_maxrealnumber, _state);
                    }
                    else
                    {
                        referencee = referencee+y.ptr.p_double[i]*ae_log(y.ptr.p_double[i]/y2.ptr.p_double[i], _state);
                    }
                }
            }
        }
        for(i=0; i<=wcount-1; i++)
        {
            wprev = network.weights.ptr.p_double[i];
            network.weights.ptr.p_double[i] = wprev+h;
            mlpprocess(&network, &x, &y2, _state);
            network.weights.ptr.p_double[i] = wprev-h;
            mlpprocess(&network, &x, &y1, _state);
            network.weights.ptr.p_double[i] = wprev;
            v = (double)(0);
            if( nkind!=1 )
            {
                for(j=0; j<=nout-1; j++)
                {
                    v = v+0.5*(ae_sqr(y2.ptr.p_double[j]-y.ptr.p_double[j], _state)-ae_sqr(y1.ptr.p_double[j]-y.ptr.p_double[j], _state))/(2*h);
                }
            }
            else
            {
                for(j=0; j<=nout-1; j++)
                {
                    if( ae_fp_neq(y.ptr.p_double[j],(double)(0)) )
                    {
                        if( ae_fp_eq(y2.ptr.p_double[j],(double)(0)) )
                        {
                            v = v+y.ptr.p_double[j]*ae_log(ae_maxrealnumber, _state);
                        }
                        else
                        {
                            v = v+y.ptr.p_double[j]*ae_log(y.ptr.p_double[j]/y2.ptr.p_double[j], _state);
                        }
                        if( ae_fp_eq(y1.ptr.p_double[j],(double)(0)) )
                        {
                            v = v-y.ptr.p_double[j]*ae_log(ae_maxrealnumber, _state);
                        }
                        else
                        {
                            v = v-y.ptr.p_double[j]*ae_log(y.ptr.p_double[j]/y1.ptr.p_double[j], _state);
                        }
                    }
                }
                v = v/(2*h);
            }
            referenceg.ptr.p_double[i] = v;
        }
        mlpgradn(&network, &x, &y, &e, &grad2, _state);
        seterrorflagdiff(err, e, referencee, etol, escale, _state);
        seterrorflagdiff(err, mlperrorn(&network, &xy, 1, _state), referencee, etol, escale, _state);
        seterrorflag(err, ae_fp_greater(testmlpbaseunit_vectordiff(&referenceg, &grad2, wcount, gscale, _state),nonstricttolerance), _state);
        
        /*
         * Test that gradient calculation functions automatically allocate
         * space for gradient, if needed.
         *
         * NOTE: we perform test with empty dataset.
         */
        sparsecreate(1, nin+nout, 0, &sparsexy, _state);
        sparseconverttocrs(&sparsexy, _state);
        ae_vector_set_length(&grad1, 1, _state);
        mlpgradbatch(&network, &xy, 0, &e1, &grad1, _state);
        seterrorflag(err, grad1.cnt!=wcount, _state);
        ae_vector_set_length(&grad1, 1, _state);
        mlpgradbatchsparse(&network, &sparsexy, 0, &e1, &grad1, _state);
        seterrorflag(err, grad1.cnt!=wcount, _state);
        ae_vector_set_length(&grad1, 1, _state);
        mlpgradbatchsubset(&network, &xy, 0, &idx, 0, &e1, &grad1, _state);
        seterrorflag(err, grad1.cnt!=wcount, _state);
        ae_vector_set_length(&grad1, 1, _state);
        mlpgradbatchsparsesubset(&network, &sparsexy, 0, &idx, 0, &e1, &grad1, _state);
        seterrorflag(err, grad1.cnt!=wcount, _state);
        
        /*
         * Test MLPError(), MLPErrorSparse(), MLPGradBatch(), MLPGradBatchSparse() for many-element dataset:
         * * generate random dataset XY
         * * calculate "reference" error/gradient using MLPGrad(), which was tested in previous
         *   section and is assumed to work correctly
         * * test results returned by MLPGradBatch/MLPGradBatchSparse against reference ones
         *
         * NOTE: about 10% of tests are performed with zero SSize
         */
        ssize = sizemin+ae_randominteger(sizemax-sizemin+1, _state);
        ae_matrix_set_length(&xy, ae_maxint(ssize, 1, _state), nin+nout, _state);
        sparsecreate(ae_maxint(ssize, 1, _state), nin+nout, ssize*(nin+nout), &sparsexy, _state);
        for(i=0; i<=wcount-1; i++)
        {
            referenceg.ptr.p_double[i] = (double)(0);
        }
        referencee = (double)(0);
        for(i=0; i<=ssize-1; i++)
        {
            for(j=0; j<=nin-1; j++)
            {
                x1.ptr.p_double[j] = 4*ae_randomreal(_state)-2;
                sparseset(&sparsexy, i, j, x1.ptr.p_double[j], _state);
            }
            ae_v_move(&xy.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,nin-1));
            if( mlpissoftmax(&network, _state) )
            {
                for(j=0; j<=nout-1; j++)
                {
                    y1.ptr.p_double[j] = (double)(0);
                }
                xy.ptr.pp_double[i][nin] = (double)(ae_randominteger(nout, _state));
                sparseset(&sparsexy, i, nin, xy.ptr.pp_double[i][nin], _state);
                y1.ptr.p_double[ae_round(xy.ptr.pp_double[i][nin], _state)] = (double)(1);
            }
            else
            {
                for(j=0; j<=nout-1; j++)
                {
                    y1.ptr.p_double[j] = 4*ae_randomreal(_state)-2;
                    sparseset(&sparsexy, i, nin+j, y1.ptr.p_double[j], _state);
                }
                ae_v_move(&xy.ptr.pp_double[i][nin], 1, &y1.ptr.p_double[0], 1, ae_v_len(nin,nin+nout-1));
            }
            mlpgrad(&network, &x1, &y1, &v, &grad2, _state);
            referencee = referencee+v;
            ae_v_add(&referenceg.ptr.p_double[0], 1, &grad2.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        }
        sparseconverttocrs(&sparsexy, _state);
        e2 = mlperror(&network, &xy, ssize, _state);
        seterrorflagdiff(err, e2, referencee, etol, escale, _state);
        e2 = mlperrorsparse(&network, &sparsexy, ssize, _state);
        seterrorflagdiff(err, e2, referencee, etol, escale, _state);
        mlpgradbatch(&network, &xy, ssize, &e2, &grad2, _state);
        seterrorflagdiff(err, e2, referencee, etol, escale, _state);
        seterrorflag(err, ae_fp_greater(testmlpbaseunit_vectordiff(&referenceg, &grad2, wcount, gscale, _state),etol), _state);
        mlpgradbatchsparse(&network, &sparsexy, ssize, &esp, &gradsp, _state);
        seterrorflagdiff(err, esp, referencee, etol, escale, _state);
        seterrorflag(err, ae_fp_greater(testmlpbaseunit_vectordiff(&referenceg, &gradsp, wcount, gscale, _state),etol), _state);
        
        /*
         * Test MLPErrorSubset(), MLPGradBatchSubset(), MLPErrorSparseSubset(), MLPGradBatchSparseSubset()
         * for many-element dataset with different types of subsets:
         * * generate random dataset XY
         * * "reference" error/gradient are calculated with MLPGradBatch(),
         *   which was tested in previous section and is assumed to work correctly
         * * we perform tests for different subsets:
         *   * SubsetSize<0 - subset is a full dataset
         *   * SubsetSize=0 - subset is empty
         *   * SubsetSize>0 - random subset
         */
        ssize = sizemin+ae_randominteger(sizemax-sizemin+1, _state);
        ae_matrix_set_length(&xy, ae_maxint(ssize, 1, _state), nin+nout, _state);
        sparsecreate(ae_maxint(ssize, 1, _state), nin+nout, ssize*(nin+nout), &sparsexy, _state);
        for(i=0; i<=ssize-1; i++)
        {
            for(j=0; j<=nin-1; j++)
            {
                x1.ptr.p_double[j] = 4*ae_randomreal(_state)-2;
                sparseset(&sparsexy, i, j, x1.ptr.p_double[j], _state);
            }
            ae_v_move(&xy.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,nin-1));
            if( mlpissoftmax(&network, _state) )
            {
                for(j=0; j<=nout-1; j++)
                {
                    y1.ptr.p_double[j] = (double)(0);
                }
                xy.ptr.pp_double[i][nin] = (double)(ae_randominteger(nout, _state));
                sparseset(&sparsexy, i, nin, xy.ptr.pp_double[i][nin], _state);
                y1.ptr.p_double[ae_round(xy.ptr.pp_double[i][nin], _state)] = (double)(1);
            }
            else
            {
                for(j=0; j<=nout-1; j++)
                {
                    y1.ptr.p_double[j] = 4*ae_randomreal(_state)-2;
                    sparseset(&sparsexy, i, nin+j, y1.ptr.p_double[j], _state);
                }
                ae_v_move(&xy.ptr.pp_double[i][nin], 1, &y1.ptr.p_double[0], 1, ae_v_len(nin,nin+nout-1));
            }
        }
        sparseconverttocrs(&sparsexy, _state);
        if( ssize>0 )
        {
            subsetsize = 1+ae_randominteger(10, _state);
            ae_matrix_set_length(&xy2, subsetsize, nin+nout, _state);
            ae_vector_set_length(&idx, subsetsize, _state);
            sparsecreate(subsetsize, nin+nout, subsetsize*(nin+nout), &sparsexy2, _state);
            if( mlpissoftmax(&network, _state) )
            {
                rowsize = nin+1;
            }
            else
            {
                rowsize = nin+nout;
            }
            for(i=0; i<=subsetsize-1; i++)
            {
                k = ae_randominteger(ssize, _state);
                idx.ptr.p_int[i] = k;
                for(j=0; j<=rowsize-1; j++)
                {
                    xy2.ptr.pp_double[i][j] = xy.ptr.pp_double[k][j];
                    sparseset(&sparsexy2, i, j, sparseget(&sparsexy, k, j, _state), _state);
                }
            }
            sparseconverttocrs(&sparsexy2, _state);
        }
        else
        {
            subsetsize = 0;
            ae_matrix_set_length(&xy2, 0, 0, _state);
            ae_vector_set_length(&idx, 0, _state);
            sparsecreate(1, nin+nout, 0, &sparsexy2, _state);
            sparseconverttocrs(&sparsexy2, _state);
        }
        mlpgradbatch(&network, &xy, ssize, &referencee, &referenceg, _state);
        e2 = mlperrorsubset(&network, &xy, ssize, &idx, -1, _state);
        esp = mlperrorsparsesubset(&network, &sparsexy, ssize, &idx, -1, _state);
        seterrorflagdiff(err, e2, referencee, etol, escale, _state);
        seterrorflagdiff(err, esp, referencee, etol, escale, _state);
        mlpgradbatchsubset(&network, &xy, ssize, &idx, -1, &e2, &grad2, _state);
        mlpgradbatchsparsesubset(&network, &sparsexy, ssize, &idx, -1, &esp, &gradsp, _state);
        seterrorflagdiff(err, e2, referencee, etol, escale, _state);
        seterrorflagdiff(err, esp, referencee, etol, escale, _state);
        seterrorflag(err, ae_fp_greater(testmlpbaseunit_vectordiff(&referenceg, &grad2, wcount, gscale, _state),etol), _state);
        seterrorflag(err, ae_fp_greater(testmlpbaseunit_vectordiff(&referenceg, &gradsp, wcount, gscale, _state),etol), _state);
        mlpgradbatch(&network, &xy, 0, &referencee, &referenceg, _state);
        e2 = mlperrorsubset(&network, &xy, ssize, &idx, 0, _state);
        esp = mlperrorsparsesubset(&network, &sparsexy, ssize, &idx, 0, _state);
        seterrorflagdiff(err, e2, referencee, etol, escale, _state);
        seterrorflagdiff(err, esp, referencee, etol, escale, _state);
        mlpgradbatchsubset(&network, &xy, ssize, &idx, 0, &e2, &grad2, _state);
        mlpgradbatchsparsesubset(&network, &sparsexy, ssize, &idx, 0, &esp, &gradsp, _state);
        seterrorflagdiff(err, e2, referencee, etol, escale, _state);
        seterrorflagdiff(err, esp, referencee, etol, escale, _state);
        seterrorflag(err, ae_fp_greater(testmlpbaseunit_vectordiff(&referenceg, &grad2, wcount, gscale, _state),etol), _state);
        seterrorflag(err, ae_fp_greater(testmlpbaseunit_vectordiff(&referenceg, &gradsp, wcount, gscale, _state),etol), _state);
        mlpgradbatch(&network, &xy2, subsetsize, &referencee, &referenceg, _state);
        e2 = mlperrorsubset(&network, &xy, ssize, &idx, subsetsize, _state);
        esp = mlperrorsparsesubset(&network, &sparsexy, ssize, &idx, subsetsize, _state);
        seterrorflagdiff(err, e2, referencee, etol, escale, _state);
        seterrorflagdiff(err, esp, referencee, etol, escale, _state);
        mlpgradbatchsubset(&network, &xy, ssize, &idx, subsetsize, &e2, &grad2, _state);
        mlpgradbatchsparsesubset(&network, &sparsexy, ssize, &idx, subsetsize, &esp, &gradsp, _state);
        seterrorflagdiff(err, e2, referencee, etol, escale, _state);
        seterrorflagdiff(err, esp, referencee, etol, escale, _state);
        seterrorflag(err, ae_fp_greater(testmlpbaseunit_vectordiff(&referenceg, &grad2, wcount, gscale, _state),etol), _state);
        seterrorflag(err, ae_fp_greater(testmlpbaseunit_vectordiff(&referenceg, &gradsp, wcount, gscale, _state),etol), _state);
        
        /*
         * Test MLPGradNBatch() for many-element dataset:
         * * generate random dataset XY
         * * calculate "reference" error/gradient using MLPGrad(), which was tested in previous
         *   section and is assumed to work correctly
         * * test results returned by MLPGradNBatch against reference ones
         */
        ssize = sizemin+ae_randominteger(sizemax-sizemin+1, _state);
        ae_matrix_set_length(&xy, ssize, nin+nout, _state);
        for(i=0; i<=wcount-1; i++)
        {
            referenceg.ptr.p_double[i] = (double)(0);
        }
        referencee = (double)(0);
        for(i=0; i<=ssize-1; i++)
        {
            for(j=0; j<=nin-1; j++)
            {
                x1.ptr.p_double[j] = 4*ae_randomreal(_state)-2;
            }
            ae_v_move(&xy.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,nin-1));
            if( mlpissoftmax(&network, _state) )
            {
                for(j=0; j<=nout-1; j++)
                {
                    y1.ptr.p_double[j] = (double)(0);
                }
                xy.ptr.pp_double[i][nin] = (double)(ae_randominteger(nout, _state));
                y1.ptr.p_double[ae_round(xy.ptr.pp_double[i][nin], _state)] = (double)(1);
            }
            else
            {
                for(j=0; j<=nout-1; j++)
                {
                    y1.ptr.p_double[j] = 4*ae_randomreal(_state)-2;
                }
                ae_v_move(&xy.ptr.pp_double[i][nin], 1, &y1.ptr.p_double[0], 1, ae_v_len(nin,nin+nout-1));
            }
            mlpgradn(&network, &x1, &y1, &v, &grad2, _state);
            referencee = referencee+v;
            ae_v_add(&referenceg.ptr.p_double[0], 1, &grad2.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        }
        mlpgradnbatch(&network, &xy, ssize, &e2, &grad2, _state);
        seterrorflagdiff(err, e2, referencee, etol, escale, _state);
        seterrorflag(err, ae_fp_greater(testmlpbaseunit_vectordiff(&referenceg, &grad2, wcount, gscale, _state),etol), _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Hessian functions test
*************************************************************************/
static void testmlpbaseunit_testhessian(ae_int_t nkind,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t passcount,
     ae_bool* err,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron network;
    ae_int_t hkind;
    ae_int_t n1;
    ae_int_t n2;
    ae_int_t wcount;
    double h;
    double etol;
    ae_int_t pass;
    ae_int_t i;
    ae_int_t j;
    ae_int_t ssize;
    double a1;
    double a2;
    ae_matrix xy;
    ae_matrix h1;
    ae_matrix h2;
    ae_vector grad1;
    ae_vector grad2;
    ae_vector grad3;
    ae_vector x;
    ae_vector y;
    ae_vector x1;
    ae_vector x2;
    ae_vector y1;
    ae_vector y2;
    double v;
    double e1;
    double e2;
    double wprev;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&network, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&h1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&h2, 0, 0, DT_REAL, _state);
    ae_vector_init(&grad1, 0, DT_REAL, _state);
    ae_vector_init(&grad2, 0, DT_REAL, _state);
    ae_vector_init(&grad3, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);
    ae_vector_init(&y1, 0, DT_REAL, _state);
    ae_vector_init(&y2, 0, DT_REAL, _state);

    ae_assert(passcount>=2, "PassCount<2!", _state);
    a1 = (double)(0);
    a2 = (double)(0);
    if( nkind==2 )
    {
        a1 = 1000*ae_randomreal(_state)-500;
        a2 = 2*ae_randomreal(_state)-1;
    }
    if( nkind==3 )
    {
        a1 = 1000*ae_randomreal(_state)-500;
        a2 = a1+(2*ae_randominteger(2, _state)-1)*(0.1+0.9*ae_randomreal(_state));
    }
    testmlpbaseunit_createnetwork(&network, nkind, a1, a2, nin, nhid1, nhid2, nout, _state);
    mlpproperties(&network, &n1, &n2, &wcount, _state);
    h = 0.0001;
    etol = 0.05;
    
    /*
     * Initialize
     */
    ae_vector_set_length(&x, nin-1+1, _state);
    ae_vector_set_length(&x1, nin-1+1, _state);
    ae_vector_set_length(&x2, nin-1+1, _state);
    ae_vector_set_length(&y, nout-1+1, _state);
    ae_vector_set_length(&y1, nout-1+1, _state);
    ae_vector_set_length(&y2, nout-1+1, _state);
    ae_vector_set_length(&grad1, wcount-1+1, _state);
    ae_vector_set_length(&grad2, wcount-1+1, _state);
    ae_vector_set_length(&grad3, wcount-1+1, _state);
    ae_matrix_set_length(&h1, wcount-1+1, wcount-1+1, _state);
    ae_matrix_set_length(&h2, wcount-1+1, wcount-1+1, _state);
    
    /*
     * Process
     */
    for(pass=1; pass<=passcount; pass++)
    {
        mlprandomizefull(&network, _state);
        
        /*
         * Test hessian calculation .
         * E1 contains total error (calculated using MLPGrad/MLPGradN)
         * Grad1 contains total gradient (calculated using MLPGrad/MLPGradN)
         * H1 contains Hessian calculated using differences of gradients
         *
         * E2, Grad2 and H2 contains corresponing values calculated using MLPHessianBatch/MLPHessianNBatch
         */
        for(hkind=0; hkind<=1; hkind++)
        {
            ssize = 1+ae_randominteger(10, _state);
            ae_matrix_set_length(&xy, ssize-1+1, nin+nout-1+1, _state);
            for(i=0; i<=wcount-1; i++)
            {
                grad1.ptr.p_double[i] = (double)(0);
            }
            for(i=0; i<=wcount-1; i++)
            {
                for(j=0; j<=wcount-1; j++)
                {
                    h1.ptr.pp_double[i][j] = (double)(0);
                }
            }
            e1 = (double)(0);
            for(i=0; i<=ssize-1; i++)
            {
                
                /*
                 * X, Y
                 */
                for(j=0; j<=nin-1; j++)
                {
                    x1.ptr.p_double[j] = 4*ae_randomreal(_state)-2;
                }
                ae_v_move(&xy.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,nin-1));
                if( mlpissoftmax(&network, _state) )
                {
                    for(j=0; j<=nout-1; j++)
                    {
                        y1.ptr.p_double[j] = (double)(0);
                    }
                    xy.ptr.pp_double[i][nin] = (double)(ae_randominteger(nout, _state));
                    y1.ptr.p_double[ae_round(xy.ptr.pp_double[i][nin], _state)] = (double)(1);
                }
                else
                {
                    for(j=0; j<=nout-1; j++)
                    {
                        y1.ptr.p_double[j] = 4*ae_randomreal(_state)-2;
                    }
                    ae_v_move(&xy.ptr.pp_double[i][nin], 1, &y1.ptr.p_double[0], 1, ae_v_len(nin,nin+nout-1));
                }
                
                /*
                 * E1, Grad1
                 */
                if( hkind==0 )
                {
                    mlpgrad(&network, &x1, &y1, &v, &grad2, _state);
                }
                else
                {
                    mlpgradn(&network, &x1, &y1, &v, &grad2, _state);
                }
                e1 = e1+v;
                ae_v_add(&grad1.ptr.p_double[0], 1, &grad2.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
                
                /*
                 * H1
                 */
                for(j=0; j<=wcount-1; j++)
                {
                    wprev = network.weights.ptr.p_double[j];
                    network.weights.ptr.p_double[j] = wprev-2*h;
                    if( hkind==0 )
                    {
                        mlpgrad(&network, &x1, &y1, &v, &grad2, _state);
                    }
                    else
                    {
                        mlpgradn(&network, &x1, &y1, &v, &grad2, _state);
                    }
                    network.weights.ptr.p_double[j] = wprev-h;
                    if( hkind==0 )
                    {
                        mlpgrad(&network, &x1, &y1, &v, &grad3, _state);
                    }
                    else
                    {
                        mlpgradn(&network, &x1, &y1, &v, &grad3, _state);
                    }
                    ae_v_subd(&grad2.ptr.p_double[0], 1, &grad3.ptr.p_double[0], 1, ae_v_len(0,wcount-1), 8);
                    network.weights.ptr.p_double[j] = wprev+h;
                    if( hkind==0 )
                    {
                        mlpgrad(&network, &x1, &y1, &v, &grad3, _state);
                    }
                    else
                    {
                        mlpgradn(&network, &x1, &y1, &v, &grad3, _state);
                    }
                    ae_v_addd(&grad2.ptr.p_double[0], 1, &grad3.ptr.p_double[0], 1, ae_v_len(0,wcount-1), 8);
                    network.weights.ptr.p_double[j] = wprev+2*h;
                    if( hkind==0 )
                    {
                        mlpgrad(&network, &x1, &y1, &v, &grad3, _state);
                    }
                    else
                    {
                        mlpgradn(&network, &x1, &y1, &v, &grad3, _state);
                    }
                    ae_v_sub(&grad2.ptr.p_double[0], 1, &grad3.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
                    v = 1/(12*h);
                    ae_v_addd(&h1.ptr.pp_double[j][0], 1, &grad2.ptr.p_double[0], 1, ae_v_len(0,wcount-1), v);
                    network.weights.ptr.p_double[j] = wprev;
                }
            }
            if( hkind==0 )
            {
                mlphessianbatch(&network, &xy, ssize, &e2, &grad2, &h2, _state);
            }
            else
            {
                mlphessiannbatch(&network, &xy, ssize, &e2, &grad2, &h2, _state);
            }
            *err = *err||ae_fp_greater(ae_fabs(e1-e2, _state)/e1,etol);
            for(i=0; i<=wcount-1; i++)
            {
                if( ae_fp_greater(ae_fabs(grad1.ptr.p_double[i], _state),1.0E-2) )
                {
                    *err = *err||ae_fp_greater(ae_fabs((grad2.ptr.p_double[i]-grad1.ptr.p_double[i])/grad1.ptr.p_double[i], _state),etol);
                }
                else
                {
                    *err = *err||ae_fp_greater(ae_fabs(grad2.ptr.p_double[i]-grad1.ptr.p_double[i], _state),etol);
                }
            }
            for(i=0; i<=wcount-1; i++)
            {
                for(j=0; j<=wcount-1; j++)
                {
                    if( ae_fp_greater(ae_fabs(h1.ptr.pp_double[i][j], _state),5.0E-2) )
                    {
                        *err = *err||ae_fp_greater(ae_fabs((h1.ptr.pp_double[i][j]-h2.ptr.pp_double[i][j])/h1.ptr.pp_double[i][j], _state),etol);
                    }
                    else
                    {
                        *err = *err||ae_fp_greater(ae_fabs(h2.ptr.pp_double[i][j]-h1.ptr.pp_double[i][j], _state),etol);
                    }
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Error functions (other than MLPError and MLPErrorN) test.

Network of type NKind is created, with  NIn  inputs,  NHid1*NHid2   hidden
layers (one layer if NHid2=0), NOut outputs. PassCount  random  passes  is
performed. Dataset has random size in [SizeMin,SizeMax].
*************************************************************************/
static void testmlpbaseunit_testerr(ae_int_t nkind,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t passcount,
     ae_int_t sizemin,
     ae_int_t sizemax,
     ae_bool* err,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron network;
    sparsematrix sparsexy;
    ae_int_t n1;
    ae_int_t n2;
    ae_int_t wcount;
    double etol;
    double escale;
    double a1;
    double a2;
    ae_int_t pass;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t ssize;
    ae_int_t subsetsize;
    ae_matrix xy;
    ae_vector y;
    ae_vector x1;
    ae_vector y1;
    ae_vector idx;
    ae_vector dummy;
    double refrmserror;
    double refclserror;
    double refrelclserror;
    double refavgce;
    double refavgerror;
    double refavgrelerror;
    ae_int_t avgrelcnt;
    modelerrors allerrors;
    ae_int_t nnmax;
    ae_int_t dsmax;
    double relclstolerance;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&network, _state);
    _sparsematrix_init(&sparsexy, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&y1, 0, DT_REAL, _state);
    ae_vector_init(&idx, 0, DT_INT, _state);
    ae_vector_init(&dummy, 0, DT_INT, _state);
    _modelerrors_init(&allerrors, _state);

    a1 = (double)(0);
    a2 = (double)(0);
    if( nkind==2 )
    {
        a1 = 1000*ae_randomreal(_state)-500;
        a2 = 2*ae_randomreal(_state)-1;
    }
    if( nkind==3 )
    {
        a1 = 1000*ae_randomreal(_state)-500;
        a2 = a1+(2*ae_randominteger(2, _state)-1)*(0.1+0.9*ae_randomreal(_state));
    }
    testmlpbaseunit_createnetwork(&network, nkind, a1, a2, nin, nhid1, nhid2, nout, _state);
    mlpproperties(&network, &n1, &n2, &wcount, _state);
    etol = 1.0E-4;
    escale = 1.0E-2;
    
    /*
     * Initialize
     */
    ae_vector_set_length(&x1, nin, _state);
    ae_vector_set_length(&y, nout, _state);
    ae_vector_set_length(&y1, nout, _state);
    
    /*
     * Process
     */
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * Randomize network, then re-randomaze weights manually.
         *
         * NOTE: weights magnitude is chosen to be small, about 0.1,
         *       which allows us to avoid oversaturated network.
         *       In 10% of cases we use zero weights.
         */
        mlprandomizefull(&network, _state);
        if( ae_fp_less_eq(ae_randomreal(_state),0.1) )
        {
            for(i=0; i<=wcount-1; i++)
            {
                network.weights.ptr.p_double[i] = 0.0;
            }
        }
        else
        {
            for(i=0; i<=wcount-1; i++)
            {
                network.weights.ptr.p_double[i] = 0.2*ae_randomreal(_state)-0.1;
            }
        }
        
        /*
         * Generate random dataset.
         * Calculate reference errors.
         *
         * NOTE: about 10% of tests are performed with zero SSize
         */
        ssize = sizemin+ae_randominteger(sizemax-sizemin+1, _state);
        if( mlpissoftmax(&network, _state) )
        {
            ae_matrix_set_length(&xy, ae_maxint(ssize, 1, _state), nin+1, _state);
            sparsecreate(ae_maxint(ssize, 1, _state), nin+1, 0, &sparsexy, _state);
        }
        else
        {
            ae_matrix_set_length(&xy, ae_maxint(ssize, 1, _state), nin+nout, _state);
            sparsecreate(ae_maxint(ssize, 1, _state), nin+nout, 0, &sparsexy, _state);
        }
        refrmserror = 0.0;
        refclserror = 0.0;
        refavgce = 0.0;
        refavgerror = 0.0;
        refavgrelerror = 0.0;
        avgrelcnt = 0;
        for(i=0; i<=ssize-1; i++)
        {
            
            /*
             * Fill I-th row
             */
            for(j=0; j<=nin-1; j++)
            {
                x1.ptr.p_double[j] = 4*ae_randomreal(_state)-2;
                sparseset(&sparsexy, i, j, x1.ptr.p_double[j], _state);
            }
            ae_v_move(&xy.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,nin-1));
            if( mlpissoftmax(&network, _state) )
            {
                for(j=0; j<=nout-1; j++)
                {
                    y1.ptr.p_double[j] = (double)(0);
                }
                xy.ptr.pp_double[i][nin] = (double)(ae_randominteger(nout, _state));
                sparseset(&sparsexy, i, nin, xy.ptr.pp_double[i][nin], _state);
                y1.ptr.p_double[ae_round(xy.ptr.pp_double[i][nin], _state)] = (double)(1);
            }
            else
            {
                for(j=0; j<=nout-1; j++)
                {
                    y1.ptr.p_double[j] = 4*ae_randomreal(_state)-2;
                    sparseset(&sparsexy, i, nin+j, y1.ptr.p_double[j], _state);
                }
                ae_v_move(&xy.ptr.pp_double[i][nin], 1, &y1.ptr.p_double[0], 1, ae_v_len(nin,nin+nout-1));
            }
            
            /*
             * Process
             */
            mlpprocess(&network, &x1, &y, _state);
            
            /*
             * Update reference errors
             */
            nnmax = 0;
            if( mlpissoftmax(&network, _state) )
            {
                if( ae_fp_greater(y.ptr.p_double[ae_round(xy.ptr.pp_double[i][nin], _state)],(double)(0)) )
                {
                    refavgce = refavgce+ae_log(1/y.ptr.p_double[ae_round(xy.ptr.pp_double[i][nin], _state)], _state);
                }
                else
                {
                    refavgce = refavgce+ae_log(ae_maxrealnumber, _state);
                }
            }
            if( mlpissoftmax(&network, _state) )
            {
                dsmax = ae_round(xy.ptr.pp_double[i][nin], _state);
            }
            else
            {
                dsmax = 0;
            }
            for(j=0; j<=nout-1; j++)
            {
                refrmserror = refrmserror+ae_sqr(y.ptr.p_double[j]-y1.ptr.p_double[j], _state);
                refavgerror = refavgerror+ae_fabs(y.ptr.p_double[j]-y1.ptr.p_double[j], _state);
                if( ae_fp_neq(y1.ptr.p_double[j],(double)(0)) )
                {
                    refavgrelerror = refavgrelerror+ae_fabs(y.ptr.p_double[j]-y1.ptr.p_double[j], _state)/ae_fabs(y1.ptr.p_double[j], _state);
                    avgrelcnt = avgrelcnt+1;
                }
                if( ae_fp_greater(y.ptr.p_double[j],y.ptr.p_double[nnmax]) )
                {
                    nnmax = j;
                }
                if( !mlpissoftmax(&network, _state)&&ae_fp_greater(y1.ptr.p_double[j],y1.ptr.p_double[dsmax]) )
                {
                    dsmax = j;
                }
            }
            if( nnmax!=dsmax )
            {
                refclserror = refclserror+1;
            }
        }
        sparseconverttocrs(&sparsexy, _state);
        if( ssize>0 )
        {
            refrmserror = ae_sqrt(refrmserror/(ssize*nout), _state);
            refavgerror = refavgerror/(ssize*nout);
            refrelclserror = refclserror/ssize;
            refavgce = refavgce/(ssize*ae_log((double)(2), _state));
        }
        else
        {
            refrelclserror = 0.0;
        }
        if( avgrelcnt>0 )
        {
            refavgrelerror = refavgrelerror/avgrelcnt;
        }
        
        /*
         * Test "continuous" errors on full dataset
         */
        seterrorflagdiff(err, mlprmserror(&network, &xy, ssize, _state), refrmserror, etol, escale, _state);
        seterrorflagdiff(err, mlpavgce(&network, &xy, ssize, _state), refavgce, etol, escale, _state);
        seterrorflagdiff(err, mlpavgerror(&network, &xy, ssize, _state), refavgerror, etol, escale, _state);
        seterrorflagdiff(err, mlpavgrelerror(&network, &xy, ssize, _state), refavgrelerror, etol, escale, _state);
        seterrorflagdiff(err, mlprmserrorsparse(&network, &sparsexy, ssize, _state), refrmserror, etol, escale, _state);
        seterrorflagdiff(err, mlpavgcesparse(&network, &sparsexy, ssize, _state), refavgce, etol, escale, _state);
        seterrorflagdiff(err, mlpavgerrorsparse(&network, &sparsexy, ssize, _state), refavgerror, etol, escale, _state);
        seterrorflagdiff(err, mlpavgrelerrorsparse(&network, &sparsexy, ssize, _state), refavgrelerror, etol, escale, _state);
        mlpallerrorssubset(&network, &xy, ssize, &dummy, -1, &allerrors, _state);
        seterrorflagdiff(err, allerrors.avgce, refavgce, etol, escale, _state);
        seterrorflagdiff(err, allerrors.rmserror, refrmserror, etol, escale, _state);
        seterrorflagdiff(err, allerrors.avgerror, refavgerror, etol, escale, _state);
        seterrorflagdiff(err, allerrors.avgrelerror, refavgrelerror, etol, escale, _state);
        mlpallerrorssparsesubset(&network, &sparsexy, ssize, &dummy, -1, &allerrors, _state);
        seterrorflagdiff(err, allerrors.avgce, refavgce, etol, escale, _state);
        seterrorflagdiff(err, allerrors.rmserror, refrmserror, etol, escale, _state);
        seterrorflagdiff(err, allerrors.avgerror, refavgerror, etol, escale, _state);
        seterrorflagdiff(err, allerrors.avgrelerror, refavgrelerror, etol, escale, _state);
        
        /*
         * Test errors on dataset given by subset.
         * We perform only limited test for RMS error, assuming that either all errors
         * are calculated correctly (subject to subset given by Idx) - or none of them.
         */
        if( ssize>0 )
        {
            subsetsize = ae_randominteger(10, _state);
        }
        else
        {
            subsetsize = 0;
        }
        ae_vector_set_length(&idx, subsetsize, _state);
        refrmserror = 0.0;
        for(i=0; i<=subsetsize-1; i++)
        {
            k = ae_randominteger(ssize, _state);
            idx.ptr.p_int[i] = k;
            ae_v_move(&x1.ptr.p_double[0], 1, &xy.ptr.pp_double[k][0], 1, ae_v_len(0,nin-1));
            if( mlpissoftmax(&network, _state) )
            {
                for(j=0; j<=nout-1; j++)
                {
                    y1.ptr.p_double[j] = (double)(0);
                }
                y1.ptr.p_double[ae_round(xy.ptr.pp_double[k][nin], _state)] = (double)(1);
            }
            else
            {
                for(j=0; j<=nout-1; j++)
                {
                    y1.ptr.p_double[j] = xy.ptr.pp_double[k][nin+j];
                }
            }
            mlpprocess(&network, &x1, &y, _state);
            for(j=0; j<=nout-1; j++)
            {
                refrmserror = refrmserror+ae_sqr(y.ptr.p_double[j]-y1.ptr.p_double[j], _state);
            }
        }
        if( subsetsize>0 )
        {
            refrmserror = ae_sqrt(refrmserror/(subsetsize*nout), _state);
        }
        mlpallerrorssubset(&network, &xy, ssize, &idx, subsetsize, &allerrors, _state);
        seterrorflagdiff(err, allerrors.rmserror, refrmserror, etol, escale, _state);
        mlpallerrorssparsesubset(&network, &sparsexy, ssize, &idx, subsetsize, &allerrors, _state);
        seterrorflagdiff(err, allerrors.rmserror, refrmserror, etol, escale, _state);
        
        /*
         * Test "discontinuous" error function.
         * Even slight changes in the network output may force these functions
         * to change by 1. So, we test them with relaxed criteria, corresponding to
         * difference in classification of two samples.
         */
        if( ssize>0 )
        {
            relclstolerance = 2.5/ssize;
            seterrorflag(err, ae_fp_greater(ae_fabs(mlpclserror(&network, &xy, ssize, _state)-refclserror, _state),ssize*relclstolerance), _state);
            seterrorflag(err, ae_fp_greater(ae_fabs(mlprelclserror(&network, &xy, ssize, _state)-refrelclserror, _state),relclstolerance), _state);
            seterrorflag(err, ae_fp_greater(ae_fabs(mlprelclserrorsparse(&network, &sparsexy, ssize, _state)-refrelclserror, _state),relclstolerance), _state);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Special tests
*************************************************************************/
static void testmlpbaseunit_spectests(ae_bool* inferrors,
     ae_bool* procerrors,
     ae_bool* graderrors,
     ae_bool* hesserrors,
     ae_bool* errerrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;
    ae_matrix xy;
    double f;
    ae_vector g;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&net, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_vector_init(&g, 0, DT_REAL, _state);

    
    /*
     * Special test for overflow in TanH:
     * * create 1x1x1 linear network
     * * create dataset with 1 item: [x, y] = [0, 1]
     * * set network weights to [10000000, 10000000, 10000000, 10000000]
     * * check that error function is finite
     * * check that gradient is finite
     */
    mlpcreate1(1, 1, 1, &net, _state);
    ae_matrix_set_length(&xy, 1, 2, _state);
    xy.ptr.pp_double[0][0] = (double)(0);
    xy.ptr.pp_double[0][1] = 1.0;
    for(i=0; i<=mlpgetweightscount(&net, _state)-1; i++)
    {
        net.weights.ptr.p_double[i] = 10000000.0;
    }
    mlpgradbatch(&net, &xy, 1, &f, &g, _state);
    seterrorflag(graderrors, !ae_isfinite(f, _state), _state);
    seterrorflag(graderrors, !ae_isfinite(mlperror(&net, &xy, 1, _state), _state), _state);
    for(i=0; i<=mlpgetweightscount(&net, _state)-1; i++)
    {
        seterrorflag(graderrors, !ae_isfinite(g.ptr.p_double[i], _state), _state);
    }
    
    /*
     * Special test for overflow in SOFTMAX layer:
     * * create 1x1x2 classifier network
     * * create dataset with 1 item: [x, y] = [0, 1]
     * * set network weights to [10000000, 10000000, 10000000, 10000000]
     * * check that error function is finite
     * * check that gradient is finite
     */
    mlpcreatec1(1, 1, 2, &net, _state);
    ae_matrix_set_length(&xy, 1, 2, _state);
    xy.ptr.pp_double[0][0] = (double)(0);
    xy.ptr.pp_double[0][1] = (double)(1);
    for(i=0; i<=mlpgetweightscount(&net, _state)-1; i++)
    {
        net.weights.ptr.p_double[i] = 10000000.0;
    }
    mlpgradbatch(&net, &xy, 1, &f, &g, _state);
    seterrorflag(graderrors, !ae_isfinite(f, _state), _state);
    seterrorflag(graderrors, !ae_isfinite(mlperror(&net, &xy, 1, _state), _state), _state);
    for(i=0; i<=mlpgetweightscount(&net, _state)-1; i++)
    {
        seterrorflag(graderrors, !ae_isfinite(g.ptr.p_double[i], _state), _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
The function test functions MLPGradBatchMasked and MLPGradBatchSparseMasked.
*************************************************************************/
static ae_bool testmlpbaseunit_testmlpgbsubset(ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;
    ae_matrix a;
    ae_matrix parta;
    sparsematrix sa;
    sparsematrix partsa;
    ae_vector idx;
    double e1;
    double e2;
    ae_vector grad1;
    ae_vector grad2;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t w;
    ae_int_t wcount;
    ae_int_t nhid1;
    ae_int_t nhid2;
    ae_int_t nkind;
    double a1;
    double a2;
    ae_int_t n1;
    ae_int_t n2;
    ae_int_t ssize;
    ae_int_t maxssize;
    ae_int_t sbsize;
    ae_int_t nvar;
    ae_int_t variant;
    ae_int_t i;
    ae_int_t j;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&net, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&parta, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&sa, _state);
    _sparsematrix_init(&partsa, _state);
    ae_vector_init(&idx, 0, DT_INT, _state);
    ae_vector_init(&grad1, 0, DT_REAL, _state);
    ae_vector_init(&grad2, 0, DT_REAL, _state);

    
    /*
     * Variant:
     *  * 1 - there are all rows;
     *  * 2 - there are no one rows;
     *  * 3 - there are some random rows.
     */
    nvar = 3;
    maxssize = 96;
    for(ssize=0; ssize<=maxssize; ssize++)
    {
        ae_vector_set_length(&idx, ssize, _state);
        nkind = ae_randominteger(4, _state);
        a1 = (double)(0);
        a2 = (double)(0);
        if( nkind==2 )
        {
            a1 = 1000*ae_randomreal(_state)-500;
            a2 = 2*ae_randomreal(_state)-1;
        }
        if( nkind==3 )
        {
            a1 = 1000*ae_randomreal(_state)-500;
            a2 = a1+(2*ae_randominteger(2, _state)-1)*(0.1+0.9*ae_randomreal(_state));
        }
        nin = ae_randominteger(20, _state)+1;
        nhid1 = ae_randominteger(5, _state);
        if( nhid1==0 )
        {
            nhid2 = 0;
        }
        else
        {
            nhid2 = ae_randominteger(5, _state);
        }
        nout = ae_randominteger(20, _state)+2;
        testmlpbaseunit_createnetwork(&net, nkind, a1, a2, nin, nhid1, nhid2, nout, _state);
        mlpproperties(&net, &n1, &n2, &wcount, _state);
        if( mlpissoftmax(&net, _state) )
        {
            w = nin+1;
            if( ssize>0 )
            {
                ae_matrix_set_length(&a, ssize, w, _state);
                sparsecreate(ssize, w, ssize*w, &sa, _state);
            }
            else
            {
                ae_matrix_set_length(&a, 0, 0, _state);
                sparsecreate(1, 1, 0, &sa, _state);
            }
            for(i=0; i<=ssize-1; i++)
            {
                for(j=0; j<=w-1; j++)
                {
                    a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                    sparseset(&sa, i, j, a.ptr.pp_double[i][j], _state);
                }
            }
            for(i=0; i<=ssize-1; i++)
            {
                a.ptr.pp_double[i][nin] = (double)(ae_randominteger(nout, _state));
                sparseset(&sa, i, nin, a.ptr.pp_double[i][nin], _state);
            }
        }
        else
        {
            w = nin+nout;
            if( ssize>0 )
            {
                ae_matrix_set_length(&a, ssize, w, _state);
                sparsecreate(ssize, w, ssize*w, &sa, _state);
            }
            else
            {
                ae_matrix_set_length(&a, 0, 0, _state);
                sparsecreate(1, 1, 0, &sa, _state);
            }
            for(i=0; i<=ssize-1; i++)
            {
                for(j=0; j<=w-1; j++)
                {
                    a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                    sparseset(&sa, i, j, a.ptr.pp_double[i][j], _state);
                }
            }
        }
        sparseconverttocrs(&sa, _state);
        for(variant=1; variant<=nvar; variant++)
        {
            sbsize = -1;
            if( variant==1 )
            {
                sbsize = ssize;
                for(i=0; i<=sbsize-1; i++)
                {
                    idx.ptr.p_int[i] = i;
                }
            }
            if( variant==2 )
            {
                sbsize = 0;
            }
            if( variant==3 )
            {
                if( ssize==0 )
                {
                    sbsize = 0;
                }
                else
                {
                    sbsize = ae_randominteger(ssize, _state);
                }
                for(i=0; i<=sbsize-1; i++)
                {
                    idx.ptr.p_int[i] = ae_randominteger(ssize, _state);
                }
            }
            ae_assert(sbsize>=0, "mlpbase test: integrity check failed", _state);
            if( sbsize!=0 )
            {
                ae_matrix_set_length(&parta, sbsize, w, _state);
                sparsecreate(sbsize, w, sbsize*w, &partsa, _state);
            }
            else
            {
                ae_matrix_set_length(&parta, 0, 0, _state);
                sparsecreate(1, 1, 0, &partsa, _state);
            }
            for(i=0; i<=sbsize-1; i++)
            {
                ae_v_move(&parta.ptr.pp_double[i][0], 1, &a.ptr.pp_double[idx.ptr.p_int[i]][0], 1, ae_v_len(0,w-1));
                for(j=0; j<=w-1; j++)
                {
                    sparseset(&partsa, i, j, parta.ptr.pp_double[i][j], _state);
                }
            }
            sparseconverttocrs(&partsa, _state);
            mlpgradbatch(&net, &parta, sbsize, &e1, &grad1, _state);
            mlpgradbatchsubset(&net, &a, ssize, &idx, sbsize, &e2, &grad2, _state);
            
            /*
             * Test for dense matrix
             */
            if( ae_fp_greater(ae_fabs(e1-e2, _state),1.0E-6) )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            for(i=0; i<=wcount-1; i++)
            {
                if( ae_fp_greater(ae_fabs(grad1.ptr.p_double[i]-grad2.ptr.p_double[i], _state),1.0E-6) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
            
            /*
             * Test for sparse matrix
             */
            mlpgradbatchsparse(&net, &partsa, sbsize, &e1, &grad1, _state);
            mlpgradbatchsparsesubset(&net, &sa, ssize, &idx, sbsize, &e2, &grad2, _state);
            if( ae_fp_greater(ae_fabs(e1-e2, _state),1.0E-6) )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            for(i=0; i<=wcount-1; i++)
            {
                if( ae_fp_greater(ae_fabs(grad1.ptr.p_double[i]-grad2.ptr.p_double[i], _state),1.0E-6) )
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


/*$ End $*/
