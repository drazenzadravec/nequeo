using Nequeo.Cryptography.Key.Asn1;
using Nequeo.Cryptography.Key.Asn1.Pkcs;

namespace Nequeo.Cryptography.Key.Asn1.Smime
{
    public abstract class SmimeAttributes
    {
        public static readonly DerObjectIdentifier SmimeCapabilities = PkcsObjectIdentifiers.Pkcs9AtSmimeCapabilities;
        public static readonly DerObjectIdentifier EncrypKeyPref = PkcsObjectIdentifiers.IdAAEncrypKeyPref;
    }
}
