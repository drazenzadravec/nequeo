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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading;

namespace Nequeo.Wpf.UI.Custom
{
    /// <summary>
    /// Editable label control.
    /// </summary>
    public class EditLabel : Control
    {
        /// <summary>
        /// Editable label control.
        /// </summary>
        static EditLabel()
        {
            CommandManager.RegisterClassCommandBinding(typeof(MenuItem), new CommandBinding(EditCommand, OnEditLabel));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditLabel), new FrameworkPropertyMetadata(typeof(EditLabel)));
        }

        /// <summary>
        /// Editable label control.
        /// </summary>
        public EditLabel()
            : base()
        {
        }

        /// <summary>
        /// Create edit command routed command.
        /// </summary>
        public static readonly RoutedCommand EditCommand = new RoutedCommand("Edit", typeof(EditLabel),
             new InputGestureCollection(new KeyGesture[] { new KeyGesture(Key.F2, ModifierKeys.None, "F2") }));


        /// <summary>
        /// This is a very special command execution that expects the sender to be a MenuItem that is a direct child of ContextMenu which again
        /// belongs to an EditLabel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnEditLabel(object sender, ExecutedRoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            if (item != null)
            {

                EditLabel label = (item.Parent as ContextMenu).PlacementTarget as EditLabel;
                if (label != null)
                {
                    label.EditMode = true;
                    label.SelectAll();
                }
            }
        }

        /// <summary>
        /// Select all text.
        /// </summary>
        public void SelectAll()
        {
            TextBox.Focus();
            TextBox.SelectAll();
        }

        /// <summary>
        /// The label text.
        /// </summary>
        public object Text
        {
            get { return (object)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Create the Text property dependency.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(object), typeof(EditLabel), new UIPropertyMetadata(null));

        /// <summary>
        /// The label edit mode.
        /// </summary>
        public bool EditMode
        {
            get { return (bool)GetValue(EditModeProperty); }
            set { SetValue(EditModeProperty, value); }
        }

        /// <summary>
        /// Create the Edit Mode property dependency.
        /// </summary>
        public static readonly DependencyProperty EditModeProperty =
            DependencyProperty.Register("EditMode", typeof(bool), typeof(EditLabel), new FrameworkPropertyMetadata(false, OnEditModePropertyChanged));


        /// <summary>
        /// On edit mode property changed event handler.
        /// </summary>
        /// <param name="d">The property dependency.</param>
        /// <param name="e">The property dependency event arguments.</param>
        private static void OnEditModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            if (newValue) (d as EditLabel).FocusTextBox(); else (d as EditLabel).UnfocusTextBox();
        }

        /// <summary>
        /// Unfocus text box.
        /// </summary>
        private void UnfocusTextBox()
        {
        }

        /// <summary>
        /// Focus text box.
        /// </summary>
        private void FocusTextBox()
        {
            TextBox.Focus();
            TextBox.SelectAll();
            TextBox.Focus();
        }

        /// <summary>
        /// On apply template handler.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // Attach to the lost focus event in the textbox.
            TextBox.LostFocus += new RoutedEventHandler(TextBox_LostFocus);
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Text box lost focus.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The routed event arguments.</param>
        void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            EditMode = false;
        }

        /// <summary>
        /// On lost keyboard focus.
        /// </summary>
        /// <param name="e">The keyboard focus changed event arguments.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            EditMode = false;
            base.OnLostKeyboardFocus(e);
        }

        /// <summary>
        /// On lost focus.
        /// </summary>
        /// <param name="e">The routed event arguments.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            EditMode = false;
        }

        /// <summary>
        /// Gets the text box template part.
        /// </summary>
        private TextBox TextBox
        {
            get
            {
                return GetTemplateChild("PART_TextBox") as TextBox;
            }
        }

        /// <summary>
        /// On mouse double click event.
        /// </summary>
        /// <param name="e">The mouse button event arguments.</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            EditMode = true;
            e.Handled = true;
            base.OnMouseDoubleClick(e);
            TextBox.Focus();
            TextBox.SelectAll();

        }

        /// <summary>
        /// On mouse down event.
        /// </summary>
        /// <param name="e">The mouse button event arguments.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            //  if (EditMode)
            {
                base.OnMouseDown(e);
            }
        }

        /// <summary>
        /// On key down event.
        /// </summary>
        /// <param name="e">Key event arguments.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F2:
                    if (!EditMode)
                    {
                        e.Handled = true;
                        EditMode = true;
                        //    FocusTextBox();
                    }
                    break;

                case Key.Return:
                    EditMode = false;
                    e.Handled = true;
                    break;

                case Key.Escape:
                    TextBox.Undo();
                    EditMode = false;
                    e.Handled = true;
                    break;
            }
            base.OnKeyDown(e);
        }

    }
}
