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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Collections;
using System.Diagnostics;
using Nequeo.Wpf.Controls.Ribbon.Interfaces;

namespace Nequeo.Wpf.Controls
{
    public class RibbonWrapPanel : Panel
    {
        static RibbonWrapPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonWrapPanel), new FrameworkPropertyMetadata(typeof(RibbonWrapPanel)));
        }


        const double smallHeight = 24;


        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement e in Children) e.Measure(infiniteSize);
            return ArrangeOrMeasure(false);
        }


        private static Size infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

        protected override Size ArrangeOverride(Size finalSize)
        {
            return ArrangeOrMeasure(true);
        }

        private Size ArrangeOrMeasure(bool arrange)
        {
            double left = 0;
            int rowIndex = 0;
            int maxRows = 3;

            List<UIElement> rowElements = new List<UIElement>(maxRows);

            foreach (UIElement e in Children)
            {
                if (e.Visibility != Visibility.Visible) continue;
                IRibbonControl ribbonControl = e as IRibbonControl;
                Size dsize = e.DesiredSize;
                if (dsize.Height > smallHeight)
                {
                    if (rowIndex > 0)
                    {
                        left += ArrangeRow(rowElements, left, arrange);
                        rowIndex = 0;
                    }
                    if (arrange)
                    {
                        Size size = e.DesiredSize;
                        double h = Math.Max(smallHeight, size.Height);
                        e.Arrange(new Rect(left, 0, size.Width, h));
                    }
                    left += e.DesiredSize.Width;
                }
                else
                {
                    RibbonSize size = RibbonBar.GetSize(e);
                    if (size != RibbonSize.Minimized)
                    {
                        rowElements.Add(e);
                        if (++rowIndex == maxRows)
                        {
                            left += ArrangeRow(rowElements, left, arrange);
                            rowIndex = 0;
                        }
                    }
                }
            }
            left += ArrangeRow(rowElements, left, arrange);

            left = Math.Max(32, left);
            return new Size(left, smallHeight * 3);
        }


        protected double ArrangeRow(List<UIElement> rowElements, double left, bool arrange)
        {
            double max = 0;

            double rowHeight = smallHeight + (rowElements.Count == 2 ? (smallHeight / 3) : 0);
            double topOffset = rowElements.Count == 2 ? smallHeight / 3 : 0;

            foreach (UIElement e in rowElements)
            {
                max = Math.Max(e.DesiredSize.Width, max);
            }
            foreach (UIElement e in rowElements)
            {
                if (arrange)
                {
                    double h = Math.Max(smallHeight, e.DesiredSize.Height);
                    double w = e is IRibbonStretch ? max : e.DesiredSize.Width;

                    FrameworkElement fe = e as FrameworkElement;
                    if (fe != null && fe.HorizontalAlignment != HorizontalAlignment.Left)
                    {
                        switch (fe.HorizontalAlignment)
                        {
                            case HorizontalAlignment.Right:
                                e.Arrange(new Rect(max - w + left, topOffset, w, h));
                                break;

                            case HorizontalAlignment.Center:
                                e.Arrange(new Rect((max - w) / 2 + left, topOffset, w, h));
                                break;

                            case HorizontalAlignment.Left:
                            case HorizontalAlignment.Stretch:
                                e.Arrange(new Rect(left, topOffset, w, h));
                                break;
                        }
                    }
                    else e.Arrange(new Rect(left, topOffset, w, h));
                }
                topOffset += rowHeight;
            }
            rowElements.Clear();
            return max;
        }


    }
}
