/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;

namespace Nequeo.Extension
{
    /// <summary>
    /// Type extensions.
    /// </summary>
    public static class TypeExtenderExtensions
    {
        /// <summary>
        /// Get the name of the property from the expression.
        /// </summary>
        /// <typeparam name="T">The current type.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="owner">The current type.</param>
        /// <param name="expression">The property expression.</param>
        /// <returns>The name of the property.</returns>
        public static string GetPropertyName<T, TProperty>(this T owner, Expression<Func<T, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                if (unaryExpression != null)
                {
                    memberExpression = unaryExpression.Operand as MemberExpression;

                    if (memberExpression == null)
                        throw new NotImplementedException();
                }
                else
                    throw new NotImplementedException();
            }

            // Extract the name of the property from the expression.
            var propertyName = memberExpression.Member.Name;
            return propertyName;
        }

        /// <summary>
        /// Get the name of the property from the expression.
        /// </summary>
        /// <typeparam name="T">The current type.</typeparam>
        /// <param name="owner">The current type.</param>
        /// <param name="expression">The property expression.</param>
        /// <returns>The name of the property.</returns>
        public static string GetPropertyName<T>(this T owner, Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var body = expression.Body as MemberExpression;

            if (body == null)
            {
                throw new ArgumentException("Invalid argument", "propertyExpression");
            }

            var property = body.Member as PropertyInfo;

            if (property == null)
            {
                throw new ArgumentException("Argument is not a property", "propertyExpression");
            }

            return property.Name;
        }
    }
}
