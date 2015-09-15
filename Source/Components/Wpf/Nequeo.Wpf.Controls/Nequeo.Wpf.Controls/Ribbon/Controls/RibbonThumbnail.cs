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
using System.Windows.Media;
using System.Windows;

namespace Nequeo.Wpf.Controls
{
    /// <summary>
    /// Represents an item container for a RibbonGallery.
    /// </summary>
    public class RibbonThumbnail:ListBoxItem
    {
        static RibbonThumbnail()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonThumbnail), new FrameworkPropertyMetadata(typeof(RibbonThumbnail)));
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(RibbonThumbnail), new UIPropertyMetadata(null));


        public RibbonThumbnail Original { get; internal set; }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            Select();
            e.Handled = true;
        }

        private void Select()
        {
            RibbonThumbnail original = this.Original != null ? Original : this;
            if (Gallery != null) Gallery.SelectedItem = original;
        }

        private RibbonGallery Gallery
        {
            get {
                RibbonGallery gallery = Parent as RibbonGallery;
                if (gallery == null)
                {
                    FrameworkElement e = Parent as FrameworkElement;
                    gallery = e != null ? e.TemplatedParent as RibbonGallery : null;
                }
                return gallery;
            }
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            // also close the poupup when the item is already selected:           
            if (this.IsFocused)
            {
                RibbonGallery g = Gallery;
                if (g != null) g.IsDropDownOpen = false;
            }
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            RibbonThumbnail thumb = Original != null ? Original : this;
            if (Gallery != null) Gallery.HotThumbnail = thumb;
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (Gallery != null) Gallery.HotThumbnail = null;
        }


        protected override void OnKeyUp(System.Windows.Input.KeyEventArgs e)
        {
            if (!e.Handled)
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Enter:
                    case System.Windows.Input.Key.Space:
                        Select();
                        break;
                }
            }
            base.OnKeyUp(e);
        }

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Stretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(RibbonThumbnail), new UIPropertyMetadata(Stretch.Fill));



    }
}
