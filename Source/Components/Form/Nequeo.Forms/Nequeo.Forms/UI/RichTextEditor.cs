using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Nequeo.Drawing.Printing;

namespace Nequeo.Forms.UI
{
    /// <summary>
    /// Rich text editor.
    /// </summary>
    public partial class RichTextEditor : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public RichTextEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Print command implemented using RichTextBoxDocument class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void printToolStripButton_Click(object sender, EventArgs e)
        {
            // create document
            var doc = new RichTextBoxDocument(richTextBox1);

            // set document header and footer
            // use '[page]' to get the current page number in headers and footers
            // use '[pages]' to get the page count in headers and footers
            //doc.Header = string.Format("\t - RichTextBoxDocument - \r\n** {0} **", richTextBox1.Name);
            //doc.Footer = string.Format("{0}\t{1}\tPage [page] of [pages]", DateTime.Today.ToShortDateString(), DateTime.Now.ToShortTimeString());

            // preview the document
            using (var dlg = new PreviewDialog())
            {
                dlg.Document = doc;
                dlg.ShowDialog(this);
            }
        }

        /// <summary>
        /// Simple implementation of standard file commands (new, open, save)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void newToolStripButton_Click(object sender, EventArgs e)
        {
            // If text exists.
            if (richTextBox1.TextLength > 0)
            {
                // Get the result.
                DialogResult result = MessageBox.Show("Save the current document first.", 
                    "Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                // If no the clear the text.
                if (result == DialogResult.No)
                {
                    // Clear the text.
                    richTextBox1.Clear();
                }
            }
            else
            {
                // Clear the text.
                richTextBox1.Clear();
            }
        }

        /// <summary>
        /// Open file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void openToolStripButton_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.DefaultExt = ".rtf";
                dlg.Filter = "Rich Text Files (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|Html Files (*.htm *.html)|*.htm;*.html|All Files (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    FileStream file = null;
                    try
                    {
                        // If the file is rtf.
                        if (System.IO.Path.GetExtension(dlg.FileName).Equals(".rtf"))
                        {
                            richTextBox1.LoadFile(dlg.FileName, RichTextBoxStreamType.RichText);
                        }
                        else if(System.IO.Path.GetExtension(dlg.FileName).Equals(".txt"))
                        {
                            richTextBox1.LoadFile(dlg.FileName, RichTextBoxStreamType.PlainText);
                        }
                        else
                        {
                            // Load just the text.
                            // Open the file.
                            file = new FileStream(dlg.FileName, FileMode.Open,
                                 FileAccess.Read, FileShare.ReadWrite);

                            // Read all the data from the file.
                            byte[] buffer = new byte[file.Length];
                            int bytesRead = file.Read(buffer, 0, (int)file.Length);
                            string fileData = Encoding.Default.GetString(buffer);

                            // Set the the richtextbox text value.
                            richTextBox1.Text = fileData;
                        }
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(x.Message, "Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (file != null)
                            file.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Save file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void saveToolStripButton_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.DefaultExt = ".rtf";
                dlg.Filter = "Rich Text Files (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|Html Files (*.htm *.html)|*.htm;*.html|All Files (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
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
                            richTextBox1.SaveFile(dlg.FileName, RichTextBoxStreamType.RichText);
                        }
                        else if (System.IO.Path.GetExtension(dlg.FileName).Equals(".txt"))
                        {
                            richTextBox1.SaveFile(dlg.FileName, RichTextBoxStreamType.PlainText);
                        }
                        else
                        {
                            // Save the text only.
                            // Create a new file stream
                            // truncate the file.
                            file = new FileStream(dlg.FileName, FileMode.Truncate,
                                 FileAccess.Write, FileShare.ReadWrite);

                            // Get the bytes from the text.
                            byte[] buffer = Encoding.Default.GetBytes(richTextBox1.Text);
                            file.Write(buffer, 0, buffer.Length);
                        }
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(x.Message, "Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (file != null)
                            file.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Cut.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cutToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        /// <summary>
        /// Copy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void copyToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        /// <summary>
        /// Paste.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        /// <summary>
        /// Exit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Undo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonUndo_Click(object sender, EventArgs e)
        {
            richTextBox1.Undo();
        }

        /// <summary>
        /// Redo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonRedo_Click(object sender, EventArgs e)
        {
            richTextBox1.Redo();
        }

        /// <summary>
        /// Zoom In.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonZoomIn_Click(object sender, EventArgs e)
        {
            try
            {
                // If the zoom factor is not too big
                // then zoom in more.
                if (richTextBox1.ZoomFactor < 64)
                    richTextBox1.ZoomFactor = richTextBox1.ZoomFactor + 0.5f;
            }
            catch { }
        }

        /// <summary>
        /// Zoom out
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonZoomOut_Click(object sender, EventArgs e)
        {
            try
            {
                // If the zoom factor is not too small
                // then zoom out more.
                if (richTextBox1.ZoomFactor > (1 / 64))
                    richTextBox1.ZoomFactor = richTextBox1.ZoomFactor - 0.5f;
            }
            catch { }
        }

        /// <summary>
        /// Font dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonFont_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionFont != null)
                fontDialogMain.Font = richTextBox1.SelectionFont;
            else
                fontDialogMain.Font = richTextBox1.Font;

            if (fontDialogMain.ShowDialog() == DialogResult.OK)
            {
                if (richTextBox1.SelectionFont != null)
                    richTextBox1.SelectionFont = fontDialogMain.Font;
                else
                    richTextBox1.Font = fontDialogMain.Font;
            }
        }

        /// <summary>
        /// Colour dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonColor_Click(object sender, EventArgs e)
        {
            colorDialogMain.Color = richTextBox1.SelectionColor;

            if (colorDialogMain.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.SelectionColor = colorDialogMain.Color;
            }
        }

        /// <summary>
        /// Bold
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonBold_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionFont != null)
            {
                richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont, richTextBox1.SelectionFont.Style ^ FontStyle.Bold);
            }
        }

        /// <summary>
        /// Italic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonItalic_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionFont != null)
            {
                richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont, richTextBox1.SelectionFont.Style ^ FontStyle.Italic);
            }
        }

        /// <summary>
        /// Underline
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonUnderline_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionFont != null)
            {
                richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont, richTextBox1.SelectionFont.Style ^ FontStyle.Underline);
            }
        }

        /// <summary>
        /// Left
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonLeft_Click(object sender, EventArgs e)
        {
            //change horizontal alignment to left
            richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
        }

        /// <summary>
        /// Centre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonCentre_Click(object sender, EventArgs e)
        {
            //change horizontal alignment to center
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
        }

        /// <summary>
        /// Right
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonRight_Click(object sender, EventArgs e)
        {
            //change horizontal alignment to right
            richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
        }

        /// <summary>
        /// Strikeout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonStrikethrough_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionFont != null)
            {
                richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont, richTextBox1.SelectionFont.Style ^ FontStyle.Strikeout);
            }
        }

        /// <summary>
        /// Super
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonSuper_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Sub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonSub_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Bullet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonBullet_Click(object sender, EventArgs e)
        {
            if (!richTextBox1.SelectionBullet)
                richTextBox1.SelectionBullet = true;
            else
                richTextBox1.SelectionBullet = false;
        }

        /// <summary>
        /// Insert image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonInsertImage_Click(object sender, EventArgs e)
        {
            // Open the dialog.
            using (var dlg = new OpenFileDialog())
            {
                // Set image files
                dlg.Filter = "Image Files (*.bmp *.jpg *.png *.ico)|*.bmp;*.jpg;*.png;*.ico|All Files (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Load the image.
                        Bitmap myBitmap = new Bitmap(dlg.FileName);

                        // Copy the bitmap to the clipboard.
                        Clipboard.SetDataObject(myBitmap);

                        // Get the format for the object type.
                        DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Bitmap);

                        // After verifying that the data can be pasted, paste
                        if (richTextBox1.CanPaste(myFormat))
                        {
                            richTextBox1.Paste(myFormat);
                        }
                        else
                        {
                            MessageBox.Show("The data format that you attempted site" +
                              " is not supportedby this control.");
                        }
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(x.Message, "Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Find text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void findTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchBox searchBox = new SearchBox(richTextBox1);
            searchBox.OnSearchText = (string text, int indexToText) => OnSearchText(text, indexToText);
            searchBox.Show(this);
        }

        /// <summary>
        /// On serach text complete.
        /// </summary>
        /// <param name="text">The original text.</param>
        /// <param name="indexToText">The index to the text.</param>
        private void OnSearchText(string text, int indexToText)
        {
            // Determine whether the text was found. 
            if (indexToText >= 0 && !String.IsNullOrEmpty(text))
            {
                // No more text to find, the search cursor
                // is at the end of the text.
                if (indexToText >= richTextBox1.TextLength)
                {
                    // Searched to end.
                    MessageBox.Show("Searched to the end of the text.", "Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Select the text that was search.
                    richTextBox1.Select(indexToText, text.Length);
                    richTextBox1.Focus();
                }
            }
            else
            {
                // Text not found.
                MessageBox.Show("The text can not be found.", "Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
