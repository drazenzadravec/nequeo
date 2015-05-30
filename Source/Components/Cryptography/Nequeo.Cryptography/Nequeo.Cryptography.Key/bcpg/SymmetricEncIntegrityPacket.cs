using System;
using System.IO;

namespace Nequeo.Cryptography.Key.Bcpg
{
	public class SymmetricEncIntegrityPacket
		: InputStreamPacket
	{
		internal readonly int version;

		internal SymmetricEncIntegrityPacket(
			BcpgInputStream bcpgIn)
			: base(bcpgIn)
        {
			version = bcpgIn.ReadByte();
        }
    }
}
