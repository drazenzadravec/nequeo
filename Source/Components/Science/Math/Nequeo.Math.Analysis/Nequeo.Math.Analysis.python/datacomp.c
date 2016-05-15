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


#include "stdafx.h"
#include "datacomp.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
k-means++ clusterization.
Backward compatibility function, we recommend to use CLUSTERING subpackage
as better replacement.

  -- ALGLIB --
     Copyright 21.03.2009 by Bochkanov Sergey
*************************************************************************/
void kmeansgenerate(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t k,
     ae_int_t restarts,
     ae_int_t* info,
     /* Real    */ ae_matrix* c,
     /* Integer */ ae_vector* xyc,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix dummy;
    ae_int_t itscnt;
    double e;
    kmeansbuffers buf;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    ae_matrix_clear(c);
    ae_vector_clear(xyc);
    ae_matrix_init(&dummy, 0, 0, DT_REAL, _state);
    _kmeansbuffers_init(&buf, _state);

    kmeansinitbuf(&buf, _state);
    kmeansgenerateinternal(xy, npoints, nvars, k, 0, 0, restarts, ae_false, info, &itscnt, c, ae_true, &dummy, ae_false, xyc, &e, &buf, _state);
    ae_frame_leave(_state);
}


/*$ End $*/
