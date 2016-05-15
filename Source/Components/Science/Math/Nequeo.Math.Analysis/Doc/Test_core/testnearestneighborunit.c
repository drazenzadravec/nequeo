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
#include "testnearestneighborunit.h"


/*$ Declarations $*/
static ae_bool testnearestneighborunit_kdtresultsdifferent(/* Real    */ ae_matrix* refxy,
     ae_int_t ntotal,
     /* Real    */ ae_matrix* qx,
     /* Real    */ ae_matrix* qxy,
     /* Integer */ ae_vector* qt,
     ae_int_t n,
     ae_int_t nx,
     ae_int_t ny,
     ae_state *_state);
static double testnearestneighborunit_vnorm(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_int_t normtype,
     ae_state *_state);
static void testnearestneighborunit_testkdtuniform(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t nx,
     ae_int_t ny,
     ae_int_t normtype,
     ae_bool* kdterrors,
     ae_state *_state);
static void testnearestneighborunit_testkdtreeserialization(ae_bool* err,
     ae_state *_state);
static ae_bool testnearestneighborunit_testspecialcases(ae_state *_state);


/*$ Body $*/


/*************************************************************************
Testing Nearest Neighbor Search
*************************************************************************/
ae_bool testnearestneighbor(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix xy;
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_int_t normtype;
    ae_int_t nx;
    ae_int_t ny;
    ae_int_t n;
    ae_int_t smalln;
    ae_int_t largen;
    ae_int_t passcount;
    ae_int_t pass;
    ae_bool waserrors;
    ae_bool kdterrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);

    kdterrors = ae_false;
    passcount = 2;
    smalln = 256;
    largen = 2048;
    ny = 3;
    
    /*
     *
     */
    testnearestneighborunit_testkdtreeserialization(&kdterrors, _state);
    for(pass=1; pass<=passcount; pass++)
    {
        for(normtype=0; normtype<=2; normtype++)
        {
            for(nx=1; nx<=3; nx++)
            {
                
                /*
                 * Test in hypercube
                 */
                ae_matrix_set_length(&xy, largen, nx+ny, _state);
                for(i=0; i<=largen-1; i++)
                {
                    for(j=0; j<=nx+ny-1; j++)
                    {
                        xy.ptr.pp_double[i][j] = 10*ae_randomreal(_state)-5;
                    }
                }
                for(n=1; n<=10; n++)
                {
                    testnearestneighborunit_testkdtuniform(&xy, n, nx, ae_randominteger(ny+1, _state), normtype, &kdterrors, _state);
                }
                testnearestneighborunit_testkdtuniform(&xy, largen, nx, ae_randominteger(ny+1, _state), normtype, &kdterrors, _state);
                
                /*
                 * Test clustered (2*N points, pairs of equal points)
                 */
                ae_matrix_set_length(&xy, 2*smalln, nx+ny, _state);
                for(i=0; i<=smalln-1; i++)
                {
                    for(j=0; j<=nx+ny-1; j++)
                    {
                        xy.ptr.pp_double[2*i+0][j] = 10*ae_randomreal(_state)-5;
                        xy.ptr.pp_double[2*i+1][j] = xy.ptr.pp_double[2*i+0][j];
                    }
                }
                testnearestneighborunit_testkdtuniform(&xy, 2*smalln, nx, ae_randominteger(ny+1, _state), normtype, &kdterrors, _state);
                
                /*
                 * Test degenerate case: all points are same except for one
                 */
                ae_matrix_set_length(&xy, smalln, nx+ny, _state);
                v = ae_randomreal(_state);
                for(i=0; i<=smalln-2; i++)
                {
                    for(j=0; j<=nx+ny-1; j++)
                    {
                        xy.ptr.pp_double[i][j] = v;
                    }
                }
                for(j=0; j<=nx+ny-1; j++)
                {
                    xy.ptr.pp_double[smalln-1][j] = 10*ae_randomreal(_state)-5;
                }
                testnearestneighborunit_testkdtuniform(&xy, smalln, nx, ae_randominteger(ny+1, _state), normtype, &kdterrors, _state);
            }
        }
    }
    kdterrors = kdterrors||testnearestneighborunit_testspecialcases(_state);
    
    /*
     * report
     */
    waserrors = kdterrors;
    if( !silent )
    {
        printf("TESTING NEAREST NEIGHBOR SEARCH\n");
        printf("* KD TREES:                              ");
        if( !kdterrors )
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
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testnearestneighbor(ae_bool silent, ae_state *_state)
{
    return testnearestneighbor(silent, _state);
}


/*************************************************************************
Compare results from different queries:
* X     just X-values
* XY    X-values and Y-values
* XT    X-values and tag values
*************************************************************************/
static ae_bool testnearestneighborunit_kdtresultsdifferent(/* Real    */ ae_matrix* refxy,
     ae_int_t ntotal,
     /* Real    */ ae_matrix* qx,
     /* Real    */ ae_matrix* qxy,
     /* Integer */ ae_vector* qt,
     ae_int_t n,
     ae_int_t nx,
     ae_int_t ny,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_bool result;


    result = ae_false;
    for(i=0; i<=n-1; i++)
    {
        if( qt->ptr.p_int[i]<0||qt->ptr.p_int[i]>=ntotal )
        {
            result = ae_true;
            return result;
        }
        for(j=0; j<=nx-1; j++)
        {
            result = result||ae_fp_neq(qx->ptr.pp_double[i][j],refxy->ptr.pp_double[qt->ptr.p_int[i]][j]);
            result = result||ae_fp_neq(qxy->ptr.pp_double[i][j],refxy->ptr.pp_double[qt->ptr.p_int[i]][j]);
        }
        for(j=0; j<=ny-1; j++)
        {
            result = result||ae_fp_neq(qxy->ptr.pp_double[i][nx+j],refxy->ptr.pp_double[qt->ptr.p_int[i]][nx+j]);
        }
    }
    return result;
}


/*************************************************************************
Returns norm
*************************************************************************/
static double testnearestneighborunit_vnorm(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_int_t normtype,
     ae_state *_state)
{
    ae_int_t i;
    double result;


    result = ae_randomreal(_state);
    if( normtype==0 )
    {
        result = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            result = ae_maxreal(result, ae_fabs(x->ptr.p_double[i], _state), _state);
        }
        return result;
    }
    if( normtype==1 )
    {
        result = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            result = result+ae_fabs(x->ptr.p_double[i], _state);
        }
        return result;
    }
    if( normtype==2 )
    {
        result = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            result = result+ae_sqr(x->ptr.p_double[i], _state);
        }
        result = ae_sqrt(result, _state);
        return result;
    }
    return result;
}


/*************************************************************************
Testing Nearest Neighbor Search on uniformly distributed hypercube

NormType: 0, 1, 2
D: space dimension
N: points count
*************************************************************************/
static void testnearestneighborunit_testkdtuniform(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t nx,
     ae_int_t ny,
     ae_int_t normtype,
     ae_bool* kdterrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    double errtol;
    ae_vector tags;
    ae_vector ptx;
    ae_vector tmpx;
    ae_vector tmpb;
    kdtree treex;
    kdtree treexy;
    kdtree treext;
    ae_matrix qx;
    ae_matrix qxy;
    ae_vector qtags;
    ae_vector qr;
    ae_int_t kx;
    ae_int_t kxy;
    ae_int_t kt;
    double eps;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t task;
    ae_bool isequal;
    double r;
    ae_int_t q;
    ae_int_t qcount;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&tags, 0, DT_INT, _state);
    ae_vector_init(&ptx, 0, DT_REAL, _state);
    ae_vector_init(&tmpx, 0, DT_REAL, _state);
    ae_vector_init(&tmpb, 0, DT_BOOL, _state);
    _kdtree_init(&treex, _state);
    _kdtree_init(&treexy, _state);
    _kdtree_init(&treext, _state);
    ae_matrix_init(&qx, 0, 0, DT_REAL, _state);
    ae_matrix_init(&qxy, 0, 0, DT_REAL, _state);
    ae_vector_init(&qtags, 0, DT_INT, _state);
    ae_vector_init(&qr, 0, DT_REAL, _state);

    qcount = 10;
    
    /*
     * Tol - roundoff error tolerance (for '>=' comparisons)
     */
    errtol = 100000*ae_machineepsilon;
    
    /*
     * fill tags
     */
    ae_vector_set_length(&tags, n, _state);
    for(i=0; i<=n-1; i++)
    {
        tags.ptr.p_int[i] = i;
    }
    
    /*
     * build trees
     */
    kdtreebuild(xy, n, nx, 0, normtype, &treex, _state);
    kdtreebuild(xy, n, nx, ny, normtype, &treexy, _state);
    kdtreebuildtagged(xy, &tags, n, nx, 0, normtype, &treext, _state);
    
    /*
     * allocate arrays
     */
    ae_vector_set_length(&tmpx, nx, _state);
    ae_vector_set_length(&tmpb, n, _state);
    ae_matrix_set_length(&qx, n, nx, _state);
    ae_matrix_set_length(&qxy, n, nx+ny, _state);
    ae_vector_set_length(&qtags, n, _state);
    ae_vector_set_length(&qr, n, _state);
    ae_vector_set_length(&ptx, nx, _state);
    
    /*
     * test general K-NN queries (with self-matches):
     * * compare results from different trees (must be equal) and
     *   check that correct (value,tag) pairs are returned
     * * test results from XT tree - let R be radius of query result.
     *   then all points not in result must be not closer than R.
     */
    for(q=1; q<=qcount; q++)
    {
        
        /*
         * Select K: 1..N
         */
        if( ae_fp_greater(ae_randomreal(_state),0.5) )
        {
            k = 1+ae_randominteger(n, _state);
        }
        else
        {
            k = 1;
        }
        
        /*
         * Select point (either one of the points, or random)
         */
        if( ae_fp_greater(ae_randomreal(_state),0.5) )
        {
            i = ae_randominteger(n, _state);
            ae_v_move(&ptx.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nx-1));
        }
        else
        {
            for(i=0; i<=nx-1; i++)
            {
                ptx.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
        }
        
        /*
         * Test:
         * * consistency of results from different queries
         * * points in query are IN the R-sphere (or at the boundary),
         *   and points not in query are outside of the R-sphere (or at the boundary)
         * * distances are correct and are ordered
         */
        kx = kdtreequeryknn(&treex, &ptx, k, ae_true, _state);
        kxy = kdtreequeryknn(&treexy, &ptx, k, ae_true, _state);
        kt = kdtreequeryknn(&treext, &ptx, k, ae_true, _state);
        if( (kx!=k||kxy!=k)||kt!=k )
        {
            *kdterrors = ae_true;
            ae_frame_leave(_state);
            return;
        }
        kdtreequeryresultsxi(&treex, &qx, _state);
        kdtreequeryresultsxyi(&treexy, &qxy, _state);
        kdtreequeryresultstagsi(&treext, &qtags, _state);
        kdtreequeryresultsdistancesi(&treext, &qr, _state);
        *kdterrors = *kdterrors||testnearestneighborunit_kdtresultsdifferent(xy, n, &qx, &qxy, &qtags, k, nx, ny, _state);
        kdtreequeryresultsx(&treex, &qx, _state);
        kdtreequeryresultsxy(&treexy, &qxy, _state);
        kdtreequeryresultstags(&treext, &qtags, _state);
        kdtreequeryresultsdistances(&treext, &qr, _state);
        *kdterrors = *kdterrors||testnearestneighborunit_kdtresultsdifferent(xy, n, &qx, &qxy, &qtags, k, nx, ny, _state);
        for(i=0; i<=n-1; i++)
        {
            tmpb.ptr.p_bool[i] = ae_true;
        }
        r = (double)(0);
        for(i=0; i<=k-1; i++)
        {
            tmpb.ptr.p_bool[qtags.ptr.p_int[i]] = ae_false;
            ae_v_move(&tmpx.ptr.p_double[0], 1, &ptx.ptr.p_double[0], 1, ae_v_len(0,nx-1));
            ae_v_sub(&tmpx.ptr.p_double[0], 1, &qx.ptr.pp_double[i][0], 1, ae_v_len(0,nx-1));
            r = ae_maxreal(r, testnearestneighborunit_vnorm(&tmpx, nx, normtype, _state), _state);
        }
        for(i=0; i<=n-1; i++)
        {
            if( tmpb.ptr.p_bool[i] )
            {
                ae_v_move(&tmpx.ptr.p_double[0], 1, &ptx.ptr.p_double[0], 1, ae_v_len(0,nx-1));
                ae_v_sub(&tmpx.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nx-1));
                *kdterrors = *kdterrors||ae_fp_less(testnearestneighborunit_vnorm(&tmpx, nx, normtype, _state),r*(1-errtol));
            }
        }
        for(i=0; i<=k-2; i++)
        {
            *kdterrors = *kdterrors||ae_fp_greater(qr.ptr.p_double[i],qr.ptr.p_double[i+1]);
        }
        for(i=0; i<=k-1; i++)
        {
            ae_v_move(&tmpx.ptr.p_double[0], 1, &ptx.ptr.p_double[0], 1, ae_v_len(0,nx-1));
            ae_v_sub(&tmpx.ptr.p_double[0], 1, &xy->ptr.pp_double[qtags.ptr.p_int[i]][0], 1, ae_v_len(0,nx-1));
            *kdterrors = *kdterrors||ae_fp_greater(ae_fabs(testnearestneighborunit_vnorm(&tmpx, nx, normtype, _state)-qr.ptr.p_double[i], _state),errtol);
        }
        
        /*
         * Test reallocation properties: buffered functions must automatically
         * resize array which is too small, but leave unchanged array which is
         * too large.
         */
        if( n>=2 )
        {
            
            /*
             * First step: array is too small, two elements are required
             */
            k = 2;
            kx = kdtreequeryknn(&treex, &ptx, k, ae_true, _state);
            kxy = kdtreequeryknn(&treexy, &ptx, k, ae_true, _state);
            kt = kdtreequeryknn(&treext, &ptx, k, ae_true, _state);
            if( (kx!=k||kxy!=k)||kt!=k )
            {
                *kdterrors = ae_true;
                ae_frame_leave(_state);
                return;
            }
            ae_matrix_set_length(&qx, 1, 1, _state);
            ae_matrix_set_length(&qxy, 1, 1, _state);
            ae_vector_set_length(&qtags, 1, _state);
            ae_vector_set_length(&qr, 1, _state);
            kdtreequeryresultsx(&treex, &qx, _state);
            kdtreequeryresultsxy(&treexy, &qxy, _state);
            kdtreequeryresultstags(&treext, &qtags, _state);
            kdtreequeryresultsdistances(&treext, &qr, _state);
            *kdterrors = *kdterrors||testnearestneighborunit_kdtresultsdifferent(xy, n, &qx, &qxy, &qtags, k, nx, ny, _state);
            
            /*
             * Second step: array is one row larger than needed, so only first
             * row is overwritten. Test it.
             */
            k = 1;
            kx = kdtreequeryknn(&treex, &ptx, k, ae_true, _state);
            kxy = kdtreequeryknn(&treexy, &ptx, k, ae_true, _state);
            kt = kdtreequeryknn(&treext, &ptx, k, ae_true, _state);
            if( (kx!=k||kxy!=k)||kt!=k )
            {
                *kdterrors = ae_true;
                ae_frame_leave(_state);
                return;
            }
            for(i=0; i<=nx-1; i++)
            {
                qx.ptr.pp_double[1][i] = _state->v_nan;
            }
            for(i=0; i<=nx+ny-1; i++)
            {
                qxy.ptr.pp_double[1][i] = _state->v_nan;
            }
            qtags.ptr.p_int[1] = 999;
            qr.ptr.p_double[1] = _state->v_nan;
            kdtreequeryresultsx(&treex, &qx, _state);
            kdtreequeryresultsxy(&treexy, &qxy, _state);
            kdtreequeryresultstags(&treext, &qtags, _state);
            kdtreequeryresultsdistances(&treext, &qr, _state);
            *kdterrors = *kdterrors||testnearestneighborunit_kdtresultsdifferent(xy, n, &qx, &qxy, &qtags, k, nx, ny, _state);
            for(i=0; i<=nx-1; i++)
            {
                *kdterrors = *kdterrors||!ae_isnan(qx.ptr.pp_double[1][i], _state);
            }
            for(i=0; i<=nx+ny-1; i++)
            {
                *kdterrors = *kdterrors||!ae_isnan(qxy.ptr.pp_double[1][i], _state);
            }
            *kdterrors = *kdterrors||!(qtags.ptr.p_int[1]==999);
            *kdterrors = *kdterrors||!ae_isnan(qr.ptr.p_double[1], _state);
        }
        
        /*
         * Test reallocation properties: 'interactive' functions must allocate
         * new array on each call.
         */
        if( n>=2 )
        {
            
            /*
             * On input array is either too small or too large
             */
            for(k=1; k<=2; k++)
            {
                ae_assert(k==1||k==2, "KNN: internal error (unexpected K)!", _state);
                kx = kdtreequeryknn(&treex, &ptx, k, ae_true, _state);
                kxy = kdtreequeryknn(&treexy, &ptx, k, ae_true, _state);
                kt = kdtreequeryknn(&treext, &ptx, k, ae_true, _state);
                if( (kx!=k||kxy!=k)||kt!=k )
                {
                    *kdterrors = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
                ae_matrix_set_length(&qx, 3-k, 3-k, _state);
                ae_matrix_set_length(&qxy, 3-k, 3-k, _state);
                ae_vector_set_length(&qtags, 3-k, _state);
                ae_vector_set_length(&qr, 3-k, _state);
                kdtreequeryresultsxi(&treex, &qx, _state);
                kdtreequeryresultsxyi(&treexy, &qxy, _state);
                kdtreequeryresultstagsi(&treext, &qtags, _state);
                kdtreequeryresultsdistancesi(&treext, &qr, _state);
                *kdterrors = *kdterrors||testnearestneighborunit_kdtresultsdifferent(xy, n, &qx, &qxy, &qtags, k, nx, ny, _state);
                *kdterrors = (*kdterrors||qx.rows!=k)||qx.cols!=nx;
                *kdterrors = (*kdterrors||qxy.rows!=k)||qxy.cols!=nx+ny;
                *kdterrors = *kdterrors||qtags.cnt!=k;
                *kdterrors = *kdterrors||qr.cnt!=k;
            }
        }
    }
    
    /*
     * test general approximate K-NN queries (with self-matches):
     * * compare results from different trees (must be equal) and
     *   check that correct (value,tag) pairs are returned
     * * test results from XT tree - let R be radius of query result.
     *   then all points not in result must be not closer than R/(1+Eps).
     */
    for(q=1; q<=qcount; q++)
    {
        
        /*
         * Select K: 1..N
         */
        if( ae_fp_greater(ae_randomreal(_state),0.5) )
        {
            k = 1+ae_randominteger(n, _state);
        }
        else
        {
            k = 1;
        }
        
        /*
         * Select Eps
         */
        eps = 0.5+ae_randomreal(_state);
        
        /*
         * Select point (either one of the points, or random)
         */
        if( ae_fp_greater(ae_randomreal(_state),0.5) )
        {
            i = ae_randominteger(n, _state);
            ae_v_move(&ptx.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nx-1));
        }
        else
        {
            for(i=0; i<=nx-1; i++)
            {
                ptx.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
        }
        
        /*
         * Test:
         * * consistency of results from different queries
         * * points in query are IN the R-sphere (or at the boundary),
         *   and points not in query are outside of the R-sphere (or at the boundary)
         * * distances are correct and are ordered
         */
        kx = kdtreequeryaknn(&treex, &ptx, k, ae_true, eps, _state);
        kxy = kdtreequeryaknn(&treexy, &ptx, k, ae_true, eps, _state);
        kt = kdtreequeryaknn(&treext, &ptx, k, ae_true, eps, _state);
        if( (kx!=k||kxy!=k)||kt!=k )
        {
            *kdterrors = ae_true;
            ae_frame_leave(_state);
            return;
        }
        kdtreequeryresultsxi(&treex, &qx, _state);
        kdtreequeryresultsxyi(&treexy, &qxy, _state);
        kdtreequeryresultstagsi(&treext, &qtags, _state);
        kdtreequeryresultsdistancesi(&treext, &qr, _state);
        *kdterrors = *kdterrors||testnearestneighborunit_kdtresultsdifferent(xy, n, &qx, &qxy, &qtags, k, nx, ny, _state);
        kdtreequeryresultsx(&treex, &qx, _state);
        kdtreequeryresultsxy(&treexy, &qxy, _state);
        kdtreequeryresultstags(&treext, &qtags, _state);
        kdtreequeryresultsdistances(&treext, &qr, _state);
        *kdterrors = *kdterrors||testnearestneighborunit_kdtresultsdifferent(xy, n, &qx, &qxy, &qtags, k, nx, ny, _state);
        for(i=0; i<=n-1; i++)
        {
            tmpb.ptr.p_bool[i] = ae_true;
        }
        r = (double)(0);
        for(i=0; i<=k-1; i++)
        {
            tmpb.ptr.p_bool[qtags.ptr.p_int[i]] = ae_false;
            ae_v_move(&tmpx.ptr.p_double[0], 1, &ptx.ptr.p_double[0], 1, ae_v_len(0,nx-1));
            ae_v_sub(&tmpx.ptr.p_double[0], 1, &qx.ptr.pp_double[i][0], 1, ae_v_len(0,nx-1));
            r = ae_maxreal(r, testnearestneighborunit_vnorm(&tmpx, nx, normtype, _state), _state);
        }
        for(i=0; i<=n-1; i++)
        {
            if( tmpb.ptr.p_bool[i] )
            {
                ae_v_move(&tmpx.ptr.p_double[0], 1, &ptx.ptr.p_double[0], 1, ae_v_len(0,nx-1));
                ae_v_sub(&tmpx.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nx-1));
                *kdterrors = *kdterrors||ae_fp_less(testnearestneighborunit_vnorm(&tmpx, nx, normtype, _state),r*(1-errtol)/(1+eps));
            }
        }
        for(i=0; i<=k-2; i++)
        {
            *kdterrors = *kdterrors||ae_fp_greater(qr.ptr.p_double[i],qr.ptr.p_double[i+1]);
        }
        for(i=0; i<=k-1; i++)
        {
            ae_v_move(&tmpx.ptr.p_double[0], 1, &ptx.ptr.p_double[0], 1, ae_v_len(0,nx-1));
            ae_v_sub(&tmpx.ptr.p_double[0], 1, &xy->ptr.pp_double[qtags.ptr.p_int[i]][0], 1, ae_v_len(0,nx-1));
            *kdterrors = *kdterrors||ae_fp_greater(ae_fabs(testnearestneighborunit_vnorm(&tmpx, nx, normtype, _state)-qr.ptr.p_double[i], _state),errtol);
        }
    }
    
    /*
     * test general R-NN queries  (with self-matches):
     * * compare results from different trees (must be equal) and
     *   check that correct (value,tag) pairs are returned
     * * test results from XT tree - let R be radius of query result.
     *   then all points not in result must be not closer than R.
     */
    for(q=1; q<=qcount; q++)
    {
        
        /*
         * Select R
         */
        if( ae_fp_greater(ae_randomreal(_state),0.3) )
        {
            r = ae_maxreal(ae_randomreal(_state), ae_machineepsilon, _state);
        }
        else
        {
            r = ae_machineepsilon;
        }
        
        /*
         * Select point (either one of the points, or random)
         */
        if( ae_fp_greater(ae_randomreal(_state),0.5) )
        {
            i = ae_randominteger(n, _state);
            ae_v_move(&ptx.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nx-1));
        }
        else
        {
            for(i=0; i<=nx-1; i++)
            {
                ptx.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
        }
        
        /*
         * Test:
         * * consistency of results from different queries
         * * points in query are IN the R-sphere (or at the boundary),
         *   and points not in query are outside of the R-sphere (or at the boundary)
         * * distances are correct and are ordered
         */
        kx = kdtreequeryrnn(&treex, &ptx, r, ae_true, _state);
        kxy = kdtreequeryrnn(&treexy, &ptx, r, ae_true, _state);
        kt = kdtreequeryrnn(&treext, &ptx, r, ae_true, _state);
        if( kxy!=kx||kt!=kx )
        {
            *kdterrors = ae_true;
            ae_frame_leave(_state);
            return;
        }
        kdtreequeryresultsxi(&treex, &qx, _state);
        kdtreequeryresultsxyi(&treexy, &qxy, _state);
        kdtreequeryresultstagsi(&treext, &qtags, _state);
        kdtreequeryresultsdistancesi(&treext, &qr, _state);
        *kdterrors = *kdterrors||testnearestneighborunit_kdtresultsdifferent(xy, n, &qx, &qxy, &qtags, kx, nx, ny, _state);
        kdtreequeryresultsx(&treex, &qx, _state);
        kdtreequeryresultsxy(&treexy, &qxy, _state);
        kdtreequeryresultstags(&treext, &qtags, _state);
        kdtreequeryresultsdistances(&treext, &qr, _state);
        *kdterrors = *kdterrors||testnearestneighborunit_kdtresultsdifferent(xy, n, &qx, &qxy, &qtags, kx, nx, ny, _state);
        for(i=0; i<=n-1; i++)
        {
            tmpb.ptr.p_bool[i] = ae_true;
        }
        for(i=0; i<=kx-1; i++)
        {
            tmpb.ptr.p_bool[qtags.ptr.p_int[i]] = ae_false;
        }
        for(i=0; i<=n-1; i++)
        {
            ae_v_move(&tmpx.ptr.p_double[0], 1, &ptx.ptr.p_double[0], 1, ae_v_len(0,nx-1));
            ae_v_sub(&tmpx.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nx-1));
            if( tmpb.ptr.p_bool[i] )
            {
                *kdterrors = *kdterrors||ae_fp_less(testnearestneighborunit_vnorm(&tmpx, nx, normtype, _state),r*(1-errtol));
            }
            else
            {
                *kdterrors = *kdterrors||ae_fp_greater(testnearestneighborunit_vnorm(&tmpx, nx, normtype, _state),r*(1+errtol));
            }
        }
        for(i=0; i<=kx-2; i++)
        {
            *kdterrors = *kdterrors||ae_fp_greater(qr.ptr.p_double[i],qr.ptr.p_double[i+1]);
        }
    }
    
    /*
     * Test self-matching:
     * * self-match - nearest neighbor of each point in XY is the point itself
     * * no self-match - nearest neighbor is NOT the point itself
     */
    if( n>1 )
    {
        
        /*
         * test for N=1 have non-general form, but it is not really needed
         */
        for(task=0; task<=1; task++)
        {
            for(i=0; i<=n-1; i++)
            {
                ae_v_move(&ptx.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nx-1));
                kx = kdtreequeryknn(&treex, &ptx, 1, task==0, _state);
                kdtreequeryresultsxi(&treex, &qx, _state);
                if( kx!=1 )
                {
                    *kdterrors = ae_true;
                    ae_frame_leave(_state);
                    return;
                }
                isequal = ae_true;
                for(j=0; j<=nx-1; j++)
                {
                    isequal = isequal&&ae_fp_eq(qx.ptr.pp_double[0][j],ptx.ptr.p_double[j]);
                }
                if( task==0 )
                {
                    *kdterrors = *kdterrors||!isequal;
                }
                else
                {
                    *kdterrors = *kdterrors||isequal;
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Testing serialization of KD trees

This function sets Err to True on errors, but leaves it unchanged on success
*************************************************************************/
static void testnearestneighborunit_testkdtreeserialization(ae_bool* err,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t nx;
    ae_int_t ny;
    ae_int_t normtype;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t q;
    ae_matrix xy;
    ae_vector x;
    ae_vector tags;
    ae_vector qsizes;
    double threshold;
    kdtree tree0;
    kdtree tree1;
    ae_int_t k0;
    ae_int_t k1;
    ae_matrix xy0;
    ae_matrix xy1;
    ae_vector tags0;
    ae_vector tags1;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&tags, 0, DT_INT, _state);
    ae_vector_init(&qsizes, 0, DT_INT, _state);
    _kdtree_init(&tree0, _state);
    _kdtree_init(&tree1, _state);
    ae_matrix_init(&xy0, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xy1, 0, 0, DT_REAL, _state);
    ae_vector_init(&tags0, 0, DT_INT, _state);
    ae_vector_init(&tags1, 0, DT_INT, _state);

    threshold = 100*ae_machineepsilon;
    
    /*
     * different N, NX, NY, NormType
     */
    n = 1;
    while(n<=51)
    {
        
        /*
         * prepare array with query sizes
         */
        ae_vector_set_length(&qsizes, 4, _state);
        qsizes.ptr.p_int[0] = 1;
        qsizes.ptr.p_int[1] = ae_minint(2, n, _state);
        qsizes.ptr.p_int[2] = ae_minint(4, n, _state);
        qsizes.ptr.p_int[3] = n;
        
        /*
         * different NX/NY/NormType
         */
        for(nx=1; nx<=2; nx++)
        {
            for(ny=0; ny<=2; ny++)
            {
                for(normtype=0; normtype<=2; normtype++)
                {
                    
                    /*
                     * Prepare data
                     */
                    ae_matrix_set_length(&xy, n, nx+ny, _state);
                    ae_vector_set_length(&tags, n, _state);
                    for(i=0; i<=n-1; i++)
                    {
                        for(j=0; j<=nx+ny-1; j++)
                        {
                            xy.ptr.pp_double[i][j] = ae_randomreal(_state);
                        }
                        tags.ptr.p_int[i] = ae_randominteger(100, _state);
                    }
                    
                    /*
                     * Build tree, pass it through serializer
                     */
                    kdtreebuildtagged(&xy, &tags, n, nx, ny, normtype, &tree0, _state);
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
                        kdtreealloc(&_local_serializer, &tree0, _state);
                        _local_ssize = ae_serializer_get_alloc_size(&_local_serializer);
                        ae_db_malloc(&_local_dynamic_block, _local_ssize+1, _state, ae_true);
                        ae_serializer_sstart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                        kdtreeserialize(&_local_serializer, &tree0, _state);
                        ae_serializer_stop(&_local_serializer);
                        ae_serializer_clear(&_local_serializer);
                        
                        ae_serializer_init(&_local_serializer);
                        ae_serializer_ustart_str(&_local_serializer, (char*)_local_dynamic_block.ptr);
                        kdtreeunserialize(&_local_serializer, &tree1, _state);
                        ae_serializer_stop(&_local_serializer);
                        ae_serializer_clear(&_local_serializer);
                        
                        ae_frame_leave(_state);
                    }
                    
                    /*
                     * For each point of XY we make queries with different sizes
                     */
                    ae_vector_set_length(&x, nx, _state);
                    for(k=0; k<=n-1; k++)
                    {
                        for(q=0; q<=qsizes.cnt-1; q++)
                        {
                            ae_v_move(&x.ptr.p_double[0], 1, &xy.ptr.pp_double[k][0], 1, ae_v_len(0,nx-1));
                            k0 = kdtreequeryknn(&tree0, &x, qsizes.ptr.p_int[q], ae_true, _state);
                            k1 = kdtreequeryknn(&tree1, &x, qsizes.ptr.p_int[q], ae_true, _state);
                            if( k0!=k1 )
                            {
                                *err = ae_true;
                                ae_frame_leave(_state);
                                return;
                            }
                            kdtreequeryresultsxy(&tree0, &xy0, _state);
                            kdtreequeryresultsxy(&tree1, &xy1, _state);
                            for(i=0; i<=k0-1; i++)
                            {
                                for(j=0; j<=nx+ny-1; j++)
                                {
                                    if( ae_fp_greater(ae_fabs(xy0.ptr.pp_double[i][j]-xy1.ptr.pp_double[i][j], _state),threshold) )
                                    {
                                        *err = ae_true;
                                        ae_frame_leave(_state);
                                        return;
                                    }
                                }
                            }
                            kdtreequeryresultstags(&tree0, &tags0, _state);
                            kdtreequeryresultstags(&tree1, &tags1, _state);
                            for(i=0; i<=k0-1; i++)
                            {
                                if( tags0.ptr.p_int[i]!=tags1.ptr.p_int[i] )
                                {
                                    *err = ae_true;
                                    ae_frame_leave(_state);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
        
        /*
         * Next N
         */
        n = n+25;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function tests different special cases:
* Kd-tree for a zero number of points
* Kd-tree for array with a lot of duplicates (early versions of ALGLIB
  raised stack overflow on such datasets)

It returns True on errors, False on success.
*************************************************************************/
static ae_bool testnearestneighborunit_testspecialcases(ae_state *_state)
{
    ae_frame _frame_block;
    kdtree kdt;
    ae_matrix xy;
    ae_vector tags;
    ae_vector x;
    ae_int_t n;
    ae_int_t nk;
    ae_int_t nx;
    ae_int_t ny;
    ae_int_t normtype;
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _kdtree_init(&kdt, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_vector_init(&tags, 0, DT_INT, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);

    result = ae_false;
    for(nx=1; nx<=3; nx++)
    {
        for(ny=0; ny<=3; ny++)
        {
            for(normtype=0; normtype<=2; normtype++)
            {
                
                /*
                 * Build tree
                 */
                if( ae_fp_greater(ae_randomreal(_state),0.5) )
                {
                    kdtreebuildtagged(&xy, &tags, 0, nx, ny, normtype, &kdt, _state);
                }
                else
                {
                    kdtreebuild(&xy, 0, nx, ny, normtype, &kdt, _state);
                }
                
                /*
                 * Test different queries
                 */
                ae_vector_set_length(&x, nx, _state);
                for(i=0; i<=nx-1; i++)
                {
                    x.ptr.p_double[i] = ae_randomreal(_state);
                }
                result = result||kdtreequeryknn(&kdt, &x, 1, ae_true, _state)>0;
                result = result||kdtreequeryrnn(&kdt, &x, 1.0E6, ae_true, _state)>0;
                result = result||kdtreequeryaknn(&kdt, &x, 1, ae_true, 2.0, _state)>0;
            }
        }
    }
    
    /*
     * Ability to handle array with a lot of duplicates without causing
     * stack overflow.
     *
     * Two situations are handled:
     * * array where ALL N elements are duplicates
     * * array where there are NK distinct elements and N-NK duplicates
     */
    nx = 2;
    ny = 1;
    n = 100000;
    nk = 100;
    v = ae_randomreal(_state);
    ae_matrix_set_length(&xy, n, nx+ny, _state);
    ae_vector_set_length(&x, nx, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=nx+ny-1; j++)
        {
            xy.ptr.pp_double[i][j] = v;
        }
    }
    kdtreebuild(&xy, n, nx, ny, 2, &kdt, _state);
    for(j=0; j<=nx-1; j++)
    {
        x.ptr.p_double[j] = v;
    }
    result = result||kdtreequeryrnn(&kdt, &x, 0.0001, ae_true, _state)!=n;
    for(i=0; i<=nk-1; i++)
    {
        for(j=0; j<=nx+ny-1; j++)
        {
            xy.ptr.pp_double[i][j] = ae_randomreal(_state);
        }
    }
    kdtreebuild(&xy, n, nx, ny, 2, &kdt, _state);
    result = result||kdtreequeryrnn(&kdt, &x, 0.0001, ae_true, _state)<n-nk;
    ae_frame_leave(_state);
    return result;
}


/*$ End $*/
