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
#include "dforest.h"


/*$ Declarations $*/
static ae_int_t dforest_innernodewidth = 3;
static ae_int_t dforest_leafnodewidth = 2;
static ae_int_t dforest_dfusestrongsplits = 1;
static ae_int_t dforest_dfuseevs = 2;
static ae_int_t dforest_dffirstversion = 0;
static ae_int_t dforest_dfclserror(decisionforest* df,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state);
static void dforest_dfprocessinternal(decisionforest* df,
     ae_int_t offs,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state);
static void dforest_dfbuildtree(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t nclasses,
     ae_int_t nfeatures,
     ae_int_t nvarsinpool,
     ae_int_t flags,
     dfinternalbuffers* bufs,
     hqrndstate* rs,
     ae_state *_state);
static void dforest_dfbuildtreerec(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t nclasses,
     ae_int_t nfeatures,
     ae_int_t nvarsinpool,
     ae_int_t flags,
     ae_int_t* numprocessed,
     ae_int_t idx1,
     ae_int_t idx2,
     dfinternalbuffers* bufs,
     hqrndstate* rs,
     ae_state *_state);
static void dforest_dfsplitc(/* Real    */ ae_vector* x,
     /* Integer */ ae_vector* c,
     /* Integer */ ae_vector* cntbuf,
     ae_int_t n,
     ae_int_t nc,
     ae_int_t flags,
     ae_int_t* info,
     double* threshold,
     double* e,
     /* Real    */ ae_vector* sortrbuf,
     /* Integer */ ae_vector* sortibuf,
     ae_state *_state);
static void dforest_dfsplitr(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t flags,
     ae_int_t* info,
     double* threshold,
     double* e,
     /* Real    */ ae_vector* sortrbuf,
     /* Real    */ ae_vector* sortrbuf2,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
This subroutine builds random decision forest.

INPUT PARAMETERS:
    XY          -   training set
    NPoints     -   training set size, NPoints>=1
    NVars       -   number of independent variables, NVars>=1
    NClasses    -   task type:
                    * NClasses=1 - regression task with one
                                   dependent variable
                    * NClasses>1 - classification task with
                                   NClasses classes.
    NTrees      -   number of trees in a forest, NTrees>=1.
                    recommended values: 50-100.
    R           -   percent of a training set used to build
                    individual trees. 0<R<=1.
                    recommended values: 0.1 <= R <= 0.66.

OUTPUT PARAMETERS:
    Info        -   return code:
                    * -2, if there is a point with class number
                          outside of [0..NClasses-1].
                    * -1, if incorrect parameters was passed
                          (NPoints<1, NVars<1, NClasses<1, NTrees<1, R<=0
                          or R>1).
                    *  1, if task has been solved
    DF          -   model built
    Rep         -   training report, contains error on a training set
                    and out-of-bag estimates of generalization error.

  -- ALGLIB --
     Copyright 19.02.2009 by Bochkanov Sergey
*************************************************************************/
void dfbuildrandomdecisionforest(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t nclasses,
     ae_int_t ntrees,
     double r,
     ae_int_t* info,
     decisionforest* df,
     dfreport* rep,
     ae_state *_state)
{
    ae_int_t samplesize;

    *info = 0;
    _decisionforest_clear(df);
    _dfreport_clear(rep);

    if( ae_fp_less_eq(r,(double)(0))||ae_fp_greater(r,(double)(1)) )
    {
        *info = -1;
        return;
    }
    samplesize = ae_maxint(ae_round(r*npoints, _state), 1, _state);
    dfbuildinternal(xy, npoints, nvars, nclasses, ntrees, samplesize, ae_maxint(nvars/2, 1, _state), dforest_dfusestrongsplits+dforest_dfuseevs, info, df, rep, _state);
}


/*************************************************************************
This subroutine builds random decision forest.
This function gives ability to tune number of variables used when choosing
best split.

INPUT PARAMETERS:
    XY          -   training set
    NPoints     -   training set size, NPoints>=1
    NVars       -   number of independent variables, NVars>=1
    NClasses    -   task type:
                    * NClasses=1 - regression task with one
                                   dependent variable
                    * NClasses>1 - classification task with
                                   NClasses classes.
    NTrees      -   number of trees in a forest, NTrees>=1.
                    recommended values: 50-100.
    NRndVars    -   number of variables used when choosing best split
    R           -   percent of a training set used to build
                    individual trees. 0<R<=1.
                    recommended values: 0.1 <= R <= 0.66.

OUTPUT PARAMETERS:
    Info        -   return code:
                    * -2, if there is a point with class number
                          outside of [0..NClasses-1].
                    * -1, if incorrect parameters was passed
                          (NPoints<1, NVars<1, NClasses<1, NTrees<1, R<=0
                          or R>1).
                    *  1, if task has been solved
    DF          -   model built
    Rep         -   training report, contains error on a training set
                    and out-of-bag estimates of generalization error.

  -- ALGLIB --
     Copyright 19.02.2009 by Bochkanov Sergey
*************************************************************************/
void dfbuildrandomdecisionforestx1(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t nclasses,
     ae_int_t ntrees,
     ae_int_t nrndvars,
     double r,
     ae_int_t* info,
     decisionforest* df,
     dfreport* rep,
     ae_state *_state)
{
    ae_int_t samplesize;

    *info = 0;
    _decisionforest_clear(df);
    _dfreport_clear(rep);

    if( ae_fp_less_eq(r,(double)(0))||ae_fp_greater(r,(double)(1)) )
    {
        *info = -1;
        return;
    }
    if( nrndvars<=0||nrndvars>nvars )
    {
        *info = -1;
        return;
    }
    samplesize = ae_maxint(ae_round(r*npoints, _state), 1, _state);
    dfbuildinternal(xy, npoints, nvars, nclasses, ntrees, samplesize, nrndvars, dforest_dfusestrongsplits+dforest_dfuseevs, info, df, rep, _state);
}


void dfbuildinternal(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t nclasses,
     ae_int_t ntrees,
     ae_int_t samplesize,
     ae_int_t nfeatures,
     ae_int_t flags,
     ae_int_t* info,
     decisionforest* df,
     dfreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t tmpi;
    ae_int_t lasttreeoffs;
    ae_int_t offs;
    ae_int_t ooboffs;
    ae_int_t treesize;
    ae_int_t nvarsinpool;
    ae_bool useevs;
    dfinternalbuffers bufs;
    ae_vector permbuf;
    ae_vector oobbuf;
    ae_vector oobcntbuf;
    ae_matrix xys;
    ae_vector x;
    ae_vector y;
    ae_int_t oobcnt;
    ae_int_t oobrelcnt;
    double v;
    double vmin;
    double vmax;
    ae_bool bflag;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _decisionforest_clear(df);
    _dfreport_clear(rep);
    _dfinternalbuffers_init(&bufs, _state);
    ae_vector_init(&permbuf, 0, DT_INT, _state);
    ae_vector_init(&oobbuf, 0, DT_REAL, _state);
    ae_vector_init(&oobcntbuf, 0, DT_INT, _state);
    ae_matrix_init(&xys, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    _hqrndstate_init(&rs, _state);

    
    /*
     * Test for inputs
     */
    if( (((((npoints<1||samplesize<1)||samplesize>npoints)||nvars<1)||nclasses<1)||ntrees<1)||nfeatures<1 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    if( nclasses>1 )
    {
        for(i=0; i<=npoints-1; i++)
        {
            if( ae_round(xy->ptr.pp_double[i][nvars], _state)<0||ae_round(xy->ptr.pp_double[i][nvars], _state)>=nclasses )
            {
                *info = -2;
                ae_frame_leave(_state);
                return;
            }
        }
    }
    *info = 1;
    
    /*
     * Flags
     */
    useevs = flags/dforest_dfuseevs%2!=0;
    
    /*
     * Allocate data, prepare header
     */
    treesize = 1+dforest_innernodewidth*(samplesize-1)+dforest_leafnodewidth*samplesize;
    ae_vector_set_length(&permbuf, npoints-1+1, _state);
    ae_vector_set_length(&bufs.treebuf, treesize-1+1, _state);
    ae_vector_set_length(&bufs.idxbuf, npoints-1+1, _state);
    ae_vector_set_length(&bufs.tmpbufr, npoints-1+1, _state);
    ae_vector_set_length(&bufs.tmpbufr2, npoints-1+1, _state);
    ae_vector_set_length(&bufs.tmpbufi, npoints-1+1, _state);
    ae_vector_set_length(&bufs.sortrbuf, npoints, _state);
    ae_vector_set_length(&bufs.sortrbuf2, npoints, _state);
    ae_vector_set_length(&bufs.sortibuf, npoints, _state);
    ae_vector_set_length(&bufs.varpool, nvars-1+1, _state);
    ae_vector_set_length(&bufs.evsbin, nvars-1+1, _state);
    ae_vector_set_length(&bufs.evssplits, nvars-1+1, _state);
    ae_vector_set_length(&bufs.classibuf, 2*nclasses-1+1, _state);
    ae_vector_set_length(&oobbuf, nclasses*npoints-1+1, _state);
    ae_vector_set_length(&oobcntbuf, npoints-1+1, _state);
    ae_vector_set_length(&df->trees, ntrees*treesize-1+1, _state);
    ae_matrix_set_length(&xys, samplesize-1+1, nvars+1, _state);
    ae_vector_set_length(&x, nvars-1+1, _state);
    ae_vector_set_length(&y, nclasses-1+1, _state);
    for(i=0; i<=npoints-1; i++)
    {
        permbuf.ptr.p_int[i] = i;
    }
    for(i=0; i<=npoints*nclasses-1; i++)
    {
        oobbuf.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=npoints-1; i++)
    {
        oobcntbuf.ptr.p_int[i] = 0;
    }
    
    /*
     * Prepare variable pool and EVS (extended variable selection/splitting) buffers
     * (whether EVS is turned on or not):
     * 1. detect binary variables and pre-calculate splits for them
     * 2. detect variables with non-distinct values and exclude them from pool
     */
    for(i=0; i<=nvars-1; i++)
    {
        bufs.varpool.ptr.p_int[i] = i;
    }
    nvarsinpool = nvars;
    if( useevs )
    {
        for(j=0; j<=nvars-1; j++)
        {
            vmin = xy->ptr.pp_double[0][j];
            vmax = vmin;
            for(i=0; i<=npoints-1; i++)
            {
                v = xy->ptr.pp_double[i][j];
                vmin = ae_minreal(vmin, v, _state);
                vmax = ae_maxreal(vmax, v, _state);
            }
            if( ae_fp_eq(vmin,vmax) )
            {
                
                /*
                 * exclude variable from pool
                 */
                bufs.varpool.ptr.p_int[j] = bufs.varpool.ptr.p_int[nvarsinpool-1];
                bufs.varpool.ptr.p_int[nvarsinpool-1] = -1;
                nvarsinpool = nvarsinpool-1;
                continue;
            }
            bflag = ae_false;
            for(i=0; i<=npoints-1; i++)
            {
                v = xy->ptr.pp_double[i][j];
                if( ae_fp_neq(v,vmin)&&ae_fp_neq(v,vmax) )
                {
                    bflag = ae_true;
                    break;
                }
            }
            if( bflag )
            {
                
                /*
                 * non-binary variable
                 */
                bufs.evsbin.ptr.p_bool[j] = ae_false;
            }
            else
            {
                
                /*
                 * Prepare
                 */
                bufs.evsbin.ptr.p_bool[j] = ae_true;
                bufs.evssplits.ptr.p_double[j] = 0.5*(vmin+vmax);
                if( ae_fp_less_eq(bufs.evssplits.ptr.p_double[j],vmin) )
                {
                    bufs.evssplits.ptr.p_double[j] = vmax;
                }
            }
        }
    }
    
    /*
     * RANDOM FOREST FORMAT
     * W[0]         -   size of array
     * W[1]         -   version number
     * W[2]         -   NVars
     * W[3]         -   NClasses (1 for regression)
     * W[4]         -   NTrees
     * W[5]         -   trees offset
     *
     *
     * TREE FORMAT
     * W[Offs]      -   size of sub-array
     *     node info:
     * W[K+0]       -   variable number        (-1 for leaf mode)
     * W[K+1]       -   threshold              (class/value for leaf node)
     * W[K+2]       -   ">=" branch index      (absent for leaf node)
     *
     */
    df->nvars = nvars;
    df->nclasses = nclasses;
    df->ntrees = ntrees;
    
    /*
     * Build forest
     */
    hqrndrandomize(&rs, _state);
    offs = 0;
    for(i=0; i<=ntrees-1; i++)
    {
        
        /*
         * Prepare sample
         */
        for(k=0; k<=samplesize-1; k++)
        {
            j = k+hqrnduniformi(&rs, npoints-k, _state);
            tmpi = permbuf.ptr.p_int[k];
            permbuf.ptr.p_int[k] = permbuf.ptr.p_int[j];
            permbuf.ptr.p_int[j] = tmpi;
            j = permbuf.ptr.p_int[k];
            ae_v_move(&xys.ptr.pp_double[k][0], 1, &xy->ptr.pp_double[j][0], 1, ae_v_len(0,nvars));
        }
        
        /*
         * build tree, copy
         */
        dforest_dfbuildtree(&xys, samplesize, nvars, nclasses, nfeatures, nvarsinpool, flags, &bufs, &rs, _state);
        j = ae_round(bufs.treebuf.ptr.p_double[0], _state);
        ae_v_move(&df->trees.ptr.p_double[offs], 1, &bufs.treebuf.ptr.p_double[0], 1, ae_v_len(offs,offs+j-1));
        lasttreeoffs = offs;
        offs = offs+j;
        
        /*
         * OOB estimates
         */
        for(k=samplesize; k<=npoints-1; k++)
        {
            for(j=0; j<=nclasses-1; j++)
            {
                y.ptr.p_double[j] = (double)(0);
            }
            j = permbuf.ptr.p_int[k];
            ae_v_move(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[j][0], 1, ae_v_len(0,nvars-1));
            dforest_dfprocessinternal(df, lasttreeoffs, &x, &y, _state);
            ae_v_add(&oobbuf.ptr.p_double[j*nclasses], 1, &y.ptr.p_double[0], 1, ae_v_len(j*nclasses,(j+1)*nclasses-1));
            oobcntbuf.ptr.p_int[j] = oobcntbuf.ptr.p_int[j]+1;
        }
    }
    df->bufsize = offs;
    
    /*
     * Normalize OOB results
     */
    for(i=0; i<=npoints-1; i++)
    {
        if( oobcntbuf.ptr.p_int[i]!=0 )
        {
            v = (double)1/(double)oobcntbuf.ptr.p_int[i];
            ae_v_muld(&oobbuf.ptr.p_double[i*nclasses], 1, ae_v_len(i*nclasses,i*nclasses+nclasses-1), v);
        }
    }
    
    /*
     * Calculate training set estimates
     */
    rep->relclserror = dfrelclserror(df, xy, npoints, _state);
    rep->avgce = dfavgce(df, xy, npoints, _state);
    rep->rmserror = dfrmserror(df, xy, npoints, _state);
    rep->avgerror = dfavgerror(df, xy, npoints, _state);
    rep->avgrelerror = dfavgrelerror(df, xy, npoints, _state);
    
    /*
     * Calculate OOB estimates.
     */
    rep->oobrelclserror = (double)(0);
    rep->oobavgce = (double)(0);
    rep->oobrmserror = (double)(0);
    rep->oobavgerror = (double)(0);
    rep->oobavgrelerror = (double)(0);
    oobcnt = 0;
    oobrelcnt = 0;
    for(i=0; i<=npoints-1; i++)
    {
        if( oobcntbuf.ptr.p_int[i]!=0 )
        {
            ooboffs = i*nclasses;
            if( nclasses>1 )
            {
                
                /*
                 * classification-specific code
                 */
                k = ae_round(xy->ptr.pp_double[i][nvars], _state);
                tmpi = 0;
                for(j=1; j<=nclasses-1; j++)
                {
                    if( ae_fp_greater(oobbuf.ptr.p_double[ooboffs+j],oobbuf.ptr.p_double[ooboffs+tmpi]) )
                    {
                        tmpi = j;
                    }
                }
                if( tmpi!=k )
                {
                    rep->oobrelclserror = rep->oobrelclserror+1;
                }
                if( ae_fp_neq(oobbuf.ptr.p_double[ooboffs+k],(double)(0)) )
                {
                    rep->oobavgce = rep->oobavgce-ae_log(oobbuf.ptr.p_double[ooboffs+k], _state);
                }
                else
                {
                    rep->oobavgce = rep->oobavgce-ae_log(ae_minrealnumber, _state);
                }
                for(j=0; j<=nclasses-1; j++)
                {
                    if( j==k )
                    {
                        rep->oobrmserror = rep->oobrmserror+ae_sqr(oobbuf.ptr.p_double[ooboffs+j]-1, _state);
                        rep->oobavgerror = rep->oobavgerror+ae_fabs(oobbuf.ptr.p_double[ooboffs+j]-1, _state);
                        rep->oobavgrelerror = rep->oobavgrelerror+ae_fabs(oobbuf.ptr.p_double[ooboffs+j]-1, _state);
                        oobrelcnt = oobrelcnt+1;
                    }
                    else
                    {
                        rep->oobrmserror = rep->oobrmserror+ae_sqr(oobbuf.ptr.p_double[ooboffs+j], _state);
                        rep->oobavgerror = rep->oobavgerror+ae_fabs(oobbuf.ptr.p_double[ooboffs+j], _state);
                    }
                }
            }
            else
            {
                
                /*
                 * regression-specific code
                 */
                rep->oobrmserror = rep->oobrmserror+ae_sqr(oobbuf.ptr.p_double[ooboffs]-xy->ptr.pp_double[i][nvars], _state);
                rep->oobavgerror = rep->oobavgerror+ae_fabs(oobbuf.ptr.p_double[ooboffs]-xy->ptr.pp_double[i][nvars], _state);
                if( ae_fp_neq(xy->ptr.pp_double[i][nvars],(double)(0)) )
                {
                    rep->oobavgrelerror = rep->oobavgrelerror+ae_fabs((oobbuf.ptr.p_double[ooboffs]-xy->ptr.pp_double[i][nvars])/xy->ptr.pp_double[i][nvars], _state);
                    oobrelcnt = oobrelcnt+1;
                }
            }
            
            /*
             * update OOB estimates count.
             */
            oobcnt = oobcnt+1;
        }
    }
    if( oobcnt>0 )
    {
        rep->oobrelclserror = rep->oobrelclserror/oobcnt;
        rep->oobavgce = rep->oobavgce/oobcnt;
        rep->oobrmserror = ae_sqrt(rep->oobrmserror/(oobcnt*nclasses), _state);
        rep->oobavgerror = rep->oobavgerror/(oobcnt*nclasses);
        if( oobrelcnt>0 )
        {
            rep->oobavgrelerror = rep->oobavgrelerror/oobrelcnt;
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Procesing

INPUT PARAMETERS:
    DF      -   decision forest model
    X       -   input vector,  array[0..NVars-1].

OUTPUT PARAMETERS:
    Y       -   result. Regression estimate when solving regression  task,
                vector of posterior probabilities for classification task.

See also DFProcessI.

  -- ALGLIB --
     Copyright 16.02.2009 by Bochkanov Sergey
*************************************************************************/
void dfprocess(decisionforest* df,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{
    ae_int_t offs;
    ae_int_t i;
    double v;


    
    /*
     * Proceed
     */
    if( y->cnt<df->nclasses )
    {
        ae_vector_set_length(y, df->nclasses, _state);
    }
    offs = 0;
    for(i=0; i<=df->nclasses-1; i++)
    {
        y->ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=df->ntrees-1; i++)
    {
        
        /*
         * Process basic tree
         */
        dforest_dfprocessinternal(df, offs, x, y, _state);
        
        /*
         * Next tree
         */
        offs = offs+ae_round(df->trees.ptr.p_double[offs], _state);
    }
    v = (double)1/(double)df->ntrees;
    ae_v_muld(&y->ptr.p_double[0], 1, ae_v_len(0,df->nclasses-1), v);
}


/*************************************************************************
'interactive' variant of DFProcess for languages like Python which support
constructs like "Y = DFProcessI(DF,X)" and interactive mode of interpreter

This function allocates new array on each call,  so  it  is  significantly
slower than its 'non-interactive' counterpart, but it is  more  convenient
when you call it from command line.

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
void dfprocessi(decisionforest* df,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{

    ae_vector_clear(y);

    dfprocess(df, x, y, _state);
}


/*************************************************************************
Relative classification error on the test set

INPUT PARAMETERS:
    DF      -   decision forest model
    XY      -   test set
    NPoints -   test set size

RESULT:
    percent of incorrectly classified cases.
    Zero if model solves regression task.

  -- ALGLIB --
     Copyright 16.02.2009 by Bochkanov Sergey
*************************************************************************/
double dfrelclserror(decisionforest* df,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double result;


    result = (double)dforest_dfclserror(df, xy, npoints, _state)/(double)npoints;
    return result;
}


/*************************************************************************
Average cross-entropy (in bits per element) on the test set

INPUT PARAMETERS:
    DF      -   decision forest model
    XY      -   test set
    NPoints -   test set size

RESULT:
    CrossEntropy/(NPoints*LN(2)).
    Zero if model solves regression task.

  -- ALGLIB --
     Copyright 16.02.2009 by Bochkanov Sergey
*************************************************************************/
double dfavgce(decisionforest* df,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    ae_vector y;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t tmpi;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);

    ae_vector_set_length(&x, df->nvars-1+1, _state);
    ae_vector_set_length(&y, df->nclasses-1+1, _state);
    result = (double)(0);
    for(i=0; i<=npoints-1; i++)
    {
        ae_v_move(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,df->nvars-1));
        dfprocess(df, &x, &y, _state);
        if( df->nclasses>1 )
        {
            
            /*
             * classification-specific code
             */
            k = ae_round(xy->ptr.pp_double[i][df->nvars], _state);
            tmpi = 0;
            for(j=1; j<=df->nclasses-1; j++)
            {
                if( ae_fp_greater(y.ptr.p_double[j],y.ptr.p_double[tmpi]) )
                {
                    tmpi = j;
                }
            }
            if( ae_fp_neq(y.ptr.p_double[k],(double)(0)) )
            {
                result = result-ae_log(y.ptr.p_double[k], _state);
            }
            else
            {
                result = result-ae_log(ae_minrealnumber, _state);
            }
        }
    }
    result = result/npoints;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
RMS error on the test set

INPUT PARAMETERS:
    DF      -   decision forest model
    XY      -   test set
    NPoints -   test set size

RESULT:
    root mean square error.
    Its meaning for regression task is obvious. As for
    classification task, RMS error means error when estimating posterior
    probabilities.

  -- ALGLIB --
     Copyright 16.02.2009 by Bochkanov Sergey
*************************************************************************/
double dfrmserror(decisionforest* df,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    ae_vector y;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t tmpi;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);

    ae_vector_set_length(&x, df->nvars-1+1, _state);
    ae_vector_set_length(&y, df->nclasses-1+1, _state);
    result = (double)(0);
    for(i=0; i<=npoints-1; i++)
    {
        ae_v_move(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,df->nvars-1));
        dfprocess(df, &x, &y, _state);
        if( df->nclasses>1 )
        {
            
            /*
             * classification-specific code
             */
            k = ae_round(xy->ptr.pp_double[i][df->nvars], _state);
            tmpi = 0;
            for(j=1; j<=df->nclasses-1; j++)
            {
                if( ae_fp_greater(y.ptr.p_double[j],y.ptr.p_double[tmpi]) )
                {
                    tmpi = j;
                }
            }
            for(j=0; j<=df->nclasses-1; j++)
            {
                if( j==k )
                {
                    result = result+ae_sqr(y.ptr.p_double[j]-1, _state);
                }
                else
                {
                    result = result+ae_sqr(y.ptr.p_double[j], _state);
                }
            }
        }
        else
        {
            
            /*
             * regression-specific code
             */
            result = result+ae_sqr(y.ptr.p_double[0]-xy->ptr.pp_double[i][df->nvars], _state);
        }
    }
    result = ae_sqrt(result/(npoints*df->nclasses), _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Average error on the test set

INPUT PARAMETERS:
    DF      -   decision forest model
    XY      -   test set
    NPoints -   test set size

RESULT:
    Its meaning for regression task is obvious. As for
    classification task, it means average error when estimating posterior
    probabilities.

  -- ALGLIB --
     Copyright 16.02.2009 by Bochkanov Sergey
*************************************************************************/
double dfavgerror(decisionforest* df,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    ae_vector y;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);

    ae_vector_set_length(&x, df->nvars-1+1, _state);
    ae_vector_set_length(&y, df->nclasses-1+1, _state);
    result = (double)(0);
    for(i=0; i<=npoints-1; i++)
    {
        ae_v_move(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,df->nvars-1));
        dfprocess(df, &x, &y, _state);
        if( df->nclasses>1 )
        {
            
            /*
             * classification-specific code
             */
            k = ae_round(xy->ptr.pp_double[i][df->nvars], _state);
            for(j=0; j<=df->nclasses-1; j++)
            {
                if( j==k )
                {
                    result = result+ae_fabs(y.ptr.p_double[j]-1, _state);
                }
                else
                {
                    result = result+ae_fabs(y.ptr.p_double[j], _state);
                }
            }
        }
        else
        {
            
            /*
             * regression-specific code
             */
            result = result+ae_fabs(y.ptr.p_double[0]-xy->ptr.pp_double[i][df->nvars], _state);
        }
    }
    result = result/(npoints*df->nclasses);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Average relative error on the test set

INPUT PARAMETERS:
    DF      -   decision forest model
    XY      -   test set
    NPoints -   test set size

RESULT:
    Its meaning for regression task is obvious. As for
    classification task, it means average relative error when estimating
    posterior probability of belonging to the correct class.

  -- ALGLIB --
     Copyright 16.02.2009 by Bochkanov Sergey
*************************************************************************/
double dfavgrelerror(decisionforest* df,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    ae_vector y;
    ae_int_t relcnt;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);

    ae_vector_set_length(&x, df->nvars-1+1, _state);
    ae_vector_set_length(&y, df->nclasses-1+1, _state);
    result = (double)(0);
    relcnt = 0;
    for(i=0; i<=npoints-1; i++)
    {
        ae_v_move(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,df->nvars-1));
        dfprocess(df, &x, &y, _state);
        if( df->nclasses>1 )
        {
            
            /*
             * classification-specific code
             */
            k = ae_round(xy->ptr.pp_double[i][df->nvars], _state);
            for(j=0; j<=df->nclasses-1; j++)
            {
                if( j==k )
                {
                    result = result+ae_fabs(y.ptr.p_double[j]-1, _state);
                    relcnt = relcnt+1;
                }
            }
        }
        else
        {
            
            /*
             * regression-specific code
             */
            if( ae_fp_neq(xy->ptr.pp_double[i][df->nvars],(double)(0)) )
            {
                result = result+ae_fabs((y.ptr.p_double[0]-xy->ptr.pp_double[i][df->nvars])/xy->ptr.pp_double[i][df->nvars], _state);
                relcnt = relcnt+1;
            }
        }
    }
    if( relcnt>0 )
    {
        result = result/relcnt;
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Copying of DecisionForest strucure

INPUT PARAMETERS:
    DF1 -   original

OUTPUT PARAMETERS:
    DF2 -   copy

  -- ALGLIB --
     Copyright 13.02.2009 by Bochkanov Sergey
*************************************************************************/
void dfcopy(decisionforest* df1, decisionforest* df2, ae_state *_state)
{

    _decisionforest_clear(df2);

    df2->nvars = df1->nvars;
    df2->nclasses = df1->nclasses;
    df2->ntrees = df1->ntrees;
    df2->bufsize = df1->bufsize;
    ae_vector_set_length(&df2->trees, df1->bufsize-1+1, _state);
    ae_v_move(&df2->trees.ptr.p_double[0], 1, &df1->trees.ptr.p_double[0], 1, ae_v_len(0,df1->bufsize-1));
}


/*************************************************************************
Serializer: allocation

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void dfalloc(ae_serializer* s, decisionforest* forest, ae_state *_state)
{


    ae_serializer_alloc_entry(s);
    ae_serializer_alloc_entry(s);
    ae_serializer_alloc_entry(s);
    ae_serializer_alloc_entry(s);
    ae_serializer_alloc_entry(s);
    ae_serializer_alloc_entry(s);
    allocrealarray(s, &forest->trees, forest->bufsize, _state);
}


/*************************************************************************
Serializer: serialization

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void dfserialize(ae_serializer* s,
     decisionforest* forest,
     ae_state *_state)
{


    ae_serializer_serialize_int(s, getrdfserializationcode(_state), _state);
    ae_serializer_serialize_int(s, dforest_dffirstversion, _state);
    ae_serializer_serialize_int(s, forest->nvars, _state);
    ae_serializer_serialize_int(s, forest->nclasses, _state);
    ae_serializer_serialize_int(s, forest->ntrees, _state);
    ae_serializer_serialize_int(s, forest->bufsize, _state);
    serializerealarray(s, &forest->trees, forest->bufsize, _state);
}


/*************************************************************************
Serializer: unserialization

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void dfunserialize(ae_serializer* s,
     decisionforest* forest,
     ae_state *_state)
{
    ae_int_t i0;
    ae_int_t i1;

    _decisionforest_clear(forest);

    
    /*
     * check correctness of header
     */
    ae_serializer_unserialize_int(s, &i0, _state);
    ae_assert(i0==getrdfserializationcode(_state), "DFUnserialize: stream header corrupted", _state);
    ae_serializer_unserialize_int(s, &i1, _state);
    ae_assert(i1==dforest_dffirstversion, "DFUnserialize: stream header corrupted", _state);
    
    /*
     * Unserialize data
     */
    ae_serializer_unserialize_int(s, &forest->nvars, _state);
    ae_serializer_unserialize_int(s, &forest->nclasses, _state);
    ae_serializer_unserialize_int(s, &forest->ntrees, _state);
    ae_serializer_unserialize_int(s, &forest->bufsize, _state);
    unserializerealarray(s, &forest->trees, _state);
}


/*************************************************************************
Classification error
*************************************************************************/
static ae_int_t dforest_dfclserror(decisionforest* df,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector x;
    ae_vector y;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t tmpi;
    ae_int_t result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);

    if( df->nclasses<=1 )
    {
        result = 0;
        ae_frame_leave(_state);
        return result;
    }
    ae_vector_set_length(&x, df->nvars-1+1, _state);
    ae_vector_set_length(&y, df->nclasses-1+1, _state);
    result = 0;
    for(i=0; i<=npoints-1; i++)
    {
        ae_v_move(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,df->nvars-1));
        dfprocess(df, &x, &y, _state);
        k = ae_round(xy->ptr.pp_double[i][df->nvars], _state);
        tmpi = 0;
        for(j=1; j<=df->nclasses-1; j++)
        {
            if( ae_fp_greater(y.ptr.p_double[j],y.ptr.p_double[tmpi]) )
            {
                tmpi = j;
            }
        }
        if( tmpi!=k )
        {
            result = result+1;
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Internal subroutine for processing one decision tree starting at Offs
*************************************************************************/
static void dforest_dfprocessinternal(decisionforest* df,
     ae_int_t offs,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{
    ae_int_t k;
    ae_int_t idx;


    
    /*
     * Set pointer to the root
     */
    k = offs+1;
    
    /*
     * Navigate through the tree
     */
    for(;;)
    {
        if( ae_fp_eq(df->trees.ptr.p_double[k],(double)(-1)) )
        {
            if( df->nclasses==1 )
            {
                y->ptr.p_double[0] = y->ptr.p_double[0]+df->trees.ptr.p_double[k+1];
            }
            else
            {
                idx = ae_round(df->trees.ptr.p_double[k+1], _state);
                y->ptr.p_double[idx] = y->ptr.p_double[idx]+1;
            }
            break;
        }
        if( ae_fp_less(x->ptr.p_double[ae_round(df->trees.ptr.p_double[k], _state)],df->trees.ptr.p_double[k+1]) )
        {
            k = k+dforest_innernodewidth;
        }
        else
        {
            k = offs+ae_round(df->trees.ptr.p_double[k+2], _state);
        }
    }
}


/*************************************************************************
Builds one decision tree. Just a wrapper for the DFBuildTreeRec.
*************************************************************************/
static void dforest_dfbuildtree(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t nclasses,
     ae_int_t nfeatures,
     ae_int_t nvarsinpool,
     ae_int_t flags,
     dfinternalbuffers* bufs,
     hqrndstate* rs,
     ae_state *_state)
{
    ae_int_t numprocessed;
    ae_int_t i;


    ae_assert(npoints>0, "Assertion failed", _state);
    
    /*
     * Prepare IdxBuf. It stores indices of the training set elements.
     * When training set is being split, contents of IdxBuf is
     * correspondingly reordered so we can know which elements belong
     * to which branch of decision tree.
     */
    for(i=0; i<=npoints-1; i++)
    {
        bufs->idxbuf.ptr.p_int[i] = i;
    }
    
    /*
     * Recursive procedure
     */
    numprocessed = 1;
    dforest_dfbuildtreerec(xy, npoints, nvars, nclasses, nfeatures, nvarsinpool, flags, &numprocessed, 0, npoints-1, bufs, rs, _state);
    bufs->treebuf.ptr.p_double[0] = (double)(numprocessed);
}


/*************************************************************************
Builds one decision tree (internal recursive subroutine)

Parameters:
    TreeBuf     -   large enough array, at least TreeSize
    IdxBuf      -   at least NPoints elements
    TmpBufR     -   at least NPoints
    TmpBufR2    -   at least NPoints
    TmpBufI     -   at least NPoints
    TmpBufI2    -   at least NPoints+1
*************************************************************************/
static void dforest_dfbuildtreerec(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t nclasses,
     ae_int_t nfeatures,
     ae_int_t nvarsinpool,
     ae_int_t flags,
     ae_int_t* numprocessed,
     ae_int_t idx1,
     ae_int_t idx2,
     dfinternalbuffers* bufs,
     hqrndstate* rs,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_bool bflag;
    ae_int_t i1;
    ae_int_t i2;
    ae_int_t info;
    double sl;
    double sr;
    double w;
    ae_int_t idxbest;
    double ebest;
    double tbest;
    ae_int_t varcur;
    double s;
    double v;
    double v1;
    double v2;
    double threshold;
    ae_int_t oldnp;
    double currms;
    ae_bool useevs;


    
    /*
     * these initializers are not really necessary,
     * but without them compiler complains about uninitialized locals
     */
    tbest = (double)(0);
    
    /*
     * Prepare
     */
    ae_assert(npoints>0, "Assertion failed", _state);
    ae_assert(idx2>=idx1, "Assertion failed", _state);
    useevs = flags/dforest_dfuseevs%2!=0;
    
    /*
     * Leaf node
     */
    if( idx2==idx1 )
    {
        bufs->treebuf.ptr.p_double[*numprocessed] = (double)(-1);
        bufs->treebuf.ptr.p_double[*numprocessed+1] = xy->ptr.pp_double[bufs->idxbuf.ptr.p_int[idx1]][nvars];
        *numprocessed = *numprocessed+dforest_leafnodewidth;
        return;
    }
    
    /*
     * Non-leaf node.
     * Select random variable, prepare split:
     * 1. prepare default solution - no splitting, class at random
     * 2. investigate possible splits, compare with default/best
     */
    idxbest = -1;
    if( nclasses>1 )
    {
        
        /*
         * default solution for classification
         */
        for(i=0; i<=nclasses-1; i++)
        {
            bufs->classibuf.ptr.p_int[i] = 0;
        }
        s = (double)(idx2-idx1+1);
        for(i=idx1; i<=idx2; i++)
        {
            j = ae_round(xy->ptr.pp_double[bufs->idxbuf.ptr.p_int[i]][nvars], _state);
            bufs->classibuf.ptr.p_int[j] = bufs->classibuf.ptr.p_int[j]+1;
        }
        ebest = (double)(0);
        for(i=0; i<=nclasses-1; i++)
        {
            ebest = ebest+bufs->classibuf.ptr.p_int[i]*ae_sqr(1-bufs->classibuf.ptr.p_int[i]/s, _state)+(s-bufs->classibuf.ptr.p_int[i])*ae_sqr(bufs->classibuf.ptr.p_int[i]/s, _state);
        }
        ebest = ae_sqrt(ebest/(nclasses*(idx2-idx1+1)), _state);
    }
    else
    {
        
        /*
         * default solution for regression
         */
        v = (double)(0);
        for(i=idx1; i<=idx2; i++)
        {
            v = v+xy->ptr.pp_double[bufs->idxbuf.ptr.p_int[i]][nvars];
        }
        v = v/(idx2-idx1+1);
        ebest = (double)(0);
        for(i=idx1; i<=idx2; i++)
        {
            ebest = ebest+ae_sqr(xy->ptr.pp_double[bufs->idxbuf.ptr.p_int[i]][nvars]-v, _state);
        }
        ebest = ae_sqrt(ebest/(idx2-idx1+1), _state);
    }
    i = 0;
    while(i<=ae_minint(nfeatures, nvarsinpool, _state)-1)
    {
        
        /*
         * select variables from pool
         */
        j = i+hqrnduniformi(rs, nvarsinpool-i, _state);
        k = bufs->varpool.ptr.p_int[i];
        bufs->varpool.ptr.p_int[i] = bufs->varpool.ptr.p_int[j];
        bufs->varpool.ptr.p_int[j] = k;
        varcur = bufs->varpool.ptr.p_int[i];
        
        /*
         * load variable values to working array
         *
         * apply EVS preprocessing: if all variable values are same,
         * variable is excluded from pool.
         *
         * This is necessary for binary pre-splits (see later) to work.
         */
        for(j=idx1; j<=idx2; j++)
        {
            bufs->tmpbufr.ptr.p_double[j-idx1] = xy->ptr.pp_double[bufs->idxbuf.ptr.p_int[j]][varcur];
        }
        if( useevs )
        {
            bflag = ae_false;
            v = bufs->tmpbufr.ptr.p_double[0];
            for(j=0; j<=idx2-idx1; j++)
            {
                if( ae_fp_neq(bufs->tmpbufr.ptr.p_double[j],v) )
                {
                    bflag = ae_true;
                    break;
                }
            }
            if( !bflag )
            {
                
                /*
                 * exclude variable from pool,
                 * go to the next iteration.
                 * I is not increased.
                 */
                k = bufs->varpool.ptr.p_int[i];
                bufs->varpool.ptr.p_int[i] = bufs->varpool.ptr.p_int[nvarsinpool-1];
                bufs->varpool.ptr.p_int[nvarsinpool-1] = k;
                nvarsinpool = nvarsinpool-1;
                continue;
            }
        }
        
        /*
         * load labels to working array
         */
        if( nclasses>1 )
        {
            for(j=idx1; j<=idx2; j++)
            {
                bufs->tmpbufi.ptr.p_int[j-idx1] = ae_round(xy->ptr.pp_double[bufs->idxbuf.ptr.p_int[j]][nvars], _state);
            }
        }
        else
        {
            for(j=idx1; j<=idx2; j++)
            {
                bufs->tmpbufr2.ptr.p_double[j-idx1] = xy->ptr.pp_double[bufs->idxbuf.ptr.p_int[j]][nvars];
            }
        }
        
        /*
         * calculate split
         */
        if( useevs&&bufs->evsbin.ptr.p_bool[varcur] )
        {
            
            /*
             * Pre-calculated splits for binary variables.
             * Threshold is already known, just calculate RMS error
             */
            threshold = bufs->evssplits.ptr.p_double[varcur];
            if( nclasses>1 )
            {
                
                /*
                 * classification-specific code
                 */
                for(j=0; j<=2*nclasses-1; j++)
                {
                    bufs->classibuf.ptr.p_int[j] = 0;
                }
                sl = (double)(0);
                sr = (double)(0);
                for(j=0; j<=idx2-idx1; j++)
                {
                    k = bufs->tmpbufi.ptr.p_int[j];
                    if( ae_fp_less(bufs->tmpbufr.ptr.p_double[j],threshold) )
                    {
                        bufs->classibuf.ptr.p_int[k] = bufs->classibuf.ptr.p_int[k]+1;
                        sl = sl+1;
                    }
                    else
                    {
                        bufs->classibuf.ptr.p_int[k+nclasses] = bufs->classibuf.ptr.p_int[k+nclasses]+1;
                        sr = sr+1;
                    }
                }
                ae_assert(ae_fp_neq(sl,(double)(0))&&ae_fp_neq(sr,(double)(0)), "DFBuildTreeRec: something strange!", _state);
                currms = (double)(0);
                for(j=0; j<=nclasses-1; j++)
                {
                    w = (double)(bufs->classibuf.ptr.p_int[j]);
                    currms = currms+w*ae_sqr(w/sl-1, _state);
                    currms = currms+(sl-w)*ae_sqr(w/sl, _state);
                    w = (double)(bufs->classibuf.ptr.p_int[nclasses+j]);
                    currms = currms+w*ae_sqr(w/sr-1, _state);
                    currms = currms+(sr-w)*ae_sqr(w/sr, _state);
                }
                currms = ae_sqrt(currms/(nclasses*(idx2-idx1+1)), _state);
            }
            else
            {
                
                /*
                 * regression-specific code
                 */
                sl = (double)(0);
                sr = (double)(0);
                v1 = (double)(0);
                v2 = (double)(0);
                for(j=0; j<=idx2-idx1; j++)
                {
                    if( ae_fp_less(bufs->tmpbufr.ptr.p_double[j],threshold) )
                    {
                        v1 = v1+bufs->tmpbufr2.ptr.p_double[j];
                        sl = sl+1;
                    }
                    else
                    {
                        v2 = v2+bufs->tmpbufr2.ptr.p_double[j];
                        sr = sr+1;
                    }
                }
                ae_assert(ae_fp_neq(sl,(double)(0))&&ae_fp_neq(sr,(double)(0)), "DFBuildTreeRec: something strange!", _state);
                v1 = v1/sl;
                v2 = v2/sr;
                currms = (double)(0);
                for(j=0; j<=idx2-idx1; j++)
                {
                    if( ae_fp_less(bufs->tmpbufr.ptr.p_double[j],threshold) )
                    {
                        currms = currms+ae_sqr(v1-bufs->tmpbufr2.ptr.p_double[j], _state);
                    }
                    else
                    {
                        currms = currms+ae_sqr(v2-bufs->tmpbufr2.ptr.p_double[j], _state);
                    }
                }
                currms = ae_sqrt(currms/(idx2-idx1+1), _state);
            }
            info = 1;
        }
        else
        {
            
            /*
             * Generic splits
             */
            if( nclasses>1 )
            {
                dforest_dfsplitc(&bufs->tmpbufr, &bufs->tmpbufi, &bufs->classibuf, idx2-idx1+1, nclasses, dforest_dfusestrongsplits, &info, &threshold, &currms, &bufs->sortrbuf, &bufs->sortibuf, _state);
            }
            else
            {
                dforest_dfsplitr(&bufs->tmpbufr, &bufs->tmpbufr2, idx2-idx1+1, dforest_dfusestrongsplits, &info, &threshold, &currms, &bufs->sortrbuf, &bufs->sortrbuf2, _state);
            }
        }
        if( info>0 )
        {
            if( ae_fp_less_eq(currms,ebest) )
            {
                ebest = currms;
                idxbest = varcur;
                tbest = threshold;
            }
        }
        
        /*
         * Next iteration
         */
        i = i+1;
    }
    
    /*
     * to split or not to split
     */
    if( idxbest<0 )
    {
        
        /*
         * All values are same, cannot split.
         */
        bufs->treebuf.ptr.p_double[*numprocessed] = (double)(-1);
        if( nclasses>1 )
        {
            
            /*
             * Select random class label (randomness allows us to
             * approximate distribution of the classes)
             */
            bufs->treebuf.ptr.p_double[*numprocessed+1] = (double)(ae_round(xy->ptr.pp_double[bufs->idxbuf.ptr.p_int[idx1+hqrnduniformi(rs, idx2-idx1+1, _state)]][nvars], _state));
        }
        else
        {
            
            /*
             * Select average (for regression task).
             */
            v = (double)(0);
            for(i=idx1; i<=idx2; i++)
            {
                v = v+xy->ptr.pp_double[bufs->idxbuf.ptr.p_int[i]][nvars]/(idx2-idx1+1);
            }
            bufs->treebuf.ptr.p_double[*numprocessed+1] = v;
        }
        *numprocessed = *numprocessed+dforest_leafnodewidth;
    }
    else
    {
        
        /*
         * we can split
         */
        bufs->treebuf.ptr.p_double[*numprocessed] = (double)(idxbest);
        bufs->treebuf.ptr.p_double[*numprocessed+1] = tbest;
        i1 = idx1;
        i2 = idx2;
        while(i1<=i2)
        {
            
            /*
             * Reorder indices so that left partition is in [Idx1..I1-1],
             * and right partition is in [I2+1..Idx2]
             */
            if( ae_fp_less(xy->ptr.pp_double[bufs->idxbuf.ptr.p_int[i1]][idxbest],tbest) )
            {
                i1 = i1+1;
                continue;
            }
            if( ae_fp_greater_eq(xy->ptr.pp_double[bufs->idxbuf.ptr.p_int[i2]][idxbest],tbest) )
            {
                i2 = i2-1;
                continue;
            }
            j = bufs->idxbuf.ptr.p_int[i1];
            bufs->idxbuf.ptr.p_int[i1] = bufs->idxbuf.ptr.p_int[i2];
            bufs->idxbuf.ptr.p_int[i2] = j;
            i1 = i1+1;
            i2 = i2-1;
        }
        oldnp = *numprocessed;
        *numprocessed = *numprocessed+dforest_innernodewidth;
        dforest_dfbuildtreerec(xy, npoints, nvars, nclasses, nfeatures, nvarsinpool, flags, numprocessed, idx1, i1-1, bufs, rs, _state);
        bufs->treebuf.ptr.p_double[oldnp+2] = (double)(*numprocessed);
        dforest_dfbuildtreerec(xy, npoints, nvars, nclasses, nfeatures, nvarsinpool, flags, numprocessed, i2+1, idx2, bufs, rs, _state);
    }
}


/*************************************************************************
Makes split on attribute
*************************************************************************/
static void dforest_dfsplitc(/* Real    */ ae_vector* x,
     /* Integer */ ae_vector* c,
     /* Integer */ ae_vector* cntbuf,
     ae_int_t n,
     ae_int_t nc,
     ae_int_t flags,
     ae_int_t* info,
     double* threshold,
     double* e,
     /* Real    */ ae_vector* sortrbuf,
     /* Integer */ ae_vector* sortibuf,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t neq;
    ae_int_t nless;
    ae_int_t ngreater;
    ae_int_t q;
    ae_int_t qmin;
    ae_int_t qmax;
    ae_int_t qcnt;
    double cursplit;
    ae_int_t nleft;
    double v;
    double cure;
    double w;
    double sl;
    double sr;

    *info = 0;
    *threshold = 0;
    *e = 0;

    tagsortfasti(x, c, sortrbuf, sortibuf, n, _state);
    *e = ae_maxrealnumber;
    *threshold = 0.5*(x->ptr.p_double[0]+x->ptr.p_double[n-1]);
    *info = -3;
    if( flags/dforest_dfusestrongsplits%2==0 )
    {
        
        /*
         * weak splits, split at half
         */
        qcnt = 2;
        qmin = 1;
        qmax = 1;
    }
    else
    {
        
        /*
         * strong splits: choose best quartile
         */
        qcnt = 4;
        qmin = 1;
        qmax = 3;
    }
    for(q=qmin; q<=qmax; q++)
    {
        cursplit = x->ptr.p_double[n*q/qcnt];
        neq = 0;
        nless = 0;
        ngreater = 0;
        for(i=0; i<=n-1; i++)
        {
            if( ae_fp_less(x->ptr.p_double[i],cursplit) )
            {
                nless = nless+1;
            }
            if( ae_fp_eq(x->ptr.p_double[i],cursplit) )
            {
                neq = neq+1;
            }
            if( ae_fp_greater(x->ptr.p_double[i],cursplit) )
            {
                ngreater = ngreater+1;
            }
        }
        ae_assert(neq!=0, "DFSplitR: NEq=0, something strange!!!", _state);
        if( nless!=0||ngreater!=0 )
        {
            
            /*
             * set threshold between two partitions, with
             * some tweaking to avoid problems with floating point
             * arithmetics.
             *
             * The problem is that when you calculates C = 0.5*(A+B) there
             * can be no C which lies strictly between A and B (for example,
             * there is no floating point number which is
             * greater than 1 and less than 1+eps). In such situations
             * we choose right side as theshold (remember that
             * points which lie on threshold falls to the right side).
             */
            if( nless<ngreater )
            {
                cursplit = 0.5*(x->ptr.p_double[nless+neq-1]+x->ptr.p_double[nless+neq]);
                nleft = nless+neq;
                if( ae_fp_less_eq(cursplit,x->ptr.p_double[nless+neq-1]) )
                {
                    cursplit = x->ptr.p_double[nless+neq];
                }
            }
            else
            {
                cursplit = 0.5*(x->ptr.p_double[nless-1]+x->ptr.p_double[nless]);
                nleft = nless;
                if( ae_fp_less_eq(cursplit,x->ptr.p_double[nless-1]) )
                {
                    cursplit = x->ptr.p_double[nless];
                }
            }
            *info = 1;
            cure = (double)(0);
            for(i=0; i<=2*nc-1; i++)
            {
                cntbuf->ptr.p_int[i] = 0;
            }
            for(i=0; i<=nleft-1; i++)
            {
                cntbuf->ptr.p_int[c->ptr.p_int[i]] = cntbuf->ptr.p_int[c->ptr.p_int[i]]+1;
            }
            for(i=nleft; i<=n-1; i++)
            {
                cntbuf->ptr.p_int[nc+c->ptr.p_int[i]] = cntbuf->ptr.p_int[nc+c->ptr.p_int[i]]+1;
            }
            sl = (double)(nleft);
            sr = (double)(n-nleft);
            v = (double)(0);
            for(i=0; i<=nc-1; i++)
            {
                w = (double)(cntbuf->ptr.p_int[i]);
                v = v+w*ae_sqr(w/sl-1, _state);
                v = v+(sl-w)*ae_sqr(w/sl, _state);
                w = (double)(cntbuf->ptr.p_int[nc+i]);
                v = v+w*ae_sqr(w/sr-1, _state);
                v = v+(sr-w)*ae_sqr(w/sr, _state);
            }
            cure = ae_sqrt(v/(nc*n), _state);
            if( ae_fp_less(cure,*e) )
            {
                *threshold = cursplit;
                *e = cure;
            }
        }
    }
}


/*************************************************************************
Makes split on attribute
*************************************************************************/
static void dforest_dfsplitr(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t flags,
     ae_int_t* info,
     double* threshold,
     double* e,
     /* Real    */ ae_vector* sortrbuf,
     /* Real    */ ae_vector* sortrbuf2,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t neq;
    ae_int_t nless;
    ae_int_t ngreater;
    ae_int_t q;
    ae_int_t qmin;
    ae_int_t qmax;
    ae_int_t qcnt;
    double cursplit;
    ae_int_t nleft;
    double v;
    double cure;

    *info = 0;
    *threshold = 0;
    *e = 0;

    tagsortfastr(x, y, sortrbuf, sortrbuf2, n, _state);
    *e = ae_maxrealnumber;
    *threshold = 0.5*(x->ptr.p_double[0]+x->ptr.p_double[n-1]);
    *info = -3;
    if( flags/dforest_dfusestrongsplits%2==0 )
    {
        
        /*
         * weak splits, split at half
         */
        qcnt = 2;
        qmin = 1;
        qmax = 1;
    }
    else
    {
        
        /*
         * strong splits: choose best quartile
         */
        qcnt = 4;
        qmin = 1;
        qmax = 3;
    }
    for(q=qmin; q<=qmax; q++)
    {
        cursplit = x->ptr.p_double[n*q/qcnt];
        neq = 0;
        nless = 0;
        ngreater = 0;
        for(i=0; i<=n-1; i++)
        {
            if( ae_fp_less(x->ptr.p_double[i],cursplit) )
            {
                nless = nless+1;
            }
            if( ae_fp_eq(x->ptr.p_double[i],cursplit) )
            {
                neq = neq+1;
            }
            if( ae_fp_greater(x->ptr.p_double[i],cursplit) )
            {
                ngreater = ngreater+1;
            }
        }
        ae_assert(neq!=0, "DFSplitR: NEq=0, something strange!!!", _state);
        if( nless!=0||ngreater!=0 )
        {
            
            /*
             * set threshold between two partitions, with
             * some tweaking to avoid problems with floating point
             * arithmetics.
             *
             * The problem is that when you calculates C = 0.5*(A+B) there
             * can be no C which lies strictly between A and B (for example,
             * there is no floating point number which is
             * greater than 1 and less than 1+eps). In such situations
             * we choose right side as theshold (remember that
             * points which lie on threshold falls to the right side).
             */
            if( nless<ngreater )
            {
                cursplit = 0.5*(x->ptr.p_double[nless+neq-1]+x->ptr.p_double[nless+neq]);
                nleft = nless+neq;
                if( ae_fp_less_eq(cursplit,x->ptr.p_double[nless+neq-1]) )
                {
                    cursplit = x->ptr.p_double[nless+neq];
                }
            }
            else
            {
                cursplit = 0.5*(x->ptr.p_double[nless-1]+x->ptr.p_double[nless]);
                nleft = nless;
                if( ae_fp_less_eq(cursplit,x->ptr.p_double[nless-1]) )
                {
                    cursplit = x->ptr.p_double[nless];
                }
            }
            *info = 1;
            cure = (double)(0);
            v = (double)(0);
            for(i=0; i<=nleft-1; i++)
            {
                v = v+y->ptr.p_double[i];
            }
            v = v/nleft;
            for(i=0; i<=nleft-1; i++)
            {
                cure = cure+ae_sqr(y->ptr.p_double[i]-v, _state);
            }
            v = (double)(0);
            for(i=nleft; i<=n-1; i++)
            {
                v = v+y->ptr.p_double[i];
            }
            v = v/(n-nleft);
            for(i=nleft; i<=n-1; i++)
            {
                cure = cure+ae_sqr(y->ptr.p_double[i]-v, _state);
            }
            cure = ae_sqrt(cure/n, _state);
            if( ae_fp_less(cure,*e) )
            {
                *threshold = cursplit;
                *e = cure;
            }
        }
    }
}


void _decisionforest_init(void* _p, ae_state *_state)
{
    decisionforest *p = (decisionforest*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->trees, 0, DT_REAL, _state);
}


void _decisionforest_init_copy(void* _dst, void* _src, ae_state *_state)
{
    decisionforest *dst = (decisionforest*)_dst;
    decisionforest *src = (decisionforest*)_src;
    dst->nvars = src->nvars;
    dst->nclasses = src->nclasses;
    dst->ntrees = src->ntrees;
    dst->bufsize = src->bufsize;
    ae_vector_init_copy(&dst->trees, &src->trees, _state);
}


void _decisionforest_clear(void* _p)
{
    decisionforest *p = (decisionforest*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->trees);
}


void _decisionforest_destroy(void* _p)
{
    decisionforest *p = (decisionforest*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->trees);
}


void _dfreport_init(void* _p, ae_state *_state)
{
    dfreport *p = (dfreport*)_p;
    ae_touch_ptr((void*)p);
}


void _dfreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    dfreport *dst = (dfreport*)_dst;
    dfreport *src = (dfreport*)_src;
    dst->relclserror = src->relclserror;
    dst->avgce = src->avgce;
    dst->rmserror = src->rmserror;
    dst->avgerror = src->avgerror;
    dst->avgrelerror = src->avgrelerror;
    dst->oobrelclserror = src->oobrelclserror;
    dst->oobavgce = src->oobavgce;
    dst->oobrmserror = src->oobrmserror;
    dst->oobavgerror = src->oobavgerror;
    dst->oobavgrelerror = src->oobavgrelerror;
}


void _dfreport_clear(void* _p)
{
    dfreport *p = (dfreport*)_p;
    ae_touch_ptr((void*)p);
}


void _dfreport_destroy(void* _p)
{
    dfreport *p = (dfreport*)_p;
    ae_touch_ptr((void*)p);
}


void _dfinternalbuffers_init(void* _p, ae_state *_state)
{
    dfinternalbuffers *p = (dfinternalbuffers*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->treebuf, 0, DT_REAL, _state);
    ae_vector_init(&p->idxbuf, 0, DT_INT, _state);
    ae_vector_init(&p->tmpbufr, 0, DT_REAL, _state);
    ae_vector_init(&p->tmpbufr2, 0, DT_REAL, _state);
    ae_vector_init(&p->tmpbufi, 0, DT_INT, _state);
    ae_vector_init(&p->classibuf, 0, DT_INT, _state);
    ae_vector_init(&p->sortrbuf, 0, DT_REAL, _state);
    ae_vector_init(&p->sortrbuf2, 0, DT_REAL, _state);
    ae_vector_init(&p->sortibuf, 0, DT_INT, _state);
    ae_vector_init(&p->varpool, 0, DT_INT, _state);
    ae_vector_init(&p->evsbin, 0, DT_BOOL, _state);
    ae_vector_init(&p->evssplits, 0, DT_REAL, _state);
}


void _dfinternalbuffers_init_copy(void* _dst, void* _src, ae_state *_state)
{
    dfinternalbuffers *dst = (dfinternalbuffers*)_dst;
    dfinternalbuffers *src = (dfinternalbuffers*)_src;
    ae_vector_init_copy(&dst->treebuf, &src->treebuf, _state);
    ae_vector_init_copy(&dst->idxbuf, &src->idxbuf, _state);
    ae_vector_init_copy(&dst->tmpbufr, &src->tmpbufr, _state);
    ae_vector_init_copy(&dst->tmpbufr2, &src->tmpbufr2, _state);
    ae_vector_init_copy(&dst->tmpbufi, &src->tmpbufi, _state);
    ae_vector_init_copy(&dst->classibuf, &src->classibuf, _state);
    ae_vector_init_copy(&dst->sortrbuf, &src->sortrbuf, _state);
    ae_vector_init_copy(&dst->sortrbuf2, &src->sortrbuf2, _state);
    ae_vector_init_copy(&dst->sortibuf, &src->sortibuf, _state);
    ae_vector_init_copy(&dst->varpool, &src->varpool, _state);
    ae_vector_init_copy(&dst->evsbin, &src->evsbin, _state);
    ae_vector_init_copy(&dst->evssplits, &src->evssplits, _state);
}


void _dfinternalbuffers_clear(void* _p)
{
    dfinternalbuffers *p = (dfinternalbuffers*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->treebuf);
    ae_vector_clear(&p->idxbuf);
    ae_vector_clear(&p->tmpbufr);
    ae_vector_clear(&p->tmpbufr2);
    ae_vector_clear(&p->tmpbufi);
    ae_vector_clear(&p->classibuf);
    ae_vector_clear(&p->sortrbuf);
    ae_vector_clear(&p->sortrbuf2);
    ae_vector_clear(&p->sortibuf);
    ae_vector_clear(&p->varpool);
    ae_vector_clear(&p->evsbin);
    ae_vector_clear(&p->evssplits);
}


void _dfinternalbuffers_destroy(void* _p)
{
    dfinternalbuffers *p = (dfinternalbuffers*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->treebuf);
    ae_vector_destroy(&p->idxbuf);
    ae_vector_destroy(&p->tmpbufr);
    ae_vector_destroy(&p->tmpbufr2);
    ae_vector_destroy(&p->tmpbufi);
    ae_vector_destroy(&p->classibuf);
    ae_vector_destroy(&p->sortrbuf);
    ae_vector_destroy(&p->sortrbuf2);
    ae_vector_destroy(&p->sortibuf);
    ae_vector_destroy(&p->varpool);
    ae_vector_destroy(&p->evsbin);
    ae_vector_destroy(&p->evssplits);
}


/*$ End $*/
