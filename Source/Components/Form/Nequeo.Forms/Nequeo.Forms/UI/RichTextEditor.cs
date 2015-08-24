using System;
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
            richTextBox1.Clear();
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
                dlg.Filter = "Rich Text Files (*.rtf)|*.rtf|Text Files (*.txt)|*.txt";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        richTextBox1.LoadFile(dlg.FileName);
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(x.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                dlg.Filter = "Rich Text Files (*.rtf)|*.rtf|Text Files (*.txt)|*.txt";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        richTextBox1.SaveFile(dlg.FileName);
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(x.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
