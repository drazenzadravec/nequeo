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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;

using Nequeo.Data.DataType;

namespace Nequeo.Web.Conversion
{
    /// <summary>
    /// Web control type conversion
    /// </summary>
    public static class WebControlType
    {
        /// <summary>
        /// Convert from one type to another.
        /// </summary>
        /// <param name="control">The web control instance.</param>
        /// <param name="type">The system type to convert to.</param>
        /// <returns>The new type containing the data.</returns>
        public static object WebControlConvertType(
            System.Web.UI.Control control, Type type)
        {
            switch (control.GetType().FullName.ToLower())
            {
                case "system.web.ui.webcontrols.checkbox":
                    CheckBox checkBox = (CheckBox)control;
                    return Nequeo.DataType.ConvertType(checkBox.Checked, type);

                default:
                    TextBox textBox = (TextBox)control;
                    return Nequeo.DataType.ConvertType(textBox.Text, type);
            }
        }

        /// <summary>
        /// Create a new web controls instance of the specified type.
        /// </summary>
        /// <param name="type">The system type.</param>
        /// <returns>The created web control.</returns>
        public static System.Web.UI.WebControls.WebControl CreateWebControl(Type type)
        {
            switch (type.FullName.ToLower())
            {
                case "system.boolean":
                case "boolean":
                case "bool":
                    return new CheckBox();

                default:
                    return new TextBox();
            }
        }

        /// <summary>
        /// Assign the default web control property with the specified value.
        /// </summary>
        /// <param name="webControl">The web control instance.</param>
        /// <param name="value">The value to assign to the web control.</param>
        /// <param name="type">The web control type.</param>
        /// <returns>The value assign web control.</returns>
        public static System.Web.UI.WebControls.WebControl AssignWebControlValue(
            System.Web.UI.WebControls.WebControl webControl, object value, Type type)
        {
            switch (type.FullName.ToLower())
            {
                case "system.web.ui.webcontrols.checkbox":
                    CheckBox checkBox = (CheckBox)webControl;
                    checkBox.Checked = (bool)value;
                    return checkBox;

                default:
                    TextBox textBox = (TextBox)webControl;
                    textBox.Text = value.ToString();
                    return textBox;
            }
        }

        /// <summary>
        /// Create a new web control required field validator from the type.
        /// </summary>
        /// <param name="type">The system type.</param>
        /// <returns>The new validator instance.</returns>
        public static System.Web.UI.WebControls.BaseValidator CreateWebControlRequiredFieldValidator(Type type)
        {
            switch (type.FullName.ToLower())
            {
                case "system.boolean":
                case "boolean":
                case "bool":
                    return null;

                default:
                    RequiredFieldValidator reg0 = new RequiredFieldValidator();
                    reg0.Text = "Required";
                    return reg0;
            }
        }

        /// <summary>
        /// Create a calendar control that extendes the text box.
        /// </summary>
        /// <param name="targetControl">The text box control.</param>
        /// <returns>The calendar control.</returns>
        public static Nequeo.Web.UI.ScriptControl.Calendar CreateCalendarControl(
            System.Web.UI.WebControls.WebControl targetControl)
        {
            // Return the calendar control.
            return new Nequeo.Web.UI.ScriptControl.Calendar()
            {
                ID = targetControl.ID + "_Calendar",
                Format = "dd/MM/yyyy",
                TargetControlID = targetControl.ID,
                PopupPosition = Nequeo.Web.Common.CalendarPosition.Right
            };
        }

        /// <summary>
        /// Create a new web control regular expression validator from the type.
        /// </summary>
        /// <param name="type">The system type.</param>
        /// <returns>The new validator instance.</returns>
        public static System.Web.UI.WebControls.BaseValidator CreateWebControlRegularExpressionValidator(Type type)
        {
            switch (type.FullName.ToLower())
            {
                case "system.sbyte":
                case "sbyte":
                case "system.int16":
                case "int16":
                case "short":
                case "system.int32":
                case "int32":
                case "int":
                case "system.int64":
                case "int64":
                case "long":
                    RegularExpressionValidator reg1 = new RegularExpressionValidator();
                    reg1.Text = "Not an integer";
                    reg1.ValidationExpression = @"^([0-9]+)|(\-[0-9]+)$";
                    return reg1;

                case "system.byte":
                case "byte":
                case "system.uint16":
                case "uint16":
                case "system.uint32":
                case "uint32":
                case "uint":
                case "system.uint64":
                case "uint64":
                case "ulong":
                    RegularExpressionValidator reg2 = new RegularExpressionValidator();
                    reg2.Text = "Not an un-signed integer";
                    reg2.ValidationExpression = @"^([0-9]+)$";
                    return reg2;

                case "system.single":
                case "single":
                case "float":
                case "system.double":
                case "double":
                case "system.decimal":
                case "decimal":
                    RegularExpressionValidator reg3 = new RegularExpressionValidator();
                    reg3.Text = "Not a number";
                    reg3.ValidationExpression = @"^([0-9]+\.[0-9]+)|(\.[0-9]+)|([0-9]+)|(\-[0-9]+\.[0-9]+)|(\-\.[0-9]+)|(\-[0-9]+)$";
                    return reg3;

                case "system.char":
                case "char":
                    RegularExpressionValidator reg4 = new RegularExpressionValidator();
                    reg4.Text = "Not a character";
                    reg4.ValidationExpression = @"^([a-z])|([A-Z]+)|([0-9])$";
                    return reg4;

                case "system.datetime":
                case "datetime":
                    RegularExpressionValidator reg5 = new RegularExpressionValidator();
                    reg5.Text = "Not a valid date time";
                    reg5.ValidationExpression = @"^([1-9]|[12][0-9]|3[01])[-/.]([1-9]|1[012])[-/.](19|20)\d\d$";
                    return reg5;

                default:
                    return null;
            }
        }
    }
}
