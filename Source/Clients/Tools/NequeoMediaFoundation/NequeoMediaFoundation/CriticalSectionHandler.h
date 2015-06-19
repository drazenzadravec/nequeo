/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CriticalSectionHandler.h
*  Purpose :       CriticalSectionHandler class.
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

#pragma once

#ifndef _CRITICALSECTIONHANDLER_H
#define _CRITICALSECTIONHANDLER_H

#include "MediaGlobal.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Critical section handler.
			/// </summary>
			class CriticalSectionHandler
			{
			private:
				CRITICAL_SECTION _criticalSection;
			public:
				/// <summary>
				/// Constructor for the current class.
				/// </summary>
				CriticalSectionHandler()
				{
					InitializeCriticalSection(&_criticalSection);
				}

				/// <summary>
				/// This destructor.
				/// </summary>
				~CriticalSectionHandler()
				{
					DeleteCriticalSection(&_criticalSection);
				}

				/// <summary>
				/// Enter critical section.
				/// </summary>
				void Lock()
				{
					EnterCriticalSection(&_criticalSection);
				}

				/// <summary>
				/// Leave critical section.
				/// </summary>
				void Unlock()
				{
					LeaveCriticalSection(&_criticalSection);
				}
			};

			/// <summary>
			/// Auto lock handler.
			/// </summary>
			class AutoLockHandler
			{
			private:
				CriticalSectionHandler *_pCriticalSection;
			public:
				/// <summary>
				/// Constructor for the current class.
				/// </summary>
				/// <param name="crit">The critical section handler reference.</param>
				AutoLockHandler(CriticalSectionHandler& crit)
				{
					_pCriticalSection = &crit;
					_pCriticalSection->Lock();
				}

				/// <summary>
				/// This destructor.
				/// </summary>
				~AutoLockHandler()
				{
					_pCriticalSection->Unlock();
				}
			};
		}
	}
}
#endif