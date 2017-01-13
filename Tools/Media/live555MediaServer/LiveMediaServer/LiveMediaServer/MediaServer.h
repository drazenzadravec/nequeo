/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MediaServer.h
*  Purpose :       Media Server class.
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

#include "MediaGlobal.h"

#include "BasicUsageEnvironment.hh"
#include "DynamicRTSPServer.hh"
#include "version.hh"

namespace Live {
	namespace Media
	{
		/// <summary>
		/// Live media server.
		/// </summary>
		class EXPORT_LIVE_MEDIA_SERVER_API MediaServer
		{
		public:
			/// <summary>
			/// Live media server.
			/// </summary>
			MediaServer();

			/// <summary>
			/// This destructor. Call release to cleanup resources.
			/// </summary>
			virtual ~MediaServer();

			/// <summary>
			/// Add a user.
			/// </summary>
			/// <param name="username">The user username.</param>
			/// <param name="password">The user password.</param>
			void AddUser(char const* username, char const* password);

			/// <summary>
			/// Remove a user.
			/// </summary>
			/// <param name="username">The user username.</param>
			void RemoveUser(const char* username);

			/// <summary>
			/// Gets or sets the rtsp server port number.
			/// </summary>
			void SetRtspServerPortNumber(unsigned int port);
			unsigned int GetRtspServerPortNumber() const;

			/// <summary>
			/// Gets or sets the up tunneling over http port number.
			/// </summary>
			void SetUpTunnelingOverHttpPortNumber(unsigned int port);
			unsigned int GetUpTunnelingOverHttpPortNumber() const;

			/// <summary>
			/// Has the server been initialised.
			/// </summary>
			/// <returns>True if initialised; else false.</returns>
			bool IsInitialised();

			/// <summary>
			/// Has the server been started.
			/// </summary>
			/// <returns>True if started; else false.</returns>
			bool HasStarted();

			/// <summary>
			/// Initialise the server.
			/// </summary>
			void Initialise();

			/// <summary>
			/// Start the server.
			/// </summary>
			void Start();

			/// <summary>
			/// Stop the server.
			/// </summary>
			void Stop();

		private:
			bool _disposed;
			bool _initialised;
			bool _started;

			std::thread _thread;

			unsigned int _rtspServerPortNumber;
			unsigned int _upTunnelingOverHttpPortNumber;

			std::unique_ptr<TaskScheduler> _scheduler;
			std::unique_ptr<BasicUsageEnvironment> _env;
			std::unique_ptr<DynamicRTSPServer> _rtspServer;
			std::unique_ptr<UserAuthenticationDatabase> _authDB;

			void StartThread();
		};
	}
}