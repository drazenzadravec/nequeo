/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          TextEncoding.cpp
*  Purpose :       TextEncoding header.
*
*/

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

#include "stdafx.h"

#include "TextEncoding.h"
#include "ASCIIEncoding.h"
#include "UTF8Encoding.h"
#include "Latin1Encoding.h"
#include "Latin9Encoding.h"
#include "UTF16Encoding.h"
#include "Windows1252Encoding.h"
#include "Threading\RWLock.h"
#include "Threading\SingletonHolder.h"
#include "Exceptions\Exception.h"
#include "Exceptions\ExceptionCode.h"
#include <map>

using Nequeo::Threading::RWLock;

namespace Nequeo{
	namespace IO{
		namespace Text
		{
			class TextEncodingManager
			{
			public:
				TextEncodingManager()
				{
					TextEncoding::Ptr pUtf8Encoding(new UTF8Encoding);
					add(pUtf8Encoding, TextEncoding::GLOBAL);

					add(new ASCIIEncoding());
					add(new Latin1Encoding);
					add(new Latin9Encoding);
					add(pUtf8Encoding);
					add(new UTF16Encoding);
					add(new Windows1252Encoding);
				}

				~TextEncodingManager()
				{
				}

				void add(TextEncoding::Ptr pEncoding)
				{
					add(pEncoding, pEncoding->canonicalName());
				}

				void add(TextEncoding::Ptr pEncoding, const std::string& name)
				{
					RWLock::ScopedLock lock(_lock, true);

					_encodings[name] = pEncoding;
				}

				void remove(const std::string& name)
				{
					RWLock::ScopedLock lock(_lock, true);

					_encodings.erase(name);
				}

				TextEncoding::Ptr find(const std::string& name) const
				{
					RWLock::ScopedLock lock(_lock);

					EncodingMap::const_iterator it = _encodings.find(name);
					if (it != _encodings.end())
						return it->second;

					for (it = _encodings.begin(); it != _encodings.end(); ++it)
					{
						if (it->second->isA(name))
							return it->second;
					}
					return TextEncoding::Ptr();
				}

			private:
				TextEncodingManager(const TextEncodingManager&);
				TextEncodingManager& operator = (const TextEncodingManager&);

				struct ILT
				{
					bool operator() (const std::string& s1, const std::string& s2) const
					{
						return Nequeo::icompare(s1, s2) < 0;
					}
				};

				typedef std::map<std::string, TextEncoding::Ptr, ILT> EncodingMap;

				EncodingMap    _encodings;
				mutable RWLock _lock;
			};


			//
			// TextEncoding
			//


			const std::string TextEncoding::GLOBAL;


			TextEncoding::~TextEncoding()
			{
			}


			int TextEncoding::convert(const unsigned char* bytes) const
			{
				return static_cast<int>(*bytes);
			}


			int TextEncoding::convert(int ch, unsigned char* bytes, int length) const
			{
				return 0;
			}


			int TextEncoding::queryConvert(const unsigned char* bytes, int length) const
			{
				return (int)*bytes;
			}


			int TextEncoding::sequenceLength(const unsigned char* bytes, int length) const
			{
				return 1;
			}


			TextEncoding& TextEncoding::byName(const std::string& encodingName)
			{
				TextEncoding* pEncoding = manager().find(encodingName);
				if (pEncoding)
					return *pEncoding;
				else
					throw Nequeo::Exceptions::NotFoundException(encodingName);
			}


			TextEncoding::Ptr TextEncoding::find(const std::string& encodingName)
			{
				return manager().find(encodingName);
			}


			void TextEncoding::add(TextEncoding::Ptr pEncoding)
			{
				manager().add(pEncoding, pEncoding->canonicalName());
			}


			void TextEncoding::add(TextEncoding::Ptr pEncoding, const std::string& name)
			{
				manager().add(pEncoding, name);
			}


			void TextEncoding::remove(const std::string& encodingName)
			{
				manager().remove(encodingName);
			}


			TextEncoding::Ptr TextEncoding::global(TextEncoding::Ptr encoding)
			{
				TextEncoding::Ptr prev = find(GLOBAL);
				add(encoding, GLOBAL);
				return prev;
			}


			TextEncoding& TextEncoding::global()
			{
				return byName(GLOBAL);
			}


			namespace
			{
				static Nequeo::Threading::SingletonHolder<TextEncodingManager> sh;
			}


			TextEncodingManager& TextEncoding::manager()
			{
				return *sh.get();
			}
		}
	}
}