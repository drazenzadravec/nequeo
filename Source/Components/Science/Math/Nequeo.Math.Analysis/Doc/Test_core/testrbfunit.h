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

#ifndef _testrbfunit_h
#define _testrbfunit_h

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
#include "rbf.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool testrbf(ae_bool silent, ae_state *_state);
ae_bool _pexec_testrbf(ae_bool silent, ae_state *_state);


/*************************************************************************
The test  has  to  check, that  algorithm can solve problems of matrix are
degenerate.
    * used model with linear term;
    * points locate in a subspace of dimension less than an original space.

  -- ALGLIB --
     Copyright 13.12.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool sqrdegmatrixrbftest(ae_bool silent, ae_state *_state);


/*************************************************************************
Function for testing basic functionality of RBF module on regular grids with
multi-layer algorithm in 1D.

  -- ALGLIB --
     Copyright 2.03.2012 by Bochkanov Sergey
*************************************************************************/
ae_bool basicmultilayerrbf1dtest(ae_state *_state);


/*$ End $*/
#endif

