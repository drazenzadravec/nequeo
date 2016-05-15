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

#ifndef _qqpsolver_h
#define _qqpsolver_h

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
#include "hblas.h"
#include "sblas.h"
#include "ortfac.h"
#include "fbls.h"
#include "cqmodels.h"
#include "blas.h"
#include "bdsvd.h"
#include "svd.h"
#include "optserv.h"
#include "snnls.h"
#include "sactivesets.h"


/*$ Declarations $*/


/*************************************************************************
This object stores settings for QQP solver.
It must be initialized with QQPLoadDefaults().
After initialization you may change settings.
*************************************************************************/
typedef struct
{
    double epsg;
    double epsf;
    double epsx;
    ae_int_t maxouterits;
    ae_bool cgphase;
    ae_bool cnphase;
    ae_int_t cgminits;
    ae_int_t cgmaxits;
    ae_int_t cnmaxupdates;
    ae_int_t sparsesolver;
} qqpsettings;


/*************************************************************************
This object stores temporaries used by QuickQP solver.
*************************************************************************/
typedef struct
{
    ae_int_t n;
    ae_int_t nmain;
    ae_int_t nslack;
    ae_int_t nec;
    ae_int_t nic;
    ae_int_t akind;
    ae_matrix densea;
    sparsematrix sparsea;
    ae_bool sparseupper;
    double absamax;
    double absasum;
    double absasum2;
    ae_vector b;
    ae_vector bndl;
    ae_vector bndu;
    ae_vector havebndl;
    ae_vector havebndu;
    ae_matrix cleic;
    ae_vector xs;
    ae_vector gc;
    ae_vector xp;
    ae_vector dc;
    ae_vector dp;
    ae_vector cgc;
    ae_vector cgp;
    sactiveset sas;
    ae_vector activated;
    ae_int_t nfree;
    ae_int_t cnmodelage;
    ae_matrix densez;
    sparsematrix sparsecca;
    ae_vector yidx;
    ae_vector regdiag;
    ae_vector regx0;
    ae_vector tmpcn;
    ae_vector tmpcni;
    ae_vector tmpcnb;
    ae_vector tmp0;
    ae_vector stpbuf;
    sparsebuffers sbuf;
    ae_int_t repinneriterationscount;
    ae_int_t repouteriterationscount;
    ae_int_t repncholesky;
    ae_int_t repncupdates;
} qqpbuffers;


/*$ Body $*/


/*************************************************************************
This function initializes QQPSettings structure with default settings.

Newly created structure MUST be initialized by default settings  -  or  by
copy of the already initialized structure.

  -- ALGLIB --
     Copyright 14.05.2011 by Bochkanov Sergey
*************************************************************************/
void qqploaddefaults(ae_int_t nmain, qqpsettings* s, ae_state *_state);


/*************************************************************************
This function initializes QQPSettings  structure  with  copy  of  another,
already initialized structure.

  -- ALGLIB --
     Copyright 14.05.2011 by Bochkanov Sergey
*************************************************************************/
void qqpcopysettings(qqpsettings* src, qqpsettings* dst, ae_state *_state);


/*************************************************************************
This function runs QQP solver; it returns after optimization  process  was
completed. Following QP problem is solved:

    min(0.5*(x-x_origin)'*A*(x-x_origin)+b'*(x-x_origin))
    
subject to boundary constraints.

IMPORTANT: UNLIKE MANY OTHER SOLVERS, THIS FUNCTION DOES NOT  REQUIRE  YOU
           TO INITIALIZE STATE OBJECT. IT CAN BE AUTOMATICALLY INITIALIZED
           DURING SOLUTION PROCESS.

INPUT PARAMETERS:
    AC          -   for dense problems (AKind=0) A-term of CQM object
                    contains system matrix. Other terms are unspecified
                    and should not be referenced.
    SparseAC    -   for sparse problems (AKind=1
    AKind       -   sparse matrix format:
                    * 0 for dense matrix
                    * 1 for sparse matrix
    SparseUpper -   which triangle of SparseAC stores matrix  -  upper  or
                    lower one (for dense matrices this  parameter  is  not
                    actual).
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
    Settings    -   QQPSettings object initialized by one of the initialization
                    functions.
    SState      -   object which stores temporaries:
                    * uninitialized object is automatically initialized
                    * previously allocated memory is reused as much
                      as possible
    XS          -   initial point, array[NC]
    
    
OUTPUT PARAMETERS:
    XS          -   last point
    TerminationType-termination type:
                    *
                    *
                    *

  -- ALGLIB --
     Copyright 14.05.2011 by Bochkanov Sergey
*************************************************************************/
void qqpoptimize(convexquadraticmodel* ac,
     sparsematrix* sparseac,
     ae_int_t akind,
     ae_bool sparseupper,
     /* Real    */ ae_vector* bc,
     /* Real    */ ae_vector* bndlc,
     /* Real    */ ae_vector* bnduc,
     /* Real    */ ae_vector* sc,
     /* Real    */ ae_vector* xoriginc,
     ae_int_t nc,
     /* Real    */ ae_matrix* cleicc,
     ae_int_t nec,
     ae_int_t nic,
     qqpsettings* settings,
     qqpbuffers* sstate,
     /* Real    */ ae_vector* xs,
     ae_int_t* terminationtype,
     ae_state *_state);
void _qqpsettings_init(void* _p, ae_state *_state);
void _qqpsettings_init_copy(void* _dst, void* _src, ae_state *_state);
void _qqpsettings_clear(void* _p);
void _qqpsettings_destroy(void* _p);
void _qqpbuffers_init(void* _p, ae_state *_state);
void _qqpbuffers_init_copy(void* _dst, void* _src, ae_state *_state);
void _qqpbuffers_clear(void* _p);
void _qqpbuffers_destroy(void* _p);


/*$ End $*/
#endif

