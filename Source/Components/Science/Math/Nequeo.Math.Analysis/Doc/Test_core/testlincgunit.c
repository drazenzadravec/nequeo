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


#include <stdafx.h>
#include <stdio.h>
#include "testlincgunit.h"


/*$ Declarations $*/
static double testlincgunit_e0 = 1.0E-6;
static double testlincgunit_maxcond = 30;
static ae_bool testlincgunit_complextest(ae_bool silent, ae_state *_state);
static ae_bool testlincgunit_complexres(ae_bool silent, ae_state *_state);
static ae_bool testlincgunit_basictestx(ae_bool silent, ae_state *_state);
static ae_bool testlincgunit_testrcorrectness(ae_bool silent,
     ae_state *_state);
static ae_bool testlincgunit_basictestiters(ae_bool silent,
     ae_state *_state);
static ae_bool testlincgunit_krylovsubspacetest(ae_bool silent,
     ae_state *_state);
static ae_bool testlincgunit_sparsetest(ae_bool silent, ae_state *_state);
static ae_bool testlincgunit_precondtest(ae_bool silent, ae_state *_state);
static void testlincgunit_gramshmidtortnorm(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_int_t k,
     double eps,
     /* Real    */ ae_matrix* b,
     ae_int_t* k2,
     ae_state *_state);
static ae_bool testlincgunit_frombasis(/* Real    */ ae_vector* x,
     /* Real    */ ae_matrix* basis,
     ae_int_t n,
     ae_int_t k,
     double eps,
     ae_state *_state);


/*$ Body $*/


ae_bool testlincg(ae_bool silent, ae_state *_state)
{
    ae_bool basictestxerrors;
    ae_bool basictestiterserr;
    ae_bool complexreserrors;
    ae_bool complexerrors;
    ae_bool rcorrectness;
    ae_bool krylovsubspaceerr;
    ae_bool sparseerrors;
    ae_bool preconderrors;
    ae_bool waserrors;
    ae_bool result;


    basictestxerrors = testlincgunit_basictestx(ae_true, _state);
    basictestiterserr = testlincgunit_basictestiters(ae_true, _state);
    complexreserrors = testlincgunit_complexres(ae_true, _state);
    complexerrors = testlincgunit_complextest(ae_true, _state);
    rcorrectness = testlincgunit_testrcorrectness(ae_true, _state);
    krylovsubspaceerr = testlincgunit_krylovsubspacetest(ae_true, _state);
    sparseerrors = testlincgunit_sparsetest(ae_true, _state);
    preconderrors = testlincgunit_precondtest(ae_true, _state);
    
    /*
     * report
     */
    waserrors = ((((((basictestxerrors||complexreserrors)||complexerrors)||rcorrectness)||basictestiterserr)||krylovsubspaceerr)||sparseerrors)||preconderrors;
    if( !silent )
    {
        printf("TESTING LinCG\n");
        printf("BasicTestX:                                   ");
        if( basictestxerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("BasicTestIters:                               ");
        if( basictestiterserr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("ComplexResTest:                               ");
        if( complexreserrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("ComplexTest:                                  ");
        if( complexerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("R2 correctness:                               ");
        if( rcorrectness )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("KrylovSubSpaceTest:                           ");
        if( krylovsubspaceerr )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("SparseTest:                                   ");
        if( sparseerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("PrecondTest:                                  ");
        if( preconderrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        
        /*
         *was errors?
         */
        if( waserrors )
        {
            printf("TEST FAILED\n");
        }
        else
        {
            printf("TEST PASSED\n");
        }
        printf("\n\n");
    }
    result = !waserrors;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testlincg(ae_bool silent, ae_state *_state)
{
    return testlincg(silent, _state);
}


/*************************************************************************
Function for testing LinCGIteration function(custom option), which to solve
Ax=b(here A is random positive definite matrix NxN, b is random vector). It
uses  the  default  stopping criterion(RNorm<FEps=10^-6). If algorithm does
more iterations than size  of  the problem, then  some errors are possible.
The test verifies the following propirties:
    1. (A*pk,pm)=0 for any m<>k;
    2. (rk,rm)=0 for any m<>k;
    3. (rk,pm)=0 for any m<>k;

INPUT:
    Silent   -   if true then function output report

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlincgunit_complextest(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    lincgstate state;
    lincgreport rep;
    ae_matrix a;
    ae_vector b;
    ae_int_t n;
    double c;
    ae_vector x0;
    ae_vector residual;
    double normofresidual;
    double sclr;
    double na;
    double nv0;
    double nv1;
    ae_int_t sz;
    double mx;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t l;
    double tmp;
    ae_matrix mtx;
    ae_matrix mtp;
    ae_matrix mtr;
    double getrnorm;
    ae_int_t numofit;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _lincgstate_init(&state, _state);
    _lincgreport_init(&rep, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&residual, 0, DT_REAL, _state);
    ae_matrix_init(&mtx, 0, 0, DT_REAL, _state);
    ae_matrix_init(&mtp, 0, 0, DT_REAL, _state);
    ae_matrix_init(&mtr, 0, 0, DT_REAL, _state);

    mx = (double)(100);
    n = 5;
    for(sz=1; sz<=n; sz++)
    {
        
        /*
         * Generate:
         * * random A with norm NA (equal to 1.0),
         * * random right part B whose elements are uniformly distributed in [-MX,+MX]
         * * random starting point X0 whose elements are uniformly distributed in [-MX,+MX]
         */
        c = 15+15*ae_randomreal(_state);
        spdmatrixrndcond(sz, c, &a, _state);
        na = (double)(1);
        ae_vector_set_length(&b, sz, _state);
        for(i=0; i<=sz-1; i++)
        {
            b.ptr.p_double[i] = mx*(2*ae_randomreal(_state)-1);
        }
        ae_vector_set_length(&x0, sz, _state);
        for(i=0; i<=sz-1; i++)
        {
            x0.ptr.p_double[i] = mx*(2*ae_randomreal(_state)-1);
        }
        ae_matrix_set_length(&mtx, sz+1, sz, _state);
        
        /*
         * Start optimization, record its progress for further analysis
         * NOTE: we set update frequency of R to 2 in order to test that R is updated correctly
         */
        lincgcreate(sz, &state, _state);
        lincgsetxrep(&state, ae_true, _state);
        lincgsetb(&state, &b, _state);
        lincgsetstartingpoint(&state, &x0, _state);
        lincgsetcond(&state, (double)(0), sz, _state);
        lincgsetrupdatefreq(&state, 2, _state);
        numofit = 0;
        getrnorm = ae_maxrealnumber;
        while(lincgiteration(&state, _state))
        {
            if( state.needmv )
            {
                for(i=0; i<=sz-1; i++)
                {
                    state.mv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=sz-1; j++)
                    {
                        state.mv.ptr.p_double[i] = state.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                    }
                }
            }
            if( state.needvmv )
            {
                state.vmv = (double)(0);
                for(i=0; i<=sz-1; i++)
                {
                    state.mv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=sz-1; j++)
                    {
                        state.mv.ptr.p_double[i] = state.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*state.x.ptr.p_double[j];
                    }
                    state.vmv = state.vmv+state.mv.ptr.p_double[i]*state.x.ptr.p_double[i];
                }
            }
            if( state.needprec )
            {
                for(i=0; i<=sz-1; i++)
                {
                    state.pv.ptr.p_double[i] = state.x.ptr.p_double[i];
                }
            }
            if( state.xupdated )
            {
                
                /*
                 * Save current point to MtX, it will be used later for additional tests
                 */
                if( numofit>=mtx.rows )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
                for(i=0; i<=sz-1; i++)
                {
                    mtx.ptr.pp_double[numofit][i] = state.x.ptr.p_double[i];
                }
                getrnorm = state.r2;
                numofit = numofit+1;
            }
        }
        lincgresults(&state, &x0, &rep, _state);
        if( ae_fp_neq(getrnorm,rep.r2) )
        {
            if( !silent )
            {
                printf("IterationsCount=%0d;\nNMV=%0d;\nTerminationType=%0d;\n",
                    (int)(rep.iterationscount),
                    (int)(rep.nmv),
                    (int)(rep.terminationtype));
                printf("Size=%0d;\nCond=%0.5f;\nComplexTest::Fail::GetRNorm<>Rep.R2!(%0.2e<>%0.2e)\n",
                    (int)(sz),
                    (double)(c),
                    (double)(getrnorm),
                    (double)(rep.r2));
            }
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Calculate residual, check result
         */
        ae_vector_set_length(&residual, sz, _state);
        for(i=0; i<=sz-1; i++)
        {
            tmp = (double)(0);
            for(j=0; j<=sz-1; j++)
            {
                tmp = tmp+a.ptr.pp_double[i][j]*x0.ptr.p_double[j];
            }
            residual.ptr.p_double[i] = b.ptr.p_double[i]-tmp;
        }
        normofresidual = (double)(0);
        for(i=0; i<=sz-1; i++)
        {
            if( ae_fp_greater(ae_fabs(residual.ptr.p_double[i], _state),testlincgunit_e0) )
            {
                if( !silent )
                {
                    printf("IterationsCount=%0d;\nNMV=%0d;\nTerminationType=%0d;\n",
                        (int)(rep.iterationscount),
                        (int)(rep.nmv),
                        (int)(rep.terminationtype));
                    printf("Size=%0d;\nCond=%0.5f;\nComplexTest::Fail::Discripancy[%0d]>E0!(%0.2e>%0.2e)\n",
                        (int)(sz),
                        (double)(c),
                        (int)(i),
                        (double)(residual.ptr.p_double[i]),
                        (double)(testlincgunit_e0));
                }
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            normofresidual = normofresidual+residual.ptr.p_double[i]*residual.ptr.p_double[i];
        }
        if( ae_fp_greater(ae_fabs(normofresidual-rep.r2, _state),testlincgunit_e0) )
        {
            if( !silent )
            {
                printf("IterationsCount=%0d;\nNMV=%0d;\nTerminationType=%0d;\n",
                    (int)(rep.iterationscount),
                    (int)(rep.nmv),
                    (int)(rep.terminationtype));
                printf("Size=%0d;\nCond=%0.5f;\nComplexTest::Fail::||NormOfResidual-Rep.R2||>E0!(%0.2e>%0.2e)\n",
                    (int)(sz),
                    (double)(c),
                    (double)(ae_fabs(normofresidual-rep.r2, _state)),
                    (double)(testlincgunit_e0));
                printf("NormOfResidual=%0.2e; Rep.R2=%0.2e\n",
                    (double)(normofresidual),
                    (double)(rep.r2));
            }
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Check algorithm properties (conjugacy/orthogonality).
         * Here we use MtX which was filled during algorithm progress towards solution.
         *
         * NOTE: this test is skipped when algorithm converged in less than SZ iterations.
         */
        if( sz>1&&rep.iterationscount==sz )
        {
            ae_matrix_set_length(&mtp, sz, sz, _state);
            ae_matrix_set_length(&mtr, sz, sz, _state);
            for(i=0; i<=sz-1; i++)
            {
                for(j=0; j<=sz-1; j++)
                {
                    mtp.ptr.pp_double[i][j] = mtx.ptr.pp_double[i+1][j]-mtx.ptr.pp_double[i][j];
                    tmp = (double)(0);
                    for(k=0; k<=sz-1; k++)
                    {
                        tmp = tmp+a.ptr.pp_double[j][k]*mtx.ptr.pp_double[i][k];
                    }
                    mtr.ptr.pp_double[i][j] = b.ptr.p_double[j]-tmp;
                }
            }
            
            /*
             *(Api,pj)=0?
             */
            sclr = (double)(0);
            nv0 = (double)(0);
            nv1 = (double)(0);
            for(i=0; i<=sz-1; i++)
            {
                for(j=0; j<=sz-1; j++)
                {
                    if( i==j )
                    {
                        continue;
                    }
                    for(k=0; k<=sz-1; k++)
                    {
                        tmp = (double)(0);
                        for(l=0; l<=sz-1; l++)
                        {
                            tmp = tmp+a.ptr.pp_double[k][l]*mtp.ptr.pp_double[i][l];
                        }
                        sclr = sclr+tmp*mtp.ptr.pp_double[j][k];
                        nv0 = nv0+mtp.ptr.pp_double[i][k]*mtp.ptr.pp_double[i][k];
                        nv1 = nv1+mtp.ptr.pp_double[j][k]*mtp.ptr.pp_double[j][k];
                    }
                    nv0 = ae_sqrt(nv0, _state);
                    nv1 = ae_sqrt(nv1, _state);
                    if( ae_fp_greater(ae_fabs(sclr, _state),testlincgunit_e0*na*nv0*nv1) )
                    {
                        if( !silent )
                        {
                            printf("IterationsCount=%0d;\nNMV=%0d;\nTerminationType=%0d;\n",
                                (int)(rep.iterationscount),
                                (int)(rep.nmv),
                                (int)(rep.terminationtype));
                            printf("Size=%0d;\nCond=%0.5f;\nComplexTest::Fail::(Ap%0d,p%0d)!=0\n{Sclr=%0.15f; NA=%0.15f NV0=%0.15f NV1=%0.15f;}\n",
                                (int)(sz),
                                (double)(c),
                                (int)(i),
                                (int)(j),
                                (double)(sclr),
                                (double)(na),
                                (double)(nv0),
                                (double)(nv1));
                        }
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
            
            /*
             *(ri,pj)=0?
             */
            for(i=1; i<=sz-1; i++)
            {
                for(j=0; j<=i-1; j++)
                {
                    sclr = (double)(0);
                    nv0 = (double)(0);
                    nv1 = (double)(0);
                    for(k=0; k<=sz-1; k++)
                    {
                        sclr = sclr+mtr.ptr.pp_double[i][k]*mtp.ptr.pp_double[j][k];
                        nv0 = nv0+mtr.ptr.pp_double[i][k]*mtr.ptr.pp_double[i][k];
                        nv1 = nv1+mtp.ptr.pp_double[j][k]*mtp.ptr.pp_double[j][k];
                    }
                    nv0 = ae_sqrt(nv0, _state);
                    nv1 = ae_sqrt(nv1, _state);
                    if( ae_fp_greater(ae_fabs(sclr, _state),testlincgunit_e0*nv0*nv1) )
                    {
                        if( !silent )
                        {
                            printf("IterationsCount=%0d;\nNMV=%0d;\nTerminationType=%0d;\n",
                                (int)(rep.iterationscount),
                                (int)(rep.nmv),
                                (int)(rep.terminationtype));
                            printf("Size=%0d;\nCond=%0.5f;\nComplexTest::Fail::(r%0d,p%0d)!=0\n{Sclr=%0.15f; NV0=%0.15f NV1=%0.15f;}\n",
                                (int)(sz),
                                (double)(c),
                                (int)(i),
                                (int)(j),
                                (double)(sclr),
                                (double)(nv0),
                                (double)(nv1));
                        }
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
            
            /*
             *(ri,rj)=0?
             */
            for(i=0; i<=sz-1; i++)
            {
                for(j=i+1; j<=sz-1; j++)
                {
                    sclr = (double)(0);
                    nv0 = (double)(0);
                    nv1 = (double)(0);
                    for(k=0; k<=sz-1; k++)
                    {
                        sclr = sclr+mtr.ptr.pp_double[i][k]*mtr.ptr.pp_double[j][k];
                        nv0 = nv0+mtr.ptr.pp_double[i][k]*mtr.ptr.pp_double[i][k];
                        nv1 = nv1+mtr.ptr.pp_double[j][k]*mtr.ptr.pp_double[j][k];
                    }
                    nv0 = ae_sqrt(nv0, _state);
                    nv1 = ae_sqrt(nv1, _state);
                    if( ae_fp_greater(ae_fabs(sclr, _state),testlincgunit_e0*nv0*nv1) )
                    {
                        if( !silent )
                        {
                            printf("IterationsCount=%0d;\nNMV=%0d;\nTerminationType=%0d;\n",
                                (int)(rep.iterationscount),
                                (int)(rep.nmv),
                                (int)(rep.terminationtype));
                            printf("Size=%0d;\nCond=%0.5f;\nComplexTest::Fail::(rm,rk)!=0\n{Sclr=%0.15f; NV0=%0.15f NV1=%0.15f;}\n",
                                (int)(sz),
                                (double)(c),
                                (double)(sclr),
                                (double)(nv0),
                                (double)(nv1));
                        }
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
        }
    }
    if( !silent )
    {
        printf("ComplexTest::Ok\n");
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function prepare problem with a known solution 'Xs'(A*Xs-b=0). There
b is A*Xs. After, function check algorithm result and 'Xs'.
There used two stopping criterions:
    1. achieved the required precision(StCrit=0);
    2. execution of the required number of iterations(StCrit=1).

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlincgunit_complexres(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    lincgstate s;
    lincgreport rep;
    ae_matrix a;
    ae_vector b;
    ae_vector xs;
    ae_vector x0;
    double err;
    ae_int_t n;
    ae_int_t sz;
    double c;
    ae_int_t i;
    ae_int_t j;
    ae_int_t stcrit;
    double mx;
    double tmp;
    double eps;
    ae_int_t xp;
    ae_int_t nxp;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _lincgstate_init(&s, _state);
    _lincgreport_init(&rep, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&xs, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);

    sz = 5;
    mx = (double)(100);
    nxp = 100;
    for(xp=0; xp<=nxp-1; xp++)
    {
        for(n=1; n<=sz; n++)
        {
            for(stcrit=0; stcrit<=1; stcrit++)
            {
                
                /*
                 * Generate:
                 * * random A with norm NA (equal to 1.0),
                 * * random solution XS whose elements are uniformly distributed in [-MX,+MX]
                 * * random starting point X0 whose elements are uniformly distributed in [-MX,+MX]
                 * * B = A*Xs
                 */
                c = (testlincgunit_maxcond-1)*ae_randomreal(_state)+1;
                spdmatrixrndcond(n, c, &a, _state);
                ae_vector_set_length(&b, n, _state);
                ae_vector_set_length(&x0, n, _state);
                ae_vector_set_length(&xs, n, _state);
                for(i=0; i<=n-1; i++)
                {
                    x0.ptr.p_double[i] = mx*(2*ae_randomreal(_state)-1);
                    xs.ptr.p_double[i] = mx*(2*ae_randomreal(_state)-1);
                }
                eps = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    b.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        b.ptr.p_double[i] = b.ptr.p_double[i]+a.ptr.pp_double[i][j]*xs.ptr.p_double[j];
                    }
                    eps = eps+b.ptr.p_double[i]*b.ptr.p_double[i];
                }
                eps = 1.0E-6*ae_sqrt(eps, _state);
                
                /*
                 * Solve with different stopping criteria
                 */
                lincgcreate(n, &s, _state);
                lincgsetb(&s, &b, _state);
                lincgsetstartingpoint(&s, &x0, _state);
                lincgsetxrep(&s, ae_true, _state);
                if( stcrit==0 )
                {
                    lincgsetcond(&s, 1.0E-6, 0, _state);
                }
                else
                {
                    lincgsetcond(&s, (double)(0), n, _state);
                }
                while(lincgiteration(&s, _state))
                {
                    if( s.needmv )
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            s.mv.ptr.p_double[i] = (double)(0);
                            for(j=0; j<=n-1; j++)
                            {
                                s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                            }
                        }
                    }
                    if( s.needvmv )
                    {
                        s.vmv = (double)(0);
                        for(i=0; i<=n-1; i++)
                        {
                            s.mv.ptr.p_double[i] = (double)(0);
                            for(j=0; j<=n-1; j++)
                            {
                                s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                            }
                            s.vmv = s.vmv+s.mv.ptr.p_double[i]*s.x.ptr.p_double[i];
                        }
                    }
                    if( s.needprec )
                    {
                        for(i=0; i<=n-1; i++)
                        {
                            s.pv.ptr.p_double[i] = s.x.ptr.p_double[i];
                        }
                    }
                }
                lincgresults(&s, &x0, &rep, _state);
                
                /*
                 * Check result
                 */
                err = 0.0;
                for(i=0; i<=n-1; i++)
                {
                    tmp = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        tmp = tmp+a.ptr.pp_double[i][j]*x0.ptr.p_double[j];
                    }
                    err = err+ae_sqr(b.ptr.p_double[i]-tmp, _state);
                }
                err = ae_sqrt(err, _state);
                if( ae_fp_greater(err,eps) )
                {
                    if( !silent )
                    {
                        printf("ComplexRes::fail\n");
                        printf("IterationsCount=%0d\n",
                            (int)(rep.iterationscount));
                        printf("NMV=%0d\n",
                            (int)(rep.nmv));
                        printf("TerminationType=%0d\n",
                            (int)(rep.terminationtype));
                        printf("X and Xs...\n");
                        for(j=0; j<=n-1; j++)
                        {
                            printf("x[%0d]=%0.10f; xs[%0d]=%0.10f\n",
                                (int)(j),
                                (double)(x0.ptr.p_double[j]),
                                (int)(j),
                                (double)(xs.ptr.p_double[j]));
                        }
                    }
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
    }
    
    /*
     *test has been passed
     */
    if( !silent )
    {
        printf("ComplexRes::Ok\n");
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function check, that XUpdated return State.X=X0 at zero iteration and
State.X=X(algorithm result) at last.

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlincgunit_basictestx(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    lincgstate s;
    lincgreport rep;
    ae_matrix a;
    ae_vector b;
    ae_vector x0;
    ae_vector x00;
    ae_vector x01;
    ae_int_t n;
    ae_int_t sz;
    double c;
    ae_int_t i;
    ae_int_t j;
    double mx;
    ae_int_t iters;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _lincgstate_init(&s, _state);
    _lincgreport_init(&rep, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x00, 0, DT_REAL, _state);
    ae_vector_init(&x01, 0, DT_REAL, _state);

    sz = 5;
    mx = (double)(100);
    for(n=1; n<=sz; n++)
    {
        
        /*
         * Generate:
         * * random A with norm NA (equal to 1.0),
         * * random right part B whose elements are uniformly distributed in [-MX,+MX]
         * * random starting point X0 whose elements are uniformly distributed in [-MX,+MX]
         */
        c = (testlincgunit_maxcond-1)*ae_randomreal(_state)+1;
        spdmatrixrndcond(n, c, &a, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&x0, n, _state);
        ae_vector_set_length(&x00, n, _state);
        ae_vector_set_length(&x01, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = mx*(2*ae_randomreal(_state)-1);
            b.ptr.p_double[i] = mx*(2*ae_randomreal(_state)-1);
        }
        
        /*
         * Solve, save first and last reported points to x00 and x01
         */
        lincgcreate(n, &s, _state);
        lincgsetb(&s, &b, _state);
        lincgsetstartingpoint(&s, &x0, _state);
        lincgsetxrep(&s, ae_true, _state);
        lincgsetcond(&s, (double)(0), n, _state);
        iters = 0;
        while(lincgiteration(&s, _state))
        {
            if( s.needmv )
            {
                for(i=0; i<=n-1; i++)
                {
                    s.mv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                    }
                }
            }
            if( s.needvmv )
            {
                s.vmv = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    s.mv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                    }
                    s.vmv = s.vmv+s.mv.ptr.p_double[i]*s.x.ptr.p_double[i];
                }
            }
            if( s.needprec )
            {
                for(i=0; i<=n-1; i++)
                {
                    s.pv.ptr.p_double[i] = s.x.ptr.p_double[i];
                }
            }
            if( s.xupdated )
            {
                if( iters==0 )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        x00.ptr.p_double[i] = s.x.ptr.p_double[i];
                    }
                }
                if( iters==n )
                {
                    for(i=0; i<=n-1; i++)
                    {
                        x01.ptr.p_double[i] = s.x.ptr.p_double[i];
                    }
                }
                iters = iters+1;
            }
        }
        
        /*
         * Check first and last points
         */
        for(i=0; i<=n-1; i++)
        {
            if( ae_fp_neq(x00.ptr.p_double[i],x0.ptr.p_double[i]) )
            {
                if( !silent )
                {
                    printf("BasicTestX::fail\n");
                    printf("IterationsCount=%0d\n",
                        (int)(rep.iterationscount));
                    printf("NMV=%0d\n",
                        (int)(rep.nmv));
                    printf("TerminationType=%0d\n",
                        (int)(rep.terminationtype));
                    for(j=0; j<=n-1; j++)
                    {
                        printf("x0=%0.5f; x00=%0.5f;\n",
                            (double)(x0.ptr.p_double[j]),
                            (double)(x00.ptr.p_double[j]));
                    }
                }
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
        lincgresults(&s, &x0, &rep, _state);
        for(i=0; i<=n-1; i++)
        {
            if( ae_fp_neq(x01.ptr.p_double[i],x0.ptr.p_double[i]) )
            {
                if( !silent )
                {
                    printf("BasicTestX::fail\n");
                    printf("IterationsCount=%0d\n",
                        (int)(rep.iterationscount));
                    printf("NMV=%0d\n",
                        (int)(rep.nmv));
                    printf("TerminationType=%0d\n",
                        (int)(rep.terminationtype));
                    for(j=0; j<=n-1; j++)
                    {
                        printf("x0=%0.5f; x01=%0.5f;\n",
                            (double)(x0.ptr.p_double[j]),
                            (double)(x01.ptr.p_double[j]));
                    }
                }
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
    }
    
    /*
     *test has been passed
     */
    if( !silent )
    {
        printf("BasicTestIters::Ok\n");
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function checks that XUpdated returns correct State.R2. It creates
large badly conditioned problem (N=50), which should be large enough and
ill-conditioned enough to cause periodic recalculation of R.

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlincgunit_testrcorrectness(ae_bool silent,
     ae_state *_state)
{
    ae_frame _frame_block;
    lincgstate s;
    lincgreport rep;
    ae_matrix a;
    ae_vector b;
    ae_int_t n;
    double c;
    ae_int_t i;
    ae_int_t j;
    double r2;
    double v;
    double rtol;
    ae_int_t maxits;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _lincgstate_init(&s, _state);
    _lincgreport_init(&rep, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);

    rtol = 1.0E6*ae_machineepsilon;
    n = 50;
    maxits = n/2;
    c = (double)(10000);
    spdmatrixrndcond(n, c, &a, _state);
    ae_vector_set_length(&b, n, _state);
    for(i=0; i<=n-1; i++)
    {
        b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
    }
    lincgcreate(n, &s, _state);
    lincgsetb(&s, &b, _state);
    lincgsetxrep(&s, ae_true, _state);
    lincgsetcond(&s, (double)(0), maxits, _state);
    while(lincgiteration(&s, _state))
    {
        if( s.needmv )
        {
            for(i=0; i<=n-1; i++)
            {
                s.mv.ptr.p_double[i] = (double)(0);
                for(j=0; j<=n-1; j++)
                {
                    s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                }
            }
        }
        if( s.needvmv )
        {
            s.vmv = (double)(0);
            for(i=0; i<=n-1; i++)
            {
                s.mv.ptr.p_double[i] = (double)(0);
                for(j=0; j<=n-1; j++)
                {
                    s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                }
                s.vmv = s.vmv+s.mv.ptr.p_double[i]*s.x.ptr.p_double[i];
            }
        }
        if( s.needprec )
        {
            for(i=0; i<=n-1; i++)
            {
                s.pv.ptr.p_double[i] = s.x.ptr.p_double[i];
            }
        }
        if( s.xupdated )
        {
            
            /*
             * calculate R2, compare with value returned in state.R2
             */
            r2 = (double)(0);
            for(i=0; i<=n-1; i++)
            {
                v = (double)(0);
                for(j=0; j<=n-1; j++)
                {
                    v = v+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                }
                r2 = r2+ae_sqr(v-b.ptr.p_double[i], _state);
            }
            if( ae_fp_greater(ae_fabs(r2-s.r2, _state),rtol) )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
    }
    lincgresults(&s, &b, &rep, _state);
    if( rep.iterationscount!=maxits )
    {
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function check, that number of iterations are't more than MaxIts.

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlincgunit_basictestiters(ae_bool silent,
     ae_state *_state)
{
    ae_frame _frame_block;
    lincgstate s;
    lincgreport rep;
    ae_matrix a;
    ae_vector b;
    ae_vector x0;
    ae_int_t n;
    ae_int_t sz;
    double c;
    ae_int_t i;
    ae_int_t j;
    double mx;
    ae_int_t iters;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _lincgstate_init(&s, _state);
    _lincgreport_init(&rep, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);

    sz = 5;
    mx = (double)(100);
    for(n=1; n<=sz; n++)
    {
        
        /*
         * Generate:
         * * random A with norm NA (equal to 1.0),
         * * random right part B whose elements are uniformly distributed in [-MX,+MX]
         * * random starting point X0 whose elements are uniformly distributed in [-MX,+MX]
         */
        c = (testlincgunit_maxcond-1)*ae_randomreal(_state)+1;
        spdmatrixrndcond(n, c, &a, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&x0, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = mx*(2*ae_randomreal(_state)-1);
            b.ptr.p_double[i] = mx*(2*ae_randomreal(_state)-1);
        }
        
        /*
         * Solve
         */
        lincgcreate(n, &s, _state);
        lincgsetb(&s, &b, _state);
        lincgsetstartingpoint(&s, &x0, _state);
        lincgsetxrep(&s, ae_true, _state);
        lincgsetcond(&s, (double)(0), n, _state);
        iters = 0;
        while(lincgiteration(&s, _state))
        {
            if( s.needmv )
            {
                for(i=0; i<=n-1; i++)
                {
                    s.mv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                    }
                }
            }
            if( s.needvmv )
            {
                s.vmv = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    s.mv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                    }
                    s.vmv = s.vmv+s.mv.ptr.p_double[i]*s.x.ptr.p_double[i];
                }
            }
            if( s.needprec )
            {
                for(i=0; i<=n-1; i++)
                {
                    s.pv.ptr.p_double[i] = s.x.ptr.p_double[i];
                }
            }
            if( s.xupdated )
            {
                iters = iters+1;
            }
        }
        lincgresults(&s, &x0, &rep, _state);
        
        /*
         * Check
         */
        if( iters!=rep.iterationscount+1||iters>n+1 )
        {
            if( !silent )
            {
                printf("BasicTestIters::fail\n");
                printf("IterationsCount=%0d\n",
                    (int)(rep.iterationscount));
                printf("NMV=%0d\n",
                    (int)(rep.nmv));
                printf("TerminationType=%0d\n",
                    (int)(rep.terminationtype));
                printf("Iters=%0d\n",
                    (int)(iters));
            }
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Restart problem
         */
        c = (testlincgunit_maxcond-1)*ae_randomreal(_state)+1;
        spdmatrixrndcond(n, c, &a, _state);
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = mx*(2*ae_randomreal(_state)-1);
            b.ptr.p_double[i] = mx*(2*ae_randomreal(_state)-1);
        }
        lincgsetstartingpoint(&s, &x0, _state);
        lincgrestart(&s, _state);
        lincgsetb(&s, &b, _state);
        iters = 0;
        while(lincgiteration(&s, _state))
        {
            if( s.needmv )
            {
                for(i=0; i<=n-1; i++)
                {
                    s.mv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                    }
                }
            }
            if( s.needvmv )
            {
                s.vmv = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    s.mv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                    }
                    s.vmv = s.vmv+s.mv.ptr.p_double[i]*s.x.ptr.p_double[i];
                }
            }
            if( s.needprec )
            {
                for(i=0; i<=n-1; i++)
                {
                    s.pv.ptr.p_double[i] = s.x.ptr.p_double[i];
                }
            }
            if( s.xupdated )
            {
                iters = iters+1;
            }
        }
        lincgresults(&s, &x0, &rep, _state);
        
        /*
         *check
         */
        if( iters!=rep.iterationscount+1||iters>n+1 )
        {
            if( !silent )
            {
                printf("BasicTestIters::fail\n");
                printf("IterationsCount=%0d\n",
                    (int)(rep.iterationscount));
                printf("NMV=%0d\n",
                    (int)(rep.nmv));
                printf("TerminationType=%0d\n",
                    (int)(rep.terminationtype));
                printf("Iters=%0d\n",
                    (int)(iters));
            }
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    
    /*
     *test has been passed
     */
    if( !silent )
    {
        printf("BasicTestIters::Ok\n");
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This function check, that programmed method is Krylov subspace methed.

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlincgunit_krylovsubspacetest(ae_bool silent,
     ae_state *_state)
{
    ae_frame _frame_block;
    lincgstate s;
    lincgreport rep;
    ae_matrix a;
    ae_vector b;
    ae_vector x0;
    ae_matrix ksr;
    ae_vector r0;
    ae_vector tarray;
    ae_matrix mtx;
    ae_int_t n;
    ae_int_t sz;
    double c;
    ae_int_t i;
    ae_int_t j;
    ae_int_t l;
    ae_int_t m;
    double mx;
    double tmp;
    double normr0;
    ae_int_t numofit;
    ae_int_t maxits;
    ae_int_t k2;
    double eps;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _lincgstate_init(&s, _state);
    _lincgreport_init(&rep, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_matrix_init(&ksr, 0, 0, DT_REAL, _state);
    ae_vector_init(&r0, 0, DT_REAL, _state);
    ae_vector_init(&tarray, 0, DT_REAL, _state);
    ae_matrix_init(&mtx, 0, 0, DT_REAL, _state);

    eps = 1.0E-6;
    maxits = 3;
    sz = 5;
    mx = (double)(100);
    for(n=1; n<=sz; n++)
    {
        
        /*
         * Generate:
         * * random A with unit norm
         * * cond(A) in [0.5*MaxCond, 1.0*MaxCond]
         * * random x0 and b such that |A*x0-b| is large enough for algorithm to make at least one iteration.
         *
         * IMPORTANT: it is very important to have cond(A) both (1) not very large and
         *            (2) not very small. Large cond(A) will make our problem ill-conditioned,
         *            thus analytic properties won't hold. Small cond(A), from the other side,
         *            will give us rapid convergence of the algorithm - in fact, too rapid.
         *            Krylov basis will be dominated by numerical noise and test may fail.
         */
        c = testlincgunit_maxcond*(0.5*ae_randomreal(_state)+0.5);
        spdmatrixrndcond(n, c, &a, _state);
        ae_matrix_set_length(&mtx, n+1, n, _state);
        ae_matrix_set_length(&ksr, n, n, _state);
        ae_vector_set_length(&r0, n, _state);
        ae_vector_set_length(&tarray, n, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&x0, n, _state);
        do
        {
            for(i=0; i<=n-1; i++)
            {
                x0.ptr.p_double[i] = mx*(2*ae_randomreal(_state)-1);
                b.ptr.p_double[i] = mx*(2*ae_randomreal(_state)-1);
            }
            normr0 = (double)(0);
            for(i=0; i<=n-1; i++)
            {
                tmp = (double)(0);
                for(j=0; j<=n-1; j++)
                {
                    tmp = tmp+a.ptr.pp_double[i][j]*x0.ptr.p_double[j];
                }
                r0.ptr.p_double[i] = b.ptr.p_double[i]-tmp;
                normr0 = normr0+r0.ptr.p_double[i]*r0.ptr.p_double[i];
            }
        }
        while(ae_fp_less_eq(ae_sqrt(normr0, _state),eps));
        
        /*
         * Fill KSR by {r0, A*r0, A^2*r0, ... }
         */
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                ksr.ptr.pp_double[i][j] = r0.ptr.p_double[j];
            }
            for(j=0; j<=i-1; j++)
            {
                for(l=0; l<=n-1; l++)
                {
                    tarray.ptr.p_double[l] = (double)(0);
                    for(m=0; m<=n-1; m++)
                    {
                        tarray.ptr.p_double[l] = tarray.ptr.p_double[l]+a.ptr.pp_double[l][m]*ksr.ptr.pp_double[i][m];
                    }
                }
                for(l=0; l<=n-1; l++)
                {
                    ksr.ptr.pp_double[i][l] = tarray.ptr.p_double[l];
                }
            }
        }
        
        /*
         * Solve system, record intermediate points for futher analysis.
         * NOTE: we set update frequency of R to 2 in order to test that R is updated correctly
         */
        lincgcreate(n, &s, _state);
        lincgsetb(&s, &b, _state);
        lincgsetstartingpoint(&s, &x0, _state);
        lincgsetxrep(&s, ae_true, _state);
        lincgsetcond(&s, (double)(0), n, _state);
        lincgsetrupdatefreq(&s, 2, _state);
        numofit = 0;
        while(lincgiteration(&s, _state))
        {
            if( s.needmv )
            {
                for(i=0; i<=n-1; i++)
                {
                    s.mv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                    }
                }
            }
            if( s.needvmv )
            {
                s.vmv = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    s.mv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                    }
                    s.vmv = s.vmv+s.mv.ptr.p_double[i]*s.x.ptr.p_double[i];
                }
            }
            if( s.needprec )
            {
                for(i=0; i<=n-1; i++)
                {
                    s.pv.ptr.p_double[i] = s.x.ptr.p_double[i];
                }
            }
            if( s.xupdated )
            {
                for(i=0; i<=n-1; i++)
                {
                    mtx.ptr.pp_double[numofit][i] = s.x.ptr.p_double[i];
                }
                numofit = numofit+1;
            }
        }
        
        /*
         * Check that I-th step S_i=X[I+1]-X[i] belongs to I-th Krylov subspace.
         * Checks are done for first K2 steps, with K2 small enough to avoid
         * numerical errors.
         */
        if( n<=maxits )
        {
            k2 = n;
        }
        else
        {
            k2 = maxits;
        }
        for(i=0; i<=k2-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                tarray.ptr.p_double[j] = mtx.ptr.pp_double[i+1][j]-mtx.ptr.pp_double[i][j];
            }
            if( !testlincgunit_frombasis(&tarray, &ksr, n, i+1, testlincgunit_e0, _state) )
            {
                if( !silent )
                {
                    printf("KrylovSubspaceTest::FAIL\n");
                    printf("Size=%0d; Iters=%0d;\n",
                        (int)(n),
                        (int)(i));
                }
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
    }
    if( !silent )
    {
        printf("KrylovSubspaceTest::OK\n");
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing  LinCgSolveSparse. This function  prepare problem with
a known solution 'Xs'(A*Xs-b=0). There b is A*Xs. After, function calculate
result by LinCGSolveSparse and compares it with 'Xs'.

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlincgunit_sparsetest(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    lincgstate s;
    lincgreport rep;
    ae_matrix a;
    ae_vector b;
    ae_vector xs;
    ae_vector x0;
    ae_vector x1;
    sparsematrix uppera;
    sparsematrix lowera;
    double err;
    ae_int_t n;
    ae_int_t sz;
    double c;
    ae_int_t i;
    ae_int_t j;
    double mx;
    double eps;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _lincgstate_init(&s, _state);
    _lincgreport_init(&rep, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&xs, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    _sparsematrix_init(&uppera, _state);
    _sparsematrix_init(&lowera, _state);

    sz = 5;
    mx = (double)(100);
    for(n=1; n<=sz; n++)
    {
        
        /*
         * Generate:
         * * random A with unit norm
         * * random X0 (starting point) and XS (known solution)
         * Copy dense A to sparse SA
         */
        c = (testlincgunit_maxcond-1)*ae_randomreal(_state)+1;
        spdmatrixrndcond(n, c, &a, _state);
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&xs, n, _state);
        for(i=0; i<=n-1; i++)
        {
            xs.ptr.p_double[i] = mx*(2*ae_randomreal(_state)-1);
        }
        eps = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            b.ptr.p_double[i] = (double)(0);
            for(j=0; j<=n-1; j++)
            {
                b.ptr.p_double[i] = b.ptr.p_double[i]+a.ptr.pp_double[i][j]*xs.ptr.p_double[j];
            }
            eps = eps+b.ptr.p_double[i]*b.ptr.p_double[i];
        }
        eps = 1.0E-6*ae_sqrt(eps, _state);
        sparsecreate(n, n, 0, &uppera, _state);
        sparsecreate(n, n, 0, &lowera, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( j>=i )
                {
                    sparseset(&uppera, i, j, a.ptr.pp_double[i][j], _state);
                }
                if( j<=i )
                {
                    sparseset(&lowera, i, j, a.ptr.pp_double[i][j], _state);
                }
            }
        }
        sparseconverttocrs(&uppera, _state);
        sparseconverttocrs(&lowera, _state);
        
        /*
         * Test upper triangle
         */
        lincgcreate(n, &s, _state);
        lincgsetcond(&s, (double)(0), n, _state);
        lincgsolvesparse(&s, &uppera, ae_true, &b, _state);
        lincgresults(&s, &x0, &rep, _state);
        err = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            err = err+ae_sqr(x0.ptr.p_double[i]-xs.ptr.p_double[i], _state);
        }
        err = ae_sqrt(err, _state);
        if( ae_fp_greater(err,eps) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Test lower triangle
         */
        lincgcreate(n, &s, _state);
        lincgsetcond(&s, (double)(0), n, _state);
        lincgsolvesparse(&s, &lowera, ae_false, &b, _state);
        lincgresults(&s, &x1, &rep, _state);
        err = (double)(0);
        for(i=0; i<=n-1; i++)
        {
            err = err+ae_sqr(x1.ptr.p_double[i]-xs.ptr.p_double[i], _state);
        }
        err = ae_sqrt(err, _state);
        if( ae_fp_greater(err,eps) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Function for testing  the preconditioned conjugate gradient method.

  -- ALGLIB --
     Copyright 14.11.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool testlincgunit_precondtest(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    lincgstate s;
    lincgreport rep;
    ae_matrix a;
    ae_matrix ta;
    sparsematrix sa;
    ae_vector m;
    ae_matrix mtx;
    ae_matrix mtprex;
    ae_vector de;
    ae_vector rde;
    ae_vector b;
    ae_vector tb;
    ae_vector d;
    ae_vector xe;
    ae_vector x0;
    ae_vector tx0;
    ae_vector err;
    ae_int_t n;
    ae_int_t sz;
    ae_int_t numofit;
    double c;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double eps;
    ae_bool bflag;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _lincgstate_init(&s, _state);
    _lincgreport_init(&rep, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&ta, 0, 0, DT_REAL, _state);
    _sparsematrix_init(&sa, _state);
    ae_vector_init(&m, 0, DT_REAL, _state);
    ae_matrix_init(&mtx, 0, 0, DT_REAL, _state);
    ae_matrix_init(&mtprex, 0, 0, DT_REAL, _state);
    ae_vector_init(&de, 0, DT_REAL, _state);
    ae_vector_init(&rde, 0, DT_REAL, _state);
    ae_vector_init(&b, 0, DT_REAL, _state);
    ae_vector_init(&tb, 0, DT_REAL, _state);
    ae_vector_init(&d, 0, DT_REAL, _state);
    ae_vector_init(&xe, 0, DT_REAL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&tx0, 0, DT_REAL, _state);
    ae_vector_init(&err, 0, DT_REAL, _state);

    
    /*
     * Test 1.
     * 
     * Preconditioned CG for A*x=b with preconditioner M=E*E' is algebraically 
     * equivalent to non-preconditioned CG for (inv(E)*A*inv(E'))*z = inv(E)*b
     * with z=E'*x.
     * 
     * We test it by generating random preconditioner, running algorithm twice -
     * one  time  for  original  problem  with  preconditioner , another one  for
     * modified problem without preconditioner.
     */
    sz = 5;
    for(n=1; n<=sz; n++)
    {
        
        /*
         * Generate:
         * * random A with unit norm
         * * random positive definite diagonal preconditioner M
         * * dE=sqrt(M)
         * * rdE=dE^(-1)
         * * tA = rdE*A*rdE
         * * random x0 and b - for original preconditioned problem
         * * tx0 and tb - for modified problem
         */
        c = (testlincgunit_maxcond-1)*ae_randomreal(_state)+1;
        spdmatrixrndcond(n, c, &a, _state);
        ae_matrix_set_length(&ta, n, n, _state);
        ae_matrix_set_length(&mtx, n+1, n, _state);
        ae_matrix_set_length(&mtprex, n+1, n, _state);
        ae_vector_set_length(&m, n, _state);
        ae_vector_set_length(&de, n, _state);
        ae_vector_set_length(&rde, n, _state);
        for(i=0; i<=n-1; i++)
        {
            m.ptr.p_double[i] = ae_randomreal(_state)+0.5;
            de.ptr.p_double[i] = ae_sqrt(m.ptr.p_double[i], _state);
            rde.ptr.p_double[i] = 1/de.ptr.p_double[i];
        }
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                ta.ptr.pp_double[i][j] = rde.ptr.p_double[i]*a.ptr.pp_double[i][j]*rde.ptr.p_double[j];
            }
        }
        ae_vector_set_length(&b, n, _state);
        ae_vector_set_length(&tb, n, _state);
        ae_vector_set_length(&x0, n, _state);
        ae_vector_set_length(&tx0, n, _state);
        ae_vector_set_length(&err, n, _state);
        for(i=0; i<=n-1; i++)
        {
            x0.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            b.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
        }
        eps = 1.0E-5;
        for(i=0; i<=n-1; i++)
        {
            tx0.ptr.p_double[i] = de.ptr.p_double[i]*x0.ptr.p_double[i];
            tb.ptr.p_double[i] = rde.ptr.p_double[i]*b.ptr.p_double[i];
        }
        
        /*
         * Solve two problems, intermediate points are saved to MtX and MtPreX
         */
        lincgcreate(n, &s, _state);
        lincgsetb(&s, &b, _state);
        lincgsetstartingpoint(&s, &x0, _state);
        lincgsetxrep(&s, ae_true, _state);
        lincgsetcond(&s, (double)(0), n, _state);
        numofit = 0;
        while(lincgiteration(&s, _state))
        {
            if( s.needmv )
            {
                for(i=0; i<=n-1; i++)
                {
                    s.mv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                    }
                }
            }
            if( s.needvmv )
            {
                s.vmv = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    s.mv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+a.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                    }
                    s.vmv = s.vmv+s.mv.ptr.p_double[i]*s.x.ptr.p_double[i];
                }
            }
            if( s.needprec )
            {
                for(i=0; i<=n-1; i++)
                {
                    s.pv.ptr.p_double[i] = s.x.ptr.p_double[i]/m.ptr.p_double[i];
                }
            }
            if( s.xupdated )
            {
                if( numofit>=mtx.rows )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
                for(i=0; i<=n-1; i++)
                {
                    mtx.ptr.pp_double[numofit][i] = s.x.ptr.p_double[i];
                }
                numofit = numofit+1;
            }
        }
        lincgsetstartingpoint(&s, &tx0, _state);
        lincgsetb(&s, &tb, _state);
        lincgrestart(&s, _state);
        numofit = 0;
        while(lincgiteration(&s, _state))
        {
            if( s.needmv )
            {
                for(i=0; i<=n-1; i++)
                {
                    s.mv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+ta.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                    }
                }
            }
            if( s.needvmv )
            {
                s.vmv = (double)(0);
                for(i=0; i<=n-1; i++)
                {
                    s.mv.ptr.p_double[i] = (double)(0);
                    for(j=0; j<=n-1; j++)
                    {
                        s.mv.ptr.p_double[i] = s.mv.ptr.p_double[i]+ta.ptr.pp_double[i][j]*s.x.ptr.p_double[j];
                    }
                    s.vmv = s.vmv+s.mv.ptr.p_double[i]*s.x.ptr.p_double[i];
                }
            }
            if( s.needprec )
            {
                for(i=0; i<=n-1; i++)
                {
                    s.pv.ptr.p_double[i] = s.x.ptr.p_double[i];
                }
            }
            if( s.xupdated )
            {
                if( numofit>=mtprex.rows )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
                for(i=0; i<=n-1; i++)
                {
                    mtprex.ptr.pp_double[numofit][i] = s.x.ptr.p_double[i];
                }
                numofit = numofit+1;
            }
        }
        
        /*
         * Compare results - sequence of points generated when solving original problem with
         * points generated by modified problem.
         */
        for(i=0; i<=numofit-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                if( ae_fp_greater(ae_fabs(mtx.ptr.pp_double[i][j]-rde.ptr.p_double[j]*mtprex.ptr.pp_double[i][j], _state),eps) )
                {
                    if( !silent )
                    {
                        printf("PrecondTest::fail\n");
                        printf("Size=%0d\n",
                            (int)(n));
                        printf("IterationsCount=%0d\n",
                            (int)(rep.iterationscount));
                        printf("NMV=%0d\n",
                            (int)(rep.nmv));
                        printf("TerminationType=%0d\n",
                            (int)(rep.terminationtype));
                        printf("X and X^...\n");
                        for(k=0; k<=n-1; k++)
                        {
                            printf("I=%0d; mtx[%0d]=%0.10f; mtx^[%0d]=%0.10f\n",
                                (int)(i),
                                (int)(k),
                                (double)(mtx.ptr.pp_double[i][k]),
                                (int)(k),
                                (double)(mtprex.ptr.pp_double[i][k]));
                        }
                    }
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
    }
    
    /*
     * Test 2.
     *
     * We test automatic diagonal preconditioning used by SolveSparse.
     * In order to do so we:
     * 1. generate 20*20 matrix A0 with condition number equal to 1.0E1
     * 2. generate random "exact" solution xe and right part b=A0*xe
     * 3. generate random ill-conditioned diagonal scaling matrix D with
     *    condition number equal to 1.0E50:
     * 4. transform A*x=b into badly scaled problem:
     *    A0*x0=b0
     *    A0*D*(inv(D)*x0)=b0
     *    (D*A0*D)*(inv(D)*x0)=(D*b0)
     *    finally we got new problem A*x=b with A=D*A0*D, b=D*b0, x=inv(D)*x0
     *
     * Then we solve A*x=b:
     * 1. with default preconditioner
     * 2. with explicitly activayed diagonal preconditioning
     * 3. with unit preconditioner.
     * 1st and 2nd solutions must be close to xe, 3rd solution must be very
     * far from the true one.
     */
    n = 20;
    spdmatrixrndcond(n, 1.0E1, &ta, _state);
    ae_vector_set_length(&xe, n, _state);
    for(i=0; i<=n-1; i++)
    {
        xe.ptr.p_double[i] = randomnormal(_state);
    }
    ae_vector_set_length(&b, n, _state);
    for(i=0; i<=n-1; i++)
    {
        b.ptr.p_double[i] = (double)(0);
        for(j=0; j<=n-1; j++)
        {
            b.ptr.p_double[i] = b.ptr.p_double[i]+ta.ptr.pp_double[i][j]*xe.ptr.p_double[j];
        }
    }
    ae_vector_set_length(&d, n, _state);
    for(i=0; i<=n-1; i++)
    {
        d.ptr.p_double[i] = ae_pow((double)(10), 100*ae_randomreal(_state)-50, _state);
    }
    ae_matrix_set_length(&a, n, n, _state);
    sparsecreate(n, n, n*n, &sa, _state);
    for(i=0; i<=n-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            a.ptr.pp_double[i][j] = d.ptr.p_double[i]*ta.ptr.pp_double[i][j]*d.ptr.p_double[j];
            sparseset(&sa, i, j, a.ptr.pp_double[i][j], _state);
        }
        b.ptr.p_double[i] = b.ptr.p_double[i]*d.ptr.p_double[i];
        xe.ptr.p_double[i] = xe.ptr.p_double[i]/d.ptr.p_double[i];
    }
    sparseconverttocrs(&sa, _state);
    lincgcreate(n, &s, _state);
    lincgsetcond(&s, (double)(0), 2*n, _state);
    lincgsolvesparse(&s, &sa, ae_true, &b, _state);
    lincgresults(&s, &x0, &rep, _state);
    if( rep.terminationtype<=0 )
    {
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_greater(ae_fabs(xe.ptr.p_double[i]-x0.ptr.p_double[i], _state),5.0E-2/d.ptr.p_double[i]) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    lincgsetprecunit(&s, _state);
    lincgsolvesparse(&s, &sa, ae_true, &b, _state);
    lincgresults(&s, &x0, &rep, _state);
    if( rep.terminationtype>0 )
    {
        bflag = ae_false;
        for(i=0; i<=n-1; i++)
        {
            bflag = bflag||ae_fp_greater(ae_fabs(xe.ptr.p_double[i]-x0.ptr.p_double[i], _state),5.0E-2/d.ptr.p_double[i]);
        }
        if( !bflag )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    lincgsetprecdiag(&s, _state);
    lincgsolvesparse(&s, &sa, ae_true, &b, _state);
    lincgresults(&s, &x0, &rep, _state);
    if( rep.terminationtype<=0 )
    {
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    for(i=0; i<=n-1; i++)
    {
        if( ae_fp_greater(ae_fabs(xe.ptr.p_double[i]-x0.ptr.p_double[i], _state),5.0E-2/d.ptr.p_double[i]) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    
    /*
     *test has been passed
     */
    if( !silent )
    {
        printf("PrecondTest::Ok\n");
    }
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Orthogonalization by Gram-Shmidt method.
*************************************************************************/
static void testlincgunit_gramshmidtortnorm(/* Real    */ ae_matrix* a,
     ae_int_t n,
     ae_int_t k,
     double eps,
     /* Real    */ ae_matrix* b,
     ae_int_t* k2,
     ae_state *_state)
{
    double scaling;
    double tmp;
    double e;
    ae_int_t i;
    ae_int_t j;
    ae_int_t l;
    ae_int_t m;
    double sc;

    ae_matrix_clear(b);
    *k2 = 0;

    *k2 = 0;
    scaling = (double)(0);
    ae_matrix_set_length(b, k, n, _state);
    for(i=0; i<=k-1; i++)
    {
        tmp = (double)(0);
        for(j=0; j<=n-1; j++)
        {
            tmp = tmp+a->ptr.pp_double[i][j]*a->ptr.pp_double[i][j];
        }
        if( ae_fp_greater(tmp,scaling) )
        {
            scaling = tmp;
        }
    }
    scaling = ae_sqrt(scaling, _state);
    e = eps*scaling;
    for(i=0; i<=k-1; i++)
    {
        tmp = (double)(0);
        for(j=0; j<=n-1; j++)
        {
            b->ptr.pp_double[*k2][j] = a->ptr.pp_double[i][j];
            tmp = tmp+a->ptr.pp_double[i][j]*a->ptr.pp_double[i][j];
        }
        tmp = ae_sqrt(tmp, _state);
        if( ae_fp_less_eq(tmp,e) )
        {
            continue;
        }
        for(j=0; j<=*k2-1; j++)
        {
            sc = (double)(0);
            for(m=0; m<=n-1; m++)
            {
                sc = sc+b->ptr.pp_double[*k2][m]*b->ptr.pp_double[j][m];
            }
            for(l=0; l<=n-1; l++)
            {
                b->ptr.pp_double[*k2][l] = b->ptr.pp_double[*k2][l]-sc*b->ptr.pp_double[j][l];
            }
        }
        tmp = (double)(0);
        for(j=0; j<=n-1; j++)
        {
            tmp = tmp+b->ptr.pp_double[*k2][j]*b->ptr.pp_double[*k2][j];
        }
        tmp = ae_sqrt(tmp, _state);
        if( ae_fp_less_eq(tmp,e) )
        {
            continue;
        }
        else
        {
            for(j=0; j<=n-1; j++)
            {
                b->ptr.pp_double[*k2][j] = b->ptr.pp_double[*k2][j]/tmp;
            }
        }
        *k2 = *k2+1;
    }
}


/*************************************************************************
Checks that a vector belongs to the basis.
*************************************************************************/
static ae_bool testlincgunit_frombasis(/* Real    */ ae_vector* x,
     /* Real    */ ae_matrix* basis,
     ae_int_t n,
     ae_int_t k,
     double eps,
     ae_state *_state)
{
    ae_frame _frame_block;
    double normx;
    ae_matrix ortnormbasis;
    ae_int_t k2;
    ae_int_t i;
    ae_int_t j;
    double alpha;
    ae_vector alphas;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&ortnormbasis, 0, 0, DT_REAL, _state);
    ae_vector_init(&alphas, 0, DT_REAL, _state);

    ae_vector_set_length(&alphas, k, _state);
    
    /*
     *calculating NORM for X
     */
    normx = (double)(0);
    for(i=0; i<=n-1; i++)
    {
        normx = normx+x->ptr.p_double[i]*x->ptr.p_double[i];
    }
    normx = ae_sqrt(normx, _state);
    
    /*
     *Gram-Shmidt method
     */
    testlincgunit_gramshmidtortnorm(basis, n, k, eps, &ortnormbasis, &k2, _state);
    for(i=0; i<=k2-1; i++)
    {
        alpha = (double)(0);
        for(j=0; j<=n-1; j++)
        {
            alpha = alpha+x->ptr.p_double[j]*ortnormbasis.ptr.pp_double[i][j];
        }
        alphas.ptr.p_double[i] = alpha;
    }
    
    /*
     *check
     */
    for(i=0; i<=n-1; i++)
    {
        alpha = (double)(0);
        for(j=0; j<=k2-1; j++)
        {
            alpha = alpha+alphas.ptr.p_double[j]*ortnormbasis.ptr.pp_double[j][i];
        }
        if( ae_fp_greater(ae_fabs(x->ptr.p_double[i]-alpha, _state),normx*eps) )
        {
            result = ae_false;
            ae_frame_leave(_state);
            return result;
        }
    }
    result = ae_true;
    ae_frame_leave(_state);
    return result;
}


/*$ End $*/
