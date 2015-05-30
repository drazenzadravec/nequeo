using System;

using Nequeo.Cryptography.Key.Utilities.IO;

namespace Nequeo.Cryptography.Key.Cms
{
	internal class NullOutputStream
		: BaseOutputStream
	{
		public override void WriteByte(byte b)
		{
			// do nothing
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			// do nothing
		}
	}
}
