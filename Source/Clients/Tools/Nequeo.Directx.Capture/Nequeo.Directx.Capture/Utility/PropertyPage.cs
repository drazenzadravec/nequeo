/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2012 http://www.nequeo.com.au/
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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using System.Linq;
using System.Text;

using DirectShowLib;
using DirectShowLib.BDA;
using DirectShowLib.DES;
using DirectShowLib.DMO;
using DirectShowLib.Dvd;
using DirectShowLib.MultimediaStreaming;
using DirectShowLib.SBE;

namespace Nequeo.Directx.Utility
{
    /// <summary>
    /// A base class for representing property pages exposed by filters.
    /// </summary>
    public class PropertyPage : IDisposable
    {
        /// <summary>
        /// A base class for representing property pages exposed by filters.
        /// </summary>
        public PropertyPage()
        {
        }

        /// <summary> Name of property page. This name may not be unique </summary>
        public string Name;

        /// <summary> Does this property page support saving and loading the user's choices. </summary>
        public bool SupportsPersisting = false;

        /// <summary> 
        ///  Get or set the state of the property page. This is used to save
        ///  and restore the user's choices without redisplaying the property page. 
        /// </summary>
        /// <remarks>
        ///  After showing this property page, read and store the value of 
        ///  this property. At a later time, the user's choices can be 
        ///  reloaded by setting this property with the value stored earlier. 
        ///  Note that some property pages, after setting this property, 
        ///  will not reflect the new state. However, the filter will use the
        ///  new settings. 
        ///  
        /// <para>
        ///  When reading this property, copy the entire array at once then manipulate
        ///  your local copy (e..g byte[] myState = propertyPage.State). When
        ///  setting this property set the entire array at once (e.g. propertyPage = myState).
        /// </para>
        ///  
        /// <para>
        ///  Not all property pages support saving/loading state. Check the 
        ///  <see cref="SupportsPersisting"/> property to determine if this 
        ///  property page supports it.
        /// </para>
        /// </remarks>
        public virtual byte[] State
        {
            get { throw new NotSupportedException("This property page does not support persisting state."); }
            set { throw new NotSupportedException("This property page does not support persisting state."); }
        }

        /// <summary> 
        ///  Show the property page. Some property pages cannot be displayed 
        ///  while previewing and/or capturing. This method will block until
        ///  the property page is closed by the user.
        /// </summary>
        public virtual void Show(Control owner)
        {
            throw new NotSupportedException("Not implemented. Use a derived class. ");
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
        public virtual void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
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
        ~PropertyPage()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
