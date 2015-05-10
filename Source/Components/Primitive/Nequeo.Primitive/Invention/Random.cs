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
using System.Net.Mail;
using System.Net;
using System.ComponentModel.Composition;

using Nequeo.ComponentModel.Composition;

namespace Nequeo.Invention
{
    /// <summary>
    /// Random password generator class.
    /// </summary>
    /// <remarks>Includes special characters.</remarks>
    [Export(typeof(IRandomGenerator))]
    [ContentMetadata(Name = "PasswordGenerator")]
    public class PasswordGenerator : IRandomGenerator
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly PasswordGenerator Instance = new PasswordGenerator();

        /// <summary>
        /// Static constructor
        /// </summary>
        static PasswordGenerator() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public PasswordGenerator()
        {
        }
        #endregion

        #region Private Constants
        private const string applicationName = "Nequeo";
        private const string eventNamespace = "Nequeo.Invention";
        #endregion

        #region Private Fields
        // Define default min and max password lengths.
        private int DEFAULT_MIN_PASSWORD_LENGTH = 8;
        private int DEFAULT_MAX_PASSWORD_LENGTH = 10;

        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
        private string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        private string PASSWORD_CHARS_NUMERIC = "0123456789";
        private string PASSWORD_CHARS_SPECIAL = "*$-+?_&=!%{}/";
        #endregion

        #region Private Random Methods
        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <param name="minLength">Minimum password length.</param>
        /// <param name="maxLength">Maximum password length.</param>
        /// <returns>Randomly generated password.</returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        private string RandomEx(int minLength, int maxLength)
        {
            try
            {
                // Make sure that input parameters are valid.
                if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                    return null;

                // Create a local array containing supported password characters
                // grouped by types. You can remove character groups from this
                // array, but doing so will weaken the password strength.
                char[][] charGroups = new char[][] 
                {
                    PASSWORD_CHARS_LCASE.ToCharArray(),
                    PASSWORD_CHARS_UCASE.ToCharArray(),
                    PASSWORD_CHARS_NUMERIC.ToCharArray(),
                    PASSWORD_CHARS_SPECIAL.ToCharArray()
                };

                // Use this array to track the number of unused characters in each
                // character group.
                int[] charsLeftInGroup = new int[charGroups.Length];

                // Initially, all characters in each group are not used.
                for (int i = 0; i < charsLeftInGroup.Length; i++)
                    charsLeftInGroup[i] = charGroups[i].Length;

                // Use this array to track (iterate through) unused character groups.
                int[] leftGroupsOrder = new int[charGroups.Length];

                // Initially, all character groups are not used.
                for (int i = 0; i < leftGroupsOrder.Length; i++)
                    leftGroupsOrder[i] = i;

                // Because we cannot use the default randomizer, which is based on the
                // current time (it will produce the same "random" number within a
                // second), we will use a random number generator to seed the
                // randomizer.

                // Use a 4-byte array to fill it with random bytes and convert it then
                // to an integer value.
                byte[] randomBytes = new byte[4];

                // Generate 4 random bytes.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(randomBytes);

                // Convert 4 bytes into a 32-bit integer value.
                int seed = (randomBytes[0] & 0x7f) << 24 |
                            randomBytes[1] << 16 |
                            randomBytes[2] << 8 |
                            randomBytes[3];

                // Now, this is real randomization.
                Random random = new Random(seed);

                // This array will hold password characters.
                char[] password = null;

                // Allocate appropriate memory for the password.
                if (minLength < maxLength)
                    password = new char[random.Next(minLength, maxLength + 1)];
                else
                    password = new char[minLength];

                // Index of the next character to be added to password.
                int nextCharIdx;

                // Index of the next character group to be processed.
                int nextGroupIdx;

                // Index which will be used to track not processed character groups.
                int nextLeftGroupsOrderIdx;

                // Index of the last non-processed character in a group.
                int lastCharIdx;

                // Index of the last non-processed group.
                int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

                // Generate password characters one at a time.
                for (int i = 0; i < password.Length; i++)
                {
                    // If only one character group remained unprocessed, process it;
                    // otherwise, pick a random character group from the unprocessed
                    // group list. To allow a special character to appear in the
                    // first position, increment the second parameter of the Next
                    // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                    if (lastLeftGroupsOrderIdx == 0)
                        nextLeftGroupsOrderIdx = 0;
                    else
                        nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx);

                    // Get the actual index of the character group, from which we will
                    // pick the next character.
                    nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                    // Get the index of the last unprocessed characters in this group.
                    lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                    // If only one unprocessed character is left, pick it; otherwise,
                    // get a random character from the unused character list.
                    if (lastCharIdx == 0)
                        nextCharIdx = 0;
                    else
                        nextCharIdx = random.Next(0, lastCharIdx + 1);

                    // Add this character to the password.
                    password[i] = charGroups[nextGroupIdx][nextCharIdx];

                    // If we processed the last character in this group, start over.
                    if (lastCharIdx == 0)
                        charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                    // There are more unprocessed characters left.
                    else
                    {
                        // Swap processed character with the last unprocessed character
                        // so that we don't pick it until we process all characters in
                        // this group.
                        if (lastCharIdx != nextCharIdx)
                        {
                            char temp = charGroups[nextGroupIdx][lastCharIdx];
                            charGroups[nextGroupIdx][lastCharIdx] =
                                        charGroups[nextGroupIdx][nextCharIdx];
                            charGroups[nextGroupIdx][nextCharIdx] = temp;
                        }
                        // Decrement the number of unprocessed characters in
                        // this group.
                        charsLeftInGroup[nextGroupIdx]--;
                    }

                    // If we processed the last group, start all over.
                    if (lastLeftGroupsOrderIdx == 0)
                        lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                    // There are more unprocessed groups left.
                    else
                    {
                        // Swap processed group with the last unprocessed group
                        // so that we don't pick it until we process all groups.
                        if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                        {
                            int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                            leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                        leftGroupsOrder[nextLeftGroupsOrderIdx];
                            leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                        }
                        // Decrement the number of unprocessed groups.
                        lastLeftGroupsOrderIdx--;
                    }
                }

                // Convert password characters into 
                // a string and return the result.
                return new string(password);
            }
            catch (System.Exception ex)
            {
                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
        }
        #endregion

        #region Public Random Methods
        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <returns>Randomly generated password.</returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        [Export("RandomGenerator")]
        public virtual string Random()
        {
            return RandomEx(DEFAULT_MIN_PASSWORD_LENGTH,
                            DEFAULT_MAX_PASSWORD_LENGTH);
        }

        /// <summary>
        /// Generates a random password of the exact length.
        /// </summary>
        /// <param name="length">Exact password length.</param>
        /// <returns>Randomly generated password.</returns>
        public virtual string Random(int length)
        {
            return RandomEx(length, length);
        }

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <param name="minLength">Minimum password length.</param>
        /// <param name="maxLength">Maximum password length.</param>
        /// <returns>Randomly generated password.</returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        public virtual string Random(int minLength, int maxLength)
        {
            return RandomEx(minLength, maxLength);
        }
        #endregion
    }

    /// <summary>
    /// Random standard password generator class.
    /// </summary>
    /// <remarks>Does not includes special characters.</remarks>
    [Export(typeof(IRandomGenerator))]
    [ContentMetadata(Name = "PasswordStandardGenerator")]
    public class PasswordStandardGenerator : IRandomGenerator
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly PasswordStandardGenerator Instance = new PasswordStandardGenerator();

        /// <summary>
        /// Static constructor
        /// </summary>
        static PasswordStandardGenerator() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public PasswordStandardGenerator()
        {
        }
        #endregion

        #region Private Constants
        private const string applicationName = "Nequeo";
        private const string eventNamespace = "Nequeo.Invention";
        #endregion

        #region Private Fields
        // Define default min and max password lengths.
        private int DEFAULT_MIN_PASSWORD_LENGTH = 8;
        private int DEFAULT_MAX_PASSWORD_LENGTH = 10;

        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
        private string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        private string PASSWORD_CHARS_NUMERIC = "0123456789";
        private string PASSWORD_CHARS_SPECIAL = "0123456789";
        #endregion

        #region Private Random Methods
        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <param name="minLength">Minimum password length.</param>
        /// <param name="maxLength">Maximum password length.</param>
        /// <returns>Randomly generated password.</returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        private string RandomEx(int minLength, int maxLength)
        {
            try
            {
                // Make sure that input parameters are valid.
                if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                    return null;

                // Create a local array containing supported password characters
                // grouped by types. You can remove character groups from this
                // array, but doing so will weaken the password strength.
                char[][] charGroups = new char[][] 
                {
                    PASSWORD_CHARS_LCASE.ToCharArray(),
                    PASSWORD_CHARS_UCASE.ToCharArray(),
                    PASSWORD_CHARS_NUMERIC.ToCharArray(),
                    PASSWORD_CHARS_SPECIAL.ToCharArray()
                };

                // Use this array to track the number of unused characters in each
                // character group.
                int[] charsLeftInGroup = new int[charGroups.Length];

                // Initially, all characters in each group are not used.
                for (int i = 0; i < charsLeftInGroup.Length; i++)
                    charsLeftInGroup[i] = charGroups[i].Length;

                // Use this array to track (iterate through) unused character groups.
                int[] leftGroupsOrder = new int[charGroups.Length];

                // Initially, all character groups are not used.
                for (int i = 0; i < leftGroupsOrder.Length; i++)
                    leftGroupsOrder[i] = i;

                // Because we cannot use the default randomizer, which is based on the
                // current time (it will produce the same "random" number within a
                // second), we will use a random number generator to seed the
                // randomizer.

                // Use a 4-byte array to fill it with random bytes and convert it then
                // to an integer value.
                byte[] randomBytes = new byte[4];

                // Generate 4 random bytes.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(randomBytes);

                // Convert 4 bytes into a 32-bit integer value.
                int seed = (randomBytes[0] & 0x7f) << 24 |
                            randomBytes[1] << 16 |
                            randomBytes[2] << 8 |
                            randomBytes[3];

                // Now, this is real randomization.
                Random random = new Random(seed);

                // This array will hold password characters.
                char[] password = null;

                // Allocate appropriate memory for the password.
                if (minLength < maxLength)
                    password = new char[random.Next(minLength, maxLength + 1)];
                else
                    password = new char[minLength];

                // Index of the next character to be added to password.
                int nextCharIdx;

                // Index of the next character group to be processed.
                int nextGroupIdx;

                // Index which will be used to track not processed character groups.
                int nextLeftGroupsOrderIdx;

                // Index of the last non-processed character in a group.
                int lastCharIdx;

                // Index of the last non-processed group.
                int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

                // Generate password characters one at a time.
                for (int i = 0; i < password.Length; i++)
                {
                    // If only one character group remained unprocessed, process it;
                    // otherwise, pick a random character group from the unprocessed
                    // group list. To allow a special character to appear in the
                    // first position, increment the second parameter of the Next
                    // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                    if (lastLeftGroupsOrderIdx == 0)
                        nextLeftGroupsOrderIdx = 0;
                    else
                        nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx);

                    // Get the actual index of the character group, from which we will
                    // pick the next character.
                    nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                    // Get the index of the last unprocessed characters in this group.
                    lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                    // If only one unprocessed character is left, pick it; otherwise,
                    // get a random character from the unused character list.
                    if (lastCharIdx == 0)
                        nextCharIdx = 0;
                    else
                        nextCharIdx = random.Next(0, lastCharIdx + 1);

                    // Add this character to the password.
                    password[i] = charGroups[nextGroupIdx][nextCharIdx];

                    // If we processed the last character in this group, start over.
                    if (lastCharIdx == 0)
                        charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                    // There are more unprocessed characters left.
                    else
                    {
                        // Swap processed character with the last unprocessed character
                        // so that we don't pick it until we process all characters in
                        // this group.
                        if (lastCharIdx != nextCharIdx)
                        {
                            char temp = charGroups[nextGroupIdx][lastCharIdx];
                            charGroups[nextGroupIdx][lastCharIdx] =
                                        charGroups[nextGroupIdx][nextCharIdx];
                            charGroups[nextGroupIdx][nextCharIdx] = temp;
                        }
                        // Decrement the number of unprocessed characters in
                        // this group.
                        charsLeftInGroup[nextGroupIdx]--;
                    }

                    // If we processed the last group, start all over.
                    if (lastLeftGroupsOrderIdx == 0)
                        lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                    // There are more unprocessed groups left.
                    else
                    {
                        // Swap processed group with the last unprocessed group
                        // so that we don't pick it until we process all groups.
                        if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                        {
                            int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                            leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                        leftGroupsOrder[nextLeftGroupsOrderIdx];
                            leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                        }
                        // Decrement the number of unprocessed groups.
                        lastLeftGroupsOrderIdx--;
                    }
                }

                // Convert password characters into 
                // a string and return the result.
                return new string(password);
            }
            catch (System.Exception ex)
            {
                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
        }
        #endregion

        #region Public Random Methods
        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <returns>Randomly generated password.</returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        [Export("RandomGenerator")]
        public virtual string Random()
        {
            return RandomEx(DEFAULT_MIN_PASSWORD_LENGTH,
                            DEFAULT_MAX_PASSWORD_LENGTH);
        }

        /// <summary>
        /// Generates a random password of the exact length.
        /// </summary>
        /// <param name="length">Exact password length.</param>
        /// <returns>Randomly generated password.</returns>
        public virtual string Random(int length)
        {
            return RandomEx(length, length);
        }

        /// <summary>
        /// Generates a random password.
        /// </summary>
        /// <param name="minLength">Minimum password length.</param>
        /// <param name="maxLength">Maximum password length.</param>
        /// <returns>Randomly generated password.</returns>
        /// <remarks>
        /// The length of the generated password will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        public virtual string Random(int minLength, int maxLength)
        {
            return RandomEx(minLength, maxLength);
        }
        #endregion
    }

    /// <summary>
    /// Random number generator class.
    /// </summary>
    [Export(typeof(IRandomGenerator))]
    [ContentMetadata(Name = "NumberGenerator")]
    public class NumberGenerator : IRandomGenerator
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly NumberGenerator Instance = new NumberGenerator();

        /// <summary>
        /// Static constructor
        /// </summary>
        static NumberGenerator() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public NumberGenerator()
        {
        }
        #endregion

        #region Private Constants
        private const string applicationName = "Nequeo";
        private const string eventNamespace = "Nequeo.Invention";
        #endregion

        #region Private Fields
        // Define default min and max number lengths.
        private int DEFAULT_MIN_NUMBER_LENGTH = 8;
        private int DEFAULT_MAX_NUMBER_LENGTH = 10;

        // Define supported number characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private string NUMBER_CHARS_NUMERIC_1 = "0123456789";
        private string NUMBER_CHARS_NUMERIC_2 = "0123456789";
        private string NUMBER_CHARS_NUMERIC_3 = "0123456789";
        private string NUMBER_CHARS_NUMERIC_4 = "0123456789";
        #endregion

        #region Private Random Methods
        /// <summary>
        /// Generates a random number.
        /// </summary>
        /// <param name="minLength">Minimum number length.</param>
        /// <param name="maxLength">Maximum number length.</param>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        private string RandomEx(int minLength, int maxLength)
        {
            try
            {
                // Make sure that input parameters are valid.
                if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                    return null;

                // Create a local array containing supported number characters
                // grouped by types. You can remove character groups from this
                // array, but doing so will weaken the number strength.
                char[][] charGroups = new char[][] 
                {
                    NUMBER_CHARS_NUMERIC_1.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_2.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_3.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_4.ToCharArray()
                };

                // Use this array to track the number of unused characters in each
                // character group.
                int[] charsLeftInGroup = new int[charGroups.Length];

                // Initially, all characters in each group are not used.
                for (int i = 0; i < charsLeftInGroup.Length; i++)
                    charsLeftInGroup[i] = charGroups[i].Length;

                // Use this array to track (iterate through) unused character groups.
                int[] leftGroupsOrder = new int[charGroups.Length];

                // Initially, all character groups are not used.
                for (int i = 0; i < leftGroupsOrder.Length; i++)
                    leftGroupsOrder[i] = i;

                // Because we cannot use the default randomizer, which is based on the
                // current time (it will produce the same "random" number within a
                // second), we will use a random number generator to seed the
                // randomizer.

                // Use a 4-byte array to fill it with random bytes and convert it then
                // to an integer value.
                byte[] randomBytes = new byte[4];

                // Generate 4 random bytes.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(randomBytes);

                // Convert 4 bytes into a 32-bit integer value.
                int seed = (randomBytes[0] & 0x7f) << 24 |
                            randomBytes[1] << 16 |
                            randomBytes[2] << 8 |
                            randomBytes[3];

                // Now, this is real randomization.
                Random random = new Random(seed);

                // This array will hold number characters.
                char[] number = null;

                // Allocate appropriate memory for the number.
                if (minLength < maxLength)
                    number = new char[random.Next(minLength, maxLength + 1)];
                else
                    number = new char[minLength];

                // Index of the next character to be added to number.
                int nextCharIdx;

                // Index of the next character group to be processed.
                int nextGroupIdx;

                // Index which will be used to track not processed character groups.
                int nextLeftGroupsOrderIdx;

                // Index of the last non-processed character in a group.
                int lastCharIdx;

                // Index of the last non-processed group.
                int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

                // Generate password characters one at a time.
                for (int i = 0; i < number.Length; i++)
                {
                    // If only one character group remained unprocessed, process it;
                    // otherwise, pick a random character group from the unprocessed
                    // group list. To allow a special character to appear in the
                    // first position, increment the second parameter of the Next
                    // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                    if (lastLeftGroupsOrderIdx == 0)
                        nextLeftGroupsOrderIdx = 0;
                    else
                        nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx);

                    // Get the actual index of the character group, from which we will
                    // pick the next character.
                    nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                    // Get the index of the last unprocessed characters in this group.
                    lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                    // If only one unprocessed character is left, pick it; otherwise,
                    // get a random character from the unused character list.
                    if (lastCharIdx == 0)
                        nextCharIdx = 0;
                    else
                        nextCharIdx = random.Next(0, lastCharIdx + 1);

                    // Add this character to the number.
                    number[i] = charGroups[nextGroupIdx][nextCharIdx];

                    // If we processed the last character in this group, start over.
                    if (lastCharIdx == 0)
                        charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                    // There are more unprocessed characters left.
                    else
                    {
                        // Swap processed character with the last unprocessed character
                        // so that we don't pick it until we process all characters in
                        // this group.
                        if (lastCharIdx != nextCharIdx)
                        {
                            char temp = charGroups[nextGroupIdx][lastCharIdx];
                            charGroups[nextGroupIdx][lastCharIdx] =
                                        charGroups[nextGroupIdx][nextCharIdx];
                            charGroups[nextGroupIdx][nextCharIdx] = temp;
                        }
                        // Decrement the number of unprocessed characters in
                        // this group.
                        charsLeftInGroup[nextGroupIdx]--;
                    }

                    // If we processed the last group, start all over.
                    if (lastLeftGroupsOrderIdx == 0)
                        lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                    // There are more unprocessed groups left.
                    else
                    {
                        // Swap processed group with the last unprocessed group
                        // so that we don't pick it until we process all groups.
                        if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                        {
                            int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                            leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                        leftGroupsOrder[nextLeftGroupsOrderIdx];
                            leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                        }
                        // Decrement the number of unprocessed groups.
                        lastLeftGroupsOrderIdx--;
                    }
                }

                // Convert number characters into 
                // a string and return the result.
                return new string(number);
            }
            catch (System.Exception ex)
            {
                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
        }
        #endregion

        #region Public Random Methods
        /// <summary>
        /// Generates a random number.
        /// </summary>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        [Export("RandomGenerator")]
        public virtual string Random()
        {
            return RandomEx(DEFAULT_MIN_NUMBER_LENGTH,
                            DEFAULT_MAX_NUMBER_LENGTH);
        }

        /// <summary>
        /// Generates a random number of the exact length.
        /// </summary>
        /// <param name="length">Exact number length.</param>
        /// <returns>Randomly generated number.</returns>
        public virtual string Random(int length)
        {
            return RandomEx(length, length);
        }

        /// <summary>
        /// Generates a random number.
        /// </summary>
        /// <param name="minLength">Minimum number length.</param>
        /// <param name="maxLength">Maximum number length.</param>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        public virtual string Random(int minLength, int maxLength)
        {
            return RandomEx(minLength, maxLength);
        }
        #endregion
    }

    /// <summary>
    /// Random upper case letter generator class.
    /// </summary>
    [Export(typeof(IRandomGenerator))]
    [ContentMetadata(Name = "UpperCaseGenerator")]
    public class UpperCaseGenerator : IRandomGenerator
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly UpperCaseGenerator Instance = new UpperCaseGenerator();

        /// <summary>
        /// Static constructor
        /// </summary>
        static UpperCaseGenerator() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public UpperCaseGenerator()
        {
        }
        #endregion

        #region Private Constants
        private const string applicationName = "Nequeo";
        private const string eventNamespace = "Nequeo.Invention";
        #endregion

        #region Private Fields
        // Define default min and max number lengths.
        private int DEFAULT_MIN_NUMBER_LENGTH = 8;
        private int DEFAULT_MAX_NUMBER_LENGTH = 10;

        // Define supported number characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private string NUMBER_CHARS_NUMERIC_1 = "ABCDEFGHJKLMNPQRSTWXYZ";
        private string NUMBER_CHARS_NUMERIC_2 = "ABCDEFGHJKLMNPQRSTWXYZ";
        private string NUMBER_CHARS_NUMERIC_3 = "ABCDEFGHJKLMNPQRSTWXYZ";
        private string NUMBER_CHARS_NUMERIC_4 = "ABCDEFGHJKLMNPQRSTWXYZ";
        #endregion

        #region Private Random Methods
        /// <summary>
        /// Generates a random upper case number.
        /// </summary>
        /// <param name="minLength">Minimum number length.</param>
        /// <param name="maxLength">Maximum number length.</param>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        private string RandomEx(int minLength, int maxLength)
        {
            try
            {
                // Make sure that input parameters are valid.
                if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                    return null;

                // Create a local array containing supported number characters
                // grouped by types. You can remove character groups from this
                // array, but doing so will weaken the number strength.
                char[][] charGroups = new char[][] 
                {
                    NUMBER_CHARS_NUMERIC_1.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_2.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_3.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_4.ToCharArray()
                };

                // Use this array to track the number of unused characters in each
                // character group.
                int[] charsLeftInGroup = new int[charGroups.Length];

                // Initially, all characters in each group are not used.
                for (int i = 0; i < charsLeftInGroup.Length; i++)
                    charsLeftInGroup[i] = charGroups[i].Length;

                // Use this array to track (iterate through) unused character groups.
                int[] leftGroupsOrder = new int[charGroups.Length];

                // Initially, all character groups are not used.
                for (int i = 0; i < leftGroupsOrder.Length; i++)
                    leftGroupsOrder[i] = i;

                // Because we cannot use the default randomizer, which is based on the
                // current time (it will produce the same "random" number within a
                // second), we will use a random number generator to seed the
                // randomizer.

                // Use a 4-byte array to fill it with random bytes and convert it then
                // to an integer value.
                byte[] randomBytes = new byte[4];

                // Generate 4 random bytes.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(randomBytes);

                // Convert 4 bytes into a 32-bit integer value.
                int seed = (randomBytes[0] & 0x7f) << 24 |
                            randomBytes[1] << 16 |
                            randomBytes[2] << 8 |
                            randomBytes[3];

                // Now, this is real randomization.
                Random random = new Random(seed);

                // This array will hold number characters.
                char[] number = null;

                // Allocate appropriate memory for the number.
                if (minLength < maxLength)
                    number = new char[random.Next(minLength, maxLength + 1)];
                else
                    number = new char[minLength];

                // Index of the next character to be added to number.
                int nextCharIdx;

                // Index of the next character group to be processed.
                int nextGroupIdx;

                // Index which will be used to track not processed character groups.
                int nextLeftGroupsOrderIdx;

                // Index of the last non-processed character in a group.
                int lastCharIdx;

                // Index of the last non-processed group.
                int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

                // Generate password characters one at a time.
                for (int i = 0; i < number.Length; i++)
                {
                    // If only one character group remained unprocessed, process it;
                    // otherwise, pick a random character group from the unprocessed
                    // group list. To allow a special character to appear in the
                    // first position, increment the second parameter of the Next
                    // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                    if (lastLeftGroupsOrderIdx == 0)
                        nextLeftGroupsOrderIdx = 0;
                    else
                        nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx);

                    // Get the actual index of the character group, from which we will
                    // pick the next character.
                    nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                    // Get the index of the last unprocessed characters in this group.
                    lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                    // If only one unprocessed character is left, pick it; otherwise,
                    // get a random character from the unused character list.
                    if (lastCharIdx == 0)
                        nextCharIdx = 0;
                    else
                        nextCharIdx = random.Next(0, lastCharIdx + 1);

                    // Add this character to the number.
                    number[i] = charGroups[nextGroupIdx][nextCharIdx];

                    // If we processed the last character in this group, start over.
                    if (lastCharIdx == 0)
                        charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                    // There are more unprocessed characters left.
                    else
                    {
                        // Swap processed character with the last unprocessed character
                        // so that we don't pick it until we process all characters in
                        // this group.
                        if (lastCharIdx != nextCharIdx)
                        {
                            char temp = charGroups[nextGroupIdx][lastCharIdx];
                            charGroups[nextGroupIdx][lastCharIdx] =
                                        charGroups[nextGroupIdx][nextCharIdx];
                            charGroups[nextGroupIdx][nextCharIdx] = temp;
                        }
                        // Decrement the number of unprocessed characters in
                        // this group.
                        charsLeftInGroup[nextGroupIdx]--;
                    }

                    // If we processed the last group, start all over.
                    if (lastLeftGroupsOrderIdx == 0)
                        lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                    // There are more unprocessed groups left.
                    else
                    {
                        // Swap processed group with the last unprocessed group
                        // so that we don't pick it until we process all groups.
                        if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                        {
                            int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                            leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                        leftGroupsOrder[nextLeftGroupsOrderIdx];
                            leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                        }
                        // Decrement the number of unprocessed groups.
                        lastLeftGroupsOrderIdx--;
                    }
                }

                // Convert number characters into 
                // a string and return the result.
                return new string(number);
            }
            catch (System.Exception ex)
            {
                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
        }
        #endregion

        #region Public Random Methods
        /// <summary>
        /// Generates a random upper case number.
        /// </summary>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        [Export("RandomGenerator")]
        public virtual string Random()
        {
            return RandomEx(DEFAULT_MIN_NUMBER_LENGTH,
                            DEFAULT_MAX_NUMBER_LENGTH);
        }

        /// <summary>
        /// Generates a random upper case number.
        /// </summary>
        /// <param name="length">Exact number length.</param>
        /// <returns>Randomly generated number.</returns>
        public virtual string Random(int length)
        {
            return RandomEx(length, length);
        }

        /// <summary>
        /// Generates a random upper case number.
        /// </summary>
        /// <param name="minLength">Minimum number length.</param>
        /// <param name="maxLength">Maximum number length.</param>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        public virtual string Random(int minLength, int maxLength)
        {
            return RandomEx(minLength, maxLength);
        }
        #endregion
    }

    /// <summary>
    /// Random lower case letter generator class.
    /// </summary>
    [Export(typeof(IRandomGenerator))]
    [ContentMetadata(Name = "LowerCaseGenerator")]
    public class LowerCaseGenerator : IRandomGenerator
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly LowerCaseGenerator Instance = new LowerCaseGenerator();

        /// <summary>
        /// Static constructor
        /// </summary>
        static LowerCaseGenerator() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public LowerCaseGenerator()
        {
        }
        #endregion

        #region Private Constants
        private const string applicationName = "Nequeo";
        private const string eventNamespace = "Nequeo.Invention";
        #endregion

        #region Private Fields
        // Define default min and max number lengths.
        private int DEFAULT_MIN_NUMBER_LENGTH = 8;
        private int DEFAULT_MAX_NUMBER_LENGTH = 10;

        // Define supported number characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private string NUMBER_CHARS_NUMERIC_1 = "abcdefgijkmnopqrstwxyz";
        private string NUMBER_CHARS_NUMERIC_2 = "abcdefgijkmnopqrstwxyz";
        private string NUMBER_CHARS_NUMERIC_3 = "abcdefgijkmnopqrstwxyz";
        private string NUMBER_CHARS_NUMERIC_4 = "abcdefgijkmnopqrstwxyz";
        #endregion

        #region Private Random Methods
        /// <summary>
        /// Generates a random lower case number.
        /// </summary>
        /// <param name="minLength">Minimum number length.</param>
        /// <param name="maxLength">Maximum number length.</param>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        private string RandomEx(int minLength, int maxLength)
        {
            try
            {
                // Make sure that input parameters are valid.
                if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                    return null;

                // Create a local array containing supported number characters
                // grouped by types. You can remove character groups from this
                // array, but doing so will weaken the number strength.
                char[][] charGroups = new char[][] 
                {
                    NUMBER_CHARS_NUMERIC_1.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_2.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_3.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_4.ToCharArray()
                };

                // Use this array to track the number of unused characters in each
                // character group.
                int[] charsLeftInGroup = new int[charGroups.Length];

                // Initially, all characters in each group are not used.
                for (int i = 0; i < charsLeftInGroup.Length; i++)
                    charsLeftInGroup[i] = charGroups[i].Length;

                // Use this array to track (iterate through) unused character groups.
                int[] leftGroupsOrder = new int[charGroups.Length];

                // Initially, all character groups are not used.
                for (int i = 0; i < leftGroupsOrder.Length; i++)
                    leftGroupsOrder[i] = i;

                // Because we cannot use the default randomizer, which is based on the
                // current time (it will produce the same "random" number within a
                // second), we will use a random number generator to seed the
                // randomizer.

                // Use a 4-byte array to fill it with random bytes and convert it then
                // to an integer value.
                byte[] randomBytes = new byte[4];

                // Generate 4 random bytes.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(randomBytes);

                // Convert 4 bytes into a 32-bit integer value.
                int seed = (randomBytes[0] & 0x7f) << 24 |
                            randomBytes[1] << 16 |
                            randomBytes[2] << 8 |
                            randomBytes[3];

                // Now, this is real randomization.
                Random random = new Random(seed);

                // This array will hold number characters.
                char[] number = null;

                // Allocate appropriate memory for the number.
                if (minLength < maxLength)
                    number = new char[random.Next(minLength, maxLength + 1)];
                else
                    number = new char[minLength];

                // Index of the next character to be added to number.
                int nextCharIdx;

                // Index of the next character group to be processed.
                int nextGroupIdx;

                // Index which will be used to track not processed character groups.
                int nextLeftGroupsOrderIdx;

                // Index of the last non-processed character in a group.
                int lastCharIdx;

                // Index of the last non-processed group.
                int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

                // Generate password characters one at a time.
                for (int i = 0; i < number.Length; i++)
                {
                    // If only one character group remained unprocessed, process it;
                    // otherwise, pick a random character group from the unprocessed
                    // group list. To allow a special character to appear in the
                    // first position, increment the second parameter of the Next
                    // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                    if (lastLeftGroupsOrderIdx == 0)
                        nextLeftGroupsOrderIdx = 0;
                    else
                        nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx);

                    // Get the actual index of the character group, from which we will
                    // pick the next character.
                    nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                    // Get the index of the last unprocessed characters in this group.
                    lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                    // If only one unprocessed character is left, pick it; otherwise,
                    // get a random character from the unused character list.
                    if (lastCharIdx == 0)
                        nextCharIdx = 0;
                    else
                        nextCharIdx = random.Next(0, lastCharIdx + 1);

                    // Add this character to the number.
                    number[i] = charGroups[nextGroupIdx][nextCharIdx];

                    // If we processed the last character in this group, start over.
                    if (lastCharIdx == 0)
                        charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                    // There are more unprocessed characters left.
                    else
                    {
                        // Swap processed character with the last unprocessed character
                        // so that we don't pick it until we process all characters in
                        // this group.
                        if (lastCharIdx != nextCharIdx)
                        {
                            char temp = charGroups[nextGroupIdx][lastCharIdx];
                            charGroups[nextGroupIdx][lastCharIdx] =
                                        charGroups[nextGroupIdx][nextCharIdx];
                            charGroups[nextGroupIdx][nextCharIdx] = temp;
                        }
                        // Decrement the number of unprocessed characters in
                        // this group.
                        charsLeftInGroup[nextGroupIdx]--;
                    }

                    // If we processed the last group, start all over.
                    if (lastLeftGroupsOrderIdx == 0)
                        lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                    // There are more unprocessed groups left.
                    else
                    {
                        // Swap processed group with the last unprocessed group
                        // so that we don't pick it until we process all groups.
                        if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                        {
                            int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                            leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                        leftGroupsOrder[nextLeftGroupsOrderIdx];
                            leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                        }
                        // Decrement the number of unprocessed groups.
                        lastLeftGroupsOrderIdx--;
                    }
                }

                // Convert number characters into 
                // a string and return the result.
                return new string(number);
            }
            catch (System.Exception ex)
            {
                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
        }
        #endregion

        #region Public Random Methods
        /// <summary>
        /// Generates a random lower case number.
        /// </summary>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        [Export("RandomGenerator")]
        public virtual string Random()
        {
            return RandomEx(DEFAULT_MIN_NUMBER_LENGTH,
                            DEFAULT_MAX_NUMBER_LENGTH);
        }

        /// <summary>
        /// Generates a random lower case number.
        /// </summary>
        /// <param name="length">Exact number length.</param>
        /// <returns>Randomly generated number.</returns>
        public virtual string Random(int length)
        {
            return RandomEx(length, length);
        }

        /// <summary>
        /// Generates a random lower case number.
        /// </summary>
        /// <param name="minLength">Minimum number length.</param>
        /// <param name="maxLength">Maximum number length.</param>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        public virtual string Random(int minLength, int maxLength)
        {
            return RandomEx(minLength, maxLength);
        }
        #endregion
    }

    /// <summary>
    /// Random lower case and upper case letter generator class.
    /// </summary>
    [Export(typeof(IRandomGenerator))]
    [ContentMetadata(Name = "LowerUpperCaseGenerator")]
    public class LowerUpperCaseGenerator : IRandomGenerator
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly LowerUpperCaseGenerator Instance = new LowerUpperCaseGenerator();

        /// <summary>
        /// Static constructor
        /// </summary>
        static LowerUpperCaseGenerator() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public LowerUpperCaseGenerator()
        {
        }
        #endregion

        #region Private Constants
        private const string applicationName = "Nequeo";
        private const string eventNamespace = "Nequeo.Invention";
        #endregion

        #region Private Fields
        // Define default min and max number lengths.
        private int DEFAULT_MIN_NUMBER_LENGTH = 8;
        private int DEFAULT_MAX_NUMBER_LENGTH = 10;

        // Define supported number characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private string NUMBER_CHARS_NUMERIC_1 = "abcdefgijkmnopqrstwxyz";
        private string NUMBER_CHARS_NUMERIC_2 = "ABCDEFGHJKLMNPQRSTWXYZ";
        private string NUMBER_CHARS_NUMERIC_3 = "abcdefgijkmnopqrstwxyz";
        private string NUMBER_CHARS_NUMERIC_4 = "ABCDEFGHJKLMNPQRSTWXYZ";
        #endregion

        #region Private Random Methods
        /// <summary>
        /// Generates a random lower and upper case number.
        /// </summary>
        /// <param name="minLength">Minimum number length.</param>
        /// <param name="maxLength">Maximum number length.</param>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        private string RandomEx(int minLength, int maxLength)
        {
            try
            {
                // Make sure that input parameters are valid.
                if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                    return null;

                // Create a local array containing supported number characters
                // grouped by types. You can remove character groups from this
                // array, but doing so will weaken the number strength.
                char[][] charGroups = new char[][] 
                {
                    NUMBER_CHARS_NUMERIC_1.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_2.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_3.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_4.ToCharArray()
                };

                // Use this array to track the number of unused characters in each
                // character group.
                int[] charsLeftInGroup = new int[charGroups.Length];

                // Initially, all characters in each group are not used.
                for (int i = 0; i < charsLeftInGroup.Length; i++)
                    charsLeftInGroup[i] = charGroups[i].Length;

                // Use this array to track (iterate through) unused character groups.
                int[] leftGroupsOrder = new int[charGroups.Length];

                // Initially, all character groups are not used.
                for (int i = 0; i < leftGroupsOrder.Length; i++)
                    leftGroupsOrder[i] = i;

                // Because we cannot use the default randomizer, which is based on the
                // current time (it will produce the same "random" number within a
                // second), we will use a random number generator to seed the
                // randomizer.

                // Use a 4-byte array to fill it with random bytes and convert it then
                // to an integer value.
                byte[] randomBytes = new byte[4];

                // Generate 4 random bytes.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(randomBytes);

                // Convert 4 bytes into a 32-bit integer value.
                int seed = (randomBytes[0] & 0x7f) << 24 |
                            randomBytes[1] << 16 |
                            randomBytes[2] << 8 |
                            randomBytes[3];

                // Now, this is real randomization.
                Random random = new Random(seed);

                // This array will hold number characters.
                char[] number = null;

                // Allocate appropriate memory for the number.
                if (minLength < maxLength)
                    number = new char[random.Next(minLength, maxLength + 1)];
                else
                    number = new char[minLength];

                // Index of the next character to be added to number.
                int nextCharIdx;

                // Index of the next character group to be processed.
                int nextGroupIdx;

                // Index which will be used to track not processed character groups.
                int nextLeftGroupsOrderIdx;

                // Index of the last non-processed character in a group.
                int lastCharIdx;

                // Index of the last non-processed group.
                int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

                // Generate password characters one at a time.
                for (int i = 0; i < number.Length; i++)
                {
                    // If only one character group remained unprocessed, process it;
                    // otherwise, pick a random character group from the unprocessed
                    // group list. To allow a special character to appear in the
                    // first position, increment the second parameter of the Next
                    // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                    if (lastLeftGroupsOrderIdx == 0)
                        nextLeftGroupsOrderIdx = 0;
                    else
                        nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx);

                    // Get the actual index of the character group, from which we will
                    // pick the next character.
                    nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                    // Get the index of the last unprocessed characters in this group.
                    lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                    // If only one unprocessed character is left, pick it; otherwise,
                    // get a random character from the unused character list.
                    if (lastCharIdx == 0)
                        nextCharIdx = 0;
                    else
                        nextCharIdx = random.Next(0, lastCharIdx + 1);

                    // Add this character to the number.
                    number[i] = charGroups[nextGroupIdx][nextCharIdx];

                    // If we processed the last character in this group, start over.
                    if (lastCharIdx == 0)
                        charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                    // There are more unprocessed characters left.
                    else
                    {
                        // Swap processed character with the last unprocessed character
                        // so that we don't pick it until we process all characters in
                        // this group.
                        if (lastCharIdx != nextCharIdx)
                        {
                            char temp = charGroups[nextGroupIdx][lastCharIdx];
                            charGroups[nextGroupIdx][lastCharIdx] =
                                        charGroups[nextGroupIdx][nextCharIdx];
                            charGroups[nextGroupIdx][nextCharIdx] = temp;
                        }
                        // Decrement the number of unprocessed characters in
                        // this group.
                        charsLeftInGroup[nextGroupIdx]--;
                    }

                    // If we processed the last group, start all over.
                    if (lastLeftGroupsOrderIdx == 0)
                        lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                    // There are more unprocessed groups left.
                    else
                    {
                        // Swap processed group with the last unprocessed group
                        // so that we don't pick it until we process all groups.
                        if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                        {
                            int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                            leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                        leftGroupsOrder[nextLeftGroupsOrderIdx];
                            leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                        }
                        // Decrement the number of unprocessed groups.
                        lastLeftGroupsOrderIdx--;
                    }
                }

                // Convert number characters into 
                // a string and return the result.
                return new string(number);
            }
            catch (System.Exception ex)
            {
                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
        }
        #endregion

        #region Public Random Methods
        /// <summary>
        /// Generates a random lower and upper case number.
        /// </summary>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        [Export("RandomGenerator")]
        public virtual string Random()
        {
            return RandomEx(DEFAULT_MIN_NUMBER_LENGTH,
                            DEFAULT_MAX_NUMBER_LENGTH);
        }

        /// <summary>
        /// Generates a random lower and upper case number.
        /// </summary>
        /// <param name="length">Exact number length.</param>
        /// <returns>Randomly generated number.</returns>
        public virtual string Random(int length)
        {
            return RandomEx(length, length);
        }

        /// <summary>
        /// Generates a random lower and upper case number.
        /// </summary>
        /// <param name="minLength">Minimum number length.</param>
        /// <param name="maxLength">Maximum number length.</param>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        public virtual string Random(int minLength, int maxLength)
        {
            return RandomEx(minLength, maxLength);
        }
        #endregion
    }

    /// <summary>
    /// Random number and lower case letter generator class.
    /// </summary>
    [Export(typeof(IRandomGenerator))]
    [ContentMetadata(Name = "NumberLowerCaseGenerator")]
    public class NumberLowerCaseGenerator : IRandomGenerator
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly NumberLowerCaseGenerator Instance = new NumberLowerCaseGenerator();

        /// <summary>
        /// Static constructor
        /// </summary>
        static NumberLowerCaseGenerator() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public NumberLowerCaseGenerator()
        {
        }
        #endregion

        #region Private Constants
        private const string applicationName = "Nequeo";
        private const string eventNamespace = "Nequeo.Invention";
        #endregion

        #region Private Fields
        // Define default min and max number lengths.
        private int DEFAULT_MIN_NUMBER_LENGTH = 8;
        private int DEFAULT_MAX_NUMBER_LENGTH = 10;

        // Define supported number characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private string NUMBER_CHARS_NUMERIC_1 = "abcdefgijkmnopqrstwxyz";
        private string NUMBER_CHARS_NUMERIC_2 = "0123456789";
        private string NUMBER_CHARS_NUMERIC_3 = "abcdefgijkmnopqrstwxyz";
        private string NUMBER_CHARS_NUMERIC_4 = "0123456789";
        #endregion

        #region Private Random Methods
        /// <summary>
        /// Generates a random number and lower case number.
        /// </summary>
        /// <param name="minLength">Minimum number length.</param>
        /// <param name="maxLength">Maximum number length.</param>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        private string RandomEx(int minLength, int maxLength)
        {
            try
            {
                // Make sure that input parameters are valid.
                if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                    return null;

                // Create a local array containing supported number characters
                // grouped by types. You can remove character groups from this
                // array, but doing so will weaken the number strength.
                char[][] charGroups = new char[][] 
                {
                    NUMBER_CHARS_NUMERIC_1.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_2.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_3.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_4.ToCharArray()
                };

                // Use this array to track the number of unused characters in each
                // character group.
                int[] charsLeftInGroup = new int[charGroups.Length];

                // Initially, all characters in each group are not used.
                for (int i = 0; i < charsLeftInGroup.Length; i++)
                    charsLeftInGroup[i] = charGroups[i].Length;

                // Use this array to track (iterate through) unused character groups.
                int[] leftGroupsOrder = new int[charGroups.Length];

                // Initially, all character groups are not used.
                for (int i = 0; i < leftGroupsOrder.Length; i++)
                    leftGroupsOrder[i] = i;

                // Because we cannot use the default randomizer, which is based on the
                // current time (it will produce the same "random" number within a
                // second), we will use a random number generator to seed the
                // randomizer.

                // Use a 4-byte array to fill it with random bytes and convert it then
                // to an integer value.
                byte[] randomBytes = new byte[4];

                // Generate 4 random bytes.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(randomBytes);

                // Convert 4 bytes into a 32-bit integer value.
                int seed = (randomBytes[0] & 0x7f) << 24 |
                            randomBytes[1] << 16 |
                            randomBytes[2] << 8 |
                            randomBytes[3];

                // Now, this is real randomization.
                Random random = new Random(seed);

                // This array will hold number characters.
                char[] number = null;

                // Allocate appropriate memory for the number.
                if (minLength < maxLength)
                    number = new char[random.Next(minLength, maxLength + 1)];
                else
                    number = new char[minLength];

                // Index of the next character to be added to number.
                int nextCharIdx;

                // Index of the next character group to be processed.
                int nextGroupIdx;

                // Index which will be used to track not processed character groups.
                int nextLeftGroupsOrderIdx;

                // Index of the last non-processed character in a group.
                int lastCharIdx;

                // Index of the last non-processed group.
                int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

                // Generate password characters one at a time.
                for (int i = 0; i < number.Length; i++)
                {
                    // If only one character group remained unprocessed, process it;
                    // otherwise, pick a random character group from the unprocessed
                    // group list. To allow a special character to appear in the
                    // first position, increment the second parameter of the Next
                    // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                    if (lastLeftGroupsOrderIdx == 0)
                        nextLeftGroupsOrderIdx = 0;
                    else
                        nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx);

                    // Get the actual index of the character group, from which we will
                    // pick the next character.
                    nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                    // Get the index of the last unprocessed characters in this group.
                    lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                    // If only one unprocessed character is left, pick it; otherwise,
                    // get a random character from the unused character list.
                    if (lastCharIdx == 0)
                        nextCharIdx = 0;
                    else
                        nextCharIdx = random.Next(0, lastCharIdx + 1);

                    // Add this character to the number.
                    number[i] = charGroups[nextGroupIdx][nextCharIdx];

                    // If we processed the last character in this group, start over.
                    if (lastCharIdx == 0)
                        charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                    // There are more unprocessed characters left.
                    else
                    {
                        // Swap processed character with the last unprocessed character
                        // so that we don't pick it until we process all characters in
                        // this group.
                        if (lastCharIdx != nextCharIdx)
                        {
                            char temp = charGroups[nextGroupIdx][lastCharIdx];
                            charGroups[nextGroupIdx][lastCharIdx] =
                                        charGroups[nextGroupIdx][nextCharIdx];
                            charGroups[nextGroupIdx][nextCharIdx] = temp;
                        }
                        // Decrement the number of unprocessed characters in
                        // this group.
                        charsLeftInGroup[nextGroupIdx]--;
                    }

                    // If we processed the last group, start all over.
                    if (lastLeftGroupsOrderIdx == 0)
                        lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                    // There are more unprocessed groups left.
                    else
                    {
                        // Swap processed group with the last unprocessed group
                        // so that we don't pick it until we process all groups.
                        if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                        {
                            int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                            leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                        leftGroupsOrder[nextLeftGroupsOrderIdx];
                            leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                        }
                        // Decrement the number of unprocessed groups.
                        lastLeftGroupsOrderIdx--;
                    }
                }

                // Convert number characters into 
                // a string and return the result.
                return new string(number);
            }
            catch (System.Exception ex)
            {
                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
        }
        #endregion

        #region Public Random Methods
        /// <summary>
        /// Generates a random number and lower case number.
        /// </summary>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        [Export("RandomGenerator")]
        public virtual string Random()
        {
            return RandomEx(DEFAULT_MIN_NUMBER_LENGTH,
                            DEFAULT_MAX_NUMBER_LENGTH);
        }

        /// <summary>
        /// Generates a random number and lower case number.
        /// </summary>
        /// <param name="length">Exact number length.</param>
        /// <returns>Randomly generated number.</returns>
        public virtual string Random(int length)
        {
            return RandomEx(length, length);
        }

        /// <summary>
        /// Generates a random number and lower case number.
        /// </summary>
        /// <param name="minLength">Minimum number length.</param>
        /// <param name="maxLength">Maximum number length.</param>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        public virtual string Random(int minLength, int maxLength)
        {
            return RandomEx(minLength, maxLength);
        }
        #endregion
    }

    /// <summary>
    /// Random number and upper case letter generator class.
    /// </summary>
    [Export(typeof(IRandomGenerator))]
    [ContentMetadata(Name = "NumberUpperCaseGenerator")]
    public class NumberUpperCaseGenerator : IRandomGenerator
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly NumberUpperCaseGenerator Instance = new NumberUpperCaseGenerator();

        /// <summary>
        /// Static constructor
        /// </summary>
        static NumberUpperCaseGenerator() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public NumberUpperCaseGenerator()
        {
        }
        #endregion

        #region Private Constants
        private const string applicationName = "Nequeo";
        private const string eventNamespace = "Nequeo.Invention";
        #endregion

        #region Private Fields
        // Define default min and max number lengths.
        private int DEFAULT_MIN_NUMBER_LENGTH = 8;
        private int DEFAULT_MAX_NUMBER_LENGTH = 10;

        // Define supported number characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private string NUMBER_CHARS_NUMERIC_1 = "ABCDEFGHJKLMNPQRSTWXYZ";
        private string NUMBER_CHARS_NUMERIC_2 = "0123456789";
        private string NUMBER_CHARS_NUMERIC_3 = "ABCDEFGHJKLMNPQRSTWXYZ";
        private string NUMBER_CHARS_NUMERIC_4 = "0123456789";
        #endregion

        #region Private Random Methods
        /// <summary>
        /// Generates a random number and upper case number.
        /// </summary>
        /// <param name="minLength">Minimum number length.</param>
        /// <param name="maxLength">Maximum number length.</param>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        private string RandomEx(int minLength, int maxLength)
        {
            try
            {
                // Make sure that input parameters are valid.
                if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                    return null;

                // Create a local array containing supported number characters
                // grouped by types. You can remove character groups from this
                // array, but doing so will weaken the number strength.
                char[][] charGroups = new char[][] 
                {
                    NUMBER_CHARS_NUMERIC_1.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_2.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_3.ToCharArray(),
                    NUMBER_CHARS_NUMERIC_4.ToCharArray()
                };

                // Use this array to track the number of unused characters in each
                // character group.
                int[] charsLeftInGroup = new int[charGroups.Length];

                // Initially, all characters in each group are not used.
                for (int i = 0; i < charsLeftInGroup.Length; i++)
                    charsLeftInGroup[i] = charGroups[i].Length;

                // Use this array to track (iterate through) unused character groups.
                int[] leftGroupsOrder = new int[charGroups.Length];

                // Initially, all character groups are not used.
                for (int i = 0; i < leftGroupsOrder.Length; i++)
                    leftGroupsOrder[i] = i;

                // Because we cannot use the default randomizer, which is based on the
                // current time (it will produce the same "random" number within a
                // second), we will use a random number generator to seed the
                // randomizer.

                // Use a 4-byte array to fill it with random bytes and convert it then
                // to an integer value.
                byte[] randomBytes = new byte[4];

                // Generate 4 random bytes.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(randomBytes);

                // Convert 4 bytes into a 32-bit integer value.
                int seed = (randomBytes[0] & 0x7f) << 24 |
                            randomBytes[1] << 16 |
                            randomBytes[2] << 8 |
                            randomBytes[3];

                // Now, this is real randomization.
                Random random = new Random(seed);

                // This array will hold number characters.
                char[] number = null;

                // Allocate appropriate memory for the number.
                if (minLength < maxLength)
                    number = new char[random.Next(minLength, maxLength + 1)];
                else
                    number = new char[minLength];

                // Index of the next character to be added to number.
                int nextCharIdx;

                // Index of the next character group to be processed.
                int nextGroupIdx;

                // Index which will be used to track not processed character groups.
                int nextLeftGroupsOrderIdx;

                // Index of the last non-processed character in a group.
                int lastCharIdx;

                // Index of the last non-processed group.
                int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

                // Generate password characters one at a time.
                for (int i = 0; i < number.Length; i++)
                {
                    // If only one character group remained unprocessed, process it;
                    // otherwise, pick a random character group from the unprocessed
                    // group list. To allow a special character to appear in the
                    // first position, increment the second parameter of the Next
                    // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                    if (lastLeftGroupsOrderIdx == 0)
                        nextLeftGroupsOrderIdx = 0;
                    else
                        nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx);

                    // Get the actual index of the character group, from which we will
                    // pick the next character.
                    nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                    // Get the index of the last unprocessed characters in this group.
                    lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                    // If only one unprocessed character is left, pick it; otherwise,
                    // get a random character from the unused character list.
                    if (lastCharIdx == 0)
                        nextCharIdx = 0;
                    else
                        nextCharIdx = random.Next(0, lastCharIdx + 1);

                    // Add this character to the number.
                    number[i] = charGroups[nextGroupIdx][nextCharIdx];

                    // If we processed the last character in this group, start over.
                    if (lastCharIdx == 0)
                        charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                    // There are more unprocessed characters left.
                    else
                    {
                        // Swap processed character with the last unprocessed character
                        // so that we don't pick it until we process all characters in
                        // this group.
                        if (lastCharIdx != nextCharIdx)
                        {
                            char temp = charGroups[nextGroupIdx][lastCharIdx];
                            charGroups[nextGroupIdx][lastCharIdx] =
                                        charGroups[nextGroupIdx][nextCharIdx];
                            charGroups[nextGroupIdx][nextCharIdx] = temp;
                        }
                        // Decrement the number of unprocessed characters in
                        // this group.
                        charsLeftInGroup[nextGroupIdx]--;
                    }

                    // If we processed the last group, start all over.
                    if (lastLeftGroupsOrderIdx == 0)
                        lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                    // There are more unprocessed groups left.
                    else
                    {
                        // Swap processed group with the last unprocessed group
                        // so that we don't pick it until we process all groups.
                        if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                        {
                            int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                            leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                        leftGroupsOrder[nextLeftGroupsOrderIdx];
                            leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                        }
                        // Decrement the number of unprocessed groups.
                        lastLeftGroupsOrderIdx--;
                    }
                }

                // Convert number characters into 
                // a string and return the result.
                return new string(number);
            }
            catch (System.Exception ex)
            {
                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
        }
        #endregion

        #region Public Random Methods
        /// <summary>
        /// Generates a random number and upper case number.
        /// </summary>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        [Export("RandomGenerator")]
        public virtual string Random()
        {
            return RandomEx(DEFAULT_MIN_NUMBER_LENGTH,
                            DEFAULT_MAX_NUMBER_LENGTH);
        }

        /// <summary>
        /// Generates a random number and upper case number.
        /// </summary>
        /// <param name="length">Exact number length.</param>
        /// <returns>Randomly generated number.</returns>
        public virtual string Random(int length)
        {
            return RandomEx(length, length);
        }

        /// <summary>
        /// Generates a random number and upper case number.
        /// </summary>
        /// <param name="minLength">Minimum number length.</param>
        /// <param name="maxLength">Maximum number length.</param>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        public virtual string Random(int minLength, int maxLength)
        {
            return RandomEx(minLength, maxLength);
        }
        #endregion
    }

    /// <summary>
    /// Token generator class.
    /// </summary>
    [Export(typeof(IRandomGenerator))]
    [ContentMetadata(Name = "TokenGenerator")]
    public class TokenGenerator : IRandomGenerator
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly TokenGenerator Instance = new TokenGenerator();

        /// <summary>
        /// Static constructor
        /// </summary>
        static TokenGenerator() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public TokenGenerator()
        {
        }
        #endregion

        #region Private Constants
        private const string applicationName = "Nequeo";
        private const string eventNamespace = "Nequeo.Invention";
        #endregion

        #region Private Fields
        // Define default min and max password lengths.
        private int DEFAULT_MIN_PASSWORD_LENGTH = 8;
        private int DEFAULT_MAX_PASSWORD_LENGTH = 10;

        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
        private string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        private string PASSWORD_CHARS_NUMERIC = "0123456789";
        private string PASSWORD_CHARS_SPECIAL = "0123456789";
        #endregion

        #region Private Random Methods
        /// <summary>
        /// Generates a random token.
        /// </summary>
        /// <param name="minLength">Minimum token length.</param>
        /// <param name="maxLength">Maximum token length.</param>
        /// <returns>Randomly generated token.</returns>
        /// <remarks>
        /// The length of the generated token will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        private string RandomEx(int minLength, int maxLength)
        {
            try
            {
                // Make sure that input parameters are valid.
                if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                    return null;

                // Create a local array containing supported password characters
                // grouped by types. You can remove character groups from this
                // array, but doing so will weaken the password strength.
                char[][] charGroups = new char[][] 
                {
                    PASSWORD_CHARS_LCASE.ToCharArray(),
                    PASSWORD_CHARS_UCASE.ToCharArray(),
                    PASSWORD_CHARS_NUMERIC.ToCharArray(),
                    PASSWORD_CHARS_SPECIAL.ToCharArray()
                };

                // Use this array to track the number of unused characters in each
                // character group.
                int[] charsLeftInGroup = new int[charGroups.Length];

                // Initially, all characters in each group are not used.
                for (int i = 0; i < charsLeftInGroup.Length; i++)
                    charsLeftInGroup[i] = charGroups[i].Length;

                // Use this array to track (iterate through) unused character groups.
                int[] leftGroupsOrder = new int[charGroups.Length];

                // Initially, all character groups are not used.
                for (int i = 0; i < leftGroupsOrder.Length; i++)
                    leftGroupsOrder[i] = i;

                // Because we cannot use the default randomizer, which is based on the
                // current time (it will produce the same "random" number within a
                // second), we will use a random number generator to seed the
                // randomizer.

                // Use a 4-byte array to fill it with random bytes and convert it then
                // to an integer value.
                byte[] randomBytes = new byte[4];

                // Generate 4 random bytes.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(randomBytes);

                // Convert 4 bytes into a 32-bit integer value.
                int seed = (randomBytes[0] & 0x7f) << 24 |
                            randomBytes[1] << 16 |
                            randomBytes[2] << 8 |
                            randomBytes[3];

                // Now, this is real randomization.
                Random random = new Random(seed);

                // This array will hold password characters.
                char[] password = null;

                // Allocate appropriate memory for the password.
                if (minLength < maxLength)
                    password = new char[random.Next(minLength, maxLength + 1)];
                else
                    password = new char[minLength];

                // Index of the next character to be added to password.
                int nextCharIdx;

                // Index of the next character group to be processed.
                int nextGroupIdx;

                // Index which will be used to track not processed character groups.
                int nextLeftGroupsOrderIdx;

                // Index of the last non-processed character in a group.
                int lastCharIdx;

                // Index of the last non-processed group.
                int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

                // Generate password characters one at a time.
                for (int i = 0; i < password.Length; i++)
                {
                    // If only one character group remained unprocessed, process it;
                    // otherwise, pick a random character group from the unprocessed
                    // group list. To allow a special character to appear in the
                    // first position, increment the second parameter of the Next
                    // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                    if (lastLeftGroupsOrderIdx == 0)
                        nextLeftGroupsOrderIdx = 0;
                    else
                        nextLeftGroupsOrderIdx = random.Next(0, lastLeftGroupsOrderIdx);

                    // Get the actual index of the character group, from which we will
                    // pick the next character.
                    nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                    // Get the index of the last unprocessed characters in this group.
                    lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                    // If only one unprocessed character is left, pick it; otherwise,
                    // get a random character from the unused character list.
                    if (lastCharIdx == 0)
                        nextCharIdx = 0;
                    else
                        nextCharIdx = random.Next(0, lastCharIdx + 1);

                    // Add this character to the password.
                    password[i] = charGroups[nextGroupIdx][nextCharIdx];

                    // If we processed the last character in this group, start over.
                    if (lastCharIdx == 0)
                        charsLeftInGroup[nextGroupIdx] = charGroups[nextGroupIdx].Length;
                    // There are more unprocessed characters left.
                    else
                    {
                        // Swap processed character with the last unprocessed character
                        // so that we don't pick it until we process all characters in
                        // this group.
                        if (lastCharIdx != nextCharIdx)
                        {
                            char temp = charGroups[nextGroupIdx][lastCharIdx];
                            charGroups[nextGroupIdx][lastCharIdx] =
                                        charGroups[nextGroupIdx][nextCharIdx];
                            charGroups[nextGroupIdx][nextCharIdx] = temp;
                        }
                        // Decrement the number of unprocessed characters in
                        // this group.
                        charsLeftInGroup[nextGroupIdx]--;
                    }

                    // If we processed the last group, start all over.
                    if (lastLeftGroupsOrderIdx == 0)
                        lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                    // There are more unprocessed groups left.
                    else
                    {
                        // Swap processed group with the last unprocessed group
                        // so that we don't pick it until we process all groups.
                        if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                        {
                            int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                            leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                        leftGroupsOrder[nextLeftGroupsOrderIdx];
                            leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                        }
                        // Decrement the number of unprocessed groups.
                        lastLeftGroupsOrderIdx--;
                    }
                }

                // Convert password characters into 
                // a string and return the result.
                return new string(password);
            }
            catch (System.Exception ex)
            {
                // Throw a general exception.
                throw new System.Exception(ex.Message, ex.InnerException);
            }
        }
        #endregion

        #region Public Random Methods
        /// <summary>
        /// Generates a random token.
        /// </summary>
        /// <returns>Randomly generated token.</returns>
        /// <remarks>
        /// The length of the generated token will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        [Export("RandomGenerator")]
        public virtual string Random()
        {
            return RandomEx(DEFAULT_MIN_PASSWORD_LENGTH,
                            DEFAULT_MAX_PASSWORD_LENGTH);
        }

        /// <summary>
        /// Generates a random token of the exact length.
        /// </summary>
        /// <param name="length">Exact token length.</param>
        /// <returns>Randomly generated token.</returns>
        public virtual string Random(int length)
        {
            return RandomEx(length, length);
        }

        /// <summary>
        /// Generates a random token.
        /// </summary>
        /// <param name="minLength">Minimum token length.</param>
        /// <param name="maxLength">Maximum token length.</param>
        /// <returns>Randomly generated token.</returns>
        /// <remarks>
        /// The length of the generated token will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        public virtual string Random(int minLength, int maxLength)
        {
            return RandomEx(minLength, maxLength);
        }
        #endregion
    }

    /// <summary>
    /// Random charactor generator interface.
    /// </summary>
    public interface IRandomGenerator
    {
        #region Public Random Methods
        /// <summary>
        /// Generates a random number and upper case number.
        /// </summary>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random. It will be no shorter than the minimum default and
        /// no longer than maximum default.
        /// </remarks>
        string Random();

        /// <summary>
        /// Generates a random number and upper case number.
        /// </summary>
        /// <param name="length">Exact number length.</param>
        /// <returns>Randomly generated number.</returns>
        string Random(int length);

        /// <summary>
        /// Generates a random number and upper case number.
        /// </summary>
        /// <param name="minLength">Minimum number length.</param>
        /// <param name="maxLength">Maximum number length.</param>
        /// <returns>Randomly generated number.</returns>
        /// <remarks>
        /// The length of the generated number will be determined at
        /// random and it will fall with the range determined by the
        /// function parameters.
        /// </remarks>
        string Random(int minLength, int maxLength);

        #endregion
    }
}
