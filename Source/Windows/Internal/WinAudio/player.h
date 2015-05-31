// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

//-----------------------------------------------------------
//
// player.h
//
//   Player class. A Player object plays a stream from a
//   wave file, tone generator, or capture device.
//
//-------------------------------------------------------

#undef INITGUID

#include <wtypes.h>
#include <winerror.h>
#include <mmdeviceapi.h>
#include <audioclient.h>
#include <audiopolicy.h>

// Audio playback threads launched by Player.
extern DWORD WINAPI PlayWaveStream(LPVOID);
extern DWORD WINAPI PlayCaptureStream(LPVOID);

// Enumeration: audio sources
typedef enum
{
    eNullSource = 0,
    eToneGenerator,
    eWaveFile,
    eCaptureEndpoint

} EAudioSourceType;


// Notification callbacks from Player to client.
// The client implements the callback methods.
class PlayerCallbacks
{
public:
    // Player calls this method when playback volume level changes.
    virtual void VolumeChangeCallback(float volume, BOOL mute) = 0;

    // Player calls this method when playback stream stops.
    virtual void PlayerStopCallback() = 0;

    // Player calls this method when capture device is unplugged.
    virtual void CaptureDisconnectCallback() = 0;

    // Player calls this method when rendering device is unplugged.
    virtual void RenderDisconnectCallback() = 0;
};


//
// Player class
//   A Player object plays audio streams. The Player object
//   (implemented in source file player.cpp) and its audio playback
//   threads (in playwave.cpp and capture.cpp) are the only code
//   modules in this sample that call the methods in the MMDevice
//   API and WASAPI.
//
class Player
{
    HWND m_hDlg;
    IMMDeviceEnumerator *m_pEnumerator;
    BOOL m_ExclusiveMode;
    BOOL m_RepeatMode;
    IMMDeviceCollection *m_pRenderCollection;
    IMMDeviceCollection *m_pCaptureCollection;
    IMMDevice *m_pDeviceOut;
    IMMDevice *m_pDeviceIn;
    IAudioClient *m_pClientOut;
    IAudioClient *m_pClientIn;
    EAudioSourceType m_audioSourceType;
    WCHAR m_szFileName[256];
    HANDLE m_hThread;
    GUID *m_pEventContext;
    IAudioSessionControl *m_pRenderSessionControl;
    IAudioSessionControl *m_pCaptureSessionControl;
    IAudioSessionEvents *m_pRenderSessionEvents;
    IAudioSessionEvents *m_pCaptureSessionEvents;

    // Monitored by audio stream threads
    BOOL m_keepPlaying;

    // Audio playback threads and session event notifications
    friend DWORD WINAPI PlayWaveStream(LPVOID);
    friend DWORD WINAPI PlayCaptureStream(LPVOID);
    friend class CAudioSessionEvents;

    // Notification callbacks from Player to client
    PlayerCallbacks *m_pPlayerCallbacks;

    // Private methods
    void _GetDeviceName(IMMDevice *pDevice, LPWSTR szBuffer, int bufferLen);
    void _RegisterNotificationCallbacks(IMMDevice *pDevice, EDataFlow dir);

public:
    Player(HWND hDlg);
    ~Player();

    BOOL IsApiSupported() { return m_pEnumerator != NULL; };
    void SetExclusiveMode(BOOL enable) { m_ExclusiveMode = enable; };
    void SetRepeatMode(BOOL enable) { m_RepeatMode = enable; };

    void RefreshDeviceList(EDataFlow dir);
    int GetDeviceListCount(EDataFlow dir);
    void GetListDeviceName(EDataFlow dir, int index, LPWSTR szBuffer, int bufferLen);
    BOOL SelectDeviceFromList(EDataFlow dir, int index);

    BOOL SelectDefaultDevice(EDataFlow dir, ERole role);
    void GetSelectedDeviceName(EDataFlow dir, LPWSTR szBuffer, int bufferLen);

    void SetWaveFile(WCHAR *pFileName);
    BOOL Play(EAudioSourceType type);
    BOOL Stop();

    void SetVolume(float volume);
    void SetPlayerCallbacks(PlayerCallbacks *pCallbacks);
};

