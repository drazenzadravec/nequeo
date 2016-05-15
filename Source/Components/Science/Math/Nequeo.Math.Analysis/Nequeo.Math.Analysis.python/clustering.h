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

#ifndef _clustering_h
#define _clustering_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "tsort.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"
#include "hqrnd.h"
#include "blas.h"
#include "basicstatops.h"
#include "basestat.h"


/*$ Declarations $*/


/*************************************************************************
This structure is used to  store  temporaries  for  KMeansGenerateInternal
function, so we will be able to  reuse  them  during  multiple  subsequent
calls.
    
  -- ALGLIB --
     Copyright 24.07.2015 by Bochkanov Sergey
*************************************************************************/
typedef struct
{
    ae_matrix ct;
    ae_matrix ctbest;
    ae_vector xycbest;
    ae_vector xycprev;
    ae_vector d2;
    ae_vector csizes;
    apbuffers initbuf;
    ae_shared_pool updatepool;
} kmeansbuffers;


/*************************************************************************
This structure is a clusterization engine.

You should not try to access its fields directly.
Use ALGLIB functions in order to work with this object.

  -- ALGLIB --
     Copyright 10.07.2012 by Bochkanov Sergey
*************************************************************************/
typedef struct
{
    ae_int_t npoints;
    ae_int_t nfeatures;
    ae_int_t disttype;
    ae_matrix xy;
    ae_matrix d;
    ae_int_t ahcalgo;
    ae_int_t kmeansrestarts;
    ae_int_t kmeansmaxits;
    ae_int_t kmeansinitalgo;
    ae_bool kmeansdbgnoits;
    ae_matrix tmpd;
    apbuffers distbuf;
    kmeansbuffers kmeanstmp;
} clusterizerstate;


/*************************************************************************
This structure  is used to store results of the agglomerative hierarchical
clustering (AHC).

Following information is returned:

* TerminationType - completion code:
  * 1   for successful completion of algorithm
  * -5  inappropriate combination of  clustering  algorithm  and  distance
        function was used. As for now, it  is  possible  only when  Ward's
        method is called for dataset with non-Euclidean distance function.
  In case negative completion code is returned,  other  fields  of  report
  structure are invalid and should not be used.

* NPoints contains number of points in the original dataset

* Z contains information about merges performed  (see below).  Z  contains
  indexes from the original (unsorted) dataset and it can be used when you
  need to know what points were merged. However, it is not convenient when
  you want to build a dendrograd (see below).
  
* if  you  want  to  build  dendrogram, you  can use Z, but it is not good
  option, because Z contains  indexes from  unsorted  dataset.  Dendrogram
  built from such dataset is likely to have intersections. So, you have to
  reorder you points before building dendrogram.
  Permutation which reorders point is returned in P. Another representation
  of  merges,  which  is  more  convenient for dendorgram construction, is
  returned in PM.
  
* more information on format of Z, P and PM can be found below and in the
  examples from ALGLIB Reference Manual.

FORMAL DESCRIPTION OF FIELDS:
    NPoints         number of points
    Z               array[NPoints-1,2],  contains   indexes   of  clusters
                    linked in pairs to  form  clustering  tree.  I-th  row
                    corresponds to I-th merge:
                    * Z[I,0] - index of the first cluster to merge
                    * Z[I,1] - index of the second cluster to merge
                    * Z[I,0]<Z[I,1]
                    * clusters are  numbered  from 0 to 2*NPoints-2,  with
                      indexes from 0 to NPoints-1 corresponding to  points
                      of the original dataset, and indexes from NPoints to
                      2*NPoints-2  correspond  to  clusters  generated  by
                      subsequent  merges  (I-th  row  of Z creates cluster
                      with index NPoints+I).
                    
                    IMPORTANT: indexes in Z[] are indexes in the ORIGINAL,
                    unsorted dataset. In addition to  Z algorithm  outputs
                    permutation which rearranges points in such  way  that
                    subsequent merges are  performed  on  adjacent  points
                    (such order is needed if you want to build dendrogram).
                    However,  indexes  in  Z  are  related  to   original,
                    unrearranged sequence of points.
                    
    P               array[NPoints], permutation which reorders points  for
                    dendrogram  construction.  P[i] contains  index of the
                    position  where  we  should  move  I-th  point  of the
                    original dataset in order to apply merges PZ/PM.

    PZ              same as Z, but for permutation of points given  by  P.
                    The  only  thing  which  changed  are  indexes  of the
                    original points; indexes of clusters remained same.
                    
    MergeDist       array[NPoints-1], contains distances between  clusters
                    being merged (MergeDist[i] correspond to merge  stored
                    in Z[i,...]):
                    * CLINK, SLINK and  average  linkage algorithms report
                      "raw", unmodified distance metric.
                    * Ward's   method   reports   weighted   intra-cluster
                      variance, which is equal to ||Ca-Cb||^2 * Sa*Sb/(Sa+Sb).
                      Here  A  and  B  are  clusters being merged, Ca is a
                      center of A, Cb is a center of B, Sa is a size of A,
                      Sb is a size of B.
                    
    PM              array[NPoints-1,6], another representation of  merges,
                    which is suited for dendrogram construction. It  deals
                    with rearranged points (permutation P is applied)  and
                    represents merges in a form which different  from  one
                    used by Z.
                    For each I from 0 to NPoints-2, I-th row of PM represents
                    merge performed on two clusters C0 and C1. Here:
                    * C0 contains points with indexes PM[I,0]...PM[I,1]
                    * C1 contains points with indexes PM[I,2]...PM[I,3]
                    * indexes stored in PM are given for dataset sorted
                      according to permutation P
                    * PM[I,1]=PM[I,2]-1 (only adjacent clusters are merged)
                    * PM[I,0]<=PM[I,1], PM[I,2]<=PM[I,3], i.e. both
                      clusters contain at least one point
                    * heights of "subdendrograms" corresponding  to  C0/C1
                      are stored in PM[I,4]  and  PM[I,5].  Subdendrograms
                      corresponding   to   single-point   clusters    have
                      height=0. Dendrogram of the merge result has  height
                      H=max(H0,H1)+1.

NOTE: there is one-to-one correspondence between merges described by Z and
      PM. I-th row of Z describes same merge of clusters as I-th row of PM,
      with "left" cluster from Z corresponding to the "left" one from PM.

  -- ALGLIB --
     Copyright 10.07.2012 by Bochkanov Sergey
*************************************************************************/
typedef struct
{
    ae_int_t terminationtype;
    ae_int_t npoints;
    ae_vector p;
    ae_matrix z;
    ae_matrix pz;
    ae_matrix pm;
    ae_vector mergedist;
} ahcreport;


/*************************************************************************
This  structure   is  used  to  store  results of the  k-means  clustering
algorithm.

Following information is always returned:
* NPoints contains number of points in the original dataset
* TerminationType contains completion code, negative on failure, positive
  on success
* K contains number of clusters

For positive TerminationType we return:
* NFeatures contains number of variables in the original dataset
* C, which contains centers found by algorithm
* CIdx, which maps points of the original dataset to clusters

FORMAL DESCRIPTION OF FIELDS:
    NPoints         number of points, >=0
    NFeatures       number of variables, >=1
    TerminationType completion code:
                    * -5 if  distance  type  is  anything  different  from
                         Euclidean metric
                    * -3 for degenerate dataset: a) less  than  K  distinct
                         points, b) K=0 for non-empty dataset.
                    * +1 for successful completion
    K               number of clusters
    C               array[K,NFeatures], rows of the array store centers
    CIdx            array[NPoints], which contains cluster indexes
    IterationsCount actual number of iterations performed by clusterizer.
                    If algorithm performed more than one random restart,
                    total number of iterations is returned.
    Energy          merit function, "energy", sum  of  squared  deviations
                    from cluster centers
    
  -- ALGLIB --
     Copyright 27.11.2012 by Bochkanov Sergey
*************************************************************************/
typedef struct
{
    ae_int_t npoints;
    ae_int_t nfeatures;
    ae_int_t terminationtype;
    ae_int_t iterationscount;
    double energy;
    ae_int_t k;
    ae_matrix c;
    ae_vector cidx;
} kmeansreport;


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
void clusterizercreate(clusterizerstate* s, ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);
void _pexec_clusterizerrunahc(clusterizerstate* s,
    ahcreport* rep, ae_state *_state);


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
     ae_state *_state);
void _pexec_clusterizerrunkmeans(clusterizerstate* s,
    ae_int_t k,
    kmeansreport* rep, ae_state *_state);


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
     ae_state *_state);
void _pexec_clusterizergetdistances(/* Real    */ ae_matrix* xy,
    ae_int_t npoints,
    ae_int_t nfeatures,
    ae_int_t disttype,
    /* Real    */ ae_matrix* d, ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
void kmeansinitbuf(kmeansbuffers* buf, ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);
void _kmeansbuffers_init(void* _p, ae_state *_state);
void _kmeansbuffers_init_copy(void* _dst, void* _src, ae_state *_state);
void _kmeansbuffers_clear(void* _p);
void _kmeansbuffers_destroy(void* _p);
void _clusterizerstate_init(void* _p, ae_state *_state);
void _clusterizerstate_init_copy(void* _dst, void* _src, ae_state *_state);
void _clusterizerstate_clear(void* _p);
void _clusterizerstate_destroy(void* _p);
void _ahcreport_init(void* _p, ae_state *_state);
void _ahcreport_init_copy(void* _dst, void* _src, ae_state *_state);
void _ahcreport_clear(void* _p);
void _ahcreport_destroy(void* _p);
void _kmeansreport_init(void* _p, ae_state *_state);
void _kmeansreport_init_copy(void* _dst, void* _src, ae_state *_state);
void _kmeansreport_clear(void* _p);
void _kmeansreport_destroy(void* _p);


/*$ End $*/
#endif

