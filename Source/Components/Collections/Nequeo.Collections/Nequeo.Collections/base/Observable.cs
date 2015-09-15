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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Nequeo.Collections
{
	/// <summary>
	/// The generic general observable collection class, collects the general objects.
	/// </summary>
	/// <typeparam name="T">The generic general type to collect.</typeparam>
	public class Observable<T> : ObservableCollection<T>
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public Observable()
		{
		}

		/// <summary>
		/// Add the range of types.
		/// </summary>
		/// <param name="data">The range of types to add.</param>
		public void AddRange(T[] data)
		{
			// Add each item to the collection.
			foreach (T item in data)
				Add(item);
		}

		/// <summary>
		/// Add the range of types.
		/// </summary>
		/// <param name="data">The range of types to add.</param>
		public void AddRange(IList<T> data)
		{
			// Add each item to the collection.
			foreach (T item in data)
				Add(item);
		}

		/// <summary>
		/// Add the range of types.
		/// </summary>
		/// <param name="data">The range of types to add.</param>
		public void AddRange(ICollection<T> data)
		{
			// Add each item to the collection.
			foreach (T item in data)
				Add(item);
		}

		/// <summary>
		/// Add the range of types.
		/// </summary>
		/// <param name="data">The range of types to add.</param>
		public void AddRange(Nequeo.Collections.Collection<T> data)
		{
			// Add each item to the collection.
			foreach (T item in data)
				Add(item);
		}

		/// <summary>
		/// Add the range of types.
		/// </summary>
		/// <param name="data">The range of types to add.</param>
		public void AddRange(Nequeo.Collections.BindingCollection<T> data)
		{
			// Add each item to the collection.
			foreach (T item in data)
				Add(item);
		}
	}

	/// <summary>
	/// Observable base.
	/// </summary>
	public abstract class ObservableBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
		/// <summary>
		/// Property changed event.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Property changing event, triggered when a property is changing.
        /// </summary>
        public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Provides access to the PropertyChanged event handler to derived classes.
        /// </summary>
        protected PropertyChangedEventHandler PropertyChangedHandler
        {
            get
            {
                return PropertyChanged;
            }
        }

        /// <summary>
        /// Provides access to the PropertyChanging event handler to derived classes.
        /// </summary>
        protected PropertyChangingEventHandler PropertyChangingHandler
        {
            get
            {
                return PropertyChanging;
            }
        }

        /// <summary>
        /// Raise the property changed event.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public void RaisePropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

        /// <summary>
        /// Raise the property changing event handle for the attached event.
        /// </summary>
        public void RaisePropertyChanging(string propertyName)
        {
            if ((this.PropertyChanging != null))
            {
                this.PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }
    }
}
