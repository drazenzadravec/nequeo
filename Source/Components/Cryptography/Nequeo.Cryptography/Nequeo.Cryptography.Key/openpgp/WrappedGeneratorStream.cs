using System.IO;

using Nequeo.Cryptography.Key.Asn1.Utilities;

namespace Nequeo.Cryptography.Key.Bcpg.OpenPgp
{
	public class WrappedGeneratorStream
		: FilterStream
	{
		private readonly IStreamGenerator gen;

		public WrappedGeneratorStream(
			IStreamGenerator	gen,
			Stream				str)
			: base(str)
		{
			this.gen = gen;
		}

		public override void Close()
		{
			gen.Close();
		}
	}
}
