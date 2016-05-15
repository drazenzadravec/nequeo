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
#include "testcreflectionsunit.h"


/*$ Declarations $*/


/*$ Body $*/


ae_bool testcreflections(ae_bool silent, ae_state *_state)
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
    ae_complex tmp;
    ae_complex beta;
    ae_complex tau;
    double err;
    double mer;
    double mel;
    double meg;
    ae_int_t pass;
    ae_int_t passcount;
    ae_bool waserrors;
    double threshold;
    ae_bool result;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&x, 0, DT_COMPLEX, _state);
    ae_vector_init(&v, 0, DT_COMPLEX, _state);
    ae_vector_init(&work, 0, DT_COMPLEX, _state);
    ae_matrix_init(&h, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&a, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&b, 0, 0, DT_COMPLEX, _state);
    ae_matrix_init(&c, 0, 0, DT_COMPLEX, _state);

    threshold = 1000*ae_machineepsilon;
    passcount = 1000;
    mer = (double)(0);
    mel = (double)(0);
    meg = (double)(0);
    for(pass=1; pass<=passcount; pass++)
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
         * GenerateReflection
         */
        for(i=1; i<=n; i++)
        {
            x.ptr.p_complex[i].x = 2*ae_randomreal(_state)-1;
            x.ptr.p_complex[i].y = 2*ae_randomreal(_state)-1;
            v.ptr.p_complex[i] = x.ptr.p_complex[i];
        }
        complexgeneratereflection(&v, n, &tau, _state);
        beta = v.ptr.p_complex[1];
        v.ptr.p_complex[1] = ae_complex_from_i(1);
        for(i=1; i<=n; i++)
        {
            for(j=1; j<=n; j++)
            {
                if( i==j )
                {
                    h.ptr.pp_complex[i][j] = ae_c_d_sub(1,ae_c_mul(ae_c_mul(tau,v.ptr.p_complex[i]),ae_c_conj(v.ptr.p_complex[j], _state)));
                }
                else
                {
                    h.ptr.pp_complex[i][j] = ae_c_neg(ae_c_mul(ae_c_mul(tau,v.ptr.p_complex[i]),ae_c_conj(v.ptr.p_complex[j], _state)));
                }
            }
        }
        err = (double)(0);
        for(i=1; i<=n; i++)
        {
            tmp = ae_v_cdotproduct(&h.ptr.pp_complex[1][i], h.stride, "Conj", &x.ptr.p_complex[1], 1, "N", ae_v_len(1,n));
            if( i==1 )
            {
                err = ae_maxreal(err, ae_c_abs(ae_c_sub(tmp,beta), _state), _state);
            }
            else
            {
                err = ae_maxreal(err, ae_c_abs(tmp, _state), _state);
            }
        }
        err = ae_maxreal(err, ae_fabs(beta.y, _state), _state);
        meg = ae_maxreal(meg, err, _state);
        
        /*
         * ApplyReflectionFromTheLeft
         */
        for(i=1; i<=m; i++)
        {
            x.ptr.p_complex[i].x = 2*ae_randomreal(_state)-1;
            x.ptr.p_complex[i].y = 2*ae_randomreal(_state)-1;
            v.ptr.p_complex[i] = x.ptr.p_complex[i];
        }
        for(i=1; i<=m; i++)
        {
            for(j=1; j<=n; j++)
            {
                a.ptr.pp_complex[i][j].x = 2*ae_randomreal(_state)-1;
                a.ptr.pp_complex[i][j].y = 2*ae_randomreal(_state)-1;
                b.ptr.pp_complex[i][j] = a.ptr.pp_complex[i][j];
            }
        }
        complexgeneratereflection(&v, m, &tau, _state);
        beta = v.ptr.p_complex[1];
        v.ptr.p_complex[1] = ae_complex_from_i(1);
        complexapplyreflectionfromtheleft(&b, tau, &v, 1, m, 1, n, &work, _state);
        for(i=1; i<=m; i++)
        {
            for(j=1; j<=m; j++)
            {
                if( i==j )
                {
                    h.ptr.pp_complex[i][j] = ae_c_d_sub(1,ae_c_mul(ae_c_mul(tau,v.ptr.p_complex[i]),ae_c_conj(v.ptr.p_complex[j], _state)));
                }
                else
                {
                    h.ptr.pp_complex[i][j] = ae_c_neg(ae_c_mul(ae_c_mul(tau,v.ptr.p_complex[i]),ae_c_conj(v.ptr.p_complex[j], _state)));
                }
            }
        }
        for(i=1; i<=m; i++)
        {
            for(j=1; j<=n; j++)
            {
                tmp = ae_v_cdotproduct(&h.ptr.pp_complex[i][1], 1, "N", &a.ptr.pp_complex[1][j], a.stride, "N", ae_v_len(1,m));
                c.ptr.pp_complex[i][j] = tmp;
            }
        }
        err = (double)(0);
        for(i=1; i<=m; i++)
        {
            for(j=1; j<=n; j++)
            {
                err = ae_maxreal(err, ae_c_abs(ae_c_sub(b.ptr.pp_complex[i][j],c.ptr.pp_complex[i][j]), _state), _state);
            }
        }
        mel = ae_maxreal(mel, err, _state);
        
        /*
         * ApplyReflectionFromTheRight
         */
        for(i=1; i<=n; i++)
        {
            x.ptr.p_complex[i] = ae_complex_from_d(2*ae_randomreal(_state)-1);
            v.ptr.p_complex[i] = x.ptr.p_complex[i];
        }
        for(i=1; i<=m; i++)
        {
            for(j=1; j<=n; j++)
            {
                a.ptr.pp_complex[i][j] = ae_complex_from_d(2*ae_randomreal(_state)-1);
                b.ptr.pp_complex[i][j] = a.ptr.pp_complex[i][j];
            }
        }
        complexgeneratereflection(&v, n, &tau, _state);
        beta = v.ptr.p_complex[1];
        v.ptr.p_complex[1] = ae_complex_from_i(1);
        complexapplyreflectionfromtheright(&b, tau, &v, 1, m, 1, n, &work, _state);
        for(i=1; i<=n; i++)
        {
            for(j=1; j<=n; j++)
            {
                if( i==j )
                {
                    h.ptr.pp_complex[i][j] = ae_c_d_sub(1,ae_c_mul(ae_c_mul(tau,v.ptr.p_complex[i]),ae_c_conj(v.ptr.p_complex[j], _state)));
                }
                else
                {
                    h.ptr.pp_complex[i][j] = ae_c_neg(ae_c_mul(ae_c_mul(tau,v.ptr.p_complex[i]),ae_c_conj(v.ptr.p_complex[j], _state)));
                }
            }
        }
        for(i=1; i<=m; i++)
        {
            for(j=1; j<=n; j++)
            {
                tmp = ae_v_cdotproduct(&a.ptr.pp_complex[i][1], 1, "N", &h.ptr.pp_complex[1][j], h.stride, "N", ae_v_len(1,n));
                c.ptr.pp_complex[i][j] = tmp;
            }
        }
        err = (double)(0);
        for(i=1; i<=m; i++)
        {
            for(j=1; j<=n; j++)
            {
                err = ae_maxreal(err, ae_c_abs(ae_c_sub(b.ptr.pp_complex[i][j],c.ptr.pp_complex[i][j]), _state), _state);
            }
        }
        mer = ae_maxreal(mer, err, _state);
    }
    
    /*
     * Overflow crash test
     */
    ae_vector_set_length(&x, 10+1, _state);
    ae_vector_set_length(&v, 10+1, _state);
    for(i=1; i<=10; i++)
    {
        v.ptr.p_complex[i] = ae_complex_from_d(ae_maxrealnumber*0.01*(2*ae_randomreal(_state)-1));
    }
    complexgeneratereflection(&v, 10, &tau, _state);
    
    /*
     * report
     */
    waserrors = (ae_fp_greater(meg,threshold)||ae_fp_greater(mel,threshold))||ae_fp_greater(mer,threshold);
    if( !silent )
    {
        printf("TESTING COMPLEX REFLECTIONS\n");
        printf("Generate error:                          %5.3e\n",
            (double)(meg));
        printf("Apply(L) error:                          %5.3e\n",
            (double)(mel));
        printf("Apply(R) error:                          %5.3e\n",
            (double)(mer));
        printf("Threshold:                               %5.3e\n",
            (double)(threshold));
        printf("Overflow crash test:                     PASSED\n");
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
ae_bool _pexec_testcreflections(ae_bool silent, ae_state *_state)
{
    return testcreflections(silent, _state);
}


/*$ End $*/
