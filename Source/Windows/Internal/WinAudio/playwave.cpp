// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

//-----------------------------------------------------------
//
// playwave.cpp
//
//   Audio playback thread for a wave file or tone generator.
//   The PlayWaveStream function plays a wave stream on the
//   endpoint rendering device.
//
//-----------------------------------------------------------

#include <assert.h>
#include "player.h"
#include "source.h"

extern HRESULT ValidateWaveFormatEx(WAVEFORMATEX *pWfx);

// REFERENCE_TIME time units per millisecond
#define MFTIMES_PER_MILLISEC  10000

#define EXIT_ON_ERROR(hres)  \
              if (FAILED(hres)) { goto Exit; }
#define SAFE_RELEASE(punk)  \
              if ((punk) != NULL)  \
                { (punk)->Release(); (punk) = NULL; }

//
// Audio playback thread -- Launched by Player
//
DWORD WINAPI PlayWaveStream(LPVOID pPlayerObject)
{
    HRESULT hr = S_OK;
    WAVEFORMATEX *pWfxOut = NULL;
    IAudioRenderClient *pRenderClient = NULL;
    AudioSource *pSource = NULL;
    SineWaveSource *pSineWave = NULL;
    WaveFileReader *pWaveReader = NULL;

    // Specify a sleep period of 50 milliseconds.
    DWORD sleepPeriod = 50;

    // Request a buffer duration of 200 milliseconds.
    REFERENCE_TIME bufferDuration = 4 * sleepPeriod * MFTIMES_PER_MILLISEC;

    CoInitializeEx(NULL, COINIT_MULTITHREADED);

    // Get rendering device's IAudioClient interface from player.
    Player *pPlayer = (Player*)pPlayerObject;
    assert(pPlayer != NULL);

    IAudioClient *pClientOut = pPlayer->m_pClientOut;
    assert(pClientOut != NULL);

    // Get rendering device's wave format. (Later on, remember
    // to free *pWfxOut by calling CoTaskMemFree.)
    hr = pClientOut->GetMixFormat(&pWfxOut);
    EXIT_ON_ERROR(hr)

    // Did user want to use a wave file or the tone
    // generator as the source for the audio stream?
    switch (pPlayer->m_audioSourceType)
    {
    case eToneGenerator:
        pSineWave = new SineWaveSource(pWfxOut);
        pSource = pSineWave;
        break;
    case eWaveFile:
        pWaveReader = new WaveFileReader(pPlayer->m_szFileName,
                                         pPlayer->m_RepeatMode);
        pSource = pWaveReader;
        break;
    }
    if (pSource == NULL)
    {
        EXIT_ON_ERROR(hr = E_OUTOFMEMORY)
    }

    hr = pSource->GetStreamStatus();
    if (hr != S_OK)
    {
        MessageBox(pPlayer->m_hDlg,
                   L"The application does not support the source wave format",
                   L"Cannot Play Audio", MB_OK);
    }
    EXIT_ON_ERROR(hr)

    // Get stream format for audio source. (The pWfxIn pointer remains
    // valid until the *pSource object is deleted.)
    WAVEFORMATEX *pWfxIn = NULL;
    pWfxIn = pSource->GetWaveFormat();
    assert(pWfxIn != NULL);

    // The upcoming Initialize call will fail if the source format
    // is not valid. If so, tell the user...
    hr = ValidateWaveFormatEx(pWfxIn);
    if (hr == S_OK)
    {
        // The upcoming Initialize call will fail if the source format is incompatible
        // with the mix format. If so, tell the user...
        WAVEFORMATEX *pWfxTemp = NULL;
        hr = pClientOut->IsFormatSupported(AUDCLNT_SHAREMODE_SHARED, pWfxIn, &pWfxTemp);
        assert(hr != S_OK || pWfxTemp == NULL);
        CoTaskMemFree(pWfxTemp);
    }
    if (hr != S_OK)
    {
        MessageBox(pPlayer->m_hDlg,
                   L"The application does not support the source wave format",
                   L"Cannot Play Audio", MB_OK);
        EXIT_ON_ERROR(hr = E_FAIL)
    }

    // Create a rendering stream with the same format as the audio source.
    hr = pClientOut->Initialize(AUDCLNT_SHAREMODE_SHARED,  // shared mode
                                0,                         // stream flags
                                bufferDuration,            // buffer duration
                                0,                         // periodicity
                                pWfxIn,                    // wave format
                                NULL);                     // session GUID
    EXIT_ON_ERROR(hr)

    hr = pClientOut->GetService(__uuidof(IAudioRenderClient),
                                (void**)&pRenderClient);
    EXIT_ON_ERROR(hr)

    // Calculate the size in bytes of an audio frame.
    ULONG frameSize = pWfxIn->nChannels * pWfxIn->wBitsPerSample / 8;

    // Get length of allocated rendering buffer.
    UINT32 bufferLengthOut = 0;
    hr = pClientOut->GetBufferSize(&bufferLengthOut);
    EXIT_ON_ERROR(hr)

    // Before starting stream, set up initial conditions.
    BYTE *pDataOut = NULL;
    hr = pRenderClient->GetBuffer(bufferLengthOut, &pDataOut);
    EXIT_ON_ERROR(hr)

    pSource->LoadDataBytes(pDataOut, bufferLengthOut * frameSize);

    hr = pRenderClient->ReleaseBuffer(bufferLengthOut, 0);
    EXIT_ON_ERROR(hr)

    // Start up the rendering stream.
    hr = pClientOut->Start();
    EXIT_ON_ERROR(hr)

    // Each loop copies one device period's worth of data
    // from the audio source to the rendering buffer.
    while (pPlayer->m_keepPlaying == TRUE &&
           pSource->MoreDataAvailable() == TRUE)
    {
        // Sleep for one device period.
        Sleep(sleepPeriod);

        // See how much space is available in render buffer.
        UINT32 padding = 0;
        hr = pClientOut->GetCurrentPadding(&padding);
        EXIT_ON_ERROR(hr)

        UINT32 available = bufferLengthOut - padding;

        if (available != 0)
        {
            // Get pointer to next space in render buffer.
            hr = pRenderClient->GetBuffer(available, &pDataOut);
            EXIT_ON_ERROR(hr)

            // Load data from audio source into rendering buffer.
            pSource->LoadDataBytes(pDataOut, available * frameSize);

            hr = pRenderClient->ReleaseBuffer(available, 0);
            EXIT_ON_ERROR(hr)
        }
    }

    // Sleep for one device period.
    Sleep(sleepPeriod);

    // Stop playback stream.
    hr = pClientOut->Stop();
    EXIT_ON_ERROR(hr)

Exit:
    SAFE_RELEASE(pPlayer->m_pClientOut)
    SAFE_RELEASE(pRenderClient)
    CoTaskMemFree(pWfxOut);
    delete pSineWave;
    delete pWaveReader;
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

    return (DWORD)hr;
}

