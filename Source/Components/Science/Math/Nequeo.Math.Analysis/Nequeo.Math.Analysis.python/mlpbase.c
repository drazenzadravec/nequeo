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
#include "mlpbase.h"


/*$ Declarations $*/
static ae_int_t mlpbase_mlpvnum = 7;
static ae_int_t mlpbase_mlpfirstversion = 0;
static ae_int_t mlpbase_nfieldwidth = 4;
static ae_int_t mlpbase_hlconnfieldwidth = 5;
static ae_int_t mlpbase_hlnfieldwidth = 4;
static ae_int_t mlpbase_gradbasecasecost = 50000;
static ae_int_t mlpbase_microbatchsize = 64;
static void mlpbase_addinputlayer(ae_int_t ncount,
     /* Integer */ ae_vector* lsizes,
     /* Integer */ ae_vector* ltypes,
     /* Integer */ ae_vector* lconnfirst,
     /* Integer */ ae_vector* lconnlast,
     ae_int_t* lastproc,
     ae_state *_state);
static void mlpbase_addbiasedsummatorlayer(ae_int_t ncount,
     /* Integer */ ae_vector* lsizes,
     /* Integer */ ae_vector* ltypes,
     /* Integer */ ae_vector* lconnfirst,
     /* Integer */ ae_vector* lconnlast,
     ae_int_t* lastproc,
     ae_state *_state);
static void mlpbase_addactivationlayer(ae_int_t functype,
     /* Integer */ ae_vector* lsizes,
     /* Integer */ ae_vector* ltypes,
     /* Integer */ ae_vector* lconnfirst,
     /* Integer */ ae_vector* lconnlast,
     ae_int_t* lastproc,
     ae_state *_state);
static void mlpbase_addzerolayer(/* Integer */ ae_vector* lsizes,
     /* Integer */ ae_vector* ltypes,
     /* Integer */ ae_vector* lconnfirst,
     /* Integer */ ae_vector* lconnlast,
     ae_int_t* lastproc,
     ae_state *_state);
static void mlpbase_hladdinputlayer(multilayerperceptron* network,
     ae_int_t* connidx,
     ae_int_t* neuroidx,
     ae_int_t* structinfoidx,
     ae_int_t nin,
     ae_state *_state);
static void mlpbase_hladdoutputlayer(multilayerperceptron* network,
     ae_int_t* connidx,
     ae_int_t* neuroidx,
     ae_int_t* structinfoidx,
     ae_int_t* weightsidx,
     ae_int_t k,
     ae_int_t nprev,
     ae_int_t nout,
     ae_bool iscls,
     ae_bool islinearout,
     ae_state *_state);
static void mlpbase_hladdhiddenlayer(multilayerperceptron* network,
     ae_int_t* connidx,
     ae_int_t* neuroidx,
     ae_int_t* structinfoidx,
     ae_int_t* weightsidx,
     ae_int_t k,
     ae_int_t nprev,
     ae_int_t ncur,
     ae_state *_state);
static void mlpbase_fillhighlevelinformation(multilayerperceptron* network,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_bool iscls,
     ae_bool islinearout,
     ae_state *_state);
static void mlpbase_mlpcreate(ae_int_t nin,
     ae_int_t nout,
     /* Integer */ ae_vector* lsizes,
     /* Integer */ ae_vector* ltypes,
     /* Integer */ ae_vector* lconnfirst,
     /* Integer */ ae_vector* lconnlast,
     ae_int_t layerscount,
     ae_bool isclsnet,
     multilayerperceptron* network,
     ae_state *_state);
static void mlpbase_mlphessianbatchinternal(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t ssize,
     ae_bool naturalerr,
     double* e,
     /* Real    */ ae_vector* grad,
     /* Real    */ ae_matrix* h,
     ae_state *_state);
static void mlpbase_mlpinternalcalculategradient(multilayerperceptron* network,
     /* Real    */ ae_vector* neurons,
     /* Real    */ ae_vector* weights,
     /* Real    */ ae_vector* derror,
     /* Real    */ ae_vector* grad,
     ae_bool naturalerrorfunc,
     ae_state *_state);
static void mlpbase_mlpchunkedgradient(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t cstart,
     ae_int_t csize,
     /* Real    */ ae_vector* batch4buf,
     /* Real    */ ae_vector* hpcbuf,
     double* e,
     ae_bool naturalerrorfunc,
     ae_state *_state);
static void mlpbase_mlpchunkedprocess(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t cstart,
     ae_int_t csize,
     /* Real    */ ae_vector* batch4buf,
     /* Real    */ ae_vector* hpcbuf,
     ae_state *_state);
static double mlpbase_safecrossentropy(double t,
     double z,
     ae_state *_state);
static void mlpbase_randomizebackwardpass(multilayerperceptron* network,
     ae_int_t neuronidx,
     double v,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
This function returns number of weights  updates  which  is  required  for
gradient calculation problem to be splitted.
*************************************************************************/
ae_int_t mlpgradsplitcost(ae_state *_state)
{
    ae_int_t result;


    result = mlpbase_gradbasecasecost;
    return result;
}


/*************************************************************************
This  function  returns  number  of elements in subset of dataset which is
required for gradient calculation problem to be splitted.
*************************************************************************/
ae_int_t mlpgradsplitsize(ae_state *_state)
{
    ae_int_t result;


    result = mlpbase_microbatchsize;
    return result;
}


/*************************************************************************
Creates  neural  network  with  NIn  inputs,  NOut outputs, without hidden
layers, with linear output layer. Network weights are  filled  with  small
random values.

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlpcreate0(ae_int_t nin,
     ae_int_t nout,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lsizes;
    ae_vector ltypes;
    ae_vector lconnfirst;
    ae_vector lconnlast;
    ae_int_t layerscount;
    ae_int_t lastproc;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_clear(network);
    ae_vector_init(&lsizes, 0, DT_INT, _state);
    ae_vector_init(&ltypes, 0, DT_INT, _state);
    ae_vector_init(&lconnfirst, 0, DT_INT, _state);
    ae_vector_init(&lconnlast, 0, DT_INT, _state);

    layerscount = 1+3;
    
    /*
     * Allocate arrays
     */
    ae_vector_set_length(&lsizes, layerscount-1+1, _state);
    ae_vector_set_length(&ltypes, layerscount-1+1, _state);
    ae_vector_set_length(&lconnfirst, layerscount-1+1, _state);
    ae_vector_set_length(&lconnlast, layerscount-1+1, _state);
    
    /*
     * Layers
     */
    mlpbase_addinputlayer(nin, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(-5, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    
    /*
     * Create
     */
    mlpbase_mlpcreate(nin, nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, layerscount, ae_false, network, _state);
    mlpbase_fillhighlevelinformation(network, nin, 0, 0, nout, ae_false, ae_true, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Same  as  MLPCreate0,  but  with  one  hidden  layer  (NHid  neurons) with
non-linear activation function. Output layer is linear.

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlpcreate1(ae_int_t nin,
     ae_int_t nhid,
     ae_int_t nout,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lsizes;
    ae_vector ltypes;
    ae_vector lconnfirst;
    ae_vector lconnlast;
    ae_int_t layerscount;
    ae_int_t lastproc;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_clear(network);
    ae_vector_init(&lsizes, 0, DT_INT, _state);
    ae_vector_init(&ltypes, 0, DT_INT, _state);
    ae_vector_init(&lconnfirst, 0, DT_INT, _state);
    ae_vector_init(&lconnlast, 0, DT_INT, _state);

    layerscount = 1+3+3;
    
    /*
     * Allocate arrays
     */
    ae_vector_set_length(&lsizes, layerscount-1+1, _state);
    ae_vector_set_length(&ltypes, layerscount-1+1, _state);
    ae_vector_set_length(&lconnfirst, layerscount-1+1, _state);
    ae_vector_set_length(&lconnlast, layerscount-1+1, _state);
    
    /*
     * Layers
     */
    mlpbase_addinputlayer(nin, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nhid, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(-5, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    
    /*
     * Create
     */
    mlpbase_mlpcreate(nin, nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, layerscount, ae_false, network, _state);
    mlpbase_fillhighlevelinformation(network, nin, nhid, 0, nout, ae_false, ae_true, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Same as MLPCreate0, but with two hidden layers (NHid1 and  NHid2  neurons)
with non-linear activation function. Output layer is linear.
 $ALL

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlpcreate2(ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lsizes;
    ae_vector ltypes;
    ae_vector lconnfirst;
    ae_vector lconnlast;
    ae_int_t layerscount;
    ae_int_t lastproc;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_clear(network);
    ae_vector_init(&lsizes, 0, DT_INT, _state);
    ae_vector_init(&ltypes, 0, DT_INT, _state);
    ae_vector_init(&lconnfirst, 0, DT_INT, _state);
    ae_vector_init(&lconnlast, 0, DT_INT, _state);

    layerscount = 1+3+3+3;
    
    /*
     * Allocate arrays
     */
    ae_vector_set_length(&lsizes, layerscount-1+1, _state);
    ae_vector_set_length(&ltypes, layerscount-1+1, _state);
    ae_vector_set_length(&lconnfirst, layerscount-1+1, _state);
    ae_vector_set_length(&lconnlast, layerscount-1+1, _state);
    
    /*
     * Layers
     */
    mlpbase_addinputlayer(nin, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nhid1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nhid2, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(-5, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    
    /*
     * Create
     */
    mlpbase_mlpcreate(nin, nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, layerscount, ae_false, network, _state);
    mlpbase_fillhighlevelinformation(network, nin, nhid1, nhid2, nout, ae_false, ae_true, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Creates  neural  network  with  NIn  inputs,  NOut outputs, without hidden
layers with non-linear output layer. Network weights are filled with small
random values.

Activation function of the output layer takes values:

    (B, +INF), if D>=0

or

    (-INF, B), if D<0.


  -- ALGLIB --
     Copyright 30.03.2008 by Bochkanov Sergey
*************************************************************************/
void mlpcreateb0(ae_int_t nin,
     ae_int_t nout,
     double b,
     double d,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lsizes;
    ae_vector ltypes;
    ae_vector lconnfirst;
    ae_vector lconnlast;
    ae_int_t layerscount;
    ae_int_t lastproc;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_clear(network);
    ae_vector_init(&lsizes, 0, DT_INT, _state);
    ae_vector_init(&ltypes, 0, DT_INT, _state);
    ae_vector_init(&lconnfirst, 0, DT_INT, _state);
    ae_vector_init(&lconnlast, 0, DT_INT, _state);

    layerscount = 1+3;
    if( ae_fp_greater_eq(d,(double)(0)) )
    {
        d = (double)(1);
    }
    else
    {
        d = (double)(-1);
    }
    
    /*
     * Allocate arrays
     */
    ae_vector_set_length(&lsizes, layerscount-1+1, _state);
    ae_vector_set_length(&ltypes, layerscount-1+1, _state);
    ae_vector_set_length(&lconnfirst, layerscount-1+1, _state);
    ae_vector_set_length(&lconnlast, layerscount-1+1, _state);
    
    /*
     * Layers
     */
    mlpbase_addinputlayer(nin, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(3, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    
    /*
     * Create
     */
    mlpbase_mlpcreate(nin, nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, layerscount, ae_false, network, _state);
    mlpbase_fillhighlevelinformation(network, nin, 0, 0, nout, ae_false, ae_false, _state);
    
    /*
     * Turn on ouputs shift/scaling.
     */
    for(i=nin; i<=nin+nout-1; i++)
    {
        network->columnmeans.ptr.p_double[i] = b;
        network->columnsigmas.ptr.p_double[i] = d;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Same as MLPCreateB0 but with non-linear hidden layer.

  -- ALGLIB --
     Copyright 30.03.2008 by Bochkanov Sergey
*************************************************************************/
void mlpcreateb1(ae_int_t nin,
     ae_int_t nhid,
     ae_int_t nout,
     double b,
     double d,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lsizes;
    ae_vector ltypes;
    ae_vector lconnfirst;
    ae_vector lconnlast;
    ae_int_t layerscount;
    ae_int_t lastproc;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_clear(network);
    ae_vector_init(&lsizes, 0, DT_INT, _state);
    ae_vector_init(&ltypes, 0, DT_INT, _state);
    ae_vector_init(&lconnfirst, 0, DT_INT, _state);
    ae_vector_init(&lconnlast, 0, DT_INT, _state);

    layerscount = 1+3+3;
    if( ae_fp_greater_eq(d,(double)(0)) )
    {
        d = (double)(1);
    }
    else
    {
        d = (double)(-1);
    }
    
    /*
     * Allocate arrays
     */
    ae_vector_set_length(&lsizes, layerscount-1+1, _state);
    ae_vector_set_length(&ltypes, layerscount-1+1, _state);
    ae_vector_set_length(&lconnfirst, layerscount-1+1, _state);
    ae_vector_set_length(&lconnlast, layerscount-1+1, _state);
    
    /*
     * Layers
     */
    mlpbase_addinputlayer(nin, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nhid, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(3, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    
    /*
     * Create
     */
    mlpbase_mlpcreate(nin, nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, layerscount, ae_false, network, _state);
    mlpbase_fillhighlevelinformation(network, nin, nhid, 0, nout, ae_false, ae_false, _state);
    
    /*
     * Turn on ouputs shift/scaling.
     */
    for(i=nin; i<=nin+nout-1; i++)
    {
        network->columnmeans.ptr.p_double[i] = b;
        network->columnsigmas.ptr.p_double[i] = d;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Same as MLPCreateB0 but with two non-linear hidden layers.

  -- ALGLIB --
     Copyright 30.03.2008 by Bochkanov Sergey
*************************************************************************/
void mlpcreateb2(ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     double b,
     double d,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lsizes;
    ae_vector ltypes;
    ae_vector lconnfirst;
    ae_vector lconnlast;
    ae_int_t layerscount;
    ae_int_t lastproc;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_clear(network);
    ae_vector_init(&lsizes, 0, DT_INT, _state);
    ae_vector_init(&ltypes, 0, DT_INT, _state);
    ae_vector_init(&lconnfirst, 0, DT_INT, _state);
    ae_vector_init(&lconnlast, 0, DT_INT, _state);

    layerscount = 1+3+3+3;
    if( ae_fp_greater_eq(d,(double)(0)) )
    {
        d = (double)(1);
    }
    else
    {
        d = (double)(-1);
    }
    
    /*
     * Allocate arrays
     */
    ae_vector_set_length(&lsizes, layerscount-1+1, _state);
    ae_vector_set_length(&ltypes, layerscount-1+1, _state);
    ae_vector_set_length(&lconnfirst, layerscount-1+1, _state);
    ae_vector_set_length(&lconnlast, layerscount-1+1, _state);
    
    /*
     * Layers
     */
    mlpbase_addinputlayer(nin, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nhid1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nhid2, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(3, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    
    /*
     * Create
     */
    mlpbase_mlpcreate(nin, nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, layerscount, ae_false, network, _state);
    mlpbase_fillhighlevelinformation(network, nin, nhid1, nhid2, nout, ae_false, ae_false, _state);
    
    /*
     * Turn on ouputs shift/scaling.
     */
    for(i=nin; i<=nin+nout-1; i++)
    {
        network->columnmeans.ptr.p_double[i] = b;
        network->columnsigmas.ptr.p_double[i] = d;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Creates  neural  network  with  NIn  inputs,  NOut outputs, without hidden
layers with non-linear output layer. Network weights are filled with small
random values. Activation function of the output layer takes values [A,B].

  -- ALGLIB --
     Copyright 30.03.2008 by Bochkanov Sergey
*************************************************************************/
void mlpcreater0(ae_int_t nin,
     ae_int_t nout,
     double a,
     double b,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lsizes;
    ae_vector ltypes;
    ae_vector lconnfirst;
    ae_vector lconnlast;
    ae_int_t layerscount;
    ae_int_t lastproc;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_clear(network);
    ae_vector_init(&lsizes, 0, DT_INT, _state);
    ae_vector_init(&ltypes, 0, DT_INT, _state);
    ae_vector_init(&lconnfirst, 0, DT_INT, _state);
    ae_vector_init(&lconnlast, 0, DT_INT, _state);

    layerscount = 1+3;
    
    /*
     * Allocate arrays
     */
    ae_vector_set_length(&lsizes, layerscount-1+1, _state);
    ae_vector_set_length(&ltypes, layerscount-1+1, _state);
    ae_vector_set_length(&lconnfirst, layerscount-1+1, _state);
    ae_vector_set_length(&lconnlast, layerscount-1+1, _state);
    
    /*
     * Layers
     */
    mlpbase_addinputlayer(nin, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    
    /*
     * Create
     */
    mlpbase_mlpcreate(nin, nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, layerscount, ae_false, network, _state);
    mlpbase_fillhighlevelinformation(network, nin, 0, 0, nout, ae_false, ae_false, _state);
    
    /*
     * Turn on outputs shift/scaling.
     */
    for(i=nin; i<=nin+nout-1; i++)
    {
        network->columnmeans.ptr.p_double[i] = 0.5*(a+b);
        network->columnsigmas.ptr.p_double[i] = 0.5*(a-b);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Same as MLPCreateR0, but with non-linear hidden layer.

  -- ALGLIB --
     Copyright 30.03.2008 by Bochkanov Sergey
*************************************************************************/
void mlpcreater1(ae_int_t nin,
     ae_int_t nhid,
     ae_int_t nout,
     double a,
     double b,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lsizes;
    ae_vector ltypes;
    ae_vector lconnfirst;
    ae_vector lconnlast;
    ae_int_t layerscount;
    ae_int_t lastproc;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_clear(network);
    ae_vector_init(&lsizes, 0, DT_INT, _state);
    ae_vector_init(&ltypes, 0, DT_INT, _state);
    ae_vector_init(&lconnfirst, 0, DT_INT, _state);
    ae_vector_init(&lconnlast, 0, DT_INT, _state);

    layerscount = 1+3+3;
    
    /*
     * Allocate arrays
     */
    ae_vector_set_length(&lsizes, layerscount-1+1, _state);
    ae_vector_set_length(&ltypes, layerscount-1+1, _state);
    ae_vector_set_length(&lconnfirst, layerscount-1+1, _state);
    ae_vector_set_length(&lconnlast, layerscount-1+1, _state);
    
    /*
     * Layers
     */
    mlpbase_addinputlayer(nin, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nhid, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    
    /*
     * Create
     */
    mlpbase_mlpcreate(nin, nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, layerscount, ae_false, network, _state);
    mlpbase_fillhighlevelinformation(network, nin, nhid, 0, nout, ae_false, ae_false, _state);
    
    /*
     * Turn on outputs shift/scaling.
     */
    for(i=nin; i<=nin+nout-1; i++)
    {
        network->columnmeans.ptr.p_double[i] = 0.5*(a+b);
        network->columnsigmas.ptr.p_double[i] = 0.5*(a-b);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Same as MLPCreateR0, but with two non-linear hidden layers.

  -- ALGLIB --
     Copyright 30.03.2008 by Bochkanov Sergey
*************************************************************************/
void mlpcreater2(ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     double a,
     double b,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lsizes;
    ae_vector ltypes;
    ae_vector lconnfirst;
    ae_vector lconnlast;
    ae_int_t layerscount;
    ae_int_t lastproc;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_clear(network);
    ae_vector_init(&lsizes, 0, DT_INT, _state);
    ae_vector_init(&ltypes, 0, DT_INT, _state);
    ae_vector_init(&lconnfirst, 0, DT_INT, _state);
    ae_vector_init(&lconnlast, 0, DT_INT, _state);

    layerscount = 1+3+3+3;
    
    /*
     * Allocate arrays
     */
    ae_vector_set_length(&lsizes, layerscount-1+1, _state);
    ae_vector_set_length(&ltypes, layerscount-1+1, _state);
    ae_vector_set_length(&lconnfirst, layerscount-1+1, _state);
    ae_vector_set_length(&lconnlast, layerscount-1+1, _state);
    
    /*
     * Layers
     */
    mlpbase_addinputlayer(nin, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nhid1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nhid2, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    
    /*
     * Create
     */
    mlpbase_mlpcreate(nin, nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, layerscount, ae_false, network, _state);
    mlpbase_fillhighlevelinformation(network, nin, nhid1, nhid2, nout, ae_false, ae_false, _state);
    
    /*
     * Turn on outputs shift/scaling.
     */
    for(i=nin; i<=nin+nout-1; i++)
    {
        network->columnmeans.ptr.p_double[i] = 0.5*(a+b);
        network->columnsigmas.ptr.p_double[i] = 0.5*(a-b);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Creates classifier network with NIn  inputs  and  NOut  possible  classes.
Network contains no hidden layers and linear output  layer  with  SOFTMAX-
normalization  (so  outputs  sums  up  to  1.0  and  converge to posterior
probabilities).

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlpcreatec0(ae_int_t nin,
     ae_int_t nout,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lsizes;
    ae_vector ltypes;
    ae_vector lconnfirst;
    ae_vector lconnlast;
    ae_int_t layerscount;
    ae_int_t lastproc;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_clear(network);
    ae_vector_init(&lsizes, 0, DT_INT, _state);
    ae_vector_init(&ltypes, 0, DT_INT, _state);
    ae_vector_init(&lconnfirst, 0, DT_INT, _state);
    ae_vector_init(&lconnlast, 0, DT_INT, _state);

    ae_assert(nout>=2, "MLPCreateC0: NOut<2!", _state);
    layerscount = 1+2+1;
    
    /*
     * Allocate arrays
     */
    ae_vector_set_length(&lsizes, layerscount-1+1, _state);
    ae_vector_set_length(&ltypes, layerscount-1+1, _state);
    ae_vector_set_length(&lconnfirst, layerscount-1+1, _state);
    ae_vector_set_length(&lconnlast, layerscount-1+1, _state);
    
    /*
     * Layers
     */
    mlpbase_addinputlayer(nin, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nout-1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addzerolayer(&lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    
    /*
     * Create
     */
    mlpbase_mlpcreate(nin, nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, layerscount, ae_true, network, _state);
    mlpbase_fillhighlevelinformation(network, nin, 0, 0, nout, ae_true, ae_true, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Same as MLPCreateC0, but with one non-linear hidden layer.

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlpcreatec1(ae_int_t nin,
     ae_int_t nhid,
     ae_int_t nout,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lsizes;
    ae_vector ltypes;
    ae_vector lconnfirst;
    ae_vector lconnlast;
    ae_int_t layerscount;
    ae_int_t lastproc;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_clear(network);
    ae_vector_init(&lsizes, 0, DT_INT, _state);
    ae_vector_init(&ltypes, 0, DT_INT, _state);
    ae_vector_init(&lconnfirst, 0, DT_INT, _state);
    ae_vector_init(&lconnlast, 0, DT_INT, _state);

    ae_assert(nout>=2, "MLPCreateC1: NOut<2!", _state);
    layerscount = 1+3+2+1;
    
    /*
     * Allocate arrays
     */
    ae_vector_set_length(&lsizes, layerscount-1+1, _state);
    ae_vector_set_length(&ltypes, layerscount-1+1, _state);
    ae_vector_set_length(&lconnfirst, layerscount-1+1, _state);
    ae_vector_set_length(&lconnlast, layerscount-1+1, _state);
    
    /*
     * Layers
     */
    mlpbase_addinputlayer(nin, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nhid, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nout-1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addzerolayer(&lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    
    /*
     * Create
     */
    mlpbase_mlpcreate(nin, nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, layerscount, ae_true, network, _state);
    mlpbase_fillhighlevelinformation(network, nin, nhid, 0, nout, ae_true, ae_true, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Same as MLPCreateC0, but with two non-linear hidden layers.

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlpcreatec2(ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector lsizes;
    ae_vector ltypes;
    ae_vector lconnfirst;
    ae_vector lconnlast;
    ae_int_t layerscount;
    ae_int_t lastproc;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_clear(network);
    ae_vector_init(&lsizes, 0, DT_INT, _state);
    ae_vector_init(&ltypes, 0, DT_INT, _state);
    ae_vector_init(&lconnfirst, 0, DT_INT, _state);
    ae_vector_init(&lconnlast, 0, DT_INT, _state);

    ae_assert(nout>=2, "MLPCreateC2: NOut<2!", _state);
    layerscount = 1+3+3+2+1;
    
    /*
     * Allocate arrays
     */
    ae_vector_set_length(&lsizes, layerscount-1+1, _state);
    ae_vector_set_length(&ltypes, layerscount-1+1, _state);
    ae_vector_set_length(&lconnfirst, layerscount-1+1, _state);
    ae_vector_set_length(&lconnlast, layerscount-1+1, _state);
    
    /*
     * Layers
     */
    mlpbase_addinputlayer(nin, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nhid1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nhid2, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addactivationlayer(1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addbiasedsummatorlayer(nout-1, &lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    mlpbase_addzerolayer(&lsizes, &ltypes, &lconnfirst, &lconnlast, &lastproc, _state);
    
    /*
     * Create
     */
    mlpbase_mlpcreate(nin, nout, &lsizes, &ltypes, &lconnfirst, &lconnlast, layerscount, ae_true, network, _state);
    mlpbase_fillhighlevelinformation(network, nin, nhid1, nhid2, nout, ae_true, ae_true, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Copying of neural network

INPUT PARAMETERS:
    Network1 -   original

OUTPUT PARAMETERS:
    Network2 -   copy

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlpcopy(multilayerperceptron* network1,
     multilayerperceptron* network2,
     ae_state *_state)
{

    _multilayerperceptron_clear(network2);

    mlpcopyshared(network1, network2, _state);
}


/*************************************************************************
Copying of neural network (second parameter is passed as shared object).

INPUT PARAMETERS:
    Network1 -   original

OUTPUT PARAMETERS:
    Network2 -   copy

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlpcopyshared(multilayerperceptron* network1,
     multilayerperceptron* network2,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t wcount;
    ae_int_t i;
    mlpbuffers buf;
    smlpgrad sgrad;

    ae_frame_make(_state, &_frame_block);
    _mlpbuffers_init(&buf, _state);
    _smlpgrad_init(&sgrad, _state);

    
    /*
     * Copy scalar and array fields
     */
    network2->hlnetworktype = network1->hlnetworktype;
    network2->hlnormtype = network1->hlnormtype;
    copyintegerarray(&network1->hllayersizes, &network2->hllayersizes, _state);
    copyintegerarray(&network1->hlconnections, &network2->hlconnections, _state);
    copyintegerarray(&network1->hlneurons, &network2->hlneurons, _state);
    copyintegerarray(&network1->structinfo, &network2->structinfo, _state);
    copyrealarray(&network1->weights, &network2->weights, _state);
    copyrealarray(&network1->columnmeans, &network2->columnmeans, _state);
    copyrealarray(&network1->columnsigmas, &network2->columnsigmas, _state);
    copyrealarray(&network1->neurons, &network2->neurons, _state);
    copyrealarray(&network1->dfdnet, &network2->dfdnet, _state);
    copyrealarray(&network1->derror, &network2->derror, _state);
    copyrealarray(&network1->x, &network2->x, _state);
    copyrealarray(&network1->y, &network2->y, _state);
    copyrealarray(&network1->nwbuf, &network2->nwbuf, _state);
    copyintegerarray(&network1->integerbuf, &network2->integerbuf, _state);
    
    /*
     * copy buffers
     */
    wcount = mlpgetweightscount(network1, _state);
    ae_shared_pool_set_seed(&network2->buf, &buf, sizeof(buf), _mlpbuffers_init, _mlpbuffers_init_copy, _mlpbuffers_destroy, _state);
    ae_vector_set_length(&sgrad.g, wcount, _state);
    sgrad.f = 0.0;
    for(i=0; i<=wcount-1; i++)
    {
        sgrad.g.ptr.p_double[i] = 0.0;
    }
    ae_shared_pool_set_seed(&network2->gradbuf, &sgrad, sizeof(sgrad), _smlpgrad_init, _smlpgrad_init_copy, _smlpgrad_destroy, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
This function compares architectures of neural networks.  Only  geometries
are compared, weights and other parameters are not tested.

  -- ALGLIB --
     Copyright 20.06.2013 by Bochkanov Sergey
*************************************************************************/
ae_bool mlpsamearchitecture(multilayerperceptron* network1,
     multilayerperceptron* network2,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t ninfo;
    ae_bool result;


    ae_assert(network1->structinfo.cnt>0&&network1->structinfo.cnt>=network1->structinfo.ptr.p_int[0], "MLPSameArchitecture: Network1 is uninitialized", _state);
    ae_assert(network2->structinfo.cnt>0&&network2->structinfo.cnt>=network2->structinfo.ptr.p_int[0], "MLPSameArchitecture: Network2 is uninitialized", _state);
    result = ae_false;
    if( network1->structinfo.ptr.p_int[0]!=network2->structinfo.ptr.p_int[0] )
    {
        return result;
    }
    ninfo = network1->structinfo.ptr.p_int[0];
    for(i=0; i<=ninfo-1; i++)
    {
        if( network1->structinfo.ptr.p_int[i]!=network2->structinfo.ptr.p_int[i] )
        {
            return result;
        }
    }
    result = ae_true;
    return result;
}


/*************************************************************************
This function copies tunable  parameters (weights/means/sigmas)  from  one
network to another with same architecture. It  performs  some  rudimentary
checks that architectures are same, and throws exception if check fails.

It is intended for fast copying of states between two  network  which  are
known to have same geometry.

INPUT PARAMETERS:
    Network1 -   source, must be correctly initialized
    Network2 -   target, must have same architecture

OUTPUT PARAMETERS:
    Network2 -   network state is copied from source to target

  -- ALGLIB --
     Copyright 20.06.2013 by Bochkanov Sergey
*************************************************************************/
void mlpcopytunableparameters(multilayerperceptron* network1,
     multilayerperceptron* network2,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t ninfo;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;


    ae_assert(network1->structinfo.cnt>0&&network1->structinfo.cnt>=network1->structinfo.ptr.p_int[0], "MLPCopyTunableParameters: Network1 is uninitialized", _state);
    ae_assert(network2->structinfo.cnt>0&&network2->structinfo.cnt>=network2->structinfo.ptr.p_int[0], "MLPCopyTunableParameters: Network2 is uninitialized", _state);
    ae_assert(network1->structinfo.ptr.p_int[0]==network2->structinfo.ptr.p_int[0], "MLPCopyTunableParameters: Network1 geometry differs from that of Network2", _state);
    ninfo = network1->structinfo.ptr.p_int[0];
    for(i=0; i<=ninfo-1; i++)
    {
        ae_assert(network1->structinfo.ptr.p_int[i]==network2->structinfo.ptr.p_int[i], "MLPCopyTunableParameters: Network1 geometry differs from that of Network2", _state);
    }
    mlpproperties(network1, &nin, &nout, &wcount, _state);
    for(i=0; i<=wcount-1; i++)
    {
        network2->weights.ptr.p_double[i] = network1->weights.ptr.p_double[i];
    }
    if( mlpissoftmax(network1, _state) )
    {
        for(i=0; i<=nin-1; i++)
        {
            network2->columnmeans.ptr.p_double[i] = network1->columnmeans.ptr.p_double[i];
            network2->columnsigmas.ptr.p_double[i] = network1->columnsigmas.ptr.p_double[i];
        }
    }
    else
    {
        for(i=0; i<=nin+nout-1; i++)
        {
            network2->columnmeans.ptr.p_double[i] = network1->columnmeans.ptr.p_double[i];
            network2->columnsigmas.ptr.p_double[i] = network1->columnsigmas.ptr.p_double[i];
        }
    }
}


/*************************************************************************
This  function  exports  tunable   parameters  (weights/means/sigmas) from
network to contiguous array. Nothing is guaranteed about array format, the
only thing you can count for is that MLPImportTunableParameters() will  be
able to parse it.

It is intended for fast copying of states between network and backup array

INPUT PARAMETERS:
    Network     -   source, must be correctly initialized
    P           -   array to use. If its size is enough to store data,  it
                    is reused.

OUTPUT PARAMETERS:
    P           -   array which stores network parameters, resized if needed
    PCount      -   number of parameters stored in array.

  -- ALGLIB --
     Copyright 20.06.2013 by Bochkanov Sergey
*************************************************************************/
void mlpexporttunableparameters(multilayerperceptron* network,
     /* Real    */ ae_vector* p,
     ae_int_t* pcount,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;

    *pcount = 0;

    ae_assert(network->structinfo.cnt>0&&network->structinfo.cnt>=network->structinfo.ptr.p_int[0], "MLPExportTunableParameters: Network is uninitialized", _state);
    mlpproperties(network, &nin, &nout, &wcount, _state);
    if( mlpissoftmax(network, _state) )
    {
        *pcount = wcount+2*nin;
        rvectorsetlengthatleast(p, *pcount, _state);
        k = 0;
        for(i=0; i<=wcount-1; i++)
        {
            p->ptr.p_double[k] = network->weights.ptr.p_double[i];
            k = k+1;
        }
        for(i=0; i<=nin-1; i++)
        {
            p->ptr.p_double[k] = network->columnmeans.ptr.p_double[i];
            k = k+1;
            p->ptr.p_double[k] = network->columnsigmas.ptr.p_double[i];
            k = k+1;
        }
    }
    else
    {
        *pcount = wcount+2*(nin+nout);
        rvectorsetlengthatleast(p, *pcount, _state);
        k = 0;
        for(i=0; i<=wcount-1; i++)
        {
            p->ptr.p_double[k] = network->weights.ptr.p_double[i];
            k = k+1;
        }
        for(i=0; i<=nin+nout-1; i++)
        {
            p->ptr.p_double[k] = network->columnmeans.ptr.p_double[i];
            k = k+1;
            p->ptr.p_double[k] = network->columnsigmas.ptr.p_double[i];
            k = k+1;
        }
    }
}


/*************************************************************************
This  function imports  tunable   parameters  (weights/means/sigmas) which
were exported by MLPExportTunableParameters().

It is intended for fast copying of states between network and backup array

INPUT PARAMETERS:
    Network     -   target:
                    * must be correctly initialized
                    * must have same geometry as network used to export params
    P           -   array with parameters

  -- ALGLIB --
     Copyright 20.06.2013 by Bochkanov Sergey
*************************************************************************/
void mlpimporttunableparameters(multilayerperceptron* network,
     /* Real    */ ae_vector* p,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;


    ae_assert(network->structinfo.cnt>0&&network->structinfo.cnt>=network->structinfo.ptr.p_int[0], "MLPImportTunableParameters: Network is uninitialized", _state);
    mlpproperties(network, &nin, &nout, &wcount, _state);
    if( mlpissoftmax(network, _state) )
    {
        k = 0;
        for(i=0; i<=wcount-1; i++)
        {
            network->weights.ptr.p_double[i] = p->ptr.p_double[k];
            k = k+1;
        }
        for(i=0; i<=nin-1; i++)
        {
            network->columnmeans.ptr.p_double[i] = p->ptr.p_double[k];
            k = k+1;
            network->columnsigmas.ptr.p_double[i] = p->ptr.p_double[k];
            k = k+1;
        }
    }
    else
    {
        k = 0;
        for(i=0; i<=wcount-1; i++)
        {
            network->weights.ptr.p_double[i] = p->ptr.p_double[k];
            k = k+1;
        }
        for(i=0; i<=nin+nout-1; i++)
        {
            network->columnmeans.ptr.p_double[i] = p->ptr.p_double[k];
            k = k+1;
            network->columnsigmas.ptr.p_double[i] = p->ptr.p_double[k];
            k = k+1;
        }
    }
}


/*************************************************************************
Serialization of MultiLayerPerceptron strucure

INPUT PARAMETERS:
    Network -   original

OUTPUT PARAMETERS:
    RA      -   array of real numbers which stores network,
                array[0..RLen-1]
    RLen    -   RA lenght

  -- ALGLIB --
     Copyright 29.03.2008 by Bochkanov Sergey
*************************************************************************/
void mlpserializeold(multilayerperceptron* network,
     /* Real    */ ae_vector* ra,
     ae_int_t* rlen,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t ssize;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t sigmalen;
    ae_int_t offs;

    ae_vector_clear(ra);
    *rlen = 0;

    
    /*
     * Unload info
     */
    ssize = network->structinfo.ptr.p_int[0];
    nin = network->structinfo.ptr.p_int[1];
    nout = network->structinfo.ptr.p_int[2];
    wcount = network->structinfo.ptr.p_int[4];
    if( mlpissoftmax(network, _state) )
    {
        sigmalen = nin;
    }
    else
    {
        sigmalen = nin+nout;
    }
    
    /*
     *  RA format:
     *      LEN         DESRC.
     *      1           RLen
     *      1           version (MLPVNum)
     *      1           StructInfo size
     *      SSize       StructInfo
     *      WCount      Weights
     *      SigmaLen    ColumnMeans
     *      SigmaLen    ColumnSigmas
     */
    *rlen = 3+ssize+wcount+2*sigmalen;
    ae_vector_set_length(ra, *rlen-1+1, _state);
    ra->ptr.p_double[0] = (double)(*rlen);
    ra->ptr.p_double[1] = (double)(mlpbase_mlpvnum);
    ra->ptr.p_double[2] = (double)(ssize);
    offs = 3;
    for(i=0; i<=ssize-1; i++)
    {
        ra->ptr.p_double[offs+i] = (double)(network->structinfo.ptr.p_int[i]);
    }
    offs = offs+ssize;
    ae_v_move(&ra->ptr.p_double[offs], 1, &network->weights.ptr.p_double[0], 1, ae_v_len(offs,offs+wcount-1));
    offs = offs+wcount;
    ae_v_move(&ra->ptr.p_double[offs], 1, &network->columnmeans.ptr.p_double[0], 1, ae_v_len(offs,offs+sigmalen-1));
    offs = offs+sigmalen;
    ae_v_move(&ra->ptr.p_double[offs], 1, &network->columnsigmas.ptr.p_double[0], 1, ae_v_len(offs,offs+sigmalen-1));
    offs = offs+sigmalen;
}


/*************************************************************************
Unserialization of MultiLayerPerceptron strucure

INPUT PARAMETERS:
    RA      -   real array which stores network

OUTPUT PARAMETERS:
    Network -   restored network

  -- ALGLIB --
     Copyright 29.03.2008 by Bochkanov Sergey
*************************************************************************/
void mlpunserializeold(/* Real    */ ae_vector* ra,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t ssize;
    ae_int_t ntotal;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t sigmalen;
    ae_int_t offs;

    _multilayerperceptron_clear(network);

    ae_assert(ae_round(ra->ptr.p_double[1], _state)==mlpbase_mlpvnum, "MLPUnserialize: incorrect array!", _state);
    
    /*
     * Unload StructInfo from IA
     */
    offs = 3;
    ssize = ae_round(ra->ptr.p_double[2], _state);
    ae_vector_set_length(&network->structinfo, ssize-1+1, _state);
    for(i=0; i<=ssize-1; i++)
    {
        network->structinfo.ptr.p_int[i] = ae_round(ra->ptr.p_double[offs+i], _state);
    }
    offs = offs+ssize;
    
    /*
     * Unload info from StructInfo
     */
    ssize = network->structinfo.ptr.p_int[0];
    nin = network->structinfo.ptr.p_int[1];
    nout = network->structinfo.ptr.p_int[2];
    ntotal = network->structinfo.ptr.p_int[3];
    wcount = network->structinfo.ptr.p_int[4];
    if( network->structinfo.ptr.p_int[6]==0 )
    {
        sigmalen = nin+nout;
    }
    else
    {
        sigmalen = nin;
    }
    
    /*
     * Allocate space for other fields
     */
    ae_vector_set_length(&network->weights, wcount-1+1, _state);
    ae_vector_set_length(&network->columnmeans, sigmalen-1+1, _state);
    ae_vector_set_length(&network->columnsigmas, sigmalen-1+1, _state);
    ae_vector_set_length(&network->neurons, ntotal-1+1, _state);
    ae_vector_set_length(&network->nwbuf, ae_maxint(wcount, 2*nout, _state)-1+1, _state);
    ae_vector_set_length(&network->dfdnet, ntotal-1+1, _state);
    ae_vector_set_length(&network->x, nin-1+1, _state);
    ae_vector_set_length(&network->y, nout-1+1, _state);
    ae_vector_set_length(&network->derror, ntotal-1+1, _state);
    
    /*
     * Copy parameters from RA
     */
    ae_v_move(&network->weights.ptr.p_double[0], 1, &ra->ptr.p_double[offs], 1, ae_v_len(0,wcount-1));
    offs = offs+wcount;
    ae_v_move(&network->columnmeans.ptr.p_double[0], 1, &ra->ptr.p_double[offs], 1, ae_v_len(0,sigmalen-1));
    offs = offs+sigmalen;
    ae_v_move(&network->columnsigmas.ptr.p_double[0], 1, &ra->ptr.p_double[offs], 1, ae_v_len(0,sigmalen-1));
    offs = offs+sigmalen;
}


/*************************************************************************
Randomization of neural network weights

  -- ALGLIB --
     Copyright 06.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlprandomize(multilayerperceptron* network, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t ntotal;
    ae_int_t istart;
    hqrndstate r;
    ae_int_t entrysize;
    ae_int_t entryoffs;
    ae_int_t neuronidx;
    ae_int_t neurontype;
    double vmean;
    double vvar;
    ae_int_t i;
    ae_int_t n1;
    ae_int_t n2;
    double desiredsigma;
    ae_int_t montecarlocnt;
    double ef;
    double ef2;
    double v;
    double wscale;

    ae_frame_make(_state, &_frame_block);
    _hqrndstate_init(&r, _state);

    hqrndrandomize(&r, _state);
    mlpproperties(network, &nin, &nout, &wcount, _state);
    ntotal = network->structinfo.ptr.p_int[3];
    istart = network->structinfo.ptr.p_int[5];
    desiredsigma = 0.5;
    montecarlocnt = 20;
    
    /*
     * Stage 1:
     * * Network.Weights is filled by standard deviation of weights
     * * default values: sigma=1
     */
    for(i=0; i<=wcount-1; i++)
    {
        network->weights.ptr.p_double[i] = 1.0;
    }
    
    /*
     * Stage 2:
     * * assume that input neurons have zero mean and unit standard deviation
     * * assume that constant neurons have zero standard deviation
     * * perform forward pass along neurons
     * * for each non-input non-constant neuron:
     *   * calculate mean and standard deviation of neuron's output
     *     assuming that we know means/deviations of neurons which feed it
     *     and assuming that weights has unit variance and zero mean.
     * * for each nonlinear neuron additionally we perform backward pass:
     *   * scale variances of weights which feed it in such way that neuron's
     *     input has unit standard deviation
     *
     * NOTE: this algorithm assumes that each connection feeds at most one
     *       non-linear neuron. This assumption can be incorrect in upcoming
     *       architectures with strong neurons. However, algorithm should
     *       work smoothly even in this case.
     *
     * During this stage we use Network.RndBuf, which is grouped into NTotal
     * entries, each of them having following format:
     *
     * Buf[Offset+0]        mean value of neuron's output
     * Buf[Offset+1]        standard deviation of neuron's output
     * 
     *
     */
    entrysize = 2;
    rvectorsetlengthatleast(&network->rndbuf, entrysize*ntotal, _state);
    for(neuronidx=0; neuronidx<=ntotal-1; neuronidx++)
    {
        neurontype = network->structinfo.ptr.p_int[istart+neuronidx*mlpbase_nfieldwidth+0];
        entryoffs = entrysize*neuronidx;
        if( neurontype==-2 )
        {
            
            /*
             * Input neuron: zero mean, unit variance.
             */
            network->rndbuf.ptr.p_double[entryoffs+0] = 0.0;
            network->rndbuf.ptr.p_double[entryoffs+1] = 1.0;
            continue;
        }
        if( neurontype==-3 )
        {
            
            /*
             * "-1" neuron: mean=-1, zero variance.
             */
            network->rndbuf.ptr.p_double[entryoffs+0] = -1.0;
            network->rndbuf.ptr.p_double[entryoffs+1] = 0.0;
            continue;
        }
        if( neurontype==-4 )
        {
            
            /*
             * "0" neuron: mean=0, zero variance.
             */
            network->rndbuf.ptr.p_double[entryoffs+0] = 0.0;
            network->rndbuf.ptr.p_double[entryoffs+1] = 0.0;
            continue;
        }
        if( neurontype==0 )
        {
            
            /*
             * Adaptive summator neuron:
             * * calculate its mean and variance.
             * * we assume that weights of this neuron have unit variance and zero mean.
             * * thus, neuron's output is always have zero mean
             * * as for variance, it is a bit more interesting:
             *   * let n[i] is i-th input neuron
             *   * let w[i] is i-th weight
             *   * we assume that n[i] and w[i] are independently distributed
             *   * Var(n0*w0+n1*w1+...) = Var(n0*w0)+Var(n1*w1)+...
             *   * Var(X*Y) = mean(X)^2*Var(Y) + mean(Y)^2*Var(X) + Var(X)*Var(Y)
             *   * mean(w[i])=0, var(w[i])=1
             *   * Var(n[i]*w[i]) = mean(n[i])^2 + Var(n[i])
             */
            n1 = network->structinfo.ptr.p_int[istart+neuronidx*mlpbase_nfieldwidth+2];
            n2 = n1+network->structinfo.ptr.p_int[istart+neuronidx*mlpbase_nfieldwidth+1]-1;
            vmean = 0.0;
            vvar = 0.0;
            for(i=n1; i<=n2; i++)
            {
                vvar = vvar+ae_sqr(network->rndbuf.ptr.p_double[entrysize*i+0], _state)+ae_sqr(network->rndbuf.ptr.p_double[entrysize*i+1], _state);
            }
            network->rndbuf.ptr.p_double[entryoffs+0] = vmean;
            network->rndbuf.ptr.p_double[entryoffs+1] = ae_sqrt(vvar, _state);
            continue;
        }
        if( neurontype==-5 )
        {
            
            /*
             * Linear activation function
             */
            i = network->structinfo.ptr.p_int[istart+neuronidx*mlpbase_nfieldwidth+2];
            vmean = network->rndbuf.ptr.p_double[entrysize*i+0];
            vvar = ae_sqr(network->rndbuf.ptr.p_double[entrysize*i+1], _state);
            if( ae_fp_greater(vvar,(double)(0)) )
            {
                wscale = desiredsigma/ae_sqrt(vvar, _state);
            }
            else
            {
                wscale = 1.0;
            }
            mlpbase_randomizebackwardpass(network, i, wscale, _state);
            network->rndbuf.ptr.p_double[entryoffs+0] = vmean*wscale;
            network->rndbuf.ptr.p_double[entryoffs+1] = desiredsigma;
            continue;
        }
        if( neurontype>0 )
        {
            
            /*
             * Nonlinear activation function:
             * * scale its inputs
             * * estimate mean/sigma of its output using Monte-Carlo method
             *   (we simulate different inputs with unit deviation and
             *   sample activation function output on such inputs)
             */
            i = network->structinfo.ptr.p_int[istart+neuronidx*mlpbase_nfieldwidth+2];
            vmean = network->rndbuf.ptr.p_double[entrysize*i+0];
            vvar = ae_sqr(network->rndbuf.ptr.p_double[entrysize*i+1], _state);
            if( ae_fp_greater(vvar,(double)(0)) )
            {
                wscale = desiredsigma/ae_sqrt(vvar, _state);
            }
            else
            {
                wscale = 1.0;
            }
            mlpbase_randomizebackwardpass(network, i, wscale, _state);
            ef = 0.0;
            ef2 = 0.0;
            vmean = vmean*wscale;
            for(i=0; i<=montecarlocnt-1; i++)
            {
                v = vmean+desiredsigma*hqrndnormal(&r, _state);
                ef = ef+v;
                ef2 = ef2+v*v;
            }
            ef = ef/montecarlocnt;
            ef2 = ef2/montecarlocnt;
            network->rndbuf.ptr.p_double[entryoffs+0] = ef;
            network->rndbuf.ptr.p_double[entryoffs+1] = ae_maxreal(ef2-ef*ef, 0.0, _state);
            continue;
        }
        ae_assert(ae_false, "MLPRandomize: unexpected neuron type", _state);
    }
    
    /*
     * Stage 3: generate weights.
     */
    for(i=0; i<=wcount-1; i++)
    {
        network->weights.ptr.p_double[i] = network->weights.ptr.p_double[i]*hqrndnormal(&r, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Randomization of neural network weights and standartisator

  -- ALGLIB --
     Copyright 10.03.2008 by Bochkanov Sergey
*************************************************************************/
void mlprandomizefull(multilayerperceptron* network, ae_state *_state)
{
    ae_int_t i;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t ntotal;
    ae_int_t istart;
    ae_int_t offs;
    ae_int_t ntype;


    mlpproperties(network, &nin, &nout, &wcount, _state);
    ntotal = network->structinfo.ptr.p_int[3];
    istart = network->structinfo.ptr.p_int[5];
    
    /*
     * Process network
     */
    mlprandomize(network, _state);
    for(i=0; i<=nin-1; i++)
    {
        network->columnmeans.ptr.p_double[i] = ae_randomreal(_state)-0.5;
        network->columnsigmas.ptr.p_double[i] = ae_randomreal(_state)+0.5;
    }
    if( !mlpissoftmax(network, _state) )
    {
        for(i=0; i<=nout-1; i++)
        {
            offs = istart+(ntotal-nout+i)*mlpbase_nfieldwidth;
            ntype = network->structinfo.ptr.p_int[offs+0];
            if( ntype==0 )
            {
                
                /*
                 * Shifts are changed only for linear outputs neurons
                 */
                network->columnmeans.ptr.p_double[nin+i] = 2*ae_randomreal(_state)-1;
            }
            if( ntype==0||ntype==3 )
            {
                
                /*
                 * Scales are changed only for linear or bounded outputs neurons.
                 * Note that scale randomization preserves sign.
                 */
                network->columnsigmas.ptr.p_double[nin+i] = ae_sign(network->columnsigmas.ptr.p_double[nin+i], _state)*(1.5*ae_randomreal(_state)+0.5);
            }
        }
    }
}


/*************************************************************************
Internal subroutine.

  -- ALGLIB --
     Copyright 30.03.2008 by Bochkanov Sergey
*************************************************************************/
void mlpinitpreprocessor(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t ssize,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t jmax;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t ntotal;
    ae_int_t istart;
    ae_int_t offs;
    ae_int_t ntype;
    ae_vector means;
    ae_vector sigmas;
    double s;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&means, 0, DT_REAL, _state);
    ae_vector_init(&sigmas, 0, DT_REAL, _state);

    mlpproperties(network, &nin, &nout, &wcount, _state);
    ntotal = network->structinfo.ptr.p_int[3];
    istart = network->structinfo.ptr.p_int[5];
    
    /*
     * Means/Sigmas
     */
    if( mlpissoftmax(network, _state) )
    {
        jmax = nin-1;
    }
    else
    {
        jmax = nin+nout-1;
    }
    ae_vector_set_length(&means, jmax+1, _state);
    ae_vector_set_length(&sigmas, jmax+1, _state);
    for(i=0; i<=jmax; i++)
    {
        means.ptr.p_double[i] = (double)(0);
        sigmas.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=ssize-1; i++)
    {
        for(j=0; j<=jmax; j++)
        {
            means.ptr.p_double[j] = means.ptr.p_double[j]+xy->ptr.pp_double[i][j];
        }
    }
    for(i=0; i<=jmax; i++)
    {
        means.ptr.p_double[i] = means.ptr.p_double[i]/ssize;
    }
    for(i=0; i<=ssize-1; i++)
    {
        for(j=0; j<=jmax; j++)
        {
            sigmas.ptr.p_double[j] = sigmas.ptr.p_double[j]+ae_sqr(xy->ptr.pp_double[i][j]-means.ptr.p_double[j], _state);
        }
    }
    for(i=0; i<=jmax; i++)
    {
        sigmas.ptr.p_double[i] = ae_sqrt(sigmas.ptr.p_double[i]/ssize, _state);
    }
    
    /*
     * Inputs
     */
    for(i=0; i<=nin-1; i++)
    {
        network->columnmeans.ptr.p_double[i] = means.ptr.p_double[i];
        network->columnsigmas.ptr.p_double[i] = sigmas.ptr.p_double[i];
        if( ae_fp_eq(network->columnsigmas.ptr.p_double[i],(double)(0)) )
        {
            network->columnsigmas.ptr.p_double[i] = (double)(1);
        }
    }
    
    /*
     * Outputs
     */
    if( !mlpissoftmax(network, _state) )
    {
        for(i=0; i<=nout-1; i++)
        {
            offs = istart+(ntotal-nout+i)*mlpbase_nfieldwidth;
            ntype = network->structinfo.ptr.p_int[offs+0];
            
            /*
             * Linear outputs
             */
            if( ntype==0 )
            {
                network->columnmeans.ptr.p_double[nin+i] = means.ptr.p_double[nin+i];
                network->columnsigmas.ptr.p_double[nin+i] = sigmas.ptr.p_double[nin+i];
                if( ae_fp_eq(network->columnsigmas.ptr.p_double[nin+i],(double)(0)) )
                {
                    network->columnsigmas.ptr.p_double[nin+i] = (double)(1);
                }
            }
            
            /*
             * Bounded outputs (half-interval)
             */
            if( ntype==3 )
            {
                s = means.ptr.p_double[nin+i]-network->columnmeans.ptr.p_double[nin+i];
                if( ae_fp_eq(s,(double)(0)) )
                {
                    s = (double)(ae_sign(network->columnsigmas.ptr.p_double[nin+i], _state));
                }
                if( ae_fp_eq(s,(double)(0)) )
                {
                    s = 1.0;
                }
                network->columnsigmas.ptr.p_double[nin+i] = ae_sign(network->columnsigmas.ptr.p_double[nin+i], _state)*ae_fabs(s, _state);
                if( ae_fp_eq(network->columnsigmas.ptr.p_double[nin+i],(double)(0)) )
                {
                    network->columnsigmas.ptr.p_double[nin+i] = (double)(1);
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine.
Initialization for preprocessor based on a sample.

INPUT
    Network -   initialized neural network;
    XY      -   sample, given by sparse matrix;
    SSize   -   sample size.

OUTPUT
    Network -   neural network with initialised preprocessor.

  -- ALGLIB --
     Copyright 26.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlpinitpreprocessorsparse(multilayerperceptron* network,
     sparsematrix* xy,
     ae_int_t ssize,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t jmax;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t ntotal;
    ae_int_t istart;
    ae_int_t offs;
    ae_int_t ntype;
    ae_vector means;
    ae_vector sigmas;
    double s;
    ae_int_t i;
    ae_int_t j;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&means, 0, DT_REAL, _state);
    ae_vector_init(&sigmas, 0, DT_REAL, _state);

    mlpproperties(network, &nin, &nout, &wcount, _state);
    ntotal = network->structinfo.ptr.p_int[3];
    istart = network->structinfo.ptr.p_int[5];
    
    /*
     * Means/Sigmas
     */
    if( mlpissoftmax(network, _state) )
    {
        jmax = nin-1;
    }
    else
    {
        jmax = nin+nout-1;
    }
    ae_vector_set_length(&means, jmax+1, _state);
    ae_vector_set_length(&sigmas, jmax+1, _state);
    for(i=0; i<=jmax; i++)
    {
        means.ptr.p_double[i] = (double)(0);
        sigmas.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=ssize-1; i++)
    {
        sparsegetrow(xy, i, &network->xyrow, _state);
        for(j=0; j<=jmax; j++)
        {
            means.ptr.p_double[j] = means.ptr.p_double[j]+network->xyrow.ptr.p_double[j];
        }
    }
    for(i=0; i<=jmax; i++)
    {
        means.ptr.p_double[i] = means.ptr.p_double[i]/ssize;
    }
    for(i=0; i<=ssize-1; i++)
    {
        sparsegetrow(xy, i, &network->xyrow, _state);
        for(j=0; j<=jmax; j++)
        {
            sigmas.ptr.p_double[j] = sigmas.ptr.p_double[j]+ae_sqr(network->xyrow.ptr.p_double[j]-means.ptr.p_double[j], _state);
        }
    }
    for(i=0; i<=jmax; i++)
    {
        sigmas.ptr.p_double[i] = ae_sqrt(sigmas.ptr.p_double[i]/ssize, _state);
    }
    
    /*
     * Inputs
     */
    for(i=0; i<=nin-1; i++)
    {
        network->columnmeans.ptr.p_double[i] = means.ptr.p_double[i];
        network->columnsigmas.ptr.p_double[i] = sigmas.ptr.p_double[i];
        if( ae_fp_eq(network->columnsigmas.ptr.p_double[i],(double)(0)) )
        {
            network->columnsigmas.ptr.p_double[i] = (double)(1);
        }
    }
    
    /*
     * Outputs
     */
    if( !mlpissoftmax(network, _state) )
    {
        for(i=0; i<=nout-1; i++)
        {
            offs = istart+(ntotal-nout+i)*mlpbase_nfieldwidth;
            ntype = network->structinfo.ptr.p_int[offs+0];
            
            /*
             * Linear outputs
             */
            if( ntype==0 )
            {
                network->columnmeans.ptr.p_double[nin+i] = means.ptr.p_double[nin+i];
                network->columnsigmas.ptr.p_double[nin+i] = sigmas.ptr.p_double[nin+i];
                if( ae_fp_eq(network->columnsigmas.ptr.p_double[nin+i],(double)(0)) )
                {
                    network->columnsigmas.ptr.p_double[nin+i] = (double)(1);
                }
            }
            
            /*
             * Bounded outputs (half-interval)
             */
            if( ntype==3 )
            {
                s = means.ptr.p_double[nin+i]-network->columnmeans.ptr.p_double[nin+i];
                if( ae_fp_eq(s,(double)(0)) )
                {
                    s = (double)(ae_sign(network->columnsigmas.ptr.p_double[nin+i], _state));
                }
                if( ae_fp_eq(s,(double)(0)) )
                {
                    s = 1.0;
                }
                network->columnsigmas.ptr.p_double[nin+i] = ae_sign(network->columnsigmas.ptr.p_double[nin+i], _state)*ae_fabs(s, _state);
                if( ae_fp_eq(network->columnsigmas.ptr.p_double[nin+i],(double)(0)) )
                {
                    network->columnsigmas.ptr.p_double[nin+i] = (double)(1);
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine.
Initialization for preprocessor based on a subsample.

INPUT PARAMETERS:
    Network -   network initialized with one of the network creation funcs
    XY      -   original dataset; one sample = one row;
                first NIn columns contain inputs,
                next NOut columns - desired outputs.
    SetSize -   real size of XY, SetSize>=0;
    Idx     -   subset of SubsetSize elements, array[SubsetSize]:
                * Idx[I] stores row index in the original dataset which is
                  given by XY. Gradient is calculated with respect to rows
                  whose indexes are stored in Idx[].
                * Idx[]  must store correct indexes; this function  throws
                  an  exception  in  case  incorrect index (less than 0 or
                  larger than rows(XY)) is given
                * Idx[]  may  store  indexes  in  any  order and even with
                  repetitions.
    SubsetSize- number of elements in Idx[] array.

OUTPUT:
    Network -   neural network with initialised preprocessor.
    
NOTE: when  SubsetSize<0 is used full dataset by call MLPInitPreprocessor
      function.

  -- ALGLIB --
     Copyright 23.08.2012 by Bochkanov Sergey
*************************************************************************/
void mlpinitpreprocessorsubset(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t setsize,
     /* Integer */ ae_vector* idx,
     ae_int_t subsetsize,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t jmax;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t ntotal;
    ae_int_t istart;
    ae_int_t offs;
    ae_int_t ntype;
    ae_vector means;
    ae_vector sigmas;
    double s;
    ae_int_t npoints;
    ae_int_t i;
    ae_int_t j;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&means, 0, DT_REAL, _state);
    ae_vector_init(&sigmas, 0, DT_REAL, _state);

    ae_assert(setsize>=0, "MLPInitPreprocessorSubset: SetSize<0", _state);
    if( subsetsize<0 )
    {
        mlpinitpreprocessor(network, xy, setsize, _state);
        ae_frame_leave(_state);
        return;
    }
    ae_assert(subsetsize<=idx->cnt, "MLPInitPreprocessorSubset: SubsetSize>Length(Idx)", _state);
    npoints = setsize;
    for(i=0; i<=subsetsize-1; i++)
    {
        ae_assert(idx->ptr.p_int[i]>=0, "MLPInitPreprocessorSubset: incorrect index of XY row(Idx[I]<0)", _state);
        ae_assert(idx->ptr.p_int[i]<=npoints-1, "MLPInitPreprocessorSubset: incorrect index of XY row(Idx[I]>Rows(XY)-1)", _state);
    }
    mlpproperties(network, &nin, &nout, &wcount, _state);
    ntotal = network->structinfo.ptr.p_int[3];
    istart = network->structinfo.ptr.p_int[5];
    
    /*
     * Means/Sigmas
     */
    if( mlpissoftmax(network, _state) )
    {
        jmax = nin-1;
    }
    else
    {
        jmax = nin+nout-1;
    }
    ae_vector_set_length(&means, jmax+1, _state);
    ae_vector_set_length(&sigmas, jmax+1, _state);
    for(i=0; i<=jmax; i++)
    {
        means.ptr.p_double[i] = (double)(0);
        sigmas.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=subsetsize-1; i++)
    {
        for(j=0; j<=jmax; j++)
        {
            means.ptr.p_double[j] = means.ptr.p_double[j]+xy->ptr.pp_double[idx->ptr.p_int[i]][j];
        }
    }
    for(i=0; i<=jmax; i++)
    {
        means.ptr.p_double[i] = means.ptr.p_double[i]/subsetsize;
    }
    for(i=0; i<=subsetsize-1; i++)
    {
        for(j=0; j<=jmax; j++)
        {
            sigmas.ptr.p_double[j] = sigmas.ptr.p_double[j]+ae_sqr(xy->ptr.pp_double[idx->ptr.p_int[i]][j]-means.ptr.p_double[j], _state);
        }
    }
    for(i=0; i<=jmax; i++)
    {
        sigmas.ptr.p_double[i] = ae_sqrt(sigmas.ptr.p_double[i]/subsetsize, _state);
    }
    
    /*
     * Inputs
     */
    for(i=0; i<=nin-1; i++)
    {
        network->columnmeans.ptr.p_double[i] = means.ptr.p_double[i];
        network->columnsigmas.ptr.p_double[i] = sigmas.ptr.p_double[i];
        if( ae_fp_eq(network->columnsigmas.ptr.p_double[i],(double)(0)) )
        {
            network->columnsigmas.ptr.p_double[i] = (double)(1);
        }
    }
    
    /*
     * Outputs
     */
    if( !mlpissoftmax(network, _state) )
    {
        for(i=0; i<=nout-1; i++)
        {
            offs = istart+(ntotal-nout+i)*mlpbase_nfieldwidth;
            ntype = network->structinfo.ptr.p_int[offs+0];
            
            /*
             * Linear outputs
             */
            if( ntype==0 )
            {
                network->columnmeans.ptr.p_double[nin+i] = means.ptr.p_double[nin+i];
                network->columnsigmas.ptr.p_double[nin+i] = sigmas.ptr.p_double[nin+i];
                if( ae_fp_eq(network->columnsigmas.ptr.p_double[nin+i],(double)(0)) )
                {
                    network->columnsigmas.ptr.p_double[nin+i] = (double)(1);
                }
            }
            
            /*
             * Bounded outputs (half-interval)
             */
            if( ntype==3 )
            {
                s = means.ptr.p_double[nin+i]-network->columnmeans.ptr.p_double[nin+i];
                if( ae_fp_eq(s,(double)(0)) )
                {
                    s = (double)(ae_sign(network->columnsigmas.ptr.p_double[nin+i], _state));
                }
                if( ae_fp_eq(s,(double)(0)) )
                {
                    s = 1.0;
                }
                network->columnsigmas.ptr.p_double[nin+i] = ae_sign(network->columnsigmas.ptr.p_double[nin+i], _state)*ae_fabs(s, _state);
                if( ae_fp_eq(network->columnsigmas.ptr.p_double[nin+i],(double)(0)) )
                {
                    network->columnsigmas.ptr.p_double[nin+i] = (double)(1);
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine.
Initialization for preprocessor based on a subsample.

INPUT PARAMETERS:
    Network -   network initialized with one of the network creation funcs
    XY      -   original dataset, given by sparse matrix;
                one sample = one row;
                first NIn columns contain inputs,
                next NOut columns - desired outputs.
    SetSize -   real size of XY, SetSize>=0;
    Idx     -   subset of SubsetSize elements, array[SubsetSize]:
                * Idx[I] stores row index in the original dataset which is
                  given by XY. Gradient is calculated with respect to rows
                  whose indexes are stored in Idx[].
                * Idx[]  must store correct indexes; this function  throws
                  an  exception  in  case  incorrect index (less than 0 or
                  larger than rows(XY)) is given
                * Idx[]  may  store  indexes  in  any  order and even with
                  repetitions.
    SubsetSize- number of elements in Idx[] array.
    
OUTPUT:
    Network -   neural network with initialised preprocessor.
    
NOTE: when SubsetSize<0 is used full dataset by call
      MLPInitPreprocessorSparse function.
      
  -- ALGLIB --
     Copyright 26.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlpinitpreprocessorsparsesubset(multilayerperceptron* network,
     sparsematrix* xy,
     ae_int_t setsize,
     /* Integer */ ae_vector* idx,
     ae_int_t subsetsize,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t jmax;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t ntotal;
    ae_int_t istart;
    ae_int_t offs;
    ae_int_t ntype;
    ae_vector means;
    ae_vector sigmas;
    double s;
    ae_int_t npoints;
    ae_int_t i;
    ae_int_t j;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&means, 0, DT_REAL, _state);
    ae_vector_init(&sigmas, 0, DT_REAL, _state);

    ae_assert(setsize>=0, "MLPInitPreprocessorSparseSubset: SetSize<0", _state);
    if( subsetsize<0 )
    {
        mlpinitpreprocessorsparse(network, xy, setsize, _state);
        ae_frame_leave(_state);
        return;
    }
    ae_assert(subsetsize<=idx->cnt, "MLPInitPreprocessorSparseSubset: SubsetSize>Length(Idx)", _state);
    npoints = setsize;
    for(i=0; i<=subsetsize-1; i++)
    {
        ae_assert(idx->ptr.p_int[i]>=0, "MLPInitPreprocessorSparseSubset: incorrect index of XY row(Idx[I]<0)", _state);
        ae_assert(idx->ptr.p_int[i]<=npoints-1, "MLPInitPreprocessorSparseSubset: incorrect index of XY row(Idx[I]>Rows(XY)-1)", _state);
    }
    mlpproperties(network, &nin, &nout, &wcount, _state);
    ntotal = network->structinfo.ptr.p_int[3];
    istart = network->structinfo.ptr.p_int[5];
    
    /*
     * Means/Sigmas
     */
    if( mlpissoftmax(network, _state) )
    {
        jmax = nin-1;
    }
    else
    {
        jmax = nin+nout-1;
    }
    ae_vector_set_length(&means, jmax+1, _state);
    ae_vector_set_length(&sigmas, jmax+1, _state);
    for(i=0; i<=jmax; i++)
    {
        means.ptr.p_double[i] = (double)(0);
        sigmas.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=subsetsize-1; i++)
    {
        sparsegetrow(xy, idx->ptr.p_int[i], &network->xyrow, _state);
        for(j=0; j<=jmax; j++)
        {
            means.ptr.p_double[j] = means.ptr.p_double[j]+network->xyrow.ptr.p_double[j];
        }
    }
    for(i=0; i<=jmax; i++)
    {
        means.ptr.p_double[i] = means.ptr.p_double[i]/subsetsize;
    }
    for(i=0; i<=subsetsize-1; i++)
    {
        sparsegetrow(xy, idx->ptr.p_int[i], &network->xyrow, _state);
        for(j=0; j<=jmax; j++)
        {
            sigmas.ptr.p_double[j] = sigmas.ptr.p_double[j]+ae_sqr(network->xyrow.ptr.p_double[j]-means.ptr.p_double[j], _state);
        }
    }
    for(i=0; i<=jmax; i++)
    {
        sigmas.ptr.p_double[i] = ae_sqrt(sigmas.ptr.p_double[i]/subsetsize, _state);
    }
    
    /*
     * Inputs
     */
    for(i=0; i<=nin-1; i++)
    {
        network->columnmeans.ptr.p_double[i] = means.ptr.p_double[i];
        network->columnsigmas.ptr.p_double[i] = sigmas.ptr.p_double[i];
        if( ae_fp_eq(network->columnsigmas.ptr.p_double[i],(double)(0)) )
        {
            network->columnsigmas.ptr.p_double[i] = (double)(1);
        }
    }
    
    /*
     * Outputs
     */
    if( !mlpissoftmax(network, _state) )
    {
        for(i=0; i<=nout-1; i++)
        {
            offs = istart+(ntotal-nout+i)*mlpbase_nfieldwidth;
            ntype = network->structinfo.ptr.p_int[offs+0];
            
            /*
             * Linear outputs
             */
            if( ntype==0 )
            {
                network->columnmeans.ptr.p_double[nin+i] = means.ptr.p_double[nin+i];
                network->columnsigmas.ptr.p_double[nin+i] = sigmas.ptr.p_double[nin+i];
                if( ae_fp_eq(network->columnsigmas.ptr.p_double[nin+i],(double)(0)) )
                {
                    network->columnsigmas.ptr.p_double[nin+i] = (double)(1);
                }
            }
            
            /*
             * Bounded outputs (half-interval)
             */
            if( ntype==3 )
            {
                s = means.ptr.p_double[nin+i]-network->columnmeans.ptr.p_double[nin+i];
                if( ae_fp_eq(s,(double)(0)) )
                {
                    s = (double)(ae_sign(network->columnsigmas.ptr.p_double[nin+i], _state));
                }
                if( ae_fp_eq(s,(double)(0)) )
                {
                    s = 1.0;
                }
                network->columnsigmas.ptr.p_double[nin+i] = ae_sign(network->columnsigmas.ptr.p_double[nin+i], _state)*ae_fabs(s, _state);
                if( ae_fp_eq(network->columnsigmas.ptr.p_double[nin+i],(double)(0)) )
                {
                    network->columnsigmas.ptr.p_double[nin+i] = (double)(1);
                }
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Returns information about initialized network: number of inputs, outputs,
weights.

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlpproperties(multilayerperceptron* network,
     ae_int_t* nin,
     ae_int_t* nout,
     ae_int_t* wcount,
     ae_state *_state)
{

    *nin = 0;
    *nout = 0;
    *wcount = 0;

    *nin = network->structinfo.ptr.p_int[1];
    *nout = network->structinfo.ptr.p_int[2];
    *wcount = network->structinfo.ptr.p_int[4];
}


/*************************************************************************
Returns number of "internal", low-level neurons in the network (one  which
is stored in StructInfo).

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
ae_int_t mlpntotal(multilayerperceptron* network, ae_state *_state)
{
    ae_int_t result;


    result = network->structinfo.ptr.p_int[3];
    return result;
}


/*************************************************************************
Returns number of inputs.

  -- ALGLIB --
     Copyright 19.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_int_t mlpgetinputscount(multilayerperceptron* network,
     ae_state *_state)
{
    ae_int_t result;


    result = network->structinfo.ptr.p_int[1];
    return result;
}


/*************************************************************************
Returns number of outputs.

  -- ALGLIB --
     Copyright 19.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_int_t mlpgetoutputscount(multilayerperceptron* network,
     ae_state *_state)
{
    ae_int_t result;


    result = network->structinfo.ptr.p_int[2];
    return result;
}


/*************************************************************************
Returns number of weights.

  -- ALGLIB --
     Copyright 19.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_int_t mlpgetweightscount(multilayerperceptron* network,
     ae_state *_state)
{
    ae_int_t result;


    result = network->structinfo.ptr.p_int[4];
    return result;
}


/*************************************************************************
Tells whether network is SOFTMAX-normalized (i.e. classifier) or not.

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
ae_bool mlpissoftmax(multilayerperceptron* network, ae_state *_state)
{
    ae_bool result;


    result = network->structinfo.ptr.p_int[6]==1;
    return result;
}


/*************************************************************************
This function returns total number of layers (including input, hidden and
output layers).

  -- ALGLIB --
     Copyright 25.03.2011 by Bochkanov Sergey
*************************************************************************/
ae_int_t mlpgetlayerscount(multilayerperceptron* network,
     ae_state *_state)
{
    ae_int_t result;


    result = network->hllayersizes.cnt;
    return result;
}


/*************************************************************************
This function returns size of K-th layer.

K=0 corresponds to input layer, K=CNT-1 corresponds to output layer.

Size of the output layer is always equal to the number of outputs, although
when we have softmax-normalized network, last neuron doesn't have any
connections - it is just zero.

  -- ALGLIB --
     Copyright 25.03.2011 by Bochkanov Sergey
*************************************************************************/
ae_int_t mlpgetlayersize(multilayerperceptron* network,
     ae_int_t k,
     ae_state *_state)
{
    ae_int_t result;


    ae_assert(k>=0&&k<network->hllayersizes.cnt, "MLPGetLayerSize: incorrect layer index", _state);
    result = network->hllayersizes.ptr.p_int[k];
    return result;
}


/*************************************************************************
This function returns offset/scaling coefficients for I-th input of the
network.

INPUT PARAMETERS:
    Network     -   network
    I           -   input index

OUTPUT PARAMETERS:
    Mean        -   mean term
    Sigma       -   sigma term, guaranteed to be nonzero.

I-th input is passed through linear transformation
    IN[i] = (IN[i]-Mean)/Sigma
before feeding to the network

  -- ALGLIB --
     Copyright 25.03.2011 by Bochkanov Sergey
*************************************************************************/
void mlpgetinputscaling(multilayerperceptron* network,
     ae_int_t i,
     double* mean,
     double* sigma,
     ae_state *_state)
{

    *mean = 0;
    *sigma = 0;

    ae_assert(i>=0&&i<network->hllayersizes.ptr.p_int[0], "MLPGetInputScaling: incorrect (nonexistent) I", _state);
    *mean = network->columnmeans.ptr.p_double[i];
    *sigma = network->columnsigmas.ptr.p_double[i];
    if( ae_fp_eq(*sigma,(double)(0)) )
    {
        *sigma = (double)(1);
    }
}


/*************************************************************************
This function returns offset/scaling coefficients for I-th output of the
network.

INPUT PARAMETERS:
    Network     -   network
    I           -   input index

OUTPUT PARAMETERS:
    Mean        -   mean term
    Sigma       -   sigma term, guaranteed to be nonzero.

I-th output is passed through linear transformation
    OUT[i] = OUT[i]*Sigma+Mean
before returning it to user. In case we have SOFTMAX-normalized network,
we return (Mean,Sigma)=(0.0,1.0).

  -- ALGLIB --
     Copyright 25.03.2011 by Bochkanov Sergey
*************************************************************************/
void mlpgetoutputscaling(multilayerperceptron* network,
     ae_int_t i,
     double* mean,
     double* sigma,
     ae_state *_state)
{

    *mean = 0;
    *sigma = 0;

    ae_assert(i>=0&&i<network->hllayersizes.ptr.p_int[network->hllayersizes.cnt-1], "MLPGetOutputScaling: incorrect (nonexistent) I", _state);
    if( network->structinfo.ptr.p_int[6]==1 )
    {
        *mean = (double)(0);
        *sigma = (double)(1);
    }
    else
    {
        *mean = network->columnmeans.ptr.p_double[network->hllayersizes.ptr.p_int[0]+i];
        *sigma = network->columnsigmas.ptr.p_double[network->hllayersizes.ptr.p_int[0]+i];
    }
}


/*************************************************************************
This function returns information about Ith neuron of Kth layer

INPUT PARAMETERS:
    Network     -   network
    K           -   layer index
    I           -   neuron index (within layer)

OUTPUT PARAMETERS:
    FKind       -   activation function type (used by MLPActivationFunction())
                    this value is zero for input or linear neurons
    Threshold   -   also called offset, bias
                    zero for input neurons
                    
NOTE: this function throws exception if layer or neuron with  given  index
do not exists.

  -- ALGLIB --
     Copyright 25.03.2011 by Bochkanov Sergey
*************************************************************************/
void mlpgetneuroninfo(multilayerperceptron* network,
     ae_int_t k,
     ae_int_t i,
     ae_int_t* fkind,
     double* threshold,
     ae_state *_state)
{
    ae_int_t ncnt;
    ae_int_t istart;
    ae_int_t highlevelidx;
    ae_int_t activationoffset;

    *fkind = 0;
    *threshold = 0;

    ncnt = network->hlneurons.cnt/mlpbase_hlnfieldwidth;
    istart = network->structinfo.ptr.p_int[5];
    
    /*
     * search
     */
    network->integerbuf.ptr.p_int[0] = k;
    network->integerbuf.ptr.p_int[1] = i;
    highlevelidx = recsearch(&network->hlneurons, mlpbase_hlnfieldwidth, 2, 0, ncnt, &network->integerbuf, _state);
    ae_assert(highlevelidx>=0, "MLPGetNeuronInfo: incorrect (nonexistent) layer or neuron index", _state);
    
    /*
     * 1. find offset of the activation function record in the
     */
    if( network->hlneurons.ptr.p_int[highlevelidx*mlpbase_hlnfieldwidth+2]>=0 )
    {
        activationoffset = istart+network->hlneurons.ptr.p_int[highlevelidx*mlpbase_hlnfieldwidth+2]*mlpbase_nfieldwidth;
        *fkind = network->structinfo.ptr.p_int[activationoffset+0];
    }
    else
    {
        *fkind = 0;
    }
    if( network->hlneurons.ptr.p_int[highlevelidx*mlpbase_hlnfieldwidth+3]>=0 )
    {
        *threshold = network->weights.ptr.p_double[network->hlneurons.ptr.p_int[highlevelidx*mlpbase_hlnfieldwidth+3]];
    }
    else
    {
        *threshold = (double)(0);
    }
}


/*************************************************************************
This function returns information about connection from I0-th neuron of
K0-th layer to I1-th neuron of K1-th layer.

INPUT PARAMETERS:
    Network     -   network
    K0          -   layer index
    I0          -   neuron index (within layer)
    K1          -   layer index
    I1          -   neuron index (within layer)

RESULT:
    connection weight (zero for non-existent connections)

This function:
1. throws exception if layer or neuron with given index do not exists.
2. returns zero if neurons exist, but there is no connection between them

  -- ALGLIB --
     Copyright 25.03.2011 by Bochkanov Sergey
*************************************************************************/
double mlpgetweight(multilayerperceptron* network,
     ae_int_t k0,
     ae_int_t i0,
     ae_int_t k1,
     ae_int_t i1,
     ae_state *_state)
{
    ae_int_t ccnt;
    ae_int_t highlevelidx;
    double result;


    ccnt = network->hlconnections.cnt/mlpbase_hlconnfieldwidth;
    
    /*
     * check params
     */
    ae_assert(k0>=0&&k0<network->hllayersizes.cnt, "MLPGetWeight: incorrect (nonexistent) K0", _state);
    ae_assert(i0>=0&&i0<network->hllayersizes.ptr.p_int[k0], "MLPGetWeight: incorrect (nonexistent) I0", _state);
    ae_assert(k1>=0&&k1<network->hllayersizes.cnt, "MLPGetWeight: incorrect (nonexistent) K1", _state);
    ae_assert(i1>=0&&i1<network->hllayersizes.ptr.p_int[k1], "MLPGetWeight: incorrect (nonexistent) I1", _state);
    
    /*
     * search
     */
    network->integerbuf.ptr.p_int[0] = k0;
    network->integerbuf.ptr.p_int[1] = i0;
    network->integerbuf.ptr.p_int[2] = k1;
    network->integerbuf.ptr.p_int[3] = i1;
    highlevelidx = recsearch(&network->hlconnections, mlpbase_hlconnfieldwidth, 4, 0, ccnt, &network->integerbuf, _state);
    if( highlevelidx>=0 )
    {
        result = network->weights.ptr.p_double[network->hlconnections.ptr.p_int[highlevelidx*mlpbase_hlconnfieldwidth+4]];
    }
    else
    {
        result = (double)(0);
    }
    return result;
}


/*************************************************************************
This function sets offset/scaling coefficients for I-th input of the
network.

INPUT PARAMETERS:
    Network     -   network
    I           -   input index
    Mean        -   mean term
    Sigma       -   sigma term (if zero, will be replaced by 1.0)

NTE: I-th input is passed through linear transformation
    IN[i] = (IN[i]-Mean)/Sigma
before feeding to the network. This function sets Mean and Sigma.

  -- ALGLIB --
     Copyright 25.03.2011 by Bochkanov Sergey
*************************************************************************/
void mlpsetinputscaling(multilayerperceptron* network,
     ae_int_t i,
     double mean,
     double sigma,
     ae_state *_state)
{


    ae_assert(i>=0&&i<network->hllayersizes.ptr.p_int[0], "MLPSetInputScaling: incorrect (nonexistent) I", _state);
    ae_assert(ae_isfinite(mean, _state), "MLPSetInputScaling: infinite or NAN Mean", _state);
    ae_assert(ae_isfinite(sigma, _state), "MLPSetInputScaling: infinite or NAN Sigma", _state);
    if( ae_fp_eq(sigma,(double)(0)) )
    {
        sigma = (double)(1);
    }
    network->columnmeans.ptr.p_double[i] = mean;
    network->columnsigmas.ptr.p_double[i] = sigma;
}


/*************************************************************************
This function sets offset/scaling coefficients for I-th output of the
network.

INPUT PARAMETERS:
    Network     -   network
    I           -   input index
    Mean        -   mean term
    Sigma       -   sigma term (if zero, will be replaced by 1.0)

OUTPUT PARAMETERS:

NOTE: I-th output is passed through linear transformation
    OUT[i] = OUT[i]*Sigma+Mean
before returning it to user. This function sets Sigma/Mean. In case we
have SOFTMAX-normalized network, you can not set (Sigma,Mean) to anything
other than(0.0,1.0) - this function will throw exception.

  -- ALGLIB --
     Copyright 25.03.2011 by Bochkanov Sergey
*************************************************************************/
void mlpsetoutputscaling(multilayerperceptron* network,
     ae_int_t i,
     double mean,
     double sigma,
     ae_state *_state)
{


    ae_assert(i>=0&&i<network->hllayersizes.ptr.p_int[network->hllayersizes.cnt-1], "MLPSetOutputScaling: incorrect (nonexistent) I", _state);
    ae_assert(ae_isfinite(mean, _state), "MLPSetOutputScaling: infinite or NAN Mean", _state);
    ae_assert(ae_isfinite(sigma, _state), "MLPSetOutputScaling: infinite or NAN Sigma", _state);
    if( network->structinfo.ptr.p_int[6]==1 )
    {
        ae_assert(ae_fp_eq(mean,(double)(0)), "MLPSetOutputScaling: you can not set non-zero Mean term for classifier network", _state);
        ae_assert(ae_fp_eq(sigma,(double)(1)), "MLPSetOutputScaling: you can not set non-unit Sigma term for classifier network", _state);
    }
    else
    {
        if( ae_fp_eq(sigma,(double)(0)) )
        {
            sigma = (double)(1);
        }
        network->columnmeans.ptr.p_double[network->hllayersizes.ptr.p_int[0]+i] = mean;
        network->columnsigmas.ptr.p_double[network->hllayersizes.ptr.p_int[0]+i] = sigma;
    }
}


/*************************************************************************
This function modifies information about Ith neuron of Kth layer

INPUT PARAMETERS:
    Network     -   network
    K           -   layer index
    I           -   neuron index (within layer)
    FKind       -   activation function type (used by MLPActivationFunction())
                    this value must be zero for input neurons
                    (you can not set activation function for input neurons)
    Threshold   -   also called offset, bias
                    this value must be zero for input neurons
                    (you can not set threshold for input neurons)

NOTES:
1. this function throws exception if layer or neuron with given index do
   not exists.
2. this function also throws exception when you try to set non-linear
   activation function for input neurons (any kind of network) or for output
   neurons of classifier network.
3. this function throws exception when you try to set non-zero threshold for
   input neurons (any kind of network).

  -- ALGLIB --
     Copyright 25.03.2011 by Bochkanov Sergey
*************************************************************************/
void mlpsetneuroninfo(multilayerperceptron* network,
     ae_int_t k,
     ae_int_t i,
     ae_int_t fkind,
     double threshold,
     ae_state *_state)
{
    ae_int_t ncnt;
    ae_int_t istart;
    ae_int_t highlevelidx;
    ae_int_t activationoffset;


    ae_assert(ae_isfinite(threshold, _state), "MLPSetNeuronInfo: infinite or NAN Threshold", _state);
    
    /*
     * convenience vars
     */
    ncnt = network->hlneurons.cnt/mlpbase_hlnfieldwidth;
    istart = network->structinfo.ptr.p_int[5];
    
    /*
     * search
     */
    network->integerbuf.ptr.p_int[0] = k;
    network->integerbuf.ptr.p_int[1] = i;
    highlevelidx = recsearch(&network->hlneurons, mlpbase_hlnfieldwidth, 2, 0, ncnt, &network->integerbuf, _state);
    ae_assert(highlevelidx>=0, "MLPSetNeuronInfo: incorrect (nonexistent) layer or neuron index", _state);
    
    /*
     * activation function
     */
    if( network->hlneurons.ptr.p_int[highlevelidx*mlpbase_hlnfieldwidth+2]>=0 )
    {
        activationoffset = istart+network->hlneurons.ptr.p_int[highlevelidx*mlpbase_hlnfieldwidth+2]*mlpbase_nfieldwidth;
        network->structinfo.ptr.p_int[activationoffset+0] = fkind;
    }
    else
    {
        ae_assert(fkind==0, "MLPSetNeuronInfo: you try to set activation function for neuron which can not have one", _state);
    }
    
    /*
     * Threshold
     */
    if( network->hlneurons.ptr.p_int[highlevelidx*mlpbase_hlnfieldwidth+3]>=0 )
    {
        network->weights.ptr.p_double[network->hlneurons.ptr.p_int[highlevelidx*mlpbase_hlnfieldwidth+3]] = threshold;
    }
    else
    {
        ae_assert(ae_fp_eq(threshold,(double)(0)), "MLPSetNeuronInfo: you try to set non-zero threshold for neuron which can not have one", _state);
    }
}


/*************************************************************************
This function modifies information about connection from I0-th neuron of
K0-th layer to I1-th neuron of K1-th layer.

INPUT PARAMETERS:
    Network     -   network
    K0          -   layer index
    I0          -   neuron index (within layer)
    K1          -   layer index
    I1          -   neuron index (within layer)
    W           -   connection weight (must be zero for non-existent
                    connections)

This function:
1. throws exception if layer or neuron with given index do not exists.
2. throws exception if you try to set non-zero weight for non-existent
   connection

  -- ALGLIB --
     Copyright 25.03.2011 by Bochkanov Sergey
*************************************************************************/
void mlpsetweight(multilayerperceptron* network,
     ae_int_t k0,
     ae_int_t i0,
     ae_int_t k1,
     ae_int_t i1,
     double w,
     ae_state *_state)
{
    ae_int_t ccnt;
    ae_int_t highlevelidx;


    ccnt = network->hlconnections.cnt/mlpbase_hlconnfieldwidth;
    
    /*
     * check params
     */
    ae_assert(k0>=0&&k0<network->hllayersizes.cnt, "MLPSetWeight: incorrect (nonexistent) K0", _state);
    ae_assert(i0>=0&&i0<network->hllayersizes.ptr.p_int[k0], "MLPSetWeight: incorrect (nonexistent) I0", _state);
    ae_assert(k1>=0&&k1<network->hllayersizes.cnt, "MLPSetWeight: incorrect (nonexistent) K1", _state);
    ae_assert(i1>=0&&i1<network->hllayersizes.ptr.p_int[k1], "MLPSetWeight: incorrect (nonexistent) I1", _state);
    ae_assert(ae_isfinite(w, _state), "MLPSetWeight: infinite or NAN weight", _state);
    
    /*
     * search
     */
    network->integerbuf.ptr.p_int[0] = k0;
    network->integerbuf.ptr.p_int[1] = i0;
    network->integerbuf.ptr.p_int[2] = k1;
    network->integerbuf.ptr.p_int[3] = i1;
    highlevelidx = recsearch(&network->hlconnections, mlpbase_hlconnfieldwidth, 4, 0, ccnt, &network->integerbuf, _state);
    if( highlevelidx>=0 )
    {
        network->weights.ptr.p_double[network->hlconnections.ptr.p_int[highlevelidx*mlpbase_hlconnfieldwidth+4]] = w;
    }
    else
    {
        ae_assert(ae_fp_eq(w,(double)(0)), "MLPSetWeight: you try to set non-zero weight for non-existent connection", _state);
    }
}


/*************************************************************************
Neural network activation function

INPUT PARAMETERS:
    NET         -   neuron input
    K           -   function index (zero for linear function)

OUTPUT PARAMETERS:
    F           -   function
    DF          -   its derivative
    D2F         -   its second derivative

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlpactivationfunction(double net,
     ae_int_t k,
     double* f,
     double* df,
     double* d2f,
     ae_state *_state)
{
    double net2;
    double arg;
    double root;
    double r;

    *f = 0;
    *df = 0;
    *d2f = 0;

    if( k==0||k==-5 )
    {
        *f = net;
        *df = (double)(1);
        *d2f = (double)(0);
        return;
    }
    if( k==1 )
    {
        
        /*
         * TanH activation function
         */
        if( ae_fp_less(ae_fabs(net, _state),(double)(100)) )
        {
            *f = ae_tanh(net, _state);
        }
        else
        {
            *f = (double)(ae_sign(net, _state));
        }
        *df = 1-*f*(*f);
        *d2f = -2*(*f)*(*df);
        return;
    }
    if( k==3 )
    {
        
        /*
         * EX activation function
         */
        if( ae_fp_greater_eq(net,(double)(0)) )
        {
            net2 = net*net;
            arg = net2+1;
            root = ae_sqrt(arg, _state);
            *f = net+root;
            r = net/root;
            *df = 1+r;
            *d2f = (root-net*r)/arg;
        }
        else
        {
            *f = ae_exp(net, _state);
            *df = *f;
            *d2f = *f;
        }
        return;
    }
    if( k==2 )
    {
        *f = ae_exp(-ae_sqr(net, _state), _state);
        *df = -2*net*(*f);
        *d2f = -2*(*f+*df*net);
        return;
    }
    *f = (double)(0);
    *df = (double)(0);
    *d2f = (double)(0);
}


/*************************************************************************
Procesing

INPUT PARAMETERS:
    Network -   neural network
    X       -   input vector,  array[0..NIn-1].

OUTPUT PARAMETERS:
    Y       -   result. Regression estimate when solving regression  task,
                vector of posterior probabilities for classification task.

See also MLPProcessI

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlpprocess(multilayerperceptron* network,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{


    if( y->cnt<network->structinfo.ptr.p_int[2] )
    {
        ae_vector_set_length(y, network->structinfo.ptr.p_int[2], _state);
    }
    mlpinternalprocessvector(&network->structinfo, &network->weights, &network->columnmeans, &network->columnsigmas, &network->neurons, &network->dfdnet, x, y, _state);
}


/*************************************************************************
'interactive'  variant  of  MLPProcess  for  languages  like  Python which
support constructs like "Y = MLPProcess(NN,X)" and interactive mode of the
interpreter

This function allocates new array on each call,  so  it  is  significantly
slower than its 'non-interactive' counterpart, but it is  more  convenient
when you call it from command line.

  -- ALGLIB --
     Copyright 21.09.2010 by Bochkanov Sergey
*************************************************************************/
void mlpprocessi(multilayerperceptron* network,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{

    ae_vector_clear(y);

    mlpprocess(network, x, y, _state);
}


/*************************************************************************
Error of the neural network on dataset.


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore systems.
  ! Second improvement gives constant speedup (2-3x, depending on your CPU)
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
    Network     -   neural network;
    XY          -   training  set,  see  below  for  information  on   the
                    training set format;
    NPoints     -   points count.

RESULT:
    sum-of-squares error, SUM(sqr(y[i]-desired_y[i])/2)

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
dataset format is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
double mlperror(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double result;


    ae_assert(xy->rows>=npoints, "MLPError: XY has less than NPoints rows", _state);
    if( npoints>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+1, "MLPError: XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPError: XY has less than NIn+NOut columns", _state);
        }
    }
    mlpallerrorsx(network, xy, &network->dummysxy, npoints, 0, &network->dummyidx, 0, npoints, 0, &network->buf, &network->err, _state);
    result = ae_sqr(network->err.rmserror, _state)*npoints*mlpgetoutputscount(network, _state)/2;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
double _pexec_mlperror(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t npoints, ae_state *_state)
{
    return mlperror(network,xy,npoints, _state);
}


/*************************************************************************
Error of the neural network on dataset given by sparse matrix.


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore systems.
  ! Second improvement gives constant speedup (2-3x, depending on your CPU)
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
    Network     -   neural network
    XY          -   training  set,  see  below  for  information  on   the
                    training set format. This function checks  correctness
                    of  the  dataset  (no  NANs/INFs,  class  numbers  are
                    correct) and throws exception when  incorrect  dataset
                    is passed.  Sparse  matrix  must  use  CRS  format for
                    storage.
    NPoints     -   points count, >=0

RESULT:
    sum-of-squares error, SUM(sqr(y[i]-desired_y[i])/2)

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
dataset format is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).
  
  -- ALGLIB --
     Copyright 23.07.2012 by Bochkanov Sergey
*************************************************************************/
double mlperrorsparse(multilayerperceptron* network,
     sparsematrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double result;


    ae_assert(sparseiscrs(xy, _state), "MLPErrorSparse: XY is not in CRS format.", _state);
    ae_assert(sparsegetnrows(xy, _state)>=npoints, "MLPErrorSparse: XY has less than NPoints rows", _state);
    if( npoints>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(sparsegetncols(xy, _state)>=mlpgetinputscount(network, _state)+1, "MLPErrorSparse: XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(sparsegetncols(xy, _state)>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPErrorSparse: XY has less than NIn+NOut columns", _state);
        }
    }
    mlpallerrorsx(network, &network->dummydxy, xy, npoints, 1, &network->dummyidx, 0, npoints, 0, &network->buf, &network->err, _state);
    result = ae_sqr(network->err.rmserror, _state)*npoints*mlpgetoutputscount(network, _state)/2;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
double _pexec_mlperrorsparse(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t npoints, ae_state *_state)
{
    return mlperrorsparse(network,xy,npoints, _state);
}


/*************************************************************************
Natural error function for neural network, internal subroutine.

NOTE: this function is single-threaded. Unlike other  error  function,  it
receives no speed-up from being executed in SMP mode.

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
double mlperrorn(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t ssize,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    double e;
    double result;


    mlpproperties(network, &nin, &nout, &wcount, _state);
    result = (double)(0);
    for(i=0; i<=ssize-1; i++)
    {
        
        /*
         * Process vector
         */
        ae_v_move(&network->x.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nin-1));
        mlpprocess(network, &network->x, &network->y, _state);
        
        /*
         * Update error function
         */
        if( network->structinfo.ptr.p_int[6]==0 )
        {
            
            /*
             * Least squares error function
             */
            ae_v_sub(&network->y.ptr.p_double[0], 1, &xy->ptr.pp_double[i][nin], 1, ae_v_len(0,nout-1));
            e = ae_v_dotproduct(&network->y.ptr.p_double[0], 1, &network->y.ptr.p_double[0], 1, ae_v_len(0,nout-1));
            result = result+e/2;
        }
        else
        {
            
            /*
             * Cross-entropy error function
             */
            k = ae_round(xy->ptr.pp_double[i][nin], _state);
            if( k>=0&&k<nout )
            {
                result = result+mlpbase_safecrossentropy((double)(1), network->y.ptr.p_double[k], _state);
            }
        }
    }
    return result;
}


/*************************************************************************
Classification error of the neural network on dataset.


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network     -   neural network;
    XY          -   training  set,  see  below  for  information  on   the
                    training set format;
    NPoints     -   points count.

RESULT:
    classification error (number of misclassified cases)

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
dataset format is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
ae_int_t mlpclserror(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_int_t result;


    ae_assert(xy->rows>=npoints, "MLPClsError: XY has less than NPoints rows", _state);
    if( npoints>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+1, "MLPClsError: XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPClsError: XY has less than NIn+NOut columns", _state);
        }
    }
    mlpallerrorsx(network, xy, &network->dummysxy, npoints, 0, &network->dummyidx, 0, npoints, 0, &network->buf, &network->err, _state);
    result = ae_round(npoints*network->err.relclserror, _state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_int_t _pexec_mlpclserror(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t npoints, ae_state *_state)
{
    return mlpclserror(network,xy,npoints, _state);
}


/*************************************************************************
Relative classification error on the test set.


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network     -   neural network;
    XY          -   training  set,  see  below  for  information  on   the
                    training set format;
    NPoints     -   points count.

RESULT:
Percent   of incorrectly   classified  cases.  Works  both  for classifier
networks and general purpose networks used as classifiers.

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
dataset format is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).

  -- ALGLIB --
     Copyright 25.12.2008 by Bochkanov Sergey
*************************************************************************/
double mlprelclserror(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double result;


    ae_assert(xy->rows>=npoints, "MLPRelClsError: XY has less than NPoints rows", _state);
    if( npoints>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+1, "MLPRelClsError: XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPRelClsError: XY has less than NIn+NOut columns", _state);
        }
    }
    if( npoints>0 )
    {
        result = (double)mlpclserror(network, xy, npoints, _state)/(double)npoints;
    }
    else
    {
        result = 0.0;
    }
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
double _pexec_mlprelclserror(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t npoints, ae_state *_state)
{
    return mlprelclserror(network,xy,npoints, _state);
}


/*************************************************************************
Relative classification error on the test set given by sparse matrix.


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network     -   neural network;
    XY          -   training  set,  see  below  for  information  on   the
                    training set format. Sparse matrix must use CRS format
                    for storage.
    NPoints     -   points count, >=0.

RESULT:
Percent   of incorrectly   classified  cases.  Works  both  for classifier
networks and general purpose networks used as classifiers.

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
dataset format is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).
  
  -- ALGLIB --
     Copyright 09.08.2012 by Bochkanov Sergey
*************************************************************************/
double mlprelclserrorsparse(multilayerperceptron* network,
     sparsematrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double result;


    ae_assert(sparseiscrs(xy, _state), "MLPRelClsErrorSparse: sparse matrix XY is not in CRS format.", _state);
    ae_assert(sparsegetnrows(xy, _state)>=npoints, "MLPRelClsErrorSparse: sparse matrix XY has less than NPoints rows", _state);
    if( npoints>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(sparsegetncols(xy, _state)>=mlpgetinputscount(network, _state)+1, "MLPRelClsErrorSparse: sparse matrix XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(sparsegetncols(xy, _state)>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPRelClsErrorSparse: sparse matrix XY has less than NIn+NOut columns", _state);
        }
    }
    mlpallerrorsx(network, &network->dummydxy, xy, npoints, 1, &network->dummyidx, 0, npoints, 0, &network->buf, &network->err, _state);
    result = network->err.relclserror;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
double _pexec_mlprelclserrorsparse(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t npoints, ae_state *_state)
{
    return mlprelclserrorsparse(network,xy,npoints, _state);
}


/*************************************************************************
Average cross-entropy  (in bits  per element) on the test set.


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network     -   neural network;
    XY          -   training  set,  see  below  for  information  on   the
                    training set format;
    NPoints     -   points count.

RESULT:
CrossEntropy/(NPoints*LN(2)).
Zero if network solves regression task.

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
dataset format is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).

  -- ALGLIB --
     Copyright 08.01.2009 by Bochkanov Sergey
*************************************************************************/
double mlpavgce(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double result;


    ae_assert(xy->rows>=npoints, "MLPAvgCE: XY has less than NPoints rows", _state);
    if( npoints>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+1, "MLPAvgCE: XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPAvgCE: XY has less than NIn+NOut columns", _state);
        }
    }
    mlpallerrorsx(network, xy, &network->dummysxy, npoints, 0, &network->dummyidx, 0, npoints, 0, &network->buf, &network->err, _state);
    result = network->err.avgce;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
double _pexec_mlpavgce(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t npoints, ae_state *_state)
{
    return mlpavgce(network,xy,npoints, _state);
}


/*************************************************************************
Average  cross-entropy  (in bits  per element)  on the  test set  given by
sparse matrix.


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network     -   neural network;
    XY          -   training  set,  see  below  for  information  on   the
                    training set format. This function checks  correctness
                    of  the  dataset  (no  NANs/INFs,  class  numbers  are
                    correct) and throws exception when  incorrect  dataset
                    is passed.  Sparse  matrix  must  use  CRS  format for
                    storage.
    NPoints     -   points count, >=0.

RESULT:
CrossEntropy/(NPoints*LN(2)).
Zero if network solves regression task.

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
dataset format is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).
  
  -- ALGLIB --
     Copyright 9.08.2012 by Bochkanov Sergey
*************************************************************************/
double mlpavgcesparse(multilayerperceptron* network,
     sparsematrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double result;


    ae_assert(sparseiscrs(xy, _state), "MLPAvgCESparse: sparse matrix XY is not in CRS format.", _state);
    ae_assert(sparsegetnrows(xy, _state)>=npoints, "MLPAvgCESparse: sparse matrix XY has less than NPoints rows", _state);
    if( npoints>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(sparsegetncols(xy, _state)>=mlpgetinputscount(network, _state)+1, "MLPAvgCESparse: sparse matrix XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(sparsegetncols(xy, _state)>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPAvgCESparse: sparse matrix XY has less than NIn+NOut columns", _state);
        }
    }
    mlpallerrorsx(network, &network->dummydxy, xy, npoints, 1, &network->dummyidx, 0, npoints, 0, &network->buf, &network->err, _state);
    result = network->err.avgce;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
double _pexec_mlpavgcesparse(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t npoints, ae_state *_state)
{
    return mlpavgcesparse(network,xy,npoints, _state);
}


/*************************************************************************
RMS error on the test set given.


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network     -   neural network;
    XY          -   training  set,  see  below  for  information  on   the
                    training set format;
    NPoints     -   points count.

RESULT:
Root mean  square error. Its meaning for regression task is obvious. As for
classification  task,  RMS  error  means  error  when estimating  posterior
probabilities.

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
dataset format is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
double mlprmserror(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double result;


    ae_assert(xy->rows>=npoints, "MLPRMSError: XY has less than NPoints rows", _state);
    if( npoints>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+1, "MLPRMSError: XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPRMSError: XY has less than NIn+NOut columns", _state);
        }
    }
    mlpallerrorsx(network, xy, &network->dummysxy, npoints, 0, &network->dummyidx, 0, npoints, 0, &network->buf, &network->err, _state);
    result = network->err.rmserror;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
double _pexec_mlprmserror(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t npoints, ae_state *_state)
{
    return mlprmserror(network,xy,npoints, _state);
}


/*************************************************************************
RMS error on the test set given by sparse matrix.


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network     -   neural network;
    XY          -   training  set,  see  below  for  information  on   the
                    training set format. This function checks  correctness
                    of  the  dataset  (no  NANs/INFs,  class  numbers  are
                    correct) and throws exception when  incorrect  dataset
                    is passed.  Sparse  matrix  must  use  CRS  format for
                    storage.
    NPoints     -   points count, >=0.

RESULT:
Root mean  square error. Its meaning for regression task is obvious. As for
classification  task,  RMS  error  means  error  when estimating  posterior
probabilities.

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
dataset format is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).
  
  -- ALGLIB --
     Copyright 09.08.2012 by Bochkanov Sergey
*************************************************************************/
double mlprmserrorsparse(multilayerperceptron* network,
     sparsematrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double result;


    ae_assert(sparseiscrs(xy, _state), "MLPRMSErrorSparse: sparse matrix XY is not in CRS format.", _state);
    ae_assert(sparsegetnrows(xy, _state)>=npoints, "MLPRMSErrorSparse: sparse matrix XY has less than NPoints rows", _state);
    if( npoints>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(sparsegetncols(xy, _state)>=mlpgetinputscount(network, _state)+1, "MLPRMSErrorSparse: sparse matrix XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(sparsegetncols(xy, _state)>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPRMSErrorSparse: sparse matrix XY has less than NIn+NOut columns", _state);
        }
    }
    mlpallerrorsx(network, &network->dummydxy, xy, npoints, 1, &network->dummyidx, 0, npoints, 0, &network->buf, &network->err, _state);
    result = network->err.rmserror;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
double _pexec_mlprmserrorsparse(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t npoints, ae_state *_state)
{
    return mlprmserrorsparse(network,xy,npoints, _state);
}


/*************************************************************************
Average absolute error on the test set.


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network     -   neural network;
    XY          -   training  set,  see  below  for  information  on   the
                    training set format;
    NPoints     -   points count.

RESULT:
Its meaning for regression task is obvious. As for classification task, it
means average error when estimating posterior probabilities.

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
dataset format is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).

  -- ALGLIB --
     Copyright 11.03.2008 by Bochkanov Sergey
*************************************************************************/
double mlpavgerror(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double result;


    ae_assert(xy->rows>=npoints, "MLPAvgError: XY has less than NPoints rows", _state);
    if( npoints>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+1, "MLPAvgError: XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPAvgError: XY has less than NIn+NOut columns", _state);
        }
    }
    mlpallerrorsx(network, xy, &network->dummysxy, npoints, 0, &network->dummyidx, 0, npoints, 0, &network->buf, &network->err, _state);
    result = network->err.avgerror;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
double _pexec_mlpavgerror(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t npoints, ae_state *_state)
{
    return mlpavgerror(network,xy,npoints, _state);
}


/*************************************************************************
Average absolute error on the test set given by sparse matrix.


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network     -   neural network;
    XY          -   training  set,  see  below  for  information  on   the
                    training set format. This function checks  correctness
                    of  the  dataset  (no  NANs/INFs,  class  numbers  are
                    correct) and throws exception when  incorrect  dataset
                    is passed.  Sparse  matrix  must  use  CRS  format for
                    storage.
    NPoints     -   points count, >=0.

RESULT:
Its meaning for regression task is obvious. As for classification task, it
means average error when estimating posterior probabilities.

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
dataset format is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).
  
  -- ALGLIB --
     Copyright 09.08.2012 by Bochkanov Sergey
*************************************************************************/
double mlpavgerrorsparse(multilayerperceptron* network,
     sparsematrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double result;


    ae_assert(sparseiscrs(xy, _state), "MLPAvgErrorSparse: XY is not in CRS format.", _state);
    ae_assert(sparsegetnrows(xy, _state)>=npoints, "MLPAvgErrorSparse: XY has less than NPoints rows", _state);
    if( npoints>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(sparsegetncols(xy, _state)>=mlpgetinputscount(network, _state)+1, "MLPAvgErrorSparse: XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(sparsegetncols(xy, _state)>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPAvgErrorSparse: XY has less than NIn+NOut columns", _state);
        }
    }
    mlpallerrorsx(network, &network->dummydxy, xy, npoints, 1, &network->dummyidx, 0, npoints, 0, &network->buf, &network->err, _state);
    result = network->err.avgerror;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
double _pexec_mlpavgerrorsparse(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t npoints, ae_state *_state)
{
    return mlpavgerrorsparse(network,xy,npoints, _state);
}


/*************************************************************************
Average relative error on the test set.


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network     -   neural network;
    XY          -   training  set,  see  below  for  information  on   the
                    training set format;
    NPoints     -   points count.

RESULT:
Its meaning for regression task is obvious. As for classification task, it
means  average  relative  error  when  estimating posterior probability of
belonging to the correct class.

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
dataset format is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).

  -- ALGLIB --
     Copyright 11.03.2008 by Bochkanov Sergey
*************************************************************************/
double mlpavgrelerror(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double result;


    ae_assert(xy->rows>=npoints, "MLPAvgRelError: XY has less than NPoints rows", _state);
    if( npoints>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+1, "MLPAvgRelError: XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPAvgRelError: XY has less than NIn+NOut columns", _state);
        }
    }
    mlpallerrorsx(network, xy, &network->dummysxy, npoints, 0, &network->dummyidx, 0, npoints, 0, &network->buf, &network->err, _state);
    result = network->err.avgrelerror;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
double _pexec_mlpavgrelerror(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t npoints, ae_state *_state)
{
    return mlpavgrelerror(network,xy,npoints, _state);
}


/*************************************************************************
Average relative error on the test set given by sparse matrix.


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network     -   neural network;
    XY          -   training  set,  see  below  for  information  on   the
                    training set format. This function checks  correctness
                    of  the  dataset  (no  NANs/INFs,  class  numbers  are
                    correct) and throws exception when  incorrect  dataset
                    is passed.  Sparse  matrix  must  use  CRS  format for
                    storage.
    NPoints     -   points count, >=0.

RESULT:
Its meaning for regression task is obvious. As for classification task, it
means  average  relative  error  when  estimating posterior probability of
belonging to the correct class.

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
dataset format is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).
  
  -- ALGLIB --
     Copyright 09.08.2012 by Bochkanov Sergey
*************************************************************************/
double mlpavgrelerrorsparse(multilayerperceptron* network,
     sparsematrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    double result;


    ae_assert(sparseiscrs(xy, _state), "MLPAvgRelErrorSparse: XY is not in CRS format.", _state);
    ae_assert(sparsegetnrows(xy, _state)>=npoints, "MLPAvgRelErrorSparse: XY has less than NPoints rows", _state);
    if( npoints>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(sparsegetncols(xy, _state)>=mlpgetinputscount(network, _state)+1, "MLPAvgRelErrorSparse: XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(sparsegetncols(xy, _state)>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPAvgRelErrorSparse: XY has less than NIn+NOut columns", _state);
        }
    }
    mlpallerrorsx(network, &network->dummydxy, xy, npoints, 1, &network->dummyidx, 0, npoints, 0, &network->buf, &network->err, _state);
    result = network->err.avgrelerror;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
double _pexec_mlpavgrelerrorsparse(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t npoints, ae_state *_state)
{
    return mlpavgrelerrorsparse(network,xy,npoints, _state);
}


/*************************************************************************
Gradient calculation

INPUT PARAMETERS:
    Network -   network initialized with one of the network creation funcs
    X       -   input vector, length of array must be at least NIn
    DesiredY-   desired outputs, length of array must be at least NOut
    Grad    -   possibly preallocated array. If size of array is smaller
                than WCount, it will be reallocated. It is recommended to
                reuse previously allocated array to reduce allocation
                overhead.

OUTPUT PARAMETERS:
    E       -   error function, SUM(sqr(y[i]-desiredy[i])/2,i)
    Grad    -   gradient of E with respect to weights of network, array[WCount]
    
  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlpgrad(multilayerperceptron* network,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* desiredy,
     double* e,
     /* Real    */ ae_vector* grad,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t nout;
    ae_int_t ntotal;

    *e = 0;

    
    /*
     * Alloc
     */
    rvectorsetlengthatleast(grad, network->structinfo.ptr.p_int[4], _state);
    
    /*
     * Prepare dError/dOut, internal structures
     */
    mlpprocess(network, x, &network->y, _state);
    nout = network->structinfo.ptr.p_int[2];
    ntotal = network->structinfo.ptr.p_int[3];
    *e = (double)(0);
    for(i=0; i<=ntotal-1; i++)
    {
        network->derror.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=nout-1; i++)
    {
        network->derror.ptr.p_double[ntotal-nout+i] = network->y.ptr.p_double[i]-desiredy->ptr.p_double[i];
        *e = *e+ae_sqr(network->y.ptr.p_double[i]-desiredy->ptr.p_double[i], _state)/2;
    }
    
    /*
     * gradient
     */
    mlpbase_mlpinternalcalculategradient(network, &network->neurons, &network->weights, &network->derror, grad, ae_false, _state);
}


/*************************************************************************
Gradient calculation (natural error function is used)

INPUT PARAMETERS:
    Network -   network initialized with one of the network creation funcs
    X       -   input vector, length of array must be at least NIn
    DesiredY-   desired outputs, length of array must be at least NOut
    Grad    -   possibly preallocated array. If size of array is smaller
                than WCount, it will be reallocated. It is recommended to
                reuse previously allocated array to reduce allocation
                overhead.

OUTPUT PARAMETERS:
    E       -   error function, sum-of-squares for regression networks,
                cross-entropy for classification networks.
    Grad    -   gradient of E with respect to weights of network, array[WCount]

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlpgradn(multilayerperceptron* network,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* desiredy,
     double* e,
     /* Real    */ ae_vector* grad,
     ae_state *_state)
{
    double s;
    ae_int_t i;
    ae_int_t nout;
    ae_int_t ntotal;

    *e = 0;

    
    /*
     * Alloc
     */
    rvectorsetlengthatleast(grad, network->structinfo.ptr.p_int[4], _state);
    
    /*
     * Prepare dError/dOut, internal structures
     */
    mlpprocess(network, x, &network->y, _state);
    nout = network->structinfo.ptr.p_int[2];
    ntotal = network->structinfo.ptr.p_int[3];
    for(i=0; i<=ntotal-1; i++)
    {
        network->derror.ptr.p_double[i] = (double)(0);
    }
    *e = (double)(0);
    if( network->structinfo.ptr.p_int[6]==0 )
    {
        
        /*
         * Regression network, least squares
         */
        for(i=0; i<=nout-1; i++)
        {
            network->derror.ptr.p_double[ntotal-nout+i] = network->y.ptr.p_double[i]-desiredy->ptr.p_double[i];
            *e = *e+ae_sqr(network->y.ptr.p_double[i]-desiredy->ptr.p_double[i], _state)/2;
        }
    }
    else
    {
        
        /*
         * Classification network, cross-entropy
         */
        s = (double)(0);
        for(i=0; i<=nout-1; i++)
        {
            s = s+desiredy->ptr.p_double[i];
        }
        for(i=0; i<=nout-1; i++)
        {
            network->derror.ptr.p_double[ntotal-nout+i] = s*network->y.ptr.p_double[i]-desiredy->ptr.p_double[i];
            *e = *e+mlpbase_safecrossentropy(desiredy->ptr.p_double[i], network->y.ptr.p_double[i], _state);
        }
    }
    
    /*
     * gradient
     */
    mlpbase_mlpinternalcalculategradient(network, &network->neurons, &network->weights, &network->derror, grad, ae_true, _state);
}


/*************************************************************************
Batch gradient calculation for a set of inputs/outputs


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network -   network initialized with one of the network creation funcs
    XY      -   original dataset in dense format; one sample = one row:
                * first NIn columns contain inputs,
                * for regression problem, next NOut columns store
                  desired outputs.
                * for classification problem, next column (just one!)
                  stores class number.
    SSize   -   number of elements in XY
    Grad    -   possibly preallocated array. If size of array is smaller
                than WCount, it will be reallocated. It is recommended to
                reuse previously allocated array to reduce allocation
                overhead.

OUTPUT PARAMETERS:
    E       -   error function, SUM(sqr(y[i]-desiredy[i])/2,i)
    Grad    -   gradient of E with respect to weights of network, array[WCount]

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlpgradbatch(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t ssize,
     double* e,
     /* Real    */ ae_vector* grad,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t subset0;
    ae_int_t subset1;
    ae_int_t subsettype;
    smlpgrad *sgrad;
    ae_smart_ptr _sgrad;

    ae_frame_make(_state, &_frame_block);
    *e = 0;
    ae_smart_ptr_init(&_sgrad, (void**)&sgrad, _state);

    ae_assert(ssize>=0, "MLPGradBatchSparse: SSize<0", _state);
    subset0 = 0;
    subset1 = ssize;
    subsettype = 0;
    mlpproperties(network, &nin, &nout, &wcount, _state);
    rvectorsetlengthatleast(grad, wcount, _state);
    ae_shared_pool_first_recycled(&network->gradbuf, &_sgrad, _state);
    while(sgrad!=NULL)
    {
        sgrad->f = 0.0;
        for(i=0; i<=wcount-1; i++)
        {
            sgrad->g.ptr.p_double[i] = 0.0;
        }
        ae_shared_pool_next_recycled(&network->gradbuf, &_sgrad, _state);
    }
    mlpgradbatchx(network, xy, &network->dummysxy, ssize, 0, &network->dummyidx, subset0, subset1, subsettype, &network->buf, &network->gradbuf, _state);
    *e = 0.0;
    for(i=0; i<=wcount-1; i++)
    {
        grad->ptr.p_double[i] = 0.0;
    }
    ae_shared_pool_first_recycled(&network->gradbuf, &_sgrad, _state);
    while(sgrad!=NULL)
    {
        *e = *e+sgrad->f;
        for(i=0; i<=wcount-1; i++)
        {
            grad->ptr.p_double[i] = grad->ptr.p_double[i]+sgrad->g.ptr.p_double[i];
        }
        ae_shared_pool_next_recycled(&network->gradbuf, &_sgrad, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_mlpgradbatch(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t ssize,
    double* e,
    /* Real    */ ae_vector* grad, ae_state *_state)
{
    mlpgradbatch(network,xy,ssize,e,grad, _state);
}


/*************************************************************************
Batch gradient calculation for a set  of inputs/outputs  given  by  sparse
matrices


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network -   network initialized with one of the network creation funcs
    XY      -   original dataset in sparse format; one sample = one row:
                * MATRIX MUST BE STORED IN CRS FORMAT
                * first NIn columns contain inputs.
                * for regression problem, next NOut columns store
                  desired outputs.
                * for classification problem, next column (just one!)
                  stores class number.
    SSize   -   number of elements in XY
    Grad    -   possibly preallocated array. If size of array is smaller
                than WCount, it will be reallocated. It is recommended to
                reuse previously allocated array to reduce allocation
                overhead.

OUTPUT PARAMETERS:
    E       -   error function, SUM(sqr(y[i]-desiredy[i])/2,i)
    Grad    -   gradient of E with respect to weights of network, array[WCount]

  -- ALGLIB --
     Copyright 26.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlpgradbatchsparse(multilayerperceptron* network,
     sparsematrix* xy,
     ae_int_t ssize,
     double* e,
     /* Real    */ ae_vector* grad,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t subset0;
    ae_int_t subset1;
    ae_int_t subsettype;
    smlpgrad *sgrad;
    ae_smart_ptr _sgrad;

    ae_frame_make(_state, &_frame_block);
    *e = 0;
    ae_smart_ptr_init(&_sgrad, (void**)&sgrad, _state);

    ae_assert(ssize>=0, "MLPGradBatchSparse: SSize<0", _state);
    ae_assert(sparseiscrs(xy, _state), "MLPGradBatchSparse: sparse matrix XY must be in CRS format.", _state);
    subset0 = 0;
    subset1 = ssize;
    subsettype = 0;
    mlpproperties(network, &nin, &nout, &wcount, _state);
    rvectorsetlengthatleast(grad, wcount, _state);
    ae_shared_pool_first_recycled(&network->gradbuf, &_sgrad, _state);
    while(sgrad!=NULL)
    {
        sgrad->f = 0.0;
        for(i=0; i<=wcount-1; i++)
        {
            sgrad->g.ptr.p_double[i] = 0.0;
        }
        ae_shared_pool_next_recycled(&network->gradbuf, &_sgrad, _state);
    }
    mlpgradbatchx(network, &network->dummydxy, xy, ssize, 1, &network->dummyidx, subset0, subset1, subsettype, &network->buf, &network->gradbuf, _state);
    *e = 0.0;
    for(i=0; i<=wcount-1; i++)
    {
        grad->ptr.p_double[i] = 0.0;
    }
    ae_shared_pool_first_recycled(&network->gradbuf, &_sgrad, _state);
    while(sgrad!=NULL)
    {
        *e = *e+sgrad->f;
        for(i=0; i<=wcount-1; i++)
        {
            grad->ptr.p_double[i] = grad->ptr.p_double[i]+sgrad->g.ptr.p_double[i];
        }
        ae_shared_pool_next_recycled(&network->gradbuf, &_sgrad, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_mlpgradbatchsparse(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t ssize,
    double* e,
    /* Real    */ ae_vector* grad, ae_state *_state)
{
    mlpgradbatchsparse(network,xy,ssize,e,grad, _state);
}


/*************************************************************************
Batch gradient calculation for a subset of dataset


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network -   network initialized with one of the network creation funcs
    XY      -   original dataset in dense format; one sample = one row:
                * first NIn columns contain inputs,
                * for regression problem, next NOut columns store
                  desired outputs.
                * for classification problem, next column (just one!)
                  stores class number.
    SetSize -   real size of XY, SetSize>=0;
    Idx     -   subset of SubsetSize elements, array[SubsetSize]:
                * Idx[I] stores row index in the original dataset which is
                  given by XY. Gradient is calculated with respect to rows
                  whose indexes are stored in Idx[].
                * Idx[]  must store correct indexes; this function  throws
                  an  exception  in  case  incorrect index (less than 0 or
                  larger than rows(XY)) is given
                * Idx[]  may  store  indexes  in  any  order and even with
                  repetitions.
    SubsetSize- number of elements in Idx[] array:
                * positive value means that subset given by Idx[] is processed
                * zero value results in zero gradient
                * negative value means that full dataset is processed
    Grad      - possibly  preallocated array. If size of array is  smaller
                than WCount, it will be reallocated. It is  recommended to
                reuse  previously  allocated  array  to  reduce allocation
                overhead.

OUTPUT PARAMETERS:
    E         - error function, SUM(sqr(y[i]-desiredy[i])/2,i)
    Grad      - gradient  of  E  with  respect   to  weights  of  network,
                array[WCount]

  -- ALGLIB --
     Copyright 26.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlpgradbatchsubset(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t setsize,
     /* Integer */ ae_vector* idx,
     ae_int_t subsetsize,
     double* e,
     /* Real    */ ae_vector* grad,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t npoints;
    ae_int_t subset0;
    ae_int_t subset1;
    ae_int_t subsettype;
    smlpgrad *sgrad;
    ae_smart_ptr _sgrad;

    ae_frame_make(_state, &_frame_block);
    *e = 0;
    ae_smart_ptr_init(&_sgrad, (void**)&sgrad, _state);

    ae_assert(setsize>=0, "MLPGradBatchSubset: SetSize<0", _state);
    ae_assert(subsetsize<=idx->cnt, "MLPGradBatchSubset: SubsetSize>Length(Idx)", _state);
    npoints = setsize;
    if( subsetsize<0 )
    {
        subset0 = 0;
        subset1 = setsize;
        subsettype = 0;
    }
    else
    {
        subset0 = 0;
        subset1 = subsetsize;
        subsettype = 1;
        for(i=0; i<=subsetsize-1; i++)
        {
            ae_assert(idx->ptr.p_int[i]>=0, "MLPGradBatchSubset: incorrect index of XY row(Idx[I]<0)", _state);
            ae_assert(idx->ptr.p_int[i]<=npoints-1, "MLPGradBatchSubset: incorrect index of XY row(Idx[I]>Rows(XY)-1)", _state);
        }
    }
    mlpproperties(network, &nin, &nout, &wcount, _state);
    rvectorsetlengthatleast(grad, wcount, _state);
    ae_shared_pool_first_recycled(&network->gradbuf, &_sgrad, _state);
    while(sgrad!=NULL)
    {
        sgrad->f = 0.0;
        for(i=0; i<=wcount-1; i++)
        {
            sgrad->g.ptr.p_double[i] = 0.0;
        }
        ae_shared_pool_next_recycled(&network->gradbuf, &_sgrad, _state);
    }
    mlpgradbatchx(network, xy, &network->dummysxy, setsize, 0, idx, subset0, subset1, subsettype, &network->buf, &network->gradbuf, _state);
    *e = 0.0;
    for(i=0; i<=wcount-1; i++)
    {
        grad->ptr.p_double[i] = 0.0;
    }
    ae_shared_pool_first_recycled(&network->gradbuf, &_sgrad, _state);
    while(sgrad!=NULL)
    {
        *e = *e+sgrad->f;
        for(i=0; i<=wcount-1; i++)
        {
            grad->ptr.p_double[i] = grad->ptr.p_double[i]+sgrad->g.ptr.p_double[i];
        }
        ae_shared_pool_next_recycled(&network->gradbuf, &_sgrad, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_mlpgradbatchsubset(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t setsize,
    /* Integer */ ae_vector* idx,
    ae_int_t subsetsize,
    double* e,
    /* Real    */ ae_vector* grad, ae_state *_state)
{
    mlpgradbatchsubset(network,xy,setsize,idx,subsetsize,e,grad, _state);
}


/*************************************************************************
Batch gradient calculation for a set of inputs/outputs  for  a  subset  of
dataset given by set of indexes.


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network -   network initialized with one of the network creation funcs
    XY      -   original dataset in sparse format; one sample = one row:
                * MATRIX MUST BE STORED IN CRS FORMAT
                * first NIn columns contain inputs,
                * for regression problem, next NOut columns store
                  desired outputs.
                * for classification problem, next column (just one!)
                  stores class number.
    SetSize -   real size of XY, SetSize>=0;
    Idx     -   subset of SubsetSize elements, array[SubsetSize]:
                * Idx[I] stores row index in the original dataset which is
                  given by XY. Gradient is calculated with respect to rows
                  whose indexes are stored in Idx[].
                * Idx[]  must store correct indexes; this function  throws
                  an  exception  in  case  incorrect index (less than 0 or
                  larger than rows(XY)) is given
                * Idx[]  may  store  indexes  in  any  order and even with
                  repetitions.
    SubsetSize- number of elements in Idx[] array:
                * positive value means that subset given by Idx[] is processed
                * zero value results in zero gradient
                * negative value means that full dataset is processed
    Grad      - possibly  preallocated array. If size of array is  smaller
                than WCount, it will be reallocated. It is  recommended to
                reuse  previously  allocated  array  to  reduce allocation
                overhead.

OUTPUT PARAMETERS:
    E       -   error function, SUM(sqr(y[i]-desiredy[i])/2,i)
    Grad    -   gradient  of  E  with  respect   to  weights  of  network,
                array[WCount]

NOTE: when  SubsetSize<0 is used full dataset by call MLPGradBatchSparse
      function.
    
  -- ALGLIB --
     Copyright 26.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlpgradbatchsparsesubset(multilayerperceptron* network,
     sparsematrix* xy,
     ae_int_t setsize,
     /* Integer */ ae_vector* idx,
     ae_int_t subsetsize,
     double* e,
     /* Real    */ ae_vector* grad,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t npoints;
    ae_int_t subset0;
    ae_int_t subset1;
    ae_int_t subsettype;
    smlpgrad *sgrad;
    ae_smart_ptr _sgrad;

    ae_frame_make(_state, &_frame_block);
    *e = 0;
    ae_smart_ptr_init(&_sgrad, (void**)&sgrad, _state);

    ae_assert(setsize>=0, "MLPGradBatchSparseSubset: SetSize<0", _state);
    ae_assert(subsetsize<=idx->cnt, "MLPGradBatchSparseSubset: SubsetSize>Length(Idx)", _state);
    ae_assert(sparseiscrs(xy, _state), "MLPGradBatchSparseSubset: sparse matrix XY must be in CRS format.", _state);
    npoints = setsize;
    if( subsetsize<0 )
    {
        subset0 = 0;
        subset1 = setsize;
        subsettype = 0;
    }
    else
    {
        subset0 = 0;
        subset1 = subsetsize;
        subsettype = 1;
        for(i=0; i<=subsetsize-1; i++)
        {
            ae_assert(idx->ptr.p_int[i]>=0, "MLPGradBatchSparseSubset: incorrect index of XY row(Idx[I]<0)", _state);
            ae_assert(idx->ptr.p_int[i]<=npoints-1, "MLPGradBatchSparseSubset: incorrect index of XY row(Idx[I]>Rows(XY)-1)", _state);
        }
    }
    mlpproperties(network, &nin, &nout, &wcount, _state);
    rvectorsetlengthatleast(grad, wcount, _state);
    ae_shared_pool_first_recycled(&network->gradbuf, &_sgrad, _state);
    while(sgrad!=NULL)
    {
        sgrad->f = 0.0;
        for(i=0; i<=wcount-1; i++)
        {
            sgrad->g.ptr.p_double[i] = 0.0;
        }
        ae_shared_pool_next_recycled(&network->gradbuf, &_sgrad, _state);
    }
    mlpgradbatchx(network, &network->dummydxy, xy, setsize, 1, idx, subset0, subset1, subsettype, &network->buf, &network->gradbuf, _state);
    *e = 0.0;
    for(i=0; i<=wcount-1; i++)
    {
        grad->ptr.p_double[i] = 0.0;
    }
    ae_shared_pool_first_recycled(&network->gradbuf, &_sgrad, _state);
    while(sgrad!=NULL)
    {
        *e = *e+sgrad->f;
        for(i=0; i<=wcount-1; i++)
        {
            grad->ptr.p_double[i] = grad->ptr.p_double[i]+sgrad->g.ptr.p_double[i];
        }
        ae_shared_pool_next_recycled(&network->gradbuf, &_sgrad, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_mlpgradbatchsparsesubset(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t setsize,
    /* Integer */ ae_vector* idx,
    ae_int_t subsetsize,
    double* e,
    /* Real    */ ae_vector* grad, ae_state *_state)
{
    mlpgradbatchsparsesubset(network,xy,setsize,idx,subsetsize,e,grad, _state);
}


/*************************************************************************
Internal function which actually calculates batch gradient for a subset or
full dataset, which can be represented in different formats.

THIS FUNCTION IS NOT INTENDED TO BE USED BY ALGLIB USERS!

  -- ALGLIB --
     Copyright 26.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlpgradbatchx(multilayerperceptron* network,
     /* Real    */ ae_matrix* densexy,
     sparsematrix* sparsexy,
     ae_int_t datasetsize,
     ae_int_t datasettype,
     /* Integer */ ae_vector* idx,
     ae_int_t subset0,
     ae_int_t subset1,
     ae_int_t subsettype,
     ae_shared_pool* buf,
     ae_shared_pool* gradbuf,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t rowsize;
    ae_int_t srcidx;
    ae_int_t cstart;
    ae_int_t csize;
    ae_int_t j;
    double problemcost;
    mlpbuffers *buf2;
    ae_smart_ptr _buf2;
    ae_int_t len0;
    ae_int_t len1;
    mlpbuffers *pbuf;
    ae_smart_ptr _pbuf;
    smlpgrad *sgrad;
    ae_smart_ptr _sgrad;

    ae_frame_make(_state, &_frame_block);
    ae_smart_ptr_init(&_buf2, (void**)&buf2, _state);
    ae_smart_ptr_init(&_pbuf, (void**)&pbuf, _state);
    ae_smart_ptr_init(&_sgrad, (void**)&sgrad, _state);

    ae_assert(datasetsize>=0, "MLPGradBatchX: SetSize<0", _state);
    ae_assert(datasettype==0||datasettype==1, "MLPGradBatchX: DatasetType is incorrect", _state);
    ae_assert(subsettype==0||subsettype==1, "MLPGradBatchX: SubsetType is incorrect", _state);
    
    /*
     * Determine network and dataset properties
     */
    mlpproperties(network, &nin, &nout, &wcount, _state);
    if( mlpissoftmax(network, _state) )
    {
        rowsize = nin+1;
    }
    else
    {
        rowsize = nin+nout;
    }
    
    /*
     * Split problem.
     *
     * Splitting problem allows us to reduce  effect  of  single-precision
     * arithmetics (SSE-optimized version of MLPChunkedGradient uses single
     * precision  internally, but converts them to  double precision after
     * results are exported from HPC buffer to network). Small batches are
     * calculated in single precision, results are  aggregated  in  double
     * precision, and it allows us to avoid accumulation  of  errors  when
     * we process very large batches (tens of thousands of items).
     *
     * NOTE: it is important to use real arithmetics for ProblemCost
     *       because ProblemCost may be larger than MAXINT.
     */
    problemcost = (double)(subset1-subset0);
    problemcost = problemcost*wcount;
    if( subset1-subset0>=2*mlpbase_microbatchsize&&ae_fp_greater(problemcost,(double)(mlpbase_gradbasecasecost)) )
    {
        splitlength(subset1-subset0, mlpbase_microbatchsize, &len0, &len1, _state);
        mlpgradbatchx(network, densexy, sparsexy, datasetsize, datasettype, idx, subset0, subset0+len0, subsettype, buf, gradbuf, _state);
        mlpgradbatchx(network, densexy, sparsexy, datasetsize, datasettype, idx, subset0+len0, subset1, subsettype, buf, gradbuf, _state);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Chunked processing
     */
    ae_shared_pool_retrieve(gradbuf, &_sgrad, _state);
    ae_shared_pool_retrieve(buf, &_pbuf, _state);
    hpcpreparechunkedgradient(&network->weights, wcount, mlpntotal(network, _state), nin, nout, pbuf, _state);
    cstart = subset0;
    while(cstart<subset1)
    {
        
        /*
         * Determine size of current chunk and copy it to PBuf.XY
         */
        csize = ae_minint(subset1, cstart+pbuf->chunksize, _state)-cstart;
        for(j=0; j<=csize-1; j++)
        {
            srcidx = -1;
            if( subsettype==0 )
            {
                srcidx = cstart+j;
            }
            if( subsettype==1 )
            {
                srcidx = idx->ptr.p_int[cstart+j];
            }
            ae_assert(srcidx>=0, "MLPGradBatchX: internal error", _state);
            if( datasettype==0 )
            {
                ae_v_move(&pbuf->xy.ptr.pp_double[j][0], 1, &densexy->ptr.pp_double[srcidx][0], 1, ae_v_len(0,rowsize-1));
            }
            if( datasettype==1 )
            {
                sparsegetrow(sparsexy, srcidx, &pbuf->xyrow, _state);
                ae_v_move(&pbuf->xy.ptr.pp_double[j][0], 1, &pbuf->xyrow.ptr.p_double[0], 1, ae_v_len(0,rowsize-1));
            }
        }
        
        /*
         * Process chunk and advance line pointer
         */
        mlpbase_mlpchunkedgradient(network, &pbuf->xy, 0, csize, &pbuf->batch4buf, &pbuf->hpcbuf, &sgrad->f, ae_false, _state);
        cstart = cstart+pbuf->chunksize;
    }
    hpcfinalizechunkedgradient(pbuf, &sgrad->g, _state);
    ae_shared_pool_recycle(buf, &_pbuf, _state);
    ae_shared_pool_recycle(gradbuf, &_sgrad, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Batch gradient calculation for a set of inputs/outputs
(natural error function is used)

INPUT PARAMETERS:
    Network -   network initialized with one of the network creation funcs
    XY      -   set of inputs/outputs; one sample = one row;
                first NIn columns contain inputs,
                next NOut columns - desired outputs.
    SSize   -   number of elements in XY
    Grad    -   possibly preallocated array. If size of array is smaller
                than WCount, it will be reallocated. It is recommended to
                reuse previously allocated array to reduce allocation
                overhead.

OUTPUT PARAMETERS:
    E       -   error function, sum-of-squares for regression networks,
                cross-entropy for classification networks.
    Grad    -   gradient of E with respect to weights of network, array[WCount]

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlpgradnbatch(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t ssize,
     double* e,
     /* Real    */ ae_vector* grad,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    mlpbuffers *pbuf;
    ae_smart_ptr _pbuf;

    ae_frame_make(_state, &_frame_block);
    *e = 0;
    ae_smart_ptr_init(&_pbuf, (void**)&pbuf, _state);

    
    /*
     * Alloc
     */
    mlpproperties(network, &nin, &nout, &wcount, _state);
    ae_shared_pool_retrieve(&network->buf, &_pbuf, _state);
    hpcpreparechunkedgradient(&network->weights, wcount, mlpntotal(network, _state), nin, nout, pbuf, _state);
    rvectorsetlengthatleast(grad, wcount, _state);
    for(i=0; i<=wcount-1; i++)
    {
        grad->ptr.p_double[i] = (double)(0);
    }
    *e = (double)(0);
    i = 0;
    while(i<=ssize-1)
    {
        mlpbase_mlpchunkedgradient(network, xy, i, ae_minint(ssize, i+pbuf->chunksize, _state)-i, &pbuf->batch4buf, &pbuf->hpcbuf, e, ae_true, _state);
        i = i+pbuf->chunksize;
    }
    hpcfinalizechunkedgradient(pbuf, grad, _state);
    ae_shared_pool_recycle(&network->buf, &_pbuf, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Batch Hessian calculation (natural error function) using R-algorithm.
Internal subroutine.

  -- ALGLIB --
     Copyright 26.01.2008 by Bochkanov Sergey.
     
     Hessian calculation based on R-algorithm described in
     "Fast Exact Multiplication by the Hessian",
     B. A. Pearlmutter,
     Neural Computation, 1994.
*************************************************************************/
void mlphessiannbatch(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t ssize,
     double* e,
     /* Real    */ ae_vector* grad,
     /* Real    */ ae_matrix* h,
     ae_state *_state)
{

    *e = 0;

    mlpbase_mlphessianbatchinternal(network, xy, ssize, ae_true, e, grad, h, _state);
}


/*************************************************************************
Batch Hessian calculation using R-algorithm.
Internal subroutine.

  -- ALGLIB --
     Copyright 26.01.2008 by Bochkanov Sergey.

     Hessian calculation based on R-algorithm described in
     "Fast Exact Multiplication by the Hessian",
     B. A. Pearlmutter,
     Neural Computation, 1994.
*************************************************************************/
void mlphessianbatch(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t ssize,
     double* e,
     /* Real    */ ae_vector* grad,
     /* Real    */ ae_matrix* h,
     ae_state *_state)
{

    *e = 0;

    mlpbase_mlphessianbatchinternal(network, xy, ssize, ae_false, e, grad, h, _state);
}


/*************************************************************************
Internal subroutine, shouldn't be called by user.
*************************************************************************/
void mlpinternalprocessvector(/* Integer */ ae_vector* structinfo,
     /* Real    */ ae_vector* weights,
     /* Real    */ ae_vector* columnmeans,
     /* Real    */ ae_vector* columnsigmas,
     /* Real    */ ae_vector* neurons,
     /* Real    */ ae_vector* dfdnet,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t n1;
    ae_int_t n2;
    ae_int_t w1;
    ae_int_t w2;
    ae_int_t ntotal;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t istart;
    ae_int_t offs;
    double net;
    double f;
    double df;
    double d2f;
    double mx;
    ae_bool perr;


    
    /*
     * Read network geometry
     */
    nin = structinfo->ptr.p_int[1];
    nout = structinfo->ptr.p_int[2];
    ntotal = structinfo->ptr.p_int[3];
    istart = structinfo->ptr.p_int[5];
    
    /*
     * Inputs standartisation and putting in the network
     */
    for(i=0; i<=nin-1; i++)
    {
        if( ae_fp_neq(columnsigmas->ptr.p_double[i],(double)(0)) )
        {
            neurons->ptr.p_double[i] = (x->ptr.p_double[i]-columnmeans->ptr.p_double[i])/columnsigmas->ptr.p_double[i];
        }
        else
        {
            neurons->ptr.p_double[i] = x->ptr.p_double[i]-columnmeans->ptr.p_double[i];
        }
    }
    
    /*
     * Process network
     */
    for(i=0; i<=ntotal-1; i++)
    {
        offs = istart+i*mlpbase_nfieldwidth;
        if( structinfo->ptr.p_int[offs+0]>0||structinfo->ptr.p_int[offs+0]==-5 )
        {
            
            /*
             * Activation function
             */
            mlpactivationfunction(neurons->ptr.p_double[structinfo->ptr.p_int[offs+2]], structinfo->ptr.p_int[offs+0], &f, &df, &d2f, _state);
            neurons->ptr.p_double[i] = f;
            dfdnet->ptr.p_double[i] = df;
            continue;
        }
        if( structinfo->ptr.p_int[offs+0]==0 )
        {
            
            /*
             * Adaptive summator
             */
            n1 = structinfo->ptr.p_int[offs+2];
            n2 = n1+structinfo->ptr.p_int[offs+1]-1;
            w1 = structinfo->ptr.p_int[offs+3];
            w2 = w1+structinfo->ptr.p_int[offs+1]-1;
            net = ae_v_dotproduct(&weights->ptr.p_double[w1], 1, &neurons->ptr.p_double[n1], 1, ae_v_len(w1,w2));
            neurons->ptr.p_double[i] = net;
            dfdnet->ptr.p_double[i] = 1.0;
            touchint(&n2, _state);
            continue;
        }
        if( structinfo->ptr.p_int[offs+0]<0 )
        {
            perr = ae_true;
            if( structinfo->ptr.p_int[offs+0]==-2 )
            {
                
                /*
                 * input neuron, left unchanged
                 */
                perr = ae_false;
            }
            if( structinfo->ptr.p_int[offs+0]==-3 )
            {
                
                /*
                 * "-1" neuron
                 */
                neurons->ptr.p_double[i] = (double)(-1);
                perr = ae_false;
            }
            if( structinfo->ptr.p_int[offs+0]==-4 )
            {
                
                /*
                 * "0" neuron
                 */
                neurons->ptr.p_double[i] = (double)(0);
                perr = ae_false;
            }
            ae_assert(!perr, "MLPInternalProcessVector: internal error - unknown neuron type!", _state);
            continue;
        }
    }
    
    /*
     * Extract result
     */
    ae_v_move(&y->ptr.p_double[0], 1, &neurons->ptr.p_double[ntotal-nout], 1, ae_v_len(0,nout-1));
    
    /*
     * Softmax post-processing or standardisation if needed
     */
    ae_assert(structinfo->ptr.p_int[6]==0||structinfo->ptr.p_int[6]==1, "MLPInternalProcessVector: unknown normalization type!", _state);
    if( structinfo->ptr.p_int[6]==1 )
    {
        
        /*
         * Softmax
         */
        mx = y->ptr.p_double[0];
        for(i=1; i<=nout-1; i++)
        {
            mx = ae_maxreal(mx, y->ptr.p_double[i], _state);
        }
        net = (double)(0);
        for(i=0; i<=nout-1; i++)
        {
            y->ptr.p_double[i] = ae_exp(y->ptr.p_double[i]-mx, _state);
            net = net+y->ptr.p_double[i];
        }
        for(i=0; i<=nout-1; i++)
        {
            y->ptr.p_double[i] = y->ptr.p_double[i]/net;
        }
    }
    else
    {
        
        /*
         * Standardisation
         */
        for(i=0; i<=nout-1; i++)
        {
            y->ptr.p_double[i] = y->ptr.p_double[i]*columnsigmas->ptr.p_double[nin+i]+columnmeans->ptr.p_double[nin+i];
        }
    }
}


/*************************************************************************
Serializer: allocation

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void mlpalloc(ae_serializer* s,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t fkind;
    double threshold;
    double v0;
    double v1;
    ae_int_t nin;
    ae_int_t nout;


    nin = network->hllayersizes.ptr.p_int[0];
    nout = network->hllayersizes.ptr.p_int[network->hllayersizes.cnt-1];
    ae_serializer_alloc_entry(s);
    ae_serializer_alloc_entry(s);
    ae_serializer_alloc_entry(s);
    allocintegerarray(s, &network->hllayersizes, -1, _state);
    for(i=1; i<=network->hllayersizes.cnt-1; i++)
    {
        for(j=0; j<=network->hllayersizes.ptr.p_int[i]-1; j++)
        {
            mlpgetneuroninfo(network, i, j, &fkind, &threshold, _state);
            ae_serializer_alloc_entry(s);
            ae_serializer_alloc_entry(s);
            for(k=0; k<=network->hllayersizes.ptr.p_int[i-1]-1; k++)
            {
                ae_serializer_alloc_entry(s);
            }
        }
    }
    for(j=0; j<=nin-1; j++)
    {
        mlpgetinputscaling(network, j, &v0, &v1, _state);
        ae_serializer_alloc_entry(s);
        ae_serializer_alloc_entry(s);
    }
    for(j=0; j<=nout-1; j++)
    {
        mlpgetoutputscaling(network, j, &v0, &v1, _state);
        ae_serializer_alloc_entry(s);
        ae_serializer_alloc_entry(s);
    }
}


/*************************************************************************
Serializer: serialization

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void mlpserialize(ae_serializer* s,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t fkind;
    double threshold;
    double v0;
    double v1;
    ae_int_t nin;
    ae_int_t nout;


    nin = network->hllayersizes.ptr.p_int[0];
    nout = network->hllayersizes.ptr.p_int[network->hllayersizes.cnt-1];
    ae_serializer_serialize_int(s, getmlpserializationcode(_state), _state);
    ae_serializer_serialize_int(s, mlpbase_mlpfirstversion, _state);
    ae_serializer_serialize_bool(s, mlpissoftmax(network, _state), _state);
    serializeintegerarray(s, &network->hllayersizes, -1, _state);
    for(i=1; i<=network->hllayersizes.cnt-1; i++)
    {
        for(j=0; j<=network->hllayersizes.ptr.p_int[i]-1; j++)
        {
            mlpgetneuroninfo(network, i, j, &fkind, &threshold, _state);
            ae_serializer_serialize_int(s, fkind, _state);
            ae_serializer_serialize_double(s, threshold, _state);
            for(k=0; k<=network->hllayersizes.ptr.p_int[i-1]-1; k++)
            {
                ae_serializer_serialize_double(s, mlpgetweight(network, i-1, k, i, j, _state), _state);
            }
        }
    }
    for(j=0; j<=nin-1; j++)
    {
        mlpgetinputscaling(network, j, &v0, &v1, _state);
        ae_serializer_serialize_double(s, v0, _state);
        ae_serializer_serialize_double(s, v1, _state);
    }
    for(j=0; j<=nout-1; j++)
    {
        mlpgetoutputscaling(network, j, &v0, &v1, _state);
        ae_serializer_serialize_double(s, v0, _state);
        ae_serializer_serialize_double(s, v1, _state);
    }
}


/*************************************************************************
Serializer: unserialization

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void mlpunserialize(ae_serializer* s,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i0;
    ae_int_t i1;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t fkind;
    double threshold;
    double v0;
    double v1;
    ae_int_t nin;
    ae_int_t nout;
    ae_bool issoftmax;
    ae_vector layersizes;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_clear(network);
    ae_vector_init(&layersizes, 0, DT_INT, _state);

    
    /*
     * check correctness of header
     */
    ae_serializer_unserialize_int(s, &i0, _state);
    ae_assert(i0==getmlpserializationcode(_state), "MLPUnserialize: stream header corrupted", _state);
    ae_serializer_unserialize_int(s, &i1, _state);
    ae_assert(i1==mlpbase_mlpfirstversion, "MLPUnserialize: stream header corrupted", _state);
    
    /*
     * Create network
     */
    ae_serializer_unserialize_bool(s, &issoftmax, _state);
    unserializeintegerarray(s, &layersizes, _state);
    ae_assert((layersizes.cnt==2||layersizes.cnt==3)||layersizes.cnt==4, "MLPUnserialize: too many hidden layers!", _state);
    nin = layersizes.ptr.p_int[0];
    nout = layersizes.ptr.p_int[layersizes.cnt-1];
    if( layersizes.cnt==2 )
    {
        if( issoftmax )
        {
            mlpcreatec0(layersizes.ptr.p_int[0], layersizes.ptr.p_int[1], network, _state);
        }
        else
        {
            mlpcreate0(layersizes.ptr.p_int[0], layersizes.ptr.p_int[1], network, _state);
        }
    }
    if( layersizes.cnt==3 )
    {
        if( issoftmax )
        {
            mlpcreatec1(layersizes.ptr.p_int[0], layersizes.ptr.p_int[1], layersizes.ptr.p_int[2], network, _state);
        }
        else
        {
            mlpcreate1(layersizes.ptr.p_int[0], layersizes.ptr.p_int[1], layersizes.ptr.p_int[2], network, _state);
        }
    }
    if( layersizes.cnt==4 )
    {
        if( issoftmax )
        {
            mlpcreatec2(layersizes.ptr.p_int[0], layersizes.ptr.p_int[1], layersizes.ptr.p_int[2], layersizes.ptr.p_int[3], network, _state);
        }
        else
        {
            mlpcreate2(layersizes.ptr.p_int[0], layersizes.ptr.p_int[1], layersizes.ptr.p_int[2], layersizes.ptr.p_int[3], network, _state);
        }
    }
    
    /*
     * Load neurons and weights
     */
    for(i=1; i<=layersizes.cnt-1; i++)
    {
        for(j=0; j<=layersizes.ptr.p_int[i]-1; j++)
        {
            ae_serializer_unserialize_int(s, &fkind, _state);
            ae_serializer_unserialize_double(s, &threshold, _state);
            mlpsetneuroninfo(network, i, j, fkind, threshold, _state);
            for(k=0; k<=layersizes.ptr.p_int[i-1]-1; k++)
            {
                ae_serializer_unserialize_double(s, &v0, _state);
                mlpsetweight(network, i-1, k, i, j, v0, _state);
            }
        }
    }
    
    /*
     * Load standartizator
     */
    for(j=0; j<=nin-1; j++)
    {
        ae_serializer_unserialize_double(s, &v0, _state);
        ae_serializer_unserialize_double(s, &v1, _state);
        mlpsetinputscaling(network, j, v0, v1, _state);
    }
    for(j=0; j<=nout-1; j++)
    {
        ae_serializer_unserialize_double(s, &v0, _state);
        ae_serializer_unserialize_double(s, &v1, _state);
        mlpsetoutputscaling(network, j, v0, v1, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Calculation of all types of errors on subset of dataset.

FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network -   network initialized with one of the network creation funcs
    XY      -   original dataset; one sample = one row;
                first NIn columns contain inputs,
                next NOut columns - desired outputs.
    SetSize -   real size of XY, SetSize>=0;
    Subset  -   subset of SubsetSize elements, array[SubsetSize];
    SubsetSize- number of elements in Subset[] array:
                * if SubsetSize>0, rows of XY with indices Subset[0]...
                  ...Subset[SubsetSize-1] are processed
                * if SubsetSize=0, zeros are returned
                * if SubsetSize<0, entire dataset is  processed;  Subset[]
                  array is ignored in this case.

OUTPUT PARAMETERS:
    Rep     -   it contains all type of errors.

  -- ALGLIB --
     Copyright 04.09.2012 by Bochkanov Sergey
*************************************************************************/
void mlpallerrorssubset(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t setsize,
     /* Integer */ ae_vector* subset,
     ae_int_t subsetsize,
     modelerrors* rep,
     ae_state *_state)
{
    ae_int_t idx0;
    ae_int_t idx1;
    ae_int_t idxtype;

    _modelerrors_clear(rep);

    ae_assert(xy->rows>=setsize, "MLPAllErrorsSubset: XY has less than SetSize rows", _state);
    if( setsize>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+1, "MLPAllErrorsSubset: XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPAllErrorsSubset: XY has less than NIn+NOut columns", _state);
        }
    }
    if( subsetsize>=0 )
    {
        idx0 = 0;
        idx1 = subsetsize;
        idxtype = 1;
    }
    else
    {
        idx0 = 0;
        idx1 = setsize;
        idxtype = 0;
    }
    mlpallerrorsx(network, xy, &network->dummysxy, setsize, 0, subset, idx0, idx1, idxtype, &network->buf, rep, _state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_mlpallerrorssubset(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t setsize,
    /* Integer */ ae_vector* subset,
    ae_int_t subsetsize,
    modelerrors* rep, ae_state *_state)
{
    mlpallerrorssubset(network,xy,setsize,subset,subsetsize,rep, _state);
}


/*************************************************************************
Calculation of all types of errors on subset of dataset.

FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network -   network initialized with one of the network creation funcs
    XY      -   original dataset given by sparse matrix;
                one sample = one row;
                first NIn columns contain inputs,
                next NOut columns - desired outputs.
    SetSize -   real size of XY, SetSize>=0;
    Subset  -   subset of SubsetSize elements, array[SubsetSize];
    SubsetSize- number of elements in Subset[] array:
                * if SubsetSize>0, rows of XY with indices Subset[0]...
                  ...Subset[SubsetSize-1] are processed
                * if SubsetSize=0, zeros are returned
                * if SubsetSize<0, entire dataset is  processed;  Subset[]
                  array is ignored in this case.

OUTPUT PARAMETERS:
    Rep     -   it contains all type of errors.


  -- ALGLIB --
     Copyright 04.09.2012 by Bochkanov Sergey
*************************************************************************/
void mlpallerrorssparsesubset(multilayerperceptron* network,
     sparsematrix* xy,
     ae_int_t setsize,
     /* Integer */ ae_vector* subset,
     ae_int_t subsetsize,
     modelerrors* rep,
     ae_state *_state)
{
    ae_int_t idx0;
    ae_int_t idx1;
    ae_int_t idxtype;

    _modelerrors_clear(rep);

    ae_assert(sparseiscrs(xy, _state), "MLPAllErrorsSparseSubset: XY is not in CRS format.", _state);
    ae_assert(sparsegetnrows(xy, _state)>=setsize, "MLPAllErrorsSparseSubset: XY has less than SetSize rows", _state);
    if( setsize>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(sparsegetncols(xy, _state)>=mlpgetinputscount(network, _state)+1, "MLPAllErrorsSparseSubset: XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(sparsegetncols(xy, _state)>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPAllErrorsSparseSubset: XY has less than NIn+NOut columns", _state);
        }
    }
    if( subsetsize>=0 )
    {
        idx0 = 0;
        idx1 = subsetsize;
        idxtype = 1;
    }
    else
    {
        idx0 = 0;
        idx1 = setsize;
        idxtype = 0;
    }
    mlpallerrorsx(network, &network->dummydxy, xy, setsize, 1, subset, idx0, idx1, idxtype, &network->buf, rep, _state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_mlpallerrorssparsesubset(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t setsize,
    /* Integer */ ae_vector* subset,
    ae_int_t subsetsize,
    modelerrors* rep, ae_state *_state)
{
    mlpallerrorssparsesubset(network,xy,setsize,subset,subsetsize,rep, _state);
}


/*************************************************************************
Error of the neural network on subset of dataset.


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network   -     neural network;
    XY        -     training  set,  see  below  for  information  on   the
                    training set format;
    SetSize   -     real size of XY, SetSize>=0;
    Subset    -     subset of SubsetSize elements, array[SubsetSize];
    SubsetSize-     number of elements in Subset[] array:
                    * if SubsetSize>0, rows of XY with indices Subset[0]...
                      ...Subset[SubsetSize-1] are processed
                    * if SubsetSize=0, zeros are returned
                    * if SubsetSize<0, entire dataset is  processed;  Subset[]
                      array is ignored in this case.

RESULT:
    sum-of-squares error, SUM(sqr(y[i]-desired_y[i])/2)

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
dataset format is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).

  -- ALGLIB --
     Copyright 04.09.2012 by Bochkanov Sergey
*************************************************************************/
double mlperrorsubset(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t setsize,
     /* Integer */ ae_vector* subset,
     ae_int_t subsetsize,
     ae_state *_state)
{
    ae_int_t idx0;
    ae_int_t idx1;
    ae_int_t idxtype;
    double result;


    ae_assert(xy->rows>=setsize, "MLPErrorSubset: XY has less than SetSize rows", _state);
    if( setsize>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+1, "MLPErrorSubset: XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(xy->cols>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPErrorSubset: XY has less than NIn+NOut columns", _state);
        }
    }
    if( subsetsize>=0 )
    {
        idx0 = 0;
        idx1 = subsetsize;
        idxtype = 1;
    }
    else
    {
        idx0 = 0;
        idx1 = setsize;
        idxtype = 0;
    }
    mlpallerrorsx(network, xy, &network->dummysxy, setsize, 0, subset, idx0, idx1, idxtype, &network->buf, &network->err, _state);
    result = ae_sqr(network->err.rmserror, _state)*(idx1-idx0)*mlpgetoutputscount(network, _state)/2;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
double _pexec_mlperrorsubset(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t setsize,
    /* Integer */ ae_vector* subset,
    ae_int_t subsetsize, ae_state *_state)
{
    return mlperrorsubset(network,xy,setsize,subset,subsetsize, _state);
}


/*************************************************************************
Error of the neural network on subset of sparse dataset.


FOR USERS OF COMMERCIAL EDITION:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function:
  ! * multicore support (C++ and C# computational cores)
  ! * SSE support 
  !
  ! First improvement gives close-to-linear speedup on multicore  systems.
  ! Second improvement gives constant speedup (2-3x depending on your CPU)
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
    Network   -     neural network;
    XY        -     training  set,  see  below  for  information  on   the
                    training set format. This function checks  correctness
                    of  the  dataset  (no  NANs/INFs,  class  numbers  are
                    correct) and throws exception when  incorrect  dataset
                    is passed.  Sparse  matrix  must  use  CRS  format for
                    storage.
    SetSize   -     real size of XY, SetSize>=0;
                    it is used when SubsetSize<0;
    Subset    -     subset of SubsetSize elements, array[SubsetSize];
    SubsetSize-     number of elements in Subset[] array:
                    * if SubsetSize>0, rows of XY with indices Subset[0]...
                      ...Subset[SubsetSize-1] are processed
                    * if SubsetSize=0, zeros are returned
                    * if SubsetSize<0, entire dataset is  processed;  Subset[]
                      array is ignored in this case.

RESULT:
    sum-of-squares error, SUM(sqr(y[i]-desired_y[i])/2)

DATASET FORMAT:

This  function  uses  two  different  dataset formats - one for regression
networks, another one for classification networks.

For regression networks with NIn inputs and NOut outputs following dataset
format is used:
* dataset is given by NPoints*(NIn+NOut) matrix
* each row corresponds to one example
* first NIn columns are inputs, next NOut columns are outputs

For classification networks with NIn inputs and NClasses clases  following
dataset format is used:
* dataset is given by NPoints*(NIn+1) matrix
* each row corresponds to one example
* first NIn columns are inputs, last column stores class number (from 0 to
  NClasses-1).

  -- ALGLIB --
     Copyright 04.09.2012 by Bochkanov Sergey
*************************************************************************/
double mlperrorsparsesubset(multilayerperceptron* network,
     sparsematrix* xy,
     ae_int_t setsize,
     /* Integer */ ae_vector* subset,
     ae_int_t subsetsize,
     ae_state *_state)
{
    ae_int_t idx0;
    ae_int_t idx1;
    ae_int_t idxtype;
    double result;


    ae_assert(sparseiscrs(xy, _state), "MLPErrorSparseSubset: XY is not in CRS format.", _state);
    ae_assert(sparsegetnrows(xy, _state)>=setsize, "MLPErrorSparseSubset: XY has less than SetSize rows", _state);
    if( setsize>0 )
    {
        if( mlpissoftmax(network, _state) )
        {
            ae_assert(sparsegetncols(xy, _state)>=mlpgetinputscount(network, _state)+1, "MLPErrorSparseSubset: XY has less than NIn+1 columns", _state);
        }
        else
        {
            ae_assert(sparsegetncols(xy, _state)>=mlpgetinputscount(network, _state)+mlpgetoutputscount(network, _state), "MLPErrorSparseSubset: XY has less than NIn+NOut columns", _state);
        }
    }
    if( subsetsize>=0 )
    {
        idx0 = 0;
        idx1 = subsetsize;
        idxtype = 1;
    }
    else
    {
        idx0 = 0;
        idx1 = setsize;
        idxtype = 0;
    }
    mlpallerrorsx(network, &network->dummydxy, xy, setsize, 1, subset, idx0, idx1, idxtype, &network->buf, &network->err, _state);
    result = ae_sqr(network->err.rmserror, _state)*(idx1-idx0)*mlpgetoutputscount(network, _state)/2;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
double _pexec_mlperrorsparsesubset(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t setsize,
    /* Integer */ ae_vector* subset,
    ae_int_t subsetsize, ae_state *_state)
{
    return mlperrorsparsesubset(network,xy,setsize,subset,subsetsize, _state);
}


/*************************************************************************
Calculation of all types of errors at once for a subset or  full  dataset,
which can be represented in different formats.

THIS INTERNAL FUNCTION IS NOT INTENDED TO BE USED BY ALGLIB USERS!

  -- ALGLIB --
     Copyright 26.07.2012 by Bochkanov Sergey
*************************************************************************/
void mlpallerrorsx(multilayerperceptron* network,
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
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t rowsize;
    ae_bool iscls;
    ae_int_t srcidx;
    ae_int_t cstart;
    ae_int_t csize;
    ae_int_t j;
    mlpbuffers *pbuf;
    ae_smart_ptr _pbuf;
    ae_int_t len0;
    ae_int_t len1;
    modelerrors rep0;
    modelerrors rep1;

    ae_frame_make(_state, &_frame_block);
    ae_smart_ptr_init(&_pbuf, (void**)&pbuf, _state);
    _modelerrors_init(&rep0, _state);
    _modelerrors_init(&rep1, _state);

    ae_assert(datasetsize>=0, "MLPAllErrorsX: SetSize<0", _state);
    ae_assert(datasettype==0||datasettype==1, "MLPAllErrorsX: DatasetType is incorrect", _state);
    ae_assert(subsettype==0||subsettype==1, "MLPAllErrorsX: SubsetType is incorrect", _state);
    
    /*
     * Determine network properties
     */
    mlpproperties(network, &nin, &nout, &wcount, _state);
    iscls = mlpissoftmax(network, _state);
    
    /*
     * Split problem.
     *
     * Splitting problem allows us to reduce  effect  of  single-precision
     * arithmetics (SSE-optimized version of MLPChunkedProcess uses single
     * precision  internally, but converts them to  double precision after
     * results are exported from HPC buffer to network). Small batches are
     * calculated in single precision, results are  aggregated  in  double
     * precision, and it allows us to avoid accumulation  of  errors  when
     * we process very large batches (tens of thousands of items).
     *
     * NOTE: it is important to use real arithmetics for ProblemCost
     *       because ProblemCost may be larger than MAXINT.
     */
    if( subset1-subset0>=2*mlpbase_microbatchsize&&ae_fp_greater(inttoreal(subset1-subset0, _state)*inttoreal(wcount, _state),(double)(mlpbase_gradbasecasecost)) )
    {
        splitlength(subset1-subset0, mlpbase_microbatchsize, &len0, &len1, _state);
        mlpallerrorsx(network, densexy, sparsexy, datasetsize, datasettype, idx, subset0, subset0+len0, subsettype, buf, &rep0, _state);
        mlpallerrorsx(network, densexy, sparsexy, datasetsize, datasettype, idx, subset0+len0, subset1, subsettype, buf, &rep1, _state);
        rep->relclserror = (len0*rep0.relclserror+len1*rep1.relclserror)/(len0+len1);
        rep->avgce = (len0*rep0.avgce+len1*rep1.avgce)/(len0+len1);
        rep->rmserror = ae_sqrt((len0*ae_sqr(rep0.rmserror, _state)+len1*ae_sqr(rep1.rmserror, _state))/(len0+len1), _state);
        rep->avgerror = (len0*rep0.avgerror+len1*rep1.avgerror)/(len0+len1);
        rep->avgrelerror = (len0*rep0.avgrelerror+len1*rep1.avgrelerror)/(len0+len1);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Retrieve and prepare
     */
    ae_shared_pool_retrieve(buf, &_pbuf, _state);
    if( iscls )
    {
        rowsize = nin+1;
        dserrallocate(nout, &pbuf->tmp0, _state);
    }
    else
    {
        rowsize = nin+nout;
        dserrallocate(-nout, &pbuf->tmp0, _state);
    }
    
    /*
     * Processing
     */
    hpcpreparechunkedgradient(&network->weights, wcount, mlpntotal(network, _state), nin, nout, pbuf, _state);
    cstart = subset0;
    while(cstart<subset1)
    {
        
        /*
         * Determine size of current chunk and copy it to PBuf.XY
         */
        csize = ae_minint(subset1, cstart+pbuf->chunksize, _state)-cstart;
        for(j=0; j<=csize-1; j++)
        {
            srcidx = -1;
            if( subsettype==0 )
            {
                srcidx = cstart+j;
            }
            if( subsettype==1 )
            {
                srcidx = idx->ptr.p_int[cstart+j];
            }
            ae_assert(srcidx>=0, "MLPAllErrorsX: internal error", _state);
            if( datasettype==0 )
            {
                ae_v_move(&pbuf->xy.ptr.pp_double[j][0], 1, &densexy->ptr.pp_double[srcidx][0], 1, ae_v_len(0,rowsize-1));
            }
            if( datasettype==1 )
            {
                sparsegetrow(sparsexy, srcidx, &pbuf->xyrow, _state);
                ae_v_move(&pbuf->xy.ptr.pp_double[j][0], 1, &pbuf->xyrow.ptr.p_double[0], 1, ae_v_len(0,rowsize-1));
            }
        }
        
        /*
         * Unpack XY and process (temporary code, to be replaced by chunked processing)
         */
        for(j=0; j<=csize-1; j++)
        {
            ae_v_move(&pbuf->xy2.ptr.pp_double[j][0], 1, &pbuf->xy.ptr.pp_double[j][0], 1, ae_v_len(0,rowsize-1));
        }
        mlpbase_mlpchunkedprocess(network, &pbuf->xy2, 0, csize, &pbuf->batch4buf, &pbuf->hpcbuf, _state);
        for(j=0; j<=csize-1; j++)
        {
            ae_v_move(&pbuf->x.ptr.p_double[0], 1, &pbuf->xy2.ptr.pp_double[j][0], 1, ae_v_len(0,nin-1));
            ae_v_move(&pbuf->y.ptr.p_double[0], 1, &pbuf->xy2.ptr.pp_double[j][nin], 1, ae_v_len(0,nout-1));
            if( iscls )
            {
                pbuf->desiredy.ptr.p_double[0] = pbuf->xy.ptr.pp_double[j][nin];
            }
            else
            {
                ae_v_move(&pbuf->desiredy.ptr.p_double[0], 1, &pbuf->xy.ptr.pp_double[j][nin], 1, ae_v_len(0,nout-1));
            }
            dserraccumulate(&pbuf->tmp0, &pbuf->y, &pbuf->desiredy, _state);
        }
        
        /*
         * Process chunk and advance line pointer
         */
        cstart = cstart+pbuf->chunksize;
    }
    dserrfinish(&pbuf->tmp0, _state);
    rep->relclserror = pbuf->tmp0.ptr.p_double[0];
    rep->avgce = pbuf->tmp0.ptr.p_double[1]/ae_log((double)(2), _state);
    rep->rmserror = pbuf->tmp0.ptr.p_double[2];
    rep->avgerror = pbuf->tmp0.ptr.p_double[3];
    rep->avgrelerror = pbuf->tmp0.ptr.p_double[4];
    
    /*
     * Recycle
     */
    ae_shared_pool_recycle(buf, &_pbuf, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine: adding new input layer to network
*************************************************************************/
static void mlpbase_addinputlayer(ae_int_t ncount,
     /* Integer */ ae_vector* lsizes,
     /* Integer */ ae_vector* ltypes,
     /* Integer */ ae_vector* lconnfirst,
     /* Integer */ ae_vector* lconnlast,
     ae_int_t* lastproc,
     ae_state *_state)
{


    lsizes->ptr.p_int[0] = ncount;
    ltypes->ptr.p_int[0] = -2;
    lconnfirst->ptr.p_int[0] = 0;
    lconnlast->ptr.p_int[0] = 0;
    *lastproc = 0;
}


/*************************************************************************
Internal subroutine: adding new summator layer to network
*************************************************************************/
static void mlpbase_addbiasedsummatorlayer(ae_int_t ncount,
     /* Integer */ ae_vector* lsizes,
     /* Integer */ ae_vector* ltypes,
     /* Integer */ ae_vector* lconnfirst,
     /* Integer */ ae_vector* lconnlast,
     ae_int_t* lastproc,
     ae_state *_state)
{


    lsizes->ptr.p_int[*lastproc+1] = 1;
    ltypes->ptr.p_int[*lastproc+1] = -3;
    lconnfirst->ptr.p_int[*lastproc+1] = 0;
    lconnlast->ptr.p_int[*lastproc+1] = 0;
    lsizes->ptr.p_int[*lastproc+2] = ncount;
    ltypes->ptr.p_int[*lastproc+2] = 0;
    lconnfirst->ptr.p_int[*lastproc+2] = *lastproc;
    lconnlast->ptr.p_int[*lastproc+2] = *lastproc+1;
    *lastproc = *lastproc+2;
}


/*************************************************************************
Internal subroutine: adding new summator layer to network
*************************************************************************/
static void mlpbase_addactivationlayer(ae_int_t functype,
     /* Integer */ ae_vector* lsizes,
     /* Integer */ ae_vector* ltypes,
     /* Integer */ ae_vector* lconnfirst,
     /* Integer */ ae_vector* lconnlast,
     ae_int_t* lastproc,
     ae_state *_state)
{


    ae_assert(functype>0||functype==-5, "AddActivationLayer: incorrect function type", _state);
    lsizes->ptr.p_int[*lastproc+1] = lsizes->ptr.p_int[*lastproc];
    ltypes->ptr.p_int[*lastproc+1] = functype;
    lconnfirst->ptr.p_int[*lastproc+1] = *lastproc;
    lconnlast->ptr.p_int[*lastproc+1] = *lastproc;
    *lastproc = *lastproc+1;
}


/*************************************************************************
Internal subroutine: adding new zero layer to network
*************************************************************************/
static void mlpbase_addzerolayer(/* Integer */ ae_vector* lsizes,
     /* Integer */ ae_vector* ltypes,
     /* Integer */ ae_vector* lconnfirst,
     /* Integer */ ae_vector* lconnlast,
     ae_int_t* lastproc,
     ae_state *_state)
{


    lsizes->ptr.p_int[*lastproc+1] = 1;
    ltypes->ptr.p_int[*lastproc+1] = -4;
    lconnfirst->ptr.p_int[*lastproc+1] = 0;
    lconnlast->ptr.p_int[*lastproc+1] = 0;
    *lastproc = *lastproc+1;
}


/*************************************************************************
This routine adds input layer to the high-level description of the network.

It modifies Network.HLConnections and Network.HLNeurons  and  assumes that
these  arrays  have  enough  place  to  store  data.  It accepts following
parameters:
    Network     -   network
    ConnIdx     -   index of the first free entry in the HLConnections
    NeuroIdx    -   index of the first free entry in the HLNeurons
    StructInfoIdx-  index of the first entry in the low level description
                    of the current layer (in the StructInfo array)
    NIn         -   number of inputs
                    
It modified Network and indices.
*************************************************************************/
static void mlpbase_hladdinputlayer(multilayerperceptron* network,
     ae_int_t* connidx,
     ae_int_t* neuroidx,
     ae_int_t* structinfoidx,
     ae_int_t nin,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t offs;


    offs = mlpbase_hlnfieldwidth*(*neuroidx);
    for(i=0; i<=nin-1; i++)
    {
        network->hlneurons.ptr.p_int[offs+0] = 0;
        network->hlneurons.ptr.p_int[offs+1] = i;
        network->hlneurons.ptr.p_int[offs+2] = -1;
        network->hlneurons.ptr.p_int[offs+3] = -1;
        offs = offs+mlpbase_hlnfieldwidth;
    }
    *neuroidx = *neuroidx+nin;
    *structinfoidx = *structinfoidx+nin;
}


/*************************************************************************
This routine adds output layer to the high-level description of
the network.

It modifies Network.HLConnections and Network.HLNeurons  and  assumes that
these  arrays  have  enough  place  to  store  data.  It accepts following
parameters:
    Network     -   network
    ConnIdx     -   index of the first free entry in the HLConnections
    NeuroIdx    -   index of the first free entry in the HLNeurons
    StructInfoIdx-  index of the first entry in the low level description
                    of the current layer (in the StructInfo array)
    WeightsIdx  -   index of the first entry in the Weights array which
                    corresponds to the current layer
    K           -   current layer index
    NPrev       -   number of neurons in the previous layer
    NOut        -   number of outputs
    IsCls       -   is it classifier network?
    IsLinear    -   is it network with linear output?

It modified Network and ConnIdx/NeuroIdx/StructInfoIdx/WeightsIdx.
*************************************************************************/
static void mlpbase_hladdoutputlayer(multilayerperceptron* network,
     ae_int_t* connidx,
     ae_int_t* neuroidx,
     ae_int_t* structinfoidx,
     ae_int_t* weightsidx,
     ae_int_t k,
     ae_int_t nprev,
     ae_int_t nout,
     ae_bool iscls,
     ae_bool islinearout,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t neurooffs;
    ae_int_t connoffs;


    ae_assert((iscls&&islinearout)||!iscls, "HLAddOutputLayer: internal error", _state);
    neurooffs = mlpbase_hlnfieldwidth*(*neuroidx);
    connoffs = mlpbase_hlconnfieldwidth*(*connidx);
    if( !iscls )
    {
        
        /*
         * Regression network
         */
        for(i=0; i<=nout-1; i++)
        {
            network->hlneurons.ptr.p_int[neurooffs+0] = k;
            network->hlneurons.ptr.p_int[neurooffs+1] = i;
            network->hlneurons.ptr.p_int[neurooffs+2] = *structinfoidx+1+nout+i;
            network->hlneurons.ptr.p_int[neurooffs+3] = *weightsidx+nprev+(nprev+1)*i;
            neurooffs = neurooffs+mlpbase_hlnfieldwidth;
        }
        for(i=0; i<=nprev-1; i++)
        {
            for(j=0; j<=nout-1; j++)
            {
                network->hlconnections.ptr.p_int[connoffs+0] = k-1;
                network->hlconnections.ptr.p_int[connoffs+1] = i;
                network->hlconnections.ptr.p_int[connoffs+2] = k;
                network->hlconnections.ptr.p_int[connoffs+3] = j;
                network->hlconnections.ptr.p_int[connoffs+4] = *weightsidx+i+j*(nprev+1);
                connoffs = connoffs+mlpbase_hlconnfieldwidth;
            }
        }
        *connidx = *connidx+nprev*nout;
        *neuroidx = *neuroidx+nout;
        *structinfoidx = *structinfoidx+2*nout+1;
        *weightsidx = *weightsidx+nout*(nprev+1);
    }
    else
    {
        
        /*
         * Classification network
         */
        for(i=0; i<=nout-2; i++)
        {
            network->hlneurons.ptr.p_int[neurooffs+0] = k;
            network->hlneurons.ptr.p_int[neurooffs+1] = i;
            network->hlneurons.ptr.p_int[neurooffs+2] = -1;
            network->hlneurons.ptr.p_int[neurooffs+3] = *weightsidx+nprev+(nprev+1)*i;
            neurooffs = neurooffs+mlpbase_hlnfieldwidth;
        }
        network->hlneurons.ptr.p_int[neurooffs+0] = k;
        network->hlneurons.ptr.p_int[neurooffs+1] = i;
        network->hlneurons.ptr.p_int[neurooffs+2] = -1;
        network->hlneurons.ptr.p_int[neurooffs+3] = -1;
        for(i=0; i<=nprev-1; i++)
        {
            for(j=0; j<=nout-2; j++)
            {
                network->hlconnections.ptr.p_int[connoffs+0] = k-1;
                network->hlconnections.ptr.p_int[connoffs+1] = i;
                network->hlconnections.ptr.p_int[connoffs+2] = k;
                network->hlconnections.ptr.p_int[connoffs+3] = j;
                network->hlconnections.ptr.p_int[connoffs+4] = *weightsidx+i+j*(nprev+1);
                connoffs = connoffs+mlpbase_hlconnfieldwidth;
            }
        }
        *connidx = *connidx+nprev*(nout-1);
        *neuroidx = *neuroidx+nout;
        *structinfoidx = *structinfoidx+nout+2;
        *weightsidx = *weightsidx+(nout-1)*(nprev+1);
    }
}


/*************************************************************************
This routine adds hidden layer to the high-level description of
the network.

It modifies Network.HLConnections and Network.HLNeurons  and  assumes that
these  arrays  have  enough  place  to  store  data.  It accepts following
parameters:
    Network     -   network
    ConnIdx     -   index of the first free entry in the HLConnections
    NeuroIdx    -   index of the first free entry in the HLNeurons
    StructInfoIdx-  index of the first entry in the low level description
                    of the current layer (in the StructInfo array)
    WeightsIdx  -   index of the first entry in the Weights array which
                    corresponds to the current layer
    K           -   current layer index
    NPrev       -   number of neurons in the previous layer
    NCur        -   number of neurons in the current layer

It modified Network and ConnIdx/NeuroIdx/StructInfoIdx/WeightsIdx.
*************************************************************************/
static void mlpbase_hladdhiddenlayer(multilayerperceptron* network,
     ae_int_t* connidx,
     ae_int_t* neuroidx,
     ae_int_t* structinfoidx,
     ae_int_t* weightsidx,
     ae_int_t k,
     ae_int_t nprev,
     ae_int_t ncur,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t neurooffs;
    ae_int_t connoffs;


    neurooffs = mlpbase_hlnfieldwidth*(*neuroidx);
    connoffs = mlpbase_hlconnfieldwidth*(*connidx);
    for(i=0; i<=ncur-1; i++)
    {
        network->hlneurons.ptr.p_int[neurooffs+0] = k;
        network->hlneurons.ptr.p_int[neurooffs+1] = i;
        network->hlneurons.ptr.p_int[neurooffs+2] = *structinfoidx+1+ncur+i;
        network->hlneurons.ptr.p_int[neurooffs+3] = *weightsidx+nprev+(nprev+1)*i;
        neurooffs = neurooffs+mlpbase_hlnfieldwidth;
    }
    for(i=0; i<=nprev-1; i++)
    {
        for(j=0; j<=ncur-1; j++)
        {
            network->hlconnections.ptr.p_int[connoffs+0] = k-1;
            network->hlconnections.ptr.p_int[connoffs+1] = i;
            network->hlconnections.ptr.p_int[connoffs+2] = k;
            network->hlconnections.ptr.p_int[connoffs+3] = j;
            network->hlconnections.ptr.p_int[connoffs+4] = *weightsidx+i+j*(nprev+1);
            connoffs = connoffs+mlpbase_hlconnfieldwidth;
        }
    }
    *connidx = *connidx+nprev*ncur;
    *neuroidx = *neuroidx+ncur;
    *structinfoidx = *structinfoidx+2*ncur+1;
    *weightsidx = *weightsidx+ncur*(nprev+1);
}


/*************************************************************************
This function fills high level information about network created using
internal MLPCreate() function.

This function does NOT examine StructInfo for low level information, it
just expects that network has following structure:

    input neuron            \
    ...                      | input layer
    input neuron            /
    
    "-1" neuron             \
    biased summator          |
    ...                      |
    biased summator          | hidden layer(s), if there are exists any
    activation function      |
    ...                      |
    activation function     /
    
    "-1" neuron            \
    biased summator         | output layer:
    ...                     |
    biased summator         | * we have NOut summators/activators for regression networks
    activation function     | * we have only NOut-1 summators and no activators for classifiers
    ...                     | * we have "0" neuron only when we have classifier
    activation function     |
    "0" neuron              /


  -- ALGLIB --
     Copyright 30.03.2008 by Bochkanov Sergey
*************************************************************************/
static void mlpbase_fillhighlevelinformation(multilayerperceptron* network,
     ae_int_t nin,
     ae_int_t nhid1,
     ae_int_t nhid2,
     ae_int_t nout,
     ae_bool iscls,
     ae_bool islinearout,
     ae_state *_state)
{
    ae_int_t idxweights;
    ae_int_t idxstruct;
    ae_int_t idxneuro;
    ae_int_t idxconn;


    ae_assert((iscls&&islinearout)||!iscls, "FillHighLevelInformation: internal error", _state);
    
    /*
     * Preparations common to all types of networks
     */
    idxweights = 0;
    idxneuro = 0;
    idxstruct = 0;
    idxconn = 0;
    network->hlnetworktype = 0;
    
    /*
     * network without hidden layers
     */
    if( nhid1==0 )
    {
        ae_vector_set_length(&network->hllayersizes, 2, _state);
        network->hllayersizes.ptr.p_int[0] = nin;
        network->hllayersizes.ptr.p_int[1] = nout;
        if( !iscls )
        {
            ae_vector_set_length(&network->hlconnections, mlpbase_hlconnfieldwidth*nin*nout, _state);
            ae_vector_set_length(&network->hlneurons, mlpbase_hlnfieldwidth*(nin+nout), _state);
            network->hlnormtype = 0;
        }
        else
        {
            ae_vector_set_length(&network->hlconnections, mlpbase_hlconnfieldwidth*nin*(nout-1), _state);
            ae_vector_set_length(&network->hlneurons, mlpbase_hlnfieldwidth*(nin+nout), _state);
            network->hlnormtype = 1;
        }
        mlpbase_hladdinputlayer(network, &idxconn, &idxneuro, &idxstruct, nin, _state);
        mlpbase_hladdoutputlayer(network, &idxconn, &idxneuro, &idxstruct, &idxweights, 1, nin, nout, iscls, islinearout, _state);
        return;
    }
    
    /*
     * network with one hidden layers
     */
    if( nhid2==0 )
    {
        ae_vector_set_length(&network->hllayersizes, 3, _state);
        network->hllayersizes.ptr.p_int[0] = nin;
        network->hllayersizes.ptr.p_int[1] = nhid1;
        network->hllayersizes.ptr.p_int[2] = nout;
        if( !iscls )
        {
            ae_vector_set_length(&network->hlconnections, mlpbase_hlconnfieldwidth*(nin*nhid1+nhid1*nout), _state);
            ae_vector_set_length(&network->hlneurons, mlpbase_hlnfieldwidth*(nin+nhid1+nout), _state);
            network->hlnormtype = 0;
        }
        else
        {
            ae_vector_set_length(&network->hlconnections, mlpbase_hlconnfieldwidth*(nin*nhid1+nhid1*(nout-1)), _state);
            ae_vector_set_length(&network->hlneurons, mlpbase_hlnfieldwidth*(nin+nhid1+nout), _state);
            network->hlnormtype = 1;
        }
        mlpbase_hladdinputlayer(network, &idxconn, &idxneuro, &idxstruct, nin, _state);
        mlpbase_hladdhiddenlayer(network, &idxconn, &idxneuro, &idxstruct, &idxweights, 1, nin, nhid1, _state);
        mlpbase_hladdoutputlayer(network, &idxconn, &idxneuro, &idxstruct, &idxweights, 2, nhid1, nout, iscls, islinearout, _state);
        return;
    }
    
    /*
     * Two hidden layers
     */
    ae_vector_set_length(&network->hllayersizes, 4, _state);
    network->hllayersizes.ptr.p_int[0] = nin;
    network->hllayersizes.ptr.p_int[1] = nhid1;
    network->hllayersizes.ptr.p_int[2] = nhid2;
    network->hllayersizes.ptr.p_int[3] = nout;
    if( !iscls )
    {
        ae_vector_set_length(&network->hlconnections, mlpbase_hlconnfieldwidth*(nin*nhid1+nhid1*nhid2+nhid2*nout), _state);
        ae_vector_set_length(&network->hlneurons, mlpbase_hlnfieldwidth*(nin+nhid1+nhid2+nout), _state);
        network->hlnormtype = 0;
    }
    else
    {
        ae_vector_set_length(&network->hlconnections, mlpbase_hlconnfieldwidth*(nin*nhid1+nhid1*nhid2+nhid2*(nout-1)), _state);
        ae_vector_set_length(&network->hlneurons, mlpbase_hlnfieldwidth*(nin+nhid1+nhid2+nout), _state);
        network->hlnormtype = 1;
    }
    mlpbase_hladdinputlayer(network, &idxconn, &idxneuro, &idxstruct, nin, _state);
    mlpbase_hladdhiddenlayer(network, &idxconn, &idxneuro, &idxstruct, &idxweights, 1, nin, nhid1, _state);
    mlpbase_hladdhiddenlayer(network, &idxconn, &idxneuro, &idxstruct, &idxweights, 2, nhid1, nhid2, _state);
    mlpbase_hladdoutputlayer(network, &idxconn, &idxneuro, &idxstruct, &idxweights, 3, nhid2, nout, iscls, islinearout, _state);
}


/*************************************************************************
Internal subroutine.

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
static void mlpbase_mlpcreate(ae_int_t nin,
     ae_int_t nout,
     /* Integer */ ae_vector* lsizes,
     /* Integer */ ae_vector* ltypes,
     /* Integer */ ae_vector* lconnfirst,
     /* Integer */ ae_vector* lconnlast,
     ae_int_t layerscount,
     ae_bool isclsnet,
     multilayerperceptron* network,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t ssize;
    ae_int_t ntotal;
    ae_int_t wcount;
    ae_int_t offs;
    ae_int_t nprocessed;
    ae_int_t wallocated;
    ae_vector localtemp;
    ae_vector lnfirst;
    ae_vector lnsyn;
    mlpbuffers buf;
    smlpgrad sgrad;

    ae_frame_make(_state, &_frame_block);
    _multilayerperceptron_clear(network);
    ae_vector_init(&localtemp, 0, DT_INT, _state);
    ae_vector_init(&lnfirst, 0, DT_INT, _state);
    ae_vector_init(&lnsyn, 0, DT_INT, _state);
    _mlpbuffers_init(&buf, _state);
    _smlpgrad_init(&sgrad, _state);

    
    /*
     * Check
     */
    ae_assert(layerscount>0, "MLPCreate: wrong parameters!", _state);
    ae_assert(ltypes->ptr.p_int[0]==-2, "MLPCreate: wrong LTypes[0] (must be -2)!", _state);
    for(i=0; i<=layerscount-1; i++)
    {
        ae_assert(lsizes->ptr.p_int[i]>0, "MLPCreate: wrong LSizes!", _state);
        ae_assert(lconnfirst->ptr.p_int[i]>=0&&(lconnfirst->ptr.p_int[i]<i||i==0), "MLPCreate: wrong LConnFirst!", _state);
        ae_assert(lconnlast->ptr.p_int[i]>=lconnfirst->ptr.p_int[i]&&(lconnlast->ptr.p_int[i]<i||i==0), "MLPCreate: wrong LConnLast!", _state);
    }
    
    /*
     * Build network geometry
     */
    ae_vector_set_length(&lnfirst, layerscount-1+1, _state);
    ae_vector_set_length(&lnsyn, layerscount-1+1, _state);
    ntotal = 0;
    wcount = 0;
    for(i=0; i<=layerscount-1; i++)
    {
        
        /*
         * Analyze connections.
         * This code must throw an assertion in case of unknown LTypes[I]
         */
        lnsyn.ptr.p_int[i] = -1;
        if( ltypes->ptr.p_int[i]>=0||ltypes->ptr.p_int[i]==-5 )
        {
            lnsyn.ptr.p_int[i] = 0;
            for(j=lconnfirst->ptr.p_int[i]; j<=lconnlast->ptr.p_int[i]; j++)
            {
                lnsyn.ptr.p_int[i] = lnsyn.ptr.p_int[i]+lsizes->ptr.p_int[j];
            }
        }
        else
        {
            if( (ltypes->ptr.p_int[i]==-2||ltypes->ptr.p_int[i]==-3)||ltypes->ptr.p_int[i]==-4 )
            {
                lnsyn.ptr.p_int[i] = 0;
            }
        }
        ae_assert(lnsyn.ptr.p_int[i]>=0, "MLPCreate: internal error #0!", _state);
        
        /*
         * Other info
         */
        lnfirst.ptr.p_int[i] = ntotal;
        ntotal = ntotal+lsizes->ptr.p_int[i];
        if( ltypes->ptr.p_int[i]==0 )
        {
            wcount = wcount+lnsyn.ptr.p_int[i]*lsizes->ptr.p_int[i];
        }
    }
    ssize = 7+ntotal*mlpbase_nfieldwidth;
    
    /*
     * Allocate
     */
    ae_vector_set_length(&network->structinfo, ssize-1+1, _state);
    ae_vector_set_length(&network->weights, wcount-1+1, _state);
    if( isclsnet )
    {
        ae_vector_set_length(&network->columnmeans, nin-1+1, _state);
        ae_vector_set_length(&network->columnsigmas, nin-1+1, _state);
    }
    else
    {
        ae_vector_set_length(&network->columnmeans, nin+nout-1+1, _state);
        ae_vector_set_length(&network->columnsigmas, nin+nout-1+1, _state);
    }
    ae_vector_set_length(&network->neurons, ntotal-1+1, _state);
    ae_vector_set_length(&network->nwbuf, ae_maxint(wcount, 2*nout, _state)-1+1, _state);
    ae_vector_set_length(&network->integerbuf, 3+1, _state);
    ae_vector_set_length(&network->dfdnet, ntotal-1+1, _state);
    ae_vector_set_length(&network->x, nin-1+1, _state);
    ae_vector_set_length(&network->y, nout-1+1, _state);
    ae_vector_set_length(&network->derror, ntotal-1+1, _state);
    
    /*
     * Fill structure: global info
     */
    network->structinfo.ptr.p_int[0] = ssize;
    network->structinfo.ptr.p_int[1] = nin;
    network->structinfo.ptr.p_int[2] = nout;
    network->structinfo.ptr.p_int[3] = ntotal;
    network->structinfo.ptr.p_int[4] = wcount;
    network->structinfo.ptr.p_int[5] = 7;
    if( isclsnet )
    {
        network->structinfo.ptr.p_int[6] = 1;
    }
    else
    {
        network->structinfo.ptr.p_int[6] = 0;
    }
    
    /*
     * Fill structure: neuron connections
     */
    nprocessed = 0;
    wallocated = 0;
    for(i=0; i<=layerscount-1; i++)
    {
        for(j=0; j<=lsizes->ptr.p_int[i]-1; j++)
        {
            offs = network->structinfo.ptr.p_int[5]+nprocessed*mlpbase_nfieldwidth;
            network->structinfo.ptr.p_int[offs+0] = ltypes->ptr.p_int[i];
            if( ltypes->ptr.p_int[i]==0 )
            {
                
                /*
                 * Adaptive summator:
                 * * connections with weights to previous neurons
                 */
                network->structinfo.ptr.p_int[offs+1] = lnsyn.ptr.p_int[i];
                network->structinfo.ptr.p_int[offs+2] = lnfirst.ptr.p_int[lconnfirst->ptr.p_int[i]];
                network->structinfo.ptr.p_int[offs+3] = wallocated;
                wallocated = wallocated+lnsyn.ptr.p_int[i];
                nprocessed = nprocessed+1;
            }
            if( ltypes->ptr.p_int[i]>0||ltypes->ptr.p_int[i]==-5 )
            {
                
                /*
                 * Activation layer:
                 * * each neuron connected to one (only one) of previous neurons.
                 * * no weights
                 */
                network->structinfo.ptr.p_int[offs+1] = 1;
                network->structinfo.ptr.p_int[offs+2] = lnfirst.ptr.p_int[lconnfirst->ptr.p_int[i]]+j;
                network->structinfo.ptr.p_int[offs+3] = -1;
                nprocessed = nprocessed+1;
            }
            if( (ltypes->ptr.p_int[i]==-2||ltypes->ptr.p_int[i]==-3)||ltypes->ptr.p_int[i]==-4 )
            {
                nprocessed = nprocessed+1;
            }
        }
    }
    ae_assert(wallocated==wcount, "MLPCreate: internal error #1!", _state);
    ae_assert(nprocessed==ntotal, "MLPCreate: internal error #2!", _state);
    
    /*
     * Fill weights by small random values
     * Initialize means and sigmas
     */
    for(i=0; i<=nin-1; i++)
    {
        network->columnmeans.ptr.p_double[i] = (double)(0);
        network->columnsigmas.ptr.p_double[i] = (double)(1);
    }
    if( !isclsnet )
    {
        for(i=0; i<=nout-1; i++)
        {
            network->columnmeans.ptr.p_double[nin+i] = (double)(0);
            network->columnsigmas.ptr.p_double[nin+i] = (double)(1);
        }
    }
    mlprandomize(network, _state);
    
    /*
     * Seed buffers
     */
    ae_shared_pool_set_seed(&network->buf, &buf, sizeof(buf), _mlpbuffers_init, _mlpbuffers_init_copy, _mlpbuffers_destroy, _state);
    ae_vector_set_length(&sgrad.g, wcount, _state);
    sgrad.f = 0.0;
    for(i=0; i<=wcount-1; i++)
    {
        sgrad.g.ptr.p_double[i] = 0.0;
    }
    ae_shared_pool_set_seed(&network->gradbuf, &sgrad, sizeof(sgrad), _smlpgrad_init, _smlpgrad_init_copy, _smlpgrad_destroy, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine for Hessian calculation.

WARNING! Unspeakable math far beyong human capabilities :)
*************************************************************************/
static void mlpbase_mlphessianbatchinternal(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t ssize,
     ae_bool naturalerr,
     double* e,
     /* Real    */ ae_vector* grad,
     /* Real    */ ae_matrix* h,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t wcount;
    ae_int_t ntotal;
    ae_int_t istart;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t kl;
    ae_int_t offs;
    ae_int_t n1;
    ae_int_t n2;
    ae_int_t w1;
    ae_int_t w2;
    double s;
    double t;
    double v;
    double et;
    ae_bool bflag;
    double f;
    double df;
    double d2f;
    double deidyj;
    double mx;
    double q;
    double z;
    double s2;
    double expi;
    double expj;
    ae_vector x;
    ae_vector desiredy;
    ae_vector gt;
    ae_vector zeros;
    ae_matrix rx;
    ae_matrix ry;
    ae_matrix rdx;
    ae_matrix rdy;

    ae_frame_make(_state, &_frame_block);
    *e = 0;
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&desiredy, 0, DT_REAL, _state);
    ae_vector_init(&gt, 0, DT_REAL, _state);
    ae_vector_init(&zeros, 0, DT_REAL, _state);
    ae_matrix_init(&rx, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ry, 0, 0, DT_REAL, _state);
    ae_matrix_init(&rdx, 0, 0, DT_REAL, _state);
    ae_matrix_init(&rdy, 0, 0, DT_REAL, _state);

    mlpproperties(network, &nin, &nout, &wcount, _state);
    ntotal = network->structinfo.ptr.p_int[3];
    istart = network->structinfo.ptr.p_int[5];
    
    /*
     * Prepare
     */
    ae_vector_set_length(&x, nin-1+1, _state);
    ae_vector_set_length(&desiredy, nout-1+1, _state);
    ae_vector_set_length(&zeros, wcount-1+1, _state);
    ae_vector_set_length(&gt, wcount-1+1, _state);
    ae_matrix_set_length(&rx, ntotal+nout-1+1, wcount-1+1, _state);
    ae_matrix_set_length(&ry, ntotal+nout-1+1, wcount-1+1, _state);
    ae_matrix_set_length(&rdx, ntotal+nout-1+1, wcount-1+1, _state);
    ae_matrix_set_length(&rdy, ntotal+nout-1+1, wcount-1+1, _state);
    *e = (double)(0);
    for(i=0; i<=wcount-1; i++)
    {
        zeros.ptr.p_double[i] = (double)(0);
    }
    ae_v_move(&grad->ptr.p_double[0], 1, &zeros.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
    for(i=0; i<=wcount-1; i++)
    {
        ae_v_move(&h->ptr.pp_double[i][0], 1, &zeros.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
    }
    
    /*
     * Process
     */
    for(k=0; k<=ssize-1; k++)
    {
        
        /*
         * Process vector with MLPGradN.
         * Now Neurons, DFDNET and DError contains results of the last run.
         */
        ae_v_move(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[k][0], 1, ae_v_len(0,nin-1));
        if( mlpissoftmax(network, _state) )
        {
            
            /*
             * class labels outputs
             */
            kl = ae_round(xy->ptr.pp_double[k][nin], _state);
            for(i=0; i<=nout-1; i++)
            {
                if( i==kl )
                {
                    desiredy.ptr.p_double[i] = (double)(1);
                }
                else
                {
                    desiredy.ptr.p_double[i] = (double)(0);
                }
            }
        }
        else
        {
            
            /*
             * real outputs
             */
            ae_v_move(&desiredy.ptr.p_double[0], 1, &xy->ptr.pp_double[k][nin], 1, ae_v_len(0,nout-1));
        }
        if( naturalerr )
        {
            mlpgradn(network, &x, &desiredy, &et, &gt, _state);
        }
        else
        {
            mlpgrad(network, &x, &desiredy, &et, &gt, _state);
        }
        
        /*
         * grad, error
         */
        *e = *e+et;
        ae_v_add(&grad->ptr.p_double[0], 1, &gt.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        
        /*
         * Hessian.
         * Forward pass of the R-algorithm
         */
        for(i=0; i<=ntotal-1; i++)
        {
            offs = istart+i*mlpbase_nfieldwidth;
            ae_v_move(&rx.ptr.pp_double[i][0], 1, &zeros.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            ae_v_move(&ry.ptr.pp_double[i][0], 1, &zeros.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
            if( network->structinfo.ptr.p_int[offs+0]>0||network->structinfo.ptr.p_int[offs+0]==-5 )
            {
                
                /*
                 * Activation function
                 */
                n1 = network->structinfo.ptr.p_int[offs+2];
                ae_v_move(&rx.ptr.pp_double[i][0], 1, &ry.ptr.pp_double[n1][0], 1, ae_v_len(0,wcount-1));
                v = network->dfdnet.ptr.p_double[i];
                ae_v_moved(&ry.ptr.pp_double[i][0], 1, &rx.ptr.pp_double[i][0], 1, ae_v_len(0,wcount-1), v);
                continue;
            }
            if( network->structinfo.ptr.p_int[offs+0]==0 )
            {
                
                /*
                 * Adaptive summator
                 */
                n1 = network->structinfo.ptr.p_int[offs+2];
                n2 = n1+network->structinfo.ptr.p_int[offs+1]-1;
                w1 = network->structinfo.ptr.p_int[offs+3];
                w2 = w1+network->structinfo.ptr.p_int[offs+1]-1;
                for(j=n1; j<=n2; j++)
                {
                    v = network->weights.ptr.p_double[w1+j-n1];
                    ae_v_addd(&rx.ptr.pp_double[i][0], 1, &ry.ptr.pp_double[j][0], 1, ae_v_len(0,wcount-1), v);
                    rx.ptr.pp_double[i][w1+j-n1] = rx.ptr.pp_double[i][w1+j-n1]+network->neurons.ptr.p_double[j];
                }
                ae_v_move(&ry.ptr.pp_double[i][0], 1, &rx.ptr.pp_double[i][0], 1, ae_v_len(0,wcount-1));
                continue;
            }
            if( network->structinfo.ptr.p_int[offs+0]<0 )
            {
                bflag = ae_true;
                if( network->structinfo.ptr.p_int[offs+0]==-2 )
                {
                    
                    /*
                     * input neuron, left unchanged
                     */
                    bflag = ae_false;
                }
                if( network->structinfo.ptr.p_int[offs+0]==-3 )
                {
                    
                    /*
                     * "-1" neuron, left unchanged
                     */
                    bflag = ae_false;
                }
                if( network->structinfo.ptr.p_int[offs+0]==-4 )
                {
                    
                    /*
                     * "0" neuron, left unchanged
                     */
                    bflag = ae_false;
                }
                ae_assert(!bflag, "MLPHessianNBatch: internal error - unknown neuron type!", _state);
                continue;
            }
        }
        
        /*
         * Hessian. Backward pass of the R-algorithm.
         *
         * Stage 1. Initialize RDY
         */
        for(i=0; i<=ntotal+nout-1; i++)
        {
            ae_v_move(&rdy.ptr.pp_double[i][0], 1, &zeros.ptr.p_double[0], 1, ae_v_len(0,wcount-1));
        }
        if( network->structinfo.ptr.p_int[6]==0 )
        {
            
            /*
             * Standardisation.
             *
             * In context of the Hessian calculation standardisation
             * is considered as additional layer with weightless
             * activation function:
             *
             * F(NET) := Sigma*NET
             *
             * So we add one more layer to forward pass, and
             * make forward/backward pass through this layer.
             */
            for(i=0; i<=nout-1; i++)
            {
                n1 = ntotal-nout+i;
                n2 = ntotal+i;
                
                /*
                 * Forward pass from N1 to N2
                 */
                ae_v_move(&rx.ptr.pp_double[n2][0], 1, &ry.ptr.pp_double[n1][0], 1, ae_v_len(0,wcount-1));
                v = network->columnsigmas.ptr.p_double[nin+i];
                ae_v_moved(&ry.ptr.pp_double[n2][0], 1, &rx.ptr.pp_double[n2][0], 1, ae_v_len(0,wcount-1), v);
                
                /*
                 * Initialization of RDY
                 */
                ae_v_move(&rdy.ptr.pp_double[n2][0], 1, &ry.ptr.pp_double[n2][0], 1, ae_v_len(0,wcount-1));
                
                /*
                 * Backward pass from N2 to N1:
                 * 1. Calculate R(dE/dX).
                 * 2. No R(dE/dWij) is needed since weight of activation neuron
                 *    is fixed to 1. So we can update R(dE/dY) for
                 *    the connected neuron (note that Vij=0, Wij=1)
                 */
                df = network->columnsigmas.ptr.p_double[nin+i];
                ae_v_moved(&rdx.ptr.pp_double[n2][0], 1, &rdy.ptr.pp_double[n2][0], 1, ae_v_len(0,wcount-1), df);
                ae_v_add(&rdy.ptr.pp_double[n1][0], 1, &rdx.ptr.pp_double[n2][0], 1, ae_v_len(0,wcount-1));
            }
        }
        else
        {
            
            /*
             * Softmax.
             *
             * Initialize RDY using generalized expression for ei'(yi)
             * (see expression (9) from p. 5 of "Fast Exact Multiplication by the Hessian").
             *
             * When we are working with softmax network, generalized
             * expression for ei'(yi) is used because softmax
             * normalization leads to ei, which depends on all y's
             */
            if( naturalerr )
            {
                
                /*
                 * softmax + cross-entropy.
                 * We have:
                 *
                 * S = sum(exp(yk)),
                 * ei = sum(trn)*exp(yi)/S-trn_i
                 *
                 * j=i:   d(ei)/d(yj) = T*exp(yi)*(S-exp(yi))/S^2
                 * j<>i:  d(ei)/d(yj) = -T*exp(yi)*exp(yj)/S^2
                 */
                t = (double)(0);
                for(i=0; i<=nout-1; i++)
                {
                    t = t+desiredy.ptr.p_double[i];
                }
                mx = network->neurons.ptr.p_double[ntotal-nout];
                for(i=0; i<=nout-1; i++)
                {
                    mx = ae_maxreal(mx, network->neurons.ptr.p_double[ntotal-nout+i], _state);
                }
                s = (double)(0);
                for(i=0; i<=nout-1; i++)
                {
                    network->nwbuf.ptr.p_double[i] = ae_exp(network->neurons.ptr.p_double[ntotal-nout+i]-mx, _state);
                    s = s+network->nwbuf.ptr.p_double[i];
                }
                for(i=0; i<=nout-1; i++)
                {
                    for(j=0; j<=nout-1; j++)
                    {
                        if( j==i )
                        {
                            deidyj = t*network->nwbuf.ptr.p_double[i]*(s-network->nwbuf.ptr.p_double[i])/ae_sqr(s, _state);
                            ae_v_addd(&rdy.ptr.pp_double[ntotal-nout+i][0], 1, &ry.ptr.pp_double[ntotal-nout+i][0], 1, ae_v_len(0,wcount-1), deidyj);
                        }
                        else
                        {
                            deidyj = -t*network->nwbuf.ptr.p_double[i]*network->nwbuf.ptr.p_double[j]/ae_sqr(s, _state);
                            ae_v_addd(&rdy.ptr.pp_double[ntotal-nout+i][0], 1, &ry.ptr.pp_double[ntotal-nout+j][0], 1, ae_v_len(0,wcount-1), deidyj);
                        }
                    }
                }
            }
            else
            {
                
                /*
                 * For a softmax + squared error we have expression
                 * far beyond human imagination so we dont even try
                 * to comment on it. Just enjoy the code...
                 *
                 * P.S. That's why "natural error" is called "natural" -
                 * compact beatiful expressions, fast code....
                 */
                mx = network->neurons.ptr.p_double[ntotal-nout];
                for(i=0; i<=nout-1; i++)
                {
                    mx = ae_maxreal(mx, network->neurons.ptr.p_double[ntotal-nout+i], _state);
                }
                s = (double)(0);
                s2 = (double)(0);
                for(i=0; i<=nout-1; i++)
                {
                    network->nwbuf.ptr.p_double[i] = ae_exp(network->neurons.ptr.p_double[ntotal-nout+i]-mx, _state);
                    s = s+network->nwbuf.ptr.p_double[i];
                    s2 = s2+ae_sqr(network->nwbuf.ptr.p_double[i], _state);
                }
                q = (double)(0);
                for(i=0; i<=nout-1; i++)
                {
                    q = q+(network->y.ptr.p_double[i]-desiredy.ptr.p_double[i])*network->nwbuf.ptr.p_double[i];
                }
                for(i=0; i<=nout-1; i++)
                {
                    z = -q+(network->y.ptr.p_double[i]-desiredy.ptr.p_double[i])*s;
                    expi = network->nwbuf.ptr.p_double[i];
                    for(j=0; j<=nout-1; j++)
                    {
                        expj = network->nwbuf.ptr.p_double[j];
                        if( j==i )
                        {
                            deidyj = expi/ae_sqr(s, _state)*((z+expi)*(s-2*expi)/s+expi*s2/ae_sqr(s, _state));
                        }
                        else
                        {
                            deidyj = expi*expj/ae_sqr(s, _state)*(s2/ae_sqr(s, _state)-2*z/s-(expi+expj)/s+(network->y.ptr.p_double[i]-desiredy.ptr.p_double[i])-(network->y.ptr.p_double[j]-desiredy.ptr.p_double[j]));
                        }
                        ae_v_addd(&rdy.ptr.pp_double[ntotal-nout+i][0], 1, &ry.ptr.pp_double[ntotal-nout+j][0], 1, ae_v_len(0,wcount-1), deidyj);
                    }
                }
            }
        }
        
        /*
         * Hessian. Backward pass of the R-algorithm
         *
         * Stage 2. Process.
         */
        for(i=ntotal-1; i>=0; i--)
        {
            
            /*
             * Possible variants:
             * 1. Activation function
             * 2. Adaptive summator
             * 3. Special neuron
             */
            offs = istart+i*mlpbase_nfieldwidth;
            if( network->structinfo.ptr.p_int[offs+0]>0||network->structinfo.ptr.p_int[offs+0]==-5 )
            {
                n1 = network->structinfo.ptr.p_int[offs+2];
                
                /*
                 * First, calculate R(dE/dX).
                 */
                mlpactivationfunction(network->neurons.ptr.p_double[n1], network->structinfo.ptr.p_int[offs+0], &f, &df, &d2f, _state);
                v = d2f*network->derror.ptr.p_double[i];
                ae_v_moved(&rdx.ptr.pp_double[i][0], 1, &rdy.ptr.pp_double[i][0], 1, ae_v_len(0,wcount-1), df);
                ae_v_addd(&rdx.ptr.pp_double[i][0], 1, &rx.ptr.pp_double[i][0], 1, ae_v_len(0,wcount-1), v);
                
                /*
                 * No R(dE/dWij) is needed since weight of activation neuron
                 * is fixed to 1.
                 *
                 * So we can update R(dE/dY) for the connected neuron.
                 * (note that Vij=0, Wij=1)
                 */
                ae_v_add(&rdy.ptr.pp_double[n1][0], 1, &rdx.ptr.pp_double[i][0], 1, ae_v_len(0,wcount-1));
                continue;
            }
            if( network->structinfo.ptr.p_int[offs+0]==0 )
            {
                
                /*
                 * Adaptive summator
                 */
                n1 = network->structinfo.ptr.p_int[offs+2];
                n2 = n1+network->structinfo.ptr.p_int[offs+1]-1;
                w1 = network->structinfo.ptr.p_int[offs+3];
                w2 = w1+network->structinfo.ptr.p_int[offs+1]-1;
                
                /*
                 * First, calculate R(dE/dX).
                 */
                ae_v_move(&rdx.ptr.pp_double[i][0], 1, &rdy.ptr.pp_double[i][0], 1, ae_v_len(0,wcount-1));
                
                /*
                 * Then, calculate R(dE/dWij)
                 */
                for(j=w1; j<=w2; j++)
                {
                    v = network->neurons.ptr.p_double[n1+j-w1];
                    ae_v_addd(&h->ptr.pp_double[j][0], 1, &rdx.ptr.pp_double[i][0], 1, ae_v_len(0,wcount-1), v);
                    v = network->derror.ptr.p_double[i];
                    ae_v_addd(&h->ptr.pp_double[j][0], 1, &ry.ptr.pp_double[n1+j-w1][0], 1, ae_v_len(0,wcount-1), v);
                }
                
                /*
                 * And finally, update R(dE/dY) for connected neurons.
                 */
                for(j=w1; j<=w2; j++)
                {
                    v = network->weights.ptr.p_double[j];
                    ae_v_addd(&rdy.ptr.pp_double[n1+j-w1][0], 1, &rdx.ptr.pp_double[i][0], 1, ae_v_len(0,wcount-1), v);
                    rdy.ptr.pp_double[n1+j-w1][j] = rdy.ptr.pp_double[n1+j-w1][j]+network->derror.ptr.p_double[i];
                }
                continue;
            }
            if( network->structinfo.ptr.p_int[offs+0]<0 )
            {
                bflag = ae_false;
                if( (network->structinfo.ptr.p_int[offs+0]==-2||network->structinfo.ptr.p_int[offs+0]==-3)||network->structinfo.ptr.p_int[offs+0]==-4 )
                {
                    
                    /*
                     * Special neuron type, no back-propagation required
                     */
                    bflag = ae_true;
                }
                ae_assert(bflag, "MLPHessianNBatch: unknown neuron type!", _state);
                continue;
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine

Network must be processed by MLPProcess on X
*************************************************************************/
static void mlpbase_mlpinternalcalculategradient(multilayerperceptron* network,
     /* Real    */ ae_vector* neurons,
     /* Real    */ ae_vector* weights,
     /* Real    */ ae_vector* derror,
     /* Real    */ ae_vector* grad,
     ae_bool naturalerrorfunc,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t n1;
    ae_int_t n2;
    ae_int_t w1;
    ae_int_t w2;
    ae_int_t ntotal;
    ae_int_t istart;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t offs;
    double dedf;
    double dfdnet;
    double v;
    double fown;
    double deown;
    double net;
    double mx;
    ae_bool bflag;


    
    /*
     * Read network geometry
     */
    nin = network->structinfo.ptr.p_int[1];
    nout = network->structinfo.ptr.p_int[2];
    ntotal = network->structinfo.ptr.p_int[3];
    istart = network->structinfo.ptr.p_int[5];
    
    /*
     * Pre-processing of dError/dOut:
     * from dError/dOut(normalized) to dError/dOut(non-normalized)
     */
    ae_assert(network->structinfo.ptr.p_int[6]==0||network->structinfo.ptr.p_int[6]==1, "MLPInternalCalculateGradient: unknown normalization type!", _state);
    if( network->structinfo.ptr.p_int[6]==1 )
    {
        
        /*
         * Softmax
         */
        if( !naturalerrorfunc )
        {
            mx = network->neurons.ptr.p_double[ntotal-nout];
            for(i=0; i<=nout-1; i++)
            {
                mx = ae_maxreal(mx, network->neurons.ptr.p_double[ntotal-nout+i], _state);
            }
            net = (double)(0);
            for(i=0; i<=nout-1; i++)
            {
                network->nwbuf.ptr.p_double[i] = ae_exp(network->neurons.ptr.p_double[ntotal-nout+i]-mx, _state);
                net = net+network->nwbuf.ptr.p_double[i];
            }
            v = ae_v_dotproduct(&network->derror.ptr.p_double[ntotal-nout], 1, &network->nwbuf.ptr.p_double[0], 1, ae_v_len(ntotal-nout,ntotal-1));
            for(i=0; i<=nout-1; i++)
            {
                fown = network->nwbuf.ptr.p_double[i];
                deown = network->derror.ptr.p_double[ntotal-nout+i];
                network->nwbuf.ptr.p_double[nout+i] = (-v+deown*fown+deown*(net-fown))*fown/ae_sqr(net, _state);
            }
            for(i=0; i<=nout-1; i++)
            {
                network->derror.ptr.p_double[ntotal-nout+i] = network->nwbuf.ptr.p_double[nout+i];
            }
        }
    }
    else
    {
        
        /*
         * Un-standardisation
         */
        for(i=0; i<=nout-1; i++)
        {
            network->derror.ptr.p_double[ntotal-nout+i] = network->derror.ptr.p_double[ntotal-nout+i]*network->columnsigmas.ptr.p_double[nin+i];
        }
    }
    
    /*
     * Backpropagation
     */
    for(i=ntotal-1; i>=0; i--)
    {
        
        /*
         * Extract info
         */
        offs = istart+i*mlpbase_nfieldwidth;
        if( network->structinfo.ptr.p_int[offs+0]>0||network->structinfo.ptr.p_int[offs+0]==-5 )
        {
            
            /*
             * Activation function
             */
            dedf = network->derror.ptr.p_double[i];
            dfdnet = network->dfdnet.ptr.p_double[i];
            derror->ptr.p_double[network->structinfo.ptr.p_int[offs+2]] = derror->ptr.p_double[network->structinfo.ptr.p_int[offs+2]]+dedf*dfdnet;
            continue;
        }
        if( network->structinfo.ptr.p_int[offs+0]==0 )
        {
            
            /*
             * Adaptive summator
             */
            n1 = network->structinfo.ptr.p_int[offs+2];
            n2 = n1+network->structinfo.ptr.p_int[offs+1]-1;
            w1 = network->structinfo.ptr.p_int[offs+3];
            w2 = w1+network->structinfo.ptr.p_int[offs+1]-1;
            dedf = network->derror.ptr.p_double[i];
            dfdnet = 1.0;
            v = dedf*dfdnet;
            ae_v_moved(&grad->ptr.p_double[w1], 1, &neurons->ptr.p_double[n1], 1, ae_v_len(w1,w2), v);
            ae_v_addd(&derror->ptr.p_double[n1], 1, &weights->ptr.p_double[w1], 1, ae_v_len(n1,n2), v);
            continue;
        }
        if( network->structinfo.ptr.p_int[offs+0]<0 )
        {
            bflag = ae_false;
            if( (network->structinfo.ptr.p_int[offs+0]==-2||network->structinfo.ptr.p_int[offs+0]==-3)||network->structinfo.ptr.p_int[offs+0]==-4 )
            {
                
                /*
                 * Special neuron type, no back-propagation required
                 */
                bflag = ae_true;
            }
            ae_assert(bflag, "MLPInternalCalculateGradient: unknown neuron type!", _state);
            continue;
        }
    }
}


static void mlpbase_mlpchunkedgradient(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t cstart,
     ae_int_t csize,
     /* Real    */ ae_vector* batch4buf,
     /* Real    */ ae_vector* hpcbuf,
     double* e,
     ae_bool naturalerrorfunc,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t kl;
    ae_int_t ntotal;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t offs;
    double f;
    double df;
    double d2f;
    double v;
    double vv;
    double s;
    double fown;
    double deown;
    ae_bool bflag;
    ae_int_t istart;
    ae_int_t entrysize;
    ae_int_t dfoffs;
    ae_int_t derroroffs;
    ae_int_t entryoffs;
    ae_int_t neuronidx;
    ae_int_t srcentryoffs;
    ae_int_t srcneuronidx;
    ae_int_t srcweightidx;
    ae_int_t neurontype;
    ae_int_t nweights;
    ae_int_t offs0;
    ae_int_t offs1;
    ae_int_t offs2;
    double v0;
    double v1;
    double v2;
    double v3;
    double s0;
    double s1;
    double s2;
    double s3;
    ae_int_t chunksize;


    chunksize = 4;
    ae_assert(csize<=chunksize, "MLPChunkedGradient: internal error (CSize>ChunkSize)", _state);
    
    /*
     * Try to use HPC core, if possible
     */
    if( hpcchunkedgradient(&network->weights, &network->structinfo, &network->columnmeans, &network->columnsigmas, xy, cstart, csize, batch4buf, hpcbuf, e, naturalerrorfunc, _state) )
    {
        return;
    }
    
    /*
     * Read network geometry, prepare data
     */
    nin = network->structinfo.ptr.p_int[1];
    nout = network->structinfo.ptr.p_int[2];
    ntotal = network->structinfo.ptr.p_int[3];
    istart = network->structinfo.ptr.p_int[5];
    entrysize = 12;
    dfoffs = 4;
    derroroffs = 8;
    
    /*
     * Fill Batch4Buf by zeros.
     *
     * THIS STAGE IS VERY IMPORTANT!
     *
     * We fill all components of entry - neuron values, dF/dNET, dError/dF.
     * It allows us to easily handle  situations  when  CSize<ChunkSize  by
     * simply  working  with  ALL  components  of  Batch4Buf,  without ever
     * looking at CSize. The idea is that dError/dF for  absent  components
     * will be initialized by zeros - and won't be  rewritten  by  non-zero
     * values during backpropagation.
     */
    for(i=0; i<=entrysize*ntotal-1; i++)
    {
        batch4buf->ptr.p_double[i] = (double)(0);
    }
    
    /*
     * Forward pass:
     * 1. Load data into Batch4Buf. If CSize<ChunkSize, data are padded by zeros.
     * 2. Perform forward pass through network
     */
    for(i=0; i<=nin-1; i++)
    {
        entryoffs = entrysize*i;
        for(j=0; j<=csize-1; j++)
        {
            if( ae_fp_neq(network->columnsigmas.ptr.p_double[i],(double)(0)) )
            {
                batch4buf->ptr.p_double[entryoffs+j] = (xy->ptr.pp_double[cstart+j][i]-network->columnmeans.ptr.p_double[i])/network->columnsigmas.ptr.p_double[i];
            }
            else
            {
                batch4buf->ptr.p_double[entryoffs+j] = xy->ptr.pp_double[cstart+j][i]-network->columnmeans.ptr.p_double[i];
            }
        }
    }
    for(neuronidx=0; neuronidx<=ntotal-1; neuronidx++)
    {
        entryoffs = entrysize*neuronidx;
        offs = istart+neuronidx*mlpbase_nfieldwidth;
        neurontype = network->structinfo.ptr.p_int[offs+0];
        if( neurontype>0||neurontype==-5 )
        {
            
            /*
             * "activation function" neuron, which takes value of neuron SrcNeuronIdx
             * and applies activation function to it.
             *
             * This neuron has no weights and no tunable parameters.
             */
            srcneuronidx = network->structinfo.ptr.p_int[offs+2];
            srcentryoffs = entrysize*srcneuronidx;
            mlpactivationfunction(batch4buf->ptr.p_double[srcentryoffs+0], neurontype, &f, &df, &d2f, _state);
            batch4buf->ptr.p_double[entryoffs+0] = f;
            batch4buf->ptr.p_double[entryoffs+0+dfoffs] = df;
            mlpactivationfunction(batch4buf->ptr.p_double[srcentryoffs+1], neurontype, &f, &df, &d2f, _state);
            batch4buf->ptr.p_double[entryoffs+1] = f;
            batch4buf->ptr.p_double[entryoffs+1+dfoffs] = df;
            mlpactivationfunction(batch4buf->ptr.p_double[srcentryoffs+2], neurontype, &f, &df, &d2f, _state);
            batch4buf->ptr.p_double[entryoffs+2] = f;
            batch4buf->ptr.p_double[entryoffs+2+dfoffs] = df;
            mlpactivationfunction(batch4buf->ptr.p_double[srcentryoffs+3], neurontype, &f, &df, &d2f, _state);
            batch4buf->ptr.p_double[entryoffs+3] = f;
            batch4buf->ptr.p_double[entryoffs+3+dfoffs] = df;
            continue;
        }
        if( neurontype==0 )
        {
            
            /*
             * "adaptive summator" neuron, whose output is a weighted sum of inputs.
             * It has weights, but has no activation function.
             */
            nweights = network->structinfo.ptr.p_int[offs+1];
            srcneuronidx = network->structinfo.ptr.p_int[offs+2];
            srcentryoffs = entrysize*srcneuronidx;
            srcweightidx = network->structinfo.ptr.p_int[offs+3];
            v0 = (double)(0);
            v1 = (double)(0);
            v2 = (double)(0);
            v3 = (double)(0);
            for(j=0; j<=nweights-1; j++)
            {
                v = network->weights.ptr.p_double[srcweightidx];
                srcweightidx = srcweightidx+1;
                v0 = v0+v*batch4buf->ptr.p_double[srcentryoffs+0];
                v1 = v1+v*batch4buf->ptr.p_double[srcentryoffs+1];
                v2 = v2+v*batch4buf->ptr.p_double[srcentryoffs+2];
                v3 = v3+v*batch4buf->ptr.p_double[srcentryoffs+3];
                srcentryoffs = srcentryoffs+entrysize;
            }
            batch4buf->ptr.p_double[entryoffs+0] = v0;
            batch4buf->ptr.p_double[entryoffs+1] = v1;
            batch4buf->ptr.p_double[entryoffs+2] = v2;
            batch4buf->ptr.p_double[entryoffs+3] = v3;
            batch4buf->ptr.p_double[entryoffs+0+dfoffs] = (double)(1);
            batch4buf->ptr.p_double[entryoffs+1+dfoffs] = (double)(1);
            batch4buf->ptr.p_double[entryoffs+2+dfoffs] = (double)(1);
            batch4buf->ptr.p_double[entryoffs+3+dfoffs] = (double)(1);
            continue;
        }
        if( neurontype<0 )
        {
            bflag = ae_false;
            if( neurontype==-2 )
            {
                
                /*
                 * Input neuron, left unchanged
                 */
                bflag = ae_true;
            }
            if( neurontype==-3 )
            {
                
                /*
                 * "-1" neuron
                 */
                batch4buf->ptr.p_double[entryoffs+0] = (double)(-1);
                batch4buf->ptr.p_double[entryoffs+1] = (double)(-1);
                batch4buf->ptr.p_double[entryoffs+2] = (double)(-1);
                batch4buf->ptr.p_double[entryoffs+3] = (double)(-1);
                batch4buf->ptr.p_double[entryoffs+0+dfoffs] = (double)(0);
                batch4buf->ptr.p_double[entryoffs+1+dfoffs] = (double)(0);
                batch4buf->ptr.p_double[entryoffs+2+dfoffs] = (double)(0);
                batch4buf->ptr.p_double[entryoffs+3+dfoffs] = (double)(0);
                bflag = ae_true;
            }
            if( neurontype==-4 )
            {
                
                /*
                 * "0" neuron
                 */
                batch4buf->ptr.p_double[entryoffs+0] = (double)(0);
                batch4buf->ptr.p_double[entryoffs+1] = (double)(0);
                batch4buf->ptr.p_double[entryoffs+2] = (double)(0);
                batch4buf->ptr.p_double[entryoffs+3] = (double)(0);
                batch4buf->ptr.p_double[entryoffs+0+dfoffs] = (double)(0);
                batch4buf->ptr.p_double[entryoffs+1+dfoffs] = (double)(0);
                batch4buf->ptr.p_double[entryoffs+2+dfoffs] = (double)(0);
                batch4buf->ptr.p_double[entryoffs+3+dfoffs] = (double)(0);
                bflag = ae_true;
            }
            ae_assert(bflag, "MLPChunkedGradient: internal error - unknown neuron type!", _state);
            continue;
        }
    }
    
    /*
     * Intermediate phase between forward and backward passes.
     *
     * For regression networks:
     * * forward pass is completely done (no additional post-processing is
     *   needed).
     * * before starting backward pass, we have to  calculate  dError/dOut
     *   for output neurons. We also update error at this phase.
     *
     * For classification networks:
     * * in addition to forward pass we  apply  SOFTMAX  normalization  to
     *   output neurons.
     * * after applying normalization, we have to  calculate  dError/dOut,
     *   which is calculated in two steps:
     *   * first, we calculate derivative of error with respect to SOFTMAX
     *     normalized outputs (normalized dError)
     *   * then,  we calculate derivative of error with respect to  values
     *     of outputs BEFORE normalization was applied to them
     */
    ae_assert(network->structinfo.ptr.p_int[6]==0||network->structinfo.ptr.p_int[6]==1, "MLPChunkedGradient: unknown normalization type!", _state);
    if( network->structinfo.ptr.p_int[6]==1 )
    {
        
        /*
         * SOFTMAX-normalized network.
         *
         * First,  calculate (V0,V1,V2,V3)  -  component-wise  maximum
         * of output neurons. This vector of maximum  values  will  be
         * used for normalization  of  outputs  prior  to  calculating
         * exponentials.
         *
         * NOTE: the only purpose of this stage is to prevent overflow
         *       during calculation of exponentials.  With  this stage
         *       we  make  sure  that  all exponentials are calculated
         *       with non-positive argument. If you load (0,0,0,0)  to
         *       (V0,V1,V2,V3), your program will continue  working  -
         *       although with less robustness.
         */
        entryoffs = entrysize*(ntotal-nout);
        v0 = batch4buf->ptr.p_double[entryoffs+0];
        v1 = batch4buf->ptr.p_double[entryoffs+1];
        v2 = batch4buf->ptr.p_double[entryoffs+2];
        v3 = batch4buf->ptr.p_double[entryoffs+3];
        entryoffs = entryoffs+entrysize;
        for(i=1; i<=nout-1; i++)
        {
            v = batch4buf->ptr.p_double[entryoffs+0];
            if( v>v0 )
            {
                v0 = v;
            }
            v = batch4buf->ptr.p_double[entryoffs+1];
            if( v>v1 )
            {
                v1 = v;
            }
            v = batch4buf->ptr.p_double[entryoffs+2];
            if( v>v2 )
            {
                v2 = v;
            }
            v = batch4buf->ptr.p_double[entryoffs+3];
            if( v>v3 )
            {
                v3 = v;
            }
            entryoffs = entryoffs+entrysize;
        }
        
        /*
         * Then,  calculate exponentials and place them to part of the
         * array which  is  located  past  the  last  entry.  We  also
         * calculate sum of exponentials which will be stored past the
         * exponentials.
         */
        entryoffs = entrysize*(ntotal-nout);
        offs0 = entrysize*ntotal;
        s0 = (double)(0);
        s1 = (double)(0);
        s2 = (double)(0);
        s3 = (double)(0);
        for(i=0; i<=nout-1; i++)
        {
            v = ae_exp(batch4buf->ptr.p_double[entryoffs+0]-v0, _state);
            s0 = s0+v;
            batch4buf->ptr.p_double[offs0+0] = v;
            v = ae_exp(batch4buf->ptr.p_double[entryoffs+1]-v1, _state);
            s1 = s1+v;
            batch4buf->ptr.p_double[offs0+1] = v;
            v = ae_exp(batch4buf->ptr.p_double[entryoffs+2]-v2, _state);
            s2 = s2+v;
            batch4buf->ptr.p_double[offs0+2] = v;
            v = ae_exp(batch4buf->ptr.p_double[entryoffs+3]-v3, _state);
            s3 = s3+v;
            batch4buf->ptr.p_double[offs0+3] = v;
            entryoffs = entryoffs+entrysize;
            offs0 = offs0+chunksize;
        }
        offs0 = entrysize*ntotal+2*nout*chunksize;
        batch4buf->ptr.p_double[offs0+0] = s0;
        batch4buf->ptr.p_double[offs0+1] = s1;
        batch4buf->ptr.p_double[offs0+2] = s2;
        batch4buf->ptr.p_double[offs0+3] = s3;
        
        /*
         * Now we have:
         * * Batch4Buf[0...EntrySize*NTotal-1] stores:
         *   * NTotal*ChunkSize neuron output values (SOFTMAX normalization
         *     was not applied to these values),
         *   * NTotal*ChunkSize values of dF/dNET (derivative of neuron
         *     output with respect to its input)
         *   * NTotal*ChunkSize zeros in the elements which correspond to
         *     dError/dOut (derivative of error with respect to neuron output).
         * * Batch4Buf[EntrySize*NTotal...EntrySize*NTotal+ChunkSize*NOut-1] -
         *   stores exponentials of last NOut neurons.
         * * Batch4Buf[EntrySize*NTotal+ChunkSize*NOut-1...EntrySize*NTotal+ChunkSize*2*NOut-1]
         *   - can be used for temporary calculations
         * * Batch4Buf[EntrySize*NTotal+ChunkSize*2*NOut...EntrySize*NTotal+ChunkSize*2*NOut+ChunkSize-1]
         *   - stores sum-of-exponentials
         *
         * Block below calculates derivatives of error function with respect 
         * to non-SOFTMAX-normalized output values of last NOut neurons.
         *
         * It is quite complicated; we do not describe algebra behind it,
         * but if you want you may check it yourself :)
         */
        if( naturalerrorfunc )
        {
            
            /*
             * Calculate  derivative  of  error  with respect to values of
             * output  neurons  PRIOR TO SOFTMAX NORMALIZATION. Because we
             * use natural error function (cross-entropy), we  can  do  so
             * very easy.
             */
            offs0 = entrysize*ntotal+2*nout*chunksize;
            for(k=0; k<=csize-1; k++)
            {
                s = batch4buf->ptr.p_double[offs0+k];
                kl = ae_round(xy->ptr.pp_double[cstart+k][nin], _state);
                offs1 = (ntotal-nout)*entrysize+derroroffs+k;
                offs2 = entrysize*ntotal+k;
                for(i=0; i<=nout-1; i++)
                {
                    if( i==kl )
                    {
                        v = (double)(1);
                    }
                    else
                    {
                        v = (double)(0);
                    }
                    vv = batch4buf->ptr.p_double[offs2];
                    batch4buf->ptr.p_double[offs1] = vv/s-v;
                    *e = *e+mlpbase_safecrossentropy(v, vv/s, _state);
                    offs1 = offs1+entrysize;
                    offs2 = offs2+chunksize;
                }
            }
        }
        else
        {
            
            /*
             * SOFTMAX normalization makes things very difficult.
             * Sorry, we do not dare to describe this esoteric math
             * in details.
             */
            offs0 = entrysize*ntotal+chunksize*2*nout;
            for(k=0; k<=csize-1; k++)
            {
                s = batch4buf->ptr.p_double[offs0+k];
                kl = ae_round(xy->ptr.pp_double[cstart+k][nin], _state);
                vv = (double)(0);
                offs1 = entrysize*ntotal+k;
                offs2 = entrysize*ntotal+nout*chunksize+k;
                for(i=0; i<=nout-1; i++)
                {
                    fown = batch4buf->ptr.p_double[offs1];
                    if( i==kl )
                    {
                        deown = fown/s-1;
                    }
                    else
                    {
                        deown = fown/s;
                    }
                    batch4buf->ptr.p_double[offs2] = deown;
                    vv = vv+deown*fown;
                    *e = *e+deown*deown/2;
                    offs1 = offs1+chunksize;
                    offs2 = offs2+chunksize;
                }
                offs1 = entrysize*ntotal+k;
                offs2 = entrysize*ntotal+nout*chunksize+k;
                for(i=0; i<=nout-1; i++)
                {
                    fown = batch4buf->ptr.p_double[offs1];
                    deown = batch4buf->ptr.p_double[offs2];
                    batch4buf->ptr.p_double[(ntotal-nout+i)*entrysize+derroroffs+k] = (-vv+deown*fown+deown*(s-fown))*fown/ae_sqr(s, _state);
                    offs1 = offs1+chunksize;
                    offs2 = offs2+chunksize;
                }
            }
        }
    }
    else
    {
        
        /*
         * Regression network with sum-of-squares function.
         *
         * For each NOut of last neurons:
         * * calculate difference between actual and desired output
         * * calculate dError/dOut for this neuron (proportional to difference)
         * * store in in last 4 components of entry (these values are used
         *   to start backpropagation)
         * * update error
         */
        for(i=0; i<=nout-1; i++)
        {
            v0 = network->columnsigmas.ptr.p_double[nin+i];
            v1 = network->columnmeans.ptr.p_double[nin+i];
            entryoffs = entrysize*(ntotal-nout+i);
            offs0 = entryoffs;
            offs1 = entryoffs+derroroffs;
            for(j=0; j<=csize-1; j++)
            {
                v = batch4buf->ptr.p_double[offs0+j]*v0+v1-xy->ptr.pp_double[cstart+j][nin+i];
                batch4buf->ptr.p_double[offs1+j] = v*v0;
                *e = *e+v*v/2;
            }
        }
    }
    
    /*
     * Backpropagation
     */
    for(neuronidx=ntotal-1; neuronidx>=0; neuronidx--)
    {
        entryoffs = entrysize*neuronidx;
        offs = istart+neuronidx*mlpbase_nfieldwidth;
        neurontype = network->structinfo.ptr.p_int[offs+0];
        if( neurontype>0||neurontype==-5 )
        {
            
            /*
             * Activation function
             */
            srcneuronidx = network->structinfo.ptr.p_int[offs+2];
            srcentryoffs = entrysize*srcneuronidx;
            offs0 = srcentryoffs+derroroffs;
            offs1 = entryoffs+derroroffs;
            offs2 = entryoffs+dfoffs;
            batch4buf->ptr.p_double[offs0+0] = batch4buf->ptr.p_double[offs0+0]+batch4buf->ptr.p_double[offs1+0]*batch4buf->ptr.p_double[offs2+0];
            batch4buf->ptr.p_double[offs0+1] = batch4buf->ptr.p_double[offs0+1]+batch4buf->ptr.p_double[offs1+1]*batch4buf->ptr.p_double[offs2+1];
            batch4buf->ptr.p_double[offs0+2] = batch4buf->ptr.p_double[offs0+2]+batch4buf->ptr.p_double[offs1+2]*batch4buf->ptr.p_double[offs2+2];
            batch4buf->ptr.p_double[offs0+3] = batch4buf->ptr.p_double[offs0+3]+batch4buf->ptr.p_double[offs1+3]*batch4buf->ptr.p_double[offs2+3];
            continue;
        }
        if( neurontype==0 )
        {
            
            /*
             * Adaptive summator
             */
            nweights = network->structinfo.ptr.p_int[offs+1];
            srcneuronidx = network->structinfo.ptr.p_int[offs+2];
            srcentryoffs = entrysize*srcneuronidx;
            srcweightidx = network->structinfo.ptr.p_int[offs+3];
            v0 = batch4buf->ptr.p_double[entryoffs+derroroffs+0];
            v1 = batch4buf->ptr.p_double[entryoffs+derroroffs+1];
            v2 = batch4buf->ptr.p_double[entryoffs+derroroffs+2];
            v3 = batch4buf->ptr.p_double[entryoffs+derroroffs+3];
            for(j=0; j<=nweights-1; j++)
            {
                offs0 = srcentryoffs;
                offs1 = srcentryoffs+derroroffs;
                v = network->weights.ptr.p_double[srcweightidx];
                hpcbuf->ptr.p_double[srcweightidx] = hpcbuf->ptr.p_double[srcweightidx]+batch4buf->ptr.p_double[offs0+0]*v0+batch4buf->ptr.p_double[offs0+1]*v1+batch4buf->ptr.p_double[offs0+2]*v2+batch4buf->ptr.p_double[offs0+3]*v3;
                batch4buf->ptr.p_double[offs1+0] = batch4buf->ptr.p_double[offs1+0]+v*v0;
                batch4buf->ptr.p_double[offs1+1] = batch4buf->ptr.p_double[offs1+1]+v*v1;
                batch4buf->ptr.p_double[offs1+2] = batch4buf->ptr.p_double[offs1+2]+v*v2;
                batch4buf->ptr.p_double[offs1+3] = batch4buf->ptr.p_double[offs1+3]+v*v3;
                srcentryoffs = srcentryoffs+entrysize;
                srcweightidx = srcweightidx+1;
            }
            continue;
        }
        if( neurontype<0 )
        {
            bflag = ae_false;
            if( (neurontype==-2||neurontype==-3)||neurontype==-4 )
            {
                
                /*
                 * Special neuron type, no back-propagation required
                 */
                bflag = ae_true;
            }
            ae_assert(bflag, "MLPInternalCalculateGradient: unknown neuron type!", _state);
            continue;
        }
    }
}


static void mlpbase_mlpchunkedprocess(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t cstart,
     ae_int_t csize,
     /* Real    */ ae_vector* batch4buf,
     /* Real    */ ae_vector* hpcbuf,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t ntotal;
    ae_int_t nin;
    ae_int_t nout;
    ae_int_t offs;
    double f;
    double df;
    double d2f;
    double v;
    ae_bool bflag;
    ae_int_t istart;
    ae_int_t entrysize;
    ae_int_t entryoffs;
    ae_int_t neuronidx;
    ae_int_t srcentryoffs;
    ae_int_t srcneuronidx;
    ae_int_t srcweightidx;
    ae_int_t neurontype;
    ae_int_t nweights;
    ae_int_t offs0;
    double v0;
    double v1;
    double v2;
    double v3;
    double s0;
    double s1;
    double s2;
    double s3;
    ae_int_t chunksize;


    chunksize = 4;
    ae_assert(csize<=chunksize, "MLPChunkedProcess: internal error (CSize>ChunkSize)", _state);
    
    /*
     * Try to use HPC core, if possible
     */
    if( hpcchunkedprocess(&network->weights, &network->structinfo, &network->columnmeans, &network->columnsigmas, xy, cstart, csize, batch4buf, hpcbuf, _state) )
    {
        return;
    }
    
    /*
     * Read network geometry, prepare data
     */
    nin = network->structinfo.ptr.p_int[1];
    nout = network->structinfo.ptr.p_int[2];
    ntotal = network->structinfo.ptr.p_int[3];
    istart = network->structinfo.ptr.p_int[5];
    entrysize = 4;
    
    /*
     * Fill Batch4Buf by zeros.
     *
     * THIS STAGE IS VERY IMPORTANT!
     *
     * We fill all components of entry - neuron values, dF/dNET, dError/dF.
     * It allows us to easily handle  situations  when  CSize<ChunkSize  by
     * simply  working  with  ALL  components  of  Batch4Buf,  without ever
     * looking at CSize.
     */
    for(i=0; i<=entrysize*ntotal-1; i++)
    {
        batch4buf->ptr.p_double[i] = (double)(0);
    }
    
    /*
     * Forward pass:
     * 1. Load data into Batch4Buf. If CSize<ChunkSize, data are padded by zeros.
     * 2. Perform forward pass through network
     */
    for(i=0; i<=nin-1; i++)
    {
        entryoffs = entrysize*i;
        for(j=0; j<=csize-1; j++)
        {
            if( ae_fp_neq(network->columnsigmas.ptr.p_double[i],(double)(0)) )
            {
                batch4buf->ptr.p_double[entryoffs+j] = (xy->ptr.pp_double[cstart+j][i]-network->columnmeans.ptr.p_double[i])/network->columnsigmas.ptr.p_double[i];
            }
            else
            {
                batch4buf->ptr.p_double[entryoffs+j] = xy->ptr.pp_double[cstart+j][i]-network->columnmeans.ptr.p_double[i];
            }
        }
    }
    for(neuronidx=0; neuronidx<=ntotal-1; neuronidx++)
    {
        entryoffs = entrysize*neuronidx;
        offs = istart+neuronidx*mlpbase_nfieldwidth;
        neurontype = network->structinfo.ptr.p_int[offs+0];
        if( neurontype>0||neurontype==-5 )
        {
            
            /*
             * "activation function" neuron, which takes value of neuron SrcNeuronIdx
             * and applies activation function to it.
             *
             * This neuron has no weights and no tunable parameters.
             */
            srcneuronidx = network->structinfo.ptr.p_int[offs+2];
            srcentryoffs = entrysize*srcneuronidx;
            mlpactivationfunction(batch4buf->ptr.p_double[srcentryoffs+0], neurontype, &f, &df, &d2f, _state);
            batch4buf->ptr.p_double[entryoffs+0] = f;
            mlpactivationfunction(batch4buf->ptr.p_double[srcentryoffs+1], neurontype, &f, &df, &d2f, _state);
            batch4buf->ptr.p_double[entryoffs+1] = f;
            mlpactivationfunction(batch4buf->ptr.p_double[srcentryoffs+2], neurontype, &f, &df, &d2f, _state);
            batch4buf->ptr.p_double[entryoffs+2] = f;
            mlpactivationfunction(batch4buf->ptr.p_double[srcentryoffs+3], neurontype, &f, &df, &d2f, _state);
            batch4buf->ptr.p_double[entryoffs+3] = f;
            continue;
        }
        if( neurontype==0 )
        {
            
            /*
             * "adaptive summator" neuron, whose output is a weighted sum of inputs.
             * It has weights, but has no activation function.
             */
            nweights = network->structinfo.ptr.p_int[offs+1];
            srcneuronidx = network->structinfo.ptr.p_int[offs+2];
            srcentryoffs = entrysize*srcneuronidx;
            srcweightidx = network->structinfo.ptr.p_int[offs+3];
            v0 = (double)(0);
            v1 = (double)(0);
            v2 = (double)(0);
            v3 = (double)(0);
            for(j=0; j<=nweights-1; j++)
            {
                v = network->weights.ptr.p_double[srcweightidx];
                srcweightidx = srcweightidx+1;
                v0 = v0+v*batch4buf->ptr.p_double[srcentryoffs+0];
                v1 = v1+v*batch4buf->ptr.p_double[srcentryoffs+1];
                v2 = v2+v*batch4buf->ptr.p_double[srcentryoffs+2];
                v3 = v3+v*batch4buf->ptr.p_double[srcentryoffs+3];
                srcentryoffs = srcentryoffs+entrysize;
            }
            batch4buf->ptr.p_double[entryoffs+0] = v0;
            batch4buf->ptr.p_double[entryoffs+1] = v1;
            batch4buf->ptr.p_double[entryoffs+2] = v2;
            batch4buf->ptr.p_double[entryoffs+3] = v3;
            continue;
        }
        if( neurontype<0 )
        {
            bflag = ae_false;
            if( neurontype==-2 )
            {
                
                /*
                 * Input neuron, left unchanged
                 */
                bflag = ae_true;
            }
            if( neurontype==-3 )
            {
                
                /*
                 * "-1" neuron
                 */
                batch4buf->ptr.p_double[entryoffs+0] = (double)(-1);
                batch4buf->ptr.p_double[entryoffs+1] = (double)(-1);
                batch4buf->ptr.p_double[entryoffs+2] = (double)(-1);
                batch4buf->ptr.p_double[entryoffs+3] = (double)(-1);
                bflag = ae_true;
            }
            if( neurontype==-4 )
            {
                
                /*
                 * "0" neuron
                 */
                batch4buf->ptr.p_double[entryoffs+0] = (double)(0);
                batch4buf->ptr.p_double[entryoffs+1] = (double)(0);
                batch4buf->ptr.p_double[entryoffs+2] = (double)(0);
                batch4buf->ptr.p_double[entryoffs+3] = (double)(0);
                bflag = ae_true;
            }
            ae_assert(bflag, "MLPChunkedProcess: internal error - unknown neuron type!", _state);
            continue;
        }
    }
    
    /*
     * SOFTMAX normalization or scaling.
     */
    ae_assert(network->structinfo.ptr.p_int[6]==0||network->structinfo.ptr.p_int[6]==1, "MLPChunkedProcess: unknown normalization type!", _state);
    if( network->structinfo.ptr.p_int[6]==1 )
    {
        
        /*
         * SOFTMAX-normalized network.
         *
         * First,  calculate (V0,V1,V2,V3)  -  component-wise  maximum
         * of output neurons. This vector of maximum  values  will  be
         * used for normalization  of  outputs  prior  to  calculating
         * exponentials.
         *
         * NOTE: the only purpose of this stage is to prevent overflow
         *       during calculation of exponentials.  With  this stage
         *       we  make  sure  that  all exponentials are calculated
         *       with non-positive argument. If you load (0,0,0,0)  to
         *       (V0,V1,V2,V3), your program will continue  working  -
         *       although with less robustness.
         */
        entryoffs = entrysize*(ntotal-nout);
        v0 = batch4buf->ptr.p_double[entryoffs+0];
        v1 = batch4buf->ptr.p_double[entryoffs+1];
        v2 = batch4buf->ptr.p_double[entryoffs+2];
        v3 = batch4buf->ptr.p_double[entryoffs+3];
        entryoffs = entryoffs+entrysize;
        for(i=1; i<=nout-1; i++)
        {
            v = batch4buf->ptr.p_double[entryoffs+0];
            if( v>v0 )
            {
                v0 = v;
            }
            v = batch4buf->ptr.p_double[entryoffs+1];
            if( v>v1 )
            {
                v1 = v;
            }
            v = batch4buf->ptr.p_double[entryoffs+2];
            if( v>v2 )
            {
                v2 = v;
            }
            v = batch4buf->ptr.p_double[entryoffs+3];
            if( v>v3 )
            {
                v3 = v;
            }
            entryoffs = entryoffs+entrysize;
        }
        
        /*
         * Then,  calculate exponentials and place them to part of the
         * array which  is  located  past  the  last  entry.  We  also
         * calculate sum of exponentials.
         */
        entryoffs = entrysize*(ntotal-nout);
        offs0 = entrysize*ntotal;
        s0 = (double)(0);
        s1 = (double)(0);
        s2 = (double)(0);
        s3 = (double)(0);
        for(i=0; i<=nout-1; i++)
        {
            v = ae_exp(batch4buf->ptr.p_double[entryoffs+0]-v0, _state);
            s0 = s0+v;
            batch4buf->ptr.p_double[offs0+0] = v;
            v = ae_exp(batch4buf->ptr.p_double[entryoffs+1]-v1, _state);
            s1 = s1+v;
            batch4buf->ptr.p_double[offs0+1] = v;
            v = ae_exp(batch4buf->ptr.p_double[entryoffs+2]-v2, _state);
            s2 = s2+v;
            batch4buf->ptr.p_double[offs0+2] = v;
            v = ae_exp(batch4buf->ptr.p_double[entryoffs+3]-v3, _state);
            s3 = s3+v;
            batch4buf->ptr.p_double[offs0+3] = v;
            entryoffs = entryoffs+entrysize;
            offs0 = offs0+chunksize;
        }
        
        /*
         * Write SOFTMAX-normalized values to the output array.
         */
        offs0 = entrysize*ntotal;
        for(i=0; i<=nout-1; i++)
        {
            if( csize>0 )
            {
                xy->ptr.pp_double[cstart+0][nin+i] = batch4buf->ptr.p_double[offs0+0]/s0;
            }
            if( csize>1 )
            {
                xy->ptr.pp_double[cstart+1][nin+i] = batch4buf->ptr.p_double[offs0+1]/s1;
            }
            if( csize>2 )
            {
                xy->ptr.pp_double[cstart+2][nin+i] = batch4buf->ptr.p_double[offs0+2]/s2;
            }
            if( csize>3 )
            {
                xy->ptr.pp_double[cstart+3][nin+i] = batch4buf->ptr.p_double[offs0+3]/s3;
            }
            offs0 = offs0+chunksize;
        }
    }
    else
    {
        
        /*
         * Regression network with sum-of-squares function.
         *
         * For each NOut of last neurons:
         * * calculate difference between actual and desired output
         * * calculate dError/dOut for this neuron (proportional to difference)
         * * store in in last 4 components of entry (these values are used
         *   to start backpropagation)
         * * update error
         */
        for(i=0; i<=nout-1; i++)
        {
            v0 = network->columnsigmas.ptr.p_double[nin+i];
            v1 = network->columnmeans.ptr.p_double[nin+i];
            entryoffs = entrysize*(ntotal-nout+i);
            for(j=0; j<=csize-1; j++)
            {
                xy->ptr.pp_double[cstart+j][nin+i] = batch4buf->ptr.p_double[entryoffs+j]*v0+v1;
            }
        }
    }
}


/*************************************************************************
Returns T*Ln(T/Z), guarded against overflow/underflow.
Internal subroutine.
*************************************************************************/
static double mlpbase_safecrossentropy(double t,
     double z,
     ae_state *_state)
{
    double r;
    double result;


    if( ae_fp_eq(t,(double)(0)) )
    {
        result = (double)(0);
    }
    else
    {
        if( ae_fp_greater(ae_fabs(z, _state),(double)(1)) )
        {
            
            /*
             * Shouldn't be the case with softmax,
             * but we just want to be sure.
             */
            if( ae_fp_eq(t/z,(double)(0)) )
            {
                r = ae_minrealnumber;
            }
            else
            {
                r = t/z;
            }
        }
        else
        {
            
            /*
             * Normal case
             */
            if( ae_fp_eq(z,(double)(0))||ae_fp_greater_eq(ae_fabs(t, _state),ae_maxrealnumber*ae_fabs(z, _state)) )
            {
                r = ae_maxrealnumber;
            }
            else
            {
                r = t/z;
            }
        }
        result = t*ae_log(r, _state);
    }
    return result;
}


/*************************************************************************
This function performs backward pass of neural network randimization:
* it assumes that Network.Weights stores standard deviation of weights
  (weights are not generated yet, only their deviations are present)
* it sets deviations of weights which feed NeuronIdx-th neuron to specified value
* it recursively passes to deeper neuron and modifies their weights
* it stops after encountering nonlinear neurons, linear activation function,
  input neurons, "0" and "-1" neurons

  -- ALGLIB --
     Copyright 27.06.2013 by Bochkanov Sergey
*************************************************************************/
static void mlpbase_randomizebackwardpass(multilayerperceptron* network,
     ae_int_t neuronidx,
     double v,
     ae_state *_state)
{
    ae_int_t istart;
    ae_int_t neurontype;
    ae_int_t n1;
    ae_int_t n2;
    ae_int_t w1;
    ae_int_t w2;
    ae_int_t offs;
    ae_int_t i;


    istart = network->structinfo.ptr.p_int[5];
    neurontype = network->structinfo.ptr.p_int[istart+neuronidx*mlpbase_nfieldwidth+0];
    if( neurontype==-2 )
    {
        
        /*
         * Input neuron - stop
         */
        return;
    }
    if( neurontype==-3 )
    {
        
        /*
         * "-1" neuron: stop
         */
        return;
    }
    if( neurontype==-4 )
    {
        
        /*
         * "0" neuron: stop
         */
        return;
    }
    if( neurontype==0 )
    {
        
        /*
         * Adaptive summator neuron:
         * * modify deviations of its weights
         * * recursively call this function for its inputs
         */
        offs = istart+neuronidx*mlpbase_nfieldwidth;
        n1 = network->structinfo.ptr.p_int[offs+2];
        n2 = n1+network->structinfo.ptr.p_int[offs+1]-1;
        w1 = network->structinfo.ptr.p_int[offs+3];
        w2 = w1+network->structinfo.ptr.p_int[offs+1]-1;
        for(i=w1; i<=w2; i++)
        {
            network->weights.ptr.p_double[i] = v;
        }
        for(i=n1; i<=n2; i++)
        {
            mlpbase_randomizebackwardpass(network, i, v, _state);
        }
        return;
    }
    if( neurontype==-5 )
    {
        
        /*
         * Linear activation function: stop
         */
        return;
    }
    if( neurontype>0 )
    {
        
        /*
         * Nonlinear activation function: stop
         */
        return;
    }
    ae_assert(ae_false, "RandomizeBackwardPass: unexpected neuron type", _state);
}


void _modelerrors_init(void* _p, ae_state *_state)
{
    modelerrors *p = (modelerrors*)_p;
    ae_touch_ptr((void*)p);
}


void _modelerrors_init_copy(void* _dst, void* _src, ae_state *_state)
{
    modelerrors *dst = (modelerrors*)_dst;
    modelerrors *src = (modelerrors*)_src;
    dst->relclserror = src->relclserror;
    dst->avgce = src->avgce;
    dst->rmserror = src->rmserror;
    dst->avgerror = src->avgerror;
    dst->avgrelerror = src->avgrelerror;
}


void _modelerrors_clear(void* _p)
{
    modelerrors *p = (modelerrors*)_p;
    ae_touch_ptr((void*)p);
}


void _modelerrors_destroy(void* _p)
{
    modelerrors *p = (modelerrors*)_p;
    ae_touch_ptr((void*)p);
}


void _smlpgrad_init(void* _p, ae_state *_state)
{
    smlpgrad *p = (smlpgrad*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->g, 0, DT_REAL, _state);
}


void _smlpgrad_init_copy(void* _dst, void* _src, ae_state *_state)
{
    smlpgrad *dst = (smlpgrad*)_dst;
    smlpgrad *src = (smlpgrad*)_src;
    dst->f = src->f;
    ae_vector_init_copy(&dst->g, &src->g, _state);
}


void _smlpgrad_clear(void* _p)
{
    smlpgrad *p = (smlpgrad*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->g);
}


void _smlpgrad_destroy(void* _p)
{
    smlpgrad *p = (smlpgrad*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->g);
}


void _multilayerperceptron_init(void* _p, ae_state *_state)
{
    multilayerperceptron *p = (multilayerperceptron*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->hllayersizes, 0, DT_INT, _state);
    ae_vector_init(&p->hlconnections, 0, DT_INT, _state);
    ae_vector_init(&p->hlneurons, 0, DT_INT, _state);
    ae_vector_init(&p->structinfo, 0, DT_INT, _state);
    ae_vector_init(&p->weights, 0, DT_REAL, _state);
    ae_vector_init(&p->columnmeans, 0, DT_REAL, _state);
    ae_vector_init(&p->columnsigmas, 0, DT_REAL, _state);
    ae_vector_init(&p->neurons, 0, DT_REAL, _state);
    ae_vector_init(&p->dfdnet, 0, DT_REAL, _state);
    ae_vector_init(&p->derror, 0, DT_REAL, _state);
    ae_vector_init(&p->x, 0, DT_REAL, _state);
    ae_vector_init(&p->y, 0, DT_REAL, _state);
    ae_matrix_init(&p->xy, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->xyrow, 0, DT_REAL, _state);
    ae_vector_init(&p->nwbuf, 0, DT_REAL, _state);
    ae_vector_init(&p->integerbuf, 0, DT_INT, _state);
    _modelerrors_init(&p->err, _state);
    ae_vector_init(&p->rndbuf, 0, DT_REAL, _state);
    ae_shared_pool_init(&p->buf, _state);
    ae_shared_pool_init(&p->gradbuf, _state);
    ae_matrix_init(&p->dummydxy, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&p->dummysxy, _state);
    ae_vector_init(&p->dummyidx, 0, DT_INT, _state);
    ae_shared_pool_init(&p->dummypool, _state);
}


void _multilayerperceptron_init_copy(void* _dst, void* _src, ae_state *_state)
{
    multilayerperceptron *dst = (multilayerperceptron*)_dst;
    multilayerperceptron *src = (multilayerperceptron*)_src;
    dst->hlnetworktype = src->hlnetworktype;
    dst->hlnormtype = src->hlnormtype;
    ae_vector_init_copy(&dst->hllayersizes, &src->hllayersizes, _state);
    ae_vector_init_copy(&dst->hlconnections, &src->hlconnections, _state);
    ae_vector_init_copy(&dst->hlneurons, &src->hlneurons, _state);
    ae_vector_init_copy(&dst->structinfo, &src->structinfo, _state);
    ae_vector_init_copy(&dst->weights, &src->weights, _state);
    ae_vector_init_copy(&dst->columnmeans, &src->columnmeans, _state);
    ae_vector_init_copy(&dst->columnsigmas, &src->columnsigmas, _state);
    ae_vector_init_copy(&dst->neurons, &src->neurons, _state);
    ae_vector_init_copy(&dst->dfdnet, &src->dfdnet, _state);
    ae_vector_init_copy(&dst->derror, &src->derror, _state);
    ae_vector_init_copy(&dst->x, &src->x, _state);
    ae_vector_init_copy(&dst->y, &src->y, _state);
    ae_matrix_init_copy(&dst->xy, &src->xy, _state);
    ae_vector_init_copy(&dst->xyrow, &src->xyrow, _state);
    ae_vector_init_copy(&dst->nwbuf, &src->nwbuf, _state);
    ae_vector_init_copy(&dst->integerbuf, &src->integerbuf, _state);
    _modelerrors_init_copy(&dst->err, &src->err, _state);
    ae_vector_init_copy(&dst->rndbuf, &src->rndbuf, _state);
    ae_shared_pool_init_copy(&dst->buf, &src->buf, _state);
    ae_shared_pool_init_copy(&dst->gradbuf, &src->gradbuf, _state);
    ae_matrix_init_copy(&dst->dummydxy, &src->dummydxy, _state);
    _sparsematrix_init_copy(&dst->dummysxy, &src->dummysxy, _state);
    ae_vector_init_copy(&dst->dummyidx, &src->dummyidx, _state);
    ae_shared_pool_init_copy(&dst->dummypool, &src->dummypool, _state);
}


void _multilayerperceptron_clear(void* _p)
{
    multilayerperceptron *p = (multilayerperceptron*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->hllayersizes);
    ae_vector_clear(&p->hlconnections);
    ae_vector_clear(&p->hlneurons);
    ae_vector_clear(&p->structinfo);
    ae_vector_clear(&p->weights);
    ae_vector_clear(&p->columnmeans);
    ae_vector_clear(&p->columnsigmas);
    ae_vector_clear(&p->neurons);
    ae_vector_clear(&p->dfdnet);
    ae_vector_clear(&p->derror);
    ae_vector_clear(&p->x);
    ae_vector_clear(&p->y);
    ae_matrix_clear(&p->xy);
    ae_vector_clear(&p->xyrow);
    ae_vector_clear(&p->nwbuf);
    ae_vector_clear(&p->integerbuf);
    _modelerrors_clear(&p->err);
    ae_vector_clear(&p->rndbuf);
    ae_shared_pool_clear(&p->buf);
    ae_shared_pool_clear(&p->gradbuf);
    ae_matrix_clear(&p->dummydxy);
    _sparsematrix_clear(&p->dummysxy);
    ae_vector_clear(&p->dummyidx);
    ae_shared_pool_clear(&p->dummypool);
}


void _multilayerperceptron_destroy(void* _p)
{
    multilayerperceptron *p = (multilayerperceptron*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->hllayersizes);
    ae_vector_destroy(&p->hlconnections);
    ae_vector_destroy(&p->hlneurons);
    ae_vector_destroy(&p->structinfo);
    ae_vector_destroy(&p->weights);
    ae_vector_destroy(&p->columnmeans);
    ae_vector_destroy(&p->columnsigmas);
    ae_vector_destroy(&p->neurons);
    ae_vector_destroy(&p->dfdnet);
    ae_vector_destroy(&p->derror);
    ae_vector_destroy(&p->x);
    ae_vector_destroy(&p->y);
    ae_matrix_destroy(&p->xy);
    ae_vector_destroy(&p->xyrow);
    ae_vector_destroy(&p->nwbuf);
    ae_vector_destroy(&p->integerbuf);
    _modelerrors_destroy(&p->err);
    ae_vector_destroy(&p->rndbuf);
    ae_shared_pool_destroy(&p->buf);
    ae_shared_pool_destroy(&p->gradbuf);
    ae_matrix_destroy(&p->dummydxy);
    _sparsematrix_destroy(&p->dummysxy);
    ae_vector_destroy(&p->dummyidx);
    ae_shared_pool_destroy(&p->dummypool);
}


/*$ End $*/
