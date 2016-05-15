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
#include "bdss.h"


/*$ Declarations $*/
static double bdss_xlny(double x, double y, ae_state *_state);
static double bdss_getcv(/* Integer */ ae_vector* cnt,
     ae_int_t nc,
     ae_state *_state);
static void bdss_tieaddc(/* Integer */ ae_vector* c,
     /* Integer */ ae_vector* ties,
     ae_int_t ntie,
     ae_int_t nc,
     /* Integer */ ae_vector* cnt,
     ae_state *_state);
static void bdss_tiesubc(/* Integer */ ae_vector* c,
     /* Integer */ ae_vector* ties,
     ae_int_t ntie,
     ae_int_t nc,
     /* Integer */ ae_vector* cnt,
     ae_state *_state);


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
     ae_state *_state)
{

    ae_vector_clear(buf);

    ae_vector_set_length(buf, 7+1, _state);
    buf->ptr.p_double[0] = (double)(0);
    buf->ptr.p_double[1] = (double)(0);
    buf->ptr.p_double[2] = (double)(0);
    buf->ptr.p_double[3] = (double)(0);
    buf->ptr.p_double[4] = (double)(0);
    buf->ptr.p_double[5] = (double)(nclasses);
    buf->ptr.p_double[6] = (double)(0);
    buf->ptr.p_double[7] = (double)(0);
}


/*************************************************************************
See DSErrAllocate for comments on this routine.

  -- ALGLIB --
     Copyright 11.01.2009 by Bochkanov Sergey
*************************************************************************/
void dserraccumulate(/* Real    */ ae_vector* buf,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* desiredy,
     ae_state *_state)
{
    ae_int_t nclasses;
    ae_int_t nout;
    ae_int_t offs;
    ae_int_t mmax;
    ae_int_t rmax;
    ae_int_t j;
    double v;
    double ev;


    offs = 5;
    nclasses = ae_round(buf->ptr.p_double[offs], _state);
    if( nclasses>0 )
    {
        
        /*
         * Classification
         */
        rmax = ae_round(desiredy->ptr.p_double[0], _state);
        mmax = 0;
        for(j=1; j<=nclasses-1; j++)
        {
            if( ae_fp_greater(y->ptr.p_double[j],y->ptr.p_double[mmax]) )
            {
                mmax = j;
            }
        }
        if( mmax!=rmax )
        {
            buf->ptr.p_double[0] = buf->ptr.p_double[0]+1;
        }
        if( ae_fp_greater(y->ptr.p_double[rmax],(double)(0)) )
        {
            buf->ptr.p_double[1] = buf->ptr.p_double[1]-ae_log(y->ptr.p_double[rmax], _state);
        }
        else
        {
            buf->ptr.p_double[1] = buf->ptr.p_double[1]+ae_log(ae_maxrealnumber, _state);
        }
        for(j=0; j<=nclasses-1; j++)
        {
            v = y->ptr.p_double[j];
            if( j==rmax )
            {
                ev = (double)(1);
            }
            else
            {
                ev = (double)(0);
            }
            buf->ptr.p_double[2] = buf->ptr.p_double[2]+ae_sqr(v-ev, _state);
            buf->ptr.p_double[3] = buf->ptr.p_double[3]+ae_fabs(v-ev, _state);
            if( ae_fp_neq(ev,(double)(0)) )
            {
                buf->ptr.p_double[4] = buf->ptr.p_double[4]+ae_fabs((v-ev)/ev, _state);
                buf->ptr.p_double[offs+2] = buf->ptr.p_double[offs+2]+1;
            }
        }
        buf->ptr.p_double[offs+1] = buf->ptr.p_double[offs+1]+1;
    }
    else
    {
        
        /*
         * Regression
         */
        nout = -nclasses;
        rmax = 0;
        for(j=1; j<=nout-1; j++)
        {
            if( ae_fp_greater(desiredy->ptr.p_double[j],desiredy->ptr.p_double[rmax]) )
            {
                rmax = j;
            }
        }
        mmax = 0;
        for(j=1; j<=nout-1; j++)
        {
            if( ae_fp_greater(y->ptr.p_double[j],y->ptr.p_double[mmax]) )
            {
                mmax = j;
            }
        }
        if( mmax!=rmax )
        {
            buf->ptr.p_double[0] = buf->ptr.p_double[0]+1;
        }
        for(j=0; j<=nout-1; j++)
        {
            v = y->ptr.p_double[j];
            ev = desiredy->ptr.p_double[j];
            buf->ptr.p_double[2] = buf->ptr.p_double[2]+ae_sqr(v-ev, _state);
            buf->ptr.p_double[3] = buf->ptr.p_double[3]+ae_fabs(v-ev, _state);
            if( ae_fp_neq(ev,(double)(0)) )
            {
                buf->ptr.p_double[4] = buf->ptr.p_double[4]+ae_fabs((v-ev)/ev, _state);
                buf->ptr.p_double[offs+2] = buf->ptr.p_double[offs+2]+1;
            }
        }
        buf->ptr.p_double[offs+1] = buf->ptr.p_double[offs+1]+1;
    }
}


/*************************************************************************
See DSErrAllocate for comments on this routine.

  -- ALGLIB --
     Copyright 11.01.2009 by Bochkanov Sergey
*************************************************************************/
void dserrfinish(/* Real    */ ae_vector* buf, ae_state *_state)
{
    ae_int_t nout;
    ae_int_t offs;


    offs = 5;
    nout = ae_iabs(ae_round(buf->ptr.p_double[offs], _state), _state);
    if( ae_fp_neq(buf->ptr.p_double[offs+1],(double)(0)) )
    {
        buf->ptr.p_double[0] = buf->ptr.p_double[0]/buf->ptr.p_double[offs+1];
        buf->ptr.p_double[1] = buf->ptr.p_double[1]/buf->ptr.p_double[offs+1];
        buf->ptr.p_double[2] = ae_sqrt(buf->ptr.p_double[2]/(nout*buf->ptr.p_double[offs+1]), _state);
        buf->ptr.p_double[3] = buf->ptr.p_double[3]/(nout*buf->ptr.p_double[offs+1]);
    }
    if( ae_fp_neq(buf->ptr.p_double[offs+2],(double)(0)) )
    {
        buf->ptr.p_double[4] = buf->ptr.p_double[4]/buf->ptr.p_double[offs+2];
    }
}


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
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_vector tmp;
    double mean;
    double variance;
    double skewness;
    double kurtosis;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    ae_vector_clear(means);
    ae_vector_clear(sigmas);
    ae_vector_init(&tmp, 0, DT_REAL, _state);

    
    /*
     * Test parameters
     */
    if( npoints<=0||nvars<1 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    *info = 1;
    
    /*
     * Standartization
     */
    ae_vector_set_length(means, nvars-1+1, _state);
    ae_vector_set_length(sigmas, nvars-1+1, _state);
    ae_vector_set_length(&tmp, npoints-1+1, _state);
    for(j=0; j<=nvars-1; j++)
    {
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][j], xy->stride, ae_v_len(0,npoints-1));
        samplemoments(&tmp, npoints, &mean, &variance, &skewness, &kurtosis, _state);
        means->ptr.p_double[j] = mean;
        sigmas->ptr.p_double[j] = ae_sqrt(variance, _state);
        if( ae_fp_eq(sigmas->ptr.p_double[j],(double)(0)) )
        {
            sigmas->ptr.p_double[j] = (double)(1);
        }
        for(i=0; i<=npoints-1; i++)
        {
            xy->ptr.pp_double[i][j] = (xy->ptr.pp_double[i][j]-means->ptr.p_double[j])/sigmas->ptr.p_double[j];
        }
    }
    ae_frame_leave(_state);
}


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
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t j;
    ae_vector tmp;
    double mean;
    double variance;
    double skewness;
    double kurtosis;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    ae_vector_clear(means);
    ae_vector_clear(sigmas);
    ae_vector_init(&tmp, 0, DT_REAL, _state);

    
    /*
     * Test parameters
     */
    if( npoints<=0||nvars<1 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    *info = 1;
    
    /*
     * Standartization
     */
    ae_vector_set_length(means, nvars-1+1, _state);
    ae_vector_set_length(sigmas, nvars-1+1, _state);
    ae_vector_set_length(&tmp, npoints-1+1, _state);
    for(j=0; j<=nvars-1; j++)
    {
        ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[0][j], xy->stride, ae_v_len(0,npoints-1));
        samplemoments(&tmp, npoints, &mean, &variance, &skewness, &kurtosis, _state);
        means->ptr.p_double[j] = mean;
        sigmas->ptr.p_double[j] = ae_sqrt(variance, _state);
        if( ae_fp_eq(sigmas->ptr.p_double[j],(double)(0)) )
        {
            sigmas->ptr.p_double[j] = (double)(1);
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************

  -- ALGLIB --
     Copyright 19.05.2008 by Bochkanov Sergey
*************************************************************************/
double dsgetmeanmindistance(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_vector tmp;
    ae_vector tmp2;
    double v;
    double result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    ae_vector_init(&tmp2, 0, DT_REAL, _state);

    
    /*
     * Test parameters
     */
    if( npoints<=0||nvars<1 )
    {
        result = (double)(0);
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Process
     */
    ae_vector_set_length(&tmp, npoints-1+1, _state);
    for(i=0; i<=npoints-1; i++)
    {
        tmp.ptr.p_double[i] = ae_maxrealnumber;
    }
    ae_vector_set_length(&tmp2, nvars-1+1, _state);
    for(i=0; i<=npoints-1; i++)
    {
        for(j=i+1; j<=npoints-1; j++)
        {
            ae_v_move(&tmp2.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
            ae_v_sub(&tmp2.ptr.p_double[0], 1, &xy->ptr.pp_double[j][0], 1, ae_v_len(0,nvars-1));
            v = ae_v_dotproduct(&tmp2.ptr.p_double[0], 1, &tmp2.ptr.p_double[0], 1, ae_v_len(0,nvars-1));
            v = ae_sqrt(v, _state);
            tmp.ptr.p_double[i] = ae_minreal(tmp.ptr.p_double[i], v, _state);
            tmp.ptr.p_double[j] = ae_minreal(tmp.ptr.p_double[j], v, _state);
        }
    }
    result = (double)(0);
    for(i=0; i<=npoints-1; i++)
    {
        result = result+tmp.ptr.p_double[i]/npoints;
    }
    ae_frame_leave(_state);
    return result;
}


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
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t k;
    ae_vector tmp;

    ae_frame_make(_state, &_frame_block);
    ae_vector_clear(ties);
    *tiecount = 0;
    ae_vector_clear(p1);
    ae_vector_clear(p2);
    ae_vector_init(&tmp, 0, DT_INT, _state);

    
    /*
     * Special case
     */
    if( n<=0 )
    {
        *tiecount = 0;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Sort A
     */
    tagsort(a, n, p1, p2, _state);
    
    /*
     * Process ties
     */
    *tiecount = 1;
    for(i=1; i<=n-1; i++)
    {
        if( ae_fp_neq(a->ptr.p_double[i],a->ptr.p_double[i-1]) )
        {
            *tiecount = *tiecount+1;
        }
    }
    ae_vector_set_length(ties, *tiecount+1, _state);
    ties->ptr.p_int[0] = 0;
    k = 1;
    for(i=1; i<=n-1; i++)
    {
        if( ae_fp_neq(a->ptr.p_double[i],a->ptr.p_double[i-1]) )
        {
            ties->ptr.p_int[k] = i;
            k = k+1;
        }
    }
    ties->ptr.p_int[*tiecount] = n;
    ae_frame_leave(_state);
}


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
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t k;
    ae_vector tmp;

    ae_frame_make(_state, &_frame_block);
    *tiecount = 0;
    ae_vector_init(&tmp, 0, DT_INT, _state);

    
    /*
     * Special case
     */
    if( n<=0 )
    {
        *tiecount = 0;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Sort A
     */
    tagsortfasti(a, b, bufr, bufi, n, _state);
    
    /*
     * Process ties
     */
    ties->ptr.p_int[0] = 0;
    k = 1;
    for(i=1; i<=n-1; i++)
    {
        if( ae_fp_neq(a->ptr.p_double[i],a->ptr.p_double[i-1]) )
        {
            ties->ptr.p_int[k] = i;
            k = k+1;
        }
    }
    ties->ptr.p_int[k] = n;
    *tiecount = k;
    ae_frame_leave(_state);
}


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
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _a;
    ae_vector _c;
    ae_int_t i;
    ae_int_t t;
    double s;
    ae_vector ties;
    ae_int_t tiecount;
    ae_vector p1;
    ae_vector p2;
    ae_int_t k;
    ae_int_t koptimal;
    double pak;
    double pbk;
    double cvoptimal;
    double cv;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_init_copy(&_c, c, _state);
    c = &_c;
    *info = 0;
    *threshold = 0;
    *pal = 0;
    *pbl = 0;
    *par = 0;
    *pbr = 0;
    *cve = 0;
    ae_vector_init(&ties, 0, DT_INT, _state);
    ae_vector_init(&p1, 0, DT_INT, _state);
    ae_vector_init(&p2, 0, DT_INT, _state);

    
    /*
     * Test for errors in inputs
     */
    if( n<=0 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        if( c->ptr.p_int[i]!=0&&c->ptr.p_int[i]!=1 )
        {
            *info = -2;
            ae_frame_leave(_state);
            return;
        }
    }
    *info = 1;
    
    /*
     * Tie
     */
    dstie(a, n, &ties, &tiecount, &p1, &p2, _state);
    for(i=0; i<=n-1; i++)
    {
        if( p2.ptr.p_int[i]!=i )
        {
            t = c->ptr.p_int[i];
            c->ptr.p_int[i] = c->ptr.p_int[p2.ptr.p_int[i]];
            c->ptr.p_int[p2.ptr.p_int[i]] = t;
        }
    }
    
    /*
     * Special case: number of ties is 1.
     *
     * NOTE: we assume that P[i,j] equals to 0 or 1,
     *       intermediate values are not allowed.
     */
    if( tiecount==1 )
    {
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * General case, number of ties > 1
     *
     * NOTE: we assume that P[i,j] equals to 0 or 1,
     *       intermediate values are not allowed.
     */
    *pal = (double)(0);
    *pbl = (double)(0);
    *par = (double)(0);
    *pbr = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        if( c->ptr.p_int[i]==0 )
        {
            *par = *par+1;
        }
        if( c->ptr.p_int[i]==1 )
        {
            *pbr = *pbr+1;
        }
    }
    koptimal = -1;
    cvoptimal = ae_maxrealnumber;
    for(k=0; k<=tiecount-2; k++)
    {
        
        /*
         * first, obtain information about K-th tie which is
         * moved from R-part to L-part
         */
        pak = (double)(0);
        pbk = (double)(0);
        for(i=ties.ptr.p_int[k]; i<=ties.ptr.p_int[k+1]-1; i++)
        {
            if( c->ptr.p_int[i]==0 )
            {
                pak = pak+1;
            }
            if( c->ptr.p_int[i]==1 )
            {
                pbk = pbk+1;
            }
        }
        
        /*
         * Calculate cross-validation CE
         */
        cv = (double)(0);
        cv = cv-bdss_xlny(*pal+pak, (*pal+pak)/(*pal+pak+(*pbl)+pbk+1), _state);
        cv = cv-bdss_xlny(*pbl+pbk, (*pbl+pbk)/(*pal+pak+1+(*pbl)+pbk), _state);
        cv = cv-bdss_xlny(*par-pak, (*par-pak)/(*par-pak+(*pbr)-pbk+1), _state);
        cv = cv-bdss_xlny(*pbr-pbk, (*pbr-pbk)/(*par-pak+1+(*pbr)-pbk), _state);
        
        /*
         * Compare with best
         */
        if( ae_fp_less(cv,cvoptimal) )
        {
            cvoptimal = cv;
            koptimal = k;
        }
        
        /*
         * update
         */
        *pal = *pal+pak;
        *pbl = *pbl+pbk;
        *par = *par-pak;
        *pbr = *pbr-pbk;
    }
    *cve = cvoptimal;
    *threshold = 0.5*(a->ptr.p_double[ties.ptr.p_int[koptimal]]+a->ptr.p_double[ties.ptr.p_int[koptimal+1]]);
    *pal = (double)(0);
    *pbl = (double)(0);
    *par = (double)(0);
    *pbr = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_less(a->ptr.p_double[i],*threshold) )
        {
            if( c->ptr.p_int[i]==0 )
            {
                *pal = *pal+1;
            }
            else
            {
                *pbl = *pbl+1;
            }
        }
        else
        {
            if( c->ptr.p_int[i]==0 )
            {
                *par = *par+1;
            }
            else
            {
                *pbr = *pbr+1;
            }
        }
    }
    s = *pal+(*pbl);
    *pal = *pal/s;
    *pbl = *pbl/s;
    s = *par+(*pbr);
    *par = *par/s;
    *pbr = *pbr/s;
    ae_frame_leave(_state);
}


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
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;
    ae_int_t cl;
    ae_int_t tiecount;
    double cbest;
    double cc;
    ae_int_t koptimal;
    ae_int_t sl;
    ae_int_t sr;
    double v;
    double w;
    double x;

    *info = 0;
    *threshold = 0;
    *rms = 0;
    *cvrms = 0;

    
    /*
     * Test for errors in inputs
     */
    if( n<=0||nc<2 )
    {
        *info = -1;
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        if( c->ptr.p_int[i]<0||c->ptr.p_int[i]>=nc )
        {
            *info = -2;
            return;
        }
    }
    *info = 1;
    
    /*
     * Tie
     */
    dstiefasti(a, c, n, tiesbuf, &tiecount, bufr, bufi, _state);
    
    /*
     * Special case: number of ties is 1.
     */
    if( tiecount==1 )
    {
        *info = -3;
        return;
    }
    
    /*
     * General case, number of ties > 1
     */
    for(i=0; i<=2*nc-1; i++)
    {
        cntbuf->ptr.p_int[i] = 0;
    }
    for(i=0; i<=n-1; i++)
    {
        cntbuf->ptr.p_int[nc+c->ptr.p_int[i]] = cntbuf->ptr.p_int[nc+c->ptr.p_int[i]]+1;
    }
    koptimal = -1;
    *threshold = a->ptr.p_double[n-1];
    cbest = ae_maxrealnumber;
    sl = 0;
    sr = n;
    for(k=0; k<=tiecount-2; k++)
    {
        
        /*
         * first, move Kth tie from right to left
         */
        for(i=tiesbuf->ptr.p_int[k]; i<=tiesbuf->ptr.p_int[k+1]-1; i++)
        {
            cl = c->ptr.p_int[i];
            cntbuf->ptr.p_int[cl] = cntbuf->ptr.p_int[cl]+1;
            cntbuf->ptr.p_int[nc+cl] = cntbuf->ptr.p_int[nc+cl]-1;
        }
        sl = sl+(tiesbuf->ptr.p_int[k+1]-tiesbuf->ptr.p_int[k]);
        sr = sr-(tiesbuf->ptr.p_int[k+1]-tiesbuf->ptr.p_int[k]);
        
        /*
         * Calculate RMS error
         */
        v = (double)(0);
        for(i=0; i<=nc-1; i++)
        {
            w = (double)(cntbuf->ptr.p_int[i]);
            v = v+w*ae_sqr(w/sl-1, _state);
            v = v+(sl-w)*ae_sqr(w/sl, _state);
            w = (double)(cntbuf->ptr.p_int[nc+i]);
            v = v+w*ae_sqr(w/sr-1, _state);
            v = v+(sr-w)*ae_sqr(w/sr, _state);
        }
        v = ae_sqrt(v/(nc*n), _state);
        
        /*
         * Compare with best
         */
        x = (double)(2*sl)/(double)(sl+sr)-1;
        cc = v*(1-alpha+alpha*ae_sqr(x, _state));
        if( ae_fp_less(cc,cbest) )
        {
            
            /*
             * store split
             */
            *rms = v;
            koptimal = k;
            cbest = cc;
            
            /*
             * calculate CVRMS error
             */
            *cvrms = (double)(0);
            for(i=0; i<=nc-1; i++)
            {
                if( sl>1 )
                {
                    w = (double)(cntbuf->ptr.p_int[i]);
                    *cvrms = *cvrms+w*ae_sqr((w-1)/(sl-1)-1, _state);
                    *cvrms = *cvrms+(sl-w)*ae_sqr(w/(sl-1), _state);
                }
                else
                {
                    w = (double)(cntbuf->ptr.p_int[i]);
                    *cvrms = *cvrms+w*ae_sqr((double)1/(double)nc-1, _state);
                    *cvrms = *cvrms+(sl-w)*ae_sqr((double)1/(double)nc, _state);
                }
                if( sr>1 )
                {
                    w = (double)(cntbuf->ptr.p_int[nc+i]);
                    *cvrms = *cvrms+w*ae_sqr((w-1)/(sr-1)-1, _state);
                    *cvrms = *cvrms+(sr-w)*ae_sqr(w/(sr-1), _state);
                }
                else
                {
                    w = (double)(cntbuf->ptr.p_int[nc+i]);
                    *cvrms = *cvrms+w*ae_sqr((double)1/(double)nc-1, _state);
                    *cvrms = *cvrms+(sr-w)*ae_sqr((double)1/(double)nc, _state);
                }
            }
            *cvrms = ae_sqrt(*cvrms/(nc*n), _state);
        }
    }
    
    /*
     * Calculate threshold.
     * Code is a bit complicated because there can be such
     * numbers that 0.5(A+B) equals to A or B (if A-B=epsilon)
     */
    *threshold = 0.5*(a->ptr.p_double[tiesbuf->ptr.p_int[koptimal]]+a->ptr.p_double[tiesbuf->ptr.p_int[koptimal+1]]);
    if( ae_fp_less_eq(*threshold,a->ptr.p_double[tiesbuf->ptr.p_int[koptimal]]) )
    {
        *threshold = a->ptr.p_double[tiesbuf->ptr.p_int[koptimal+1]];
    }
}


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
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _a;
    ae_vector _c;
    ae_int_t i;
    ae_int_t j;
    ae_int_t j1;
    ae_int_t k;
    ae_vector ties;
    ae_int_t tiecount;
    ae_vector p1;
    ae_vector p2;
    ae_vector cnt;
    double v2;
    ae_int_t bestk;
    double bestcve;
    ae_vector bestsizes;
    double curcve;
    ae_vector cursizes;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_init_copy(&_c, c, _state);
    c = &_c;
    *info = 0;
    ae_vector_clear(thresholds);
    *ni = 0;
    *cve = 0;
    ae_vector_init(&ties, 0, DT_INT, _state);
    ae_vector_init(&p1, 0, DT_INT, _state);
    ae_vector_init(&p2, 0, DT_INT, _state);
    ae_vector_init(&cnt, 0, DT_INT, _state);
    ae_vector_init(&bestsizes, 0, DT_INT, _state);
    ae_vector_init(&cursizes, 0, DT_INT, _state);

    
    /*
     * Test for errors in inputs
     */
    if( (n<=0||nc<2)||kmax<2 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        if( c->ptr.p_int[i]<0||c->ptr.p_int[i]>=nc )
        {
            *info = -2;
            ae_frame_leave(_state);
            return;
        }
    }
    *info = 1;
    
    /*
     * Tie
     */
    dstie(a, n, &ties, &tiecount, &p1, &p2, _state);
    for(i=0; i<=n-1; i++)
    {
        if( p2.ptr.p_int[i]!=i )
        {
            k = c->ptr.p_int[i];
            c->ptr.p_int[i] = c->ptr.p_int[p2.ptr.p_int[i]];
            c->ptr.p_int[p2.ptr.p_int[i]] = k;
        }
    }
    
    /*
     * Special cases
     */
    if( tiecount==1 )
    {
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * General case:
     * 0. allocate arrays
     */
    kmax = ae_minint(kmax, tiecount, _state);
    ae_vector_set_length(&bestsizes, kmax-1+1, _state);
    ae_vector_set_length(&cursizes, kmax-1+1, _state);
    ae_vector_set_length(&cnt, nc-1+1, _state);
    
    /*
     * General case:
     * 1. prepare "weak" solution (two subintervals, divided at median)
     */
    v2 = ae_maxrealnumber;
    j = -1;
    for(i=1; i<=tiecount-1; i++)
    {
        if( ae_fp_less(ae_fabs(ties.ptr.p_int[i]-0.5*(n-1), _state),v2) )
        {
            v2 = ae_fabs(ties.ptr.p_int[i]-0.5*n, _state);
            j = i;
        }
    }
    ae_assert(j>0, "DSSplitK: internal error #1!", _state);
    bestk = 2;
    bestsizes.ptr.p_int[0] = ties.ptr.p_int[j];
    bestsizes.ptr.p_int[1] = n-j;
    bestcve = (double)(0);
    for(i=0; i<=nc-1; i++)
    {
        cnt.ptr.p_int[i] = 0;
    }
    for(i=0; i<=j-1; i++)
    {
        bdss_tieaddc(c, &ties, i, nc, &cnt, _state);
    }
    bestcve = bestcve+bdss_getcv(&cnt, nc, _state);
    for(i=0; i<=nc-1; i++)
    {
        cnt.ptr.p_int[i] = 0;
    }
    for(i=j; i<=tiecount-1; i++)
    {
        bdss_tieaddc(c, &ties, i, nc, &cnt, _state);
    }
    bestcve = bestcve+bdss_getcv(&cnt, nc, _state);
    
    /*
     * General case:
     * 2. Use greedy algorithm to find sub-optimal split in O(KMax*N) time
     */
    for(k=2; k<=kmax; k++)
    {
        
        /*
         * Prepare greedy K-interval split
         */
        for(i=0; i<=k-1; i++)
        {
            cursizes.ptr.p_int[i] = 0;
        }
        i = 0;
        j = 0;
        while(j<=tiecount-1&&i<=k-1)
        {
            
            /*
             * Rule: I-th bin is empty, fill it
             */
            if( cursizes.ptr.p_int[i]==0 )
            {
                cursizes.ptr.p_int[i] = ties.ptr.p_int[j+1]-ties.ptr.p_int[j];
                j = j+1;
                continue;
            }
            
            /*
             * Rule: (K-1-I) bins left, (K-1-I) ties left (1 tie per bin); next bin
             */
            if( tiecount-j==k-1-i )
            {
                i = i+1;
                continue;
            }
            
            /*
             * Rule: last bin, always place in current
             */
            if( i==k-1 )
            {
                cursizes.ptr.p_int[i] = cursizes.ptr.p_int[i]+ties.ptr.p_int[j+1]-ties.ptr.p_int[j];
                j = j+1;
                continue;
            }
            
            /*
             * Place J-th tie in I-th bin, or leave for I+1-th bin.
             */
            if( ae_fp_less(ae_fabs(cursizes.ptr.p_int[i]+ties.ptr.p_int[j+1]-ties.ptr.p_int[j]-(double)n/(double)k, _state),ae_fabs(cursizes.ptr.p_int[i]-(double)n/(double)k, _state)) )
            {
                cursizes.ptr.p_int[i] = cursizes.ptr.p_int[i]+ties.ptr.p_int[j+1]-ties.ptr.p_int[j];
                j = j+1;
            }
            else
            {
                i = i+1;
            }
        }
        ae_assert(cursizes.ptr.p_int[k-1]!=0&&j==tiecount, "DSSplitK: internal error #1", _state);
        
        /*
         * Calculate CVE
         */
        curcve = (double)(0);
        j = 0;
        for(i=0; i<=k-1; i++)
        {
            for(j1=0; j1<=nc-1; j1++)
            {
                cnt.ptr.p_int[j1] = 0;
            }
            for(j1=j; j1<=j+cursizes.ptr.p_int[i]-1; j1++)
            {
                cnt.ptr.p_int[c->ptr.p_int[j1]] = cnt.ptr.p_int[c->ptr.p_int[j1]]+1;
            }
            curcve = curcve+bdss_getcv(&cnt, nc, _state);
            j = j+cursizes.ptr.p_int[i];
        }
        
        /*
         * Choose best variant
         */
        if( ae_fp_less(curcve,bestcve) )
        {
            for(i=0; i<=k-1; i++)
            {
                bestsizes.ptr.p_int[i] = cursizes.ptr.p_int[i];
            }
            bestcve = curcve;
            bestk = k;
        }
    }
    
    /*
     * Transform from sizes to thresholds
     */
    *cve = bestcve;
    *ni = bestk;
    ae_vector_set_length(thresholds, *ni-2+1, _state);
    j = bestsizes.ptr.p_int[0];
    for(i=1; i<=bestk-1; i++)
    {
        thresholds->ptr.p_double[i-1] = 0.5*(a->ptr.p_double[j-1]+a->ptr.p_double[j]);
        j = j+bestsizes.ptr.p_int[i];
    }
    ae_frame_leave(_state);
}


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
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector _a;
    ae_vector _c;
    ae_int_t i;
    ae_int_t j;
    ae_int_t s;
    ae_int_t jl;
    ae_int_t jr;
    double v2;
    ae_vector ties;
    ae_int_t tiecount;
    ae_vector p1;
    ae_vector p2;
    double cvtemp;
    ae_vector cnt;
    ae_vector cnt2;
    ae_matrix cv;
    ae_matrix splits;
    ae_int_t k;
    ae_int_t koptimal;
    double cvoptimal;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init_copy(&_a, a, _state);
    a = &_a;
    ae_vector_init_copy(&_c, c, _state);
    c = &_c;
    *info = 0;
    ae_vector_clear(thresholds);
    *ni = 0;
    *cve = 0;
    ae_vector_init(&ties, 0, DT_INT, _state);
    ae_vector_init(&p1, 0, DT_INT, _state);
    ae_vector_init(&p2, 0, DT_INT, _state);
    ae_vector_init(&cnt, 0, DT_INT, _state);
    ae_vector_init(&cnt2, 0, DT_INT, _state);
    ae_matrix_init(&cv, 0, 0, DT_REAL, _state);
    ae_matrix_init(&splits, 0, 0, DT_INT, _state);

    
    /*
     * Test for errors in inputs
     */
    if( (n<=0||nc<2)||kmax<2 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        if( c->ptr.p_int[i]<0||c->ptr.p_int[i]>=nc )
        {
            *info = -2;
            ae_frame_leave(_state);
            return;
        }
    }
    *info = 1;
    
    /*
     * Tie
     */
    dstie(a, n, &ties, &tiecount, &p1, &p2, _state);
    for(i=0; i<=n-1; i++)
    {
        if( p2.ptr.p_int[i]!=i )
        {
            k = c->ptr.p_int[i];
            c->ptr.p_int[i] = c->ptr.p_int[p2.ptr.p_int[i]];
            c->ptr.p_int[p2.ptr.p_int[i]] = k;
        }
    }
    
    /*
     * Special cases
     */
    if( tiecount==1 )
    {
        *info = -3;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * General case
     * Use dynamic programming to find best split in O(KMax*NC*TieCount^2) time
     */
    kmax = ae_minint(kmax, tiecount, _state);
    ae_matrix_set_length(&cv, kmax-1+1, tiecount-1+1, _state);
    ae_matrix_set_length(&splits, kmax-1+1, tiecount-1+1, _state);
    ae_vector_set_length(&cnt, nc-1+1, _state);
    ae_vector_set_length(&cnt2, nc-1+1, _state);
    for(j=0; j<=nc-1; j++)
    {
        cnt.ptr.p_int[j] = 0;
    }
    for(j=0; j<=tiecount-1; j++)
    {
        bdss_tieaddc(c, &ties, j, nc, &cnt, _state);
        splits.ptr.pp_int[0][j] = 0;
        cv.ptr.pp_double[0][j] = bdss_getcv(&cnt, nc, _state);
    }
    for(k=1; k<=kmax-1; k++)
    {
        for(j=0; j<=nc-1; j++)
        {
            cnt.ptr.p_int[j] = 0;
        }
        
        /*
         * Subtask size J in [K..TieCount-1]:
         * optimal K-splitting on ties from 0-th to J-th.
         */
        for(j=k; j<=tiecount-1; j++)
        {
            
            /*
             * Update Cnt - let it contain classes of ties from K-th to J-th
             */
            bdss_tieaddc(c, &ties, j, nc, &cnt, _state);
            
            /*
             * Search for optimal split point S in [K..J]
             */
            for(i=0; i<=nc-1; i++)
            {
                cnt2.ptr.p_int[i] = cnt.ptr.p_int[i];
            }
            cv.ptr.pp_double[k][j] = cv.ptr.pp_double[k-1][j-1]+bdss_getcv(&cnt2, nc, _state);
            splits.ptr.pp_int[k][j] = j;
            for(s=k+1; s<=j; s++)
            {
                
                /*
                 * Update Cnt2 - let it contain classes of ties from S-th to J-th
                 */
                bdss_tiesubc(c, &ties, s-1, nc, &cnt2, _state);
                
                /*
                 * Calculate CVE
                 */
                cvtemp = cv.ptr.pp_double[k-1][s-1]+bdss_getcv(&cnt2, nc, _state);
                if( ae_fp_less(cvtemp,cv.ptr.pp_double[k][j]) )
                {
                    cv.ptr.pp_double[k][j] = cvtemp;
                    splits.ptr.pp_int[k][j] = s;
                }
            }
        }
    }
    
    /*
     * Choose best partition, output result
     */
    koptimal = -1;
    cvoptimal = ae_maxrealnumber;
    for(k=0; k<=kmax-1; k++)
    {
        if( ae_fp_less(cv.ptr.pp_double[k][tiecount-1],cvoptimal) )
        {
            cvoptimal = cv.ptr.pp_double[k][tiecount-1];
            koptimal = k;
        }
    }
    ae_assert(koptimal>=0, "DSOptimalSplitK: internal error #1!", _state);
    if( koptimal==0 )
    {
        
        /*
         * Special case: best partition is one big interval.
         * Even 2-partition is not better.
         * This is possible when dealing with "weak" predictor variables.
         *
         * Make binary split as close to the median as possible.
         */
        v2 = ae_maxrealnumber;
        j = -1;
        for(i=1; i<=tiecount-1; i++)
        {
            if( ae_fp_less(ae_fabs(ties.ptr.p_int[i]-0.5*(n-1), _state),v2) )
            {
                v2 = ae_fabs(ties.ptr.p_int[i]-0.5*(n-1), _state);
                j = i;
            }
        }
        ae_assert(j>0, "DSOptimalSplitK: internal error #2!", _state);
        ae_vector_set_length(thresholds, 0+1, _state);
        thresholds->ptr.p_double[0] = 0.5*(a->ptr.p_double[ties.ptr.p_int[j-1]]+a->ptr.p_double[ties.ptr.p_int[j]]);
        *ni = 2;
        *cve = (double)(0);
        for(i=0; i<=nc-1; i++)
        {
            cnt.ptr.p_int[i] = 0;
        }
        for(i=0; i<=j-1; i++)
        {
            bdss_tieaddc(c, &ties, i, nc, &cnt, _state);
        }
        *cve = *cve+bdss_getcv(&cnt, nc, _state);
        for(i=0; i<=nc-1; i++)
        {
            cnt.ptr.p_int[i] = 0;
        }
        for(i=j; i<=tiecount-1; i++)
        {
            bdss_tieaddc(c, &ties, i, nc, &cnt, _state);
        }
        *cve = *cve+bdss_getcv(&cnt, nc, _state);
    }
    else
    {
        
        /*
         * General case: 2 or more intervals
         *
         * NOTE: we initialize both JL and JR (left and right bounds),
         *       altough algorithm needs only JL.
         */
        ae_vector_set_length(thresholds, koptimal-1+1, _state);
        *ni = koptimal+1;
        *cve = cv.ptr.pp_double[koptimal][tiecount-1];
        jl = splits.ptr.pp_int[koptimal][tiecount-1];
        jr = tiecount-1;
        for(k=koptimal; k>=1; k--)
        {
            thresholds->ptr.p_double[k-1] = 0.5*(a->ptr.p_double[ties.ptr.p_int[jl-1]]+a->ptr.p_double[ties.ptr.p_int[jl]]);
            jr = jl-1;
            jl = splits.ptr.pp_int[k-1][jl-1];
        }
        touchint(&jr, _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Internal function
*************************************************************************/
static double bdss_xlny(double x, double y, ae_state *_state)
{
    double result;


    if( ae_fp_eq(x,(double)(0)) )
    {
        result = (double)(0);
    }
    else
    {
        result = x*ae_log(y, _state);
    }
    return result;
}


/*************************************************************************
Internal function,
returns number of samples of class I in Cnt[I]
*************************************************************************/
static double bdss_getcv(/* Integer */ ae_vector* cnt,
     ae_int_t nc,
     ae_state *_state)
{
    ae_int_t i;
    double s;
    double result;


    s = (double)(0);
    for(i=0; i<=nc-1; i++)
    {
        s = s+cnt->ptr.p_int[i];
    }
    result = (double)(0);
    for(i=0; i<=nc-1; i++)
    {
        result = result-bdss_xlny((double)(cnt->ptr.p_int[i]), cnt->ptr.p_int[i]/(s+nc-1), _state);
    }
    return result;
}


/*************************************************************************
Internal function, adds number of samples of class I in tie NTie to Cnt[I]
*************************************************************************/
static void bdss_tieaddc(/* Integer */ ae_vector* c,
     /* Integer */ ae_vector* ties,
     ae_int_t ntie,
     ae_int_t nc,
     /* Integer */ ae_vector* cnt,
     ae_state *_state)
{
    ae_int_t i;


    for(i=ties->ptr.p_int[ntie]; i<=ties->ptr.p_int[ntie+1]-1; i++)
    {
        cnt->ptr.p_int[c->ptr.p_int[i]] = cnt->ptr.p_int[c->ptr.p_int[i]]+1;
    }
}


/*************************************************************************
Internal function, subtracts number of samples of class I in tie NTie to Cnt[I]
*************************************************************************/
static void bdss_tiesubc(/* Integer */ ae_vector* c,
     /* Integer */ ae_vector* ties,
     ae_int_t ntie,
     ae_int_t nc,
     /* Integer */ ae_vector* cnt,
     ae_state *_state)
{
    ae_int_t i;


    for(i=ties->ptr.p_int[ntie]; i<=ties->ptr.p_int[ntie+1]-1; i++)
    {
        cnt->ptr.p_int[c->ptr.p_int[i]] = cnt->ptr.p_int[c->ptr.p_int[i]]-1;
    }
}


void _cvreport_init(void* _p, ae_state *_state)
{
    cvreport *p = (cvreport*)_p;
    ae_touch_ptr((void*)p);
}


void _cvreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    cvreport *dst = (cvreport*)_dst;
    cvreport *src = (cvreport*)_src;
    dst->relclserror = src->relclserror;
    dst->avgce = src->avgce;
    dst->rmserror = src->rmserror;
    dst->avgerror = src->avgerror;
    dst->avgrelerror = src->avgrelerror;
}


void _cvreport_clear(void* _p)
{
    cvreport *p = (cvreport*)_p;
    ae_touch_ptr((void*)p);
}


void _cvreport_destroy(void* _p)
{
    cvreport *p = (cvreport*)_p;
    ae_touch_ptr((void*)p);
}


/*$ End $*/
