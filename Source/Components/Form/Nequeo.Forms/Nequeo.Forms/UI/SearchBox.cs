using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nequeo.Forms.UI
{
    /// <summary>
    /// Simple search box.
    /// </summary>
    public partial class SearchBox : Form
    {
        /// <summary>
        /// Simple search box.
        /// </summary>
        /// <param name="textBox">The text box to search in.</param>
        public SearchBox(TextBox textBox)
        {
            InitializeComponent();

            _textBox = textBox;
        }

        /// <summary>
        /// Simple search box.
        /// </summary>
        /// <param name="richTextBox">The rich text box to search in.</param>
        /// <param name="finds">Specifies how a text search is carried out in a System.Windows.Forms.RichTextBox control.</param>
        public SearchBox(RichTextBox richTextBox, RichTextBoxFinds finds = RichTextBoxFinds.None)
        {
            InitializeComponent();

            _richTextBox = richTextBox;
            _finds = finds;
        }

        private Action<string, int> _onSearchText = null;

        private TextBox _textBox = null;
        private RichTextBox _richTextBox = null;
        private RichTextBoxFinds _finds = RichTextBoxFinds.None;

        private int _start = 0;
        private int _indexToText = -1;

        /// <summary>
        /// Gets or sets the action handler when searching for text.
        /// This is triggered each time the search text button is pressed.
        /// The passed data is the original text and the index of the text.
        /// </summary>
        public Action<string, int> OnSearchText
        {
            get { return _onSearchText; }
            set { _onSearchText = value; }
        }

        /// <summary>
        /// Text search changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxSearchText_TextChanged(object sender, EventArgs e)
        {
            _start = 0;
            _indexToText = -1;
        }

        /// <summary>
        /// Search text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSearchText_Click(object sender, EventArgs e)
        {
            if (_richTextBox != null)
            {
                // If text exists.
                if (!String.IsNullOrEmpty(textBoxSearchText.Text))
                {
                    // If at the end of the text.
                    if (_start >= _richTextBox.TextLength)
                    {
                        // Set to the end of the text.
                        _indexToText = _richTextBox.TextLength;
                    }
                    else
                    {
                        // Find the text.
                        _indexToText = _richTextBox.Find(textBoxSearchText.Text, _start, _finds);

                        // Determine whether the text was found. 
                        if (_indexToText >= 0)
                        {
                            // New start point if found.
                            _start = _indexToText + textBoxSearchText.TextLength;
                        }
                    }
                }

                // Send the information to the client.
                _onSearchText?.Invoke(textBoxSearchText.Text, _indexToText);
            }

            if (_textBox != null)
            {
                // If text exists.
                if (!String.IsNullOrEmpty(textBoxSearchText.Text))
                {
                    // If at the end of the text.
                    if (_start >= _textBox.TextLength)
                    {
                        // Set to the end of the text.
                        _indexToText = _textBox.TextLength;
                    }
                    else
                    {
                        // If text exists.
                        if (!String.IsNullOrEmpty(_textBox.Text))
                        {
                            // For each char in text.
                            for (int i = _start; i < _textBox.TextLength; i++)
                            {
                                // Find the text.
                                IEnumerable<char> searchResults = _textBox.Text.Skip(_start).Take(textBoxSearchText.TextLength);
                                if (searchResults.Count() > 0)
                                {
                                    // Is match.
                                    string result = new string(searchResults.ToArray());
                                    if (result.ToLower() == textBoxSearchText.Text.ToLower())
                                    {
                                        // Set the starting index.
                                        _indexToText = _start;

                                        // Determine whether the text was found. 
                                        if (_indexToText >= 0)
                                        {
                                            // New start point if found.
                                            _start = _indexToText + textBoxSearchText.TextLength;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                // Send the information to the client.
                _onSearchText?.Invoke(textBoxSearchText.Text, _indexToText);
            }
        }
    }
}
