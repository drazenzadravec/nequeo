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
#include "testautogkunit.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Test
*************************************************************************/
ae_bool testautogk(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    double a;
    double b;
    autogkstate state;
    autogkreport rep;
    double v;
    double exact;
    double eabs;
    double alpha;
    ae_int_t pkind;
    double errtol;
    ae_bool simpleerrors;
    ae_bool sngenderrors;
    ae_bool waserrors;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    _autogkstate_init(&state, _state);
    _autogkreport_init(&rep, _state);

    simpleerrors = ae_false;
    sngenderrors = ae_false;
    waserrors = ae_false;
    errtol = 10000*ae_machineepsilon;
    
    /*
     * Simple test: integral(exp(x),+-1,+-2), no maximum width requirements
     */
    a = (2*ae_randominteger(2, _state)-1)*1.0;
    b = (2*ae_randominteger(2, _state)-1)*2.0;
    autogksmooth(a, b, &state, _state);
    while(autogkiteration(&state, _state))
    {
        state.f = ae_exp(state.x, _state);
    }
    autogkresults(&state, &v, &rep, _state);
    exact = ae_exp(b, _state)-ae_exp(a, _state);
    eabs = ae_fabs(ae_exp(b, _state)-ae_exp(a, _state), _state);
    if( rep.terminationtype<=0 )
    {
        simpleerrors = ae_true;
    }
    else
    {
        simpleerrors = simpleerrors||ae_fp_greater(ae_fabs(exact-v, _state),errtol*eabs);
    }
    
    /*
     * Simple test: integral(exp(x),+-1,+-2), XWidth=0.1
     */
    a = (2*ae_randominteger(2, _state)-1)*1.0;
    b = (2*ae_randominteger(2, _state)-1)*2.0;
    autogksmoothw(a, b, 0.1, &state, _state);
    while(autogkiteration(&state, _state))
    {
        state.f = ae_exp(state.x, _state);
    }
    autogkresults(&state, &v, &rep, _state);
    exact = ae_exp(b, _state)-ae_exp(a, _state);
    eabs = ae_fabs(ae_exp(b, _state)-ae_exp(a, _state), _state);
    if( rep.terminationtype<=0 )
    {
        simpleerrors = ae_true;
    }
    else
    {
        simpleerrors = simpleerrors||ae_fp_greater(ae_fabs(exact-v, _state),errtol*eabs);
    }
    
    /*
     * Simple test: integral(cos(100*x),0,2*pi), no maximum width requirements
     */
    a = (double)(0);
    b = 2*ae_pi;
    autogksmooth(a, b, &state, _state);
    while(autogkiteration(&state, _state))
    {
        state.f = ae_cos(100*state.x, _state);
    }
    autogkresults(&state, &v, &rep, _state);
    exact = (double)(0);
    eabs = (double)(4);
    if( rep.terminationtype<=0 )
    {
        simpleerrors = ae_true;
    }
    else
    {
        simpleerrors = simpleerrors||ae_fp_greater(ae_fabs(exact-v, _state),errtol*eabs);
    }
    
    /*
     * Simple test: integral(cos(100*x),0,2*pi), XWidth=0.3
     */
    a = (double)(0);
    b = 2*ae_pi;
    autogksmoothw(a, b, 0.3, &state, _state);
    while(autogkiteration(&state, _state))
    {
        state.f = ae_cos(100*state.x, _state);
    }
    autogkresults(&state, &v, &rep, _state);
    exact = (double)(0);
    eabs = (double)(4);
    if( rep.terminationtype<=0 )
    {
        simpleerrors = ae_true;
    }
    else
    {
        simpleerrors = simpleerrors||ae_fp_greater(ae_fabs(exact-v, _state),errtol*eabs);
    }
    
    /*
     * singular problem on [a,b] = [0.1, 0.5]
     *     f2(x) = (1+x)*(b-x)^alpha, -1 < alpha < 1
     */
    for(pkind=0; pkind<=6; pkind++)
    {
        a = 0.1;
        b = 0.5;
        alpha = 0.0;
        if( pkind==0 )
        {
            alpha = -0.9;
        }
        if( pkind==1 )
        {
            alpha = -0.5;
        }
        if( pkind==2 )
        {
            alpha = -0.1;
        }
        if( pkind==3 )
        {
            alpha = 0.0;
        }
        if( pkind==4 )
        {
            alpha = 0.1;
        }
        if( pkind==5 )
        {
            alpha = 0.5;
        }
        if( pkind==6 )
        {
            alpha = 0.9;
        }
        
        /*
         * f1(x) = (1+x)*(x-a)^alpha, -1 < alpha < 1
         * 1. use singular integrator for [a,b]
         * 2. use singular integrator for [b,a]
         */
        exact = ae_pow(b-a, alpha+2, _state)/(alpha+2)+(1+a)*ae_pow(b-a, alpha+1, _state)/(alpha+1);
        eabs = ae_fabs(exact, _state);
        autogksingular(a, b, alpha, 0.0, &state, _state);
        while(autogkiteration(&state, _state))
        {
            if( ae_fp_less(state.xminusa,0.01) )
            {
                state.f = ae_pow(state.xminusa, alpha, _state)*(1+state.x);
            }
            else
            {
                state.f = ae_pow(state.x-a, alpha, _state)*(1+state.x);
            }
        }
        autogkresults(&state, &v, &rep, _state);
        if( rep.terminationtype<=0 )
        {
            sngenderrors = ae_true;
        }
        else
        {
            sngenderrors = sngenderrors||ae_fp_greater(ae_fabs(v-exact, _state),errtol*eabs);
        }
        autogksingular(b, a, 0.0, alpha, &state, _state);
        while(autogkiteration(&state, _state))
        {
            if( ae_fp_greater(state.bminusx,-0.01) )
            {
                state.f = ae_pow(-state.bminusx, alpha, _state)*(1+state.x);
            }
            else
            {
                state.f = ae_pow(state.x-a, alpha, _state)*(1+state.x);
            }
        }
        autogkresults(&state, &v, &rep, _state);
        if( rep.terminationtype<=0 )
        {
            sngenderrors = ae_true;
        }
        else
        {
            sngenderrors = sngenderrors||ae_fp_greater(ae_fabs(-v-exact, _state),errtol*eabs);
        }
        
        /*
         * f1(x) = (1+x)*(b-x)^alpha, -1 < alpha < 1
         * 1. use singular integrator for [a,b]
         * 2. use singular integrator for [b,a]
         */
        exact = (1+b)*ae_pow(b-a, alpha+1, _state)/(alpha+1)-ae_pow(b-a, alpha+2, _state)/(alpha+2);
        eabs = ae_fabs(exact, _state);
        autogksingular(a, b, 0.0, alpha, &state, _state);
        while(autogkiteration(&state, _state))
        {
            if( ae_fp_less(state.bminusx,0.01) )
            {
                state.f = ae_pow(state.bminusx, alpha, _state)*(1+state.x);
            }
            else
            {
                state.f = ae_pow(b-state.x, alpha, _state)*(1+state.x);
            }
        }
        autogkresults(&state, &v, &rep, _state);
        if( rep.terminationtype<=0 )
        {
            sngenderrors = ae_true;
        }
        else
        {
            sngenderrors = sngenderrors||ae_fp_greater(ae_fabs(v-exact, _state),errtol*eabs);
        }
        autogksingular(b, a, alpha, 0.0, &state, _state);
        while(autogkiteration(&state, _state))
        {
            if( ae_fp_greater(state.xminusa,-0.01) )
            {
                state.f = ae_pow(-state.xminusa, alpha, _state)*(1+state.x);
            }
            else
            {
                state.f = ae_pow(b-state.x, alpha, _state)*(1+state.x);
            }
        }
        autogkresults(&state, &v, &rep, _state);
        if( rep.terminationtype<=0 )
        {
            sngenderrors = ae_true;
        }
        else
        {
            sngenderrors = sngenderrors||ae_fp_greater(ae_fabs(-v-exact, _state),errtol*eabs);
        }
    }
    
    /*
     * end
     */
    waserrors = simpleerrors||sngenderrors;
    if( !silent )
    {
        printf("TESTING AUTOGK\n");
        printf("INTEGRATION WITH GIVEN ACCURACY:          ");
        if( simpleerrors||sngenderrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* SIMPLE PROBLEMS:                        ");
        if( simpleerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("* SINGULAR PROBLEMS (ENDS OF INTERVAL):   ");
        if( sngenderrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
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
ae_bool _pexec_testautogk(ae_bool silent, ae_state *_state)
{
    return testautogk(silent, _state);
}


/*$ End $*/
