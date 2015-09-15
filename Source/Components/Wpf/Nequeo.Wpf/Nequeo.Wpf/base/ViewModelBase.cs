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
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Linq.Expressions;

using Nequeo.Wpf.Messaging;
using Nequeo.Wpf.Common;
using Nequeo.Collections;
using Nequeo.Extension;
using Nequeo.Reflection;

namespace Nequeo.Wpf
{
    /// <summary>
    /// A base class for the ViewModel classes in the MVVM pattern.
    /// </summary>
    /// <typeparam name="TImplementation">The base interface implementation.</typeparam>
    public abstract class ViewModelBase<TImplementation> : ViewModelBase
        where TImplementation : class
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        protected ViewModelBase()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        /// <param name="messenger">An instance of a <see cref="Messenger" />
        /// used to broadcast messages to other objects. If null, this class
        /// will attempt to broadcast using the Messenger's default
        /// instance.</param>
        protected ViewModelBase(IMessenger messenger)
            : base(messenger)
        {
        }

        private string _implementationName = string.Empty;
        private TImplementation _implementation = default(TImplementation);

        /// <summary>
        /// Gets or sets the implementation. Initially this is set from the configuration file.
        /// </summary>
        public TImplementation Implementation
        {
            get
            {
                // If null then create the instance from the configuration file.
                if(_implementation == default(TImplementation))
                {
                    // Create the instance of the implementation type.
                    Nequeo.Configuration.Reader reader = new Configuration.Reader();
                    string refProvider = reader.GetReflectionProvider(_implementationName);
                    _implementation = reader.CreateReflectionProvider<TImplementation>(refProvider);
                }
                return _implementation;
            }
            set { _implementation = value; }
        }

        /// <summary>
        /// Gets or sets the implementation name to use when creating the instance.
        /// </summary>
        public string ImplementationName
        {
            get { return _implementationName; }
            set { _implementationName = value; }
        }
    }

    /// <summary>
    /// A base class for the ViewModel classes in the MVVM pattern.
    /// </summary>
    public abstract class ViewModelBase : ObservableBase, ICleanup, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        protected ViewModelBase()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        /// <param name="messenger">An instance of a <see cref="Messenger" />
        /// used to broadcast messages to other objects. If null, this class
        /// will attempt to broadcast using the Messenger's default
        /// instance.</param>
        protected ViewModelBase(IMessenger messenger)
        {
            MessengerInstance = messenger;
        }

        private static bool? _isInDesignMode;
        private IMessenger _messengerInstance;

        /// <summary>
        /// Release any resources used.
        /// </summary>
        protected abstract void Release();

        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running under Blend or Visual Studio).
        /// </summary>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Non static member needed for data binding")]
        public bool IsInDesignMode
        {
            get
            {
                return IsInDesignModeStatic;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running in Blend or Visual Studio).
        /// </summary>
        [SuppressMessage(
            "Microsoft.Security",
            "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands",
            Justification = "The security risk here is neglectible.")]
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    _isInDesignMode
                        = (bool)DependencyPropertyDescriptor
                                        .FromProperty(prop, typeof(FrameworkElement))
                                        .Metadata.DefaultValue;
                }

                return _isInDesignMode.Value;
            }
        }

        /// <summary>
        /// Gets or sets an instance of a <see cref="IMessenger" /> used to
        /// broadcast messages to other objects. If null, this class will
        /// attempt to broadcast using the Messenger's default instance.
        /// </summary>
        protected IMessenger MessengerInstance
        {
            get
            {
                return _messengerInstance ?? Messenger.Default;
            }
            set
            {
                _messengerInstance = value;
            }
        }

        /// <summary>
        /// Unregisters this instance from the Messenger class.
        /// <para>To cleanup additional resources, override this method, clean
        /// up and then call base.Cleanup().</para>
        /// </summary>
        public virtual void Cleanup()
        {
            MessengerInstance.Unregister(this);
        }

        /// <summary>
        /// Broadcasts a PropertyChangedMessage using either the instance of
        /// the Messenger that was passed to this class (if available) 
        /// or the Messenger's default instance.
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="oldValue">The value of the property before it
        /// changed.</param>
        /// <param name="newValue">The value of the property after it
        /// changed.</param>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        protected virtual void Broadcast<T>(T oldValue, T newValue, string propertyName)
        {
            var message = new PropertyChangedMessage<T>(this, oldValue, newValue, propertyName);
            MessengerInstance.Send(message);
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available).
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        /// <param name="oldValue">The property's value before the change
        /// occurred.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        /// <remarks>If the propertyName parameter
        /// does not correspond to an existing property on the current class, an
        /// exception is thrown in DEBUG configuration only.</remarks>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1026:DefaultParametersShouldNotBeUsed"),
        SuppressMessage(
            "Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        protected virtual void RaisePropertyChanged<T>(
            string propertyName,
            T oldValue = default(T),
            T newValue = default(T),
            bool broadcast = false)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("This method cannot be called with an empty string", "propertyName");
            }

            RaisePropertyChanged(propertyName);

            if (broadcast)
            {
                Broadcast(oldValue, newValue, propertyName);
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available).
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property
        /// that changed.</param>
        /// <param name="oldValue">The property's value before the change
        /// occurred.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        [SuppressMessage(
            "Microsoft.Design",
            "CA1006:GenericMethodsShouldProvideTypeParameter",
            Justification = "This syntax is more convenient than the alternatives.")]
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression, T oldValue, T newValue, bool broadcast)
        {
            var handler = this.PropertyChangedHandler;

            if (handler != null
                || broadcast)
            {
                var propertyName = TypeAccessor.GetPropertyName(propertyExpression);

                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }

                if (broadcast)
                {
                    Broadcast(oldValue, newValue, propertyName);
                }
            }
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available). 
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyExpression">An expression identifying the property
        /// that changed.</param>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        /// <returns>True if the PropertyChanged event was raised, false otherwise.</returns>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "This syntax is more convenient than the alternatives."),
         SuppressMessage(
            "Microsoft.Design",
            "CA1045:DoNotPassTypesByReference",
            MessageId = "1#")]
        protected bool Set<T>(
            Expression<Func<T>> propertyExpression,
            ref T field,
            T newValue,
            bool broadcast)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            RaisePropertyChanging(TypeAccessor.GetPropertyName(propertyExpression));

            var oldValue = field;
            field = newValue;
            RaisePropertyChanged(propertyExpression, oldValue, field, broadcast);
            return true;
        }

        /// <summary>
        /// Assigns a new value to the property. Then, raises the
        /// PropertyChanged event if needed, and broadcasts a
        /// PropertyChangedMessage using the Messenger instance (or the
        /// static default instance if no Messenger instance is available). 
        /// </summary>
        /// <typeparam name="T">The type of the property that
        /// changed.</typeparam>
        /// <param name="propertyName">The name of the property that
        /// changed.</param>
        /// <param name="field">The field storing the property's value.</param>
        /// <param name="newValue">The property's value after the change
        /// occurred.</param>
        /// <param name="broadcast">If true, a PropertyChangedMessage will
        /// be broadcasted. If false, only the event will be raised.</param>
        /// <returns>True if the PropertyChanged event was raised, false otherwise.</returns>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1026:DefaultParametersShouldNotBeUsed"),
         SuppressMessage(
            "Microsoft.Design",
            "CA1045:DoNotPassTypesByReference",
            MessageId = "1#")]
        protected bool Set<T>(
            string propertyName,
            ref T field,
            T newValue = default(T),
            bool broadcast = false)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }

            RaisePropertyChanging(propertyName);

            var oldValue = field;
            field = newValue;

            RaisePropertyChanged(propertyName, oldValue, field, broadcast);

            return true;
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

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
            if (!_disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Cleanup
                    Cleanup();

                    // Release.
                    Release();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~ViewModelBase()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
