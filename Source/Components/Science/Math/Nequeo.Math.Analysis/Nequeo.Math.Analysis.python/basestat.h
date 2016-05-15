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

#ifndef _basestat_h
#define _basestat_h

#include "aenv.h"
#include "ialglib.h"
#include "apserv.h"
#include "tsort.h"
#include "basicstatops.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Calculation of the distribution moments: mean, variance, skewness, kurtosis.

INPUT PARAMETERS:
    X       -   sample
    N       -   N>=0, sample size:
                * if given, only leading N elements of X are processed
                * if not given, automatically determined from size of X
    
OUTPUT PARAMETERS
    Mean    -   mean.
    Variance-   variance.
    Skewness-   skewness (if variance<>0; zero otherwise).
    Kurtosis-   kurtosis (if variance<>0; zero otherwise).

NOTE: variance is calculated by dividing sum of squares by N-1, not N.

  -- ALGLIB --
     Copyright 06.09.2006 by Bochkanov Sergey
*************************************************************************/
void samplemoments(/* Real    */ ae_vector* x,
     ae_int_t n,
     double* mean,
     double* variance,
     double* skewness,
     double* kurtosis,
     ae_state *_state);


/*************************************************************************
Calculation of the mean.

INPUT PARAMETERS:
    X       -   sample
    N       -   N>=0, sample size:
                * if given, only leading N elements of X are processed
                * if not given, automatically determined from size of X

NOTE:
                
This function return result  which calculated by 'SampleMoments' function
and stored at 'Mean' variable.


  -- ALGLIB --
     Copyright 06.09.2006 by Bochkanov Sergey
*************************************************************************/
double samplemean(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
Calculation of the variance.

INPUT PARAMETERS:
    X       -   sample
    N       -   N>=0, sample size:
                * if given, only leading N elements of X are processed
                * if not given, automatically determined from size of X

NOTE:
                
This function return result  which calculated by 'SampleMoments' function
and stored at 'Variance' variable.


  -- ALGLIB --
     Copyright 06.09.2006 by Bochkanov Sergey
*************************************************************************/
double samplevariance(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
Calculation of the skewness.

INPUT PARAMETERS:
    X       -   sample
    N       -   N>=0, sample size:
                * if given, only leading N elements of X are processed
                * if not given, automatically determined from size of X

NOTE:
                
This function return result  which calculated by 'SampleMoments' function
and stored at 'Skewness' variable.


  -- ALGLIB --
     Copyright 06.09.2006 by Bochkanov Sergey
*************************************************************************/
double sampleskewness(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
Calculation of the kurtosis.

INPUT PARAMETERS:
    X       -   sample
    N       -   N>=0, sample size:
                * if given, only leading N elements of X are processed
                * if not given, automatically determined from size of X

NOTE:
                
This function return result  which calculated by 'SampleMoments' function
and stored at 'Kurtosis' variable.


  -- ALGLIB --
     Copyright 06.09.2006 by Bochkanov Sergey
*************************************************************************/
double samplekurtosis(/* Real    */ ae_vector* x,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
ADev

Input parameters:
    X   -   sample
    N   -   N>=0, sample size:
            * if given, only leading N elements of X are processed
            * if not given, automatically determined from size of X
    
Output parameters:
    ADev-   ADev

  -- ALGLIB --
     Copyright 06.09.2006 by Bochkanov Sergey
*************************************************************************/
void sampleadev(/* Real    */ ae_vector* x,
     ae_int_t n,
     double* adev,
     ae_state *_state);


/*************************************************************************
Median calculation.

Input parameters:
    X   -   sample (array indexes: [0..N-1])
    N   -   N>=0, sample size:
            * if given, only leading N elements of X are processed
            * if not given, automatically determined from size of X

Output parameters:
    Median

  -- ALGLIB --
     Copyright 06.09.2006 by Bochkanov Sergey
*************************************************************************/
void samplemedian(/* Real    */ ae_vector* x,
     ae_int_t n,
     double* median,
     ae_state *_state);


/*************************************************************************
Percentile calculation.

Input parameters:
    X   -   sample (array indexes: [0..N-1])
    N   -   N>=0, sample size:
            * if given, only leading N elements of X are processed
            * if not given, automatically determined from size of X
    P   -   percentile (0<=P<=1)

Output parameters:
    V   -   percentile

  -- ALGLIB --
     Copyright 01.03.2008 by Bochkanov Sergey
*************************************************************************/
void samplepercentile(/* Real    */ ae_vector* x,
     ae_int_t n,
     double p,
     double* v,
     ae_state *_state);


/*************************************************************************
2-sample covariance

Input parameters:
    X       -   sample 1 (array indexes: [0..N-1])
    Y       -   sample 2 (array indexes: [0..N-1])
    N       -   N>=0, sample size:
                * if given, only N leading elements of X/Y are processed
                * if not given, automatically determined from input sizes

Result:
    covariance (zero for N=0 or N=1)

  -- ALGLIB --
     Copyright 28.10.2010 by Bochkanov Sergey
*************************************************************************/
double cov2(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
Pearson product-moment correlation coefficient

Input parameters:
    X       -   sample 1 (array indexes: [0..N-1])
    Y       -   sample 2 (array indexes: [0..N-1])
    N       -   N>=0, sample size:
                * if given, only N leading elements of X/Y are processed
                * if not given, automatically determined from input sizes

Result:
    Pearson product-moment correlation coefficient
    (zero for N=0 or N=1)

  -- ALGLIB --
     Copyright 28.10.2010 by Bochkanov Sergey
*************************************************************************/
double pearsoncorr2(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
Spearman's rank correlation coefficient

Input parameters:
    X       -   sample 1 (array indexes: [0..N-1])
    Y       -   sample 2 (array indexes: [0..N-1])
    N       -   N>=0, sample size:
                * if given, only N leading elements of X/Y are processed
                * if not given, automatically determined from input sizes

Result:
    Spearman's rank correlation coefficient
    (zero for N=0 or N=1)

  -- ALGLIB --
     Copyright 09.04.2007 by Bochkanov Sergey
*************************************************************************/
double spearmancorr2(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
Covariance matrix

SMP EDITION OF ALGLIB:

  ! This function can utilize multicore capabilities of  your system.  In
  ! order to do this you have to call version with "smp_" prefix,   which
  ! indicates that multicore code will be used.
  ! 
  ! This note is given for users of SMP edition; if you use GPL  edition,
  ! or commercial edition of ALGLIB without SMP support, you  still  will
  ! be able to call smp-version of this function,  but  all  computations
  ! will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.
  !
  ! You should remember that starting/stopping worker thread always  have
  ! non-zero cost. Although  multicore  version  is  pretty  efficient on
  ! large problems, we do not recommend you to use it on small problems -
  ! with covariance matrices smaller than 128*128.

INPUT PARAMETERS:
    X   -   array[N,M], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    N   -   N>=0, number of observations:
            * if given, only leading N rows of X are used
            * if not given, automatically determined from input size
    M   -   M>0, number of variables:
            * if given, only leading M columns of X are used
            * if not given, automatically determined from input size

OUTPUT PARAMETERS:
    C   -   array[M,M], covariance matrix (zero if N=0 or N=1)

  -- ALGLIB --
     Copyright 28.10.2010 by Bochkanov Sergey
*************************************************************************/
void covm(/* Real    */ ae_matrix* x,
     ae_int_t n,
     ae_int_t m,
     /* Real    */ ae_matrix* c,
     ae_state *_state);
void _pexec_covm(/* Real    */ ae_matrix* x,
    ae_int_t n,
    ae_int_t m,
    /* Real    */ ae_matrix* c, ae_state *_state);


/*************************************************************************
Pearson product-moment correlation matrix

SMP EDITION OF ALGLIB:

  ! This function can utilize multicore capabilities of  your system.  In
  ! order to do this you have to call version with "smp_" prefix,   which
  ! indicates that multicore code will be used.
  ! 
  ! This note is given for users of SMP edition; if you use GPL  edition,
  ! or commercial edition of ALGLIB without SMP support, you  still  will
  ! be able to call smp-version of this function,  but  all  computations
  ! will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.
  !
  ! You should remember that starting/stopping worker thread always  have
  ! non-zero cost. Although  multicore  version  is  pretty  efficient on
  ! large problems, we do not recommend you to use it on small problems -
  ! with correlation matrices smaller than 128*128.

INPUT PARAMETERS:
    X   -   array[N,M], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    N   -   N>=0, number of observations:
            * if given, only leading N rows of X are used
            * if not given, automatically determined from input size
    M   -   M>0, number of variables:
            * if given, only leading M columns of X are used
            * if not given, automatically determined from input size

OUTPUT PARAMETERS:
    C   -   array[M,M], correlation matrix (zero if N=0 or N=1)

  -- ALGLIB --
     Copyright 28.10.2010 by Bochkanov Sergey
*************************************************************************/
void pearsoncorrm(/* Real    */ ae_matrix* x,
     ae_int_t n,
     ae_int_t m,
     /* Real    */ ae_matrix* c,
     ae_state *_state);
void _pexec_pearsoncorrm(/* Real    */ ae_matrix* x,
    ae_int_t n,
    ae_int_t m,
    /* Real    */ ae_matrix* c, ae_state *_state);


/*************************************************************************
Spearman's rank correlation matrix

SMP EDITION OF ALGLIB:

  ! This function can utilize multicore capabilities of  your system.  In
  ! order to do this you have to call version with "smp_" prefix,   which
  ! indicates that multicore code will be used.
  ! 
  ! This note is given for users of SMP edition; if you use GPL  edition,
  ! or commercial edition of ALGLIB without SMP support, you  still  will
  ! be able to call smp-version of this function,  but  all  computations
  ! will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.
  !
  ! You should remember that starting/stopping worker thread always  have
  ! non-zero cost. Although  multicore  version  is  pretty  efficient on
  ! large problems, we do not recommend you to use it on small problems -
  ! with correlation matrices smaller than 128*128.

INPUT PARAMETERS:
    X   -   array[N,M], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    N   -   N>=0, number of observations:
            * if given, only leading N rows of X are used
            * if not given, automatically determined from input size
    M   -   M>0, number of variables:
            * if given, only leading M columns of X are used
            * if not given, automatically determined from input size

OUTPUT PARAMETERS:
    C   -   array[M,M], correlation matrix (zero if N=0 or N=1)

  -- ALGLIB --
     Copyright 28.10.2010 by Bochkanov Sergey
*************************************************************************/
void spearmancorrm(/* Real    */ ae_matrix* x,
     ae_int_t n,
     ae_int_t m,
     /* Real    */ ae_matrix* c,
     ae_state *_state);
void _pexec_spearmancorrm(/* Real    */ ae_matrix* x,
    ae_int_t n,
    ae_int_t m,
    /* Real    */ ae_matrix* c, ae_state *_state);


/*************************************************************************
Cross-covariance matrix

SMP EDITION OF ALGLIB:

  ! This function can utilize multicore capabilities of  your system.  In
  ! order to do this you have to call version with "smp_" prefix,   which
  ! indicates that multicore code will be used.
  ! 
  ! This note is given for users of SMP edition; if you use GPL  edition,
  ! or commercial edition of ALGLIB without SMP support, you  still  will
  ! be able to call smp-version of this function,  but  all  computations
  ! will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.
  !
  ! You should remember that starting/stopping worker thread always  have
  ! non-zero cost. Although  multicore  version  is  pretty  efficient on
  ! large problems, we do not recommend you to use it on small problems -
  ! with covariance matrices smaller than 128*128.

INPUT PARAMETERS:
    X   -   array[N,M1], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    Y   -   array[N,M2], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    N   -   N>=0, number of observations:
            * if given, only leading N rows of X/Y are used
            * if not given, automatically determined from input sizes
    M1  -   M1>0, number of variables in X:
            * if given, only leading M1 columns of X are used
            * if not given, automatically determined from input size
    M2  -   M2>0, number of variables in Y:
            * if given, only leading M1 columns of X are used
            * if not given, automatically determined from input size

OUTPUT PARAMETERS:
    C   -   array[M1,M2], cross-covariance matrix (zero if N=0 or N=1)

  -- ALGLIB --
     Copyright 28.10.2010 by Bochkanov Sergey
*************************************************************************/
void covm2(/* Real    */ ae_matrix* x,
     /* Real    */ ae_matrix* y,
     ae_int_t n,
     ae_int_t m1,
     ae_int_t m2,
     /* Real    */ ae_matrix* c,
     ae_state *_state);
void _pexec_covm2(/* Real    */ ae_matrix* x,
    /* Real    */ ae_matrix* y,
    ae_int_t n,
    ae_int_t m1,
    ae_int_t m2,
    /* Real    */ ae_matrix* c, ae_state *_state);


/*************************************************************************
Pearson product-moment cross-correlation matrix

SMP EDITION OF ALGLIB:

  ! This function can utilize multicore capabilities of  your system.  In
  ! order to do this you have to call version with "smp_" prefix,   which
  ! indicates that multicore code will be used.
  ! 
  ! This note is given for users of SMP edition; if you use GPL  edition,
  ! or commercial edition of ALGLIB without SMP support, you  still  will
  ! be able to call smp-version of this function,  but  all  computations
  ! will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.
  !
  ! You should remember that starting/stopping worker thread always  have
  ! non-zero cost. Although  multicore  version  is  pretty  efficient on
  ! large problems, we do not recommend you to use it on small problems -
  ! with correlation matrices smaller than 128*128.

INPUT PARAMETERS:
    X   -   array[N,M1], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    Y   -   array[N,M2], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    N   -   N>=0, number of observations:
            * if given, only leading N rows of X/Y are used
            * if not given, automatically determined from input sizes
    M1  -   M1>0, number of variables in X:
            * if given, only leading M1 columns of X are used
            * if not given, automatically determined from input size
    M2  -   M2>0, number of variables in Y:
            * if given, only leading M1 columns of X are used
            * if not given, automatically determined from input size

OUTPUT PARAMETERS:
    C   -   array[M1,M2], cross-correlation matrix (zero if N=0 or N=1)

  -- ALGLIB --
     Copyright 28.10.2010 by Bochkanov Sergey
*************************************************************************/
void pearsoncorrm2(/* Real    */ ae_matrix* x,
     /* Real    */ ae_matrix* y,
     ae_int_t n,
     ae_int_t m1,
     ae_int_t m2,
     /* Real    */ ae_matrix* c,
     ae_state *_state);
void _pexec_pearsoncorrm2(/* Real    */ ae_matrix* x,
    /* Real    */ ae_matrix* y,
    ae_int_t n,
    ae_int_t m1,
    ae_int_t m2,
    /* Real    */ ae_matrix* c, ae_state *_state);


/*************************************************************************
Spearman's rank cross-correlation matrix

SMP EDITION OF ALGLIB:

  ! This function can utilize multicore capabilities of  your system.  In
  ! order to do this you have to call version with "smp_" prefix,   which
  ! indicates that multicore code will be used.
  ! 
  ! This note is given for users of SMP edition; if you use GPL  edition,
  ! or commercial edition of ALGLIB without SMP support, you  still  will
  ! be able to call smp-version of this function,  but  all  computations
  ! will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.
  !
  ! You should remember that starting/stopping worker thread always  have
  ! non-zero cost. Although  multicore  version  is  pretty  efficient on
  ! large problems, we do not recommend you to use it on small problems -
  ! with correlation matrices smaller than 128*128.

INPUT PARAMETERS:
    X   -   array[N,M1], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    Y   -   array[N,M2], sample matrix:
            * J-th column corresponds to J-th variable
            * I-th row corresponds to I-th observation
    N   -   N>=0, number of observations:
            * if given, only leading N rows of X/Y are used
            * if not given, automatically determined from input sizes
    M1  -   M1>0, number of variables in X:
            * if given, only leading M1 columns of X are used
            * if not given, automatically determined from input size
    M2  -   M2>0, number of variables in Y:
            * if given, only leading M1 columns of X are used
            * if not given, automatically determined from input size

OUTPUT PARAMETERS:
    C   -   array[M1,M2], cross-correlation matrix (zero if N=0 or N=1)

  -- ALGLIB --
     Copyright 28.10.2010 by Bochkanov Sergey
*************************************************************************/
void spearmancorrm2(/* Real    */ ae_matrix* x,
     /* Real    */ ae_matrix* y,
     ae_int_t n,
     ae_int_t m1,
     ae_int_t m2,
     /* Real    */ ae_matrix* c,
     ae_state *_state);
void _pexec_spearmancorrm2(/* Real    */ ae_matrix* x,
    /* Real    */ ae_matrix* y,
    ae_int_t n,
    ae_int_t m1,
    ae_int_t m2,
    /* Real    */ ae_matrix* c, ae_state *_state);


/*************************************************************************
This function replaces data in XY by their ranks:
* XY is processed row-by-row
* rows are processed separately
* tied data are correctly handled (tied ranks are calculated)
* ranking starts from 0, ends at NFeatures-1
* sum of within-row values is equal to (NFeatures-1)*NFeatures/2

SMP EDITION OF ALGLIB:

  ! This function can utilize multicore capabilities of  your system.  In
  ! order to do this you have to call version with "smp_" prefix,   which
  ! indicates that multicore code will be used.
  ! 
  ! This note is given for users of SMP edition; if you use GPL  edition,
  ! or commercial edition of ALGLIB without SMP support, you  still  will
  ! be able to call smp-version of this function,  but  all  computations
  ! will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.
  !
  ! You should remember that starting/stopping worker thread always  have
  ! non-zero cost. Although  multicore  version  is  pretty  efficient on
  ! large problems, we do not recommend you to use it on small problems -
  ! ones where expected operations count is less than 100.000

INPUT PARAMETERS:
    XY      -   array[NPoints,NFeatures], dataset
    NPoints -   number of points
    NFeatures-  number of features

OUTPUT PARAMETERS:
    XY      -   data are replaced by their within-row ranks;
                ranking starts from 0, ends at NFeatures-1

  -- ALGLIB --
     Copyright 18.04.2013 by Bochkanov Sergey
*************************************************************************/
void rankdata(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nfeatures,
     ae_state *_state);
void _pexec_rankdata(/* Real    */ ae_matrix* xy,
    ae_int_t npoints,
    ae_int_t nfeatures, ae_state *_state);


/*************************************************************************
This function replaces data in XY by their CENTERED ranks:
* XY is processed row-by-row
* rows are processed separately
* tied data are correctly handled (tied ranks are calculated)
* centered ranks are just usual ranks, but centered in such way  that  sum
  of within-row values is equal to 0.0.
* centering is performed by subtracting mean from each row, i.e it changes
  mean value, but does NOT change higher moments

SMP EDITION OF ALGLIB:

  ! This function can utilize multicore capabilities of  your system.  In
  ! order to do this you have to call version with "smp_" prefix,   which
  ! indicates that multicore code will be used.
  ! 
  ! This note is given for users of SMP edition; if you use GPL  edition,
  ! or commercial edition of ALGLIB without SMP support, you  still  will
  ! be able to call smp-version of this function,  but  all  computations
  ! will be done serially.
  !
  ! We recommend you to carefully read ALGLIB Reference  Manual,  section
  ! called 'SMP support', before using parallel version of this function.
  !
  ! You should remember that starting/stopping worker thread always  have
  ! non-zero cost. Although  multicore  version  is  pretty  efficient on
  ! large problems, we do not recommend you to use it on small problems -
  ! ones where expected operations count is less than 100.000

INPUT PARAMETERS:
    XY      -   array[NPoints,NFeatures], dataset
    NPoints -   number of points
    NFeatures-  number of features

OUTPUT PARAMETERS:
    XY      -   data are replaced by their within-row ranks;
                ranking starts from 0, ends at NFeatures-1

  -- ALGLIB --
     Copyright 18.04.2013 by Bochkanov Sergey
*************************************************************************/
void rankdatacentered(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nfeatures,
     ae_state *_state);
void _pexec_rankdatacentered(/* Real    */ ae_matrix* xy,
    ae_int_t npoints,
    ae_int_t nfeatures, ae_state *_state);


/*************************************************************************
Obsolete function, we recommend to use PearsonCorr2().

  -- ALGLIB --
     Copyright 09.04.2007 by Bochkanov Sergey
*************************************************************************/
double pearsoncorrelation(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_state *_state);


/*************************************************************************
Obsolete function, we recommend to use SpearmanCorr2().

    -- ALGLIB --
    Copyright 09.04.2007 by Bochkanov Sergey
*************************************************************************/
double spearmanrankcorrelation(/* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     ae_int_t n,
     ae_state *_state);


/*$ End $*/
#endif

