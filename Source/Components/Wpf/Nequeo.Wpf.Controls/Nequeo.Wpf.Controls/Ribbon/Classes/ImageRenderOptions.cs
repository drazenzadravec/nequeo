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
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace Nequeo.Wpf.Controls
{
    /// <summary>
    /// Specifies how to render Images in Ribbon Controls such as RibbonButton, RibbonSplitButton, etc.
    /// </summary>
    public class ImageRenderOptions
    {
        static ImageRenderOptions()
        {
            LargeImageScalingModeProperty.AddOwner(typeof(RibbonButton));
            LargeImageScalingModeProperty.AddOwner(typeof(TextBlock));
        }

        public static BitmapScalingMode GetLargeImageScalingMode(DependencyObject obj)
        {
            return (BitmapScalingMode)obj.GetValue(LargeImageScalingModeProperty);
        }

        public static void SetLargeImageScalingMode(DependencyObject obj, BitmapScalingMode value)
        {
            obj.SetValue(LargeImageScalingModeProperty, value);
        }

        public static readonly DependencyProperty LargeImageScalingModeProperty =
            DependencyProperty.RegisterAttached("LargeImageScalingMode", 
            typeof(BitmapScalingMode), 
            typeof(ImageRenderOptions),
            new FrameworkPropertyMetadata(BitmapScalingMode.NearestNeighbor, FrameworkPropertyMetadataOptions.Inherits));



        public static BitmapScalingMode GetSmallImageScalingMode(DependencyObject obj)
        {
            return (BitmapScalingMode)obj.GetValue(SmallImageScalingModeProperty);
        }

        public static void SetSmallImageScalingMode(DependencyObject obj, BitmapScalingMode value)
        {
            obj.SetValue(SmallImageScalingModeProperty, value);
        }

        public static readonly DependencyProperty SmallImageScalingModeProperty =
            DependencyProperty.RegisterAttached("SmallImageScalingMode", 
            typeof(BitmapScalingMode), 
            typeof(ImageRenderOptions),
            new FrameworkPropertyMetadata(BitmapScalingMode.NearestNeighbor, FrameworkPropertyMetadataOptions.Inherits));





        public static EdgeMode GetLargeEdgeMode(DependencyObject obj)
        {
            return (EdgeMode)obj.GetValue(LargeEdgeModeProperty);
        }

        public static void SetLargeEdgeMode(DependencyObject obj, EdgeMode value)
        {
            obj.SetValue(LargeEdgeModeProperty, value);
        }

        public static readonly DependencyProperty LargeEdgeModeProperty =
            DependencyProperty.RegisterAttached("LargeEdgeMode", typeof(EdgeMode), typeof(ImageRenderOptions), new UIPropertyMetadata(EdgeMode.Aliased));




        public static EdgeMode GetSmallEdgeMode(DependencyObject obj)
        {
            return (EdgeMode)obj.GetValue(SmallEdgeModeProperty);
        }

        public static void SetSmallEdgeMode(DependencyObject obj, EdgeMode value)
        {
            obj.SetValue(SmallEdgeModeProperty, value);
        }

        public static readonly DependencyProperty SmallEdgeModeProperty =
            DependencyProperty.RegisterAttached("SmallEdgeMode", typeof(EdgeMode), typeof(ImageRenderOptions), new UIPropertyMetadata(EdgeMode.Aliased));





    }
}
