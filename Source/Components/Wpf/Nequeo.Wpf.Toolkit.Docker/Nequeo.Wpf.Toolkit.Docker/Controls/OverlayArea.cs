/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Nequeo Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://Nequeo.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Nequeo.Wpf.Docker.Layout;

namespace Nequeo.Wpf.Docker.Controls
{
    public abstract class OverlayArea : IOverlayWindowArea
    {
        internal OverlayArea(IOverlayWindow overlayWindow)
        {
            _overlayWindow = overlayWindow;
        }

        IOverlayWindow _overlayWindow;

        Rect? _screenDetectionArea;
        Rect IOverlayWindowArea.ScreenDetectionArea
        {
            get
            {
                return _screenDetectionArea.Value;
            }
        }

        protected void SetScreenDetectionArea(Rect rect)
        {
            _screenDetectionArea = rect;
        }




    }
}
