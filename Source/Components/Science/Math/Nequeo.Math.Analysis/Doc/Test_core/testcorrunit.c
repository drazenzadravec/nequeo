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
#include "testcorrunit.h"


/*$ Declarations $*/
static void testcorrunit_refcorrc1d(/* Complex */ ae_vector* signal,
     ae_int_t n,
     /* Complex */ ae_vector* pattern,
     ae_int_t m,
     /* Complex */ ae_vector* r,
     ae_state *_state);
static void testcorrunit_refcorrc1dcircular(/* Complex */ ae_vector* signal,
     ae_int_t n,
     /* Complex */ ae_vector* pattern,
     ae_int_t m,
     /* Complex */ ae_vector* r,
     ae_state *_state);
static void testcorrunit_refcorrr1d(/* Real    */ ae_vector* signal,
     ae_int_t n,
     /* Real    */ ae_vector* pattern,
     ae_int_t m,
     /* Real    */ ae_vector* r,
     ae_state *_state);
static void testcorrunit_refcorrr1dcircular(/* Real    */ ae_vector* signal,
     ae_int_t n,
     /* Real    */ ae_vector* pattern,
     ae_int_t m,
     /* Real    */ ae_vector* r,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Test
*************************************************************************/
ae_bool testcorr(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t m;
    ae_int_t n;
    ae_int_t i;
    ae_vector ra;
    ae_vector rb;
    ae_vector rr1;
    ae_vector rr2;
    ae_vector ca;
    ae_vector cb;
    ae_vector cr1;
    ae_vector cr2;
    ae_int_t maxn;
    double referr;
    double refrerr;
    double errtol;
    ae_bool referrors;
    ae_bool refrerrors;
    ae_bool inverrors;
    ae_bool invrerrors;
    ae_bool waserrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&ra, 0, DT_REAL, _state);
    ae_vector_init(&rb, 0, DT_REAL, _state);
    ae_vector_init(&rr1, 0, DT_REAL, _state);
    ae_vector_init(&rr2, 0, DT_REAL, _state);
    ae_vector_init(&ca, 0, DT_COMPLEX, _state);
    ae_vector_init(&cb, 0, DT_COMPLEX, _state);
    ae_vector_init(&cr1, 0, DT_COMPLEX, _state);
    ae_vector_init(&cr2, 0, DT_COMPLEX, _state);

    maxn = 32;
    errtol = 100000*ae_pow((double)(maxn), (double)3/(double)2, _state)*ae_machineepsilon;
    referrors = ae_false;
    refrerrors = ae_false;
    inverrors = ae_false;
    invrerrors = ae_false;
    waserrors = ae_false;
    
    /*
     * Test against reference O(N^2) implementation.
     */
    referr = (double)(0);
    refrerr = (double)(0);
    for(m=1; m<=maxn; m++)
    {
        for(n=1; n<=maxn; n++)
        {
            
            /*
             * Complex correlation
             */
            ae_vector_set_length(&ca, m, _state);
            for(i=0; i<=m-1; i++)
            {
                ca.ptr.p_complex[i].x = 2*ae_randomreal(_state)-1;
                ca.ptr.p_complex[i].y = 2*ae_randomreal(_state)-1;
            }
            ae_vector_set_length(&cb, n, _state);
            for(i=0; i<=n-1; i++)
            {
                cb.ptr.p_complex[i].x = 2*ae_randomreal(_state)-1;
                cb.ptr.p_complex[i].y = 2*ae_randomreal(_state)-1;
            }
            ae_vector_set_length(&cr1, 1, _state);
            corrc1d(&ca, m, &cb, n, &cr1, _state);
            testcorrunit_refcorrc1d(&ca, m, &cb, n, &cr2, _state);
            for(i=0; i<=m+n-2; i++)
            {
                referr = ae_maxreal(referr, ae_c_abs(ae_c_sub(cr1.ptr.p_complex[i],cr2.ptr.p_complex[i]), _state), _state);
            }
            ae_vector_set_length(&cr1, 1, _state);
            corrc1dcircular(&ca, m, &cb, n, &cr1, _state);
            testcorrunit_refcorrc1dcircular(&ca, m, &cb, n, &cr2, _state);
            for(i=0; i<=m-1; i++)
            {
                referr = ae_maxreal(referr, ae_c_abs(ae_c_sub(cr1.ptr.p_complex[i],cr2.ptr.p_complex[i]), _state), _state);
            }
            
            /*
             * Real correlation
             */
            ae_vector_set_length(&ra, m, _state);
            for(i=0; i<=m-1; i++)
            {
                ra.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            ae_vector_set_length(&rb, n, _state);
            for(i=0; i<=n-1; i++)
            {
                rb.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            ae_vector_set_length(&rr1, 1, _state);
            corrr1d(&ra, m, &rb, n, &rr1, _state);
            testcorrunit_refcorrr1d(&ra, m, &rb, n, &rr2, _state);
            for(i=0; i<=m+n-2; i++)
            {
                refrerr = ae_maxreal(refrerr, ae_fabs(rr1.ptr.p_double[i]-rr2.ptr.p_double[i], _state), _state);
            }
            ae_vector_set_length(&rr1, 1, _state);
            corrr1dcircular(&ra, m, &rb, n, &rr1, _state);
            testcorrunit_refcorrr1dcircular(&ra, m, &rb, n, &rr2, _state);
            for(i=0; i<=m-1; i++)
            {
                refrerr = ae_maxreal(refrerr, ae_fabs(rr1.ptr.p_double[i]-rr2.ptr.p_double[i], _state), _state);
            }
        }
    }
    referrors = referrors||ae_fp_greater(referr,errtol);
    refrerrors = refrerrors||ae_fp_greater(refrerr,errtol);
    
    /*
     * end
     */
    waserrors = referrors||refrerrors;
    if( !silent )
    {
        printf("TESTING CORRELATION\n");
        printf("FINAL RESULT:                             ");
        if( waserrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* AGAINST REFERENCE COMPLEX CORR:         ");
        if( referrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* AGAINST REFERENCE REAL CORR:            ");
        if( refrerrors )
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
ae_bool _pexec_testcorr(ae_bool silent, ae_state *_state)
{
    return testcorr(silent, _state);
}


/*************************************************************************
Reference implementation
*************************************************************************/
static void testcorrunit_refcorrc1d(/* Complex */ ae_vector* signal,
     ae_int_t n,
     /* Complex */ ae_vector* pattern,
     ae_int_t m,
     /* Complex */ ae_vector* r,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_complex v;
    ae_vector s;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(r);
    ae_vector_init(&s, 0, DT_COMPLEX, _state);

    ae_vector_set_length(&s, m+n-1, _state);
    ae_v_cmove(&s.ptr.p_complex[0], 1, &signal->ptr.p_complex[0], 1, "N", ae_v_len(0,n-1));
    for(i=n; i<=m+n-2; i++)
    {
        s.ptr.p_complex[i] = ae_complex_from_i(0);
    }
    ae_vector_set_length(r, m+n-1, _state);
    for(i=0; i<=n-1; i++)
    {
        v = ae_complex_from_i(0);
        for(j=0; j<=m-1; j++)
        {
            if( i+j>=n )
            {
                break;
            }
            v = ae_c_add(v,ae_c_mul(ae_c_conj(pattern->ptr.p_complex[j], _state),s.ptr.p_complex[i+j]));
        }
        r->ptr.p_complex[i] = v;
    }
    for(i=1; i<=m-1; i++)
    {
        v = ae_complex_from_i(0);
        for(j=i; j<=m-1; j++)
        {
            v = ae_c_add(v,ae_c_mul(ae_c_conj(pattern->ptr.p_complex[j], _state),s.ptr.p_complex[j-i]));
        }
        r->ptr.p_complex[m+n-1-i] = v;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Reference implementation
*************************************************************************/
static void testcorrunit_refcorrc1dcircular(/* Complex */ ae_vector* signal,
     ae_int_t n,
     /* Complex */ ae_vector* pattern,
     ae_int_t m,
     /* Complex */ ae_vector* r,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_complex v;

    ae_vector_clear(r);

    ae_vector_set_length(r, n, _state);
    for(i=0; i<=n-1; i++)
    {
        v = ae_complex_from_i(0);
        for(j=0; j<=m-1; j++)
        {
            v = ae_c_add(v,ae_c_mul(ae_c_conj(pattern->ptr.p_complex[j], _state),signal->ptr.p_complex[(i+j)%n]));
        }
        r->ptr.p_complex[i] = v;
    }
}


/*************************************************************************
Reference implementation
*************************************************************************/
static void testcorrunit_refcorrr1d(/* Real    */ ae_vector* signal,
     ae_int_t n,
     /* Real    */ ae_vector* pattern,
     ae_int_t m,
     /* Real    */ ae_vector* r,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_vector s;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(r);
    ae_vector_init(&s, 0, DT_REAL, _state);

    ae_vector_set_length(&s, m+n-1, _state);
    ae_v_move(&s.ptr.p_double[0], 1, &signal->ptr.p_double[0], 1, ae_v_len(0,n-1));
    for(i=n; i<=m+n-2; i++)
    {
        s.ptr.p_double[i] = (double)(0);
    }
    ae_vector_set_length(r, m+n-1, _state);
    for(i=0; i<=n-1; i++)
    {
        v = (double)(0);
        for(j=0; j<=m-1; j++)
        {
            if( i+j>=n )
            {
                break;
            }
            v = v+pattern->ptr.p_double[j]*s.ptr.p_double[i+j];
        }
        r->ptr.p_double[i] = v;
    }
    for(i=1; i<=m-1; i++)
    {
        v = (double)(0);
        for(j=i; j<=m-1; j++)
        {
            v = v+pattern->ptr.p_double[j]*s.ptr.p_double[-i+j];
        }
        r->ptr.p_double[m+n-1-i] = v;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Reference implementation
*************************************************************************/
static void testcorrunit_refcorrr1dcircular(/* Real    */ ae_vector* signal,
     ae_int_t n,
     /* Real    */ ae_vector* pattern,
     ae_int_t m,
     /* Real    */ ae_vector* r,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;

    ae_vector_clear(r);

    ae_vector_set_length(r, n, _state);
    for(i=0; i<=n-1; i++)
    {
        v = (double)(0);
        for(j=0; j<=m-1; j++)
        {
            v = v+pattern->ptr.p_double[j]*signal->ptr.p_double[(i+j)%n];
        }
        r->ptr.p_double[i] = v;
    }
}


/*$ End $*/
