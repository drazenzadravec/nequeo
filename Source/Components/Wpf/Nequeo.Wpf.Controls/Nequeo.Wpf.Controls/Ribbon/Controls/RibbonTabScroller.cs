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
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Nequeo.Wpf.Controls
{
    /// <summary>
    /// used to scroll a RibbonTab on demand if it does not fit the available width.
    /// </summary>
    [TemplatePart(Name = partScrollViewer)]
    internal class RibbonTabScroller : ContentControl
    {
        const string partScrollViewer = "PART_ScrollViewer";

        static RibbonTabScroller()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTabScroller), new FrameworkPropertyMetadata(typeof(RibbonTabScroller)));
            RegisterCommands();
        }


        private static void RegisterCommands()
        {
            CommandManager.RegisterClassCommandBinding(typeof(RibbonTabScroller), new CommandBinding(scrollLeftCommand, ScrollLeftExecute));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonTabScroller), new CommandBinding(scrollRightCommand, ScrollRightExecute));
        }

        private static void ScrollLeftExecute(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonTabScroller scroller = (RibbonTabScroller)sender;
            scroller.Alignment = RibbonBarAlignment.Left;
        }


        private static void ScrollRightExecute(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonTabScroller scroller = (RibbonTabScroller)sender;
            scroller.Alignment = RibbonBarAlignment.Right;
        }


        private static RoutedUICommand scrollLeftCommand = new RoutedUICommand("Scroll Left", "ScrollLeftCommand", typeof(RibbonTabScroller));
        private static RoutedUICommand scrollRightCommand = new RoutedUICommand("Scroll Right", "ScrollRightCommand", typeof(RibbonTabScroller));

        public static RoutedUICommand ScrollLeftCommand { get { return scrollLeftCommand; } }
        public static RoutedUICommand ScrollRightCommand { get { return scrollRightCommand; } }



        private ScrollViewer scrollViewer;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            scrollViewer = GetTemplateChild(partScrollViewer) as ScrollViewer;
        }



        public RibbonBarAlignment Alignment
        {
            get { return (RibbonBarAlignment)GetValue(AlignmentProperty); }
            set { SetValue(AlignmentProperty, value); }
        }



        public bool CanAnimate
        {
            get { return (bool)GetValue(CanAnimateProperty); }
            set { SetValue(CanAnimateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanAnimate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanAnimateProperty =
            DependencyProperty.Register("CanAnimate", typeof(bool), typeof(RibbonTabScroller), new UIPropertyMetadata(true));



        private void SetOffset(double offset)
        {
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToHorizontalOffset(offset);
            }
        }


        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }


        // Using a DependencyProperty as the backing store for Offset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(double), typeof(RibbonTabScroller),
            new UIPropertyMetadata(0.0, OffsetPropertyChanged));

        public static void OffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTabScroller scroller = (RibbonTabScroller)d;
            double offset = (double)e.NewValue;
            scroller.SetOffset(offset);

        }


        // Using a DependencyProperty as the backing store for Alignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlignmentProperty =
            DependencyProperty.Register("Alignment", typeof(RibbonBarAlignment), typeof(RibbonTabScroller),
            new UIPropertyMetadata(RibbonBarAlignment.Full, AlignmentPropertyChanged));

        public static void AlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTabScroller scroller = (RibbonTabScroller)d;
            RibbonBarAlignment alignment = (RibbonBarAlignment)e.NewValue;

            switch(alignment)
            {
                case RibbonBarAlignment.Right:
                    scroller.ScrollRight();
                    break;

                default:
                    scroller.ScrollLeft();
                    break;
            }

            scroller.InvalidateMeasure();
            scroller.InvalidateArrange();

        }



        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(RibbonTabScroller), new UIPropertyMetadata(Colors.Transparent, ColorPropertyChanged));

        public static void ColorPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            RibbonTabScroller ts = (RibbonTabScroller)o;
            Color color = (Color)e.NewValue;
            ts.IsColorized = color != Colors.Transparent;
            ts.OnColorPopertyChanged(e);
        }

        protected virtual void OnColorPopertyChanged(DependencyPropertyChangedEventArgs e)
        {
            
        }




        public bool IsColorized
        {
            get { return (bool)GetValue(IsColorizedProperty); }
            set { SetValue(IsColorizedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsColorized.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsColorizedProperty =
            DependencyProperty.Register("IsColorized", typeof(bool), typeof(RibbonTabScroller), new UIPropertyMetadata(false));



        private void ScrollLeft()
        {
            MoveTo(0.0);
        }

        private void MoveTo(double offset)
        {
            if (CanAnimate)
            {
                DoubleAnimation a = new DoubleAnimation(offset, new Duration(TimeSpan.FromMilliseconds(250)));
                a.DecelerationRatio = 1.0;
                BeginAnimation(OffsetProperty, a);
            }
            else
            {
                Offset = offset;
            }
        }

        private void ScrollRight()
        {
            if (scrollViewer!=null)
            {
                double w = Math.Max(0, scrollViewer.ExtentWidth - scrollViewer.ActualWidth);
                MoveTo(w);
            }
        }
    }
}
