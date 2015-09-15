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
    public partial class RibbonWindow
    {
        public static readonly RoutedUICommand CloseCommand = new RoutedUICommand("Close", "CloseCommand", typeof(RibbonWindow));
        public static readonly RoutedUICommand MinimizeCommand = new RoutedUICommand("Minimize", "MinimizeCommand", typeof(RibbonWindow));
        public static readonly RoutedUICommand MaximizeCommand = new RoutedUICommand("Maximize", "MaximizeCommand", typeof(RibbonWindow));

        private static void RegisterCommands()
        {
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(CloseCommand, PerformClose));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(MinimizeCommand, PerformMinimize));
            CommandManager.RegisterClassCommandBinding(typeof(RibbonWindow), new CommandBinding(MaximizeCommand, PerformMaximize));

        }


        private static void PerformClose(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonWindow window = (RibbonWindow)sender;
            window.Close();
        }

        private static void PerformMinimize(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonWindow window = (RibbonWindow)sender;
            window.WindowState = WindowState.Minimized;
        }

        private static void PerformMaximize(object sender, ExecutedRoutedEventArgs e)
        {
            RibbonWindow window = (RibbonWindow)sender;
            window.WindowState = window.WindowState == WindowState.Maximized  ? WindowState.Normal : WindowState.Maximized;
        }
    }
}
