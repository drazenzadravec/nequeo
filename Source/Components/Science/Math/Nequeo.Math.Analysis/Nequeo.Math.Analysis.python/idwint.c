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


#include "stdafx.h"
#include "idwint.h"


/*$ Declarations $*/
static double idwint_idwqfactor = 1.5;
static ae_int_t idwint_idwkmin = 5;
static double idwint_idwcalcq(idwinterpolant* z,
     /* Real    */ ae_vector* x,
     ae_int_t k,
     ae_state *_state);
static void idwint_idwinit1(ae_int_t n,
     ae_int_t nx,
     ae_int_t d,
     ae_int_t nq,
     ae_int_t nw,
     idwinterpolant* z,
     ae_state *_state);
static void idwint_idwinternalsolver(/* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* fmatrix,
     /* Real    */ ae_vector* temp,
     ae_int_t n,
     ae_int_t m,
     ae_int_t* info,
     /* Real    */ ae_vector* x,
     double* taskrcond,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
IDW interpolation

INPUT PARAMETERS:
    Z   -   IDW interpolant built with one of model building
            subroutines.
    X   -   array[0..NX-1], interpolation point

Result:
    IDW interpolant Z(X)

  -- ALGLIB --
     Copyright 02.03.2010 by Bochkanov Sergey
*************************************************************************/
double idwcalc(idwinterpolant* z,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;
    double r;
    double s;
    double w;
    double v1;
    double v2;
    double d0;
    double di;
    double result;


    
    /*
     * these initializers are not really necessary,
     * but without them compiler complains about uninitialized locals
     */
    k = 0;
    
    /*
     * Query
     */
    if( z->modeltype==0 )
    {
        
        /*
         * NQ/NW-based model
         */
        k = kdtreequeryknn(&z->tree, x, z->nw, ae_true, _state);
        kdtreequeryresultsdistances(&z->tree, &z->rbuf, _state);
        kdtreequeryresultstags(&z->tree, &z->tbuf, _state);
    }
    if( z->modeltype==1 )
    {
        
        /*
         * R-based model
         */
        k = kdtreequeryrnn(&z->tree, x, z->r, ae_true, _state);
        kdtreequeryresultsdistances(&z->tree, &z->rbuf, _state);
        kdtreequeryresultstags(&z->tree, &z->tbuf, _state);
        if( k<idwint_idwkmin )
        {
            
            /*
             * we need at least IDWKMin points
             */
            k = kdtreequeryknn(&z->tree, x, idwint_idwkmin, ae_true, _state);
            kdtreequeryresultsdistances(&z->tree, &z->rbuf, _state);
            kdtreequeryresultstags(&z->tree, &z->tbuf, _state);
        }
    }
    
    /*
     * initialize weights for linear/quadratic members calculation.
     *
     * NOTE 1: weights are calculated using NORMALIZED modified
     * Shepard's formula. Original formula gives w(i) = sqr((R-di)/(R*di)),
     * where di is i-th distance, R is max(di). Modified formula have
     * following form:
     *     w_mod(i) = 1, if di=d0
     *     w_mod(i) = w(i)/w(0), if di<>d0
     *
     * NOTE 2: self-match is USED for this query
     *
     * NOTE 3: last point almost always gain zero weight, but it MUST
     * be used for fitting because sometimes it will gain NON-ZERO
     * weight - for example, when all distances are equal.
     */
    r = z->rbuf.ptr.p_double[k-1];
    d0 = z->rbuf.ptr.p_double[0];
    result = (double)(0);
    s = (double)(0);
    for(i=0; i<=k-1; i++)
    {
        di = z->rbuf.ptr.p_double[i];
        if( ae_fp_eq(di,d0) )
        {
            
            /*
             * distance is equal to shortest, set it 1.0
             * without explicitly calculating (which would give
             * us same result, but 'll expose us to the risk of
             * division by zero).
             */
            w = (double)(1);
        }
        else
        {
            
            /*
             * use normalized formula
             */
            v1 = (r-di)/(r-d0);
            v2 = d0/di;
            w = ae_sqr(v1*v2, _state);
        }
        result = result+w*idwint_idwcalcq(z, x, z->tbuf.ptr.p_int[i], _state);
        s = s+w;
    }
    result = result/s;
    return result;
}


/*************************************************************************
IDW interpolant using modified Shepard method for uniform point
distributions.

INPUT PARAMETERS:
    XY  -   X and Y values, array[0..N-1,0..NX].
            First NX columns contain X-values, last column contain
            Y-values.
    N   -   number of nodes, N>0.
    NX  -   space dimension, NX>=1.
    D   -   nodal function type, either:
            * 0     constant  model.  Just  for  demonstration only, worst
                    model ever.
            * 1     linear model, least squares fitting. Simpe  model  for
                    datasets too small for quadratic models
            * 2     quadratic  model,  least  squares  fitting. Best model
                    available (if your dataset is large enough).
            * -1    "fast"  linear  model,  use  with  caution!!!   It  is
                    significantly  faster than linear/quadratic and better
                    than constant model. But it is less robust (especially
                    in the presence of noise).
    NQ  -   number of points used to calculate  nodal  functions  (ignored
            for constant models). NQ should be LARGER than:
            * max(1.5*(1+NX),2^NX+1) for linear model,
            * max(3/4*(NX+2)*(NX+1),2^NX+1) for quadratic model.
            Values less than this threshold will be silently increased.
    NW  -   number of points used to calculate weights and to interpolate.
            Required: >=2^NX+1, values less than this  threshold  will  be
            silently increased.
            Recommended value: about 2*NQ

OUTPUT PARAMETERS:
    Z   -   IDW interpolant.
    
NOTES:
  * best results are obtained with quadratic models, worst - with constant
    models
  * when N is large, NQ and NW must be significantly smaller than  N  both
    to obtain optimal performance and to obtain optimal accuracy. In 2  or
    3-dimensional tasks NQ=15 and NW=25 are good values to start with.
  * NQ  and  NW  may  be  greater  than  N.  In  such  cases  they will be
    automatically decreased.
  * this subroutine is always succeeds (as long as correct parameters  are
    passed).
  * see  'Multivariate  Interpolation  of Large Sets of Scattered Data' by
    Robert J. Renka for more information on this algorithm.
  * this subroutine assumes that point distribution is uniform at the small
    scales.  If  it  isn't  -  for  example,  points are concentrated along
    "lines", but "lines" distribution is uniform at the larger scale - then
    you should use IDWBuildModifiedShepardR()


  -- ALGLIB PROJECT --
     Copyright 02.03.2010 by Bochkanov Sergey
*************************************************************************/
void idwbuildmodifiedshepard(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t nx,
     ae_int_t d,
     ae_int_t nq,
     ae_int_t nw,
     idwinterpolant* z,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t j2;
    ae_int_t j3;
    double v;
    double r;
    double s;
    double d0;
    double di;
    double v1;
    double v2;
    ae_int_t nc;
    ae_int_t offs;
    ae_vector x;
    ae_vector qrbuf;
    ae_matrix qxybuf;
    ae_vector y;
    ae_matrix fmatrix;
    ae_vector w;
    ae_vector qsol;
    ae_vector temp;
    ae_vector tags;
    ae_int_t info;
    double taskrcond;

    ae_frame_make(_state, &_frame_block);
    _idwinterpolant_clear(z);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&qrbuf, 0, DT_REAL, _state);
    ae_matrix_init(&qxybuf, 0, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_matrix_init(&fmatrix, 0, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&qsol, 0, DT_REAL, _state);
    ae_vector_init(&temp, 0, DT_REAL, _state);
    ae_vector_init(&tags, 0, DT_INT, _state);

    
    /*
     * these initializers are not really necessary,
     * but without them compiler complains about uninitialized locals
     */
    nc = 0;
    
    /*
     * assertions
     */
    ae_assert(n>0, "IDWBuildModifiedShepard: N<=0!", _state);
    ae_assert(nx>=1, "IDWBuildModifiedShepard: NX<1!", _state);
    ae_assert(d>=-1&&d<=2, "IDWBuildModifiedShepard: D<>-1 and D<>0 and D<>1 and D<>2!", _state);
    
    /*
     * Correct parameters if needed
     */
    if( d==1 )
    {
        nq = ae_maxint(nq, ae_iceil(idwint_idwqfactor*(1+nx), _state)+1, _state);
        nq = ae_maxint(nq, ae_round(ae_pow((double)(2), (double)(nx), _state), _state)+1, _state);
    }
    if( d==2 )
    {
        nq = ae_maxint(nq, ae_iceil(idwint_idwqfactor*(nx+2)*(nx+1)/2, _state)+1, _state);
        nq = ae_maxint(nq, ae_round(ae_pow((double)(2), (double)(nx), _state), _state)+1, _state);
    }
    nw = ae_maxint(nw, ae_round(ae_pow((double)(2), (double)(nx), _state), _state)+1, _state);
    nq = ae_minint(nq, n, _state);
    nw = ae_minint(nw, n, _state);
    
    /*
     * primary initialization of Z
     */
    idwint_idwinit1(n, nx, d, nq, nw, z, _state);
    z->modeltype = 0;
    
    /*
     * Create KD-tree
     */
    ae_vector_set_length(&tags, n, _state);
    for(i=0; i<=n-1; i++)
    {
        tags.ptr.p_int[i] = i;
    }
    kdtreebuildtagged(xy, &tags, n, nx, 1, 2, &z->tree, _state);
    
    /*
     * build nodal functions
     */
    ae_vector_set_length(&temp, nq+1, _state);
    ae_vector_set_length(&x, nx, _state);
    ae_vector_set_length(&qrbuf, nq, _state);
    ae_matrix_set_length(&qxybuf, nq, nx+1, _state);
    if( d==-1 )
    {
        ae_vector_set_length(&w, nq, _state);
    }
    if( d==1 )
    {
        ae_vector_set_length(&y, nq, _state);
        ae_vector_set_length(&w, nq, _state);
        ae_vector_set_length(&qsol, nx, _state);
        
        /*
         * NX for linear members,
         * 1 for temporary storage
         */
        ae_matrix_set_length(&fmatrix, nq, nx+1, _state);
    }
    if( d==2 )
    {
        ae_vector_set_length(&y, nq, _state);
        ae_vector_set_length(&w, nq, _state);
        ae_vector_set_length(&qsol, nx+ae_round(nx*(nx+1)*0.5, _state), _state);
        
        /*
         * NX for linear members,
         * Round(NX*(NX+1)*0.5) for quadratic model,
         * 1 for temporary storage
         */
        ae_matrix_set_length(&fmatrix, nq, nx+ae_round(nx*(nx+1)*0.5, _state)+1, _state);
    }
    for(i=0; i<=n-1; i++)
    {
        
        /*
         * Initialize center and function value.
         * If D=0 it is all what we need
         */
        ae_v_move(&z->q.ptr.pp_double[i][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nx));
        if( d==0 )
        {
            continue;
        }
        
        /*
         * calculate weights for linear/quadratic members calculation.
         *
         * NOTE 1: weights are calculated using NORMALIZED modified
         * Shepard's formula. Original formula is w(i) = sqr((R-di)/(R*di)),
         * where di is i-th distance, R is max(di). Modified formula have
         * following form:
         *     w_mod(i) = 1, if di=d0
         *     w_mod(i) = w(i)/w(0), if di<>d0
         *
         * NOTE 2: self-match is NOT used for this query
         *
         * NOTE 3: last point almost always gain zero weight, but it MUST
         * be used for fitting because sometimes it will gain NON-ZERO
         * weight - for example, when all distances are equal.
         */
        ae_v_move(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nx-1));
        k = kdtreequeryknn(&z->tree, &x, nq, ae_false, _state);
        kdtreequeryresultsxy(&z->tree, &qxybuf, _state);
        kdtreequeryresultsdistances(&z->tree, &qrbuf, _state);
        r = qrbuf.ptr.p_double[k-1];
        d0 = qrbuf.ptr.p_double[0];
        for(j=0; j<=k-1; j++)
        {
            di = qrbuf.ptr.p_double[j];
            if( ae_fp_eq(di,d0) )
            {
                
                /*
                 * distance is equal to shortest, set it 1.0
                 * without explicitly calculating (which would give
                 * us same result, but 'll expose us to the risk of
                 * division by zero).
                 */
                w.ptr.p_double[j] = (double)(1);
            }
            else
            {
                
                /*
                 * use normalized formula
                 */
                v1 = (r-di)/(r-d0);
                v2 = d0/di;
                w.ptr.p_double[j] = ae_sqr(v1*v2, _state);
            }
        }
        
        /*
         * calculate linear/quadratic members
         */
        if( d==-1 )
        {
            
            /*
             * "Fast" linear nodal function calculated using
             * inverse distance weighting
             */
            for(j=0; j<=nx-1; j++)
            {
                x.ptr.p_double[j] = (double)(0);
            }
            s = (double)(0);
            for(j=0; j<=k-1; j++)
            {
                
                /*
                 * calculate J-th inverse distance weighted gradient:
                 *     grad_k = (y_j-y_k)*(x_j-x_k)/sqr(norm(x_j-x_k))
                 *     grad   = sum(wk*grad_k)/sum(w_k)
                 */
                v = (double)(0);
                for(j2=0; j2<=nx-1; j2++)
                {
                    v = v+ae_sqr(qxybuf.ptr.pp_double[j][j2]-xy->ptr.pp_double[i][j2], _state);
                }
                
                /*
                 * Although x_j<>x_k, sqr(norm(x_j-x_k)) may be zero due to
                 * underflow. If it is, we assume than J-th gradient is zero
                 * (i.e. don't add anything)
                 */
                if( ae_fp_neq(v,(double)(0)) )
                {
                    for(j2=0; j2<=nx-1; j2++)
                    {
                        x.ptr.p_double[j2] = x.ptr.p_double[j2]+w.ptr.p_double[j]*(qxybuf.ptr.pp_double[j][nx]-xy->ptr.pp_double[i][nx])*(qxybuf.ptr.pp_double[j][j2]-xy->ptr.pp_double[i][j2])/v;
                    }
                }
                s = s+w.ptr.p_double[j];
            }
            for(j=0; j<=nx-1; j++)
            {
                z->q.ptr.pp_double[i][nx+1+j] = x.ptr.p_double[j]/s;
            }
        }
        else
        {
            
            /*
             * Least squares models: build
             */
            if( d==1 )
            {
                
                /*
                 * Linear nodal function calculated using
                 * least squares fitting to its neighbors
                 */
                for(j=0; j<=k-1; j++)
                {
                    for(j2=0; j2<=nx-1; j2++)
                    {
                        fmatrix.ptr.pp_double[j][j2] = qxybuf.ptr.pp_double[j][j2]-xy->ptr.pp_double[i][j2];
                    }
                    y.ptr.p_double[j] = qxybuf.ptr.pp_double[j][nx]-xy->ptr.pp_double[i][nx];
                }
                nc = nx;
            }
            if( d==2 )
            {
                
                /*
                 * Quadratic nodal function calculated using
                 * least squares fitting to its neighbors
                 */
                for(j=0; j<=k-1; j++)
                {
                    offs = 0;
                    for(j2=0; j2<=nx-1; j2++)
                    {
                        fmatrix.ptr.pp_double[j][offs] = qxybuf.ptr.pp_double[j][j2]-xy->ptr.pp_double[i][j2];
                        offs = offs+1;
                    }
                    for(j2=0; j2<=nx-1; j2++)
                    {
                        for(j3=j2; j3<=nx-1; j3++)
                        {
                            fmatrix.ptr.pp_double[j][offs] = (qxybuf.ptr.pp_double[j][j2]-xy->ptr.pp_double[i][j2])*(qxybuf.ptr.pp_double[j][j3]-xy->ptr.pp_double[i][j3]);
                            offs = offs+1;
                        }
                    }
                    y.ptr.p_double[j] = qxybuf.ptr.pp_double[j][nx]-xy->ptr.pp_double[i][nx];
                }
                nc = nx+ae_round(nx*(nx+1)*0.5, _state);
            }
            idwint_idwinternalsolver(&y, &w, &fmatrix, &temp, k, nc, &info, &qsol, &taskrcond, _state);
            
            /*
             * Least squares models: copy results
             */
            if( info>0 )
            {
                
                /*
                 * LLS task is solved, copy results
                 */
                z->debugworstrcond = ae_minreal(z->debugworstrcond, taskrcond, _state);
                z->debugbestrcond = ae_maxreal(z->debugbestrcond, taskrcond, _state);
                for(j=0; j<=nc-1; j++)
                {
                    z->q.ptr.pp_double[i][nx+1+j] = qsol.ptr.p_double[j];
                }
            }
            else
            {
                
                /*
                 * Solver failure, very strange, but we will use
                 * zero values to handle it.
                 */
                z->debugsolverfailures = z->debugsolverfailures+1;
                for(j=0; j<=nc-1; j++)
                {
                    z->q.ptr.pp_double[i][nx+1+j] = (double)(0);
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
IDW interpolant using modified Shepard method for non-uniform datasets.

This type of model uses  constant  nodal  functions and interpolates using
all nodes which are closer than user-specified radius R. It  may  be  used
when points distribution is non-uniform at the small scale, but it  is  at
the distances as large as R.

INPUT PARAMETERS:
    XY  -   X and Y values, array[0..N-1,0..NX].
            First NX columns contain X-values, last column contain
            Y-values.
    N   -   number of nodes, N>0.
    NX  -   space dimension, NX>=1.
    R   -   radius, R>0

OUTPUT PARAMETERS:
    Z   -   IDW interpolant.

NOTES:
* if there is less than IDWKMin points within  R-ball,  algorithm  selects
  IDWKMin closest ones, so that continuity properties of  interpolant  are
  preserved even far from points.

  -- ALGLIB PROJECT --
     Copyright 11.04.2010 by Bochkanov Sergey
*************************************************************************/
void idwbuildmodifiedshepardr(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t nx,
     double r,
     idwinterpolant* z,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_vector tags;

    ae_frame_make(_state, &_frame_block);
    _idwinterpolant_clear(z);
    ae_vector_init(&tags, 0, DT_INT, _state);

    
    /*
     * assertions
     */
    ae_assert(n>0, "IDWBuildModifiedShepardR: N<=0!", _state);
    ae_assert(nx>=1, "IDWBuildModifiedShepardR: NX<1!", _state);
    ae_assert(ae_fp_greater(r,(double)(0)), "IDWBuildModifiedShepardR: R<=0!", _state);
    
    /*
     * primary initialization of Z
     */
    idwint_idwinit1(n, nx, 0, 0, n, z, _state);
    z->modeltype = 1;
    z->r = r;
    
    /*
     * Create KD-tree
     */
    ae_vector_set_length(&tags, n, _state);
    for(i=0; i<=n-1; i++)
    {
        tags.ptr.p_int[i] = i;
    }
    kdtreebuildtagged(xy, &tags, n, nx, 1, 2, &z->tree, _state);
    
    /*
     * build nodal functions
     */
    for(i=0; i<=n-1; i++)
    {
        ae_v_move(&z->q.ptr.pp_double[i][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nx));
    }
    ae_frame_leave(_state);
}


/*************************************************************************
IDW model for noisy data.

This subroutine may be used to handle noisy data, i.e. data with noise  in
OUTPUT values.  It differs from IDWBuildModifiedShepard() in the following
aspects:
* nodal functions are not constrained to pass through  nodes:  Qi(xi)<>yi,
  i.e. we have fitting  instead  of  interpolation.
* weights which are used during least  squares fitting stage are all equal
  to 1.0 (independently of distance)
* "fast"-linear or constant nodal functions are not supported (either  not
  robust enough or too rigid)

This problem require far more complex tuning than interpolation  problems.
Below you can find some recommendations regarding this problem:
* focus on tuning NQ; it controls noise reduction. As for NW, you can just
  make it equal to 2*NQ.
* you can use cross-validation to determine optimal NQ.
* optimal NQ is a result of complex tradeoff  between  noise  level  (more
  noise = larger NQ required) and underlying  function  complexity  (given
  fixed N, larger NQ means smoothing of compex features in the data).  For
  example, NQ=N will reduce noise to the minimum level possible,  but  you
  will end up with just constant/linear/quadratic (depending on  D)  least
  squares model for the whole dataset.

INPUT PARAMETERS:
    XY  -   X and Y values, array[0..N-1,0..NX].
            First NX columns contain X-values, last column contain
            Y-values.
    N   -   number of nodes, N>0.
    NX  -   space dimension, NX>=1.
    D   -   nodal function degree, either:
            * 1     linear model, least squares fitting. Simpe  model  for
                    datasets too small for quadratic models (or  for  very
                    noisy problems).
            * 2     quadratic  model,  least  squares  fitting. Best model
                    available (if your dataset is large enough).
    NQ  -   number of points used to calculate nodal functions.  NQ should
            be  significantly   larger   than  1.5  times  the  number  of
            coefficients in a nodal function to overcome effects of noise:
            * larger than 1.5*(1+NX) for linear model,
            * larger than 3/4*(NX+2)*(NX+1) for quadratic model.
            Values less than this threshold will be silently increased.
    NW  -   number of points used to calculate weights and to interpolate.
            Required: >=2^NX+1, values less than this  threshold  will  be
            silently increased.
            Recommended value: about 2*NQ or larger

OUTPUT PARAMETERS:
    Z   -   IDW interpolant.

NOTES:
  * best results are obtained with quadratic models, linear models are not
    recommended to use unless you are pretty sure that it is what you want
  * this subroutine is always succeeds (as long as correct parameters  are
    passed).
  * see  'Multivariate  Interpolation  of Large Sets of Scattered Data' by
    Robert J. Renka for more information on this algorithm.


  -- ALGLIB PROJECT --
     Copyright 02.03.2010 by Bochkanov Sergey
*************************************************************************/
void idwbuildnoisy(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t nx,
     ae_int_t d,
     ae_int_t nq,
     ae_int_t nw,
     idwinterpolant* z,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t j2;
    ae_int_t j3;
    double v;
    ae_int_t nc;
    ae_int_t offs;
    double taskrcond;
    ae_vector x;
    ae_vector qrbuf;
    ae_matrix qxybuf;
    ae_vector y;
    ae_vector w;
    ae_matrix fmatrix;
    ae_vector qsol;
    ae_vector tags;
    ae_vector temp;
    ae_int_t info;

    ae_frame_make(_state, &_frame_block);
    _idwinterpolant_clear(z);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&qrbuf, 0, DT_REAL, _state);
    ae_matrix_init(&qxybuf, 0, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_matrix_init(&fmatrix, 0, 0, DT_REAL, _state);
    ae_vector_init(&qsol, 0, DT_REAL, _state);
    ae_vector_init(&tags, 0, DT_INT, _state);
    ae_vector_init(&temp, 0, DT_REAL, _state);

    
    /*
     * these initializers are not really necessary,
     * but without them compiler complains about uninitialized locals
     */
    nc = 0;
    
    /*
     * assertions
     */
    ae_assert(n>0, "IDWBuildNoisy: N<=0!", _state);
    ae_assert(nx>=1, "IDWBuildNoisy: NX<1!", _state);
    ae_assert(d>=1&&d<=2, "IDWBuildNoisy: D<>1 and D<>2!", _state);
    
    /*
     * Correct parameters if needed
     */
    if( d==1 )
    {
        nq = ae_maxint(nq, ae_iceil(idwint_idwqfactor*(1+nx), _state)+1, _state);
    }
    if( d==2 )
    {
        nq = ae_maxint(nq, ae_iceil(idwint_idwqfactor*(nx+2)*(nx+1)/2, _state)+1, _state);
    }
    nw = ae_maxint(nw, ae_round(ae_pow((double)(2), (double)(nx), _state), _state)+1, _state);
    nq = ae_minint(nq, n, _state);
    nw = ae_minint(nw, n, _state);
    
    /*
     * primary initialization of Z
     */
    idwint_idwinit1(n, nx, d, nq, nw, z, _state);
    z->modeltype = 0;
    
    /*
     * Create KD-tree
     */
    ae_vector_set_length(&tags, n, _state);
    for(i=0; i<=n-1; i++)
    {
        tags.ptr.p_int[i] = i;
    }
    kdtreebuildtagged(xy, &tags, n, nx, 1, 2, &z->tree, _state);
    
    /*
     * build nodal functions
     * (special algorithm for noisy data is used)
     */
    ae_vector_set_length(&temp, nq+1, _state);
    ae_vector_set_length(&x, nx, _state);
    ae_vector_set_length(&qrbuf, nq, _state);
    ae_matrix_set_length(&qxybuf, nq, nx+1, _state);
    if( d==1 )
    {
        ae_vector_set_length(&y, nq, _state);
        ae_vector_set_length(&w, nq, _state);
        ae_vector_set_length(&qsol, 1+nx, _state);
        
        /*
         * 1 for constant member,
         * NX for linear members,
         * 1 for temporary storage
         */
        ae_matrix_set_length(&fmatrix, nq, 1+nx+1, _state);
    }
    if( d==2 )
    {
        ae_vector_set_length(&y, nq, _state);
        ae_vector_set_length(&w, nq, _state);
        ae_vector_set_length(&qsol, 1+nx+ae_round(nx*(nx+1)*0.5, _state), _state);
        
        /*
         * 1 for constant member,
         * NX for linear members,
         * Round(NX*(NX+1)*0.5) for quadratic model,
         * 1 for temporary storage
         */
        ae_matrix_set_length(&fmatrix, nq, 1+nx+ae_round(nx*(nx+1)*0.5, _state)+1, _state);
    }
    for(i=0; i<=n-1; i++)
    {
        
        /*
         * Initialize center.
         */
        ae_v_move(&z->q.ptr.pp_double[i][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nx-1));
        
        /*
         * Calculate linear/quadratic members
         * using least squares fit
         * NOTE 1: all weight are equal to 1.0
         * NOTE 2: self-match is USED for this query
         */
        ae_v_move(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nx-1));
        k = kdtreequeryknn(&z->tree, &x, nq, ae_true, _state);
        kdtreequeryresultsxy(&z->tree, &qxybuf, _state);
        kdtreequeryresultsdistances(&z->tree, &qrbuf, _state);
        if( d==1 )
        {
            
            /*
             * Linear nodal function calculated using
             * least squares fitting to its neighbors
             */
            for(j=0; j<=k-1; j++)
            {
                fmatrix.ptr.pp_double[j][0] = 1.0;
                for(j2=0; j2<=nx-1; j2++)
                {
                    fmatrix.ptr.pp_double[j][1+j2] = qxybuf.ptr.pp_double[j][j2]-xy->ptr.pp_double[i][j2];
                }
                y.ptr.p_double[j] = qxybuf.ptr.pp_double[j][nx];
                w.ptr.p_double[j] = (double)(1);
            }
            nc = 1+nx;
        }
        if( d==2 )
        {
            
            /*
             * Quadratic nodal function calculated using
             * least squares fitting to its neighbors
             */
            for(j=0; j<=k-1; j++)
            {
                fmatrix.ptr.pp_double[j][0] = (double)(1);
                offs = 1;
                for(j2=0; j2<=nx-1; j2++)
                {
                    fmatrix.ptr.pp_double[j][offs] = qxybuf.ptr.pp_double[j][j2]-xy->ptr.pp_double[i][j2];
                    offs = offs+1;
                }
                for(j2=0; j2<=nx-1; j2++)
                {
                    for(j3=j2; j3<=nx-1; j3++)
                    {
                        fmatrix.ptr.pp_double[j][offs] = (qxybuf.ptr.pp_double[j][j2]-xy->ptr.pp_double[i][j2])*(qxybuf.ptr.pp_double[j][j3]-xy->ptr.pp_double[i][j3]);
                        offs = offs+1;
                    }
                }
                y.ptr.p_double[j] = qxybuf.ptr.pp_double[j][nx];
                w.ptr.p_double[j] = (double)(1);
            }
            nc = 1+nx+ae_round(nx*(nx+1)*0.5, _state);
        }
        idwint_idwinternalsolver(&y, &w, &fmatrix, &temp, k, nc, &info, &qsol, &taskrcond, _state);
        
        /*
         * Least squares models: copy results
         */
        if( info>0 )
        {
            
            /*
             * LLS task is solved, copy results
             */
            z->debugworstrcond = ae_minreal(z->debugworstrcond, taskrcond, _state);
            z->debugbestrcond = ae_maxreal(z->debugbestrcond, taskrcond, _state);
            for(j=0; j<=nc-1; j++)
            {
                z->q.ptr.pp_double[i][nx+j] = qsol.ptr.p_double[j];
            }
        }
        else
        {
            
            /*
             * Solver failure, very strange, but we will use
             * zero values to handle it.
             */
            z->debugsolverfailures = z->debugsolverfailures+1;
            v = (double)(0);
            for(j=0; j<=k-1; j++)
            {
                v = v+qxybuf.ptr.pp_double[j][nx];
            }
            z->q.ptr.pp_double[i][nx] = v/k;
            for(j=0; j<=nc-2; j++)
            {
                z->q.ptr.pp_double[i][nx+1+j] = (double)(0);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine: K-th nodal function calculation

  -- ALGLIB --
     Copyright 02.03.2010 by Bochkanov Sergey
*************************************************************************/
static double idwint_idwcalcq(idwinterpolant* z,
     /* Real    */ ae_vector* x,
     ae_int_t k,
     ae_state *_state)
{
    ae_int_t nx;
    ae_int_t i;
    ae_int_t j;
    ae_int_t offs;
    double result;


    nx = z->nx;
    
    /*
     * constant member
     */
    result = z->q.ptr.pp_double[k][nx];
    
    /*
     * linear members
     */
    if( z->d>=1 )
    {
        for(i=0; i<=nx-1; i++)
        {
            result = result+z->q.ptr.pp_double[k][nx+1+i]*(x->ptr.p_double[i]-z->q.ptr.pp_double[k][i]);
        }
    }
    
    /*
     * quadratic members
     */
    if( z->d>=2 )
    {
        offs = nx+1+nx;
        for(i=0; i<=nx-1; i++)
        {
            for(j=i; j<=nx-1; j++)
            {
                result = result+z->q.ptr.pp_double[k][offs]*(x->ptr.p_double[i]-z->q.ptr.pp_double[k][i])*(x->ptr.p_double[j]-z->q.ptr.pp_double[k][j]);
                offs = offs+1;
            }
        }
    }
    return result;
}


/*************************************************************************
Initialization of internal structures.

It assumes correctness of all parameters.

  -- ALGLIB --
     Copyright 02.03.2010 by Bochkanov Sergey
*************************************************************************/
static void idwint_idwinit1(ae_int_t n,
     ae_int_t nx,
     ae_int_t d,
     ae_int_t nq,
     ae_int_t nw,
     idwinterpolant* z,
     ae_state *_state)
{


    z->debugsolverfailures = 0;
    z->debugworstrcond = 1.0;
    z->debugbestrcond = (double)(0);
    z->n = n;
    z->nx = nx;
    z->d = 0;
    if( d==1 )
    {
        z->d = 1;
    }
    if( d==2 )
    {
        z->d = 2;
    }
    if( d==-1 )
    {
        z->d = 1;
    }
    z->nw = nw;
    if( d==-1 )
    {
        ae_matrix_set_length(&z->q, n, nx+1+nx, _state);
    }
    if( d==0 )
    {
        ae_matrix_set_length(&z->q, n, nx+1, _state);
    }
    if( d==1 )
    {
        ae_matrix_set_length(&z->q, n, nx+1+nx, _state);
    }
    if( d==2 )
    {
        ae_matrix_set_length(&z->q, n, nx+1+nx+ae_round(nx*(nx+1)*0.5, _state), _state);
    }
    ae_vector_set_length(&z->tbuf, nw, _state);
    ae_vector_set_length(&z->rbuf, nw, _state);
    ae_matrix_set_length(&z->xybuf, nw, nx+1, _state);
    ae_vector_set_length(&z->xbuf, nx, _state);
}


/*************************************************************************
Linear least squares solver for small tasks.

Works faster than standard ALGLIB solver in non-degenerate cases  (due  to
absense of internal allocations and optimized row/colums).  In  degenerate
cases it calls standard solver, which results in small performance penalty
associated with preliminary steps.

INPUT PARAMETERS:
    Y           array[0..N-1]
    W           array[0..N-1]
    FMatrix     array[0..N-1,0..M], have additional column for temporary
                values
    Temp        array[0..N]
*************************************************************************/
static void idwint_idwinternalsolver(/* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* fmatrix,
     /* Real    */ ae_vector* temp,
     ae_int_t n,
     ae_int_t m,
     ae_int_t* info,
     /* Real    */ ae_vector* x,
     double* taskrcond,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    double v;
    double tau;
    ae_vector b;
    densesolverlsreport srep;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    ae_vector_init(&b, 0, DT_REAL, _state);
    _densesolverlsreport_init(&srep, _state);

    
    /*
     * set up info
     */
    *info = 1;
    
    /*
     * prepare matrix
     */
    for(i=0; i<=n-1; i++)
    {
        fmatrix->ptr.pp_double[i][m] = y->ptr.p_double[i];
        v = w->ptr.p_double[i];
        ae_v_muld(&fmatrix->ptr.pp_double[i][0], 1, ae_v_len(0,m), v);
    }
    
    /*
     * use either fast algorithm or general algorithm
     */
    if( m<=n )
    {
        
        /*
         * QR decomposition
         * We assume that M<=N (we would have called LSFit() otherwise)
         */
        for(i=0; i<=m-1; i++)
        {
            if( i<n-1 )
            {
                ae_v_move(&temp->ptr.p_double[1], 1, &fmatrix->ptr.pp_double[i][i], fmatrix->stride, ae_v_len(1,n-i));
                generatereflection(temp, n-i, &tau, _state);
                fmatrix->ptr.pp_double[i][i] = temp->ptr.p_double[1];
                temp->ptr.p_double[1] = (double)(1);
                for(j=i+1; j<=m; j++)
                {
                    v = ae_v_dotproduct(&fmatrix->ptr.pp_double[i][j], fmatrix->stride, &temp->ptr.p_double[1], 1, ae_v_len(i,n-1));
                    v = tau*v;
                    ae_v_subd(&fmatrix->ptr.pp_double[i][j], fmatrix->stride, &temp->ptr.p_double[1], 1, ae_v_len(i,n-1), v);
                }
            }
        }
        
        /*
         * Check condition number
         */
        *taskrcond = rmatrixtrrcondinf(fmatrix, m, ae_true, ae_false, _state);
        
        /*
         * use either fast algorithm for non-degenerate cases
         * or slow algorithm for degenerate cases
         */
        if( ae_fp_greater(*taskrcond,10000*n*ae_machineepsilon) )
        {
            
            /*
             * solve triangular system R*x = FMatrix[0:M-1,M]
             * using fast algorithm, then exit
             */
            x->ptr.p_double[m-1] = fmatrix->ptr.pp_double[m-1][m]/fmatrix->ptr.pp_double[m-1][m-1];
            for(i=m-2; i>=0; i--)
            {
                v = ae_v_dotproduct(&fmatrix->ptr.pp_double[i][i+1], 1, &x->ptr.p_double[i+1], 1, ae_v_len(i+1,m-1));
                x->ptr.p_double[i] = (fmatrix->ptr.pp_double[i][m]-v)/fmatrix->ptr.pp_double[i][i];
            }
        }
        else
        {
            
            /*
             * use more general algorithm
             */
            ae_vector_set_length(&b, m, _state);
            for(i=0; i<=m-1; i++)
            {
                for(j=0; j<=i-1; j++)
                {
                    fmatrix->ptr.pp_double[i][j] = 0.0;
                }
                b.ptr.p_double[i] = fmatrix->ptr.pp_double[i][m];
            }
            rmatrixsolvels(fmatrix, m, m, &b, 10000*ae_machineepsilon, info, &srep, x, _state);
        }
    }
    else
    {
        
        /*
         * use more general algorithm
         */
        ae_vector_set_length(&b, n, _state);
        for(i=0; i<=n-1; i++)
        {
            b.ptr.p_double[i] = fmatrix->ptr.pp_double[i][m];
        }
        rmatrixsolvels(fmatrix, n, m, &b, 10000*ae_machineepsilon, info, &srep, x, _state);
        *taskrcond = srep.r2;
    }
    ae_frame_leave(_state);
}


void _idwinterpolant_init(void* _p, ae_state *_state)
{
    idwinterpolant *p = (idwinterpolant*)_p;
    ae_touch_ptr((void*)p);
    _kdtree_init(&p->tree, _state);
    ae_matrix_init(&p->q, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->xbuf, 0, DT_REAL, _state);
    ae_vector_init(&p->tbuf, 0, DT_INT, _state);
    ae_vector_init(&p->rbuf, 0, DT_REAL, _state);
    ae_matrix_init(&p->xybuf, 0, 0, DT_REAL, _state);
}


void _idwinterpolant_init_copy(void* _dst, void* _src, ae_state *_state)
{
    idwinterpolant *dst = (idwinterpolant*)_dst;
    idwinterpolant *src = (idwinterpolant*)_src;
    dst->n = src->n;
    dst->nx = src->nx;
    dst->d = src->d;
    dst->r = src->r;
    dst->nw = src->nw;
    _kdtree_init_copy(&dst->tree, &src->tree, _state);
    dst->modeltype = src->modeltype;
    ae_matrix_init_copy(&dst->q, &src->q, _state);
    ae_vector_init_copy(&dst->xbuf, &src->xbuf, _state);
    ae_vector_init_copy(&dst->tbuf, &src->tbuf, _state);
    ae_vector_init_copy(&dst->rbuf, &src->rbuf, _state);
    ae_matrix_init_copy(&dst->xybuf, &src->xybuf, _state);
    dst->debugsolverfailures = src->debugsolverfailures;
    dst->debugworstrcond = src->debugworstrcond;
    dst->debugbestrcond = src->debugbestrcond;
}


void _idwinterpolant_clear(void* _p)
{
    idwinterpolant *p = (idwinterpolant*)_p;
    ae_touch_ptr((void*)p);
    _kdtree_clear(&p->tree);
    ae_matrix_clear(&p->q);
    ae_vector_clear(&p->xbuf);
    ae_vector_clear(&p->tbuf);
    ae_vector_clear(&p->rbuf);
    ae_matrix_clear(&p->xybuf);
}


void _idwinterpolant_destroy(void* _p)
{
    idwinterpolant *p = (idwinterpolant*)_p;
    ae_touch_ptr((void*)p);
    _kdtree_destroy(&p->tree);
    ae_matrix_destroy(&p->q);
    ae_vector_destroy(&p->xbuf);
    ae_vector_destroy(&p->tbuf);
    ae_vector_destroy(&p->rbuf);
    ae_matrix_destroy(&p->xybuf);
}


/*$ End $*/
