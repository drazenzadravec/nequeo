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
using System.Printing;
using System.Reflection;

using Nequeo.Wpf.UI.Printing;

namespace Nequeo.Wpf.UI.Text
{
    /// <summary>
    /// Rich text box editor control.
    /// </summary>
    public partial class RichTextEditorControl : UserControl
    {
        /// <summary>
        /// Rich text box editor control.
        /// </summary>
        public RichTextEditorControl()
        {
            InitializeComponent();

            // Setup the printable object.
            _printDocument = new PrintDocument(mainRTB);
            _currentTextBox = mainRTB;
        }

        private FindDialog _findDlg = null;
        private PrintDocument _printDocument = null;
        System.Windows.Controls.Primitives.TextBoxBase _currentTextBox;

        private string _findString;
        private string _replaceString;
        private bool _findMatchCase = false;
        private bool _findUpward = false;
        private bool _firstReplace;

        /// <summary>
        /// Page setup custom routed command.
        /// </summary>
        public static readonly RoutedCommand PageSetupCommand = new RoutedCommand("PageSetup", typeof(RichTextEditorControl), null);

        /// <summary>
        /// Print async
        /// </summary>
        public static readonly RoutedCommand PrintAsyncCommand = new RoutedCommand("PrintAsync", typeof(RichTextEditorControl), null);

        /// <summary>
        /// Print custom routed command.
        /// </summary>
        public static readonly RoutedCommand PrintCommand = new RoutedCommand("Print", typeof(RichTextEditorControl), null);

        /// <summary>
        /// Preview custom routed command.
        /// </summary>
        public static readonly RoutedCommand PreviewCommand = new RoutedCommand("Preview", typeof(RichTextEditorControl), null);

        /// <summary>
        /// Find custom routed command.
        /// </summary>
        public static readonly RoutedCommand FindCommand = new RoutedCommand("Find", typeof(RichTextEditorControl), null);

        /// <summary>
        /// Find next custom routed command.
        /// </summary>
        public static readonly RoutedCommand FindNextCommand = new RoutedCommand("FindNext", typeof(RichTextEditorControl), null);

        /// <summary>
        /// Replace custom routed command.
        /// </summary>
        public static readonly RoutedCommand ReplaceCommand = new RoutedCommand("Replace", typeof(RichTextEditorControl), null);

        /// <summary>
        /// Spell checker command.
        /// </summary>
        public static readonly RoutedCommand SpellCheckCommand = new RoutedCommand("SpellCheck", typeof(RichTextEditorControl), null);

        /// <summary>
        /// Date time command.
        /// </summary>
        public static readonly RoutedCommand DateTimeCommand = new RoutedCommand("DateTime", typeof(RichTextEditorControl), null);

        /// <summary>
        /// Word wrap command.
        /// </summary>
        public static readonly RoutedCommand WordWrapCommand = new RoutedCommand("WordWrap", typeof(RichTextEditorControl), null);

        /// <summary>
        /// Bind to the Text dependancy.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
          DependencyProperty.Register("Text", typeof(string), typeof(RichTextEditorControl),
          new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the text property.
        /// </summary>
        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected text.
        /// </summary>
        private string SelectedText
        {
            get
            {
                return mainRTB.Selection.Text;
            }
            set
            {
                TextRange tr = new TextRange(mainRTB.Selection.Start, mainRTB.Selection.End);
                tr.Text = value;
            }
        }

        /// <summary>
        /// Gets the text content.
        /// </summary>
        private string TextContent
        {
            get
            {
                TextRange tr = new TextRange(mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);
                return tr.Text;
            }
        }

        /// <summary>
        /// Gets or sets the selection length.
        /// </summary>
        private int SelectionLength
        {
            get
            {
                return mainRTB.Selection.Text.Length;
            }
            set
            {
                TextPointer p = (mainRTB.Selection.Start);
                p = p.GetPositionAtOffset(value);
                mainRTB.Selection.Select(mainRTB.Selection.Start, p);
            }
        }

        /// <summary>
        /// Gets or sets the selection start.
        /// </summary>
        private int SelectionStart
        {
            get
            {
                return mainRTB.Document.ContentStart.GetOffsetToPosition(mainRTB.Selection.Start);
            }
            set
            {
                TextPointer p = (mainRTB.Document.ContentStart);
                p = p.GetPositionAtOffset(value);
                mainRTB.Selection.Select(p, p);
            }
        }

        /// <summary>
        /// New document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void New_Click(object sender, RoutedEventArgs e)
        {
            // If text exists.
            if (mainRTB.Document.Blocks.Count > 0)
            {
                // Get the result.
                MessageBoxResult result = MessageBox.Show("Save the current document first.",
                    "Editor", MessageBoxButton.YesNo, MessageBoxImage.Information);

                // If no then clear the text.
                if (result == MessageBoxResult.No)
                {
                    // Clear the text.
                    FlowDocument copy = new FlowDocument();
                    mainRTB.Document = copy;
                }
            }
        }

        /// <summary>
        /// Open document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".rtf";
            dlg.Filter = "Rich Text Files (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|Html Files (*.htm *.html)|*.htm;*.html|All Files (*.*)|*.*";

            if (dlg.ShowDialog() == true)
            {
                FileStream file = null;
                try
                {
                    // If the file is rtf.
                    if (System.IO.Path.GetExtension(dlg.FileName).Equals(".rtf"))
                    {
                        // Open the file.
                        file = File.Open(dlg.FileName, FileMode.Open);

                        // Load the file into the document.
                        FlowDocument copy = new FlowDocument();
                        TextRange dest = new TextRange(copy.ContentStart, copy.ContentEnd);
                        dest.Load(file, DataFormats.Rtf);
                        mainRTB.Document = copy;
                    }
                    else if (System.IO.Path.GetExtension(dlg.FileName).Equals(".txt"))
                    {
                        // Open the file.
                        file = File.Open(dlg.FileName, FileMode.Open);

                        // Load the file into the document.
                        FlowDocument copy = new FlowDocument();
                        TextRange dest = new TextRange(copy.ContentStart, copy.ContentEnd);
                        dest.Load(file, DataFormats.Text);
                        mainRTB.Document = copy;
                    }
                    else if ((System.IO.Path.GetExtension(dlg.FileName).Equals(".htm")) || (System.IO.Path.GetExtension(dlg.FileName).Equals(".html")))
                    {
                        // Open the file.
                        file = File.Open(dlg.FileName, FileMode.Open);

                        // Load the file into the document.
                        FlowDocument copy = new FlowDocument();
                        TextRange dest = new TextRange(copy.ContentStart, copy.ContentEnd);
                        dest.Load(file, DataFormats.Html);
                        mainRTB.Document = copy;
                    }
                    else
                    {
                        // Open the file.
                        file = File.Open(dlg.FileName, FileMode.Open);

                        // Load the file into the document.
                        FlowDocument copy = new FlowDocument();
                        TextRange dest = new TextRange(copy.ContentStart, copy.ContentEnd);
                        dest.Load(file, DataFormats.Text);
                        mainRTB.Document = copy;
                    }
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message, "Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
            }
        }

        /// <summary>
        /// Save document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".rtf";
            dlg.Filter = "Rich Text Files (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|Html Files (*.htm *.html)|*.htm;*.html|All Files (*.*)|*.*";

            if (dlg.ShowDialog() == true)
            {
                FileStream file = null;
                try
                {
                    // Create the file.
                    file = File.Create(dlg.FileName);
                    file.Close();

                    // If the file is rtf.
                    if (System.IO.Path.GetExtension(dlg.FileName).Equals(".rtf"))
                    {
                        // Save the text only.
                        // Create a new file stream
                        // truncate the file.
                        file = new FileStream(dlg.FileName, FileMode.Truncate,
                             FileAccess.Write, FileShare.ReadWrite);

                        // Save the data.
                        TextRange source = new TextRange(mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);
                        source.Save(file, DataFormats.Rtf);
                    }
                    else if (System.IO.Path.GetExtension(dlg.FileName).Equals(".txt"))
                    {
                        // Save the text only.
                        // Create a new file stream
                        // truncate the file.
                        file = new FileStream(dlg.FileName, FileMode.Truncate,
                             FileAccess.Write, FileShare.ReadWrite);

                        // Save the data.
                        TextRange source = new TextRange(mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);
                        source.Save(file, DataFormats.Text);
                    }
                    else if ((System.IO.Path.GetExtension(dlg.FileName).Equals(".htm")) || (System.IO.Path.GetExtension(dlg.FileName).Equals(".html")))
                    {
                        // Save the text only.
                        // Create a new file stream
                        // truncate the file.
                        file = new FileStream(dlg.FileName, FileMode.Truncate,
                             FileAccess.Write, FileShare.ReadWrite);

                        // Save the data.
                        TextRange source = new TextRange(mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);
                        source.Save(file, DataFormats.Html);
                    }
                    else
                    {
                        // Save the text only.
                        // Create a new file stream
                        // truncate the file.
                        file = new FileStream(dlg.FileName, FileMode.Truncate,
                             FileAccess.Write, FileShare.ReadWrite);

                        // Save the data.
                        TextRange source = new TextRange(mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);
                        source.Save(file, DataFormats.Text);
                    }
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message, "Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
            }
        }

        /// <summary>
        /// Print document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            // Get the document.
            FlowDocument flow = mainRTB.Document;
            double fontSize = flow.FontSize * 2;

            // Send the rich text data to the print preview.
            IDocumentPaginatorSource document = new Nequeo.Wpf.UI.Printing.RichTextBoxDocument(mainRTB, new Size(800, 1000), new Size(1, 1), fontSize).Document;
            Nequeo.Wpf.UI.Printing.PreviewDialog dialog = new Nequeo.Wpf.UI.Printing.PreviewDialog(document);
            Nequeo.Wpf.UI.Printing.Preview preview = new Nequeo.Wpf.UI.Printing.Preview(dialog);
            preview.ShowDialog();
        }

        /// <summary>
        /// Insert image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image Files (*.bmp *.jpg *.png *.ico)|*.bmp;*.jpg;*.png;*.ico|All Files (*.*)|*.*";

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    // Add the image to the current position.
                    Paragraph block = new Paragraph();
                    BitmapImage bitmap = new BitmapImage(new Uri(dlg.FileName));
                    Image image = new Image();
                    image.Source = bitmap;
                    image.Width = bitmap.Width;
                    image.Height = bitmap.Height;
                    block.Inlines.Add(image);

                    // Get the current position.
                    TextPointer position = mainRTB.Document.ContentStart.GetInsertionPosition(LogicalDirection.Forward);
                    Paragraph currentBlock = position.Paragraph;

                    // Insert the block with the image.
                    mainRTB.Document.Blocks.InsertAfter(currentBlock, block);
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message, "Editor", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Insert hyperlink.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertHyperlink_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected text.
            TextSelection text = mainRTB.Selection;
            if (!text.IsEmpty)
            {
                TextPointer p1 = text.Start;
                TextPointer p2 = text.End;
                TextRange tr = new TextRange(p1, p2);
                string URI = tr.Text;

                if (p1 != null && p2 != null)
                {
                    Hyperlink link = new Hyperlink(p1, p2);

                    link.IsEnabled = true;
                    link.NavigateUri = new Uri(URI);
                    link.RequestNavigate += Extension.RichTextBoxAssistant.OnUrlClickRequestNavigate;
                }
            }
        }

        /// <summary>
        /// Font colour document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FontColour_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            colorDialog.AllowFullOpen = true;

            // If ok.
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Media.Color col = new System.Windows.Media.Color();
                col.A = colorDialog.Color.A;
                col.B = colorDialog.Color.B;
                col.G = colorDialog.Color.G;
                col.R = colorDialog.Color.R;

                // Get the selected colour.
                System.Windows.Media.Brush brush = new SolidColorBrush(col);

                // Get the selected text.
                TextSelection text = mainRTB.Selection;
                if (!text.IsEmpty)
                {
                    // Apply the colour to the text.
                    text.ApplyPropertyValue(RichTextBox.ForegroundProperty, brush);
                }
            }
        }

        /// <summary>
        /// Font colour document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Font_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FontDialog fontDialog = new System.Windows.Forms.FontDialog();

            // If ok.
            if (fontDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Get the selected text.
                TextSelection text = mainRTB.Selection;
                if (!text.IsEmpty)
                {
                    // Font family converter.
                    FontFamilyConverter ffc = new FontFamilyConverter();

                    // Apply the properties to the text.
                    text.ApplyPropertyValue(RichTextBox.FontSizeProperty, (double)fontDialog.Font.Size);
                    text.ApplyPropertyValue(RichTextBox.FontFamilyProperty, (FontFamily)ffc.ConvertFromString(fontDialog.Font.Name));

                    // Apply the properties to the text.
                    if (fontDialog.Font.Bold)
                        text.ApplyPropertyValue(RichTextBox.FontWeightProperty, FontWeights.Bold);
                    else
                        text.ApplyPropertyValue(RichTextBox.FontWeightProperty, FontWeights.Normal);

                    // Apply the properties to the text.
                    if (fontDialog.Font.Italic)
                        text.ApplyPropertyValue(RichTextBox.FontStyleProperty, FontStyles.Italic);
                    else
                        text.ApplyPropertyValue(RichTextBox.FontStyleProperty, FontStyles.Normal);

                }
            }
        }

        /// <summary>
        /// Background colour document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundColour_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            colorDialog.AllowFullOpen = true;

            // If ok.
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Media.Color col = new System.Windows.Media.Color();
                col.A = colorDialog.Color.A;
                col.B = colorDialog.Color.B;
                col.G = colorDialog.Color.G;
                col.R = colorDialog.Color.R;

                // Get the selected colour.
                System.Windows.Media.Brush brush = new SolidColorBrush(col);

                //Current word at the pointer            
                // Get the selected text.
                TextSelection text = mainRTB.Selection;
                if (!text.IsEmpty)
                {
                    text.ApplyPropertyValue(TextElement.BackgroundProperty, brush);
                }
            }
        }

        /// <summary>
        /// Print the document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoPrintCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _printDocument.Print();
        }

        /// <summary>
        /// Print page setup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoPageSetupCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _printDocument.PageSetup();
        }

        /// <summary>
        /// Print async.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoPrintAsyncCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _printDocument.PrintAsync();
        }

        /// <summary>
        /// Preview the document.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoPreviewCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _printDocument.Preview();
        }

        /// <summary>
        /// Spell checker.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoSpellCheckCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _currentTextBox.SpellCheck.IsEnabled = !(_currentTextBox.SpellCheck.IsEnabled);
            _spellChecked.IsChecked = _currentTextBox.SpellCheck.IsEnabled;
        }

        /// <summary>
        /// Select all text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoSelectAllCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _currentTextBox.Focus();
            _currentTextBox.SelectAll();
        }

        /// <summary>
        /// Insert time and date.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoTimeDateCommand(object sender, ExecutedRoutedEventArgs e)
        {
            string date = DateTime.Now.ToString();
            using (_currentTextBox.DeclareChangeBlock())
            {
                SelectedText += date;
            }
            SelectionStart += date.Length;
            SelectionLength = 0;
        }

        /// <summary>
        /// Find text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoFindCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextContent))
                return;

            EnsureFind();
            _findDlg.Title = "Find";
            _findDlg.ShowReplace = false;
            _findDlg.Show();
        }

        /// <summary>
        /// Find next command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoFindNextCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_findString))
            {
                EnsureFind();
                _findDlg.ShowReplace = false;
                _findDlg.Show();
            }
            else
                Find(false);
        }

        /// <summary>
        /// Replace text command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoReplaceCommand(object sender, ExecutedRoutedEventArgs e)
        {
            EnsureFind();
            _findDlg.Title = "Replace";
            _findDlg.ShowReplace = true;
            _findDlg.Show();
            _findDlg.FindWhat = _findString;
            _findDlg.ReplaceWith = _replaceString;
        }

        /// <summary>
        /// Can execute find command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanExecuteFindCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Ensure find.
        /// </summary>
        private void EnsureFind()
        {
            if (_findDlg == null)
            {
                _findDlg = new FindDialog();

                _findDlg._findWhat.Focus();
                if (_findString != null)
                {
                    _findDlg._findWhat.SelectedText = _findString;
                    _findDlg.ReplaceWith = _replaceString;
                }

                _firstReplace = true;

                _findDlg.FindNext += delegate (object sender, EventArgs args)
                {
                    StoreFindDialogBoxValues();
                    Find(false);
                };

                _findDlg.Replace += delegate (object sender, EventArgs args)
                {
                    StoreFindDialogBoxValues();
                    String _currentFindSelection = SelectedText;
                    if (!_findMatchCase)
                    {
                        _currentFindSelection = _currentFindSelection.ToLower();
                        _findString = _findString.ToLower();
                    }
                    if ((!_firstReplace) || ((_currentFindSelection == _findString)))
                    {
                        using (_currentTextBox.DeclareChangeBlock())
                        {
                            SelectedText = _findDlg.ReplaceWith;
                        }
                    }
                    else
                    {
                        _firstReplace = false;
                    }
                    Find(false);
                    _findDlg.Focus();
                };

                _findDlg.ReplaceAll += delegate (object sender, EventArgs args)
                {
                    StoreFindDialogBoxValues();
                    SelectionStart = 0;
                    SelectionLength = 0;
                    while (Find(true))
                    {
                        using (_currentTextBox.DeclareChangeBlock())
                        {
                            SelectedText = _findDlg.ReplaceWith;
                            SelectionLength = _findDlg.ReplaceWith.Length;
                        }
                    }
                    SelectionStart = 0;
                    SelectionLength = 0;
                    _findDlg.Focus();
                };

                _findDlg.Closed += delegate (object sender, EventArgs args)
                {
                    _findDlg = null;
                };
            }
        }

        /// <summary>
        /// Store find dialog box values.
        /// </summary>
        private void StoreFindDialogBoxValues()
        {
            _findString = _findDlg.FindWhat;
            _replaceString = _findDlg.ReplaceWith;
        }

        /// <summary>
        /// Update find info.
        /// </summary>
        private void UpdateFindInfo()
        {
            _findString = _findDlg.FindWhat.ToString();
            if (_findDlg.MatchCase == true)
                _findMatchCase = true;
            else
                _findMatchCase = false;

            if ((bool)_findDlg.SearchUp)
                _findUpward = true;
            else
                _findUpward = false;
        }

        /// <summary>
        /// Find the text.
        /// </summary>
        /// <param name="ReplaceAll">Replace all text.</param>
        /// <returns>True all has been found; else false.</returns>
        private bool Find(bool ReplaceAll)
        {
            if (_findDlg != null)
                UpdateFindInfo();

            Assembly frameworkAssembly;     // current assembly (for reflection of Find method)
            Type findEngineType;            // Type we're looking for
            BindingFlags flags;             // params for reflection
            object[] args;                  // args for calling internal method
            TextRange searchContainer;      // RTB to search in
            string searchPattern;           // pattern to search for
            MethodInfo findMethod;          // located Find method


            System.Globalization.CultureInfo cultureInfo;
            flags = BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

            TextRange rangeFound;
            int findFlags = 0;

            // searching downward- from current caret position to end of RTB.
            // TODO Find upwards not implemented.
            searchContainer = new TextRange(mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);

            //text to be found
            searchPattern = _findString;

            //if case has to be matched
            findFlags = _findMatchCase ? 1 : 0;
            cultureInfo = System.Globalization.CultureInfo.CurrentCulture;
            args = new object[] { searchContainer.Start, searchContainer.End, searchPattern, findFlags, cultureInfo };
            frameworkAssembly = typeof(RichTextBox).Assembly;
            findEngineType = frameworkAssembly.GetType("System.Windows.Documents.TextFindEngine", true, false);
            findMethod = findEngineType.GetMethod("Find", flags);

            if (findMethod == null)
            {
                throw new ApplicationException(@"Find method on TextFindEngine type not found");
            }

            rangeFound = findMethod.Invoke(null, args) as TextRange;

            if (rangeFound == null)
            {
                if (!ReplaceAll)
                {
                    String message = String.Format("Cannot find {0}", _findString);
                    MessageBox.Show(message, "Editor", MessageBoxButton.OK, MessageBoxImage.Information);
                    _firstReplace = true;
                }

                return false;
            }
            else
            {
                mainRTB.Selection.Select(rangeFound.Start, rangeFound.End);
                return true;
            }
        }
    }
}
