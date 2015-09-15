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

namespace Nequeo.Wpf.Views
{
    /// <summary>
    /// An interface defining how navigation between pages should
    /// be performed in various frameworks such as Windows, 
    /// Windows Phone, Android, iOS etc.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// The key corresponding to the currently displayed page.
        /// </summary>
        string CurrentPageKey
        {
            get;
        }

        /// <summary>
        /// If possible, instructs the navigation service
        /// to discard the current page and display the previous page
        /// on the navigation stack.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Instructs the navigation service to display a new page
        /// corresponding to the given key. Depending on the platforms,
        /// the navigation service might have to be configured with a
        /// key/page list.
        /// </summary>
        /// <param name="pageKey">The key corresponding to the page
        /// that should be displayed.</param>
        void NavigateTo(string pageKey);

        /// <summary>
        /// Instructs the navigation service to display a new page
        /// corresponding to the given key, and passes a parameter
        /// to the new page.
        /// Depending on the platforms, the navigation service might 
        /// have to be Configure with a key/page list.
        /// </summary>
        /// <param name="pageKey">The key corresponding to the page
        /// that should be displayed.</param>
        /// <param name="parameter">The parameter that should be passed
        /// to the new page.</param>
        void NavigateTo(string pageKey, object parameter);
    }
}