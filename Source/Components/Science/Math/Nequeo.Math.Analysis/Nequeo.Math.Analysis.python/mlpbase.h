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

#ifndef _mlpbase_h
#define _mlpbase_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "tsort.h"
#include "basicstatops.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"
#include "basestat.h"
#include "bdss.h"
#include "hpccores.h"
#include "scodes.h"
#include "hqrnd.h"
#include "sparse.h"


/*$ Declarations $*/


/*************************************************************************
Model's errors:
    * RelCLSError   -   fraction of misclassified cases.
    * AvgCE         -   acerage cross-entropy
    * RMSError      -   root-mean-square error
    * AvgError      -   average error
    * AvgRelError   -   average relative error
    
NOTE 1: RelCLSError/AvgCE are zero on regression problems.

NOTE 2: on classification problems  RMSError/AvgError/AvgRelError  contain
        errors in prediction of posterior probabilities
*************************************************************************/
typedef struct
{
    double relclserror;
    double avgce;
    double rmserror;
    double avgerror;
    double avgrelerror;
} modelerrors;


/*************************************************************************
This structure is used to store MLP error and gradient.
*************************************************************************/
typedef struct
{
    double f;
    ae_vector g;
} smlpgrad;


typedef struct
{
    ae_int_t hlnetworktype;
    ae_int_t hlnormtype;
    ae_vector hllayersizes;
    ae_vector hlconnections;
    ae_vector hlneurons;
    ae_vector structinfo;
    ae_vector weights;
    ae_vector columnmeans;
    ae_vector columnsigmas;
    ae_vector neurons;
    ae_vector dfdnet;
    ae_vector derror;
    ae_vector x;
    ae_vector y;
    ae_matrix xy;
    ae_vector xyrow;
    ae_vector nwbuf;
    ae_vector integerbuf;
    modelerrors err;
    ae_vector rndbuf;
    ae_shared_pool buf;
    ae_shared_pool gradbuf;
    ae_matrix dummydxy;
    sparsematrix dummysxy;
    ae_vector dummyidx;
    ae_shared_pool dummypool;
} multilayerperceptron;


/*$ Body $*/


/*************************************************************************
This function returns number of weights  updates  which  is  required  for
gradient calculation problem to be splitted.
*************************************************************************/
ae_int_t mlpgradsplitcost(ae_state *_state);


/*************************************************************************
This  function  returns  number  of elements in subset of dataset which is
required for gradient calculation problem to be splitted.
*************************************************************************/
ae_int_t mlpgradsplitsize(ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


/*************************************************************************
Same as MLPCreateC0, but with one non-linear hidden layer.

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlpcreatec1(ae_int_t nin,
     ae_int_t nhid,
     ae_int_t nout,
     multilayerperceptron* network,
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


/*************************************************************************
This function compares architectures of neural networks.  Only  geometries
are compared, weights and other parameters are not tested.

  -- ALGLIB --
     Copyright 20.06.2013 by Bochkanov Sergey
*************************************************************************/
ae_bool mlpsamearchitecture(multilayerperceptron* network1,
     multilayerperceptron* network2,
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


/*************************************************************************
Randomization of neural network weights

  -- ALGLIB --
     Copyright 06.11.2007 by Bochkanov Sergey
*************************************************************************/
void mlprandomize(multilayerperceptron* network, ae_state *_state);


/*************************************************************************
Randomization of neural network weights and standartisator

  -- ALGLIB --
     Copyright 10.03.2008 by Bochkanov Sergey
*************************************************************************/
void mlprandomizefull(multilayerperceptron* network, ae_state *_state);


/*************************************************************************
Internal subroutine.

  -- ALGLIB --
     Copyright 30.03.2008 by Bochkanov Sergey
*************************************************************************/
void mlpinitpreprocessor(multilayerperceptron* network,
     /* Real    */ ae_matrix* xy,
     ae_int_t ssize,
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


/*************************************************************************
Returns number of "internal", low-level neurons in the network (one  which
is stored in StructInfo).

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
ae_int_t mlpntotal(multilayerperceptron* network, ae_state *_state);


/*************************************************************************
Returns number of inputs.

  -- ALGLIB --
     Copyright 19.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_int_t mlpgetinputscount(multilayerperceptron* network,
     ae_state *_state);


/*************************************************************************
Returns number of outputs.

  -- ALGLIB --
     Copyright 19.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_int_t mlpgetoutputscount(multilayerperceptron* network,
     ae_state *_state);


/*************************************************************************
Returns number of weights.

  -- ALGLIB --
     Copyright 19.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_int_t mlpgetweightscount(multilayerperceptron* network,
     ae_state *_state);


/*************************************************************************
Tells whether network is SOFTMAX-normalized (i.e. classifier) or not.

  -- ALGLIB --
     Copyright 04.11.2007 by Bochkanov Sergey
*************************************************************************/
ae_bool mlpissoftmax(multilayerperceptron* network, ae_state *_state);


/*************************************************************************
This function returns total number of layers (including input, hidden and
output layers).

  -- ALGLIB --
     Copyright 25.03.2011 by Bochkanov Sergey
*************************************************************************/
ae_int_t mlpgetlayerscount(multilayerperceptron* network,
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);
double _pexec_mlperror(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t npoints, ae_state *_state);


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
     ae_state *_state);
double _pexec_mlperrorsparse(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t npoints, ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);
ae_int_t _pexec_mlpclserror(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t npoints, ae_state *_state);


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
     ae_state *_state);
double _pexec_mlprelclserror(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t npoints, ae_state *_state);


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
     ae_state *_state);
double _pexec_mlprelclserrorsparse(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t npoints, ae_state *_state);


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
     ae_state *_state);
double _pexec_mlpavgce(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t npoints, ae_state *_state);


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
     ae_state *_state);
double _pexec_mlpavgcesparse(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t npoints, ae_state *_state);


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
     ae_state *_state);
double _pexec_mlprmserror(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t npoints, ae_state *_state);


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
     ae_state *_state);
double _pexec_mlprmserrorsparse(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t npoints, ae_state *_state);


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
     ae_state *_state);
double _pexec_mlpavgerror(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t npoints, ae_state *_state);


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
     ae_state *_state);
double _pexec_mlpavgerrorsparse(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t npoints, ae_state *_state);


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
     ae_state *_state);
double _pexec_mlpavgrelerror(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t npoints, ae_state *_state);


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
     ae_state *_state);
double _pexec_mlpavgrelerrorsparse(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t npoints, ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);
void _pexec_mlpgradbatch(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t ssize,
    double* e,
    /* Real    */ ae_vector* grad, ae_state *_state);


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
     ae_state *_state);
void _pexec_mlpgradbatchsparse(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t ssize,
    double* e,
    /* Real    */ ae_vector* grad, ae_state *_state);


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
     ae_state *_state);
void _pexec_mlpgradbatchsubset(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t setsize,
    /* Integer */ ae_vector* idx,
    ae_int_t subsetsize,
    double* e,
    /* Real    */ ae_vector* grad, ae_state *_state);


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
     ae_state *_state);
void _pexec_mlpgradbatchsparsesubset(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t setsize,
    /* Integer */ ae_vector* idx,
    ae_int_t subsetsize,
    double* e,
    /* Real    */ ae_vector* grad, ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


/*************************************************************************
Serializer: allocation

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void mlpalloc(ae_serializer* s,
     multilayerperceptron* network,
     ae_state *_state);


/*************************************************************************
Serializer: serialization

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void mlpserialize(ae_serializer* s,
     multilayerperceptron* network,
     ae_state *_state);


/*************************************************************************
Serializer: unserialization

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void mlpunserialize(ae_serializer* s,
     multilayerperceptron* network,
     ae_state *_state);


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
     ae_state *_state);
void _pexec_mlpallerrorssubset(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t setsize,
    /* Integer */ ae_vector* subset,
    ae_int_t subsetsize,
    modelerrors* rep, ae_state *_state);


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
     ae_state *_state);
void _pexec_mlpallerrorssparsesubset(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t setsize,
    /* Integer */ ae_vector* subset,
    ae_int_t subsetsize,
    modelerrors* rep, ae_state *_state);


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
     ae_state *_state);
double _pexec_mlperrorsubset(multilayerperceptron* network,
    /* Real    */ ae_matrix* xy,
    ae_int_t setsize,
    /* Integer */ ae_vector* subset,
    ae_int_t subsetsize, ae_state *_state);


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
     ae_state *_state);
double _pexec_mlperrorsparsesubset(multilayerperceptron* network,
    sparsematrix* xy,
    ae_int_t setsize,
    /* Integer */ ae_vector* subset,
    ae_int_t subsetsize, ae_state *_state);


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
     ae_state *_state);
void _modelerrors_init(void* _p, ae_state *_state);
void _modelerrors_init_copy(void* _dst, void* _src, ae_state *_state);
void _modelerrors_clear(void* _p);
void _modelerrors_destroy(void* _p);
void _smlpgrad_init(void* _p, ae_state *_state);
void _smlpgrad_init_copy(void* _dst, void* _src, ae_state *_state);
void _smlpgrad_clear(void* _p);
void _smlpgrad_destroy(void* _p);
void _multilayerperceptron_init(void* _p, ae_state *_state);
void _multilayerperceptron_init_copy(void* _dst, void* _src, ae_state *_state);
void _multilayerperceptron_clear(void* _p);
void _multilayerperceptron_destroy(void* _p);


/*$ End $*/
#endif

