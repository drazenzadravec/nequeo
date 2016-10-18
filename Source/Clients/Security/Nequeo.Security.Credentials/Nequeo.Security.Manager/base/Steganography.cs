/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
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
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace Nequeo.Security
{
	/// <summary>
	/// Steganography implementation.
	/// </summary>
	public sealed class Steganography
	{
		/// <summary>
		/// Steganography implementation.
		/// </summary>
		public Steganography()
		{
		}

		/// <summary>
		/// Embed the text into the image.
		/// </summary>
		/// <param name="text">The text to embed.</param>
		/// <param name="image">The image to embed the text into.</param>
		/// <param name="sharedKey">The shared key used with the common salt.</param>
		/// <returns>The image containing the embedded text.</returns>
		public Bitmap EmbedText(string text, Bitmap image, string sharedKey)
		{
			// Create a new derived key.
			byte[] cryptoKey = Nequeo.Cryptography.RandomDerivedKey.Generate(sharedKey);
			Nequeo.Cryptography.AdvancedAES aes = new Cryptography.AdvancedAES();

			// Encrypt the key.
			byte[] encryptedText = aes.EncryptToMemory(Encoding.Default.GetBytes(text), cryptoKey);
			string textBase64 = Nequeo.Conversion.Context.ByteArrayToHexString(encryptedText);

			// Create the image.
			return Nequeo.Drawing.TextEmbedding.EmbedText(textBase64, image);
		}

		/// <summary>
		/// Embed the text into the image.
		/// </summary>
		/// <param name="text">The text to embed.</param>
		/// <param name="image">The image to embed the text into.</param>
		/// <param name="sharedKey">The shared key used with the common salt.</param>
		/// <param name="salt">The common salt used to generate the key.</param>
		/// <returns>The image containing the embedded text.</returns>
		public Bitmap EmbedText(string text, Bitmap image, string sharedKey, byte[] salt)
		{
			// Create a new derived key.
			byte[] cryptoKey = Nequeo.Cryptography.RandomDerivedKey.Generate(sharedKey, salt);
			Nequeo.Cryptography.AdvancedAES aes = new Cryptography.AdvancedAES();

			// Encrypt the key.
			byte[] encryptedText = aes.EncryptToMemory(Encoding.Default.GetBytes(text), cryptoKey);
			string textBase64 = Nequeo.Conversion.Context.ByteArrayToHexString(encryptedText);

			// Create the image.
			return Nequeo.Drawing.TextEmbedding.EmbedText(textBase64, image);
		}

		/// <summary>
		/// Embed the text into the image.
		/// </summary>
		/// <param name="text">The text to embed.</param>
		/// <param name="image">The image stream to embed the text into.</param>
		/// <param name="sharedKey">The shared key used with the common salt.</param>
		/// <returns>The image containing the embedded text.</returns>
		public Bitmap EmbedText(string text, Stream image, string sharedKey)
		{
			// Create a new derived key.
			byte[] cryptoKey = Nequeo.Cryptography.RandomDerivedKey.Generate(sharedKey);
			Nequeo.Cryptography.AdvancedAES aes = new Cryptography.AdvancedAES();

			// Encrypt the key.
			byte[] encryptedText = aes.EncryptToMemory(Encoding.Default.GetBytes(text), cryptoKey);
			string textBase64 = Nequeo.Conversion.Context.ByteArrayToHexString(encryptedText);

			// Create the image.
			Bitmap bmp = new Bitmap(image);
			return Nequeo.Drawing.TextEmbedding.EmbedText(textBase64, bmp);
		}

		/// <summary>
		/// Embed the text into the image.
		/// </summary>
		/// <param name="text">The text to embed.</param>
		/// <param name="image">The image stream to embed the text into.</param>
		/// <param name="sharedKey">The shared key used with the common salt.</param>
		/// <param name="salt">The common salt used to generate the key.</param>
		/// <returns>The image containing the embedded text.</returns>
		public Bitmap EmbedText(string text, Stream image, string sharedKey, byte[] salt)
		{
			// Create a new derived key.
			byte[] cryptoKey = Nequeo.Cryptography.RandomDerivedKey.Generate(sharedKey, salt);
			Nequeo.Cryptography.AdvancedAES aes = new Cryptography.AdvancedAES();

			// Encrypt the key.
			byte[] encryptedText = aes.EncryptToMemory(Encoding.Default.GetBytes(text), cryptoKey);
			string textBase64 = Nequeo.Conversion.Context.ByteArrayToHexString(encryptedText);

			// Create the image.
			Bitmap bmp = new Bitmap(image);
			return Nequeo.Drawing.TextEmbedding.EmbedText(textBase64, bmp);
		}

		/// <summary>
		/// Extract the text from the image.
		/// </summary>
		/// <param name="image">The image that contains the text to extract.</param>
		/// <param name="sharedKey">The shared key used with the common salt.</param>
		/// <returns>The extracted text.</returns>
		public string ExtractText(Bitmap image, string sharedKey)
		{
			// Create a new derived key.
			byte[] cryptoKey = Nequeo.Cryptography.RandomDerivedKey.Generate(sharedKey);
			Nequeo.Cryptography.AdvancedAES aes = new Cryptography.AdvancedAES();

			// Extract the text.
			string textBase64 = Nequeo.Drawing.TextEmbedding.ExtractText(image);
			byte[] encryptedText = Nequeo.Conversion.Context.HexStringToByteArray(textBase64);
			byte[] decryptedText = aes.DecryptFromMemory(encryptedText, cryptoKey);

			// Return the text.
			return Encoding.Default.GetString(decryptedText);
		}

		/// <summary>
		/// Extract the text from the image.
		/// </summary>
		/// <param name="image">The image that contains the text to extract.</param>
		/// <param name="sharedKey">The shared key used with the common salt.</param>
		/// <param name="salt">The common salt used to generate the key.</param>
		/// <returns>The extracted text.</returns>
		public string ExtractText(Bitmap image, string sharedKey, byte[] salt)
		{
			// Create a new derived key.
			byte[] cryptoKey = Nequeo.Cryptography.RandomDerivedKey.Generate(sharedKey, salt);
			Nequeo.Cryptography.AdvancedAES aes = new Cryptography.AdvancedAES();

			// Extract the text.
			string textBase64 = Nequeo.Drawing.TextEmbedding.ExtractText(image);
			byte[] encryptedText = Nequeo.Conversion.Context.HexStringToByteArray(textBase64);
			byte[] decryptedText = aes.DecryptFromMemory(encryptedText, cryptoKey);

			// Return the text.
			return Encoding.Default.GetString(decryptedText);
		}

		/// <summary>
		/// Extract the text from the image.
		/// </summary>
		/// <param name="image">The image stream that contains the text to extract.</param>
		/// <param name="sharedKey">The shared key used with the common salt.</param>
		/// <returns>The extracted text.</returns>
		public string ExtractText(Stream image, string sharedKey)
		{
			// Create a new derived key.
			byte[] cryptoKey = Nequeo.Cryptography.RandomDerivedKey.Generate(sharedKey);
			Nequeo.Cryptography.AdvancedAES aes = new Cryptography.AdvancedAES();

			// Extract the text.
			Bitmap bmp = new Bitmap(image);
			string textBase64 = Nequeo.Drawing.TextEmbedding.ExtractText(bmp);
			byte[] encryptedText = Nequeo.Conversion.Context.HexStringToByteArray(textBase64);
			byte[] decryptedText = aes.DecryptFromMemory(encryptedText, cryptoKey);

			// Return the text.
			return Encoding.Default.GetString(decryptedText);
		}

		/// <summary>
		/// Extract the text from the image.
		/// </summary>
		/// <param name="image">The stream image that contains the text to extract.</param>
		/// <param name="sharedKey">The shared key used with the common salt.</param>
		/// <param name="salt">The common salt used to generate the key.</param>
		/// <returns>The extracted text.</returns>
		public string ExtractText(Stream image, string sharedKey, byte[] salt)
		{
			// Create a new derived key.
			byte[] cryptoKey = Nequeo.Cryptography.RandomDerivedKey.Generate(sharedKey, salt);
			Nequeo.Cryptography.AdvancedAES aes = new Cryptography.AdvancedAES();

			// Extract the text.
			Bitmap bmp = new Bitmap(image);
			string textBase64 = Nequeo.Drawing.TextEmbedding.ExtractText(bmp);
			byte[] encryptedText = Nequeo.Conversion.Context.HexStringToByteArray(textBase64);
			byte[] decryptedText = aes.DecryptFromMemory(encryptedText, cryptoKey);

			// Return the text.
			return Encoding.Default.GetString(decryptedText);
		}
	}
}
