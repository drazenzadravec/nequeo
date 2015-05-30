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
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Automation;
using System.Globalization;
using System.Diagnostics;

namespace Nequeo.Wpf.UI.Custom
{
    /// <summary>
    /// Numeric up down custom control.
    /// </summary>
    public class NumericUpDown : Control
    {
        /// <summary>
        /// Static control initialiser.
        /// </summary>
        static NumericUpDown()
        {
            // Initialise the commands for up and down.
            InitializeCommands();

            // Listen to MouseLeftButtonDown event to determine 
            // if the focus should be placed on this control.
            // If the mouse is clickec on this control then
            // trigger the event.
            EventManager.RegisterClassHandler(typeof(NumericUpDown),
                Mouse.MouseDownEvent, new MouseButtonEventHandler(NumericUpDown.OnMouseButtonDown), true);

            // Attach this contol to the xaml type in the
            // themes fodler.
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NumericUpDown()
            : base()
        {
            // Update the text block value.
            UpdateValueString();
        }

        private const decimal DefaultMinValue = 0;
        private const decimal DefaultValue = DefaultMinValue;
        private const decimal DefaultMaxValue = 100;
        private const decimal DefaultChange = 1;
        private const int DefaultDecimalPlaces = 0;

        private static RoutedCommand _increaseCommand;
        private static RoutedCommand _decreaseCommand;

        private NumberFormatInfo _numberFormatInfo = new NumberFormatInfo();

        /// <summary>
        /// Identifies the ValueChanged routed event.
        /// </summary>
        public static readonly RoutedEvent ValueChangedEvent = 
            EventManager.RegisterRoutedEvent(
                "ValueChanged", 
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<decimal>), 
                typeof(NumericUpDown));

        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", 
                typeof(decimal), 
                typeof(NumericUpDown),
                new FrameworkPropertyMetadata(DefaultValue,
                    new PropertyChangedCallback(OnValueChanged),
                    new CoerceValueCallback(CoerceValue)));

        /// <summary>
        /// Identifies the Minimum dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum", 
                typeof(decimal), 
                typeof(NumericUpDown),
                new FrameworkPropertyMetadata(DefaultMinValue,
                    new PropertyChangedCallback(OnMinimumChanged),
                    new CoerceValueCallback(CoerceMinimum)));

        /// <summary>
        /// Identifies the Maximum dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum", 
                typeof(decimal), 
                typeof(NumericUpDown),
                new FrameworkPropertyMetadata(DefaultMaxValue,
                    new PropertyChangedCallback(OnMaximumChanged),
                    new CoerceValueCallback(CoerceMaximum)));

        /// <summary>
        /// Identifies the Change dependency property.
        /// </summary>
        public static readonly DependencyProperty ChangeProperty =
            DependencyProperty.Register(
                "Change", 
                typeof(decimal), 
                typeof(NumericUpDown),
                new FrameworkPropertyMetadata(DefaultChange, 
                    new PropertyChangedCallback(OnChangeChanged), 
                    new CoerceValueCallback(CoerceChange)),
                new ValidateValueCallback(ValidateChange));

        /// <summary>
        /// Identifies the DecimalPlaces dependency property.
        /// </summary>
        public static readonly DependencyProperty DecimalPlacesProperty =
            DependencyProperty.Register(
                "DecimalPlaces", 
                typeof(int), 
                typeof(NumericUpDown),
                new FrameworkPropertyMetadata(DefaultDecimalPlaces,
                    new PropertyChangedCallback(OnDecimalPlacesChanged)), 
                new ValidateValueCallback(ValidateDecimalPlaces));

        /// <summary>
        /// Identifies the ValueString dependency property. Provides a dependency property 
        /// identifier for limited write access to a read-only dependency property.
        /// </summary>
        private static readonly DependencyPropertyKey ValueStringPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "ValueString", 
                typeof(string), 
                typeof(NumericUpDown), 
                new PropertyMetadata());

        /// <summary>
        ///  Identifies the ValueString dependency property. Represents a property that can 
        ///  be set through methods such as, styling, data binding, animation, and inheritance.
        /// </summary>
        public static readonly DependencyProperty ValueStringProperty = ValueStringPropertyKey.DependencyProperty;

        /// <summary>
        /// Occurs when the Value property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<decimal> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }
        
        /// <summary>
        /// Gets sets, internal value that will be the text in the TextBlock control.
        /// </summary>
        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Gets sets, the minimum value that can be applied in the text.
        /// </summary>
        public decimal Minimum
        {
            get { return (decimal)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Gets sets, the maximum value that can be applied in the text.
        /// </summary>
        public decimal Maximum
        {
            get { return (decimal)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Gets sets, the incremental change factor from minimum to maximun and visa.vera
        /// </summary>
        public decimal Change
        {
            get { return (decimal)GetValue(ChangeProperty); }
            set { SetValue(ChangeProperty, value); }
        }

        /// <summary>
        /// Gets sets, the number of decimal places to apply.
        /// </summary>
        public int DecimalPlaces
        {
            get { return (int)GetValue(DecimalPlacesProperty); }
            set { SetValue(DecimalPlacesProperty, value); }
        }

        /// <summary>
        /// Gets the value in the text as string.
        /// </summary>
        public string ValueString
        {
            get { return (string)GetValue(ValueStringProperty); }
        }

        /// <summary>
        /// Gets, the increase command routed handler for the up button.
        /// </summary>
        public static RoutedCommand IncreaseCommand
        {
            get { return _increaseCommand; }
        }

        /// <summary>
        /// Gets, the decrease command routed handler for the down button.
        /// </summary>
        public static RoutedCommand DecreaseCommand
        {
            get { return _decreaseCommand; }
        }

        /// <summary>
        /// This is a class handler for MouseLeftButtonDown event.
        /// The purpose of this handle is to move input focus to NumericUpDown when user pressed
        /// mouse left button on any part of slider that is not focusable.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event that occured.</param>
        private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            NumericUpDown control = (NumericUpDown)sender;

            // When someone click on a part in the NumericUpDown and it's not focusable
            // NumericUpDown needs to take the focus in order to process keyboard correctly
            if (!control.IsKeyboardFocusWithin)
            {
                e.Handled = control.Focus() || e.Handled;
            }
        }

        /// <summary>
        /// Initialise the commands for up and down.
        /// </summary>
        private static void InitializeCommands()
        {
            // Create a new instance of the command router
            _increaseCommand = new RoutedCommand("IncreaseCommand", typeof(NumericUpDown));

            // Register the command actions, when the button is clicked
            // and when the up key is pressed.
            CommandManager.RegisterClassCommandBinding(typeof(NumericUpDown), new CommandBinding(_increaseCommand, OnIncreaseCommand));
            CommandManager.RegisterClassInputBinding(typeof(NumericUpDown), new InputBinding(_increaseCommand, new KeyGesture(Key.Up)));

            // Create a new instance of the command router
            _decreaseCommand = new RoutedCommand("DecreaseCommand", typeof(NumericUpDown));

            // Register the command actions, when the button is clicked
            // and when the down key is pressed.
            CommandManager.RegisterClassCommandBinding(typeof(NumericUpDown), new CommandBinding(_decreaseCommand, OnDecreaseCommand));
            CommandManager.RegisterClassInputBinding(typeof(NumericUpDown), new InputBinding(_decreaseCommand, new KeyGesture(Key.Down)));
        }

        /// <summary>
        /// Handles the increase command event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The rvent argument.</param>
        private static void OnIncreaseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            NumericUpDown control = sender as NumericUpDown;
            if (control != null)
                control.OnIncrease();
        }

        /// <summary>
        /// Handles the decrease command event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The rvent argument.</param>
        private static void OnDecreaseCommand(object sender, ExecutedRoutedEventArgs e)
        {
            NumericUpDown control = sender as NumericUpDown;
            if (control != null)
                control.OnDecrease();
        }

        /// <summary>
        /// Increases the value.
        /// </summary>
        protected virtual void OnIncrease()
        {
            this.Value += Change;
        }

        /// <summary>
        /// Decreases the value.
        /// </summary>
        protected virtual void OnDecrease()
        {
            this.Value -= Change;
        }

        /// <summary>
        /// The handler when the value changes.
        /// </summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="args">The event arguments.</param>
        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            // Get the curent control
            NumericUpDown control = (NumericUpDown)obj;

            // Get the new and onld values.
            decimal oldValue = (decimal)args.OldValue;
            decimal newValue = (decimal)args.NewValue;

            // Get the automation peer control.
            NumericUpDownAutomationPeer peer = UIElementAutomationPeer.FromElement(control) as NumericUpDownAutomationPeer;
            if (peer != null)
                peer.RaiseValueChangedEvent(oldValue, newValue);
            
            // Create a new routed property changed event argument
            // add the value change routed event.
            RoutedPropertyChangedEventArgs<decimal> e = new RoutedPropertyChangedEventArgs<decimal>(
                oldValue, newValue, ValueChangedEvent);

            // Rasie the value change event to the attached client.
            control.OnValueChanged(e);
            control.UpdateValueString();
        }

        /// <summary>
        /// Create a new numeric automation peer from the override method.
        /// </summary>
        /// <returns>The automation perr.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new NumericUpDownAutomationPeer(this);
        }

        /// <summary>
        /// Raises the ValueChanged event to the user that is attachec to the ValueChanged event router.
        /// </summary>
        /// <param name="args">Arguments associated with the ValueChanged event.</param>
        protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<decimal> args)
        {
            RaiseEvent(args);
        }

        /// <summary>
        /// Update the value in the text block as a string.
        /// </summary>
        private void UpdateValueString()
        {
            _numberFormatInfo.NumberDecimalDigits = this.DecimalPlaces;
            string newValueString = this.Value.ToString("f", _numberFormatInfo);
            this.SetValue(ValueStringPropertyKey, newValueString);
        }

        /// <summary>
        /// Coerce the value
        /// </summary>
        /// <param name="element">The current control.</param>
        /// <param name="value">The current value</param>
        /// <returns>The new value to assign.</returns>
        private static object CoerceValue(DependencyObject element, object value)
        {
            decimal newValue = (decimal)value;
            NumericUpDown control = (NumericUpDown)element;

            // Get the maximum value of the to values
            newValue = System.Math.Max(control.Minimum, System.Math.Min(control.Maximum, newValue));
            newValue = Decimal.Round(newValue, control.DecimalPlaces);

            // Return the new value.
            return newValue;
        }
        
        /// <summary>
        /// Minimum value changed handler.
        /// </summary>
        /// <param name="element">The dependency object.</param>
        /// <param name="args">Dependency arguments.</param>
        private static void OnMinimumChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            // Coerce the maximum property and
            // the value property.
            element.CoerceValue(MaximumProperty);
            element.CoerceValue(ValueProperty);
        }

        /// <summary>
        /// Coerce the minimum value.
        /// </summary>
        /// <param name="element">The dependency object.</param>
        /// <param name="value">The current minimum value.</param>
        /// <returns>The new minimum value.</returns>
        private static object CoerceMinimum(DependencyObject element, object value)
        {
            decimal minimum = (decimal)value;
            NumericUpDown control = (NumericUpDown)element;
            return Decimal.Round(minimum, control.DecimalPlaces);
        }

        /// <summary>
        /// Maximum value changed handler.
        /// </summary>
        /// <param name="element">The dependency object.</param>
        /// <param name="args">Dependency arguments.</param>
        private static void OnMaximumChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            // Coerce the value property.
            element.CoerceValue(ValueProperty);
        }

        /// <summary>
        /// Coerce the maximum value.
        /// </summary>
        /// <param name="element">The dependency object.</param>
        /// <param name="value">The current maximum value.</param>
        /// <returns>The new maximum value.</returns>
        private static object CoerceMaximum(DependencyObject element, object value)
        {
            NumericUpDown control = (NumericUpDown)element;
            decimal newMaximum = (decimal)value;
            return Decimal.Round(System.Math.Max(newMaximum, control.Minimum), control.DecimalPlaces);
        }

        /// <summary>
        /// Decimal Places value changed handler.
        /// </summary>
        /// <param name="element">The dependency object.</param>
        /// <param name="args">Dependency arguments.</param>
        private static void OnDecimalPlacesChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
            // Coerce the properties.
            NumericUpDown control = (NumericUpDown)element;
            control.CoerceValue(ChangeProperty);
            control.CoerceValue(MinimumProperty);
            control.CoerceValue(MaximumProperty);
            control.CoerceValue(ValueProperty);
            control.UpdateValueString();
        }

        /// <summary>
        /// Validate the number of decimal places.
        /// </summary>
        /// <param name="value">The current decimal place value</param>
        /// <returns>Is the decimal place greater than or equal to zero then true else false.</returns>
        private static bool ValidateDecimalPlaces(object value)
        {
            int decimalPlaces = (int)value;
            return decimalPlaces >= 0;
        }

        /// <summary>
        /// Change value changed handler.
        /// </summary>
        /// <param name="element">The dependency object.</param>
        /// <param name="args">Dependency arguments.</param>
        private static void OnChangeChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        /// Validate the value of the change increment value.
        /// </summary>
        /// <param name="value">The current change value.</param>
        /// <returns>True if the change value is greater that zero else false.</returns>
        private static bool ValidateChange(object value)
        {
            decimal change = (decimal)value;
            return change > 0;
        }

        /// <summary>
        /// Coerce the change value.
        /// </summary>
        /// <param name="element">The dependency object.</param>
        /// <param name="value">The current change value.</param>
        /// <returns>The new change value.</returns>
        private static object CoerceChange(DependencyObject element, object value)
        {
            decimal newChange = (decimal)value;
            NumericUpDown control = (NumericUpDown)element;

            // Get the chnage value according to the decimal places.
            decimal coercedNewChange = Decimal.Round(newChange, control.DecimalPlaces);

            //If Change is .1 and DecimalPlaces is changed from 1 to 0, we want Change to go to 1, not 0.
            //Put another way, Change should always be rounded to DecimalPlaces, but never smaller than the 
            //previous Change
            if (coercedNewChange < newChange)
                coercedNewChange = SmallestForDecimalPlaces(control.DecimalPlaces);

            // Return the new change value.
            return coercedNewChange;
        }

        /// <summary>
        /// Get the smallest decimal places value.
        /// </summary>
        /// <param name="decimalPlaces">The current decimal places value.</param>
        /// <returns>The decimal places.</returns>
        private static decimal SmallestForDecimalPlaces(int decimalPlaces)
        {
            if (decimalPlaces < 0)
                throw new ArgumentException("decimalPlaces");

            decimal d = 1;

            // 1/10, 1/100, 1/1000, 1/10000 etc... 0.00001 etc.
            for (int i = 0; i < decimalPlaces; i++)
                d /= 10;

            // Return the decimal place holder
            return d;
        }
    }

    /// <summary>
    /// Numeric up down automation handler.
    /// </summary>
    public class NumericUpDownAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
    {
        /// <summary>
        /// Deafault Constructor.
        /// </summary>
        /// <param name="control">The numeric up down control</param>
        public NumericUpDownAutomationPeer(NumericUpDown control)
            : base(control)
        {
        }

        /// <summary>
        /// Gets, the current numeric up down control instance.
        /// </summary>
        private NumericUpDown MyOwner
        {
            get { return (NumericUpDown)base.Owner; }
        }

        /// <summary>
        /// Get the class name of the numeric up down control.
        /// </summary>
        /// <returns>The class name.</returns>
        protected override string GetClassNameCore()
        {
            return "NumericUpDown";
        }

        /// <summary>
        /// Get the automation control type to apply.
        /// </summary>
        /// <returns>The automation type.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Spinner;
        }

        /// <summary>
        /// Get the automation pattern
        /// </summary>
        /// <param name="patternInterface">The pattern interface.</param>
        /// <returns>The current automation pattern interface.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            // If the pattern interface is a range type
            // then return this automation control.
            if (patternInterface == PatternInterface.RangeValue)
            {
                return this;
            }
            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Raise value change event.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        internal void RaiseValueChangedEvent(decimal oldValue, decimal newValue)
        {
            // Raise the event
            base.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty,
                (double)oldValue, (double)newValue);
        }

        /// <summary>
        /// Gets, is read only value property control.
        /// </summary>
        bool IRangeValueProvider.IsReadOnly
        {
            get { return !IsEnabled(); }
        }

        /// <summary>
        /// Gets, the largest change from minimum to maximum and visa.versa.
        /// </summary>
        double IRangeValueProvider.LargeChange
        {
            get { return (double)MyOwner.Change; }
        }

        /// <summary>
        /// Gets, the maximun value.
        /// </summary>
        double IRangeValueProvider.Maximum
        {
            get { return (double)MyOwner.Maximum; }
        }

        /// <summary>
        /// Gets, the minimum value.
        /// </summary>
        double IRangeValueProvider.Minimum
        {
            get { return (double)MyOwner.Minimum; }
        }

        /// <summary>
        /// Gets, the smallest change from minimum to maximum and visa.versa.
        /// </summary>
        double IRangeValueProvider.SmallChange
        {
            get { return (double)MyOwner.Change; }
        }

        /// <summary>
        /// Gets, the curent value.
        /// </summary>
        double IRangeValueProvider.Value
        {
            get { return (double)MyOwner.Value; }
        }

        /// <summary>
        /// Set the new value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        void IRangeValueProvider.SetValue(double value)
        {
            if (!IsEnabled())
                throw new ElementNotEnabledException();

            decimal val = (decimal)value;
            if (val < MyOwner.Minimum || val > MyOwner.Maximum)
                throw new ArgumentOutOfRangeException("value");

            // Assign the new value to the control.
            MyOwner.Value = val;
        }
    }
}
