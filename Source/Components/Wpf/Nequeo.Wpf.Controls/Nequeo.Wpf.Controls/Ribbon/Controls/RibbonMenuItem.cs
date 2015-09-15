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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;
using Nequeo.Wpf.Controls.Ribbon.Interfaces;
using Nequeo.Wpf.Controls.Interfaces;

namespace Nequeo.Wpf.Controls
{
   [ContentProperty("Items")]
    public class RibbonMenuItem:MenuItem,IKeyTipControl
    {
       const string partPopup = "PART_Popup";

        static RibbonMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonMenuItem), new FrameworkPropertyMetadata(typeof(RibbonMenuItem)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonMenuItem();
        }



        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(RibbonMenuItem), new UIPropertyMetadata(null));


        private Popup popup;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (popup != null)
            {
                popup.Opened -= OnPopupOpenend;
                popup.Closed -= OnPopupClosed;
            }
            popup = GetTemplateChild(partPopup) as Popup;
            if (popup != null)
            {
                popup.Opened += new EventHandler(OnPopupOpenend);
                popup.Closed += new EventHandler(OnPopupClosed);
            }
        }

        protected virtual void OnPopupClosed(object sender, EventArgs e)
        {
            IsSubmenuOpen = false;
        }

        protected virtual void OnPopupOpenend(object sender, EventArgs e)
        {
            RibbonApplicationMenu menu = ItemsControl.ItemsControlFromItemContainer(this) as RibbonApplicationMenu;
            if (menu != null)
            {
                Rect subMenuRect = menu.GetSubMenuRect(this);
                popup.Placement = PlacementMode.Relative;
                popup.VerticalOffset = subMenuRect.Top;
                popup.HorizontalOffset = subMenuRect.Left;
                popup.Width = subMenuRect.Width;
                popup.Height = subMenuRect.Height;
            }
        }



        protected override void OnSubmenuOpened(RoutedEventArgs e)
        {
            base.OnSubmenuOpened(e);
            if (popup != null) popup.IsOpen = true;
        }

        protected override void OnSubmenuClosed(RoutedEventArgs e)
        {
            base.OnSubmenuClosed(e);
            if (popup != null) popup.IsOpen = false;
        }
      


        #region IKeyboardCommand Members

        public void ExecuteKeyTip()
        {
            if (this.HasItems)
            {
                IsSubmenuOpen ^= true;
            }
            else
            {
                OnClick();
            }
        }

        #endregion
    }
}
