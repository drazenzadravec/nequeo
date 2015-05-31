// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

//-----------------------------------------------------------
//
// sinewave.cpp
//
//   Implementation of SineWaveSource class. A SineWaveSource
//   object generates a wave stream that consists of a 523-Hz
//   sine wave.
//
//-----------------------------------------------------------

#include <math.h>
#include <assert.h>
#include <wtypes.h>
#include <basetsd.h>
#include <mmreg.h>
#include <ks.h>
#include <ksmedia.h>
#include "source.h"

// Celebrity constant
#define PI  3.14159265358979323846

// Amplitude of generated sine wave
#define SINE_WAVE_AMPLITUDE  0.7L

// Middle C frequency (in Hz)
#define MIDDLE_C_FREQ  261.63L

// Renormalize after generating this many samples.
#define MAX_SAMPLE_COUNT  100000

//
// Constructor -- Open wave file and parse file header.
//
SineWaveSource::SineWaveSource(WAVEFORMATEX *pFormat)
{
    m_hrStreamStatus = S_OK;

    // Assume that the input format is WAVEFORMATEXTENSIBLE.
    assert(pFormat->wFormatTag == WAVE_FORMAT_EXTENSIBLE);
    assert(pFormat->cbSize >= sizeof(WAVEFORMATEXTENSIBLE) - sizeof(WAVEFORMATEX));

    memcpy(&m_wfx, pFormat, sizeof(m_wfx));

    if (m_wfx.SubFormat != KSDATAFORMAT_SUBTYPE_IEEE_FLOAT &&
        m_wfx.SubFormat != KSDATAFORMAT_SUBTYPE_PCM)
    {
        // Can't handle the specified format.
        m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;
        return;
    }

    if (m_wfx.SubFormat == KSDATAFORMAT_SUBTYPE_IEEE_FLOAT)
    {
        m_sampleType = eIeeeFloat;
    }
    else
    {
        switch (m_wfx.Format.wBitsPerSample)
        {
        case 8:
            m_sampleType = e8BitPcm;
            break;
        case 16:
            m_sampleType = e16BitPcm;
            break;
        case 32:
            m_sampleType = e32BitPcm;
            break;
        default:
            // Can't handle the specified format.
            m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;
        }
    }

    // This object generates a 523-Hz sine wave (one octave above
    // middle C) regardless of the sample rate of the stream.
    // Calculate the number of samples to generate for each
    // period of the sine wave.
    double samplesPerPeriod = (double)m_wfx.Format.nSamplesPerSec /
                              (2 * MIDDLE_C_FREQ);

    // Calculate the number of radians between successive samples.
    double dt = 2 * PI / samplesPerPeriod;

    // Use the following formulas to incrementally calculate successive
    // sine wave samples:
    //           x[n] = x[n-1] * cos(dt) - y[n-1] * sin(dt)
    //           y[n] = x[n-1] * sin(dt) + y[n-1] * cos(dt)
    //
    // Use successive values of y as the generated sine wave.
    m_sindt = sin(dt);  // define loop constant
    m_cosdt = cos(dt);  // define loop constant
    m_x = SINE_WAVE_AMPLITUDE;    // initial value
    m_y = 0;                      // initial value

    // The incremental formulas above accumulate error over time.
    // Monitor the error by keeping track of how many samples we've
    // generated since the last renormalization of m_x and m_y.
    m_NumSamples = 0;
}

//
// Reset the stream.
//
HRESULT SineWaveSource::ResetDataPosition()
{
    if (m_hrStreamStatus != S_OK)
    {
        return m_hrStreamStatus;  // oops -- can't handle this format
    }

    // Set initial cosine and sine values.
    m_x = SINE_WAVE_AMPLITUDE;
    m_y = 0;

    return S_OK;
}

//
// Load next block of wave data from file.
//
HRESULT SineWaveSource::LoadDataBytes(BYTE *pBuffer, ULONG NumBytes)
{
    int NumSamples = 0;

    if (m_hrStreamStatus != S_OK)
    {
        return m_hrStreamStatus;  // oops -- can't handle this format
    }
    if (pBuffer == NULL)
    {
        return E_POINTER;
    }
    if (NumBytes == 0)
    {
        return E_INVALIDARG;
    }

    switch (m_sampleType)
    {
    case eIeeeFloat:
        {
            float *pData = (float*)pBuffer;
            NumSamples = NumBytes / sizeof(float) / m_wfx.Format.nChannels;
            for (int i = 0; i < NumSamples; i++)
            {
                for (int j = 0; j < m_wfx.Format.nChannels; j++)
                {
                    *pData++ = (float)m_y;
                }
                double x = m_x * m_cosdt - m_y * m_sindt;
                double y = m_x * m_sindt + m_y * m_cosdt;
                m_x = x;
                m_y = y;
            }
        }
        break;
    case e8BitPcm:
        {
            BYTE *pData = (BYTE*)pBuffer;
            NumSamples = NumBytes / m_wfx.Format.nChannels;
            for (int i = 0; i < NumSamples; i++)
            {
                for (int j = 0; j < m_wfx.Format.nChannels; j++)
                {
                    *pData++ = BYTE(0x7F * m_y + 0x80);
                }
                double x = m_x * m_cosdt - m_y * m_sindt;
                double y = m_x * m_sindt + m_y * m_cosdt;
                m_x = x;
                m_y = y;
            }
        }
        break;
    case e16BitPcm:
        {
            INT16 *pData = (INT16*)pBuffer;
            NumSamples = NumBytes / sizeof(INT16) / m_wfx.Format.nChannels;
            for (int i = 0; i < NumSamples; i++)
            {
                for (int j = 0; j < m_wfx.Format.nChannels; j++)
                {
                    *pData++ = INT16(0x7FFF * m_y);
                }
                double x = m_x * m_cosdt - m_y * m_sindt;
                double y = m_x * m_sindt + m_y * m_cosdt;
                m_x = x;
                m_y = y;
            }
        }
        break;
    case e32BitPcm:
        {
            INT32 *pData = (INT32*)pBuffer;
            NumSamples = NumBytes / sizeof(INT32) / m_wfx.Format.nChannels;
            for (int i = 0; i < NumSamples; i++)
            {
                for (int j = 0; j < m_wfx.Format.nChannels; j++)
                {
                    *pData++ = INT32(0x7FFFFFFF * m_y);
                }
                double x = m_x * m_cosdt - m_y * m_sindt;
                double y = m_x * m_sindt + m_y * m_cosdt;
                m_x = x;
                m_y = y;
            }
        }
        break;
    default:
        assert(0);
        break;
    }

    // The incremental formula that we use to generate successive sine wave
    // samples gradually accumulates error. Occasional correction is needed.
    m_NumSamples += NumSamples;
    if (m_NumSamples > MAX_SAMPLE_COUNT)
    {
        // Renormalize (m_x, m_y) to compensate for accumulated error.
        double amplitude = sqrt(m_x * m_x + m_y * m_y);
        m_x *= SINE_WAVE_AMPLITUDE / amplitude;
        m_y *= SINE_WAVE_AMPLITUDE / amplitude;
        m_NumSamples = 0;
    }

    return S_OK;
}



