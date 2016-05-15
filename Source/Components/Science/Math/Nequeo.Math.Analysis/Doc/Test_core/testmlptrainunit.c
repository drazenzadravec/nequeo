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
#include "testmlptrainunit.h"


/*$ Declarations $*/
static ae_bool testmlptrainunit_testmlptraines(ae_state *_state);
static ae_bool testmlptrainunit_testmlptrainregr(ae_state *_state);
static ae_bool testmlptrainunit_testmlpxorregr(ae_state *_state);
static ae_bool testmlptrainunit_testmlptrainclass(ae_state *_state);
static ae_bool testmlptrainunit_testmlpxorcls(ae_state *_state);
static ae_bool testmlptrainunit_testmlpzeroweights(ae_state *_state);
static ae_bool testmlptrainunit_testmlprestarts(ae_state *_state);
static ae_bool testmlptrainunit_testmlpcverror(ae_state *_state);
static ae_bool testmlptrainunit_testmlptrainens(ae_state *_state);
static ae_bool testmlptrainunit_testmlptrainensregr(ae_state *_state);
static ae_bool testmlptrainunit_testmlptrainenscls(ae_state *_state);


/*$ Body $*/


ae_bool testmlptrain(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_bool waserrors;
    ae_int_t maxn;
    ae_int_t maxhid;
    ae_int_t info;
    multilayerperceptron network;
    multilayerperceptron network2;
    mlpreport rep;
    mlpcvreport cvrep;
    ae_matrix xy;
    ae_matrix valxy;
    ae_bool trnerrors;
    ae_bool mlpcverrorerr;
    ae_bool mlptrainregrerr;
    ae_bool mlptrainclasserr;
    ae_bool mlprestartserr;
    ae_bool mlpxorregrerr;
    ae_bool mlpxorclserr;
    ae_bool mlptrainenserrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&network, _state);
    _multilayerperceptron_init(&network2, _state);
    _mlpreport_init(&rep, _state);
    _mlpcvreport_init(&cvrep, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&valxy, 0, 0, DT_REAL, _state);

    waserrors = ae_false;
    trnerrors = ae_false;
    mlpcverrorerr = ae_false;
    mlptrainregrerr = ae_false;
    mlptrainclasserr = ae_false;
    mlprestartserr = ae_false;
    mlpxorregrerr = ae_false;
    mlpxorclserr = ae_false;
    mlptrainenserrors = ae_false;
    maxn = 4;
    maxhid = 4;
    
    /*
     * Test network training on simple XOR problem
     */
    ae_matrix_set_length(&xy, 3+1, 2+1, _state);
    xy.ptr.pp_double[0][0] = (double)(-1);
    xy.ptr.pp_double[0][1] = (double)(-1);
    xy.ptr.pp_double[0][2] = (double)(-1);
    xy.ptr.pp_double[1][0] = (double)(1);
    xy.ptr.pp_double[1][1] = (double)(-1);
    xy.ptr.pp_double[1][2] = (double)(1);
    xy.ptr.pp_double[2][0] = (double)(-1);
    xy.ptr.pp_double[2][1] = (double)(1);
    xy.ptr.pp_double[2][2] = (double)(1);
    xy.ptr.pp_double[3][0] = (double)(1);
    xy.ptr.pp_double[3][1] = (double)(1);
    xy.ptr.pp_double[3][2] = (double)(-1);
    mlpcreate1(2, 2, 1, &network, _state);
    mlptrainlm(&network, &xy, 4, 0.001, 10, &info, &rep, _state);
    trnerrors = trnerrors||ae_fp_greater(mlprmserror(&network, &xy, 4, _state),0.1);
    
    /*
     * Test early stopping
     */
    trnerrors = trnerrors||testmlptrainunit_testmlptraines(_state);
    
    /*
     * Test for function MLPFoldCV()
     */
    mlpcverrorerr = testmlptrainunit_testmlpcverror(_state);
    
    /*
     * Test for training functions
     */
    mlptrainregrerr = testmlptrainunit_testmlptrainregr(_state)||testmlptrainunit_testmlpzeroweights(_state);
    mlptrainclasserr = testmlptrainunit_testmlptrainclass(_state);
    mlprestartserr = testmlptrainunit_testmlprestarts(_state);
    mlpxorregrerr = testmlptrainunit_testmlpxorregr(_state);
    mlpxorclserr = testmlptrainunit_testmlpxorcls(_state);
    
    /*
     * Training for ensembles
     */
    mlptrainenserrors = (testmlptrainunit_testmlptrainens(_state)||testmlptrainunit_testmlptrainensregr(_state))||testmlptrainunit_testmlptrainenscls(_state);
    
    /*
     * Final report
     */
    waserrors = ((((((trnerrors||mlptrainregrerr)||mlptrainclasserr)||mlprestartserr)||mlpxorregrerr)||mlpxorclserr)||mlpcverrorerr)||mlptrainenserrors;
    if( !silent )
    {
        printf("MLP TEST\n");
        printf("CROSS-VALIDATION ERRORS:                 ");
        if( !mlpcverrorerr )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("TRAINING:                                ");
        if( !trnerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("TRAIN -LM -LBFGS FOR REGRESSION:         ");
        if( mlptrainregrerr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("TRAIN -LM -LBFGS FOR CLASSIFIER:         ");
        if( mlptrainclasserr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("PARAMETER RESTARTS IN TRAIN -LBFGS:      ");
        if( mlprestartserr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("TRAINIG WITH TRAINER FOR REGRESSION:     ");
        if( mlpxorregrerr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("TRAINIG WITH TRAINER FOR CLASSIFIER:     ");
        if( mlpxorclserr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("TRAINING ENSEMBLES:                      ");
        if( mlptrainenserrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        if( waserrors )
        {
            printf("TEST SUMMARY: FAILED\n");
        }
        else
        {
            printf("TEST SUMMARY: PASSED\n");
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
ae_bool _pexec_testmlptrain(ae_bool silent, ae_state *_state)
{
    return testmlptrain(silent, _state);
}


/*************************************************************************
This function tests MLPTrainES().

It returns True in case of errors, False when no errors were detected
*************************************************************************/
static ae_bool testmlptrainunit_testmlptraines(ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t pass;
    ae_int_t passcount;
    multilayerperceptron network;
    ae_matrix trnxy;
    ae_matrix valxy;
    ae_vector x;
    ae_vector y;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_int_t nrestarts;
    ae_int_t info;
    mlpreport rep;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&network, _state);
    ae_matrix_init(&trnxy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&valxy, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    _mlpreport_init(&rep, _state);

    result = ae_false;
    
    /*
     * First test checks that MLPTrainES() - when training set is equal to the validation
     * set, MLPTrainES() behaves just like a "normal" training algorithm.
     *
     * Test sequence:
     * * generate training set - 100 random points from 2D square [-1,+1]*[-1,+1]
     * * generate network with 2 inputs, no hidden layers, nonlinear output layer,
     *   use its outputs as target values for the test set
     * * randomize network
     * * train with MLPTrainES(), using original set as both training and validation set
     * * trained network must reproduce training set with good precision
     *
     * NOTE: it is important to test algorithm on nonlinear network because linear
     *       problems converge too fast. Slow convergence is important to detect
     *       some kinds of bugs.
     *
     * NOTE: it is important to have NRestarts at least equal to 5, because with just
     *       one restart algorithm fails test about once in several thousands of passes.
     */
    passcount = 10;
    nrestarts = 5;
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * Create network, generate training/validation sets
         */
        mlpcreater0(2, 1, -2.0, 2.0, &network, _state);
        mlprandomize(&network, _state);
        n = 100;
        ae_matrix_set_length(&trnxy, n, 3, _state);
        ae_matrix_set_length(&valxy, n, 3, _state);
        ae_vector_set_length(&x, 2, _state);
        ae_vector_set_length(&y, 1, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=1; j++)
            {
                trnxy.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                valxy.ptr.pp_double[i][j] = trnxy.ptr.pp_double[i][j];
                x.ptr.p_double[j] = trnxy.ptr.pp_double[i][j];
            }
            mlpprocess(&network, &x, &y, _state);
            trnxy.ptr.pp_double[i][2] = y.ptr.p_double[0];
            valxy.ptr.pp_double[i][2] = y.ptr.p_double[0];
        }
        mlprandomize(&network, _state);
        mlptraines(&network, &trnxy, n, &valxy, n, 0.0001, nrestarts, &info, &rep, _state);
        if( info<=0 )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        if( ae_fp_greater(ae_sqrt(mlperror(&network, &valxy, n, _state)/n, _state),0.01) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This  function   tests   MLPTrainLM,  MLPTrainLBFGS   and  MLPTrainNetwork
functions  for regression.  It check that  train functions  work correctly.
Test use Create1 with 10 neurons.
Test function is f(x,y)=X^2+cos(3*Pi*y).
*************************************************************************/
static ae_bool testmlptrainunit_testmlptrainregr(ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;
    mlptrainer trainer;
    mlpreport rep;
    ae_int_t info;
    ae_matrix xy;
    sparsematrix sm;
    ae_vector x;
    ae_vector y;
    ae_int_t n;
    ae_int_t sn;
    ae_int_t nneurons;
    double vdecay;
    double averr;
    double st;
    double eps;
    double traineps;
    ae_int_t nneedrest;
    ae_int_t trainits;
    ae_int_t shift;
    ae_int_t i;
    ae_int_t j;
    ae_int_t vtrain;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&net, _state);
    _mlptrainer_init(&trainer, _state);
    _mlpreport_init(&rep, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&sm, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);

    eps = 0.01;
    vdecay = 0.001;
    nneurons = 10;
    nneedrest = 5;
    traineps = 1.0E-3;
    trainits = 0;
    sn = 5;
    n = sn*sn;
    st = (double)2/(double)(sn-1);
    sparsecreate(n, 3, n*3, &sm, _state);
    ae_matrix_set_length(&xy, n, 3, _state);
    ae_vector_set_length(&x, 2, _state);
    for(vtrain=0; vtrain<=3; vtrain++)
    {
        averr = (double)(0);
        
        /*
         * Create a train set(uniformly distributed set of points).
         */
        for(i=0; i<=sn-1; i++)
        {
            for(j=0; j<=sn-1; j++)
            {
                shift = i*sn+j;
                xy.ptr.pp_double[shift][0] = i*st-1.0;
                xy.ptr.pp_double[shift][1] = j*st-1.0;
                xy.ptr.pp_double[shift][2] = xy.ptr.pp_double[shift][0]*xy.ptr.pp_double[shift][0]+ae_cos(3*ae_pi*xy.ptr.pp_double[shift][1], _state);
            }
        }
        
        /*
         * Create and train a neural network
         */
        mlpcreate1(2, nneurons, 1, &net, _state);
        if( vtrain==0 )
        {
            mlptrainlm(&net, &xy, n, vdecay, nneedrest, &info, &rep, _state);
        }
        if( vtrain==1 )
        {
            mlptrainlbfgs(&net, &xy, n, vdecay, nneedrest, traineps, trainits, &info, &rep, _state);
        }
        
        /*
         * Train with trainer, using:
         *  * dense matrix;
         */
        if( vtrain==2 )
        {
            mlpcreatetrainer(2, 1, &trainer, _state);
            mlpsetdataset(&trainer, &xy, n, _state);
            mlpsetdecay(&trainer, vdecay, _state);
            mlpsetcond(&trainer, traineps, trainits, _state);
            mlptrainnetwork(&trainer, &net, nneedrest, &rep, _state);
        }
        
        /*
         *  * sparse matrix.
         */
        if( vtrain==3 )
        {
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=2; j++)
                {
                    sparseset(&sm, i, j, xy.ptr.pp_double[i][j], _state);
                }
            }
            mlpcreatetrainer(2, 1, &trainer, _state);
            mlpsetsparsedataset(&trainer, &sm, n, _state);
            mlpsetdecay(&trainer, vdecay, _state);
            mlpsetcond(&trainer, traineps, trainits, _state);
            mlptrainnetwork(&trainer, &net, nneedrest, &rep, _state);
        }
        
        /*
         * Check that network is trained correctly
         */
        for(i=0; i<=n-1; i++)
        {
            x.ptr.p_double[0] = xy.ptr.pp_double[i][0];
            x.ptr.p_double[1] = xy.ptr.pp_double[i][1];
            mlpprocess(&net, &x, &y, _state);
            
            /*
             * Calculate average error
             */
            averr = averr+ae_fabs(y.ptr.p_double[0]-xy.ptr.pp_double[i][2], _state);
        }
        if( ae_fp_greater(averr/n,eps) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This  function tests  MLPTrainNetwork/MLPStartTraining/MLPContinueTraining
functions for  regression.  It check  that train  functions work correctly.
Test use Create1 with 2 neurons.
Test function is XOR(x,y).
*************************************************************************/
static ae_bool testmlptrainunit_testmlpxorregr(ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;
    mlptrainer trainer;
    mlpreport rep;
    ae_matrix xy;
    sparsematrix sm;
    ae_vector x;
    ae_vector y;
    ae_int_t n;
    ae_int_t sn;
    ae_int_t nneurons;
    double vdecay;
    double averr;
    double eps;
    ae_int_t numxp;
    double traineps;
    ae_int_t nneedrest;
    ae_int_t trainits;
    ae_int_t shift;
    ae_int_t i;
    ae_int_t j;
    ae_int_t vtrain;
    ae_int_t xp;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&net, _state);
    _mlptrainer_init(&trainer, _state);
    _mlpreport_init(&rep, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&sm, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);

    eps = 0.01;
    numxp = 15;
    vdecay = 0.001;
    nneurons = 3;
    nneedrest = 1;
    traineps = 1.0E-4;
    trainits = 0;
    sn = 2;
    n = sn*sn;
    sparsecreate(n, 3, n*3, &sm, _state);
    ae_matrix_set_length(&xy, n, 3, _state);
    ae_vector_set_length(&x, 2, _state);
    for(xp=1; xp<=numxp; xp++)
    {
        for(vtrain=0; vtrain<=3; vtrain++)
        {
            averr = (double)(0);
            
            /*
             * Create a train set(uniformly distributed set of points).
             */
            for(i=0; i<=sn-1; i++)
            {
                for(j=0; j<=sn-1; j++)
                {
                    shift = i*sn+j;
                    xy.ptr.pp_double[shift][0] = (double)(i);
                    xy.ptr.pp_double[shift][1] = (double)(j);
                    if( ae_fp_eq(xy.ptr.pp_double[shift][0],xy.ptr.pp_double[shift][1]) )
                    {
                        xy.ptr.pp_double[shift][2] = (double)(0);
                    }
                    else
                    {
                        xy.ptr.pp_double[shift][2] = (double)(1);
                    }
                }
            }
            
            /*
             * Create and train a neural network
             */
            mlpcreate1(2, nneurons, 1, &net, _state);
            
            /*
             * Train with trainer, using:
             *  * dense matrix;
             */
            if( vtrain==0 )
            {
                mlpcreatetrainer(2, 1, &trainer, _state);
                mlpsetdataset(&trainer, &xy, n, _state);
                mlpsetdecay(&trainer, vdecay, _state);
                mlpsetcond(&trainer, traineps, trainits, _state);
                mlptrainnetwork(&trainer, &net, nneedrest, &rep, _state);
            }
            if( vtrain==1 )
            {
                mlpcreatetrainer(2, 1, &trainer, _state);
                mlpsetdataset(&trainer, &xy, n, _state);
                mlpsetdecay(&trainer, vdecay, _state);
                mlpsetcond(&trainer, traineps, trainits, _state);
                mlpstarttraining(&trainer, &net, ae_true, _state);
                while(mlpcontinuetraining(&trainer, &net, _state))
                {
                }
            }
            
            /*
             *  * sparse matrix.
             */
            if( vtrain==2 )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=2; j++)
                    {
                        sparseset(&sm, i, j, xy.ptr.pp_double[i][j], _state);
                    }
                }
                mlpcreatetrainer(2, 1, &trainer, _state);
                mlpsetsparsedataset(&trainer, &sm, n, _state);
                mlpsetdecay(&trainer, vdecay, _state);
                mlpsetcond(&trainer, traineps, trainits, _state);
                mlptrainnetwork(&trainer, &net, nneedrest, &rep, _state);
            }
            if( vtrain==3 )
            {
                for(i=0; i<=n-1; i++)
                {
                    for(j=0; j<=2; j++)
                    {
                        sparseset(&sm, i, j, xy.ptr.pp_double[i][j], _state);
                    }
                }
                mlpcreatetrainer(2, 1, &trainer, _state);
                mlpsetsparsedataset(&trainer, &sm, n, _state);
                mlpsetdecay(&trainer, vdecay, _state);
                mlpsetcond(&trainer, traineps, trainits, _state);
                mlpstarttraining(&trainer, &net, ae_true, _state);
                while(mlpcontinuetraining(&trainer, &net, _state))
                {
                }
            }
            
            /*
             * Check that network is trained correctly
             */
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[0] = xy.ptr.pp_double[i][0];
                x.ptr.p_double[1] = xy.ptr.pp_double[i][1];
                mlpprocess(&net, &x, &y, _state);
                
                /*
                 * Calculate average error
                 */
                averr = averr+ae_fabs(y.ptr.p_double[0]-xy.ptr.pp_double[i][2], _state);
            }
            if( ae_fp_greater(averr/n,eps) )
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
This  function   tests   MLPTrainLM,  MLPTrainLBFGS  and   MLPTrainNetwork
functions for classification problems. It check that train  functions work
correctly  when  is used CreateC1  function.  Here  the network  tries  to
distinguish positive from negative numbers.
*************************************************************************/
static ae_bool testmlptrainunit_testmlptrainclass(ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;
    mlptrainer trainer;
    mlpreport rep;
    ae_int_t info;
    ae_matrix xy;
    sparsematrix sm;
    ae_vector x;
    ae_vector y;
    ae_int_t n;
    double vdecay;
    double traineps;
    ae_int_t nneedrest;
    ae_int_t trainits;
    double tmp;
    double mnc;
    double mxc;
    ae_int_t nxp;
    ae_int_t i;
    ae_int_t rndind;
    ae_int_t vtrain;
    ae_int_t xp;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&net, _state);
    _mlptrainer_init(&trainer, _state);
    _mlpreport_init(&rep, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&sm, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);

    mnc = (double)(10);
    mxc = (double)(11);
    nxp = 15;
    vdecay = 0.001;
    nneedrest = 10;
    traineps = 1.0E-4;
    trainits = 0;
    n = 100;
    sparsecreate(n, 2, n*2, &sm, _state);
    ae_vector_set_length(&x, 1, _state);
    ae_matrix_set_length(&xy, n, 2, _state);
    for(xp=1; xp<=nxp; xp++)
    {
        for(vtrain=0; vtrain<=3; vtrain++)
        {
            
            /*
             * Initialization:
             *  * create negative part of the set;
             */
            for(i=0; i<=n/2-1; i++)
            {
                xy.ptr.pp_double[i][0] = -1*((mxc-mnc)*ae_randomreal(_state)+mnc);
                xy.ptr.pp_double[i][1] = (double)(0);
            }
            
            /*
             *  * create positive part of the set;
             */
            for(i=n/2; i<=n-1; i++)
            {
                xy.ptr.pp_double[i][0] = (mxc-mnc)*ae_randomreal(_state)+mnc;
                xy.ptr.pp_double[i][1] = (double)(1);
            }
            
            /*
             *  * mix two parts.
             */
            for(i=0; i<=n-1; i++)
            {
                do
                {
                    rndind = ae_randominteger(n, _state);
                }
                while(rndind==i);
                tmp = xy.ptr.pp_double[i][0];
                xy.ptr.pp_double[i][0] = xy.ptr.pp_double[rndind][0];
                xy.ptr.pp_double[rndind][0] = tmp;
                tmp = xy.ptr.pp_double[i][1];
                xy.ptr.pp_double[i][1] = xy.ptr.pp_double[rndind][1];
                xy.ptr.pp_double[rndind][1] = tmp;
            }
            
            /*
             * Create and train a neural network
             */
            mlpcreatec0(1, 2, &net, _state);
            if( vtrain==0 )
            {
                mlptrainlm(&net, &xy, n, vdecay, nneedrest, &info, &rep, _state);
            }
            if( vtrain==1 )
            {
                mlptrainlbfgs(&net, &xy, n, vdecay, nneedrest, traineps, trainits, &info, &rep, _state);
            }
            
            /*
             * Train with trainer, using:
             *  * dense matrix;
             */
            if( vtrain==2 )
            {
                mlpcreatetrainercls(1, 2, &trainer, _state);
                mlpsetdataset(&trainer, &xy, n, _state);
                mlpsetdecay(&trainer, vdecay, _state);
                mlpsetcond(&trainer, traineps, trainits, _state);
                mlptrainnetwork(&trainer, &net, nneedrest, &rep, _state);
            }
            
            /*
             *  * sparse matrix.
             */
            if( vtrain==3 )
            {
                for(i=0; i<=n-1; i++)
                {
                    sparseset(&sm, i, 0, xy.ptr.pp_double[i][0], _state);
                    sparseset(&sm, i, 1, xy.ptr.pp_double[i][1], _state);
                }
                mlpcreatetrainercls(1, 2, &trainer, _state);
                mlpsetsparsedataset(&trainer, &sm, n, _state);
                mlpsetdecay(&trainer, vdecay, _state);
                mlpsetcond(&trainer, traineps, trainits, _state);
                mlptrainnetwork(&trainer, &net, nneedrest, &rep, _state);
            }
            
            /*
             * Test on training set
             */
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[0] = xy.ptr.pp_double[i][0];
                mlpprocess(&net, &x, &y, _state);
                
                /*
                 * Negative number has to be negative and
                 * positive number has to be positive.
                 */
                if( ((ae_fp_less(x.ptr.p_double[0],(double)(0))&&ae_fp_less(y.ptr.p_double[0],0.95))&&ae_fp_greater(y.ptr.p_double[1],0.05))||((ae_fp_greater_eq(x.ptr.p_double[0],(double)(0))&&ae_fp_greater(y.ptr.p_double[0],0.05))&&ae_fp_less(y.ptr.p_double[1],0.95)) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
            
            /*
             * Test on random set
             */
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[0] = ae_pow((double)(-1), (double)(ae_randominteger(2, _state)), _state)*((mxc-mnc)*ae_randomreal(_state)+mnc);
                mlpprocess(&net, &x, &y, _state);
                if( ((ae_fp_less(x.ptr.p_double[0],(double)(0))&&ae_fp_less(y.ptr.p_double[0],0.95))&&ae_fp_greater(y.ptr.p_double[1],0.05))||((ae_fp_greater_eq(x.ptr.p_double[0],(double)(0))&&ae_fp_greater(y.ptr.p_double[0],0.05))&&ae_fp_less(y.ptr.p_double[1],0.95)) )
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
This function tests   MLPTrainNetwork/MLPStartTraining/MLPContinueTraining
functions for classification problems. It check that train  functions work
correctly  when  is used CreateC1  function.  Here  the network  tries  to
distinguish positive from negative numbers.
*************************************************************************/
static ae_bool testmlptrainunit_testmlpxorcls(ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;
    mlptrainer trainer;
    mlpreport rep;
    ae_matrix xy;
    sparsematrix sm;
    ae_vector x;
    ae_vector y;
    ae_int_t n;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    double e;
    double ebest;
    double v;
    ae_vector wbest;
    double vdecay;
    double traineps;
    ae_int_t nneurons;
    ae_int_t nneedrest;
    ae_int_t trainits;
    ae_int_t nxp;
    ae_int_t i;
    ae_int_t vtrain;
    ae_int_t xp;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&net, _state);
    _mlptrainer_init(&trainer, _state);
    _mlpreport_init(&rep, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&sm, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_vector_init(&wbest, 0, DT_REAL, _state);

    nxp = 15;
    nneurons = 3;
    vdecay = 0.001;
    nneedrest = 3;
    traineps = 1.0E-4;
    trainits = 0;
    n = 4;
    sparsecreate(n, 3, n*3, &sm, _state);
    ae_vector_set_length(&x, 2, _state);
    ae_matrix_set_length(&xy, n, 3, _state);
    
    /*
     * Initialization:
     */
    xy.ptr.pp_double[0][0] = (double)(0);
    xy.ptr.pp_double[0][1] = (double)(0);
    xy.ptr.pp_double[0][2] = (double)(0);
    xy.ptr.pp_double[1][0] = (double)(0);
    xy.ptr.pp_double[1][1] = (double)(1);
    xy.ptr.pp_double[1][2] = (double)(1);
    xy.ptr.pp_double[2][0] = (double)(1);
    xy.ptr.pp_double[2][1] = (double)(0);
    xy.ptr.pp_double[2][2] = (double)(1);
    xy.ptr.pp_double[3][0] = (double)(1);
    xy.ptr.pp_double[3][1] = (double)(1);
    xy.ptr.pp_double[3][2] = (double)(0);
    
    /*
     * Create a neural network
     */
    mlpcreatec1(2, nneurons, 2, &net, _state);
    mlpproperties(&net, &nin, &nout, &wcount, _state);
    ae_vector_set_length(&wbest, wcount, _state);
    
    /*
     * Test
     */
    for(xp=1; xp<=nxp; xp++)
    {
        for(vtrain=0; vtrain<=3; vtrain++)
        {
            
            /*
             * Train with trainer, using:
             *  * dense matrix;
             */
            if( vtrain==0 )
            {
                mlpcreatetrainercls(2, 2, &trainer, _state);
                mlpsetdataset(&trainer, &xy, n, _state);
                mlpsetdecay(&trainer, vdecay, _state);
                mlpsetcond(&trainer, traineps, trainits, _state);
                mlptrainnetwork(&trainer, &net, nneedrest, &rep, _state);
            }
            if( vtrain==1 )
            {
                mlpcreatetrainercls(2, 2, &trainer, _state);
                mlpsetdataset(&trainer, &xy, n, _state);
                mlpsetdecay(&trainer, vdecay, _state);
                mlpsetcond(&trainer, traineps, trainits, _state);
                ebest = ae_maxrealnumber;
                for(i=1; i<=nneedrest; i++)
                {
                    mlpstarttraining(&trainer, &net, ae_true, _state);
                    while(mlpcontinuetraining(&trainer, &net, _state))
                    {
                    }
                    v = ae_v_dotproduct(&net.weights.ptr.p_double[0], 1, &net.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
                    e = mlperror(&net, &xy, n, _state)+0.5*vdecay*v;
                    
                    /*
                     * Compare with the best answer.
                     */
                    if( ae_fp_less(e,ebest) )
                    {
                        ae_v_move(&wbest.ptr.p_double[0], 1, &net.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
                        ebest = e;
                    }
                }
                
                /*
                 * The best result
                 */
                ae_v_move(&net.weights.ptr.p_double[0], 1, &wbest.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            }
            
            /*
             *  * sparse matrix.
             */
            if( vtrain==2 )
            {
                for(i=0; i<=n-1; i++)
                {
                    sparseset(&sm, i, 0, xy.ptr.pp_double[i][0], _state);
                    sparseset(&sm, i, 1, xy.ptr.pp_double[i][1], _state);
                    sparseset(&sm, i, 2, xy.ptr.pp_double[i][2], _state);
                }
                mlpcreatetrainercls(2, 2, &trainer, _state);
                mlpsetsparsedataset(&trainer, &sm, n, _state);
                mlpsetdecay(&trainer, vdecay, _state);
                mlpsetcond(&trainer, traineps, trainits, _state);
                mlptrainnetwork(&trainer, &net, nneedrest, &rep, _state);
            }
            if( vtrain==3 )
            {
                for(i=0; i<=n-1; i++)
                {
                    sparseset(&sm, i, 0, xy.ptr.pp_double[i][0], _state);
                    sparseset(&sm, i, 1, xy.ptr.pp_double[i][1], _state);
                    sparseset(&sm, i, 2, xy.ptr.pp_double[i][2], _state);
                }
                mlpcreatetrainercls(2, 2, &trainer, _state);
                mlpsetsparsedataset(&trainer, &sm, n, _state);
                mlpsetdecay(&trainer, vdecay, _state);
                mlpsetcond(&trainer, traineps, trainits, _state);
                ebest = ae_maxrealnumber;
                for(i=1; i<=nneedrest; i++)
                {
                    mlpstarttraining(&trainer, &net, ae_true, _state);
                    while(mlpcontinuetraining(&trainer, &net, _state))
                    {
                    }
                    v = ae_v_dotproduct(&net.weights.ptr.p_double[0], 1, &net.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
                    e = mlperror(&net, &xy, n, _state)+0.5*vdecay*v;
                    
                    /*
                     * Compare with the best answer.
                     */
                    if( ae_fp_less(e,ebest) )
                    {
                        ae_v_move(&wbest.ptr.p_double[0], 1, &net.weights.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
                        ebest = e;
                    }
                }
                
                /*
                 * The best result
                 */
                ae_v_move(&net.weights.ptr.p_double[0], 1, &wbest.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            }
            
            /*
             * Test on training set
             */
            for(i=0; i<=n-1; i++)
            {
                x.ptr.p_double[0] = xy.ptr.pp_double[i][0];
                x.ptr.p_double[1] = xy.ptr.pp_double[i][1];
                mlpprocess(&net, &x, &y, _state);
                if( ((ae_fp_eq(x.ptr.p_double[0],x.ptr.p_double[1])&&ae_fp_less(y.ptr.p_double[0],0.95))&&ae_fp_greater(y.ptr.p_double[1],0.05))||((ae_fp_neq(x.ptr.p_double[0],x.ptr.p_double[1])&&ae_fp_greater(y.ptr.p_double[0],0.05))&&ae_fp_less(y.ptr.p_double[1],0.95)) )
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
The  test  check,  that  all weights are zero after training with trainer
using empty dataset(either zero size or is't used MLPSetDataSet function).
Test  on  regression and classification problems given by dense or sparse
matrix.

NOTE: Result of the function is written in MLPTrainRegrErr variable in
      unit test.
*************************************************************************/
static ae_bool testmlptrainunit_testmlpzeroweights(ae_state *_state)
{
    ae_frame _frame_block;
    mlptrainer trainer;
    multilayerperceptron net;
    mlpreport rep;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t mxnin;
    ae_int_t mxnout;
    double vdecay;
    double traineps;
    ae_int_t trainits;
    ae_int_t nneedrest;
    ae_matrix dds;
    sparsematrix sds;
    ae_bool iscls;
    ae_bool issparse;
    ae_int_t c;
    ae_int_t n;
    ae_int_t mnn;
    ae_int_t mxn;
    ae_int_t xp;
    ae_int_t nxp;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _mlptrainer_init(&trainer, _state);
    _multilayerperceptron_init(&net, _state);
    _mlpreport_init(&rep, _state);
    ae_matrix_init(&dds, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&sds, _state);

    mxn = 20;
    mnn = 10;
    mxnin = 10;
    mxnout = 10;
    vdecay = 1.0E-3;
    nneedrest = 1;
    traineps = 1.0E-3;
    trainits = 0;
    sparsecreate(1, 1, 0, &sds, _state);
    sparseconverttocrs(&sds, _state);
    nxp = 10;
    for(xp=1; xp<=nxp; xp++)
    {
        c = ae_randominteger(2, _state);
        iscls = c==1;
        c = ae_randominteger(2, _state);
        issparse = c==1;
        
        /*
         * Create trainer and network
         */
        if( !iscls )
        {
            
            /*
             * Regression
             */
            nin = ae_randominteger(mxnin, _state)+1;
            nout = ae_randominteger(mxnout, _state)+1;
            mlpcreatetrainer(nin, nout, &trainer, _state);
            mlpcreate0(nin, nout, &net, _state);
        }
        else
        {
            
            /*
             * Classification
             */
            nin = ae_randominteger(mxnin, _state)+1;
            nout = ae_randominteger(mxnout, _state)+2;
            mlpcreatetrainercls(nin, nout, &trainer, _state);
            mlpcreatec0(nin, nout, &net, _state);
        }
        n = ae_randominteger(2, _state)-1;
        if( n==0 )
        {
            if( !issparse )
            {
                mlpsetdataset(&trainer, &dds, n, _state);
            }
            else
            {
                mlpsetsparsedataset(&trainer, &sds, n, _state);
            }
        }
        mlpsetdecay(&trainer, vdecay, _state);
        mlpsetcond(&trainer, traineps, trainits, _state);
        c = ae_randominteger(2, _state);
        if( c==0 )
        {
            mlpstarttraining(&trainer, &net, ae_true, _state);
            while(mlpcontinuetraining(&trainer, &net, _state))
            {
            }
        }
        if( c==1 )
        {
            mlptrainnetwork(&trainer, &net, nneedrest, &rep, _state);
        }
        
        /*
         * Check weights
         */
        mlpproperties(&net, &nin, &nout, &wcount, _state);
        for(c=0; c<=wcount-1; c++)
        {
            if( ae_fp_neq(net.weights.ptr.p_double[c],(double)(0)) )
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
This function tests that increasing numbers of restarts lead to statistical
improvement quality of solution.
Neural network created by Create1(10 neurons) and trained by MLPTrainLBFGS.

TEST's DISCRIPTION:

Net0   -   network trained with one restart (denoted as R1)
Net1   -   network trained with more than one restart (denoted as Rn)

We must refuse hypothesis that R1 equivalent to Rn.
Here Mean = N/2, Sigma = Sqrt(N)/2.
       _
      | 0   -   R1 worse than Rn;
 ri = |
      |_1   -   Rn same or worse then R1.
    
If Sum(ri)<Mean-5*Sigma then hypothesis is refused and test is passed.
In another case if Mean-5*Sigma<=Sum(ri)<=Mean+5*Sigma then hypothesis
is't refused and test is broken; and if Mean+5*Sigma<Sum(ri) then test
broken too hard!
*************************************************************************/
static ae_bool testmlptrainunit_testmlprestarts(ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net0;
    multilayerperceptron net1;
    mlptrainer trainer;
    mlpreport rep;
    ae_int_t info;
    sparsematrix sm;
    ae_matrix xy;
    ae_vector x;
    ae_vector y;
    ae_int_t n;
    ae_int_t nneurons;
    double vdecay;
    ae_int_t wcount0;
    ae_int_t wcount1;
    ae_int_t nin;
    ae_int_t nout;
    double avval;
    double e0;
    double e1;
    double mean;
    double numsigma;
    ae_int_t numxp;
    double traineps;
    ae_int_t nneedrest;
    ae_int_t trainits;
    ae_int_t i;
    ae_int_t vtrain;
    ae_int_t xp;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&net0, _state);
    _multilayerperceptron_init(&net1, _state);
    _mlptrainer_init(&trainer, _state);
    _mlpreport_init(&rep, _state);
    _sparsematrix_init(&sm, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);

    vdecay = 0.001;
    nneurons = 4;
    nneedrest = 3;
    traineps = 0.00;
    trainits = 2;
    n = 20;
    numxp = 400;
    ae_matrix_set_length(&xy, n, 2, _state);
    ae_vector_set_length(&x, 1, _state);
    sparsecreate(n, 2, n*2, &sm, _state);
    mean = numxp/2.0;
    numsigma = 5.0*ae_sqrt((double)(numxp), _state)/2.0;
    for(vtrain=0; vtrain<=2; vtrain++)
    {
        avval = (double)(0);
        for(xp=1; xp<=numxp; xp++)
        {
            
            /*
             * Create a train set
             */
            for(i=0; i<=n-1; i++)
            {
                xy.ptr.pp_double[i][0] = 2*ae_randomreal(_state)-1;
                xy.ptr.pp_double[i][1] = 2*ae_randomreal(_state)-1;
            }
            
            /*
             * Create and train a neural network
             */
            mlpcreate1(1, nneurons, 1, &net0, _state);
            mlpcreate1(1, nneurons, 1, &net1, _state);
            if( vtrain==0 )
            {
                mlptrainlbfgs(&net0, &xy, n, vdecay, 1, traineps, trainits, &info, &rep, _state);
                mlptrainlbfgs(&net1, &xy, n, vdecay, nneedrest, traineps, trainits, &info, &rep, _state);
            }
            if( vtrain==1 )
            {
                mlpcreatetrainer(1, 1, &trainer, _state);
                mlpsetdataset(&trainer, &xy, n, _state);
                mlpsetdecay(&trainer, vdecay, _state);
                mlpsetcond(&trainer, traineps, trainits, _state);
                mlptrainnetwork(&trainer, &net0, 1, &rep, _state);
                mlptrainnetwork(&trainer, &net1, nneedrest, &rep, _state);
            }
            if( vtrain==2 )
            {
                for(i=0; i<=n-1; i++)
                {
                    sparseset(&sm, i, 0, xy.ptr.pp_double[i][0], _state);
                    sparseset(&sm, i, 1, xy.ptr.pp_double[i][1], _state);
                }
                mlpcreatetrainer(1, 1, &trainer, _state);
                mlpsetsparsedataset(&trainer, &sm, n, _state);
                mlpsetdecay(&trainer, vdecay, _state);
                mlpsetcond(&trainer, traineps, trainits, _state);
                mlptrainnetwork(&trainer, &net0, 1, &rep, _state);
                mlptrainnetwork(&trainer, &net1, nneedrest, &rep, _state);
            }
            
            /*
             * Calculate errors for...
             *
             * ...for Net0, trained with 1 restart.
             */
            mlpproperties(&net0, &nin, &nout, &wcount0, _state);
            e0 = ae_v_dotproduct(&net0.weights.ptr.p_double[0], 1, &net0.weights.ptr.p_double[0], 1, ae_v_len(0,wcount0-1));
            e0 = mlperrorn(&net0, &xy, n, _state)+0.5*vdecay*e0;
            
            /*
             * ...for Net1, trained with NNeedRest>1 restarts.
             */
            mlpproperties(&net1, &nin, &nout, &wcount1, _state);
            e1 = ae_v_dotproduct(&net1.weights.ptr.p_double[0], 1, &net1.weights.ptr.p_double[0], 1, ae_v_len(0,wcount1-1));
            e1 = mlperrorn(&net1, &xy, n, _state)+0.5*vdecay*e1;
            if( ae_fp_less_eq(e0,e1) )
            {
                avval = avval+1;
            }
        }
        if( ae_fp_less(mean-numsigma,avval) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
The function test function MLPKFoldCV.
*************************************************************************/
static ae_bool testmlptrainunit_testmlpcverror(ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;
    mlptrainer trainer;
    mlpreport rep;
    mlpreport cvrep;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t nneurons;
    ae_int_t rowsz;
    double decay;
    double wstep;
    ae_int_t maxits;
    ae_int_t foldscount;
    ae_int_t nneedrest;
    sparsematrix sptrainingset;
    ae_matrix trainingset;
    ae_matrix testset;
    ae_int_t npoints;
    ae_int_t ntstpoints;
    double mean;
    double numsigma;
    double diffms;
    double tstrelclserror;
    double tstavgce;
    double tstrmserror;
    double tstavgerror;
    double tstavgrelerror;
    ae_int_t r0;
    ae_int_t r1;
    ae_int_t r2;
    ae_int_t r3;
    ae_int_t r4;
    ae_int_t ntest;
    ae_int_t xp;
    ae_int_t nxp;
    ae_bool isregr;
    ae_int_t issparse;
    ae_int_t i;
    ae_int_t j;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_init(&net, _state);
    _mlptrainer_init(&trainer, _state);
    _mlpreport_init(&rep, _state);
    _mlpreport_init(&cvrep, _state);
    _sparsematrix_init(&sptrainingset, _state);
    ae_matrix_init(&trainingset, 0, 0, DT_REAL, _state);
    ae_matrix_init(&testset, 0, 0, DT_REAL, _state);

    decay = 1.0E-6;
    wstep = 0.0;
    foldscount = 5;
    nneedrest = 1;
    ntest = ae_randominteger(4, _state);
    nxp = 1000;
    maxits = 50;
    nin = 1;
    nout = 1;
    npoints = 5;
    ntstpoints = 100;
    isregr = ae_true;
    nneurons = 3;
    if( ntest==1 )
    {
        nxp = 1000;
        maxits = 50;
        nin = 1;
        nout = 10;
        npoints = 5;
        ntstpoints = 100;
        isregr = ae_true;
    }
    if( ntest==2 )
    {
        nxp = 1000;
        maxits = 50;
        nin = 10;
        nout = 1;
        npoints = 20;
        ntstpoints = 100;
        isregr = ae_true;
    }
    if( ntest==3 )
    {
        nxp = 2000;
        maxits = 10;
        nin = 1;
        nneurons = 3;
        nout = 3;
        npoints = 10;
        ntstpoints = 100;
        isregr = ae_false;
    }
    mean = nxp/2.0;
    numsigma = 5.0*ae_sqrt((double)(nxp), _state)/2.0;
    diffms = mean-numsigma;
    issparse = ae_randominteger(2, _state);
    if( isregr )
    {
        mlpcreate0(nin, nout, &net, _state);
        mlpcreatetrainer(nin, nout, &trainer, _state);
    }
    else
    {
        mlpcreatec1(nin, nneurons, nout, &net, _state);
        mlpcreatetrainercls(nin, nout, &trainer, _state);
    }
    mlpsetcond(&trainer, wstep, maxits, _state);
    mlpsetdecay(&trainer, decay, _state);
    if( isregr )
    {
        rowsz = nin+nout;
    }
    else
    {
        rowsz = nin+1;
    }
    r0 = 0;
    r1 = 0;
    r2 = 0;
    r3 = 0;
    r4 = 0;
    for(xp=1; xp<=nxp; xp++)
    {
        
        /*
         * Dense matrix
         */
        if( issparse==0 )
        {
            rmatrixsetlengthatleast(&trainingset, npoints, rowsz, _state);
            
            /*
             * Create training set
             */
            for(i=0; i<=npoints-1; i++)
            {
                for(j=0; j<=nin-1; j++)
                {
                    trainingset.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                }
            }
            if( isregr )
            {
                for(i=0; i<=npoints-1; i++)
                {
                    for(j=nin; j<=rowsz-1; j++)
                    {
                        trainingset.ptr.pp_double[i][j] = 2*ae_randomreal(_state)+1;
                    }
                }
            }
            else
            {
                for(i=0; i<=npoints-1; i++)
                {
                    for(j=nin; j<=rowsz-1; j++)
                    {
                        trainingset.ptr.pp_double[i][j] = (double)(ae_randominteger(nout, _state));
                    }
                }
            }
            mlpsetdataset(&trainer, &trainingset, npoints, _state);
        }
        
        /*
         * Sparse matrix
         */
        if( issparse==1 )
        {
            sparsecreate(npoints, rowsz, npoints*rowsz, &sptrainingset, _state);
            
            /*
             * Create training set
             */
            for(i=0; i<=npoints-1; i++)
            {
                for(j=0; j<=nin-1; j++)
                {
                    sparseset(&sptrainingset, i, j, 2*ae_randomreal(_state)-1, _state);
                }
            }
            if( isregr )
            {
                for(i=0; i<=npoints-1; i++)
                {
                    for(j=nin; j<=rowsz-1; j++)
                    {
                        sparseset(&sptrainingset, i, j, 2*ae_randomreal(_state)+1, _state);
                    }
                }
            }
            else
            {
                for(i=0; i<=npoints-1; i++)
                {
                    for(j=nin; j<=rowsz-1; j++)
                    {
                        sparseset(&sptrainingset, i, j, (double)(ae_randominteger(nout, _state)), _state);
                    }
                }
            }
            sparseconverttocrs(&sptrainingset, _state);
            mlpsetsparsedataset(&trainer, &sptrainingset, npoints, _state);
        }
        rmatrixsetlengthatleast(&testset, ntstpoints, rowsz, _state);
        
        /*
         * Create test set
         */
        for(i=0; i<=ntstpoints-1; i++)
        {
            for(j=0; j<=nin-1; j++)
            {
                testset.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
            }
        }
        if( isregr )
        {
            for(i=0; i<=ntstpoints-1; i++)
            {
                for(j=nin; j<=rowsz-1; j++)
                {
                    testset.ptr.pp_double[i][j] = 2*ae_randomreal(_state)+1;
                }
            }
        }
        else
        {
            for(i=0; i<=ntstpoints-1; i++)
            {
                for(j=nin; j<=rowsz-1; j++)
                {
                    testset.ptr.pp_double[i][j] = (double)(ae_randominteger(nout, _state));
                }
            }
        }
        mlptrainnetwork(&trainer, &net, nneedrest, &rep, _state);
        tstrelclserror = (double)(0);
        tstavgce = (double)(0);
        tstrmserror = (double)(0);
        tstavgerror = (double)(0);
        tstavgrelerror = (double)(0);
        if( !isregr )
        {
            tstrelclserror = mlprelclserror(&net, &testset, ntstpoints, _state);
            tstavgce = mlpavgce(&net, &testset, ntstpoints, _state);
        }
        tstrmserror = mlprmserror(&net, &testset, ntstpoints, _state);
        tstavgerror = mlpavgerror(&net, &testset, ntstpoints, _state);
        tstavgrelerror = mlpavgrelerror(&net, &testset, ntstpoints, _state);
        
        /*
         * Cross-validation
         */
        mlpkfoldcv(&trainer, &net, nneedrest, foldscount, &cvrep, _state);
        if( !isregr )
        {
            if( ae_fp_less(ae_fabs(tstrelclserror-rep.relclserror, _state),ae_fabs(tstrelclserror-cvrep.relclserror, _state)) )
            {
                r0 = r0+1;
            }
            if( ae_fp_less(ae_fabs(tstavgce-rep.avgce, _state),ae_fabs(tstavgce-cvrep.avgce, _state)) )
            {
                r1 = r1+1;
            }
        }
        if( ae_fp_less(ae_fabs(tstrmserror-rep.rmserror, _state),ae_fabs(tstrmserror-cvrep.rmserror, _state)) )
        {
            r2 = r2+1;
        }
        if( ae_fp_less(ae_fabs(tstavgerror-rep.avgerror, _state),ae_fabs(tstavgerror-cvrep.avgerror, _state)) )
        {
            r3 = r3+1;
        }
        if( ae_fp_less(ae_fabs(tstavgrelerror-rep.avgrelerror, _state),ae_fabs(tstavgrelerror-cvrep.avgrelerror, _state)) )
        {
            r4 = r4+1;
        }
    }
    if( !isregr )
    {
        if( ae_fp_less_eq(diffms,(double)(r0))||ae_fp_less_eq(diffms,(double)(r1)) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    if( (ae_fp_less_eq(diffms,(double)(r2))||ae_fp_less_eq(diffms,(double)(r3)))||ae_fp_less_eq(diffms,(double)(r4)) )
    {
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Test FoldCV when  no dataset  was specified with
     * MLPSetDataset/SetSparseDataset(), or subset with
     * only one point  was  given.
     *
     * NPoints values:
     *  * -1 - don't set dataset with using MLPSetDataset..;
     *  *  0 - zero dataset;
     *  *  1 - dataset with one point.
     */
    for(npoints=-1; npoints<=1; npoints++)
    {
        if( isregr )
        {
            mlpcreatetrainer(nin, nout, &trainer, _state);
        }
        else
        {
            mlpcreatetrainercls(nin, nout, &trainer, _state);
        }
        if( npoints>-1 )
        {
            if( issparse==0 )
            {
                mlpsetdataset(&trainer, &trainingset, npoints, _state);
            }
            if( issparse==1 )
            {
                mlpsetsparsedataset(&trainer, &sptrainingset, npoints, _state);
            }
        }
        mlpkfoldcv(&trainer, &net, nneedrest, foldscount, &cvrep, _state);
        if( ((((((ae_fp_neq(cvrep.relclserror,(double)(0))||ae_fp_neq(cvrep.avgce,(double)(0)))||ae_fp_neq(cvrep.rmserror,(double)(0)))||ae_fp_neq(cvrep.avgerror,(double)(0)))||ae_fp_neq(cvrep.avgrelerror,(double)(0)))||cvrep.ngrad!=0)||cvrep.nhess!=0)||cvrep.ncholesky!=0 )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
The  function  tests  functions  for  training  ensembles:  MLPEBaggingLM,
MLPEBaggingLBFGS.
*************************************************************************/
static ae_bool testmlptrainunit_testmlptrainens(ae_state *_state)
{
    ae_frame _frame_block;
    mlpensemble ensemble;
    mlpreport rep;
    mlpcvreport oobrep;
    ae_int_t info;
    ae_matrix xy;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t npoints;
    ae_int_t nhid;
    ae_int_t algtype;
    ae_int_t tasktype;
    ae_int_t pass;
    double e;
    ae_int_t nless;
    ae_int_t nall;
    ae_int_t nclasses;
    ae_int_t i;
    ae_int_t j;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _mlpensemble_init(&ensemble, _state);
    _mlpreport_init(&rep, _state);
    _mlpcvreport_init(&oobrep, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);

    result = ae_false;
    
    /*
     * network training must reduce error
     * test on random regression task
     */
    nin = 3;
    nout = 2;
    nhid = 5;
    npoints = 100;
    nless = 0;
    nall = 0;
    for(pass=1; pass<=10; pass++)
    {
        for(algtype=0; algtype<=1; algtype++)
        {
            for(tasktype=0; tasktype<=1; tasktype++)
            {
                if( tasktype==0 )
                {
                    ae_matrix_set_length(&xy, npoints, nin+nout, _state);
                    for(i=0; i<=npoints-1; i++)
                    {
                        for(j=0; j<=nin+nout-1; j++)
                        {
                            xy.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                        }
                    }
                    mlpecreate1(nin, nhid, nout, 1+ae_randominteger(3, _state), &ensemble, _state);
                }
                else
                {
                    ae_matrix_set_length(&xy, npoints, nin+1, _state);
                    nclasses = 2+ae_randominteger(2, _state);
                    for(i=0; i<=npoints-1; i++)
                    {
                        for(j=0; j<=nin-1; j++)
                        {
                            xy.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                        }
                        xy.ptr.pp_double[i][nin] = (double)(ae_randominteger(nclasses, _state));
                    }
                    mlpecreatec1(nin, nhid, nclasses, 1+ae_randominteger(3, _state), &ensemble, _state);
                }
                e = mlpermserror(&ensemble, &xy, npoints, _state);
                if( algtype==0 )
                {
                    mlpebagginglm(&ensemble, &xy, npoints, 0.001, 1, &info, &rep, &oobrep, _state);
                }
                else
                {
                    mlpebagginglbfgs(&ensemble, &xy, npoints, 0.001, 1, 0.01, 0, &info, &rep, &oobrep, _state);
                }
                if( info<0 )
                {
                    result = ae_true;
                }
                else
                {
                    if( ae_fp_less(mlpermserror(&ensemble, &xy, npoints, _state),e) )
                    {
                        nless = nless+1;
                    }
                }
                nall = nall+1;
            }
        }
    }
    result = result||ae_fp_greater((double)(nall-nless),0.3*nall);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Testing  for  functions  MLPETrainES and MLPTrainEnsembleES  on regression
problems. Returns TRUE for errors, FALSE for success.
*************************************************************************/
static ae_bool testmlptrainunit_testmlptrainensregr(ae_state *_state)
{
    ae_frame _frame_block;
    mlptrainer trainer;
    mlpensemble netens;
    mlpreport rep;
    modelerrors repx;
    ae_int_t info;
    sparsematrix xytrainsp;
    ae_matrix xytrain;
    ae_matrix xytest;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t nneurons;
    ae_vector x;
    ae_vector y;
    double decay;
    double wstep;
    ae_int_t maxits;
    ae_int_t nneedrest;
    ae_int_t enssize;
    double mnval;
    double mxval;
    ae_int_t ntrain;
    ae_int_t ntest;
    double avgerr;
    ae_int_t issparse;
    ae_int_t withtrainer;
    double eps;
    ae_int_t xp;
    ae_int_t i;
    ae_int_t j;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _mlptrainer_init(&trainer, _state);
    _mlpensemble_init(&netens, _state);
    _mlpreport_init(&rep, _state);
    _modelerrors_init(&repx, _state);
    _sparsematrix_init(&xytrainsp, _state);
    ae_matrix_init(&xytrain, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xytest, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);

    result = ae_false;
    
    /*
     * This test checks ability to train ensemble on simple regression
     * problem "f(x0,x1,x2,...) = x0 + x1 + x2 + ...".
     */
    eps = 5.0E-2;
    mnval = (double)(-1);
    mxval = (double)(1);
    ntrain = 40;
    ntest = 20;
    decay = 1.0E-3;
    wstep = 1.0E-3;
    maxits = 20;
    nneedrest = 1;
    nneurons = 20;
    nout = 1;
    enssize = 100;
    for(xp=1; xp<=2; xp++)
    {
        nin = ae_randominteger(3, _state)+1;
        rvectorsetlengthatleast(&x, nin, _state);
        mlpcreatetrainer(nin, nout, &trainer, _state);
        mlpsetdecay(&trainer, decay, _state);
        mlpsetcond(&trainer, wstep, maxits, _state);
        rmatrixsetlengthatleast(&xytrain, ntrain, nin+nout, _state);
        rmatrixsetlengthatleast(&xytest, ntest, nin+nout, _state);
        withtrainer = ae_randominteger(2, _state);
        issparse = 0;
        if( withtrainer==0 )
        {
            issparse = 0;
        }
        if( withtrainer==1 )
        {
            issparse = ae_randominteger(2, _state);
        }
        
        /*
         * Training set
         */
        for(i=0; i<=ntrain-1; i++)
        {
            for(j=0; j<=nin-1; j++)
            {
                xytrain.ptr.pp_double[i][j] = (mxval-mnval)*ae_randomreal(_state)+mnval;
            }
            xytrain.ptr.pp_double[i][nin] = (double)(0);
            for(j=0; j<=nin-1; j++)
            {
                xytrain.ptr.pp_double[i][nin] = xytrain.ptr.pp_double[i][nin]+xytrain.ptr.pp_double[i][j];
            }
        }
        if( withtrainer==1 )
        {
            
            /*
             * Dense matrix
             */
            if( issparse==0 )
            {
                mlpsetdataset(&trainer, &xytrain, ntrain, _state);
            }
            
            /*
             * Sparse matrix
             */
            if( issparse==1 )
            {
                sparsecreate(ntrain, nin+nout, ntrain*(nin+nout), &xytrainsp, _state);
                
                /*
                 * Just copy dense matrix to sparse matrix(using SparseGet() is too expensive).
                 */
                for(i=0; i<=ntrain-1; i++)
                {
                    for(j=0; j<=nin+nout-1; j++)
                    {
                        sparseset(&xytrainsp, i, j, xytrain.ptr.pp_double[i][j], _state);
                    }
                }
                sparseconverttocrs(&xytrainsp, _state);
                mlpsetsparsedataset(&trainer, &xytrainsp, ntrain, _state);
            }
        }
        
        /*
         * Test set
         */
        for(i=0; i<=ntest-1; i++)
        {
            for(j=0; j<=nin-1; j++)
            {
                xytest.ptr.pp_double[i][j] = (mxval-mnval)*ae_randomreal(_state)+mnval;
            }
            xytest.ptr.pp_double[i][nin] = (double)(0);
            for(j=0; j<=nin-1; j++)
            {
                xytest.ptr.pp_double[i][nin] = xytest.ptr.pp_double[i][nin]+xytest.ptr.pp_double[i][j];
            }
        }
        
        /*
         * Create ensemble
         */
        mlpecreate1(nin, nneurons, nout, enssize, &netens, _state);
        
        /*
         * Train ensembles:
         *  * without trainer;
         */
        if( withtrainer==0 )
        {
            mlpetraines(&netens, &xytrain, ntrain, decay, nneedrest, &info, &rep, _state);
        }
        
        /*
         *  * with trainer.
         */
        if( withtrainer==1 )
        {
            mlptrainensemblees(&trainer, &netens, nneedrest, &rep, _state);
        }
        
        /*
         * Test that Rep contains correct error values
         */
        mlpeallerrorsx(&netens, &xytrain, &xytrainsp, ntrain, 0, &netens.network.dummyidx, 0, ntrain, 0, &netens.network.buf, &repx, _state);
        seterrorflagdiff(&result, rep.relclserror, repx.relclserror, 1.0E-4, 1.0E-2, _state);
        seterrorflagdiff(&result, rep.avgce, repx.avgce, 1.0E-4, 1.0E-2, _state);
        seterrorflagdiff(&result, rep.rmserror, repx.rmserror, 1.0E-4, 1.0E-2, _state);
        seterrorflagdiff(&result, rep.avgerror, repx.avgerror, 1.0E-4, 1.0E-2, _state);
        seterrorflagdiff(&result, rep.avgrelerror, repx.avgrelerror, 1.0E-4, 1.0E-2, _state);
        
        /*
         * Test that network fits data well. Calculate average error:
         *  * on training dataset;
         *  * on test dataset. (here we reduce the accuracy
         *    requirements - average error is compared with 2*Eps).
         */
        avgerr = (double)(0);
        for(i=0; i<=ntrain-1; i++)
        {
            if( issparse==0 )
            {
                ae_v_move(&x.ptr.p_double[0], 1, &xytrain.ptr.pp_double[i][0], 1, ae_v_len(0,nin-1));
            }
            if( issparse==1 )
            {
                sparsegetrow(&xytrainsp, i, &x, _state);
            }
            mlpeprocess(&netens, &x, &y, _state);
            avgerr = avgerr+ae_fabs(y.ptr.p_double[0]-xytrain.ptr.pp_double[i][nin], _state);
        }
        avgerr = avgerr/ntrain;
        seterrorflag(&result, ae_fp_greater(avgerr,eps), _state);
        avgerr = (double)(0);
        for(i=0; i<=ntest-1; i++)
        {
            ae_v_move(&x.ptr.p_double[0], 1, &xytest.ptr.pp_double[i][0], 1, ae_v_len(0,nin-1));
            mlpeprocess(&netens, &x, &y, _state);
            avgerr = avgerr+ae_fabs(y.ptr.p_double[0]-xytest.ptr.pp_double[i][nin], _state);
        }
        avgerr = avgerr/ntest;
        seterrorflag(&result, ae_fp_greater(avgerr,2*eps), _state);
    }
    
    /*
     * Catch bug in implementation of MLPTrainEnsembleX:
     * test ensemble training on empty dataset.
     *
     * Unfixed version should crash with violation of array
     * bounds (at least in C#).
     */
    nin = 2;
    nout = 2;
    nneurons = 3;
    enssize = 3;
    nneedrest = 2;
    wstep = 0.001;
    maxits = 2;
    mlpcreatetrainer(nin, nout, &trainer, _state);
    mlpsetcond(&trainer, wstep, maxits, _state);
    mlpecreate1(nin, nneurons, nout, enssize, &netens, _state);
    mlptrainensemblees(&trainer, &netens, nneedrest, &rep, _state);
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Testing for functions MLPETrainES and MLPTrainEnsembleES on classification
problems.
*************************************************************************/
static ae_bool testmlptrainunit_testmlptrainenscls(ae_state *_state)
{
    ae_frame _frame_block;
    mlptrainer trainer;
    mlpensemble netens;
    mlpreport rep;
    ae_int_t info;
    sparsematrix xytrainsp;
    ae_matrix xytrain;
    ae_matrix xytest;
    ae_int_t nin;
    ae_int_t nout;
    ae_vector x;
    ae_vector y;
    double decay;
    double wstep;
    ae_int_t maxits;
    ae_int_t nneedrest;
    ae_int_t enssize;
    ae_int_t val;
    ae_int_t ntrain;
    ae_int_t ntest;
    double avgerr;
    double eps;
    double delta;
    ae_int_t issparse;
    ae_int_t withtrainer;
    ae_int_t xp;
    ae_int_t nxp;
    ae_int_t i;
    ae_int_t j;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _mlptrainer_init(&trainer, _state);
    _mlpensemble_init(&netens, _state);
    _mlpreport_init(&rep, _state);
    _sparsematrix_init(&xytrainsp, _state);
    ae_matrix_init(&xytrain, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xytest, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);

    eps = 5.0E-2;
    delta = 0.1;
    ntrain = 90;
    ntest = 90;
    nin = 3;
    nout = 3;
    rvectorsetlengthatleast(&x, nin, _state);
    rmatrixsetlengthatleast(&xytrain, ntrain, nin+1, _state);
    rmatrixsetlengthatleast(&xytest, ntest, nin+1, _state);
    decay = 1.0E-3;
    wstep = 1.0E-3;
    maxits = 100;
    nneedrest = 1;
    mlpcreatetrainercls(nin, nout, &trainer, _state);
    mlpsetdecay(&trainer, decay, _state);
    mlpsetcond(&trainer, wstep, maxits, _state);
    nxp = 5;
    for(xp=1; xp<=nxp; xp++)
    {
        enssize = ae_round(ae_pow((double)(10), (double)(ae_randominteger(2, _state)+1), _state), _state);
        withtrainer = ae_randominteger(2, _state);
        issparse = 0;
        if( withtrainer==0 )
        {
            issparse = 0;
        }
        if( withtrainer==1 )
        {
            issparse = ae_randominteger(2, _state);
        }
        for(i=0; i<=ntrain-1; i++)
        {
            val = i%nin;
            for(j=0; j<=nin-1; j++)
            {
                xytrain.ptr.pp_double[i][j] = delta*(ae_randomreal(_state)-1);
            }
            xytrain.ptr.pp_double[i][val] = xytrain.ptr.pp_double[i][val]+1;
            xytrain.ptr.pp_double[i][nin] = (double)(val);
        }
        
        /*
         * Set dense dataset in trainer
         */
        if( issparse==0 )
        {
            mlpsetdataset(&trainer, &xytrain, ntrain, _state);
        }
        
        /*
         *  * Sparse dataset(create it with using dense dataset).
         */
        if( issparse==1 )
        {
            sparsecreate(ntrain, nin+1, ntrain*(nin+1), &xytrainsp, _state);
            for(i=0; i<=ntrain-1; i++)
            {
                for(j=0; j<=nin-1; j++)
                {
                    sparseset(&xytrainsp, i, j, xytrain.ptr.pp_double[i][j], _state);
                }
                sparseset(&xytrainsp, i, nin, xytrain.ptr.pp_double[i][nin], _state);
            }
            sparseconverttocrs(&xytrainsp, _state);
            
            /*
             * Set sparse dataset in trainer
             */
            mlpsetsparsedataset(&trainer, &xytrainsp, ntrain, _state);
        }
        
        /*
         * Create test set
         */
        for(i=0; i<=ntest-1; i++)
        {
            val = ae_randominteger(nin, _state);
            for(j=0; j<=nin-1; j++)
            {
                xytest.ptr.pp_double[i][j] = delta*(ae_randomreal(_state)-1);
            }
            xytest.ptr.pp_double[i][val] = xytest.ptr.pp_double[i][val]+1;
            xytest.ptr.pp_double[i][nin] = (double)(val);
        }
        
        /*
         * Create ensemble
         */
        mlpecreatec0(nin, nout, enssize, &netens, _state);
        
        /*
         * Train ensembles:
         *  * without trainer;
         */
        if( withtrainer==0 )
        {
            mlpetraines(&netens, &xytrain, ntrain, decay, nneedrest, &info, &rep, _state);
        }
        
        /*
         *  * with trainer.
         */
        if( withtrainer==1 )
        {
            mlptrainensemblees(&trainer, &netens, nneedrest, &rep, _state);
        }
        
        /*
         * Calculate average error:
         *  * on training dataset;
         */
        avgerr = (double)(0);
        for(i=0; i<=ntrain-1; i++)
        {
            if( issparse==0 )
            {
                ae_v_move(&x.ptr.p_double[0], 1, &xytrain.ptr.pp_double[i][0], 1, ae_v_len(0,nin-1));
            }
            if( issparse==1 )
            {
                sparsegetrow(&xytrainsp, i, &x, _state);
            }
            mlpeprocess(&netens, &x, &y, _state);
            for(j=0; j<=nout-1; j++)
            {
                if( ae_fp_neq((double)(j),xytrain.ptr.pp_double[i][nin]) )
                {
                    avgerr = avgerr+y.ptr.p_double[j];
                }
                else
                {
                    avgerr = avgerr+(1-y.ptr.p_double[j]);
                }
            }
        }
        avgerr = avgerr/(ntrain*nout);
        if( ae_fp_greater(avgerr,eps) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         *  * on test dataset.
         */
        avgerr = (double)(0);
        for(i=0; i<=ntest-1; i++)
        {
            ae_v_move(&x.ptr.p_double[0], 1, &xytest.ptr.pp_double[i][0], 1, ae_v_len(0,nin-1));
            mlpeprocess(&netens, &x, &y, _state);
            for(j=0; j<=nout-1; j++)
            {
                if( ae_fp_neq((double)(j),xytest.ptr.pp_double[i][nin]) )
                {
                    avgerr = avgerr+y.ptr.p_double[j];
                }
                else
                {
                    avgerr = avgerr+(1-y.ptr.p_double[j]);
                }
            }
        }
        avgerr = avgerr/(ntest*nout);
        if( ae_fp_greater(avgerr,eps) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*$ End $*/
