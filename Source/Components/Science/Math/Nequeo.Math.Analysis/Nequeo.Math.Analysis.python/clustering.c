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
#include "clustering.h"


/*$ Declarations $*/
static double clustering_parallelcomplexity = 200000;
static ae_int_t clustering_kmeansblocksize = 32;
static ae_int_t clustering_kmeansparalleldim = 8;
static ae_int_t clustering_kmeansparallelk = 8;
static void clustering_selectinitialcenters(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t initalgo,
     ae_int_t k,
     /* Real    */ ae_matrix* ct,
     apbuffers* initbuf,
     ae_shared_pool* updatepool,
     ae_state *_state);
static ae_bool clustering_fixcenters(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     /* Real    */ ae_matrix* ct,
     ae_int_t k,
     apbuffers* initbuf,
     ae_shared_pool* updatepool,
     ae_state *_state);
static void clustering_clusterizerrunahcinternal(clusterizerstate* s,
     /* Real    */ ae_matrix* d,
     ahcreport* rep,
     ae_state *_state);
static void clustering_evaluatedistancematrixrec(/* Real    */ ae_matrix* xy,
     ae_int_t nfeatures,
     ae_int_t disttype,
     /* Real    */ ae_matrix* d,
     ae_int_t i0,
     ae_int_t i1,
     ae_int_t j0,
     ae_int_t j1,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
This function initializes clusterizer object. Newly initialized object  is
empty, i.e. it does not contain dataset. You should use it as follows:
1. creation
2. dataset is added with ClusterizerSetPoints()
3. additional parameters are set
3. clusterization is performed with one of the clustering functions

  -- ALGLIB --
     Copyright 10.07.2012 by Bochkanov Sergey
*************************************************************************/
void clusterizercreate(clusterizerstate* s, ae_state *_state)
{

    _clusterizerstate_clear(s);

    s->npoints = 0;
    s->nfeatures = 0;
    s->disttype = 2;
    s->ahcalgo = 0;
    s->kmeansrestarts = 1;
    s->kmeansmaxits = 0;
    s->kmeansinitalgo = 0;
    s->kmeansdbgnoits = ae_false;
    kmeansinitbuf(&s->kmeanstmp, _state);
}


/*************************************************************************
This function adds dataset to the clusterizer structure.

This function overrides all previous calls  of  ClusterizerSetPoints()  or
ClusterizerSetDistances().

INPUT PARAMETERS:
    S       -   clusterizer state, initialized by ClusterizerCreate()
    XY      -   array[NPoints,NFeatures], dataset
    NPoints -   number of points, >=0
    NFeatures-  number of features, >=1
    DistType-   distance function:
                *  0    Chebyshev distance  (L-inf norm)
                *  1    city block distance (L1 norm)
                *  2    Euclidean distance  (L2 norm), non-squared
                * 10    Pearson correlation:
                        dist(a,b) = 1-corr(a,b)
                * 11    Absolute Pearson correlation:
                        dist(a,b) = 1-|corr(a,b)|
                * 12    Uncentered Pearson correlation (cosine of the angle):
                        dist(a,b) = a'*b/(|a|*|b|)
                * 13    Absolute uncentered Pearson correlation
                        dist(a,b) = |a'*b|/(|a|*|b|)
                * 20    Spearman rank correlation:
                        dist(a,b) = 1-rankcorr(a,b)
                * 21    Absolute Spearman rank correlation
                        dist(a,b) = 1-|rankcorr(a,b)|

NOTE 1: different distance functions have different performance penalty:
        * Euclidean or Pearson correlation distances are the fastest ones
        * Spearman correlation distance function is a bit slower
        * city block and Chebyshev distances are order of magnitude slower
       
        The reason behing difference in performance is that correlation-based
        distance functions are computed using optimized linear algebra kernels,
        while Chebyshev and city block distance functions are computed using
        simple nested loops with two branches at each iteration.
        
NOTE 2: different clustering algorithms have different limitations:
        * agglomerative hierarchical clustering algorithms may be used with
          any kind of distance metric
        * k-means++ clustering algorithm may be used only  with  Euclidean
          distance function
        Thus, list of specific clustering algorithms you may  use  depends
        on distance function you specify when you set your dataset.
       
  -- ALGLIB --
     Copyright 10.07.2012 by Bochkanov Sergey
*************************************************************************/
void clusterizersetpoints(clusterizerstate* s,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nfeatures,
     ae_int_t disttype,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert((((((((disttype==0||disttype==1)||disttype==2)||disttype==10)||disttype==11)||disttype==12)||disttype==13)||disttype==20)||disttype==21, "ClusterizerSetPoints: incorrect DistType", _state);
    ae_assert(npoints>=0, "ClusterizerSetPoints: NPoints<0", _state);
    ae_assert(nfeatures>=1, "ClusterizerSetPoints: NFeatures<1", _state);
    ae_assert(xy->rows>=npoints, "ClusterizerSetPoints: Rows(XY)<NPoints", _state);
    ae_assert(xy->cols>=nfeatures, "ClusterizerSetPoints: Cols(XY)<NFeatures", _state);
    ae_assert(apservisfinitematrix(xy, npoints, nfeatures, _state), "ClusterizerSetPoints: XY contains NAN/INF", _state);
    s->npoints = npoints;
    s->nfeatures = nfeatures;
    s->disttype = disttype;
    rmatrixsetlengthatleast(&s->xy, npoints, nfeatures, _state);
    for(i=0; i<=npoints-1; i++)
    {
        ae_v_move(&s->xy.ptr.pp_double[i][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nfeatures-1));
    }
}


/*************************************************************************
This function adds dataset given by distance  matrix  to  the  clusterizer
structure. It is important that dataset is not  given  explicitly  -  only
distance matrix is given.

This function overrides all previous calls  of  ClusterizerSetPoints()  or
ClusterizerSetDistances().

INPUT PARAMETERS:
    S       -   clusterizer state, initialized by ClusterizerCreate()
    D       -   array[NPoints,NPoints], distance matrix given by its upper
                or lower triangle (main diagonal is  ignored  because  its
                entries are expected to be zero).
    NPoints -   number of points
    IsUpper -   whether upper or lower triangle of D is given.
        
NOTE 1: different clustering algorithms have different limitations:
        * agglomerative hierarchical clustering algorithms may be used with
          any kind of distance metric, including one  which  is  given  by
          distance matrix
        * k-means++ clustering algorithm may be used only  with  Euclidean
          distance function and explicitly given points - it  can  not  be
          used with dataset given by distance matrix
        Thus, if you call this function, you will be unable to use k-means
        clustering algorithm to process your problem.

  -- ALGLIB --
     Copyright 10.07.2012 by Bochkanov Sergey
*************************************************************************/
void clusterizersetdistances(clusterizerstate* s,
     /* Real    */ ae_matrix* d,
     ae_int_t npoints,
     ae_bool isupper,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t j0;
    ae_int_t j1;


    ae_assert(npoints>=0, "ClusterizerSetDistances: NPoints<0", _state);
    ae_assert(d->rows>=npoints, "ClusterizerSetDistances: Rows(D)<NPoints", _state);
    ae_assert(d->cols>=npoints, "ClusterizerSetDistances: Cols(D)<NPoints", _state);
    s->npoints = npoints;
    s->nfeatures = 0;
    s->disttype = -1;
    rmatrixsetlengthatleast(&s->d, npoints, npoints, _state);
    for(i=0; i<=npoints-1; i++)
    {
        if( isupper )
        {
            j0 = i+1;
            j1 = npoints-1;
        }
        else
        {
            j0 = 0;
            j1 = i-1;
        }
        for(j=j0; j<=j1; j++)
        {
            ae_assert(ae_isfinite(d->ptr.pp_double[i][j], _state)&&ae_fp_greater_eq(d->ptr.pp_double[i][j],(double)(0)), "ClusterizerSetDistances: D contains infinite, NAN or negative elements", _state);
            s->d.ptr.pp_double[i][j] = d->ptr.pp_double[i][j];
            s->d.ptr.pp_double[j][i] = d->ptr.pp_double[i][j];
        }
        s->d.ptr.pp_double[i][i] = (double)(0);
    }
}


/*************************************************************************
This function sets agglomerative hierarchical clustering algorithm

INPUT PARAMETERS:
    S       -   clusterizer state, initialized by ClusterizerCreate()
    Algo    -   algorithm type:
                * 0     complete linkage (default algorithm)
                * 1     single linkage
                * 2     unweighted average linkage
                * 3     weighted average linkage
                * 4     Ward's method

NOTE: Ward's method works correctly only with Euclidean  distance,  that's
      why algorithm will return negative termination  code  (failure)  for
      any other distance type.
      
      It is possible, however,  to  use  this  method  with  user-supplied
      distance matrix. It  is  your  responsibility  to pass one which was
      calculated with Euclidean distance function.

  -- ALGLIB --
     Copyright 10.07.2012 by Bochkanov Sergey
*************************************************************************/
void clusterizersetahcalgo(clusterizerstate* s,
     ae_int_t algo,
     ae_state *_state)
{


    ae_assert((((algo==0||algo==1)||algo==2)||algo==3)||algo==4, "ClusterizerSetHCAlgo: incorrect algorithm type", _state);
    s->ahcalgo = algo;
}


/*************************************************************************
This  function  sets k-means properties:  number  of  restarts and maximum
number of iterations per one run.

INPUT PARAMETERS:
    S       -   clusterizer state, initialized by ClusterizerCreate()
    Restarts-   restarts count, >=1.
                k-means++ algorithm performs several restarts and  chooses
                best set of centers (one with minimum squared distance).
    MaxIts  -   maximum number of k-means iterations performed during  one
                run. >=0, zero value means that algorithm performs unlimited
                number of iterations.

  -- ALGLIB --
     Copyright 10.07.2012 by Bochkanov Sergey
*************************************************************************/
void clusterizersetkmeanslimits(clusterizerstate* s,
     ae_int_t restarts,
     ae_int_t maxits,
     ae_state *_state)
{


    ae_assert(restarts>=1, "ClusterizerSetKMeansLimits: Restarts<=0", _state);
    ae_assert(maxits>=0, "ClusterizerSetKMeansLimits: MaxIts<0", _state);
    s->kmeansrestarts = restarts;
    s->kmeansmaxits = maxits;
}


/*************************************************************************
This function sets k-means  initialization  algorithm.  Several  different
algorithms can be chosen, including k-means++.

INPUT PARAMETERS:
    S       -   clusterizer state, initialized by ClusterizerCreate()
    InitAlgo-   initialization algorithm:
                * 0  automatic selection ( different  versions  of  ALGLIB
                     may select different algorithms)
                * 1  random initialization
                * 2  k-means++ initialization  (best  quality  of  initial
                     centers, but long  non-parallelizable  initialization
                     phase with bad cache locality)
                * 3  "fast-greedy"  algorithm  with  efficient,  easy   to
                     parallelize initialization. Quality of initial centers
                     is  somewhat  worse  than  that  of  k-means++.  This
                     algorithm is a default one in the current version  of
                     ALGLIB.
                *-1  "debug" algorithm which always selects first  K  rows
                     of dataset; this algorithm is used for debug purposes
                     only. Do not use it in the industrial code!

  -- ALGLIB --
     Copyright 21.01.2015 by Bochkanov Sergey
*************************************************************************/
void clusterizersetkmeansinit(clusterizerstate* s,
     ae_int_t initalgo,
     ae_state *_state)
{


    ae_assert(initalgo>=-1&&initalgo<=3, "ClusterizerSetKMeansInit: InitAlgo is incorrect", _state);
    s->kmeansinitalgo = initalgo;
}


/*************************************************************************
This function performs agglomerative hierarchical clustering

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Agglomerative  hierarchical  clustering  algorithm  has  two   phases:
  ! distance matrix calculation  and  clustering  itself. Only first phase
  ! (distance matrix calculation) is accelerated by Intel MKL  and  multi-
  ! threading. Thus, acceleration is significant only for  medium or high-
  ! dimensional problems.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS:
    S       -   clusterizer state, initialized by ClusterizerCreate()

OUTPUT PARAMETERS:
    Rep     -   clustering results; see description of AHCReport
                structure for more information.

NOTE 1: hierarchical clustering algorithms require large amounts of memory.
        In particular, this implementation needs  sizeof(double)*NPoints^2
        bytes, which are used to store distance matrix. In  case  we  work
        with user-supplied matrix, this amount is multiplied by 2 (we have
        to store original matrix and to work with its copy).
        
        For example, problem with 10000 points  would require 800M of RAM,
        even when working in a 1-dimensional space.

  -- ALGLIB --
     Copyright 10.07.2012 by Bochkanov Sergey
*************************************************************************/
void clusterizerrunahc(clusterizerstate* s,
     ahcreport* rep,
     ae_state *_state)
{
    ae_int_t npoints;
    ae_int_t nfeatures;

    _ahcreport_clear(rep);

    npoints = s->npoints;
    nfeatures = s->nfeatures;
    
    /*
     * Fill Rep.NPoints, quick exit when NPoints<=1
     */
    rep->npoints = npoints;
    if( npoints==0 )
    {
        ae_vector_set_length(&rep->p, 0, _state);
        ae_matrix_set_length(&rep->z, 0, 0, _state);
        ae_matrix_set_length(&rep->pz, 0, 0, _state);
        ae_matrix_set_length(&rep->pm, 0, 0, _state);
        ae_vector_set_length(&rep->mergedist, 0, _state);
        rep->terminationtype = 1;
        return;
    }
    if( npoints==1 )
    {
        ae_vector_set_length(&rep->p, 1, _state);
        ae_matrix_set_length(&rep->z, 0, 0, _state);
        ae_matrix_set_length(&rep->pz, 0, 0, _state);
        ae_matrix_set_length(&rep->pm, 0, 0, _state);
        ae_vector_set_length(&rep->mergedist, 0, _state);
        rep->p.ptr.p_int[0] = 0;
        rep->terminationtype = 1;
        return;
    }
    
    /*
     * More than one point
     */
    if( s->disttype==-1 )
    {
        
        /*
         * Run clusterizer with user-supplied distance matrix
         */
        clustering_clusterizerrunahcinternal(s, &s->d, rep, _state);
        return;
    }
    else
    {
        
        /*
         * Check combination of AHC algo and distance type
         */
        if( s->ahcalgo==4&&s->disttype!=2 )
        {
            rep->terminationtype = -5;
            return;
        }
        
        /*
         * Build distance matrix D.
         */
        clusterizergetdistancesbuf(&s->distbuf, &s->xy, npoints, nfeatures, s->disttype, &s->tmpd, _state);
        
        /*
         * Run clusterizer
         */
        clustering_clusterizerrunahcinternal(s, &s->tmpd, rep, _state);
        return;
    }
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_clusterizerrunahc(clusterizerstate* s,
    ahcreport* rep, ae_state *_state)
{
    clusterizerrunahc(s,rep, _state);
}


/*************************************************************************
This function performs clustering by k-means++ algorithm.

You may change algorithm properties by calling:
* ClusterizerSetKMeansLimits() to change number of restarts or iterations
* ClusterizerSetKMeansInit() to change initialization algorithm

By  default,  one  restart  and  unlimited number of iterations are  used.
Initialization algorithm is chosen automatically.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes  two important  improvements  of
  ! this function:
  ! * multicore support (can be used from C# and C++)
  ! * access to high-performance C++ core (actual for C# users)
  !
  ! K-means clustering  algorithm has two  phases:  selection  of  initial
  ! centers  and  clustering  itself.  ALGLIB  parallelizes  both  phases.
  ! Parallel version is optimized for the following  scenario:  medium  or
  ! high-dimensional problem (20 or more dimensions) with large number  of
  ! points and clusters. However, some speed-up can be obtained even  when
  ! assumptions above are violated.
  !
  ! As for native-vs-managed comparison, working with native  core  brings
  ! 30-40% improvement in speed over pure C# version of ALGLIB.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS:
    S       -   clusterizer state, initialized by ClusterizerCreate()
    K       -   number of clusters, K>=0.
                K  can  be  zero only when algorithm is called  for  empty
                dataset,  in   this   case   completion  code  is  set  to
                success (+1).
                If  K=0  and  dataset  size  is  non-zero,  we   can   not
                meaningfully assign points to some center  (there  are  no
                centers because K=0) and  return  -3  as  completion  code
                (failure).

OUTPUT PARAMETERS:
    Rep     -   clustering results; see description of KMeansReport
                structure for more information.

NOTE 1: k-means  clustering  can  be  performed  only  for  datasets  with
        Euclidean  distance  function.  Algorithm  will  return   negative
        completion code in Rep.TerminationType in case dataset  was  added
        to clusterizer with DistType other than Euclidean (or dataset  was
        specified by distance matrix instead of explicitly given points).

  -- ALGLIB --
     Copyright 10.07.2012 by Bochkanov Sergey
*************************************************************************/
void clusterizerrunkmeans(clusterizerstate* s,
     ae_int_t k,
     kmeansreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix dummy;

    ae_frame_make(_state, &_frame_block);
    _kmeansreport_clear(rep);
    ae_matrix_init(&dummy, 0, 0, DT_REAL, _state);

    ae_assert(k>=0, "ClusterizerRunKMeans: K<0", _state);
    
    /*
     * Incorrect distance type
     */
    if( s->disttype!=2 )
    {
        rep->npoints = s->npoints;
        rep->terminationtype = -5;
        rep->k = k;
        rep->iterationscount = 0;
        rep->energy = 0.0;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * K>NPoints or (K=0 and NPoints>0)
     */
    if( k>s->npoints||(k==0&&s->npoints>0) )
    {
        rep->npoints = s->npoints;
        rep->terminationtype = -3;
        rep->k = k;
        rep->iterationscount = 0;
        rep->energy = 0.0;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * No points
     */
    if( s->npoints==0 )
    {
        rep->npoints = 0;
        rep->terminationtype = 1;
        rep->k = k;
        rep->iterationscount = 0;
        rep->energy = 0.0;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Normal case:
     * 1<=K<=NPoints, Euclidean distance 
     */
    rep->npoints = s->npoints;
    rep->nfeatures = s->nfeatures;
    rep->k = k;
    rep->npoints = s->npoints;
    rep->nfeatures = s->nfeatures;
    kmeansgenerateinternal(&s->xy, s->npoints, s->nfeatures, k, s->kmeansinitalgo, s->kmeansmaxits, s->kmeansrestarts, s->kmeansdbgnoits, &rep->terminationtype, &rep->iterationscount, &dummy, ae_false, &rep->c, ae_true, &rep->cidx, &rep->energy, &s->kmeanstmp, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_clusterizerrunkmeans(clusterizerstate* s,
    ae_int_t k,
    kmeansreport* rep, ae_state *_state)
{
    clusterizerrunkmeans(s,k,rep, _state);
}


/*************************************************************************
This function returns distance matrix for dataset

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multicore support
  !
  ! Agglomerative  hierarchical  clustering  algorithm  has  two   phases:
  ! distance matrix calculation  and  clustering  itself. Only first phase
  ! (distance matrix calculation) is accelerated by Intel MKL  and  multi-
  ! threading. Thus, acceleration is significant only for  medium or high-
  ! dimensional problems.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS:
    XY      -   array[NPoints,NFeatures], dataset
    NPoints -   number of points, >=0
    NFeatures-  number of features, >=1
    DistType-   distance function:
                *  0    Chebyshev distance  (L-inf norm)
                *  1    city block distance (L1 norm)
                *  2    Euclidean distance  (L2 norm, non-squared)
                * 10    Pearson correlation:
                        dist(a,b) = 1-corr(a,b)
                * 11    Absolute Pearson correlation:
                        dist(a,b) = 1-|corr(a,b)|
                * 12    Uncentered Pearson correlation (cosine of the angle):
                        dist(a,b) = a'*b/(|a|*|b|)
                * 13    Absolute uncentered Pearson correlation
                        dist(a,b) = |a'*b|/(|a|*|b|)
                * 20    Spearman rank correlation:
                        dist(a,b) = 1-rankcorr(a,b)
                * 21    Absolute Spearman rank correlation
                        dist(a,b) = 1-|rankcorr(a,b)|

OUTPUT PARAMETERS:
    D       -   array[NPoints,NPoints], distance matrix
                (full matrix is returned, with lower and upper triangles)

NOTE:  different distance functions have different performance penalty:
       * Euclidean or Pearson correlation distances are the fastest ones
       * Spearman correlation distance function is a bit slower
       * city block and Chebyshev distances are order of magnitude slower
       
       The reason behing difference in performance is that correlation-based
       distance functions are computed using optimized linear algebra kernels,
       while Chebyshev and city block distance functions are computed using
       simple nested loops with two branches at each iteration.

  -- ALGLIB --
     Copyright 10.07.2012 by Bochkanov Sergey
*************************************************************************/
void clusterizergetdistances(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nfeatures,
     ae_int_t disttype,
     /* Real    */ ae_matrix* d,
     ae_state *_state)
{
    ae_frame _frame_block;
    apbuffers buf;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_clear(d);
    _apbuffers_init(&buf, _state);

    ae_assert(nfeatures>=1, "ClusterizerGetDistances: NFeatures<1", _state);
    ae_assert(npoints>=0, "ClusterizerGetDistances: NPoints<1", _state);
    ae_assert((((((((disttype==0||disttype==1)||disttype==2)||disttype==10)||disttype==11)||disttype==12)||disttype==13)||disttype==20)||disttype==21, "ClusterizerGetDistances: incorrect DistType", _state);
    ae_assert(xy->rows>=npoints, "ClusterizerGetDistances: Rows(XY)<NPoints", _state);
    ae_assert(xy->cols>=nfeatures, "ClusterizerGetDistances: Cols(XY)<NFeatures", _state);
    ae_assert(apservisfinitematrix(xy, npoints, nfeatures, _state), "ClusterizerGetDistances: XY contains NAN/INF", _state);
    clusterizergetdistancesbuf(&buf, xy, npoints, nfeatures, disttype, d, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_clusterizergetdistances(/* Real    */ ae_matrix* xy,
    ae_int_t npoints,
    ae_int_t nfeatures,
    ae_int_t disttype,
    /* Real    */ ae_matrix* d, ae_state *_state)
{
    clusterizergetdistances(xy,npoints,nfeatures,disttype,d, _state);
}


/*************************************************************************
Buffered version  of  ClusterizerGetDistances()  which  reuses  previously
allocated space.

  -- ALGLIB --
     Copyright 29.05.2015 by Bochkanov Sergey
*************************************************************************/
void clusterizergetdistancesbuf(apbuffers* buf,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nfeatures,
     ae_int_t disttype,
     /* Real    */ ae_matrix* d,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;
    double vv;
    double vr;


    ae_assert(nfeatures>=1, "ClusterizerGetDistancesBuf: NFeatures<1", _state);
    ae_assert(npoints>=0, "ClusterizerGetDistancesBuf: NPoints<1", _state);
    ae_assert((((((((disttype==0||disttype==1)||disttype==2)||disttype==10)||disttype==11)||disttype==12)||disttype==13)||disttype==20)||disttype==21, "ClusterizerGetDistancesBuf: incorrect DistType", _state);
    ae_assert(xy->rows>=npoints, "ClusterizerGetDistancesBuf: Rows(XY)<NPoints", _state);
    ae_assert(xy->cols>=nfeatures, "ClusterizerGetDistancesBuf: Cols(XY)<NFeatures", _state);
    ae_assert(apservisfinitematrix(xy, npoints, nfeatures, _state), "ClusterizerGetDistancesBuf: XY contains NAN/INF", _state);
    
    /*
     * Quick exit
     */
    if( npoints==0 )
    {
        return;
    }
    if( npoints==1 )
    {
        rmatrixsetlengthatleast(d, 1, 1, _state);
        d->ptr.pp_double[0][0] = (double)(0);
        return;
    }
    
    /*
     * Build distance matrix D.
     */
    if( disttype==0||disttype==1 )
    {
        
        /*
         * Chebyshev or city-block distances:
         * * recursively calculate upper triangle (with main diagonal)
         * * copy it to the bottom part of the matrix
         */
        rmatrixsetlengthatleast(d, npoints, npoints, _state);
        clustering_evaluatedistancematrixrec(xy, nfeatures, disttype, d, 0, npoints, 0, npoints, _state);
        rmatrixenforcesymmetricity(d, npoints, ae_true, _state);
        return;
    }
    if( disttype==2 )
    {
        
        /*
         * Euclidean distance
         *
         * NOTE: parallelization is done within RMatrixSYRK
         */
        rmatrixsetlengthatleast(d, npoints, npoints, _state);
        rmatrixsetlengthatleast(&buf->rm0, npoints, nfeatures, _state);
        rvectorsetlengthatleast(&buf->ra1, nfeatures, _state);
        rvectorsetlengthatleast(&buf->ra0, npoints, _state);
        for(j=0; j<=nfeatures-1; j++)
        {
            buf->ra1.ptr.p_double[j] = 0.0;
        }
        v = (double)1/(double)npoints;
        for(i=0; i<=npoints-1; i++)
        {
            ae_v_addd(&buf->ra1.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nfeatures-1), v);
        }
        for(i=0; i<=npoints-1; i++)
        {
            ae_v_move(&buf->rm0.ptr.pp_double[i][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nfeatures-1));
            ae_v_sub(&buf->rm0.ptr.pp_double[i][0], 1, &buf->ra1.ptr.p_double[0], 1, ae_v_len(0,nfeatures-1));
        }
        rmatrixsyrk(npoints, nfeatures, 1.0, &buf->rm0, 0, 0, 0, 0.0, d, 0, 0, ae_true, _state);
        for(i=0; i<=npoints-1; i++)
        {
            buf->ra0.ptr.p_double[i] = d->ptr.pp_double[i][i];
        }
        for(i=0; i<=npoints-1; i++)
        {
            d->ptr.pp_double[i][i] = 0.0;
            for(j=i+1; j<=npoints-1; j++)
            {
                v = ae_sqrt(ae_maxreal(buf->ra0.ptr.p_double[i]+buf->ra0.ptr.p_double[j]-2*d->ptr.pp_double[i][j], 0.0, _state), _state);
                d->ptr.pp_double[i][j] = v;
            }
        }
        rmatrixenforcesymmetricity(d, npoints, ae_true, _state);
        return;
    }
    if( disttype==10||disttype==11 )
    {
        
        /*
         * Absolute/nonabsolute Pearson correlation distance
         *
         * NOTE: parallelization is done within PearsonCorrM, which calls RMatrixSYRK internally
         */
        rmatrixsetlengthatleast(d, npoints, npoints, _state);
        rvectorsetlengthatleast(&buf->ra0, npoints, _state);
        rmatrixsetlengthatleast(&buf->rm0, npoints, nfeatures, _state);
        for(i=0; i<=npoints-1; i++)
        {
            v = 0.0;
            for(j=0; j<=nfeatures-1; j++)
            {
                v = v+xy->ptr.pp_double[i][j];
            }
            v = v/nfeatures;
            for(j=0; j<=nfeatures-1; j++)
            {
                buf->rm0.ptr.pp_double[i][j] = xy->ptr.pp_double[i][j]-v;
            }
        }
        rmatrixsyrk(npoints, nfeatures, 1.0, &buf->rm0, 0, 0, 0, 0.0, d, 0, 0, ae_true, _state);
        for(i=0; i<=npoints-1; i++)
        {
            buf->ra0.ptr.p_double[i] = d->ptr.pp_double[i][i];
        }
        for(i=0; i<=npoints-1; i++)
        {
            d->ptr.pp_double[i][i] = 0.0;
            for(j=i+1; j<=npoints-1; j++)
            {
                v = d->ptr.pp_double[i][j]/ae_sqrt(buf->ra0.ptr.p_double[i]*buf->ra0.ptr.p_double[j], _state);
                if( disttype==10 )
                {
                    v = 1-v;
                }
                else
                {
                    v = 1-ae_fabs(v, _state);
                }
                v = ae_maxreal(v, 0.0, _state);
                d->ptr.pp_double[i][j] = v;
            }
        }
        rmatrixenforcesymmetricity(d, npoints, ae_true, _state);
        return;
    }
    if( disttype==12||disttype==13 )
    {
        
        /*
         * Absolute/nonabsolute uncentered Pearson correlation distance
         *
         * NOTE: parallelization is done within RMatrixSYRK
         */
        rmatrixsetlengthatleast(d, npoints, npoints, _state);
        rvectorsetlengthatleast(&buf->ra0, npoints, _state);
        rmatrixsyrk(npoints, nfeatures, 1.0, xy, 0, 0, 0, 0.0, d, 0, 0, ae_true, _state);
        for(i=0; i<=npoints-1; i++)
        {
            buf->ra0.ptr.p_double[i] = d->ptr.pp_double[i][i];
        }
        for(i=0; i<=npoints-1; i++)
        {
            d->ptr.pp_double[i][i] = 0.0;
            for(j=i+1; j<=npoints-1; j++)
            {
                v = d->ptr.pp_double[i][j]/ae_sqrt(buf->ra0.ptr.p_double[i]*buf->ra0.ptr.p_double[j], _state);
                if( disttype==13 )
                {
                    v = ae_fabs(v, _state);
                }
                v = ae_minreal(v, 1.0, _state);
                d->ptr.pp_double[i][j] = 1-v;
            }
        }
        rmatrixenforcesymmetricity(d, npoints, ae_true, _state);
        return;
    }
    if( disttype==20||disttype==21 )
    {
        
        /*
         * Spearman rank correlation
         *
         * NOTE: parallelization of correlation matrix is done within
         *       PearsonCorrM, which calls RMatrixSYRK internally
         */
        rmatrixsetlengthatleast(d, npoints, npoints, _state);
        rvectorsetlengthatleast(&buf->ra0, npoints, _state);
        rmatrixsetlengthatleast(&buf->rm0, npoints, nfeatures, _state);
        rmatrixcopy(npoints, nfeatures, xy, 0, 0, &buf->rm0, 0, 0, _state);
        rankdatacentered(&buf->rm0, npoints, nfeatures, _state);
        rmatrixsyrk(npoints, nfeatures, 1.0, &buf->rm0, 0, 0, 0, 0.0, d, 0, 0, ae_true, _state);
        for(i=0; i<=npoints-1; i++)
        {
            if( ae_fp_greater(d->ptr.pp_double[i][i],(double)(0)) )
            {
                buf->ra0.ptr.p_double[i] = 1/ae_sqrt(d->ptr.pp_double[i][i], _state);
            }
            else
            {
                buf->ra0.ptr.p_double[i] = 0.0;
            }
        }
        for(i=0; i<=npoints-1; i++)
        {
            v = buf->ra0.ptr.p_double[i];
            d->ptr.pp_double[i][i] = 0.0;
            for(j=i+1; j<=npoints-1; j++)
            {
                vv = d->ptr.pp_double[i][j]*v*buf->ra0.ptr.p_double[j];
                if( disttype==20 )
                {
                    vr = 1-vv;
                }
                else
                {
                    vr = 1-ae_fabs(vv, _state);
                }
                if( ae_fp_less(vr,(double)(0)) )
                {
                    vr = 0.0;
                }
                d->ptr.pp_double[i][j] = vr;
            }
        }
        rmatrixenforcesymmetricity(d, npoints, ae_true, _state);
        return;
    }
    ae_assert(ae_false, "Assertion failed", _state);
}


/*************************************************************************
This function takes as input clusterization report Rep,  desired  clusters
count K, and builds top K clusters from hierarchical clusterization  tree.
It returns assignment of points to clusters (array of cluster indexes).

INPUT PARAMETERS:
    Rep     -   report from ClusterizerRunAHC() performed on XY
    K       -   desired number of clusters, 1<=K<=NPoints.
                K can be zero only when NPoints=0.

OUTPUT PARAMETERS:
    CIdx    -   array[NPoints], I-th element contains cluster index  (from
                0 to K-1) for I-th point of the dataset.
    CZ      -   array[K]. This array allows  to  convert  cluster  indexes
                returned by this function to indexes used by  Rep.Z.  J-th
                cluster returned by this function corresponds to  CZ[J]-th
                cluster stored in Rep.Z/PZ/PM.
                It is guaranteed that CZ[I]<CZ[I+1].

NOTE: K clusters built by this subroutine are assumed to have no hierarchy.
      Although  they  were  obtained  by  manipulation with top K nodes of
      dendrogram  (i.e.  hierarchical  decomposition  of  dataset),   this
      function does not return information about hierarchy.  Each  of  the
      clusters stand on its own.
      
NOTE: Cluster indexes returned by this function  does  not  correspond  to
      indexes returned in Rep.Z/PZ/PM. Either you work  with  hierarchical
      representation of the dataset (dendrogram), or you work with  "flat"
      representation returned by this function.  Each  of  representations
      has its own clusters indexing system (former uses [0, 2*NPoints-2]),
      while latter uses [0..K-1]), although  it  is  possible  to  perform
      conversion from one system to another by means of CZ array, returned
      by this function, which allows you to convert indexes stored in CIdx
      to the numeration system used by Rep.Z.
      
NOTE: this subroutine is optimized for moderate values of K. Say, for  K=5
      it will perform many times faster than  for  K=100.  Its  worst-case
      performance is O(N*K), although in average case  it  perform  better
      (up to O(N*log(K))).

  -- ALGLIB --
     Copyright 10.07.2012 by Bochkanov Sergey
*************************************************************************/
void clusterizergetkclusters(ahcreport* rep,
     ae_int_t k,
     /* Integer */ ae_vector* cidx,
     /* Integer */ ae_vector* cz,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t mergeidx;
    ae_int_t i0;
    ae_int_t i1;
    ae_int_t t;
    ae_vector presentclusters;
    ae_vector clusterindexes;
    ae_vector clustersizes;
    ae_vector tmpidx;
    ae_int_t npoints;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(cidx);
    ae_vector_clear(cz);
    ae_vector_init(&presentclusters, 0, DT_BOOL, _state);
    ae_vector_init(&clusterindexes, 0, DT_INT, _state);
    ae_vector_init(&clustersizes, 0, DT_INT, _state);
    ae_vector_init(&tmpidx, 0, DT_INT, _state);

    npoints = rep->npoints;
    ae_assert(npoints>=0, "ClusterizerGetKClusters: internal error in Rep integrity", _state);
    ae_assert(k>=0, "ClusterizerGetKClusters: K<=0", _state);
    ae_assert(k<=npoints, "ClusterizerGetKClusters: K>NPoints", _state);
    ae_assert(k>0||npoints==0, "ClusterizerGetKClusters: K<=0", _state);
    ae_assert(npoints==rep->npoints, "ClusterizerGetKClusters: NPoints<>Rep.NPoints", _state);
    
    /*
     * Quick exit
     */
    if( npoints==0 )
    {
        ae_frame_leave(_state);
        return;
    }
    if( npoints==1 )
    {
        ae_vector_set_length(cz, 1, _state);
        ae_vector_set_length(cidx, 1, _state);
        cz->ptr.p_int[0] = 0;
        cidx->ptr.p_int[0] = 0;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Replay merges, from top to bottom,
     * keep track of clusters being present at the moment
     */
    ae_vector_set_length(&presentclusters, 2*npoints-1, _state);
    ae_vector_set_length(&tmpidx, npoints, _state);
    for(i=0; i<=2*npoints-3; i++)
    {
        presentclusters.ptr.p_bool[i] = ae_false;
    }
    presentclusters.ptr.p_bool[2*npoints-2] = ae_true;
    for(i=0; i<=npoints-1; i++)
    {
        tmpidx.ptr.p_int[i] = 2*npoints-2;
    }
    for(mergeidx=npoints-2; mergeidx>=npoints-k; mergeidx--)
    {
        
        /*
         * Update information about clusters being present at the moment
         */
        presentclusters.ptr.p_bool[npoints+mergeidx] = ae_false;
        presentclusters.ptr.p_bool[rep->z.ptr.pp_int[mergeidx][0]] = ae_true;
        presentclusters.ptr.p_bool[rep->z.ptr.pp_int[mergeidx][1]] = ae_true;
        
        /*
         * Update TmpIdx according to the current state of the dataset
         *
         * NOTE: TmpIdx contains cluster indexes from [0..2*NPoints-2];
         *       we will convert them to [0..K-1] later.
         */
        i0 = rep->pm.ptr.pp_int[mergeidx][0];
        i1 = rep->pm.ptr.pp_int[mergeidx][1];
        t = rep->z.ptr.pp_int[mergeidx][0];
        for(i=i0; i<=i1; i++)
        {
            tmpidx.ptr.p_int[i] = t;
        }
        i0 = rep->pm.ptr.pp_int[mergeidx][2];
        i1 = rep->pm.ptr.pp_int[mergeidx][3];
        t = rep->z.ptr.pp_int[mergeidx][1];
        for(i=i0; i<=i1; i++)
        {
            tmpidx.ptr.p_int[i] = t;
        }
    }
    
    /*
     * Fill CZ - array which allows us to convert cluster indexes
     * from one system to another.
     */
    ae_vector_set_length(cz, k, _state);
    ae_vector_set_length(&clusterindexes, 2*npoints-1, _state);
    t = 0;
    for(i=0; i<=2*npoints-2; i++)
    {
        if( presentclusters.ptr.p_bool[i] )
        {
            cz->ptr.p_int[t] = i;
            clusterindexes.ptr.p_int[i] = t;
            t = t+1;
        }
    }
    ae_assert(t==k, "ClusterizerGetKClusters: internal error", _state);
    
    /*
     * Convert indexes stored in CIdx
     */
    ae_vector_set_length(cidx, npoints, _state);
    for(i=0; i<=npoints-1; i++)
    {
        cidx->ptr.p_int[i] = clusterindexes.ptr.p_int[tmpidx.ptr.p_int[rep->p.ptr.p_int[i]]];
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This  function  accepts  AHC  report  Rep,  desired  minimum  intercluster
distance and returns top clusters from  hierarchical  clusterization  tree
which are separated by distance R or HIGHER.

It returns assignment of points to clusters (array of cluster indexes).

There is one more function with similar name - ClusterizerSeparatedByCorr,
which returns clusters with intercluster correlation equal to R  or  LOWER
(note: higher for distance, lower for correlation).

INPUT PARAMETERS:
    Rep     -   report from ClusterizerRunAHC() performed on XY
    R       -   desired minimum intercluster distance, R>=0

OUTPUT PARAMETERS:
    K       -   number of clusters, 1<=K<=NPoints
    CIdx    -   array[NPoints], I-th element contains cluster index  (from
                0 to K-1) for I-th point of the dataset.
    CZ      -   array[K]. This array allows  to  convert  cluster  indexes
                returned by this function to indexes used by  Rep.Z.  J-th
                cluster returned by this function corresponds to  CZ[J]-th
                cluster stored in Rep.Z/PZ/PM.
                It is guaranteed that CZ[I]<CZ[I+1].

NOTE: K clusters built by this subroutine are assumed to have no hierarchy.
      Although  they  were  obtained  by  manipulation with top K nodes of
      dendrogram  (i.e.  hierarchical  decomposition  of  dataset),   this
      function does not return information about hierarchy.  Each  of  the
      clusters stand on its own.
      
NOTE: Cluster indexes returned by this function  does  not  correspond  to
      indexes returned in Rep.Z/PZ/PM. Either you work  with  hierarchical
      representation of the dataset (dendrogram), or you work with  "flat"
      representation returned by this function.  Each  of  representations
      has its own clusters indexing system (former uses [0, 2*NPoints-2]),
      while latter uses [0..K-1]), although  it  is  possible  to  perform
      conversion from one system to another by means of CZ array, returned
      by this function, which allows you to convert indexes stored in CIdx
      to the numeration system used by Rep.Z.
      
NOTE: this subroutine is optimized for moderate values of K. Say, for  K=5
      it will perform many times faster than  for  K=100.  Its  worst-case
      performance is O(N*K), although in average case  it  perform  better
      (up to O(N*log(K))).

  -- ALGLIB --
     Copyright 10.07.2012 by Bochkanov Sergey
*************************************************************************/
void clusterizerseparatedbydist(ahcreport* rep,
     double r,
     ae_int_t* k,
     /* Integer */ ae_vector* cidx,
     /* Integer */ ae_vector* cz,
     ae_state *_state)
{

    *k = 0;
    ae_vector_clear(cidx);
    ae_vector_clear(cz);

    ae_assert(ae_isfinite(r, _state)&&ae_fp_greater_eq(r,(double)(0)), "ClusterizerSeparatedByDist: R is infinite or less than 0", _state);
    *k = 1;
    while(*k<rep->npoints&&ae_fp_greater_eq(rep->mergedist.ptr.p_double[rep->npoints-1-(*k)],r))
    {
        *k = *k+1;
    }
    clusterizergetkclusters(rep, *k, cidx, cz, _state);
}


/*************************************************************************
This  function  accepts  AHC  report  Rep,  desired  maximum  intercluster
correlation and returns top clusters from hierarchical clusterization tree
which are separated by correlation R or LOWER.

It returns assignment of points to clusters (array of cluster indexes).

There is one more function with similar name - ClusterizerSeparatedByDist,
which returns clusters with intercluster distance equal  to  R  or  HIGHER
(note: higher for distance, lower for correlation).

INPUT PARAMETERS:
    Rep     -   report from ClusterizerRunAHC() performed on XY
    R       -   desired maximum intercluster correlation, -1<=R<=+1

OUTPUT PARAMETERS:
    K       -   number of clusters, 1<=K<=NPoints
    CIdx    -   array[NPoints], I-th element contains cluster index  (from
                0 to K-1) for I-th point of the dataset.
    CZ      -   array[K]. This array allows  to  convert  cluster  indexes
                returned by this function to indexes used by  Rep.Z.  J-th
                cluster returned by this function corresponds to  CZ[J]-th
                cluster stored in Rep.Z/PZ/PM.
                It is guaranteed that CZ[I]<CZ[I+1].

NOTE: K clusters built by this subroutine are assumed to have no hierarchy.
      Although  they  were  obtained  by  manipulation with top K nodes of
      dendrogram  (i.e.  hierarchical  decomposition  of  dataset),   this
      function does not return information about hierarchy.  Each  of  the
      clusters stand on its own.
      
NOTE: Cluster indexes returned by this function  does  not  correspond  to
      indexes returned in Rep.Z/PZ/PM. Either you work  with  hierarchical
      representation of the dataset (dendrogram), or you work with  "flat"
      representation returned by this function.  Each  of  representations
      has its own clusters indexing system (former uses [0, 2*NPoints-2]),
      while latter uses [0..K-1]), although  it  is  possible  to  perform
      conversion from one system to another by means of CZ array, returned
      by this function, which allows you to convert indexes stored in CIdx
      to the numeration system used by Rep.Z.
      
NOTE: this subroutine is optimized for moderate values of K. Say, for  K=5
      it will perform many times faster than  for  K=100.  Its  worst-case
      performance is O(N*K), although in average case  it  perform  better
      (up to O(N*log(K))).

  -- ALGLIB --
     Copyright 10.07.2012 by Bochkanov Sergey
*************************************************************************/
void clusterizerseparatedbycorr(ahcreport* rep,
     double r,
     ae_int_t* k,
     /* Integer */ ae_vector* cidx,
     /* Integer */ ae_vector* cz,
     ae_state *_state)
{

    *k = 0;
    ae_vector_clear(cidx);
    ae_vector_clear(cz);

    ae_assert((ae_isfinite(r, _state)&&ae_fp_greater_eq(r,(double)(-1)))&&ae_fp_less_eq(r,(double)(1)), "ClusterizerSeparatedByCorr: R is infinite or less than 0", _state);
    *k = 1;
    while(*k<rep->npoints&&ae_fp_greater_eq(rep->mergedist.ptr.p_double[rep->npoints-1-(*k)],1-r))
    {
        *k = *k+1;
    }
    clusterizergetkclusters(rep, *k, cidx, cz, _state);
}


/*************************************************************************
K-means++ initialization

INPUT PARAMETERS:
    Buf         -   special reusable structure which stores previously allocated
                    memory, intended to avoid memory fragmentation when solving
                    multiple subsequent problems. Must be initialized prior to
                    usage.

OUTPUT PARAMETERS:
    Buf         -   initialized structure

  -- ALGLIB --
     Copyright 24.07.2015 by Bochkanov Sergey
*************************************************************************/
void kmeansinitbuf(kmeansbuffers* buf, ae_state *_state)
{
    ae_frame _frame_block;
    apbuffers updateseed;

    ae_frame_make(_state, &_frame_block);
    _apbuffers_init(&updateseed, _state);

    ae_shared_pool_set_seed(&buf->updatepool, &updateseed, sizeof(updateseed), _apbuffers_init, _apbuffers_init_copy, _apbuffers_destroy, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
K-means++ clusterization

INPUT PARAMETERS:
    XY          -   dataset, array [0..NPoints-1,0..NVars-1].
    NPoints     -   dataset size, NPoints>=K
    NVars       -   number of variables, NVars>=1
    K           -   desired number of clusters, K>=1
    InitAlgo    -   initialization algorithm:
                    * 0 - automatic selection of best algorithm
                    * 1 - random selection of centers
                    * 2 - k-means++
                    * 3 - fast-greedy init
                    *-1 - first K rows of dataset are used
                          (special debug algorithm)
    MaxIts      -   iterations limit or zero for no limit
    Restarts    -   number of restarts, Restarts>=1
    KMeansDbgNoIts- debug flag; if set, Lloyd's iteration is not performed,
                    only initialization phase.
    Buf         -   special reusable structure which stores previously allocated
                    memory, intended to avoid memory fragmentation when solving
                    multiple subsequent problems:
                    * MUST BE INITIALIZED WITH KMeansInitBuffers() CALL BEFORE
                      FIRST PASS TO THIS FUNCTION!
                    * subsequent passes must be made without re-initialization

OUTPUT PARAMETERS:
    Info        -   return code:
                    * -3, if task is degenerate (number of distinct points is
                          less than K)
                    * -1, if incorrect NPoints/NFeatures/K/Restarts was passed
                    *  1, if subroutine finished successfully
    IterationsCount- actual number of iterations performed by clusterizer
    CCol        -   array[0..NVars-1,0..K-1].matrix whose columns store
                    cluster's centers
    NeedCCol    -   True in case caller requires to store result in CCol
    CRow        -   array[0..K-1,0..NVars-1], same as CCol, but centers are
                    stored in rows
    NeedCRow    -   True in case caller requires to store result in CCol
    XYC         -   array[NPoints], which contains cluster indexes
    Energy      -   merit function of clusterization

  -- ALGLIB --
     Copyright 21.03.2009 by Bochkanov Sergey
*************************************************************************/
void kmeansgenerateinternal(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t k,
     ae_int_t initalgo,
     ae_int_t maxits,
     ae_int_t restarts,
     ae_bool kmeansdbgnoits,
     ae_int_t* info,
     ae_int_t* iterationscount,
     /* Real    */ ae_matrix* ccol,
     ae_bool needccol,
     /* Real    */ ae_matrix* crow,
     ae_bool needcrow,
     /* Integer */ ae_vector* xyc,
     double* energy,
     kmeansbuffers* buf,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t i1;
    double e;
    double eprev;
    double v;
    double vv;
    ae_bool waschanges;
    ae_bool zerosizeclusters;
    ae_int_t pass;
    ae_int_t itcnt;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    *iterationscount = 0;
    ae_matrix_clear(ccol);
    ae_matrix_clear(crow);
    ae_vector_clear(xyc);
    *energy = 0;
    _hqrndstate_init(&rs, _state);

    
    /*
     * Test parameters
     */
    if( ((npoints<k||nvars<1)||k<1)||restarts<1 )
    {
        *info = -1;
        *iterationscount = 0;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * TODO: special case K=1
     * TODO: special case K=NPoints
     */
    *info = 1;
    *iterationscount = 0;
    
    /*
     * Multiple passes of k-means++ algorithm
     */
    ae_vector_set_length(xyc, npoints, _state);
    rmatrixsetlengthatleast(&buf->ct, k, nvars, _state);
    rmatrixsetlengthatleast(&buf->ctbest, k, nvars, _state);
    ivectorsetlengthatleast(&buf->xycprev, npoints, _state);
    ivectorsetlengthatleast(&buf->xycbest, npoints, _state);
    rvectorsetlengthatleast(&buf->d2, npoints, _state);
    ivectorsetlengthatleast(&buf->csizes, k, _state);
    *energy = ae_maxrealnumber;
    hqrndrandomize(&rs, _state);
    for(pass=1; pass<=restarts; pass++)
    {
        
        /*
         * Select initial centers.
         *
         * Note that for performance reasons centers are stored in ROWS of CT, not
         * in columns. We'll transpose CT in the end and store it in the C.
         *
         * Also note that SelectInitialCenters() may return degenerate set of centers
         * (some of them have no corresponding points in dataset, some are non-distinct).
         * Algorithm below is robust enough to deal with such set.
         */
        clustering_selectinitialcenters(xy, npoints, nvars, initalgo, k, &buf->ct, &buf->initbuf, &buf->updatepool, _state);
        
        /*
         * Lloyd's iteration
         */
        if( !kmeansdbgnoits )
        {
            
            /*
             * Perform iteration as usual, in normal mode
             */
            for(i=0; i<=npoints-1; i++)
            {
                xyc->ptr.p_int[i] = -1;
            }
            eprev = ae_maxrealnumber;
            e = ae_maxrealnumber;
            itcnt = 0;
            while(maxits==0||itcnt<maxits)
            {
                
                /*
                 * Update iteration counter
                 */
                itcnt = itcnt+1;
                inc(iterationscount, _state);
                
                /*
                 * Call KMeansUpdateDistances(), fill XYC with center numbers,
                 * D2 with center distances.
                 */
                for(i=0; i<=npoints-1; i++)
                {
                    buf->xycprev.ptr.p_int[i] = xyc->ptr.p_int[i];
                }
                kmeansupdatedistances(xy, 0, npoints, nvars, &buf->ct, 0, k, xyc, &buf->d2, &buf->updatepool, _state);
                waschanges = ae_false;
                for(i=0; i<=npoints-1; i++)
                {
                    waschanges = waschanges||xyc->ptr.p_int[i]!=buf->xycprev.ptr.p_int[i];
                }
                
                /*
                 * Update centers
                 */
                for(j=0; j<=k-1; j++)
                {
                    buf->csizes.ptr.p_int[j] = 0;
                }
                for(i=0; i<=k-1; i++)
                {
                    for(j=0; j<=nvars-1; j++)
                    {
                        buf->ct.ptr.pp_double[i][j] = (double)(0);
                    }
                }
                for(i=0; i<=npoints-1; i++)
                {
                    buf->csizes.ptr.p_int[xyc->ptr.p_int[i]] = buf->csizes.ptr.p_int[xyc->ptr.p_int[i]]+1;
                    ae_v_add(&buf->ct.ptr.pp_double[xyc->ptr.p_int[i]][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
                }
                zerosizeclusters = ae_false;
                for(j=0; j<=k-1; j++)
                {
                    if( buf->csizes.ptr.p_int[j]!=0 )
                    {
                        v = (double)1/(double)buf->csizes.ptr.p_int[j];
                        ae_v_muld(&buf->ct.ptr.pp_double[j][0], 1, ae_v_len(0,nvars-1), v);
                    }
                    zerosizeclusters = zerosizeclusters||buf->csizes.ptr.p_int[j]==0;
                }
                if( zerosizeclusters )
                {
                    
                    /*
                     * Some clusters have zero size - rare, but possible.
                     * We'll choose new centers for such clusters using k-means++ rule
                     * and restart algorithm
                     */
                    if( !clustering_fixcenters(xy, npoints, nvars, &buf->ct, k, &buf->initbuf, &buf->updatepool, _state) )
                    {
                        *info = -3;
                        ae_frame_leave(_state);
                        return;
                    }
                    continue;
                }
                
                /*
                 * Stop if one of two conditions is met:
                 * 1. nothing has changed during iteration
                 * 2. energy function increased after recalculation on new centers
                 */
                e = (double)(0);
                for(i=0; i<=npoints-1; i++)
                {
                    v = 0.0;
                    i1 = xyc->ptr.p_int[i];
                    for(j=0; j<=nvars-1; j++)
                    {
                        vv = xy->ptr.pp_double[i][j]-buf->ct.ptr.pp_double[i1][j];
                        v = v+vv*vv;
                    }
                    e = e+v;
                }
                if( !waschanges||ae_fp_greater_eq(e,eprev) )
                {
                    break;
                }
                
                /*
                 * Update EPrev
                 */
                eprev = e;
            }
        }
        else
        {
            
            /*
             * Debug mode: no Lloyd's iteration.
             * We just calculate potential E.
             */
            kmeansupdatedistances(xy, 0, npoints, nvars, &buf->ct, 0, k, xyc, &buf->d2, &buf->updatepool, _state);
            e = (double)(0);
            for(i=0; i<=npoints-1; i++)
            {
                e = e+buf->d2.ptr.p_double[i];
            }
        }
        
        /*
         * Compare E with best centers found so far
         */
        if( ae_fp_less(e,*energy) )
        {
            
            /*
             * store partition.
             */
            *energy = e;
            copymatrix(&buf->ct, 0, k-1, 0, nvars-1, &buf->ctbest, 0, k-1, 0, nvars-1, _state);
            for(i=0; i<=npoints-1; i++)
            {
                buf->xycbest.ptr.p_int[i] = xyc->ptr.p_int[i];
            }
        }
    }
    
    /*
     * Copy and transpose
     */
    if( needccol )
    {
        ae_matrix_set_length(ccol, nvars, k, _state);
        copyandtranspose(&buf->ctbest, 0, k-1, 0, nvars-1, ccol, 0, nvars-1, 0, k-1, _state);
    }
    if( needcrow )
    {
        ae_matrix_set_length(crow, k, nvars, _state);
        rmatrixcopy(k, nvars, &buf->ctbest, 0, 0, crow, 0, 0, _state);
    }
    for(i=0; i<=npoints-1; i++)
    {
        xyc->ptr.p_int[i] = buf->xycbest.ptr.p_int[i];
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This procedure recalculates distances from points to centers  and  assigns
each point to closest center.

INPUT PARAMETERS:
    XY          -   dataset, array [0..NPoints-1,0..NVars-1].
    Idx0,Idx1   -   define range of dataset [Idx0,Idx1) to process;
                    right boundary is not included.
    NVars       -   number of variables, NVars>=1
    CT          -   matrix of centers, centers are stored in rows
    CIdx0,CIdx1 -   define range of centers [CIdx0,CIdx1) to process;
                    right boundary is not included.
    XYC         -   preallocated output buffer, 
    XYDist2     -   preallocated output buffer
    Tmp         -   temporary buffer, automatically reallocated if needed
    BufferPool  -   shared pool seeded with instance of APBuffers structure
                    (seed instance can be unitialized). It is recommended
                    to use this pool only with KMeansUpdateDistances()
                    function.

OUTPUT PARAMETERS:
    XYC         -   new assignment of points to centers are stored
                    in [Idx0,Idx1)
    XYDist2     -   squared distances from points to their centers are
                    stored in [Idx0,Idx1)

  -- ALGLIB --
     Copyright 21.01.2015 by Bochkanov Sergey
*************************************************************************/
void kmeansupdatedistances(/* Real    */ ae_matrix* xy,
     ae_int_t idx0,
     ae_int_t idx1,
     ae_int_t nvars,
     /* Real    */ ae_matrix* ct,
     ae_int_t cidx0,
     ae_int_t cidx1,
     /* Integer */ ae_vector* xyc,
     /* Real    */ ae_vector* xydist2,
     ae_shared_pool* bufferpool,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t i0;
    ae_int_t i1;
    ae_int_t j;
    ae_int_t cclosest;
    double dclosest;
    double vv;
    apbuffers *buf;
    ae_smart_ptr _buf;
    double rcomplexity;
    ae_int_t task0;
    ae_int_t task1;
    ae_int_t pblkcnt;
    ae_int_t cblkcnt;
    ae_int_t vblkcnt;
    ae_int_t pblk;
    ae_int_t cblk;
    ae_int_t vblk;
    ae_int_t p0;
    ae_int_t p1;
    ae_int_t c0;
    ae_int_t c1;
    ae_int_t v0;
    ae_int_t v1;
    double v00;
    double v01;
    double v10;
    double v11;
    double vp0;
    double vp1;
    double vc0;
    double vc1;
    ae_int_t pcnt;
    ae_int_t pcntpadded;
    ae_int_t ccnt;
    ae_int_t ccntpadded;
    ae_int_t offs0;
    ae_int_t offs00;
    ae_int_t offs01;
    ae_int_t offs10;
    ae_int_t offs11;
    ae_int_t vcnt;
    ae_int_t stride;

    ae_frame_make(_state, &_frame_block);
    ae_smart_ptr_init(&_buf, (void**)&buf, _state);

    
    /*
     * Quick exit for special cases
     */
    if( idx1<=idx0 )
    {
        ae_frame_leave(_state);
        return;
    }
    if( cidx1<=cidx0 )
    {
        ae_frame_leave(_state);
        return;
    }
    if( nvars<=0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Try to recursively divide/process dataset
     *
     * NOTE: real arithmetics is used to avoid integer overflow on large problem sizes
     */
    rcomplexity = (double)(idx1-idx0);
    rcomplexity = rcomplexity*(cidx1-cidx0);
    rcomplexity = rcomplexity*nvars;
    if( ((ae_fp_greater_eq(rcomplexity,clustering_parallelcomplexity)&&idx1-idx0>=2*clustering_kmeansblocksize)&&nvars>=clustering_kmeansparalleldim)&&cidx1-cidx0>=clustering_kmeansparallelk )
    {
        splitlength(idx1-idx0, clustering_kmeansblocksize, &task0, &task1, _state);
        kmeansupdatedistances(xy, idx0, idx0+task0, nvars, ct, cidx0, cidx1, xyc, xydist2, bufferpool, _state);
        kmeansupdatedistances(xy, idx0+task0, idx1, nvars, ct, cidx0, cidx1, xyc, xydist2, bufferpool, _state);
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Dataset chunk is selected.
     * 
     * Process it with blocked algorithm:
     * * iterate over points, process them in KMeansBlockSize-ed chunks
     * * for each chunk of dataset, iterate over centers, process them in KMeansBlockSize-ed chunks
     * * for each chunk of dataset/centerset, iterate over variables, process them in KMeansBlockSize-ed chunks
     */
    ae_assert(clustering_kmeansblocksize%2==0, "KMeansUpdateDistances: internal error", _state);
    ae_shared_pool_retrieve(bufferpool, &_buf, _state);
    rvectorsetlengthatleast(&buf->ra0, clustering_kmeansblocksize*clustering_kmeansblocksize, _state);
    rvectorsetlengthatleast(&buf->ra1, clustering_kmeansblocksize*clustering_kmeansblocksize, _state);
    rvectorsetlengthatleast(&buf->ra2, clustering_kmeansblocksize*clustering_kmeansblocksize, _state);
    rvectorsetlengthatleast(&buf->ra3, clustering_kmeansblocksize, _state);
    ivectorsetlengthatleast(&buf->ia3, clustering_kmeansblocksize, _state);
    pblkcnt = chunkscount(idx1-idx0, clustering_kmeansblocksize, _state);
    cblkcnt = chunkscount(cidx1-cidx0, clustering_kmeansblocksize, _state);
    vblkcnt = chunkscount(nvars, clustering_kmeansblocksize, _state);
    for(pblk=0; pblk<=pblkcnt-1; pblk++)
    {
        
        /*
         * Process PBlk-th chunk of dataset.
         */
        p0 = idx0+pblk*clustering_kmeansblocksize;
        p1 = ae_minint(p0+clustering_kmeansblocksize, idx1, _state);
        
        /*
         * Prepare RA3[]/IA3[] for storage of best distances and best cluster numbers.
         */
        for(i=0; i<=clustering_kmeansblocksize-1; i++)
        {
            buf->ra3.ptr.p_double[i] = ae_maxrealnumber;
            buf->ia3.ptr.p_int[i] = -1;
        }
        
        /*
         * Iterare over chunks of centerset.
         */
        for(cblk=0; cblk<=cblkcnt-1; cblk++)
        {
            
            /*
             * Process CBlk-th chunk of centerset
             */
            c0 = cidx0+cblk*clustering_kmeansblocksize;
            c1 = ae_minint(c0+clustering_kmeansblocksize, cidx1, _state);
            
            /*
             * At this point we have to calculate a set of pairwise distances
             * between points [P0,P1) and centers [C0,C1) and select best center
             * for each point. It can also be done with blocked algorithm
             * (blocking for variables).
             *
             * Following arrays are used:
             * * RA0[] - matrix of distances, padded by zeros for even size,
             *           rows are stored with stride KMeansBlockSize.
             * * RA1[] - matrix of points (variables corresponding to current
             *           block are extracted), padded by zeros for even size,
             *           rows are stored with stride KMeansBlockSize.
             * * RA2[] - matrix of centers (variables corresponding to current
             *           block are extracted), padded by zeros for even size,
             *           rows are stored with stride KMeansBlockSize.
             *
             */
            pcnt = p1-p0;
            pcntpadded = pcnt+pcnt%2;
            ccnt = c1-c0;
            ccntpadded = ccnt+ccnt%2;
            stride = clustering_kmeansblocksize;
            ae_assert(pcntpadded<=clustering_kmeansblocksize, "KMeansUpdateDistances: integrity error", _state);
            ae_assert(ccntpadded<=clustering_kmeansblocksize, "KMeansUpdateDistances: integrity error", _state);
            for(i=0; i<=pcntpadded-1; i++)
            {
                for(j=0; j<=ccntpadded-1; j++)
                {
                    buf->ra0.ptr.p_double[i*stride+j] = 0.0;
                }
            }
            for(vblk=0; vblk<=vblkcnt-1; vblk++)
            {
                
                /*
                 * Fetch VBlk-th block of variables to arrays RA1 (points) and RA2 (centers).
                 * Pad points and centers with zeros.
                 */
                v0 = vblk*clustering_kmeansblocksize;
                v1 = ae_minint(v0+clustering_kmeansblocksize, nvars, _state);
                vcnt = v1-v0;
                for(i=0; i<=pcnt-1; i++)
                {
                    for(j=0; j<=vcnt-1; j++)
                    {
                        buf->ra1.ptr.p_double[i*stride+j] = xy->ptr.pp_double[p0+i][v0+j];
                    }
                }
                for(i=pcnt; i<=pcntpadded-1; i++)
                {
                    for(j=0; j<=vcnt-1; j++)
                    {
                        buf->ra1.ptr.p_double[i*stride+j] = 0.0;
                    }
                }
                for(i=0; i<=ccnt-1; i++)
                {
                    for(j=0; j<=vcnt-1; j++)
                    {
                        buf->ra2.ptr.p_double[i*stride+j] = ct->ptr.pp_double[c0+i][v0+j];
                    }
                }
                for(i=ccnt; i<=ccntpadded-1; i++)
                {
                    for(j=0; j<=vcnt-1; j++)
                    {
                        buf->ra2.ptr.p_double[i*stride+j] = 0.0;
                    }
                }
                
                /*
                 * Update distance matrix with sums-of-squared-differences of RA1 and RA2
                 */
                i0 = 0;
                while(i0<pcntpadded)
                {
                    i1 = 0;
                    while(i1<ccntpadded)
                    {
                        offs0 = i0*stride+i1;
                        v00 = buf->ra0.ptr.p_double[offs0];
                        v01 = buf->ra0.ptr.p_double[offs0+1];
                        v10 = buf->ra0.ptr.p_double[offs0+stride];
                        v11 = buf->ra0.ptr.p_double[offs0+stride+1];
                        offs00 = i0*stride;
                        offs01 = offs00+stride;
                        offs10 = i1*stride;
                        offs11 = offs10+stride;
                        for(j=0; j<=vcnt-1; j++)
                        {
                            vp0 = buf->ra1.ptr.p_double[offs00+j];
                            vp1 = buf->ra1.ptr.p_double[offs01+j];
                            vc0 = buf->ra2.ptr.p_double[offs10+j];
                            vc1 = buf->ra2.ptr.p_double[offs11+j];
                            vv = vp0-vc0;
                            v00 = v00+vv*vv;
                            vv = vp0-vc1;
                            v01 = v01+vv*vv;
                            vv = vp1-vc0;
                            v10 = v10+vv*vv;
                            vv = vp1-vc1;
                            v11 = v11+vv*vv;
                        }
                        offs0 = i0*stride+i1;
                        buf->ra0.ptr.p_double[offs0] = v00;
                        buf->ra0.ptr.p_double[offs0+1] = v01;
                        buf->ra0.ptr.p_double[offs0+stride] = v10;
                        buf->ra0.ptr.p_double[offs0+stride+1] = v11;
                        i1 = i1+2;
                    }
                    i0 = i0+2;
                }
            }
            for(i=0; i<=pcnt-1; i++)
            {
                cclosest = buf->ia3.ptr.p_int[i];
                dclosest = buf->ra3.ptr.p_double[i];
                for(j=0; j<=ccnt-1; j++)
                {
                    if( ae_fp_less(buf->ra0.ptr.p_double[i*stride+j],dclosest) )
                    {
                        dclosest = buf->ra0.ptr.p_double[i*stride+j];
                        cclosest = c0+j;
                    }
                }
                buf->ia3.ptr.p_int[i] = cclosest;
                buf->ra3.ptr.p_double[i] = dclosest;
            }
        }
        
        /*
         * Store best centers to XYC[]
         */
        for(i=p0; i<=p1-1; i++)
        {
            xyc->ptr.p_int[i] = buf->ia3.ptr.p_int[i-p0];
            xydist2->ptr.p_double[i] = buf->ra3.ptr.p_double[i-p0];
        }
    }
    ae_shared_pool_recycle(bufferpool, &_buf, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
This function selects initial centers according to specified initialization
algorithm.

IMPORTANT: this function provides no  guarantees  regarding  selection  of
           DIFFERENT  centers.  Centers  returned  by  this  function  may
           include duplicates (say, when random sampling is  used). It  is
           also possible that some centers are empty.
           Algorithm which uses this function must be able to deal with it.
           Say, you may want to use FixCenters() in order to fix empty centers.

INPUT PARAMETERS:
    XY          -   dataset, array [0..NPoints-1,0..NVars-1].
    NPoints     -   points count
    NVars       -   number of variables, NVars>=1
    InitAlgo    -   initialization algorithm:
                    * 0 - automatic selection of best algorithm
                    * 1 - random selection
                    * 2 - k-means++
                    * 3 - fast-greedy init
                    *-1 - first K rows of dataset are used (debug algorithm)
    K           -   number of centers, K>=1
    CT          -   possibly preallocated output buffer, resized if needed
    InitBuf     -   internal buffer, possibly unitialized instance of
                    APBuffers. It is recommended to use this instance only
                    with SelectInitialCenters() and FixCenters() functions,
                    because these functions may allocate really large storage.
    UpdatePool  -   shared pool seeded with instance of APBuffers structure
                    (seed instance can be unitialized). Used internally with
                    KMeansUpdateDistances() function. It is recommended
                    to use this pool ONLY with KMeansUpdateDistances()
                    function.

OUTPUT PARAMETERS:
    CT          -   set of K clusters, one per row
    
RESULT:
    True on success, False on failure (impossible to create K independent clusters)

  -- ALGLIB --
     Copyright 21.01.2015 by Bochkanov Sergey
*************************************************************************/
static void clustering_selectinitialcenters(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t initalgo,
     ae_int_t k,
     /* Real    */ ae_matrix* ct,
     apbuffers* initbuf,
     ae_shared_pool* updatepool,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t cidx;
    ae_int_t i;
    ae_int_t j;
    double v;
    double vv;
    double s;
    ae_int_t lastnz;
    ae_int_t ptidx;
    ae_int_t samplesize;
    ae_int_t samplescntnew;
    ae_int_t samplescntall;
    double samplescale;
    hqrndstate rs;

    ae_frame_make(_state, &_frame_block);
    _hqrndstate_init(&rs, _state);

    hqrndrandomize(&rs, _state);
    
    /*
     * Check parameters
     */
    ae_assert(npoints>0, "SelectInitialCenters: internal error", _state);
    ae_assert(nvars>0, "SelectInitialCenters: internal error", _state);
    ae_assert(k>0, "SelectInitialCenters: internal error", _state);
    if( initalgo==0 )
    {
        initalgo = 3;
    }
    rmatrixsetlengthatleast(ct, k, nvars, _state);
    
    /*
     * Random initialization
     */
    if( initalgo==-1 )
    {
        for(i=0; i<=k-1; i++)
        {
            ae_v_move(&ct->ptr.pp_double[i][0], 1, &xy->ptr.pp_double[i%npoints][0], 1, ae_v_len(0,nvars-1));
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Random initialization
     */
    if( initalgo==1 )
    {
        for(i=0; i<=k-1; i++)
        {
            j = hqrnduniformi(&rs, npoints, _state);
            ae_v_move(&ct->ptr.pp_double[i][0], 1, &xy->ptr.pp_double[j][0], 1, ae_v_len(0,nvars-1));
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * k-means++ initialization
     */
    if( initalgo==2 )
    {
        
        /*
         * Prepare distances array.
         * Select initial center at random.
         */
        rvectorsetlengthatleast(&initbuf->ra0, npoints, _state);
        for(i=0; i<=npoints-1; i++)
        {
            initbuf->ra0.ptr.p_double[i] = ae_maxrealnumber;
        }
        ptidx = hqrnduniformi(&rs, npoints, _state);
        ae_v_move(&ct->ptr.pp_double[0][0], 1, &xy->ptr.pp_double[ptidx][0], 1, ae_v_len(0,nvars-1));
        
        /*
         * For each newly added center repeat:
         * * reevaluate distances from points to best centers
         * * sample points with probability dependent on distance
         * * add new center
         */
        for(cidx=0; cidx<=k-2; cidx++)
        {
            
            /*
             * Reevaluate distances
             */
            s = 0.0;
            for(i=0; i<=npoints-1; i++)
            {
                v = 0.0;
                for(j=0; j<=nvars-1; j++)
                {
                    vv = xy->ptr.pp_double[i][j]-ct->ptr.pp_double[cidx][j];
                    v = v+vv*vv;
                }
                if( ae_fp_less(v,initbuf->ra0.ptr.p_double[i]) )
                {
                    initbuf->ra0.ptr.p_double[i] = v;
                }
                s = s+initbuf->ra0.ptr.p_double[i];
            }
            
            /*
             * If all distances are zero, it means that we can not find enough
             * distinct points. In this case we just select non-distinct center
             * at random and continue iterations. This issue will be handled
             * later in the FixCenters() function.
             */
            if( ae_fp_eq(s,0.0) )
            {
                ptidx = hqrnduniformi(&rs, npoints, _state);
                ae_v_move(&ct->ptr.pp_double[cidx+1][0], 1, &xy->ptr.pp_double[ptidx][0], 1, ae_v_len(0,nvars-1));
                continue;
            }
            
            /*
             * Select point as center using its distance.
             * We also handle situation when because of rounding errors
             * no point was selected - in this case, last non-zero one
             * will be used.
             */
            v = hqrnduniformr(&rs, _state);
            vv = 0.0;
            lastnz = -1;
            ptidx = -1;
            for(i=0; i<=npoints-1; i++)
            {
                if( ae_fp_eq(initbuf->ra0.ptr.p_double[i],0.0) )
                {
                    continue;
                }
                lastnz = i;
                vv = vv+initbuf->ra0.ptr.p_double[i];
                if( ae_fp_less_eq(v,vv/s) )
                {
                    ptidx = i;
                    break;
                }
            }
            ae_assert(lastnz>=0, "SelectInitialCenters: integrity error", _state);
            if( ptidx<0 )
            {
                ptidx = lastnz;
            }
            ae_v_move(&ct->ptr.pp_double[cidx+1][0], 1, &xy->ptr.pp_double[ptidx][0], 1, ae_v_len(0,nvars-1));
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * "Fast-greedy" algorithm based on "Scalable k-means++".
     *
     * We perform several rounds, within each round we sample about 0.5*K points
     * (not exactly 0.5*K) until we have 2*K points sampled. Before each round
     * we calculate distances from dataset points to closest points sampled so far.
     * We sample dataset points independently using distance xtimes 0.5*K divided by total
     * as probability (similar to k-means++, but each point is sampled independently;
     * after each round we have roughtly 0.5*K points added to sample).
     *
     * After sampling is done, we run "greedy" version of k-means++ on this subsample
     * which selects most distant point on every round.
     */
    if( initalgo==3 )
    {
        
        /*
         * Prepare arrays.
         * Select initial center at random, add it to "new" part of sample,
         * which is stored at the beginning of the array
         */
        samplesize = 2*k;
        samplescale = 0.5*k;
        rmatrixsetlengthatleast(&initbuf->rm0, samplesize, nvars, _state);
        ptidx = hqrnduniformi(&rs, npoints, _state);
        ae_v_move(&initbuf->rm0.ptr.pp_double[0][0], 1, &xy->ptr.pp_double[ptidx][0], 1, ae_v_len(0,nvars-1));
        samplescntnew = 1;
        samplescntall = 1;
        rvectorsetlengthatleast(&initbuf->ra0, npoints, _state);
        rvectorsetlengthatleast(&initbuf->ra1, npoints, _state);
        ivectorsetlengthatleast(&initbuf->ia1, npoints, _state);
        for(i=0; i<=npoints-1; i++)
        {
            initbuf->ra0.ptr.p_double[i] = ae_maxrealnumber;
        }
        
        /*
         * Repeat until samples count is 2*K
         */
        while(samplescntall<samplesize)
        {
            
            /*
             * Evaluate distances from points to NEW centers, store to RA1.
             * Reset counter of "new" centers.
             */
            kmeansupdatedistances(xy, 0, npoints, nvars, &initbuf->rm0, samplescntall-samplescntnew, samplescntall, &initbuf->ia1, &initbuf->ra1, updatepool, _state);
            samplescntnew = 0;
            
            /*
             * Merge new distances with old ones.
             * Calculate sum of distances, if sum is exactly zero - fill sample
             * by randomly selected points and terminate.
             */
            s = 0.0;
            for(i=0; i<=npoints-1; i++)
            {
                initbuf->ra0.ptr.p_double[i] = ae_minreal(initbuf->ra0.ptr.p_double[i], initbuf->ra1.ptr.p_double[i], _state);
                s = s+initbuf->ra0.ptr.p_double[i];
            }
            if( ae_fp_eq(s,0.0) )
            {
                while(samplescntall<samplesize)
                {
                    ptidx = hqrnduniformi(&rs, npoints, _state);
                    ae_v_move(&initbuf->rm0.ptr.pp_double[samplescntall][0], 1, &xy->ptr.pp_double[ptidx][0], 1, ae_v_len(0,nvars-1));
                    inc(&samplescntall, _state);
                    inc(&samplescntnew, _state);
                }
                break;
            }
            
            /*
             * Sample points independently.
             */
            for(i=0; i<=npoints-1; i++)
            {
                if( samplescntall==samplesize )
                {
                    break;
                }
                if( ae_fp_eq(initbuf->ra0.ptr.p_double[i],0.0) )
                {
                    continue;
                }
                if( ae_fp_less_eq(hqrnduniformr(&rs, _state),samplescale*initbuf->ra0.ptr.p_double[i]/s) )
                {
                    ae_v_move(&initbuf->rm0.ptr.pp_double[samplescntall][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
                    inc(&samplescntall, _state);
                    inc(&samplescntnew, _state);
                }
            }
        }
        
        /*
         * Run greedy version of k-means on sampled points
         */
        rvectorsetlengthatleast(&initbuf->ra0, samplescntall, _state);
        for(i=0; i<=samplescntall-1; i++)
        {
            initbuf->ra0.ptr.p_double[i] = ae_maxrealnumber;
        }
        ptidx = hqrnduniformi(&rs, samplescntall, _state);
        ae_v_move(&ct->ptr.pp_double[0][0], 1, &initbuf->rm0.ptr.pp_double[ptidx][0], 1, ae_v_len(0,nvars-1));
        for(cidx=0; cidx<=k-2; cidx++)
        {
            
            /*
             * Reevaluate distances
             */
            for(i=0; i<=samplescntall-1; i++)
            {
                v = 0.0;
                for(j=0; j<=nvars-1; j++)
                {
                    vv = initbuf->rm0.ptr.pp_double[i][j]-ct->ptr.pp_double[cidx][j];
                    v = v+vv*vv;
                }
                if( ae_fp_less(v,initbuf->ra0.ptr.p_double[i]) )
                {
                    initbuf->ra0.ptr.p_double[i] = v;
                }
            }
            
            /*
             * Select point as center in greedy manner - most distant
             * point is selected.
             */
            ptidx = 0;
            for(i=0; i<=samplescntall-1; i++)
            {
                if( ae_fp_greater(initbuf->ra0.ptr.p_double[i],initbuf->ra0.ptr.p_double[ptidx]) )
                {
                    ptidx = i;
                }
            }
            ae_v_move(&ct->ptr.pp_double[cidx+1][0], 1, &initbuf->rm0.ptr.pp_double[ptidx][0], 1, ae_v_len(0,nvars-1));
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Internal error
     */
    ae_assert(ae_false, "SelectInitialCenters: internal error", _state);
    ae_frame_leave(_state);
}


/*************************************************************************
This function "fixes" centers, i.e. replaces ones which have  no  neighbor
points by new centers which have at least one neighbor. If it is impossible
to fix centers (not enough distinct points in the dataset), this function
returns False.

INPUT PARAMETERS:
    XY          -   dataset, array [0..NPoints-1,0..NVars-1].
    NPoints     -   points count, >=1
    NVars       -   number of variables, NVars>=1
    CT          -   centers
    K           -   number of centers, K>=1
    InitBuf     -   internal buffer, possibly unitialized instance of
                    APBuffers. It is recommended to use this instance only
                    with SelectInitialCenters() and FixCenters() functions,
                    because these functions may allocate really large storage.
    UpdatePool  -   shared pool seeded with instance of APBuffers structure
                    (seed instance can be unitialized). Used internally with
                    KMeansUpdateDistances() function. It is recommended
                    to use this pool ONLY with KMeansUpdateDistances()
                    function.

OUTPUT PARAMETERS:
    CT          -   set of K centers, one per row
    
RESULT:
    True on success, False on failure (impossible to create K independent clusters)

  -- ALGLIB --
     Copyright 21.01.2015 by Bochkanov Sergey
*************************************************************************/
static ae_bool clustering_fixcenters(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     /* Real    */ ae_matrix* ct,
     ae_int_t k,
     apbuffers* initbuf,
     ae_shared_pool* updatepool,
     ae_state *_state)
{
    ae_int_t fixiteration;
    ae_int_t centertofix;
    ae_int_t i;
    ae_int_t j;
    ae_int_t pdistant;
    double ddistant;
    double v;
    ae_bool result;


    ae_assert(npoints>=1, "FixCenters: internal error", _state);
    ae_assert(nvars>=1, "FixCenters: internal error", _state);
    ae_assert(k>=1, "FixCenters: internal error", _state);
    
    /*
     * Calculate distances from points to best centers (RA0)
     * and best center indexes (IA0)
     */
    ivectorsetlengthatleast(&initbuf->ia0, npoints, _state);
    rvectorsetlengthatleast(&initbuf->ra0, npoints, _state);
    kmeansupdatedistances(xy, 0, npoints, nvars, ct, 0, k, &initbuf->ia0, &initbuf->ra0, updatepool, _state);
    
    /*
     * Repeat loop:
     * * find first center which has no corresponding point
     * * set it to the most distant (from the rest of the centerset) point
     * * recalculate distances, update IA0/RA0
     * * repeat
     *
     * Loop is repeated for at most 2*K iterations. It is stopped once we have
     * no "empty" clusters.
     */
    bvectorsetlengthatleast(&initbuf->ba0, k, _state);
    for(fixiteration=0; fixiteration<=2*k; fixiteration++)
    {
        
        /*
         * Select center to fix (one which is not mentioned in IA0),
         * terminate if there is no such center.
         * BA0[] stores True for centers which have at least one point.
         */
        for(i=0; i<=k-1; i++)
        {
            initbuf->ba0.ptr.p_bool[i] = ae_false;
        }
        for(i=0; i<=npoints-1; i++)
        {
            initbuf->ba0.ptr.p_bool[initbuf->ia0.ptr.p_int[i]] = ae_true;
        }
        centertofix = -1;
        for(i=0; i<=k-1; i++)
        {
            if( !initbuf->ba0.ptr.p_bool[i] )
            {
                centertofix = i;
                break;
            }
        }
        if( centertofix<0 )
        {
            result = ae_true;
            return result;
        }
        
        /*
         * Replace center to fix by the most distant point.
         * Update IA0/RA0
         */
        pdistant = 0;
        ddistant = initbuf->ra0.ptr.p_double[pdistant];
        for(i=0; i<=npoints-1; i++)
        {
            if( ae_fp_greater(initbuf->ra0.ptr.p_double[i],ddistant) )
            {
                ddistant = initbuf->ra0.ptr.p_double[i];
                pdistant = i;
            }
        }
        if( ae_fp_eq(ddistant,0.0) )
        {
            break;
        }
        ae_v_move(&ct->ptr.pp_double[centertofix][0], 1, &xy->ptr.pp_double[pdistant][0], 1, ae_v_len(0,nvars-1));
        for(i=0; i<=npoints-1; i++)
        {
            v = 0.0;
            for(j=0; j<=nvars-1; j++)
            {
                v = v+ae_sqr(xy->ptr.pp_double[i][j]-ct->ptr.pp_double[centertofix][j], _state);
            }
            if( ae_fp_less(v,initbuf->ra0.ptr.p_double[i]) )
            {
                initbuf->ra0.ptr.p_double[i] = v;
                initbuf->ia0.ptr.p_int[i] = centertofix;
            }
        }
    }
    result = ae_false;
    return result;
}


/*************************************************************************
This  function  performs  agglomerative  hierarchical  clustering    using
precomputed  distance  matrix.  Internal  function,  should  not be called
directly.

INPUT PARAMETERS:
    S       -   clusterizer state, initialized by ClusterizerCreate()
    D       -   distance matrix, array[S.NFeatures,S.NFeatures]
                Contents of the matrix is destroyed during
                algorithm operation.

OUTPUT PARAMETERS:
    Rep     -   clustering results; see description of AHCReport
                structure for more information.

  -- ALGLIB --
     Copyright 10.07.2012 by Bochkanov Sergey
*************************************************************************/
static void clustering_clusterizerrunahcinternal(clusterizerstate* s,
     /* Real    */ ae_matrix* d,
     ahcreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    ae_int_t mergeidx;
    ae_int_t c0;
    ae_int_t c1;
    ae_int_t s0;
    ae_int_t s1;
    ae_int_t ar;
    ae_int_t br;
    ae_int_t npoints;
    ae_vector cidx;
    ae_vector csizes;
    ae_vector nnidx;
    ae_matrix cinfo;
    ae_int_t n0;
    ae_int_t n1;
    ae_int_t ni;
    double d01;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&cidx, 0, DT_INT, _state);
    ae_vector_init(&csizes, 0, DT_INT, _state);
    ae_vector_init(&nnidx, 0, DT_INT, _state);
    ae_matrix_init(&cinfo, 0, 0, DT_INT, _state);

    npoints = s->npoints;
    
    /*
     * Fill Rep.NPoints, quick exit when NPoints<=1
     */
    rep->npoints = npoints;
    if( npoints==0 )
    {
        ae_vector_set_length(&rep->p, 0, _state);
        ae_matrix_set_length(&rep->z, 0, 0, _state);
        ae_matrix_set_length(&rep->pz, 0, 0, _state);
        ae_matrix_set_length(&rep->pm, 0, 0, _state);
        ae_vector_set_length(&rep->mergedist, 0, _state);
        rep->terminationtype = 1;
        ae_frame_leave(_state);
        return;
    }
    if( npoints==1 )
    {
        ae_vector_set_length(&rep->p, 1, _state);
        ae_matrix_set_length(&rep->z, 0, 0, _state);
        ae_matrix_set_length(&rep->pz, 0, 0, _state);
        ae_matrix_set_length(&rep->pm, 0, 0, _state);
        ae_vector_set_length(&rep->mergedist, 0, _state);
        rep->p.ptr.p_int[0] = 0;
        rep->terminationtype = 1;
        ae_frame_leave(_state);
        return;
    }
    ae_matrix_set_length(&rep->z, npoints-1, 2, _state);
    ae_vector_set_length(&rep->mergedist, npoints-1, _state);
    rep->terminationtype = 1;
    
    /*
     * Build list of nearest neighbors
     */
    ae_vector_set_length(&nnidx, npoints, _state);
    for(i=0; i<=npoints-1; i++)
    {
        
        /*
         * Calculate index of the nearest neighbor
         */
        k = -1;
        v = ae_maxrealnumber;
        for(j=0; j<=npoints-1; j++)
        {
            if( j!=i&&ae_fp_less(d->ptr.pp_double[i][j],v) )
            {
                k = j;
                v = d->ptr.pp_double[i][j];
            }
        }
        ae_assert(ae_fp_less(v,ae_maxrealnumber), "ClusterizerRunAHC: internal error", _state);
        nnidx.ptr.p_int[i] = k;
    }
    
    /*
     * For AHCAlgo=4 (Ward's method) replace distances by their squares times 0.5
     */
    if( s->ahcalgo==4 )
    {
        for(i=0; i<=npoints-1; i++)
        {
            for(j=0; j<=npoints-1; j++)
            {
                d->ptr.pp_double[i][j] = 0.5*d->ptr.pp_double[i][j]*d->ptr.pp_double[i][j];
            }
        }
    }
    
    /*
     * Distance matrix is built, perform merges.
     *
     * NOTE 1: CIdx is array[NPoints] which maps rows/columns of the
     *         distance matrix D to indexes of clusters. Values of CIdx
     *         from [0,NPoints) denote single-point clusters, and values
     *         from [NPoints,2*NPoints-1) denote ones obtained by merging
     *         smaller clusters. Negative calues correspond to absent clusters.
     *
     *         Initially it contains [0...NPoints-1], after each merge
     *         one element of CIdx (one with index C0) is replaced by
     *         NPoints+MergeIdx, and another one with index C1 is
     *         rewritten by -1.
     * 
     * NOTE 2: CSizes is array[NPoints] which stores sizes of clusters.
     *         
     */
    ae_vector_set_length(&cidx, npoints, _state);
    ae_vector_set_length(&csizes, npoints, _state);
    for(i=0; i<=npoints-1; i++)
    {
        cidx.ptr.p_int[i] = i;
        csizes.ptr.p_int[i] = 1;
    }
    for(mergeidx=0; mergeidx<=npoints-2; mergeidx++)
    {
        
        /*
         * Select pair of clusters (C0,C1) with CIdx[C0]<CIdx[C1] to merge.
         */
        c0 = -1;
        c1 = -1;
        d01 = ae_maxrealnumber;
        for(i=0; i<=npoints-1; i++)
        {
            if( cidx.ptr.p_int[i]>=0 )
            {
                if( ae_fp_less(d->ptr.pp_double[i][nnidx.ptr.p_int[i]],d01) )
                {
                    c0 = i;
                    c1 = nnidx.ptr.p_int[i];
                    d01 = d->ptr.pp_double[i][nnidx.ptr.p_int[i]];
                }
            }
        }
        ae_assert(ae_fp_less(d01,ae_maxrealnumber), "ClusterizerRunAHC: internal error", _state);
        if( cidx.ptr.p_int[c0]>cidx.ptr.p_int[c1] )
        {
            i = c1;
            c1 = c0;
            c0 = i;
        }
        
        /*
         * Fill one row of Rep.Z and one element of Rep.MergeDist
         */
        rep->z.ptr.pp_int[mergeidx][0] = cidx.ptr.p_int[c0];
        rep->z.ptr.pp_int[mergeidx][1] = cidx.ptr.p_int[c1];
        rep->mergedist.ptr.p_double[mergeidx] = d01;
        
        /*
         * Update distance matrix:
         * * row/column C0 are updated by distances to the new cluster
         * * row/column C1 are considered empty (we can fill them by zeros,
         *   but do not want to spend time - we just ignore them)
         *
         * NOTE: it is important to update distance matrix BEFORE CIdx/CSizes
         *       are updated.
         */
        ae_assert((((s->ahcalgo==0||s->ahcalgo==1)||s->ahcalgo==2)||s->ahcalgo==3)||s->ahcalgo==4, "ClusterizerRunAHC: internal error", _state);
        for(i=0; i<=npoints-1; i++)
        {
            if( i!=c0&&i!=c1 )
            {
                n0 = csizes.ptr.p_int[c0];
                n1 = csizes.ptr.p_int[c1];
                ni = csizes.ptr.p_int[i];
                if( s->ahcalgo==0 )
                {
                    d->ptr.pp_double[i][c0] = ae_maxreal(d->ptr.pp_double[i][c0], d->ptr.pp_double[i][c1], _state);
                }
                if( s->ahcalgo==1 )
                {
                    d->ptr.pp_double[i][c0] = ae_minreal(d->ptr.pp_double[i][c0], d->ptr.pp_double[i][c1], _state);
                }
                if( s->ahcalgo==2 )
                {
                    d->ptr.pp_double[i][c0] = (csizes.ptr.p_int[c0]*d->ptr.pp_double[i][c0]+csizes.ptr.p_int[c1]*d->ptr.pp_double[i][c1])/(csizes.ptr.p_int[c0]+csizes.ptr.p_int[c1]);
                }
                if( s->ahcalgo==3 )
                {
                    d->ptr.pp_double[i][c0] = (d->ptr.pp_double[i][c0]+d->ptr.pp_double[i][c1])/2;
                }
                if( s->ahcalgo==4 )
                {
                    d->ptr.pp_double[i][c0] = ((n0+ni)*d->ptr.pp_double[i][c0]+(n1+ni)*d->ptr.pp_double[i][c1]-ni*d01)/(n0+n1+ni);
                }
                d->ptr.pp_double[c0][i] = d->ptr.pp_double[i][c0];
            }
        }
        
        /*
         * Update CIdx and CSizes
         */
        cidx.ptr.p_int[c0] = npoints+mergeidx;
        cidx.ptr.p_int[c1] = -1;
        csizes.ptr.p_int[c0] = csizes.ptr.p_int[c0]+csizes.ptr.p_int[c1];
        csizes.ptr.p_int[c1] = 0;
        
        /*
         * Update nearest neighbors array:
         * * update nearest neighbors of everything except for C0/C1
         * * update neighbors of C0/C1
         */
        for(i=0; i<=npoints-1; i++)
        {
            if( (cidx.ptr.p_int[i]>=0&&i!=c0)&&(nnidx.ptr.p_int[i]==c0||nnidx.ptr.p_int[i]==c1) )
            {
                
                /*
                 * I-th cluster which is distinct from C0/C1 has former C0/C1 cluster as its nearest
                 * neighbor. We handle this issue depending on specific AHC algorithm being used.
                 */
                if( s->ahcalgo==1 )
                {
                    
                    /*
                     * Single linkage. Merging of two clusters together
                     * does NOT change distances between new cluster and
                     * other clusters.
                     *
                     * The only thing we have to do is to update nearest neighbor index
                     */
                    nnidx.ptr.p_int[i] = c0;
                }
                else
                {
                    
                    /*
                     * Something other than single linkage. We have to re-examine
                     * all the row to find nearest neighbor.
                     */
                    k = -1;
                    v = ae_maxrealnumber;
                    for(j=0; j<=npoints-1; j++)
                    {
                        if( (cidx.ptr.p_int[j]>=0&&j!=i)&&ae_fp_less(d->ptr.pp_double[i][j],v) )
                        {
                            k = j;
                            v = d->ptr.pp_double[i][j];
                        }
                    }
                    ae_assert(ae_fp_less(v,ae_maxrealnumber)||mergeidx==npoints-2, "ClusterizerRunAHC: internal error", _state);
                    nnidx.ptr.p_int[i] = k;
                }
            }
        }
        k = -1;
        v = ae_maxrealnumber;
        for(j=0; j<=npoints-1; j++)
        {
            if( (cidx.ptr.p_int[j]>=0&&j!=c0)&&ae_fp_less(d->ptr.pp_double[c0][j],v) )
            {
                k = j;
                v = d->ptr.pp_double[c0][j];
            }
        }
        ae_assert(ae_fp_less(v,ae_maxrealnumber)||mergeidx==npoints-2, "ClusterizerRunAHC: internal error", _state);
        nnidx.ptr.p_int[c0] = k;
    }
    
    /*
     * Calculate Rep.P and Rep.PM.
     *
     * In order to do that, we fill CInfo matrix - (2*NPoints-1)*3 matrix,
     * with I-th row containing:
     * * CInfo[I,0]     -   size of I-th cluster
     * * CInfo[I,1]     -   beginning of I-th cluster
     * * CInfo[I,2]     -   end of I-th cluster
     * * CInfo[I,3]     -   height of I-th cluster
     *
     * We perform it as follows:
     * * first NPoints clusters have unit size (CInfo[I,0]=1) and zero
     *   height (CInfo[I,3]=0)
     * * we replay NPoints-1 merges from first to last and fill sizes of
     *   corresponding clusters (new size is a sum of sizes of clusters
     *   being merged) and height (new height is max(heights)+1).
     * * now we ready to determine locations of clusters. Last cluster
     *   spans entire dataset, we know it. We replay merges from last to
     *   first, during each merge we already know location of the merge
     *   result, and we can position first cluster to the left part of
     *   the result, and second cluster to the right part.
     */
    ae_vector_set_length(&rep->p, npoints, _state);
    ae_matrix_set_length(&rep->pm, npoints-1, 6, _state);
    ae_matrix_set_length(&cinfo, 2*npoints-1, 4, _state);
    for(i=0; i<=npoints-1; i++)
    {
        cinfo.ptr.pp_int[i][0] = 1;
        cinfo.ptr.pp_int[i][3] = 0;
    }
    for(i=0; i<=npoints-2; i++)
    {
        cinfo.ptr.pp_int[npoints+i][0] = cinfo.ptr.pp_int[rep->z.ptr.pp_int[i][0]][0]+cinfo.ptr.pp_int[rep->z.ptr.pp_int[i][1]][0];
        cinfo.ptr.pp_int[npoints+i][3] = ae_maxint(cinfo.ptr.pp_int[rep->z.ptr.pp_int[i][0]][3], cinfo.ptr.pp_int[rep->z.ptr.pp_int[i][1]][3], _state)+1;
    }
    cinfo.ptr.pp_int[2*npoints-2][1] = 0;
    cinfo.ptr.pp_int[2*npoints-2][2] = npoints-1;
    for(i=npoints-2; i>=0; i--)
    {
        
        /*
         * We merge C0 which spans [A0,B0] and C1 (spans [A1,B1]),
         * with unknown A0, B0, A1, B1. However, we know that result
         * is CR, which spans [AR,BR] with known AR/BR, and we know
         * sizes of C0, C1, CR (denotes as S0, S1, SR).
         */
        c0 = rep->z.ptr.pp_int[i][0];
        c1 = rep->z.ptr.pp_int[i][1];
        s0 = cinfo.ptr.pp_int[c0][0];
        s1 = cinfo.ptr.pp_int[c1][0];
        ar = cinfo.ptr.pp_int[npoints+i][1];
        br = cinfo.ptr.pp_int[npoints+i][2];
        cinfo.ptr.pp_int[c0][1] = ar;
        cinfo.ptr.pp_int[c0][2] = ar+s0-1;
        cinfo.ptr.pp_int[c1][1] = br-(s1-1);
        cinfo.ptr.pp_int[c1][2] = br;
        rep->pm.ptr.pp_int[i][0] = cinfo.ptr.pp_int[c0][1];
        rep->pm.ptr.pp_int[i][1] = cinfo.ptr.pp_int[c0][2];
        rep->pm.ptr.pp_int[i][2] = cinfo.ptr.pp_int[c1][1];
        rep->pm.ptr.pp_int[i][3] = cinfo.ptr.pp_int[c1][2];
        rep->pm.ptr.pp_int[i][4] = cinfo.ptr.pp_int[c0][3];
        rep->pm.ptr.pp_int[i][5] = cinfo.ptr.pp_int[c1][3];
    }
    for(i=0; i<=npoints-1; i++)
    {
        ae_assert(cinfo.ptr.pp_int[i][1]==cinfo.ptr.pp_int[i][2], "Assertion failed", _state);
        rep->p.ptr.p_int[i] = cinfo.ptr.pp_int[i][1];
    }
    
    /*
     * Calculate Rep.PZ
     */
    ae_matrix_set_length(&rep->pz, npoints-1, 2, _state);
    for(i=0; i<=npoints-2; i++)
    {
        rep->pz.ptr.pp_int[i][0] = rep->z.ptr.pp_int[i][0];
        rep->pz.ptr.pp_int[i][1] = rep->z.ptr.pp_int[i][1];
        if( rep->pz.ptr.pp_int[i][0]<npoints )
        {
            rep->pz.ptr.pp_int[i][0] = rep->p.ptr.p_int[rep->pz.ptr.pp_int[i][0]];
        }
        if( rep->pz.ptr.pp_int[i][1]<npoints )
        {
            rep->pz.ptr.pp_int[i][1] = rep->p.ptr.p_int[rep->pz.ptr.pp_int[i][1]];
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function recursively evaluates distance matrix  for  SOME  (not all!)
distance types.

INPUT PARAMETERS:
    XY      -   array[?,NFeatures], dataset
    NFeatures-  number of features, >=1
    DistType-   distance function:
                *  0    Chebyshev distance  (L-inf norm)
                *  1    city block distance (L1 norm)
    D       -   preallocated output matrix
    I0,I1   -   half interval of rows to calculate: [I0,I1) is processed
    J0,J1   -   half interval of cols to calculate: [J0,J1) is processed

OUTPUT PARAMETERS:
    D       -   array[NPoints,NPoints], distance matrix
                upper triangle and main diagonal are initialized with
                data.

NOTE: intersection of [I0,I1) and [J0,J1)  may  completely  lie  in  upper
      triangle, only partially intersect with it, or have zero intersection.
      In any case, only intersection of submatrix given by [I0,I1)*[J0,J1)
      with upper triangle of the matrix is evaluated.
      
      Say, for 4x4 distance matrix A:
      * [0,2)*[0,2) will result in evaluation of A00, A01, A11
      * [2,4)*[2,4) will result in evaluation of A22, A23, A32, A33
      * [2,4)*[0,2) will result in evaluation of empty set of elements
      

  -- ALGLIB --
     Copyright 07.04.2013 by Bochkanov Sergey
*************************************************************************/
static void clustering_evaluatedistancematrixrec(/* Real    */ ae_matrix* xy,
     ae_int_t nfeatures,
     ae_int_t disttype,
     /* Real    */ ae_matrix* d,
     ae_int_t i0,
     ae_int_t i1,
     ae_int_t j0,
     ae_int_t j1,
     ae_state *_state)
{
    double rcomplexity;
    ae_int_t len0;
    ae_int_t len1;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    double vv;


    ae_assert(disttype==0||disttype==1, "EvaluateDistanceMatrixRec: incorrect DistType", _state);
    
    /*
     * Normalize J0/J1:
     * * J0:=max(J0,I0) - we ignore lower triangle
     * * J1:=max(J1,J0) - normalize J1
     */
    j0 = ae_maxint(j0, i0, _state);
    j1 = ae_maxint(j1, j0, _state);
    if( j1<=j0||i1<=i0 )
    {
        return;
    }
    
    /*
     * Try to process in parallel. Two condtions must hold in order to
     * activate parallel processing:
     * 1. I1-I0>2 or J1-J0>2
     * 2. (I1-I0)*(J1-J0)*NFeatures>=ParallelComplexity
     *
     * NOTE: all quantities are converted to reals in order to avoid
     *       integer overflow during multiplication
     *
     * NOTE: strict inequality in (1) is necessary to reduce task to 2x2
     *       basecases. In future versions we will be able to handle such
     *       basecases more efficiently than 1x1 cases.
     */
    rcomplexity = (double)(i1-i0);
    rcomplexity = rcomplexity*(j1-j0);
    rcomplexity = rcomplexity*nfeatures;
    if( ae_fp_greater_eq(rcomplexity,clustering_parallelcomplexity)&&(i1-i0>2||j1-j0>2) )
    {
        
        /*
         * Recursive division along largest of dimensions
         */
        if( i1-i0>j1-j0 )
        {
            splitlengtheven(i1-i0, &len0, &len1, _state);
            clustering_evaluatedistancematrixrec(xy, nfeatures, disttype, d, i0, i0+len0, j0, j1, _state);
            clustering_evaluatedistancematrixrec(xy, nfeatures, disttype, d, i0+len0, i1, j0, j1, _state);
        }
        else
        {
            splitlengtheven(j1-j0, &len0, &len1, _state);
            clustering_evaluatedistancematrixrec(xy, nfeatures, disttype, d, i0, i1, j0, j0+len0, _state);
            clustering_evaluatedistancematrixrec(xy, nfeatures, disttype, d, i0, i1, j0+len0, j1, _state);
        }
        return;
    }
    
    /*
     * Sequential processing
     */
    for(i=i0; i<=i1-1; i++)
    {
        for(j=j0; j<=j1-1; j++)
        {
            if( j>=i )
            {
                v = 0.0;
                if( disttype==0 )
                {
                    for(k=0; k<=nfeatures-1; k++)
                    {
                        vv = xy->ptr.pp_double[i][k]-xy->ptr.pp_double[j][k];
                        if( ae_fp_less(vv,(double)(0)) )
                        {
                            vv = -vv;
                        }
                        if( ae_fp_greater(vv,v) )
                        {
                            v = vv;
                        }
                    }
                }
                if( disttype==1 )
                {
                    for(k=0; k<=nfeatures-1; k++)
                    {
                        vv = xy->ptr.pp_double[i][k]-xy->ptr.pp_double[j][k];
                        if( ae_fp_less(vv,(double)(0)) )
                        {
                            vv = -vv;
                        }
                        v = v+vv;
                    }
                }
                d->ptr.pp_double[i][j] = v;
            }
        }
    }
}


void _kmeansbuffers_init(void* _p, ae_state *_state)
{
    kmeansbuffers *p = (kmeansbuffers*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_init(&p->ct, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->ctbest, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->xycbest, 0, DT_INT, _state);
    ae_vector_init(&p->xycprev, 0, DT_INT, _state);
    ae_vector_init(&p->d2, 0, DT_REAL, _state);
    ae_vector_init(&p->csizes, 0, DT_INT, _state);
    _apbuffers_init(&p->initbuf, _state);
    ae_shared_pool_init(&p->updatepool, _state);
}


void _kmeansbuffers_init_copy(void* _dst, void* _src, ae_state *_state)
{
    kmeansbuffers *dst = (kmeansbuffers*)_dst;
    kmeansbuffers *src = (kmeansbuffers*)_src;
    ae_matrix_init_copy(&dst->ct, &src->ct, _state);
    ae_matrix_init_copy(&dst->ctbest, &src->ctbest, _state);
    ae_vector_init_copy(&dst->xycbest, &src->xycbest, _state);
    ae_vector_init_copy(&dst->xycprev, &src->xycprev, _state);
    ae_vector_init_copy(&dst->d2, &src->d2, _state);
    ae_vector_init_copy(&dst->csizes, &src->csizes, _state);
    _apbuffers_init_copy(&dst->initbuf, &src->initbuf, _state);
    ae_shared_pool_init_copy(&dst->updatepool, &src->updatepool, _state);
}


void _kmeansbuffers_clear(void* _p)
{
    kmeansbuffers *p = (kmeansbuffers*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_clear(&p->ct);
    ae_matrix_clear(&p->ctbest);
    ae_vector_clear(&p->xycbest);
    ae_vector_clear(&p->xycprev);
    ae_vector_clear(&p->d2);
    ae_vector_clear(&p->csizes);
    _apbuffers_clear(&p->initbuf);
    ae_shared_pool_clear(&p->updatepool);
}


void _kmeansbuffers_destroy(void* _p)
{
    kmeansbuffers *p = (kmeansbuffers*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_destroy(&p->ct);
    ae_matrix_destroy(&p->ctbest);
    ae_vector_destroy(&p->xycbest);
    ae_vector_destroy(&p->xycprev);
    ae_vector_destroy(&p->d2);
    ae_vector_destroy(&p->csizes);
    _apbuffers_destroy(&p->initbuf);
    ae_shared_pool_destroy(&p->updatepool);
}


void _clusterizerstate_init(void* _p, ae_state *_state)
{
    clusterizerstate *p = (clusterizerstate*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_init(&p->xy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->d, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->tmpd, 0, 0, DT_REAL, _state);
    _apbuffers_init(&p->distbuf, _state);
    _kmeansbuffers_init(&p->kmeanstmp, _state);
}


void _clusterizerstate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    clusterizerstate *dst = (clusterizerstate*)_dst;
    clusterizerstate *src = (clusterizerstate*)_src;
    dst->npoints = src->npoints;
    dst->nfeatures = src->nfeatures;
    dst->disttype = src->disttype;
    ae_matrix_init_copy(&dst->xy, &src->xy, _state);
    ae_matrix_init_copy(&dst->d, &src->d, _state);
    dst->ahcalgo = src->ahcalgo;
    dst->kmeansrestarts = src->kmeansrestarts;
    dst->kmeansmaxits = src->kmeansmaxits;
    dst->kmeansinitalgo = src->kmeansinitalgo;
    dst->kmeansdbgnoits = src->kmeansdbgnoits;
    ae_matrix_init_copy(&dst->tmpd, &src->tmpd, _state);
    _apbuffers_init_copy(&dst->distbuf, &src->distbuf, _state);
    _kmeansbuffers_init_copy(&dst->kmeanstmp, &src->kmeanstmp, _state);
}


void _clusterizerstate_clear(void* _p)
{
    clusterizerstate *p = (clusterizerstate*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_clear(&p->xy);
    ae_matrix_clear(&p->d);
    ae_matrix_clear(&p->tmpd);
    _apbuffers_clear(&p->distbuf);
    _kmeansbuffers_clear(&p->kmeanstmp);
}


void _clusterizerstate_destroy(void* _p)
{
    clusterizerstate *p = (clusterizerstate*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_destroy(&p->xy);
    ae_matrix_destroy(&p->d);
    ae_matrix_destroy(&p->tmpd);
    _apbuffers_destroy(&p->distbuf);
    _kmeansbuffers_destroy(&p->kmeanstmp);
}


void _ahcreport_init(void* _p, ae_state *_state)
{
    ahcreport *p = (ahcreport*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->p, 0, DT_INT, _state);
    ae_matrix_init(&p->z, 0, 0, DT_INT, _state);
    ae_matrix_init(&p->pz, 0, 0, DT_INT, _state);
    ae_matrix_init(&p->pm, 0, 0, DT_INT, _state);
    ae_vector_init(&p->mergedist, 0, DT_REAL, _state);
}


void _ahcreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    ahcreport *dst = (ahcreport*)_dst;
    ahcreport *src = (ahcreport*)_src;
    dst->terminationtype = src->terminationtype;
    dst->npoints = src->npoints;
    ae_vector_init_copy(&dst->p, &src->p, _state);
    ae_matrix_init_copy(&dst->z, &src->z, _state);
    ae_matrix_init_copy(&dst->pz, &src->pz, _state);
    ae_matrix_init_copy(&dst->pm, &src->pm, _state);
    ae_vector_init_copy(&dst->mergedist, &src->mergedist, _state);
}


void _ahcreport_clear(void* _p)
{
    ahcreport *p = (ahcreport*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->p);
    ae_matrix_clear(&p->z);
    ae_matrix_clear(&p->pz);
    ae_matrix_clear(&p->pm);
    ae_vector_clear(&p->mergedist);
}


void _ahcreport_destroy(void* _p)
{
    ahcreport *p = (ahcreport*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->p);
    ae_matrix_destroy(&p->z);
    ae_matrix_destroy(&p->pz);
    ae_matrix_destroy(&p->pm);
    ae_vector_destroy(&p->mergedist);
}


void _kmeansreport_init(void* _p, ae_state *_state)
{
    kmeansreport *p = (kmeansreport*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_init(&p->c, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->cidx, 0, DT_INT, _state);
}


void _kmeansreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    kmeansreport *dst = (kmeansreport*)_dst;
    kmeansreport *src = (kmeansreport*)_src;
    dst->npoints = src->npoints;
    dst->nfeatures = src->nfeatures;
    dst->terminationtype = src->terminationtype;
    dst->iterationscount = src->iterationscount;
    dst->energy = src->energy;
    dst->k = src->k;
    ae_matrix_init_copy(&dst->c, &src->c, _state);
    ae_vector_init_copy(&dst->cidx, &src->cidx, _state);
}


void _kmeansreport_clear(void* _p)
{
    kmeansreport *p = (kmeansreport*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_clear(&p->c);
    ae_vector_clear(&p->cidx);
}


void _kmeansreport_destroy(void* _p)
{
    kmeansreport *p = (kmeansreport*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_destroy(&p->c);
    ae_vector_destroy(&p->cidx);
}


/*$ End $*/
