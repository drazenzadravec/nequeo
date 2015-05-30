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
using System.Collections;
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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Threading;

namespace Nequeo.Wpf.Validation
{
    /// <summary>
    /// Integer validation rule (any valid integer number).
    /// </summary>
    public class Integer : ValidationRule
    {
        private long _min = 0;
        
        /// <summary>
        /// Gets sets, the minimum value before validation applies
        /// </summary>
        public long Minimum
        {
            get { return _min; }
            set { _min = value; }
        }

        /// <summary>
        /// On validate.
        /// </summary>
        public event EventHandler<Nequeo.Custom.ValidationArgs> OnValidate;

        /// <summary>
        /// Validation trigger
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            long currentValue = 0;

            try
            {
                // Has a valid number been passed.
                if (((string)value).Length > 0)
                    currentValue = Int64.Parse((String)value);
            }
            catch (Exception ex)
            {
                OnValidation(false);
                // An illegal characters has been entered.
                return new ValidationResult(false, "Illegal characters or " + ex.Message);
            }

            // If the number entered is less than the minimum
            if (currentValue < Minimum)
            {
                OnValidation(false);
                return new ValidationResult(false, "Please enter a value greater than or equal to : " + _min.ToString());
            }
            else
            {
                OnValidation(true);
                return new ValidationResult(true, null);
            }       
        }

        /// <summary>
        /// On validation
        /// </summary>
        /// <param name="valid">Is the validation successfull.</param>
        private void OnValidation(bool valid)
        {
            if (OnValidate != null)
            {
                Nequeo.Custom.ValidationArgs op = new Nequeo.Custom.ValidationArgs(valid);
                OnValidate(this, op);
            }
        }
    }
}
