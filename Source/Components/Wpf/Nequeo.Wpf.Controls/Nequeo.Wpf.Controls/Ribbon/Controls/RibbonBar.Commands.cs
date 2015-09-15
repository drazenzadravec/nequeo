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

namespace Nequeo.Wpf.Controls
{
    partial class RibbonBar
    {
        public static readonly RoutedUICommand AlignGroupsLeftCommand = new RoutedUICommand("Align Left", "AlignGroupsLeftCommand", typeof(RibbonBar));
        public static readonly RoutedUICommand AlignGroupsRightCommand = new RoutedUICommand("Align Right", "AlignGroupsRightCommand", typeof(RibbonBar));
        public  static readonly RoutedUICommand CollapseRibbonBarCommand = new RoutedUICommand("", "CollapseRibbonBarCommand", typeof(RibbonBar));

        public static readonly RoutedUICommand QAPlacementTopCommand = new RoutedUICommand("Show Above the Ribbon.", "QAPlacementTopCommand", typeof(RibbonBar));
        public static readonly RoutedUICommand QAPlacementBottomCommand = new RoutedUICommand("Show Below the Ribbon.", "QAPlacementBottomCommand", typeof(RibbonBar));

        private static void RegisterCommands()
        {
            CommandManager.RegisterClassCommandBinding(typeof(RibbonBar), new CommandBinding(AlignGroupsLeftCommand, alignGroupsLeft));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonBar), new CommandBinding(AlignGroupsRightCommand, alignGroupsRight));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonBar), new CommandBinding(CollapseRibbonBarCommand, collapseRibbonBar));

            CommandManager.RegisterClassCommandBinding(typeof(RibbonBar), new CommandBinding(QAPlacementTopCommand, QAPlacementTop, IsQAPlacementTopEnabled));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonBar), new CommandBinding(QAPlacementBottomCommand, QAPlacementBottom, IsQAPlacementBottomEnabled));
        }

        private static void QAPlacementTop(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)sender;
            bar.ToolbarPlacement = QAPlacement.Top;
        }

        private static void QAPlacementBottom(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)sender;
            bar.ToolbarPlacement = QAPlacement.Bottom;
        }

        private static void IsQAPlacementTopEnabled(object sender, CanExecuteRoutedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)sender;
            e.CanExecute = bar.ToolbarPlacement == QAPlacement.Bottom;
        }

        private static void IsQAPlacementBottomEnabled(object sender, CanExecuteRoutedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)sender;
            e.CanExecute = bar.ToolbarPlacement == QAPlacement.Top;
        }

        private static void collapseRibbonBar(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)sender;
            bar.IsExpanded = false;

        }

        private static void alignGroupsLeft(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)sender;
            bar.AlignGroupsLeft();
       }


        private static void alignGroupsRight(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonBar bar = (RibbonBar)sender;
            bar.AlignGroupsRight();
        }
    }
}
