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


#include "stdafx.h"
#include "sblas.h"


/*$ Declarations $*/


/*$ Body $*/


void symmetricmatrixvectormultiply(/* Real    */ ae_matrix* a,
     ae_bool isupper,
     ae_int_t i1,
     ae_int_t i2,
     /* Real    */ ae_vector* x,
     double alpha,
     /* Real    */ ae_vector* y,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t ba1;
    ae_int_t ba2;
    ae_int_t by1;
    ae_int_t by2;
    ae_int_t bx1;
    ae_int_t bx2;
    ae_int_t n;
    double v;


    n = i2-i1+1;
    if( n<=0 )
    {
        return;
    }
    
    /*
     * Let A = L + D + U, where
     *  L is strictly lower triangular (main diagonal is zero)
     *  D is diagonal
     *  U is strictly upper triangular (main diagonal is zero)
     *
     * A*x = L*x + D*x + U*x
     *
     * Calculate D*x first
     */
    for(i=i1; i<=i2; i++)
    {
        y->ptr.p_double[i-i1+1] = a->ptr.pp_double[i][i]*x->ptr.p_double[i-i1+1];
    }
    
    /*
     * Add L*x + U*x
     */
    if( isupper )
    {
        for(i=i1; i<=i2-1; i++)
        {
            
            /*
             * Add L*x to the result
             */
            v = x->ptr.p_double[i-i1+1];
            by1 = i-i1+2;
            by2 = n;
            ba1 = i+1;
            ba2 = i2;
            ae_v_addd(&y->ptr.p_double[by1], 1, &a->ptr.pp_double[i][ba1], 1, ae_v_len(by1,by2), v);
            
            /*
             * Add U*x to the result
             */
            bx1 = i-i1+2;
            bx2 = n;
            ba1 = i+1;
            ba2 = i2;
            v = ae_v_dotproduct(&x->ptr.p_double[bx1], 1, &a->ptr.pp_double[i][ba1], 1, ae_v_len(bx1,bx2));
            y->ptr.p_double[i-i1+1] = y->ptr.p_double[i-i1+1]+v;
        }
    }
    else
    {
        for(i=i1+1; i<=i2; i++)
        {
            
            /*
             * Add L*x to the result
             */
            bx1 = 1;
            bx2 = i-i1;
            ba1 = i1;
            ba2 = i-1;
            v = ae_v_dotproduct(&x->ptr.p_double[bx1], 1, &a->ptr.pp_double[i][ba1], 1, ae_v_len(bx1,bx2));
            y->ptr.p_double[i-i1+1] = y->ptr.p_double[i-i1+1]+v;
            
            /*
             * Add U*x to the result
             */
            v = x->ptr.p_double[i-i1+1];
            by1 = 1;
            by2 = i-i1;
            ba1 = i1;
            ba2 = i-1;
            ae_v_addd(&y->ptr.p_double[by1], 1, &a->ptr.pp_double[i][ba1], 1, ae_v_len(by1,by2), v);
        }
    }
    ae_v_muld(&y->ptr.p_double[1], 1, ae_v_len(1,n), alpha);
    touchint(&ba2, _state);
}


void symmetricrank2update(/* Real    */ ae_matrix* a,
     ae_bool isupper,
     ae_int_t i1,
     ae_int_t i2,
     /* Real    */ ae_vector* x,
     /* Real    */ ae_vector* y,
     /* Real    */ ae_vector* t,
     double alpha,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t tp1;
    ae_int_t tp2;
    double v;


    if( isupper )
    {
        for(i=i1; i<=i2; i++)
        {
            tp1 = i+1-i1;
            tp2 = i2-i1+1;
            v = x->ptr.p_double[i+1-i1];
            ae_v_moved(&t->ptr.p_double[tp1], 1, &y->ptr.p_double[tp1], 1, ae_v_len(tp1,tp2), v);
            v = y->ptr.p_double[i+1-i1];
            ae_v_addd(&t->ptr.p_double[tp1], 1, &x->ptr.p_double[tp1], 1, ae_v_len(tp1,tp2), v);
            ae_v_muld(&t->ptr.p_double[tp1], 1, ae_v_len(tp1,tp2), alpha);
            ae_v_add(&a->ptr.pp_double[i][i], 1, &t->ptr.p_double[tp1], 1, ae_v_len(i,i2));
        }
    }
    else
    {
        for(i=i1; i<=i2; i++)
        {
            tp1 = 1;
            tp2 = i+1-i1;
            v = x->ptr.p_double[i+1-i1];
            ae_v_moved(&t->ptr.p_double[tp1], 1, &y->ptr.p_double[tp1], 1, ae_v_len(tp1,tp2), v);
            v = y->ptr.p_double[i+1-i1];
            ae_v_addd(&t->ptr.p_double[tp1], 1, &x->ptr.p_double[tp1], 1, ae_v_len(tp1,tp2), v);
            ae_v_muld(&t->ptr.p_double[tp1], 1, ae_v_len(tp1,tp2), alpha);
            ae_v_add(&a->ptr.pp_double[i][i1], 1, &t->ptr.p_double[tp1], 1, ae_v_len(i1,i));
        }
    }
}


/*$ End $*/
