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

#ifndef _cqmodels_h
#define _cqmodels_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "reflections.h"
#include "creflections.h"
#include "hqrnd.h"
#include "matgen.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"
#include "tsort.h"
#include "sparse.h"
#include "rotations.h"
#include "trfac.h"
#include "hblas.h"
#include "sblas.h"
#include "ortfac.h"
#include "fbls.h"


/*$ Declarations $*/


/*************************************************************************
This structure describes convex quadratic model of the form:
    f(x) = 0.5*(Alpha*x'*A*x + Tau*x'*D*x) + 0.5*Theta*(Q*x-r)'*(Q*x-r) + b'*x
where:
    * Alpha>=0, Tau>=0, Theta>=0, Alpha+Tau>0.
    * A is NxN matrix, Q is NxK matrix (N>=1, K>=0), b is Nx1 vector,
      D is NxN diagonal matrix.
    * "main" quadratic term Alpha*A+Lambda*D is symmetric
      positive definite
Structure may contain optional equality constraints of the form x[i]=x0[i],
in this case functions provided by this unit calculate Newton step subject
to these equality constraints.
*************************************************************************/
typedef struct
{
    ae_int_t n;
    ae_int_t k;
    double alpha;
    double tau;
    double theta;
    ae_matrix a;
    ae_matrix q;
    ae_vector b;
    ae_vector r;
    ae_vector xc;
    ae_vector d;
    ae_vector activeset;
    ae_matrix tq2dense;
    ae_matrix tk2;
    ae_vector tq2diag;
    ae_vector tq1;
    ae_vector tk1;
    double tq0;
    double tk0;
    ae_vector txc;
    ae_vector tb;
    ae_int_t nfree;
    ae_int_t ecakind;
    ae_matrix ecadense;
    ae_matrix eq;
    ae_matrix eccm;
    ae_vector ecadiag;
    ae_vector eb;
    double ec;
    ae_vector tmp0;
    ae_vector tmp1;
    ae_vector tmpg;
    ae_matrix tmp2;
    ae_bool ismaintermchanged;
    ae_bool issecondarytermchanged;
    ae_bool islineartermchanged;
    ae_bool isactivesetchanged;
} convexquadraticmodel;


/*$ Body $*/


/*************************************************************************
This subroutine is used to initialize CQM. By default, empty NxN model  is
generated, with Alpha=Lambda=Theta=0.0 and zero b.

Previously allocated buffer variables are reused as much as possible.

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
void cqminit(ae_int_t n, convexquadraticmodel* s, ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
void cqmdropa(convexquadraticmodel* s, ae_state *_state);


/*************************************************************************
This subroutine changes linear term of the model

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
void cqmsetb(convexquadraticmodel* s,
     /* Real    */ ae_vector* b,
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


/*************************************************************************
This subroutine evaluates model at X. Active constraints are ignored.

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
double cqmeval(convexquadraticmodel* s,
     /* Real    */ ae_vector* x,
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


/*************************************************************************
This subroutine evaluates x'*(0.5*alpha*A+tau*D)*x

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
double cqmxtadx2(convexquadraticmodel* s,
     /* Real    */ ae_vector* x,
     ae_state *_state);


/*************************************************************************
This subroutine evaluates (0.5*alpha*A+tau*D)*x

Y is automatically resized if needed

  -- ALGLIB --
     Copyright 12.06.2012 by Bochkanov Sergey
*************************************************************************/
void cqmadx(convexquadraticmodel* s,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);


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
     ae_state *_state);
void _convexquadraticmodel_init(void* _p, ae_state *_state);
void _convexquadraticmodel_init_copy(void* _dst, void* _src, ae_state *_state);
void _convexquadraticmodel_clear(void* _p);
void _convexquadraticmodel_destroy(void* _p);


/*$ End $*/
#endif

