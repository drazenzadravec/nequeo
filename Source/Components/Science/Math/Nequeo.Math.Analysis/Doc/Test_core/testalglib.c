#include <stdio.h>
#include <time.h>
/*************************************************************************
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
*************************************************************************/
#include "apserv.h"
#include "hqrnd.h"
#include "scodes.h"
#include "tsort.h"
#include "nearestneighbor.h"
#include "xdebug.h"
#include "ablasf.h"
#include "ablasmkl.h"
#include "ablas.h"
#include "basicstatops.h"
#include "basestat.h"
#include "bdss.h"
#include "blas.h"
#include "clustering.h"
#include "datacomp.h"
#include "dforest.h"
#include "gammafunc.h"
#include "normaldistr.h"
#include "igammaf.h"
#include "hblas.h"
#include "reflections.h"
#include "creflections.h"
#include "sblas.h"
#include "ortfac.h"
#include "rotations.h"
#include "bdsvd.h"
#include "svd.h"
#include "linreg.h"
#include "filters.h"
#include "hsschur.h"
#include "evd.h"
#include "matgen.h"
#include "sparse.h"
#include "trfac.h"
#include "trlinsolve.h"
#include "safesolve.h"
#include "rcond.h"
#include "matinv.h"
#include "lda.h"
#include "hpccores.h"
#include "mlpbase.h"
#include "xblas.h"
#include "densesolver.h"
#include "logit.h"
#include "optserv.h"
#include "fbls.h"
#include "cqmodels.h"
#include "snnls.h"
#include "sactivesets.h"
#include "linmin.h"
#include "mincg.h"
#include "minbleic.h"
#include "mcpd.h"
#include "mlpe.h"
#include "minlbfgs.h"
#include "mlptrain.h"
#include "pca.h"
#include "odesolver.h"
#include "ntheory.h"
#include "ftbase.h"
#include "fft.h"
#include "conv.h"
#include "corr.h"
#include "fht.h"
#include "gq.h"
#include "gkq.h"
#include "autogk.h"
#include "idwint.h"
#include "ratint.h"
#include "polint.h"
#include "spline1d.h"
#include "normestimator.h"
#include "qqpsolver.h"
#include "qpbleicsolver.h"
#include "qpcholeskysolver.h"
#include "minqp.h"
#include "minlm.h"
#include "lsfit.h"
#include "parametric.h"
#include "linlsqr.h"
#include "rbf.h"
#include "spline2d.h"
#include "spline3d.h"
#include "matdet.h"
#include "spdgevd.h"
#include "inverseupdate.h"
#include "schur.h"
#include "mincomp.h"
#include "minnlc.h"
#include "minns.h"
#include "lincg.h"
#include "nleq.h"
#include "polynomialsolver.h"
#include "airyf.h"
#include "bessel.h"
#include "betaf.h"
#include "ibetaf.h"
#include "nearunityunit.h"
#include "binomialdistr.h"
#include "chebyshev.h"
#include "chisquaredistr.h"
#include "dawson.h"
#include "elliptic.h"
#include "expintegrals.h"
#include "fdistr.h"
#include "fresnel.h"
#include "hermite.h"
#include "jacobianelliptic.h"
#include "laguerre.h"
#include "legendre.h"
#include "poissondistr.h"
#include "psif.h"
#include "studenttdistr.h"
#include "trigintegrals.h"
#include "correlationtests.h"
#include "jarquebera.h"
#include "mannwhitneyu.h"
#include "stest.h"
#include "studentttests.h"
#include "variancetests.h"
#include "wsr.h"
#include "alglibbasics.h"

#include "testhqrndunit.h"
#include "testtsortunit.h"
#include "testnearestneighborunit.h"
#include "testablasunit.h"
#include "testbasestatunit.h"
#include "testbdssunit.h"
#include "testblasunit.h"
#include "testclusteringunit.h"
#include "testdforestunit.h"
#include "testgammafuncunit.h"
#include "testhblasunit.h"
#include "testreflectionsunit.h"
#include "testcreflectionsunit.h"
#include "testsblasunit.h"
#include "testortfacunit.h"
#include "testbdsvdunit.h"
#include "testsvdunit.h"
#include "testlinregunit.h"
#include "testfiltersunit.h"
#include "testevdunit.h"
#include "testmatgenunit.h"
#include "testsparseunit.h"
#include "testtrfacunit.h"
#include "testtrlinsolveunit.h"
#include "testsafesolveunit.h"
#include "testrcondunit.h"
#include "testmatinvunit.h"
#include "testldaunit.h"
#include "testmlpbaseunit.h"
#include "testxblasunit.h"
#include "testdensesolverunit.h"
#include "testoptservunit.h"
#include "testfblsunit.h"
#include "testcqmodelsunit.h"
#include "testsnnlsunit.h"
#include "testsactivesetsunit.h"
#include "testlinminunit.h"
#include "testmincgunit.h"
#include "testminbleicunit.h"
#include "testmcpdunit.h"
#include "testmlpeunit.h"
#include "testminlbfgsunit.h"
#include "testmlptrainunit.h"
#include "testpcaunit.h"
#include "testodesolverunit.h"
#include "testfftunit.h"
#include "testconvunit.h"
#include "testcorrunit.h"
#include "testfhtunit.h"
#include "testgqunit.h"
#include "testgkqunit.h"
#include "testautogkunit.h"
#include "testidwintunit.h"
#include "testratintunit.h"
#include "testpolintunit.h"
#include "testspline1dunit.h"
#include "testnormestimatorunit.h"
#include "testminqpunit.h"
#include "testminlmunit.h"
#include "testlsfitunit.h"
#include "testparametricunit.h"
#include "testlinlsqrunit.h"
#include "testrbfunit.h"
#include "testspline2dunit.h"
#include "testspline3dunit.h"
#include "testspdgevdunit.h"
#include "testinverseupdateunit.h"
#include "testschurunit.h"
#include "testminnlcunit.h"
#include "testminnsunit.h"
#include "testlincgunit.h"
#include "testnlequnit.h"
#include "testpolynomialsolverunit.h"
#include "testchebyshevunit.h"
#include "testhermiteunit.h"
#include "testlaguerreunit.h"
#include "testlegendreunit.h"
#include "teststestunit.h"
#include "teststudentttestsunit.h"
#include "testalglibbasicsunit.h"


/*$ Body $*/
#if (AE_OS==AE_WINDOWS) || defined(AE_DEBUG4WINDOWS)
#include <windows.h>
#endif
#if (AE_OS==AE_POSIX) || defined(AE_DEBUG4POSIX)
#include <unistd.h>
#include <pthread.h>
#endif
#define AE_SINGLECORE           1
#define AE_SEQUENTIAL_MULTICORE 2
#define AE_PARALLEL_SINGLECORE 3
#define AE_PARALLEL_MULTICORE 4
#define AE_SKIP_TEST 5
unsigned seed;
int global_failure_flag = 0;
ae_bool use_smp = ae_false;
#if (AE_OS==AE_WINDOWS) || defined(AE_DEBUG4WINDOWS)
CRITICAL_SECTION tests_lock;
CRITICAL_SECTION print_lock;
#elif (AE_OS==AE_POSIX) || defined(AE_DEBUG4POSIX)
pthread_mutex_t tests_lock;
pthread_mutex_t print_lock;
#else
void *tests_lock = NULL;
void *print_lock = NULL;
#endif
typedef struct
{
    const char *name;
    ae_bool (*seq_testfunc)(ae_bool, ae_state*);
    ae_bool(*smp_testfunc)(ae_bool, ae_state*);
    
} _s_testrecord;
int unittests_processed = 0;
_s_testrecord unittests[] =
{
    {"hqrnd",testhqrnd,_pexec_testhqrnd},
    {"tsort",testtsort,_pexec_testtsort},
    {"nearestneighbor",testnearestneighbor,_pexec_testnearestneighbor},
    {"ablas",testablas,_pexec_testablas},
    {"basestat",testbasestat,_pexec_testbasestat},
    {"bdss",testbdss,_pexec_testbdss},
    {"blas",testblas,_pexec_testblas},
    {"clustering",testclustering,_pexec_testclustering},
    {"dforest",testdforest,_pexec_testdforest},
    {"gammafunc",testgammafunc,_pexec_testgammafunc},
    {"hblas",testhblas,_pexec_testhblas},
    {"reflections",testreflections,_pexec_testreflections},
    {"creflections",testcreflections,_pexec_testcreflections},
    {"sblas",testsblas,_pexec_testsblas},
    {"ortfac",testortfac,_pexec_testortfac},
    {"bdsvd",testbdsvd,_pexec_testbdsvd},
    {"svd",testsvd,_pexec_testsvd},
    {"linreg",testlinreg,_pexec_testlinreg},
    {"filters",testfilters,_pexec_testfilters},
    {"evd",testevd,_pexec_testevd},
    {"matgen",testmatgen,_pexec_testmatgen},
    {"sparse",testsparse,_pexec_testsparse},
    {"trfac",testtrfac,_pexec_testtrfac},
    {"trlinsolve",testtrlinsolve,_pexec_testtrlinsolve},
    {"safesolve",testsafesolve,_pexec_testsafesolve},
    {"rcond",testrcond,_pexec_testrcond},
    {"matinv",testmatinv,_pexec_testmatinv},
    {"lda",testlda,_pexec_testlda},
    {"mlpbase",testmlpbase,_pexec_testmlpbase},
    {"xblas",testxblas,_pexec_testxblas},
    {"densesolver",testdensesolver,_pexec_testdensesolver},
    {"optserv",testoptserv,_pexec_testoptserv},
    {"fbls",testfbls,_pexec_testfbls},
    {"cqmodels",testcqmodels,_pexec_testcqmodels},
    {"snnls",testsnnls,_pexec_testsnnls},
    {"sactivesets",testsactivesets,_pexec_testsactivesets},
    {"linmin",testlinmin,_pexec_testlinmin},
    {"mincg",testmincg,_pexec_testmincg},
    {"minbleic",testminbleic,_pexec_testminbleic},
    {"mcpd",testmcpd,_pexec_testmcpd},
    {"mlpe",testmlpe,_pexec_testmlpe},
    {"minlbfgs",testminlbfgs,_pexec_testminlbfgs},
    {"mlptrain",testmlptrain,_pexec_testmlptrain},
    {"pca",testpca,_pexec_testpca},
    {"odesolver",testodesolver,_pexec_testodesolver},
    {"fft",testfft,_pexec_testfft},
    {"conv",testconv,_pexec_testconv},
    {"corr",testcorr,_pexec_testcorr},
    {"fht",testfht,_pexec_testfht},
    {"gq",testgq,_pexec_testgq},
    {"gkq",testgkq,_pexec_testgkq},
    {"autogk",testautogk,_pexec_testautogk},
    {"idwint",testidwint,_pexec_testidwint},
    {"ratint",testratint,_pexec_testratint},
    {"polint",testpolint,_pexec_testpolint},
    {"spline1d",testspline1d,_pexec_testspline1d},
    {"normestimator",testnormestimator,_pexec_testnormestimator},
    {"minqp",testminqp,_pexec_testminqp},
    {"minlm",testminlm,_pexec_testminlm},
    {"lsfit",testlsfit,_pexec_testlsfit},
    {"parametric",testparametric,_pexec_testparametric},
    {"linlsqr",testlinlsqr,_pexec_testlinlsqr},
    {"rbf",testrbf,_pexec_testrbf},
    {"spline2d",testspline2d,_pexec_testspline2d},
    {"spline3d",testspline3d,_pexec_testspline3d},
    {"spdgevd",testspdgevd,_pexec_testspdgevd},
    {"inverseupdate",testinverseupdate,_pexec_testinverseupdate},
    {"schur",testschur,_pexec_testschur},
    {"minnlc",testminnlc,_pexec_testminnlc},
    {"minns",testminns,_pexec_testminns},
    {"lincg",testlincg,_pexec_testlincg},
    {"nleq",testnleq,_pexec_testnleq},
    {"polynomialsolver",testpolynomialsolver,_pexec_testpolynomialsolver},
    {"chebyshev",testchebyshev,_pexec_testchebyshev},
    {"hermite",testhermite,_pexec_testhermite},
    {"laguerre",testlaguerre,_pexec_testlaguerre},
    {"legendre",testlegendre,_pexec_testlegendre},
    {"stest",teststest,_pexec_teststest},
    {"studentttests",teststudentttests,_pexec_teststudentttests},
    {"alglibbasics",testalglibbasics,_pexec_testalglibbasics},

    {NULL, NULL, NULL}
};

#if (AE_OS==AE_WINDOWS) || defined(AE_DEBUG4WINDOWS)
void acquire_lock(CRITICAL_SECTION *p_lock)
{
    EnterCriticalSection(p_lock);
}
void release_lock(CRITICAL_SECTION *p_lock)
{
    LeaveCriticalSection(p_lock);
}
#elif (AE_OS==AE_POSIX) || defined(AE_DEBUG4POSIX)
void acquire_lock(pthread_mutex_t *p_lock)
{
    pthread_mutex_lock(p_lock);
}
void release_lock(pthread_mutex_t *p_lock)
{
    pthread_mutex_unlock(p_lock);
}
#else
void acquire_lock(void **p_lock)
{
}
void release_lock(void **p_lock)
{
}
#endif

ae_bool call_unittest(
    ae_bool(*seq_testfunc)(ae_bool, ae_state*),
    ae_bool(*smp_testfunc)(ae_bool, ae_state*),
    int *psticky)
{
#ifndef AE_USE_CPP_ERROR_HANDLING
    ae_state _alglib_env_state;
    ae_frame _frame_block;
    jmp_buf _break_jump;
    ae_bool result;
    
    ae_state_init(&_alglib_env_state);
    if( setjmp(_break_jump) )
    {
        *psticky = 1;
        return ae_false;
    }
    ae_state_set_break_jump(&_alglib_env_state, &_break_jump);
    ae_frame_make(&_alglib_env_state, &_frame_block);
    if( use_smp )
        result = smp_testfunc(ae_true, &_alglib_env_state);
    else
        result = seq_testfunc(ae_true, &_alglib_env_state);
    ae_state_clear(&_alglib_env_state);
    if( !result )
        *psticky = 1;
    return result;
#else
    try
    {
        ae_state _alglib_env_state;
        ae_frame _frame_block;
        ae_bool result;
        
        ae_state_init(&_alglib_env_state);
        ae_frame_make(&_alglib_env_state, &_frame_block);
        if( use_smp )
            result = smp_testfunc(ae_true, &_alglib_env_state);
        else
            result = seq_testfunc(ae_true, &_alglib_env_state);
        ae_state_clear(&_alglib_env_state);
        if( !result )
            *psticky = 1;
        return result;
    }
    catch(...)
    {
        *psticky = 1;
        return ae_false;
    }
#endif
}

#if (AE_OS==AE_WINDOWS) || defined(AE_DEBUG4WINDOWS)
DWORD WINAPI tester_function(LPVOID T)
#elif AE_OS==AE_POSIX   || defined(AE_DEBUG4POSIX)
void* tester_function(void *T)
#else
void tester_function(void *T)
#endif
{
    int idx;
    ae_bool status;
    for(;;)
    {
        /*
         * try to acquire test record
         */
        acquire_lock(&tests_lock);
        if( unittests[unittests_processed].name==NULL )
        {
            release_lock(&tests_lock);
            break;
        
        }
        idx = unittests_processed;
        unittests_processed++;
        release_lock(&tests_lock);
        
        /*
         * Call unit test
         */
        status = call_unittest(
            unittests[idx].seq_testfunc,
            unittests[idx].smp_testfunc,
            &global_failure_flag);
        acquire_lock(&print_lock);
        if( status )
            printf("%-32s OK\n", unittests[idx].name);
        else
            printf("%-32s FAILED\n", unittests[idx].name);
        fflush(stdout);
        release_lock(&print_lock);
    }
#if AE_OS==AE_WINDOWS || defined(AE_DEBUG4WINDOWS)
    return 0;
#elif AE_OS==AE_POSIX || defined(AE_DEBUG4POSIX)
    return NULL;
#else
#endif    
}

int main(int argc, char **argv)
{
    time_t time_0, time_1;
    union
    {
        double a;
        ae_int32_t p[2];
    } u;
    if( argc==2 )
        seed = (unsigned)atoi(argv[1]);
    else
    {
        time_t t;
        seed = (unsigned)time(&t);
    }
#if (AE_OS==AE_WINDOWS) || defined(AE_DEBUG4WINDOWS)
    InitializeCriticalSection(&tests_lock);
    InitializeCriticalSection(&print_lock);
#elif (AE_OS==AE_POSIX) || defined(AE_DEBUG4POSIX)
    pthread_mutex_init(&tests_lock, NULL);
    pthread_mutex_init(&print_lock, NULL);
#endif
    
    /*
     * SMP settings
     */
#if AE_TEST==AE_PARALLEL_MULTICORE || AE_TEST==AE_SEQUENTIAL_MULTICORE
    use_smp = ae_true;
#else
    use_smp = ae_false;
#endif
    
    /* 
     * Seed
     */
    printf("SEED: %u\n", (unsigned int)seed);
    srand(seed);
    
    /* 
     * Compiler
     */
#if AE_COMPILER==AE_GNUC
    printf("COMPILER: GCC\n");
#elif AE_COMPILER==AE_SUNC
    printf("COMPILER: SunStudio\n");
#elif AE_COMPILER==AE_MSVC
    printf("COMPILER: MSVC\n");
#else
    printf("COMPILER: unknown\n");
#endif
    
    /*
     * Architecture
     */
    if( sizeof(void*)==4 )
        printf("HARDWARE: 32-bit\n");
    else if( sizeof(void*)==8 )
        printf("HARDWARE: 64-bit\n");
    else
        printf("HARDWARE: strange (non-32, non-64)\n");
    
    /* 
     * determine endianness of hardware.
     * 1983 is a good number - non-periodic double representation allow us to
     * easily distinguish between upper and lower halfs and to detect mixed endian hardware.
     */
    u.a = 1.0/1983.0; 
    if( u.p[1]==0x3f408642 )
        printf("HARDWARE: little-endian\n");
    else if( u.p[0]==0x3f408642 )
        printf("HARDWARE: big-endian\n");
    else
        printf("HARDWARE: mixed-endian\n");
    
    /* 
     * CPU (as defined)
     */
#if AE_CPU==AE_INTEL
    printf("CPU:   Intel\n");
#elif AE_CPU==AE_SPARC
    printf("CPU:   SPARC\n");
#else
    printf("CPU:   unknown\n");
#endif

    /* 
     * Cores count
     */
#ifdef AE_HPC
    printf("CORES: %d\n", (int)ae_cores_count());
#else
    printf("CORES: 1 (serial version)\n");
#endif

    /*
     * Support for vendor libraries
     */
#ifdef AE_MKL
    printf("LIBS:  MKL (Intel)\n");
#else
    printf("LIBS:  \n");
#endif

    /* 
     * CPUID results
     */
    printf("CPUID: %s\n", ae_cpuid()&CPU_SSE2 ? "sse2" : "");

    /* 
     * OS
     */
#if AE_OS==AE_WINDOWS
    printf("OS: Windows\n");
#elif AE_OS==AE_POSIX
    printf("OS: POSIX\n");
#else
    printf("OS: unknown\n");
#endif

    /* 
     * Testing mode
     */
#if (AE_TEST==0) || (AE_TEST==AE_SINGLECORE)
    printf("TESTING MODE: single core\n");
#elif AE_TEST==AE_PARALLEL_SINGLECORE
    printf("TESTING MODE: single core, parallel\n");
#elif AE_TEST==AE_SEQUENTIAL_MULTICORE
    printf("TESTING MODE: milti-core, sequential\n");
#elif AE_TEST==AE_PARALLEL_MULTICORE
    printf("TESTING MODE: milti-core, parallel\n");
#elif AE_TEST==AE_SKIP_TEST
    printf("TESTING MODE: just compiling\n");
    printf("Done in 0 seconds\n");
    return 0;
#else
#error Unknown AE_TEST being passed
#endif
    
    /*
     * now we are ready to test!
     */
    time(&time_0);
#ifdef AE_HPC
    if( ae_smpselftests() )
        printf("%-32s OK\n", "SMP self tests");
    else
    {
        printf("%-32s FAILED\n", "SMP self tests");
        return 1;
    }
#endif
    fflush(stdout);
#if AE_TEST==0 || AE_TEST==AE_SINGLECORE || AE_TEST==AE_SEQUENTIAL_MULTICORE || AE_TEST==AE_SKIP_TEST
    tester_function(NULL);
#elif AE_TEST==AE_PARALLEL_MULTICORE || AE_TEST==AE_PARALLEL_SINGLECORE
#ifdef AE_HPC
    ae_set_cores_to_use(0);
#endif
#if AE_OS==AE_WINDOWS || defined(AE_DEBUG4WINDOWS)
    {
        SYSTEM_INFO sysInfo;
        HANDLE  *hThreads = NULL;
        int idx;
        GetSystemInfo(&sysInfo);
        ae_assert(sysInfo.dwNumberOfProcessors>=1, "processors count is less than 1", NULL);
        hThreads = (HANDLE*)malloc(sizeof(HANDLE)*sysInfo.dwNumberOfProcessors);
        ae_assert(hThreads!=NULL, "malloc failure", NULL);
        for(idx=0; idx<sysInfo.dwNumberOfProcessors; idx++)
            hThreads[idx] = CreateThread(NULL, 0, tester_function, NULL, 0, NULL);
        WaitForMultipleObjects(sysInfo.dwNumberOfProcessors, hThreads, TRUE, INFINITE);
    }
#elif AE_OS==AE_POSIX || defined(AE_DEBUG4POSIX)
    {
        long cpu_cnt;
        pthread_t  *threads = NULL;
        int idx;
        cpu_cnt = sysconf(_SC_NPROCESSORS_ONLN);
        ae_assert(cpu_cnt>=1, "processors count is less than 1", NULL);
        threads = (pthread_t*)malloc(sizeof(pthread_t)*cpu_cnt);
        ae_assert(threads!=NULL, "malloc failure", NULL);
        for(idx=0; idx<cpu_cnt; idx++)
        {
            int status = pthread_create(&threads[idx], NULL, tester_function, NULL);
            if( status!=0 )
            {
                printf("Failed to create thread\n");
                abort();
            }
        }
        for(idx=0; idx<cpu_cnt; idx++)
            pthread_join(threads[idx], NULL);
    }
#else
#error Unable to determine OS (define AE_OS, AE_DEBUG4WINDOWS or AE_DEBUG4POSIX)
#endif
#else
#error Unexpected test mode
#endif
    time(&time_1);
    printf("Done in %ld seconds\n", (long)difftime(time_1, time_0));

    /*
     * Free structures
     */
#if (AE_OS==AE_WINDOWS) || defined(AE_DEBUG4WINDOWS)
    DeleteCriticalSection(&tests_lock);
    DeleteCriticalSection(&print_lock);
#elif (AE_OS==AE_POSIX) || defined(AE_DEBUG4POSIX)
    pthread_mutex_destroy(&tests_lock);
    pthread_mutex_destroy(&print_lock);
#endif

    /*
     * Return result
     */
    return global_failure_flag;
}
/*$ End $*/
