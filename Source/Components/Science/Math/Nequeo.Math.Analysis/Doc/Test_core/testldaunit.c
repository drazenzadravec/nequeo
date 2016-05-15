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
#include "testldaunit.h"


/*$ Declarations $*/
static void testldaunit_gensimpleset(ae_int_t nfeatures,
     ae_int_t nclasses,
     ae_int_t nsamples,
     ae_int_t axis,
     /* Real    */ ae_matrix* xy,
     ae_state *_state);
static void testldaunit_gendeg1set(ae_int_t nfeatures,
     ae_int_t nclasses,
     ae_int_t nsamples,
     ae_int_t axis,
     /* Real    */ ae_matrix* xy,
     ae_state *_state);
static double testldaunit_generatenormal(double mean,
     double sigma,
     ae_state *_state);
static ae_bool testldaunit_testwn(/* Real    */ ae_matrix* xy,
     /* Real    */ ae_matrix* wn,
     ae_int_t ns,
     ae_int_t nf,
     ae_int_t nc,
     ae_int_t ndeg,
     ae_state *_state);
static double testldaunit_calcj(ae_int_t nf,
     /* Real    */ ae_matrix* st,
     /* Real    */ ae_matrix* sw,
     /* Real    */ ae_vector* w,
     double* p,
     double* q,
     ae_state *_state);
static void testldaunit_fishers(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nfeatures,
     ae_int_t nclasses,
     /* Real    */ ae_matrix* st,
     /* Real    */ ae_matrix* sw,
     ae_state *_state);


/*$ Body $*/


ae_bool testlda(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t maxnf;
    ae_int_t maxns;
    ae_int_t maxnc;
    ae_int_t passcount;
    ae_bool ldanerrors;
    ae_bool lda1errors;
    ae_bool waserrors;
    ae_int_t nf;
    ae_int_t nc;
    ae_int_t ns;
    ae_int_t i;
    ae_int_t info;
    ae_int_t pass;
    ae_int_t axis;
    ae_matrix xy;
    ae_matrix wn;
    ae_vector w1;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&wn, 0, 0, DT_REAL, _state);
    ae_vector_init(&w1, 0, DT_REAL, _state);

    
    /*
     * Primary settings
     */
    maxnf = 10;
    maxns = 1000;
    maxnc = 5;
    passcount = 1;
    waserrors = ae_false;
    ldanerrors = ae_false;
    lda1errors = ae_false;
    
    /*
     * General tests
     */
    for(nf=1; nf<=maxnf; nf++)
    {
        for(nc=2; nc<=maxnc; nc++)
        {
            for(pass=1; pass<=passcount; pass++)
            {
                
                /*
                 * Simple test for LDA-N/LDA-1
                 */
                axis = ae_randominteger(nf, _state);
                ns = maxns/2+ae_randominteger(maxns/2, _state);
                testldaunit_gensimpleset(nf, nc, ns, axis, &xy, _state);
                fisherldan(&xy, ns, nf, nc, &info, &wn, _state);
                if( info!=1 )
                {
                    ldanerrors = ae_true;
                    continue;
                }
                ldanerrors = ldanerrors||!testldaunit_testwn(&xy, &wn, ns, nf, nc, 0, _state);
                ldanerrors = ldanerrors||ae_fp_less_eq(ae_fabs(wn.ptr.pp_double[axis][0], _state),0.75);
                fisherlda(&xy, ns, nf, nc, &info, &w1, _state);
                for(i=0; i<=nf-1; i++)
                {
                    lda1errors = lda1errors||ae_fp_neq(w1.ptr.p_double[i],wn.ptr.pp_double[i][0]);
                }
                
                /*
                 * Degenerate test for LDA-N
                 */
                if( nf>=3 )
                {
                    ns = maxns/2+ae_randominteger(maxns/2, _state);
                    
                    /*
                     * there are two duplicate features,
                     * axis is oriented along non-duplicate feature
                     */
                    axis = ae_randominteger(nf-2, _state);
                    testldaunit_gendeg1set(nf, nc, ns, axis, &xy, _state);
                    fisherldan(&xy, ns, nf, nc, &info, &wn, _state);
                    if( info!=2 )
                    {
                        ldanerrors = ae_true;
                        continue;
                    }
                    ldanerrors = ldanerrors||ae_fp_less_eq(wn.ptr.pp_double[axis][0],0.75);
                    fisherlda(&xy, ns, nf, nc, &info, &w1, _state);
                    for(i=0; i<=nf-1; i++)
                    {
                        lda1errors = lda1errors||ae_fp_neq(w1.ptr.p_double[i],wn.ptr.pp_double[i][0]);
                    }
                }
            }
        }
    }
    
    /*
     * Final report
     */
    waserrors = ldanerrors||lda1errors;
    if( !silent )
    {
        printf("LDA TEST\n");
        printf("FISHER LDA-N:                            ");
        if( !ldanerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("FISHER LDA-1:                            ");
        if( !lda1errors )
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
ae_bool _pexec_testlda(ae_bool silent, ae_state *_state)
{
    return testlda(silent, _state);
}


/*************************************************************************
Generates 'simple' set - a sequence of unit 'balls' at (0,0), (1,0), (2,0)
and so on.
*************************************************************************/
static void testldaunit_gensimpleset(ae_int_t nfeatures,
     ae_int_t nclasses,
     ae_int_t nsamples,
     ae_int_t axis,
     /* Real    */ ae_matrix* xy,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t c;

    ae_matrix_clear(xy);

    ae_assert(axis>=0&&axis<nfeatures, "GenSimpleSet: wrong Axis!", _state);
    ae_matrix_set_length(xy, nsamples-1+1, nfeatures+1, _state);
    for(i=0; i<=nsamples-1; i++)
    {
        for(j=0; j<=nfeatures-1; j++)
        {
            xy->ptr.pp_double[i][j] = testldaunit_generatenormal(0.0, 1.0, _state);
        }
        c = i%nclasses;
        xy->ptr.pp_double[i][axis] = xy->ptr.pp_double[i][axis]+c;
        xy->ptr.pp_double[i][nfeatures] = (double)(c);
    }
}


/*************************************************************************
Generates 'degenerate' set #1.
NFeatures>=3.
*************************************************************************/
static void testldaunit_gendeg1set(ae_int_t nfeatures,
     ae_int_t nclasses,
     ae_int_t nsamples,
     ae_int_t axis,
     /* Real    */ ae_matrix* xy,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t c;

    ae_matrix_clear(xy);

    ae_assert(axis>=0&&axis<nfeatures, "GenDeg1Set: wrong Axis!", _state);
    ae_assert(nfeatures>=3, "GenDeg1Set: wrong NFeatures!", _state);
    ae_matrix_set_length(xy, nsamples-1+1, nfeatures+1, _state);
    if( axis>=nfeatures-2 )
    {
        axis = nfeatures-3;
    }
    for(i=0; i<=nsamples-1; i++)
    {
        for(j=0; j<=nfeatures-2; j++)
        {
            xy->ptr.pp_double[i][j] = testldaunit_generatenormal(0.0, 1.0, _state);
        }
        xy->ptr.pp_double[i][nfeatures-1] = xy->ptr.pp_double[i][nfeatures-2];
        c = i%nclasses;
        xy->ptr.pp_double[i][axis] = xy->ptr.pp_double[i][axis]+c;
        xy->ptr.pp_double[i][nfeatures] = (double)(c);
    }
}


/*************************************************************************
Normal random number
*************************************************************************/
static double testldaunit_generatenormal(double mean,
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
Tests WN for correctness
*************************************************************************/
static ae_bool testldaunit_testwn(/* Real    */ ae_matrix* xy,
     /* Real    */ ae_matrix* wn,
     ae_int_t ns,
     ae_int_t nf,
     ae_int_t nc,
     ae_int_t ndeg,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix st;
    ae_matrix sw;
    ae_matrix a;
    ae_matrix z;
    ae_vector tx;
    ae_vector jp;
    ae_vector jq;
    ae_vector work;
    ae_int_t i;
    ae_int_t j;
    double v;
    double wprev;
    double tol;
    double p;
    double q;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&st, 0, 0, DT_REAL, _state);
    ae_matrix_init(&sw, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&z, 0, 0, DT_REAL, _state);
    ae_vector_init(&tx, 0, DT_REAL, _state);
    ae_vector_init(&jp, 0, DT_REAL, _state);
    ae_vector_init(&jq, 0, DT_REAL, _state);
    ae_vector_init(&work, 0, DT_REAL, _state);

    tol = (double)(10000);
    result = ae_true;
    testldaunit_fishers(xy, ns, nf, nc, &st, &sw, _state);
    
    /*
     * Test for decreasing of J
     */
    ae_vector_set_length(&tx, nf-1+1, _state);
    ae_vector_set_length(&jp, nf-1+1, _state);
    ae_vector_set_length(&jq, nf-1+1, _state);
    for(j=0; j<=nf-1; j++)
    {
        ae_v_move(&tx.ptr.p_double[0], 1, &wn->ptr.pp_double[0][j], wn->stride, ae_v_len(0,nf-1));
        v = testldaunit_calcj(nf, &st, &sw, &tx, &p, &q, _state);
        jp.ptr.p_double[j] = p;
        jq.ptr.p_double[j] = q;
    }
    for(i=1; i<=nf-1-ndeg; i++)
    {
        result = result&&ae_fp_greater_eq(jp.ptr.p_double[i-1]/jq.ptr.p_double[i-1],(1-tol*ae_machineepsilon)*jp.ptr.p_double[i]/jq.ptr.p_double[i]);
    }
    for(i=nf-1-ndeg+1; i<=nf-1; i++)
    {
        result = result&&ae_fp_less_eq(jp.ptr.p_double[i],tol*ae_machineepsilon*jp.ptr.p_double[0]);
    }
    
    /*
     * Test for J optimality
     */
    ae_v_move(&tx.ptr.p_double[0], 1, &wn->ptr.pp_double[0][0], wn->stride, ae_v_len(0,nf-1));
    v = testldaunit_calcj(nf, &st, &sw, &tx, &p, &q, _state);
    for(i=0; i<=nf-1; i++)
    {
        wprev = tx.ptr.p_double[i];
        tx.ptr.p_double[i] = wprev+0.01;
        result = result&&ae_fp_greater_eq(v,(1-tol*ae_machineepsilon)*testldaunit_calcj(nf, &st, &sw, &tx, &p, &q, _state));
        tx.ptr.p_double[i] = wprev-0.01;
        result = result&&ae_fp_greater_eq(v,(1-tol*ae_machineepsilon)*testldaunit_calcj(nf, &st, &sw, &tx, &p, &q, _state));
        tx.ptr.p_double[i] = wprev;
    }
    
    /*
     * Test for linear independence of W
     */
    ae_vector_set_length(&work, nf+1, _state);
    ae_matrix_set_length(&a, nf-1+1, nf-1+1, _state);
    matrixmatrixmultiply(wn, 0, nf-1, 0, nf-1, ae_false, wn, 0, nf-1, 0, nf-1, ae_true, 1.0, &a, 0, nf-1, 0, nf-1, 0.0, &work, _state);
    if( smatrixevd(&a, nf, 1, ae_true, &tx, &z, _state) )
    {
        result = result&&ae_fp_greater(tx.ptr.p_double[0],tx.ptr.p_double[nf-1]*1000*ae_machineepsilon);
    }
    
    /*
     * Test for other properties
     */
    for(j=0; j<=nf-1; j++)
    {
        v = ae_v_dotproduct(&wn->ptr.pp_double[0][j], wn->stride, &wn->ptr.pp_double[0][j], wn->stride, ae_v_len(0,nf-1));
        v = ae_sqrt(v, _state);
        result = result&&ae_fp_less_eq(ae_fabs(v-1, _state),1000*ae_machineepsilon);
        v = (double)(0);
        for(i=0; i<=nf-1; i++)
        {
            v = v+wn->ptr.pp_double[i][j];
        }
        result = result&&ae_fp_greater_eq(v,(double)(0));
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Calculates J
*************************************************************************/
static double testldaunit_calcj(ae_int_t nf,
     /* Real    */ ae_matrix* st,
     /* Real    */ ae_matrix* sw,
     /* Real    */ ae_vector* w,
     double* p,
     double* q,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector tx;
    ae_int_t i;
    double v;
    double result;

    ae_frame_make(_state, &_frame_block);
    *p = 0;
    *q = 0;
    ae_vector_init(&tx, 0, DT_REAL, _state);

    ae_vector_set_length(&tx, nf-1+1, _state);
    for(i=0; i<=nf-1; i++)
    {
        v = ae_v_dotproduct(&st->ptr.pp_double[i][0], 1, &w->ptr.p_double[0], 1, ae_v_len(0,nf-1));
        tx.ptr.p_double[i] = v;
    }
    v = ae_v_dotproduct(&w->ptr.p_double[0], 1, &tx.ptr.p_double[0], 1, ae_v_len(0,nf-1));
    *p = v;
    for(i=0; i<=nf-1; i++)
    {
        v = ae_v_dotproduct(&sw->ptr.pp_double[i][0], 1, &w->ptr.p_double[0], 1, ae_v_len(0,nf-1));
        tx.ptr.p_double[i] = v;
    }
    v = ae_v_dotproduct(&w->ptr.p_double[0], 1, &tx.ptr.p_double[0], 1, ae_v_len(0,nf-1));
    *q = v;
    result = *p/(*q);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Calculates ST/SW
*************************************************************************/
static void testldaunit_fishers(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nfeatures,
     ae_int_t nclasses,
     /* Real    */ ae_matrix* st,
     /* Real    */ ae_matrix* sw,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    ae_vector c;
    ae_vector mu;
    ae_matrix muc;
    ae_vector nc;
    ae_vector tf;
    ae_vector work;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(st);
    ae_matrix_clear(sw);
    ae_vector_init(&c, 0, DT_INT, _state);
    ae_vector_init(&mu, 0, DT_REAL, _state);
    ae_matrix_init(&muc, 0, 0, DT_REAL, _state);
    ae_vector_init(&nc, 0, DT_INT, _state);
    ae_vector_init(&tf, 0, DT_REAL, _state);
    ae_vector_init(&work, 0, DT_REAL, _state);

    
    /*
     * Prepare temporaries
     */
    ae_vector_set_length(&tf, nfeatures-1+1, _state);
    ae_vector_set_length(&work, nfeatures+1, _state);
    
    /*
     * Convert class labels from reals to integers (just for convenience)
     */
    ae_vector_set_length(&c, npoints-1+1, _state);
    for(i=0; i<=npoints-1; i++)
    {
        c.ptr.p_int[i] = ae_round(xy->ptr.pp_double[i][nfeatures], _state);
    }
    
    /*
     * Calculate class sizes and means
     */
    ae_vector_set_length(&mu, nfeatures-1+1, _state);
    ae_matrix_set_length(&muc, nclasses-1+1, nfeatures-1+1, _state);
    ae_vector_set_length(&nc, nclasses-1+1, _state);
    for(j=0; j<=nfeatures-1; j++)
    {
        mu.ptr.p_double[j] = (double)(0);
    }
    for(i=0; i<=nclasses-1; i++)
    {
        nc.ptr.p_int[i] = 0;
        for(j=0; j<=nfeatures-1; j++)
        {
            muc.ptr.pp_double[i][j] = (double)(0);
        }
    }
    for(i=0; i<=npoints-1; i++)
    {
        ae_v_add(&mu.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nfeatures-1));
        ae_v_add(&muc.ptr.pp_double[c.ptr.p_int[i]][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nfeatures-1));
        nc.ptr.p_int[c.ptr.p_int[i]] = nc.ptr.p_int[c.ptr.p_int[i]]+1;
    }
    for(i=0; i<=nclasses-1; i++)
    {
        v = (double)1/(double)nc.ptr.p_int[i];
        ae_v_muld(&muc.ptr.pp_double[i][0], 1, ae_v_len(0,nfeatures-1), v);
    }
    v = (double)1/(double)npoints;
    ae_v_muld(&mu.ptr.p_double[0], 1, ae_v_len(0,nfeatures-1), v);
    
    /*
     * Create ST matrix
     */
    ae_matrix_set_length(st, nfeatures-1+1, nfeatures-1+1, _state);
    for(i=0; i<=nfeatures-1; i++)
    {
        for(j=0; j<=nfeatures-1; j++)
        {
            st->ptr.pp_double[i][j] = (double)(0);
        }
    }
    for(k=0; k<=npoints-1; k++)
    {
        ae_v_move(&tf.ptr.p_double[0], 1, &xy->ptr.pp_double[k][0], 1, ae_v_len(0,nfeatures-1));
        ae_v_sub(&tf.ptr.p_double[0], 1, &mu.ptr.p_double[0], 1, ae_v_len(0,nfeatures-1));
        for(i=0; i<=nfeatures-1; i++)
        {
            v = tf.ptr.p_double[i];
            ae_v_addd(&st->ptr.pp_double[i][0], 1, &tf.ptr.p_double[0], 1, ae_v_len(0,nfeatures-1), v);
        }
    }
    
    /*
     * Create SW matrix
     */
    ae_matrix_set_length(sw, nfeatures-1+1, nfeatures-1+1, _state);
    for(i=0; i<=nfeatures-1; i++)
    {
        for(j=0; j<=nfeatures-1; j++)
        {
            sw->ptr.pp_double[i][j] = (double)(0);
        }
    }
    for(k=0; k<=npoints-1; k++)
    {
        ae_v_move(&tf.ptr.p_double[0], 1, &xy->ptr.pp_double[k][0], 1, ae_v_len(0,nfeatures-1));
        ae_v_sub(&tf.ptr.p_double[0], 1, &muc.ptr.pp_double[c.ptr.p_int[k]][0], 1, ae_v_len(0,nfeatures-1));
        for(i=0; i<=nfeatures-1; i++)
        {
            v = tf.ptr.p_double[i];
            ae_v_addd(&sw->ptr.pp_double[i][0], 1, &tf.ptr.p_double[0], 1, ae_v_len(0,nfeatures-1), v);
        }
    }
    ae_frame_leave(_state);
}


/*$ End $*/
