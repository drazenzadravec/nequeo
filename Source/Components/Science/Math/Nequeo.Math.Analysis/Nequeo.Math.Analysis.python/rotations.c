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
#include "rotations.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
Application of a sequence of  elementary rotations to a matrix

The algorithm pre-multiplies the matrix by a sequence of rotation
transformations which is given by arrays C and S. Depending on the value
of the IsForward parameter either 1 and 2, 3 and 4 and so on (if IsForward=true)
rows are rotated, or the rows N and N-1, N-2 and N-3 and so on, are rotated.

Not the whole matrix but only a part of it is transformed (rows from M1 to
M2, columns from N1 to N2). Only the elements of this submatrix are changed.

Input parameters:
    IsForward   -   the sequence of the rotation application.
    M1,M2       -   the range of rows to be transformed.
    N1, N2      -   the range of columns to be transformed.
    C,S         -   transformation coefficients.
                    Array whose index ranges within [1..M2-M1].
    A           -   processed matrix.
    WORK        -   working array whose index ranges within [N1..N2].

Output parameters:
    A           -   transformed matrix.

Utility subroutine.
*************************************************************************/
void applyrotationsfromtheleft(ae_bool isforward,
     ae_int_t m1,
     ae_int_t m2,
     ae_int_t n1,
     ae_int_t n2,
     /* Real    */ ae_vector* c,
     /* Real    */ ae_vector* s,
     /* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* work,
     ae_state *_state)
{
    ae_int_t j;
    ae_int_t jp1;
    double ctemp;
    double stemp;
    double temp;


    if( m1>m2||n1>n2 )
    {
        return;
    }
    
    /*
     * Form  P * A
     */
    if( isforward )
    {
        if( n1!=n2 )
        {
            
            /*
             * Common case: N1<>N2
             */
            for(j=m1; j<=m2-1; j++)
            {
                ctemp = c->ptr.p_double[j-m1+1];
                stemp = s->ptr.p_double[j-m1+1];
                if( ae_fp_neq(ctemp,(double)(1))||ae_fp_neq(stemp,(double)(0)) )
                {
                    jp1 = j+1;
                    ae_v_moved(&work->ptr.p_double[n1], 1, &a->ptr.pp_double[jp1][n1], 1, ae_v_len(n1,n2), ctemp);
                    ae_v_subd(&work->ptr.p_double[n1], 1, &a->ptr.pp_double[j][n1], 1, ae_v_len(n1,n2), stemp);
                    ae_v_muld(&a->ptr.pp_double[j][n1], 1, ae_v_len(n1,n2), ctemp);
                    ae_v_addd(&a->ptr.pp_double[j][n1], 1, &a->ptr.pp_double[jp1][n1], 1, ae_v_len(n1,n2), stemp);
                    ae_v_move(&a->ptr.pp_double[jp1][n1], 1, &work->ptr.p_double[n1], 1, ae_v_len(n1,n2));
                }
            }
        }
        else
        {
            
            /*
             * Special case: N1=N2
             */
            for(j=m1; j<=m2-1; j++)
            {
                ctemp = c->ptr.p_double[j-m1+1];
                stemp = s->ptr.p_double[j-m1+1];
                if( ae_fp_neq(ctemp,(double)(1))||ae_fp_neq(stemp,(double)(0)) )
                {
                    temp = a->ptr.pp_double[j+1][n1];
                    a->ptr.pp_double[j+1][n1] = ctemp*temp-stemp*a->ptr.pp_double[j][n1];
                    a->ptr.pp_double[j][n1] = stemp*temp+ctemp*a->ptr.pp_double[j][n1];
                }
            }
        }
    }
    else
    {
        if( n1!=n2 )
        {
            
            /*
             * Common case: N1<>N2
             */
            for(j=m2-1; j>=m1; j--)
            {
                ctemp = c->ptr.p_double[j-m1+1];
                stemp = s->ptr.p_double[j-m1+1];
                if( ae_fp_neq(ctemp,(double)(1))||ae_fp_neq(stemp,(double)(0)) )
                {
                    jp1 = j+1;
                    ae_v_moved(&work->ptr.p_double[n1], 1, &a->ptr.pp_double[jp1][n1], 1, ae_v_len(n1,n2), ctemp);
                    ae_v_subd(&work->ptr.p_double[n1], 1, &a->ptr.pp_double[j][n1], 1, ae_v_len(n1,n2), stemp);
                    ae_v_muld(&a->ptr.pp_double[j][n1], 1, ae_v_len(n1,n2), ctemp);
                    ae_v_addd(&a->ptr.pp_double[j][n1], 1, &a->ptr.pp_double[jp1][n1], 1, ae_v_len(n1,n2), stemp);
                    ae_v_move(&a->ptr.pp_double[jp1][n1], 1, &work->ptr.p_double[n1], 1, ae_v_len(n1,n2));
                }
            }
        }
        else
        {
            
            /*
             * Special case: N1=N2
             */
            for(j=m2-1; j>=m1; j--)
            {
                ctemp = c->ptr.p_double[j-m1+1];
                stemp = s->ptr.p_double[j-m1+1];
                if( ae_fp_neq(ctemp,(double)(1))||ae_fp_neq(stemp,(double)(0)) )
                {
                    temp = a->ptr.pp_double[j+1][n1];
                    a->ptr.pp_double[j+1][n1] = ctemp*temp-stemp*a->ptr.pp_double[j][n1];
                    a->ptr.pp_double[j][n1] = stemp*temp+ctemp*a->ptr.pp_double[j][n1];
                }
            }
        }
    }
}


/*************************************************************************
Application of a sequence of  elementary rotations to a matrix

The algorithm post-multiplies the matrix by a sequence of rotation
transformations which is given by arrays C and S. Depending on the value
of the IsForward parameter either 1 and 2, 3 and 4 and so on (if IsForward=true)
rows are rotated, or the rows N and N-1, N-2 and N-3 and so on are rotated.

Not the whole matrix but only a part of it is transformed (rows from M1
to M2, columns from N1 to N2). Only the elements of this submatrix are changed.

Input parameters:
    IsForward   -   the sequence of the rotation application.
    M1,M2       -   the range of rows to be transformed.
    N1, N2      -   the range of columns to be transformed.
    C,S         -   transformation coefficients.
                    Array whose index ranges within [1..N2-N1].
    A           -   processed matrix.
    WORK        -   working array whose index ranges within [M1..M2].

Output parameters:
    A           -   transformed matrix.

Utility subroutine.
*************************************************************************/
void applyrotationsfromtheright(ae_bool isforward,
     ae_int_t m1,
     ae_int_t m2,
     ae_int_t n1,
     ae_int_t n2,
     /* Real    */ ae_vector* c,
     /* Real    */ ae_vector* s,
     /* Real    */ ae_matrix* a,
     /* Real    */ ae_vector* work,
     ae_state *_state)
{
    ae_int_t j;
    ae_int_t jp1;
    double ctemp;
    double stemp;
    double temp;


    
    /*
     * Form A * P'
     */
    if( isforward )
    {
        if( m1!=m2 )
        {
            
            /*
             * Common case: M1<>M2
             */
            for(j=n1; j<=n2-1; j++)
            {
                ctemp = c->ptr.p_double[j-n1+1];
                stemp = s->ptr.p_double[j-n1+1];
                if( ae_fp_neq(ctemp,(double)(1))||ae_fp_neq(stemp,(double)(0)) )
                {
                    jp1 = j+1;
                    ae_v_moved(&work->ptr.p_double[m1], 1, &a->ptr.pp_double[m1][jp1], a->stride, ae_v_len(m1,m2), ctemp);
                    ae_v_subd(&work->ptr.p_double[m1], 1, &a->ptr.pp_double[m1][j], a->stride, ae_v_len(m1,m2), stemp);
                    ae_v_muld(&a->ptr.pp_double[m1][j], a->stride, ae_v_len(m1,m2), ctemp);
                    ae_v_addd(&a->ptr.pp_double[m1][j], a->stride, &a->ptr.pp_double[m1][jp1], a->stride, ae_v_len(m1,m2), stemp);
                    ae_v_move(&a->ptr.pp_double[m1][jp1], a->stride, &work->ptr.p_double[m1], 1, ae_v_len(m1,m2));
                }
            }
        }
        else
        {
            
            /*
             * Special case: M1=M2
             */
            for(j=n1; j<=n2-1; j++)
            {
                ctemp = c->ptr.p_double[j-n1+1];
                stemp = s->ptr.p_double[j-n1+1];
                if( ae_fp_neq(ctemp,(double)(1))||ae_fp_neq(stemp,(double)(0)) )
                {
                    temp = a->ptr.pp_double[m1][j+1];
                    a->ptr.pp_double[m1][j+1] = ctemp*temp-stemp*a->ptr.pp_double[m1][j];
                    a->ptr.pp_double[m1][j] = stemp*temp+ctemp*a->ptr.pp_double[m1][j];
                }
            }
        }
    }
    else
    {
        if( m1!=m2 )
        {
            
            /*
             * Common case: M1<>M2
             */
            for(j=n2-1; j>=n1; j--)
            {
                ctemp = c->ptr.p_double[j-n1+1];
                stemp = s->ptr.p_double[j-n1+1];
                if( ae_fp_neq(ctemp,(double)(1))||ae_fp_neq(stemp,(double)(0)) )
                {
                    jp1 = j+1;
                    ae_v_moved(&work->ptr.p_double[m1], 1, &a->ptr.pp_double[m1][jp1], a->stride, ae_v_len(m1,m2), ctemp);
                    ae_v_subd(&work->ptr.p_double[m1], 1, &a->ptr.pp_double[m1][j], a->stride, ae_v_len(m1,m2), stemp);
                    ae_v_muld(&a->ptr.pp_double[m1][j], a->stride, ae_v_len(m1,m2), ctemp);
                    ae_v_addd(&a->ptr.pp_double[m1][j], a->stride, &a->ptr.pp_double[m1][jp1], a->stride, ae_v_len(m1,m2), stemp);
                    ae_v_move(&a->ptr.pp_double[m1][jp1], a->stride, &work->ptr.p_double[m1], 1, ae_v_len(m1,m2));
                }
            }
        }
        else
        {
            
            /*
             * Special case: M1=M2
             */
            for(j=n2-1; j>=n1; j--)
            {
                ctemp = c->ptr.p_double[j-n1+1];
                stemp = s->ptr.p_double[j-n1+1];
                if( ae_fp_neq(ctemp,(double)(1))||ae_fp_neq(stemp,(double)(0)) )
                {
                    temp = a->ptr.pp_double[m1][j+1];
                    a->ptr.pp_double[m1][j+1] = ctemp*temp-stemp*a->ptr.pp_double[m1][j];
                    a->ptr.pp_double[m1][j] = stemp*temp+ctemp*a->ptr.pp_double[m1][j];
                }
            }
        }
    }
}


/*************************************************************************
The subroutine generates the elementary rotation, so that:

[  CS  SN  ]  .  [ F ]  =  [ R ]
[ -SN  CS  ]     [ G ]     [ 0 ]

CS**2 + SN**2 = 1
*************************************************************************/
void generaterotation(double f,
     double g,
     double* cs,
     double* sn,
     double* r,
     ae_state *_state)
{
    double f1;
    double g1;

    *cs = 0;
    *sn = 0;
    *r = 0;

    if( ae_fp_eq(g,(double)(0)) )
    {
        *cs = (double)(1);
        *sn = (double)(0);
        *r = f;
    }
    else
    {
        if( ae_fp_eq(f,(double)(0)) )
        {
            *cs = (double)(0);
            *sn = (double)(1);
            *r = g;
        }
        else
        {
            f1 = f;
            g1 = g;
            if( ae_fp_greater(ae_fabs(f1, _state),ae_fabs(g1, _state)) )
            {
                *r = ae_fabs(f1, _state)*ae_sqrt(1+ae_sqr(g1/f1, _state), _state);
            }
            else
            {
                *r = ae_fabs(g1, _state)*ae_sqrt(1+ae_sqr(f1/g1, _state), _state);
            }
            *cs = f1/(*r);
            *sn = g1/(*r);
            if( ae_fp_greater(ae_fabs(f, _state),ae_fabs(g, _state))&&ae_fp_less(*cs,(double)(0)) )
            {
                *cs = -*cs;
                *sn = -*sn;
                *r = -*r;
            }
        }
    }
}


/*$ End $*/
