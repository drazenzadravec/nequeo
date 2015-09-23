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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nequeo.Wpf.UI
{
    /// <summary>
    /// Folder browser dialog.
    /// </summary>
    public partial class FolderBrowserDialog : Window
    {
        /// <summary>
        /// Folder browser dialog.
        /// </summary>
        /// <param name="initialPath">The initial folder path.</param>
        public FolderBrowserDialog(string initialPath = "")
        {
            InitializeComponent();

            // Add special folders.
            for (int i = 0; i <= 8; i++)
                _specialFolders.Add("");

            _specialFolders[(int)SpecialFolders.Desktop] = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _specialFolders[(int)SpecialFolders.Documents] = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _specialFolders[(int)SpecialFolders.Pictures] = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            _specialFolders[(int)SpecialFolders.Videos] = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            _specialFolders[(int)SpecialFolders.Computer] = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            _specialFolders[(int)SpecialFolders.Music] = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            _specialFolders[(int)SpecialFolders.StartMenu] = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            _specialFolders[(int)SpecialFolders.ProgramFiles] = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            _specialFolders[(int)SpecialFolders.ProgramFilesX86] = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            if (String.IsNullOrEmpty(initialPath))
                this.InitialPath = GetSpecialFolders(SpecialFolders.Desktop);
            else
                this.InitialPath = initialPath;

            SelectedPath = "";
            this.TextForComputer = "Computer";
            TreeDepth = 0;
        }

        private bool _lastValueOfEmptyFolders = true;
        private bool _lastValueOfHiddenFolders = true;
        private List<string> _specialFolders = new List<string>();

        /// <summary>
        /// Gets or sets the Root-Path of the folder browser.
        /// </summary>
        public string InitialPath { get; set; }

        /// <summary>
        /// Gets or sets how deep the tree should be shown.
        /// </summary>
        public int TreeDepth { get; set; }

        /// <summary>
        /// Gets or sets the dialog result.
        /// </summary>
        public new DialogResult DialogResult { set; get; }

        /// <summary>
        /// Gets or sets the selected path.
        /// </summary>
        public string SelectedPath { get; set; }

        /// <summary>
        /// Gets or sets the text for computer.
        /// </summary>
        public string TextForComputer { get; set; }

        /// <summary>
        /// Gets or sets the show of empty folders.
        /// </summary>
        /// <value>The show empty folders.</value>
        public bool? ShowEmptyFolders
        {
            get
            {
                return this.checkBoxEmptyFolders.IsChecked;
            }
            set
            {
                this.checkBoxEmptyFolders.IsChecked = value;
            }
        }

        /// <summary>
        /// Gets or sets the show of hidden folders.
        /// </summary>
        /// <value>The show hidden folders.</value>
        public bool? ShowHiddenFolders
        {
            get
            {
                return this.checkBoxHiddenFolders.IsChecked;
            }
            set
            {
                this.checkBoxHiddenFolders.IsChecked = value;
            }
        }

        /// <summary>
        /// Loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadFolders(this.InitialPath);
            this.treeViewFolders.Focus();
        }

        /// <summary>
        /// Check box empty folders checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxEmptyFolders_Checked(object sender, RoutedEventArgs e)
        {
            if (_lastValueOfEmptyFolders != true)
                CheckedChanged();

            _lastValueOfEmptyFolders = true;
            e.Handled = true;
        }

        /// <summary>
        /// Check box empty folders unchecked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxEmptyFolders_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_lastValueOfEmptyFolders != false)
                CheckedChanged();

            _lastValueOfEmptyFolders = false;
            e.Handled = true;
        }

        /// <summary>
        /// Check box hidden folders checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxHiddenFolders_Checked(object sender, RoutedEventArgs e)
        {
            if (_lastValueOfHiddenFolders != true)
                CheckedChanged();

            _lastValueOfHiddenFolders = true;
            e.Handled = true;
        }

        /// <summary>
        /// Check box hidden folders unchecked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxHiddenFolders_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_lastValueOfHiddenFolders != false)
                CheckedChanged();

            _lastValueOfHiddenFolders = false;
            e.Handled = true;
        }

        /// <summary>
        /// New folder clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewFolder_Click(object sender, RoutedEventArgs e)
        {
            NewFolderDialog nf = new NewFolderDialog(this.SelectedPath);
            nf.ShowDialog();
            if (nf.DialogResult == DialogResult.OK)
            {
                ReloadFolder(SelectedPath, nf.FolderName);
            }
            e.Handled = true;
        }

        /// <summary>
        /// OK clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = DialogResult.OK;
            try
            {
                this.SelectedPath = ((TreeViewItem)this.treeViewFolders.SelectedItem).ToolTip.ToString();
            }
            catch (Exception)
            {
                Exception own = new Exception("No valid folder chosen.");
                throw own;
            }
            this.Close();
        }

        /// <summary>
        /// Cancel clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Folder tree view selected item changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewFolders_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                this.SelectedPath = ((TreeViewItem)this.treeViewFolders.SelectedItem).ToolTip.ToString();
                this.btnNewFolder.IsEnabled = true;
            }
            catch (Exception)
            {
                this.btnNewFolder.IsEnabled = false;
            }
            e.Handled = true;
        }

        /// <summary>
        /// Get the special folder.
        /// </summary>
        /// <param name="folder">The special folder name.</param>
        /// <returns>The special folder path.</returns>
        public string GetSpecialFolders(SpecialFolders folder)
        {
            return _specialFolders[(int)folder];
        }

        /// <summary>
        /// Checked changed.
        /// </summary>
        private void CheckedChanged()
        {
            this.treeViewFolders.Items.Clear();
            ReloadFolder(this.InitialPath, this.SelectedPath);
        }

        /// <summary>
        /// Load the folders.
        /// </summary>
        /// <param name="initialPath">The initial folder path.</param>
        private void LoadFolders(string initialPath = "")
        {
            this.treeViewFolders.Items.Clear();
            if (initialPath != "") //
                this.InitialPath = initialPath;

            if (!Directory.Exists(this.InitialPath))
            {
                Exception e = new Exception("The Initial Path doesn't exist.");
                throw e;
            }

            if (this.SelectedPath != "")
            {
                if (!Directory.Exists(this.SelectedPath))
                {
                    Exception e = new Exception("The Selected Path doesn't exist.");
                    throw e;
                }

                if (!SelectedPathIsInInitialPath())
                {
                    Exception e = new Exception("The Selected Path isn't part of the Initial Path.");
                    throw e;
                }
            }

            treeViewFolders.Items.Add(LoadDirectories(this.InitialPath));
            ((TreeViewItem)treeViewFolders.Items[0]).IsExpanded = true;
            SelectPath();
        }

        /// <summary>
        /// Load the directories.
        /// </summary>
        /// <param name="path">The current path.</param>
        /// <returns>The tree view item.</returns>
        private TreeViewItem LoadDirectories(string path)
        {
            TreeViewItem tvi = CreateTreeVieItem(path);
            tvi.Items.Clear();
            if (this.InitialPath == GetSpecialFolders(SpecialFolders.Desktop))
            {
                LoadDesktop(tvi);
            }
            string[] folders = Directory.GetDirectories(path);
            for (int i = 0; i < folders.Length; i++)
            {
                try
                {
                    if (this.checkBoxEmptyFolders.IsChecked != true)
                    {
                        if (!FolderIsEmpty(folders[i]))
                            tvi.Items.Add(CreateTreeVieItem(folders[i]));
                    }
                    else if (this.checkBoxHiddenFolders.IsChecked != true)
                    {
                        if (!FolderIsHidden(folders[i]))
                            tvi.Items.Add(CreateTreeVieItem(folders[i]));
                    }
                    else
                        tvi.Items.Add(CreateTreeVieItem(folders[i]));
                }
                catch (Exception) //If there are no rights for the folder, bad solution but best now
                { }
            }
            return tvi;
        }

        /// <summary>
        /// Folder is empty.
        /// </summary>
        /// <param name="path">Current path.</param>
        /// <returns>True else false.</returns>
        private bool FolderIsEmpty(string path)
        {
            if (Directory.GetDirectories(path).Length > 0)
                return false;
            if (Directory.GetFiles(path).Length > 0)
                return false;
            return true;
        }

        /// <summary>
        /// Folder is hidden.
        /// </summary>
        /// <param name="path">Current path.</param>
        /// <returns>True else false.</returns>
        private bool FolderIsHidden(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            if (di.Attributes.ToString().Contains(FileAttributes.Hidden.ToString()))
                return true;
            return false;
        }

        /// <summary>
        /// Load the desktop.
        /// </summary>
        /// <param name="tvi">Tree view items.</param>
        private void LoadDesktop(TreeViewItem tvi)
        {
            tvi.Header = GetSpecialFolders(SpecialFolders.Desktop).Remove(0, GetSpecialFolders(SpecialFolders.Desktop).LastIndexOf("\\") + 1);
            tvi.ToolTip = GetSpecialFolders(SpecialFolders.Desktop);
            tvi.Items.Add(CreateTreeVieItem(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));

            // Create the desktop tree view.
            TreeViewItem computer = new TreeViewItem();
            computer.Header = this.TextForComputer;
            computer.Expanded += new RoutedEventHandler(computer_Expanded);
            computer.Selected += new RoutedEventHandler(tvi_Selected);
            computer.Items.Add("");
            tvi.Items.Add(computer);
        }

        /// <summary>
        /// Create a tree view item.
        /// </summary>
        /// <param name="path">The current folder path,</param>
        /// <returns>Tree view item.</returns>
        private TreeViewItem CreateTreeVieItem(string path)
        {
            TreeViewItem tvi = new TreeViewItem();
            string header = path.Remove(0, path.LastIndexOf("\\") + 1);
            if (header == "") //The Path is a Drive (e.g. C\, after removing \ -> "")
            {
                header = path.Remove(path.Length - 1);
            }

            // Create the tree view items.
            tvi.Header = header;
            tvi.ToolTip = path;
            tvi.MouseRightButtonDown += new MouseButtonEventHandler(treeViewItems_MouseRightButtonDown);
            if (this.TreeDepth == 0) //Without depth
            {
                if (path != "")
                {
                    if (Directory.GetDirectories(path).Length > 0)
                    {
                        tvi.Items.Add(new TreeViewItem());
                        tvi.Expanded += new RoutedEventHandler(tvi_Expanded);
                        tvi.Selected += new RoutedEventHandler(tvi_Selected);
                    }
                }
            }
            else //with depth
            {
                if (path != "" && GetDepth(path) < this.TreeDepth)
                {
                    if (Directory.GetDirectories(path).Length > 0)
                    {
                        tvi.Items.Add(new TreeViewItem());
                        tvi.Expanded += new RoutedEventHandler(tvi_Expanded);
                        tvi.Selected += new RoutedEventHandler(tvi_Selected);
                    }
                }
                else
                {
                    tvi.Expanded += new RoutedEventHandler(doNothing_Event);
                    tvi.Selected += new RoutedEventHandler(doNothing_Event);
                }
            }
            return tvi;
        }

        /// <summary>
        /// Get the depth.
        /// </summary>
        /// <param name="path">Current path.</param>
        /// <returns>The depth.</returns>
        private int GetDepth(string path)
        {
            int depth = GetNativeDepth(path);
            if (this.InitialPath == GetSpecialFolders(SpecialFolders.Desktop))
            {
                if (path.Contains(GetSpecialFolders(SpecialFolders.Desktop))) //Desktop->
                    depth -= GetNativeDepth(GetSpecialFolders(SpecialFolders.Desktop));
                else if (path.Contains(GetSpecialFolders(SpecialFolders.Documents))) //Desktop->Documents->
                {
                    depth -= GetNativeDepth(GetSpecialFolders(SpecialFolders.Documents));
                    if (path == GetSpecialFolders(SpecialFolders.Documents)) // + Desktop
                        depth += 1;
                    else // + Desktop + Documents
                        depth += 2;
                }
                else if (path.Length == 3) // all drives: C:\, D:\, etc..
                    depth = 3;
                else
                    depth -= 3; // Desktop->Computer->Drive->
            }
            else
            {
                depth -= GetNativeDepth(this.InitialPath);
                depth += 1; // + the Root of TreeView
            }
            return depth;
        }

        /// <summary>
        /// Get native depth.
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>the depth of the path in the filesystem</returns>
        private int GetNativeDepth(string path)
        {
            return path.Split('\\').Length;
        }

        /// <summary>
        /// Tree vire item mouse right button down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeViewItems_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)sender;
            tvi.IsSelected = true;
            e.Handled = true;
        }

        /// <summary>
        /// Do nothing event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void doNothing_Event(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Tree view selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvi_Selected(object sender, RoutedEventArgs e)
        {
            var tvi = e.OriginalSource as TreeViewItem;
            if (tvi.Items.Count <= 1 && !tvi.IsExpanded)
                tvi.IsExpanded = true;
            e.Handled = true;
        }

        /// <summary>
        /// Tree view expanded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvi_Expanded(object sender, RoutedEventArgs e)
        {
            var tvi = e.OriginalSource as TreeViewItem;
            if (tvi.Items.Count <= 1 && tvi.IsExpanded)
            {
                tvi.Items.Clear();
                string[] folders = Directory.GetDirectories(tvi.ToolTip.ToString());
                for (int i = 0; i < folders.Length; i++)
                {
                    try
                    {
                        if (this.checkBoxEmptyFolders.IsChecked != true)
                        {
                            if (!FolderIsEmpty(folders[i]))
                                tvi.Items.Add(CreateTreeVieItem(folders[i]));
                        }
                        else if (this.checkBoxHiddenFolders.IsChecked != true)
                        {
                            if (!FolderIsHidden(folders[i]))
                                tvi.Items.Add(CreateTreeVieItem(folders[i]));
                        }
                        else
                            tvi.Items.Add(CreateTreeVieItem(folders[i]));
                    }
                    catch (Exception) //If there are no rights for the folder, bad solution but best now
                    { }
                }
            }
            e.Handled = true;
        }

        /// <summary>
        /// Computer expanded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void computer_Expanded(object sender, RoutedEventArgs e)
        {
            var tvi = GetTreeViewItem((TreeViewItem)this.treeViewFolders.Items[0], "Computer");
            if (tvi.Items.Count <= 1 && tvi.IsExpanded)
            {
                tvi.Items.Clear();
                string[] folders = Directory.GetLogicalDrives();
                for (int i = 0; i < folders.Length; i++)
                {
                    try
                    {
                        if (this.checkBoxEmptyFolders.IsChecked != true)
                        {
                            if (!FolderIsEmpty(folders[i]))
                                tvi.Items.Add(CreateTreeVieItem(folders[i]));
                        }
                        else if (this.checkBoxHiddenFolders.IsChecked != true)
                        {
                            if (!FolderIsHidden(folders[i]))
                                tvi.Items.Add(CreateTreeVieItem(folders[i]));
                        }
                        else
                            tvi.Items.Add(CreateTreeVieItem(folders[i]));
                    }
                    catch (Exception) //The Drive is not ready, bad solution but best now
                    { }
                }
            }
            e.Handled = true;
        }

        /// <summary>
        /// Selected path is in initial path
        /// </summary>
        /// <returns>True; else false.</returns>
        private bool SelectedPathIsInInitialPath()
        {
            if (this.InitialPath != GetSpecialFolders(SpecialFolders.Desktop))
            {
                string[] initial = this.InitialPath.Split('\\');
                string[] selected = this.SelectedPath.Split('\\');
                for (int i = 0; i < initial.Length; i++)
                {
                    if (selected[i] != initial[i])
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Reload the folders.
        /// </summary>
        /// <param name="path">The current path.</param>
        /// <param name="newFolder">The new folder.</param>
        private void ReloadFolder(string path, string newFolder)
        {
            this.treeViewFolders.Items.Clear();
            if (path == this.InitialPath)
                LoadFolders();
            else
            {
                string[] selected = this.SelectedPath.Split('\\');
                TreeViewItem actual = (TreeViewItem)this.treeViewFolders.Items[0];
                int startindex = 0; //if desktop

                if (this.InitialPath != GetSpecialFolders(SpecialFolders.Desktop))
                {
                    string[] initial = this.InitialPath.Split('\\');
                    startindex = selected.Length - initial.Length;
                    startindex += 2; //to correct the substraction
                }
                else
                {
                    actual = GetTreeViewItem(actual, "Computer");
                }

                for (int i = startindex; i < selected.Length; i++)
                {
                    actual = GetTreeViewItem(actual, selected[i]);
                }

                actual.Items.Clear();
                actual.IsSelected = true;
                actual.IsExpanded = false;//for actiating the expanded-event
                actual.IsExpanded = true;
                this.btnNewFolder.IsEnabled = true;
            }
            if (this.SelectedPath != newFolder)
                this.SelectedPath = path + "\\" + newFolder;

            SelectPath();
        }

        /// <summary>
        /// Select the path.
        /// </summary>
        private void SelectPath()
        {
            if (this.SelectedPath != "" && this.SelectedPath != this.InitialPath)
            {
                string[] selected = this.SelectedPath.Split('\\');
                TreeViewItem actual = (TreeViewItem)this.treeViewFolders.Items[0];
                int startindex = 0; //if desktop

                if (this.InitialPath != GetSpecialFolders(SpecialFolders.Desktop))
                {
                    string[] initial = this.InitialPath.Split('\\');
                    startindex = selected.Length - initial.Length;
                    startindex += 2; //to correct the substraction
                }
                else
                {
                    actual = GetTreeViewItem(actual, "Computer");
                    actual.IsSelected = true;
                    actual.IsExpanded = true;
                }

                for (int i = startindex; i < selected.Length; i++)
                {
                    actual = GetTreeViewItem(actual, selected[i]);
                    actual.IsSelected = true;
                    actual.IsExpanded = true;
                }

                this.btnNewFolder.IsEnabled = true;
            }
            else if (this.SelectedPath == this.InitialPath)
            {
                TreeViewItem actual = (TreeViewItem)this.treeViewFolders.Items[0];
                actual.IsSelected = true;
                actual.IsExpanded = true;
            }
        }

        /// <summary>
        /// get tree view item.
        /// </summary>
        /// <param name="actual">The current tree view item.</param>
        /// <param name="subItemName">sub item name.</param>
        /// <returns>The new tree view item.</returns>
        private TreeViewItem GetTreeViewItem(TreeViewItem actual, string subItemName)
        {
            for (int i = 0; i < actual.Items.Count; i++)
            {
                if (((TreeViewItem)actual.Items[i]).Header.ToString() == subItemName)
                    return (TreeViewItem)actual.Items[i];
            }
            return null;
        }
    }
}
