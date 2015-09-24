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

namespace Nequeo.Wpf.UI.Custom
{
    /// <summary>
    /// Colour list box control.
    /// </summary>
    public partial class ColourListBoxControl : UserControl
    {
        /// <summary>
        /// Colour list box control.
        /// </summary>
        public ColourListBoxControl()
        {
            InitializeComponent();

            // Add the colour list box.
            PresetFontColorCombo(Brushes.AliceBlue);
        }

        /// <summary>
        /// Text colour changed.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="args"></param>
        private void TextColorChanged(object color, TextChangedEventArgs args)
        {
            ColorComboBoxItem item = TextBoxChanged(color as TextBox, TextColorListBox) as ColorComboBoxItem;
            if (item == null)
            {
                TextColorTextBox.Foreground = Brushes.Gray;
            }
            else
            {
                TextColorTextBox.Foreground = Brushes.Black;
            }
        }

        /// <summary>
        /// Text colour selected.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        private void TextColorSelected(object o, SelectionChangedEventArgs args)
        {
            if (TextColorListBox.SelectedItem != null)
            {
                TextColorTextBox.Text = TextColorListBox.SelectedItem.ToString();
            }
        }

        /// <summary>
        /// Text box changed.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <param name="listBox">The list box.</param>
        /// <returns></returns>
        private object TextBoxChanged(TextBox textBox, ListBox listBox)
        {
            // Scroll the listbox so that the nearest listbox entry less than or
            // equal to the text box entry is in view. If the textbox entry exactly
            // matches a listbox entry, select the entry.
            // Returns selected list item or null if none matching.


            // Find the nearest string in the listbox. Note that listboxes are not
            // necessarily in alphabetic order - for example the typeface listbox
            // is sorted by stretch, weight and style, not by alphabetic value of the
            // descriptive text.
            //
            // This code finds the (first) item exactly the same as the text box, or
            // if there is no equal value, it finds the item immediately in front
            // of the first item alphabetically greater than the textbox.

            int nearestItem = 0;
            bool foundNearest = false;

            for (int i = 0; i < listBox.Items.Count; i++)
            {
                int comparison = (listBox.Items[i] as IFontChooserComboBoxItem).CompareWithString(textBox.Text);

                if (comparison < 0)
                {
                    if (!foundNearest)
                    {
                        nearestItem = i;
                    }
                }
                else if (comparison == 0)
                {
                    nearestItem = i;
                    foundNearest = true;
                }
                else
                {
                    foundNearest = true;
                }
            }

            listBox.Items.MoveCurrentToPosition(nearestItem);
            // Doesn't change highlight: listBox.ScrollIntoView(listBox.Items[nearestItem]);

            if ((listBox.Items[nearestItem] as IFontChooserComboBoxItem).CompareWithString(textBox.Text) == 0)
            {
                // TextBox exactly matches an entry in the list box
                // Make sure it is selected (but don't fire a selection change if it is already selected)
                if (listBox.SelectedIndex != nearestItem)
                {
                    listBox.SelectedIndex = nearestItem;
                }
                return listBox.Items[nearestItem];  // This is the item we matched
            }
            else
            {
                // Text string does not match any entry in the list
                return null;
            }
        }

        /// <summary>
        /// Create the list of all known colours.
        /// </summary>
        /// <param name="foreground">The foreground colour.</param>
        private void PresetFontColorCombo(System.Windows.Media.Brush foreground)
        {
            int i;
            // Fill combobox with all known named colors

            for (i = 0; i < KnownColor.ColorNames.Length; i++)
            {
                TextColorListBox.Items.Add(new ColorComboBoxItem(
                    KnownColor.ColorNames[i],
                    (SolidColorBrush)KnownColor.ColorTable[KnownColor.ColorNames[i]]
                ));
            }


            // Look for and display incoming color

            string colorName = "Black";  // Will use black if incoming color is not a known named color

            if (foreground is SolidColorBrush)
            {
                Brush brush = foreground as SolidColorBrush;
                for (i = 0; i < TextColorListBox.Items.Count; i++)
                {
                    if ((TextColorListBox.Items[i] as ColorComboBoxItem).Brush == brush)
                    {
                        colorName = (TextColorListBox.Items[i] as ColorComboBoxItem).ToString();
                    }
                }
            }

            TextColorTextBox.Text = colorName;
        }
    }
}
