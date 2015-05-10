/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2014 http://www.nequeo.com.au/
*
*  File :          Unicode.cpp
*  Purpose :       Unicode header.
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

#include "Unicode.h"

extern "C"
{
#include "base\pcre_config.h"
#include "base\pcre_internal.h"
}

namespace Nequeo{
	namespace IO{
		namespace Text
		{
			void Unicode::properties(int ch, CharacterProperties& props)
			{
				if (ch > UCP_MAX_CODEPOINT) ch = 0;
				const ucd_record* ucd = GET_UCD(ch);
				props.category = static_cast<CharacterCategory>(_pcre_ucp_gentype[ucd->chartype]);
				props.type = static_cast<CharacterType>(ucd->chartype);
				props.script = static_cast<Script>(ucd->script);
			}


			int Unicode::toLower(int ch)
			{
				if (isUpper(ch))
					return static_cast<int>(UCD_OTHERCASE(static_cast<unsigned>(ch)));
				else
					return ch;
			}


			int Unicode::toUpper(int ch)
			{
				if (isLower(ch))
					return static_cast<int>(UCD_OTHERCASE(static_cast<unsigned>(ch)));
				else
					return ch;
			}
		}
	}
}