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
#include "testtsortunit.h"


/*$ Declarations $*/
static void testtsortunit_unset1di(/* Integer */ ae_vector* a,
     ae_state *_state);
static void testtsortunit_testsortresults(/* Real    */ ae_vector* asorted,
     /* Integer */ ae_vector* p1,
     /* Integer */ ae_vector* p2,
     /* Real    */ ae_vector* aoriginal,
     ae_int_t n,
     ae_bool* waserrors,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Testing tag sort
*************************************************************************/
ae_bool testtsort(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool waserrors;
    ae_int_t n;
    ae_int_t i;
    ae_int_t m;
    ae_int_t offs;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t maxn;
    ae_vector a;
    ae_vector a0;
    ae_vector a1;
    ae_vector a2;
    ae_vector a3;
    ae_vector i1;
    ae_vector i2;
    ae_vector i3;
    ae_vector a4;
    ae_vector pa4;
    ae_vector ar;
    ae_vector ar2;
    ae_vector ai;
    ae_vector p1;
    ae_vector p2;
    ae_vector bufr1;
    ae_vector bufr2;
    ae_vector bufi1;
    ae_bool distinctvals;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&a, 0, DT_REAL, _state);
    ae_vector_init(&a0, 0, DT_REAL, _state);
    ae_vector_init(&a1, 0, DT_REAL, _state);
    ae_vector_init(&a2, 0, DT_REAL, _state);
    ae_vector_init(&a3, 0, DT_REAL, _state);
    ae_vector_init(&i1, 0, DT_INT, _state);
    ae_vector_init(&i2, 0, DT_INT, _state);
    ae_vector_init(&i3, 0, DT_INT, _state);
    ae_vector_init(&a4, 0, DT_INT, _state);
    ae_vector_init(&pa4, 0, DT_INT, _state);
    ae_vector_init(&ar, 0, DT_REAL, _state);
    ae_vector_init(&ar2, 0, DT_REAL, _state);
    ae_vector_init(&ai, 0, DT_INT, _state);
    ae_vector_init(&p1, 0, DT_INT, _state);
    ae_vector_init(&p2, 0, DT_INT, _state);
    ae_vector_init(&bufr1, 0, DT_REAL, _state);
    ae_vector_init(&bufr2, 0, DT_REAL, _state);
    ae_vector_init(&bufi1, 0, DT_INT, _state);

    waserrors = ae_false;
    maxn = 100;
    passcount = 10;
    
    /*
     * Test tagsort
     */
    for(n=1; n<=maxn; n++)
    {
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             * Pprobably distinct sort:
             * * generate array of integer random numbers.
             *   Because of birthday paradox, random numbers have to be VERY large
             *   in order to avoid situation when we have distinct values.
             * * sort A0 using TagSort and test sort results
             * * now we can use A0 as reference point and test other functions
             */
            testtsortunit_unset1di(&p1, _state);
            testtsortunit_unset1di(&p2, _state);
            ae_vector_set_length(&a, n, _state);
            ae_vector_set_length(&a0, n, _state);
            ae_vector_set_length(&a1, n, _state);
            ae_vector_set_length(&a2, n, _state);
            ae_vector_set_length(&a3, n, _state);
            ae_vector_set_length(&a4, n, _state);
            ae_vector_set_length(&ar, n, _state);
            ae_vector_set_length(&ar2, n, _state);
            ae_vector_set_length(&ai, n, _state);
            for(i=0; i<=n-1; i++)
            {
                a.ptr.p_double[i] = (double)(ae_randominteger(100000000, _state));
                a0.ptr.p_double[i] = a.ptr.p_double[i];
                a1.ptr.p_double[i] = a.ptr.p_double[i];
                a2.ptr.p_double[i] = a.ptr.p_double[i];
                a3.ptr.p_double[i] = a.ptr.p_double[i];
                a4.ptr.p_int[i] = ae_round(a.ptr.p_double[i], _state);
                ar.ptr.p_double[i] = (double)(i);
                ar2.ptr.p_double[i] = (double)(i);
                ai.ptr.p_int[i] = i;
            }
            tagsort(&a0, n, &p1, &p2, _state);
            testtsortunit_testsortresults(&a0, &p1, &p2, &a, n, &waserrors, _state);
            distinctvals = ae_true;
            for(i=1; i<=n-1; i++)
            {
                distinctvals = distinctvals&&ae_fp_neq(a0.ptr.p_double[i],a0.ptr.p_double[i-1]);
            }
            if( distinctvals )
            {
                tagsortfasti(&a1, &ai, &bufr1, &bufi1, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    waserrors = (waserrors||ae_fp_neq(a1.ptr.p_double[i],a0.ptr.p_double[i]))||ai.ptr.p_int[i]!=p1.ptr.p_int[i];
                }
                tagsortfastr(&a2, &ar, &bufr1, &bufr2, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    waserrors = (waserrors||ae_fp_neq(a2.ptr.p_double[i],a0.ptr.p_double[i]))||ae_fp_neq(ar.ptr.p_double[i],(double)(p1.ptr.p_int[i]));
                }
                tagsortfast(&a3, &bufr1, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    waserrors = waserrors||ae_fp_neq(a3.ptr.p_double[i],a0.ptr.p_double[i]);
                }
                tagsortmiddleir(&a4, &ar2, 0, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    waserrors = (waserrors||ae_fp_neq((double)(a4.ptr.p_int[i]),a0.ptr.p_double[i]))||ae_fp_neq(ar2.ptr.p_double[i],(double)(p1.ptr.p_int[i]));
                }
            }
            
            /*
             * Non-distinct sort.
             * We test that keys are correctly reordered, but do NOT test order of values.
             */
            testtsortunit_unset1di(&p1, _state);
            testtsortunit_unset1di(&p2, _state);
            ae_vector_set_length(&a, n, _state);
            ae_vector_set_length(&a0, n, _state);
            ae_vector_set_length(&a1, n, _state);
            ae_vector_set_length(&a2, n, _state);
            ae_vector_set_length(&a3, n, _state);
            ae_vector_set_length(&a4, n, _state);
            ae_vector_set_length(&ar, n, _state);
            ae_vector_set_length(&ar2, n, _state);
            ae_vector_set_length(&ai, n, _state);
            for(i=0; i<=n-1; i++)
            {
                a.ptr.p_double[i] = (double)((n-i)/2);
                a0.ptr.p_double[i] = a.ptr.p_double[i];
                a1.ptr.p_double[i] = a.ptr.p_double[i];
                a2.ptr.p_double[i] = a.ptr.p_double[i];
                a3.ptr.p_double[i] = a.ptr.p_double[i];
                a4.ptr.p_int[i] = ae_round(a.ptr.p_double[i], _state);
                ar.ptr.p_double[i] = (double)(i);
                ar2.ptr.p_double[i] = (double)(i);
                ai.ptr.p_int[i] = i;
            }
            tagsort(&a0, n, &p1, &p2, _state);
            testtsortunit_testsortresults(&a0, &p1, &p2, &a, n, &waserrors, _state);
            tagsortfasti(&a1, &ai, &bufr1, &bufi1, n, _state);
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors||ae_fp_neq(a1.ptr.p_double[i],a0.ptr.p_double[i]);
            }
            tagsortfastr(&a2, &ar, &bufr1, &bufr2, n, _state);
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors||ae_fp_neq(a2.ptr.p_double[i],a0.ptr.p_double[i]);
            }
            tagsortfast(&a3, &bufr1, n, _state);
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors||ae_fp_neq(a3.ptr.p_double[i],a0.ptr.p_double[i]);
            }
            tagsortmiddleir(&a4, &ar2, 0, n, _state);
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors||ae_fp_neq((double)(a4.ptr.p_int[i]),a0.ptr.p_double[i]);
            }
            
            /*
             * 'All same' sort
             * We test that keys are correctly reordered, but do NOT test order of values.
             */
            testtsortunit_unset1di(&p1, _state);
            testtsortunit_unset1di(&p2, _state);
            ae_vector_set_length(&a, n, _state);
            ae_vector_set_length(&a0, n, _state);
            ae_vector_set_length(&a1, n, _state);
            ae_vector_set_length(&a2, n, _state);
            ae_vector_set_length(&a3, n, _state);
            ae_vector_set_length(&a4, n, _state);
            ae_vector_set_length(&ar, n, _state);
            ae_vector_set_length(&ar2, n, _state);
            ae_vector_set_length(&ai, n, _state);
            for(i=0; i<=n-1; i++)
            {
                a.ptr.p_double[i] = (double)(0);
                a0.ptr.p_double[i] = a.ptr.p_double[i];
                a1.ptr.p_double[i] = a.ptr.p_double[i];
                a2.ptr.p_double[i] = a.ptr.p_double[i];
                a3.ptr.p_double[i] = a.ptr.p_double[i];
                a4.ptr.p_int[i] = ae_round(a.ptr.p_double[i], _state);
                ar.ptr.p_double[i] = (double)(i);
                ar2.ptr.p_double[i] = (double)(i);
                ai.ptr.p_int[i] = i;
            }
            tagsort(&a0, n, &p1, &p2, _state);
            testtsortunit_testsortresults(&a0, &p1, &p2, &a, n, &waserrors, _state);
            tagsortfasti(&a1, &ai, &bufr1, &bufi1, n, _state);
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors||ae_fp_neq(a1.ptr.p_double[i],a0.ptr.p_double[i]);
            }
            tagsortfastr(&a2, &ar, &bufr1, &bufr2, n, _state);
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors||ae_fp_neq(a2.ptr.p_double[i],a0.ptr.p_double[i]);
            }
            tagsortfast(&a3, &bufr1, n, _state);
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors||ae_fp_neq(a3.ptr.p_double[i],a0.ptr.p_double[i]);
            }
            tagsortmiddleir(&a4, &ar2, 0, n, _state);
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors||ae_fp_neq((double)(a4.ptr.p_int[i]),a0.ptr.p_double[i]);
            }
            
            /*
             * 0-1 sort
             * We test that keys are correctly reordered, but do NOT test order of values.
             */
            testtsortunit_unset1di(&p1, _state);
            testtsortunit_unset1di(&p2, _state);
            ae_vector_set_length(&a, n, _state);
            ae_vector_set_length(&a0, n, _state);
            ae_vector_set_length(&a1, n, _state);
            ae_vector_set_length(&a2, n, _state);
            ae_vector_set_length(&a3, n, _state);
            ae_vector_set_length(&a4, n, _state);
            ae_vector_set_length(&ar, n, _state);
            ae_vector_set_length(&ar2, n, _state);
            ae_vector_set_length(&ai, n, _state);
            for(i=0; i<=n-1; i++)
            {
                a.ptr.p_double[i] = (double)(ae_randominteger(2, _state));
                a0.ptr.p_double[i] = a.ptr.p_double[i];
                a1.ptr.p_double[i] = a.ptr.p_double[i];
                a2.ptr.p_double[i] = a.ptr.p_double[i];
                a3.ptr.p_double[i] = a.ptr.p_double[i];
                a4.ptr.p_int[i] = ae_round(a.ptr.p_double[i], _state);
                ar.ptr.p_double[i] = (double)(i);
                ar2.ptr.p_double[i] = (double)(i);
                ai.ptr.p_int[i] = i;
            }
            tagsort(&a0, n, &p1, &p2, _state);
            testtsortunit_testsortresults(&a0, &p1, &p2, &a, n, &waserrors, _state);
            tagsortfasti(&a1, &ai, &bufr1, &bufi1, n, _state);
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors||ae_fp_neq(a1.ptr.p_double[i],a0.ptr.p_double[i]);
            }
            tagsortfastr(&a2, &ar, &bufr1, &bufr2, n, _state);
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors||ae_fp_neq(a2.ptr.p_double[i],a0.ptr.p_double[i]);
            }
            tagsortfast(&a3, &bufr1, n, _state);
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors||ae_fp_neq(a3.ptr.p_double[i],a0.ptr.p_double[i]);
            }
            tagsortmiddleir(&a4, &ar2, 0, n, _state);
            for(i=0; i<=n-1; i++)
            {
                waserrors = waserrors||ae_fp_neq((double)(a4.ptr.p_int[i]),a0.ptr.p_double[i]);
            }
            
            /*
             * Special test for TagSortMiddleIR: sorting in the middle gives same results
             * as sorting in the beginning of the array
             */
            m = 3*n;
            offs = ae_randominteger(n, _state);
            ae_vector_set_length(&i1, m, _state);
            ae_vector_set_length(&i2, m, _state);
            ae_vector_set_length(&i3, m, _state);
            ae_vector_set_length(&ar, m, _state);
            ae_vector_set_length(&ar2, m, _state);
            for(i=0; i<=m-1; i++)
            {
                i1.ptr.p_int[i] = ae_randominteger(100000000, _state);
                i2.ptr.p_int[i] = i1.ptr.p_int[i];
                i3.ptr.p_int[i] = i1.ptr.p_int[i];
                ar.ptr.p_double[i] = (double)(i);
                ar2.ptr.p_double[i] = (double)(i);
            }
            for(i=0; i<=n-1; i++)
            {
                i1.ptr.p_int[i] = i1.ptr.p_int[offs+i];
                ar.ptr.p_double[i] = ar.ptr.p_double[offs+i];
            }
            tagsortmiddleir(&i1, &ar, 0, n, _state);
            for(i=1; i<=n-1; i++)
            {
                distinctvals = distinctvals&&i1.ptr.p_int[i]!=i1.ptr.p_int[i-1];
            }
            if( distinctvals )
            {
                tagsortmiddleir(&i2, &ar2, offs, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    waserrors = (waserrors||i2.ptr.p_int[offs+i]!=i1.ptr.p_int[i])||ae_fp_neq(ar2.ptr.p_double[offs+i],ar.ptr.p_double[i]);
                }
            }
        }
    }
    
    /*
     * report
     */
    if( !silent )
    {
        printf("TESTING TAGSORT\n");
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
ae_bool _pexec_testtsort(ae_bool silent, ae_state *_state)
{
    return testtsort(silent, _state);
}


/*************************************************************************
Unsets 1D array.
*************************************************************************/
static void testtsortunit_unset1di(/* Integer */ ae_vector* a,
     ae_state *_state)
{


    ae_vector_set_length(a, 0+1, _state);
    a->ptr.p_int[0] = ae_randominteger(3, _state)-1;
}


static void testtsortunit_testsortresults(/* Real    */ ae_vector* asorted,
     /* Integer */ ae_vector* p1,
     /* Integer */ ae_vector* p2,
     /* Real    */ ae_vector* aoriginal,
     ae_int_t n,
     ae_bool* waserrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_vector a2;
    double t;
    ae_vector f;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&a2, 0, DT_REAL, _state);
    ae_vector_init(&f, 0, DT_INT, _state);

    ae_vector_set_length(&a2, n-1+1, _state);
    ae_vector_set_length(&f, n-1+1, _state);
    
    /*
     * is set ordered?
     */
    for(i=0; i<=n-2; i++)
    {
        *waserrors = *waserrors||ae_fp_greater(asorted->ptr.p_double[i],asorted->ptr.p_double[i+1]);
    }
    
    /*
     * P1 correctness
     */
    for(i=0; i<=n-1; i++)
    {
        *waserrors = *waserrors||ae_fp_neq(asorted->ptr.p_double[i],aoriginal->ptr.p_double[p1->ptr.p_int[i]]);
    }
    for(i=0; i<=n-1; i++)
    {
        f.ptr.p_int[i] = 0;
    }
    for(i=0; i<=n-1; i++)
    {
        f.ptr.p_int[p1->ptr.p_int[i]] = f.ptr.p_int[p1->ptr.p_int[i]]+1;
    }
    for(i=0; i<=n-1; i++)
    {
        *waserrors = *waserrors||f.ptr.p_int[i]!=1;
    }
    
    /*
     * P2 correctness
     */
    for(i=0; i<=n-1; i++)
    {
        a2.ptr.p_double[i] = aoriginal->ptr.p_double[i];
    }
    for(i=0; i<=n-1; i++)
    {
        if( p2->ptr.p_int[i]!=i )
        {
            t = a2.ptr.p_double[i];
            a2.ptr.p_double[i] = a2.ptr.p_double[p2->ptr.p_int[i]];
            a2.ptr.p_double[p2->ptr.p_int[i]] = t;
        }
    }
    for(i=0; i<=n-1; i++)
    {
        *waserrors = *waserrors||ae_fp_neq(asorted->ptr.p_double[i],a2.ptr.p_double[i]);
    }
    ae_frame_leave(_state);
}


/*$ End $*/
