/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :
*  Purpose :
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

//////////////////////////////////////////////////////////////////////////
//  CritSec
//  Description: Wraps a critical section.
//////////////////////////////////////////////////////////////////////////

class CritSec
{
public:
    CRITICAL_SECTION m_criticalSection;
public:
    CritSec()
    {
        InitializeCriticalSectionEx(&m_criticalSection, 100, 0);
    }

    ~CritSec()
    {
        DeleteCriticalSection(&m_criticalSection);
    }

	_Acquires_lock_(m_criticalSection)
    void Lock()
    {
        EnterCriticalSection(&m_criticalSection);
    }

	_Releases_lock_(m_criticalSection)
    void Unlock()
    {
        LeaveCriticalSection(&m_criticalSection);
    }
};


//////////////////////////////////////////////////////////////////////////
//  AutoLock
//  Description: Provides automatic locking and unlocking of a 
//               of a critical section.
//
//  Note: The AutoLock object must go out of scope before the CritSec.
//////////////////////////////////////////////////////////////////////////

class AutoLock
{
private:
    CritSec *m_pCriticalSection;
public:
	_Acquires_lock_(m_pCriticalSection)
    AutoLock(CritSec& crit)
    {
        m_pCriticalSection = &crit;
        m_pCriticalSection->Lock();
    }

	_Releases_lock_(m_pCriticalSection)
    ~AutoLock()
    {
	    m_pCriticalSection->Unlock();
    }
};
