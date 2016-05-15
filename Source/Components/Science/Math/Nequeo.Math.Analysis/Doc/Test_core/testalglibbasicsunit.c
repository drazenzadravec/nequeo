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
#include "testalglibbasicsunit.h"


/*$ Declarations $*/
static ae_bool testalglibbasicsunit_testcomplexarithmetics(ae_bool silent,
     ae_state *_state);
static ae_bool testalglibbasicsunit_testieeespecial(ae_bool silent,
     ae_state *_state);
static ae_bool testalglibbasicsunit_testswapfunctions(ae_bool silent,
     ae_state *_state);
static ae_bool testalglibbasicsunit_teststandardfunctions(ae_bool silent,
     ae_state *_state);
static ae_bool testalglibbasicsunit_testserializationfunctions(ae_bool silent,
     ae_state *_state);
static void testalglibbasicsunit_createpoolandrecords(poolrec2* seedrec2,
     poolrec2* seedrec2copy,
     ae_shared_pool* pool,
     ae_state *_state);
static ae_bool testalglibbasicsunit_sharedpoolerrors(ae_state *_state);
static ae_bool testalglibbasicsunit_testsharedpool(ae_bool silent,
     ae_state *_state);
static void testalglibbasicsunit_testsort0func(/* Integer */ ae_vector* a,
     /* Integer */ ae_vector* buf,
     ae_int_t idx0,
     ae_int_t idx2,
     ae_state *_state);
static ae_bool testalglibbasicsunit_performtestsort0(ae_state *_state);
static void testalglibbasicsunit_testsort1func(/* Integer */ ae_vector* a,
     /* Integer */ ae_vector* buf,
     ae_int_t idx0,
     ae_int_t idx2,
     ae_bool usesmp,
     ae_state *_state);
static ae_bool testalglibbasicsunit_performtestsort1(ae_state *_state);
static void testalglibbasicsunit_testsort2func(/* Integer */ ae_vector* a,
     /* Integer */ ae_vector* buf,
     ae_int_t idx0,
     ae_int_t idx2,
     ae_state *_state);
static ae_bool testalglibbasicsunit_performtestsort2(ae_state *_state);
static ae_bool testalglibbasicsunit_performtestpoolsum(ae_state *_state);
static void testalglibbasicsunit_parallelpoolsum(ae_shared_pool* sumpool,
     ae_int_t ind0,
     ae_int_t ind1,
     ae_state *_state);
static void testalglibbasicsunit_mergesortedarrays(/* Integer */ ae_vector* a,
     /* Integer */ ae_vector* buf,
     ae_int_t idx0,
     ae_int_t idx1,
     ae_int_t idx2,
     ae_state *_state);
static ae_bool testalglibbasicsunit_testsmp(ae_bool silent,
     ae_state *_state);


/*$ Body $*/


void rec4serializationalloc(ae_serializer* s,
     rec4serialization* v,
     ae_state *_state)
{
    ae_int_t i;


    
    /*
     * boolean fields
     */
    ae_serializer_alloc_entry(s);
    for(i=0; i<=v->b.cnt-1; i++)
    {
        ae_serializer_alloc_entry(s);
    }
    
    /*
     * integer fields
     */
    ae_serializer_alloc_entry(s);
    for(i=0; i<=v->i.cnt-1; i++)
    {
        ae_serializer_alloc_entry(s);
    }
    
    /*
     * real fields
     */
    ae_serializer_alloc_entry(s);
    for(i=0; i<=v->r.cnt-1; i++)
    {
        ae_serializer_alloc_entry(s);
    }
}


void rec4serializationserialize(ae_serializer* s,
     rec4serialization* v,
     ae_state *_state)
{
    ae_int_t i;


    
    /*
     * boolean fields
     */
    ae_serializer_serialize_int(s, v->b.cnt, _state);
    for(i=0; i<=v->b.cnt-1; i++)
    {
        ae_serializer_serialize_bool(s, v->b.ptr.p_bool[i], _state);
    }
    
    /*
     * integer fields
     */
    ae_serializer_serialize_int(s, v->i.cnt, _state);
    for(i=0; i<=v->i.cnt-1; i++)
    {
        ae_serializer_serialize_int(s, v->i.ptr.p_int[i], _state);
    }
    
    /*
     * real fields
     */
    ae_serializer_serialize_int(s, v->r.cnt, _state);
    for(i=0; i<=v->r.cnt-1; i++)
    {
        ae_serializer_serialize_double(s, v->r.ptr.p_double[i], _state);
    }
}


void rec4serializationunserialize(ae_serializer* s,
     rec4serialization* v,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;
    ae_bool bv;
    ae_int_t iv;
    double rv;

    _rec4serialization_clear(v);

    
    /*
     * boolean fields
     */
    ae_serializer_unserialize_int(s, &k, _state);
    if( k>0 )
    {
        ae_vector_set_length(&v->b, k, _state);
        for(i=0; i<=k-1; i++)
        {
            ae_serializer_unserialize_bool(s, &bv, _state);
            v->b.ptr.p_bool[i] = bv;
        }
    }
    
    /*
     * integer fields
     */
    ae_serializer_unserialize_int(s, &k, _state);
    if( k>0 )
    {
        ae_vector_set_length(&v->i, k, _state);
        for(i=0; i<=k-1; i++)
        {
            ae_serializer_unserialize_int(s, &iv, _state);
            v->i.ptr.p_int[i] = iv;
        }
    }
    
    /*
     * real fields
     */
    ae_serializer_unserialize_int(s, &k, _state);
    if( k>0 )
    {
        ae_vector_set_length(&v->r, k, _state);
        for(i=0; i<=k-1; i++)
        {
            ae_serializer_unserialize_double(s, &rv, _state);
            v->r.ptr.p_double[i] = rv;
        }
    }
}


ae_bool testalglibbasics(ae_bool silent, ae_state *_state)
{
    ae_bool result;


    result = ae_true;
    result = result&&testalglibbasicsunit_testcomplexarithmetics(silent, _state);
    result = result&&testalglibbasicsunit_testieeespecial(silent, _state);
    result = result&&testalglibbasicsunit_testswapfunctions(silent, _state);
    result = result&&testalglibbasicsunit_teststandardfunctions(silent, _state);
    result = result&&testalglibbasicsunit_testserializationfunctions(silent, _state);
    result = result&&testalglibbasicsunit_testsharedpool(silent, _state);
    result = result&&testalglibbasicsunit_testsmp(silent, _state);
    if( !silent )
    {
        printf("\n\n");
    }
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testalglibbasics(ae_bool silent, ae_state *_state)
{
    return testalglibbasics(silent, _state);
}


/*************************************************************************
Complex arithmetics test
*************************************************************************/
static ae_bool testalglibbasicsunit_testcomplexarithmetics(ae_bool silent,
     ae_state *_state)
{
    ae_bool absc;
    ae_bool addcc;
    ae_bool addcr;
    ae_bool addrc;
    ae_bool subcc;
    ae_bool subcr;
    ae_bool subrc;
    ae_bool mulcc;
    ae_bool mulcr;
    ae_bool mulrc;
    ae_bool divcc;
    ae_bool divcr;
    ae_bool divrc;
    ae_complex ca;
    ae_complex cb;
    ae_complex res;
    double ra;
    double rb;
    double threshold;
    ae_int_t pass;
    ae_int_t passcount;
    ae_bool result;


    threshold = 100*ae_machineepsilon;
    passcount = 1000;
    result = ae_true;
    absc = ae_true;
    addcc = ae_true;
    addcr = ae_true;
    addrc = ae_true;
    subcc = ae_true;
    subcr = ae_true;
    subrc = ae_true;
    mulcc = ae_true;
    mulcr = ae_true;
    mulrc = ae_true;
    divcc = ae_true;
    divcr = ae_true;
    divrc = ae_true;
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * Test AbsC
         */
        ca.x = 2*ae_randomreal(_state)-1;
        ca.y = 2*ae_randomreal(_state)-1;
        ra = ae_c_abs(ca, _state);
        absc = absc&&ae_fp_less(ae_fabs(ra-ae_sqrt(ae_sqr(ca.x, _state)+ae_sqr(ca.y, _state), _state), _state),threshold);
        
        /*
         * test Add
         */
        ca.x = 2*ae_randomreal(_state)-1;
        ca.y = 2*ae_randomreal(_state)-1;
        cb.x = 2*ae_randomreal(_state)-1;
        cb.y = 2*ae_randomreal(_state)-1;
        ra = 2*ae_randomreal(_state)-1;
        rb = 2*ae_randomreal(_state)-1;
        res = ae_c_add(ca,cb);
        addcc = (addcc&&ae_fp_less(ae_fabs(res.x-ca.x-cb.x, _state),threshold))&&ae_fp_less(ae_fabs(res.y-ca.y-cb.y, _state),threshold);
        res = ae_c_add_d(ca,rb);
        addcr = (addcr&&ae_fp_less(ae_fabs(res.x-ca.x-rb, _state),threshold))&&ae_fp_less(ae_fabs(res.y-ca.y, _state),threshold);
        res = ae_c_add_d(cb,ra);
        addrc = (addrc&&ae_fp_less(ae_fabs(res.x-ra-cb.x, _state),threshold))&&ae_fp_less(ae_fabs(res.y-cb.y, _state),threshold);
        
        /*
         * test Sub
         */
        ca.x = 2*ae_randomreal(_state)-1;
        ca.y = 2*ae_randomreal(_state)-1;
        cb.x = 2*ae_randomreal(_state)-1;
        cb.y = 2*ae_randomreal(_state)-1;
        ra = 2*ae_randomreal(_state)-1;
        rb = 2*ae_randomreal(_state)-1;
        res = ae_c_sub(ca,cb);
        subcc = (subcc&&ae_fp_less(ae_fabs(res.x-(ca.x-cb.x), _state),threshold))&&ae_fp_less(ae_fabs(res.y-(ca.y-cb.y), _state),threshold);
        res = ae_c_sub_d(ca,rb);
        subcr = (subcr&&ae_fp_less(ae_fabs(res.x-(ca.x-rb), _state),threshold))&&ae_fp_less(ae_fabs(res.y-ca.y, _state),threshold);
        res = ae_c_d_sub(ra,cb);
        subrc = (subrc&&ae_fp_less(ae_fabs(res.x-(ra-cb.x), _state),threshold))&&ae_fp_less(ae_fabs(res.y+cb.y, _state),threshold);
        
        /*
         * test Mul
         */
        ca.x = 2*ae_randomreal(_state)-1;
        ca.y = 2*ae_randomreal(_state)-1;
        cb.x = 2*ae_randomreal(_state)-1;
        cb.y = 2*ae_randomreal(_state)-1;
        ra = 2*ae_randomreal(_state)-1;
        rb = 2*ae_randomreal(_state)-1;
        res = ae_c_mul(ca,cb);
        mulcc = (mulcc&&ae_fp_less(ae_fabs(res.x-(ca.x*cb.x-ca.y*cb.y), _state),threshold))&&ae_fp_less(ae_fabs(res.y-(ca.x*cb.y+ca.y*cb.x), _state),threshold);
        res = ae_c_mul_d(ca,rb);
        mulcr = (mulcr&&ae_fp_less(ae_fabs(res.x-ca.x*rb, _state),threshold))&&ae_fp_less(ae_fabs(res.y-ca.y*rb, _state),threshold);
        res = ae_c_mul_d(cb,ra);
        mulrc = (mulrc&&ae_fp_less(ae_fabs(res.x-ra*cb.x, _state),threshold))&&ae_fp_less(ae_fabs(res.y-ra*cb.y, _state),threshold);
        
        /*
         * test Div
         */
        ca.x = 2*ae_randomreal(_state)-1;
        ca.y = 2*ae_randomreal(_state)-1;
        do
        {
            cb.x = 2*ae_randomreal(_state)-1;
            cb.y = 2*ae_randomreal(_state)-1;
        }
        while(ae_fp_less_eq(ae_c_abs(cb, _state),0.5));
        ra = 2*ae_randomreal(_state)-1;
        do
        {
            rb = 2*ae_randomreal(_state)-1;
        }
        while(ae_fp_less_eq(ae_fabs(rb, _state),0.5));
        res = ae_c_div(ca,cb);
        divcc = (divcc&&ae_fp_less(ae_fabs(ae_c_mul(res,cb).x-ca.x, _state),threshold))&&ae_fp_less(ae_fabs(ae_c_mul(res,cb).y-ca.y, _state),threshold);
        res = ae_c_div_d(ca,rb);
        divcr = (divcr&&ae_fp_less(ae_fabs(res.x-ca.x/rb, _state),threshold))&&ae_fp_less(ae_fabs(res.y-ca.y/rb, _state),threshold);
        res = ae_c_d_div(ra,cb);
        divrc = (divrc&&ae_fp_less(ae_fabs(ae_c_mul(res,cb).x-ra, _state),threshold))&&ae_fp_less(ae_fabs(ae_c_mul(res,cb).y, _state),threshold);
    }
    
    /*
     * summary
     */
    result = result&&absc;
    result = result&&addcc;
    result = result&&addcr;
    result = result&&addrc;
    result = result&&subcc;
    result = result&&subcr;
    result = result&&subrc;
    result = result&&mulcc;
    result = result&&mulcr;
    result = result&&mulrc;
    result = result&&divcc;
    result = result&&divcr;
    result = result&&divrc;
    if( !silent )
    {
        if( result )
        {
            printf("COMPLEX ARITHMETICS:                     OK\n");
        }
        else
        {
            printf("COMPLEX ARITHMETICS:                     FAILED\n");
            printf("* AddCC - - - - - - - - - - - - - - - -  ");
            if( addcc )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* AddCR - - - - - - - - - - - - - - - -  ");
            if( addcr )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* AddRC - - - - - - - - - - - - - - - -  ");
            if( addrc )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* SubCC - - - - - - - - - - - - - - - -  ");
            if( subcc )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* SubCR - - - - - - - - - - - - - - - -  ");
            if( subcr )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* SubRC - - - - - - - - - - - - - - - -  ");
            if( subrc )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* MulCC - - - - - - - - - - - - - - - -  ");
            if( mulcc )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* MulCR - - - - - - - - - - - - - - - -  ");
            if( mulcr )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* MulRC - - - - - - - - - - - - - - - -  ");
            if( mulrc )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* DivCC - - - - - - - - - - - - - - - -  ");
            if( divcc )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* DivCR - - - - - - - - - - - - - - - -  ");
            if( divcr )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* DivRC - - - - - - - - - - - - - - - -  ");
            if( divrc )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
        }
    }
    return result;
}


/*************************************************************************
Tests for IEEE special quantities
*************************************************************************/
static ae_bool testalglibbasicsunit_testieeespecial(ae_bool silent,
     ae_state *_state)
{
    ae_bool oknan;
    ae_bool okinf;
    ae_bool okother;
    double v1;
    double v2;
    ae_bool result;


    result = ae_true;
    oknan = ae_true;
    okinf = ae_true;
    okother = ae_true;
    
    /*
     * Test classification functions
     */
    okother = okother&&!ae_isinf(_state->v_nan, _state);
    okother = okother&&ae_isinf(_state->v_posinf, _state);
    okother = okother&&!ae_isinf(ae_maxrealnumber, _state);
    okother = okother&&!ae_isinf(1.0, _state);
    okother = okother&&!ae_isinf(ae_minrealnumber, _state);
    okother = okother&&!ae_isinf(0.0, _state);
    okother = okother&&!ae_isinf(-ae_minrealnumber, _state);
    okother = okother&&!ae_isinf(-1.0, _state);
    okother = okother&&!ae_isinf(-ae_maxrealnumber, _state);
    okother = okother&&ae_isinf(_state->v_neginf, _state);
    okother = okother&&!ae_isposinf(_state->v_nan, _state);
    okother = okother&&ae_isposinf(_state->v_posinf, _state);
    okother = okother&&!ae_isposinf(ae_maxrealnumber, _state);
    okother = okother&&!ae_isposinf(1.0, _state);
    okother = okother&&!ae_isposinf(ae_minrealnumber, _state);
    okother = okother&&!ae_isposinf(0.0, _state);
    okother = okother&&!ae_isposinf(-ae_minrealnumber, _state);
    okother = okother&&!ae_isposinf(-1.0, _state);
    okother = okother&&!ae_isposinf(-ae_maxrealnumber, _state);
    okother = okother&&!ae_isposinf(_state->v_neginf, _state);
    okother = okother&&!ae_isneginf(_state->v_nan, _state);
    okother = okother&&!ae_isneginf(_state->v_posinf, _state);
    okother = okother&&!ae_isneginf(ae_maxrealnumber, _state);
    okother = okother&&!ae_isneginf(1.0, _state);
    okother = okother&&!ae_isneginf(ae_minrealnumber, _state);
    okother = okother&&!ae_isneginf(0.0, _state);
    okother = okother&&!ae_isneginf(-ae_minrealnumber, _state);
    okother = okother&&!ae_isneginf(-1.0, _state);
    okother = okother&&!ae_isneginf(-ae_maxrealnumber, _state);
    okother = okother&&ae_isneginf(_state->v_neginf, _state);
    okother = okother&&ae_isnan(_state->v_nan, _state);
    okother = okother&&!ae_isnan(_state->v_posinf, _state);
    okother = okother&&!ae_isnan(ae_maxrealnumber, _state);
    okother = okother&&!ae_isnan(1.0, _state);
    okother = okother&&!ae_isnan(ae_minrealnumber, _state);
    okother = okother&&!ae_isnan(0.0, _state);
    okother = okother&&!ae_isnan(-ae_minrealnumber, _state);
    okother = okother&&!ae_isnan(-1.0, _state);
    okother = okother&&!ae_isnan(-ae_maxrealnumber, _state);
    okother = okother&&!ae_isnan(_state->v_neginf, _state);
    okother = okother&&!ae_isfinite(_state->v_nan, _state);
    okother = okother&&!ae_isfinite(_state->v_posinf, _state);
    okother = okother&&ae_isfinite(ae_maxrealnumber, _state);
    okother = okother&&ae_isfinite(1.0, _state);
    okother = okother&&ae_isfinite(ae_minrealnumber, _state);
    okother = okother&&ae_isfinite(0.0, _state);
    okother = okother&&ae_isfinite(-ae_minrealnumber, _state);
    okother = okother&&ae_isfinite(-1.0, _state);
    okother = okother&&ae_isfinite(-ae_maxrealnumber, _state);
    okother = okother&&!ae_isfinite(_state->v_neginf, _state);
    
    /*
     * Test NAN
     */
    v1 = _state->v_nan;
    v2 = _state->v_nan;
    oknan = oknan&&ae_isnan(v1, _state);
    oknan = oknan&&ae_fp_neq(v1,v2);
    oknan = oknan&&!ae_fp_eq(v1,v2);
    
    /*
     * Test INF:
     * * basic properties
     * * comparisons involving PosINF on one of the sides
     * * comparisons involving NegINF on one of the sides
     */
    v1 = _state->v_posinf;
    v2 = _state->v_neginf;
    okinf = okinf&&ae_isinf(_state->v_posinf, _state);
    okinf = okinf&&ae_isinf(v1, _state);
    okinf = okinf&&ae_isinf(_state->v_neginf, _state);
    okinf = okinf&&ae_isinf(v2, _state);
    okinf = okinf&&ae_isposinf(_state->v_posinf, _state);
    okinf = okinf&&ae_isposinf(v1, _state);
    okinf = okinf&&!ae_isposinf(_state->v_neginf, _state);
    okinf = okinf&&!ae_isposinf(v2, _state);
    okinf = okinf&&!ae_isneginf(_state->v_posinf, _state);
    okinf = okinf&&!ae_isneginf(v1, _state);
    okinf = okinf&&ae_isneginf(_state->v_neginf, _state);
    okinf = okinf&&ae_isneginf(v2, _state);
    okinf = okinf&&ae_fp_eq(_state->v_posinf,_state->v_posinf);
    okinf = okinf&&ae_fp_eq(_state->v_posinf,v1);
    okinf = okinf&&!ae_fp_eq(_state->v_posinf,_state->v_neginf);
    okinf = okinf&&!ae_fp_eq(_state->v_posinf,v2);
    okinf = okinf&&!ae_fp_eq(_state->v_posinf,(double)(0));
    okinf = okinf&&!ae_fp_eq(_state->v_posinf,1.2);
    okinf = okinf&&!ae_fp_eq(_state->v_posinf,-1.2);
    okinf = okinf&&ae_fp_eq(v1,_state->v_posinf);
    okinf = okinf&&!ae_fp_eq(_state->v_neginf,_state->v_posinf);
    okinf = okinf&&!ae_fp_eq(v2,_state->v_posinf);
    okinf = okinf&&!ae_fp_eq((double)(0),_state->v_posinf);
    okinf = okinf&&!ae_fp_eq(1.2,_state->v_posinf);
    okinf = okinf&&!ae_fp_eq(-1.2,_state->v_posinf);
    okinf = okinf&&!ae_fp_neq(_state->v_posinf,_state->v_posinf);
    okinf = okinf&&!ae_fp_neq(_state->v_posinf,v1);
    okinf = okinf&&ae_fp_neq(_state->v_posinf,_state->v_neginf);
    okinf = okinf&&ae_fp_neq(_state->v_posinf,v2);
    okinf = okinf&&ae_fp_neq(_state->v_posinf,(double)(0));
    okinf = okinf&&ae_fp_neq(_state->v_posinf,1.2);
    okinf = okinf&&ae_fp_neq(_state->v_posinf,-1.2);
    okinf = okinf&&!ae_fp_neq(v1,_state->v_posinf);
    okinf = okinf&&ae_fp_neq(_state->v_neginf,_state->v_posinf);
    okinf = okinf&&ae_fp_neq(v2,_state->v_posinf);
    okinf = okinf&&ae_fp_neq((double)(0),_state->v_posinf);
    okinf = okinf&&ae_fp_neq(1.2,_state->v_posinf);
    okinf = okinf&&ae_fp_neq(-1.2,_state->v_posinf);
    okinf = okinf&&!ae_fp_less(_state->v_posinf,_state->v_posinf);
    okinf = okinf&&!ae_fp_less(_state->v_posinf,v1);
    okinf = okinf&&!ae_fp_less(_state->v_posinf,_state->v_neginf);
    okinf = okinf&&!ae_fp_less(_state->v_posinf,v2);
    okinf = okinf&&!ae_fp_less(_state->v_posinf,(double)(0));
    okinf = okinf&&!ae_fp_less(_state->v_posinf,1.2);
    okinf = okinf&&!ae_fp_less(_state->v_posinf,-1.2);
    okinf = okinf&&!ae_fp_less(v1,_state->v_posinf);
    okinf = okinf&&ae_fp_less(_state->v_neginf,_state->v_posinf);
    okinf = okinf&&ae_fp_less(v2,_state->v_posinf);
    okinf = okinf&&ae_fp_less((double)(0),_state->v_posinf);
    okinf = okinf&&ae_fp_less(1.2,_state->v_posinf);
    okinf = okinf&&ae_fp_less(-1.2,_state->v_posinf);
    okinf = okinf&&ae_fp_less_eq(_state->v_posinf,_state->v_posinf);
    okinf = okinf&&ae_fp_less_eq(_state->v_posinf,v1);
    okinf = okinf&&!ae_fp_less_eq(_state->v_posinf,_state->v_neginf);
    okinf = okinf&&!ae_fp_less_eq(_state->v_posinf,v2);
    okinf = okinf&&!ae_fp_less_eq(_state->v_posinf,(double)(0));
    okinf = okinf&&!ae_fp_less_eq(_state->v_posinf,1.2);
    okinf = okinf&&!ae_fp_less_eq(_state->v_posinf,-1.2);
    okinf = okinf&&ae_fp_less_eq(v1,_state->v_posinf);
    okinf = okinf&&ae_fp_less_eq(_state->v_neginf,_state->v_posinf);
    okinf = okinf&&ae_fp_less_eq(v2,_state->v_posinf);
    okinf = okinf&&ae_fp_less_eq((double)(0),_state->v_posinf);
    okinf = okinf&&ae_fp_less_eq(1.2,_state->v_posinf);
    okinf = okinf&&ae_fp_less_eq(-1.2,_state->v_posinf);
    okinf = okinf&&!ae_fp_greater(_state->v_posinf,_state->v_posinf);
    okinf = okinf&&!ae_fp_greater(_state->v_posinf,v1);
    okinf = okinf&&ae_fp_greater(_state->v_posinf,_state->v_neginf);
    okinf = okinf&&ae_fp_greater(_state->v_posinf,v2);
    okinf = okinf&&ae_fp_greater(_state->v_posinf,(double)(0));
    okinf = okinf&&ae_fp_greater(_state->v_posinf,1.2);
    okinf = okinf&&ae_fp_greater(_state->v_posinf,-1.2);
    okinf = okinf&&!ae_fp_greater(v1,_state->v_posinf);
    okinf = okinf&&!ae_fp_greater(_state->v_neginf,_state->v_posinf);
    okinf = okinf&&!ae_fp_greater(v2,_state->v_posinf);
    okinf = okinf&&!ae_fp_greater((double)(0),_state->v_posinf);
    okinf = okinf&&!ae_fp_greater(1.2,_state->v_posinf);
    okinf = okinf&&!ae_fp_greater(-1.2,_state->v_posinf);
    okinf = okinf&&ae_fp_greater_eq(_state->v_posinf,_state->v_posinf);
    okinf = okinf&&ae_fp_greater_eq(_state->v_posinf,v1);
    okinf = okinf&&ae_fp_greater_eq(_state->v_posinf,_state->v_neginf);
    okinf = okinf&&ae_fp_greater_eq(_state->v_posinf,v2);
    okinf = okinf&&ae_fp_greater_eq(_state->v_posinf,(double)(0));
    okinf = okinf&&ae_fp_greater_eq(_state->v_posinf,1.2);
    okinf = okinf&&ae_fp_greater_eq(_state->v_posinf,-1.2);
    okinf = okinf&&ae_fp_greater_eq(v1,_state->v_posinf);
    okinf = okinf&&!ae_fp_greater_eq(_state->v_neginf,_state->v_posinf);
    okinf = okinf&&!ae_fp_greater_eq(v2,_state->v_posinf);
    okinf = okinf&&!ae_fp_greater_eq((double)(0),_state->v_posinf);
    okinf = okinf&&!ae_fp_greater_eq(1.2,_state->v_posinf);
    okinf = okinf&&!ae_fp_greater_eq(-1.2,_state->v_posinf);
    okinf = okinf&&!ae_fp_eq(_state->v_neginf,_state->v_posinf);
    okinf = okinf&&!ae_fp_eq(_state->v_neginf,v1);
    okinf = okinf&&ae_fp_eq(_state->v_neginf,_state->v_neginf);
    okinf = okinf&&ae_fp_eq(_state->v_neginf,v2);
    okinf = okinf&&!ae_fp_eq(_state->v_neginf,(double)(0));
    okinf = okinf&&!ae_fp_eq(_state->v_neginf,1.2);
    okinf = okinf&&!ae_fp_eq(_state->v_neginf,-1.2);
    okinf = okinf&&!ae_fp_eq(v1,_state->v_neginf);
    okinf = okinf&&ae_fp_eq(_state->v_neginf,_state->v_neginf);
    okinf = okinf&&ae_fp_eq(v2,_state->v_neginf);
    okinf = okinf&&!ae_fp_eq((double)(0),_state->v_neginf);
    okinf = okinf&&!ae_fp_eq(1.2,_state->v_neginf);
    okinf = okinf&&!ae_fp_eq(-1.2,_state->v_neginf);
    okinf = okinf&&ae_fp_neq(_state->v_neginf,_state->v_posinf);
    okinf = okinf&&ae_fp_neq(_state->v_neginf,v1);
    okinf = okinf&&!ae_fp_neq(_state->v_neginf,_state->v_neginf);
    okinf = okinf&&!ae_fp_neq(_state->v_neginf,v2);
    okinf = okinf&&ae_fp_neq(_state->v_neginf,(double)(0));
    okinf = okinf&&ae_fp_neq(_state->v_neginf,1.2);
    okinf = okinf&&ae_fp_neq(_state->v_neginf,-1.2);
    okinf = okinf&&ae_fp_neq(v1,_state->v_neginf);
    okinf = okinf&&!ae_fp_neq(_state->v_neginf,_state->v_neginf);
    okinf = okinf&&!ae_fp_neq(v2,_state->v_neginf);
    okinf = okinf&&ae_fp_neq((double)(0),_state->v_neginf);
    okinf = okinf&&ae_fp_neq(1.2,_state->v_neginf);
    okinf = okinf&&ae_fp_neq(-1.2,_state->v_neginf);
    okinf = okinf&&ae_fp_less(_state->v_neginf,_state->v_posinf);
    okinf = okinf&&ae_fp_less(_state->v_neginf,v1);
    okinf = okinf&&!ae_fp_less(_state->v_neginf,_state->v_neginf);
    okinf = okinf&&!ae_fp_less(_state->v_neginf,v2);
    okinf = okinf&&ae_fp_less(_state->v_neginf,(double)(0));
    okinf = okinf&&ae_fp_less(_state->v_neginf,1.2);
    okinf = okinf&&ae_fp_less(_state->v_neginf,-1.2);
    okinf = okinf&&!ae_fp_less(v1,_state->v_neginf);
    okinf = okinf&&!ae_fp_less(_state->v_neginf,_state->v_neginf);
    okinf = okinf&&!ae_fp_less(v2,_state->v_neginf);
    okinf = okinf&&!ae_fp_less((double)(0),_state->v_neginf);
    okinf = okinf&&!ae_fp_less(1.2,_state->v_neginf);
    okinf = okinf&&!ae_fp_less(-1.2,_state->v_neginf);
    okinf = okinf&&ae_fp_less_eq(_state->v_neginf,_state->v_posinf);
    okinf = okinf&&ae_fp_less_eq(_state->v_neginf,v1);
    okinf = okinf&&ae_fp_less_eq(_state->v_neginf,_state->v_neginf);
    okinf = okinf&&ae_fp_less_eq(_state->v_neginf,v2);
    okinf = okinf&&ae_fp_less_eq(_state->v_neginf,(double)(0));
    okinf = okinf&&ae_fp_less_eq(_state->v_neginf,1.2);
    okinf = okinf&&ae_fp_less_eq(_state->v_neginf,-1.2);
    okinf = okinf&&!ae_fp_less_eq(v1,_state->v_neginf);
    okinf = okinf&&ae_fp_less_eq(_state->v_neginf,_state->v_neginf);
    okinf = okinf&&ae_fp_less_eq(v2,_state->v_neginf);
    okinf = okinf&&!ae_fp_less_eq((double)(0),_state->v_neginf);
    okinf = okinf&&!ae_fp_less_eq(1.2,_state->v_neginf);
    okinf = okinf&&!ae_fp_less_eq(-1.2,_state->v_neginf);
    okinf = okinf&&!ae_fp_greater(_state->v_neginf,_state->v_posinf);
    okinf = okinf&&!ae_fp_greater(_state->v_neginf,v1);
    okinf = okinf&&!ae_fp_greater(_state->v_neginf,_state->v_neginf);
    okinf = okinf&&!ae_fp_greater(_state->v_neginf,v2);
    okinf = okinf&&!ae_fp_greater(_state->v_neginf,(double)(0));
    okinf = okinf&&!ae_fp_greater(_state->v_neginf,1.2);
    okinf = okinf&&!ae_fp_greater(_state->v_neginf,-1.2);
    okinf = okinf&&ae_fp_greater(v1,_state->v_neginf);
    okinf = okinf&&!ae_fp_greater(_state->v_neginf,_state->v_neginf);
    okinf = okinf&&!ae_fp_greater(v2,_state->v_neginf);
    okinf = okinf&&ae_fp_greater((double)(0),_state->v_neginf);
    okinf = okinf&&ae_fp_greater(1.2,_state->v_neginf);
    okinf = okinf&&ae_fp_greater(-1.2,_state->v_neginf);
    okinf = okinf&&!ae_fp_greater_eq(_state->v_neginf,_state->v_posinf);
    okinf = okinf&&!ae_fp_greater_eq(_state->v_neginf,v1);
    okinf = okinf&&ae_fp_greater_eq(_state->v_neginf,_state->v_neginf);
    okinf = okinf&&ae_fp_greater_eq(_state->v_neginf,v2);
    okinf = okinf&&!ae_fp_greater_eq(_state->v_neginf,(double)(0));
    okinf = okinf&&!ae_fp_greater_eq(_state->v_neginf,1.2);
    okinf = okinf&&!ae_fp_greater_eq(_state->v_neginf,-1.2);
    okinf = okinf&&ae_fp_greater_eq(v1,_state->v_neginf);
    okinf = okinf&&ae_fp_greater_eq(_state->v_neginf,_state->v_neginf);
    okinf = okinf&&ae_fp_greater_eq(v2,_state->v_neginf);
    okinf = okinf&&ae_fp_greater_eq((double)(0),_state->v_neginf);
    okinf = okinf&&ae_fp_greater_eq(1.2,_state->v_neginf);
    okinf = okinf&&ae_fp_greater_eq(-1.2,_state->v_neginf);
    
    /*
     * summary
     */
    result = result&&oknan;
    result = result&&okinf;
    result = result&&okother;
    if( !silent )
    {
        if( result )
        {
            printf("IEEE SPECIAL VALUES:                     OK\n");
        }
        else
        {
            printf("IEEE SPECIAL VALUES:                     FAILED\n");
            printf("* NAN - - - - - - - - - - - - - - - - -  ");
            if( oknan )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* INF - - - - - - - - - - - - - - - - -  ");
            if( okinf )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* FUNCTIONS - - - - - - - - - - - - - -  ");
            if( okother )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
        }
    }
    return result;
}


/*************************************************************************
Tests for swapping functions
*************************************************************************/
static ae_bool testalglibbasicsunit_testswapfunctions(ae_bool silent,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool okb1;
    ae_bool okb2;
    ae_bool oki1;
    ae_bool oki2;
    ae_bool okr1;
    ae_bool okr2;
    ae_bool okc1;
    ae_bool okc2;
    ae_vector b11;
    ae_vector b12;
    ae_vector i11;
    ae_vector i12;
    ae_vector r11;
    ae_vector r12;
    ae_vector c11;
    ae_vector c12;
    ae_matrix b21;
    ae_matrix b22;
    ae_matrix i21;
    ae_matrix i22;
    ae_matrix r21;
    ae_matrix r22;
    ae_matrix c21;
    ae_matrix c22;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&b11, 0, DT_BOOL, _state);
    ae_vector_init(&b12, 0, DT_BOOL, _state);
    ae_vector_init(&i11, 0, DT_INT, _state);
    ae_vector_init(&i12, 0, DT_INT, _state);
    ae_vector_init(&r11, 0, DT_REAL, _state);
    ae_vector_init(&r12, 0, DT_REAL, _state);
    ae_vector_init(&c11, 0, DT_COMPLEX, _state);
    ae_vector_init(&c12, 0, DT_COMPLEX, _state);
    ae_matrix_init(&b21, 0, 0, DT_BOOL, _state);
    ae_matrix_init(&b22, 0, 0, DT_BOOL, _state);
    ae_matrix_init(&i21, 0, 0, DT_INT, _state);
    ae_matrix_init(&i22, 0, 0, DT_INT, _state);
    ae_matrix_init(&r21, 0, 0, DT_REAL, _state);
    ae_matrix_init(&r22, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c21, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&c22, 0, 0, DT_COMPLEX, _state);

    result = ae_true;
    okb1 = ae_true;
    okb2 = ae_true;
    oki1 = ae_true;
    oki2 = ae_true;
    okr1 = ae_true;
    okr2 = ae_true;
    okc1 = ae_true;
    okc2 = ae_true;
    
    /*
     * Test B1 swaps
     */
    ae_vector_set_length(&b11, 1, _state);
    ae_vector_set_length(&b12, 2, _state);
    b11.ptr.p_bool[0] = ae_true;
    b12.ptr.p_bool[0] = ae_false;
    b12.ptr.p_bool[1] = ae_true;
    ae_swap_vectors(&b11, &b12);
    if( b11.cnt==2&&b12.cnt==1 )
    {
        okb1 = okb1&&!b11.ptr.p_bool[0];
        okb1 = okb1&&b11.ptr.p_bool[1];
        okb1 = okb1&&b12.ptr.p_bool[0];
    }
    else
    {
        okb1 = ae_false;
    }
    
    /*
     * Test I1 swaps
     */
    ae_vector_set_length(&i11, 1, _state);
    ae_vector_set_length(&i12, 2, _state);
    i11.ptr.p_int[0] = 1;
    i12.ptr.p_int[0] = 2;
    i12.ptr.p_int[1] = 3;
    ae_swap_vectors(&i11, &i12);
    if( i11.cnt==2&&i12.cnt==1 )
    {
        oki1 = oki1&&i11.ptr.p_int[0]==2;
        oki1 = oki1&&i11.ptr.p_int[1]==3;
        oki1 = oki1&&i12.ptr.p_int[0]==1;
    }
    else
    {
        oki1 = ae_false;
    }
    
    /*
     * Test R1 swaps
     */
    ae_vector_set_length(&r11, 1, _state);
    ae_vector_set_length(&r12, 2, _state);
    r11.ptr.p_double[0] = 1.5;
    r12.ptr.p_double[0] = 2.5;
    r12.ptr.p_double[1] = 3.5;
    ae_swap_vectors(&r11, &r12);
    if( r11.cnt==2&&r12.cnt==1 )
    {
        okr1 = okr1&&ae_fp_eq(r11.ptr.p_double[0],2.5);
        okr1 = okr1&&ae_fp_eq(r11.ptr.p_double[1],3.5);
        okr1 = okr1&&ae_fp_eq(r12.ptr.p_double[0],1.5);
    }
    else
    {
        okr1 = ae_false;
    }
    
    /*
     * Test C1 swaps
     */
    ae_vector_set_length(&c11, 1, _state);
    ae_vector_set_length(&c12, 2, _state);
    c11.ptr.p_complex[0] = ae_complex_from_i(1);
    c12.ptr.p_complex[0] = ae_complex_from_i(2);
    c12.ptr.p_complex[1] = ae_complex_from_i(3);
    ae_swap_vectors(&c11, &c12);
    if( c11.cnt==2&&c12.cnt==1 )
    {
        okc1 = okc1&&ae_c_eq_d(c11.ptr.p_complex[0],(double)(2));
        okc1 = okc1&&ae_c_eq_d(c11.ptr.p_complex[1],(double)(3));
        okc1 = okc1&&ae_c_eq_d(c12.ptr.p_complex[0],(double)(1));
    }
    else
    {
        okc1 = ae_false;
    }
    
    /*
     * Test B2 swaps
     */
    ae_matrix_set_length(&b21, 1, 2, _state);
    ae_matrix_set_length(&b22, 2, 1, _state);
    b21.ptr.pp_bool[0][0] = ae_true;
    b21.ptr.pp_bool[0][1] = ae_false;
    b22.ptr.pp_bool[0][0] = ae_false;
    b22.ptr.pp_bool[1][0] = ae_true;
    ae_swap_matrices(&b21, &b22);
    if( ((b21.rows==2&&b21.cols==1)&&b22.rows==1)&&b22.cols==2 )
    {
        okb2 = okb2&&!b21.ptr.pp_bool[0][0];
        okb2 = okb2&&b21.ptr.pp_bool[1][0];
        okb2 = okb2&&b22.ptr.pp_bool[0][0];
        okb2 = okb2&&!b22.ptr.pp_bool[0][1];
    }
    else
    {
        okb2 = ae_false;
    }
    
    /*
     * Test I2 swaps
     */
    ae_matrix_set_length(&i21, 1, 2, _state);
    ae_matrix_set_length(&i22, 2, 1, _state);
    i21.ptr.pp_int[0][0] = 1;
    i21.ptr.pp_int[0][1] = 2;
    i22.ptr.pp_int[0][0] = 3;
    i22.ptr.pp_int[1][0] = 4;
    ae_swap_matrices(&i21, &i22);
    if( ((i21.rows==2&&i21.cols==1)&&i22.rows==1)&&i22.cols==2 )
    {
        oki2 = oki2&&i21.ptr.pp_int[0][0]==3;
        oki2 = oki2&&i21.ptr.pp_int[1][0]==4;
        oki2 = oki2&&i22.ptr.pp_int[0][0]==1;
        oki2 = oki2&&i22.ptr.pp_int[0][1]==2;
    }
    else
    {
        oki2 = ae_false;
    }
    
    /*
     * Test R2 swaps
     */
    ae_matrix_set_length(&r21, 1, 2, _state);
    ae_matrix_set_length(&r22, 2, 1, _state);
    r21.ptr.pp_double[0][0] = (double)(1);
    r21.ptr.pp_double[0][1] = (double)(2);
    r22.ptr.pp_double[0][0] = (double)(3);
    r22.ptr.pp_double[1][0] = (double)(4);
    ae_swap_matrices(&r21, &r22);
    if( ((r21.rows==2&&r21.cols==1)&&r22.rows==1)&&r22.cols==2 )
    {
        okr2 = okr2&&ae_fp_eq(r21.ptr.pp_double[0][0],(double)(3));
        okr2 = okr2&&ae_fp_eq(r21.ptr.pp_double[1][0],(double)(4));
        okr2 = okr2&&ae_fp_eq(r22.ptr.pp_double[0][0],(double)(1));
        okr2 = okr2&&ae_fp_eq(r22.ptr.pp_double[0][1],(double)(2));
    }
    else
    {
        okr2 = ae_false;
    }
    
    /*
     * Test C2 swaps
     */
    ae_matrix_set_length(&c21, 1, 2, _state);
    ae_matrix_set_length(&c22, 2, 1, _state);
    c21.ptr.pp_complex[0][0] = ae_complex_from_i(1);
    c21.ptr.pp_complex[0][1] = ae_complex_from_i(2);
    c22.ptr.pp_complex[0][0] = ae_complex_from_i(3);
    c22.ptr.pp_complex[1][0] = ae_complex_from_i(4);
    ae_swap_matrices(&c21, &c22);
    if( ((c21.rows==2&&c21.cols==1)&&c22.rows==1)&&c22.cols==2 )
    {
        okc2 = okc2&&ae_c_eq_d(c21.ptr.pp_complex[0][0],(double)(3));
        okc2 = okc2&&ae_c_eq_d(c21.ptr.pp_complex[1][0],(double)(4));
        okc2 = okc2&&ae_c_eq_d(c22.ptr.pp_complex[0][0],(double)(1));
        okc2 = okc2&&ae_c_eq_d(c22.ptr.pp_complex[0][1],(double)(2));
    }
    else
    {
        okc2 = ae_false;
    }
    
    /*
     * summary
     */
    result = result&&okb1;
    result = result&&okb2;
    result = result&&oki1;
    result = result&&oki2;
    result = result&&okr1;
    result = result&&okr2;
    result = result&&okc1;
    result = result&&okc2;
    if( !silent )
    {
        if( result )
        {
            printf("SWAPPING FUNCTIONS:                      OK\n");
        }
        else
        {
            printf("SWAPPING FUNCTIONS:                      FAILED\n");
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Tests for standard functions
*************************************************************************/
static ae_bool testalglibbasicsunit_teststandardfunctions(ae_bool silent,
     ae_state *_state)
{
    ae_bool result;


    result = ae_true;
    
    /*
     * Test Sign()
     */
    result = result&&ae_sign(1.2, _state)==1;
    result = result&&ae_sign((double)(0), _state)==0;
    result = result&&ae_sign(-1.2, _state)==-1;
    
    /*
     * summary
     */
    if( !silent )
    {
        if( result )
        {
            printf("STANDARD FUNCTIONS:                      OK\n");
        }
        else
        {
            printf("STANDARD FUNCTIONS:                      FAILED\n");
        }
    }
    return result;
}


/*************************************************************************
Tests for serualization functions
*************************************************************************/
static ae_bool testalglibbasicsunit_testserializationfunctions(ae_bool silent,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool okb;
    ae_bool oki;
    ae_bool okr;
    ae_int_t nb;
    ae_int_t ni;
    ae_int_t nr;
    ae_int_t i;
    rec4serialization r0;
    rec4serialization r1;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _rec4serialization_init(&r0, _state);
    _rec4serialization_init(&r1, _state);

    result = ae_true;
    okb = ae_true;
    oki = ae_true;
    okr = ae_true;
    for(nb=1; nb<=4; nb++)
    {
        for(ni=1; ni<=4; ni++)
        {
            for(nr=1; nr<=4; nr++)
            {
                ae_vector_set_length(&r0.b, nb, _state);
                for(i=0; i<=nb-1; i++)
                {
                    r0.b.ptr.p_bool[i] = ae_randominteger(2, _state)!=0;
                }
                ae_vector_set_length(&r0.i, ni, _state);
                for(i=0; i<=ni-1; i++)
                {
                    r0.i.ptr.p_int[i] = ae_randominteger(10, _state)-5;
                }
                ae_vector_set_length(&r0.r, nr, _state);
                for(i=0; i<=nr-1; i++)
                {
                    r0.r.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                }
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
                    rec4serializationalloc(&_local_serializer, &r0, _state);
                    _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                    ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                    ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                    rec4serializationserialize(&_local_serializer, &r0, _state);
                    ae_serializer_stop(&_local_serializer);
                    ae_serializer_clear(&_local_serializer);
                    
                    ae_serializer_init(&_local_serializer);
                    ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                    rec4serializationunserialize(&_local_serializer, &r1, _state);
                    ae_serializer_stop(&_local_serializer);
                    ae_serializer_clear(&_local_serializer);
                    
                    ae_frame_leave(_state);
                }
                if( (r0.b.cnt==r1.b.cnt&&r0.i.cnt==r1.i.cnt)&&r0.r.cnt==r1.r.cnt )
                {
                    for(i=0; i<=nb-1; i++)
                    {
                        okb = okb&&((r0.b.ptr.p_bool[i]&&r1.b.ptr.p_bool[i])||(!r0.b.ptr.p_bool[i]&&!r1.b.ptr.p_bool[i]));
                    }
                    for(i=0; i<=ni-1; i++)
                    {
                        oki = oki&&r0.i.ptr.p_int[i]==r1.i.ptr.p_int[i];
                    }
                    for(i=0; i<=nr-1; i++)
                    {
                        okr = okr&&ae_fp_eq(r0.r.ptr.p_double[i],r1.r.ptr.p_double[i]);
                    }
                }
                else
                {
                    oki = ae_false;
                }
            }
        }
    }
    
    /*
     * summary
     */
    result = result&&okb;
    result = result&&oki;
    result = result&&okr;
    if( !silent )
    {
        if( result )
        {
            printf("SERIALIZATION FUNCTIONS:                 OK\n");
        }
        else
        {
            printf("SERIALIZATION FUNCTIONS:                 FAILED\n");
            printf("* BOOLEAN - - - - - - - - - - - - - - -  ");
            if( okb )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* INTEGER - - - - - - - - - - - - - - -  ");
            if( oki )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* REAL  - - - - - - - - - - - - - - - -  ");
            if( okr )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Tests for pool functions
*************************************************************************/
static void testalglibbasicsunit_createpoolandrecords(poolrec2* seedrec2,
     poolrec2* seedrec2copy,
     ae_shared_pool* pool,
     ae_state *_state)
{

    _poolrec2_clear(seedrec2);
    _poolrec2_clear(seedrec2copy);
    ae_shared_pool_clear(pool);

    seedrec2->bval = ae_fp_greater(ae_randomreal(_state),0.5);
    seedrec2->recval.bval = ae_fp_greater(ae_randomreal(_state),0.5);
    seedrec2->recval.ival = ae_randominteger(10, _state);
    seedrec2->recval.rval = ae_randomreal(_state);
    seedrec2->recval.cval.x = ae_randomreal(_state);
    seedrec2->recval.cval.y = ae_randomreal(_state);
    ae_vector_set_length(&seedrec2->recval.i1val, 3, _state);
    seedrec2->recval.i1val.ptr.p_int[0] = ae_randominteger(10, _state);
    seedrec2->recval.i1val.ptr.p_int[1] = ae_randominteger(10, _state);
    seedrec2->recval.i1val.ptr.p_int[2] = ae_randominteger(10, _state);
    seedrec2copy->bval = seedrec2->bval;
    seedrec2copy->recval.bval = seedrec2->recval.bval;
    seedrec2copy->recval.ival = seedrec2->recval.ival;
    seedrec2copy->recval.rval = seedrec2->recval.rval;
    seedrec2copy->recval.cval = seedrec2->recval.cval;
    ae_vector_set_length(&seedrec2copy->recval.i1val, 3, _state);
    seedrec2copy->recval.i1val.ptr.p_int[0] = seedrec2->recval.i1val.ptr.p_int[0];
    seedrec2copy->recval.i1val.ptr.p_int[1] = seedrec2->recval.i1val.ptr.p_int[1];
    seedrec2copy->recval.i1val.ptr.p_int[2] = seedrec2->recval.i1val.ptr.p_int[2];
    ae_shared_pool_set_seed(pool, seedrec2, sizeof(*seedrec2), _poolrec2_init, _poolrec2_init_copy, _poolrec2_destroy, _state);
}


static ae_bool testalglibbasicsunit_sharedpoolerrors(ae_state *_state)
{
    ae_frame _frame_block;
    poolrec1 seedrec1;
    poolrec2 seedrec2;
    poolrec2 seedrec2copy;
    ae_shared_pool pool;
    ae_shared_pool pool2;
    poolrec2 *prec2;
    ae_smart_ptr _prec2;
    poolrec2 *p0;
    ae_smart_ptr _p0;
    poolrec2 *p1;
    ae_smart_ptr _p1;
    poolrec2 *p2;
    ae_smart_ptr _p2;
    poolrec1 *q0;
    ae_smart_ptr _q0;
    poolrec1 *q1;
    ae_smart_ptr _q1;
    ae_shared_pool *ppool0;
    ae_smart_ptr _ppool0;
    ae_shared_pool *ppool1;
    ae_smart_ptr _ppool1;
    ae_int_t val100cnt;
    ae_int_t val101cnt;
    ae_int_t val102cnt;
    ae_int_t tmpval;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _poolrec1_init(&seedrec1, _state);
    _poolrec2_init(&seedrec2, _state);
    _poolrec2_init(&seedrec2copy, _state);
    ae_shared_pool_init(&pool, _state);
    ae_shared_pool_init(&pool2, _state);
    ae_smart_ptr_init(&_prec2, (void**)&prec2, _state);
    ae_smart_ptr_init(&_p0, (void**)&p0, _state);
    ae_smart_ptr_init(&_p1, (void**)&p1, _state);
    ae_smart_ptr_init(&_p2, (void**)&p2, _state);
    ae_smart_ptr_init(&_q0, (void**)&q0, _state);
    ae_smart_ptr_init(&_q1, (void**)&q1, _state);
    ae_smart_ptr_init(&_ppool0, (void**)&ppool0, _state);
    ae_smart_ptr_init(&_ppool1, (void**)&ppool1, _state);

    result = ae_true;
    
    /*
     * Test 1: test that:
     * a) smart pointer is null by default
     * b) "conventional local" is valid by default
     * b) unitinitialized shared pool is "not initialized"
     */
    if( prec2!=NULL )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( !(&seedrec1!=NULL) )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( ae_shared_pool_is_initialized(&pool) )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Test 2: basic copying of complex structures
     * * check that pool is recognized as "initialized"
     * * change original seed record,
     * * retrieve value from pool,
     * * check that it is valid
     * * and it is unchanged.
     */
    testalglibbasicsunit_createpoolandrecords(&seedrec2, &seedrec2copy, &pool, _state);
    if( !ae_shared_pool_is_initialized(&pool) )
    {
        ae_frame_leave(_state);
        return result;
    }
    seedrec2.bval = !seedrec2.bval;
    seedrec2.recval.i1val.ptr.p_int[0] = seedrec2.recval.i1val.ptr.p_int[0]+1;
    ae_shared_pool_retrieve(&pool, &_prec2, _state);
    if( !(prec2!=NULL) )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( (seedrec2copy.bval&&!prec2->bval)||(prec2->bval&&!seedrec2copy.bval) )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( seedrec2copy.recval.i1val.ptr.p_int[0]!=prec2->recval.i1val.ptr.p_int[0] )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Test 3: unrecycled values are lost
     * * retrieve value from pool,
     * * change it,
     * * retrieve one more time,
     * * check that it is unchanged.
     */
    testalglibbasicsunit_createpoolandrecords(&seedrec2, &seedrec2copy, &pool, _state);
    ae_shared_pool_retrieve(&pool, &_prec2, _state);
    prec2->recval.ival = prec2->recval.ival+1;
    ae_shared_pool_retrieve(&pool, &_prec2, _state);
    if( prec2->recval.ival!=seedrec2copy.recval.ival )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Test 4: recycled values are reused, PoolClearRecycled() removes recycled values
     * * retrieve value from pool,
     * * change it,
     * * recycle,
     * * check that recycled pointer is null
     * * retrieve one more time,
     * * check that modified value was returned,
     * * recycle,
     * * clear pool,
     * * retrieve one more time,
     * * check that unmodified value was returned,
     */
    testalglibbasicsunit_createpoolandrecords(&seedrec2, &seedrec2copy, &pool, _state);
    ae_shared_pool_retrieve(&pool, &_prec2, _state);
    prec2->recval.ival = prec2->recval.ival+1;
    ae_shared_pool_recycle(&pool, &_prec2, _state);
    if( prec2!=NULL )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_shared_pool_retrieve(&pool, &_prec2, _state);
    if( !(prec2!=NULL) )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( prec2->recval.ival!=seedrec2copy.recval.ival+1 )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_shared_pool_recycle(&pool, &_prec2, _state);
    ae_shared_pool_clear_recycled(&pool, _state);
    ae_shared_pool_retrieve(&pool, &_prec2, _state);
    if( !(prec2!=NULL) )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( prec2->recval.ival!=seedrec2copy.recval.ival )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Test 5: basic enumeration
     * * retrieve 3 values from pool
     * * fill RecVal.iVal by 100, 101, 102
     * * recycle values
     * * enumerate, check that each iVal occurs only once during enumeration
     * * repeat enumeration to make sure that it can be repeated
     */
    testalglibbasicsunit_createpoolandrecords(&seedrec2, &seedrec2copy, &pool, _state);
    ae_shared_pool_retrieve(&pool, &_p0, _state);
    ae_shared_pool_retrieve(&pool, &_p1, _state);
    ae_shared_pool_retrieve(&pool, &_p2, _state);
    p0->recval.ival = 100;
    p1->recval.ival = 101;
    p2->recval.ival = 102;
    ae_shared_pool_recycle(&pool, &_p1, _state);
    ae_shared_pool_recycle(&pool, &_p2, _state);
    ae_shared_pool_recycle(&pool, &_p0, _state);
    val100cnt = 0;
    val101cnt = 0;
    val102cnt = 0;
    ae_shared_pool_first_recycled(&pool, &_prec2, _state);
    while(prec2!=NULL)
    {
        if( prec2->recval.ival==100 )
        {
            val100cnt = val100cnt+1;
        }
        if( prec2->recval.ival==101 )
        {
            val101cnt = val101cnt+1;
        }
        if( prec2->recval.ival==102 )
        {
            val102cnt = val102cnt+1;
        }
        ae_shared_pool_next_recycled(&pool, &_prec2, _state);
    }
    if( (val100cnt!=1||val101cnt!=1)||val102cnt!=1 )
    {
        ae_frame_leave(_state);
        return result;
    }
    val100cnt = 0;
    val101cnt = 0;
    val102cnt = 0;
    ae_shared_pool_first_recycled(&pool, &_prec2, _state);
    while(prec2!=NULL)
    {
        if( prec2->recval.ival==100 )
        {
            val100cnt = val100cnt+1;
        }
        if( prec2->recval.ival==101 )
        {
            val101cnt = val101cnt+1;
        }
        if( prec2->recval.ival==102 )
        {
            val102cnt = val102cnt+1;
        }
        ae_shared_pool_next_recycled(&pool, &_prec2, _state);
    }
    if( (val100cnt!=1||val101cnt!=1)||val102cnt!=1 )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Test 6: pool itself can be pooled
     * * pool can be seeded with another pool
     * * smart pointers to pool are correctly handled
     * * pool correctly returns different references on "retrieve":
     *   * we retrieve, modify and recycle back to PPool0
     *   * we retrieve from PPool1 - unmodified value is returned
     *   * we retrievefrom PPool0  - modified value is returned
     */
    testalglibbasicsunit_createpoolandrecords(&seedrec2, &seedrec2copy, &pool, _state);
    ae_shared_pool_set_seed(&pool2, &pool, sizeof(pool), ae_shared_pool_init, ae_shared_pool_init_copy, ae_shared_pool_destroy, _state);
    if( ppool0!=NULL||ppool1!=NULL )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_shared_pool_retrieve(&pool2, &_ppool0, _state);
    ae_shared_pool_retrieve(&pool2, &_ppool1, _state);
    if( !(ppool0!=NULL&&ppool1!=NULL) )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_shared_pool_retrieve(ppool0, &_p0, _state);
    p0->recval.ival = p0->recval.ival+1;
    tmpval = p0->recval.ival;
    ae_shared_pool_recycle(ppool0, &_p0, _state);
    ae_shared_pool_retrieve(ppool1, &_p1, _state);
    if( p1->recval.ival==tmpval )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_shared_pool_recycle(ppool1, &_p1, _state);
    ae_shared_pool_retrieve(ppool0, &_p0, _state);
    if( p0->recval.ival!=tmpval )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Test 7: pools which are fields of records are correctly handled
     * * pool can be seeded with record which has initialized pool as its field
     * * when record is retrieved from pool, its fields are correctly copied (including
     *   fields which are pools)
     */
    testalglibbasicsunit_createpoolandrecords(&seedrec2, &seedrec2copy, &pool, _state);
    tmpval = 99;
    seedrec1.ival = tmpval;
    ae_shared_pool_set_seed(&seedrec2.pool, &seedrec1, sizeof(seedrec1), _poolrec1_init, _poolrec1_init_copy, _poolrec1_destroy, _state);
    ae_shared_pool_set_seed(&pool, &seedrec2, sizeof(seedrec2), _poolrec2_init, _poolrec2_init_copy, _poolrec2_destroy, _state);
    ae_shared_pool_retrieve(&pool, &_p0, _state);
    ae_shared_pool_retrieve(&p0->pool, &_q0, _state);
    q0->ival = tmpval-1;
    ae_shared_pool_recycle(&p0->pool, &_q0, _state);
    ae_shared_pool_retrieve(&pool, &_p1, _state);
    ae_shared_pool_retrieve(&p1->pool, &_q1, _state);
    if( q1->ival!=tmpval )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_shared_pool_recycle(&p1->pool, &_q1, _state);
    ae_shared_pool_retrieve(&p0->pool, &_q0, _state);
    if( q0->ival!=tmpval-1 )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Test 8: after call to PoolReset(), call to PoolFirstRecycled() returns null references
     */
    testalglibbasicsunit_createpoolandrecords(&seedrec2, &seedrec2copy, &pool, _state);
    ae_shared_pool_retrieve(&pool, &_p0, _state);
    ae_shared_pool_retrieve(&pool, &_p1, _state);
    ae_shared_pool_retrieve(&pool, &_p2, _state);
    ae_shared_pool_recycle(&pool, &_p1, _state);
    ae_shared_pool_recycle(&pool, &_p2, _state);
    ae_shared_pool_recycle(&pool, &_p0, _state);
    ae_shared_pool_first_recycled(&pool, &_p0, _state);
    if( !(p0!=NULL) )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_shared_pool_next_recycled(&pool, &_p0, _state);
    if( !(p0!=NULL) )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_shared_pool_next_recycled(&pool, &_p0, _state);
    if( !(p0!=NULL) )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_shared_pool_next_recycled(&pool, &_p0, _state);
    if( p0!=NULL )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_shared_pool_reset(&pool, _state);
    ae_shared_pool_first_recycled(&pool, &_p0, _state);
    if( p0!=NULL )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_shared_pool_next_recycled(&pool, &_p0, _state);
    if( p0!=NULL )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_shared_pool_next_recycled(&pool, &_p0, _state);
    if( p0!=NULL )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_shared_pool_next_recycled(&pool, &_p0, _state);
    if( p0!=NULL )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Test 9: invalid pointer is recognized as non-null (we do not reference it, just test)
     */
    testalglibbasicsunit_createpoolandrecords(&seedrec2, &seedrec2copy, &pool, _state);
    ae_shared_pool_retrieve(&pool, &_p0, _state);
    ae_shared_pool_retrieve(&pool, &_p1, _state);
    ae_shared_pool_retrieve(&pool, &_p2, _state);
    ae_shared_pool_recycle(&pool, &_p1, _state);
    ae_shared_pool_recycle(&pool, &_p2, _state);
    ae_shared_pool_recycle(&pool, &_p0, _state);
    ae_shared_pool_first_recycled(&pool, &_p0, _state);
    if( !(p0!=NULL) )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_shared_pool_clear_recycled(&pool, _state);
    if( !(p0!=NULL) )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Test 9: non-null pointer is nulled by calling SetNull()
     */
    testalglibbasicsunit_createpoolandrecords(&seedrec2, &seedrec2copy, &pool, _state);
    ae_shared_pool_retrieve(&pool, &_p0, _state);
    if( !(p0!=NULL) )
    {
        ae_frame_leave(_state);
        return result;
    }
    ae_smart_ptr_assign(&_p0, NULL, ae_false, ae_false, NULL);
    if( p0!=NULL )
    {
        ae_frame_leave(_state);
        return result;
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


static ae_bool testalglibbasicsunit_testsharedpool(ae_bool silent,
     ae_state *_state)
{
    ae_bool result;


    result = !testalglibbasicsunit_sharedpoolerrors(_state);
    if( !silent )
    {
        if( result )
        {
            printf("SHARED POOL:                             OK\n");
        }
        else
        {
            printf("SHARED POOL:                             FAILED\n");
        }
    }
    return result;
}


/*************************************************************************
Tests for SMP functions

testSort0: sort function
*************************************************************************/
static void testalglibbasicsunit_testsort0func(/* Integer */ ae_vector* a,
     /* Integer */ ae_vector* buf,
     ae_int_t idx0,
     ae_int_t idx2,
     ae_state *_state)
{
    ae_int_t idx1;


    if( idx2<=idx0+1 )
    {
        return;
    }
    idx1 = (idx0+idx2)/2;
    testalglibbasicsunit_testsort0func(a, buf, idx0, idx1, _state);
    testalglibbasicsunit_testsort0func(a, buf, idx1, idx2, _state);
    testalglibbasicsunit_mergesortedarrays(a, buf, idx0, idx1, idx2, _state);
}


/*************************************************************************
testSort0: recursive sorting by splitting array into two subarrays.
Returns True on success, False on failure.
*************************************************************************/
static ae_bool testalglibbasicsunit_performtestsort0(ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector a;
    ae_vector buf;
    ae_int_t i;
    ae_int_t k;
    ae_int_t t;
    ae_int_t n;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&a, 0, DT_INT, _state);
    ae_vector_init(&buf, 0, DT_INT, _state);

    n = 100000;
    ae_vector_set_length(&a, n, _state);
    ae_vector_set_length(&buf, n, _state);
    for(i=0; i<=n-1; i++)
    {
        a.ptr.p_int[i] = i;
    }
    for(i=0; i<=n-1; i++)
    {
        k = ae_randominteger(n, _state);
        if( k!=i )
        {
            t = a.ptr.p_int[i];
            a.ptr.p_int[i] = a.ptr.p_int[k];
            a.ptr.p_int[k] = t;
        }
    }
    testalglibbasicsunit_testsort0func(&a, &buf, 0, n, _state);
    result = ae_true;
    for(i=0; i<=n-1; i++)
    {
        result = result&&a.ptr.p_int[i]==i;
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
TestSort0: sort function
*************************************************************************/
static void testalglibbasicsunit_testsort1func(/* Integer */ ae_vector* a,
     /* Integer */ ae_vector* buf,
     ae_int_t idx0,
     ae_int_t idx2,
     ae_bool usesmp,
     ae_state *_state)
{
    ae_int_t idxa;
    ae_int_t idxb;
    ae_int_t idxc;
    ae_int_t cnt4;


    if( idx2<=idx0+1 )
    {
        return;
    }
    if( idx2==idx0+2 )
    {
        testalglibbasicsunit_mergesortedarrays(a, buf, idx0, idx0+1, idx0+2, _state);
        return;
    }
    if( idx2==idx0+3 )
    {
        testalglibbasicsunit_mergesortedarrays(a, buf, idx0+0, idx0+1, idx0+2, _state);
        testalglibbasicsunit_mergesortedarrays(a, buf, idx0+0, idx0+2, idx0+3, _state);
        return;
    }
    if( idx2==idx0+4 )
    {
        testalglibbasicsunit_mergesortedarrays(a, buf, idx0+0, idx0+1, idx0+2, _state);
        testalglibbasicsunit_mergesortedarrays(a, buf, idx0+2, idx0+3, idx0+4, _state);
        testalglibbasicsunit_mergesortedarrays(a, buf, idx0+0, idx0+2, idx0+4, _state);
        return;
    }
    cnt4 = (idx2-idx0)/4;
    idxa = idx0+cnt4;
    idxb = idx0+2*cnt4;
    idxc = idx0+3*cnt4;
    testalglibbasicsunit_testsort1func(a, buf, idx0, idxa, usesmp, _state);
    testalglibbasicsunit_testsort1func(a, buf, idxa, idxb, usesmp, _state);
    testalglibbasicsunit_testsort1func(a, buf, idxb, idxc, usesmp, _state);
    testalglibbasicsunit_testsort1func(a, buf, idxc, idx2, usesmp, _state);
    testalglibbasicsunit_mergesortedarrays(a, buf, idx0, idxa, idxb, _state);
    testalglibbasicsunit_mergesortedarrays(a, buf, idxb, idxc, idx2, _state);
    testalglibbasicsunit_mergesortedarrays(a, buf, idx0, idxb, idx2, _state);
}


/*************************************************************************
TestSort0: recursive sorting by splitting array into 4 subarrays.

Sorting is performed in three rounds:
* parallel sorting of randomly permuted array
* result is randomly shuffled and sequentially sorted
* result is randomly shuffled (again) and sorted in parallel mode (again)

The idea of such "multitry sort" is that we test ability of  SMP  core  to
interleave highly parallel parts of code with long sequential parts.

Returns True on success, False on failure.
*************************************************************************/
static ae_bool testalglibbasicsunit_performtestsort1(ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector a;
    ae_vector buf;
    ae_int_t i;
    ae_int_t k;
    ae_int_t t;
    ae_int_t n;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&a, 0, DT_INT, _state);
    ae_vector_init(&buf, 0, DT_INT, _state);

    
    /*
     * Generate array
     */
    n = 100000;
    ae_vector_set_length(&a, n, _state);
    ae_vector_set_length(&buf, n, _state);
    for(i=0; i<=n-1; i++)
    {
        a.ptr.p_int[i] = i;
    }
    
    /*
     * round 0: parallel sorting of randomly permuted array
     */
    for(i=0; i<=n-1; i++)
    {
        k = ae_randominteger(n, _state);
        if( k!=i )
        {
            t = a.ptr.p_int[i];
            a.ptr.p_int[i] = a.ptr.p_int[k];
            a.ptr.p_int[k] = t;
        }
    }
    testalglibbasicsunit_testsort1func(&a, &buf, 0, n, ae_true, _state);
    
    /*
     * round 1: result is randomly shuffled and sequentially sorted
     */
    for(i=0; i<=n-1; i++)
    {
        k = ae_randominteger(n, _state);
        if( k!=i )
        {
            t = a.ptr.p_int[i];
            a.ptr.p_int[i] = a.ptr.p_int[k];
            a.ptr.p_int[k] = t;
        }
    }
    testalglibbasicsunit_testsort1func(&a, &buf, 0, n, ae_false, _state);
    
    /*
     * round 2: result is randomly shuffled (again) and sorted in parallel mode (again)
     */
    for(i=0; i<=n-1; i++)
    {
        k = ae_randominteger(n, _state);
        if( k!=i )
        {
            t = a.ptr.p_int[i];
            a.ptr.p_int[i] = a.ptr.p_int[k];
            a.ptr.p_int[k] = t;
        }
    }
    testalglibbasicsunit_testsort1func(&a, &buf, 0, n, ae_true, _state);
    
    /*
     * Test
     */
    result = ae_true;
    for(i=0; i<=n-1; i++)
    {
        result = result&&a.ptr.p_int[i]==i;
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Tests for SMP functions

testSort2: sort function
*************************************************************************/
static void testalglibbasicsunit_testsort2func(/* Integer */ ae_vector* a,
     /* Integer */ ae_vector* buf,
     ae_int_t idx0,
     ae_int_t idx2,
     ae_state *_state)
{
    ae_int_t idx1;


    if( idx2<=idx0+1 )
    {
        return;
    }
    idx1 = idx0+1+ae_randominteger(idx2-idx0-1, _state);
    testalglibbasicsunit_testsort0func(a, buf, idx0, idx1, _state);
    testalglibbasicsunit_testsort0func(a, buf, idx1, idx2, _state);
    testalglibbasicsunit_mergesortedarrays(a, buf, idx0, idx1, idx2, _state);
}


/*************************************************************************
testSort2: recursive sorting by splitting array into two subarrays of
different length (main difference from testsort0).
Returns True on success, False on failure.
*************************************************************************/
static ae_bool testalglibbasicsunit_performtestsort2(ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector a;
    ae_vector buf;
    ae_int_t i;
    ae_int_t k;
    ae_int_t t;
    ae_int_t n;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&a, 0, DT_INT, _state);
    ae_vector_init(&buf, 0, DT_INT, _state);

    n = 100000;
    ae_vector_set_length(&a, n, _state);
    ae_vector_set_length(&buf, n, _state);
    for(i=0; i<=n-1; i++)
    {
        a.ptr.p_int[i] = i;
    }
    for(i=0; i<=n-1; i++)
    {
        k = ae_randominteger(n, _state);
        if( k!=i )
        {
            t = a.ptr.p_int[i];
            a.ptr.p_int[i] = a.ptr.p_int[k];
            a.ptr.p_int[k] = t;
        }
    }
    testalglibbasicsunit_testsort2func(&a, &buf, 0, n, _state);
    result = ae_true;
    for(i=0; i<=n-1; i++)
    {
        result = result&&a.ptr.p_int[i]==i;
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
TestPoolSum: summation with pool

We perform summation of 500000 numbers (each of them is equal to 1) in the
recurrent manner, by accumulation of result in the pool.

This test checks pool ability to handle continuous stream of operations.

Returns True on success, False on failure.
*************************************************************************/
static ae_bool testalglibbasicsunit_performtestpoolsum(ae_state *_state)
{
    ae_frame _frame_block;
    ae_shared_pool pool;
    poolsummand *ptr;
    ae_smart_ptr _ptr;
    poolsummand seed;
    ae_int_t n;
    ae_int_t sum;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_shared_pool_init(&pool, _state);
    ae_smart_ptr_init(&_ptr, (void**)&ptr, _state);
    _poolsummand_init(&seed, _state);

    n = 500000;
    seed.val = 0;
    ae_shared_pool_set_seed(&pool, &seed, sizeof(seed), _poolsummand_init, _poolsummand_init_copy, _poolsummand_destroy, _state);
    testalglibbasicsunit_parallelpoolsum(&pool, 0, n, _state);
    sum = 0;
    ae_shared_pool_first_recycled(&pool, &_ptr, _state);
    while(ptr!=NULL)
    {
        sum = sum+ptr->val;
        ae_shared_pool_next_recycled(&pool, &_ptr, _state);
    }
    result = sum==n;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Summation routune for parallel summation test.
*************************************************************************/
static void testalglibbasicsunit_parallelpoolsum(ae_shared_pool* sumpool,
     ae_int_t ind0,
     ae_int_t ind1,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    poolsummand *ptr;
    ae_smart_ptr _ptr;

    ae_frame_make(_state, &_frame_block);
    ae_smart_ptr_init(&_ptr, (void**)&ptr, _state);

    if( ind1-ind0<=2 )
    {
        ae_shared_pool_retrieve(sumpool, &_ptr, _state);
        ptr->val = ptr->val+ind1-ind0;
        ae_shared_pool_recycle(sumpool, &_ptr, _state);
    }
    else
    {
        i = (ind0+ind1)/2;
        testalglibbasicsunit_parallelpoolsum(sumpool, ind0, i, _state);
        testalglibbasicsunit_parallelpoolsum(sumpool, i, ind1, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function merges sorted A[Idx0,Idx1) and A[Idx1,Idx2) into sorted array
A[Idx0,Idx2) using corresponding elements of Buf.
*************************************************************************/
static void testalglibbasicsunit_mergesortedarrays(/* Integer */ ae_vector* a,
     /* Integer */ ae_vector* buf,
     ae_int_t idx0,
     ae_int_t idx1,
     ae_int_t idx2,
     ae_state *_state)
{
    ae_int_t srcleft;
    ae_int_t srcright;
    ae_int_t dst;


    srcleft = idx0;
    srcright = idx1;
    dst = idx0;
    for(;;)
    {
        if( srcleft==idx1&&srcright==idx2 )
        {
            break;
        }
        if( srcleft==idx1 )
        {
            buf->ptr.p_int[dst] = a->ptr.p_int[srcright];
            srcright = srcright+1;
            dst = dst+1;
            continue;
        }
        if( srcright==idx2 )
        {
            buf->ptr.p_int[dst] = a->ptr.p_int[srcleft];
            srcleft = srcleft+1;
            dst = dst+1;
            continue;
        }
        if( a->ptr.p_int[srcleft]<a->ptr.p_int[srcright] )
        {
            buf->ptr.p_int[dst] = a->ptr.p_int[srcleft];
            srcleft = srcleft+1;
            dst = dst+1;
        }
        else
        {
            buf->ptr.p_int[dst] = a->ptr.p_int[srcright];
            srcright = srcright+1;
            dst = dst+1;
        }
    }
    for(dst=idx0; dst<=idx2-1; dst++)
    {
        a->ptr.p_int[dst] = buf->ptr.p_int[dst];
    }
}


static ae_bool testalglibbasicsunit_testsmp(ae_bool silent,
     ae_state *_state)
{
    ae_bool t0;
    ae_bool t1;
    ae_bool t2;
    ae_bool ts;
    ae_bool result;


    t0 = testalglibbasicsunit_performtestsort0(_state);
    t1 = testalglibbasicsunit_performtestsort1(_state);
    t2 = testalglibbasicsunit_performtestsort2(_state);
    ts = testalglibbasicsunit_performtestpoolsum(_state);
    result = ((t0&&t1)&&t2)&&ts;
    if( !silent )
    {
        if( result )
        {
            printf("SMP FUNCTIONS:                           OK\n");
        }
        else
        {
            printf("SMP FUNCTIONS:                           FAILED\n");
            printf("* TEST SORT0 (sorting, split-2)          ");
            if( t0 )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* TEST SORT1 (sorting, split-4)          ");
            if( t1 )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* TEST SORT2 (sorting, split-2, unequal) ");
            if( t2 )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
            printf("* TEST POOLSUM (accumulation with pool)  ");
            if( ts )
            {
                printf("OK\n");
            }
            else
            {
                printf("FAILED\n");
            }
        }
    }
    return result;
}


void _rec1_init(void* _p, ae_state *_state)
{
    rec1 *p = (rec1*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->b1field, 0, DT_BOOL, _state);
    ae_vector_init(&p->r1field, 0, DT_REAL, _state);
    ae_vector_init(&p->i1field, 0, DT_INT, _state);
    ae_vector_init(&p->c1field, 0, DT_COMPLEX, _state);
    ae_matrix_init(&p->b2field, 0, 0, DT_BOOL, _state);
    ae_matrix_init(&p->r2field, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->i2field, 0, 0, DT_INT, _state);
    ae_matrix_init(&p->c2field, 0, 0, DT_COMPLEX, _state);
}


void _rec1_init_copy(void* _dst, void* _src, ae_state *_state)
{
    rec1 *dst = (rec1*)_dst;
    rec1 *src = (rec1*)_src;
    dst->bfield = src->bfield;
    dst->rfield = src->rfield;
    dst->ifield = src->ifield;
    dst->cfield = src->cfield;
    ae_vector_init_copy(&dst->b1field, &src->b1field, _state);
    ae_vector_init_copy(&dst->r1field, &src->r1field, _state);
    ae_vector_init_copy(&dst->i1field, &src->i1field, _state);
    ae_vector_init_copy(&dst->c1field, &src->c1field, _state);
    ae_matrix_init_copy(&dst->b2field, &src->b2field, _state);
    ae_matrix_init_copy(&dst->r2field, &src->r2field, _state);
    ae_matrix_init_copy(&dst->i2field, &src->i2field, _state);
    ae_matrix_init_copy(&dst->c2field, &src->c2field, _state);
}


void _rec1_clear(void* _p)
{
    rec1 *p = (rec1*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->b1field);
    ae_vector_clear(&p->r1field);
    ae_vector_clear(&p->i1field);
    ae_vector_clear(&p->c1field);
    ae_matrix_clear(&p->b2field);
    ae_matrix_clear(&p->r2field);
    ae_matrix_clear(&p->i2field);
    ae_matrix_clear(&p->c2field);
}


void _rec1_destroy(void* _p)
{
    rec1 *p = (rec1*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->b1field);
    ae_vector_destroy(&p->r1field);
    ae_vector_destroy(&p->i1field);
    ae_vector_destroy(&p->c1field);
    ae_matrix_destroy(&p->b2field);
    ae_matrix_destroy(&p->r2field);
    ae_matrix_destroy(&p->i2field);
    ae_matrix_destroy(&p->c2field);
}


void _rec4serialization_init(void* _p, ae_state *_state)
{
    rec4serialization *p = (rec4serialization*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->b, 0, DT_BOOL, _state);
    ae_vector_init(&p->i, 0, DT_INT, _state);
    ae_vector_init(&p->r, 0, DT_REAL, _state);
}


void _rec4serialization_init_copy(void* _dst, void* _src, ae_state *_state)
{
    rec4serialization *dst = (rec4serialization*)_dst;
    rec4serialization *src = (rec4serialization*)_src;
    ae_vector_init_copy(&dst->b, &src->b, _state);
    ae_vector_init_copy(&dst->i, &src->i, _state);
    ae_vector_init_copy(&dst->r, &src->r, _state);
}


void _rec4serialization_clear(void* _p)
{
    rec4serialization *p = (rec4serialization*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->b);
    ae_vector_clear(&p->i);
    ae_vector_clear(&p->r);
}


void _rec4serialization_destroy(void* _p)
{
    rec4serialization *p = (rec4serialization*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->b);
    ae_vector_destroy(&p->i);
    ae_vector_destroy(&p->r);
}


void _poolrec1_init(void* _p, ae_state *_state)
{
    poolrec1 *p = (poolrec1*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->i1val, 0, DT_INT, _state);
}


void _poolrec1_init_copy(void* _dst, void* _src, ae_state *_state)
{
    poolrec1 *dst = (poolrec1*)_dst;
    poolrec1 *src = (poolrec1*)_src;
    dst->cval = src->cval;
    dst->rval = src->rval;
    dst->ival = src->ival;
    dst->bval = src->bval;
    ae_vector_init_copy(&dst->i1val, &src->i1val, _state);
}


void _poolrec1_clear(void* _p)
{
    poolrec1 *p = (poolrec1*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->i1val);
}


void _poolrec1_destroy(void* _p)
{
    poolrec1 *p = (poolrec1*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->i1val);
}


void _poolrec2_init(void* _p, ae_state *_state)
{
    poolrec2 *p = (poolrec2*)_p;
    ae_touch_ptr((void*)p);
    _poolrec1_init(&p->recval, _state);
    ae_shared_pool_init(&p->pool, _state);
}


void _poolrec2_init_copy(void* _dst, void* _src, ae_state *_state)
{
    poolrec2 *dst = (poolrec2*)_dst;
    poolrec2 *src = (poolrec2*)_src;
    dst->bval = src->bval;
    _poolrec1_init_copy(&dst->recval, &src->recval, _state);
    ae_shared_pool_init_copy(&dst->pool, &src->pool, _state);
}


void _poolrec2_clear(void* _p)
{
    poolrec2 *p = (poolrec2*)_p;
    ae_touch_ptr((void*)p);
    _poolrec1_clear(&p->recval);
    ae_shared_pool_clear(&p->pool);
}


void _poolrec2_destroy(void* _p)
{
    poolrec2 *p = (poolrec2*)_p;
    ae_touch_ptr((void*)p);
    _poolrec1_destroy(&p->recval);
    ae_shared_pool_destroy(&p->pool);
}


void _poolsummand_init(void* _p, ae_state *_state)
{
    poolsummand *p = (poolsummand*)_p;
    ae_touch_ptr((void*)p);
}


void _poolsummand_init_copy(void* _dst, void* _src, ae_state *_state)
{
    poolsummand *dst = (poolsummand*)_dst;
    poolsummand *src = (poolsummand*)_src;
    dst->val = src->val;
}


void _poolsummand_clear(void* _p)
{
    poolsummand *p = (poolsummand*)_p;
    ae_touch_ptr((void*)p);
}


void _poolsummand_destroy(void* _p)
{
    poolsummand *p = (poolsummand*)_p;
    ae_touch_ptr((void*)p);
}


/*$ End $*/
