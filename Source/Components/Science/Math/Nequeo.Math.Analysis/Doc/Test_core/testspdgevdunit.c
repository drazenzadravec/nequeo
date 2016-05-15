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
#include "testspdgevdunit.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Testing bidiagonal SVD decomposition subroutine
*************************************************************************/
ae_bool testspdgevd(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t pass;
    ae_int_t n;
    ae_int_t passcount;
    ae_int_t maxn;
    ae_int_t atask;
    ae_int_t btask;
    ae_vector d;
    ae_vector t1;
    ae_matrix a;
    ae_matrix b;
    ae_matrix afull;
    ae_matrix bfull;
    ae_matrix l;
    ae_matrix z;
    ae_bool isuppera;
    ae_bool isupperb;
    ae_int_t i;
    ae_int_t j;
    ae_int_t minij;
    double v;
    double v1;
    double v2;
    double err;
    double valerr;
    double threshold;
    ae_bool waserrors;
    ae_bool wfailed;
    ae_bool wnsorted;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&t1, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&b, 0, 0, DT_REAL, _state);
    ae_matrix_init(&afull, 0, 0, DT_REAL, _state);
    ae_matrix_init(&bfull, 0, 0, DT_REAL, _state);
    ae_matrix_init(&l, 0, 0, DT_REAL, _state);
    ae_matrix_init(&z, 0, 0, DT_REAL, _state);

    threshold = 10000*ae_machineepsilon;
    valerr = (double)(0);
    wfailed = ae_false;
    wnsorted = ae_false;
    maxn = 20;
    passcount = 5;
    
    /*
     * Main cycle
     */
    for(n=1; n<=maxn; n++)
    {
        for(pass=1; pass<=passcount; pass++)
        {
            for(atask=0; atask<=1; atask++)
            {
                for(btask=0; btask<=1; btask++)
                {
                    isuppera = atask==0;
                    isupperb = btask==0;
                    
                    /*
                     * Initialize A, B, AFull, BFull
                     */
                    ae_vector_set_length(&t1, n-1+1, _state);
                    ae_matrix_set_length(&a, n-1+1, n-1+1, _state);
                    ae_matrix_set_length(&b, n-1+1, n-1+1, _state);
                    ae_matrix_set_length(&afull, n-1+1, n-1+1, _state);
                    ae_matrix_set_length(&bfull, n-1+1, n-1+1, _state);
                    ae_matrix_set_length(&l, n-1+1, n-1+1, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                            a.ptr.pp_double[j][i] = a.ptr.pp_double[i][j];
                            afull.ptr.pp_double[i][j] = a.ptr.pp_double[i][j];
                            afull.ptr.pp_double[j][i] = a.ptr.pp_double[i][j];
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=i+1; j<=n-1; j++)
                        {
                            l.ptr.pp_double[i][j] = ae_randomreal(_state);
                            l.ptr.pp_double[j][i] = l.ptr.pp_double[i][j];
                        }
                        l.ptr.pp_double[i][i] = 1.5+ae_randomreal(_state);
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            minij = ae_minint(i, j, _state);
                            v = ae_v_dotproduct(&l.ptr.pp_double[i][0], 1, &l.ptr.pp_double[0][j], l.stride, ae_v_len(0,minij));
                            b.ptr.pp_double[i][j] = v;
                            b.ptr.pp_double[j][i] = v;
                            bfull.ptr.pp_double[i][j] = v;
                            bfull.ptr.pp_double[j][i] = v;
                        }
                    }
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=n-1; j++)
                        {
                            if( isuppera )
                            {
                                if( j<i )
                                {
                                    a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                                }
                            }
                            else
                            {
                                if( i<j )
                                {
                                    a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                                }
                            }
                            if( isupperb )
                            {
                                if( j<i )
                                {
                                    b.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                                }
                            }
                            else
                            {
                                if( i<j )
                                {
                                    b.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                                }
                            }
                        }
                    }
                    
                    /*
                     * Problem 1
                     */
                    if( !smatrixgevd(&a, n, isuppera, &b, isupperb, 1, 1, &d, &z, _state) )
                    {
                        wfailed = ae_true;
                        continue;
                    }
                    err = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            v1 = ae_v_dotproduct(&afull.ptr.pp_double[i][0], 1, &z.ptr.pp_double[0][j], z.stride, ae_v_len(0,n-1));
                            v2 = ae_v_dotproduct(&bfull.ptr.pp_double[i][0], 1, &z.ptr.pp_double[0][j], z.stride, ae_v_len(0,n-1));
                            err = ae_maxreal(err, ae_fabs(v1-d.ptr.p_double[j]*v2, _state), _state);
                        }
                    }
                    valerr = ae_maxreal(err, valerr, _state);
                    
                    /*
                     * Problem 2
                     */
                    if( !smatrixgevd(&a, n, isuppera, &b, isupperb, 1, 2, &d, &z, _state) )
                    {
                        wfailed = ae_true;
                        continue;
                    }
                    err = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            v1 = ae_v_dotproduct(&bfull.ptr.pp_double[i][0], 1, &z.ptr.pp_double[0][j], z.stride, ae_v_len(0,n-1));
                            t1.ptr.p_double[i] = v1;
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            v2 = ae_v_dotproduct(&afull.ptr.pp_double[i][0], 1, &t1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                            err = ae_maxreal(err, ae_fabs(v2-d.ptr.p_double[j]*z.ptr.pp_double[i][j], _state), _state);
                        }
                    }
                    valerr = ae_maxreal(err, valerr, _state);
                    
                    /*
                     * Test problem 3
                     */
                    if( !smatrixgevd(&a, n, isuppera, &b, isupperb, 1, 3, &d, &z, _state) )
                    {
                        wfailed = ae_true;
                        continue;
                    }
                    err = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            v1 = ae_v_dotproduct(&afull.ptr.pp_double[i][0], 1, &z.ptr.pp_double[0][j], z.stride, ae_v_len(0,n-1));
                            t1.ptr.p_double[i] = v1;
                        }
                        for(i=0; i<=n-1; i++)
                        {
                            v2 = ae_v_dotproduct(&bfull.ptr.pp_double[i][0], 1, &t1.ptr.p_double[0], 1, ae_v_len(0,n-1));
                            err = ae_maxreal(err, ae_fabs(v2-d.ptr.p_double[j]*z.ptr.pp_double[i][j], _state), _state);
                        }
                    }
                    valerr = ae_maxreal(err, valerr, _state);
                }
            }
        }
    }
    
    /*
     * report
     */
    waserrors = (ae_fp_greater(valerr,threshold)||wfailed)||wnsorted;
    if( !silent )
    {
        printf("TESTING SYMMETRIC GEVD\n");
        printf("Av-lambdav error (generalized):          %5.3e\n",
            (double)(valerr));
        printf("Eigen values order:                      ");
        if( !wnsorted )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("Always converged:                        ");
        if( !wfailed )
        {
            printf("YES\n");
        }
        else
        {
            printf("NO\n");
        }
        printf("Threshold:                               %5.3e\n",
            (double)(threshold));
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
ae_bool _pexec_testspdgevd(ae_bool silent, ae_state *_state)
{
    return testspdgevd(silent, _state);
}


/*$ End $*/
