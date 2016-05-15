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
#include "testgammafuncunit.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool testgammafunc(ae_bool silent, ae_state *_state)
{
    double threshold;
    double v;
    double s;
    ae_bool waserrors;
    ae_bool gammaerrors;
    ae_bool lngammaerrors;
    ae_bool result;


    gammaerrors = ae_false;
    lngammaerrors = ae_false;
    waserrors = ae_false;
    threshold = 100*ae_machineepsilon;
    
    /*
     *
     */
    gammaerrors = gammaerrors||ae_fp_greater(ae_fabs(gammafunction(0.5, _state)-ae_sqrt(ae_pi, _state), _state),threshold);
    gammaerrors = gammaerrors||ae_fp_greater(ae_fabs(gammafunction(1.5, _state)-0.5*ae_sqrt(ae_pi, _state), _state),threshold);
    v = lngamma(0.5, &s, _state);
    lngammaerrors = (lngammaerrors||ae_fp_greater(ae_fabs(v-ae_log(ae_sqrt(ae_pi, _state), _state), _state),threshold))||ae_fp_neq(s,(double)(1));
    v = lngamma(1.5, &s, _state);
    lngammaerrors = (lngammaerrors||ae_fp_greater(ae_fabs(v-ae_log(0.5*ae_sqrt(ae_pi, _state), _state), _state),threshold))||ae_fp_neq(s,(double)(1));
    
    /*
     * report
     */
    waserrors = gammaerrors||lngammaerrors;
    if( !silent )
    {
        printf("TESTING GAMMA FUNCTION\n");
        printf("GAMMA:                                   ");
        if( gammaerrors )
        {
            printf("FAILED\n");
        }
        else
        {
            printf("OK\n");
        }
        printf("LN GAMMA:                                ");
        if( lngammaerrors )
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
    
    /*
     * end
     */
    result = !waserrors;
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testgammafunc(ae_bool silent, ae_state *_state)
{
    return testgammafunc(silent, _state);
}


/*$ End $*/
