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
using System.Threading;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;

namespace Nequeo.Invention
{
    /// <summary>
    /// Password strength level
    /// </summary>
    public enum PasswordStrengthLevel
    {
        /// <summary>
        /// Poor; score 0 - 10
        /// </summary>
        Poor = 0,
        /// <summary>
        /// Poor to Weak; score 11 - 20
        /// </summary>
        Poor_to_Weak = 1,
        /// <summary>
        /// Weak; score 21 - 30
        /// </summary>
        Weak = 2,
        /// <summary>
        /// Weak to Average; score 31 - 40
        /// </summary>
        Weak_to_Average = 3,
        /// <summary>
        /// Average; score 41 - 50
        /// </summary>
        Average = 4,
        /// <summary>
        /// Average to Good; score 51 - 60
        /// </summary>
        Average_to_Good = 5,
        /// <summary>
        /// Good; score 61 - 70
        /// </summary>
        Good = 6,
        /// <summary>
        /// Good to Strong; score 71 - 80
        /// </summary>
        Good_to_Strong = 7,
        /// <summary>
        /// Strong; score 81 - 90
        /// </summary>
        Strong = 8,
        /// <summary>
        /// Strong to Exellent; score 91 - 100
        /// </summary>
        Strong_to_Exellent = 9,
        /// <summary>
        /// Exellent; score 101 - Infinity
        /// </summary>
        Exellent = 10,
    }

    /// <summary>
    /// Sets the initial password strength values.
    /// </summary>
    public struct PasswordStrengthInitialValues
    {
        /// <summary>
        /// Password strength initialisation value constructor.
        /// </summary>
        /// <param name="lowerThreshold">The lower threshold character count; that is, if equal to this value then lower score is applied.</param>
        /// <param name="higherThreshold">The higher threshold character count; that is, if equal to this value then higher score is applied. This value must be greater than lower threshold.</param>
        /// <param name="higherThresholdScore">The higher threshold score; that is, if greater than higher threshold than the difference is multiplied by this value.</param>
        /// <param name="lowerScore">The lower score; that is, if equal to the lower threshold value then this score is applied.</param>
        /// <param name="higherScore">The higher score; that is, if equal to the higher threshold value then this score is applied.</param>
        public PasswordStrengthInitialValues(int lowerThreshold, int higherThreshold, int higherThresholdScore, int lowerScore, int higherScore)
        {
            if (higherThreshold <= lowerThreshold) throw new IndexOutOfRangeException("Higher threshold must be greater than lower threshold.");

            LowerThreshold = lowerThreshold;
            HigherThreshold = higherThreshold;
            HigherThresholdScore = higherThresholdScore;
            LowerScore = lowerScore;
            HigherScore = higherScore;
        }

        /// <summary>
        /// The lower threshold character count; that is, if equal to this value then lower score is applied.
        /// </summary>
        public int LowerThreshold;

        /// <summary>
        /// The higher threshold character count; that is, if equal to this value then higher score is applied. This value must be greater than lower threshold.
        /// </summary>
        public int HigherThreshold;

        /// <summary>
        /// The higher threshold score; that is, if greater than higher threshold than the difference is multiplied by this value. 
        /// </summary>
        public int HigherThresholdScore;

        /// <summary>
        /// The lower score; that is, if equal to the lower threshold value then this score is applied.
        /// </summary>
        public int LowerScore;

        /// <summary>
        /// The higher score; that is, if equal to the higher threshold value then this score is applied.
        /// </summary>
        public int HigherScore;
    }

    /// <summary>
    /// Validation class.
    /// </summary>
    public sealed class Validation
    {
        /// <summary>
        /// Gets the password strength level for the password.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <param name="score">The score calculated for the password.</param>
        /// <param name="initialValues">The initial password strength values.</param>
        /// <returns>The password strength level.</returns>
        public static PasswordStrengthLevel PasswordStrength(string password, out int score, PasswordStrengthInitialValues? initialValues = null)
        {
            if (String.IsNullOrEmpty(password)) throw new ArgumentNullException("password");

            score = 0;
            PasswordStrengthLevel passwordStrengthLevel = PasswordStrengthLevel.Poor;

            // Upper case letter initialisation values.
            int upperCaseLetterInitLowerThreshold = 1;
            int upperCaseLetterInitHigherThreshold = 2;
            int upperCaseLetterInitHigherThresholdScore = 2;
            int upperCaseLetterInitLowerScore = 7;
            int upperCaseLetterInitHigherScore = 15;

            // Lower case letter initialisation values.
            int lowerCaseLetterInitLowerThreshold = 1;
            int lowerCaseLetterInitHigherThreshold = 2;
            int lowerCaseLetterInitHigherThresholdScore = 2;
            int lowerCaseLetterInitLowerScore = 7;
            int lowerCaseLetterInitHigherScore = 15;

            // Number initialisation values.
            int numberCountInitLowerThreshold = 1;
            int numberCountInitHigherThreshold = 2;
            int numberCountInitHigherThresholdScore = 2;
            int numberCountInitLowerScore = 7;
            int numberCountInitHigherScore = 15;

            // Special characters initialisation values.
            int specialCountInitLowerThreshold = 1;
            int specialCountInitHigherThreshold = 2;
            int specialCountInitHigherThresholdScore = 2;
            int specialCountInitLowerScore = 7;
            int specialCountInitHigherScore = 15;

            // Set the initial password strength values.
            if (initialValues != null)
            {
                // Upper case letter initialisation values.
                upperCaseLetterInitLowerThreshold = initialValues.Value.LowerThreshold;
                upperCaseLetterInitHigherThreshold = initialValues.Value.HigherThreshold;
                upperCaseLetterInitHigherThresholdScore = initialValues.Value.HigherThresholdScore;
                upperCaseLetterInitLowerScore = initialValues.Value.LowerScore;
                upperCaseLetterInitHigherScore = initialValues.Value.HigherScore;

                // Lower case letter initialisation values.
                lowerCaseLetterInitLowerThreshold = initialValues.Value.LowerThreshold;
                lowerCaseLetterInitHigherThreshold = initialValues.Value.HigherThreshold;
                lowerCaseLetterInitHigherThresholdScore = initialValues.Value.HigherThresholdScore;
                lowerCaseLetterInitLowerScore = initialValues.Value.LowerScore;
                lowerCaseLetterInitHigherScore = initialValues.Value.HigherScore;

                // Number initialisation values.
                numberCountInitLowerThreshold = initialValues.Value.LowerThreshold;
                numberCountInitHigherThreshold = initialValues.Value.HigherThreshold;
                numberCountInitHigherThresholdScore = initialValues.Value.HigherThresholdScore;
                numberCountInitLowerScore = initialValues.Value.LowerScore;
                numberCountInitHigherScore = initialValues.Value.HigherScore;

                // Special characters initialisation values.
                specialCountInitLowerThreshold = initialValues.Value.LowerThreshold;
                specialCountInitHigherThreshold = initialValues.Value.HigherThreshold;
                specialCountInitHigherThresholdScore = initialValues.Value.HigherThresholdScore;
                specialCountInitLowerScore = initialValues.Value.LowerScore;
                specialCountInitHigherScore = initialValues.Value.HigherScore;
            }

            // Initialise the character count in the password.
            int upperCaseLetterCount = 0;
            int lowerCaseLetterCount = 0;
            int numberCount = 0;
            int specialCount = 0;

            // For each character in the password
            foreach (Char character in password)
            {
                // Is upper case letter
                if (Char.IsUpper(character))
                    // Count upper case letters.
                    upperCaseLetterCount++;

                // Is lower case letter
                if (Char.IsLower(character))
                    // Count lower case letters.
                    lowerCaseLetterCount++;

                // Is number
                if (Char.IsNumber(character))
                    // Count numbers.
                    numberCount++;

                // Select the special characters.
                switch(character)
                {
                    case '*':
                    case '$':
                    case '-':
                    case '+':
                    case '?':
                    case '_':
                    case '&':
                    case '=':
                    case '!':
                    case '%':
                    case '{':
                    case '}':
                    case '/':
                        // Count special characters.
                        specialCount++;
                        break;
                }
            }

            // Upper case letter initial scores for
            // lower threshold number of characters (1 - char), and
            // higher threshold number of characters (2 - char).
            if (upperCaseLetterCount == upperCaseLetterInitLowerThreshold)
                score += upperCaseLetterInitLowerScore;
            else if (upperCaseLetterCount >= upperCaseLetterInitHigherThreshold)
                score += upperCaseLetterInitHigherScore;

            // Lower case letter initial scores for
            // lower threshold number of characters (1 - char), and
            // higher threshold number of characters (2 - char).
            if (lowerCaseLetterCount == lowerCaseLetterInitLowerThreshold)
                score += lowerCaseLetterInitLowerScore;
            else if (lowerCaseLetterCount >= lowerCaseLetterInitHigherThreshold)
                score += lowerCaseLetterInitHigherScore;

            // Number initial scores for
            // lower threshold number of characters (1 - char), and
            // higher threshold number of characters (2 - char).
            if (numberCount == numberCountInitLowerThreshold)
                score += numberCountInitLowerScore;
            else if (numberCount >= numberCountInitHigherThreshold)
                score += numberCountInitHigherScore;

            // Special charcaters initial scores for
            // lower threshold number of characters (1 - char), and
            // higher threshold number of characters (2 - char).
            if (specialCount == specialCountInitLowerThreshold)
                score += specialCountInitLowerScore;
            else if (specialCount >= specialCountInitHigherThreshold)
                score += specialCountInitHigherScore;

            // Set the score for each upper case letter greater then
            // the higher threshold value, multiply by the higher
            // threshold score for each value over the highr threshold.
            if (upperCaseLetterCount > upperCaseLetterInitHigherThreshold)
                score += ((upperCaseLetterCount - upperCaseLetterInitHigherThreshold) * upperCaseLetterInitHigherThresholdScore);

            // Set the score for each lower case letter greater then
            // the higher threshold value, multiply by the higher
            // threshold score for each value over the highr threshold.
            if (lowerCaseLetterCount > lowerCaseLetterInitHigherThreshold)
                score += ((lowerCaseLetterCount - lowerCaseLetterInitHigherThreshold) * lowerCaseLetterInitHigherThresholdScore);

            // Set the score for each number greater then
            // the higher threshold value, multiply by the higher
            // threshold score for each value over the highr threshold.
            if (numberCount > numberCountInitHigherThreshold)
                score += ((numberCount - numberCountInitHigherThreshold) * numberCountInitHigherThresholdScore);

            // Set the score for each special character greater then
            // the higher threshold value, multiply by the higher
            // threshold score for each value over the highr threshold.
            if (specialCount > specialCountInitHigherThreshold)
                score += ((specialCount - specialCountInitHigherThreshold) * specialCountInitHigherThresholdScore);

            // Set the password strength level
            switch(score)
            {
                case 0: case 1: case 2: case 3: case 4: case 5: case 6: case 7: case 8: case 9: case 10:
                    passwordStrengthLevel = PasswordStrengthLevel.Poor;
                    break;
                case 11: case 12: case 13: case 14: case 15: case 16: case 17: case 18: case 19: case 20:
                    passwordStrengthLevel = PasswordStrengthLevel.Poor_to_Weak;
                    break;
                case 21: case 22: case 23: case 24: case 25: case 26: case 27: case 28: case 29: case 30:
                    passwordStrengthLevel = PasswordStrengthLevel.Weak;
                    break;
                case 31: case 32: case 33: case 34: case 35: case 36: case 37: case 38: case 39: case 40:
                    passwordStrengthLevel = PasswordStrengthLevel.Weak_to_Average;
                    break;
                case 41: case 42: case 43: case 44: case 45: case 46: case 47: case 48: case 49: case 50:
                    passwordStrengthLevel = PasswordStrengthLevel.Average;
                    break;
                case 51: case 52: case 53: case 54: case 55: case 56: case 57: case 58: case 59: case 60:
                    passwordStrengthLevel = PasswordStrengthLevel.Average_to_Good;
                    break;
                case 61: case 62: case 63: case 64: case 65: case 66: case 67: case 68: case 69: case 70:
                    passwordStrengthLevel = PasswordStrengthLevel.Good;
                    break;
                case 71: case 72: case 73: case 74: case 75: case 76: case 77: case 78: case 79: case 80:
                    passwordStrengthLevel = PasswordStrengthLevel.Good_to_Strong;
                    break;
                case 81: case 82: case 83: case 84: case 85: case 86: case 87: case 88: case 89: case 90:
                    passwordStrengthLevel = PasswordStrengthLevel.Strong;
                    break;
                case 91: case 92: case 93: case 94: case 95: case 96: case 97: case 98: case 99: case 100:
                    passwordStrengthLevel = PasswordStrengthLevel.Strong_to_Exellent;
                    break;
                default:
                    passwordStrengthLevel = PasswordStrengthLevel.Exellent;
                    break;
            }

            // Return the password strength level.
            return passwordStrengthLevel;
        }

        /// <summary>
        /// Validates any number.
        /// </summary>
        /// <param name="number">String, contains the value to validate</param>
        /// <returns>True if type is a match else false</returns>
        public static bool Number(string number)
        {
            // Validates numbers 3446.77888 or .865886 or 20
            // \. means match only one " . " but " . " means match anything once at that point.
            if (!Regex.IsMatch(number.Trim(), @"^([0-9]+\.[0-9]+)$") &&
                !Regex.IsMatch(number.Trim(), @"^(\.[0-9]+)$") &&
                !Regex.IsMatch(number.ToString().Trim(), @"^([0-9]+)$") &&
                !Regex.IsMatch(number.Trim(), @"^(\-[0-9]+\.[0-9]+)$") &&
                !Regex.IsMatch(number.Trim(), @"^(\-\.[0-9]+)$") &&
                !Regex.IsMatch(number.ToString().Trim(), @"^(\-[0-9]+)$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validates only integer types.
        /// </summary>
        /// <param name="integer">String, contains the value to validate</param>
        /// <returns>True if type is a match else false</returns>
        public static bool Integer(string integer)
        {
            if (!Regex.IsMatch(integer.ToString().Trim(), @"^([0-9]+)$") &&
                !Regex.IsMatch(integer.ToString().Trim(), @"^(\-[0-9]+)$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validates only double types
        /// </summary>
        /// <param name="number">String, contains the value to validate</param>
        /// <returns>True if type is a match else false</returns>
        public static bool PositiveNumber(string number)
        {
            // Validates numbers 3446.77888 or .865886 or 20
            // \. means match only one " . " but " . " means match anything once at that point.
            if (!Regex.IsMatch(number.Trim(), @"^([0-9]+\.[0-9]+)$") &&
                !Regex.IsMatch(number.Trim(), @"^(\.[0-9]+)$") &&
                !Regex.IsMatch(number.ToString().Trim(), @"^([0-9]+)$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validates only integer types.
        /// </summary>
        /// <param name="integer">String, contains the value to validate</param>
        /// <returns>True if type is a match else false</returns>
        public static bool PositiveInteger(string integer)
        {
            if (!Regex.IsMatch(integer.ToString().Trim(), @"^([0-9]+)$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validates only double types
        /// </summary>
        /// <param name="number">String, contains the value to validate</param>
        /// <returns>True if type is a match else false</returns>
        public static bool NegativeNumber(string number)
        {
            // Validates numbers 3446.77888 or .865886 or 20
            // \. means match only one " . " but " . " means match anything once at that point.
            if (!Regex.IsMatch(number.Trim(), @"^(\-[0-9]+\.[0-9]+)$") &&
                !Regex.IsMatch(number.Trim(), @"^(\-\.[0-9]+)$") &&
                !Regex.IsMatch(number.ToString().Trim(), @"^(\-[0-9]+)$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validates only integer types.
        /// </summary>
        /// <param name="integer">String, contains the value to validate</param>
        /// <returns>True if type is a match else false</returns>
        public static bool NegativeInteger(string integer)
        {
            if (!Regex.IsMatch(integer.ToString().Trim(), @"^(\-[0-9]+)$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// This function validates a date in the form dd-mm-yyyy.
        /// </summary>
        /// <param name="date">The date as a string to valid.</param>
        /// <returns>True if type is a match else false</returns>
        public static bool Date(string date)
        {
            if (!Regex.IsMatch(date.Trim(),
                @"^(0[1-9]|[12][0-9]|3[01])[-/.](0[1-9]|1[012])[-/.](19|20)\d\d$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validates email addresses, the simplest regex.
        /// </summary>
        /// <param name="emailAddress">String, contains the value to validate</param>
        /// <returns>True if type is a match else false</returns>
        public static bool EmailAddressSimple(string emailAddress)
        {
            // This regular expression will validate one email address.
            if (!Regex.IsMatch(emailAddress.ToString().Trim(),
                @"^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validates email addresses, using a standard regex.
        /// </summary>
        /// <param name="emailAddress">String, contains the value to validate</param>
        /// <returns>True if type is a match else false</returns>
        public static bool EmailAddressStandard(string emailAddress)
        {
            // This regular expression will validate one email address.
            if (!Regex.IsMatch(emailAddress.ToString().Trim(),
                @"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validates email addresses using the official RFC standard.
        /// </summary>
        /// <param name="emailAddress">String, contains the value to validate</param>
        /// <returns>True if type is a match else false</returns>
        public static bool EmailAddressOfficial(string emailAddress)
        {
            // This regular expression will validate one email address.
            if (!Regex.IsMatch(emailAddress.ToString().Trim(),
                @"^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validates only alfanumeric values.
        /// </summary>
        /// <param name="alfaNumeric">Contains the value to validate.</param>
        /// <returns>True if type is a match else false.</returns>
        public static bool IsAlfaNumeric(string alfaNumeric)
        {
            if (!Regex.IsMatch(alfaNumeric.Trim(), @"^([a-zA-Z0-9]+)$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validates the json data.
        /// </summary>
        /// <param name="jSon">The JSON formatted data to validate</param>
        /// <returns>True if type is a match else false.</returns>
        public static bool IsJSonData(string jSon)
        {
            if (!Regex.IsMatch(jSon.Trim(), @"[^,:{}\[\]0-9.\-+Eaeflnr-u \n\r\t]"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validates the v4 ip address
        /// </summary>
        /// <param name="ipAddressV4">The v4 ip address.</param>
        /// <returns>True if type is a match else false.</returns>
        public static bool IsIPAddressV4(string ipAddressV4)
        {
            if (!Regex.IsMatch(ipAddressV4.Trim(), @"^(\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b)$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validates the v6 ip address
        /// </summary>
        /// <param name="ipAddressV6">The v6 ip address.</param>
        /// <returns>True if type is a match else false.</returns>
        public static bool IsIPAddressV6(string ipAddressV6)
        {
            if (!Regex.IsMatch(ipAddressV6.Trim(), @"^\s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:)))(%.+)?\s*$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validate a password that must be
        /// between minimumPasswordLength and maximumPasswordLength length and contain
        /// at least one number, 
        /// at least one special character,
        /// at least one upper case character and 
        /// at least one lower case character.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <param name="minimumPasswordLength">The minimum password length; ( 4 or more).</param>
        /// <param name="maximumPasswordLength">The maximum password length.</param>
        /// <returns>True if type is a match else false.</returns>
        /// <![CDATA[Special Character (@#$%^&+=)]]>
        public static bool PasswordNumberUppercaseLowercaseSpecialchar(string password, int minimumPasswordLength = 4, int maximumPasswordLength = 500)
        {
            if (!Regex.IsMatch(password.ToString().Trim(),
                    @"^.*(?=.{" + minimumPasswordLength.ToString() + "," + maximumPasswordLength.ToString() + @"})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[*$-+?_&=!%{}/]).*$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validate a password that must be
        /// between minimumPasswordLength and maximumPasswordLength length and contain
        /// at least one number, 
        /// at least one upper case character and 
        /// at least one lower case character.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <param name="minimumPasswordLength">The minimum password length; ( 4 or more).</param>
        /// <param name="maximumPasswordLength">The maximum password length.</param>
        /// <returns>True if type is a match else false.</returns>
        public static bool PasswordNumberUppercaseLowercase(string password, int minimumPasswordLength = 4, int maximumPasswordLength = 500)
        {
            if (!Regex.IsMatch(password.ToString().Trim(),
                    @"^.*(?=.{" + minimumPasswordLength.ToString() + "," + maximumPasswordLength.ToString() + @"})(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validate a password that must be
        /// between minimumPasswordLength and maximumPasswordLength length and contain
        /// at least one upper case character and 
        /// at least one lower case character.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <param name="minimumPasswordLength">The minimum password length; ( 4 or more).</param>
        /// <param name="maximumPasswordLength">The maximum password length.</param>
        /// <returns>True if type is a match else false.</returns>
        public static bool PasswordUppercaseLowercase(string password, int minimumPasswordLength = 4, int maximumPasswordLength = 500)
        {
            if (!Regex.IsMatch(password.ToString().Trim(),
                    @"^.*(?=.{" + minimumPasswordLength.ToString() + "," + maximumPasswordLength.ToString() + @"})(?=.*[a-z])(?=.*[A-Z]).*$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validate a password that must be
        /// between minimumPasswordLength and maximumPasswordLength length and contain
        /// at least one upper case character.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <param name="minimumPasswordLength">The minimum password length; ( 4 or more).</param>
        /// <param name="maximumPasswordLength">The maximum password length.</param>
        /// <returns>True if type is a match else false.</returns>
        public static bool PasswordUppercase(string password, int minimumPasswordLength = 4, int maximumPasswordLength = 500)
        {
            if (!Regex.IsMatch(password.ToString().Trim(),
                    @"^.*(?=.{" + minimumPasswordLength.ToString() + "," + maximumPasswordLength.ToString() + @"})(?=.*[A-Z]).*$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validate a password that must be
        /// between minimumPasswordLength and maximumPasswordLength length and contain
        /// at least one lower case character.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <param name="minimumPasswordLength">The minimum password length; ( 4 or more).</param>
        /// <param name="maximumPasswordLength">The maximum password length.</param>
        /// <returns>True if type is a match else false.</returns>
        public static bool PasswordLowercase(string password, int minimumPasswordLength = 4, int maximumPasswordLength = 500)
        {
            if (!Regex.IsMatch(password.ToString().Trim(),
                    @"^.*(?=.{" + minimumPasswordLength.ToString() + "," + maximumPasswordLength.ToString() + @"})(?=.*[a-z]).*$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validate a password that must be
        /// between minimumPasswordLength and maximumPasswordLength length and contain
        /// starts with a letter and ends with a letter.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <param name="minimumPasswordLength">The minimum password length; ( 4 or more).</param>
        /// <param name="maximumPasswordLength">The maximum password length.</param>
        /// <returns>True if type is a match else false.</returns>
        public static bool PasswordStartsWithLetterEndsWithLetter(string password, int minimumPasswordLength = 4, int maximumPasswordLength = 500)
        {
            if (!Regex.IsMatch(password.ToString().Trim(),
                    @"^[A-Za-z]\w{" + minimumPasswordLength.ToString() + "," + maximumPasswordLength.ToString() + @"}[A-Za-z]$"))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Validate the xml file with the xsd file.
        /// </summary>
        /// <param name="xsdFile">The xsd file containing the schema.</param>
        /// <param name="xmlFile">The xml file to validate with the schema</param>
        /// <param name="errorMessage">The error is not validated.</param>
        /// <returns>True if valid else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static bool IsXmlValid(string xsdFile, string xmlFile, out string errorMessage)
        {
            // Xml text reader.
            XmlTextReader xsdReader = null;
            XmlReader xmlReader = null;
            bool xmlValidationError = true;
            string xmlErrorMessage = string.Empty;

            try
            {
                // Validate the filter xml file.
                // Load the xsd file into to xml reader.
                xsdReader = new XmlTextReader(xsdFile);

                // Read the xsd file add the xsd to the collection.
                XmlSchema schema = XmlSchema.Read(xsdReader, null);
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(schema);

                // Load the xml file into the x document
                // parse the file as a valid xml.
                xmlReader = XmlReader.Create(xmlFile);
                XDocument xDoc = System.Xml.Linq.XDocument.Load(xmlReader);
                xDoc.Validate(schemas,
                    (o, e) =>
                    {
                        xmlErrorMessage = e.Message;
                        xmlValidationError = false;
                    });

                // Assign the error message
                // and return the result.
                errorMessage = xmlErrorMessage;
                return xmlValidationError;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (xsdReader != null)
                    xsdReader.Close();

                if (xmlReader != null)
                    xmlReader.Close();
            }
        }

        /// <summary>
        /// Validate the xml string with the sxd string.
        /// </summary>
        /// <param name="xsdStream">The xsd memory stream containing the schema.</param>
        /// <param name="xmlStream">The xml memory stream to validate with the schema</param>
        /// <param name="errorMessage">The error is not validated.</param>
        /// <returns>True if valid else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static bool IsXmlValid(MemoryStream xsdStream, MemoryStream xmlStream, out string errorMessage)
        {
            // Xml text reader.
            XmlReader xmlReader = null;
            bool xmlValidationError = true;
            string xmlErrorMessage = string.Empty;

            try
            {
                // Read the xsd file add the xsd to the collection.
                XmlSchema schema = XmlSchema.Read(xsdStream, null);
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(schema);

                // Load the xml file into the x document
                // parse the file as a valid xml.
                xmlReader = XmlReader.Create(xmlStream);
                XDocument xDoc = System.Xml.Linq.XDocument.Load(xmlReader);
                xDoc.Validate(schemas,
                    (o, e) =>
                    {
                        xmlErrorMessage = e.Message;
                        xmlValidationError = false;
                    });

                // Assign the error message
                // and return the result.
                errorMessage = xmlErrorMessage;
                return xmlValidationError;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (xmlReader != null)
                    xmlReader.Close();
            }
        }

        /// <summary>
        /// Validate the xml string with the sxd string.
        /// </summary>
        /// <param name="xsdString">The xsd string containing the schema.</param>
        /// <param name="xmlString">The xml string to validate with the schema</param>
        /// <param name="errorMessage">The error is not validated.</param>
        /// <returns>True if valid else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static bool IsXmlStringValid(string xsdString, string xmlString, out string errorMessage)
        {
            // Xml text reader.
            MemoryStream xsdMemReader = null;
            MemoryStream xmlMemReader = null;
            XmlReader xmlReader = null;
            bool xmlValidationError = true;
            string xmlErrorMessage = string.Empty;

            try
            {
                // Validate the filter xml file.
                // Load the xsd file into to xml reader.
                xsdMemReader = new MemoryStream(Encoding.ASCII.GetBytes(xsdString));
                xmlMemReader = new MemoryStream(Encoding.ASCII.GetBytes(xmlString));

                // Read the xsd file add the xsd to the collection.
                XmlSchema schema = XmlSchema.Read(xsdMemReader, null);
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(schema);

                // Load the xml file into the x document
                // parse the file as a valid xml.
                xmlReader = XmlReader.Create(xmlMemReader);
                XDocument xDoc = System.Xml.Linq.XDocument.Load(xmlReader);
                xDoc.Validate(schemas,
                    (o, e) =>
                    {
                        xmlErrorMessage = e.Message;
                        xmlValidationError = false;
                    });

                // Assign the error message
                // and return the result.
                errorMessage = xmlErrorMessage;
                return xmlValidationError;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (xsdMemReader != null)
                    xsdMemReader.Close();

                if (xmlMemReader != null)
                    xmlMemReader.Close();

                if (xmlReader != null)
                    xmlReader.Close();
            }
        }

        /// <summary>
        /// Validate the xml file with the xsd file.
        /// </summary>
        /// <param name="xsdString">The xsd string containing the schema.</param>
        /// <param name="xmlFile">The xml file to validate with the schema</param>
        /// <param name="errorMessage">The error is not validated.</param>
        /// <returns>True if valid else false.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static bool IsXmlValidEx(string xsdString, string xmlFile, out string errorMessage)
        {
            // Xml text reader.
            MemoryStream xsdMemReader = null;
            XmlReader xmlReader = null;
            bool xmlValidationError = true;
            string xmlErrorMessage = string.Empty;

            try
            {
                // Validate the filter xml file.
                // Load the xsd file into to xml reader.
                xsdMemReader = new MemoryStream(Encoding.ASCII.GetBytes(xsdString));

                // Read the xsd file add the xsd to the collection.
                XmlSchema schema = XmlSchema.Read(xsdMemReader, null);
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(schema);

                // Load the xml file into the x document
                // parse the file as a valid xml.
                xmlReader = XmlReader.Create(xmlFile);
                XDocument xDoc = System.Xml.Linq.XDocument.Load(xmlReader);
                xDoc.Validate(schemas,
                    (o, e) =>
                    {
                        xmlErrorMessage = e.Message;
                        xmlValidationError = false;
                    });

                // Assign the error message
                // and return the result.
                errorMessage = xmlErrorMessage;
                return xmlValidationError;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (xsdMemReader != null)
                    xsdMemReader.Close();

                if (xmlReader != null)
                    xmlReader.Close();
            }
        }

        /// <summary>
        /// Entropy (Shannon entropy H(X) = - SUM(p(x) * log(p(x))) i = 1 to n) is a measure of the uncertainty 
        /// in a random variable (Order and Disorder), which quantifies the expected value of the information 
        /// contained in a message.
        /// </summary>
        /// <param name="phrase">The phrase to analyse.</param>
        /// <param name="bitRate">The entropy bit rate.</param>
        /// <param name="metricRate">The randomness of the phase.</param>
        /// <returns>The measure of uncertainty H(X).</returns>
        /// <remarks>
        /// Shannon entropy is the average unpredictability in a random variable, which 
        /// is equivalent to its information content. Shannon entropy provides an absolute limit on the best possible 
        /// lossless encoding or compression of any communication, assuming that [5] the communication may be represented 
        /// as a sequence of independent and identically distributed random variables. Shannon's source coding 
        /// theorem shows that, in the limit, the average length of the shortest possible representation to encode 
        /// the messages in a given alphabet is their entropy divided by the logarithm of the number of symbols in 
        /// the target alphabet.
        /// </remarks>
        public static double EntropyShannon(string phrase, out double bitRate, out double metricRate)
        {
            // Make sure a phrase has been passed.
            if (String.IsNullOrEmpty(phrase)) throw new ArgumentNullException("phrase");

            double entropy = 0.0;

            // Get the target character collection.
            char[] targetChars = phrase.ToCharArray();
            int targetCharCount = targetChars.Length;
            int[] uniqueCharAmount = null;

            // Sort the phrase.
            char[] targetCharsSorted = Nequeo.Invention.ArrayComparer.Sort<char>(targetChars, false);

            // Get the list of unique characters and
            // the amount of each unique character.
            char[] targetUniqueChars = Nequeo.Invention.ArrayComparer.Uniqueness<char>(targetCharsSorted, ref uniqueCharAmount);

            // For each probability of each unique character.
            for (int i = 0; i < uniqueCharAmount.Length; i++)
            {
                // Calculate the probability of the current
                // charcter amount.
                double probability = (double)uniqueCharAmount[i] / (double)targetCharCount;

                // Calculate the probability sum.
                entropy += (probability * System.Math.Log(probability, 2.0));
            }

            // Calculate the entropy H(X).
            entropy = (-1) * entropy;

            // The randomness of the phase.
            metricRate = entropy / (double)targetCharCount;

            // The entropy bit rate.
            bitRate = (double)targetCharCount * entropy;

            // Return the entropy.
            return entropy;
        }

        /// <summary>
        /// Calculates the time (years) a phrase (password) would take to crack.
        /// </summary>
        /// <param name="phrase">The password or phrase to examine.</param>
        /// <param name="combinations">The number of possible combinations.</param>
        /// <param name="calculationsPerSecond">The calculations per second a system can perform.</param>
        /// <returns>The time (years) it would take to crack the phrase.</returns>
        public static double PasswordCrackTime(string phrase, out double combinations, double calculationsPerSecond = 40000000000000000.00)
        {
            double possibleCombinations = 1;
            int numberOfCombinations = 0;
            int numberOfChars = phrase.Length;
            double numberOfSecondsInAYear = (double)(60 * 60 * 24 * 365);

            bool hasLowerCaseLetters = false;
            bool hasUpperCaseLetters = false;
            bool hasNumbers = false;
            bool hasSpecialChars = false;

            int combinationsOfLowerCaseLetters = 26;
            int combinationsOfUpperCaseLetters = 26;
            int combinationsOfNumbers = 10;
            int combinationsOfSpecialChars = 13;

            // For each char calculate the number
            // of combinations.
            foreach (char item in phrase)
            {
                // If lower case char
                if (Char.IsLower(item))
                {
                    // If lower case char combinations
                    // have not been included.
                    if (!hasLowerCaseLetters)
                    {
                        // Add to the possible number of combinations
                        hasLowerCaseLetters = true;
                        numberOfCombinations += combinationsOfLowerCaseLetters;
                    }
                    continue;
                }
                else if (Char.IsUpper(item))
                {
                    if (!hasUpperCaseLetters)
                    {
                        hasUpperCaseLetters = true;
                        numberOfCombinations += combinationsOfUpperCaseLetters;
                    }
                    continue;
                }
                else if (Char.IsDigit(item))
                {
                    if (!hasNumbers)
                    {
                        hasNumbers = true;
                        numberOfCombinations += combinationsOfNumbers;
                    }
                    continue;
                }
                else
                {
                    if (!hasSpecialChars)
                    {
                        hasSpecialChars = true;
                        numberOfCombinations += combinationsOfSpecialChars;
                    }
                    continue;
                }
            }

            // For each char calculate the number
            // of possible combinations.
            foreach (char item in phrase)
            {
                possibleCombinations *= (double)numberOfCombinations;
            }

            // The number of combinations
            combinations = possibleCombinations;

            // Return the number of years the phrase
            // would take to be cracked.
            return (double)possibleCombinations / (calculationsPerSecond * numberOfSecondsInAYear);
        }
    }
}
