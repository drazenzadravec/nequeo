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
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Nequeo.Wpf.Controls
{
    partial class RibbonGallery
    {
        public static RoutedUICommand ScrollUpCommand = new RoutedUICommand("ScrollUpCommand", "ScrollUpCommand", typeof(RibbonGallery));
        public static RoutedUICommand ScrollDownCommand = new RoutedUICommand("ScrollDownCommand", "ScrollDownCommand", typeof(RibbonGallery));

        private static void RegisterCommands()
        {
            CommandManager.RegisterClassCommandBinding(typeof(RibbonGallery), new CommandBinding(ScrollUpCommand, scrollUpCommand));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonGallery), new CommandBinding(ScrollDownCommand, scrollDownCommand));
        }

        private static void scrollUpCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonGallery gallery = (RibbonGallery)sender;
            //RoutedEventArgs args = new RoutedEventArgs(LaunchDialogEvent);
            //gallery.RaiseEvent(args);
            gallery.ScrollUp();
        }



        private static void scrollDownCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonGallery gallery = (RibbonGallery)sender;
            //RoutedEventArgs args = new RoutedEventArgs(LaunchDialogEvent);
            //gallery.RaiseEvent(args);
            gallery.ScrollDown();


        }

        /// <summary>
        /// Gets the ScrollViewer for the In-Ribbon Panel.
        /// </summary>
        protected virtual ScrollViewer ScrollViewer
        {
            get { return wrapPanel.Parent as ScrollViewer; }
        }

        /// <summary>
        /// Scrolls the In-Ribbon Gallery on row down.
        /// </summary>
        public void ScrollDown()
        {
            ScrollViewer sv = ScrollViewer;
            if (sv != null)
            {
                double h = CalculateInRibbonThumbnailHeight();
                double offset = Math.Floor(sv.VerticalOffset / h) * h + h;
                ScrollTo(offset);
            }          
        }


        /// <summary>
        /// Scrolls the In-Ribbon Gallery on row up.
        /// </summary>
        public void ScrollUp()
        {
            ScrollViewer sv = ScrollViewer;
            if (sv != null)
            {
                double h = CalculateInRibbonThumbnailHeight();
                double offset = Math.Ceiling(sv.VerticalOffset / h) * h - h;
                ScrollTo(offset);
            }
        }

        private void ScrollTo(double verticalOffset)
        {
            VerticalScrollOffset = ScrollViewer.VerticalOffset;
            DoubleAnimation a = new DoubleAnimation(verticalOffset, new Duration(TimeSpan.FromMilliseconds(250)));
            a.DecelerationRatio = 1.0;
            BeginAnimation(VerticalScrollOffsetProperty, a);
        }


        /// <summary>
        /// Gets or set the vertical scroll offset for the In-Ribbon Gallery.
        /// This a dependency property
        /// </summary>
        /// <remarks>
        /// This property is used to perform scrolling animation.
        /// </remarks>
        public double VerticalScrollOffset
        {
            get { return (double)GetValue(VerticalScrollOffsetProperty); }
            set { SetValue(VerticalScrollOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VerticalScrollOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VerticalScrollOffsetProperty =
            DependencyProperty.Register("VerticalScrollOffset", typeof(double), typeof(RibbonGallery), new UIPropertyMetadata(0.0, VerticalScrollOffsetPropertyChanged));

        private static void VerticalScrollOffsetPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonGallery g = (RibbonGallery)o;
            double value = (double)e.NewValue;
            g.OnVerticalScrollOffsetPropertychanged(value);
        }

        private void OnVerticalScrollOffsetPropertychanged(double value)
        {
            ScrollViewer sv = ScrollViewer;
            if (sv != null) sv.ScrollToVerticalOffset(value);
        }



    }
}
