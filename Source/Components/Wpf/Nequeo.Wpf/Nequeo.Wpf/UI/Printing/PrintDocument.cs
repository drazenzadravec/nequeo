/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.IO.Packaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps;
using System.Windows.Xps.Serialization;
using System.Windows.Xps.Packaging;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Printing;
using System.Windows.Documents.Serialization;

using PageSetupDialog = System.Windows.Forms.PageSetupDialog;
using PageSetupDialogResult = System.Windows.Forms.DialogResult;

namespace Nequeo.Wpf.UI.Printing
{
    /// <summary>
    /// Print document.
    /// </summary>
    public class PrintDocument
    {
        /// <summary>
        /// Print document.
        /// </summary>
        /// <param name="richTextBox">The rich text box containing the document to print.</param>
        /// <param name="documentName">The unique document name used for printing.</param>
        public PrintDocument(RichTextBox richTextBox, string documentName = "")
        {
            _printCase = 0;
            _richTextBox = richTextBox;
            _documentName = documentName;

            _pageOrientation = null;
            _pageSize = null;
        }

        /// <summary>
        /// Print text document.
        /// </summary>
        /// <param name="textContent">The text to print.</param>
        /// <param name="font">The font to use when printing stream data.</param>
        /// <param name="fontSize">The font size to use when printing stream data.</param>
        /// <param name="brush">The font colour when printing stream data.</param>
        /// <param name="documentName">The unique document name used for printing.</param>
        public PrintDocument(string textContent, Typeface font, double fontSize, Brush brush, string documentName = "")
        {
            _printCase = 1;
            _textContent = textContent;
            _documentName = documentName;

            // Set the font.
            _font = font;
            _brush = brush;
            _fontSize = fontSize;

            _pageOrientation = null;
            _pageSize = null;
        }

        private int _printCase = 0;
        private string _textContent = "";
        private RichTextBox _richTextBox = null;
        private string _documentName = "";

        // Flag which indicates whether synchronous
        // or asynchronous printing was invoked.
        private bool _asyncPrintFlag = false;
        private System.Windows.Controls.PrintDialog _printDlg = null;

        private PageOrientation? _pageOrientation;
        private PageMediaSizeName? _pageSize;

        // Printing related
        double _leftMargin, _rightMargin, _topMargin, _bottomMargin;            //Margins saved in units of 1000th of an inch.
        PrintQueue _currentPrinter = null;
        PrintTicket _currentPrinterSettings = null;

        private double _fontSize;
        private Typeface _font = null;
        private Brush _brush = Brushes.Black;

        /// <summary>
        /// Gets or sets the flow direction.
        /// </summary>
        private FlowDirection FlowDirectionSettings
        {
            get
            {
                return FlowDirection.LeftToRight;
            }
        }

        /// <summary>
        /// Show print document dialog.
        /// </summary>
        public void Print()
        {
            SetUpPrintDialog();

            bool? dialogResult = _printDlg.ShowDialog();

            if (dialogResult == true)
            {
                PrintDoc();
                SavePrintInformation();
            }
        }

        /// <summary>
        /// Show page setup dialog.
        /// </summary>
        public void PageSetup()
        {
            PageSetupDialog pageSetupDlg = SetUpPageSetupDialog();
            PageSetupDialogResult dialogResult = pageSetupDlg.ShowDialog();

            if (dialogResult == PageSetupDialogResult.OK)
            {
                SavePageSetupInformation(pageSetupDlg);
            }
        }

        /// <summary>
        /// Show print document dialog and print async.
        /// </summary>
        public void PrintAsync()
        {
            _asyncPrintFlag = true;
            Print();
        }

        /// <summary>
        /// Show the preview dialog.
        /// </summary>
        public void Preview()
        {
            IDocumentPaginatorSource document = null;
            Nequeo.Wpf.UI.Printing.PreviewDialog dialog = null;
            Nequeo.Wpf.UI.Printing.Preview preview = null;

            // Select the print case.
            switch (_printCase)
            {
                case 0:
                    // Get the document.
                    FlowDocument flow = _richTextBox.Document;
                    double fontSize = flow.FontSize * 2;

                    // Send the rich text data to the print preview.
                    document = new Nequeo.Wpf.UI.Printing.RichTextBoxDocument(_richTextBox, new Size(800, 1000), new Size(1, 1), fontSize).Document;
                    dialog = new Nequeo.Wpf.UI.Printing.PreviewDialog(document);
                    preview = new Nequeo.Wpf.UI.Printing.Preview(dialog);
                    preview.ShowDialog();
                    break;

                case 1:
                    MemoryStream reader = null;
                    try
                    {
                        // Load the data into a stream.
                        byte[] buffer = Encoding.Default.GetBytes(_textContent);
                        reader = new MemoryStream(buffer);

                        // Send the text data to the print preview.
                        document = new Nequeo.Wpf.UI.Printing.PrintableDocument(reader, new Size(800, 1000), new Size(1, 1), _font, _fontSize, _brush).Document;
                        dialog = new Nequeo.Wpf.UI.Printing.PreviewDialog(document);
                        preview = new Nequeo.Wpf.UI.Printing.Preview(dialog);
                        preview.ShowDialog();
                    }
                    catch { }
                    finally
                    {
                        if (reader != null)
                            reader.Dispose();
                    }
                    break;
            }
        }

        /// <summary>
        /// Set up print dialog.
        /// </summary>
        private void SetUpPrintDialog()
        {
            _printDlg = new PrintDialog();
            LoadCurrentPrinterChoice(_printDlg);
        }

        /// <summary>
        /// Load current printer choice.
        /// </summary>
        /// <param name="dlg">Print dialog.</param>
        private void LoadCurrentPrinterChoice(PrintDialog dlg)
        {
            //load up printer info
            if (_currentPrinter != null)
            {
                dlg.PrintQueue = _currentPrinter;
            }
            if (_currentPrinterSettings != null)
            {
                dlg.PrintTicket = _currentPrinterSettings;
            }
        }

        /// <summary>
        /// Save print information.
        /// </summary>
        private void SavePrintInformation()
        {
            SaveCurrentPrinterChoice(_printDlg);
        }

        /// <summary>
        /// Save the current print choices
        /// </summary>
        /// <param name="dlg">Print dialog.</param>
        private void SaveCurrentPrinterChoice(PrintDialog dlg)
        {
            if (dlg.PrintQueue != null)
            {
                _currentPrinter = dlg.PrintQueue;
            }
            if (dlg.PrintTicket != null)
            {
                _currentPrinterSettings = dlg.PrintTicket;
            }
        }

        /// <summary>
        /// Save page setup information.
        /// </summary>
        /// <param name="pageSetupDlg">Page setup dialog.</param>
        private void SavePageSetupInformation(PageSetupDialog pageSetupDlg)
        {
            // Save any preferences that might have been customized:
            // Current printer, page size, orientation, margins
            //    SaveCurrentPrinterChoice(pageSetupDlg);

            // Win32 PageSetupDialog uses hundredths of inches. 
            _leftMargin = pageSetupDlg.PageSettings.Margins.Left;// / 25400;
            _rightMargin = pageSetupDlg.PageSettings.Margins.Right;// / 25400;
            _topMargin = pageSetupDlg.PageSettings.Margins.Top;// / 25400;
            _bottomMargin = pageSetupDlg.PageSettings.Margins.Bottom;// / 25400;

            PrintTicket printTicket = _currentPrinterSettings;
            if (pageSetupDlg.PageSettings.Landscape)
            {
                _pageOrientation = printTicket.PageOrientation = PageOrientation.Landscape;
            }
            else
            {
                _pageOrientation = printTicket.PageOrientation = PageOrientation.Portrait;
            }

            if (printTicket.PageMediaSize != null)
            {
                _pageSize = printTicket.PageMediaSize.PageMediaSizeName;
            }
        }

        /// <summary>
        /// Print the document.
        /// </summary>
        private void PrintDoc()
        {
            PrintTicket printTicket = _printDlg.PrintTicket;
            FlowDocument flowDocForPrinting = GetFlowDocumentForPrinting(printTicket);

            // In case the user has changed some settings (e.g., orientation or page size)
            // by going to the printer preferences dialog from the print dialog itself,
            // set the printer queue's User ticket to the one attached to the print dialog.
            _printDlg.PrintQueue.UserPrintTicket = printTicket;

            // Print the FlowDocument

            _printDlg.PrintQueue.CurrentJobSettings.Description = (String.IsNullOrEmpty(_documentName) ? Guid.NewGuid().ToString() : _documentName);
            XpsDocumentWriter docWriter = PrintQueue.CreateXpsDocumentWriter(_printDlg.PrintQueue);

            // Use our IDocumentPaginator implementation so we can insert headers and footers,
            // if present.
            // PrintableAreaHeight and Width are passed to the paginator to establish the 
            // true printable area for the document.
            HeaderFooterDocumentPaginator paginator = new HeaderFooterDocumentPaginator(
                ((IDocumentPaginatorSource)flowDocForPrinting).DocumentPaginator,
                null,
                null,
                _printDlg.PrintableAreaHeight,
                _printDlg.PrintableAreaWidth);

            if (_asyncPrintFlag == false)
            {
                try
                {
                    docWriter.Write(paginator, printTicket);
                }
                catch (PrintingCanceledException) { }
            }
            else
            {
                // Changes for Async printing start here:
                Application.Current.MainWindow.Opacity = 0.7;
                PrintProgressWindow dlg = new PrintProgressWindow(docWriter);
                dlg.PageNumber = 0;

                docWriter.WritingProgressChanged += new WritingProgressChangedEventHandler(dlg.OnWritingProgressChanged);
                docWriter.WritingCompleted += new WritingCompletedEventHandler(dlg.OnWritingCompleted);
                docWriter.WriteAsync(paginator, printTicket);
                dlg.ShowDialog();

                // Reset the flag here for next printing invocation.
                _asyncPrintFlag = false;
            }
        }

        /// <summary>
        /// Setup page print dialog.
        /// </summary>
        /// <returns>The page setup dialog with settings.</returns>
        private PageSetupDialog SetUpPageSetupDialog()
        {
            PageSetupDialog pageSetupDlg = new PageSetupDialog();
            pageSetupDlg.PageSettings = new System.Drawing.Printing.PageSettings();
            pageSetupDlg.PrinterSettings = new System.Drawing.Printing.PrinterSettings();

            //LoadCurrentPrinterChoice(pageSetupDlg);
            //load up margin info
            // Win32 PageSetupDialog uses hundredths of inches. 
            pageSetupDlg.PageSettings.Margins.Left = (int)_leftMargin;
            pageSetupDlg.PageSettings.Margins.Right = (int)_rightMargin;
            pageSetupDlg.PageSettings.Margins.Top = (int)_topMargin;
            pageSetupDlg.PageSettings.Margins.Bottom = (int)_bottomMargin;

            PrintTicket printTicket;

            if (_currentPrinterSettings != null)
            {
                printTicket = _currentPrinterSettings;
            }
            else
            {
                printTicket = new PrintTicket();
            }

            // Load up orientation info
            if (_pageOrientation != null)
            {
                printTicket.PageOrientation = (PageOrientation)_pageOrientation;

                switch (_pageOrientation)
                {
                    case PageOrientation.Landscape:
                        pageSetupDlg.PageSettings.Landscape = true;
                        break;
                    case PageOrientation.Portrait:
                        pageSetupDlg.PageSettings.Landscape = false;
                        break;
                    // Any other settings, the Win32 PageSetupDialog cannot handle so, we default to portrait
                    default:
                        pageSetupDlg.PageSettings.Landscape = false;
                        break;
                }
            }

            // Load up page size info
            if (_pageSize != null)
            {
                printTicket.PageMediaSize = new PageMediaSize((PageMediaSizeName)_pageSize);
            }
            _currentPrinterSettings = printTicket;

            return pageSetupDlg;
        }

        /// <summary>
        /// Get the flow document from the rich text box for printing.
        /// </summary>
        /// <param name="printTicket">The current print ticket.</param>
        /// <returns>The flow document.</returns>
        private FlowDocument GetFlowDocumentForPrinting(PrintTicket printTicket)
        {
            //Create a FlowDocument that will contain text to be printed.
            FlowDocument flowDoc = new FlowDocument();
            TextRange flowDocTextRange = new TextRange(flowDoc.ContentStart, flowDoc.ContentEnd);
            TextRange rtbTextRange;

            //Using RichTextBox to generate text in FlowDocument

            //case 1: Printing contents of Rich Text Box.
            if (_printCase == 0)
            {
                // Using current Rich textbox to generate text for Flow Document.
                rtbTextRange = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
            }
            //Printing contents of a plain text box
            else
            {
                //create a new Rich Text Box and transfer printable text from current Textbox to it.
                RichTextBox printRichTextBox = new RichTextBox();
                printRichTextBox.FontFamily = _font.FontFamily;
                printRichTextBox.FontSize = _fontSize;
                printRichTextBox.FontWeight = _font.Weight;
                printRichTextBox.Foreground = _brush;

                // Create a new paragraph block containing the text from the TextBox and add it to
                // the RichTextBox's document.
                Paragraph newPara = new Paragraph();
                newPara.Inlines.Clear();
                newPara.Inlines.Add(new Run(_textContent));
                if (printRichTextBox.Document.Blocks.FirstBlock != null)
                {
                    printRichTextBox.Document.Blocks.InsertBefore(printRichTextBox.Document.Blocks.FirstBlock, newPara);
                }
                else
                {
                    printRichTextBox.Document.Blocks.Add(newPara);
                }


                rtbTextRange = new TextRange(printRichTextBox.Document.ContentStart, printRichTextBox.Document.ContentEnd);
            }

            //transfering contents of RichtextBox textrange to flowDocTextRange in XML format
            //flowDoc now contains text to be printed

            string _rangeXML = TextRange_GetXml(rtbTextRange);
            TextRange_SetXml(flowDocTextRange, _rangeXML);

            ////Set margins of the Flow Document to values set in Page SetUp dialog.
            ////convert inches to logical pixels
            // Win32 PageSetupDialog uses hundredths of inches.
            // Divide by 100 to start with inches.
            LengthConverter converter = new LengthConverter();
            double leftInPixels = (double)converter.ConvertFromInvariantString((_leftMargin / 100).ToString() + " in");
            double rightInPixels = (double)converter.ConvertFromInvariantString((_rightMargin / 100).ToString() + " in");
            double topInPixels = (double)converter.ConvertFromInvariantString((_topMargin / 100).ToString() + " in");
            double bottomInPixels = (double)converter.ConvertFromInvariantString((_bottomMargin / 100).ToString() + " in");

            System.Windows.Thickness pagethickness = new Thickness(leftInPixels, topInPixels, rightInPixels, bottomInPixels);
            flowDoc.PagePadding = pagethickness;

            double maxColumnWidth;

            if (printTicket.PageMediaSize != null &&
                printTicket.PageMediaSize.Width != null)
            {
                maxColumnWidth = printTicket.PageMediaSize.Width.Value - leftInPixels - rightInPixels;
            }
            else
            {
                // fallback to Letter size if PrintTicket doesn't specify the media width
                maxColumnWidth = 816 - leftInPixels - rightInPixels;
            }

            flowDoc.ColumnWidth = maxColumnWidth;       //ensures we get only one column
            flowDoc.FlowDirection = FlowDirectionSettings;
            return flowDoc;
        }

        /// <summary>
        /// Get xaml from TextRange.Xml property
        /// </summary>
        /// <param name="range">TextRange</param>
        /// <returns>return a string serialized from the TextRange</returns>
        private string TextRange_GetXml(TextRange range)
        {
            MemoryStream mstream;

            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            mstream = new MemoryStream();
            range.Save(mstream, DataFormats.Xaml);

            //must move the stream pointer to the beginning since range.save() will move it to the end.
            mstream.Seek(0, SeekOrigin.Begin);

            //Create a stream reader to read the xaml.
            StreamReader stringReader = new StreamReader(mstream);
            return stringReader.ReadToEnd();
        }

        /// <summary>
        /// Set xml to TextRange.Xml property.
        /// </summary>
        /// <param name="range">TextRange</param>
        /// <param name="xml">Xaml to be set</param>        
        private void TextRange_SetXml(TextRange range, string xml)
        {
            MemoryStream mstream;
            if (null == xml)
            {
                throw new ArgumentNullException("xml");
            }
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            mstream = new MemoryStream();
            StreamWriter sWriter = new StreamWriter(mstream);

            mstream.Seek(0, SeekOrigin.Begin); //this line may not be needed.
            sWriter.Write(xml);
            sWriter.Flush();

            //move the stream pointer to the beginning.
            mstream.Seek(0, SeekOrigin.Begin);
            range.Load(mstream, DataFormats.Xaml);
        }
    }
}
