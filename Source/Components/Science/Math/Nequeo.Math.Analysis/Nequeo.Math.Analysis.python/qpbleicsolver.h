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

#ifndef _qpbleicsolver_h
#define _qpbleicsolver_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "hqrnd.h"
#include "tsort.h"
#include "sparse.h"
#include "reflections.h"
#include "creflections.h"
#include "matgen.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"
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
#include "cqmodels.h"
#include "snnls.h"
#include "sactivesets.h"
#include "mincg.h"
#include "xblas.h"
#include "densesolver.h"
#include "minbleic.h"


/*$ Declarations $*/


/*************************************************************************
This object stores settings for QPBLEIC solver.
It must be initialized with QPBLEICLoadDefaults().
After initialization you may change settings.
*************************************************************************/
typedef struct
{
    double epsg;
    double epsf;
    double epsx;
    ae_int_t maxits;
} qpbleicsettings;


/*************************************************************************
This object stores temporaries used by QuickQP solver.
*************************************************************************/
typedef struct
{
    minbleicstate solver;
    minbleicreport solverrep;
    ae_vector tmp0;
    ae_vector tmp1;
    ae_vector tmpi;
    ae_int_t repinneriterationscount;
    ae_int_t repouteriterationscount;
} qpbleicbuffers;


/*$ Body $*/


/*************************************************************************
This function initializes QPBLEICSettings structure with default settings.

Newly created structure MUST be initialized by default settings  -  or  by
copy of the already initialized structure.

  -- ALGLIB --
     Copyright 14.05.2011 by Bochkanov Sergey
*************************************************************************/
void qpbleicloaddefaults(ae_int_t nmain,
     qpbleicsettings* s,
     ae_state *_state);


/*************************************************************************
This function initializes QPBLEICSettings  structure  with  copy  of  another,
already initialized structure.

  -- ALGLIB --
     Copyright 14.05.2011 by Bochkanov Sergey
*************************************************************************/
void qpbleiccopysettings(qpbleicsettings* src,
     qpbleicsettings* dst,
     ae_state *_state);


/*************************************************************************
This function runs QPBLEIC solver; it returns after optimization   process
was completed. Following QP problem is solved:

    min(0.5*(x-x_origin)'*A*(x-x_origin)+b'*(x-x_origin))
    
subject to boundary constraints.

INPUT PARAMETERS:
    AC          -   for dense problems (AKind=0), A-term of CQM object
                    contains system matrix. Other terms are unspecified
                    and should not be referenced.
    SparseAC    -   for sparse problems (AKind=1
    AKind       -   sparse matrix format:
                    * 0 for dense matrix
                    * 1 for sparse matrix
    SparseUpper -   which triangle of SparseAC stores matrix  -  upper  or
                    lower one (for dense matrices this  parameter  is  not
                    actual).
    AbsASum     -   SUM(|A[i,j]|)
    AbsASum2    -   SUM(A[i,j]^2)
    BC          -   linear term, array[NC]
    BndLC       -   lower bound, array[NC]
    BndUC       -   upper bound, array[NC]
    SC          -   scale vector, array[NC]:
                    * I-th element contains scale of I-th variable,
                    * SC[I]>0
    XOriginC    -   origin term, array[NC]. Can be zero.
    NC          -   number of variables in the  original  formulation  (no
                    slack variables).
    CLEICC      -   linear equality/inequality constraints. Present version
                    of this function does NOT provide  publicly  available
                    support for linear constraints. This feature  will  be
                    introduced in the future versions of the function.
    NEC, NIC    -   number of equality/inequality constraints.
                    MUST BE ZERO IN THE CURRENT VERSION!!!
    Settings    -   QPBLEICSettings object initialized by one of the initialization
                    functions.
    SState      -   object which stores temporaries:
                    * if uninitialized object was passed, FirstCall parameter MUST
                      be set to True; object will be automatically initialized by the
                      function, and FirstCall will be set to False.
                    * if FirstCall=False, it is assumed that this parameter was already
                      initialized by previous call to this function with same
                      problem dimensions (variable count N).
    FirstCall   -   whether it is first call of this function for this specific
                    instance of SState, with this number of variables N specified.
    XS          -   initial point, array[NC]
    
    
OUTPUT PARAMETERS:
    XS          -   last point
    FirstCall   -   uncondtionally set to False
    TerminationType-termination type:
                    *
                    *
                    *

  -- ALGLIB --
     Copyright 14.05.2011 by Bochkanov Sergey
*************************************************************************/
void qpbleicoptimize(convexquadraticmodel* a,
     sparsematrix* sparsea,
     ae_int_t akind,
     ae_bool sparseaupper,
     double absasum,
     double absasum2,
     /* Real    */ ae_vector* b,
     /* Real    */ ae_vector* bndl,
     /* Real    */ ae_vector* bndu,
     /* Real    */ ae_vector* s,
     /* Real    */ ae_vector* xorigin,
     ae_int_t n,
     /* Real    */ ae_matrix* cleic,
     ae_int_t nec,
     ae_int_t nic,
     qpbleicsettings* settings,
     qpbleicbuffers* sstate,
     ae_bool* firstcall,
     /* Real    */ ae_vector* xs,
     ae_int_t* terminationtype,
     ae_state *_state);
void _qpbleicsettings_init(void* _p, ae_state *_state);
void _qpbleicsettings_init_copy(void* _dst, void* _src, ae_state *_state);
void _qpbleicsettings_clear(void* _p);
void _qpbleicsettings_destroy(void* _p);
void _qpbleicbuffers_init(void* _p, ae_state *_state);
void _qpbleicbuffers_init_copy(void* _dst, void* _src, ae_state *_state);
void _qpbleicbuffers_clear(void* _p);
void _qpbleicbuffers_destroy(void* _p);


/*$ End $*/
#endif

