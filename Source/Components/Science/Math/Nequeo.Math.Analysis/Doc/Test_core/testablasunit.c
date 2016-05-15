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
#include "testablasunit.h"


/*$ Declarations $*/
static ae_bool testablasunit_testtrsm(ae_int_t minn,
     ae_int_t maxn,
     ae_state *_state);
static ae_bool testablasunit_testsyrk(ae_int_t minn,
     ae_int_t maxn,
     ae_state *_state);
static ae_bool testablasunit_testgemm(ae_int_t minn,
     ae_int_t maxn,
     ae_state *_state);
static ae_bool testablasunit_testtrans(ae_int_t minn,
     ae_int_t maxn,
     ae_state *_state);
static ae_bool testablasunit_testrank1(ae_int_t minn,
     ae_int_t maxn,
     ae_state *_state);
static ae_bool testablasunit_testmv(ae_int_t minn,
     ae_int_t maxn,
     ae_state *_state);
static ae_bool testablasunit_testcopy(ae_int_t minn,
     ae_int_t maxn,
     ae_state *_state);
static void testablasunit_refcmatrixrighttrsm(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Complex */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state);
static void testablasunit_refcmatrixlefttrsm(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Complex */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state);
static void testablasunit_refrmatrixrighttrsm(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Real    */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state);
static void testablasunit_refrmatrixlefttrsm(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Real    */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state);
static ae_bool testablasunit_internalcmatrixtrinverse(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunittriangular,
     ae_state *_state);
static ae_bool testablasunit_internalrmatrixtrinverse(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunittriangular,
     ae_state *_state);
static void testablasunit_refcmatrixherk(ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     double beta,
     /* Complex */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_bool isupper,
     ae_state *_state);
static void testablasunit_refrmatrixsyrk(ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     double beta,
     /* Real    */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_bool isupper,
     ae_state *_state);
static void testablasunit_refcmatrixgemm(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     ae_complex alpha,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     /* Complex */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     ae_int_t optypeb,
     ae_complex beta,
     /* Complex */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_state *_state);
static void testablasunit_refrmatrixgemm(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     /* Real    */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     ae_int_t optypeb,
     double beta,
     /* Real    */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_state *_state);


/*$ Body $*/


ae_bool testablas(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    double threshold;
    ae_bool trsmerrors;
    ae_bool syrkerrors;
    ae_bool gemmerrors;
    ae_bool transerrors;
    ae_bool rank1errors;
    ae_bool mverrors;
    ae_bool copyerrors;
    ae_bool waserrors;
    ae_matrix ra;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&ra, 0, 0, DT_REAL, _state);

    trsmerrors = ae_false;
    syrkerrors = ae_false;
    gemmerrors = ae_false;
    transerrors = ae_false;
    rank1errors = ae_false;
    mverrors = ae_false;
    copyerrors = ae_false;
    waserrors = ae_false;
    threshold = 10000*ae_machineepsilon;
    trsmerrors = trsmerrors||testablasunit_testtrsm(1, 3*ablasblocksize(&ra, _state)+1, _state);
    syrkerrors = syrkerrors||testablasunit_testsyrk(1, 3*ablasblocksize(&ra, _state)+1, _state);
    gemmerrors = gemmerrors||testablasunit_testgemm(1, 3*ablasblocksize(&ra, _state)+1, _state);
    transerrors = transerrors||testablasunit_testtrans(1, 3*ablasblocksize(&ra, _state)+1, _state);
    rank1errors = rank1errors||testablasunit_testrank1(1, 3*ablasblocksize(&ra, _state)+1, _state);
    mverrors = mverrors||testablasunit_testmv(1, 3*ablasblocksize(&ra, _state)+1, _state);
    copyerrors = copyerrors||testablasunit_testcopy(1, 3*ablasblocksize(&ra, _state)+1, _state);
    gemmerrors = gemmerrors||testablasunit_testgemm(8*ablasblocksize(&ra, _state)-1, 8*ablasblocksize(&ra, _state)+1, _state);
    
    /*
     * report
     */
    waserrors = (((((trsmerrors||syrkerrors)||gemmerrors)||transerrors)||rank1errors)||mverrors)||copyerrors;
    if( !silent )
    {
        printf("TESTING ABLAS\n");
        printf("* TRSM:                                  ");
        if( trsmerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* SYRK:                                  ");
        if( syrkerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* GEMM:                                  ");
        if( gemmerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* TRANS:                                 ");
        if( transerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* RANK1:                                 ");
        if( rank1errors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* MV:                                    ");
        if( mverrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* COPY:                                  ");
        if( copyerrors )
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
        printf("\n\n");
    }
    result = !waserrors;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testablas(ae_bool silent, ae_state *_state)
{
    return testablas(silent, _state);
}


/*************************************************************************
?Matrix????TRSM tests

Returns False for passed test, True - for failed
*************************************************************************/
static ae_bool testablasunit_testtrsm(ae_int_t minn,
     ae_int_t maxn,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t m;
    ae_int_t mx;
    ae_int_t i;
    ae_int_t j;
    ae_int_t optype;
    ae_int_t uppertype;
    ae_int_t unittype;
    ae_int_t xoffsi;
    ae_int_t xoffsj;
    ae_int_t aoffsitype;
    ae_int_t aoffsjtype;
    ae_int_t aoffsi;
    ae_int_t aoffsj;
    ae_matrix refra;
    ae_matrix refrxl;
    ae_matrix refrxr;
    ae_matrix refca;
    ae_matrix refcxl;
    ae_matrix refcxr;
    ae_matrix ra;
    ae_matrix ca;
    ae_matrix rxr1;
    ae_matrix rxl1;
    ae_matrix cxr1;
    ae_matrix cxl1;
    ae_matrix rxr2;
    ae_matrix rxl2;
    ae_matrix cxr2;
    ae_matrix cxl2;
    double threshold;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&refra, 0, 0, DT_REAL, _state);
    ae_matrix_init(&refrxl, 0, 0, DT_REAL, _state);
    ae_matrix_init(&refrxr, 0, 0, DT_REAL, _state);
    ae_matrix_init(&refca, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&refcxl, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&refcxr, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&ra, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ca, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&rxr1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&rxl1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cxr1, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&cxl1, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&rxr2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&rxl2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cxr2, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&cxl2, 0, 0, DT_COMPLEX, _state);

    threshold = ae_sqr((double)(maxn), _state)*100*ae_machineepsilon;
    result = ae_false;
    for(mx=minn; mx<=maxn; mx++)
    {
        
        /*
         * Select random M/N in [1,MX] such that max(M,N)=MX
         */
        m = 1+ae_randominteger(mx, _state);
        n = 1+ae_randominteger(mx, _state);
        if( ae_fp_greater(ae_randomreal(_state),0.5) )
        {
            m = mx;
        }
        else
        {
            n = mx;
        }
        
        /*
         * Initialize RefRA/RefCA by random matrices whose upper
         * and lower triangle submatrices are non-degenerate
         * well-conditioned matrices.
         *
         * Matrix size is 2Mx2M (four copies of same MxM matrix
         * to test different offsets)
         */
        ae_matrix_set_length(&refra, 2*m, 2*m, _state);
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                refra.ptr.pp_double[i][j] = 0.2*ae_randomreal(_state)-0.1;
            }
        }
        for(i=0; i<=m-1; i++)
        {
            refra.ptr.pp_double[i][i] = (2*ae_randominteger(1, _state)-1)*(2*m+ae_randomreal(_state));
        }
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                refra.ptr.pp_double[i+m][j] = refra.ptr.pp_double[i][j];
                refra.ptr.pp_double[i][j+m] = refra.ptr.pp_double[i][j];
                refra.ptr.pp_double[i+m][j+m] = refra.ptr.pp_double[i][j];
            }
        }
        ae_matrix_set_length(&refca, 2*m, 2*m, _state);
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                refca.ptr.pp_complex[i][j].x = 0.2*ae_randomreal(_state)-0.1;
                refca.ptr.pp_complex[i][j].y = 0.2*ae_randomreal(_state)-0.1;
            }
        }
        for(i=0; i<=m-1; i++)
        {
            refca.ptr.pp_complex[i][i].x = (2*ae_randominteger(2, _state)-1)*(2*m+ae_randomreal(_state));
            refca.ptr.pp_complex[i][i].y = (2*ae_randominteger(2, _state)-1)*(2*m+ae_randomreal(_state));
        }
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                refca.ptr.pp_complex[i+m][j] = refca.ptr.pp_complex[i][j];
                refca.ptr.pp_complex[i][j+m] = refca.ptr.pp_complex[i][j];
                refca.ptr.pp_complex[i+m][j+m] = refca.ptr.pp_complex[i][j];
            }
        }
        
        /*
         * Generate random XL/XR.
         *
         * XR is NxM matrix (matrix for 'Right' subroutines)
         * XL is MxN matrix (matrix for 'Left' subroutines)
         */
        ae_matrix_set_length(&refrxr, n, m, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                refrxr.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
        }
        ae_matrix_set_length(&refrxl, m, n, _state);
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                refrxl.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
        }
        ae_matrix_set_length(&refcxr, n, m, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                refcxr.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                refcxr.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
            }
        }
        ae_matrix_set_length(&refcxl, m, n, _state);
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                refcxl.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                refcxl.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
            }
        }
        
        /*
         * test different types of operations, offsets, and so on...
         *
         * to avoid unnecessary slowdown we don't test ALL possible
         * combinations of operation types. We just generate one random
         * set of parameters and test it.
         */
        ae_matrix_set_length(&ra, 2*m, 2*m, _state);
        ae_matrix_set_length(&rxr1, n, m, _state);
        ae_matrix_set_length(&rxr2, n, m, _state);
        ae_matrix_set_length(&rxl1, m, n, _state);
        ae_matrix_set_length(&rxl2, m, n, _state);
        ae_matrix_set_length(&ca, 2*m, 2*m, _state);
        ae_matrix_set_length(&cxr1, n, m, _state);
        ae_matrix_set_length(&cxr2, n, m, _state);
        ae_matrix_set_length(&cxl1, m, n, _state);
        ae_matrix_set_length(&cxl2, m, n, _state);
        optype = ae_randominteger(3, _state);
        uppertype = ae_randominteger(2, _state);
        unittype = ae_randominteger(2, _state);
        xoffsi = ae_randominteger(2, _state);
        xoffsj = ae_randominteger(2, _state);
        aoffsitype = ae_randominteger(2, _state);
        aoffsjtype = ae_randominteger(2, _state);
        aoffsi = m*aoffsitype;
        aoffsj = m*aoffsjtype;
        
        /*
         * copy A, XR, XL (fill unused parts with random garbage)
         */
        for(i=0; i<=2*m-1; i++)
        {
            for(j=0; j<=2*m-1; j++)
            {
                if( ((i>=aoffsi&&i<aoffsi+m)&&j>=aoffsj)&&j<aoffsj+m )
                {
                    ca.ptr.pp_complex[i][j] = refca.ptr.pp_complex[i][j];
                    ra.ptr.pp_double[i][j] = refra.ptr.pp_double[i][j];
                }
                else
                {
                    ca.ptr.pp_complex[i][j] = ae_complex_from_d(ae_randomreal(_state));
                    ra.ptr.pp_double[i][j] = ae_randomreal(_state);
                }
            }
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                if( i>=xoffsi&&j>=xoffsj )
                {
                    cxr1.ptr.pp_complex[i][j] = refcxr.ptr.pp_complex[i][j];
                    cxr2.ptr.pp_complex[i][j] = refcxr.ptr.pp_complex[i][j];
                    rxr1.ptr.pp_double[i][j] = refrxr.ptr.pp_double[i][j];
                    rxr2.ptr.pp_double[i][j] = refrxr.ptr.pp_double[i][j];
                }
                else
                {
                    cxr1.ptr.pp_complex[i][j] = ae_complex_from_d(ae_randomreal(_state));
                    cxr2.ptr.pp_complex[i][j] = cxr1.ptr.pp_complex[i][j];
                    rxr1.ptr.pp_double[i][j] = ae_randomreal(_state);
                    rxr2.ptr.pp_double[i][j] = rxr1.ptr.pp_double[i][j];
                }
            }
        }
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( i>=xoffsi&&j>=xoffsj )
                {
                    cxl1.ptr.pp_complex[i][j] = refcxl.ptr.pp_complex[i][j];
                    cxl2.ptr.pp_complex[i][j] = refcxl.ptr.pp_complex[i][j];
                    rxl1.ptr.pp_double[i][j] = refrxl.ptr.pp_double[i][j];
                    rxl2.ptr.pp_double[i][j] = refrxl.ptr.pp_double[i][j];
                }
                else
                {
                    cxl1.ptr.pp_complex[i][j] = ae_complex_from_d(ae_randomreal(_state));
                    cxl2.ptr.pp_complex[i][j] = cxl1.ptr.pp_complex[i][j];
                    rxl1.ptr.pp_double[i][j] = ae_randomreal(_state);
                    rxl2.ptr.pp_double[i][j] = rxl1.ptr.pp_double[i][j];
                }
            }
        }
        
        /*
         * Test CXR
         */
        cmatrixrighttrsm(n-xoffsi, m-xoffsj, &ca, aoffsi, aoffsj, uppertype==0, unittype==0, optype, &cxr1, xoffsi, xoffsj, _state);
        testablasunit_refcmatrixrighttrsm(n-xoffsi, m-xoffsj, &ca, aoffsi, aoffsj, uppertype==0, unittype==0, optype, &cxr2, xoffsi, xoffsj, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                result = result||ae_fp_greater(ae_c_abs(ae_c_sub(cxr1.ptr.pp_complex[i][j],cxr2.ptr.pp_complex[i][j]), _state),threshold);
            }
        }
        
        /*
         * Test CXL
         */
        cmatrixlefttrsm(m-xoffsi, n-xoffsj, &ca, aoffsi, aoffsj, uppertype==0, unittype==0, optype, &cxl1, xoffsi, xoffsj, _state);
        testablasunit_refcmatrixlefttrsm(m-xoffsi, n-xoffsj, &ca, aoffsi, aoffsj, uppertype==0, unittype==0, optype, &cxl2, xoffsi, xoffsj, _state);
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                result = result||ae_fp_greater(ae_c_abs(ae_c_sub(cxl1.ptr.pp_complex[i][j],cxl2.ptr.pp_complex[i][j]), _state),threshold);
            }
        }
        if( optype<2 )
        {
            
            /*
             * Test RXR
             */
            rmatrixrighttrsm(n-xoffsi, m-xoffsj, &ra, aoffsi, aoffsj, uppertype==0, unittype==0, optype, &rxr1, xoffsi, xoffsj, _state);
            testablasunit_refrmatrixrighttrsm(n-xoffsi, m-xoffsj, &ra, aoffsi, aoffsj, uppertype==0, unittype==0, optype, &rxr2, xoffsi, xoffsj, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=m-1; j++)
                {
                    result = result||ae_fp_greater(ae_fabs(rxr1.ptr.pp_double[i][j]-rxr2.ptr.pp_double[i][j], _state),threshold);
                }
            }
            
            /*
             * Test RXL
             */
            rmatrixlefttrsm(m-xoffsi, n-xoffsj, &ra, aoffsi, aoffsj, uppertype==0, unittype==0, optype, &rxl1, xoffsi, xoffsj, _state);
            testablasunit_refrmatrixlefttrsm(m-xoffsi, n-xoffsj, &ra, aoffsi, aoffsj, uppertype==0, unittype==0, optype, &rxl2, xoffsi, xoffsj, _state);
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    result = result||ae_fp_greater(ae_fabs(rxl1.ptr.pp_double[i][j]-rxl2.ptr.pp_double[i][j], _state),threshold);
                }
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
SYRK tests

Returns False for passed test, True - for failed
*************************************************************************/
static ae_bool testablasunit_testsyrk(ae_int_t minn,
     ae_int_t maxn,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t k;
    ae_int_t mx;
    ae_int_t i;
    ae_int_t j;
    ae_int_t uppertype;
    ae_int_t xoffsi;
    ae_int_t xoffsj;
    ae_int_t aoffsitype;
    ae_int_t aoffsjtype;
    ae_int_t aoffsi;
    ae_int_t aoffsj;
    ae_int_t alphatype;
    ae_int_t betatype;
    ae_matrix refra;
    ae_matrix refrc;
    ae_matrix refca;
    ae_matrix refcc;
    double alpha;
    double beta;
    ae_matrix ra1;
    ae_matrix ra2;
    ae_matrix ca1;
    ae_matrix ca2;
    ae_matrix rc;
    ae_matrix rct;
    ae_matrix cc;
    ae_matrix cct;
    double threshold;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&refra, 0, 0, DT_REAL, _state);
    ae_matrix_init(&refrc, 0, 0, DT_REAL, _state);
    ae_matrix_init(&refca, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&refcc, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&ra1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ra2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ca1, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&ca2, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&rc, 0, 0, DT_REAL, _state);
    ae_matrix_init(&rct, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cc, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&cct, 0, 0, DT_COMPLEX, _state);

    threshold = maxn*100*ae_machineepsilon;
    result = ae_false;
    for(mx=minn; mx<=maxn; mx++)
    {
        
        /*
         * Select random M/N in [1,MX] such that max(M,N)=MX
         */
        k = 1+ae_randominteger(mx, _state);
        n = 1+ae_randominteger(mx, _state);
        if( ae_fp_greater(ae_randomreal(_state),0.5) )
        {
            k = mx;
        }
        else
        {
            n = mx;
        }
        
        /*
         * Initialize RefRA/RefCA by random Hermitian matrices,
         * RefRC/RefCC by random matrices
         *
         * RA/CA size is 2Nx2N (four copies of same NxN matrix
         * to test different offsets)
         */
        ae_matrix_set_length(&refra, 2*n, 2*n, _state);
        ae_matrix_set_length(&refca, 2*n, 2*n, _state);
        for(i=0; i<=n-1; i++)
        {
            refra.ptr.pp_double[i][i] = 2*ae_randomreal(_state)-1;
            refca.ptr.pp_complex[i][i] = ae_complex_from_d(2*ae_randomreal(_state)-1);
            for(j=i+1; j<=n-1; j++)
            {
                refra.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                refca.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                refca.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                refra.ptr.pp_double[j][i] = refra.ptr.pp_double[i][j];
                refca.ptr.pp_complex[j][i] = ae_c_conj(refca.ptr.pp_complex[i][j], _state);
            }
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                refra.ptr.pp_double[i+n][j] = refra.ptr.pp_double[i][j];
                refra.ptr.pp_double[i][j+n] = refra.ptr.pp_double[i][j];
                refra.ptr.pp_double[i+n][j+n] = refra.ptr.pp_double[i][j];
                refca.ptr.pp_complex[i+n][j] = refca.ptr.pp_complex[i][j];
                refca.ptr.pp_complex[i][j+n] = refca.ptr.pp_complex[i][j];
                refca.ptr.pp_complex[i+n][j+n] = refca.ptr.pp_complex[i][j];
            }
        }
        ae_matrix_set_length(&refrc, n, k, _state);
        ae_matrix_set_length(&refcc, n, k, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=k-1; j++)
            {
                refrc.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                refcc.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                refcc.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
            }
        }
        
        /*
         * test different types of operations, offsets, and so on...
         *
         * to avoid unnecessary slowdown we don't test ALL possible
         * combinations of operation types. We just generate one random
         * set of parameters and test it.
         */
        ae_matrix_set_length(&ra1, 2*n, 2*n, _state);
        ae_matrix_set_length(&ra2, 2*n, 2*n, _state);
        ae_matrix_set_length(&ca1, 2*n, 2*n, _state);
        ae_matrix_set_length(&ca2, 2*n, 2*n, _state);
        ae_matrix_set_length(&rc, n, k, _state);
        ae_matrix_set_length(&rct, k, n, _state);
        ae_matrix_set_length(&cc, n, k, _state);
        ae_matrix_set_length(&cct, k, n, _state);
        uppertype = ae_randominteger(2, _state);
        xoffsi = ae_randominteger(2, _state);
        xoffsj = ae_randominteger(2, _state);
        aoffsitype = ae_randominteger(2, _state);
        aoffsjtype = ae_randominteger(2, _state);
        alphatype = ae_randominteger(2, _state);
        betatype = ae_randominteger(2, _state);
        aoffsi = n*aoffsitype;
        aoffsj = n*aoffsjtype;
        alpha = alphatype*(2*ae_randomreal(_state)-1);
        beta = betatype*(2*ae_randomreal(_state)-1);
        
        /*
         * copy A, C (fill unused parts with random garbage)
         */
        for(i=0; i<=2*n-1; i++)
        {
            for(j=0; j<=2*n-1; j++)
            {
                if( ((i>=aoffsi&&i<aoffsi+n)&&j>=aoffsj)&&j<aoffsj+n )
                {
                    ca1.ptr.pp_complex[i][j] = refca.ptr.pp_complex[i][j];
                    ca2.ptr.pp_complex[i][j] = refca.ptr.pp_complex[i][j];
                    ra1.ptr.pp_double[i][j] = refra.ptr.pp_double[i][j];
                    ra2.ptr.pp_double[i][j] = refra.ptr.pp_double[i][j];
                }
                else
                {
                    ca1.ptr.pp_complex[i][j] = ae_complex_from_d(ae_randomreal(_state));
                    ca2.ptr.pp_complex[i][j] = ca1.ptr.pp_complex[i][j];
                    ra1.ptr.pp_double[i][j] = ae_randomreal(_state);
                    ra2.ptr.pp_double[i][j] = ra1.ptr.pp_double[i][j];
                }
            }
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=k-1; j++)
            {
                if( i>=xoffsi&&j>=xoffsj )
                {
                    rc.ptr.pp_double[i][j] = refrc.ptr.pp_double[i][j];
                    rct.ptr.pp_double[j][i] = refrc.ptr.pp_double[i][j];
                    cc.ptr.pp_complex[i][j] = refcc.ptr.pp_complex[i][j];
                    cct.ptr.pp_complex[j][i] = refcc.ptr.pp_complex[i][j];
                }
                else
                {
                    rc.ptr.pp_double[i][j] = ae_randomreal(_state);
                    rct.ptr.pp_double[j][i] = rc.ptr.pp_double[i][j];
                    cc.ptr.pp_complex[i][j] = ae_complex_from_d(ae_randomreal(_state));
                    cct.ptr.pp_complex[j][i] = cct.ptr.pp_complex[j][i];
                }
            }
        }
        
        /*
         * Test complex
         * Only one of transform types is selected and tested
         */
        if( ae_fp_greater(ae_randomreal(_state),0.5) )
        {
            cmatrixherk(n-xoffsi, k-xoffsj, alpha, &cc, xoffsi, xoffsj, 0, beta, &ca1, aoffsi, aoffsj, uppertype==0, _state);
            testablasunit_refcmatrixherk(n-xoffsi, k-xoffsj, alpha, &cc, xoffsi, xoffsj, 0, beta, &ca2, aoffsi, aoffsj, uppertype==0, _state);
        }
        else
        {
            cmatrixherk(n-xoffsi, k-xoffsj, alpha, &cct, xoffsj, xoffsi, 2, beta, &ca1, aoffsi, aoffsj, uppertype==0, _state);
            testablasunit_refcmatrixherk(n-xoffsi, k-xoffsj, alpha, &cct, xoffsj, xoffsi, 2, beta, &ca2, aoffsi, aoffsj, uppertype==0, _state);
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                result = result||ae_fp_greater(ae_c_abs(ae_c_sub(ca1.ptr.pp_complex[i][j],ca2.ptr.pp_complex[i][j]), _state),threshold);
            }
        }
        
        /*
         * Test old version of HERK (named SYRK)
         * Only one of transform types is selected and tested
         */
        if( ae_fp_greater(ae_randomreal(_state),0.5) )
        {
            cmatrixsyrk(n-xoffsi, k-xoffsj, alpha, &cc, xoffsi, xoffsj, 0, beta, &ca1, aoffsi, aoffsj, uppertype==0, _state);
            testablasunit_refcmatrixherk(n-xoffsi, k-xoffsj, alpha, &cc, xoffsi, xoffsj, 0, beta, &ca2, aoffsi, aoffsj, uppertype==0, _state);
        }
        else
        {
            cmatrixsyrk(n-xoffsi, k-xoffsj, alpha, &cct, xoffsj, xoffsi, 2, beta, &ca1, aoffsi, aoffsj, uppertype==0, _state);
            testablasunit_refcmatrixherk(n-xoffsi, k-xoffsj, alpha, &cct, xoffsj, xoffsi, 2, beta, &ca2, aoffsi, aoffsj, uppertype==0, _state);
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                result = result||ae_fp_greater(ae_c_abs(ae_c_sub(ca1.ptr.pp_complex[i][j],ca2.ptr.pp_complex[i][j]), _state),threshold);
            }
        }
        
        /*
         * Test real
         * Only one of transform types is selected and tested
         */
        if( ae_fp_greater(ae_randomreal(_state),0.5) )
        {
            rmatrixsyrk(n-xoffsi, k-xoffsj, alpha, &rc, xoffsi, xoffsj, 0, beta, &ra1, aoffsi, aoffsj, uppertype==0, _state);
            testablasunit_refrmatrixsyrk(n-xoffsi, k-xoffsj, alpha, &rc, xoffsi, xoffsj, 0, beta, &ra2, aoffsi, aoffsj, uppertype==0, _state);
        }
        else
        {
            rmatrixsyrk(n-xoffsi, k-xoffsj, alpha, &rct, xoffsj, xoffsi, 1, beta, &ra1, aoffsi, aoffsj, uppertype==0, _state);
            testablasunit_refrmatrixsyrk(n-xoffsi, k-xoffsj, alpha, &rct, xoffsj, xoffsi, 1, beta, &ra2, aoffsi, aoffsj, uppertype==0, _state);
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                result = result||ae_fp_greater(ae_fabs(ra1.ptr.pp_double[i][j]-ra2.ptr.pp_double[i][j], _state),threshold);
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
GEMM tests

Returns False for passed test, True - for failed
*************************************************************************/
static ae_bool testablasunit_testgemm(ae_int_t minn,
     ae_int_t maxn,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t m;
    ae_int_t n;
    ae_int_t k;
    ae_int_t mx;
    ae_int_t i;
    ae_int_t j;
    ae_int_t aoffsi;
    ae_int_t aoffsj;
    ae_int_t aoptype;
    ae_int_t aoptyper;
    ae_int_t boffsi;
    ae_int_t boffsj;
    ae_int_t boptype;
    ae_int_t boptyper;
    ae_int_t coffsi;
    ae_int_t coffsj;
    ae_matrix refra;
    ae_matrix refrb;
    ae_matrix refrc;
    ae_matrix refca;
    ae_matrix refcb;
    ae_matrix refcc;
    double alphar;
    double betar;
    ae_complex alphac;
    ae_complex betac;
    ae_matrix rc1;
    ae_matrix rc2;
    ae_matrix cc1;
    ae_matrix cc2;
    double threshold;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&refra, 0, 0, DT_REAL, _state);
    ae_matrix_init(&refrb, 0, 0, DT_REAL, _state);
    ae_matrix_init(&refrc, 0, 0, DT_REAL, _state);
    ae_matrix_init(&refca, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&refcb, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&refcc, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&rc1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&rc2, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cc1, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&cc2, 0, 0, DT_COMPLEX, _state);

    threshold = maxn*100*ae_machineepsilon;
    result = ae_false;
    for(mx=minn; mx<=maxn; mx++)
    {
        
        /*
         * Select random M/N/K in [1,MX] such that max(M,N,K)=MX
         */
        m = 1+ae_randominteger(mx, _state);
        n = 1+ae_randominteger(mx, _state);
        k = 1+ae_randominteger(mx, _state);
        i = ae_randominteger(3, _state);
        if( i==0 )
        {
            m = mx;
        }
        if( i==1 )
        {
            n = mx;
        }
        if( i==2 )
        {
            k = mx;
        }
        
        /*
         * Initialize A/B/C by random matrices with size (MaxN+1)*(MaxN+1)
         */
        ae_matrix_set_length(&refra, maxn+1, maxn+1, _state);
        ae_matrix_set_length(&refrb, maxn+1, maxn+1, _state);
        ae_matrix_set_length(&refrc, maxn+1, maxn+1, _state);
        ae_matrix_set_length(&refca, maxn+1, maxn+1, _state);
        ae_matrix_set_length(&refcb, maxn+1, maxn+1, _state);
        ae_matrix_set_length(&refcc, maxn+1, maxn+1, _state);
        for(i=0; i<=maxn; i++)
        {
            for(j=0; j<=maxn; j++)
            {
                refra.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                refrb.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                refrc.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                refca.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                refca.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                refcb.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                refcb.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                refcc.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                refcc.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
            }
        }
        
        /*
         * test different types of operations, offsets, and so on...
         *
         * to avoid unnecessary slowdown we don't test ALL possible
         * combinations of operation types. We just generate one random
         * set of parameters and test it.
         */
        ae_matrix_set_length(&rc1, maxn+1, maxn+1, _state);
        ae_matrix_set_length(&rc2, maxn+1, maxn+1, _state);
        ae_matrix_set_length(&cc1, maxn+1, maxn+1, _state);
        ae_matrix_set_length(&cc2, maxn+1, maxn+1, _state);
        aoffsi = ae_randominteger(2, _state);
        aoffsj = ae_randominteger(2, _state);
        aoptype = ae_randominteger(3, _state);
        aoptyper = ae_randominteger(2, _state);
        boffsi = ae_randominteger(2, _state);
        boffsj = ae_randominteger(2, _state);
        boptype = ae_randominteger(3, _state);
        boptyper = ae_randominteger(2, _state);
        coffsi = ae_randominteger(2, _state);
        coffsj = ae_randominteger(2, _state);
        alphar = ae_randominteger(2, _state)*(2*ae_randomreal(_state)-1);
        betar = ae_randominteger(2, _state)*(2*ae_randomreal(_state)-1);
        if( ae_fp_greater(ae_randomreal(_state),0.5) )
        {
            alphac.x = 2*ae_randomreal(_state)-1;
            alphac.y = 2*ae_randomreal(_state)-1;
        }
        else
        {
            alphac = ae_complex_from_i(0);
        }
        if( ae_fp_greater(ae_randomreal(_state),0.5) )
        {
            betac.x = 2*ae_randomreal(_state)-1;
            betac.y = 2*ae_randomreal(_state)-1;
        }
        else
        {
            betac = ae_complex_from_i(0);
        }
        
        /*
         * copy C
         */
        for(i=0; i<=maxn; i++)
        {
            for(j=0; j<=maxn; j++)
            {
                rc1.ptr.pp_double[i][j] = refrc.ptr.pp_double[i][j];
                rc2.ptr.pp_double[i][j] = refrc.ptr.pp_double[i][j];
                cc1.ptr.pp_complex[i][j] = refcc.ptr.pp_complex[i][j];
                cc2.ptr.pp_complex[i][j] = refcc.ptr.pp_complex[i][j];
            }
        }
        
        /*
         * Test complex
         */
        cmatrixgemm(m, n, k, alphac, &refca, aoffsi, aoffsj, aoptype, &refcb, boffsi, boffsj, boptype, betac, &cc1, coffsi, coffsj, _state);
        testablasunit_refcmatrixgemm(m, n, k, alphac, &refca, aoffsi, aoffsj, aoptype, &refcb, boffsi, boffsj, boptype, betac, &cc2, coffsi, coffsj, _state);
        for(i=0; i<=maxn; i++)
        {
            for(j=0; j<=maxn; j++)
            {
                result = result||ae_fp_greater(ae_c_abs(ae_c_sub(cc1.ptr.pp_complex[i][j],cc2.ptr.pp_complex[i][j]), _state),threshold);
            }
        }
        
        /*
         * Test real
         */
        rmatrixgemm(m, n, k, alphar, &refra, aoffsi, aoffsj, aoptyper, &refrb, boffsi, boffsj, boptyper, betar, &rc1, coffsi, coffsj, _state);
        testablasunit_refrmatrixgemm(m, n, k, alphar, &refra, aoffsi, aoffsj, aoptyper, &refrb, boffsi, boffsj, boptyper, betar, &rc2, coffsi, coffsj, _state);
        for(i=0; i<=maxn; i++)
        {
            for(j=0; j<=maxn; j++)
            {
                result = result||ae_fp_greater(ae_fabs(rc1.ptr.pp_double[i][j]-rc2.ptr.pp_double[i][j], _state),threshold);
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
transpose tests

Returns False for passed test, True - for failed
*************************************************************************/
static ae_bool testablasunit_testtrans(ae_int_t minn,
     ae_int_t maxn,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t m;
    ae_int_t n;
    ae_int_t mx;
    ae_int_t i;
    ae_int_t j;
    ae_int_t aoffsi;
    ae_int_t aoffsj;
    ae_int_t boffsi;
    ae_int_t boffsj;
    double v1;
    double v2;
    double threshold;
    ae_matrix refra;
    ae_matrix refrb;
    ae_matrix refca;
    ae_matrix refcb;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&refra, 0, 0, DT_REAL, _state);
    ae_matrix_init(&refrb, 0, 0, DT_REAL, _state);
    ae_matrix_init(&refca, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&refcb, 0, 0, DT_COMPLEX, _state);

    result = ae_false;
    threshold = 1000*ae_machineepsilon;
    for(mx=minn; mx<=maxn; mx++)
    {
        
        /*
         * Select random M/N in [1,MX] such that max(M,N)=MX
         * Generate random V1 and V2 which are used to fill
         * RefRB/RefCB with control values.
         */
        m = 1+ae_randominteger(mx, _state);
        n = 1+ae_randominteger(mx, _state);
        if( ae_randominteger(2, _state)==0 )
        {
            m = mx;
        }
        else
        {
            n = mx;
        }
        v1 = ae_randomreal(_state);
        v2 = ae_randomreal(_state);
        
        /*
         * Initialize A by random matrix with size (MaxN+1)*(MaxN+1)
         * Fill B with control values
         */
        ae_matrix_set_length(&refra, maxn+1, maxn+1, _state);
        ae_matrix_set_length(&refrb, maxn+1, maxn+1, _state);
        ae_matrix_set_length(&refca, maxn+1, maxn+1, _state);
        ae_matrix_set_length(&refcb, maxn+1, maxn+1, _state);
        for(i=0; i<=maxn; i++)
        {
            for(j=0; j<=maxn; j++)
            {
                refra.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                refca.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                refca.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                refrb.ptr.pp_double[i][j] = i*v1+j*v2;
                refcb.ptr.pp_complex[i][j] = ae_complex_from_d(i*v1+j*v2);
            }
        }
        
        /*
         * test different offsets (zero or one)
         *
         * to avoid unnecessary slowdown we don't test ALL possible
         * combinations of operation types. We just generate one random
         * set of parameters and test it.
         */
        aoffsi = ae_randominteger(2, _state);
        aoffsj = ae_randominteger(2, _state);
        boffsi = ae_randominteger(2, _state);
        boffsj = ae_randominteger(2, _state);
        rmatrixtranspose(m, n, &refra, aoffsi, aoffsj, &refrb, boffsi, boffsj, _state);
        for(i=0; i<=maxn; i++)
        {
            for(j=0; j<=maxn; j++)
            {
                if( ((i<boffsi||i>=boffsi+n)||j<boffsj)||j>=boffsj+m )
                {
                    result = result||ae_fp_greater(ae_fabs(refrb.ptr.pp_double[i][j]-(v1*i+v2*j), _state),threshold);
                }
                else
                {
                    result = result||ae_fp_greater(ae_fabs(refrb.ptr.pp_double[i][j]-refra.ptr.pp_double[aoffsi+j-boffsj][aoffsj+i-boffsi], _state),threshold);
                }
            }
        }
        cmatrixtranspose(m, n, &refca, aoffsi, aoffsj, &refcb, boffsi, boffsj, _state);
        for(i=0; i<=maxn; i++)
        {
            for(j=0; j<=maxn; j++)
            {
                if( ((i<boffsi||i>=boffsi+n)||j<boffsj)||j>=boffsj+m )
                {
                    result = result||ae_fp_greater(ae_c_abs(ae_c_sub_d(refcb.ptr.pp_complex[i][j],v1*i+v2*j), _state),threshold);
                }
                else
                {
                    result = result||ae_fp_greater(ae_c_abs(ae_c_sub(refcb.ptr.pp_complex[i][j],refca.ptr.pp_complex[aoffsi+j-boffsj][aoffsj+i-boffsi]), _state),threshold);
                }
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
rank-1tests

Returns False for passed test, True - for failed
*************************************************************************/
static ae_bool testablasunit_testrank1(ae_int_t minn,
     ae_int_t maxn,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t m;
    ae_int_t n;
    ae_int_t mx;
    ae_int_t i;
    ae_int_t j;
    ae_int_t aoffsi;
    ae_int_t aoffsj;
    ae_int_t uoffs;
    ae_int_t voffs;
    double threshold;
    ae_matrix refra;
    ae_matrix refrb;
    ae_matrix refca;
    ae_matrix refcb;
    ae_vector ru;
    ae_vector rv;
    ae_vector cu;
    ae_vector cv;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&refra, 0, 0, DT_REAL, _state);
    ae_matrix_init(&refrb, 0, 0, DT_REAL, _state);
    ae_matrix_init(&refca, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&refcb, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&ru, 0, DT_REAL, _state);
    ae_vector_init(&rv, 0, DT_REAL, _state);
    ae_vector_init(&cu, 0, DT_COMPLEX, _state);
    ae_vector_init(&cv, 0, DT_COMPLEX, _state);

    result = ae_false;
    threshold = 1000*ae_machineepsilon;
    for(mx=minn; mx<=maxn; mx++)
    {
        
        /*
         * Select random M/N in [1,MX] such that max(M,N)=MX
         */
        m = 1+ae_randominteger(mx, _state);
        n = 1+ae_randominteger(mx, _state);
        if( ae_randominteger(2, _state)==0 )
        {
            m = mx;
        }
        else
        {
            n = mx;
        }
        
        /*
         * Initialize A by random matrix with size (MaxN+1)*(MaxN+1)
         * Fill B with control values
         */
        ae_matrix_set_length(&refra, maxn+maxn, maxn+maxn, _state);
        ae_matrix_set_length(&refrb, maxn+maxn, maxn+maxn, _state);
        ae_matrix_set_length(&refca, maxn+maxn, maxn+maxn, _state);
        ae_matrix_set_length(&refcb, maxn+maxn, maxn+maxn, _state);
        for(i=0; i<=2*maxn-1; i++)
        {
            for(j=0; j<=2*maxn-1; j++)
            {
                refra.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                refca.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                refca.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                refrb.ptr.pp_double[i][j] = refra.ptr.pp_double[i][j];
                refcb.ptr.pp_complex[i][j] = refca.ptr.pp_complex[i][j];
            }
        }
        ae_vector_set_length(&ru, 2*m, _state);
        ae_vector_set_length(&cu, 2*m, _state);
        for(i=0; i<=2*m-1; i++)
        {
            ru.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            cu.ptr.p_complex[i].x = 2*ae_randomreal(_state)-1;
            cu.ptr.p_complex[i].y = 2*ae_randomreal(_state)-1;
        }
        ae_vector_set_length(&rv, 2*n, _state);
        ae_vector_set_length(&cv, 2*n, _state);
        for(i=0; i<=2*n-1; i++)
        {
            rv.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            cv.ptr.p_complex[i].x = 2*ae_randomreal(_state)-1;
            cv.ptr.p_complex[i].y = 2*ae_randomreal(_state)-1;
        }
        
        /*
         * test different offsets (zero or one)
         *
         * to avoid unnecessary slowdown we don't test ALL possible
         * combinations of operation types. We just generate one random
         * set of parameters and test it.
         */
        aoffsi = ae_randominteger(maxn, _state);
        aoffsj = ae_randominteger(maxn, _state);
        uoffs = ae_randominteger(m, _state);
        voffs = ae_randominteger(n, _state);
        cmatrixrank1(m, n, &refca, aoffsi, aoffsj, &cu, uoffs, &cv, voffs, _state);
        for(i=0; i<=2*maxn-1; i++)
        {
            for(j=0; j<=2*maxn-1; j++)
            {
                if( ((i<aoffsi||i>=aoffsi+m)||j<aoffsj)||j>=aoffsj+n )
                {
                    result = result||ae_fp_greater(ae_c_abs(ae_c_sub(refca.ptr.pp_complex[i][j],refcb.ptr.pp_complex[i][j]), _state),threshold);
                }
                else
                {
                    result = result||ae_fp_greater(ae_c_abs(ae_c_sub(refca.ptr.pp_complex[i][j],ae_c_add(refcb.ptr.pp_complex[i][j],ae_c_mul(cu.ptr.p_complex[i-aoffsi+uoffs],cv.ptr.p_complex[j-aoffsj+voffs]))), _state),threshold);
                }
            }
        }
        rmatrixrank1(m, n, &refra, aoffsi, aoffsj, &ru, uoffs, &rv, voffs, _state);
        for(i=0; i<=2*maxn-1; i++)
        {
            for(j=0; j<=2*maxn-1; j++)
            {
                if( ((i<aoffsi||i>=aoffsi+m)||j<aoffsj)||j>=aoffsj+n )
                {
                    result = result||ae_fp_greater(ae_fabs(refra.ptr.pp_double[i][j]-refrb.ptr.pp_double[i][j], _state),threshold);
                }
                else
                {
                    result = result||ae_fp_greater(ae_fabs(refra.ptr.pp_double[i][j]-(refrb.ptr.pp_double[i][j]+ru.ptr.p_double[i-aoffsi+uoffs]*rv.ptr.p_double[j-aoffsj+voffs]), _state),threshold);
                }
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
MV tests

Returns False for passed test, True - for failed
*************************************************************************/
static ae_bool testablasunit_testmv(ae_int_t minn,
     ae_int_t maxn,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t m;
    ae_int_t n;
    ae_int_t mx;
    ae_int_t i;
    ae_int_t j;
    ae_int_t aoffsi;
    ae_int_t aoffsj;
    ae_int_t xoffs;
    ae_int_t yoffs;
    ae_int_t opca;
    ae_int_t opra;
    double threshold;
    double rv1;
    double rv2;
    ae_complex cv1;
    ae_complex cv2;
    ae_matrix refra;
    ae_matrix refca;
    ae_vector rx;
    ae_vector ry;
    ae_vector cx;
    ae_vector cy;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&refra, 0, 0, DT_REAL, _state);
    ae_matrix_init(&refca, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&rx, 0, DT_REAL, _state);
    ae_vector_init(&ry, 0, DT_REAL, _state);
    ae_vector_init(&cx, 0, DT_COMPLEX, _state);
    ae_vector_init(&cy, 0, DT_COMPLEX, _state);

    result = ae_false;
    threshold = 1000*ae_machineepsilon;
    for(mx=minn; mx<=maxn; mx++)
    {
        
        /*
         * Select random M/N in [1,MX] such that max(M,N)=MX
         */
        m = 1+ae_randominteger(mx, _state);
        n = 1+ae_randominteger(mx, _state);
        if( ae_randominteger(2, _state)==0 )
        {
            m = mx;
        }
        else
        {
            n = mx;
        }
        
        /*
         * Initialize A by random matrix with size (MaxN+MaxN)*(MaxN+MaxN)
         * Initialize X by random vector with size (MaxN+MaxN)
         * Fill Y by control values
         */
        ae_matrix_set_length(&refra, maxn+maxn, maxn+maxn, _state);
        ae_matrix_set_length(&refca, maxn+maxn, maxn+maxn, _state);
        for(i=0; i<=2*maxn-1; i++)
        {
            for(j=0; j<=2*maxn-1; j++)
            {
                refra.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                refca.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                refca.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
            }
        }
        ae_vector_set_length(&rx, 2*maxn, _state);
        ae_vector_set_length(&cx, 2*maxn, _state);
        ae_vector_set_length(&ry, 2*maxn, _state);
        ae_vector_set_length(&cy, 2*maxn, _state);
        for(i=0; i<=2*maxn-1; i++)
        {
            rx.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            cx.ptr.p_complex[i].x = 2*ae_randomreal(_state)-1;
            cx.ptr.p_complex[i].y = 2*ae_randomreal(_state)-1;
            ry.ptr.p_double[i] = (double)(i);
            cy.ptr.p_complex[i] = ae_complex_from_i(i);
        }
        
        /*
         * test different offsets (zero or one)
         *
         * to avoid unnecessary slowdown we don't test ALL possible
         * combinations of operation types. We just generate one random
         * set of parameters and test it.
         */
        aoffsi = ae_randominteger(maxn, _state);
        aoffsj = ae_randominteger(maxn, _state);
        xoffs = ae_randominteger(maxn, _state);
        yoffs = ae_randominteger(maxn, _state);
        opca = ae_randominteger(3, _state);
        opra = ae_randominteger(2, _state);
        cmatrixmv(m, n, &refca, aoffsi, aoffsj, opca, &cx, xoffs, &cy, yoffs, _state);
        for(i=0; i<=2*maxn-1; i++)
        {
            if( i<yoffs||i>=yoffs+m )
            {
                result = result||ae_c_neq_d(cy.ptr.p_complex[i],(double)(i));
            }
            else
            {
                cv1 = cy.ptr.p_complex[i];
                cv2 = ae_complex_from_d(0.0);
                if( opca==0 )
                {
                    cv2 = ae_v_cdotproduct(&refca.ptr.pp_complex[aoffsi+i-yoffs][aoffsj], 1, "N", &cx.ptr.p_complex[xoffs], 1, "N", ae_v_len(aoffsj,aoffsj+n-1));
                }
                if( opca==1 )
                {
                    cv2 = ae_v_cdotproduct(&refca.ptr.pp_complex[aoffsi][aoffsj+i-yoffs], refca.stride, "N", &cx.ptr.p_complex[xoffs], 1, "N", ae_v_len(aoffsi,aoffsi+n-1));
                }
                if( opca==2 )
                {
                    cv2 = ae_v_cdotproduct(&refca.ptr.pp_complex[aoffsi][aoffsj+i-yoffs], refca.stride, "Conj", &cx.ptr.p_complex[xoffs], 1, "N", ae_v_len(aoffsi,aoffsi+n-1));
                }
                result = result||ae_fp_greater(ae_c_abs(ae_c_sub(cv1,cv2), _state),threshold);
            }
        }
        rmatrixmv(m, n, &refra, aoffsi, aoffsj, opra, &rx, xoffs, &ry, yoffs, _state);
        for(i=0; i<=2*maxn-1; i++)
        {
            if( i<yoffs||i>=yoffs+m )
            {
                result = result||ae_fp_neq(ry.ptr.p_double[i],(double)(i));
            }
            else
            {
                rv1 = ry.ptr.p_double[i];
                rv2 = (double)(0);
                if( opra==0 )
                {
                    rv2 = ae_v_dotproduct(&refra.ptr.pp_double[aoffsi+i-yoffs][aoffsj], 1, &rx.ptr.p_double[xoffs], 1, ae_v_len(aoffsj,aoffsj+n-1));
                }
                if( opra==1 )
                {
                    rv2 = ae_v_dotproduct(&refra.ptr.pp_double[aoffsi][aoffsj+i-yoffs], refra.stride, &rx.ptr.p_double[xoffs], 1, ae_v_len(aoffsi,aoffsi+n-1));
                }
                result = result||ae_fp_greater(ae_fabs(rv1-rv2, _state),threshold);
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
COPY tests

Returns False for passed test, True - for failed
*************************************************************************/
static ae_bool testablasunit_testcopy(ae_int_t minn,
     ae_int_t maxn,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t m;
    ae_int_t n;
    ae_int_t mx;
    ae_int_t i;
    ae_int_t j;
    ae_int_t aoffsi;
    ae_int_t aoffsj;
    ae_int_t boffsi;
    ae_int_t boffsj;
    double threshold;
    ae_matrix ra;
    ae_matrix rb;
    ae_matrix ca;
    ae_matrix cb;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&ra, 0, 0, DT_REAL, _state);
    ae_matrix_init(&rb, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ca, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&cb, 0, 0, DT_COMPLEX, _state);

    result = ae_false;
    threshold = 1000*ae_machineepsilon;
    for(mx=minn; mx<=maxn; mx++)
    {
        
        /*
         * Select random M/N in [1,MX] such that max(M,N)=MX
         */
        m = 1+ae_randominteger(mx, _state);
        n = 1+ae_randominteger(mx, _state);
        if( ae_randominteger(2, _state)==0 )
        {
            m = mx;
        }
        else
        {
            n = mx;
        }
        
        /*
         * Initialize A by random matrix with size (MaxN+MaxN)*(MaxN+MaxN)
         * Initialize X by random vector with size (MaxN+MaxN)
         * Fill Y by control values
         */
        ae_matrix_set_length(&ra, maxn+maxn, maxn+maxn, _state);
        ae_matrix_set_length(&ca, maxn+maxn, maxn+maxn, _state);
        ae_matrix_set_length(&rb, maxn+maxn, maxn+maxn, _state);
        ae_matrix_set_length(&cb, maxn+maxn, maxn+maxn, _state);
        for(i=0; i<=2*maxn-1; i++)
        {
            for(j=0; j<=2*maxn-1; j++)
            {
                ra.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                ca.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                ca.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                rb.ptr.pp_double[i][j] = (double)(1+2*i+3*j);
                cb.ptr.pp_complex[i][j] = ae_complex_from_i(1+2*i+3*j);
            }
        }
        
        /*
         * test different offsets (zero or one)
         *
         * to avoid unnecessary slowdown we don't test ALL possible
         * combinations of operation types. We just generate one random
         * set of parameters and test it.
         */
        aoffsi = ae_randominteger(maxn, _state);
        aoffsj = ae_randominteger(maxn, _state);
        boffsi = ae_randominteger(maxn, _state);
        boffsj = ae_randominteger(maxn, _state);
        cmatrixcopy(m, n, &ca, aoffsi, aoffsj, &cb, boffsi, boffsj, _state);
        for(i=0; i<=2*maxn-1; i++)
        {
            for(j=0; j<=2*maxn-1; j++)
            {
                if( ((i<boffsi||i>=boffsi+m)||j<boffsj)||j>=boffsj+n )
                {
                    result = result||ae_c_neq_d(cb.ptr.pp_complex[i][j],(double)(1+2*i+3*j));
                }
                else
                {
                    result = result||ae_fp_greater(ae_c_abs(ae_c_sub(ca.ptr.pp_complex[aoffsi+i-boffsi][aoffsj+j-boffsj],cb.ptr.pp_complex[i][j]), _state),threshold);
                }
            }
        }
        rmatrixcopy(m, n, &ra, aoffsi, aoffsj, &rb, boffsi, boffsj, _state);
        for(i=0; i<=2*maxn-1; i++)
        {
            for(j=0; j<=2*maxn-1; j++)
            {
                if( ((i<boffsi||i>=boffsi+m)||j<boffsj)||j>=boffsj+n )
                {
                    result = result||ae_fp_neq(rb.ptr.pp_double[i][j],(double)(1+2*i+3*j));
                }
                else
                {
                    result = result||ae_fp_greater(ae_fabs(ra.ptr.pp_double[aoffsi+i-boffsi][aoffsj+j-boffsj]-rb.ptr.pp_double[i][j], _state),threshold);
                }
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Reference implementation

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
static void testablasunit_refcmatrixrighttrsm(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Complex */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a1;
    ae_matrix a2;
    ae_vector tx;
    ae_int_t i;
    ae_int_t j;
    ae_complex vc;
    ae_bool rupper;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a1, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&a2, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&tx, 0, DT_COMPLEX, _state);

    if( n*m==0 )
    {
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&a1, n, n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a1.ptr.pp_complex[i][j] = ae_complex_from_i(0);
        }
    }
    if( isupper )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=i; j<=n-1; j++)
            {
                a1.ptr.pp_complex[i][j] = a->ptr.pp_complex[i1+i][j1+j];
            }
        }
    }
    else
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=i; j++)
            {
                a1.ptr.pp_complex[i][j] = a->ptr.pp_complex[i1+i][j1+j];
            }
        }
    }
    rupper = isupper;
    if( isunit )
    {
        for(i=0; i<=n-1; i++)
        {
            a1.ptr.pp_complex[i][i] = ae_complex_from_i(1);
        }
    }
    ae_matrix_set_length(&a2, n, n, _state);
    if( optype==0 )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a2.ptr.pp_complex[i][j] = a1.ptr.pp_complex[i][j];
            }
        }
    }
    if( optype==1 )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a2.ptr.pp_complex[i][j] = a1.ptr.pp_complex[j][i];
            }
        }
        rupper = !rupper;
    }
    if( optype==2 )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a2.ptr.pp_complex[i][j] = ae_c_conj(a1.ptr.pp_complex[j][i], _state);
            }
        }
        rupper = !rupper;
    }
    testablasunit_internalcmatrixtrinverse(&a2, n, rupper, ae_false, _state);
    ae_vector_set_length(&tx, n, _state);
    for(i=0; i<=m-1; i++)
    {
        ae_v_cmove(&tx.ptr.p_complex[0], 1, &x->ptr.pp_complex[i2+i][j2], 1, "N", ae_v_len(0,n-1));
        for(j=0; j<=n-1; j++)
        {
            vc = ae_v_cdotproduct(&tx.ptr.p_complex[0], 1, "N", &a2.ptr.pp_complex[0][j], a2.stride, "N", ae_v_len(0,n-1));
            x->ptr.pp_complex[i2+i][j2+j] = vc;
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Reference implementation

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
static void testablasunit_refcmatrixlefttrsm(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Complex */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a1;
    ae_matrix a2;
    ae_vector tx;
    ae_int_t i;
    ae_int_t j;
    ae_complex vc;
    ae_bool rupper;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a1, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&a2, 0, 0, DT_COMPLEX, _state);
    ae_vector_init(&tx, 0, DT_COMPLEX, _state);

    if( n*m==0 )
    {
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&a1, m, m, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=m-1; j++)
        {
            a1.ptr.pp_complex[i][j] = ae_complex_from_i(0);
        }
    }
    if( isupper )
    {
        for(i=0; i<=m-1; i++)
        {
            for(j=i; j<=m-1; j++)
            {
                a1.ptr.pp_complex[i][j] = a->ptr.pp_complex[i1+i][j1+j];
            }
        }
    }
    else
    {
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=i; j++)
            {
                a1.ptr.pp_complex[i][j] = a->ptr.pp_complex[i1+i][j1+j];
            }
        }
    }
    rupper = isupper;
    if( isunit )
    {
        for(i=0; i<=m-1; i++)
        {
            a1.ptr.pp_complex[i][i] = ae_complex_from_i(1);
        }
    }
    ae_matrix_set_length(&a2, m, m, _state);
    if( optype==0 )
    {
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                a2.ptr.pp_complex[i][j] = a1.ptr.pp_complex[i][j];
            }
        }
    }
    if( optype==1 )
    {
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                a2.ptr.pp_complex[i][j] = a1.ptr.pp_complex[j][i];
            }
        }
        rupper = !rupper;
    }
    if( optype==2 )
    {
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                a2.ptr.pp_complex[i][j] = ae_c_conj(a1.ptr.pp_complex[j][i], _state);
            }
        }
        rupper = !rupper;
    }
    testablasunit_internalcmatrixtrinverse(&a2, m, rupper, ae_false, _state);
    ae_vector_set_length(&tx, m, _state);
    for(j=0; j<=n-1; j++)
    {
        ae_v_cmove(&tx.ptr.p_complex[0], 1, &x->ptr.pp_complex[i2][j2+j], x->stride, "N", ae_v_len(0,m-1));
        for(i=0; i<=m-1; i++)
        {
            vc = ae_v_cdotproduct(&a2.ptr.pp_complex[i][0], 1, "N", &tx.ptr.p_complex[0], 1, "N", ae_v_len(0,m-1));
            x->ptr.pp_complex[i2+i][j2+j] = vc;
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Reference implementation

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
static void testablasunit_refrmatrixrighttrsm(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Real    */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a1;
    ae_matrix a2;
    ae_vector tx;
    ae_int_t i;
    ae_int_t j;
    double vr;
    ae_bool rupper;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a2, 0, 0, DT_REAL, _state);
    ae_vector_init(&tx, 0, DT_REAL, _state);

    if( n*m==0 )
    {
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&a1, n, n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a1.ptr.pp_double[i][j] = (double)(0);
        }
    }
    if( isupper )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=i; j<=n-1; j++)
            {
                a1.ptr.pp_double[i][j] = a->ptr.pp_double[i1+i][j1+j];
            }
        }
    }
    else
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=i; j++)
            {
                a1.ptr.pp_double[i][j] = a->ptr.pp_double[i1+i][j1+j];
            }
        }
    }
    rupper = isupper;
    if( isunit )
    {
        for(i=0; i<=n-1; i++)
        {
            a1.ptr.pp_double[i][i] = (double)(1);
        }
    }
    ae_matrix_set_length(&a2, n, n, _state);
    if( optype==0 )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a2.ptr.pp_double[i][j] = a1.ptr.pp_double[i][j];
            }
        }
    }
    if( optype==1 )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a2.ptr.pp_double[i][j] = a1.ptr.pp_double[j][i];
            }
        }
        rupper = !rupper;
    }
    testablasunit_internalrmatrixtrinverse(&a2, n, rupper, ae_false, _state);
    ae_vector_set_length(&tx, n, _state);
    for(i=0; i<=m-1; i++)
    {
        ae_v_move(&tx.ptr.p_double[0], 1, &x->ptr.pp_double[i2+i][j2], 1, ae_v_len(0,n-1));
        for(j=0; j<=n-1; j++)
        {
            vr = ae_v_dotproduct(&tx.ptr.p_double[0], 1, &a2.ptr.pp_double[0][j], a2.stride, ae_v_len(0,n-1));
            x->ptr.pp_double[i2+i][j2+j] = vr;
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Reference implementation

  -- ALGLIB routine --
     15.12.2009
     Bochkanov Sergey
*************************************************************************/
static void testablasunit_refrmatrixlefttrsm(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_int_t i1,
     ae_int_t j1,
     ae_bool isupper,
     ae_bool isunit,
     ae_int_t optype,
     /* Real    */ ae_matrix* x,
     ae_int_t i2,
     ae_int_t j2,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a1;
    ae_matrix a2;
    ae_vector tx;
    ae_int_t i;
    ae_int_t j;
    double vr;
    ae_bool rupper;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&a1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a2, 0, 0, DT_REAL, _state);
    ae_vector_init(&tx, 0, DT_REAL, _state);

    if( n*m==0 )
    {
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&a1, m, m, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=m-1; j++)
        {
            a1.ptr.pp_double[i][j] = (double)(0);
        }
    }
    if( isupper )
    {
        for(i=0; i<=m-1; i++)
        {
            for(j=i; j<=m-1; j++)
            {
                a1.ptr.pp_double[i][j] = a->ptr.pp_double[i1+i][j1+j];
            }
        }
    }
    else
    {
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=i; j++)
            {
                a1.ptr.pp_double[i][j] = a->ptr.pp_double[i1+i][j1+j];
            }
        }
    }
    rupper = isupper;
    if( isunit )
    {
        for(i=0; i<=m-1; i++)
        {
            a1.ptr.pp_double[i][i] = (double)(1);
        }
    }
    ae_matrix_set_length(&a2, m, m, _state);
    if( optype==0 )
    {
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                a2.ptr.pp_double[i][j] = a1.ptr.pp_double[i][j];
            }
        }
    }
    if( optype==1 )
    {
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=m-1; j++)
            {
                a2.ptr.pp_double[i][j] = a1.ptr.pp_double[j][i];
            }
        }
        rupper = !rupper;
    }
    testablasunit_internalrmatrixtrinverse(&a2, m, rupper, ae_false, _state);
    ae_vector_set_length(&tx, m, _state);
    for(j=0; j<=n-1; j++)
    {
        ae_v_move(&tx.ptr.p_double[0], 1, &x->ptr.pp_double[i2][j2+j], x->stride, ae_v_len(0,m-1));
        for(i=0; i<=m-1; i++)
        {
            vr = ae_v_dotproduct(&a2.ptr.pp_double[i][0], 1, &tx.ptr.p_double[0], 1, ae_v_len(0,m-1));
            x->ptr.pp_double[i2+i][j2+j] = vr;
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine.
Triangular matrix inversion

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     February 29, 1992
*************************************************************************/
static ae_bool testablasunit_internalcmatrixtrinverse(/* Complex */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunittriangular,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool nounit;
    ae_int_t i;
    ae_int_t j;
    ae_complex v;
    ae_complex ajj;
    ae_vector t;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&t, 0, DT_COMPLEX, _state);

    result = ae_true;
    ae_vector_set_length(&t, n-1+1, _state);
    
    /*
     * Test the input parameters.
     */
    nounit = !isunittriangular;
    if( isupper )
    {
        
        /*
         * Compute inverse of upper triangular matrix.
         */
        for(j=0; j<=n-1; j++)
        {
            if( nounit )
            {
                if( ae_c_eq_d(a->ptr.pp_complex[j][j],(double)(0)) )
                {
                    result = ae_false;
                    ae_frame_leave(_state);
                    return result;
                }
                a->ptr.pp_complex[j][j] = ae_c_d_div(1,a->ptr.pp_complex[j][j]);
                ajj = ae_c_neg(a->ptr.pp_complex[j][j]);
            }
            else
            {
                ajj = ae_complex_from_i(-1);
            }
            
            /*
             * Compute elements 1:j-1 of j-th column.
             */
            if( j>0 )
            {
                ae_v_cmove(&t.ptr.p_complex[0], 1, &a->ptr.pp_complex[0][j], a->stride, "N", ae_v_len(0,j-1));
                for(i=0; i<=j-1; i++)
                {
                    if( i+1<j )
                    {
                        v = ae_v_cdotproduct(&a->ptr.pp_complex[i][i+1], 1, "N", &t.ptr.p_complex[i+1], 1, "N", ae_v_len(i+1,j-1));
                    }
                    else
                    {
                        v = ae_complex_from_i(0);
                    }
                    if( nounit )
                    {
                        a->ptr.pp_complex[i][j] = ae_c_add(v,ae_c_mul(a->ptr.pp_complex[i][i],t.ptr.p_complex[i]));
                    }
                    else
                    {
                        a->ptr.pp_complex[i][j] = ae_c_add(v,t.ptr.p_complex[i]);
                    }
                }
                ae_v_cmulc(&a->ptr.pp_complex[0][j], a->stride, ae_v_len(0,j-1), ajj);
            }
        }
    }
    else
    {
        
        /*
         * Compute inverse of lower triangular matrix.
         */
        for(j=n-1; j>=0; j--)
        {
            if( nounit )
            {
                if( ae_c_eq_d(a->ptr.pp_complex[j][j],(double)(0)) )
                {
                    result = ae_false;
                    ae_frame_leave(_state);
                    return result;
                }
                a->ptr.pp_complex[j][j] = ae_c_d_div(1,a->ptr.pp_complex[j][j]);
                ajj = ae_c_neg(a->ptr.pp_complex[j][j]);
            }
            else
            {
                ajj = ae_complex_from_i(-1);
            }
            if( j+1<n )
            {
                
                /*
                 * Compute elements j+1:n of j-th column.
                 */
                ae_v_cmove(&t.ptr.p_complex[j+1], 1, &a->ptr.pp_complex[j+1][j], a->stride, "N", ae_v_len(j+1,n-1));
                for(i=j+1; i<=n-1; i++)
                {
                    if( i>j+1 )
                    {
                        v = ae_v_cdotproduct(&a->ptr.pp_complex[i][j+1], 1, "N", &t.ptr.p_complex[j+1], 1, "N", ae_v_len(j+1,i-1));
                    }
                    else
                    {
                        v = ae_complex_from_i(0);
                    }
                    if( nounit )
                    {
                        a->ptr.pp_complex[i][j] = ae_c_add(v,ae_c_mul(a->ptr.pp_complex[i][i],t.ptr.p_complex[i]));
                    }
                    else
                    {
                        a->ptr.pp_complex[i][j] = ae_c_add(v,t.ptr.p_complex[i]);
                    }
                }
                ae_v_cmulc(&a->ptr.pp_complex[j+1][j], a->stride, ae_v_len(j+1,n-1), ajj);
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Internal subroutine.
Triangular matrix inversion

  -- LAPACK routine (version 3.0) --
     Univ. of Tennessee, Univ. of California Berkeley, NAG Ltd.,
     Courant Institute, Argonne National Lab, and Rice University
     February 29, 1992
*************************************************************************/
static ae_bool testablasunit_internalrmatrixtrinverse(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_bool isupper,
     ae_bool isunittriangular,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool nounit;
    ae_int_t i;
    ae_int_t j;
    double v;
    double ajj;
    ae_vector t;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&t, 0, DT_REAL, _state);

    result = ae_true;
    ae_vector_set_length(&t, n-1+1, _state);
    
    /*
     * Test the input parameters.
     */
    nounit = !isunittriangular;
    if( isupper )
    {
        
        /*
         * Compute inverse of upper triangular matrix.
         */
        for(j=0; j<=n-1; j++)
        {
            if( nounit )
            {
                if( ae_fp_eq(a->ptr.pp_double[j][j],(double)(0)) )
                {
                    result = ae_false;
                    ae_frame_leave(_state);
                    return result;
                }
                a->ptr.pp_double[j][j] = 1/a->ptr.pp_double[j][j];
                ajj = -a->ptr.pp_double[j][j];
            }
            else
            {
                ajj = (double)(-1);
            }
            
            /*
             * Compute elements 1:j-1 of j-th column.
             */
            if( j>0 )
            {
                ae_v_move(&t.ptr.p_double[0], 1, &a->ptr.pp_double[0][j], a->stride, ae_v_len(0,j-1));
                for(i=0; i<=j-1; i++)
                {
                    if( i<j-1 )
                    {
                        v = ae_v_dotproduct(&a->ptr.pp_double[i][i+1], 1, &t.ptr.p_double[i+1], 1, ae_v_len(i+1,j-1));
                    }
                    else
                    {
                        v = (double)(0);
                    }
                    if( nounit )
                    {
                        a->ptr.pp_double[i][j] = v+a->ptr.pp_double[i][i]*t.ptr.p_double[i];
                    }
                    else
                    {
                        a->ptr.pp_double[i][j] = v+t.ptr.p_double[i];
                    }
                }
                ae_v_muld(&a->ptr.pp_double[0][j], a->stride, ae_v_len(0,j-1), ajj);
            }
        }
    }
    else
    {
        
        /*
         * Compute inverse of lower triangular matrix.
         */
        for(j=n-1; j>=0; j--)
        {
            if( nounit )
            {
                if( ae_fp_eq(a->ptr.pp_double[j][j],(double)(0)) )
                {
                    result = ae_false;
                    ae_frame_leave(_state);
                    return result;
                }
                a->ptr.pp_double[j][j] = 1/a->ptr.pp_double[j][j];
                ajj = -a->ptr.pp_double[j][j];
            }
            else
            {
                ajj = (double)(-1);
            }
            if( j<n-1 )
            {
                
                /*
                 * Compute elements j+1:n of j-th column.
                 */
                ae_v_move(&t.ptr.p_double[j+1], 1, &a->ptr.pp_double[j+1][j], a->stride, ae_v_len(j+1,n-1));
                for(i=j+1; i<=n-1; i++)
                {
                    if( i>j+1 )
                    {
                        v = ae_v_dotproduct(&a->ptr.pp_double[i][j+1], 1, &t.ptr.p_double[j+1], 1, ae_v_len(j+1,i-1));
                    }
                    else
                    {
                        v = (double)(0);
                    }
                    if( nounit )
                    {
                        a->ptr.pp_double[i][j] = v+a->ptr.pp_double[i][i]*t.ptr.p_double[i];
                    }
                    else
                    {
                        a->ptr.pp_double[i][j] = v+t.ptr.p_double[i];
                    }
                }
                ae_v_muld(&a->ptr.pp_double[j+1][j], a->stride, ae_v_len(j+1,n-1), ajj);
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Reference SYRK subroutine.

  -- ALGLIB routine --
     16.12.2009
     Bochkanov Sergey
*************************************************************************/
static void testablasunit_refcmatrixherk(ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     double beta,
     /* Complex */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_bool isupper,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix ae;
    ae_int_t i;
    ae_int_t j;
    ae_complex vc;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&ae, 0, 0, DT_COMPLEX, _state);

    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( (isupper&&j>=i)||(!isupper&&j<=i) )
            {
                if( ae_fp_eq(beta,(double)(0)) )
                {
                    c->ptr.pp_complex[i+ic][j+jc] = ae_complex_from_i(0);
                }
                else
                {
                    c->ptr.pp_complex[i+ic][j+jc] = ae_c_mul_d(c->ptr.pp_complex[i+ic][j+jc],beta);
                }
            }
        }
    }
    if( ae_fp_eq(alpha,(double)(0)) )
    {
        ae_frame_leave(_state);
        return;
    }
    if( n*k>0 )
    {
        ae_matrix_set_length(&ae, n, k, _state);
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=k-1; j++)
        {
            if( optypea==0 )
            {
                ae.ptr.pp_complex[i][j] = a->ptr.pp_complex[ia+i][ja+j];
            }
            if( optypea==2 )
            {
                ae.ptr.pp_complex[i][j] = ae_c_conj(a->ptr.pp_complex[ia+j][ja+i], _state);
            }
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            vc = ae_complex_from_i(0);
            if( k>0 )
            {
                vc = ae_v_cdotproduct(&ae.ptr.pp_complex[i][0], 1, "N", &ae.ptr.pp_complex[j][0], 1, "Conj", ae_v_len(0,k-1));
            }
            vc = ae_c_mul_d(vc,alpha);
            if( isupper&&j>=i )
            {
                c->ptr.pp_complex[ic+i][jc+j] = ae_c_add(vc,c->ptr.pp_complex[ic+i][jc+j]);
            }
            if( !isupper&&j<=i )
            {
                c->ptr.pp_complex[ic+i][jc+j] = ae_c_add(vc,c->ptr.pp_complex[ic+i][jc+j]);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Reference SYRK subroutine.

  -- ALGLIB routine --
     16.12.2009
     Bochkanov Sergey
*************************************************************************/
static void testablasunit_refrmatrixsyrk(ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     double beta,
     /* Real    */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_bool isupper,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix ae;
    ae_int_t i;
    ae_int_t j;
    double vr;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&ae, 0, 0, DT_REAL, _state);

    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( (isupper&&j>=i)||(!isupper&&j<=i) )
            {
                if( ae_fp_eq(beta,(double)(0)) )
                {
                    c->ptr.pp_double[i+ic][j+jc] = (double)(0);
                }
                else
                {
                    c->ptr.pp_double[i+ic][j+jc] = c->ptr.pp_double[i+ic][j+jc]*beta;
                }
            }
        }
    }
    if( ae_fp_eq(alpha,(double)(0)) )
    {
        ae_frame_leave(_state);
        return;
    }
    if( n*k>0 )
    {
        ae_matrix_set_length(&ae, n, k, _state);
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=k-1; j++)
        {
            if( optypea==0 )
            {
                ae.ptr.pp_double[i][j] = a->ptr.pp_double[ia+i][ja+j];
            }
            if( optypea==1 )
            {
                ae.ptr.pp_double[i][j] = a->ptr.pp_double[ia+j][ja+i];
            }
        }
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            vr = (double)(0);
            if( k>0 )
            {
                vr = ae_v_dotproduct(&ae.ptr.pp_double[i][0], 1, &ae.ptr.pp_double[j][0], 1, ae_v_len(0,k-1));
            }
            vr = alpha*vr;
            if( isupper&&j>=i )
            {
                c->ptr.pp_double[ic+i][jc+j] = vr+c->ptr.pp_double[ic+i][jc+j];
            }
            if( !isupper&&j<=i )
            {
                c->ptr.pp_double[ic+i][jc+j] = vr+c->ptr.pp_double[ic+i][jc+j];
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Reference GEMM,
ALGLIB subroutine
*************************************************************************/
static void testablasunit_refcmatrixgemm(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     ae_complex alpha,
     /* Complex */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     /* Complex */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     ae_int_t optypeb,
     ae_complex beta,
     /* Complex */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix ae;
    ae_matrix be;
    ae_int_t i;
    ae_int_t j;
    ae_complex vc;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&ae, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&be, 0, 0, DT_COMPLEX, _state);

    ae_matrix_set_length(&ae, m, k, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=k-1; j++)
        {
            if( optypea==0 )
            {
                ae.ptr.pp_complex[i][j] = a->ptr.pp_complex[ia+i][ja+j];
            }
            if( optypea==1 )
            {
                ae.ptr.pp_complex[i][j] = a->ptr.pp_complex[ia+j][ja+i];
            }
            if( optypea==2 )
            {
                ae.ptr.pp_complex[i][j] = ae_c_conj(a->ptr.pp_complex[ia+j][ja+i], _state);
            }
        }
    }
    ae_matrix_set_length(&be, k, n, _state);
    for(i=0; i<=k-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( optypeb==0 )
            {
                be.ptr.pp_complex[i][j] = b->ptr.pp_complex[ib+i][jb+j];
            }
            if( optypeb==1 )
            {
                be.ptr.pp_complex[i][j] = b->ptr.pp_complex[ib+j][jb+i];
            }
            if( optypeb==2 )
            {
                be.ptr.pp_complex[i][j] = ae_c_conj(b->ptr.pp_complex[ib+j][jb+i], _state);
            }
        }
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            vc = ae_v_cdotproduct(&ae.ptr.pp_complex[i][0], 1, "N", &be.ptr.pp_complex[0][j], be.stride, "N", ae_v_len(0,k-1));
            vc = ae_c_mul(alpha,vc);
            if( ae_c_neq_d(beta,(double)(0)) )
            {
                vc = ae_c_add(vc,ae_c_mul(beta,c->ptr.pp_complex[ic+i][jc+j]));
            }
            c->ptr.pp_complex[ic+i][jc+j] = vc;
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Reference GEMM,
ALGLIB subroutine
*************************************************************************/
static void testablasunit_refrmatrixgemm(ae_int_t m,
     ae_int_t n,
     ae_int_t k,
     double alpha,
     /* Real    */ ae_matrix* a,
     ae_int_t ia,
     ae_int_t ja,
     ae_int_t optypea,
     /* Real    */ ae_matrix* b,
     ae_int_t ib,
     ae_int_t jb,
     ae_int_t optypeb,
     double beta,
     /* Real    */ ae_matrix* c,
     ae_int_t ic,
     ae_int_t jc,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix ae;
    ae_matrix be;
    ae_int_t i;
    ae_int_t j;
    double vc;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&ae, 0, 0, DT_REAL, _state);
    ae_matrix_init(&be, 0, 0, DT_REAL, _state);

    ae_matrix_set_length(&ae, m, k, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=k-1; j++)
        {
            if( optypea==0 )
            {
                ae.ptr.pp_double[i][j] = a->ptr.pp_double[ia+i][ja+j];
            }
            if( optypea==1 )
            {
                ae.ptr.pp_double[i][j] = a->ptr.pp_double[ia+j][ja+i];
            }
        }
    }
    ae_matrix_set_length(&be, k, n, _state);
    for(i=0; i<=k-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( optypeb==0 )
            {
                be.ptr.pp_double[i][j] = b->ptr.pp_double[ib+i][jb+j];
            }
            if( optypeb==1 )
            {
                be.ptr.pp_double[i][j] = b->ptr.pp_double[ib+j][jb+i];
            }
        }
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            vc = ae_v_dotproduct(&ae.ptr.pp_double[i][0], 1, &be.ptr.pp_double[0][j], be.stride, ae_v_len(0,k-1));
            vc = alpha*vc;
            if( ae_fp_neq(beta,(double)(0)) )
            {
                vc = vc+beta*c->ptr.pp_double[ic+i][jc+j];
            }
            c->ptr.pp_double[ic+i][jc+j] = vc;
        }
    }
    ae_frame_leave(_state);
}


/*$ End $*/
