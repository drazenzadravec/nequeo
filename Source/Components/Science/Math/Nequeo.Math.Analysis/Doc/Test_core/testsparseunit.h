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

#ifndef _testsparseunit_h
#define _testsparseunit_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "hqrnd.h"
#include "tsort.h"
#include "sparse.h"


/*$ Declarations $*/


typedef struct
{
    ae_int_t n;
    ae_int_t m;
    ae_int_t matkind;
    ae_int_t triangle;
    ae_matrix bufa;
    hqrndstate rs;
    rcommstate rcs;
} sparsegenerator;


/*$ Body $*/


ae_bool testsparse(ae_bool silent, ae_state *_state);
ae_bool _pexec_testsparse(ae_bool silent, ae_state *_state);


/*************************************************************************
Function for testing basic SKS functional.
Returns True on errors, False on success.

  -- ALGLIB PROJECT --
     Copyright 16.01.1014 by Bochkanov Sergey
*************************************************************************/
ae_bool skstest(ae_state *_state);


/*************************************************************************
Function for testing basic functional

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool basicfunctest(ae_state *_state);


/*************************************************************************
Function for testing Level 2 unsymmetric linear algebra functions.
Additionally it tests SparseGet() for several matrix formats.
Returns True on failure.

  -- ALGLIB PROJECT --
     Copyright 20.01.2014 by Bochkanov Sergey
*************************************************************************/
ae_bool testlevel2unsymmetric(ae_state *_state);


/*************************************************************************
Function for testing Level 3 unsymmetric linear algebra functions.
Additionally it tests SparseGet() for several matrix formats.
Returns True on failure.

  -- ALGLIB PROJECT --
     Copyright 20.01.2014 by Bochkanov Sergey
*************************************************************************/
ae_bool testlevel3unsymmetric(ae_state *_state);


/*************************************************************************
Function for testing Level 2 symmetric linear algebra functions.
Additionally it tests SparseGet() for several matrix formats.
Returns True on failure.

  -- ALGLIB PROJECT --
     Copyright 20.01.2014 by Bochkanov Sergey
*************************************************************************/
ae_bool testlevel2symmetric(ae_state *_state);


/*************************************************************************
Function for testing Level 2 symmetric linear algebra functions.
Additionally it tests SparseGet() for several matrix formats.
Returns True on failure.

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool testlevel3symmetric(ae_state *_state);


/*************************************************************************
Function for testing Level 2 triangular linear algebra functions.
Returns True on failure.

  -- ALGLIB PROJECT --
     Copyright 20.01.2014 by Bochkanov Sergey
*************************************************************************/
ae_bool testlevel2triangular(ae_state *_state);


/*************************************************************************
Function for testing basic functional

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool basicfuncrandomtest(ae_state *_state);


/*************************************************************************
Function for testing multyplication matrix with vector

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool linearfunctionstest(ae_state *_state);


/*************************************************************************
Function for testing multyplication for simmetric matrix with vector

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool linearfunctionsstest(ae_state *_state);


/*************************************************************************
Function for testing multyplication sparse matrix with nerrow dense matrix

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool linearfunctionsmmtest(ae_state *_state);


/*************************************************************************
Function for testing multyplication for simmetric sparse matrix with narrow
dense matrix

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool linearfunctionssmmtest(ae_state *_state);


/*************************************************************************
Function for basic test SparseCopy

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool basiccopyfunctest(ae_bool silent, ae_state *_state);


/*************************************************************************
Function for testing SparseCopy

  -- ALGLIB PROJECT --
     Copyright 14.10.2011 by Bochkanov Sergey
*************************************************************************/
ae_bool copyfunctest(ae_bool silent, ae_state *_state);
void _sparsegenerator_init(void* _p, ae_state *_state);
void _sparsegenerator_init_copy(void* _dst, void* _src, ae_state *_state);
void _sparsegenerator_clear(void* _p);
void _sparsegenerator_destroy(void* _p);


/*$ End $*/
#endif

