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
#include "testmlpeunit.h"


/*$ Declarations $*/
static void testmlpeunit_createensemble(mlpensemble* ensemble,
     ae_int_t nkind,
     double a1,
     double a2,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t ec,
     ae_state *_state);
static void testmlpeunit_unsetensemble(mlpensemble* ensemble,
     ae_state *_state);
static void testmlpeunit_testinformational(ae_int_t nkind,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t ec,
     ae_int_t passcount,
     ae_bool* err,
     ae_state *_state);
static void testmlpeunit_testprocessing(ae_int_t nkind,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t ec,
     ae_int_t passcount,
     ae_bool* err,
     ae_state *_state);
static void testmlpeunit_testerr(ae_int_t nkind,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t ec,
     ae_int_t passcount,
     ae_int_t sizemin,
     ae_int_t sizemax,
     ae_bool* err,
     ae_state *_state);


/*$ Body $*/


ae_bool testmlpe(ae_bool silent, ae_state *_state)
{
    ae_bool waserrors;
    ae_bool inferrors;
    ae_bool procerrors;
    ae_bool errerrors;
    ae_int_t passcount;
    ae_int_t maxn;
    ae_int_t maxhid;
    ae_int_t nf;
    ae_int_t nl;
    ae_int_t nhid1;
    ae_int_t nhid2;
    ae_int_t ec;
    ae_int_t nkind;
    ae_int_t sizemin;
    ae_int_t sizemax;
    ae_bool result;


    waserrors = ae_false;
    inferrors = ae_false;
    procerrors = ae_false;
    errerrors = ae_false;
    passcount = 5;
    maxn = 3;
    maxhid = 3;
    
    /*
     * General MLP ensembles tests
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
                        for(ec=1; ec<=3; ec++)
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
                            testmlpeunit_testinformational(nkind, nf, nhid1, nhid2, nl, ec, passcount, &inferrors, _state);
                            testmlpeunit_testprocessing(nkind, nf, nhid1, nhid2, nl, ec, passcount, &procerrors, _state);
                            testmlpeunit_testerr(nkind, nf, nhid1, nhid2, nl, ec, passcount, sizemin, sizemax, &errerrors, _state);
                        }
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
    nhid1 = 10;
    nhid2 = 10;
    nl = 2;
    ec = 10;
    sizemin = 1000;
    sizemax = 1000;
    testmlpeunit_testerr(0, nf, nhid1, nhid2, nl, ec, 1, sizemin, sizemax, &errerrors, _state);
    
    /*
     * Final report
     */
    waserrors = (inferrors||procerrors)||errerrors;
    if( !silent )
    {
        printf("MLP ENSEMBLE TEST\n");
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
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testmlpe(ae_bool silent, ae_state *_state)
{
    return testmlpe(silent, _state);
}


/*************************************************************************
Network creation
*************************************************************************/
static void testmlpeunit_createensemble(mlpensemble* ensemble,
     ae_int_t nkind,
     double a1,
     double a2,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t ec,
     ae_state *_state)
{


    ae_assert(((nin>0&&nhid1>=0)&&nhid2>=0)&&nout>0, "CreateNetwork error", _state);
    ae_assert(nhid1!=0||nhid2==0, "CreateNetwork error", _state);
    ae_assert(nkind!=1||nout>=2, "CreateNetwork error", _state);
    if( nhid1==0 )
    {
        
        /*
         * No hidden layers
         */
        if( nkind==0 )
        {
            mlpecreate0(nin, nout, ec, ensemble, _state);
        }
        else
        {
            if( nkind==1 )
            {
                mlpecreatec0(nin, nout, ec, ensemble, _state);
            }
            else
            {
                if( nkind==2 )
                {
                    mlpecreateb0(nin, nout, a1, a2, ec, ensemble, _state);
                }
                else
                {
                    if( nkind==3 )
                    {
                        mlpecreater0(nin, nout, a1, a2, ec, ensemble, _state);
                    }
                }
            }
        }
        return;
    }
    if( nhid2==0 )
    {
        
        /*
         * One hidden layer
         */
        if( nkind==0 )
        {
            mlpecreate1(nin, nhid1, nout, ec, ensemble, _state);
        }
        else
        {
            if( nkind==1 )
            {
                mlpecreatec1(nin, nhid1, nout, ec, ensemble, _state);
            }
            else
            {
                if( nkind==2 )
                {
                    mlpecreateb1(nin, nhid1, nout, a1, a2, ec, ensemble, _state);
                }
                else
                {
                    if( nkind==3 )
                    {
                        mlpecreater1(nin, nhid1, nout, a1, a2, ec, ensemble, _state);
                    }
                }
            }
        }
        return;
    }
    
    /*
     * Two hidden layers
     */
    if( nkind==0 )
    {
        mlpecreate2(nin, nhid1, nhid2, nout, ec, ensemble, _state);
    }
    else
    {
        if( nkind==1 )
        {
            mlpecreatec2(nin, nhid1, nhid2, nout, ec, ensemble, _state);
        }
        else
        {
            if( nkind==2 )
            {
                mlpecreateb2(nin, nhid1, nhid2, nout, a1, a2, ec, ensemble, _state);
            }
            else
            {
                if( nkind==3 )
                {
                    mlpecreater2(nin, nhid1, nhid2, nout, a1, a2, ec, ensemble, _state);
                }
            }
        }
    }
}


/*************************************************************************
Unsets network (initialize it to smallest network possible
*************************************************************************/
static void testmlpeunit_unsetensemble(mlpensemble* ensemble,
     ae_state *_state)
{


    mlpecreate0(1, 1, 1, ensemble, _state);
}


/*************************************************************************
Iformational functions test
*************************************************************************/
static void testmlpeunit_testinformational(ae_int_t nkind,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t ec,
     ae_int_t passcount,
     ae_bool* err,
     ae_state *_state)
{
    ae_frame _frame_block;
    mlpensemble ensemble;
    ae_int_t n1;
    ae_int_t n2;

    ae_frame_make(_state, &_frame_block);
    _mlpensemble_init(&ensemble, _state);

    testmlpeunit_createensemble(&ensemble, nkind, -1.0, 1.0, nin, nhid1, nhid2, nout, ec, _state);
    mlpeproperties(&ensemble, &n1, &n2, _state);
    *err = (*err||n1!=nin)||n2!=nout;
    ae_frame_leave(_state);
}


/*************************************************************************
Processing functions test
*************************************************************************/
static void testmlpeunit_testprocessing(ae_int_t nkind,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t ec,
     ae_int_t passcount,
     ae_bool* err,
     ae_state *_state)
{
    ae_frame _frame_block;
    mlpensemble ensemble;
    mlpensemble ensemble2;
    double a1;
    double a2;
    ae_int_t pass;
    ae_int_t rkind;
    ae_int_t i;
    ae_bool allsame;
    ae_vector x1;
    ae_vector x2;
    ae_vector y1;
    ae_vector y2;
    ae_vector ra;
    ae_vector ra2;
    double v;

    ae_frame_make(_state, &_frame_block);
    _mlpensemble_init(&ensemble, _state);
    _mlpensemble_init(&ensemble2, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&x2, 0, DT_REAL, _state);
    ae_vector_init(&y1, 0, DT_REAL, _state);
    ae_vector_init(&y2, 0, DT_REAL, _state);
    ae_vector_init(&ra, 0, DT_REAL, _state);
    ae_vector_init(&ra2, 0, DT_REAL, _state);

    
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
    
    /*
     * Initialize arrays
     */
    ae_vector_set_length(&x1, nin-1+1, _state);
    ae_vector_set_length(&x2, nin-1+1, _state);
    ae_vector_set_length(&y1, nout-1+1, _state);
    ae_vector_set_length(&y2, nout-1+1, _state);
    
    /*
     * Main cycle:
     * * Pass is a number of repeated test
     * * RKind is a "replication kind":
     *   * RKind=0 means that we work with original ensemble
     *   * RKind=1 means that we work with replica created with MLPECopy()
     *   * RKind=2 means that we work with replica created with serialization/unserialization
     */
    for(pass=1; pass<=passcount; pass++)
    {
        for(rkind=0; rkind<=2; rkind++)
        {
            
            /*
             * Create network, pass through replication in order to test that replicated network works correctly.
             */
            testmlpeunit_createensemble(&ensemble, nkind, a1, a2, nin, nhid1, nhid2, nout, ec, _state);
            if( rkind==1 )
            {
                mlpecopy(&ensemble, &ensemble2, _state);
                testmlpeunit_unsetensemble(&ensemble, _state);
                mlpecopy(&ensemble2, &ensemble, _state);
                testmlpeunit_unsetensemble(&ensemble2, _state);
            }
            if( rkind==2 )
            {
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
                    mlpealloc(&_local_serializer, &ensemble, _state);
                    _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                    ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                    ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                    mlpeserialize(&_local_serializer, &ensemble, _state);
                    ae_serializer_stop(&_local_serializer);
                    ae_serializer_clear(&_local_serializer);
                    
                    ae_serializer_init(&_local_serializer);
                    ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                    mlpeunserialize(&_local_serializer, &ensemble2, _state);
                    ae_serializer_stop(&_local_serializer);
                    ae_serializer_clear(&_local_serializer);
                    
                    ae_frame_leave(_state);
                }
                testmlpeunit_unsetensemble(&ensemble, _state);
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
                    mlpealloc(&_local_serializer, &ensemble2, _state);
                    _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                    ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                    ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                    mlpeserialize(&_local_serializer, &ensemble2, _state);
                    ae_serializer_stop(&_local_serializer);
                    ae_serializer_clear(&_local_serializer);
                    
                    ae_serializer_init(&_local_serializer);
                    ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                    mlpeunserialize(&_local_serializer, &ensemble, _state);
                    ae_serializer_stop(&_local_serializer);
                    ae_serializer_clear(&_local_serializer);
                    
                    ae_frame_leave(_state);
                }
                testmlpeunit_unsetensemble(&ensemble2, _state);
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
            mlpeprocess(&ensemble, &x1, &y1, _state);
            mlpeprocess(&ensemble, &x2, &y2, _state);
            allsame = ae_true;
            for(i=0; i<=nout-1; i++)
            {
                allsame = allsame&&ae_fp_eq(y1.ptr.p_double[i],y2.ptr.p_double[i]);
            }
            *err = *err||!allsame;
            
            /*
             * Same inputs on original network leads to same outputs
             * on copy created using MLPCopy
             */
            testmlpeunit_unsetensemble(&ensemble2, _state);
            mlpecopy(&ensemble, &ensemble2, _state);
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
            mlpeprocess(&ensemble, &x1, &y1, _state);
            mlpeprocess(&ensemble2, &x2, &y2, _state);
            allsame = ae_true;
            for(i=0; i<=nout-1; i++)
            {
                allsame = allsame&&ae_fp_eq(y1.ptr.p_double[i],y2.ptr.p_double[i]);
            }
            *err = *err||!allsame;
            
            /*
             * Same inputs on original network leads to same outputs
             * on copy created using MLPSerialize
             */
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
                mlpealloc(&_local_serializer, &ensemble, _state);
                _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                mlpeserialize(&_local_serializer, &ensemble, _state);
                ae_serializer_stop(&_local_serializer);
                ae_serializer_clear(&_local_serializer);
                
                ae_serializer_init(&_local_serializer);
                ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                mlpeunserialize(&_local_serializer, &ensemble2, _state);
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
            mlpeprocess(&ensemble, &x1, &y1, _state);
            mlpeprocess(&ensemble2, &x2, &y2, _state);
            allsame = ae_true;
            for(i=0; i<=nout-1; i++)
            {
                allsame = allsame&&ae_fp_eq(y1.ptr.p_double[i],y2.ptr.p_double[i]);
            }
            *err = *err||!allsame;
            
            /*
             * Different inputs leads to different outputs (non-zero network)
             */
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
            mlpeprocess(&ensemble, &x1, &y1, _state);
            mlpeprocess(&ensemble, &x2, &y2, _state);
            allsame = ae_true;
            for(i=0; i<=nout-1; i++)
            {
                allsame = allsame&&ae_fp_eq(y1.ptr.p_double[i],y2.ptr.p_double[i]);
            }
            *err = *err||allsame;
            
            /*
             * Randomization changes outputs (when inputs are unchanged, non-zero network)
             */
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
            mlpecopy(&ensemble, &ensemble2, _state);
            mlperandomize(&ensemble2, _state);
            mlpeprocess(&ensemble, &x1, &y1, _state);
            mlpeprocess(&ensemble2, &x1, &y2, _state);
            allsame = ae_true;
            for(i=0; i<=nout-1; i++)
            {
                allsame = allsame&&ae_fp_eq(y1.ptr.p_double[i],y2.ptr.p_double[i]);
            }
            *err = *err||allsame;
            
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
                mlpeprocess(&ensemble, &x1, &y1, _state);
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
                mlpeprocess(&ensemble, &x1, &y1, _state);
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
                mlpeprocess(&ensemble, &x1, &y1, _state);
                for(i=0; i<=nout-1; i++)
                {
                    *err = (*err||ae_fp_less(y1.ptr.p_double[i],ae_minreal(a1, a2, _state)))||ae_fp_greater(y1.ptr.p_double[i],ae_maxreal(a1, a2, _state));
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Error functions

Ensemble of type NKind is created, with  NIn  inputs,  NHid1*NHid2  hidden
layers (one layer if NHid2=0), NOut outputs. PassCount  random  passes  is
performed. Dataset has random size in [SizeMin,SizeMax].
*************************************************************************/
static void testmlpeunit_testerr(ae_int_t nkind,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t ec,
     ae_int_t passcount,
     ae_int_t sizemin,
     ae_int_t sizemax,
     ae_bool* err,
     ae_state *_state)
{
    ae_frame _frame_block;
    mlpensemble ensemble;
    sparsematrix sparsexy;
    sparsematrix sparsexy2;
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
    ae_int_t ssize;
    ae_matrix xy;
    ae_matrix xy2;
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

    ae_frame_make(_state, &_frame_block);
    _mlpensemble_init(&ensemble, _state);
    _sparsematrix_init(&sparsexy, _state);
    _sparsematrix_init(&sparsexy2, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xy2, 0, 0, DT_REAL, _state);
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
    testmlpeunit_createensemble(&ensemble, nkind, a1, a2, nin, nhid1, nhid2, nout, ec, _state);
    mlpproperties(&ensemble.network, &n1, &n2, &wcount, _state);
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
         * Randomize Ensemble, then re-randomaze weights manually.
         *
         * NOTE: weights magnitude is chosen to be small, about 0.1,
         *       which allows us to avoid oversaturated Ensemble.
         *       In 10% of cases we use zero weights.
         */
        mlperandomize(&ensemble, _state);
        if( ae_fp_less_eq(ae_randomreal(_state),0.1) )
        {
            for(i=0; i<=wcount*ec-1; i++)
            {
                ensemble.weights.ptr.p_double[i] = 0.0;
            }
        }
        else
        {
            for(i=0; i<=wcount*ec-1; i++)
            {
                ensemble.weights.ptr.p_double[i] = 0.2*ae_randomreal(_state)-0.1;
            }
        }
        
        /*
         * Generate random dataset.
         * Calculate reference errors.
         *
         * NOTE: about 10% of tests are performed with zero SSize
         */
        ssize = sizemin+ae_randominteger(sizemax-sizemin+1, _state);
        if( mlpeissoftmax(&ensemble, _state) )
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
            if( mlpeissoftmax(&ensemble, _state) )
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
            mlpeprocess(&ensemble, &x1, &y, _state);
            
            /*
             * Update reference errors
             */
            nnmax = 0;
            if( mlpeissoftmax(&ensemble, _state) )
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
            if( mlpeissoftmax(&ensemble, _state) )
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
                if( !mlpeissoftmax(&ensemble, _state)&&ae_fp_greater(y1.ptr.p_double[j],y1.ptr.p_double[dsmax]) )
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
        seterrorflagdiff(err, mlpermserror(&ensemble, &xy, ssize, _state), refrmserror, etol, escale, _state);
        seterrorflagdiff(err, mlpeavgce(&ensemble, &xy, ssize, _state), refavgce, etol, escale, _state);
        seterrorflagdiff(err, mlpeavgerror(&ensemble, &xy, ssize, _state), refavgerror, etol, escale, _state);
        seterrorflagdiff(err, mlpeavgrelerror(&ensemble, &xy, ssize, _state), refavgrelerror, etol, escale, _state);
    }
    ae_frame_leave(_state);
}


/*$ End $*/
