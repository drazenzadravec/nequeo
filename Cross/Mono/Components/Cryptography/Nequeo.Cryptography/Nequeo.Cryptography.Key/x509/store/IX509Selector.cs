using System;

namespace Nequeo.Cryptography.Key.X509.Store
{
	public interface IX509Selector
#if !SILVERLIGHT
		: ICloneable
#endif
	{
#if SILVERLIGHT
        object Clone();
#endif
        bool Match(object obj);
	}
}
