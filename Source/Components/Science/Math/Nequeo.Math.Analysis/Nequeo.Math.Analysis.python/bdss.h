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

#ifndef _bdss_h
#define _bdss_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "tsort.h"
#include "basicstatops.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"
#include "basestat.h"


/*$ Declarations $*/


typedef struct
{
    double relclserror;
    double avgce;
    double rmserror;
    double avgerror;
    double avgrelerror;
} cvreport;


/*$ Body $*/


/*************************************************************************
This set of routines (DSErrAllocate, DSErrAccumulate, DSErrFinish)
calculates different error functions (classification error, cross-entropy,
rms, avg, avg.rel errors).

1. DSErrAllocate prepares buffer.
2. DSErrAccumulate accumulates individual errors:
    * Y contains predicted output (posterior probabilities for classification)
    * DesiredY contains desired output (class number for classification)
3. DSErrFinish outputs results:
   * Buf[0] contains relative classification error (zero for regression tasks)
   * Buf[1] contains avg. cross-entropy (zero for regression tasks)
   * Buf[2] contains rms error (regression, classification)
   * Buf[3] contains average error (regression, classification)
   * Buf[4] contains average relative error (regression, classification)
   
NOTES(1):
    "NClasses>0" means that we have classification task.
    "NClasses<0" means regression task with -NClasses real outputs.

NOTES(2):
    rms. avg, avg.rel errors for classification tasks are interpreted as
    errors in posterior probabilities with respect to probabilities given
    by training/test set.

  -- ALGLIB --
     Copyright 11.01.2009 by Bochkanov Sergey
*************************************************************************/
void dserrallocate(ae_int_t nclasses,
     /* Real    */ ae_vector* buf,
     ae_state *_state);


/*************************************************************************
See DSErrAllocate for comments on this routine.

  -- ALGLIB --
     Copyright 11.01.2009 by Bochkanov Sergey
*************************************************************************/
void dserraccumulate(/* Real    */ ae_vector* buf,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* desiredy,
     ae_state *_state);


/*************************************************************************
See DSErrAllocate for comments on this routine.

  -- ALGLIB --
     Copyright 11.01.2009 by Bochkanov Sergey
*************************************************************************/
void dserrfinish(/* Real    */ ae_vector* buf, ae_state *_state);


/*************************************************************************

  -- ALGLIB --
     Copyright 19.05.2008 by Bochkanov Sergey
*************************************************************************/
void dsnormalize(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t* info,
     /* Real    */ ae_vector* means,
     /* Real    */ ae_vector* sigmas,
     ae_state *_state);


/*************************************************************************

  -- ALGLIB --
     Copyright 19.05.2008 by Bochkanov Sergey
*************************************************************************/
void dsnormalizec(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t* info,
     /* Real    */ ae_vector* means,
     /* Real    */ ae_vector* sigmas,
     ae_state *_state);


/*************************************************************************

  -- ALGLIB --
     Copyright 19.05.2008 by Bochkanov Sergey
*************************************************************************/
double dsgetmeanmindistance(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_state *_state);


/*************************************************************************

  -- ALGLIB --
     Copyright 19.05.2008 by Bochkanov Sergey
*************************************************************************/
void dstie(/* Real    */ ae_vector* a,
     ae_int_t n,
     /* Integer */ ae_vector* ties,
     ae_int_t* tiecount,
     /* Integer */ ae_vector* p1,
     /* Integer */ ae_vector* p2,
     ae_state *_state);


/*************************************************************************

  -- ALGLIB --
     Copyright 11.12.2008 by Bochkanov Sergey
*************************************************************************/
void dstiefasti(/* Real    */ ae_vector* a,
     /* Integer */ ae_vector* b,
     ae_int_t n,
     /* Integer */ ae_vector* ties,
     ae_int_t* tiecount,
     /* Real    */ ae_vector* bufr,
     /* Integer */ ae_vector* bufi,
     ae_state *_state);


/*************************************************************************
Optimal binary classification

Algorithms finds optimal (=with minimal cross-entropy) binary partition.
Internal subroutine.

INPUT PARAMETERS:
    A       -   array[0..N-1], variable
    C       -   array[0..N-1], class numbers (0 or 1).
    N       -   array size

OUTPUT PARAMETERS:
    Info    -   completetion code:
                * -3, all values of A[] are same (partition is impossible)
                * -2, one of C[] is incorrect (<0, >1)
                * -1, incorrect pararemets were passed (N<=0).
                *  1, OK
    Threshold-  partiton boundary. Left part contains values which are
                strictly less than Threshold. Right part contains values
                which are greater than or equal to Threshold.
    PAL, PBL-   probabilities P(0|v<Threshold) and P(1|v<Threshold)
    PAR, PBR-   probabilities P(0|v>=Threshold) and P(1|v>=Threshold)
    CVE     -   cross-validation estimate of cross-entropy

  -- ALGLIB --
     Copyright 22.05.2008 by Bochkanov Sergey
*************************************************************************/
void dsoptimalsplit2(/* Real    */ ae_vector* a,
     /* Integer */ ae_vector* c,
     ae_int_t n,
     ae_int_t* info,
     double* threshold,
     double* pal,
     double* pbl,
     double* par,
     double* pbr,
     double* cve,
     ae_state *_state);


/*************************************************************************
Optimal partition, internal subroutine. Fast version.

Accepts:
    A       array[0..N-1]       array of attributes     array[0..N-1]
    C       array[0..N-1]       array of class labels
    TiesBuf array[0..N]         temporaries (ties)
    CntBuf  array[0..2*NC-1]    temporaries (counts)
    Alpha                       centering factor (0<=alpha<=1, recommended value - 0.05)
    BufR    array[0..N-1]       temporaries
    BufI    array[0..N-1]       temporaries

Output:
    Info    error code (">0"=OK, "<0"=bad)
    RMS     training set RMS error
    CVRMS   leave-one-out RMS error
    
Note:
    content of all arrays is changed by subroutine;
    it doesn't allocate temporaries.

  -- ALGLIB --
     Copyright 11.12.2008 by Bochkanov Sergey
*************************************************************************/
void dsoptimalsplit2fast(/* Real    */ ae_vector* a,
     /* Integer */ ae_vector* c,
     /* Integer */ ae_vector* tiesbuf,
     /* Integer */ ae_vector* cntbuf,
     /* Real    */ ae_vector* bufr,
     /* Integer */ ae_vector* bufi,
     ae_int_t n,
     ae_int_t nc,
     double alpha,
     ae_int_t* info,
     double* threshold,
     double* rms,
     double* cvrms,
     ae_state *_state);


/*************************************************************************
Automatic non-optimal discretization, internal subroutine.

  -- ALGLIB --
     Copyright 22.05.2008 by Bochkanov Sergey
*************************************************************************/
void dssplitk(/* Real    */ ae_vector* a,
     /* Integer */ ae_vector* c,
     ae_int_t n,
     ae_int_t nc,
     ae_int_t kmax,
     ae_int_t* info,
     /* Real    */ ae_vector* thresholds,
     ae_int_t* ni,
     double* cve,
     ae_state *_state);


/*************************************************************************
Automatic optimal discretization, internal subroutine.

  -- ALGLIB --
     Copyright 22.05.2008 by Bochkanov Sergey
*************************************************************************/
void dsoptimalsplitk(/* Real    */ ae_vector* a,
     /* Integer */ ae_vector* c,
     ae_int_t n,
     ae_int_t nc,
     ae_int_t kmax,
     ae_int_t* info,
     /* Real    */ ae_vector* thresholds,
     ae_int_t* ni,
     double* cve,
     ae_state *_state);
void _cvreport_init(void* _p, ae_state *_state);
void _cvreport_init_copy(void* _dst, void* _src, ae_state *_state);
void _cvreport_clear(void* _p);
void _cvreport_destroy(void* _p);


/*$ End $*/
#endif

