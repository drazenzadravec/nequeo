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

#ifndef _lsfit_h
#define _lsfit_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "tsort.h"
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


/*$ Declarations $*/


/*************************************************************************
Polynomial fitting report:
    TaskRCond       reciprocal of task's condition number
    RMSError        RMS error
    AvgError        average error
    AvgRelError     average relative error (for non-zero Y[I])
    MaxError        maximum error
*************************************************************************/
typedef struct
{
    double taskrcond;
    double rmserror;
    double avgerror;
    double avgrelerror;
    double maxerror;
} polynomialfitreport;


/*************************************************************************
Barycentric fitting report:
    RMSError        RMS error
    AvgError        average error
    AvgRelError     average relative error (for non-zero Y[I])
    MaxError        maximum error
    TaskRCond       reciprocal of task's condition number
*************************************************************************/
typedef struct
{
    double taskrcond;
    ae_int_t dbest;
    double rmserror;
    double avgerror;
    double avgrelerror;
    double maxerror;
} barycentricfitreport;


/*************************************************************************
Spline fitting report:
    RMSError        RMS error
    AvgError        average error
    AvgRelError     average relative error (for non-zero Y[I])
    MaxError        maximum error
    
Fields  below are  filled  by   obsolete    functions   (Spline1DFitCubic,
Spline1DFitHermite). Modern fitting functions do NOT fill these fields:
    TaskRCond       reciprocal of task's condition number
*************************************************************************/
typedef struct
{
    double taskrcond;
    double rmserror;
    double avgerror;
    double avgrelerror;
    double maxerror;
} spline1dfitreport;


/*************************************************************************
Least squares fitting report. This structure contains informational fields
which are set by fitting functions provided by this unit.

Different functions initialize different sets of  fields,  so  you  should
read documentation on specific function you used in order  to  know  which
fields are initialized.

    TaskRCond       reciprocal of task's condition number
    IterationsCount number of internal iterations
    
    VarIdx          if user-supplied gradient contains errors  which  were
                    detected by nonlinear fitter, this  field  is  set  to
                    index  of  the  first  component  of gradient which is
                    suspected to be spoiled by bugs.

    RMSError        RMS error
    AvgError        average error
    AvgRelError     average relative error (for non-zero Y[I])
    MaxError        maximum error

    WRMSError       weighted RMS error

    CovPar          covariance matrix for parameters, filled by some solvers
    ErrPar          vector of errors in parameters, filled by some solvers
    ErrCurve        vector of fit errors -  variability  of  the  best-fit
                    curve, filled by some solvers.
    Noise           vector of per-point noise estimates, filled by
                    some solvers.
    R2              coefficient of determination (non-weighted, non-adjusted),
                    filled by some solvers.
*************************************************************************/
typedef struct
{
    double taskrcond;
    ae_int_t iterationscount;
    ae_int_t varidx;
    double rmserror;
    double avgerror;
    double avgrelerror;
    double maxerror;
    double wrmserror;
    ae_matrix covpar;
    ae_vector errpar;
    ae_vector errcurve;
    ae_vector noise;
    double r2;
} lsfitreport;


/*************************************************************************
Nonlinear fitter.

You should use ALGLIB functions to work with fitter.
Never try to access its fields directly!
*************************************************************************/
typedef struct
{
    ae_int_t optalgo;
    ae_int_t m;
    ae_int_t k;
    double epsf;
    double epsx;
    ae_int_t maxits;
    double stpmax;
    ae_bool xrep;
    ae_vector s;
    ae_vector bndl;
    ae_vector bndu;
    ae_matrix taskx;
    ae_vector tasky;
    ae_int_t npoints;
    ae_vector taskw;
    ae_int_t nweights;
    ae_int_t wkind;
    ae_int_t wits;
    double diffstep;
    double teststep;
    ae_bool xupdated;
    ae_bool needf;
    ae_bool needfg;
    ae_bool needfgh;
    ae_int_t pointindex;
    ae_vector x;
    ae_vector c;
    double f;
    ae_vector g;
    ae_matrix h;
    ae_vector wcur;
    ae_vector tmp;
    ae_vector tmpf;
    ae_matrix tmpjac;
    ae_matrix tmpjacw;
    double tmpnoise;
    matinvreport invrep;
    ae_int_t repiterationscount;
    ae_int_t repterminationtype;
    ae_int_t repvaridx;
    double reprmserror;
    double repavgerror;
    double repavgrelerror;
    double repmaxerror;
    double repwrmserror;
    lsfitreport rep;
    minlmstate optstate;
    minlmreport optrep;
    ae_int_t prevnpt;
    ae_int_t prevalgo;
    rcommstate rstate;
} lsfitstate;


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);
void _pexec_polynomialfit(/* Real    */ ae_vector* x,
    /* Real    */ ae_vector* y,
    ae_int_t n,
    ae_int_t m,
    ae_int_t* info,
    barycentricinterpolant* p,
    polynomialfitreport* rep, ae_state *_state);


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
     ae_state *_state);
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
    polynomialfitreport* rep, ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);
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
    barycentricfitreport* rep, ae_state *_state);


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
     ae_state *_state);
void _pexec_barycentricfitfloaterhormann(/* Real    */ ae_vector* x,
    /* Real    */ ae_vector* y,
    ae_int_t n,
    ae_int_t m,
    ae_int_t* info,
    barycentricinterpolant* b,
    barycentricfitreport* rep, ae_state *_state);


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
     ae_state *_state);
void _pexec_spline1dfitpenalized(/* Real    */ ae_vector* x,
    /* Real    */ ae_vector* y,
    ae_int_t n,
    ae_int_t m,
    double rho,
    ae_int_t* info,
    spline1dinterpolant* s,
    spline1dfitreport* rep, ae_state *_state);


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
     ae_state *_state);
void _pexec_spline1dfitpenalizedw(/* Real    */ ae_vector* x,
    /* Real    */ ae_vector* y,
    /* Real    */ ae_vector* w,
    ae_int_t n,
    ae_int_t m,
    double rho,
    ae_int_t* info,
    spline1dinterpolant* s,
    spline1dfitreport* rep, ae_state *_state);


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
     ae_state *_state);
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
    spline1dfitreport* rep, ae_state *_state);


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
     ae_state *_state);
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
    spline1dfitreport* rep, ae_state *_state);


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
     ae_state *_state);
void _pexec_spline1dfitcubic(/* Real    */ ae_vector* x,
    /* Real    */ ae_vector* y,
    ae_int_t n,
    ae_int_t m,
    ae_int_t* info,
    spline1dinterpolant* s,
    spline1dfitreport* rep, ae_state *_state);


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
     ae_state *_state);
void _pexec_spline1dfithermite(/* Real    */ ae_vector* x,
    /* Real    */ ae_vector* y,
    ae_int_t n,
    ae_int_t m,
    ae_int_t* info,
    spline1dinterpolant* s,
    spline1dfitreport* rep, ae_state *_state);


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
     ae_state *_state);
void _pexec_lsfitlinearw(/* Real    */ ae_vector* y,
    /* Real    */ ae_vector* w,
    /* Real    */ ae_matrix* fmatrix,
    ae_int_t n,
    ae_int_t m,
    ae_int_t* info,
    /* Real    */ ae_vector* c,
    lsfitreport* rep, ae_state *_state);


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
     ae_state *_state);
void _pexec_lsfitlinearwc(/* Real    */ ae_vector* y,
    /* Real    */ ae_vector* w,
    /* Real    */ ae_matrix* fmatrix,
    /* Real    */ ae_matrix* cmatrix,
    ae_int_t n,
    ae_int_t m,
    ae_int_t k,
    ae_int_t* info,
    /* Real    */ ae_vector* c,
    lsfitreport* rep, ae_state *_state);


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
     ae_state *_state);
void _pexec_lsfitlinear(/* Real    */ ae_vector* y,
    /* Real    */ ae_matrix* fmatrix,
    ae_int_t n,
    ae_int_t m,
    ae_int_t* info,
    /* Real    */ ae_vector* c,
    lsfitreport* rep, ae_state *_state);


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
     ae_state *_state);
void _pexec_lsfitlinearc(/* Real    */ ae_vector* y,
    /* Real    */ ae_matrix* fmatrix,
    /* Real    */ ae_matrix* cmatrix,
    ae_int_t n,
    ae_int_t m,
    ae_int_t k,
    ae_int_t* info,
    /* Real    */ ae_vector* c,
    lsfitreport* rep, ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
void lsfitsetstpmax(lsfitstate* state, double stpmax, ae_state *_state);


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
void lsfitsetxrep(lsfitstate* state, ae_bool needxrep, ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
ae_bool lsfititeration(lsfitstate* state, ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);
void _polynomialfitreport_init(void* _p, ae_state *_state);
void _polynomialfitreport_init_copy(void* _dst, void* _src, ae_state *_state);
void _polynomialfitreport_clear(void* _p);
void _polynomialfitreport_destroy(void* _p);
void _barycentricfitreport_init(void* _p, ae_state *_state);
void _barycentricfitreport_init_copy(void* _dst, void* _src, ae_state *_state);
void _barycentricfitreport_clear(void* _p);
void _barycentricfitreport_destroy(void* _p);
void _spline1dfitreport_init(void* _p, ae_state *_state);
void _spline1dfitreport_init_copy(void* _dst, void* _src, ae_state *_state);
void _spline1dfitreport_clear(void* _p);
void _spline1dfitreport_destroy(void* _p);
void _lsfitreport_init(void* _p, ae_state *_state);
void _lsfitreport_init_copy(void* _dst, void* _src, ae_state *_state);
void _lsfitreport_clear(void* _p);
void _lsfitreport_destroy(void* _p);
void _lsfitstate_init(void* _p, ae_state *_state);
void _lsfitstate_init_copy(void* _dst, void* _src, ae_state *_state);
void _lsfitstate_clear(void* _p);
void _lsfitstate_destroy(void* _p);


/*$ End $*/
#endif

