/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaPlayer.cpp
*  Purpose :       MediaPlayer class.
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

#include "MediaPlayer.h"

#include <assert.h>

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Constructor for the current class. Use static CreateInstance method to instantiate.
			/// </summary>
			/// <param name="hVideo">The handle to the video window.</param>
			/// <param name="hEvent">The handle to the window to receive notifications.</param>
			/// <param name="hr">The result reference.</param>
			MediaPlayer::MediaPlayer(HWND hVideo, HWND hEvent, HRESULT &hr) :
				_pSession(NULL),
				_pSource(NULL),
				_pVideoDisplay(NULL),
				_pClock(NULL),
				_pRate(NULL),
				_pRateSupport(NULL),
				_bPending(FALSE),
				_hwndVideo(hVideo),
				_hwndEvent(hEvent),
				_playerState(Ready),
				_hCloseEvent(NULL),
				_hNotifyStateEvent(NULL),
				_hNotifyErrorEvent(NULL),
				_nRefCount(0),
				_pContentProtectionManager(NULL),
				_shutDown(false),
				_disposed(false)
			{
			}

			/// <summary>
			/// This destructor.
			/// </summary>
			MediaPlayer::~MediaPlayer()
			{
				// If not disposed.
				if (!_disposed)
				{
					// Indicate that dispose has been called.
					_disposed = true;

					SAFE_RELEASE(_pClock);
					SAFE_RELEASE(_pRate);
					SAFE_RELEASE(_pRateSupport);
				}
				
				// If FALSE, the app did not call Shutdown().
				assert(_pSession == NULL);  
			}

			/// <summary>
			/// Static class method to create the MediaPlayer object.
			/// </summary>
			/// <param name="hVideo">The handle to the video window.</param>
			/// <param name="hEvent">The handle to the window to receive notifications.</param>
			/// <param name="ppPlayer">Receives an AddRef's pointer to the MediaPlayer object. The caller must release the pointer.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::CreateInstance(HWND hVideo, HWND hEvent, MediaPlayer **ppPlayer)
			{
				// Make sure the a video and event handler exists.
				assert(hVideo != NULL);
				assert(hEvent != NULL);

				// Initialse the result to all is good.
				HRESULT hr = S_OK;

				// MediaPlayer constructor sets the ref count to zero.
				// Create method calls AddRef.
				// Create a new medis player instance.
				MediaPlayer *pPlayer = new MediaPlayer(hVideo, hEvent, hr);

				// If the player was not created.
				if (pPlayer == NULL)
				{
					// Out of memory result.
					hr = E_OUTOFMEMORY;
				}

				// If successful the initialise the player.
				if (SUCCEEDED(hr))
				{
					// Call initialise to to load
					// all player resources.
					hr = pPlayer->Initialize();
				}

				// If successful initialisation.
				if (SUCCEEDED(hr))
				{
					// Increment the reference count of the current player.
					*ppPlayer = pPlayer;
					(*ppPlayer)->AddRef();
				}
				else
				{
					// Delete the instance of the player
					// if not successful.
					delete pPlayer;
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Initializes the MediaPlayer object. This method is called by the
			/// CreateInstance method.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::Initialize()
			{
				// Initialse the result to all is good.
				HRESULT hr = S_OK;

				// Create a new close player event handler.
				_hCloseEvent = CreateEvent(NULL, FALSE, FALSE, NULL);

				// If event was not created.
				if (_hCloseEvent == NULL)
				{
					// Get the result value.
					hr = __HRESULT_FROM_WIN32(GetLastError());
				}

				// If successful creation of the close event.
				if (SUCCEEDED(hr))
				{
					// Start up Media Foundation platform.
					hr = MFStartup(MF_VERSION);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Add a new player ref item.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG MediaPlayer::AddRef()
			{
				// Increment the player ref count.
				return InterlockedIncrement(&_nRefCount);
			}

			/// <summary>
			/// Release this player resources.
			/// </summary>
			/// <returns>The result.</returns>
			ULONG MediaPlayer::Release()
			{
				// Decrement the player ref count.
				ULONG uCount = InterlockedDecrement(&_nRefCount);

				// If released.
				if (uCount == 0)
				{
					// Delete this players resources.
					delete this;
				}

				// For thread safety, return a temporary variable.
				return uCount;
			}

			/// <summary>
			/// Get the player reference for the reference id.
			/// </summary>
			/// <param name="iid">The player reference id.</param>
			/// <param name="ppv">The current player reference.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::QueryInterface(REFIID iid, void** ppv)
			{
				// If null the return invalid pointer.
				if (!ppv)
				{
					return E_POINTER;
				}

				// If the id is unknown.
				if (iid == IID_IUnknown)
				{
					// Return this player with default player reference.
					*ppv = static_cast<IUnknown*>(this);
				}
				else if (iid == IID_IMFAsyncCallback)
				{
					// Return this player reference with async callback.
					*ppv = static_cast<IMFAsyncCallback*>(this);
				}
				else
				{
					// Return No such interface supported.
					return E_NOINTERFACE;
				}

				// Add a new player ref item.
				AddRef();

				// Return all is good.
				return S_OK;
			}

			/// <summary>
			/// Callback for asynchronous BeginGetEvent method.
			/// </summary>
			/// <param name="pAsyncResult">The pointer to the result.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::Invoke(IMFAsyncResult *pResult)
			{
				HRESULT hr = S_OK;
				IMFMediaEvent* pEvent = NULL;
				MediaEventType meType = MEUnknown;  // Event type
				HRESULT hrStatus = S_OK;	        // Event status

				// Used with MESessionTopologyStatus event.  
				MF_TOPOSTATUS TopoStatus = MF_TOPOSTATUS_INVALID;   

				// Get the event from the event queue.
				hr = _pSession->EndGetEvent(pResult, &pEvent);

				// If successful get end of event.
				if (SUCCEEDED(hr))
				{
					// Get the event type.
					hr = pEvent->GetType(&meType);
				}

				// If successful get event type.
				if (SUCCEEDED(hr))
				{
					// Get the event status. If the operation that triggered the event did
					// not succeed, the status is a failure code.
					hr = pEvent->GetStatus(&hrStatus);
				}

				// If successful get event status.
				if (SUCCEEDED(hr))
				{
					// Display the event type name.
					TRACE((L"Media event: %s", EventName(meType)));

					// Check if the async operation succeeded.
					if (SUCCEEDED(hrStatus))
					{
						// Switch on the event type. Update the internal 
						// state of the MediaPlayer as needed.
						switch (meType)
						{
						case MESessionTopologyStatus:
							// Get the status code.
							hr = pEvent->GetUINT32(MF_EVENT_TOPOLOGY_STATUS, (UINT32*)&TopoStatus);
							if (SUCCEEDED(hr))
							{
								switch (TopoStatus)
								{
								case MF_TOPOSTATUS_READY:
									hr = OnTopologyReady(pEvent);
									break;
								default:
									// Nothing to do.
									break;
								}
							}
							break;

						case MESessionStarted:
							hr = OnSessionStarted(pEvent);
							break;

						case MESessionPaused:
							hr = OnSessionPaused(pEvent);
							break;

						case MESessionClosed:
							hr = OnSessionClosed(pEvent);
							break;

						case MESessionStopped:
							hr = OnSessionStopped(pEvent);
							break;

						case MEEndOfPresentation:
							hr = OnPresentationEnded(pEvent);
							break;

						case MEUnknown:
						case MESessionEnded:
							hr = OnSessionEnded(pEvent);
							break;
						}

					}
					else
					{
						// The async operation failed. Notify the application
						NotifyError(hrStatus);
					}
				}

				// Request another event.
				if (meType != MESessionClosed)
				{
					// Start a new event listener for this player events.
					hr = _pSession->BeginGetEvent(this, NULL);
				}

				// Release the current event handler.
				SAFE_RELEASE(pEvent);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Clear the current media values.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::Clear()
			{
				_caps = 0;
				_bCanScrub = FALSE;
				_hnsDuration = 0;
				_fPrevRate = 1.0f;
				_bPending = FALSE;

				SAFE_RELEASE(_pClock);
				SAFE_RELEASE(_pRate);
				SAFE_RELEASE(_pRateSupport);
				SAFE_RELEASE(_pSource);
				SAFE_RELEASE(_pSession);
				SAFE_RELEASE(_pContentProtectionManager);

				ZeroMemory(&_normalState, sizeof(_normalState));
				_normalState.command = CmdStop;
				_normalState.fRate = 1.0f;

				ZeroMemory(&_requestState, sizeof(_requestState));
				_requestState.command = CmdNone;
				_requestState.fRate = 1.0f;

				// Return all is good.
				return S_OK;
			}

			/// <summary>
			/// Opena URL resources, can be a file, network or internet resource.
			/// </summary>
			/// <param name="sURL">The string of the URL.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::OpenURL(const WCHAR *sURL)
			{
				// 1. Create a new media session.
				// 2. Create the media source.
				// 3. Create the topology.
				// 4. Queue the topology [asynchronous]
				// 5. Start playback [asynchronous - does not happen in this method.]

				HRESULT hr = S_OK;
				IMFTopology *pTopology = NULL;
				_shutDown = false;

				// Create the media session.
				hr = CreateSession();

				// If successful created session.
				if (SUCCEEDED(hr))
				{
					// Create the media source.
					hr = CreateMediaSource(sURL);
				}

				// If successful created media source.
				if (SUCCEEDED(hr))
				{
					// Create a partial topology.
					hr = CreateTopologyFromSource(&pTopology);
				}

				// If successful created topology from source.
				if (SUCCEEDED(hr))
				{
					// Set the topology on the media session.
					hr = _pSession->SetTopology(0, pTopology);
				}

				// If successful topology on the media session.
				if (SUCCEEDED(hr))
				{
					// Set our state to "open pending"
					_playerState = OpenPending;

					// Notify that the state has changed.
					NotifyState();
				}
				else
				{
					// Notify that an error has occured.
					NotifyError(hr);
					_playerState = Ready;
				}

				// Release the current topology handler.
				SAFE_RELEASE(pTopology);

				// If SetTopology succeeded, the media session will queue an 
				// MESessionTopologySet event.
				return hr;
			}

			/// <summary>
			/// Opena URL resources, can be a file, network or internet resource.
			/// </summary>
			/// <param name="sURL">The null-terminated string that contains the URL of the byte stream. The URL is optional and can be NULL.</param>
			/// <param name="pByteStream">The byte stream's IMFByteStream interface.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::OpenURL(const WCHAR *sURL, IMFByteStream *pByteStream)
			{
				// 1. Create a new media session.
				// 2. Create the media source.
				// 3. Create the topology.
				// 4. Queue the topology [asynchronous]
				// 5. Start playback [asynchronous - does not happen in this method.]

				HRESULT hr = S_OK;
				IMFTopology *pTopology = NULL;
				_shutDown = false;

				// Create the media session.
				hr = CreateSession();

				// If successful created session.
				if (SUCCEEDED(hr))
				{
					// Create the media source.
					hr = CreateMediaSource(sURL, pByteStream);
				}

				// If successful created media source.
				if (SUCCEEDED(hr))
				{
					// Create a partial topology.
					hr = CreateTopologyFromSource(&pTopology);
				}

				// If successful created topology from source.
				if (SUCCEEDED(hr))
				{
					// Set the topology on the media session.
					hr = _pSession->SetTopology(0, pTopology);
				}

				// If successful topology on the media session.
				if (SUCCEEDED(hr))
				{
					// Set our state to "open pending"
					_playerState = OpenPending;

					// Notify that the state has changed.
					NotifyState();
				}
				else
				{
					// Notify that an error has occured.
					NotifyError(hr);
					_playerState = Ready;
				}

				// Release the current topology handler.
				SAFE_RELEASE(pTopology);

				// If SetTopology succeeded, the media session will queue an 
				// MESessionTopologySet event.
				return hr;
			}

			/// <summary>
			/// Close the media.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::Close()
			{
				// If already closed.
				if (_playerState == Closed)
				{
					// Closed.
					return S_OK;
				}

				// If no session or no source.
				if (_pSession == NULL || _pSource == NULL)
				{
					// Catastrophic failure
					return E_UNEXPECTED;
				}

				AutoLockHandler lock(_critsec);

				// Close the session playback.
				HRESULT hr = _pSession->Close();

				_normalState.command = CmdClose;
				_bPending = CMD_PENDING;

				// If successful closed.
				if (SUCCEEDED(hr))
				{
					// Set our state to "closed"
					_playerState = Closed;

					// Notify that the state has changed.
					NotifyState();
				}
				else
				{
					// Notify that an error has occured.
					NotifyError(hr);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Start playing the media.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::Play()
			{
				// If not state is paused.
				if (_playerState != Paused && _playerState != Stopped)
				{
					// Unspecified error.
					return E_FAIL;
				}

				// If no session or no source.
				if (_pSession == NULL || _pSource == NULL)
				{
					// Catastrophic failure
					return E_UNEXPECTED;
				}

				// Start the play back.
				HRESULT hr = StartPlayback();

				// If successful playback.
				if (SUCCEEDED(hr))
				{
					// Set our state to "start pending"
					_playerState = StartPending;

					// Notify that the state has changed.
					NotifyState();
				}
				else
				{
					// Notify that an error has occured.
					NotifyError(hr);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Pause playback of the media.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::Pause()
			{
				// If not playback has started.
				if (_playerState != Started && _playerState != Stopped)
				{
					// Unspecified error.
					return E_FAIL;
				}

				// If no session or no source.
				if (_pSession == NULL || _pSource == NULL)
				{
					// Catastrophic failure
					return E_UNEXPECTED;
				}

				AutoLockHandler lock(_critsec);

				// Pause the session playback.
				HRESULT hr = _pSession->Pause();

				_normalState.command = CmdPause;
				_bPending = CMD_PENDING;

				// If successful pause.
				if (SUCCEEDED(hr))
				{
					// Set our state to "pause pending"
					_playerState = PausePending;

					// Notify that the state has changed.
					NotifyState();
				}
				else
				{
					// Notify that an error has occured.
					NotifyError(hr);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Stop playback of the media.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::Stop()
			{
				// If not playback has started or not paused.
				if (_playerState != Started && _playerState != Paused)
				{
					// Unspecified error.
					return E_FAIL;
				}

				// If no session or no source.
				if (_pSession == NULL || _pSource == NULL)
				{
					// Catastrophic failure
					return E_UNEXPECTED;
				}

				AutoLockHandler lock(_critsec);

				// Stop the session playback.
				HRESULT hr = _pSession->Stop();

				_normalState.command = CmdStop;
				_bPending = CMD_PENDING;

				// If successful stopped.
				if (SUCCEEDED(hr))
				{
					// Set our state to "pause pending"
					_playerState = Stopped;

					// Notify that the state has changed.
					NotifyState();
				}
				else
				{
					// Notify that an error has occured.
					NotifyError(hr);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Shutdown of the media. Releases all resources held by this object.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::Shutdown()
			{
				// Initally all is good.
				HRESULT hr = S_OK;

				// If not shutdown.
				if (!_shutDown)
				{
					// Close the session.
					hr = CloseSession();

					// Shutdown the Media Foundation platform
					MFShutdown();

					// Close the close event handler.
					CloseHandle(_hCloseEvent);
					_shutDown = true;
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Repaint the video window.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::Repaint()
			{
				// Initally all is good.
				HRESULT hr = S_OK;

				// If a video display control has been assigned.
				if (_pVideoDisplay)
				{
					// Repaint the video.
					hr = _pVideoDisplay->RepaintVideo();
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Repaint the video window. The application should call this method when it receives a WM_SIZE message.
			/// </summary>
			/// <param name="width">The width of the player.</param>
			/// <param name="height">The height of the player.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT	MediaPlayer::ResizeVideo(WORD width, WORD height)
			{
				// Initally all is good.
				HRESULT hr = S_OK;

				// If a video display control has been assigned.
				if (_pVideoDisplay)
				{
					MFVideoNormalizedRect nRect = { 0.0f, 0.0f, 1.0f, 1.0f };
					RECT rcDest = { 0, 0, width, height };

					// Set the player position in the control.
					hr = _pVideoDisplay->SetVideoPosition(&nRect, &rcDest);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Get the media duration.
			/// </summary>
			/// <param name="phnsDuration">The media duration.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::GetDuration(MFTIME *phnsDuration)
			{
				if (phnsDuration == NULL)
				{
					return E_POINTER;
				}

				// Assign the duration.
				*phnsDuration = _hnsDuration;

				if (_hnsDuration == 0)
				{
					return MF_E_NO_DURATION;
				}
				else
				{
					return S_OK;
				}
			}

			/// <summary>
			/// Get the current media position.
			/// </summary>
			/// <param name="phnsPosition">The media position.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::GetCurrentPosition(MFTIME *phnsPosition)
			{
				if (phnsPosition == NULL)
				{
					return E_POINTER;
				}

				HRESULT hr = S_OK;

				// Lock this critical section.
				AutoLockHandler lock(_critsec);

				if (_pClock == NULL)
				{
					return MF_E_NO_CLOCK;
				}

				// Return, in order:
				// 1. Cached seek request (nominal position).
				// 2. Pending seek operation (nominal position).
				// 3. Presentation time (actual position).
				if (_requestState.command == CmdSeek)
				{
					// Assign the currrent position.
					*phnsPosition = _requestState.hnsStart;
				}
				else if (_bPending & CMD_PENDING_SEEK)
				{
					// Assign the currrent position.
					*phnsPosition = _normalState.hnsStart;
				}
				else
				{
					// Assign the presentation clock
					// with the current position.
					hr = _pClock->GetTime(phnsPosition);
				}

				return hr;
			}

			/// <summary>
			/// Get the current media position, only use this in a thread safe controlled environment.
			/// </summary>
			/// <param name="phnsPosition">The media position.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::GetCurrentPositionDirect(MFTIME *phnsPosition)
			{
				if (phnsPosition == NULL)
				{
					return E_POINTER;
				}

				HRESULT hr = S_OK;

				// Lock this critical section.
				AutoLockHandler lock(_critsec);

				if (_pClock == NULL)
				{
					return MF_E_NO_CLOCK;
				}

				// Assign the presentation clock
				// with the current position.
				hr = _pClock->GetTime(phnsPosition);

				return hr;
			}

			/// <summary>
			/// Set the media position.
			/// </summary>
			/// <param name="hnsPosition">The media position.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::SetPosition(MFTIME hnsPosition)
			{
				AutoLockHandler lock(_critsec);

				HRESULT hr = S_OK;

				// If pending request.
				if (_bPending)
				{
					// Currently seeking or changing rates, so cache this request.
					_requestState.command = CmdSeek;
					_requestState.hnsStart = hnsPosition;
				}
				else
				{
					// Set the internal position.
					hr = SetPositionInternal(hnsPosition);
				}

				return hr;
			}

			/// <summary>
			/// Try set the media position now.
			/// </summary>
			/// <param name="hnsPosition">The media position.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::SetPositionNoPending(MFTIME hnsPosition)
			{
				AutoLockHandler lock(_critsec);

				HRESULT hr = S_OK;

				// Set the internal position.
				hr = SetPositionInternal(hnsPosition);
				return hr;
			}

			/// <summary>
			/// Can seek.
			/// </summary>
			/// <param name="pbCanSeek">True if can seek; else false.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::CanSeek(BOOL *pbCanSeek)
			{
				if (pbCanSeek == NULL)
				{
					return E_POINTER;
				}

				// Note: The MFSESSIONCAP_SEEK flag is sufficient for seeking. However, to
				// implement a seek bar, an application also needs the duration (to get 
				// the valid range) and a presentation clock (to get the current position).
				*pbCanSeek = (
					((_caps & MFSESSIONCAP_SEEK) == MFSESSIONCAP_SEEK) &&
					(_hnsDuration > 0) &&
					(_pClock != NULL));

				return S_OK;
			}

			/// <summary>
			/// Queries whether the current session supports scrubbing.
			/// </summary>
			/// <param name="pbCanScrub">True if can scrub; else false.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::CanScrub(BOOL *pbCanScrub)
			{
				if (pbCanScrub == NULL)
				{
					return E_POINTER;
				}

				*pbCanScrub = _bCanScrub;
				return S_OK;
			}

			/// <summary>
			/// Enables or disables scrubbing.
			/// </summary>
			/// <param name="bScrub">True to scrub; else false</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::Scrub(BOOL bScrub)
			{
				// Scrubbing is implemented as rate = 0.
				AutoLockHandler lock(_critsec);

				if (!_pRate)
				{
					return MF_E_INVALIDREQUEST;
				}
				if (!_bCanScrub)
				{
					return MF_E_INVALIDREQUEST;
				}

				HRESULT hr = S_OK;

				if (bScrub)
				{
					// Enter scrubbing mode. Cache the current rate.

					if (GetNominalRate() != 0)
					{
						_fPrevRate = _normalState.fRate;
					}

					hr = SetRate(0.0f);
				}
				else
				{
					// Leaving scrubbing mode. Restore the old rate.

					if (GetNominalRate() == 0)
					{
						hr = SetRate(_fPrevRate);
					}
				}

				return hr;
			}

			/// <summary>
			/// Queries whether the current session supports fast-forward.
			/// </summary>
			/// <param name="pbCanFF">True if can fast forward; else false.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::CanFastForward(BOOL *pbCanFF)
			{
				if (pbCanFF == NULL)
				{
					return E_POINTER;
				}

				*pbCanFF = ((_caps & MFSESSIONCAP_RATE_FORWARD) == MFSESSIONCAP_RATE_FORWARD);
				return S_OK;
			}

			/// <summary>
			/// Queries whether the current session supports rewind (reverse play).
			/// </summary>
			/// <param name="pbCanRewind">True if can rewind; else false.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::CanRewind(BOOL *pbCanRewind)
			{
				if (pbCanRewind == NULL)
				{
					return E_POINTER;
				}

				*pbCanRewind = ((_caps & MFSESSIONCAP_RATE_REVERSE) == MFSESSIONCAP_RATE_REVERSE);
				return S_OK;
			}

			/// <summary>
			/// Switches to fast-forward playback, as follows:
			/// If the current rate is < 0 (reverse play), switch to 1x speed.
			/// Otherwise, double the current playback rate.
			/// Note: This method is for convenience; the application can also call SetRate.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::FastForward()
			{
				if (!_pRate)
				{
					return MF_E_INVALIDREQUEST;
				}

				HRESULT hr = S_OK;
				float   fTarget = GetNominalRate() * 2;

				if (fTarget <= 0.0f)
				{
					fTarget = 1.0f;
				}

				hr = SetRate(fTarget);
				return hr;
			}

			/// <summary>
			/// Switches to reverse playback, as follows:
			/// If the current rate is > 0 (forward playback), switch to -1x speed.
			/// Otherwise, double the current (reverse) playback rate.
			/// Note: This method is for convenience; the application can also call SetRate.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::Rewind()
			{
				if (!_pRate)
				{
					return MF_E_INVALIDREQUEST;
				}

				HRESULT hr = S_OK;
				float   fTarget = GetNominalRate() * 2;

				if (fTarget >= 0.0f)
				{
					fTarget = -1.0f;
				}

				hr = SetRate(fTarget);
				return hr;
			}

			/// <summary>
			/// Sets the playback rate.
			/// </summary>
			/// <param name="fRate">The playback rate.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::SetRate(float fRate)
			{
				HRESULT hr = S_OK;
				BOOL bThin = FALSE;

				AutoLockHandler lock(_critsec);

				if (fRate == GetNominalRate())
				{
					return S_OK; // no-op
				}

				if (_pRateSupport == NULL)
				{
					return MF_E_INVALIDREQUEST;
				}

				// Check if this rate is supported. Try non-thinned playback first,
				// then fall back to thinned playback.

				hr = _pRateSupport->IsRateSupported(FALSE, fRate, NULL);

				if (FAILED(hr))
				{
					bThin = TRUE;
					hr = _pRateSupport->IsRateSupported(TRUE, fRate, NULL);
				}

				if (FAILED(hr))
				{
					// Unsupported rate.
					return hr;
				}

				// If there is an operation pending, cache the request.
				if (_bPending)
				{
					_requestState.fRate = fRate;
					_requestState.bThin = bThin;

					// Remember the current transport state (play, paused, etc), so that we
					// can restore it after the rate change, if necessary. However, if 
					// anothercommand is already pending, that one takes precedent.

					if (_requestState.command == CmdNone)
					{
						_requestState.command = _normalState.command;
					}

				}
				else
				{
					// No pending operation. Commit the new rate.
					hr = CommitRateChange(fRate, bThin);
				}

				return hr;
			}

			/// <summary>
			/// Content protection manager.
			/// </summary>
			/// <param name="ppManager">The content protection manger.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT  MediaPlayer::GetContentProtectionManager(ContentProtectionManager **ppManager)
			{
				// If the manager has not been created.
				if (ppManager == NULL)
				{
					return E_INVALIDARG;
				}

				if (_pContentProtectionManager == NULL)
				{
					// Session wasn't created yet. No helper object;
					return E_FAIL; 
				}

				// Assign the protection manger.
				*ppManager = _pContentProtectionManager;
				(*ppManager)->AddRef();

				// Return all is good.
				return S_OK;
			}

			/// <summary>
			/// Creates a new instance of the media session.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::CreateSession()
			{
				// Close the old session, if any.
				HRESULT hr = CloseSession();

				// Clear other session values.
				Clear();

				// If successful close session.
				if (SUCCEEDED(hr))
				{
					// Create the media session.
					hr = MFCreateMediaSession(NULL, &_pSession);

					// If successful create session.
					if (SUCCEEDED(hr))
					{
						// Get the session capabilities.
						hr = _pSession->GetSessionCapabilities(&_caps);
					}
				}

				// If successful session.
				if (SUCCEEDED(hr))
				{
					// Start pulling events from the media session.
					hr = _pSession->BeginGetEvent((IMFAsyncCallback*)this, NULL);
				}

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Creates a new instance of the content protection media session.
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::CreateProtectionSession()
			{
				IMFAttributes *pAttributes = NULL;
				IMFActivate   *pEnablerActivate = NULL;

				// Close the old session, if any.
				HRESULT hr = CloseSession();

				// If successful close session.
				if (SUCCEEDED(hr))
				{
					// Create a new attribute store.
					hr = MFCreateAttributes(&pAttributes, 1);
				}

				// If successful create attributes.
				if (SUCCEEDED(hr))
				{
					// Was released in CloseSession.
					assert(_pContentProtectionManager == NULL); 

					// Create the content protection manager.
					hr = ContentProtectionManager::CreateInstance(_hwndEvent, &_pContentProtectionManager);
				}

				// If successful create manager.
				if (SUCCEEDED(hr))
				{
					// Set the MF_SESSION_CONTENT_PROTECTION_MANAGER attribute with a pointer
					// to the content protection manager.
					hr = pAttributes->SetUnknown(MF_SESSION_CONTENT_PROTECTION_MANAGER, (IMFContentProtectionManager*)_pContentProtectionManager);
				}

				// If successful set attibutes.
				if (SUCCEEDED(hr))
				{
					// Create the PMP media session.
					hr = MFCreatePMPMediaSession(
						0, // Can use this flag: MFPMPSESSION_UNPROTECTED_PROCESS
						pAttributes,
						&_pSession,
						&pEnablerActivate);
				}

				// If successful session.
				if (SUCCEEDED(hr))
				{
					// Start pulling events from the media session.
					hr = _pSession->BeginGetEvent((IMFAsyncCallback*)this, NULL);
				}
				else
				{
					// TODO:

					// If MFCreatePMPMediaSession fails it might return an IMFActivate pointer.
					// This indicates that a trusted binary failed to load in the protected process.
					// An application can use the IMFActivate pointer to create an enabler object, which 
					// provides revocation and renewal information for the component that failed to
					// load. 

					// This sample does not demonstrate that feature. Instead, we simply treat this
					// case as a playback failure. 
				}

				// Release the resources.
				SAFE_RELEASE(pAttributes);
				SAFE_RELEASE(pEnablerActivate);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Closes the media session. 
			/// </summary>
			/// <returns>The result of the operation.</returns>
			/// <remarks>
			/// MFMediaSession::Close method is asynchronous, but the
			/// MediaPlayer::CloseSession method waits on the MESessionClosed event.
			/// The MESessionClosed event is guaranteed to be the last event 
			/// that the media session fires.
			/// </remarks>
			HRESULT MediaPlayer::CloseSession()
			{
				// Initally all is good.
				HRESULT hr = S_OK;

				// Safe release of the video display control handler.
				SAFE_RELEASE(_pVideoDisplay);

				// If a session exists.
				if (_pSession)
				{
					// Close the session.
					hr = _pSession->Close();
					
					// If successful close session.
					if (SUCCEEDED(hr))
					{
						// Wait for the close event operation to complete
						DWORD res = WaitForSingleObject(_hCloseEvent, 5000);
						if (res == WAIT_TIMEOUT)
						{
							// Timed out.
							TRACE((L"WaitForSingleObject timed out!"));
						}
					}
				}

				// If a media source exists.
				if (_pSource)
				{
					// Complete shutdown operations
					_pSource->Shutdown();
				}

				// If a media session exists.
				if (_pSession)
				{
					// Shut down the media session. (Synchronous operation, no events.)
					_pSession->Shutdown();
				}

				// Safe release of the source and session handlers.
				SAFE_RELEASE(_pClock);
				SAFE_RELEASE(_pRate);
				SAFE_RELEASE(_pRateSupport);
				SAFE_RELEASE(_pSource);
				SAFE_RELEASE(_pSession);
				SAFE_RELEASE(_pContentProtectionManager);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Starts playback from the current position. 
			/// </summary>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::StartPlayback()
			{
				// If a session does exist.
				assert(_pSession != NULL);

				// Initally all is good.
				HRESULT hr = S_OK;
				AutoLockHandler lock(_critsec);

				// Initiate a property variant.
				PROPVARIANT varStart;
				PropVariantInit(&varStart);

				// Variant start is empty.
				varStart.vt = VT_EMPTY;

				// Start the playback of the media.
				hr = _pSession->Start(&GUID_NULL, &varStart);
				
				// Not strictly needed here but good form.
				// Clear the variant.
				PropVariantClear(&varStart);

				_normalState.command = CmdStart;
				_bPending = CMD_PENDING;

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Create a media source from a URL, can be a file, network or internet resource.
			/// </summary>
			/// <param name="sURL">The string of the URL.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::CreateMediaSource(const WCHAR *sURL)
			{
				// Initally all is good.
				HRESULT hr = S_OK;

				IMFSourceResolver* pSourceResolver = NULL;
				IUnknown* pSource = NULL;

				// If successful.
				if (SUCCEEDED(hr))
				{
					// Create the source resolver.
					hr = MFCreateSourceResolver(&pSourceResolver);
				}

				// If successful source resolver.
				if (SUCCEEDED(hr))
				{
					MF_OBJECT_TYPE ObjectType = MF_OBJECT_INVALID;

					// Use the source resolver to create the media source.
					hr = pSourceResolver->CreateObjectFromURL(
						sURL,						// URL of the source.
						MF_RESOLUTION_MEDIASOURCE,	// Create a source object.
						NULL,						// Optional property store.
						&ObjectType,				// Receives the created object type. 
						&pSource);					// Receives a pointer to the media source.
				}

				// If successful media source.
				if (SUCCEEDED(hr))
				{
					// Get the IMFMediaSource interface from the media source.
					hr = pSource->QueryInterface(__uuidof(IMFMediaSource), (void**)&_pSource);
				}

				// Clean up
				SAFE_RELEASE(pSourceResolver);
				SAFE_RELEASE(pSource);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Create a media source from a URL, can be a file, network or internet resource.
			/// </summary>
			/// <param name="sURL">The null-terminated string that contains the URL of the byte stream. The URL is optional and can be NULL.</param>
			/// <param name="pByteStream">The byte stream's IMFByteStream interface.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::CreateMediaSource(const WCHAR *sURL, IMFByteStream *pByteStream)
			{
				// Initally all is good.
				HRESULT hr = S_OK;

				IMFSourceResolver* pSourceResolver = NULL;
				IUnknown* pSource = NULL;

				// If successful.
				if (SUCCEEDED(hr))
				{
					// Create the source resolver.
					hr = MFCreateSourceResolver(&pSourceResolver);
				}

				// If successful source resolver.
				if (SUCCEEDED(hr))
				{
					MF_OBJECT_TYPE ObjectType = MF_OBJECT_INVALID;

					// Use the source resolver to create the media source.
					hr = pSourceResolver->CreateObjectFromByteStream(
						pByteStream,				// Byte stream's IMFByteStream interface
						sURL,						// URL of the source. Optional.
						MF_RESOLUTION_MEDIASOURCE,	// Create a source object.
						NULL,						// Optional property store.
						&ObjectType,				// Receives the created object type. 
						&pSource);					// Receives a pointer to the media source.
				}

				// If successful media source.
				if (SUCCEEDED(hr))
				{
					// Get the IMFMediaSource interface from the media source.
					hr = pSource->QueryInterface(__uuidof(IMFMediaSource), (void**)&_pSource);
				}

				// Clean up
				SAFE_RELEASE(pSourceResolver);
				SAFE_RELEASE(pSource);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Create a playback topology from the media source.
			/// </summary>
			/// <param name="ppTopology">The topology.</param>
			/// <returns>The result of the operation.</returns>
			/// <remarks>
			/// The media source must be created already.
			/// Call CreateMediaSource() before calling this method.
			/// </remarks>
			HRESULT MediaPlayer::CreateTopologyFromSource(IMFTopology **ppTopology)
			{
				// If the session and source exists.
				assert(_pSession != NULL);
				assert(_pSource != NULL);

				// Initally all is good.
				HRESULT hr = S_OK;
				HRESULT hrTmp = S_OK;   // For non-critical failures.

				IMFTopology *pTopology = NULL;
				IMFClock *pClock = NULL;
				IMFPresentationDescriptor* pSourcePD = NULL;
				DWORD cSourceStreams = 0;

				// Create a new topology.
				hr = MFCreateTopology(&pTopology);

				// If successful.
				if (SUCCEEDED(hr))
				{
					// Create the presentation descriptor for the media source.
					hr = _pSource->CreatePresentationDescriptor(&pSourcePD);
				}

				// If successful presentation descriptor.
				if (SUCCEEDED(hr))
				{
					// Get the duration from the presentation descriptor
					(void)pSourcePD->GetUINT64(MF_PD_DURATION, (UINT64*)&_hnsDuration);

					// Get the presentation clock.
					hrTmp = _pSession->GetClock(&pClock);
					if (SUCCEEDED(hrTmp))
					{
						// Create the clock.
						hrTmp = pClock->QueryInterface(IID_PPV_ARGS(&_pClock));
					}

					// If clock was created.
					if (SUCCEEDED(hrTmp))
					{
						// Get the rate control interface.
						hrTmp = MFGetService(_pSession, MF_RATE_CONTROL_SERVICE, IID_PPV_ARGS(&_pRate));
					}

					// If rate was created.
					if (SUCCEEDED(hrTmp))
					{
						// Get the rate support interface.
						hrTmp = MFGetService(_pSession, MF_RATE_CONTROL_SERVICE, IID_PPV_ARGS(&_pRateSupport));
					}

					// If rate support was created.
					if (SUCCEEDED(hrTmp))
					{
						// Check if rate 0 (scrubbing) is supported.
						hrTmp = _pRateSupport->IsRateSupported(TRUE, 0, NULL);
					}

					// If if rate 0 (scrubbing) is supported.
					if (SUCCEEDED(hrTmp))
					{
						_bCanScrub = TRUE;
					}

					// Get the number of streams in the media source.
					hr = pSourcePD->GetStreamDescriptorCount(&cSourceStreams);
				}

				// If successful stream descriptor count.
				if (SUCCEEDED(hr))
				{
					// For each stream, create the topology nodes and add them to the topology.
					for (DWORD i = 0; i < cSourceStreams; i++)
					{
						// Add the branch to partial topology.
						hr = AddBranchToPartialTopology(pTopology, pSourcePD, i);
						if (FAILED(hr))
						{
							// Break on error.
							break;
						}
					}
				}

				// If successful add branch.
				if (SUCCEEDED(hr))
				{
					// Return the IMFTopology pointer to the caller.
					*ppTopology = pTopology;
					(*ppTopology)->AddRef();
				}

				// Clean up
				SAFE_RELEASE(pTopology);
				SAFE_RELEASE(pSourcePD);
				SAFE_RELEASE(pClock);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Adds a topology branch for one stream.
			/// </summary>
			/// <param name="pTopology">The topology.</param>
			/// <param name="pSourcePD">The topology.</param>
			/// <param name="iStream">The topology.</param>
			/// <returns>The result of the operation.</returns>
			/// <remarks>
			///  Pre-conditions: The topology must be created already.
			///
			///  Notes: For each stream, we must do the following:
			///    1. Create a source node associated with the stream. 
			///    2. Create an output node for the renderer. 
			///    3. Connect the two nodes.
			///  The media session will resolve the topology, so we do not have
			///  to worry about decoders or other transforms.
			/// </remarks>
			HRESULT MediaPlayer::AddBranchToPartialTopology(IMFTopology *pTopology, IMFPresentationDescriptor *pSourcePD, DWORD iStream)
			{
				// If the topology exists.
				assert(pTopology != NULL);

				IMFStreamDescriptor* pSourceSD = NULL;
				IMFTopologyNode* pSourceNode = NULL;
				IMFTopologyNode* pOutputNode = NULL;
				BOOL fSelected = FALSE;

				// Initally all is good.
				HRESULT hr = S_OK;

				// Get the stream descriptor for this stream.
				hr = pSourcePD->GetStreamDescriptorByIndex(iStream, &fSelected, &pSourceSD);
				
				// If successful stream descriptor by index.
				if (SUCCEEDED(hr))
				{
					// Create the topology branch only if the stream is selected.
					// Otherwise, do nothing.
					if (fSelected)
					{
						// Create a source node for this stream.
						hr = CreateSourceStreamNode(pSourcePD, pSourceSD, &pSourceNode);

						// If successful source stream node.
						if (SUCCEEDED(hr))
						{
							// Create the output node for the renderer.
							hr = CreateOutputNode(pSourceSD, &pOutputNode);
						}

						// If successful output node.
						if (SUCCEEDED(hr))
						{
							// Add both nodes to the topology.
							hr = pTopology->AddNode(pSourceNode);
						}

						// If successful source node.
						if (SUCCEEDED(hr))
						{
							// Add both nodes to the output node.
							hr = pTopology->AddNode(pOutputNode);
						}

						// If successful output node.
						if (SUCCEEDED(hr))
						{
							// Connect the source node to the output node.
							hr = pSourceNode->ConnectOutput(0, pOutputNode, 0);
						}
					}
				}

				// Clean up.
				SAFE_RELEASE(pSourceSD);
				SAFE_RELEASE(pSourceNode);
				SAFE_RELEASE(pOutputNode);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Creates a source-stream node for a stream.
			/// </summary>
			/// <param name="pSourcePD">Presentation descriptor for the media source.</param>
			/// <param name="pSourceSD">Stream descriptor for the stream.</param>
			/// <param name="ppNode">Receives a pointer to the new node.</param>
			/// <returns>The result of the operation.</returns>
			/// <remarks>
			/// Pre-conditions: Create the media source.
			/// </remarks>
			HRESULT MediaPlayer::CreateSourceStreamNode(IMFPresentationDescriptor *pSourcePD, IMFStreamDescriptor *pSourceSD, IMFTopologyNode **ppNode)
			{
				// If the topology exists.
				assert(_pSource != NULL);

				IMFTopologyNode *pNode = NULL;

				// Initally all is good.
				HRESULT hr = S_OK;

				// Create the source-stream node. 
				hr = MFCreateTopologyNode(MF_TOPOLOGY_SOURCESTREAM_NODE, &pNode);

				// If successful topology.
				if (SUCCEEDED(hr))
				{
					// Set attribute: Pointer to the media source.
					hr = pNode->SetUnknown(MF_TOPONODE_SOURCE, _pSource);
				}

				// If successful set unknown pointer.
				if (SUCCEEDED(hr))
				{
					// Set attribute: Pointer to the presentation descriptor.
					hr = pNode->SetUnknown(MF_TOPONODE_PRESENTATION_DESCRIPTOR, pSourcePD);
				}

				// If successful set unknown pointer presentation descriptor.
				if (SUCCEEDED(hr))
				{
					// Set attribute: Pointer to the stream descriptor.
					hr = pNode->SetUnknown(MF_TOPONODE_STREAM_DESCRIPTOR, pSourceSD);
				}

				// If successful set unknown pointer stream descriptor.
				if (SUCCEEDED(hr))
				{
					// Return the IMFTopologyNode pointer to the caller.
					*ppNode = pNode;
					(*ppNode)->AddRef();
				}

				// Clean up.
				SAFE_RELEASE(pNode);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Create an output node for a stream.
			/// </summary>
			/// <param name="pSourceSD">Stream descriptor for the stream.</param>
			/// <param name="ppNode">Receives a pointer to the new node.</param>
			/// <returns>The result of the operation.</returns>
			/// <remarks>
			///  This function does the following:
			///  1. Chooses a renderer based on the media type of the stream.
			///  2. Creates an IActivate object for the renderer.
			///  3. Creates an output topology node.
			///  4. Sets the IActivate pointer on the node.
			/// </remarks>
			HRESULT MediaPlayer::CreateOutputNode(IMFStreamDescriptor *pSourceSD, IMFTopologyNode **ppNode)
			{
				IMFTopologyNode *pNode = NULL;
				IMFMediaTypeHandler *pHandler = NULL;
				IMFActivate *pRendererActivate = NULL;

				GUID guidMajorType = GUID_NULL;

				// Initally all is good.
				HRESULT hr = S_OK;

				// Get the stream ID.
				DWORD streamID = 0;

				// Just for debugging, ignore any failures.
				// Get stream identifier.
				pSourceSD->GetStreamIdentifier(&streamID);

				// Get the media type handler for the stream.
				hr = pSourceSD->GetMediaTypeHandler(&pHandler);

				// If successful get media type handler.
				if (SUCCEEDED(hr))
				{
					// Get the major media type.
					hr = pHandler->GetMajorType(&guidMajorType);
				}

				// If successful get major type.
				if (SUCCEEDED(hr))
				{
					// Create a downstream node.
					hr = MFCreateTopologyNode(MF_TOPOLOGY_OUTPUT_NODE, &pNode);
				}

				// If successful create topology.
				if (SUCCEEDED(hr))
				{
					// Create an IMFActivate object for the renderer, based on the media type.
					if (MFMediaType_Audio == guidMajorType)
					{
						// Create the audio renderer.
						hr = MFCreateAudioRendererActivate(&pRendererActivate);
					}
					else if (MFMediaType_Video == guidMajorType)
					{
						// Create the video renderer.
						hr = MFCreateVideoRendererActivate(_hwndVideo, &pRendererActivate);
					}
					else
					{
						hr = E_FAIL;
					}
				}

				// If successful create video renderer activate.
				if (SUCCEEDED(hr))
				{
					// Set the IActivate object on the output node.
					hr = pNode->SetObject(pRendererActivate);
				}

				// If successful  set renderer activate object.
				if (SUCCEEDED(hr))
				{
					// Return the IMFTopologyNode pointer to the caller.
					*ppNode = pNode;
					(*ppNode)->AddRef();
				}

				// Clean up.
				SAFE_RELEASE(pNode);
				SAFE_RELEASE(pHandler);
				SAFE_RELEASE(pRendererActivate);

				// Return the result.
				return hr;
			}

			/// <summary>
			/// Handler for MESessionTopologyReady event.
			/// </summary>
			/// <param name="pEvent">The media event handler.</param>
			/// <returns>The result of the operation.</returns>
			/// <remarks>
			///  - The MESessionTopologySet event means the session queued the 
			///    topology, but the topology is not ready yet. Generally, the 
			///    applicationno need to respond to this event unless there is an
			///    error.
			///  - The MESessionTopologyReady event means the new topology is
			///    ready for playback. After this event is received, any calls to
			///    IMFGetService will get service interfaces from the new topology.
			/// </remarks>
			HRESULT MediaPlayer::OnTopologyReady(IMFMediaEvent *pEvent)
			{
				// Ask for the IMFVideoDisplayControl interface.
				// This interface is implemented by the EVR and is
				// exposed by the media session as a service.

				// Note: This call is expected to fail if the source
				// does not have video.

				// Get the media service
				MFGetService(
					_pSession,
					MR_VIDEO_RENDER_SERVICE,
					__uuidof(IMFVideoDisplayControl),
					(void**)&_pVideoDisplay);

				// Start the media playback.
				HRESULT hr = StartPlayback();
				if (FAILED(hr))
				{
					// Notify error.
					NotifyError(hr);
				}

				// If we succeeded, the Start call is pending. Don't notify the app yet.
				return S_OK;
			}

			/// <summary>
			/// Handler for MESessionTopologyReady event.
			/// </summary>
			/// <param name="pEvent">The media event handler.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::OnSessionStarted(IMFMediaEvent *pEvent)
			{
				_playerState = Started;

				// Notify state changed.
				NotifyState();

				// Return all is good.
				return S_OK;
			}

			/// <summary>
			/// Handler for MESessionPaused event.
			/// </summary>
			/// <param name="pEvent">The media event handler.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::OnSessionPaused(IMFMediaEvent *pEvent)
			{
				_playerState = Paused;

				// Notify state changed.
				NotifyState();

				// Return all is good.
				return S_OK;
			}

			/// <summary>
			/// Handler for MESessionClosed event.
			/// </summary>
			/// <param name="pEvent">The media event handler.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::OnSessionClosed(IMFMediaEvent *pEvent)
			{
				// The application thread is waiting on this event, inside the 
				_playerState = Closed;

				// Trigger the close event handler.
				SetEvent(_hCloseEvent);

				// Return all is good.
				return S_OK;
			}

			/// <summary>
			/// Handler for MESessionStopped event.
			/// </summary>
			/// <param name="pEvent">The media event handler.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::OnSessionStopped(IMFMediaEvent *pEvent)
			{
				_playerState = Stopped;

				// Notify state changed.
				NotifyState();

				// Return all is good.
				return S_OK;
			}

			/// <summary>
			/// Handler for MEEndOfPresentation event.
			/// </summary>
			/// <param name="pEvent">The media event handler.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::OnPresentationEnded(IMFMediaEvent *pEvent)
			{
				_playerState = Ready;

				// Notify state changed.
				NotifyState();

				// Return all is good.
				return S_OK;
			}

			/// <summary>
			/// Handler for MESessionEnded event.
			/// </summary>
			/// <param name="pEvent">The media event handler.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::OnSessionEnded(IMFMediaEvent *pEvent)
			{
				_playerState = Ended;

				// Notify state changed.
				NotifyState();

				// Return all is good.
				return S_OK;
			}

			/// <summary>
			/// Set the media position.
			/// </summary>
			/// <param name="hnsPosition">The media position.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::SetPositionInternal(const MFTIME &hnsPosition)
			{

				// If the session exists.
				if (_pSession == NULL)
				{
					return MF_E_INVALIDREQUEST;
				}

				HRESULT hr = S_OK;

				// Create a property variant.
				PROPVARIANT varStart;
				varStart.vt = VT_I8;
				varStart.hVal.QuadPart = hnsPosition;

				// Start the session with the new position.
				hr = _pSession->Start(NULL, &varStart);

				if (SUCCEEDED(hr))
				{
					// Store the pending state.
					_normalState.command = CmdStart;
					_normalState.hnsStart = hnsPosition;
					_bPending = CMD_PENDING_SEEK;
				}
				return hr;
			}

			/// <summary>
			/// Commit the rate change.
			/// </summary>
			/// <param name="fRate">The new rate.</param>
			/// <param name="bThin">Is thin rate.</param>
			/// <returns>The result of the operation.</returns>
			HRESULT MediaPlayer::CommitRateChange(float fRate, BOOL bThin)
			{

				// Caller holds the lock.

				HRESULT hr = S_OK;
				MFTIME  hnsSystemTime = 0;
				MFTIME  hnsClockTime = 0;

				Command cmdNow = _normalState.command;
				IMFClock *pClock = NULL;

				// Allowed rate transitions:

				// Positive <-> negative:   Stopped
				// Negative <-> zero:       Stopped
				// Postive <-> zero:        Paused or stopped
				if ((fRate > 0 && _normalState.fRate <= 0) || (fRate < 0 && _normalState.fRate >= 0))
				{
					// Transition to stopped.
					if (cmdNow == CmdStart)
					{
						// Get the current clock position. This will be the restart time.
						hr = _pSession->GetClock(&pClock);
						if (FAILED(hr))
						{
							goto done;
						}

						(void)pClock->GetCorrelatedTime(0, &hnsClockTime, &hnsSystemTime);

						assert(hnsSystemTime != 0);

						// Stop and set the rate
						hr = Stop();
						if (FAILED(hr))
						{
							goto done;
						}

						// Cache Request: Restart from stop.
						_requestState.command = CmdSeek;
						_requestState.hnsStart = hnsClockTime;
					}
					else if (cmdNow == CmdPause)
					{
						// The current state is paused.
						// For this rate change, the session must be stopped. However, the 
						// session cannot transition back from stopped to paused. 
						// Therefore, this rate transition is not supported while paused.
						hr = MF_E_UNSUPPORTED_STATE_TRANSITION;
						goto done;
					}
				}
				else if (fRate == 0 && _normalState.fRate != 0)
				{
					if (cmdNow != CmdPause)
					{
						// Transition to paused.

						// This transisition requires the paused state.

						// Pause and set the rate.
						hr = Pause();
						if (FAILED(hr))
						{
							goto done;
						}

						// Request: Switch back to current state.
						_requestState.command = cmdNow;
					}
				}

				// Set the rate.
				hr = _pRate->SetRate(bThin, fRate);
				if (FAILED(hr))
				{
					goto done;
				}

				// Adjust our current rate and requested rate.
				_requestState.fRate = _normalState.fRate = fRate;

			done:
				SAFE_RELEASE(pClock);
				return hr;
			}

			/// <summary>
			/// Get the normal rate.
			/// </summary>
			float MediaPlayer::GetNominalRate()
			{
				return _requestState.fRate;
			}
		}
	}
}