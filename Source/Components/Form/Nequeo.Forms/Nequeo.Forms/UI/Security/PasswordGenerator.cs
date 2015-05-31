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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Nequeo.Cryptography;
using Nequeo.Threading;
using Nequeo.Custom;
using Nequeo.Invention;

namespace Nequeo.Forms.UI.Security
{
    /// <summary>
    /// Password generation tool.
    /// </summary>
    public partial class PasswordGenerator : UserControl
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public PasswordGenerator()
        {
            InitializeComponent();
        }

        int _selection = 0;

        /// <summary>
        /// Control loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasswordGenerator_Load(object sender, EventArgs e)
        {
            _selection = 0;
        }

        /// <summary>
        /// Password generation minimum length.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPasswordGeneratorMin_TextChanged(object sender, EventArgs e)
        {
            if ((txtPasswordGeneratorMin.Text != string.Empty) && (txtPasswordGeneratorMax.Text != string.Empty))
                btnPasswordGeneratorGen.Enabled = true;
            else
                btnPasswordGeneratorGen.Enabled = false;
        }

        /// <summary>
        /// Password generation maximum length.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtPasswordGeneratorMax_TextChanged(object sender, EventArgs e)
        {
            if ((txtPasswordGeneratorMin.Text != string.Empty) && (txtPasswordGeneratorMax.Text != string.Empty))
                btnPasswordGeneratorGen.Enabled = true;
            else
                btnPasswordGeneratorGen.Enabled = false;
        }

        /// <summary>
        /// Generate the password.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPasswordGeneratorGen_Click(object sender, EventArgs e)
        {
            // Get the minumum and maximum values.
            int min = Int32.Parse(txtPasswordGeneratorMin.Text);
            int max = Int32.Parse(txtPasswordGeneratorMax.Text);

            // If the minumum is greater than the maximum
            if (min > max)
            {
                MessageBox.Show(this, "The minimum length must not be greater than the maximum length.", "Invalid Length",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Invention.IRandomGenerator gen = null;
                string genValue = string.Empty;

                // Select the generation type.
                switch (_selection)
                {
                    case 1:
                        gen = new Invention.PasswordStandardGenerator();
                        break;

                    case 2:
                        gen = new Invention.LowerUpperCaseGenerator();
                        break;

                    case 3:
                        gen = new Invention.NumberLowerCaseGenerator();
                        break;

                    case 4:
                        gen = new Invention.NumberUpperCaseGenerator();
                        break;

                    case 5:
                        gen = new Invention.LowerCaseGenerator();
                        break;

                    case 6:
                        gen = new Invention.UpperCaseGenerator();
                        break;

                    case 7:
                        gen = new Invention.NumberGenerator();
                        break;

                    case 8:
                        gen = new Invention.TokenGenerator();
                        break;

                    case 9:
                        break;

                    case 0:
                    default:
                        gen = new Invention.PasswordGenerator();
                        break;
                }

                // Select the generation type.
                switch (_selection)
                {
                    case 9:
                        genValue = Guid.NewGuid().ToString();
                        break;

                    default:
                        genValue = gen.Random(min, max);
                        break;
                }

                // Generate the password.
                txtPasswordGeneratorGen.Text = genValue;

                // Get the password length score.
                int passwordLengthScore = 0;
                passwordStrengthLevel.Text = Invention.Validation.
                    PasswordStrength(genValue, out passwordLengthScore).ToString().Replace('_', ' ') + ",  " +
                    "Score = " + passwordLengthScore.ToString() + ",  Exellent > 100";

                // Calculate the entropy value.
                double bitRate = 0.0;
                double metricRate = 0.0;
                double entropy = Invention.Validation.EntropyShannon(genValue, out bitRate, out metricRate);
                lblEntropyValue.Text = entropy.ToString() + ",  Bit Rate = " + bitRate.ToString() + ",  Metric Rate = " + metricRate.ToString();

                // calculate the years taken to crak the password.
                double combinations = 1;
                double crackYears = Invention.Validation.PasswordCrackTime(genValue, out combinations);

                string newNumber = "";
                int exponNotation = 0;
                string metricPrefixName = "";
                string metricPrefixSymbol = "";
                string name = Invention.Converter.GetNumberShortScaleName(crackYears, out newNumber, out exponNotation, out metricPrefixName, out metricPrefixSymbol);
                lblPasswordCrackTimeValue.Text = newNumber.Split('.')[0] + " " + metricPrefixSymbol + " (" + name + ") Years" + "   (" + crackYears + " Years)";
                txtPasswordCombinations.Text = combinations.ToString();
            }
        }

        /// <summary>
        /// Generate Lower case, Upper case, numbers and special characters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonLUNS_CheckedChanged(object sender, EventArgs e)
        {
            _selection = 0;
        }

        /// <summary>
        /// Generate Lower case, Upper case and numbers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonLUN_CheckedChanged(object sender, EventArgs e)
        {
            _selection = 1;
        }

        /// <summary>
        /// Generate Lower case and Upper case.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonLU_CheckedChanged(object sender, EventArgs e)
        {
            _selection = 2;
        }

        /// <summary>
        /// Generate Lower case and numbers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonLN_CheckedChanged(object sender, EventArgs e)
        {
            _selection = 3;
        }

        /// <summary>
        /// Generate Upper case and numbers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonUN_CheckedChanged(object sender, EventArgs e)
        {
            _selection = 4;
        }

        /// <summary>
        /// Generate Lower case.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonL_CheckedChanged(object sender, EventArgs e)
        {
            _selection = 5;
        }

        /// <summary>
        /// Generate Upper case.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonU_CheckedChanged(object sender, EventArgs e)
        {
            _selection = 6;
        }

        /// <summary>
        /// Generate Number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonN_CheckedChanged(object sender, EventArgs e)
        {
            _selection = 7;
        }

        /// <summary>
        /// Generate Token.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonT_CheckedChanged(object sender, EventArgs e)
        {
            _selection = 8;
        }

        /// <summary>
        /// Generate GUID.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonGUID_CheckedChanged(object sender, EventArgs e)
        {
            _selection = 9;
        }
    }
}
