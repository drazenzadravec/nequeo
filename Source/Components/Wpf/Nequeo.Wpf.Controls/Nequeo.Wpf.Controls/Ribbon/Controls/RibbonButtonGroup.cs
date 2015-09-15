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
using Nequeo.Wpf.Controls.Ribbon.Interfaces;

namespace Nequeo.Wpf.Controls
{

    [ContentProperty("Items")]
    public class RibbonButtonGroup : ItemsControl
    {
        static RibbonButtonGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonButtonGroup), new FrameworkPropertyMetadata(typeof(RibbonButtonGroup)));
        }


        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RibbonButtonGroup), new UIPropertyMetadata(new CornerRadius(3)));



        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            int max = Items.Count-1;
            CornerRadius r = CornerRadius;
            for (int i = 0; i <= max; i++)
            {
                IRibbonButton b = Items[i] as IRibbonButton;
                if (i == 0 && i == max) b.CornerRadius = CornerRadius;
                else if (i == 0) b.CornerRadius = new CornerRadius(r.TopLeft, 0d, 0d, r.BottomLeft);
                else if (i == max) b.CornerRadius = new CornerRadius(0d, r.TopRight, r.BottomRight, 0d);
                else b.CornerRadius = new CornerRadius(0);
            }
        }        


        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonButton();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            IRibbonButton b = element as IRibbonButton;
            if (b != null)
            {
                RibbonBar.SetSize(element, RibbonSize.Small);
            }
        }
    }
}
