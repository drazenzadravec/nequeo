/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Executor.cpp
*  Purpose :       Executor class.
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

#include "Executor.h"
#include "ThreadTask.h"

using namespace Nequeo::Threading;

static const char* POOLED_CLASS_TAG = "PooledThreadExecutor";

bool DefaultExecutor::SubmitToThread(std::function<void()>&&  fx)
{
	std::thread t(fx);
	t.detach();
	return true;
}

PooledThreadExecutor::PooledThreadExecutor(size_t poolSize, OverflowPolicy overflowPolicy) :
	m_poolSize(poolSize), m_overflowPolicy(overflowPolicy)
{
	for (size_t index = 0; index < m_poolSize; ++index)
	{
		m_threadTaskHandles.push_back(Nequeo::New<ThreadTask>(POOLED_CLASS_TAG, *this));
	}
}

PooledThreadExecutor::~PooledThreadExecutor()
{
	for (auto threadTask : m_threadTaskHandles)
	{
		threadTask->StopProcessingWork();
	}

	m_syncPoint.notify_all();

	for (auto threadTask : m_threadTaskHandles)
	{
		Nequeo::Delete(threadTask);
	}

	while (m_tasks.size() > 0)
	{
		std::function<void()>* fn = m_tasks.front();
		m_tasks.pop();

		if (fn)
		{
			Nequeo::Delete(fn);
		}
	}

}

bool PooledThreadExecutor::SubmitToThread(std::function<void()>&& fn)
{
	//avoid the need to do copies inside the lock. Instead lets do a pointer push.
	std::function<void()>* fnCpy = Nequeo::New<std::function<void()>>(POOLED_CLASS_TAG, std::forward<std::function<void()>>(fn));

	{
		std::lock_guard<std::mutex> locker(m_queueLock);

		if (m_overflowPolicy == OverflowPolicy::REJECT_IMMEDIATELY && m_poolSize >= m_tasks.size())
		{
			return false;
		}

		m_tasks.push(fnCpy);
	}

	m_syncPoint.notify_one();

	return true;
}

std::function<void()>* PooledThreadExecutor::PopTask()
{
	std::lock_guard<std::mutex> locker(m_queueLock);

	if (m_tasks.size() > 0)
	{
		std::function<void()>* fn = m_tasks.front();
		if (fn)
		{
			m_tasks.pop();
			return fn;
		}
	}

	return nullptr;
}

bool PooledThreadExecutor::HasTasks()
{
	std::lock_guard<std::mutex> locker(m_queueLock);
	return m_tasks.size() > 0;
}