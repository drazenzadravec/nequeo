/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          Executor.h
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

#pragma once

#include "GlobalThreading.h"
#include "Base\Allocator.h"
#include "Base\FunctionTemplates.h"

#include <functional>
#include <mutex>
#include <condition_variable>

#define NEQUEO_THREADING_BUILD_FUNCTION(func) Nequeo::FunctionBuild(func)
#define NEQUEO_THREADING_BUILD_TYPED_FUNCTION(func, type) Nequeo::FunctionBuild<type>(func)

namespace Nequeo
{
	namespace Threading
	{
		class ThreadTask;

		/**
		* Interface for implementing an Executor, to implement a custom thread execution strategy, inherit from this class
		* and override SubmitToThread().
		*/
		class Executor
		{
		public:
			virtual ~Executor() = default;

			/**
			* Send function and its arguments to the SubmitToThread function.
			*/
			template<class Fn, class ... Args>
			bool Submit(Fn&& fn, Args&& ... args)
			{
				return SubmitToThread(NEQUEO_THREADING_BUILD_TYPED_FUNCTION(std::bind(std::forward<Fn>(fn), std::forward<Args>(args)...), void()));
			}


		protected:
			/**
			* To implement your own executor implementation, then simply subclass Executor and implement this method.
			*/
			virtual bool SubmitToThread(std::function<void()>&&) = 0;
		};


		/**
		* Default Executor implementation. Simply spawns a thread and detaches it.
		*/
		class DefaultExecutor : public Executor
		{
		public:
			DefaultExecutor() {}
		protected:
			bool SubmitToThread(std::function<void()>&&) override;
		};

		enum class OverflowPolicy
		{
			QUEUE_TASKS_EVENLY_ACCROSS_THREADS,
			REJECT_IMMEDIATELY
		};

		/**
		* Thread Pool Executor implementation.
		*/
		class PooledThreadExecutor : public Executor
		{
		public:
			PooledThreadExecutor(size_t poolSize, OverflowPolicy overflowPolicy = OverflowPolicy::QUEUE_TASKS_EVENLY_ACCROSS_THREADS);
			~PooledThreadExecutor();

			/**
			* Rule of 5 stuff.
			* Don't copy or move
			*/
			PooledThreadExecutor(const PooledThreadExecutor&) = delete;
			PooledThreadExecutor& operator =(const PooledThreadExecutor&) = delete;
			PooledThreadExecutor(PooledThreadExecutor&&) = delete;
			PooledThreadExecutor& operator =(PooledThreadExecutor&&) = delete;

		protected:
			bool SubmitToThread(std::function<void()>&&) override;

		private:
			Nequeo::Queue<std::function<void()>*> m_tasks;
			std::mutex m_queueLock;
			std::mutex m_syncPointLock;
			std::condition_variable m_syncPoint;
			Nequeo::Vector<ThreadTask*> m_threadTaskHandles;
			size_t m_poolSize;
			OverflowPolicy m_overflowPolicy;

			/**
			* Once you call this, you are responsible for freeing the memory pointed to by task.
			*/
			std::function<void()>* PopTask();
			bool HasTasks();

			friend class ThreadTask;
		};
	}
}
