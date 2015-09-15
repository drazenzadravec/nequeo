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

namespace Nequeo.Wpf.Controls
{
    internal class InternalGroupPanel : Panel
    {
        /// <summary>
        /// Gets the maximum height to decide whether a control is small enough to be grouped vertically.
        /// </summary>
        public const double MaxSmallHeight = 24;


        public int RowsToRender
        {
            get { return (int)GetValue(RowsToRenderProperty); }
            set { SetValue(RowsToRenderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowsToRender.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowsToRenderProperty =
            DependencyProperty.Register("RowsToRender", typeof(int), typeof(InternalGroupPanel),
                    new FrameworkPropertyMetadata(2,
                            FrameworkPropertyMetadataOptions.AffectsMeasure | 
                            FrameworkPropertyMetadataOptions.AffectsArrange | 
                            FrameworkPropertyMetadataOptions.AffectsParentMeasure));



        private List<UIElement> dynamicOrder = new List<UIElement>();

        static Size infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

        protected override Size MeasureOverride(Size availableSize)
        {
            dynamicOrder.Clear();

            foreach (UIElement e in Children) e.Measure(infiniteSize);
            if (RowsToRender == 2)
            {
                Size size = FitsInDynamicRows(availableSize, 2, Children);
                if (!size.IsEmpty) return size;
            }
            else
            {

                var ordered = (from UIElement e in Children orderby e.DesiredSize.Width descending select e).ToList();

                bool swap = false;
                IEnumerable<UIElement> elements;
                while ((elements = Get3Elements(ordered, swap)) != null)
                {
                    swap ^= true;
                    foreach (UIElement e in elements) dynamicOrder.Add(e);
                }

                Size size = FitsInDynamicRows(availableSize, 3, dynamicOrder);
                if (!size.IsEmpty) return size;
            }

            return new Size(48, 3 * MaxSmallHeight);
        }

        private IEnumerable<UIElement> Get3Elements(List<UIElement> ordered, bool swap)
        {
            if (ordered.Count == 0) return null;

            List<UIElement> result = ordered.Take(3).ToList();
            if (ordered.Count > 0) ordered.RemoveAt(0);
            if (ordered.Count > 0) ordered.RemoveAt(0);
            if (ordered.Count > 0) ordered.RemoveAt(0);
            while (result.Count < 3) result.Add(null);

            if (swap) result.Reverse();
            return result;
        }

        private Size FitsInDynamicRows(Size availableSize, int rows, IEnumerable children)
        {
            double[] rowWidth = new double[rows];
            int currentRow = 0;

            foreach (UIElement e in children)
            {
                if (e == null)
                {
                    if (++currentRow >= rows) currentRow = 0;
                    continue;
                }
                Size size = e.DesiredSize;
                bool isLarge = size.Height > MaxSmallHeight;

                if (isLarge)
                {
                    currentRow = 0;
                    double maxWidth = rowWidth.Max() + size.Width;
                    if (maxWidth > availableSize.Width) return Size.Empty;

                    for (int i = 0; i < rows; i++) rowWidth[i] = maxWidth;
                }
                else
                {
                    rowWidth[currentRow] += size.Width;
                    if (rowWidth[currentRow] > availableSize.Width) return Size.Empty;

                    if (++currentRow >= rows)
                    {
                        currentRow = 0;
                    }
                }
            }

            return new Size(rowWidth.Max(), 3 * MaxSmallHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (finalSize.Width > 50)
            {
                return ArrangeDynamicRows(finalSize);
            }
            else
            {
                return ArrangeEmpty(finalSize);
            }
        }

        private Size ArrangeDynamicRows(Size finalSize)
        {
            int rowsToRender = RowsToRender;
            double[] left = new double[rowsToRender];
            double rowHeight = MaxSmallHeight;
            double topOffset = rowsToRender == 2 ? MaxSmallHeight / 3 : 0;

            int rowIndex = 0;

            IEnumerable children = dynamicOrder.Count > 0 ? (IEnumerable)dynamicOrder : Children;
            foreach (UIElement e in children)
            {
                if (e != null)
                {
                    double w = e.DesiredSize.Width;
                    e.Arrange(new Rect(left[rowIndex], topOffset + rowIndex * (rowHeight + topOffset), w, rowHeight));
                    left[rowIndex] += w;
                }
                rowIndex++;
                if (rowIndex >= rowsToRender)
                {
                    rowIndex = 0;
                }
            }
            return new Size(left.Max(), 3 * MaxSmallHeight);
        }

        private Size ArrangeEmpty(Size finalSize)
        {
            Rect r = new Rect(0, 0, 0, 0);
            foreach (UIElement e in Children)
            {
                e.Arrange(r);
            }
            return finalSize;
        }


    }
}
