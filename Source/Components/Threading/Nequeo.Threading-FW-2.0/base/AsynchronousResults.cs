/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace Nequeo.Threading
{
    #region Generic Function Delegates
    /// <summary>
    /// Encapsulates a method that has no parameters and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult FunctionHandler<TResult>();

    /// <summary>
    /// Encapsulates a method that has one parameter and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult FunctionHandler<TResult, T1>(T1 parameter1);

    /// <summary>
    /// Encapsulates a method that has two parameters and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult FunctionHandler<TResult, T1, T2>(T1 parameter1, T2 parameter2);

    /// <summary>
    /// Encapsulates a method that has three parameters and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult FunctionHandler<TResult, T1, T2, T3>(T1 parameter1, T2 parameter2, T3 parameter3);

    /// <summary>
    /// Encapsulates a method that has four parameters and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter4">The fourth parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult FunctionHandler<TResult, T1, T2, T3, T4>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4);

    /// <summary>
    /// Encapsulates a method that has five parameters and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter4">The fourth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter5">The fifth parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult FunctionHandler<TResult, T1, T2, T3, T4, T5>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5);

    /// <summary>
    /// Encapsulates a method that has six parameters and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter4">The fourth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter5">The fifth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter6">The sixth parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult FunctionHandler<TResult, T1, T2, T3, T4, T5, T6>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6);

    /// <summary>
    /// Encapsulates a method that has seven parameters and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter4">The fourth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter5">The fifth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter6">The sixth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter7">The seventh parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult FunctionHandler<TResult, T1, T2, T3, T4, T5, T6, T7>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7);

    /// <summary>
    /// Encapsulates a method that has eight parameters and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter4">The fourth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter5">The fifth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter6">The sixth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter7">The seventh parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter8">The eighth parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult FunctionHandler<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7, T8 parameter8);

    /// <summary>
    /// Encapsulates a method that has eight parameters and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T9">The type of the nineth parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter4">The fourth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter5">The fifth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter6">The sixth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter7">The seventh parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter8">The eighth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter9">The nineth parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult FunctionHandler<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7, T8 parameter8, T9 parameter9);

    /// <summary>
    /// Encapsulates a method that has eight parameters and returns a value of the type specified by the TResult parameter.
    /// </summary>
    /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T9">The type of the nineth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter4">The fourth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter5">The fifth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter6">The sixth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter7">The seventh parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter8">The eighth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter9">The nineth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter10">The tenth parameter of the method that this delegate encapsulates.</param>
    /// <returns>The return value of the method that this delegate encapsulates.</returns>
    public delegate TResult FunctionHandler<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7, T8 parameter8, T9 parameter9, T10 parameter10);
    #endregion

    #region Generic Action Delegates
    /// <summary>
    /// Encapsulates a method that takes no parameters and does not return a value.
    /// </summary>
    public delegate void ActionHandler();

    /// <summary>
    /// Encapsulates a method that has one parameter and does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    public delegate void ActionHandler<T1>(T1 parameter1);

    /// <summary>
    /// Encapsulates a method that has two parameters and does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    public delegate void ActionHandler<T1, T2>(T1 parameter1, T2 parameter2);

    /// <summary>
    /// Encapsulates a method that has three parameters and does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    public delegate void ActionHandler<T1, T2, T3>(T1 parameter1, T2 parameter2, T3 parameter3);

    /// <summary>
    /// Encapsulates a method that has four parameters and does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter4">The fourth parameter of the method that this delegate encapsulates.</param>
    public delegate void ActionHandler<T1, T2, T3, T4>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4);

    /// <summary>
    /// Encapsulates a method that has five parameters and does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter4">The fourth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter5">The fifth parameter of the method that this delegate encapsulates.</param>
    public delegate void ActionHandler<T1, T2, T3, T4, T5>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5);

    /// <summary>
    /// Encapsulates a method that has six parameters and does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter4">The fourth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter5">The fifth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter6">The sixth parameter of the method that this delegate encapsulates.</param>
    public delegate void ActionHandler<T1, T2, T3, T4, T5, T6>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6);

    /// <summary>
    /// Encapsulates a method that has seven parameters and does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter4">The fourth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter5">The fifth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter6">The sixth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter7">The seventh parameter of the method that this delegate encapsulates.</param>
    public delegate void ActionHandler<T1, T2, T3, T4, T5, T6, T7>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7);

    /// <summary>
    /// Encapsulates a method that has eight parameters and does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter4">The fourth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter5">The fifth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter6">The sixth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter7">The seventh parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter8">The eighth parameter of the method that this delegate encapsulates.</param>
    public delegate void ActionHandler<T1, T2, T3, T4, T5, T6, T7, T8>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7, T8 parameter8);

    /// <summary>
    /// Encapsulates a method that has eight parameters and does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T9">The type of the nineth parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter4">The fourth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter5">The fifth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter6">The sixth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter7">The seventh parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter8">The eighth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter9">The nineth parameter of the method that this delegate encapsulates.</param>
    public delegate void ActionHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7, T8 parameter8, T9 parameter9);

    /// <summary>
    /// Encapsulates a method that has eight parameters and does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T6">The type of the sixth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T7">The type of the seventh parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T8">The type of the eighth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T9">The type of the nineth parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T10">The type of the tenth parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="parameter1">The first parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter2">The second parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter3">The third parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter4">The fourth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter5">The fifth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter6">The sixth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter7">The seventh parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter8">The eighth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter9">The nineth parameter of the method that this delegate encapsulates.</param>
    /// <param name="parameter10">The tenth parameter of the method that this delegate encapsulates.</param>
    public delegate void ActionHandler<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7, T8 parameter8, T9 parameter9, T10 parameter10);
    #endregion

    #region Generic Event Delegates
    /// <summary>
    /// Encapsulates a method that has no event parameters.
    /// </summary>
    /// <param name="sender">The object that has sent the message.</param>
    public delegate void EventHandler(Object sender);

    /// <summary>
    /// Encapsulates a method that has one event parameter.
    /// </summary>
    /// <typeparam name="T1">The type of the first event argument parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="sender">The object that has sent the message.</param>
    /// <param name="e1">The first event argument parameter of the method that this delegate encapsulates.</param>
    public delegate void EventHandler<T1>(Object sender, T1 e1);

    /// <summary>
    /// Encapsulates a method that has two event parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first event argument parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second event argument parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="sender">The object that has sent the message.</param>
    /// <param name="e1">The first event argument parameter of the method that this delegate encapsulates.</param>
    /// <param name="e2">The second event argument parameter of the method that this delegate encapsulates.</param>
    public delegate void EventHandler<T1, T2>(Object sender, T1 e1, T2 e2);

    /// <summary>
    /// Encapsulates a method that has three event parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first event argument parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second event argument parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third event argument parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="sender">The object that has sent the message.</param>
    /// <param name="e1">The first event argument parameter of the method that this delegate encapsulates.</param>
    /// <param name="e2">The second event argument parameter of the method that this delegate encapsulates.</param>
    /// <param name="e3">The third event argument parameter of the method that this delegate encapsulates.</param>
    public delegate void EventHandler<T1, T2, T3>(Object sender, T1 e1, T2 e2, T3 e3);

    /// <summary>
    /// Encapsulates a method that has four event parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first event argument parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second event argument parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third event argument parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth event argument parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="sender">The object that has sent the message.</param>
    /// <param name="e1">The first event argument parameter of the method that this delegate encapsulates.</param>
    /// <param name="e2">The second event argument parameter of the method that this delegate encapsulates.</param>
    /// <param name="e3">The third event argument parameter of the method that this delegate encapsulates.</param>
    /// <param name="e4">The fourth event argument parameter of the method that this delegate encapsulates.</param>
    public delegate void EventHandler<T1, T2, T3, T4>(Object sender, T1 e1, T2 e2, T3 e3, T4 e4);

    /// <summary>
    /// Encapsulates a method that has five event parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first event argument parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T2">The type of the second event argument parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T3">The type of the third event argument parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T4">The type of the fourth event argument parameter of the method that this delegate encapsulates.</typeparam>
    /// <typeparam name="T5">The type of the fifth event argument parameter of the method that this delegate encapsulates.</typeparam>
    /// <param name="sender">The object that has sent the message.</param>
    /// <param name="e1">The first event argument parameter of the method that this delegate encapsulates.</param>
    /// <param name="e2">The second event argument parameter of the method that this delegate encapsulates.</param>
    /// <param name="e3">The third event argument parameter of the method that this delegate encapsulates.</param>
    /// <param name="e4">The fourth event argument parameter of the method that this delegate encapsulates.</param>
    /// <param name="e5">The fifth event argument parameter of the method that this delegate encapsulates.</param>
    public delegate void EventHandler<T1, T2, T3, T4, T5>(Object sender, T1 e1, T2 e2, T3 e3, T4 e4, T5 e5);
    #endregion

    /// <summary>
    /// A generic base class for IAsyncResult implementations.
    /// </summary>
    public abstract class AsyncResult : IAsyncResult
    {
        #region Constructors
        /// <summary>
        /// Constructor for the class.
        /// </summary>
        /// <param name="callback">The method reference when the operation completes.</param>
        /// <param name="state">The object containing the custom state.</param>
        protected AsyncResult(AsyncCallback callback, object state)
        {
            this.callback = callback;
            this.state = state;
            this.thisLock = new object();
        }
        #endregion

        #region Fields
        AsyncCallback callback = null;
        ManualResetEvent manualResetEvent = null;
        Exception exception = null;

        object state = null;
        object thisLock = null;
        bool endCalled = false;
        bool isCompleted = false;
        bool completedSynchronously = false;
        #endregion

        #region Properties
        /// <summary>
        /// Get the object containing the custom state.
        /// </summary>
        public object AsyncState
        {
            get { return state; }
        }
        
        /// <summary>
        /// Get the current thread wait handler.
        /// </summary>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                // If the event is not null.
                // Return the thread event handler.
                if (manualResetEvent != null)
                    return manualResetEvent;

                // Lock the current object.
                lock (ThisLock)
                    // Create a new maunal reset event to
                    // notify threads of a completed operation.
                    if (manualResetEvent == null)
                        manualResetEvent = new ManualResetEvent(isCompleted);

                // Return the object.
                return manualResetEvent;
            }
        }

        /// <summary>
        /// Get the completed synchronously indicator.
        /// </summary>
        public bool CompletedSynchronously
        {
            get { return completedSynchronously; }
        }

        /// <summary>
        /// Get the is operation complete indicator.
        /// </summary>
        public bool IsCompleted
        {
            get { return isCompleted; }
        }

        /// <summary>
        /// Get the current object create.
        /// </summary>
        private object ThisLock
        {
            get { return this.thisLock; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the completed synchronously value, Indicates that
        /// the operation has completed or not.
        /// </summary>
        /// <param name="completedSynchronously">Has the operation completed.</param>
        /// <remarks>Call this version of complete when your asynchronous 
        /// operation is complete.  This will update the state
        /// of the operation and notify the callback.</remarks>
        /// <exception cref="System.InvalidOperationException">Cannot call complete twice.</exception>
        protected void Complete(bool completedSynchronously)
        {
            if (isCompleted)
                // It's a bug to call Complete twice.
                throw new InvalidOperationException("Cannot call complete twice.");

            // Assign the completed synchronously indicator.
            this.completedSynchronously = completedSynchronously;

            // If the operation was completed.
            if (completedSynchronously)
                this.isCompleted = true;
            else
                // Lock the current thread.
                lock (ThisLock)
                {
                    this.isCompleted = true;

                    // Indicate to all the threrads that
                    // the operation has completed.
                    if (this.manualResetEvent != null)
                        this.manualResetEvent.Set();
                }
            
            // If the callback throws, there is a bug in the 
            // callback implementation. Send the AsyncCallback
            // method a message indicating the operation has
            // completed. Call the AsyncCallback delegate to the
            // reference client thread method.
            if (callback != null)
                callback(this);
        }

        /// <summary>
        /// Sets the completed synchronously value, Indicates that
        /// the operation has completed or not.
        /// </summary>
        /// <param name="completedSynchronously">Has the operation completed.</param>
        /// <param name="exception">The exception the has occured.</param>
        /// <remarks>Call this version of complete if you raise an 
        /// exception during processing.  In addition to notifying
        /// the callback, it will capture the exception and store 
        /// it to be thrown during AsyncResult.End.</remarks>
        protected void Complete(bool completedSynchronously, Exception exception)
        {
            this.exception = exception;
            Complete(completedSynchronously);
        }
        #endregion

        #region Static Data Type Methods
        /// <summary>
        /// End the asynchronous result operation.
        /// </summary>
        /// <typeparam name="TAsyncResult">A strongly typed AsynchronousResult.</typeparam>
        /// <param name="result">The status of the asynchronous operation.</param>
        /// <returns>A strongly typed AsynchronousResult.</returns>
        /// <remarks>End should be called when the End function for the 
        /// asynchronous operation is complete. It ensures the asynchronous 
        /// operation is complete, and does some common validation.</remarks>
        /// <exception cref="System.ArgumentNullException">IAsyncResult is null.</exception>
        /// <exception cref="System.ArgumentException">Invalid async result.</exception>
        /// <exception cref="System.InvalidOperationException">Async object already ended.</exception>
        protected static TAsyncResult End<TAsyncResult>(IAsyncResult result)
            where TAsyncResult : AsyncResult
        {
            if (result == null)
                throw new ArgumentNullException("result");

            // Get the current AsyncResult.
            TAsyncResult asyncResult = result as TAsyncResult;

            // If no asynchronous operation exists.
            if (asyncResult == null)
                throw new ArgumentException("Invalid async result.", "result");

            // If the asynchronous operation has already ended.
            if (asyncResult.endCalled)
                throw new InvalidOperationException("Async object already ended.");

            // Set the asynchronous operation as ended.
            asyncResult.endCalled = true;

            // If the asynchronous result has not completed
            // then wait until it does.
            if (!asyncResult.isCompleted)
                asyncResult.AsyncWaitHandle.WaitOne();

            // Close the thread event handler.
            if (asyncResult.manualResetEvent != null)
                asyncResult.manualResetEvent.Close();

            // If an exception has occured then
            // throw the excetion to the client.
            if (asyncResult.exception != null)
                throw asyncResult.exception;

            // Return the completed result.
            return asyncResult;
        }
        #endregion
    }

    /// <summary>
    /// A strongly typed asynchronous result.
    /// </summary>
    /// <typeparam name="T">The type of data that is to be returned.</typeparam>
    public abstract class AsynchronousResult<T> : AsyncResult
    {
        #region Constructors
        /// <summary>
        /// Constructor for the class.
        /// </summary>
        /// <param name="callback">The method reference when the operation completes.</param>
        /// <param name="state">The object containing the custom state.</param>
        protected AsynchronousResult(AsyncCallback callback, object state)
            : base(callback, state){ }
        #endregion

        #region Fields
        // The current data type to return.
        private T data;
        #endregion

        #region Properties
        /// <summary>
        /// The data returned after execution has
        /// completed.
        /// </summary>
        public T Data
        {
            get { return data; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets the completion status of the operation.
        /// </summary>
        /// <param name="data">The type containing the data to return.</param>
        /// <param name="completedSynchronously">Has the operation completed.</param>
        protected void Complete(T data, bool completedSynchronously)
        {
            this.data = data;
            base.Complete(completedSynchronously);
        }
        #endregion

        #region Static Data Type Methods
        /// <summary>
        /// Ends the operation and returns the data.
        /// </summary>
        /// <param name="result">The current asynchronous result.</param>
        /// <returns>The data type of the result.</returns>
        public static T End(IAsyncResult result)
        {
            // Return the strongly typed AsynchronousResult
            // and end the AsyncResult operation.
            AsynchronousResult<T> typedResult =
                AsyncResult.End<AsynchronousResult<T>>(result);

            // Returns the operation data type.
            return typedResult.Data;
        }
        #endregion
    }

    /// <summary>
    /// The thread item object.
    /// </summary>
    public sealed class WorkItem
    {
        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="waitCallback">The wait callback delegate.</param>
        /// <param name="state">The state object.</param>
        /// <param name="executionContext">The current execution context.</param>
        public WorkItem(WaitCallback waitCallback, Object state, ExecutionContext executionContext)
        {
            _callback = waitCallback;
            _state = state;
            _executionContext = executionContext;
        }
        #endregion

        #region Fields
        private WaitCallback _callback;
        private Object _state;
        private ExecutionContext _executionContext;
        #endregion

        #region Properties
        /// <summary>
        /// Gets, the wait callback delegate.
        /// </summary>
        public WaitCallback Callback 
        { 
            get { return _callback; } 
        }
        
        /// <summary>
        /// Gets, the state object.
        /// </summary>
        public Object State
        {
            get { return _state; }
        }

        /// <summary>
        /// Gets, the current execution context.
        /// </summary>
        public ExecutionContext Context 
        { 
            get { return _executionContext; }
        }
        #endregion
    }

    /// <summary>
    /// The thread item object.
    /// </summary>
    public sealed class WorkItemExtended
    {
        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="waitCallback">The wait callback delegate.</param>
        /// <param name="state">The state object.</param>
        /// <param name="executionContext">The current execution context.</param>
        public WorkItemExtended(WaitCallback waitCallback, WorkItemExtendedState state, ExecutionContext executionContext)
        {
            _callback = waitCallback;
            _state = state;
            _executionContext = executionContext;
        }
        #endregion

        #region Fields
        private WaitCallback _callback;
        private WorkItemExtendedState _state;
        private ExecutionContext _executionContext;
        #endregion

        #region Properties
        /// <summary>
        /// Gets, the wait callback delegate.
        /// </summary>
        public WaitCallback Callback
        {
            get { return _callback; }
        }

        /// <summary>
        /// Gets, the state object.
        /// </summary>
        public WorkItemExtendedState State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// Gets, the current execution context.
        /// </summary>
        public ExecutionContext Context
        {
            get { return _executionContext; }
        }
        #endregion
    }

    /// <summary>
    /// The work item state extended object.
    /// </summary>
    public sealed class WorkItemExtendedState
    {
        #region Fields
        private Object _state;
        private WorkItemExtended _workItem;
        #endregion

        #region Properties
        /// <summary>
        /// Gets sets, the state object.
        /// </summary>
        public Object State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// Gets, the current work item.
        /// </summary>
        public WorkItemExtended WorkItem
        {
            get { return _workItem; }
            set { _workItem = value; }
        }
        #endregion
    }

    /// <summary>
    /// Thread work item status.
    /// </summary>
    public enum WorkItemStatus : int
    {
        #region Work Item Status Enums
        /// <summary>
        /// Completed thread status.
        /// </summary>
        Completed = 1, 
        /// <summary>
        /// Queued thread status.
        /// </summary>
        Queued = 2, 
        /// <summary>
        /// Executing thread status.
        /// </summary>
        Executing = 3, 
        /// <summary>
        /// Aborted thread status.
        /// </summary>
        Aborted = 4 
        #endregion
    }

    /// <summary>
    /// Abortable thread pool.
    /// </summary>
    public static class ThreadPoolContext
    {
        #region Fields
        private static LinkedList<WorkItem> _callbacks = new LinkedList<WorkItem>();
        private static Dictionary<WorkItem, Thread> _threads = new Dictionary<WorkItem, Thread>();
        #endregion

        #region Private Static Methods
        /// <summary>
        /// Handles the current thread work item.
        /// </summary>
        /// <param name="state">The state object.</param>
        private static void HandleItem(object state)
        {
            WorkItem item = null;

            try
            {
                // Lock the current call back
                // work item.
                lock (_callbacks)
                {
                    if (_callbacks.Count > 0)
                    {
                        item = _callbacks.First.Value;
                        _callbacks.RemoveFirst();
                    }

                    // If the curent work item
                    // is null the return.
                    if (item == null) 
                        return;

                    // Add the current work item
                    // to the work item thread collection.
                    _threads.Add(item, Thread.CurrentThread);

                }
                
                // Execute the thread within
                // the context.
                ExecutionContext.Run(item.Context,
                    delegate { item.Callback(item.State); }, null);
            }
            finally
            {
                // Lock the current call back
                // and execute the removal of the
                // work item thread.
                lock (_callbacks)
                {
                    if (item != null) 
                        _threads.Remove(item);
                }
            }
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Queues a method for execution. The method executes when a thread pool thread
        /// becomes available.
        /// </summary>
        /// <param name="callback">A System.Threading.WaitCallback that represents the method to be executed.</param>
        /// <returns>The current work item.</returns>
        public static WorkItem QueueUserWorkItem(WaitCallback callback)
        {
            return QueueUserWorkItem(callback, null);
        }

        /// <summary>
        /// Queues a method for execution. The method executes when a thread pool thread
        /// becomes available.
        /// </summary>
        /// <param name="callback">A System.Threading.WaitCallback that represents the method to be executed.</param>
        /// <param name="state">An object containing data to be used by the method.</param>
        /// <returns>The current work item.</returns>
        public static WorkItem QueueUserWorkItem(WaitCallback callback, Object state)
        {
            // Make sure a callback delegate has been specified.
            if (callback == null) throw new ArgumentNullException("callback");

            // Create a new work item and
            // capture the current thread context.
            WorkItem item = new WorkItem(callback, state, ExecutionContext.Capture());

            // Lock the call back and
            // add the work item to the
            // work item collection.
            lock (_callbacks)
                _callbacks.AddLast(item);

            // Start a new worker thread queue item
            ThreadPool.QueueUserWorkItem(new WaitCallback(HandleItem));

            // Return the current thread item.
            return item;
        }

        /// <summary>
        /// Is the work item a member of the thread collection.
        /// </summary>
        /// <param name="item">The work item to test.</param>
        /// <returns>True if the work items is in the collection else false.</returns>
        public static bool IsWorkItemInThreads(WorkItem item)
        {
            // Make sure a callback delegate has been specified.
            if (item == null) throw new ArgumentNullException("item");

            // Is the work item in the current
            // collection of threads
            if (_threads.ContainsKey(item))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Cancel the current work item thread
        /// </summary>
        /// <param name="item">The work item to cancel.</param>
        /// <param name="allowAbort">Can the work item be aborted.</param>
        /// <returns>The work item status.</returns>
        public static WorkItemStatus Cancel(WorkItem item, bool allowAbort)
        {
            // Make sure a work item
            // has been passed.
            if (item == null) throw new ArgumentNullException("item");

            // Lock the call back.
            lock (_callbacks)
            {
                // Find the current work item
                // in the collection.
                LinkedListNode<WorkItem> node = _callbacks.Find(item);

                // If the work item is not null
                if (node != null)
                {
                    // The remove the work item from
                    // the collection, and return
                    // the status as queued.
                    _callbacks.Remove(node);
                    return WorkItemStatus.Queued;
                }
                    // Else if the thread collection
                    // contains the work thread, indicating
                    // that the thread is running
                else if (_threads.ContainsKey(item))
                {
                    // If aborting the thread
                    if (allowAbort)
                    {
                        // Abort the current thread.
                        // Remove the work item from
                        // the collection.
                        _threads[item].Abort();
                        _threads.Remove(item);
                        return WorkItemStatus.Aborted;
                    }
                    else 
                        // Current thread is executing.
                        return WorkItemStatus.Executing;
                }
                else 
                    // Current thread is complete.
                    return WorkItemStatus.Completed;
            }
        }
        #endregion
    }

    /// <summary>
    /// Abortable thread pool.
    /// </summary>
    public static class ThreadPoolContextExtended
    {
        #region Fields
        private static LinkedList<WorkItemExtended> _callbacks = new LinkedList<WorkItemExtended>();
        private static Dictionary<WorkItemExtended, Thread> _threads = new Dictionary<WorkItemExtended, Thread>();
        #endregion

        #region Private Static Methods
        /// <summary>
        /// Handles the current thread work item.
        /// </summary>
        /// <param name="state">The state object.</param>
        private static void HandleItem(object state)
        {
            WorkItemExtended item = null;

            try
            {
                // Lock the current call back
                // work item.
                lock (_callbacks)
                {
                    if (_callbacks.Count > 0)
                    {
                        item = _callbacks.First.Value;
                        _callbacks.RemoveFirst();
                    }

                    // If the curent work item
                    // is null the return.
                    if (item == null)
                        return;

                    // Add the current work item
                    // to the work item thread collection.
                    _threads.Add(item, Thread.CurrentThread);

                }

                // Execute the thread within
                // the context.
                ExecutionContext.Run(item.Context,
                    delegate { item.Callback(item.State); }, null);
            }
            finally
            {
                // Lock the current call back
                // and execute the removal of the
                // work item thread.
                lock (_callbacks)
                {
                    if (item != null)
                        _threads.Remove(item);
                }
            }
        }
        #endregion

        #region Public Static Methods
        /// <summary>
        /// Queues a method for execution. The method executes when a thread pool thread
        /// becomes available.
        /// </summary>
        /// <param name="callback">A System.Threading.WaitCallback that represents the method to be executed.</param>
        /// <param name="state">An object containing data to be used by the method.</param>
        /// <returns>The current work item.</returns>
        public static WorkItemExtended QueueUserWorkItem(WaitCallback callback, WorkItemExtendedState state)
        {
            // Make sure a callback delegate has been specified.
            if (callback == null) throw new ArgumentNullException("callback");
            if (state == null) throw new ArgumentNullException("state");

            // Create a new work item and
            // capture the current thread context.
            WorkItemExtended item = new WorkItemExtended(callback, state, ExecutionContext.Capture());
            state.WorkItem = item;
            item.State = state;

            // Lock the call back and
            // add the work item to the
            // work item collection.
            lock (_callbacks)
                _callbacks.AddLast(item);

            // Start a new worker thread queue item
            ThreadPool.QueueUserWorkItem(new WaitCallback(HandleItem));

            // Return the current thread item.
            return item;
        }

        /// <summary>
        /// Is the work item a member of the thread collection.
        /// </summary>
        /// <param name="item">The work item to test.</param>
        /// <returns>True if the work items is in the collection else false.</returns>
        public static bool IsWorkItemInThreads(WorkItemExtended item)
        {
            // Make sure a callback delegate has been specified.
            if (item == null) throw new ArgumentNullException("item");

            // Is the work item in the current
            // collection of threads
            if (_threads.ContainsKey(item))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Cancel the current work item thread
        /// </summary>
        /// <param name="item">The work item to cancel.</param>
        /// <param name="allowAbort">Can the work item be aborted.</param>
        /// <returns>The work item status.</returns>
        public static WorkItemStatus Cancel(WorkItemExtended item, bool allowAbort)
        {
            // Make sure a work item
            // has been passed.
            if (item == null) throw new ArgumentNullException("item");

            // Lock the call back.
            lock (_callbacks)
            {
                // Find the current work item
                // in the collection.
                LinkedListNode<WorkItemExtended> node = _callbacks.Find(item);

                // If the work item is not null
                if (node != null)
                {
                    // The remove the work item from
                    // the collection, and return
                    // the status as queued.
                    _callbacks.Remove(node);
                    return WorkItemStatus.Queued;
                }
                // Else if the thread collection
                // contains the work thread, indicating
                // that the thread is running
                else if (_threads.ContainsKey(item))
                {
                    // If aborting the thread
                    if (allowAbort)
                    {
                        // Abort the current thread.
                        // Remove the work item from
                        // the collection.
                        _threads[item].Abort();
                        _threads.Remove(item);
                        return WorkItemStatus.Aborted;
                    }
                    else
                        // Current thread is executing.
                        return WorkItemStatus.Executing;
                }
                else
                    // Current thread is complete.
                    return WorkItemStatus.Completed;
            }
        }
        #endregion
    }
}
