// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.


//-----------------------------------------------------------
//
// capture.cpp
//
//   Capture a wave stream from an endpoint capture device
//   and play the stream on the endpoint rendering device.
//   The PlayCaptureStream function creates two streams --
//   one on the capture device, and one on the rendering
//   device. Then, as the data arrives in the capture buffer
//   from the capture device, the function simply copies the
//   data from the capture buffer to the rendering buffer so
//   that the rendering device can play it.
//
//-----------------------------------------------------------

#include <assert.h>
#include <wtypes.h>
#include <winerror.h>
#include <objbase.h>
#include <mmreg.h>
#include <ks.h>
#include <ksmedia.h>
#include <ksguid.h>
#include "player.h"

// MFTIME time units per second and per millisecond
#define MFTIMES_PER_MILLISEC  10000

#define EXIT_ON_ERROR(hres)  \
              if (FAILED(hres)) { goto Exit; }
#define SAFE_RELEASE(punk)  \
              if ((punk) != NULL)  \
                { (punk)->Release(); (punk) = NULL; }

//
// Audio capture and playback thread -- Launched by Player
//
DWORD WINAPI PlayCaptureStream(LPVOID pPlayerObject)
{
    HRESULT hr = S_OK;
    WAVEFORMATEX *pWfx = NULL;
    IAudioRenderClient *pRenderClient = NULL;
    IAudioCaptureClient *pCaptureClient = NULL;

    // Specify a sleep period of 50 milliseconds.
    DWORD sleepPeriod = 50;

    // Request a buffer duration of 100 milliseconds.
    REFERENCE_TIME bufferDuration = 2 * sleepPeriod * MFTIMES_PER_MILLISEC;

    CoInitializeEx(NULL, COINIT_MULTITHREADED);

    Player *pPlayer = (Player*)pPlayerObject;
    assert(pPlayer != NULL);

    IAudioClient *pClientOut = pPlayer->m_pClientOut;
    assert(pClientOut != NULL);

    IAudioClient *pClientIn = pPlayer->m_pClientIn;
    assert(pClientIn != NULL);

    // Get the capture stream format. (Later on, remember to free
    // *pWfx by calling CoTaskMemFree.)
    hr = pClientIn->GetMixFormat(&pWfx);
    EXIT_ON_ERROR(hr)
    ULONG frameSize = pWfx->nChannels * pWfx->wBitsPerSample / 8;

    // Create a rendering stream with the same format as capture stream.
    hr = pClientOut->Initialize(AUDCLNT_SHAREMODE_SHARED,  // shared mode
                                0,                         // stream flags
                                bufferDuration,            // buffer duration
                                0,                         // periodicity
                                pWfx,                      // wave format
                                NULL);                     // session GUID
    EXIT_ON_ERROR(hr)

    hr = pClientOut->GetService(__uuidof(IAudioRenderClient),
                                (void**)&pRenderClient);
    EXIT_ON_ERROR(hr)

    // Create the capture stream.
    hr = pClientIn->Initialize(AUDCLNT_SHAREMODE_SHARED,  // shared mode
                               0,                         // stream flags
                               bufferDuration,            // buffer duration
                               0,                         // periodicity
                               pWfx,                      // wave format
                               NULL);                     // session GUID
    EXIT_ON_ERROR(hr)

    hr = pClientIn->GetService(__uuidof(IAudioCaptureClient),
                               (void**)&pCaptureClient);
    EXIT_ON_ERROR(hr)

    // Get lengths of allocated capture and rendering buffers.
    UINT32 bufferLengthIn = 0;
    hr = pClientOut->GetBufferSize(&bufferLengthIn);
    EXIT_ON_ERROR(hr)

    UINT32 bufferLengthOut = 0;
    hr = pClientOut->GetBufferSize(&bufferLengthOut);
    EXIT_ON_ERROR(hr)

    // Initial conditions: Before starting the stream, fill the
    // rendering buffer with silence.
    BYTE *pDataOut = NULL;
    hr = pRenderClient->GetBuffer(bufferLengthOut, &pDataOut);
    EXIT_ON_ERROR(hr)

    hr = pRenderClient->ReleaseBuffer(bufferLengthOut, AUDCLNT_BUFFERFLAGS_SILENT);
    EXIT_ON_ERROR(hr)

    // Start up the capture and rendering streams.
    hr = pClientIn->Start();
    EXIT_ON_ERROR(hr)

    hr = pClientOut->Start();
    EXIT_ON_ERROR(hr)

    // Each loop below copies one device period's worth of data
    // from the capture buffer to the rendering buffer.
    while (pPlayer->m_keepPlaying == TRUE)
    {
        // Sleep for one device period.
        Sleep(sleepPeriod);

        while (pPlayer->m_keepPlaying == TRUE)
        {
            // See how much space is available in render buffer.
            UINT32 padding = 0;
            hr = pClientOut->GetCurrentPadding(&padding);
            EXIT_ON_ERROR(hr)

            UINT32 available = bufferLengthOut - padding;

            UINT32 packetLength = 0;
            hr = pCaptureClient->GetNextPacketSize(&packetLength);
            EXIT_ON_ERROR(hr)

            if (packetLength == 0)
            {
                // No capture packet is available right now.
                // Sleep for a while...
                break;
            }

            if (packetLength > available)
            {
                // Not enough space in render buffer to store
                // next capture packet. Sleep for a while...
                break;
            }

            // Get pointer to next data packet in capture buffer.
            BYTE *pDataIn = 0;
            UINT32 packetLength2 = 0;
            DWORD flags = 0;
            hr = pCaptureClient->GetBuffer(&pDataIn, &packetLength2,
                                           &flags, NULL, NULL);
            EXIT_ON_ERROR(hr)
            assert(packetLength == packetLength2);

            // If the silence flag is set on the capture buffer,
            // pass the flag to the render buffer.
            flags &= AUDCLNT_BUFFERFLAGS_SILENT;

            // Get pointer to next space in render buffer.
            hr = pRenderClient->GetBuffer(packetLength, &pDataOut);
            EXIT_ON_ERROR(hr)

            // Unless the silence flag is set, copy the data packet
            // from the capture buffer to the render buffer.
            if (flags == 0)
            {
                // Calculate the packet size in bytes.
                UINT32 packetSize = packetLength * frameSize;

                // This app has only two threads, and we know that the
                // other thread doesn't call memcpy, so *maybe* we can
                // get away with not using the multithreaded CRT lib.
                memcpy(pDataOut, pDataIn, packetSize);
            }

            hr = pCaptureClient->ReleaseBuffer(packetLength);
            EXIT_ON_ERROR(hr)

            hr = pRenderClient->ReleaseBuffer(packetLength, flags);
            EXIT_ON_ERROR(hr)
        }
    }

    // Stop the capture and playback streams.
    hr = pClientOut->Stop();
    EXIT_ON_ERROR(hr)

    hr = pClientIn->Stop();
    EXIT_ON_ERROR(hr)

Exit:
    SAFE_RELEASE(pPlayer->m_pClientIn)
    SAFE_RELEASE(pPlayer->m_pClientOut)
    SAFE_RELEASE(pRenderClient)
    SAFE_RELEASE(pCaptureClient)
    CoTaskMemFree(pWfx);

    if (pPlayer->m_keepPlaying == TRUE)
    {
        // The stream is stopping, but not because the client
        // told us to stop it. Unless we send this notification,
        // the client won't know that the stream is stopping.
        if (pPlayer->m_pPlayerCallbacks != NULL)
        {
            pPlayer->m_pPlayerCallbacks->PlayerStopCallback();
        }
    }

    return(DWORD)hr;
}

