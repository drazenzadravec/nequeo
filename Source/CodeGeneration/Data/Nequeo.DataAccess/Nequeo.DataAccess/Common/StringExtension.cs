using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nequeo.CustomTool.CodeGenerator.Common
{
    /// <summary>
    /// Class that extends the System.String type.
    /// </summary>
    public static class StringExtensions
    {
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
                Replace("{", "").Replace("}", "").Replace("|", "");
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

            return (LinqToDataTypes.NumberValidator(stringValue)) ? "_" + stringValue + "_" : stringValue;
        }
    }
}
