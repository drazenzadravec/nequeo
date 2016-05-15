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

#ifndef _rbf_h
#define _rbf_h

#include "aenv.h"
#include "ialglib.h"
#include "scodes.h"
#include "apserv.h"
#include "tsort.h"
#include "nearestneighbor.h"
#include "ratint.h"
#include "polint.h"
#include "spline1d.h"
#include "reflections.h"
#include "creflections.h"
#include "hqrnd.h"
#include "matgen.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"
#include "sparse.h"
#include "rotations.h"
#include "trfac.h"
#include "trlinsolve.h"
#include "safesolve.h"
#include "rcond.h"
#include "matinv.h"
#include "hblas.h"
#include "sblas.h"
#include "ortfac.h"
#include "blas.h"
#include "bdsvd.h"
#include "svd.h"
#include "optserv.h"
#include "normestimator.h"
#include "xblas.h"
#include "densesolver.h"
#include "fbls.h"
#include "cqmodels.h"
#include "snnls.h"
#include "sactivesets.h"
#include "qqpsolver.h"
#include "linmin.h"
#include "mincg.h"
#include "minbleic.h"
#include "qpbleicsolver.h"
#include "qpcholeskysolver.h"
#include "minqp.h"
#include "minlbfgs.h"
#include "minlm.h"
#include "lsfit.h"
#include "linlsqr.h"


/*$ Declarations $*/


/*************************************************************************
RBF model.

Never try to directly work with fields of this object - always use  ALGLIB
functions to use this object.
*************************************************************************/
typedef struct
{
    ae_int_t ny;
    ae_int_t nx;
    ae_int_t nc;
    ae_int_t nl;
    kdtree tree;
    ae_matrix xc;
    ae_matrix wr;
    double rmax;
    ae_matrix v;
    ae_int_t gridtype;
    ae_bool fixrad;
    double lambdav;
    double radvalue;
    double radzvalue;
    ae_int_t nlayers;
    ae_int_t aterm;
    ae_int_t algorithmtype;
    double epsort;
    double epserr;
    ae_int_t maxits;
    double h;
    ae_int_t n;
    ae_matrix x;
    ae_matrix y;
    ae_vector calcbufxcx;
    ae_matrix calcbufx;
    ae_vector calcbuftags;
} rbfmodel;


/*************************************************************************
RBF solution report:
* TerminationType   -   termination type, positive values - success,
                        non-positive - failure.
*************************************************************************/
typedef struct
{
    ae_int_t arows;
    ae_int_t acols;
    ae_int_t annz;
    ae_int_t iterationscount;
    ae_int_t nmv;
    ae_int_t terminationtype;
} rbfreport;


/*$ Body $*/


/*************************************************************************
This function creates RBF  model  for  a  scalar (NY=1)  or  vector (NY>1)
function in a NX-dimensional space (NX=2 or NX=3).

Newly created model is empty. It can be used for interpolation right after
creation, but it just returns zeros. You have to add points to the  model,
tune interpolation settings, and then  call  model  construction  function
RBFBuildModel() which will update model according to your specification.

USAGE:
1. User creates model with RBFCreate()
2. User adds dataset with RBFSetPoints() (points do NOT have to  be  on  a
   regular grid)
3. (OPTIONAL) User chooses polynomial term by calling:
   * RBFLinTerm() to set linear term
   * RBFConstTerm() to set constant term
   * RBFZeroTerm() to set zero term
   By default, linear term is used.
4. User chooses specific RBF algorithm to use: either QNN (RBFSetAlgoQNN)
   or ML (RBFSetAlgoMultiLayer).
5. User calls RBFBuildModel() function which rebuilds model  according  to
   the specification
6. User may call RBFCalc() to calculate model value at the specified point,
   RBFGridCalc() to  calculate   model  values at the points of the regular
   grid. User may extract model coefficients with RBFUnpack() call.
   
INPUT PARAMETERS:
    NX      -   dimension of the space, NX=2 or NX=3
    NY      -   function dimension, NY>=1

OUTPUT PARAMETERS:
    S       -   RBF model (initially equals to zero)

NOTE 1: memory requirements. RBF models require amount of memory  which is
        proportional  to  the  number  of data points. Memory is allocated 
        during model construction, but most of this memory is freed  after
        model coefficients are calculated.
        
        Some approximate estimates for N centers with default settings are
        given below:
        * about 250*N*(sizeof(double)+2*sizeof(int)) bytes  of  memory  is
          needed during model construction stage.
        * about 15*N*sizeof(double) bytes is needed after model is built.
        For example, for N=100000 we may need 0.6 GB of memory  to  build
        model, but just about 0.012 GB to store it.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
void rbfcreate(ae_int_t nx, ae_int_t ny, rbfmodel* s, ae_state *_state);


/*************************************************************************
This function adds dataset.

This function overrides results of the previous calls, i.e. multiple calls
of this function will result in only the last set being added.

INPUT PARAMETERS:
    S       -   RBF model, initialized by RBFCreate() call.
    XY      -   points, array[N,NX+NY]. One row corresponds to  one  point
                in the dataset. First NX elements  are  coordinates,  next
                NY elements are function values. Array may  be larger than 
                specific,  in  this  case  only leading [N,NX+NY] elements 
                will be used.
    N       -   number of points in the dataset

After you've added dataset and (optionally) tuned algorithm  settings  you
should call RBFBuildModel() in order to build a model for you.

NOTE: this   function  has   some   serialization-related  subtleties.  We
      recommend you to study serialization examples from ALGLIB  Reference
      Manual if you want to perform serialization of your models.
      

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
void rbfsetpoints(rbfmodel* s,
     /* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
This  function  sets  RBF interpolation algorithm. ALGLIB supports several
RBF algorithms with different properties.

This algorithm is called RBF-QNN and  it  is  good  for  point  sets  with
following properties:
a) all points are distinct
b) all points are well separated.
c) points  distribution  is  approximately  uniform.  There is no "contour
   lines", clusters of points, or other small-scale structures.

Algorithm description:
1) interpolation centers are allocated to data points
2) interpolation radii are calculated as distances to the  nearest centers
   times Q coefficient (where Q is a value from [0.75,1.50]).
3) after  performing (2) radii are transformed in order to avoid situation
   when single outlier has very large radius and  influences  many  points
   across all dataset. Transformation has following form:
       new_r[i] = min(r[i],Z*median(r[]))
   where r[i] is I-th radius, median()  is a median  radius across  entire
   dataset, Z is user-specified value which controls amount  of  deviation
   from median radius.

When (a) is violated,  we  will  be unable to build RBF model. When (b) or
(c) are violated, model will be built, but interpolation quality  will  be
low. See http://www.alglib.net/interpolation/ for more information on this
subject.

This algorithm is used by default.

Additional Q parameter controls smoothness properties of the RBF basis:
* Q<0.75 will give perfectly conditioned basis,  but  terrible  smoothness
  properties (RBF interpolant will have sharp peaks around function values)
* Q around 1.0 gives good balance between smoothness and condition number
* Q>1.5 will lead to badly conditioned systems and slow convergence of the
  underlying linear solver (although smoothness will be very good)
* Q>2.0 will effectively make optimizer useless because it won't  converge
  within reasonable amount of iterations. It is possible to set such large
  Q, but it is advised not to do so.

INPUT PARAMETERS:
    S       -   RBF model, initialized by RBFCreate() call
    Q       -   Q parameter, Q>0, recommended value - 1.0
    Z       -   Z parameter, Z>0, recommended value - 5.0

NOTE: this   function  has   some   serialization-related  subtleties.  We
      recommend you to study serialization examples from ALGLIB  Reference
      Manual if you want to perform serialization of your models.


  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
void rbfsetalgoqnn(rbfmodel* s, double q, double z, ae_state *_state);


/*************************************************************************
This  function  sets  RBF interpolation algorithm. ALGLIB supports several
RBF algorithms with different properties.

This  algorithm is called RBF-ML. It builds  multilayer  RBF  model,  i.e.
model with subsequently decreasing  radii,  which  allows  us  to  combine
smoothness (due to  large radii of  the first layers) with  exactness (due
to small radii of the last layers) and fast convergence.

Internally RBF-ML uses many different  means  of acceleration, from sparse
matrices  to  KD-trees,  which  results in algorithm whose working time is
roughly proportional to N*log(N)*Density*RBase^2*NLayers,  where  N  is  a
number of points, Density is an average density if points per unit of  the
interpolation space, RBase is an initial radius, NLayers is  a  number  of
layers.

RBF-ML is good for following kinds of interpolation problems:
1. "exact" problems (perfect fit) with well separated points
2. least squares problems with arbitrary distribution of points (algorithm
   gives  perfect  fit  where it is possible, and resorts to least squares
   fit in the hard areas).
3. noisy problems where  we  want  to  apply  some  controlled  amount  of
   smoothing.

INPUT PARAMETERS:
    S       -   RBF model, initialized by RBFCreate() call
    RBase   -   RBase parameter, RBase>0
    NLayers -   NLayers parameter, NLayers>0, recommended value  to  start
                with - about 5.
    LambdaV -   regularization value, can be useful when  solving  problem
                in the least squares sense.  Optimal  lambda  is  problem-
                dependent and require trial and error. In our  experience,
                good lambda can be as large as 0.1, and you can use  0.001
                as initial guess.
                Default  value  - 0.01, which is used when LambdaV is  not
                given.  You  can  specify  zero  value,  but  it  is   not
                recommended to do so.

TUNING ALGORITHM

In order to use this algorithm you have to choose three parameters:
* initial radius RBase
* number of layers in the model NLayers
* regularization coefficient LambdaV

Initial radius is easy to choose - you can pick any number  several  times
larger  than  the  average  distance between points. Algorithm won't break
down if you choose radius which is too large (model construction time will
increase, but model will be built correctly).

Choose such number of layers that RLast=RBase/2^(NLayers-1)  (radius  used
by  the  last  layer)  will  be  smaller than the typical distance between
points.  In  case  model  error  is  too large, you can increase number of
layers.  Having  more  layers  will make model construction and evaluation
proportionally slower, but it will allow you to have model which precisely
fits your data. From the other side, if you want to  suppress  noise,  you
can DECREASE number of layers to make your model less flexible.

Regularization coefficient LambdaV controls smoothness of  the  individual
models built for each layer. We recommend you to use default value in case
you don't want to tune this parameter,  because  having  non-zero  LambdaV
accelerates and stabilizes internal iterative algorithm. In case you  want
to suppress noise you can use  LambdaV  as  additional  parameter  (larger
value = more smoothness) to tune.

TYPICAL ERRORS

1. Using  initial  radius  which is too large. Memory requirements  of the
   RBF-ML are roughly proportional to N*Density*RBase^2 (where Density  is
   an average density of points per unit of the interpolation  space).  In
   the extreme case of the very large RBase we will need O(N^2)  units  of
   memory - and many layers in order to decrease radius to some reasonably
   small value.

2. Using too small number of layers - RBF models with large radius are not
   flexible enough to reproduce small variations in the  target  function.
   You  need  many  layers  with  different radii, from large to small, in
   order to have good model.

3. Using  initial  radius  which  is  too  small.  You will get model with
   "holes" in the areas which are too far away from interpolation centers.
   However, algorithm will work correctly (and quickly) in this case.

4. Using too many layers - you will get too large and too slow model. This
   model  will  perfectly  reproduce  your function, but maybe you will be
   able to achieve similar results with less layers (and less memory).
   
  -- ALGLIB --
     Copyright 02.03.2012 by Bochkanov Sergey
*************************************************************************/
void rbfsetalgomultilayer(rbfmodel* s,
     double rbase,
     ae_int_t nlayers,
     double lambdav,
     ae_state *_state);


/*************************************************************************
This function sets linear term (model is a sum of radial  basis  functions
plus linear polynomial). This function won't have effect until  next  call 
to RBFBuildModel().

INPUT PARAMETERS:
    S       -   RBF model, initialized by RBFCreate() call

NOTE: this   function  has   some   serialization-related  subtleties.  We
      recommend you to study serialization examples from ALGLIB  Reference
      Manual if you want to perform serialization of your models.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
void rbfsetlinterm(rbfmodel* s, ae_state *_state);


/*************************************************************************
This function sets constant term (model is a sum of radial basis functions
plus constant).  This  function  won't  have  effect  until  next  call to 
RBFBuildModel().

INPUT PARAMETERS:
    S       -   RBF model, initialized by RBFCreate() call

NOTE: this   function  has   some   serialization-related  subtleties.  We
      recommend you to study serialization examples from ALGLIB  Reference
      Manual if you want to perform serialization of your models.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
void rbfsetconstterm(rbfmodel* s, ae_state *_state);


/*************************************************************************
This  function  sets  zero  term (model is a sum of radial basis functions 
without polynomial term). This function won't have effect until next  call
to RBFBuildModel().

INPUT PARAMETERS:
    S       -   RBF model, initialized by RBFCreate() call

NOTE: this   function  has   some   serialization-related  subtleties.  We
      recommend you to study serialization examples from ALGLIB  Reference
      Manual if you want to perform serialization of your models.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
void rbfsetzeroterm(rbfmodel* s, ae_state *_state);


/*************************************************************************
This function sets stopping criteria of the underlying linear solver.

INPUT PARAMETERS:
    S       -   RBF model, initialized by RBFCreate() call
    EpsOrt  -   orthogonality stopping criterion, EpsOrt>=0. Algorithm will
                stop when ||A'*r||<=EpsOrt where A' is a transpose of  the 
                system matrix, r is a residual vector.
                Recommended value of EpsOrt is equal to 1E-6.
                This criterion will stop algorithm when we have "bad fit"
                situation, i.e. when we should stop in a point with large,
                nonzero residual.
    EpsErr  -   residual stopping  criterion.  Algorithm  will  stop  when
                ||r||<=EpsErr*||b||, where r is a residual vector, b is  a
                right part of the system (function values).
                Recommended value of EpsErr is equal to 1E-3 or 1E-6.
                This  criterion  will  stop  algorithm  in  a  "good  fit" 
                situation when we have near-zero residual near the desired
                solution.
    MaxIts  -   this criterion will stop algorithm after MaxIts iterations.
                It should be used for debugging purposes only!
                Zero MaxIts means that no limit is placed on the number of
                iterations.

We  recommend  to  set  moderate  non-zero  values   EpsOrt   and   EpsErr 
simultaneously. Values equal to 10E-6 are good to start with. In case  you
need high performance and do not need high precision ,  you  may  decrease
EpsErr down to 0.001. However, we do not recommend decreasing EpsOrt.

As for MaxIts, we recommend to leave it zero unless you know what you do.

NOTE: this   function  has   some   serialization-related  subtleties.  We
      recommend you to study serialization examples from ALGLIB  Reference
      Manual if you want to perform serialization of your models.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
void rbfsetcond(rbfmodel* s,
     double epsort,
     double epserr,
     ae_int_t maxits,
     ae_state *_state);


/*************************************************************************
This   function  builds  RBF  model  and  returns  report  (contains  some 
information which can be used for evaluation of the algorithm properties).

Call to this function modifies RBF model by calculating its centers/radii/
weights  and  saving  them  into  RBFModel  structure.  Initially RBFModel 
contain zero coefficients, but after call to this function  we  will  have
coefficients which were calculated in order to fit our dataset.

After you called this function you can call RBFCalc(),  RBFGridCalc()  and
other model calculation functions.

INPUT PARAMETERS:
    S       -   RBF model, initialized by RBFCreate() call
    Rep     -   report:
                * Rep.TerminationType:
                  * -5 - non-distinct basis function centers were detected,
                         interpolation aborted
                  * -4 - nonconvergence of the internal SVD solver
                  *  1 - successful termination
                Fields are used for debugging purposes:
                * Rep.IterationsCount - iterations count of the LSQR solver
                * Rep.NMV - number of matrix-vector products
                * Rep.ARows - rows count for the system matrix
                * Rep.ACols - columns count for the system matrix
                * Rep.ANNZ - number of significantly non-zero elements
                  (elements above some algorithm-determined threshold)

NOTE:  failure  to  build  model will leave current state of the structure
unchanged.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
void rbfbuildmodel(rbfmodel* s, rbfreport* rep, ae_state *_state);


/*************************************************************************
This function calculates values of the RBF model in the given point.

This function should be used when we have NY=1 (scalar function) and  NX=2
(2-dimensional space). If you have 3-dimensional space, use RBFCalc3(). If
you have general situation (NX-dimensional space, NY-dimensional function)
you should use general, less efficient implementation RBFCalc().

If  you  want  to  calculate  function  values  many times, consider using 
RBFGridCalc2(), which is far more efficient than many subsequent calls  to
RBFCalc2().

This function returns 0.0 when:
* model is not initialized
* NX<>2
 *NY<>1

INPUT PARAMETERS:
    S       -   RBF model
    X0      -   first coordinate, finite number
    X1      -   second coordinate, finite number

RESULT:
    value of the model or 0.0 (as defined above)

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
double rbfcalc2(rbfmodel* s, double x0, double x1, ae_state *_state);


/*************************************************************************
This function calculates values of the RBF model in the given point.

This function should be used when we have NY=1 (scalar function) and  NX=3
(3-dimensional space). If you have 2-dimensional space, use RBFCalc2(). If
you have general situation (NX-dimensional space, NY-dimensional function)
you should use general, less efficient implementation RBFCalc().

This function returns 0.0 when:
* model is not initialized
* NX<>3
 *NY<>1

INPUT PARAMETERS:
    S       -   RBF model
    X0      -   first coordinate, finite number
    X1      -   second coordinate, finite number
    X2      -   third coordinate, finite number

RESULT:
    value of the model or 0.0 (as defined above)

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
double rbfcalc3(rbfmodel* s,
     double x0,
     double x1,
     double x2,
     ae_state *_state);


/*************************************************************************
This function calculates values of the RBF model at the given point.

This is general function which can be used for arbitrary NX (dimension  of 
the space of arguments) and NY (dimension of the function itself). However
when  you  have  NY=1  you  may  find more convenient to use RBFCalc2() or 
RBFCalc3().

This function returns 0.0 when model is not initialized.

INPUT PARAMETERS:
    S       -   RBF model
    X       -   coordinates, array[NX].
                X may have more than NX elements, in this case only 
                leading NX will be used.

OUTPUT PARAMETERS:
    Y       -   function value, array[NY]. Y is out-parameter and 
                reallocated after call to this function. In case you  want
                to reuse previously allocated Y, you may use RBFCalcBuf(),
                which reallocates Y only when it is too small.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
void rbfcalc(rbfmodel* s,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state);


/*************************************************************************
This function calculates values of the RBF model at the given point.

Same as RBFCalc(), but does not reallocate Y when in is large enough to 
store function values.

INPUT PARAMETERS:
    S       -   RBF model
    X       -   coordinates, array[NX].
                X may have more than NX elements, in this case only 
                leading NX will be used.
    Y       -   possibly preallocated array

OUTPUT PARAMETERS:
    Y       -   function value, array[NY]. Y is not reallocated when it
                is larger than NY.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
void rbfcalcbuf(rbfmodel* s,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state);


/*************************************************************************
This function calculates values of the RBF model at the regular grid.

Grid have N0*N1 points, with Point[I,J] = (X0[I], X1[J])

This function returns 0.0 when:
* model is not initialized
* NX<>2
 *NY<>1

INPUT PARAMETERS:
    S       -   RBF model
    X0      -   array of grid nodes, first coordinates, array[N0]
    N0      -   grid size (number of nodes) in the first dimension
    X1      -   array of grid nodes, second coordinates, array[N1]
    N1      -   grid size (number of nodes) in the second dimension

OUTPUT PARAMETERS:
    Y       -   function values, array[N0,N1]. Y is out-variable and 
                is reallocated by this function.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
void rbfgridcalc2(rbfmodel* s,
     /* Real    */ ae_vector* x0,
     ae_int_t n0,
     /* Real    */ ae_vector* x1,
     ae_int_t n1,
     /* Real    */ ae_matrix* y,
     ae_state *_state);


/*************************************************************************
This function "unpacks" RBF model by extracting its coefficients.

INPUT PARAMETERS:
    S       -   RBF model

OUTPUT PARAMETERS:
    NX      -   dimensionality of argument
    NY      -   dimensionality of the target function
    XWR     -   model information, array[NC,NX+NY+1].
                One row of the array corresponds to one basis function:
                * first NX columns  - coordinates of the center 
                * next NY columns   - weights, one per dimension of the 
                                      function being modelled
                * last column       - radius, same for all dimensions of
                                      the function being modelled
    NC      -   number of the centers
    V       -   polynomial  term , array[NY,NX+1]. One row per one 
                dimension of the function being modelled. First NX 
                elements are linear coefficients, V[NX] is equal to the 
                constant part.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
void rbfunpack(rbfmodel* s,
     ae_int_t* nx,
     ae_int_t* ny,
     /* Real    */ ae_matrix* xwr,
     ae_int_t* nc,
     /* Real    */ ae_matrix* v,
     ae_state *_state);


/*************************************************************************
Serializer: allocation

  -- ALGLIB --
     Copyright 02.02.2012 by Bochkanov Sergey
*************************************************************************/
void rbfalloc(ae_serializer* s, rbfmodel* model, ae_state *_state);


/*************************************************************************
Serializer: serialization

  -- ALGLIB --
     Copyright 02.02.2012 by Bochkanov Sergey
*************************************************************************/
void rbfserialize(ae_serializer* s, rbfmodel* model, ae_state *_state);


/*************************************************************************
Serializer: unserialization

  -- ALGLIB --
     Copyright 02.02.2012 by Bochkanov Sergey
*************************************************************************/
void rbfunserialize(ae_serializer* s, rbfmodel* model, ae_state *_state);
void _rbfmodel_init(void* _p, ae_state *_state);
void _rbfmodel_init_copy(void* _dst, void* _src, ae_state *_state);
void _rbfmodel_clear(void* _p);
void _rbfmodel_destroy(void* _p);
void _rbfreport_init(void* _p, ae_state *_state);
void _rbfreport_init_copy(void* _dst, void* _src, ae_state *_state);
void _rbfreport_clear(void* _p);
void _rbfreport_destroy(void* _p);


/*$ End $*/
#endif

