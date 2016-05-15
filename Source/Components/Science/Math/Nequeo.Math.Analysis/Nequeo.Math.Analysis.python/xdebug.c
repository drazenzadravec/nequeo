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
#include "xdebug.h"


/*$ Declarations $*/


/*$ Body $*/


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Creates and returns XDebugRecord1 structure:
* integer and complex fields of Rec1 are set to 1 and 1+i correspondingly
* array field of Rec1 is set to [2,3]

  -- ALGLIB --
     Copyright 27.05.2014 by Bochkanov Sergey
*************************************************************************/
void xdebuginitrecord1(xdebugrecord1* rec1, ae_state *_state)
{

    _xdebugrecord1_clear(rec1);

    rec1->i = 1;
    rec1->c.x = (double)(1);
    rec1->c.y = (double)(1);
    ae_vector_set_length(&rec1->a, 2, _state);
    rec1->a.ptr.p_double[0] = (double)(2);
    rec1->a.ptr.p_double[1] = (double)(3);
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Counts number of True values in the boolean 1D array.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
ae_int_t xdebugb1count(/* Boolean */ ae_vector* a, ae_state *_state)
{
    ae_int_t i;
    ae_int_t result;


    result = 0;
    for(i=0; i<=a->cnt-1; i++)
    {
        if( a->ptr.p_bool[i] )
        {
            result = result+1;
        }
    }
    return result;
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Replace all values in array by NOT(a[i]).
Array is passed using "shared" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugb1not(/* Boolean */ ae_vector* a, ae_state *_state)
{
    ae_int_t i;


    for(i=0; i<=a->cnt-1; i++)
    {
        a->ptr.p_bool[i] = !a->ptr.p_bool[i];
    }
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Appends copy of array to itself.
Array is passed using "var" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugb1appendcopy(/* Boolean */ ae_vector* a, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_vector b;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&b, 0, DT_BOOL, _state);

    ae_vector_set_length(&b, a->cnt, _state);
    for(i=0; i<=b.cnt-1; i++)
    {
        b.ptr.p_bool[i] = a->ptr.p_bool[i];
    }
    ae_vector_set_length(a, 2*b.cnt, _state);
    for(i=0; i<=a->cnt-1; i++)
    {
        a->ptr.p_bool[i] = b.ptr.p_bool[i%b.cnt];
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Generate N-element array with even-numbered elements set to True.
Array is passed using "out" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugb1outeven(ae_int_t n,
     /* Boolean */ ae_vector* a,
     ae_state *_state)
{
    ae_int_t i;

    ae_vector_clear(a);

    ae_vector_set_length(a, n, _state);
    for(i=0; i<=a->cnt-1; i++)
    {
        a->ptr.p_bool[i] = i%2==0;
    }
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Returns sum of elements in the array.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
ae_int_t xdebugi1sum(/* Integer */ ae_vector* a, ae_state *_state)
{
    ae_int_t i;
    ae_int_t result;


    result = 0;
    for(i=0; i<=a->cnt-1; i++)
    {
        result = result+a->ptr.p_int[i];
    }
    return result;
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Replace all values in array by -A[I]
Array is passed using "shared" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugi1neg(/* Integer */ ae_vector* a, ae_state *_state)
{
    ae_int_t i;


    for(i=0; i<=a->cnt-1; i++)
    {
        a->ptr.p_int[i] = -a->ptr.p_int[i];
    }
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Appends copy of array to itself.
Array is passed using "var" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugi1appendcopy(/* Integer */ ae_vector* a, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_vector b;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&b, 0, DT_INT, _state);

    ae_vector_set_length(&b, a->cnt, _state);
    for(i=0; i<=b.cnt-1; i++)
    {
        b.ptr.p_int[i] = a->ptr.p_int[i];
    }
    ae_vector_set_length(a, 2*b.cnt, _state);
    for(i=0; i<=a->cnt-1; i++)
    {
        a->ptr.p_int[i] = b.ptr.p_int[i%b.cnt];
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Generate N-element array with even-numbered A[I] set to I, and odd-numbered
ones set to 0.

Array is passed using "out" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugi1outeven(ae_int_t n,
     /* Integer */ ae_vector* a,
     ae_state *_state)
{
    ae_int_t i;

    ae_vector_clear(a);

    ae_vector_set_length(a, n, _state);
    for(i=0; i<=a->cnt-1; i++)
    {
        if( i%2==0 )
        {
            a->ptr.p_int[i] = i;
        }
        else
        {
            a->ptr.p_int[i] = 0;
        }
    }
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Returns sum of elements in the array.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
double xdebugr1sum(/* Real    */ ae_vector* a, ae_state *_state)
{
    ae_int_t i;
    double result;


    result = (double)(0);
    for(i=0; i<=a->cnt-1; i++)
    {
        result = result+a->ptr.p_double[i];
    }
    return result;
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Replace all values in array by -A[I]
Array is passed using "shared" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugr1neg(/* Real    */ ae_vector* a, ae_state *_state)
{
    ae_int_t i;


    for(i=0; i<=a->cnt-1; i++)
    {
        a->ptr.p_double[i] = -a->ptr.p_double[i];
    }
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Appends copy of array to itself.
Array is passed using "var" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugr1appendcopy(/* Real    */ ae_vector* a, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_vector b;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&b, 0, DT_REAL, _state);

    ae_vector_set_length(&b, a->cnt, _state);
    for(i=0; i<=b.cnt-1; i++)
    {
        b.ptr.p_double[i] = a->ptr.p_double[i];
    }
    ae_vector_set_length(a, 2*b.cnt, _state);
    for(i=0; i<=a->cnt-1; i++)
    {
        a->ptr.p_double[i] = b.ptr.p_double[i%b.cnt];
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Generate N-element array with even-numbered A[I] set to I*0.25,
and odd-numbered ones are set to 0.

Array is passed using "out" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugr1outeven(ae_int_t n,
     /* Real    */ ae_vector* a,
     ae_state *_state)
{
    ae_int_t i;

    ae_vector_clear(a);

    ae_vector_set_length(a, n, _state);
    for(i=0; i<=a->cnt-1; i++)
    {
        if( i%2==0 )
        {
            a->ptr.p_double[i] = i*0.25;
        }
        else
        {
            a->ptr.p_double[i] = (double)(0);
        }
    }
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Returns sum of elements in the array.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
ae_complex xdebugc1sum(/* Complex */ ae_vector* a, ae_state *_state)
{
    ae_int_t i;
    ae_complex result;


    result = ae_complex_from_i(0);
    for(i=0; i<=a->cnt-1; i++)
    {
        result = ae_c_add(result,a->ptr.p_complex[i]);
    }
    return result;
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Replace all values in array by -A[I]
Array is passed using "shared" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugc1neg(/* Complex */ ae_vector* a, ae_state *_state)
{
    ae_int_t i;


    for(i=0; i<=a->cnt-1; i++)
    {
        a->ptr.p_complex[i] = ae_c_neg(a->ptr.p_complex[i]);
    }
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Appends copy of array to itself.
Array is passed using "var" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugc1appendcopy(/* Complex */ ae_vector* a, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_vector b;

    ae_frame_make(_state, &_frame_block);
    ae_vector_init(&b, 0, DT_COMPLEX, _state);

    ae_vector_set_length(&b, a->cnt, _state);
    for(i=0; i<=b.cnt-1; i++)
    {
        b.ptr.p_complex[i] = a->ptr.p_complex[i];
    }
    ae_vector_set_length(a, 2*b.cnt, _state);
    for(i=0; i<=a->cnt-1; i++)
    {
        a->ptr.p_complex[i] = b.ptr.p_complex[i%b.cnt];
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Generate N-element array with even-numbered A[K] set to (x,y) = (K*0.25, K*0.125)
and odd-numbered ones are set to 0.

Array is passed using "out" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugc1outeven(ae_int_t n,
     /* Complex */ ae_vector* a,
     ae_state *_state)
{
    ae_int_t i;

    ae_vector_clear(a);

    ae_vector_set_length(a, n, _state);
    for(i=0; i<=a->cnt-1; i++)
    {
        if( i%2==0 )
        {
            a->ptr.p_complex[i].x = i*0.250;
            a->ptr.p_complex[i].y = i*0.125;
        }
        else
        {
            a->ptr.p_complex[i] = ae_complex_from_i(0);
        }
    }
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Counts number of True values in the boolean 2D array.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
ae_int_t xdebugb2count(/* Boolean */ ae_matrix* a, ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t result;


    result = 0;
    for(i=0; i<=a->rows-1; i++)
    {
        for(j=0; j<=a->cols-1; j++)
        {
            if( a->ptr.pp_bool[i][j] )
            {
                result = result+1;
            }
        }
    }
    return result;
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Replace all values in array by NOT(a[i]).
Array is passed using "shared" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugb2not(/* Boolean */ ae_matrix* a, ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    for(i=0; i<=a->rows-1; i++)
    {
        for(j=0; j<=a->cols-1; j++)
        {
            a->ptr.pp_bool[i][j] = !a->ptr.pp_bool[i][j];
        }
    }
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Transposes array.
Array is passed using "var" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugb2transpose(/* Boolean */ ae_matrix* a, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_matrix b;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&b, 0, 0, DT_BOOL, _state);

    ae_matrix_set_length(&b, a->rows, a->cols, _state);
    for(i=0; i<=b.rows-1; i++)
    {
        for(j=0; j<=b.cols-1; j++)
        {
            b.ptr.pp_bool[i][j] = a->ptr.pp_bool[i][j];
        }
    }
    ae_matrix_set_length(a, b.cols, b.rows, _state);
    for(i=0; i<=b.rows-1; i++)
    {
        for(j=0; j<=b.cols-1; j++)
        {
            a->ptr.pp_bool[j][i] = b.ptr.pp_bool[i][j];
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Generate MxN matrix with elements set to "Sin(3*I+5*J)>0"
Array is passed using "out" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugb2outsin(ae_int_t m,
     ae_int_t n,
     /* Boolean */ ae_matrix* a,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(a);

    ae_matrix_set_length(a, m, n, _state);
    for(i=0; i<=a->rows-1; i++)
    {
        for(j=0; j<=a->cols-1; j++)
        {
            a->ptr.pp_bool[i][j] = ae_fp_greater(ae_sin((double)(3*i+5*j), _state),(double)(0));
        }
    }
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Returns sum of elements in the array.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
ae_int_t xdebugi2sum(/* Integer */ ae_matrix* a, ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_int_t result;


    result = 0;
    for(i=0; i<=a->rows-1; i++)
    {
        for(j=0; j<=a->cols-1; j++)
        {
            result = result+a->ptr.pp_int[i][j];
        }
    }
    return result;
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Replace all values in array by -a[i,j]
Array is passed using "shared" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugi2neg(/* Integer */ ae_matrix* a, ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    for(i=0; i<=a->rows-1; i++)
    {
        for(j=0; j<=a->cols-1; j++)
        {
            a->ptr.pp_int[i][j] = -a->ptr.pp_int[i][j];
        }
    }
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Transposes array.
Array is passed using "var" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugi2transpose(/* Integer */ ae_matrix* a, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_matrix b;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&b, 0, 0, DT_INT, _state);

    ae_matrix_set_length(&b, a->rows, a->cols, _state);
    for(i=0; i<=b.rows-1; i++)
    {
        for(j=0; j<=b.cols-1; j++)
        {
            b.ptr.pp_int[i][j] = a->ptr.pp_int[i][j];
        }
    }
    ae_matrix_set_length(a, b.cols, b.rows, _state);
    for(i=0; i<=b.rows-1; i++)
    {
        for(j=0; j<=b.cols-1; j++)
        {
            a->ptr.pp_int[j][i] = b.ptr.pp_int[i][j];
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Generate MxN matrix with elements set to "Sign(Sin(3*I+5*J))"
Array is passed using "out" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugi2outsin(ae_int_t m,
     ae_int_t n,
     /* Integer */ ae_matrix* a,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(a);

    ae_matrix_set_length(a, m, n, _state);
    for(i=0; i<=a->rows-1; i++)
    {
        for(j=0; j<=a->cols-1; j++)
        {
            a->ptr.pp_int[i][j] = ae_sign(ae_sin((double)(3*i+5*j), _state), _state);
        }
    }
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Returns sum of elements in the array.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
double xdebugr2sum(/* Real    */ ae_matrix* a, ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double result;


    result = (double)(0);
    for(i=0; i<=a->rows-1; i++)
    {
        for(j=0; j<=a->cols-1; j++)
        {
            result = result+a->ptr.pp_double[i][j];
        }
    }
    return result;
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Replace all values in array by -a[i,j]
Array is passed using "shared" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugr2neg(/* Real    */ ae_matrix* a, ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    for(i=0; i<=a->rows-1; i++)
    {
        for(j=0; j<=a->cols-1; j++)
        {
            a->ptr.pp_double[i][j] = -a->ptr.pp_double[i][j];
        }
    }
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Transposes array.
Array is passed using "var" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugr2transpose(/* Real    */ ae_matrix* a, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_matrix b;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&b, 0, 0, DT_REAL, _state);

    ae_matrix_set_length(&b, a->rows, a->cols, _state);
    for(i=0; i<=b.rows-1; i++)
    {
        for(j=0; j<=b.cols-1; j++)
        {
            b.ptr.pp_double[i][j] = a->ptr.pp_double[i][j];
        }
    }
    ae_matrix_set_length(a, b.cols, b.rows, _state);
    for(i=0; i<=b.rows-1; i++)
    {
        for(j=0; j<=b.cols-1; j++)
        {
            a->ptr.pp_double[j][i] = b.ptr.pp_double[i][j];
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Generate MxN matrix with elements set to "Sin(3*I+5*J)"
Array is passed using "out" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugr2outsin(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(a);

    ae_matrix_set_length(a, m, n, _state);
    for(i=0; i<=a->rows-1; i++)
    {
        for(j=0; j<=a->cols-1; j++)
        {
            a->ptr.pp_double[i][j] = ae_sin((double)(3*i+5*j), _state);
        }
    }
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Returns sum of elements in the array.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
ae_complex xdebugc2sum(/* Complex */ ae_matrix* a, ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    ae_complex result;


    result = ae_complex_from_i(0);
    for(i=0; i<=a->rows-1; i++)
    {
        for(j=0; j<=a->cols-1; j++)
        {
            result = ae_c_add(result,a->ptr.pp_complex[i][j]);
        }
    }
    return result;
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Replace all values in array by -a[i,j]
Array is passed using "shared" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugc2neg(/* Complex */ ae_matrix* a, ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;


    for(i=0; i<=a->rows-1; i++)
    {
        for(j=0; j<=a->cols-1; j++)
        {
            a->ptr.pp_complex[i][j] = ae_c_neg(a->ptr.pp_complex[i][j]);
        }
    }
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Transposes array.
Array is passed using "var" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugc2transpose(/* Complex */ ae_matrix* a, ae_state *_state)
{
    ae_frame _frame_block;
    ae_int_t i;
    ae_int_t j;
    ae_matrix b;

    ae_frame_make(_state, &_frame_block);
    ae_matrix_init(&b, 0, 0, DT_COMPLEX, _state);

    ae_matrix_set_length(&b, a->rows, a->cols, _state);
    for(i=0; i<=b.rows-1; i++)
    {
        for(j=0; j<=b.cols-1; j++)
        {
            b.ptr.pp_complex[i][j] = a->ptr.pp_complex[i][j];
        }
    }
    ae_matrix_set_length(a, b.cols, b.rows, _state);
    for(i=0; i<=b.rows-1; i++)
    {
        for(j=0; j<=b.cols-1; j++)
        {
            a->ptr.pp_complex[j][i] = b.ptr.pp_complex[i][j];
        }
    }
    ae_frame_leave(_state);
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Generate MxN matrix with elements set to "Sin(3*I+5*J),Cos(3*I+5*J)"
Array is passed using "out" convention.

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
void xdebugc2outsincos(ae_int_t m,
     ae_int_t n,
     /* Complex */ ae_matrix* a,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;

    ae_matrix_clear(a);

    ae_matrix_set_length(a, m, n, _state);
    for(i=0; i<=a->rows-1; i++)
    {
        for(j=0; j<=a->cols-1; j++)
        {
            a->ptr.pp_complex[i][j].x = ae_sin((double)(3*i+5*j), _state);
            a->ptr.pp_complex[i][j].y = ae_cos((double)(3*i+5*j), _state);
        }
    }
}


/*************************************************************************
This is debug function intended for testing ALGLIB interface generator.
Never use it in any real life project.

Returns sum of a[i,j]*(1+b[i,j]) such that c[i,j] is True

  -- ALGLIB --
     Copyright 11.10.2013 by Bochkanov Sergey
*************************************************************************/
double xdebugmaskedbiasedproductsum(ae_int_t m,
     ae_int_t n,
     /* Real    */ ae_matrix* a,
     /* Real    */ ae_matrix* b,
     /* Boolean */ ae_matrix* c,
     ae_state *_state)
{
    ae_int_t i;
    ae_int_t j;
    double result;


    ae_assert(m>=a->rows, "Assertion failed", _state);
    ae_assert(m>=b->rows, "Assertion failed", _state);
    ae_assert(m>=c->rows, "Assertion failed", _state);
    ae_assert(n>=a->cols, "Assertion failed", _state);
    ae_assert(n>=b->cols, "Assertion failed", _state);
    ae_assert(n>=c->cols, "Assertion failed", _state);
    result = 0.0;
    for(i=0; i<=m-1; i++)
    {
        for(j=0; j<=n-1; j++)
        {
            if( c->ptr.pp_bool[i][j] )
            {
                result = result+a->ptr.pp_double[i][j]*(1+b->ptr.pp_double[i][j]);
            }
        }
    }
    return result;
}


void _xdebugrecord1_init(void* _p, ae_state *_state)
{
    xdebugrecord1 *p = (xdebugrecord1*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_init(&p->a, 0, DT_REAL, _state);
}


void _xdebugrecord1_init_copy(void* _dst, void* _src, ae_state *_state)
{
    xdebugrecord1 *dst = (xdebugrecord1*)_dst;
    xdebugrecord1 *src = (xdebugrecord1*)_src;
    dst->i = src->i;
    dst->c = src->c;
    ae_vector_init_copy(&dst->a, &src->a, _state);
}


void _xdebugrecord1_clear(void* _p)
{
    xdebugrecord1 *p = (xdebugrecord1*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_clear(&p->a);
}


void _xdebugrecord1_destroy(void* _p)
{
    xdebugrecord1 *p = (xdebugrecord1*)_p;
    ae_touch_ptr((void*)p);
    ae_vector_destroy(&p->a);
}


/*$ End $*/
