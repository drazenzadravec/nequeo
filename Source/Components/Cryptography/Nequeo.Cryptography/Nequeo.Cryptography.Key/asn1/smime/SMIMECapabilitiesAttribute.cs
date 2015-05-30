using Nequeo.Cryptography.Key.Asn1;
using Nequeo.Cryptography.Key.Asn1.X509;

namespace Nequeo.Cryptography.Key.Asn1.Smime
{
    public class SmimeCapabilitiesAttribute
        : AttributeX509
    {
        public SmimeCapabilitiesAttribute(
            SmimeCapabilityVector capabilities)
            : base(SmimeAttributes.SmimeCapabilities,
                    new DerSet(new DerSequence(capabilities.ToAsn1EncodableVector())))
        {
        }
    }
}
