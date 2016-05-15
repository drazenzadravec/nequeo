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
#include "logit.h"


/*$ Declarations $*/
static double logit_xtol = 100*ae_machineepsilon;
static double logit_ftol = 0.0001;
static double logit_gtol = 0.3;
static ae_int_t logit_maxfev = 20;
static double logit_stpmin = 1.0E-2;
static double logit_stpmax = 1.0E5;
static ae_int_t logit_logitvnum = 6;
static void logit_mnliexp(/* Real    */ ae_vector* w,
     /* Real    */ ae_vector* x,
     ae_state *_state);
static void logit_mnlallerrors(logitmodel* lm,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     double* relcls,
     double* avgce,
     double* rms,
     double* avg,
     double* avgrel,
     ae_state *_state);
static void logit_mnlmcsrch(ae_int_t n,
     /* Real    */ ae_vector* x,
     double* f,
     /* Real    */ ae_vector* g,
     /* Real    */ ae_vector* s,
     double* stp,
     ae_int_t* info,
     ae_int_t* nfev,
     /* Real    */ ae_vector* wa,
     logitmcstate* state,
     ae_int_t* stage,
     ae_state *_state);
static void logit_mnlmcstep(double* stx,
     double* fx,
     double* dx,
     double* sty,
     double* fy,
     double* dy,
     double* stp,
     double fp,
     double dp,
     ae_bool* brackt,
     double stmin,
     double stmax,
     ae_int_t* info,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
This subroutine trains logit model.

INPUT PARAMETERS:
    XY          -   training set, array[0..NPoints-1,0..NVars]
                    First NVars columns store values of independent
                    variables, next column stores number of class (from 0
                    to NClasses-1) which dataset element belongs to. Fractional
                    values are rounded to nearest integer.
    NPoints     -   training set size, NPoints>=1
    NVars       -   number of independent variables, NVars>=1
    NClasses    -   number of classes, NClasses>=2

OUTPUT PARAMETERS:
    Info        -   return code:
                    * -2, if there is a point with class number
                          outside of [0..NClasses-1].
                    * -1, if incorrect parameters was passed
                          (NPoints<NVars+2, NVars<1, NClasses<2).
                    *  1, if task has been solved
    LM          -   model built
    Rep         -   training report

  -- ALGLIB --
     Copyright 10.09.2008 by Bochkanov Sergey
*************************************************************************/
void mnltrainh(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t nclasses,
     ae_int_t* info,
     logitmodel* lm,
     mnlreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t ssize;
    ae_bool allsame;
    ae_int_t offs;
    double decay;
    double v;
    double s;
    multilayerperceptron network;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    double e;
    ae_vector g;
    ae_matrix h;
    ae_bool spd;
    ae_vector x;
    ae_vector y;
    ae_vector wbase;
    double wstep;
    ae_vector wdir;
    ae_vector work;
    ae_int_t mcstage;
    logitmcstate mcstate;
    ae_int_t mcinfo;
    ae_int_t mcnfev;
    ae_int_t solverinfo;
    densesolverreport solverrep;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _logitmodel_clear(lm);
    _mnlreport_clear(rep);
    _multilayerperceptron_init(&network, _state);
    ae_vector_init(&g, 0, DT_REAL, _state);
    ae_matrix_init(&h, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&wbase, 0, DT_REAL, _state);
    ae_vector_init(&wdir, 0, DT_REAL, _state);
    ae_vector_init(&work, 0, DT_REAL, _state);
    _logitmcstate_init(&mcstate, _state);
    _densesolverreport_init(&solverrep, _state);

    decay = 0.001;
    
    /*
     * Test for inputs
     */
    if( (npoints<nvars+2||nvars<1)||nclasses<2 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=npoints-1; i++)
    {
        if( ae_round(xy->ptr.pp_double[i][nvars], _state)<0||ae_round(xy->ptr.pp_double[i][nvars], _state)>=nclasses )
        {
            *info = -2;
            ae_frame_leave(_state);
            return;
        }
    }
    *info = 1;
    
    /*
     * Initialize data
     */
    rep->ngrad = 0;
    rep->nhess = 0;
    
    /*
     * Allocate array
     */
    offs = 5;
    ssize = 5+(nvars+1)*(nclasses-1)+nclasses;
    ae_vector_set_length(&lm->w, ssize-1+1, _state);
    lm->w.ptr.p_double[0] = (double)(ssize);
    lm->w.ptr.p_double[1] = (double)(logit_logitvnum);
    lm->w.ptr.p_double[2] = (double)(nvars);
    lm->w.ptr.p_double[3] = (double)(nclasses);
    lm->w.ptr.p_double[4] = (double)(offs);
    
    /*
     * Degenerate case: all outputs are equal
     */
    allsame = ae_true;
    for(i=1; i<=npoints-1; i++)
    {
        if( ae_round(xy->ptr.pp_double[i][nvars], _state)!=ae_round(xy->ptr.pp_double[i-1][nvars], _state) )
        {
            allsame = ae_false;
        }
    }
    if( allsame )
    {
        for(i=0; i<=(nvars+1)*(nclasses-1)-1; i++)
        {
            lm->w.ptr.p_double[offs+i] = (double)(0);
        }
        v = -2*ae_log(ae_minrealnumber, _state);
        k = ae_round(xy->ptr.pp_double[0][nvars], _state);
        if( k==nclasses-1 )
        {
            for(i=0; i<=nclasses-2; i++)
            {
                lm->w.ptr.p_double[offs+i*(nvars+1)+nvars] = -v;
            }
        }
        else
        {
            for(i=0; i<=nclasses-2; i++)
            {
                if( i==k )
                {
                    lm->w.ptr.p_double[offs+i*(nvars+1)+nvars] = v;
                }
                else
                {
                    lm->w.ptr.p_double[offs+i*(nvars+1)+nvars] = (double)(0);
                }
            }
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * General case.
     * Prepare task and network. Allocate space.
     */
    mlpcreatec0(nvars, nclasses, &network, _state);
    mlpinitpreprocessor(&network, xy, npoints, _state);
    mlpproperties(&network, &nin, &nout, &wcount, _state);
    for(i=0; i<=wcount-1; i++)
    {
        network.weights.ptr.p_double[i] = (2*ae_randomreal(_state)-1)/nvars;
    }
    ae_vector_set_length(&g, wcount-1+1, _state);
    ae_matrix_set_length(&h, wcount-1+1, wcount-1+1, _state);
    ae_vector_set_length(&wbase, wcount-1+1, _state);
    ae_vector_set_length(&wdir, wcount-1+1, _state);
    ae_vector_set_length(&work, wcount-1+1, _state);
    
    /*
     * First stage: optimize in gradient direction.
     */
    for(k=0; k<=wcount/3+10; k++)
    {
        
        /*
         * Calculate gradient in starting point
         */
        mlpgradnbatch(&network, xy, npoints, &e, &g, _state);
        v = ae_v_dotproduct(&network.weights.ptr.p_double[0], 1, &network.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        e = e+0.5*decay*v;
        ae_v_addd(&g.ptr.p_double[0], 1, &network.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1), decay);
        rep->ngrad = rep->ngrad+1;
        
        /*
         * Setup optimization scheme
         */
        ae_v_moveneg(&wdir.ptr.p_double[0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        v = ae_v_dotproduct(&wdir.ptr.p_double[0], 1, &wdir.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        wstep = ae_sqrt(v, _state);
        v = 1/ae_sqrt(v, _state);
        ae_v_muld(&wdir.ptr.p_double[0], 1, ae_v_len(0,wcount-1), v);
        mcstage = 0;
        logit_mnlmcsrch(wcount, &network.weights, &e, &g, &wdir, &wstep, &mcinfo, &mcnfev, &work, &mcstate, &mcstage, _state);
        while(mcstage!=0)
        {
            mlpgradnbatch(&network, xy, npoints, &e, &g, _state);
            v = ae_v_dotproduct(&network.weights.ptr.p_double[0], 1, &network.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            e = e+0.5*decay*v;
            ae_v_addd(&g.ptr.p_double[0], 1, &network.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1), decay);
            rep->ngrad = rep->ngrad+1;
            logit_mnlmcsrch(wcount, &network.weights, &e, &g, &wdir, &wstep, &mcinfo, &mcnfev, &work, &mcstate, &mcstage, _state);
        }
    }
    
    /*
     * Second stage: use Hessian when we are close to the minimum
     */
    for(;;)
    {
        
        /*
         * Calculate and update E/G/H
         */
        mlphessiannbatch(&network, xy, npoints, &e, &g, &h, _state);
        v = ae_v_dotproduct(&network.weights.ptr.p_double[0], 1, &network.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        e = e+0.5*decay*v;
        ae_v_addd(&g.ptr.p_double[0], 1, &network.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1), decay);
        for(k=0; k<=wcount-1; k++)
        {
            h.ptr.pp_double[k][k] = h.ptr.pp_double[k][k]+decay;
        }
        rep->nhess = rep->nhess+1;
        
        /*
         * Select step direction
         * NOTE: it is important to use lower-triangle Cholesky
         * factorization since it is much faster than higher-triangle version.
         */
        spd = spdmatrixcholesky(&h, wcount, ae_false, _state);
        spdmatrixcholeskysolve(&h, wcount, ae_false, &g, &solverinfo, &solverrep, &wdir, _state);
        spd = solverinfo>0;
        if( spd )
        {
            
            /*
             * H is positive definite.
             * Step in Newton direction.
             */
            ae_v_muld(&wdir.ptr.p_double[0], 1, ae_v_len(0,wcount-1), -1);
            spd = ae_true;
        }
        else
        {
            
            /*
             * H is indefinite.
             * Step in gradient direction.
             */
            ae_v_moveneg(&wdir.ptr.p_double[0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            spd = ae_false;
        }
        
        /*
         * Optimize in WDir direction
         */
        v = ae_v_dotproduct(&wdir.ptr.p_double[0], 1, &wdir.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        wstep = ae_sqrt(v, _state);
        v = 1/ae_sqrt(v, _state);
        ae_v_muld(&wdir.ptr.p_double[0], 1, ae_v_len(0,wcount-1), v);
        mcstage = 0;
        logit_mnlmcsrch(wcount, &network.weights, &e, &g, &wdir, &wstep, &mcinfo, &mcnfev, &work, &mcstate, &mcstage, _state);
        while(mcstage!=0)
        {
            mlpgradnbatch(&network, xy, npoints, &e, &g, _state);
            v = ae_v_dotproduct(&network.weights.ptr.p_double[0], 1, &network.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            e = e+0.5*decay*v;
            ae_v_addd(&g.ptr.p_double[0], 1, &network.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1), decay);
            rep->ngrad = rep->ngrad+1;
            logit_mnlmcsrch(wcount, &network.weights, &e, &g, &wdir, &wstep, &mcinfo, &mcnfev, &work, &mcstate, &mcstage, _state);
        }
        if( spd&&((mcinfo==2||mcinfo==4)||mcinfo==6) )
        {
            break;
        }
    }
    
    /*
     * Convert from NN format to MNL format
     */
    ae_v_move(&lm->w.ptr.p_double[offs], 1, &network.weights.ptr.p_double[0], 1, ae_v_len(offs,offs+wcount-1));
    for(k=0; k<=nvars-1; k++)
    {
        for(i=0; i<=nclasses-2; i++)
        {
            s = network.columnsigmas.ptr.p_double[k];
            if( ae_fp_eq(s,(double)(0)) )
            {
                s = (double)(1);
            }
            j = offs+(nvars+1)*i;
            v = lm->w.ptr.p_double[j+k];
            lm->w.ptr.p_double[j+k] = v/s;
            lm->w.ptr.p_double[j+nvars] = lm->w.ptr.p_double[j+nvars]+v*network.columnmeans.ptr.p_double[k]/s;
        }
    }
    for(k=0; k<=nclasses-2; k++)
    {
        lm->w.ptr.p_double[offs+(nvars+1)*k+nvars] = -lm->w.ptr.p_double[offs+(nvars+1)*k+nvars];
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Procesing

INPUT PARAMETERS:
    LM      -   logit model, passed by non-constant reference
                (some fields of structure are used as temporaries
                when calculating model output).
    X       -   input vector,  array[0..NVars-1].
    Y       -   (possibly) preallocated buffer; if size of Y is less than
                NClasses, it will be reallocated.If it is large enough, it
                is NOT reallocated, so we can save some time on reallocation.

OUTPUT PARAMETERS:
    Y       -   result, array[0..NClasses-1]
                Vector of posterior probabilities for classification task.

  -- ALGLIB --
     Copyright 10.09.2008 by Bochkanov Sergey
*************************************************************************/
void mnlprocess(logitmodel* lm,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{
    ae_int_t nvars;
    ae_int_t nclasses;
    ae_int_t offs;
    ae_int_t i;
    ae_int_t i1;
    double s;


    ae_assert(ae_fp_eq(lm->w.ptr.p_double[1],(double)(logit_logitvnum)), "MNLProcess: unexpected model version", _state);
    nvars = ae_round(lm->w.ptr.p_double[2], _state);
    nclasses = ae_round(lm->w.ptr.p_double[3], _state);
    offs = ae_round(lm->w.ptr.p_double[4], _state);
    logit_mnliexp(&lm->w, x, _state);
    s = (double)(0);
    i1 = offs+(nvars+1)*(nclasses-1);
    for(i=i1; i<=i1+nclasses-1; i++)
    {
        s = s+lm->w.ptr.p_double[i];
    }
    if( y->cnt<nclasses )
    {
        ae_vector_set_length(y, nclasses, _state);
    }
    for(i=0; i<=nclasses-1; i++)
    {
        y->ptr.p_double[i] = lm->w.ptr.p_double[i1+i]/s;
    }
}


/*************************************************************************
'interactive'  variant  of  MNLProcess  for  languages  like  Python which
support constructs like "Y = MNLProcess(LM,X)" and interactive mode of the
interpreter

This function allocates new array on each call,  so  it  is  significantly
slower than its 'non-interactive' counterpart, but it is  more  convenient
when you call it from command line.

  -- ALGLIB --
     Copyright 10.09.2008 by Bochkanov Sergey
*************************************************************************/
void mnlprocessi(logitmodel* lm,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{

    ae_vector_clear(y);

    mnlprocess(lm, x, y, _state);
}


/*************************************************************************
Unpacks coefficients of logit model. Logit model have form:

    P(class=i) = S(i) / (S(0) + S(1) + ... +S(M-1))
          S(i) = Exp(A[i,0]*X[0] + ... + A[i,N-1]*X[N-1] + A[i,N]), when i<M-1
        S(M-1) = 1

INPUT PARAMETERS:
    LM          -   logit model in ALGLIB format

OUTPUT PARAMETERS:
    V           -   coefficients, array[0..NClasses-2,0..NVars]
    NVars       -   number of independent variables
    NClasses    -   number of classes

  -- ALGLIB --
     Copyright 10.09.2008 by Bochkanov Sergey
*************************************************************************/
void mnlunpack(logitmodel* lm,
     /* Real    */ ae_matrix* a,
     ae_int_t* nvars,
     ae_int_t* nclasses,
     ae_state *_state)
{
    ae_int_t offs;
    ae_int_t i;

    ae_matrix_clear(a);
    *nvars = 0;
    *nclasses = 0;

    ae_assert(ae_fp_eq(lm->w.ptr.p_double[1],(double)(logit_logitvnum)), "MNLUnpack: unexpected model version", _state);
    *nvars = ae_round(lm->w.ptr.p_double[2], _state);
    *nclasses = ae_round(lm->w.ptr.p_double[3], _state);
    offs = ae_round(lm->w.ptr.p_double[4], _state);
    ae_matrix_set_length(a, *nclasses-2+1, *nvars+1, _state);
    for(i=0; i<=*nclasses-2; i++)
    {
        ae_v_move(&a->ptr.pp_double[i][0], 1, &lm->w.ptr.p_double[offs+i*(*nvars+1)], 1, ae_v_len(0,*nvars));
    }
}


/*************************************************************************
"Packs" coefficients and creates logit model in ALGLIB format (MNLUnpack
reversed).

INPUT PARAMETERS:
    A           -   model (see MNLUnpack)
    NVars       -   number of independent variables
    NClasses    -   number of classes

OUTPUT PARAMETERS:
    LM          -   logit model.

  -- ALGLIB --
     Copyright 10.09.2008 by Bochkanov Sergey
*************************************************************************/
void mnlpack(/* Real    */ ae_matrix* a,
     ae_int_t nvars,
     ae_int_t nclasses,
     logitmodel* lm,
     ae_state *_state)
{
    ae_int_t offs;
    ae_int_t i;
    ae_int_t ssize;

    _logitmodel_clear(lm);

    offs = 5;
    ssize = 5+(nvars+1)*(nclasses-1)+nclasses;
    ae_vector_set_length(&lm->w, ssize-1+1, _state);
    lm->w.ptr.p_double[0] = (double)(ssize);
    lm->w.ptr.p_double[1] = (double)(logit_logitvnum);
    lm->w.ptr.p_double[2] = (double)(nvars);
    lm->w.ptr.p_double[3] = (double)(nclasses);
    lm->w.ptr.p_double[4] = (double)(offs);
    for(i=0; i<=nclasses-2; i++)
    {
        ae_v_move(&lm->w.ptr.p_double[offs+i*(nvars+1)], 1, &a->ptr.pp_double[i][0], 1, ae_v_len(offs+i*(nvars+1),offs+i*(nvars+1)+nvars));
    }
}


/*************************************************************************
Copying of LogitModel strucure

INPUT PARAMETERS:
    LM1 -   original

OUTPUT PARAMETERS:
    LM2 -   copy

  -- ALGLIB --
     Copyright 15.03.2009 by Bochkanov Sergey
*************************************************************************/
void mnlcopy(logitmodel* lm1, logitmodel* lm2, ae_state *_state)
{
    ae_int_t k;

    _logitmodel_clear(lm2);

    k = ae_round(lm1->w.ptr.p_double[0], _state);
    ae_vector_set_length(&lm2->w, k-1+1, _state);
    ae_v_move(&lm2->w.ptr.p_double[0], 1, &lm1->w.ptr.p_double[0], 1, ae_v_len(0,k-1));
}


/*************************************************************************
Average cross-entropy (in bits per element) on the test set

INPUT PARAMETERS:
    LM      -   logit model
    XY      -   test set
    NPoints -   test set size

RESULT:
    CrossEntropy/(NPoints*ln(2)).

  -- ALGLIB --
     Copyright 10.09.2008 by Bochkanov Sergey
*************************************************************************/
double mnlavgce(logitmodel* lm,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t nvars;
    ae_int_t nclasses;
    ae_int_t i;
    ae_vector workx;
    ae_vector worky;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&workx, 0, DT_REAL, _state);
    ae_vector_init(&worky, 0, DT_REAL, _state);

    ae_assert(ae_fp_eq(lm->w.ptr.p_double[1],(double)(logit_logitvnum)), "MNLClsError: unexpected model version", _state);
    nvars = ae_round(lm->w.ptr.p_double[2], _state);
    nclasses = ae_round(lm->w.ptr.p_double[3], _state);
    ae_vector_set_length(&workx, nvars-1+1, _state);
    ae_vector_set_length(&worky, nclasses-1+1, _state);
    result = (double)(0);
    for(i=0; i<=npoints-1; i++)
    {
        ae_assert(ae_round(xy->ptr.pp_double[i][nvars], _state)>=0&&ae_round(xy->ptr.pp_double[i][nvars], _state)<nclasses, "MNLAvgCE: incorrect class number!", _state);
        
        /*
         * Process
         */
        ae_v_move(&workx.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
        mnlprocess(lm, &workx, &worky, _state);
        if( ae_fp_greater(worky.ptr.p_double[ae_round(xy->ptr.pp_double[i][nvars], _state)],(double)(0)) )
        {
            result = result-ae_log(worky.ptr.p_double[ae_round(xy->ptr.pp_double[i][nvars], _state)], _state);
        }
        else
        {
            result = result-ae_log(ae_minrealnumber, _state);
        }
    }
    result = result/(npoints*ae_log((double)(2), _state));
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Relative classification error on the test set

INPUT PARAMETERS:
    LM      -   logit model
    XY      -   test set
    NPoints -   test set size

RESULT:
    percent of incorrectly classified cases.

  -- ALGLIB --
     Copyright 10.09.2008 by Bochkanov Sergey
*************************************************************************/
double mnlrelclserror(logitmodel* lm,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double result;


    result = (double)mnlclserror(lm, xy, npoints, _state)/(double)npoints;
    return result;
}


/*************************************************************************
RMS error on the test set

INPUT PARAMETERS:
    LM      -   logit model
    XY      -   test set
    NPoints -   test set size

RESULT:
    root mean square error (error when estimating posterior probabilities).

  -- ALGLIB --
     Copyright 30.08.2008 by Bochkanov Sergey
*************************************************************************/
double mnlrmserror(logitmodel* lm,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double relcls;
    double avgce;
    double rms;
    double avg;
    double avgrel;
    double result;


    ae_assert(ae_round(lm->w.ptr.p_double[1], _state)==logit_logitvnum, "MNLRMSError: Incorrect MNL version!", _state);
    logit_mnlallerrors(lm, xy, npoints, &relcls, &avgce, &rms, &avg, &avgrel, _state);
    result = rms;
    return result;
}


/*************************************************************************
Average error on the test set

INPUT PARAMETERS:
    LM      -   logit model
    XY      -   test set
    NPoints -   test set size

RESULT:
    average error (error when estimating posterior probabilities).

  -- ALGLIB --
     Copyright 30.08.2008 by Bochkanov Sergey
*************************************************************************/
double mnlavgerror(logitmodel* lm,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double relcls;
    double avgce;
    double rms;
    double avg;
    double avgrel;
    double result;


    ae_assert(ae_round(lm->w.ptr.p_double[1], _state)==logit_logitvnum, "MNLRMSError: Incorrect MNL version!", _state);
    logit_mnlallerrors(lm, xy, npoints, &relcls, &avgce, &rms, &avg, &avgrel, _state);
    result = avg;
    return result;
}


/*************************************************************************
Average relative error on the test set

INPUT PARAMETERS:
    LM      -   logit model
    XY      -   test set
    NPoints -   test set size

RESULT:
    average relative error (error when estimating posterior probabilities).

  -- ALGLIB --
     Copyright 30.08.2008 by Bochkanov Sergey
*************************************************************************/
double mnlavgrelerror(logitmodel* lm,
     /* Real    */ ae_matrix* xy,
     ae_int_t ssize,
     ae_state *_state)
{
    double relcls;
    double avgce;
    double rms;
    double avg;
    double avgrel;
    double result;


    ae_assert(ae_round(lm->w.ptr.p_double[1], _state)==logit_logitvnum, "MNLRMSError: Incorrect MNL version!", _state);
    logit_mnlallerrors(lm, xy, ssize, &relcls, &avgce, &rms, &avg, &avgrel, _state);
    result = avgrel;
    return result;
}


/*************************************************************************
Classification error on test set = MNLRelClsError*NPoints

  -- ALGLIB --
     Copyright 10.09.2008 by Bochkanov Sergey
*************************************************************************/
ae_int_t mnlclserror(logitmodel* lm,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t nvars;
    ae_int_t nclasses;
    ae_int_t i;
    ae_int_t j;
    ae_vector workx;
    ae_vector worky;
    ae_int_t nmax;
    ae_int_t result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&workx, 0, DT_REAL, _state);
    ae_vector_init(&worky, 0, DT_REAL, _state);

    ae_assert(ae_fp_eq(lm->w.ptr.p_double[1],(double)(logit_logitvnum)), "MNLClsError: unexpected model version", _state);
    nvars = ae_round(lm->w.ptr.p_double[2], _state);
    nclasses = ae_round(lm->w.ptr.p_double[3], _state);
    ae_vector_set_length(&workx, nvars-1+1, _state);
    ae_vector_set_length(&worky, nclasses-1+1, _state);
    result = 0;
    for(i=0; i<=npoints-1; i++)
    {
        
        /*
         * Process
         */
        ae_v_move(&workx.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
        mnlprocess(lm, &workx, &worky, _state);
        
        /*
         * Logit version of the answer
         */
        nmax = 0;
        for(j=0; j<=nclasses-1; j++)
        {
            if( ae_fp_greater(worky.ptr.p_double[j],worky.ptr.p_double[nmax]) )
            {
                nmax = j;
            }
        }
        
        /*
         * compare
         */
        if( nmax!=ae_round(xy->ptr.pp_double[i][nvars], _state) )
        {
            result = result+1;
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Internal subroutine. Places exponents of the anti-overflow shifted
internal linear outputs into the service part of the W array.
*************************************************************************/
static void logit_mnliexp(/* Real    */ ae_vector* w,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t nvars;
    ae_int_t nclasses;
    ae_int_t offs;
    ae_int_t i;
    ae_int_t i1;
    double v;
    double mx;


    ae_assert(ae_fp_eq(w->ptr.p_double[1],(double)(logit_logitvnum)), "LOGIT: unexpected model version", _state);
    nvars = ae_round(w->ptr.p_double[2], _state);
    nclasses = ae_round(w->ptr.p_double[3], _state);
    offs = ae_round(w->ptr.p_double[4], _state);
    i1 = offs+(nvars+1)*(nclasses-1);
    for(i=0; i<=nclasses-2; i++)
    {
        v = ae_v_dotproduct(&w->ptr.p_double[offs+i*(nvars+1)], 1, &x->ptr.p_double[0], 1, ae_v_len(offs+i*(nvars+1),offs+i*(nvars+1)+nvars-1));
        w->ptr.p_double[i1+i] = v+w->ptr.p_double[offs+i*(nvars+1)+nvars];
    }
    w->ptr.p_double[i1+nclasses-1] = (double)(0);
    mx = (double)(0);
    for(i=i1; i<=i1+nclasses-1; i++)
    {
        mx = ae_maxreal(mx, w->ptr.p_double[i], _state);
    }
    for(i=i1; i<=i1+nclasses-1; i++)
    {
        w->ptr.p_double[i] = ae_exp(w->ptr.p_double[i]-mx, _state);
    }
}


/*************************************************************************
Calculation of all types of errors

  -- ALGLIB --
     Copyright 30.08.2008 by Bochkanov Sergey
*************************************************************************/
static void logit_mnlallerrors(logitmodel* lm,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     double* relcls,
     double* avgce,
     double* rms,
     double* avg,
     double* avgrel,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t nvars;
    ae_int_t nclasses;
    ae_int_t i;
    ae_vector buf;
    ae_vector workx;
    ae_vector y;
    ae_vector dy;

    ae_frame_make(_state, &_frame_block);
    *relcls = 0;
    *avgce = 0;
    *rms = 0;
    *avg = 0;
    *avgrel = 0;
    ae_vector_init(&buf, 0, DT_REAL, _state);
    ae_vector_init(&workx, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&dy, 0, DT_REAL, _state);

    ae_assert(ae_round(lm->w.ptr.p_double[1], _state)==logit_logitvnum, "MNL unit: Incorrect MNL version!", _state);
    nvars = ae_round(lm->w.ptr.p_double[2], _state);
    nclasses = ae_round(lm->w.ptr.p_double[3], _state);
    ae_vector_set_length(&workx, nvars-1+1, _state);
    ae_vector_set_length(&y, nclasses-1+1, _state);
    ae_vector_set_length(&dy, 0+1, _state);
    dserrallocate(nclasses, &buf, _state);
    for(i=0; i<=npoints-1; i++)
    {
        ae_v_move(&workx.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
        mnlprocess(lm, &workx, &y, _state);
        dy.ptr.p_double[0] = xy->ptr.pp_double[i][nvars];
        dserraccumulate(&buf, &y, &dy, _state);
    }
    dserrfinish(&buf, _state);
    *relcls = buf.ptr.p_double[0];
    *avgce = buf.ptr.p_double[1];
    *rms = buf.ptr.p_double[2];
    *avg = buf.ptr.p_double[3];
    *avgrel = buf.ptr.p_double[4];
    ae_frame_leave(_state);
}


/*************************************************************************
THE  PURPOSE  OF  MCSRCH  IS  TO  FIND A STEP WHICH SATISFIES A SUFFICIENT
DECREASE CONDITION AND A CURVATURE CONDITION.

AT EACH STAGE THE SUBROUTINE  UPDATES  AN  INTERVAL  OF  UNCERTAINTY  WITH
ENDPOINTS  STX  AND  STY.  THE INTERVAL OF UNCERTAINTY IS INITIALLY CHOSEN
SO THAT IT CONTAINS A MINIMIZER OF THE MODIFIED FUNCTION

    F(X+STP*S) - F(X) - FTOL*STP*(GRADF(X)'S).

IF  A STEP  IS OBTAINED FOR  WHICH THE MODIFIED FUNCTION HAS A NONPOSITIVE
FUNCTION  VALUE  AND  NONNEGATIVE  DERIVATIVE,   THEN   THE   INTERVAL  OF
UNCERTAINTY IS CHOSEN SO THAT IT CONTAINS A MINIMIZER OF F(X+STP*S).

THE  ALGORITHM  IS  DESIGNED TO FIND A STEP WHICH SATISFIES THE SUFFICIENT
DECREASE CONDITION

    F(X+STP*S) .LE. F(X) + FTOL*STP*(GRADF(X)'S),

AND THE CURVATURE CONDITION

    ABS(GRADF(X+STP*S)'S)) .LE. GTOL*ABS(GRADF(X)'S).

IF  FTOL  IS  LESS  THAN GTOL AND IF, FOR EXAMPLE, THE FUNCTION IS BOUNDED
BELOW,  THEN  THERE  IS  ALWAYS  A  STEP  WHICH SATISFIES BOTH CONDITIONS.
IF  NO  STEP  CAN BE FOUND  WHICH  SATISFIES  BOTH  CONDITIONS,  THEN  THE
ALGORITHM  USUALLY STOPS  WHEN  ROUNDING ERRORS  PREVENT FURTHER PROGRESS.
IN THIS CASE STP ONLY SATISFIES THE SUFFICIENT DECREASE CONDITION.

PARAMETERS DESCRIPRION

N IS A POSITIVE INTEGER INPUT VARIABLE SET TO THE NUMBER OF VARIABLES.

X IS  AN  ARRAY  OF  LENGTH N. ON INPUT IT MUST CONTAIN THE BASE POINT FOR
THE LINE SEARCH. ON OUTPUT IT CONTAINS X+STP*S.

F IS  A  VARIABLE. ON INPUT IT MUST CONTAIN THE VALUE OF F AT X. ON OUTPUT
IT CONTAINS THE VALUE OF F AT X + STP*S.

G IS AN ARRAY OF LENGTH N. ON INPUT IT MUST CONTAIN THE GRADIENT OF F AT X.
ON OUTPUT IT CONTAINS THE GRADIENT OF F AT X + STP*S.

S IS AN INPUT ARRAY OF LENGTH N WHICH SPECIFIES THE SEARCH DIRECTION.

STP  IS  A NONNEGATIVE VARIABLE. ON INPUT STP CONTAINS AN INITIAL ESTIMATE
OF A SATISFACTORY STEP. ON OUTPUT STP CONTAINS THE FINAL ESTIMATE.

FTOL AND GTOL ARE NONNEGATIVE INPUT VARIABLES. TERMINATION OCCURS WHEN THE
SUFFICIENT DECREASE CONDITION AND THE DIRECTIONAL DERIVATIVE CONDITION ARE
SATISFIED.

XTOL IS A NONNEGATIVE INPUT VARIABLE. TERMINATION OCCURS WHEN THE RELATIVE
WIDTH OF THE INTERVAL OF UNCERTAINTY IS AT MOST XTOL.

STPMIN AND STPMAX ARE NONNEGATIVE INPUT VARIABLES WHICH SPECIFY LOWER  AND
UPPER BOUNDS FOR THE STEP.

MAXFEV IS A POSITIVE INTEGER INPUT VARIABLE. TERMINATION OCCURS WHEN THE
NUMBER OF CALLS TO FCN IS AT LEAST MAXFEV BY THE END OF AN ITERATION.

INFO IS AN INTEGER OUTPUT VARIABLE SET AS FOLLOWS:
    INFO = 0  IMPROPER INPUT PARAMETERS.

    INFO = 1  THE SUFFICIENT DECREASE CONDITION AND THE
              DIRECTIONAL DERIVATIVE CONDITION HOLD.

    INFO = 2  RELATIVE WIDTH OF THE INTERVAL OF UNCERTAINTY
              IS AT MOST XTOL.

    INFO = 3  NUMBER OF CALLS TO FCN HAS REACHED MAXFEV.

    INFO = 4  THE STEP IS AT THE LOWER BOUND STPMIN.

    INFO = 5  THE STEP IS AT THE UPPER BOUND STPMAX.

    INFO = 6  ROUNDING ERRORS PREVENT FURTHER PROGRESS.
              THERE MAY NOT BE A STEP WHICH SATISFIES THE
              SUFFICIENT DECREASE AND CURVATURE CONDITIONS.
              TOLERANCES MAY BE TOO SMALL.

NFEV IS AN INTEGER OUTPUT VARIABLE SET TO THE NUMBER OF CALLS TO FCN.

WA IS A WORK ARRAY OF LENGTH N.

ARGONNE NATIONAL LABORATORY. MINPACK PROJECT. JUNE 1983
JORGE J. MORE', DAVID J. THUENTE
*************************************************************************/
static void logit_mnlmcsrch(ae_int_t n,
     /* Real    */ ae_vector* x,
     double* f,
     /* Real    */ ae_vector* g,
     /* Real    */ ae_vector* s,
     double* stp,
     ae_int_t* info,
     ae_int_t* nfev,
     /* Real    */ ae_vector* wa,
     logitmcstate* state,
     ae_int_t* stage,
     ae_state *_state)
{
    double v;
    double p5;
    double p66;
    double zero;


    
    /*
     * init
     */
    p5 = 0.5;
    p66 = 0.66;
    state->xtrapf = 4.0;
    zero = (double)(0);
    
    /*
     * Main cycle
     */
    for(;;)
    {
        if( *stage==0 )
        {
            
            /*
             * NEXT
             */
            *stage = 2;
            continue;
        }
        if( *stage==2 )
        {
            state->infoc = 1;
            *info = 0;
            
            /*
             *     CHECK THE INPUT PARAMETERS FOR ERRORS.
             */
            if( ((((((n<=0||ae_fp_less_eq(*stp,(double)(0)))||ae_fp_less(logit_ftol,(double)(0)))||ae_fp_less(logit_gtol,zero))||ae_fp_less(logit_xtol,zero))||ae_fp_less(logit_stpmin,zero))||ae_fp_less(logit_stpmax,logit_stpmin))||logit_maxfev<=0 )
            {
                *stage = 0;
                return;
            }
            
            /*
             *     COMPUTE THE INITIAL GRADIENT IN THE SEARCH DIRECTION
             *     AND CHECK THAT S IS A DESCENT DIRECTION.
             */
            v = ae_v_dotproduct(&g->ptr.p_double[0], 1, &s->ptr.p_double[0], 1, ae_v_len(0,n-1));
            state->dginit = v;
            if( ae_fp_greater_eq(state->dginit,(double)(0)) )
            {
                *stage = 0;
                return;
            }
            
            /*
             *     INITIALIZE LOCAL VARIABLES.
             */
            state->brackt = ae_false;
            state->stage1 = ae_true;
            *nfev = 0;
            state->finit = *f;
            state->dgtest = logit_ftol*state->dginit;
            state->width = logit_stpmax-logit_stpmin;
            state->width1 = state->width/p5;
            ae_v_move(&wa->ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
            
            /*
             *     THE VARIABLES STX, FX, DGX CONTAIN THE VALUES OF THE STEP,
             *     FUNCTION, AND DIRECTIONAL DERIVATIVE AT THE BEST STEP.
             *     THE VARIABLES STY, FY, DGY CONTAIN THE VALUE OF THE STEP,
             *     FUNCTION, AND DERIVATIVE AT THE OTHER ENDPOINT OF
             *     THE INTERVAL OF UNCERTAINTY.
             *     THE VARIABLES STP, F, DG CONTAIN THE VALUES OF THE STEP,
             *     FUNCTION, AND DERIVATIVE AT THE CURRENT STEP.
             */
            state->stx = (double)(0);
            state->fx = state->finit;
            state->dgx = state->dginit;
            state->sty = (double)(0);
            state->fy = state->finit;
            state->dgy = state->dginit;
            
            /*
             * NEXT
             */
            *stage = 3;
            continue;
        }
        if( *stage==3 )
        {
            
            /*
             *     START OF ITERATION.
             *
             *     SET THE MINIMUM AND MAXIMUM STEPS TO CORRESPOND
             *     TO THE PRESENT INTERVAL OF UNCERTAINTY.
             */
            if( state->brackt )
            {
                if( ae_fp_less(state->stx,state->sty) )
                {
                    state->stmin = state->stx;
                    state->stmax = state->sty;
                }
                else
                {
                    state->stmin = state->sty;
                    state->stmax = state->stx;
                }
            }
            else
            {
                state->stmin = state->stx;
                state->stmax = *stp+state->xtrapf*(*stp-state->stx);
            }
            
            /*
             *        FORCE THE STEP TO BE WITHIN THE BOUNDS STPMAX AND STPMIN.
             */
            if( ae_fp_greater(*stp,logit_stpmax) )
            {
                *stp = logit_stpmax;
            }
            if( ae_fp_less(*stp,logit_stpmin) )
            {
                *stp = logit_stpmin;
            }
            
            /*
             *        IF AN UNUSUAL TERMINATION IS TO OCCUR THEN LET
             *        STP BE THE LOWEST POINT OBTAINED SO FAR.
             */
            if( (((state->brackt&&(ae_fp_less_eq(*stp,state->stmin)||ae_fp_greater_eq(*stp,state->stmax)))||*nfev>=logit_maxfev-1)||state->infoc==0)||(state->brackt&&ae_fp_less_eq(state->stmax-state->stmin,logit_xtol*state->stmax)) )
            {
                *stp = state->stx;
            }
            
            /*
             *        EVALUATE THE FUNCTION AND GRADIENT AT STP
             *        AND COMPUTE THE DIRECTIONAL DERIVATIVE.
             */
            ae_v_move(&x->ptr.p_double[0], 1, &wa->ptr.p_double[0], 1, ae_v_len(0,n-1));
            ae_v_addd(&x->ptr.p_double[0], 1, &s->ptr.p_double[0], 1, ae_v_len(0,n-1), *stp);
            
            /*
             * NEXT
             */
            *stage = 4;
            return;
        }
        if( *stage==4 )
        {
            *info = 0;
            *nfev = *nfev+1;
            v = ae_v_dotproduct(&g->ptr.p_double[0], 1, &s->ptr.p_double[0], 1, ae_v_len(0,n-1));
            state->dg = v;
            state->ftest1 = state->finit+*stp*state->dgtest;
            
            /*
             *        TEST FOR CONVERGENCE.
             */
            if( (state->brackt&&(ae_fp_less_eq(*stp,state->stmin)||ae_fp_greater_eq(*stp,state->stmax)))||state->infoc==0 )
            {
                *info = 6;
            }
            if( (ae_fp_eq(*stp,logit_stpmax)&&ae_fp_less_eq(*f,state->ftest1))&&ae_fp_less_eq(state->dg,state->dgtest) )
            {
                *info = 5;
            }
            if( ae_fp_eq(*stp,logit_stpmin)&&(ae_fp_greater(*f,state->ftest1)||ae_fp_greater_eq(state->dg,state->dgtest)) )
            {
                *info = 4;
            }
            if( *nfev>=logit_maxfev )
            {
                *info = 3;
            }
            if( state->brackt&&ae_fp_less_eq(state->stmax-state->stmin,logit_xtol*state->stmax) )
            {
                *info = 2;
            }
            if( ae_fp_less_eq(*f,state->ftest1)&&ae_fp_less_eq(ae_fabs(state->dg, _state),-logit_gtol*state->dginit) )
            {
                *info = 1;
            }
            
            /*
             *        CHECK FOR TERMINATION.
             */
            if( *info!=0 )
            {
                *stage = 0;
                return;
            }
            
            /*
             *        IN THE FIRST STAGE WE SEEK A STEP FOR WHICH THE MODIFIED
             *        FUNCTION HAS A NONPOSITIVE VALUE AND NONNEGATIVE DERIVATIVE.
             */
            if( (state->stage1&&ae_fp_less_eq(*f,state->ftest1))&&ae_fp_greater_eq(state->dg,ae_minreal(logit_ftol, logit_gtol, _state)*state->dginit) )
            {
                state->stage1 = ae_false;
            }
            
            /*
             *        A MODIFIED FUNCTION IS USED TO PREDICT THE STEP ONLY IF
             *        WE HAVE NOT OBTAINED A STEP FOR WHICH THE MODIFIED
             *        FUNCTION HAS A NONPOSITIVE FUNCTION VALUE AND NONNEGATIVE
             *        DERIVATIVE, AND IF A LOWER FUNCTION VALUE HAS BEEN
             *        OBTAINED BUT THE DECREASE IS NOT SUFFICIENT.
             */
            if( (state->stage1&&ae_fp_less_eq(*f,state->fx))&&ae_fp_greater(*f,state->ftest1) )
            {
                
                /*
                 *           DEFINE THE MODIFIED FUNCTION AND DERIVATIVE VALUES.
                 */
                state->fm = *f-*stp*state->dgtest;
                state->fxm = state->fx-state->stx*state->dgtest;
                state->fym = state->fy-state->sty*state->dgtest;
                state->dgm = state->dg-state->dgtest;
                state->dgxm = state->dgx-state->dgtest;
                state->dgym = state->dgy-state->dgtest;
                
                /*
                 *           CALL CSTEP TO UPDATE THE INTERVAL OF UNCERTAINTY
                 *           AND TO COMPUTE THE NEW STEP.
                 */
                logit_mnlmcstep(&state->stx, &state->fxm, &state->dgxm, &state->sty, &state->fym, &state->dgym, stp, state->fm, state->dgm, &state->brackt, state->stmin, state->stmax, &state->infoc, _state);
                
                /*
                 *           RESET THE FUNCTION AND GRADIENT VALUES FOR F.
                 */
                state->fx = state->fxm+state->stx*state->dgtest;
                state->fy = state->fym+state->sty*state->dgtest;
                state->dgx = state->dgxm+state->dgtest;
                state->dgy = state->dgym+state->dgtest;
            }
            else
            {
                
                /*
                 *           CALL MCSTEP TO UPDATE THE INTERVAL OF UNCERTAINTY
                 *           AND TO COMPUTE THE NEW STEP.
                 */
                logit_mnlmcstep(&state->stx, &state->fx, &state->dgx, &state->sty, &state->fy, &state->dgy, stp, *f, state->dg, &state->brackt, state->stmin, state->stmax, &state->infoc, _state);
            }
            
            /*
             *        FORCE A SUFFICIENT DECREASE IN THE SIZE OF THE
             *        INTERVAL OF UNCERTAINTY.
             */
            if( state->brackt )
            {
                if( ae_fp_greater_eq(ae_fabs(state->sty-state->stx, _state),p66*state->width1) )
                {
                    *stp = state->stx+p5*(state->sty-state->stx);
                }
                state->width1 = state->width;
                state->width = ae_fabs(state->sty-state->stx, _state);
            }
            
            /*
             *  NEXT.
             */
            *stage = 3;
            continue;
        }
    }
}


static void logit_mnlmcstep(double* stx,
     double* fx,
     double* dx,
     double* sty,
     double* fy,
     double* dy,
     double* stp,
     double fp,
     double dp,
     ae_bool* brackt,
     double stmin,
     double stmax,
     ae_int_t* info,
     ae_state *_state)
{
    ae_bool bound;
    double gamma;
    double p;
    double q;
    double r;
    double s;
    double sgnd;
    double stpc;
    double stpf;
    double stpq;
    double theta;


    *info = 0;
    
    /*
     *     CHECK THE INPUT PARAMETERS FOR ERRORS.
     */
    if( ((*brackt&&(ae_fp_less_eq(*stp,ae_minreal(*stx, *sty, _state))||ae_fp_greater_eq(*stp,ae_maxreal(*stx, *sty, _state))))||ae_fp_greater_eq(*dx*(*stp-(*stx)),(double)(0)))||ae_fp_less(stmax,stmin) )
    {
        return;
    }
    
    /*
     *     DETERMINE IF THE DERIVATIVES HAVE OPPOSITE SIGN.
     */
    sgnd = dp*(*dx/ae_fabs(*dx, _state));
    
    /*
     *     FIRST CASE. A HIGHER FUNCTION VALUE.
     *     THE MINIMUM IS BRACKETED. IF THE CUBIC STEP IS CLOSER
     *     TO STX THAN THE QUADRATIC STEP, THE CUBIC STEP IS TAKEN,
     *     ELSE THE AVERAGE OF THE CUBIC AND QUADRATIC STEPS IS TAKEN.
     */
    if( ae_fp_greater(fp,*fx) )
    {
        *info = 1;
        bound = ae_true;
        theta = 3*(*fx-fp)/(*stp-(*stx))+(*dx)+dp;
        s = ae_maxreal(ae_fabs(theta, _state), ae_maxreal(ae_fabs(*dx, _state), ae_fabs(dp, _state), _state), _state);
        gamma = s*ae_sqrt(ae_sqr(theta/s, _state)-*dx/s*(dp/s), _state);
        if( ae_fp_less(*stp,*stx) )
        {
            gamma = -gamma;
        }
        p = gamma-(*dx)+theta;
        q = gamma-(*dx)+gamma+dp;
        r = p/q;
        stpc = *stx+r*(*stp-(*stx));
        stpq = *stx+*dx/((*fx-fp)/(*stp-(*stx))+(*dx))/2*(*stp-(*stx));
        if( ae_fp_less(ae_fabs(stpc-(*stx), _state),ae_fabs(stpq-(*stx), _state)) )
        {
            stpf = stpc;
        }
        else
        {
            stpf = stpc+(stpq-stpc)/2;
        }
        *brackt = ae_true;
    }
    else
    {
        if( ae_fp_less(sgnd,(double)(0)) )
        {
            
            /*
             *     SECOND CASE. A LOWER FUNCTION VALUE AND DERIVATIVES OF
             *     OPPOSITE SIGN. THE MINIMUM IS BRACKETED. IF THE CUBIC
             *     STEP IS CLOSER TO STX THAN THE QUADRATIC (SECANT) STEP,
             *     THE CUBIC STEP IS TAKEN, ELSE THE QUADRATIC STEP IS TAKEN.
             */
            *info = 2;
            bound = ae_false;
            theta = 3*(*fx-fp)/(*stp-(*stx))+(*dx)+dp;
            s = ae_maxreal(ae_fabs(theta, _state), ae_maxreal(ae_fabs(*dx, _state), ae_fabs(dp, _state), _state), _state);
            gamma = s*ae_sqrt(ae_sqr(theta/s, _state)-*dx/s*(dp/s), _state);
            if( ae_fp_greater(*stp,*stx) )
            {
                gamma = -gamma;
            }
            p = gamma-dp+theta;
            q = gamma-dp+gamma+(*dx);
            r = p/q;
            stpc = *stp+r*(*stx-(*stp));
            stpq = *stp+dp/(dp-(*dx))*(*stx-(*stp));
            if( ae_fp_greater(ae_fabs(stpc-(*stp), _state),ae_fabs(stpq-(*stp), _state)) )
            {
                stpf = stpc;
            }
            else
            {
                stpf = stpq;
            }
            *brackt = ae_true;
        }
        else
        {
            if( ae_fp_less(ae_fabs(dp, _state),ae_fabs(*dx, _state)) )
            {
                
                /*
                 *     THIRD CASE. A LOWER FUNCTION VALUE, DERIVATIVES OF THE
                 *     SAME SIGN, AND THE MAGNITUDE OF THE DERIVATIVE DECREASES.
                 *     THE CUBIC STEP IS ONLY USED IF THE CUBIC TENDS TO INFINITY
                 *     IN THE DIRECTION OF THE STEP OR IF THE MINIMUM OF THE CUBIC
                 *     IS BEYOND STP. OTHERWISE THE CUBIC STEP IS DEFINED TO BE
                 *     EITHER STPMIN OR STPMAX. THE QUADRATIC (SECANT) STEP IS ALSO
                 *     COMPUTED AND IF THE MINIMUM IS BRACKETED THEN THE THE STEP
                 *     CLOSEST TO STX IS TAKEN, ELSE THE STEP FARTHEST AWAY IS TAKEN.
                 */
                *info = 3;
                bound = ae_true;
                theta = 3*(*fx-fp)/(*stp-(*stx))+(*dx)+dp;
                s = ae_maxreal(ae_fabs(theta, _state), ae_maxreal(ae_fabs(*dx, _state), ae_fabs(dp, _state), _state), _state);
                
                /*
                 *        THE CASE GAMMA = 0 ONLY ARISES IF THE CUBIC DOES NOT TEND
                 *        TO INFINITY IN THE DIRECTION OF THE STEP.
                 */
                gamma = s*ae_sqrt(ae_maxreal((double)(0), ae_sqr(theta/s, _state)-*dx/s*(dp/s), _state), _state);
                if( ae_fp_greater(*stp,*stx) )
                {
                    gamma = -gamma;
                }
                p = gamma-dp+theta;
                q = gamma+(*dx-dp)+gamma;
                r = p/q;
                if( ae_fp_less(r,(double)(0))&&ae_fp_neq(gamma,(double)(0)) )
                {
                    stpc = *stp+r*(*stx-(*stp));
                }
                else
                {
                    if( ae_fp_greater(*stp,*stx) )
                    {
                        stpc = stmax;
                    }
                    else
                    {
                        stpc = stmin;
                    }
                }
                stpq = *stp+dp/(dp-(*dx))*(*stx-(*stp));
                if( *brackt )
                {
                    if( ae_fp_less(ae_fabs(*stp-stpc, _state),ae_fabs(*stp-stpq, _state)) )
                    {
                        stpf = stpc;
                    }
                    else
                    {
                        stpf = stpq;
                    }
                }
                else
                {
                    if( ae_fp_greater(ae_fabs(*stp-stpc, _state),ae_fabs(*stp-stpq, _state)) )
                    {
                        stpf = stpc;
                    }
                    else
                    {
                        stpf = stpq;
                    }
                }
            }
            else
            {
                
                /*
                 *     FOURTH CASE. A LOWER FUNCTION VALUE, DERIVATIVES OF THE
                 *     SAME SIGN, AND THE MAGNITUDE OF THE DERIVATIVE DOES
                 *     NOT DECREASE. IF THE MINIMUM IS NOT BRACKETED, THE STEP
                 *     IS EITHER STPMIN OR STPMAX, ELSE THE CUBIC STEP IS TAKEN.
                 */
                *info = 4;
                bound = ae_false;
                if( *brackt )
                {
                    theta = 3*(fp-(*fy))/(*sty-(*stp))+(*dy)+dp;
                    s = ae_maxreal(ae_fabs(theta, _state), ae_maxreal(ae_fabs(*dy, _state), ae_fabs(dp, _state), _state), _state);
                    gamma = s*ae_sqrt(ae_sqr(theta/s, _state)-*dy/s*(dp/s), _state);
                    if( ae_fp_greater(*stp,*sty) )
                    {
                        gamma = -gamma;
                    }
                    p = gamma-dp+theta;
                    q = gamma-dp+gamma+(*dy);
                    r = p/q;
                    stpc = *stp+r*(*sty-(*stp));
                    stpf = stpc;
                }
                else
                {
                    if( ae_fp_greater(*stp,*stx) )
                    {
                        stpf = stmax;
                    }
                    else
                    {
                        stpf = stmin;
                    }
                }
            }
        }
    }
    
    /*
     *     UPDATE THE INTERVAL OF UNCERTAINTY. THIS UPDATE DOES NOT
     *     DEPEND ON THE NEW STEP OR THE CASE ANALYSIS ABOVE.
     */
    if( ae_fp_greater(fp,*fx) )
    {
        *sty = *stp;
        *fy = fp;
        *dy = dp;
    }
    else
    {
        if( ae_fp_less(sgnd,0.0) )
        {
            *sty = *stx;
            *fy = *fx;
            *dy = *dx;
        }
        *stx = *stp;
        *fx = fp;
        *dx = dp;
    }
    
    /*
     *     COMPUTE THE NEW STEP AND SAFEGUARD IT.
     */
    stpf = ae_minreal(stmax, stpf, _state);
    stpf = ae_maxreal(stmin, stpf, _state);
    *stp = stpf;
    if( *brackt&&bound )
    {
        if( ae_fp_greater(*sty,*stx) )
        {
            *stp = ae_minreal(*stx+0.66*(*sty-(*stx)), *stp, _state);
        }
        else
        {
            *stp = ae_maxreal(*stx+0.66*(*sty-(*stx)), *stp, _state);
        }
    }
}


void _logitmodel_init(void* _p, ae_state *_state)
{
    logitmodel *p = (logitmodel*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->w, 0, DT_REAL, _state);
}


void _logitmodel_init_copy(void* _dst, void* _src, ae_state *_state)
{
    logitmodel *dst = (logitmodel*)_dst;
    logitmodel *src = (logitmodel*)_src;
    ae_vector_init_copy(&dst->w, &src->w, _state);
}


void _logitmodel_clear(void* _p)
{
    logitmodel *p = (logitmodel*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->w);
}


void _logitmodel_destroy(void* _p)
{
    logitmodel *p = (logitmodel*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->w);
}


void _logitmcstate_init(void* _p, ae_state *_state)
{
    logitmcstate *p = (logitmcstate*)_p;
    ae_touch_ptr((void*)p);
}


void _logitmcstate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    logitmcstate *dst = (logitmcstate*)_dst;
    logitmcstate *src = (logitmcstate*)_src;
    dst->brackt = src->brackt;
    dst->stage1 = src->stage1;
    dst->infoc = src->infoc;
    dst->dg = src->dg;
    dst->dgm = src->dgm;
    dst->dginit = src->dginit;
    dst->dgtest = src->dgtest;
    dst->dgx = src->dgx;
    dst->dgxm = src->dgxm;
    dst->dgy = src->dgy;
    dst->dgym = src->dgym;
    dst->finit = src->finit;
    dst->ftest1 = src->ftest1;
    dst->fm = src->fm;
    dst->fx = src->fx;
    dst->fxm = src->fxm;
    dst->fy = src->fy;
    dst->fym = src->fym;
    dst->stx = src->stx;
    dst->sty = src->sty;
    dst->stmin = src->stmin;
    dst->stmax = src->stmax;
    dst->width = src->width;
    dst->width1 = src->width1;
    dst->xtrapf = src->xtrapf;
}


void _logitmcstate_clear(void* _p)
{
    logitmcstate *p = (logitmcstate*)_p;
    ae_touch_ptr((void*)p);
}


void _logitmcstate_destroy(void* _p)
{
    logitmcstate *p = (logitmcstate*)_p;
    ae_touch_ptr((void*)p);
}


void _mnlreport_init(void* _p, ae_state *_state)
{
    mnlreport *p = (mnlreport*)_p;
    ae_touch_ptr((void*)p);
}


void _mnlreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    mnlreport *dst = (mnlreport*)_dst;
    mnlreport *src = (mnlreport*)_src;
    dst->ngrad = src->ngrad;
    dst->nhess = src->nhess;
}


void _mnlreport_clear(void* _p)
{
    mnlreport *p = (mnlreport*)_p;
    ae_touch_ptr((void*)p);
}


void _mnlreport_destroy(void* _p)
{
    mnlreport *p = (mnlreport*)_p;
    ae_touch_ptr((void*)p);
}


/*$ End $*/
