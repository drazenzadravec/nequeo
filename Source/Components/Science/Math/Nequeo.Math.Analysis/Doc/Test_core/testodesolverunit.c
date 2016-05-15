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
#include "testodesolverunit.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Test
*************************************************************************/
ae_bool testodesolver(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t passcount;
    ae_bool curerrors;
    ae_bool rkckerrors;
    ae_bool waserrors;
    ae_vector xtbl;
    ae_matrix ytbl;
    odesolverreport rep;
    ae_vector xg;
    ae_vector y;
    double h;
    double eps;
    ae_int_t solver;
    ae_int_t pass;
    ae_int_t mynfev;
    double v;
    ae_int_t m;
    ae_int_t m2;
    ae_int_t i;
    double err;
    odesolverstate state;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&xtbl, 0, DT_REAL, _state);
    ae_matrix_init(&ytbl, 0, 0, DT_REAL, _state);
    _odesolverreport_init(&rep, _state);
    ae_vector_init(&xg, 0, DT_REAL, _state);
    ae_vector_init(&y, 0, DT_REAL, _state);
    _odesolverstate_init(&state, _state);

    rkckerrors = ae_false;
    waserrors = ae_false;
    passcount = 10;
    
    /*
     * simple test: just A*sin(x)+B*cos(x)
     */
    ae_assert(passcount>=2, "Assertion failed", _state);
    for(pass=0; pass<=passcount-1; pass++)
    {
        for(solver=0; solver<=0; solver++)
        {
            
            /*
             * prepare
             */
            h = 1.0E-2;
            eps = 1.0E-5;
            if( pass%2==0 )
            {
                eps = -eps;
            }
            ae_vector_set_length(&y, 2, _state);
            for(i=0; i<=1; i++)
            {
                y.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
            }
            m = 2+ae_randominteger(10, _state);
            ae_vector_set_length(&xg, m, _state);
            xg.ptr.p_double[0] = (m-1)*ae_randomreal(_state);
            for(i=1; i<=m-1; i++)
            {
                xg.ptr.p_double[i] = xg.ptr.p_double[i-1]+ae_randomreal(_state);
            }
            v = 2*ae_pi/(xg.ptr.p_double[m-1]-xg.ptr.p_double[0]);
            ae_v_muld(&xg.ptr.p_double[0], 1, ae_v_len(0,m-1), v);
            if( ae_fp_greater(ae_randomreal(_state),0.5) )
            {
                ae_v_muld(&xg.ptr.p_double[0], 1, ae_v_len(0,m-1), -1);
            }
            mynfev = 0;
            
            /*
             * choose solver
             */
            if( solver==0 )
            {
                odesolverrkck(&y, 2, &xg, m, eps, h, &state, _state);
            }
            
            /*
             * solve
             */
            while(odesolveriteration(&state, _state))
            {
                state.dy.ptr.p_double[0] = state.y.ptr.p_double[1];
                state.dy.ptr.p_double[1] = -state.y.ptr.p_double[0];
                mynfev = mynfev+1;
            }
            odesolverresults(&state, &m2, &xtbl, &ytbl, &rep, _state);
            
            /*
             * check results
             */
            curerrors = ae_false;
            if( rep.terminationtype<=0 )
            {
                curerrors = ae_true;
            }
            else
            {
                curerrors = curerrors||m2!=m;
                err = (double)(0);
                for(i=0; i<=m-1; i++)
                {
                    err = ae_maxreal(err, ae_fabs(ytbl.ptr.pp_double[i][0]-(y.ptr.p_double[0]*ae_cos(xtbl.ptr.p_double[i]-xtbl.ptr.p_double[0], _state)+y.ptr.p_double[1]*ae_sin(xtbl.ptr.p_double[i]-xtbl.ptr.p_double[0], _state)), _state), _state);
                    err = ae_maxreal(err, ae_fabs(ytbl.ptr.pp_double[i][1]-(-y.ptr.p_double[0]*ae_sin(xtbl.ptr.p_double[i]-xtbl.ptr.p_double[0], _state)+y.ptr.p_double[1]*ae_cos(xtbl.ptr.p_double[i]-xtbl.ptr.p_double[0], _state)), _state), _state);
                }
                curerrors = curerrors||ae_fp_greater(err,10*ae_fabs(eps, _state));
                curerrors = curerrors||mynfev!=rep.nfev;
            }
            if( solver==0 )
            {
                rkckerrors = rkckerrors||curerrors;
            }
        }
    }
    
    /*
     * another test:
     *
     *     y(0)   = 0
     *     dy/dx  = f(x,y)
     *     f(x,y) = 0,   x<1
     *              x-1, x>=1
     *
     * with BOTH absolute and fractional tolerances.
     * Starting from zero will be real challenge for
     * fractional tolerance.
     */
    ae_assert(passcount>=2, "Assertion failed", _state);
    for(pass=0; pass<=passcount-1; pass++)
    {
        h = 1.0E-4;
        eps = 1.0E-4;
        if( pass%2==0 )
        {
            eps = -eps;
        }
        ae_vector_set_length(&y, 1, _state);
        y.ptr.p_double[0] = (double)(0);
        m = 21;
        ae_vector_set_length(&xg, m, _state);
        for(i=0; i<=m-1; i++)
        {
            xg.ptr.p_double[i] = (double)(2*i)/(double)(m-1);
        }
        mynfev = 0;
        odesolverrkck(&y, 1, &xg, m, eps, h, &state, _state);
        while(odesolveriteration(&state, _state))
        {
            state.dy.ptr.p_double[0] = ae_maxreal(state.x-1, (double)(0), _state);
            mynfev = mynfev+1;
        }
        odesolverresults(&state, &m2, &xtbl, &ytbl, &rep, _state);
        if( rep.terminationtype<=0 )
        {
            rkckerrors = ae_true;
        }
        else
        {
            rkckerrors = rkckerrors||m2!=m;
            err = (double)(0);
            for(i=0; i<=m-1; i++)
            {
                err = ae_maxreal(err, ae_fabs(ytbl.ptr.pp_double[i][0]-ae_sqr(ae_maxreal(xg.ptr.p_double[i]-1, (double)(0), _state), _state)/2, _state), _state);
            }
            rkckerrors = rkckerrors||ae_fp_greater(err,ae_fabs(eps, _state));
            rkckerrors = rkckerrors||mynfev!=rep.nfev;
        }
    }
    
    /*
     * end
     */
    waserrors = rkckerrors;
    if( !silent )
    {
        printf("TESTING ODE SOLVER\n");
        printf("* RK CASH-KARP:                           ");
        if( rkckerrors )
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
    }
    result = !waserrors;
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testodesolver(ae_bool silent, ae_state *_state)
{
    return testodesolver(silent, _state);
}


/*$ End $*/
