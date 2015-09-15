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
        }

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
    }
}
