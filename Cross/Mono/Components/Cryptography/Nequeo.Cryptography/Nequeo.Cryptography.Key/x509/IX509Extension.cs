using System;

using Nequeo.Cryptography.Key.Asn1;
using Nequeo.Cryptography.Key.Utilities.Collections;

namespace Nequeo.Cryptography.Key.X509
{
	public interface IX509Extension
	{
		/// <summary>
		/// Get all critical extension values, by oid
		/// </summary>
		/// <returns>IDictionary with string (OID) keys and Asn1OctetString values</returns>
		ISet GetCriticalExtensionOids();

		/// <summary>
		/// Get all non-critical extension values, by oid
		/// </summary>
		/// <returns>IDictionary with string (OID) keys and Asn1OctetString values</returns>
		ISet GetNonCriticalExtensionOids();

		[Obsolete("Use version taking a DerObjectIdentifier instead")]
		Asn1OctetString GetExtensionValue(string oid);

		Asn1OctetString GetExtensionValue(DerObjectIdentifier oid);
	}
}
