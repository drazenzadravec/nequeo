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
#include "testsparseunit.h"


/*$ Declarations $*/
static void testsparseunit_initgenerator(ae_int_t m,
     ae_int_t n,
     ae_int_t matkind,
     ae_int_t triangle,
     sparsegenerator* g,
     ae_state *_state);
static ae_bool testsparseunit_generatenext(sparsegenerator* g,
     /* Real    */ ae_matrix* da,
     sparsematrix* sa,
     ae_state *_state);
static void testsparseunit_createrandom(ae_int_t m,
     ae_int_t n,
     ae_int_t pkind,
     ae_int_t ckind,
     ae_int_t p0,
     ae_int_t p1,
     /* Real    */ ae_matrix* da,
     sparsematrix* sa,
     ae_state *_state);
static ae_bool testsparseunit_enumeratetest(ae_state *_state);
static ae_bool testsparseunit_rewriteexistingtest(ae_state *_state);
static void testsparseunit_testgetrow(ae_bool* err, ae_state *_state);
static ae_bool testsparseunit_testconvertsm(ae_state *_state);
static ae_bool testsparseunit_testgcmatrixtype(ae_state *_state);


/*$ Body $*/


ae_bool testsparse(ae_bool silent, ae_state *_state)
{
    ae_bool waserrors;
    ae_bool basicerrors;
    ae_bool linearerrors;
    ae_bool basicrnderrors;
    ae_bool level2unsymmetricerrors;
    ae_bool level2symmetricerrors;
    ae_bool level2triangularerrors;
    ae_bool level3unsymmetricerrors;
    ae_bool level3symmetricerrors;
    ae_bool linearserrors;
    ae_bool linearmmerrors;
    ae_bool linearsmmerrors;
    ae_bool getrowerrors;
    ae_bool copyerrors;
    ae_bool basiccopyerrors;
    ae_bool enumerateerrors;
    ae_bool rewriteexistingerr;
    ae_bool skserrors;
    ae_bool result;


    getrowerrors = ae_false;
    skserrors = skstest(_state);
    basicerrors = basicfunctest(_state)||testsparseunit_testgcmatrixtype(_state);
    basicrnderrors = basicfuncrandomtest(_state);
    linearerrors = linearfunctionstest(_state);
    level2unsymmetricerrors = testlevel2unsymmetric(_state);
    level2symmetricerrors = testlevel2symmetric(_state);
    level2triangularerrors = testlevel2triangular(_state);
    level3unsymmetricerrors = testlevel3unsymmetric(_state);
    level3symmetricerrors = testlevel3symmetric(_state);
    linearserrors = linearfunctionsstest(_state);
    linearmmerrors = linearfunctionsmmtest(_state);
    linearsmmerrors = linearfunctionssmmtest(_state);
    copyerrors = copyfunctest(ae_true, _state)||testsparseunit_testconvertsm(_state);
    basiccopyerrors = basiccopyfunctest(ae_true, _state);
    enumerateerrors = testsparseunit_enumeratetest(_state);
    rewriteexistingerr = testsparseunit_rewriteexistingtest(_state);
    testsparseunit_testgetrow(&getrowerrors, _state);
    
    /*
     * report
     */
    waserrors = (((((((((((((((skserrors||getrowerrors)||basicerrors)||linearerrors)||basicrnderrors)||level2unsymmetricerrors)||level2symmetricerrors)||level2triangularerrors)||level3unsymmetricerrors)||level3symmetricerrors)||linearserrors)||linearmmerrors)||linearsmmerrors)||copyerrors)||basiccopyerrors)||enumerateerrors)||rewriteexistingerr;
    if( !silent )
    {
        printf("TESTING SPARSE\n");
        printf("STORAGE FORMATS:\n");
        printf("* SKS:                                   ");
        if( !skserrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("OPERATIONS:\n");
        printf("* GETROW:                                ");
        if( !getrowerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("BLAS:\n");
        printf("* LEVEL 2 GENERAL:                       ");
        if( !level2unsymmetricerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* LEVEL 2 SYMMETRIC:                     ");
        if( !level2symmetricerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* LEVEL 2 TRIANGULAR:                    ");
        if( !level2triangularerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* LEVEL 3 GENERAL:                       ");
        if( !level3unsymmetricerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* LEVEL 3 SYMMETRIC:                     ");
        if( !level3symmetricerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("BASIC TEST:                              ");
        if( !basicerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("COPY TEST:                               ");
        if( !copyerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("BASIC_COPY TEST:                         ");
        if( !basiccopyerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("BASIC_RND TEST:                          ");
        if( !basicrnderrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("LINEAR TEST:                             ");
        if( !linearerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("LINEAR TEST FOR SYMMETRIC MATRICES:      ");
        if( !linearserrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("LINEAR MxM TEST:                         ");
        if( !linearmmerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("LINEAR MxM TEST FOR SYMMETRIC MATRICES:  ");
        if( !linearsmmerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("ENUMERATE TEST:                          ");
        if( !enumerateerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("REWRITE EXISTING TEST:                   ");
        if( !rewriteexistingerr )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
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
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testsparse(ae_bool silent, ae_state *_state)
{
    return testsparse(silent, _state);
}


/*************************************************************************
Function for testing basic SKS functional.
Returns True on errors, False on success.

  -- ALGLIB PROJECT --
     Copyright 16.01.1014 by Bochkanov Sergey
*************************************************************************/
ae_bool skstest(ae_state *_state)
{
    ae_frame _frame_block;
    sparsematrix s0;
    sparsematrix s1;
    sparsematrix s2;
    sparsematrix s3;
    sparsematrix s4;
    sparsematrix s5;
    sparsematrix s6;
    ae_int_t n;
    ae_int_t nz;
    double pnz;
    ae_int_t i;
    ae_int_t j;
    ae_int_t t0;
    ae_int_t t1;
    ae_matrix a;
    ae_matrix wasenumerated;
    ae_vector d;
    ae_vector u;
    hqrndstate rs;
    double v0;
    double v1;
    ae_int_t uppercnt;
    ae_int_t lowercnt;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _sparsematrix_init(&s0, _state);
    _sparsematrix_init(&s1, _state);
    _sparsematrix_init(&s2, _state);
    _sparsematrix_init(&s3, _state);
    _sparsematrix_init(&s4, _state);
    _sparsematrix_init(&s5, _state);
    _sparsematrix_init(&s6, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&wasenumerated, 0, 0, DT_BOOL, _state);
    ae_vector_init(&d, 0, DT_INT, _state);
    ae_vector_init(&u, 0, DT_INT, _state);
    _hqrndstate_init(&rs, _state);

    result = ae_false;
    hqrndrandomize(&rs, _state);
    for(n=1; n<=20; n++)
    {
        nz = n*n-n;
        for(;;)
        {
            
            /*
             * Generate N*N matrix where probability of non-diagonal element
             * being non-zero is PNZ. We also generate D and U - subdiagonal
             * and superdiagonal profile sizes.
             */
            if( n>1 )
            {
                pnz = (double)nz/(double)(n*n-n);
            }
            else
            {
                pnz = 1.0;
            }
            ae_vector_set_length(&d, n, _state);
            ae_vector_set_length(&u, n, _state);
            for(i=0; i<=n-1; i++)
            {
                d.ptr.p_int[i] = 0;
                u.ptr.p_int[i] = 0;
            }
            ae_matrix_set_length(&a, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( i==j||ae_fp_less_eq(hqrnduniformr(&rs, _state),pnz) )
                    {
                        a.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                        if( j<i )
                        {
                            d.ptr.p_int[i] = ae_maxint(d.ptr.p_int[i], i-j, _state);
                        }
                        else
                        {
                            u.ptr.p_int[j] = ae_maxint(u.ptr.p_int[j], j-i, _state);
                        }
                    }
                    else
                    {
                        a.ptr.pp_double[i][j] = 0.0;
                    }
                }
            }
            uppercnt = 0;
            lowercnt = 0;
            for(i=0; i<=n-1; i++)
            {
                uppercnt = uppercnt+u.ptr.p_int[i];
                lowercnt = lowercnt+d.ptr.p_int[i];
            }
            
            /*
             * Create matrix in SKS storage format, fill with RewriteExisting() calls.
             * Convert to several different formats, check their contents with SparseGet().
             */
            sparsecreatesks(n, n, &d, &u, &s0, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( ae_fp_neq(a.ptr.pp_double[i][j],(double)(0)) )
                    {
                        seterrorflag(&result, !sparserewriteexisting(&s0, i, j, a.ptr.pp_double[i][j], _state), _state);
                    }
                }
            }
            sparsecopy(&s0, &s1, _state);
            sparseconverttocrs(&s1, _state);
            sparsecopytocrs(&s0, &s2, _state);
            sparsecopytocrsbuf(&s0, &s3, _state);
            sparsecopytohash(&s0, &s4, _state);
            sparsecopytohashbuf(&s0, &s5, _state);
            sparsecopy(&s0, &s6, _state);
            sparseconverttohash(&s6, _state);
            seterrorflag(&result, sparsegetnrows(&s0, _state)!=n, _state);
            seterrorflag(&result, sparsegetncols(&s0, _state)!=n, _state);
            seterrorflag(&result, sparsegetmatrixtype(&s0, _state)!=2, _state);
            seterrorflag(&result, !sparseissks(&s0, _state), _state);
            seterrorflag(&result, sparseiscrs(&s0, _state), _state);
            seterrorflag(&result, sparseishash(&s0, _state), _state);
            seterrorflag(&result, sparseissks(&s1, _state), _state);
            seterrorflag(&result, !sparseiscrs(&s1, _state), _state);
            seterrorflag(&result, sparseishash(&s1, _state), _state);
            for(i=0; i<=n-1; i++)
            {
                v1 = a.ptr.pp_double[i][i];
                v0 = sparsegetdiagonal(&s0, i, _state);
                seterrorflag(&result, ae_fp_neq(v0,v1), _state);
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v1 = a.ptr.pp_double[i][j];
                    v0 = sparseget(&s0, i, j, _state);
                    seterrorflag(&result, ae_fp_neq(v0,v1), _state);
                    v0 = sparseget(&s1, i, j, _state);
                    seterrorflag(&result, ae_fp_neq(v0,v1), _state);
                    v0 = sparseget(&s2, i, j, _state);
                    seterrorflag(&result, ae_fp_neq(v0,v1), _state);
                    v0 = sparseget(&s3, i, j, _state);
                    seterrorflag(&result, ae_fp_neq(v0,v1), _state);
                    v0 = sparseget(&s4, i, j, _state);
                    seterrorflag(&result, ae_fp_neq(v0,v1), _state);
                    v0 = sparseget(&s5, i, j, _state);
                    seterrorflag(&result, ae_fp_neq(v0,v1), _state);
                    v0 = sparseget(&s6, i, j, _state);
                    seterrorflag(&result, ae_fp_neq(v0,v1), _state);
                }
            }
            
            /*
             * Check enumeration capabilities:
             * * each element returned by SparseEnumerate() is returned only once
             * * each non-zero element of A was enumerated
             */
            ae_matrix_set_length(&wasenumerated, n, n, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    wasenumerated.ptr.pp_bool[i][j] = ae_false;
                }
            }
            t0 = 0;
            t1 = 0;
            while(sparseenumerate(&s0, &t0, &t1, &i, &j, &v0, _state))
            {
                seterrorflag(&result, wasenumerated.ptr.pp_bool[i][j], _state);
                wasenumerated.ptr.pp_bool[i][j] = ae_true;
            }
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( ae_fp_neq(a.ptr.pp_double[i][j],(double)(0)) )
                    {
                        seterrorflag(&result, !wasenumerated.ptr.pp_bool[i][j], _state);
                    }
                }
            }
            
            /*
             * Check UpperCnt()/LowerCnt()
             */
            seterrorflag(&result, sparsegetuppercount(&s0, _state)!=uppercnt, _state);
            seterrorflag(&result, sparsegetlowercount(&s0, _state)!=lowercnt, _state);
            
            /*
             * Check in-place transposition
             */
            sparsecopy(&s0, &s1, _state);
            sparsetransposesks(&s1, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v0 = sparseget(&s0, i, j, _state);
                    v1 = sparseget(&s1, j, i, _state);
                    seterrorflag(&result, ae_fp_neq(v0,v1), _state);
                }
            }
            
            /*
             * One more check - matrix is initially created in some other format
             * (CRS or Hash) and converted to SKS later.
             */
            sparsecreate(n, n, 0, &s0, _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( ae_fp_neq(a.ptr.pp_double[i][j],(double)(0)) )
                    {
                        sparseset(&s0, i, j, a.ptr.pp_double[i][j], _state);
                    }
                }
            }
            sparsecopy(&s0, &s1, _state);
            sparseconverttosks(&s1, _state);
            sparsecopytosks(&s0, &s2, _state);
            sparsecopytosksbuf(&s0, &s3, _state);
            seterrorflag(&result, !sparseissks(&s1, _state), _state);
            seterrorflag(&result, sparseiscrs(&s1, _state), _state);
            seterrorflag(&result, sparseishash(&s1, _state), _state);
            seterrorflag(&result, !sparseissks(&s2, _state), _state);
            seterrorflag(&result, sparseiscrs(&s2, _state), _state);
            seterrorflag(&result, sparseishash(&s2, _state), _state);
            seterrorflag(&result, !sparseissks(&s3, _state), _state);
            seterrorflag(&result, sparseiscrs(&s3, _state), _state);
            seterrorflag(&result, sparseishash(&s3, _state), _state);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    v1 = a.ptr.pp_double[i][j];
                    v0 = sparseget(&s1, i, j, _state);
                    seterrorflag(&result, ae_fp_neq(v0,v1), _state);
                    v0 = sparseget(&s2, i, j, _state);
                    seterrorflag(&result, ae_fp_neq(v0,v1), _state);
                    v0 = sparseget(&s3, i, j, _state);
                    seterrorflag(&result, ae_fp_neq(v0,v1), _state);
                }
            }
            
            /*
             * Increase problem sparcity and try one more time. 
             * Stop after testing NZ=0.
             */
            if( nz==0 )
            {
                break;
            }
            nz = nz/2;
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing basic functional

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool basicfunctest(ae_state *_state)
{
    ae_frame _frame_block;
    sparsematrix s;
    ae_int_t n;
    ae_int_t m;
    ae_int_t i;
    ae_int_t j;
    ae_int_t i1;
    ae_int_t j1;
    ae_int_t uppercnt;
    ae_int_t lowercnt;
    ae_matrix a;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _sparsematrix_init(&s, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);

    n = 10;
    m = 10;
    result = ae_false;
    for(i=1; i<=m-1; i++)
    {
        for(j=1; j<=n-1; j++)
        {
            sparsecreate(i, j, 1, &s, _state);
            ae_matrix_set_length(&a, i, j, _state);
            
            /*
             * Checking for Matrix with hash table type
             */
            uppercnt = 0;
            lowercnt = 0;
            for(i1=0; i1<=i-1; i1++)
            {
                for(j1=0; j1<=j-1; j1++)
                {
                    if( j1>i1 )
                    {
                        inc(&uppercnt, _state);
                    }
                    if( j1<i1 )
                    {
                        inc(&lowercnt, _state);
                    }
                    a.ptr.pp_double[i1][j1] = i1+j1+(double)((i+j)*(m+n))/(double)2;
                    a.ptr.pp_double[i1][j1] = a.ptr.pp_double[i1][j1]+1;
                    sparseset(&s, i1, j1, i1+j1+(double)((i+j)*(m+n))/(double)2, _state);
                    sparseadd(&s, i1, j1, (double)(1), _state);
                    if( ae_fp_neq(a.ptr.pp_double[i1][j1],sparseget(&s, i1, j1, _state)) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
            for(i1=0; i1<=ae_minint(i, j, _state)-1; i1++)
            {
                if( ae_fp_neq(a.ptr.pp_double[i1][i1],sparsegetdiagonal(&s, i1, _state)) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
            seterrorflag(&result, sparsegetuppercount(&s, _state)!=uppercnt, _state);
            seterrorflag(&result, sparsegetlowercount(&s, _state)!=lowercnt, _state);
            
            /*
             * Checking for Matrix with CRS type
             */
            sparseconverttocrs(&s, _state);
            for(i1=0; i1<=i-1; i1++)
            {
                for(j1=0; j1<=j-1; j1++)
                {
                    if( ae_fp_neq(a.ptr.pp_double[i1][j1],sparseget(&s, i1, j1, _state)) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
            for(i1=0; i1<=ae_minint(i, j, _state)-1; i1++)
            {
                if( ae_fp_neq(a.ptr.pp_double[i1][i1],sparsegetdiagonal(&s, i1, _state)) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
            seterrorflag(&result, sparsegetuppercount(&s, _state)!=uppercnt, _state);
            seterrorflag(&result, sparsegetlowercount(&s, _state)!=lowercnt, _state);
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing Level 2 unsymmetric linear algebra functions.
Additionally it tests SparseGet() for several matrix formats.
Returns True on failure.

  -- ALGLIB PROJECT --
     Copyright 20.01.2014 by Bochkanov Sergey
*************************************************************************/
ae_bool testlevel2unsymmetric(ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t m;
    ae_int_t n;
    ae_vector x0;
    ae_vector x1;
    ae_vector y0;
    ae_vector y1;
    ae_int_t i;
    ae_int_t j;
    ae_matrix a;
    sparsematrix s0;
    sparsematrix sa;
    double eps;
    double v;
    sparsegenerator g;
    hqrndstate rs;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&y0, 0, DT_REAL, _state);
    ae_vector_init(&y1, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&s0, _state);
    _sparsematrix_init(&sa, _state);
    _sparsegenerator_init(&g, _state);
    _hqrndstate_init(&rs, _state);

    eps = 10000*ae_machineepsilon;
    result = ae_false;
    hqrndrandomize(&rs, _state);
    
    /*
     * Test linear algebra functions
     */
    for(m=1; m<=20; m++)
    {
        for(n=1; n<=20; n++)
        {
            testsparseunit_initgenerator(m, n, 0, 0, &g, _state);
            while(testsparseunit_generatenext(&g, &a, &sa, _state))
            {
                
                /*
                 * Convert SA to desired storage format:
                 * * to CRS if M<>N
                 * * with 50% probability to CRS or SKS, if M=N
                 */
                if( m!=n||ae_fp_less(hqrnduniformr(&rs, _state),0.5) )
                {
                    sparsecopytocrs(&sa, &s0, _state);
                }
                else
                {
                    sparsecopytosks(&sa, &s0, _state);
                }
                
                /*
                 * Test SparseGet() for SA and S0 against matrix returned in A
                 */
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        seterrorflag(&result, ae_fp_greater(ae_fabs(sparseget(&sa, i, j, _state)-a.ptr.pp_double[i][j], _state),eps), _state);
                        seterrorflag(&result, ae_fp_greater(ae_fabs(sparseget(&s0, i, j, _state)-a.ptr.pp_double[i][j], _state),eps), _state);
                    }
                }
                
                /*
                 * Test SparseMV
                 */
                ae_vector_set_length(&x0, n, _state);
                ae_vector_set_length(&x1, n, _state);
                for(j=0; j<=n-1; j++)
                {
                    x0.ptr.p_double[j] = hqrnduniformr(&rs, _state)-0.5;
                    x1.ptr.p_double[j] = x0.ptr.p_double[j];
                }
                sparsemv(&s0, &x0, &y0, _state);
                seterrorflag(&result, y0.cnt<m, _state);
                if( result )
                {
                    ae_frame_leave(_state);
                    return result;
                }
                for(i=0; i<=m-1; i++)
                {
                    v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    seterrorflag(&result, ae_fp_greater(ae_fabs(v-y0.ptr.p_double[i], _state),eps), _state);
                }
                
                /*
                 * Test SparseMTV
                 */
                ae_vector_set_length(&x0, m, _state);
                ae_vector_set_length(&x1, m, _state);
                for(j=0; j<=m-1; j++)
                {
                    x0.ptr.p_double[j] = hqrnduniformr(&rs, _state)-0.5;
                    x1.ptr.p_double[j] = x0.ptr.p_double[j];
                }
                sparsemtv(&s0, &x0, &y0, _state);
                seterrorflag(&result, y0.cnt<n, _state);
                if( result )
                {
                    ae_frame_leave(_state);
                    return result;
                }
                for(j=0; j<=n-1; j++)
                {
                    v = ae_v_dotproduct(&a.ptr.pp_double[0][j], a.stride, &x1.ptr.p_double[0], 1, ae_v_len(0,m-1));
                    seterrorflag(&result, ae_fp_greater(ae_fabs(v-y0.ptr.p_double[j], _state),eps), _state);
                }
                
                /*
                 * Test SparseMV2
                 */
                if( m==n )
                {
                    ae_vector_set_length(&x0, n, _state);
                    ae_vector_set_length(&x1, n, _state);
                    for(j=0; j<=n-1; j++)
                    {
                        x0.ptr.p_double[j] = hqrnduniformr(&rs, _state)-0.5;
                        x1.ptr.p_double[j] = x0.ptr.p_double[j];
                    }
                    sparsemv2(&s0, &x0, &y0, &y1, _state);
                    seterrorflag(&result, y0.cnt<n, _state);
                    seterrorflag(&result, y1.cnt<n, _state);
                    if( result )
                    {
                        ae_frame_leave(_state);
                        return result;
                    }
                    for(j=0; j<=n-1; j++)
                    {
                        v = ae_v_dotproduct(&a.ptr.pp_double[j][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                        seterrorflag(&result, ae_fp_greater(ae_fabs(v-y0.ptr.p_double[j], _state),eps), _state);
                        v = ae_v_dotproduct(&a.ptr.pp_double[0][j], a.stride, &x1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                        seterrorflag(&result, ae_fp_greater(ae_fabs(v-y1.ptr.p_double[j], _state),eps), _state);
                    }
                }
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing Level 3 unsymmetric linear algebra functions.
Additionally it tests SparseGet() for several matrix formats.
Returns True on failure.

  -- ALGLIB PROJECT --
     Copyright 20.01.2014 by Bochkanov Sergey
*************************************************************************/
ae_bool testlevel3unsymmetric(ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t m;
    ae_int_t n;
    ae_int_t k;
    ae_matrix x0;
    ae_matrix x1;
    ae_matrix y0;
    ae_matrix y1;
    ae_int_t i;
    ae_int_t j;
    ae_matrix a;
    sparsematrix s0;
    sparsematrix sa;
    double eps;
    double v;
    sparsegenerator g;
    hqrndstate rs;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&x0, 0, 0, DT_REAL, _state);
    ae_matrix_init(&x1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&y0, 0, 0, DT_REAL, _state);
    ae_matrix_init(&y1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&s0, _state);
    _sparsematrix_init(&sa, _state);
    _sparsegenerator_init(&g, _state);
    _hqrndstate_init(&rs, _state);

    eps = 10000*ae_machineepsilon;
    result = ae_false;
    hqrndrandomize(&rs, _state);
    
    /*
     * Test linear algebra functions
     */
    for(m=1; m<=20; m++)
    {
        for(n=1; n<=20; n++)
        {
            testsparseunit_initgenerator(m, n, 0, 0, &g, _state);
            while(testsparseunit_generatenext(&g, &a, &sa, _state))
            {
                
                /*
                 * Choose matrix width K
                 */
                k = 1+hqrnduniformi(&rs, 20, _state);
                
                /*
                 * Convert SA to desired storage format:
                 * * to CRS if M<>N
                 * * with 50% probability to CRS or SKS, if M=N
                 */
                if( m!=n||ae_fp_less(hqrnduniformr(&rs, _state),0.5) )
                {
                    sparsecopytocrs(&sa, &s0, _state);
                }
                else
                {
                    sparsecopytosks(&sa, &s0, _state);
                }
                
                /*
                 * Test SparseGet() for SA and S0 against matrix returned in A
                 */
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        seterrorflag(&result, ae_fp_neq(sparseget(&sa, i, j, _state),a.ptr.pp_double[i][j]), _state);
                        seterrorflag(&result, ae_fp_neq(sparseget(&s0, i, j, _state),a.ptr.pp_double[i][j]), _state);
                    }
                }
                
                /*
                 * Test SparseMV
                 */
                ae_matrix_set_length(&x0, n, k, _state);
                ae_matrix_set_length(&x1, n, k, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=k-1; j++)
                    {
                        x0.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                        x1.ptr.pp_double[i][j] = x0.ptr.pp_double[i][j];
                    }
                }
                sparsemm(&s0, &x0, k, &y0, _state);
                seterrorflag(&result, y0.rows<m, _state);
                seterrorflag(&result, y0.cols<k, _state);
                if( result )
                {
                    ae_frame_leave(_state);
                    return result;
                }
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=k-1; j++)
                    {
                        v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &x1.ptr.pp_double[0][j], x1.stride, ae_v_len(0,n-1));
                        seterrorflag(&result, ae_fp_greater(ae_fabs(v-y0.ptr.pp_double[i][j], _state),eps), _state);
                    }
                }
                
                /*
                 * Test SparseMTM
                 */
                ae_matrix_set_length(&x0, m, k, _state);
                ae_matrix_set_length(&x1, m, k, _state);
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=k-1; j++)
                    {
                        x0.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                        x1.ptr.pp_double[i][j] = x0.ptr.pp_double[i][j];
                    }
                }
                sparsemtm(&s0, &x0, k, &y0, _state);
                seterrorflag(&result, y0.rows<n, _state);
                seterrorflag(&result, y0.cols<k, _state);
                if( result )
                {
                    ae_frame_leave(_state);
                    return result;
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=k-1; j++)
                    {
                        v = ae_v_dotproduct(&a.ptr.pp_double[0][i], a.stride, &x1.ptr.pp_double[0][j], x1.stride, ae_v_len(0,m-1));
                        seterrorflag(&result, ae_fp_greater(ae_fabs(v-y0.ptr.pp_double[i][j], _state),eps), _state);
                    }
                }
                
                /*
                 * Test SparseMM2
                 */
                if( m==n )
                {
                    ae_matrix_set_length(&x0, n, k, _state);
                    ae_matrix_set_length(&x1, n, k, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=k-1; j++)
                        {
                            x0.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                            x1.ptr.pp_double[i][j] = x0.ptr.pp_double[i][j];
                        }
                    }
                    sparsemm2(&s0, &x0, k, &y0, &y1, _state);
                    seterrorflag(&result, y0.rows<n, _state);
                    seterrorflag(&result, y0.cols<k, _state);
                    seterrorflag(&result, y1.rows<n, _state);
                    seterrorflag(&result, y1.cols<k, _state);
                    if( result )
                    {
                        ae_frame_leave(_state);
                        return result;
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=k-1; j++)
                        {
                            v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &x1.ptr.pp_double[0][j], x1.stride, ae_v_len(0,n-1));
                            seterrorflag(&result, ae_fp_greater(ae_fabs(v-y0.ptr.pp_double[i][j], _state),eps), _state);
                            v = ae_v_dotproduct(&a.ptr.pp_double[0][i], a.stride, &x1.ptr.pp_double[0][j], x1.stride, ae_v_len(0,n-1));
                            seterrorflag(&result, ae_fp_greater(ae_fabs(v-y1.ptr.pp_double[i][j], _state),eps), _state);
                        }
                    }
                }
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing Level 2 symmetric linear algebra functions.
Additionally it tests SparseGet() for several matrix formats.
Returns True on failure.

  -- ALGLIB PROJECT --
     Copyright 20.01.2014 by Bochkanov Sergey
*************************************************************************/
ae_bool testlevel2symmetric(ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_vector x0;
    ae_vector x1;
    ae_vector y0;
    ae_vector y1;
    ae_int_t i;
    ae_int_t j;
    ae_matrix a;
    sparsematrix s0;
    sparsematrix s1;
    sparsematrix sa;
    double eps;
    double v;
    double va;
    double vb;
    sparsegenerator g;
    hqrndstate rs;
    ae_bool isupper;
    ae_int_t triangletype;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&y0, 0, DT_REAL, _state);
    ae_vector_init(&y1, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&s0, _state);
    _sparsematrix_init(&s1, _state);
    _sparsematrix_init(&sa, _state);
    _sparsegenerator_init(&g, _state);
    _hqrndstate_init(&rs, _state);

    eps = 10000*ae_machineepsilon;
    result = ae_false;
    hqrndrandomize(&rs, _state);
    
    /*
     * Test linear algebra functions
     */
    for(n=1; n<=20; n++)
    {
        for(triangletype=-1; triangletype<=1; triangletype++)
        {
            isupper = ae_fp_greater(hqrnduniformr(&rs, _state),0.5);
            if( triangletype<0 )
            {
                isupper = ae_false;
            }
            if( triangletype>0 )
            {
                isupper = ae_true;
            }
            testsparseunit_initgenerator(n, n, 0, triangletype, &g, _state);
            while(testsparseunit_generatenext(&g, &a, &sa, _state))
            {
                
                /*
                 * Convert SA to desired storage format:
                 * * S0 stores unmodified copy
                 * * S1 stores copy with unmodified triangle corresponding
                 *   to IsUpper and another triangle being spoiled by random
                 *   trash
                 */
                sparsecopytohash(&sa, &s1, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( (j<i&&isupper)||(j>i&&!isupper) )
                        {
                            sparseset(&s1, i, j, hqrnduniformr(&rs, _state), _state);
                        }
                    }
                }
                if( ae_fp_less(hqrnduniformr(&rs, _state),0.5) )
                {
                    sparsecopytocrs(&sa, &s0, _state);
                    sparseconverttocrs(&s1, _state);
                }
                else
                {
                    sparsecopytosks(&sa, &s0, _state);
                    sparseconverttosks(&s1, _state);
                }
                
                /*
                 * Test SparseGet() for SA and S0 against matrix returned in A
                 */
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        seterrorflag(&result, ae_fp_greater(ae_fabs(sparseget(&sa, i, j, _state)-a.ptr.pp_double[i][j], _state),eps), _state);
                        seterrorflag(&result, ae_fp_greater(ae_fabs(sparseget(&s0, i, j, _state)-a.ptr.pp_double[i][j], _state),eps), _state);
                        seterrorflag(&result, (j<i&&triangletype==1)&&ae_fp_neq(sparseget(&s0, i, j, _state),(double)(0)), _state);
                        seterrorflag(&result, (j>i&&triangletype==-1)&&ae_fp_neq(sparseget(&s0, i, j, _state),(double)(0)), _state);
                    }
                }
                
                /*
                 * Before we proceed with testing, update empty triangle of A
                 * with its copy from another part of the matrix.
                 */
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( (j<i&&isupper)||(j>i&&!isupper) )
                        {
                            a.ptr.pp_double[i][j] = a.ptr.pp_double[j][i];
                        }
                    }
                }
                
                /*
                 * Test SparseSMV
                 */
                ae_vector_set_length(&x0, n, _state);
                ae_vector_set_length(&x1, n, _state);
                for(j=0; j<=n-1; j++)
                {
                    x0.ptr.p_double[j] = hqrnduniformr(&rs, _state)-0.5;
                    x1.ptr.p_double[j] = x0.ptr.p_double[j];
                }
                sparsesmv(&s0, isupper, &x0, &y0, _state);
                seterrorflag(&result, y0.cnt<n, _state);
                if( result )
                {
                    ae_frame_leave(_state);
                    return result;
                }
                for(i=0; i<=n-1; i++)
                {
                    v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    seterrorflag(&result, ae_fp_greater(ae_fabs(v-y0.ptr.p_double[i], _state),eps), _state);
                }
                sparsesmv(&s1, isupper, &x0, &y1, _state);
                seterrorflag(&result, y1.cnt<n, _state);
                if( result )
                {
                    ae_frame_leave(_state);
                    return result;
                }
                for(i=0; i<=n-1; i++)
                {
                    v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    seterrorflag(&result, ae_fp_greater(ae_fabs(v-y1.ptr.p_double[i], _state),eps), _state);
                }
                
                /*
                 * Test SparseVSMV
                 */
                ae_vector_set_length(&x0, n, _state);
                ae_vector_set_length(&x1, n, _state);
                for(j=0; j<=n-1; j++)
                {
                    x0.ptr.p_double[j] = hqrnduniformr(&rs, _state)-0.5;
                    x1.ptr.p_double[j] = x0.ptr.p_double[j];
                }
                vb = 0.0;
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        vb = vb+x1.ptr.p_double[i]*a.ptr.pp_double[i][j]*x1.ptr.p_double[j];
                    }
                }
                va = sparsevsmv(&s0, isupper, &x0, _state);
                seterrorflag(&result, ae_fp_greater(ae_fabs(va-vb, _state),eps), _state);
                va = sparsevsmv(&s1, isupper, &x0, _state);
                seterrorflag(&result, ae_fp_greater(ae_fabs(va-vb, _state),eps), _state);
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing Level 2 symmetric linear algebra functions.
Additionally it tests SparseGet() for several matrix formats.
Returns True on failure.

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool testlevel3symmetric(ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t k;
    ae_matrix x0;
    ae_matrix x1;
    ae_matrix y0;
    ae_matrix y1;
    ae_int_t i;
    ae_int_t j;
    ae_matrix a;
    sparsematrix s0;
    sparsematrix s1;
    sparsematrix sa;
    double eps;
    double v;
    sparsegenerator g;
    hqrndstate rs;
    ae_bool isupper;
    ae_int_t triangletype;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&x0, 0, 0, DT_REAL, _state);
    ae_matrix_init(&x1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&y0, 0, 0, DT_REAL, _state);
    ae_matrix_init(&y1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&s0, _state);
    _sparsematrix_init(&s1, _state);
    _sparsematrix_init(&sa, _state);
    _sparsegenerator_init(&g, _state);
    _hqrndstate_init(&rs, _state);

    eps = 10000*ae_machineepsilon;
    result = ae_false;
    hqrndrandomize(&rs, _state);
    
    /*
     * Test linear algebra functions
     */
    for(n=1; n<=20; n++)
    {
        for(triangletype=-1; triangletype<=1; triangletype++)
        {
            isupper = ae_fp_greater(hqrnduniformr(&rs, _state),0.5);
            if( triangletype<0 )
            {
                isupper = ae_false;
            }
            if( triangletype>0 )
            {
                isupper = ae_true;
            }
            testsparseunit_initgenerator(n, n, 0, triangletype, &g, _state);
            while(testsparseunit_generatenext(&g, &a, &sa, _state))
            {
                
                /*
                 * Choose matrix width K
                 */
                k = 1+hqrnduniformi(&rs, 20, _state);
                
                /*
                 * Convert SA to desired storage format:
                 * * S0 stores unmodified copy
                 * * S1 stores copy with unmodified triangle corresponding
                 *   to IsUpper and another triangle being spoiled by random
                 *   trash
                 */
                sparsecopytohash(&sa, &s1, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( (j<i&&isupper)||(j>i&&!isupper) )
                        {
                            sparseset(&s1, i, j, hqrnduniformr(&rs, _state), _state);
                        }
                    }
                }
                if( ae_fp_less(hqrnduniformr(&rs, _state),0.5) )
                {
                    sparsecopytocrs(&sa, &s0, _state);
                    sparseconverttocrs(&s1, _state);
                }
                else
                {
                    sparsecopytosks(&sa, &s0, _state);
                    sparseconverttosks(&s1, _state);
                }
                
                /*
                 * Test SparseGet() for SA and S0 against matrix returned in A
                 */
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        seterrorflag(&result, ae_fp_greater(ae_fabs(sparseget(&sa, i, j, _state)-a.ptr.pp_double[i][j], _state),eps), _state);
                        seterrorflag(&result, ae_fp_greater(ae_fabs(sparseget(&s0, i, j, _state)-a.ptr.pp_double[i][j], _state),eps), _state);
                        seterrorflag(&result, (j<i&&triangletype==1)&&ae_fp_neq(sparseget(&s0, i, j, _state),(double)(0)), _state);
                        seterrorflag(&result, (j>i&&triangletype==-1)&&ae_fp_neq(sparseget(&s0, i, j, _state),(double)(0)), _state);
                    }
                }
                
                /*
                 * Before we proceed with testing, update empty triangle of A
                 * with its copy from another part of the matrix.
                 */
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( (j<i&&isupper)||(j>i&&!isupper) )
                        {
                            a.ptr.pp_double[i][j] = a.ptr.pp_double[j][i];
                        }
                    }
                }
                
                /*
                 * Test SparseSMM
                 */
                ae_matrix_set_length(&x0, n, k, _state);
                ae_matrix_set_length(&x1, n, k, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=k-1; j++)
                    {
                        x0.ptr.pp_double[i][j] = hqrnduniformr(&rs, _state)-0.5;
                        x1.ptr.pp_double[i][j] = x0.ptr.pp_double[i][j];
                    }
                }
                sparsesmm(&s0, isupper, &x0, k, &y0, _state);
                seterrorflag(&result, y0.rows<n, _state);
                seterrorflag(&result, y0.cols<k, _state);
                if( result )
                {
                    ae_frame_leave(_state);
                    return result;
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=k-1; j++)
                    {
                        v = ae_v_dotproduct(&a.ptr.pp_double[i][0], 1, &x1.ptr.pp_double[0][j], x1.stride, ae_v_len(0,n-1));
                        seterrorflag(&result, ae_fp_greater(ae_fabs(v-y0.ptr.pp_double[i][j], _state),eps), _state);
                    }
                }
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing Level 2 triangular linear algebra functions.
Returns True on failure.

  -- ALGLIB PROJECT --
     Copyright 20.01.2014 by Bochkanov Sergey
*************************************************************************/
ae_bool testlevel2triangular(ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_vector x0;
    ae_vector x1;
    ae_vector y0;
    ae_vector y1;
    ae_vector ey;
    ae_int_t i;
    ae_int_t j;
    ae_int_t i1;
    ae_int_t j1;
    ae_matrix a;
    ae_matrix ea;
    sparsematrix s0;
    sparsematrix s1;
    sparsematrix sa;
    double eps;
    double v;
    sparsegenerator g;
    hqrndstate rs;
    ae_bool isupper;
    ae_bool isunit;
    ae_int_t optype;
    ae_int_t triangletype;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&y0, 0, DT_REAL, _state);
    ae_vector_init(&y1, 0, DT_REAL, _state);
    ae_vector_init(&ey, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ea, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&s0, _state);
    _sparsematrix_init(&s1, _state);
    _sparsematrix_init(&sa, _state);
    _sparsegenerator_init(&g, _state);
    _hqrndstate_init(&rs, _state);

    eps = 10000*ae_machineepsilon;
    result = ae_false;
    hqrndrandomize(&rs, _state);
    
    /*
     * Test sparseTRMV
     */
    for(n=1; n<=20; n++)
    {
        for(triangletype=-1; triangletype<=1; triangletype++)
        {
            isupper = ae_fp_greater(hqrnduniformr(&rs, _state),0.5);
            if( triangletype<0 )
            {
                isupper = ae_false;
            }
            if( triangletype>0 )
            {
                isupper = ae_true;
            }
            testsparseunit_initgenerator(n, n, 0, triangletype, &g, _state);
            while(testsparseunit_generatenext(&g, &a, &sa, _state))
            {
                
                /*
                 * Settings (IsUpper was already set, handle the rest)
                 */
                isunit = ae_fp_less(hqrnduniformr(&rs, _state),0.5);
                optype = hqrnduniformi(&rs, 2, _state);
                
                /*
                 * Convert SA to desired storage format:
                 * * S0 stores unmodified copy
                 * * S1 stores copy with unmodified triangle corresponding
                 *   to IsUpper and another triangle being spoiled by random
                 *   trash
                 */
                sparsecopytohash(&sa, &s1, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( (j<i&&isupper)||(j>i&&!isupper) )
                        {
                            sparseset(&s1, i, j, hqrnduniformr(&rs, _state), _state);
                        }
                    }
                }
                if( ae_fp_less(hqrnduniformr(&rs, _state),0.5) )
                {
                    sparsecopytocrs(&sa, &s0, _state);
                    sparseconverttocrs(&s1, _state);
                }
                else
                {
                    sparsecopytosks(&sa, &s0, _state);
                    sparseconverttosks(&s1, _state);
                }
                
                /*
                 * Generate "effective A"
                 */
                ae_matrix_set_length(&ea, n, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        ea.ptr.pp_double[i][j] = (double)(0);
                    }
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( (j>=i&&isupper)||(j<=i&&!isupper) )
                        {
                            i1 = i;
                            j1 = j;
                            if( optype==1 )
                            {
                                swapi(&i1, &j1, _state);
                            }
                            ea.ptr.pp_double[i1][j1] = a.ptr.pp_double[i][j];
                            if( isunit&&i1==j1 )
                            {
                                ea.ptr.pp_double[i1][j1] = 1.0;
                            }
                        }
                    }
                }
                
                /*
                 * Test SparseTRMV
                 */
                ae_vector_set_length(&x0, n, _state);
                ae_vector_set_length(&x1, n, _state);
                for(j=0; j<=n-1; j++)
                {
                    x0.ptr.p_double[j] = hqrnduniformr(&rs, _state)-0.5;
                    x1.ptr.p_double[j] = x0.ptr.p_double[j];
                }
                sparsetrmv(&s0, isupper, isunit, optype, &x0, &y0, _state);
                seterrorflag(&result, y0.cnt<n, _state);
                if( result )
                {
                    ae_frame_leave(_state);
                    return result;
                }
                for(i=0; i<=n-1; i++)
                {
                    v = ae_v_dotproduct(&ea.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    seterrorflag(&result, ae_fp_greater(ae_fabs(v-y0.ptr.p_double[i], _state),eps), _state);
                }
                sparsetrmv(&s0, isupper, isunit, optype, &x0, &y1, _state);
                seterrorflag(&result, y1.cnt<n, _state);
                if( result )
                {
                    ae_frame_leave(_state);
                    return result;
                }
                for(i=0; i<=n-1; i++)
                {
                    v = ae_v_dotproduct(&ea.ptr.pp_double[i][0], 1, &x1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    seterrorflag(&result, ae_fp_greater(ae_fabs(v-y1.ptr.p_double[i], _state),eps), _state);
                }
            }
        }
    }
    
    /*
     * Test sparseTRSV
     */
    for(n=1; n<=20; n++)
    {
        for(triangletype=-1; triangletype<=1; triangletype++)
        {
            isupper = ae_fp_greater(hqrnduniformr(&rs, _state),0.5);
            if( triangletype==-1 )
            {
                isupper = ae_false;
            }
            if( triangletype==1 )
            {
                isupper = ae_true;
            }
            testsparseunit_initgenerator(n, n, 1, triangletype, &g, _state);
            while(testsparseunit_generatenext(&g, &a, &sa, _state))
            {
                
                /*
                 * Settings (IsUpper was already set, handle the rest)
                 */
                isunit = ae_fp_less(hqrnduniformr(&rs, _state),0.5);
                optype = hqrnduniformi(&rs, 2, _state);
                
                /*
                 * Convert SA to desired storage format:
                 * * S0 stores unmodified copy
                 * * S1 stores copy with unmodified triangle corresponding
                 *   to IsUpper and another triangle being spoiled by random
                 *   trash
                 */
                sparsecopytohash(&sa, &s1, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( (j<i&&isupper)||(j>i&&!isupper) )
                        {
                            sparseset(&s1, i, j, hqrnduniformr(&rs, _state), _state);
                        }
                    }
                }
                if( ae_fp_less(hqrnduniformr(&rs, _state),0.5) )
                {
                    sparsecopytocrs(&sa, &s0, _state);
                    sparseconverttocrs(&s1, _state);
                }
                else
                {
                    sparsecopytosks(&sa, &s0, _state);
                    sparseconverttosks(&s1, _state);
                }
                
                /*
                 * Generate "effective A" and EY = inv(EA)*x0
                 */
                ae_matrix_set_length(&ea, n, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        ea.ptr.pp_double[i][j] = (double)(0);
                    }
                }
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( (j>=i&&isupper)||(j<=i&&!isupper) )
                        {
                            i1 = i;
                            j1 = j;
                            if( optype==1 )
                            {
                                swapi(&i1, &j1, _state);
                            }
                            ea.ptr.pp_double[i1][j1] = a.ptr.pp_double[i][j];
                            if( isunit&&i1==j1 )
                            {
                                ea.ptr.pp_double[i1][j1] = 1.0;
                            }
                        }
                    }
                }
                ae_vector_set_length(&ey, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    ey.ptr.p_double[i] = hqrnduniformr(&rs, _state)-0.5;
                }
                ae_vector_set_length(&x0, n, _state);
                ae_vector_set_length(&x1, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    v = ae_v_dotproduct(&ea.ptr.pp_double[i][0], 1, &ey.ptr.p_double[0], 1, ae_v_len(0,n-1));
                    x0.ptr.p_double[i] = v;
                    x1.ptr.p_double[i] = v;
                }
                
                /*
                 * Test SparseTRSV
                 */
                sparsetrsv(&s0, isupper, isunit, optype, &x0, _state);
                seterrorflag(&result, x0.cnt<n, _state);
                if( result )
                {
                    ae_frame_leave(_state);
                    return result;
                }
                for(i=0; i<=n-1; i++)
                {
                    seterrorflag(&result, ae_fp_greater(ae_fabs(ey.ptr.p_double[i]-x0.ptr.p_double[i], _state),eps), _state);
                }
                sparsetrsv(&s1, isupper, isunit, optype, &x1, _state);
                seterrorflag(&result, x1.cnt<n, _state);
                if( result )
                {
                    ae_frame_leave(_state);
                    return result;
                }
                for(i=0; i<=n-1; i++)
                {
                    seterrorflag(&result, ae_fp_greater(ae_fabs(ey.ptr.p_double[i]-x1.ptr.p_double[i], _state),eps), _state);
                }
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing basic functional

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool basicfuncrandomtest(ae_state *_state)
{
    ae_frame _frame_block;
    sparsematrix s;
    ae_int_t n;
    ae_int_t m;
    ae_int_t i;
    ae_int_t j;
    ae_int_t i1;
    ae_int_t j1;
    ae_matrix a;
    ae_int_t mfigure;
    ae_int_t temp;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _sparsematrix_init(&s, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);

    n = 20;
    m = 20;
    mfigure = 10;
    for(i=1; i<=m-1; i++)
    {
        for(j=1; j<=n-1; j++)
        {
            sparsecreate(i, j, 0, &s, _state);
            ae_matrix_set_length(&a, i, j, _state);
            
            /*
             * Checking for Matrix with hash table type
             */
            for(i1=0; i1<=i-1; i1++)
            {
                for(j1=0; j1<=j-1; j1++)
                {
                    temp = 2*ae_randominteger(mfigure, _state)-mfigure;
                    a.ptr.pp_double[i1][j1] = (double)(temp);
                    if( ae_randominteger(2, _state)==0 )
                    {
                        sparseset(&s, i1, j1, (double)(temp), _state);
                        sparseset(&s, i1, j1, (double)(temp), _state);
                    }
                    else
                    {
                        sparseadd(&s, i1, j1, (double)(temp), _state);
                        sparseadd(&s, i1, j1, (double)(0), _state);
                    }
                    if( ae_fp_neq(a.ptr.pp_double[i1][j1],sparseget(&s, i1, j1, _state)) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
            
            /*
             * Nulling all elements
             */
            for(i1=0; i1<=i-1; i1++)
            {
                for(j1=0; j1<=j-1; j1++)
                {
                    if( ae_randominteger(2, _state)==0 )
                    {
                        sparseset(&s, i1, j1, (double)(0), _state);
                    }
                    else
                    {
                        sparseadd(&s, i1, j1, -1*sparseget(&s, i1, j1, _state), _state);
                    }
                }
            }
            
            /*
             * Again initialization of the matrix and check new values
             */
            for(i1=0; i1<=i-1; i1++)
            {
                for(j1=0; j1<=j-1; j1++)
                {
                    temp = 2*ae_randominteger(mfigure, _state)-mfigure;
                    a.ptr.pp_double[i1][j1] = (double)(temp);
                    if( ae_randominteger(2, _state)==0 )
                    {
                        sparseset(&s, i1, j1, (double)(temp), _state);
                    }
                    else
                    {
                        sparseadd(&s, i1, j1, (double)(temp), _state);
                    }
                    if( ae_fp_neq(a.ptr.pp_double[i1][j1],sparseget(&s, i1, j1, _state)) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
            
            /*
             * Checking for Matrix with CRS type
             */
            sparseconverttocrs(&s, _state);
            for(i1=0; i1<=i-1; i1++)
            {
                for(j1=0; j1<=j-1; j1++)
                {
                    if( ae_fp_neq(a.ptr.pp_double[i1][j1],sparseget(&s, i1, j1, _state)) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing multyplication matrix with vector

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool linearfunctionstest(ae_state *_state)
{
    ae_frame _frame_block;
    sparsematrix s;
    ae_int_t n;
    ae_int_t m;
    ae_int_t i;
    ae_int_t j;
    ae_int_t i1;
    ae_int_t j1;
    double lb;
    double rb;
    ae_matrix a;
    ae_vector x0;
    ae_vector x1;
    ae_vector ty;
    ae_vector tyt;
    ae_vector y;
    ae_vector yt;
    ae_vector y0;
    ae_vector yt0;
    double eps;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _sparsematrix_init(&s, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&ty, 0, DT_REAL, _state);
    ae_vector_init(&tyt, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&yt, 0, DT_REAL, _state);
    ae_vector_init(&y0, 0, DT_REAL, _state);
    ae_vector_init(&yt0, 0, DT_REAL, _state);

    
    /*
     * Accuracy
     */
    eps = 1000*ae_machineepsilon;
    
    /*
     * Size of the matrix (m*n)
     */
    n = 10;
    m = 10;
    
    /*
     * Left and right borders, limiting matrix values
     */
    lb = (double)(-10);
    rb = (double)(10);
    
    /*
     * Test linear algebra functions for:
     * a) sparse matrix converted to CRS from Hash-Table
     * b) sparse matrix initially created as CRS
     */
    for(i=1; i<=m-1; i++)
    {
        for(j=1; j<=n-1; j++)
        {
            
            /*
             * Prepare test problem
             */
            testsparseunit_createrandom(i, j, -1, -1, -1, -1, &a, &s, _state);
            
            /*
             * Initialize temporaries
             */
            ae_vector_set_length(&ty, i, _state);
            ae_vector_set_length(&tyt, j, _state);
            for(i1=0; i1<=i-1; i1++)
            {
                ty.ptr.p_double[i1] = (double)(0);
            }
            for(i1=0; i1<=j-1; i1++)
            {
                tyt.ptr.p_double[i1] = (double)(0);
            }
            ae_vector_set_length(&x0, j, _state);
            ae_vector_set_length(&x1, i, _state);
            for(i1=0; i1<=j-1; i1++)
            {
                x0.ptr.p_double[i1] = (rb-lb)*ae_randomreal(_state)+lb;
            }
            for(i1=0; i1<=i-1; i1++)
            {
                x1.ptr.p_double[i1] = (rb-lb)*ae_randomreal(_state)+lb;
            }
            
            /*
             * Consider two cases: square matrix, and non-square matrix
             */
            if( i!=j )
            {
                
                /*
                 * Searching true result
                 */
                for(i1=0; i1<=i-1; i1++)
                {
                    for(j1=0; j1<=j-1; j1++)
                    {
                        ty.ptr.p_double[i1] = ty.ptr.p_double[i1]+a.ptr.pp_double[i1][j1]*x0.ptr.p_double[j1];
                        tyt.ptr.p_double[j1] = tyt.ptr.p_double[j1]+a.ptr.pp_double[i1][j1]*x1.ptr.p_double[i1];
                    }
                }
                
                /*
                 * Multiplication
                 */
                sparsemv(&s, &x0, &y, _state);
                sparsemtv(&s, &x1, &yt, _state);
                
                /*
                 * Check for MV-result
                 */
                for(i1=0; i1<=i-1; i1++)
                {
                    if( ae_fp_greater_eq(ae_fabs(y.ptr.p_double[i1]-ty.ptr.p_double[i1], _state),eps) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
                
                /*
                 * Check for MTV-result
                 */
                for(i1=0; i1<=j-1; i1++)
                {
                    if( ae_fp_greater_eq(ae_fabs(yt.ptr.p_double[i1]-tyt.ptr.p_double[i1], _state),eps) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
            else
            {
                
                /*
                 * Searching true result
                 */
                for(i1=0; i1<=i-1; i1++)
                {
                    for(j1=0; j1<=j-1; j1++)
                    {
                        ty.ptr.p_double[i1] = ty.ptr.p_double[i1]+a.ptr.pp_double[i1][j1]*x0.ptr.p_double[j1];
                        tyt.ptr.p_double[j1] = tyt.ptr.p_double[j1]+a.ptr.pp_double[i1][j1]*x0.ptr.p_double[i1];
                    }
                }
                sparsemv(&s, &x0, &y, _state);
                sparsemtv(&s, &x0, &yt, _state);
                sparsemv2(&s, &x0, &y0, &yt0, _state);
                
                /*
                 * Check for MV2-result
                 */
                for(i1=0; i1<=i-1; i1++)
                {
                    if( ae_fp_greater_eq(ae_fabs(y0.ptr.p_double[i1]-ty.ptr.p_double[i1], _state),eps)||ae_fp_greater_eq(ae_fabs(yt0.ptr.p_double[i1]-tyt.ptr.p_double[i1], _state),eps) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
                
                /*
                 * Check for MV- and MTV-result by help MV2
                 */
                for(i1=0; i1<=i-1; i1++)
                {
                    if( ae_fp_greater(ae_fabs(y0.ptr.p_double[i1]-y.ptr.p_double[i1], _state),eps)||ae_fp_greater(ae_fabs(yt0.ptr.p_double[i1]-yt.ptr.p_double[i1], _state),eps) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing multyplication for simmetric matrix with vector

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool linearfunctionsstest(ae_state *_state)
{
    ae_frame _frame_block;
    sparsematrix s;
    ae_int_t m;
    ae_int_t i;
    ae_int_t i1;
    ae_int_t j1;
    double lb;
    double rb;
    ae_matrix a;
    ae_vector x0;
    ae_vector x1;
    ae_vector ty;
    ae_vector tyt;
    ae_vector y;
    ae_vector yt;
    double eps;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _sparsematrix_init(&s, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&ty, 0, DT_REAL, _state);
    ae_vector_init(&tyt, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&yt, 0, DT_REAL, _state);

    
    /*
     *Accuracy
     */
    eps = 1000*ae_machineepsilon;
    
    /*
     * Size of the matrix (m*m)
     */
    m = 10;
    
    /*
     * Left and right borders, limiting matrix values
     */
    lb = (double)(-10);
    rb = (double)(10);
    
    /*
     * Test linear algebra functions for:
     * a) sparse matrix converted to CRS from Hash-Table
     * b) sparse matrix initially created as CRS
     */
    for(i=1; i<=m-1; i++)
    {
        
        /*
         * Prepare test problem
         */
        testsparseunit_createrandom(i, i, -1, -1, -1, -1, &a, &s, _state);
        
        /*
         * Initialize temporaries
         */
        ae_vector_set_length(&ty, i, _state);
        ae_vector_set_length(&tyt, i, _state);
        ae_vector_set_length(&x0, i, _state);
        ae_vector_set_length(&x1, i, _state);
        for(i1=0; i1<=i-1; i1++)
        {
            ty.ptr.p_double[i1] = (double)(0);
            tyt.ptr.p_double[i1] = (double)(0);
            x0.ptr.p_double[i1] = (rb-lb)*ae_randomreal(_state)+lb;
            x1.ptr.p_double[i1] = (rb-lb)*ae_randomreal(_state)+lb;
        }
        
        /*
         * Searching true result for upper and lower triangles
         * of the matrix
         */
        for(i1=0; i1<=i-1; i1++)
        {
            for(j1=i1; j1<=i-1; j1++)
            {
                ty.ptr.p_double[i1] = ty.ptr.p_double[i1]+a.ptr.pp_double[i1][j1]*x0.ptr.p_double[j1];
                if( i1!=j1 )
                {
                    ty.ptr.p_double[j1] = ty.ptr.p_double[j1]+a.ptr.pp_double[i1][j1]*x0.ptr.p_double[i1];
                }
            }
        }
        for(i1=0; i1<=i-1; i1++)
        {
            for(j1=0; j1<=i1; j1++)
            {
                tyt.ptr.p_double[i1] = tyt.ptr.p_double[i1]+a.ptr.pp_double[i1][j1]*x1.ptr.p_double[j1];
                if( i1!=j1 )
                {
                    tyt.ptr.p_double[j1] = tyt.ptr.p_double[j1]+a.ptr.pp_double[i1][j1]*x1.ptr.p_double[i1];
                }
            }
        }
        
        /*
         * Multiplication
         */
        sparsesmv(&s, ae_true, &x0, &y, _state);
        sparsesmv(&s, ae_false, &x1, &yt, _state);
        
        /*
         * Check for SMV-result
         */
        for(i1=0; i1<=i-1; i1++)
        {
            if( ae_fp_greater_eq(ae_fabs(y.ptr.p_double[i1]-ty.ptr.p_double[i1], _state),eps)||ae_fp_greater_eq(ae_fabs(yt.ptr.p_double[i1]-tyt.ptr.p_double[i1], _state),eps) )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing multyplication sparse matrix with nerrow dense matrix

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool linearfunctionsmmtest(ae_state *_state)
{
    ae_frame _frame_block;
    sparsematrix s;
    ae_int_t n;
    ae_int_t m;
    ae_int_t kmax;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t i1;
    ae_int_t j1;
    ae_int_t k1;
    double lb;
    double rb;
    ae_matrix a;
    ae_matrix x0;
    ae_matrix x1;
    ae_matrix ty;
    ae_matrix tyt;
    ae_matrix y;
    ae_matrix yt;
    ae_matrix y0;
    ae_matrix yt0;
    double eps;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _sparsematrix_init(&s, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&x0, 0, 0, DT_REAL, _state);
    ae_matrix_init(&x1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ty, 0, 0, DT_REAL, _state);
    ae_matrix_init(&tyt, 0, 0, DT_REAL, _state);
    ae_matrix_init(&y, 0, 0, DT_REAL, _state);
    ae_matrix_init(&yt, 0, 0, DT_REAL, _state);
    ae_matrix_init(&y0, 0, 0, DT_REAL, _state);
    ae_matrix_init(&yt0, 0, 0, DT_REAL, _state);

    
    /*
     * Accuracy
     */
    eps = 1000*ae_machineepsilon;
    
    /*
     * Size of the matrix (m*n)
     */
    n = 32;
    m = 32;
    kmax = 32;
    
    /*
     * Left and right borders, limiting matrix values
     */
    lb = (double)(-10);
    rb = (double)(10);
    
    /*
     * Test linear algebra functions for:
     * a) sparse matrix converted to CRS from Hash-Table
     * b) sparse matrix initially created as CRS
     */
    for(i=1; i<=m-1; i++)
    {
        for(j=1; j<=n-1; j++)
        {
            
            /*
             * Prepare test problem
             */
            testsparseunit_createrandom(i, j, -1, -1, -1, -1, &a, &s, _state);
            ae_matrix_set_length(&x0, j, kmax, _state);
            ae_matrix_set_length(&x1, i, kmax, _state);
            for(i1=0; i1<=j-1; i1++)
            {
                for(j1=0; j1<=kmax-1; j1++)
                {
                    x0.ptr.pp_double[i1][j1] = (rb-lb)*ae_randomreal(_state)+lb;
                }
            }
            for(i1=0; i1<=i-1; i1++)
            {
                for(j1=0; j1<=kmax-1; j1++)
                {
                    x1.ptr.pp_double[i1][j1] = (rb-lb)*ae_randomreal(_state)+lb;
                }
            }
            ae_matrix_set_length(&ty, i, kmax, _state);
            ae_matrix_set_length(&tyt, j, kmax, _state);
            for(i1=0; i1<=i-1; i1++)
            {
                for(j1=0; j1<=kmax-1; j1++)
                {
                    ty.ptr.pp_double[i1][j1] = (double)(0);
                }
            }
            for(i1=0; i1<=j-1; i1++)
            {
                for(j1=0; j1<=kmax-1; j1++)
                {
                    tyt.ptr.pp_double[i1][j1] = (double)(0);
                }
            }
            if( i!=j )
            {
                for(i1=0; i1<=i-1; i1++)
                {
                    for(k1=0; k1<=kmax-1; k1++)
                    {
                        for(j1=0; j1<=j-1; j1++)
                        {
                            ty.ptr.pp_double[i1][k1] = ty.ptr.pp_double[i1][k1]+a.ptr.pp_double[i1][j1]*x0.ptr.pp_double[j1][k1];
                            tyt.ptr.pp_double[j1][k1] = tyt.ptr.pp_double[j1][k1]+a.ptr.pp_double[i1][j1]*x1.ptr.pp_double[i1][k1];
                        }
                    }
                }
            }
            else
            {
                for(i1=0; i1<=i-1; i1++)
                {
                    for(k1=0; k1<=kmax-1; k1++)
                    {
                        for(j1=0; j1<=j-1; j1++)
                        {
                            ty.ptr.pp_double[i1][k1] = ty.ptr.pp_double[i1][k1]+a.ptr.pp_double[i1][j1]*x0.ptr.pp_double[j1][k1];
                            tyt.ptr.pp_double[j1][k1] = tyt.ptr.pp_double[j1][k1]+a.ptr.pp_double[i1][j1]*x0.ptr.pp_double[i1][k1];
                        }
                    }
                }
            }
            for(k=1; k<=kmax; k++)
            {
                
                /*
                 * Consider two cases: square matrix, and non-square matrix
                 */
                if( i!=j )
                {
                    
                    /*
                     * Multiplication
                     */
                    sparsemm(&s, &x0, k, &y, _state);
                    sparsemtm(&s, &x1, k, &yt, _state);
                    
                    /*
                     * Check for MM-result
                     */
                    for(i1=0; i1<=i-1; i1++)
                    {
                        for(j1=0; j1<=k-1; j1++)
                        {
                            if( ae_fp_greater_eq(ae_fabs(y.ptr.pp_double[i1][j1]-ty.ptr.pp_double[i1][j1], _state),eps) )
                            {
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                        }
                    }
                    
                    /*
                     * Check for MTM-result
                     */
                    for(i1=0; i1<=j-1; i1++)
                    {
                        for(j1=0; j1<=k-1; j1++)
                        {
                            if( ae_fp_greater_eq(ae_fabs(yt.ptr.pp_double[i1][j1]-tyt.ptr.pp_double[i1][j1], _state),eps) )
                            {
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                        }
                    }
                }
                else
                {
                    sparsemm(&s, &x0, k, &y, _state);
                    sparsemtm(&s, &x0, k, &yt, _state);
                    sparsemm2(&s, &x0, k, &y0, &yt0, _state);
                    
                    /*
                     * Check for MM2-result
                     */
                    for(i1=0; i1<=i-1; i1++)
                    {
                        for(j1=0; j1<=k-1; j1++)
                        {
                            if( ae_fp_greater_eq(ae_fabs(y0.ptr.pp_double[i1][j1]-ty.ptr.pp_double[i1][j1], _state),eps)||ae_fp_greater_eq(ae_fabs(yt0.ptr.pp_double[i1][j1]-tyt.ptr.pp_double[i1][j1], _state),eps) )
                            {
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                        }
                    }
                    
                    /*
                     * Check for MV- and MTM-result by help MV2
                     */
                    for(i1=0; i1<=i-1; i1++)
                    {
                        for(j1=0; j1<=k-1; j1++)
                        {
                            if( ae_fp_greater(ae_fabs(y0.ptr.pp_double[i1][j1]-y.ptr.pp_double[i1][j1], _state),eps)||ae_fp_greater(ae_fabs(yt0.ptr.pp_double[i1][j1]-yt.ptr.pp_double[i1][j1], _state),eps) )
                            {
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                        }
                    }
                }
            }
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing multyplication for simmetric sparse matrix with narrow
dense matrix

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool linearfunctionssmmtest(ae_state *_state)
{
    ae_frame _frame_block;
    sparsematrix s;
    ae_int_t m;
    ae_int_t k;
    ae_int_t i;
    ae_int_t j;
    ae_int_t i1;
    ae_int_t j1;
    ae_int_t k1;
    double lb;
    double rb;
    ae_matrix a;
    ae_matrix x0;
    ae_matrix x1;
    ae_matrix ty;
    ae_matrix tyt;
    ae_matrix y;
    ae_matrix yt;
    double eps;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _sparsematrix_init(&s, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&x0, 0, 0, DT_REAL, _state);
    ae_matrix_init(&x1, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ty, 0, 0, DT_REAL, _state);
    ae_matrix_init(&tyt, 0, 0, DT_REAL, _state);
    ae_matrix_init(&y, 0, 0, DT_REAL, _state);
    ae_matrix_init(&yt, 0, 0, DT_REAL, _state);

    
    /*
     * Accuracy
     */
    eps = 1000*ae_machineepsilon;
    
    /*
     * Size of the matrix (m*m)
     */
    m = 32;
    k = 32;
    
    /*
     * Left and right borders, limiting matrix values
     */
    lb = (double)(-10);
    rb = (double)(10);
    
    /*
     * Test linear algebra functions for:
     * a) sparse matrix converted to CRS from Hash-Table
     * b) sparse matrix initially created as CRS
     */
    for(i=1; i<=m-1; i++)
    {
        for(j=1; j<=k-1; j++)
        {
            
            /*
             * Prepare test problem
             */
            testsparseunit_createrandom(i, i, -1, -1, -1, -1, &a, &s, _state);
            
            /*
             * Initialize temporaries
             */
            ae_matrix_set_length(&ty, i, j, _state);
            ae_matrix_set_length(&tyt, i, j, _state);
            ae_matrix_set_length(&x0, i, j, _state);
            ae_matrix_set_length(&x1, i, j, _state);
            for(i1=0; i1<=i-1; i1++)
            {
                for(j1=0; j1<=j-1; j1++)
                {
                    ty.ptr.pp_double[i1][j1] = (double)(0);
                    tyt.ptr.pp_double[i1][j1] = (double)(0);
                    x0.ptr.pp_double[i1][j1] = (rb-lb)*ae_randomreal(_state)+lb;
                    x1.ptr.pp_double[i1][j1] = (rb-lb)*ae_randomreal(_state)+lb;
                }
            }
            
            /*
             * Searching true result for upper and lower triangles
             * of the matrix
             */
            for(k1=0; k1<=j-1; k1++)
            {
                for(i1=0; i1<=i-1; i1++)
                {
                    for(j1=i1; j1<=i-1; j1++)
                    {
                        ty.ptr.pp_double[i1][k1] = ty.ptr.pp_double[i1][k1]+a.ptr.pp_double[i1][j1]*x0.ptr.pp_double[j1][k1];
                        if( i1!=j1 )
                        {
                            ty.ptr.pp_double[j1][k1] = ty.ptr.pp_double[j1][k1]+a.ptr.pp_double[i1][j1]*x0.ptr.pp_double[i1][k1];
                        }
                    }
                }
            }
            for(k1=0; k1<=j-1; k1++)
            {
                for(i1=0; i1<=i-1; i1++)
                {
                    for(j1=0; j1<=i1; j1++)
                    {
                        tyt.ptr.pp_double[i1][k1] = tyt.ptr.pp_double[i1][k1]+a.ptr.pp_double[i1][j1]*x1.ptr.pp_double[j1][k1];
                        if( i1!=j1 )
                        {
                            tyt.ptr.pp_double[j1][k1] = tyt.ptr.pp_double[j1][k1]+a.ptr.pp_double[i1][j1]*x1.ptr.pp_double[i1][k1];
                        }
                    }
                }
            }
            
            /*
             * Multiplication
             */
            sparsesmm(&s, ae_true, &x0, j, &y, _state);
            sparsesmm(&s, ae_false, &x1, j, &yt, _state);
            
            /*
             * Check for SMM-result
             */
            for(k1=0; k1<=j-1; k1++)
            {
                for(i1=0; i1<=i-1; i1++)
                {
                    if( ae_fp_greater_eq(ae_fabs(y.ptr.pp_double[i1][k1]-ty.ptr.pp_double[i1][k1], _state),eps)||ae_fp_greater_eq(ae_fabs(yt.ptr.pp_double[i1][k1]-tyt.ptr.pp_double[i1][k1], _state),eps) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for basic test SparseCopy

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool basiccopyfunctest(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    sparsematrix s;
    sparsematrix ss;
    sparsematrix sss;
    ae_int_t n;
    ae_int_t m;
    ae_vector ner;
    ae_int_t i;
    ae_int_t j;
    ae_int_t i1;
    ae_int_t j1;
    ae_matrix a;
    double a0;
    double a1;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _sparsematrix_init(&s, _state);
    _sparsematrix_init(&ss, _state);
    _sparsematrix_init(&sss, _state);
    ae_vector_init(&ner, 0, DT_INT, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);

    n = 30;
    m = 30;
    for(i=1; i<=m-1; i++)
    {
        for(j=1; j<=n-1; j++)
        {
            sparsecreate(i, j, 1, &s, _state);
            ae_matrix_set_length(&a, i, j, _state);
            ae_vector_set_length(&ner, i, _state);
            for(i1=0; i1<=i-1; i1++)
            {
                if( i1<=j-3 )
                {
                    ner.ptr.p_int[i1] = 2;
                }
                else
                {
                    if( j-3<i1&&i1<=j-2 )
                    {
                        ner.ptr.p_int[i1] = 1;
                    }
                    else
                    {
                        ner.ptr.p_int[i1] = 0;
                    }
                }
            }
            sparsecreatecrs(i, j, &ner, &sss, _state);
            
            /*
             * Checking for Matrix with hash table type
             */
            for(i1=0; i1<=i-1; i1++)
            {
                for(j1=0; j1<=j-1; j1++)
                {
                    if( j1>i1&&j1<=i1+2 )
                    {
                        a.ptr.pp_double[i1][j1] = (double)(i1+j1+1);
                        sparseset(&s, i1, j1, a.ptr.pp_double[i1][j1], _state);
                        sparseadd(&s, i1, j1, (double)(0), _state);
                        sparseset(&sss, i1, j1, a.ptr.pp_double[i1][j1], _state);
                    }
                    else
                    {
                        a.ptr.pp_double[i1][j1] = (double)(0);
                        sparseset(&s, i1, j1, a.ptr.pp_double[i1][j1], _state);
                        sparseadd(&s, i1, j1, (double)(0), _state);
                    }
                    
                    /*
                     * Check for SparseCreate
                     */
                    sparsecopy(&s, &ss, _state);
                    a0 = sparseget(&s, i1, j1, _state);
                    a1 = sparseget(&ss, i1, j1, _state);
                    if( ae_fp_neq(a0,a1) )
                    {
                        if( !silent )
                        {
                            printf("BasicCopyFuncTest::Report::SparseGet\n");
                            printf("S::[%0d,%0d]=%0.5f\n",
                                (int)(i1),
                                (int)(j1),
                                (double)(a0));
                            printf("SS::[%0d,%0d]=%0.5f\n",
                                (int)(i1),
                                (int)(j1),
                                (double)(a1));
                            printf("          TEST FAILED.\n");
                        }
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
            
            /*
             * Check for SparseCreateCRS
             */
            for(i1=0; i1<=i-1; i1++)
            {
                for(j1=0; j1<=j-1; j1++)
                {
                    sparsecopy(&sss, &ss, _state);
                    a0 = sparseget(&sss, i1, j1, _state);
                    a1 = sparseget(&ss, i1, j1, _state);
                    if( ae_fp_neq(a0,a1) )
                    {
                        if( !silent )
                        {
                            printf("BasicCopyFuncTest::Report::SparseGet\n");
                            printf("S::[%0d,%0d]=%0.5f\n",
                                (int)(i1),
                                (int)(j1),
                                (double)(a0));
                            printf("SS::[%0d,%0d]=%0.5f\n",
                                (int)(i1),
                                (int)(j1),
                                (double)(a1));
                            printf("          TEST FAILED.\n");
                        }
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
            
            /*
             * Check for Matrix with CRS type
             */
            sparseconverttocrs(&s, _state);
            sparsecopy(&s, &ss, _state);
            for(i1=0; i1<=i-1; i1++)
            {
                for(j1=0; j1<=j-1; j1++)
                {
                    a0 = sparseget(&s, i1, j1, _state);
                    a1 = sparseget(&ss, i1, j1, _state);
                    if( ae_fp_neq(a0,a1) )
                    {
                        if( !silent )
                        {
                            printf("BasicCopyFuncTest::Report::SparseGet\n");
                            printf("S::[%0d,%0d]=%0.5f\n",
                                (int)(i1),
                                (int)(j1),
                                (double)(a0));
                            printf("SS::[%0d,%0d]=%0.5f\n",
                                (int)(i1),
                                (int)(j1),
                                (double)(a1));
                            printf("          TEST FAILED.\n");
                        }
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
        }
    }
    if( !silent )
    {
        printf("          TEST IS PASSED.\n");
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing SparseCopy

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool copyfunctest(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    sparsematrix s;
    sparsematrix ss;
    ae_int_t n;
    ae_int_t m;
    ae_int_t mtype;
    ae_int_t i;
    ae_int_t j;
    ae_int_t i1;
    ae_int_t j1;
    double lb;
    double rb;
    ae_matrix a;
    ae_vector x0;
    ae_vector x1;
    ae_vector ty;
    ae_vector tyt;
    ae_vector y;
    ae_vector yt;
    ae_vector y0;
    ae_vector yt0;
    ae_vector cpy;
    ae_vector cpyt;
    ae_vector cpy0;
    ae_vector cpyt0;
    double eps;
    double a0;
    double a1;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _sparsematrix_init(&s, _state);
    _sparsematrix_init(&ss, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&ty, 0, DT_REAL, _state);
    ae_vector_init(&tyt, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&yt, 0, DT_REAL, _state);
    ae_vector_init(&y0, 0, DT_REAL, _state);
    ae_vector_init(&yt0, 0, DT_REAL, _state);
    ae_vector_init(&cpy, 0, DT_REAL, _state);
    ae_vector_init(&cpyt, 0, DT_REAL, _state);
    ae_vector_init(&cpy0, 0, DT_REAL, _state);
    ae_vector_init(&cpyt0, 0, DT_REAL, _state);

    
    /*
     * Accuracy
     */
    eps = 1000*ae_machineepsilon;
    
    /*
     * Size of the matrix (m*n)
     */
    n = 30;
    m = 30;
    
    /*
     * Left and right borders, limiting matrix values
     */
    lb = (double)(-10);
    rb = (double)(10);
    
    /*
     * Test linear algebra functions for:
     * a) sparse matrix converted to CRS from Hash-Table
     * b) sparse matrix initially created as CRS
     */
    for(i=1; i<=m-1; i++)
    {
        for(j=1; j<=n-1; j++)
        {
            for(mtype=0; mtype<=1; mtype++)
            {
                
                /*
                 * Prepare test problem
                 */
                testsparseunit_createrandom(i, j, -1, mtype, -1, -1, &a, &s, _state);
                sparsecopy(&s, &ss, _state);
                
                /*
                 * Initialize temporaries
                 */
                ae_vector_set_length(&ty, i, _state);
                ae_vector_set_length(&tyt, j, _state);
                for(i1=0; i1<=i-1; i1++)
                {
                    ty.ptr.p_double[i1] = (double)(0);
                }
                for(i1=0; i1<=j-1; i1++)
                {
                    tyt.ptr.p_double[i1] = (double)(0);
                }
                ae_vector_set_length(&x0, j, _state);
                ae_vector_set_length(&x1, i, _state);
                for(i1=0; i1<=j-1; i1++)
                {
                    x0.ptr.p_double[i1] = (rb-lb)*ae_randomreal(_state)+lb;
                }
                for(i1=0; i1<=i-1; i1++)
                {
                    x1.ptr.p_double[i1] = (rb-lb)*ae_randomreal(_state)+lb;
                }
                
                /*
                 * Consider two cases: square matrix, and non-square matrix
                 */
                if( i!=j )
                {
                    
                    /*
                     * Searching true result
                     */
                    for(i1=0; i1<=i-1; i1++)
                    {
                        for(j1=0; j1<=j-1; j1++)
                        {
                            ty.ptr.p_double[i1] = ty.ptr.p_double[i1]+a.ptr.pp_double[i1][j1]*x0.ptr.p_double[j1];
                            tyt.ptr.p_double[j1] = tyt.ptr.p_double[j1]+a.ptr.pp_double[i1][j1]*x1.ptr.p_double[i1];
                        }
                    }
                    
                    /*
                     * Multiplication
                     */
                    sparsemv(&s, &x0, &y, _state);
                    sparsemtv(&s, &x1, &yt, _state);
                    sparsemv(&ss, &x0, &cpy, _state);
                    sparsemtv(&ss, &x1, &cpyt, _state);
                    
                    /*
                     * Check for MV-result
                     */
                    for(i1=0; i1<=i-1; i1++)
                    {
                        if( (ae_fp_greater_eq(ae_fabs(y.ptr.p_double[i1]-ty.ptr.p_double[i1], _state),eps)||ae_fp_greater_eq(ae_fabs(cpy.ptr.p_double[i1]-ty.ptr.p_double[i1], _state),eps))||ae_fp_neq(cpy.ptr.p_double[i1]-y.ptr.p_double[i1],(double)(0)) )
                        {
                            if( !silent )
                            {
                                printf("CopyFuncTest::Report::RES_MV\n");
                                printf("Y[%0d]=%0.5f; tY[%0d]=%0.5f\n",
                                    (int)(i1),
                                    (double)(y.ptr.p_double[i1]),
                                    (int)(i1),
                                    (double)(ty.ptr.p_double[i1]));
                                printf("cpY[%0d]=%0.5f;\n",
                                    (int)(i1),
                                    (double)(cpy.ptr.p_double[i1]));
                                printf("          TEST FAILED.\n");
                            }
                            result = ae_true;
                            ae_frame_leave(_state);
                            return result;
                        }
                    }
                    
                    /*
                     * Check for MTV-result
                     */
                    for(i1=0; i1<=j-1; i1++)
                    {
                        if( (ae_fp_greater_eq(ae_fabs(yt.ptr.p_double[i1]-tyt.ptr.p_double[i1], _state),eps)||ae_fp_greater_eq(ae_fabs(cpyt.ptr.p_double[i1]-tyt.ptr.p_double[i1], _state),eps))||ae_fp_neq(cpyt.ptr.p_double[i1]-yt.ptr.p_double[i1],(double)(0)) )
                        {
                            if( !silent )
                            {
                                printf("CopyFuncTest::Report::RES_MTV\n");
                                printf("Yt[%0d]=%0.5f; tYt[%0d]=%0.5f\n",
                                    (int)(i1),
                                    (double)(yt.ptr.p_double[i1]),
                                    (int)(i1),
                                    (double)(tyt.ptr.p_double[i1]));
                                printf("cpYt[%0d]=%0.5f;\n",
                                    (int)(i1),
                                    (double)(cpyt.ptr.p_double[i1]));
                                printf("          TEST FAILED.\n");
                            }
                            result = ae_true;
                            ae_frame_leave(_state);
                            return result;
                        }
                    }
                    sparsecopy(&s, &ss, _state);
                    for(i1=0; i1<=i-1; i1++)
                    {
                        for(j1=0; j1<=j-1; j1++)
                        {
                            a0 = sparseget(&s, i1, j1, _state);
                            a1 = sparseget(&ss, i1, j1, _state);
                            if( ae_fp_neq(a0,a1) )
                            {
                                if( !silent )
                                {
                                    printf("CopyFuncTest::Report::SparseGet\n");
                                    printf("S::[%0d,%0d]=%0.5f\n",
                                        (int)(i1),
                                        (int)(j1),
                                        (double)(a0));
                                    printf("SS::[%0d,%0d]=%0.5f\n",
                                        (int)(i1),
                                        (int)(j1),
                                        (double)(a1));
                                    printf("          TEST FAILED.\n");
                                }
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                        }
                    }
                }
                else
                {
                    
                    /*
                     * Searching true result
                     */
                    for(i1=0; i1<=i-1; i1++)
                    {
                        for(j1=0; j1<=j-1; j1++)
                        {
                            ty.ptr.p_double[i1] = ty.ptr.p_double[i1]+a.ptr.pp_double[i1][j1]*x0.ptr.p_double[j1];
                            tyt.ptr.p_double[j1] = tyt.ptr.p_double[j1]+a.ptr.pp_double[i1][j1]*x0.ptr.p_double[i1];
                        }
                    }
                    
                    /*
                     * Multiplication
                     */
                    sparsemv(&s, &x0, &y, _state);
                    sparsemtv(&s, &x0, &yt, _state);
                    sparsemv2(&s, &x0, &y0, &yt0, _state);
                    sparsemv(&ss, &x0, &cpy, _state);
                    sparsemtv(&ss, &x0, &cpyt, _state);
                    sparsemv2(&ss, &x0, &cpy0, &cpyt0, _state);
                    
                    /*
                     * Check for MV2-result
                     */
                    for(i1=0; i1<=i-1; i1++)
                    {
                        if( ((((ae_fp_greater_eq(ae_fabs(y0.ptr.p_double[i1]-ty.ptr.p_double[i1], _state),eps)||ae_fp_greater_eq(ae_fabs(yt0.ptr.p_double[i1]-tyt.ptr.p_double[i1], _state),eps))||ae_fp_greater_eq(ae_fabs(cpy0.ptr.p_double[i1]-ty.ptr.p_double[i1], _state),eps))||ae_fp_greater_eq(ae_fabs(cpyt0.ptr.p_double[i1]-tyt.ptr.p_double[i1], _state),eps))||ae_fp_neq(cpy0.ptr.p_double[i1]-y0.ptr.p_double[i1],(double)(0)))||ae_fp_neq(cpyt0.ptr.p_double[i1]-yt0.ptr.p_double[i1],(double)(0)) )
                        {
                            if( !silent )
                            {
                                printf("CopyFuncTest::Report::RES_MV2\n");
                                printf("Y0[%0d]=%0.5f; tY[%0d]=%0.5f\n",
                                    (int)(i1),
                                    (double)(y0.ptr.p_double[i1]),
                                    (int)(i1),
                                    (double)(ty.ptr.p_double[i1]));
                                printf("Yt0[%0d]=%0.5f; tYt[%0d]=%0.5f\n",
                                    (int)(i1),
                                    (double)(yt0.ptr.p_double[i1]),
                                    (int)(i1),
                                    (double)(tyt.ptr.p_double[i1]));
                                printf("cpY0[%0d]=%0.5f;\n",
                                    (int)(i1),
                                    (double)(cpy0.ptr.p_double[i1]));
                                printf("cpYt0[%0d]=%0.5f;\n",
                                    (int)(i1),
                                    (double)(cpyt0.ptr.p_double[i1]));
                                printf("          TEST FAILED.\n");
                            }
                            result = ae_true;
                            ae_frame_leave(_state);
                            return result;
                        }
                    }
                    
                    /*
                     * Check for MV- and MTV-result by help MV2
                     */
                    for(i1=0; i1<=i-1; i1++)
                    {
                        if( ((ae_fp_greater(ae_fabs(y0.ptr.p_double[i1]-y.ptr.p_double[i1], _state),eps)||ae_fp_greater(ae_fabs(yt0.ptr.p_double[i1]-yt.ptr.p_double[i1], _state),eps))||ae_fp_greater(ae_fabs(cpy0.ptr.p_double[i1]-cpy.ptr.p_double[i1], _state),eps))||ae_fp_greater(ae_fabs(cpyt0.ptr.p_double[i1]-cpyt.ptr.p_double[i1], _state),eps) )
                        {
                            if( !silent )
                            {
                                printf("CopyFuncTest::Report::RES_MV_MVT\n");
                                printf("Y0[%0d]=%0.5f; Y[%0d]=%0.5f\n",
                                    (int)(i1),
                                    (double)(y0.ptr.p_double[i1]),
                                    (int)(i1),
                                    (double)(y.ptr.p_double[i1]));
                                printf("Yt0[%0d]=%0.5f; Yt[%0d]=%0.5f\n",
                                    (int)(i1),
                                    (double)(yt0.ptr.p_double[i1]),
                                    (int)(i1),
                                    (double)(yt.ptr.p_double[i1]));
                                printf("cpY0[%0d]=%0.5f; cpY[%0d]=%0.5f\n",
                                    (int)(i1),
                                    (double)(cpy0.ptr.p_double[i1]),
                                    (int)(i1),
                                    (double)(cpy.ptr.p_double[i1]));
                                printf("cpYt0[%0d]=%0.5f; cpYt[%0d]=%0.5f\n",
                                    (int)(i1),
                                    (double)(cpyt0.ptr.p_double[i1]),
                                    (int)(i1),
                                    (double)(cpyt.ptr.p_double[i1]));
                                printf("          TEST FAILED.\n");
                            }
                            result = ae_true;
                            ae_frame_leave(_state);
                            return result;
                        }
                    }
                    sparsecopy(&s, &ss, _state);
                    for(i1=0; i1<=i-1; i1++)
                    {
                        for(j1=0; j1<=j-1; j1++)
                        {
                            a0 = sparseget(&s, i1, j1, _state);
                            a1 = sparseget(&ss, i1, j1, _state);
                            if( ae_fp_neq(a0,a1) )
                            {
                                if( !silent )
                                {
                                    printf("CopyFuncTest::Report::SparseGet\n");
                                    printf("S::[%0d,%0d]=%0.5f\n",
                                        (int)(i1),
                                        (int)(j1),
                                        (double)(a0));
                                    printf("SS::[%0d,%0d]=%0.5f\n",
                                        (int)(i1),
                                        (int)(j1),
                                        (double)(a1));
                                    printf("          TEST FAILED.\n");
                                }
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                        }
                    }
                }
            }
        }
    }
    if( !silent )
    {
        printf("          TEST IS PASSED.\n");
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function initializes sparse matrix generator, which is used to generate
a set of matrices with sequentially increasing sparsity.

PARAMETERS:
    M, N        -   matrix size. If M=0, then matrix is square N*N.
                    N and M must be small enough to store N*M dense matrix.
    MatKind     -   matrix properties:
                    * 0     -   general sparse (no structure)
                    * 1     -   general sparse, but diagonal is always present and non-zero
                    * 2     -   diagonally dominant, SPD
    Triangle    -   triangle being returned:
                    * +1    -   upper triangle
                    * -1    -   lower triangle
                    *  0    -   full matrix is returned
                    
OUTPUT PARAMETERS:
    G           -   generator
    A           -   matrix A in dense format
    SA          -   matrix A in sparse format (hash-table storage)
*************************************************************************/
static void testsparseunit_initgenerator(ae_int_t m,
     ae_int_t n,
     ae_int_t matkind,
     ae_int_t triangle,
     sparsegenerator* g,
     ae_state *_state)
{

    _sparsegenerator_clear(g);

    g->n = n;
    g->m = m;
    g->matkind = matkind;
    g->triangle = triangle;
    hqrndrandomize(&g->rs, _state);
    ae_vector_set_length(&g->rcs.ia, 5+1, _state);
    ae_vector_set_length(&g->rcs.ra, 1+1, _state);
    g->rcs.stage = -1;
}


static ae_bool testsparseunit_generatenext(sparsegenerator* g,
     /* Real    */ ae_matrix* da,
     sparsematrix* sa,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t m;
    ae_int_t nz;
    ae_int_t nzd;
    double pnz;
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_bool result;

    ae_matrix_clear(da);
    _sparsematrix_clear(sa);

    
    /*
     * Reverse communication preparations
     * I know it looks ugly, but it works the same way
     * anywhere from C++ to Python.
     *
     * This code initializes locals by:
     * * random values determined during code
     *   generation - on first subroutine call
     * * values from previous call - on subsequent calls
     */
    if( g->rcs.stage>=0 )
    {
        n = g->rcs.ia.ptr.p_int[0];
        m = g->rcs.ia.ptr.p_int[1];
        nz = g->rcs.ia.ptr.p_int[2];
        nzd = g->rcs.ia.ptr.p_int[3];
        i = g->rcs.ia.ptr.p_int[4];
        j = g->rcs.ia.ptr.p_int[5];
        pnz = g->rcs.ra.ptr.p_double[0];
        v = g->rcs.ra.ptr.p_double[1];
    }
    else
    {
        n = -983;
        m = -989;
        nz = -834;
        nzd = 900;
        i = -287;
        j = 364;
        pnz = 214;
        v = -338;
    }
    if( g->rcs.stage==0 )
    {
        goto lbl_0;
    }
    if( g->rcs.stage==1 )
    {
        goto lbl_1;
    }
    
    /*
     * Routine body
     */
    n = g->n;
    if( g->m==0 )
    {
        m = n;
    }
    else
    {
        m = g->m;
    }
    ae_assert(m>0&&n>0, "GenerateNext: incorrect N/M", _state);
    
    /*
     * Generate general sparse matrix
     */
    if( g->matkind!=0 )
    {
        goto lbl_2;
    }
    nz = n*m;
lbl_4:
    if( ae_false )
    {
        goto lbl_5;
    }
    
    /*
     * Generate dense N*N matrix where probability of element
     * being non-zero is PNZ.
     */
    pnz = (double)nz/(double)(n*m);
    ae_matrix_set_length(&g->bufa, m, n, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( ae_fp_less_eq(hqrnduniformr(&g->rs, _state),pnz) )
            {
                g->bufa.ptr.pp_double[i][j] = hqrnduniformr(&g->rs, _state)-0.5;
            }
            else
            {
                g->bufa.ptr.pp_double[i][j] = 0.0;
            }
        }
    }
    
    /*
     * Output matrix and RComm
     */
    ae_matrix_set_length(da, m, n, _state);
    sparsecreate(m, n, ae_round(pnz*m*n, _state), sa, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( (j<=i&&g->triangle<=0)||(j>=i&&g->triangle>=0) )
            {
                da->ptr.pp_double[i][j] = g->bufa.ptr.pp_double[i][j];
                sparseset(sa, i, j, g->bufa.ptr.pp_double[i][j], _state);
            }
            else
            {
                da->ptr.pp_double[i][j] = 0.0;
            }
        }
    }
    g->rcs.stage = 0;
    goto lbl_rcomm;
lbl_0:
    
    /*
     * Increase problem sparcity and try one more time. 
     * Stop after testing NZ=0.
     */
    if( nz==0 )
    {
        goto lbl_5;
    }
    nz = nz/2;
    goto lbl_4;
lbl_5:
    result = ae_false;
    return result;
lbl_2:
    
    /*
     * Generate general sparse matrix with non-zero diagonal
     */
    if( g->matkind!=1 )
    {
        goto lbl_6;
    }
    ae_assert(n==m, "GenerateNext: non-square matrix for MatKind=1", _state);
    nz = n*n-n;
lbl_8:
    if( ae_false )
    {
        goto lbl_9;
    }
    
    /*
     * Generate dense N*N matrix where probability of non-diagonal element
     * being non-zero is PNZ.
     */
    if( n>1 )
    {
        pnz = (double)nz/(double)(n*n-n);
    }
    else
    {
        pnz = (double)(1);
    }
    ae_matrix_set_length(&g->bufa, n, n, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( i==j )
            {
                do
                {
                    g->bufa.ptr.pp_double[i][i] = hqrnduniformr(&g->rs, _state)-0.5;
                }
                while(ae_fp_eq(g->bufa.ptr.pp_double[i][i],(double)(0)));
                g->bufa.ptr.pp_double[i][i] = g->bufa.ptr.pp_double[i][i]+1.5*ae_sign(g->bufa.ptr.pp_double[i][i], _state);
                continue;
            }
            if( ae_fp_less_eq(hqrnduniformr(&g->rs, _state),pnz) )
            {
                g->bufa.ptr.pp_double[i][j] = hqrnduniformr(&g->rs, _state)-0.5;
            }
            else
            {
                g->bufa.ptr.pp_double[i][j] = 0.0;
            }
        }
    }
    
    /*
     * Output matrix and RComm
     */
    ae_matrix_set_length(da, n, n, _state);
    sparsecreate(n, n, ae_round(pnz*(n*n-n)+n, _state), sa, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( (j<=i&&g->triangle<=0)||(j>=i&&g->triangle>=0) )
            {
                da->ptr.pp_double[i][j] = g->bufa.ptr.pp_double[i][j];
                sparseset(sa, i, j, g->bufa.ptr.pp_double[i][j], _state);
            }
            else
            {
                da->ptr.pp_double[i][j] = 0.0;
            }
        }
    }
    g->rcs.stage = 1;
    goto lbl_rcomm;
lbl_1:
    
    /*
     * Increase problem sparcity and try one more time. 
     * Stop after testing NZ=0.
     */
    if( nz==0 )
    {
        goto lbl_9;
    }
    nz = nz/2;
    goto lbl_8;
lbl_9:
    result = ae_false;
    return result;
lbl_6:
    ae_assert(ae_false, "Assertion failed", _state);
    result = ae_false;
    return result;
    
    /*
     * Saving state
     */
lbl_rcomm:
    result = ae_true;
    g->rcs.ia.ptr.p_int[0] = n;
    g->rcs.ia.ptr.p_int[1] = m;
    g->rcs.ia.ptr.p_int[2] = nz;
    g->rcs.ia.ptr.p_int[3] = nzd;
    g->rcs.ia.ptr.p_int[4] = i;
    g->rcs.ia.ptr.p_int[5] = j;
    g->rcs.ra.ptr.p_double[0] = pnz;
    g->rcs.ra.ptr.p_double[1] = v;
    return result;
}


/*************************************************************************
This function creates random sparse matrix with some prescribed pattern.

INPUT PARAMETERS:
    M       -   number of rows
    N       -   number of columns
    PKind   -   sparsity pattern:
                *-1 = pattern is chosen at random as well as P0/P1
                * 0 = matrix with up to P0 non-zero elements at random locations
                      (however, actual number of non-zero elements can be
                      less than P0, and in fact can be zero)
                * 1 = band matrix with P0 non-zero elements below diagonal
                      and P1 non-zero element above diagonal
                * 2 = matrix with random number of contiguous non-zero 
                      elements in the each row
    CKind   -   creation type:
                *-1 = CKind is chosen at random
                * 0 = matrix is created in Hash-Table format and converted
                      to CRS representation
                * 1 = matrix is created in CRS format

OUTPUT PARAMETERS:
    DA      -   dense representation of A, array[M,N]
    SA      -   sparse representation of A, in CRS format

  -- ALGLIB PROJECT --
     Copyright 31.10.2011 by Bochkanov Sergey
*************************************************************************/
static void testsparseunit_createrandom(ae_int_t m,
     ae_int_t n,
     ae_int_t pkind,
     ae_int_t ckind,
     ae_int_t p0,
     ae_int_t p1,
     /* Real    */ ae_matrix* da,
     sparsematrix* sa,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t maxpkind;
    ae_int_t maxckind;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    ae_vector c0;
    ae_vector c1;
    ae_vector rowsizes;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(da);
    _sparsematrix_clear(sa);
    ae_vector_init(&c0, 0, DT_INT, _state);
    ae_vector_init(&c1, 0, DT_INT, _state);
    ae_vector_init(&rowsizes, 0, DT_INT, _state);

    maxpkind = 2;
    maxckind = 1;
    ae_assert(m>=1, "CreateRandom: incorrect parameters", _state);
    ae_assert(n>=1, "CreateRandom: incorrect parameters", _state);
    ae_assert(pkind>=-1&&pkind<=maxpkind, "CreateRandom: incorrect parameters", _state);
    ae_assert(ckind>=-1&&ckind<=maxckind, "CreateRandom: incorrect parameters", _state);
    if( pkind==-1 )
    {
        pkind = ae_randominteger(maxpkind+1, _state);
        if( pkind==0 )
        {
            p0 = ae_randominteger(m*n, _state);
        }
        if( pkind==1 )
        {
            p0 = ae_randominteger(ae_minint(m, n, _state), _state);
            p1 = ae_randominteger(ae_minint(m, n, _state), _state);
        }
    }
    if( ckind==-1 )
    {
        ckind = ae_randominteger(maxckind+1, _state);
    }
    if( pkind==0 )
    {
        
        /*
         * Matrix with elements at random locations
         */
        ae_matrix_set_length(da, m, n, _state);
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                da->ptr.pp_double[i][j] = (double)(0);
            }
        }
        if( ckind==0 )
        {
            
            /*
             * Create matrix in Hash format, convert to CRS
             */
            sparsecreate(m, n, 1, sa, _state);
            for(k=0; k<=p0-1; k++)
            {
                i = ae_randominteger(m, _state);
                j = ae_randominteger(n, _state);
                v = (double)(ae_randominteger(17, _state)-8)/(double)8;
                if( ae_fp_greater(ae_randomreal(_state),0.5) )
                {
                    da->ptr.pp_double[i][j] = v;
                    sparseset(sa, i, j, v, _state);
                }
                else
                {
                    da->ptr.pp_double[i][j] = da->ptr.pp_double[i][j]+v;
                    sparseadd(sa, i, j, v, _state);
                }
            }
            sparseconverttocrs(sa, _state);
            ae_frame_leave(_state);
            return;
        }
        if( ckind==1 )
        {
            
            /*
             * Create matrix in CRS format
             */
            for(k=0; k<=p0-1; k++)
            {
                i = ae_randominteger(m, _state);
                j = ae_randominteger(n, _state);
                v = (double)(ae_randominteger(17, _state)-8)/(double)8;
                da->ptr.pp_double[i][j] = v;
            }
            ae_vector_set_length(&rowsizes, m, _state);
            for(i=0; i<=m-1; i++)
            {
                rowsizes.ptr.p_int[i] = 0;
                for(j=0; j<=n-1; j++)
                {
                    if( ae_fp_neq(da->ptr.pp_double[i][j],(double)(0)) )
                    {
                        rowsizes.ptr.p_int[i] = rowsizes.ptr.p_int[i]+1;
                    }
                }
            }
            sparsecreatecrs(m, n, &rowsizes, sa, _state);
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    if( ae_fp_neq(da->ptr.pp_double[i][j],(double)(0)) )
                    {
                        sparseset(sa, i, j, da->ptr.pp_double[i][j], _state);
                    }
                }
            }
            ae_frame_leave(_state);
            return;
        }
        ae_assert(ae_false, "CreateRandom: internal error", _state);
    }
    if( pkind==1 )
    {
        
        /*
         * Band matrix
         */
        ae_matrix_set_length(da, m, n, _state);
        ae_vector_set_length(&rowsizes, m, _state);
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                da->ptr.pp_double[i][j] = (double)(0);
            }
        }
        for(i=0; i<=m-1; i++)
        {
            for(j=ae_maxint(i-p0, 0, _state); j<=ae_minint(i+p1, n-1, _state); j++)
            {
                do
                {
                    da->ptr.pp_double[i][j] = (double)(ae_randominteger(17, _state)-8)/(double)8;
                }
                while(ae_fp_eq(da->ptr.pp_double[i][j],(double)(0)));
            }
            rowsizes.ptr.p_int[i] = ae_maxint(ae_minint(i+p1, n-1, _state)-ae_maxint(i-p0, 0, _state)+1, 0, _state);
        }
        if( ckind==0 )
        {
            sparsecreate(m, n, 1, sa, _state);
        }
        if( ckind==1 )
        {
            sparsecreatecrs(m, n, &rowsizes, sa, _state);
        }
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( ae_fp_neq(da->ptr.pp_double[i][j],(double)(0)) )
                {
                    sparseset(sa, i, j, da->ptr.pp_double[i][j], _state);
                }
            }
        }
        sparseconverttocrs(sa, _state);
        ae_frame_leave(_state);
        return;
    }
    if( pkind==2 )
    {
        
        /*
         * Matrix with one contiguous sequence of non-zero elements per row
         */
        ae_matrix_set_length(da, m, n, _state);
        ae_vector_set_length(&rowsizes, m, _state);
        ae_vector_set_length(&c0, m, _state);
        ae_vector_set_length(&c1, m, _state);
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                da->ptr.pp_double[i][j] = (double)(0);
            }
        }
        for(i=0; i<=m-1; i++)
        {
            c0.ptr.p_int[i] = ae_randominteger(n, _state);
            c1.ptr.p_int[i] = c0.ptr.p_int[i]+ae_randominteger(n-c0.ptr.p_int[i]+1, _state);
            rowsizes.ptr.p_int[i] = c1.ptr.p_int[i]-c0.ptr.p_int[i];
        }
        for(i=0; i<=m-1; i++)
        {
            for(j=c0.ptr.p_int[i]; j<=c1.ptr.p_int[i]-1; j++)
            {
                do
                {
                    da->ptr.pp_double[i][j] = (double)(ae_randominteger(17, _state)-8)/(double)8;
                }
                while(ae_fp_eq(da->ptr.pp_double[i][j],(double)(0)));
            }
        }
        if( ckind==0 )
        {
            sparsecreate(m, n, 1, sa, _state);
        }
        if( ckind==1 )
        {
            sparsecreatecrs(m, n, &rowsizes, sa, _state);
        }
        for(i=0; i<=m-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( ae_fp_neq(da->ptr.pp_double[i][j],(double)(0)) )
                {
                    sparseset(sa, i, j, da->ptr.pp_double[i][j], _state);
                }
            }
        }
        sparseconverttocrs(sa, _state);
        ae_frame_leave(_state);
        return;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function does test for SparseEnumerate function.

  -- ALGLIB PROJECT --
     Copyright 14.03.2012 by Bochkanov Sergey
*************************************************************************/
static ae_bool testsparseunit_enumeratetest(ae_state *_state)
{
    ae_frame _frame_block;
    sparsematrix spa;
    ae_matrix a;
    ae_matrix ta;
    ae_int_t m;
    ae_int_t n;
    double r;
    double v;
    ae_int_t ne;
    ae_int_t t0;
    ae_int_t t1;
    ae_int_t counter;
    ae_int_t c;
    ae_int_t hashcrs;
    ae_int_t i;
    ae_int_t j;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _sparsematrix_init(&spa, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ta, 0, 0, DT_BOOL, _state);

    r = 10.5;
    for(m=1; m<=30; m++)
    {
        for(n=1; n<=30; n++)
        {
            ne = 0;
            
            /*
             * Create matrix with non-zero elements inside the region:
             * 0<=I<S.M and 0<=J<S.N
             */
            ae_matrix_set_length(&a, m, n, _state);
            ae_matrix_set_length(&ta, m, n, _state);
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    a.ptr.pp_double[i][j] = (double)(0);
                    ta.ptr.pp_bool[i][j] = ae_false;
                }
            }
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=n-1; j++)
                {
                    c = ae_randominteger(2, _state);
                    if( c==0 )
                    {
                        a.ptr.pp_double[i][j] = (double)(0);
                    }
                    else
                    {
                        a.ptr.pp_double[i][j] = r*(2*ae_randomreal(_state)-1);
                        
                        /*
                         * Number of non-zero elements
                         */
                        ne = ne+1;
                    }
                }
            }
            for(hashcrs=0; hashcrs<=1; hashcrs++)
            {
                sparsecreate(m, n, m*n, &spa, _state);
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        sparseset(&spa, i, j, a.ptr.pp_double[i][j], _state);
                    }
                }
                if( hashcrs==1 )
                {
                    sparseconverttocrs(&spa, _state);
                }
                t0 = 0;
                t1 = 0;
                counter = 0;
                while(sparseenumerate(&spa, &t0, &t1, &i, &j, &v, _state))
                {
                    ta.ptr.pp_bool[i][j] = ae_true;
                    counter = counter+1;
                    if( ae_fp_neq(v,a.ptr.pp_double[i][j]) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
                
                /*
                 * Check that all non-zero elements was enumerated
                 */
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( ta.ptr.pp_bool[i][j]&&ae_fp_eq(a.ptr.pp_double[i][j],(double)(0)) )
                        {
                            result = ae_true;
                            ae_frame_leave(_state);
                            return result;
                        }
                    }
                }
                if( ne!=counter )
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


/*************************************************************************
This function does test for SparseRewriteExisting function.

  -- ALGLIB PROJECT --
     Copyright 14.03.2012 by Bochkanov Sergey
*************************************************************************/
static ae_bool testsparseunit_rewriteexistingtest(ae_state *_state)
{
    ae_frame _frame_block;
    sparsematrix spa;
    double spaval;
    ae_matrix a;
    ae_matrix ta;
    ae_int_t m;
    ae_int_t n;
    ae_int_t c;
    ae_int_t ne;
    ae_int_t nr;
    double r;
    double v;
    ae_int_t hashcrs;
    ae_int_t i;
    ae_int_t j;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _sparsematrix_init(&spa, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ta, 0, 0, DT_BOOL, _state);

    r = 20.0;
    for(m=1; m<=30; m++)
    {
        for(n=1; n<=30; n++)
        {
            ae_matrix_set_length(&a, m, n, _state);
            ae_matrix_set_length(&ta, m, n, _state);
            for(hashcrs=0; hashcrs<=1; hashcrs++)
            {
                v = r*(2*ae_randomreal(_state)-1);
                
                /*
                 * Creating and filling of the matrix
                 */
                ne = 0;
                sparsecreate(m, n, m*n, &spa, _state);
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        c = ae_randominteger(2, _state);
                        if( c==0 )
                        {
                            a.ptr.pp_double[i][j] = (double)(0);
                        }
                        if( c==1 )
                        {
                            do
                            {
                                a.ptr.pp_double[i][j] = r*(2*ae_randomreal(_state)-1);
                            }
                            while(ae_fp_eq(a.ptr.pp_double[i][j],(double)(0)));
                            sparseset(&spa, i, j, a.ptr.pp_double[i][j], _state);
                            ne = ne+1;
                        }
                        ta.ptr.pp_bool[i][j] = ae_false;
                    }
                }
                if( hashcrs==1 )
                {
                    sparseconverttocrs(&spa, _state);
                }
                
                /*
                 * Rewrite some elements
                 */
                nr = 0;
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        c = ae_randominteger(2, _state);
                        if( c==1 )
                        {
                            ta.ptr.pp_bool[i][j] = sparserewriteexisting(&spa, i, j, v, _state);
                            if( ta.ptr.pp_bool[i][j] )
                            {
                                a.ptr.pp_double[i][j] = v;
                                nr = nr+1;
                            }
                        }
                    }
                }
                
                /*
                 * Now we have to be sure, that all changes had made correctly
                 */
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( ta.ptr.pp_bool[i][j] )
                        {
                            spaval = sparseget(&spa, i, j, _state);
                            nr = nr-1;
                            if( ae_fp_neq(spaval,v)||ae_fp_neq(spaval,a.ptr.pp_double[i][j]) )
                            {
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                        }
                    }
                }
                if( nr!=0 )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
                
                /*
                 * Rewrite all elements
                 */
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        ta.ptr.pp_bool[i][j] = sparserewriteexisting(&spa, i, j, v, _state);
                        if( ta.ptr.pp_bool[i][j] )
                        {
                            a.ptr.pp_double[i][j] = v;
                        }
                    }
                }
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( ta.ptr.pp_bool[i][j] )
                        {
                            ne = ne-1;
                        }
                    }
                }
                if( ne!=0 )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        spaval = sparseget(&spa, i, j, _state);
                        if( ta.ptr.pp_bool[i][j] )
                        {
                            if( ae_fp_neq(spaval,v)||ae_fp_neq(spaval,a.ptr.pp_double[i][j]) )
                            {
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                        }
                        else
                        {
                            if( ae_fp_neq(spaval,(double)(0))||ae_fp_neq(spaval,a.ptr.pp_double[i][j]) )
                            {
                                result = ae_true;
                                ae_frame_leave(_state);
                                return result;
                            }
                        }
                    }
                }
            }
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Test  for  SparseGetRow/GetCompressedRow  function.   It  creates  random
dense and sparse matrices;  then  get every  row from  sparse matrix  and
compares it with every row in dense matrix.

On failure sets error flag, on success leaves it unchanged.

  -- ALGLIB PROJECT --
     Copyright 23.07.2012 by Bochkanov Sergey
*************************************************************************/
static void testsparseunit_testgetrow(ae_bool* err, ae_state *_state)
{
    ae_frame _frame_block;
    sparsematrix s;
    ae_matrix a;
    ae_int_t m;
    ae_int_t n;
    ae_int_t msize;
    ae_int_t nsize;
    ae_int_t nz;
    ae_vector vals;
    ae_vector mrow;
    ae_vector colidx;
    ae_vector wasreturned;
    ae_int_t mtype;
    ae_int_t i;
    ae_int_t j;

    ae_frame_make(_state, &_frame_block);
    _sparsematrix_init(&s, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&vals, 0, DT_REAL, _state);
    ae_vector_init(&mrow, 0, DT_REAL, _state);
    ae_vector_init(&colidx, 0, DT_INT, _state);
    ae_vector_init(&wasreturned, 0, DT_BOOL, _state);

    msize = 15;
    nsize = 15;
    for(mtype=1; mtype<=2; mtype++)
    {
        for(m=1; m<=msize; m++)
        {
            for(n=1; n<=nsize; n++)
            {
                
                /*
                 * Skip nonrectangular SKS matrices - not supported
                 */
                if( mtype==2&&m!=n )
                {
                    continue;
                }
                
                /*
                 * Create "reference" and sparse matrices
                 */
                ae_matrix_set_length(&a, m, n, _state);
                sparsecreate(m, n, 1, &s, _state);
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( ae_randominteger(5, _state)==3 )
                        {
                            a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                            sparseset(&s, i, j, a.ptr.pp_double[i][j], _state);
                        }
                        else
                        {
                            a.ptr.pp_double[i][j] = (double)(0);
                        }
                    }
                }
                
                /*
                 * Choose matrix type to test
                 */
                if( mtype==1 )
                {
                    sparseconverttocrs(&s, _state);
                }
                else
                {
                    sparseconverttosks(&s, _state);
                }
                
                /*
                 * Test SparseGetRow()
                 */
                for(i=0; i<=m-1; i++)
                {
                    sparsegetrow(&s, i, &mrow, _state);
                    for(j=0; j<=n-1; j++)
                    {
                        if( ae_fp_neq(mrow.ptr.p_double[j],a.ptr.pp_double[i][j])||ae_fp_neq(mrow.ptr.p_double[j],sparseget(&s, i, j, _state)) )
                        {
                            seterrorflag(err, ae_true, _state);
                            ae_frame_leave(_state);
                            return;
                        }
                    }
                }
                
                /*
                 * Test SparseGetCompressedRow()
                 */
                ae_vector_set_length(&wasreturned, n, _state);
                for(i=0; i<=m-1; i++)
                {
                    sparsegetcompressedrow(&s, i, &colidx, &vals, &nz, _state);
                    if( nz<0||nz>n )
                    {
                        seterrorflag(err, ae_true, _state);
                        ae_frame_leave(_state);
                        return;
                    }
                    for(j=0; j<=n-1; j++)
                    {
                        wasreturned.ptr.p_bool[j] = ae_false;
                    }
                    for(j=0; j<=nz-1; j++)
                    {
                        if( colidx.ptr.p_int[j]<0||colidx.ptr.p_int[j]>n )
                        {
                            seterrorflag(err, ae_true, _state);
                            ae_frame_leave(_state);
                            return;
                        }
                        seterrorflag(err, j>0&&colidx.ptr.p_int[j]<=colidx.ptr.p_int[j-1], _state);
                        seterrorflag(err, ae_fp_neq(vals.ptr.p_double[j],a.ptr.pp_double[i][colidx.ptr.p_int[j]])||ae_fp_neq(vals.ptr.p_double[j],sparseget(&s, i, colidx.ptr.p_int[j], _state)), _state);
                        wasreturned.ptr.p_bool[colidx.ptr.p_int[j]] = ae_true;
                    }
                    for(j=0; j<=n-1; j++)
                    {
                        seterrorflag(err, ae_fp_neq(a.ptr.pp_double[i][j],(double)(0))&&!wasreturned.ptr.p_bool[j], _state);
                    }
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Test for SparseConvert functions(isn't tested ConvertToCRS function). The
function  create random  dense and sparse  matrices  in CRS  format. Then
convert  sparse matrix  to some  format  by CONVERT_TO/COPY_TO  functions,
then it does  some modification in matrices and compares that marices are
identical.

NOTE:
    Result of the function assigned to variable CopyErrors in unit test.

  -- ALGLIB PROJECT --
     Copyright 23.07.2012 by Bochkanov Sergey
*************************************************************************/
static ae_bool testsparseunit_testconvertsm(ae_state *_state)
{
    ae_frame _frame_block;
    sparsematrix s;
    sparsematrix cs;
    ae_matrix a;
    ae_int_t m;
    ae_int_t n;
    ae_int_t msize;
    ae_int_t nsize;
    ae_vector ner;
    double tmp;
    ae_int_t i;
    ae_int_t j;
    ae_int_t vartf;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _sparsematrix_init(&s, _state);
    _sparsematrix_init(&cs, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&ner, 0, DT_INT, _state);

    msize = 15;
    nsize = 15;
    for(m=1; m<=msize; m++)
    {
        for(n=1; n<=nsize; n++)
        {
            for(vartf=0; vartf<=2; vartf++)
            {
                ae_matrix_set_length(&a, m, n, _state);
                ae_vector_set_length(&ner, m, _state);
                for(i=0; i<=m-1; i++)
                {
                    ner.ptr.p_int[i] = 0;
                    for(j=0; j<=n-1; j++)
                    {
                        if( ae_randominteger(5, _state)==3 )
                        {
                            ner.ptr.p_int[i] = ner.ptr.p_int[i]+1;
                            a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                        }
                        else
                        {
                            a.ptr.pp_double[i][j] = (double)(0);
                        }
                    }
                }
                
                /*
                 * Create sparse matrix
                 */
                sparsecreatecrs(m, n, &ner, &s, _state);
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( ae_fp_neq(a.ptr.pp_double[i][j],(double)(0)) )
                        {
                            a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                            sparseset(&s, i, j, a.ptr.pp_double[i][j], _state);
                        }
                    }
                }
                
                /*
                 * Set matrix type(we have to be sure that all formats
                 * converted correctly)
                 */
                i = ae_randominteger(2, _state);
                if( i==0 )
                {
                    sparseconverttohash(&s, _state);
                }
                if( i==1 )
                {
                    sparseconverttocrs(&s, _state);
                }
                
                /*
                 * Start test
                 */
                if( vartf==0 )
                {
                    sparseconverttohash(&s, _state);
                    sparsecopy(&s, &cs, _state);
                }
                if( vartf==1 )
                {
                    sparsecopytohash(&s, &cs, _state);
                }
                if( vartf==2 )
                {
                    sparsecopytocrs(&s, &cs, _state);
                }
                
                /*
                 * Change some elements in row
                 */
                if( vartf!=2 )
                {
                    for(i=0; i<=m-1; i++)
                    {
                        tmp = 2*ae_randomreal(_state)-1;
                        j = ae_randominteger(n, _state);
                        a.ptr.pp_double[i][j] = tmp;
                        sparseset(&cs, i, j, tmp, _state);
                        tmp = 2*ae_randomreal(_state)-1;
                        j = ae_randominteger(n, _state);
                        a.ptr.pp_double[i][j] = a.ptr.pp_double[i][j]+tmp;
                        sparseadd(&cs, i, j, tmp, _state);
                    }
                }
                
                /*
                 * Check that A is identical to S
                 */
                for(i=0; i<=m-1; i++)
                {
                    for(j=0; j<=n-1; j++)
                    {
                        if( ae_fp_neq(a.ptr.pp_double[i][j],sparseget(&cs, i, j, _state)) )
                        {
                            result = ae_true;
                            ae_frame_leave(_state);
                            return result;
                        }
                    }
                }
            }
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Test for  check/get  type functions.  The function  create sparse matrix,
converts it to desired type then check this type.

NOTE:
    Result of the function assigned to variable BasicErrors in unit test.

  -- ALGLIB PROJECT --
     Copyright 23.07.2012 by Bochkanov Sergey
*************************************************************************/
static ae_bool testsparseunit_testgcmatrixtype(ae_state *_state)
{
    ae_frame _frame_block;
    sparsematrix s;
    sparsematrix cs;
    ae_int_t m;
    ae_int_t n;
    ae_int_t msize;
    ae_int_t nsize;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _sparsematrix_init(&s, _state);
    _sparsematrix_init(&cs, _state);

    msize = 5;
    nsize = 5;
    for(m=1; m<=msize; m++)
    {
        for(n=1; n<=nsize; n++)
        {
            sparsecreate(m, n, 1, &s, _state);
            sparseconverttocrs(&s, _state);
            if( (sparseishash(&s, _state)||!sparseiscrs(&s, _state))||sparsegetmatrixtype(&s, _state)!=1 )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            sparseconverttohash(&s, _state);
            if( (!sparseishash(&s, _state)||sparseiscrs(&s, _state))||sparsegetmatrixtype(&s, _state)!=0 )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            sparsecopytocrs(&s, &cs, _state);
            if( (sparseishash(&cs, _state)||!sparseiscrs(&cs, _state))||sparsegetmatrixtype(&cs, _state)!=1 )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            sparsecopytohash(&cs, &s, _state);
            if( (!sparseishash(&s, _state)||sparseiscrs(&s, _state))||sparsegetmatrixtype(&s, _state)!=0 )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


void _sparsegenerator_init(void* _p, ae_state *_state)
{
    sparsegenerator *p = (sparsegenerator*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_init(&p->bufa, 0, 0, DT_REAL, _state);
    _hqrndstate_init(&p->rs, _state);
    _rcommstate_init(&p->rcs, _state);
}


void _sparsegenerator_init_copy(void* _dst, void* _src, ae_state *_state)
{
    sparsegenerator *dst = (sparsegenerator*)_dst;
    sparsegenerator *src = (sparsegenerator*)_src;
    dst->n = src->n;
    dst->m = src->m;
    dst->matkind = src->matkind;
    dst->triangle = src->triangle;
    ae_matrix_init_copy(&dst->bufa, &src->bufa, _state);
    _hqrndstate_init_copy(&dst->rs, &src->rs, _state);
    _rcommstate_init_copy(&dst->rcs, &src->rcs, _state);
}


void _sparsegenerator_clear(void* _p)
{
    sparsegenerator *p = (sparsegenerator*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_clear(&p->bufa);
    _hqrndstate_clear(&p->rs);
    _rcommstate_clear(&p->rcs);
}


void _sparsegenerator_destroy(void* _p)
{
    sparsegenerator *p = (sparsegenerator*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_destroy(&p->bufa);
    _hqrndstate_destroy(&p->rs);
    _rcommstate_destroy(&p->rcs);
}


/*$ End $*/
