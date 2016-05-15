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
#include "testfhtunit.h"


/*$ Declarations $*/
static void testfhtunit_reffhtr1d(/* Real    */ ae_vector* a,
     ae_int_t n,
     ae_state *_state);
static void testfhtunit_reffhtr1dinv(/* Real    */ ae_vector* a,
     ae_int_t n,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Test
*************************************************************************/
ae_bool testfht(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t i;
    ae_vector r1;
    ae_vector r2;
    ae_vector r3;
    ae_int_t maxn;
    double bidierr;
    double referr;
    double errtol;
    ae_bool referrors;
    ae_bool bidierrors;
    ae_bool waserrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&r1, 0, DT_REAL, _state);
    ae_vector_init(&r2, 0, DT_REAL, _state);
    ae_vector_init(&r3, 0, DT_REAL, _state);

    maxn = 128;
    errtol = 100000*ae_pow((double)(maxn), (double)3/(double)2, _state)*ae_machineepsilon;
    bidierrors = ae_false;
    referrors = ae_false;
    waserrors = ae_false;
    
    /*
     * Test bi-directional error: norm(x-invFHT(FHT(x)))
     */
    bidierr = (double)(0);
    for(n=1; n<=maxn; n++)
    {
        
        /*
         * FHT/invFHT
         */
        ae_vector_set_length(&r1, n, _state);
        ae_vector_set_length(&r2, n, _state);
        ae_vector_set_length(&r3, n, _state);
        for(i=0; i<=n-1; i++)
        {
            r1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            r2.ptr.p_double[i] = r1.ptr.p_double[i];
            r3.ptr.p_double[i] = r1.ptr.p_double[i];
        }
        fhtr1d(&r2, n, _state);
        fhtr1dinv(&r2, n, _state);
        fhtr1dinv(&r3, n, _state);
        fhtr1d(&r3, n, _state);
        for(i=0; i<=n-1; i++)
        {
            bidierr = ae_maxreal(bidierr, ae_fabs(r1.ptr.p_double[i]-r2.ptr.p_double[i], _state), _state);
            bidierr = ae_maxreal(bidierr, ae_fabs(r1.ptr.p_double[i]-r3.ptr.p_double[i], _state), _state);
        }
    }
    bidierrors = bidierrors||ae_fp_greater(bidierr,errtol);
    
    /*
     * Test against reference O(N^2) implementation
     */
    referr = (double)(0);
    for(n=1; n<=maxn; n++)
    {
        
        /*
         * FHT
         */
        ae_vector_set_length(&r1, n, _state);
        ae_vector_set_length(&r2, n, _state);
        for(i=0; i<=n-1; i++)
        {
            r1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            r2.ptr.p_double[i] = r1.ptr.p_double[i];
        }
        fhtr1d(&r1, n, _state);
        testfhtunit_reffhtr1d(&r2, n, _state);
        for(i=0; i<=n-1; i++)
        {
            referr = ae_maxreal(referr, ae_fabs(r1.ptr.p_double[i]-r2.ptr.p_double[i], _state), _state);
        }
        
        /*
         * inverse FHT
         */
        ae_vector_set_length(&r1, n, _state);
        ae_vector_set_length(&r2, n, _state);
        for(i=0; i<=n-1; i++)
        {
            r1.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            r2.ptr.p_double[i] = r1.ptr.p_double[i];
        }
        fhtr1dinv(&r1, n, _state);
        testfhtunit_reffhtr1dinv(&r2, n, _state);
        for(i=0; i<=n-1; i++)
        {
            referr = ae_maxreal(referr, ae_fabs(r1.ptr.p_double[i]-r2.ptr.p_double[i], _state), _state);
        }
    }
    referrors = referrors||ae_fp_greater(referr,errtol);
    
    /*
     * end
     */
    waserrors = bidierrors||referrors;
    if( !silent )
    {
        printf("TESTING FHT\n");
        printf("FINAL RESULT:                             ");
        if( waserrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* BI-DIRECTIONAL TEST:                    ");
        if( bidierrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* AGAINST REFERENCE FHT:                  ");
        if( referrors )
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
    }
    result = !waserrors;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testfht(ae_bool silent, ae_state *_state)
{
    return testfht(silent, _state);
}


/*************************************************************************
Reference FHT
*************************************************************************/
static void testfhtunit_reffhtr1d(/* Real    */ ae_vector* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector buf;
    ae_int_t i;
    ae_int_t j;
    double v;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&buf, 0, DT_REAL, _state);

    ae_assert(n>0, "RefFHTR1D: incorrect N!", _state);
    ae_vector_set_length(&buf, n, _state);
    for(i=0; i<=n-1; i++)
    {
        v = (double)(0);
        for(j=0; j<=n-1; j++)
        {
            v = v+a->ptr.p_double[j]*(ae_cos(2*ae_pi*i*j/n, _state)+ae_sin(2*ae_pi*i*j/n, _state));
        }
        buf.ptr.p_double[i] = v;
    }
    for(i=0; i<=n-1; i++)
    {
        a->ptr.p_double[i] = buf.ptr.p_double[i];
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Reference inverse FHT
*************************************************************************/
static void testfhtunit_reffhtr1dinv(/* Real    */ ae_vector* a,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(n>0, "RefFHTR1DInv: incorrect N!", _state);
    testfhtunit_reffhtr1d(a, n, _state);
    for(i=0; i<=n-1; i++)
    {
        a->ptr.p_double[i] = a->ptr.p_double[i]/n;
    }
}


/*$ End $*/
