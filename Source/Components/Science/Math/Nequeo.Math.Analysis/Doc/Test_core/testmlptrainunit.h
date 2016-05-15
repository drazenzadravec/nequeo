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

#ifndef _testmlptrainunit_h
#define _testmlptrainunit_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "tsort.h"
#include "basicstatops.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"
#include "basestat.h"
#include "bdss.h"
#include "hpccores.h"
#include "scodes.h"
#include "hqrnd.h"
#include "sparse.h"
#include "mlpbase.h"
#include "mlpe.h"
#include "reflections.h"
#include "creflections.h"
#include "matgen.h"
#include "rotations.h"
#include "trfac.h"
#include "trlinsolve.h"
#include "safesolve.h"
#include "rcond.h"
#include "matinv.h"
#include "linmin.h"
#include "hblas.h"
#include "sblas.h"
#include "ortfac.h"
#include "blas.h"
#include "bdsvd.h"
#include "svd.h"
#include "optserv.h"
#include "fbls.h"
#include "minlbfgs.h"
#include "xblas.h"
#include "densesolver.h"
#include "mlptrain.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool testmlptrain(ae_bool silent, ae_state *_state);
ae_bool _pexec_testmlptrain(ae_bool silent, ae_state *_state);


/*$ End $*/
#endif

