// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

//-----------------------------------------------------------
//
// wavefile.cpp
//
//   Wave file reader.
//
//-----------------------------------------------------------

#include <stdio.h>
#include <assert.h>
#include <wtypes.h>
#include <mmreg.h>
#include <ks.h>
#include <ksmedia.h>
#include "source.h"

// To parse a wave file, it is helpful to have two user-defined types.
// The first will receive all the information in the file header and
// in the header of the format chunk. The second will receive the
// information in the headers for the data chunks, and, if the file
// contains additional chunks, the headers for those chunks as well.

typedef struct
{
    ULONG Riff4CC;      // "RIFF" 4-character code
    ULONG FileSize;     // total file size in bytes
    ULONG Wave4CC;      // "WAVE" 4-character code
    ULONG Fmt4CC;       // "fmt " 4-character code
    ULONG FormatSize;   // wave format size in bytes
} FileHeader;

typedef struct
{
    ULONG ChunkType;
    ULONG ChunkSize;
} ChunkHeader;

// Any file smaller than this cannot possibly contain wave data.
#define MIN_WAVE_FILE_SIZE (sizeof(FileHeader)+sizeof(PCMWAVEFORMAT)+sizeof(ChunkHeader)+1)

// Macro to build FOURCC from first four characters in ASCII string
#define FOURCC(s)  ((ULONG)(s[0] | (s[1]<<8) | (s[2]<<16) | (s[3]<<24)))

//
// Constructor -- Open wave file and parse file header.
//
WaveFileReader::WaveFileReader(LPCWSTR pszFileName, BOOL repeat)
{
    m_pFile = NULL;
    m_hrStreamStatus = S_OK;
    m_dataChunkPosition = 0;
    m_totalDataBytes = 0;
    m_dataBytesRemaining = 0;
    m_repeatMode = repeat;
    ZeroMemory(&m_wfx, sizeof(m_wfx));

    // Try to open the wave file.
    if (_wfopen_s(&m_pFile, pszFileName, L"rb") != 0)
    {
        m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;  // failed to open wave file
        return;
    }

    // Copy header from wave file.
    FileHeader fileHdr;

    if (fread(&fileHdr, sizeof(fileHdr), 1, m_pFile) != 1)
    {
        m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;  // not a valid wave file
        return;
    }

    // Verify that wave file header is valid.
    if (fileHdr.Riff4CC != FOURCC("RIFF") ||
        fileHdr.FileSize < MIN_WAVE_FILE_SIZE ||
        fileHdr.Wave4CC != FOURCC("WAVE") ||
        fileHdr.Fmt4CC != FOURCC("fmt ") ||
        fileHdr.FormatSize < sizeof(PCMWAVEFORMAT))
    {
        m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;  // not a valid wave file
        return;
    }

    // Copy wave format descriptor from file.
    if (fread(&m_wfx, min(fileHdr.FormatSize,sizeof(m_wfx)), 1, m_pFile) != 1)
    {
        m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;  // not a valid wave file
        return;
    }

    // Skip over any padding at the end of the format in the format chunk.
    if (fileHdr.FormatSize > sizeof(m_wfx))
    {
        if (fseek(m_pFile, fileHdr.FormatSize-sizeof(m_wfx), SEEK_CUR) != 0)
        {
            // m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;
            return;  // not a valid wave file
        }
    }

    // If format type is PCMWAVEFORMAT, convert to valid WAVEFORMATEX structure.
    if (m_wfx.Format.wFormatTag == WAVE_FORMAT_PCM)
    {
        m_wfx.Format.cbSize = 0;
    }

    // If format type is WAVEFORMATEX, convert to WAVEFORMATEXTENSIBLE.
    if (m_wfx.Format.wFormatTag == WAVE_FORMAT_PCM ||
        m_wfx.Format.wFormatTag == WAVE_FORMAT_IEEE_FLOAT)
    {
        if (m_wfx.Format.wFormatTag == WAVE_FORMAT_PCM)
        {
            m_wfx.SubFormat = KSDATAFORMAT_SUBTYPE_PCM;
        }
        else
        {
            m_wfx.SubFormat = KSDATAFORMAT_SUBTYPE_IEEE_FLOAT;
        }
        m_wfx.Format.wFormatTag = WAVE_FORMAT_EXTENSIBLE;

        // Note that the WAVEFORMATEX structure is valid for
        // representing wave formats with only 1 or 2 channels.
        if (m_wfx.Format.nChannels == 1)
        {
            m_wfx.dwChannelMask = SPEAKER_FRONT_CENTER;
        }
        else if (m_wfx.Format.nChannels == 2)
        {
            m_wfx.dwChannelMask = SPEAKER_FRONT_LEFT | SPEAKER_FRONT_RIGHT;
        }
        else
        {
            m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;  // not a valid wave format
            return;
        }
        m_wfx.Format.cbSize = sizeof(WAVEFORMATEXTENSIBLE) - sizeof(WAVEFORMATEX);
        m_wfx.Samples.wValidBitsPerSample = m_wfx.Format.wBitsPerSample;
    }

    // This wave file reader understands only PCM and IEEE float formats.
    if (m_wfx.Format.wFormatTag != WAVE_FORMAT_EXTENSIBLE ||
        m_wfx.SubFormat != KSDATAFORMAT_SUBTYPE_PCM &&
        m_wfx.SubFormat != KSDATAFORMAT_SUBTYPE_IEEE_FLOAT)
    {
        m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;  // not a format we can handle
        return;
    }

    // Find chunk header for wave data. Skip past any other kinds of chunks.
    ChunkHeader chunkHdr;   // buffer for chunk header
    for (;;)
    {
        // Remember the file position of the data chunk (which this might be
        // -- we don't know yet). That way, if we need to play the file more
        // than once, we won't have to parse the header again each time.
        if (fgetpos(m_pFile, &m_dataChunkPosition) != 0)
        {
            m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;  // not a valid wave format
            return;
        }

        // Read header at start of next chunk of file.
        if (fread(&chunkHdr, sizeof(ChunkHeader), 1, m_pFile) != 1)
        {
            m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;  // not a valid wave file
            return;
        }
        if (chunkHdr.ChunkType == FOURCC("data"))
        {
            break;  // found start of data chunk
        }
        // This is *not* a data chunk. Skip this chunk and go to the next chunk.
        if (fseek(m_pFile, chunkHdr.ChunkSize, SEEK_CUR) != 0)
        {
            m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;  // not a valid wave file
            return;
        }
    }

    // We've found the start of the data chunk. We're ready to start
    // playing wave data...
    m_totalDataBytes = chunkHdr.ChunkSize;
    m_dataBytesRemaining = m_totalDataBytes;
    if (m_totalDataBytes == 0)
    {
        m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;
    }
}

//
// Destructor
//
WaveFileReader::~WaveFileReader()
{
    if (m_pFile)
    {
        fclose(m_pFile);
    }
}

//
// Reset the file pointer to the start of the wave data.
//
HRESULT WaveFileReader::ResetDataPosition()
{
    if (m_hrStreamStatus != S_OK)
    {
        return m_hrStreamStatus;  // oops -- can't read from this file
    }

    // Move to the header info at the start of the data chunk.
    if (fsetpos(m_pFile, &m_dataChunkPosition) != 0)
    {
        m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;  // not a valid wave file
        return AUDCLNT_E_UNSUPPORTED_FORMAT;
    }

    // Read the header for the data chunk.
    ChunkHeader chunkHdr;
    if (fread(&chunkHdr, sizeof(chunkHdr), 1, m_pFile) != 1)
    {
        m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;  // possible file corruption
        return AUDCLNT_E_UNSUPPORTED_FORMAT;
    }

    // Sanity check: The chunk header shouldn't have changed.
    if (chunkHdr.ChunkType != FOURCC("data") ||
        chunkHdr.ChunkSize != m_totalDataBytes)
    {
        m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;  // possible file corruption
        return AUDCLNT_E_UNSUPPORTED_FORMAT;
    }
    m_dataBytesRemaining = m_totalDataBytes;

    // At this point, the file pointer is positioned at
    // the beginning of the wave data.
    return S_OK;
}

//
// Load next block of wave data from file into playback buffer.
// In repeat mode, when we reach the end of the wave data in the
// file, we just reset the file pointer back to the start of the
// data and continue filling the caller's buffer until it is full.
// In single-play mode, once we reach the end of the wave data in
// the file, we just fill the buffer with silence instead of with
// real data.
//
HRESULT WaveFileReader::LoadDataBytes(BYTE *pBuffer, ULONG NumBytes)
{
    if (m_hrStreamStatus != S_OK)
    {
        return m_hrStreamStatus;
    }
    if (pBuffer == NULL)
    {
        return E_POINTER;
    }
    if (NumBytes == 0)
    {
        return E_INVALIDARG;
    }

    BYTE *pCurrent = (BYTE*)pBuffer;
    ULONG numBytesToCopy = NumBytes;

    while (numBytesToCopy > m_dataBytesRemaining)
    {
        if (fread(pCurrent, 1, m_dataBytesRemaining, m_pFile) != m_dataBytesRemaining)
        {
            m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;  // not a valid wave file
            return AUDCLNT_E_UNSUPPORTED_FORMAT;
        }
        pCurrent += m_dataBytesRemaining;
        numBytesToCopy -= m_dataBytesRemaining;
        m_dataBytesRemaining = 0;

        // The file pointer now sits at the end of the data chunk.
        // Are we operating in repeat mode?
        if (m_repeatMode == FALSE)
        {
            // Nope, we're operating in single-play mode. Fill
            // the rest of the buffer with silence and return.
            BYTE silence = (m_wfx.Format.wBitsPerSample==8) ? 0x80 : 0;
            memset(pCurrent, silence, numBytesToCopy);
            return S_OK;  // yup, we're done
        }
        // Yes, we're operating in repeat mode, so loop back to
        // the start of the wave data in the file's data chunk
        // and continue loading data into the caller's buffer.
        if (ResetDataPosition() != S_OK)
        {
            return m_hrStreamStatus;  // not a valid wave file
        }
    }

    assert(numBytesToCopy > 0);
    assert(numBytesToCopy <= m_dataBytesRemaining);

    // The remainder of the data chunk is big enough to
    // completely fill the remainder of the caller's buffer.
    if (fread(pBuffer, 1, numBytesToCopy, m_pFile) != numBytesToCopy)
    {
        m_hrStreamStatus = AUDCLNT_E_UNSUPPORTED_FORMAT;  // not a valid wave file
        return AUDCLNT_E_UNSUPPORTED_FORMAT;
    }
    m_dataBytesRemaining -= numBytesToCopy;
    pCurrent += numBytesToCopy;

    return S_OK;
}
