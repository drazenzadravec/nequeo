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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Nequeo.Wpf.UI.Text
{
    /// <summary>
    /// Find dialog.
    /// </summary>
    public partial class FindDialog : Window
    {
        /// <summary>
        /// Find dialog.
        /// </summary>
        public FindDialog()
        {
            InitializeComponent();
            _findNext.IsEnabled = _replace.IsEnabled = _replaceAll.IsEnabled = !string.IsNullOrEmpty(_findWhat.Text);
        }

        /// <summary>
        /// Find next event.
        /// </summary>
        public event EventHandler FindNext;

        /// <summary>
        /// Rplace event.
        /// </summary>
        public event EventHandler Replace;

        /// <summary>
        /// Replace all event.
        /// </summary>
        public event EventHandler ReplaceAll;

        /// <summary>
        /// Gets or sets the find what text.
        /// </summary>
        public string FindWhat
        {
            get { return _findWhat.Text; }
            set
            {
                _findWhat.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the replace with text.
        /// </summary>
        public string ReplaceWith
        {
            get { return _replaceWith.Text; }
            set
            {
                _replaceWith.Text = value;
            }
        }

        /// <summary>
        /// Gets the match case indicator.
        /// </summary>
        public bool? MatchCase { get { return _matchCase.IsChecked; } }

        /// <summary>
        /// Gets the search up indicator.
        /// </summary>
        public bool? SearchUp { get { return _findUp.IsChecked; } }

        /// <summary>
        /// Show replace.
        /// </summary>
        public bool ShowReplace
        {
            get { return _replaceWith.Visibility == Visibility.Visible; }
            set
            {
                Visibility toBe;
                if (value)
                {
                    toBe = Visibility.Visible;
                    _directionGroupBox.Visibility = Visibility.Collapsed;
                    _findDown.IsChecked = true;
                }
                else
                {
                    toBe = Visibility.Collapsed;
                    _directionGroupBox.Visibility = Visibility.Visible;
                }
                _replaceLabel.Visibility = _replaceWith.Visibility = _replace.Visibility = _replaceAll.Visibility = toBe;
            }
        }

        /// <summary>
        /// Find next clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FindNextClicked(object sender, RoutedEventArgs e)
        {
            if (FindNext != null)
            {
                FindNext(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Replace clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ReplaceClicked(object sender, RoutedEventArgs e)
        {
            if (Replace != null)
            {
                Replace(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Replace all clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ReplaceAllClicked(object sender, RoutedEventArgs e)
        {
            if (ReplaceAll != null)
            {
                ReplaceAll(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Close clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CancelClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// On activated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnActivated(object sender, EventArgs e)
        {
            _findWhat.Focus();
        }

        /// <summary>
        /// Find text changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FindTextChanged(object sender, TextChangedEventArgs e)
        {
            _findNext.IsEnabled = _replace.IsEnabled = _replaceAll.IsEnabled = !string.IsNullOrEmpty(_findWhat.Text);
        }
    }
}
