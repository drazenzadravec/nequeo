using System.IO;

namespace Nequeo.Cryptography.Key.Asn1
{
	public interface Asn1OctetStringParser
		: IAsn1Convertible
	{
		Stream GetOctetStream();
	}
}
