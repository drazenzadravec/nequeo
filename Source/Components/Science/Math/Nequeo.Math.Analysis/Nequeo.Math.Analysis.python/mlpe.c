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
#include "mlpe.h"


/*$ Declarations $*/
static ae_int_t mlpe_mlpefirstversion = 1;


/*$ Body $*/


/*************************************************************************
Like MLPCreate0, but for ensembles.

  -- ALGLIB --
     Copyright 18.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpecreate0(ae_int_t nin,
     ae_int_t nout,
     ae_int_t ensemblesize,
     mlpensemble* ensemble,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;

    ae_frame_make(_state, &_frame_block);
    _mlpensemble_clear(ensemble);
    _multilayerperceptron_init(&net, _state);

    mlpcreate0(nin, nout, &net, _state);
    mlpecreatefromnetwork(&net, ensemblesize, ensemble, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Like MLPCreate1, but for ensembles.

  -- ALGLIB --
     Copyright 18.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpecreate1(ae_int_t nin,
     ae_int_t nhid,
     ae_int_t nout,
     ae_int_t ensemblesize,
     mlpensemble* ensemble,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;

    ae_frame_make(_state, &_frame_block);
    _mlpensemble_clear(ensemble);
    _multilayerperceptron_init(&net, _state);

    mlpcreate1(nin, nhid, nout, &net, _state);
    mlpecreatefromnetwork(&net, ensemblesize, ensemble, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Like MLPCreate2, but for ensembles.

  -- ALGLIB --
     Copyright 18.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpecreate2(ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t ensemblesize,
     mlpensemble* ensemble,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;

    ae_frame_make(_state, &_frame_block);
    _mlpensemble_clear(ensemble);
    _multilayerperceptron_init(&net, _state);

    mlpcreate2(nin, nhid1, nhid2, nout, &net, _state);
    mlpecreatefromnetwork(&net, ensemblesize, ensemble, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Like MLPCreateB0, but for ensembles.

  -- ALGLIB --
     Copyright 18.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpecreateb0(ae_int_t nin,
     ae_int_t nout,
     double b,
     double d,
     ae_int_t ensemblesize,
     mlpensemble* ensemble,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;

    ae_frame_make(_state, &_frame_block);
    _mlpensemble_clear(ensemble);
    _multilayerperceptron_init(&net, _state);

    mlpcreateb0(nin, nout, b, d, &net, _state);
    mlpecreatefromnetwork(&net, ensemblesize, ensemble, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Like MLPCreateB1, but for ensembles.

  -- ALGLIB --
     Copyright 18.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpecreateb1(ae_int_t nin,
     ae_int_t nhid,
     ae_int_t nout,
     double b,
     double d,
     ae_int_t ensemblesize,
     mlpensemble* ensemble,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;

    ae_frame_make(_state, &_frame_block);
    _mlpensemble_clear(ensemble);
    _multilayerperceptron_init(&net, _state);

    mlpcreateb1(nin, nhid, nout, b, d, &net, _state);
    mlpecreatefromnetwork(&net, ensemblesize, ensemble, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Like MLPCreateB2, but for ensembles.

  -- ALGLIB --
     Copyright 18.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpecreateb2(ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     double b,
     double d,
     ae_int_t ensemblesize,
     mlpensemble* ensemble,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;

    ae_frame_make(_state, &_frame_block);
    _mlpensemble_clear(ensemble);
    _multilayerperceptron_init(&net, _state);

    mlpcreateb2(nin, nhid1, nhid2, nout, b, d, &net, _state);
    mlpecreatefromnetwork(&net, ensemblesize, ensemble, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Like MLPCreateR0, but for ensembles.

  -- ALGLIB --
     Copyright 18.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpecreater0(ae_int_t nin,
     ae_int_t nout,
     double a,
     double b,
     ae_int_t ensemblesize,
     mlpensemble* ensemble,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;

    ae_frame_make(_state, &_frame_block);
    _mlpensemble_clear(ensemble);
    _multilayerperceptron_init(&net, _state);

    mlpcreater0(nin, nout, a, b, &net, _state);
    mlpecreatefromnetwork(&net, ensemblesize, ensemble, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Like MLPCreateR1, but for ensembles.

  -- ALGLIB --
     Copyright 18.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpecreater1(ae_int_t nin,
     ae_int_t nhid,
     ae_int_t nout,
     double a,
     double b,
     ae_int_t ensemblesize,
     mlpensemble* ensemble,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;

    ae_frame_make(_state, &_frame_block);
    _mlpensemble_clear(ensemble);
    _multilayerperceptron_init(&net, _state);

    mlpcreater1(nin, nhid, nout, a, b, &net, _state);
    mlpecreatefromnetwork(&net, ensemblesize, ensemble, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Like MLPCreateR2, but for ensembles.

  -- ALGLIB --
     Copyright 18.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpecreater2(ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     double a,
     double b,
     ae_int_t ensemblesize,
     mlpensemble* ensemble,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;

    ae_frame_make(_state, &_frame_block);
    _mlpensemble_clear(ensemble);
    _multilayerperceptron_init(&net, _state);

    mlpcreater2(nin, nhid1, nhid2, nout, a, b, &net, _state);
    mlpecreatefromnetwork(&net, ensemblesize, ensemble, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Like MLPCreateC0, but for ensembles.

  -- ALGLIB --
     Copyright 18.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpecreatec0(ae_int_t nin,
     ae_int_t nout,
     ae_int_t ensemblesize,
     mlpensemble* ensemble,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;

    ae_frame_make(_state, &_frame_block);
    _mlpensemble_clear(ensemble);
    _multilayerperceptron_init(&net, _state);

    mlpcreatec0(nin, nout, &net, _state);
    mlpecreatefromnetwork(&net, ensemblesize, ensemble, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Like MLPCreateC1, but for ensembles.

  -- ALGLIB --
     Copyright 18.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpecreatec1(ae_int_t nin,
     ae_int_t nhid,
     ae_int_t nout,
     ae_int_t ensemblesize,
     mlpensemble* ensemble,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;

    ae_frame_make(_state, &_frame_block);
    _mlpensemble_clear(ensemble);
    _multilayerperceptron_init(&net, _state);

    mlpcreatec1(nin, nhid, nout, &net, _state);
    mlpecreatefromnetwork(&net, ensemblesize, ensemble, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Like MLPCreateC2, but for ensembles.

  -- ALGLIB --
     Copyright 18.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpecreatec2(ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_int_t ensemblesize,
     mlpensemble* ensemble,
     ae_state *_state)
{
    ae_frame _frame_block;
    multilayerperceptron net;

    ae_frame_make(_state, &_frame_block);
    _mlpensemble_clear(ensemble);
    _multilayerperceptron_init(&net, _state);

    mlpcreatec2(nin, nhid1, nhid2, nout, &net, _state);
    mlpecreatefromnetwork(&net, ensemblesize, ensemble, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Creates ensemble from network. Only network geometry is copied.

  -- ALGLIB --
     Copyright 17.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpecreatefromnetwork(multilayerperceptron* network,
     ae_int_t ensemblesize,
     mlpensemble* ensemble,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t ccount;
    ae_int_t wcount;

    _mlpensemble_clear(ensemble);

    ae_assert(ensemblesize>0, "MLPECreate: incorrect ensemble size!", _state);
    
    /*
     * Copy network
     */
    mlpcopy(network, &ensemble->network, _state);
    
    /*
     * network properties
     */
    if( mlpissoftmax(network, _state) )
    {
        ccount = mlpgetinputscount(&ensemble->network, _state);
    }
    else
    {
        ccount = mlpgetinputscount(&ensemble->network, _state)+mlpgetoutputscount(&ensemble->network, _state);
    }
    wcount = mlpgetweightscount(&ensemble->network, _state);
    ensemble->ensemblesize = ensemblesize;
    
    /*
     * weights, means, sigmas
     */
    ae_vector_set_length(&ensemble->weights, ensemblesize*wcount, _state);
    ae_vector_set_length(&ensemble->columnmeans, ensemblesize*ccount, _state);
    ae_vector_set_length(&ensemble->columnsigmas, ensemblesize*ccount, _state);
    for(i=0; i<=ensemblesize*wcount-1; i++)
    {
        ensemble->weights.ptr.p_double[i] = ae_randomreal(_state)-0.5;
    }
    for(i=0; i<=ensemblesize-1; i++)
    {
        ae_v_move(&ensemble->columnmeans.ptr.p_double[i*ccount], 1, &network->columnmeans.ptr.p_double[0], 1, ae_v_len(i*ccount,(i+1)*ccount-1));
        ae_v_move(&ensemble->columnsigmas.ptr.p_double[i*ccount], 1, &network->columnsigmas.ptr.p_double[0], 1, ae_v_len(i*ccount,(i+1)*ccount-1));
    }
    
    /*
     * temporaries, internal buffers
     */
    ae_vector_set_length(&ensemble->y, mlpgetoutputscount(&ensemble->network, _state), _state);
}


/*************************************************************************
Copying of MLPEnsemble strucure

INPUT PARAMETERS:
    Ensemble1 -   original

OUTPUT PARAMETERS:
    Ensemble2 -   copy

  -- ALGLIB --
     Copyright 17.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpecopy(mlpensemble* ensemble1,
     mlpensemble* ensemble2,
     ae_state *_state)
{
    ae_int_t ccount;
    ae_int_t wcount;

    _mlpensemble_clear(ensemble2);

    
    /*
     * Unload info
     */
    if( mlpissoftmax(&ensemble1->network, _state) )
    {
        ccount = mlpgetinputscount(&ensemble1->network, _state);
    }
    else
    {
        ccount = mlpgetinputscount(&ensemble1->network, _state)+mlpgetoutputscount(&ensemble1->network, _state);
    }
    wcount = mlpgetweightscount(&ensemble1->network, _state);
    
    /*
     * Allocate space
     */
    ae_vector_set_length(&ensemble2->weights, ensemble1->ensemblesize*wcount, _state);
    ae_vector_set_length(&ensemble2->columnmeans, ensemble1->ensemblesize*ccount, _state);
    ae_vector_set_length(&ensemble2->columnsigmas, ensemble1->ensemblesize*ccount, _state);
    ae_vector_set_length(&ensemble2->y, mlpgetoutputscount(&ensemble1->network, _state), _state);
    
    /*
     * Copy
     */
    ensemble2->ensemblesize = ensemble1->ensemblesize;
    ae_v_move(&ensemble2->weights.ptr.p_double[0], 1, &ensemble1->weights.ptr.p_double[0], 1, ae_v_len(0,ensemble1->ensemblesize*wcount-1));
    ae_v_move(&ensemble2->columnmeans.ptr.p_double[0], 1, &ensemble1->columnmeans.ptr.p_double[0], 1, ae_v_len(0,ensemble1->ensemblesize*ccount-1));
    ae_v_move(&ensemble2->columnsigmas.ptr.p_double[0], 1, &ensemble1->columnsigmas.ptr.p_double[0], 1, ae_v_len(0,ensemble1->ensemblesize*ccount-1));
    mlpcopy(&ensemble1->network, &ensemble2->network, _state);
}


/*************************************************************************
Randomization of MLP ensemble

  -- ALGLIB --
     Copyright 17.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlperandomize(mlpensemble* ensemble, ae_state *_state)
{
    ae_int_t i;
    ae_int_t wcount;


    wcount = mlpgetweightscount(&ensemble->network, _state);
    for(i=0; i<=ensemble->ensemblesize*wcount-1; i++)
    {
        ensemble->weights.ptr.p_double[i] = ae_randomreal(_state)-0.5;
    }
}


/*************************************************************************
Return ensemble properties (number of inputs and outputs).

  -- ALGLIB --
     Copyright 17.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpeproperties(mlpensemble* ensemble,
     ae_int_t* nin,
     ae_int_t* nout,
     ae_state *_state)
{

    *nin = 0;
    *nout = 0;

    *nin = mlpgetinputscount(&ensemble->network, _state);
    *nout = mlpgetoutputscount(&ensemble->network, _state);
}


/*************************************************************************
Return normalization type (whether ensemble is SOFTMAX-normalized or not).

  -- ALGLIB --
     Copyright 17.02.2009 by Bochkanov Sergey
*************************************************************************/
ae_bool mlpeissoftmax(mlpensemble* ensemble, ae_state *_state)
{
    ae_bool result;


    result = mlpissoftmax(&ensemble->network, _state);
    return result;
}


/*************************************************************************
Procesing

INPUT PARAMETERS:
    Ensemble-   neural networks ensemble
    X       -   input vector,  array[0..NIn-1].
    Y       -   (possibly) preallocated buffer; if size of Y is less than
                NOut, it will be reallocated. If it is large enough, it
                is NOT reallocated, so we can save some time on reallocation.


OUTPUT PARAMETERS:
    Y       -   result. Regression estimate when solving regression  task,
                vector of posterior probabilities for classification task.

  -- ALGLIB --
     Copyright 17.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpeprocess(mlpensemble* ensemble,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t es;
    ae_int_t wc;
    ae_int_t cc;
    double v;
    ae_int_t nout;


    if( y->cnt<mlpgetoutputscount(&ensemble->network, _state) )
    {
        ae_vector_set_length(y, mlpgetoutputscount(&ensemble->network, _state), _state);
    }
    es = ensemble->ensemblesize;
    wc = mlpgetweightscount(&ensemble->network, _state);
    if( mlpissoftmax(&ensemble->network, _state) )
    {
        cc = mlpgetinputscount(&ensemble->network, _state);
    }
    else
    {
        cc = mlpgetinputscount(&ensemble->network, _state)+mlpgetoutputscount(&ensemble->network, _state);
    }
    v = (double)1/(double)es;
    nout = mlpgetoutputscount(&ensemble->network, _state);
    for(i=0; i<=nout-1; i++)
    {
        y->ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=es-1; i++)
    {
        ae_v_move(&ensemble->network.weights.ptr.p_double[0], 1, &ensemble->weights.ptr.p_double[i*wc], 1, ae_v_len(0,wc-1));
        ae_v_move(&ensemble->network.columnmeans.ptr.p_double[0], 1, &ensemble->columnmeans.ptr.p_double[i*cc], 1, ae_v_len(0,cc-1));
        ae_v_move(&ensemble->network.columnsigmas.ptr.p_double[0], 1, &ensemble->columnsigmas.ptr.p_double[i*cc], 1, ae_v_len(0,cc-1));
        mlpprocess(&ensemble->network, x, &ensemble->y, _state);
        ae_v_addd(&y->ptr.p_double[0], 1, &ensemble->y.ptr.p_double[0], 1, ae_v_len(0,nout-1), v);
    }
}


/*************************************************************************
'interactive'  variant  of  MLPEProcess  for  languages  like Python which
support constructs like "Y = MLPEProcess(LM,X)" and interactive mode of the
interpreter

This function allocates new array on each call,  so  it  is  significantly
slower than its 'non-interactive' counterpart, but it is  more  convenient
when you call it from command line.

  -- ALGLIB --
     Copyright 17.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpeprocessi(mlpensemble* ensemble,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{

    ae_vector_clear(y);

    mlpeprocess(ensemble, x, y, _state);
}


/*************************************************************************
Calculation of all types of errors

  -- ALGLIB --
     Copyright 17.02.2009 by Bochkanov Sergey
*************************************************************************/
void mlpeallerrorsx(mlpensemble* ensemble,
     /* Real    */ ae_matrix* densexy,
     sparsematrix* sparsexy,
     ae_int_t datasetsize,
     ae_int_t datasettype,
     /* Integer */ ae_vector* idx,
     ae_int_t subset0,
     ae_int_t subset1,
     ae_int_t subsettype,
     ae_shared_pool* buf,
     modelerrors* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t nin;
    ae_int_t nout;
    ae_bool iscls;
    ae_int_t srcidx;
    mlpbuffers *pbuf;
    ae_smart_ptr _pbuf;
    modelerrors rep0;
    modelerrors rep1;

    ae_frame_make(_state, &_frame_block);
    ae_smart_ptr_init(&_pbuf, (void**)&pbuf, _state);
    _modelerrors_init(&rep0, _state);
    _modelerrors_init(&rep1, _state);

    
    /*
     * Get network information
     */
    nin = mlpgetinputscount(&ensemble->network, _state);
    nout = mlpgetoutputscount(&ensemble->network, _state);
    iscls = mlpissoftmax(&ensemble->network, _state);
    
    /*
     * Retrieve buffer, prepare, process data, recycle buffer
     */
    ae_shared_pool_retrieve(buf, &_pbuf, _state);
    if( iscls )
    {
        dserrallocate(nout, &pbuf->tmp0, _state);
    }
    else
    {
        dserrallocate(-nout, &pbuf->tmp0, _state);
    }
    rvectorsetlengthatleast(&pbuf->x, nin, _state);
    rvectorsetlengthatleast(&pbuf->y, nout, _state);
    rvectorsetlengthatleast(&pbuf->desiredy, nout, _state);
    for(i=subset0; i<=subset1-1; i++)
    {
        srcidx = -1;
        if( subsettype==0 )
        {
            srcidx = i;
        }
        if( subsettype==1 )
        {
            srcidx = idx->ptr.p_int[i];
        }
        ae_assert(srcidx>=0, "MLPEAllErrorsX: internal error", _state);
        if( datasettype==0 )
        {
            ae_v_move(&pbuf->x.ptr.p_double[0], 1, &densexy->ptr.pp_double[srcidx][0], 1, ae_v_len(0,nin-1));
        }
        if( datasettype==1 )
        {
            sparsegetrow(sparsexy, srcidx, &pbuf->x, _state);
        }
        mlpeprocess(ensemble, &pbuf->x, &pbuf->y, _state);
        if( mlpissoftmax(&ensemble->network, _state) )
        {
            if( datasettype==0 )
            {
                pbuf->desiredy.ptr.p_double[0] = densexy->ptr.pp_double[srcidx][nin];
            }
            if( datasettype==1 )
            {
                pbuf->desiredy.ptr.p_double[0] = sparseget(sparsexy, srcidx, nin, _state);
            }
        }
        else
        {
            if( datasettype==0 )
            {
                ae_v_move(&pbuf->desiredy.ptr.p_double[0], 1, &densexy->ptr.pp_double[srcidx][nin], 1, ae_v_len(0,nout-1));
            }
            if( datasettype==1 )
            {
                for(j=0; j<=nout-1; j++)
                {
                    pbuf->desiredy.ptr.p_double[j] = sparseget(sparsexy, srcidx, nin+j, _state);
                }
            }
        }
        dserraccumulate(&pbuf->tmp0, &pbuf->y, &pbuf->desiredy, _state);
    }
    dserrfinish(&pbuf->tmp0, _state);
    rep->relclserror = pbuf->tmp0.ptr.p_double[0];
    rep->avgce = pbuf->tmp0.ptr.p_double[1]/ae_log((double)(2), _state);
    rep->rmserror = pbuf->tmp0.ptr.p_double[2];
    rep->avgerror = pbuf->tmp0.ptr.p_double[3];
    rep->avgrelerror = pbuf->tmp0.ptr.p_double[4];
    ae_shared_pool_recycle(buf, &_pbuf, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Calculation of all types of errors on dataset given by sparse matrix

  -- ALGLIB --
     Copyright 10.09.2012 by Bochkanov Sergey
*************************************************************************/
void mlpeallerrorssparse(mlpensemble* ensemble,
     sparsematrix* xy,
     ae_int_t npoints,
     double* relcls,
     double* avgce,
     double* rms,
     double* avg,
     double* avgrel,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_vector buf;
    ae_vector workx;
    ae_vector y;
    ae_vector dy;
    ae_int_t nin;
    ae_int_t nout;

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

    nin = mlpgetinputscount(&ensemble->network, _state);
    nout = mlpgetoutputscount(&ensemble->network, _state);
    if( mlpissoftmax(&ensemble->network, _state) )
    {
        ae_vector_set_length(&dy, 1, _state);
        dserrallocate(nout, &buf, _state);
    }
    else
    {
        ae_vector_set_length(&dy, nout, _state);
        dserrallocate(-nout, &buf, _state);
    }
    for(i=0; i<=npoints-1; i++)
    {
        sparsegetrow(xy, i, &workx, _state);
        mlpeprocess(ensemble, &workx, &y, _state);
        if( mlpissoftmax(&ensemble->network, _state) )
        {
            dy.ptr.p_double[0] = workx.ptr.p_double[nin];
        }
        else
        {
            ae_v_move(&dy.ptr.p_double[0], 1, &workx.ptr.p_double[nin], 1, ae_v_len(0,nout-1));
        }
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
Relative classification error on the test set

INPUT PARAMETERS:
    Ensemble-   ensemble
    XY      -   test set
    NPoints -   test set size

RESULT:
    percent of incorrectly classified cases.
    Works both for classifier betwork and for regression networks which
are used as classifiers.

  -- ALGLIB --
     Copyright 17.02.2009 by Bochkanov Sergey
*************************************************************************/
double mlperelclserror(mlpensemble* ensemble,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_frame _frame_block;
    modelerrors rep;
    double result;

    ae_frame_make(_state, &_frame_block);
    _modelerrors_init(&rep, _state);

    mlpeallerrorsx(ensemble, xy, &ensemble->network.dummysxy, npoints, 0, &ensemble->network.dummyidx, 0, npoints, 0, &ensemble->network.buf, &rep, _state);
    result = rep.relclserror;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Average cross-entropy (in bits per element) on the test set

INPUT PARAMETERS:
    Ensemble-   ensemble
    XY      -   test set
    NPoints -   test set size

RESULT:
    CrossEntropy/(NPoints*LN(2)).
    Zero if ensemble solves regression task.

  -- ALGLIB --
     Copyright 17.02.2009 by Bochkanov Sergey
*************************************************************************/
double mlpeavgce(mlpensemble* ensemble,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_frame _frame_block;
    modelerrors rep;
    double result;

    ae_frame_make(_state, &_frame_block);
    _modelerrors_init(&rep, _state);

    mlpeallerrorsx(ensemble, xy, &ensemble->network.dummysxy, npoints, 0, &ensemble->network.dummyidx, 0, npoints, 0, &ensemble->network.buf, &rep, _state);
    result = rep.avgce;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
RMS error on the test set

INPUT PARAMETERS:
    Ensemble-   ensemble
    XY      -   test set
    NPoints -   test set size

RESULT:
    root mean square error.
    Its meaning for regression task is obvious. As for classification task
RMS error means error when estimating posterior probabilities.

  -- ALGLIB --
     Copyright 17.02.2009 by Bochkanov Sergey
*************************************************************************/
double mlpermserror(mlpensemble* ensemble,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_frame _frame_block;
    modelerrors rep;
    double result;

    ae_frame_make(_state, &_frame_block);
    _modelerrors_init(&rep, _state);

    mlpeallerrorsx(ensemble, xy, &ensemble->network.dummysxy, npoints, 0, &ensemble->network.dummyidx, 0, npoints, 0, &ensemble->network.buf, &rep, _state);
    result = rep.rmserror;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Average error on the test set

INPUT PARAMETERS:
    Ensemble-   ensemble
    XY      -   test set
    NPoints -   test set size

RESULT:
    Its meaning for regression task is obvious. As for classification task
it means average error when estimating posterior probabilities.

  -- ALGLIB --
     Copyright 17.02.2009 by Bochkanov Sergey
*************************************************************************/
double mlpeavgerror(mlpensemble* ensemble,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_frame _frame_block;
    modelerrors rep;
    double result;

    ae_frame_make(_state, &_frame_block);
    _modelerrors_init(&rep, _state);

    mlpeallerrorsx(ensemble, xy, &ensemble->network.dummysxy, npoints, 0, &ensemble->network.dummyidx, 0, npoints, 0, &ensemble->network.buf, &rep, _state);
    result = rep.avgerror;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Average relative error on the test set

INPUT PARAMETERS:
    Ensemble-   ensemble
    XY      -   test set
    NPoints -   test set size

RESULT:
    Its meaning for regression task is obvious. As for classification task
it means average relative error when estimating posterior probabilities.

  -- ALGLIB --
     Copyright 17.02.2009 by Bochkanov Sergey
*************************************************************************/
double mlpeavgrelerror(mlpensemble* ensemble,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_frame _frame_block;
    modelerrors rep;
    double result;

    ae_frame_make(_state, &_frame_block);
    _modelerrors_init(&rep, _state);

    mlpeallerrorsx(ensemble, xy, &ensemble->network.dummysxy, npoints, 0, &ensemble->network.dummyidx, 0, npoints, 0, &ensemble->network.buf, &rep, _state);
    result = rep.avgrelerror;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Serializer: allocation

  -- ALGLIB --
     Copyright 19.10.2011 by Bochkanov Sergey
*************************************************************************/
void mlpealloc(ae_serializer* s, mlpensemble* ensemble, ae_state *_state)
{


    ae_serializer_alloc_entry(s);
    ae_serializer_alloc_entry(s);
    ae_serializer_alloc_entry(s);
    allocrealarray(s, &ensemble->weights, -1, _state);
    allocrealarray(s, &ensemble->columnmeans, -1, _state);
    allocrealarray(s, &ensemble->columnsigmas, -1, _state);
    mlpalloc(s, &ensemble->network, _state);
}


/*************************************************************************
Serializer: serialization

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void mlpeserialize(ae_serializer* s,
     mlpensemble* ensemble,
     ae_state *_state)
{


    ae_serializer_serialize_int(s, getmlpeserializationcode(_state), _state);
    ae_serializer_serialize_int(s, mlpe_mlpefirstversion, _state);
    ae_serializer_serialize_int(s, ensemble->ensemblesize, _state);
    serializerealarray(s, &ensemble->weights, -1, _state);
    serializerealarray(s, &ensemble->columnmeans, -1, _state);
    serializerealarray(s, &ensemble->columnsigmas, -1, _state);
    mlpserialize(s, &ensemble->network, _state);
}


/*************************************************************************
Serializer: unserialization

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void mlpeunserialize(ae_serializer* s,
     mlpensemble* ensemble,
     ae_state *_state)
{
    ae_int_t i0;
    ae_int_t i1;

    _mlpensemble_clear(ensemble);

    
    /*
     * check correctness of header
     */
    ae_serializer_unserialize_int(s, &i0, _state);
    ae_assert(i0==getmlpeserializationcode(_state), "MLPEUnserialize: stream header corrupted", _state);
    ae_serializer_unserialize_int(s, &i1, _state);
    ae_assert(i1==mlpe_mlpefirstversion, "MLPEUnserialize: stream header corrupted", _state);
    
    /*
     * Create network
     */
    ae_serializer_unserialize_int(s, &ensemble->ensemblesize, _state);
    unserializerealarray(s, &ensemble->weights, _state);
    unserializerealarray(s, &ensemble->columnmeans, _state);
    unserializerealarray(s, &ensemble->columnsigmas, _state);
    mlpunserialize(s, &ensemble->network, _state);
    
    /*
     * Allocate termoraries
     */
    ae_vector_set_length(&ensemble->y, mlpgetoutputscount(&ensemble->network, _state), _state);
}


void _mlpensemble_init(void* _p, ae_state *_state)
{
    mlpensemble *p = (mlpensemble*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->weights, 0, DT_REAL, _state);
    ae_vector_init(&p->columnmeans, 0, DT_REAL, _state);
    ae_vector_init(&p->columnsigmas, 0, DT_REAL, _state);
    _multilayerperceptron_init(&p->network, _state);
    ae_vector_init(&p->y, 0, DT_REAL, _state);
}


void _mlpensemble_init_copy(void* _dst, void* _src, ae_state *_state)
{
    mlpensemble *dst = (mlpensemble*)_dst;
    mlpensemble *src = (mlpensemble*)_src;
    dst->ensemblesize = src->ensemblesize;
    ae_vector_init_copy(&dst->weights, &src->weights, _state);
    ae_vector_init_copy(&dst->columnmeans, &src->columnmeans, _state);
    ae_vector_init_copy(&dst->columnsigmas, &src->columnsigmas, _state);
    _multilayerperceptron_init_copy(&dst->network, &src->network, _state);
    ae_vector_init_copy(&dst->y, &src->y, _state);
}


void _mlpensemble_clear(void* _p)
{
    mlpensemble *p = (mlpensemble*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->weights);
    ae_vector_clear(&p->columnmeans);
    ae_vector_clear(&p->columnsigmas);
    _multilayerperceptron_clear(&p->network);
    ae_vector_clear(&p->y);
}


void _mlpensemble_destroy(void* _p)
{
    mlpensemble *p = (mlpensemble*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->weights);
    ae_vector_destroy(&p->columnmeans);
    ae_vector_destroy(&p->columnsigmas);
    _multilayerperceptron_destroy(&p->network);
    ae_vector_destroy(&p->y);
}


/*$ End $*/
