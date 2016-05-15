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

#ifndef _testminqpunit_h
#define _testminqpunit_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "reflections.h"
#include "creflections.h"
#include "hqrnd.h"
#include "matgen.h"
#include "tsort.h"
#include "sparse.h"
#include "normestimator.h"
#include "hblas.h"
#include "sblas.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"
#include "ortfac.h"
#include "blas.h"
#include "rotations.h"
#include "bdsvd.h"
#include "svd.h"
#include "trfac.h"
#include "trlinsolve.h"
#include "safesolve.h"
#include "rcond.h"
#include "xblas.h"
#include "densesolver.h"
#include "matinv.h"
#include "fbls.h"
#include "cqmodels.h"
#include "optserv.h"
#include "snnls.h"
#include "sactivesets.h"
#include "qqpsolver.h"
#include "linmin.h"
#include "mincg.h"
#include "minbleic.h"
#include "qpbleicsolver.h"
#include "qpcholeskysolver.h"
#include "minqp.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool testminqp(ae_bool silent, ae_state *_state);
ae_bool _pexec_testminqp(ae_bool silent, ae_state *_state);


/*************************************************************************
Function to test: 'MinQPCreate', 'MinQPSetQuadraticTerm', 'MinQPSetBC', 
'MinQPSetOrigin', 'MinQPSetStartingPoint', 'MinQPOptimize', 'MinQPResults'.

Test problem:
    A = diag(aii), aii>0 (random)
    b = 0
    random bounds (either no bounds, one bound, two bounds a<b, two bounds a=b)
    random start point
    dimension - from 1 to 5.
    
Returns True on success, False on failure.
*************************************************************************/
ae_bool simpletest(ae_state *_state);


/*************************************************************************
Function to test: 'MinQPCreate', 'MinQPSetLinearTerm', 'MinQPSetQuadraticTerm',
'MinQPSetOrigin', 'MinQPSetStartingPoint', 'MinQPOptimize', 'MinQPResults'.

Test problem:
    A = positive-definite matrix, obtained by 'SPDMatrixRndCond' function
    b <> 0
    without bounds
    random start point
    dimension - from 1 to 5.
*************************************************************************/
ae_bool functest1(ae_state *_state);


/*************************************************************************
Function to test: 'MinQPCreate', 'MinQPSetLinearTerm', 'MinQPSetQuadraticTerm',
'MinQPSetBC', 'MinQPSetOrigin', 'MinQPSetStartingPoint', 'MinQPOptimize', 
'MinQPResults'.

Test problem:
    A = positive-definite matrix, obtained by 'SPDMatrixRndCond' function
    b <> 0
    boundary constraints
    random start point
    dimension - from 1 to 5.
*************************************************************************/
ae_bool functest2(ae_state *_state);


/*************************************************************************
ConsoleTest.
*************************************************************************/
ae_bool consoletest(ae_state *_state);


/*************************************************************************
This function performs tests specific for Cholesky solver
    
Returns True on success, False on failure.
*************************************************************************/
ae_bool choleskytests(ae_state *_state);


/*************************************************************************
This function performs tests specific for QuickQP solver
    
Returns True on failure.
*************************************************************************/
ae_bool quickqptests(ae_state *_state);


/*************************************************************************
This function performs tests specific for BLEIC solver
    
Returns True on error, False on success.
*************************************************************************/
ae_bool bleictests(ae_state *_state);


/*$ End $*/
#endif

