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
#include "nearestneighbor.h"


/*$ Declarations $*/
static ae_int_t nearestneighbor_splitnodesize = 6;
static ae_int_t nearestneighbor_kdtreefirstversion = 0;
static void nearestneighbor_kdtreesplit(kdtree* kdt,
     ae_int_t i1,
     ae_int_t i2,
     ae_int_t d,
     double s,
     ae_int_t* i3,
     ae_state *_state);
static void nearestneighbor_kdtreegeneratetreerec(kdtree* kdt,
     ae_int_t* nodesoffs,
     ae_int_t* splitsoffs,
     ae_int_t i1,
     ae_int_t i2,
     ae_int_t maxleafsize,
     ae_state *_state);
static void nearestneighbor_kdtreequerynnrec(kdtree* kdt,
     ae_int_t offs,
     ae_state *_state);
static void nearestneighbor_kdtreeinitbox(kdtree* kdt,
     /* Real    */ ae_vector* x,
     ae_state *_state);
static void nearestneighbor_kdtreeallocdatasetindependent(kdtree* kdt,
     ae_int_t nx,
     ae_int_t ny,
     ae_state *_state);
static void nearestneighbor_kdtreeallocdatasetdependent(kdtree* kdt,
     ae_int_t n,
     ae_int_t nx,
     ae_int_t ny,
     ae_state *_state);
static void nearestneighbor_kdtreealloctemporaries(kdtree* kdt,
     ae_int_t n,
     ae_int_t nx,
     ae_int_t ny,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
KD-tree creation

This subroutine creates KD-tree from set of X-values and optional Y-values

INPUT PARAMETERS
    XY      -   dataset, array[0..N-1,0..NX+NY-1].
                one row corresponds to one point.
                first NX columns contain X-values, next NY (NY may be zero)
                columns may contain associated Y-values
    N       -   number of points, N>=0.
    NX      -   space dimension, NX>=1.
    NY      -   number of optional Y-values, NY>=0.
    NormType-   norm type:
                * 0 denotes infinity-norm
                * 1 denotes 1-norm
                * 2 denotes 2-norm (Euclidean norm)
                
OUTPUT PARAMETERS
    KDT     -   KD-tree
    
    
NOTES

1. KD-tree  creation  have O(N*logN) complexity and O(N*(2*NX+NY))  memory
   requirements.
2. Although KD-trees may be used with any combination of N  and  NX,  they
   are more efficient than brute-force search only when N >> 4^NX. So they
   are most useful in low-dimensional tasks (NX=2, NX=3). NX=1  is another
   inefficient case, because  simple  binary  search  (without  additional
   structures) is much more efficient in such tasks than KD-trees.

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
void kdtreebuild(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t nx,
     ae_int_t ny,
     ae_int_t normtype,
     kdtree* kdt,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector tags;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    _kdtree_clear(kdt);
    ae_vector_init(&tags, 0, DT_INT, _state);

    ae_assert(n>=0, "KDTreeBuild: N<0", _state);
    ae_assert(nx>=1, "KDTreeBuild: NX<1", _state);
    ae_assert(ny>=0, "KDTreeBuild: NY<0", _state);
    ae_assert(normtype>=0&&normtype<=2, "KDTreeBuild: incorrect NormType", _state);
    ae_assert(xy->rows>=n, "KDTreeBuild: rows(X)<N", _state);
    ae_assert(xy->cols>=nx+ny||n==0, "KDTreeBuild: cols(X)<NX+NY", _state);
    ae_assert(apservisfinitematrix(xy, n, nx+ny, _state), "KDTreeBuild: XY contains infinite or NaN values", _state);
    if( n>0 )
    {
        ae_vector_set_length(&tags, n, _state);
        for(i=0; i<=n-1; i++)
        {
            tags.ptr.p_int[i] = 0;
        }
    }
    kdtreebuildtagged(xy, &tags, n, nx, ny, normtype, kdt, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
KD-tree creation

This  subroutine  creates  KD-tree  from set of X-values, integer tags and
optional Y-values

INPUT PARAMETERS
    XY      -   dataset, array[0..N-1,0..NX+NY-1].
                one row corresponds to one point.
                first NX columns contain X-values, next NY (NY may be zero)
                columns may contain associated Y-values
    Tags    -   tags, array[0..N-1], contains integer tags associated
                with points.
    N       -   number of points, N>=0
    NX      -   space dimension, NX>=1.
    NY      -   number of optional Y-values, NY>=0.
    NormType-   norm type:
                * 0 denotes infinity-norm
                * 1 denotes 1-norm
                * 2 denotes 2-norm (Euclidean norm)

OUTPUT PARAMETERS
    KDT     -   KD-tree

NOTES

1. KD-tree  creation  have O(N*logN) complexity and O(N*(2*NX+NY))  memory
   requirements.
2. Although KD-trees may be used with any combination of N  and  NX,  they
   are more efficient than brute-force search only when N >> 4^NX. So they
   are most useful in low-dimensional tasks (NX=2, NX=3). NX=1  is another
   inefficient case, because  simple  binary  search  (without  additional
   structures) is much more efficient in such tasks than KD-trees.

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
void kdtreebuildtagged(/* Real    */ ae_matrix* xy,
     /* Integer */ ae_vector* tags,
     ae_int_t n,
     ae_int_t nx,
     ae_int_t ny,
     ae_int_t normtype,
     kdtree* kdt,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t maxnodes;
    ae_int_t nodesoffs;
    ae_int_t splitsoffs;

    _kdtree_clear(kdt);

    ae_assert(n>=0, "KDTreeBuildTagged: N<0", _state);
    ae_assert(nx>=1, "KDTreeBuildTagged: NX<1", _state);
    ae_assert(ny>=0, "KDTreeBuildTagged: NY<0", _state);
    ae_assert(normtype>=0&&normtype<=2, "KDTreeBuildTagged: incorrect NormType", _state);
    ae_assert(xy->rows>=n, "KDTreeBuildTagged: rows(X)<N", _state);
    ae_assert(xy->cols>=nx+ny||n==0, "KDTreeBuildTagged: cols(X)<NX+NY", _state);
    ae_assert(apservisfinitematrix(xy, n, nx+ny, _state), "KDTreeBuildTagged: XY contains infinite or NaN values", _state);
    
    /*
     * initialize
     */
    kdt->n = n;
    kdt->nx = nx;
    kdt->ny = ny;
    kdt->normtype = normtype;
    kdt->kcur = 0;
    
    /*
     * N=0 => quick exit
     */
    if( n==0 )
    {
        return;
    }
    
    /*
     * Allocate
     */
    nearestneighbor_kdtreeallocdatasetindependent(kdt, nx, ny, _state);
    nearestneighbor_kdtreeallocdatasetdependent(kdt, n, nx, ny, _state);
    
    /*
     * Initial fill
     */
    for(i=0; i<=n-1; i++)
    {
        ae_v_move(&kdt->xy.ptr.pp_double[i][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nx-1));
        ae_v_move(&kdt->xy.ptr.pp_double[i][nx], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(nx,2*nx+ny-1));
        kdt->tags.ptr.p_int[i] = tags->ptr.p_int[i];
    }
    
    /*
     * Determine bounding box
     */
    ae_v_move(&kdt->boxmin.ptr.p_double[0], 1, &kdt->xy.ptr.pp_double[0][0], 1, ae_v_len(0,nx-1));
    ae_v_move(&kdt->boxmax.ptr.p_double[0], 1, &kdt->xy.ptr.pp_double[0][0], 1, ae_v_len(0,nx-1));
    for(i=1; i<=n-1; i++)
    {
        for(j=0; j<=nx-1; j++)
        {
            kdt->boxmin.ptr.p_double[j] = ae_minreal(kdt->boxmin.ptr.p_double[j], kdt->xy.ptr.pp_double[i][j], _state);
            kdt->boxmax.ptr.p_double[j] = ae_maxreal(kdt->boxmax.ptr.p_double[j], kdt->xy.ptr.pp_double[i][j], _state);
        }
    }
    
    /*
     * prepare tree structure
     * * MaxNodes=N because we guarantee no trivial splits, i.e.
     *   every split will generate two non-empty boxes
     */
    maxnodes = n;
    ae_vector_set_length(&kdt->nodes, nearestneighbor_splitnodesize*2*maxnodes, _state);
    ae_vector_set_length(&kdt->splits, 2*maxnodes, _state);
    nodesoffs = 0;
    splitsoffs = 0;
    ae_v_move(&kdt->curboxmin.ptr.p_double[0], 1, &kdt->boxmin.ptr.p_double[0], 1, ae_v_len(0,nx-1));
    ae_v_move(&kdt->curboxmax.ptr.p_double[0], 1, &kdt->boxmax.ptr.p_double[0], 1, ae_v_len(0,nx-1));
    nearestneighbor_kdtreegeneratetreerec(kdt, &nodesoffs, &splitsoffs, 0, n, 8, _state);
}


/*************************************************************************
K-NN query: K nearest neighbors

INPUT PARAMETERS
    KDT         -   KD-tree
    X           -   point, array[0..NX-1].
    K           -   number of neighbors to return, K>=1
    SelfMatch   -   whether self-matches are allowed:
                    * if True, nearest neighbor may be the point itself
                      (if it exists in original dataset)
                    * if False, then only points with non-zero distance
                      are returned
                    * if not given, considered True

RESULT
    number of actual neighbors found (either K or N, if K>N).

This  subroutine  performs  query  and  stores  its result in the internal
structures of the KD-tree. You can use  following  subroutines  to  obtain
these results:
* KDTreeQueryResultsX() to get X-values
* KDTreeQueryResultsXY() to get X- and Y-values
* KDTreeQueryResultsTags() to get tag values
* KDTreeQueryResultsDistances() to get distances

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
ae_int_t kdtreequeryknn(kdtree* kdt,
     /* Real    */ ae_vector* x,
     ae_int_t k,
     ae_bool selfmatch,
     ae_state *_state)
{
    ae_int_t result;


    ae_assert(k>=1, "KDTreeQueryKNN: K<1!", _state);
    ae_assert(x->cnt>=kdt->nx, "KDTreeQueryKNN: Length(X)<NX!", _state);
    ae_assert(isfinitevector(x, kdt->nx, _state), "KDTreeQueryKNN: X contains infinite or NaN values!", _state);
    result = kdtreequeryaknn(kdt, x, k, selfmatch, 0.0, _state);
    return result;
}


/*************************************************************************
R-NN query: all points within R-sphere centered at X

INPUT PARAMETERS
    KDT         -   KD-tree
    X           -   point, array[0..NX-1].
    R           -   radius of sphere (in corresponding norm), R>0
    SelfMatch   -   whether self-matches are allowed:
                    * if True, nearest neighbor may be the point itself
                      (if it exists in original dataset)
                    * if False, then only points with non-zero distance
                      are returned
                    * if not given, considered True

RESULT
    number of neighbors found, >=0

This  subroutine  performs  query  and  stores  its result in the internal
structures of the KD-tree. You can use  following  subroutines  to  obtain
actual results:
* KDTreeQueryResultsX() to get X-values
* KDTreeQueryResultsXY() to get X- and Y-values
* KDTreeQueryResultsTags() to get tag values
* KDTreeQueryResultsDistances() to get distances

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
ae_int_t kdtreequeryrnn(kdtree* kdt,
     /* Real    */ ae_vector* x,
     double r,
     ae_bool selfmatch,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t result;


    ae_assert(ae_fp_greater(r,(double)(0)), "KDTreeQueryRNN: incorrect R!", _state);
    ae_assert(x->cnt>=kdt->nx, "KDTreeQueryRNN: Length(X)<NX!", _state);
    ae_assert(isfinitevector(x, kdt->nx, _state), "KDTreeQueryRNN: X contains infinite or NaN values!", _state);
    
    /*
     * Handle special case: KDT.N=0
     */
    if( kdt->n==0 )
    {
        kdt->kcur = 0;
        result = 0;
        return result;
    }
    
    /*
     * Prepare parameters
     */
    kdt->kneeded = 0;
    if( kdt->normtype!=2 )
    {
        kdt->rneeded = r;
    }
    else
    {
        kdt->rneeded = ae_sqr(r, _state);
    }
    kdt->selfmatch = selfmatch;
    kdt->approxf = (double)(1);
    kdt->kcur = 0;
    
    /*
     * calculate distance from point to current bounding box
     */
    nearestneighbor_kdtreeinitbox(kdt, x, _state);
    
    /*
     * call recursive search
     * results are returned as heap
     */
    nearestneighbor_kdtreequerynnrec(kdt, 0, _state);
    
    /*
     * pop from heap to generate ordered representation
     *
     * last element is not pop'ed because it is already in
     * its place
     */
    result = kdt->kcur;
    j = kdt->kcur;
    for(i=kdt->kcur; i>=2; i--)
    {
        tagheappopi(&kdt->r, &kdt->idx, &j, _state);
    }
    return result;
}


/*************************************************************************
K-NN query: approximate K nearest neighbors

INPUT PARAMETERS
    KDT         -   KD-tree
    X           -   point, array[0..NX-1].
    K           -   number of neighbors to return, K>=1
    SelfMatch   -   whether self-matches are allowed:
                    * if True, nearest neighbor may be the point itself
                      (if it exists in original dataset)
                    * if False, then only points with non-zero distance
                      are returned
                    * if not given, considered True
    Eps         -   approximation factor, Eps>=0. eps-approximate  nearest
                    neighbor  is  a  neighbor  whose distance from X is at
                    most (1+eps) times distance of true nearest neighbor.

RESULT
    number of actual neighbors found (either K or N, if K>N).
    
NOTES
    significant performance gain may be achieved only when Eps  is  is  on
    the order of magnitude of 1 or larger.

This  subroutine  performs  query  and  stores  its result in the internal
structures of the KD-tree. You can use  following  subroutines  to  obtain
these results:
* KDTreeQueryResultsX() to get X-values
* KDTreeQueryResultsXY() to get X- and Y-values
* KDTreeQueryResultsTags() to get tag values
* KDTreeQueryResultsDistances() to get distances

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
ae_int_t kdtreequeryaknn(kdtree* kdt,
     /* Real    */ ae_vector* x,
     ae_int_t k,
     ae_bool selfmatch,
     double eps,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t result;


    ae_assert(k>0, "KDTreeQueryAKNN: incorrect K!", _state);
    ae_assert(ae_fp_greater_eq(eps,(double)(0)), "KDTreeQueryAKNN: incorrect Eps!", _state);
    ae_assert(x->cnt>=kdt->nx, "KDTreeQueryAKNN: Length(X)<NX!", _state);
    ae_assert(isfinitevector(x, kdt->nx, _state), "KDTreeQueryAKNN: X contains infinite or NaN values!", _state);
    
    /*
     * Handle special case: KDT.N=0
     */
    if( kdt->n==0 )
    {
        kdt->kcur = 0;
        result = 0;
        return result;
    }
    
    /*
     * Prepare parameters
     */
    k = ae_minint(k, kdt->n, _state);
    kdt->kneeded = k;
    kdt->rneeded = (double)(0);
    kdt->selfmatch = selfmatch;
    if( kdt->normtype==2 )
    {
        kdt->approxf = 1/ae_sqr(1+eps, _state);
    }
    else
    {
        kdt->approxf = 1/(1+eps);
    }
    kdt->kcur = 0;
    
    /*
     * calculate distance from point to current bounding box
     */
    nearestneighbor_kdtreeinitbox(kdt, x, _state);
    
    /*
     * call recursive search
     * results are returned as heap
     */
    nearestneighbor_kdtreequerynnrec(kdt, 0, _state);
    
    /*
     * pop from heap to generate ordered representation
     *
     * last element is non pop'ed because it is already in
     * its place
     */
    result = kdt->kcur;
    j = kdt->kcur;
    for(i=kdt->kcur; i>=2; i--)
    {
        tagheappopi(&kdt->r, &kdt->idx, &j, _state);
    }
    return result;
}


/*************************************************************************
X-values from last query

INPUT PARAMETERS
    KDT     -   KD-tree
    X       -   possibly pre-allocated buffer. If X is too small to store
                result, it is resized. If size(X) is enough to store
                result, it is left unchanged.

OUTPUT PARAMETERS
    X       -   rows are filled with X-values

NOTES
1. points are ordered by distance from the query point (first = closest)
2. if  XY is larger than required to store result, only leading part  will
   be overwritten; trailing part will be left unchanged. So  if  on  input
   XY = [[A,B],[C,D]], and result is [1,2],  then  on  exit  we  will  get
   XY = [[1,2],[C,D]]. This is done purposely to increase performance;  if
   you want function  to  resize  array  according  to  result  size,  use
   function with same name and suffix 'I'.

SEE ALSO
* KDTreeQueryResultsXY()            X- and Y-values
* KDTreeQueryResultsTags()          tag values
* KDTreeQueryResultsDistances()     distances

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
void kdtreequeryresultsx(kdtree* kdt,
     /* Real    */ ae_matrix* x,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;


    if( kdt->kcur==0 )
    {
        return;
    }
    if( x->rows<kdt->kcur||x->cols<kdt->nx )
    {
        ae_matrix_set_length(x, kdt->kcur, kdt->nx, _state);
    }
    k = kdt->kcur;
    for(i=0; i<=k-1; i++)
    {
        ae_v_move(&x->ptr.pp_double[i][0], 1, &kdt->xy.ptr.pp_double[kdt->idx.ptr.p_int[i]][kdt->nx], 1, ae_v_len(0,kdt->nx-1));
    }
}


/*************************************************************************
X- and Y-values from last query

INPUT PARAMETERS
    KDT     -   KD-tree
    XY      -   possibly pre-allocated buffer. If XY is too small to store
                result, it is resized. If size(XY) is enough to store
                result, it is left unchanged.

OUTPUT PARAMETERS
    XY      -   rows are filled with points: first NX columns with
                X-values, next NY columns - with Y-values.

NOTES
1. points are ordered by distance from the query point (first = closest)
2. if  XY is larger than required to store result, only leading part  will
   be overwritten; trailing part will be left unchanged. So  if  on  input
   XY = [[A,B],[C,D]], and result is [1,2],  then  on  exit  we  will  get
   XY = [[1,2],[C,D]]. This is done purposely to increase performance;  if
   you want function  to  resize  array  according  to  result  size,  use
   function with same name and suffix 'I'.

SEE ALSO
* KDTreeQueryResultsX()             X-values
* KDTreeQueryResultsTags()          tag values
* KDTreeQueryResultsDistances()     distances

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
void kdtreequeryresultsxy(kdtree* kdt,
     /* Real    */ ae_matrix* xy,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;


    if( kdt->kcur==0 )
    {
        return;
    }
    if( xy->rows<kdt->kcur||xy->cols<kdt->nx+kdt->ny )
    {
        ae_matrix_set_length(xy, kdt->kcur, kdt->nx+kdt->ny, _state);
    }
    k = kdt->kcur;
    for(i=0; i<=k-1; i++)
    {
        ae_v_move(&xy->ptr.pp_double[i][0], 1, &kdt->xy.ptr.pp_double[kdt->idx.ptr.p_int[i]][kdt->nx], 1, ae_v_len(0,kdt->nx+kdt->ny-1));
    }
}


/*************************************************************************
Tags from last query

INPUT PARAMETERS
    KDT     -   KD-tree
    Tags    -   possibly pre-allocated buffer. If X is too small to store
                result, it is resized. If size(X) is enough to store
                result, it is left unchanged.

OUTPUT PARAMETERS
    Tags    -   filled with tags associated with points,
                or, when no tags were supplied, with zeros

NOTES
1. points are ordered by distance from the query point (first = closest)
2. if  XY is larger than required to store result, only leading part  will
   be overwritten; trailing part will be left unchanged. So  if  on  input
   XY = [[A,B],[C,D]], and result is [1,2],  then  on  exit  we  will  get
   XY = [[1,2],[C,D]]. This is done purposely to increase performance;  if
   you want function  to  resize  array  according  to  result  size,  use
   function with same name and suffix 'I'.

SEE ALSO
* KDTreeQueryResultsX()             X-values
* KDTreeQueryResultsXY()            X- and Y-values
* KDTreeQueryResultsDistances()     distances

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
void kdtreequeryresultstags(kdtree* kdt,
     /* Integer */ ae_vector* tags,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;


    if( kdt->kcur==0 )
    {
        return;
    }
    if( tags->cnt<kdt->kcur )
    {
        ae_vector_set_length(tags, kdt->kcur, _state);
    }
    k = kdt->kcur;
    for(i=0; i<=k-1; i++)
    {
        tags->ptr.p_int[i] = kdt->tags.ptr.p_int[kdt->idx.ptr.p_int[i]];
    }
}


/*************************************************************************
Distances from last query

INPUT PARAMETERS
    KDT     -   KD-tree
    R       -   possibly pre-allocated buffer. If X is too small to store
                result, it is resized. If size(X) is enough to store
                result, it is left unchanged.

OUTPUT PARAMETERS
    R       -   filled with distances (in corresponding norm)

NOTES
1. points are ordered by distance from the query point (first = closest)
2. if  XY is larger than required to store result, only leading part  will
   be overwritten; trailing part will be left unchanged. So  if  on  input
   XY = [[A,B],[C,D]], and result is [1,2],  then  on  exit  we  will  get
   XY = [[1,2],[C,D]]. This is done purposely to increase performance;  if
   you want function  to  resize  array  according  to  result  size,  use
   function with same name and suffix 'I'.

SEE ALSO
* KDTreeQueryResultsX()             X-values
* KDTreeQueryResultsXY()            X- and Y-values
* KDTreeQueryResultsTags()          tag values

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
void kdtreequeryresultsdistances(kdtree* kdt,
     /* Real    */ ae_vector* r,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;


    if( kdt->kcur==0 )
    {
        return;
    }
    if( r->cnt<kdt->kcur )
    {
        ae_vector_set_length(r, kdt->kcur, _state);
    }
    k = kdt->kcur;
    
    /*
     * unload norms
     *
     * Abs() call is used to handle cases with negative norms
     * (generated during KFN requests)
     */
    if( kdt->normtype==0 )
    {
        for(i=0; i<=k-1; i++)
        {
            r->ptr.p_double[i] = ae_fabs(kdt->r.ptr.p_double[i], _state);
        }
    }
    if( kdt->normtype==1 )
    {
        for(i=0; i<=k-1; i++)
        {
            r->ptr.p_double[i] = ae_fabs(kdt->r.ptr.p_double[i], _state);
        }
    }
    if( kdt->normtype==2 )
    {
        for(i=0; i<=k-1; i++)
        {
            r->ptr.p_double[i] = ae_sqrt(ae_fabs(kdt->r.ptr.p_double[i], _state), _state);
        }
    }
}


/*************************************************************************
X-values from last query; 'interactive' variant for languages like  Python
which   support    constructs   like  "X = KDTreeQueryResultsXI(KDT)"  and
interactive mode of interpreter.

This function allocates new array on each call,  so  it  is  significantly
slower than its 'non-interactive' counterpart, but it is  more  convenient
when you call it from command line.

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
void kdtreequeryresultsxi(kdtree* kdt,
     /* Real    */ ae_matrix* x,
     ae_state *_state)
{

    ae_matrix_clear(x);

    kdtreequeryresultsx(kdt, x, _state);
}


/*************************************************************************
XY-values from last query; 'interactive' variant for languages like Python
which   support    constructs   like "XY = KDTreeQueryResultsXYI(KDT)" and
interactive mode of interpreter.

This function allocates new array on each call,  so  it  is  significantly
slower than its 'non-interactive' counterpart, but it is  more  convenient
when you call it from command line.

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
void kdtreequeryresultsxyi(kdtree* kdt,
     /* Real    */ ae_matrix* xy,
     ae_state *_state)
{

    ae_matrix_clear(xy);

    kdtreequeryresultsxy(kdt, xy, _state);
}


/*************************************************************************
Tags  from  last  query;  'interactive' variant for languages like  Python
which  support  constructs  like "Tags = KDTreeQueryResultsTagsI(KDT)" and
interactive mode of interpreter.

This function allocates new array on each call,  so  it  is  significantly
slower than its 'non-interactive' counterpart, but it is  more  convenient
when you call it from command line.

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
void kdtreequeryresultstagsi(kdtree* kdt,
     /* Integer */ ae_vector* tags,
     ae_state *_state)
{

    ae_vector_clear(tags);

    kdtreequeryresultstags(kdt, tags, _state);
}


/*************************************************************************
Distances from last query; 'interactive' variant for languages like Python
which  support  constructs   like  "R = KDTreeQueryResultsDistancesI(KDT)"
and interactive mode of interpreter.

This function allocates new array on each call,  so  it  is  significantly
slower than its 'non-interactive' counterpart, but it is  more  convenient
when you call it from command line.

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
void kdtreequeryresultsdistancesi(kdtree* kdt,
     /* Real    */ ae_vector* r,
     ae_state *_state)
{

    ae_vector_clear(r);

    kdtreequeryresultsdistances(kdt, r, _state);
}


/*************************************************************************
Serializer: allocation

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void kdtreealloc(ae_serializer* s, kdtree* tree, ae_state *_state)
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
    allocrealmatrix(s, &tree->xy, -1, -1, _state);
    allocintegerarray(s, &tree->tags, -1, _state);
    allocrealarray(s, &tree->boxmin, -1, _state);
    allocrealarray(s, &tree->boxmax, -1, _state);
    allocintegerarray(s, &tree->nodes, -1, _state);
    allocrealarray(s, &tree->splits, -1, _state);
}


/*************************************************************************
Serializer: serialization

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void kdtreeserialize(ae_serializer* s, kdtree* tree, ae_state *_state)
{


    
    /*
     * Header
     */
    ae_serializer_serialize_int(s, getkdtreeserializationcode(_state), _state);
    ae_serializer_serialize_int(s, nearestneighbor_kdtreefirstversion, _state);
    
    /*
     * Data
     */
    ae_serializer_serialize_int(s, tree->n, _state);
    ae_serializer_serialize_int(s, tree->nx, _state);
    ae_serializer_serialize_int(s, tree->ny, _state);
    ae_serializer_serialize_int(s, tree->normtype, _state);
    serializerealmatrix(s, &tree->xy, -1, -1, _state);
    serializeintegerarray(s, &tree->tags, -1, _state);
    serializerealarray(s, &tree->boxmin, -1, _state);
    serializerealarray(s, &tree->boxmax, -1, _state);
    serializeintegerarray(s, &tree->nodes, -1, _state);
    serializerealarray(s, &tree->splits, -1, _state);
}


/*************************************************************************
Serializer: unserialization

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void kdtreeunserialize(ae_serializer* s, kdtree* tree, ae_state *_state)
{
    ae_int_t i0;
    ae_int_t i1;

    _kdtree_clear(tree);

    
    /*
     * check correctness of header
     */
    ae_serializer_unserialize_int(s, &i0, _state);
    ae_assert(i0==getkdtreeserializationcode(_state), "KDTreeUnserialize: stream header corrupted", _state);
    ae_serializer_unserialize_int(s, &i1, _state);
    ae_assert(i1==nearestneighbor_kdtreefirstversion, "KDTreeUnserialize: stream header corrupted", _state);
    
    /*
     * Unserialize data
     */
    ae_serializer_unserialize_int(s, &tree->n, _state);
    ae_serializer_unserialize_int(s, &tree->nx, _state);
    ae_serializer_unserialize_int(s, &tree->ny, _state);
    ae_serializer_unserialize_int(s, &tree->normtype, _state);
    unserializerealmatrix(s, &tree->xy, _state);
    unserializeintegerarray(s, &tree->tags, _state);
    unserializerealarray(s, &tree->boxmin, _state);
    unserializerealarray(s, &tree->boxmax, _state);
    unserializeintegerarray(s, &tree->nodes, _state);
    unserializerealarray(s, &tree->splits, _state);
    nearestneighbor_kdtreealloctemporaries(tree, tree->n, tree->nx, tree->ny, _state);
}


/*************************************************************************
Rearranges nodes [I1,I2) using partition in D-th dimension with S as threshold.
Returns split position I3: [I1,I3) and [I3,I2) are created as result.

This subroutine doesn't create tree structures, just rearranges nodes.
*************************************************************************/
static void nearestneighbor_kdtreesplit(kdtree* kdt,
     ae_int_t i1,
     ae_int_t i2,
     ae_int_t d,
     double s,
     ae_int_t* i3,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t ileft;
    ae_int_t iright;
    double v;

    *i3 = 0;

    ae_assert(kdt->n>0, "KDTreeSplit: internal error", _state);
    
    /*
     * split XY/Tags in two parts:
     * * [ILeft,IRight] is non-processed part of XY/Tags
     *
     * After cycle is done, we have Ileft=IRight. We deal with
     * this element separately.
     *
     * After this, [I1,ILeft) contains left part, and [ILeft,I2)
     * contains right part.
     */
    ileft = i1;
    iright = i2-1;
    while(ileft<iright)
    {
        if( ae_fp_less_eq(kdt->xy.ptr.pp_double[ileft][d],s) )
        {
            
            /*
             * XY[ILeft] is on its place.
             * Advance ILeft.
             */
            ileft = ileft+1;
        }
        else
        {
            
            /*
             * XY[ILeft,..] must be at IRight.
             * Swap and advance IRight.
             */
            for(i=0; i<=2*kdt->nx+kdt->ny-1; i++)
            {
                v = kdt->xy.ptr.pp_double[ileft][i];
                kdt->xy.ptr.pp_double[ileft][i] = kdt->xy.ptr.pp_double[iright][i];
                kdt->xy.ptr.pp_double[iright][i] = v;
            }
            j = kdt->tags.ptr.p_int[ileft];
            kdt->tags.ptr.p_int[ileft] = kdt->tags.ptr.p_int[iright];
            kdt->tags.ptr.p_int[iright] = j;
            iright = iright-1;
        }
    }
    if( ae_fp_less_eq(kdt->xy.ptr.pp_double[ileft][d],s) )
    {
        ileft = ileft+1;
    }
    else
    {
        iright = iright-1;
    }
    *i3 = ileft;
}


/*************************************************************************
Recursive kd-tree generation subroutine.

PARAMETERS
    KDT         tree
    NodesOffs   unused part of Nodes[] which must be filled by tree
    SplitsOffs  unused part of Splits[]
    I1, I2      points from [I1,I2) are processed
    
NodesOffs[] and SplitsOffs[] must be large enough.

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
static void nearestneighbor_kdtreegeneratetreerec(kdtree* kdt,
     ae_int_t* nodesoffs,
     ae_int_t* splitsoffs,
     ae_int_t i1,
     ae_int_t i2,
     ae_int_t maxleafsize,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t nx;
    ae_int_t ny;
    ae_int_t i;
    ae_int_t j;
    ae_int_t oldoffs;
    ae_int_t i3;
    ae_int_t cntless;
    ae_int_t cntgreater;
    double minv;
    double maxv;
    ae_int_t minidx;
    ae_int_t maxidx;
    ae_int_t d;
    double ds;
    double s;
    double v;
    double v0;
    double v1;


    ae_assert(kdt->n>0, "KDTreeGenerateTreeRec: internal error", _state);
    ae_assert(i2>i1, "KDTreeGenerateTreeRec: internal error", _state);
    
    /*
     * Generate leaf if needed
     */
    if( i2-i1<=maxleafsize )
    {
        kdt->nodes.ptr.p_int[*nodesoffs+0] = i2-i1;
        kdt->nodes.ptr.p_int[*nodesoffs+1] = i1;
        *nodesoffs = *nodesoffs+2;
        return;
    }
    
    /*
     * Load values for easier access
     */
    nx = kdt->nx;
    ny = kdt->ny;
    
    /*
     * Select dimension to split:
     * * D is a dimension number
     * In case bounding box has zero size, we enforce creation of the leaf node.
     */
    d = 0;
    ds = kdt->curboxmax.ptr.p_double[0]-kdt->curboxmin.ptr.p_double[0];
    for(i=1; i<=nx-1; i++)
    {
        v = kdt->curboxmax.ptr.p_double[i]-kdt->curboxmin.ptr.p_double[i];
        if( ae_fp_greater(v,ds) )
        {
            ds = v;
            d = i;
        }
    }
    if( ae_fp_eq(ds,(double)(0)) )
    {
        kdt->nodes.ptr.p_int[*nodesoffs+0] = i2-i1;
        kdt->nodes.ptr.p_int[*nodesoffs+1] = i1;
        *nodesoffs = *nodesoffs+2;
        return;
    }
    
    /*
     * Select split position S using sliding midpoint rule,
     * rearrange points into [I1,I3) and [I3,I2).
     *
     * In case all points has same value of D-th component
     * (MinV=MaxV) we enforce D-th dimension of bounding
     * box to become exactly zero and repeat tree construction.
     */
    s = kdt->curboxmin.ptr.p_double[d]+0.5*ds;
    ae_v_move(&kdt->buf.ptr.p_double[0], 1, &kdt->xy.ptr.pp_double[i1][d], kdt->xy.stride, ae_v_len(0,i2-i1-1));
    n = i2-i1;
    cntless = 0;
    cntgreater = 0;
    minv = kdt->buf.ptr.p_double[0];
    maxv = kdt->buf.ptr.p_double[0];
    minidx = i1;
    maxidx = i1;
    for(i=0; i<=n-1; i++)
    {
        v = kdt->buf.ptr.p_double[i];
        if( ae_fp_less(v,minv) )
        {
            minv = v;
            minidx = i1+i;
        }
        if( ae_fp_greater(v,maxv) )
        {
            maxv = v;
            maxidx = i1+i;
        }
        if( ae_fp_less(v,s) )
        {
            cntless = cntless+1;
        }
        if( ae_fp_greater(v,s) )
        {
            cntgreater = cntgreater+1;
        }
    }
    if( ae_fp_eq(minv,maxv) )
    {
        
        /*
         * In case all points has same value of D-th component
         * (MinV=MaxV) we enforce D-th dimension of bounding
         * box to become exactly zero and repeat tree construction.
         */
        v0 = kdt->curboxmin.ptr.p_double[d];
        v1 = kdt->curboxmax.ptr.p_double[d];
        kdt->curboxmin.ptr.p_double[d] = minv;
        kdt->curboxmax.ptr.p_double[d] = maxv;
        nearestneighbor_kdtreegeneratetreerec(kdt, nodesoffs, splitsoffs, i1, i2, maxleafsize, _state);
        kdt->curboxmin.ptr.p_double[d] = v0;
        kdt->curboxmax.ptr.p_double[d] = v1;
        return;
    }
    if( cntless>0&&cntgreater>0 )
    {
        
        /*
         * normal midpoint split
         */
        nearestneighbor_kdtreesplit(kdt, i1, i2, d, s, &i3, _state);
    }
    else
    {
        
        /*
         * sliding midpoint
         */
        if( cntless==0 )
        {
            
            /*
             * 1. move split to MinV,
             * 2. place one point to the left bin (move to I1),
             *    others - to the right bin
             */
            s = minv;
            if( minidx!=i1 )
            {
                for(i=0; i<=2*nx+ny-1; i++)
                {
                    v = kdt->xy.ptr.pp_double[minidx][i];
                    kdt->xy.ptr.pp_double[minidx][i] = kdt->xy.ptr.pp_double[i1][i];
                    kdt->xy.ptr.pp_double[i1][i] = v;
                }
                j = kdt->tags.ptr.p_int[minidx];
                kdt->tags.ptr.p_int[minidx] = kdt->tags.ptr.p_int[i1];
                kdt->tags.ptr.p_int[i1] = j;
            }
            i3 = i1+1;
        }
        else
        {
            
            /*
             * 1. move split to MaxV,
             * 2. place one point to the right bin (move to I2-1),
             *    others - to the left bin
             */
            s = maxv;
            if( maxidx!=i2-1 )
            {
                for(i=0; i<=2*nx+ny-1; i++)
                {
                    v = kdt->xy.ptr.pp_double[maxidx][i];
                    kdt->xy.ptr.pp_double[maxidx][i] = kdt->xy.ptr.pp_double[i2-1][i];
                    kdt->xy.ptr.pp_double[i2-1][i] = v;
                }
                j = kdt->tags.ptr.p_int[maxidx];
                kdt->tags.ptr.p_int[maxidx] = kdt->tags.ptr.p_int[i2-1];
                kdt->tags.ptr.p_int[i2-1] = j;
            }
            i3 = i2-1;
        }
    }
    
    /*
     * Generate 'split' node
     */
    kdt->nodes.ptr.p_int[*nodesoffs+0] = 0;
    kdt->nodes.ptr.p_int[*nodesoffs+1] = d;
    kdt->nodes.ptr.p_int[*nodesoffs+2] = *splitsoffs;
    kdt->splits.ptr.p_double[*splitsoffs+0] = s;
    oldoffs = *nodesoffs;
    *nodesoffs = *nodesoffs+nearestneighbor_splitnodesize;
    *splitsoffs = *splitsoffs+1;
    
    /*
     * Recirsive generation:
     * * update CurBox
     * * call subroutine
     * * restore CurBox
     */
    kdt->nodes.ptr.p_int[oldoffs+3] = *nodesoffs;
    v = kdt->curboxmax.ptr.p_double[d];
    kdt->curboxmax.ptr.p_double[d] = s;
    nearestneighbor_kdtreegeneratetreerec(kdt, nodesoffs, splitsoffs, i1, i3, maxleafsize, _state);
    kdt->curboxmax.ptr.p_double[d] = v;
    kdt->nodes.ptr.p_int[oldoffs+4] = *nodesoffs;
    v = kdt->curboxmin.ptr.p_double[d];
    kdt->curboxmin.ptr.p_double[d] = s;
    nearestneighbor_kdtreegeneratetreerec(kdt, nodesoffs, splitsoffs, i3, i2, maxleafsize, _state);
    kdt->curboxmin.ptr.p_double[d] = v;
}


/*************************************************************************
Recursive subroutine for NN queries.

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
static void nearestneighbor_kdtreequerynnrec(kdtree* kdt,
     ae_int_t offs,
     ae_state *_state)
{
    double ptdist;
    ae_int_t i;
    ae_int_t j;
    ae_int_t nx;
    ae_int_t i1;
    ae_int_t i2;
    ae_int_t d;
    double s;
    double v;
    double t1;
    ae_int_t childbestoffs;
    ae_int_t childworstoffs;
    ae_int_t childoffs;
    double prevdist;
    ae_bool todive;
    ae_bool bestisleft;
    ae_bool updatemin;


    ae_assert(kdt->n>0, "KDTreeQueryNNRec: internal error", _state);
    
    /*
     * Leaf node.
     * Process points.
     */
    if( kdt->nodes.ptr.p_int[offs]>0 )
    {
        i1 = kdt->nodes.ptr.p_int[offs+1];
        i2 = i1+kdt->nodes.ptr.p_int[offs];
        for(i=i1; i<=i2-1; i++)
        {
            
            /*
             * Calculate distance
             */
            ptdist = (double)(0);
            nx = kdt->nx;
            if( kdt->normtype==0 )
            {
                for(j=0; j<=nx-1; j++)
                {
                    ptdist = ae_maxreal(ptdist, ae_fabs(kdt->xy.ptr.pp_double[i][j]-kdt->x.ptr.p_double[j], _state), _state);
                }
            }
            if( kdt->normtype==1 )
            {
                for(j=0; j<=nx-1; j++)
                {
                    ptdist = ptdist+ae_fabs(kdt->xy.ptr.pp_double[i][j]-kdt->x.ptr.p_double[j], _state);
                }
            }
            if( kdt->normtype==2 )
            {
                for(j=0; j<=nx-1; j++)
                {
                    ptdist = ptdist+ae_sqr(kdt->xy.ptr.pp_double[i][j]-kdt->x.ptr.p_double[j], _state);
                }
            }
            
            /*
             * Skip points with zero distance if self-matches are turned off
             */
            if( ae_fp_eq(ptdist,(double)(0))&&!kdt->selfmatch )
            {
                continue;
            }
            
            /*
             * We CAN'T process point if R-criterion isn't satisfied,
             * i.e. (RNeeded<>0) AND (PtDist>R).
             */
            if( ae_fp_eq(kdt->rneeded,(double)(0))||ae_fp_less_eq(ptdist,kdt->rneeded) )
            {
                
                /*
                 * R-criterion is satisfied, we must either:
                 * * replace worst point, if (KNeeded<>0) AND (KCur=KNeeded)
                 *   (or skip, if worst point is better)
                 * * add point without replacement otherwise
                 */
                if( kdt->kcur<kdt->kneeded||kdt->kneeded==0 )
                {
                    
                    /*
                     * add current point to heap without replacement
                     */
                    tagheappushi(&kdt->r, &kdt->idx, &kdt->kcur, ptdist, i, _state);
                }
                else
                {
                    
                    /*
                     * New points are added or not, depending on their distance.
                     * If added, they replace element at the top of the heap
                     */
                    if( ae_fp_less(ptdist,kdt->r.ptr.p_double[0]) )
                    {
                        if( kdt->kneeded==1 )
                        {
                            kdt->idx.ptr.p_int[0] = i;
                            kdt->r.ptr.p_double[0] = ptdist;
                        }
                        else
                        {
                            tagheapreplacetopi(&kdt->r, &kdt->idx, kdt->kneeded, ptdist, i, _state);
                        }
                    }
                }
            }
        }
        return;
    }
    
    /*
     * Simple split
     */
    if( kdt->nodes.ptr.p_int[offs]==0 )
    {
        
        /*
         * Load:
         * * D  dimension to split
         * * S  split position
         */
        d = kdt->nodes.ptr.p_int[offs+1];
        s = kdt->splits.ptr.p_double[kdt->nodes.ptr.p_int[offs+2]];
        
        /*
         * Calculate:
         * * ChildBestOffs      child box with best chances
         * * ChildWorstOffs     child box with worst chances
         */
        if( ae_fp_less_eq(kdt->x.ptr.p_double[d],s) )
        {
            childbestoffs = kdt->nodes.ptr.p_int[offs+3];
            childworstoffs = kdt->nodes.ptr.p_int[offs+4];
            bestisleft = ae_true;
        }
        else
        {
            childbestoffs = kdt->nodes.ptr.p_int[offs+4];
            childworstoffs = kdt->nodes.ptr.p_int[offs+3];
            bestisleft = ae_false;
        }
        
        /*
         * Navigate through childs
         */
        for(i=0; i<=1; i++)
        {
            
            /*
             * Select child to process:
             * * ChildOffs      current child offset in Nodes[]
             * * UpdateMin      whether minimum or maximum value
             *                  of bounding box is changed on update
             */
            if( i==0 )
            {
                childoffs = childbestoffs;
                updatemin = !bestisleft;
            }
            else
            {
                updatemin = bestisleft;
                childoffs = childworstoffs;
            }
            
            /*
             * Update bounding box and current distance
             */
            if( updatemin )
            {
                prevdist = kdt->curdist;
                t1 = kdt->x.ptr.p_double[d];
                v = kdt->curboxmin.ptr.p_double[d];
                if( ae_fp_less_eq(t1,s) )
                {
                    if( kdt->normtype==0 )
                    {
                        kdt->curdist = ae_maxreal(kdt->curdist, s-t1, _state);
                    }
                    if( kdt->normtype==1 )
                    {
                        kdt->curdist = kdt->curdist-ae_maxreal(v-t1, (double)(0), _state)+s-t1;
                    }
                    if( kdt->normtype==2 )
                    {
                        kdt->curdist = kdt->curdist-ae_sqr(ae_maxreal(v-t1, (double)(0), _state), _state)+ae_sqr(s-t1, _state);
                    }
                }
                kdt->curboxmin.ptr.p_double[d] = s;
            }
            else
            {
                prevdist = kdt->curdist;
                t1 = kdt->x.ptr.p_double[d];
                v = kdt->curboxmax.ptr.p_double[d];
                if( ae_fp_greater_eq(t1,s) )
                {
                    if( kdt->normtype==0 )
                    {
                        kdt->curdist = ae_maxreal(kdt->curdist, t1-s, _state);
                    }
                    if( kdt->normtype==1 )
                    {
                        kdt->curdist = kdt->curdist-ae_maxreal(t1-v, (double)(0), _state)+t1-s;
                    }
                    if( kdt->normtype==2 )
                    {
                        kdt->curdist = kdt->curdist-ae_sqr(ae_maxreal(t1-v, (double)(0), _state), _state)+ae_sqr(t1-s, _state);
                    }
                }
                kdt->curboxmax.ptr.p_double[d] = s;
            }
            
            /*
             * Decide: to dive into cell or not to dive
             */
            if( ae_fp_neq(kdt->rneeded,(double)(0))&&ae_fp_greater(kdt->curdist,kdt->rneeded) )
            {
                todive = ae_false;
            }
            else
            {
                if( kdt->kcur<kdt->kneeded||kdt->kneeded==0 )
                {
                    
                    /*
                     * KCur<KNeeded (i.e. not all points are found)
                     */
                    todive = ae_true;
                }
                else
                {
                    
                    /*
                     * KCur=KNeeded, decide to dive or not to dive
                     * using point position relative to bounding box.
                     */
                    todive = ae_fp_less_eq(kdt->curdist,kdt->r.ptr.p_double[0]*kdt->approxf);
                }
            }
            if( todive )
            {
                nearestneighbor_kdtreequerynnrec(kdt, childoffs, _state);
            }
            
            /*
             * Restore bounding box and distance
             */
            if( updatemin )
            {
                kdt->curboxmin.ptr.p_double[d] = v;
            }
            else
            {
                kdt->curboxmax.ptr.p_double[d] = v;
            }
            kdt->curdist = prevdist;
        }
        return;
    }
}


/*************************************************************************
Copies X[] to KDT.X[]
Loads distance from X[] to bounding box.
Initializes CurBox[].

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
static void nearestneighbor_kdtreeinitbox(kdtree* kdt,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t i;
    double vx;
    double vmin;
    double vmax;


    ae_assert(kdt->n>0, "KDTreeInitBox: internal error", _state);
    
    /*
     * calculate distance from point to current bounding box
     */
    kdt->curdist = (double)(0);
    if( kdt->normtype==0 )
    {
        for(i=0; i<=kdt->nx-1; i++)
        {
            vx = x->ptr.p_double[i];
            vmin = kdt->boxmin.ptr.p_double[i];
            vmax = kdt->boxmax.ptr.p_double[i];
            kdt->x.ptr.p_double[i] = vx;
            kdt->curboxmin.ptr.p_double[i] = vmin;
            kdt->curboxmax.ptr.p_double[i] = vmax;
            if( ae_fp_less(vx,vmin) )
            {
                kdt->curdist = ae_maxreal(kdt->curdist, vmin-vx, _state);
            }
            else
            {
                if( ae_fp_greater(vx,vmax) )
                {
                    kdt->curdist = ae_maxreal(kdt->curdist, vx-vmax, _state);
                }
            }
        }
    }
    if( kdt->normtype==1 )
    {
        for(i=0; i<=kdt->nx-1; i++)
        {
            vx = x->ptr.p_double[i];
            vmin = kdt->boxmin.ptr.p_double[i];
            vmax = kdt->boxmax.ptr.p_double[i];
            kdt->x.ptr.p_double[i] = vx;
            kdt->curboxmin.ptr.p_double[i] = vmin;
            kdt->curboxmax.ptr.p_double[i] = vmax;
            if( ae_fp_less(vx,vmin) )
            {
                kdt->curdist = kdt->curdist+vmin-vx;
            }
            else
            {
                if( ae_fp_greater(vx,vmax) )
                {
                    kdt->curdist = kdt->curdist+vx-vmax;
                }
            }
        }
    }
    if( kdt->normtype==2 )
    {
        for(i=0; i<=kdt->nx-1; i++)
        {
            vx = x->ptr.p_double[i];
            vmin = kdt->boxmin.ptr.p_double[i];
            vmax = kdt->boxmax.ptr.p_double[i];
            kdt->x.ptr.p_double[i] = vx;
            kdt->curboxmin.ptr.p_double[i] = vmin;
            kdt->curboxmax.ptr.p_double[i] = vmax;
            if( ae_fp_less(vx,vmin) )
            {
                kdt->curdist = kdt->curdist+ae_sqr(vmin-vx, _state);
            }
            else
            {
                if( ae_fp_greater(vx,vmax) )
                {
                    kdt->curdist = kdt->curdist+ae_sqr(vx-vmax, _state);
                }
            }
        }
    }
}


/*************************************************************************
This function allocates all dataset-independent array  fields  of  KDTree,
i.e.  such  array  fields  that  their dimensions do not depend on dataset
size.

This function do not sets KDT.NX or KDT.NY - it just allocates arrays

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
static void nearestneighbor_kdtreeallocdatasetindependent(kdtree* kdt,
     ae_int_t nx,
     ae_int_t ny,
     ae_state *_state)
{


    ae_assert(kdt->n>0, "KDTreeAllocDatasetIndependent: internal error", _state);
    ae_vector_set_length(&kdt->x, nx, _state);
    ae_vector_set_length(&kdt->boxmin, nx, _state);
    ae_vector_set_length(&kdt->boxmax, nx, _state);
    ae_vector_set_length(&kdt->curboxmin, nx, _state);
    ae_vector_set_length(&kdt->curboxmax, nx, _state);
}


/*************************************************************************
This function allocates all dataset-dependent array fields of KDTree, i.e.
such array fields that their dimensions depend on dataset size.

This function do not sets KDT.N, KDT.NX or KDT.NY -
it just allocates arrays.

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
static void nearestneighbor_kdtreeallocdatasetdependent(kdtree* kdt,
     ae_int_t n,
     ae_int_t nx,
     ae_int_t ny,
     ae_state *_state)
{


    ae_assert(n>0, "KDTreeAllocDatasetDependent: internal error", _state);
    ae_matrix_set_length(&kdt->xy, n, 2*nx+ny, _state);
    ae_vector_set_length(&kdt->tags, n, _state);
    ae_vector_set_length(&kdt->idx, n, _state);
    ae_vector_set_length(&kdt->r, n, _state);
    ae_vector_set_length(&kdt->x, nx, _state);
    ae_vector_set_length(&kdt->buf, ae_maxint(n, nx, _state), _state);
    ae_vector_set_length(&kdt->nodes, nearestneighbor_splitnodesize*2*n, _state);
    ae_vector_set_length(&kdt->splits, 2*n, _state);
}


/*************************************************************************
This function allocates temporaries.

This function do not sets KDT.N, KDT.NX or KDT.NY -
it just allocates arrays.

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
static void nearestneighbor_kdtreealloctemporaries(kdtree* kdt,
     ae_int_t n,
     ae_int_t nx,
     ae_int_t ny,
     ae_state *_state)
{


    ae_assert(n>0, "KDTreeAllocTemporaries: internal error", _state);
    ae_vector_set_length(&kdt->x, nx, _state);
    ae_vector_set_length(&kdt->idx, n, _state);
    ae_vector_set_length(&kdt->r, n, _state);
    ae_vector_set_length(&kdt->buf, ae_maxint(n, nx, _state), _state);
    ae_vector_set_length(&kdt->curboxmin, nx, _state);
    ae_vector_set_length(&kdt->curboxmax, nx, _state);
}


void _kdtree_init(void* _p, ae_state *_state)
{
    kdtree *p = (kdtree*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_init(&p->xy, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->tags, 0, DT_INT, _state);
    ae_vector_init(&p->boxmin, 0, DT_REAL, _state);
    ae_vector_init(&p->boxmax, 0, DT_REAL, _state);
    ae_vector_init(&p->nodes, 0, DT_INT, _state);
    ae_vector_init(&p->splits, 0, DT_REAL, _state);
    ae_vector_init(&p->x, 0, DT_REAL, _state);
    ae_vector_init(&p->idx, 0, DT_INT, _state);
    ae_vector_init(&p->r, 0, DT_REAL, _state);
    ae_vector_init(&p->buf, 0, DT_REAL, _state);
    ae_vector_init(&p->curboxmin, 0, DT_REAL, _state);
    ae_vector_init(&p->curboxmax, 0, DT_REAL, _state);
}


void _kdtree_init_copy(void* _dst, void* _src, ae_state *_state)
{
    kdtree *dst = (kdtree*)_dst;
    kdtree *src = (kdtree*)_src;
    dst->n = src->n;
    dst->nx = src->nx;
    dst->ny = src->ny;
    dst->normtype = src->normtype;
    ae_matrix_init_copy(&dst->xy, &src->xy, _state);
    ae_vector_init_copy(&dst->tags, &src->tags, _state);
    ae_vector_init_copy(&dst->boxmin, &src->boxmin, _state);
    ae_vector_init_copy(&dst->boxmax, &src->boxmax, _state);
    ae_vector_init_copy(&dst->nodes, &src->nodes, _state);
    ae_vector_init_copy(&dst->splits, &src->splits, _state);
    ae_vector_init_copy(&dst->x, &src->x, _state);
    dst->kneeded = src->kneeded;
    dst->rneeded = src->rneeded;
    dst->selfmatch = src->selfmatch;
    dst->approxf = src->approxf;
    dst->kcur = src->kcur;
    ae_vector_init_copy(&dst->idx, &src->idx, _state);
    ae_vector_init_copy(&dst->r, &src->r, _state);
    ae_vector_init_copy(&dst->buf, &src->buf, _state);
    ae_vector_init_copy(&dst->curboxmin, &src->curboxmin, _state);
    ae_vector_init_copy(&dst->curboxmax, &src->curboxmax, _state);
    dst->curdist = src->curdist;
    dst->debugcounter = src->debugcounter;
}


void _kdtree_clear(void* _p)
{
    kdtree *p = (kdtree*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_clear(&p->xy);
    ae_vector_clear(&p->tags);
    ae_vector_clear(&p->boxmin);
    ae_vector_clear(&p->boxmax);
    ae_vector_clear(&p->nodes);
    ae_vector_clear(&p->splits);
    ae_vector_clear(&p->x);
    ae_vector_clear(&p->idx);
    ae_vector_clear(&p->r);
    ae_vector_clear(&p->buf);
    ae_vector_clear(&p->curboxmin);
    ae_vector_clear(&p->curboxmax);
}


void _kdtree_destroy(void* _p)
{
    kdtree *p = (kdtree*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_destroy(&p->xy);
    ae_vector_destroy(&p->tags);
    ae_vector_destroy(&p->boxmin);
    ae_vector_destroy(&p->boxmax);
    ae_vector_destroy(&p->nodes);
    ae_vector_destroy(&p->splits);
    ae_vector_destroy(&p->x);
    ae_vector_destroy(&p->idx);
    ae_vector_destroy(&p->r);
    ae_vector_destroy(&p->buf);
    ae_vector_destroy(&p->curboxmin);
    ae_vector_destroy(&p->curboxmax);
}


/*$ End $*/
