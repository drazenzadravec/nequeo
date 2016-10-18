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
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Globalization;

using Nequeo.Invention;

namespace Nequeo.Extension
{
    /// <summary>
    /// Class that extends the System.String type.
    /// </summary>
    public static class StringExtensions
    {
        private static readonly Regex NameExpression = new Regex("([A-Z]+(?=$|[A-Z][a-z])|[A-Z]?[a-z]+)", RegexOptions.Compiled);

        #region Public Methods
        /// <summary>
        /// Is one of.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <param name="candidates">The values to compare.</param>
        /// <returns>True if the s exists in the candiadtes.</returns>
        internal static bool IsOneOf(string s, params string[] candidates)
        {
            foreach (string candidate in candidates)
            {
                if (s == candidate)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Get all words split by whitspace.
        /// </summary>
        /// <param name="stringValue">The current string value.</param>
        /// <returns>The array of words.</returns>
        public static string[] Words(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            string[] words = stringValue.Split(new string[] { " ", "\t", "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            return words;
        }

        /// <summary>
        /// Get all unique words split by whitspace.
        /// </summary>
        /// <param name="stringValue">The current string value.</param>
        /// <returns>The array of words.</returns>
        public static string[] UniqueWords(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            List<string> uniqueWords = new List<string>();
            string[] words = stringValue.Split(new string[] { " ", "\t", "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length > 0)
            {
                // Get the to lower comparer.
                Nequeo.Invention.ToLowerComparer comparer = new ToLowerComparer();

                // Add the first.
                uniqueWords.Add(words[0]);

                // For each word.
                for (int i = 1; i < words.Length; i++)
                {
                    // If the word does not exist.
                    if (!uniqueWords.Contains(words[i], comparer))
                    {
                        // Add the word.
                        uniqueWords.Add(words[i]);
                    }
                }
            }

            // Return the words.
            return uniqueWords.ToArray();
        }

        /// <summary>
        /// Remove the punctuation from the string.
        /// </summary>
        /// <param name="stringValue">The current string value.</param>
        /// <returns>The string without punctuation.</returns>
        public static string RemovePunctuation(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            return stringValue.Replace(".", "").Replace(" ", "").
                Replace(",", "").Replace(";", "").Replace(":", "").
                Replace("$", "").Replace("/", "").Replace("\\", "").
                Replace("?", "").Replace("*", "").Replace("<", "").
                Replace(">", "").Replace("%", "").Replace("&", "").
                Replace("=", "").Replace("\"", "").Replace("[", "").
                Replace("]", "").Replace("{", "").Replace("}", "").
                Replace("|", "").Replace("!", "").Replace("(", "").
                Replace(")", "");
        }

        /// <summary>
        /// Remove the punctuation from the start of the string.
        /// </summary>
        /// <param name="stringValue">The current string value.</param>
        /// <returns>The string without punctuation.</returns>
        public static string RemovePunctuationFromStart(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            return stringValue.TrimStart(
                new char[] { '.', ' ', ',', ';', ':', '$', '/', '\\', '?', '*', '<', '>', '%', '&', '=', '"', '[', ']', '{', '}', '|', '!', '(', ')' });
        }

        /// <summary>
        /// Remove the punctuation from the end of the string.
        /// </summary>
        /// <param name="stringValue">The current string value.</param>
        /// <returns>The string without punctuation.</returns>
        public static string RemovePunctuationFromEnd(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            return stringValue.TrimEnd(
                new char[] { '.', ' ', ',', ';', ':', '$', '/', '\\', '?', '*', '<', '>', '%', '&', '=', '"', '[', ']', '{', '}', '|', '!', '(', ')' });
        }

        /// <summary>
        /// Remove the punctuation from the start and end of the string.
        /// </summary>
        /// <param name="stringValue">The current string value.</param>
        /// <returns>The string without punctuation.</returns>
        public static string RemovePunctuationFromStartAndEnd(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            return stringValue.Trim(
                new char[] { '.', ' ', ',', ';', ':', '$', '/', '\\', '?', '*', '<', '>', '%', '&', '=', '"', '[', ']', '{', '}', '|', '!', '(', ')' });
        }

        /// <summary>
        /// Trim the string array.
        /// </summary>
        /// <param name="array">The current array.</param>
        /// <returns>The trimmed array.</returns>
        public static string[] Trim(this string[] array)
        {
            // If the string is null or empty.
            if (array == null)
                throw new System.ArgumentNullException();

            List<string> col = new List<string>();
            for (int i = 0; i < array.Length; i++)
            {
                col.Add(array[i].Trim());
            }
            return col.ToArray();
        }

        /// <summary>
        /// Get the int array from the string array.
        /// </summary>
        /// <param name="array">The current array.</param>
        /// <returns>The int array.</returns>
        public static int[] ToIntArray(this string[] array)
        {
            // If the string is null or empty.
            if (array == null)
                throw new System.ArgumentNullException();

            List<int> col = new List<int>();
            for (int i = 0; i < array.Length; i++)
            {
                col.Add(Int32.Parse(array[i].Trim()));
            }
            return col.ToArray();
        }

        /// <summary>
        /// Get the long array from the string array.
        /// </summary>
        /// <param name="array">The current array.</param>
        /// <returns>The long array.</returns>
        public static long[] ToLongArray(this string[] array)
        {
            // If the string is null or empty.
            if (array == null)
                throw new System.ArgumentNullException();

            List<long> col = new List<long>();
            for (int i = 0; i < array.Length; i++)
            {
                col.Add(Int64.Parse(array[i].Trim()));
            }
            return col.ToArray();
        }

        /// <summary>
        /// Get the bool array from the string array.
        /// </summary>
        /// <param name="array">The current array.</param>
        /// <returns>The bool array.</returns>
        public static bool[] ToBoolArray(this string[] array)
        {
            // If the string is null or empty.
            if (array == null)
                throw new System.ArgumentNullException();

            List<bool> col = new List<bool>();
            for (int i = 0; i < array.Length; i++)
            {
                col.Add(Boolean.Parse(array[i].Trim()));
            }
            return col.ToArray();
        }

        /// <summary>
        /// Get the double array from the string array.
        /// </summary>
        /// <param name="array">The current array.</param>
        /// <returns>The double array.</returns>
        public static double[] ToDoubleArray(this string[] array)
        {
            // If the string is null or empty.
            if (array == null)
                throw new System.ArgumentNullException();

            List<double> col = new List<double>();
            for (int i = 0; i < array.Length; i++)
            {
                col.Add(Double.Parse(array[i].Trim()));
            }
            return col.ToArray();
        }

        /// <summary>
        /// Get the decimal array from the string array.
        /// </summary>
        /// <param name="array">The current array.</param>
        /// <returns>The decimal array.</returns>
        public static decimal[] ToDecimalArray(this string[] array)
        {
            // If the string is null or empty.
            if (array == null)
                throw new System.ArgumentNullException();

            List<decimal> col = new List<decimal>();
            for (int i = 0; i < array.Length; i++)
            {
                col.Add(Decimal.Parse(array[i].Trim()));
            }
            return col.ToArray();
        }

        /// <summary>
        /// Split the string using a csv delimeter.
        /// </summary>
        /// <param name="stringValue">The current string.</param>
        /// <returns>The collection of items.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null or empty</exception>
        public static string[] SplitCsv(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            return Regex.Split(stringValue, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
        }

        /// <summary>
        /// Split the string using a pipe delimeter.
        /// </summary>
        /// <param name="stringValue">The current string.</param>
        /// <returns>The collection of items.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null or empty</exception>
        public static string[] SplitPipe(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            return stringValue.Split('|');
        }

        /// <summary>
        /// Split the string using a tab delimeter.
        /// </summary>
        /// <param name="stringValue">The current string.</param>
        /// <returns>The collection of items.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null or empty</exception>
        public static string[] SplitTab(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            return stringValue.Split('\t');
        }

        /// <summary>
        /// Split the string according to the delimiter.
        /// </summary>
        /// <param name="stringValue">The current string.</param>
        /// <param name="delimiter">The delimeter to split against.</param>
        /// <returns>The collection of items.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null or empty</exception>
        public static string[] Split(this String stringValue, Nequeo.Custom.SplitDelimiterName delimiter)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            switch (delimiter)
            {
                case Custom.SplitDelimiterName.Csv:
                    return Regex.Split(stringValue, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                case Custom.SplitDelimiterName.Pipe:
                    return stringValue.Split('|');

                case Custom.SplitDelimiterName.Tab:
                    return stringValue.Split('\t');

                case Custom.SplitDelimiterName.At:
                    return stringValue.Split('@');

                case Custom.SplitDelimiterName.BackSlash:
                    return stringValue.Split('/');

                case Custom.SplitDelimiterName.Dot:
                    return stringValue.Split('.');

                case Custom.SplitDelimiterName.Equal:
                    return stringValue.Split('=');

                case Custom.SplitDelimiterName.ForwardSlash:
                    return stringValue.Split('\\');

                case Custom.SplitDelimiterName.Comma:
                    return stringValue.Split(',');

                case Custom.SplitDelimiterName.Return:
                    return stringValue.Split('\r');

                case Custom.SplitDelimiterName.LineFeed:
                    return stringValue.Split('\n');

                case Custom.SplitDelimiterName.ReturnLineFeed:
                    return stringValue.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                case Custom.SplitDelimiterName.Colon:
                    return stringValue.Split(':');

                case Custom.SplitDelimiterName.SemiColon:
                    return stringValue.Split(';');

                default:
                    return stringValue.Split(',');
            }
        }

        /// <summary>
        /// Converts the word from singular to plural.
        /// </summary>
        /// <param name="stringValue">The current string.</param>
        /// <returns>The converted word as plural.</returns>
        public static string Plural(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            string newValue = stringValue;

            if (newValue.EndsWith("x", StringComparison.InvariantCultureIgnoreCase)
                || newValue.EndsWith("ch", StringComparison.InvariantCultureIgnoreCase)
                || newValue.EndsWith("ss", StringComparison.InvariantCultureIgnoreCase))
            {
                return newValue + "es";
            }
            else if (newValue.EndsWith("y", StringComparison.InvariantCultureIgnoreCase))
            {
                return newValue.Substring(0, newValue.Length - 1) + "ies";
            }
            else if (!newValue.EndsWith("s"))
            {
                return newValue + "s";
            }
            return newValue;
        }

        /// <summary>
        /// Gets the word count of the current string.
        /// </summary>
        /// <param name="stringValue">The current string.</param>
        /// <returns>The word count.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null or empty</exception>
        public static int WordCount(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            // Get the word count.
            return stringValue.Split(new char[] { ' ', '.', '?', '!', ',' },
                StringSplitOptions.RemoveEmptyEntries).Length;
        }

        /// <summary>
        /// Converts the first letter of each word found to upper case
        /// and all other letters in each word found to lower case.
        /// </summary>
        /// <param name="stringValue">The current string.</param>
        /// <returns>The newly formatted string.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null or empty</exception>
        public static String ToUpperFirstToLowerRest(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            // Get each word.
            string[] words = stringValue.Split(new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            // Iteration item.
            int i = 0;
            string[] itemWords = new string[words.Count()];

            // For each word found iterate through
            // the array.
            foreach (string word in words)
            {
                // Convert the first letter to upper and
                // all other letters to lower.
                string firstLetterToUpper = word.Substring(0, 1).ToUpper().Trim();
                string otherLettersToLower = word.Substring(1).ToLower().Trim();

                // Combine the word.
                itemWords[i++] = String.Format("{0}{1}", firstLetterToUpper, otherLettersToLower);
            }

            // Join back each word with the new format.
            string newString = string.Join(" ", itemWords);
            return newString;
        }

        /// <summary>
        /// Converts the first letter of each word found to upper case.
        /// </summary>
        /// <param name="stringValue">The current string.</param>
        /// <returns>The newly formatted string.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null or empty</exception>
        public static String ToUpperFirstLetterInEachWord(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            // Get each word.
            string[] words = stringValue.Split(new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            // Iteration item.
            int i = 0;
            string[] itemWords = new string[words.Count()];

            // For each word found iterate through
            // the array.
            foreach (string word in words)
            {
                // Convert the first letter to upper.
                string firstLetterToUpper = word.Substring(0, 1).ToUpper().Trim();
                string otherLetters = word.Substring(1).Trim();

                // Combine the word.
                itemWords[i++] = String.Format("{0}{1}", firstLetterToUpper, otherLetters);
            }

            // Join back each word with the new format.
            string newString = string.Join(" ", itemWords);
            return newString;
        }

        /// <summary>
        /// Converts to first letter of the string to upper case.
        /// </summary>
        /// <param name="stringValue">The current string.</param>
        /// <returns>The newly formatted string.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null or empty</exception>
        public static String ToUpperFirstLetter(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            // Turn the first letter into upper
            // and all others remain.
            string firstLetterToUpper = stringValue.Substring(0, 1).ToUpper();
            string allOtherLetters = stringValue.Substring(1);

            // Return the new string.
            return String.Format("{0}{1}", firstLetterToUpper, allOtherLetters);
        }

        /// <summary>
        /// Converts the first letter of each word found to lower case.
        /// </summary>
        /// <param name="stringValue">The current string.</param>
        /// <returns>The newly formatted string.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null or empty</exception>
        public static String ToLowerFirstLetterInEachWord(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            // Get each word.
            string[] words = stringValue.Split(new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            // Iteration item.
            int i = 0;
            string[] itemWords = new string[words.Count()];

            // For each word found iterate through
            // the array.
            foreach (string word in words)
            {
                // Convert the first letter to upper and
                // all other letters to lower.
                string firstLetterToLower = word.Substring(0, 1).ToLower().Trim();
                string otherLetters = word.Trim();

                // Combine the word.
                itemWords[i++] = String.Format("{0}{1}", firstLetterToLower, otherLetters);
            }

            // Join back each word with the new format.
            string newString = string.Join(" ", itemWords);
            return newString;
        }

        /// <summary>
        /// Converts to first letter of the string to lower case.
        /// </summary>
        /// <param name="stringValue">The current string.</param>
        /// <returns>The newly formatted string.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null or empty</exception>
        public static String ToLowerFirstLetter(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            // Turn the first letter into upper
            // and all others remain.
            string firstLetterToUpper = stringValue.Substring(0, 1).ToLower();
            string allOtherLetters = stringValue.Substring(1);

            // Return the new string.
            return String.Format("{0}{1}", firstLetterToUpper, allOtherLetters);
        }

        /// <summary>
        /// Removes the key operands from the string.
        /// </summary>
        /// <param name="stringValue">The current string.</param>
        /// <returns>The newly formatted string.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null or empty</exception>
        public static String ReplaceKeyOperands(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            return stringValue.Replace(".", "").Replace(" ", "").
                Replace(",", "").Replace(";", "").Replace(":", "").
                Replace("-", "").Replace("$", "").Replace("/", "").
                Replace("\\", "").Replace("?", "").Replace("+", "").
                Replace("*", "").Replace("<", "").Replace(">", "").
                Replace("#", "").Replace("@", "").Replace("%", "").
                Replace("&", "").Replace("=", "").Replace("\"", "").
                Replace("'", "").Replace("[", "").Replace("]", "").
                Replace("{", "").Replace("}", "").Replace("|", "").
                Replace("!", "").Replace("(", "").Replace(")", "");
        }

        /// <summary>
        /// Replaces the string if it is a number to a string with 'N' at the begining of the number.
        /// </summary>
        /// <param name="stringValue">The current string.</param>
        /// <returns>The newly formatted string.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null or empty</exception>
        public static String ReplaceNumbers(this String stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            return (Validation.Number(stringValue)) ? "_" + stringValue + "_" : stringValue;
        }

        /// <summary>
        /// Get the Utc offset hours in the string.
        /// </summary>
        /// <param name="stringValue">The current string.</param>
        /// <returns>The utc offset hours.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null or empty</exception>
        public static Int32 GetUtcOffsetHours(this string stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            if (stringValue.Length > 1)
            {
                string internalString =
                    stringValue.
                    Trim().
                    TrimStart('+').
                    TrimStart('-').
                    Trim().
                    Substring(0, 2).
                    TrimStart('0');

                if (internalString.Length < 1)
                    return 0;
                else
                    return Convert.ToInt32(
                                        stringValue.
                                        Trim().
                                        TrimStart('+').
                                        TrimStart('-').
                                        Trim().
                                        Substring(0, 2).
                                        TrimStart('0'));
            }
            else
                return Convert.ToInt32(
                    stringValue.
                    Trim().
                    TrimStart('+').
                    TrimStart('-').
                    Trim().
                    Substring(0, 1));
        }

        /// <summary>
        /// Get the Utc offset minutes in the string.
        /// </summary>
        /// <param name="stringValue">The current string.</param>
        /// <returns>The utc offset minutes.</returns>
        /// <exception cref="System.ArgumentNullException">Source object can not be null or empty</exception>
        public static Int32 GetUtcOffsetMinutes(this string stringValue)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(stringValue))
                throw new System.ArgumentNullException();

            // Make sure the correct number of characters are passed.
            if (stringValue.Length < 3)
                return 0;

            if (stringValue.Length > 3)
            {
                string internalString =
                    stringValue.
                    Trim().
                    TrimStart('+').
                    TrimStart('-').
                    Trim().
                    Substring(2, 2).
                    TrimStart('0');

                if (internalString.Length < 1)
                    return 0;
                else
                    return Convert.ToInt32(
                                        stringValue.
                                        Trim().
                                        TrimStart('+').
                                        TrimStart('-').
                                        Trim().
                                        Substring(2, 2).
                                        TrimStart('0'));
            }
            else
                return Convert.ToInt32(
                    stringValue.
                    Trim().
                    TrimStart('+').
                    TrimStart('-').
                    Trim().
                    Substring(2, 1));
        }

        /// <summary>
        /// Replaces the format item in a specified System.String with the text equivalent of the value of a 
        /// corresponding System.Object instance in a specified array.
        /// </summary>
        /// <param name="instance">A string to format.</param>
        /// <param name="args">An System.Object array containing zero or more objects to format.</param>
        /// <returns>A copy of format in which the format items have been replaced by the System.String 
        /// equivalent of the corresponding instances of System.Object in args.</returns>
        public static string FormatWith(this string instance, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, instance, args);
        }

        /// <summary>
        /// Is the value empty or null.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>True if a value exists; else false.</returns>
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Determines whether this instance and another specified System.String object have the same value.
        /// </summary>
        /// <param name="instance">The string to check equality.</param>
        /// <param name="comparing">The comparing with string.</param>
        /// <returns>
        /// <c>true</c> if the value of the comparing parameter is the same as this string; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCaseSensitiveEqual(this string instance, string comparing)
        {
            return string.CompareOrdinal(instance, comparing) == 0;
        }

        /// <summary>
        /// Determines whether this instance and another specified System.String object have the same value.
        /// </summary>
        /// <param name="instance">The string to check equality.</param>
        /// <param name="comparing">The comparing with string.</param>
        /// <returns>
        /// <c>true</c> if the value of the comparing parameter is the same as this string; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCaseInsensitiveEqual(this string instance, string comparing)
        {
            return string.Compare(instance, comparing, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Determines whether this instance is null or empty string.
        /// </summary>
        /// <param name="instance">The string to check its value.</param>
        /// <returns>
        /// <c>true</c> if the value is null or empty string; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this string instance)
        {
            return string.IsNullOrEmpty(instance);
        }

        /// <summary>
        /// Compresses the specified instance (using Gzip algorithm).
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The Base64 compressed value.</returns>
        public static string Compress(this string instance)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(instance))
                throw new System.ArgumentNullException();

            byte[] binary = Encoding.Default.GetBytes(instance);
            byte[] compressed;

            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress))
                {
                    zip.Write(binary, 0, binary.Length);
                    compressed = ms.ToArray();
                }
            }

            byte[] compressedWithLength = new byte[compressed.Length + 4];

            Buffer.BlockCopy(compressed, 0, compressedWithLength, 4, compressed.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(binary.Length), 0, compressedWithLength, 0, 4);

            return Convert.ToBase64String(compressedWithLength);
        }

        /// <summary>
        /// Decompresses the specified instance (using Gzip algorithm).
        /// </summary>
        /// <param name="instance">The Base64 compressed instance.</param>
        /// <returns>The decompressed value.</returns>
        public static string Decompress(this string instance)
        {
            // If the string is null or empty.
            if (String.IsNullOrEmpty(instance))
                throw new System.ArgumentNullException();

            byte[] compressed = Convert.FromBase64String(instance);
            byte[] binary;

            using (MemoryStream ms = new MemoryStream())
            {
                int length = BitConverter.ToInt32(compressed, 0);
                ms.Write(compressed, 4, compressed.Length - 4);

                binary = new byte[length];

                ms.Seek(0, SeekOrigin.Begin);

                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(binary, 0, binary.Length);
                }
            }

            return Encoding.UTF8.GetString(binary);
        }

        /// <summary>
        /// Get the enum value from the instance.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="instance">The string instance.</param>
        /// <param name="defaultValue">The default enum type.</param>
        /// <returns>The enum type.</returns>
        public static T ToEnum<T>(this string instance, T defaultValue) where T : IComparable, IFormattable
        {
            T convertedValue = defaultValue;

            if (!string.IsNullOrEmpty(instance))
            {
                try
                {
                    convertedValue = (T)Enum.Parse(typeof(T), instance.Trim(), true);
                }
                catch (ArgumentException)
                {
                }
            }

            return convertedValue;
        }

        /// <summary>
        /// Convert first letter to upper case of each word.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <returns>The title.</returns>
        public static string AsTitle(this string value)
        {
            int lastIndex = value.LastIndexOf(".", StringComparison.Ordinal);

            if (lastIndex > -1)
            {
                value = value.Substring(lastIndex + 1);
            }

            return value.SplitPascalCase();
        }

        /// <summary>
        /// Split pascal case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The pascal case.</returns>
        public static string SplitPascalCase(this string value)
        {
            return NameExpression.Replace(value, " $1").Trim();
        }

        /// <summary>
        /// Convert to string.
        /// </summary>
        /// <param name="value">The instance.</param>
        /// <param name="bs">The byte array.</param>
        /// <returns>The string from the byte array.</returns>
        public static string FromByteArray(this string value, byte[] bs)
        {
            char[] cs = new char[bs.Length];
            for (int i = 0; i < cs.Length; ++i)
            {
                cs[i] = Convert.ToChar(bs[i]);
            }
            return new string(cs);
        }

        /// <summary>
        /// From byte array to string.
        /// </summary>
        /// <param name="bs">The byte array.</param>
        /// <returns>The string.</returns>
        public static string FromByteArray(byte[] bs)
        {
            char[] cs = new char[bs.Length];
            for (int i = 0; i < cs.Length; ++i)
            {
                cs[i] = Convert.ToChar(bs[i]);
            }
            return new string(cs);
        }

        /// <summary>
        /// Convert the char array to byte array.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <param name="cs">The char array.</param>
        /// <returns>The byte array.</returns>
        public static byte[] ToByteArray(this string value, char[] cs)
        {
            byte[] bs = new byte[cs.Length];
            for (int i = 0; i < bs.Length; ++i)
            {
                bs[i] = Convert.ToByte(cs[i]);
            }
            return bs;
        }

        /// <summary>
        /// Convert to byte array.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>The byte array.</returns>
        public static byte[] ToByteArray(this string value)
        {
            byte[] bs = new byte[value.Length];
            for (int i = 0; i < bs.Length; ++i)
            {
                bs[i] = Convert.ToByte(value[i]);
            }
            return bs;
        }

        /// <summary>
        /// Convert to ASCII from byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bytes">The byte array.</param>
        /// <returns>The ASCII string.</returns>
        public static string FromAsciiByteArray(this string value, byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Convert to ASCII from byte array.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <returns>The ASCII string.</returns>
        public static string FromAsciiByteArray(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Convert to ASCII byte array.
        /// </summary>
        /// <param name="value">The current string.</param>
        /// <param name="cs">The array of chars to convert.</param>
        /// <returns>The byte array.</returns>
        public static byte[] ToAsciiByteArray(this string value, char[] cs)
        {
            return Encoding.ASCII.GetBytes(cs);
        }

        /// <summary>
        /// Convert to ASCII byte array.
        /// </summary>
        /// <param name="value">The chars to convert.</param>
        /// <returns>The byte array.</returns>
        public static byte[] ToAsciiByteArray(this string value)
        {
            return Encoding.ASCII.GetBytes(value);
        }

        /// <summary>
        /// Convert from UTF8 byte array to string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bytes">The UTF8 byte array.</param>
        /// <returns>The string.</returns>
        public static string FromUtf8ByteArray(this string value, byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Convert to UTF8 byte array.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <param name="cs">The char array.</param>
        /// <returns>The byte array.</returns>
        public static byte[] ToUtf8ByteArray(this string value, char[] cs)
        {
            return Encoding.UTF8.GetBytes(cs);
        }

        /// <summary>
        /// Convert to UTF8 byte array.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <returns>The byte array.</returns>
        public static byte[] ToUtf8ByteArray(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        /// <summary>
        /// Convert from Default byte array to string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="bytes">The Default byte array.</param>
        /// <returns>The string.</returns>
        public static string FromDefaultByteArray(this string value, byte[] bytes)
        {
            return Encoding.Default.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Convert to Default byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="cs">The char array.</param>
        /// <returns>The byte array.</returns>
        public static byte[] ToDefaultByteArray(this string value, char[] cs)
        {
            return Encoding.Default.GetBytes(cs);
        }

        /// <summary>
        /// Convert to Default byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The byte array.</returns>
        public static byte[] ToDefaultByteArray(this string value)
        {
            return Encoding.Default.GetBytes(value);
        }
        #endregion
    }
}
