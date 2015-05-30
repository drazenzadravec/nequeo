/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Collections;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Nequeo.Convertible;

namespace Nequeo.Collections
{
    /// <summary>
    /// The generic general binding collection class, collects the general objects.
    /// </summary>
    /// <typeparam name="T">The generic general type to collect.</typeparam>
    public class BindingCollection<T> : BindingList<T>
	{
        /// <summary>
        /// Default constructor
        /// </summary>
        public BindingCollection()
        {
        }
	}

    /// <summary>
    /// Number store collection.
    /// </summary>
    public class NumberCollection : System.Collections.IEnumerable, IDisposable
    {
        /// <summary>
        /// Number store collection.
        /// </summary>
        public NumberCollection() 
        { 
            _collection = new Collection<Number>();
        }

        private Collection<Number> _collection = null;

        /// <summary>
        /// Add the number.
        /// </summary>
        /// <param name="number1">A number.</param>
        public void Add(Number number1)
        {
            _collection.Add(number1);
        }

        /// <summary>
        /// Add the number.
        /// </summary>
        /// <param name="number1">A number.</param>
        /// <param name="number2">A number.</param>
        public void Add(Number number1, Number number2)
        {
            _collection.Add(number1);
            _collection.Add(number2);
        }

        /// <summary>
        /// Add the number.
        /// </summary>
        /// <param name="number1">A number.</param>
        /// <param name="number2">A number.</param>
        /// <param name="number3">A number.</param>
        public void Add(Number number1, Number number2, Number number3)
        {
            _collection.Add(number1);
            _collection.Add(number2);
            _collection.Add(number3);
        }

        /// <summary>
        /// Add the number.
        /// </summary>
        /// <param name="number1">A number.</param>
        /// <param name="number2">A number.</param>
        /// <param name="number3">A number.</param>
        /// <param name="number4">A number.</param>
        public void Add(Number number1, Number number2, Number number3, Number number4)
        {
            _collection.Add(number1);
            _collection.Add(number2);
            _collection.Add(number3);
            _collection.Add(number4);
        }

        /// <summary>
        /// Add the number.
        /// </summary>
        /// <param name="number1">A number.</param>
        /// <param name="number2">A number.</param>
        /// <param name="number3">A number.</param>
        /// <param name="number4">A number.</param>
        /// <param name="number5">A number.</param>
        public void Add(Number number1, Number number2, Number number3, Number number4, Number number5)
        {
            _collection.Add(number1);
            _collection.Add(number2);
            _collection.Add(number3);
            _collection.Add(number4);
            _collection.Add(number5);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator object that can be used to iterate through the collection.</returns>
        public IEnumerator GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        /// <summary>
        /// The list of all numbers separated by '\r\n'.
        /// </summary>
        /// <returns>Returns a string that represents the current object.</returns>
        public override string ToString()
        {
            StringBuilder numbers = new StringBuilder();

            // For each number.
            foreach (Number number in _collection)
            {
                // Get the number.
                numbers.Append(number.ToString() + "\r\n");
            }

            // Return the numbers.
            return numbers.ToString();
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // Note disposing has been done.
                disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if(_collection != null)
                        _collection.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _collection = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~NumberCollection()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }

    /// <summary>
    /// String store collection.
    /// </summary>
    public class StringCollection : System.Collections.IEnumerable, IDisposable
    {
        /// <summary>
        /// String store collection.
        /// </summary>
        public StringCollection()
        {
            _collection = new Collection<String>();
        }

        private Collection<String> _collection = null;

        /// <summary>
        /// Add the string.
        /// </summary>
        /// <param name="string1">A string.</param>
        public void Add(String string1)
        {
            _collection.Add(string1);
        }

        /// <summary>
        /// Add the string.
        /// </summary>
        /// <param name="string1">A string.</param>
        /// <param name="string2">A string.</param>
        public void Add(String string1, String string2)
        {
            _collection.Add(string1);
            _collection.Add(string2);
        }

        /// <summary>
        /// Add the string.
        /// </summary>
        /// <param name="string1">A string.</param>
        /// <param name="string2">A string.</param>
        /// <param name="string3">A string.</param>
        public void Add(String string1, String string2, String string3)
        {
            _collection.Add(string1);
            _collection.Add(string2);
            _collection.Add(string3);
        }

        /// <summary>
        /// Add the string.
        /// </summary>
        /// <param name="string1">A string.</param>
        /// <param name="string2">A string.</param>
        /// <param name="string3">A string.</param>
        /// <param name="string4">A string.</param>
        public void Add(String string1, String string2, String string3, String string4)
        {
            _collection.Add(string1);
            _collection.Add(string2);
            _collection.Add(string3);
            _collection.Add(string4);
        }

        /// <summary>
        /// Add the string.
        /// </summary>
        /// <param name="string1">A string.</param>
        /// <param name="string2">A string.</param>
        /// <param name="string3">A string.</param>
        /// <param name="string4">A string.</param>
        /// <param name="string5">A string.</param>
        public void Add(String string1, String string2, String string3, String string4, String string5)
        {
            _collection.Add(string1);
            _collection.Add(string2);
            _collection.Add(string3);
            _collection.Add(string4);
            _collection.Add(string5);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator object that can be used to iterate through the collection.</returns>
        public IEnumerator GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        /// <summary>
        /// The list of all strings separated by ' ' (space).
        /// </summary>
        /// <returns>Returns a string that represents the current object.</returns>
        public override string ToString()
        {
            StringBuilder strings = new StringBuilder();

            // For each string.
            foreach (string item in _collection)
            {
                // Get the string.
                strings.Append(item.ToString() + " ");
            }

            // Return the strings.
            return strings.ToString();
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // Note disposing has been done.
                disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (_collection != null)
                        _collection.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _collection = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~StringCollection()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
