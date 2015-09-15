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
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Nequeo.Wpf.Controls
{
    [TemplatePart(Name = partDropDown)]
    [ContentProperty("Items")]
    public class RibbonSplitButton : RibbonDropDownButton
    {
        const string partDropDown = "PART_DropDown";

        static RibbonSplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonSplitButton), new FrameworkPropertyMetadata(typeof(RibbonSplitButton)));
        }


        protected  Control DropDownButton {get;private set;}


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (DropDownButton != null)
            {
                DropDownButton.MouseDown -= OnDropDownButtonDown;
                DropDownButton.MouseUp -= OnDropDownButtonUp;
            }
            DropDownButton = GetTemplateChild(partDropDown) as Control;
            if (DropDownButton != null)
            {
                DropDownButton.MouseLeftButtonDown += new MouseButtonEventHandler(OnDropDownButtonDown);
                DropDownButton.MouseLeftButtonUp += new MouseButtonEventHandler(OnDropDownButtonUp);
            }
        }

        protected virtual void OnDropDownButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            IsDropDownPressed ^= true;
            EnsurePopupRemainsOnMouseUp();
        }

        protected virtual void OnDropDownButtonUp(object sender, MouseButtonEventArgs e)
        {
            EnsurePopupDoesNotStayOpen();
        }




        public ClickMode ClickMode
        {
            get { return (ClickMode)GetValue(ClickModeProperty); }
            set { SetValue(ClickModeProperty, value); }
        }

        public static readonly DependencyProperty ClickModeProperty =
            DependencyProperty.Register("ClickMode", typeof(ClickMode), typeof(RibbonSplitButton), new UIPropertyMetadata(ClickMode.Release));


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            IsPressed = true;
            EnsurePopupRemainsOnMouseUp();
            if (ClickMode == ClickMode.Press) PerformClick();
        }

        protected override void HandleMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //base.HandleMouseLeftButtonDown(e);
        }

        protected override void HandleMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            //base.HandleMouseLeftButtonUp(e);
        }


        private void PerformClick()
        {
            OnClick();
        }

        protected override void OnClick()
        {
            if ((Command != null) && Command.CanExecute(CommandParameter)) Command.Execute(CommandParameter);
            base.OnClick();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            bool wasPressed = IsPressed;
            IsPressed = false;
            base.OnMouseLeftButtonUp(e);
            EnsurePopupDoesNotStayOpen();
            if (wasPressed && ClickMode == ClickMode.Release) PerformClick();

        }

        protected override void ToggleDropDownState()
        {
            // do not show the popup menu at this place.
        }

    }
}
