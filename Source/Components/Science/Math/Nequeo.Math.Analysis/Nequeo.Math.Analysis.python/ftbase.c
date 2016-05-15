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
#include "ftbase.h"


/*$ Declarations $*/
static ae_int_t ftbase_coltype = 0;
static ae_int_t ftbase_coloperandscnt = 1;
static ae_int_t ftbase_coloperandsize = 2;
static ae_int_t ftbase_colmicrovectorsize = 3;
static ae_int_t ftbase_colparam0 = 4;
static ae_int_t ftbase_colparam1 = 5;
static ae_int_t ftbase_colparam2 = 6;
static ae_int_t ftbase_colparam3 = 7;
static ae_int_t ftbase_colscnt = 8;
static ae_int_t ftbase_opend = 0;
static ae_int_t ftbase_opcomplexreffft = 1;
static ae_int_t ftbase_opbluesteinsfft = 2;
static ae_int_t ftbase_opcomplexcodeletfft = 3;
static ae_int_t ftbase_opcomplexcodelettwfft = 4;
static ae_int_t ftbase_opradersfft = 5;
static ae_int_t ftbase_opcomplextranspose = -1;
static ae_int_t ftbase_opcomplexfftfactors = -2;
static ae_int_t ftbase_opstart = -3;
static ae_int_t ftbase_opjmp = -4;
static ae_int_t ftbase_opparallelcall = -5;
static ae_int_t ftbase_maxradix = 6;
static ae_int_t ftbase_updatetw = 16;
static ae_int_t ftbase_recursivethreshold = 1024;
static ae_int_t ftbase_raderthreshold = 19;
static ae_int_t ftbase_ftbasecodeletrecommended = 5;
static double ftbase_ftbaseinefficiencyfactor = 1.3;
static ae_int_t ftbase_ftbasemaxsmoothfactor = 5;
static void ftbase_ftdeterminespacerequirements(ae_int_t n,
     ae_int_t* precrsize,
     ae_int_t* precisize,
     ae_state *_state);
static void ftbase_ftcomplexfftplanrec(ae_int_t n,
     ae_int_t k,
     ae_bool childplan,
     ae_bool topmostplan,
     ae_int_t* rowptr,
     ae_int_t* bluesteinsize,
     ae_int_t* precrptr,
     ae_int_t* preciptr,
     fasttransformplan* plan,
     ae_state *_state);
static void ftbase_ftpushentry(fasttransformplan* plan,
     ae_int_t* rowptr,
     ae_int_t etype,
     ae_int_t eopcnt,
     ae_int_t eopsize,
     ae_int_t emcvsize,
     ae_int_t eparam0,
     ae_state *_state);
static void ftbase_ftpushentry2(fasttransformplan* plan,
     ae_int_t* rowptr,
     ae_int_t etype,
     ae_int_t eopcnt,
     ae_int_t eopsize,
     ae_int_t emcvsize,
     ae_int_t eparam0,
     ae_int_t eparam1,
     ae_state *_state);
static void ftbase_ftpushentry4(fasttransformplan* plan,
     ae_int_t* rowptr,
     ae_int_t etype,
     ae_int_t eopcnt,
     ae_int_t eopsize,
     ae_int_t emcvsize,
     ae_int_t eparam0,
     ae_int_t eparam1,
     ae_int_t eparam2,
     ae_int_t eparam3,
     ae_state *_state);
static void ftbase_ftapplysubplan(fasttransformplan* plan,
     ae_int_t subplan,
     /* Real    */ ae_vector* a,
     ae_int_t abase,
     ae_int_t aoffset,
     /* Real    */ ae_vector* buf,
     ae_int_t repcnt,
     ae_state *_state);
static void ftbase_ftapplycomplexreffft(/* Real    */ ae_vector* a,
     ae_int_t offs,
     ae_int_t operandscnt,
     ae_int_t operandsize,
     ae_int_t microvectorsize,
     /* Real    */ ae_vector* buf,
     ae_state *_state);
static void ftbase_ftapplycomplexcodeletfft(/* Real    */ ae_vector* a,
     ae_int_t offs,
     ae_int_t operandscnt,
     ae_int_t operandsize,
     ae_int_t microvectorsize,
     ae_state *_state);
static void ftbase_ftapplycomplexcodelettwfft(/* Real    */ ae_vector* a,
     ae_int_t offs,
     ae_int_t operandscnt,
     ae_int_t operandsize,
     ae_int_t microvectorsize,
     ae_state *_state);
static void ftbase_ftprecomputebluesteinsfft(ae_int_t n,
     ae_int_t m,
     /* Real    */ ae_vector* precr,
     ae_int_t offs,
     ae_state *_state);
static void ftbase_ftbluesteinsfft(fasttransformplan* plan,
     /* Real    */ ae_vector* a,
     ae_int_t abase,
     ae_int_t aoffset,
     ae_int_t operandscnt,
     ae_int_t n,
     ae_int_t m,
     ae_int_t precoffs,
     ae_int_t subplan,
     /* Real    */ ae_vector* bufa,
     /* Real    */ ae_vector* bufb,
     /* Real    */ ae_vector* bufc,
     /* Real    */ ae_vector* bufd,
     ae_state *_state);
static void ftbase_ftprecomputeradersfft(ae_int_t n,
     ae_int_t rq,
     ae_int_t riq,
     /* Real    */ ae_vector* precr,
     ae_int_t offs,
     ae_state *_state);
static void ftbase_ftradersfft(fasttransformplan* plan,
     /* Real    */ ae_vector* a,
     ae_int_t abase,
     ae_int_t aoffset,
     ae_int_t operandscnt,
     ae_int_t n,
     ae_int_t subplan,
     ae_int_t rq,
     ae_int_t riq,
     ae_int_t precoffs,
     /* Real    */ ae_vector* buf,
     ae_state *_state);
static void ftbase_ftfactorize(ae_int_t n,
     ae_bool isroot,
     ae_int_t* n1,
     ae_int_t* n2,
     ae_state *_state);
static ae_int_t ftbase_ftoptimisticestimate(ae_int_t n, ae_state *_state);
static void ftbase_ffttwcalc(/* Real    */ ae_vector* a,
     ae_int_t aoffset,
     ae_int_t n1,
     ae_int_t n2,
     ae_state *_state);
static void ftbase_internalcomplexlintranspose(/* Real    */ ae_vector* a,
     ae_int_t m,
     ae_int_t n,
     ae_int_t astart,
     /* Real    */ ae_vector* buf,
     ae_state *_state);
static void ftbase_ffticltrec(/* Real    */ ae_vector* a,
     ae_int_t astart,
     ae_int_t astride,
     /* Real    */ ae_vector* b,
     ae_int_t bstart,
     ae_int_t bstride,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state);
static void ftbase_fftirltrec(/* Real    */ ae_vector* a,
     ae_int_t astart,
     ae_int_t astride,
     /* Real    */ ae_vector* b,
     ae_int_t bstart,
     ae_int_t bstride,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state);
static void ftbase_ftbasefindsmoothrec(ae_int_t n,
     ae_int_t seed,
     ae_int_t leastfactor,
     ae_int_t* best,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
This subroutine generates FFT plan for K complex FFT's with length N each.

INPUT PARAMETERS:
    N           -   FFT length (in complex numbers), N>=1
    K           -   number of repetitions, K>=1
    
OUTPUT PARAMETERS:
    Plan        -   plan

  -- ALGLIB --
     Copyright 05.04.2013 by Bochkanov Sergey
*************************************************************************/
void ftcomplexfftplan(ae_int_t n,
     ae_int_t k,
     fasttransformplan* plan,
     ae_state *_state)
{
    ae_frame _frame_block;
    srealarray bluesteinbuf;
    ae_int_t rowptr;
    ae_int_t bluesteinsize;
    ae_int_t precrptr;
    ae_int_t preciptr;
    ae_int_t precrsize;
    ae_int_t precisize;

    ae_frame_make(_state, &_frame_block);
    _fasttransformplan_clear(plan);
    _srealarray_init(&bluesteinbuf, _state);

    
    /*
     * Initial check for parameters
     */
    ae_assert(n>0, "FTComplexFFTPlan: N<=0", _state);
    ae_assert(k>0, "FTComplexFFTPlan: K<=0", _state);
    
    /*
     * Determine required sizes of precomputed real and integer
     * buffers. This stage of code is highly dependent on internals
     * of FTComplexFFTPlanRec() and must be kept synchronized with
     * possible changes in internals of plan generation function.
     *
     * Buffer size is determined as follows:
     * * N is factorized
     * * we factor out anything which is less or equal to MaxRadix
     * * prime factor F>RaderThreshold requires 4*FTBaseFindSmooth(2*F-1)
     *   real entries to store precomputed Quantities for Bluestein's
     *   transformation
     * * prime factor F<=RaderThreshold does NOT require
     *   precomputed storage
     */
    precrsize = 0;
    precisize = 0;
    ftbase_ftdeterminespacerequirements(n, &precrsize, &precisize, _state);
    if( precrsize>0 )
    {
        ae_vector_set_length(&plan->precr, precrsize, _state);
    }
    if( precisize>0 )
    {
        ae_vector_set_length(&plan->preci, precisize, _state);
    }
    
    /*
     * Generate plan
     */
    rowptr = 0;
    precrptr = 0;
    preciptr = 0;
    bluesteinsize = 1;
    ae_vector_set_length(&plan->buffer, 2*n*k, _state);
    ftbase_ftcomplexfftplanrec(n, k, ae_true, ae_true, &rowptr, &bluesteinsize, &precrptr, &preciptr, plan, _state);
    ae_vector_set_length(&bluesteinbuf.val, bluesteinsize, _state);
    ae_shared_pool_set_seed(&plan->bluesteinpool, &bluesteinbuf, sizeof(bluesteinbuf), _srealarray_init, _srealarray_init_copy, _srealarray_destroy, _state);
    
    /*
     * Check that actual amount of precomputed space used by transformation
     * plan is EXACTLY equal to amount of space allocated by us.
     */
    ae_assert(precrptr==precrsize, "FTComplexFFTPlan: internal error (PrecRPtr<>PrecRSize)", _state);
    ae_assert(preciptr==precisize, "FTComplexFFTPlan: internal error (PrecRPtr<>PrecRSize)", _state);
    ae_frame_leave(_state);
}


/*************************************************************************
This subroutine applies transformation plan to input/output array A.

INPUT PARAMETERS:
    Plan        -   transformation plan
    A           -   array, must be large enough for plan to work
    OffsA       -   offset of the subarray to process
    RepCnt      -   repetition count (transformation is repeatedly applied
                    to subsequent subarrays)
    
OUTPUT PARAMETERS:
    Plan        -   plan (temporary buffers can be modified, plan itself
                    is unchanged and can be reused)
    A           -   transformed array

  -- ALGLIB --
     Copyright 05.04.2013 by Bochkanov Sergey
*************************************************************************/
void ftapplyplan(fasttransformplan* plan,
     /* Real    */ ae_vector* a,
     ae_int_t offsa,
     ae_int_t repcnt,
     ae_state *_state)
{
    ae_int_t plansize;
    ae_int_t i;


    plansize = plan->entries.ptr.pp_int[0][ftbase_coloperandscnt]*plan->entries.ptr.pp_int[0][ftbase_coloperandsize]*plan->entries.ptr.pp_int[0][ftbase_colmicrovectorsize];
    for(i=0; i<=repcnt-1; i++)
    {
        ftbase_ftapplysubplan(plan, 0, a, offsa+plansize*i, 0, &plan->buffer, 1, _state);
    }
}


/*************************************************************************
Returns good factorization N=N1*N2.

Usually N1<=N2 (but not always - small N's may be exception).
if N1<>1 then N2<>1.

Factorization is chosen depending on task type and codelets we have.

  -- ALGLIB --
     Copyright 01.05.2009 by Bochkanov Sergey
*************************************************************************/
void ftbasefactorize(ae_int_t n,
     ae_int_t tasktype,
     ae_int_t* n1,
     ae_int_t* n2,
     ae_state *_state)
{
    ae_int_t j;

    *n1 = 0;
    *n2 = 0;

    *n1 = 0;
    *n2 = 0;
    
    /*
     * try to find good codelet
     */
    if( *n1*(*n2)!=n )
    {
        for(j=ftbase_ftbasecodeletrecommended; j>=2; j--)
        {
            if( n%j==0 )
            {
                *n1 = j;
                *n2 = n/j;
                break;
            }
        }
    }
    
    /*
     * try to factorize N
     */
    if( *n1*(*n2)!=n )
    {
        for(j=ftbase_ftbasecodeletrecommended+1; j<=n-1; j++)
        {
            if( n%j==0 )
            {
                *n1 = j;
                *n2 = n/j;
                break;
            }
        }
    }
    
    /*
     * looks like N is prime :(
     */
    if( *n1*(*n2)!=n )
    {
        *n1 = 1;
        *n2 = n;
    }
    
    /*
     * normalize
     */
    if( *n2==1&&*n1!=1 )
    {
        *n2 = *n1;
        *n1 = 1;
    }
}


/*************************************************************************
Is number smooth?

  -- ALGLIB --
     Copyright 01.05.2009 by Bochkanov Sergey
*************************************************************************/
ae_bool ftbaseissmooth(ae_int_t n, ae_state *_state)
{
    ae_int_t i;
    ae_bool result;


    for(i=2; i<=ftbase_ftbasemaxsmoothfactor; i++)
    {
        while(n%i==0)
        {
            n = n/i;
        }
    }
    result = n==1;
    return result;
}


/*************************************************************************
Returns smallest smooth (divisible only by 2, 3, 5) number that is greater
than or equal to max(N,2)

  -- ALGLIB --
     Copyright 01.05.2009 by Bochkanov Sergey
*************************************************************************/
ae_int_t ftbasefindsmooth(ae_int_t n, ae_state *_state)
{
    ae_int_t best;
    ae_int_t result;


    best = 2;
    while(best<n)
    {
        best = 2*best;
    }
    ftbase_ftbasefindsmoothrec(n, 1, 2, &best, _state);
    result = best;
    return result;
}


/*************************************************************************
Returns  smallest  smooth  (divisible only by 2, 3, 5) even number that is
greater than or equal to max(N,2)

  -- ALGLIB --
     Copyright 01.05.2009 by Bochkanov Sergey
*************************************************************************/
ae_int_t ftbasefindsmootheven(ae_int_t n, ae_state *_state)
{
    ae_int_t best;
    ae_int_t result;


    best = 2;
    while(best<n)
    {
        best = 2*best;
    }
    ftbase_ftbasefindsmoothrec(n, 2, 2, &best, _state);
    result = best;
    return result;
}


/*************************************************************************
Returns estimate of FLOP count for the FFT.

It is only an estimate based on operations count for the PERFECT FFT
and relative inefficiency of the algorithm actually used.

N should be power of 2, estimates are badly wrong for non-power-of-2 N's.

  -- ALGLIB --
     Copyright 01.05.2009 by Bochkanov Sergey
*************************************************************************/
double ftbasegetflopestimate(ae_int_t n, ae_state *_state)
{
    double result;


    result = ftbase_ftbaseinefficiencyfactor*(4*n*ae_log((double)(n), _state)/ae_log((double)(2), _state)-6*n+8);
    return result;
}


/*************************************************************************
This function returns EXACT estimate of the space requirements for N-point
FFT. Internals of this function are highly dependent on details of different
FFTs employed by this unit, so every time algorithm is changed this function
has to be rewritten.

INPUT PARAMETERS:
    N           -   transform length
    PrecRSize   -   must be set to zero
    PrecISize   -   must be set to zero
    
OUTPUT PARAMETERS:
    PrecRSize   -   number of real temporaries required for transformation
    PrecISize   -   number of integer temporaries required for transformation

    
  -- ALGLIB --
     Copyright 05.04.2013 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ftdeterminespacerequirements(ae_int_t n,
     ae_int_t* precrsize,
     ae_int_t* precisize,
     ae_state *_state)
{
    ae_int_t ncur;
    ae_int_t f;
    ae_int_t i;


    
    /*
     * Determine required sizes of precomputed real and integer
     * buffers. This stage of code is highly dependent on internals
     * of FTComplexFFTPlanRec() and must be kept synchronized with
     * possible changes in internals of plan generation function.
     *
     * Buffer size is determined as follows:
     * * N is factorized
     * * we factor out anything which is less or equal to MaxRadix
     * * prime factor F>RaderThreshold requires 4*FTBaseFindSmooth(2*F-1)
     *   real entries to store precomputed Quantities for Bluestein's
     *   transformation
     * * prime factor F<=RaderThreshold requires 2*(F-1)+ESTIMATE(F-1)
     *   precomputed storage
     */
    ncur = n;
    for(i=2; i<=ftbase_maxradix; i++)
    {
        while(ncur%i==0)
        {
            ncur = ncur/i;
        }
    }
    f = 2;
    while(f<=ncur)
    {
        while(ncur%f==0)
        {
            if( f>ftbase_raderthreshold )
            {
                *precrsize = *precrsize+4*ftbasefindsmooth(2*f-1, _state);
            }
            else
            {
                *precrsize = *precrsize+2*(f-1);
                ftbase_ftdeterminespacerequirements(f-1, precrsize, precisize, _state);
            }
            ncur = ncur/f;
        }
        f = f+1;
    }
}


/*************************************************************************
Recurrent function called by FTComplexFFTPlan() and other functions. It
recursively builds transformation plan

INPUT PARAMETERS:
    N           -   FFT length (in complex numbers), N>=1
    K           -   number of repetitions, K>=1
    ChildPlan   -   if True, plan generator inserts OpStart/opEnd in the
                    plan header/footer.
    TopmostPlan -   if True, plan generator assumes that it is topmost plan:
                    * it may use global buffer for transpositions
                    and there is no other plan which executes in parallel
    RowPtr      -   index which points to past-the-last entry generated so far
    BluesteinSize-  amount of storage (in real numbers) required for Bluestein buffer
    PrecRPtr    -   pointer to unused part of precomputed real buffer (Plan.PrecR):
                    * when this function stores some data to precomputed buffer,
                      it advances pointer.
                    * it is responsibility of the function to assert that
                      Plan.PrecR has enough space to store data before actually
                      writing to buffer.
                    * it is responsibility of the caller to allocate enough
                      space before calling this function
    PrecIPtr    -   pointer to unused part of precomputed integer buffer (Plan.PrecI):
                    * when this function stores some data to precomputed buffer,
                      it advances pointer.
                    * it is responsibility of the function to assert that
                      Plan.PrecR has enough space to store data before actually
                      writing to buffer.
                    * it is responsibility of the caller to allocate enough
                      space before calling this function
    Plan        -   plan (generated so far)
    
OUTPUT PARAMETERS:
    RowPtr      -   updated pointer (advanced by number of entries generated
                    by function)
    BluesteinSize-  updated amount
                    (may be increased, but may never be decreased)
        
NOTE: in case TopmostPlan is True, ChildPlan is also must be True.
    
  -- ALGLIB --
     Copyright 05.04.2013 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ftcomplexfftplanrec(ae_int_t n,
     ae_int_t k,
     ae_bool childplan,
     ae_bool topmostplan,
     ae_int_t* rowptr,
     ae_int_t* bluesteinsize,
     ae_int_t* precrptr,
     ae_int_t* preciptr,
     fasttransformplan* plan,
     ae_state *_state)
{
    ae_frame _frame_block;
    srealarray localbuf;
    ae_int_t m;
    ae_int_t n1;
    ae_int_t n2;
    ae_int_t gq;
    ae_int_t giq;
    ae_int_t row0;
    ae_int_t row1;
    ae_int_t row2;
    ae_int_t row3;

    ae_frame_make(_state, &_frame_block);
    _srealarray_init(&localbuf, _state);

    ae_assert(n>0, "FTComplexFFTPlan: N<=0", _state);
    ae_assert(k>0, "FTComplexFFTPlan: K<=0", _state);
    ae_assert(!topmostplan||childplan, "FTComplexFFTPlan: ChildPlan is inconsistent with TopmostPlan", _state);
    
    /*
     * Try to generate "topmost" plan
     */
    if( topmostplan&&n>ftbase_recursivethreshold )
    {
        ftbase_ftfactorize(n, ae_false, &n1, &n2, _state);
        if( n1*n2==0 )
        {
            
            /*
             * Handle prime-factor FFT with Bluestein's FFT.
             * Determine size of Bluestein's buffer.
             */
            m = ftbasefindsmooth(2*n-1, _state);
            *bluesteinsize = ae_maxint(2*m, *bluesteinsize, _state);
            
            /*
             * Generate plan
             */
            ftbase_ftpushentry2(plan, rowptr, ftbase_opstart, k, n, 2, -1, ftbase_ftoptimisticestimate(n, _state), _state);
            ftbase_ftpushentry4(plan, rowptr, ftbase_opbluesteinsfft, k, n, 2, m, 2, *precrptr, 0, _state);
            row0 = *rowptr;
            ftbase_ftpushentry(plan, rowptr, ftbase_opjmp, 0, 0, 0, 0, _state);
            ftbase_ftcomplexfftplanrec(m, 1, ae_true, ae_true, rowptr, bluesteinsize, precrptr, preciptr, plan, _state);
            row1 = *rowptr;
            plan->entries.ptr.pp_int[row0][ftbase_colparam0] = row1-row0;
            ftbase_ftpushentry(plan, rowptr, ftbase_opend, k, n, 2, 0, _state);
            
            /*
             * Fill precomputed buffer
             */
            ftbase_ftprecomputebluesteinsfft(n, m, &plan->precr, *precrptr, _state);
            
            /*
             * Update pointer to the precomputed area
             */
            *precrptr = *precrptr+4*m;
        }
        else
        {
            
            /*
             * Handle composite FFT with recursive Cooley-Tukey which
             * uses global buffer instead of local one.
             */
            ftbase_ftpushentry2(plan, rowptr, ftbase_opstart, k, n, 2, -1, ftbase_ftoptimisticestimate(n, _state), _state);
            ftbase_ftpushentry(plan, rowptr, ftbase_opcomplextranspose, k, n, 2, n1, _state);
            row0 = *rowptr;
            ftbase_ftpushentry2(plan, rowptr, ftbase_opparallelcall, k*n2, n1, 2, 0, ftbase_ftoptimisticestimate(n, _state), _state);
            ftbase_ftpushentry(plan, rowptr, ftbase_opcomplexfftfactors, k, n, 2, n1, _state);
            ftbase_ftpushentry(plan, rowptr, ftbase_opcomplextranspose, k, n, 2, n2, _state);
            row2 = *rowptr;
            ftbase_ftpushentry2(plan, rowptr, ftbase_opparallelcall, k*n1, n2, 2, 0, ftbase_ftoptimisticestimate(n, _state), _state);
            ftbase_ftpushentry(plan, rowptr, ftbase_opcomplextranspose, k, n, 2, n1, _state);
            ftbase_ftpushentry(plan, rowptr, ftbase_opend, k, n, 2, 0, _state);
            row1 = *rowptr;
            ftbase_ftcomplexfftplanrec(n1, 1, ae_true, ae_false, rowptr, bluesteinsize, precrptr, preciptr, plan, _state);
            plan->entries.ptr.pp_int[row0][ftbase_colparam0] = row1-row0;
            row3 = *rowptr;
            ftbase_ftcomplexfftplanrec(n2, 1, ae_true, ae_false, rowptr, bluesteinsize, precrptr, preciptr, plan, _state);
            plan->entries.ptr.pp_int[row2][ftbase_colparam0] = row3-row2;
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Prepare "non-topmost" plan:
     * * calculate factorization
     * * use local (shared) buffer
     * * update buffer size - ANY plan will need at least
     *   2*N temporaries, additional requirements can be
     *   applied later
     */
    ftbase_ftfactorize(n, ae_false, &n1, &n2, _state);
    
    /*
     * Handle FFT's with N1*N2=0: either small-N or prime-factor
     */
    if( n1*n2==0 )
    {
        if( n<=ftbase_maxradix )
        {
            
            /*
             * Small-N FFT
             */
            if( childplan )
            {
                ftbase_ftpushentry2(plan, rowptr, ftbase_opstart, k, n, 2, -1, ftbase_ftoptimisticestimate(n, _state), _state);
            }
            ftbase_ftpushentry(plan, rowptr, ftbase_opcomplexcodeletfft, k, n, 2, 0, _state);
            if( childplan )
            {
                ftbase_ftpushentry(plan, rowptr, ftbase_opend, k, n, 2, 0, _state);
            }
            ae_frame_leave(_state);
            return;
        }
        if( n<=ftbase_raderthreshold )
        {
            
            /*
             * Handle prime-factor FFT's with Rader's FFT
             */
            m = n-1;
            if( childplan )
            {
                ftbase_ftpushentry2(plan, rowptr, ftbase_opstart, k, n, 2, -1, ftbase_ftoptimisticestimate(n, _state), _state);
            }
            findprimitiverootandinverse(n, &gq, &giq, _state);
            ftbase_ftpushentry4(plan, rowptr, ftbase_opradersfft, k, n, 2, 2, gq, giq, *precrptr, _state);
            ftbase_ftprecomputeradersfft(n, gq, giq, &plan->precr, *precrptr, _state);
            *precrptr = *precrptr+2*(n-1);
            row0 = *rowptr;
            ftbase_ftpushentry(plan, rowptr, ftbase_opjmp, 0, 0, 0, 0, _state);
            ftbase_ftcomplexfftplanrec(m, 1, ae_true, ae_false, rowptr, bluesteinsize, precrptr, preciptr, plan, _state);
            row1 = *rowptr;
            plan->entries.ptr.pp_int[row0][ftbase_colparam0] = row1-row0;
            if( childplan )
            {
                ftbase_ftpushentry(plan, rowptr, ftbase_opend, k, n, 2, 0, _state);
            }
        }
        else
        {
            
            /*
             * Handle prime-factor FFT's with Bluestein's FFT
             */
            m = ftbasefindsmooth(2*n-1, _state);
            *bluesteinsize = ae_maxint(2*m, *bluesteinsize, _state);
            if( childplan )
            {
                ftbase_ftpushentry2(plan, rowptr, ftbase_opstart, k, n, 2, -1, ftbase_ftoptimisticestimate(n, _state), _state);
            }
            ftbase_ftpushentry4(plan, rowptr, ftbase_opbluesteinsfft, k, n, 2, m, 2, *precrptr, 0, _state);
            ftbase_ftprecomputebluesteinsfft(n, m, &plan->precr, *precrptr, _state);
            *precrptr = *precrptr+4*m;
            row0 = *rowptr;
            ftbase_ftpushentry(plan, rowptr, ftbase_opjmp, 0, 0, 0, 0, _state);
            ftbase_ftcomplexfftplanrec(m, 1, ae_true, ae_false, rowptr, bluesteinsize, precrptr, preciptr, plan, _state);
            row1 = *rowptr;
            plan->entries.ptr.pp_int[row0][ftbase_colparam0] = row1-row0;
            if( childplan )
            {
                ftbase_ftpushentry(plan, rowptr, ftbase_opend, k, n, 2, 0, _state);
            }
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Handle Cooley-Tukey FFT with small N1
     */
    if( n1<=ftbase_maxradix )
    {
        
        /*
         * Specialized transformation for small N1:
         * * N2 short inplace FFT's, each N1-point, with integrated twiddle factors
         * * N1 long FFT's
         * * final transposition
         */
        if( childplan )
        {
            ftbase_ftpushentry2(plan, rowptr, ftbase_opstart, k, n, 2, -1, ftbase_ftoptimisticestimate(n, _state), _state);
        }
        ftbase_ftpushentry(plan, rowptr, ftbase_opcomplexcodelettwfft, k, n1, 2*n2, 0, _state);
        ftbase_ftcomplexfftplanrec(n2, k*n1, ae_false, ae_false, rowptr, bluesteinsize, precrptr, preciptr, plan, _state);
        ftbase_ftpushentry(plan, rowptr, ftbase_opcomplextranspose, k, n, 2, n1, _state);
        if( childplan )
        {
            ftbase_ftpushentry(plan, rowptr, ftbase_opend, k, n, 2, 0, _state);
        }
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Handle general Cooley-Tukey FFT, either "flat" or "recursive"
     */
    if( n<=ftbase_recursivethreshold )
    {
        
        /*
         * General code for large N1/N2, "flat" version without explicit recurrence
         * (nested subplans are inserted directly into the body of the plan)
         */
        if( childplan )
        {
            ftbase_ftpushentry2(plan, rowptr, ftbase_opstart, k, n, 2, -1, ftbase_ftoptimisticestimate(n, _state), _state);
        }
        ftbase_ftpushentry(plan, rowptr, ftbase_opcomplextranspose, k, n, 2, n1, _state);
        ftbase_ftcomplexfftplanrec(n1, k*n2, ae_false, ae_false, rowptr, bluesteinsize, precrptr, preciptr, plan, _state);
        ftbase_ftpushentry(plan, rowptr, ftbase_opcomplexfftfactors, k, n, 2, n1, _state);
        ftbase_ftpushentry(plan, rowptr, ftbase_opcomplextranspose, k, n, 2, n2, _state);
        ftbase_ftcomplexfftplanrec(n2, k*n1, ae_false, ae_false, rowptr, bluesteinsize, precrptr, preciptr, plan, _state);
        ftbase_ftpushentry(plan, rowptr, ftbase_opcomplextranspose, k, n, 2, n1, _state);
        if( childplan )
        {
            ftbase_ftpushentry(plan, rowptr, ftbase_opend, k, n, 2, 0, _state);
        }
    }
    else
    {
        
        /*
         * General code for large N1/N2, "recursive" version - nested subplans
         * are separated from the plan body.
         *
         * Generate parent plan.
         */
        if( childplan )
        {
            ftbase_ftpushentry2(plan, rowptr, ftbase_opstart, k, n, 2, -1, ftbase_ftoptimisticestimate(n, _state), _state);
        }
        ftbase_ftpushentry(plan, rowptr, ftbase_opcomplextranspose, k, n, 2, n1, _state);
        row0 = *rowptr;
        ftbase_ftpushentry2(plan, rowptr, ftbase_opparallelcall, k*n2, n1, 2, 0, ftbase_ftoptimisticestimate(n, _state), _state);
        ftbase_ftpushentry(plan, rowptr, ftbase_opcomplexfftfactors, k, n, 2, n1, _state);
        ftbase_ftpushentry(plan, rowptr, ftbase_opcomplextranspose, k, n, 2, n2, _state);
        row2 = *rowptr;
        ftbase_ftpushentry2(plan, rowptr, ftbase_opparallelcall, k*n1, n2, 2, 0, ftbase_ftoptimisticestimate(n, _state), _state);
        ftbase_ftpushentry(plan, rowptr, ftbase_opcomplextranspose, k, n, 2, n1, _state);
        if( childplan )
        {
            ftbase_ftpushentry(plan, rowptr, ftbase_opend, k, n, 2, 0, _state);
        }
        
        /*
         * Generate child subplans, insert refence to parent plans
         */
        row1 = *rowptr;
        ftbase_ftcomplexfftplanrec(n1, 1, ae_true, ae_false, rowptr, bluesteinsize, precrptr, preciptr, plan, _state);
        plan->entries.ptr.pp_int[row0][ftbase_colparam0] = row1-row0;
        row3 = *rowptr;
        ftbase_ftcomplexfftplanrec(n2, 1, ae_true, ae_false, rowptr, bluesteinsize, precrptr, preciptr, plan, _state);
        plan->entries.ptr.pp_int[row2][ftbase_colparam0] = row3-row2;
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This function pushes one more entry to the plan. It resizes Entries matrix
if needed.

INPUT PARAMETERS:
    Plan        -   plan (generated so far)
    RowPtr      -   index which points to past-the-last entry generated so far
    EType       -   entry type
    EOpCnt      -   operands count
    EOpSize     -   operand size
    EMcvSize    -   microvector size
    EParam0     -   parameter 0
    
OUTPUT PARAMETERS:
    Plan        -   updated plan    
    RowPtr      -   updated pointer

NOTE: Param1 is set to -1.
    
  -- ALGLIB --
     Copyright 05.04.2013 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ftpushentry(fasttransformplan* plan,
     ae_int_t* rowptr,
     ae_int_t etype,
     ae_int_t eopcnt,
     ae_int_t eopsize,
     ae_int_t emcvsize,
     ae_int_t eparam0,
     ae_state *_state)
{


    ftbase_ftpushentry2(plan, rowptr, etype, eopcnt, eopsize, emcvsize, eparam0, -1, _state);
}


/*************************************************************************
Same as FTPushEntry(), but sets Param0 AND Param1.
This function pushes one more entry to the plan. It resized Entries matrix
if needed.

INPUT PARAMETERS:
    Plan        -   plan (generated so far)
    RowPtr      -   index which points to past-the-last entry generated so far
    EType       -   entry type
    EOpCnt      -   operands count
    EOpSize     -   operand size
    EMcvSize    -   microvector size
    EParam0     -   parameter 0
    EParam1     -   parameter 1
    
OUTPUT PARAMETERS:
    Plan        -   updated plan    
    RowPtr      -   updated pointer

  -- ALGLIB --
     Copyright 05.04.2013 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ftpushentry2(fasttransformplan* plan,
     ae_int_t* rowptr,
     ae_int_t etype,
     ae_int_t eopcnt,
     ae_int_t eopsize,
     ae_int_t emcvsize,
     ae_int_t eparam0,
     ae_int_t eparam1,
     ae_state *_state)
{


    if( *rowptr>=plan->entries.rows )
    {
        imatrixresize(&plan->entries, ae_maxint(2*plan->entries.rows, 1, _state), ftbase_colscnt, _state);
    }
    plan->entries.ptr.pp_int[*rowptr][ftbase_coltype] = etype;
    plan->entries.ptr.pp_int[*rowptr][ftbase_coloperandscnt] = eopcnt;
    plan->entries.ptr.pp_int[*rowptr][ftbase_coloperandsize] = eopsize;
    plan->entries.ptr.pp_int[*rowptr][ftbase_colmicrovectorsize] = emcvsize;
    plan->entries.ptr.pp_int[*rowptr][ftbase_colparam0] = eparam0;
    plan->entries.ptr.pp_int[*rowptr][ftbase_colparam1] = eparam1;
    plan->entries.ptr.pp_int[*rowptr][ftbase_colparam2] = 0;
    plan->entries.ptr.pp_int[*rowptr][ftbase_colparam3] = 0;
    *rowptr = *rowptr+1;
}


/*************************************************************************
Same as FTPushEntry(), but sets Param0, Param1, Param2 and Param3.
This function pushes one more entry to the plan. It resized Entries matrix
if needed.

INPUT PARAMETERS:
    Plan        -   plan (generated so far)
    RowPtr      -   index which points to past-the-last entry generated so far
    EType       -   entry type
    EOpCnt      -   operands count
    EOpSize     -   operand size
    EMcvSize    -   microvector size
    EParam0     -   parameter 0
    EParam1     -   parameter 1
    EParam2     -   parameter 2
    EParam3     -   parameter 3
    
OUTPUT PARAMETERS:
    Plan        -   updated plan    
    RowPtr      -   updated pointer

  -- ALGLIB --
     Copyright 05.04.2013 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ftpushentry4(fasttransformplan* plan,
     ae_int_t* rowptr,
     ae_int_t etype,
     ae_int_t eopcnt,
     ae_int_t eopsize,
     ae_int_t emcvsize,
     ae_int_t eparam0,
     ae_int_t eparam1,
     ae_int_t eparam2,
     ae_int_t eparam3,
     ae_state *_state)
{


    if( *rowptr>=plan->entries.rows )
    {
        imatrixresize(&plan->entries, ae_maxint(2*plan->entries.rows, 1, _state), ftbase_colscnt, _state);
    }
    plan->entries.ptr.pp_int[*rowptr][ftbase_coltype] = etype;
    plan->entries.ptr.pp_int[*rowptr][ftbase_coloperandscnt] = eopcnt;
    plan->entries.ptr.pp_int[*rowptr][ftbase_coloperandsize] = eopsize;
    plan->entries.ptr.pp_int[*rowptr][ftbase_colmicrovectorsize] = emcvsize;
    plan->entries.ptr.pp_int[*rowptr][ftbase_colparam0] = eparam0;
    plan->entries.ptr.pp_int[*rowptr][ftbase_colparam1] = eparam1;
    plan->entries.ptr.pp_int[*rowptr][ftbase_colparam2] = eparam2;
    plan->entries.ptr.pp_int[*rowptr][ftbase_colparam3] = eparam3;
    *rowptr = *rowptr+1;
}


/*************************************************************************
This subroutine applies subplan to input/output array A.

INPUT PARAMETERS:
    Plan        -   transformation plan
    SubPlan     -   subplan index
    A           -   array, must be large enough for plan to work
    ABase       -   base offset in array A, this value points to start of
                    subarray whose length is equal to length of the plan
    AOffset     -   offset with respect to ABase, 0<=AOffset<PlanLength.
                    This is an offset within large PlanLength-subarray of
                    the chunk to process.
    Buf         -   temporary buffer whose length is equal to plan length
                    (without taking into account RepCnt) or larger.
    OffsBuf     -   offset in the buffer array
    RepCnt      -   repetition count (transformation is repeatedly applied
                    to subsequent subarrays)
    
OUTPUT PARAMETERS:
    Plan        -   plan (temporary buffers can be modified, plan itself
                    is unchanged and can be reused)
    A           -   transformed array

  -- ALGLIB --
     Copyright 05.04.2013 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ftapplysubplan(fasttransformplan* plan,
     ae_int_t subplan,
     /* Real    */ ae_vector* a,
     ae_int_t abase,
     ae_int_t aoffset,
     /* Real    */ ae_vector* buf,
     ae_int_t repcnt,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t rowidx;
    ae_int_t i;
    ae_int_t n1;
    ae_int_t n2;
    ae_int_t operation;
    ae_int_t operandscnt;
    ae_int_t operandsize;
    ae_int_t microvectorsize;
    ae_int_t param0;
    ae_int_t param1;
    ae_int_t parentsize;
    ae_int_t childsize;
    ae_int_t chunksize;
    ae_int_t lastchunksize;
    srealarray *bufa;
    ae_smart_ptr _bufa;
    srealarray *bufb;
    ae_smart_ptr _bufb;
    srealarray *bufc;
    ae_smart_ptr _bufc;
    srealarray *bufd;
    ae_smart_ptr _bufd;

    ae_frame_make(_state, &_frame_block);
    ae_smart_ptr_init(&_bufa, (void**)&bufa, _state);
    ae_smart_ptr_init(&_bufb, (void**)&bufb, _state);
    ae_smart_ptr_init(&_bufc, (void**)&bufc, _state);
    ae_smart_ptr_init(&_bufd, (void**)&bufd, _state);

    ae_assert(plan->entries.ptr.pp_int[subplan][ftbase_coltype]==ftbase_opstart, "FTApplySubPlan: incorrect subplan header", _state);
    rowidx = subplan+1;
    while(plan->entries.ptr.pp_int[rowidx][ftbase_coltype]!=ftbase_opend)
    {
        operation = plan->entries.ptr.pp_int[rowidx][ftbase_coltype];
        operandscnt = repcnt*plan->entries.ptr.pp_int[rowidx][ftbase_coloperandscnt];
        operandsize = plan->entries.ptr.pp_int[rowidx][ftbase_coloperandsize];
        microvectorsize = plan->entries.ptr.pp_int[rowidx][ftbase_colmicrovectorsize];
        param0 = plan->entries.ptr.pp_int[rowidx][ftbase_colparam0];
        param1 = plan->entries.ptr.pp_int[rowidx][ftbase_colparam1];
        touchint(&param1, _state);
        
        /*
         * Process "jump" operation
         */
        if( operation==ftbase_opjmp )
        {
            rowidx = rowidx+plan->entries.ptr.pp_int[rowidx][ftbase_colparam0];
            continue;
        }
        
        /*
         * Process "parallel call" operation:
         * * we perform initial check for consistency between parent and child plans
         * * we call FTSplitAndApplyParallelPlan(), which splits parallel plan into
         *   several parallel tasks
         */
        if( operation==ftbase_opparallelcall )
        {
            parentsize = operandsize*microvectorsize;
            childsize = plan->entries.ptr.pp_int[rowidx+param0][ftbase_coloperandscnt]*plan->entries.ptr.pp_int[rowidx+param0][ftbase_coloperandsize]*plan->entries.ptr.pp_int[rowidx+param0][ftbase_colmicrovectorsize];
            ae_assert(plan->entries.ptr.pp_int[rowidx+param0][ftbase_coltype]==ftbase_opstart, "FTApplySubPlan: incorrect child subplan header", _state);
            ae_assert(parentsize==childsize, "FTApplySubPlan: incorrect child subplan header", _state);
            chunksize = ae_maxint(ftbase_recursivethreshold/childsize, 1, _state);
            lastchunksize = operandscnt%chunksize;
            if( lastchunksize==0 )
            {
                lastchunksize = chunksize;
            }
            i = 0;
            while(i<operandscnt)
            {
                chunksize = ae_minint(chunksize, operandscnt-i, _state);
                ftbase_ftapplysubplan(plan, rowidx+param0, a, abase, aoffset+i*childsize, buf, chunksize, _state);
                i = i+chunksize;
            }
            rowidx = rowidx+1;
            continue;
        }
        
        /*
         * Process "reference complex FFT" operation
         */
        if( operation==ftbase_opcomplexreffft )
        {
            ftbase_ftapplycomplexreffft(a, abase+aoffset, operandscnt, operandsize, microvectorsize, buf, _state);
            rowidx = rowidx+1;
            continue;
        }
        
        /*
         * Process "codelet FFT" operation
         */
        if( operation==ftbase_opcomplexcodeletfft )
        {
            ftbase_ftapplycomplexcodeletfft(a, abase+aoffset, operandscnt, operandsize, microvectorsize, _state);
            rowidx = rowidx+1;
            continue;
        }
        
        /*
         * Process "integrated codelet FFT" operation
         */
        if( operation==ftbase_opcomplexcodelettwfft )
        {
            ftbase_ftapplycomplexcodelettwfft(a, abase+aoffset, operandscnt, operandsize, microvectorsize, _state);
            rowidx = rowidx+1;
            continue;
        }
        
        /*
         * Process Bluestein's FFT operation
         */
        if( operation==ftbase_opbluesteinsfft )
        {
            ae_assert(microvectorsize==2, "FTApplySubPlan: microvectorsize!=2 for Bluesteins FFT", _state);
            ae_shared_pool_retrieve(&plan->bluesteinpool, &_bufa, _state);
            ae_shared_pool_retrieve(&plan->bluesteinpool, &_bufb, _state);
            ae_shared_pool_retrieve(&plan->bluesteinpool, &_bufc, _state);
            ae_shared_pool_retrieve(&plan->bluesteinpool, &_bufd, _state);
            ftbase_ftbluesteinsfft(plan, a, abase, aoffset, operandscnt, operandsize, plan->entries.ptr.pp_int[rowidx][ftbase_colparam0], plan->entries.ptr.pp_int[rowidx][ftbase_colparam2], rowidx+plan->entries.ptr.pp_int[rowidx][ftbase_colparam1], &bufa->val, &bufb->val, &bufc->val, &bufd->val, _state);
            ae_shared_pool_recycle(&plan->bluesteinpool, &_bufa, _state);
            ae_shared_pool_recycle(&plan->bluesteinpool, &_bufb, _state);
            ae_shared_pool_recycle(&plan->bluesteinpool, &_bufc, _state);
            ae_shared_pool_recycle(&plan->bluesteinpool, &_bufd, _state);
            rowidx = rowidx+1;
            continue;
        }
        
        /*
         * Process Rader's FFT
         */
        if( operation==ftbase_opradersfft )
        {
            ftbase_ftradersfft(plan, a, abase, aoffset, operandscnt, operandsize, rowidx+plan->entries.ptr.pp_int[rowidx][ftbase_colparam0], plan->entries.ptr.pp_int[rowidx][ftbase_colparam1], plan->entries.ptr.pp_int[rowidx][ftbase_colparam2], plan->entries.ptr.pp_int[rowidx][ftbase_colparam3], buf, _state);
            rowidx = rowidx+1;
            continue;
        }
        
        /*
         * Process "complex twiddle factors" operation
         */
        if( operation==ftbase_opcomplexfftfactors )
        {
            ae_assert(microvectorsize==2, "FTApplySubPlan: MicrovectorSize<>1", _state);
            n1 = plan->entries.ptr.pp_int[rowidx][ftbase_colparam0];
            n2 = operandsize/n1;
            for(i=0; i<=operandscnt-1; i++)
            {
                ftbase_ffttwcalc(a, abase+aoffset+i*operandsize*2, n1, n2, _state);
            }
            rowidx = rowidx+1;
            continue;
        }
        
        /*
         * Process "complex transposition" operation
         */
        if( operation==ftbase_opcomplextranspose )
        {
            ae_assert(microvectorsize==2, "FTApplySubPlan: MicrovectorSize<>1", _state);
            n1 = plan->entries.ptr.pp_int[rowidx][ftbase_colparam0];
            n2 = operandsize/n1;
            for(i=0; i<=operandscnt-1; i++)
            {
                ftbase_internalcomplexlintranspose(a, n1, n2, abase+aoffset+i*operandsize*2, buf, _state);
            }
            rowidx = rowidx+1;
            continue;
        }
        
        /*
         * Error
         */
        ae_assert(ae_false, "FTApplySubPlan: unexpected plan type", _state);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This subroutine applies complex reference FFT to input/output array A.

VERY SLOW OPERATION, do not use it in real life plans :)

INPUT PARAMETERS:
    A           -   array, must be large enough for plan to work
    Offs        -   offset of the subarray to process
    OperandsCnt -   operands count (see description of FastTransformPlan)
    OperandSize -   operand size (see description of FastTransformPlan)
    MicrovectorSize-microvector size (see description of FastTransformPlan)
    Buf         -   temporary array, must be at least OperandsCnt*OperandSize*MicrovectorSize
    
OUTPUT PARAMETERS:
    A           -   transformed array

  -- ALGLIB --
     Copyright 05.04.2013 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ftapplycomplexreffft(/* Real    */ ae_vector* a,
     ae_int_t offs,
     ae_int_t operandscnt,
     ae_int_t operandsize,
     ae_int_t microvectorsize,
     /* Real    */ ae_vector* buf,
     ae_state *_state)
{
    ae_int_t opidx;
    ae_int_t i;
    ae_int_t k;
    double hre;
    double him;
    double c;
    double s;
    double re;
    double im;
    ae_int_t n;


    ae_assert(operandscnt>=1, "FTApplyComplexRefFFT: OperandsCnt<1", _state);
    ae_assert(operandsize>=1, "FTApplyComplexRefFFT: OperandSize<1", _state);
    ae_assert(microvectorsize==2, "FTApplyComplexRefFFT: MicrovectorSize<>2", _state);
    n = operandsize;
    for(opidx=0; opidx<=operandscnt-1; opidx++)
    {
        for(i=0; i<=n-1; i++)
        {
            hre = (double)(0);
            him = (double)(0);
            for(k=0; k<=n-1; k++)
            {
                re = a->ptr.p_double[offs+opidx*operandsize*2+2*k+0];
                im = a->ptr.p_double[offs+opidx*operandsize*2+2*k+1];
                c = ae_cos(-2*ae_pi*k*i/n, _state);
                s = ae_sin(-2*ae_pi*k*i/n, _state);
                hre = hre+c*re-s*im;
                him = him+c*im+s*re;
            }
            buf->ptr.p_double[2*i+0] = hre;
            buf->ptr.p_double[2*i+1] = him;
        }
        for(i=0; i<=operandsize*2-1; i++)
        {
            a->ptr.p_double[offs+opidx*operandsize*2+i] = buf->ptr.p_double[i];
        }
    }
}


/*************************************************************************
This subroutine applies complex codelet FFT to input/output array A.

INPUT PARAMETERS:
    A           -   array, must be large enough for plan to work
    Offs        -   offset of the subarray to process
    OperandsCnt -   operands count (see description of FastTransformPlan)
    OperandSize -   operand size (see description of FastTransformPlan)
    MicrovectorSize-microvector size, must be 2
    
OUTPUT PARAMETERS:
    A           -   transformed array

  -- ALGLIB --
     Copyright 05.04.2013 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ftapplycomplexcodeletfft(/* Real    */ ae_vector* a,
     ae_int_t offs,
     ae_int_t operandscnt,
     ae_int_t operandsize,
     ae_int_t microvectorsize,
     ae_state *_state)
{
    ae_int_t opidx;
    ae_int_t n;
    ae_int_t aoffset;
    double a0x;
    double a0y;
    double a1x;
    double a1y;
    double a2x;
    double a2y;
    double a3x;
    double a3y;
    double a4x;
    double a4y;
    double a5x;
    double a5y;
    double v0;
    double v1;
    double v2;
    double v3;
    double t1x;
    double t1y;
    double t2x;
    double t2y;
    double t3x;
    double t3y;
    double t4x;
    double t4y;
    double t5x;
    double t5y;
    double m1x;
    double m1y;
    double m2x;
    double m2y;
    double m3x;
    double m3y;
    double m4x;
    double m4y;
    double m5x;
    double m5y;
    double s1x;
    double s1y;
    double s2x;
    double s2y;
    double s3x;
    double s3y;
    double s4x;
    double s4y;
    double s5x;
    double s5y;
    double c1;
    double c2;
    double c3;
    double c4;
    double c5;
    double v;


    ae_assert(operandscnt>=1, "FTApplyComplexCodeletFFT: OperandsCnt<1", _state);
    ae_assert(operandsize>=1, "FTApplyComplexCodeletFFT: OperandSize<1", _state);
    ae_assert(microvectorsize==2, "FTApplyComplexCodeletFFT: MicrovectorSize<>2", _state);
    n = operandsize;
    
    /*
     * Hard-coded transforms for different N's
     */
    ae_assert(n<=ftbase_maxradix, "FTApplyComplexCodeletFFT: N>MaxRadix", _state);
    if( n==2 )
    {
        for(opidx=0; opidx<=operandscnt-1; opidx++)
        {
            aoffset = offs+opidx*operandsize*2;
            a0x = a->ptr.p_double[aoffset+0];
            a0y = a->ptr.p_double[aoffset+1];
            a1x = a->ptr.p_double[aoffset+2];
            a1y = a->ptr.p_double[aoffset+3];
            v0 = a0x+a1x;
            v1 = a0y+a1y;
            v2 = a0x-a1x;
            v3 = a0y-a1y;
            a->ptr.p_double[aoffset+0] = v0;
            a->ptr.p_double[aoffset+1] = v1;
            a->ptr.p_double[aoffset+2] = v2;
            a->ptr.p_double[aoffset+3] = v3;
        }
        return;
    }
    if( n==3 )
    {
        c1 = ae_cos(2*ae_pi/3, _state)-1;
        c2 = ae_sin(2*ae_pi/3, _state);
        for(opidx=0; opidx<=operandscnt-1; opidx++)
        {
            aoffset = offs+opidx*operandsize*2;
            a0x = a->ptr.p_double[aoffset+0];
            a0y = a->ptr.p_double[aoffset+1];
            a1x = a->ptr.p_double[aoffset+2];
            a1y = a->ptr.p_double[aoffset+3];
            a2x = a->ptr.p_double[aoffset+4];
            a2y = a->ptr.p_double[aoffset+5];
            t1x = a1x+a2x;
            t1y = a1y+a2y;
            a0x = a0x+t1x;
            a0y = a0y+t1y;
            m1x = c1*t1x;
            m1y = c1*t1y;
            m2x = c2*(a1y-a2y);
            m2y = c2*(a2x-a1x);
            s1x = a0x+m1x;
            s1y = a0y+m1y;
            a1x = s1x+m2x;
            a1y = s1y+m2y;
            a2x = s1x-m2x;
            a2y = s1y-m2y;
            a->ptr.p_double[aoffset+0] = a0x;
            a->ptr.p_double[aoffset+1] = a0y;
            a->ptr.p_double[aoffset+2] = a1x;
            a->ptr.p_double[aoffset+3] = a1y;
            a->ptr.p_double[aoffset+4] = a2x;
            a->ptr.p_double[aoffset+5] = a2y;
        }
        return;
    }
    if( n==4 )
    {
        for(opidx=0; opidx<=operandscnt-1; opidx++)
        {
            aoffset = offs+opidx*operandsize*2;
            a0x = a->ptr.p_double[aoffset+0];
            a0y = a->ptr.p_double[aoffset+1];
            a1x = a->ptr.p_double[aoffset+2];
            a1y = a->ptr.p_double[aoffset+3];
            a2x = a->ptr.p_double[aoffset+4];
            a2y = a->ptr.p_double[aoffset+5];
            a3x = a->ptr.p_double[aoffset+6];
            a3y = a->ptr.p_double[aoffset+7];
            t1x = a0x+a2x;
            t1y = a0y+a2y;
            t2x = a1x+a3x;
            t2y = a1y+a3y;
            m2x = a0x-a2x;
            m2y = a0y-a2y;
            m3x = a1y-a3y;
            m3y = a3x-a1x;
            a->ptr.p_double[aoffset+0] = t1x+t2x;
            a->ptr.p_double[aoffset+1] = t1y+t2y;
            a->ptr.p_double[aoffset+4] = t1x-t2x;
            a->ptr.p_double[aoffset+5] = t1y-t2y;
            a->ptr.p_double[aoffset+2] = m2x+m3x;
            a->ptr.p_double[aoffset+3] = m2y+m3y;
            a->ptr.p_double[aoffset+6] = m2x-m3x;
            a->ptr.p_double[aoffset+7] = m2y-m3y;
        }
        return;
    }
    if( n==5 )
    {
        v = 2*ae_pi/5;
        c1 = (ae_cos(v, _state)+ae_cos(2*v, _state))/2-1;
        c2 = (ae_cos(v, _state)-ae_cos(2*v, _state))/2;
        c3 = -ae_sin(v, _state);
        c4 = -(ae_sin(v, _state)+ae_sin(2*v, _state));
        c5 = ae_sin(v, _state)-ae_sin(2*v, _state);
        for(opidx=0; opidx<=operandscnt-1; opidx++)
        {
            aoffset = offs+opidx*operandsize*2;
            t1x = a->ptr.p_double[aoffset+2]+a->ptr.p_double[aoffset+8];
            t1y = a->ptr.p_double[aoffset+3]+a->ptr.p_double[aoffset+9];
            t2x = a->ptr.p_double[aoffset+4]+a->ptr.p_double[aoffset+6];
            t2y = a->ptr.p_double[aoffset+5]+a->ptr.p_double[aoffset+7];
            t3x = a->ptr.p_double[aoffset+2]-a->ptr.p_double[aoffset+8];
            t3y = a->ptr.p_double[aoffset+3]-a->ptr.p_double[aoffset+9];
            t4x = a->ptr.p_double[aoffset+6]-a->ptr.p_double[aoffset+4];
            t4y = a->ptr.p_double[aoffset+7]-a->ptr.p_double[aoffset+5];
            t5x = t1x+t2x;
            t5y = t1y+t2y;
            a->ptr.p_double[aoffset+0] = a->ptr.p_double[aoffset+0]+t5x;
            a->ptr.p_double[aoffset+1] = a->ptr.p_double[aoffset+1]+t5y;
            m1x = c1*t5x;
            m1y = c1*t5y;
            m2x = c2*(t1x-t2x);
            m2y = c2*(t1y-t2y);
            m3x = -c3*(t3y+t4y);
            m3y = c3*(t3x+t4x);
            m4x = -c4*t4y;
            m4y = c4*t4x;
            m5x = -c5*t3y;
            m5y = c5*t3x;
            s3x = m3x-m4x;
            s3y = m3y-m4y;
            s5x = m3x+m5x;
            s5y = m3y+m5y;
            s1x = a->ptr.p_double[aoffset+0]+m1x;
            s1y = a->ptr.p_double[aoffset+1]+m1y;
            s2x = s1x+m2x;
            s2y = s1y+m2y;
            s4x = s1x-m2x;
            s4y = s1y-m2y;
            a->ptr.p_double[aoffset+2] = s2x+s3x;
            a->ptr.p_double[aoffset+3] = s2y+s3y;
            a->ptr.p_double[aoffset+4] = s4x+s5x;
            a->ptr.p_double[aoffset+5] = s4y+s5y;
            a->ptr.p_double[aoffset+6] = s4x-s5x;
            a->ptr.p_double[aoffset+7] = s4y-s5y;
            a->ptr.p_double[aoffset+8] = s2x-s3x;
            a->ptr.p_double[aoffset+9] = s2y-s3y;
        }
        return;
    }
    if( n==6 )
    {
        c1 = ae_cos(2*ae_pi/3, _state)-1;
        c2 = ae_sin(2*ae_pi/3, _state);
        c3 = ae_cos(-ae_pi/3, _state);
        c4 = ae_sin(-ae_pi/3, _state);
        for(opidx=0; opidx<=operandscnt-1; opidx++)
        {
            aoffset = offs+opidx*operandsize*2;
            a0x = a->ptr.p_double[aoffset+0];
            a0y = a->ptr.p_double[aoffset+1];
            a1x = a->ptr.p_double[aoffset+2];
            a1y = a->ptr.p_double[aoffset+3];
            a2x = a->ptr.p_double[aoffset+4];
            a2y = a->ptr.p_double[aoffset+5];
            a3x = a->ptr.p_double[aoffset+6];
            a3y = a->ptr.p_double[aoffset+7];
            a4x = a->ptr.p_double[aoffset+8];
            a4y = a->ptr.p_double[aoffset+9];
            a5x = a->ptr.p_double[aoffset+10];
            a5y = a->ptr.p_double[aoffset+11];
            v0 = a0x;
            v1 = a0y;
            a0x = a0x+a3x;
            a0y = a0y+a3y;
            a3x = v0-a3x;
            a3y = v1-a3y;
            v0 = a1x;
            v1 = a1y;
            a1x = a1x+a4x;
            a1y = a1y+a4y;
            a4x = v0-a4x;
            a4y = v1-a4y;
            v0 = a2x;
            v1 = a2y;
            a2x = a2x+a5x;
            a2y = a2y+a5y;
            a5x = v0-a5x;
            a5y = v1-a5y;
            t4x = a4x*c3-a4y*c4;
            t4y = a4x*c4+a4y*c3;
            a4x = t4x;
            a4y = t4y;
            t5x = -a5x*c3-a5y*c4;
            t5y = a5x*c4-a5y*c3;
            a5x = t5x;
            a5y = t5y;
            t1x = a1x+a2x;
            t1y = a1y+a2y;
            a0x = a0x+t1x;
            a0y = a0y+t1y;
            m1x = c1*t1x;
            m1y = c1*t1y;
            m2x = c2*(a1y-a2y);
            m2y = c2*(a2x-a1x);
            s1x = a0x+m1x;
            s1y = a0y+m1y;
            a1x = s1x+m2x;
            a1y = s1y+m2y;
            a2x = s1x-m2x;
            a2y = s1y-m2y;
            t1x = a4x+a5x;
            t1y = a4y+a5y;
            a3x = a3x+t1x;
            a3y = a3y+t1y;
            m1x = c1*t1x;
            m1y = c1*t1y;
            m2x = c2*(a4y-a5y);
            m2y = c2*(a5x-a4x);
            s1x = a3x+m1x;
            s1y = a3y+m1y;
            a4x = s1x+m2x;
            a4y = s1y+m2y;
            a5x = s1x-m2x;
            a5y = s1y-m2y;
            a->ptr.p_double[aoffset+0] = a0x;
            a->ptr.p_double[aoffset+1] = a0y;
            a->ptr.p_double[aoffset+2] = a3x;
            a->ptr.p_double[aoffset+3] = a3y;
            a->ptr.p_double[aoffset+4] = a1x;
            a->ptr.p_double[aoffset+5] = a1y;
            a->ptr.p_double[aoffset+6] = a4x;
            a->ptr.p_double[aoffset+7] = a4y;
            a->ptr.p_double[aoffset+8] = a2x;
            a->ptr.p_double[aoffset+9] = a2y;
            a->ptr.p_double[aoffset+10] = a5x;
            a->ptr.p_double[aoffset+11] = a5y;
        }
        return;
    }
}


/*************************************************************************
This subroutine applies complex "integrated" codelet FFT  to  input/output
array A. "Integrated" codelet differs from "normal" one in following ways:
* it can work with MicrovectorSize>1
* hence, it can be used in Cooley-Tukey FFT without transpositions
* it performs inlined multiplication by twiddle factors of Cooley-Tukey
  FFT with N2=MicrovectorSize/2.

INPUT PARAMETERS:
    A           -   array, must be large enough for plan to work
    Offs        -   offset of the subarray to process
    OperandsCnt -   operands count (see description of FastTransformPlan)
    OperandSize -   operand size (see description of FastTransformPlan)
    MicrovectorSize-microvector size, must be 1
    
OUTPUT PARAMETERS:
    A           -   transformed array

  -- ALGLIB --
     Copyright 05.04.2013 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ftapplycomplexcodelettwfft(/* Real    */ ae_vector* a,
     ae_int_t offs,
     ae_int_t operandscnt,
     ae_int_t operandsize,
     ae_int_t microvectorsize,
     ae_state *_state)
{
    ae_int_t opidx;
    ae_int_t mvidx;
    ae_int_t n;
    ae_int_t m;
    ae_int_t aoffset0;
    ae_int_t aoffset2;
    ae_int_t aoffset4;
    ae_int_t aoffset6;
    ae_int_t aoffset8;
    ae_int_t aoffset10;
    double a0x;
    double a0y;
    double a1x;
    double a1y;
    double a2x;
    double a2y;
    double a3x;
    double a3y;
    double a4x;
    double a4y;
    double a5x;
    double a5y;
    double v0;
    double v1;
    double v2;
    double v3;
    double q0x;
    double q0y;
    double t1x;
    double t1y;
    double t2x;
    double t2y;
    double t3x;
    double t3y;
    double t4x;
    double t4y;
    double t5x;
    double t5y;
    double m1x;
    double m1y;
    double m2x;
    double m2y;
    double m3x;
    double m3y;
    double m4x;
    double m4y;
    double m5x;
    double m5y;
    double s1x;
    double s1y;
    double s2x;
    double s2y;
    double s3x;
    double s3y;
    double s4x;
    double s4y;
    double s5x;
    double s5y;
    double c1;
    double c2;
    double c3;
    double c4;
    double c5;
    double v;
    double tw0;
    double tw1;
    double twx;
    double twxm1;
    double twy;
    double tw2x;
    double tw2y;
    double tw3x;
    double tw3y;
    double tw4x;
    double tw4y;
    double tw5x;
    double tw5y;


    ae_assert(operandscnt>=1, "FTApplyComplexCodeletFFT: OperandsCnt<1", _state);
    ae_assert(operandsize>=1, "FTApplyComplexCodeletFFT: OperandSize<1", _state);
    ae_assert(microvectorsize>=1, "FTApplyComplexCodeletFFT: MicrovectorSize<>1", _state);
    ae_assert(microvectorsize%2==0, "FTApplyComplexCodeletFFT: MicrovectorSize is not even", _state);
    n = operandsize;
    m = microvectorsize/2;
    
    /*
     * Hard-coded transforms for different N's
     */
    ae_assert(n<=ftbase_maxradix, "FTApplyComplexCodeletTwFFT: N>MaxRadix", _state);
    if( n==2 )
    {
        v = -2*ae_pi/(n*m);
        tw0 = -2*ae_sqr(ae_sin(0.5*v, _state), _state);
        tw1 = ae_sin(v, _state);
        for(opidx=0; opidx<=operandscnt-1; opidx++)
        {
            aoffset0 = offs+opidx*operandsize*microvectorsize;
            aoffset2 = aoffset0+microvectorsize;
            twxm1 = 0.0;
            twy = 0.0;
            for(mvidx=0; mvidx<=m-1; mvidx++)
            {
                a0x = a->ptr.p_double[aoffset0];
                a0y = a->ptr.p_double[aoffset0+1];
                a1x = a->ptr.p_double[aoffset2];
                a1y = a->ptr.p_double[aoffset2+1];
                v0 = a0x+a1x;
                v1 = a0y+a1y;
                v2 = a0x-a1x;
                v3 = a0y-a1y;
                a->ptr.p_double[aoffset0] = v0;
                a->ptr.p_double[aoffset0+1] = v1;
                a->ptr.p_double[aoffset2] = v2*(1+twxm1)-v3*twy;
                a->ptr.p_double[aoffset2+1] = v3*(1+twxm1)+v2*twy;
                aoffset0 = aoffset0+2;
                aoffset2 = aoffset2+2;
                if( (mvidx+1)%ftbase_updatetw==0 )
                {
                    v = -2*ae_pi*(mvidx+1)/(n*m);
                    twxm1 = ae_sin(0.5*v, _state);
                    twxm1 = -2*twxm1*twxm1;
                    twy = ae_sin(v, _state);
                }
                else
                {
                    v = twxm1+tw0+twxm1*tw0-twy*tw1;
                    twy = twy+tw1+twxm1*tw1+twy*tw0;
                    twxm1 = v;
                }
            }
        }
        return;
    }
    if( n==3 )
    {
        v = -2*ae_pi/(n*m);
        tw0 = -2*ae_sqr(ae_sin(0.5*v, _state), _state);
        tw1 = ae_sin(v, _state);
        c1 = ae_cos(2*ae_pi/3, _state)-1;
        c2 = ae_sin(2*ae_pi/3, _state);
        for(opidx=0; opidx<=operandscnt-1; opidx++)
        {
            aoffset0 = offs+opidx*operandsize*microvectorsize;
            aoffset2 = aoffset0+microvectorsize;
            aoffset4 = aoffset2+microvectorsize;
            twx = 1.0;
            twxm1 = 0.0;
            twy = 0.0;
            for(mvidx=0; mvidx<=m-1; mvidx++)
            {
                a0x = a->ptr.p_double[aoffset0];
                a0y = a->ptr.p_double[aoffset0+1];
                a1x = a->ptr.p_double[aoffset2];
                a1y = a->ptr.p_double[aoffset2+1];
                a2x = a->ptr.p_double[aoffset4];
                a2y = a->ptr.p_double[aoffset4+1];
                t1x = a1x+a2x;
                t1y = a1y+a2y;
                a0x = a0x+t1x;
                a0y = a0y+t1y;
                m1x = c1*t1x;
                m1y = c1*t1y;
                m2x = c2*(a1y-a2y);
                m2y = c2*(a2x-a1x);
                s1x = a0x+m1x;
                s1y = a0y+m1y;
                a1x = s1x+m2x;
                a1y = s1y+m2y;
                a2x = s1x-m2x;
                a2y = s1y-m2y;
                tw2x = twx*twx-twy*twy;
                tw2y = 2*twx*twy;
                a->ptr.p_double[aoffset0] = a0x;
                a->ptr.p_double[aoffset0+1] = a0y;
                a->ptr.p_double[aoffset2] = a1x*twx-a1y*twy;
                a->ptr.p_double[aoffset2+1] = a1y*twx+a1x*twy;
                a->ptr.p_double[aoffset4] = a2x*tw2x-a2y*tw2y;
                a->ptr.p_double[aoffset4+1] = a2y*tw2x+a2x*tw2y;
                aoffset0 = aoffset0+2;
                aoffset2 = aoffset2+2;
                aoffset4 = aoffset4+2;
                if( (mvidx+1)%ftbase_updatetw==0 )
                {
                    v = -2*ae_pi*(mvidx+1)/(n*m);
                    twxm1 = ae_sin(0.5*v, _state);
                    twxm1 = -2*twxm1*twxm1;
                    twy = ae_sin(v, _state);
                    twx = twxm1+1;
                }
                else
                {
                    v = twxm1+tw0+twxm1*tw0-twy*tw1;
                    twy = twy+tw1+twxm1*tw1+twy*tw0;
                    twxm1 = v;
                    twx = v+1;
                }
            }
        }
        return;
    }
    if( n==4 )
    {
        v = -2*ae_pi/(n*m);
        tw0 = -2*ae_sqr(ae_sin(0.5*v, _state), _state);
        tw1 = ae_sin(v, _state);
        for(opidx=0; opidx<=operandscnt-1; opidx++)
        {
            aoffset0 = offs+opidx*operandsize*microvectorsize;
            aoffset2 = aoffset0+microvectorsize;
            aoffset4 = aoffset2+microvectorsize;
            aoffset6 = aoffset4+microvectorsize;
            twx = 1.0;
            twxm1 = 0.0;
            twy = 0.0;
            for(mvidx=0; mvidx<=m-1; mvidx++)
            {
                a0x = a->ptr.p_double[aoffset0];
                a0y = a->ptr.p_double[aoffset0+1];
                a1x = a->ptr.p_double[aoffset2];
                a1y = a->ptr.p_double[aoffset2+1];
                a2x = a->ptr.p_double[aoffset4];
                a2y = a->ptr.p_double[aoffset4+1];
                a3x = a->ptr.p_double[aoffset6];
                a3y = a->ptr.p_double[aoffset6+1];
                t1x = a0x+a2x;
                t1y = a0y+a2y;
                t2x = a1x+a3x;
                t2y = a1y+a3y;
                m2x = a0x-a2x;
                m2y = a0y-a2y;
                m3x = a1y-a3y;
                m3y = a3x-a1x;
                tw2x = twx*twx-twy*twy;
                tw2y = 2*twx*twy;
                tw3x = twx*tw2x-twy*tw2y;
                tw3y = twx*tw2y+twy*tw2x;
                a1x = m2x+m3x;
                a1y = m2y+m3y;
                a2x = t1x-t2x;
                a2y = t1y-t2y;
                a3x = m2x-m3x;
                a3y = m2y-m3y;
                a->ptr.p_double[aoffset0] = t1x+t2x;
                a->ptr.p_double[aoffset0+1] = t1y+t2y;
                a->ptr.p_double[aoffset2] = a1x*twx-a1y*twy;
                a->ptr.p_double[aoffset2+1] = a1y*twx+a1x*twy;
                a->ptr.p_double[aoffset4] = a2x*tw2x-a2y*tw2y;
                a->ptr.p_double[aoffset4+1] = a2y*tw2x+a2x*tw2y;
                a->ptr.p_double[aoffset6] = a3x*tw3x-a3y*tw3y;
                a->ptr.p_double[aoffset6+1] = a3y*tw3x+a3x*tw3y;
                aoffset0 = aoffset0+2;
                aoffset2 = aoffset2+2;
                aoffset4 = aoffset4+2;
                aoffset6 = aoffset6+2;
                if( (mvidx+1)%ftbase_updatetw==0 )
                {
                    v = -2*ae_pi*(mvidx+1)/(n*m);
                    twxm1 = ae_sin(0.5*v, _state);
                    twxm1 = -2*twxm1*twxm1;
                    twy = ae_sin(v, _state);
                    twx = twxm1+1;
                }
                else
                {
                    v = twxm1+tw0+twxm1*tw0-twy*tw1;
                    twy = twy+tw1+twxm1*tw1+twy*tw0;
                    twxm1 = v;
                    twx = v+1;
                }
            }
        }
        return;
    }
    if( n==5 )
    {
        v = -2*ae_pi/(n*m);
        tw0 = -2*ae_sqr(ae_sin(0.5*v, _state), _state);
        tw1 = ae_sin(v, _state);
        v = 2*ae_pi/5;
        c1 = (ae_cos(v, _state)+ae_cos(2*v, _state))/2-1;
        c2 = (ae_cos(v, _state)-ae_cos(2*v, _state))/2;
        c3 = -ae_sin(v, _state);
        c4 = -(ae_sin(v, _state)+ae_sin(2*v, _state));
        c5 = ae_sin(v, _state)-ae_sin(2*v, _state);
        for(opidx=0; opidx<=operandscnt-1; opidx++)
        {
            aoffset0 = offs+opidx*operandsize*microvectorsize;
            aoffset2 = aoffset0+microvectorsize;
            aoffset4 = aoffset2+microvectorsize;
            aoffset6 = aoffset4+microvectorsize;
            aoffset8 = aoffset6+microvectorsize;
            twx = 1.0;
            twxm1 = 0.0;
            twy = 0.0;
            for(mvidx=0; mvidx<=m-1; mvidx++)
            {
                a0x = a->ptr.p_double[aoffset0];
                a0y = a->ptr.p_double[aoffset0+1];
                a1x = a->ptr.p_double[aoffset2];
                a1y = a->ptr.p_double[aoffset2+1];
                a2x = a->ptr.p_double[aoffset4];
                a2y = a->ptr.p_double[aoffset4+1];
                a3x = a->ptr.p_double[aoffset6];
                a3y = a->ptr.p_double[aoffset6+1];
                a4x = a->ptr.p_double[aoffset8];
                a4y = a->ptr.p_double[aoffset8+1];
                t1x = a1x+a4x;
                t1y = a1y+a4y;
                t2x = a2x+a3x;
                t2y = a2y+a3y;
                t3x = a1x-a4x;
                t3y = a1y-a4y;
                t4x = a3x-a2x;
                t4y = a3y-a2y;
                t5x = t1x+t2x;
                t5y = t1y+t2y;
                q0x = a0x+t5x;
                q0y = a0y+t5y;
                m1x = c1*t5x;
                m1y = c1*t5y;
                m2x = c2*(t1x-t2x);
                m2y = c2*(t1y-t2y);
                m3x = -c3*(t3y+t4y);
                m3y = c3*(t3x+t4x);
                m4x = -c4*t4y;
                m4y = c4*t4x;
                m5x = -c5*t3y;
                m5y = c5*t3x;
                s3x = m3x-m4x;
                s3y = m3y-m4y;
                s5x = m3x+m5x;
                s5y = m3y+m5y;
                s1x = q0x+m1x;
                s1y = q0y+m1y;
                s2x = s1x+m2x;
                s2y = s1y+m2y;
                s4x = s1x-m2x;
                s4y = s1y-m2y;
                tw2x = twx*twx-twy*twy;
                tw2y = 2*twx*twy;
                tw3x = twx*tw2x-twy*tw2y;
                tw3y = twx*tw2y+twy*tw2x;
                tw4x = tw2x*tw2x-tw2y*tw2y;
                tw4y = tw2x*tw2y+tw2y*tw2x;
                a1x = s2x+s3x;
                a1y = s2y+s3y;
                a2x = s4x+s5x;
                a2y = s4y+s5y;
                a3x = s4x-s5x;
                a3y = s4y-s5y;
                a4x = s2x-s3x;
                a4y = s2y-s3y;
                a->ptr.p_double[aoffset0] = q0x;
                a->ptr.p_double[aoffset0+1] = q0y;
                a->ptr.p_double[aoffset2] = a1x*twx-a1y*twy;
                a->ptr.p_double[aoffset2+1] = a1x*twy+a1y*twx;
                a->ptr.p_double[aoffset4] = a2x*tw2x-a2y*tw2y;
                a->ptr.p_double[aoffset4+1] = a2x*tw2y+a2y*tw2x;
                a->ptr.p_double[aoffset6] = a3x*tw3x-a3y*tw3y;
                a->ptr.p_double[aoffset6+1] = a3x*tw3y+a3y*tw3x;
                a->ptr.p_double[aoffset8] = a4x*tw4x-a4y*tw4y;
                a->ptr.p_double[aoffset8+1] = a4x*tw4y+a4y*tw4x;
                aoffset0 = aoffset0+2;
                aoffset2 = aoffset2+2;
                aoffset4 = aoffset4+2;
                aoffset6 = aoffset6+2;
                aoffset8 = aoffset8+2;
                if( (mvidx+1)%ftbase_updatetw==0 )
                {
                    v = -2*ae_pi*(mvidx+1)/(n*m);
                    twxm1 = ae_sin(0.5*v, _state);
                    twxm1 = -2*twxm1*twxm1;
                    twy = ae_sin(v, _state);
                    twx = twxm1+1;
                }
                else
                {
                    v = twxm1+tw0+twxm1*tw0-twy*tw1;
                    twy = twy+tw1+twxm1*tw1+twy*tw0;
                    twxm1 = v;
                    twx = v+1;
                }
            }
        }
        return;
    }
    if( n==6 )
    {
        c1 = ae_cos(2*ae_pi/3, _state)-1;
        c2 = ae_sin(2*ae_pi/3, _state);
        c3 = ae_cos(-ae_pi/3, _state);
        c4 = ae_sin(-ae_pi/3, _state);
        v = -2*ae_pi/(n*m);
        tw0 = -2*ae_sqr(ae_sin(0.5*v, _state), _state);
        tw1 = ae_sin(v, _state);
        for(opidx=0; opidx<=operandscnt-1; opidx++)
        {
            aoffset0 = offs+opidx*operandsize*microvectorsize;
            aoffset2 = aoffset0+microvectorsize;
            aoffset4 = aoffset2+microvectorsize;
            aoffset6 = aoffset4+microvectorsize;
            aoffset8 = aoffset6+microvectorsize;
            aoffset10 = aoffset8+microvectorsize;
            twx = 1.0;
            twxm1 = 0.0;
            twy = 0.0;
            for(mvidx=0; mvidx<=m-1; mvidx++)
            {
                a0x = a->ptr.p_double[aoffset0+0];
                a0y = a->ptr.p_double[aoffset0+1];
                a1x = a->ptr.p_double[aoffset2+0];
                a1y = a->ptr.p_double[aoffset2+1];
                a2x = a->ptr.p_double[aoffset4+0];
                a2y = a->ptr.p_double[aoffset4+1];
                a3x = a->ptr.p_double[aoffset6+0];
                a3y = a->ptr.p_double[aoffset6+1];
                a4x = a->ptr.p_double[aoffset8+0];
                a4y = a->ptr.p_double[aoffset8+1];
                a5x = a->ptr.p_double[aoffset10+0];
                a5y = a->ptr.p_double[aoffset10+1];
                v0 = a0x;
                v1 = a0y;
                a0x = a0x+a3x;
                a0y = a0y+a3y;
                a3x = v0-a3x;
                a3y = v1-a3y;
                v0 = a1x;
                v1 = a1y;
                a1x = a1x+a4x;
                a1y = a1y+a4y;
                a4x = v0-a4x;
                a4y = v1-a4y;
                v0 = a2x;
                v1 = a2y;
                a2x = a2x+a5x;
                a2y = a2y+a5y;
                a5x = v0-a5x;
                a5y = v1-a5y;
                t4x = a4x*c3-a4y*c4;
                t4y = a4x*c4+a4y*c3;
                a4x = t4x;
                a4y = t4y;
                t5x = -a5x*c3-a5y*c4;
                t5y = a5x*c4-a5y*c3;
                a5x = t5x;
                a5y = t5y;
                t1x = a1x+a2x;
                t1y = a1y+a2y;
                a0x = a0x+t1x;
                a0y = a0y+t1y;
                m1x = c1*t1x;
                m1y = c1*t1y;
                m2x = c2*(a1y-a2y);
                m2y = c2*(a2x-a1x);
                s1x = a0x+m1x;
                s1y = a0y+m1y;
                a1x = s1x+m2x;
                a1y = s1y+m2y;
                a2x = s1x-m2x;
                a2y = s1y-m2y;
                t1x = a4x+a5x;
                t1y = a4y+a5y;
                a3x = a3x+t1x;
                a3y = a3y+t1y;
                m1x = c1*t1x;
                m1y = c1*t1y;
                m2x = c2*(a4y-a5y);
                m2y = c2*(a5x-a4x);
                s1x = a3x+m1x;
                s1y = a3y+m1y;
                a4x = s1x+m2x;
                a4y = s1y+m2y;
                a5x = s1x-m2x;
                a5y = s1y-m2y;
                tw2x = twx*twx-twy*twy;
                tw2y = 2*twx*twy;
                tw3x = twx*tw2x-twy*tw2y;
                tw3y = twx*tw2y+twy*tw2x;
                tw4x = tw2x*tw2x-tw2y*tw2y;
                tw4y = 2*tw2x*tw2y;
                tw5x = tw3x*tw2x-tw3y*tw2y;
                tw5y = tw3x*tw2y+tw3y*tw2x;
                a->ptr.p_double[aoffset0+0] = a0x;
                a->ptr.p_double[aoffset0+1] = a0y;
                a->ptr.p_double[aoffset2+0] = a3x*twx-a3y*twy;
                a->ptr.p_double[aoffset2+1] = a3y*twx+a3x*twy;
                a->ptr.p_double[aoffset4+0] = a1x*tw2x-a1y*tw2y;
                a->ptr.p_double[aoffset4+1] = a1y*tw2x+a1x*tw2y;
                a->ptr.p_double[aoffset6+0] = a4x*tw3x-a4y*tw3y;
                a->ptr.p_double[aoffset6+1] = a4y*tw3x+a4x*tw3y;
                a->ptr.p_double[aoffset8+0] = a2x*tw4x-a2y*tw4y;
                a->ptr.p_double[aoffset8+1] = a2y*tw4x+a2x*tw4y;
                a->ptr.p_double[aoffset10+0] = a5x*tw5x-a5y*tw5y;
                a->ptr.p_double[aoffset10+1] = a5y*tw5x+a5x*tw5y;
                aoffset0 = aoffset0+2;
                aoffset2 = aoffset2+2;
                aoffset4 = aoffset4+2;
                aoffset6 = aoffset6+2;
                aoffset8 = aoffset8+2;
                aoffset10 = aoffset10+2;
                if( (mvidx+1)%ftbase_updatetw==0 )
                {
                    v = -2*ae_pi*(mvidx+1)/(n*m);
                    twxm1 = ae_sin(0.5*v, _state);
                    twxm1 = -2*twxm1*twxm1;
                    twy = ae_sin(v, _state);
                    twx = twxm1+1;
                }
                else
                {
                    v = twxm1+tw0+twxm1*tw0-twy*tw1;
                    twy = twy+tw1+twxm1*tw1+twy*tw0;
                    twxm1 = v;
                    twx = v+1;
                }
            }
        }
        return;
    }
}


/*************************************************************************
This subroutine precomputes data for complex Bluestein's  FFT  and  writes
them to array PrecR[] at specified offset. It  is  responsibility  of  the
caller to make sure that PrecR[] is large enough.

INPUT PARAMETERS:
    N           -   original size of the transform
    M           -   size of the "padded" Bluestein's transform
    PrecR       -   preallocated array
    Offs        -   offset
    
OUTPUT PARAMETERS:
    PrecR       -   data at Offs:Offs+4*M-1 are modified:
                    * PrecR[Offs:Offs+2*M-1] stores Z[k]=exp(i*pi*k^2/N)
                    * PrecR[Offs+2*M:Offs+4*M-1] stores FFT of the Z
                    Other parts of PrecR are unchanged.
                    
NOTE: this function performs internal M-point FFT. It allocates temporary
      plan which is destroyed after leaving this function.

  -- ALGLIB --
     Copyright 08.05.2013 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ftprecomputebluesteinsfft(ae_int_t n,
     ae_int_t m,
     /* Real    */ ae_vector* precr,
     ae_int_t offs,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    double bx;
    double by;
    fasttransformplan plan;

    ae_frame_make(_state, &_frame_block);
    _fasttransformplan_init(&plan, _state);

    
    /*
     * Fill first half of PrecR with b[k] = exp(i*pi*k^2/N)
     */
    for(i=0; i<=2*m-1; i++)
    {
        precr->ptr.p_double[offs+i] = (double)(0);
    }
    for(i=0; i<=n-1; i++)
    {
        bx = ae_cos(ae_pi/n*i*i, _state);
        by = ae_sin(ae_pi/n*i*i, _state);
        precr->ptr.p_double[offs+2*i+0] = bx;
        precr->ptr.p_double[offs+2*i+1] = by;
        precr->ptr.p_double[offs+2*((m-i)%m)+0] = bx;
        precr->ptr.p_double[offs+2*((m-i)%m)+1] = by;
    }
    
    /*
     * Precomputed FFT
     */
    ftcomplexfftplan(m, 1, &plan, _state);
    for(i=0; i<=2*m-1; i++)
    {
        precr->ptr.p_double[offs+2*m+i] = precr->ptr.p_double[offs+i];
    }
    ftbase_ftapplysubplan(&plan, 0, precr, offs+2*m, 0, &plan.buffer, 1, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
This subroutine applies complex Bluestein's FFT to input/output array A.

INPUT PARAMETERS:
    Plan        -   transformation plan
    A           -   array, must be large enough for plan to work
    ABase       -   base offset in array A, this value points to start of
                    subarray whose length is equal to length of the plan
    AOffset     -   offset with respect to ABase, 0<=AOffset<PlanLength.
                    This is an offset within large PlanLength-subarray of
                    the chunk to process.
    OperandsCnt -   number of repeated operands (length N each)
    N           -   original data length (measured in complex numbers)
    M           -   padded data length (measured in complex numbers)
    PrecOffs    -   offset of the precomputed data for the plan
    SubPlan     -   position of the length-M FFT subplan which is used by
                    transformation
    BufA        -   temporary buffer, at least 2*M elements
    BufB        -   temporary buffer, at least 2*M elements
    BufC        -   temporary buffer, at least 2*M elements
    BufD        -   temporary buffer, at least 2*M elements
    
OUTPUT PARAMETERS:
    A           -   transformed array

  -- ALGLIB --
     Copyright 05.04.2013 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ftbluesteinsfft(fasttransformplan* plan,
     /* Real    */ ae_vector* a,
     ae_int_t abase,
     ae_int_t aoffset,
     ae_int_t operandscnt,
     ae_int_t n,
     ae_int_t m,
     ae_int_t precoffs,
     ae_int_t subplan,
     /* Real    */ ae_vector* bufa,
     /* Real    */ ae_vector* bufb,
     /* Real    */ ae_vector* bufc,
     /* Real    */ ae_vector* bufd,
     ae_state *_state)
{
    ae_int_t op;
    ae_int_t i;
    double x;
    double y;
    double bx;
    double by;
    double ax;
    double ay;
    double rx;
    double ry;
    ae_int_t p0;
    ae_int_t p1;
    ae_int_t p2;


    for(op=0; op<=operandscnt-1; op++)
    {
        
        /*
         * Multiply A by conj(Z), store to buffer.
         * Pad A by zeros.
         *
         * NOTE: Z[k]=exp(i*pi*k^2/N)
         */
        p0 = abase+aoffset+op*2*n;
        p1 = precoffs;
        for(i=0; i<=n-1; i++)
        {
            x = a->ptr.p_double[p0+0];
            y = a->ptr.p_double[p0+1];
            bx = plan->precr.ptr.p_double[p1+0];
            by = -plan->precr.ptr.p_double[p1+1];
            bufa->ptr.p_double[2*i+0] = x*bx-y*by;
            bufa->ptr.p_double[2*i+1] = x*by+y*bx;
            p0 = p0+2;
            p1 = p1+2;
        }
        for(i=2*n; i<=2*m-1; i++)
        {
            bufa->ptr.p_double[i] = (double)(0);
        }
        
        /*
         * Perform convolution of A and Z (using precomputed
         * FFT of Z stored in Plan structure).
         */
        ftbase_ftapplysubplan(plan, subplan, bufa, 0, 0, bufc, 1, _state);
        p0 = 0;
        p1 = precoffs+2*m;
        for(i=0; i<=m-1; i++)
        {
            ax = bufa->ptr.p_double[p0+0];
            ay = bufa->ptr.p_double[p0+1];
            bx = plan->precr.ptr.p_double[p1+0];
            by = plan->precr.ptr.p_double[p1+1];
            bufa->ptr.p_double[p0+0] = ax*bx-ay*by;
            bufa->ptr.p_double[p0+1] = -(ax*by+ay*bx);
            p0 = p0+2;
            p1 = p1+2;
        }
        ftbase_ftapplysubplan(plan, subplan, bufa, 0, 0, bufc, 1, _state);
        
        /*
         * Post processing:
         *     A:=conj(Z)*conj(A)/M
         * Here conj(A)/M corresponds to last stage of inverse DFT,
         * and conj(Z) comes from Bluestein's FFT algorithm.
         */
        p0 = precoffs;
        p1 = 0;
        p2 = abase+aoffset+op*2*n;
        for(i=0; i<=n-1; i++)
        {
            bx = plan->precr.ptr.p_double[p0+0];
            by = plan->precr.ptr.p_double[p0+1];
            rx = bufa->ptr.p_double[p1+0]/m;
            ry = -bufa->ptr.p_double[p1+1]/m;
            a->ptr.p_double[p2+0] = rx*bx-ry*(-by);
            a->ptr.p_double[p2+1] = rx*(-by)+ry*bx;
            p0 = p0+2;
            p1 = p1+2;
            p2 = p2+2;
        }
    }
}


/*************************************************************************
This subroutine precomputes data for complex Rader's FFT and  writes  them
to array PrecR[] at specified offset. It  is  responsibility of the caller
to make sure that PrecR[] is large enough.

INPUT PARAMETERS:
    N           -   original size of the transform (before reduction to N-1)
    RQ          -   primitive root modulo N
    RIQ         -   inverse of primitive root modulo N
    PrecR       -   preallocated array
    Offs        -   offset
    
OUTPUT PARAMETERS:
    PrecR       -   data at Offs:Offs+2*(N-1)-1 store FFT of Rader's factors,
                    other parts of PrecR are unchanged.
                    
NOTE: this function performs internal (N-1)-point FFT. It allocates temporary
      plan which is destroyed after leaving this function.

  -- ALGLIB --
     Copyright 08.05.2013 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ftprecomputeradersfft(ae_int_t n,
     ae_int_t rq,
     ae_int_t riq,
     /* Real    */ ae_vector* precr,
     ae_int_t offs,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t q;
    fasttransformplan plan;
    ae_int_t kiq;
    double v;

    ae_frame_make(_state, &_frame_block);
    _fasttransformplan_init(&plan, _state);

    
    /*
     * Fill PrecR with Rader factors, perform FFT
     */
    kiq = 1;
    for(q=0; q<=n-2; q++)
    {
        v = -2*ae_pi*kiq/n;
        precr->ptr.p_double[offs+2*q+0] = ae_cos(v, _state);
        precr->ptr.p_double[offs+2*q+1] = ae_sin(v, _state);
        kiq = kiq*riq%n;
    }
    ftcomplexfftplan(n-1, 1, &plan, _state);
    ftbase_ftapplysubplan(&plan, 0, precr, offs, 0, &plan.buffer, 1, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
This subroutine applies complex Rader's FFT to input/output array A.

INPUT PARAMETERS:
    A           -   array, must be large enough for plan to work
    ABase       -   base offset in array A, this value points to start of
                    subarray whose length is equal to length of the plan
    AOffset     -   offset with respect to ABase, 0<=AOffset<PlanLength.
                    This is an offset within large PlanLength-subarray of
                    the chunk to process.
    OperandsCnt -   number of repeated operands (length N each)
    N           -   original data length (measured in complex numbers)
    SubPlan     -   position of the (N-1)-point FFT subplan which is used
                    by transformation
    RQ          -   primitive root modulo N
    RIQ         -   inverse of primitive root modulo N
    PrecOffs    -   offset of the precomputed data for the plan
    Buf         -   temporary array
    
OUTPUT PARAMETERS:
    A           -   transformed array

  -- ALGLIB --
     Copyright 05.04.2013 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ftradersfft(fasttransformplan* plan,
     /* Real    */ ae_vector* a,
     ae_int_t abase,
     ae_int_t aoffset,
     ae_int_t operandscnt,
     ae_int_t n,
     ae_int_t subplan,
     ae_int_t rq,
     ae_int_t riq,
     ae_int_t precoffs,
     /* Real    */ ae_vector* buf,
     ae_state *_state)
{
    ae_int_t opidx;
    ae_int_t i;
    ae_int_t q;
    ae_int_t kq;
    ae_int_t kiq;
    double x0;
    double y0;
    ae_int_t p0;
    ae_int_t p1;
    double ax;
    double ay;
    double bx;
    double by;
    double rx;
    double ry;


    ae_assert(operandscnt>=1, "FTApplyComplexRefFFT: OperandsCnt<1", _state);
    
    /*
     * Process operands
     */
    for(opidx=0; opidx<=operandscnt-1; opidx++)
    {
        
        /*
         * fill QA
         */
        kq = 1;
        p0 = abase+aoffset+opidx*n*2;
        p1 = aoffset+opidx*n*2;
        rx = a->ptr.p_double[p0+0];
        ry = a->ptr.p_double[p0+1];
        x0 = rx;
        y0 = ry;
        for(q=0; q<=n-2; q++)
        {
            ax = a->ptr.p_double[p0+2*kq+0];
            ay = a->ptr.p_double[p0+2*kq+1];
            buf->ptr.p_double[p1+0] = ax;
            buf->ptr.p_double[p1+1] = ay;
            rx = rx+ax;
            ry = ry+ay;
            kq = kq*rq%n;
            p1 = p1+2;
        }
        p0 = abase+aoffset+opidx*n*2;
        p1 = aoffset+opidx*n*2;
        for(q=0; q<=n-2; q++)
        {
            a->ptr.p_double[p0] = buf->ptr.p_double[p1];
            a->ptr.p_double[p0+1] = buf->ptr.p_double[p1+1];
            p0 = p0+2;
            p1 = p1+2;
        }
        
        /*
         * Convolution
         */
        ftbase_ftapplysubplan(plan, subplan, a, abase, aoffset+opidx*n*2, buf, 1, _state);
        p0 = abase+aoffset+opidx*n*2;
        p1 = precoffs;
        for(i=0; i<=n-2; i++)
        {
            ax = a->ptr.p_double[p0+0];
            ay = a->ptr.p_double[p0+1];
            bx = plan->precr.ptr.p_double[p1+0];
            by = plan->precr.ptr.p_double[p1+1];
            a->ptr.p_double[p0+0] = ax*bx-ay*by;
            a->ptr.p_double[p0+1] = -(ax*by+ay*bx);
            p0 = p0+2;
            p1 = p1+2;
        }
        ftbase_ftapplysubplan(plan, subplan, a, abase, aoffset+opidx*n*2, buf, 1, _state);
        p0 = abase+aoffset+opidx*n*2;
        for(i=0; i<=n-2; i++)
        {
            a->ptr.p_double[p0+0] = a->ptr.p_double[p0+0]/(n-1);
            a->ptr.p_double[p0+1] = -a->ptr.p_double[p0+1]/(n-1);
            p0 = p0+2;
        }
        
        /*
         * Result
         */
        buf->ptr.p_double[aoffset+opidx*n*2+0] = rx;
        buf->ptr.p_double[aoffset+opidx*n*2+1] = ry;
        kiq = 1;
        p0 = aoffset+opidx*n*2;
        p1 = abase+aoffset+opidx*n*2;
        for(q=0; q<=n-2; q++)
        {
            buf->ptr.p_double[p0+2*kiq+0] = x0+a->ptr.p_double[p1+0];
            buf->ptr.p_double[p0+2*kiq+1] = y0+a->ptr.p_double[p1+1];
            kiq = kiq*riq%n;
            p1 = p1+2;
        }
        p0 = abase+aoffset+opidx*n*2;
        p1 = aoffset+opidx*n*2;
        for(q=0; q<=n-1; q++)
        {
            a->ptr.p_double[p0] = buf->ptr.p_double[p1];
            a->ptr.p_double[p0+1] = buf->ptr.p_double[p1+1];
            p0 = p0+2;
            p1 = p1+2;
        }
    }
}


/*************************************************************************
Factorizes task size N into product of two smaller sizes N1 and N2

INPUT PARAMETERS:
    N       -   task size, N>0
    IsRoot  -   whether taks is root task (first one in a sequence)
    
OUTPUT PARAMETERS:
    N1, N2  -   such numbers that:
                * for prime N:                  N1=N2=0
                * for composite N<=MaxRadix:    N1=N2=0
                * for composite N>MaxRadix:     1<=N1<=N2, N1*N2=N

  -- ALGLIB --
     Copyright 08.04.2013 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ftfactorize(ae_int_t n,
     ae_bool isroot,
     ae_int_t* n1,
     ae_int_t* n2,
     ae_state *_state)
{
    ae_int_t j;
    ae_int_t k;

    *n1 = 0;
    *n2 = 0;

    ae_assert(n>0, "FTFactorize: N<=0", _state);
    *n1 = 0;
    *n2 = 0;
    
    /*
     * Small N
     */
    if( n<=ftbase_maxradix )
    {
        return;
    }
    
    /*
     * Large N, recursive split
     */
    if( n>ftbase_recursivethreshold )
    {
        k = ae_iceil(ae_sqrt((double)(n), _state), _state)+1;
        ae_assert(k*k>=n, "FTFactorize: internal error during recursive factorization", _state);
        for(j=k; j>=2; j--)
        {
            if( n%j==0 )
            {
                *n1 = ae_minint(n/j, j, _state);
                *n2 = ae_maxint(n/j, j, _state);
                return;
            }
        }
    }
    
    /*
     * N>MaxRadix, try to find good codelet
     */
    for(j=ftbase_maxradix; j>=2; j--)
    {
        if( n%j==0 )
        {
            *n1 = j;
            *n2 = n/j;
            break;
        }
    }
    
    /*
     * In case no good codelet was found,
     * try to factorize N into product of ANY primes.
     */
    if( *n1*(*n2)!=n )
    {
        for(j=2; j<=n-1; j++)
        {
            if( n%j==0 )
            {
                *n1 = j;
                *n2 = n/j;
                break;
            }
            if( j*j>n )
            {
                break;
            }
        }
    }
    
    /*
     * normalize
     */
    if( *n1>(*n2) )
    {
        j = *n1;
        *n1 = *n2;
        *n2 = j;
    }
}


/*************************************************************************
Returns optimistic estimate of the FFT cost, in UNITs (1 UNIT = 100 KFLOPs)

INPUT PARAMETERS:
    N       -   task size, N>0
    
RESULU:
    cost in UNITs, rounded down to nearest integer

NOTE: If FFT cost is less than 1 UNIT, it will return 0 as result.

  -- ALGLIB --
     Copyright 08.04.2013 by Bochkanov Sergey
*************************************************************************/
static ae_int_t ftbase_ftoptimisticestimate(ae_int_t n, ae_state *_state)
{
    ae_int_t result;


    ae_assert(n>0, "FTOptimisticEstimate: N<=0", _state);
    result = ae_ifloor(1.0E-5*5*n*ae_log((double)(n), _state)/ae_log((double)(2), _state), _state);
    return result;
}


/*************************************************************************
Twiddle factors calculation

  -- ALGLIB --
     Copyright 01.05.2009 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ffttwcalc(/* Real    */ ae_vector* a,
     ae_int_t aoffset,
     ae_int_t n1,
     ae_int_t n2,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j2;
    ae_int_t n;
    ae_int_t halfn1;
    ae_int_t offs;
    double x;
    double y;
    double twxm1;
    double twy;
    double twbasexm1;
    double twbasey;
    double twrowxm1;
    double twrowy;
    double tmpx;
    double tmpy;
    double v;
    ae_int_t updatetw2;


    
    /*
     * Multiplication by twiddle factors for complex Cooley-Tukey FFT
     * with N factorized as N1*N2.
     *
     * Naive solution to this problem is given below:
     *
     *     > for K:=1 to N2-1 do
     *     >     for J:=1 to N1-1 do
     *     >     begin
     *     >         Idx:=K*N1+J;
     *     >         X:=A[AOffset+2*Idx+0];
     *     >         Y:=A[AOffset+2*Idx+1];
     *     >         TwX:=Cos(-2*Pi()*K*J/(N1*N2));
     *     >         TwY:=Sin(-2*Pi()*K*J/(N1*N2));
     *     >         A[AOffset+2*Idx+0]:=X*TwX-Y*TwY;
     *     >         A[AOffset+2*Idx+1]:=X*TwY+Y*TwX;
     *     >     end;
     *
     * However, there are exist more efficient solutions.
     *
     * Each pass of the inner cycle corresponds to multiplication of one
     * entry of A by W[k,j]=exp(-I*2*pi*k*j/N). This factor can be rewritten
     * as exp(-I*2*pi*k/N)^j. So we can replace costly exponentiation by
     * repeated multiplication: W[k,j+1]=W[k,j]*exp(-I*2*pi*k/N), with
     * second factor being computed once in the beginning of the iteration.
     *
     * Also, exp(-I*2*pi*k/N) can be represented as exp(-I*2*pi/N)^k, i.e.
     * we have W[K+1,1]=W[K,1]*W[1,1].
     *
     * In our loop we use following variables:
     * * [TwBaseXM1,TwBaseY] =   [cos(2*pi/N)-1,     sin(2*pi/N)]
     * * [TwRowXM1, TwRowY]  =   [cos(2*pi*I/N)-1,   sin(2*pi*I/N)]
     * * [TwXM1,    TwY]     =   [cos(2*pi*I*J/N)-1, sin(2*pi*I*J/N)]
     *
     * Meaning of the variables:
     * * [TwXM1,TwY] is current twiddle factor W[I,J]
     * * [TwRowXM1, TwRowY] is W[I,1]
     * * [TwBaseXM1,TwBaseY] is W[1,1]
     *
     * During inner loop we multiply current twiddle factor by W[I,1],
     * during outer loop we update W[I,1].
     *
     */
    ae_assert(ftbase_updatetw>=2, "FFTTwCalc: internal error - UpdateTw<2", _state);
    updatetw2 = ftbase_updatetw/2;
    halfn1 = n1/2;
    n = n1*n2;
    v = -2*ae_pi/n;
    twbasexm1 = -2*ae_sqr(ae_sin(0.5*v, _state), _state);
    twbasey = ae_sin(v, _state);
    twrowxm1 = (double)(0);
    twrowy = (double)(0);
    offs = aoffset;
    for(i=0; i<=n2-1; i++)
    {
        
        /*
         * Initialize twiddle factor for current row
         */
        twxm1 = (double)(0);
        twy = (double)(0);
        
        /*
         * N1-point block is separated into 2-point chunks and residual 1-point chunk
         * (in case N1 is odd). Unrolled loop is several times faster.
         */
        for(j2=0; j2<=halfn1-1; j2++)
        {
            
            /*
             * Processing:
             * * process first element in a chunk.
             * * update twiddle factor (unconditional update)
             * * process second element
             * * conditional update of the twiddle factor
             */
            x = a->ptr.p_double[offs+0];
            y = a->ptr.p_double[offs+1];
            tmpx = x*(1+twxm1)-y*twy;
            tmpy = x*twy+y*(1+twxm1);
            a->ptr.p_double[offs+0] = tmpx;
            a->ptr.p_double[offs+1] = tmpy;
            tmpx = (1+twxm1)*twrowxm1-twy*twrowy;
            twy = twy+(1+twxm1)*twrowy+twy*twrowxm1;
            twxm1 = twxm1+tmpx;
            x = a->ptr.p_double[offs+2];
            y = a->ptr.p_double[offs+3];
            tmpx = x*(1+twxm1)-y*twy;
            tmpy = x*twy+y*(1+twxm1);
            a->ptr.p_double[offs+2] = tmpx;
            a->ptr.p_double[offs+3] = tmpy;
            offs = offs+4;
            if( (j2+1)%updatetw2==0&&j2<halfn1-1 )
            {
                
                /*
                 * Recalculate twiddle factor
                 */
                v = -2*ae_pi*i*2*(j2+1)/n;
                twxm1 = ae_sin(0.5*v, _state);
                twxm1 = -2*twxm1*twxm1;
                twy = ae_sin(v, _state);
            }
            else
            {
                
                /*
                 * Update twiddle factor
                 */
                tmpx = (1+twxm1)*twrowxm1-twy*twrowy;
                twy = twy+(1+twxm1)*twrowy+twy*twrowxm1;
                twxm1 = twxm1+tmpx;
            }
        }
        if( n1%2==1 )
        {
            
            /*
             * Handle residual chunk
             */
            x = a->ptr.p_double[offs+0];
            y = a->ptr.p_double[offs+1];
            tmpx = x*(1+twxm1)-y*twy;
            tmpy = x*twy+y*(1+twxm1);
            a->ptr.p_double[offs+0] = tmpx;
            a->ptr.p_double[offs+1] = tmpy;
            offs = offs+2;
        }
        
        /*
         * update TwRow: TwRow(new) = TwRow(old)*TwBase
         */
        if( i<n2-1 )
        {
            if( (i+1)%ftbase_updatetw==0 )
            {
                v = -2*ae_pi*(i+1)/n;
                twrowxm1 = ae_sin(0.5*v, _state);
                twrowxm1 = -2*twrowxm1*twrowxm1;
                twrowy = ae_sin(v, _state);
            }
            else
            {
                tmpx = twbasexm1+twrowxm1*twbasexm1-twrowy*twbasey;
                tmpy = twbasey+twrowxm1*twbasey+twrowy*twbasexm1;
                twrowxm1 = twrowxm1+tmpx;
                twrowy = twrowy+tmpy;
            }
        }
    }
}


/*************************************************************************
Linear transpose: transpose complex matrix stored in 1-dimensional array

  -- ALGLIB --
     Copyright 01.05.2009 by Bochkanov Sergey
*************************************************************************/
static void ftbase_internalcomplexlintranspose(/* Real    */ ae_vector* a,
     ae_int_t m,
     ae_int_t n,
     ae_int_t astart,
     /* Real    */ ae_vector* buf,
     ae_state *_state)
{


    ftbase_ffticltrec(a, astart, n, buf, 0, m, m, n, _state);
    ae_v_move(&a->ptr.p_double[astart], 1, &buf->ptr.p_double[0], 1, ae_v_len(astart,astart+2*m*n-1));
}


/*************************************************************************
Recurrent subroutine for a InternalComplexLinTranspose

Write A^T to B, where:
* A is m*n complex matrix stored in array A as pairs of real/image values,
  beginning from AStart position, with AStride stride
* B is n*m complex matrix stored in array B as pairs of real/image values,
  beginning from BStart position, with BStride stride
stride is measured in complex numbers, i.e. in real/image pairs.

  -- ALGLIB --
     Copyright 01.05.2009 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ffticltrec(/* Real    */ ae_vector* a,
     ae_int_t astart,
     ae_int_t astride,
     /* Real    */ ae_vector* b,
     ae_int_t bstart,
     ae_int_t bstride,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t idx1;
    ae_int_t idx2;
    ae_int_t m2;
    ae_int_t m1;
    ae_int_t n1;


    if( m==0||n==0 )
    {
        return;
    }
    if( ae_maxint(m, n, _state)<=8 )
    {
        m2 = 2*bstride;
        for(i=0; i<=m-1; i++)
        {
            idx1 = bstart+2*i;
            idx2 = astart+2*i*astride;
            for(j=0; j<=n-1; j++)
            {
                b->ptr.p_double[idx1+0] = a->ptr.p_double[idx2+0];
                b->ptr.p_double[idx1+1] = a->ptr.p_double[idx2+1];
                idx1 = idx1+m2;
                idx2 = idx2+2;
            }
        }
        return;
    }
    if( n>m )
    {
        
        /*
         * New partition:
         *
         * "A^T -> B" becomes "(A1 A2)^T -> ( B1 )
         *                                  ( B2 )
         */
        n1 = n/2;
        if( n-n1>=8&&n1%8!=0 )
        {
            n1 = n1+(8-n1%8);
        }
        ae_assert(n-n1>0, "Assertion failed", _state);
        ftbase_ffticltrec(a, astart, astride, b, bstart, bstride, m, n1, _state);
        ftbase_ffticltrec(a, astart+2*n1, astride, b, bstart+2*n1*bstride, bstride, m, n-n1, _state);
    }
    else
    {
        
        /*
         * New partition:
         *
         * "A^T -> B" becomes "( A1 )^T -> ( B1 B2 )
         *                     ( A2 )
         */
        m1 = m/2;
        if( m-m1>=8&&m1%8!=0 )
        {
            m1 = m1+(8-m1%8);
        }
        ae_assert(m-m1>0, "Assertion failed", _state);
        ftbase_ffticltrec(a, astart, astride, b, bstart, bstride, m1, n, _state);
        ftbase_ffticltrec(a, astart+2*m1*astride, astride, b, bstart+2*m1, bstride, m-m1, n, _state);
    }
}


/*************************************************************************
Recurrent subroutine for a InternalRealLinTranspose


  -- ALGLIB --
     Copyright 01.05.2009 by Bochkanov Sergey
*************************************************************************/
static void ftbase_fftirltrec(/* Real    */ ae_vector* a,
     ae_int_t astart,
     ae_int_t astride,
     /* Real    */ ae_vector* b,
     ae_int_t bstart,
     ae_int_t bstride,
     ae_int_t m,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t idx1;
    ae_int_t idx2;
    ae_int_t m1;
    ae_int_t n1;


    if( m==0||n==0 )
    {
        return;
    }
    if( ae_maxint(m, n, _state)<=8 )
    {
        for(i=0; i<=m-1; i++)
        {
            idx1 = bstart+i;
            idx2 = astart+i*astride;
            for(j=0; j<=n-1; j++)
            {
                b->ptr.p_double[idx1] = a->ptr.p_double[idx2];
                idx1 = idx1+bstride;
                idx2 = idx2+1;
            }
        }
        return;
    }
    if( n>m )
    {
        
        /*
         * New partition:
         *
         * "A^T -> B" becomes "(A1 A2)^T -> ( B1 )
         *                                  ( B2 )
         */
        n1 = n/2;
        if( n-n1>=8&&n1%8!=0 )
        {
            n1 = n1+(8-n1%8);
        }
        ae_assert(n-n1>0, "Assertion failed", _state);
        ftbase_fftirltrec(a, astart, astride, b, bstart, bstride, m, n1, _state);
        ftbase_fftirltrec(a, astart+n1, astride, b, bstart+n1*bstride, bstride, m, n-n1, _state);
    }
    else
    {
        
        /*
         * New partition:
         *
         * "A^T -> B" becomes "( A1 )^T -> ( B1 B2 )
         *                     ( A2 )
         */
        m1 = m/2;
        if( m-m1>=8&&m1%8!=0 )
        {
            m1 = m1+(8-m1%8);
        }
        ae_assert(m-m1>0, "Assertion failed", _state);
        ftbase_fftirltrec(a, astart, astride, b, bstart, bstride, m1, n, _state);
        ftbase_fftirltrec(a, astart+m1*astride, astride, b, bstart+m1, bstride, m-m1, n, _state);
    }
}


/*************************************************************************
recurrent subroutine for FFTFindSmoothRec

  -- ALGLIB --
     Copyright 01.05.2009 by Bochkanov Sergey
*************************************************************************/
static void ftbase_ftbasefindsmoothrec(ae_int_t n,
     ae_int_t seed,
     ae_int_t leastfactor,
     ae_int_t* best,
     ae_state *_state)
{


    ae_assert(ftbase_ftbasemaxsmoothfactor<=5, "FTBaseFindSmoothRec: internal error!", _state);
    if( seed>=n )
    {
        *best = ae_minint(*best, seed, _state);
        return;
    }
    if( leastfactor<=2 )
    {
        ftbase_ftbasefindsmoothrec(n, seed*2, 2, best, _state);
    }
    if( leastfactor<=3 )
    {
        ftbase_ftbasefindsmoothrec(n, seed*3, 3, best, _state);
    }
    if( leastfactor<=5 )
    {
        ftbase_ftbasefindsmoothrec(n, seed*5, 5, best, _state);
    }
}


void _fasttransformplan_init(void* _p, ae_state *_state)
{
    fasttransformplan *p = (fasttransformplan*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_init(&p->entries, 0, 0, DT_INT, _state);
    ae_vector_init(&p->buffer, 0, DT_REAL, _state);
    ae_vector_init(&p->precr, 0, DT_REAL, _state);
    ae_vector_init(&p->preci, 0, DT_REAL, _state);
    ae_shared_pool_init(&p->bluesteinpool, _state);
}


void _fasttransformplan_init_copy(void* _dst, void* _src, ae_state *_state)
{
    fasttransformplan *dst = (fasttransformplan*)_dst;
    fasttransformplan *src = (fasttransformplan*)_src;
    ae_matrix_init_copy(&dst->entries, &src->entries, _state);
    ae_vector_init_copy(&dst->buffer, &src->buffer, _state);
    ae_vector_init_copy(&dst->precr, &src->precr, _state);
    ae_vector_init_copy(&dst->preci, &src->preci, _state);
    ae_shared_pool_init_copy(&dst->bluesteinpool, &src->bluesteinpool, _state);
}


void _fasttransformplan_clear(void* _p)
{
    fasttransformplan *p = (fasttransformplan*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_clear(&p->entries);
    ae_vector_clear(&p->buffer);
    ae_vector_clear(&p->precr);
    ae_vector_clear(&p->preci);
    ae_shared_pool_clear(&p->bluesteinpool);
}


void _fasttransformplan_destroy(void* _p)
{
    fasttransformplan *p = (fasttransformplan*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_destroy(&p->entries);
    ae_vector_destroy(&p->buffer);
    ae_vector_destroy(&p->precr);
    ae_vector_destroy(&p->preci);
    ae_shared_pool_destroy(&p->bluesteinpool);
}


/*$ End $*/
