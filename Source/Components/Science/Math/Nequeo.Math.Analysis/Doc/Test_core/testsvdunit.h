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

#ifndef _testsvdunit_h
#define _testsvdunit_h

#include "aenv.h"
#include "ialglib.h"
#include "hblas.h"
#include "apserv.h"
#include "reflections.h"
#include "creflections.h"
#include "sblas.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"
#include "ortfac.h"
#include "blas.h"
#include "rotations.h"
#include "hqrnd.h"
#include "bdsvd.h"
#include "svd.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Testing SVD decomposition subroutine
*************************************************************************/
ae_bool testsvd(ae_bool silent, ae_state *_state);
ae_bool _pexec_testsvd(ae_bool silent, ae_state *_state);


/*$ End $*/
#endif

