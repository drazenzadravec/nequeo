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
#include "mlptrain.h"


/*$ Declarations $*/
static double mlptrain_mindecay = 0.001;
static ae_int_t mlptrain_defaultlbfgsfactor = 6;
static void mlptrain_mlpkfoldcvgeneral(multilayerperceptron* n,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     double decay,
     ae_int_t restarts,
     ae_int_t foldscount,
     ae_bool lmalgorithm,
     double wstep,
     ae_int_t maxits,
     ae_int_t* info,
     mlpreport* rep,
     mlpcvreport* cvrep,
     ae_state *_state);
static void mlptrain_mlpkfoldsplit(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nclasses,
     ae_int_t foldscount,
     ae_bool stratifiedsplits,
     /* Integer */ ae_vector* folds,
     ae_state *_state);
static void mlptrain_mthreadcv(mlptrainer* s,
     ae_int_t rowsize,
     ae_int_t nrestarts,
     /* Integer */ ae_vector* folds,
     ae_int_t fold,
     ae_int_t dfold,
     /* Real    */ ae_matrix* cvy,
     ae_shared_pool* pooldatacv,
     ae_state *_state);
static void mlptrain_mlptrainnetworkx(mlptrainer* s,
     ae_int_t nrestarts,
     ae_int_t algokind,
     /* Integer */ ae_vector* trnsubset,
     ae_int_t trnsubsetsize,
     /* Integer */ ae_vector* valsubset,
     ae_int_t valsubsetsize,
     multilayerperceptron* network,
     mlpreport* rep,
     ae_bool isrootcall,
     ae_shared_pool* sessions,
     ae_state *_state);
static void mlptrain_mlptrainensemblex(mlptrainer* s,
     mlpensemble* ensemble,
     ae_int_t idx0,
     ae_int_t idx1,
     ae_int_t nrestarts,
     ae_int_t trainingmethod,
     sinteger* ngrad,
     ae_bool isrootcall,
     ae_shared_pool* esessions,
     ae_state *_state);
static void mlptrain_mlpstarttrainingx(mlptrainer* s,
     ae_bool randomstart,
     ae_int_t algokind,
     /* Integer */ ae_vector* subset,
     ae_int_t subsetsize,
     smlptrnsession* session,
     ae_state *_state);
static ae_bool mlptrain_mlpcontinuetrainingx(mlptrainer* s,
     /* Integer */ ae_vector* subset,
     ae_int_t subsetsize,
     ae_int_t* ngradbatch,
     smlptrnsession* session,
     ae_state *_state);
static void mlptrain_mlpebagginginternal(mlpensemble* ensemble,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     double decay,
     ae_int_t restarts,
     double wstep,
     ae_int_t maxits,
     ae_bool lmalgorithm,
     ae_int_t* info,
     mlpreport* rep,
     mlpcvreport* ooberrors,
     ae_state *_state);
static void mlptrain_initmlptrnsession(multilayerperceptron* networktrained,
     ae_bool randomizenetwork,
     mlptrainer* trainer,
     smlptrnsession* session,
     ae_state *_state);
static void mlptrain_initmlptrnsessions(multilayerperceptron* networktrained,
     ae_bool randomizenetwork,
     mlptrainer* trainer,
     ae_shared_pool* sessions,
     ae_state *_state);
static void mlptrain_initmlpetrnsession(multilayerperceptron* individualnetwork,
     mlptrainer* trainer,
     mlpetrnsession* session,
     ae_state *_state);
static void mlptrain_initmlpetrnsessions(multilayerperceptron* individualnetwork,
     mlptrainer* trainer,
     ae_shared_pool* sessions,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Neural network training  using  modified  Levenberg-Marquardt  with  exact
Hessian calculation and regularization. Subroutine trains  neural  network
with restarts from random positions. Algorithm is well  suited  for  small
and medium scale problems (hundreds of weights).

INPUT PARAMETERS:
    Network     -   neural network with initialized geometry
    XY          -   training set
    NPoints     -   training set size
    Decay       -   weight decay constant, >=0.001
                    Decay term 'Decay*||Weights||^2' is added to error
                    function.
                    If you don't know what Decay to choose, use 0.001.
    Restarts    -   number of restarts from random position, >0.
                    If you don't know what Restarts to choose, use 2.

OUTPUT PARAMETERS:
    Network     -   trained neural network.
    Info        -   return code:
                    * -9, if internal matrix inverse subroutine failed
                    * -2, if there is a point with class number
                          outside of [0..NOut-1].
                    * -1, if wrong parameters specified
                          (NPoints<0, Restarts<1).
                    *  2, if task has been solved.
    Rep         -   training report

  -- ALGLIB --
     Copyright 10.03.2009 by Bochkanov Sergey
*************************************************************************/
void mlptrainlm(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     double decay,
     ae_int_t restarts,
     ae_int_t* info,
     mlpreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    double lmsteptol;
    ae_int_t i;
    ae_int_t k;
    double v;
    double e;
    double enew;
    double xnorm2;
    double stepnorm;
    ae_vector g;
    ae_vector d;
    ae_matrix h;
    ae_matrix hmod;
    ae_matrix z;
    ae_bool spd;
    double nu;
    double lambdav;
    double lambdaup;
    double lambdadown;
    minlbfgsreport internalrep;
    minlbfgsstate state;
    ae_vector x;
    ae_vector y;
    ae_vector wbase;
    ae_vector wdir;
    ae_vector wt;
    ae_vector wx;
    ae_int_t pass;
    ae_vector wbest;
    double ebest;
    ae_int_t invinfo;
    matinvreport invrep;
    ae_int_t solverinfo;
    densesolverreport solverrep;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _mlpreport_clear(rep);
    ae_vector_init(&g, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_matrix_init(&h, 0, 0, DT_REAL, _state);
    ae_matrix_init(&hmod, 0, 0, DT_REAL, _state);
    ae_matrix_init(&z, 0, 0, DT_REAL, _state);
    _minlbfgsreport_init(&internalrep, _state);
    _minlbfgsstate_init(&state, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&wbase, 0, DT_REAL, _state);
    ae_vector_init(&wdir, 0, DT_REAL, _state);
    ae_vector_init(&wt, 0, DT_REAL, _state);
    ae_vector_init(&wx, 0, DT_REAL, _state);
    ae_vector_init(&wbest, 0, DT_REAL, _state);
    _matinvreport_init(&invrep, _state);
    _densesolverreport_init(&solverrep, _state);

    mlpproperties(network, &nin, &nout, &wcount, _state);
    lambdaup = (double)(10);
    lambdadown = 0.3;
    lmsteptol = 0.001;
    
    /*
     * Test for inputs
     */
    if( npoints<=0||restarts<1 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    if( mlpissoftmax(network, _state) )
    {
        for(i=0; i<=npoints-1; i++)
        {
            if( ae_round(xy->ptr.pp_double[i][nin], _state)<0||ae_round(xy->ptr.pp_double[i][nin], _state)>=nout )
            {
                *info = -2;
                ae_frame_leave(_state);
                return;
            }
        }
    }
    decay = ae_maxreal(decay, mlptrain_mindecay, _state);
    *info = 2;
    
    /*
     * Initialize data
     */
    rep->ngrad = 0;
    rep->nhess = 0;
    rep->ncholesky = 0;
    
    /*
     * General case.
     * Prepare task and network. Allocate space.
     */
    mlpinitpreprocessor(network, xy, npoints, _state);
    ae_vector_set_length(&g, wcount-1+1, _state);
    ae_matrix_set_length(&h, wcount-1+1, wcount-1+1, _state);
    ae_matrix_set_length(&hmod, wcount-1+1, wcount-1+1, _state);
    ae_vector_set_length(&wbase, wcount-1+1, _state);
    ae_vector_set_length(&wdir, wcount-1+1, _state);
    ae_vector_set_length(&wbest, wcount-1+1, _state);
    ae_vector_set_length(&wt, wcount-1+1, _state);
    ae_vector_set_length(&wx, wcount-1+1, _state);
    ebest = ae_maxrealnumber;
    
    /*
     * Multiple passes
     */
    for(pass=1; pass<=restarts; pass++)
    {
        
        /*
         * Initialize weights
         */
        mlprandomize(network, _state);
        
        /*
         * First stage of the hybrid algorithm: LBFGS
         */
        ae_v_move(&wbase.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        minlbfgscreate(wcount, ae_minint(wcount, 5, _state), &wbase, &state, _state);
        minlbfgssetcond(&state, (double)(0), (double)(0), (double)(0), ae_maxint(25, wcount, _state), _state);
        while(minlbfgsiteration(&state, _state))
        {
            
            /*
             * gradient
             */
            ae_v_move(&network->weights.ptr.p_double[0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            mlpgradbatch(network, xy, npoints, &state.f, &state.g, _state);
            
            /*
             * weight decay
             */
            v = ae_v_dotproduct(&network->weights.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            state.f = state.f+0.5*decay*v;
            ae_v_addd(&state.g.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1), decay);
            
            /*
             * next iteration
             */
            rep->ngrad = rep->ngrad+1;
        }
        minlbfgsresults(&state, &wbase, &internalrep, _state);
        ae_v_move(&network->weights.ptr.p_double[0], 1, &wbase.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        
        /*
         * Second stage of the hybrid algorithm: LM
         *
         * Initialize H with identity matrix,
         * G with gradient,
         * E with regularized error.
         */
        mlphessianbatch(network, xy, npoints, &e, &g, &h, _state);
        v = ae_v_dotproduct(&network->weights.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        e = e+0.5*decay*v;
        ae_v_addd(&g.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1), decay);
        for(k=0; k<=wcount-1; k++)
        {
            h.ptr.pp_double[k][k] = h.ptr.pp_double[k][k]+decay;
        }
        rep->nhess = rep->nhess+1;
        lambdav = 0.001;
        nu = (double)(2);
        for(;;)
        {
            
            /*
             * 1. HMod = H+lambda*I
             * 2. Try to solve (H+Lambda*I)*dx = -g.
             *    Increase lambda if left part is not positive definite.
             */
            for(i=0; i<=wcount-1; i++)
            {
                ae_v_move(&hmod.ptr.pp_double[i][0], 1, &h.ptr.pp_double[i][0], 1, ae_v_len(0,wcount-1));
                hmod.ptr.pp_double[i][i] = hmod.ptr.pp_double[i][i]+lambdav;
            }
            spd = spdmatrixcholesky(&hmod, wcount, ae_true, _state);
            rep->ncholesky = rep->ncholesky+1;
            if( !spd )
            {
                lambdav = lambdav*lambdaup*nu;
                nu = nu*2;
                continue;
            }
            spdmatrixcholeskysolve(&hmod, wcount, ae_true, &g, &solverinfo, &solverrep, &wdir, _state);
            if( solverinfo<0 )
            {
                lambdav = lambdav*lambdaup*nu;
                nu = nu*2;
                continue;
            }
            ae_v_muld(&wdir.ptr.p_double[0], 1, ae_v_len(0,wcount-1), -1);
            
            /*
             * Lambda found.
             * 1. Save old w in WBase
             * 1. Test some stopping criterions
             * 2. If error(w+wdir)>error(w), increase lambda
             */
            ae_v_add(&network->weights.ptr.p_double[0], 1, &wdir.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            xnorm2 = ae_v_dotproduct(&network->weights.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            stepnorm = ae_v_dotproduct(&wdir.ptr.p_double[0], 1, &wdir.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            stepnorm = ae_sqrt(stepnorm, _state);
            enew = mlperror(network, xy, npoints, _state)+0.5*decay*xnorm2;
            if( ae_fp_less(stepnorm,lmsteptol*(1+ae_sqrt(xnorm2, _state))) )
            {
                break;
            }
            if( ae_fp_greater(enew,e) )
            {
                lambdav = lambdav*lambdaup*nu;
                nu = nu*2;
                continue;
            }
            
            /*
             * Optimize using inv(cholesky(H)) as preconditioner
             */
            rmatrixtrinverse(&hmod, wcount, ae_true, ae_false, &invinfo, &invrep, _state);
            if( invinfo<=0 )
            {
                
                /*
                 * if matrix can't be inverted then exit with errors
                 * TODO: make WCount steps in direction suggested by HMod
                 */
                *info = -9;
                ae_frame_leave(_state);
                return;
            }
            ae_v_move(&wbase.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            for(i=0; i<=wcount-1; i++)
            {
                wt.ptr.p_double[i] = (double)(0);
            }
            minlbfgscreatex(wcount, wcount, &wt, 1, 0.0, &state, _state);
            minlbfgssetcond(&state, (double)(0), (double)(0), (double)(0), 5, _state);
            while(minlbfgsiteration(&state, _state))
            {
                
                /*
                 * gradient
                 */
                for(i=0; i<=wcount-1; i++)
                {
                    v = ae_v_dotproduct(&state.x.ptr.p_double[i], 1, &hmod.ptr.pp_double[i][i], 1, ae_v_len(i,wcount-1));
                    network->weights.ptr.p_double[i] = wbase.ptr.p_double[i]+v;
                }
                mlpgradbatch(network, xy, npoints, &state.f, &g, _state);
                for(i=0; i<=wcount-1; i++)
                {
                    state.g.ptr.p_double[i] = (double)(0);
                }
                for(i=0; i<=wcount-1; i++)
                {
                    v = g.ptr.p_double[i];
                    ae_v_addd(&state.g.ptr.p_double[i], 1, &hmod.ptr.pp_double[i][i], 1, ae_v_len(i,wcount-1), v);
                }
                
                /*
                 * weight decay
                 * grad(x'*x) = A'*(x0+A*t)
                 */
                v = ae_v_dotproduct(&network->weights.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
                state.f = state.f+0.5*decay*v;
                for(i=0; i<=wcount-1; i++)
                {
                    v = decay*network->weights.ptr.p_double[i];
                    ae_v_addd(&state.g.ptr.p_double[i], 1, &hmod.ptr.pp_double[i][i], 1, ae_v_len(i,wcount-1), v);
                }
                
                /*
                 * next iteration
                 */
                rep->ngrad = rep->ngrad+1;
            }
            minlbfgsresults(&state, &wt, &internalrep, _state);
            
            /*
             * Accept new position.
             * Calculate Hessian
             */
            for(i=0; i<=wcount-1; i++)
            {
                v = ae_v_dotproduct(&wt.ptr.p_double[i], 1, &hmod.ptr.pp_double[i][i], 1, ae_v_len(i,wcount-1));
                network->weights.ptr.p_double[i] = wbase.ptr.p_double[i]+v;
            }
            mlphessianbatch(network, xy, npoints, &e, &g, &h, _state);
            v = ae_v_dotproduct(&network->weights.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            e = e+0.5*decay*v;
            ae_v_addd(&g.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1), decay);
            for(k=0; k<=wcount-1; k++)
            {
                h.ptr.pp_double[k][k] = h.ptr.pp_double[k][k]+decay;
            }
            rep->nhess = rep->nhess+1;
            
            /*
             * Update lambda
             */
            lambdav = lambdav*lambdadown;
            nu = (double)(2);
        }
        
        /*
         * update WBest
         */
        v = ae_v_dotproduct(&network->weights.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        e = 0.5*decay*v+mlperror(network, xy, npoints, _state);
        if( ae_fp_less(e,ebest) )
        {
            ebest = e;
            ae_v_move(&wbest.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        }
    }
    
    /*
     * copy WBest to output
     */
    ae_v_move(&network->weights.ptr.p_double[0], 1, &wbest.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
    ae_frame_leave(_state);
}


/*************************************************************************
Neural  network  training  using  L-BFGS  algorithm  with  regularization.
Subroutine  trains  neural  network  with  restarts from random positions.
Algorithm  is  well  suited  for  problems  of  any dimensionality (memory
requirements and step complexity are linear by weights number).

INPUT PARAMETERS:
    Network     -   neural network with initialized geometry
    XY          -   training set
    NPoints     -   training set size
    Decay       -   weight decay constant, >=0.001
                    Decay term 'Decay*||Weights||^2' is added to error
                    function.
                    If you don't know what Decay to choose, use 0.001.
    Restarts    -   number of restarts from random position, >0.
                    If you don't know what Restarts to choose, use 2.
    WStep       -   stopping criterion. Algorithm stops if  step  size  is
                    less than WStep. Recommended value - 0.01.  Zero  step
                    size means stopping after MaxIts iterations.
    MaxIts      -   stopping   criterion.  Algorithm  stops  after  MaxIts
                    iterations (NOT gradient  calculations).  Zero  MaxIts
                    means stopping when step is sufficiently small.

OUTPUT PARAMETERS:
    Network     -   trained neural network.
    Info        -   return code:
                    * -8, if both WStep=0 and MaxIts=0
                    * -2, if there is a point with class number
                          outside of [0..NOut-1].
                    * -1, if wrong parameters specified
                          (NPoints<0, Restarts<1).
                    *  2, if task has been solved.
    Rep         -   training report

  -- ALGLIB --
     Copyright 09.12.2007 by Bochkanov Sergey
*************************************************************************/
void mlptrainlbfgs(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     double decay,
     ae_int_t restarts,
     double wstep,
     ae_int_t maxits,
     ae_int_t* info,
     mlpreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t pass;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_vector w;
    ae_vector wbest;
    double e;
    double v;
    double ebest;
    minlbfgsreport internalrep;
    minlbfgsstate state;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _mlpreport_clear(rep);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&wbest, 0, DT_REAL, _state);
    _minlbfgsreport_init(&internalrep, _state);
    _minlbfgsstate_init(&state, _state);

    
    /*
     * Test inputs, parse flags, read network geometry
     */
    if( ae_fp_eq(wstep,(double)(0))&&maxits==0 )
    {
        *info = -8;
        ae_frame_leave(_state);
        return;
    }
    if( ((npoints<=0||restarts<1)||ae_fp_less(wstep,(double)(0)))||maxits<0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    mlpproperties(network, &nin, &nout, &wcount, _state);
    if( mlpissoftmax(network, _state) )
    {
        for(i=0; i<=npoints-1; i++)
        {
            if( ae_round(xy->ptr.pp_double[i][nin], _state)<0||ae_round(xy->ptr.pp_double[i][nin], _state)>=nout )
            {
                *info = -2;
                ae_frame_leave(_state);
                return;
            }
        }
    }
    decay = ae_maxreal(decay, mlptrain_mindecay, _state);
    *info = 2;
    
    /*
     * Prepare
     */
    mlpinitpreprocessor(network, xy, npoints, _state);
    ae_vector_set_length(&w, wcount-1+1, _state);
    ae_vector_set_length(&wbest, wcount-1+1, _state);
    ebest = ae_maxrealnumber;
    
    /*
     * Multiple starts
     */
    rep->ncholesky = 0;
    rep->nhess = 0;
    rep->ngrad = 0;
    for(pass=1; pass<=restarts; pass++)
    {
        
        /*
         * Process
         */
        mlprandomize(network, _state);
        ae_v_move(&w.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        minlbfgscreate(wcount, ae_minint(wcount, 10, _state), &w, &state, _state);
        minlbfgssetcond(&state, 0.0, 0.0, wstep, maxits, _state);
        while(minlbfgsiteration(&state, _state))
        {
            ae_v_move(&network->weights.ptr.p_double[0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            mlpgradnbatch(network, xy, npoints, &state.f, &state.g, _state);
            v = ae_v_dotproduct(&network->weights.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            state.f = state.f+0.5*decay*v;
            ae_v_addd(&state.g.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1), decay);
            rep->ngrad = rep->ngrad+1;
        }
        minlbfgsresults(&state, &w, &internalrep, _state);
        ae_v_move(&network->weights.ptr.p_double[0], 1, &w.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        
        /*
         * Compare with best
         */
        v = ae_v_dotproduct(&network->weights.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        e = mlperrorn(network, xy, npoints, _state)+0.5*decay*v;
        if( ae_fp_less(e,ebest) )
        {
            ae_v_move(&wbest.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            ebest = e;
        }
    }
    
    /*
     * The best network
     */
    ae_v_move(&network->weights.ptr.p_double[0], 1, &wbest.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
    ae_frame_leave(_state);
}


/*************************************************************************
Neural network training using early stopping (base algorithm - L-BFGS with
regularization).

INPUT PARAMETERS:
    Network     -   neural network with initialized geometry
    TrnXY       -   training set
    TrnSize     -   training set size, TrnSize>0
    ValXY       -   validation set
    ValSize     -   validation set size, ValSize>0
    Decay       -   weight decay constant, >=0.001
                    Decay term 'Decay*||Weights||^2' is added to error
                    function.
                    If you don't know what Decay to choose, use 0.001.
    Restarts    -   number of restarts, either:
                    * strictly positive number - algorithm make specified
                      number of restarts from random position.
                    * -1, in which case algorithm makes exactly one run
                      from the initial state of the network (no randomization).
                    If you don't know what Restarts to choose, choose one
                    one the following:
                    * -1 (deterministic start)
                    * +1 (one random restart)
                    * +5 (moderate amount of random restarts)

OUTPUT PARAMETERS:
    Network     -   trained neural network.
    Info        -   return code:
                    * -2, if there is a point with class number
                          outside of [0..NOut-1].
                    * -1, if wrong parameters specified
                          (NPoints<0, Restarts<1, ...).
                    *  2, task has been solved, stopping  criterion  met -
                          sufficiently small step size.  Not expected  (we
                          use  EARLY  stopping)  but  possible  and not an
                          error.
                    *  6, task has been solved, stopping  criterion  met -
                          increasing of validation set error.
    Rep         -   training report

NOTE:

Algorithm stops if validation set error increases for  a  long  enough  or
step size is small enought  (there  are  task  where  validation  set  may
decrease for eternity). In any case solution returned corresponds  to  the
minimum of validation set error.

  -- ALGLIB --
     Copyright 10.03.2009 by Bochkanov Sergey
*************************************************************************/
void mlptraines(multilayerperceptron* network,
     /* Real    */ ae_matrix* trnxy,
     ae_int_t trnsize,
     /* Real    */ ae_matrix* valxy,
     ae_int_t valsize,
     double decay,
     ae_int_t restarts,
     ae_int_t* info,
     mlpreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t pass;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_vector w;
    ae_vector wbest;
    double e;
    double v;
    double ebest;
    ae_vector wfinal;
    double efinal;
    ae_int_t itcnt;
    ae_int_t itbest;
    minlbfgsreport internalrep;
    minlbfgsstate state;
    double wstep;
    ae_bool needrandomization;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _mlpreport_clear(rep);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&wbest, 0, DT_REAL, _state);
    ae_vector_init(&wfinal, 0, DT_REAL, _state);
    _minlbfgsreport_init(&internalrep, _state);
    _minlbfgsstate_init(&state, _state);

    wstep = 0.001;
    
    /*
     * Test inputs, parse flags, read network geometry
     */
    if( ((trnsize<=0||valsize<=0)||(restarts<1&&restarts!=-1))||ae_fp_less(decay,(double)(0)) )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    if( restarts==-1 )
    {
        needrandomization = ae_false;
        restarts = 1;
    }
    else
    {
        needrandomization = ae_true;
    }
    mlpproperties(network, &nin, &nout, &wcount, _state);
    if( mlpissoftmax(network, _state) )
    {
        for(i=0; i<=trnsize-1; i++)
        {
            if( ae_round(trnxy->ptr.pp_double[i][nin], _state)<0||ae_round(trnxy->ptr.pp_double[i][nin], _state)>=nout )
            {
                *info = -2;
                ae_frame_leave(_state);
                return;
            }
        }
        for(i=0; i<=valsize-1; i++)
        {
            if( ae_round(valxy->ptr.pp_double[i][nin], _state)<0||ae_round(valxy->ptr.pp_double[i][nin], _state)>=nout )
            {
                *info = -2;
                ae_frame_leave(_state);
                return;
            }
        }
    }
    *info = 2;
    
    /*
     * Prepare
     */
    mlpinitpreprocessor(network, trnxy, trnsize, _state);
    ae_vector_set_length(&w, wcount-1+1, _state);
    ae_vector_set_length(&wbest, wcount-1+1, _state);
    ae_vector_set_length(&wfinal, wcount-1+1, _state);
    efinal = ae_maxrealnumber;
    for(i=0; i<=wcount-1; i++)
    {
        wfinal.ptr.p_double[i] = (double)(0);
    }
    
    /*
     * Multiple starts
     */
    rep->ncholesky = 0;
    rep->nhess = 0;
    rep->ngrad = 0;
    for(pass=1; pass<=restarts; pass++)
    {
        
        /*
         * Process
         */
        if( needrandomization )
        {
            mlprandomize(network, _state);
        }
        ebest = mlperror(network, valxy, valsize, _state);
        ae_v_move(&wbest.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        itbest = 0;
        itcnt = 0;
        ae_v_move(&w.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        minlbfgscreate(wcount, ae_minint(wcount, 10, _state), &w, &state, _state);
        minlbfgssetcond(&state, 0.0, 0.0, wstep, 0, _state);
        minlbfgssetxrep(&state, ae_true, _state);
        while(minlbfgsiteration(&state, _state))
        {
            
            /*
             * Calculate gradient
             */
            if( state.needfg )
            {
                ae_v_move(&network->weights.ptr.p_double[0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
                mlpgradnbatch(network, trnxy, trnsize, &state.f, &state.g, _state);
                v = ae_v_dotproduct(&network->weights.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
                state.f = state.f+0.5*decay*v;
                ae_v_addd(&state.g.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1), decay);
                rep->ngrad = rep->ngrad+1;
            }
            
            /*
             * Validation set
             */
            if( state.xupdated )
            {
                ae_v_move(&network->weights.ptr.p_double[0], 1, &state.x.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
                e = mlperror(network, valxy, valsize, _state);
                if( ae_fp_less(e,ebest) )
                {
                    ebest = e;
                    ae_v_move(&wbest.ptr.p_double[0], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
                    itbest = itcnt;
                }
                if( itcnt>30&&ae_fp_greater((double)(itcnt),1.5*itbest) )
                {
                    *info = 6;
                    break;
                }
                itcnt = itcnt+1;
            }
        }
        minlbfgsresults(&state, &w, &internalrep, _state);
        
        /*
         * Compare with final answer
         */
        if( ae_fp_less(ebest,efinal) )
        {
            ae_v_move(&wfinal.ptr.p_double[0], 1, &wbest.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            efinal = ebest;
        }
    }
    
    /*
     * The best network
     */
    ae_v_move(&network->weights.ptr.p_double[0], 1, &wfinal.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
    ae_frame_leave(_state);
}


/*************************************************************************
Cross-validation estimate of generalization error.

Base algorithm - L-BFGS.

INPUT PARAMETERS:
    Network     -   neural network with initialized geometry.   Network is
                    not changed during cross-validation -  it is used only
                    as a representative of its architecture.
    XY          -   training set.
    SSize       -   training set size
    Decay       -   weight  decay, same as in MLPTrainLBFGS
    Restarts    -   number of restarts, >0.
                    restarts are counted for each partition separately, so
                    total number of restarts will be Restarts*FoldsCount.
    WStep       -   stopping criterion, same as in MLPTrainLBFGS
    MaxIts      -   stopping criterion, same as in MLPTrainLBFGS
    FoldsCount  -   number of folds in k-fold cross-validation,
                    2<=FoldsCount<=SSize.
                    recommended value: 10.

OUTPUT PARAMETERS:
    Info        -   return code, same as in MLPTrainLBFGS
    Rep         -   report, same as in MLPTrainLM/MLPTrainLBFGS
    CVRep       -   generalization error estimates

  -- ALGLIB --
     Copyright 09.12.2007 by Bochkanov Sergey
*************************************************************************/
void mlpkfoldcvlbfgs(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     double decay,
     ae_int_t restarts,
     double wstep,
     ae_int_t maxits,
     ae_int_t foldscount,
     ae_int_t* info,
     mlpreport* rep,
     mlpcvreport* cvrep,
     ae_state *_state)
{

    *info = 0;
    _mlpreport_clear(rep);
    _mlpcvreport_clear(cvrep);

    mlptrain_mlpkfoldcvgeneral(network, xy, npoints, decay, restarts, foldscount, ae_false, wstep, maxits, info, rep, cvrep, _state);
}


/*************************************************************************
Cross-validation estimate of generalization error.

Base algorithm - Levenberg-Marquardt.

INPUT PARAMETERS:
    Network     -   neural network with initialized geometry.   Network is
                    not changed during cross-validation -  it is used only
                    as a representative of its architecture.
    XY          -   training set.
    SSize       -   training set size
    Decay       -   weight  decay, same as in MLPTrainLBFGS
    Restarts    -   number of restarts, >0.
                    restarts are counted for each partition separately, so
                    total number of restarts will be Restarts*FoldsCount.
    FoldsCount  -   number of folds in k-fold cross-validation,
                    2<=FoldsCount<=SSize.
                    recommended value: 10.

OUTPUT PARAMETERS:
    Info        -   return code, same as in MLPTrainLBFGS
    Rep         -   report, same as in MLPTrainLM/MLPTrainLBFGS
    CVRep       -   generalization error estimates

  -- ALGLIB --
     Copyright 09.12.2007 by Bochkanov Sergey
*************************************************************************/
void mlpkfoldcvlm(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     double decay,
     ae_int_t restarts,
     ae_int_t foldscount,
     ae_int_t* info,
     mlpreport* rep,
     mlpcvreport* cvrep,
     ae_state *_state)
{

    *info = 0;
    _mlpreport_clear(rep);
    _mlpcvreport_clear(cvrep);

    mlptrain_mlpkfoldcvgeneral(network, xy, npoints, decay, restarts, foldscount, ae_true, 0.0, 0, info, rep, cvrep, _state);
}


/*************************************************************************
This function estimates generalization error using cross-validation on the
current dataset with current training settings.

FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support (C++ computational core)
  !
  ! Second improvement gives constant  speedup (2-3X).  First  improvement
  ! gives  close-to-linear  speedup  on   multicore   systems.   Following
  ! operations can be executed in parallel:
  ! * FoldsCount cross-validation rounds (always)
  ! * NRestarts training sessions performed within each of
  !   cross-validation rounds (if NRestarts>1)
  ! * gradient calculation over large dataset (if dataset is large enough)
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! In order to use SSE features you have to:
  ! * use commercial version of ALGLIB on Intel processors
  ! * use C++ computational core
  !
  ! This note is given for users of commercial edition; if  you  use  GPL
  ! edition, you still will be able to call smp-version of this function,
  ! but all computations will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.

INPUT PARAMETERS:
    S           -   trainer object
    Network     -   neural network. It must have same number of inputs and
                    output/classes as was specified during creation of the
                    trainer object. Network is not changed  during  cross-
                    validation and is not trained - it  is  used  only  as
                    representative of its architecture. I.e., we  estimate
                    generalization properties of  ARCHITECTURE,  not  some
                    specific network.
    NRestarts   -   number of restarts, >=0:
                    * NRestarts>0  means  that  for  each cross-validation
                      round   specified  number   of  random  restarts  is
                      performed,  with  best  network  being  chosen after
                      training.
                    * NRestarts=0 is same as NRestarts=1
    FoldsCount  -   number of folds in k-fold cross-validation:
                    * 2<=FoldsCount<=size of dataset
                    * recommended value: 10.
                    * values larger than dataset size will be silently
                      truncated down to dataset size

OUTPUT PARAMETERS:
    Rep         -   structure which contains cross-validation estimates:
                    * Rep.RelCLSError - fraction of misclassified cases.
                    * Rep.AvgCE - acerage cross-entropy
                    * Rep.RMSError - root-mean-square error
                    * Rep.AvgError - average error
                    * Rep.AvgRelError - average relative error
                    
NOTE: when no dataset was specified with MLPSetDataset/SetSparseDataset(),
      or subset with only one point  was  given,  zeros  are  returned  as
      estimates.

NOTE: this method performs FoldsCount cross-validation  rounds,  each  one
      with NRestarts random starts.  Thus,  FoldsCount*NRestarts  networks
      are trained in total.

NOTE: Rep.RelCLSError/Rep.AvgCE are zero on regression problems.

NOTE: on classification problems Rep.RMSError/Rep.AvgError/Rep.AvgRelError
      contain errors in prediction of posterior probabilities.
        
  -- ALGLIB --
     Copyright 23.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlpkfoldcv(mlptrainer* s,
     multilayerperceptron* network,
     ae_int_t nrestarts,
     ae_int_t foldscount,
     mlpreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_shared_pool pooldatacv;
    mlpparallelizationcv datacv;
    mlpparallelizationcv *sdatacv;
    ae_smart_ptr _sdatacv;
    ae_matrix cvy;
    ae_vector folds;
    ae_vector buf;
    ae_vector dy;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t rowsize;
    ae_int_t ntype;
    ae_int_t ttype;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    _mlpreport_clear(rep);
    ae_shared_pool_init(&pooldatacv, _state);
    _mlpparallelizationcv_init(&datacv, _state);
    ae_smart_ptr_init(&_sdatacv, (void**)&sdatacv, _state);
    ae_matrix_init(&cvy, 0, 0, DT_REAL, _state);
    ae_vector_init(&folds, 0, DT_INT, _state);
    ae_vector_init(&buf, 0, DT_REAL, _state);
    ae_vector_init(&dy, 0, DT_REAL, _state);
    _hqrndstate_init(&rs, _state);

    if( !mlpissoftmax(network, _state) )
    {
        ntype = 0;
    }
    else
    {
        ntype = 1;
    }
    if( s->rcpar )
    {
        ttype = 0;
    }
    else
    {
        ttype = 1;
    }
    ae_assert(ntype==ttype, "MLPKFoldCV: type of input network is not similar to network type in trainer object", _state);
    ae_assert(s->npoints>=0, "MLPKFoldCV: possible trainer S is not initialized(S.NPoints<0)", _state);
    mlpproperties(network, &nin, &nout, &wcount, _state);
    ae_assert(s->nin==nin, "MLPKFoldCV:  number of inputs in trainer is not equal to number of inputs in network", _state);
    ae_assert(s->nout==nout, "MLPKFoldCV:  number of outputs in trainer is not equal to number of outputs in network", _state);
    ae_assert(nrestarts>=0, "MLPKFoldCV: NRestarts<0", _state);
    ae_assert(foldscount>=2, "MLPKFoldCV: FoldsCount<2", _state);
    if( foldscount>s->npoints )
    {
        foldscount = s->npoints;
    }
    rep->relclserror = (double)(0);
    rep->avgce = (double)(0);
    rep->rmserror = (double)(0);
    rep->avgerror = (double)(0);
    rep->avgrelerror = (double)(0);
    hqrndrandomize(&rs, _state);
    rep->ngrad = 0;
    rep->nhess = 0;
    rep->ncholesky = 0;
    if( s->npoints==0||s->npoints==1 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Read network geometry, test parameters
     */
    if( s->rcpar )
    {
        rowsize = nin+nout;
        ae_vector_set_length(&dy, nout, _state);
        dserrallocate(-nout, &buf, _state);
    }
    else
    {
        rowsize = nin+1;
        ae_vector_set_length(&dy, 1, _state);
        dserrallocate(nout, &buf, _state);
    }
    
    /*
     * Folds
     */
    ae_vector_set_length(&folds, s->npoints, _state);
    for(i=0; i<=s->npoints-1; i++)
    {
        folds.ptr.p_int[i] = i*foldscount/s->npoints;
    }
    for(i=0; i<=s->npoints-2; i++)
    {
        j = i+hqrnduniformi(&rs, s->npoints-i, _state);
        if( j!=i )
        {
            k = folds.ptr.p_int[i];
            folds.ptr.p_int[i] = folds.ptr.p_int[j];
            folds.ptr.p_int[j] = k;
        }
    }
    ae_matrix_set_length(&cvy, s->npoints, nout, _state);
    
    /*
     * Initialize SEED-value for shared pool
     */
    datacv.ngrad = 0;
    mlpcopy(network, &datacv.network, _state);
    ae_vector_set_length(&datacv.subset, s->npoints, _state);
    ae_vector_set_length(&datacv.xyrow, rowsize, _state);
    ae_vector_set_length(&datacv.y, nout, _state);
    
    /*
     * Create shared pool
     */
    ae_shared_pool_set_seed(&pooldatacv, &datacv, sizeof(datacv), _mlpparallelizationcv_init, _mlpparallelizationcv_init_copy, _mlpparallelizationcv_destroy, _state);
    
    /*
     * Parallelization
     */
    mlptrain_mthreadcv(s, rowsize, nrestarts, &folds, 0, foldscount, &cvy, &pooldatacv, _state);
    
    /*
     * Calculate value for NGrad
     */
    ae_shared_pool_first_recycled(&pooldatacv, &_sdatacv, _state);
    while(sdatacv!=NULL)
    {
        rep->ngrad = rep->ngrad+sdatacv->ngrad;
        ae_shared_pool_next_recycled(&pooldatacv, &_sdatacv, _state);
    }
    
    /*
     * Connect of results and calculate cross-validation error
     */
    for(i=0; i<=s->npoints-1; i++)
    {
        if( s->datatype==0 )
        {
            ae_v_move(&datacv.xyrow.ptr.p_double[0], 1, &s->densexy.ptr.pp_double[i][0], 1, ae_v_len(0,rowsize-1));
        }
        if( s->datatype==1 )
        {
            sparsegetrow(&s->sparsexy, i, &datacv.xyrow, _state);
        }
        ae_v_move(&datacv.y.ptr.p_double[0], 1, &cvy.ptr.pp_double[i][0], 1, ae_v_len(0,nout-1));
        if( s->rcpar )
        {
            ae_v_move(&dy.ptr.p_double[0], 1, &datacv.xyrow.ptr.p_double[nin], 1, ae_v_len(0,nout-1));
        }
        else
        {
            dy.ptr.p_double[0] = datacv.xyrow.ptr.p_double[nin];
        }
        dserraccumulate(&buf, &datacv.y, &dy, _state);
    }
    dserrfinish(&buf, _state);
    rep->relclserror = buf.ptr.p_double[0];
    rep->avgce = buf.ptr.p_double[1];
    rep->rmserror = buf.ptr.p_double[2];
    rep->avgerror = buf.ptr.p_double[3];
    rep->avgrelerror = buf.ptr.p_double[4];
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_mlpkfoldcv(mlptrainer* s,
    multilayerperceptron* network,
    ae_int_t nrestarts,
    ae_int_t foldscount,
    mlpreport* rep, ae_state *_state)
{
    mlpkfoldcv(s,network,nrestarts,foldscount,rep, _state);
}


/*************************************************************************
Creation of the network trainer object for regression networks

INPUT PARAMETERS:
    NIn         -   number of inputs, NIn>=1
    NOut        -   number of outputs, NOut>=1

OUTPUT PARAMETERS:
    S           -   neural network trainer object.
                    This structure can be used to train any regression
                    network with NIn inputs and NOut outputs.

  -- ALGLIB --
     Copyright 23.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlpcreatetrainer(ae_int_t nin,
     ae_int_t nout,
     mlptrainer* s,
     ae_state *_state)
{

    _mlptrainer_clear(s);

    ae_assert(nin>=1, "MLPCreateTrainer: NIn<1.", _state);
    ae_assert(nout>=1, "MLPCreateTrainer: NOut<1.", _state);
    s->nin = nin;
    s->nout = nout;
    s->rcpar = ae_true;
    s->lbfgsfactor = mlptrain_defaultlbfgsfactor;
    s->decay = 1.0E-6;
    mlpsetcond(s, (double)(0), 0, _state);
    s->datatype = 0;
    s->npoints = 0;
    mlpsetalgobatch(s, _state);
}


/*************************************************************************
Creation of the network trainer object for classification networks

INPUT PARAMETERS:
    NIn         -   number of inputs, NIn>=1
    NClasses    -   number of classes, NClasses>=2

OUTPUT PARAMETERS:
    S           -   neural network trainer object.
                    This structure can be used to train any classification
                    network with NIn inputs and NOut outputs.

  -- ALGLIB --
     Copyright 23.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlpcreatetrainercls(ae_int_t nin,
     ae_int_t nclasses,
     mlptrainer* s,
     ae_state *_state)
{

    _mlptrainer_clear(s);

    ae_assert(nin>=1, "MLPCreateTrainerCls: NIn<1.", _state);
    ae_assert(nclasses>=2, "MLPCreateTrainerCls: NClasses<2.", _state);
    s->nin = nin;
    s->nout = nclasses;
    s->rcpar = ae_false;
    s->lbfgsfactor = mlptrain_defaultlbfgsfactor;
    s->decay = 1.0E-6;
    mlpsetcond(s, (double)(0), 0, _state);
    s->datatype = 0;
    s->npoints = 0;
    mlpsetalgobatch(s, _state);
}


/*************************************************************************
This function sets "current dataset" of the trainer object to  one  passed
by user.

INPUT PARAMETERS:
    S           -   trainer object
    XY          -   training  set,  see  below  for  information  on   the
                    training set format. This function checks  correctness
                    of  the  dataset  (no  NANs/INFs,  class  numbers  are
                    correct) and throws exception when  incorrect  dataset
                    is passed.
    NPoints     -   points count, >=0.

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
datasetformat is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).
  
  -- ALGLIB --
     Copyright 23.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlpsetdataset(mlptrainer* s,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_int_t ndim;
    ae_int_t i;
    ae_int_t j;


    ae_assert(s->nin>=1, "MLPSetDataset: possible parameter S is not initialized or spoiled(S.NIn<=0).", _state);
    ae_assert(npoints>=0, "MLPSetDataset: NPoint<0", _state);
    ae_assert(npoints<=xy->rows, "MLPSetDataset: invalid size of matrix XY(NPoint more then rows of matrix XY)", _state);
    s->datatype = 0;
    s->npoints = npoints;
    if( npoints==0 )
    {
        return;
    }
    if( s->rcpar )
    {
        ae_assert(s->nout>=1, "MLPSetDataset: possible parameter S is not initialized or is spoiled(NOut<1 for regression).", _state);
        ndim = s->nin+s->nout;
        ae_assert(ndim<=xy->cols, "MLPSetDataset: invalid size of matrix XY(too few columns in matrix XY).", _state);
        ae_assert(apservisfinitematrix(xy, npoints, ndim, _state), "MLPSetDataset: parameter XY contains Infinite or NaN.", _state);
    }
    else
    {
        ae_assert(s->nout>=2, "MLPSetDataset: possible parameter S is not initialized or is spoiled(NClasses<2 for classifier).", _state);
        ndim = s->nin+1;
        ae_assert(ndim<=xy->cols, "MLPSetDataset: invalid size of matrix XY(too few columns in matrix XY).", _state);
        ae_assert(apservisfinitematrix(xy, npoints, ndim, _state), "MLPSetDataset: parameter XY contains Infinite or NaN.", _state);
        for(i=0; i<=npoints-1; i++)
        {
            ae_assert(ae_round(xy->ptr.pp_double[i][s->nin], _state)>=0&&ae_round(xy->ptr.pp_double[i][s->nin], _state)<s->nout, "MLPSetDataset: invalid parameter XY(in classifier used nonexistent class number: either XY[.,NIn]<0 or XY[.,NIn]>=NClasses).", _state);
        }
    }
    rmatrixsetlengthatleast(&s->densexy, npoints, ndim, _state);
    for(i=0; i<=npoints-1; i++)
    {
        for(j=0; j<=ndim-1; j++)
        {
            s->densexy.ptr.pp_double[i][j] = xy->ptr.pp_double[i][j];
        }
    }
}


/*************************************************************************
This function sets "current dataset" of the trainer object to  one  passed
by user (sparse matrix is used to store dataset).

INPUT PARAMETERS:
    S           -   trainer object
    XY          -   training  set,  see  below  for  information  on   the
                    training set format. This function checks  correctness
                    of  the  dataset  (no  NANs/INFs,  class  numbers  are
                    correct) and throws exception when  incorrect  dataset
                    is passed. Any  sparse  storage  format  can be  used:
                    Hash-table, CRS...
    NPoints     -   points count, >=0

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
datasetformat is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).
  
  -- ALGLIB --
     Copyright 23.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlpsetsparsedataset(mlptrainer* s,
     sparsematrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double v;
    ae_int_t t0;
    ae_int_t t1;
    ae_int_t i;
    ae_int_t j;


    
    /*
     * Check correctness of the data
     */
    ae_assert(s->nin>0, "MLPSetSparseDataset: possible parameter S is not initialized or spoiled(S.NIn<=0).", _state);
    ae_assert(npoints>=0, "MLPSetSparseDataset: NPoint<0", _state);
    ae_assert(npoints<=sparsegetnrows(xy, _state), "MLPSetSparseDataset: invalid size of sparse matrix XY(NPoint more then rows of matrix XY)", _state);
    if( npoints>0 )
    {
        t0 = 0;
        t1 = 0;
        if( s->rcpar )
        {
            ae_assert(s->nout>=1, "MLPSetSparseDataset: possible parameter S is not initialized or is spoiled(NOut<1 for regression).", _state);
            ae_assert(s->nin+s->nout<=sparsegetncols(xy, _state), "MLPSetSparseDataset: invalid size of sparse matrix XY(too few columns in sparse matrix XY).", _state);
            while(sparseenumerate(xy, &t0, &t1, &i, &j, &v, _state))
            {
                if( i<npoints&&j<s->nin+s->nout )
                {
                    ae_assert(ae_isfinite(v, _state), "MLPSetSparseDataset: sparse matrix XY contains Infinite or NaN.", _state);
                }
            }
        }
        else
        {
            ae_assert(s->nout>=2, "MLPSetSparseDataset: possible parameter S is not initialized or is spoiled(NClasses<2 for classifier).", _state);
            ae_assert(s->nin+1<=sparsegetncols(xy, _state), "MLPSetSparseDataset: invalid size of sparse matrix XY(too few columns in sparse matrix XY).", _state);
            while(sparseenumerate(xy, &t0, &t1, &i, &j, &v, _state))
            {
                if( i<npoints&&j<=s->nin )
                {
                    if( j!=s->nin )
                    {
                        ae_assert(ae_isfinite(v, _state), "MLPSetSparseDataset: sparse matrix XY contains Infinite or NaN.", _state);
                    }
                    else
                    {
                        ae_assert((ae_isfinite(v, _state)&&ae_round(v, _state)>=0)&&ae_round(v, _state)<s->nout, "MLPSetSparseDataset: invalid sparse matrix XY(in classifier used nonexistent class number: either XY[.,NIn]<0 or XY[.,NIn]>=NClasses).", _state);
                    }
                }
            }
        }
    }
    
    /*
     * Set dataset
     */
    s->datatype = 1;
    s->npoints = npoints;
    sparsecopytocrs(xy, &s->sparsexy, _state);
}


/*************************************************************************
This function sets weight decay coefficient which is used for training.

INPUT PARAMETERS:
    S           -   trainer object
    Decay       -   weight  decay  coefficient,  >=0.  Weight  decay  term
                    'Decay*||Weights||^2' is added to error  function.  If
                    you don't know what Decay to choose, use 1.0E-3.
                    Weight decay can be set to zero,  in this case network
                    is trained without weight decay.

NOTE: by default network uses some small nonzero value for weight decay.

  -- ALGLIB --
     Copyright 23.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlpsetdecay(mlptrainer* s, double decay, ae_state *_state)
{


    ae_assert(ae_isfinite(decay, _state), "MLPSetDecay: parameter Decay contains Infinite or NaN.", _state);
    ae_assert(ae_fp_greater_eq(decay,(double)(0)), "MLPSetDecay: Decay<0.", _state);
    s->decay = decay;
}


/*************************************************************************
This function sets stopping criteria for the optimizer.

INPUT PARAMETERS:
    S           -   trainer object
    WStep       -   stopping criterion. Algorithm stops if  step  size  is
                    less than WStep. Recommended value - 0.01.  Zero  step
                    size means stopping after MaxIts iterations.
                    WStep>=0.
    MaxIts      -   stopping   criterion.  Algorithm  stops  after  MaxIts
                    epochs (full passes over entire dataset).  Zero MaxIts
                    means stopping when step is sufficiently small.
                    MaxIts>=0.

NOTE: by default, WStep=0.005 and MaxIts=0 are used. These values are also
      used when MLPSetCond() is called with WStep=0 and MaxIts=0.
      
NOTE: these stopping criteria are used for all kinds of neural training  -
      from "conventional" networks to early stopping ensembles. When  used
      for "conventional" networks, they are  used  as  the  only  stopping
      criteria. When combined with early stopping, they used as ADDITIONAL
      stopping criteria which can terminate early stopping algorithm.

  -- ALGLIB --
     Copyright 23.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlpsetcond(mlptrainer* s,
     double wstep,
     ae_int_t maxits,
     ae_state *_state)
{


    ae_assert(ae_isfinite(wstep, _state), "MLPSetCond: parameter WStep contains Infinite or NaN.", _state);
    ae_assert(ae_fp_greater_eq(wstep,(double)(0)), "MLPSetCond: WStep<0.", _state);
    ae_assert(maxits>=0, "MLPSetCond: MaxIts<0.", _state);
    if( ae_fp_neq(wstep,(double)(0))||maxits!=0 )
    {
        s->wstep = wstep;
        s->maxits = maxits;
    }
    else
    {
        s->wstep = 0.005;
        s->maxits = 0;
    }
}


/*************************************************************************
This function sets training algorithm: batch training using L-BFGS will be
used.

This algorithm:
* the most robust for small-scale problems, but may be too slow for  large
  scale ones.
* perfoms full pass through the dataset before performing step
* uses conditions specified by MLPSetCond() for stopping
* is default one used by trainer object

INPUT PARAMETERS:
    S           -   trainer object

  -- ALGLIB --
     Copyright 23.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlpsetalgobatch(mlptrainer* s, ae_state *_state)
{


    s->algokind = 0;
}


/*************************************************************************
This function trains neural network passed to this function, using current
dataset (one which was passed to MLPSetDataset() or MLPSetSparseDataset())
and current training settings. Training  from  NRestarts  random  starting
positions is performed, best network is chosen.

Training is performed using current training algorithm.

FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support (C++ computational core)
  !
  ! Second improvement gives constant  speedup (2-3X).  First  improvement
  ! gives  close-to-linear  speedup  on   multicore   systems.   Following
  ! operations can be executed in parallel:
  ! * NRestarts training sessions performed within each of
  !   cross-validation rounds (if NRestarts>1)
  ! * gradient calculation over large dataset (if dataset is large enough)
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! In order to use SSE features you have to:
  ! * use commercial version of ALGLIB on Intel processors
  ! * use C++ computational core
  !
  ! This note is given for users of commercial edition; if  you  use  GPL
  ! edition, you still will be able to call smp-version of this function,
  ! but all computations will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.

INPUT PARAMETERS:
    S           -   trainer object
    Network     -   neural network. It must have same number of inputs and
                    output/classes as was specified during creation of the
                    trainer object.
    NRestarts   -   number of restarts, >=0:
                    * NRestarts>0 means that specified  number  of  random
                      restarts are performed, best network is chosen after
                      training
                    * NRestarts=0 means that current state of the  network
                      is used for training.

OUTPUT PARAMETERS:
    Network     -   trained network

NOTE: when no dataset was specified with MLPSetDataset/SetSparseDataset(),
      network  is  filled  by zero  values.  Same  behavior  for functions
      MLPStartTraining and MLPContinueTraining.

NOTE: this method uses sum-of-squares error function for training.

  -- ALGLIB --
     Copyright 23.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlptrainnetwork(mlptrainer* s,
     multilayerperceptron* network,
     ae_int_t nrestarts,
     mlpreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t ntype;
    ae_int_t ttype;
    ae_shared_pool trnpool;

    ae_frame_make(_state, &_frame_block);
    _mlpreport_clear(rep);
    ae_shared_pool_init(&trnpool, _state);

    ae_assert(s->npoints>=0, "MLPTrainNetwork: parameter S is not initialized or is spoiled(S.NPoints<0)", _state);
    if( !mlpissoftmax(network, _state) )
    {
        ntype = 0;
    }
    else
    {
        ntype = 1;
    }
    if( s->rcpar )
    {
        ttype = 0;
    }
    else
    {
        ttype = 1;
    }
    ae_assert(ntype==ttype, "MLPTrainNetwork: type of input network is not similar to network type in trainer object", _state);
    mlpproperties(network, &nin, &nout, &wcount, _state);
    ae_assert(s->nin==nin, "MLPTrainNetwork: number of inputs in trainer is not equal to number of inputs in network", _state);
    ae_assert(s->nout==nout, "MLPTrainNetwork: number of outputs in trainer is not equal to number of outputs in network", _state);
    ae_assert(nrestarts>=0, "MLPTrainNetwork: NRestarts<0.", _state);
    
    /*
     * Train
     */
    mlptrain_mlptrainnetworkx(s, nrestarts, -1, &s->subset, -1, &s->subset, 0, network, rep, ae_true, &trnpool, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_mlptrainnetwork(mlptrainer* s,
    multilayerperceptron* network,
    ae_int_t nrestarts,
    mlpreport* rep, ae_state *_state)
{
    mlptrainnetwork(s,network,nrestarts,rep, _state);
}


/*************************************************************************
IMPORTANT: this is an "expert" version of the MLPTrain() function.  We  do
           not recommend you to use it unless you are pretty sure that you
           need ability to monitor training progress.

This function performs step-by-step training of the neural  network.  Here
"step-by-step" means that training  starts  with  MLPStartTraining() call,
and then user subsequently calls MLPContinueTraining() to perform one more
iteration of the training.

After call to this function trainer object remembers network and  is ready
to  train  it.  However,  no  training  is  performed  until first call to 
MLPContinueTraining() function. Subsequent calls  to MLPContinueTraining()
will advance training progress one iteration further.

EXAMPLE:
    >
    > ...initialize network and trainer object....
    >
    > MLPStartTraining(Trainer, Network, True)
    > while MLPContinueTraining(Trainer, Network) do
    >     ...visualize training progress...
    >

INPUT PARAMETERS:
    S           -   trainer object
    Network     -   neural network. It must have same number of inputs and
                    output/classes as was specified during creation of the
                    trainer object.
    RandomStart -   randomize network before training or not:
                    * True  means  that  network  is  randomized  and  its
                      initial state (one which was passed to  the  trainer
                      object) is lost.
                    * False  means  that  training  is  started  from  the
                      current state of the network
                    
OUTPUT PARAMETERS:
    Network     -   neural network which is ready to training (weights are
                    initialized, preprocessor is initialized using current
                    training set)

NOTE: this method uses sum-of-squares error function for training.

NOTE: it is expected that trainer object settings are NOT  changed  during
      step-by-step training, i.e. no  one  changes  stopping  criteria  or
      training set during training. It is possible and there is no defense
      against  such  actions,  but  algorithm  behavior  in  such cases is
      undefined and can be unpredictable.

  -- ALGLIB --
     Copyright 23.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlpstarttraining(mlptrainer* s,
     multilayerperceptron* network,
     ae_bool randomstart,
     ae_state *_state)
{
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t ntype;
    ae_int_t ttype;


    ae_assert(s->npoints>=0, "MLPStartTraining: parameter S is not initialized or is spoiled(S.NPoints<0)", _state);
    if( !mlpissoftmax(network, _state) )
    {
        ntype = 0;
    }
    else
    {
        ntype = 1;
    }
    if( s->rcpar )
    {
        ttype = 0;
    }
    else
    {
        ttype = 1;
    }
    ae_assert(ntype==ttype, "MLPStartTraining: type of input network is not similar to network type in trainer object", _state);
    mlpproperties(network, &nin, &nout, &wcount, _state);
    ae_assert(s->nin==nin, "MLPStartTraining: number of inputs in trainer is not equal to number of inputs in the network.", _state);
    ae_assert(s->nout==nout, "MLPStartTraining: number of outputs in trainer is not equal to number of outputs in the network.", _state);
    
    /*
     * Initialize temporaries
     */
    mlptrain_initmlptrnsession(network, randomstart, s, &s->session, _state);
    
    /*
     * Train network
     */
    mlptrain_mlpstarttrainingx(s, randomstart, -1, &s->subset, -1, &s->session, _state);
    
    /*
     * Update network
     */
    mlpcopytunableparameters(&s->session.network, network, _state);
}


/*************************************************************************
IMPORTANT: this is an "expert" version of the MLPTrain() function.  We  do
           not recommend you to use it unless you are pretty sure that you
           need ability to monitor training progress.
           
FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support (C++ computational core)
  !
  ! Second improvement gives constant  speedup (2-3X).  First  improvement
  ! gives  close-to-linear  speedup  on   multicore   systems.   Following
  ! operations can be executed in parallel:
  ! * gradient calculation over large dataset (if dataset is large enough)
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! In order to use SSE features you have to:
  ! * use commercial version of ALGLIB on Intel processors
  ! * use C++ computational core
  !
  ! This note is given for users of commercial edition; if  you  use  GPL
  ! edition, you still will be able to call smp-version of this function,
  ! but all computations will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.

This function performs step-by-step training of the neural  network.  Here
"step-by-step" means that training starts  with  MLPStartTraining()  call,
and then user subsequently calls MLPContinueTraining() to perform one more
iteration of the training.

This  function  performs  one  more  iteration of the training and returns
either True (training continues) or False (training stopped). In case True
was returned, Network weights are updated according to the  current  state
of the optimization progress. In case False was  returned,  no  additional
updates is performed (previous update of  the  network weights moved us to
the final point, and no additional updates is needed).

EXAMPLE:
    >
    > [initialize network and trainer object]
    >
    > MLPStartTraining(Trainer, Network, True)
    > while MLPContinueTraining(Trainer, Network) do
    >     [visualize training progress]
    >

INPUT PARAMETERS:
    S           -   trainer object
    Network     -   neural  network  structure,  which  is  used to  store
                    current state of the training process.
                    
OUTPUT PARAMETERS:
    Network     -   weights of the neural network  are  rewritten  by  the
                    current approximation.

NOTE: this method uses sum-of-squares error function for training.

NOTE: it is expected that trainer object settings are NOT  changed  during
      step-by-step training, i.e. no  one  changes  stopping  criteria  or
      training set during training. It is possible and there is no defense
      against  such  actions,  but  algorithm  behavior  in  such cases is
      undefined and can be unpredictable.
      
NOTE: It  is  expected that Network is the same one which  was  passed  to
      MLPStartTraining() function.  However,  THIS  function  checks  only
      following:
      * that number of network inputs is consistent with trainer object
        settings
      * that number of network outputs/classes is consistent with  trainer
        object settings
      * that number of network weights is the same as number of weights in
        the network passed to MLPStartTraining() function
      Exception is thrown when these conditions are violated.
      
      It is also expected that you do not change state of the  network  on
      your own - the only party who has right to change network during its
      training is a trainer object. Any attempt to interfere with  trainer
      may lead to unpredictable results.
      

  -- ALGLIB --
     Copyright 23.07.2012 by Bochkanov Sergey
*************************************************************************/
ae_bool mlpcontinuetraining(mlptrainer* s,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t ntype;
    ae_int_t ttype;
    ae_bool result;


    ae_assert(s->npoints>=0, "MLPContinueTraining: parameter S is not initialized or is spoiled(S.NPoints<0)", _state);
    if( s->rcpar )
    {
        ttype = 0;
    }
    else
    {
        ttype = 1;
    }
    if( !mlpissoftmax(network, _state) )
    {
        ntype = 0;
    }
    else
    {
        ntype = 1;
    }
    ae_assert(ntype==ttype, "MLPContinueTraining: type of input network is not similar to network type in trainer object.", _state);
    mlpproperties(network, &nin, &nout, &wcount, _state);
    ae_assert(s->nin==nin, "MLPContinueTraining: number of inputs in trainer is not equal to number of inputs in the network.", _state);
    ae_assert(s->nout==nout, "MLPContinueTraining: number of outputs in trainer is not equal to number of outputs in the network.", _state);
    result = mlptrain_mlpcontinuetrainingx(s, &s->subset, -1, &s->ngradbatch, &s->session, _state);
    if( result )
    {
        ae_v_move(&network->weights.ptr.p_double[0], 1, &s->session.network.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
    }
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_mlpcontinuetraining(mlptrainer* s,
    multilayerperceptron* network, ae_state *_state)
{
    return mlpcontinuetraining(s,network, _state);
}


/*************************************************************************
Training neural networks ensemble using  bootstrap  aggregating (bagging).
Modified Levenberg-Marquardt algorithm is used as base training method.

INPUT PARAMETERS:
    Ensemble    -   model with initialized geometry
    XY          -   training set
    NPoints     -   training set size
    Decay       -   weight decay coefficient, >=0.001
    Restarts    -   restarts, >0.

OUTPUT PARAMETERS:
    Ensemble    -   trained model
    Info        -   return code:
                    * -2, if there is a point with class number
                          outside of [0..NClasses-1].
                    * -1, if incorrect parameters was passed
                          (NPoints<0, Restarts<1).
                    *  2, if task has been solved.
    Rep         -   training report.
    OOBErrors   -   out-of-bag generalization error estimate

  -- ALGLIB --
     Copyright 17.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpebagginglm(mlpensemble* ensemble,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     double decay,
     ae_int_t restarts,
     ae_int_t* info,
     mlpreport* rep,
     mlpcvreport* ooberrors,
     ae_state *_state)
{

    *info = 0;
    _mlpreport_clear(rep);
    _mlpcvreport_clear(ooberrors);

    mlptrain_mlpebagginginternal(ensemble, xy, npoints, decay, restarts, 0.0, 0, ae_true, info, rep, ooberrors, _state);
}


/*************************************************************************
Training neural networks ensemble using  bootstrap  aggregating (bagging).
L-BFGS algorithm is used as base training method.

INPUT PARAMETERS:
    Ensemble    -   model with initialized geometry
    XY          -   training set
    NPoints     -   training set size
    Decay       -   weight decay coefficient, >=0.001
    Restarts    -   restarts, >0.
    WStep       -   stopping criterion, same as in MLPTrainLBFGS
    MaxIts      -   stopping criterion, same as in MLPTrainLBFGS

OUTPUT PARAMETERS:
    Ensemble    -   trained model
    Info        -   return code:
                    * -8, if both WStep=0 and MaxIts=0
                    * -2, if there is a point with class number
                          outside of [0..NClasses-1].
                    * -1, if incorrect parameters was passed
                          (NPoints<0, Restarts<1).
                    *  2, if task has been solved.
    Rep         -   training report.
    OOBErrors   -   out-of-bag generalization error estimate

  -- ALGLIB --
     Copyright 17.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpebagginglbfgs(mlpensemble* ensemble,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     double decay,
     ae_int_t restarts,
     double wstep,
     ae_int_t maxits,
     ae_int_t* info,
     mlpreport* rep,
     mlpcvreport* ooberrors,
     ae_state *_state)
{

    *info = 0;
    _mlpreport_clear(rep);
    _mlpcvreport_clear(ooberrors);

    mlptrain_mlpebagginginternal(ensemble, xy, npoints, decay, restarts, wstep, maxits, ae_false, info, rep, ooberrors, _state);
}


/*************************************************************************
Training neural networks ensemble using early stopping.

INPUT PARAMETERS:
    Ensemble    -   model with initialized geometry
    XY          -   training set
    NPoints     -   training set size
    Decay       -   weight decay coefficient, >=0.001
    Restarts    -   restarts, >0.

OUTPUT PARAMETERS:
    Ensemble    -   trained model
    Info        -   return code:
                    * -2, if there is a point with class number
                          outside of [0..NClasses-1].
                    * -1, if incorrect parameters was passed
                          (NPoints<0, Restarts<1).
                    *  6, if task has been solved.
    Rep         -   training report.
    OOBErrors   -   out-of-bag generalization error estimate

  -- ALGLIB --
     Copyright 10.03.2009 by Bochkanov Sergey
*************************************************************************/
void mlpetraines(mlpensemble* ensemble,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     double decay,
     ae_int_t restarts,
     ae_int_t* info,
     mlpreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t k;
    ae_int_t ccount;
    ae_int_t pcount;
    ae_matrix trnxy;
    ae_matrix valxy;
    ae_int_t trnsize;
    ae_int_t valsize;
    ae_int_t tmpinfo;
    mlpreport tmprep;
    modelerrors moderr;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _mlpreport_clear(rep);
    ae_matrix_init(&trnxy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&valxy, 0, 0, DT_REAL, _state);
    _mlpreport_init(&tmprep, _state);
    _modelerrors_init(&moderr, _state);

    nin = mlpgetinputscount(&ensemble->network, _state);
    nout = mlpgetoutputscount(&ensemble->network, _state);
    wcount = mlpgetweightscount(&ensemble->network, _state);
    if( (npoints<2||restarts<1)||ae_fp_less(decay,(double)(0)) )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    if( mlpissoftmax(&ensemble->network, _state) )
    {
        for(i=0; i<=npoints-1; i++)
        {
            if( ae_round(xy->ptr.pp_double[i][nin], _state)<0||ae_round(xy->ptr.pp_double[i][nin], _state)>=nout )
            {
                *info = -2;
                ae_frame_leave(_state);
                return;
            }
        }
    }
    *info = 6;
    
    /*
     * allocate
     */
    if( mlpissoftmax(&ensemble->network, _state) )
    {
        ccount = nin+1;
        pcount = nin;
    }
    else
    {
        ccount = nin+nout;
        pcount = nin+nout;
    }
    ae_matrix_set_length(&trnxy, npoints, ccount, _state);
    ae_matrix_set_length(&valxy, npoints, ccount, _state);
    rep->ngrad = 0;
    rep->nhess = 0;
    rep->ncholesky = 0;
    
    /*
     * train networks
     */
    for(k=0; k<=ensemble->ensemblesize-1; k++)
    {
        
        /*
         * Split set
         */
        do
        {
            trnsize = 0;
            valsize = 0;
            for(i=0; i<=npoints-1; i++)
            {
                if( ae_fp_less(ae_randomreal(_state),0.66) )
                {
                    
                    /*
                     * Assign sample to training set
                     */
                    ae_v_move(&trnxy.ptr.pp_double[trnsize][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,ccount-1));
                    trnsize = trnsize+1;
                }
                else
                {
                    
                    /*
                     * Assign sample to validation set
                     */
                    ae_v_move(&valxy.ptr.pp_double[valsize][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,ccount-1));
                    valsize = valsize+1;
                }
            }
        }
        while(!(trnsize!=0&&valsize!=0));
        
        /*
         * Train
         */
        mlptraines(&ensemble->network, &trnxy, trnsize, &valxy, valsize, decay, restarts, &tmpinfo, &tmprep, _state);
        if( tmpinfo<0 )
        {
            *info = tmpinfo;
            ae_frame_leave(_state);
            return;
        }
        
        /*
         * save results
         */
        ae_v_move(&ensemble->weights.ptr.p_double[k*wcount], 1, &ensemble->network.weights.ptr.p_double[0], 1, ae_v_len(k*wcount,(k+1)*wcount-1));
        ae_v_move(&ensemble->columnmeans.ptr.p_double[k*pcount], 1, &ensemble->network.columnmeans.ptr.p_double[0], 1, ae_v_len(k*pcount,(k+1)*pcount-1));
        ae_v_move(&ensemble->columnsigmas.ptr.p_double[k*pcount], 1, &ensemble->network.columnsigmas.ptr.p_double[0], 1, ae_v_len(k*pcount,(k+1)*pcount-1));
        rep->ngrad = rep->ngrad+tmprep.ngrad;
        rep->nhess = rep->nhess+tmprep.nhess;
        rep->ncholesky = rep->ncholesky+tmprep.ncholesky;
    }
    mlpeallerrorsx(ensemble, xy, &ensemble->network.dummysxy, npoints, 0, &ensemble->network.dummyidx, 0, npoints, 0, &ensemble->network.buf, &moderr, _state);
    rep->relclserror = moderr.relclserror;
    rep->avgce = moderr.avgce;
    rep->rmserror = moderr.rmserror;
    rep->avgerror = moderr.avgerror;
    rep->avgrelerror = moderr.avgrelerror;
    ae_frame_leave(_state);
}


/*************************************************************************
This function trains neural network ensemble passed to this function using
current dataset and early stopping training algorithm. Each early stopping
round performs NRestarts  random  restarts  (thus,  EnsembleSize*NRestarts
training rounds is performed in total).

FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support (C++ computational core)
  !
  ! Second improvement gives constant  speedup (2-3X).  First  improvement
  ! gives  close-to-linear  speedup  on   multicore   systems.   Following
  ! operations can be executed in parallel:
  ! * EnsembleSize  training  sessions  performed  for  each  of  ensemble
  !   members (always parallelized)
  ! * NRestarts  training  sessions  performed  within  each  of  training
  !   sessions (if NRestarts>1)
  ! * gradient calculation over large dataset (if dataset is large enough)
  !
  ! In order to use multicore features you have to:
  ! * use commercial version of ALGLIB
  ! * call  this  function  with  "smp_"  prefix,  which  indicates  that
  !   multicore code will be used (for multicore support)
  !
  ! In order to use SSE features you have to:
  ! * use commercial version of ALGLIB on Intel processors
  ! * use C++ computational core
  !
  ! This note is given for users of commercial edition; if  you  use  GPL
  ! edition, you still will be able to call smp-version of this function,
  ! but all computations will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.

INPUT PARAMETERS:
    S           -   trainer object;
    Ensemble    -   neural network ensemble. It must have same  number  of
                    inputs and outputs/classes  as  was  specified  during
                    creation of the trainer object.
    NRestarts   -   number of restarts, >=0:
                    * NRestarts>0 means that specified  number  of  random
                      restarts are performed during each ES round;
                    * NRestarts=0 is silently replaced by 1.

OUTPUT PARAMETERS:
    Ensemble    -   trained ensemble;
    Rep         -   it contains all type of errors.
    
NOTE: this training method uses BOTH early stopping and weight decay!  So,
      you should select weight decay before starting training just as  you
      select it before training "conventional" networks.

NOTE: when no dataset was specified with MLPSetDataset/SetSparseDataset(),
      or  single-point  dataset  was  passed,  ensemble  is filled by zero
      values.

NOTE: this method uses sum-of-squares error function for training.

  -- ALGLIB --
     Copyright 22.08.2012 by Bochkanov Sergey
*************************************************************************/
void mlptrainensemblees(mlptrainer* s,
     mlpensemble* ensemble,
     ae_int_t nrestarts,
     mlpreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t ntype;
    ae_int_t ttype;
    ae_shared_pool esessions;
    sinteger sgrad;
    modelerrors tmprep;

    ae_frame_make(_state, &_frame_block);
    _mlpreport_clear(rep);
    ae_shared_pool_init(&esessions, _state);
    _sinteger_init(&sgrad, _state);
    _modelerrors_init(&tmprep, _state);

    ae_assert(s->npoints>=0, "MLPTrainEnsembleES: parameter S is not initialized or is spoiled(S.NPoints<0)", _state);
    if( !mlpeissoftmax(ensemble, _state) )
    {
        ntype = 0;
    }
    else
    {
        ntype = 1;
    }
    if( s->rcpar )
    {
        ttype = 0;
    }
    else
    {
        ttype = 1;
    }
    ae_assert(ntype==ttype, "MLPTrainEnsembleES: internal error - type of input network is not similar to network type in trainer object", _state);
    nin = mlpgetinputscount(&ensemble->network, _state);
    ae_assert(s->nin==nin, "MLPTrainEnsembleES: number of inputs in trainer is not equal to number of inputs in ensemble network", _state);
    nout = mlpgetoutputscount(&ensemble->network, _state);
    ae_assert(s->nout==nout, "MLPTrainEnsembleES: number of outputs in trainer is not equal to number of outputs in ensemble network", _state);
    ae_assert(nrestarts>=0, "MLPTrainEnsembleES: NRestarts<0.", _state);
    
    /*
     * Initialize parameter Rep
     */
    rep->relclserror = (double)(0);
    rep->avgce = (double)(0);
    rep->rmserror = (double)(0);
    rep->avgerror = (double)(0);
    rep->avgrelerror = (double)(0);
    rep->ngrad = 0;
    rep->nhess = 0;
    rep->ncholesky = 0;
    
    /*
     * Allocate
     */
    ivectorsetlengthatleast(&s->subset, s->npoints, _state);
    ivectorsetlengthatleast(&s->valsubset, s->npoints, _state);
    
    /*
     * Start training
     *
     * NOTE: ESessions is not initialized because MLPTrainEnsembleX
     *       needs uninitialized pool.
     */
    sgrad.val = 0;
    mlptrain_mlptrainensemblex(s, ensemble, 0, ensemble->ensemblesize, nrestarts, 0, &sgrad, ae_true, &esessions, _state);
    rep->ngrad = sgrad.val;
    
    /*
     * Calculate errors.
     */
    if( s->datatype==0 )
    {
        mlpeallerrorsx(ensemble, &s->densexy, &s->sparsexy, s->npoints, 0, &ensemble->network.dummyidx, 0, s->npoints, 0, &ensemble->network.buf, &tmprep, _state);
    }
    if( s->datatype==1 )
    {
        mlpeallerrorsx(ensemble, &s->densexy, &s->sparsexy, s->npoints, 1, &ensemble->network.dummyidx, 0, s->npoints, 0, &ensemble->network.buf, &tmprep, _state);
    }
    rep->relclserror = tmprep.relclserror;
    rep->avgce = tmprep.avgce;
    rep->rmserror = tmprep.rmserror;
    rep->avgerror = tmprep.avgerror;
    rep->avgrelerror = tmprep.avgrelerror;
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_mlptrainensemblees(mlptrainer* s,
    mlpensemble* ensemble,
    ae_int_t nrestarts,
    mlpreport* rep, ae_state *_state)
{
    mlptrainensemblees(s,ensemble,nrestarts,rep, _state);
}


/*************************************************************************
Internal cross-validation subroutine
*************************************************************************/
static void mlptrain_mlpkfoldcvgeneral(multilayerperceptron* n,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     double decay,
     ae_int_t restarts,
     ae_int_t foldscount,
     ae_bool lmalgorithm,
     double wstep,
     ae_int_t maxits,
     ae_int_t* info,
     mlpreport* rep,
     mlpcvreport* cvrep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t fold;
    ae_int_t j;
    ae_int_t k;
    multilayerperceptron network;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t rowlen;
    ae_int_t wcount;
    ae_int_t nclasses;
    ae_int_t tssize;
    ae_int_t cvssize;
    ae_matrix cvset;
    ae_matrix testset;
    ae_vector folds;
    ae_int_t relcnt;
    mlpreport internalrep;
    ae_vector x;
    ae_vector y;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _mlpreport_clear(rep);
    _mlpcvreport_clear(cvrep);
    _multilayerperceptron_init(&network, _state);
    ae_matrix_init(&cvset, 0, 0, DT_REAL, _state);
    ae_matrix_init(&testset, 0, 0, DT_REAL, _state);
    ae_vector_init(&folds, 0, DT_INT, _state);
    _mlpreport_init(&internalrep, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);

    
    /*
     * Read network geometry, test parameters
     */
    mlpproperties(n, &nin, &nout, &wcount, _state);
    if( mlpissoftmax(n, _state) )
    {
        nclasses = nout;
        rowlen = nin+1;
    }
    else
    {
        nclasses = -nout;
        rowlen = nin+nout;
    }
    if( (npoints<=0||foldscount<2)||foldscount>npoints )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    mlpcopy(n, &network, _state);
    
    /*
     * K-fold out cross-validation.
     * First, estimate generalization error
     */
    ae_matrix_set_length(&testset, npoints-1+1, rowlen-1+1, _state);
    ae_matrix_set_length(&cvset, npoints-1+1, rowlen-1+1, _state);
    ae_vector_set_length(&x, nin-1+1, _state);
    ae_vector_set_length(&y, nout-1+1, _state);
    mlptrain_mlpkfoldsplit(xy, npoints, nclasses, foldscount, ae_false, &folds, _state);
    cvrep->relclserror = (double)(0);
    cvrep->avgce = (double)(0);
    cvrep->rmserror = (double)(0);
    cvrep->avgerror = (double)(0);
    cvrep->avgrelerror = (double)(0);
    rep->ngrad = 0;
    rep->nhess = 0;
    rep->ncholesky = 0;
    relcnt = 0;
    for(fold=0; fold<=foldscount-1; fold++)
    {
        
        /*
         * Separate set
         */
        tssize = 0;
        cvssize = 0;
        for(i=0; i<=npoints-1; i++)
        {
            if( folds.ptr.p_int[i]==fold )
            {
                ae_v_move(&testset.ptr.pp_double[tssize][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,rowlen-1));
                tssize = tssize+1;
            }
            else
            {
                ae_v_move(&cvset.ptr.pp_double[cvssize][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,rowlen-1));
                cvssize = cvssize+1;
            }
        }
        
        /*
         * Train on CV training set
         */
        if( lmalgorithm )
        {
            mlptrainlm(&network, &cvset, cvssize, decay, restarts, info, &internalrep, _state);
        }
        else
        {
            mlptrainlbfgs(&network, &cvset, cvssize, decay, restarts, wstep, maxits, info, &internalrep, _state);
        }
        if( *info<0 )
        {
            cvrep->relclserror = (double)(0);
            cvrep->avgce = (double)(0);
            cvrep->rmserror = (double)(0);
            cvrep->avgerror = (double)(0);
            cvrep->avgrelerror = (double)(0);
            ae_frame_leave(_state);
            return;
        }
        rep->ngrad = rep->ngrad+internalrep.ngrad;
        rep->nhess = rep->nhess+internalrep.nhess;
        rep->ncholesky = rep->ncholesky+internalrep.ncholesky;
        
        /*
         * Estimate error using CV test set
         */
        if( mlpissoftmax(&network, _state) )
        {
            
            /*
             * classification-only code
             */
            cvrep->relclserror = cvrep->relclserror+mlpclserror(&network, &testset, tssize, _state);
            cvrep->avgce = cvrep->avgce+mlperrorn(&network, &testset, tssize, _state);
        }
        for(i=0; i<=tssize-1; i++)
        {
            ae_v_move(&x.ptr.p_double[0], 1, &testset.ptr.pp_double[i][0], 1, ae_v_len(0,nin-1));
            mlpprocess(&network, &x, &y, _state);
            if( mlpissoftmax(&network, _state) )
            {
                
                /*
                 * Classification-specific code
                 */
                k = ae_round(testset.ptr.pp_double[i][nin], _state);
                for(j=0; j<=nout-1; j++)
                {
                    if( j==k )
                    {
                        cvrep->rmserror = cvrep->rmserror+ae_sqr(y.ptr.p_double[j]-1, _state);
                        cvrep->avgerror = cvrep->avgerror+ae_fabs(y.ptr.p_double[j]-1, _state);
                        cvrep->avgrelerror = cvrep->avgrelerror+ae_fabs(y.ptr.p_double[j]-1, _state);
                        relcnt = relcnt+1;
                    }
                    else
                    {
                        cvrep->rmserror = cvrep->rmserror+ae_sqr(y.ptr.p_double[j], _state);
                        cvrep->avgerror = cvrep->avgerror+ae_fabs(y.ptr.p_double[j], _state);
                    }
                }
            }
            else
            {
                
                /*
                 * Regression-specific code
                 */
                for(j=0; j<=nout-1; j++)
                {
                    cvrep->rmserror = cvrep->rmserror+ae_sqr(y.ptr.p_double[j]-testset.ptr.pp_double[i][nin+j], _state);
                    cvrep->avgerror = cvrep->avgerror+ae_fabs(y.ptr.p_double[j]-testset.ptr.pp_double[i][nin+j], _state);
                    if( ae_fp_neq(testset.ptr.pp_double[i][nin+j],(double)(0)) )
                    {
                        cvrep->avgrelerror = cvrep->avgrelerror+ae_fabs((y.ptr.p_double[j]-testset.ptr.pp_double[i][nin+j])/testset.ptr.pp_double[i][nin+j], _state);
                        relcnt = relcnt+1;
                    }
                }
            }
        }
    }
    if( mlpissoftmax(&network, _state) )
    {
        cvrep->relclserror = cvrep->relclserror/npoints;
        cvrep->avgce = cvrep->avgce/(ae_log((double)(2), _state)*npoints);
    }
    cvrep->rmserror = ae_sqrt(cvrep->rmserror/(npoints*nout), _state);
    cvrep->avgerror = cvrep->avgerror/(npoints*nout);
    if( relcnt>0 )
    {
        cvrep->avgrelerror = cvrep->avgrelerror/relcnt;
    }
    *info = 1;
    ae_frame_leave(_state);
}


/*************************************************************************
Subroutine prepares K-fold split of the training set.

NOTES:
    "NClasses>0" means that we have classification task.
    "NClasses<0" means regression task with -NClasses real outputs.
*************************************************************************/
static void mlptrain_mlpkfoldsplit(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nclasses,
     ae_int_t foldscount,
     ae_bool stratifiedsplits,
     /* Integer */ ae_vector* folds,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(folds);
    _hqrndstate_init(&rs, _state);

    
    /*
     * test parameters
     */
    ae_assert(npoints>0, "MLPKFoldSplit: wrong NPoints!", _state);
    ae_assert(nclasses>1||nclasses<0, "MLPKFoldSplit: wrong NClasses!", _state);
    ae_assert(foldscount>=2&&foldscount<=npoints, "MLPKFoldSplit: wrong FoldsCount!", _state);
    ae_assert(!stratifiedsplits, "MLPKFoldSplit: stratified splits are not supported!", _state);
    
    /*
     * Folds
     */
    hqrndrandomize(&rs, _state);
    ae_vector_set_length(folds, npoints-1+1, _state);
    for(i=0; i<=npoints-1; i++)
    {
        folds->ptr.p_int[i] = i*foldscount/npoints;
    }
    for(i=0; i<=npoints-2; i++)
    {
        j = i+hqrnduniformi(&rs, npoints-i, _state);
        if( j!=i )
        {
            k = folds->ptr.p_int[i];
            folds->ptr.p_int[i] = folds->ptr.p_int[j];
            folds->ptr.p_int[j] = k;
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine for parallelization function MLPFoldCV.


INPUT PARAMETERS:
    S         -   trainer object;
    RowSize   -   row size(eitherNIn+NOut or NIn+1);
    NRestarts -   number of restarts(>=0);
    Folds     -   cross-validation set;
    Fold      -   the number of first cross-validation(>=0);
    DFold     -   the number of second cross-validation(>=Fold+1);
    CVY       -   parameter which stores  the result is returned by network,
                  training on I-th cross-validation set.
                  It has to be preallocated.
    PoolDataCV-   parameter for parallelization.
    
NOTE: There are no checks on the parameters correctness.

  -- ALGLIB --
     Copyright 25.09.2012 by Bochkanov Sergey
*************************************************************************/
static void mlptrain_mthreadcv(mlptrainer* s,
     ae_int_t rowsize,
     ae_int_t nrestarts,
     /* Integer */ ae_vector* folds,
     ae_int_t fold,
     ae_int_t dfold,
     /* Real    */ ae_matrix* cvy,
     ae_shared_pool* pooldatacv,
     ae_state *_state)
{
    ae_frame _frame_block;
    mlpparallelizationcv *datacv;
    ae_smart_ptr _datacv;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_smart_ptr_init(&_datacv, (void**)&datacv, _state);

    if( fold==dfold-1 )
    {
        
        /*
         * Separate set
         */
        ae_shared_pool_retrieve(pooldatacv, &_datacv, _state);
        datacv->subsetsize = 0;
        for(i=0; i<=s->npoints-1; i++)
        {
            if( folds->ptr.p_int[i]!=fold )
            {
                datacv->subset.ptr.p_int[datacv->subsetsize] = i;
                datacv->subsetsize = datacv->subsetsize+1;
            }
        }
        
        /*
         * Train on CV training set
         */
        mlptrain_mlptrainnetworkx(s, nrestarts, -1, &datacv->subset, datacv->subsetsize, &datacv->subset, 0, &datacv->network, &datacv->rep, ae_true, &datacv->trnpool, _state);
        datacv->ngrad = datacv->ngrad+datacv->rep.ngrad;
        
        /*
         * Estimate error using CV test set
         */
        for(i=0; i<=s->npoints-1; i++)
        {
            if( folds->ptr.p_int[i]==fold )
            {
                if( s->datatype==0 )
                {
                    ae_v_move(&datacv->xyrow.ptr.p_double[0], 1, &s->densexy.ptr.pp_double[i][0], 1, ae_v_len(0,rowsize-1));
                }
                if( s->datatype==1 )
                {
                    sparsegetrow(&s->sparsexy, i, &datacv->xyrow, _state);
                }
                mlpprocess(&datacv->network, &datacv->xyrow, &datacv->y, _state);
                ae_v_move(&cvy->ptr.pp_double[i][0], 1, &datacv->y.ptr.p_double[0], 1, ae_v_len(0,s->nout-1));
            }
        }
        ae_shared_pool_recycle(pooldatacv, &_datacv, _state);
    }
    else
    {
        ae_assert(fold<dfold-1, "MThreadCV: internal error(Fold>DFold-1).", _state);
        mlptrain_mthreadcv(s, rowsize, nrestarts, folds, fold, (fold+dfold)/2, cvy, pooldatacv, _state);
        mlptrain_mthreadcv(s, rowsize, nrestarts, folds, (fold+dfold)/2, dfold, cvy, pooldatacv, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function trains neural network passed to this function, using current
dataset (one which was passed to MLPSetDataset() or MLPSetSparseDataset())
and current training settings. Training  from  NRestarts  random  starting
positions is performed, best network is chosen.

This function is inteded to be used internally. It may be used in  several
settings:
* training with ValSubsetSize=0, corresponds  to  "normal"  training  with
  termination  criteria  based on S.MaxIts (steps count) and S.WStep (step
  size). Training sample is given by TrnSubset/TrnSubsetSize.
* training with ValSubsetSize>0, corresponds to  early  stopping  training
  with additional MaxIts/WStep stopping criteria. Training sample is given
  by TrnSubset/TrnSubsetSize, validation sample  is  given  by  ValSubset/
  ValSubsetSize.

  -- ALGLIB --
     Copyright 13.08.2012 by Bochkanov Sergey
*************************************************************************/
static void mlptrain_mlptrainnetworkx(mlptrainer* s,
     ae_int_t nrestarts,
     ae_int_t algokind,
     /* Integer */ ae_vector* trnsubset,
     ae_int_t trnsubsetsize,
     /* Integer */ ae_vector* valsubset,
     ae_int_t valsubsetsize,
     multilayerperceptron* network,
     mlpreport* rep,
     ae_bool isrootcall,
     ae_shared_pool* sessions,
     ae_state *_state)
{
    ae_frame _frame_block;
    modelerrors modrep;
    double eval;
    double ebest;
    ae_int_t ngradbatch;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t pcount;
    ae_int_t itbest;
    ae_int_t itcnt;
    ae_int_t ntype;
    ae_int_t ttype;
    ae_bool rndstart;
    ae_int_t i;
    ae_int_t nr0;
    ae_int_t nr1;
    mlpreport rep0;
    mlpreport rep1;
    ae_bool randomizenetwork;
    double bestrmserror;
    smlptrnsession *psession;
    ae_smart_ptr _psession;

    ae_frame_make(_state, &_frame_block);
    _modelerrors_init(&modrep, _state);
    _mlpreport_init(&rep0, _state);
    _mlpreport_init(&rep1, _state);
    ae_smart_ptr_init(&_psession, (void**)&psession, _state);

    mlpproperties(network, &nin, &nout, &wcount, _state);
    
    /*
     * Process root call
     */
    if( isrootcall )
    {
        
        /*
         * Check correctness of parameters
         */
        ae_assert(algokind==0||algokind==-1, "MLPTrainNetworkX: unexpected AlgoKind", _state);
        ae_assert(s->npoints>=0, "MLPTrainNetworkX: internal error - parameter S is not initialized or is spoiled(S.NPoints<0)", _state);
        if( s->rcpar )
        {
            ttype = 0;
        }
        else
        {
            ttype = 1;
        }
        if( !mlpissoftmax(network, _state) )
        {
            ntype = 0;
        }
        else
        {
            ntype = 1;
        }
        ae_assert(ntype==ttype, "MLPTrainNetworkX: internal error - type of the training network is not similar to network type in trainer object", _state);
        ae_assert(s->nin==nin, "MLPTrainNetworkX: internal error - number of inputs in trainer is not equal to number of inputs in the training network.", _state);
        ae_assert(s->nout==nout, "MLPTrainNetworkX: internal error - number of outputs in trainer is not equal to number of outputs in the training network.", _state);
        ae_assert(nrestarts>=0, "MLPTrainNetworkX: internal error - NRestarts<0.", _state);
        ae_assert(trnsubset->cnt>=trnsubsetsize, "MLPTrainNetworkX: internal error - parameter TrnSubsetSize more than input subset size(Length(TrnSubset)<TrnSubsetSize)", _state);
        for(i=0; i<=trnsubsetsize-1; i++)
        {
            ae_assert(trnsubset->ptr.p_int[i]>=0&&trnsubset->ptr.p_int[i]<=s->npoints-1, "MLPTrainNetworkX: internal error - parameter TrnSubset contains incorrect index(TrnSubset[I]<0 or TrnSubset[I]>S.NPoints-1)", _state);
        }
        ae_assert(valsubset->cnt>=valsubsetsize, "MLPTrainNetworkX: internal error - parameter ValSubsetSize more than input subset size(Length(ValSubset)<ValSubsetSize)", _state);
        for(i=0; i<=valsubsetsize-1; i++)
        {
            ae_assert(valsubset->ptr.p_int[i]>=0&&valsubset->ptr.p_int[i]<=s->npoints-1, "MLPTrainNetworkX: internal error - parameter ValSubset contains incorrect index(ValSubset[I]<0 or ValSubset[I]>S.NPoints-1)", _state);
        }
        
        /*
         * Train
         */
        randomizenetwork = nrestarts>0;
        mlptrain_initmlptrnsessions(network, randomizenetwork, s, sessions, _state);
        mlptrain_mlptrainnetworkx(s, nrestarts, algokind, trnsubset, trnsubsetsize, valsubset, valsubsetsize, network, rep, ae_false, sessions, _state);
        
        /*
         * Choose best network
         */
        bestrmserror = ae_maxrealnumber;
        ae_shared_pool_first_recycled(sessions, &_psession, _state);
        while(psession!=NULL)
        {
            if( ae_fp_less(psession->bestrmserror,bestrmserror) )
            {
                mlpimporttunableparameters(network, &psession->bestparameters, _state);
                bestrmserror = psession->bestrmserror;
            }
            ae_shared_pool_next_recycled(sessions, &_psession, _state);
        }
        
        /*
         * Calculate errors
         */
        if( s->datatype==0 )
        {
            mlpallerrorssubset(network, &s->densexy, s->npoints, trnsubset, trnsubsetsize, &modrep, _state);
        }
        if( s->datatype==1 )
        {
            mlpallerrorssparsesubset(network, &s->sparsexy, s->npoints, trnsubset, trnsubsetsize, &modrep, _state);
        }
        rep->relclserror = modrep.relclserror;
        rep->avgce = modrep.avgce;
        rep->rmserror = modrep.rmserror;
        rep->avgerror = modrep.avgerror;
        rep->avgrelerror = modrep.avgrelerror;
        
        /*
         * Done
         */
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Split problem, if we have more than 1 restart
     */
    if( nrestarts>=2 )
    {
        
        /*
         * Divide problem with NRestarts into two: NR0 and NR1.
         */
        nr0 = nrestarts/2;
        nr1 = nrestarts-nr0;
        mlptrain_mlptrainnetworkx(s, nr0, algokind, trnsubset, trnsubsetsize, valsubset, valsubsetsize, network, &rep0, ae_false, sessions, _state);
        mlptrain_mlptrainnetworkx(s, nr1, algokind, trnsubset, trnsubsetsize, valsubset, valsubsetsize, network, &rep1, ae_false, sessions, _state);
        
        /*
         * Aggregate results
         */
        rep->ngrad = rep0.ngrad+rep1.ngrad;
        rep->nhess = rep0.nhess+rep1.nhess;
        rep->ncholesky = rep0.ncholesky+rep1.ncholesky;
        
        /*
         * Done :)
         */
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Execution with NRestarts=1 or NRestarts=0:
     * * NRestarts=1 means that network is restarted from random position
     * * NRestarts=0 means that network is not randomized
     */
    ae_assert(nrestarts==0||nrestarts==1, "MLPTrainNetworkX: internal error", _state);
    rep->ngrad = 0;
    rep->nhess = 0;
    rep->ncholesky = 0;
    ae_shared_pool_retrieve(sessions, &_psession, _state);
    if( ((s->datatype==0||s->datatype==1)&&s->npoints>0)&&trnsubsetsize!=0 )
    {
        
        /*
         * Train network using combination of early stopping and step-size
         * and step-count based criteria. Network state with best value of
         * validation set error is stored in WBuf0. When validation set is
         * zero, most recent state of network is stored.
         */
        rndstart = nrestarts!=0;
        ngradbatch = 0;
        eval = (double)(0);
        ebest = (double)(0);
        itbest = 0;
        itcnt = 0;
        mlptrain_mlpstarttrainingx(s, rndstart, algokind, trnsubset, trnsubsetsize, psession, _state);
        if( s->datatype==0 )
        {
            ebest = mlperrorsubset(&psession->network, &s->densexy, s->npoints, valsubset, valsubsetsize, _state);
        }
        if( s->datatype==1 )
        {
            ebest = mlperrorsparsesubset(&psession->network, &s->sparsexy, s->npoints, valsubset, valsubsetsize, _state);
        }
        ae_v_move(&psession->wbuf0.ptr.p_double[0], 1, &psession->network.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        while(mlptrain_mlpcontinuetrainingx(s, trnsubset, trnsubsetsize, &ngradbatch, psession, _state))
        {
            if( s->datatype==0 )
            {
                eval = mlperrorsubset(&psession->network, &s->densexy, s->npoints, valsubset, valsubsetsize, _state);
            }
            if( s->datatype==1 )
            {
                eval = mlperrorsparsesubset(&psession->network, &s->sparsexy, s->npoints, valsubset, valsubsetsize, _state);
            }
            if( ae_fp_less_eq(eval,ebest)||valsubsetsize==0 )
            {
                ae_v_move(&psession->wbuf0.ptr.p_double[0], 1, &psession->network.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
                ebest = eval;
                itbest = itcnt;
            }
            if( itcnt>30&&ae_fp_greater((double)(itcnt),1.5*itbest) )
            {
                break;
            }
            itcnt = itcnt+1;
        }
        ae_v_move(&psession->network.weights.ptr.p_double[0], 1, &psession->wbuf0.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        rep->ngrad = ngradbatch;
    }
    else
    {
        for(i=0; i<=wcount-1; i++)
        {
            psession->network.weights.ptr.p_double[i] = (double)(0);
        }
    }
    
    /*
     * Evaluate network performance and update PSession.BestParameters/BestRMSError
     * (if needed).
     */
    if( s->datatype==0 )
    {
        mlpallerrorssubset(&psession->network, &s->densexy, s->npoints, trnsubset, trnsubsetsize, &modrep, _state);
    }
    if( s->datatype==1 )
    {
        mlpallerrorssparsesubset(&psession->network, &s->sparsexy, s->npoints, trnsubset, trnsubsetsize, &modrep, _state);
    }
    if( ae_fp_less(modrep.rmserror,psession->bestrmserror) )
    {
        mlpexporttunableparameters(&psession->network, &psession->bestparameters, &pcount, _state);
        psession->bestrmserror = modrep.rmserror;
    }
    
    /*
     * Move session back to pool
     */
    ae_shared_pool_recycle(sessions, &_psession, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
This function trains neural network ensemble passed to this function using
current dataset and early stopping training algorithm. Each early stopping
round performs NRestarts  random  restarts  (thus,  EnsembleSize*NRestarts
training rounds is performed in total).


  -- ALGLIB --
     Copyright 22.08.2012 by Bochkanov Sergey
*************************************************************************/
static void mlptrain_mlptrainensemblex(mlptrainer* s,
     mlpensemble* ensemble,
     ae_int_t idx0,
     ae_int_t idx1,
     ae_int_t nrestarts,
     ae_int_t trainingmethod,
     sinteger* ngrad,
     ae_bool isrootcall,
     ae_shared_pool* esessions,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t pcount;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t trnsubsetsize;
    ae_int_t valsubsetsize;
    ae_int_t k0;
    sinteger ngrad0;
    sinteger ngrad1;
    mlpetrnsession *psession;
    ae_smart_ptr _psession;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    _sinteger_init(&ngrad0, _state);
    _sinteger_init(&ngrad1, _state);
    ae_smart_ptr_init(&_psession, (void**)&psession, _state);
    _hqrndstate_init(&rs, _state);

    nin = mlpgetinputscount(&ensemble->network, _state);
    nout = mlpgetoutputscount(&ensemble->network, _state);
    wcount = mlpgetweightscount(&ensemble->network, _state);
    if( mlpissoftmax(&ensemble->network, _state) )
    {
        pcount = nin;
    }
    else
    {
        pcount = nin+nout;
    }
    if( nrestarts<=0 )
    {
        nrestarts = 1;
    }
    
    /*
     * Handle degenerate case
     */
    if( s->npoints<2 )
    {
        for(i=idx0; i<=idx1-1; i++)
        {
            for(j=0; j<=wcount-1; j++)
            {
                ensemble->weights.ptr.p_double[i*wcount+j] = 0.0;
            }
            for(j=0; j<=pcount-1; j++)
            {
                ensemble->columnmeans.ptr.p_double[i*pcount+j] = 0.0;
                ensemble->columnsigmas.ptr.p_double[i*pcount+j] = 1.0;
            }
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Process root call
     */
    if( isrootcall )
    {
        
        /*
         * Prepare:
         * * prepare MLPETrnSessions
         * * fill ensemble by zeros (helps to detect errors)
         */
        mlptrain_initmlpetrnsessions(&ensemble->network, s, esessions, _state);
        for(i=idx0; i<=idx1-1; i++)
        {
            for(j=0; j<=wcount-1; j++)
            {
                ensemble->weights.ptr.p_double[i*wcount+j] = 0.0;
            }
            for(j=0; j<=pcount-1; j++)
            {
                ensemble->columnmeans.ptr.p_double[i*pcount+j] = 0.0;
                ensemble->columnsigmas.ptr.p_double[i*pcount+j] = 0.0;
            }
        }
        
        /*
         * Train in non-root mode and exit
         */
        mlptrain_mlptrainensemblex(s, ensemble, idx0, idx1, nrestarts, trainingmethod, ngrad, ae_false, esessions, _state);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Split problem
     */
    if( idx1-idx0>=2 )
    {
        k0 = (idx1-idx0)/2;
        ngrad0.val = 0;
        ngrad1.val = 0;
        mlptrain_mlptrainensemblex(s, ensemble, idx0, idx0+k0, nrestarts, trainingmethod, &ngrad0, ae_false, esessions, _state);
        mlptrain_mlptrainensemblex(s, ensemble, idx0+k0, idx1, nrestarts, trainingmethod, &ngrad1, ae_false, esessions, _state);
        ngrad->val = ngrad0.val+ngrad1.val;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Retrieve and prepare session
     */
    ae_shared_pool_retrieve(esessions, &_psession, _state);
    
    /*
     * Train
     */
    hqrndrandomize(&rs, _state);
    for(k=idx0; k<=idx1-1; k++)
    {
        
        /*
         * Split set
         */
        trnsubsetsize = 0;
        valsubsetsize = 0;
        if( trainingmethod==0 )
        {
            do
            {
                trnsubsetsize = 0;
                valsubsetsize = 0;
                for(i=0; i<=s->npoints-1; i++)
                {
                    if( ae_fp_less(ae_randomreal(_state),0.66) )
                    {
                        
                        /*
                         * Assign sample to training set
                         */
                        psession->trnsubset.ptr.p_int[trnsubsetsize] = i;
                        trnsubsetsize = trnsubsetsize+1;
                    }
                    else
                    {
                        
                        /*
                         * Assign sample to validation set
                         */
                        psession->valsubset.ptr.p_int[valsubsetsize] = i;
                        valsubsetsize = valsubsetsize+1;
                    }
                }
            }
            while(!(trnsubsetsize!=0&&valsubsetsize!=0));
        }
        if( trainingmethod==1 )
        {
            valsubsetsize = 0;
            trnsubsetsize = s->npoints;
            for(i=0; i<=s->npoints-1; i++)
            {
                psession->trnsubset.ptr.p_int[i] = hqrnduniformi(&rs, s->npoints, _state);
            }
        }
        
        /*
         * Train
         */
        mlptrain_mlptrainnetworkx(s, nrestarts, -1, &psession->trnsubset, trnsubsetsize, &psession->valsubset, valsubsetsize, &psession->network, &psession->mlprep, ae_true, &psession->mlpsessions, _state);
        ngrad->val = ngrad->val+psession->mlprep.ngrad;
        
        /*
         * Save results
         */
        ae_v_move(&ensemble->weights.ptr.p_double[k*wcount], 1, &psession->network.weights.ptr.p_double[0], 1, ae_v_len(k*wcount,(k+1)*wcount-1));
        ae_v_move(&ensemble->columnmeans.ptr.p_double[k*pcount], 1, &psession->network.columnmeans.ptr.p_double[0], 1, ae_v_len(k*pcount,(k+1)*pcount-1));
        ae_v_move(&ensemble->columnsigmas.ptr.p_double[k*pcount], 1, &psession->network.columnsigmas.ptr.p_double[0], 1, ae_v_len(k*pcount,(k+1)*pcount-1));
    }
    
    /*
     * Recycle session
     */
    ae_shared_pool_recycle(esessions, &_psession, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
This function performs step-by-step training of the neural  network.  Here
"step-by-step" means that training  starts  with  MLPStartTrainingX  call,
and then user subsequently calls MLPContinueTrainingX  to perform one more
iteration of the training.

After call to this function trainer object remembers network and  is ready
to  train  it.  However,  no  training  is  performed  until first call to 
MLPContinueTraining() function. Subsequent calls  to MLPContinueTraining()
will advance traing progress one iteration further.


  -- ALGLIB --
     Copyright 13.08.2012 by Bochkanov Sergey
*************************************************************************/
static void mlptrain_mlpstarttrainingx(mlptrainer* s,
     ae_bool randomstart,
     ae_int_t algokind,
     /* Integer */ ae_vector* subset,
     ae_int_t subsetsize,
     smlptrnsession* session,
     ae_state *_state)
{
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t ntype;
    ae_int_t ttype;
    ae_int_t i;


    
    /*
     * Check parameters
     */
    ae_assert(s->npoints>=0, "MLPStartTrainingX: internal error - parameter S is not initialized or is spoiled(S.NPoints<0)", _state);
    ae_assert(algokind==0||algokind==-1, "MLPStartTrainingX: unexpected AlgoKind", _state);
    if( s->rcpar )
    {
        ttype = 0;
    }
    else
    {
        ttype = 1;
    }
    if( !mlpissoftmax(&session->network, _state) )
    {
        ntype = 0;
    }
    else
    {
        ntype = 1;
    }
    ae_assert(ntype==ttype, "MLPStartTrainingX: internal error - type of the resulting network is not similar to network type in trainer object", _state);
    mlpproperties(&session->network, &nin, &nout, &wcount, _state);
    ae_assert(s->nin==nin, "MLPStartTrainingX: number of inputs in trainer is not equal to number of inputs in the network.", _state);
    ae_assert(s->nout==nout, "MLPStartTrainingX: number of outputs in trainer is not equal to number of outputs in the network.", _state);
    ae_assert(subset->cnt>=subsetsize, "MLPStartTrainingX: internal error - parameter SubsetSize more than input subset size(Length(Subset)<SubsetSize)", _state);
    for(i=0; i<=subsetsize-1; i++)
    {
        ae_assert(subset->ptr.p_int[i]>=0&&subset->ptr.p_int[i]<=s->npoints-1, "MLPStartTrainingX: internal error - parameter Subset contains incorrect index(Subset[I]<0 or Subset[I]>S.NPoints-1)", _state);
    }
    
    /*
     * Prepare session
     */
    minlbfgssetcond(&session->optimizer, 0.0, 0.0, s->wstep, s->maxits, _state);
    if( s->npoints>0&&subsetsize!=0 )
    {
        if( randomstart )
        {
            mlprandomize(&session->network, _state);
        }
        minlbfgsrestartfrom(&session->optimizer, &session->network.weights, _state);
    }
    else
    {
        for(i=0; i<=wcount-1; i++)
        {
            session->network.weights.ptr.p_double[i] = (double)(0);
        }
    }
    if( algokind==-1 )
    {
        session->algoused = s->algokind;
        if( s->algokind==1 )
        {
            session->minibatchsize = s->minibatchsize;
        }
    }
    else
    {
        session->algoused = 0;
    }
    hqrndrandomize(&session->generator, _state);
    ae_vector_set_length(&session->rstate.ia, 15+1, _state);
    ae_vector_set_length(&session->rstate.ra, 1+1, _state);
    session->rstate.stage = -1;
}


/*************************************************************************
This function performs step-by-step training of the neural  network.  Here
"step-by-step" means  that training starts  with  MLPStartTrainingX  call,
and then user subsequently calls MLPContinueTrainingX  to perform one more
iteration of the training.

This  function  performs  one  more  iteration of the training and returns
either True (training continues) or False (training stopped). In case True
was returned, Network weights are updated according to the  current  state
of the optimization progress. In case False was  returned,  no  additional
updates is performed (previous update of  the  network weights moved us to
the final point, and no additional updates is needed).

EXAMPLE:
    >
    > [initialize network and trainer object]
    >
    > MLPStartTraining(Trainer, Network, True)
    > while MLPContinueTraining(Trainer, Network) do
    >     [visualize training progress]
    >


  -- ALGLIB --
     Copyright 13.08.2012 by Bochkanov Sergey
*************************************************************************/
static ae_bool mlptrain_mlpcontinuetrainingx(mlptrainer* s,
     /* Integer */ ae_vector* subset,
     ae_int_t subsetsize,
     ae_int_t* ngradbatch,
     smlptrnsession* session,
     ae_state *_state)
{
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t twcount;
    ae_int_t ntype;
    ae_int_t ttype;
    double decay;
    double v;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t trnsetsize;
    ae_int_t epoch;
    ae_int_t minibatchcount;
    ae_int_t minibatchidx;
    ae_int_t cursize;
    ae_int_t idx0;
    ae_int_t idx1;
    ae_bool result;


    
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
    if( session->rstate.stage>=0 )
    {
        nin = session->rstate.ia.ptr.p_int[0];
        nout = session->rstate.ia.ptr.p_int[1];
        wcount = session->rstate.ia.ptr.p_int[2];
        twcount = session->rstate.ia.ptr.p_int[3];
        ntype = session->rstate.ia.ptr.p_int[4];
        ttype = session->rstate.ia.ptr.p_int[5];
        i = session->rstate.ia.ptr.p_int[6];
        j = session->rstate.ia.ptr.p_int[7];
        k = session->rstate.ia.ptr.p_int[8];
        trnsetsize = session->rstate.ia.ptr.p_int[9];
        epoch = session->rstate.ia.ptr.p_int[10];
        minibatchcount = session->rstate.ia.ptr.p_int[11];
        minibatchidx = session->rstate.ia.ptr.p_int[12];
        cursize = session->rstate.ia.ptr.p_int[13];
        idx0 = session->rstate.ia.ptr.p_int[14];
        idx1 = session->rstate.ia.ptr.p_int[15];
        decay = session->rstate.ra.ptr.p_double[0];
        v = session->rstate.ra.ptr.p_double[1];
    }
    else
    {
        nin = -983;
        nout = -989;
        wcount = -834;
        twcount = 900;
        ntype = -287;
        ttype = 364;
        i = 214;
        j = -338;
        k = -686;
        trnsetsize = 912;
        epoch = 585;
        minibatchcount = 497;
        minibatchidx = -271;
        cursize = -581;
        idx0 = 745;
        idx1 = -533;
        decay = -77;
        v = 678;
    }
    if( session->rstate.stage==0 )
    {
        goto lbl_0;
    }
    
    /*
     * Routine body
     */
    
    /*
     * Check correctness of inputs
     */
    ae_assert(s->npoints>=0, "MLPContinueTrainingX: internal error - parameter S is not initialized or is spoiled(S.NPoints<0).", _state);
    if( s->rcpar )
    {
        ttype = 0;
    }
    else
    {
        ttype = 1;
    }
    if( !mlpissoftmax(&session->network, _state) )
    {
        ntype = 0;
    }
    else
    {
        ntype = 1;
    }
    ae_assert(ntype==ttype, "MLPContinueTrainingX: internal error - type of the resulting network is not similar to network type in trainer object.", _state);
    mlpproperties(&session->network, &nin, &nout, &wcount, _state);
    ae_assert(s->nin==nin, "MLPContinueTrainingX: internal error - number of inputs in trainer is not equal to number of inputs in the network.", _state);
    ae_assert(s->nout==nout, "MLPContinueTrainingX: internal error - number of outputs in trainer is not equal to number of outputs in the network.", _state);
    ae_assert(subset->cnt>=subsetsize, "MLPContinueTrainingX: internal error - parameter SubsetSize more than input subset size(Length(Subset)<SubsetSize).", _state);
    for(i=0; i<=subsetsize-1; i++)
    {
        ae_assert(subset->ptr.p_int[i]>=0&&subset->ptr.p_int[i]<=s->npoints-1, "MLPContinueTrainingX: internal error - parameter Subset contains incorrect index(Subset[I]<0 or Subset[I]>S.NPoints-1).", _state);
    }
    
    /*
     * Quick exit on empty training set
     */
    if( s->npoints==0||subsetsize==0 )
    {
        result = ae_false;
        return result;
    }
    
    /*
     * Minibatch training
     */
    if( session->algoused==1 )
    {
        ae_assert(ae_false, "MINIBATCH TRAINING IS NOT IMPLEMENTED YET", _state);
    }
    
    /*
     * Last option: full batch training
     */
    decay = s->decay;
lbl_1:
    if( !minlbfgsiteration(&session->optimizer, _state) )
    {
        goto lbl_2;
    }
    if( !session->optimizer.xupdated )
    {
        goto lbl_3;
    }
    ae_v_move(&session->network.weights.ptr.p_double[0], 1, &session->optimizer.x.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
    session->rstate.stage = 0;
    goto lbl_rcomm;
lbl_0:
lbl_3:
    ae_v_move(&session->network.weights.ptr.p_double[0], 1, &session->optimizer.x.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
    if( s->datatype==0 )
    {
        mlpgradbatchsubset(&session->network, &s->densexy, s->npoints, subset, subsetsize, &session->optimizer.f, &session->optimizer.g, _state);
    }
    if( s->datatype==1 )
    {
        mlpgradbatchsparsesubset(&session->network, &s->sparsexy, s->npoints, subset, subsetsize, &session->optimizer.f, &session->optimizer.g, _state);
    }
    
    /*
     * Increment number of operations performed on batch gradient
     */
    *ngradbatch = *ngradbatch+1;
    v = ae_v_dotproduct(&session->network.weights.ptr.p_double[0], 1, &session->network.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
    session->optimizer.f = session->optimizer.f+0.5*decay*v;
    ae_v_addd(&session->optimizer.g.ptr.p_double[0], 1, &session->network.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1), decay);
    goto lbl_1;
lbl_2:
    minlbfgsresultsbuf(&session->optimizer, &session->network.weights, &session->optimizerrep, _state);
    result = ae_false;
    return result;
    
    /*
     * Saving state
     */
lbl_rcomm:
    result = ae_true;
    session->rstate.ia.ptr.p_int[0] = nin;
    session->rstate.ia.ptr.p_int[1] = nout;
    session->rstate.ia.ptr.p_int[2] = wcount;
    session->rstate.ia.ptr.p_int[3] = twcount;
    session->rstate.ia.ptr.p_int[4] = ntype;
    session->rstate.ia.ptr.p_int[5] = ttype;
    session->rstate.ia.ptr.p_int[6] = i;
    session->rstate.ia.ptr.p_int[7] = j;
    session->rstate.ia.ptr.p_int[8] = k;
    session->rstate.ia.ptr.p_int[9] = trnsetsize;
    session->rstate.ia.ptr.p_int[10] = epoch;
    session->rstate.ia.ptr.p_int[11] = minibatchcount;
    session->rstate.ia.ptr.p_int[12] = minibatchidx;
    session->rstate.ia.ptr.p_int[13] = cursize;
    session->rstate.ia.ptr.p_int[14] = idx0;
    session->rstate.ia.ptr.p_int[15] = idx1;
    session->rstate.ra.ptr.p_double[0] = decay;
    session->rstate.ra.ptr.p_double[1] = v;
    return result;
}


/*************************************************************************
Internal bagging subroutine.

  -- ALGLIB --
     Copyright 19.02.2009 by Bochkanov Sergey
*************************************************************************/
static void mlptrain_mlpebagginginternal(mlpensemble* ensemble,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     double decay,
     ae_int_t restarts,
     double wstep,
     ae_int_t maxits,
     ae_bool lmalgorithm,
     ae_int_t* info,
     mlpreport* rep,
     mlpcvreport* ooberrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix xys;
    ae_vector s;
    ae_matrix oobbuf;
    ae_vector oobcntbuf;
    ae_vector x;
    ae_vector y;
    ae_vector dy;
    ae_vector dsbuf;
    ae_int_t ccnt;
    ae_int_t pcnt;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    mlpreport tmprep;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _mlpreport_clear(rep);
    _mlpcvreport_clear(ooberrors);
    ae_matrix_init(&xys, 0, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_BOOL, _state);
    ae_matrix_init(&oobbuf, 0, 0, DT_REAL, _state);
    ae_vector_init(&oobcntbuf, 0, DT_INT, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&dy, 0, DT_REAL, _state);
    ae_vector_init(&dsbuf, 0, DT_REAL, _state);
    _mlpreport_init(&tmprep, _state);
    _hqrndstate_init(&rs, _state);

    nin = mlpgetinputscount(&ensemble->network, _state);
    nout = mlpgetoutputscount(&ensemble->network, _state);
    wcount = mlpgetweightscount(&ensemble->network, _state);
    
    /*
     * Test for inputs
     */
    if( (!lmalgorithm&&ae_fp_eq(wstep,(double)(0)))&&maxits==0 )
    {
        *info = -8;
        ae_frame_leave(_state);
        return;
    }
    if( ((npoints<=0||restarts<1)||ae_fp_less(wstep,(double)(0)))||maxits<0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    if( mlpissoftmax(&ensemble->network, _state) )
    {
        for(i=0; i<=npoints-1; i++)
        {
            if( ae_round(xy->ptr.pp_double[i][nin], _state)<0||ae_round(xy->ptr.pp_double[i][nin], _state)>=nout )
            {
                *info = -2;
                ae_frame_leave(_state);
                return;
            }
        }
    }
    
    /*
     * allocate temporaries
     */
    *info = 2;
    rep->ngrad = 0;
    rep->nhess = 0;
    rep->ncholesky = 0;
    ooberrors->relclserror = (double)(0);
    ooberrors->avgce = (double)(0);
    ooberrors->rmserror = (double)(0);
    ooberrors->avgerror = (double)(0);
    ooberrors->avgrelerror = (double)(0);
    if( mlpissoftmax(&ensemble->network, _state) )
    {
        ccnt = nin+1;
        pcnt = nin;
    }
    else
    {
        ccnt = nin+nout;
        pcnt = nin+nout;
    }
    ae_matrix_set_length(&xys, npoints, ccnt, _state);
    ae_vector_set_length(&s, npoints, _state);
    ae_matrix_set_length(&oobbuf, npoints, nout, _state);
    ae_vector_set_length(&oobcntbuf, npoints, _state);
    ae_vector_set_length(&x, nin, _state);
    ae_vector_set_length(&y, nout, _state);
    if( mlpissoftmax(&ensemble->network, _state) )
    {
        ae_vector_set_length(&dy, 1, _state);
    }
    else
    {
        ae_vector_set_length(&dy, nout, _state);
    }
    for(i=0; i<=npoints-1; i++)
    {
        for(j=0; j<=nout-1; j++)
        {
            oobbuf.ptr.pp_double[i][j] = (double)(0);
        }
    }
    for(i=0; i<=npoints-1; i++)
    {
        oobcntbuf.ptr.p_int[i] = 0;
    }
    
    /*
     * main bagging cycle
     */
    hqrndrandomize(&rs, _state);
    for(k=0; k<=ensemble->ensemblesize-1; k++)
    {
        
        /*
         * prepare dataset
         */
        for(i=0; i<=npoints-1; i++)
        {
            s.ptr.p_bool[i] = ae_false;
        }
        for(i=0; i<=npoints-1; i++)
        {
            j = hqrnduniformi(&rs, npoints, _state);
            s.ptr.p_bool[j] = ae_true;
            ae_v_move(&xys.ptr.pp_double[i][0], 1, &xy->ptr.pp_double[j][0], 1, ae_v_len(0,ccnt-1));
        }
        
        /*
         * train
         */
        if( lmalgorithm )
        {
            mlptrainlm(&ensemble->network, &xys, npoints, decay, restarts, info, &tmprep, _state);
        }
        else
        {
            mlptrainlbfgs(&ensemble->network, &xys, npoints, decay, restarts, wstep, maxits, info, &tmprep, _state);
        }
        if( *info<0 )
        {
            ae_frame_leave(_state);
            return;
        }
        
        /*
         * save results
         */
        rep->ngrad = rep->ngrad+tmprep.ngrad;
        rep->nhess = rep->nhess+tmprep.nhess;
        rep->ncholesky = rep->ncholesky+tmprep.ncholesky;
        ae_v_move(&ensemble->weights.ptr.p_double[k*wcount], 1, &ensemble->network.weights.ptr.p_double[0], 1, ae_v_len(k*wcount,(k+1)*wcount-1));
        ae_v_move(&ensemble->columnmeans.ptr.p_double[k*pcnt], 1, &ensemble->network.columnmeans.ptr.p_double[0], 1, ae_v_len(k*pcnt,(k+1)*pcnt-1));
        ae_v_move(&ensemble->columnsigmas.ptr.p_double[k*pcnt], 1, &ensemble->network.columnsigmas.ptr.p_double[0], 1, ae_v_len(k*pcnt,(k+1)*pcnt-1));
        
        /*
         * OOB estimates
         */
        for(i=0; i<=npoints-1; i++)
        {
            if( !s.ptr.p_bool[i] )
            {
                ae_v_move(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nin-1));
                mlpprocess(&ensemble->network, &x, &y, _state);
                ae_v_add(&oobbuf.ptr.pp_double[i][0], 1, &y.ptr.p_double[0], 1, ae_v_len(0,nout-1));
                oobcntbuf.ptr.p_int[i] = oobcntbuf.ptr.p_int[i]+1;
            }
        }
    }
    
    /*
     * OOB estimates
     */
    if( mlpissoftmax(&ensemble->network, _state) )
    {
        dserrallocate(nout, &dsbuf, _state);
    }
    else
    {
        dserrallocate(-nout, &dsbuf, _state);
    }
    for(i=0; i<=npoints-1; i++)
    {
        if( oobcntbuf.ptr.p_int[i]!=0 )
        {
            v = (double)1/(double)oobcntbuf.ptr.p_int[i];
            ae_v_moved(&y.ptr.p_double[0], 1, &oobbuf.ptr.pp_double[i][0], 1, ae_v_len(0,nout-1), v);
            if( mlpissoftmax(&ensemble->network, _state) )
            {
                dy.ptr.p_double[0] = xy->ptr.pp_double[i][nin];
            }
            else
            {
                ae_v_moved(&dy.ptr.p_double[0], 1, &xy->ptr.pp_double[i][nin], 1, ae_v_len(0,nout-1), v);
            }
            dserraccumulate(&dsbuf, &y, &dy, _state);
        }
    }
    dserrfinish(&dsbuf, _state);
    ooberrors->relclserror = dsbuf.ptr.p_double[0];
    ooberrors->avgce = dsbuf.ptr.p_double[1];
    ooberrors->rmserror = dsbuf.ptr.p_double[2];
    ooberrors->avgerror = dsbuf.ptr.p_double[3];
    ooberrors->avgrelerror = dsbuf.ptr.p_double[4];
    ae_frame_leave(_state);
}


/*************************************************************************
This function initializes temporaries needed for training session.


  -- ALGLIB --
     Copyright 01.07.2013 by Bochkanov Sergey
*************************************************************************/
static void mlptrain_initmlptrnsession(multilayerperceptron* networktrained,
     ae_bool randomizenetwork,
     mlptrainer* trainer,
     smlptrnsession* session,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t pcount;
    ae_vector dummysubset;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&dummysubset, 0, DT_INT, _state);

    
    /*
     * Prepare network:
     * * copy input network to Session.Network
     * * re-initialize preprocessor and weights if RandomizeNetwork=True
     */
    mlpcopy(networktrained, &session->network, _state);
    if( randomizenetwork )
    {
        ae_assert(trainer->datatype==0||trainer->datatype==1, "InitTemporaries: unexpected Trainer.DataType", _state);
        if( trainer->datatype==0 )
        {
            mlpinitpreprocessorsubset(&session->network, &trainer->densexy, trainer->npoints, &dummysubset, -1, _state);
        }
        if( trainer->datatype==1 )
        {
            mlpinitpreprocessorsparsesubset(&session->network, &trainer->sparsexy, trainer->npoints, &dummysubset, -1, _state);
        }
        mlprandomize(&session->network, _state);
        session->randomizenetwork = ae_true;
    }
    else
    {
        session->randomizenetwork = ae_false;
    }
    
    /*
     * Determine network geometry and initialize optimizer 
     */
    mlpproperties(&session->network, &nin, &nout, &wcount, _state);
    minlbfgscreate(wcount, ae_minint(wcount, trainer->lbfgsfactor, _state), &session->network.weights, &session->optimizer, _state);
    minlbfgssetxrep(&session->optimizer, ae_true, _state);
    
    /*
     * Create buffers
     */
    ae_vector_set_length(&session->wbuf0, wcount, _state);
    ae_vector_set_length(&session->wbuf1, wcount, _state);
    
    /*
     * Initialize session result
     */
    mlpexporttunableparameters(&session->network, &session->bestparameters, &pcount, _state);
    session->bestrmserror = ae_maxrealnumber;
    ae_frame_leave(_state);
}


/*************************************************************************
This function initializes temporaries needed for training session.

*************************************************************************/
static void mlptrain_initmlptrnsessions(multilayerperceptron* networktrained,
     ae_bool randomizenetwork,
     mlptrainer* trainer,
     ae_shared_pool* sessions,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector dummysubset;
    smlptrnsession t;
    smlptrnsession *p;
    ae_smart_ptr _p;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&dummysubset, 0, DT_INT, _state);
    _smlptrnsession_init(&t, _state);
    ae_smart_ptr_init(&_p, (void**)&p, _state);

    if( ae_shared_pool_is_initialized(sessions) )
    {
        
        /*
         * Pool was already initialized.
         * Clear sessions stored in the pool.
         */
        ae_shared_pool_first_recycled(sessions, &_p, _state);
        while(p!=NULL)
        {
            ae_assert(mlpsamearchitecture(&p->network, networktrained, _state), "InitMLPTrnSessions: internal consistency error", _state);
            p->bestrmserror = ae_maxrealnumber;
            ae_shared_pool_next_recycled(sessions, &_p, _state);
        }
    }
    else
    {
        
        /*
         * Prepare session and seed pool
         */
        mlptrain_initmlptrnsession(networktrained, randomizenetwork, trainer, &t, _state);
        ae_shared_pool_set_seed(sessions, &t, sizeof(t), _smlptrnsession_init, _smlptrnsession_init_copy, _smlptrnsession_destroy, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function initializes temporaries needed for ensemble training.

*************************************************************************/
static void mlptrain_initmlpetrnsession(multilayerperceptron* individualnetwork,
     mlptrainer* trainer,
     mlpetrnsession* session,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector dummysubset;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&dummysubset, 0, DT_INT, _state);

    
    /*
     * Prepare network:
     * * copy input network to Session.Network
     * * re-initialize preprocessor and weights if RandomizeNetwork=True
     */
    mlpcopy(individualnetwork, &session->network, _state);
    mlptrain_initmlptrnsessions(individualnetwork, ae_true, trainer, &session->mlpsessions, _state);
    ivectorsetlengthatleast(&session->trnsubset, trainer->npoints, _state);
    ivectorsetlengthatleast(&session->valsubset, trainer->npoints, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
This function initializes temporaries needed for training session.

*************************************************************************/
static void mlptrain_initmlpetrnsessions(multilayerperceptron* individualnetwork,
     mlptrainer* trainer,
     ae_shared_pool* sessions,
     ae_state *_state)
{
    ae_frame _frame_block;
    mlpetrnsession t;

    ae_frame_make(_state, &_frame_block);
    _mlpetrnsession_init(&t, _state);

    if( !ae_shared_pool_is_initialized(sessions) )
    {
        mlptrain_initmlpetrnsession(individualnetwork, trainer, &t, _state);
        ae_shared_pool_set_seed(sessions, &t, sizeof(t), _mlpetrnsession_init, _mlpetrnsession_init_copy, _mlpetrnsession_destroy, _state);
    }
    ae_frame_leave(_state);
}


void _mlpreport_init(void* _p, ae_state *_state)
{
    mlpreport *p = (mlpreport*)_p;
    ae_touch_ptr((void*)p);
}


void _mlpreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    mlpreport *dst = (mlpreport*)_dst;
    mlpreport *src = (mlpreport*)_src;
    dst->relclserror = src->relclserror;
    dst->avgce = src->avgce;
    dst->rmserror = src->rmserror;
    dst->avgerror = src->avgerror;
    dst->avgrelerror = src->avgrelerror;
    dst->ngrad = src->ngrad;
    dst->nhess = src->nhess;
    dst->ncholesky = src->ncholesky;
}


void _mlpreport_clear(void* _p)
{
    mlpreport *p = (mlpreport*)_p;
    ae_touch_ptr((void*)p);
}


void _mlpreport_destroy(void* _p)
{
    mlpreport *p = (mlpreport*)_p;
    ae_touch_ptr((void*)p);
}


void _mlpcvreport_init(void* _p, ae_state *_state)
{
    mlpcvreport *p = (mlpcvreport*)_p;
    ae_touch_ptr((void*)p);
}


void _mlpcvreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    mlpcvreport *dst = (mlpcvreport*)_dst;
    mlpcvreport *src = (mlpcvreport*)_src;
    dst->relclserror = src->relclserror;
    dst->avgce = src->avgce;
    dst->rmserror = src->rmserror;
    dst->avgerror = src->avgerror;
    dst->avgrelerror = src->avgrelerror;
}


void _mlpcvreport_clear(void* _p)
{
    mlpcvreport *p = (mlpcvreport*)_p;
    ae_touch_ptr((void*)p);
}


void _mlpcvreport_destroy(void* _p)
{
    mlpcvreport *p = (mlpcvreport*)_p;
    ae_touch_ptr((void*)p);
}


void _smlptrnsession_init(void* _p, ae_state *_state)
{
    smlptrnsession *p = (smlptrnsession*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->bestparameters, 0, DT_REAL, _state);
    _multilayerperceptron_init(&p->network, _state);
    _minlbfgsstate_init(&p->optimizer, _state);
    _minlbfgsreport_init(&p->optimizerrep, _state);
    ae_vector_init(&p->wbuf0, 0, DT_REAL, _state);
    ae_vector_init(&p->wbuf1, 0, DT_REAL, _state);
    ae_vector_init(&p->allminibatches, 0, DT_INT, _state);
    ae_vector_init(&p->currentminibatch, 0, DT_INT, _state);
    _rcommstate_init(&p->rstate, _state);
    _hqrndstate_init(&p->generator, _state);
}


void _smlptrnsession_init_copy(void* _dst, void* _src, ae_state *_state)
{
    smlptrnsession *dst = (smlptrnsession*)_dst;
    smlptrnsession *src = (smlptrnsession*)_src;
    ae_vector_init_copy(&dst->bestparameters, &src->bestparameters, _state);
    dst->bestrmserror = src->bestrmserror;
    dst->randomizenetwork = src->randomizenetwork;
    _multilayerperceptron_init_copy(&dst->network, &src->network, _state);
    _minlbfgsstate_init_copy(&dst->optimizer, &src->optimizer, _state);
    _minlbfgsreport_init_copy(&dst->optimizerrep, &src->optimizerrep, _state);
    ae_vector_init_copy(&dst->wbuf0, &src->wbuf0, _state);
    ae_vector_init_copy(&dst->wbuf1, &src->wbuf1, _state);
    ae_vector_init_copy(&dst->allminibatches, &src->allminibatches, _state);
    ae_vector_init_copy(&dst->currentminibatch, &src->currentminibatch, _state);
    _rcommstate_init_copy(&dst->rstate, &src->rstate, _state);
    dst->algoused = src->algoused;
    dst->minibatchsize = src->minibatchsize;
    _hqrndstate_init_copy(&dst->generator, &src->generator, _state);
}


void _smlptrnsession_clear(void* _p)
{
    smlptrnsession *p = (smlptrnsession*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->bestparameters);
    _multilayerperceptron_clear(&p->network);
    _minlbfgsstate_clear(&p->optimizer);
    _minlbfgsreport_clear(&p->optimizerrep);
    ae_vector_clear(&p->wbuf0);
    ae_vector_clear(&p->wbuf1);
    ae_vector_clear(&p->allminibatches);
    ae_vector_clear(&p->currentminibatch);
    _rcommstate_clear(&p->rstate);
    _hqrndstate_clear(&p->generator);
}


void _smlptrnsession_destroy(void* _p)
{
    smlptrnsession *p = (smlptrnsession*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->bestparameters);
    _multilayerperceptron_destroy(&p->network);
    _minlbfgsstate_destroy(&p->optimizer);
    _minlbfgsreport_destroy(&p->optimizerrep);
    ae_vector_destroy(&p->wbuf0);
    ae_vector_destroy(&p->wbuf1);
    ae_vector_destroy(&p->allminibatches);
    ae_vector_destroy(&p->currentminibatch);
    _rcommstate_destroy(&p->rstate);
    _hqrndstate_destroy(&p->generator);
}


void _mlpetrnsession_init(void* _p, ae_state *_state)
{
    mlpetrnsession *p = (mlpetrnsession*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->trnsubset, 0, DT_INT, _state);
    ae_vector_init(&p->valsubset, 0, DT_INT, _state);
    ae_shared_pool_init(&p->mlpsessions, _state);
    _mlpreport_init(&p->mlprep, _state);
    _multilayerperceptron_init(&p->network, _state);
}


void _mlpetrnsession_init_copy(void* _dst, void* _src, ae_state *_state)
{
    mlpetrnsession *dst = (mlpetrnsession*)_dst;
    mlpetrnsession *src = (mlpetrnsession*)_src;
    ae_vector_init_copy(&dst->trnsubset, &src->trnsubset, _state);
    ae_vector_init_copy(&dst->valsubset, &src->valsubset, _state);
    ae_shared_pool_init_copy(&dst->mlpsessions, &src->mlpsessions, _state);
    _mlpreport_init_copy(&dst->mlprep, &src->mlprep, _state);
    _multilayerperceptron_init_copy(&dst->network, &src->network, _state);
}


void _mlpetrnsession_clear(void* _p)
{
    mlpetrnsession *p = (mlpetrnsession*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->trnsubset);
    ae_vector_clear(&p->valsubset);
    ae_shared_pool_clear(&p->mlpsessions);
    _mlpreport_clear(&p->mlprep);
    _multilayerperceptron_clear(&p->network);
}


void _mlpetrnsession_destroy(void* _p)
{
    mlpetrnsession *p = (mlpetrnsession*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->trnsubset);
    ae_vector_destroy(&p->valsubset);
    ae_shared_pool_destroy(&p->mlpsessions);
    _mlpreport_destroy(&p->mlprep);
    _multilayerperceptron_destroy(&p->network);
}


void _mlptrainer_init(void* _p, ae_state *_state)
{
    mlptrainer *p = (mlptrainer*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_init(&p->densexy, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&p->sparsexy, _state);
    _smlptrnsession_init(&p->session, _state);
    ae_vector_init(&p->subset, 0, DT_INT, _state);
    ae_vector_init(&p->valsubset, 0, DT_INT, _state);
}


void _mlptrainer_init_copy(void* _dst, void* _src, ae_state *_state)
{
    mlptrainer *dst = (mlptrainer*)_dst;
    mlptrainer *src = (mlptrainer*)_src;
    dst->nin = src->nin;
    dst->nout = src->nout;
    dst->rcpar = src->rcpar;
    dst->lbfgsfactor = src->lbfgsfactor;
    dst->decay = src->decay;
    dst->wstep = src->wstep;
    dst->maxits = src->maxits;
    dst->datatype = src->datatype;
    dst->npoints = src->npoints;
    ae_matrix_init_copy(&dst->densexy, &src->densexy, _state);
    _sparsematrix_init_copy(&dst->sparsexy, &src->sparsexy, _state);
    _smlptrnsession_init_copy(&dst->session, &src->session, _state);
    dst->ngradbatch = src->ngradbatch;
    ae_vector_init_copy(&dst->subset, &src->subset, _state);
    dst->subsetsize = src->subsetsize;
    ae_vector_init_copy(&dst->valsubset, &src->valsubset, _state);
    dst->valsubsetsize = src->valsubsetsize;
    dst->algokind = src->algokind;
    dst->minibatchsize = src->minibatchsize;
}


void _mlptrainer_clear(void* _p)
{
    mlptrainer *p = (mlptrainer*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_clear(&p->densexy);
    _sparsematrix_clear(&p->sparsexy);
    _smlptrnsession_clear(&p->session);
    ae_vector_clear(&p->subset);
    ae_vector_clear(&p->valsubset);
}


void _mlptrainer_destroy(void* _p)
{
    mlptrainer *p = (mlptrainer*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_destroy(&p->densexy);
    _sparsematrix_destroy(&p->sparsexy);
    _smlptrnsession_destroy(&p->session);
    ae_vector_destroy(&p->subset);
    ae_vector_destroy(&p->valsubset);
}


void _mlpparallelizationcv_init(void* _p, ae_state *_state)
{
    mlpparallelizationcv *p = (mlpparallelizationcv*)_p;
    ae_touch_ptr((void*)p);
    _multilayerperceptron_init(&p->network, _state);
    _mlpreport_init(&p->rep, _state);
    ae_vector_init(&p->subset, 0, DT_INT, _state);
    ae_vector_init(&p->xyrow, 0, DT_REAL, _state);
    ae_vector_init(&p->y, 0, DT_REAL, _state);
    ae_shared_pool_init(&p->trnpool, _state);
}


void _mlpparallelizationcv_init_copy(void* _dst, void* _src, ae_state *_state)
{
    mlpparallelizationcv *dst = (mlpparallelizationcv*)_dst;
    mlpparallelizationcv *src = (mlpparallelizationcv*)_src;
    _multilayerperceptron_init_copy(&dst->network, &src->network, _state);
    _mlpreport_init_copy(&dst->rep, &src->rep, _state);
    ae_vector_init_copy(&dst->subset, &src->subset, _state);
    dst->subsetsize = src->subsetsize;
    ae_vector_init_copy(&dst->xyrow, &src->xyrow, _state);
    ae_vector_init_copy(&dst->y, &src->y, _state);
    dst->ngrad = src->ngrad;
    ae_shared_pool_init_copy(&dst->trnpool, &src->trnpool, _state);
}


void _mlpparallelizationcv_clear(void* _p)
{
    mlpparallelizationcv *p = (mlpparallelizationcv*)_p;
    ae_touch_ptr((void*)p);
    _multilayerperceptron_clear(&p->network);
    _mlpreport_clear(&p->rep);
    ae_vector_clear(&p->subset);
    ae_vector_clear(&p->xyrow);
    ae_vector_clear(&p->y);
    ae_shared_pool_clear(&p->trnpool);
}


void _mlpparallelizationcv_destroy(void* _p)
{
    mlpparallelizationcv *p = (mlpparallelizationcv*)_p;
    ae_touch_ptr((void*)p);
    _multilayerperceptron_destroy(&p->network);
    _mlpreport_destroy(&p->rep);
    ae_vector_destroy(&p->subset);
    ae_vector_destroy(&p->xyrow);
    ae_vector_destroy(&p->y);
    ae_shared_pool_destroy(&p->trnpool);
}


/*$ End $*/
