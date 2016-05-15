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
#include "lsfit.h"


/*$ Declarations $*/
static void lsfit_rdpanalyzesection(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t i0,
     ae_int_t i1,
     ae_int_t* worstidx,
     double* worsterror,
     ae_state *_state);
static void lsfit_rdprecursive(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t i0,
     ae_int_t i1,
     double eps,
     /* Real    */ ae_vector* xout,
     /* Real    */ ae_vector* yout,
     ae_int_t* nout,
     ae_state *_state);
static void lsfit_logisticfitinternal(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_bool is4pl,
     double lambdav,
     minlmstate* state,
     minlmreport* replm,
     /* Real    */ ae_vector* p1,
     double* flast,
     ae_state *_state);
static void lsfit_spline1dfitinternal(ae_int_t st,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     ae_int_t n,
     /* Real    */ ae_vector* xc,
     /* Real    */ ae_vector* yc,
     /* Integer */ ae_vector* dc,
     ae_int_t k,
     ae_int_t m,
     ae_int_t* info,
     spline1dinterpolant* s,
     spline1dfitreport* rep,
     ae_state *_state);
static void lsfit_lsfitlinearinternal(/* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* fmatrix,
     ae_int_t n,
     ae_int_t m,
     ae_int_t* info,
     /* Real    */ ae_vector* c,
     lsfitreport* rep,
     ae_state *_state);
static void lsfit_lsfitclearrequestfields(lsfitstate* state,
     ae_state *_state);
static void lsfit_barycentriccalcbasis(barycentricinterpolant* b,
     double t,
     /* Real    */ ae_vector* y,
     ae_state *_state);
static void lsfit_internalchebyshevfit(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     ae_int_t n,
     /* Real    */ ae_vector* xc,
     /* Real    */ ae_vector* yc,
     /* Integer */ ae_vector* dc,
     ae_int_t k,
     ae_int_t m,
     ae_int_t* info,
     /* Real    */ ae_vector* c,
     lsfitreport* rep,
     ae_state *_state);
static void lsfit_barycentricfitwcfixedd(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     ae_int_t n,
     /* Real    */ ae_vector* xc,
     /* Real    */ ae_vector* yc,
     /* Integer */ ae_vector* dc,
     ae_int_t k,
     ae_int_t m,
     ae_int_t d,
     ae_int_t* info,
     barycentricinterpolant* b,
     barycentricfitreport* rep,
     ae_state *_state);
static void lsfit_clearreport(lsfitreport* rep, ae_state *_state);
static void lsfit_estimateerrors(/* Real    */ ae_matrix* f1,
     /* Real    */ ae_vector* f0,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* s,
     ae_int_t n,
     ae_int_t k,
     lsfitreport* rep,
     /* Real    */ ae_matrix* z,
     ae_int_t zkind,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
This  subroutine fits piecewise linear curve to points with Ramer-Douglas-
Peucker algorithm, which stops after generating specified number of linear
sections.

IMPORTANT:
* it does NOT perform least-squares fitting; it  builds  curve,  but  this
  curve does not minimize some least squares metric.  See  description  of
  RDP algorithm (say, in Wikipedia) for more details on WHAT is performed.
* this function does NOT work with parametric curves  (i.e.  curves  which
  can be represented as {X(t),Y(t)}. It works with curves   which  can  be
  represented as Y(X). Thus,  it  is  impossible  to  model  figures  like
  circles  with  this  functions.
  If  you  want  to  work  with  parametric   curves,   you   should   use
  ParametricRDPFixed() function provided  by  "Parametric"  subpackage  of
  "Interpolation" package.

INPUT PARAMETERS:
    X       -   array of X-coordinates:
                * at least N elements
                * can be unordered (points are automatically sorted)
                * this function may accept non-distinct X (see below for
                  more information on handling of such inputs)
    Y       -   array of Y-coordinates:
                * at least N elements
    N       -   number of elements in X/Y
    M       -   desired number of sections:
                * at most M sections are generated by this function
                * less than M sections can be generated if we have N<M
                  (or some X are non-distinct).

OUTPUT PARAMETERS:
    X2      -   X-values of corner points for piecewise approximation,
                has length NSections+1 or zero (for NSections=0).
    Y2      -   Y-values of corner points,
                has length NSections+1 or zero (for NSections=0).
    NSections-  number of sections found by algorithm, NSections<=M,
                NSections can be zero for degenerate datasets
                (N<=1 or all X[] are non-distinct).

NOTE: X2/Y2 are ordered arrays, i.e. (X2[0],Y2[0]) is  a  first  point  of
      curve, (X2[NSection-1],Y2[NSection-1]) is the last point.

  -- ALGLIB --
     Copyright 02.10.2014 by Bochkanov Sergey
*************************************************************************/
void lstfitpiecewiselinearrdpfixed(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t m,
     /* Real    */ ae_vector* x2,
     /* Real    */ ae_vector* y2,
     ae_int_t* nsections,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t k0;
    ae_int_t k1;
    ae_int_t k2;
    ae_vector buf0;
    ae_vector buf1;
    ae_matrix sections;
    ae_vector points;
    double v;
    ae_int_t worstidx;
    double worsterror;
    ae_int_t idx0;
    ae_int_t idx1;
    double e0;
    double e1;
    ae_vector heaperrors;
    ae_vector heaptags;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    ae_vector_clear(x2);
    ae_vector_clear(y2);
    *nsections = 0;
    ae_vector_init(&buf0, 0, DT_REAL, _state);
    ae_vector_init(&buf1, 0, DT_REAL, _state);
    ae_matrix_init(&sections, 0, 0, DT_REAL, _state);
    ae_vector_init(&points, 0, DT_REAL, _state);
    ae_vector_init(&heaperrors, 0, DT_REAL, _state);
    ae_vector_init(&heaptags, 0, DT_INT, _state);

    ae_assert(n>=0, "LSTFitPiecewiseLinearRDPFixed: N<0", _state);
    ae_assert(m>=1, "LSTFitPiecewiseLinearRDPFixed: M<1", _state);
    ae_assert(x->cnt>=n, "LSTFitPiecewiseLinearRDPFixed: Length(X)<N", _state);
    ae_assert(y->cnt>=n, "LSTFitPiecewiseLinearRDPFixed: Length(Y)<N", _state);
    if( n<=1 )
    {
        *nsections = 0;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Sort points.
     * Handle possible ties (tied values are replaced by their mean)
     */
    tagsortfastr(x, y, &buf0, &buf1, n, _state);
    i = 0;
    while(i<=n-1)
    {
        j = i+1;
        v = y->ptr.p_double[i];
        while(j<=n-1&&ae_fp_eq(x->ptr.p_double[j],x->ptr.p_double[i]))
        {
            v = v+y->ptr.p_double[j];
            j = j+1;
        }
        v = v/(j-i);
        for(k=i; k<=j-1; k++)
        {
            y->ptr.p_double[k] = v;
        }
        i = j;
    }
    
    /*
     * Handle degenerate case x[0]=x[N-1]
     */
    if( ae_fp_eq(x->ptr.p_double[n-1],x->ptr.p_double[0]) )
    {
        *nsections = 0;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Prepare first section
     */
    lsfit_rdpanalyzesection(x, y, 0, n-1, &worstidx, &worsterror, _state);
    ae_matrix_set_length(&sections, m, 4, _state);
    ae_vector_set_length(&heaperrors, m, _state);
    ae_vector_set_length(&heaptags, m, _state);
    *nsections = 1;
    sections.ptr.pp_double[0][0] = (double)(0);
    sections.ptr.pp_double[0][1] = (double)(n-1);
    sections.ptr.pp_double[0][2] = (double)(worstidx);
    sections.ptr.pp_double[0][3] = worsterror;
    heaperrors.ptr.p_double[0] = worsterror;
    heaptags.ptr.p_int[0] = 0;
    ae_assert(ae_fp_eq(sections.ptr.pp_double[0][1],(double)(n-1)), "RDP algorithm: integrity check failed", _state);
    
    /*
     * Main loop.
     * Repeatedly find section with worst error and divide it.
     * Terminate after M-th section, or because of other reasons (see loop internals).
     */
    while(*nsections<m)
    {
        
        /*
         * Break if worst section has zero error.
         * Store index of worst section to K.
         */
        if( ae_fp_eq(heaperrors.ptr.p_double[0],(double)(0)) )
        {
            break;
        }
        k = heaptags.ptr.p_int[0];
        
        /*
         * K-th section is divided in two:
         * * first  one spans interval from X[Sections[K,0]] to X[Sections[K,2]]
         * * second one spans interval from X[Sections[K,2]] to X[Sections[K,1]]
         *
         * First section is stored at K-th position, second one is appended to the table.
         * Then we update heap which stores pairs of (error,section_index)
         */
        k0 = ae_round(sections.ptr.pp_double[k][0], _state);
        k1 = ae_round(sections.ptr.pp_double[k][1], _state);
        k2 = ae_round(sections.ptr.pp_double[k][2], _state);
        lsfit_rdpanalyzesection(x, y, k0, k2, &idx0, &e0, _state);
        lsfit_rdpanalyzesection(x, y, k2, k1, &idx1, &e1, _state);
        sections.ptr.pp_double[k][0] = (double)(k0);
        sections.ptr.pp_double[k][1] = (double)(k2);
        sections.ptr.pp_double[k][2] = (double)(idx0);
        sections.ptr.pp_double[k][3] = e0;
        tagheapreplacetopi(&heaperrors, &heaptags, *nsections, e0, k, _state);
        sections.ptr.pp_double[*nsections][0] = (double)(k2);
        sections.ptr.pp_double[*nsections][1] = (double)(k1);
        sections.ptr.pp_double[*nsections][2] = (double)(idx1);
        sections.ptr.pp_double[*nsections][3] = e1;
        tagheappushi(&heaperrors, &heaptags, nsections, e1, *nsections, _state);
    }
    
    /*
     * Convert from sections to points
     */
    ae_vector_set_length(&points, *nsections+1, _state);
    k = ae_round(sections.ptr.pp_double[0][1], _state);
    for(i=0; i<=*nsections-1; i++)
    {
        points.ptr.p_double[i] = (double)(ae_round(sections.ptr.pp_double[i][0], _state));
        if( ae_fp_greater(x->ptr.p_double[ae_round(sections.ptr.pp_double[i][1], _state)],x->ptr.p_double[k]) )
        {
            k = ae_round(sections.ptr.pp_double[i][1], _state);
        }
    }
    points.ptr.p_double[*nsections] = (double)(k);
    tagsortfast(&points, &buf0, *nsections+1, _state);
    
    /*
     * Output sections:
     * * first NSection elements of X2/Y2 are filled by x/y at left boundaries of sections
     * * last element of X2/Y2 is filled by right boundary of rightmost section
     * * X2/Y2 is sorted by ascending of X2
     */
    ae_vector_set_length(x2, *nsections+1, _state);
    ae_vector_set_length(y2, *nsections+1, _state);
    for(i=0; i<=*nsections; i++)
    {
        x2->ptr.p_double[i] = x->ptr.p_double[ae_round(points.ptr.p_double[i], _state)];
        y2->ptr.p_double[i] = y->ptr.p_double[ae_round(points.ptr.p_double[i], _state)];
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This  subroutine fits piecewise linear curve to points with Ramer-Douglas-
Peucker algorithm, which stops after achieving desired precision.

IMPORTANT:
* it performs non-least-squares fitting; it builds curve, but  this  curve
  does not minimize some least squares  metric.  See  description  of  RDP
  algorithm (say, in Wikipedia) for more details on WHAT is performed.
* this function does NOT work with parametric curves  (i.e.  curves  which
  can be represented as {X(t),Y(t)}. It works with curves   which  can  be
  represented as Y(X). Thus, it is impossible to model figures like circles
  with this functions.
  If  you  want  to  work  with  parametric   curves,   you   should   use
  ParametricRDPFixed() function provided  by  "Parametric"  subpackage  of
  "Interpolation" package.

INPUT PARAMETERS:
    X       -   array of X-coordinates:
                * at least N elements
                * can be unordered (points are automatically sorted)
                * this function may accept non-distinct X (see below for
                  more information on handling of such inputs)
    Y       -   array of Y-coordinates:
                * at least N elements
    N       -   number of elements in X/Y
    Eps     -   positive number, desired precision.
    

OUTPUT PARAMETERS:
    X2      -   X-values of corner points for piecewise approximation,
                has length NSections+1 or zero (for NSections=0).
    Y2      -   Y-values of corner points,
                has length NSections+1 or zero (for NSections=0).
    NSections-  number of sections found by algorithm,
                NSections can be zero for degenerate datasets
                (N<=1 or all X[] are non-distinct).

NOTE: X2/Y2 are ordered arrays, i.e. (X2[0],Y2[0]) is  a  first  point  of
      curve, (X2[NSection-1],Y2[NSection-1]) is the last point.

  -- ALGLIB --
     Copyright 02.10.2014 by Bochkanov Sergey
*************************************************************************/
void lstfitpiecewiselinearrdp(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     double eps,
     /* Real    */ ae_vector* x2,
     /* Real    */ ae_vector* y2,
     ae_int_t* nsections,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_vector buf0;
    ae_vector buf1;
    ae_vector xtmp;
    ae_vector ytmp;
    double v;
    ae_int_t npts;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    ae_vector_clear(x2);
    ae_vector_clear(y2);
    *nsections = 0;
    ae_vector_init(&buf0, 0, DT_REAL, _state);
    ae_vector_init(&buf1, 0, DT_REAL, _state);
    ae_vector_init(&xtmp, 0, DT_REAL, _state);
    ae_vector_init(&ytmp, 0, DT_REAL, _state);

    ae_assert(n>=0, "LSTFitPiecewiseLinearRDP: N<0", _state);
    ae_assert(ae_fp_greater(eps,(double)(0)), "LSTFitPiecewiseLinearRDP: Eps<=0", _state);
    ae_assert(x->cnt>=n, "LSTFitPiecewiseLinearRDP: Length(X)<N", _state);
    ae_assert(y->cnt>=n, "LSTFitPiecewiseLinearRDP: Length(Y)<N", _state);
    if( n<=1 )
    {
        *nsections = 0;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Sort points.
     * Handle possible ties (tied values are replaced by their mean)
     */
    tagsortfastr(x, y, &buf0, &buf1, n, _state);
    i = 0;
    while(i<=n-1)
    {
        j = i+1;
        v = y->ptr.p_double[i];
        while(j<=n-1&&ae_fp_eq(x->ptr.p_double[j],x->ptr.p_double[i]))
        {
            v = v+y->ptr.p_double[j];
            j = j+1;
        }
        v = v/(j-i);
        for(k=i; k<=j-1; k++)
        {
            y->ptr.p_double[k] = v;
        }
        i = j;
    }
    
    /*
     * Handle degenerate case x[0]=x[N-1]
     */
    if( ae_fp_eq(x->ptr.p_double[n-1],x->ptr.p_double[0]) )
    {
        *nsections = 0;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Prepare data for recursive algorithm
     */
    ae_vector_set_length(&xtmp, n, _state);
    ae_vector_set_length(&ytmp, n, _state);
    npts = 2;
    xtmp.ptr.p_double[0] = x->ptr.p_double[0];
    ytmp.ptr.p_double[0] = y->ptr.p_double[0];
    xtmp.ptr.p_double[1] = x->ptr.p_double[n-1];
    ytmp.ptr.p_double[1] = y->ptr.p_double[n-1];
    lsfit_rdprecursive(x, y, 0, n-1, eps, &xtmp, &ytmp, &npts, _state);
    
    /*
     * Output sections:
     * * first NSection elements of X2/Y2 are filled by x/y at left boundaries of sections
     * * last element of X2/Y2 is filled by right boundary of rightmost section
     * * X2/Y2 is sorted by ascending of X2
     */
    *nsections = npts-1;
    ae_vector_set_length(x2, npts, _state);
    ae_vector_set_length(y2, npts, _state);
    for(i=0; i<=*nsections; i++)
    {
        x2->ptr.p_double[i] = xtmp.ptr.p_double[i];
        y2->ptr.p_double[i] = ytmp.ptr.p_double[i];
    }
    tagsortfastr(x2, y2, &buf0, &buf1, npts, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Fitting by polynomials in barycentric form. This function provides  simple
unterface for unconstrained unweighted fitting. See  PolynomialFitWC()  if
you need constrained fitting.

Task is linear, so linear least squares solver is used. Complexity of this
computational scheme is O(N*M^2), mostly dominated by least squares solver

SEE ALSO:
    PolynomialFitWC()
    
COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multithreading support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Speed-up provided by multithreading greatly depends  on  problem  size
  ! - only large problems (number of coefficients is more than 500) can be
  ! efficiently multithreaded.
  !
  ! Generally, commercial ALGLIB is several times faster than  open-source
  ! generic C edition, and many times faster than open-source C# edition.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS:
    X   -   points, array[0..N-1].
    Y   -   function values, array[0..N-1].
    N   -   number of points, N>0
            * if given, only leading N elements of X/Y are used
            * if not given, automatically determined from sizes of X/Y
    M   -   number of basis functions (= polynomial_degree + 1), M>=1

OUTPUT PARAMETERS:
    Info-   same format as in LSFitLinearW() subroutine:
            * Info>0    task is solved
            * Info<=0   an error occured:
                        -4 means inconvergence of internal SVD
    P   -   interpolant in barycentric form.
    Rep -   report, same format as in LSFitLinearW() subroutine.
            Following fields are set:
            * RMSError      rms error on the (X,Y).
            * AvgError      average error on the (X,Y).
            * AvgRelError   average relative error on the non-zero Y
            * MaxError      maximum error
                            NON-WEIGHTED ERRORS ARE CALCULATED

NOTES:
    you can convert P from barycentric form  to  the  power  or  Chebyshev
    basis with PolynomialBar2Pow() or PolynomialBar2Cheb() functions  from
    POLINT subpackage.

  -- ALGLIB PROJECT --
     Copyright 10.12.2009 by Bochkanov Sergey
*************************************************************************/
void polynomialfit(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t m,
     ae_int_t* info,
     barycentricinterpolant* p,
     polynomialfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_vector w;
    ae_vector xc;
    ae_vector yc;
    ae_vector dc;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _barycentricinterpolant_clear(p);
    _polynomialfitreport_clear(rep);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&xc, 0, DT_REAL, _state);
    ae_vector_init(&yc, 0, DT_REAL, _state);
    ae_vector_init(&dc, 0, DT_INT, _state);

    ae_assert(n>0, "PolynomialFit: N<=0!", _state);
    ae_assert(m>0, "PolynomialFit: M<=0!", _state);
    ae_assert(x->cnt>=n, "PolynomialFit: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "PolynomialFit: Length(Y)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "PolynomialFit: X contains infinite or NaN values!", _state);
    ae_assert(isfinitevector(y, n, _state), "PolynomialFit: Y contains infinite or NaN values!", _state);
    ae_vector_set_length(&w, n, _state);
    for(i=0; i<=n-1; i++)
    {
        w.ptr.p_double[i] = (double)(1);
    }
    polynomialfitwc(x, y, &w, n, &xc, &yc, &dc, 0, m, info, p, rep, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_polynomialfit(/* Real    */ ae_vector* x,
    /* Real    */ ae_vector* y,
    ae_int_t n,
    ae_int_t m,
    ae_int_t* info,
    barycentricinterpolant* p,
    polynomialfitreport* rep, ae_state *_state)
{
    polynomialfit(x,y,n,m,info,p,rep, _state);
}


/*************************************************************************
Weighted  fitting by polynomials in barycentric form, with constraints  on
function values or first derivatives.

Small regularizing term is used when solving constrained tasks (to improve
stability).

Task is linear, so linear least squares solver is used. Complexity of this
computational scheme is O(N*M^2), mostly dominated by least squares solver

SEE ALSO:
    PolynomialFit()
    
COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multithreading support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Speed-up provided by multithreading greatly depends  on  problem  size
  ! - only large problems (number of coefficients is more than 500) can be
  ! efficiently multithreaded.
  !
  ! Generally, commercial ALGLIB is several times faster than  open-source
  ! generic C edition, and many times faster than open-source C# edition.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS:
    X   -   points, array[0..N-1].
    Y   -   function values, array[0..N-1].
    W   -   weights, array[0..N-1]
            Each summand in square  sum  of  approximation deviations from
            given  values  is  multiplied  by  the square of corresponding
            weight. Fill it by 1's if you don't  want  to  solve  weighted
            task.
    N   -   number of points, N>0.
            * if given, only leading N elements of X/Y/W are used
            * if not given, automatically determined from sizes of X/Y/W
    XC  -   points where polynomial values/derivatives are constrained,
            array[0..K-1].
    YC  -   values of constraints, array[0..K-1]
    DC  -   array[0..K-1], types of constraints:
            * DC[i]=0   means that P(XC[i])=YC[i]
            * DC[i]=1   means that P'(XC[i])=YC[i]
            SEE BELOW FOR IMPORTANT INFORMATION ON CONSTRAINTS
    K   -   number of constraints, 0<=K<M.
            K=0 means no constraints (XC/YC/DC are not used in such cases)
    M   -   number of basis functions (= polynomial_degree + 1), M>=1

OUTPUT PARAMETERS:
    Info-   same format as in LSFitLinearW() subroutine:
            * Info>0    task is solved
            * Info<=0   an error occured:
                        -4 means inconvergence of internal SVD
                        -3 means inconsistent constraints
    P   -   interpolant in barycentric form.
    Rep -   report, same format as in LSFitLinearW() subroutine.
            Following fields are set:
            * RMSError      rms error on the (X,Y).
            * AvgError      average error on the (X,Y).
            * AvgRelError   average relative error on the non-zero Y
            * MaxError      maximum error
                            NON-WEIGHTED ERRORS ARE CALCULATED

IMPORTANT:
    this subroitine doesn't calculate task's condition number for K<>0.

NOTES:
    you can convert P from barycentric form  to  the  power  or  Chebyshev
    basis with PolynomialBar2Pow() or PolynomialBar2Cheb() functions  from
    POLINT subpackage.

SETTING CONSTRAINTS - DANGERS AND OPPORTUNITIES:

Setting constraints can lead  to undesired  results,  like ill-conditioned
behavior, or inconsistency being detected. From the other side,  it allows
us to improve quality of the fit. Here we summarize  our  experience  with
constrained regression splines:
* even simple constraints can be inconsistent, see  Wikipedia  article  on
  this subject: http://en.wikipedia.org/wiki/Birkhoff_interpolation
* the  greater  is  M (given  fixed  constraints),  the  more chances that
  constraints will be consistent
* in the general case, consistency of constraints is NOT GUARANTEED.
* in the one special cases, however, we can  guarantee  consistency.  This
  case  is:  M>1  and constraints on the function values (NOT DERIVATIVES)

Our final recommendation is to use constraints  WHEN  AND  ONLY  when  you
can't solve your task without them. Anything beyond  special  cases  given
above is not guaranteed and may result in inconsistency.

  -- ALGLIB PROJECT --
     Copyright 10.12.2009 by Bochkanov Sergey
*************************************************************************/
void polynomialfitwc(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     ae_int_t n,
     /* Real    */ ae_vector* xc,
     /* Real    */ ae_vector* yc,
     /* Integer */ ae_vector* dc,
     ae_int_t k,
     ae_int_t m,
     ae_int_t* info,
     barycentricinterpolant* p,
     polynomialfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_vector _w;
    ae_vector _xc;
    ae_vector _yc;
    double xa;
    double xb;
    double sa;
    double sb;
    ae_vector xoriginal;
    ae_vector yoriginal;
    ae_vector y2;
    ae_vector w2;
    ae_vector tmp;
    ae_vector tmp2;
    ae_vector bx;
    ae_vector by;
    ae_vector bw;
    ae_int_t i;
    ae_int_t j;
    double u;
    double v;
    double s;
    ae_int_t relcnt;
    lsfitreport lrep;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    ae_vector_init_copy(&_w, w, _state);
    w = &_w;
    ae_vector_init_copy(&_xc, xc, _state);
    xc = &_xc;
    ae_vector_init_copy(&_yc, yc, _state);
    yc = &_yc;
    *info = 0;
    _barycentricinterpolant_clear(p);
    _polynomialfitreport_clear(rep);
    ae_vector_init(&xoriginal, 0, DT_REAL, _state);
    ae_vector_init(&yoriginal, 0, DT_REAL, _state);
    ae_vector_init(&y2, 0, DT_REAL, _state);
    ae_vector_init(&w2, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    ae_vector_init(&tmp2, 0, DT_REAL, _state);
    ae_vector_init(&bx, 0, DT_REAL, _state);
    ae_vector_init(&by, 0, DT_REAL, _state);
    ae_vector_init(&bw, 0, DT_REAL, _state);
    _lsfitreport_init(&lrep, _state);

    ae_assert(n>0, "PolynomialFitWC: N<=0!", _state);
    ae_assert(m>0, "PolynomialFitWC: M<=0!", _state);
    ae_assert(k>=0, "PolynomialFitWC: K<0!", _state);
    ae_assert(k<m, "PolynomialFitWC: K>=M!", _state);
    ae_assert(x->cnt>=n, "PolynomialFitWC: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "PolynomialFitWC: Length(Y)<N!", _state);
    ae_assert(w->cnt>=n, "PolynomialFitWC: Length(W)<N!", _state);
    ae_assert(xc->cnt>=k, "PolynomialFitWC: Length(XC)<K!", _state);
    ae_assert(yc->cnt>=k, "PolynomialFitWC: Length(YC)<K!", _state);
    ae_assert(dc->cnt>=k, "PolynomialFitWC: Length(DC)<K!", _state);
    ae_assert(isfinitevector(x, n, _state), "PolynomialFitWC: X contains infinite or NaN values!", _state);
    ae_assert(isfinitevector(y, n, _state), "PolynomialFitWC: Y contains infinite or NaN values!", _state);
    ae_assert(isfinitevector(w, n, _state), "PolynomialFitWC: X contains infinite or NaN values!", _state);
    ae_assert(isfinitevector(xc, k, _state), "PolynomialFitWC: XC contains infinite or NaN values!", _state);
    ae_assert(isfinitevector(yc, k, _state), "PolynomialFitWC: YC contains infinite or NaN values!", _state);
    for(i=0; i<=k-1; i++)
    {
        ae_assert(dc->ptr.p_int[i]==0||dc->ptr.p_int[i]==1, "PolynomialFitWC: one of DC[] is not 0 or 1!", _state);
    }
    
    /*
     * Scale X, Y, XC, YC.
     * Solve scaled problem using internal Chebyshev fitting function.
     */
    lsfitscalexy(x, y, w, n, xc, yc, dc, k, &xa, &xb, &sa, &sb, &xoriginal, &yoriginal, _state);
    lsfit_internalchebyshevfit(x, y, w, n, xc, yc, dc, k, m, info, &tmp, &lrep, _state);
    if( *info<0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Generate barycentric model and scale it
     * * BX, BY store barycentric model nodes
     * * FMatrix is reused (remember - it is at least MxM, what we need)
     *
     * Model intialization is done in O(M^2). In principle, it can be
     * done in O(M*log(M)), but before it we solved task with O(N*M^2)
     * complexity, so it is only a small amount of total time spent.
     */
    ae_vector_set_length(&bx, m, _state);
    ae_vector_set_length(&by, m, _state);
    ae_vector_set_length(&bw, m, _state);
    ae_vector_set_length(&tmp2, m, _state);
    s = (double)(1);
    for(i=0; i<=m-1; i++)
    {
        if( m!=1 )
        {
            u = ae_cos(ae_pi*i/(m-1), _state);
        }
        else
        {
            u = (double)(0);
        }
        v = (double)(0);
        for(j=0; j<=m-1; j++)
        {
            if( j==0 )
            {
                tmp2.ptr.p_double[j] = (double)(1);
            }
            else
            {
                if( j==1 )
                {
                    tmp2.ptr.p_double[j] = u;
                }
                else
                {
                    tmp2.ptr.p_double[j] = 2*u*tmp2.ptr.p_double[j-1]-tmp2.ptr.p_double[j-2];
                }
            }
            v = v+tmp.ptr.p_double[j]*tmp2.ptr.p_double[j];
        }
        bx.ptr.p_double[i] = u;
        by.ptr.p_double[i] = v;
        bw.ptr.p_double[i] = s;
        if( i==0||i==m-1 )
        {
            bw.ptr.p_double[i] = 0.5*bw.ptr.p_double[i];
        }
        s = -s;
    }
    barycentricbuildxyw(&bx, &by, &bw, m, p, _state);
    barycentriclintransx(p, 2/(xb-xa), -(xa+xb)/(xb-xa), _state);
    barycentriclintransy(p, sb-sa, sa, _state);
    
    /*
     * Scale absolute errors obtained from LSFitLinearW.
     * Relative error should be calculated separately
     * (because of shifting/scaling of the task)
     */
    rep->taskrcond = lrep.taskrcond;
    rep->rmserror = lrep.rmserror*(sb-sa);
    rep->avgerror = lrep.avgerror*(sb-sa);
    rep->maxerror = lrep.maxerror*(sb-sa);
    rep->avgrelerror = (double)(0);
    relcnt = 0;
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_neq(yoriginal.ptr.p_double[i],(double)(0)) )
        {
            rep->avgrelerror = rep->avgrelerror+ae_fabs(barycentriccalc(p, xoriginal.ptr.p_double[i], _state)-yoriginal.ptr.p_double[i], _state)/ae_fabs(yoriginal.ptr.p_double[i], _state);
            relcnt = relcnt+1;
        }
    }
    if( relcnt!=0 )
    {
        rep->avgrelerror = rep->avgrelerror/relcnt;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_polynomialfitwc(/* Real    */ ae_vector* x,
    /* Real    */ ae_vector* y,
    /* Real    */ ae_vector* w,
    ae_int_t n,
    /* Real    */ ae_vector* xc,
    /* Real    */ ae_vector* yc,
    /* Integer */ ae_vector* dc,
    ae_int_t k,
    ae_int_t m,
    ae_int_t* info,
    barycentricinterpolant* p,
    polynomialfitreport* rep, ae_state *_state)
{
    polynomialfitwc(x,y,w,n,xc,yc,dc,k,m,info,p,rep, _state);
}


/*************************************************************************
This function calculates value of four-parameter logistic (4PL)  model  at
specified point X. 4PL model has following form:

    F(x|A,B,C,D) = D+(A-D)/(1+Power(x/C,B))

INPUT PARAMETERS:
    X       -   current point, X>=0:
                * zero X is correctly handled even for B<=0
                * negative X results in exception.
    A, B, C, D- parameters of 4PL model:
                * A is unconstrained
                * B is unconstrained; zero or negative values are handled
                  correctly.
                * C>0, non-positive value results in exception
                * D is unconstrained
                
RESULT:
    model value at X

NOTE: if B=0, denominator is assumed to be equal to 2.0 even  for  zero  X
      (strictly speaking, 0^0 is undefined).

NOTE: this function also throws exception  if  all  input  parameters  are
      correct, but overflow was detected during calculations.
      
NOTE: this function performs a lot of checks;  if  you  need  really  high
      performance, consider evaluating model  yourself,  without  checking
      for degenerate cases.
      
    
  -- ALGLIB PROJECT --
     Copyright 14.05.2014 by Bochkanov Sergey
*************************************************************************/
double logisticcalc4(double x,
     double a,
     double b,
     double c,
     double d,
     ae_state *_state)
{
    double result;


    ae_assert(ae_isfinite(x, _state), "LogisticCalc4: X is not finite", _state);
    ae_assert(ae_isfinite(a, _state), "LogisticCalc4: A is not finite", _state);
    ae_assert(ae_isfinite(b, _state), "LogisticCalc4: B is not finite", _state);
    ae_assert(ae_isfinite(c, _state), "LogisticCalc4: C is not finite", _state);
    ae_assert(ae_isfinite(d, _state), "LogisticCalc4: D is not finite", _state);
    ae_assert(ae_fp_greater_eq(x,(double)(0)), "LogisticCalc4: X is negative", _state);
    ae_assert(ae_fp_greater(c,(double)(0)), "LogisticCalc4: C is non-positive", _state);
    
    /*
     * Check for degenerate cases
     */
    if( ae_fp_eq(b,(double)(0)) )
    {
        result = 0.5*(a+d);
        return result;
    }
    if( ae_fp_eq(x,(double)(0)) )
    {
        if( ae_fp_greater(b,(double)(0)) )
        {
            result = a;
        }
        else
        {
            result = d;
        }
        return result;
    }
    
    /*
     * General case
     */
    result = d+(a-d)/(1.0+ae_pow(x/c, b, _state));
    ae_assert(ae_isfinite(result, _state), "LogisticCalc4: overflow during calculations", _state);
    return result;
}


/*************************************************************************
This function calculates value of five-parameter logistic (5PL)  model  at
specified point X. 5PL model has following form:

    F(x|A,B,C,D,G) = D+(A-D)/Power(1+Power(x/C,B),G)

INPUT PARAMETERS:
    X       -   current point, X>=0:
                * zero X is correctly handled even for B<=0
                * negative X results in exception.
    A, B, C, D, G- parameters of 5PL model:
                * A is unconstrained
                * B is unconstrained; zero or negative values are handled
                  correctly.
                * C>0, non-positive value results in exception
                * D is unconstrained
                * G>0, non-positive value results in exception
                
RESULT:
    model value at X

NOTE: if B=0, denominator is assumed to be equal to Power(2.0,G) even  for
      zero X (strictly speaking, 0^0 is undefined).

NOTE: this function also throws exception  if  all  input  parameters  are
      correct, but overflow was detected during calculations.
      
NOTE: this function performs a lot of checks;  if  you  need  really  high
      performance, consider evaluating model  yourself,  without  checking
      for degenerate cases.
      
    
  -- ALGLIB PROJECT --
     Copyright 14.05.2014 by Bochkanov Sergey
*************************************************************************/
double logisticcalc5(double x,
     double a,
     double b,
     double c,
     double d,
     double g,
     ae_state *_state)
{
    double result;


    ae_assert(ae_isfinite(x, _state), "LogisticCalc5: X is not finite", _state);
    ae_assert(ae_isfinite(a, _state), "LogisticCalc5: A is not finite", _state);
    ae_assert(ae_isfinite(b, _state), "LogisticCalc5: B is not finite", _state);
    ae_assert(ae_isfinite(c, _state), "LogisticCalc5: C is not finite", _state);
    ae_assert(ae_isfinite(d, _state), "LogisticCalc5: D is not finite", _state);
    ae_assert(ae_isfinite(g, _state), "LogisticCalc5: G is not finite", _state);
    ae_assert(ae_fp_greater_eq(x,(double)(0)), "LogisticCalc5: X is negative", _state);
    ae_assert(ae_fp_greater(c,(double)(0)), "LogisticCalc5: C is non-positive", _state);
    ae_assert(ae_fp_greater(g,(double)(0)), "LogisticCalc5: G is non-positive", _state);
    
    /*
     * Check for degenerate cases
     */
    if( ae_fp_eq(b,(double)(0)) )
    {
        result = d+(a-d)/ae_pow(2.0, g, _state);
        return result;
    }
    if( ae_fp_eq(x,(double)(0)) )
    {
        if( ae_fp_greater(b,(double)(0)) )
        {
            result = a;
        }
        else
        {
            result = d;
        }
        return result;
    }
    
    /*
     * General case
     */
    result = d+(a-d)/ae_pow(1.0+ae_pow(x/c, b, _state), g, _state);
    ae_assert(ae_isfinite(result, _state), "LogisticCalc5: overflow during calculations", _state);
    return result;
}


/*************************************************************************
This function fits four-parameter logistic (4PL) model  to  data  provided
by user. 4PL model has following form:

    F(x|A,B,C,D) = D+(A-D)/(1+Power(x/C,B))

Here:
    * A, D - unconstrained (see LogisticFit4EC() for constrained 4PL)
    * B>=0
    * C>0
    
IMPORTANT: output of this function is constrained in  such  way that  B>0.
           Because 4PL model is symmetric with respect to B, there  is  no
           need to explore  B<0.  Constraining  B  makes  algorithm easier
           to stabilize and debug.
           Users  who  for  some  reason  prefer to work with negative B's
           should transform output themselves (swap A and D, replace B  by
           -B).
           
4PL fitting is implemented as follows:
* we perform small number of restarts from random locations which helps to
  solve problem of bad local extrema. Locations are only partially  random
  - we use input data to determine good  initial  guess,  but  we  include
  controlled amount of randomness.
* we perform Levenberg-Marquardt fitting with very  tight  constraints  on
  parameters B and C - it allows us to find good  initial  guess  for  the
  second stage without risk of running into "flat spot".
* second  Levenberg-Marquardt  round  is   performed   without   excessive
  constraints. Results from the previous round are used as initial guess.
* after fitting is done, we compare results with best values found so far,
  rewrite "best solution" if needed, and move to next random location.
  
Overall algorithm is very stable and is not prone to  bad  local  extrema.
Furthermore, it automatically scales when input data have  very  large  or
very small range.

INPUT PARAMETERS:
    X       -   array[N], stores X-values.
                MUST include only non-negative numbers  (but  may  include
                zero values). Can be unsorted.
    Y       -   array[N], values to fit.
    N       -   number of points. If N is less than  length  of  X/Y, only
                leading N elements are used.
                
OUTPUT PARAMETERS:
    A, B, C, D- parameters of 4PL model
    Rep     -   fitting report. This structure has many fields,  but  ONLY
                ONES LISTED BELOW ARE SET:
                * Rep.IterationsCount - number of iterations performed
                * Rep.RMSError - root-mean-square error
                * Rep.AvgError - average absolute error
                * Rep.AvgRelError - average relative error (calculated for
                  non-zero Y-values)
                * Rep.MaxError - maximum absolute error
                * Rep.R2 - coefficient of determination,  R-squared.  This
                  coefficient   is  calculated  as  R2=1-RSS/TSS  (in case
                  of nonlinear  regression  there  are  multiple  ways  to
                  define R2, each of them giving different results).
                  
NOTE: after  you  obtained  coefficients,  you  can  evaluate  model  with
      LogisticCalc4() function.

NOTE: if you need better control over fitting process than provided by this
      function, you may use LogisticFit45X().
                
NOTE: step is automatically scaled according to scale of parameters  being
      fitted before we compare its length with EpsX. Thus,  this  function
      can be used to fit data with very small or very large values without
      changing EpsX.
    

  -- ALGLIB PROJECT --
     Copyright 14.02.2014 by Bochkanov Sergey
*************************************************************************/
void logisticfit4(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     double* a,
     double* b,
     double* c,
     double* d,
     lsfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    double g;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    *a = 0;
    *b = 0;
    *c = 0;
    *d = 0;
    _lsfitreport_clear(rep);

    logisticfit45x(x, y, n, _state->v_nan, _state->v_nan, ae_true, 0.0, 0.0, 0, a, b, c, d, &g, rep, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
This function fits four-parameter logistic (4PL) model  to  data  provided
by user, with optional constraints on parameters A and D.  4PL  model  has
following form:

    F(x|A,B,C,D) = D+(A-D)/(1+Power(x/C,B))

Here:
    * A, D - with optional equality constraints
    * B>=0
    * C>0
    
IMPORTANT: output of this function is constrained in  such  way that  B>0.
           Because 4PL model is symmetric with respect to B, there  is  no
           need to explore  B<0.  Constraining  B  makes  algorithm easier
           to stabilize and debug.
           Users  who  for  some  reason  prefer to work with negative B's
           should transform output themselves (swap A and D, replace B  by
           -B).
           
4PL fitting is implemented as follows:
* we perform small number of restarts from random locations which helps to
  solve problem of bad local extrema. Locations are only partially  random
  - we use input data to determine good  initial  guess,  but  we  include
  controlled amount of randomness.
* we perform Levenberg-Marquardt fitting with very  tight  constraints  on
  parameters B and C - it allows us to find good  initial  guess  for  the
  second stage without risk of running into "flat spot".
* second  Levenberg-Marquardt  round  is   performed   without   excessive
  constraints. Results from the previous round are used as initial guess.
* after fitting is done, we compare results with best values found so far,
  rewrite "best solution" if needed, and move to next random location.
  
Overall algorithm is very stable and is not prone to  bad  local  extrema.
Furthermore, it automatically scales when input data have  very  large  or
very small range.

INPUT PARAMETERS:
    X       -   array[N], stores X-values.
                MUST include only non-negative numbers  (but  may  include
                zero values). Can be unsorted.
    Y       -   array[N], values to fit.
    N       -   number of points. If N is less than  length  of  X/Y, only
                leading N elements are used.
    CnstrLeft-  optional equality constraint for model value at the   left
                boundary (at X=0). Specify NAN (Not-a-Number)  if  you  do
                not need constraint on the model value at X=0 (in C++  you
                can pass alglib::fp_nan as parameter, in  C#  it  will  be
                Double.NaN).
                See  below,  section  "EQUALITY  CONSTRAINTS"   for   more
                information about constraints.
    CnstrRight- optional equality constraint for model value at X=infinity.
                Specify NAN (Not-a-Number) if you do not  need  constraint
                on the model value (in C++  you can pass alglib::fp_nan as
                parameter, in  C# it will  be Double.NaN).
                See  below,  section  "EQUALITY  CONSTRAINTS"   for   more
                information about constraints.
                
OUTPUT PARAMETERS:
    A, B, C, D- parameters of 4PL model
    Rep     -   fitting report. This structure has many fields,  but  ONLY
                ONES LISTED BELOW ARE SET:
                * Rep.IterationsCount - number of iterations performed
                * Rep.RMSError - root-mean-square error
                * Rep.AvgError - average absolute error
                * Rep.AvgRelError - average relative error (calculated for
                  non-zero Y-values)
                * Rep.MaxError - maximum absolute error
                * Rep.R2 - coefficient of determination,  R-squared.  This
                  coefficient   is  calculated  as  R2=1-RSS/TSS  (in case
                  of nonlinear  regression  there  are  multiple  ways  to
                  define R2, each of them giving different results).

NOTE: after  you  obtained  coefficients,  you  can  evaluate  model  with
      LogisticCalc4() function.

NOTE: if you need better control over fitting process than provided by this
      function, you may use LogisticFit45X().
                
NOTE: step is automatically scaled according to scale of parameters  being
      fitted before we compare its length with EpsX. Thus,  this  function
      can be used to fit data with very small or very large values without
      changing EpsX.

EQUALITY CONSTRAINTS ON PARAMETERS

4PL/5PL solver supports equality constraints on model values at  the  left
boundary (X=0) and right  boundary  (X=infinity).  These  constraints  are
completely optional and you can specify both of them, only  one  -  or  no
constraints at all.

Parameter  CnstrLeft  contains  left  constraint (or NAN for unconstrained
fitting), and CnstrRight contains right  one.  For  4PL,  left  constraint
ALWAYS corresponds to parameter A, and right one is ALWAYS  constraint  on
D. That's because 4PL model is normalized in such way that B>=0.
    

  -- ALGLIB PROJECT --
     Copyright 14.02.2014 by Bochkanov Sergey
*************************************************************************/
void logisticfit4ec(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     double cnstrleft,
     double cnstrright,
     double* a,
     double* b,
     double* c,
     double* d,
     lsfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    double g;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    *a = 0;
    *b = 0;
    *c = 0;
    *d = 0;
    _lsfitreport_clear(rep);

    logisticfit45x(x, y, n, cnstrleft, cnstrright, ae_true, 0.0, 0.0, 0, a, b, c, d, &g, rep, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
This function fits five-parameter logistic (5PL) model  to  data  provided
by user. 5PL model has following form:

    F(x|A,B,C,D,G) = D+(A-D)/Power(1+Power(x/C,B),G)

Here:
    * A, D - unconstrained
    * B - unconstrained
    * C>0
    * G>0
    
IMPORTANT: unlike in  4PL  fitting,  output  of  this  function   is   NOT
           constrained in  such  way that B is guaranteed to be  positive.
           Furthermore,  unlike  4PL,  5PL  model  is  NOT  symmetric with
           respect to B, so you can NOT transform model to equivalent one,
           with B having desired sign (>0 or <0).
    
5PL fitting is implemented as follows:
* we perform small number of restarts from random locations which helps to
  solve problem of bad local extrema. Locations are only partially  random
  - we use input data to determine good  initial  guess,  but  we  include
  controlled amount of randomness.
* we perform Levenberg-Marquardt fitting with very  tight  constraints  on
  parameters B and C - it allows us to find good  initial  guess  for  the
  second stage without risk of running into "flat spot".  Parameter  G  is
  fixed at G=1.
* second  Levenberg-Marquardt  round  is   performed   without   excessive
  constraints on B and C, but with G still equal to 1.  Results  from  the
  previous round are used as initial guess.
* third Levenberg-Marquardt round relaxes constraints on G  and  tries  two
  different models - one with B>0 and one with B<0.
* after fitting is done, we compare results with best values found so far,
  rewrite "best solution" if needed, and move to next random location.
  
Overall algorithm is very stable and is not prone to  bad  local  extrema.
Furthermore, it automatically scales when input data have  very  large  or
very small range.

INPUT PARAMETERS:
    X       -   array[N], stores X-values.
                MUST include only non-negative numbers  (but  may  include
                zero values). Can be unsorted.
    Y       -   array[N], values to fit.
    N       -   number of points. If N is less than  length  of  X/Y, only
                leading N elements are used.
                
OUTPUT PARAMETERS:
    A,B,C,D,G-  parameters of 5PL model
    Rep     -   fitting report. This structure has many fields,  but  ONLY
                ONES LISTED BELOW ARE SET:
                * Rep.IterationsCount - number of iterations performed
                * Rep.RMSError - root-mean-square error
                * Rep.AvgError - average absolute error
                * Rep.AvgRelError - average relative error (calculated for
                  non-zero Y-values)
                * Rep.MaxError - maximum absolute error
                * Rep.R2 - coefficient of determination,  R-squared.  This
                  coefficient   is  calculated  as  R2=1-RSS/TSS  (in case
                  of nonlinear  regression  there  are  multiple  ways  to
                  define R2, each of them giving different results).

NOTE: after  you  obtained  coefficients,  you  can  evaluate  model  with
      LogisticCalc5() function.

NOTE: if you need better control over fitting process than provided by this
      function, you may use LogisticFit45X().
                
NOTE: step is automatically scaled according to scale of parameters  being
      fitted before we compare its length with EpsX. Thus,  this  function
      can be used to fit data with very small or very large values without
      changing EpsX.
    

  -- ALGLIB PROJECT --
     Copyright 14.02.2014 by Bochkanov Sergey
*************************************************************************/
void logisticfit5(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     double* a,
     double* b,
     double* c,
     double* d,
     double* g,
     lsfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    *a = 0;
    *b = 0;
    *c = 0;
    *d = 0;
    *g = 0;
    _lsfitreport_clear(rep);

    logisticfit45x(x, y, n, _state->v_nan, _state->v_nan, ae_false, 0.0, 0.0, 0, a, b, c, d, g, rep, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
This function fits five-parameter logistic (5PL) model  to  data  provided
by user, subject to optional equality constraints on parameters A  and  D.
5PL model has following form:

    F(x|A,B,C,D,G) = D+(A-D)/Power(1+Power(x/C,B),G)

Here:
    * A, D - with optional equality constraints
    * B - unconstrained
    * C>0
    * G>0
    
IMPORTANT: unlike in  4PL  fitting,  output  of  this  function   is   NOT
           constrained in  such  way that B is guaranteed to be  positive.
           Furthermore,  unlike  4PL,  5PL  model  is  NOT  symmetric with
           respect to B, so you can NOT transform model to equivalent one,
           with B having desired sign (>0 or <0).
    
5PL fitting is implemented as follows:
* we perform small number of restarts from random locations which helps to
  solve problem of bad local extrema. Locations are only partially  random
  - we use input data to determine good  initial  guess,  but  we  include
  controlled amount of randomness.
* we perform Levenberg-Marquardt fitting with very  tight  constraints  on
  parameters B and C - it allows us to find good  initial  guess  for  the
  second stage without risk of running into "flat spot".  Parameter  G  is
  fixed at G=1.
* second  Levenberg-Marquardt  round  is   performed   without   excessive
  constraints on B and C, but with G still equal to 1.  Results  from  the
  previous round are used as initial guess.
* third Levenberg-Marquardt round relaxes constraints on G  and  tries  two
  different models - one with B>0 and one with B<0.
* after fitting is done, we compare results with best values found so far,
  rewrite "best solution" if needed, and move to next random location.
  
Overall algorithm is very stable and is not prone to  bad  local  extrema.
Furthermore, it automatically scales when input data have  very  large  or
very small range.

INPUT PARAMETERS:
    X       -   array[N], stores X-values.
                MUST include only non-negative numbers  (but  may  include
                zero values). Can be unsorted.
    Y       -   array[N], values to fit.
    N       -   number of points. If N is less than  length  of  X/Y, only
                leading N elements are used.
    CnstrLeft-  optional equality constraint for model value at the   left
                boundary (at X=0). Specify NAN (Not-a-Number)  if  you  do
                not need constraint on the model value at X=0 (in C++  you
                can pass alglib::fp_nan as parameter, in  C#  it  will  be
                Double.NaN).
                See  below,  section  "EQUALITY  CONSTRAINTS"   for   more
                information about constraints.
    CnstrRight- optional equality constraint for model value at X=infinity.
                Specify NAN (Not-a-Number) if you do not  need  constraint
                on the model value (in C++  you can pass alglib::fp_nan as
                parameter, in  C# it will  be Double.NaN).
                See  below,  section  "EQUALITY  CONSTRAINTS"   for   more
                information about constraints.
                
OUTPUT PARAMETERS:
    A,B,C,D,G-  parameters of 5PL model
    Rep     -   fitting report. This structure has many fields,  but  ONLY
                ONES LISTED BELOW ARE SET:
                * Rep.IterationsCount - number of iterations performed
                * Rep.RMSError - root-mean-square error
                * Rep.AvgError - average absolute error
                * Rep.AvgRelError - average relative error (calculated for
                  non-zero Y-values)
                * Rep.MaxError - maximum absolute error
                * Rep.R2 - coefficient of determination,  R-squared.  This
                  coefficient   is  calculated  as  R2=1-RSS/TSS  (in case
                  of nonlinear  regression  there  are  multiple  ways  to
                  define R2, each of them giving different results).

NOTE: after  you  obtained  coefficients,  you  can  evaluate  model  with
      LogisticCalc5() function.

NOTE: if you need better control over fitting process than provided by this
      function, you may use LogisticFit45X().
                
NOTE: step is automatically scaled according to scale of parameters  being
      fitted before we compare its length with EpsX. Thus,  this  function
      can be used to fit data with very small or very large values without
      changing EpsX.

EQUALITY CONSTRAINTS ON PARAMETERS

5PL solver supports equality constraints on model  values  at   the   left
boundary (X=0) and right  boundary  (X=infinity).  These  constraints  are
completely optional and you can specify both of them, only  one  -  or  no
constraints at all.

Parameter  CnstrLeft  contains  left  constraint (or NAN for unconstrained
fitting), and CnstrRight contains right  one.

Unlike 4PL one, 5PL model is NOT symmetric with respect to  change in sign
of B. Thus, negative B's are possible, and left constraint  may  constrain
parameter A (for positive B's)  -  or  parameter  D  (for  negative  B's).
Similarly changes meaning of right constraint.

You do not have to decide what parameter to  constrain  -  algorithm  will
automatically determine correct parameters as fitting progresses. However,
question highlighted above is important when you interpret fitting results.
    

  -- ALGLIB PROJECT --
     Copyright 14.02.2014 by Bochkanov Sergey
*************************************************************************/
void logisticfit5ec(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     double cnstrleft,
     double cnstrright,
     double* a,
     double* b,
     double* c,
     double* d,
     double* g,
     lsfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    *a = 0;
    *b = 0;
    *c = 0;
    *d = 0;
    *g = 0;
    _lsfitreport_clear(rep);

    logisticfit45x(x, y, n, cnstrleft, cnstrright, ae_false, 0.0, 0.0, 0, a, b, c, d, g, rep, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
This is "expert" 4PL/5PL fitting function, which can be used if  you  need
better control over fitting process than provided  by  LogisticFit4()  or
LogisticFit5().

This function fits model of the form

    F(x|A,B,C,D)   = D+(A-D)/(1+Power(x/C,B))           (4PL model)

or

    F(x|A,B,C,D,G) = D+(A-D)/Power(1+Power(x/C,B),G)    (5PL model)
    
Here:
    * A, D - unconstrained
    * B>=0 for 4PL, unconstrained for 5PL
    * C>0
    * G>0 (if present)

INPUT PARAMETERS:
    X       -   array[N], stores X-values.
                MUST include only non-negative numbers  (but  may  include
                zero values). Can be unsorted.
    Y       -   array[N], values to fit.
    N       -   number of points. If N is less than  length  of  X/Y, only
                leading N elements are used.
    CnstrLeft-  optional equality constraint for model value at the   left
                boundary (at X=0). Specify NAN (Not-a-Number)  if  you  do
                not need constraint on the model value at X=0 (in C++  you
                can pass alglib::fp_nan as parameter, in  C#  it  will  be
                Double.NaN).
                See  below,  section  "EQUALITY  CONSTRAINTS"   for   more
                information about constraints.
    CnstrRight- optional equality constraint for model value at X=infinity.
                Specify NAN (Not-a-Number) if you do not  need  constraint
                on the model value (in C++  you can pass alglib::fp_nan as
                parameter, in  C# it will  be Double.NaN).
                See  below,  section  "EQUALITY  CONSTRAINTS"   for   more
                information about constraints.
    Is4PL   -   whether 4PL or 5PL models are fitted
    LambdaV -   regularization coefficient, LambdaV>=0.
                Set it to zero unless you know what you are doing.
    EpsX    -   stopping condition (step size), EpsX>=0.
                Zero value means that small step is automatically chosen.
                See notes below for more information.
    RsCnt   -   number of repeated restarts from  random  points.  4PL/5PL
                models are prone to problem of bad local extrema. Utilizing
                multiple random restarts allows  us  to  improve algorithm
                convergence.
                RsCnt>=0.
                Zero value means that function automatically choose  small
                amount of restarts (recommended).
                
OUTPUT PARAMETERS:
    A, B, C, D- parameters of 4PL model
    G       -   parameter of 5PL model; for Is4PL=True, G=1 is returned.
    Rep     -   fitting report. This structure has many fields,  but  ONLY
                ONES LISTED BELOW ARE SET:
                * Rep.IterationsCount - number of iterations performed
                * Rep.RMSError - root-mean-square error
                * Rep.AvgError - average absolute error
                * Rep.AvgRelError - average relative error (calculated for
                  non-zero Y-values)
                * Rep.MaxError - maximum absolute error
                * Rep.R2 - coefficient of determination,  R-squared.  This
                  coefficient   is  calculated  as  R2=1-RSS/TSS  (in case
                  of nonlinear  regression  there  are  multiple  ways  to
                  define R2, each of them giving different results).
                
NOTE: after  you  obtained  coefficients,  you  can  evaluate  model  with
      LogisticCalc5() function.

NOTE: step is automatically scaled according to scale of parameters  being
      fitted before we compare its length with EpsX. Thus,  this  function
      can be used to fit data with very small or very large values without
      changing EpsX.

EQUALITY CONSTRAINTS ON PARAMETERS

4PL/5PL solver supports equality constraints on model values at  the  left
boundary (X=0) and right  boundary  (X=infinity).  These  constraints  are
completely optional and you can specify both of them, only  one  -  or  no
constraints at all.

Parameter  CnstrLeft  contains  left  constraint (or NAN for unconstrained
fitting), and CnstrRight contains right  one.  For  4PL,  left  constraint
ALWAYS corresponds to parameter A, and right one is ALWAYS  constraint  on
D. That's because 4PL model is normalized in such way that B>=0.

For 5PL model things are different. Unlike  4PL  one,  5PL  model  is  NOT
symmetric with respect to  change  in  sign  of  B. Thus, negative B's are
possible, and left constraint may constrain parameter A (for positive B's)
- or parameter D (for negative B's). Similarly changes  meaning  of  right
constraint.

You do not have to decide what parameter to  constrain  -  algorithm  will
automatically determine correct parameters as fitting progresses. However,
question highlighted above is important when you interpret fitting results.
    

  -- ALGLIB PROJECT --
     Copyright 14.02.2014 by Bochkanov Sergey
*************************************************************************/
void logisticfit45x(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     double cnstrleft,
     double cnstrright,
     ae_bool is4pl,
     double lambdav,
     double epsx,
     ae_int_t rscnt,
     double* a,
     double* b,
     double* c,
     double* d,
     double* g,
     lsfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_int_t i;
    ae_int_t k;
    ae_int_t innerit;
    ae_int_t outerit;
    ae_int_t nz;
    double v;
    double b00;
    double b01;
    double b10;
    double b11;
    double b30;
    double b31;
    ae_vector p0;
    ae_vector p1;
    ae_vector p2;
    ae_vector bndl;
    ae_vector bndu;
    ae_vector s;
    ae_matrix z;
    hqrndstate rs;
    minlmstate state;
    minlmreport replm;
    ae_int_t maxits;
    double fbest;
    double flast;
    double flast2;
    double scalex;
    double scaley;
    ae_vector bufx;
    ae_vector bufy;
    double rss;
    double tss;
    double meany;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    *a = 0;
    *b = 0;
    *c = 0;
    *d = 0;
    *g = 0;
    _lsfitreport_clear(rep);
    ae_vector_init(&p0, 0, DT_REAL, _state);
    ae_vector_init(&p1, 0, DT_REAL, _state);
    ae_vector_init(&p2, 0, DT_REAL, _state);
    ae_vector_init(&bndl, 0, DT_REAL, _state);
    ae_vector_init(&bndu, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_matrix_init(&z, 0, 0, DT_REAL, _state);
    _hqrndstate_init(&rs, _state);
    _minlmstate_init(&state, _state);
    _minlmreport_init(&replm, _state);
    ae_vector_init(&bufx, 0, DT_REAL, _state);
    ae_vector_init(&bufy, 0, DT_REAL, _state);

    ae_assert(ae_isfinite(epsx, _state), "LogisticFitX: EpsX is infinite/NAN", _state);
    ae_assert(ae_isfinite(lambdav, _state), "LogisticFitX: LambdaV is infinite/NAN", _state);
    ae_assert(ae_isfinite(cnstrleft, _state)||ae_isnan(cnstrleft, _state), "LogisticFitX: CnstrLeft is NOT finite or NAN", _state);
    ae_assert(ae_isfinite(cnstrright, _state)||ae_isnan(cnstrright, _state), "LogisticFitX: CnstrRight is NOT finite or NAN", _state);
    ae_assert(ae_fp_greater_eq(lambdav,(double)(0)), "LogisticFitX: negative LambdaV", _state);
    ae_assert(n>0, "LogisticFitX: N<=0", _state);
    ae_assert(rscnt>=0, "LogisticFitX: RsCnt<0", _state);
    ae_assert(ae_fp_greater_eq(epsx,(double)(0)), "LogisticFitX: EpsX<0", _state);
    ae_assert(x->cnt>=n, "LogisticFitX: Length(X)<N", _state);
    ae_assert(y->cnt>=n, "LogisticFitX: Length(Y)<N", _state);
    ae_assert(isfinitevector(x, n, _state), "LogisticFitX: X contains infinite/NAN values", _state);
    ae_assert(isfinitevector(y, n, _state), "LogisticFitX: X contains infinite/NAN values", _state);
    hqrndseed(2211, 1033044, &rs, _state);
    lsfit_clearreport(rep, _state);
    if( ae_fp_eq(epsx,(double)(0)) )
    {
        epsx = 1.0E-10;
    }
    if( rscnt==0 )
    {
        rscnt = 4;
    }
    maxits = 1000;
    
    /*
     * Sort points by X.
     * Determine number of zero and non-zero values.
     */
    tagsortfastr(x, y, &bufx, &bufy, n, _state);
    ae_assert(ae_fp_greater_eq(x->ptr.p_double[0],(double)(0)), "LogisticFitX: some X[] are negative", _state);
    nz = n;
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_greater(x->ptr.p_double[i],(double)(0)) )
        {
            nz = i;
            break;
        }
    }
    
    /*
     * For NZ=N (all X[] are zero) special code is used.
     * For NZ<N we use general-purpose code.
     */
    rep->iterationscount = 0;
    if( nz==n )
    {
        
        /*
         * NZ=N, degenerate problem.
         * No need to run optimizer.
         */
        v = 0.0;
        for(i=0; i<=n-1; i++)
        {
            v = v+y->ptr.p_double[i];
        }
        v = v/n;
        if( ae_isfinite(cnstrleft, _state) )
        {
            *a = cnstrleft;
        }
        else
        {
            *a = v;
        }
        *b = (double)(1);
        *c = (double)(1);
        if( ae_isfinite(cnstrright, _state) )
        {
            *d = cnstrright;
        }
        else
        {
            *d = *a;
        }
        *g = (double)(1);
    }
    else
    {
        
        /*
         * Non-degenerate problem.
         * Determine scale of data.
         */
        scalex = x->ptr.p_double[nz+(n-nz)/2];
        ae_assert(ae_fp_greater(scalex,(double)(0)), "LogisticFitX: internal error", _state);
        v = 0.0;
        for(i=0; i<=n-1; i++)
        {
            v = v+y->ptr.p_double[i];
        }
        v = v/n;
        scaley = 0.0;
        for(i=0; i<=n-1; i++)
        {
            scaley = scaley+ae_sqr(y->ptr.p_double[i]-v, _state);
        }
        scaley = ae_sqrt(scaley/n, _state);
        if( ae_fp_eq(scaley,(double)(0)) )
        {
            scaley = 1.0;
        }
        ae_vector_set_length(&s, 5, _state);
        s.ptr.p_double[0] = scaley;
        s.ptr.p_double[1] = 0.1;
        s.ptr.p_double[2] = scalex;
        s.ptr.p_double[3] = scaley;
        s.ptr.p_double[4] = 0.1;
        ae_vector_set_length(&p0, 5, _state);
        p0.ptr.p_double[0] = (double)(0);
        p0.ptr.p_double[1] = (double)(0);
        p0.ptr.p_double[2] = (double)(0);
        p0.ptr.p_double[3] = (double)(0);
        p0.ptr.p_double[4] = (double)(0);
        ae_vector_set_length(&bndl, 5, _state);
        ae_vector_set_length(&bndu, 5, _state);
        minlmcreatevj(5, n+5, &p0, &state, _state);
        minlmsetscale(&state, &s, _state);
        minlmsetcond(&state, 0.0, 0.0, epsx, maxits, _state);
        minlmsetxrep(&state, ae_true, _state);
        
        /*
         * Main loop - includes THREE (!) nested iterations:
         *
         * 1. Inner iteration is minimization of target function from
         *    the current initial point P1 subject to boundary constraints
         *    given by arrays BndL and BndU.
         *
         * 2. Middle iteration changes boundary constraints from tight to
         *    relaxed ones:
         *    * at the first middle iteration we optimize with "tight"
         *      constraints on parameters B and C (P[1] and P[2]). It
         *      allows us to find good initial point for the next middle
         *      iteration without risk of running into "hard" points (B=0, C=0).
         *      Initial point is initialized by outer iteration.
         *      Solution is placed to P1.
         *    * at the second middle iteration we relax boundary constraints
         *      on B and C. Solution P1 from the first middle iteration is
         *      used as initial point for the second one.
         *    * both first and second iterations are 4PL models, even when
         *      we fit 5PL.
         *    * additionally, for 5PL models, we use results from the second
         *      middle iteration is initial guess for 5PL fit.
         *    * after middle iteration is over we compare quality of the
         *      solution stored in P1 and offload it to A/B/C/D/G, if it
         *      is better.
         *
         * 3. Outer iteration (starts below) changes following parameters:
         *    * initial point
         *    * "tight" constraints BndL/BndU
         *    * "relaxed" constraints BndL/BndU
         *
         * Below we prepare combined matrix Z of optimization settings for
         * outer/middle iterations:
         *
         *     [ P00 BndL00 BndU00 BndL01 BndU01 ]
         *     [                                 ]
         *     [ P10 BndL10 BndU10 BndL11 BndU11 ]
         *
         * Here:
         * * Pi0 is initial point for I-th outer iteration
         * * BndLij is lower boundary for I-th outer iteration, J-th inner iteration
         * * BndUij - same as BndLij
         */
        ae_matrix_set_length(&z, rscnt, 5+4*5, _state);
        for(i=0; i<=rscnt-1; i++)
        {
            if( ae_isfinite(cnstrleft, _state) )
            {
                z.ptr.pp_double[i][0] = cnstrleft;
            }
            else
            {
                z.ptr.pp_double[i][0] = y->ptr.p_double[0]+0.25*scaley*(hqrnduniformr(&rs, _state)-0.5);
            }
            z.ptr.pp_double[i][1] = 0.5+hqrnduniformr(&rs, _state);
            z.ptr.pp_double[i][2] = x->ptr.p_double[nz+hqrnduniformi(&rs, n-nz, _state)];
            if( ae_isfinite(cnstrright, _state) )
            {
                z.ptr.pp_double[i][3] = cnstrright;
            }
            else
            {
                z.ptr.pp_double[i][3] = y->ptr.p_double[n-1]+0.25*scaley*(hqrnduniformr(&rs, _state)-0.5);
            }
            z.ptr.pp_double[i][4] = 1.0;
            if( ae_isfinite(cnstrleft, _state) )
            {
                z.ptr.pp_double[i][5+0] = cnstrleft;
                z.ptr.pp_double[i][10+0] = cnstrleft;
            }
            else
            {
                z.ptr.pp_double[i][5+0] = _state->v_neginf;
                z.ptr.pp_double[i][10+0] = _state->v_posinf;
            }
            z.ptr.pp_double[i][5+1] = 0.5;
            z.ptr.pp_double[i][10+1] = 2.0;
            z.ptr.pp_double[i][5+2] = 0.5*scalex;
            z.ptr.pp_double[i][10+2] = 2.0*scalex;
            if( ae_isfinite(cnstrright, _state) )
            {
                z.ptr.pp_double[i][5+3] = cnstrright;
                z.ptr.pp_double[i][10+3] = cnstrright;
            }
            else
            {
                z.ptr.pp_double[i][5+3] = _state->v_neginf;
                z.ptr.pp_double[i][10+3] = _state->v_posinf;
            }
            z.ptr.pp_double[i][5+4] = 1.0;
            z.ptr.pp_double[i][10+4] = 1.0;
            if( ae_isfinite(cnstrleft, _state) )
            {
                z.ptr.pp_double[i][15+0] = cnstrleft;
                z.ptr.pp_double[i][20+0] = cnstrleft;
            }
            else
            {
                z.ptr.pp_double[i][15+0] = _state->v_neginf;
                z.ptr.pp_double[i][20+0] = _state->v_posinf;
            }
            z.ptr.pp_double[i][15+1] = 0.01;
            z.ptr.pp_double[i][20+1] = _state->v_posinf;
            z.ptr.pp_double[i][15+2] = ae_machineepsilon*scalex;
            z.ptr.pp_double[i][20+2] = _state->v_posinf;
            if( ae_isfinite(cnstrright, _state) )
            {
                z.ptr.pp_double[i][15+3] = cnstrright;
                z.ptr.pp_double[i][20+3] = cnstrright;
            }
            else
            {
                z.ptr.pp_double[i][15+3] = _state->v_neginf;
                z.ptr.pp_double[i][20+3] = _state->v_posinf;
            }
            z.ptr.pp_double[i][15+4] = 1.0;
            z.ptr.pp_double[i][20+4] = 1.0;
        }
        
        /*
         * Run outer iterations
         */
        *a = (double)(0);
        *b = (double)(1);
        *c = (double)(1);
        *d = (double)(1);
        *g = (double)(1);
        fbest = ae_maxrealnumber;
        ae_vector_set_length(&p1, 5, _state);
        ae_vector_set_length(&p2, 5, _state);
        for(outerit=0; outerit<=z.rows-1; outerit++)
        {
            
            /*
             * Beginning of the middle iterations.
             * Prepare initial point P1.
             */
            for(i=0; i<=4; i++)
            {
                p1.ptr.p_double[i] = z.ptr.pp_double[outerit][i];
            }
            flast = ae_maxrealnumber;
            for(innerit=0; innerit<=1; innerit++)
            {
                
                /*
                 * Set current boundary constraints.
                 * Run inner iteration.
                 */
                for(i=0; i<=4; i++)
                {
                    bndl.ptr.p_double[i] = z.ptr.pp_double[outerit][5+innerit*10+0+i];
                    bndu.ptr.p_double[i] = z.ptr.pp_double[outerit][5+innerit*10+5+i];
                }
                minlmsetbc(&state, &bndl, &bndu, _state);
                lsfit_logisticfitinternal(x, y, n, ae_true, lambdav, &state, &replm, &p1, &flast, _state);
                rep->iterationscount = rep->iterationscount+replm.iterationscount;
            }
            
            /*
             * Middle iteration: try to fit with 5-parameter logistic model (if needed).
             *
             * We perform two attempts to fit: one with B>0, another one with B<0.
             * For PL4, these are equivalent up to transposition of A/D, but for 5PL
             * sign of B is very important.
             *
             * NOTE: results of 4PL fit are used as initial point for 5PL.
             */
            if( !is4pl )
            {
                
                /*
                 * Loosen constraints on G,
                 * save constraints on A/B/D to B0/B1
                 */
                bndl.ptr.p_double[4] = 0.1;
                bndu.ptr.p_double[4] = 10.0;
                b00 = bndl.ptr.p_double[0];
                b01 = bndu.ptr.p_double[0];
                b10 = bndl.ptr.p_double[1];
                b11 = bndu.ptr.p_double[1];
                b30 = bndl.ptr.p_double[3];
                b31 = bndu.ptr.p_double[3];
                
                /*
                 * First attempt: fitting with positive B
                 */
                p2.ptr.p_double[0] = p1.ptr.p_double[0];
                p2.ptr.p_double[1] = p1.ptr.p_double[1];
                p2.ptr.p_double[2] = p1.ptr.p_double[2];
                p2.ptr.p_double[3] = p1.ptr.p_double[3];
                p2.ptr.p_double[4] = p1.ptr.p_double[4];
                bndl.ptr.p_double[0] = b00;
                bndu.ptr.p_double[0] = b01;
                bndl.ptr.p_double[1] = b10;
                bndu.ptr.p_double[1] = b11;
                bndl.ptr.p_double[3] = b30;
                bndu.ptr.p_double[3] = b31;
                minlmsetbc(&state, &bndl, &bndu, _state);
                lsfit_logisticfitinternal(x, y, n, ae_false, lambdav, &state, &replm, &p2, &flast2, _state);
                rep->iterationscount = rep->iterationscount+replm.iterationscount;
                if( ae_fp_less(flast2,flast) )
                {
                    for(i=0; i<=4; i++)
                    {
                        p1.ptr.p_double[i] = p2.ptr.p_double[i];
                    }
                    flast = flast2;
                }
                
                /*
                 * First attempt: fitting with negative B
                 */
                p2.ptr.p_double[0] = p1.ptr.p_double[3];
                p2.ptr.p_double[1] = -p1.ptr.p_double[1];
                p2.ptr.p_double[2] = p1.ptr.p_double[2];
                p2.ptr.p_double[3] = p1.ptr.p_double[0];
                p2.ptr.p_double[4] = p1.ptr.p_double[4];
                bndl.ptr.p_double[0] = b30;
                bndu.ptr.p_double[0] = b31;
                bndl.ptr.p_double[1] = -b11;
                bndu.ptr.p_double[1] = -b10;
                bndl.ptr.p_double[3] = b00;
                bndu.ptr.p_double[3] = b01;
                minlmsetbc(&state, &bndl, &bndu, _state);
                lsfit_logisticfitinternal(x, y, n, ae_false, lambdav, &state, &replm, &p2, &flast2, _state);
                rep->iterationscount = rep->iterationscount+replm.iterationscount;
                if( ae_fp_less(flast2,flast) )
                {
                    for(i=0; i<=4; i++)
                    {
                        p1.ptr.p_double[i] = p2.ptr.p_double[i];
                    }
                    flast = flast2;
                }
            }
            
            /*
             * Middle iteration is done, compare its results with best value
             * found so far.
             */
            if( ae_fp_less(flast,fbest) )
            {
                *a = p1.ptr.p_double[0];
                *b = p1.ptr.p_double[1];
                *c = p1.ptr.p_double[2];
                *d = p1.ptr.p_double[3];
                *g = p1.ptr.p_double[4];
                fbest = flast;
            }
        }
    }
    
    /*
     * Calculate errors
     */
    rep->rmserror = (double)(0);
    rep->avgerror = (double)(0);
    rep->avgrelerror = (double)(0);
    rep->maxerror = (double)(0);
    k = 0;
    rss = 0.0;
    tss = 0.0;
    meany = 0.0;
    for(i=0; i<=n-1; i++)
    {
        meany = meany+y->ptr.p_double[i];
    }
    meany = meany/n;
    for(i=0; i<=n-1; i++)
    {
        
        /*
         * Calculate residual from regression
         */
        if( ae_fp_greater(x->ptr.p_double[i],(double)(0)) )
        {
            v = *d+(*a-(*d))/ae_pow(1.0+ae_pow(x->ptr.p_double[i]/(*c), *b, _state), *g, _state)-y->ptr.p_double[i];
        }
        else
        {
            if( ae_fp_greater_eq(*b,(double)(0)) )
            {
                v = *a-y->ptr.p_double[i];
            }
            else
            {
                v = *d-y->ptr.p_double[i];
            }
        }
        
        /*
         * Update RSS (residual sum of squares) and TSS (total sum of squares)
         * which are used to calculate coefficient of determination.
         *
         * NOTE: we use formula R2 = 1-RSS/TSS because it has nice property of
         *       being equal to 0.0 if and only if model perfectly fits data.
         *
         *       When we fit nonlinear models, there are exist multiple ways of
         *       determining R2, each of them giving different results. Formula
         *       above is the most intuitive one.
         */
        rss = rss+v*v;
        tss = tss+ae_sqr(y->ptr.p_double[i]-meany, _state);
        
        /*
         * Update errors
         */
        rep->rmserror = rep->rmserror+ae_sqr(v, _state);
        rep->avgerror = rep->avgerror+ae_fabs(v, _state);
        if( ae_fp_neq(y->ptr.p_double[i],(double)(0)) )
        {
            rep->avgrelerror = rep->avgrelerror+ae_fabs(v/y->ptr.p_double[i], _state);
            k = k+1;
        }
        rep->maxerror = ae_maxreal(rep->maxerror, ae_fabs(v, _state), _state);
    }
    rep->rmserror = ae_sqrt(rep->rmserror/n, _state);
    rep->avgerror = rep->avgerror/n;
    if( k>0 )
    {
        rep->avgrelerror = rep->avgrelerror/k;
    }
    rep->r2 = 1.0-rss/tss;
    ae_frame_leave(_state);
}


/*************************************************************************
Weghted rational least  squares  fitting  using  Floater-Hormann  rational
functions  with  optimal  D  chosen  from  [0,9],  with  constraints   and
individual weights.

Equidistant  grid  with M node on [min(x),max(x)]  is  used to build basis
functions. Different values of D are tried, optimal D (least WEIGHTED root
mean square error) is chosen.  Task  is  linear,  so  linear least squares
solver  is  used.  Complexity  of  this  computational  scheme is O(N*M^2)
(mostly dominated by the least squares solver).

SEE ALSO
* BarycentricFitFloaterHormann(), "lightweight" fitting without invididual
  weights and constraints.
  
COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multithreading support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Speed-up provided by multithreading greatly depends  on  problem  size
  ! - only large problems (number of coefficients is more than 500) can be
  ! efficiently multithreaded.
  !
  ! Generally, commercial ALGLIB is several times faster than  open-source
  ! generic C edition, and many times faster than open-source C# edition.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS:
    X   -   points, array[0..N-1].
    Y   -   function values, array[0..N-1].
    W   -   weights, array[0..N-1]
            Each summand in square  sum  of  approximation deviations from
            given  values  is  multiplied  by  the square of corresponding
            weight. Fill it by 1's if you don't  want  to  solve  weighted
            task.
    N   -   number of points, N>0.
    XC  -   points where function values/derivatives are constrained,
            array[0..K-1].
    YC  -   values of constraints, array[0..K-1]
    DC  -   array[0..K-1], types of constraints:
            * DC[i]=0   means that S(XC[i])=YC[i]
            * DC[i]=1   means that S'(XC[i])=YC[i]
            SEE BELOW FOR IMPORTANT INFORMATION ON CONSTRAINTS
    K   -   number of constraints, 0<=K<M.
            K=0 means no constraints (XC/YC/DC are not used in such cases)
    M   -   number of basis functions ( = number_of_nodes), M>=2.

OUTPUT PARAMETERS:
    Info-   same format as in LSFitLinearWC() subroutine.
            * Info>0    task is solved
            * Info<=0   an error occured:
                        -4 means inconvergence of internal SVD
                        -3 means inconsistent constraints
                        -1 means another errors in parameters passed
                           (N<=0, for example)
    B   -   barycentric interpolant.
    Rep -   report, same format as in LSFitLinearWC() subroutine.
            Following fields are set:
            * DBest         best value of the D parameter
            * RMSError      rms error on the (X,Y).
            * AvgError      average error on the (X,Y).
            * AvgRelError   average relative error on the non-zero Y
            * MaxError      maximum error
                            NON-WEIGHTED ERRORS ARE CALCULATED

IMPORTANT:
    this subroutine doesn't calculate task's condition number for K<>0.

SETTING CONSTRAINTS - DANGERS AND OPPORTUNITIES:

Setting constraints can lead  to undesired  results,  like ill-conditioned
behavior, or inconsistency being detected. From the other side,  it allows
us to improve quality of the fit. Here we summarize  our  experience  with
constrained barycentric interpolants:
* excessive  constraints  can  be  inconsistent.   Floater-Hormann   basis
  functions aren't as flexible as splines (although they are very smooth).
* the more evenly constraints are spread across [min(x),max(x)],  the more
  chances that they will be consistent
* the  greater  is  M (given  fixed  constraints),  the  more chances that
  constraints will be consistent
* in the general case, consistency of constraints IS NOT GUARANTEED.
* in the several special cases, however, we CAN guarantee consistency.
* one of this cases is constraints on the function  VALUES at the interval
  boundaries. Note that consustency of the  constraints  on  the  function
  DERIVATIVES is NOT guaranteed (you can use in such cases  cubic  splines
  which are more flexible).
* another  special  case  is ONE constraint on the function value (OR, but
  not AND, derivative) anywhere in the interval

Our final recommendation is to use constraints  WHEN  AND  ONLY  WHEN  you
can't solve your task without them. Anything beyond  special  cases  given
above is not guaranteed and may result in inconsistency.

  -- ALGLIB PROJECT --
     Copyright 18.08.2009 by Bochkanov Sergey
*************************************************************************/
void barycentricfitfloaterhormannwc(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     ae_int_t n,
     /* Real    */ ae_vector* xc,
     /* Real    */ ae_vector* yc,
     /* Integer */ ae_vector* dc,
     ae_int_t k,
     ae_int_t m,
     ae_int_t* info,
     barycentricinterpolant* b,
     barycentricfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t d;
    ae_int_t i;
    double wrmscur;
    double wrmsbest;
    barycentricinterpolant locb;
    barycentricfitreport locrep;
    ae_int_t locinfo;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _barycentricinterpolant_clear(b);
    _barycentricfitreport_clear(rep);
    _barycentricinterpolant_init(&locb, _state);
    _barycentricfitreport_init(&locrep, _state);

    ae_assert(n>0, "BarycentricFitFloaterHormannWC: N<=0!", _state);
    ae_assert(m>0, "BarycentricFitFloaterHormannWC: M<=0!", _state);
    ae_assert(k>=0, "BarycentricFitFloaterHormannWC: K<0!", _state);
    ae_assert(k<m, "BarycentricFitFloaterHormannWC: K>=M!", _state);
    ae_assert(x->cnt>=n, "BarycentricFitFloaterHormannWC: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "BarycentricFitFloaterHormannWC: Length(Y)<N!", _state);
    ae_assert(w->cnt>=n, "BarycentricFitFloaterHormannWC: Length(W)<N!", _state);
    ae_assert(xc->cnt>=k, "BarycentricFitFloaterHormannWC: Length(XC)<K!", _state);
    ae_assert(yc->cnt>=k, "BarycentricFitFloaterHormannWC: Length(YC)<K!", _state);
    ae_assert(dc->cnt>=k, "BarycentricFitFloaterHormannWC: Length(DC)<K!", _state);
    ae_assert(isfinitevector(x, n, _state), "BarycentricFitFloaterHormannWC: X contains infinite or NaN values!", _state);
    ae_assert(isfinitevector(y, n, _state), "BarycentricFitFloaterHormannWC: Y contains infinite or NaN values!", _state);
    ae_assert(isfinitevector(w, n, _state), "BarycentricFitFloaterHormannWC: X contains infinite or NaN values!", _state);
    ae_assert(isfinitevector(xc, k, _state), "BarycentricFitFloaterHormannWC: XC contains infinite or NaN values!", _state);
    ae_assert(isfinitevector(yc, k, _state), "BarycentricFitFloaterHormannWC: YC contains infinite or NaN values!", _state);
    for(i=0; i<=k-1; i++)
    {
        ae_assert(dc->ptr.p_int[i]==0||dc->ptr.p_int[i]==1, "BarycentricFitFloaterHormannWC: one of DC[] is not 0 or 1!", _state);
    }
    
    /*
     * Find optimal D
     *
     * Info is -3 by default (degenerate constraints).
     * If LocInfo will always be equal to -3, Info will remain equal to -3.
     * If at least once LocInfo will be -4, Info will be -4.
     */
    wrmsbest = ae_maxrealnumber;
    rep->dbest = -1;
    *info = -3;
    for(d=0; d<=ae_minint(9, n-1, _state); d++)
    {
        lsfit_barycentricfitwcfixedd(x, y, w, n, xc, yc, dc, k, m, d, &locinfo, &locb, &locrep, _state);
        ae_assert((locinfo==-4||locinfo==-3)||locinfo>0, "BarycentricFitFloaterHormannWC: unexpected result from BarycentricFitWCFixedD!", _state);
        if( locinfo>0 )
        {
            
            /*
             * Calculate weghted RMS
             */
            wrmscur = (double)(0);
            for(i=0; i<=n-1; i++)
            {
                wrmscur = wrmscur+ae_sqr(w->ptr.p_double[i]*(y->ptr.p_double[i]-barycentriccalc(&locb, x->ptr.p_double[i], _state)), _state);
            }
            wrmscur = ae_sqrt(wrmscur/n, _state);
            if( ae_fp_less(wrmscur,wrmsbest)||rep->dbest<0 )
            {
                barycentriccopy(&locb, b, _state);
                rep->dbest = d;
                *info = 1;
                rep->rmserror = locrep.rmserror;
                rep->avgerror = locrep.avgerror;
                rep->avgrelerror = locrep.avgrelerror;
                rep->maxerror = locrep.maxerror;
                rep->taskrcond = locrep.taskrcond;
                wrmsbest = wrmscur;
            }
        }
        else
        {
            if( locinfo!=-3&&*info<0 )
            {
                *info = locinfo;
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_barycentricfitfloaterhormannwc(/* Real    */ ae_vector* x,
    /* Real    */ ae_vector* y,
    /* Real    */ ae_vector* w,
    ae_int_t n,
    /* Real    */ ae_vector* xc,
    /* Real    */ ae_vector* yc,
    /* Integer */ ae_vector* dc,
    ae_int_t k,
    ae_int_t m,
    ae_int_t* info,
    barycentricinterpolant* b,
    barycentricfitreport* rep, ae_state *_state)
{
    barycentricfitfloaterhormannwc(x,y,w,n,xc,yc,dc,k,m,info,b,rep, _state);
}


/*************************************************************************
Rational least squares fitting using  Floater-Hormann  rational  functions
with optimal D chosen from [0,9].

Equidistant  grid  with M node on [min(x),max(x)]  is  used to build basis
functions. Different values of D are tried, optimal  D  (least  root  mean
square error) is chosen.  Task  is  linear, so linear least squares solver
is used. Complexity  of  this  computational  scheme is  O(N*M^2)  (mostly
dominated by the least squares solver).

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multithreading support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Speed-up provided by multithreading greatly depends  on  problem  size
  ! - only large problems (number of coefficients is more than 500) can be
  ! efficiently multithreaded.
  !
  ! Generally, commercial ALGLIB is several times faster than  open-source
  ! generic C edition, and many times faster than open-source C# edition.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS:
    X   -   points, array[0..N-1].
    Y   -   function values, array[0..N-1].
    N   -   number of points, N>0.
    M   -   number of basis functions ( = number_of_nodes), M>=2.

OUTPUT PARAMETERS:
    Info-   same format as in LSFitLinearWC() subroutine.
            * Info>0    task is solved
            * Info<=0   an error occured:
                        -4 means inconvergence of internal SVD
                        -3 means inconsistent constraints
    B   -   barycentric interpolant.
    Rep -   report, same format as in LSFitLinearWC() subroutine.
            Following fields are set:
            * DBest         best value of the D parameter
            * RMSError      rms error on the (X,Y).
            * AvgError      average error on the (X,Y).
            * AvgRelError   average relative error on the non-zero Y
            * MaxError      maximum error
                            NON-WEIGHTED ERRORS ARE CALCULATED

  -- ALGLIB PROJECT --
     Copyright 18.08.2009 by Bochkanov Sergey
*************************************************************************/
void barycentricfitfloaterhormann(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t m,
     ae_int_t* info,
     barycentricinterpolant* b,
     barycentricfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector w;
    ae_vector xc;
    ae_vector yc;
    ae_vector dc;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _barycentricinterpolant_clear(b);
    _barycentricfitreport_clear(rep);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&xc, 0, DT_REAL, _state);
    ae_vector_init(&yc, 0, DT_REAL, _state);
    ae_vector_init(&dc, 0, DT_INT, _state);

    ae_assert(n>0, "BarycentricFitFloaterHormann: N<=0!", _state);
    ae_assert(m>0, "BarycentricFitFloaterHormann: M<=0!", _state);
    ae_assert(x->cnt>=n, "BarycentricFitFloaterHormann: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "BarycentricFitFloaterHormann: Length(Y)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "BarycentricFitFloaterHormann: X contains infinite or NaN values!", _state);
    ae_assert(isfinitevector(y, n, _state), "BarycentricFitFloaterHormann: Y contains infinite or NaN values!", _state);
    ae_vector_set_length(&w, n, _state);
    for(i=0; i<=n-1; i++)
    {
        w.ptr.p_double[i] = (double)(1);
    }
    barycentricfitfloaterhormannwc(x, y, &w, n, &xc, &yc, &dc, 0, m, info, b, rep, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_barycentricfitfloaterhormann(/* Real    */ ae_vector* x,
    /* Real    */ ae_vector* y,
    ae_int_t n,
    ae_int_t m,
    ae_int_t* info,
    barycentricinterpolant* b,
    barycentricfitreport* rep, ae_state *_state)
{
    barycentricfitfloaterhormann(x,y,n,m,info,b,rep, _state);
}


/*************************************************************************
Fitting by penalized cubic spline.

Equidistant grid with M nodes on [min(x,xc),max(x,xc)] is  used  to  build
basis functions. Basis functions are cubic splines with  natural  boundary
conditions. Problem is regularized by  adding non-linearity penalty to the
usual least squares penalty function:

    S(x) = arg min { LS + P }, where
    LS   = SUM { w[i]^2*(y[i] - S(x[i]))^2 } - least squares penalty
    P    = C*10^rho*integral{ S''(x)^2*dx } - non-linearity penalty
    rho  - tunable constant given by user
    C    - automatically determined scale parameter,
           makes penalty invariant with respect to scaling of X, Y, W.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multithreading support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Speed-up provided by multithreading greatly depends  on  problem  size
  ! - only large problems (number of coefficients is more than 500) can be
  ! efficiently multithreaded.
  !
  ! Generally, commercial ALGLIB is several times faster than  open-source
  ! generic C edition, and many times faster than open-source C# edition.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.
           
INPUT PARAMETERS:
    X   -   points, array[0..N-1].
    Y   -   function values, array[0..N-1].
    N   -   number of points (optional):
            * N>0
            * if given, only first N elements of X/Y are processed
            * if not given, automatically determined from X/Y sizes
    M   -   number of basis functions ( = number_of_nodes), M>=4.
    Rho -   regularization  constant  passed   by   user.   It   penalizes
            nonlinearity in the regression spline. It  is  logarithmically
            scaled,  i.e.  actual  value  of  regularization  constant  is
            calculated as 10^Rho. It is automatically scaled so that:
            * Rho=2.0 corresponds to moderate amount of nonlinearity
            * generally, it should be somewhere in the [-8.0,+8.0]
            If you do not want to penalize nonlineary,
            pass small Rho. Values as low as -15 should work.

OUTPUT PARAMETERS:
    Info-   same format as in LSFitLinearWC() subroutine.
            * Info>0    task is solved
            * Info<=0   an error occured:
                        -4 means inconvergence of internal SVD or
                           Cholesky decomposition; problem may be
                           too ill-conditioned (very rare)
    S   -   spline interpolant.
    Rep -   Following fields are set:
            * RMSError      rms error on the (X,Y).
            * AvgError      average error on the (X,Y).
            * AvgRelError   average relative error on the non-zero Y
            * MaxError      maximum error
                            NON-WEIGHTED ERRORS ARE CALCULATED

IMPORTANT:
    this subroitine doesn't calculate task's condition number for K<>0.

NOTE 1: additional nodes are added to the spline outside  of  the  fitting
interval to force linearity when x<min(x,xc) or x>max(x,xc).  It  is  done
for consistency - we penalize non-linearity  at [min(x,xc),max(x,xc)],  so
it is natural to force linearity outside of this interval.

NOTE 2: function automatically sorts points,  so  caller may pass unsorted
array.

  -- ALGLIB PROJECT --
     Copyright 18.08.2009 by Bochkanov Sergey
*************************************************************************/
void spline1dfitpenalized(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t m,
     double rho,
     ae_int_t* info,
     spline1dinterpolant* s,
     spline1dfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_vector w;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    *info = 0;
    _spline1dinterpolant_clear(s);
    _spline1dfitreport_clear(rep);
    ae_vector_init(&w, 0, DT_REAL, _state);

    ae_assert(n>=1, "Spline1DFitPenalized: N<1!", _state);
    ae_assert(m>=4, "Spline1DFitPenalized: M<4!", _state);
    ae_assert(x->cnt>=n, "Spline1DFitPenalized: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Spline1DFitPenalized: Length(Y)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "Spline1DFitPenalized: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(y, n, _state), "Spline1DFitPenalized: Y contains infinite or NAN values!", _state);
    ae_assert(ae_isfinite(rho, _state), "Spline1DFitPenalized: Rho is infinite!", _state);
    ae_vector_set_length(&w, n, _state);
    for(i=0; i<=n-1; i++)
    {
        w.ptr.p_double[i] = (double)(1);
    }
    spline1dfitpenalizedw(x, y, &w, n, m, rho, info, s, rep, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_spline1dfitpenalized(/* Real    */ ae_vector* x,
    /* Real    */ ae_vector* y,
    ae_int_t n,
    ae_int_t m,
    double rho,
    ae_int_t* info,
    spline1dinterpolant* s,
    spline1dfitreport* rep, ae_state *_state)
{
    spline1dfitpenalized(x,y,n,m,rho,info,s,rep, _state);
}


/*************************************************************************
Weighted fitting by penalized cubic spline.

Equidistant grid with M nodes on [min(x,xc),max(x,xc)] is  used  to  build
basis functions. Basis functions are cubic splines with  natural  boundary
conditions. Problem is regularized by  adding non-linearity penalty to the
usual least squares penalty function:

    S(x) = arg min { LS + P }, where
    LS   = SUM { w[i]^2*(y[i] - S(x[i]))^2 } - least squares penalty
    P    = C*10^rho*integral{ S''(x)^2*dx } - non-linearity penalty
    rho  - tunable constant given by user
    C    - automatically determined scale parameter,
           makes penalty invariant with respect to scaling of X, Y, W.

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multithreading support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Speed-up provided by multithreading greatly depends  on  problem  size
  ! - only large problems (number of coefficients is more than 500) can be
  ! efficiently multithreaded.
  !
  ! Generally, commercial ALGLIB is several times faster than  open-source
  ! generic C edition, and many times faster than open-source C# edition.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.
           
INPUT PARAMETERS:
    X   -   points, array[0..N-1].
    Y   -   function values, array[0..N-1].
    W   -   weights, array[0..N-1]
            Each summand in square  sum  of  approximation deviations from
            given  values  is  multiplied  by  the square of corresponding
            weight. Fill it by 1's if you don't  want  to  solve  weighted
            problem.
    N   -   number of points (optional):
            * N>0
            * if given, only first N elements of X/Y/W are processed
            * if not given, automatically determined from X/Y/W sizes
    M   -   number of basis functions ( = number_of_nodes), M>=4.
    Rho -   regularization  constant  passed   by   user.   It   penalizes
            nonlinearity in the regression spline. It  is  logarithmically
            scaled,  i.e.  actual  value  of  regularization  constant  is
            calculated as 10^Rho. It is automatically scaled so that:
            * Rho=2.0 corresponds to moderate amount of nonlinearity
            * generally, it should be somewhere in the [-8.0,+8.0]
            If you do not want to penalize nonlineary,
            pass small Rho. Values as low as -15 should work.

OUTPUT PARAMETERS:
    Info-   same format as in LSFitLinearWC() subroutine.
            * Info>0    task is solved
            * Info<=0   an error occured:
                        -4 means inconvergence of internal SVD or
                           Cholesky decomposition; problem may be
                           too ill-conditioned (very rare)
    S   -   spline interpolant.
    Rep -   Following fields are set:
            * RMSError      rms error on the (X,Y).
            * AvgError      average error on the (X,Y).
            * AvgRelError   average relative error on the non-zero Y
            * MaxError      maximum error
                            NON-WEIGHTED ERRORS ARE CALCULATED

IMPORTANT:
    this subroitine doesn't calculate task's condition number for K<>0.

NOTE 1: additional nodes are added to the spline outside  of  the  fitting
interval to force linearity when x<min(x,xc) or x>max(x,xc).  It  is  done
for consistency - we penalize non-linearity  at [min(x,xc),max(x,xc)],  so
it is natural to force linearity outside of this interval.

NOTE 2: function automatically sorts points,  so  caller may pass unsorted
array.

  -- ALGLIB PROJECT --
     Copyright 19.10.2010 by Bochkanov Sergey
*************************************************************************/
void spline1dfitpenalizedw(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     ae_int_t n,
     ae_int_t m,
     double rho,
     ae_int_t* info,
     spline1dinterpolant* s,
     spline1dfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_vector _w;
    ae_int_t i;
    ae_int_t j;
    ae_int_t b;
    double v;
    double relcnt;
    double xa;
    double xb;
    double sa;
    double sb;
    ae_vector xoriginal;
    ae_vector yoriginal;
    double pdecay;
    double tdecay;
    ae_matrix fmatrix;
    ae_vector fcolumn;
    ae_vector y2;
    ae_vector w2;
    ae_vector xc;
    ae_vector yc;
    ae_vector dc;
    double fdmax;
    double admax;
    ae_matrix amatrix;
    ae_matrix d2matrix;
    double fa;
    double ga;
    double fb;
    double gb;
    double lambdav;
    ae_vector bx;
    ae_vector by;
    ae_vector bd1;
    ae_vector bd2;
    ae_vector tx;
    ae_vector ty;
    ae_vector td;
    spline1dinterpolant bs;
    ae_matrix nmatrix;
    ae_vector rightpart;
    fblslincgstate cgstate;
    ae_vector c;
    ae_vector tmp0;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    ae_vector_init_copy(&_w, w, _state);
    w = &_w;
    *info = 0;
    _spline1dinterpolant_clear(s);
    _spline1dfitreport_clear(rep);
    ae_vector_init(&xoriginal, 0, DT_REAL, _state);
    ae_vector_init(&yoriginal, 0, DT_REAL, _state);
    ae_matrix_init(&fmatrix, 0, 0, DT_REAL, _state);
    ae_vector_init(&fcolumn, 0, DT_REAL, _state);
    ae_vector_init(&y2, 0, DT_REAL, _state);
    ae_vector_init(&w2, 0, DT_REAL, _state);
    ae_vector_init(&xc, 0, DT_REAL, _state);
    ae_vector_init(&yc, 0, DT_REAL, _state);
    ae_vector_init(&dc, 0, DT_INT, _state);
    ae_matrix_init(&amatrix, 0, 0, DT_REAL, _state);
    ae_matrix_init(&d2matrix, 0, 0, DT_REAL, _state);
    ae_vector_init(&bx, 0, DT_REAL, _state);
    ae_vector_init(&by, 0, DT_REAL, _state);
    ae_vector_init(&bd1, 0, DT_REAL, _state);
    ae_vector_init(&bd2, 0, DT_REAL, _state);
    ae_vector_init(&tx, 0, DT_REAL, _state);
    ae_vector_init(&ty, 0, DT_REAL, _state);
    ae_vector_init(&td, 0, DT_REAL, _state);
    _spline1dinterpolant_init(&bs, _state);
    ae_matrix_init(&nmatrix, 0, 0, DT_REAL, _state);
    ae_vector_init(&rightpart, 0, DT_REAL, _state);
    _fblslincgstate_init(&cgstate, _state);
    ae_vector_init(&c, 0, DT_REAL, _state);
    ae_vector_init(&tmp0, 0, DT_REAL, _state);

    ae_assert(n>=1, "Spline1DFitPenalizedW: N<1!", _state);
    ae_assert(m>=4, "Spline1DFitPenalizedW: M<4!", _state);
    ae_assert(x->cnt>=n, "Spline1DFitPenalizedW: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Spline1DFitPenalizedW: Length(Y)<N!", _state);
    ae_assert(w->cnt>=n, "Spline1DFitPenalizedW: Length(W)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "Spline1DFitPenalizedW: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(y, n, _state), "Spline1DFitPenalizedW: Y contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(w, n, _state), "Spline1DFitPenalizedW: Y contains infinite or NAN values!", _state);
    ae_assert(ae_isfinite(rho, _state), "Spline1DFitPenalizedW: Rho is infinite!", _state);
    
    /*
     * Prepare LambdaV
     */
    v = -ae_log(ae_machineepsilon, _state)/ae_log((double)(10), _state);
    if( ae_fp_less(rho,-v) )
    {
        rho = -v;
    }
    if( ae_fp_greater(rho,v) )
    {
        rho = v;
    }
    lambdav = ae_pow((double)(10), rho, _state);
    
    /*
     * Sort X, Y, W
     */
    heapsortdpoints(x, y, w, n, _state);
    
    /*
     * Scale X, Y, XC, YC
     */
    lsfitscalexy(x, y, w, n, &xc, &yc, &dc, 0, &xa, &xb, &sa, &sb, &xoriginal, &yoriginal, _state);
    
    /*
     * Allocate space
     */
    ae_matrix_set_length(&fmatrix, n, m, _state);
    ae_matrix_set_length(&amatrix, m, m, _state);
    ae_matrix_set_length(&d2matrix, m, m, _state);
    ae_vector_set_length(&bx, m, _state);
    ae_vector_set_length(&by, m, _state);
    ae_vector_set_length(&fcolumn, n, _state);
    ae_matrix_set_length(&nmatrix, m, m, _state);
    ae_vector_set_length(&rightpart, m, _state);
    ae_vector_set_length(&tmp0, ae_maxint(m, n, _state), _state);
    ae_vector_set_length(&c, m, _state);
    
    /*
     * Fill:
     * * FMatrix by values of basis functions
     * * TmpAMatrix by second derivatives of I-th function at J-th point
     * * CMatrix by constraints
     */
    fdmax = (double)(0);
    for(b=0; b<=m-1; b++)
    {
        
        /*
         * Prepare I-th basis function
         */
        for(j=0; j<=m-1; j++)
        {
            bx.ptr.p_double[j] = (double)(2*j)/(double)(m-1)-1;
            by.ptr.p_double[j] = (double)(0);
        }
        by.ptr.p_double[b] = (double)(1);
        spline1dgriddiff2cubic(&bx, &by, m, 2, 0.0, 2, 0.0, &bd1, &bd2, _state);
        spline1dbuildcubic(&bx, &by, m, 2, 0.0, 2, 0.0, &bs, _state);
        
        /*
         * Calculate B-th column of FMatrix
         * Update FDMax (maximum column norm)
         */
        spline1dconvcubic(&bx, &by, m, 2, 0.0, 2, 0.0, x, n, &fcolumn, _state);
        ae_v_move(&fmatrix.ptr.pp_double[0][b], fmatrix.stride, &fcolumn.ptr.p_double[0], 1, ae_v_len(0,n-1));
        v = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            v = v+ae_sqr(w->ptr.p_double[i]*fcolumn.ptr.p_double[i], _state);
        }
        fdmax = ae_maxreal(fdmax, v, _state);
        
        /*
         * Fill temporary with second derivatives of basis function
         */
        ae_v_move(&d2matrix.ptr.pp_double[b][0], 1, &bd2.ptr.p_double[0], 1, ae_v_len(0,m-1));
    }
    
    /*
     * * calculate penalty matrix A
     * * calculate max of diagonal elements of A
     * * calculate PDecay - coefficient before penalty matrix
     */
    for(i=0; i<=m-1; i++)
    {
        for(j=i; j<=m-1; j++)
        {
            
            /*
             * calculate integral(B_i''*B_j'') where B_i and B_j are
             * i-th and j-th basis splines.
             * B_i and B_j are piecewise linear functions.
             */
            v = (double)(0);
            for(b=0; b<=m-2; b++)
            {
                fa = d2matrix.ptr.pp_double[i][b];
                fb = d2matrix.ptr.pp_double[i][b+1];
                ga = d2matrix.ptr.pp_double[j][b];
                gb = d2matrix.ptr.pp_double[j][b+1];
                v = v+(bx.ptr.p_double[b+1]-bx.ptr.p_double[b])*(fa*ga+(fa*(gb-ga)+ga*(fb-fa))/2+(fb-fa)*(gb-ga)/3);
            }
            amatrix.ptr.pp_double[i][j] = v;
            amatrix.ptr.pp_double[j][i] = v;
        }
    }
    admax = (double)(0);
    for(i=0; i<=m-1; i++)
    {
        admax = ae_maxreal(admax, ae_fabs(amatrix.ptr.pp_double[i][i], _state), _state);
    }
    pdecay = lambdav*fdmax/admax;
    
    /*
     * Calculate TDecay for Tikhonov regularization
     */
    tdecay = fdmax*(1+pdecay)*10*ae_machineepsilon;
    
    /*
     * Prepare system
     *
     * NOTE: FMatrix is spoiled during this process
     */
    for(i=0; i<=n-1; i++)
    {
        v = w->ptr.p_double[i];
        ae_v_muld(&fmatrix.ptr.pp_double[i][0], 1, ae_v_len(0,m-1), v);
    }
    rmatrixgemm(m, m, n, 1.0, &fmatrix, 0, 0, 1, &fmatrix, 0, 0, 0, 0.0, &nmatrix, 0, 0, _state);
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=m-1; j++)
        {
            nmatrix.ptr.pp_double[i][j] = nmatrix.ptr.pp_double[i][j]+pdecay*amatrix.ptr.pp_double[i][j];
        }
    }
    for(i=0; i<=m-1; i++)
    {
        nmatrix.ptr.pp_double[i][i] = nmatrix.ptr.pp_double[i][i]+tdecay;
    }
    for(i=0; i<=m-1; i++)
    {
        rightpart.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=n-1; i++)
    {
        v = y->ptr.p_double[i]*w->ptr.p_double[i];
        ae_v_addd(&rightpart.ptr.p_double[0], 1, &fmatrix.ptr.pp_double[i][0], 1, ae_v_len(0,m-1), v);
    }
    
    /*
     * Solve system
     */
    if( !spdmatrixcholesky(&nmatrix, m, ae_true, _state) )
    {
        *info = -4;
        ae_frame_leave(_state);
        return;
    }
    fblscholeskysolve(&nmatrix, 1.0, m, ae_true, &rightpart, &tmp0, _state);
    ae_v_move(&c.ptr.p_double[0], 1, &rightpart.ptr.p_double[0], 1, ae_v_len(0,m-1));
    
    /*
     * add nodes to force linearity outside of the fitting interval
     */
    spline1dgriddiffcubic(&bx, &c, m, 2, 0.0, 2, 0.0, &bd1, _state);
    ae_vector_set_length(&tx, m+2, _state);
    ae_vector_set_length(&ty, m+2, _state);
    ae_vector_set_length(&td, m+2, _state);
    ae_v_move(&tx.ptr.p_double[1], 1, &bx.ptr.p_double[0], 1, ae_v_len(1,m));
    ae_v_move(&ty.ptr.p_double[1], 1, &rightpart.ptr.p_double[0], 1, ae_v_len(1,m));
    ae_v_move(&td.ptr.p_double[1], 1, &bd1.ptr.p_double[0], 1, ae_v_len(1,m));
    tx.ptr.p_double[0] = tx.ptr.p_double[1]-(tx.ptr.p_double[2]-tx.ptr.p_double[1]);
    ty.ptr.p_double[0] = ty.ptr.p_double[1]-td.ptr.p_double[1]*(tx.ptr.p_double[2]-tx.ptr.p_double[1]);
    td.ptr.p_double[0] = td.ptr.p_double[1];
    tx.ptr.p_double[m+1] = tx.ptr.p_double[m]+(tx.ptr.p_double[m]-tx.ptr.p_double[m-1]);
    ty.ptr.p_double[m+1] = ty.ptr.p_double[m]+td.ptr.p_double[m]*(tx.ptr.p_double[m]-tx.ptr.p_double[m-1]);
    td.ptr.p_double[m+1] = td.ptr.p_double[m];
    spline1dbuildhermite(&tx, &ty, &td, m+2, s, _state);
    spline1dlintransx(s, 2/(xb-xa), -(xa+xb)/(xb-xa), _state);
    spline1dlintransy(s, sb-sa, sa, _state);
    *info = 1;
    
    /*
     * Fill report
     */
    rep->rmserror = (double)(0);
    rep->avgerror = (double)(0);
    rep->avgrelerror = (double)(0);
    rep->maxerror = (double)(0);
    relcnt = (double)(0);
    spline1dconvcubic(&bx, &rightpart, m, 2, 0.0, 2, 0.0, x, n, &fcolumn, _state);
    for(i=0; i<=n-1; i++)
    {
        v = (sb-sa)*fcolumn.ptr.p_double[i]+sa;
        rep->rmserror = rep->rmserror+ae_sqr(v-yoriginal.ptr.p_double[i], _state);
        rep->avgerror = rep->avgerror+ae_fabs(v-yoriginal.ptr.p_double[i], _state);
        if( ae_fp_neq(yoriginal.ptr.p_double[i],(double)(0)) )
        {
            rep->avgrelerror = rep->avgrelerror+ae_fabs(v-yoriginal.ptr.p_double[i], _state)/ae_fabs(yoriginal.ptr.p_double[i], _state);
            relcnt = relcnt+1;
        }
        rep->maxerror = ae_maxreal(rep->maxerror, ae_fabs(v-yoriginal.ptr.p_double[i], _state), _state);
    }
    rep->rmserror = ae_sqrt(rep->rmserror/n, _state);
    rep->avgerror = rep->avgerror/n;
    if( ae_fp_neq(relcnt,(double)(0)) )
    {
        rep->avgrelerror = rep->avgrelerror/relcnt;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_spline1dfitpenalizedw(/* Real    */ ae_vector* x,
    /* Real    */ ae_vector* y,
    /* Real    */ ae_vector* w,
    ae_int_t n,
    ae_int_t m,
    double rho,
    ae_int_t* info,
    spline1dinterpolant* s,
    spline1dfitreport* rep, ae_state *_state)
{
    spline1dfitpenalizedw(x,y,w,n,m,rho,info,s,rep, _state);
}


/*************************************************************************
Weighted fitting by cubic  spline,  with constraints on function values or
derivatives.

Equidistant grid with M-2 nodes on [min(x,xc),max(x,xc)] is  used to build
basis functions. Basis functions are cubic splines with continuous  second
derivatives  and  non-fixed first  derivatives  at  interval  ends.  Small
regularizing term is used  when  solving  constrained  tasks  (to  improve
stability).

Task is linear, so linear least squares solver is used. Complexity of this
computational scheme is O(N*M^2), mostly dominated by least squares solver

SEE ALSO
    Spline1DFitHermiteWC()  -   fitting by Hermite splines (more flexible,
                                less smooth)
    Spline1DFitCubic()      -   "lightweight" fitting  by  cubic  splines,
                                without invididual weights and constraints

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multithreading support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Speed-up provided by multithreading greatly depends  on  problem  size
  ! - only large problems (number of coefficients is more than 500) can be
  ! efficiently multithreaded.
  !
  ! Generally, commercial ALGLIB is several times faster than  open-source
  ! generic C edition, and many times faster than open-source C# edition.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.
                                
INPUT PARAMETERS:
    X   -   points, array[0..N-1].
    Y   -   function values, array[0..N-1].
    W   -   weights, array[0..N-1]
            Each summand in square  sum  of  approximation deviations from
            given  values  is  multiplied  by  the square of corresponding
            weight. Fill it by 1's if you don't  want  to  solve  weighted
            task.
    N   -   number of points (optional):
            * N>0
            * if given, only first N elements of X/Y/W are processed
            * if not given, automatically determined from X/Y/W sizes
    XC  -   points where spline values/derivatives are constrained,
            array[0..K-1].
    YC  -   values of constraints, array[0..K-1]
    DC  -   array[0..K-1], types of constraints:
            * DC[i]=0   means that S(XC[i])=YC[i]
            * DC[i]=1   means that S'(XC[i])=YC[i]
            SEE BELOW FOR IMPORTANT INFORMATION ON CONSTRAINTS
    K   -   number of constraints (optional):
            * 0<=K<M.
            * K=0 means no constraints (XC/YC/DC are not used)
            * if given, only first K elements of XC/YC/DC are used
            * if not given, automatically determined from XC/YC/DC
    M   -   number of basis functions ( = number_of_nodes+2), M>=4.

OUTPUT PARAMETERS:
    Info-   same format as in LSFitLinearWC() subroutine.
            * Info>0    task is solved
            * Info<=0   an error occured:
                        -4 means inconvergence of internal SVD
                        -3 means inconsistent constraints
    S   -   spline interpolant.
    Rep -   report, same format as in LSFitLinearWC() subroutine.
            Following fields are set:
            * RMSError      rms error on the (X,Y).
            * AvgError      average error on the (X,Y).
            * AvgRelError   average relative error on the non-zero Y
            * MaxError      maximum error
                            NON-WEIGHTED ERRORS ARE CALCULATED

IMPORTANT:
    this subroitine doesn't calculate task's condition number for K<>0.


ORDER OF POINTS

Subroutine automatically sorts points, so caller may pass unsorted array.

SETTING CONSTRAINTS - DANGERS AND OPPORTUNITIES:

Setting constraints can lead  to undesired  results,  like ill-conditioned
behavior, or inconsistency being detected. From the other side,  it allows
us to improve quality of the fit. Here we summarize  our  experience  with
constrained regression splines:
* excessive constraints can be inconsistent. Splines are  piecewise  cubic
  functions, and it is easy to create an example, where  large  number  of
  constraints  concentrated  in  small  area will result in inconsistency.
  Just because spline is not flexible enough to satisfy all of  them.  And
  same constraints spread across the  [min(x),max(x)]  will  be  perfectly
  consistent.
* the more evenly constraints are spread across [min(x),max(x)],  the more
  chances that they will be consistent
* the  greater  is  M (given  fixed  constraints),  the  more chances that
  constraints will be consistent
* in the general case, consistency of constraints IS NOT GUARANTEED.
* in the several special cases, however, we CAN guarantee consistency.
* one of this cases is constraints  on  the  function  values  AND/OR  its
  derivatives at the interval boundaries.
* another  special  case  is ONE constraint on the function value (OR, but
  not AND, derivative) anywhere in the interval

Our final recommendation is to use constraints  WHEN  AND  ONLY  WHEN  you
can't solve your task without them. Anything beyond  special  cases  given
above is not guaranteed and may result in inconsistency.


  -- ALGLIB PROJECT --
     Copyright 18.08.2009 by Bochkanov Sergey
*************************************************************************/
void spline1dfitcubicwc(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     ae_int_t n,
     /* Real    */ ae_vector* xc,
     /* Real    */ ae_vector* yc,
     /* Integer */ ae_vector* dc,
     ae_int_t k,
     ae_int_t m,
     ae_int_t* info,
     spline1dinterpolant* s,
     spline1dfitreport* rep,
     ae_state *_state)
{
    ae_int_t i;

    *info = 0;
    _spline1dinterpolant_clear(s);
    _spline1dfitreport_clear(rep);

    ae_assert(n>=1, "Spline1DFitCubicWC: N<1!", _state);
    ae_assert(m>=4, "Spline1DFitCubicWC: M<4!", _state);
    ae_assert(k>=0, "Spline1DFitCubicWC: K<0!", _state);
    ae_assert(k<m, "Spline1DFitCubicWC: K>=M!", _state);
    ae_assert(x->cnt>=n, "Spline1DFitCubicWC: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Spline1DFitCubicWC: Length(Y)<N!", _state);
    ae_assert(w->cnt>=n, "Spline1DFitCubicWC: Length(W)<N!", _state);
    ae_assert(xc->cnt>=k, "Spline1DFitCubicWC: Length(XC)<K!", _state);
    ae_assert(yc->cnt>=k, "Spline1DFitCubicWC: Length(YC)<K!", _state);
    ae_assert(dc->cnt>=k, "Spline1DFitCubicWC: Length(DC)<K!", _state);
    ae_assert(isfinitevector(x, n, _state), "Spline1DFitCubicWC: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(y, n, _state), "Spline1DFitCubicWC: Y contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(w, n, _state), "Spline1DFitCubicWC: Y contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(xc, k, _state), "Spline1DFitCubicWC: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(yc, k, _state), "Spline1DFitCubicWC: Y contains infinite or NAN values!", _state);
    for(i=0; i<=k-1; i++)
    {
        ae_assert(dc->ptr.p_int[i]==0||dc->ptr.p_int[i]==1, "Spline1DFitCubicWC: DC[i] is neither 0 or 1!", _state);
    }
    lsfit_spline1dfitinternal(0, x, y, w, n, xc, yc, dc, k, m, info, s, rep, _state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_spline1dfitcubicwc(/* Real    */ ae_vector* x,
    /* Real    */ ae_vector* y,
    /* Real    */ ae_vector* w,
    ae_int_t n,
    /* Real    */ ae_vector* xc,
    /* Real    */ ae_vector* yc,
    /* Integer */ ae_vector* dc,
    ae_int_t k,
    ae_int_t m,
    ae_int_t* info,
    spline1dinterpolant* s,
    spline1dfitreport* rep, ae_state *_state)
{
    spline1dfitcubicwc(x,y,w,n,xc,yc,dc,k,m,info,s,rep, _state);
}


/*************************************************************************
Weighted  fitting  by Hermite spline,  with constraints on function values
or first derivatives.

Equidistant grid with M nodes on [min(x,xc),max(x,xc)] is  used  to  build
basis functions. Basis functions are Hermite splines.  Small  regularizing
term is used when solving constrained tasks (to improve stability).

Task is linear, so linear least squares solver is used. Complexity of this
computational scheme is O(N*M^2), mostly dominated by least squares solver

SEE ALSO
    Spline1DFitCubicWC()    -   fitting by Cubic splines (less flexible,
                                more smooth)
    Spline1DFitHermite()    -   "lightweight" Hermite fitting, without
                                invididual weights and constraints

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multithreading support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Speed-up provided by multithreading greatly depends  on  problem  size
  ! - only large problems (number of coefficients is more than 500) can be
  ! efficiently multithreaded.
  !
  ! Generally, commercial ALGLIB is several times faster than  open-source
  ! generic C edition, and many times faster than open-source C# edition.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.
                                
INPUT PARAMETERS:
    X   -   points, array[0..N-1].
    Y   -   function values, array[0..N-1].
    W   -   weights, array[0..N-1]
            Each summand in square  sum  of  approximation deviations from
            given  values  is  multiplied  by  the square of corresponding
            weight. Fill it by 1's if you don't  want  to  solve  weighted
            task.
    N   -   number of points (optional):
            * N>0
            * if given, only first N elements of X/Y/W are processed
            * if not given, automatically determined from X/Y/W sizes
    XC  -   points where spline values/derivatives are constrained,
            array[0..K-1].
    YC  -   values of constraints, array[0..K-1]
    DC  -   array[0..K-1], types of constraints:
            * DC[i]=0   means that S(XC[i])=YC[i]
            * DC[i]=1   means that S'(XC[i])=YC[i]
            SEE BELOW FOR IMPORTANT INFORMATION ON CONSTRAINTS
    K   -   number of constraints (optional):
            * 0<=K<M.
            * K=0 means no constraints (XC/YC/DC are not used)
            * if given, only first K elements of XC/YC/DC are used
            * if not given, automatically determined from XC/YC/DC
    M   -   number of basis functions (= 2 * number of nodes),
            M>=4,
            M IS EVEN!

OUTPUT PARAMETERS:
    Info-   same format as in LSFitLinearW() subroutine:
            * Info>0    task is solved
            * Info<=0   an error occured:
                        -4 means inconvergence of internal SVD
                        -3 means inconsistent constraints
                        -2 means odd M was passed (which is not supported)
                        -1 means another errors in parameters passed
                           (N<=0, for example)
    S   -   spline interpolant.
    Rep -   report, same format as in LSFitLinearW() subroutine.
            Following fields are set:
            * RMSError      rms error on the (X,Y).
            * AvgError      average error on the (X,Y).
            * AvgRelError   average relative error on the non-zero Y
            * MaxError      maximum error
                            NON-WEIGHTED ERRORS ARE CALCULATED

IMPORTANT:
    this subroitine doesn't calculate task's condition number for K<>0.

IMPORTANT:
    this subroitine supports only even M's


ORDER OF POINTS

Subroutine automatically sorts points, so caller may pass unsorted array.

SETTING CONSTRAINTS - DANGERS AND OPPORTUNITIES:

Setting constraints can lead  to undesired  results,  like ill-conditioned
behavior, or inconsistency being detected. From the other side,  it allows
us to improve quality of the fit. Here we summarize  our  experience  with
constrained regression splines:
* excessive constraints can be inconsistent. Splines are  piecewise  cubic
  functions, and it is easy to create an example, where  large  number  of
  constraints  concentrated  in  small  area will result in inconsistency.
  Just because spline is not flexible enough to satisfy all of  them.  And
  same constraints spread across the  [min(x),max(x)]  will  be  perfectly
  consistent.
* the more evenly constraints are spread across [min(x),max(x)],  the more
  chances that they will be consistent
* the  greater  is  M (given  fixed  constraints),  the  more chances that
  constraints will be consistent
* in the general case, consistency of constraints is NOT GUARANTEED.
* in the several special cases, however, we can guarantee consistency.
* one of this cases is  M>=4  and   constraints  on   the  function  value
  (AND/OR its derivative) at the interval boundaries.
* another special case is M>=4  and  ONE  constraint on the function value
  (OR, BUT NOT AND, derivative) anywhere in [min(x),max(x)]

Our final recommendation is to use constraints  WHEN  AND  ONLY  when  you
can't solve your task without them. Anything beyond  special  cases  given
above is not guaranteed and may result in inconsistency.

  -- ALGLIB PROJECT --
     Copyright 18.08.2009 by Bochkanov Sergey
*************************************************************************/
void spline1dfithermitewc(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     ae_int_t n,
     /* Real    */ ae_vector* xc,
     /* Real    */ ae_vector* yc,
     /* Integer */ ae_vector* dc,
     ae_int_t k,
     ae_int_t m,
     ae_int_t* info,
     spline1dinterpolant* s,
     spline1dfitreport* rep,
     ae_state *_state)
{
    ae_int_t i;

    *info = 0;
    _spline1dinterpolant_clear(s);
    _spline1dfitreport_clear(rep);

    ae_assert(n>=1, "Spline1DFitHermiteWC: N<1!", _state);
    ae_assert(m>=4, "Spline1DFitHermiteWC: M<4!", _state);
    ae_assert(m%2==0, "Spline1DFitHermiteWC: M is odd!", _state);
    ae_assert(k>=0, "Spline1DFitHermiteWC: K<0!", _state);
    ae_assert(k<m, "Spline1DFitHermiteWC: K>=M!", _state);
    ae_assert(x->cnt>=n, "Spline1DFitHermiteWC: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Spline1DFitHermiteWC: Length(Y)<N!", _state);
    ae_assert(w->cnt>=n, "Spline1DFitHermiteWC: Length(W)<N!", _state);
    ae_assert(xc->cnt>=k, "Spline1DFitHermiteWC: Length(XC)<K!", _state);
    ae_assert(yc->cnt>=k, "Spline1DFitHermiteWC: Length(YC)<K!", _state);
    ae_assert(dc->cnt>=k, "Spline1DFitHermiteWC: Length(DC)<K!", _state);
    ae_assert(isfinitevector(x, n, _state), "Spline1DFitHermiteWC: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(y, n, _state), "Spline1DFitHermiteWC: Y contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(w, n, _state), "Spline1DFitHermiteWC: Y contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(xc, k, _state), "Spline1DFitHermiteWC: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(yc, k, _state), "Spline1DFitHermiteWC: Y contains infinite or NAN values!", _state);
    for(i=0; i<=k-1; i++)
    {
        ae_assert(dc->ptr.p_int[i]==0||dc->ptr.p_int[i]==1, "Spline1DFitHermiteWC: DC[i] is neither 0 or 1!", _state);
    }
    lsfit_spline1dfitinternal(1, x, y, w, n, xc, yc, dc, k, m, info, s, rep, _state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_spline1dfithermitewc(/* Real    */ ae_vector* x,
    /* Real    */ ae_vector* y,
    /* Real    */ ae_vector* w,
    ae_int_t n,
    /* Real    */ ae_vector* xc,
    /* Real    */ ae_vector* yc,
    /* Integer */ ae_vector* dc,
    ae_int_t k,
    ae_int_t m,
    ae_int_t* info,
    spline1dinterpolant* s,
    spline1dfitreport* rep, ae_state *_state)
{
    spline1dfithermitewc(x,y,w,n,xc,yc,dc,k,m,info,s,rep, _state);
}


/*************************************************************************
Least squares fitting by cubic spline.

This subroutine is "lightweight" alternative for more complex and feature-
rich Spline1DFitCubicWC().  See  Spline1DFitCubicWC() for more information
about subroutine parameters (we don't duplicate it here because of length)

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multithreading support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Speed-up provided by multithreading greatly depends  on  problem  size
  ! - only large problems (number of coefficients is more than 500) can be
  ! efficiently multithreaded.
  !
  ! Generally, commercial ALGLIB is several times faster than  open-source
  ! generic C edition, and many times faster than open-source C# edition.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

  -- ALGLIB PROJECT --
     Copyright 18.08.2009 by Bochkanov Sergey
*************************************************************************/
void spline1dfitcubic(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t m,
     ae_int_t* info,
     spline1dinterpolant* s,
     spline1dfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_vector w;
    ae_vector xc;
    ae_vector yc;
    ae_vector dc;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _spline1dinterpolant_clear(s);
    _spline1dfitreport_clear(rep);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&xc, 0, DT_REAL, _state);
    ae_vector_init(&yc, 0, DT_REAL, _state);
    ae_vector_init(&dc, 0, DT_INT, _state);

    ae_assert(n>=1, "Spline1DFitCubic: N<1!", _state);
    ae_assert(m>=4, "Spline1DFitCubic: M<4!", _state);
    ae_assert(x->cnt>=n, "Spline1DFitCubic: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Spline1DFitCubic: Length(Y)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "Spline1DFitCubic: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(y, n, _state), "Spline1DFitCubic: Y contains infinite or NAN values!", _state);
    ae_vector_set_length(&w, n, _state);
    for(i=0; i<=n-1; i++)
    {
        w.ptr.p_double[i] = (double)(1);
    }
    spline1dfitcubicwc(x, y, &w, n, &xc, &yc, &dc, 0, m, info, s, rep, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_spline1dfitcubic(/* Real    */ ae_vector* x,
    /* Real    */ ae_vector* y,
    ae_int_t n,
    ae_int_t m,
    ae_int_t* info,
    spline1dinterpolant* s,
    spline1dfitreport* rep, ae_state *_state)
{
    spline1dfitcubic(x,y,n,m,info,s,rep, _state);
}


/*************************************************************************
Least squares fitting by Hermite spline.

This subroutine is "lightweight" alternative for more complex and feature-
rich Spline1DFitHermiteWC().  See Spline1DFitHermiteWC()  description  for
more information about subroutine parameters (we don't duplicate  it  here
because of length).

COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multithreading support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Speed-up provided by multithreading greatly depends  on  problem  size
  ! - only large problems (number of coefficients is more than 500) can be
  ! efficiently multithreaded.
  !
  ! Generally, commercial ALGLIB is several times faster than  open-source
  ! generic C edition, and many times faster than open-source C# edition.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

  -- ALGLIB PROJECT --
     Copyright 18.08.2009 by Bochkanov Sergey
*************************************************************************/
void spline1dfithermite(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_int_t m,
     ae_int_t* info,
     spline1dinterpolant* s,
     spline1dfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_vector w;
    ae_vector xc;
    ae_vector yc;
    ae_vector dc;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _spline1dinterpolant_clear(s);
    _spline1dfitreport_clear(rep);
    ae_vector_init(&w, 0, DT_REAL, _state);
    ae_vector_init(&xc, 0, DT_REAL, _state);
    ae_vector_init(&yc, 0, DT_REAL, _state);
    ae_vector_init(&dc, 0, DT_INT, _state);

    ae_assert(n>=1, "Spline1DFitHermite: N<1!", _state);
    ae_assert(m>=4, "Spline1DFitHermite: M<4!", _state);
    ae_assert(m%2==0, "Spline1DFitHermite: M is odd!", _state);
    ae_assert(x->cnt>=n, "Spline1DFitHermite: Length(X)<N!", _state);
    ae_assert(y->cnt>=n, "Spline1DFitHermite: Length(Y)<N!", _state);
    ae_assert(isfinitevector(x, n, _state), "Spline1DFitHermite: X contains infinite or NAN values!", _state);
    ae_assert(isfinitevector(y, n, _state), "Spline1DFitHermite: Y contains infinite or NAN values!", _state);
    ae_vector_set_length(&w, n, _state);
    for(i=0; i<=n-1; i++)
    {
        w.ptr.p_double[i] = (double)(1);
    }
    spline1dfithermitewc(x, y, &w, n, &xc, &yc, &dc, 0, m, info, s, rep, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_spline1dfithermite(/* Real    */ ae_vector* x,
    /* Real    */ ae_vector* y,
    ae_int_t n,
    ae_int_t m,
    ae_int_t* info,
    spline1dinterpolant* s,
    spline1dfitreport* rep, ae_state *_state)
{
    spline1dfithermite(x,y,n,m,info,s,rep, _state);
}


/*************************************************************************
Weighted linear least squares fitting.

QR decomposition is used to reduce task to MxM, then triangular solver  or
SVD-based solver is used depending on condition number of the  system.  It
allows to maximize speed and retain decent accuracy.

IMPORTANT: if you want to perform  polynomial  fitting,  it  may  be  more
           convenient to use PolynomialFit() function. This function gives
           best  results  on  polynomial  problems  and  solves  numerical
           stability  issues  which  arise  when   you   fit   high-degree
           polynomials to your data.
           
COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multithreading support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Speed-up provided by multithreading greatly depends  on  problem  size
  ! - only large problems (number of coefficients is more than 500) can be
  ! efficiently multithreaded.
  !
  ! Generally, commercial ALGLIB is several times faster than  open-source
  ! generic C edition, and many times faster than open-source C# edition.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS:
    Y       -   array[0..N-1] Function values in  N  points.
    W       -   array[0..N-1]  Weights  corresponding to function  values.
                Each summand in square  sum  of  approximation  deviations
                from  given  values  is  multiplied  by  the   square   of
                corresponding weight.
    FMatrix -   a table of basis functions values, array[0..N-1, 0..M-1].
                FMatrix[I, J] - value of J-th basis function in I-th point.
    N       -   number of points used. N>=1.
    M       -   number of basis functions, M>=1.

OUTPUT PARAMETERS:
    Info    -   error code:
                * -4    internal SVD decomposition subroutine failed (very
                        rare and for degenerate systems only)
                * -1    incorrect N/M were specified
                *  1    task is solved
    C       -   decomposition coefficients, array[0..M-1]
    Rep     -   fitting report. Following fields are set:
                * Rep.TaskRCond     reciprocal of condition number
                * R2                non-adjusted coefficient of determination
                                    (non-weighted)
                * RMSError          rms error on the (X,Y).
                * AvgError          average error on the (X,Y).
                * AvgRelError       average relative error on the non-zero Y
                * MaxError          maximum error
                                    NON-WEIGHTED ERRORS ARE CALCULATED
                
ERRORS IN PARAMETERS                
                
This  solver  also  calculates different kinds of errors in parameters and
fills corresponding fields of report:
* Rep.CovPar        covariance matrix for parameters, array[K,K].
* Rep.ErrPar        errors in parameters, array[K],
                    errpar = sqrt(diag(CovPar))
* Rep.ErrCurve      vector of fit errors - standard deviations of empirical
                    best-fit curve from "ideal" best-fit curve built  with
                    infinite number of samples, array[N].
                    errcurve = sqrt(diag(F*CovPar*F')),
                    where F is functions matrix.
* Rep.Noise         vector of per-point estimates of noise, array[N]
            
NOTE:       noise in the data is estimated as follows:
            * for fitting without user-supplied  weights  all  points  are
              assumed to have same level of noise, which is estimated from
              the data
            * for fitting with user-supplied weights we assume that  noise
              level in I-th point is inversely proportional to Ith weight.
              Coefficient of proportionality is estimated from the data.
            
NOTE:       we apply small amount of regularization when we invert squared
            Jacobian and calculate covariance matrix. It  guarantees  that
            algorithm won't divide by zero  during  inversion,  but  skews
            error estimates a bit (fractional error is about 10^-9).
            
            However, we believe that this difference is insignificant  for
            all practical purposes except for the situation when you  want
            to compare ALGLIB results with "reference"  implementation  up
            to the last significant digit.
            
NOTE:       covariance matrix is estimated using  correction  for  degrees
            of freedom (covariances are divided by N-M instead of dividing
            by N).
                                    
  -- ALGLIB --
     Copyright 17.08.2009 by Bochkanov Sergey
*************************************************************************/
void lsfitlinearw(/* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* fmatrix,
     ae_int_t n,
     ae_int_t m,
     ae_int_t* info,
     /* Real    */ ae_vector* c,
     lsfitreport* rep,
     ae_state *_state)
{

    *info = 0;
    ae_vector_clear(c);
    _lsfitreport_clear(rep);

    ae_assert(n>=1, "LSFitLinearW: N<1!", _state);
    ae_assert(m>=1, "LSFitLinearW: M<1!", _state);
    ae_assert(y->cnt>=n, "LSFitLinearW: length(Y)<N!", _state);
    ae_assert(isfinitevector(y, n, _state), "LSFitLinearW: Y contains infinite or NaN values!", _state);
    ae_assert(w->cnt>=n, "LSFitLinearW: length(W)<N!", _state);
    ae_assert(isfinitevector(w, n, _state), "LSFitLinearW: W contains infinite or NaN values!", _state);
    ae_assert(fmatrix->rows>=n, "LSFitLinearW: rows(FMatrix)<N!", _state);
    ae_assert(fmatrix->cols>=m, "LSFitLinearW: cols(FMatrix)<M!", _state);
    ae_assert(apservisfinitematrix(fmatrix, n, m, _state), "LSFitLinearW: FMatrix contains infinite or NaN values!", _state);
    lsfit_lsfitlinearinternal(y, w, fmatrix, n, m, info, c, rep, _state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_lsfitlinearw(/* Real    */ ae_vector* y,
    /* Real    */ ae_vector* w,
    /* Real    */ ae_matrix* fmatrix,
    ae_int_t n,
    ae_int_t m,
    ae_int_t* info,
    /* Real    */ ae_vector* c,
    lsfitreport* rep, ae_state *_state)
{
    lsfitlinearw(y,w,fmatrix,n,m,info,c,rep, _state);
}


/*************************************************************************
Weighted constained linear least squares fitting.

This  is  variation  of LSFitLinearW(), which searchs for min|A*x=b| given
that  K  additional  constaints  C*x=bc are satisfied. It reduces original
task to modified one: min|B*y-d| WITHOUT constraints,  then LSFitLinearW()
is called.

IMPORTANT: if you want to perform  polynomial  fitting,  it  may  be  more
           convenient to use PolynomialFit() function. This function gives
           best  results  on  polynomial  problems  and  solves  numerical
           stability  issues  which  arise  when   you   fit   high-degree
           polynomials to your data.
           
COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multithreading support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Speed-up provided by multithreading greatly depends  on  problem  size
  ! - only large problems (number of coefficients is more than 500) can be
  ! efficiently multithreaded.
  !
  ! Generally, commercial ALGLIB is several times faster than  open-source
  ! generic C edition, and many times faster than open-source C# edition.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS:
    Y       -   array[0..N-1] Function values in  N  points.
    W       -   array[0..N-1]  Weights  corresponding to function  values.
                Each summand in square  sum  of  approximation  deviations
                from  given  values  is  multiplied  by  the   square   of
                corresponding weight.
    FMatrix -   a table of basis functions values, array[0..N-1, 0..M-1].
                FMatrix[I,J] - value of J-th basis function in I-th point.
    CMatrix -   a table of constaints, array[0..K-1,0..M].
                I-th row of CMatrix corresponds to I-th linear constraint:
                CMatrix[I,0]*C[0] + ... + CMatrix[I,M-1]*C[M-1] = CMatrix[I,M]
    N       -   number of points used. N>=1.
    M       -   number of basis functions, M>=1.
    K       -   number of constraints, 0 <= K < M
                K=0 corresponds to absence of constraints.

OUTPUT PARAMETERS:
    Info    -   error code:
                * -4    internal SVD decomposition subroutine failed (very
                        rare and for degenerate systems only)
                * -3    either   too   many  constraints  (M   or   more),
                        degenerate  constraints   (some   constraints  are
                        repetead twice) or inconsistent  constraints  were
                        specified.
                *  1    task is solved
    C       -   decomposition coefficients, array[0..M-1]
    Rep     -   fitting report. Following fields are set:
                * R2                non-adjusted coefficient of determination
                                    (non-weighted)
                * RMSError          rms error on the (X,Y).
                * AvgError          average error on the (X,Y).
                * AvgRelError       average relative error on the non-zero Y
                * MaxError          maximum error
                                    NON-WEIGHTED ERRORS ARE CALCULATED

IMPORTANT:
    this subroitine doesn't calculate task's condition number for K<>0.
                
ERRORS IN PARAMETERS                
                
This  solver  also  calculates different kinds of errors in parameters and
fills corresponding fields of report:
* Rep.CovPar        covariance matrix for parameters, array[K,K].
* Rep.ErrPar        errors in parameters, array[K],
                    errpar = sqrt(diag(CovPar))
* Rep.ErrCurve      vector of fit errors - standard deviations of empirical
                    best-fit curve from "ideal" best-fit curve built  with
                    infinite number of samples, array[N].
                    errcurve = sqrt(diag(F*CovPar*F')),
                    where F is functions matrix.
* Rep.Noise         vector of per-point estimates of noise, array[N]

IMPORTANT:  errors  in  parameters  are  calculated  without  taking  into
            account boundary/linear constraints! Presence  of  constraints
            changes distribution of errors, but there is no  easy  way  to
            account for constraints when you calculate covariance matrix.
            
NOTE:       noise in the data is estimated as follows:
            * for fitting without user-supplied  weights  all  points  are
              assumed to have same level of noise, which is estimated from
              the data
            * for fitting with user-supplied weights we assume that  noise
              level in I-th point is inversely proportional to Ith weight.
              Coefficient of proportionality is estimated from the data.
            
NOTE:       we apply small amount of regularization when we invert squared
            Jacobian and calculate covariance matrix. It  guarantees  that
            algorithm won't divide by zero  during  inversion,  but  skews
            error estimates a bit (fractional error is about 10^-9).
            
            However, we believe that this difference is insignificant  for
            all practical purposes except for the situation when you  want
            to compare ALGLIB results with "reference"  implementation  up
            to the last significant digit.
            
NOTE:       covariance matrix is estimated using  correction  for  degrees
            of freedom (covariances are divided by N-M instead of dividing
            by N).

  -- ALGLIB --
     Copyright 07.09.2009 by Bochkanov Sergey
*************************************************************************/
void lsfitlinearwc(/* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* fmatrix,
     /* Real    */ ae_matrix* cmatrix,
     ae_int_t n,
     ae_int_t m,
     ae_int_t k,
     ae_int_t* info,
     /* Real    */ ae_vector* c,
     lsfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _y;
    ae_matrix _cmatrix;
    ae_int_t i;
    ae_int_t j;
    ae_vector tau;
    ae_matrix q;
    ae_matrix f2;
    ae_vector tmp;
    ae_vector c0;
    double v;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    ae_matrix_init_copy(&_cmatrix, cmatrix, _state);
    cmatrix = &_cmatrix;
    *info = 0;
    ae_vector_clear(c);
    _lsfitreport_clear(rep);
    ae_vector_init(&tau, 0, DT_REAL, _state);
    ae_matrix_init(&q, 0, 0, DT_REAL, _state);
    ae_matrix_init(&f2, 0, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    ae_vector_init(&c0, 0, DT_REAL, _state);

    ae_assert(n>=1, "LSFitLinearWC: N<1!", _state);
    ae_assert(m>=1, "LSFitLinearWC: M<1!", _state);
    ae_assert(k>=0, "LSFitLinearWC: K<0!", _state);
    ae_assert(y->cnt>=n, "LSFitLinearWC: length(Y)<N!", _state);
    ae_assert(isfinitevector(y, n, _state), "LSFitLinearWC: Y contains infinite or NaN values!", _state);
    ae_assert(w->cnt>=n, "LSFitLinearWC: length(W)<N!", _state);
    ae_assert(isfinitevector(w, n, _state), "LSFitLinearWC: W contains infinite or NaN values!", _state);
    ae_assert(fmatrix->rows>=n, "LSFitLinearWC: rows(FMatrix)<N!", _state);
    ae_assert(fmatrix->cols>=m, "LSFitLinearWC: cols(FMatrix)<M!", _state);
    ae_assert(apservisfinitematrix(fmatrix, n, m, _state), "LSFitLinearWC: FMatrix contains infinite or NaN values!", _state);
    ae_assert(cmatrix->rows>=k, "LSFitLinearWC: rows(CMatrix)<K!", _state);
    ae_assert(cmatrix->cols>=m+1||k==0, "LSFitLinearWC: cols(CMatrix)<M+1!", _state);
    ae_assert(apservisfinitematrix(cmatrix, k, m+1, _state), "LSFitLinearWC: CMatrix contains infinite or NaN values!", _state);
    if( k>=m )
    {
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Solve
     */
    if( k==0 )
    {
        
        /*
         * no constraints
         */
        lsfit_lsfitlinearinternal(y, w, fmatrix, n, m, info, c, rep, _state);
    }
    else
    {
        
        /*
         * First, find general form solution of constraints system:
         * * factorize C = L*Q
         * * unpack Q
         * * fill upper part of C with zeros (for RCond)
         *
         * We got C=C0+Q2'*y where Q2 is lower M-K rows of Q.
         */
        rmatrixlq(cmatrix, k, m, &tau, _state);
        rmatrixlqunpackq(cmatrix, k, m, &tau, m, &q, _state);
        for(i=0; i<=k-1; i++)
        {
            for(j=i+1; j<=m-1; j++)
            {
                cmatrix->ptr.pp_double[i][j] = 0.0;
            }
        }
        if( ae_fp_less(rmatrixlurcondinf(cmatrix, k, _state),1000*ae_machineepsilon) )
        {
            *info = -3;
            ae_frame_leave(_state);
            return;
        }
        ae_vector_set_length(&tmp, k, _state);
        for(i=0; i<=k-1; i++)
        {
            if( i>0 )
            {
                v = ae_v_dotproduct(&cmatrix->ptr.pp_double[i][0], 1, &tmp.ptr.p_double[0], 1, ae_v_len(0,i-1));
            }
            else
            {
                v = (double)(0);
            }
            tmp.ptr.p_double[i] = (cmatrix->ptr.pp_double[i][m]-v)/cmatrix->ptr.pp_double[i][i];
        }
        ae_vector_set_length(&c0, m, _state);
        for(i=0; i<=m-1; i++)
        {
            c0.ptr.p_double[i] = (double)(0);
        }
        for(i=0; i<=k-1; i++)
        {
            v = tmp.ptr.p_double[i];
            ae_v_addd(&c0.ptr.p_double[0], 1, &q.ptr.pp_double[i][0], 1, ae_v_len(0,m-1), v);
        }
        
        /*
         * Second, prepare modified matrix F2 = F*Q2' and solve modified task
         */
        ae_vector_set_length(&tmp, ae_maxint(n, m, _state)+1, _state);
        ae_matrix_set_length(&f2, n, m-k, _state);
        matrixvectormultiply(fmatrix, 0, n-1, 0, m-1, ae_false, &c0, 0, m-1, -1.0, y, 0, n-1, 1.0, _state);
        rmatrixgemm(n, m-k, m, 1.0, fmatrix, 0, 0, 0, &q, k, 0, 1, 0.0, &f2, 0, 0, _state);
        lsfit_lsfitlinearinternal(y, w, &f2, n, m-k, info, &tmp, rep, _state);
        rep->taskrcond = (double)(-1);
        if( *info<=0 )
        {
            ae_frame_leave(_state);
            return;
        }
        
        /*
         * then, convert back to original answer: C = C0 + Q2'*Y0
         */
        ae_vector_set_length(c, m, _state);
        ae_v_move(&c->ptr.p_double[0], 1, &c0.ptr.p_double[0], 1, ae_v_len(0,m-1));
        matrixvectormultiply(&q, k, m-1, 0, m-1, ae_true, &tmp, 0, m-k-1, 1.0, c, 0, m-1, 1.0, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_lsfitlinearwc(/* Real    */ ae_vector* y,
    /* Real    */ ae_vector* w,
    /* Real    */ ae_matrix* fmatrix,
    /* Real    */ ae_matrix* cmatrix,
    ae_int_t n,
    ae_int_t m,
    ae_int_t k,
    ae_int_t* info,
    /* Real    */ ae_vector* c,
    lsfitreport* rep, ae_state *_state)
{
    lsfitlinearwc(y,w,fmatrix,cmatrix,n,m,k,info,c,rep, _state);
}


/*************************************************************************
Linear least squares fitting.

QR decomposition is used to reduce task to MxM, then triangular solver  or
SVD-based solver is used depending on condition number of the  system.  It
allows to maximize speed and retain decent accuracy.

IMPORTANT: if you want to perform  polynomial  fitting,  it  may  be  more
           convenient to use PolynomialFit() function. This function gives
           best  results  on  polynomial  problems  and  solves  numerical
           stability  issues  which  arise  when   you   fit   high-degree
           polynomials to your data.
           
COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multithreading support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Speed-up provided by multithreading greatly depends  on  problem  size
  ! - only large problems (number of coefficients is more than 500) can be
  ! efficiently multithreaded.
  !
  ! Generally, commercial ALGLIB is several times faster than  open-source
  ! generic C edition, and many times faster than open-source C# edition.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS:
    Y       -   array[0..N-1] Function values in  N  points.
    FMatrix -   a table of basis functions values, array[0..N-1, 0..M-1].
                FMatrix[I, J] - value of J-th basis function in I-th point.
    N       -   number of points used. N>=1.
    M       -   number of basis functions, M>=1.

OUTPUT PARAMETERS:
    Info    -   error code:
                * -4    internal SVD decomposition subroutine failed (very
                        rare and for degenerate systems only)
                *  1    task is solved
    C       -   decomposition coefficients, array[0..M-1]
    Rep     -   fitting report. Following fields are set:
                * Rep.TaskRCond     reciprocal of condition number
                * R2                non-adjusted coefficient of determination
                                    (non-weighted)
                * RMSError          rms error on the (X,Y).
                * AvgError          average error on the (X,Y).
                * AvgRelError       average relative error on the non-zero Y
                * MaxError          maximum error
                                    NON-WEIGHTED ERRORS ARE CALCULATED
                
ERRORS IN PARAMETERS                
                
This  solver  also  calculates different kinds of errors in parameters and
fills corresponding fields of report:
* Rep.CovPar        covariance matrix for parameters, array[K,K].
* Rep.ErrPar        errors in parameters, array[K],
                    errpar = sqrt(diag(CovPar))
* Rep.ErrCurve      vector of fit errors - standard deviations of empirical
                    best-fit curve from "ideal" best-fit curve built  with
                    infinite number of samples, array[N].
                    errcurve = sqrt(diag(F*CovPar*F')),
                    where F is functions matrix.
* Rep.Noise         vector of per-point estimates of noise, array[N]
            
NOTE:       noise in the data is estimated as follows:
            * for fitting without user-supplied  weights  all  points  are
              assumed to have same level of noise, which is estimated from
              the data
            * for fitting with user-supplied weights we assume that  noise
              level in I-th point is inversely proportional to Ith weight.
              Coefficient of proportionality is estimated from the data.
            
NOTE:       we apply small amount of regularization when we invert squared
            Jacobian and calculate covariance matrix. It  guarantees  that
            algorithm won't divide by zero  during  inversion,  but  skews
            error estimates a bit (fractional error is about 10^-9).
            
            However, we believe that this difference is insignificant  for
            all practical purposes except for the situation when you  want
            to compare ALGLIB results with "reference"  implementation  up
            to the last significant digit.
            
NOTE:       covariance matrix is estimated using  correction  for  degrees
            of freedom (covariances are divided by N-M instead of dividing
            by N).

  -- ALGLIB --
     Copyright 17.08.2009 by Bochkanov Sergey
*************************************************************************/
void lsfitlinear(/* Real    */ ae_vector* y,
     /* Real    */ ae_matrix* fmatrix,
     ae_int_t n,
     ae_int_t m,
     ae_int_t* info,
     /* Real    */ ae_vector* c,
     lsfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector w;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    ae_vector_clear(c);
    _lsfitreport_clear(rep);
    ae_vector_init(&w, 0, DT_REAL, _state);

    ae_assert(n>=1, "LSFitLinear: N<1!", _state);
    ae_assert(m>=1, "LSFitLinear: M<1!", _state);
    ae_assert(y->cnt>=n, "LSFitLinear: length(Y)<N!", _state);
    ae_assert(isfinitevector(y, n, _state), "LSFitLinear: Y contains infinite or NaN values!", _state);
    ae_assert(fmatrix->rows>=n, "LSFitLinear: rows(FMatrix)<N!", _state);
    ae_assert(fmatrix->cols>=m, "LSFitLinear: cols(FMatrix)<M!", _state);
    ae_assert(apservisfinitematrix(fmatrix, n, m, _state), "LSFitLinear: FMatrix contains infinite or NaN values!", _state);
    ae_vector_set_length(&w, n, _state);
    for(i=0; i<=n-1; i++)
    {
        w.ptr.p_double[i] = (double)(1);
    }
    lsfit_lsfitlinearinternal(y, &w, fmatrix, n, m, info, c, rep, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_lsfitlinear(/* Real    */ ae_vector* y,
    /* Real    */ ae_matrix* fmatrix,
    ae_int_t n,
    ae_int_t m,
    ae_int_t* info,
    /* Real    */ ae_vector* c,
    lsfitreport* rep, ae_state *_state)
{
    lsfitlinear(y,fmatrix,n,m,info,c,rep, _state);
}


/*************************************************************************
Constained linear least squares fitting.

This  is  variation  of LSFitLinear(),  which searchs for min|A*x=b| given
that  K  additional  constaints  C*x=bc are satisfied. It reduces original
task to modified one: min|B*y-d| WITHOUT constraints,  then  LSFitLinear()
is called.

IMPORTANT: if you want to perform  polynomial  fitting,  it  may  be  more
           convenient to use PolynomialFit() function. This function gives
           best  results  on  polynomial  problems  and  solves  numerical
           stability  issues  which  arise  when   you   fit   high-degree
           polynomials to your data.
           
COMMERCIAL EDITION OF ALGLIB:

  ! Commercial version of ALGLIB includes two  important  improvements  of
  ! this function, which can be used from C++ and C#:
  ! * Intel MKL support (lightweight Intel MKL is shipped with ALGLIB)
  ! * multithreading support
  !
  ! Intel MKL gives approximately constant  (with  respect  to  number  of
  ! worker threads) acceleration factor which depends on CPU  being  used,
  ! problem  size  and  "baseline"  ALGLIB  edition  which  is  used   for
  ! comparison.
  !
  ! Speed-up provided by multithreading greatly depends  on  problem  size
  ! - only large problems (number of coefficients is more than 500) can be
  ! efficiently multithreaded.
  !
  ! Generally, commercial ALGLIB is several times faster than  open-source
  ! generic C edition, and many times faster than open-source C# edition.
  !
  ! We recommend you to read 'Working with commercial version' section  of
  ! ALGLIB Reference Manual in order to find out how to  use  performance-
  ! related features provided by commercial edition of ALGLIB.

INPUT PARAMETERS:
    Y       -   array[0..N-1] Function values in  N  points.
    FMatrix -   a table of basis functions values, array[0..N-1, 0..M-1].
                FMatrix[I,J] - value of J-th basis function in I-th point.
    CMatrix -   a table of constaints, array[0..K-1,0..M].
                I-th row of CMatrix corresponds to I-th linear constraint:
                CMatrix[I,0]*C[0] + ... + CMatrix[I,M-1]*C[M-1] = CMatrix[I,M]
    N       -   number of points used. N>=1.
    M       -   number of basis functions, M>=1.
    K       -   number of constraints, 0 <= K < M
                K=0 corresponds to absence of constraints.

OUTPUT PARAMETERS:
    Info    -   error code:
                * -4    internal SVD decomposition subroutine failed (very
                        rare and for degenerate systems only)
                * -3    either   too   many  constraints  (M   or   more),
                        degenerate  constraints   (some   constraints  are
                        repetead twice) or inconsistent  constraints  were
                        specified.
                *  1    task is solved
    C       -   decomposition coefficients, array[0..M-1]
    Rep     -   fitting report. Following fields are set:
                * R2                non-adjusted coefficient of determination
                                    (non-weighted)
                * RMSError          rms error on the (X,Y).
                * AvgError          average error on the (X,Y).
                * AvgRelError       average relative error on the non-zero Y
                * MaxError          maximum error
                                    NON-WEIGHTED ERRORS ARE CALCULATED

IMPORTANT:
    this subroitine doesn't calculate task's condition number for K<>0.
                
ERRORS IN PARAMETERS                
                
This  solver  also  calculates different kinds of errors in parameters and
fills corresponding fields of report:
* Rep.CovPar        covariance matrix for parameters, array[K,K].
* Rep.ErrPar        errors in parameters, array[K],
                    errpar = sqrt(diag(CovPar))
* Rep.ErrCurve      vector of fit errors - standard deviations of empirical
                    best-fit curve from "ideal" best-fit curve built  with
                    infinite number of samples, array[N].
                    errcurve = sqrt(diag(F*CovPar*F')),
                    where F is functions matrix.
* Rep.Noise         vector of per-point estimates of noise, array[N]

IMPORTANT:  errors  in  parameters  are  calculated  without  taking  into
            account boundary/linear constraints! Presence  of  constraints
            changes distribution of errors, but there is no  easy  way  to
            account for constraints when you calculate covariance matrix.
            
NOTE:       noise in the data is estimated as follows:
            * for fitting without user-supplied  weights  all  points  are
              assumed to have same level of noise, which is estimated from
              the data
            * for fitting with user-supplied weights we assume that  noise
              level in I-th point is inversely proportional to Ith weight.
              Coefficient of proportionality is estimated from the data.
            
NOTE:       we apply small amount of regularization when we invert squared
            Jacobian and calculate covariance matrix. It  guarantees  that
            algorithm won't divide by zero  during  inversion,  but  skews
            error estimates a bit (fractional error is about 10^-9).
            
            However, we believe that this difference is insignificant  for
            all practical purposes except for the situation when you  want
            to compare ALGLIB results with "reference"  implementation  up
            to the last significant digit.
            
NOTE:       covariance matrix is estimated using  correction  for  degrees
            of freedom (covariances are divided by N-M instead of dividing
            by N).

  -- ALGLIB --
     Copyright 07.09.2009 by Bochkanov Sergey
*************************************************************************/
void lsfitlinearc(/* Real    */ ae_vector* y,
     /* Real    */ ae_matrix* fmatrix,
     /* Real    */ ae_matrix* cmatrix,
     ae_int_t n,
     ae_int_t m,
     ae_int_t k,
     ae_int_t* info,
     /* Real    */ ae_vector* c,
     lsfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _y;
    ae_vector w;
    ae_int_t i;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    *info = 0;
    ae_vector_clear(c);
    _lsfitreport_clear(rep);
    ae_vector_init(&w, 0, DT_REAL, _state);

    ae_assert(n>=1, "LSFitLinearC: N<1!", _state);
    ae_assert(m>=1, "LSFitLinearC: M<1!", _state);
    ae_assert(k>=0, "LSFitLinearC: K<0!", _state);
    ae_assert(y->cnt>=n, "LSFitLinearC: length(Y)<N!", _state);
    ae_assert(isfinitevector(y, n, _state), "LSFitLinearC: Y contains infinite or NaN values!", _state);
    ae_assert(fmatrix->rows>=n, "LSFitLinearC: rows(FMatrix)<N!", _state);
    ae_assert(fmatrix->cols>=m, "LSFitLinearC: cols(FMatrix)<M!", _state);
    ae_assert(apservisfinitematrix(fmatrix, n, m, _state), "LSFitLinearC: FMatrix contains infinite or NaN values!", _state);
    ae_assert(cmatrix->rows>=k, "LSFitLinearC: rows(CMatrix)<K!", _state);
    ae_assert(cmatrix->cols>=m+1||k==0, "LSFitLinearC: cols(CMatrix)<M+1!", _state);
    ae_assert(apservisfinitematrix(cmatrix, k, m+1, _state), "LSFitLinearC: CMatrix contains infinite or NaN values!", _state);
    ae_vector_set_length(&w, n, _state);
    for(i=0; i<=n-1; i++)
    {
        w.ptr.p_double[i] = (double)(1);
    }
    lsfitlinearwc(y, &w, fmatrix, cmatrix, n, m, k, info, c, rep, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
void _pexec_lsfitlinearc(/* Real    */ ae_vector* y,
    /* Real    */ ae_matrix* fmatrix,
    /* Real    */ ae_matrix* cmatrix,
    ae_int_t n,
    ae_int_t m,
    ae_int_t k,
    ae_int_t* info,
    /* Real    */ ae_vector* c,
    lsfitreport* rep, ae_state *_state)
{
    lsfitlinearc(y,fmatrix,cmatrix,n,m,k,info,c,rep, _state);
}


/*************************************************************************
Weighted nonlinear least squares fitting using function values only.

Combination of numerical differentiation and secant updates is used to
obtain function Jacobian.

Nonlinear task min(F(c)) is solved, where

    F(c) = (w[0]*(f(c,x[0])-y[0]))^2 + ... + (w[n-1]*(f(c,x[n-1])-y[n-1]))^2,

    * N is a number of points,
    * M is a dimension of a space points belong to,
    * K is a dimension of a space of parameters being fitted,
    * w is an N-dimensional vector of weight coefficients,
    * x is a set of N points, each of them is an M-dimensional vector,
    * c is a K-dimensional vector of parameters being fitted

This subroutine uses only f(c,x[i]).

INPUT PARAMETERS:
    X       -   array[0..N-1,0..M-1], points (one row = one point)
    Y       -   array[0..N-1], function values.
    W       -   weights, array[0..N-1]
    C       -   array[0..K-1], initial approximation to the solution,
    N       -   number of points, N>1
    M       -   dimension of space
    K       -   number of parameters being fitted
    DiffStep-   numerical differentiation step;
                should not be very small or large;
                large = loss of accuracy
                small = growth of round-off errors

OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state

  -- ALGLIB --
     Copyright 18.10.2008 by Bochkanov Sergey
*************************************************************************/
void lsfitcreatewf(/* Real    */ ae_matrix* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_vector* c,
     ae_int_t n,
     ae_int_t m,
     ae_int_t k,
     double diffstep,
     lsfitstate* state,
     ae_state *_state)
{
    ae_int_t i;

    _lsfitstate_clear(state);

    ae_assert(n>=1, "LSFitCreateWF: N<1!", _state);
    ae_assert(m>=1, "LSFitCreateWF: M<1!", _state);
    ae_assert(k>=1, "LSFitCreateWF: K<1!", _state);
    ae_assert(c->cnt>=k, "LSFitCreateWF: length(C)<K!", _state);
    ae_assert(isfinitevector(c, k, _state), "LSFitCreateWF: C contains infinite or NaN values!", _state);
    ae_assert(y->cnt>=n, "LSFitCreateWF: length(Y)<N!", _state);
    ae_assert(isfinitevector(y, n, _state), "LSFitCreateWF: Y contains infinite or NaN values!", _state);
    ae_assert(w->cnt>=n, "LSFitCreateWF: length(W)<N!", _state);
    ae_assert(isfinitevector(w, n, _state), "LSFitCreateWF: W contains infinite or NaN values!", _state);
    ae_assert(x->rows>=n, "LSFitCreateWF: rows(X)<N!", _state);
    ae_assert(x->cols>=m, "LSFitCreateWF: cols(X)<M!", _state);
    ae_assert(apservisfinitematrix(x, n, m, _state), "LSFitCreateWF: X contains infinite or NaN values!", _state);
    ae_assert(ae_isfinite(diffstep, _state), "LSFitCreateWF: DiffStep is not finite!", _state);
    ae_assert(ae_fp_greater(diffstep,(double)(0)), "LSFitCreateWF: DiffStep<=0!", _state);
    state->teststep = (double)(0);
    state->diffstep = diffstep;
    state->npoints = n;
    state->nweights = n;
    state->wkind = 1;
    state->m = m;
    state->k = k;
    lsfitsetcond(state, 0.0, 0.0, 0, _state);
    lsfitsetstpmax(state, 0.0, _state);
    lsfitsetxrep(state, ae_false, _state);
    ae_matrix_set_length(&state->taskx, n, m, _state);
    ae_vector_set_length(&state->tasky, n, _state);
    ae_vector_set_length(&state->taskw, n, _state);
    ae_vector_set_length(&state->c, k, _state);
    ae_vector_set_length(&state->x, m, _state);
    ae_v_move(&state->c.ptr.p_double[0], 1, &c->ptr.p_double[0], 1, ae_v_len(0,k-1));
    ae_v_move(&state->taskw.ptr.p_double[0], 1, &w->ptr.p_double[0], 1, ae_v_len(0,n-1));
    for(i=0; i<=n-1; i++)
    {
        ae_v_move(&state->taskx.ptr.pp_double[i][0], 1, &x->ptr.pp_double[i][0], 1, ae_v_len(0,m-1));
        state->tasky.ptr.p_double[i] = y->ptr.p_double[i];
    }
    ae_vector_set_length(&state->s, k, _state);
    ae_vector_set_length(&state->bndl, k, _state);
    ae_vector_set_length(&state->bndu, k, _state);
    for(i=0; i<=k-1; i++)
    {
        state->s.ptr.p_double[i] = 1.0;
        state->bndl.ptr.p_double[i] = _state->v_neginf;
        state->bndu.ptr.p_double[i] = _state->v_posinf;
    }
    state->optalgo = 0;
    state->prevnpt = -1;
    state->prevalgo = -1;
    minlmcreatev(k, n, &state->c, diffstep, &state->optstate, _state);
    lsfit_lsfitclearrequestfields(state, _state);
    ae_vector_set_length(&state->rstate.ia, 6+1, _state);
    ae_vector_set_length(&state->rstate.ra, 8+1, _state);
    state->rstate.stage = -1;
}


/*************************************************************************
Nonlinear least squares fitting using function values only.

Combination of numerical differentiation and secant updates is used to
obtain function Jacobian.

Nonlinear task min(F(c)) is solved, where

    F(c) = (f(c,x[0])-y[0])^2 + ... + (f(c,x[n-1])-y[n-1])^2,

    * N is a number of points,
    * M is a dimension of a space points belong to,
    * K is a dimension of a space of parameters being fitted,
    * w is an N-dimensional vector of weight coefficients,
    * x is a set of N points, each of them is an M-dimensional vector,
    * c is a K-dimensional vector of parameters being fitted

This subroutine uses only f(c,x[i]).

INPUT PARAMETERS:
    X       -   array[0..N-1,0..M-1], points (one row = one point)
    Y       -   array[0..N-1], function values.
    C       -   array[0..K-1], initial approximation to the solution,
    N       -   number of points, N>1
    M       -   dimension of space
    K       -   number of parameters being fitted
    DiffStep-   numerical differentiation step;
                should not be very small or large;
                large = loss of accuracy
                small = growth of round-off errors

OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state

  -- ALGLIB --
     Copyright 18.10.2008 by Bochkanov Sergey
*************************************************************************/
void lsfitcreatef(/* Real    */ ae_matrix* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* c,
     ae_int_t n,
     ae_int_t m,
     ae_int_t k,
     double diffstep,
     lsfitstate* state,
     ae_state *_state)
{
    ae_int_t i;

    _lsfitstate_clear(state);

    ae_assert(n>=1, "LSFitCreateF: N<1!", _state);
    ae_assert(m>=1, "LSFitCreateF: M<1!", _state);
    ae_assert(k>=1, "LSFitCreateF: K<1!", _state);
    ae_assert(c->cnt>=k, "LSFitCreateF: length(C)<K!", _state);
    ae_assert(isfinitevector(c, k, _state), "LSFitCreateF: C contains infinite or NaN values!", _state);
    ae_assert(y->cnt>=n, "LSFitCreateF: length(Y)<N!", _state);
    ae_assert(isfinitevector(y, n, _state), "LSFitCreateF: Y contains infinite or NaN values!", _state);
    ae_assert(x->rows>=n, "LSFitCreateF: rows(X)<N!", _state);
    ae_assert(x->cols>=m, "LSFitCreateF: cols(X)<M!", _state);
    ae_assert(apservisfinitematrix(x, n, m, _state), "LSFitCreateF: X contains infinite or NaN values!", _state);
    ae_assert(x->rows>=n, "LSFitCreateF: rows(X)<N!", _state);
    ae_assert(x->cols>=m, "LSFitCreateF: cols(X)<M!", _state);
    ae_assert(apservisfinitematrix(x, n, m, _state), "LSFitCreateF: X contains infinite or NaN values!", _state);
    ae_assert(ae_isfinite(diffstep, _state), "LSFitCreateF: DiffStep is not finite!", _state);
    ae_assert(ae_fp_greater(diffstep,(double)(0)), "LSFitCreateF: DiffStep<=0!", _state);
    state->teststep = (double)(0);
    state->diffstep = diffstep;
    state->npoints = n;
    state->wkind = 0;
    state->m = m;
    state->k = k;
    lsfitsetcond(state, 0.0, 0.0, 0, _state);
    lsfitsetstpmax(state, 0.0, _state);
    lsfitsetxrep(state, ae_false, _state);
    ae_matrix_set_length(&state->taskx, n, m, _state);
    ae_vector_set_length(&state->tasky, n, _state);
    ae_vector_set_length(&state->c, k, _state);
    ae_vector_set_length(&state->x, m, _state);
    ae_v_move(&state->c.ptr.p_double[0], 1, &c->ptr.p_double[0], 1, ae_v_len(0,k-1));
    for(i=0; i<=n-1; i++)
    {
        ae_v_move(&state->taskx.ptr.pp_double[i][0], 1, &x->ptr.pp_double[i][0], 1, ae_v_len(0,m-1));
        state->tasky.ptr.p_double[i] = y->ptr.p_double[i];
    }
    ae_vector_set_length(&state->s, k, _state);
    ae_vector_set_length(&state->bndl, k, _state);
    ae_vector_set_length(&state->bndu, k, _state);
    for(i=0; i<=k-1; i++)
    {
        state->s.ptr.p_double[i] = 1.0;
        state->bndl.ptr.p_double[i] = _state->v_neginf;
        state->bndu.ptr.p_double[i] = _state->v_posinf;
    }
    state->optalgo = 0;
    state->prevnpt = -1;
    state->prevalgo = -1;
    minlmcreatev(k, n, &state->c, diffstep, &state->optstate, _state);
    lsfit_lsfitclearrequestfields(state, _state);
    ae_vector_set_length(&state->rstate.ia, 6+1, _state);
    ae_vector_set_length(&state->rstate.ra, 8+1, _state);
    state->rstate.stage = -1;
}


/*************************************************************************
Weighted nonlinear least squares fitting using gradient only.

Nonlinear task min(F(c)) is solved, where

    F(c) = (w[0]*(f(c,x[0])-y[0]))^2 + ... + (w[n-1]*(f(c,x[n-1])-y[n-1]))^2,
    
    * N is a number of points,
    * M is a dimension of a space points belong to,
    * K is a dimension of a space of parameters being fitted,
    * w is an N-dimensional vector of weight coefficients,
    * x is a set of N points, each of them is an M-dimensional vector,
    * c is a K-dimensional vector of parameters being fitted
    
This subroutine uses only f(c,x[i]) and its gradient.
    
INPUT PARAMETERS:
    X       -   array[0..N-1,0..M-1], points (one row = one point)
    Y       -   array[0..N-1], function values.
    W       -   weights, array[0..N-1]
    C       -   array[0..K-1], initial approximation to the solution,
    N       -   number of points, N>1
    M       -   dimension of space
    K       -   number of parameters being fitted
    CheapFG -   boolean flag, which is:
                * True  if both function and gradient calculation complexity
                        are less than O(M^2).  An improved  algorithm  can
                        be  used  which corresponds  to  FGJ  scheme  from
                        MINLM unit.
                * False otherwise.
                        Standard Jacibian-bases  Levenberg-Marquardt  algo
                        will be used (FJ scheme).

OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state

See also:
    LSFitResults
    LSFitCreateFG (fitting without weights)
    LSFitCreateWFGH (fitting using Hessian)
    LSFitCreateFGH (fitting using Hessian, without weights)

  -- ALGLIB --
     Copyright 17.08.2009 by Bochkanov Sergey
*************************************************************************/
void lsfitcreatewfg(/* Real    */ ae_matrix* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_vector* c,
     ae_int_t n,
     ae_int_t m,
     ae_int_t k,
     ae_bool cheapfg,
     lsfitstate* state,
     ae_state *_state)
{
    ae_int_t i;

    _lsfitstate_clear(state);

    ae_assert(n>=1, "LSFitCreateWFG: N<1!", _state);
    ae_assert(m>=1, "LSFitCreateWFG: M<1!", _state);
    ae_assert(k>=1, "LSFitCreateWFG: K<1!", _state);
    ae_assert(c->cnt>=k, "LSFitCreateWFG: length(C)<K!", _state);
    ae_assert(isfinitevector(c, k, _state), "LSFitCreateWFG: C contains infinite or NaN values!", _state);
    ae_assert(y->cnt>=n, "LSFitCreateWFG: length(Y)<N!", _state);
    ae_assert(isfinitevector(y, n, _state), "LSFitCreateWFG: Y contains infinite or NaN values!", _state);
    ae_assert(w->cnt>=n, "LSFitCreateWFG: length(W)<N!", _state);
    ae_assert(isfinitevector(w, n, _state), "LSFitCreateWFG: W contains infinite or NaN values!", _state);
    ae_assert(x->rows>=n, "LSFitCreateWFG: rows(X)<N!", _state);
    ae_assert(x->cols>=m, "LSFitCreateWFG: cols(X)<M!", _state);
    ae_assert(apservisfinitematrix(x, n, m, _state), "LSFitCreateWFG: X contains infinite or NaN values!", _state);
    state->teststep = (double)(0);
    state->diffstep = (double)(0);
    state->npoints = n;
    state->nweights = n;
    state->wkind = 1;
    state->m = m;
    state->k = k;
    lsfitsetcond(state, 0.0, 0.0, 0, _state);
    lsfitsetstpmax(state, 0.0, _state);
    lsfitsetxrep(state, ae_false, _state);
    ae_matrix_set_length(&state->taskx, n, m, _state);
    ae_vector_set_length(&state->tasky, n, _state);
    ae_vector_set_length(&state->taskw, n, _state);
    ae_vector_set_length(&state->c, k, _state);
    ae_vector_set_length(&state->x, m, _state);
    ae_vector_set_length(&state->g, k, _state);
    ae_v_move(&state->c.ptr.p_double[0], 1, &c->ptr.p_double[0], 1, ae_v_len(0,k-1));
    ae_v_move(&state->taskw.ptr.p_double[0], 1, &w->ptr.p_double[0], 1, ae_v_len(0,n-1));
    for(i=0; i<=n-1; i++)
    {
        ae_v_move(&state->taskx.ptr.pp_double[i][0], 1, &x->ptr.pp_double[i][0], 1, ae_v_len(0,m-1));
        state->tasky.ptr.p_double[i] = y->ptr.p_double[i];
    }
    ae_vector_set_length(&state->s, k, _state);
    ae_vector_set_length(&state->bndl, k, _state);
    ae_vector_set_length(&state->bndu, k, _state);
    for(i=0; i<=k-1; i++)
    {
        state->s.ptr.p_double[i] = 1.0;
        state->bndl.ptr.p_double[i] = _state->v_neginf;
        state->bndu.ptr.p_double[i] = _state->v_posinf;
    }
    state->optalgo = 1;
    state->prevnpt = -1;
    state->prevalgo = -1;
    if( cheapfg )
    {
        minlmcreatevgj(k, n, &state->c, &state->optstate, _state);
    }
    else
    {
        minlmcreatevj(k, n, &state->c, &state->optstate, _state);
    }
    lsfit_lsfitclearrequestfields(state, _state);
    ae_vector_set_length(&state->rstate.ia, 6+1, _state);
    ae_vector_set_length(&state->rstate.ra, 8+1, _state);
    state->rstate.stage = -1;
}


/*************************************************************************
Nonlinear least squares fitting using gradient only, without individual
weights.

Nonlinear task min(F(c)) is solved, where

    F(c) = ((f(c,x[0])-y[0]))^2 + ... + ((f(c,x[n-1])-y[n-1]))^2,

    * N is a number of points,
    * M is a dimension of a space points belong to,
    * K is a dimension of a space of parameters being fitted,
    * x is a set of N points, each of them is an M-dimensional vector,
    * c is a K-dimensional vector of parameters being fitted

This subroutine uses only f(c,x[i]) and its gradient.

INPUT PARAMETERS:
    X       -   array[0..N-1,0..M-1], points (one row = one point)
    Y       -   array[0..N-1], function values.
    C       -   array[0..K-1], initial approximation to the solution,
    N       -   number of points, N>1
    M       -   dimension of space
    K       -   number of parameters being fitted
    CheapFG -   boolean flag, which is:
                * True  if both function and gradient calculation complexity
                        are less than O(M^2).  An improved  algorithm  can
                        be  used  which corresponds  to  FGJ  scheme  from
                        MINLM unit.
                * False otherwise.
                        Standard Jacibian-bases  Levenberg-Marquardt  algo
                        will be used (FJ scheme).

OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state

  -- ALGLIB --
     Copyright 17.08.2009 by Bochkanov Sergey
*************************************************************************/
void lsfitcreatefg(/* Real    */ ae_matrix* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* c,
     ae_int_t n,
     ae_int_t m,
     ae_int_t k,
     ae_bool cheapfg,
     lsfitstate* state,
     ae_state *_state)
{
    ae_int_t i;

    _lsfitstate_clear(state);

    ae_assert(n>=1, "LSFitCreateFG: N<1!", _state);
    ae_assert(m>=1, "LSFitCreateFG: M<1!", _state);
    ae_assert(k>=1, "LSFitCreateFG: K<1!", _state);
    ae_assert(c->cnt>=k, "LSFitCreateFG: length(C)<K!", _state);
    ae_assert(isfinitevector(c, k, _state), "LSFitCreateFG: C contains infinite or NaN values!", _state);
    ae_assert(y->cnt>=n, "LSFitCreateFG: length(Y)<N!", _state);
    ae_assert(isfinitevector(y, n, _state), "LSFitCreateFG: Y contains infinite or NaN values!", _state);
    ae_assert(x->rows>=n, "LSFitCreateFG: rows(X)<N!", _state);
    ae_assert(x->cols>=m, "LSFitCreateFG: cols(X)<M!", _state);
    ae_assert(apservisfinitematrix(x, n, m, _state), "LSFitCreateFG: X contains infinite or NaN values!", _state);
    ae_assert(x->rows>=n, "LSFitCreateFG: rows(X)<N!", _state);
    ae_assert(x->cols>=m, "LSFitCreateFG: cols(X)<M!", _state);
    ae_assert(apservisfinitematrix(x, n, m, _state), "LSFitCreateFG: X contains infinite or NaN values!", _state);
    state->teststep = (double)(0);
    state->diffstep = (double)(0);
    state->npoints = n;
    state->wkind = 0;
    state->m = m;
    state->k = k;
    lsfitsetcond(state, 0.0, 0.0, 0, _state);
    lsfitsetstpmax(state, 0.0, _state);
    lsfitsetxrep(state, ae_false, _state);
    ae_matrix_set_length(&state->taskx, n, m, _state);
    ae_vector_set_length(&state->tasky, n, _state);
    ae_vector_set_length(&state->c, k, _state);
    ae_vector_set_length(&state->x, m, _state);
    ae_vector_set_length(&state->g, k, _state);
    ae_v_move(&state->c.ptr.p_double[0], 1, &c->ptr.p_double[0], 1, ae_v_len(0,k-1));
    for(i=0; i<=n-1; i++)
    {
        ae_v_move(&state->taskx.ptr.pp_double[i][0], 1, &x->ptr.pp_double[i][0], 1, ae_v_len(0,m-1));
        state->tasky.ptr.p_double[i] = y->ptr.p_double[i];
    }
    ae_vector_set_length(&state->s, k, _state);
    ae_vector_set_length(&state->bndl, k, _state);
    ae_vector_set_length(&state->bndu, k, _state);
    for(i=0; i<=k-1; i++)
    {
        state->s.ptr.p_double[i] = 1.0;
        state->bndl.ptr.p_double[i] = _state->v_neginf;
        state->bndu.ptr.p_double[i] = _state->v_posinf;
    }
    state->optalgo = 1;
    state->prevnpt = -1;
    state->prevalgo = -1;
    if( cheapfg )
    {
        minlmcreatevgj(k, n, &state->c, &state->optstate, _state);
    }
    else
    {
        minlmcreatevj(k, n, &state->c, &state->optstate, _state);
    }
    lsfit_lsfitclearrequestfields(state, _state);
    ae_vector_set_length(&state->rstate.ia, 6+1, _state);
    ae_vector_set_length(&state->rstate.ra, 8+1, _state);
    state->rstate.stage = -1;
}


/*************************************************************************
Weighted nonlinear least squares fitting using gradient/Hessian.

Nonlinear task min(F(c)) is solved, where

    F(c) = (w[0]*(f(c,x[0])-y[0]))^2 + ... + (w[n-1]*(f(c,x[n-1])-y[n-1]))^2,

    * N is a number of points,
    * M is a dimension of a space points belong to,
    * K is a dimension of a space of parameters being fitted,
    * w is an N-dimensional vector of weight coefficients,
    * x is a set of N points, each of them is an M-dimensional vector,
    * c is a K-dimensional vector of parameters being fitted

This subroutine uses f(c,x[i]), its gradient and its Hessian.

INPUT PARAMETERS:
    X       -   array[0..N-1,0..M-1], points (one row = one point)
    Y       -   array[0..N-1], function values.
    W       -   weights, array[0..N-1]
    C       -   array[0..K-1], initial approximation to the solution,
    N       -   number of points, N>1
    M       -   dimension of space
    K       -   number of parameters being fitted

OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state

  -- ALGLIB --
     Copyright 17.08.2009 by Bochkanov Sergey
*************************************************************************/
void lsfitcreatewfgh(/* Real    */ ae_matrix* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_vector* c,
     ae_int_t n,
     ae_int_t m,
     ae_int_t k,
     lsfitstate* state,
     ae_state *_state)
{
    ae_int_t i;

    _lsfitstate_clear(state);

    ae_assert(n>=1, "LSFitCreateWFGH: N<1!", _state);
    ae_assert(m>=1, "LSFitCreateWFGH: M<1!", _state);
    ae_assert(k>=1, "LSFitCreateWFGH: K<1!", _state);
    ae_assert(c->cnt>=k, "LSFitCreateWFGH: length(C)<K!", _state);
    ae_assert(isfinitevector(c, k, _state), "LSFitCreateWFGH: C contains infinite or NaN values!", _state);
    ae_assert(y->cnt>=n, "LSFitCreateWFGH: length(Y)<N!", _state);
    ae_assert(isfinitevector(y, n, _state), "LSFitCreateWFGH: Y contains infinite or NaN values!", _state);
    ae_assert(w->cnt>=n, "LSFitCreateWFGH: length(W)<N!", _state);
    ae_assert(isfinitevector(w, n, _state), "LSFitCreateWFGH: W contains infinite or NaN values!", _state);
    ae_assert(x->rows>=n, "LSFitCreateWFGH: rows(X)<N!", _state);
    ae_assert(x->cols>=m, "LSFitCreateWFGH: cols(X)<M!", _state);
    ae_assert(apservisfinitematrix(x, n, m, _state), "LSFitCreateWFGH: X contains infinite or NaN values!", _state);
    state->teststep = (double)(0);
    state->diffstep = (double)(0);
    state->npoints = n;
    state->nweights = n;
    state->wkind = 1;
    state->m = m;
    state->k = k;
    lsfitsetcond(state, 0.0, 0.0, 0, _state);
    lsfitsetstpmax(state, 0.0, _state);
    lsfitsetxrep(state, ae_false, _state);
    ae_matrix_set_length(&state->taskx, n, m, _state);
    ae_vector_set_length(&state->tasky, n, _state);
    ae_vector_set_length(&state->taskw, n, _state);
    ae_vector_set_length(&state->c, k, _state);
    ae_matrix_set_length(&state->h, k, k, _state);
    ae_vector_set_length(&state->x, m, _state);
    ae_vector_set_length(&state->g, k, _state);
    ae_v_move(&state->c.ptr.p_double[0], 1, &c->ptr.p_double[0], 1, ae_v_len(0,k-1));
    ae_v_move(&state->taskw.ptr.p_double[0], 1, &w->ptr.p_double[0], 1, ae_v_len(0,n-1));
    for(i=0; i<=n-1; i++)
    {
        ae_v_move(&state->taskx.ptr.pp_double[i][0], 1, &x->ptr.pp_double[i][0], 1, ae_v_len(0,m-1));
        state->tasky.ptr.p_double[i] = y->ptr.p_double[i];
    }
    ae_vector_set_length(&state->s, k, _state);
    ae_vector_set_length(&state->bndl, k, _state);
    ae_vector_set_length(&state->bndu, k, _state);
    for(i=0; i<=k-1; i++)
    {
        state->s.ptr.p_double[i] = 1.0;
        state->bndl.ptr.p_double[i] = _state->v_neginf;
        state->bndu.ptr.p_double[i] = _state->v_posinf;
    }
    state->optalgo = 2;
    state->prevnpt = -1;
    state->prevalgo = -1;
    minlmcreatefgh(k, &state->c, &state->optstate, _state);
    lsfit_lsfitclearrequestfields(state, _state);
    ae_vector_set_length(&state->rstate.ia, 6+1, _state);
    ae_vector_set_length(&state->rstate.ra, 8+1, _state);
    state->rstate.stage = -1;
}


/*************************************************************************
Nonlinear least squares fitting using gradient/Hessian, without individial
weights.

Nonlinear task min(F(c)) is solved, where

    F(c) = ((f(c,x[0])-y[0]))^2 + ... + ((f(c,x[n-1])-y[n-1]))^2,

    * N is a number of points,
    * M is a dimension of a space points belong to,
    * K is a dimension of a space of parameters being fitted,
    * x is a set of N points, each of them is an M-dimensional vector,
    * c is a K-dimensional vector of parameters being fitted

This subroutine uses f(c,x[i]), its gradient and its Hessian.

INPUT PARAMETERS:
    X       -   array[0..N-1,0..M-1], points (one row = one point)
    Y       -   array[0..N-1], function values.
    C       -   array[0..K-1], initial approximation to the solution,
    N       -   number of points, N>1
    M       -   dimension of space
    K       -   number of parameters being fitted

OUTPUT PARAMETERS:
    State   -   structure which stores algorithm state


  -- ALGLIB --
     Copyright 17.08.2009 by Bochkanov Sergey
*************************************************************************/
void lsfitcreatefgh(/* Real    */ ae_matrix* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* c,
     ae_int_t n,
     ae_int_t m,
     ae_int_t k,
     lsfitstate* state,
     ae_state *_state)
{
    ae_int_t i;

    _lsfitstate_clear(state);

    ae_assert(n>=1, "LSFitCreateFGH: N<1!", _state);
    ae_assert(m>=1, "LSFitCreateFGH: M<1!", _state);
    ae_assert(k>=1, "LSFitCreateFGH: K<1!", _state);
    ae_assert(c->cnt>=k, "LSFitCreateFGH: length(C)<K!", _state);
    ae_assert(isfinitevector(c, k, _state), "LSFitCreateFGH: C contains infinite or NaN values!", _state);
    ae_assert(y->cnt>=n, "LSFitCreateFGH: length(Y)<N!", _state);
    ae_assert(isfinitevector(y, n, _state), "LSFitCreateFGH: Y contains infinite or NaN values!", _state);
    ae_assert(x->rows>=n, "LSFitCreateFGH: rows(X)<N!", _state);
    ae_assert(x->cols>=m, "LSFitCreateFGH: cols(X)<M!", _state);
    ae_assert(apservisfinitematrix(x, n, m, _state), "LSFitCreateFGH: X contains infinite or NaN values!", _state);
    state->teststep = (double)(0);
    state->diffstep = (double)(0);
    state->npoints = n;
    state->wkind = 0;
    state->m = m;
    state->k = k;
    lsfitsetcond(state, 0.0, 0.0, 0, _state);
    lsfitsetstpmax(state, 0.0, _state);
    lsfitsetxrep(state, ae_false, _state);
    ae_matrix_set_length(&state->taskx, n, m, _state);
    ae_vector_set_length(&state->tasky, n, _state);
    ae_vector_set_length(&state->c, k, _state);
    ae_matrix_set_length(&state->h, k, k, _state);
    ae_vector_set_length(&state->x, m, _state);
    ae_vector_set_length(&state->g, k, _state);
    ae_v_move(&state->c.ptr.p_double[0], 1, &c->ptr.p_double[0], 1, ae_v_len(0,k-1));
    for(i=0; i<=n-1; i++)
    {
        ae_v_move(&state->taskx.ptr.pp_double[i][0], 1, &x->ptr.pp_double[i][0], 1, ae_v_len(0,m-1));
        state->tasky.ptr.p_double[i] = y->ptr.p_double[i];
    }
    ae_vector_set_length(&state->s, k, _state);
    ae_vector_set_length(&state->bndl, k, _state);
    ae_vector_set_length(&state->bndu, k, _state);
    for(i=0; i<=k-1; i++)
    {
        state->s.ptr.p_double[i] = 1.0;
        state->bndl.ptr.p_double[i] = _state->v_neginf;
        state->bndu.ptr.p_double[i] = _state->v_posinf;
    }
    state->optalgo = 2;
    state->prevnpt = -1;
    state->prevalgo = -1;
    minlmcreatefgh(k, &state->c, &state->optstate, _state);
    lsfit_lsfitclearrequestfields(state, _state);
    ae_vector_set_length(&state->rstate.ia, 6+1, _state);
    ae_vector_set_length(&state->rstate.ra, 8+1, _state);
    state->rstate.stage = -1;
}


/*************************************************************************
Stopping conditions for nonlinear least squares fitting.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    EpsF    -   stopping criterion. Algorithm stops if
                |F(k+1)-F(k)| <= EpsF*max{|F(k)|, |F(k+1)|, 1}
    EpsX    -   >=0
                The subroutine finishes its work if  on  k+1-th  iteration
                the condition |v|<=EpsX is fulfilled, where:
                * |.| means Euclidian norm
                * v - scaled step vector, v[i]=dx[i]/s[i]
                * dx - ste pvector, dx=X(k+1)-X(k)
                * s - scaling coefficients set by LSFitSetScale()
    MaxIts  -   maximum number of iterations. If MaxIts=0, the  number  of
                iterations   is    unlimited.   Only   Levenberg-Marquardt
                iterations  are  counted  (L-BFGS/CG  iterations  are  NOT
                counted because their cost is very low compared to that of
                LM).

NOTE

Passing EpsF=0, EpsX=0 and MaxIts=0 (simultaneously) will lead to automatic
stopping criterion selection (according to the scheme used by MINLM unit).


  -- ALGLIB --
     Copyright 17.08.2009 by Bochkanov Sergey
*************************************************************************/
void lsfitsetcond(lsfitstate* state,
     double epsf,
     double epsx,
     ae_int_t maxits,
     ae_state *_state)
{


    ae_assert(ae_isfinite(epsf, _state), "LSFitSetCond: EpsF is not finite!", _state);
    ae_assert(ae_fp_greater_eq(epsf,(double)(0)), "LSFitSetCond: negative EpsF!", _state);
    ae_assert(ae_isfinite(epsx, _state), "LSFitSetCond: EpsX is not finite!", _state);
    ae_assert(ae_fp_greater_eq(epsx,(double)(0)), "LSFitSetCond: negative EpsX!", _state);
    ae_assert(maxits>=0, "LSFitSetCond: negative MaxIts!", _state);
    state->epsf = epsf;
    state->epsx = epsx;
    state->maxits = maxits;
}


/*************************************************************************
This function sets maximum step length

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    StpMax  -   maximum step length, >=0. Set StpMax to 0.0,  if you don't
                want to limit step length.

Use this subroutine when you optimize target function which contains exp()
or  other  fast  growing  functions,  and optimization algorithm makes too
large  steps  which  leads  to overflow. This function allows us to reject
steps  that  are  too  large  (and  therefore  expose  us  to the possible
overflow) without actually calculating function value at the x+stp*d.

NOTE: non-zero StpMax leads to moderate  performance  degradation  because
intermediate  step  of  preconditioned L-BFGS optimization is incompatible
with limits on step size.

  -- ALGLIB --
     Copyright 02.04.2010 by Bochkanov Sergey
*************************************************************************/
void lsfitsetstpmax(lsfitstate* state, double stpmax, ae_state *_state)
{


    ae_assert(ae_fp_greater_eq(stpmax,(double)(0)), "LSFitSetStpMax: StpMax<0!", _state);
    state->stpmax = stpmax;
}


/*************************************************************************
This function turns on/off reporting.

INPUT PARAMETERS:
    State   -   structure which stores algorithm state
    NeedXRep-   whether iteration reports are needed or not
    
When reports are needed, State.C (current parameters) and State.F (current
value of fitting function) are reported.


  -- ALGLIB --
     Copyright 15.08.2010 by Bochkanov Sergey
*************************************************************************/
void lsfitsetxrep(lsfitstate* state, ae_bool needxrep, ae_state *_state)
{


    state->xrep = needxrep;
}


/*************************************************************************
This function sets scaling coefficients for underlying optimizer.

ALGLIB optimizers use scaling matrices to test stopping  conditions  (step
size and gradient are scaled before comparison with tolerances).  Scale of
the I-th variable is a translation invariant measure of:
a) "how large" the variable is
b) how large the step should be to make significant changes in the function

Generally, scale is NOT considered to be a form of preconditioner.  But LM
optimizer is unique in that it uses scaling matrix both  in  the  stopping
condition tests and as Marquardt damping factor.

Proper scaling is very important for the algorithm performance. It is less
important for the quality of results, but still has some influence (it  is
easier  to  converge  when  variables  are  properly  scaled, so premature
stopping is possible when very badly scalled variables are  combined  with
relaxed stopping conditions).

INPUT PARAMETERS:
    State   -   structure stores algorithm state
    S       -   array[N], non-zero scaling coefficients
                S[i] may be negative, sign doesn't matter.

  -- ALGLIB --
     Copyright 14.01.2011 by Bochkanov Sergey
*************************************************************************/
void lsfitsetscale(lsfitstate* state,
     /* Real    */ ae_vector* s,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(s->cnt>=state->k, "LSFitSetScale: Length(S)<K", _state);
    for(i=0; i<=state->k-1; i++)
    {
        ae_assert(ae_isfinite(s->ptr.p_double[i], _state), "LSFitSetScale: S contains infinite or NAN elements", _state);
        ae_assert(ae_fp_neq(s->ptr.p_double[i],(double)(0)), "LSFitSetScale: S contains infinite or NAN elements", _state);
        state->s.ptr.p_double[i] = ae_fabs(s->ptr.p_double[i], _state);
    }
}


/*************************************************************************
This function sets boundary constraints for underlying optimizer

Boundary constraints are inactive by default (after initial creation).
They are preserved until explicitly turned off with another SetBC() call.

INPUT PARAMETERS:
    State   -   structure stores algorithm state
    BndL    -   lower bounds, array[K].
                If some (all) variables are unbounded, you may specify
                very small number or -INF (latter is recommended because
                it will allow solver to use better algorithm).
    BndU    -   upper bounds, array[K].
                If some (all) variables are unbounded, you may specify
                very large number or +INF (latter is recommended because
                it will allow solver to use better algorithm).

NOTE 1: it is possible to specify BndL[i]=BndU[i]. In this case I-th
variable will be "frozen" at X[i]=BndL[i]=BndU[i].

NOTE 2: unlike other constrained optimization algorithms, this solver  has
following useful properties:
* bound constraints are always satisfied exactly
* function is evaluated only INSIDE area specified by bound constraints

  -- ALGLIB --
     Copyright 14.01.2011 by Bochkanov Sergey
*************************************************************************/
void lsfitsetbc(lsfitstate* state,
     /* Real    */ ae_vector* bndl,
     /* Real    */ ae_vector* bndu,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;


    k = state->k;
    ae_assert(bndl->cnt>=k, "LSFitSetBC: Length(BndL)<K", _state);
    ae_assert(bndu->cnt>=k, "LSFitSetBC: Length(BndU)<K", _state);
    for(i=0; i<=k-1; i++)
    {
        ae_assert(ae_isfinite(bndl->ptr.p_double[i], _state)||ae_isneginf(bndl->ptr.p_double[i], _state), "LSFitSetBC: BndL contains NAN or +INF", _state);
        ae_assert(ae_isfinite(bndu->ptr.p_double[i], _state)||ae_isposinf(bndu->ptr.p_double[i], _state), "LSFitSetBC: BndU contains NAN or -INF", _state);
        if( ae_isfinite(bndl->ptr.p_double[i], _state)&&ae_isfinite(bndu->ptr.p_double[i], _state) )
        {
            ae_assert(ae_fp_less_eq(bndl->ptr.p_double[i],bndu->ptr.p_double[i]), "LSFitSetBC: BndL[i]>BndU[i]", _state);
        }
        state->bndl.ptr.p_double[i] = bndl->ptr.p_double[i];
        state->bndu.ptr.p_double[i] = bndu->ptr.p_double[i];
    }
}


/*************************************************************************
NOTES:

1. this algorithm is somewhat unusual because it works with  parameterized
   function f(C,X), where X is a function argument (we  have  many  points
   which are characterized by different  argument  values),  and  C  is  a
   parameter to fit.

   For example, if we want to do linear fit by f(c0,c1,x) = c0*x+c1,  then
   x will be argument, and {c0,c1} will be parameters.
   
   It is important to understand that this algorithm finds minimum in  the
   space of function PARAMETERS (not arguments), so it  needs  derivatives
   of f() with respect to C, not X.
   
   In the example above it will need f=c0*x+c1 and {df/dc0,df/dc1} = {x,1}
   instead of {df/dx} = {c0}.

2. Callback functions accept C as the first parameter, and X as the second

3. If  state  was  created  with  LSFitCreateFG(),  algorithm  needs  just
   function   and   its   gradient,   but   if   state   was  created with
   LSFitCreateFGH(), algorithm will need function, gradient and Hessian.
   
   According  to  the  said  above,  there  ase  several  versions of this
   function, which accept different sets of callbacks.
   
   This flexibility opens way to subtle errors - you may create state with
   LSFitCreateFGH() (optimization using Hessian), but call function  which
   does not accept Hessian. So when algorithm will request Hessian,  there
   will be no callback to call. In this case exception will be thrown.
   
   Be careful to avoid such errors because there is no way to find them at
   compile time - you can see them at runtime only.

  -- ALGLIB --
     Copyright 17.08.2009 by Bochkanov Sergey
*************************************************************************/
ae_bool lsfititeration(lsfitstate* state, ae_state *_state)
{
    double lx;
    double lf;
    double ld;
    double rx;
    double rf;
    double rd;
    ae_int_t n;
    ae_int_t m;
    ae_int_t k;
    double v;
    double vv;
    double relcnt;
    ae_int_t i;
    ae_int_t j;
    ae_int_t j1;
    ae_int_t info;
    ae_bool result;


    
    /*
     * Reverse communication preparations
     * I know it looks ugly, but it works the same way
     * anywhere from C++ to Python.
     *
     * This code initializes locals by:
     * * random values determined during code
     *   generation - on first subroutine call
     * * values from previous call - on subsequent calls
     */
    if( state->rstate.stage>=0 )
    {
        n = state->rstate.ia.ptr.p_int[0];
        m = state->rstate.ia.ptr.p_int[1];
        k = state->rstate.ia.ptr.p_int[2];
        i = state->rstate.ia.ptr.p_int[3];
        j = state->rstate.ia.ptr.p_int[4];
        j1 = state->rstate.ia.ptr.p_int[5];
        info = state->rstate.ia.ptr.p_int[6];
        lx = state->rstate.ra.ptr.p_double[0];
        lf = state->rstate.ra.ptr.p_double[1];
        ld = state->rstate.ra.ptr.p_double[2];
        rx = state->rstate.ra.ptr.p_double[3];
        rf = state->rstate.ra.ptr.p_double[4];
        rd = state->rstate.ra.ptr.p_double[5];
        v = state->rstate.ra.ptr.p_double[6];
        vv = state->rstate.ra.ptr.p_double[7];
        relcnt = state->rstate.ra.ptr.p_double[8];
    }
    else
    {
        n = -983;
        m = -989;
        k = -834;
        i = 900;
        j = -287;
        j1 = 364;
        info = 214;
        lx = -338;
        lf = -686;
        ld = 912;
        rx = 585;
        rf = 497;
        rd = -271;
        v = -581;
        vv = 745;
        relcnt = -533;
    }
    if( state->rstate.stage==0 )
    {
        goto lbl_0;
    }
    if( state->rstate.stage==1 )
    {
        goto lbl_1;
    }
    if( state->rstate.stage==2 )
    {
        goto lbl_2;
    }
    if( state->rstate.stage==3 )
    {
        goto lbl_3;
    }
    if( state->rstate.stage==4 )
    {
        goto lbl_4;
    }
    if( state->rstate.stage==5 )
    {
        goto lbl_5;
    }
    if( state->rstate.stage==6 )
    {
        goto lbl_6;
    }
    if( state->rstate.stage==7 )
    {
        goto lbl_7;
    }
    if( state->rstate.stage==8 )
    {
        goto lbl_8;
    }
    if( state->rstate.stage==9 )
    {
        goto lbl_9;
    }
    if( state->rstate.stage==10 )
    {
        goto lbl_10;
    }
    if( state->rstate.stage==11 )
    {
        goto lbl_11;
    }
    if( state->rstate.stage==12 )
    {
        goto lbl_12;
    }
    if( state->rstate.stage==13 )
    {
        goto lbl_13;
    }
    
    /*
     * Routine body
     */
    
    /*
     * Init
     */
    if( state->wkind==1 )
    {
        ae_assert(state->npoints==state->nweights, "LSFitFit: number of points is not equal to the number of weights", _state);
    }
    state->repvaridx = -1;
    n = state->npoints;
    m = state->m;
    k = state->k;
    minlmsetcond(&state->optstate, 0.0, state->epsf, state->epsx, state->maxits, _state);
    minlmsetstpmax(&state->optstate, state->stpmax, _state);
    minlmsetxrep(&state->optstate, state->xrep, _state);
    minlmsetscale(&state->optstate, &state->s, _state);
    minlmsetbc(&state->optstate, &state->bndl, &state->bndu, _state);
    
    /*
     *  Check that user-supplied gradient is correct
     */
    lsfit_lsfitclearrequestfields(state, _state);
    if( !(ae_fp_greater(state->teststep,(double)(0))&&state->optalgo==1) )
    {
        goto lbl_14;
    }
    for(i=0; i<=k-1; i++)
    {
        if( ae_isfinite(state->bndl.ptr.p_double[i], _state) )
        {
            state->c.ptr.p_double[i] = ae_maxreal(state->c.ptr.p_double[i], state->bndl.ptr.p_double[i], _state);
        }
        if( ae_isfinite(state->bndu.ptr.p_double[i], _state) )
        {
            state->c.ptr.p_double[i] = ae_minreal(state->c.ptr.p_double[i], state->bndu.ptr.p_double[i], _state);
        }
    }
    state->needfg = ae_true;
    i = 0;
lbl_16:
    if( i>k-1 )
    {
        goto lbl_18;
    }
    ae_assert(ae_fp_less_eq(state->bndl.ptr.p_double[i],state->c.ptr.p_double[i])&&ae_fp_less_eq(state->c.ptr.p_double[i],state->bndu.ptr.p_double[i]), "LSFitIteration: internal error(State.C is out of bounds)", _state);
    v = state->c.ptr.p_double[i];
    j = 0;
lbl_19:
    if( j>n-1 )
    {
        goto lbl_21;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->taskx.ptr.pp_double[j][0], 1, ae_v_len(0,m-1));
    state->c.ptr.p_double[i] = v-state->teststep*state->s.ptr.p_double[i];
    if( ae_isfinite(state->bndl.ptr.p_double[i], _state) )
    {
        state->c.ptr.p_double[i] = ae_maxreal(state->c.ptr.p_double[i], state->bndl.ptr.p_double[i], _state);
    }
    lx = state->c.ptr.p_double[i];
    state->rstate.stage = 0;
    goto lbl_rcomm;
lbl_0:
    lf = state->f;
    ld = state->g.ptr.p_double[i];
    state->c.ptr.p_double[i] = v+state->teststep*state->s.ptr.p_double[i];
    if( ae_isfinite(state->bndu.ptr.p_double[i], _state) )
    {
        state->c.ptr.p_double[i] = ae_minreal(state->c.ptr.p_double[i], state->bndu.ptr.p_double[i], _state);
    }
    rx = state->c.ptr.p_double[i];
    state->rstate.stage = 1;
    goto lbl_rcomm;
lbl_1:
    rf = state->f;
    rd = state->g.ptr.p_double[i];
    state->c.ptr.p_double[i] = (lx+rx)/2;
    if( ae_isfinite(state->bndl.ptr.p_double[i], _state) )
    {
        state->c.ptr.p_double[i] = ae_maxreal(state->c.ptr.p_double[i], state->bndl.ptr.p_double[i], _state);
    }
    if( ae_isfinite(state->bndu.ptr.p_double[i], _state) )
    {
        state->c.ptr.p_double[i] = ae_minreal(state->c.ptr.p_double[i], state->bndu.ptr.p_double[i], _state);
    }
    state->rstate.stage = 2;
    goto lbl_rcomm;
lbl_2:
    state->c.ptr.p_double[i] = v;
    if( !derivativecheck(lf, ld, rf, rd, state->f, state->g.ptr.p_double[i], rx-lx, _state) )
    {
        state->repvaridx = i;
        state->repterminationtype = -7;
        result = ae_false;
        return result;
    }
    j = j+1;
    goto lbl_19;
lbl_21:
    i = i+1;
    goto lbl_16;
lbl_18:
    state->needfg = ae_false;
lbl_14:
    
    /*
     * Fill WCur by weights:
     * * for WKind=0 unit weights are chosen
     * * for WKind=1 we use user-supplied weights stored in State.TaskW
     */
    rvectorsetlengthatleast(&state->wcur, n, _state);
    for(i=0; i<=n-1; i++)
    {
        state->wcur.ptr.p_double[i] = 1.0;
        if( state->wkind==1 )
        {
            state->wcur.ptr.p_double[i] = state->taskw.ptr.p_double[i];
        }
    }
    
    /*
     * Optimize
     */
lbl_22:
    if( !minlmiteration(&state->optstate, _state) )
    {
        goto lbl_23;
    }
    if( !state->optstate.needfi )
    {
        goto lbl_24;
    }
    
    /*
     * calculate f[] = wi*(f(xi,c)-yi)
     */
    i = 0;
lbl_26:
    if( i>n-1 )
    {
        goto lbl_28;
    }
    ae_v_move(&state->c.ptr.p_double[0], 1, &state->optstate.x.ptr.p_double[0], 1, ae_v_len(0,k-1));
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->taskx.ptr.pp_double[i][0], 1, ae_v_len(0,m-1));
    state->pointindex = i;
    lsfit_lsfitclearrequestfields(state, _state);
    state->needf = ae_true;
    state->rstate.stage = 3;
    goto lbl_rcomm;
lbl_3:
    state->needf = ae_false;
    vv = state->wcur.ptr.p_double[i];
    state->optstate.fi.ptr.p_double[i] = vv*(state->f-state->tasky.ptr.p_double[i]);
    i = i+1;
    goto lbl_26;
lbl_28:
    goto lbl_22;
lbl_24:
    if( !state->optstate.needf )
    {
        goto lbl_29;
    }
    
    /*
     * calculate F = sum (wi*(f(xi,c)-yi))^2
     */
    state->optstate.f = (double)(0);
    i = 0;
lbl_31:
    if( i>n-1 )
    {
        goto lbl_33;
    }
    ae_v_move(&state->c.ptr.p_double[0], 1, &state->optstate.x.ptr.p_double[0], 1, ae_v_len(0,k-1));
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->taskx.ptr.pp_double[i][0], 1, ae_v_len(0,m-1));
    state->pointindex = i;
    lsfit_lsfitclearrequestfields(state, _state);
    state->needf = ae_true;
    state->rstate.stage = 4;
    goto lbl_rcomm;
lbl_4:
    state->needf = ae_false;
    vv = state->wcur.ptr.p_double[i];
    state->optstate.f = state->optstate.f+ae_sqr(vv*(state->f-state->tasky.ptr.p_double[i]), _state);
    i = i+1;
    goto lbl_31;
lbl_33:
    goto lbl_22;
lbl_29:
    if( !state->optstate.needfg )
    {
        goto lbl_34;
    }
    
    /*
     * calculate F/gradF
     */
    state->optstate.f = (double)(0);
    for(i=0; i<=k-1; i++)
    {
        state->optstate.g.ptr.p_double[i] = (double)(0);
    }
    i = 0;
lbl_36:
    if( i>n-1 )
    {
        goto lbl_38;
    }
    ae_v_move(&state->c.ptr.p_double[0], 1, &state->optstate.x.ptr.p_double[0], 1, ae_v_len(0,k-1));
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->taskx.ptr.pp_double[i][0], 1, ae_v_len(0,m-1));
    state->pointindex = i;
    lsfit_lsfitclearrequestfields(state, _state);
    state->needfg = ae_true;
    state->rstate.stage = 5;
    goto lbl_rcomm;
lbl_5:
    state->needfg = ae_false;
    vv = state->wcur.ptr.p_double[i];
    state->optstate.f = state->optstate.f+ae_sqr(vv*(state->f-state->tasky.ptr.p_double[i]), _state);
    v = ae_sqr(vv, _state)*2*(state->f-state->tasky.ptr.p_double[i]);
    ae_v_addd(&state->optstate.g.ptr.p_double[0], 1, &state->g.ptr.p_double[0], 1, ae_v_len(0,k-1), v);
    i = i+1;
    goto lbl_36;
lbl_38:
    goto lbl_22;
lbl_34:
    if( !state->optstate.needfij )
    {
        goto lbl_39;
    }
    
    /*
     * calculate Fi/jac(Fi)
     */
    i = 0;
lbl_41:
    if( i>n-1 )
    {
        goto lbl_43;
    }
    ae_v_move(&state->c.ptr.p_double[0], 1, &state->optstate.x.ptr.p_double[0], 1, ae_v_len(0,k-1));
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->taskx.ptr.pp_double[i][0], 1, ae_v_len(0,m-1));
    state->pointindex = i;
    lsfit_lsfitclearrequestfields(state, _state);
    state->needfg = ae_true;
    state->rstate.stage = 6;
    goto lbl_rcomm;
lbl_6:
    state->needfg = ae_false;
    vv = state->wcur.ptr.p_double[i];
    state->optstate.fi.ptr.p_double[i] = vv*(state->f-state->tasky.ptr.p_double[i]);
    ae_v_moved(&state->optstate.j.ptr.pp_double[i][0], 1, &state->g.ptr.p_double[0], 1, ae_v_len(0,k-1), vv);
    i = i+1;
    goto lbl_41;
lbl_43:
    goto lbl_22;
lbl_39:
    if( !state->optstate.needfgh )
    {
        goto lbl_44;
    }
    
    /*
     * calculate F/grad(F)/hess(F)
     */
    state->optstate.f = (double)(0);
    for(i=0; i<=k-1; i++)
    {
        state->optstate.g.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=k-1; i++)
    {
        for(j=0; j<=k-1; j++)
        {
            state->optstate.h.ptr.pp_double[i][j] = (double)(0);
        }
    }
    i = 0;
lbl_46:
    if( i>n-1 )
    {
        goto lbl_48;
    }
    ae_v_move(&state->c.ptr.p_double[0], 1, &state->optstate.x.ptr.p_double[0], 1, ae_v_len(0,k-1));
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->taskx.ptr.pp_double[i][0], 1, ae_v_len(0,m-1));
    state->pointindex = i;
    lsfit_lsfitclearrequestfields(state, _state);
    state->needfgh = ae_true;
    state->rstate.stage = 7;
    goto lbl_rcomm;
lbl_7:
    state->needfgh = ae_false;
    vv = state->wcur.ptr.p_double[i];
    state->optstate.f = state->optstate.f+ae_sqr(vv*(state->f-state->tasky.ptr.p_double[i]), _state);
    v = ae_sqr(vv, _state)*2*(state->f-state->tasky.ptr.p_double[i]);
    ae_v_addd(&state->optstate.g.ptr.p_double[0], 1, &state->g.ptr.p_double[0], 1, ae_v_len(0,k-1), v);
    for(j=0; j<=k-1; j++)
    {
        v = 2*ae_sqr(vv, _state)*state->g.ptr.p_double[j];
        ae_v_addd(&state->optstate.h.ptr.pp_double[j][0], 1, &state->g.ptr.p_double[0], 1, ae_v_len(0,k-1), v);
        v = 2*ae_sqr(vv, _state)*(state->f-state->tasky.ptr.p_double[i]);
        ae_v_addd(&state->optstate.h.ptr.pp_double[j][0], 1, &state->h.ptr.pp_double[j][0], 1, ae_v_len(0,k-1), v);
    }
    i = i+1;
    goto lbl_46;
lbl_48:
    goto lbl_22;
lbl_44:
    if( !state->optstate.xupdated )
    {
        goto lbl_49;
    }
    
    /*
     * Report new iteration
     */
    ae_v_move(&state->c.ptr.p_double[0], 1, &state->optstate.x.ptr.p_double[0], 1, ae_v_len(0,k-1));
    state->f = state->optstate.f;
    lsfit_lsfitclearrequestfields(state, _state);
    state->xupdated = ae_true;
    state->rstate.stage = 8;
    goto lbl_rcomm;
lbl_8:
    state->xupdated = ae_false;
    goto lbl_22;
lbl_49:
    goto lbl_22;
lbl_23:
    minlmresults(&state->optstate, &state->c, &state->optrep, _state);
    state->repterminationtype = state->optrep.terminationtype;
    state->repiterationscount = state->optrep.iterationscount;
    
    /*
     * calculate errors
     */
    if( state->repterminationtype<=0 )
    {
        goto lbl_51;
    }
    
    /*
     * Calculate RMS/Avg/Max/... errors
     */
    state->reprmserror = (double)(0);
    state->repwrmserror = (double)(0);
    state->repavgerror = (double)(0);
    state->repavgrelerror = (double)(0);
    state->repmaxerror = (double)(0);
    relcnt = (double)(0);
    i = 0;
lbl_53:
    if( i>n-1 )
    {
        goto lbl_55;
    }
    ae_v_move(&state->c.ptr.p_double[0], 1, &state->c.ptr.p_double[0], 1, ae_v_len(0,k-1));
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->taskx.ptr.pp_double[i][0], 1, ae_v_len(0,m-1));
    state->pointindex = i;
    lsfit_lsfitclearrequestfields(state, _state);
    state->needf = ae_true;
    state->rstate.stage = 9;
    goto lbl_rcomm;
lbl_9:
    state->needf = ae_false;
    v = state->f;
    vv = state->wcur.ptr.p_double[i];
    state->reprmserror = state->reprmserror+ae_sqr(v-state->tasky.ptr.p_double[i], _state);
    state->repwrmserror = state->repwrmserror+ae_sqr(vv*(v-state->tasky.ptr.p_double[i]), _state);
    state->repavgerror = state->repavgerror+ae_fabs(v-state->tasky.ptr.p_double[i], _state);
    if( ae_fp_neq(state->tasky.ptr.p_double[i],(double)(0)) )
    {
        state->repavgrelerror = state->repavgrelerror+ae_fabs(v-state->tasky.ptr.p_double[i], _state)/ae_fabs(state->tasky.ptr.p_double[i], _state);
        relcnt = relcnt+1;
    }
    state->repmaxerror = ae_maxreal(state->repmaxerror, ae_fabs(v-state->tasky.ptr.p_double[i], _state), _state);
    i = i+1;
    goto lbl_53;
lbl_55:
    state->reprmserror = ae_sqrt(state->reprmserror/n, _state);
    state->repwrmserror = ae_sqrt(state->repwrmserror/n, _state);
    state->repavgerror = state->repavgerror/n;
    if( ae_fp_neq(relcnt,(double)(0)) )
    {
        state->repavgrelerror = state->repavgrelerror/relcnt;
    }
    
    /*
     * Calculate covariance matrix
     */
    rmatrixsetlengthatleast(&state->tmpjac, n, k, _state);
    rvectorsetlengthatleast(&state->tmpf, n, _state);
    rvectorsetlengthatleast(&state->tmp, k, _state);
    if( ae_fp_less_eq(state->diffstep,(double)(0)) )
    {
        goto lbl_56;
    }
    
    /*
     * Compute Jacobian by means of numerical differentiation
     */
    lsfit_lsfitclearrequestfields(state, _state);
    state->needf = ae_true;
    i = 0;
lbl_58:
    if( i>n-1 )
    {
        goto lbl_60;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->taskx.ptr.pp_double[i][0], 1, ae_v_len(0,m-1));
    state->pointindex = i;
    state->rstate.stage = 10;
    goto lbl_rcomm;
lbl_10:
    state->tmpf.ptr.p_double[i] = state->f;
    j = 0;
lbl_61:
    if( j>k-1 )
    {
        goto lbl_63;
    }
    v = state->c.ptr.p_double[j];
    lx = v-state->diffstep*state->s.ptr.p_double[j];
    state->c.ptr.p_double[j] = lx;
    if( ae_isfinite(state->bndl.ptr.p_double[j], _state) )
    {
        state->c.ptr.p_double[j] = ae_maxreal(state->c.ptr.p_double[j], state->bndl.ptr.p_double[j], _state);
    }
    state->rstate.stage = 11;
    goto lbl_rcomm;
lbl_11:
    lf = state->f;
    rx = v+state->diffstep*state->s.ptr.p_double[j];
    state->c.ptr.p_double[j] = rx;
    if( ae_isfinite(state->bndu.ptr.p_double[j], _state) )
    {
        state->c.ptr.p_double[j] = ae_minreal(state->c.ptr.p_double[j], state->bndu.ptr.p_double[j], _state);
    }
    state->rstate.stage = 12;
    goto lbl_rcomm;
lbl_12:
    rf = state->f;
    state->c.ptr.p_double[j] = v;
    if( ae_fp_neq(rx,lx) )
    {
        state->tmpjac.ptr.pp_double[i][j] = (rf-lf)/(rx-lx);
    }
    else
    {
        state->tmpjac.ptr.pp_double[i][j] = (double)(0);
    }
    j = j+1;
    goto lbl_61;
lbl_63:
    i = i+1;
    goto lbl_58;
lbl_60:
    state->needf = ae_false;
    goto lbl_57;
lbl_56:
    
    /*
     * Jacobian is calculated with user-provided analytic gradient
     */
    lsfit_lsfitclearrequestfields(state, _state);
    state->needfg = ae_true;
    i = 0;
lbl_64:
    if( i>n-1 )
    {
        goto lbl_66;
    }
    ae_v_move(&state->x.ptr.p_double[0], 1, &state->taskx.ptr.pp_double[i][0], 1, ae_v_len(0,m-1));
    state->pointindex = i;
    state->rstate.stage = 13;
    goto lbl_rcomm;
lbl_13:
    state->tmpf.ptr.p_double[i] = state->f;
    for(j=0; j<=k-1; j++)
    {
        state->tmpjac.ptr.pp_double[i][j] = state->g.ptr.p_double[j];
    }
    i = i+1;
    goto lbl_64;
lbl_66:
    state->needfg = ae_false;
lbl_57:
    for(i=0; i<=k-1; i++)
    {
        state->tmp.ptr.p_double[i] = 0.0;
    }
    lsfit_estimateerrors(&state->tmpjac, &state->tmpf, &state->tasky, &state->wcur, &state->tmp, &state->s, n, k, &state->rep, &state->tmpjacw, 0, _state);
lbl_51:
    result = ae_false;
    return result;
    
    /*
     * Saving state
     */
lbl_rcomm:
    result = ae_true;
    state->rstate.ia.ptr.p_int[0] = n;
    state->rstate.ia.ptr.p_int[1] = m;
    state->rstate.ia.ptr.p_int[2] = k;
    state->rstate.ia.ptr.p_int[3] = i;
    state->rstate.ia.ptr.p_int[4] = j;
    state->rstate.ia.ptr.p_int[5] = j1;
    state->rstate.ia.ptr.p_int[6] = info;
    state->rstate.ra.ptr.p_double[0] = lx;
    state->rstate.ra.ptr.p_double[1] = lf;
    state->rstate.ra.ptr.p_double[2] = ld;
    state->rstate.ra.ptr.p_double[3] = rx;
    state->rstate.ra.ptr.p_double[4] = rf;
    state->rstate.ra.ptr.p_double[5] = rd;
    state->rstate.ra.ptr.p_double[6] = v;
    state->rstate.ra.ptr.p_double[7] = vv;
    state->rstate.ra.ptr.p_double[8] = relcnt;
    return result;
}


/*************************************************************************
Nonlinear least squares fitting results.

Called after return from LSFitFit().

INPUT PARAMETERS:
    State   -   algorithm state

OUTPUT PARAMETERS:
    Info    -   completion code:
                    * -7    gradient verification failed.
                            See LSFitSetGradientCheck() for more information.
                    *  1    relative function improvement is no more than
                            EpsF.
                    *  2    relative step is no more than EpsX.
                    *  4    gradient norm is no more than EpsG
                    *  5    MaxIts steps was taken
                    *  7    stopping conditions are too stringent,
                            further improvement is impossible
    C       -   array[0..K-1], solution
    Rep     -   optimization report. On success following fields are set:
                * R2                non-adjusted coefficient of determination
                                    (non-weighted)
                * RMSError          rms error on the (X,Y).
                * AvgError          average error on the (X,Y).
                * AvgRelError       average relative error on the non-zero Y
                * MaxError          maximum error
                                    NON-WEIGHTED ERRORS ARE CALCULATED
                * WRMSError         weighted rms error on the (X,Y).
                
ERRORS IN PARAMETERS                
                
This  solver  also  calculates different kinds of errors in parameters and
fills corresponding fields of report:
* Rep.CovPar        covariance matrix for parameters, array[K,K].
* Rep.ErrPar        errors in parameters, array[K],
                    errpar = sqrt(diag(CovPar))
* Rep.ErrCurve      vector of fit errors - standard deviations of empirical
                    best-fit curve from "ideal" best-fit curve built  with
                    infinite number of samples, array[N].
                    errcurve = sqrt(diag(J*CovPar*J')),
                    where J is Jacobian matrix.
* Rep.Noise         vector of per-point estimates of noise, array[N]

IMPORTANT:  errors  in  parameters  are  calculated  without  taking  into
            account boundary/linear constraints! Presence  of  constraints
            changes distribution of errors, but there is no  easy  way  to
            account for constraints when you calculate covariance matrix.
            
NOTE:       noise in the data is estimated as follows:
            * for fitting without user-supplied  weights  all  points  are
              assumed to have same level of noise, which is estimated from
              the data
            * for fitting with user-supplied weights we assume that  noise
              level in I-th point is inversely proportional to Ith weight.
              Coefficient of proportionality is estimated from the data.
            
NOTE:       we apply small amount of regularization when we invert squared
            Jacobian and calculate covariance matrix. It  guarantees  that
            algorithm won't divide by zero  during  inversion,  but  skews
            error estimates a bit (fractional error is about 10^-9).
            
            However, we believe that this difference is insignificant  for
            all practical purposes except for the situation when you  want
            to compare ALGLIB results with "reference"  implementation  up
            to the last significant digit.
            
NOTE:       covariance matrix is estimated using  correction  for  degrees
            of freedom (covariances are divided by N-M instead of dividing
            by N).

  -- ALGLIB --
     Copyright 17.08.2009 by Bochkanov Sergey
*************************************************************************/
void lsfitresults(lsfitstate* state,
     ae_int_t* info,
     /* Real    */ ae_vector* c,
     lsfitreport* rep,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    *info = 0;
    ae_vector_clear(c);
    _lsfitreport_clear(rep);

    lsfit_clearreport(rep, _state);
    *info = state->repterminationtype;
    rep->varidx = state->repvaridx;
    if( *info>0 )
    {
        ae_vector_set_length(c, state->k, _state);
        ae_v_move(&c->ptr.p_double[0], 1, &state->c.ptr.p_double[0], 1, ae_v_len(0,state->k-1));
        rep->rmserror = state->reprmserror;
        rep->wrmserror = state->repwrmserror;
        rep->avgerror = state->repavgerror;
        rep->avgrelerror = state->repavgrelerror;
        rep->maxerror = state->repmaxerror;
        rep->iterationscount = state->repiterationscount;
        ae_matrix_set_length(&rep->covpar, state->k, state->k, _state);
        ae_vector_set_length(&rep->errpar, state->k, _state);
        ae_vector_set_length(&rep->errcurve, state->npoints, _state);
        ae_vector_set_length(&rep->noise, state->npoints, _state);
        rep->r2 = state->rep.r2;
        for(i=0; i<=state->k-1; i++)
        {
            for(j=0; j<=state->k-1; j++)
            {
                rep->covpar.ptr.pp_double[i][j] = state->rep.covpar.ptr.pp_double[i][j];
            }
            rep->errpar.ptr.p_double[i] = state->rep.errpar.ptr.p_double[i];
        }
        for(i=0; i<=state->npoints-1; i++)
        {
            rep->errcurve.ptr.p_double[i] = state->rep.errcurve.ptr.p_double[i];
            rep->noise.ptr.p_double[i] = state->rep.noise.ptr.p_double[i];
        }
    }
}


/*************************************************************************
This  subroutine  turns  on  verification  of  the  user-supplied analytic
gradient:
* user calls this subroutine before fitting begins
* LSFitFit() is called
* prior to actual fitting, for  each  point  in  data  set  X_i  and  each
  component  of  parameters  being  fited C_j algorithm performs following
  steps:
  * two trial steps are made to C_j-TestStep*S[j] and C_j+TestStep*S[j],
    where C_j is j-th parameter and S[j] is a scale of j-th parameter
  * if needed, steps are bounded with respect to constraints on C[]
  * F(X_i|C) is evaluated at these trial points
  * we perform one more evaluation in the middle point of the interval
  * we  build  cubic  model using function values and derivatives at trial
    points and we compare its prediction with actual value in  the  middle
    point
  * in case difference between prediction and actual value is higher  than
    some predetermined threshold, algorithm stops with completion code -7;
    Rep.VarIdx is set to index of the parameter with incorrect derivative.
* after verification is over, algorithm proceeds to the actual optimization.

NOTE 1: verification needs N*K (points count * parameters count)  gradient
        evaluations. It is very costly and you should use it only for  low
        dimensional  problems,  when  you  want  to  be  sure  that you've
        correctly calculated analytic derivatives. You should not  use  it
        in the production code  (unless  you  want  to  check  derivatives
        provided by some third party).

NOTE 2: you  should  carefully  choose  TestStep. Value which is too large
        (so large that function behaviour is significantly non-cubic) will
        lead to false alarms. You may use  different  step  for  different
        parameters by means of setting scale with LSFitSetScale().

NOTE 3: this function may lead to false positives. In case it reports that
        I-th  derivative was calculated incorrectly, you may decrease test
        step  and  try  one  more  time  - maybe your function changes too
        sharply  and  your  step  is  too  large for such rapidly chanding
        function.

NOTE 4: this function works only for optimizers created with LSFitCreateWFG()
        or LSFitCreateFG() constructors.
        
INPUT PARAMETERS:
    State       -   structure used to store algorithm state
    TestStep    -   verification step:
                    * TestStep=0 turns verification off
                    * TestStep>0 activates verification

  -- ALGLIB --
     Copyright 15.06.2012 by Bochkanov Sergey
*************************************************************************/
void lsfitsetgradientcheck(lsfitstate* state,
     double teststep,
     ae_state *_state)
{


    ae_assert(ae_isfinite(teststep, _state), "LSFitSetGradientCheck: TestStep contains NaN or Infinite", _state);
    ae_assert(ae_fp_greater_eq(teststep,(double)(0)), "LSFitSetGradientCheck: invalid argument TestStep(TestStep<0)", _state);
    state->teststep = teststep;
}


/*************************************************************************
Internal subroutine: automatic scaling for LLS tasks.
NEVER CALL IT DIRECTLY!

Maps abscissas to [-1,1], standartizes ordinates and correspondingly scales
constraints. It also scales weights so that max(W[i])=1

Transformations performed:
* X, XC         [XA,XB] => [-1,+1]
                transformation makes min(X)=-1, max(X)=+1

* Y             [SA,SB] => [0,1]
                transformation makes mean(Y)=0, stddev(Y)=1
                
* YC            transformed accordingly to SA, SB, DC[I]

  -- ALGLIB PROJECT --
     Copyright 08.09.2009 by Bochkanov Sergey
*************************************************************************/
void lsfitscalexy(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     ae_int_t n,
     /* Real    */ ae_vector* xc,
     /* Real    */ ae_vector* yc,
     /* Integer */ ae_vector* dc,
     ae_int_t k,
     double* xa,
     double* xb,
     double* sa,
     double* sb,
     /* Real    */ ae_vector* xoriginal,
     /* Real    */ ae_vector* yoriginal,
     ae_state *_state)
{
    double xmin;
    double xmax;
    ae_int_t i;
    double mx;

    *xa = 0;
    *xb = 0;
    *sa = 0;
    *sb = 0;
    ae_vector_clear(xoriginal);
    ae_vector_clear(yoriginal);

    ae_assert(n>=1, "LSFitScaleXY: incorrect N", _state);
    ae_assert(k>=0, "LSFitScaleXY: incorrect K", _state);
    
    /*
     * Calculate xmin/xmax.
     * Force xmin<>xmax.
     */
    xmin = x->ptr.p_double[0];
    xmax = x->ptr.p_double[0];
    for(i=1; i<=n-1; i++)
    {
        xmin = ae_minreal(xmin, x->ptr.p_double[i], _state);
        xmax = ae_maxreal(xmax, x->ptr.p_double[i], _state);
    }
    for(i=0; i<=k-1; i++)
    {
        xmin = ae_minreal(xmin, xc->ptr.p_double[i], _state);
        xmax = ae_maxreal(xmax, xc->ptr.p_double[i], _state);
    }
    if( ae_fp_eq(xmin,xmax) )
    {
        if( ae_fp_eq(xmin,(double)(0)) )
        {
            xmin = (double)(-1);
            xmax = (double)(1);
        }
        else
        {
            if( ae_fp_greater(xmin,(double)(0)) )
            {
                xmin = 0.5*xmin;
            }
            else
            {
                xmax = 0.5*xmax;
            }
        }
    }
    
    /*
     * Transform abscissas: map [XA,XB] to [0,1]
     *
     * Store old X[] in XOriginal[] (it will be used
     * to calculate relative error).
     */
    ae_vector_set_length(xoriginal, n, _state);
    ae_v_move(&xoriginal->ptr.p_double[0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
    *xa = xmin;
    *xb = xmax;
    for(i=0; i<=n-1; i++)
    {
        x->ptr.p_double[i] = 2*(x->ptr.p_double[i]-0.5*(*xa+(*xb)))/(*xb-(*xa));
    }
    for(i=0; i<=k-1; i++)
    {
        ae_assert(dc->ptr.p_int[i]>=0, "LSFitScaleXY: internal error!", _state);
        xc->ptr.p_double[i] = 2*(xc->ptr.p_double[i]-0.5*(*xa+(*xb)))/(*xb-(*xa));
        yc->ptr.p_double[i] = yc->ptr.p_double[i]*ae_pow(0.5*(*xb-(*xa)), (double)(dc->ptr.p_int[i]), _state);
    }
    
    /*
     * Transform function values: map [SA,SB] to [0,1]
     * SA = mean(Y),
     * SB = SA+stddev(Y).
     *
     * Store old Y[] in YOriginal[] (it will be used
     * to calculate relative error).
     */
    ae_vector_set_length(yoriginal, n, _state);
    ae_v_move(&yoriginal->ptr.p_double[0], 1, &y->ptr.p_double[0], 1, ae_v_len(0,n-1));
    *sa = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        *sa = *sa+y->ptr.p_double[i];
    }
    *sa = *sa/n;
    *sb = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        *sb = *sb+ae_sqr(y->ptr.p_double[i]-(*sa), _state);
    }
    *sb = ae_sqrt(*sb/n, _state)+(*sa);
    if( ae_fp_eq(*sb,*sa) )
    {
        *sb = 2*(*sa);
    }
    if( ae_fp_eq(*sb,*sa) )
    {
        *sb = *sa+1;
    }
    for(i=0; i<=n-1; i++)
    {
        y->ptr.p_double[i] = (y->ptr.p_double[i]-(*sa))/(*sb-(*sa));
    }
    for(i=0; i<=k-1; i++)
    {
        if( dc->ptr.p_int[i]==0 )
        {
            yc->ptr.p_double[i] = (yc->ptr.p_double[i]-(*sa))/(*sb-(*sa));
        }
        else
        {
            yc->ptr.p_double[i] = yc->ptr.p_double[i]/(*sb-(*sa));
        }
    }
    
    /*
     * Scale weights
     */
    mx = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        mx = ae_maxreal(mx, ae_fabs(w->ptr.p_double[i], _state), _state);
    }
    if( ae_fp_neq(mx,(double)(0)) )
    {
        for(i=0; i<=n-1; i++)
        {
            w->ptr.p_double[i] = w->ptr.p_double[i]/mx;
        }
    }
}


/*************************************************************************
This function analyzes section of curve for processing by RDP algorithm:
given set of points X,Y with indexes [I0,I1] it returns point with
worst deviation from linear model (non-parametric version which sees curve
as Y(x)).

Input parameters:
    X, Y        -   SORTED arrays.
    I0,I1       -   interval (boundaries included) to process
    Eps         -   desired precision
    
OUTPUT PARAMETERS:
    WorstIdx    -   index of worst point
    WorstError  -   error at worst point
    
NOTE: this function guarantees that it returns exactly zero for a section
      with less than 3 points.

  -- ALGLIB PROJECT --
     Copyright 02.10.2014 by Bochkanov Sergey
*************************************************************************/
static void lsfit_rdpanalyzesection(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t i0,
     ae_int_t i1,
     ae_int_t* worstidx,
     double* worsterror,
     ae_state *_state)
{
    ae_int_t i;
    double xleft;
    double xright;
    double vx;
    double ve;
    double a;
    double b;

    *worstidx = 0;
    *worsterror = 0;

    xleft = x->ptr.p_double[i0];
    xright = x->ptr.p_double[i1];
    if( i1-i0+1<3||ae_fp_eq(xright,xleft) )
    {
        *worstidx = i0;
        *worsterror = 0.0;
        return;
    }
    a = (y->ptr.p_double[i1]-y->ptr.p_double[i0])/(xright-xleft);
    b = (y->ptr.p_double[i0]*xright-y->ptr.p_double[i1]*xleft)/(xright-xleft);
    *worstidx = -1;
    *worsterror = (double)(0);
    for(i=i0+1; i<=i1-1; i++)
    {
        vx = x->ptr.p_double[i];
        ve = ae_fabs(a*vx+b-y->ptr.p_double[i], _state);
        if( (ae_fp_greater(vx,xleft)&&ae_fp_less(vx,xright))&&ae_fp_greater(ve,*worsterror) )
        {
            *worsterror = ve;
            *worstidx = i;
        }
    }
}


/*************************************************************************
Recursive splitting of interval [I0,I1] (right boundary included) with RDP
algorithm (non-parametric version which sees curve as Y(x)).

Input parameters:
    X, Y        -   SORTED arrays.
    I0,I1       -   interval (boundaries included) to process
    Eps         -   desired precision
    XOut,YOut   -   preallocated output arrays large enough to store result;
                    XOut[0..1], YOut[0..1] contain first and last points of
                    curve
    NOut        -   must contain 2 on input
    
OUTPUT PARAMETERS:
    XOut, YOut  -   curve generated by RDP algorithm, UNSORTED
    NOut        -   number of points in curve

  -- ALGLIB PROJECT --
     Copyright 02.10.2014 by Bochkanov Sergey
*************************************************************************/
static void lsfit_rdprecursive(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t i0,
     ae_int_t i1,
     double eps,
     /* Real    */ ae_vector* xout,
     /* Real    */ ae_vector* yout,
     ae_int_t* nout,
     ae_state *_state)
{
    ae_int_t worstidx;
    double worsterror;


    ae_assert(ae_fp_greater(eps,(double)(0)), "RDPRecursive: internal error, Eps<0", _state);
    lsfit_rdpanalyzesection(x, y, i0, i1, &worstidx, &worsterror, _state);
    if( ae_fp_less_eq(worsterror,eps) )
    {
        return;
    }
    xout->ptr.p_double[*nout] = x->ptr.p_double[worstidx];
    yout->ptr.p_double[*nout] = y->ptr.p_double[worstidx];
    *nout = *nout+1;
    if( worstidx-i0<i1-worstidx )
    {
        lsfit_rdprecursive(x, y, i0, worstidx, eps, xout, yout, nout, _state);
        lsfit_rdprecursive(x, y, worstidx, i1, eps, xout, yout, nout, _state);
    }
    else
    {
        lsfit_rdprecursive(x, y, worstidx, i1, eps, xout, yout, nout, _state);
        lsfit_rdprecursive(x, y, i0, worstidx, eps, xout, yout, nout, _state);
    }
}


/*************************************************************************
Internal 4PL/5PL fitting function.

Accepts X, Y and already initialized and prepared MinLMState structure.
On input P1 contains initial guess, on output it contains solution.  FLast
stores function value at P1.
*************************************************************************/
static void lsfit_logisticfitinternal(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_bool is4pl,
     double lambdav,
     minlmstate* state,
     minlmreport* replm,
     /* Real    */ ae_vector* p1,
     double* flast,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double ta;
    double tb;
    double tc;
    double td;
    double tg;
    double vp;
    double vp0;
    double vp1;

    *flast = 0;

    minlmrestartfrom(state, p1, _state);
    while(minlmiteration(state, _state))
    {
        ta = state->x.ptr.p_double[0];
        tb = state->x.ptr.p_double[1];
        tc = state->x.ptr.p_double[2];
        td = state->x.ptr.p_double[3];
        tg = state->x.ptr.p_double[4];
        if( state->xupdated )
        {
            
            /*
             * Save best function value obtained so far.
             */
            *flast = state->f;
            continue;
        }
        if( state->needfi )
        {
            
            /*
             * Function vector
             */
            for(i=0; i<=n-1; i++)
            {
                if( ae_fp_greater(x->ptr.p_double[i],(double)(0)) )
                {
                    state->fi.ptr.p_double[i] = td+(ta-td)/ae_pow(1.0+ae_pow(x->ptr.p_double[i]/tc, tb, _state), tg, _state)-y->ptr.p_double[i];
                }
                else
                {
                    if( ae_fp_greater_eq(tb,(double)(0)) )
                    {
                        state->fi.ptr.p_double[i] = ta-y->ptr.p_double[i];
                    }
                    else
                    {
                        state->fi.ptr.p_double[i] = td-y->ptr.p_double[i];
                    }
                }
            }
            for(i=0; i<=4; i++)
            {
                state->fi.ptr.p_double[n+i] = lambdav*state->x.ptr.p_double[i];
            }
            continue;
        }
        if( state->needfij )
        {
            
            /*
             * Function vector and Jacobian
             */
            for(i=0; i<=n-1; i++)
            {
                if( ae_fp_greater(x->ptr.p_double[i],(double)(0)) )
                {
                    if( is4pl )
                    {
                        vp = ae_pow(x->ptr.p_double[i]/tc, tb, _state);
                        state->fi.ptr.p_double[i] = td+(ta-td)/(1+vp)-y->ptr.p_double[i];
                        state->j.ptr.pp_double[i][0] = 1/(1+vp);
                        state->j.ptr.pp_double[i][1] = -(ta-td)*vp*ae_log(x->ptr.p_double[i]/tc, _state)/ae_sqr(1+vp, _state);
                        state->j.ptr.pp_double[i][2] = (ta-td)*(tb/tc)*vp/ae_sqr(1+vp, _state);
                        state->j.ptr.pp_double[i][3] = 1-1/(1+vp);
                        state->j.ptr.pp_double[i][4] = (double)(0);
                    }
                    else
                    {
                        vp0 = ae_pow(x->ptr.p_double[i]/tc, tb, _state);
                        vp1 = ae_pow(1+vp0, tg, _state);
                        state->fi.ptr.p_double[i] = td+(ta-td)/vp1-y->ptr.p_double[i];
                        state->j.ptr.pp_double[i][0] = 1/vp1;
                        state->j.ptr.pp_double[i][1] = (ta-td)*(-tg)*ae_pow(1+vp0, -tg-1, _state)*vp0*ae_log(x->ptr.p_double[i]/tc, _state);
                        state->j.ptr.pp_double[i][2] = (ta-td)*(-tg)*ae_pow(1+vp0, -tg-1, _state)*vp0*(-tb/tc);
                        state->j.ptr.pp_double[i][3] = 1-1/vp1;
                        state->j.ptr.pp_double[i][4] = -(ta-td)/vp1*ae_log(1+vp0, _state);
                    }
                }
                else
                {
                    if( ae_fp_greater_eq(tb,(double)(0)) )
                    {
                        state->fi.ptr.p_double[i] = ta-y->ptr.p_double[i];
                        state->j.ptr.pp_double[i][0] = (double)(1);
                        state->j.ptr.pp_double[i][1] = (double)(0);
                        state->j.ptr.pp_double[i][2] = (double)(0);
                        state->j.ptr.pp_double[i][3] = (double)(0);
                        state->j.ptr.pp_double[i][4] = (double)(0);
                    }
                    else
                    {
                        state->fi.ptr.p_double[i] = td-y->ptr.p_double[i];
                        state->j.ptr.pp_double[i][0] = (double)(0);
                        state->j.ptr.pp_double[i][1] = (double)(0);
                        state->j.ptr.pp_double[i][2] = (double)(0);
                        state->j.ptr.pp_double[i][3] = (double)(1);
                        state->j.ptr.pp_double[i][4] = (double)(0);
                    }
                }
            }
            for(i=0; i<=4; i++)
            {
                for(j=0; j<=4; j++)
                {
                    state->j.ptr.pp_double[n+i][j] = 0.0;
                }
            }
            for(i=0; i<=4; i++)
            {
                state->fi.ptr.p_double[n+i] = lambdav*state->x.ptr.p_double[i];
                state->j.ptr.pp_double[n+i][i] = lambdav;
            }
            continue;
        }
        ae_assert(ae_false, "LogisticFitX: internal error", _state);
    }
    minlmresultsbuf(state, p1, replm, _state);
    ae_assert(replm->terminationtype>0, "LogisticFitX: internal error", _state);
}


/*************************************************************************
Internal spline fitting subroutine

  -- ALGLIB PROJECT --
     Copyright 08.09.2009 by Bochkanov Sergey
*************************************************************************/
static void lsfit_spline1dfitinternal(ae_int_t st,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     ae_int_t n,
     /* Real    */ ae_vector* xc,
     /* Real    */ ae_vector* yc,
     /* Integer */ ae_vector* dc,
     ae_int_t k,
     ae_int_t m,
     ae_int_t* info,
     spline1dinterpolant* s,
     spline1dfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_vector _w;
    ae_vector _xc;
    ae_vector _yc;
    ae_matrix fmatrix;
    ae_matrix cmatrix;
    ae_vector y2;
    ae_vector w2;
    ae_vector sx;
    ae_vector sy;
    ae_vector sd;
    ae_vector tmp;
    ae_vector xoriginal;
    ae_vector yoriginal;
    lsfitreport lrep;
    double v0;
    double v1;
    double v2;
    double mx;
    spline1dinterpolant s2;
    ae_int_t i;
    ae_int_t j;
    ae_int_t relcnt;
    double xa;
    double xb;
    double sa;
    double sb;
    double bl;
    double br;
    double decay;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    ae_vector_init_copy(&_w, w, _state);
    w = &_w;
    ae_vector_init_copy(&_xc, xc, _state);
    xc = &_xc;
    ae_vector_init_copy(&_yc, yc, _state);
    yc = &_yc;
    *info = 0;
    _spline1dinterpolant_clear(s);
    _spline1dfitreport_clear(rep);
    ae_matrix_init(&fmatrix, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cmatrix, 0, 0, DT_REAL, _state);
    ae_vector_init(&y2, 0, DT_REAL, _state);
    ae_vector_init(&w2, 0, DT_REAL, _state);
    ae_vector_init(&sx, 0, DT_REAL, _state);
    ae_vector_init(&sy, 0, DT_REAL, _state);
    ae_vector_init(&sd, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    ae_vector_init(&xoriginal, 0, DT_REAL, _state);
    ae_vector_init(&yoriginal, 0, DT_REAL, _state);
    _lsfitreport_init(&lrep, _state);
    _spline1dinterpolant_init(&s2, _state);

    ae_assert(st==0||st==1, "Spline1DFit: internal error!", _state);
    if( st==0&&m<4 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    if( st==1&&m<4 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    if( (n<1||k<0)||k>=m )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=k-1; i++)
    {
        *info = 0;
        if( dc->ptr.p_int[i]<0 )
        {
            *info = -1;
        }
        if( dc->ptr.p_int[i]>1 )
        {
            *info = -1;
        }
        if( *info<0 )
        {
            ae_frame_leave(_state);
            return;
        }
    }
    if( st==1&&m%2!=0 )
    {
        
        /*
         * Hermite fitter must have even number of basis functions
         */
        *info = -2;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * weight decay for correct handling of task which becomes
     * degenerate after constraints are applied
     */
    decay = 10000*ae_machineepsilon;
    
    /*
     * Scale X, Y, XC, YC
     */
    lsfitscalexy(x, y, w, n, xc, yc, dc, k, &xa, &xb, &sa, &sb, &xoriginal, &yoriginal, _state);
    
    /*
     * allocate space, initialize:
     * * SX     -   grid for basis functions
     * * SY     -   values of basis functions at grid points
     * * FMatrix-   values of basis functions at X[]
     * * CMatrix-   values (derivatives) of basis functions at XC[]
     */
    ae_vector_set_length(&y2, n+m, _state);
    ae_vector_set_length(&w2, n+m, _state);
    ae_matrix_set_length(&fmatrix, n+m, m, _state);
    if( k>0 )
    {
        ae_matrix_set_length(&cmatrix, k, m+1, _state);
    }
    if( st==0 )
    {
        
        /*
         * allocate space for cubic spline
         */
        ae_vector_set_length(&sx, m-2, _state);
        ae_vector_set_length(&sy, m-2, _state);
        for(j=0; j<=m-2-1; j++)
        {
            sx.ptr.p_double[j] = (double)(2*j)/(double)(m-2-1)-1;
        }
    }
    if( st==1 )
    {
        
        /*
         * allocate space for Hermite spline
         */
        ae_vector_set_length(&sx, m/2, _state);
        ae_vector_set_length(&sy, m/2, _state);
        ae_vector_set_length(&sd, m/2, _state);
        for(j=0; j<=m/2-1; j++)
        {
            sx.ptr.p_double[j] = (double)(2*j)/(double)(m/2-1)-1;
        }
    }
    
    /*
     * Prepare design and constraints matrices:
     * * fill constraints matrix
     * * fill first N rows of design matrix with values
     * * fill next M rows of design matrix with regularizing term
     * * append M zeros to Y
     * * append M elements, mean(abs(W)) each, to W
     */
    for(j=0; j<=m-1; j++)
    {
        
        /*
         * prepare Jth basis function
         */
        if( st==0 )
        {
            
            /*
             * cubic spline basis
             */
            for(i=0; i<=m-2-1; i++)
            {
                sy.ptr.p_double[i] = (double)(0);
            }
            bl = (double)(0);
            br = (double)(0);
            if( j<m-2 )
            {
                sy.ptr.p_double[j] = (double)(1);
            }
            if( j==m-2 )
            {
                bl = (double)(1);
            }
            if( j==m-1 )
            {
                br = (double)(1);
            }
            spline1dbuildcubic(&sx, &sy, m-2, 1, bl, 1, br, &s2, _state);
        }
        if( st==1 )
        {
            
            /*
             * Hermite basis
             */
            for(i=0; i<=m/2-1; i++)
            {
                sy.ptr.p_double[i] = (double)(0);
                sd.ptr.p_double[i] = (double)(0);
            }
            if( j%2==0 )
            {
                sy.ptr.p_double[j/2] = (double)(1);
            }
            else
            {
                sd.ptr.p_double[j/2] = (double)(1);
            }
            spline1dbuildhermite(&sx, &sy, &sd, m/2, &s2, _state);
        }
        
        /*
         * values at X[], XC[]
         */
        for(i=0; i<=n-1; i++)
        {
            fmatrix.ptr.pp_double[i][j] = spline1dcalc(&s2, x->ptr.p_double[i], _state);
        }
        for(i=0; i<=k-1; i++)
        {
            ae_assert(dc->ptr.p_int[i]>=0&&dc->ptr.p_int[i]<=2, "Spline1DFit: internal error!", _state);
            spline1ddiff(&s2, xc->ptr.p_double[i], &v0, &v1, &v2, _state);
            if( dc->ptr.p_int[i]==0 )
            {
                cmatrix.ptr.pp_double[i][j] = v0;
            }
            if( dc->ptr.p_int[i]==1 )
            {
                cmatrix.ptr.pp_double[i][j] = v1;
            }
            if( dc->ptr.p_int[i]==2 )
            {
                cmatrix.ptr.pp_double[i][j] = v2;
            }
        }
    }
    for(i=0; i<=k-1; i++)
    {
        cmatrix.ptr.pp_double[i][m] = yc->ptr.p_double[i];
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=m-1; j++)
        {
            if( i==j )
            {
                fmatrix.ptr.pp_double[n+i][j] = decay;
            }
            else
            {
                fmatrix.ptr.pp_double[n+i][j] = (double)(0);
            }
        }
    }
    ae_vector_set_length(&y2, n+m, _state);
    ae_vector_set_length(&w2, n+m, _state);
    ae_v_move(&y2.ptr.p_double[0], 1, &y->ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_move(&w2.ptr.p_double[0], 1, &w->ptr.p_double[0], 1, ae_v_len(0,n-1));
    mx = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        mx = mx+ae_fabs(w->ptr.p_double[i], _state);
    }
    mx = mx/n;
    for(i=0; i<=m-1; i++)
    {
        y2.ptr.p_double[n+i] = (double)(0);
        w2.ptr.p_double[n+i] = mx;
    }
    
    /*
     * Solve constrained task
     */
    if( k>0 )
    {
        
        /*
         * solve using regularization
         */
        lsfitlinearwc(&y2, &w2, &fmatrix, &cmatrix, n+m, m, k, info, &tmp, &lrep, _state);
    }
    else
    {
        
        /*
         * no constraints, no regularization needed
         */
        lsfitlinearwc(y, w, &fmatrix, &cmatrix, n, m, k, info, &tmp, &lrep, _state);
    }
    if( *info<0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Generate spline and scale it
     */
    if( st==0 )
    {
        
        /*
         * cubic spline basis
         */
        ae_v_move(&sy.ptr.p_double[0], 1, &tmp.ptr.p_double[0], 1, ae_v_len(0,m-2-1));
        spline1dbuildcubic(&sx, &sy, m-2, 1, tmp.ptr.p_double[m-2], 1, tmp.ptr.p_double[m-1], s, _state);
    }
    if( st==1 )
    {
        
        /*
         * Hermite basis
         */
        for(i=0; i<=m/2-1; i++)
        {
            sy.ptr.p_double[i] = tmp.ptr.p_double[2*i];
            sd.ptr.p_double[i] = tmp.ptr.p_double[2*i+1];
        }
        spline1dbuildhermite(&sx, &sy, &sd, m/2, s, _state);
    }
    spline1dlintransx(s, 2/(xb-xa), -(xa+xb)/(xb-xa), _state);
    spline1dlintransy(s, sb-sa, sa, _state);
    
    /*
     * Scale absolute errors obtained from LSFitLinearW.
     * Relative error should be calculated separately
     * (because of shifting/scaling of the task)
     */
    rep->taskrcond = lrep.taskrcond;
    rep->rmserror = lrep.rmserror*(sb-sa);
    rep->avgerror = lrep.avgerror*(sb-sa);
    rep->maxerror = lrep.maxerror*(sb-sa);
    rep->avgrelerror = (double)(0);
    relcnt = 0;
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_neq(yoriginal.ptr.p_double[i],(double)(0)) )
        {
            rep->avgrelerror = rep->avgrelerror+ae_fabs(spline1dcalc(s, xoriginal.ptr.p_double[i], _state)-yoriginal.ptr.p_double[i], _state)/ae_fabs(yoriginal.ptr.p_double[i], _state);
            relcnt = relcnt+1;
        }
    }
    if( relcnt!=0 )
    {
        rep->avgrelerror = rep->avgrelerror/relcnt;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal fitting subroutine
*************************************************************************/
static void lsfit_lsfitlinearinternal(/* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_matrix* fmatrix,
     ae_int_t n,
     ae_int_t m,
     ae_int_t* info,
     /* Real    */ ae_vector* c,
     lsfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    double threshold;
    ae_matrix ft;
    ae_matrix q;
    ae_matrix l;
    ae_matrix r;
    ae_vector b;
    ae_vector wmod;
    ae_vector tau;
    ae_vector nzeros;
    ae_vector s;
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_vector sv;
    ae_matrix u;
    ae_matrix vt;
    ae_vector tmp;
    ae_vector utb;
    ae_vector sutb;
    ae_int_t relcnt;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    ae_vector_clear(c);
    _lsfitreport_clear(rep);
    ae_matrix_init(&ft, 0, 0, DT_REAL, _state);
    ae_matrix_init(&q, 0, 0, DT_REAL, _state);
    ae_matrix_init(&l, 0, 0, DT_REAL, _state);
    ae_matrix_init(&r, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&wmod, 0, DT_REAL, _state);
    ae_vector_init(&tau, 0, DT_REAL, _state);
    ae_vector_init(&nzeros, 0, DT_REAL, _state);
    ae_vector_init(&s, 0, DT_REAL, _state);
    ae_vector_init(&sv, 0, DT_REAL, _state);
    ae_matrix_init(&u, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vt, 0, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    ae_vector_init(&utb, 0, DT_REAL, _state);
    ae_vector_init(&sutb, 0, DT_REAL, _state);

    lsfit_clearreport(rep, _state);
    if( n<1||m<1 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    *info = 1;
    threshold = ae_sqrt(ae_machineepsilon, _state);
    
    /*
     * Degenerate case, needs special handling
     */
    if( n<m )
    {
        
        /*
         * Create design matrix.
         */
        ae_matrix_set_length(&ft, n, m, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&wmod, n, _state);
        for(j=0; j<=n-1; j++)
        {
            v = w->ptr.p_double[j];
            ae_v_moved(&ft.ptr.pp_double[j][0], 1, &fmatrix->ptr.pp_double[j][0], 1, ae_v_len(0,m-1), v);
            b.ptr.p_double[j] = w->ptr.p_double[j]*y->ptr.p_double[j];
            wmod.ptr.p_double[j] = (double)(1);
        }
        
        /*
         * LQ decomposition and reduction to M=N
         */
        ae_vector_set_length(c, m, _state);
        for(i=0; i<=m-1; i++)
        {
            c->ptr.p_double[i] = (double)(0);
        }
        rep->taskrcond = (double)(0);
        rmatrixlq(&ft, n, m, &tau, _state);
        rmatrixlqunpackq(&ft, n, m, &tau, n, &q, _state);
        rmatrixlqunpackl(&ft, n, m, &l, _state);
        lsfit_lsfitlinearinternal(&b, &wmod, &l, n, n, info, &tmp, rep, _state);
        if( *info<=0 )
        {
            ae_frame_leave(_state);
            return;
        }
        for(i=0; i<=n-1; i++)
        {
            v = tmp.ptr.p_double[i];
            ae_v_addd(&c->ptr.p_double[0], 1, &q.ptr.pp_double[i][0], 1, ae_v_len(0,m-1), v);
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * N>=M. Generate design matrix and reduce to N=M using
     * QR decomposition.
     */
    ae_matrix_set_length(&ft, n, m, _state);
    ae_vector_set_length(&b, n, _state);
    for(j=0; j<=n-1; j++)
    {
        v = w->ptr.p_double[j];
        ae_v_moved(&ft.ptr.pp_double[j][0], 1, &fmatrix->ptr.pp_double[j][0], 1, ae_v_len(0,m-1), v);
        b.ptr.p_double[j] = w->ptr.p_double[j]*y->ptr.p_double[j];
    }
    rmatrixqr(&ft, n, m, &tau, _state);
    rmatrixqrunpackq(&ft, n, m, &tau, m, &q, _state);
    rmatrixqrunpackr(&ft, n, m, &r, _state);
    ae_vector_set_length(&tmp, m, _state);
    for(i=0; i<=m-1; i++)
    {
        tmp.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=n-1; i++)
    {
        v = b.ptr.p_double[i];
        ae_v_addd(&tmp.ptr.p_double[0], 1, &q.ptr.pp_double[i][0], 1, ae_v_len(0,m-1), v);
    }
    ae_vector_set_length(&b, m, _state);
    ae_v_move(&b.ptr.p_double[0], 1, &tmp.ptr.p_double[0], 1, ae_v_len(0,m-1));
    
    /*
     * R contains reduced MxM design upper triangular matrix,
     * B contains reduced Mx1 right part.
     *
     * Determine system condition number and decide
     * should we use triangular solver (faster) or
     * SVD-based solver (more stable).
     *
     * We can use LU-based RCond estimator for this task.
     */
    rep->taskrcond = rmatrixlurcondinf(&r, m, _state);
    if( ae_fp_greater(rep->taskrcond,threshold) )
    {
        
        /*
         * use QR-based solver
         */
        ae_vector_set_length(c, m, _state);
        c->ptr.p_double[m-1] = b.ptr.p_double[m-1]/r.ptr.pp_double[m-1][m-1];
        for(i=m-2; i>=0; i--)
        {
            v = ae_v_dotproduct(&r.ptr.pp_double[i][i+1], 1, &c->ptr.p_double[i+1], 1, ae_v_len(i+1,m-1));
            c->ptr.p_double[i] = (b.ptr.p_double[i]-v)/r.ptr.pp_double[i][i];
        }
    }
    else
    {
        
        /*
         * use SVD-based solver
         */
        if( !rmatrixsvd(&r, m, m, 1, 1, 2, &sv, &u, &vt, _state) )
        {
            *info = -4;
            ae_frame_leave(_state);
            return;
        }
        ae_vector_set_length(&utb, m, _state);
        ae_vector_set_length(&sutb, m, _state);
        for(i=0; i<=m-1; i++)
        {
            utb.ptr.p_double[i] = (double)(0);
        }
        for(i=0; i<=m-1; i++)
        {
            v = b.ptr.p_double[i];
            ae_v_addd(&utb.ptr.p_double[0], 1, &u.ptr.pp_double[i][0], 1, ae_v_len(0,m-1), v);
        }
        if( ae_fp_greater(sv.ptr.p_double[0],(double)(0)) )
        {
            rep->taskrcond = sv.ptr.p_double[m-1]/sv.ptr.p_double[0];
            for(i=0; i<=m-1; i++)
            {
                if( ae_fp_greater(sv.ptr.p_double[i],threshold*sv.ptr.p_double[0]) )
                {
                    sutb.ptr.p_double[i] = utb.ptr.p_double[i]/sv.ptr.p_double[i];
                }
                else
                {
                    sutb.ptr.p_double[i] = (double)(0);
                }
            }
        }
        else
        {
            rep->taskrcond = (double)(0);
            for(i=0; i<=m-1; i++)
            {
                sutb.ptr.p_double[i] = (double)(0);
            }
        }
        ae_vector_set_length(c, m, _state);
        for(i=0; i<=m-1; i++)
        {
            c->ptr.p_double[i] = (double)(0);
        }
        for(i=0; i<=m-1; i++)
        {
            v = sutb.ptr.p_double[i];
            ae_v_addd(&c->ptr.p_double[0], 1, &vt.ptr.pp_double[i][0], 1, ae_v_len(0,m-1), v);
        }
    }
    
    /*
     * calculate errors
     */
    rep->rmserror = (double)(0);
    rep->avgerror = (double)(0);
    rep->avgrelerror = (double)(0);
    rep->maxerror = (double)(0);
    relcnt = 0;
    for(i=0; i<=n-1; i++)
    {
        v = ae_v_dotproduct(&fmatrix->ptr.pp_double[i][0], 1, &c->ptr.p_double[0], 1, ae_v_len(0,m-1));
        rep->rmserror = rep->rmserror+ae_sqr(v-y->ptr.p_double[i], _state);
        rep->avgerror = rep->avgerror+ae_fabs(v-y->ptr.p_double[i], _state);
        if( ae_fp_neq(y->ptr.p_double[i],(double)(0)) )
        {
            rep->avgrelerror = rep->avgrelerror+ae_fabs(v-y->ptr.p_double[i], _state)/ae_fabs(y->ptr.p_double[i], _state);
            relcnt = relcnt+1;
        }
        rep->maxerror = ae_maxreal(rep->maxerror, ae_fabs(v-y->ptr.p_double[i], _state), _state);
    }
    rep->rmserror = ae_sqrt(rep->rmserror/n, _state);
    rep->avgerror = rep->avgerror/n;
    if( relcnt!=0 )
    {
        rep->avgrelerror = rep->avgrelerror/relcnt;
    }
    ae_vector_set_length(&nzeros, n, _state);
    ae_vector_set_length(&s, m, _state);
    for(i=0; i<=m-1; i++)
    {
        s.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=m-1; j++)
        {
            s.ptr.p_double[j] = s.ptr.p_double[j]+ae_sqr(fmatrix->ptr.pp_double[i][j], _state);
        }
        nzeros.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=m-1; i++)
    {
        if( ae_fp_neq(s.ptr.p_double[i],(double)(0)) )
        {
            s.ptr.p_double[i] = ae_sqrt(1/s.ptr.p_double[i], _state);
        }
        else
        {
            s.ptr.p_double[i] = (double)(1);
        }
    }
    lsfit_estimateerrors(fmatrix, &nzeros, y, w, c, &s, n, m, rep, &r, 1, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Internal subroutine
*************************************************************************/
static void lsfit_lsfitclearrequestfields(lsfitstate* state,
     ae_state *_state)
{


    state->needf = ae_false;
    state->needfg = ae_false;
    state->needfgh = ae_false;
    state->xupdated = ae_false;
}


/*************************************************************************
Internal subroutine, calculates barycentric basis functions.
Used for efficient simultaneous calculation of N basis functions.

  -- ALGLIB --
     Copyright 17.08.2009 by Bochkanov Sergey
*************************************************************************/
static void lsfit_barycentriccalcbasis(barycentricinterpolant* b,
     double t,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{
    double s2;
    double s;
    double v;
    ae_int_t i;
    ae_int_t j;


    
    /*
     * special case: N=1
     */
    if( b->n==1 )
    {
        y->ptr.p_double[0] = (double)(1);
        return;
    }
    
    /*
     * Here we assume that task is normalized, i.e.:
     * 1. abs(Y[i])<=1
     * 2. abs(W[i])<=1
     * 3. X[] is ordered
     *
     * First, we decide: should we use "safe" formula (guarded
     * against overflow) or fast one?
     */
    s = ae_fabs(t-b->x.ptr.p_double[0], _state);
    for(i=0; i<=b->n-1; i++)
    {
        v = b->x.ptr.p_double[i];
        if( ae_fp_eq(v,t) )
        {
            for(j=0; j<=b->n-1; j++)
            {
                y->ptr.p_double[j] = (double)(0);
            }
            y->ptr.p_double[i] = (double)(1);
            return;
        }
        v = ae_fabs(t-v, _state);
        if( ae_fp_less(v,s) )
        {
            s = v;
        }
    }
    s2 = (double)(0);
    for(i=0; i<=b->n-1; i++)
    {
        v = s/(t-b->x.ptr.p_double[i]);
        v = v*b->w.ptr.p_double[i];
        y->ptr.p_double[i] = v;
        s2 = s2+v;
    }
    v = 1/s2;
    ae_v_muld(&y->ptr.p_double[0], 1, ae_v_len(0,b->n-1), v);
}


/*************************************************************************
This is internal function for Chebyshev fitting.

It assumes that input data are normalized:
* X/XC belong to [-1,+1],
* mean(Y)=0, stddev(Y)=1.

It does not checks inputs for errors.

This function is used to fit general (shifted) Chebyshev models, power
basis models or barycentric models.

INPUT PARAMETERS:
    X   -   points, array[0..N-1].
    Y   -   function values, array[0..N-1].
    W   -   weights, array[0..N-1]
    N   -   number of points, N>0.
    XC  -   points where polynomial values/derivatives are constrained,
            array[0..K-1].
    YC  -   values of constraints, array[0..K-1]
    DC  -   array[0..K-1], types of constraints:
            * DC[i]=0   means that P(XC[i])=YC[i]
            * DC[i]=1   means that P'(XC[i])=YC[i]
    K   -   number of constraints, 0<=K<M.
            K=0 means no constraints (XC/YC/DC are not used in such cases)
    M   -   number of basis functions (= polynomial_degree + 1), M>=1

OUTPUT PARAMETERS:
    Info-   same format as in LSFitLinearW() subroutine:
            * Info>0    task is solved
            * Info<=0   an error occured:
                        -4 means inconvergence of internal SVD
                        -3 means inconsistent constraints
    C   -   interpolant in Chebyshev form; [-1,+1] is used as base interval
    Rep -   report, same format as in LSFitLinearW() subroutine.
            Following fields are set:
            * RMSError      rms error on the (X,Y).
            * AvgError      average error on the (X,Y).
            * AvgRelError   average relative error on the non-zero Y
            * MaxError      maximum error
                            NON-WEIGHTED ERRORS ARE CALCULATED

IMPORTANT:
    this subroitine doesn't calculate task's condition number for K<>0.

  -- ALGLIB PROJECT --
     Copyright 10.12.2009 by Bochkanov Sergey
*************************************************************************/
static void lsfit_internalchebyshevfit(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     ae_int_t n,
     /* Real    */ ae_vector* xc,
     /* Real    */ ae_vector* yc,
     /* Integer */ ae_vector* dc,
     ae_int_t k,
     ae_int_t m,
     ae_int_t* info,
     /* Real    */ ae_vector* c,
     lsfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _xc;
    ae_vector _yc;
    ae_vector y2;
    ae_vector w2;
    ae_vector tmp;
    ae_vector tmp2;
    ae_vector tmpdiff;
    ae_vector bx;
    ae_vector by;
    ae_vector bw;
    ae_matrix fmatrix;
    ae_matrix cmatrix;
    ae_int_t i;
    ae_int_t j;
    double mx;
    double decay;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_xc, xc, _state);
    xc = &_xc;
    ae_vector_init_copy(&_yc, yc, _state);
    yc = &_yc;
    *info = 0;
    ae_vector_clear(c);
    _lsfitreport_clear(rep);
    ae_vector_init(&y2, 0, DT_REAL, _state);
    ae_vector_init(&w2, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    ae_vector_init(&tmp2, 0, DT_REAL, _state);
    ae_vector_init(&tmpdiff, 0, DT_REAL, _state);
    ae_vector_init(&bx, 0, DT_REAL, _state);
    ae_vector_init(&by, 0, DT_REAL, _state);
    ae_vector_init(&bw, 0, DT_REAL, _state);
    ae_matrix_init(&fmatrix, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cmatrix, 0, 0, DT_REAL, _state);

    lsfit_clearreport(rep, _state);
    
    /*
     * weight decay for correct handling of task which becomes
     * degenerate after constraints are applied
     */
    decay = 10000*ae_machineepsilon;
    
    /*
     * allocate space, initialize/fill:
     * * FMatrix-   values of basis functions at X[]
     * * CMatrix-   values (derivatives) of basis functions at XC[]
     * * fill constraints matrix
     * * fill first N rows of design matrix with values
     * * fill next M rows of design matrix with regularizing term
     * * append M zeros to Y
     * * append M elements, mean(abs(W)) each, to W
     */
    ae_vector_set_length(&y2, n+m, _state);
    ae_vector_set_length(&w2, n+m, _state);
    ae_vector_set_length(&tmp, m, _state);
    ae_vector_set_length(&tmpdiff, m, _state);
    ae_matrix_set_length(&fmatrix, n+m, m, _state);
    if( k>0 )
    {
        ae_matrix_set_length(&cmatrix, k, m+1, _state);
    }
    
    /*
     * Fill design matrix, Y2, W2:
     * * first N rows with basis functions for original points
     * * next M rows with decay terms
     */
    for(i=0; i<=n-1; i++)
    {
        
        /*
         * prepare Ith row
         * use Tmp for calculations to avoid multidimensional arrays overhead
         */
        for(j=0; j<=m-1; j++)
        {
            if( j==0 )
            {
                tmp.ptr.p_double[j] = (double)(1);
            }
            else
            {
                if( j==1 )
                {
                    tmp.ptr.p_double[j] = x->ptr.p_double[i];
                }
                else
                {
                    tmp.ptr.p_double[j] = 2*x->ptr.p_double[i]*tmp.ptr.p_double[j-1]-tmp.ptr.p_double[j-2];
                }
            }
        }
        ae_v_move(&fmatrix.ptr.pp_double[i][0], 1, &tmp.ptr.p_double[0], 1, ae_v_len(0,m-1));
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=m-1; j++)
        {
            if( i==j )
            {
                fmatrix.ptr.pp_double[n+i][j] = decay;
            }
            else
            {
                fmatrix.ptr.pp_double[n+i][j] = (double)(0);
            }
        }
    }
    ae_v_move(&y2.ptr.p_double[0], 1, &y->ptr.p_double[0], 1, ae_v_len(0,n-1));
    ae_v_move(&w2.ptr.p_double[0], 1, &w->ptr.p_double[0], 1, ae_v_len(0,n-1));
    mx = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        mx = mx+ae_fabs(w->ptr.p_double[i], _state);
    }
    mx = mx/n;
    for(i=0; i<=m-1; i++)
    {
        y2.ptr.p_double[n+i] = (double)(0);
        w2.ptr.p_double[n+i] = mx;
    }
    
    /*
     * fill constraints matrix
     */
    for(i=0; i<=k-1; i++)
    {
        
        /*
         * prepare Ith row
         * use Tmp for basis function values,
         * TmpDiff for basos function derivatives
         */
        for(j=0; j<=m-1; j++)
        {
            if( j==0 )
            {
                tmp.ptr.p_double[j] = (double)(1);
                tmpdiff.ptr.p_double[j] = (double)(0);
            }
            else
            {
                if( j==1 )
                {
                    tmp.ptr.p_double[j] = xc->ptr.p_double[i];
                    tmpdiff.ptr.p_double[j] = (double)(1);
                }
                else
                {
                    tmp.ptr.p_double[j] = 2*xc->ptr.p_double[i]*tmp.ptr.p_double[j-1]-tmp.ptr.p_double[j-2];
                    tmpdiff.ptr.p_double[j] = 2*(tmp.ptr.p_double[j-1]+xc->ptr.p_double[i]*tmpdiff.ptr.p_double[j-1])-tmpdiff.ptr.p_double[j-2];
                }
            }
        }
        if( dc->ptr.p_int[i]==0 )
        {
            ae_v_move(&cmatrix.ptr.pp_double[i][0], 1, &tmp.ptr.p_double[0], 1, ae_v_len(0,m-1));
        }
        if( dc->ptr.p_int[i]==1 )
        {
            ae_v_move(&cmatrix.ptr.pp_double[i][0], 1, &tmpdiff.ptr.p_double[0], 1, ae_v_len(0,m-1));
        }
        cmatrix.ptr.pp_double[i][m] = yc->ptr.p_double[i];
    }
    
    /*
     * Solve constrained task
     */
    if( k>0 )
    {
        
        /*
         * solve using regularization
         */
        lsfitlinearwc(&y2, &w2, &fmatrix, &cmatrix, n+m, m, k, info, c, rep, _state);
    }
    else
    {
        
        /*
         * no constraints, no regularization needed
         */
        lsfitlinearwc(y, w, &fmatrix, &cmatrix, n, m, 0, info, c, rep, _state);
    }
    if( *info<0 )
    {
        ae_frame_leave(_state);
        return;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal Floater-Hormann fitting subroutine for fixed D
*************************************************************************/
static void lsfit_barycentricfitwcfixedd(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     ae_int_t n,
     /* Real    */ ae_vector* xc,
     /* Real    */ ae_vector* yc,
     /* Integer */ ae_vector* dc,
     ae_int_t k,
     ae_int_t m,
     ae_int_t d,
     ae_int_t* info,
     barycentricinterpolant* b,
     barycentricfitreport* rep,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _x;
    ae_vector _y;
    ae_vector _w;
    ae_vector _xc;
    ae_vector _yc;
    ae_matrix fmatrix;
    ae_matrix cmatrix;
    ae_vector y2;
    ae_vector w2;
    ae_vector sx;
    ae_vector sy;
    ae_vector sbf;
    ae_vector xoriginal;
    ae_vector yoriginal;
    ae_vector tmp;
    lsfitreport lrep;
    double v0;
    double v1;
    double mx;
    barycentricinterpolant b2;
    ae_int_t i;
    ae_int_t j;
    ae_int_t relcnt;
    double xa;
    double xb;
    double sa;
    double sb;
    double decay;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_x, x, _state);
    x = &_x;
    ae_vector_init_copy(&_y, y, _state);
    y = &_y;
    ae_vector_init_copy(&_w, w, _state);
    w = &_w;
    ae_vector_init_copy(&_xc, xc, _state);
    xc = &_xc;
    ae_vector_init_copy(&_yc, yc, _state);
    yc = &_yc;
    *info = 0;
    _barycentricinterpolant_clear(b);
    _barycentricfitreport_clear(rep);
    ae_matrix_init(&fmatrix, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cmatrix, 0, 0, DT_REAL, _state);
    ae_vector_init(&y2, 0, DT_REAL, _state);
    ae_vector_init(&w2, 0, DT_REAL, _state);
    ae_vector_init(&sx, 0, DT_REAL, _state);
    ae_vector_init(&sy, 0, DT_REAL, _state);
    ae_vector_init(&sbf, 0, DT_REAL, _state);
    ae_vector_init(&xoriginal, 0, DT_REAL, _state);
    ae_vector_init(&yoriginal, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    _lsfitreport_init(&lrep, _state);
    _barycentricinterpolant_init(&b2, _state);

    if( ((n<1||m<2)||k<0)||k>=m )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=k-1; i++)
    {
        *info = 0;
        if( dc->ptr.p_int[i]<0 )
        {
            *info = -1;
        }
        if( dc->ptr.p_int[i]>1 )
        {
            *info = -1;
        }
        if( *info<0 )
        {
            ae_frame_leave(_state);
            return;
        }
    }
    
    /*
     * weight decay for correct handling of task which becomes
     * degenerate after constraints are applied
     */
    decay = 10000*ae_machineepsilon;
    
    /*
     * Scale X, Y, XC, YC
     */
    lsfitscalexy(x, y, w, n, xc, yc, dc, k, &xa, &xb, &sa, &sb, &xoriginal, &yoriginal, _state);
    
    /*
     * allocate space, initialize:
     * * FMatrix-   values of basis functions at X[]
     * * CMatrix-   values (derivatives) of basis functions at XC[]
     */
    ae_vector_set_length(&y2, n+m, _state);
    ae_vector_set_length(&w2, n+m, _state);
    ae_matrix_set_length(&fmatrix, n+m, m, _state);
    if( k>0 )
    {
        ae_matrix_set_length(&cmatrix, k, m+1, _state);
    }
    ae_vector_set_length(&y2, n+m, _state);
    ae_vector_set_length(&w2, n+m, _state);
    
    /*
     * Prepare design and constraints matrices:
     * * fill constraints matrix
     * * fill first N rows of design matrix with values
     * * fill next M rows of design matrix with regularizing term
     * * append M zeros to Y
     * * append M elements, mean(abs(W)) each, to W
     */
    ae_vector_set_length(&sx, m, _state);
    ae_vector_set_length(&sy, m, _state);
    ae_vector_set_length(&sbf, m, _state);
    for(j=0; j<=m-1; j++)
    {
        sx.ptr.p_double[j] = (double)(2*j)/(double)(m-1)-1;
    }
    for(i=0; i<=m-1; i++)
    {
        sy.ptr.p_double[i] = (double)(1);
    }
    barycentricbuildfloaterhormann(&sx, &sy, m, d, &b2, _state);
    mx = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        lsfit_barycentriccalcbasis(&b2, x->ptr.p_double[i], &sbf, _state);
        ae_v_move(&fmatrix.ptr.pp_double[i][0], 1, &sbf.ptr.p_double[0], 1, ae_v_len(0,m-1));
        y2.ptr.p_double[i] = y->ptr.p_double[i];
        w2.ptr.p_double[i] = w->ptr.p_double[i];
        mx = mx+ae_fabs(w->ptr.p_double[i], _state)/n;
    }
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=m-1; j++)
        {
            if( i==j )
            {
                fmatrix.ptr.pp_double[n+i][j] = decay;
            }
            else
            {
                fmatrix.ptr.pp_double[n+i][j] = (double)(0);
            }
        }
        y2.ptr.p_double[n+i] = (double)(0);
        w2.ptr.p_double[n+i] = mx;
    }
    if( k>0 )
    {
        for(j=0; j<=m-1; j++)
        {
            for(i=0; i<=m-1; i++)
            {
                sy.ptr.p_double[i] = (double)(0);
            }
            sy.ptr.p_double[j] = (double)(1);
            barycentricbuildfloaterhormann(&sx, &sy, m, d, &b2, _state);
            for(i=0; i<=k-1; i++)
            {
                ae_assert(dc->ptr.p_int[i]>=0&&dc->ptr.p_int[i]<=1, "BarycentricFit: internal error!", _state);
                barycentricdiff1(&b2, xc->ptr.p_double[i], &v0, &v1, _state);
                if( dc->ptr.p_int[i]==0 )
                {
                    cmatrix.ptr.pp_double[i][j] = v0;
                }
                if( dc->ptr.p_int[i]==1 )
                {
                    cmatrix.ptr.pp_double[i][j] = v1;
                }
            }
        }
        for(i=0; i<=k-1; i++)
        {
            cmatrix.ptr.pp_double[i][m] = yc->ptr.p_double[i];
        }
    }
    
    /*
     * Solve constrained task
     */
    if( k>0 )
    {
        
        /*
         * solve using regularization
         */
        lsfitlinearwc(&y2, &w2, &fmatrix, &cmatrix, n+m, m, k, info, &tmp, &lrep, _state);
    }
    else
    {
        
        /*
         * no constraints, no regularization needed
         */
        lsfitlinearwc(y, w, &fmatrix, &cmatrix, n, m, k, info, &tmp, &lrep, _state);
    }
    if( *info<0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Generate interpolant and scale it
     */
    ae_v_move(&sy.ptr.p_double[0], 1, &tmp.ptr.p_double[0], 1, ae_v_len(0,m-1));
    barycentricbuildfloaterhormann(&sx, &sy, m, d, b, _state);
    barycentriclintransx(b, 2/(xb-xa), -(xa+xb)/(xb-xa), _state);
    barycentriclintransy(b, sb-sa, sa, _state);
    
    /*
     * Scale absolute errors obtained from LSFitLinearW.
     * Relative error should be calculated separately
     * (because of shifting/scaling of the task)
     */
    rep->taskrcond = lrep.taskrcond;
    rep->rmserror = lrep.rmserror*(sb-sa);
    rep->avgerror = lrep.avgerror*(sb-sa);
    rep->maxerror = lrep.maxerror*(sb-sa);
    rep->avgrelerror = (double)(0);
    relcnt = 0;
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_neq(yoriginal.ptr.p_double[i],(double)(0)) )
        {
            rep->avgrelerror = rep->avgrelerror+ae_fabs(barycentriccalc(b, xoriginal.ptr.p_double[i], _state)-yoriginal.ptr.p_double[i], _state)/ae_fabs(yoriginal.ptr.p_double[i], _state);
            relcnt = relcnt+1;
        }
    }
    if( relcnt!=0 )
    {
        rep->avgrelerror = rep->avgrelerror/relcnt;
    }
    ae_frame_leave(_state);
}


static void lsfit_clearreport(lsfitreport* rep, ae_state *_state)
{


    rep->taskrcond = (double)(0);
    rep->iterationscount = 0;
    rep->varidx = -1;
    rep->rmserror = (double)(0);
    rep->avgerror = (double)(0);
    rep->avgrelerror = (double)(0);
    rep->maxerror = (double)(0);
    rep->wrmserror = (double)(0);
    rep->r2 = (double)(0);
    ae_matrix_set_length(&rep->covpar, 0, 0, _state);
    ae_vector_set_length(&rep->errpar, 0, _state);
    ae_vector_set_length(&rep->errcurve, 0, _state);
    ae_vector_set_length(&rep->noise, 0, _state);
}


/*************************************************************************
This internal function estimates covariance matrix and other error-related
information for linear/nonlinear least squares model.

It has a bit awkward interface, but it can be used  for  both  linear  and
nonlinear problems.

INPUT PARAMETERS:
    F1  -   array[0..N-1,0..K-1]:
            * for linear problems - matrix of function values
            * for nonlinear problems - Jacobian matrix
    F0  -   array[0..N-1]:
            * for linear problems - must be filled with zeros
            * for nonlinear problems - must store values of function being
              fitted
    Y   -   array[0..N-1]:
            * for linear and nonlinear problems - must store target values
    W   -   weights, array[0..N-1]:
            * for linear and nonlinear problems - weights
    X   -   array[0..K-1]:
            * for linear and nonlinear problems - current solution
    S   -   array[0..K-1]:
            * its components should be strictly positive
            * squared inverse of this diagonal matrix is used as damping
              factor for covariance matrix (linear and nonlinear problems)
            * for nonlinear problems, when scale of the variables is usually
              explicitly given by user, you may use scale vector for this
              parameter
            * for linear problems you may set this parameter to
              S=sqrt(1/diag(F'*F))
            * this parameter is automatically rescaled by this function,
              only relative magnitudes of its components (with respect to
              each other) matter.
    N   -   number of points, N>0.
    K   -   number of dimensions
    Rep -   structure which is used to store results
    Z   -   additional matrix which, depending on ZKind, may contain some
            information used to accelerate calculations - or just can be
            temporary buffer:
            * for ZKind=0       Z contains no information, just temporary
                                buffer which can be resized and used as needed
            * for ZKind=1       Z contains triangular matrix from QR
                                decomposition of W*F1. This matrix can be used
                                to speedup calculation of covariance matrix.
                                It should not be changed by algorithm.
    ZKind-  contents of Z

OUTPUT PARAMETERS:

* Rep.CovPar        covariance matrix for parameters, array[K,K].
* Rep.ErrPar        errors in parameters, array[K],
                    errpar = sqrt(diag(CovPar))
* Rep.ErrCurve      vector of fit errors - standard deviations of empirical
                    best-fit curve from "ideal" best-fit curve built  with
                    infinite number of samples, array[N].
                    errcurve = sqrt(diag(J*CovPar*J')),
                    where J is Jacobian matrix.
* Rep.Noise         vector of per-point estimates of noise, array[N]
* Rep.R2            coefficient of determination (non-weighted)

Other fields of Rep are not changed.

IMPORTANT:  errors  in  parameters  are  calculated  without  taking  into
            account boundary/linear constraints! Presence  of  constraints
            changes distribution of errors, but there is no  easy  way  to
            account for constraints when you calculate covariance matrix.
            
NOTE:       noise in the data is estimated as follows:
            * for fitting without user-supplied  weights  all  points  are
              assumed to have same level of noise, which is estimated from
              the data
            * for fitting with user-supplied weights we assume that  noise
              level in I-th point is inversely proportional to Ith weight.
              Coefficient of proportionality is estimated from the data.
            
NOTE:       we apply small amount of regularization when we invert squared
            Jacobian and calculate covariance matrix. It  guarantees  that
            algorithm won't divide by zero  during  inversion,  but  skews
            error estimates a bit (fractional error is about 10^-9).
            
            However, we believe that this difference is insignificant  for
            all practical purposes except for the situation when you  want
            to compare ALGLIB results with "reference"  implementation  up
            to the last significant digit.

  -- ALGLIB PROJECT --
     Copyright 10.12.2009 by Bochkanov Sergey
*************************************************************************/
static void lsfit_estimateerrors(/* Real    */ ae_matrix* f1,
     /* Real    */ ae_vector* f0,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* w,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* s,
     ae_int_t n,
     ae_int_t k,
     lsfitreport* rep,
     /* Real    */ ae_matrix* z,
     ae_int_t zkind,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _s;
    ae_int_t i;
    ae_int_t j;
    ae_int_t j1;
    double v;
    double noisec;
    ae_int_t info;
    matinvreport invrep;
    ae_int_t nzcnt;
    double avg;
    double rss;
    double tss;
    double sz;
    double ss;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_s, s, _state);
    s = &_s;
    _matinvreport_init(&invrep, _state);

    
    /*
     * Compute NZCnt - count of non-zero weights
     */
    nzcnt = 0;
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_neq(w->ptr.p_double[i],(double)(0)) )
        {
            nzcnt = nzcnt+1;
        }
    }
    
    /*
     * Compute R2
     */
    if( nzcnt>0 )
    {
        avg = 0.0;
        for(i=0; i<=n-1; i++)
        {
            if( ae_fp_neq(w->ptr.p_double[i],(double)(0)) )
            {
                avg = avg+y->ptr.p_double[i];
            }
        }
        avg = avg/nzcnt;
        rss = 0.0;
        tss = 0.0;
        for(i=0; i<=n-1; i++)
        {
            if( ae_fp_neq(w->ptr.p_double[i],(double)(0)) )
            {
                v = ae_v_dotproduct(&f1->ptr.pp_double[i][0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,k-1));
                v = v+f0->ptr.p_double[i];
                rss = rss+ae_sqr(v-y->ptr.p_double[i], _state);
                tss = tss+ae_sqr(y->ptr.p_double[i]-avg, _state);
            }
        }
        if( ae_fp_neq(tss,(double)(0)) )
        {
            rep->r2 = ae_maxreal(1.0-rss/tss, 0.0, _state);
        }
        else
        {
            rep->r2 = 1.0;
        }
    }
    else
    {
        rep->r2 = (double)(0);
    }
    
    /*
     * Compute estimate of proportionality between noise in the data and weights:
     *     NoiseC = mean(per-point-noise*per-point-weight)
     * Noise level (standard deviation) at each point is equal to NoiseC/W[I].
     */
    if( nzcnt>k )
    {
        noisec = 0.0;
        for(i=0; i<=n-1; i++)
        {
            if( ae_fp_neq(w->ptr.p_double[i],(double)(0)) )
            {
                v = ae_v_dotproduct(&f1->ptr.pp_double[i][0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,k-1));
                v = v+f0->ptr.p_double[i];
                noisec = noisec+ae_sqr((v-y->ptr.p_double[i])*w->ptr.p_double[i], _state);
            }
        }
        noisec = ae_sqrt(noisec/(nzcnt-k), _state);
    }
    else
    {
        noisec = 0.0;
    }
    
    /*
     * Two branches on noise level:
     * * NoiseC>0   normal situation
     * * NoiseC=0   degenerate case CovPar is filled by zeros
     */
    rmatrixsetlengthatleast(&rep->covpar, k, k, _state);
    if( ae_fp_greater(noisec,(double)(0)) )
    {
        
        /*
         * Normal situation: non-zero noise level
         */
        ae_assert(zkind==0||zkind==1, "LSFit: internal error in EstimateErrors() function", _state);
        if( zkind==0 )
        {
            
            /*
             * Z contains no additional information which can be used to speed up
             * calculations. We have to calculate covariance matrix on our own:
             * * Compute scaled Jacobian N*J, where N[i,i]=WCur[I]/NoiseC, store in Z
             * * Compute Z'*Z, store in CovPar
             * * Apply moderate regularization to CovPar and compute matrix inverse.
             *   In case inverse failed, increase regularization parameter and try
             *   again.
             */
            rmatrixsetlengthatleast(z, n, k, _state);
            for(i=0; i<=n-1; i++)
            {
                v = w->ptr.p_double[i]/noisec;
                ae_v_moved(&z->ptr.pp_double[i][0], 1, &f1->ptr.pp_double[i][0], 1, ae_v_len(0,k-1), v);
            }
            
            /*
             * Convert S to automatically scaled damped matrix:
             * * calculate SZ - sum of diagonal elements of Z'*Z
             * * calculate SS - sum of diagonal elements of S^(-2)
             * * overwrite S by (SZ/SS)*S^(-2)
             * * now S has approximately same magnitude as giagonal of Z'*Z
             */
            sz = (double)(0);
            for(i=0; i<=n-1; i++)
            {
                for(j=0; j<=k-1; j++)
                {
                    sz = sz+z->ptr.pp_double[i][j]*z->ptr.pp_double[i][j];
                }
            }
            if( ae_fp_eq(sz,(double)(0)) )
            {
                sz = (double)(1);
            }
            ss = (double)(0);
            for(j=0; j<=k-1; j++)
            {
                ss = ss+1/ae_sqr(s->ptr.p_double[j], _state);
            }
            for(j=0; j<=k-1; j++)
            {
                s->ptr.p_double[j] = sz/ss/ae_sqr(s->ptr.p_double[j], _state);
            }
            
            /*
             * Calculate damped inverse inv(Z'*Z+S).
             * We increase damping factor V until Z'*Z become well-conditioned.
             */
            v = 1.0E3*ae_machineepsilon;
            do
            {
                rmatrixsyrk(k, n, 1.0, z, 0, 0, 2, 0.0, &rep->covpar, 0, 0, ae_true, _state);
                for(i=0; i<=k-1; i++)
                {
                    rep->covpar.ptr.pp_double[i][i] = rep->covpar.ptr.pp_double[i][i]+v*s->ptr.p_double[i];
                }
                spdmatrixinverse(&rep->covpar, k, ae_true, &info, &invrep, _state);
                v = 10*v;
            }
            while(info<=0);
            for(i=0; i<=k-1; i++)
            {
                for(j=i+1; j<=k-1; j++)
                {
                    rep->covpar.ptr.pp_double[j][i] = rep->covpar.ptr.pp_double[i][j];
                }
            }
        }
        if( zkind==1 )
        {
            
            /*
             * We can reuse additional information:
             * * Z contains R matrix from QR decomposition of W*F1 
             * * After multiplication by 1/NoiseC we get Z_mod = N*F1, where diag(N)=w[i]/NoiseC
             * * Such triangular Z_mod is a Cholesky factor from decomposition of J'*N'*N*J.
             *   Thus, we can calculate covariance matrix as inverse of the matrix given by
             *   its Cholesky decomposition. It allow us to avoid time-consuming calculation
             *   of J'*N'*N*J in CovPar - complexity is reduced from O(N*K^2) to O(K^3), which
             *   is quite good because K is usually orders of magnitude smaller than N.
             *
             * First, convert S to automatically scaled damped matrix:
             * * calculate SZ - sum of magnitudes of diagonal elements of Z/NoiseC
             * * calculate SS - sum of diagonal elements of S^(-1)
             * * overwrite S by (SZ/SS)*S^(-1)
             * * now S has approximately same magnitude as giagonal of Z'*Z
             */
            sz = (double)(0);
            for(j=0; j<=k-1; j++)
            {
                sz = sz+ae_fabs(z->ptr.pp_double[j][j]/noisec, _state);
            }
            if( ae_fp_eq(sz,(double)(0)) )
            {
                sz = (double)(1);
            }
            ss = (double)(0);
            for(j=0; j<=k-1; j++)
            {
                ss = ss+1/s->ptr.p_double[j];
            }
            for(j=0; j<=k-1; j++)
            {
                s->ptr.p_double[j] = sz/ss/s->ptr.p_double[j];
            }
            
            /*
             * Calculate damped inverse of inv((Z+v*S)'*(Z+v*S))
             * We increase damping factor V until matrix become well-conditioned.
             */
            v = 1.0E3*ae_machineepsilon;
            do
            {
                for(i=0; i<=k-1; i++)
                {
                    for(j=i; j<=k-1; j++)
                    {
                        rep->covpar.ptr.pp_double[i][j] = z->ptr.pp_double[i][j]/noisec;
                    }
                    rep->covpar.ptr.pp_double[i][i] = rep->covpar.ptr.pp_double[i][i]+v*s->ptr.p_double[i];
                }
                spdmatrixcholeskyinverse(&rep->covpar, k, ae_true, &info, &invrep, _state);
                v = 10*v;
            }
            while(info<=0);
            for(i=0; i<=k-1; i++)
            {
                for(j=i+1; j<=k-1; j++)
                {
                    rep->covpar.ptr.pp_double[j][i] = rep->covpar.ptr.pp_double[i][j];
                }
            }
        }
    }
    else
    {
        
        /*
         * Degenerate situation: zero noise level, covariance matrix is zero.
         */
        for(i=0; i<=k-1; i++)
        {
            for(j=0; j<=k-1; j++)
            {
                rep->covpar.ptr.pp_double[j][i] = (double)(0);
            }
        }
    }
    
    /*
     * Estimate erorrs in parameters, curve and per-point noise
     */
    rvectorsetlengthatleast(&rep->errpar, k, _state);
    rvectorsetlengthatleast(&rep->errcurve, n, _state);
    rvectorsetlengthatleast(&rep->noise, n, _state);
    for(i=0; i<=k-1; i++)
    {
        rep->errpar.ptr.p_double[i] = ae_sqrt(rep->covpar.ptr.pp_double[i][i], _state);
    }
    for(i=0; i<=n-1; i++)
    {
        
        /*
         * ErrCurve[I] is sqrt(P[i,i]) where P=J*CovPar*J'
         */
        v = 0.0;
        for(j=0; j<=k-1; j++)
        {
            for(j1=0; j1<=k-1; j1++)
            {
                v = v+f1->ptr.pp_double[i][j]*rep->covpar.ptr.pp_double[j][j1]*f1->ptr.pp_double[i][j1];
            }
        }
        rep->errcurve.ptr.p_double[i] = ae_sqrt(v, _state);
        
        /*
         * Noise[i] is filled using weights and current estimate of noise level
         */
        if( ae_fp_neq(w->ptr.p_double[i],(double)(0)) )
        {
            rep->noise.ptr.p_double[i] = noisec/w->ptr.p_double[i];
        }
        else
        {
            rep->noise.ptr.p_double[i] = (double)(0);
        }
    }
    ae_frame_leave(_state);
}


void _polynomialfitreport_init(void* _p, ae_state *_state)
{
    polynomialfitreport *p = (polynomialfitreport*)_p;
    ae_touch_ptr((void*)p);
}


void _polynomialfitreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    polynomialfitreport *dst = (polynomialfitreport*)_dst;
    polynomialfitreport *src = (polynomialfitreport*)_src;
    dst->taskrcond = src->taskrcond;
    dst->rmserror = src->rmserror;
    dst->avgerror = src->avgerror;
    dst->avgrelerror = src->avgrelerror;
    dst->maxerror = src->maxerror;
}


void _polynomialfitreport_clear(void* _p)
{
    polynomialfitreport *p = (polynomialfitreport*)_p;
    ae_touch_ptr((void*)p);
}


void _polynomialfitreport_destroy(void* _p)
{
    polynomialfitreport *p = (polynomialfitreport*)_p;
    ae_touch_ptr((void*)p);
}


void _barycentricfitreport_init(void* _p, ae_state *_state)
{
    barycentricfitreport *p = (barycentricfitreport*)_p;
    ae_touch_ptr((void*)p);
}


void _barycentricfitreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    barycentricfitreport *dst = (barycentricfitreport*)_dst;
    barycentricfitreport *src = (barycentricfitreport*)_src;
    dst->taskrcond = src->taskrcond;
    dst->dbest = src->dbest;
    dst->rmserror = src->rmserror;
    dst->avgerror = src->avgerror;
    dst->avgrelerror = src->avgrelerror;
    dst->maxerror = src->maxerror;
}


void _barycentricfitreport_clear(void* _p)
{
    barycentricfitreport *p = (barycentricfitreport*)_p;
    ae_touch_ptr((void*)p);
}


void _barycentricfitreport_destroy(void* _p)
{
    barycentricfitreport *p = (barycentricfitreport*)_p;
    ae_touch_ptr((void*)p);
}


void _spline1dfitreport_init(void* _p, ae_state *_state)
{
    spline1dfitreport *p = (spline1dfitreport*)_p;
    ae_touch_ptr((void*)p);
}


void _spline1dfitreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    spline1dfitreport *dst = (spline1dfitreport*)_dst;
    spline1dfitreport *src = (spline1dfitreport*)_src;
    dst->taskrcond = src->taskrcond;
    dst->rmserror = src->rmserror;
    dst->avgerror = src->avgerror;
    dst->avgrelerror = src->avgrelerror;
    dst->maxerror = src->maxerror;
}


void _spline1dfitreport_clear(void* _p)
{
    spline1dfitreport *p = (spline1dfitreport*)_p;
    ae_touch_ptr((void*)p);
}


void _spline1dfitreport_destroy(void* _p)
{
    spline1dfitreport *p = (spline1dfitreport*)_p;
    ae_touch_ptr((void*)p);
}


void _lsfitreport_init(void* _p, ae_state *_state)
{
    lsfitreport *p = (lsfitreport*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_init(&p->covpar, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->errpar, 0, DT_REAL, _state);
    ae_vector_init(&p->errcurve, 0, DT_REAL, _state);
    ae_vector_init(&p->noise, 0, DT_REAL, _state);
}


void _lsfitreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    lsfitreport *dst = (lsfitreport*)_dst;
    lsfitreport *src = (lsfitreport*)_src;
    dst->taskrcond = src->taskrcond;
    dst->iterationscount = src->iterationscount;
    dst->varidx = src->varidx;
    dst->rmserror = src->rmserror;
    dst->avgerror = src->avgerror;
    dst->avgrelerror = src->avgrelerror;
    dst->maxerror = src->maxerror;
    dst->wrmserror = src->wrmserror;
    ae_matrix_init_copy(&dst->covpar, &src->covpar, _state);
    ae_vector_init_copy(&dst->errpar, &src->errpar, _state);
    ae_vector_init_copy(&dst->errcurve, &src->errcurve, _state);
    ae_vector_init_copy(&dst->noise, &src->noise, _state);
    dst->r2 = src->r2;
}


void _lsfitreport_clear(void* _p)
{
    lsfitreport *p = (lsfitreport*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_clear(&p->covpar);
    ae_vector_clear(&p->errpar);
    ae_vector_clear(&p->errcurve);
    ae_vector_clear(&p->noise);
}


void _lsfitreport_destroy(void* _p)
{
    lsfitreport *p = (lsfitreport*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_destroy(&p->covpar);
    ae_vector_destroy(&p->errpar);
    ae_vector_destroy(&p->errcurve);
    ae_vector_destroy(&p->noise);
}


void _lsfitstate_init(void* _p, ae_state *_state)
{
    lsfitstate *p = (lsfitstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->s, 0, DT_REAL, _state);
    ae_vector_init(&p->bndl, 0, DT_REAL, _state);
    ae_vector_init(&p->bndu, 0, DT_REAL, _state);
    ae_matrix_init(&p->taskx, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->tasky, 0, DT_REAL, _state);
    ae_vector_init(&p->taskw, 0, DT_REAL, _state);
    ae_vector_init(&p->x, 0, DT_REAL, _state);
    ae_vector_init(&p->c, 0, DT_REAL, _state);
    ae_vector_init(&p->g, 0, DT_REAL, _state);
    ae_matrix_init(&p->h, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->wcur, 0, DT_REAL, _state);
    ae_vector_init(&p->tmp, 0, DT_REAL, _state);
    ae_vector_init(&p->tmpf, 0, DT_REAL, _state);
    ae_matrix_init(&p->tmpjac, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->tmpjacw, 0, 0, DT_REAL, _state);
    _matinvreport_init(&p->invrep, _state);
    _lsfitreport_init(&p->rep, _state);
    _minlmstate_init(&p->optstate, _state);
    _minlmreport_init(&p->optrep, _state);
    _rcommstate_init(&p->rstate, _state);
}


void _lsfitstate_init_copy(void* _dst, void* _src, ae_state *_state)
{
    lsfitstate *dst = (lsfitstate*)_dst;
    lsfitstate *src = (lsfitstate*)_src;
    dst->optalgo = src->optalgo;
    dst->m = src->m;
    dst->k = src->k;
    dst->epsf = src->epsf;
    dst->epsx = src->epsx;
    dst->maxits = src->maxits;
    dst->stpmax = src->stpmax;
    dst->xrep = src->xrep;
    ae_vector_init_copy(&dst->s, &src->s, _state);
    ae_vector_init_copy(&dst->bndl, &src->bndl, _state);
    ae_vector_init_copy(&dst->bndu, &src->bndu, _state);
    ae_matrix_init_copy(&dst->taskx, &src->taskx, _state);
    ae_vector_init_copy(&dst->tasky, &src->tasky, _state);
    dst->npoints = src->npoints;
    ae_vector_init_copy(&dst->taskw, &src->taskw, _state);
    dst->nweights = src->nweights;
    dst->wkind = src->wkind;
    dst->wits = src->wits;
    dst->diffstep = src->diffstep;
    dst->teststep = src->teststep;
    dst->xupdated = src->xupdated;
    dst->needf = src->needf;
    dst->needfg = src->needfg;
    dst->needfgh = src->needfgh;
    dst->pointindex = src->pointindex;
    ae_vector_init_copy(&dst->x, &src->x, _state);
    ae_vector_init_copy(&dst->c, &src->c, _state);
    dst->f = src->f;
    ae_vector_init_copy(&dst->g, &src->g, _state);
    ae_matrix_init_copy(&dst->h, &src->h, _state);
    ae_vector_init_copy(&dst->wcur, &src->wcur, _state);
    ae_vector_init_copy(&dst->tmp, &src->tmp, _state);
    ae_vector_init_copy(&dst->tmpf, &src->tmpf, _state);
    ae_matrix_init_copy(&dst->tmpjac, &src->tmpjac, _state);
    ae_matrix_init_copy(&dst->tmpjacw, &src->tmpjacw, _state);
    dst->tmpnoise = src->tmpnoise;
    _matinvreport_init_copy(&dst->invrep, &src->invrep, _state);
    dst->repiterationscount = src->repiterationscount;
    dst->repterminationtype = src->repterminationtype;
    dst->repvaridx = src->repvaridx;
    dst->reprmserror = src->reprmserror;
    dst->repavgerror = src->repavgerror;
    dst->repavgrelerror = src->repavgrelerror;
    dst->repmaxerror = src->repmaxerror;
    dst->repwrmserror = src->repwrmserror;
    _lsfitreport_init_copy(&dst->rep, &src->rep, _state);
    _minlmstate_init_copy(&dst->optstate, &src->optstate, _state);
    _minlmreport_init_copy(&dst->optrep, &src->optrep, _state);
    dst->prevnpt = src->prevnpt;
    dst->prevalgo = src->prevalgo;
    _rcommstate_init_copy(&dst->rstate, &src->rstate, _state);
}


void _lsfitstate_clear(void* _p)
{
    lsfitstate *p = (lsfitstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->s);
    ae_vector_clear(&p->bndl);
    ae_vector_clear(&p->bndu);
    ae_matrix_clear(&p->taskx);
    ae_vector_clear(&p->tasky);
    ae_vector_clear(&p->taskw);
    ae_vector_clear(&p->x);
    ae_vector_clear(&p->c);
    ae_vector_clear(&p->g);
    ae_matrix_clear(&p->h);
    ae_vector_clear(&p->wcur);
    ae_vector_clear(&p->tmp);
    ae_vector_clear(&p->tmpf);
    ae_matrix_clear(&p->tmpjac);
    ae_matrix_clear(&p->tmpjacw);
    _matinvreport_clear(&p->invrep);
    _lsfitreport_clear(&p->rep);
    _minlmstate_clear(&p->optstate);
    _minlmreport_clear(&p->optrep);
    _rcommstate_clear(&p->rstate);
}


void _lsfitstate_destroy(void* _p)
{
    lsfitstate *p = (lsfitstate*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->s);
    ae_vector_destroy(&p->bndl);
    ae_vector_destroy(&p->bndu);
    ae_matrix_destroy(&p->taskx);
    ae_vector_destroy(&p->tasky);
    ae_vector_destroy(&p->taskw);
    ae_vector_destroy(&p->x);
    ae_vector_destroy(&p->c);
    ae_vector_destroy(&p->g);
    ae_matrix_destroy(&p->h);
    ae_vector_destroy(&p->wcur);
    ae_vector_destroy(&p->tmp);
    ae_vector_destroy(&p->tmpf);
    ae_matrix_destroy(&p->tmpjac);
    ae_matrix_destroy(&p->tmpjacw);
    _matinvreport_destroy(&p->invrep);
    _lsfitreport_destroy(&p->rep);
    _minlmstate_destroy(&p->optstate);
    _minlmreport_destroy(&p->optrep);
    _rcommstate_destroy(&p->rstate);
}


/*$ End $*/
