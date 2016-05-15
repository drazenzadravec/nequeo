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

#ifndef _testspdgevdunit_h
#define _testspdgevdunit_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "reflections.h"
#include "creflections.h"
#include "hqrnd.h"
#include "matgen.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"
#include "tsort.h"
#include "sparse.h"
#include "rotations.h"
#include "trfac.h"
#include "sblas.h"
#include "blas.h"
#include "trlinsolve.h"
#include "safesolve.h"
#include "rcond.h"
#include "matinv.h"
#include "hblas.h"
#include "ortfac.h"
#include "hsschur.h"
#include "evd.h"
#include "spdgevd.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Testing bidiagonal SVD decomposition subroutine
*************************************************************************/
ae_bool testspdgevd(ae_bool silent, ae_state *_state);
ae_bool _pexec_testspdgevd(ae_bool silent, ae_state *_state);


/*$ End $*/
#endif

