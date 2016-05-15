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
#include "linreg.h"


/*$ Declarations $*/
static ae_int_t linreg_lrvnum = 5;
static void linreg_lrinternal(/* Real    */ ae_matrix* xy,
     /* Real    */ ae_vector* s,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t* info,
     linearmodel* lm,
     lrreport* ar,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Linear regression

Subroutine builds model:

    Y = A(0)*X[0] + ... + A(N-1)*X[N-1] + A(N)

and model found in ALGLIB format, covariation matrix, training set  errors
(rms,  average,  average  relative)   and  leave-one-out  cross-validation
estimate of the generalization error. CV  estimate calculated  using  fast
algorithm with O(NPoints*NVars) complexity.

When  covariation  matrix  is  calculated  standard deviations of function
values are assumed to be equal to RMS error on the training set.

INPUT PARAMETERS:
    XY          -   training set, array [0..NPoints-1,0..NVars]:
                    * NVars columns - independent variables
                    * last column - dependent variable
    NPoints     -   training set size, NPoints>NVars+1
    NVars       -   number of independent variables

OUTPUT PARAMETERS:
    Info        -   return code:
                    * -255, in case of unknown internal error
                    * -4, if internal SVD subroutine haven't converged
                    * -1, if incorrect parameters was passed (NPoints<NVars+2, NVars<1).
                    *  1, if subroutine successfully finished
    LM          -   linear model in the ALGLIB format. Use subroutines of
                    this unit to work with the model.
    AR          -   additional results


  -- ALGLIB --
     Copyright 02.08.2008 by Bochkanov Sergey
*************************************************************************/
void lrbuild(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t* info,
     linearmodel* lm,
     lrreport* ar,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector s;
    ae_int_t i;
    double sigma2;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _linearmodel_clear(lm);
    _lrreport_clear(ar);
    ae_vector_init(&s, 0, DT_REAL, _state);

    if( npoints<=nvars+1||nvars<1 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_vector_set_length(&s, npoints-1+1, _state);
    for(i=0; i<=npoints-1; i++)
    {
        s.ptr.p_double[i] = (double)(1);
    }
    lrbuilds(xy, &s, npoints, nvars, info, lm, ar, _state);
    if( *info<0 )
    {
        ae_frame_leave(_state);
        return;
    }
    sigma2 = ae_sqr(ar->rmserror, _state)*npoints/(npoints-nvars-1);
    for(i=0; i<=nvars; i++)
    {
        ae_v_muld(&ar->c.ptr.pp_double[i][0], 1, ae_v_len(0,nvars), sigma2);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Linear regression

Variant of LRBuild which uses vector of standatd deviations (errors in
function values).

INPUT PARAMETERS:
    XY          -   training set, array [0..NPoints-1,0..NVars]:
                    * NVars columns - independent variables
                    * last column - dependent variable
    S           -   standard deviations (errors in function values)
                    array[0..NPoints-1], S[i]>0.
    NPoints     -   training set size, NPoints>NVars+1
    NVars       -   number of independent variables

OUTPUT PARAMETERS:
    Info        -   return code:
                    * -255, in case of unknown internal error
                    * -4, if internal SVD subroutine haven't converged
                    * -1, if incorrect parameters was passed (NPoints<NVars+2, NVars<1).
                    * -2, if S[I]<=0
                    *  1, if subroutine successfully finished
    LM          -   linear model in the ALGLIB format. Use subroutines of
                    this unit to work with the model.
    AR          -   additional results


  -- ALGLIB --
     Copyright 02.08.2008 by Bochkanov Sergey
*************************************************************************/
void lrbuilds(/* Real    */ ae_matrix* xy,
     /* Real    */ ae_vector* s,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t* info,
     linearmodel* lm,
     lrreport* ar,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix xyi;
    ae_vector x;
    ae_vector means;
    ae_vector sigmas;
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_int_t offs;
    double mean;
    double variance;
    double skewness;
    double kurtosis;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _linearmodel_clear(lm);
    _lrreport_clear(ar);
    ae_matrix_init(&xyi, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&means, 0, DT_REAL, _state);
    ae_vector_init(&sigmas, 0, DT_REAL, _state);

    
    /*
     * Test parameters
     */
    if( npoints<=nvars+1||nvars<1 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Copy data, add one more column (constant term)
     */
    ae_matrix_set_length(&xyi, npoints-1+1, nvars+1+1, _state);
    for(i=0; i<=npoints-1; i++)
    {
        ae_v_move(&xyi.ptr.pp_double[i][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
        xyi.ptr.pp_double[i][nvars] = (double)(1);
        xyi.ptr.pp_double[i][nvars+1] = xy->ptr.pp_double[i][nvars];
    }
    
    /*
     * Standartization
     */
    ae_vector_set_length(&x, npoints-1+1, _state);
    ae_vector_set_length(&means, nvars-1+1, _state);
    ae_vector_set_length(&sigmas, nvars-1+1, _state);
    for(j=0; j<=nvars-1; j++)
    {
        ae_v_move(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[0][j], xy->stride, ae_v_len(0,npoints-1));
        samplemoments(&x, npoints, &mean, &variance, &skewness, &kurtosis, _state);
        means.ptr.p_double[j] = mean;
        sigmas.ptr.p_double[j] = ae_sqrt(variance, _state);
        if( ae_fp_eq(sigmas.ptr.p_double[j],(double)(0)) )
        {
            sigmas.ptr.p_double[j] = (double)(1);
        }
        for(i=0; i<=npoints-1; i++)
        {
            xyi.ptr.pp_double[i][j] = (xyi.ptr.pp_double[i][j]-means.ptr.p_double[j])/sigmas.ptr.p_double[j];
        }
    }
    
    /*
     * Internal processing
     */
    linreg_lrinternal(&xyi, s, npoints, nvars+1, info, lm, ar, _state);
    if( *info<0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Un-standartization
     */
    offs = ae_round(lm->w.ptr.p_double[3], _state);
    for(j=0; j<=nvars-1; j++)
    {
        
        /*
         * Constant term is updated (and its covariance too,
         * since it gets some variance from J-th component)
         */
        lm->w.ptr.p_double[offs+nvars] = lm->w.ptr.p_double[offs+nvars]-lm->w.ptr.p_double[offs+j]*means.ptr.p_double[j]/sigmas.ptr.p_double[j];
        v = means.ptr.p_double[j]/sigmas.ptr.p_double[j];
        ae_v_subd(&ar->c.ptr.pp_double[nvars][0], 1, &ar->c.ptr.pp_double[j][0], 1, ae_v_len(0,nvars), v);
        ae_v_subd(&ar->c.ptr.pp_double[0][nvars], ar->c.stride, &ar->c.ptr.pp_double[0][j], ar->c.stride, ae_v_len(0,nvars), v);
        
        /*
         * J-th term is updated
         */
        lm->w.ptr.p_double[offs+j] = lm->w.ptr.p_double[offs+j]/sigmas.ptr.p_double[j];
        v = 1/sigmas.ptr.p_double[j];
        ae_v_muld(&ar->c.ptr.pp_double[j][0], 1, ae_v_len(0,nvars), v);
        ae_v_muld(&ar->c.ptr.pp_double[0][j], ar->c.stride, ae_v_len(0,nvars), v);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Like LRBuildS, but builds model

    Y = A(0)*X[0] + ... + A(N-1)*X[N-1]

i.e. with zero constant term.

  -- ALGLIB --
     Copyright 30.10.2008 by Bochkanov Sergey
*************************************************************************/
void lrbuildzs(/* Real    */ ae_matrix* xy,
     /* Real    */ ae_vector* s,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t* info,
     linearmodel* lm,
     lrreport* ar,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix xyi;
    ae_vector x;
    ae_vector c;
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_int_t offs;
    double mean;
    double variance;
    double skewness;
    double kurtosis;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _linearmodel_clear(lm);
    _lrreport_clear(ar);
    ae_matrix_init(&xyi, 0, 0, DT_REAL, _state);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&c, 0, DT_REAL, _state);

    
    /*
     * Test parameters
     */
    if( npoints<=nvars+1||nvars<1 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Copy data, add one more column (constant term)
     */
    ae_matrix_set_length(&xyi, npoints-1+1, nvars+1+1, _state);
    for(i=0; i<=npoints-1; i++)
    {
        ae_v_move(&xyi.ptr.pp_double[i][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
        xyi.ptr.pp_double[i][nvars] = (double)(0);
        xyi.ptr.pp_double[i][nvars+1] = xy->ptr.pp_double[i][nvars];
    }
    
    /*
     * Standartization: unusual scaling
     */
    ae_vector_set_length(&x, npoints-1+1, _state);
    ae_vector_set_length(&c, nvars-1+1, _state);
    for(j=0; j<=nvars-1; j++)
    {
        ae_v_move(&x.ptr.p_double[0], 1, &xy->ptr.pp_double[0][j], xy->stride, ae_v_len(0,npoints-1));
        samplemoments(&x, npoints, &mean, &variance, &skewness, &kurtosis, _state);
        if( ae_fp_greater(ae_fabs(mean, _state),ae_sqrt(variance, _state)) )
        {
            
            /*
             * variation is relatively small, it is better to
             * bring mean value to 1
             */
            c.ptr.p_double[j] = mean;
        }
        else
        {
            
            /*
             * variation is large, it is better to bring variance to 1
             */
            if( ae_fp_eq(variance,(double)(0)) )
            {
                variance = (double)(1);
            }
            c.ptr.p_double[j] = ae_sqrt(variance, _state);
        }
        for(i=0; i<=npoints-1; i++)
        {
            xyi.ptr.pp_double[i][j] = xyi.ptr.pp_double[i][j]/c.ptr.p_double[j];
        }
    }
    
    /*
     * Internal processing
     */
    linreg_lrinternal(&xyi, s, npoints, nvars+1, info, lm, ar, _state);
    if( *info<0 )
    {
        ae_frame_leave(_state);
        return;
    }
    
    /*
     * Un-standartization
     */
    offs = ae_round(lm->w.ptr.p_double[3], _state);
    for(j=0; j<=nvars-1; j++)
    {
        
        /*
         * J-th term is updated
         */
        lm->w.ptr.p_double[offs+j] = lm->w.ptr.p_double[offs+j]/c.ptr.p_double[j];
        v = 1/c.ptr.p_double[j];
        ae_v_muld(&ar->c.ptr.pp_double[j][0], 1, ae_v_len(0,nvars), v);
        ae_v_muld(&ar->c.ptr.pp_double[0][j], ar->c.stride, ae_v_len(0,nvars), v);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Like LRBuild but builds model

    Y = A(0)*X[0] + ... + A(N-1)*X[N-1]

i.e. with zero constant term.

  -- ALGLIB --
     Copyright 30.10.2008 by Bochkanov Sergey
*************************************************************************/
void lrbuildz(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t* info,
     linearmodel* lm,
     lrreport* ar,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector s;
    ae_int_t i;
    double sigma2;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _linearmodel_clear(lm);
    _lrreport_clear(ar);
    ae_vector_init(&s, 0, DT_REAL, _state);

    if( npoints<=nvars+1||nvars<1 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_vector_set_length(&s, npoints-1+1, _state);
    for(i=0; i<=npoints-1; i++)
    {
        s.ptr.p_double[i] = (double)(1);
    }
    lrbuildzs(xy, &s, npoints, nvars, info, lm, ar, _state);
    if( *info<0 )
    {
        ae_frame_leave(_state);
        return;
    }
    sigma2 = ae_sqr(ar->rmserror, _state)*npoints/(npoints-nvars-1);
    for(i=0; i<=nvars; i++)
    {
        ae_v_muld(&ar->c.ptr.pp_double[i][0], 1, ae_v_len(0,nvars), sigma2);
    }
    ae_frame_leave(_state);
}


/*************************************************************************
Unpacks coefficients of linear model.

INPUT PARAMETERS:
    LM          -   linear model in ALGLIB format

OUTPUT PARAMETERS:
    V           -   coefficients, array[0..NVars]
                    constant term (intercept) is stored in the V[NVars].
    NVars       -   number of independent variables (one less than number
                    of coefficients)

  -- ALGLIB --
     Copyright 30.08.2008 by Bochkanov Sergey
*************************************************************************/
void lrunpack(linearmodel* lm,
     /* Real    */ ae_vector* v,
     ae_int_t* nvars,
     ae_state *_state)
{
    ae_int_t offs;

    ae_vector_clear(v);
    *nvars = 0;

    ae_assert(ae_round(lm->w.ptr.p_double[1], _state)==linreg_lrvnum, "LINREG: Incorrect LINREG version!", _state);
    *nvars = ae_round(lm->w.ptr.p_double[2], _state);
    offs = ae_round(lm->w.ptr.p_double[3], _state);
    ae_vector_set_length(v, *nvars+1, _state);
    ae_v_move(&v->ptr.p_double[0], 1, &lm->w.ptr.p_double[offs], 1, ae_v_len(0,*nvars));
}


/*************************************************************************
"Packs" coefficients and creates linear model in ALGLIB format (LRUnpack
reversed).

INPUT PARAMETERS:
    V           -   coefficients, array[0..NVars]
    NVars       -   number of independent variables

OUTPUT PAREMETERS:
    LM          -   linear model.

  -- ALGLIB --
     Copyright 30.08.2008 by Bochkanov Sergey
*************************************************************************/
void lrpack(/* Real    */ ae_vector* v,
     ae_int_t nvars,
     linearmodel* lm,
     ae_state *_state)
{
    ae_int_t offs;

    _linearmodel_clear(lm);

    ae_vector_set_length(&lm->w, 4+nvars+1, _state);
    offs = 4;
    lm->w.ptr.p_double[0] = (double)(4+nvars+1);
    lm->w.ptr.p_double[1] = (double)(linreg_lrvnum);
    lm->w.ptr.p_double[2] = (double)(nvars);
    lm->w.ptr.p_double[3] = (double)(offs);
    ae_v_move(&lm->w.ptr.p_double[offs], 1, &v->ptr.p_double[0], 1, ae_v_len(offs,offs+nvars));
}


/*************************************************************************
Procesing

INPUT PARAMETERS:
    LM      -   linear model
    X       -   input vector,  array[0..NVars-1].

Result:
    value of linear model regression estimate

  -- ALGLIB --
     Copyright 03.09.2008 by Bochkanov Sergey
*************************************************************************/
double lrprocess(linearmodel* lm,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    double v;
    ae_int_t offs;
    ae_int_t nvars;
    double result;


    ae_assert(ae_round(lm->w.ptr.p_double[1], _state)==linreg_lrvnum, "LINREG: Incorrect LINREG version!", _state);
    nvars = ae_round(lm->w.ptr.p_double[2], _state);
    offs = ae_round(lm->w.ptr.p_double[3], _state);
    v = ae_v_dotproduct(&x->ptr.p_double[0], 1, &lm->w.ptr.p_double[offs], 1, ae_v_len(0,nvars-1));
    result = v+lm->w.ptr.p_double[offs+nvars];
    return result;
}


/*************************************************************************
RMS error on the test set

INPUT PARAMETERS:
    LM      -   linear model
    XY      -   test set
    NPoints -   test set size

RESULT:
    root mean square error.

  -- ALGLIB --
     Copyright 30.08.2008 by Bochkanov Sergey
*************************************************************************/
double lrrmserror(linearmodel* lm,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_int_t i;
    double v;
    ae_int_t offs;
    ae_int_t nvars;
    double result;


    ae_assert(ae_round(lm->w.ptr.p_double[1], _state)==linreg_lrvnum, "LINREG: Incorrect LINREG version!", _state);
    nvars = ae_round(lm->w.ptr.p_double[2], _state);
    offs = ae_round(lm->w.ptr.p_double[3], _state);
    result = (double)(0);
    for(i=0; i<=npoints-1; i++)
    {
        v = ae_v_dotproduct(&xy->ptr.pp_double[i][0], 1, &lm->w.ptr.p_double[offs], 1, ae_v_len(0,nvars-1));
        v = v+lm->w.ptr.p_double[offs+nvars];
        result = result+ae_sqr(v-xy->ptr.pp_double[i][nvars], _state);
    }
    result = ae_sqrt(result/npoints, _state);
    return result;
}


/*************************************************************************
Average error on the test set

INPUT PARAMETERS:
    LM      -   linear model
    XY      -   test set
    NPoints -   test set size

RESULT:
    average error.

  -- ALGLIB --
     Copyright 30.08.2008 by Bochkanov Sergey
*************************************************************************/
double lravgerror(linearmodel* lm,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_int_t i;
    double v;
    ae_int_t offs;
    ae_int_t nvars;
    double result;


    ae_assert(ae_round(lm->w.ptr.p_double[1], _state)==linreg_lrvnum, "LINREG: Incorrect LINREG version!", _state);
    nvars = ae_round(lm->w.ptr.p_double[2], _state);
    offs = ae_round(lm->w.ptr.p_double[3], _state);
    result = (double)(0);
    for(i=0; i<=npoints-1; i++)
    {
        v = ae_v_dotproduct(&xy->ptr.pp_double[i][0], 1, &lm->w.ptr.p_double[offs], 1, ae_v_len(0,nvars-1));
        v = v+lm->w.ptr.p_double[offs+nvars];
        result = result+ae_fabs(v-xy->ptr.pp_double[i][nvars], _state);
    }
    result = result/npoints;
    return result;
}


/*************************************************************************
RMS error on the test set

INPUT PARAMETERS:
    LM      -   linear model
    XY      -   test set
    NPoints -   test set size

RESULT:
    average relative error.

  -- ALGLIB --
     Copyright 30.08.2008 by Bochkanov Sergey
*************************************************************************/
double lravgrelerror(linearmodel* lm,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t k;
    double v;
    ae_int_t offs;
    ae_int_t nvars;
    double result;


    ae_assert(ae_round(lm->w.ptr.p_double[1], _state)==linreg_lrvnum, "LINREG: Incorrect LINREG version!", _state);
    nvars = ae_round(lm->w.ptr.p_double[2], _state);
    offs = ae_round(lm->w.ptr.p_double[3], _state);
    result = (double)(0);
    k = 0;
    for(i=0; i<=npoints-1; i++)
    {
        if( ae_fp_neq(xy->ptr.pp_double[i][nvars],(double)(0)) )
        {
            v = ae_v_dotproduct(&xy->ptr.pp_double[i][0], 1, &lm->w.ptr.p_double[offs], 1, ae_v_len(0,nvars-1));
            v = v+lm->w.ptr.p_double[offs+nvars];
            result = result+ae_fabs((v-xy->ptr.pp_double[i][nvars])/xy->ptr.pp_double[i][nvars], _state);
            k = k+1;
        }
    }
    if( k!=0 )
    {
        result = result/k;
    }
    return result;
}


/*************************************************************************
Copying of LinearModel strucure

INPUT PARAMETERS:
    LM1 -   original

OUTPUT PARAMETERS:
    LM2 -   copy

  -- ALGLIB --
     Copyright 15.03.2009 by Bochkanov Sergey
*************************************************************************/
void lrcopy(linearmodel* lm1, linearmodel* lm2, ae_state *_state)
{
    ae_int_t k;

    _linearmodel_clear(lm2);

    k = ae_round(lm1->w.ptr.p_double[0], _state);
    ae_vector_set_length(&lm2->w, k-1+1, _state);
    ae_v_move(&lm2->w.ptr.p_double[0], 1, &lm1->w.ptr.p_double[0], 1, ae_v_len(0,k-1));
}


void lrlines(/* Real    */ ae_matrix* xy,
     /* Real    */ ae_vector* s,
     ae_int_t n,
     ae_int_t* info,
     double* a,
     double* b,
     double* vara,
     double* varb,
     double* covab,
     double* corrab,
     double* p,
     ae_state *_state)
{
    ae_int_t i;
    double ss;
    double sx;
    double sxx;
    double sy;
    double stt;
    double e1;
    double e2;
    double t;
    double chi2;

    *info = 0;
    *a = 0;
    *b = 0;
    *vara = 0;
    *varb = 0;
    *covab = 0;
    *corrab = 0;
    *p = 0;

    if( n<2 )
    {
        *info = -1;
        return;
    }
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_less_eq(s->ptr.p_double[i],(double)(0)) )
        {
            *info = -2;
            return;
        }
    }
    *info = 1;
    
    /*
     * Calculate S, SX, SY, SXX
     */
    ss = (double)(0);
    sx = (double)(0);
    sy = (double)(0);
    sxx = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        t = ae_sqr(s->ptr.p_double[i], _state);
        ss = ss+1/t;
        sx = sx+xy->ptr.pp_double[i][0]/t;
        sy = sy+xy->ptr.pp_double[i][1]/t;
        sxx = sxx+ae_sqr(xy->ptr.pp_double[i][0], _state)/t;
    }
    
    /*
     * Test for condition number
     */
    t = ae_sqrt(4*ae_sqr(sx, _state)+ae_sqr(ss-sxx, _state), _state);
    e1 = 0.5*(ss+sxx+t);
    e2 = 0.5*(ss+sxx-t);
    if( ae_fp_less_eq(ae_minreal(e1, e2, _state),1000*ae_machineepsilon*ae_maxreal(e1, e2, _state)) )
    {
        *info = -3;
        return;
    }
    
    /*
     * Calculate A, B
     */
    *a = (double)(0);
    *b = (double)(0);
    stt = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        t = (xy->ptr.pp_double[i][0]-sx/ss)/s->ptr.p_double[i];
        *b = *b+t*xy->ptr.pp_double[i][1]/s->ptr.p_double[i];
        stt = stt+ae_sqr(t, _state);
    }
    *b = *b/stt;
    *a = (sy-sx*(*b))/ss;
    
    /*
     * Calculate goodness-of-fit
     */
    if( n>2 )
    {
        chi2 = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            chi2 = chi2+ae_sqr((xy->ptr.pp_double[i][1]-(*a)-*b*xy->ptr.pp_double[i][0])/s->ptr.p_double[i], _state);
        }
        *p = incompletegammac((double)(n-2)/(double)2, chi2/2, _state);
    }
    else
    {
        *p = (double)(1);
    }
    
    /*
     * Calculate other parameters
     */
    *vara = (1+ae_sqr(sx, _state)/(ss*stt))/ss;
    *varb = 1/stt;
    *covab = -sx/(ss*stt);
    *corrab = *covab/ae_sqrt(*vara*(*varb), _state);
}


void lrline(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t* info,
     double* a,
     double* b,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_vector s;
    ae_int_t i;
    double vara;
    double varb;
    double covab;
    double corrab;
    double p;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    *a = 0;
    *b = 0;
    ae_vector_init(&s, 0, DT_REAL, _state);

    if( n<2 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    ae_vector_set_length(&s, n-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        s.ptr.p_double[i] = (double)(1);
    }
    lrlines(xy, &s, n, info, a, b, &vara, &varb, &covab, &corrab, &p, _state);
    ae_frame_leave(_state);
}


/*************************************************************************
Internal linear regression subroutine
*************************************************************************/
static void linreg_lrinternal(/* Real    */ ae_matrix* xy,
     /* Real    */ ae_vector* s,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t* info,
     linearmodel* lm,
     lrreport* ar,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix a;
    ae_matrix u;
    ae_matrix vt;
    ae_matrix vm;
    ae_matrix xym;
    ae_vector b;
    ae_vector sv;
    ae_vector t;
    ae_vector svi;
    ae_vector work;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t ncv;
    ae_int_t na;
    ae_int_t nacv;
    double r;
    double p;
    double epstol;
    lrreport ar2;
    ae_int_t offs;
    linearmodel tlm;

    ae_frame_make(_state, &_frame_block);
    *info = 0;
    _linearmodel_clear(lm);
    _lrreport_clear(ar);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&u, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vt, 0, 0, DT_REAL, _state);
    ae_matrix_init(&vm, 0, 0, DT_REAL, _state);
    ae_matrix_init(&xym, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&sv, 0, DT_REAL, _state);
    ae_vector_init(&t, 0, DT_REAL, _state);
    ae_vector_init(&svi, 0, DT_REAL, _state);
    ae_vector_init(&work, 0, DT_REAL, _state);
    _lrreport_init(&ar2, _state);
    _linearmodel_init(&tlm, _state);

    epstol = (double)(1000);
    
    /*
     * Check for errors in data
     */
    if( npoints<nvars||nvars<1 )
    {
        *info = -1;
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=npoints-1; i++)
    {
        if( ae_fp_less_eq(s->ptr.p_double[i],(double)(0)) )
        {
            *info = -2;
            ae_frame_leave(_state);
            return;
        }
    }
    *info = 1;
    
    /*
     * Create design matrix
     */
    ae_matrix_set_length(&a, npoints-1+1, nvars-1+1, _state);
    ae_vector_set_length(&b, npoints-1+1, _state);
    for(i=0; i<=npoints-1; i++)
    {
        r = 1/s->ptr.p_double[i];
        ae_v_moved(&a.ptr.pp_double[i][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1), r);
        b.ptr.p_double[i] = xy->ptr.pp_double[i][nvars]/s->ptr.p_double[i];
    }
    
    /*
     * Allocate W:
     * W[0]     array size
     * W[1]     version number, 0
     * W[2]     NVars (minus 1, to be compatible with external representation)
     * W[3]     coefficients offset
     */
    ae_vector_set_length(&lm->w, 4+nvars-1+1, _state);
    offs = 4;
    lm->w.ptr.p_double[0] = (double)(4+nvars);
    lm->w.ptr.p_double[1] = (double)(linreg_lrvnum);
    lm->w.ptr.p_double[2] = (double)(nvars-1);
    lm->w.ptr.p_double[3] = (double)(offs);
    
    /*
     * Solve problem using SVD:
     *
     * 0. check for degeneracy (different types)
     * 1. A = U*diag(sv)*V'
     * 2. T = b'*U
     * 3. w = SUM((T[i]/sv[i])*V[..,i])
     * 4. cov(wi,wj) = SUM(Vji*Vjk/sv[i]^2,K=1..M)
     *
     * see $15.4 of "Numerical Recipes in C" for more information
     */
    ae_vector_set_length(&t, nvars-1+1, _state);
    ae_vector_set_length(&svi, nvars-1+1, _state);
    ae_matrix_set_length(&ar->c, nvars-1+1, nvars-1+1, _state);
    ae_matrix_set_length(&vm, nvars-1+1, nvars-1+1, _state);
    if( !rmatrixsvd(&a, npoints, nvars, 1, 1, 2, &sv, &u, &vt, _state) )
    {
        *info = -4;
        ae_frame_leave(_state);
        return;
    }
    if( ae_fp_less_eq(sv.ptr.p_double[0],(double)(0)) )
    {
        
        /*
         * Degenerate case: zero design matrix.
         */
        for(i=offs; i<=offs+nvars-1; i++)
        {
            lm->w.ptr.p_double[i] = (double)(0);
        }
        ar->rmserror = lrrmserror(lm, xy, npoints, _state);
        ar->avgerror = lravgerror(lm, xy, npoints, _state);
        ar->avgrelerror = lravgrelerror(lm, xy, npoints, _state);
        ar->cvrmserror = ar->rmserror;
        ar->cvavgerror = ar->avgerror;
        ar->cvavgrelerror = ar->avgrelerror;
        ar->ncvdefects = 0;
        ae_vector_set_length(&ar->cvdefects, nvars-1+1, _state);
        for(i=0; i<=nvars-1; i++)
        {
            ar->cvdefects.ptr.p_int[i] = -1;
        }
        ae_matrix_set_length(&ar->c, nvars-1+1, nvars-1+1, _state);
        for(i=0; i<=nvars-1; i++)
        {
            for(j=0; j<=nvars-1; j++)
            {
                ar->c.ptr.pp_double[i][j] = (double)(0);
            }
        }
        ae_frame_leave(_state);
        return;
    }
    if( ae_fp_less_eq(sv.ptr.p_double[nvars-1],epstol*ae_machineepsilon*sv.ptr.p_double[0]) )
    {
        
        /*
         * Degenerate case, non-zero design matrix.
         *
         * We can leave it and solve task in SVD least squares fashion.
         * Solution and covariance matrix will be obtained correctly,
         * but CV error estimates - will not. It is better to reduce
         * it to non-degenerate task and to obtain correct CV estimates.
         */
        for(k=nvars; k>=1; k--)
        {
            if( ae_fp_greater(sv.ptr.p_double[k-1],epstol*ae_machineepsilon*sv.ptr.p_double[0]) )
            {
                
                /*
                 * Reduce
                 */
                ae_matrix_set_length(&xym, npoints-1+1, k+1, _state);
                for(i=0; i<=npoints-1; i++)
                {
                    for(j=0; j<=k-1; j++)
                    {
                        r = ae_v_dotproduct(&xy->ptr.pp_double[i][0], 1, &vt.ptr.pp_double[j][0], 1, ae_v_len(0,nvars-1));
                        xym.ptr.pp_double[i][j] = r;
                    }
                    xym.ptr.pp_double[i][k] = xy->ptr.pp_double[i][nvars];
                }
                
                /*
                 * Solve
                 */
                linreg_lrinternal(&xym, s, npoints, k, info, &tlm, &ar2, _state);
                if( *info!=1 )
                {
                    ae_frame_leave(_state);
                    return;
                }
                
                /*
                 * Convert back to un-reduced format
                 */
                for(j=0; j<=nvars-1; j++)
                {
                    lm->w.ptr.p_double[offs+j] = (double)(0);
                }
                for(j=0; j<=k-1; j++)
                {
                    r = tlm.w.ptr.p_double[offs+j];
                    ae_v_addd(&lm->w.ptr.p_double[offs], 1, &vt.ptr.pp_double[j][0], 1, ae_v_len(offs,offs+nvars-1), r);
                }
                ar->rmserror = ar2.rmserror;
                ar->avgerror = ar2.avgerror;
                ar->avgrelerror = ar2.avgrelerror;
                ar->cvrmserror = ar2.cvrmserror;
                ar->cvavgerror = ar2.cvavgerror;
                ar->cvavgrelerror = ar2.cvavgrelerror;
                ar->ncvdefects = ar2.ncvdefects;
                ae_vector_set_length(&ar->cvdefects, nvars-1+1, _state);
                for(j=0; j<=ar->ncvdefects-1; j++)
                {
                    ar->cvdefects.ptr.p_int[j] = ar2.cvdefects.ptr.p_int[j];
                }
                for(j=ar->ncvdefects; j<=nvars-1; j++)
                {
                    ar->cvdefects.ptr.p_int[j] = -1;
                }
                ae_matrix_set_length(&ar->c, nvars-1+1, nvars-1+1, _state);
                ae_vector_set_length(&work, nvars+1, _state);
                matrixmatrixmultiply(&ar2.c, 0, k-1, 0, k-1, ae_false, &vt, 0, k-1, 0, nvars-1, ae_false, 1.0, &vm, 0, k-1, 0, nvars-1, 0.0, &work, _state);
                matrixmatrixmultiply(&vt, 0, k-1, 0, nvars-1, ae_true, &vm, 0, k-1, 0, nvars-1, ae_false, 1.0, &ar->c, 0, nvars-1, 0, nvars-1, 0.0, &work, _state);
                ae_frame_leave(_state);
                return;
            }
        }
        *info = -255;
        ae_frame_leave(_state);
        return;
    }
    for(i=0; i<=nvars-1; i++)
    {
        if( ae_fp_greater(sv.ptr.p_double[i],epstol*ae_machineepsilon*sv.ptr.p_double[0]) )
        {
            svi.ptr.p_double[i] = 1/sv.ptr.p_double[i];
        }
        else
        {
            svi.ptr.p_double[i] = (double)(0);
        }
    }
    for(i=0; i<=nvars-1; i++)
    {
        t.ptr.p_double[i] = (double)(0);
    }
    for(i=0; i<=npoints-1; i++)
    {
        r = b.ptr.p_double[i];
        ae_v_addd(&t.ptr.p_double[0], 1, &u.ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1), r);
    }
    for(i=0; i<=nvars-1; i++)
    {
        lm->w.ptr.p_double[offs+i] = (double)(0);
    }
    for(i=0; i<=nvars-1; i++)
    {
        r = t.ptr.p_double[i]*svi.ptr.p_double[i];
        ae_v_addd(&lm->w.ptr.p_double[offs], 1, &vt.ptr.pp_double[i][0], 1, ae_v_len(offs,offs+nvars-1), r);
    }
    for(j=0; j<=nvars-1; j++)
    {
        r = svi.ptr.p_double[j];
        ae_v_moved(&vm.ptr.pp_double[0][j], vm.stride, &vt.ptr.pp_double[j][0], 1, ae_v_len(0,nvars-1), r);
    }
    for(i=0; i<=nvars-1; i++)
    {
        for(j=i; j<=nvars-1; j++)
        {
            r = ae_v_dotproduct(&vm.ptr.pp_double[i][0], 1, &vm.ptr.pp_double[j][0], 1, ae_v_len(0,nvars-1));
            ar->c.ptr.pp_double[i][j] = r;
            ar->c.ptr.pp_double[j][i] = r;
        }
    }
    
    /*
     * Leave-1-out cross-validation error.
     *
     * NOTATIONS:
     * A            design matrix
     * A*x = b      original linear least squares task
     * U*S*V'       SVD of A
     * ai           i-th row of the A
     * bi           i-th element of the b
     * xf           solution of the original LLS task
     *
     * Cross-validation error of i-th element from a sample is
     * calculated using following formula:
     *
     *     ERRi = ai*xf - (ai*xf-bi*(ui*ui'))/(1-ui*ui')     (1)
     *
     * This formula can be derived from normal equations of the
     * original task
     *
     *     (A'*A)x = A'*b                                    (2)
     *
     * by applying modification (zeroing out i-th row of A) to (2):
     *
     *     (A-ai)'*(A-ai) = (A-ai)'*b
     *
     * and using Sherman-Morrison formula for updating matrix inverse
     *
     * NOTE 1: b is not zeroed out since it is much simpler and
     * does not influence final result.
     *
     * NOTE 2: some design matrices A have such ui that 1-ui*ui'=0.
     * Formula (1) can't be applied for such cases and they are skipped
     * from CV calculation (which distorts resulting CV estimate).
     * But from the properties of U we can conclude that there can
     * be no more than NVars such vectors. Usually
     * NVars << NPoints, so in a normal case it only slightly
     * influences result.
     */
    ncv = 0;
    na = 0;
    nacv = 0;
    ar->rmserror = (double)(0);
    ar->avgerror = (double)(0);
    ar->avgrelerror = (double)(0);
    ar->cvrmserror = (double)(0);
    ar->cvavgerror = (double)(0);
    ar->cvavgrelerror = (double)(0);
    ar->ncvdefects = 0;
    ae_vector_set_length(&ar->cvdefects, nvars-1+1, _state);
    for(i=0; i<=nvars-1; i++)
    {
        ar->cvdefects.ptr.p_int[i] = -1;
    }
    for(i=0; i<=npoints-1; i++)
    {
        
        /*
         * Error on a training set
         */
        r = ae_v_dotproduct(&xy->ptr.pp_double[i][0], 1, &lm->w.ptr.p_double[offs], 1, ae_v_len(0,nvars-1));
        ar->rmserror = ar->rmserror+ae_sqr(r-xy->ptr.pp_double[i][nvars], _state);
        ar->avgerror = ar->avgerror+ae_fabs(r-xy->ptr.pp_double[i][nvars], _state);
        if( ae_fp_neq(xy->ptr.pp_double[i][nvars],(double)(0)) )
        {
            ar->avgrelerror = ar->avgrelerror+ae_fabs((r-xy->ptr.pp_double[i][nvars])/xy->ptr.pp_double[i][nvars], _state);
            na = na+1;
        }
        
        /*
         * Error using fast leave-one-out cross-validation
         */
        p = ae_v_dotproduct(&u.ptr.pp_double[i][0], 1, &u.ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
        if( ae_fp_greater(p,1-epstol*ae_machineepsilon) )
        {
            ar->cvdefects.ptr.p_int[ar->ncvdefects] = i;
            ar->ncvdefects = ar->ncvdefects+1;
            continue;
        }
        r = s->ptr.p_double[i]*(r/s->ptr.p_double[i]-b.ptr.p_double[i]*p)/(1-p);
        ar->cvrmserror = ar->cvrmserror+ae_sqr(r-xy->ptr.pp_double[i][nvars], _state);
        ar->cvavgerror = ar->cvavgerror+ae_fabs(r-xy->ptr.pp_double[i][nvars], _state);
        if( ae_fp_neq(xy->ptr.pp_double[i][nvars],(double)(0)) )
        {
            ar->cvavgrelerror = ar->cvavgrelerror+ae_fabs((r-xy->ptr.pp_double[i][nvars])/xy->ptr.pp_double[i][nvars], _state);
            nacv = nacv+1;
        }
        ncv = ncv+1;
    }
    if( ncv==0 )
    {
        
        /*
         * Something strange: ALL ui are degenerate.
         * Unexpected...
         */
        *info = -255;
        ae_frame_leave(_state);
        return;
    }
    ar->rmserror = ae_sqrt(ar->rmserror/npoints, _state);
    ar->avgerror = ar->avgerror/npoints;
    if( na!=0 )
    {
        ar->avgrelerror = ar->avgrelerror/na;
    }
    ar->cvrmserror = ae_sqrt(ar->cvrmserror/ncv, _state);
    ar->cvavgerror = ar->cvavgerror/ncv;
    if( nacv!=0 )
    {
        ar->cvavgrelerror = ar->cvavgrelerror/nacv;
    }
    ae_frame_leave(_state);
}


void _linearmodel_init(void* _p, ae_state *_state)
{
    linearmodel *p = (linearmodel*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->w, 0, DT_REAL, _state);
}


void _linearmodel_init_copy(void* _dst, void* _src, ae_state *_state)
{
    linearmodel *dst = (linearmodel*)_dst;
    linearmodel *src = (linearmodel*)_src;
    ae_vector_init_copy(&dst->w, &src->w, _state);
}


void _linearmodel_clear(void* _p)
{
    linearmodel *p = (linearmodel*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->w);
}


void _linearmodel_destroy(void* _p)
{
    linearmodel *p = (linearmodel*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->w);
}


void _lrreport_init(void* _p, ae_state *_state)
{
    lrreport *p = (lrreport*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_init(&p->c, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->cvdefects, 0, DT_INT, _state);
}


void _lrreport_init_copy(void* _dst, void* _src, ae_state *_state)
{
    lrreport *dst = (lrreport*)_dst;
    lrreport *src = (lrreport*)_src;
    ae_matrix_init_copy(&dst->c, &src->c, _state);
    dst->rmserror = src->rmserror;
    dst->avgerror = src->avgerror;
    dst->avgrelerror = src->avgrelerror;
    dst->cvrmserror = src->cvrmserror;
    dst->cvavgerror = src->cvavgerror;
    dst->cvavgrelerror = src->cvavgrelerror;
    dst->ncvdefects = src->ncvdefects;
    ae_vector_init_copy(&dst->cvdefects, &src->cvdefects, _state);
}


void _lrreport_clear(void* _p)
{
    lrreport *p = (lrreport*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_clear(&p->c);
    ae_vector_clear(&p->cvdefects);
}


void _lrreport_destroy(void* _p)
{
    lrreport *p = (lrreport*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_destroy(&p->c);
    ae_vector_destroy(&p->cvdefects);
}


/*$ End $*/
