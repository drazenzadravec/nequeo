// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

//-----------------------------------------------------------
//
// source.h
//   AudioSource class. An AudioSource object can be either
//   a tone generator (derived class SineWaveSource) or a
//   wave file reader (derived class WaveFileReader).
//
//-----------------------------------------------------------

#include <stdio.h>
#include <wtypes.h>
#include <mmreg.h>
#include <audioclient.h>

//
// Generic audio source.
//
class AudioSource
{
public:
    virtual HRESULT GetStreamStatus() = 0;
    virtual WAVEFORMATEX *GetWaveFormat() = 0;
    virtual BOOL MoreDataAvailable() = 0;
    virtual HRESULT LoadDataBytes(BYTE *pDataBuffer, ULONG NumBytes) = 0;
    virtual HRESULT ResetDataPosition() = 0;
};

// Audio sample representation
typedef enum
{
    eIeeeFloat = 1,  // IEEE float
    e8BitPcm,        // 8-bit PCM
    e16BitPcm,       // 16-bit PCM
    e32BitPcm        // 32-bit PCM

} ESampleType;

//
// Sine wave generator.
//
class SineWaveSource : public AudioSource
{
    WAVEFORMATEXTENSIBLE m_wfx;
    ESampleType m_sampleType;
    HRESULT m_hrStreamStatus;
    ULONG m_NumSamples;
    double m_x;
    double m_y;
    double m_cosdt;
    double m_sindt;

public:
    SineWaveSource(WAVEFORMATEX *pFormat);
    ~SineWaveSource() {};
    HRESULT GetStreamStatus() { return m_hrStreamStatus; };
    WAVEFORMATEX *GetWaveFormat() { return &m_wfx.Format; };
    BOOL MoreDataAvailable() { return (m_hrStreamStatus == S_OK); };
    HRESULT LoadDataBytes(BYTE *pDataBuffer, ULONG NumBytes);
    HRESULT ResetDataPosition();
};


//
// Wave file reader.
//
class WaveFileReader : public AudioSource
{
    FILE *m_pFile;
    HRESULT m_hrStreamStatus;
    fpos_t m_dataChunkPosition;
    ULONG m_totalDataBytes;
    ULONG m_dataBytesRemaining;
    WAVEFORMATEXTENSIBLE m_wfx;
    BOOL m_repeatMode;

public:
    WaveFileReader(LPCWSTR pszFileName, BOOL RepeatMode);
    ~WaveFileReader();
    HRESULT GetStreamStatus() { return m_hrStreamStatus; };
    WAVEFORMATEX *GetWaveFormat()
    {
        return (m_hrStreamStatus==S_OK) ? &m_wfx.Format : NULL;
    };
    BOOL MoreDataAvailable()
    {
        return (m_hrStreamStatus == S_OK &&
                (m_repeatMode == TRUE || m_dataBytesRemaining > 0));
    };
    HRESULT LoadDataBytes(BYTE *pDataBuffer, ULONG NumBytes);
    HRESULT ResetDataPosition();
};
