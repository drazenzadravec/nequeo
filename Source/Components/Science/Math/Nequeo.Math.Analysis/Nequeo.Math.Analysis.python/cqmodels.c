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
#include "cqmodels.h"


/*$ Declarations $*/
static ae_int_t cqmodels_newtonrefinementits = 3;
static ae_bool cqmodels_cqmrebuild(convexquadraticmodel* s,
     ae_state *_state);
static void cqmodels_cqmsolveea(convexquadraticmodel* s,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* tmp,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
This subroutine is used to initialize CQM. By default, empty NxN model  is
generated, with Alpha=Lambda=Theta=0.0 and zero b.

Previously allocated buffer variables are reused as much as possible.

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
void cqminit(ae_int_t n, convexquadraticmodel* s, ae_state *_state)
{
    ae_int_t i;


    s->n = n;
    s->k = 0;
    s->nfree = n;
    s->ecakind = -1;
    s->alpha = 0.0;
    s->tau = 0.0;
    s->theta = 0.0;
    s->ismaintermchanged = ae_true;
    s->issecondarytermchanged = ae_true;
    s->islineartermchanged = ae_true;
    s->isactivesetchanged = ae_true;
    bvectorsetlengthatleast(&s->activeset, n, _state);
    rvectorsetlengthatleast(&s->xc, n, _state);
    rvectorsetlengthatleast(&s->eb, n, _state);
    rvectorsetlengthatleast(&s->tq1, n, _state);
    rvectorsetlengthatleast(&s->txc, n, _state);
    rvectorsetlengthatleast(&s->tb, n, _state);
    rvectorsetlengthatleast(&s->b, s->n, _state);
    rvectorsetlengthatleast(&s->tk1, s->n, _state);
    for(i=0; i<=n-1; i++)
    {
        s->activeset.ptr.p_bool[i] = ae_false;
        s->xc.ptr.p_double[i] = 0.0;
        s->b.ptr.p_double[i] = 0.0;
    }
}


/*************************************************************************
This subroutine changes main quadratic term of the model.

INPUT PARAMETERS:
    S       -   model
    A       -   NxN matrix, only upper or lower triangle is referenced
    IsUpper -   True, when matrix is stored in upper triangle
    Alpha   -   multiplier; when Alpha=0, A is not referenced at all

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
void cqmseta(convexquadraticmodel* s,
     /* Real    */ ae_matrix* a,
     ae_bool isupper,
     double alpha,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;


    ae_assert(ae_isfinite(alpha, _state)&&ae_fp_greater_eq(alpha,(double)(0)), "CQMSetA: Alpha<0 or is not finite number", _state);
    ae_assert(ae_fp_eq(alpha,(double)(0))||isfinitertrmatrix(a, s->n, isupper, _state), "CQMSetA: A is not finite NxN matrix", _state);
    s->alpha = alpha;
    if( ae_fp_greater(alpha,(double)(0)) )
    {
        rmatrixsetlengthatleast(&s->a, s->n, s->n, _state);
        rmatrixsetlengthatleast(&s->ecadense, s->n, s->n, _state);
        rmatrixsetlengthatleast(&s->tq2dense, s->n, s->n, _state);
        for(i=0; i<=s->n-1; i++)
        {
            for(j=i; j<=s->n-1; j++)
            {
                if( isupper )
                {
                    v = a->ptr.pp_double[i][j];
                }
                else
                {
                    v = a->ptr.pp_double[j][i];
                }
                s->a.ptr.pp_double[i][j] = v;
                s->a.ptr.pp_double[j][i] = v;
            }
        }
    }
    s->ismaintermchanged = ae_true;
}


/*************************************************************************
This subroutine changes main quadratic term of the model.

INPUT PARAMETERS:
    S       -   model
    A       -   possibly preallocated buffer
    
OUTPUT PARAMETERS:
    A       -   NxN matrix, full matrix is returned.
                Zero matrix is returned if model is empty.

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
void cqmgeta(convexquadraticmodel* s,
     /* Real    */ ae_matrix* a,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double v;
    ae_int_t n;


    n = s->n;
    rmatrixsetlengthatleast(a, n, n, _state);
    if( ae_fp_greater(s->alpha,(double)(0)) )
    {
        v = s->alpha;
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a->ptr.pp_double[i][j] = v*s->a.ptr.pp_double[i][j];
            }
        }
    }
    else
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                a->ptr.pp_double[i][j] = 0.0;
            }
        }
    }
}


/*************************************************************************
This subroutine rewrites diagonal of the main quadratic term of the  model
(dense  A)  by  vector  Z/Alpha (current value of the Alpha coefficient is
used).

IMPORTANT: in  case  model  has  no  dense  quadratic  term, this function
           allocates N*N dense matrix of zeros, and fills its diagonal  by
           non-zero values.

INPUT PARAMETERS:
    S       -   model
    Z       -   new diagonal, array[N]

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
void cqmrewritedensediagonal(convexquadraticmodel* s,
     /* Real    */ ae_vector* z,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;


    n = s->n;
    if( ae_fp_eq(s->alpha,(double)(0)) )
    {
        rmatrixsetlengthatleast(&s->a, s->n, s->n, _state);
        rmatrixsetlengthatleast(&s->ecadense, s->n, s->n, _state);
        rmatrixsetlengthatleast(&s->tq2dense, s->n, s->n, _state);
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                s->a.ptr.pp_double[i][j] = 0.0;
            }
        }
        s->alpha = 1.0;
    }
    for(i=0; i<=s->n-1; i++)
    {
        s->a.ptr.pp_double[i][i] = z->ptr.p_double[i]/s->alpha;
    }
    s->ismaintermchanged = ae_true;
}


/*************************************************************************
This subroutine changes diagonal quadratic term of the model.

INPUT PARAMETERS:
    S       -   model
    D       -   array[N], semidefinite diagonal matrix
    Tau     -   multiplier; when Tau=0, D is not referenced at all

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
void cqmsetd(convexquadraticmodel* s,
     /* Real    */ ae_vector* d,
     double tau,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(ae_isfinite(tau, _state)&&ae_fp_greater_eq(tau,(double)(0)), "CQMSetD: Tau<0 or is not finite number", _state);
    ae_assert(ae_fp_eq(tau,(double)(0))||isfinitevector(d, s->n, _state), "CQMSetD: D is not finite Nx1 vector", _state);
    s->tau = tau;
    if( ae_fp_greater(tau,(double)(0)) )
    {
        rvectorsetlengthatleast(&s->d, s->n, _state);
        rvectorsetlengthatleast(&s->ecadiag, s->n, _state);
        rvectorsetlengthatleast(&s->tq2diag, s->n, _state);
        for(i=0; i<=s->n-1; i++)
        {
            ae_assert(ae_fp_greater_eq(d->ptr.p_double[i],(double)(0)), "CQMSetD: D[i]<0", _state);
            s->d.ptr.p_double[i] = d->ptr.p_double[i];
        }
    }
    s->ismaintermchanged = ae_true;
}


/*************************************************************************
This subroutine drops main quadratic term A from the model. It is same  as
call  to  CQMSetA()  with  zero  A,   but gives better performance because
algorithm  knows  that  matrix  is  zero  and  can  optimize    subsequent
calculations.

INPUT PARAMETERS:
    S       -   model

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
void cqmdropa(convexquadraticmodel* s, ae_state *_state)
{


    s->alpha = 0.0;
    s->ismaintermchanged = ae_true;
}


/*************************************************************************
This subroutine changes linear term of the model

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
void cqmsetb(convexquadraticmodel* s,
     /* Real    */ ae_vector* b,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(isfinitevector(b, s->n, _state), "CQMSetB: B is not finite vector", _state);
    rvectorsetlengthatleast(&s->b, s->n, _state);
    for(i=0; i<=s->n-1; i++)
    {
        s->b.ptr.p_double[i] = b->ptr.p_double[i];
    }
    s->islineartermchanged = ae_true;
}


/*************************************************************************
This subroutine changes linear term of the model

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
void cqmsetq(convexquadraticmodel* s,
     /* Real    */ ae_matrix* q,
     /* Real    */ ae_vector* r,
     ae_int_t k,
     double theta,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    ae_assert(k>=0, "CQMSetQ: K<0", _state);
    ae_assert((k==0||ae_fp_eq(theta,(double)(0)))||apservisfinitematrix(q, k, s->n, _state), "CQMSetQ: Q is not finite matrix", _state);
    ae_assert((k==0||ae_fp_eq(theta,(double)(0)))||isfinitevector(r, k, _state), "CQMSetQ: R is not finite vector", _state);
    ae_assert(ae_isfinite(theta, _state)&&ae_fp_greater_eq(theta,(double)(0)), "CQMSetQ: Theta<0 or is not finite number", _state);
    
    /*
     * degenerate case: K=0 or Theta=0
     */
    if( k==0||ae_fp_eq(theta,(double)(0)) )
    {
        s->k = 0;
        s->theta = (double)(0);
        s->issecondarytermchanged = ae_true;
        return;
    }
    
    /*
     * General case: both Theta>0 and K>0
     */
    s->k = k;
    s->theta = theta;
    rmatrixsetlengthatleast(&s->q, s->k, s->n, _state);
    rvectorsetlengthatleast(&s->r, s->k, _state);
    rmatrixsetlengthatleast(&s->eq, s->k, s->n, _state);
    rmatrixsetlengthatleast(&s->eccm, s->k, s->k, _state);
    rmatrixsetlengthatleast(&s->tk2, s->k, s->n, _state);
    for(i=0; i<=s->k-1; i++)
    {
        for(j=0; j<=s->n-1; j++)
        {
            s->q.ptr.pp_double[i][j] = q->ptr.pp_double[i][j];
        }
        s->r.ptr.p_double[i] = r->ptr.p_double[i];
    }
    s->issecondarytermchanged = ae_true;
}


/*************************************************************************
This subroutine changes active set

INPUT PARAMETERS
    S       -   model
    X       -   array[N], constraint values
    ActiveSet-  array[N], active set. If ActiveSet[I]=True, then I-th
                variables is constrained to X[I].

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
void cqmsetactiveset(convexquadraticmodel* s,
     /* Real    */ ae_vector* x,
     /* Boolean */ ae_vector* activeset,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert(x->cnt>=s->n, "CQMSetActiveSet: Length(X)<N", _state);
    ae_assert(activeset->cnt>=s->n, "CQMSetActiveSet: Length(ActiveSet)<N", _state);
    for(i=0; i<=s->n-1; i++)
    {
        s->isactivesetchanged = s->isactivesetchanged||(s->activeset.ptr.p_bool[i]&&!activeset->ptr.p_bool[i]);
        s->isactivesetchanged = s->isactivesetchanged||(activeset->ptr.p_bool[i]&&!s->activeset.ptr.p_bool[i]);
        s->activeset.ptr.p_bool[i] = activeset->ptr.p_bool[i];
        if( activeset->ptr.p_bool[i] )
        {
            ae_assert(ae_isfinite(x->ptr.p_double[i], _state), "CQMSetActiveSet: X[] contains infinite constraints", _state);
            s->isactivesetchanged = s->isactivesetchanged||ae_fp_neq(s->xc.ptr.p_double[i],x->ptr.p_double[i]);
            s->xc.ptr.p_double[i] = x->ptr.p_double[i];
        }
    }
}


/*************************************************************************
This subroutine evaluates model at X. Active constraints are ignored.

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
double cqmeval(convexquadraticmodel* s,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    double v;
    double result;


    n = s->n;
    ae_assert(isfinitevector(x, n, _state), "CQMEval: X is not finite vector", _state);
    result = 0.0;
    
    /*
     * main quadratic term
     */
    if( ae_fp_greater(s->alpha,(double)(0)) )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                result = result+s->alpha*0.5*x->ptr.p_double[i]*s->a.ptr.pp_double[i][j]*x->ptr.p_double[j];
            }
        }
    }
    if( ae_fp_greater(s->tau,(double)(0)) )
    {
        for(i=0; i<=n-1; i++)
        {
            result = result+0.5*ae_sqr(x->ptr.p_double[i], _state)*s->tau*s->d.ptr.p_double[i];
        }
    }
    
    /*
     * secondary quadratic term
     */
    if( ae_fp_greater(s->theta,(double)(0)) )
    {
        for(i=0; i<=s->k-1; i++)
        {
            v = ae_v_dotproduct(&s->q.ptr.pp_double[i][0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
            result = result+0.5*s->theta*ae_sqr(v-s->r.ptr.p_double[i], _state);
        }
    }
    
    /*
     * linear term
     */
    for(i=0; i<=s->n-1; i++)
    {
        result = result+x->ptr.p_double[i]*s->b.ptr.p_double[i];
    }
    return result;
}


/*************************************************************************
This subroutine evaluates model at X. Active constraints are ignored.
It returns:
    R   -   model value
    Noise-  estimate of the numerical noise in data

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
void cqmevalx(convexquadraticmodel* s,
     /* Real    */ ae_vector* x,
     double* r,
     double* noise,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    double v;
    double v2;
    double mxq;
    double eps;

    *r = 0;
    *noise = 0;

    n = s->n;
    ae_assert(isfinitevector(x, n, _state), "CQMEval: X is not finite vector", _state);
    *r = 0.0;
    *noise = 0.0;
    eps = 2*ae_machineepsilon;
    mxq = 0.0;
    
    /*
     * Main quadratic term.
     *
     * Noise from the main quadratic term is equal to the
     * maximum summand in the term.
     */
    if( ae_fp_greater(s->alpha,(double)(0)) )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                v = s->alpha*0.5*x->ptr.p_double[i]*s->a.ptr.pp_double[i][j]*x->ptr.p_double[j];
                *r = *r+v;
                *noise = ae_maxreal(*noise, eps*ae_fabs(v, _state), _state);
            }
        }
    }
    if( ae_fp_greater(s->tau,(double)(0)) )
    {
        for(i=0; i<=n-1; i++)
        {
            v = 0.5*ae_sqr(x->ptr.p_double[i], _state)*s->tau*s->d.ptr.p_double[i];
            *r = *r+v;
            *noise = ae_maxreal(*noise, eps*ae_fabs(v, _state), _state);
        }
    }
    
    /*
     * secondary quadratic term
     *
     * Noise from the secondary quadratic term is estimated as follows:
     * * noise in qi*x-r[i] is estimated as
     *   Eps*MXQ = Eps*max(|r[i]|, |q[i,j]*x[j]|)
     * * noise in (qi*x-r[i])^2 is estimated as
     *   NOISE = (|qi*x-r[i]|+Eps*MXQ)^2-(|qi*x-r[i]|)^2
     *         = Eps*MXQ*(2*|qi*x-r[i]|+Eps*MXQ)
     */
    if( ae_fp_greater(s->theta,(double)(0)) )
    {
        for(i=0; i<=s->k-1; i++)
        {
            v = 0.0;
            mxq = ae_fabs(s->r.ptr.p_double[i], _state);
            for(j=0; j<=n-1; j++)
            {
                v2 = s->q.ptr.pp_double[i][j]*x->ptr.p_double[j];
                v = v+v2;
                mxq = ae_maxreal(mxq, ae_fabs(v2, _state), _state);
            }
            *r = *r+0.5*s->theta*ae_sqr(v-s->r.ptr.p_double[i], _state);
            *noise = ae_maxreal(*noise, eps*mxq*(2*ae_fabs(v-s->r.ptr.p_double[i], _state)+eps*mxq), _state);
        }
    }
    
    /*
     * linear term
     */
    for(i=0; i<=s->n-1; i++)
    {
        *r = *r+x->ptr.p_double[i]*s->b.ptr.p_double[i];
        *noise = ae_maxreal(*noise, eps*ae_fabs(x->ptr.p_double[i]*s->b.ptr.p_double[i], _state), _state);
    }
    
    /*
     * Final update of the noise
     */
    *noise = n*(*noise);
}


/*************************************************************************
This  subroutine  evaluates  gradient of the model; active constraints are
ignored.

INPUT PARAMETERS:
    S       -   convex model
    X       -   point, array[N]
    G       -   possibly preallocated buffer; resized, if too small

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
void cqmgradunconstrained(convexquadraticmodel* s,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* g,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    double v;


    n = s->n;
    ae_assert(isfinitevector(x, n, _state), "CQMEvalGradUnconstrained: X is not finite vector", _state);
    rvectorsetlengthatleast(g, n, _state);
    for(i=0; i<=n-1; i++)
    {
        g->ptr.p_double[i] = (double)(0);
    }
    
    /*
     * main quadratic term
     */
    if( ae_fp_greater(s->alpha,(double)(0)) )
    {
        for(i=0; i<=n-1; i++)
        {
            v = 0.0;
            for(j=0; j<=n-1; j++)
            {
                v = v+s->alpha*s->a.ptr.pp_double[i][j]*x->ptr.p_double[j];
            }
            g->ptr.p_double[i] = g->ptr.p_double[i]+v;
        }
    }
    if( ae_fp_greater(s->tau,(double)(0)) )
    {
        for(i=0; i<=n-1; i++)
        {
            g->ptr.p_double[i] = g->ptr.p_double[i]+x->ptr.p_double[i]*s->tau*s->d.ptr.p_double[i];
        }
    }
    
    /*
     * secondary quadratic term
     */
    if( ae_fp_greater(s->theta,(double)(0)) )
    {
        for(i=0; i<=s->k-1; i++)
        {
            v = ae_v_dotproduct(&s->q.ptr.pp_double[i][0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
            v = s->theta*(v-s->r.ptr.p_double[i]);
            ae_v_addd(&g->ptr.p_double[0], 1, &s->q.ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
        }
    }
    
    /*
     * linear term
     */
    for(i=0; i<=n-1; i++)
    {
        g->ptr.p_double[i] = g->ptr.p_double[i]+s->b.ptr.p_double[i];
    }
}


/*************************************************************************
This subroutine evaluates x'*(0.5*alpha*A+tau*D)*x

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
double cqmxtadx2(convexquadraticmodel* s,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    double result;


    n = s->n;
    ae_assert(isfinitevector(x, n, _state), "CQMEval: X is not finite vector", _state);
    result = 0.0;
    
    /*
     * main quadratic term
     */
    if( ae_fp_greater(s->alpha,(double)(0)) )
    {
        for(i=0; i<=n-1; i++)
        {
            for(j=0; j<=n-1; j++)
            {
                result = result+s->alpha*0.5*x->ptr.p_double[i]*s->a.ptr.pp_double[i][j]*x->ptr.p_double[j];
            }
        }
    }
    if( ae_fp_greater(s->tau,(double)(0)) )
    {
        for(i=0; i<=n-1; i++)
        {
            result = result+0.5*ae_sqr(x->ptr.p_double[i], _state)*s->tau*s->d.ptr.p_double[i];
        }
    }
    return result;
}


/*************************************************************************
This subroutine evaluates (0.5*alpha*A+tau*D)*x

Y is automatically resized if needed

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
void cqmadx(convexquadraticmodel* s,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    double v;


    n = s->n;
    ae_assert(isfinitevector(x, n, _state), "CQMEval: X is not finite vector", _state);
    rvectorsetlengthatleast(y, n, _state);
    
    /*
     * main quadratic term
     */
    for(i=0; i<=n-1; i++)
    {
        y->ptr.p_double[i] = (double)(0);
    }
    if( ae_fp_greater(s->alpha,(double)(0)) )
    {
        for(i=0; i<=n-1; i++)
        {
            v = ae_v_dotproduct(&s->a.ptr.pp_double[i][0], 1, &x->ptr.p_double[0], 1, ae_v_len(0,n-1));
            y->ptr.p_double[i] = y->ptr.p_double[i]+s->alpha*v;
        }
    }
    if( ae_fp_greater(s->tau,(double)(0)) )
    {
        for(i=0; i<=n-1; i++)
        {
            y->ptr.p_double[i] = y->ptr.p_double[i]+x->ptr.p_double[i]*s->tau*s->d.ptr.p_double[i];
        }
    }
}


/*************************************************************************
This subroutine finds optimum of the model. It returns  False  on  failure
(indefinite/semidefinite matrix).  Optimum  is  found  subject  to  active
constraints.

INPUT PARAMETERS
    S       -   model
    X       -   possibly preallocated buffer; automatically resized, if
                too small enough.

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
ae_bool cqmconstrainedoptimum(convexquadraticmodel* s,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t nfree;
    ae_int_t k;
    ae_int_t i;
    double v;
    ae_int_t cidx0;
    ae_int_t itidx;
    ae_bool result;


    
    /*
     * Rebuild internal structures
     */
    if( !cqmodels_cqmrebuild(s, _state) )
    {
        result = ae_false;
        return result;
    }
    n = s->n;
    k = s->k;
    nfree = s->nfree;
    result = ae_true;
    
    /*
     * Calculate initial point for the iterative refinement:
     * * free components are set to zero
     * * constrained components are set to their constrained values
     */
    rvectorsetlengthatleast(x, n, _state);
    for(i=0; i<=n-1; i++)
    {
        if( s->activeset.ptr.p_bool[i] )
        {
            x->ptr.p_double[i] = s->xc.ptr.p_double[i];
        }
        else
        {
            x->ptr.p_double[i] = (double)(0);
        }
    }
    
    /*
     * Iterative refinement.
     *
     * In an ideal world without numerical errors it would be enough
     * to make just one Newton step from initial point:
     *   x_new = -H^(-1)*grad(x=0)
     * However, roundoff errors can significantly deteriorate quality
     * of the solution. So we have to recalculate gradient and to
     * perform Newton steps several times.
     *
     * Below we perform fixed number of Newton iterations.
     */
    for(itidx=0; itidx<=cqmodels_newtonrefinementits-1; itidx++)
    {
        
        /*
         * Calculate gradient at the current point.
         * Move free components of the gradient in the beginning.
         */
        cqmgradunconstrained(s, x, &s->tmpg, _state);
        cidx0 = 0;
        for(i=0; i<=n-1; i++)
        {
            if( !s->activeset.ptr.p_bool[i] )
            {
                s->tmpg.ptr.p_double[cidx0] = s->tmpg.ptr.p_double[i];
                cidx0 = cidx0+1;
            }
        }
        
        /*
         * Free components of the extrema are calculated in the first NFree elements of TXC.
         *
         * First, we have to calculate original Newton step, without rank-K perturbations
         */
        ae_v_moveneg(&s->txc.ptr.p_double[0], 1, &s->tmpg.ptr.p_double[0], 1, ae_v_len(0,nfree-1));
        cqmodels_cqmsolveea(s, &s->txc, &s->tmp0, _state);
        
        /*
         * Then, we account for rank-K correction.
         * Woodbury matrix identity is used.
         */
        if( s->k>0&&ae_fp_greater(s->theta,(double)(0)) )
        {
            rvectorsetlengthatleast(&s->tmp0, ae_maxint(nfree, k, _state), _state);
            rvectorsetlengthatleast(&s->tmp1, ae_maxint(nfree, k, _state), _state);
            ae_v_moveneg(&s->tmp1.ptr.p_double[0], 1, &s->tmpg.ptr.p_double[0], 1, ae_v_len(0,nfree-1));
            cqmodels_cqmsolveea(s, &s->tmp1, &s->tmp0, _state);
            for(i=0; i<=k-1; i++)
            {
                v = ae_v_dotproduct(&s->eq.ptr.pp_double[i][0], 1, &s->tmp1.ptr.p_double[0], 1, ae_v_len(0,nfree-1));
                s->tmp0.ptr.p_double[i] = v;
            }
            fblscholeskysolve(&s->eccm, 1.0, k, ae_true, &s->tmp0, &s->tmp1, _state);
            for(i=0; i<=nfree-1; i++)
            {
                s->tmp1.ptr.p_double[i] = 0.0;
            }
            for(i=0; i<=k-1; i++)
            {
                v = s->tmp0.ptr.p_double[i];
                ae_v_addd(&s->tmp1.ptr.p_double[0], 1, &s->eq.ptr.pp_double[i][0], 1, ae_v_len(0,nfree-1), v);
            }
            cqmodels_cqmsolveea(s, &s->tmp1, &s->tmp0, _state);
            ae_v_sub(&s->txc.ptr.p_double[0], 1, &s->tmp1.ptr.p_double[0], 1, ae_v_len(0,nfree-1));
        }
        
        /*
         * Unpack components from TXC into X. We pass through all
         * free components of X and add our step.
         */
        cidx0 = 0;
        for(i=0; i<=n-1; i++)
        {
            if( !s->activeset.ptr.p_bool[i] )
            {
                x->ptr.p_double[i] = x->ptr.p_double[i]+s->txc.ptr.p_double[cidx0];
                cidx0 = cidx0+1;
            }
        }
    }
    return result;
}


/*************************************************************************
This function scales vector  by  multiplying it by inverse of the diagonal
of the Hessian matrix. It should be used to  accelerate  steepest  descent
phase of the QP solver.

Although  it  is  called  "scale-grad",  it  can be called for any vector,
whether it is gradient, anti-gradient, or just some vector.

This function does NOT takes into account current set of  constraints,  it
just performs matrix-vector multiplication  without  taking  into  account
constraints.

INPUT PARAMETERS:
    S       -   model
    X       -   vector to scale

OUTPUT PARAMETERS:
    X       -   scaled vector
    
NOTE:
    when called for non-SPD matrices, it silently skips components of X
    which correspond to zero or negative diagonal elements.
    
NOTE:
    this function uses diagonals of A and D; it ignores Q - rank-K term of
    the quadratic model.

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
void cqmscalevector(convexquadraticmodel* s,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t i;
    double v;


    n = s->n;
    for(i=0; i<=n-1; i++)
    {
        v = 0.0;
        if( ae_fp_greater(s->alpha,(double)(0)) )
        {
            v = v+s->a.ptr.pp_double[i][i];
        }
        if( ae_fp_greater(s->tau,(double)(0)) )
        {
            v = v+s->d.ptr.p_double[i];
        }
        if( ae_fp_greater(v,(double)(0)) )
        {
            x->ptr.p_double[i] = x->ptr.p_double[i]/v;
        }
    }
}


/*************************************************************************
This subroutine calls CQMRebuild() and evaluates model at X subject to
active constraints.

It  is  intended  for  debug  purposes only, because it evaluates model by
means of temporaries, which were calculated  by  CQMRebuild().  The   only
purpose of this function  is  to  check  correctness  of  CQMRebuild()  by
comparing results of this function with ones obtained by CQMEval(),  which
is  used  as  reference  point. The  idea is that significant deviation in
results  of  these  two  functions  is  evidence  of  some  error  in  the
CQMRebuild().

NOTE: suffix T denotes that temporaries marked by T-prefix are used. There
      is one more variant of this function, which uses  "effective"  model
      built by CQMRebuild().

NOTE2: in case CQMRebuild() fails (due to model non-convexity), this
      function returns NAN.

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
double cqmdebugconstrainedevalt(convexquadraticmodel* s,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t nfree;
    ae_int_t i;
    ae_int_t j;
    double v;
    double result;


    n = s->n;
    ae_assert(isfinitevector(x, n, _state), "CQMDebugConstrainedEvalT: X is not finite vector", _state);
    if( !cqmodels_cqmrebuild(s, _state) )
    {
        result = _state->v_nan;
        return result;
    }
    result = 0.0;
    nfree = s->nfree;
    
    /*
     * Reorder variables
     */
    j = 0;
    for(i=0; i<=n-1; i++)
    {
        if( !s->activeset.ptr.p_bool[i] )
        {
            ae_assert(j<nfree, "CQMDebugConstrainedEvalT: internal error", _state);
            s->txc.ptr.p_double[j] = x->ptr.p_double[i];
            j = j+1;
        }
    }
    
    /*
     * TQ2, TQ1, TQ0
     *
     */
    if( ae_fp_greater(s->alpha,(double)(0)) )
    {
        
        /*
         * Dense TQ2
         */
        for(i=0; i<=nfree-1; i++)
        {
            for(j=0; j<=nfree-1; j++)
            {
                result = result+0.5*s->txc.ptr.p_double[i]*s->tq2dense.ptr.pp_double[i][j]*s->txc.ptr.p_double[j];
            }
        }
    }
    else
    {
        
        /*
         * Diagonal TQ2
         */
        for(i=0; i<=nfree-1; i++)
        {
            result = result+0.5*s->tq2diag.ptr.p_double[i]*ae_sqr(s->txc.ptr.p_double[i], _state);
        }
    }
    for(i=0; i<=nfree-1; i++)
    {
        result = result+s->tq1.ptr.p_double[i]*s->txc.ptr.p_double[i];
    }
    result = result+s->tq0;
    
    /*
     * TK2, TK1, TK0
     */
    if( s->k>0&&ae_fp_greater(s->theta,(double)(0)) )
    {
        for(i=0; i<=s->k-1; i++)
        {
            v = (double)(0);
            for(j=0; j<=nfree-1; j++)
            {
                v = v+s->tk2.ptr.pp_double[i][j]*s->txc.ptr.p_double[j];
            }
            result = result+0.5*ae_sqr(v, _state);
        }
        for(i=0; i<=nfree-1; i++)
        {
            result = result+s->tk1.ptr.p_double[i]*s->txc.ptr.p_double[i];
        }
        result = result+s->tk0;
    }
    
    /*
     * TB (Bf and Bc parts)
     */
    for(i=0; i<=n-1; i++)
    {
        result = result+s->tb.ptr.p_double[i]*s->txc.ptr.p_double[i];
    }
    return result;
}


/*************************************************************************
This subroutine calls CQMRebuild() and evaluates model at X subject to
active constraints.

It  is  intended  for  debug  purposes only, because it evaluates model by
means of "effective" matrices built by CQMRebuild(). The only  purpose  of
this function is to check correctness of CQMRebuild() by comparing results
of this function with  ones  obtained  by  CQMEval(),  which  is  used  as
reference  point.  The  idea  is  that significant deviation in results of
these two functions is evidence of some error in the CQMRebuild().

NOTE: suffix E denotes that effective matrices. There is one more  variant
      of this function, which uses temporary matrices built by
      CQMRebuild().

NOTE2: in case CQMRebuild() fails (due to model non-convexity), this
      function returns NAN.

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
double cqmdebugconstrainedevale(convexquadraticmodel* s,
     /* Real    */ ae_vector* x,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t nfree;
    ae_int_t i;
    ae_int_t j;
    double v;
    double result;


    n = s->n;
    ae_assert(isfinitevector(x, n, _state), "CQMDebugConstrainedEvalE: X is not finite vector", _state);
    if( !cqmodels_cqmrebuild(s, _state) )
    {
        result = _state->v_nan;
        return result;
    }
    result = 0.0;
    nfree = s->nfree;
    
    /*
     * Reorder variables
     */
    j = 0;
    for(i=0; i<=n-1; i++)
    {
        if( !s->activeset.ptr.p_bool[i] )
        {
            ae_assert(j<nfree, "CQMDebugConstrainedEvalE: internal error", _state);
            s->txc.ptr.p_double[j] = x->ptr.p_double[i];
            j = j+1;
        }
    }
    
    /*
     * ECA
     */
    ae_assert((s->ecakind==0||s->ecakind==1)||(s->ecakind==-1&&nfree==0), "CQMDebugConstrainedEvalE: unexpected ECAKind", _state);
    if( s->ecakind==0 )
    {
        
        /*
         * Dense ECA
         */
        for(i=0; i<=nfree-1; i++)
        {
            v = 0.0;
            for(j=i; j<=nfree-1; j++)
            {
                v = v+s->ecadense.ptr.pp_double[i][j]*s->txc.ptr.p_double[j];
            }
            result = result+0.5*ae_sqr(v, _state);
        }
    }
    if( s->ecakind==1 )
    {
        
        /*
         * Diagonal ECA
         */
        for(i=0; i<=nfree-1; i++)
        {
            result = result+0.5*ae_sqr(s->ecadiag.ptr.p_double[i]*s->txc.ptr.p_double[i], _state);
        }
    }
    
    /*
     * EQ
     */
    for(i=0; i<=s->k-1; i++)
    {
        v = 0.0;
        for(j=0; j<=nfree-1; j++)
        {
            v = v+s->eq.ptr.pp_double[i][j]*s->txc.ptr.p_double[j];
        }
        result = result+0.5*ae_sqr(v, _state);
    }
    
    /*
     * EB
     */
    for(i=0; i<=nfree-1; i++)
    {
        result = result+s->eb.ptr.p_double[i]*s->txc.ptr.p_double[i];
    }
    
    /*
     * EC
     */
    result = result+s->ec;
    return result;
}


/*************************************************************************
Internal function, rebuilds "effective" model subject to constraints.
Returns False on failure (non-SPD main quadratic term)

  -- ALGLIB --
     Copyright 10.05.2011 by Bochkanov Sergey
*************************************************************************/
static ae_bool cqmodels_cqmrebuild(convexquadraticmodel* s,
     ae_state *_state)
{
    ae_int_t n;
    ae_int_t nfree;
    ae_int_t k;
    ae_int_t i;
    ae_int_t j;
    ae_int_t ridx0;
    ae_int_t ridx1;
    ae_int_t cidx0;
    ae_int_t cidx1;
    double v;
    ae_bool result;


    if( ae_fp_eq(s->alpha,(double)(0))&&ae_fp_eq(s->tau,(double)(0)) )
    {
        
        /*
         * Non-SPD model, quick exit
         */
        result = ae_false;
        return result;
    }
    result = ae_true;
    n = s->n;
    k = s->k;
    
    /*
     * Determine number of free variables.
     * Fill TXC - array whose last N-NFree elements store constraints.
     */
    if( s->isactivesetchanged )
    {
        s->nfree = 0;
        for(i=0; i<=n-1; i++)
        {
            if( !s->activeset.ptr.p_bool[i] )
            {
                s->nfree = s->nfree+1;
            }
        }
        j = s->nfree;
        for(i=0; i<=n-1; i++)
        {
            if( s->activeset.ptr.p_bool[i] )
            {
                s->txc.ptr.p_double[j] = s->xc.ptr.p_double[i];
                j = j+1;
            }
        }
    }
    nfree = s->nfree;
    
    /*
     * Re-evaluate TQ2/TQ1/TQ0, if needed
     */
    if( s->isactivesetchanged||s->ismaintermchanged )
    {
        
        /*
         * Handle cases Alpha>0 and Alpha=0 separately:
         * * in the first case we have dense matrix
         * * in the second one we have diagonal matrix, which can be
         *   handled more efficiently
         */
        if( ae_fp_greater(s->alpha,(double)(0)) )
        {
            
            /*
             * Alpha>0, dense QP
             *
             * Split variables into two groups - free (F) and constrained (C). Reorder
             * variables in such way that free vars come first, constrained are last:
             * x = [xf, xc].
             * 
             * Main quadratic term x'*(alpha*A+tau*D)*x now splits into quadratic part,
             * linear part and constant part:
             *                   ( alpha*Aff+tau*Df  alpha*Afc        ) ( xf )              
             *   0.5*( xf' xc' )*(                                    )*(    ) =
             *                   ( alpha*Acf         alpha*Acc+tau*Dc ) ( xc )
             *
             *   = 0.5*xf'*(alpha*Aff+tau*Df)*xf + (alpha*Afc*xc)'*xf + 0.5*xc'(alpha*Acc+tau*Dc)*xc
             *                    
             * We store these parts into temporary variables:
             * * alpha*Aff+tau*Df, alpha*Afc, alpha*Acc+tau*Dc are stored into upper
             *   triangle of TQ2
             * * alpha*Afc*xc is stored into TQ1
             * * 0.5*xc'(alpha*Acc+tau*Dc)*xc is stored into TQ0
             *
             * Below comes first part of the work - generation of TQ2:
             * * we pass through rows of A and copy I-th row into upper block (Aff/Afc) or
             *   lower one (Acf/Acc) of TQ2, depending on presence of X[i] in the active set.
             *   RIdx0 variable contains current position for insertion into upper block,
             *   RIdx1 contains current position for insertion into lower one.
             * * within each row, we copy J-th element into left half (Aff/Acf) or right
             *   one (Afc/Acc), depending on presence of X[j] in the active set. CIdx0
             *   contains current position for insertion into left block, CIdx1 contains
             *   position for insertion into right one.
             * * during copying, we multiply elements by alpha and add diagonal matrix D.
             */
            ridx0 = 0;
            ridx1 = s->nfree;
            for(i=0; i<=n-1; i++)
            {
                cidx0 = 0;
                cidx1 = s->nfree;
                for(j=0; j<=n-1; j++)
                {
                    if( !s->activeset.ptr.p_bool[i]&&!s->activeset.ptr.p_bool[j] )
                    {
                        
                        /*
                         * Element belongs to Aff
                         */
                        v = s->alpha*s->a.ptr.pp_double[i][j];
                        if( i==j&&ae_fp_greater(s->tau,(double)(0)) )
                        {
                            v = v+s->tau*s->d.ptr.p_double[i];
                        }
                        s->tq2dense.ptr.pp_double[ridx0][cidx0] = v;
                    }
                    if( !s->activeset.ptr.p_bool[i]&&s->activeset.ptr.p_bool[j] )
                    {
                        
                        /*
                         * Element belongs to Afc
                         */
                        s->tq2dense.ptr.pp_double[ridx0][cidx1] = s->alpha*s->a.ptr.pp_double[i][j];
                    }
                    if( s->activeset.ptr.p_bool[i]&&!s->activeset.ptr.p_bool[j] )
                    {
                        
                        /*
                         * Element belongs to Acf
                         */
                        s->tq2dense.ptr.pp_double[ridx1][cidx0] = s->alpha*s->a.ptr.pp_double[i][j];
                    }
                    if( s->activeset.ptr.p_bool[i]&&s->activeset.ptr.p_bool[j] )
                    {
                        
                        /*
                         * Element belongs to Acc
                         */
                        v = s->alpha*s->a.ptr.pp_double[i][j];
                        if( i==j&&ae_fp_greater(s->tau,(double)(0)) )
                        {
                            v = v+s->tau*s->d.ptr.p_double[i];
                        }
                        s->tq2dense.ptr.pp_double[ridx1][cidx1] = v;
                    }
                    if( s->activeset.ptr.p_bool[j] )
                    {
                        cidx1 = cidx1+1;
                    }
                    else
                    {
                        cidx0 = cidx0+1;
                    }
                }
                if( s->activeset.ptr.p_bool[i] )
                {
                    ridx1 = ridx1+1;
                }
                else
                {
                    ridx0 = ridx0+1;
                }
            }
            
            /*
             * Now we have TQ2, and we can evaluate TQ1.
             * In the special case when we have Alpha=0, NFree=0 or NFree=N,
             * TQ1 is filled by zeros.
             */
            for(i=0; i<=n-1; i++)
            {
                s->tq1.ptr.p_double[i] = 0.0;
            }
            if( s->nfree>0&&s->nfree<n )
            {
                rmatrixmv(s->nfree, n-s->nfree, &s->tq2dense, 0, s->nfree, 0, &s->txc, s->nfree, &s->tq1, 0, _state);
            }
            
            /*
             * And finally, we evaluate TQ0.
             */
            v = 0.0;
            for(i=s->nfree; i<=n-1; i++)
            {
                for(j=s->nfree; j<=n-1; j++)
                {
                    v = v+0.5*s->txc.ptr.p_double[i]*s->tq2dense.ptr.pp_double[i][j]*s->txc.ptr.p_double[j];
                }
            }
            s->tq0 = v;
        }
        else
        {
            
            /*
             * Alpha=0, diagonal QP
             *
             * Split variables into two groups - free (F) and constrained (C). Reorder
             * variables in such way that free vars come first, constrained are last:
             * x = [xf, xc].
             * 
             * Main quadratic term x'*(tau*D)*x now splits into quadratic and constant
             * parts:
             *                   ( tau*Df        ) ( xf )              
             *   0.5*( xf' xc' )*(               )*(    ) =
             *                   (        tau*Dc ) ( xc )
             *
             *   = 0.5*xf'*(tau*Df)*xf + 0.5*xc'(tau*Dc)*xc
             *                    
             * We store these parts into temporary variables:
             * * tau*Df is stored in TQ2Diag
             * * 0.5*xc'(tau*Dc)*xc is stored into TQ0
             */
            s->tq0 = 0.0;
            ridx0 = 0;
            for(i=0; i<=n-1; i++)
            {
                if( !s->activeset.ptr.p_bool[i] )
                {
                    s->tq2diag.ptr.p_double[ridx0] = s->tau*s->d.ptr.p_double[i];
                    ridx0 = ridx0+1;
                }
                else
                {
                    s->tq0 = s->tq0+0.5*s->tau*s->d.ptr.p_double[i]*ae_sqr(s->xc.ptr.p_double[i], _state);
                }
            }
            for(i=0; i<=n-1; i++)
            {
                s->tq1.ptr.p_double[i] = 0.0;
            }
        }
    }
    
    /*
     * Re-evaluate TK2/TK1/TK0, if needed
     */
    if( s->isactivesetchanged||s->issecondarytermchanged )
    {
        
        /*
         * Split variables into two groups - free (F) and constrained (C). Reorder
         * variables in such way that free vars come first, constrained are last:
         * x = [xf, xc].
         * 
         * Secondary term theta*(Q*x-r)'*(Q*x-r) now splits into quadratic part,
         * linear part and constant part:
         *             (          ( xf )     )'  (          ( xf )     )
         *   0.5*theta*( (Qf Qc)'*(    ) - r ) * ( (Qf Qc)'*(    ) - r ) =
         *             (          ( xc )     )   (          ( xc )     )
         *
         *   = 0.5*theta*xf'*(Qf'*Qf)*xf + theta*((Qc*xc-r)'*Qf)*xf + 
         *     + theta*(-r'*(Qc*xc-r)-0.5*r'*r+0.5*xc'*Qc'*Qc*xc)
         *                    
         * We store these parts into temporary variables:
         * * sqrt(theta)*Qf is stored into TK2
         * * theta*((Qc*xc-r)'*Qf) is stored into TK1
         * * theta*(-r'*(Qc*xc-r)-0.5*r'*r+0.5*xc'*Qc'*Qc*xc) is stored into TK0
         *
         * We use several other temporaries to store intermediate results:
         * * Tmp0 - to store Qc*xc-r
         * * Tmp1 - to store Qc*xc
         *
         * Generation of TK2/TK1/TK0 is performed as follows:
         * * we fill TK2/TK1/TK0 (to handle K=0 or Theta=0)
         * * other steps are performed only for K>0 and Theta>0
         * * we pass through columns of Q and copy I-th column into left block (Qf) or
         *   right one (Qc) of TK2, depending on presence of X[i] in the active set.
         *   CIdx0 variable contains current position for insertion into upper block,
         *   CIdx1 contains current position for insertion into lower one.
         * * we calculate Qc*xc-r and store it into Tmp0
         * * we calculate TK0 and TK1
         * * we multiply leading part of TK2 which stores Qf by sqrt(theta)
         *   it is important to perform this step AFTER calculation of TK0 and TK1,
         *   because we need original (non-modified) Qf to calculate TK0 and TK1.
         */
        for(j=0; j<=n-1; j++)
        {
            for(i=0; i<=k-1; i++)
            {
                s->tk2.ptr.pp_double[i][j] = 0.0;
            }
            s->tk1.ptr.p_double[j] = 0.0;
        }
        s->tk0 = 0.0;
        if( s->k>0&&ae_fp_greater(s->theta,(double)(0)) )
        {
            
            /*
             * Split Q into Qf and Qc
             * Calculate Qc*xc-r, store in Tmp0
             */
            rvectorsetlengthatleast(&s->tmp0, k, _state);
            rvectorsetlengthatleast(&s->tmp1, k, _state);
            cidx0 = 0;
            cidx1 = nfree;
            for(i=0; i<=k-1; i++)
            {
                s->tmp1.ptr.p_double[i] = 0.0;
            }
            for(j=0; j<=n-1; j++)
            {
                if( s->activeset.ptr.p_bool[j] )
                {
                    for(i=0; i<=k-1; i++)
                    {
                        s->tk2.ptr.pp_double[i][cidx1] = s->q.ptr.pp_double[i][j];
                        s->tmp1.ptr.p_double[i] = s->tmp1.ptr.p_double[i]+s->q.ptr.pp_double[i][j]*s->txc.ptr.p_double[cidx1];
                    }
                    cidx1 = cidx1+1;
                }
                else
                {
                    for(i=0; i<=k-1; i++)
                    {
                        s->tk2.ptr.pp_double[i][cidx0] = s->q.ptr.pp_double[i][j];
                    }
                    cidx0 = cidx0+1;
                }
            }
            for(i=0; i<=k-1; i++)
            {
                s->tmp0.ptr.p_double[i] = s->tmp1.ptr.p_double[i]-s->r.ptr.p_double[i];
            }
            
            /*
             * Calculate TK0
             */
            v = 0.0;
            for(i=0; i<=k-1; i++)
            {
                v = v+s->theta*(0.5*ae_sqr(s->tmp1.ptr.p_double[i], _state)-s->r.ptr.p_double[i]*s->tmp0.ptr.p_double[i]-0.5*ae_sqr(s->r.ptr.p_double[i], _state));
            }
            s->tk0 = v;
            
            /*
             * Calculate TK1
             */
            if( nfree>0 )
            {
                for(i=0; i<=k-1; i++)
                {
                    v = s->theta*s->tmp0.ptr.p_double[i];
                    ae_v_addd(&s->tk1.ptr.p_double[0], 1, &s->tk2.ptr.pp_double[i][0], 1, ae_v_len(0,nfree-1), v);
                }
            }
            
            /*
             * Calculate TK2
             */
            if( nfree>0 )
            {
                v = ae_sqrt(s->theta, _state);
                for(i=0; i<=k-1; i++)
                {
                    ae_v_muld(&s->tk2.ptr.pp_double[i][0], 1, ae_v_len(0,nfree-1), v);
                }
            }
        }
    }
    
    /*
     * Re-evaluate TB
     */
    if( s->isactivesetchanged||s->islineartermchanged )
    {
        ridx0 = 0;
        ridx1 = nfree;
        for(i=0; i<=n-1; i++)
        {
            if( s->activeset.ptr.p_bool[i] )
            {
                s->tb.ptr.p_double[ridx1] = s->b.ptr.p_double[i];
                ridx1 = ridx1+1;
            }
            else
            {
                s->tb.ptr.p_double[ridx0] = s->b.ptr.p_double[i];
                ridx0 = ridx0+1;
            }
        }
    }
    
    /*
     * Compose ECA: either dense ECA or diagonal ECA
     */
    if( (s->isactivesetchanged||s->ismaintermchanged)&&nfree>0 )
    {
        if( ae_fp_greater(s->alpha,(double)(0)) )
        {
            
            /*
             * Dense ECA
             */
            s->ecakind = 0;
            for(i=0; i<=nfree-1; i++)
            {
                for(j=i; j<=nfree-1; j++)
                {
                    s->ecadense.ptr.pp_double[i][j] = s->tq2dense.ptr.pp_double[i][j];
                }
            }
            if( !spdmatrixcholeskyrec(&s->ecadense, 0, nfree, ae_true, &s->tmp0, _state) )
            {
                result = ae_false;
                return result;
            }
        }
        else
        {
            
            /*
             * Diagonal ECA
             */
            s->ecakind = 1;
            for(i=0; i<=nfree-1; i++)
            {
                if( ae_fp_less(s->tq2diag.ptr.p_double[i],(double)(0)) )
                {
                    result = ae_false;
                    return result;
                }
                s->ecadiag.ptr.p_double[i] = ae_sqrt(s->tq2diag.ptr.p_double[i], _state);
            }
        }
    }
    
    /*
     * Compose EQ
     */
    if( s->isactivesetchanged||s->issecondarytermchanged )
    {
        for(i=0; i<=k-1; i++)
        {
            for(j=0; j<=nfree-1; j++)
            {
                s->eq.ptr.pp_double[i][j] = s->tk2.ptr.pp_double[i][j];
            }
        }
    }
    
    /*
     * Calculate ECCM
     */
    if( ((((s->isactivesetchanged||s->ismaintermchanged)||s->issecondarytermchanged)&&s->k>0)&&ae_fp_greater(s->theta,(double)(0)))&&nfree>0 )
    {
        
        /*
         * Calculate ECCM - Cholesky factor of the "effective" capacitance
         * matrix CM = I + EQ*inv(EffectiveA)*EQ'.
         *
         * We calculate CM as follows:
         *   CM = I + EQ*inv(EffectiveA)*EQ'
         *      = I + EQ*ECA^(-1)*ECA^(-T)*EQ'
         *      = I + (EQ*ECA^(-1))*(EQ*ECA^(-1))'
         *
         * Then we perform Cholesky decomposition of CM.
         */
        rmatrixsetlengthatleast(&s->tmp2, k, n, _state);
        rmatrixcopy(k, nfree, &s->eq, 0, 0, &s->tmp2, 0, 0, _state);
        ae_assert(s->ecakind==0||s->ecakind==1, "CQMRebuild: unexpected ECAKind", _state);
        if( s->ecakind==0 )
        {
            rmatrixrighttrsm(k, nfree, &s->ecadense, 0, 0, ae_true, ae_false, 0, &s->tmp2, 0, 0, _state);
        }
        if( s->ecakind==1 )
        {
            for(i=0; i<=k-1; i++)
            {
                for(j=0; j<=nfree-1; j++)
                {
                    s->tmp2.ptr.pp_double[i][j] = s->tmp2.ptr.pp_double[i][j]/s->ecadiag.ptr.p_double[j];
                }
            }
        }
        for(i=0; i<=k-1; i++)
        {
            for(j=0; j<=k-1; j++)
            {
                s->eccm.ptr.pp_double[i][j] = 0.0;
            }
            s->eccm.ptr.pp_double[i][i] = 1.0;
        }
        rmatrixsyrk(k, nfree, 1.0, &s->tmp2, 0, 0, 0, 1.0, &s->eccm, 0, 0, ae_true, _state);
        if( !spdmatrixcholeskyrec(&s->eccm, 0, k, ae_true, &s->tmp0, _state) )
        {
            result = ae_false;
            return result;
        }
    }
    
    /*
     * Compose EB and EC
     *
     * NOTE: because these quantities are cheap to compute, we do not
     * use caching here.
     */
    for(i=0; i<=nfree-1; i++)
    {
        s->eb.ptr.p_double[i] = s->tq1.ptr.p_double[i]+s->tk1.ptr.p_double[i]+s->tb.ptr.p_double[i];
    }
    s->ec = s->tq0+s->tk0;
    for(i=nfree; i<=n-1; i++)
    {
        s->ec = s->ec+s->tb.ptr.p_double[i]*s->txc.ptr.p_double[i];
    }
    
    /*
     * Change cache status - everything is cached 
     */
    s->ismaintermchanged = ae_false;
    s->issecondarytermchanged = ae_false;
    s->islineartermchanged = ae_false;
    s->isactivesetchanged = ae_false;
    return result;
}


/*************************************************************************
Internal function, solves system Effective_A*x = b.
It should be called after successful completion of CQMRebuild().

INPUT PARAMETERS:
    S       -   quadratic model, after call to CQMRebuild()
    X       -   right part B, array[S.NFree]
    Tmp     -   temporary array, automatically reallocated if needed

OUTPUT PARAMETERS:
    X       -   solution, array[S.NFree]
    
NOTE: when called with zero S.NFree, returns silently
NOTE: this function assumes that EA is non-degenerate

  -- ALGLIB --
     Copyright 10.05.2011 by Bochkanov Sergey
*************************************************************************/
static void cqmodels_cqmsolveea(convexquadraticmodel* s,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* tmp,
     ae_state *_state)
{
    ae_int_t i;


    ae_assert((s->ecakind==0||s->ecakind==1)||(s->ecakind==-1&&s->nfree==0), "CQMSolveEA: unexpected ECAKind", _state);
    if( s->ecakind==0 )
    {
        
        /*
         * Dense ECA, use FBLSCholeskySolve() dense solver.
         */
        fblscholeskysolve(&s->ecadense, 1.0, s->nfree, ae_true, x, tmp, _state);
    }
    if( s->ecakind==1 )
    {
        
        /*
         * Diagonal ECA
         */
        for(i=0; i<=s->nfree-1; i++)
        {
            x->ptr.p_double[i] = x->ptr.p_double[i]/ae_sqr(s->ecadiag.ptr.p_double[i], _state);
        }
    }
}


void _convexquadraticmodel_init(void* _p, ae_state *_state)
{
    convexquadraticmodel *p = (convexquadraticmodel*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_init(&p->a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->q, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->b, 0, DT_REAL, _state);
    ae_vector_init(&p->r, 0, DT_REAL, _state);
    ae_vector_init(&p->xc, 0, DT_REAL, _state);
    ae_vector_init(&p->d, 0, DT_REAL, _state);
    ae_vector_init(&p->activeset, 0, DT_BOOL, _state);
    ae_matrix_init(&p->tq2dense, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->tk2, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->tq2diag, 0, DT_REAL, _state);
    ae_vector_init(&p->tq1, 0, DT_REAL, _state);
    ae_vector_init(&p->tk1, 0, DT_REAL, _state);
    ae_vector_init(&p->txc, 0, DT_REAL, _state);
    ae_vector_init(&p->tb, 0, DT_REAL, _state);
    ae_matrix_init(&p->ecadense, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->eq, 0, 0, DT_REAL, _state);
    ae_matrix_init(&p->eccm, 0, 0, DT_REAL, _state);
    ae_vector_init(&p->ecadiag, 0, DT_REAL, _state);
    ae_vector_init(&p->eb, 0, DT_REAL, _state);
    ae_vector_init(&p->tmp0, 0, DT_REAL, _state);
    ae_vector_init(&p->tmp1, 0, DT_REAL, _state);
    ae_vector_init(&p->tmpg, 0, DT_REAL, _state);
    ae_matrix_init(&p->tmp2, 0, 0, DT_REAL, _state);
}


void _convexquadraticmodel_init_copy(void* _dst, void* _src, ae_state *_state)
{
    convexquadraticmodel *dst = (convexquadraticmodel*)_dst;
    convexquadraticmodel *src = (convexquadraticmodel*)_src;
    dst->n = src->n;
    dst->k = src->k;
    dst->alpha = src->alpha;
    dst->tau = src->tau;
    dst->theta = src->theta;
    ae_matrix_init_copy(&dst->a, &src->a, _state);
    ae_matrix_init_copy(&dst->q, &src->q, _state);
    ae_vector_init_copy(&dst->b, &src->b, _state);
    ae_vector_init_copy(&dst->r, &src->r, _state);
    ae_vector_init_copy(&dst->xc, &src->xc, _state);
    ae_vector_init_copy(&dst->d, &src->d, _state);
    ae_vector_init_copy(&dst->activeset, &src->activeset, _state);
    ae_matrix_init_copy(&dst->tq2dense, &src->tq2dense, _state);
    ae_matrix_init_copy(&dst->tk2, &src->tk2, _state);
    ae_vector_init_copy(&dst->tq2diag, &src->tq2diag, _state);
    ae_vector_init_copy(&dst->tq1, &src->tq1, _state);
    ae_vector_init_copy(&dst->tk1, &src->tk1, _state);
    dst->tq0 = src->tq0;
    dst->tk0 = src->tk0;
    ae_vector_init_copy(&dst->txc, &src->txc, _state);
    ae_vector_init_copy(&dst->tb, &src->tb, _state);
    dst->nfree = src->nfree;
    dst->ecakind = src->ecakind;
    ae_matrix_init_copy(&dst->ecadense, &src->ecadense, _state);
    ae_matrix_init_copy(&dst->eq, &src->eq, _state);
    ae_matrix_init_copy(&dst->eccm, &src->eccm, _state);
    ae_vector_init_copy(&dst->ecadiag, &src->ecadiag, _state);
    ae_vector_init_copy(&dst->eb, &src->eb, _state);
    dst->ec = src->ec;
    ae_vector_init_copy(&dst->tmp0, &src->tmp0, _state);
    ae_vector_init_copy(&dst->tmp1, &src->tmp1, _state);
    ae_vector_init_copy(&dst->tmpg, &src->tmpg, _state);
    ae_matrix_init_copy(&dst->tmp2, &src->tmp2, _state);
    dst->ismaintermchanged = src->ismaintermchanged;
    dst->issecondarytermchanged = src->issecondarytermchanged;
    dst->islineartermchanged = src->islineartermchanged;
    dst->isactivesetchanged = src->isactivesetchanged;
}


void _convexquadraticmodel_clear(void* _p)
{
    convexquadraticmodel *p = (convexquadraticmodel*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_clear(&p->a);
    ae_matrix_clear(&p->q);
    ae_vector_clear(&p->b);
    ae_vector_clear(&p->r);
    ae_vector_clear(&p->xc);
    ae_vector_clear(&p->d);
    ae_vector_clear(&p->activeset);
    ae_matrix_clear(&p->tq2dense);
    ae_matrix_clear(&p->tk2);
    ae_vector_clear(&p->tq2diag);
    ae_vector_clear(&p->tq1);
    ae_vector_clear(&p->tk1);
    ae_vector_clear(&p->txc);
    ae_vector_clear(&p->tb);
    ae_matrix_clear(&p->ecadense);
    ae_matrix_clear(&p->eq);
    ae_matrix_clear(&p->eccm);
    ae_vector_clear(&p->ecadiag);
    ae_vector_clear(&p->eb);
    ae_vector_clear(&p->tmp0);
    ae_vector_clear(&p->tmp1);
    ae_vector_clear(&p->tmpg);
    ae_matrix_clear(&p->tmp2);
}


void _convexquadraticmodel_destroy(void* _p)
{
    convexquadraticmodel *p = (convexquadraticmodel*)_p;
    ae_touch_ptr((void*)p);
    ae_matrix_destroy(&p->a);
    ae_matrix_destroy(&p->q);
    ae_vector_destroy(&p->b);
    ae_vector_destroy(&p->r);
    ae_vector_destroy(&p->xc);
    ae_vector_destroy(&p->d);
    ae_vector_destroy(&p->activeset);
    ae_matrix_destroy(&p->tq2dense);
    ae_matrix_destroy(&p->tk2);
    ae_vector_destroy(&p->tq2diag);
    ae_vector_destroy(&p->tq1);
    ae_vector_destroy(&p->tk1);
    ae_vector_destroy(&p->txc);
    ae_vector_destroy(&p->tb);
    ae_matrix_destroy(&p->ecadense);
    ae_matrix_destroy(&p->eq);
    ae_matrix_destroy(&p->eccm);
    ae_vector_destroy(&p->ecadiag);
    ae_vector_destroy(&p->eb);
    ae_vector_destroy(&p->tmp0);
    ae_vector_destroy(&p->tmp1);
    ae_vector_destroy(&p->tmpg);
    ae_matrix_destroy(&p->tmp2);
}


/*$ End $*/
