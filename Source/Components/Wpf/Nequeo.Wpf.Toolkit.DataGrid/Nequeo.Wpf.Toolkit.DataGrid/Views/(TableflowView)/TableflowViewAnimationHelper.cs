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

namespace Nequeo.Wpf.DataGrid.Views
{
  internal class TableflowViewAnimationHelper
  {
    #region Public Methods

    public static double GetAnimationStep( double startPos, double finalPos, double elapsedTime, double totalTime )
    {
      const double power = 6;

      return startPos +
        ( ( 1 - System.Math.Pow( totalTime - elapsedTime, power ) / System.Math.Pow( totalTime, power ) ) *
        ( finalPos - startPos ) );
    } 

    #endregion
  }
}
