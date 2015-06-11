/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          MediaPlayer.h
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

#pragma once

#ifndef _MEDIAPLAYER_H
#define _MEDIAPLAYER_H

#include "MediaGlobal.h"
#include "PlayerState.h"

namespace Nequeo {
	namespace Media {
		namespace Foundation
		{
			/// <summary>
			/// Providers the base for a media foundation player.
			/// </summary>
			class MediaPlayer : public IMFAsyncCallback
			{
			public:
				/// <summary>
				/// Static class method to create the MediaPlayer object.
				/// </summary>
				/// <param name="hVideo">The handle to the video window.</param>
				/// <param name="hEvent">The handle to the window to receive notifications.</param>
				/// <param name="ppPlayer">Receives an AddRef's pointer to the MediaPlayer object. The caller must release the pointer.</param>
				/// <returns>The result of the operation.</returns>
				static HRESULT CreateInstance(HWND hVideo, HWND hEvent, MediaPlayer **ppPlayer);

				/// <summary>
				/// Add a new player ref item.
				/// </summary>
				/// <returns>The result.</returns>
				STDMETHODIMP_(ULONG) AddRef();

				/// <summary>
				/// Release this player resources.
				/// </summary>
				/// <returns>The result.</returns>
				STDMETHODIMP_(ULONG) Release();

				/// <summary>
				/// Get the player reference for the reference id.
				/// </summary>
				/// <param name="iid">The player reference id.</param>
				/// <param name="ppv">The current player reference.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP QueryInterface(REFIID iid, void** ppv);

				/// <summary>
				/// Get the player parameters.
				/// </summary>
				/// <param name="p1">The player parameter.</param>
				/// <param name="p2">The player parameter.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP GetParameters(DWORD* p1, DWORD* p2)
				{
					// Implementation of this method is optional.
					return E_NOTIMPL;
				}

				/// <summary>
				/// Callback for asynchronous BeginGetEvent method.
				/// </summary>
				/// <param name="pAsyncResult">The pointer to the result.</param>
				/// <returns>The result of the operation.</returns>
				STDMETHODIMP Invoke(IMFAsyncResult* pAsyncResult);

				/// <summary>
				/// Opena URL resources, can be a file, network or internet resource.
				/// </summary>
				/// <param name="sURL">The string of the URL.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT		OpenURL(const WCHAR *sURL);

				/// <summary>
				/// Start playing the media.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				HRESULT		Play();

				/// <summary>
				/// Pause playback of the media.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				HRESULT		Pause();

				/// <summary>
				/// Stop playback of the media.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				HRESULT		Stop();

				/// <summary>
				/// Shutdown of the media. Releases all resources held by this object.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				HRESULT		Shutdown();

				/// <summary>
				/// Repaint the video window. The application should call this method when it receives a WM_PAINT message.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				HRESULT		Repaint();

				/// <summary>
				/// Repaint the video window. The application should call this method when it receives a WM_SIZE message.
				/// </summary>
				/// <param name="width">The width of the player.</param>
				/// <param name="height">The height of the player.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT		ResizeVideo(WORD width, WORD height);

				/// <summary>
				/// Get the current state of the media playback.
				/// </summary>
				/// <returns>The playback state.</returns>
				PlayerState GetState() const { return _state; }

				/// <summary>
				/// Get an indicator specifying if a video display control has been set.
				/// </summary>
				/// <returns>True if a video display control has ben set; else false.</returns>
				BOOL		HasVideo() const { return (_pVideoDisplay != NULL); }

			protected:
				/// <summary>
				/// Constructor for the current class. Use static CreateInstance method to instantiate.
				/// </summary>
				/// <param name="hVideo">The handle to the video window.</param>
				/// <param name="hEvent">The handle to the window to receive notifications.</param>
				/// <param name="hr">The result reference.</param>
				MediaPlayer(HWND hVideo, HWND hEvent, HRESULT &hr);

				/// <summary>
				/// This destructor. call release to cleanup resources.
				/// </summary>
				virtual ~MediaPlayer();

				/// <summary>
				/// Initializes the CPlayer object. This method is called by the
				/// CreateInstance method.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				virtual HRESULT Initialize();

				/// <summary>
				/// Notifies the application when the state changes.
				/// </summary>
				void NotifyState()
				{
					PostMessage(_hwndEvent, WM_APP_NOTIFY, (WPARAM)_state, (LPARAM)0);
				}

				/// <summary>
				/// Notifies the application when an error occurs.
				/// </summary>
				void NotifyError(HRESULT hr)
				{
					_state = Ready;
					PostMessage(_hwndEvent, WM_APP_ERROR, (WPARAM)hr, 0);
				}

				/// <summary>
				/// Creates a new instance of the media session.
				/// </summary>
				/// <returns>The result of the operation.</returns>
				HRESULT CreateSession();

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
				HRESULT CloseSession();

				/// <summary>
				/// Starts playback from the current position. 
				/// </summary>
				/// <returns>The result of the operation.</returns>
				HRESULT StartPlayback();

				/// <summary>
				/// Create a media source from a URL, can be a file, network or internet resource.
				/// </summary>
				/// <param name="sURL">The string of the URL.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT CreateMediaSource(const WCHAR *sURL);

				/// <summary>
				/// Create a playback topology from the media source.
				/// </summary>
				/// <param name="ppTopology">The topology.</param>
				/// <returns>The result of the operation.</returns>
				/// <remarks>
				/// The media source must be created already.
				/// Call CreateMediaSource() before calling this method.
				/// </remarks>
				HRESULT CreateTopologyFromSource(IMFTopology **ppTopology);

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
				HRESULT AddBranchToPartialTopology(
					IMFTopology *pTopology,
					IMFPresentationDescriptor *pSourcePD,
					DWORD iStream);

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
				HRESULT CreateSourceStreamNode(
					IMFPresentationDescriptor *pSourcePD,
					IMFStreamDescriptor *pSourceSD,
					IMFTopologyNode **ppNode);

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
				HRESULT CreateOutputNode(
					IMFStreamDescriptor *pSourceSD,
					IMFTopologyNode **ppNode);

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
				HRESULT OnTopologyReady(IMFMediaEvent *pEvent);

				/// <summary>
				/// Handler for MESessionStarted event.
				/// </summary>
				/// <param name="pEvent">The media event handler.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT OnSessionStarted(IMFMediaEvent *pEvent);

				/// <summary>
				/// Handler for MESessionPaused event.
				/// </summary>
				/// <param name="pEvent">The media event handler.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT OnSessionPaused(IMFMediaEvent *pEvent);

				/// <summary>
				/// Handler for MESessionClosed event.
				/// </summary>
				/// <param name="pEvent">The media event handler.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT OnSessionClosed(IMFMediaEvent *pEvent);

				/// <summary>
				/// Handler for MESessionStopped event.
				/// </summary>
				/// <param name="pEvent">The media event handler.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT OnSessionStopped(IMFMediaEvent *pEvent);

				/// <summary>
				/// Handler for MEEndOfPresentation event.
				/// </summary>
				/// <param name="pEvent">The media event handler.</param>
				/// <returns>The result of the operation.</returns>
				HRESULT OnPresentationEnded(IMFMediaEvent *pEvent);

			private:
				bool _disposed;
				long                    _nRefCount;			// Reference count.

				IMFMediaSession			*_pSession;			// Media session.
				IMFMediaSource			*_pSource;			// Media sources.
				IMFVideoDisplayControl	*_pVideoDisplay;	// The media video display control.

				HWND				    _hwndVideo;			// Video window.
				HWND				    _hwndEvent;			// App window to receive events.
				PlayerState			    _state;				// Current state of the media session.
				HANDLE				    _hCloseEvent;		// Event to wait on while closing.

			};
		}
	}
}
#endif