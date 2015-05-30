using System;
using System.IO;

namespace Nequeo.Cryptography.Key.Cms
{
	internal interface CmsReadable
	{
		Stream GetInputStream();
	}
}
