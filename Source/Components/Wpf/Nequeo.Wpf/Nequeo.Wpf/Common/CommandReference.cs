/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
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
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using System.IO;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Data;

namespace Nequeo.Wpf.Common
{
    /// <summary>
    /// This class facilitates associating a key binding in XAML markup to a command
    /// defined in a View Model by exposing a Command dependency property.
    /// The class derives from Freezable to work around a limitation in WPF when data-binding from XAML.
    /// </summary>
    public class CommandReference : Freezable, ICommand
    {
        /// <summary>
        /// This class facilitates associating a key binding in XAML markup to a command
        /// defined in a View Model by exposing a Command dependency property.
        /// The class derives from Freezable to work around a limitation in WPF when data-binding from XAML.
        /// </summary>
        public CommandReference()
        {
        }

        /// <summary>
        /// Command dependency.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandReference), new PropertyMetadata(new PropertyChangedCallback(OnCommandChanged)));

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Can the command be executed.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>True if it can be executed; else false.</returns>
        public bool CanExecute(object parameter)
        {
            if (Command != null)
                return Command.CanExecute(parameter);
            return false;
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public void Execute(object parameter)
        {
            Command.Execute(parameter);
        }

        /// <summary>
        /// Can execute changed event.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// On command changed.
        /// </summary>
        /// <param name="d">The dependency.</param>
        /// <param name="e">The property changed event.</param>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CommandReference commandReference = d as CommandReference;
            ICommand oldCommand = e.OldValue as ICommand;
            ICommand newCommand = e.NewValue as ICommand;

            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -= commandReference.CanExecuteChanged;
            }
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += commandReference.CanExecuteChanged;
            }
        }

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the System.Windows.Freezable derived class.
        /// </summary>
        /// <returns>Defines an object that has a modifiable state and a read-only (frozen) state. Classes that derive 
        /// from Freezable provide detailed change notification, can be made immutable, and can clone themselves.</returns>
        protected override Freezable CreateInstanceCore()
        {
            throw new NotImplementedException();
        }
    }
}
