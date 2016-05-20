/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
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
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Nequeo.Cryptography
{
    /// <summary>
    /// Cryptographic Random Derived Key Generator (PBKDF2).
    /// </summary>
    public class RandomDerivedKey
    {
        /// <summary>
        /// Internal common salt.
        /// </summary>
        private static byte[] _salt = Encoding.Default.GetBytes("0Qr=K/6x2Yd-*p9LR%m74E{i?yN8s1+J");

        /// <summary>
        /// Generate a random salt.
        /// </summary>
        /// <param name="minimum">The minimum length of the salt.</param>
        /// <param name="maximum">The minimum length of the salt.</param>
        /// <returns>The random salt value.</returns>
        public static byte[] GenerateSalt(int minimum = 32, int maximum = 32)
        {
            // Generate a random salt.
            Nequeo.Cryptography.RandomPassword salt = new Nequeo.Cryptography.RandomPassword();
            string saltString = salt.Generate(minimum, maximum);
            return Encoding.Default.GetBytes(saltString);
        }

        /// <summary>
        /// Generate the derived key.
        /// </summary>
        /// <param name="sharedKey">The shared key used with the common salt.</param>
        /// <param name="keySize">The derived key size to generate.</param>
        /// <returns>The derived key.</returns>
        public static byte[] Generate(string sharedKey, int keySize = 32)
        {
            // Generate the key from the shared secret and the salt
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedKey, _salt);
            return key.GetBytes(keySize);
        }

        /// <summary>
        /// Generate the derived key.
        /// </summary>
        /// <param name="sharedKey">The shared key used with the common salt.</param>
        /// <param name="keySize">The derived key size to generate.</param>
        /// <param name="iterations">The number of iterations.</param>
        /// <returns>The derived key.</returns>
        public static byte[] GenerateEx(string sharedKey, int keySize = 32, int iterations = 5000)
        {
            // Generate the key from the shared secret and the salt
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedKey, _salt, iterations);
            return key.GetBytes(keySize);
        }

        /// <summary>
        /// Generate the derived key.
        /// </summary>
        /// <param name="sharedKey">The shared key used with the common salt.</param>
        /// <param name="salt">The common salt used to generate the key.</param>
        /// <param name="keySize">The derived key size to generate.</param>
        /// <returns>The derived key.</returns>
        public static byte[] Generate(string sharedKey, byte[] salt, int keySize = 32)
        {
            // Generate the key from the shared secret and the salt
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedKey, salt);
            return key.GetBytes(keySize);
        }

        /// <summary>
        /// Generate the derived key.
        /// </summary>
        /// <param name="sharedKey">The shared key used with the common salt.</param>
        /// <param name="salt">The common salt used to generate the key.</param>
        /// <param name="keySize">The derived key size to generate.</param>
        /// <param name="iterations">The number of iterations.</param>
        /// <returns>The derived key.</returns>
        public static byte[] GenerateEx(string sharedKey, byte[] salt, int keySize = 32, int iterations = 5000)
        {
            // Generate the key from the shared secret and the salt
            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedKey, salt, iterations);
            return key.GetBytes(keySize);
        }
    }

    /// <summary>
    /// Cryptographic Random Number Generator (RNG).
    /// </summary>
    public class RandomNumber
    {
        /// <summary>
        /// Generate the random number.
        /// </summary>
        /// <param name="keySize">The size of the number.</param>
        /// <param name="nonZeroBytes">True to generate a sequence of random nonzero values; else false.</param>
        /// <returns>The array of numbers.</returns>
        public static byte[] Generate(long keySize, bool nonZeroBytes = false)
        {
            // Get the random number generator service provider.
            byte[] data = new byte[keySize];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            // If non zero bytes.
            if (nonZeroBytes)
                rng.GetNonZeroBytes(data);
            else
                rng.GetBytes(data);

            // Return the array of numbers.
            return data;
        }

        /// <summary>
        /// Generate the random number.
        /// </summary>
        /// <param name="keySize">The size of the number.</param>
        /// <param name="nonZeroBytes">True to generate a sequence of random nonzero values; else false.</param>
        /// <returns>The number.</returns>
        public static short GenerateShort(short keySize, bool nonZeroBytes = false)
        {
            // Get the random number generator service provider.
            byte[] data = Generate(keySize, nonZeroBytes);
            string number = Encoding.Default.GetString(data);

            // Return the number.
            return Int16.Parse(number);
        }

        /// <summary>
        /// Generate the random number.
        /// </summary>
        /// <param name="keySize">The size of the number.</param>
        /// <param name="nonZeroBytes">True to generate a sequence of random nonzero values; else false.</param>
        /// <returns>The number.</returns>
        public static int GenerateInt(int keySize, bool nonZeroBytes = false)
        {
            // Get the random number generator service provider.
            byte[] data = Generate(keySize, nonZeroBytes);
            string number = Encoding.Default.GetString(data);

            // Return the number.
            return Int32.Parse(number);
        }

        /// <summary>
        /// Generate the random number.
        /// </summary>
        /// <param name="keySize">The size of the number.</param>
        /// <param name="nonZeroBytes">True to generate a sequence of random nonzero values; else false.</param>
        /// <returns>The number.</returns>
        public static long GenerateLong(long keySize, bool nonZeroBytes = false)
        {
            // Get the random number generator service provider.
            byte[] data = Generate(keySize, nonZeroBytes);
            string number = Encoding.Default.GetString(data);

            // Return the number.
            return Int64.Parse(number);
        }
    }

    /// <summary>
    /// Cryptographic Random Password Generator (RNG).
    /// </summary>
    public class RandomPassword
    {
        #region Constructors
        /// <summary>
        /// Create a new static instance
        /// </summary>
        public static readonly RandomPassword Instance = new RandomPassword();

        /// <summary>
        /// Static constructor
        /// </summary>
        static RandomPassword() { }  // Trigger lazy initialization of static fields

        /// <summary>
        /// Constructor for the current class.
        /// </summary>
        public RandomPassword()
        {
        }
        #endregion

        #region Private Fields
        // Define default min and max password lengths.
        private int DEFAULT_MIN_PASSWORD_LENGTH = 32;
        private int DEFAULT_MAX_PASSWORD_LENGTH = 32;

        // Define supported password characters divided into groups.
        // You can add (or remove) characters to (from) these groups.
        private string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
        private string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        private string PASSWORD_CHARS_NUMERIC = "0123456789";
        private string PASSWORD_CHARS_SPECIAL = "~^*$-+?_&=!%{}/#[]()<>.,";
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
        public virtual string Generate()
        {
            return RandomEx(DEFAULT_MIN_PASSWORD_LENGTH,
                            DEFAULT_MAX_PASSWORD_LENGTH);
        }

        /// <summary>
        /// Generates a random password of the exact length.
        /// </summary>
        /// <param name="length">Exact password length.</param>
        /// <returns>Randomly generated password.</returns>
        public virtual string Generate(int length)
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
        public virtual string Generate(int minLength, int maxLength)
        {
            return RandomEx(minLength, maxLength);
        }
        #endregion
    }
}
