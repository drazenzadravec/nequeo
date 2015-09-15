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
using System.ComponentModel;

using Nequeo.Extension;
using Nequeo.Collections;
using Nequeo.Collections.Extension;
using Nequeo.Wpf.Common;

namespace Nequeo.Wpf.UI
{
    /// <summary>
    /// Rich text box editor.
    /// </summary>
    public partial class RichTextEditor : Window
    {
        /// <summary>
        /// Rich text box editor.
        /// </summary>
        public RichTextEditor()
        {
            InitializeComponent();

            // Attach this windows data context
            // to the rich text editor binding context.
            this.DataContext = new RichTextEditorDataContext();
        }
    }

    /// <summary>
    /// Rich text editor data context binding helper.
    /// </summary>
    internal class RichTextEditorDataContext : ObservableBase
    {
        /// <summary>
        /// Rich text editor data context binding helper.
        /// </summary>
        public RichTextEditorDataContext()
        {
            // Assign the action handler for this property.
            GetXamlCommand = new DelegateCommand(() =>
            {
                MessageBox.Show(this.Text);
            });
        }

        private string _text = string.Empty;

        /// <summary>
        /// Gets the xaml command.
        /// </summary>
        public DelegateCommand GetXamlCommand { get; private set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text
        {
            get{ return _text; }
            set
            {
                _text = value;
                this.RaisePropertyChanged(p => p.Text);
            }
        }
    }
}
