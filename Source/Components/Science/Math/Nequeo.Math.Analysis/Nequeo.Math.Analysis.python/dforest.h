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

#ifndef _dforest_h
#define _dforest_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "scodes.h"
#include "tsort.h"
#include "hqrnd.h"
#include "basicstatops.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"
#include "basestat.h"
#include "bdss.h"


/*$ Declarations $*/


typedef struct
{
    ae_int_t nvars;
    ae_int_t nclasses;
    ae_int_t ntrees;
    ae_int_t bufsize;
    ae_vector trees;
} decisionforest;


typedef struct
{
    double relclserror;
    double avgce;
    double rmserror;
    double avgerror;
    double avgrelerror;
    double oobrelclserror;
    double oobavgce;
    double oobrmserror;
    double oobavgerror;
    double oobavgrelerror;
} dfreport;


typedef struct
{
    ae_vector treebuf;
    ae_vector idxbuf;
    ae_vector tmpbufr;
    ae_vector tmpbufr2;
    ae_vector tmpbufi;
    ae_vector classibuf;
    ae_vector sortrbuf;
    ae_vector sortrbuf2;
    ae_vector sortibuf;
    ae_vector varpool;
    ae_vector evsbin;
    ae_vector evssplits;
} dfinternalbuffers;


/*$ Body $*/


/*************************************************************************
This subroutine builds random decision forest.

INPUT PARAMETERS:
    XY          -   training set
    NPoints     -   training set size, NPoints>=1
    NVars       -   number of independent variables, NVars>=1
    NClasses    -   task type:
                    * NClasses=1 - regression task with one
                                   dependent variable
                    * NClasses>1 - classification task with
                                   NClasses classes.
    NTrees      -   number of trees in a forest, NTrees>=1.
                    recommended values: 50-100.
    R           -   percent of a training set used to build
                    individual trees. 0<R<=1.
                    recommended values: 0.1 <= R <= 0.66.

OUTPUT PARAMETERS:
    Info        -   return code:
                    * -2, if there is a point with class number
                          outside of [0..NClasses-1].
                    * -1, if incorrect parameters was passed
                          (NPoints<1, NVars<1, NClasses<1, NTrees<1, R<=0
                          or R>1).
                    *  1, if task has been solved
    DF          -   model built
    Rep         -   training report, contains error on a training set
                    and out-of-bag estimates of generalization error.

  -- ALGLIB --
     Copyright 19.02.2009 by Bochkanov Sergey
*************************************************************************/
void dfbuildrandomdecisionforest(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t nclasses,
     ae_int_t ntrees,
     double r,
     ae_int_t* info,
     decisionforest* df,
     dfreport* rep,
     ae_state *_state);


/*************************************************************************
This subroutine builds random decision forest.
This function gives ability to tune number of variables used when choosing
best split.

INPUT PARAMETERS:
    XY          -   training set
    NPoints     -   training set size, NPoints>=1
    NVars       -   number of independent variables, NVars>=1
    NClasses    -   task type:
                    * NClasses=1 - regression task with one
                                   dependent variable
                    * NClasses>1 - classification task with
                                   NClasses classes.
    NTrees      -   number of trees in a forest, NTrees>=1.
                    recommended values: 50-100.
    NRndVars    -   number of variables used when choosing best split
    R           -   percent of a training set used to build
                    individual trees. 0<R<=1.
                    recommended values: 0.1 <= R <= 0.66.

OUTPUT PARAMETERS:
    Info        -   return code:
                    * -2, if there is a point with class number
                          outside of [0..NClasses-1].
                    * -1, if incorrect parameters was passed
                          (NPoints<1, NVars<1, NClasses<1, NTrees<1, R<=0
                          or R>1).
                    *  1, if task has been solved
    DF          -   model built
    Rep         -   training report, contains error on a training set
                    and out-of-bag estimates of generalization error.

  -- ALGLIB --
     Copyright 19.02.2009 by Bochkanov Sergey
*************************************************************************/
void dfbuildrandomdecisionforestx1(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t nclasses,
     ae_int_t ntrees,
     ae_int_t nrndvars,
     double r,
     ae_int_t* info,
     decisionforest* df,
     dfreport* rep,
     ae_state *_state);


void dfbuildinternal(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     ae_int_t nclasses,
     ae_int_t ntrees,
     ae_int_t samplesize,
     ae_int_t nfeatures,
     ae_int_t flags,
     ae_int_t* info,
     decisionforest* df,
     dfreport* rep,
     ae_state *_state);


/*************************************************************************
Procesing

INPUT PARAMETERS:
    DF      -   decision forest model
    X       -   input vector,  array[0..NVars-1].

OUTPUT PARAMETERS:
    Y       -   result. Regression estimate when solving regression  task,
                vector of posterior probabilities for classification task.

See also DFProcessI.

  -- ALGLIB --
     Copyright 16.02.2009 by Bochkanov Sergey
*************************************************************************/
void dfprocess(decisionforest* df,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state);


/*************************************************************************
'interactive' variant of DFProcess for languages like Python which support
constructs like "Y = DFProcessI(DF,X)" and interactive mode of interpreter

This function allocates new array on each call,  so  it  is  significantly
slower than its 'non-interactive' counterpart, but it is  more  convenient
when you call it from command line.

  -- ALGLIB --
     Copyright 28.02.2010 by Bochkanov Sergey
*************************************************************************/
void dfprocessi(decisionforest* df,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_state *_state);


/*************************************************************************
Relative classification error on the test set

INPUT PARAMETERS:
    DF      -   decision forest model
    XY      -   test set
    NPoints -   test set size

RESULT:
    percent of incorrectly classified cases.
    Zero if model solves regression task.

  -- ALGLIB --
     Copyright 16.02.2009 by Bochkanov Sergey
*************************************************************************/
double dfrelclserror(decisionforest* df,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state);


/*************************************************************************
Average cross-entropy (in bits per element) on the test set

INPUT PARAMETERS:
    DF      -   decision forest model
    XY      -   test set
    NPoints -   test set size

RESULT:
    CrossEntropy/(NPoints*LN(2)).
    Zero if model solves regression task.

  -- ALGLIB --
     Copyright 16.02.2009 by Bochkanov Sergey
*************************************************************************/
double dfavgce(decisionforest* df,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state);


/*************************************************************************
RMS error on the test set

INPUT PARAMETERS:
    DF      -   decision forest model
    XY      -   test set
    NPoints -   test set size

RESULT:
    root mean square error.
    Its meaning for regression task is obvious. As for
    classification task, RMS error means error when estimating posterior
    probabilities.

  -- ALGLIB --
     Copyright 16.02.2009 by Bochkanov Sergey
*************************************************************************/
double dfrmserror(decisionforest* df,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state);


/*************************************************************************
Average error on the test set

INPUT PARAMETERS:
    DF      -   decision forest model
    XY      -   test set
    NPoints -   test set size

RESULT:
    Its meaning for regression task is obvious. As for
    classification task, it means average error when estimating posterior
    probabilities.

  -- ALGLIB --
     Copyright 16.02.2009 by Bochkanov Sergey
*************************************************************************/
double dfavgerror(decisionforest* df,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state);


/*************************************************************************
Average relative error on the test set

INPUT PARAMETERS:
    DF      -   decision forest model
    XY      -   test set
    NPoints -   test set size

RESULT:
    Its meaning for regression task is obvious. As for
    classification task, it means average relative error when estimating
    posterior probability of belonging to the correct class.

  -- ALGLIB --
     Copyright 16.02.2009 by Bochkanov Sergey
*************************************************************************/
double dfavgrelerror(decisionforest* df,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_state *_state);


/*************************************************************************
Copying of DecisionForest strucure

INPUT PARAMETERS:
    DF1 -   original

OUTPUT PARAMETERS:
    DF2 -   copy

  -- ALGLIB --
     Copyright 13.02.2009 by Bochkanov Sergey
*************************************************************************/
void dfcopy(decisionforest* df1, decisionforest* df2, ae_state *_state);


/*************************************************************************
Serializer: allocation

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void dfalloc(ae_serializer* s, decisionforest* forest, ae_state *_state);


/*************************************************************************
Serializer: serialization

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void dfserialize(ae_serializer* s,
     decisionforest* forest,
     ae_state *_state);


/*************************************************************************
Serializer: unserialization

  -- ALGLIB --
     Copyright 14.03.2011 by Bochkanov Sergey
*************************************************************************/
void dfunserialize(ae_serializer* s,
     decisionforest* forest,
     ae_state *_state);
void _decisionforest_init(void* _p, ae_state *_state);
void _decisionforest_init_copy(void* _dst, void* _src, ae_state *_state);
void _decisionforest_clear(void* _p);
void _decisionforest_destroy(void* _p);
void _dfreport_init(void* _p, ae_state *_state);
void _dfreport_init_copy(void* _dst, void* _src, ae_state *_state);
void _dfreport_clear(void* _p);
void _dfreport_destroy(void* _p);
void _dfinternalbuffers_init(void* _p, ae_state *_state);
void _dfinternalbuffers_init_copy(void* _dst, void* _src, ae_state *_state);
void _dfinternalbuffers_clear(void* _p);
void _dfinternalbuffers_destroy(void* _p);


/*$ End $*/
#endif

