/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          ThreadTask.cpp
*  Purpose :       ThreadTask class.
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

#include "ThreadTask.h"
#include "Executor.h"

using namespace Nequeo::Threading;

ThreadTask::ThreadTask(PooledThreadExecutor& executor) : 
	m_continue(true), m_executor(executor), m_thread(std::bind(&ThreadTask::MainTaskRunner, this))
{
}

ThreadTask::~ThreadTask()
{
	StopProcessingWork();
	m_thread.join();
}

void ThreadTask::MainTaskRunner()
{
	while (m_continue)
	{
		while (m_continue && m_executor.HasTasks())
		{
			auto fn = m_executor.PopTask();
			if (fn)
			{
				(*fn)();
				Nequeo::Delete(fn);
			}
		}

		std::unique_lock<std::mutex> locker(m_executor.m_syncPointLock);
		if (m_continue)
		{
			m_executor.m_syncPoint.wait(locker);
		}
	}
}

void ThreadTask::StopProcessingWork()
{
	m_continue = false;
}
