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
#include "testclusteringunit.h"


/*$ Declarations $*/
static ae_bool testclusteringunit_basicahctests(ae_state *_state);
static ae_bool testclusteringunit_advancedahctests(ae_state *_state);
static void testclusteringunit_kmeanssimpletest1(ae_int_t nvars,
     ae_int_t nc,
     ae_int_t passcount,
     ae_bool* converrors,
     ae_bool* othererrors,
     ae_bool* simpleerrors,
     ae_state *_state);
static void testclusteringunit_kmeansspecialtests(ae_bool* othererrors,
     ae_state *_state);
static void testclusteringunit_kmeansinfinitelooptest(ae_bool* othererrors,
     ae_state *_state);
static void testclusteringunit_kmeansrestartstest(ae_bool* converrors,
     ae_bool* restartserrors,
     ae_state *_state);
static double testclusteringunit_rnormal(ae_state *_state);
static void testclusteringunit_rsphere(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t i,
     ae_state *_state);
static double testclusteringunit_distfunc(/* Real    */ ae_vector* x0,
     /* Real    */ ae_vector* x1,
     ae_int_t d,
     ae_int_t disttype,
     ae_state *_state);
static ae_bool testclusteringunit_errorsinmerges(/* Real    */ ae_matrix* d,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nf,
     ahcreport* rep,
     ae_int_t ahcalgo,
     ae_state *_state);
static void testclusteringunit_kmeansreferenceupdatedistances(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     /* Real    */ ae_matrix* ct,
     ae_int_t k,
     /* Integer */ ae_vector* xyc,
     /* Real    */ ae_vector* xydist2,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Testing clustering
*************************************************************************/
ae_bool testclustering(ae_bool silent, ae_state *_state)
{
    ae_bool waserrors;
    ae_bool basicahcerrors;
    ae_bool ahcerrors;
    ae_bool kmeansconverrors;
    ae_bool kmeanssimpleerrors;
    ae_bool kmeansothererrors;
    ae_bool kmeansrestartserrors;
    ae_int_t passcount;
    ae_int_t nf;
    ae_int_t nc;
    ae_bool result;


    
    /*
     * AHC tests
     */
    basicahcerrors = testclusteringunit_basicahctests(_state);
    ahcerrors = testclusteringunit_advancedahctests(_state);
    
    /*
     * k-means tests
     */
    passcount = 10;
    kmeansconverrors = ae_false;
    kmeansothererrors = ae_false;
    kmeanssimpleerrors = ae_false;
    kmeansrestartserrors = ae_false;
    testclusteringunit_kmeansspecialtests(&kmeansothererrors, _state);
    testclusteringunit_kmeansinfinitelooptest(&kmeansothererrors, _state);
    testclusteringunit_kmeansrestartstest(&kmeansconverrors, &kmeansrestartserrors, _state);
    for(nf=1; nf<=5; nf++)
    {
        for(nc=1; nc<=5; nc++)
        {
            testclusteringunit_kmeanssimpletest1(nf, nc, passcount, &kmeansconverrors, &kmeansothererrors, &kmeanssimpleerrors, _state);
        }
    }
    
    /*
     * Results
     */
    waserrors = ae_false;
    waserrors = waserrors||(basicahcerrors||ahcerrors);
    waserrors = waserrors||(((kmeansconverrors||kmeansothererrors)||kmeanssimpleerrors)||kmeansrestartserrors);
    if( !silent )
    {
        printf("TESTING CLUSTERING\n");
        printf("AHC:                                \n");
        printf("* BASIC TESTS                       ");
        if( !basicahcerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* GENERAL TESTS                     ");
        if( !ahcerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("K-MEANS:                            \n");
        printf("* CONVERGENCE                       ");
        if( !kmeansconverrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* SIMPLE TASKS                      ");
        if( !kmeanssimpleerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* OTHER PROPERTIES                  ");
        if( !kmeansothererrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("* RESTARTS PROPERTIES               ");
        if( !kmeansrestartserrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
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
ae_bool _pexec_testclustering(ae_bool silent, ae_state *_state)
{
    return testclustering(silent, _state);
}


/*************************************************************************
Basic agglomerative hierarchical clustering tests:
returns True on failure, False on success.

Basic tests study algorithm behavior on simple,  hand-made  datasets  with
small number of points (1..10).
*************************************************************************/
static ae_bool testclusteringunit_basicahctests(ae_state *_state)
{
    ae_frame _frame_block;
    clusterizerstate s;
    ahcreport rep;
    ae_matrix xy;
    ae_matrix d;
    ae_matrix c;
    ae_bool berr;
    ae_int_t ahcalgo;
    ae_int_t i;
    ae_int_t j;
    ae_int_t npoints;
    ae_int_t k;
    ae_vector cidx;
    ae_vector cz;
    ae_vector cidx2;
    ae_vector cz2;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _clusterizerstate_init(&s, _state);
    _ahcreport_init(&rep, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&d, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_vector_init(&cidx, 0, DT_INT, _state);
    ae_vector_init(&cz, 0, DT_INT, _state);
    ae_vector_init(&cidx2, 0, DT_INT, _state);
    ae_vector_init(&cz2, 0, DT_INT, _state);

    result = ae_true;
    
    /*
     * Test on empty problem
     */
    clusterizercreate(&s, _state);
    clusterizerrunahc(&s, &rep, _state);
    if( rep.npoints!=0 )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Test on problem with one point
     */
    ae_matrix_set_length(&xy, 1, 2, _state);
    xy.ptr.pp_double[0][0] = ae_randomreal(_state);
    xy.ptr.pp_double[0][1] = ae_randomreal(_state);
    clusterizercreate(&s, _state);
    clusterizersetpoints(&s, &xy, 1, 2, 0, _state);
    clusterizerrunahc(&s, &rep, _state);
    if( rep.npoints!=1 )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Test on problem with two points
     */
    ae_matrix_set_length(&xy, 2, 2, _state);
    xy.ptr.pp_double[0][0] = ae_randomreal(_state);
    xy.ptr.pp_double[0][1] = ae_randomreal(_state);
    xy.ptr.pp_double[1][0] = ae_randomreal(_state);
    xy.ptr.pp_double[1][1] = ae_randomreal(_state);
    clusterizercreate(&s, _state);
    clusterizersetpoints(&s, &xy, 2, 2, 0, _state);
    clusterizerrunahc(&s, &rep, _state);
    if( (rep.npoints!=2||rep.z.rows!=1)||rep.z.cols!=2 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( rep.z.ptr.pp_int[0][0]!=0||rep.z.ptr.pp_int[0][1]!=1 )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Test on specially designed problem which should have
     * following dendrogram:
     *
     *   ------
     *   |    |
     * ----  ----
     * |  |  |  |
     * 0  1  2  3
     *
     * ...with first merge performed on 0 and 1, second merge
     * performed on 2 and 3. Complete linkage is used.
     *
     * Additionally we test ClusterizerSeparatedByDist() on this
     * problem for different distances. Test is performed by
     * comparing function result with ClusterizerGetKClusters()
     * for known K.
     */
    ae_matrix_set_length(&xy, 4, 1, _state);
    xy.ptr.pp_double[0][0] = 0.0;
    xy.ptr.pp_double[1][0] = 1.0;
    xy.ptr.pp_double[2][0] = 3.0;
    xy.ptr.pp_double[3][0] = 4.1;
    clusterizercreate(&s, _state);
    clusterizersetpoints(&s, &xy, 4, 1, 0, _state);
    clusterizersetahcalgo(&s, 0, _state);
    clusterizerrunahc(&s, &rep, _state);
    if( (((rep.npoints!=4||rep.z.rows!=3)||rep.z.cols!=2)||rep.pz.rows!=3)||rep.pz.cols!=2 )
    {
        ae_frame_leave(_state);
        return result;
    }
    berr = ae_false;
    berr = (berr||rep.z.ptr.pp_int[0][0]!=0)||rep.z.ptr.pp_int[0][1]!=1;
    berr = (berr||rep.z.ptr.pp_int[1][0]!=2)||rep.z.ptr.pp_int[1][1]!=3;
    berr = (berr||rep.z.ptr.pp_int[2][0]!=4)||rep.z.ptr.pp_int[2][1]!=5;
    berr = (((berr||rep.p.ptr.p_int[0]!=0)||rep.p.ptr.p_int[1]!=1)||rep.p.ptr.p_int[2]!=2)||rep.p.ptr.p_int[3]!=3;
    berr = (berr||rep.pz.ptr.pp_int[0][0]!=0)||rep.pz.ptr.pp_int[0][1]!=1;
    berr = (berr||rep.pz.ptr.pp_int[1][0]!=2)||rep.pz.ptr.pp_int[1][1]!=3;
    berr = (berr||rep.pz.ptr.pp_int[2][0]!=4)||rep.pz.ptr.pp_int[2][1]!=5;
    berr = (((berr||rep.pm.ptr.pp_int[0][0]!=0)||rep.pm.ptr.pp_int[0][1]!=0)||rep.pm.ptr.pp_int[0][2]!=1)||rep.pm.ptr.pp_int[0][3]!=1;
    berr = (((berr||rep.pm.ptr.pp_int[1][0]!=2)||rep.pm.ptr.pp_int[1][1]!=2)||rep.pm.ptr.pp_int[1][2]!=3)||rep.pm.ptr.pp_int[1][3]!=3;
    berr = (((berr||rep.pm.ptr.pp_int[2][0]!=0)||rep.pm.ptr.pp_int[2][1]!=1)||rep.pm.ptr.pp_int[2][2]!=2)||rep.pm.ptr.pp_int[2][3]!=3;
    if( berr )
    {
        ae_frame_leave(_state);
        return result;
    }
    clusterizerseparatedbydist(&rep, 0.5, &k, &cidx, &cz, _state);
    clusterizergetkclusters(&rep, 4, &cidx2, &cz2, _state);
    if( k!=4 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( ((cidx.ptr.p_int[0]!=cidx2.ptr.p_int[0]||cidx.ptr.p_int[1]!=cidx2.ptr.p_int[1])||cidx.ptr.p_int[2]!=cidx2.ptr.p_int[2])||cidx.ptr.p_int[3]!=cidx2.ptr.p_int[3] )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( ((cz.ptr.p_int[0]!=cz2.ptr.p_int[0]||cz.ptr.p_int[1]!=cz2.ptr.p_int[1])||cz.ptr.p_int[2]!=cz2.ptr.p_int[2])||cz.ptr.p_int[3]!=cz2.ptr.p_int[3] )
    {
        ae_frame_leave(_state);
        return result;
    }
    clusterizerseparatedbydist(&rep, 1.05, &k, &cidx, &cz, _state);
    clusterizergetkclusters(&rep, 3, &cidx2, &cz2, _state);
    if( k!=3 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( ((cidx.ptr.p_int[0]!=cidx2.ptr.p_int[0]||cidx.ptr.p_int[1]!=cidx2.ptr.p_int[1])||cidx.ptr.p_int[2]!=cidx2.ptr.p_int[2])||cidx.ptr.p_int[3]!=cidx2.ptr.p_int[3] )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( (cz.ptr.p_int[0]!=cz2.ptr.p_int[0]||cz.ptr.p_int[1]!=cz2.ptr.p_int[1])||cz.ptr.p_int[2]!=cz2.ptr.p_int[2] )
    {
        ae_frame_leave(_state);
        return result;
    }
    clusterizerseparatedbydist(&rep, 1.15, &k, &cidx, &cz, _state);
    clusterizergetkclusters(&rep, 2, &cidx2, &cz2, _state);
    if( k!=2 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( ((cidx.ptr.p_int[0]!=cidx2.ptr.p_int[0]||cidx.ptr.p_int[1]!=cidx2.ptr.p_int[1])||cidx.ptr.p_int[2]!=cidx2.ptr.p_int[2])||cidx.ptr.p_int[3]!=cidx2.ptr.p_int[3] )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( cz.ptr.p_int[0]!=cz2.ptr.p_int[0]||cz.ptr.p_int[1]!=cz2.ptr.p_int[1] )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Test on specially designed problem with Pearson distance
     * which should have following dendrogram:
     *
     *   ------
     *   |    |
     * ----  ----
     * |  |  |  |
     * 0  1  2  3
     *
     * This problem is used to test ClusterizerSeparatedByDist().
     * The test is performed by comparing function result with
     * ClusterizerGetKClusters() for known K.
     *
     * NOTE:
     * * corr(a0,a1) = 0.866
     * * corr(a2,a3) = 0.990
     * * corr(a0/a1, a2/a3)<=0.5
     */
    ae_matrix_set_length(&xy, 4, 3, _state);
    xy.ptr.pp_double[0][0] = 0.3;
    xy.ptr.pp_double[0][1] = 0.5;
    xy.ptr.pp_double[0][2] = 0.3;
    xy.ptr.pp_double[1][0] = 0.3;
    xy.ptr.pp_double[1][1] = 0.5;
    xy.ptr.pp_double[1][2] = 0.4;
    xy.ptr.pp_double[2][0] = 0.1;
    xy.ptr.pp_double[2][1] = 0.5;
    xy.ptr.pp_double[2][2] = 0.9;
    xy.ptr.pp_double[3][0] = 0.1;
    xy.ptr.pp_double[3][1] = 0.4;
    xy.ptr.pp_double[3][2] = 0.9;
    clusterizercreate(&s, _state);
    clusterizersetpoints(&s, &xy, 4, 3, 10, _state);
    clusterizersetahcalgo(&s, 1, _state);
    clusterizerrunahc(&s, &rep, _state);
    clusterizerseparatedbycorr(&rep, 0.999, &k, &cidx, &cz, _state);
    clusterizergetkclusters(&rep, 4, &cidx2, &cz2, _state);
    if( k!=4 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( ((cidx.ptr.p_int[0]!=cidx2.ptr.p_int[0]||cidx.ptr.p_int[1]!=cidx2.ptr.p_int[1])||cidx.ptr.p_int[2]!=cidx2.ptr.p_int[2])||cidx.ptr.p_int[3]!=cidx2.ptr.p_int[3] )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( ((cz.ptr.p_int[0]!=cz2.ptr.p_int[0]||cz.ptr.p_int[1]!=cz2.ptr.p_int[1])||cz.ptr.p_int[2]!=cz2.ptr.p_int[2])||cz.ptr.p_int[3]!=cz2.ptr.p_int[3] )
    {
        ae_frame_leave(_state);
        return result;
    }
    clusterizerseparatedbycorr(&rep, 0.900, &k, &cidx, &cz, _state);
    clusterizergetkclusters(&rep, 3, &cidx2, &cz2, _state);
    if( k!=3 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( ((cidx.ptr.p_int[0]!=cidx2.ptr.p_int[0]||cidx.ptr.p_int[1]!=cidx2.ptr.p_int[1])||cidx.ptr.p_int[2]!=cidx2.ptr.p_int[2])||cidx.ptr.p_int[3]!=cidx2.ptr.p_int[3] )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( (cz.ptr.p_int[0]!=cz2.ptr.p_int[0]||cz.ptr.p_int[1]!=cz2.ptr.p_int[1])||cz.ptr.p_int[2]!=cz2.ptr.p_int[2] )
    {
        ae_frame_leave(_state);
        return result;
    }
    clusterizerseparatedbycorr(&rep, 0.600, &k, &cidx, &cz, _state);
    clusterizergetkclusters(&rep, 2, &cidx2, &cz2, _state);
    if( k!=2 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( ((cidx.ptr.p_int[0]!=cidx2.ptr.p_int[0]||cidx.ptr.p_int[1]!=cidx2.ptr.p_int[1])||cidx.ptr.p_int[2]!=cidx2.ptr.p_int[2])||cidx.ptr.p_int[3]!=cidx2.ptr.p_int[3] )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( cz.ptr.p_int[0]!=cz2.ptr.p_int[0]||cz.ptr.p_int[1]!=cz2.ptr.p_int[1] )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Single linkage vs. complete linkage
     */
    ae_matrix_set_length(&xy, 6, 1, _state);
    xy.ptr.pp_double[0][0] = 0.0;
    xy.ptr.pp_double[1][0] = 1.0;
    xy.ptr.pp_double[2][0] = 2.1;
    xy.ptr.pp_double[3][0] = 3.3;
    xy.ptr.pp_double[4][0] = 6.0;
    xy.ptr.pp_double[5][0] = 4.6;
    clusterizercreate(&s, _state);
    clusterizersetpoints(&s, &xy, 6, 1, 0, _state);
    clusterizersetahcalgo(&s, 0, _state);
    clusterizerrunahc(&s, &rep, _state);
    if( rep.npoints!=6||rep.p.cnt!=6 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( ((rep.z.rows!=5||rep.z.cols!=2)||rep.pz.rows!=5)||rep.pz.cols!=2 )
    {
        ae_frame_leave(_state);
        return result;
    }
    berr = ae_false;
    berr = berr||rep.p.ptr.p_int[0]!=2;
    berr = berr||rep.p.ptr.p_int[1]!=3;
    berr = berr||rep.p.ptr.p_int[2]!=4;
    berr = berr||rep.p.ptr.p_int[3]!=5;
    berr = berr||rep.p.ptr.p_int[4]!=0;
    berr = berr||rep.p.ptr.p_int[5]!=1;
    berr = (berr||rep.z.ptr.pp_int[0][0]!=0)||rep.z.ptr.pp_int[0][1]!=1;
    berr = (berr||rep.z.ptr.pp_int[1][0]!=2)||rep.z.ptr.pp_int[1][1]!=3;
    berr = (berr||rep.z.ptr.pp_int[2][0]!=4)||rep.z.ptr.pp_int[2][1]!=5;
    berr = (berr||rep.z.ptr.pp_int[3][0]!=6)||rep.z.ptr.pp_int[3][1]!=7;
    berr = (berr||rep.z.ptr.pp_int[4][0]!=8)||rep.z.ptr.pp_int[4][1]!=9;
    berr = (berr||rep.pz.ptr.pp_int[0][0]!=2)||rep.pz.ptr.pp_int[0][1]!=3;
    berr = (berr||rep.pz.ptr.pp_int[1][0]!=4)||rep.pz.ptr.pp_int[1][1]!=5;
    berr = (berr||rep.pz.ptr.pp_int[2][0]!=0)||rep.pz.ptr.pp_int[2][1]!=1;
    berr = (berr||rep.pz.ptr.pp_int[3][0]!=6)||rep.pz.ptr.pp_int[3][1]!=7;
    berr = (berr||rep.pz.ptr.pp_int[4][0]!=8)||rep.pz.ptr.pp_int[4][1]!=9;
    if( berr )
    {
        ae_frame_leave(_state);
        return result;
    }
    clusterizersetahcalgo(&s, 1, _state);
    clusterizerrunahc(&s, &rep, _state);
    if( (rep.npoints!=6||rep.z.rows!=5)||rep.z.cols!=2 )
    {
        ae_frame_leave(_state);
        return result;
    }
    berr = ae_false;
    berr = (berr||rep.z.ptr.pp_int[0][0]!=0)||rep.z.ptr.pp_int[0][1]!=1;
    berr = (berr||rep.z.ptr.pp_int[1][0]!=2)||rep.z.ptr.pp_int[1][1]!=6;
    berr = (berr||rep.z.ptr.pp_int[2][0]!=3)||rep.z.ptr.pp_int[2][1]!=7;
    berr = (berr||rep.z.ptr.pp_int[3][0]!=5)||rep.z.ptr.pp_int[3][1]!=8;
    berr = (berr||rep.z.ptr.pp_int[4][0]!=4)||rep.z.ptr.pp_int[4][1]!=9;
    if( berr )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Test which differentiates complete linkage and average linkage from
     * single linkage:
     * * we have cluster C0={(-0.5), (0)},
     *   cluster C1={(19.0), (20.0), (21.0), (22.0), (23.0)},
     *   and point P between them - (10.0)
     * * we try three different strategies - single linkage, complete
     *   linkage, average linkage.
     * * any strategy will merge C0 first, then merge points of C1,
     *   and then merge P with C0 or C1 (depending on linkage type)
     * * we test that:
     *   a) C0 is merged first
     *   b) after 5 merges (including merge of C0), P is merged with C0 or C1
     *   c) P is merged with C1 when we have single linkage, with C0 otherwise
     */
    ae_matrix_set_length(&xy, 8, 1, _state);
    xy.ptr.pp_double[0][0] = -0.5;
    xy.ptr.pp_double[1][0] = 0.0;
    xy.ptr.pp_double[2][0] = 10.0;
    xy.ptr.pp_double[3][0] = 19.0;
    xy.ptr.pp_double[4][0] = 20.0;
    xy.ptr.pp_double[5][0] = 21.0;
    xy.ptr.pp_double[6][0] = 22.0;
    xy.ptr.pp_double[7][0] = 23.0;
    clusterizercreate(&s, _state);
    clusterizersetpoints(&s, &xy, 8, 1, 0, _state);
    for(ahcalgo=0; ahcalgo<=2; ahcalgo++)
    {
        clusterizersetahcalgo(&s, ahcalgo, _state);
        clusterizerrunahc(&s, &rep, _state);
        if( (rep.npoints!=8||rep.z.rows!=7)||rep.z.cols!=2 )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( rep.z.ptr.pp_int[0][0]!=0||rep.z.ptr.pp_int[0][1]!=1 )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( rep.z.ptr.pp_int[5][0]!=2&&rep.z.ptr.pp_int[5][1]!=2 )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( rep.z.ptr.pp_int[5][0]!=2&&rep.z.ptr.pp_int[5][1]!=2 )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( (ahcalgo==0||ahcalgo==2)&&(rep.z.ptr.pp_int[5][0]!=8&&rep.z.ptr.pp_int[5][1]!=8) )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( ahcalgo==1&&(rep.z.ptr.pp_int[5][0]==8||rep.z.ptr.pp_int[5][1]==8) )
        {
            ae_frame_leave(_state);
            return result;
        }
    }
    
    /*
     * Test which differentiates single linkage and average linkage from
     * complete linkage:
     * * we have cluster C0={(-2.5), (-2.0)},
     *   cluster C1={(19.0), (20.0), (21.0), (22.0), (23.0)},
     *   and point P between them - (10.0)
     * * we try three different strategies - single linkage, complete
     *   linkage, average linkage.
     * * any strategy will merge C0 first, then merge points of C1,
     *   and then merge P with C0 or C1 (depending on linkage type)
     * * we test that:
     *   a) C0 is merged first
     *   b) after 5 merges (including merge of C0), P is merged with C0 or C1
     *   c) P is merged with C0 when we have complete linkage, with C1 otherwise
     */
    ae_matrix_set_length(&xy, 8, 1, _state);
    xy.ptr.pp_double[0][0] = -2.5;
    xy.ptr.pp_double[1][0] = -2.0;
    xy.ptr.pp_double[2][0] = 10.0;
    xy.ptr.pp_double[3][0] = 19.0;
    xy.ptr.pp_double[4][0] = 20.0;
    xy.ptr.pp_double[5][0] = 21.0;
    xy.ptr.pp_double[6][0] = 22.0;
    xy.ptr.pp_double[7][0] = 23.0;
    clusterizercreate(&s, _state);
    clusterizersetpoints(&s, &xy, 8, 1, 0, _state);
    for(ahcalgo=0; ahcalgo<=2; ahcalgo++)
    {
        clusterizersetahcalgo(&s, ahcalgo, _state);
        clusterizerrunahc(&s, &rep, _state);
        if( (rep.npoints!=8||rep.z.rows!=7)||rep.z.cols!=2 )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( rep.z.ptr.pp_int[0][0]!=0||rep.z.ptr.pp_int[0][1]!=1 )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( rep.z.ptr.pp_int[5][0]!=2&&rep.z.ptr.pp_int[5][1]!=2 )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( rep.z.ptr.pp_int[5][0]!=2&&rep.z.ptr.pp_int[5][1]!=2 )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( ahcalgo==0&&(rep.z.ptr.pp_int[5][0]!=8&&rep.z.ptr.pp_int[5][1]!=8) )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( (ahcalgo==1||ahcalgo==2)&&(rep.z.ptr.pp_int[5][0]==8||rep.z.ptr.pp_int[5][1]==8) )
        {
            ae_frame_leave(_state);
            return result;
        }
    }
    
    /*
     * Test which differentiates weighred average linkage from unweighted average linkage:
     * * we have cluster C0={(0.0), (1.5), (2.5)},
     *   cluster C1={(7.5), (7.99)},
     *   and point P between them - (4.5)
     * * we try two different strategies - weighted average linkage and unweighted average linkage
     * * any strategy will merge C1 first, then merge points of C0,
     *   and then merge P with C0 or C1 (depending on linkage type)
     * * we test that:
     *   a) C1 is merged first, C0 is merged after that
     *   b) after first 3 merges P is merged with C0 or C1
     *   c) P is merged with C1 when we have weighted average linkage, with C0 otherwise
     */
    ae_matrix_set_length(&xy, 6, 1, _state);
    xy.ptr.pp_double[0][0] = 0.0;
    xy.ptr.pp_double[1][0] = 1.5;
    xy.ptr.pp_double[2][0] = 2.5;
    xy.ptr.pp_double[3][0] = 4.5;
    xy.ptr.pp_double[4][0] = 7.5;
    xy.ptr.pp_double[5][0] = 7.99;
    clusterizercreate(&s, _state);
    clusterizersetpoints(&s, &xy, 6, 1, 0, _state);
    for(ahcalgo=2; ahcalgo<=3; ahcalgo++)
    {
        clusterizersetahcalgo(&s, ahcalgo, _state);
        clusterizerrunahc(&s, &rep, _state);
        if( (rep.npoints!=6||rep.z.rows!=5)||rep.z.cols!=2 )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( rep.z.ptr.pp_int[0][0]!=4||rep.z.ptr.pp_int[0][1]!=5 )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( rep.z.ptr.pp_int[1][0]!=1||rep.z.ptr.pp_int[1][1]!=2 )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( rep.z.ptr.pp_int[2][0]!=0||rep.z.ptr.pp_int[2][1]!=7 )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( rep.z.ptr.pp_int[3][0]!=3 )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( ahcalgo==2&&rep.z.ptr.pp_int[3][1]!=8 )
        {
            ae_frame_leave(_state);
            return result;
        }
        if( ahcalgo==3&&rep.z.ptr.pp_int[3][1]!=6 )
        {
            ae_frame_leave(_state);
            return result;
        }
    }
    
    /*
     * Test which checks correctness of Ward's method on very basic problem
     */
    ae_matrix_set_length(&xy, 4, 1, _state);
    xy.ptr.pp_double[0][0] = 0.0;
    xy.ptr.pp_double[1][0] = 1.0;
    xy.ptr.pp_double[2][0] = 3.1;
    xy.ptr.pp_double[3][0] = 4.0;
    clusterizercreate(&s, _state);
    clusterizersetpoints(&s, &xy, xy.rows, xy.cols, 2, _state);
    clusterizergetdistances(&xy, xy.rows, xy.cols, 2, &d, _state);
    clusterizersetahcalgo(&s, 4, _state);
    clusterizerrunahc(&s, &rep, _state);
    if( testclusteringunit_errorsinmerges(&d, &xy, xy.rows, xy.cols, &rep, 4, _state) )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * One more Ward's test
     */
    ae_matrix_set_length(&xy, 8, 2, _state);
    xy.ptr.pp_double[0][0] = 0.4700566262;
    xy.ptr.pp_double[0][1] = 0.4565938448;
    xy.ptr.pp_double[1][0] = 0.2394499506;
    xy.ptr.pp_double[1][1] = 0.1750209592;
    xy.ptr.pp_double[2][0] = 0.6518417019;
    xy.ptr.pp_double[2][1] = 0.6151370746;
    xy.ptr.pp_double[3][0] = 0.9863942841;
    xy.ptr.pp_double[3][1] = 0.7855012189;
    xy.ptr.pp_double[4][0] = 0.1517812919;
    xy.ptr.pp_double[4][1] = 0.2600174758;
    xy.ptr.pp_double[5][0] = 0.7840203638;
    xy.ptr.pp_double[5][1] = 0.9023597604;
    xy.ptr.pp_double[6][0] = 0.2604194835;
    xy.ptr.pp_double[6][1] = 0.9792704661;
    xy.ptr.pp_double[7][0] = 0.6353096042;
    xy.ptr.pp_double[7][1] = 0.8252606906;
    clusterizercreate(&s, _state);
    clusterizersetpoints(&s, &xy, xy.rows, xy.cols, 2, _state);
    clusterizergetdistances(&xy, xy.rows, xy.cols, 2, &d, _state);
    clusterizersetahcalgo(&s, 4, _state);
    clusterizerrunahc(&s, &rep, _state);
    if( rep.z.ptr.pp_int[0][0]!=1||rep.z.ptr.pp_int[0][1]!=4 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( rep.z.ptr.pp_int[1][0]!=5||rep.z.ptr.pp_int[1][1]!=7 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( rep.z.ptr.pp_int[2][0]!=0||rep.z.ptr.pp_int[2][1]!=2 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( rep.z.ptr.pp_int[3][0]!=3||rep.z.ptr.pp_int[3][1]!=9 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( rep.z.ptr.pp_int[4][0]!=10||rep.z.ptr.pp_int[4][1]!=11 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( rep.z.ptr.pp_int[5][0]!=6||rep.z.ptr.pp_int[5][1]!=12 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( rep.z.ptr.pp_int[6][0]!=8||rep.z.ptr.pp_int[6][1]!=13 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( testclusteringunit_errorsinmerges(&d, &xy, xy.rows, xy.cols, &rep, 4, _state) )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Ability to solve problems with zero distance matrix
     */
    npoints = 20;
    ae_matrix_set_length(&d, npoints, npoints, _state);
    for(i=0; i<=npoints-1; i++)
    {
        for(j=0; j<=npoints-1; j++)
        {
            d.ptr.pp_double[i][j] = 0.0;
        }
    }
    for(ahcalgo=0; ahcalgo<=4; ahcalgo++)
    {
        clusterizercreate(&s, _state);
        clusterizersetdistances(&s, &d, npoints, ae_true, _state);
        clusterizersetahcalgo(&s, ahcalgo, _state);
        clusterizerrunahc(&s, &rep, _state);
        if( (rep.npoints!=npoints||rep.z.rows!=npoints-1)||rep.z.cols!=2 )
        {
            ae_frame_leave(_state);
            return result;
        }
    }
    
    /*
     * Test GetKClusters()
     */
    ae_matrix_set_length(&xy, 8, 1, _state);
    xy.ptr.pp_double[0][0] = -2.5;
    xy.ptr.pp_double[1][0] = -2.0;
    xy.ptr.pp_double[2][0] = 10.0;
    xy.ptr.pp_double[3][0] = 19.0;
    xy.ptr.pp_double[4][0] = 20.0;
    xy.ptr.pp_double[5][0] = 21.0;
    xy.ptr.pp_double[6][0] = 22.0;
    xy.ptr.pp_double[7][0] = 23.0;
    clusterizercreate(&s, _state);
    clusterizersetpoints(&s, &xy, 8, 1, 0, _state);
    clusterizersetahcalgo(&s, 0, _state);
    clusterizerrunahc(&s, &rep, _state);
    clusterizergetkclusters(&rep, 3, &cidx, &cz, _state);
    if( ((((((cidx.ptr.p_int[0]!=1||cidx.ptr.p_int[1]!=1)||cidx.ptr.p_int[2]!=0)||cidx.ptr.p_int[3]!=2)||cidx.ptr.p_int[4]!=2)||cidx.ptr.p_int[5]!=2)||cidx.ptr.p_int[6]!=2)||cidx.ptr.p_int[7]!=2 )
    {
        ae_frame_leave(_state);
        return result;
    }
    if( (cz.ptr.p_int[0]!=2||cz.ptr.p_int[1]!=8)||cz.ptr.p_int[2]!=12 )
    {
        ae_frame_leave(_state);
        return result;
    }
    
    /*
     * Test is done
     */
    result = ae_false;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Advanced  agglomerative  hierarchical  clustering  tests : returns True on
failure, False on success.

Advanced testing subroutine perform several automatically generated tests.
*************************************************************************/
static ae_bool testclusteringunit_advancedahctests(ae_state *_state)
{
    ae_frame _frame_block;
    clusterizerstate s;
    ahcreport rep;
    ae_matrix xy;
    ae_matrix dm;
    ae_matrix dm2;
    ae_vector idx;
    ae_vector disttypes;
    ae_vector x0;
    ae_vector x1;
    ae_int_t d;
    ae_int_t n;
    ae_int_t npoints;
    ae_int_t ahcalgo;
    ae_int_t disttype;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    double v;
    ae_int_t t;
    ae_int_t euclidean;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _clusterizerstate_init(&s, _state);
    _ahcreport_init(&rep, _state);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&dm, 0, 0, DT_REAL, _state);
    ae_matrix_init(&dm2, 0, 0, DT_REAL, _state);
    ae_vector_init(&idx, 0, DT_INT, _state);
    ae_vector_init(&disttypes, 0, DT_INT, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);

    result = ae_false;
    euclidean = 2;
    
    /*
     * Test on D-dimensional problem:
     * * D = 2...5
     * * D clusters, each has N points;
     *   centers are located at x=(0 ... 1 ... 0);
     *   cluster radii are approximately 0.1
     * * single/complete/unweighted_average/weighted_average linkage/Ward's method are tested
     * * Euclidean distance is used, either:
     *   a) one given by distance matrix (ClusterizerSetDistances)
     *   b) one calculated from dataset (ClusterizerSetPoints)
     * * we have N*D points, and N*D-1 merges in total
     * * points are randomly rearranged after generation
     *
     * For all kinds of linkage we perform following test:
     * * for each point we remember index of its cluster
     *   (one which is determined during dataset generation)
     * * we clusterize points with ALGLIB capabilities
     * * we scan Rep.Z and perform first D*(N-1) merges
     * * for each merge we check that it merges points
     *   from same cluster;
     *
     * Additonally, we call ErrorsInMerges(). See function comments 
     * for more information about specific tests performed. This function
     * allows us to check that clusters are built exactly as specified by
     * definition of the clustering algorithm.
     */
    for(d=2; d<=5; d++)
    {
        for(ahcalgo=0; ahcalgo<=4; ahcalgo++)
        {
            n = ae_round(ae_pow((double)(3), (double)(ae_randominteger(3, _state)), _state), _state);
            npoints = d*n;
            
            /*
             * 1. generate dataset.
             * 2. fill Idx (array of cluster indexes):
             *    * first N*D elements store cluster indexes
             *    * next D*(N-1) elements are filled during merges
             * 3. build distance matrix DM
             */
            ae_matrix_set_length(&xy, n*d, d, _state);
            ae_vector_set_length(&idx, n*d+d*(n-1), _state);
            for(i=0; i<=n*d-1; i++)
            {
                for(j=0; j<=d-1; j++)
                {
                    xy.ptr.pp_double[i][j] = 0.2*ae_randomreal(_state)-0.1;
                }
                xy.ptr.pp_double[i][i%d] = xy.ptr.pp_double[i][i%d]+1.0;
                idx.ptr.p_int[i] = i%d;
            }
            for(i=0; i<=n*d-1; i++)
            {
                k = ae_randominteger(n*d, _state);
                if( k!=i )
                {
                    for(j=0; j<=d-1; j++)
                    {
                        v = xy.ptr.pp_double[i][j];
                        xy.ptr.pp_double[i][j] = xy.ptr.pp_double[k][j];
                        xy.ptr.pp_double[k][j] = v;
                    }
                    t = idx.ptr.p_int[k];
                    idx.ptr.p_int[k] = idx.ptr.p_int[i];
                    idx.ptr.p_int[i] = t;
                }
            }
            ae_matrix_set_length(&dm, npoints, npoints, _state);
            ae_vector_set_length(&x0, d, _state);
            ae_vector_set_length(&x1, d, _state);
            for(i=0; i<=npoints-1; i++)
            {
                for(j=0; j<=npoints-1; j++)
                {
                    ae_v_move(&x0.ptr.p_double[0], 1, &xy.ptr.pp_double[i][0], 1, ae_v_len(0,d-1));
                    ae_v_move(&x1.ptr.p_double[0], 1, &xy.ptr.pp_double[j][0], 1, ae_v_len(0,d-1));
                    dm.ptr.pp_double[i][j] = testclusteringunit_distfunc(&x0, &x1, d, euclidean, _state);
                }
            }
            
            /*
             * Clusterize with SetPoints()
             */
            clusterizercreate(&s, _state);
            clusterizersetpoints(&s, &xy, n*d, d, euclidean, _state);
            clusterizersetahcalgo(&s, ahcalgo, _state);
            clusterizerrunahc(&s, &rep, _state);
            
            /*
             * Tests:
             * * replay first D*(N-1) merges; these merges should take place
             *   within clusters, intercluster merges will be performed at the
             *   last stages of the processing.
             * * test with ErrorsInMerges()
             */
            if( rep.npoints!=npoints )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            for(i=0; i<=d*(n-1)-1; i++)
            {
                
                /*
                 * Check correctness of I-th row of Z
                 */
                if( (rep.z.ptr.pp_int[i][0]<0||rep.z.ptr.pp_int[i][0]>=rep.z.ptr.pp_int[i][1])||rep.z.ptr.pp_int[i][1]>=d*n+i )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
                
                /*
                 * Check that merge is performed within cluster
                 */
                if( idx.ptr.p_int[rep.z.ptr.pp_int[i][0]]!=idx.ptr.p_int[rep.z.ptr.pp_int[i][1]] )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
                
                /*
                 * Write new entry of Idx.
                 * Both points from the same cluster, so result of the merge
                 * belongs to the same cluster
                 */
                idx.ptr.p_int[n*d+i] = idx.ptr.p_int[rep.z.ptr.pp_int[i][1]];
            }
            if( ((ahcalgo==0||ahcalgo==1)||ahcalgo==2)||ahcalgo==4 )
            {
                if( testclusteringunit_errorsinmerges(&dm, &xy, d*n, d, &rep, ahcalgo, _state) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
            
            /*
             * Clusterize one more time, now with distance matrix
             */
            clusterizercreate(&s, _state);
            clusterizersetdistances(&s, &dm, n*d, ae_fp_greater(ae_randomreal(_state),0.5), _state);
            clusterizersetahcalgo(&s, ahcalgo, _state);
            clusterizerrunahc(&s, &rep, _state);
            
            /*
             * Tests:
             * * replay first D*(N-1) merges; these merges should take place
             *   within clusters, intercluster merges will be performed at the
             *   last stages of the processing.
             * * test with ErrorsInMerges()
             */
            if( rep.npoints!=npoints )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            for(i=0; i<=d*(n-1)-1; i++)
            {
                
                /*
                 * Check correctness of I-th row of Z
                 */
                if( (rep.z.ptr.pp_int[i][0]<0||rep.z.ptr.pp_int[i][0]>=rep.z.ptr.pp_int[i][1])||rep.z.ptr.pp_int[i][1]>=d*n+i )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
                
                /*
                 * Check that merge is performed within cluster
                 */
                if( idx.ptr.p_int[rep.z.ptr.pp_int[i][0]]!=idx.ptr.p_int[rep.z.ptr.pp_int[i][1]] )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
                
                /*
                 * Write new entry of Idx.
                 * Both points from the same cluster, so result of the merge
                 * belongs to the same cluster
                 */
                idx.ptr.p_int[n*d+i] = idx.ptr.p_int[rep.z.ptr.pp_int[i][1]];
            }
            if( ((ahcalgo==0||ahcalgo==1)||ahcalgo==2)||ahcalgo==4 )
            {
                if( testclusteringunit_errorsinmerges(&dm, &xy, d*n, d, &rep, ahcalgo, _state) )
                {
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
    }
    
    /*
     * Test on random D-dimensional problem:
     * * D = 2...5
     * * N=1..16 random points from unit hypercube
     * * single/complete/unweighted_average linkage/Ward's method are tested
     * * different distance functions are tested
     * * we call ErrorsInMerges() and we check distance matrix
     *   calculated by unit test against one returned by GetDistances()
     */
    ae_vector_set_length(&disttypes, 9, _state);
    disttypes.ptr.p_int[0] = 0;
    disttypes.ptr.p_int[1] = 1;
    disttypes.ptr.p_int[2] = 2;
    disttypes.ptr.p_int[3] = 10;
    disttypes.ptr.p_int[4] = 11;
    disttypes.ptr.p_int[5] = 12;
    disttypes.ptr.p_int[6] = 13;
    disttypes.ptr.p_int[7] = 20;
    disttypes.ptr.p_int[8] = 21;
    for(disttype=0; disttype<=disttypes.cnt-1; disttype++)
    {
        for(ahcalgo=0; ahcalgo<=4; ahcalgo++)
        {
            if( ahcalgo==3 )
            {
                continue;
            }
            if( ahcalgo==4&&disttype!=2 )
            {
                continue;
            }
            npoints = ae_round(ae_pow((double)(2), (double)(ae_randominteger(5, _state)), _state), _state);
            d = 2+ae_randominteger(4, _state);
            
            /*
             * Generate dataset and distance matrix
             */
            ae_matrix_set_length(&xy, npoints, d, _state);
            for(i=0; i<=npoints-1; i++)
            {
                for(j=0; j<=d-1; j++)
                {
                    xy.ptr.pp_double[i][j] = ae_randomreal(_state);
                }
            }
            ae_matrix_set_length(&dm, npoints, npoints, _state);
            ae_vector_set_length(&x0, d, _state);
            ae_vector_set_length(&x1, d, _state);
            for(i=0; i<=npoints-1; i++)
            {
                for(j=0; j<=npoints-1; j++)
                {
                    ae_v_move(&x0.ptr.p_double[0], 1, &xy.ptr.pp_double[i][0], 1, ae_v_len(0,d-1));
                    ae_v_move(&x1.ptr.p_double[0], 1, &xy.ptr.pp_double[j][0], 1, ae_v_len(0,d-1));
                    dm.ptr.pp_double[i][j] = testclusteringunit_distfunc(&x0, &x1, d, disttypes.ptr.p_int[disttype], _state);
                }
            }
            
            /*
             * Clusterize
             */
            clusterizercreate(&s, _state);
            clusterizersetpoints(&s, &xy, npoints, d, disttypes.ptr.p_int[disttype], _state);
            clusterizersetahcalgo(&s, ahcalgo, _state);
            clusterizerrunahc(&s, &rep, _state);
            
            /*
             * Test with ErrorsInMerges()
             */
            if( testclusteringunit_errorsinmerges(&dm, &xy, npoints, d, &rep, ahcalgo, _state) )
            {
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
            
            /*
             * Test distance matrix
             */
            clusterizergetdistances(&xy, npoints, d, disttypes.ptr.p_int[disttype], &dm2, _state);
            for(i=0; i<=npoints-1; i++)
            {
                for(j=0; j<=npoints-1; j++)
                {
                    if( !ae_isfinite(dm2.ptr.pp_double[i][j], _state)||ae_fp_greater(ae_fabs(dm.ptr.pp_double[i][j]-dm2.ptr.pp_double[i][j], _state),1.0E5*ae_machineepsilon) )
                    {
                        result = ae_true;
                        ae_frame_leave(_state);
                        return result;
                    }
                }
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Simple test 1: ellipsoid in NF-dimensional space.
compare k-means centers with random centers
*************************************************************************/
static void testclusteringunit_kmeanssimpletest1(ae_int_t nvars,
     ae_int_t nc,
     ae_int_t passcount,
     ae_bool* converrors,
     ae_bool* othererrors,
     ae_bool* simpleerrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t npoints;
    ae_int_t majoraxis;
    ae_matrix xy;
    ae_vector tmp;
    double v;
    ae_int_t i;
    ae_int_t j;
    ae_int_t pass;
    ae_int_t restarts;
    double ekmeans;
    double erandom;
    double dclosest;
    ae_int_t cclosest;
    clusterizerstate s;
    kmeansreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    _clusterizerstate_init(&s, _state);
    _kmeansreport_init(&rep, _state);

    npoints = nc*100;
    restarts = 5;
    passcount = 10;
    ae_vector_set_length(&tmp, nvars-1+1, _state);
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * Fill
         */
        ae_matrix_set_length(&xy, npoints-1+1, nvars-1+1, _state);
        majoraxis = ae_randominteger(nvars, _state);
        for(i=0; i<=npoints-1; i++)
        {
            testclusteringunit_rsphere(&xy, nvars, i, _state);
            xy.ptr.pp_double[i][majoraxis] = nc*xy.ptr.pp_double[i][majoraxis];
        }
        
        /*
         * Test
         */
        clusterizercreate(&s, _state);
        clusterizersetpoints(&s, &xy, npoints, nvars, 2, _state);
        clusterizersetkmeanslimits(&s, restarts, 0, _state);
        clusterizerrunkmeans(&s, nc, &rep, _state);
        if( rep.terminationtype<=0 )
        {
            *converrors = ae_true;
            ae_frame_leave(_state);
            return;
        }
        
        /*
         * Test that XYC is correct mapping to cluster centers
         */
        for(i=0; i<=npoints-1; i++)
        {
            cclosest = -1;
            dclosest = ae_maxrealnumber;
            for(j=0; j<=nc-1; j++)
            {
                ae_v_move(&tmp.ptr.p_double[0], 1, &xy.ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
                ae_v_sub(&tmp.ptr.p_double[0], 1, &rep.c.ptr.pp_double[j][0], 1, ae_v_len(0,nvars-1));
                v = ae_v_dotproduct(&tmp.ptr.p_double[0], 1, &tmp.ptr.p_double[0], 1, ae_v_len(0,nvars-1));
                if( ae_fp_less(v,dclosest) )
                {
                    cclosest = j;
                    dclosest = v;
                }
            }
            if( cclosest!=rep.cidx.ptr.p_int[i] )
            {
                *othererrors = ae_true;
                ae_frame_leave(_state);
                return;
            }
        }
        
        /*
         * Use first NC rows of XY as random centers
         * (XY is totally random, so it is as good as any other choice).
         *
         * Compare potential functions.
         */
        ekmeans = (double)(0);
        for(i=0; i<=npoints-1; i++)
        {
            ae_v_move(&tmp.ptr.p_double[0], 1, &xy.ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
            ae_v_sub(&tmp.ptr.p_double[0], 1, &rep.c.ptr.pp_double[rep.cidx.ptr.p_int[i]][0], 1, ae_v_len(0,nvars-1));
            v = ae_v_dotproduct(&tmp.ptr.p_double[0], 1, &tmp.ptr.p_double[0], 1, ae_v_len(0,nvars-1));
            ekmeans = ekmeans+v;
        }
        erandom = (double)(0);
        for(i=0; i<=npoints-1; i++)
        {
            dclosest = ae_maxrealnumber;
            v = (double)(0);
            for(j=0; j<=nc-1; j++)
            {
                ae_v_move(&tmp.ptr.p_double[0], 1, &xy.ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
                ae_v_sub(&tmp.ptr.p_double[0], 1, &xy.ptr.pp_double[j][0], 1, ae_v_len(0,nvars-1));
                v = ae_v_dotproduct(&tmp.ptr.p_double[0], 1, &tmp.ptr.p_double[0], 1, ae_v_len(0,nvars-1));
                if( ae_fp_less(v,dclosest) )
                {
                    dclosest = v;
                }
            }
            erandom = erandom+v;
        }
        if( ae_fp_less(erandom,ekmeans) )
        {
            *simpleerrors = ae_true;
            ae_frame_leave(_state);
            return;
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This test perform several checks for special properties

On failure sets error flag, on success leaves it unchanged.
*************************************************************************/
static void testclusteringunit_kmeansspecialtests(ae_bool* othererrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t npoints;
    ae_int_t nfeatures;
    ae_int_t nclusters;
    ae_int_t initalgo;
    ae_matrix xy;
    ae_matrix c;
    ae_int_t idx0;
    ae_int_t idx1;
    ae_int_t idx2;
    ae_int_t i;
    ae_int_t j;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t separation;
    ae_vector xyc;
    ae_vector xycref;
    ae_vector xydist2;
    ae_vector xydist2ref;
    ae_vector energies;
    hqrndstate rs;
    clusterizerstate s;
    kmeansreport rep;
    ae_shared_pool bufferpool;
    apbuffers bufferseed;
    ae_vector pointslist;
    ae_vector featureslist;
    ae_vector clusterslist;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);
    ae_vector_init(&xyc, 0, DT_INT, _state);
    ae_vector_init(&xycref, 0, DT_INT, _state);
    ae_vector_init(&xydist2, 0, DT_REAL, _state);
    ae_vector_init(&xydist2ref, 0, DT_REAL, _state);
    ae_vector_init(&energies, 0, DT_REAL, _state);
    _hqrndstate_init(&rs, _state);
    _clusterizerstate_init(&s, _state);
    _kmeansreport_init(&rep, _state);
    ae_shared_pool_init(&bufferpool, _state);
    _apbuffers_init(&bufferseed, _state);
    ae_vector_init(&pointslist, 0, DT_INT, _state);
    ae_vector_init(&featureslist, 0, DT_INT, _state);
    ae_vector_init(&clusterslist, 0, DT_INT, _state);

    hqrndrandomize(&rs, _state);
    
    /*
     * Compare different initialization algorithms:
     * * dataset is K balls, chosen at random gaussian points, with
     *   radius equal to 2^(-Separation).
     * * we generate random sample, run k-means initialization algorithm
     *   and calculate mean energy for each initialization algorithm.
     *   In order to suppress Lloyd's iteration we use KmeansDbgNoIts
     *   debug flag.
     * * then, we compare mean energies; kmeans++ must be best one,
     *   random initialization must be worst one.
     */
    ae_vector_set_length(&energies, 4, _state);
    passcount = 1000;
    npoints = 100;
    nfeatures = 3;
    nclusters = 6;
    ae_matrix_set_length(&xy, npoints, nfeatures, _state);
    ae_matrix_set_length(&c, nclusters, nfeatures, _state);
    clusterizercreate(&s, _state);
    s.kmeansdbgnoits = ae_true;
    for(separation=2; separation<=5; separation++)
    {
        
        /*
         * Try different init algorithms
         */
        for(initalgo=1; initalgo<=3; initalgo++)
        {
            energies.ptr.p_double[initalgo] = 0.0;
            clusterizersetkmeansinit(&s, initalgo, _state);
            for(pass=1; pass<=passcount; pass++)
            {
                
                /*
                 * Generate centers of balls
                 */
                for(i=0; i<=nclusters-1; i++)
                {
                    for(j=0; j<=nfeatures-1; j++)
                    {
                        c.ptr.pp_double[i][j] = hqrndnormal(&rs, _state);
                    }
                }
                
                /*
                 * Generate points
                 */
                for(i=0; i<=npoints-1; i++)
                {
                    for(j=0; j<=nfeatures-1; j++)
                    {
                        xy.ptr.pp_double[i][j] = hqrndnormal(&rs, _state)*ae_pow((double)(2), (double)(-separation), _state)+c.ptr.pp_double[i%nclusters][j];
                    }
                }
                
                /*
                 * Run clusterization
                 */
                clusterizersetpoints(&s, &xy, npoints, nfeatures, 2, _state);
                clusterizerrunkmeans(&s, nclusters, &rep, _state);
                seterrorflag(othererrors, rep.terminationtype<=0, _state);
                energies.ptr.p_double[initalgo] = energies.ptr.p_double[initalgo]+rep.energy/passcount;
            }
        }
        
        /*
         * Compare
         */
        seterrorflag(othererrors, !ae_fp_less(energies.ptr.p_double[2],energies.ptr.p_double[1]), _state);
        seterrorflag(othererrors, !ae_fp_less(energies.ptr.p_double[3],energies.ptr.p_double[1]), _state);
    }
    
    /*
     * Test distance calculation algorithm
     */
    ae_vector_set_length(&pointslist, 6, _state);
    pointslist.ptr.p_int[0] = 1;
    pointslist.ptr.p_int[1] = 10;
    pointslist.ptr.p_int[2] = 32;
    pointslist.ptr.p_int[3] = 100;
    pointslist.ptr.p_int[4] = 512;
    pointslist.ptr.p_int[5] = 8000;
    ae_vector_set_length(&featureslist, 5, _state);
    featureslist.ptr.p_int[0] = 1;
    featureslist.ptr.p_int[1] = 5;
    featureslist.ptr.p_int[2] = 32;
    featureslist.ptr.p_int[3] = 50;
    featureslist.ptr.p_int[4] = 96;
    ae_vector_set_length(&clusterslist, 5, _state);
    clusterslist.ptr.p_int[0] = 1;
    clusterslist.ptr.p_int[1] = 5;
    clusterslist.ptr.p_int[2] = 32;
    clusterslist.ptr.p_int[3] = 50;
    clusterslist.ptr.p_int[4] = 96;
    ae_shared_pool_set_seed(&bufferpool, &bufferseed, sizeof(bufferseed), _apbuffers_init, _apbuffers_init_copy, _apbuffers_destroy, _state);
    for(idx0=0; idx0<=pointslist.cnt-1; idx0++)
    {
        for(idx1=0; idx1<=featureslist.cnt-1; idx1++)
        {
            for(idx2=0; idx2<=clusterslist.cnt-1; idx2++)
            {
                npoints = pointslist.ptr.p_int[idx0];
                nfeatures = featureslist.ptr.p_int[idx1];
                nclusters = clusterslist.ptr.p_int[idx2];
                ae_matrix_set_length(&xy, npoints, nfeatures, _state);
                for(i=0; i<=npoints-1; i++)
                {
                    for(j=0; j<=nfeatures-1; j++)
                    {
                        xy.ptr.pp_double[i][j] = hqrndnormal(&rs, _state);
                    }
                }
                ae_matrix_set_length(&c, nclusters, nfeatures, _state);
                for(i=0; i<=nclusters-1; i++)
                {
                    for(j=0; j<=nfeatures-1; j++)
                    {
                        c.ptr.pp_double[i][j] = hqrndnormal(&rs, _state);
                    }
                }
                ae_vector_set_length(&xyc, npoints, _state);
                ae_vector_set_length(&xycref, npoints, _state);
                ae_vector_set_length(&xydist2, npoints, _state);
                ae_vector_set_length(&xydist2ref, npoints, _state);
                
                /*
                 * Test
                 */
                kmeansupdatedistances(&xy, 0, npoints, nfeatures, &c, 0, nclusters, &xyc, &xydist2, &bufferpool, _state);
                testclusteringunit_kmeansreferenceupdatedistances(&xy, npoints, nfeatures, &c, nclusters, &xycref, &xydist2ref, _state);
                for(i=0; i<=npoints-1; i++)
                {
                    seterrorflag(othererrors, xyc.ptr.p_int[i]!=xycref.ptr.p_int[i], _state);
                    seterrorflag(othererrors, ae_fp_greater(ae_fabs(xydist2.ptr.p_double[i]-xydist2ref.ptr.p_double[i], _state),1.0E-6), _state);
                }
            }
        }
    }
    
    /*
     * Test degenerate dataset (less than NClusters distinct points)
     */
    for(nclusters=2; nclusters<=10; nclusters++)
    {
        for(initalgo=0; initalgo<=3; initalgo++)
        {
            for(pass=1; pass<=10; pass++)
            {
                
                /*
                 * Initialize points. Two algorithms are used:
                 * * initialization by small integers (no rounding problems)
                 * * initialization by "long" fraction
                 */
                npoints = 100;
                nfeatures = 10;
                ae_matrix_set_length(&xy, npoints, nfeatures, _state);
                if( ae_fp_greater(hqrndnormal(&rs, _state),(double)(0)) )
                {
                    for(i=0; i<=nclusters-2; i++)
                    {
                        for(j=0; j<=nfeatures-1; j++)
                        {
                            xy.ptr.pp_double[i][j] = ae_sin(hqrndnormal(&rs, _state), _state);
                        }
                    }
                }
                else
                {
                    for(i=0; i<=nclusters-2; i++)
                    {
                        for(j=0; j<=nfeatures-1; j++)
                        {
                            xy.ptr.pp_double[i][j] = (double)(hqrnduniformi(&rs, 50, _state));
                        }
                    }
                }
                for(i=nclusters-1; i<=npoints-1; i++)
                {
                    idx0 = hqrnduniformi(&rs, nclusters-1, _state);
                    for(j=0; j<=nfeatures-1; j++)
                    {
                        xy.ptr.pp_double[i][j] = xy.ptr.pp_double[idx0][j];
                    }
                }
                
                /*
                 * Clusterize with unlimited number of iterations.
                 * Correct error code must be returned.
                 */
                clusterizercreate(&s, _state);
                clusterizersetpoints(&s, &xy, npoints, nfeatures, 2, _state);
                clusterizersetkmeanslimits(&s, 1, 0, _state);
                clusterizersetkmeansinit(&s, initalgo, _state);
                clusterizerrunkmeans(&s, nclusters, &rep, _state);
                seterrorflag(othererrors, rep.terminationtype!=-3, _state);
            }
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This test checks algorithm ability to handle degenerate problems without
causing infinite loop.
*************************************************************************/
static void testclusteringunit_kmeansinfinitelooptest(ae_bool* othererrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t npoints;
    ae_int_t nfeatures;
    ae_int_t nclusters;
    ae_int_t restarts;
    ae_matrix xy;
    ae_int_t i;
    ae_int_t j;
    clusterizerstate s;
    kmeansreport rep;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    _clusterizerstate_init(&s, _state);
    _kmeansreport_init(&rep, _state);

    
    /*
     * Problem 1: all points are same.
     *
     * For NClusters=1 we must get correct solution, for NClusters>1 we must get failure.
     */
    npoints = 100;
    nfeatures = 1;
    restarts = 5;
    ae_matrix_set_length(&xy, npoints, nfeatures, _state);
    for(j=0; j<=nfeatures-1; j++)
    {
        xy.ptr.pp_double[0][j] = ae_randomreal(_state);
    }
    for(i=1; i<=npoints-1; i++)
    {
        for(j=0; j<=nfeatures-1; j++)
        {
            xy.ptr.pp_double[i][j] = xy.ptr.pp_double[0][j];
        }
    }
    nclusters = 1;
    clusterizercreate(&s, _state);
    clusterizersetpoints(&s, &xy, npoints, nfeatures, 2, _state);
    clusterizersetkmeanslimits(&s, restarts, 0, _state);
    clusterizerrunkmeans(&s, nclusters, &rep, _state);
    *othererrors = *othererrors||rep.terminationtype<=0;
    for(i=0; i<=nfeatures-1; i++)
    {
        *othererrors = *othererrors||ae_fp_greater(ae_fabs(rep.c.ptr.pp_double[0][i]-xy.ptr.pp_double[0][i], _state),1000*ae_machineepsilon);
    }
    for(i=0; i<=npoints-1; i++)
    {
        *othererrors = *othererrors||rep.cidx.ptr.p_int[i]!=0;
    }
    nclusters = 5;
    clusterizerrunkmeans(&s, nclusters, &rep, _state);
    *othererrors = *othererrors||rep.terminationtype>0;
    
    /*
     * Problem 2: degenerate dataset (report by Andreas).
     */
    npoints = 57;
    nfeatures = 1;
    restarts = 1;
    nclusters = 4;
    ae_matrix_set_length(&xy, npoints, nfeatures, _state);
    xy.ptr.pp_double[0][0] = 12.244689632138986;
    xy.ptr.pp_double[1][0] = 12.244689632138982;
    xy.ptr.pp_double[2][0] = 12.244689632138986;
    xy.ptr.pp_double[3][0] = 12.244689632138982;
    xy.ptr.pp_double[4][0] = 12.244689632138986;
    xy.ptr.pp_double[5][0] = 12.244689632138986;
    xy.ptr.pp_double[6][0] = 12.244689632138986;
    xy.ptr.pp_double[7][0] = 12.244689632138986;
    xy.ptr.pp_double[8][0] = 12.244689632138986;
    xy.ptr.pp_double[9][0] = 12.244689632138986;
    xy.ptr.pp_double[10][0] = 12.244689632138989;
    xy.ptr.pp_double[11][0] = 12.244689632138984;
    xy.ptr.pp_double[12][0] = 12.244689632138986;
    xy.ptr.pp_double[13][0] = 12.244689632138986;
    xy.ptr.pp_double[14][0] = 12.244689632138989;
    xy.ptr.pp_double[15][0] = 12.244689632138986;
    xy.ptr.pp_double[16][0] = 12.244689632138986;
    xy.ptr.pp_double[17][0] = 12.244689632138986;
    xy.ptr.pp_double[18][0] = 12.244689632138986;
    xy.ptr.pp_double[19][0] = 12.244689632138989;
    xy.ptr.pp_double[20][0] = 12.244689632138972;
    xy.ptr.pp_double[21][0] = 12.244689632138986;
    xy.ptr.pp_double[22][0] = 12.244689632138986;
    xy.ptr.pp_double[23][0] = 12.244689632138986;
    xy.ptr.pp_double[24][0] = 12.244689632138984;
    xy.ptr.pp_double[25][0] = 12.244689632138982;
    xy.ptr.pp_double[26][0] = 12.244689632138986;
    xy.ptr.pp_double[27][0] = 12.244689632138986;
    xy.ptr.pp_double[28][0] = 12.244689632138986;
    xy.ptr.pp_double[29][0] = 12.244689632138986;
    xy.ptr.pp_double[30][0] = 12.244689632138986;
    xy.ptr.pp_double[31][0] = 12.244689632138986;
    xy.ptr.pp_double[32][0] = 12.244689632138986;
    xy.ptr.pp_double[33][0] = 12.244689632138986;
    xy.ptr.pp_double[34][0] = 12.244689632138986;
    xy.ptr.pp_double[35][0] = 12.244689632138982;
    xy.ptr.pp_double[36][0] = 12.244689632138989;
    xy.ptr.pp_double[37][0] = 12.244689632138986;
    xy.ptr.pp_double[38][0] = 12.244689632138986;
    xy.ptr.pp_double[39][0] = 12.244689632138986;
    xy.ptr.pp_double[40][0] = 12.244689632138986;
    xy.ptr.pp_double[41][0] = 12.244689632138986;
    xy.ptr.pp_double[42][0] = 12.244689632138986;
    xy.ptr.pp_double[43][0] = 12.244689632138986;
    xy.ptr.pp_double[44][0] = 12.244689632138986;
    xy.ptr.pp_double[45][0] = 12.244689632138986;
    xy.ptr.pp_double[46][0] = 12.244689632138986;
    xy.ptr.pp_double[47][0] = 12.244689632138986;
    xy.ptr.pp_double[48][0] = 12.244689632138986;
    xy.ptr.pp_double[49][0] = 12.244689632138986;
    xy.ptr.pp_double[50][0] = 12.244689632138984;
    xy.ptr.pp_double[51][0] = 12.244689632138986;
    xy.ptr.pp_double[52][0] = 12.244689632138986;
    xy.ptr.pp_double[53][0] = 12.244689632138986;
    xy.ptr.pp_double[54][0] = 12.244689632138986;
    xy.ptr.pp_double[55][0] = 12.244689632138986;
    xy.ptr.pp_double[56][0] = 12.244689632138986;
    clusterizercreate(&s, _state);
    clusterizersetpoints(&s, &xy, npoints, nfeatures, 2, _state);
    clusterizersetkmeanslimits(&s, restarts, 0, _state);
    clusterizerrunkmeans(&s, nclusters, &rep, _state);
    *othererrors = *othererrors||rep.terminationtype<=0;
    ae_frame_leave(_state);
}


/*************************************************************************
This non-deterministic test checks that Restarts>1 significantly  improves
quality of results.

Subroutine generates random task 3 unit balls in 2D, each with 20  points,
separated by 5 units wide gaps, and solves it  with  Restarts=1  and  with
Restarts=5. Potential functions are compared,  outcome  of  the  trial  is
either 0 or 1 (depending on what is better).

Sequence of 1000 such tasks is  solved.  If  Restarts>1  actually  improve
quality of solution, sum of outcome will be non-binomial.  If  it  doesn't
matter, it will be binomially distributed.

P.S. This test was added after report from Gianluca  Borello  who  noticed
error in the handling of multiple restarts.
*************************************************************************/
static void testclusteringunit_kmeansrestartstest(ae_bool* converrors,
     ae_bool* restartserrors,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t npoints;
    ae_int_t nvars;
    ae_int_t nclusters;
    ae_int_t clustersize;
    ae_int_t restarts;
    ae_int_t passcount;
    double sigmathreshold;
    double p;
    double s;
    ae_matrix xy;
    ae_vector tmp;
    ae_int_t i;
    ae_int_t j;
    ae_int_t pass;
    double ea;
    double eb;
    double v;
    clusterizerstate state;
    kmeansreport rep1;
    kmeansreport rep2;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&xy, 0, 0, DT_REAL, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    _clusterizerstate_init(&state, _state);
    _kmeansreport_init(&rep1, _state);
    _kmeansreport_init(&rep2, _state);

    restarts = 5;
    passcount = 1000;
    clustersize = 20;
    nclusters = 3;
    nvars = 2;
    npoints = nclusters*clustersize;
    sigmathreshold = (double)(5);
    ae_matrix_set_length(&xy, npoints, nvars, _state);
    ae_vector_set_length(&tmp, nvars, _state);
    p = (double)(0);
    for(pass=1; pass<=passcount; pass++)
    {
        
        /*
         * Fill
         */
        for(i=0; i<=npoints-1; i++)
        {
            testclusteringunit_rsphere(&xy, nvars, i, _state);
            for(j=0; j<=nvars-1; j++)
            {
                xy.ptr.pp_double[i][j] = xy.ptr.pp_double[i][j]+(double)i/(double)clustersize*5;
            }
        }
        clusterizercreate(&state, _state);
        clusterizersetpoints(&state, &xy, npoints, nvars, 2, _state);
        
        /*
         * Test: Restarts=1
         */
        clusterizersetkmeanslimits(&state, 1, 0, _state);
        clusterizerrunkmeans(&state, nclusters, &rep1, _state);
        if( rep1.terminationtype<=0 )
        {
            *converrors = ae_true;
            ae_frame_leave(_state);
            return;
        }
        ea = (double)(0);
        for(i=0; i<=npoints-1; i++)
        {
            ae_v_move(&tmp.ptr.p_double[0], 1, &xy.ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
            ae_v_sub(&tmp.ptr.p_double[0], 1, &rep1.c.ptr.pp_double[rep1.cidx.ptr.p_int[i]][0], 1, ae_v_len(0,nvars-1));
            v = ae_v_dotproduct(&tmp.ptr.p_double[0], 1, &tmp.ptr.p_double[0], 1, ae_v_len(0,nvars-1));
            ea = ea+v;
        }
        
        /*
         * Test: Restarts>1
         */
        clusterizersetkmeanslimits(&state, restarts, 0, _state);
        clusterizerrunkmeans(&state, nclusters, &rep2, _state);
        if( rep2.terminationtype<=0 )
        {
            *converrors = ae_true;
            ae_frame_leave(_state);
            return;
        }
        eb = (double)(0);
        for(i=0; i<=npoints-1; i++)
        {
            ae_v_move(&tmp.ptr.p_double[0], 1, &xy.ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
            ae_v_sub(&tmp.ptr.p_double[0], 1, &rep2.c.ptr.pp_double[rep2.cidx.ptr.p_int[i]][0], 1, ae_v_len(0,nvars-1));
            v = ae_v_dotproduct(&tmp.ptr.p_double[0], 1, &tmp.ptr.p_double[0], 1, ae_v_len(0,nvars-1));
            eb = eb+v;
        }
        
        /*
         * Calculate statistic.
         */
        if( ae_fp_less(ea,eb) )
        {
            p = p+1;
        }
        if( ae_fp_eq(ea,eb) )
        {
            p = p+0.5;
        }
    }
    
    /*
     * If Restarts doesn't influence quality of centers found, P must be
     * binomially distributed random value with mean 0.5*PassCount and
     * standard deviation Sqrt(PassCount/4).
     *
     * If Restarts do influence quality of solution, P must be significantly
     * lower than 0.5*PassCount.
     */
    s = (p-0.5*passcount)/ae_sqrt((double)passcount/(double)4, _state);
    *restartserrors = *restartserrors||ae_fp_greater(s,-sigmathreshold);
    ae_frame_leave(_state);
}


/*************************************************************************
Random normal number
*************************************************************************/
static double testclusteringunit_rnormal(ae_state *_state)
{
    double u;
    double v;
    double s;
    double x1;
    double x2;
    double result;


    for(;;)
    {
        u = 2*ae_randomreal(_state)-1;
        v = 2*ae_randomreal(_state)-1;
        s = ae_sqr(u, _state)+ae_sqr(v, _state);
        if( ae_fp_greater(s,(double)(0))&&ae_fp_less(s,(double)(1)) )
        {
            s = ae_sqrt(-2*ae_log(s, _state)/s, _state);
            x1 = u*s;
            x2 = v*s;
            break;
        }
    }
    result = x1;
    return result;
}


/*************************************************************************
Random point from sphere
*************************************************************************/
static void testclusteringunit_rsphere(/* Real    */ ae_matrix* xy,
     ae_int_t n,
     ae_int_t i,
     ae_state *_state)
{
    ae_int_t j;
    double v;


    for(j=0; j<=n-1; j++)
    {
        xy->ptr.pp_double[i][j] = testclusteringunit_rnormal(_state);
    }
    v = ae_v_dotproduct(&xy->ptr.pp_double[i][0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,n-1));
    v = ae_randomreal(_state)/ae_sqrt(v, _state);
    ae_v_muld(&xy->ptr.pp_double[i][0], 1, ae_v_len(0,n-1), v);
}


/*************************************************************************
Distance function: distance between X0 and X1

X0, X1 - array[D], points
DistType - distance type
*************************************************************************/
static double testclusteringunit_distfunc(/* Real    */ ae_vector* x0,
     /* Real    */ ae_vector* x1,
     ae_int_t d,
     ae_int_t disttype,
     ae_state *_state)
{
    ae_int_t i;
    double s0;
    double s1;
    double result;


    ae_assert((((((((disttype==0||disttype==1)||disttype==2)||disttype==10)||disttype==11)||disttype==12)||disttype==13)||disttype==20)||disttype==21, "Assertion failed", _state);
    if( disttype==0 )
    {
        result = 0.0;
        for(i=0; i<=d-1; i++)
        {
            result = ae_maxreal(result, ae_fabs(x0->ptr.p_double[i]-x1->ptr.p_double[i], _state), _state);
        }
        return result;
    }
    if( disttype==1 )
    {
        result = 0.0;
        for(i=0; i<=d-1; i++)
        {
            result = result+ae_fabs(x0->ptr.p_double[i]-x1->ptr.p_double[i], _state);
        }
        return result;
    }
    if( disttype==2 )
    {
        result = 0.0;
        for(i=0; i<=d-1; i++)
        {
            result = result+ae_sqr(x0->ptr.p_double[i]-x1->ptr.p_double[i], _state);
        }
        result = ae_sqrt(result, _state);
        return result;
    }
    if( disttype==10 )
    {
        result = ae_maxreal(1-pearsoncorr2(x0, x1, d, _state), 0.0, _state);
        return result;
    }
    if( disttype==11 )
    {
        result = ae_maxreal(1-ae_fabs(pearsoncorr2(x0, x1, d, _state), _state), 0.0, _state);
        return result;
    }
    if( disttype==12||disttype==13 )
    {
        s0 = 0.0;
        s1 = 0.0;
        for(i=0; i<=d-1; i++)
        {
            s0 = s0+ae_sqr(x0->ptr.p_double[i], _state)/d;
            s1 = s1+ae_sqr(x1->ptr.p_double[i], _state)/d;
        }
        s0 = ae_sqrt(s0, _state);
        s1 = ae_sqrt(s1, _state);
        result = (double)(0);
        for(i=0; i<=d-1; i++)
        {
            result = result+x0->ptr.p_double[i]/s0*(x1->ptr.p_double[i]/s1)/d;
        }
        if( disttype==12 )
        {
            result = ae_maxreal(1-result, 0.0, _state);
        }
        else
        {
            result = ae_maxreal(1-ae_fabs(result, _state), 0.0, _state);
        }
        return result;
    }
    if( disttype==20 )
    {
        result = ae_maxreal(1-spearmancorr2(x0, x1, d, _state), 0.0, _state);
        return result;
    }
    if( disttype==21 )
    {
        result = ae_maxreal(1-ae_fabs(spearmancorr2(x0, x1, d, _state), _state), 0.0, _state);
        return result;
    }
    result = (double)(0);
    return result;
}


/*************************************************************************
This function replays merges and checks that:
* Rep.NPoints, Rep.Z, Rep.PZ and Rep.PM are consistent and correct
* Rep.MergeDist is consistent with distance between clusters being merged
* clusters with minimal distance are merged at each step
* GetKClusters() correctly unpacks clusters for each K

NOTE: this algorithm correctly handle ties, i.e. situations where several
      pairs  of  clusters  have  same intercluster distance, and we can't
      unambiguously choose clusters to merge.

INPUT PARAMETERS
    D           -   distance matrix, array[NPoints,NPoints], full matrix
                    is given (including both triangles and zeros on the
                    main diagonal)
    XY          -   dataset matrix, array[NPoints,NF]
    NPoints     -   dataset size
    NF          -   number of features
    Rep         -   clusterizer report
    AHCAlgo     -   AHC algorithm:
                    * 0 - complete linkage
                    * 1 - single linkage
                    * 2 - unweighted average linkage

This function returns True on failure, False on success.
*************************************************************************/
static ae_bool testclusteringunit_errorsinmerges(/* Real    */ ae_matrix* d,
     /* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nf,
     ahcreport* rep,
     ae_int_t ahcalgo,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_matrix dm;
    ae_matrix cm;
    ae_vector clustersizes;
    ae_vector clusterheights;
    ae_vector b;
    ae_vector x0;
    ae_vector x1;
    ae_bool bflag;
    ae_int_t i;
    ae_int_t j;
    ae_int_t k;
    ae_int_t i0;
    ae_int_t i1;
    ae_int_t c0;
    ae_int_t c1;
    ae_int_t s0;
    ae_int_t s1;
    double v;
    ae_int_t t;
    ae_int_t mergeidx;
    ae_vector kidx;
    ae_vector kidxz;
    ae_int_t currentelement;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&dm, 0, 0, DT_REAL, _state);
    ae_matrix_init(&cm, 0, 0, DT_INT, _state);
    ae_vector_init(&clustersizes, 0, DT_INT, _state);
    ae_vector_init(&clusterheights, 0, DT_INT, _state);
    ae_vector_init(&b, 0, DT_BOOL, _state);
    ae_vector_init(&x0, 0, DT_REAL, _state);
    ae_vector_init(&x1, 0, DT_REAL, _state);
    ae_vector_init(&kidx, 0, DT_INT, _state);
    ae_vector_init(&kidxz, 0, DT_INT, _state);

    ae_assert(ahcalgo!=3, "integrity error", _state);
    result = ae_false;
    ae_vector_set_length(&x0, nf, _state);
    ae_vector_set_length(&x1, nf, _state);
    
    /*
     * Basic checks:
     * * positive completion code
     * * sizes of arrays
     * * Rep.P is correct permutation
     * * Rep.Z contains correct cluster indexes
     * * Rep.PZ is consistent with Rep.P/Rep.Z
     * * Rep.PM contains consistent indexes
     * * GetKClusters() for K=NPoints
     */
    bflag = ae_false;
    bflag = bflag||rep->terminationtype<=0;
    if( bflag )
    {
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    bflag = bflag||rep->npoints!=npoints;
    bflag = (bflag||rep->z.rows!=npoints-1)||(npoints>1&&rep->z.cols!=2);
    bflag = (bflag||rep->pz.rows!=npoints-1)||(npoints>1&&rep->pz.cols!=2);
    bflag = (bflag||rep->pm.rows!=npoints-1)||(npoints>1&&rep->pm.cols!=6);
    bflag = bflag||rep->mergedist.cnt!=npoints-1;
    bflag = bflag||rep->p.cnt!=npoints;
    if( bflag )
    {
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    ae_vector_set_length(&b, npoints, _state);
    for(i=0; i<=npoints-1; i++)
    {
        b.ptr.p_bool[i] = ae_false;
    }
    for(i=0; i<=npoints-1; i++)
    {
        if( (rep->p.ptr.p_int[i]<0||rep->p.ptr.p_int[i]>=npoints)||b.ptr.p_bool[rep->p.ptr.p_int[i]] )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        b.ptr.p_bool[rep->p.ptr.p_int[i]] = ae_true;
    }
    for(i=0; i<=npoints-2; i++)
    {
        if( (rep->z.ptr.pp_int[i][0]<0||rep->z.ptr.pp_int[i][0]>=rep->z.ptr.pp_int[i][1])||rep->z.ptr.pp_int[i][1]>=npoints+i )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        if( (rep->pz.ptr.pp_int[i][0]<0||rep->pz.ptr.pp_int[i][0]>=rep->pz.ptr.pp_int[i][1])||rep->pz.ptr.pp_int[i][1]>=npoints+i )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    for(i=0; i<=npoints-2; i++)
    {
        c0 = rep->z.ptr.pp_int[i][0];
        c1 = rep->z.ptr.pp_int[i][1];
        s0 = rep->pz.ptr.pp_int[i][0];
        s1 = rep->pz.ptr.pp_int[i][1];
        if( c0<npoints )
        {
            c0 = rep->p.ptr.p_int[c0];
        }
        if( c1<npoints )
        {
            c1 = rep->p.ptr.p_int[c1];
        }
        if( c0!=s0||c1!=s1 )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    clusterizergetkclusters(rep, npoints, &kidx, &kidxz, _state);
    if( kidx.cnt!=npoints||kidxz.cnt!=npoints )
    {
        result = ae_true;
        ae_frame_leave(_state);
        return result;
    }
    for(i=0; i<=npoints-1; i++)
    {
        if( kidxz.ptr.p_int[i]!=i||kidx.ptr.p_int[i]!=i )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
    }
    
    /*
     * Test description:
     * * we generate (2*NPoints-1)x(2*NPoints-1) matrix of distances DM and
     *   (2*NPoints-1)xNPoints matrix of clusters CM (I-th row contains indexes
     *   of elements which belong to I-th cluster, negative indexes denote
     *   empty cells). Leading N*N square of DM is just a distance matrix,
     *   other elements are filled by some large number M (used to mark empty
     *   elements).
     * * we replay all merges
     * * every time we merge clusters I and J into K, we:
     *   * check that distance between I and J is equal to the smallest
     *     element of DM (note: we account for rounding errors when we
     *     decide on that)
     *   * check that distance is consistent with Rep.MergeDist
     *   * then, we enumerate all elements in clusters being merged,
     *     and check that after permutation their indexes fall into range
     *     prescribed by Rep.PM
     *   * fill K-th column/row of D by distances to cluster K
     *   * merge I-th and J-th rows of CM and store result into K-th row
     *   * clear DM and CM: fill I-th and J-th column/row of DM by large
     *     number M, fill I-th and J-th row of CM by -1.
     *
     * NOTE: DM is initialized by distance metric specific to AHC algorithm
     *       being used. CLINK, SLINK and average linkage use user-provided
     *       distance measure, say Euclidean one, without any modifications.
     *       Ward's method uses (and reports) squared and scaled Euclidean
     *       distances.
     */
    ae_matrix_set_length(&dm, 2*npoints-1, 2*npoints-1, _state);
    ae_matrix_set_length(&cm, 2*npoints-1, npoints, _state);
    ae_vector_set_length(&clustersizes, 2*npoints-1, _state);
    for(i=0; i<=2*npoints-2; i++)
    {
        for(j=0; j<=2*npoints-2; j++)
        {
            if( i<npoints&&j<npoints )
            {
                dm.ptr.pp_double[i][j] = d->ptr.pp_double[i][j];
                if( ahcalgo==4 )
                {
                    dm.ptr.pp_double[i][j] = 0.5*ae_sqr(dm.ptr.pp_double[i][j], _state);
                }
            }
            else
            {
                dm.ptr.pp_double[i][j] = ae_maxrealnumber;
            }
        }
    }
    for(i=0; i<=2*npoints-2; i++)
    {
        for(j=0; j<=npoints-1; j++)
        {
            cm.ptr.pp_int[i][j] = -1;
        }
    }
    for(i=0; i<=npoints-1; i++)
    {
        cm.ptr.pp_int[i][0] = i;
        clustersizes.ptr.p_int[i] = 1;
    }
    for(i=npoints; i<=2*npoints-2; i++)
    {
        clustersizes.ptr.p_int[i] = 0;
    }
    ae_vector_set_length(&clusterheights, 2*npoints-1, _state);
    for(i=0; i<=npoints-1; i++)
    {
        clusterheights.ptr.p_int[i] = 0;
    }
    for(mergeidx=0; mergeidx<=npoints-2; mergeidx++)
    {
        
        /*
         * Check that clusters with minimum distance are merged,
         * and that MergeDist is consistent with results.
         *
         * NOTE: we do not check for specific cluster indexes,
         *       because it is possible to have a tie. We just
         *       check that distance between clusters is a true
         *       minimum over all possible clusters.
         */
        v = ae_maxrealnumber;
        for(i=0; i<=2*npoints-2; i++)
        {
            for(j=0; j<=2*npoints-2; j++)
            {
                if( i!=j )
                {
                    v = ae_minreal(v, dm.ptr.pp_double[i][j], _state);
                }
            }
        }
        c0 = rep->z.ptr.pp_int[mergeidx][0];
        c1 = rep->z.ptr.pp_int[mergeidx][1];
        if( ae_fp_greater(dm.ptr.pp_double[c0][c1],v+10000*ae_machineepsilon) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        if( ae_fp_greater(rep->mergedist.ptr.p_double[mergeidx],v+10000*ae_machineepsilon) )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Check that indexes of elements fall into range prescribed by Rep.PM,
         * and Rep.PM correctly described merge operation
         */
        s0 = clustersizes.ptr.p_int[c0];
        s1 = clustersizes.ptr.p_int[c1];
        for(j=0; j<=clustersizes.ptr.p_int[c0]-1; j++)
        {
            if( rep->p.ptr.p_int[cm.ptr.pp_int[c0][j]]<rep->pm.ptr.pp_int[mergeidx][0]||rep->p.ptr.p_int[cm.ptr.pp_int[c0][j]]>rep->pm.ptr.pp_int[mergeidx][1] )
            {
                
                /*
                 * Element falls outside of range described by PM
                 */
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
        for(j=0; j<=clustersizes.ptr.p_int[c1]-1; j++)
        {
            if( rep->p.ptr.p_int[cm.ptr.pp_int[c1][j]]<rep->pm.ptr.pp_int[mergeidx][2]||rep->p.ptr.p_int[cm.ptr.pp_int[c1][j]]>rep->pm.ptr.pp_int[mergeidx][3] )
            {
                
                /*
                 * Element falls outside of range described by PM
                 */
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
        if( (rep->pm.ptr.pp_int[mergeidx][1]-rep->pm.ptr.pp_int[mergeidx][0]!=s0-1||rep->pm.ptr.pp_int[mergeidx][3]-rep->pm.ptr.pp_int[mergeidx][2]!=s1-1)||rep->pm.ptr.pp_int[mergeidx][2]!=rep->pm.ptr.pp_int[mergeidx][1]+1 )
        {
            
            /*
             * Cluster size (as given by PM) is inconsistent with its actual size.
             */
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        if( rep->pm.ptr.pp_int[mergeidx][4]!=clusterheights.ptr.p_int[rep->z.ptr.pp_int[mergeidx][0]]||rep->pm.ptr.pp_int[mergeidx][5]!=clusterheights.ptr.p_int[rep->z.ptr.pp_int[mergeidx][1]] )
        {
            
            /*
             * Heights of subdendrograms as returned by PM are inconsistent with heights
             * calculated by us.
             */
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        
        /*
         * Update cluster heights
         */
        clusterheights.ptr.p_int[mergeidx+npoints] = ae_maxint(clusterheights.ptr.p_int[rep->z.ptr.pp_int[mergeidx][0]], clusterheights.ptr.p_int[rep->z.ptr.pp_int[mergeidx][1]], _state)+1;
        
        /*
         * Update CM
         */
        t = 0;
        for(j=0; j<=clustersizes.ptr.p_int[rep->z.ptr.pp_int[mergeidx][0]]-1; j++)
        {
            cm.ptr.pp_int[npoints+mergeidx][t] = cm.ptr.pp_int[rep->z.ptr.pp_int[mergeidx][0]][j];
            t = t+1;
        }
        for(j=0; j<=clustersizes.ptr.p_int[rep->z.ptr.pp_int[mergeidx][1]]-1; j++)
        {
            cm.ptr.pp_int[npoints+mergeidx][t] = cm.ptr.pp_int[rep->z.ptr.pp_int[mergeidx][1]][j];
            t = t+1;
        }
        clustersizes.ptr.p_int[npoints+mergeidx] = t;
        clustersizes.ptr.p_int[rep->z.ptr.pp_int[mergeidx][0]] = 0;
        clustersizes.ptr.p_int[rep->z.ptr.pp_int[mergeidx][1]] = 0;
        
        /*
         * Update distance matrix D
         */
        for(i=0; i<=2*npoints-2; i++)
        {
            
            /*
             * "Remove" columns/rows corresponding to clusters being merged
             */
            dm.ptr.pp_double[i][rep->z.ptr.pp_int[mergeidx][0]] = ae_maxrealnumber;
            dm.ptr.pp_double[i][rep->z.ptr.pp_int[mergeidx][1]] = ae_maxrealnumber;
            dm.ptr.pp_double[rep->z.ptr.pp_int[mergeidx][0]][i] = ae_maxrealnumber;
            dm.ptr.pp_double[rep->z.ptr.pp_int[mergeidx][1]][i] = ae_maxrealnumber;
        }
        for(i=0; i<=npoints+mergeidx-1; i++)
        {
            if( clustersizes.ptr.p_int[i]>0 )
            {
                
                /*
                 * Calculate column/row corresponding to new cluster
                 */
                if( ahcalgo==0 )
                {
                    
                    /*
                     * Calculate distance between clusters I and NPoints+MergeIdx for CLINK
                     */
                    v = 0.0;
                    for(i0=0; i0<=clustersizes.ptr.p_int[i]-1; i0++)
                    {
                        for(i1=0; i1<=clustersizes.ptr.p_int[npoints+mergeidx]-1; i1++)
                        {
                            v = ae_maxreal(v, d->ptr.pp_double[cm.ptr.pp_int[i][i0]][cm.ptr.pp_int[npoints+mergeidx][i1]], _state);
                        }
                    }
                }
                if( ahcalgo==1 )
                {
                    
                    /*
                     * Calculate distance between clusters I and NPoints+MergeIdx for SLINK
                     */
                    v = ae_maxrealnumber;
                    for(i0=0; i0<=clustersizes.ptr.p_int[i]-1; i0++)
                    {
                        for(i1=0; i1<=clustersizes.ptr.p_int[npoints+mergeidx]-1; i1++)
                        {
                            v = ae_minreal(v, d->ptr.pp_double[cm.ptr.pp_int[i][i0]][cm.ptr.pp_int[npoints+mergeidx][i1]], _state);
                        }
                    }
                }
                if( ahcalgo==2 )
                {
                    
                    /*
                     * Calculate distance between clusters I and NPoints+MergeIdx for unweighted average
                     */
                    v = 0.0;
                    t = 0;
                    for(i0=0; i0<=clustersizes.ptr.p_int[i]-1; i0++)
                    {
                        for(i1=0; i1<=clustersizes.ptr.p_int[npoints+mergeidx]-1; i1++)
                        {
                            v = v+d->ptr.pp_double[cm.ptr.pp_int[i][i0]][cm.ptr.pp_int[npoints+mergeidx][i1]];
                            t = t+1;
                        }
                    }
                    v = v/t;
                }
                if( ahcalgo==3 )
                {
                    ae_assert(ae_false, "Assertion failed", _state);
                }
                if( ahcalgo==4 )
                {
                    
                    /*
                     * Calculate distance between clusters I and NPoints+MergeIdx for Ward's method:
                     * * X0 = center of mass for cluster I
                     * * X1 = center of mass for cluster NPoints+MergeIdx
                     * * S0 = size of cluster I
                     * * S1 = size of cluster NPoints+MergeIdx
                     * * distance between clusters is S0*S1/(S0+S1)*|X0-X1|^2
                     *
                     */
                    for(j=0; j<=nf-1; j++)
                    {
                        x0.ptr.p_double[j] = 0.0;
                        x1.ptr.p_double[j] = 0.0;
                    }
                    for(i0=0; i0<=clustersizes.ptr.p_int[i]-1; i0++)
                    {
                        for(j=0; j<=nf-1; j++)
                        {
                            x0.ptr.p_double[j] = x0.ptr.p_double[j]+xy->ptr.pp_double[cm.ptr.pp_int[i][i0]][j]/clustersizes.ptr.p_int[i];
                        }
                    }
                    for(i1=0; i1<=clustersizes.ptr.p_int[npoints+mergeidx]-1; i1++)
                    {
                        for(j=0; j<=nf-1; j++)
                        {
                            x1.ptr.p_double[j] = x1.ptr.p_double[j]+xy->ptr.pp_double[cm.ptr.pp_int[npoints+mergeidx][i1]][j]/clustersizes.ptr.p_int[npoints+mergeidx];
                        }
                    }
                    v = 0.0;
                    for(j=0; j<=nf-1; j++)
                    {
                        v = v+ae_sqr(x0.ptr.p_double[j]-x1.ptr.p_double[j], _state);
                    }
                    v = v*clustersizes.ptr.p_int[i]*clustersizes.ptr.p_int[npoints+mergeidx]/(clustersizes.ptr.p_int[i]+clustersizes.ptr.p_int[npoints+mergeidx]);
                }
                dm.ptr.pp_double[i][npoints+mergeidx] = v;
                dm.ptr.pp_double[npoints+mergeidx][i] = v;
            }
        }
        
        /*
         * Check that GetKClusters() correctly unpacks clusters for K=NPoints-(MergeIdx+1):
         * * check lengths of arays
         * * check consistency of CIdx/CZ parameters
         * * scan clusters (CZ parameter), for each cluster scan CM matrix which stores
         *   cluster elements (according to our replay of merges), for each element of
         *   the current cluster check that CIdx array correctly reflects its status.
         */
        k = npoints-(mergeidx+1);
        clusterizergetkclusters(rep, k, &kidx, &kidxz, _state);
        if( kidx.cnt!=npoints||kidxz.cnt!=k )
        {
            result = ae_true;
            ae_frame_leave(_state);
            return result;
        }
        for(i=0; i<=k-2; i++)
        {
            if( (kidxz.ptr.p_int[i]<0||kidxz.ptr.p_int[i]>=kidxz.ptr.p_int[i+1])||kidxz.ptr.p_int[i+1]>2*npoints-2 )
            {
                
                /*
                 * CZ is inconsistent
                 */
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
        for(i=0; i<=npoints-1; i++)
        {
            if( kidx.ptr.p_int[i]<0||kidx.ptr.p_int[i]>=k )
            {
                
                /*
                 * CIdx is inconsistent
                 */
                result = ae_true;
                ae_frame_leave(_state);
                return result;
            }
        }
        for(i=0; i<=k-1; i++)
        {
            for(j=0; j<=clustersizes.ptr.p_int[kidxz.ptr.p_int[i]]-1; j++)
            {
                currentelement = cm.ptr.pp_int[kidxz.ptr.p_int[i]][j];
                if( kidx.ptr.p_int[currentelement]!=i )
                {
                    
                    /*
                     * We've found element which belongs to I-th cluster (according to CM
                     * matrix, which reflects current status of agglomerative clustering),
                     * but this element does not belongs to I-th cluster according to
                     * results of ClusterizerGetKClusters()
                     */
                    result = ae_true;
                    ae_frame_leave(_state);
                    return result;
                }
            }
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
This procedure is a reference version of KMeansUpdateDistances().

INPUT PARAMETERS:
    XY          -   dataset, array [0..NPoints-1,0..NVars-1].
    NPoints     -   dataset size, NPoints>=K
    NVars       -   number of variables, NVars>=1
    CT          -   matrix of centers, centers are stored in rows
    K           -   number of centers, K>=1
    XYC         -   preallocated output buffer
    XYDist2     -   preallocated output buffer

OUTPUT PARAMETERS:
    XYC         -   new assignment of points to centers
    XYDist2     -   squared distances

  -- ALGLIB --
     Copyright 21.01.2015 by Bochkanov Sergey
*************************************************************************/
static void testclusteringunit_kmeansreferenceupdatedistances(/* Real    */ ae_matrix* xy,
     ae_int_t npoints,
     ae_int_t nvars,
     /* Real    */ ae_matrix* ct,
     ae_int_t k,
     /* Integer */ ae_vector* xyc,
     /* Real    */ ae_vector* xydist2,
     ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t cclosest;
    double dclosest;
    double v;
    ae_vector tmp;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&tmp, 0, DT_REAL, _state);

    ae_vector_set_length(&tmp, nvars, _state);
    for(i=0; i<=npoints-1; i++)
    {
        cclosest = -1;
        dclosest = ae_maxrealnumber;
        for(j=0; j<=k-1; j++)
        {
            ae_v_move(&tmp.ptr.p_double[0], 1, &xy->ptr.pp_double[i][0], 1, ae_v_len(0,nvars-1));
            ae_v_sub(&tmp.ptr.p_double[0], 1, &ct->ptr.pp_double[j][0], 1, ae_v_len(0,nvars-1));
            v = ae_v_dotproduct(&tmp.ptr.p_double[0], 1, &tmp.ptr.p_double[0], 1, ae_v_len(0,nvars-1));
            if( ae_fp_less(v,dclosest) )
            {
                cclosest = j;
                dclosest = v;
            }
        }
        ae_assert(cclosest>=0, "KMeansUpdateDistances: internal error", _state);
        xyc->ptr.p_int[i] = cclosest;
        xydist2->ptr.p_double[i] = dclosest;
    }
    ae_frame_leave(_state);
}


/*$ End $*/
