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
#include "rbf.h"


/*$ Declarations $*/
static double rbf_eps = 1.0E-6;
static ae_int_t rbf_mxnx = 3;
static double rbf_rbffarradius = 6;
static double rbf_rbfnearradius = 2.1;
static double rbf_rbfmlradius = 3;
static ae_int_t rbf_rbffirstversion = 0;
static void rbf_rbfgridpoints(rbfmodel* s, ae_state *_state);
static void rbf_rbfradnn(rbfmodel* s,
     double q,
     double z,
     ae_state *_state);
static ae_bool rbf_buildlinearmodel(/* Real    */ ae_matrix* x,
     /* Real    */ ae_matrix* y,
     ae_int_t n,
     ae_int_t ny,
     ae_int_t modeltype,
     /* Real    */ ae_matrix* v,
     ae_state *_state);
static void rbf_buildrbfmodellsqr(/* Real    */ ae_matrix* x,
     /* Real    */ ae_matrix* y,
     /* Real    */ ae_matrix* xc,
     /* Real    */ ae_vector* r,
     ae_int_t n,
     ae_int_t nc,
     ae_int_t ny,
     kdtree* pointstree,
     kdtree* centerstree,
     double epsort,
     double epserr,
     ae_int_t maxits,
     ae_int_t* gnnz,
     ae_int_t* snnz,
     /* Real    */ ae_matrix* w,
     ae_int_t* info,
     ae_int_t* iterationscount,
     ae_int_t* nmv,
     ae_state *_state);
static void rbf_buildrbfmlayersmodellsqr(/* Real    */ ae_matrix* x,
     /* Real    */ ae_matrix* y,
     /* Real    */ ae_matrix* xc,
     double rval,
     /* Real    */ ae_vector* r,
     ae_int_t n,
     ae_int_t* nc,
     ae_int_t ny,
     ae_int_t nlayers,
     kdtree* centerstree,
     double epsort,
     double epserr,
     ae_int_t maxits,
     double lambdav,
     ae_int_t* annz,
     /* Real    */ ae_matrix* w,
     ae_int_t* info,
     ae_int_t* iterationscount,
     ae_int_t* nmv,
     ae_state *_state);


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
void rbfcreate(ae_int_t nx, ae_int_t ny, rbfmodel* s, ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    _rbfmodel_clear(s);

    ae_assert(nx==2||nx==3, "RBFCreate: NX<>2 and NX<>3", _state);
    ae_assert(ny>=1, "RBFCreate: NY<1", _state);
    s->nx = nx;
    s->ny = ny;
    s->nl = 0;
    s->nc = 0;
    ae_matrix_set_length(&s->v, ny, rbf_mxnx+1, _state);
    for(i=0; i<=ny-1; i++)
    {
        for(j=0; j<=rbf_mxnx; j++)
        {
            s->v.ptr.pp_double[i][j] = (double)(0);
        }
    }
    s->n = 0;
    s->rmax = (double)(0);
    s->gridtype = 2;
    s->fixrad = ae_false;
    s->radvalue = (double)(1);
    s->radzvalue = (double)(5);
    s->aterm = 1;
    s->algorithmtype = 1;
    
    /*
     * stopping criteria
     */
    s->epsort = rbf_eps;
    s->epserr = rbf_eps;
    s->maxits = 0;
}


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
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    ae_assert(n>0, "RBFSetPoints: N<0", _state);
    ae_assert(xy->rows>=n, "RBFSetPoints: Rows(XY)<N", _state);
    ae_assert(xy->cols>=s->nx+s->ny, "RBFSetPoints: Cols(XY)<NX+NY", _state);
    s->n = n;
    ae_matrix_set_length(&s->x, s->n, rbf_mxnx, _state);
    ae_matrix_set_length(&s->y, s->n, s->ny, _state);
    for(i=0; i<=s->n-1; i++)
    {
        for(j=0; j<=rbf_mxnx-1; j++)
        {
            s->x.ptr.pp_double[i][j] = (double)(0);
        }
        for(j=0; j<=s->nx-1; j++)
        {
            s->x.ptr.pp_double[i][j] = xy->ptr.pp_double[i][j];
        }
        for(j=0; j<=s->ny-1; j++)
        {
            s->y.ptr.pp_double[i][j] = xy->ptr.pp_double[i][j+s->nx];
        }
    }
}


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
void rbfsetalgoqnn(rbfmodel* s, double q, double z, ae_state *_state)
{


    ae_assert(ae_isfinite(q, _state), "RBFSetAlgoQNN: Q is infinite or NAN", _state);
    ae_assert(ae_fp_greater(q,(double)(0)), "RBFSetAlgoQNN: Q<=0", _state);
    rbf_rbfgridpoints(s, _state);
    rbf_rbfradnn(s, q, z, _state);
    s->algorithmtype = 1;
}


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
     ae_state *_state)
{


    ae_assert(ae_isfinite(rbase, _state), "RBFSetAlgoMultiLayer: RBase is infinite or NaN", _state);
    ae_assert(ae_fp_greater(rbase,(double)(0)), "RBFSetAlgoMultiLayer: RBase<=0", _state);
    ae_assert(nlayers>=0, "RBFSetAlgoMultiLayer: NLayers<0", _state);
    ae_assert(ae_isfinite(lambdav, _state), "RBFSetAlgoMultiLayer: LambdaV is infinite or NAN", _state);
    ae_assert(ae_fp_greater_eq(lambdav,(double)(0)), "RBFSetAlgoMultiLayer: LambdaV<0", _state);
    s->radvalue = rbase;
    s->nlayers = nlayers;
    s->algorithmtype = 2;
    s->lambdav = lambdav;
}


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
void rbfsetlinterm(rbfmodel* s, ae_state *_state)
{


    s->aterm = 1;
}


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
void rbfsetconstterm(rbfmodel* s, ae_state *_state)
{


    s->aterm = 2;
}


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
void rbfsetzeroterm(rbfmodel* s, ae_state *_state)
{


    s->aterm = 3;
}


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
     ae_state *_state)
{


    ae_assert(ae_isfinite(epsort, _state)&&ae_fp_greater_eq(epsort,(double)(0)), "RBFSetCond: EpsOrt is negative, INF or NAN", _state);
    ae_assert(ae_isfinite(epserr, _state)&&ae_fp_greater_eq(epserr,(double)(0)), "RBFSetCond: EpsB is negative, INF or NAN", _state);
    ae_assert(maxits>=0, "RBFSetCond: MaxIts is negative", _state);
    if( (ae_fp_eq(epsort,(double)(0))&&ae_fp_eq(epserr,(double)(0)))&&maxits==0 )
    {
        s->epsort = rbf_eps;
        s->epserr = rbf_eps;
        s->maxits = 0;
    }
    else
    {
        s->epsort = epsort;
        s->epserr = epserr;
        s->maxits = maxits;
    }
}


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
void rbfbuildmodel(rbfmodel* s, rbfreport* rep, ae_state *_state)
{
    ae_frame _frame_block;
    kdtree tree;
    kdtree ctree;
    ae_vector dist;
    ae_vector xcx;
    ae_matrix a;
    ae_matrix v;
    ae_matrix omega;
    ae_vector y;
    ae_matrix residualy;
    ae_vector radius;
    ae_matrix xc;
    ae_vector mnx;
    ae_vector mxx;
    ae_vector edge;
    ae_vector mxsteps;
    ae_int_t nc;
    double rmax;
    ae_vector tags;
    ae_vector ctags;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t k2;
    ae_int_t snnz;
    ae_vector tmp0;
    ae_vector tmp1;
    ae_int_t layerscnt;

    ae_frame_make(_state, &_frame_block);
    _rbfreport_clear(rep);
    _kdtree_init(&tree, _state);
    _kdtree_init(&ctree, _state);
    ae_vector_init(&dist, 0, DT_REAL, _state);
    ae_vector_init(&xcx, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&v, 0, 0, DT_REAL, _state);
    ae_matrix_init(&omega, 0, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    ae_matrix_init(&residualy, 0, 0, DT_REAL, _state);
    ae_vector_init(&radius, 0, DT_REAL, _state);
    ae_matrix_init(&xc, 0, 0, DT_REAL, _state);
    ae_vector_init(&mnx, 0, DT_REAL, _state);
    ae_vector_init(&mxx, 0, DT_REAL, _state);
    ae_vector_init(&edge, 0, DT_REAL, _state);
    ae_vector_init(&mxsteps, 0, DT_INT, _state);
    ae_vector_init(&tags, 0, DT_INT, _state);
    ae_vector_init(&ctags, 0, DT_INT, _state);
    ae_vector_init(&tmp0, 0, DT_REAL, _state);
    ae_vector_init(&tmp1, 0, DT_REAL, _state);

    ae_assert(s->nx==2||s->nx==3, "RBFBuildModel: S.NX<>2 or S.NX<>3!", _state);
    
    /*
     * Quick exit when we have no points
     */
    if( s->n==0 )
    {
        rep->terminationtype = 1;
        rep->iterationscount = 0;
        rep->nmv = 0;
        rep->arows = 0;
        rep->acols = 0;
        kdtreebuildtagged(&s->xc, &tags, 0, rbf_mxnx, 0, 2, &s->tree, _state);
        ae_matrix_set_length(&s->xc, 0, 0, _state);
        ae_matrix_set_length(&s->wr, 0, 0, _state);
        s->nc = 0;
        s->rmax = (double)(0);
        ae_matrix_set_length(&s->v, s->ny, rbf_mxnx+1, _state);
        for(i=0; i<=s->ny-1; i++)
        {
            for(j=0; j<=rbf_mxnx; j++)
            {
                s->v.ptr.pp_double[i][j] = (double)(0);
            }
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * General case, N>0
     */
    rep->annz = 0;
    rep->iterationscount = 0;
    rep->nmv = 0;
    ae_vector_set_length(&xcx, rbf_mxnx, _state);
    
    /*
     * First model in a sequence - linear model.
     * Residuals from linear regression are stored in the ResidualY variable
     * (used later to build RBF models).
     */
    ae_matrix_set_length(&residualy, s->n, s->ny, _state);
    for(i=0; i<=s->n-1; i++)
    {
        for(j=0; j<=s->ny-1; j++)
        {
            residualy.ptr.pp_double[i][j] = s->y.ptr.pp_double[i][j];
        }
    }
    if( !rbf_buildlinearmodel(&s->x, &residualy, s->n, s->ny, s->aterm, &v, _state) )
    {
        rep->terminationtype = -5;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Handle special case: multilayer model with NLayers=0.
     * Quick exit.
     */
    if( s->algorithmtype==2&&s->nlayers==0 )
    {
        rep->terminationtype = 1;
        rep->iterationscount = 0;
        rep->nmv = 0;
        rep->arows = 0;
        rep->acols = 0;
        kdtreebuildtagged(&s->xc, &tags, 0, rbf_mxnx, 0, 2, &s->tree, _state);
        ae_matrix_set_length(&s->xc, 0, 0, _state);
        ae_matrix_set_length(&s->wr, 0, 0, _state);
        s->nc = 0;
        s->rmax = (double)(0);
        ae_matrix_set_length(&s->v, s->ny, rbf_mxnx+1, _state);
        for(i=0; i<=s->ny-1; i++)
        {
            for(j=0; j<=rbf_mxnx; j++)
            {
                s->v.ptr.pp_double[i][j] = v.ptr.pp_double[i][j];
            }
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Second model in a sequence - RBF term.
     *
     * NOTE: assignments below are not necessary, but without them
     *       MSVC complains about unitialized variables.
     */
    nc = 0;
    rmax = (double)(0);
    layerscnt = 0;
    if( s->algorithmtype==1 )
    {
        
        /*
         * Add RBF model.
         * This model uses local KD-trees to speed-up nearest neighbor searches.
         */
        if( s->gridtype==1 )
        {
            ae_vector_set_length(&mxx, s->nx, _state);
            ae_vector_set_length(&mnx, s->nx, _state);
            ae_vector_set_length(&mxsteps, s->nx, _state);
            ae_vector_set_length(&edge, s->nx, _state);
            for(i=0; i<=s->nx-1; i++)
            {
                mxx.ptr.p_double[i] = s->x.ptr.pp_double[0][i];
                mnx.ptr.p_double[i] = s->x.ptr.pp_double[0][i];
            }
            for(i=0; i<=s->n-1; i++)
            {
                for(j=0; j<=s->nx-1; j++)
                {
                    if( ae_fp_less(mxx.ptr.p_double[j],s->x.ptr.pp_double[i][j]) )
                    {
                        mxx.ptr.p_double[j] = s->x.ptr.pp_double[i][j];
                    }
                    if( ae_fp_greater(mnx.ptr.p_double[j],s->x.ptr.pp_double[i][j]) )
                    {
                        mnx.ptr.p_double[j] = s->x.ptr.pp_double[i][j];
                    }
                }
            }
            for(i=0; i<=s->nx-1; i++)
            {
                mxsteps.ptr.p_int[i] = ae_trunc((mxx.ptr.p_double[i]-mnx.ptr.p_double[i])/(2*s->h), _state)+1;
                edge.ptr.p_double[i] = (mxx.ptr.p_double[i]+mnx.ptr.p_double[i])/2-s->h*mxsteps.ptr.p_int[i];
            }
            nc = 1;
            for(i=0; i<=s->nx-1; i++)
            {
                mxsteps.ptr.p_int[i] = 2*mxsteps.ptr.p_int[i]+1;
                nc = nc*mxsteps.ptr.p_int[i];
            }
            ae_matrix_set_length(&xc, nc, rbf_mxnx, _state);
            if( s->nx==2 )
            {
                for(i=0; i<=mxsteps.ptr.p_int[0]-1; i++)
                {
                    for(j=0; j<=mxsteps.ptr.p_int[1]-1; j++)
                    {
                        for(k2=0; k2<=rbf_mxnx-1; k2++)
                        {
                            xc.ptr.pp_double[i*mxsteps.ptr.p_int[1]+j][k2] = (double)(0);
                        }
                        xc.ptr.pp_double[i*mxsteps.ptr.p_int[1]+j][0] = edge.ptr.p_double[0]+s->h*i;
                        xc.ptr.pp_double[i*mxsteps.ptr.p_int[1]+j][1] = edge.ptr.p_double[1]+s->h*j;
                    }
                }
            }
            if( s->nx==3 )
            {
                for(i=0; i<=mxsteps.ptr.p_int[0]-1; i++)
                {
                    for(j=0; j<=mxsteps.ptr.p_int[1]-1; j++)
                    {
                        for(k=0; k<=mxsteps.ptr.p_int[2]-1; k++)
                        {
                            for(k2=0; k2<=rbf_mxnx-1; k2++)
                            {
                                xc.ptr.pp_double[i*mxsteps.ptr.p_int[1]+j][k2] = (double)(0);
                            }
                            xc.ptr.pp_double[(i*mxsteps.ptr.p_int[1]+j)*mxsteps.ptr.p_int[2]+k][0] = edge.ptr.p_double[0]+s->h*i;
                            xc.ptr.pp_double[(i*mxsteps.ptr.p_int[1]+j)*mxsteps.ptr.p_int[2]+k][1] = edge.ptr.p_double[1]+s->h*j;
                            xc.ptr.pp_double[(i*mxsteps.ptr.p_int[1]+j)*mxsteps.ptr.p_int[2]+k][2] = edge.ptr.p_double[2]+s->h*k;
                        }
                    }
                }
            }
        }
        else
        {
            if( s->gridtype==2 )
            {
                nc = s->n;
                ae_matrix_set_length(&xc, nc, rbf_mxnx, _state);
                for(i=0; i<=nc-1; i++)
                {
                    for(j=0; j<=rbf_mxnx-1; j++)
                    {
                        xc.ptr.pp_double[i][j] = s->x.ptr.pp_double[i][j];
                    }
                }
            }
            else
            {
                if( s->gridtype==3 )
                {
                    nc = s->nc;
                    ae_matrix_set_length(&xc, nc, rbf_mxnx, _state);
                    for(i=0; i<=nc-1; i++)
                    {
                        for(j=0; j<=rbf_mxnx-1; j++)
                        {
                            xc.ptr.pp_double[i][j] = s->xc.ptr.pp_double[i][j];
                        }
                    }
                }
                else
                {
                    ae_assert(ae_false, "RBFBuildModel: either S.GridType<1 or S.GridType>3!", _state);
                }
            }
        }
        rmax = (double)(0);
        ae_vector_set_length(&radius, nc, _state);
        ae_vector_set_length(&ctags, nc, _state);
        for(i=0; i<=nc-1; i++)
        {
            ctags.ptr.p_int[i] = i;
        }
        kdtreebuildtagged(&xc, &ctags, nc, rbf_mxnx, 0, 2, &ctree, _state);
        if( s->fixrad )
        {
            
            /*
             * Fixed radius
             */
            for(i=0; i<=nc-1; i++)
            {
                radius.ptr.p_double[i] = s->radvalue;
            }
            rmax = radius.ptr.p_double[0];
        }
        else
        {
            
            /*
             * Dynamic radius
             */
            if( nc==0 )
            {
                rmax = (double)(1);
            }
            else
            {
                if( nc==1 )
                {
                    radius.ptr.p_double[0] = s->radvalue;
                    rmax = radius.ptr.p_double[0];
                }
                else
                {
                    
                    /*
                     * NC>1, calculate radii using distances to nearest neigbors
                     */
                    for(i=0; i<=nc-1; i++)
                    {
                        for(j=0; j<=rbf_mxnx-1; j++)
                        {
                            xcx.ptr.p_double[j] = xc.ptr.pp_double[i][j];
                        }
                        if( kdtreequeryknn(&ctree, &xcx, 1, ae_false, _state)>0 )
                        {
                            kdtreequeryresultsdistances(&ctree, &dist, _state);
                            radius.ptr.p_double[i] = s->radvalue*dist.ptr.p_double[0];
                        }
                        else
                        {
                            
                            /*
                             * No neighbors found (it will happen when we have only one center).
                             * Initialize radius with default value.
                             */
                            radius.ptr.p_double[i] = 1.0;
                        }
                    }
                    
                    /*
                     * Apply filtering
                     */
                    rvectorsetlengthatleast(&tmp0, nc, _state);
                    for(i=0; i<=nc-1; i++)
                    {
                        tmp0.ptr.p_double[i] = radius.ptr.p_double[i];
                    }
                    tagsortfast(&tmp0, &tmp1, nc, _state);
                    for(i=0; i<=nc-1; i++)
                    {
                        radius.ptr.p_double[i] = ae_minreal(radius.ptr.p_double[i], s->radzvalue*tmp0.ptr.p_double[nc/2], _state);
                    }
                    
                    /*
                     * Calculate RMax, check that all radii are non-zero
                     */
                    for(i=0; i<=nc-1; i++)
                    {
                        rmax = ae_maxreal(rmax, radius.ptr.p_double[i], _state);
                    }
                    for(i=0; i<=nc-1; i++)
                    {
                        if( ae_fp_eq(radius.ptr.p_double[i],(double)(0)) )
                        {
                            rep->terminationtype = -5;
                            ae_frame_leave(_state);
                            return;
                        }
                    }
                }
            }
        }
        ivectorsetlengthatleast(&tags, s->n, _state);
        for(i=0; i<=s->n-1; i++)
        {
            tags.ptr.p_int[i] = i;
        }
        kdtreebuildtagged(&s->x, &tags, s->n, rbf_mxnx, 0, 2, &tree, _state);
        rbf_buildrbfmodellsqr(&s->x, &residualy, &xc, &radius, s->n, nc, s->ny, &tree, &ctree, s->epsort, s->epserr, s->maxits, &rep->annz, &snnz, &omega, &rep->terminationtype, &rep->iterationscount, &rep->nmv, _state);
        layerscnt = 1;
    }
    else
    {
        if( s->algorithmtype==2 )
        {
            rmax = s->radvalue;
            rbf_buildrbfmlayersmodellsqr(&s->x, &residualy, &xc, s->radvalue, &radius, s->n, &nc, s->ny, s->nlayers, &ctree, 1.0E-6, 1.0E-6, 50, s->lambdav, &rep->annz, &omega, &rep->terminationtype, &rep->iterationscount, &rep->nmv, _state);
            layerscnt = s->nlayers;
        }
        else
        {
            ae_assert(ae_false, "RBFBuildModel: internal error(AlgorithmType neither 1 nor 2)", _state);
        }
    }
    if( rep->terminationtype<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Model is built
     */
    s->nc = nc/layerscnt;
    s->rmax = rmax;
    s->nl = layerscnt;
    ae_matrix_set_length(&s->xc, s->nc, rbf_mxnx, _state);
    ae_matrix_set_length(&s->wr, s->nc, 1+s->nl*s->ny, _state);
    ae_matrix_set_length(&s->v, s->ny, rbf_mxnx+1, _state);
    for(i=0; i<=s->nc-1; i++)
    {
        for(j=0; j<=rbf_mxnx-1; j++)
        {
            s->xc.ptr.pp_double[i][j] = xc.ptr.pp_double[i][j];
        }
    }
    ivectorsetlengthatleast(&tags, s->nc, _state);
    for(i=0; i<=s->nc-1; i++)
    {
        tags.ptr.p_int[i] = i;
    }
    kdtreebuildtagged(&s->xc, &tags, s->nc, rbf_mxnx, 0, 2, &s->tree, _state);
    for(i=0; i<=s->nc-1; i++)
    {
        s->wr.ptr.pp_double[i][0] = radius.ptr.p_double[i];
        for(k=0; k<=layerscnt-1; k++)
        {
            for(j=0; j<=s->ny-1; j++)
            {
                s->wr.ptr.pp_double[i][1+k*s->ny+j] = omega.ptr.pp_double[k*s->nc+i][j];
            }
        }
    }
    for(i=0; i<=s->ny-1; i++)
    {
        for(j=0; j<=rbf_mxnx; j++)
        {
            s->v.ptr.pp_double[i][j] = v.ptr.pp_double[i][j];
        }
    }
    rep->terminationtype = 1;
    rep->arows = s->n;
    rep->acols = s->nc;
    ae_frame_leave(_state);
}


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
double rbfcalc2(rbfmodel* s, double x0, double x1, ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t lx;
    ae_int_t tg;
    double d2;
    double t;
    double bfcur;
    double rcur;
    double result;


    ae_assert(ae_isfinite(x0, _state), "RBFCalc2: invalid value for X0 (X0 is Inf)!", _state);
    ae_assert(ae_isfinite(x1, _state), "RBFCalc2: invalid value for X1 (X1 is Inf)!", _state);
    if( s->ny!=1||s->nx!=2 )
    {
        result = (double)(0);
        return result;
    }
    result = s->v.ptr.pp_double[0][0]*x0+s->v.ptr.pp_double[0][1]*x1+s->v.ptr.pp_double[0][rbf_mxnx];
    if( s->nc==0 )
    {
        return result;
    }
    rvectorsetlengthatleast(&s->calcbufxcx, rbf_mxnx, _state);
    for(i=0; i<=rbf_mxnx-1; i++)
    {
        s->calcbufxcx.ptr.p_double[i] = 0.0;
    }
    s->calcbufxcx.ptr.p_double[0] = x0;
    s->calcbufxcx.ptr.p_double[1] = x1;
    lx = kdtreequeryrnn(&s->tree, &s->calcbufxcx, s->rmax*rbf_rbffarradius, ae_true, _state);
    kdtreequeryresultsx(&s->tree, &s->calcbufx, _state);
    kdtreequeryresultstags(&s->tree, &s->calcbuftags, _state);
    for(i=0; i<=lx-1; i++)
    {
        tg = s->calcbuftags.ptr.p_int[i];
        d2 = ae_sqr(x0-s->calcbufx.ptr.pp_double[i][0], _state)+ae_sqr(x1-s->calcbufx.ptr.pp_double[i][1], _state);
        rcur = s->wr.ptr.pp_double[tg][0];
        bfcur = ae_exp(-d2/(rcur*rcur), _state);
        for(j=0; j<=s->nl-1; j++)
        {
            result = result+bfcur*s->wr.ptr.pp_double[tg][1+j];
            rcur = 0.5*rcur;
            t = bfcur*bfcur;
            bfcur = t*t;
        }
    }
    return result;
}


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
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t lx;
    ae_int_t tg;
    double t;
    double rcur;
    double bf;
    double result;


    ae_assert(ae_isfinite(x0, _state), "RBFCalc3: invalid value for X0 (X0 is Inf or NaN)!", _state);
    ae_assert(ae_isfinite(x1, _state), "RBFCalc3: invalid value for X1 (X1 is Inf or NaN)!", _state);
    ae_assert(ae_isfinite(x2, _state), "RBFCalc3: invalid value for X2 (X2 is Inf or NaN)!", _state);
    if( s->ny!=1||s->nx!=3 )
    {
        result = (double)(0);
        return result;
    }
    result = s->v.ptr.pp_double[0][0]*x0+s->v.ptr.pp_double[0][1]*x1+s->v.ptr.pp_double[0][2]*x2+s->v.ptr.pp_double[0][rbf_mxnx];
    if( s->nc==0 )
    {
        return result;
    }
    
    /*
     * calculating value for F(X)
     */
    rvectorsetlengthatleast(&s->calcbufxcx, rbf_mxnx, _state);
    for(i=0; i<=rbf_mxnx-1; i++)
    {
        s->calcbufxcx.ptr.p_double[i] = 0.0;
    }
    s->calcbufxcx.ptr.p_double[0] = x0;
    s->calcbufxcx.ptr.p_double[1] = x1;
    s->calcbufxcx.ptr.p_double[2] = x2;
    lx = kdtreequeryrnn(&s->tree, &s->calcbufxcx, s->rmax*rbf_rbffarradius, ae_true, _state);
    kdtreequeryresultsx(&s->tree, &s->calcbufx, _state);
    kdtreequeryresultstags(&s->tree, &s->calcbuftags, _state);
    for(i=0; i<=lx-1; i++)
    {
        tg = s->calcbuftags.ptr.p_int[i];
        rcur = s->wr.ptr.pp_double[tg][0];
        bf = ae_exp(-(ae_sqr(x0-s->calcbufx.ptr.pp_double[i][0], _state)+ae_sqr(x1-s->calcbufx.ptr.pp_double[i][1], _state)+ae_sqr(x2-s->calcbufx.ptr.pp_double[i][2], _state))/ae_sqr(rcur, _state), _state);
        for(j=0; j<=s->nl-1; j++)
        {
            result = result+bf*s->wr.ptr.pp_double[tg][1+j];
            t = bf*bf;
            bf = t*t;
        }
    }
    return result;
}


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
     ae_state *_state)
{

    ae_vector_clear(y);

    ae_assert(x->cnt>=s->nx, "RBFCalc: Length(X)<NX", _state);
    ae_assert(isfinitevector(x, s->nx, _state), "RBFCalc: X contains infinite or NaN values", _state);
    rbfcalcbuf(s, x, y, _state);
}


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
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t lx;
    ae_int_t tg;
    double t;
    double rcur;
    double bf;


    ae_assert(x->cnt>=s->nx, "RBFCalcBuf: Length(X)<NX", _state);
    ae_assert(isfinitevector(x, s->nx, _state), "RBFCalcBuf: X contains infinite or NaN values", _state);
    if( y->cnt<s->ny )
    {
        ae_vector_set_length(y, s->ny, _state);
    }
    for(i=0; i<=s->ny-1; i++)
    {
        y->ptr.p_double[i] = s->v.ptr.pp_double[i][rbf_mxnx];
        for(j=0; j<=s->nx-1; j++)
        {
            y->ptr.p_double[i] = y->ptr.p_double[i]+s->v.ptr.pp_double[i][j]*x->ptr.p_double[j];
        }
    }
    if( s->nc==0 )
    {
        return;
    }
    rvectorsetlengthatleast(&s->calcbufxcx, rbf_mxnx, _state);
    for(i=0; i<=rbf_mxnx-1; i++)
    {
        s->calcbufxcx.ptr.p_double[i] = 0.0;
    }
    for(i=0; i<=s->nx-1; i++)
    {
        s->calcbufxcx.ptr.p_double[i] = x->ptr.p_double[i];
    }
    lx = kdtreequeryrnn(&s->tree, &s->calcbufxcx, s->rmax*rbf_rbffarradius, ae_true, _state);
    kdtreequeryresultsx(&s->tree, &s->calcbufx, _state);
    kdtreequeryresultstags(&s->tree, &s->calcbuftags, _state);
    for(i=0; i<=s->ny-1; i++)
    {
        for(j=0; j<=lx-1; j++)
        {
            tg = s->calcbuftags.ptr.p_int[j];
            rcur = s->wr.ptr.pp_double[tg][0];
            bf = ae_exp(-(ae_sqr(s->calcbufxcx.ptr.p_double[0]-s->calcbufx.ptr.pp_double[j][0], _state)+ae_sqr(s->calcbufxcx.ptr.p_double[1]-s->calcbufx.ptr.pp_double[j][1], _state)+ae_sqr(s->calcbufxcx.ptr.p_double[2]-s->calcbufx.ptr.pp_double[j][2], _state))/ae_sqr(rcur, _state), _state);
            for(k=0; k<=s->nl-1; k++)
            {
                y->ptr.p_double[i] = y->ptr.p_double[i]+bf*s->wr.ptr.pp_double[tg][1+k*s->ny+i];
                t = bf*bf;
                bf = t*t;
            }
        }
    }
}


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
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector cpx0;
    ae_vector cpx1;
    ae_vector p01;
    ae_vector p11;
    ae_vector p2;
    double rlimit;
    double xcnorm2;
    ae_int_t hp01;
    double hcpx0;
    double xc0;
    double xc1;
    double omega;
    double radius;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t d;
    ae_int_t i00;
    ae_int_t i01;
    ae_int_t i10;
    ae_int_t i11;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(y);
    ae_vector_init(&cpx0, 0, DT_REAL, _state);
    ae_vector_init(&cpx1, 0, DT_REAL, _state);
    ae_vector_init(&p01, 0, DT_INT, _state);
    ae_vector_init(&p11, 0, DT_INT, _state);
    ae_vector_init(&p2, 0, DT_INT, _state);

    ae_assert(n0>0, "RBFGridCalc2: invalid value for N0 (N0<=0)!", _state);
    ae_assert(n1>0, "RBFGridCalc2: invalid value for N1 (N1<=0)!", _state);
    ae_assert(x0->cnt>=n0, "RBFGridCalc2: Length(X0)<N0", _state);
    ae_assert(x1->cnt>=n1, "RBFGridCalc2: Length(X1)<N1", _state);
    ae_assert(isfinitevector(x0, n0, _state), "RBFGridCalc2: X0 contains infinite or NaN values!", _state);
    ae_assert(isfinitevector(x1, n1, _state), "RBFGridCalc2: X1 contains infinite or NaN values!", _state);
    ae_matrix_set_length(y, n0, n1, _state);
    for(i=0; i<=n0-1; i++)
    {
        for(j=0; j<=n1-1; j++)
        {
            y->ptr.pp_double[i][j] = (double)(0);
        }
    }
    if( (s->ny!=1||s->nx!=2)||s->nc==0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     *create and sort arrays
     */
    ae_vector_set_length(&cpx0, n0, _state);
    for(i=0; i<=n0-1; i++)
    {
        cpx0.ptr.p_double[i] = x0->ptr.p_double[i];
    }
    tagsort(&cpx0, n0, &p01, &p2, _state);
    ae_vector_set_length(&cpx1, n1, _state);
    for(i=0; i<=n1-1; i++)
    {
        cpx1.ptr.p_double[i] = x1->ptr.p_double[i];
    }
    tagsort(&cpx1, n1, &p11, &p2, _state);
    
    /*
     *calculate function's value
     */
    for(i=0; i<=s->nc-1; i++)
    {
        radius = s->wr.ptr.pp_double[i][0];
        for(d=0; d<=s->nl-1; d++)
        {
            omega = s->wr.ptr.pp_double[i][1+d];
            rlimit = radius*rbf_rbffarradius;
            
            /*
             *search lower and upper indexes
             */
            i00 = lowerbound(&cpx0, n0, s->xc.ptr.pp_double[i][0]-rlimit, _state);
            i01 = upperbound(&cpx0, n0, s->xc.ptr.pp_double[i][0]+rlimit, _state);
            i10 = lowerbound(&cpx1, n1, s->xc.ptr.pp_double[i][1]-rlimit, _state);
            i11 = upperbound(&cpx1, n1, s->xc.ptr.pp_double[i][1]+rlimit, _state);
            xc0 = s->xc.ptr.pp_double[i][0];
            xc1 = s->xc.ptr.pp_double[i][1];
            for(j=i00; j<=i01-1; j++)
            {
                hcpx0 = cpx0.ptr.p_double[j];
                hp01 = p01.ptr.p_int[j];
                for(k=i10; k<=i11-1; k++)
                {
                    xcnorm2 = ae_sqr(hcpx0-xc0, _state)+ae_sqr(cpx1.ptr.p_double[k]-xc1, _state);
                    if( ae_fp_less_eq(xcnorm2,rlimit*rlimit) )
                    {
                        y->ptr.pp_double[hp01][p11.ptr.p_int[k]] = y->ptr.pp_double[hp01][p11.ptr.p_int[k]]+ae_exp(-xcnorm2/ae_sqr(radius, _state), _state)*omega;
                    }
                }
            }
            radius = 0.5*radius;
        }
    }
    
    /*
     *add linear term
     */
    for(i=0; i<=n0-1; i++)
    {
        for(j=0; j<=n1-1; j++)
        {
            y->ptr.pp_double[i][j] = y->ptr.pp_double[i][j]+s->v.ptr.pp_double[0][0]*x0->ptr.p_double[i]+s->v.ptr.pp_double[0][1]*x1->ptr.p_double[j]+s->v.ptr.pp_double[0][rbf_mxnx];
        }
    }
    ae_frame_leave(_state);
}


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
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double rcur;

    *nx = 0;
    *ny = 0;
    ae_matrix_clear(xwr);
    *nc = 0;
    ae_matrix_clear(v);

    *nx = s->nx;
    *ny = s->ny;
    *nc = s->nc;
    
    /*
     * Fill V
     */
    ae_matrix_set_length(v, s->ny, s->nx+1, _state);
    for(i=0; i<=s->ny-1; i++)
    {
        ae_v_move(&v->ptr.pp_double[i][0], 1, &s->v.ptr.pp_double[i][0], 1, ae_v_len(0,s->nx-1));
        v->ptr.pp_double[i][s->nx] = s->v.ptr.pp_double[i][rbf_mxnx];
    }
    
    /*
     * Fill XWR and V
     */
    if( *nc*s->nl>0 )
    {
        ae_matrix_set_length(xwr, s->nc*s->nl, s->nx+s->ny+1, _state);
        for(i=0; i<=s->nc-1; i++)
        {
            rcur = s->wr.ptr.pp_double[i][0];
            for(j=0; j<=s->nl-1; j++)
            {
                ae_v_move(&xwr->ptr.pp_double[i*s->nl+j][0], 1, &s->xc.ptr.pp_double[i][0], 1, ae_v_len(0,s->nx-1));
                ae_v_move(&xwr->ptr.pp_double[i*s->nl+j][s->nx], 1, &s->wr.ptr.pp_double[i][1+j*s->ny], 1, ae_v_len(s->nx,s->nx+s->ny-1));
                xwr->ptr.pp_double[i*s->nl+j][s->nx+s->ny] = rcur;
                rcur = 0.5*rcur;
            }
        }
    }
}


/*************************************************************************
Serializer: allocation

  -- ALGLIB --
     Copyright 02.02.2012 by Bochkanov Sergey
*************************************************************************/
void rbfalloc(ae_serializer* s, rbfmodel* model, ae_state *_state)
{


    
    /*
     * Header
     */
    ae_serializer_alloc_entry(s);
    ae_serializer_alloc_entry(s);
    
    /*
     * Data
     */
    ae_serializer_alloc_entry(s);
    ae_serializer_alloc_entry(s);
    ae_serializer_alloc_entry(s);
    ae_serializer_alloc_entry(s);
    kdtreealloc(s, &model->tree, _state);
    allocrealmatrix(s, &model->xc, -1, -1, _state);
    allocrealmatrix(s, &model->wr, -1, -1, _state);
    ae_serializer_alloc_entry(s);
    allocrealmatrix(s, &model->v, -1, -1, _state);
}


/*************************************************************************
Serializer: serialization

  -- ALGLIB --
     Copyright 02.02.2012 by Bochkanov Sergey
*************************************************************************/
void rbfserialize(ae_serializer* s, rbfmodel* model, ae_state *_state)
{


    
    /*
     * Header
     */
    ae_serializer_serialize_int(s, getrbfserializationcode(_state), _state);
    ae_serializer_serialize_int(s, rbf_rbffirstversion, _state);
    
    /*
     * Data
     */
    ae_serializer_serialize_int(s, model->nx, _state);
    ae_serializer_serialize_int(s, model->ny, _state);
    ae_serializer_serialize_int(s, model->nc, _state);
    ae_serializer_serialize_int(s, model->nl, _state);
    kdtreeserialize(s, &model->tree, _state);
    serializerealmatrix(s, &model->xc, -1, -1, _state);
    serializerealmatrix(s, &model->wr, -1, -1, _state);
    ae_serializer_serialize_double(s, model->rmax, _state);
    serializerealmatrix(s, &model->v, -1, -1, _state);
}


/*************************************************************************
Serializer: unserialization

  -- ALGLIB --
     Copyright 02.02.2012 by Bochkanov Sergey
*************************************************************************/
void rbfunserialize(ae_serializer* s, rbfmodel* model, ae_state *_state)
{
    ae_int_t i0;
    ae_int_t i1;
    ae_int_t nx;
    ae_int_t ny;

    _rbfmodel_clear(model);

    
    /*
     * Header
     */
    ae_serializer_unserialize_int(s, &i0, _state);
    ae_assert(i0==getrbfserializationcode(_state), "RBFUnserialize: stream header corrupted", _state);
    ae_serializer_unserialize_int(s, &i1, _state);
    ae_assert(i1==rbf_rbffirstversion, "RBFUnserialize: stream header corrupted", _state);
    
    /*
     * Unserialize primary model parameters, initialize model.
     *
     * It is necessary to call RBFCreate() because some internal fields
     * which are NOT unserialized will need initialization.
     */
    ae_serializer_unserialize_int(s, &nx, _state);
    ae_serializer_unserialize_int(s, &ny, _state);
    rbfcreate(nx, ny, model, _state);
    ae_serializer_unserialize_int(s, &model->nc, _state);
    ae_serializer_unserialize_int(s, &model->nl, _state);
    kdtreeunserialize(s, &model->tree, _state);
    unserializerealmatrix(s, &model->xc, _state);
    unserializerealmatrix(s, &model->wr, _state);
    ae_serializer_unserialize_double(s, &model->rmax, _state);
    unserializerealmatrix(s, &model->v, _state);
}


/*************************************************************************
This function changes centers allocation algorithm to one which  allocates
centers exactly at the dataset points (one input point = one center). This
function won't have effect until next call to RBFBuildModel().

INPUT PARAMETERS:
    S       -   RBF model, initialized by RBFCreate() call

NOTE: this   function  has   some   serialization-related  subtleties.  We
      recommend you to study serialization examples from ALGLIB  Reference
      Manual if you want to perform serialization of your models.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
static void rbf_rbfgridpoints(rbfmodel* s, ae_state *_state)
{


    s->gridtype = 2;
}


/*************************************************************************
This function changes radii  calculation  algorithm  to  one  which  makes
radius for I-th node equal to R[i]=DistNN[i]*Q, where:
* R[i] is a radius calculated by the algorithm
* DistNN[i] is distance from I-th center to its nearest neighbor center
* Q is a scale parameter, which should be within [0.75,1.50], with
  recommended value equal to 1.0
* after performing radii calculation, radii are transformed  in  order  to
  avoid situation when single outlier has very large radius and influences
  many points across entire dataset. Transformation has following form:
       new_r[i] = min(r[i],Z*median(r[]))
   where r[i] is I-th radius, median()  is  a median  radius across entire
   dataset, Z is user-specified value which controls amount  of  deviation
   from median radius.

This function won't have effect until next call to RBFBuildModel().

The idea behind this algorithm is to choose radii corresponding  to  basis
functions is such way that I-th radius is approximately equal to  distance
from I-th center to its nearest neighbor. In this case  interactions  with
distant points will be insignificant, and we  will  get  well  conditioned
basis.

Properties of this basis depend on the value of Q:
* Q<0.75 will give perfectly conditioned basis,  but  terrible  smoothness
  properties (RBF interpolant will have sharp peaks around function values)
* Q>1.5 will lead to badly conditioned systems and slow convergence of the
  underlying linear solver (although smoothness will be very good)
* Q around 1.0 gives good balance between smoothness and condition number


INPUT PARAMETERS:
    S       -   RBF model, initialized by RBFCreate() call
    Q       -   radius coefficient, Q>0
    Z       -   z-parameter, Z>0
    
Default value of Q is equal to 1.0
Default value of Z is equal to 5.0

NOTE: this   function  has   some   serialization-related  subtleties.  We
      recommend you to study serialization examples from ALGLIB  Reference
      Manual if you want to perform serialization of your models.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
static void rbf_rbfradnn(rbfmodel* s,
     double q,
     double z,
     ae_state *_state)
{


    ae_assert(ae_isfinite(q, _state)&&ae_fp_greater(q,(double)(0)), "RBFRadNN: Q<=0, infinite or NAN", _state);
    ae_assert(ae_isfinite(z, _state)&&ae_fp_greater(z,(double)(0)), "RBFRadNN: Z<=0, infinite or NAN", _state);
    s->fixrad = ae_false;
    s->radvalue = q;
    s->radzvalue = z;
}


static ae_bool rbf_buildlinearmodel(/* Real    */ ae_matrix* x,
     /* Real    */ ae_matrix* y,
     ae_int_t n,
     ae_int_t ny,
     ae_int_t modeltype,
     /* Real    */ ae_matrix* v,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector tmpy;
    ae_matrix a;
    double scaling;
    ae_vector shifting;
    double mn;
    double mx;
    ae_vector c;
    lsfitreport rep;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t info;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(v);
    ae_vector_init(&tmpy, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&shifting, 0, DT_REAL, _state);
    ae_vector_init(&c, 0, DT_REAL, _state);
    _lsfitreport_init(&rep, _state);

    ae_assert(n>=0, "BuildLinearModel: N<0", _state);
    ae_assert(ny>0, "BuildLinearModel: NY<=0", _state);
    
    /*
     * Handle degenerate case (N=0)
     */
    result = ae_true;
    ae_matrix_set_length(v, ny, rbf_mxnx+1, _state);
    if( n==0 )
    {
        for(j=0; j<=rbf_mxnx; j++)
        {
            for(i=0; i<=ny-1; i++)
            {
                v->ptr.pp_double[i][j] = (double)(0);
            }
        }
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Allocate temporaries
     */
    ae_vector_set_length(&tmpy, n, _state);
    
    /*
     * General linear model.
     */
    if( modeltype==1 )
    {
        
        /*
         * Calculate scaling/shifting, transform variables, prepare LLS problem
         */
        ae_matrix_set_length(&a, n, rbf_mxnx+1, _state);
        ae_vector_set_length(&shifting, rbf_mxnx, _state);
        scaling = (double)(0);
        for(i=0; i<=rbf_mxnx-1; i++)
        {
            mn = x->ptr.pp_double[0][i];
            mx = mn;
            for(j=1; j<=n-1; j++)
            {
                if( ae_fp_greater(mn,x->ptr.pp_double[j][i]) )
                {
                    mn = x->ptr.pp_double[j][i];
                }
                if( ae_fp_less(mx,x->ptr.pp_double[j][i]) )
                {
                    mx = x->ptr.pp_double[j][i];
                }
            }
            scaling = ae_maxreal(scaling, mx-mn, _state);
            shifting.ptr.p_double[i] = 0.5*(mx+mn);
        }
        if( ae_fp_eq(scaling,(double)(0)) )
        {
            scaling = (double)(1);
        }
        else
        {
            scaling = 0.5*scaling;
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=rbf_mxnx-1; j++)
            {
                a.ptr.pp_double[i][j] = (x->ptr.pp_double[i][j]-shifting.ptr.p_double[j])/scaling;
            }
        }
        for(i=0; i<=n-1; i++)
        {
            a.ptr.pp_double[i][rbf_mxnx] = (double)(1);
        }
        
        /*
         * Solve linear system in transformed variables, make backward 
         */
        for(i=0; i<=ny-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                tmpy.ptr.p_double[j] = y->ptr.pp_double[j][i];
            }
            lsfitlinear(&tmpy, &a, n, rbf_mxnx+1, &info, &c, &rep, _state);
            if( info<=0 )
            {
                result = ae_false;
                ae_frame_leave(_state);
                return result;
            }
            for(j=0; j<=rbf_mxnx-1; j++)
            {
                v->ptr.pp_double[i][j] = c.ptr.p_double[j]/scaling;
            }
            v->ptr.pp_double[i][rbf_mxnx] = c.ptr.p_double[rbf_mxnx];
            for(j=0; j<=rbf_mxnx-1; j++)
            {
                v->ptr.pp_double[i][rbf_mxnx] = v->ptr.pp_double[i][rbf_mxnx]-shifting.ptr.p_double[j]*v->ptr.pp_double[i][j];
            }
            for(j=0; j<=n-1; j++)
            {
                for(k=0; k<=rbf_mxnx-1; k++)
                {
                    y->ptr.pp_double[j][i] = y->ptr.pp_double[j][i]-x->ptr.pp_double[j][k]*v->ptr.pp_double[i][k];
                }
                y->ptr.pp_double[j][i] = y->ptr.pp_double[j][i]-v->ptr.pp_double[i][rbf_mxnx];
            }
        }
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Constant model, very simple
     */
    if( modeltype==2 )
    {
        for(i=0; i<=ny-1; i++)
        {
            for(j=0; j<=rbf_mxnx; j++)
            {
                v->ptr.pp_double[i][j] = (double)(0);
            }
            for(j=0; j<=n-1; j++)
            {
                v->ptr.pp_double[i][rbf_mxnx] = v->ptr.pp_double[i][rbf_mxnx]+y->ptr.pp_double[j][i];
            }
            if( n>0 )
            {
                v->ptr.pp_double[i][rbf_mxnx] = v->ptr.pp_double[i][rbf_mxnx]/n;
            }
            for(j=0; j<=n-1; j++)
            {
                y->ptr.pp_double[j][i] = y->ptr.pp_double[j][i]-v->ptr.pp_double[i][rbf_mxnx];
            }
        }
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Zero model
     */
    ae_assert(modeltype==3, "BuildLinearModel: unknown model type", _state);
    for(i=0; i<=ny-1; i++)
    {
        for(j=0; j<=rbf_mxnx; j++)
        {
            v->ptr.pp_double[i][j] = (double)(0);
        }
    }
    ae_frame_leave(_state);
    return result;
}


static void rbf_buildrbfmodellsqr(/* Real    */ ae_matrix* x,
     /* Real    */ ae_matrix* y,
     /* Real    */ ae_matrix* xc,
     /* Real    */ ae_vector* r,
     ae_int_t n,
     ae_int_t nc,
     ae_int_t ny,
     kdtree* pointstree,
     kdtree* centerstree,
     double epsort,
     double epserr,
     ae_int_t maxits,
     ae_int_t* gnnz,
     ae_int_t* snnz,
     /* Real    */ ae_matrix* w,
     ae_int_t* info,
     ae_int_t* iterationscount,
     ae_int_t* nmv,
     ae_state *_state)
{
    ae_frame _frame_block;
    linlsqrstate state;
    linlsqrreport lsqrrep;
    sparsematrix spg;
    sparsematrix sps;
    ae_vector nearcenterscnt;
    ae_vector nearpointscnt;
    ae_vector skipnearpointscnt;
    ae_vector farpointscnt;
    ae_int_t maxnearcenterscnt;
    ae_int_t maxnearpointscnt;
    ae_int_t maxfarpointscnt;
    ae_int_t sumnearcenterscnt;
    ae_int_t sumnearpointscnt;
    ae_int_t sumfarpointscnt;
    double maxrad;
    ae_vector pointstags;
    ae_vector centerstags;
    ae_matrix nearpoints;
    ae_matrix nearcenters;
    ae_matrix farpoints;
    ae_int_t tmpi;
    ae_int_t pointscnt;
    ae_int_t centerscnt;
    ae_vector xcx;
    ae_vector tmpy;
    ae_vector tc;
    ae_vector g;
    ae_vector c;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t sind;
    ae_matrix a;
    double vv;
    double vx;
    double vy;
    double vz;
    double vr;
    double gnorm2;
    ae_vector tmp0;
    ae_vector tmp1;
    ae_vector tmp2;
    double fx;
    ae_matrix xx;
    ae_matrix cx;
    double mrad;

    ae_frame_make(_state, &_frame_block);
    *gnnz = 0;
    *snnz = 0;
    ae_matrix_clear(w);
    *info = 0;
    *iterationscount = 0;
    *nmv = 0;
    _linlsqrstate_init(&state, _state);
    _linlsqrreport_init(&lsqrrep, _state);
    _sparsematrix_init(&spg, _state);
    _sparsematrix_init(&sps, _state);
    ae_vector_init(&nearcenterscnt, 0, DT_INT, _state);
    ae_vector_init(&nearpointscnt, 0, DT_INT, _state);
    ae_vector_init(&skipnearpointscnt, 0, DT_INT, _state);
    ae_vector_init(&farpointscnt, 0, DT_INT, _state);
    ae_vector_init(&pointstags, 0, DT_INT, _state);
    ae_vector_init(&centerstags, 0, DT_INT, _state);
    ae_matrix_init(&nearpoints, 0, 0, DT_REAL, _state);
    ae_matrix_init(&nearcenters, 0, 0, DT_REAL, _state);
    ae_matrix_init(&farpoints, 0, 0, DT_REAL, _state);
    ae_vector_init(&xcx, 0, DT_REAL, _state);
    ae_vector_init(&tmpy, 0, DT_REAL, _state);
    ae_vector_init(&tc, 0, DT_REAL, _state);
    ae_vector_init(&g, 0, DT_REAL, _state);
    ae_vector_init(&c, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&tmp0, 0, DT_REAL, _state);
    ae_vector_init(&tmp1, 0, DT_REAL, _state);
    ae_vector_init(&tmp2, 0, DT_REAL, _state);
    ae_matrix_init(&xx, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cx, 0, 0, DT_REAL, _state);

    
    /*
     * Handle special cases: NC=0
     */
    if( nc==0 )
    {
        *info = 1;
        *iterationscount = 0;
        *nmv = 0;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Prepare for general case, NC>0
     */
    ae_vector_set_length(&xcx, rbf_mxnx, _state);
    ae_vector_set_length(&pointstags, n, _state);
    ae_vector_set_length(&centerstags, nc, _state);
    *info = -1;
    *iterationscount = 0;
    *nmv = 0;
    
    /*
     * This block prepares quantities used to compute approximate cardinal basis functions (ACBFs):
     * * NearCentersCnt[]   -   array[NC], whose elements store number of near centers used to build ACBF
     * * NearPointsCnt[]    -   array[NC], number of near points used to build ACBF
     * * FarPointsCnt[]     -   array[NC], number of far points (ones where ACBF is nonzero)
     * * MaxNearCentersCnt  -   max(NearCentersCnt)
     * * MaxNearPointsCnt   -   max(NearPointsCnt)
     * * SumNearCentersCnt  -   sum(NearCentersCnt)
     * * SumNearPointsCnt   -   sum(NearPointsCnt)
     * * SumFarPointsCnt    -   sum(FarPointsCnt)
     */
    ae_vector_set_length(&nearcenterscnt, nc, _state);
    ae_vector_set_length(&nearpointscnt, nc, _state);
    ae_vector_set_length(&skipnearpointscnt, nc, _state);
    ae_vector_set_length(&farpointscnt, nc, _state);
    maxnearcenterscnt = 0;
    maxnearpointscnt = 0;
    maxfarpointscnt = 0;
    sumnearcenterscnt = 0;
    sumnearpointscnt = 0;
    sumfarpointscnt = 0;
    for(i=0; i<=nc-1; i++)
    {
        for(j=0; j<=rbf_mxnx-1; j++)
        {
            xcx.ptr.p_double[j] = xc->ptr.pp_double[i][j];
        }
        
        /*
         * Determine number of near centers and maximum radius of near centers
         */
        nearcenterscnt.ptr.p_int[i] = kdtreequeryrnn(centerstree, &xcx, r->ptr.p_double[i]*rbf_rbfnearradius, ae_true, _state);
        kdtreequeryresultstags(centerstree, &centerstags, _state);
        maxrad = (double)(0);
        for(j=0; j<=nearcenterscnt.ptr.p_int[i]-1; j++)
        {
            maxrad = ae_maxreal(maxrad, ae_fabs(r->ptr.p_double[centerstags.ptr.p_int[j]], _state), _state);
        }
        
        /*
         * Determine number of near points (ones which used to build ACBF)
         * and skipped points (the most near points which are NOT used to build ACBF
         * and are NOT included in the near points count
         */
        skipnearpointscnt.ptr.p_int[i] = kdtreequeryrnn(pointstree, &xcx, 0.1*r->ptr.p_double[i], ae_true, _state);
        nearpointscnt.ptr.p_int[i] = kdtreequeryrnn(pointstree, &xcx, (r->ptr.p_double[i]+maxrad)*rbf_rbfnearradius, ae_true, _state)-skipnearpointscnt.ptr.p_int[i];
        ae_assert(nearpointscnt.ptr.p_int[i]>=0, "BuildRBFModelLSQR: internal error", _state);
        
        /*
         * Determine number of far points
         */
        farpointscnt.ptr.p_int[i] = kdtreequeryrnn(pointstree, &xcx, ae_maxreal(r->ptr.p_double[i]*rbf_rbfnearradius+maxrad*rbf_rbffarradius, r->ptr.p_double[i]*rbf_rbffarradius, _state), ae_true, _state);
        
        /*
         * calculate sum and max, make some basic checks
         */
        ae_assert(nearcenterscnt.ptr.p_int[i]>0, "BuildRBFModelLSQR: internal error", _state);
        maxnearcenterscnt = ae_maxint(maxnearcenterscnt, nearcenterscnt.ptr.p_int[i], _state);
        maxnearpointscnt = ae_maxint(maxnearpointscnt, nearpointscnt.ptr.p_int[i], _state);
        maxfarpointscnt = ae_maxint(maxfarpointscnt, farpointscnt.ptr.p_int[i], _state);
        sumnearcenterscnt = sumnearcenterscnt+nearcenterscnt.ptr.p_int[i];
        sumnearpointscnt = sumnearpointscnt+nearpointscnt.ptr.p_int[i];
        sumfarpointscnt = sumfarpointscnt+farpointscnt.ptr.p_int[i];
    }
    *snnz = sumnearcenterscnt;
    *gnnz = sumfarpointscnt;
    ae_assert(maxnearcenterscnt>0, "BuildRBFModelLSQR: internal error", _state);
    
    /*
     * Allocate temporaries.
     *
     * NOTE: we want to avoid allocation of zero-size arrays, so we
     *       use max(desired_size,1) instead of desired_size when performing
     *       memory allocation.
     */
    ae_matrix_set_length(&a, maxnearpointscnt+maxnearcenterscnt, maxnearcenterscnt, _state);
    ae_vector_set_length(&tmpy, maxnearpointscnt+maxnearcenterscnt, _state);
    ae_vector_set_length(&g, maxnearcenterscnt, _state);
    ae_vector_set_length(&c, maxnearcenterscnt, _state);
    ae_matrix_set_length(&nearcenters, maxnearcenterscnt, rbf_mxnx, _state);
    ae_matrix_set_length(&nearpoints, ae_maxint(maxnearpointscnt, 1, _state), rbf_mxnx, _state);
    ae_matrix_set_length(&farpoints, ae_maxint(maxfarpointscnt, 1, _state), rbf_mxnx, _state);
    
    /*
     * fill matrix SpG
     */
    sparsecreate(n, nc, *gnnz, &spg, _state);
    sparsecreate(nc, nc, *snnz, &sps, _state);
    for(i=0; i<=nc-1; i++)
    {
        centerscnt = nearcenterscnt.ptr.p_int[i];
        
        /*
         * main center
         */
        for(j=0; j<=rbf_mxnx-1; j++)
        {
            xcx.ptr.p_double[j] = xc->ptr.pp_double[i][j];
        }
        
        /*
         * center's tree
         */
        tmpi = kdtreequeryknn(centerstree, &xcx, centerscnt, ae_true, _state);
        ae_assert(tmpi==centerscnt, "BuildRBFModelLSQR: internal error", _state);
        kdtreequeryresultsx(centerstree, &cx, _state);
        kdtreequeryresultstags(centerstree, &centerstags, _state);
        
        /*
         * point's tree
         */
        mrad = (double)(0);
        for(j=0; j<=centerscnt-1; j++)
        {
            mrad = ae_maxreal(mrad, r->ptr.p_double[centerstags.ptr.p_int[j]], _state);
        }
        
        /*
         * we need to be sure that 'CTree' contains
         * at least one side center
         */
        sparseset(&sps, i, i, (double)(1), _state);
        c.ptr.p_double[0] = 1.0;
        for(j=1; j<=centerscnt-1; j++)
        {
            c.ptr.p_double[j] = 0.0;
        }
        if( centerscnt>1&&nearpointscnt.ptr.p_int[i]>0 )
        {
            
            /*
             * first KDTree request for points
             */
            pointscnt = nearpointscnt.ptr.p_int[i];
            tmpi = kdtreequeryknn(pointstree, &xcx, skipnearpointscnt.ptr.p_int[i]+nearpointscnt.ptr.p_int[i], ae_true, _state);
            ae_assert(tmpi==skipnearpointscnt.ptr.p_int[i]+nearpointscnt.ptr.p_int[i], "BuildRBFModelLSQR: internal error", _state);
            kdtreequeryresultsx(pointstree, &xx, _state);
            sind = skipnearpointscnt.ptr.p_int[i];
            for(j=0; j<=pointscnt-1; j++)
            {
                vx = xx.ptr.pp_double[sind+j][0];
                vy = xx.ptr.pp_double[sind+j][1];
                vz = xx.ptr.pp_double[sind+j][2];
                for(k=0; k<=centerscnt-1; k++)
                {
                    vr = 0.0;
                    vv = vx-cx.ptr.pp_double[k][0];
                    vr = vr+vv*vv;
                    vv = vy-cx.ptr.pp_double[k][1];
                    vr = vr+vv*vv;
                    vv = vz-cx.ptr.pp_double[k][2];
                    vr = vr+vv*vv;
                    vv = r->ptr.p_double[centerstags.ptr.p_int[k]];
                    a.ptr.pp_double[j][k] = ae_exp(-vr/(vv*vv), _state);
                }
            }
            for(j=0; j<=centerscnt-1; j++)
            {
                g.ptr.p_double[j] = ae_exp(-(ae_sqr(xcx.ptr.p_double[0]-cx.ptr.pp_double[j][0], _state)+ae_sqr(xcx.ptr.p_double[1]-cx.ptr.pp_double[j][1], _state)+ae_sqr(xcx.ptr.p_double[2]-cx.ptr.pp_double[j][2], _state))/ae_sqr(r->ptr.p_double[centerstags.ptr.p_int[j]], _state), _state);
            }
            
            /*
             * calculate the problem
             */
            gnorm2 = ae_v_dotproduct(&g.ptr.p_double[0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,centerscnt-1));
            for(j=0; j<=pointscnt-1; j++)
            {
                vv = ae_v_dotproduct(&a.ptr.pp_double[j][0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,centerscnt-1));
                vv = vv/gnorm2;
                tmpy.ptr.p_double[j] = -vv;
                ae_v_subd(&a.ptr.pp_double[j][0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,centerscnt-1), vv);
            }
            for(j=pointscnt; j<=pointscnt+centerscnt-1; j++)
            {
                for(k=0; k<=centerscnt-1; k++)
                {
                    a.ptr.pp_double[j][k] = 0.0;
                }
                a.ptr.pp_double[j][j-pointscnt] = 1.0E-6;
                tmpy.ptr.p_double[j] = 0.0;
            }
            fblssolvels(&a, &tmpy, pointscnt+centerscnt, centerscnt, &tmp0, &tmp1, &tmp2, _state);
            ae_v_move(&c.ptr.p_double[0], 1, &tmpy.ptr.p_double[0], 1, ae_v_len(0,centerscnt-1));
            vv = ae_v_dotproduct(&g.ptr.p_double[0], 1, &c.ptr.p_double[0], 1, ae_v_len(0,centerscnt-1));
            vv = vv/gnorm2;
            ae_v_subd(&c.ptr.p_double[0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,centerscnt-1), vv);
            vv = 1/gnorm2;
            ae_v_addd(&c.ptr.p_double[0], 1, &g.ptr.p_double[0], 1, ae_v_len(0,centerscnt-1), vv);
            for(j=0; j<=centerscnt-1; j++)
            {
                sparseset(&sps, i, centerstags.ptr.p_int[j], c.ptr.p_double[j], _state);
            }
        }
        
        /*
         * second KDTree request for points
         */
        pointscnt = farpointscnt.ptr.p_int[i];
        tmpi = kdtreequeryknn(pointstree, &xcx, pointscnt, ae_true, _state);
        ae_assert(tmpi==pointscnt, "BuildRBFModelLSQR: internal error", _state);
        kdtreequeryresultsx(pointstree, &xx, _state);
        kdtreequeryresultstags(pointstree, &pointstags, _state);
        
        /*
         *fill SpG matrix
         */
        for(j=0; j<=pointscnt-1; j++)
        {
            fx = (double)(0);
            vx = xx.ptr.pp_double[j][0];
            vy = xx.ptr.pp_double[j][1];
            vz = xx.ptr.pp_double[j][2];
            for(k=0; k<=centerscnt-1; k++)
            {
                vr = 0.0;
                vv = vx-cx.ptr.pp_double[k][0];
                vr = vr+vv*vv;
                vv = vy-cx.ptr.pp_double[k][1];
                vr = vr+vv*vv;
                vv = vz-cx.ptr.pp_double[k][2];
                vr = vr+vv*vv;
                vv = r->ptr.p_double[centerstags.ptr.p_int[k]];
                vv = vv*vv;
                fx = fx+c.ptr.p_double[k]*ae_exp(-vr/vv, _state);
            }
            sparseset(&spg, pointstags.ptr.p_int[j], i, fx, _state);
        }
    }
    sparseconverttocrs(&spg, _state);
    sparseconverttocrs(&sps, _state);
    
    /*
     * solve by LSQR method
     */
    ae_vector_set_length(&tmpy, n, _state);
    ae_vector_set_length(&tc, nc, _state);
    ae_matrix_set_length(w, nc, ny, _state);
    linlsqrcreate(n, nc, &state, _state);
    linlsqrsetcond(&state, epsort, epserr, maxits, _state);
    for(i=0; i<=ny-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            tmpy.ptr.p_double[j] = y->ptr.pp_double[j][i];
        }
        linlsqrsolvesparse(&state, &spg, &tmpy, _state);
        linlsqrresults(&state, &c, &lsqrrep, _state);
        if( lsqrrep.terminationtype<=0 )
        {
            *info = -4;
            ae_frame_leave(_state);
            return;
        }
        sparsemtv(&sps, &c, &tc, _state);
        for(j=0; j<=nc-1; j++)
        {
            w->ptr.pp_double[j][i] = tc.ptr.p_double[j];
        }
        *iterationscount = *iterationscount+lsqrrep.iterationscount;
        *nmv = *nmv+lsqrrep.nmv;
    }
    *info = 1;
    ae_frame_leave(_state);
}


static void rbf_buildrbfmlayersmodellsqr(/* Real    */ ae_matrix* x,
     /* Real    */ ae_matrix* y,
     /* Real    */ ae_matrix* xc,
     double rval,
     /* Real    */ ae_vector* r,
     ae_int_t n,
     ae_int_t* nc,
     ae_int_t ny,
     ae_int_t nlayers,
     kdtree* centerstree,
     double epsort,
     double epserr,
     ae_int_t maxits,
     double lambdav,
     ae_int_t* annz,
     /* Real    */ ae_matrix* w,
     ae_int_t* info,
     ae_int_t* iterationscount,
     ae_int_t* nmv,
     ae_state *_state)
{
    ae_frame _frame_block;
    linlsqrstate state;
    linlsqrreport lsqrrep;
    sparsematrix spa;
    double anorm;
    ae_vector omega;
    ae_vector xx;
    ae_vector tmpy;
    ae_matrix cx;
    double yval;
    ae_int_t nec;
    ae_vector centerstags;
    ae_int_t layer;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    double rmaxbefore;
    double rmaxafter;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(xc);
    ae_vector_clear(r);
    *nc = 0;
    *annz = 0;
    ae_matrix_clear(w);
    *info = 0;
    *iterationscount = 0;
    *nmv = 0;
    _linlsqrstate_init(&state, _state);
    _linlsqrreport_init(&lsqrrep, _state);
    _sparsematrix_init(&spa, _state);
    ae_vector_init(&omega, 0, DT_REAL, _state);
    ae_vector_init(&xx, 0, DT_REAL, _state);
    ae_vector_init(&tmpy, 0, DT_REAL, _state);
    ae_matrix_init(&cx, 0, 0, DT_REAL, _state);
    ae_vector_init(&centerstags, 0, DT_INT, _state);

    ae_assert(nlayers>=0, "BuildRBFMLayersModelLSQR: invalid argument(NLayers<0)", _state);
    ae_assert(n>=0, "BuildRBFMLayersModelLSQR: invalid argument(N<0)", _state);
    ae_assert(rbf_mxnx>0&&rbf_mxnx<=3, "BuildRBFMLayersModelLSQR: internal error(invalid global const MxNX: either MxNX<=0 or MxNX>3)", _state);
    *annz = 0;
    if( n==0||nlayers==0 )
    {
        *info = 1;
        *iterationscount = 0;
        *nmv = 0;
        ae_frame_leave(_state);
        return;
    }
    *nc = n*nlayers;
    ae_vector_set_length(&xx, rbf_mxnx, _state);
    ae_vector_set_length(&centerstags, n, _state);
    ae_matrix_set_length(xc, *nc, rbf_mxnx, _state);
    ae_vector_set_length(r, *nc, _state);
    for(i=0; i<=*nc-1; i++)
    {
        for(j=0; j<=rbf_mxnx-1; j++)
        {
            xc->ptr.pp_double[i][j] = x->ptr.pp_double[i%n][j];
        }
    }
    for(i=0; i<=*nc-1; i++)
    {
        r->ptr.p_double[i] = rval/ae_pow((double)(2), (double)(i/n), _state);
    }
    for(i=0; i<=n-1; i++)
    {
        centerstags.ptr.p_int[i] = i;
    }
    kdtreebuildtagged(xc, &centerstags, n, rbf_mxnx, 0, 2, centerstree, _state);
    ae_vector_set_length(&omega, n, _state);
    ae_vector_set_length(&tmpy, n, _state);
    ae_matrix_set_length(w, *nc, ny, _state);
    *info = -1;
    *iterationscount = 0;
    *nmv = 0;
    linlsqrcreate(n, n, &state, _state);
    linlsqrsetcond(&state, epsort, epserr, maxits, _state);
    linlsqrsetlambdai(&state, 1.0E-6, _state);
    
    /*
     * calculate number of non-zero elements for sparse matrix
     */
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=rbf_mxnx-1; j++)
        {
            xx.ptr.p_double[j] = x->ptr.pp_double[i][j];
        }
        *annz = *annz+kdtreequeryrnn(centerstree, &xx, r->ptr.p_double[0]*rbf_rbfmlradius, ae_true, _state);
    }
    for(layer=0; layer<=nlayers-1; layer++)
    {
        
        /*
         * Fill sparse matrix, calculate norm(A)
         */
        anorm = 0.0;
        sparsecreate(n, n, *annz, &spa, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=rbf_mxnx-1; j++)
            {
                xx.ptr.p_double[j] = x->ptr.pp_double[i][j];
            }
            nec = kdtreequeryrnn(centerstree, &xx, r->ptr.p_double[layer*n]*rbf_rbfmlradius, ae_true, _state);
            kdtreequeryresultsx(centerstree, &cx, _state);
            kdtreequeryresultstags(centerstree, &centerstags, _state);
            for(j=0; j<=nec-1; j++)
            {
                v = ae_exp(-(ae_sqr(xx.ptr.p_double[0]-cx.ptr.pp_double[j][0], _state)+ae_sqr(xx.ptr.p_double[1]-cx.ptr.pp_double[j][1], _state)+ae_sqr(xx.ptr.p_double[2]-cx.ptr.pp_double[j][2], _state))/ae_sqr(r->ptr.p_double[layer*n+centerstags.ptr.p_int[j]], _state), _state);
                sparseset(&spa, i, centerstags.ptr.p_int[j], v, _state);
                anorm = anorm+ae_sqr(v, _state);
            }
        }
        anorm = ae_sqrt(anorm, _state);
        sparseconverttocrs(&spa, _state);
        
        /*
         * Calculate maximum residual before adding new layer.
         * This value is not used by algorithm, the only purpose is to make debugging easier.
         */
        rmaxbefore = 0.0;
        for(j=0; j<=n-1; j++)
        {
            for(i=0; i<=ny-1; i++)
            {
                rmaxbefore = ae_maxreal(rmaxbefore, ae_fabs(y->ptr.pp_double[j][i], _state), _state);
            }
        }
        
        /*
         * Process NY dimensions of the target function
         */
        for(i=0; i<=ny-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                tmpy.ptr.p_double[j] = y->ptr.pp_double[j][i];
            }
            
            /*
             * calculate Omega for current layer
             */
            linlsqrsetlambdai(&state, lambdav*anorm/n, _state);
            linlsqrsolvesparse(&state, &spa, &tmpy, _state);
            linlsqrresults(&state, &omega, &lsqrrep, _state);
            if( lsqrrep.terminationtype<=0 )
            {
                *info = -4;
                ae_frame_leave(_state);
                return;
            }
            
            /*
             * calculate error for current layer
             */
            for(j=0; j<=n-1; j++)
            {
                yval = (double)(0);
                for(k=0; k<=rbf_mxnx-1; k++)
                {
                    xx.ptr.p_double[k] = x->ptr.pp_double[j][k];
                }
                nec = kdtreequeryrnn(centerstree, &xx, r->ptr.p_double[layer*n]*rbf_rbffarradius, ae_true, _state);
                kdtreequeryresultsx(centerstree, &cx, _state);
                kdtreequeryresultstags(centerstree, &centerstags, _state);
                for(k=0; k<=nec-1; k++)
                {
                    yval = yval+omega.ptr.p_double[centerstags.ptr.p_int[k]]*ae_exp(-(ae_sqr(xx.ptr.p_double[0]-cx.ptr.pp_double[k][0], _state)+ae_sqr(xx.ptr.p_double[1]-cx.ptr.pp_double[k][1], _state)+ae_sqr(xx.ptr.p_double[2]-cx.ptr.pp_double[k][2], _state))/ae_sqr(r->ptr.p_double[layer*n+centerstags.ptr.p_int[k]], _state), _state);
                }
                y->ptr.pp_double[j][i] = y->ptr.pp_double[j][i]-yval;
            }
            
            /*
             * write Omega in out parameter W
             */
            for(j=0; j<=n-1; j++)
            {
                w->ptr.pp_double[layer*n+j][i] = omega.ptr.p_double[j];
            }
            *iterationscount = *iterationscount+lsqrrep.iterationscount;
            *nmv = *nmv+lsqrrep.nmv;
        }
        
        /*
         * Calculate maximum residual before adding new layer.
         * This value is not used by algorithm, the only purpose is to make debugging easier.
         */
        rmaxafter = 0.0;
        for(j=0; j<=n-1; j++)
        {
            for(i=0; i<=ny-1; i++)
            {
                rmaxafter = ae_maxreal(rmaxafter, ae_fabs(y->ptr.pp_double[j][i], _state), _state);
            }
        }
    }
    *info = 1;
    ae_frame_leave(_state);
}


void _rbfmodel_init(void* _p, ae_state *_state)
{
    rbfmodel *p = (rbfmodel*)_p;
    ae_touch_ptr((void*)p);
    _kdtree_init(&p->tree, _state);
    ae_matrix_init(&p->xc, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->wr, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->v, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->x, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->y, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->calcbufxcx, 0, DT_REAL, _state);
    ae_matrix_init(&p->calcbufx, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->calcbuftags, 0, DT_INT, _state);
}


void _rbfmodel_init_copy(void* _dst, void* _src, ae_state *_state)
{
    rbfmodel *dst = (rbfmodel*)_dst;
    rbfmodel *src = (rbfmodel*)_src;
    dst->ny = src->ny;
    dst->nx = src->nx;
    dst->nc = src->nc;
    dst->nl = src->nl;
    _kdtree_init_copy(&dst->tree, &src->tree, _state);
    ae_matrix_init_copy(&dst->xc, &src->xc, _state);
    ae_matrix_init_copy(&dst->wr, &src->wr, _state);
    dst->rmax = src->rmax;
    ae_matrix_init_copy(&dst->v, &src->v, _state);
    dst->gridtype = src->gridtype;
    dst->fixrad = src->fixrad;
    dst->lambdav = src->lambdav;
    dst->radvalue = src->radvalue;
    dst->radzvalue = src->radzvalue;
    dst->nlayers = src->nlayers;
    dst->aterm = src->aterm;
    dst->algorithmtype = src->algorithmtype;
    dst->epsort = src->epsort;
    dst->epserr = src->epserr;
    dst->maxits = src->maxits;
    dst->h = src->h;
    dst->n = src->n;
    ae_matrix_init_copy(&dst->x, &src->x, _state);
    ae_matrix_init_copy(&dst->y, &src->y, _state);
    ae_vector_init_copy(&dst->calcbufxcx, &src->calcbufxcx, _state);
    ae_matrix_init_copy(&dst->calcbufx, &src->calcbufx, _state);
    ae_vector_init_copy(&dst->calcbuftags, &src->calcbuftags, _state);
}


void _rbfmodel_clear(void* _p)
{
    rbfmodel *p = (rbfmodel*)_p;
    ae_touch_ptr((void*)p);
    _kdtree_clear(&p->tree);
    ae_matrix_clear(&p->xc);
    ae_matrix_clear(&p->wr);
    ae_matrix_clear(&p->v);
    ae_matrix_clear(&p->x);
    ae_matrix_clear(&p->y);
    ae_vector_clear(&p->calcbufxcx);
    ae_matrix_clear(&p->calcbufx);
    ae_vector_clear(&p->calcbuftags);
}


void _rbfmodel_destroy(void* _p)
{
    rbfmodel *p = (rbfmodel*)_p;
    ae_touch_ptr((void*)p);
    _kdtree_destroy(&p->tree);
    ae_matrix_destroy(&p->xc);
    ae_matrix_destroy(&p->wr);
    ae_matrix_destroy(&p->v);
    ae_matrix_destroy(&p->x);
    ae_matrix_destroy(&p->y);
    ae_vector_destroy(&p->calcbufxcx);
    ae_matrix_destroy(&p->calcbufx);
    ae_vector_destroy(&p->calcbuftags);
}


void _rbfreport_init(void* _p, ae_state *_state)
{
    rbfreport *p = (rbfreport*)_p;
    ae_touch_ptr((void*)p);
}


void _rbfreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    rbfreport *dst = (rbfreport*)_dst;
    rbfreport *src = (rbfreport*)_src;
    dst->arows = src->arows;
    dst->acols = src->acols;
    dst->annz = src->annz;
    dst->iterationscount = src->iterationscount;
    dst->nmv = src->nmv;
    dst->terminationtype = src->terminationtype;
}


void _rbfreport_clear(void* _p)
{
    rbfreport *p = (rbfreport*)_p;
    ae_touch_ptr((void*)p);
}


void _rbfreport_destroy(void* _p)
{
    rbfreport *p = (rbfreport*)_p;
    ae_touch_ptr((void*)p);
}


/*$ End $*/
