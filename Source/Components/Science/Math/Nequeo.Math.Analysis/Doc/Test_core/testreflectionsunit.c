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
#include "testreflectionsunit.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool testreflections(ae_bool silent, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_int_t n;
    ae_int_t m;
    ae_int_t maxmn;
    ae_vector x;
    ae_vector v;
    ae_vector work;
    ae_matrix h;
    ae_matrix a;
    ae_matrix b;
    ae_matrix c;
    double tmp;
    double beta;
    double tau;
    double err;
    double mer;
    double mel;
    double meg;
    ae_int_t pass;
    ae_int_t passcount;
    double threshold;
    ae_int_t tasktype;
    double xscale;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_REAL, _state);
    ae_vector_init(&v, 0, DT_REAL, _state);
    ae_vector_init(&work, 0, DT_REAL, _state);
    ae_matrix_init(&h, 0, 0, DT_REAL, _state);
    ae_matrix_init(&a, 0, 0, DT_REAL, _state);
    ae_matrix_init(&b, 0, 0, DT_REAL, _state);
    ae_matrix_init(&c, 0, 0, DT_REAL, _state);

    passcount = 10;
    threshold = 100*ae_machineepsilon;
    mer = (double)(0);
    mel = (double)(0);
    meg = (double)(0);
    for(pass=1; pass<=passcount; pass++)
    {
        for(n=1; n<=10; n++)
        {
            for(m=1; m<=10; m++)
            {
                
                /*
                 * Task
                 */
                n = 1+ae_randominteger(10, _state);
                m = 1+ae_randominteger(10, _state);
                maxmn = ae_maxint(m, n, _state);
                
                /*
                 * Initialize
                 */
                ae_vector_set_length(&x, maxmn+1, _state);
                ae_vector_set_length(&v, maxmn+1, _state);
                ae_vector_set_length(&work, maxmn+1, _state);
                ae_matrix_set_length(&h, maxmn+1, maxmn+1, _state);
                ae_matrix_set_length(&a, maxmn+1, maxmn+1, _state);
                ae_matrix_set_length(&b, maxmn+1, maxmn+1, _state);
                ae_matrix_set_length(&c, maxmn+1, maxmn+1, _state);
                
                /*
                 * GenerateReflection, three tasks are possible:
                 * * random X
                 * * zero X
                 * * non-zero X[1], all other are zeros
                 * * random X, near underflow scale
                 * * random X, near overflow scale
                 */
                for(tasktype=0; tasktype<=4; tasktype++)
                {
                    xscale = (double)(1);
                    if( tasktype==0 )
                    {
                        for(i=1; i<=n; i++)
                        {
                            x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                        }
                    }
                    if( tasktype==1 )
                    {
                        for(i=1; i<=n; i++)
                        {
                            x.ptr.p_double[i] = (double)(0);
                        }
                    }
                    if( tasktype==2 )
                    {
                        x.ptr.p_double[1] = 2*ae_randomreal(_state)-1;
                        for(i=2; i<=n; i++)
                        {
                            x.ptr.p_double[i] = (double)(0);
                        }
                    }
                    if( tasktype==3 )
                    {
                        for(i=1; i<=n; i++)
                        {
                            x.ptr.p_double[i] = (ae_randominteger(21, _state)-10)*ae_minrealnumber;
                        }
                        xscale = 10*ae_minrealnumber;
                    }
                    if( tasktype==4 )
                    {
                        for(i=1; i<=n; i++)
                        {
                            x.ptr.p_double[i] = (2*ae_randomreal(_state)-1)*ae_maxrealnumber;
                        }
                        xscale = ae_maxrealnumber;
                    }
                    ae_v_move(&v.ptr.p_double[1], 1, &x.ptr.p_double[1], 1, ae_v_len(1,n));
                    generatereflection(&v, n, &tau, _state);
                    beta = v.ptr.p_double[1];
                    v.ptr.p_double[1] = (double)(1);
                    for(i=1; i<=n; i++)
                    {
                        for(j=1; j<=n; j++)
                        {
                            if( i==j )
                            {
                                h.ptr.pp_double[i][j] = 1-tau*v.ptr.p_double[i]*v.ptr.p_double[j];
                            }
                            else
                            {
                                h.ptr.pp_double[i][j] = -tau*v.ptr.p_double[i]*v.ptr.p_double[j];
                            }
                        }
                    }
                    err = (double)(0);
                    for(i=1; i<=n; i++)
                    {
                        tmp = ae_v_dotproduct(&h.ptr.pp_double[i][1], 1, &x.ptr.p_double[1], 1, ae_v_len(1,n));
                        if( i==1 )
                        {
                            err = ae_maxreal(err, ae_fabs(tmp-beta, _state), _state);
                        }
                        else
                        {
                            err = ae_maxreal(err, ae_fabs(tmp, _state), _state);
                        }
                    }
                    meg = ae_maxreal(meg, err/xscale, _state);
                }
                
                /*
                 * ApplyReflectionFromTheLeft
                 */
                for(i=1; i<=m; i++)
                {
                    x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    v.ptr.p_double[i] = x.ptr.p_double[i];
                }
                for(i=1; i<=m; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                        b.ptr.pp_double[i][j] = a.ptr.pp_double[i][j];
                    }
                }
                generatereflection(&v, m, &tau, _state);
                beta = v.ptr.p_double[1];
                v.ptr.p_double[1] = (double)(1);
                applyreflectionfromtheleft(&b, tau, &v, 1, m, 1, n, &work, _state);
                for(i=1; i<=m; i++)
                {
                    for(j=1; j<=m; j++)
                    {
                        if( i==j )
                        {
                            h.ptr.pp_double[i][j] = 1-tau*v.ptr.p_double[i]*v.ptr.p_double[j];
                        }
                        else
                        {
                            h.ptr.pp_double[i][j] = -tau*v.ptr.p_double[i]*v.ptr.p_double[j];
                        }
                    }
                }
                for(i=1; i<=m; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        tmp = ae_v_dotproduct(&h.ptr.pp_double[i][1], 1, &a.ptr.pp_double[1][j], a.stride, ae_v_len(1,m));
                        c.ptr.pp_double[i][j] = tmp;
                    }
                }
                err = (double)(0);
                for(i=1; i<=m; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        err = ae_maxreal(err, ae_fabs(b.ptr.pp_double[i][j]-c.ptr.pp_double[i][j], _state), _state);
                    }
                }
                mel = ae_maxreal(mel, err, _state);
                
                /*
                 * ApplyReflectionFromTheRight
                 */
                for(i=1; i<=n; i++)
                {
                    x.ptr.p_double[i] = 2*ae_randomreal(_state)-1;
                    v.ptr.p_double[i] = x.ptr.p_double[i];
                }
                for(i=1; i<=m; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        a.ptr.pp_double[i][j] = 2*ae_randomreal(_state)-1;
                        b.ptr.pp_double[i][j] = a.ptr.pp_double[i][j];
                    }
                }
                generatereflection(&v, n, &tau, _state);
                beta = v.ptr.p_double[1];
                v.ptr.p_double[1] = (double)(1);
                applyreflectionfromtheright(&b, tau, &v, 1, m, 1, n, &work, _state);
                for(i=1; i<=n; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        if( i==j )
                        {
                            h.ptr.pp_double[i][j] = 1-tau*v.ptr.p_double[i]*v.ptr.p_double[j];
                        }
                        else
                        {
                            h.ptr.pp_double[i][j] = -tau*v.ptr.p_double[i]*v.ptr.p_double[j];
                        }
                    }
                }
                for(i=1; i<=m; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        tmp = ae_v_dotproduct(&a.ptr.pp_double[i][1], 1, &h.ptr.pp_double[1][j], h.stride, ae_v_len(1,n));
                        c.ptr.pp_double[i][j] = tmp;
                    }
                }
                err = (double)(0);
                for(i=1; i<=m; i++)
                {
                    for(j=1; j<=n; j++)
                    {
                        err = ae_maxreal(err, ae_fabs(b.ptr.pp_double[i][j]-c.ptr.pp_double[i][j], _state), _state);
                    }
                }
                mer = ae_maxreal(mer, err, _state);
            }
        }
    }
    
    /*
     * Overflow crash test
     */
    ae_vector_set_length(&x, 10+1, _state);
    ae_vector_set_length(&v, 10+1, _state);
    for(i=1; i<=10; i++)
    {
        v.ptr.p_double[i] = ae_maxrealnumber*0.01*(2*ae_randomreal(_state)-1);
    }
    generatereflection(&v, 10, &tau, _state);
    result = (ae_fp_less_eq(meg,threshold)&&ae_fp_less_eq(mel,threshold))&&ae_fp_less_eq(mer,threshold);
    if( !silent )
    {
        printf("TESTING REFLECTIONS\n");
        printf("Pass count is %0d\n",
            (int)(passcount));
        printf("Generate     absolute error is       %5.3e\n",
            (double)(meg));
        printf("Apply(Left)  absolute error is       %5.3e\n",
            (double)(mel));
        printf("Apply(Right) absolute error is       %5.3e\n",
            (double)(mer));
        printf("Overflow crash test passed\n");
        if( result )
        {
            printf("TEST PASSED\n");
        }
        else
        {
            printf("TEST FAILED\n");
        }
    }
    ae_frame_leave(_state);
    return result;
}


/*************************************************************************
Single-threaded stub. HPC ALGLIB replaces it by multithreaded code.
*************************************************************************/
ae_bool _pexec_testreflections(ae_bool silent, ae_state *_state)
{
    return testreflections(silent, _state);
}


/*$ End $*/
