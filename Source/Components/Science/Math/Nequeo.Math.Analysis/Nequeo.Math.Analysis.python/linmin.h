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

#ifndef _linmin_h
#define _linmin_h

#include "aenv.h"
#include "ialglib.h"


/*$ Declarations $*/


typedef struct
{
    ae_bool brackt;
    ae_bool stage1;
    ae_int_t infoc;
    double dg;
    double dgm;
    double dginit;
    double dgtest;
    double dgx;
    double dgxm;
    double dgy;
    double dgym;
    double finit;
    double ftest1;
    double fm;
    double fx;
    double fxm;
    double fy;
    double fym;
    double stx;
    double sty;
    double stmin;
    double stmax;
    double width;
    double width1;
    double xtrapf;
} linminstate;


typedef struct
{
    ae_bool needf;
    ae_vector x;
    double f;
    ae_int_t n;
    ae_vector xbase;
    ae_vector s;
    double stplen;
    double fcur;
    double stpmax;
    ae_int_t fmax;
    ae_int_t nfev;
    ae_int_t info;
    rcommstate rstate;
} armijostate;


/*$ Body $*/


/*************************************************************************
Normalizes direction/step pair: makes |D|=1, scales Stp.
If |D|=0, it returns, leavind D/Stp unchanged.

  -- ALGLIB --
     Copyright 01.04.2010 by Bochkanov Sergey
*************************************************************************/
void linminnormalized(/* Real    */ ae_vector* d,
     double* stp,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
THE  PURPOSE  OF  MCSRCH  IS  TO  FIND A STEP WHICH SATISFIES A SUFFICIENT
DECREASE CONDITION AND A CURVATURE CONDITION.

AT EACH STAGE THE SUBROUTINE  UPDATES  AN  INTERVAL  OF  UNCERTAINTY  WITH
ENDPOINTS  STX  AND  STY.  THE INTERVAL OF UNCERTAINTY IS INITIALLY CHOSEN
SO THAT IT CONTAINS A MINIMIZER OF THE MODIFIED FUNCTION

    F(X+STP*S) - F(X) - FTOL*STP*(GRADF(X)'S).

IF  A STEP  IS OBTAINED FOR  WHICH THE MODIFIED FUNCTION HAS A NONPOSITIVE
FUNCTION  VALUE  AND  NONNEGATIVE  DERIVATIVE,   THEN   THE   INTERVAL  OF
UNCERTAINTY IS CHOSEN SO THAT IT CONTAINS A MINIMIZER OF F(X+STP*S).

THE  ALGORITHM  IS  DESIGNED TO FIND A STEP WHICH SATISFIES THE SUFFICIENT
DECREASE CONDITION

    F(X+STP*S) .LE. F(X) + FTOL*STP*(GRADF(X)'S),

AND THE CURVATURE CONDITION

    ABS(GRADF(X+STP*S)'S)) .LE. GTOL*ABS(GRADF(X)'S).

IF  FTOL  IS  LESS  THAN GTOL AND IF, FOR EXAMPLE, THE FUNCTION IS BOUNDED
BELOW,  THEN  THERE  IS  ALWAYS  A  STEP  WHICH SATISFIES BOTH CONDITIONS.
IF  NO  STEP  CAN BE FOUND  WHICH  SATISFIES  BOTH  CONDITIONS,  THEN  THE
ALGORITHM  USUALLY STOPS  WHEN  ROUNDING ERRORS  PREVENT FURTHER PROGRESS.
IN THIS CASE STP ONLY SATISFIES THE SUFFICIENT DECREASE CONDITION.


:::::::::::::IMPORTANT NOTES:::::::::::::

NOTE 1:

This routine  guarantees that it will stop at the last point where function
value was calculated. It won't make several additional function evaluations
after finding good point. So if you store function evaluations requested by
this routine, you can be sure that last one is the point where we've stopped.

NOTE 2:

when 0<StpMax<StpMin, algorithm will terminate with INFO=5 and Stp=StpMax

NOTE 3:

this algorithm guarantees that, if MCINFO=1 or MCINFO=5, then:
* F(final_point)<F(initial_point) - strict inequality
* final_point<>initial_point - after rounding to machine precision
:::::::::::::::::::::::::::::::::::::::::


PARAMETERS DESCRIPRION

STAGE IS ZERO ON FIRST CALL, ZERO ON FINAL EXIT

N IS A POSITIVE INTEGER INPUT VARIABLE SET TO THE NUMBER OF VARIABLES.

X IS  AN  ARRAY  OF  LENGTH N. ON INPUT IT MUST CONTAIN THE BASE POINT FOR
THE LINE SEARCH. ON OUTPUT IT CONTAINS X+STP*S.

F IS  A  VARIABLE. ON INPUT IT MUST CONTAIN THE VALUE OF F AT X. ON OUTPUT
IT CONTAINS THE VALUE OF F AT X + STP*S.

G IS AN ARRAY OF LENGTH N. ON INPUT IT MUST CONTAIN THE GRADIENT OF F AT X.
ON OUTPUT IT CONTAINS THE GRADIENT OF F AT X + STP*S.

S IS AN INPUT ARRAY OF LENGTH N WHICH SPECIFIES THE SEARCH DIRECTION.

STP  IS  A NONNEGATIVE VARIABLE. ON INPUT STP CONTAINS AN INITIAL ESTIMATE
OF A SATISFACTORY STEP. ON OUTPUT STP CONTAINS THE FINAL ESTIMATE.

FTOL AND GTOL ARE NONNEGATIVE INPUT VARIABLES. TERMINATION OCCURS WHEN THE
SUFFICIENT DECREASE CONDITION AND THE DIRECTIONAL DERIVATIVE CONDITION ARE
SATISFIED.

XTOL IS A NONNEGATIVE INPUT VARIABLE. TERMINATION OCCURS WHEN THE RELATIVE
WIDTH OF THE INTERVAL OF UNCERTAINTY IS AT MOST XTOL.

STPMIN AND STPMAX ARE NONNEGATIVE INPUT VARIABLES WHICH SPECIFY LOWER  AND
UPPER BOUNDS FOR THE STEP.

MAXFEV IS A POSITIVE INTEGER INPUT VARIABLE. TERMINATION OCCURS WHEN THE
NUMBER OF CALLS TO FCN IS AT LEAST MAXFEV BY THE END OF AN ITERATION.

INFO IS AN INTEGER OUTPUT VARIABLE SET AS FOLLOWS:
    INFO = 0  IMPROPER INPUT PARAMETERS.

    INFO = 1  THE SUFFICIENT DECREASE CONDITION AND THE
              DIRECTIONAL DERIVATIVE CONDITION HOLD.

    INFO = 2  RELATIVE WIDTH OF THE INTERVAL OF UNCERTAINTY
              IS AT MOST XTOL.

    INFO = 3  NUMBER OF CALLS TO FCN HAS REACHED MAXFEV.

    INFO = 4  THE STEP IS AT THE LOWER BOUND STPMIN.

    INFO = 5  THE STEP IS AT THE UPPER BOUND STPMAX.

    INFO = 6  ROUNDING ERRORS PREVENT FURTHER PROGRESS.
              THERE MAY NOT BE A STEP WHICH SATISFIES THE
              SUFFICIENT DECREASE AND CURVATURE CONDITIONS.
              TOLERANCES MAY BE TOO SMALL.

NFEV IS AN INTEGER OUTPUT VARIABLE SET TO THE NUMBER OF CALLS TO FCN.

WA IS A WORK ARRAY OF LENGTH N.

ARGONNE NATIONAL LABORATORY. MINPACK PROJECT. JUNE 1983
JORGE J. MORE', DAVID J. THUENTE
*************************************************************************/
void mcsrch(ae_int_t n,
     /* Real    */ ae_vector* x,
     double* f,
     /* Real    */ ae_vector* g,
     /* Real    */ ae_vector* s,
     double* stp,
     double stpmax,
     double gtol,
     ae_int_t* info,
     ae_int_t* nfev,
     /* Real    */ ae_vector* wa,
     linminstate* state,
     ae_int_t* stage,
     ae_state *_state);


/*************************************************************************
These functions perform Armijo line search using  at  most  FMAX  function
evaluations.  It  doesn't  enforce  some  kind  of  " sufficient decrease"
criterion - it just tries different Armijo steps and returns optimum found
so far.

Optimization is done using F-rcomm interface:
* ArmijoCreate initializes State structure
  (reusing previously allocated buffers)
* ArmijoIteration is subsequently called
* ArmijoResults returns results

INPUT PARAMETERS:
    N       -   problem size
    X       -   array[N], starting point
    F       -   F(X+S*STP)
    S       -   step direction, S>0
    STP     -   step length
    STPMAX  -   maximum value for STP or zero (if no limit is imposed)
    FMAX    -   maximum number of function evaluations
    State   -   optimization state

  -- ALGLIB --
     Copyright 05.10.2010 by Bochkanov Sergey
*************************************************************************/
void armijocreate(ae_int_t n,
     /* Real    */ ae_vector* x,
     double f,
     /* Real    */ ae_vector* s,
     double stp,
     double stpmax,
     ae_int_t fmax,
     armijostate* state,
     ae_state *_state);


/*************************************************************************
This is rcomm-based search function

  -- ALGLIB --
     Copyright 05.10.2010 by Bochkanov Sergey
*************************************************************************/
ae_bool armijoiteration(armijostate* state, ae_state *_state);


/*************************************************************************
Results of Armijo search

OUTPUT PARAMETERS:
    INFO    -   on output it is set to one of the return codes:
                * 0     improper input params
                * 1     optimum step is found with at most FMAX evaluations
                * 3     FMAX evaluations were used,
                        X contains optimum found so far
                * 4     step is at lower bound STPMIN
                * 5     step is at upper bound
    STP     -   step length (in case of failure it is still returned)
    F       -   function value (in case of failure it is still returned)

  -- ALGLIB --
     Copyright 05.10.2010 by Bochkanov Sergey
*************************************************************************/
void armijoresults(armijostate* state,
     ae_int_t* info,
     double* stp,
     double* f,
     ae_state *_state);
void _linminstate_init(void* _p, ae_state *_state);
void _linminstate_init_copy(void* _dst, void* _src, ae_state *_state);
void _linminstate_clear(void* _p);
void _linminstate_destroy(void* _p);
void _armijostate_init(void* _p, ae_state *_state);
void _armijostate_init_copy(void* _dst, void* _src, ae_state *_state);
void _armijostate_clear(void* _p);
void _armijostate_destroy(void* _p);


/*$ End $*/
#endif

