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
#include "ntheory.h"


/*$ Declarations $*/
static ae_bool ntheory_isprime(ae_int_t n, ae_state *_state);
static ae_int_t ntheory_modmul(ae_int_t a,
     ae_int_t b,
     ae_int_t n,
     ae_state *_state);
static ae_int_t ntheory_modexp(ae_int_t a,
     ae_int_t b,
     ae_int_t n,
     ae_state *_state);


/*$ Body $*/


void findprimitiverootandinverse(ae_int_t n,
     ae_int_t* proot,
     ae_int_t* invproot,
     ae_state *_state)
{
    ae_int_t candroot;
    ae_int_t phin;
    ae_int_t q;
    ae_int_t f;
    ae_bool allnonone;
    ae_int_t x;
    ae_int_t lastx;
    ae_int_t y;
    ae_int_t lasty;
    ae_int_t a;
    ae_int_t b;
    ae_int_t t;
    ae_int_t n2;

    *proot = 0;
    *invproot = 0;

    ae_assert(n>=3, "FindPrimitiveRootAndInverse: N<3", _state);
    *proot = 0;
    *invproot = 0;
    
    /*
     * check that N is prime
     */
    ae_assert(ntheory_isprime(n, _state), "FindPrimitiveRoot: N is not prime", _state);
    
    /*
     * Because N is prime, Euler totient function is equal to N-1
     */
    phin = n-1;
    
    /*
     * Test different values of PRoot - from 2 to N-1.
     * One of these values MUST be primitive root.
     *
     * For testing we use algorithm from Wiki (Primitive root modulo n):
     * * compute phi(N)
     * * determine the different prime factors of phi(N), say p1, ..., pk
     * * for every element m of Zn*, compute m^(phi(N)/pi) mod N for i=1..k
     *   using a fast algorithm for modular exponentiation.
     * * a number m for which these k results are all different from 1 is a
     *   primitive root.
     */
    for(candroot=2; candroot<=n-1; candroot++)
    {
        
        /*
         * We have current candidate root in CandRoot.
         *
         * Scan different prime factors of PhiN. Here:
         * * F is a current candidate factor
         * * Q is a current quotient - amount which was left after dividing PhiN
         *   by all previous factors
         *
         * For each factor, perform test mentioned above.
         */
        q = phin;
        f = 2;
        allnonone = ae_true;
        while(q>1)
        {
            if( q%f==0 )
            {
                t = ntheory_modexp(candroot, phin/f, n, _state);
                if( t==1 )
                {
                    allnonone = ae_false;
                    break;
                }
                while(q%f==0)
                {
                    q = q/f;
                }
            }
            f = f+1;
        }
        if( allnonone )
        {
            *proot = candroot;
            break;
        }
    }
    ae_assert(*proot>=2, "FindPrimitiveRoot: internal error (root not found)", _state);
    
    /*
     * Use extended Euclidean algorithm to find multiplicative inverse of primitive root
     */
    x = 0;
    lastx = 1;
    y = 1;
    lasty = 0;
    a = *proot;
    b = n;
    while(b!=0)
    {
        q = a/b;
        t = a%b;
        a = b;
        b = t;
        t = lastx-q*x;
        lastx = x;
        x = t;
        t = lasty-q*y;
        lasty = y;
        y = t;
    }
    while(lastx<0)
    {
        lastx = lastx+n;
    }
    *invproot = lastx;
    
    /*
     * Check that it is safe to perform multiplication modulo N.
     * Check results for consistency.
     */
    n2 = (n-1)*(n-1);
    ae_assert(n2/(n-1)==n-1, "FindPrimitiveRoot: internal error", _state);
    ae_assert(*proot*(*invproot)/(*proot)==(*invproot), "FindPrimitiveRoot: internal error", _state);
    ae_assert(*proot*(*invproot)/(*invproot)==(*proot), "FindPrimitiveRoot: internal error", _state);
    ae_assert(*proot*(*invproot)%n==1, "FindPrimitiveRoot: internal error", _state);
}


static ae_bool ntheory_isprime(ae_int_t n, ae_state *_state)
{
    ae_int_t p;
    ae_bool result;


    result = ae_false;
    p = 2;
    while(p*p<=n)
    {
        if( n%p==0 )
        {
            return result;
        }
        p = p+1;
    }
    result = ae_true;
    return result;
}


static ae_int_t ntheory_modmul(ae_int_t a,
     ae_int_t b,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t t;
    double ra;
    double rb;
    ae_int_t result;


    ae_assert(a>=0&&a<n, "ModMul: A<0 or A>=N", _state);
    ae_assert(b>=0&&b<n, "ModMul: B<0 or B>=N", _state);
    
    /*
     * Base cases
     */
    ra = (double)(a);
    rb = (double)(b);
    if( b==0||a==0 )
    {
        result = 0;
        return result;
    }
    if( b==1||a==1 )
    {
        result = a*b;
        return result;
    }
    if( ae_fp_eq(ra*rb,(double)(a*b)) )
    {
        result = a*b%n;
        return result;
    }
    
    /*
     * Non-base cases
     */
    if( b%2==0 )
    {
        
        /*
         * A*B = (A*(B/2)) * 2
         *
         * Product T=A*(B/2) is calculated recursively, product T*2 is
         * calculated as follows:
         * * result:=T-N
         * * result:=result+T
         * * if result<0 then result:=result+N
         *
         * In case integer result overflows, we generate exception
         */
        t = ntheory_modmul(a, b/2, n, _state);
        result = t-n;
        result = result+t;
        if( result<0 )
        {
            result = result+n;
        }
    }
    else
    {
        
        /*
         * A*B = (A*(B div 2)) * 2 + A
         *
         * Product T=A*(B/2) is calculated recursively, product T*2 is
         * calculated as follows:
         * * result:=T-N
         * * result:=result+T
         * * if result<0 then result:=result+N
         *
         * In case integer result overflows, we generate exception
         */
        t = ntheory_modmul(a, b/2, n, _state);
        result = t-n;
        result = result+t;
        if( result<0 )
        {
            result = result+n;
        }
        result = result-n;
        result = result+a;
        if( result<0 )
        {
            result = result+n;
        }
    }
    return result;
}


static ae_int_t ntheory_modexp(ae_int_t a,
     ae_int_t b,
     ae_int_t n,
     ae_state *_state)
{
    ae_int_t t;
    ae_int_t result;


    ae_assert(a>=0&&a<n, "ModExp: A<0 or A>=N", _state);
    ae_assert(b>=0, "ModExp: B<0", _state);
    
    /*
     * Base cases
     */
    if( b==0 )
    {
        result = 1;
        return result;
    }
    if( b==1 )
    {
        result = a;
        return result;
    }
    
    /*
     * Non-base cases
     */
    if( b%2==0 )
    {
        t = ntheory_modmul(a, a, n, _state);
        result = ntheory_modexp(t, b/2, n, _state);
    }
    else
    {
        t = ntheory_modmul(a, a, n, _state);
        result = ntheory_modexp(t, b/2, n, _state);
        result = ntheory_modmul(result, a, n, _state);
    }
    return result;
}


/*$ End $*/
