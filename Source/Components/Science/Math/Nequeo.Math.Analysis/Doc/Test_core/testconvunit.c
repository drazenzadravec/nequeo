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
#include "testconvunit.h"


/*$ Declarations $*/
static void testconvunit_refconvc1d(/* Complex */ ae_vector* a,
     ae_int_t m,
     /* Complex */ ae_vector* b,
     ae_int_t n,
     /* Complex */ ae_vector* r,
     ae_state *_state);
static void testconvunit_refconvc1dcircular(/* Complex */ ae_vector* a,
     ae_int_t m,
     /* Complex */ ae_vector* b,
     ae_int_t n,
     /* Complex */ ae_vector* r,
     ae_state *_state);
static void testconvunit_refconvr1d(/* Real    */ ae_vector* a,
     ae_int_t m,
     /* Real    */ ae_vector* b,
     ae_int_t n,
     /* Real    */ ae_vector* r,
     ae_state *_state);
static void testconvunit_refconvr1dcircular(/* Real    */ ae_vector* a,
     ae_int_t m,
     /* Real    */ ae_vector* b,
     ae_int_t n,
     /* Real    */ ae_vector* r,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Test
*************************************************************************/
ae_bool testconv(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t m;
    ae_int_t n;
    ae_int_t i;
    ae_int_t rkind;
    ae_int_t circkind;
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
    double inverr;
    double invrerr;
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
     *
     * Automatic ConvC1D() and different algorithms of ConvC1DX() are tested.
     */
    referr = (double)(0);
    refrerr = (double)(0);
    for(m=1; m<=maxn; m++)
    {
        for(n=1; n<=maxn; n++)
        {
            for(circkind=0; circkind<=1; circkind++)
            {
                for(rkind=-3; rkind<=1; rkind++)
                {
                    
                    /*
                     * skip impossible combinations of parameters:
                     * * circular convolution, M<N, RKind<>-3 - internal subroutine does not support M<N.
                     */
                    if( (circkind!=0&&m<n)&&rkind!=-3 )
                    {
                        continue;
                    }
                    
                    /*
                     * Complex convolution
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
                    if( rkind==-3 )
                    {
                        
                        /*
                         * test wrapper subroutine:
                         * * circular/non-circular
                         */
                        if( circkind==0 )
                        {
                            convc1d(&ca, m, &cb, n, &cr1, _state);
                        }
                        else
                        {
                            convc1dcircular(&ca, m, &cb, n, &cr1, _state);
                        }
                    }
                    else
                    {
                        
                        /*
                         * test internal subroutine
                         */
                        if( m>=n )
                        {
                            
                            /*
                             * test internal subroutine:
                             * * circular/non-circular mode
                             */
                            convc1dx(&ca, m, &cb, n, circkind!=0, rkind, 0, &cr1, _state);
                        }
                        else
                        {
                            
                            /*
                             * test internal subroutine - circular mode only
                             */
                            ae_assert(circkind==0, "Convolution test: internal error!", _state);
                            convc1dx(&cb, n, &ca, m, ae_false, rkind, 0, &cr1, _state);
                        }
                    }
                    if( circkind==0 )
                    {
                        testconvunit_refconvc1d(&ca, m, &cb, n, &cr2, _state);
                    }
                    else
                    {
                        testconvunit_refconvc1dcircular(&ca, m, &cb, n, &cr2, _state);
                    }
                    if( circkind==0 )
                    {
                        for(i=0; i<=m+n-2; i++)
                        {
                            referr = ae_maxreal(referr, ae_c_abs(ae_c_sub(cr1.ptr.p_complex[i],cr2.ptr.p_complex[i]), _state), _state);
                        }
                    }
                    else
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            referr = ae_maxreal(referr, ae_c_abs(ae_c_sub(cr1.ptr.p_complex[i],cr2.ptr.p_complex[i]), _state), _state);
                        }
                    }
                    
                    /*
                     * Real convolution
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
                    if( rkind==-3 )
                    {
                        
                        /*
                         * test wrapper subroutine:
                         * * circular/non-circular
                         */
                        if( circkind==0 )
                        {
                            convr1d(&ra, m, &rb, n, &rr1, _state);
                        }
                        else
                        {
                            convr1dcircular(&ra, m, &rb, n, &rr1, _state);
                        }
                    }
                    else
                    {
                        if( m>=n )
                        {
                            
                            /*
                             * test internal subroutine:
                             * * circular/non-circular mode
                             */
                            convr1dx(&ra, m, &rb, n, circkind!=0, rkind, 0, &rr1, _state);
                        }
                        else
                        {
                            
                            /*
                             * test internal subroutine - non-circular mode only
                             */
                            convr1dx(&rb, n, &ra, m, circkind!=0, rkind, 0, &rr1, _state);
                        }
                    }
                    if( circkind==0 )
                    {
                        testconvunit_refconvr1d(&ra, m, &rb, n, &rr2, _state);
                    }
                    else
                    {
                        testconvunit_refconvr1dcircular(&ra, m, &rb, n, &rr2, _state);
                    }
                    if( circkind==0 )
                    {
                        for(i=0; i<=m+n-2; i++)
                        {
                            refrerr = ae_maxreal(refrerr, ae_fabs(rr1.ptr.p_double[i]-rr2.ptr.p_double[i], _state), _state);
                        }
                    }
                    else
                    {
                        for(i=0; i<=m-1; i++)
                        {
                            refrerr = ae_maxreal(refrerr, ae_fabs(rr1.ptr.p_double[i]-rr2.ptr.p_double[i], _state), _state);
                        }
                    }
                }
            }
        }
    }
    referrors = referrors||ae_fp_greater(referr,errtol);
    refrerrors = refrerrors||ae_fp_greater(refrerr,errtol);
    
    /*
     * Test inverse convolution
     */
    inverr = (double)(0);
    invrerr = (double)(0);
    for(m=1; m<=maxn; m++)
    {
        for(n=1; n<=maxn; n++)
        {
            
            /*
             * Complex circilar and non-circular
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
            ae_vector_set_length(&cr2, 1, _state);
            convc1d(&ca, m, &cb, n, &cr2, _state);
            convc1dinv(&cr2, m+n-1, &cb, n, &cr1, _state);
            for(i=0; i<=m-1; i++)
            {
                inverr = ae_maxreal(inverr, ae_c_abs(ae_c_sub(cr1.ptr.p_complex[i],ca.ptr.p_complex[i]), _state), _state);
            }
            ae_vector_set_length(&cr1, 1, _state);
            ae_vector_set_length(&cr2, 1, _state);
            convc1dcircular(&ca, m, &cb, n, &cr2, _state);
            convc1dcircularinv(&cr2, m, &cb, n, &cr1, _state);
            for(i=0; i<=m-1; i++)
            {
                inverr = ae_maxreal(inverr, ae_c_abs(ae_c_sub(cr1.ptr.p_complex[i],ca.ptr.p_complex[i]), _state), _state);
            }
            
            /*
             * Real circilar and non-circular
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
            ae_vector_set_length(&rr2, 1, _state);
            convr1d(&ra, m, &rb, n, &rr2, _state);
            convr1dinv(&rr2, m+n-1, &rb, n, &rr1, _state);
            for(i=0; i<=m-1; i++)
            {
                invrerr = ae_maxreal(invrerr, ae_fabs(rr1.ptr.p_double[i]-ra.ptr.p_double[i], _state), _state);
            }
            ae_vector_set_length(&rr1, 1, _state);
            ae_vector_set_length(&rr2, 1, _state);
            convr1dcircular(&ra, m, &rb, n, &rr2, _state);
            convr1dcircularinv(&rr2, m, &rb, n, &rr1, _state);
            for(i=0; i<=m-1; i++)
            {
                invrerr = ae_maxreal(invrerr, ae_fabs(rr1.ptr.p_double[i]-ra.ptr.p_double[i], _state), _state);
            }
        }
    }
    inverrors = inverrors||ae_fp_greater(inverr,errtol);
    invrerrors = invrerrors||ae_fp_greater(invrerr,errtol);
    
    /*
     * end
     */
    waserrors = ((referrors||refrerrors)||inverrors)||invrerrors;
    if( !silent )
    {
        printf("TESTING CONVOLUTION\n");
        printf("FINAL RESULT:                             ");
        if( waserrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* AGAINST REFERENCE COMPLEX CONV:         ");
        if( referrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* AGAINST REFERENCE REAL CONV:            ");
        if( refrerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* COMPLEX INVERSE:                        ");
        if( inverrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* REAL INVERSE:                           ");
        if( invrerrors )
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
ae_bool _pexec_testconv(ae_bool silent, ae_state *_state)
{
    return testconv(silent, _state);
}


/*************************************************************************
Reference implementation
*************************************************************************/
static void testconvunit_refconvc1d(/* Complex */ ae_vector* a,
     ae_int_t m,
     /* Complex */ ae_vector* b,
     ae_int_t n,
     /* Complex */ ae_vector* r,
     ae_state *_state)
{
    ae_int_t i;
    ae_complex v;

    ae_vector_clear(r);

    ae_vector_set_length(r, m+n-1, _state);
    for(i=0; i<=m+n-2; i++)
    {
        r->ptr.p_complex[i] = ae_complex_from_i(0);
    }
    for(i=0; i<=m-1; i++)
    {
        v = a->ptr.p_complex[i];
        ae_v_caddc(&r->ptr.p_complex[i], 1, &b->ptr.p_complex[0], 1, "N", ae_v_len(i,i+n-1), v);
    }
}


/*************************************************************************
Reference implementation
*************************************************************************/
static void testconvunit_refconvc1dcircular(/* Complex */ ae_vector* a,
     ae_int_t m,
     /* Complex */ ae_vector* b,
     ae_int_t n,
     /* Complex */ ae_vector* r,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i1;
    ae_int_t i2;
    ae_int_t j2;
    ae_vector buf;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(r);
    ae_vector_init(&buf, 0, DT_COMPLEX, _state);

    testconvunit_refconvc1d(a, m, b, n, &buf, _state);
    ae_vector_set_length(r, m, _state);
    ae_v_cmove(&r->ptr.p_complex[0], 1, &buf.ptr.p_complex[0], 1, "N", ae_v_len(0,m-1));
    i1 = m;
    while(i1<=m+n-2)
    {
        i2 = ae_minint(i1+m-1, m+n-2, _state);
        j2 = i2-i1;
        ae_v_cadd(&r->ptr.p_complex[0], 1, &buf.ptr.p_complex[i1], 1, "N", ae_v_len(0,j2));
        i1 = i1+m;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Reference FFT
*************************************************************************/
static void testconvunit_refconvr1d(/* Real    */ ae_vector* a,
     ae_int_t m,
     /* Real    */ ae_vector* b,
     ae_int_t n,
     /* Real    */ ae_vector* r,
     ae_state *_state)
{
    ae_int_t i;
    double v;

    ae_vector_clear(r);

    ae_vector_set_length(r, m+n-1, _state);
    for(i=0; i<=m+n-2; i++)
    {
        r->ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=m-1; i++)
    {
        v = a->ptr.p_double[i];
        ae_v_addd(&r->ptr.p_double[i], 1, &b->ptr.p_double[0], 1, ae_v_len(i,i+n-1), v);
    }
}


/*************************************************************************
Reference implementation
*************************************************************************/
static void testconvunit_refconvr1dcircular(/* Real    */ ae_vector* a,
     ae_int_t m,
     /* Real    */ ae_vector* b,
     ae_int_t n,
     /* Real    */ ae_vector* r,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i1;
    ae_int_t i2;
    ae_int_t j2;
    ae_vector buf;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(r);
    ae_vector_init(&buf, 0, DT_REAL, _state);

    testconvunit_refconvr1d(a, m, b, n, &buf, _state);
    ae_vector_set_length(r, m, _state);
    ae_v_move(&r->ptr.p_double[0], 1, &buf.ptr.p_double[0], 1, ae_v_len(0,m-1));
    i1 = m;
    while(i1<=m+n-2)
    {
        i2 = ae_minint(i1+m-1, m+n-2, _state);
        j2 = i2-i1;
        ae_v_add(&r->ptr.p_double[0], 1, &buf.ptr.p_double[i1], 1, ae_v_len(0,j2));
        i1 = i1+m;
    }
    ae_frame_leave(_state);
}


/*$ End $*/
