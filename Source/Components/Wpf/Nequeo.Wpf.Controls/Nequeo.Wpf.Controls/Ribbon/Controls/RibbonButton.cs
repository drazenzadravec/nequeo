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
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using Nequeo.Wpf.Controls.Ribbon.Interfaces;
using System.Windows.Media.Effects;
using Nequeo.Wpf.Controls.Interfaces;

namespace Nequeo.Wpf.Controls
{
    [ContentProperty("Content")]
    public class RibbonButton : Button, IRibbonButton,IKeyTipControl
    {

        static RibbonButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonButton), new FrameworkPropertyMetadata(typeof(RibbonButton)));
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RibbonButton), new UIPropertyMetadata(new CornerRadius(3)));


        public ImageSource LargeImage
        {
            get { return (ImageSource)GetValue(LargeImageProperty); }
            set { SetValue(LargeImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LargeImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LargeImageProperty =
            DependencyProperty.Register("LargeImage", typeof(ImageSource), typeof(RibbonButton), new FrameworkPropertyMetadata(null));


        public ImageSource SmallImage
        {
            get { return (ImageSource)GetValue(SmallImageProperty); }
            set { SetValue(SmallImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmallImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmallImageProperty =
            DependencyProperty.Register("SmallImage", typeof(ImageSource), typeof(RibbonButton), new FrameworkPropertyMetadata(null));



        public bool IsFlat
        {
            get { return (bool)GetValue(IsFlatProperty); }
            set { SetValue(IsFlatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFlat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFlatProperty =
            DependencyProperty.Register("IsFlat", typeof(bool), typeof(RibbonButton), new UIPropertyMetadata(true));




        /// <summary>
        /// Gets or sets how to stretch an image inside an IRibbonButton
        /// This is an attached dependency property.
        /// </summary>
        public static Stretch GetImageStretch(DependencyObject obj)
        {
            return (Stretch)obj.GetValue(ImageStretchProperty);
        }

        public static void SetImageStretch(DependencyObject obj, Stretch value)
        {
            obj.SetValue(ImageStretchProperty, value);
        }

        // Using a DependencyProperty as the backing store for ImageStretch.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageStretchProperty =
            DependencyProperty.RegisterAttached("ImageStretch", typeof(Stretch), typeof(RibbonButton), new UIPropertyMetadata(Stretch.Uniform));




        //public BitmapScalingMode LargeImageScalingMode
        //{
        //    get { return (BitmapScalingMode)GetValue(LargeImageScalingModeProperty); }
        //    set { SetValue(LargeImageScalingModeProperty, value); }
        //}

        //public static readonly DependencyProperty LargeImageScalingModeProperty = RibbonBar.LargeImageScalingModeProperty.AddOwner(typeof(RibbonButton));          



        #region IKeyboardCommand Members

        public void ExecuteKeyTip()
        {
            OnClick();
        }

        #endregion
    }
}
