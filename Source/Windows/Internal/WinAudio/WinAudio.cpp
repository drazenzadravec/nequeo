// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

//------------------------------------------------------------
//
// WinAudio.cpp
//
//   Implements the graphical user interface for the
//   "WinAudio" Windows audio sample.
//
//------------------------------------------------------------

#include <windows.h>
#include <process.h>
#include <commctrl.h>
#include "resource.h"
#include "player.h"

// Suppress compiler warning C4311
//   'type cast' : pointer truncation from 'HICON' to 'LONG'
#pragma warning (disable: 4311)

#define MAX_VOLUME_LEVEL  1000

BOOL CALLBACK DlgProc(HWND, UINT, WPARAM, LPARAM);

HINSTANCE g_hInst = NULL;
HWND g_hDlg = NULL;

//
// The Player object calls the methods in this class to
// notify the application when certain audio events occur.
//
class CPlayerCallbacks : public PlayerCallbacks
{
    // Notification callback for volume change. Typically, the user
    // adjusts the volume through the SndVol.exe application.
    void VolumeChangeCallback(float volume, BOOL mute)
    {
        EnableWindow(GetDlgItem(g_hDlg, IDC_SLIDER_VOLUME), TRUE);
        SetWindowText(GetDlgItem(g_hDlg, IDC_STATIC_MUTE),
                      (mute == TRUE) ? L"Mute" : L"");
        PostMessage(GetDlgItem(g_hDlg, IDC_SLIDER_VOLUME),
                    TBM_SETPOS, TRUE, LPARAM(volume*MAX_VOLUME_LEVEL));
    };

    // Notification callback for when stream stops playing unexpectedly
    // (typically, because the player reached the end of a wave file).
    void PlayerStopCallback()
    {
        SetActiveWindow(g_hDlg);
        PostMessage(GetDlgItem(g_hDlg, IDC_BUTTON_STOP), BM_CLICK, 0, 0);
    };

    // Notification callback for when the endpoint capture device is
    // disconnected (for example, the user pulls out the microphone plug).
    void CaptureDisconnectCallback()
    {
        SetWindowText(GetDlgItem(g_hDlg, IDC_STATIC_LASTACTION),
                      L"Capture device disconnected!");
        SendMessage(GetDlgItem(g_hDlg, IDC_COMBO_CAPTUREDEVICE), CB_RESETCONTENT, 0, 0);
    };

    // Notification callback for when the endpoint rendering device is
    // disconnected (for example, the user pulls out the headphones plug).
    void RenderDisconnectCallback()
    {
        SetWindowText(GetDlgItem(g_hDlg, IDC_STATIC_LASTACTION),
                      L"Playback device disconnected!");
        EnableWindow(GetDlgItem(g_hDlg, IDC_SLIDER_VOLUME), FALSE);
        SetWindowText(GetDlgItem(g_hDlg, IDC_STATIC_MUTE), L"Disconnected");
        SendMessage(GetDlgItem(g_hDlg, IDC_COMBO_RENDERDEVICE), CB_RESETCONTENT, 0, 0);
        SendMessage(GetDlgItem(g_hDlg, IDC_SLIDER_VOLUME), TBM_SETPOS, TRUE, 0);
    };
};


//------------------------------------------------------------
//
//  Windows main program entry point
//
//------------------------------------------------------------
int APIENTRY WinMain(HINSTANCE hInstance,
                     HINSTANCE hPrevInstance,
                     LPSTR lpCmdLine,
                     int nCmdShow)
{
    static const INITCOMMONCONTROLSEX commonCtrls = {
        sizeof(INITCOMMONCONTROLSEX),
        ICC_STANDARD_CLASSES | ICC_BAR_CLASSES
    };

    if (hPrevInstance)
        return(FALSE);

    g_hInst = hInstance;
    InitCommonControlsEx(&commonCtrls);
    DialogBox(hInstance, L"WINAUDIO", NULL, (DLGPROC)DlgProc);

    return 0;
}


//
//  DlgProc
//
BOOL CALLBACK DlgProc(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    static BOOL firstTime = TRUE;
    static Player *pPlayer = NULL;
    static CPlayerCallbacks *pCallbacks = NULL;
    static EAudioSourceType audioSource = eToneGenerator;
    static BOOL exclusiveMode = FALSE;
    static BOOL repeatMode = FALSE;
    static HWND hwndText = NULL;
    static HWND hwndMute = NULL;

    WCHAR szDeviceName[MAX_PATH];  // temp string buffer
    LRESULT nVolume = 0;
    LRESULT nCheckState = 0;
    int nControlId = 0;

    switch (message)
    {
    case WM_INITDIALOG:
        if (firstTime == TRUE)
        {
            firstTime = FALSE;
            g_hDlg = hDlg;

            // Create instance of audio stream player.
            pPlayer = new Player(hDlg);
            if (pPlayer == NULL)
            {
                MessageBox(hDlg, L"Out of memory",
                           L"Terminating WinAudio Program", MB_OK);
                EndDialog(hDlg, TRUE);
                return TRUE;
            }
            if (pPlayer->IsApiSupported() == FALSE)
            {
                MessageBox(hDlg, L"This program runs only in Windows Vista",
                           L"Terminating WinAudio Program", MB_OK);
                delete pPlayer;
                pPlayer = NULL;
                EndDialog(hDlg, TRUE);
                return TRUE;
            }

            // Set up notification callbacks from player.
            pCallbacks = new CPlayerCallbacks();
            if (pCallbacks == NULL)
            {
                MessageBox(hDlg, L"Out of memory",
                           L"Terminating WinAudio Program", MB_OK);
                delete pPlayer;
                pPlayer = NULL;
                EndDialog(hDlg, TRUE);
                return TRUE;
            }
            pPlayer->SetPlayerCallbacks(pCallbacks);

            // Load icons for dialog box.
            SetClassLong(hDlg, GCL_HICON,
                         LONG(LoadIcon(g_hInst, MAKEINTRESOURCE(IDI_LARGEICON))));
            SetClassLong(hDlg, GCL_HICONSM,
                         LONG(LoadIcon(g_hInst, MAKEINTRESOURCE(IDI_SMALLICON))));

            // Initialize controls in dialog box.
            SendDlgItemMessage(hDlg, IDC_RADIO_SHARED,
                               BM_SETCHECK, BST_CHECKED, 0);
            SendDlgItemMessage(hDlg, IDC_RADIO_TONEGEN,
                               BM_SETCHECK, BST_CHECKED, 0);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_STOP), FALSE);
            SendDlgItemMessage(hDlg, IDC_SLIDER_VOLUME, TBM_SETRANGEMIN,
                               FALSE, 0);
            SendDlgItemMessage(hDlg, IDC_SLIDER_VOLUME, TBM_SETRANGEMAX,
                               FALSE, MAX_VOLUME_LEVEL);
            EnableWindow(GetDlgItem(hDlg, IDC_SLIDER_VOLUME), FALSE);
            hwndText = GetDlgItem(hDlg, IDC_STATIC_LASTACTION);
            hwndMute = GetDlgItem(hDlg, IDC_STATIC_MUTE);
            return TRUE;
        }
        break;
    case WM_HSCROLL:
        // Volume control slider in Playback Volume box
        switch (LOWORD(wParam))
        {
        case SB_THUMBPOSITION:
            nVolume = SendDlgItemMessage(hDlg, IDC_SLIDER_VOLUME,
                                         TBM_GETPOS, 0, 0);
            pPlayer->SetVolume((float)nVolume / MAX_VOLUME_LEVEL);
            SetWindowText(hwndText, L"Change volume control");
            return TRUE;
        case SB_THUMBTRACK:
            nVolume = SendDlgItemMessage(hDlg, IDC_SLIDER_VOLUME,
                                         TBM_GETPOS, 0, 0);
            pPlayer->SetVolume((float)nVolume / MAX_VOLUME_LEVEL);
            SetWindowText(hwndText, L"Adjust volume control");
            return TRUE;
        }
        break;

    case WM_COMMAND:
        switch (nControlId = (int)LOWORD(wParam))
        {
        case IDC_BUTTON_PLAY:
            // Play button  (in Player Controls box)
            if (pPlayer->Play(audioSource) == FALSE)
            {
                MessageBeep(-1);
                return TRUE;
            }
            // Disable controls while stream plays.
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_PLAY), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_STOP), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_RADIO_SHARED), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_RADIO_EXCLUSIVE), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_CONSOLE_OUT), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_MMEDIA_OUT), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_COMMUN_OUT), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_COMBO_RENDERDEVICE), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_RADIO_TONEGEN), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_RADIO_WAVEFILE), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_CHECK_REPEAT), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_MMEDIA_OUT), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_COMMUN_OUT), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_RADIO_CAPTUREDEVICE), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_SELECTFILE), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_CONSOLE_IN), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_MMEDIA_IN), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_COMMUN_IN), FALSE);
            EnableWindow(GetDlgItem(hDlg, IDC_COMBO_CAPTUREDEVICE), FALSE);
            SetWindowText(hwndText, L"Hit Play button");
            return TRUE;
        case IDC_BUTTON_STOP:
            // Stop button in Player Controls box
            pPlayer->Stop();
            // Enable controls now that stream is no longer playing.
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_PLAY), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_STOP), FALSE);
            //EnableWindow(GetDlgItem(hDlg, IDC_RADIO_SHARED), TRUE);
            //EnableWindow(GetDlgItem(hDlg, IDC_RADIO_EXCLUSIVE), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_CONSOLE_OUT), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_MMEDIA_OUT), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_COMMUN_OUT), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_COMBO_RENDERDEVICE), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_RADIO_TONEGEN), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_RADIO_WAVEFILE), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_CHECK_REPEAT), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_MMEDIA_OUT), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_COMMUN_OUT), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_RADIO_CAPTUREDEVICE), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_SELECTFILE), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_CONSOLE_IN), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_MMEDIA_IN), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_BUTTON_COMMUN_IN), TRUE);
            EnableWindow(GetDlgItem(hDlg, IDC_COMBO_CAPTUREDEVICE), TRUE);
            SetWindowText(hwndText, L"Hit Stop button");
            return TRUE;
        case IDC_RADIO_SHARED:
            // Shared Mode in Device Sharing box
            pPlayer->SetExclusiveMode(FALSE);
            SetWindowText(hwndText, L"Switch to shared mode");
            return TRUE;
        case IDC_RADIO_EXCLUSIVE:
            // Exclusive Mode in Device Sharing box
            pPlayer->SetExclusiveMode(TRUE);
            SetWindowText(hwndText, L"Switch to exclusive mode");
            return TRUE;
        case IDC_BUTTON_CONSOLE_OUT:
            // Console button in Playback Device box
            {
                BOOL success = pPlayer->SelectDefaultDevice(eRender, eConsole);
                pPlayer->GetSelectedDeviceName(eRender, szDeviceName,
                                               sizeof(szDeviceName)/sizeof(szDeviceName)[0]);
                SendDlgItemMessage(hDlg, IDC_COMBO_RENDERDEVICE, CB_RESETCONTENT, 0, 0);
                SendDlgItemMessage(hDlg, IDC_COMBO_RENDERDEVICE, CB_ADDSTRING,
                                   0, (LPARAM)szDeviceName);
                SendDlgItemMessage(hDlg, IDC_COMBO_RENDERDEVICE, CB_SETCURSEL, 0, 0);
            }
            SetWindowText(hwndText, L"Select console device for playback");
            return TRUE;
        case IDC_BUTTON_MMEDIA_OUT:
            // Multimedia button in Playback Device box
            {
                BOOL success = pPlayer->SelectDefaultDevice(eRender, eMultimedia);
                pPlayer->GetSelectedDeviceName(eRender, szDeviceName,
                                               sizeof(szDeviceName)/sizeof(szDeviceName)[0]);
                SendDlgItemMessage(hDlg, IDC_COMBO_RENDERDEVICE, CB_RESETCONTENT, 0, 0);
                SendDlgItemMessage(hDlg, IDC_COMBO_RENDERDEVICE, CB_ADDSTRING,
                                   0, (LPARAM)szDeviceName);
                SendDlgItemMessage(hDlg, IDC_COMBO_RENDERDEVICE, CB_SETCURSEL, 0, 0);
                SetWindowText(hwndText, L"Select multimedia device for playback");
            }
            return TRUE;
        case IDC_BUTTON_COMMUN_OUT:
            // Communications button in Playback Device box
            {
                BOOL success = pPlayer->SelectDefaultDevice(eRender, eCommunications);
                pPlayer->GetSelectedDeviceName(eRender, szDeviceName,
                                               sizeof(szDeviceName)/sizeof(szDeviceName)[0]);
                SendDlgItemMessage(hDlg, IDC_COMBO_RENDERDEVICE, CB_RESETCONTENT, 0, 0);
                SendDlgItemMessage(hDlg, IDC_COMBO_RENDERDEVICE, CB_ADDSTRING,
                                   0, (LPARAM)szDeviceName);
                SendDlgItemMessage(hDlg, IDC_COMBO_RENDERDEVICE, CB_SETCURSEL, 0, 0);
            }
            SetWindowText(hwndText, L"Select communications device for playback");
            return TRUE;
        case IDC_COMBO_RENDERDEVICE:
            // Combobox in Playback Device box
            switch (HIWORD(wParam))
            {
            case CBN_DROPDOWN:
                SetWindowText(hwndText, L"Open list of available playback devices");
                SendDlgItemMessage(hDlg, IDC_COMBO_RENDERDEVICE, CB_RESETCONTENT, 0, 0);
                pPlayer->RefreshDeviceList(eRender);
                {
                    int count = pPlayer->GetDeviceListCount(eRender);
                    for (int index = 0; index < count; index++)
                    {
                        pPlayer->GetListDeviceName(eRender, index, szDeviceName,
                                                   sizeof(szDeviceName)/sizeof(szDeviceName)[0]);
                        SendDlgItemMessage(hDlg, IDC_COMBO_RENDERDEVICE, CB_ADDSTRING,
                                           0, (LPARAM)szDeviceName);
                    }
                }
                return TRUE;
            case CBN_SELENDOK:
                SetWindowText(hwndText, L"Select playback device");
                {
                    // Select render device from combobox.
                    LRESULT index = SendDlgItemMessage(hDlg, IDC_COMBO_RENDERDEVICE,
                                                       CB_GETCURSEL, 0, 0);
                    BOOL success = pPlayer->SelectDeviceFromList(eRender, (int)index);
                }
                return TRUE;
            }
            break;
        case IDC_RADIO_TONEGEN:
            // Tone Generator button in Audio Source box
            audioSource = eToneGenerator;
            SetWindowText(hwndText, L"Set source = tone generator");
            return TRUE;
        case IDC_RADIO_WAVEFILE:
            // Wave File button in Audio Source box
            audioSource = eWaveFile;
            SetWindowText(hwndText, L"Set source = wave file");
            return TRUE;
        case IDC_RADIO_CAPTUREDEVICE:
            // Capture Device button in Audio Source box
            audioSource = eCaptureEndpoint;
            SetWindowText(hwndText, L"Set source = capture device");
            return TRUE;
        case IDC_BUTTON_SELECTFILE:
            // Select File button in Wave File box
            {
                // If the Select File button is selected, show the
                // File Open dialog and get the name of the file to play.
                static WCHAR szFileName[MAX_PATH]  = L"";  // file name string
                static WCHAR szDirName[MAX_PATH]   = L"";  // directory string

                WCHAR szFileTitle[MAX_PATH] = L"";  // file title string
                OPENFILENAME ofn;              // dialog box structure

                if (szDirName[0] == L""[0])
                {
                    GetWindowsDirectory(szDirName, MAX_PATH);
                }

                // Set up structure for file dialog
                ZeroMemory(&ofn, sizeof(ofn));
                ofn.lStructSize = sizeof(OPENFILENAME);
                ofn.hwndOwner = hDlg;
                ofn.lpstrFilter = L"Wave File (*.wav)\0*.WAV\0";
                ofn.lpstrCustomFilter = NULL;
                ofn.nFilterIndex = 1;
                ofn.lpstrFile = szFileName;
                ofn.nMaxFile = MAX_PATH;
                ofn.lpstrFileTitle = szFileTitle;
                ofn.nMaxFileTitle = MAX_PATH;
                ofn.lpstrInitialDir = szDirName;
                ofn.lpstrTitle = L"Select Wave File to Play";
                ofn.lpstrDefExt = L"wav";
                ofn.Flags = OFN_SHAREAWARE | OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST;

                // Put up Open File dialog to get file name.
                if (GetOpenFileName(&ofn))
                {
                    pPlayer->SetWaveFile(szFileName);
                    SendDlgItemMessage(hDlg, IDC_EDIT_FILENAME, WM_SETTEXT, 0,
                                       (LPARAM)ofn.lpstrFileTitle);
                }
                SetWindowText(hwndText, L"Selected wave file");
            }
            // Clicking Select File button implicitly selects wave file as audio source.
            audioSource = eWaveFile;
            CheckRadioButton(hDlg, IDC_RADIO_TONEGEN,
                             IDC_RADIO_CAPTUREDEVICE, IDC_RADIO_WAVEFILE);
            return TRUE;
        case IDC_CHECK_REPEAT:
            // Repeat check button in Wave File box
            nCheckState = SendDlgItemMessage(hDlg, IDC_CHECK_REPEAT,
                                             BM_GETCHECK, 0, 0);
            pPlayer->SetRepeatMode((BOOL)nCheckState);
            if (nCheckState == 0)
            {
                SetWindowText(hwndText, L"Disable repeat mode");
            }
            else
            {
                SetWindowText(hwndText, L"Enable repeat mode");
            }
            // Clicking Repeat button implicitly selects wave file as audio source.
            audioSource = eWaveFile;
            CheckRadioButton(hDlg, IDC_RADIO_TONEGEN, IDC_RADIO_CAPTUREDEVICE,
                             IDC_RADIO_WAVEFILE);
            return TRUE;
        case IDC_BUTTON_CONSOLE_IN:
            // Console button in Capture Device box
            pPlayer->SelectDefaultDevice(eCapture, eConsole);
            pPlayer->GetSelectedDeviceName(eCapture, szDeviceName,
                                           sizeof(szDeviceName)/sizeof(szDeviceName)[0]);
            SendDlgItemMessage(hDlg, IDC_COMBO_CAPTUREDEVICE, CB_RESETCONTENT, 0, 0);
            SendDlgItemMessage(hDlg, IDC_COMBO_CAPTUREDEVICE, CB_ADDSTRING,
                               0, (LPARAM)szDeviceName);
            SendDlgItemMessage(hDlg, IDC_COMBO_CAPTUREDEVICE, CB_SETCURSEL, 0, 0);

            // Clicking Console button implicitly selects capture device as audio source.
            audioSource = eCaptureEndpoint;
            CheckRadioButton(hDlg, IDC_RADIO_TONEGEN, IDC_RADIO_CAPTUREDEVICE,
                             IDC_RADIO_CAPTUREDEVICE);
            SetWindowText(hwndText, L"Select console device for capture");
            return TRUE;
        case IDC_BUTTON_MMEDIA_IN:
            // Multimedia button in Capture Device box
            pPlayer->SelectDefaultDevice(eCapture, eMultimedia);
            pPlayer->GetSelectedDeviceName(eCapture, szDeviceName,
                                           sizeof(szDeviceName)/sizeof(szDeviceName)[0]);
            SendDlgItemMessage(hDlg, IDC_COMBO_CAPTUREDEVICE, CB_RESETCONTENT, 0, 0);
            SendDlgItemMessage(hDlg, IDC_COMBO_CAPTUREDEVICE, CB_ADDSTRING,
                               0, (LPARAM)szDeviceName);
            SendDlgItemMessage(hDlg, IDC_COMBO_CAPTUREDEVICE, CB_SETCURSEL, 0, 0);

            // Clicking Multimedia button implicitly selects capture device as audio source.
            audioSource = eCaptureEndpoint;
            CheckRadioButton(hDlg, IDC_RADIO_TONEGEN, IDC_RADIO_CAPTUREDEVICE,
                             IDC_RADIO_CAPTUREDEVICE);
            SetWindowText(hwndText, L"Select multimedia device for capture");
            return TRUE;
        case IDC_BUTTON_COMMUN_IN:
            // Communications button in Capture Device box
            pPlayer->SelectDefaultDevice(eCapture, eCommunications);
            pPlayer->GetSelectedDeviceName(eCapture, szDeviceName,
                                           sizeof(szDeviceName)/sizeof(szDeviceName)[0]);
            SendDlgItemMessage(hDlg, IDC_COMBO_CAPTUREDEVICE, CB_RESETCONTENT, 0, 0);
            SendDlgItemMessage(hDlg, IDC_COMBO_CAPTUREDEVICE, CB_ADDSTRING,
                               0, (LPARAM)szDeviceName);
            SendDlgItemMessage(hDlg, IDC_COMBO_CAPTUREDEVICE, CB_SETCURSEL, 0, 0);
            // Clicking Communication button implicitly selects capture device as audio source.
            audioSource = eCaptureEndpoint;
            CheckRadioButton(hDlg, IDC_RADIO_TONEGEN, IDC_RADIO_CAPTUREDEVICE,
                             IDC_RADIO_CAPTUREDEVICE);
            SetWindowText(hwndText, L"Select communications device for capture");
            return TRUE;
        case IDC_COMBO_CAPTUREDEVICE:
            // Combobox in Capture Device box.
            switch (HIWORD(wParam))
            {
            case CBN_DROPDOWN:
                SendDlgItemMessage(hDlg, IDC_COMBO_CAPTUREDEVICE, CB_RESETCONTENT, 0, 0);
                pPlayer->RefreshDeviceList(eCapture);
                {
                    int count = pPlayer->GetDeviceListCount(eCapture);
                    for (int index = 0; index < count; index++)
                    {
                        pPlayer->GetListDeviceName(eCapture, index, szDeviceName,
                                                   sizeof(szDeviceName)/sizeof(szDeviceName)[0]);
                        SendDlgItemMessage(hDlg, IDC_COMBO_CAPTUREDEVICE, CB_ADDSTRING,
                                           0, (LPARAM)szDeviceName);
                    }
                }
                SetWindowText(hwndText, L"Open list of available capture devices");
                return TRUE;
            case CBN_SELENDOK:
                {
                    // Select capture device from combobox.
                    LRESULT index = SendDlgItemMessage(hDlg, IDC_COMBO_CAPTUREDEVICE,
                                                   CB_GETCURSEL, 0, 0);
                    pPlayer->SelectDeviceFromList(eCapture, (int)index);
                }
                audioSource = eCaptureEndpoint;
                CheckRadioButton(hDlg, IDC_RADIO_TONEGEN,
                                 IDC_RADIO_CAPTUREDEVICE, IDC_RADIO_CAPTUREDEVICE);
                SetWindowText(hwndText, L"Select capture device");
                return TRUE;
            }
            break;
        case IDCANCEL:
            if (pPlayer != NULL)
            {
                delete pPlayer;
                pPlayer = NULL;
            }
            if (pCallbacks != NULL)
            {
                delete pCallbacks;
                pCallbacks = NULL;
            }
            EndDialog(hDlg, TRUE);
            return TRUE;
        }
        break;
    }
    return FALSE;
}

