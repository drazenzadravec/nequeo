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
using System.IO;
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

namespace Nequeo.Wpf.UI
{
    /// <summary>
    /// New folder dialog.
    /// </summary>
    public partial class NewFolderDialog : Window
    {
        /// <summary>
        /// New folder dialog.
        /// </summary>
        /// <param name="path">The folder path.</param>
        public NewFolderDialog(string path = "")
        {
            InitializeComponent();

            textBoxFolderName.Focus();
            _path = path;
            IsFolder = true;
        }

        private string _path = null;

        /// <summary>
        /// Gets or sets the dialog result.
        /// </summary>
        public new DialogResult DialogResult { set; get; }

        /// <summary>
        /// Gets or sets a value indicating whether the new object should be a folder or a file. DEFAULT: True
        /// </summary>
        /// <value><c>true</c> if the new object should be a folder; otherwise, <c>false</c>.</value>
        public bool IsFolder { get; set; }

        /// <summary>
        /// Gets or sets the name of the folder.
        /// </summary>
        /// <value>The name of the folder.</value>
        public string FolderName
        {
            get
            {
                return this.textBoxFolderName.Text;
            }
            set
            {
                this.textBoxFolderName.Text = value;
            }
        }

        /// <summary>
        /// OK clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOK_clicked(object sender, RoutedEventArgs e)
        {
            // Create the folder.
            DialogResult = DialogResult.OK;
            CreateFolder();
            this.Close();
        }

        /// <summary>
        /// Cancel clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCancel_clicked(object sender, RoutedEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Text box folder name key up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxFolderName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                DialogResult = DialogResult.OK;
                CreateFolder();
                this.Close();
            }
            e.Handled = true;
        }

        /// <summary>
        /// Create the new folder.
        /// </summary>
        private void CreateFolder()
        {
            if (this.DialogResult == DialogResult.OK)
            {
                string path = _path;
                if (path == "")
                    path = "C:\\Temp\\";

                if (!path.EndsWith("\\"))
                    path += "\\";

                path += this.FolderName;

                try
                {
                    if (this.IsFolder)
                    {
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                    }
                    else
                    {
                        if (!File.Exists(path))
                        {
                            FileStream fs = new FileStream(path, FileMode.Create);
                            fs.Close();
                        }
                    }
                }
                catch (IOException ex)
                {
                    throw ex;
                }
            }
        }
    }
}
