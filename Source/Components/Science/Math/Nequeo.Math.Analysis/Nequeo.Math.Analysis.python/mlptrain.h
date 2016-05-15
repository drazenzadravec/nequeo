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

#ifndef _mlptrain_h
#define _mlptrain_h

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
#include "mlpbase.h"
#include "mlpe.h"
#include "reflections.h"
#include "creflections.h"
#include "matgen.h"
#include "rotations.h"
#include "trfac.h"
#include "trlinsolve.h"
#include "safesolve.h"
#include "rcond.h"
#include "matinv.h"
#include "linmin.h"
#include "hblas.h"
#include "sblas.h"
#include "ortfac.h"
#include "blas.h"
#include "bdsvd.h"
#include "svd.h"
#include "optserv.h"
#include "fbls.h"
#include "minlbfgs.h"
#include "xblas.h"
#include "densesolver.h"


/*$ Declarations $*/


/*************************************************************************
Training report:
    * RelCLSError   -   fraction of misclassified cases.
    * AvgCE         -   acerage cross-entropy
    * RMSError      -   root-mean-square error
    * AvgError      -   average error
    * AvgRelError   -   average relative error
    * NGrad         -   number of gradient calculations
    * NHess         -   number of Hessian calculations
    * NCholesky     -   number of Cholesky decompositions
    
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
    ae_int_t ngrad;
    ae_int_t nhess;
    ae_int_t ncholesky;
} mlpreport;


/*************************************************************************
Cross-validation estimates of generalization error
*************************************************************************/
typedef struct
{
    double relclserror;
    double avgce;
    double rmserror;
    double avgerror;
    double avgrelerror;
} mlpcvreport;


/*************************************************************************
Temporary data structures used by following functions:
* TrainNetworkX
* StartTrainingX
* ContinueTrainingX

This structure contains:
* network being trained
* fully initialized LBFGS optimizer (we have to call MinLBFGSRestartFrom()
  before using it; usually it is done by StartTrainingX() function).
* additional temporary arrays
  
This structure should be initialized with InitMLPTrnSession() call.
*************************************************************************/
typedef struct
{
    ae_vector bestparameters;
    double bestrmserror;
    ae_bool randomizenetwork;
    multilayerperceptron network;
    minlbfgsstate optimizer;
    minlbfgsreport optimizerrep;
    ae_vector wbuf0;
    ae_vector wbuf1;
    ae_vector allminibatches;
    ae_vector currentminibatch;
    rcommstate rstate;
    ae_int_t algoused;
    ae_int_t minibatchsize;
    hqrndstate generator;
} smlptrnsession;


/*************************************************************************
Temporary data structures used by following functions:
* TrainEnsembleX

This structure contains:
* two arrays which can be used to store training and validation subsets
* sessions for MLP training

This structure should be initialized with InitMLPETrnSession() call.
*************************************************************************/
typedef struct
{
    ae_vector trnsubset;
    ae_vector valsubset;
    ae_shared_pool mlpsessions;
    mlpreport mlprep;
    multilayerperceptron network;
} mlpetrnsession;


/*************************************************************************
Trainer object for neural network.

You should not try to access fields of this object directly -  use  ALGLIB
functions to work with this object.
*************************************************************************/
typedef struct
{
    ae_int_t nin;
    ae_int_t nout;
    ae_bool rcpar;
    ae_int_t lbfgsfactor;
    double decay;
    double wstep;
    ae_int_t maxits;
    ae_int_t datatype;
    ae_int_t npoints;
    ae_matrix densexy;
    sparsematrix sparsexy;
    smlptrnsession session;
    ae_int_t ngradbatch;
    ae_vector subset;
    ae_int_t subsetsize;
    ae_vector valsubset;
    ae_int_t valsubsetsize;
    ae_int_t algokind;
    ae_int_t minibatchsize;
} mlptrainer;


/*************************************************************************
Internal record for parallelization function MLPFoldCV.
*************************************************************************/
typedef struct
{
    multilayerperceptron network;
    mlpreport rep;
    ae_vector subset;
    ae_int_t subsetsize;
    ae_vector xyrow;
    ae_vector y;
    ae_int_t ngrad;
    ae_shared_pool trnpool;
} mlpparallelizationcv;


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);
void _pexec_mlpkfoldcv(mlptrainer* s,
    multilayerperceptron* network,
    ae_int_t nrestarts,
    ae_int_t foldscount,
    mlpreport* rep, ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
void mlpsetdecay(mlptrainer* s, double decay, ae_state *_state);


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
     ae_state *_state);


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
void mlpsetalgobatch(mlptrainer* s, ae_state *_state);


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
     ae_state *_state);
void _pexec_mlptrainnetwork(mlptrainer* s,
    multilayerperceptron* network,
    ae_int_t nrestarts,
    mlpreport* rep, ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);
ae_bool _pexec_mlpcontinuetraining(mlptrainer* s,
    multilayerperceptron* network, ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);
void _pexec_mlptrainensemblees(mlptrainer* s,
    mlpensemble* ensemble,
    ae_int_t nrestarts,
    mlpreport* rep, ae_state *_state);
void _mlpreport_init(void* _p, ae_state *_state);
void _mlpreport_init_copy(void* _dst, void* _src, ae_state *_state);
void _mlpreport_clear(void* _p);
void _mlpreport_destroy(void* _p);
void _mlpcvreport_init(void* _p, ae_state *_state);
void _mlpcvreport_init_copy(void* _dst, void* _src, ae_state *_state);
void _mlpcvreport_clear(void* _p);
void _mlpcvreport_destroy(void* _p);
void _smlptrnsession_init(void* _p, ae_state *_state);
void _smlptrnsession_init_copy(void* _dst, void* _src, ae_state *_state);
void _smlptrnsession_clear(void* _p);
void _smlptrnsession_destroy(void* _p);
void _mlpetrnsession_init(void* _p, ae_state *_state);
void _mlpetrnsession_init_copy(void* _dst, void* _src, ae_state *_state);
void _mlpetrnsession_clear(void* _p);
void _mlpetrnsession_destroy(void* _p);
void _mlptrainer_init(void* _p, ae_state *_state);
void _mlptrainer_init_copy(void* _dst, void* _src, ae_state *_state);
void _mlptrainer_clear(void* _p);
void _mlptrainer_destroy(void* _p);
void _mlpparallelizationcv_init(void* _p, ae_state *_state);
void _mlpparallelizationcv_init_copy(void* _dst, void* _src, ae_state *_state);
void _mlpparallelizationcv_clear(void* _p);
void _mlpparallelizationcv_destroy(void* _p);


/*$ End $*/
#endif

