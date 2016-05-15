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
#include "testbdssunit.h"


/*$ Declarations $*/
static void testbdssunit_unset1di(/* Integer */ ae_vector* a,
     ae_state *_state);


/*$ Body $*/


/*************************************************************************
Testing BDSS operations
*************************************************************************/
ae_bool testbdss(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t n;
    ae_int_t i;
    ae_int_t j;
    ae_int_t pass;
    ae_int_t passcount;
    ae_int_t maxn;
    ae_int_t maxnq;
    ae_vector a;
    ae_vector a0;
    ae_vector at;
    ae_matrix p;
    ae_vector thresholds;
    ae_int_t ni;
    ae_vector c;
    ae_vector p1;
    ae_vector p2;
    ae_vector ties;
    ae_vector pt1;
    ae_vector pt2;
    ae_int_t tiecount;
    ae_int_t c1;
    ae_int_t c0;
    ae_int_t nc;
    ae_vector tmp;
    ae_vector sortrbuf;
    ae_vector sortrbuf2;
    ae_vector sortibuf;
    double pal;
    double pbl;
    double par;
    double pbr;
    double cve;
    double cvr;
    ae_int_t info;
    double threshold;
    ae_vector tiebuf;
    ae_vector cntbuf;
    double rms;
    double cvrms;
    ae_bool waserrors;
    ae_bool tieserrors;
    ae_bool split2errors;
    ae_bool optimalsplitkerrors;
    ae_bool splitkerrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&a, 0, DT_REAL, _state);
    ae_vector_init(&a0, 0, DT_REAL, _state);
    ae_vector_init(&at, 0, DT_REAL, _state);
    ae_matrix_init(&p, 0, 0, DT_REAL, _state);
    ae_vector_init(&thresholds, 0, DT_REAL, _state);
    ae_vector_init(&c, 0, DT_INT, _state);
    ae_vector_init(&p1, 0, DT_INT, _state);
    ae_vector_init(&p2, 0, DT_INT, _state);
    ae_vector_init(&ties, 0, DT_INT, _state);
    ae_vector_init(&pt1, 0, DT_INT, _state);
    ae_vector_init(&pt2, 0, DT_INT, _state);
    ae_vector_init(&tmp, 0, DT_REAL, _state);
    ae_vector_init(&sortrbuf, 0, DT_REAL, _state);
    ae_vector_init(&sortrbuf2, 0, DT_REAL, _state);
    ae_vector_init(&sortibuf, 0, DT_INT, _state);
    ae_vector_init(&tiebuf, 0, DT_INT, _state);
    ae_vector_init(&cntbuf, 0, DT_INT, _state);

    waserrors = ae_false;
    tieserrors = ae_false;
    split2errors = ae_false;
    splitkerrors = ae_false;
    optimalsplitkerrors = ae_false;
    maxn = 100;
    maxnq = 49;
    passcount = 10;
    
    /*
     * Test ties
     */
    for(n=1; n<=maxn; n++)
    {
        for(pass=1; pass<=passcount; pass++)
        {
            
            /*
             * untied data, test DSTie
             */
            testbdssunit_unset1di(&p1, _state);
            testbdssunit_unset1di(&p2, _state);
            testbdssunit_unset1di(&pt1, _state);
            testbdssunit_unset1di(&pt2, _state);
            ae_vector_set_length(&a, n-1+1, _state);
            ae_vector_set_length(&a0, n-1+1, _state);
            ae_vector_set_length(&at, n-1+1, _state);
            ae_vector_set_length(&tmp, n-1+1, _state);
            a.ptr.p_double[0] = 2*ae_randomreal(_state)-1;
            tmp.ptr.p_double[0] = ae_randomreal(_state);
            for(i=1; i<=n-1; i++)
            {
                
                /*
                 * A is randomly permuted
                 */
                a.ptr.p_double[i] = a.ptr.p_double[i-1]+0.1*ae_randomreal(_state)+0.1;
                tmp.ptr.p_double[i] = ae_randomreal(_state);
            }
            tagsortfastr(&tmp, &a, &sortrbuf, &sortrbuf2, n, _state);
            for(i=0; i<=n-1; i++)
            {
                a0.ptr.p_double[i] = a.ptr.p_double[i];
                at.ptr.p_double[i] = a.ptr.p_double[i];
            }
            dstie(&a0, n, &ties, &tiecount, &p1, &p2, _state);
            tagsort(&at, n, &pt1, &pt2, _state);
            for(i=0; i<=n-1; i++)
            {
                tieserrors = tieserrors||p1.ptr.p_int[i]!=pt1.ptr.p_int[i];
                tieserrors = tieserrors||p2.ptr.p_int[i]!=pt2.ptr.p_int[i];
            }
            tieserrors = tieserrors||tiecount!=n;
            if( tiecount==n )
            {
                for(i=0; i<=n; i++)
                {
                    tieserrors = tieserrors||ties.ptr.p_int[i]!=i;
                }
            }
            
            /*
             * tied data, test DSTie
             */
            testbdssunit_unset1di(&p1, _state);
            testbdssunit_unset1di(&p2, _state);
            testbdssunit_unset1di(&pt1, _state);
            testbdssunit_unset1di(&pt2, _state);
            ae_vector_set_length(&a, n-1+1, _state);
            ae_vector_set_length(&a0, n-1+1, _state);
            ae_vector_set_length(&at, n-1+1, _state);
            c1 = 0;
            c0 = 0;
            for(i=0; i<=n-1; i++)
            {
                a.ptr.p_double[i] = (double)(ae_randominteger(2, _state));
                if( ae_fp_eq(a.ptr.p_double[i],(double)(0)) )
                {
                    c0 = c0+1;
                }
                else
                {
                    c1 = c1+1;
                }
                a0.ptr.p_double[i] = a.ptr.p_double[i];
                at.ptr.p_double[i] = a.ptr.p_double[i];
            }
            dstie(&a0, n, &ties, &tiecount, &p1, &p2, _state);
            tagsort(&at, n, &pt1, &pt2, _state);
            for(i=0; i<=n-1; i++)
            {
                tieserrors = tieserrors||p1.ptr.p_int[i]!=pt1.ptr.p_int[i];
                tieserrors = tieserrors||p2.ptr.p_int[i]!=pt2.ptr.p_int[i];
            }
            if( c0==0||c1==0 )
            {
                tieserrors = tieserrors||tiecount!=1;
                if( tiecount==1 )
                {
                    tieserrors = tieserrors||ties.ptr.p_int[0]!=0;
                    tieserrors = tieserrors||ties.ptr.p_int[1]!=n;
                }
            }
            else
            {
                tieserrors = tieserrors||tiecount!=2;
                if( tiecount==2 )
                {
                    tieserrors = tieserrors||ties.ptr.p_int[0]!=0;
                    tieserrors = tieserrors||ties.ptr.p_int[1]!=c0;
                    tieserrors = tieserrors||ties.ptr.p_int[2]!=n;
                }
            }
        }
    }
    
    /*
     * split-2
     */
    
    /*
     * General tests for different N's
     */
    for(n=1; n<=maxn; n++)
    {
        ae_vector_set_length(&a, n-1+1, _state);
        ae_vector_set_length(&c, n-1+1, _state);
        
        /*
         * one-tie test
         */
        if( n%2==0 )
        {
            for(i=0; i<=n-1; i++)
            {
                a.ptr.p_double[i] = (double)(n);
                c.ptr.p_int[i] = i%2;
            }
            dsoptimalsplit2(&a, &c, n, &info, &threshold, &pal, &pbl, &par, &pbr, &cve, _state);
            if( info!=-3 )
            {
                split2errors = ae_true;
                continue;
            }
        }
        
        /*
         * two-tie test
         */
        
        /*
         * test #1
         */
        if( n>1 )
        {
            for(i=0; i<=n-1; i++)
            {
                a.ptr.p_double[i] = (double)(i/((n+1)/2));
                c.ptr.p_int[i] = i/((n+1)/2);
            }
            dsoptimalsplit2(&a, &c, n, &info, &threshold, &pal, &pbl, &par, &pbr, &cve, _state);
            if( info!=1 )
            {
                split2errors = ae_true;
                continue;
            }
            split2errors = split2errors||ae_fp_greater(ae_fabs(threshold-0.5, _state),100*ae_machineepsilon);
            split2errors = split2errors||ae_fp_greater(ae_fabs(pal-1, _state),100*ae_machineepsilon);
            split2errors = split2errors||ae_fp_greater(ae_fabs(pbl-0, _state),100*ae_machineepsilon);
            split2errors = split2errors||ae_fp_greater(ae_fabs(par-0, _state),100*ae_machineepsilon);
            split2errors = split2errors||ae_fp_greater(ae_fabs(pbr-1, _state),100*ae_machineepsilon);
        }
    }
    
    /*
     * Special "CREDIT"-test (transparency coefficient)
     */
    n = 110;
    ae_vector_set_length(&a, n-1+1, _state);
    ae_vector_set_length(&c, n-1+1, _state);
    a.ptr.p_double[0] = 0.000;
    c.ptr.p_int[0] = 0;
    a.ptr.p_double[1] = 0.000;
    c.ptr.p_int[1] = 0;
    a.ptr.p_double[2] = 0.000;
    c.ptr.p_int[2] = 0;
    a.ptr.p_double[3] = 0.000;
    c.ptr.p_int[3] = 0;
    a.ptr.p_double[4] = 0.000;
    c.ptr.p_int[4] = 0;
    a.ptr.p_double[5] = 0.000;
    c.ptr.p_int[5] = 0;
    a.ptr.p_double[6] = 0.000;
    c.ptr.p_int[6] = 0;
    a.ptr.p_double[7] = 0.000;
    c.ptr.p_int[7] = 1;
    a.ptr.p_double[8] = 0.000;
    c.ptr.p_int[8] = 0;
    a.ptr.p_double[9] = 0.000;
    c.ptr.p_int[9] = 1;
    a.ptr.p_double[10] = 0.000;
    c.ptr.p_int[10] = 0;
    a.ptr.p_double[11] = 0.000;
    c.ptr.p_int[11] = 0;
    a.ptr.p_double[12] = 0.000;
    c.ptr.p_int[12] = 0;
    a.ptr.p_double[13] = 0.000;
    c.ptr.p_int[13] = 0;
    a.ptr.p_double[14] = 0.000;
    c.ptr.p_int[14] = 0;
    a.ptr.p_double[15] = 0.000;
    c.ptr.p_int[15] = 0;
    a.ptr.p_double[16] = 0.000;
    c.ptr.p_int[16] = 0;
    a.ptr.p_double[17] = 0.000;
    c.ptr.p_int[17] = 0;
    a.ptr.p_double[18] = 0.000;
    c.ptr.p_int[18] = 0;
    a.ptr.p_double[19] = 0.000;
    c.ptr.p_int[19] = 0;
    a.ptr.p_double[20] = 0.000;
    c.ptr.p_int[20] = 0;
    a.ptr.p_double[21] = 0.000;
    c.ptr.p_int[21] = 0;
    a.ptr.p_double[22] = 0.000;
    c.ptr.p_int[22] = 1;
    a.ptr.p_double[23] = 0.000;
    c.ptr.p_int[23] = 0;
    a.ptr.p_double[24] = 0.000;
    c.ptr.p_int[24] = 0;
    a.ptr.p_double[25] = 0.000;
    c.ptr.p_int[25] = 0;
    a.ptr.p_double[26] = 0.000;
    c.ptr.p_int[26] = 0;
    a.ptr.p_double[27] = 0.000;
    c.ptr.p_int[27] = 1;
    a.ptr.p_double[28] = 0.000;
    c.ptr.p_int[28] = 0;
    a.ptr.p_double[29] = 0.000;
    c.ptr.p_int[29] = 1;
    a.ptr.p_double[30] = 0.000;
    c.ptr.p_int[30] = 0;
    a.ptr.p_double[31] = 0.000;
    c.ptr.p_int[31] = 1;
    a.ptr.p_double[32] = 0.000;
    c.ptr.p_int[32] = 0;
    a.ptr.p_double[33] = 0.000;
    c.ptr.p_int[33] = 1;
    a.ptr.p_double[34] = 0.000;
    c.ptr.p_int[34] = 0;
    a.ptr.p_double[35] = 0.030;
    c.ptr.p_int[35] = 0;
    a.ptr.p_double[36] = 0.030;
    c.ptr.p_int[36] = 0;
    a.ptr.p_double[37] = 0.050;
    c.ptr.p_int[37] = 0;
    a.ptr.p_double[38] = 0.070;
    c.ptr.p_int[38] = 1;
    a.ptr.p_double[39] = 0.110;
    c.ptr.p_int[39] = 0;
    a.ptr.p_double[40] = 0.110;
    c.ptr.p_int[40] = 1;
    a.ptr.p_double[41] = 0.120;
    c.ptr.p_int[41] = 0;
    a.ptr.p_double[42] = 0.130;
    c.ptr.p_int[42] = 0;
    a.ptr.p_double[43] = 0.140;
    c.ptr.p_int[43] = 0;
    a.ptr.p_double[44] = 0.140;
    c.ptr.p_int[44] = 0;
    a.ptr.p_double[45] = 0.140;
    c.ptr.p_int[45] = 0;
    a.ptr.p_double[46] = 0.150;
    c.ptr.p_int[46] = 0;
    a.ptr.p_double[47] = 0.150;
    c.ptr.p_int[47] = 0;
    a.ptr.p_double[48] = 0.170;
    c.ptr.p_int[48] = 0;
    a.ptr.p_double[49] = 0.190;
    c.ptr.p_int[49] = 1;
    a.ptr.p_double[50] = 0.200;
    c.ptr.p_int[50] = 0;
    a.ptr.p_double[51] = 0.200;
    c.ptr.p_int[51] = 0;
    a.ptr.p_double[52] = 0.250;
    c.ptr.p_int[52] = 0;
    a.ptr.p_double[53] = 0.250;
    c.ptr.p_int[53] = 0;
    a.ptr.p_double[54] = 0.260;
    c.ptr.p_int[54] = 0;
    a.ptr.p_double[55] = 0.270;
    c.ptr.p_int[55] = 0;
    a.ptr.p_double[56] = 0.280;
    c.ptr.p_int[56] = 0;
    a.ptr.p_double[57] = 0.310;
    c.ptr.p_int[57] = 0;
    a.ptr.p_double[58] = 0.310;
    c.ptr.p_int[58] = 0;
    a.ptr.p_double[59] = 0.330;
    c.ptr.p_int[59] = 0;
    a.ptr.p_double[60] = 0.330;
    c.ptr.p_int[60] = 0;
    a.ptr.p_double[61] = 0.340;
    c.ptr.p_int[61] = 0;
    a.ptr.p_double[62] = 0.340;
    c.ptr.p_int[62] = 0;
    a.ptr.p_double[63] = 0.370;
    c.ptr.p_int[63] = 0;
    a.ptr.p_double[64] = 0.380;
    c.ptr.p_int[64] = 1;
    a.ptr.p_double[65] = 0.380;
    c.ptr.p_int[65] = 0;
    a.ptr.p_double[66] = 0.410;
    c.ptr.p_int[66] = 0;
    a.ptr.p_double[67] = 0.460;
    c.ptr.p_int[67] = 0;
    a.ptr.p_double[68] = 0.520;
    c.ptr.p_int[68] = 0;
    a.ptr.p_double[69] = 0.530;
    c.ptr.p_int[69] = 0;
    a.ptr.p_double[70] = 0.540;
    c.ptr.p_int[70] = 0;
    a.ptr.p_double[71] = 0.560;
    c.ptr.p_int[71] = 0;
    a.ptr.p_double[72] = 0.560;
    c.ptr.p_int[72] = 0;
    a.ptr.p_double[73] = 0.570;
    c.ptr.p_int[73] = 0;
    a.ptr.p_double[74] = 0.600;
    c.ptr.p_int[74] = 0;
    a.ptr.p_double[75] = 0.600;
    c.ptr.p_int[75] = 0;
    a.ptr.p_double[76] = 0.620;
    c.ptr.p_int[76] = 0;
    a.ptr.p_double[77] = 0.650;
    c.ptr.p_int[77] = 0;
    a.ptr.p_double[78] = 0.660;
    c.ptr.p_int[78] = 0;
    a.ptr.p_double[79] = 0.680;
    c.ptr.p_int[79] = 0;
    a.ptr.p_double[80] = 0.700;
    c.ptr.p_int[80] = 0;
    a.ptr.p_double[81] = 0.750;
    c.ptr.p_int[81] = 0;
    a.ptr.p_double[82] = 0.770;
    c.ptr.p_int[82] = 0;
    a.ptr.p_double[83] = 0.770;
    c.ptr.p_int[83] = 0;
    a.ptr.p_double[84] = 0.770;
    c.ptr.p_int[84] = 0;
    a.ptr.p_double[85] = 0.790;
    c.ptr.p_int[85] = 0;
    a.ptr.p_double[86] = 0.810;
    c.ptr.p_int[86] = 0;
    a.ptr.p_double[87] = 0.840;
    c.ptr.p_int[87] = 0;
    a.ptr.p_double[88] = 0.860;
    c.ptr.p_int[88] = 0;
    a.ptr.p_double[89] = 0.870;
    c.ptr.p_int[89] = 0;
    a.ptr.p_double[90] = 0.890;
    c.ptr.p_int[90] = 0;
    a.ptr.p_double[91] = 0.900;
    c.ptr.p_int[91] = 1;
    a.ptr.p_double[92] = 0.900;
    c.ptr.p_int[92] = 0;
    a.ptr.p_double[93] = 0.910;
    c.ptr.p_int[93] = 0;
    a.ptr.p_double[94] = 0.940;
    c.ptr.p_int[94] = 0;
    a.ptr.p_double[95] = 0.950;
    c.ptr.p_int[95] = 0;
    a.ptr.p_double[96] = 0.952;
    c.ptr.p_int[96] = 0;
    a.ptr.p_double[97] = 0.970;
    c.ptr.p_int[97] = 0;
    a.ptr.p_double[98] = 0.970;
    c.ptr.p_int[98] = 0;
    a.ptr.p_double[99] = 0.980;
    c.ptr.p_int[99] = 0;
    a.ptr.p_double[100] = 1.000;
    c.ptr.p_int[100] = 0;
    a.ptr.p_double[101] = 1.000;
    c.ptr.p_int[101] = 0;
    a.ptr.p_double[102] = 1.000;
    c.ptr.p_int[102] = 0;
    a.ptr.p_double[103] = 1.000;
    c.ptr.p_int[103] = 0;
    a.ptr.p_double[104] = 1.000;
    c.ptr.p_int[104] = 0;
    a.ptr.p_double[105] = 1.020;
    c.ptr.p_int[105] = 0;
    a.ptr.p_double[106] = 1.090;
    c.ptr.p_int[106] = 0;
    a.ptr.p_double[107] = 1.130;
    c.ptr.p_int[107] = 0;
    a.ptr.p_double[108] = 1.840;
    c.ptr.p_int[108] = 0;
    a.ptr.p_double[109] = 2.470;
    c.ptr.p_int[109] = 0;
    dsoptimalsplit2(&a, &c, n, &info, &threshold, &pal, &pbl, &par, &pbr, &cve, _state);
    if( info!=1 )
    {
        split2errors = ae_true;
    }
    else
    {
        split2errors = split2errors||ae_fp_greater(ae_fabs(threshold-0.195, _state),100*ae_machineepsilon);
        split2errors = split2errors||ae_fp_greater(ae_fabs(pal-0.80, _state),0.02);
        split2errors = split2errors||ae_fp_greater(ae_fabs(pbl-0.20, _state),0.02);
        split2errors = split2errors||ae_fp_greater(ae_fabs(par-0.97, _state),0.02);
        split2errors = split2errors||ae_fp_greater(ae_fabs(pbr-0.03, _state),0.02);
    }
    
    /*
     * split-2 fast
     */
    
    /*
     * General tests for different N's
     */
    for(n=1; n<=maxn; n++)
    {
        ae_vector_set_length(&a, n-1+1, _state);
        ae_vector_set_length(&c, n-1+1, _state);
        ae_vector_set_length(&tiebuf, n+1, _state);
        ae_vector_set_length(&cntbuf, 3+1, _state);
        
        /*
         * one-tie test
         */
        if( n%2==0 )
        {
            for(i=0; i<=n-1; i++)
            {
                a.ptr.p_double[i] = (double)(n);
                c.ptr.p_int[i] = i%2;
            }
            dsoptimalsplit2fast(&a, &c, &tiebuf, &cntbuf, &sortrbuf, &sortibuf, n, 2, 0.00, &info, &threshold, &rms, &cvrms, _state);
            if( info!=-3 )
            {
                split2errors = ae_true;
                continue;
            }
        }
        
        /*
         * two-tie test
         */
        
        /*
         * test #1
         */
        if( n>1 )
        {
            for(i=0; i<=n-1; i++)
            {
                a.ptr.p_double[i] = (double)(i/((n+1)/2));
                c.ptr.p_int[i] = i/((n+1)/2);
            }
            dsoptimalsplit2fast(&a, &c, &tiebuf, &cntbuf, &sortrbuf, &sortibuf, n, 2, 0.00, &info, &threshold, &rms, &cvrms, _state);
            if( info!=1 )
            {
                split2errors = ae_true;
                continue;
            }
            split2errors = split2errors||ae_fp_greater(ae_fabs(threshold-0.5, _state),100*ae_machineepsilon);
            split2errors = split2errors||ae_fp_greater(ae_fabs(rms-0, _state),100*ae_machineepsilon);
            if( n==2 )
            {
                split2errors = split2errors||ae_fp_greater(ae_fabs(cvrms-0.5, _state),100*ae_machineepsilon);
            }
            else
            {
                if( n==3 )
                {
                    split2errors = split2errors||ae_fp_greater(ae_fabs(cvrms-ae_sqrt((2*0+2*0+2*0.25)/6, _state), _state),100*ae_machineepsilon);
                }
                else
                {
                    split2errors = split2errors||ae_fp_greater(ae_fabs(cvrms, _state),100*ae_machineepsilon);
                }
            }
        }
    }
    
    /*
     * special tests
     */
    n = 10;
    ae_vector_set_length(&a, n-1+1, _state);
    ae_vector_set_length(&c, n-1+1, _state);
    ae_vector_set_length(&tiebuf, n+1, _state);
    ae_vector_set_length(&cntbuf, 2*3-1+1, _state);
    for(i=0; i<=n-1; i++)
    {
        a.ptr.p_double[i] = (double)(i);
        if( i<=n-3 )
        {
            c.ptr.p_int[i] = 0;
        }
        else
        {
            c.ptr.p_int[i] = i-(n-3);
        }
    }
    dsoptimalsplit2fast(&a, &c, &tiebuf, &cntbuf, &sortrbuf, &sortibuf, n, 3, 0.00, &info, &threshold, &rms, &cvrms, _state);
    if( info!=1 )
    {
        split2errors = ae_true;
    }
    else
    {
        split2errors = split2errors||ae_fp_greater(ae_fabs(threshold-(n-2.5), _state),100*ae_machineepsilon);
        split2errors = split2errors||ae_fp_greater(ae_fabs(rms-ae_sqrt((0.25+0.25+0.25+0.25)/(3*n), _state), _state),100*ae_machineepsilon);
        split2errors = split2errors||ae_fp_greater(ae_fabs(cvrms-ae_sqrt((double)(1+1+1+1)/(double)(3*n), _state), _state),100*ae_machineepsilon);
    }
    
    /*
     * Optimal split-K
     */
    
    /*
     * General tests for different N's
     */
    for(n=1; n<=maxnq; n++)
    {
        ae_vector_set_length(&a, n-1+1, _state);
        ae_vector_set_length(&c, n-1+1, _state);
        
        /*
         * one-tie test
         */
        if( n%2==0 )
        {
            for(i=0; i<=n-1; i++)
            {
                a.ptr.p_double[i] = (double)(n);
                c.ptr.p_int[i] = i%2;
            }
            dsoptimalsplitk(&a, &c, n, 2, 2+ae_randominteger(5, _state), &info, &thresholds, &ni, &cve, _state);
            if( info!=-3 )
            {
                optimalsplitkerrors = ae_true;
                continue;
            }
        }
        
        /*
         * two-tie test
         */
        
        /*
         * test #1
         */
        if( n>1 )
        {
            c0 = 0;
            c1 = 0;
            for(i=0; i<=n-1; i++)
            {
                a.ptr.p_double[i] = (double)(i/((n+1)/2));
                c.ptr.p_int[i] = i/((n+1)/2);
                if( c.ptr.p_int[i]==0 )
                {
                    c0 = c0+1;
                }
                if( c.ptr.p_int[i]==1 )
                {
                    c1 = c1+1;
                }
            }
            dsoptimalsplitk(&a, &c, n, 2, 2+ae_randominteger(5, _state), &info, &thresholds, &ni, &cve, _state);
            if( info!=1 )
            {
                optimalsplitkerrors = ae_true;
                continue;
            }
            optimalsplitkerrors = optimalsplitkerrors||ni!=2;
            optimalsplitkerrors = optimalsplitkerrors||ae_fp_greater(ae_fabs(thresholds.ptr.p_double[0]-0.5, _state),100*ae_machineepsilon);
            optimalsplitkerrors = optimalsplitkerrors||ae_fp_greater(ae_fabs(cve-(-c0*ae_log((double)c0/(double)(c0+1), _state)-c1*ae_log((double)c1/(double)(c1+1), _state)), _state),100*ae_machineepsilon);
        }
        
        /*
         * test #2
         */
        if( n>2 )
        {
            c0 = 1+ae_randominteger(n-1, _state);
            c1 = n-c0;
            for(i=0; i<=n-1; i++)
            {
                if( i<c0 )
                {
                    a.ptr.p_double[i] = (double)(0);
                    c.ptr.p_int[i] = 0;
                }
                else
                {
                    a.ptr.p_double[i] = (double)(1);
                    c.ptr.p_int[i] = 1;
                }
            }
            dsoptimalsplitk(&a, &c, n, 2, 2+ae_randominteger(5, _state), &info, &thresholds, &ni, &cve, _state);
            if( info!=1 )
            {
                optimalsplitkerrors = ae_true;
                continue;
            }
            optimalsplitkerrors = optimalsplitkerrors||ni!=2;
            optimalsplitkerrors = optimalsplitkerrors||ae_fp_greater(ae_fabs(thresholds.ptr.p_double[0]-0.5, _state),100*ae_machineepsilon);
            optimalsplitkerrors = optimalsplitkerrors||ae_fp_greater(ae_fabs(cve-(-c0*ae_log((double)c0/(double)(c0+1), _state)-c1*ae_log((double)c1/(double)(c1+1), _state)), _state),100*ae_machineepsilon);
        }
        
        /*
         * multi-tie test
         */
        if( n>=16 )
        {
            
            /*
             * Multi-tie test.
             *
             * First NC-1 ties have C0 entries, remaining NC-th tie
             * have C1 entries.
             */
            nc = ae_round(ae_sqrt((double)(n), _state), _state);
            c0 = n/nc;
            c1 = n-c0*(nc-1);
            for(i=0; i<=nc-2; i++)
            {
                for(j=c0*i; j<=c0*(i+1)-1; j++)
                {
                    a.ptr.p_double[j] = (double)(j);
                    c.ptr.p_int[j] = i;
                }
            }
            for(j=c0*(nc-1); j<=n-1; j++)
            {
                a.ptr.p_double[j] = (double)(j);
                c.ptr.p_int[j] = nc-1;
            }
            dsoptimalsplitk(&a, &c, n, nc, nc+ae_randominteger(nc, _state), &info, &thresholds, &ni, &cve, _state);
            if( info!=1 )
            {
                optimalsplitkerrors = ae_true;
                continue;
            }
            optimalsplitkerrors = optimalsplitkerrors||ni!=nc;
            if( ni==nc )
            {
                for(i=0; i<=nc-2; i++)
                {
                    optimalsplitkerrors = optimalsplitkerrors||ae_fp_greater(ae_fabs(thresholds.ptr.p_double[i]-(c0*(i+1)-1+0.5), _state),100*ae_machineepsilon);
                }
                cvr = -((nc-1)*c0*ae_log((double)c0/(double)(c0+nc-1), _state)+c1*ae_log((double)c1/(double)(c1+nc-1), _state));
                optimalsplitkerrors = optimalsplitkerrors||ae_fp_greater(ae_fabs(cve-cvr, _state),100*ae_machineepsilon);
            }
        }
    }
    
    /*
     * Non-optimal split-K
     */
    
    /*
     * General tests for different N's
     */
    for(n=1; n<=maxnq; n++)
    {
        ae_vector_set_length(&a, n-1+1, _state);
        ae_vector_set_length(&c, n-1+1, _state);
        
        /*
         * one-tie test
         */
        if( n%2==0 )
        {
            for(i=0; i<=n-1; i++)
            {
                a.ptr.p_double[i] = (double)(99);
                c.ptr.p_int[i] = i%2;
            }
            dssplitk(&a, &c, n, 2, 2+ae_randominteger(5, _state), &info, &thresholds, &ni, &cve, _state);
            if( info!=-3 )
            {
                splitkerrors = ae_true;
                continue;
            }
        }
        
        /*
         * two-tie test
         */
        
        /*
         * test #1
         */
        if( n>1 )
        {
            c0 = 0;
            c1 = 0;
            for(i=0; i<=n-1; i++)
            {
                a.ptr.p_double[i] = (double)(i/((n+1)/2));
                c.ptr.p_int[i] = i/((n+1)/2);
                if( c.ptr.p_int[i]==0 )
                {
                    c0 = c0+1;
                }
                if( c.ptr.p_int[i]==1 )
                {
                    c1 = c1+1;
                }
            }
            dssplitk(&a, &c, n, 2, 2+ae_randominteger(5, _state), &info, &thresholds, &ni, &cve, _state);
            if( info!=1 )
            {
                splitkerrors = ae_true;
                continue;
            }
            splitkerrors = splitkerrors||ni!=2;
            if( ni==2 )
            {
                splitkerrors = splitkerrors||ae_fp_greater(ae_fabs(thresholds.ptr.p_double[0]-0.5, _state),100*ae_machineepsilon);
                splitkerrors = splitkerrors||ae_fp_greater(ae_fabs(cve-(-c0*ae_log((double)c0/(double)(c0+1), _state)-c1*ae_log((double)c1/(double)(c1+1), _state)), _state),100*ae_machineepsilon);
            }
        }
        
        /*
         * test #2
         */
        if( n>2 )
        {
            c0 = 1+ae_randominteger(n-1, _state);
            c1 = n-c0;
            for(i=0; i<=n-1; i++)
            {
                if( i<c0 )
                {
                    a.ptr.p_double[i] = (double)(0);
                    c.ptr.p_int[i] = 0;
                }
                else
                {
                    a.ptr.p_double[i] = (double)(1);
                    c.ptr.p_int[i] = 1;
                }
            }
            dssplitk(&a, &c, n, 2, 2+ae_randominteger(5, _state), &info, &thresholds, &ni, &cve, _state);
            if( info!=1 )
            {
                splitkerrors = ae_true;
                continue;
            }
            splitkerrors = splitkerrors||ni!=2;
            if( ni==2 )
            {
                splitkerrors = splitkerrors||ae_fp_greater(ae_fabs(thresholds.ptr.p_double[0]-0.5, _state),100*ae_machineepsilon);
                splitkerrors = splitkerrors||ae_fp_greater(ae_fabs(cve-(-c0*ae_log((double)c0/(double)(c0+1), _state)-c1*ae_log((double)c1/(double)(c1+1), _state)), _state),100*ae_machineepsilon);
            }
        }
        
        /*
         * multi-tie test
         */
        for(c0=4; c0<=n; c0++)
        {
            if( (n%c0==0&&n/c0<=c0)&&n/c0>1 )
            {
                nc = n/c0;
                for(i=0; i<=nc-1; i++)
                {
                    for(j=c0*i; j<=c0*(i+1)-1; j++)
                    {
                        a.ptr.p_double[j] = (double)(j);
                        c.ptr.p_int[j] = i;
                    }
                }
                dssplitk(&a, &c, n, nc, nc+ae_randominteger(nc, _state), &info, &thresholds, &ni, &cve, _state);
                if( info!=1 )
                {
                    splitkerrors = ae_true;
                    continue;
                }
                splitkerrors = splitkerrors||ni!=nc;
                if( ni==nc )
                {
                    for(i=0; i<=nc-2; i++)
                    {
                        splitkerrors = splitkerrors||ae_fp_greater(ae_fabs(thresholds.ptr.p_double[i]-(c0*(i+1)-1+0.5), _state),100*ae_machineepsilon);
                    }
                    cvr = -nc*c0*ae_log((double)c0/(double)(c0+nc-1), _state);
                    splitkerrors = splitkerrors||ae_fp_greater(ae_fabs(cve-cvr, _state),100*ae_machineepsilon);
                }
            }
        }
    }
    
    /*
     * report
     */
    waserrors = ((tieserrors||split2errors)||optimalsplitkerrors)||splitkerrors;
    if( !silent )
    {
        printf("TESTING BASIC DATASET SUBROUTINES\n");
        printf("TIES:                               ");
        if( !tieserrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("SPLIT-2:                            ");
        if( !split2errors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("OPTIMAL SPLIT-K:                    ");
        if( !optimalsplitkerrors )
        {
            printf("OK\n");
        }
        else
        {
            printf("FAILED\n");
        }
        printf("SPLIT-K:                            ");
        if( !splitkerrors )
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
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testbdss(ae_bool silent, ae_state *_state)
{
    return testbdss(silent, _state);
}


/*************************************************************************
Unsets 1D array.
*************************************************************************/
static void testbdssunit_unset1di(/* Integer */ ae_vector* a,
     ae_state *_state)
{


    ae_vector_set_length(a, 0+1, _state);
    a->ptr.p_int[0] = ae_randominteger(3, _state)-1;
}


/*$ End $*/
