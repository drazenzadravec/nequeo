// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

//-----------------------------------------------------------
//
// validwfx.cpp
//
//   The ValidateWaveFormatEx function validates a
//   wave format descriptor.
//
//-----------------------------------------------------------

#include <wtypes.h>
#include <mmreg.h>
#include <ks.h>
#include <ksmedia.h>

extern HRESULT ValidateWaveFormatEx(WAVEFORMATEX *pWfx);

//
// Verify that WAVEFORMATEX structure is valid.
//
HRESULT ValidateWaveFormatEx(WAVEFORMATEX *pWfx)
{
    HRESULT hr = S_OK;

    if ((0 == pWfx->nChannels) ||
        (0 == pWfx->nSamplesPerSec) ||
        (0 == pWfx->nAvgBytesPerSec) ||
        (0 == pWfx->nBlockAlign) ||
        (1024 < pWfx->cbSize))
    {
        hr = E_INVALIDARG;
        goto Exit;
    }

    if (WAVE_FORMAT_PCM == pWfx->wFormatTag || WAVE_FORMAT_IEEE_FLOAT == pWfx->wFormatTag)
    {
        if ((0 != pWfx->cbSize) ||
            (0 != (pWfx->wBitsPerSample % 8)) ||
            (2 < pWfx->nChannels) ||
            (pWfx->nAvgBytesPerSec != (pWfx->nChannels * pWfx->nSamplesPerSec * pWfx->wBitsPerSample / 8)))
        {
            hr = E_INVALIDARG;
            goto Exit;
        }
    }
    else if (WAVE_FORMAT_EXTENSIBLE == pWfx->wFormatTag)
    {
        WAVEFORMATEXTENSIBLE *pWfxx = reinterpret_cast<WAVEFORMATEXTENSIBLE*>(pWfx);

        if (((sizeof(WAVEFORMATEXTENSIBLE) - sizeof(WAVEFORMATEX)) > pWfx->cbSize))
        {
            hr = E_INVALIDARG;
            goto Exit;
        }

        if ((0 == pWfxx->Samples.wValidBitsPerSample) ||
            (pWfx->wBitsPerSample < pWfxx->Samples.wValidBitsPerSample))
        {
            hr = E_INVALIDARG;
            goto Exit;
        }
        if ((KSDATAFORMAT_SUBTYPE_PCM == pWfxx->SubFormat) ||
            (KSDATAFORMAT_SUBTYPE_IEEE_FLOAT == pWfxx->SubFormat))
        {
            if (pWfx->nAvgBytesPerSec != (pWfx->nChannels * pWfx->nSamplesPerSec * pWfx->wBitsPerSample / 8))
            {
                hr = E_INVALIDARG;
                goto Exit;
            }
        }
    }

Exit:
    return hr;
}
